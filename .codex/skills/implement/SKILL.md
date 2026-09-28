---
name: implement
description: Implement an already-decided change in this repository with the smallest coherent diff. Use after requirements and important design choices are settled; do not use for open-ended investigation, testing-only work, or review-only work.
---

# Implement

Follow `AGENTS.md`; it takes precedence over this skill. This skill defines implementation practice, not authority, approval, or safety policy.

## Approach

1. Restate the approved outcome, scope, and observable acceptance conditions before editing.
2. Inspect only the code and nearby conventions needed to locate the correct change point.
3. Prefer the smallest cohesive change that fits existing abstractions, naming, error handling, and data flow.
4. Preserve current public behavior except where the approved requirement changes it.
5. Check the affected code paths in proportion to their risk, then report what was and was not verified.

## Boundaries

- Do not fold opportunistic cleanup, broad formatting, or unrelated refactoring into the change.
- Reuse existing project mechanisms before introducing a new abstraction.
- Do not add or replace external dependencies unless that choice was explicitly approved.
- Keep compatibility-sensitive changes visible; do not silently alter public APIs, persisted formats, configuration contracts, or user-facing behavior.
- If the smallest correct implementation exceeds the approved scope, return to the escalation and approval rules in `AGENTS.md`.
- Leave detailed test selection and regression strategy to the `test` skill.

## Result

Report the files changed, the important behavior change, checks performed, and any assumptions or unresolved risks. Distinguish completed verification from checks that were not possible.
