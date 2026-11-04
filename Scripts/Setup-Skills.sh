#!/usr/bin/env bash
set -euo pipefail

ROOT="${1:-$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)}"

mkdir -p "$ROOT/.agents/skills/maui-k12-baton-workflow"
mkdir -p "$ROOT/.agents/skills/maui-step-guide-author"
mkdir -p "$ROOT/Learning/Guides" "$ROOT/Learning/Mockups" "$ROOT/Learning/Images"

write_if_missing() {
    local destination="$1"
    local content="$2"
    if [ ! -f "$destination" ]; then
        mkdir -p "$(dirname "$destination")"
        printf '%s\n' "$content" > "$destination"
    fi
}

write_if_missing "$ROOT/.agents/MAUI-Agent-Mode.json" '{
  "studentCapabilityMode": true,
  "description": {
    "true": "MAXIMIZE STUDENT CAPABILITY",
    "false": "MAXIMIZE CODE GENERATED"
  }
}'
write_if_missing "$ROOT/Learning/Current-Phase.md" '# Current Phase

Status: NOT_STARTED

Mode is controlled by `.agents/MAUI-Agent-Mode.json`.

Theory evidence:
Applied coding evidence:
Routine eligibility for agent-owned work:'
write_if_missing "$ROOT/Learning/Student-Mastery.md" '# Student Mastery

Use these levels:
- Not introduced
- Practicing
- Understands
- Can work independently

For every concept, record:
- Theory evidence
- Applied coding evidence
- Evidence source (student-authored file/diff or reconstruction)
- Routine eligibility: Yes/No

Only mark `Can work independently` and `Routine eligibility: Yes` after both evidence tracks are demonstrated. Agent-generated or copied code never counts as applied coding evidence.'
write_if_missing "$ROOT/Learning/MAUI-Learning-Book.md" '# MAUI Learning Book

This is the student'"'"'s append-only learning book.'

# Support a local package checkout while keeping the nested companion skill canonical.
PACKAGE_ROOT="$ROOT/SkillPackages/maui-k12-baton-workflow"
if [ -d "$PACKAGE_ROOT" ]; then
    CORE_SOURCE="$PACKAGE_ROOT/skills/maui-k12-baton-workflow"
    COMPANION_SOURCE="$PACKAGE_ROOT/companion-skills/maui-step-guide-author/skills/maui-step-guide-author"
    if [ -d "$CORE_SOURCE" ]; then
        cp -R "$CORE_SOURCE/"* "$ROOT/.agents/skills/maui-k12-baton-workflow/"
    fi
    if [ -d "$COMPANION_SOURCE" ]; then
        cp -R "$COMPANION_SOURCE/"* "$ROOT/.agents/skills/maui-step-guide-author/"
    fi
fi

SOURCE_DIR="$ROOT/.agents/skills/maui-step-guide-author/scripts"
TARGET_DIR="$ROOT/Scripts"
mkdir -p "$TARGET_DIR"

for source_name in generate_book_pdf.py capture_mockup_blocks.py validate_learning_book.py requirements-learning-book.txt; do
    case "$source_name" in
        generate_book_pdf.py) target_name="Generate-Learning-Book-Pdf.py" ;;
        capture_mockup_blocks.py) target_name="Capture-Mockup-Blocks.py" ;;
        validate_learning_book.py) target_name="Validate-Learning-Book.py" ;;
        requirements-learning-book.txt) target_name="Learning-Book-Requirements.txt" ;;
    esac
    source_path="$SOURCE_DIR/$source_name"
    if [ -f "$source_path" ]; then
        cp "$source_path" "$TARGET_DIR/$target_name"
    fi
done

LEARNING_BOOK_SETUP="$TARGET_DIR/Setup-Learning-Book.sh"
if [ -f "$LEARNING_BOOK_SETUP" ]; then
    "$LEARNING_BOOK_SETUP" "$ROOT"
fi

echo "MAUI K12 skill structure, visual-capture tools, and learning-book validation are ready."
echo "Default mode: MAXIMIZE STUDENT CAPABILITY"
