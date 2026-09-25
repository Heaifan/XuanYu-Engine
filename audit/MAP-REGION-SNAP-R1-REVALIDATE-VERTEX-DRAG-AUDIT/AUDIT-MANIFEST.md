# MAP-REGION-SNAP-R1 Vertex Drag Audit Manifest

## Confirmed root cause

`UiVm.MapGeometryEditing.TryBeginMapGeometryPointer` required `IsGeometryEditingActive` before starting a selected-region vertex drag. The selected region displayed vertices, but Pointer Down was rejected unless the separate inspector toggle had entered the exact internal state.

## Fix

- A selected region vertex can start a drag directly.
- Clicking the selected region interior still does not start a drag.
- Map selection and geometry selection remain synchronized.
- Direct first-vertex closure remains covered.

## Included complete source and test files

- `XuanYu.Editor.UI/Vm/Map/UiVm.MapGeometryEditing.cs`
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapRender.cs`
- `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.cs`
- `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.Close.cs`
- `XuanYu.Editor.UI/Input/UiVmMapBackend.cs`
- `XuanYu.Editor/Input/Map/MapEditingInputConsumer.cs`
- `XuanYu.Editor/Input/ViewportInputRouter.cs`
- `XuanYu.Editor/MapEditing/MapGeometryHitTester.cs`
- `XuanYu.World.Tests/UiRuntime/FeatureEditSelectionResetTests.cs`
- `XuanYu.World.Tests/UiRuntime/RegionDrawingF2PolygonTests.cs`
- `MAP-REGION-SNAP-R1-REVALIDATE-REQUIREMENTS.md`

## Verification

- New selected-region direct-drag regression: PASS.
- Region selection, direct close, safety, and geometry workflow suite: 12 passed, 0 failed, 0 skipped.
- World test project build: 0 warnings, 0 errors.
- `git diff --check`: PASS.
- Manual `run.bat` acceptance: PASS (user-confirmed through the official entry point).
- R1: FREEZE APPROVED.
