#!/usr/bin/env bash
set -euo pipefail

script_dir="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd)"
project_dir="$(cd -- "$script_dir/.." && pwd)"
config_path="$project_dir/tModLoader.local.props"
requested_path=""
force=false

while [[ $# -gt 0 ]]; do
  case "$1" in
    --tmodloader-path)
      requested_path="${2:-}"
      shift 2
      ;;
    --force)
      force=true
      shift
      ;;
    *)
      printf 'Unknown option: %s\n' "$1" >&2
      exit 2
      ;;
  esac
done

if [[ -f "$config_path" && "$force" != true ]]; then
  printf 'Keeping existing local configuration: %s\n' "$config_path"
  exit 0
fi

found_path=""
try_path() {
  local candidate="$1"
  if [[ -z "$found_path" && -f "$candidate/tMLMod.targets" ]]; then
    found_path="$candidate"
  fi
}

if [[ -n "$requested_path" ]]; then
  try_path "$requested_path"
fi

steam_roots=(
  "$HOME/Library/Application Support/Steam"
  "$HOME/.steam/steam"
  "$HOME/.local/share/Steam"
)

for steam_root in "${steam_roots[@]}"; do
  try_path "$steam_root/steamapps/common/tModLoader"

  vdf_path="$steam_root/steamapps/libraryfolders.vdf"
  if [[ -f "$vdf_path" ]]; then
    while IFS= read -r library_root; do
      library_root="${library_root//\\\\/\\}"
      try_path "$library_root/steamapps/common/tModLoader"
    done < <(sed -nE 's/.*"path"[[:space:]]+"([^"]+)".*/\1/p' "$vdf_path")
  fi
done

if [[ -n "$found_path" ]]; then
  path_value="$found_path"
  printf 'Found tModLoader: %s\n' "$found_path"
  status=0
else
  path_value="REPLACE_WITH_YOUR_TMODLOADER_PATH"
  printf 'Could not find tModLoader. Generated a template for manual editing: %s\n' "$config_path" >&2
  status=1
fi

{
  printf '%s\n' '<Project>'
  printf '%s\n' '  <PropertyGroup>'
  printf '    <TModLoaderPath>%s</TModLoaderPath>\n' "$path_value"
  printf '%s\n' '  </PropertyGroup>'
  printf '%s\n' '</Project>'
} > "$config_path"

exit "$status"
