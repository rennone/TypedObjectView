---
name: coordinate
description: Coordinate non-trivial project work by decomposing scope, ordering dependencies, assigning non-overlapping Worker ownership, and integrating verified results. Use when a change has multiple workstreams or materially benefits from delegation; skip for a single small direct task.
---

# Coordinate Project Work

`AGENTS.md` defines authority, approvals, safety, and scope boundaries and always takes precedence. Use this skill only for the execution mechanics below.

## Choose the Work Shape

1. Start from the required deliverable and decisions that must be settled before implementation.
2. Split work only at boundaries that produce independently reviewable results.
3. Order dependent work explicitly. If an investigation can change the implementation design, complete and review it before assigning implementation.
4. Run tasks in parallel only when neither consumes the other's result and their file ownership does not overlap.
5. Delegate when it reduces Manager context, creates useful parallelism, or enables independent verification. Keep a small task with the Manager when transferring context would cost more than doing it.

Assign one owner to each file or component. When overlap is unavoidable, sequence the edits or give one Worker edit ownership and another read-only review ownership.

## Apply the Agent Profiles

- Keep the Manager on `gpt-6-astra` with `high` reasoning when that profile is available.
- Create at most three Workers, each using `gpt-6-sol` with `medium` reasoning when that profile is available.
- Only the Manager creates Workers. Explicitly prohibit Workers from creating descendant agents in every delegated task.
- If the runtime requires a limited or empty history fork to override the Worker model, pass the needed context in the task contract instead of inheriting the entire Manager conversation.
- If a configured model or reasoning level is unavailable, use the closest profile for the same role and report the substitution to the human.

Use stable labels such as `Worker-1`, `Worker-2`, and `Worker-3` for the life of the task. Record each label's model and role for later commit usage attribution.

## Delegate with a Task Contract

Every Worker assignment must state:

```text
Objective:
The concrete result to produce.

Scope:
Files and components the Worker may inspect or modify.

Constraints:
Decisions already made, boundaries, and actions that require escalation.

Expected output:
Artifacts and findings the Worker must return.

Verification:
Checks the Worker should run and report.
```

Include enough local context for independent execution, but do not transfer unresolved product or architecture decisions to a Worker.

## Require a Reviewable Result

Ask Workers to return concise results in this shape when applicable:

```text
Result:
Files:
Changes:
Verification:
Issues:
Recommendation:
```

`Issues` must distinguish confirmed problems, assumptions, unresolved questions, and work intentionally left out of scope.

## Integrate as Manager

Before accepting delegated work:

- Inspect critical source and diffs directly; a Worker summary is not sufficient for consequential decisions.
- Check the result against the original requirement, assigned scope, project architecture, compatibility, error handling, maintainability, and relevant tests.
- Resolve conflicting assumptions and inconsistent interfaces across Worker results before combining them.
- For important changes, separate implementation from focused verification. The verifier should seek missed edge cases, regressions, and requirement violations rather than repeat the implementation.
- Correct small integration issues directly; return substantial or task-local problems to the owning Worker with a narrowed follow-up contract.

After review and verification finish, capture the best available recorded usage for the Manager and each Worker. Include retries under the agent that performed them, and preserve `input`, `cached input`, `output`, `reasoning`, and `total` as separate fields. Mark missing data as unknown rather than zero. Hand this usage record to the `implement` Skill for the task commit; do not ask Workers to guess or self-report token counts.

Report the integrated outcome to the human in terms of the result, important design decisions, major changes, verification, and remaining issues. Keep internal coordination details out unless they explain a decision or risk.
