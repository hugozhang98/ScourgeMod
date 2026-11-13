'use strict';

const fs = require('fs');
const path = require('path');
const { spawn } = require('child_process');
const readline = require('readline/promises');
const { readConfiguredTModLoaderPath, tModLoaderLaunchOptions } = require('./tmodloader-config');

const projectDirectory = path.resolve(__dirname, '..');

function parseOptions(args) {
  const options = { mode: undefined, dryRun: false, help: false };
  for (const argument of args) {
    if (argument === '--dry-run') {
      options.dryRun = true;
    } else if (argument === '--help' || argument === '-h') {
      options.help = true;
    } else if (['client', 'server'].includes(argument) && !options.mode) {
      options.mode = argument;
    } else if (argument !== '--') {
      throw new Error(`未知参数：${argument}。运行 yarn start --help 查看用法。`);
    }
  }
  return options;
}

function run(program, args, options = {}) {
  return new Promise((resolve, reject) => {
    const child = spawn(program, args, { cwd: projectDirectory, stdio: 'inherit', shell: false, ...options });
    child.once('error', reject);
    child.once('exit', (code, signal) => {
      if (code === 0) resolve();
      else reject(new Error(`进程退出：${signal || code}`));
    });
  });
}

async function chooseMode() {
  if (!process.stdin.isTTY) {
    throw new Error('请指定启动目标：yarn start client 或 yarn start server。');
  }
  const terminal = readline.createInterface({ input: process.stdin, output: process.stdout });
  try {
    console.log('1. 客户端（VS Code 断点调试）\n2. 服务器（非调试运行）');
    while (true) {
      const answer = (await terminal.question('请选择 [1/2]：')).trim();
      if (['1', 'client'].includes(answer)) return 'client';
      if (['2', 'server'].includes(answer)) return 'server';
      console.log('请输入 1 或 2。');
    }
  } finally {
    terminal.close();
  }
}

async function main() {
  const options = parseOptions(process.argv.slice(2));
  if (options.help) {
    console.log(`用法：
  yarn start                    选择启动目标
  yarn start client             启动客户端断点调试（同 yarn start:client）
  yarn start server             启动服务器普通运行（同 yarn start:server）
  yarn start server --dry-run    查看启动配置，不打开游戏

支持 Windows 和 macOS。安装路径读取 tModLoader.local.props。
客户端由 VS Code 编译并启动调试，首次运行会安装项目内的本地启动扩展。
服务器直接运行已有模组，读取 serverconfig.local.txt；输入 exit 保存并退出。`);
    return;
  }
  const mode = options.mode || await chooseMode();
  const tModLoaderPath = readConfiguredTModLoaderPath(path.join(projectDirectory, 'tModLoader.local.props'));
  if (!tModLoaderPath || tModLoaderPath.includes('REPLACE_WITH_YOUR_TMODLOADER_PATH')) {
    throw new Error('请先运行 yarn setup:tmodloader，配置 tModLoader.local.props。');
  }
  const launch = tModLoaderLaunchOptions(tModLoaderPath);
  if (!fs.existsSync(launch.args[0])) {
    throw new Error(`找不到 tModLoader.dll，请检查安装路径：${tModLoaderPath}`);
  }

  if (mode === 'client') {
    if (options.dryRun) {
      console.log(JSON.stringify({ mode, debugger: 'VS Code / coreclr', configuration: 'Debug tModLoader Client', ...launch, args: [...launch.args, '-nosteam'] }, null, 2));
      return;
    }
    await run(process.execPath, [path.join(__dirname, 'setup-tmodloader.js')]);
    const { startClientDebugging } = require('./vscode-debug');
    await startClientDebugging(projectDirectory);
    return;
  }

  const serverConfigPath = path.join(projectDirectory, 'serverconfig.local.txt');
  if (!fs.existsSync(serverConfigPath)) {
    throw new Error('找不到 serverconfig.local.txt。请运行 yarn setup:tmodloader，并填写 world 路径。');
  }
  const args = [...launch.args, '-server', '-nosteam', '-config', serverConfigPath];
  if (options.dryRun) {
    console.log(JSON.stringify({ mode, debug: false, build: false, ...launch, args }, null, 2));
    return;
  }
  console.log('启动服务器（非调试，不重新编译）。输入 exit 保存世界并退出。');
  await run(launch.program, args, { cwd: launch.cwd, env: { ...process.env, ...launch.env } });
}

main().catch((error) => {
  console.error(`启动失败：${error.message}`);
  process.exitCode = 1;
});
