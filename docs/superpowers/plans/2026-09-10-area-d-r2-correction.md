# AREA-D-R2-CORRECTION Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task with verification checkpoints.

**Goal:** 将 Area D 的 MapEditorPanel 放入 Inspector 内容槽，并让地图编辑模式下的 EditorLayerDock 持续可见，同时保持 Left/Right 使用 canonical XYTabs。

**Architecture:** `InspectorPanel` 是 Inspector 一级 XYTab 的动态内容宿主，EntityInspectorPanel 与 MapEditorPanel 在其中按 `IsEntityInspector` 互斥；`EditorLayerDock` 保留在 Right 编辑模式的独立底部区域，不受 Entity 选择影响。现有 Map VM、草稿/校验/Apply 链和区域工作区不重构。

**Tech Stack:** C# 12、Avalonia、AXAML、XYUI.Avalonia、xUnit、PowerShell、dotnet CLI。

**Spec:** `C:\Users\Heai\.codex\attachments\ac99dc77-b486-4638-92fb-c97660a1fd3b\pasted-text.txt`

## Global Constraints

- `CODEX = SOLE DEVELOPMENT AGENT`。
- `XuanYu.Editor.UI/Win/UnsavedChangesConfirmationWindow.axaml` 保持 dirty，`DO NOT TOUCH / DO NOT STAGE / DO NOT COMMIT`。
- Map Width/Depth/BaseHeight 继续使用 `XYTextField`；不迁移到 `XYNumberField`，不改 Map 草稿/校验/Apply 业务链。
- Left/Right 一级导航继续使用真实 `XYTabs/XYTab`；不引入 Universal Selection、Focus Manager 或 Navigation Framework。
- `EditorLayerDock` 在 Map Edit + Entity selected 时必须可见；Move/Rotate/Scale 不改变 Inspector/LayerDock 归属。
- 每个手写 `.cs` / `.axaml` 不超过 100 行；dotnet 命令严格串行；不增加 Skip/Disabled。

### Task 1: Freeze current ownership and write red tests

**Files:**
- Modify: `XuanYu.World.Tests/UiRuntime/AreaDR1Fix5RightContentOwnershipTests.cs`
- Modify: `XuanYu.World.Tests/UiRuntime/AreaBLeftWorkspaceRuntimeTests.R2.cs`
- Modify: `XuanYu.World.Tests/UiRuntime/LayerARuntimeTests.cs` if the current assertions do not express persistent Map Edit docking
- Create or modify: `XuanYu.World.Tests/UiRuntime/AreaDR2NavigationAndMapContextRuntimeTests.cs`

**Interfaces:**
- Consume current `Right`, `EditorRightTabs`, `InspectorPanel`, `MapEditorPanel`, `EditorLayerDock`, `UiVm.IsEntityInspector`, `UiVm.IsMapEditMode`, and `UiVm.SelectToolCommand`.
- Produce executable assertions for one visible MapEditorPanel or EntityInspectorPanel in Inspector content and one visible EditorLayerDock in Map Edit.

- [x] **Step 1: Write failing routing assertions.**
  - Map Edit + no Entity: visible MapEditorPanel = 1, visible EntityInspectorPanel = 0, visible EditorLayerDock = 1.
  - Map Edit + Entity: visible MapEditorPanel = 0, visible EntityInspectorPanel = 1, visible EditorLayerDock = 1.
  - For `选择`, `移动`, `旋转`, `缩放`, EntityInspectorPanel remains 1 and EditorLayerDock remains 1.
  - Assert `MapEditorPanel` is named and hosted by `InspectorPanel`, and `Right.axaml` no longer declares a MapEditorPanel sibling.
- [x] **Step 2: Run the targeted Area D tests before implementation.**
  - Run `dotnet test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-restore --filter "FullyQualifiedName~AreaDR1Fix5RightContentOwnershipTests|FullyQualifiedName~AreaDR2NavigationAndMapContextRuntimeTests"`.
  - Expected: fail because current Right-level MapEditorPanel is absent from the Inspector host and LayerDock is hidden by `!IsEntityInspector`.

### Task 2: Correct Inspector and LayerDock hosting

**Files:**
- Modify: `XuanYu.Editor.UI/Right/InspectorPanel.axaml`
- Modify: `XuanYu.Editor.UI/Right/Right.axaml`
- Test: `XuanYu.World.Tests/UiRuntime/AreaDR1Fix5RightContentOwnershipTests.cs`

**Interfaces:**
- `InspectorPanel` owns one named `MapEditorPanel` with visibility `IsMapEditMode && !IsEntityInspector` expressed through nested visibility hosts.
- `Right` owns exactly one `EditorLayerDock` in Edit mode and no MapEditorPanel sibling; its visibility is not gated by `!IsEntityInspector`.

- [x] **Step 1: Add the MapEditorPanel to InspectorPanel.**
  - Place `<local:MapEditorPanel x:Name="MapWorkspace" .../>` in the Inspector content stack.
  - Use an `IsMapEditMode` wrapper and `IsVisible="{Binding !IsEntityInspector}"` so Map and Entity content are mutually exclusive.
  - Guard the existing empty state from showing during Map Edit.
- [x] **Step 2: Remove the Right-level MapEditorPanel and uncouple LayerDock.**
  - Remove `MapWorkspace` from the lower Right grid.
  - Keep `RegionalAuthoringPanel` in its existing Edit-mode area.
  - Keep `LayerWorkspace` as the single dock instance and remove the parent `IsVisible="{Binding !IsEntityInspector}"` gate.
- [x] **Step 3: Run the routing tests.**
  - Run the Task 1 filter again.
  - Expected: all routing tests pass with Map/Entity Inspector mutual exclusion and persistent LayerDock.

### Task 3: Reconcile contracts and preserve navigation semantics

**Files:**
- Modify: impacted World UI composition/runtime contract tests identified by the targeted run
- Preserve: `XuanYu.Editor.UI/Left/Left.axaml`, `EditorRightTabs.axaml`, and canonical XYTabs keyboard implementation from the preceding R2 commit

**Interfaces:**
- Existing tests must assert the confirmed user contract, not the superseded FIX5 contract that hid LayerDock with Entity selection.

- [x] **Step 1: Update only stale assertions.**
  - Rename test descriptions/comments to state `LayerDock persistent in Map Edit`.
  - Keep tests for Left project/file switching, Right inspector/hierarchy/debug switching, selection preservation, XYTabs keyboard behavior, Map Asset uniqueness, and `XYTextField` bindings.
- [x] **Step 2: Run the affected World tests.**
  - Run `dotnet test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-restore --filter "FullyQualifiedName~AreaDR1Fix5RightContentOwnershipTests|FullyQualifiedName~AreaBLeftWorkspaceRuntimeTests|FullyQualifiedName~LayerARuntimeTests|FullyQualifiedName~AreaDR2NavigationAndMapContextRuntimeTests|FullyQualifiedName~UiMapLayoutContractTests|FullyQualifiedName~EditorWorkspaceUiCompositionTests"`.
  - Expected: pass, with no skipped tests.

### Task 4: Documentation, version, and closeout gates

**Files:**
- Modify: `changelog.md`, `file-tree.md`, `run.bat`, `XuanYu.Editor.App/XuanYu.Editor.App.csproj`, `XuanYu.Editor.UI/Win/UiWin.axaml`, `XuanYu.Editor.UI/Vm/Scene/UiVm.SceneDocument.cs`
- Preserve uncommitted: `XuanYu.Editor.UI/Win/UnsavedChangesConfirmationWindow.axaml`

**Interfaces:**
- Version strings remain identical in the four required locations.
- Changelog records the corrected Inspector/LayerDock contract and separates the known deferred dialog failure.

- [x] **Step 1: Add the top changelog entry and rebuild file-tree from `git ls-files`.**
- [x] **Step 2: Run serial formal gates.**
  - `dotnet build-server shutdown`
  - `dotnet build XuanYu.Engine.slnx --no-restore -m:1 -nr:false -p:BuildInParallel=false -p:UseSharedCompilation=false`
  - Core, WarCore, World, XYUI tests with `--no-build --no-restore`
  - `scripts/arch-a-guard.ps1`
  - XML/version/5+100/`git diff --check`
  - final `dotnet build-server shutdown`
- [ ] **Step 3: Commit only the correction files, push the current branch, verify `HEAD == origin` and `0/0`, then run the existing `run.bat` smoke path if no process lock exists.**
