'use strict';

const readline = require('node:readline');

// 普通 NPC 完全处在同一种介质中。液体倍率作用于本 tick 的位移。
const environments = {
  normal: { name: '空气', gravity: 0.3, maxFallSpeed: 10, moveScale: 1 },
  water: { name: '水', gravity: 0.2, maxFallSpeed: 7, moveScale: 0.5 },
  lava: { name: '熔岩', gravity: 0.2, maxFallSpeed: 7, moveScale: 0.5 },
  honey: { name: '蜂蜜', gravity: 0.1, maxFallSpeed: 4, moveScale: 0.25 },
  shimmer: { name: '微光', gravity: 0.15, maxFallSpeed: 5.5, moveScale: 0.375 },
};

const simulationLimit = 1_000_000;
const ticksPerSecond = 60;
const pixelsPerTile = 16;
const help = `用法
  yarn jump                             按提示逐项输入
  yarn jump <水平速度> <向上起跳速度> [选项]

两项起跳速度都按大小填写，单位为 px/tick。直接粘贴游戏中的带符号速度也会取绝对值。
小数可以写成 3.8 或 3。8。
1 格 = ${pixelsPerTile} px，${ticksPerSecond} tick = 1 秒。
默认使用普通 NPC 在平地上起跳的模型，计算到落回起跳高度的那一 tick。

  --env <名称>          normal（默认）、water、lava、honey、shimmer
  --gravity <数值>      覆盖向下重力，单位 px/tick²，设为 0 时需提供 --ticks
  --max-fall-speed <值> 覆盖最大下落速度，单位 px/tick
  --accel-x <数值>      恒定水平加速度，正值沿起跳方向，负值与其相反，单位 px/tick²，默认 0
  --pre-jump-ticks <非负整数>      起跳前原地等待的帧数，默认 0
  --post-landing-ticks <非负整数>  落地后原地等待的帧数，默认 0
  --ticks <正整数>      起跳后最多观测多少 tick，提前落地时结束（最多 1000000 tick）

设置前摇或后摇时，同时显示腾空平均 X 速度和含前后摇的平均 X 速度。
观测结束时仍在空中，后摇待落地后开始。

示例
  yarn jump 3.8 8
  yarn jump 3.8 8 --env water
  yarn jump 3.8 8 --pre-jump-ticks 20 --post-landing-ticks 25
  yarn jump 3 10 --gravity 0.15 --max-fall-speed 6 --ticks 30

模型假设环境恒定、地面平坦，上方没有障碍。NPC 自定义 AI 对速度的逐帧修改需单独考虑。`;

const numberFields = {
  vx: { label: '水平起跳速度（px/tick，例如 3.8，原地跳填 0）' },
  vy: { label: '向上起跳速度（px/tick，例如 8）', valid: n => n !== 0, range: '应为非零数值' },
  gravity: { label: '向下重力（px/tick²）', valid: n => n >= 0, range: '应大于或等于 0' },
  maxFallSpeed: { label: '下落限速（px/tick）', valid: n => n > 0, range: '应大于 0' },
  accelX: { label: '水平加速度（px/tick²，沿起跳方向为正）' },
  preJumpTicks: {
    label: '起跳前等待帧数（前摇）',
    valid: n => Number.isSafeInteger(n) && n >= 0,
    range: '应为非负整数',
  },
  postLandingTicks: {
    label: '落地后等待帧数（后摇）',
    valid: n => Number.isSafeInteger(n) && n >= 0,
    range: '应为非负整数',
  },
  ticks: {
    label: '起跳后的观测时长（tick）',
    valid: n => Number.isInteger(n) && n >= 1 && n <= simulationLimit,
    range: `应是 1 到 ${simulationLimit} 之间的整数`,
  },
};

function readNumber(field, value) {
  const { label, valid, range } = numberFields[field];
  const text = value?.trim().replaceAll('。', '.');
  const number = Number(text);
  if (!text || !Number.isFinite(number)) {
    throw new Error(`${label} 需要填写有限数值。`);
  }
  if (valid && !valid(number)) throw new Error(`${label}${range}。`);
  if (field === 'vx') return Math.abs(number);
  if (field === 'vy') return -Math.abs(number);
  return number;
}

function readOptions([x, y, ...args]) {
  const vx = readNumber('vx', x);
  const vy = readNumber('vy', y);
  const flags = {
    '--gravity': 'gravity',
    '--max-fall-speed': 'maxFallSpeed',
    '--accel-x': 'accelX',
    '--pre-jump-ticks': 'preJumpTicks',
    '--post-landing-ticks': 'postLandingTicks',
    '--ticks': 'ticks',
  };
  let environment = 'normal';
  const overrides = {};
  for (let i = 0; i < args.length; i += 2) {
    const option = args[i];
    const value = args[i + 1];
    if (option === '--env') {
      if (!Object.hasOwn(environments, value)) {
        throw new Error(`请选择环境 ${Object.keys(environments).join('、')}。`);
      }
      environment = value;
    } else if (Object.hasOwn(flags, option)) {
      const field = flags[option];
      overrides[field] = readNumber(field, value);
    } else {
      throw new Error(`未知选项 ${option}。使用 --help 查看用法。`);
    }
  }

  const options = { vx, vy, accelX: 0, ...environments[environment], ...overrides };
  if (options.gravity === 0 && options.ticks === undefined) {
    throw new Error('零重力下不会自然落地，请用 --ticks 指定观测时长。');
  }
  return options;
}

async function askOptions() {
  const terminal = readline.createInterface({ input: process.stdin, output: process.stdout, crlfDelay: Infinity });
  const lines = terminal[Symbol.asyncIterator]();

  async function ask(prompt, parse) {
    while (true) {
      terminal.setPrompt(prompt + ' ');
      terminal.prompt();
      const { value, done } = await lines.next();
      if (done) throw new Error('输入已结束，跳跃参数尚未填完。');
      try {
        return parse(value.trim());
      } catch (error) {
        console.log(error.message);
      }
    }
  }

  function askNumber(field, fallback) {
    const hint = fallback === undefined ? '' : ` [默认 ${fallback}]`;
    return ask(numberFields[field].label + hint, value =>
      value === '' && fallback !== undefined ? fallback : readNumber(field, value));
  }

  try {
    const vx = await askNumber('vx');
    const vy = await askNumber('vy');
    const presets = Object.values(environments);
    const menu = presets.map((preset, index) => `${index + 1}. ${preset.name}`).join('  ');
    const preset = await ask(`选择环境  ${menu} [默认 1]`, value => {
      if (value === '') return environments.normal;
      const index = presets.findIndex((preset, i) => value === String(i + 1) || value === preset.name);
      if (index < 0) throw new Error('请输入菜单中的环境编号或中文名称。');
      return presets[index];
    });
    const options = { vx, vy, ...preset, accelX: 0 };
    const customize = await ask('自定义物理参数、前后摇或观测时长？[y/N]', value => {
      if (['', 'n', '否'].includes(value.toLowerCase())) return false;
      if (['y', '是'].includes(value.toLowerCase())) return true;
      throw new Error('请输入 y 或 n，或直接回车使用环境默认值。');
    });
    if (customize) {
      for (const field of ['gravity', 'maxFallSpeed', 'accelX']) {
        options[field] = await askNumber(field, options[field]);
      }
      options.preJumpTicks = await askNumber('preJumpTicks', 0);
      options.postLandingTicks = await askNumber('postLandingTicks', 0);
      const allowLanding = options.gravity > 0;
      const hint = allowLanding ? '[回车算到落地]' : '[零重力，请输入 tick 数]';
      options.ticks = await ask(`${numberFields.ticks.label} ${hint}`, value =>
        value === '' && allowLanding ? undefined : readNumber('ticks', value));
    }
    return options;
  } finally {
    terminal.close();
  }
}

function simulateJump(options) {
  const { vx, vy, gravity, maxFallSpeed, accelX, moveScale, ticks } = options;
  const { preJumpTicks = 0, postLandingTicks = 0 } = options;
  let x = 0;
  let y = 0;
  let farthestX = 0;
  let farthestY = 0;

  for (let tick = 1; tick <= (ticks ?? simulationLimit); tick++) {
    const velocityX = vx + accelX * tick;
    const velocityY = Math.min(vy + gravity * tick, maxFallSpeed);
    x += velocityX * moveScale;
    y += velocityY * moveScale;
    if (!Number.isFinite(x) || !Number.isFinite(y)) {
      throw new Error('轨迹超出数值范围，请检查速度和环境参数。');
    }
    // 消除恰好落在地面时的浮点求和误差，避免多计一个水平移动 tick。
    const landed = velocityY >= 0 && y >= -Number.EPSILON * farthestY * tick;
    if (landed) y = 0;
    farthestX = Math.max(farthestX, Math.abs(x));
    farthestY = Math.max(farthestY, Math.abs(y));

    if (landed || tick === ticks) {
      const totalTicks = preJumpTicks + tick + (landed ? postLandingTicks : 0);
      if (!Number.isSafeInteger(totalTicks)) {
        throw new Error('总帧数超出数值范围，请缩短等待帧数。');
      }
      return {
        tick,
        totalTicks,
        landed,
        displacementX: x,
        farthestX,
        farthestY,
      };
    }
  }
  throw new Error(`经过 ${simulationLimit} tick 仍未落地。请检查参数或用 --ticks 指定观测时长。`);
}

function printTable(headers, rows) {
  // 表格文字仅含汉字和 ASCII。终端中每个汉字占两列。
  const width = text => text.replace(/\p{Script=Han}/gu, '  ').length;
  const widths = headers.map((header, i) => Math.max(width(header), ...rows.map(row => width(row[i]))));
  function printRow(row) {
    console.log(row.map((cell, i) => {
      const padding = ' '.repeat(widths[i] - width(cell));
      return i === 0 ? cell + padding : padding + cell;
    }).join('   '));
  }
  printRow(headers);
  console.log(widths.map(size => '-'.repeat(size)).join('   '));
  rows.forEach(printRow);
}

function printResult(options, result) {
  const seconds = result.totalTicks / ticksPerSecond;
  const status = result.landed ? '已落回起跳高度' : '仍在空中';
  const { preJumpTicks = 0, postLandingTicks = 0 } = options;
  const hasWait = preJumpTicks > 0 || postLandingTicks > 0;
  console.log(`\n起跳速度   X ${options.vx}   Y ${Math.abs(options.vy)}（px/tick，Y 向上）`);
  console.log(`跳跃结果 · ${options.name}\n${status}   ${hasWait ? '总计 ' : ''}${result.totalTicks} tick（${seconds.toFixed(2)} 秒）`);
  if (hasWait) {
    const recovery = result.landed || postLandingTicks === 0 ? `${postLandingTicks} 帧` : '待落地';
    console.log(`前摇 ${preJumpTicks} 帧   腾空 ${result.tick} 帧   后摇 ${recovery}`);
  }
  console.log();

  const horizontalSpeeds = [[hasWait ? '腾空平均 X 速度' : '平均 X 速度', result.displacementX / result.tick]];
  if (hasWait) {
    horizontalSpeeds.push(['含前后摇的平均 X 速度', result.displacementX / result.totalTicks]);
  }
  printTable(['速度统计', 'px/tick', 'px/s', '格/s (约)'], horizontalSpeeds.map(([label, speed]) => [
    label,
    speed.toFixed(3),
    (speed * ticksPerSecond).toFixed(2),
    (speed * (ticksPerSecond / pixelsPerTile)).toFixed(2),
  ]));

  console.log();
  const distances = [['X 最远距离', result.farthestX], ['Y 上升高度', result.farthestY]];
  printTable(['距离统计', 'px', '格 (约)'], distances.map(([label, distance]) => [
    label, distance.toFixed(2), (distance / pixelsPerTile).toFixed(2),
  ]));

  console.log(`\n重力 ${options.gravity} px/tick²   下落限速 ${options.maxFallSpeed} px/tick`);
  console.log(`水平加速度 ${options.accelX} px/tick²   位移倍率 ${options.moveScale}`);
}

async function main(args) {
  if (args.includes('--help') || args.includes('-h')) {
    console.log(help);
    return;
  }
  const options = args.length ? readOptions(args) : await askOptions();
  printResult(options, simulateJump(options));
}

main(process.argv.slice(2)).catch(error => {
  console.error(error.message);
  process.exitCode = 1;
});
