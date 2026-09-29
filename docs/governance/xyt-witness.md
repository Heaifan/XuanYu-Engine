# XYT-J Regression Witness R1

## Purpose

Regression Witness proves that one unchanged test is `FAIL` on the pre-fix
commit and `PASS` on the post-fix commit. It is mandatory for a user-found
bug, a regression of a formally accepted capability, or a T0/T1 False PASS /
False Freeze incident. Small development-time mistakes may use ordinary tests.

## Contract

The input record must identify `WitnessId`, `BugId`, `IncidentId`,
`Capability`, `PreFixCommit`, `PostFixCommit`, `TestId`, and `TestSetVersion`.
It also records `PreFixTestId`, `PostFixTestId`, both results, and evidence
references. The two execution Test IDs must equal `TestId`; a different test
cannot form a RED-to-GREEN witness.

The required replay is:

```text
PRE-FIX: PreFixCommit + the same TestId -> FAIL
POST-FIX: PostFixCommit + the same TestId -> PASS
```

Missing old-state evidence can only produce `INCOMPLETE`; it can never produce
`RED_CONFIRMED`. A Witness does not close an Incident automatically.

## Statuses

`RED_CONFIRMED` means the pre-fix failure is evidenced and post-fix execution
has not happened. `GREEN_CONFIRMED` means the complete pair is evidenced.
`INCOMPLETE` means required identity/evidence is missing or post-fix still
fails. `INVALID` means the claimed witness is logically contradictory, such as
pre-fix PASS or different Test IDs.

## Usage

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\scripts\governance\xyt-witness-gate.ps1 `
  -InputPath .\witness-input.json `
  -JsonPath .\witness-report.json `
  -MarkdownPath .\witness-report.md
```

The JSON report follows `xyt-witness-schema.json`; the Markdown report is a
human-readable projection of the same computed status. F Report may consume
the JSON report without invoking the central `xyt.ps1` entry point.
