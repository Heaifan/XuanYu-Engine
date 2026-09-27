# Handoff R2 Parallel Session Stability Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Let already-joined Sessions survive legal fast-forward commits while serializing only Git commit/push/advance through a durable handoff mutex.

**Architecture:** Change JOIN from exact HEAD equality to an ancestor-anchor check. Add a lock file beside the existing handoff state with explicit `commit-lock`, `commit-unlock`, and owner-aware `advance` commands; the lock is released only after successful advance or safe no-commit unlock.

**Tech Stack:** PowerShell, Windows Git, existing `handoff.cmd`, JSON files under `.git/xye-handoff/`.

**Spec:** User-provided HANDOFF-R2-PARALLEL-SESSION-STABILITY requirements in the current conversation.

## Global Constraints

- Parallel work is allowed after JOIN; only `git add`, `commit`, `push`, and `advance` are serialized.
- JOIN accepts `baselineHead == HEAD` or `baselineHead` as a strict ancestor of `HEAD`.
- ForeignDirty is always preserved; no `git add .`, reset, clean, stash, checkout, merge, or rebase is introduced.
- Mutex state is local handoff state and is never committed or pushed.

## Review Focus

- A foreign fast-forward must make a second JOIN pass without changing state.
- A second owner must not acquire or advance while the first owner holds the mutex.
- A failed advance must retain the mutex for recovery and preserve the old baseline.
- A no-commit unlock must be safe, while unlock after an unadvanced commit must be blocked.
- Existing R1 advance and prepare/join diagnostics must remain compatible.

### Task 1: Ancestor-anchor JOIN

**Files:** `tools/handoff/handoff.ps1`, `tools/handoff/handoff.selftest.ps1`

- [ ] Add a failing fixture assertion that JOIN passes after another owner creates and pushes a descendant commit before advance.
- [ ] Implement the ancestor check while retaining branch and fork protection.
- [ ] Verify JOIN remains read-only and state/dirty files are unchanged.

### Task 2: Commit mutex lifecycle

**Files:** `tools/handoff/handoff.ps1`, `tools/handoff/handoff.selftest.ps1`

- [ ] Add failing fixture assertions for lock acquisition, contention, owner mismatch, safe unlock, and automatic release after advance.
- [ ] Implement atomic mutex JSON creation/removal and owner-aware command parsing.
- [ ] Require the mutex for baseline-changing advance and preserve it on failure.

### Task 3: Documentation and gates

**Files:** `tools/handoff/HANDOFF-PROTOCOL.md`, `docs/governance/sync-handoff-sop.md`

- [ ] Document parallel JOIN, commit mutex ownership, precise staging, and serialized Git closeout.
- [ ] Run the complete real-repository fixture, diff check, and current canonical join/status verification.
