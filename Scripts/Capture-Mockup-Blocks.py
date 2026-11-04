#!/usr/bin/env python3
"""Capture a reusable HTML visual model into overview and block snapshots."""

from __future__ import annotations

import argparse
import json
import re
import sys
from pathlib import Path


def safe_name(value: str) -> str:
    cleaned = re.sub(r"[^A-Za-z0-9_-]+", "-", value.strip()).strip("-")
    return cleaned or "block"


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Capture marked learning-guide mockup blocks.")
    parser.add_argument("--mockup", required=True, help="Path to Learning/Mockups/<ScreenName>.html")
    parser.add_argument("--screen", help="Output prefix; defaults to the mockup filename")
    parser.add_argument("--output-dir", help="Defaults to <project root>/Learning/Images")
    parser.add_argument("--overview-selector", default="[data-guide-overview]")
    parser.add_argument("--block-selector", default="[data-guide-block]")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    mockup = Path(args.mockup).resolve()
    if not mockup.is_file():
        print(f"Mockup file was not found: {mockup}", file=sys.stderr)
        return 2

    try:
        from playwright.sync_api import sync_playwright
    except ImportError:
        print("Missing Playwright. Run: python -m pip install -r Scripts/Learning-Book-Requirements.txt", file=sys.stderr)
        return 3

    screen = safe_name(args.screen or mockup.stem)
    output_dir = Path(args.output_dir).resolve() if args.output_dir else mockup.parent.parent / "Images"
    output_dir.mkdir(parents=True, exist_ok=True)
    manifest: dict[str, object] = {"screen": screen, "mockup": str(mockup), "overview": None, "blocks": []}

    try:
        with sync_playwright() as playwright:
            browser = playwright.chromium.launch(headless=True)
            page = browser.new_page(viewport={"width": 430, "height": 950}, device_scale_factor=2)
            page.goto(mockup.as_uri(), wait_until="networkidle")

            overview = page.locator(args.overview_selector)
            if overview.count() != 1:
                raise RuntimeError(f"Overview selector {args.overview_selector!r} matched {overview.count()} elements; expected exactly one.")

            overview_path = output_dir / f"{screen}-overview.png"
            overview.screenshot(path=str(overview_path))
            manifest["overview"] = str(overview_path)

            blocks = page.locator(args.block_selector)
            if blocks.count() == 0:
                raise RuntimeError(f"Block selector {args.block_selector!r} matched no elements.")

            for index in range(blocks.count()):
                block = blocks.nth(index)
                label = safe_name(block.get_attribute("data-guide-block") or f"block{index + 1}")
                block_path = output_dir / f"{screen}-block{index + 1}-{label}.png"
                block.screenshot(path=str(block_path))
                manifest["blocks"].append({"name": label, "path": str(block_path)})

            browser.close()
    except Exception as error:
        print(f"Snapshot capture failed: {error}", file=sys.stderr)
        return 4

    manifest_path = output_dir / f"{screen}-snapshots.json"
    manifest_path.write_text(json.dumps(manifest, ensure_ascii=False, indent=2), encoding="utf-8")
    print(f"Captured {len(manifest['blocks'])} blocks and wrote {manifest_path}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
