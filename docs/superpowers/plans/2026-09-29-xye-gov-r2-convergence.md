# XYE-GOV-R2-CONVERGENCE Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Converge the completed A/B/C governance candidates into the existing governance system with explicit debt expiry, evidence tiers, regression witnesses, ownership-capability expiry, and an auditable recent-fix report.

**Architecture:** Preserve the existing primary Gate, Constitution, Report System, Handoff Core, and Knowledge Index. Add the three candidate gates as subordinate checks, register the unsupported wave-scoped ownership capability as deferred, and connect the rules through existing governance documents and templates.

**Tech Stack:** PowerShell 5.1/pwsh governance scripts, JSON registries, Markdown governance documents, Git/Handoff controls.

**Spec:** User task `XYE-GOV-R2-CONVERGENCE` and repository `AGENTS.md`.

## Global Constraints

- Coordinator Lane is `GOVERNANCE`; no product-code changes.
- UnknownDirty, staged changes, unexplained ForeignDirty, or candidate-tree mismatch stop convergence.
- Existing primary Gate, Constitution, Report System, and Handoff Core remain authoritative.
- No `git add .`; use exact-path staging only after all gates pass.
- Governance PASS must not be reported as DEM JITTER fixed.

## Review Focus

- Debt entries must have real activation triggers, valid envelopes, and closure evidence requirements; gate with `deferred-capability-gate`.
- T0–T4 must be monotonic, with Headless explicitly below real runtime and scale/temporal dimensions represented; gate with `test-evidence-gate`.
- Regression witnesses must distinguish PRODUCT, TEST_HARNESS, MIXED, and UNKNOWN, and reject retroactive fabricated pre-fix evidence; gate with `regression-witness-gate`.
- The recent-fix audit must preserve UNKNOWN instead of converting missing runtime/product evidence into PASS.

### Task 1: Candidate tree and governance preflight

**Files:**
- Create: `docs/superpowers/plans/2026-09-29-xye-gov-r2-convergence.md`
- Inspect: Handoff state, Git status, current governance candidates, Knowledge Index

- [ ] Record Planning and Execution Knowledge Preflight and establish the exact candidate tree/source map.
- [ ] Confirm no unknown or staged dirty state before implementation.

### Task 2: Governance integration and deferred capability registration

**Files:**
- Modify: `docs/dev-rules.md`
- Modify: `docs/governance/deferred-capabilities.json`
- Modify: existing Task/ Fix Closure / Gate / Knowledge Index and related governance documents

- [ ] Add subordinate-gate and feature/fix preflight rules without creating a second governance authority.
- [ ] Register `GOV-OWNERSHIP-LIFECYCLE-001` with an explicit valid envelope, activation trigger, blocked capability, and required closure.

### Task 3: Recent fix evidence audit

**Files:**
- Create: `docs/governance/audits/test-evidence-audit-20260926-20260928.md`

- [ ] Audit commits `7e7dc6ca`, `b949df52`, `bee02894`, `81a8ecde`, `f09245a8`, `aa285b53`, `3e09d6df`, and `b95b2e79`.
- [ ] Record root causes, evidence tiers, witness status, oracle quality, runtime gap, and product acceptance gap without inventing evidence.

### Task 4: Verification and convergence

**Files:**
- Verify: all candidate scripts, governance documents, Handoff tests, and exact candidate tree

- [ ] Run A/B/C selftests, PowerShell 5.1 and pwsh checks, 5+100, architecture/existing governance gates, and `git diff --check`.
- [ ] Re-run Handoff/Git audit, acquire the governance commit mutex, exact-stage only, commit, push, verify `HEAD == remote` and `0/0`, then advance the active baseline.

