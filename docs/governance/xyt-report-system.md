# XYT-F2 Report Module Contract

## Ownership

`xyt.ps1` remains the single central entry owner. The Report subsystem owns only `XYT/Report/**`, its schema, and the independent governance wrapper `scripts/governance/xyt-report.ps1`. This module does not modify or dot-source the central entry automatically.

## Contract

Future Integration Owners may dot-source `XYT/Report/xyt-report.ps1` and call:

```text
Invoke-XytReport -Operation Report|Aggregate|Upload ...
Invoke-XytReportTest ...
```

`Report` creates JSON and Markdown T reports. Required identity fields are Version, full Commit, Branch, TestMode, TestSetVersion, AffectedCapabilities, and UTC Timestamp. Status values are `PASS`, `FAIL`, `BLOCKED`, `TIMEOUT`, and `FLAKY`; raw evidence references are retained.

`Aggregate` supports month, quarter, and year UTC half-open windows and preserves all five statuses. `Upload -DryRun` validates a run report and optional aggregate without committing or pushing.

## Verification boundary

The module selftest is independent of `xyt.ps1` and covers report creation, identity validation, all status values, month/quarter/year aggregation, and GitHub Upload DryRun. Entry wiring is intentionally deferred to a separate Integration Owner.
