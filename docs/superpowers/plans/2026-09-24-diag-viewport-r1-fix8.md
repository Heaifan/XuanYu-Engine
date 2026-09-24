# DIAG-VIEWPORT-R1-FIX8 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Make Native Viewport Diagnostic resolve and retain `XYE.VIEWPORT`, place its card near the Native pointer, and restore Avalonia probing after Native exit without changing production input ownership.

**Architecture:** Add a Diagnostic-only Native pointer lifecycle stream at the Win32/Vulkan boundary. `DiagnosticOverlayHost` owns a short-lived Native Viewport override and rejects Avalonia probe replacement while that override is active. Existing production pointer routing remains the consumer of Native input; placement remains a pure policy over pointer, card size, and Window ClientRect.

**Tech Stack:** C#, Avalonia, Win32 child HWND message routing, xUnit, Avalonia Headless runtime tests.

**Spec:** `docs/superpowers/specs/2026-09-24-diag-viewport-r1-fix8-design.md`

## Global Constraints

- All hand-written `.cs` / `.axaml` files touched or created must remain at or below 100 lines.
- Diagnostic Native observation must not set `Handled`, capture the pointer, consume input, or change production gesture owner.
- Do not modify `ViewportInputRouter`, `GestureOwner`, `ViewportGestureLifecycle`, Camera, Picking, Gizmo, Map, Region, Road, Marker, or Snap production behavior.
- `XYE.VIEWPORT` is the only semantic identity emitted by the Native Viewport Diagnostic override.
- Viewport placement may intersect Viewport bounds, must stay inside Window ClientRect, and must avoid the pointer hot zone.

## Review Focus

- Native Move followed by Avalonia UiWin Move must retain Native ownership — test in `DiagnosticNativeTargetOwnershipTests`.
- Native Exit and host disposal must clear ownership — test in `DiagnosticNativeTargetOwnershipTests`.
- Pointer-near-edge placement must avoid the hot zone without pushing the card outside the client — test in `DiagnosticPlacementPolicyEdgesTests`.
- Repeated Native Move must reuse the existing card/highlight — test in `DiagnosticNativeOverlayRuntimeTests`.
- Observer wiring must not alter production routing/capture — test in `DiagnosticNativePointerProbeTests` and existing input suites.

---

### Task 1: Define Native Diagnostic lifecycle and target snapshot

**Files:**
- Modify: `XuanYu.Editor.UI/Viewport/Vulkan/NativePointerMessage.cs`
- Modify: `XuanYu.Editor.UI/Viewport/Input/NativePointerSourceBoundary.cs`
- Modify: `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.Input.cs`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticNativeViewportEvent.cs`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticNativeTargetOwnershipTests.cs`

**Interfaces:**
- Produces `DiagnosticNativeViewportEvent(VulkanNativeHost Host, DiagnosticNativeViewportPhase Phase, double X, double Y)` for Diagnostic-only consumers.
- Produces `DiagnosticNativeViewportPhase.Entered`, `.Moved`, `.Exited`.
- Existing `NativePointerMessage` production values and route mapping remain unchanged except for observing `WM_MOUSELEAVE`.

- [ ] **Step 1: Write failing lifecycle tests**

  Add tests that invoke the host's Native message path with Move and Leave messages and assert the Diagnostic event sequence is `Entered, Moved, Exited`; assert the event payload uses logical coordinates and the host instance.

- [ ] **Step 2: Run the focused test and verify RED**

  Run:

  ```powershell
  dotnet test .\XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-restore --filter FullyQualifiedName~DiagnosticNativeTargetOwnershipTests
  ```

  Expected: compile/test failure because the lifecycle event and Leave message are not defined.

- [ ] **Step 3: Implement the minimal lifecycle stream**

  Add `WM_MOUSELEAVE` and `TrackMouseEvent` registration in the Win32 sink. Forward Leave without entering the production route. In `VulkanNativeHost`, track whether the host is diagnostically inside, emit Entered once, Moved for subsequent moves, and Exited on Leave, capture cancellation, native disposal, or sink detachment. Keep the existing `NativePointerMoved` event temporarily as a compatibility bridge only if existing tests require it.

- [ ] **Step 4: Run focused tests and existing Native input tests**

  Run the focused filter, then:

  ```powershell
  dotnet test .\XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-restore --filter FullyQualifiedName~NativePointer
  ```

  Expected: all pass with no production input route regressions.

- [ ] **Step 5: Commit**

  ```powershell
  git add XuanYu.Editor.UI/Viewport/Vulkan XuanYu.Editor.UI/Viewport/Input XuanYu.Editor.UI/Diagnostic/DiagnosticNativeViewportEvent.cs XuanYu.World.Tests/UiRuntime/DiagnosticNativeTargetOwnershipTests.cs
  git commit -m "feat(diag): expose native viewport lifecycle"
  ```

### Task 2: Add Native Viewport target ownership to DiagnosticOverlayHost

**Files:**
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.NativeViewportProbe.cs`
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.Interaction.cs`
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.Probe.cs`
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticProbeResult.cs`
- Create: `XuanYu.Editor.UI/Diagnostic/DiagnosticNativeViewportTarget.cs`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticNativeTargetOwnershipTests.cs`

**Interfaces:**
- `DiagnosticNativeViewportTarget.Create(host, pointer, bounds)` returns a fixed `DiagnosticProbeResult` with `DebugId = "XYE.VIEWPORT"` and `ControlType = "VulkanViewport"`.
- `DiagnosticOverlayHost` exposes internal/test-visible `HasNativeViewportOverride` and clears it on Exit, unload, deactivation, and Diagnostic Mode off.

- [ ] **Step 1: Write failing ownership tests**

  Cover: Native Move creates `XYE.VIEWPORT`; a later Avalonia UiWin result is ignored while override is active; Native Exit clears it; the next Avalonia result becomes current; disabling diagnostic clears current and locked Native state.

- [ ] **Step 2: Run the focused test and verify RED**

  Run the ownership filter from Task 1. Expected: failure on missing override state or incorrect UiWin replacement.

- [ ] **Step 3: Implement fixed Native target and override precedence**

  Subscribe/unsubscribe to the lifecycle event in the existing Native probe attach methods. On Entered/Moved, translate Native logical coordinates to `_floatingLayer`, update the fixed target snapshot, and render/reposition. In `OnProbePointerMoved`, check the active Native override before resolver dispatch; if active, remember the Avalonia pointer but do not call `ProbeHover`. On Exit, clear only Native override and let the next Avalonia event resolve normally.

- [ ] **Step 4: Run ownership and existing resolver tests**

  Run:

  ```powershell
  dotnet test .\XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-restore --filter FullyQualifiedName~DiagnosticNativeTargetOwnershipTests
  dotnet test .\XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-restore --filter FullyQualifiedName~DiagnosticProbeResolver
  ```

  Expected: all pass; `UiWin` cannot replace an active Native target.

- [ ] **Step 5: Commit**

  ```powershell
  git add XuanYu.Editor.UI/Diagnostic XuanYu.World.Tests/UiRuntime/DiagnosticNativeTargetOwnershipTests.cs
  git commit -m "fix(diag): preserve native viewport target ownership"
  ```

### Task 3: Lock pointer-hot-zone placement and reuse popup visuals

**Files:**
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticPlacementModels.cs`
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticPlacementPolicy.cs`
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.Drag.cs`
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.Probe.cs`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticPlacementPolicyEdgesTests.cs`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticNativeOverlayRuntimeTests.cs`

**Interfaces:**
- `DiagnosticPlacementRequest` carries the existing pointer and available client bounds plus the safe hot-zone distance.
- `DiagnosticPlacementPolicy.Place` returns a Viewport candidate that may overlap `TargetBounds` but never intersects the pointer hot zone and never exceeds `AvailableBounds`.

- [ ] **Step 1: Write failing edge and reuse tests**

  Add cases for pointer at each client edge and a card larger than one quadrant. Assert the card remains inside the client and does not intersect a hot-zone rectangle around the pointer. Add a runtime test that sends two Native moves for the same target and asserts the Popup object references are unchanged.

- [ ] **Step 2: Run focused tests and verify RED**

  Run the placement and native overlay filters. Expected: an edge case currently either intersects the pointer hot zone or falls back to the pointer origin; reuse assertion fails if rendering recreates visuals.

- [ ] **Step 3: Implement minimal placement and reuse changes**

  Add a pointer hot-zone rectangle to the placement request or derive it from `SafeDistance`; skip candidates intersecting it for Viewport targets. Keep fallback constrained and hot-zone safe. In Native Move handling, update card snapshot/position in place when the same host remains active; rebuild only on target identity or lock-state changes.

- [ ] **Step 4: Run placement and overlay tests**

  Run:

  ```powershell
  dotnet test .\XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-restore --filter FullyQualifiedName~DiagnosticPlacementPolicy
  dotnet test .\XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-restore --filter FullyQualifiedName~DiagnosticNativeOverlayRuntimeTests
  ```

  Expected: all pass, including existing SmallControl placement behavior.

- [ ] **Step 5: Commit**

  ```powershell
  git add XuanYu.Editor.UI/Diagnostic XuanYu.World.Tests/UiRuntime/DiagnosticPlacementPolicyEdgesTests.cs XuanYu.World.Tests/UiRuntime/DiagnosticNativeOverlayRuntimeTests.cs
  git commit -m "fix(diag): place viewport card near native pointer"
  ```

### Task 4: Verify diagnostic shutdown and production input isolation

**Files:**
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.Lifecycle.cs`
- Modify: `XuanYu.Editor.UI/Diagnostic/DiagnosticOverlayHost.ProbeCard.cs`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticNativePointerProbeTests.cs`
- Test: `XuanYu.World.Tests/UiRuntime/DiagnosticNativeOverlayRuntimeTests.cs`
- Test: existing `XuanYu.World.Tests/Viewport/InputIntegration/*`

- [ ] **Step 1: Write failing shutdown/isolation tests**

  Assert Diagnostic Mode off clears Native override, popup, card, and highlight. Assert observer invocation leaves the existing production route result and capture spy unchanged.

- [ ] **Step 2: Run tests and verify RED**

  Run the focused shutdown/isolation filters. Expected: stale Native target or visual remains until the lifecycle cleanup is implemented.

- [ ] **Step 3: Implement cleanup and observer assertions**

  Route all detach/deactivate/mode-off paths through one Native override clear method. Do not add any `Handled`, Capture, or production route mutation to the Diagnostic observer.

- [ ] **Step 4: Run GATE-M verification**

  Run serially:

  ```powershell
  dotnet build .\XuanYu.Editor.UI\XuanYu.Editor.UI.csproj --no-restore -m:1 -nr:false -p:BuildInParallel=false -p:UseSharedCompilation=false
  dotnet test .\XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-build --no-restore --filter FullyQualifiedName~Diagnostic
  dotnet test .\XuanYu.World.Tests\XuanYu.World.Tests.csproj --no-build --no-restore --filter FullyQualifiedName~Viewport
  git diff --check
  ```

  Also run the repository's applicable 5+100/architecture guard scripts and record any environment or pre-existing failures separately.

- [ ] **Step 5: Commit and report residual real-machine gate**

  ```powershell
  git add XuanYu.Editor.UI XuanYu.World.Tests
  git commit -m "test(diag): close viewport diagnostic fix8 gates"
  ```

  `run.bat` and the four specified visual checks remain required for real-machine acceptance; automated tests cannot claim that gate closed.
