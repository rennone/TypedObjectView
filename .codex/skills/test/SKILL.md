---
name: test
description: Select and run focused tests for a repository change or defect, including reproduction, regression coverage, and failure triage. Use for verification work; do not use as an implementation procedure.
---

# Test

Follow `AGENTS.md`; it takes precedence over this skill. This skill defines verification practice, not authority, approval, or safety policy.

## Choose Evidence Proportionally

Select the narrowest set of checks that gives credible evidence for the change and its likely regressions.

- For a defect, reproduce the failure before the fix when feasible, confirm the same case after the fix, and cover the nearest plausible regression paths.
- For new behavior, cover the expected path plus meaningful boundaries, invalid input, and error behavior where relevant.
- For a refactor, verify preserved observable behavior and run focused checks for the touched integration points.
- For high-impact or shared code, expand from targeted tests to the relevant suite, build, or integration checks.

Prefer behavioral assertions over tests coupled to private implementation details. Add or update automated coverage when it materially protects the intended behavior; avoid tests that only restate the code.

## Interpret Failures

When a check fails, capture the exact check and relevant output, then determine whether the failure is:

- introduced by the current change;
- pre-existing and independently reproducible; or
- unresolved because available evidence is insufficient.

Use a clean comparison or a narrower reproduction when practical. Do not modify unrelated code merely to make the suite green, and do not label a failure pre-existing without evidence.

## Report Verification

Record the checks run and their outcomes. If a check cannot run, state the concrete reason, what remains unverified, and the resulting risk. Never report an unexecuted or incomplete check as passing.

Keep implementation decisions outside this skill; return implementation gaps to the appropriate task owner.
