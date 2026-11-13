'use strict';

const fs = require('fs');
const path = require('path');
const { execFileSync } = require('child_process');
const { readConfiguredTModLoaderPath, tModLoaderLaunchOptions } = require('./tmodloader-config');

const projectDirectory = path.resolve(__dirname, '..');
const configPath = path.join(projectDirectory, 'tModLoader.local.props');
const serverConfigPath = path.join(projectDirectory, 'serverconfig.local.txt');
const serverConfigExamplePath = path.join(projectDirectory, 'serverconfig.example.txt');
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

function debugConfiguration(tModLoaderPath) {
  return {
    name: 'Debug tModLoader',
    type: 'coreclr',
    request: 'launch',
    preLaunchTask: 'Build ScourgeMod',
    ...tModLoaderLaunchOptions(tModLoaderPath),
    console: 'integratedTerminal',
    justMyCode: true,
  };
}

function debugCommandLine(args) {
  // coreclr accepts a command-line string; quote paths for Windows and macOS.
  return args.map((argument) => `"${argument
    .replace(/(\\*)"/g, '$1$1\\"')
    .replace(/(\\+)$/g, '$1$1')}"`).join(' ');
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

  const clientConfiguration = debugConfiguration(tModLoaderPath);
  const directClientConfiguration = {
    ...clientConfiguration,
    name: 'Debug tModLoader Client',
    args: [...clientConfiguration.args, '-nosteam'],
  };
  const serverConfiguration = {
    ...clientConfiguration,
    name: 'Debug tModLoader Server',
    args: [
      ...clientConfiguration.args,
      '-server',
      '-nosteam',
      '-config',
      '${workspaceFolder}/serverconfig.local.txt',
    ],
  };
  const launchArgumentsInputId = 'tModLoaderLaunchArguments';
  const chooseConfiguration = {
    ...clientConfiguration,
    args: '${input:' + launchArgumentsInputId + '}',
  };
  const launchArgumentsInput = {
    id: launchArgumentsInputId,
    type: 'pickString',
    description: '选择要调试的 tModLoader 进程',
    options: [
      { label: '客户端', value: debugCommandLine(clientConfiguration.args) },
      {
        label: '服务器（读取 serverconfig.local.txt）',
        value: debugCommandLine([
          ...clientConfiguration.args,
          '-server',
          '-nosteam',
          '-config',
          serverConfigPath,
        ]),
      },
    ],
  };

  for (const configuration of [chooseConfiguration, directClientConfiguration, serverConfiguration]) {
    const index = launchSettings.configurations.findIndex((item) => item.name === configuration.name);
    if (index >= 0) {
      launchSettings.configurations[index] = configuration;
    } else {
      launchSettings.configurations.push(configuration);
    }
  }

  launchSettings.inputs = (launchSettings.inputs || [])
    .filter((input) => input.id !== launchArgumentsInputId);
  launchSettings.inputs.push(launchArgumentsInput);

  fs.writeFileSync(vscodeLaunchPath, `${JSON.stringify(launchSettings, null, 2)}\n`, 'utf8');
  console.log(`Wrote VS Code debug configuration: ${vscodeLaunchPath}`);
}

try {
  const options = parseOptions(process.argv.slice(2));

  if (!fs.existsSync(serverConfigPath)) {
    fs.copyFileSync(serverConfigExamplePath, serverConfigPath);
    console.log(`Created local server configuration. Set world to an existing .wld path: ${serverConfigPath}`);
  }

  let tModLoaderPath;
  if (fs.existsSync(configPath) && !options.force && !hasPlaceholderConfig()) {
    tModLoaderPath = readConfiguredTModLoaderPath(configPath);
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
