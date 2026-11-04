# MAUI Learning-Book Chapter Template

Use this template as a structure, not as filler. Remove sections that do not help the current student, but never invent project facts to fill them.

```markdown
# <Feature or Screen> — <student-facing title>

## מטרת הפרק / Chapter goal

- **What the learner will see or be able to do:** <observable result>
- **Current project facts:** <actual files, project names, and existing behavior>
- **Why this is the next milestone:** <connection to the agreed end goal>

## מפת הדרך / Roadmap position

| Current state | This milestone | Next milestone | Crucial idea |
| --- | --- | --- | --- |
| <fact> | <small outcome> | <fact> | <concept> |

## מפת הבלוקים / Block map

![HTML model of the completed screen](../Images/<ScreenName>-overview.png)

> This image is an HTML teaching model. The MAUI implementation is verified separately.

| Block | File and layer | Purpose | Evidence |
| --- | --- | --- | --- |
| <hero/list/action/state> | `<actual path>` | <why it belongs here> | <snapshot, test, or manual behavior> |

## Block 1 — <small, meaningful name>

### What changes

<One student-facing sentence.>

### Where it belongs and why

`<actual file path>` — <View, ViewModel, Model, or Service rationale>.

### Small code change

```xml
<!-- Include only this block. Mark unchanged context instead of pasting the whole file. -->
```

### Why it works

<Explain the layout, binding, state, command, or data-flow decision.>

![Block 1 HTML model](../Images/<ScreenName>-block1-<name>.png)

### Checkpoint

> [!TIP]
> <One prediction, explanation, or small student-owned extension.>

### Verify

- <Focused expected behavior, test, build, or validation command.>

## Block 2 — <next small change>

<Repeat the same contract only for material blocks.>

## Reflection and next step

- **Decision:** <a real trade-off in this project>
- **Question:** <why this layer/layout/command owns the behavior>
- **Small challenge:** <a safe extension with a verification criterion>

## Sources

- <official documentation used for a high-friction API or control>
```

For chapters without visual UI, omit the snapshot sections and use a focused test or state-transition table as evidence instead.
