# XYT Version Event Gate

XYT owns formal product version application. Agent code changes, passing tests,
and a reserved event do not increment the product version.

## Ledger contract

`docs/governance/version-events.tsv` is the authoritative event ledger. New
product rows use these fields:

```text
EventId, TaskId, Type, Name, BaselineVersion, AppliedVersion, Status,
AcceptanceEvidence, AcceptedTime, CandidateId, CommitId
```

New `Type` values are only `FIX` and `FEATURE`. `RESERVED`, `PROVISIONAL`, and
`INVESTIGATING` rows do not count. A formal row must reach `APPLIED`, contain
evidence, use the exact next version calculated from `BaselineVersion`, and
bind `CandidateId@AppliedVersion` to the Candidate Fingerprint.

## Gate

```powershell
scripts\governance\version-event-gate.ps1 -ChangeType FIX `
  -EventId XYT-FIRST-OP-HOTFIX-R1-G2-A `
  -CurrentVersion v0.3.0.1-fix `
  -CandidateId XYT-CAND-A `
  -CandidateFingerprint XYT-CAND-A@v0.3.0.1-fix `
  -FormalAcceptance -RequireClean
```

The gate blocks missing events, wrong or skipped bumps, duplicate Event IDs or
Applied Versions, incomplete acceptance, and candidate/version mismatches. A
blocked result emits `COMMIT ELIGIBILITY: NO` and `PUSH: DENIED`.

`xyt quick`, `xyt module`, and `xyt global` run the gate selftest. The explicit
`xyt version-gate` command is the formal closeout entry point.

## Current provisional events

`XYT-FIRST-OP-HOTFIX-R1-G2-A` and `-C` are provisional FIX candidates;
`-B` is investigating and cannot consume a version. The first accepted event
must be applied as `v0.3.0.1-fix`; the next independent FIX then uses
`v0.3.0.2-fix` after its baseline is updated to the first applied version.
