# Handoff Close Authority T0 Fix R1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Separate Lane completion from Workspace Wave closure and prevent non-Coordinator Lane sessions from closing an Active Wave.

**Architecture:** Keep the existing command dispatcher and lifecycle files compatible, but delegate Lane/global close validation to a small close-authority helper. Lane close records a terminal lane state and releases only that lane's task, ownership, and work-release records; global close requires Coordinator authority and a fully quiescent Wave.

**Tech Stack:** PowerShell, JSON state under `.git/xye-handoff`, fixture-based selftests, existing `handoff.cmd`.

**Spec:** User task `HANDOFF-CLOSE-AUTHORITY-T0-FIX-R1` and `docs/governance/handoff-task-flight-plan-contract.md`.

## Global Constraints

- Ordinary Lane close must not write `active=false` for the Workspace Wave.
- Global close requires matching Coordinator scope and no active lanes, tasks, ownership, work release, mutex, or unresolved registrations.
- Existing `prepare`, `join`, `advance`, `migrate-active`, and `WIP_RESUME` behavior remains compatible.
- No XYE product files; no `git add .`; all hand-written `.ps1` additions stay within 100 lines.
- PRE-FIX evidence is reported only if the old witness is actually observed; no fabricated RED result.

## Review Focus

- C Lane close with A/B still joined leaves the Wave active.
- Non-Coordinator global close is rejected without mutating state.
- Coordinator global close is blocked by active Lane, task, ownership, work release, or mutex state.
- Repeated Lane close is safe and post-close rejoin does not create ghost state.
- Closed-Wave Lane close cannot rewrite historical state.

### Task 1: Add failing incident witness

**Files:**
- Modify: `tools/handoff/handoff.selftest.ps1`

- [ ] Add `NonCoordinatorCannotCloseActiveWave` coverage using an isolated real Git fixture with A/B/C lane state and assert the legacy global close deactivates the Wave before the fix.
- [ ] Run the focused selftest and record the actual pre-fix result class.

### Task 2: Implement close authority and Lane close

**Files:**
- Create: `tools/handoff/close-authority.ps1`
- Modify: `tools/handoff/handoff.ps1`
- Modify: `tools/handoff/handoff.cmd` only if command help/forwarding requires it

- [ ] Add `lane-close` dispatch and delegate to the helper.
- [ ] Require active Wave and reject closed-Wave mutation.
- [ ] Mark only the requested lane terminal, release lane task/ownership/work-release records, and preserve the Wave active flag.
- [ ] Make repeated Lane close idempotent or safely rejected.
- [ ] Require matching Coordinator scope for global close.
- [ ] Reject active lanes, active tasks, unreleased ownership/work-release, held commit mutex, and unresolved task registrations.
- [ ] Preserve legacy R2 states for JOIN/status and require explicit Coordinator authority for global close.

### Task 3: Complete regression matrix and protocol evidence

**Files:**
- Modify: `tools/handoff/handoff.selftest.ps1`
- Modify: `tools/handoff/HANDOFF-PROTOCOL.md`
- Modify: `docs/governance/process-versioning.md` or the existing version event ledger only if the formal FIX event entry requires it

- [ ] Cover Cases 1–7 from the task, including migration and WIP resume compatibility.
- [ ] Run the selftest before and after the fix where possible and report Witness evidence honestly.
- [ ] Run `git diff --check`, 5+100, focused governance selftests, and the applicable version gate.

### Task 4: Coordinator closeout

**Files:**
- No additional product files.

- [ ] Perform precise staging, commit under Commit Mutex, push, verify remote tip, and run handoff close only after all Lane/resource checks pass.
- [ ] Report Acceptance Identity, Version Event, Candidate Tree Purity, unknown dirty state, commit/push evidence, and XYK Candidate handoff.
