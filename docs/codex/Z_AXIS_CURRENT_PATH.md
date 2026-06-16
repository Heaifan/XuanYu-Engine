# Z Axis Current Path

Date: 2026-06-15

## Repair note

The Z key path described in the Phase 0 audit has been repaired on 2026-06-15:

- `EditorShell.HandleRawKeyDown` now routes X/Y/Z through `SwitchMoveAxis`.
- Switching to Z calls `BuildVerticalMoveAnchor` and `EntityMoveSession.ReanchorVertical`.
- Switching back from Z to X/Y calls `TryBuildGroundMoveAnchor` and `EntityMoveSession.ReanchorGround`.
- `EntityMoveSession.UpdateVertical` now preserves current preview X/Y instead of restoring move-start X/Y.
- Regression coverage now includes `ReanchorGround_AfterVerticalMove_AllowsXConstraint`.

## High-level path

Current scene-tool input path:

```text
Win32 WndProc
-> WindowsVulkanViewportHostControl
-> NavigationPointerPressed
-> SceneToolPointerPressed
-> EditorShell.HandleSceneToolPointerPressed
-> EditorShell.TryStartMoveSession
-> VulkanSceneRayBuilder.TryBuild
-> ScenePointerPicker.Pick
-> EntityMoveSession.Begin
-> WindowsVulkanViewportHostControl SetCapture
-> WM_MOUSEMOVE
-> EditorShell.HandleRawPointerMoved
-> EditorShell.UpdateMoveSession
-> GroundMoveProjection or VerticalMoveProjection
-> EditorShell.ApplyEntityTransform
-> WorldState.SetPosition
-> RenderSceneObjectPositionWriter.Update
-> VulkanScene3dSession.UpdateEntityPosition
-> ScheduleScene3dFrame
```

Requested conceptual path and current status:

```text
Keyboard Z / Gizmo Z
-> NativeHost
-> InputRoute
-> EditorShell
-> EntityMoveSession
-> Projection
-> Preview
-> RenderScene
```

Current repository facts:

- Keyboard Z during a move is disabled in `EditorShell.HandleRawKeyDown`.
- No gizmo-axis hit path was found in this Phase 0 audit; move starts by entity picking, not by an axis handle.
- `BuildVerticalMoveAnchor` exists but has no confirmed caller.
- `EntityMoveSession.TryUpdateVertical` can map Z from a `VerticalMoveAnchor`, but the keyboard Z path does not create that anchor.

## NativeHost priority

`WindowsVulkanViewportHostControl` handles `WM_LBUTTONDOWN` in this order:

1. Navigation overlay.
2. Scene tool.
3. Legacy picking.

When `SceneToolPointerPressed` returns `BeginDrag`:

- `_leftButtonHandledBySceneTool = true`
- `_sceneToolDragCaptured = true`
- `SetCapture` is called.
- `_pickInput.OnDown()` is not called.
- `RawPointerButtonDown` is not called.

On `WM_LBUTTONUP` after scene-tool drag:

- `SceneToolPointerReleased` is called.
- `_pickInput.OnUp()` is not called.
- Legacy picking is blocked.

This means the current NativeHost event consumption path appears to block entity picking and ground picking after move-tool drag starts.

## Move start

`EditorShell.HandleSceneToolPointerPressed` starts a move only when:

- Move tool is active.
- No move session is already active.
- `TryStartMoveSession(x, y)` succeeds.

`TryStartMoveSession` requires:

- Active editor session.
- Active `VulkanScene3dSession`.
- Selected world entity.
- Native host dimensions.
- Valid `LastPresentedSnapshot`.
- Successful `VulkanSceneRayBuilder.TryBuild`.
- `ScenePointerPicker.Pick` hits the selected entity.
- Current entity position exists.

It then builds a `GroundMoveAnchor`:

```text
targetZ = selected entity Z
ray = VulkanSceneRayBuilder.TryBuild(pointer, LastPresentedSnapshot)
t = (targetZ - ray.Origin.Z) / ray.Direction.Z
planeHit = ray.Origin + ray.Direction * t
anchor = GroundMoveAnchor(initialPosition, planeHit, startPointer, t, abs(ray.Direction.Z))
```

Initial mapping mode:

- `PlaneIntersection` when ray-plane intersection is considered reliable.
- `ScreenDeltaFallback` when the start intersection is low-angle or otherwise unreliable.

Then:

```text
EntityMoveSession.Begin(entityId, initialPosition, GroundPlane, anchor, initialDirty, initialMode)
```

## X and Y path

During pointer move:

```text
EditorShell.HandleRawPointerMoved
-> UpdateMoveSession(pixelX, pixelY, deltaX, deltaY)
```

If current axis is not Z:

- In `ScreenDeltaFallback`, `FallbackToScreenDelta(deltaX, deltaY, host.Height)` is used.
- In `PlaneIntersection`, a fresh ray is built from the latest `LastPresentedSnapshot`.
- The ray is intersected with the horizontal plane at the initial entity Z.
- `GroundMoveProjection.Map(anchor, worldPos, axis)` computes:

```text
delta = currentPlaneHit - anchor.InitialPlaneHit
GroundPlane -> (delta.X, delta.Y, 0)
X -> (delta.X, 0, 0)
Y -> (0, delta.Y, 0)
Z -> (0, 0, 0)
target = anchor.InitialEntityPosition + constrainedDelta
```

The fallback path is incremental:

```text
GroundMoveProjection.MapScreenDelta(currentPosition, cameraRight, deltaX, deltaY, worldPerPixel, axis)
-> currentPosition + constrained screen delta
```

So the fallback mode depends on per-frame pointer deltas, not only on a fixed start pointer and current pointer.

## Z path

There are two Z mechanisms in the current code.

### Legacy incremental Z

`EntityMoveSession.UpdateVertical(deltaPixelY, worldPerPixel)` calls obsolete:

```text
VerticalMoveProjection.ApplyToPosition(currentPosition, initialPosition, deltaPixelY, worldPerPixel)
-> new Vector3d(initialPosition.X, initialPosition.Y, currentPosition.Z + (-deltaPixelY * worldPerPixel))
```

Confirmed behavior:

- Z is accumulated from current Z.
- X/Y are reset to `initialPosition.X/Y`.
- Existing failing tests prove this causes X/Y jumps after switching from XY to Z.

### Anchor-based Z

`WorldAxisScreenProjection.TryCreateVerticalAnchor`:

```text
project pivot
project pivot + Vector3d.UnitZ
screen axis = normalized projected axis
worldUnitsPerPixel = 1 / projectedAxisPixelLength
```

When projected Z length is below `MinAxisPixels = 2.0`, it falls back to:

```text
screenAxis = (0, -1)
worldUnitsPerPixel = fallbackWorldUnitsPerPixel
```

`VerticalMoveProjection.TryMap`:

```text
deltaX = currentPointerX - anchor.StartPointerX
deltaY = currentPointerY - anchor.StartPointerY
axisPixels = deltaX * anchor.ScreenAxisX + deltaY * anchor.ScreenAxisY
targetZ = anchor.InitialZ + axisPixels * anchor.WorldUnitsPerPixel
```

`EntityMoveSession.TryUpdateVertical` then returns:

```text
target = (initialPosition.X, initialPosition.Y, targetZ)
```

This path uses total pointer displacement from the anchor start, not per-frame deltas.

## Z activation facts

`EditorShell.HandleRawKeyDown` contains special handling while moving:

```text
X -> EntityMoveAxis.X
Y -> EntityMoveAxis.Y
Z -> null, disabled
```

For X/Y:

- If a vertical anchor exists, it is cleared.
- `SetAxisConstraint(newAxis)` is called.

For Z:

- No `BuildVerticalMoveAnchor` call occurs.
- No `BeginVertical` or `ReanchorVertical` call occurs.
- A log message tells the user Z viewport drag is under reconstruction and to use the inspector.

Therefore, switching from X/Y to Z does not currently rebuild a Z anchor through the keyboard route because the route is disabled.

## Camera snapshot facts

Current move path uses `_scene3dSession.LastPresentedSnapshot`:

- At move start in `TryStartMoveSession`.
- On each non-Z pointer move in `UpdateMoveSession`.
- In `BuildVerticalMoveAnchor`.

No frozen drag snapshot object was found. The current non-Z plane path can observe a newer presented camera snapshot during drag. The Z anchor path would freeze the projected screen axis only if an anchor is created, but Phase 0 did not find a connected caller for `BuildVerticalMoveAnchor`.

## Preview and transaction facts

`UpdateMoveSession` always calls:

```text
ApplyEntityTransform(_moveSession.CurrentPosition, MoveTool)
```

`ApplyEntityTransform` performs:

- `WorldState.SetPosition`
- `RenderSceneObjectPositionWriter.Update`
- `VulkanScene3dSession.UpdateEntityPosition`
- `_worldDirtyState.MarkDirty`
- status dirty update
- inspector text update
- frame scheduling

The method suppresses per-frame info logs for `MoveTool`, but it still marks dirty during preview. This does not match the desired transaction rule where preview should not formally mark dirty and confirm should commit once.

`ConfirmMoveSession`:

- Releases capture.
- Subscribes to `Completed`.
- Calls `_moveSession.Confirm()`.

`OnMoveCompleted` on confirm:

- Calls `ApplyEntityTransform` again if the position changed.
- Writes one info log.

`CancelMoveSession`:

- Releases capture.
- Calls `_moveSession.Cancel()`.

`OnMoveCompleted` on cancel:

- Calls `ApplyEntityTransform` with the initial position.
- Resets dirty only if `InitialWasDirty` was false.

## Confirmed root cause from current tests

The current failing tests confirm one root cause in the legacy Z path:

```text
SetAxisConstraint(EntityMoveAxis.Z)
-> UpdateVertical(deltaY, worldPerPixel)
-> VerticalMoveProjection.ApplyToPosition
-> X/Y taken from _initialPosition, not current preview position
```

That makes X/Y jump when a user switches from XY movement into legacy Z movement after already previewing a different X/Y.

## Known degradation conditions

- `WorldAxisScreenProjection` degrades when projected Z-axis length is under 2 pixels and silently uses vertical screen fallback.
- `GroundMoveProjection` degrades to per-frame screen delta when plane intersection is unreliable.
- The requested future behavior says degenerate projected axes should become unavailable below 6 pixels instead of manufacturing movement.

## Test coverage facts

Existing tests cover:

- `EntityMoveSession` direct begin/update/confirm/cancel behavior.
- Legacy vertical failures.
- `GroundMoveProjection` direct math.
- `VerticalMoveProjection.TryMap` direct math.
- `VulkanSceneRayBuilder` separately in render camera tests.

Existing tests do not yet cover:

- NativeHost event consumption through a real input route.
- A production-chain path from `PresentedCameraSnapshot` through ray build, input route, solver/session, and preview.
- Frozen camera snapshot during drag.
- Real axis switching from X/Y to Z through the editor route.
- One-move vs 100-move invariance through production input.
