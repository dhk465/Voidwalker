#!/usr/bin/env bash
# Builds the Android debug APK with a version that always rises, so `adb install -r` updates in place.
#   version code: minutes since 2026-01-01 UTC (stateless, monotonic, far below Android's 2100000000 cap)
#   version name: application/config/version from project.godot, plus the build code
# The pair is written into export_presets.cfg (local, gitignored) and into src/data/build_info.json (generated,
# gitignored), which the game reads to show its version in the hub. Run from the project root:
#   bash tools/build/android_export.sh
set -euo pipefail
if [[ -z ${GODOT:-} ]]; then
  GODOT=$(sed -n 's/^  path: "\([^"]*\)".*/\1/p' project.yaml)
  [[ -x "$GODOT" ]] || GODOT=godot
fi
OUT="build/android/Voidwalker-debug.apk"

code=$(( ($(date -u +%s) - $(date -u -d 2026-01-01 +%s)) / 60 ))
base=$(sed -n 's/^config\/version="\([^"]*\)".*/\1/p' project.godot)
[ -n "$base" ] || { echo "error: no application/config/version in project.godot" >&2; exit 1; }
name="$base ($code)"

if [[ ! -e export_presets.cfg ]]; then
  cp tools/build/android_export_presets.cfg.in export_presets.cfg
fi
grep -q '^version/code=' export_presets.cfg || { echo "error: export_presets.cfg has no Android preset" >&2; exit 1; }
sed -i "s/^version\/code=.*/version\/code=$code/; s/^version\/name=.*/version\/name=\"$name\"/" export_presets.cfg
printf '{ "name": "%s", "code": %d, "built": "%s" }\n' "$name" "$code" "$(date -u +%Y-%m-%dT%H:%MZ)" > src/data/build_info.json

mkdir -p build/android
# A clean checkout needs the editor assembly before Godot registers C# autoloads.
dotnet build Voidwalker.csproj --configuration Debug --nologo
"$GODOT" --headless --path . --export-debug "Android" "$OUT"
echo "built $OUT  version $name  code $code"
