'use strict';

const fs = require('fs');
const path = require('path');
const { spawn } = require('child_process');
const { readConfiguredTModLoaderPath, tModLoaderLaunchOptions } = require('./tmodloader-config');

function parseOptions(args) {
  const options = { count: 1, dryRun: false, help: false };
  for (let index = 0; index < args.length; index += 1) {
    const argument = args[index];
    if (argument === '--count') {
      const value = args[++index];
      if (!value || !/^\d+$/.test(value) || !Number.isSafeInteger(Number(value)) || Number(value) < 1) {
        throw new Error('--count 后需要填写正整数，例如 yarn client --count 2。');
      }
      options.count = Number(value);
    } else if (argument === '--dry-run') {
      options.dryRun = true;
    } else if (argument === '--help' || argument === '-h') {
      options.help = true;
    } else if (argument !== '--') {
      throw new Error(`未知参数：${argument}。运行 yarn client --help 查看用法。`);
    }
  }
  return options;
}

async function main() {
  const options = parseOptions(process.argv.slice(2));
  if (options.help) {
    console.log(`用法：
  yarn client                 新增一个客户端
  yarn client --count 2       新增两个客户端
  yarn client --dry-run       查看启动配置，不打开游戏

安装路径读取自项目根目录的 tModLoader.local.props。
客户端使用 -nosteam 启动，通过 IP 加入服务器。`);
    return;
  }

  const configPath = path.resolve(__dirname, '..', 'tModLoader.local.props');
  const tModLoaderPath = readConfiguredTModLoaderPath(configPath);
  if (!tModLoaderPath || tModLoaderPath.includes('REPLACE_WITH_YOUR_TMODLOADER_PATH')) {
    throw new Error('请先运行 yarn setup:tmodloader，并在 tModLoader.local.props 中配置 TModLoaderPath。');
  }

  const launch = tModLoaderLaunchOptions(tModLoaderPath);
  if (!fs.existsSync(launch.args[0])) {
    throw new Error(`找不到 tModLoader.dll，请检查 tModLoader.local.props 中的安装路径：${tModLoaderPath}`);
  }
  const args = [...launch.args, '-nosteam'];
  if (options.dryRun) {
    console.log(JSON.stringify({ count: options.count, program: launch.program, args, cwd: launch.cwd }, null, 2));
    return;
  }

  for (let index = 0; index < options.count; index += 1) {
    // 直接传递参数数组，路径中的空格、中文和特殊字符无需手动转义。
    const child = spawn(launch.program, args, {
      cwd: launch.cwd,
      env: { ...process.env, ...launch.env },
      detached: true,
      stdio: 'ignore',
      windowsHide: true,
      shell: false,
    });
    await new Promise((resolve, reject) => {
      child.once('error', reject);
      child.once('spawn', resolve);
    });
    child.unref();
    console.log(`已启动客户端进程 ${index + 1}/${options.count}（PID：${child.pid}）。`);
  }
}

main().catch((error) => {
  console.error(`启动失败：${error.message}`);
  process.exitCode = 1;
});
