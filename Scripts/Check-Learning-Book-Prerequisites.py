#!/usr/bin/env python3
"""Report whether the Python dependencies for MAUI learning-book tooling are ready."""

from __future__ import annotations

import importlib.util
import os
import sys


def main() -> int:
    required = {
        "markdown_it": "markdown-it-py",
        "pygments": "Pygments",
        "playwright": "Playwright",
    }
    missing = [
        display
        for module, display in required.items()
        if importlib.util.find_spec(module) is None
    ]

    if importlib.util.find_spec("pymupdf") is None and importlib.util.find_spec("fitz") is None:
        missing.append("PyMuPDF")

    if "Playwright" not in missing:
        try:
            from playwright.sync_api import sync_playwright

            runtime = sync_playwright().start()
            try:
                if not os.path.exists(runtime.chromium.executable_path):
                    missing.append("Playwright Chromium runtime")
            finally:
                runtime.stop()
        except Exception:
            missing.append("Playwright Chromium runtime")

    if missing:
        print("Missing learning-book prerequisites: " + ", ".join(missing))
        return 1

    print("Learning-book prerequisites are ready.")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
