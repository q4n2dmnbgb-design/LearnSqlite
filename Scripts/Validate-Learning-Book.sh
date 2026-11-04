#!/usr/bin/env bash
set -euo pipefail

PROJECT_ROOT="${1:-$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)}"
SCRIPT_PATH="$(dirname "${BASH_SOURCE[0]}")/Validate-Learning-Book.py"

if [ ! -f "$SCRIPT_PATH" ]; then
    SCRIPT_PATH="$PROJECT_ROOT/.agents/skills/maui-step-guide-author/scripts/validate_learning_book.py"
fi

if [ ! -f "$SCRIPT_PATH" ]; then
    echo "Could not find validate_learning_book.py." >&2
    exit 1
fi

python "$SCRIPT_PATH" \
    --html "$PROJECT_ROOT/Learning/MAUI-Learning-Book.html" \
    --pdf "$PROJECT_ROOT/Learning/MAUI-Learning-Book.pdf" \
    --report "$PROJECT_ROOT/Learning/MAUI-Learning-Book.validation.json" \
    "${@:2}"
