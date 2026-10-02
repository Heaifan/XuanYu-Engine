# Shared Dependency Handoff H3

`tools/governance/ownership/dependency-handoff.ps1` is the bounded Ownership Authority for shared-file transfer. It stores its graph in `.git/xye-handoff/dependency-state.json`; this runtime state is local evidence and is not a product artifact.

## Lifecycle

1. The owner records `IMPLEMENTATION_COMPLETE`, `OwnScopePass`, and `HandoffReady`.
2. `release` records `FromLane`, `ToLane`, `File`, `OldRevision`, `OldFingerprint`, `TransferPoint`, `Reason`, and `RegressionImpact`. Release does not change implementation status.
3. `acquire` is valid only for the named recipient and only when the file fingerprint is unchanged since release. An old owner cannot write after transfer; a pre-acquire modification is rejected.
4. `modify` requires the active owner. Evidence that consumes the changed file becomes `STALE_BY_DEPENDENCY_CHANGE`; its lane keeps implementation completion and receives `regressionRequired=true`.
5. After the new owner completes, `regression` revalidates the affected evidence. `integration` is blocked while any evidence remains stale or regression-required.

## Independence and deadlock

Evidence is invalidated by exact dependency-file membership, not by lane or workspace dirtiness. Therefore a change to `Bar.cs` cannot stale evidence that consumes only `Foo.cs`. Transfer rejects a non-empty `UnknownDirty` set with `UNKNOWN_DIRTY_BLOCKED`.

`deadlock -Wait A>B,B>A` and longer chains such as `A>B,B>C,C>A` return `CIRCULAR_HANDOFF_DEPENDENCY` plus the concrete cycle chain. No Product PASS/FROZEN prerequisite is used for acquire or release.

## Verification

Run `powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File tools/governance/ownership/dependency-handoff.selftest.ps1`. The selftest covers normal transfer, stale evidence, preserved implementation completion, unrelated files, two/three-node cycles, old-owner rejection, acquire-before-release, pre-acquire modification, UnknownDirty blocking, audit fields, regression, and integration closure.
