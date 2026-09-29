# XYT-D Test Execution Engine

## Boundary

XYT-D consumes a C-generated Test Plan and executes it. It does not select
tests, modify `xyt.ps1`, `xyt-runner.ps1`, Registry, Report, or Incident.

```powershell
scripts\governance\xyt-executor.ps1 -PlanPath .xyt\test-plan.json -OutputPath .xyt\execution-result.json
```

## Test Plan input

The JSON root contains `planId` and `tests`. Each test contains `testId`,
`command`, optional `dependsOn`, `workingDirectory`, `maxDurationSeconds`,
and `timeoutSeconds` (default 300). `maxDurationSeconds` is a cost budget;
`timeoutSeconds` is an execution deadline and produces `TIMEOUT`.

## Execution rules

- All ready tests without dependencies execute in the same scheduling wave.
- A dependency is runnable only after every dependency is `PASS`.
- A failed attempt is retried once. FAIL then PASS is `FLAKY`; it is never
  promoted to PASS. Two failures remain stable `FAIL`.
- `TIMEOUT` is independent of FAIL and is never classified as a product bug.
- A dependency failure, timeout, flaky, unclassified result, or cycle yields
  `BLOCKED_BY` for its dependent when the dependency is known.
- Malformed or unresolved execution input yields `UNCLASSIFIED`.
- Failure results expose `rootCause=UNKNOWN`; XYT-D never infers `PRODUCT`.
- Exceeding the identity's max duration adds `COST WARNING` and does not
  change status or fail a run by itself.

## Standard Execution Result

The result root has `schemaVersion=XYT-D-EXEC-v1.0`, `planId`, timestamps,
an overall status, and `results[]`. Each result includes `testId`, status,
attempt count, timestamps, duration, exit code/output, root cause (when
applicable), `costWarning`, and warnings. Supported statuses are `PASS`,
`FAIL`, `BLOCKED_BY`, `TIMEOUT`, `FLAKY`, and `UNCLASSIFIED`.

## Selftest

Run `XYT\Execution\xyt-executor.selftest.ps1`. It covers parallel overlap,
dependency serialization, stable FAIL, FLAKY, TIMEOUT, BLOCKED_BY, failure
sweep continuation, cost warning, and UNKNOWN/UNCLASSIFIED handling.
