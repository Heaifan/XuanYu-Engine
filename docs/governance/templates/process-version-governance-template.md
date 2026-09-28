# Process Version Governance Template

This portable template applies to any repository, project, language, runtime, or AI workflow. It standardizes process semantics, not the version string format.

## Frozen principles

1. Version Number is Process Telemetry; Commit SHA is Historical Identity.
2. One independently completed Feature creates one `FEATURE` Version Event.
3. One independently completed Bug Fix creates one `FIX` Version Event.
4. Every real consecutive repair round remains visible; FIX1/FIX2/FIX3 cannot be compressed.
5. A formal Build, Runtime, Incident, Knowledge, and Acceptance record stores Version + Commit SHA.
6. Dirty artifacts are not formal baselines unless explicitly named as a Dirty Runtime Probe.
7. A machine-readable one-event-per-row ledger supports Version Metrics and friction review.
8. Metrics feed Knowledge, Lesson, EXP, regression tests, and machine gates.
9. Planning and Execution Knowledge Preflight are both required.
10. A project may choose its existing valid version string format, but may not silently cancel the one-event-per-Feature or one-event-per-Fix semantics.

## Required adoption surface

Each adopting project provides a current-version reader, a read-only next-version calculator, an audit, a self-test, metrics, a ledger, and an Acceptance Identity report containing Version, Commit, Branch, and Dirty. The project records historical ambiguity as partial coverage instead of fabricating events. Project-specific suffixes and numeric fields remain local; the semantic event types and identity rules remain portable.

## Required closeout

```text
VERSION EVENT
Change Type: FEATURE | FIX | STABILIZATION | GOVERNANCE | NONE
Previous: <value>
Next: <value or NONE>
Reason: <evidence>

ACCEPTANCE IDENTITY
Version: <value>
Commit: <SHA>
Branch: <branch>
Dirty: NO
Identity: <Version>@<SHA>
```
