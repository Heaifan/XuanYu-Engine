# FIX-VIEWPORT-ZOOM-GROUND-DRIFT-R1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Restore pure viewport wheel zoom so camera orientation, observation pivot, and ReferencePlane projection remain stable while distance changes.

**Architecture:** Keep `CameraNavigation.TryDolly` as the single camera-geometry owner. The production wheel route will use non-anchored dolly; cursor-ray/ground intersection remains available to picking and authoring, but is not allowed to mutate the camera pivot during this pure zoom fix. Regression tests will assert orientation/pivot invariants and ReferencePlane ray intersections across repeated distances.

**Tech Stack:** C#/.NET 10, xUnit, Avalonia editor VM, CPU ViewProjection/WorldRay geometry.

**Spec:** `FIX-VIEWPORT-ZOOM-GROUND-DRIFT-R1` supplied in the user request.

## Global Constraints

- Canonical workspace: `E:\MyDoc\project-VSCode\XuanYuEngine`; no worktree or temporary repository.
- Scope is Camera Zoom / ReferencePlane / Projection only; do not expand terrain editing.
- Pure Zoom must preserve Camera Forward, Pitch/Yaw, ObservationCenter/Pivot, and FOV.
- Do not mask the issue with FarPlane expansion, hardcoded heights, full-screen ground drawing, or shader-only changes.
- All hand-written `.cs` / `.axaml` / `.js` files remain at most 100 lines.
- Automated PASS is not real-App acceptance; real Vulkan acceptance remains pending until performed by the user.

## Review Focus

- Production wheel input must not silently enter cursor-anchored pivot mutation; covered by the production composition invariant test.
- Zoom-out must retain a valid screen-ray → ReferencePlane intersection at 1m, 5m, 20m, 100m, and 500m; covered by the distance matrix test.
- Reverse-Z near/far endpoints must continue to generate a forward ray; covered by the ReferencePlane ray test.
- Twenty zoom-in/out pairs must return camera position, orientation, and pivot within tolerance; covered by the round-trip test.
- Existing cursor-anchor unit coverage must remain explicit about its lower-level API semantics and must not be mistaken for pure production wheel behavior.

### Task 1: Add failing production and geometry regressions

**Files:**
- Modify: `XuanYu.World.Tests/Viewport/InputIntegration/CameraWheelInputCompositionTests.cs`
- Create: `XuanYu.World.Tests/Viewport/InputIntegration/ViewportZoomGroundDriftRegressionTests.cs`

- [ ] Add a production wheel test that records `ObservationCenter`, Forward, Up, FOV, and a projected world reference point before repeated off-center wheel events, then asserts the pure-zoom invariants.
- [ ] Add a distance-matrix test that applies wheel deltas corresponding to 1m, 5m, 20m, 100m, and 500m camera distances and asserts a valid ReferencePlane intersection and finite projection.
- [ ] Add a 20-cycle zoom-in/out round-trip test with explicit tolerances.
- [ ] Run the focused tests and capture the expected failure from the current cursor-anchored production route.

### Task 2: Restore pure production Dolly routing

**Files:**
- Modify: `XuanYu.Editor.UI/Input/UiVmD1Handler.cs`
- Modify: `XuanYu.Editor.UI/Vm/Camera/UiVm.CameraDolly.cs` only if the minimal route requires it.

- [ ] Route production wheel handling to the pure `DollyCamera` path so `ObservationCenter` is unchanged.
- [ ] Preserve the existing `CameraNavigation.TryDolly` geometry and do not alter ReferencePlane, Reverse-Z, or FarPlane policy without new evidence.
- [ ] Run the new focused tests and the existing camera/input regression set.

### Task 3: Gate and handoff

- [ ] Run affected project build, focused tests, architecture checks, 5+100, and `git diff --check` through the repository Resolver Chain.
- [ ] Attempt real App validation only through the canonical launcher; if unavailable, report it as unverified with reproduction steps.
- [ ] Report Root Cause, Changed Files, Tests, Evidence, Known Risk, Candidate Lessons, Version Event, Acceptance Identity, and the required XYK audit handoff.
- [ ] Do not commit or push without explicit user approval.
