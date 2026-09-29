# XYT-K / Evidence Expiry & Revalidation R1

## Purpose

XYT evidence is time- and change-bounded. A historical PASS remains a record of
what was observed, but is not current proof after a related contract or test
chain changes. This mechanism never turns a trigger into `PRODUCT FAIL`.

## Record contract

Each evidence record contains `Commit`, `Version`, `Capability`, `TestId`,
`TestSetVersion`, `EvidenceLevel`, `CreatedAt`, `InvalidationTrigger`, and
`Status`. Status is one of `VALID`, `REVALIDATION_REQUIRED`, `EXPIRED`, or
`SUPERSEDED`. A clean, immutable Commit is the historical identity; Version is
process/product context and does not replace the Commit.

## Invalidation

`scripts/governance/xyt-evidence.ps1 -Operation Invalidate` reads a change
document and changes only matching `VALID` records to
`REVALIDATION_REQUIRED`. Supported triggers are:

- Capability Contract Changed
- Render Pipeline Changed
- Swapchain Lifecycle Changed
- Input Route Changed
- Terrain Pipeline Changed
- Runtime Host Changed
- Test Oracle Changed
- Test Set Major Version Changed

Capability-scoped triggers match the exact capability. Test-set major changes
also require a changed `TestSetVersion`; Oracle changes remain capability
scoped. Unsupported changes such as documentation-only edits do not invalidate
evidence. Therefore a P0 change cannot contaminate an unrelated P3 capability.

## Development and closure

`-Operation Plan` emits a `Revalidation Plan` containing only records in
`REVALIDATION_REQUIRED` or `EXPIRED`, with strategy
`MINIMUM_SUFFICIENT_TEST_SET`; it does not run every historical test.

`-Operation Closure -Phase Development` returns `ALLOW_DEVELOPMENT`, even when
revalidation is required. `-Phase Closure` returns `BLOCKED` when any declared
critical capability lacks `VALID` evidence, including missing evidence. This
keeps development unblocked while making module Freeze / release strict.

## Selftest

Run `scripts/governance/xyt-evidence.selftest.ps1`. It covers Render invalidation,
documentation-only changes, unrelated P3 isolation, Test Set Major and Oracle
changes, minimum-plan generation, Development allowance, and Closure blocking.
