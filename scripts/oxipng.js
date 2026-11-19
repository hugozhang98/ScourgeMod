'use strict';

const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { createHash } = require('node:crypto');
const { spawnSync } = require('node:child_process');

const version = '10.2.1';
const downloads = {
  'darwin-x64': {
    target: 'x86_64-apple-darwin.tar.gz',
    sha256: '111883bbe42b25e01cb1ca41f39f8094e4830b173f0e42be342ecc4ca48f3131',
  },
  'darwin-arm64': {
    target: 'aarch64-apple-darwin.tar.gz',
    sha256: '7039fcfc78e8aa1ed2b57d848057a0296f082e92b3e1807ac65402d10d926764',
  },
  'win32-x64': {
    target: 'x86_64-pc-windows-msvc.zip',
    sha256: '7e940f83ee46874b73f53031f96a15834cb70b220af27391fb06fe7b4dd798e1',
  },
  'win32-ia32': {
    target: 'i686-pc-windows-msvc.zip',
    sha256: 'cd7bd71b865d77ff1fdcd81803161276d0d62627de97f2ef7b201c6e0ff34042',
  },
  'linux-x64': {
    target: 'x86_64-unknown-linux-musl.tar.gz',
    sha256: '1813750ef592c5350ca79c88f98b2c0876d05c826dc7156245256d4255c1ad17',
  },
  'linux-arm64': {
    target: 'aarch64-unknown-linux-musl.tar.gz',
    sha256: 'ff21aa2a005b324c8dfbdf13cd2f469fb6728b27dacded82635824ecd12e3b35',
  },
};

const help = `用法
  yarn oxipng <图片或目录> [更多图片或目录]

支持相对路径和绝对路径。相对路径从运行命令时所在目录解析。
目录会递归处理 PNG/APNG，路径包含空格时请加引号。
采用无损压缩，压缩后替换同名原文件，仅在文件变小时写入。
缺少 Oxipng 时，自动下载官方版本到用户缓存，后续执行会复用。

示例
  yarn oxipng Assets
  yarn oxipng Assets/NPC_ViscousFallout.png
  yarn oxipng "/absolute/path/My Images"`;

function isPng(file) {
  return /\.(png|apng)$/i.test(file);
}

function collectImages(input, files) {
  const resolved = fs.realpathSync(input);
  const stat = fs.statSync(resolved);
  if (stat.isFile()) {
    if (!isPng(resolved)) throw new Error(`Oxipng 只支持 PNG/APNG 图片：${input}`);
    files.add(resolved);
    return;
  }
  if (!stat.isDirectory()) throw new Error(`路径不是图片或目录：${input}`);

  for (const entry of fs.readdirSync(resolved, { withFileTypes: true })) {
    const child = path.join(resolved, entry.name);
    if (entry.isDirectory() || (entry.isFile() && isPng(entry.name))) {
      collectImages(child, files);
    }
  }
}

function canRun(command) {
  const result = spawnSync(command, ['--version'], { encoding: 'utf8', timeout: 15000 });
  return !result.error && result.status === 0 && /^oxipng\s/i.test(result.stdout);
}

async function findOxipng() {
  const executable = process.platform === 'win32' ? 'oxipng.exe' : 'oxipng';
  if (canRun(executable)) return executable;

  const platform = `${process.platform}-${process.arch}`;
  const directory = path.join(os.homedir(), '.cache', 'scourge-mod', 'oxipng', version, platform);
  const cached = path.join(directory, executable);
  if (canRun(cached)) return cached;

  const download = downloads[platform];
  if (!download) {
    throw new Error(`当前系统 ${platform} 暂无自动安装包，请将 Oxipng 安装到 PATH 后重试。`);
  }

  console.log(`未检测到 Oxipng，正在安装官方版本 ${version}……`);
  fs.mkdirSync(directory, { recursive: true });
  const temporary = fs.mkdtempSync(path.join(directory, '.install-'));
  try {
    const asset = `oxipng-${version}-${download.target}`;
    const response = await fetch(
      `https://github.com/oxipng/oxipng/releases/download/v${version}/${asset}`,
      { signal: AbortSignal.timeout(120000) },
    );
    if (!response.ok) throw new Error(`下载 Oxipng 失败，HTTP ${response.status}`);
    const bytes = Buffer.from(await response.arrayBuffer());
    if (createHash('sha256').update(bytes).digest('hex') !== download.sha256) {
      throw new Error('Oxipng 安装包 SHA-256 校验失败。');
    }

    const archive = path.join(temporary, asset);
    const extracted = path.join(temporary, 'extracted');
    fs.writeFileSync(archive, bytes);
    fs.mkdirSync(extracted);
    const result = spawnSync('tar', ['-xf', archive, '-C', extracted], { encoding: 'utf8' });
    if (result.error || result.status !== 0) {
      throw new Error(`解压 Oxipng 失败：${result.error?.message || result.stderr.trim()}`);
    }

    const candidates = [path.join(extracted, executable)];
    for (const entry of fs.readdirSync(extracted, { withFileTypes: true })) {
      if (entry.isDirectory()) candidates.push(path.join(extracted, entry.name, executable));
    }
    const binary = candidates.find(candidate => fs.existsSync(candidate));
    if (!binary) throw new Error('官方安装包中未找到 Oxipng 可执行文件。');
    fs.chmodSync(binary, 0o755);
    if (!canRun(binary)) throw new Error('安装包中的 Oxipng 无法在当前系统运行。');

    //发布已验证的文件，并允许另一个运行中的命令先完成安装。
    try {
      fs.renameSync(binary, cached);
    } catch (error) {
      if (!canRun(cached)) throw error;
    }
    console.log(`Oxipng 已安装到 ${cached}`);
    return cached;
  } finally {
    fs.rmSync(temporary, { recursive: true, force: true });
  }
}

function compressImage(command, file) {
  const original = fs.readFileSync(file);
  const temporary = fs.mkdtempSync(path.join(path.dirname(file), '.oxipng-'));
  try {
    const output = path.join(temporary, 'optimized.png');
    const result = spawnSync(command, ['-o', '6', '--preserve', '--out', output, file], {
      encoding: 'utf8',
    });
    if (result.error || result.status !== 0) {
      throw new Error(result.error?.message || result.stderr.trim() || `Oxipng 退出码 ${result.status}`);
    }
    if (!fs.existsSync(output)) return { before: original.length, after: original.length };

    const optimized = fs.statSync(output);
    if (optimized.size >= original.length) return { before: original.length, after: original.length };
    if (!fs.readFileSync(file).equals(original)) {
      throw new Error('压缩期间原文件被其他程序修改，本次替换已取消。');
    }

    //同目录原子替换，压缩失败或中断时原图仍然完整。
    fs.renameSync(output, file);
    return { before: original.length, after: optimized.size };
  } finally {
    fs.rmSync(temporary, { recursive: true, force: true });
  }
}

async function main(args) {
  if (args.includes('--help') || args.includes('-h')) {
    console.log(help);
    return;
  }
  if (args.length === 0) throw new Error('请提供图片或目录。使用 yarn oxipng --help 查看用法。');

  const workingDirectory = process.env.INIT_CWD || process.cwd();
  const files = new Set();
  for (const input of args) collectImages(path.resolve(workingDirectory, input), files);
  if (files.size === 0) throw new Error('指定目录中没有 PNG/APNG 图片。');

  const command = await findOxipng();
  let before = 0;
  let after = 0;
  let failures = 0;
  for (const file of [...files].sort()) {
    try {
      const result = compressImage(command, file);
      before += result.before;
      after += result.after;
      console.log(`${path.relative(workingDirectory, file)}  ${result.before} -> ${result.after} 字节`);
    } catch (error) {
      failures += 1;
      console.error(`${file}  ${error.message}`);
    }
  }
  const reduction = before === 0 ? '0.00' : ((before - after) / before * 100).toFixed(2);
  console.log(`完成 ${files.size - failures}/${files.size} 张，${before} -> ${after} 字节，节省 ${reduction}%。`);
  if (failures > 0) process.exitCode = 1;
}

main(process.argv.slice(2)).catch(error => {
  console.error(error.message);
  process.exitCode = 1;
});
