# Unified ViewportInputRouter + Gesture Owner Arbitration Implementation Plan

> This task uses native execution in the current Codex session. No sub-agents or delegated workers are used.

**Goal:** Add a platform-neutral viewport input router with one stable gesture owner, lifecycle, and capture/release coordination without migrating existing consumers.

**Architecture:** `EditorPointerEvent` remains the platform-neutral event. `ViewportInputRouter` owns lifecycle and arbitration; `IViewportInputConsumer` is the future consumer seam; `IViewportPointerCaptureCoordinator` is the single capture/release seam. The router knows only owner identities and dispatch results, never camera/map/gizmo business.

**Tech Stack:** C#, .NET 10, existing `XuanYu.Editor` input model, xUnit tests in `XuanYu.World.Tests`.

**Spec:** `docs/wave-2.5-input-router-owner-task.md` at frozen task commit `367b6be0f5205be321bdbcf79da84195e5e906e3`.

## Global Constraints

- Only add Router, Gesture Owner, lifecycle, result, capture/release infrastructure and focused tests.
- Do not modify Camera, Picking, Gizmo, Region, Road, Marker, Snap, production Viewport, or Vulkan.
- Do not add dependencies or change shared package configuration.
- Every hand-written `.cs`, `.axaml`, and `.js` file remains at or below 100 lines.

## Review Focus

- Press arbitration chooses only the first claiming consumer and establishes the owner.
- Active moves/releases/cancellation route only to the owner.
- Observers cannot claim or replace the owner.
- Wheel dispatch works while idle and does not create a drag owner.
- Capture and release happen once, including cancel and capture loss.

### Task 1: Lock the contract with tests

Create focused tests for owner acquisition/retention/release, non-owner exclusion, wheel dispatch, capture/release, cancellation, repeated gestures, and result semantics.

### Task 2: Implement the platform-neutral infrastructure

Add small files under `XuanYu.Editor/Input`: owner enum, lifecycle enum/state, dispatch result, consumer/capture interfaces, and `ViewportInputRouter`. The router dispatches observations to all consumers, claims to the first claimant, routes active gestures only to the owner, and resets to Idle on release/cancel/capture loss.

### Task 3: Verify boundaries and gates

Run focused tests, affected project build, ARCH-A, 5+100, `git diff --check`, and a production-consumer/Vulkan scope diff audit. Update the task report with exact Git and gate evidence, then commit, push, and verify remote equality.
