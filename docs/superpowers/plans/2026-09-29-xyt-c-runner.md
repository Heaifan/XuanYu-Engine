# XYT-C Runner and Minimum Sufficient Test Set

## Goal

Generate a test set from Git changes using fixed Ownership/Capability mappings, preserve Agent additions, and report disputes instead of guessing.

## Files

- `scripts/governance/xyt-runner.ps1`: CLI orchestration and exit status.
- `scripts/governance/xyt-runner.mapping.ps1`: input and JSON helpers.
- `scripts/governance/xyt-runner.compute.ps1`: fixed mapping, union, and dispute calculation.
- `scripts/governance/xyt-runner.report.ps1`: JSON/human report.
- `docs/governance/xyt-test-mapping.json`: fixed fallback rules.
- `scripts/governance/xyt-runner.selftest.ps1`: executable contract checks.
- `docs/governance/xyt-runner-contract.md`: user-facing contract.

## Verification

- Run `scripts\governance\xyt-runner.selftest.ps1`.
- Run `git diff --check`.
- Run the existing governance selftests serially.
- Check every new hand-written PowerShell file is at most 100 lines.

## Scope decisions

Exact Handoff Ownership wins over fallback patterns. Capability/test selection is a union, never a subtraction. Unknown mappings return exit code 2 and `REVIEW_REQUIRED`; this is the user notification boundary.
