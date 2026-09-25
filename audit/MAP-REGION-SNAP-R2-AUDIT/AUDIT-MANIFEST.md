# MAP-REGION-SNAP-R2 Audit Manifest

## Scope

This package records the R2 runtime connection from region drawing to the existing Vertex → Edge snap pipeline, plus the requested road linear-geometry vertex migration.

## Root cause addressed

The repository already contained the R2 edge resolver and hysteresis pipeline, but region drawing still called the legacy vertex-only resolver. As a result, edge candidates worked in isolated pipeline tests but were never reached by the real drawing preview/input path.

## R2 changes

- Region drawing now uses `RegionSnapPipeline` with the existing 8 px enter / 12 px release settings.
- Snap status distinguishes `顶点吸附`, `边吸附`, `Alt 已取消吸附`, and `未吸附`.
- Existing R1 Alt refresh and pointer lifecycle remain the only modifier path.
- The existing resolver keeps vertex priority, exact projected world coordinates, deterministic tie-breaks, committed-region filtering, and no topology mutation.

## Road vertex migration

- `InsertRoadVertex` inserts a point between a valid pair of road points and records one existing map-history entry.
- `DeleteRoadVertex` removes one selected point while preserving the minimum two-point line.
- The Inspector exposes `加顶点` and `删顶点` for a selected road vertex. Insertion uses the midpoint of the adjacent segment; the inserted point remains selectable for later dragging.
- Region topology, schema, and shared-edge behavior are unchanged.

## Complete changed production files

- `XuanYu.Editor.UI/Right/InspectorPanel.axaml`
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapGeometryEditing.Bindings.cs`
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapGeometryEditing.RoadVertices.cs`
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapGeometryEditing.cs`
- `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.Snap.cs`
- `XuanYu.Editor/MapEditing/MapEditSession.Geometry.cs`

## Complete changed test files

- `XuanYu.World.Tests/MapEditing/MapEditSessionGeometryTests.cs`
- `XuanYu.World.Tests/UiRuntime/FeatureEditSelectionResetTests.cs`
- `XuanYu.World.Tests/UiRuntime/FeatureEditSelectionResetTests.RoadVertices.cs`
- `XuanYu.World.Tests/UiRuntime/RegionDrawingF1BTests.cs`
- `XuanYu.World.Tests/UiRuntime/RegionDrawingF1BTests.VertexCount.cs`
- `XuanYu.World.Tests/UiRuntime/RegionDrawingSnapInputChainTests.Edge.cs`
- `XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.cs`
- `XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.Edge.cs`

## Verification evidence

- R2/R1 focused World tests: 69 passed, 0 failed, 0 skipped.
- World full suite: 1863 passed, 23 existing baseline failures, 0 skipped.
- Solution build: 0 warnings, 0 errors.
- `git diff --check`: passed for staged R2 changes.
- ARCH-A: blocked by pre-existing Diagnostic native workaround rule in `XuanYu.Editor.UI/Diagnostic/DiagnosticFloatingToolWindow.cs` (`SetWindowPos`); no R2 file touches that code.
- Official manual entry remains: `D:/MyDoc/project-vsCode/XuanyuEngine/run.bat`.

## Acceptance state

```text
R1: FREEZE APPROVED
R2 automation: PASS for the affected scope
R2 manual run.bat: WAITING FOR USER ACCEPTANCE
R2 FREEZE: NOT YET
```
