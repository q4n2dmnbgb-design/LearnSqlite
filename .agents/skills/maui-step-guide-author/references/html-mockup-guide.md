# HTML Visual-Model Guide

Use an HTML mockup when an emulator is unavailable and a visual block diagram will help the student understand a MAUI screen. The mockup documents the intended composition; it does not replace native rendering tests.

Save the file as `Learning/Mockups/<ScreenName>.html`. Mark one overview and each teachable block with stable data attributes so the capture tool can work for any screen.

```html
<!doctype html>
<html lang="he" dir="rtl">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Dashboard visual model</title>
  <style>
    * { box-sizing: border-box; }
    body { margin: 0; padding: 24px; background: #0f172a; font-family: "Segoe UI", sans-serif; }
    [data-guide-overview] { width: min(390px, 100%); min-height: 760px; margin: auto; overflow: hidden; border-radius: 30px; background: #f8fafc; box-shadow: 0 18px 48px #020617; }
    [data-guide-block] { position: relative; margin: 12px; padding: 18px; border: 2px dashed #6366f1; border-radius: 18px; background: white; }
    [data-guide-block]::before { content: attr(data-guide-block); position: absolute; top: 6px; inset-inline-end: 8px; color: #4338ca; font-size: 12px; font-weight: 700; }
    .hero { min-height: 190px; padding-top: 52px; color: white; background: linear-gradient(135deg, #4f46e5, #7c3aed); }
  </style>
</head>
<body>
  <main data-guide-overview>
    <section class="hero" data-guide-block="hero">
      <h1>כותרת המסך</h1>
      <p>הסבר קצר על התוצאה שהמשתמש רואה.</p>
    </section>
    <section data-guide-block="summary">
      <h2>כרטיס סיכום</h2>
      <p>Map this block to the actual MAUI `Border` or layout container.</p>
    </section>
    <section data-guide-block="actions">
      <button type="button">פעולה</button>
    </section>
  </main>
</body>
</html>
```

Capture it with:

```powershell
python .\Scripts\Capture-Mockup-Blocks.py `
  --mockup .\Learning\Mockups\Dashboard.html `
  --screen Dashboard
```

The output is:

```text
Learning/Images/Dashboard-overview.png
Learning/Images/Dashboard-block1-hero.png
Learning/Images/Dashboard-block2-summary.png
Learning/Images/Dashboard-block3-actions.png
Learning/Images/Dashboard-snapshots.json
```

Use a different `--overview-selector` or `--block-selector` only when the supplied attributes cannot be used. The tool exits with an error if the overview selector matches anything other than one element or if no blocks are found.
