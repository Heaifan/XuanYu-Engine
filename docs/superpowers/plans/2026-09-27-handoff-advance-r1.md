# Handoff Advance R1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add a protected `handoff advance` state migration so an active Wave can accept subsequent synchronized commits without recreating the Wave.

**Architecture:** Extend the existing `tools/handoff/handoff.ps1` command dispatcher and reuse its current state/config/fact helpers. Add fixture-based PowerShell selftests with a real bare remote, and document the `join → advance → join` lifecycle in the existing handoff protocol.

**Tech Stack:** PowerShell, Windows Git, existing `handoff.cmd` wrapper, JSON state under `.git/xye-handoff/state.json`.

**Spec:** User-provided HANDOFF-ADVANCE-R1 task brief in the attached text file.

## Global Constraints

- `advance` must not reset, clean, stash, checkout, switch, merge, rebase, commit, push, or modify product files.
- `join` remains strict and read-only; `prepare` remains blocked while an Active Wave exists.
- ForeignDirty is preserved and state.json remains outside Git.
- Advance is allowed only when branch, remote tip, ahead/behind, active Wave, and ancestor guards pass.

## Review Focus

- Unpushed local commits must never advance baseline: covered by local-ahead fixture.
- Remote-behind and remote-diverged states must preserve the old baseline: covered by convergence fixtures.
- A forked old baseline must fail fast: covered by ancestor fixture.
- Dirty files must remain byte-for-byte unchanged: covered by ForeignDirty fixture.
- A successful advance must unblock the original join deadlock without allowing prepare: covered by two-step E2E fixture.

### Task 1: Protected advance command and diagnostics

**Files:**
- Modify: `tools/handoff/handoff.ps1`
- Modify: `tools/handoff/handoff.cmd`

- [ ] Add `advance` to mode parsing and implement guarded state migration.
- [ ] Add explicit join/prepare diagnostics that recommend `advance` only when safe.
- [ ] Verify the command never mutates the working tree or product files.

### Task 2: Regression and real-repository simulation

**Files:**
- Modify: `tools/handoff/handoff.selftest.ps1`

- [ ] Build a temporary repository with a bare remote and exercise noop, pass, all guards, dirty preservation, and A→B→C E2E.
- [ ] Run the selftest and retain exact failure reasons for regressions.

### Task 3: Bootstrap documentation and acceptance gate

**Files:**
- Modify: `tools/handoff/HANDOFF-PROTOCOL.md`
- Modify: `docs/governance/sync-handoff-sop.md`

- [ ] Document coordinator-only `advance` after synchronized commit/push.
- [ ] Document the `BASELINE_MOVED` stop/resolution path and prohibit manual state edits.
- [ ] Run diff checks and the complete handoff selftest.
