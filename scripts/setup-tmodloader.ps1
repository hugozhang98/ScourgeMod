[CmdletBinding()]
param(
    [string]$TModLoaderPath,
    [switch]$Force
)

$projectDir = Split-Path -Parent $PSScriptRoot
$configPath = Join-Path $projectDir 'tModLoader.local.props'

if ((Test-Path -LiteralPath $configPath) -and -not $Force) {
    Write-Host "Keeping existing local configuration: $configPath"
    exit 0
}

$candidates = [System.Collections.Generic.List[string]]::new()
if ($TModLoaderPath) {
    $candidates.Add($TModLoaderPath)
}

$steamRoots = @(
    (Join-Path ${env:ProgramFiles(x86)} 'Steam'),
    (Join-Path $env:ProgramFiles 'Steam')
) | Where-Object { $_ }

foreach ($steamRoot in $steamRoots) {
    $candidates.Add((Join-Path $steamRoot 'steamapps/common/tModLoader'))
    $libraryFolders = Join-Path $steamRoot 'steamapps/libraryfolders.vdf'

    if (Test-Path -LiteralPath $libraryFolders) {
        $rawVdf = Get-Content -LiteralPath $libraryFolders -Raw
        foreach ($match in [regex]::Matches($rawVdf, '"path"\s+"([^"]+)"')) {
            $libraryRoot = $match.Groups[1].Value.Replace('\\', '\')
            $candidates.Add((Join-Path $libraryRoot 'steamapps/common/tModLoader'))
        }
    }
}

$foundPath = $candidates |
    Select-Object -Unique |
    Where-Object { Test-Path -LiteralPath (Join-Path $_ 'tMLMod.targets') } |
    Select-Object -First 1

if ($foundPath) {
    $pathValue = $foundPath
    Write-Host "Found tModLoader: $foundPath"
    $exitCode = 0
}
else {
    $pathValue = 'REPLACE_WITH_YOUR_TMODLOADER_PATH'
    Write-Warning "Could not find tModLoader. Generated a template for manual editing: $configPath"
    $exitCode = 1
}

$escapedPath = [System.Security.SecurityElement]::Escape($pathValue)
$contents = @"
<Project>
  <PropertyGroup>
    <TModLoaderPath>$escapedPath</TModLoaderPath>
  </PropertyGroup>
</Project>
"@

Set-Content -LiteralPath $configPath -Value $contents -Encoding utf8
exit $exitCode
