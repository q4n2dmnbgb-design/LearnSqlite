# Student Mastery Tracker

This tracker controls learning ownership. Keep theory and coding evidence separate so an explanation is not mistaken for independent implementation ability.

Use these levels:

```text
Not introduced
Practicing
Understands
Can work independently
```

Use this evidence shape when updating a concept:

```text
Concept:
Level:
Theory evidence:
Applied coding evidence:
Evidence source (student-authored file/diff or reconstruction):
Routine eligibility: Yes/No
Last demonstrated:
Needs practice:
```

Theory evidence means the student explains purpose, data flow, choices, or predicted behavior in their own words.

Applied coding evidence means the student independently authors or materially modifies a meaningful code block, then explains and verifies it. Agent-generated code, copied code, approval, or an explanation of agent-written code never count as applied coding evidence.

Only mark `Can work independently` and `Routine eligibility: Yes` after both evidence tracks are recorded for the same concept. Until then, related core code stays student-owned with scaffolding and review.

## Example

```text
Concept: Commands
Level: Understands
Theory evidence: Student explained why SaveCommand uses CanExecute and named the property change that refreshes it.
Applied coding evidence: Not yet demonstrated.
Evidence source: Concept checkpoint on the profile form.
Routine eligibility: No
Last demonstrated: 2026-09-24
Needs practice: Independently add and verify CanExecute on another form without a full implementation from the agent.
```

## Concepts

```text
Models: Not introduced
Properties: Not introduced
INotifyPropertyChanged: Not introduced
ObservableCollection: Not introduced
XAML binding: Not introduced
Commands: Not introduced
CanExecute: Not introduced
async/await: Not introduced
DI: Not introduced
Shell navigation: Not introduced
validation: Not introduced
testing: Not introduced
```
