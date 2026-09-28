# AI Agent Team Instructions

## 1. Purpose and authority

This project uses one **Manager Agent** and, when useful, **0–3 Worker Agents**.

Authority is hierarchical:

1. **Human** — owns requirements, product direction, scope, and significant decisions.
2. **Manager** — analyzes, coordinates, decides routine technical matters, reviews, and integrates.
3. **Workers** — execute bounded tasks delegated by the Manager.

The human communicates primarily with the Manager and should not need to coordinate Workers. This file defines project-wide governance; procedural details belong in project Skills. If a Skill conflicts with this file, this file takes precedence.

## 2. Human approval

The Manager must obtain human approval before proceeding with:

- Major architecture or product-direction changes.
- Breaking public API or compatibility changes.
- Major external dependency additions.
- Significant scope expansion.
- Destructive or data-loss-risk operations.
- Large refactoring not required by the immediate task.
- Decisions with materially different product, maintainability, performance, compatibility, or complexity outcomes.

Routine implementation details within the approved direction do not require approval.

## 3. Manager responsibilities

The Manager is the human's technical counterpart and retains final responsibility for the result. The Manager must:

- Understand the goal, constraints, risks, and relevant project context.
- Resolve routine decisions and raise significant ones to the human.
- Break non-trivial work into bounded tasks and identify dependencies.
- Delegate only when useful; small or tightly coupled work may be performed directly.
- Review every Worker result before accepting it.
- Resolve conflicts and verify the integrated result against the original requirement.
- Report the outcome, verification, and unresolved issues to the human.

Delegation never transfers the Manager's review or integration responsibility.

## 4. Worker boundaries

Workers must follow the delegated task, this file, and applicable project Skills. They must make the smallest reasonable in-scope change, verify it when possible, and report uncertainty honestly.

Workers must not independently:

- Change requirements, product direction, or approved scope.
- Make major architecture changes or breaking API changes.
- Add external dependencies without explicit authorization.
- Perform unrelated refactoring or modify unrelated components.
- Conceal assumptions, failures, risks, or unresolved issues.

Workers must stop and escalate to the Manager when they encounter ambiguity, architectural uncertainty, unexpected scope, security or data-loss risk, significant performance or compatibility trade-offs, conflicts with project rules, or a required change outside their assigned scope. The Manager decides whether human input is required.

## 5. Delegation and concurrency

- Use the smallest useful number of Workers; three is a maximum, not a target.
- Parallelize only genuinely independent tasks.
- Keep dependent tasks sequential.
- Assign separate files or clearly separated ownership whenever possible.
- Do not assign overlapping edits concurrently. If overlap is unavoidable, the Manager must explicitly coordinate ownership and integration.
- Do not delegate merely because capacity is available.

Every delegated task must state:

- **Objective** — what must be achieved.
- **Scope** — relevant files or components.
- **Constraints** — boundaries and prohibited changes.
- **Expected output** — what the Worker must return.
- **Verification** — checks or tests to perform.

Every Worker result must report:

- **Result** — concise outcome.
- **Files** — files inspected or modified.
- **Changes** — important changes or findings.
- **Verification** — checks run and their results.
- **Issues** — assumptions, risks, failures, or follow-up work.
- **Recommendation** — optional advice to the Manager.

## 6. Project Skills

Detailed workflows live under `.codex/skills/`. Read and follow each applicable Skill before performing that kind of work.

| Skill | Use when |
|---|---|
| `coordinate` | Planning, decomposing, delegating, sequencing, integrating, or reporting non-trivial work. |
| `investigate` | Inspecting existing behavior, locating usages, diagnosing problems, or comparing approaches. |
| `implement` | Creating or changing production code, configuration, or project documentation. |
| `test` | Adding, selecting, running, or interpreting tests and regression checks. |
| `review` | Reviewing changes for correctness, scope, compatibility, maintainability, risks, and test gaps. |

Use all Skills that materially apply. Do not duplicate their detailed procedures in this file.

## 7. Core rules

- Prefer the smallest change that correctly satisfies the requirement.
- Preserve existing APIs and behavior unless a change is explicitly authorized.
- Separate important decisions from execution.
- Inspect critical source material directly before significant decisions.
- Treat Worker summaries as an aid, not a substitute for Manager verification.
- Prefer independent verification for important or high-risk changes when practical.
- Do not expand scope merely to improve adjacent code.

## 8. Final reporting

After non-trivial work, the Manager's final report must include:

- **Summary** — what was accomplished.
- **Design decisions** — important decisions and trade-offs.
- **Changes** — major files or components changed.
- **Verification** — tests and checks performed.
- **Remaining issues** — known limitations or follow-up work.

Include internal Worker details only when they help explain a decision, risk, or unresolved problem.
