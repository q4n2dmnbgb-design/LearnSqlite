---
name: maui-step-guide-author
description: Create or extend source-grounded .NET MAUI learning books and guided build plans. Use whenever a student or teacher asks for a step-by-step MAUI tutorial, a learning book for a new or existing project, an explanation of an existing screen or MVVM feature, a UI replication guide, HTML visual mockups, or validated HTML/PDF learning artifacts. Use it even when the request does not explicitly say "guide" or "book".
---

# MAUI Step Guide Author

## Purpose

Turn a real .NET MAUI project into a learning journey the student can follow, change, and explain. The durable source is Markdown; HTML mockups and the compiled HTML/PDF book are derived artifacts.

Teach the project that exists. Do not invent packages, services, data models, namespace names, or visual behavior merely to make a guide look complete.

When the student works in Hebrew, write explanations in Hebrew while preserving English code identifiers. Otherwise, match the student's working language.

## Relationship to the Baton Workflow

This skill complements `maui-k12-baton-workflow`; it never bypasses its planning, concept, ownership, or test-coverage gates.

- For a new learning book or an existing-project roadmap, use this skill during planning to map the journey before code is written.
- For an implemented screen or feature, use it during `CODE_LEARNING_REVIEWER` to create the factual chapter and artifacts.
- If the baton workflow is active, end at its current waiting stage. Do not turn a guide request into permission to dump a whole implementation.

## Choose the Guide Mode

Begin by identifying one mode with the user:

1. **New-project build-along**: create a sequence from an empty solution to the agreed outcome.
2. **Existing-project learning book**: inspect the current code, agree on the end goal, and choose the smallest useful milestones from the current state to that goal.
3. **UI replication**: deconstruct an existing or supplied design into visual and MVVM blocks while clearly labelling the HTML mockup as a visual model, not an emulator capture.

For an existing project, first record facts: solution/projects, relevant screens, ViewModels, models, services, navigation, resources, dependencies, tests, and current learning artifacts. Then propose a short roadmap that states each milestone's outcome, prerequisites, files, crucial concept, and verification. Ask the user to choose or correct the order before authoring chapters.

## Chapter Contract

Read `references/guide-template.md` whenever authoring a chapter. Keep each chapter source-grounded and give every code or UI block this compact contract:

1. **Goal and visible result**: what changes for the user or learner.
2. **Where it belongs**: exact file/layer and why that layer owns it.
3. **Small block**: only the new or changed code; mark unchanged context rather than pasting whole files.
4. **Why it works**: explain data flow, layout choice, state, command, or service boundary in student language.
5. **Evidence**: a snapshot, build/test command, expected behavior, or a narrow manual check.
6. **Student checkpoint**: one applied prediction, explanation, or small follow-up task appropriate to the current baton stage.

Use analytical tables only when they reduce cognitive load. For example, map a `CollectionView` to its collection and item template, or map a command to its `CanExecute` condition. Do not force a fixed number of blocks or tables onto every chapter.

## Block Strategy

Build from the outside in and keep visual and logic blocks paired where useful:

- Visual blocks: page shell, layout backbone, content sections, data templates, input/actions, loading/error feedback.
- Logic blocks: state properties, injected dependencies, commands, `CanExecute` refresh, async/error flow, and service calls.

Explain why a `Grid`, stack layout, `CollectionView`, `Border`, resource, binding mode, or command is appropriate for this screen. Respect the MAUI workflow guardrails: compiled bindings when known, `CanExecute` for meaningful command conditions, `IsLoading` with `ActivityIndicator`, and explicit student-readable property setters when required.

## Visual Mockups and Snapshots

Read `references/html-mockup-guide.md` when a chapter needs visual snapshots. HTML is used because an emulator may be unavailable; it is a teaching diagram, not proof that the native MAUI page rendered.

Mark the mockup with these stable attributes:

```html
<main data-guide-overview>
  <section data-guide-block="hero">...</section>
  <section data-guide-block="summary">...</section>
</main>
```

Use the bundled generic tool, after copying it into the target project's `Scripts` folder:

```powershell
python .\Scripts\Capture-Mockup-Blocks.py `
  --mockup .\Learning\Mockups\Dashboard.html `
  --screen Dashboard
```

It writes one overview image, one image per marked block, and a snapshot manifest under `Learning/Images`. Embed only generated image paths in the completed guide. Missing selectors or zero blocks are failures, not optional warnings.

## Book Compilation and Validation

Read the relevant bundled scripts only when generating artifacts:

- `scripts/generate_book_pdf.py` compiles the Markdown book and guides.
- `scripts/validate_learning_book.py` validates the generated HTML and PDF.
- `scripts/requirements-learning-book.txt` lists Python dependencies.

Install dependencies deliberately; do not install packages silently:

```powershell
python -m pip install -r .\Scripts\Learning-Book-Requirements.txt
python -m playwright install chromium
```

Generate the book with validation enabled (the default):

```powershell
python .\Scripts\Generate-Learning-Book-Pdf.py
```

The generated validation report must pass before calling the book publication-ready. It checks unresolved or placeholder images, HTML overflow in tables and code blocks, empty PDF pages, text that extends beyond a page boundary, and unresolved guide placeholders. A failed report means the output needs correction and regeneration. It is an automated safety net; visually inspect a high-risk layout or a changed print stylesheet as well.

## Required Artifacts

```text
Learning/MAUI-Learning-Book.md                 canonical book and roadmap
Learning/Guides/<ScreenName>-Guide.md          screen or feature chapter
Learning/Mockups/<ScreenName>.html             optional visual teaching model
Learning/Images/<ScreenName>-overview.png      generated overview
Learning/Images/<ScreenName>-block<N>-*.png    generated block snapshots
Learning/MAUI-Learning-Book.html                derived HTML book
Learning/MAUI-Learning-Book.pdf                 derived PDF book
Learning/MAUI-Learning-Book.validation.json    generated validation report
```

Update the book index after adding a guide. Keep the book append-only when the parent workflow requires it.

## Quality Bar

Before presenting a guide or book as complete, verify that:

- the roadmap reflects the project's current state and the user's agreed end goal;
- each code block maps to an actual source file and has a reason for its placement;
- snapshots are present for visual blocks and clearly labelled as HTML models;
- Markdown references resolve and no template placeholder remains in a completed artifact;
- the book validation report passes; and
- the parent workflow's build, test, and learning-documentation checks have evidence.

## Evaluation Coverage

The bundled `evals/evals.json` covers a new-project build-along, an existing-project roadmap, and a UI-replication request. When revising this skill, preserve those distinct paths and add a realistic case before changing the trigger description.
