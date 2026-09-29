# XYE-P1-B DEM Navigation Performance Fix R1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Remove per-chunk/per-frame temporary allocations from DEM terrain culling and projected-pixel LOD selection while preserving camera-relative, perspective, orthographic, and reverse-Z behavior.

**Architecture:** Keep the public terrain APIs unchanged. Replace LINQ and temporary corner arrays in the two hot paths with fixed-size stack/local iteration, and retain camera-relative transformation through `ViewProjectionState.RenderOrigin`. Use one deterministic 60-frame benchmark over EMPTY, SMALL, and REAL_DEM-shaped 225-chunk inputs, plus focused regression tests.

**Tech Stack:** C#, .NET 10, xUnit, `GC.GetAllocatedBytesForCurrentThread`, `Stopwatch`, repository `scripts/xye-dotnet.ps1` gates.

**Spec:** TASK ID `XYE-P1-B-DEM-PERFORMANCE-FIX-R1` supplied in the user request.

## Global Constraints

- Do not reduce DEM precision, terrain data, render capability, formal LOD quality, or view distance.
- Preserve existing `RenderOrigin` semantics and fail the task if perspective, orthographic, or reverse-Z regressions appear.
- Keep all hand-written `.cs` files at or below 100 lines.
- Do not commit or push.

## Review Focus

- Camera-relative large-world coordinates remain equivalent before and after optimization; covered by `ViewProjectionStateTests`.
- Perspective, orthographic, and reverse-Z frustum decisions remain unchanged; covered by `TerrainVisibilitySelectorTests`.
- Empty, one-chunk, and 225-chunk navigation measurements use the same 60-frame protocol and report visible counts.
- No LINQ or array materialization remains in the per-chunk culling/LOD path.
- Allocation measurement is warmed up before sampling and reports repeatable values rather than a single un-warmed observation.

### Task 1: Establish the performance baseline and failing contract

**Files:**
- Modify: `XuanYu.World.Tests/Render/TerrainNavigationAllocationTests.cs`
- Test: `XuanYu.World.Tests/Render/TerrainNavigationAllocationTests.cs`

- [ ] Preserve the existing 60-frame EMPTY/SMALL/REAL_DEM-shaped cases and add explicit visible-chunk reporting plus stable baseline output.
- [ ] Run the focused test through the resolver-backed test command and record the current allocation/CPU/chunk evidence.

### Task 2: Remove LOD projection allocations

**Files:**
- Modify: `XuanYu.Core/Space/TerrainLodSelector.cs`
- Test: `XuanYu.Core.Tests/Render/TerrainVisibilitySelectorTests.cs`

- [ ] Add or strengthen a focused regression proving projected-pixel LOD behavior remains correct for front/behind and distance cases.
- [ ] Replace `Select`, `Where`, `ToArray`, and the subsequent `Select`/`Min`/`Max` chain with direct fixed-count iteration over the eight AABB corners.
- [ ] Run focused Core terrain tests and the benchmark; continue only if allocation and behavior evidence are both improved.

### Task 3: Remove frustum culling allocations

**Files:**
- Modify: `XuanYu.Core/Space/TerrainFrustumCuller.cs`
- Test: `XuanYu.Core.Tests/Render/TerrainVisibilitySelectorTests.cs`

- [ ] Replace `Corners(...).All(...)` with direct eight-corner/plane iteration while preserving the existing reverse-Z depth predicates.
- [ ] Run the focused Core terrain tests and benchmark again.

### Task 4: Regression and completion gates

**Files:**
- Verify only; no additional production scope.

- [ ] Run Camera-relative Perspective, Orthographic, and Reverse-Z tests.
- [ ] Run Terrain tests, relevant World tests, and the affected build through the repository resolver chain.
- [ ] Run `git diff --check`, inspect the scoped diff, and report BEFORE/AFTER absolute and percentage deltas without commit/push.

