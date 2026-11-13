'use strict';

const fs = require('fs');
const http = require('http');
const vscode = require('vscode');

function canonicalPath(filePath) {
  const resolved = fs.realpathSync(filePath);
  return process.platform === 'win32' ? resolved.toLowerCase() : resolved;
}

function reply(callback, result) {
  const body = JSON.stringify(result);
  const request = http.request(callback, { method: 'POST', headers: { 'Content-Type': 'application/json' }, timeout: 5000 });
  request.on('error', () => {});
  request.on('timeout', () => request.destroy());
  request.on('response', (response) => response.resume());
  request.end(body);
}

function activate(context) {
  context.subscriptions.push(vscode.window.registerUriHandler({
    async handleUri(uri) {
      const query = new URLSearchParams(uri.query);
      let callback;
      try {
        callback = new URL(query.get('callback'));
        if (callback.protocol !== 'http:' || callback.hostname !== '127.0.0.1' || !callback.port || !/^\/[a-f0-9]{64}$/.test(callback.pathname)) {
          throw new Error('无效的本地启动回调。');
        }
      } catch {
        vscode.window.showErrorMessage('请通过项目中的 yarn start client 启动调试。');
        return;
      }

      try {
        if (uri.path !== '/client') throw new Error('未知的启动目标。');
        const requestedFolder = vscode.Uri.parse(query.get('workspace') || '');
        if (requestedFolder.scheme !== 'file') throw new Error('只能启动本地项目。');
        const targetPath = canonicalPath(requestedFolder.fsPath);
        const folder = vscode.workspace.workspaceFolders?.find((item) => canonicalPath(item.uri.fsPath) === targetPath);
        if (!folder) throw new Error('请先在当前 VS Code 窗口打开项目，再运行 yarn start client。');
        if (!vscode.workspace.isTrusted) throw new Error('请先在 VS Code 中信任此项目。');
        const started = await vscode.debug.startDebugging(folder, 'Debug tModLoader Client');
        if (!started) throw new Error('VS Code 未启动客户端调试，请查看构建输出和调试控制台。');
        reply(callback, { ok: true });
      } catch (error) {
        reply(callback, { ok: false, message: error.message });
        vscode.window.showErrorMessage(error.message);
      }
    },
  }));
}

module.exports = { activate };
