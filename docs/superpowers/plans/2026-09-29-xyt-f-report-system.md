# XYT-F Report System Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Generate auditable single-run T reports, period aggregates, and GitHub-ready uploads with immutable version/commit/branch/test identity.

**Architecture:** Keep the repository-root `xyt.ps1` as the entry point. Store machine-readable run facts beside human-readable Markdown reports, and aggregate only those facts. GitHub upload is an explicit exact-path push operation with preflight validation.

**Tech Stack:** PowerShell 5.1/pwsh, JSON, Markdown, Git, existing `scripts/resolve-version.ps1` and `scripts/xye-bootstrap.ps1` resolver chain.

**Spec:** `docs/superpowers/specs/2026-09-29-xyt-report-system-design.md`

## Global Constraints

- Every formal report binds Version, Commit, Branch, Test Mode, Affected Capability, Test Set Version, and Timestamp.
- Commit is the historical identity; dirty reports are explicitly non-formal runtime probes.
- Existing primary Gate, Constitution, Report System, and Handoff Core remain authoritative.
- Do not stage unrelated dirty files; preserve ForeignDirty.

## Review Focus

- Missing required identity must fail before a file is written.
- Dirty workspace must remain visible and must not be presented as a clean acceptance baseline.
- Quarter/year boundary calculations must not leak adjacent reports.
- Aggregation must retain FAIL and BLOCKED rather than deriving an optimistic PASS.
- Upload failure must leave artifacts recoverable and must not silently push unrelated files.

### Task 1: XYT report engine

**Files:** Create `xyt.ps1`, modify `XYT/tests/xyt.selftest.ps1` only if required.

- [ ] Write failing selftests for required identity validation and report generation.
- [ ] Run the selftest and observe failure because `XYT/xyt.ps1` is missing.
- [ ] Implement identity collection, JSON/Markdown run output, and compatible quick/module/global modes.
- [ ] Run the selftest and confirm PASS.

### Task 2: Period aggregation

**Files:** Extend `XYT/xyt.ps1`; create `XYT/tests/xyt.aggregate.selftest.ps1`.

- [ ] Write failing tests for month, quarter, and year windows plus retained failure status.
- [ ] Run them RED.
- [ ] Implement deterministic UTC window filtering and aggregate JSON/Markdown output.
- [ ] Run them GREEN.

### Task 3: GitHub upload contract

**Files:** Extend `XYT/xyt.ps1`; create `XYT/tests/xyt.upload.selftest.ps1`.

- [ ] Write failing tests for missing report, invalid identity, dirty formal upload, and exact-path manifest.
- [ ] Run them RED.
- [ ] Implement upload preflight and guarded exact-path push operation.
- [ ] Run them GREEN without pushing the shared dirty workspace.

### Task 4: Governance documentation and verification

**Files:** Create `docs/governance/xyt-report-system.md`, update `docs/docs-index.md` only if ownership permits.

- [ ] Document report schema, commands, status semantics, and GitHub workflow.
- [ ] Run all XYT selftests, governance selftests, `git diff --check`, and 5+100 for changed hand-written files.
- [ ] Record `Version Event: NONE` because this is governance/report infrastructure, then perform exact-path Git review.
