'use strict';

const fs = require('fs');
const path = require('path');

function readConfiguredTModLoaderPath(configPath) {
  if (!fs.existsSync(configPath)) {
    return undefined;
  }

  const contents = fs.readFileSync(configPath, 'utf8').replace(/<!--[\s\S]*?-->/g, '');
  const match = contents.match(/<TModLoaderPath>([\s\S]*?)<\/TModLoaderPath>/);
  if (!match || !match[1].trim()) {
    return undefined;
  }

  return match[1].trim()
    .replace(/&quot;/g, '"')
    .replace(/&apos;/g, "'")
    .replace(/&gt;/g, '>')
    .replace(/&lt;/g, '<')
    .replace(/&amp;/g, '&');
}

function tModLoaderLaunchOptions(tModLoaderPath) {
  const isMacOS = process.platform === 'darwin';
  const isWindows = process.platform === 'win32';
  const runtimeDirectories = isMacOS && process.arch === 'arm64'
    ? ['dotnet_arm64', 'dotnet']
    : ['dotnet'];
  const bundledRuntime = runtimeDirectories
    .map((directory) => path.join(tModLoaderPath, directory, isWindows ? 'dotnet.exe' : 'dotnet'))
    .find(fs.existsSync);
  const options = {
    // 优先使用游戏自带的运行时，尚未下载时使用 PATH 中的 dotnet。
    program: bundledRuntime || 'dotnet',
    args: [path.join(tModLoaderPath, 'tModLoader.dll')],
    cwd: tModLoaderPath,
  };

  if (isMacOS) {
    const nativeLibraryPath = path.join(tModLoaderPath, 'Libraries', 'Native', 'OSX');
    options.env = {
      DYLD_LIBRARY_PATH: nativeLibraryPath,
      VK_ICD_FILENAMES: path.join(nativeLibraryPath, 'MoltenVK_icd.json'),
    };
  } else if (isWindows) {
    const nativeLibraryPath = path.join(tModLoaderPath, 'Libraries', 'Native', 'Windows');
    options.env = {
      PATH: `${nativeLibraryPath}${path.delimiter}${process.env.PATH || ''}`,
    };
  }

  return options;
}

module.exports = { readConfiguredTModLoaderPath, tModLoaderLaunchOptions };
