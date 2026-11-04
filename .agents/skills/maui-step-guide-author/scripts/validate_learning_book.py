#!/usr/bin/env python3
"""Validate the HTML and PDF learning-book outputs before publication."""

from __future__ import annotations

import argparse
import json
import sys
from pathlib import Path


DEFAULT_PLACEHOLDERS = ("<ScreenName>", "<AppNamespace>", "TODO: replace")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Validate MAUI learning-book HTML and PDF artifacts.")
    parser.add_argument("--html", required=True, help="Generated MAUI-Learning-Book.html")
    parser.add_argument("--pdf", required=True, help="Generated MAUI-Learning-Book.pdf")
    parser.add_argument("--report", required=True, help="JSON validation report destination")
    parser.add_argument("--placeholder", action="append", default=[], help="Additional token that must not appear in final output")
    return parser.parse_args()


def validate_html(html_path: Path) -> list[dict[str, object]]:
    try:
        from playwright.sync_api import sync_playwright
    except ImportError as error:
        raise RuntimeError("Playwright is required. Run: python -m pip install -r Scripts/Learning-Book-Requirements.txt; python -m playwright install chromium") from error

    if not html_path.is_file():
        return [{"kind": "missing-html", "message": f"Generated HTML was not found: {html_path}"}]

    with sync_playwright() as playwright:
        browser = playwright.chromium.launch(headless=True)
        page = browser.new_page(viewport={"width": 1280, "height": 1800}, device_scale_factor=1)
        page.goto(html_path.resolve().as_uri(), wait_until="networkidle")
        page.emulate_media(media="print")
        issues = page.evaluate(
            """() => {
                const issues = [];
                for (const image of document.querySelectorAll('img')) {
                    if (!image.complete || image.naturalWidth === 0 || image.naturalHeight === 0) {
                        issues.push({ kind: 'image-not-rendered', message: image.getAttribute('src') || '(missing src)' });
                    }
                }
                for (const element of document.querySelectorAll('table, pre, .code-block, .embedded-img')) {
                    const style = getComputedStyle(element);
                    if (element.scrollWidth > element.clientWidth + 2) {
                        issues.push({ kind: 'horizontal-overflow', message: element.tagName.toLowerCase(), className: element.className });
                    }
                    if (style.overflowY !== 'visible' && element.scrollHeight > element.clientHeight + 2) {
                        issues.push({ kind: 'vertical-clipping', message: element.tagName.toLowerCase(), className: element.className });
                    }
                }
                if (document.documentElement.scrollWidth > document.documentElement.clientWidth + 2) {
                    issues.push({ kind: 'document-overflow', message: 'The generated book is wider than the print viewport.' });
                }
                return issues;
            }"""
        )
        browser.close()
    return issues


def validate_pdf(pdf_path: Path, placeholders: tuple[str, ...]) -> list[dict[str, object]]:
    try:
        try:
            import pymupdf as fitz
        except ImportError:
            import fitz
    except ImportError as error:
        raise RuntimeError("PyMuPDF is required. Run: python -m pip install -r Scripts/Learning-Book-Requirements.txt") from error

    if not pdf_path.is_file() or pdf_path.stat().st_size == 0:
        return [{"kind": "missing-pdf", "message": f"Generated PDF was not found or is empty: {pdf_path}"}]

    issues: list[dict[str, object]] = []
    document = fitz.open(pdf_path)
    if document.page_count == 0:
        issues.append({"kind": "empty-pdf", "message": "The PDF has no pages."})

    for index, page in enumerate(document, start=1):
        page_text = page.get_text("text")
        blocks = page.get_text("blocks")
        has_images = bool(page.get_images(full=True))
        if not page_text.strip() and not has_images:
            issues.append({"kind": "blank-page", "page": index, "message": "The page has no text or images."})

        for block in blocks:
            x0, y0, x1, y1, text, *_ = block
            if text.strip() and (x0 < -1 or y0 < -1 or x1 > page.rect.width + 1 or y1 > page.rect.height + 1):
                issues.append({"kind": "text-outside-page", "page": index, "message": text.strip()[:80]})

        for placeholder in placeholders:
            if placeholder and placeholder in page_text:
                issues.append({"kind": "placeholder", "page": index, "message": placeholder})

    document.close()
    return issues


def main() -> int:
    args = parse_args()
    html_path = Path(args.html).resolve()
    pdf_path = Path(args.pdf).resolve()
    report_path = Path(args.report).resolve()
    report_path.parent.mkdir(parents=True, exist_ok=True)

    report: dict[str, object] = {"html": str(html_path), "pdf": str(pdf_path), "issues": []}
    try:
        report["issues"].extend(validate_html(html_path))
        report["issues"].extend(validate_pdf(pdf_path, tuple(DEFAULT_PLACEHOLDERS) + tuple(args.placeholder)))
    except RuntimeError as error:
        report["issues"].append({"kind": "dependency", "message": str(error)})

    report["passed"] = not report["issues"]
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    if report["passed"]:
        print(f"PASS: learning-book validation report written to {report_path}")
        return 0

    print(f"FAIL: {len(report['issues'])} validation issue(s); see {report_path}", file=sys.stderr)
    for issue in report["issues"]:
        print(f"- {issue['kind']}: {issue['message']}", file=sys.stderr)
    return 1


if __name__ == "__main__":
    raise SystemExit(main())
