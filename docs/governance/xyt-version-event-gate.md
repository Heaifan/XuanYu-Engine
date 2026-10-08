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

Product `Type` values are `FIX`, `FEATURE`, and `STABILIZATION`. `GOVERNANCE`
records a material governance-only result and never advances product version.
`RESERVED`, `PROVISIONAL`, and `INVESTIGATING` product rows do not count. A
formal product row must reach `APPLIED`, contain
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

For `GOVERNANCE`, supply the exact EventId and current `BaselineVersion`. The
event is eligible without `-FormalAcceptance` only when the row is registered,
its baseline matches the current product version, it does not advance that
version, and it has no product Candidate binding. This is a commit-preflight
classification, not product acceptance or a release.

The gate has two explicit phases. The default `AUTO` selects `PRE-COMMIT` when
`-RequireClean` is absent and `POST-COMMIT` when it is present, preserving
existing callers. Pass `-Phase` explicitly in formal closeout commands.
`PRE-COMMIT` validates the registered event
against the prepared Candidate while pending staged changes are expected. Do
not pass `-RequireClean` in this phase; it is a post-commit identity check and
rejects the call when paired with `PRE-COMMIT`. The pre-commit coordinator must
separately verify the exact staged Candidate, staged diff-check, scope/purity,
tests, and applicable architecture gates.

After a successful commit, run `-Phase POST-COMMIT -RequireClean` to verify the
working tree is clean. For `GOVERNANCE`, POST-COMMIT additionally requires the
ledger row to be `APPLIED`, an acceptance timestamp, and a `CommitId` that names
an existing commit reachable from HEAD. A clean branch alone is insufficient.
This confirms clean identity only; it does not replace
the pre-commit Candidate checks, establish remote equality, or waive any global
architecture gate. Governance events never waive those checks.

`xyt quick`, `xyt module`, and `xyt global` run the gate selftest. The explicit
`xyt version-gate` command is the formal closeout entry point.

## Current provisional events

`XYT-FIRST-OP-HOTFIX-R1-G2-A` and `-C` are provisional FIX candidates;
`-B` is investigating and cannot consume a version. The first accepted event
must be applied as `v0.3.0.1-fix`; the next independent FIX then uses
`v0.3.0.2-fix` after its baseline is updated to the first applied version.
