# WAVE-2.5-D2 Map Edit Consumer Migration Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** Add tested Map Geometry, Region, Road, and Marker consumers to the existing Phase-1 input model, with Snap as an in-owner helper and complete shared cancellation cleanup.

**Architecture:** Extend the existing Router with explicit begin qualification and deterministic arbitration, then implement four small map consumers over a shared map-edit backend contract. The consumers receive only `EditorPointerEvent` and lifecycle contexts; the backend owns existing map-session operations and temporary-state cleanup. No Vulkan/Host production route is changed.

**Tech Stack:** C#, .NET, xUnit, XuanYu.Editor input contracts, existing Editor.UI map editing/session APIs.

**Spec:** `docs/superpowers/specs/2026-09-24-wave-2-5-d2-map-edit-consumer-migration-design.md`

## Global Constraints

- Work only in branch `feat/wave-2.5-d2-map-edit-consumer-migration` from `00d8f147b1ffda5eabab2ae3a308f32c00b16258`.
- Do not touch the current checkout, its 32 user changes, Camera, Picking, Gizmo, Vulkan, `VulkanNativeHost`, `Win32ViewportHost`, Production Viewport, Production Input Route, Schema, or WAVE-3.
- Reuse `EditorPointerEvent`, `ViewportInputRouter`, `GestureOwner`, `ViewportGestureLifecycle`, `IViewportInputConsumer`, and `IViewportPointerCaptureCoordinator`.
- Snap is a helper/constraint owned by the active Map Geometry, Region, Road, or Marker consumer; it is not a competing business owner.
- Every hand-written `.cs` file added or materially expanded must remain at or below 100 lines.
- Preview must not commit World state, History, Inspector refresh, or a transaction; Cancel must clear Preview, Snap Candidate, Pressed/Hover, temporary geometry, transaction, Owner, Gesture, and Capture.

## Review Focus

- A pressed event matching two map tools must select exactly one explicit owner; test `Router_SelectsSingleEligibleMapOwner` in Task 1.
- A Snap candidate must be consumed by the active business owner without becoming `GestureOwner.SnapInteractionHelper`; test `Snap_RemainsHelperOfActiveOwner` in Task 2.
- Cancel after an active Preview must not call Commit and must clear every temporary-state flag; test `Cancel_ClearsTemporaryMapState` in Task 3.
- A second pointer or stale pointer event must not mutate the active map gesture; test `Router_RejectsStalePointerAndSecondBegin` in Task 1.
- Tool/mode/window/capture termination must all use the same lifecycle terminal path; test `AllCancellationSources_ClearMapGesture` in Task 4.

### Task 1: Explicit Router arbitration

**Files:**
- Modify: `XuanYu.Editor/Input/IViewportInputConsumer.cs`
- Modify: `XuanYu.Editor/Input/ViewportInputRouter.cs`
- Test: `XuanYu.World.Tests/Viewport/ViewportInputRouterMapArbitrationTests.cs`

**Interfaces:**
- Consumes: existing `EditorPointerEvent`, `ViewportGestureState`, and Lifecycle contracts.
- Produces: `IViewportInputConsumer.CanBegin(EditorPointerEvent, ViewportGestureState)`, deterministic highest-priority Begin selection, and a single active Owner.

- [ ] Write failing tests for two eligible consumers, stale pointer input, and second Begin rejection.
- [ ] Run the focused Router test and observe the expected missing arbitration behavior.
- [ ] Add a default `CanBegin` and `BeginPriority` contract without creating another Router or Owner type.
- [ ] Make Router select only eligible consumers at the highest priority and preserve existing active lifecycle dispatch/cancel semantics.
- [ ] Run the focused Router tests and existing Router/Lifecycle tests; expect all green.
- [ ] Run `git diff --check`, verify changed `.cs` files are ≤100 lines, and commit `feat(wave-2.5-d2): add explicit map owner arbitration`.

### Task 2: Shared map consumer contract and four Owner adapters

**Files:**
- Create: `XuanYu.Editor/Input/Map/MapEditingInputState.cs`
- Create: `XuanYu.Editor/Input/Map/IMapEditingInputBackend.cs`
- Create: `XuanYu.Editor/Input/Map/MapEditingInputConsumer.cs`
- Create: `XuanYu.Editor/Input/Map/MapGeometryInputConsumer.cs`
- Create: `XuanYu.Editor/Input/Map/RegionInputConsumer.cs`
- Create: `XuanYu.Editor/Input/Map/RoadInputConsumer.cs`
- Create: `XuanYu.Editor/Input/Map/MarkerInputConsumer.cs`
- Test: `XuanYu.World.Tests/Viewport/MapInputConsumerContractTests.cs`

**Interfaces:**
- Consumes: Router begin qualification and Lifecycle callbacks from Task 1.
- Produces: four consumers with Owners `MapEdit`, `Region`, `Road`, and `Marker`; backend callbacks `Begin`, `Update`, `Commit`, `Cancel`; helper-only Snap state.

- [ ] Write failing contract tests for owner identity, Begin/Update/Commit, Snap helper usage, and independent consumer qualification.
- [ ] Run focused tests and observe missing consumer types.
- [ ] Implement the shared consumer using injected backend callbacks and four thin owner-specific adapters; no platform-event or platform-capture access.
- [ ] Ensure Update passes the active Owner to the backend and Snap is represented as helper state only.
- [ ] Run focused consumer tests and `XuanYu.World.Tests` map-input subset.
- [ ] Check 5+100/diff and commit `feat(wave-2.5-d2): add map editing consumers`.

### Task 3: Map session backend and temporary-state contract

**Files:**
- Create: `XuanYu.Editor/Input/Map/MapEditingInputBackend.cs`
- Create: `XuanYu.Editor/Input/Map/MapEditingInputSnapshot.cs`
- Test: `XuanYu.World.Tests/Viewport/MapEditingTemporaryStateTests.cs`

**Interfaces:**
- Consumes: four consumer lifecycle callbacks from Task 2.
- Produces: a testable backend state machine that distinguishes Preview, Commit, and Cancel and clears Snap Candidate, Preview, Pressed/Hover, temporary Geometry, and Transaction on terminal paths.

- [ ] Write failing tests for Preview-only Update, Commit terminal, Cancel terminal, and cancel-not-commit.
- [ ] Run focused tests and observe missing backend behavior.
- [ ] Implement the backend snapshot/state transitions with injected operation callbacks; Snap resolution is invoked during Update only.
- [ ] Make all terminal paths idempotent and ensure a new gesture cannot inherit old temporary state.
- [ ] Run focused tests and map editing regression tests.
- [ ] Check 5+100/diff and commit `feat(wave-2.5-d2): enforce map preview and cancel cleanup`.

### Task 4: Unified cancellation and integration coverage

**Files:**
- Modify: `XuanYu.Editor/Input/ViewportInputRouter.cs`
- Test: `XuanYu.World.Tests/Viewport/MapInputCancellationIntegrationTests.cs`
- Test: `XuanYu.World.Tests/Viewport/MapInputConsumerRegressionTests.cs`

**Interfaces:**
- Consumes: Task 1 Router and Task 2/3 consumers/backend.
- Produces: D2 coverage for Escape, CaptureLost, FocusLost, WindowDeactivated, ToolChanged, ModeChanged, ViewportDisposed, continuous gestures, and no duplicate consumption.

- [ ] Write failing integration tests for every required cancellation source and repeated gestures.
- [ ] Run the focused integration tests and observe uncovered cancellation behavior.
- [ ] Route ToolChanged, ModeChanged, and ViewportDisposed through the existing `EditorPointerEvent`/Lifecycle cancel path without adding a second pipeline.
- [ ] Assert after every cancel: Owner=None, Gesture=Idle, Capture=None, Preview cleared, temporary Geometry/Transaction cleared, and Commit count unchanged.
- [ ] Run D2, Phase-1 Router/Lifecycle/Cancellation/Platform Parity, and map regression test groups serially.
- [ ] Check ARCH-A, ARCH-VIEWPORT, 5+100, and `git diff --check`; commit `feat(wave-2.5-d2): close map input cancellation coverage`.

### Task 5: Full gates and delivery evidence

**Files:**
- Modify only if required by a directly evidenced D2 gate failure; otherwise no additional files.

- [ ] Run the affected project builds and all D2/Phase-1 regression suites serially.
- [ ] Run Solution Build and record exact warning/error counts.
- [ ] Run ARCH-A, ARCH-VIEWPORT, 5+100, and `git diff --check`.
- [ ] Compare failures against the recorded 22 baseline failures and classify every failure; New Regression must be zero.
- [ ] Verify D1, Production Input Route, Vulkan, Production Viewport, and WAVE-3 boundaries by diff/path audit.
- [ ] Commit only evidence/documentation required by a real gate result, then push the feature branch and verify remote tip equality.
