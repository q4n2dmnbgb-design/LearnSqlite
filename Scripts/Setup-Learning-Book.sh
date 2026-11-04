#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="${1:-$(cd "$SCRIPT_DIR/.." && pwd)}"
INSTALL_MISSING="${2:-}"

if [ -n "$INSTALL_MISSING" ] && [ "$INSTALL_MISSING" != "--install-missing" ]; then
    echo "Usage: ./Scripts/Setup-Learning-Book.sh [project-root] [--install-missing]" >&2
    exit 1
fi

PYTHON_COMMAND=""
for candidate in python3 python; do
    if command -v "$candidate" >/dev/null 2>&1; then
        candidate_path="$(command -v "$candidate")"
        if "$candidate_path" -c "import sys" >/dev/null 2>&1; then
            PYTHON_COMMAND="$candidate_path"
            break
        fi
    fi
done

probe_prerequisites() {
    "$PYTHON_COMMAND" "$SCRIPT_DIR/Check-Learning-Book-Prerequisites.py"
}

READY=false
if [ -n "$PYTHON_COMMAND" ]; then
    if PROBE_OUTPUT="$(probe_prerequisites 2>&1)"; then
        READY=true
    else
        printf '%s\n' "$PROBE_OUTPUT"
    fi
else
    echo "Missing learning-book prerequisite: Python 3."
fi

if [ "$READY" = true ]; then
    echo "Learning-book prerequisites are already available. No downloads were performed."
    exit 0
fi

echo "Learning-book generation needs the declared Python packages and the Playwright Chromium runtime."
echo "Installing them may download browser binaries, use disk space, and change the selected Python environment."

if [ "$INSTALL_MISSING" != "--install-missing" ]; then
    echo "No downloads were performed. Run ./Scripts/Setup-Learning-Book.sh \"$PROJECT_ROOT\" --install-missing when you are ready."
    exit 0
fi

if [ -z "$PYTHON_COMMAND" ]; then
    echo "Python 3 is required before learning-book dependencies can be installed." >&2
    exit 1
fi

REQUIREMENTS_FILE="$PROJECT_ROOT/Scripts/Learning-Book-Requirements.txt"
if [ ! -f "$REQUIREMENTS_FILE" ]; then
    echo "Cannot find the learning-book requirements file: $REQUIREMENTS_FILE" >&2
    exit 1
fi

echo "Installing missing learning-book prerequisites..."
if ! "$PYTHON_COMMAND" -m pip install -r "$REQUIREMENTS_FILE"; then
    echo "Python dependency installation failed." >&2
    exit 1
fi

if ! "$PYTHON_COMMAND" -m playwright install chromium; then
    echo "Playwright Chromium installation failed." >&2
    exit 1
fi

if ! PROBE_OUTPUT="$(probe_prerequisites 2>&1)"; then
    printf '%s\n' "$PROBE_OUTPUT" >&2
    echo "Learning-book prerequisites are still incomplete after installation." >&2
    exit 1
fi

echo "Learning-book prerequisites are ready."
