#!/usr/bin/env bash
# Builds the Voidwalker Codex and serves it on a local server, then opens it.
# Usage (repo root): bash tools/codex/serve.sh [port]    default port 8765; Ctrl+C stops the server.
set -e
cd "$(dirname "$0")/../.."
port="${1:-8765}"
node tools/codex/genpage.js .
url="http://127.0.0.1:$port/voidwalker-codex.html"
echo "Voidwalker Codex: $url"
case "$(uname -s)" in
  MINGW*|MSYS*|CYGWIN*) start "" "$url" ;;
  Darwin) open "$url" ;;
  *) xdg-open "$url" >/dev/null 2>&1 || true ;;
esac
cd build/codex && python -m http.server "$port" --bind 127.0.0.1
