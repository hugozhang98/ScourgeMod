#!/usr/bin/env bash
set -euo pipefail

# tML's macOS native libraries must be exported by the process that starts its
# bundled .NET runtime. The first argument is TModLoaderPath from
# tModLoader.local.props; remaining arguments are forwarded to tModLoader.
tmod_dir="$1"
shift
native_dir="$tmod_dir/Libraries/Native/OSX"

export DYLD_LIBRARY_PATH="$native_dir${DYLD_LIBRARY_PATH:+:$DYLD_LIBRARY_PATH}"
export VK_ICD_FILENAMES="$native_dir/MoltenVK_icd.json"
exec "$tmod_dir/dotnet/dotnet" "$@"
