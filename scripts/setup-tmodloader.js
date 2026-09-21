'use strict';

const fs = require('fs');
const path = require('path');
const { execFileSync } = require('child_process');

const projectDirectory = path.resolve(__dirname, '..');
const configPath = path.join(projectDirectory, 'tModLoader.local.props');
const vscodeDirectory = path.join(projectDirectory, '.vscode');
const vscodeLaunchPath = path.join(vscodeDirectory, 'launch.json');

function printUsage() {
  console.log(`Usage: node scripts/setup-tmodloader.js [options]

Options:
  --tmodloader-path <path>  Use this Steam tModLoader installation directory.
  --force                   Regenerate tModLoader.local.props if it exists.
  --help                    Show this message.`);
}

function parseOptions(args) {
  const options = { force: false };

  for (let index = 0; index < args.length; index += 1) {
    const argument = args[index];

    if (argument === '--force') {
      options.force = true;
    } else if (argument === '--tmodloader-path') {
      const value = args[index + 1];
      if (!value) {
        throw new Error('--tmodloader-path requires a directory.');
      }
      options.tModLoaderPath = value;
      index += 1;
    } else if (argument === '--help' || argument === '-h') {
      printUsage();
      process.exit(0);
    } else {
      throw new Error(`Unknown option: ${argument}`);
    }
  }

  return options;
}

function tModLoaderTargetExists(candidate) {
  return fs.existsSync(path.join(candidate, 'tMLMod.targets'));
}

function windowsRegistrySteamRoots() {
  const queries = [
    ['HKCU\\Software\\Valve\\Steam', 'SteamPath'],
    ['HKLM\\SOFTWARE\\WOW6432Node\\Valve\\Steam', 'InstallPath'],
    ['HKLM\\SOFTWARE\\Valve\\Steam', 'InstallPath'],
  ];
  const roots = [];

  for (const [registryKey, valueName] of queries) {
    try {
      const output = execFileSync('reg', ['query', registryKey, '/v', valueName], {
        encoding: 'utf8',
        stdio: ['ignore', 'pipe', 'ignore'],
      });
      const valueLine = output.split(/\r?\n/).find((line) => line.includes('REG_SZ'));
      const match = valueLine && valueLine.match(/REG_SZ\s+(.+)$/);
      if (match && match[1]) {
        roots.push(match[1].trim());
      }
    } catch {
      // Steam may not have registered this location; the other discovery paths still apply.
    }
  }

  return roots;
}

function steamRoots() {
  const home = process.env.HOME || process.env.USERPROFILE;
  const roots = [];

  if (process.platform === 'win32') {
    for (const programFiles of [process.env['ProgramFiles(x86)'], process.env.ProgramFiles]) {
      if (programFiles) {
        roots.push(path.join(programFiles, 'Steam'));
      }
    }
    roots.push(...windowsRegistrySteamRoots());
  } else if (home) {
    roots.push(
      path.join(home, 'Library', 'Application Support', 'Steam'),
      path.join(home, '.steam', 'steam'),
      path.join(home, '.local', 'share', 'Steam'),
    );
  }

  return [...new Set(roots)];
}

function hasPlaceholderConfig() {
  return fs.existsSync(configPath)
    && fs.readFileSync(configPath, 'utf8').includes('REPLACE_WITH_YOUR_TMODLOADER_PATH');
}

function librariesFromVdf(steamRoot) {
  const libraryFoldersPath = path.join(steamRoot, 'steamapps', 'libraryfolders.vdf');
  if (!fs.existsSync(libraryFoldersPath)) {
    return [];
  }

  const vdf = fs.readFileSync(libraryFoldersPath, 'utf8');
  return [...vdf.matchAll(/"path"\s+"([^"]+)"/g)]
    .map((match) => match[1].replace(/\\\\/g, '\\'));
}

function findTModLoader(options) {
  const candidates = [];
  if (options.tModLoaderPath) {
    candidates.push(options.tModLoaderPath);
  }

  for (const steamRoot of steamRoots()) {
    candidates.push(path.join(steamRoot, 'steamapps', 'common', 'tModLoader'));
    for (const libraryRoot of librariesFromVdf(steamRoot)) {
      candidates.push(path.join(libraryRoot, 'steamapps', 'common', 'tModLoader'));
    }
  }

  return [...new Set(candidates)].find(tModLoaderTargetExists);
}

function escapeXml(value) {
  return value
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&apos;');
}

function writeConfig(tModLoaderPath) {
  const contents = [
    '<Project>',
    '  <PropertyGroup>',
    `    <TModLoaderPath>${escapeXml(tModLoaderPath)}</TModLoaderPath>`,
    '  </PropertyGroup>',
    '</Project>',
    '',
  ].join('\n');

  fs.writeFileSync(configPath, contents, 'utf8');
}

function unescapeXml(value) {
  return value
    .replace(/&quot;/g, '"')
    .replace(/&apos;/g, "'")
    .replace(/&gt;/g, '>')
    .replace(/&lt;/g, '<')
    .replace(/&amp;/g, '&');
}

function configuredTModLoaderPath() {
  if (!fs.existsSync(configPath)) {
    return undefined;
  }

  const contents = fs.readFileSync(configPath, 'utf8');
  const match = contents.match(/<TModLoaderPath>([\s\S]*?)<\/TModLoaderPath>/);
  return match && match[1] ? unescapeXml(match[1].trim()) : undefined;
}

function debugConfiguration(tModLoaderPath) {
  const isMacOS = process.platform === 'darwin';
  const isWindows = process.platform === 'win32';
  const bundledDotNetDirectories = isMacOS && process.arch === 'arm64'
    ? ['dotnet_arm64', 'dotnet']
    : ['dotnet'];
  const bundledDotNetPath = bundledDotNetDirectories
    .map((directory) => path.join(tModLoaderPath, directory, isWindows ? 'dotnet.exe' : 'dotnet'))
    .find(fs.existsSync);
  const configuration = {
    name: 'Debug tModLoader',
    type: 'coreclr',
    request: 'launch',
    preLaunchTask: 'Build ScourgeMod',
    // Prefer tModLoader's bundled runtime. Windows falls back to the SDK/runtime
    // on PATH until Steam has downloaded the bundled runtime on its first launch.
    program: bundledDotNetPath || 'dotnet',
    args: [path.join(tModLoaderPath, 'tModLoader.dll')],
    cwd: tModLoaderPath,
    console: 'integratedTerminal',
    justMyCode: true,
  };

  if (isMacOS) {
    const nativeLibraryPath = path.join(tModLoaderPath, 'Libraries', 'Native', 'OSX');
    configuration.env = {
      DYLD_LIBRARY_PATH: nativeLibraryPath,
      VK_ICD_FILENAMES: path.join(nativeLibraryPath, 'MoltenVK_icd.json'),
    };
  } else if (isWindows) {
    const nativeLibraryPath = path.join(tModLoaderPath, 'Libraries', 'Native', 'Windows');
    configuration.env = {
      PATH: `${nativeLibraryPath}${path.delimiter}${process.env.PATH || ''}`,
    };
  }

  return configuration;
}

function writeVsCodeLaunchConfiguration(tModLoaderPath) {
  fs.mkdirSync(vscodeDirectory, { recursive: true });

  let launchSettings = { version: '0.2.0', configurations: [] };
  if (fs.existsSync(vscodeLaunchPath)) {
    try {
      const parsed = JSON.parse(fs.readFileSync(vscodeLaunchPath, 'utf8'));
      if (Array.isArray(parsed.configurations)) {
        launchSettings = parsed;
      }
    } catch {
      console.log(`Keeping existing VS Code launch configuration because it is not valid JSON: ${vscodeLaunchPath}`);
      return;
    }
  }

  const configuration = debugConfiguration(tModLoaderPath);
  const index = launchSettings.configurations.findIndex((item) => item.name === configuration.name);
  if (index >= 0) {
    launchSettings.configurations[index] = configuration;
  } else {
    launchSettings.configurations.push(configuration);
  }

  fs.writeFileSync(vscodeLaunchPath, `${JSON.stringify(launchSettings, null, 2)}\n`, 'utf8');
  console.log(`Wrote VS Code debug configuration: ${vscodeLaunchPath}`);
}

try {
  const options = parseOptions(process.argv.slice(2));

  let tModLoaderPath;
  if (fs.existsSync(configPath) && !options.force && !hasPlaceholderConfig()) {
    tModLoaderPath = configuredTModLoaderPath();
    console.log(`Keeping existing local configuration: ${configPath}`);
  } else {
    tModLoaderPath = findTModLoader(options);

    if (tModLoaderPath) {
      writeConfig(tModLoaderPath);
      console.log(`Found tModLoader and wrote local configuration: ${tModLoaderPath}`);
    } else {
      writeConfig('REPLACE_WITH_YOUR_TMODLOADER_PATH');
      console.log(`Could not find tModLoader. Generated a template for manual editing: ${configPath}`);
    }
  }

  if (tModLoaderPath && tModLoaderTargetExists(tModLoaderPath)) {
    writeVsCodeLaunchConfiguration(tModLoaderPath);
  }
} catch (error) {
  console.error(error instanceof Error ? error.message : error);
  process.exitCode = 1;
}
