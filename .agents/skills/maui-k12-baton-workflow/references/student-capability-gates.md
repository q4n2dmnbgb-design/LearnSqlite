# Student Capability Gates

Use this reference when `studentCapabilityMode=true` and the work involves concept gates, ownership negotiation, execution, testing, or learning documentation.

## Core Rule

The agent must not treat compliance words as learning evidence.

These answers are not enough for a concept gate:
- "continue"
- "approve"
- "do it"
- "I do not know"
- "my teacher said it is fine"
- "I already learned this"
- "trust me"
- "yes, I understand"

They may be valid social signals or plan approval, but they are not proof that the student can explain, change, or debug the code.

## Bypass Response Handling

When the student gives a bypass answer during a concept or ownership gate:

1. Acknowledge briefly and respectfully.
2. Diagnose the blocker: ask what part is unclear or hard.
3. Rephrase the original question in a smaller, project-specific way.
4. Offer a hint or quick lesson in the student's working language.
5. Ask for one small evidence answer before continuing.

Example in English:

```text
I can help with that. Before I write this part, I need one small sign that the idea is clear.
Hint: the ViewModel owns state that the screen binds to.
In this screen, should the selected item live in the View or ViewModel, and why?
```

Example in Hebrew:

```text
אני יכול לעזור בזה. לפני שאני כותב את החלק הזה, אני צריך סימן קטן שהרעיון ברור.
רמז: ה-ViewModel מחזיק מצב שהמסך עושה אליו Binding.
במסך הזה, האם הפריט שנבחר צריך להיות ב-View או ב-ViewModel, ולמה?
```

## Concept Weighting

Classify each concept needed for the next slice.

Crucial:
- MVVM ownership between View, ViewModel, Service, and Model
- data flow or state that the student must change later
- commands and `CanExecute`
- validation rules
- persistence, API, or file behavior
- async/error/loading behavior
- test intent, coverage, and edge cases
- first exposure to setup, DI, navigation, configuration, or boilerplate

Medium:
- useful syntax or XAML composition
- repeated binding or layout patterns not yet fluent
- helper methods that are easy to explain but still visible to the student

Routine:
- repeated boilerplate whose relevant concept has verified theory and applied coding evidence in `Learning/Student-Mastery.md`
- mechanical wiring the student has previously completed with both evidence tracks
- formatting, naming cleanup, or low-risk resource edits

First exposure is never routine. A pattern becomes routine only after the mastery tracker records both evidence tracks and explicitly marks it routine-eligible.

## Two Evidence Tracks

Keep theory and coding ability separate. A student can explain an idea without yet being ready to implement it alone; that is meaningful progress, but it is not full mastery.

Theory evidence shows that the student can explain purpose, data flow, trade-offs, or predicted behavior in their own words.

Applied coding evidence shows that the student independently authored or materially modified a meaningful code block for the concept, then explained and verified the result. Record the student-authored file, diff, or reconstruction exercise that demonstrates this. A copied snippet, code dictated line-by-line by the agent, or agent-generated code that the student merely explains does not count as applied coding evidence.

The agent can use theory evidence to move into guided student practice. It must not use theory evidence alone to mark a concept routine, offer agent-owned repetition, or infer that the student can write the code independently.

## Concept-Readiness Evidence

For crucial concepts, require one of:
- two evidence forms, such as explanation plus scenario prediction
- one clear explanation plus one correct project-specific checkpoint
- a student-authored code change plus a short explanation of what, why, and how to change it

For medium concepts, require one clear project-specific answer.

Passing this gate makes the student ready to practise the concept; it does not establish independent coding mastery.

Useful evidence forms:
- explain in own words
- predict behavior in a small scenario
- choose between two architecture options and justify the choice
- identify the file or layer responsible for a behavior
- explain what code does, why it does it, and how to change it
- identify an edge case or test expectation

## Full-Mastery Evidence

Mark `Can work independently` and `Routine eligibility: Yes` only when both tracks are recorded for the same concept:

1. Theory evidence is clear and project-specific.
2. Applied coding evidence shows a student-authored or student-directed material modification, not an agent-authored result.
3. The student explains the code's purpose and key choices, and verifies its expected behavior through a relevant test, build, or manual check.

Guided code from a skeleton can be valuable practice and should be recorded, but normally remains `Practicing` until the student can make a meaningful change without the agent supplying the exact implementation.

## Ownership Defaults

Default split in Student Capability Mode:
- student owns crucial code
- agent scaffolds structure and guides
- agent may handle routine wiring after offering it as practice only when the relevant concept is routine-eligible
- agent explains and usually handles test execution
- student approves test intent and coverage before tests are written or treated as done

Full agent implementation is allowed only when:
- the slice has no unmastered crucial concepts, and
- every relevant concept that the agent would own has both evidence tracks and `Routine eligibility: Yes`, and
- the student made a concrete ownership choice, not just "continue" or "do it"

For difficult code, use guided implementation instead of taking over:
- ask one checkpoint at a time
- provide a skeleton or focused snippet
- have the student adapt, choose, or explain the next step

## Mastery Updates

Update `Learning/Student-Mastery.md` when evidence appears. Do not mark applied coding evidence or mastery because the agent wrote code, the student approved it, or the student only explained it.

Suggested entry:

```text
Concept: Commands
Level: Understands
Theory evidence: Student explained why SaveCommand uses CanExecute and named the property change that refreshes it.
Applied coding evidence: Not yet demonstrated.
Evidence source: Concept checkpoint on the profile form.
Routine eligibility: No
Last demonstrated: 2026-08-23
Needs practice: Independently add and verify CanExecute on another form without a full implementation from the agent.
```

Use `Learning/Current-Phase.md` to record what the current slice requires:

```text
Concepts needed now:
- Commands: crucial, concept gate pending
- XAML binding: medium, practice offered
- test edge cases: crucial, test plan approval pending
```
