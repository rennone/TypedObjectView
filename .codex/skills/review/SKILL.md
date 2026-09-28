---
name: review
description: Review a proposed repository change for requirement fit, correctness, regressions, compatibility, maintainability, and test gaps. Use when evaluating existing work; do not repeat the implementation or broaden into unrelated redesign.
---

# Review

Follow `AGENTS.md`; it takes precedence over this skill. This skill defines review practice, not authority, approval, or safety policy.

## Review Against the Intended Change

Read the requirement, relevant diff, and enough surrounding code to understand observable behavior. Check for:

- requirement mismatches or omitted cases;
- incorrect logic, state transitions, boundary handling, or assumptions;
- regressions in adjacent callers and shared behavior;
- incomplete error handling, cleanup, or failure propagation;
- compatibility changes to public APIs, persisted data, configuration, or supported environments;
- complexity or duplication that creates a concrete maintenance risk;
- missing, weak, or misleading tests for behavior changed by the patch.

Do not request stylistic churn unless it causes a specific correctness, consistency, or maintenance problem. Do not reimplement the change during review; focus on finding actionable problems and explaining them.

## Write Actionable Findings

Order findings by severity. For each finding, include:

- a concise title and severity;
- the smallest useful file and line range;
- the condition that triggers the problem;
- the observable impact;
- the reasoning or evidence that makes it a defect; and
- a direction for correction when it is not obvious.

Use severity to express impact and urgency, not preference. Separate confirmed defects from questions and residual risks.

If no actionable findings remain, say so explicitly and summarize any verification limits or areas not exercised. Keep the overall summary secondary to the findings.
