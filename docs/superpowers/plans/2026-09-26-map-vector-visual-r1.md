# MAP-VECTOR-VISUAL-R1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax.

**Goal:** Add Vulkan Region labels and shared analytic anti-aliased Region/Road strokes without changing map Schema, Native HWND ownership, or the existing selection/Undo contracts.

**Architecture:** Keep `MapRegion.DisplayName` and `MapRoad.DisplayName` as the only label facts. Editor projection produces cached text image resources and screen-space label instances; Vulkan consumes renderer-neutral text resources and draws them through a dedicated pass. Region and Road strokes share one dedicated screen-space analytic pipeline using capsule SDF coverage, `fwidth`, `smoothstep`, round caps, and round joins; the existing `scene.vert` path is not extended.

**Tech Stack:** C#/.NET 10, Avalonia text rasterization in `Editor.UI`, Vulkan/Silk.NET, GLSL 450, xUnit, existing `run.bat` runtime gate.

**Spec:** GitHub Issue #4 — `MAP-VECTOR-VISUAL-R1`.

## Global Constraints

- `MapRegion.DisplayName` and `MapRoad.DisplayName` remain the only label facts.
- No `.xymap`/Region Dataset Schema changes.
- No Native HWND or Avalonia overlay added over the Vulkan child HWND.
- Region selection border and selected vertices remain independent from label/stroke color.
- Region and Road use one shared stroke renderer and one stroke resource contract.
- No full-scene MSAA, Vulkan wideLines, CPU readback, or per-frame texture recreation.
- Text rasterization is cached by content/font/style; camera movement only changes label placement.
- All hand-written `.cs`/`.axaml`/`.js` files remain at or below 100 physical lines.
- Pointer/hover/render paths must not add predictable O(N) full-map scans or heavy side effects.

## Review Focus

- Concave polygon labels stay inside the polygon and update after rename/geometry changes.
- CJK, ASCII, and numeric labels use the same text resource path; no ScaleIndicator glyph fallback.
- Stroke width remains screen-space stable across zoom and uses analytic coverage rather than MSAA.
- Round cap/join geometry does not double-darken corners or create gaps at closed Region seams.
- Cached label content does not rebuild its texture on camera-only movement.

### Task 1: Baseline and visual geometry contracts

**Files:**
- Create: `XuanYu.Editor/MapEditing/PolygonVisualCenter.cs`
- Create: `XuanYu.World.Tests/MapEditing/PolygonVisualCenterTests.cs`
- Create: `XuanYu.World.Tests/UiRuntime/MapVectorVisualContractTests.cs`
- Modify: `docs/superpowers/plans/2026-09-26-map-vector-visual-r1.md`

**Interfaces:**
- Produces `PolygonVisualCenter.TryFind(IReadOnlyList<MapPoint>, out MapPoint)` with an inside-polygon result for valid concave Regions.
- Produces contract tests for Region/Road shared stroke ownership and label source identity.

- [ ] Write failing tests for convex, concave, boundary, invalid, and narrow polygons.
- [ ] Run the focused tests and confirm the failure is the missing visual-center contract.
- [ ] Implement the bounded visual-center solver with deterministic fallback and no schema changes.
- [ ] Run the focused tests and `git diff --check`.
- [ ] Keep the existing uncommitted `InspectorRegionColorPreview.cs` fix in the branch.

### Task 2: Dedicated analytic Vector Stroke resource and pipeline

**Files:**
- Create: `XuanYu.Render.Abstractions/RenderVectorStrokeVertex.cs`
- Create: `XuanYu.Render.Abstractions/RenderVectorStrokeResource.cs`
- Create: `XuanYu.Render.Vulkan/Shaders/editor_vector_overlay.vert`
- Create: `XuanYu.Render.Vulkan/Shaders/editor_vector_overlay.frag`
- Create: `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.EditorVectorOverlay*.cs`
- Create: `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.VectorOverlay.cs`
- Create: `XuanYu.Render.Vulkan/Render/VectorOverlay/VulkanVectorStrokeCache.cs`
- Modify: `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.PipelineBind.cs`
- Modify: `XuanYu.Render.Vulkan/Render/VectorOverlay/VulkanClearFrameOwner.DrawVectorOverlay.cs`
- Test: `XuanYu.World.Tests/UiRuntime/MapVectorOverlayV1Tests.cs` and new stroke contract tests.

**Interfaces:**
- Consumes screen-space stroke instances with start/end, half-width, cap/join flags, color, and viewport dimensions.
- Produces one shared Region/Road stroke renderer; each segment remains O(1) geometry and the shader computes capsule coverage with `fwidth`/`smoothstep`.

- [ ] Add failing resource tests for screen-space width, closed Region seams, round cap, round join, and shared Region/Road primitive kind.
- [ ] Run the focused tests and confirm the new resource/pipeline contract is absent.
- [ ] Implement the renderer-neutral stroke resource and validator.
- [ ] Add dedicated vertex/fragment shader files; do not extend `scene.vert`.
- [ ] Compile/load the dedicated shader bytecode and bind a `Count1Bit` analytic pipeline without enabling scene-wide MSAA.
- [ ] Route Region and Road strokes through the shared stroke cache while retaining existing Fill/Marker behavior.
- [ ] Run focused tests, affected build, architecture guard, 5+100, and `git diff --check`.

### Task 3: Text Raster/Cache and Vulkan label resource

**Files:**
- Create: `XuanYu.Render.Abstractions/RenderTextOverlayResource.cs`
- Create: `XuanYu.Render.Abstractions/RenderTextOverlayInstance.cs`
- Create: `XuanYu.Editor.UI/Vm/Map/MapLabelTextRasterizer.cs`
- Create: `XuanYu.Editor.UI/Vm/Map/MapLabelTextCache.cs`
- Create: `XuanYu.Render.Vulkan/Render/TextOverlay/VulkanTextOverlayCache.cs`
- Create: `XuanYu.Render.Vulkan/Render/TextOverlay/VulkanClearFrameOwner.DrawTextOverlay.cs`
- Create: dedicated text vertex/fragment shader and pipeline files under `XuanYu.Render.Vulkan`.
- Test: CJK/ASCII/numeric raster-cache and no-rebuild-on-camera-change tests.

**Interfaces:**
- `MapLabelTextCache.GetOrCreate(string text, MapLabelTextStyle style)` returns a stable content-keyed renderer-neutral RGBA resource.
- Label instances carry only projected screen-space anchor/quad data; camera movement must not invalidate the content texture.

- [ ] Add failing tests for `区域1`, `台湾北部`, `第一战区`, ASCII, numeric text, cache hit, and content/style invalidation.
- [ ] Confirm the failure is the absence of a CJK-capable raster/cache path, not a renderer test typo.
- [ ] Implement rasterization in `Editor.UI` using the existing Avalonia font/fallback contract; do not reference Avalonia from Render projects.
- [ ] Upload/cache texture resources in Vulkan with explicit disposal and key/revision reuse.
- [ ] Add the dedicated screen-space quad pipeline and validate alpha blending.
- [ ] Run focused text tests and affected Editor/UI/Vulkan builds.

### Task 4: Region/Road projection and label placement integration

**Files:**
- Modify: `XuanYu.Editor.UI/Vm/Map/MapVectorOverlayBuilder.cs` and split helpers as required by 5+100.
- Modify: `XuanYu.Editor.UI/Vm/Map/MapRegionRenderProjection.cs`.
- Create: `XuanYu.Editor.UI/Vm/Map/MapRegionLabelProjection.cs`.
- Create: `XuanYu.Editor.UI/Vm/Map/MapRoadLabelProjection.cs`.
- Modify: `XuanYu.Editor.UI/Vm/Scene/SceneRenderProjectionAdapter.cs` and renderer-neutral projection types.
- Test: Region rename, Region geometry change, concave label containment, Road label and visibility tests.

**Interfaces:**
- Region label text comes only from `MapRegion.DisplayName`; Road label text comes only from `MapRoad.DisplayName`.
- Label anchor uses `PolygonVisualCenter`; geometry or rename changes invalidate the instance revision, while camera movement only recomputes placement.

- [ ] Add failing integration tests for rename, geometry replacement, hidden layers, CJK text, and concave containment.
- [ ] Implement Region and Road label projection through the shared text resource/cache.
- [ ] Replace current Region/Road stroke construction with the shared analytic stroke resource.
- [ ] Assert selected yellow border/vertices remain on their existing selection path.
- [ ] Run focused integration tests, affected build, architecture checks, 5+100, and `git diff --check`.

### Task 5: Runtime verification and closeout

**Files:**
- Create: `docs/superpowers/audits/2026-09-26-map-vector-visual-r1-run-bat-acceptance.md`
- Modify: task documentation only for evidence and residual risk.

- [ ] Run serial full Solution Build with the configured SDK and record warnings/errors.
- [ ] Run the applicable Region/Road/vector/text test suites serially.
- [ ] Run architecture guards, 5+100, and `git diff --check`.
- [ ] Launch with `run.bat` and verify CJK/ASCII/numeric labels, concave placement, rename/geometry refresh, smooth Region/Road strokes, zoom-stable width, and no selection-color regression.
- [ ] Record Chinese IPO evidence for each runtime path.
- [ ] Mark only code/automated gates supported by evidence; keep real-device acceptance open until observed.
- [ ] Commit atomically, push the feature branch, and verify local/remote equality.

