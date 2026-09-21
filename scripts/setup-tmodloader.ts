import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

type Options = {
  force: boolean;
  tModLoaderPath?: string;
};

const scriptDirectory = path.dirname(fileURLToPath(import.meta.url));
const projectDirectory = path.resolve(scriptDirectory, '..');
const configPath = path.join(projectDirectory, 'tModLoader.local.props');

function printUsage(): void {
  console.log(`Usage: npx --yes tsx scripts/setup-tmodloader.ts [options]

Options:
  --tmodloader-path <path>  Use this Steam tModLoader installation directory.
  --force                   Regenerate tModLoader.local.props if it exists.
  --help                    Show this message.`);
}

function parseOptions(args: string[]): Options {
  const options: Options = { force: false };

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

function tModLoaderTargetExists(candidate: string): boolean {
  return fs.existsSync(path.join(candidate, 'tMLMod.targets'));
}

function steamRoots(): string[] {
  const home = process.env.HOME ?? process.env.USERPROFILE;
  const roots: string[] = [];

  if (process.platform === 'win32') {
    for (const programFiles of [process.env['ProgramFiles(x86)'], process.env.ProgramFiles]) {
      if (programFiles) {
        roots.push(path.join(programFiles, 'Steam'));
      }
    }
  } else if (home) {
    roots.push(
      path.join(home, 'Library', 'Application Support', 'Steam'),
      path.join(home, '.steam', 'steam'),
      path.join(home, '.local', 'share', 'Steam'),
    );
  }

  return [...new Set(roots)];
}

function librariesFromVdf(steamRoot: string): string[] {
  const libraryFoldersPath = path.join(steamRoot, 'steamapps', 'libraryfolders.vdf');
  if (!fs.existsSync(libraryFoldersPath)) {
    return [];
  }

  const vdf = fs.readFileSync(libraryFoldersPath, 'utf8');
  return [...vdf.matchAll(/"path"\s+"([^"]+)"/g)]
    .map((match) => match[1].replace(/\\\\/g, '\\'));
}

function findTModLoader(options: Options): string | undefined {
  const candidates: string[] = [];
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

function escapeXml(value: string): string {
  return value
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&apos;');
}

function writeConfig(tModLoaderPath: string): void {
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

  if (fs.existsSync(configPath) && !options.force) {
    console.log(`Keeping existing local configuration: ${configPath}`);
  } else {
    const tModLoaderPath = findTModLoader(options);

    if (tModLoaderPath) {
      writeConfig(tModLoaderPath);
      console.log(`Found tModLoader and wrote local configuration: ${tModLoaderPath}`);
    } else {
      writeConfig('REPLACE_WITH_YOUR_TMODLOADER_PATH');
      console.error(`Could not find tModLoader. Generated a template for manual editing: ${configPath}`);
      process.exitCode = 1;
    }
  }
} catch (error) {
  console.error(error instanceof Error ? error.message : error);
  process.exitCode = 1;
}
