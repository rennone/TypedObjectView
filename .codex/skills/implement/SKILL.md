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

## Create the Task Commit

For repository-changing tasks, the Manager owns the commit unless the human explicitly asks not to commit.

1. Finish implementation, focused verification, and review before staging.
2. Inspect the working tree and distinguish task-owned changes from pre-existing or unrelated changes.
3. Stage only task-owned files or hunks. Workers must never stage or commit.
4. Create one commit for the human task. Keep verification fixes in that same commit; do not include later follow-up requirements.
5. Use a concise imperative subject and a body that summarizes the important changes and verification.
6. Append the AI usage trailers below using recorded runtime or trace data through the end of review and verification.
7. Create the commit, then report its hash and exact message to the human.

Use this trailer shape, omitting unused Worker lines but retaining explicit `unavailable` values when usage is not exposed:

```text
AI-Usage-Status: measured|partial|unavailable
AI-Usage-Cutoff: review-and-verification
AI-Manager: model=<model>; role=manager; input=<n|unavailable>; cached=<n|unavailable>; output=<n|unavailable>; reasoning=<n|unavailable>; total=<n|unavailable>
AI-Worker-1: model=<model>; role=<role>; input=<n|unavailable>; cached=<n|unavailable>; output=<n|unavailable>; reasoning=<n|unavailable>; total=<n|unavailable>
AI-Total-Tokens: <n|unavailable>
AI-Estimated-Cost-USD: <amount|unavailable>
AI-Pricing-As-Of: <YYYY-MM-DD|unavailable>
```

Cached input is already included in input, and reasoning is already included in output. Do not double-count them when computing `AI-Total-Tokens`. Compute a monetary estimate only from documented model-specific input, cached-input, and output rates current on `AI-Pricing-As-Of`; otherwise record `unavailable`. Tool, sandbox, and third-party charges are outside the token estimate unless a separate documented trailer is added.

If usage becomes available only after the agent turn ends, prefer a deterministic external wrapper that reads the completed trace, appends the trailers, and creates the commit. Without such a wrapper, use `partial` for the latest exposed counts or `unavailable`; never substitute guessed counts.

## Result

Report the files changed, the important behavior change, checks performed, and any assumptions or unresolved risks. Distinguish completed verification from checks that were not possible. For repository-changing tasks, also report the task commit hash and exact commit message, or state why no commit was created.
