# XYT First-Op Hotfix R1 G2 Version Event Governance Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Make XYT enforce that every formally accepted FIX/FEATURE has exactly one registered Version Event, and that only an accepted event with the matching applied version can pass the commit gate.

**Architecture:** Add a small PowerShell version-event library that reads a TSV ledger, validates event state and product version transitions, and emits a machine-readable gate result. Keep the historical `version-events.tsv` compatible while adding explicit event fields and provisional records. Wire the selftest into XYT fast/module/global closeout and expose a formal gate command for commit eligibility.

**Tech Stack:** PowerShell 5.1-compatible scripts, TSV ledger, existing `scripts/resolve-version.ps1`, XYT PowerShell dispatcher.

**Spec:** User task `XYT-FIRST-OP-HOTFIX-R1-G2` in the current conversation.

## Global Constraints

- CHANGE_TYPE is only `FIX` or `FEATURE` for new product events.
- New events start `RESERVED`; `RESERVED` and `PROVISIONAL` never consume a version.
- Only `ACCEPTED` → `APPLIED` consumes one counter and must match the current product version.
- A missing event, missing bump, wrong version, duplicate event, or skipped number yields `VERSION GATE = BLOCKED`, `COMMIT ELIGIBILITY = NO`, and `PUSH = DENIED`.
- Preserve historical governance rows and current dirty/ForeignDirty files.
- All hand-written `.ps1` files remain within the repository 5+100 rule.

## Review Focus

- A FIX/FEATURE with no event must block even when tests pass: `Test-MissingEventBlocks`.
- An accepted event whose applied version is absent or wrong must block: `Test-AcceptedWithoutBumpBlocks` and `Test-WrongVersionBlocks`.
- A reserved/provisional event must not increment counters: `Test-PendingDoesNotConsume`.
- Reusing an event ID or applying two events to one version must block: `Test-DuplicateEventBlocks`.
- Two accepted FIX events must consume two consecutive versions: `Test-ConsecutiveFixesAdvance`.

### Task 1: Define the event ledger and failing gate selftest

**Files:**
- Modify: `docs/governance/version-events.tsv`
- Create: `scripts/governance/version-event-gate.selftest.ps1`
- Create: `docs/governance/xyt-version-event-gate.md`

**Interfaces:**
- Ledger columns: `EventId`, `TaskId`, `Type`, `Name`, `BaselineVersion`, `AppliedVersion`, `Status`, `AcceptanceEvidence`, `AcceptedTime`, `CandidateId`, `CommitId`.
- Gate input fixture: `-ChangeType`, `-EventId`, `-CurrentVersion`, `-CandidateId`, `-FormalAcceptance`, `-LedgerPath`.

- [ ] Add provisional A/C and investigating B records without `AppliedVersion`.
- [ ] Add selftests for missing event, missing bump, wrong version, duplicate event, pending event, accepted event, and consecutive FIX events.
- [ ] Run the selftest before the implementation and confirm the expected failures are caused by the missing gate module.

### Task 2: Implement the PowerShell 5.1-compatible gate

**Files:**
- Create: `scripts/governance/version-event-gate.ps1`
- Modify: `tools/governance/version-lib.ps1`
- Modify: `tools/governance/version-metrics.ps1`

**Interfaces:**
- `Test-XytVersionEventGate` returns `Status`, `VersionGate`, `CommitEligibility`, `Push`, `Reason`, `FixCounter`, `FeatureCounter`, and `CandidateVersionBinding`.
- `Get-XytVersionEventLedger` parses both the new event schema and legacy rows without inventing formal events.
- Accepted application uses `Get-NextProcessVersion` and rejects gaps, duplicate IDs, duplicate applied versions, mismatched baseline, and candidate/version mismatch.

- [ ] Implement the parser and validation rules with no writes during gate evaluation.
- [ ] Run the selftest and confirm all required cases pass.
- [ ] Run existing version selftest and metrics to prove compatibility.

### Task 3: Integrate XYT closeout and formal gate output

**Files:**
- Modify: `xyt.ps1`
- Modify: `XYT/tests/xyt.selftest.ps1`
- Modify: `docs/governance/agent-completion-report-standard.md`

- [ ] Add `xyt version-gate` dispatch with formal gate parameters.
- [ ] Run version-event selftest from quick/module/global closeout.
- [ ] Emit the required report labels: `VERSION LEDGER`, `VERSION EVENT`, `FIX COUNTER`, `FEATURE COUNTER`, `MISSING EVENT GATE`, `MISSING BUMP GATE`, `DUPLICATE GATE`, `CANDIDATE VERSION BINDING`, `SELFTEST`, `POWERSHELL 5.1`, `PWSH`, `REGRESSION`, `GATE STATUS`.
- [ ] Run XYT selftests and verify the dispatcher remains compatible.

### Task 4: Verify, audit, and report

- [ ] Run planning and execution Knowledge Preflight.
- [ ] Run `git diff --check`, focused governance selftests, XYT fast/module tests, and version metrics.
- [ ] Record Version Event as `NONE` because this is governance-only and does not advance product Process Version.
- [ ] Produce changed-file counts, gate matrix, residual risks, and `KNOWLEDGE / EXPERIENCE AUDIT HANDOFF` with candidate lessons only.

