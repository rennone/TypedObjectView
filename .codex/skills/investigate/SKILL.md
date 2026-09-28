---
name: investigate
description: Inspect existing project behavior before design or implementation by tracing relevant files, call and dependency paths, edge cases, risks, and precise change points. Use when current behavior or impact is unclear; keep the investigation read-only unless implementation is explicitly requested.
---

# Investigate Existing Behavior

`AGENTS.md` defines authority, approvals, safety, and scope boundaries and always takes precedence. Investigation does not grant permission to implement a fix.

## Establish the Question

State the behavior or decision the investigation must clarify and the evidence needed to answer it. Prefer focused entry points, tests, configuration, and documentation over loading an entire subsystem.

## Trace the Relevant Path

Follow the behavior far enough to explain it end to end:

- Identify relevant files and the role each one plays.
- Trace callers and callees, data transformations, state changes, error paths, and configuration that affects the behavior.
- Locate tests and public interfaces that define or imply compatibility requirements.
- Check boundary conditions suggested by the implementation, including empty or missing input, invalid state, failures, ordering, concurrency, and platform-specific paths when relevant.
- Separate directly observed facts from inferences. Tie claimed risks and edge cases to concrete code or tests.

Do not edit production code, tests, configuration, or documentation unless the assignment explicitly includes implementation. Do not fold cleanup or opportunistic refactoring into an investigation.

## Report Findings for a Decision

Return a concise report containing:

```text
Result:
The answer to the investigation question.

Relevant files:
Each file and why it matters.

Current behavior:
The observed flow, including important call or dependency relationships.

Edge cases and risks:
Evidence-backed failure modes, regressions, or compatibility concerns.

Change candidates:
The smallest likely change points and their trade-offs; no implementation unless requested.

Uncertainty:
What remains unverified, why, and what evidence would resolve it.

Verification:
Read-only checks performed and their results.
```

If the evidence contradicts the original premise, report that directly rather than forcing a recommendation around it.
