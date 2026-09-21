'use strict';

const fs = require('node:fs');
const path = require('node:path');
const { execFileSync } = require('node:child_process');

const projectDirectory = path.resolve(__dirname, '..');
const configPath = path.join(projectDirectory, 'tModLoader.local.props');

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
      const match = valueLine?.match(/REG_SZ\s+(.+)$/);
      if (match?.[1]) {
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
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&apos;');
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

try {
  const options = parseOptions(process.argv.slice(2));

  if (fs.existsSync(configPath) && !options.force && !hasPlaceholderConfig()) {
    console.log(`Keeping existing local configuration: ${configPath}`);
  } else {
    const tModLoaderPath = findTModLoader(options);

    if (tModLoaderPath) {
      writeConfig(tModLoaderPath);
      console.log(`Found tModLoader and wrote local configuration: ${tModLoaderPath}`);
    } else {
      writeConfig('REPLACE_WITH_YOUR_TMODLOADER_PATH');
      console.log(`Could not find tModLoader. Generated a template for manual editing: ${configPath}`);
    }
  }
} catch (error) {
  console.error(error instanceof Error ? error.message : error);
  process.exitCode = 1;
}
