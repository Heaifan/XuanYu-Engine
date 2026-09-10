# Area D Region Inspector Layout Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Keep all Region edit inspector content in one scroll host and make LayerDock an independent resizable lower area.

**Architecture:** `InspectorPanel` remains the owner of one vertical `ScrollViewer`; `RegionalAuthoringPanel` becomes a child of its Region section. `Right` uses a single upper Inspector area, a splitter, and a lower LayerDock, so resizing the dock only changes available viewport height.

**Tech Stack:** Avalonia XAML/C#, existing UiVm RegionAuthoring and layer bindings, headless/source contract tests, PowerShell gates.

**Spec:** `docs/superpowers/specs/2026-09-10-area-d-xyui-canonical-layout-design.md`

## Global Constraints

- Do not change Region Dataset, drawing, Snap, Vertex, Undo/Redo, Selection, persistence, Renderer, or domain schema code.
- Do not modify or stage `XuanYu.Editor.UI/Win/UnsavedChangesConfirmationWindow.axaml`.
- Preserve `RegionalAuthoringPanel` selection routing and every existing Region/Road/Marker command binding.
- Preserve LayerDock collapse and internal layer-list scrolling.
- Keep every hand-written `.cs` and `.axaml` at or below 100 physical lines.

### Task 1: Add failing Region composition and allocation contracts

**Files:**
- Create: `XuanYu.World.Tests/UiRuntime/AreaDR2Fix4RegionInspectorLayoutTests.cs`
- Modify: `XuanYu.Editor.UI/Right/InspectorPanel.axaml`
- Modify: `XuanYu.Editor.UI/Right/Right.axaml`

**Interfaces:**
- Tests identify the single Inspector `ScrollViewer`, the Region authoring control, the splitter, and LayerDock from the live visual tree or source contract.
- Tests assert the order `区域属性 → 内容类型 → drawing/editor content`, one scroll owner, independent LayerDock region, and preserved visibility bindings.

- [ ] **Step 1: Write the failing source/runtime contracts**

  Assert the current tree fails because `RegionalAuthoringPanel` is outside `InspectorPanel` and `Right` contains a separate authoring row. Assert the target tree has one Region authoring descendant under Inspector’s ScrollViewer and no duplicate bottom authoring host.

- [ ] **Step 2: Add a layout allocation test fixture**

  Use the existing `UiHeadlessFixture` pattern from `XuanYu.World.Tests/UiRuntime/AreaDR2Fix3ProjectionDensityRuntimeTests.cs` to show the right-side composition at a bounded height, change the splitter allocation, and assert the Inspector content host remains the same ScrollViewer while LayerDock bounds change. Assert `LayerPanel` remains the owner of its internal `ListBox` scrollable region.

### Task 2: Move Region authoring into the Inspector scroll page

**Files:**
- Modify: `XuanYu.Editor.UI/Right/InspectorPanel.axaml`
- Modify: `XuanYu.Editor.UI/Right/Right.axaml`
- Modify: `XuanYu.Editor.UI/Right/RegionalAuthoringPanel.axaml`
- Do not modify: `XuanYu.Editor.UI/Right/RegionalAuthoringPanel.axaml.cs`; its DataContext/event routing remains unchanged

**Interfaces:**
- Inspector keeps one `ScrollViewer` and gains one Region composition branch containing region properties/empty state and `RegionalAuthoringPanel`.
- Right no longer renders `RegionalAuthoringPanel` below Inspector; it retains only splitter and `EditorLayerDock` in the lower area.

- [ ] **Step 1: Compose Region properties and authoring in Inspector order**

  Place the Region property/empty-state section and `RegionalAuthoringPanel` under the existing Inspector StackPanel, guarded by `IsRegionEditMode`. Keep content type tabs and Region/Road/Marker panel bindings unchanged.

- [ ] **Step 2: Remove the displaced bottom authoring row**

  Delete only the `Right.axaml` authoring host and adjust row definitions so the lower area contains the splitter and LayerDock. Keep the existing `IsEditMode`/`IsRegionEditMode` visibility boundaries and no business VM changes.

- [ ] **Step 3: Run source/runtime layout tests**

  Verify Region empty state still shows content type and drawing guidance on the same scroll page, and verify selected layer/property bindings remain available without changing their ViewModel owner.

### Task 3: Increase and bound LayerDock space

**Files:**
- Modify: `XuanYu.Editor.UI/Right/EditorLayerDock.axaml`
- Modify: `XuanYu.Editor.UI/Right/Right.axaml`
- Inspect and modify only the existing `ListBox` row in `XuanYu.Editor.UI/Right/LayerPanel.axaml` when the allocation test demonstrates that its current star row does not scroll within the bounded LayerDock

**Interfaces:**
- Expanded LayerDock has a useful default/minimum height that shows several rows; collapsed LayerDock reduces to its header; internal layer list consumes remaining dock height with its own scroll behavior.

- [ ] **Step 1: Set a practical expanded minimum and independent split rows**

  Increase the dock root minimum from the current 192 DIP to 260 DIP, which fits the header, toolbar, hint, and at least three 32 DIP layer rows. Use the lower row/splitter arrangement so the Inspector and LayerDock are separate resizable siblings.

- [ ] **Step 2: Preserve collapse and internal scrolling**

  Keep `EditorLayerDock.axaml.cs` collapse behavior and ensure the collapsed root can shrink to header height. Keep `LayerPanel`’s `ListBox` in its star row; do not move its visibility/lock bindings into another owner.

- [ ] **Step 3: Run layout and interaction contracts**

  Verify dock expansion, collapse, splitter allocation, layer visibility/lock two-way bindings, selection semantics, and Region drawing command reachability.

### Task 4: Run Area D regression and document the real result

**Files:**
- Modify: `changelog.md`
- Modify: `file-tree.md`

- [ ] **Step 1: Run focused Area D tests**

  Run `XuanYu.World.Tests`, `XuanYu.Core.Tests`, `XuanYu.WarCore.Tests`, and `xyui/avalonia/tests/XYUI.Avalonia.Tests` using the existing solution project paths, with Region authoring, LayerDock, selection, apply/undo/redo, and drawing filters first.

- [ ] **Step 2: Run the formal serial gates**

  Run `dotnet build-server shutdown`, one serial solution build, each test project serially with `--no-build --no-restore`, `scripts/arch-a-guard.ps1`, XML/version checks, and `git diff --check`.

- [ ] **Step 3: Synchronize truthful documentation**

  Add the real timestamp, changed paths, verification results, commit hash, and remaining user visual/interaction acceptance to the top of `changelog.md`; rebuild `file-tree.md` from `git ls-files` without staging the deferred file.
