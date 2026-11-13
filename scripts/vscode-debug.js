'use strict';

const fs = require('fs');
const os = require('os');
const path = require('path');
const http = require('http');
const { randomBytes } = require('crypto');
const { pathToFileURL } = require('url');
const { execFile, execFileSync } = require('child_process');
const { promisify } = require('util');

const execute = promisify(execFile);
const extensionDirectory = path.join(__dirname, 'vscode-debug-extension');
const manifest = require('./vscode-debug-extension/package.json');
const extensionId = `${manifest.publisher}.${manifest.name}`;

function findVsCode() {
  const candidates = [process.env.VSCODE_GIT_ASKPASS_NODE];
  try {
    const locations = execFileSync(process.platform === 'win32' ? 'where.exe' : 'which', ['code'], { encoding: 'utf8', stdio: ['ignore', 'pipe', 'ignore'] });
    candidates.push(...locations.trim().split(/\r?\n/));
  } catch {
    // VS Code 未加入 PATH 时，继续检查常见安装位置。
  }
  if (process.platform === 'darwin') {
    for (const root of ['/Applications', path.join(os.homedir(), 'Applications')]) {
      candidates.push(path.join(root, 'Visual Studio Code.app', 'Contents', 'MacOS', 'Code'));
    }
  } else if (process.platform === 'win32') {
    for (const root of [process.env.LOCALAPPDATA && path.join(process.env.LOCALAPPDATA, 'Programs'), process.env.ProgramFiles, process.env['ProgramFiles(x86)']].filter(Boolean)) {
      candidates.push(path.join(root, 'Microsoft VS Code', 'Code.exe'));
    }
  }
  for (const candidate of candidates.filter(Boolean)) {
    if (!fs.existsSync(candidate)) continue;
    let directory = path.dirname(fs.realpathSync(candidate));
    while (true) {
      const executable = process.platform === 'darwin' ? path.join(directory, 'MacOS', 'Code') : path.join(directory, 'Code.exe');
      const cli = path.join(directory, process.platform === 'darwin' ? 'Resources' : 'resources', 'app', 'out', 'cli.js');
      if (fs.existsSync(executable) && fs.existsSync(cli)) return { executable, cli };
      const parent = path.dirname(directory);
      if (parent === directory) break;
      directory = parent;
    }
  }
  throw new Error('找不到 VS Code。请安装 Visual Studio Code，并将 code 命令加入 PATH。');
}

// 只需打包几个文本文件；使用标准 ZIP 存储格式，避免额外安装打包工具。
function createVsix(files) {
  const entries = [];
  const directory = [];
  let offset = 0;
  for (const [filename, contents] of Object.entries(files)) {
    const name = Buffer.from(filename);
    const data = Buffer.from(contents);
    let crc = 0xffffffff;
    for (const byte of data) {
      crc ^= byte;
      for (let bit = 0; bit < 8; bit += 1) crc = (crc >>> 1) ^ ((crc & 1) ? 0xedb88320 : 0);
    }
    crc = (crc ^ 0xffffffff) >>> 0;
    const local = Buffer.alloc(30);
    local.writeUInt32LE(0x04034b50, 0);
    local.writeUInt16LE(20, 4);
    local.writeUInt16LE(0x800, 6);
    local.writeUInt16LE(0x21, 12);
    local.writeUInt32LE(crc, 14);
    local.writeUInt32LE(data.length, 18);
    local.writeUInt32LE(data.length, 22);
    local.writeUInt16LE(name.length, 26);
    const central = Buffer.alloc(46);
    central.writeUInt32LE(0x02014b50, 0);
    central.writeUInt16LE(20, 4);
    local.copy(central, 6, 4, 30);
    central.writeUInt32LE(offset, 42);
    entries.push(local, name, data);
    directory.push(central, name);
    offset += local.length + name.length + data.length;
  }
  const centralDirectory = Buffer.concat(directory);
  const end = Buffer.alloc(22);
  end.writeUInt32LE(0x06054b50, 0);
  end.writeUInt16LE(directory.length / 2, 8);
  end.writeUInt16LE(directory.length / 2, 10);
  end.writeUInt32LE(centralDirectory.length, 12);
  end.writeUInt32LE(offset, 16);
  return Buffer.concat([...entries, centralDirectory, end]);
}

async function ensureExtension(code) {
  const { stdout } = await code(['--list-extensions', '--show-versions']);
  if (stdout.split(/\r?\n/).some((line) => line.toLowerCase() === `${extensionId}@${manifest.version}`)) return;
  const temporaryDirectory = fs.mkdtempSync(path.join(os.tmpdir(), 'scourge-vscode-start-'));
  try {
    const packagePath = path.join(temporaryDirectory, 'scourge-mod-start.vsix');
    const files = {
      '[Content_Types].xml': '<?xml version="1.0"?><Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types"><Default Extension="json" ContentType="application/json"/><Default Extension="js" ContentType="application/javascript"/><Default Extension="vsixmanifest" ContentType="text/xml"/></Types>',
      'extension.vsixmanifest': `<?xml version="1.0"?><PackageManifest Version="2.0.0" xmlns="http://schemas.microsoft.com/developer/vsx-schema/2011"><Metadata><Identity Language="en-US" Id="${manifest.name}" Version="${manifest.version}" Publisher="${manifest.publisher}"/><DisplayName>${manifest.displayName}</DisplayName><Description xml:space="preserve">Local ScourgeMod debug launcher</Description><Properties><Property Id="Microsoft.VisualStudio.Code.Engine" Value="${manifest.engines.vscode}"/><Property Id="Microsoft.VisualStudio.Code.ExtensionDependencies" Value="ms-dotnettools.csharp"/><Property Id="Microsoft.VisualStudio.Code.ExecutesCode" Value="true"/></Properties></Metadata><Installation><InstallationTarget Id="Microsoft.VisualStudio.Code"/></Installation><Dependencies/><Assets><Asset Type="Microsoft.VisualStudio.Code.Manifest" Path="extension/package.json" Addressable="true"/></Assets></PackageManifest>`,
      'extension/package.json': fs.readFileSync(path.join(extensionDirectory, 'package.json')),
      'extension/extension.js': fs.readFileSync(path.join(extensionDirectory, 'extension.js')),
    };
    fs.writeFileSync(packagePath, createVsix(files));
    console.log('正在安装项目内的 VS Code 启动扩展（仅安装到本机，不发布到远端）……');
    const result = await code(['--install-extension', packagePath, '--force']);
    if (result.stdout.trim()) console.log(result.stdout.trim());
  } finally {
    fs.rmSync(temporaryDirectory, { recursive: true, force: true });
  }
}

async function startClientDebugging(projectDirectory) {
  const { executable, cli } = findVsCode();
  const env = { ...process.env, ELECTRON_RUN_AS_NODE: '1' };
  delete env.NODE_OPTIONS;
  delete env.NODE_REPL_EXTERNAL_MODULE;
  const code = (args) => execute(executable, [cli, ...args], { env, windowsHide: true });
  await ensureExtension(code);
  await code(['--reuse-window', projectDirectory]);

  const token = randomBytes(32).toString('hex');
  const server = http.createServer();
  let timer;
  try {
    await new Promise((resolve, reject) => {
      server.once('error', reject);
      server.listen(0, '127.0.0.1', resolve);
    });
    const result = new Promise((resolve, reject) => {
      timer = setTimeout(() => reject(new Error('等待 VS Code 启动调试超时。请检查 VS Code 的打开链接提示、构建输出和调试控制台。')), 120000);
      server.on('request', (request, response) => {
        if (request.method !== 'POST' || request.url !== `/${token}`) {
          response.writeHead(404).end();
          return;
        }
        let body = '';
        request.on('data', (data) => {
          body += data;
          if (body.length > 4096) request.destroy();
        });
        request.on('end', () => {
          try {
            const reply = JSON.parse(body);
            response.writeHead(200).end();
            if (reply.ok === true) resolve();
            else reject(new Error(reply.message || 'VS Code 未启动调试。'));
          } catch {
            response.writeHead(400).end();
            reject(new Error('VS Code 返回了无效的启动结果。'));
          }
        });
      });
    });
    const uri = new URL(`vscode://${extensionId}/client`);
    uri.searchParams.set('workspace', pathToFileURL(projectDirectory).toString());
    uri.searchParams.set('callback', `http://127.0.0.1:${server.address().port}/${token}`);
    console.log('等待 VS Code 启动客户端调试；首次使用时，请在 VS Code 中允许此扩展打开链接。');
    // 同时等待 CLI 和启动回执，避免把“发出了打开请求”当作调试成功。
    await Promise.all([code(['--open-url', uri.toString()]), result]);
    console.log('客户端调试已启动，可在 VS Code 中使用断点、暂停和单步。');
  } finally {
    clearTimeout(timer);
    server.closeAllConnections();
    server.close();
  }
}

module.exports = { startClientDebugging };
