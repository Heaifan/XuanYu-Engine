# XYUI-MENU-CASCADE-R1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 在 XYUI 中实现固定根菜单、多兄弟级联、Hover/Click 展开和递归生命周期，并验证 `FromModels()` 的真实交互能力。

**Architecture:** 保留 `XYMenuItem.SubMenu` 作为关系真源。重构 `XYSubMenu` 为 Trigger 到 ChildMenu 的级联控制器，不再把 ParentMenu 加入自身视觉树；根菜单由单一外部宿主固定承载，兄弟菜单通过父菜单注册表互斥展开。

**Tech Stack:** C#、Avalonia、XYUI.Avalonia、xUnit、Avalonia Headless、现有 Gallery 预览基础设施。

**Spec:** `docs/superpowers/specs/2026-09-16-xyui-menu-cascade-r1-design.md`

## Global Constraints

- 本轮只修改 XYUI Menu/SubMenu、XYUI 测试、Gallery 和对应 canonical/docs。
- 禁止修改 `XuanYu.Editor.UI`、Engine 菜单接入、`XYSplitButton` API、全局 Token 和键盘系统架构。
- ParentMenu 永远只渲染一次；根 Popup 的 Child 在打开生命周期内固定为 Root XYMenu。
- Hover 与 Click/Invoke 必须调用同一套 SubMenu 开关逻辑。
- 同一 ParentMenu 下最多打开一个直接子菜单，并递归关闭旧分支。
- 所有手写 `.cs` / `.axaml` / `.js` 单文件 ≤100 行。
- `dotnet build` / `dotnet test` 串行执行；不得把自动测试称为真机验收。

### Task 1: Lock the failing runtime contracts

**Files:**
- Create: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYMenuCascadeR1Tests.cs`
- Test existing: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYSubMenuHierarchyTests.cs`
- Test existing: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI3InteractionTests.cs`

**Interfaces:**
- Consumes: `XYMenu`, `XYMenuItem`, `XYSubMenu`, `XYMenuItemModel`, `XyuiHeadlessFixture`.
- Produces: executable failing contracts for fixed-root ownership, three sibling SubMenus, Hover/Click opening, sibling exclusivity, recursive close, leaf dispatch, and `FromModels()`.

- [ ] **Step 1: Write the failing tests**

  Add tests that construct one Root and three items. Each item must be wired with a real `XYSubMenu` and leaf `XYMenuItem`; assert all three `SubMenu.ParentMenu` references equal Root and each `Trigger` equals its item. Add a visual-tree assertion that Root has at most one visual parent after being attached to the test host.

  Add interaction tests using Headless pointer movement and `XYMenuItem.Activate()`:

  ```csharp
  line.RaiseEvent(PointerEnteredArgs(line));
  Assert.True(line.SubMenu!.IsOpen);
  surface.Activate();
  Assert.False(surface.SubMenu!.IsOpen);
  Assert.True(line.SubMenu.ChildMenu.IsOpen);
  ```

  Add `FromModels()` coverage for `点/线/面` and leaf labels `地图标记/道路/区域面`, and assert clicking `线` leaves `popupChild` reference unchanged in the host fixture.

- [ ] **Step 2: Run the tests and verify they fail**

  Run:

  ```powershell
  dotnet test xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI.Avalonia.Tests.csproj --no-restore --filter "FullyQualifiedName~XYMenuCascadeR1Tests|FullyQualifiedName~XYSubMenuHierarchyTests|FullyQualifiedName~XYUI3InteractionTests" --logger "console;verbosity=minimal"
  ```

  Expected: FAIL on Root visual ownership, three sibling submenu construction, or `FromModels()` interaction under the current ParentMenu-in-Grid implementation. Preserve the failure output as implementation evidence; do not alter Engine.

### Task 2: Separate SubMenu relation from ParentMenu visual ownership

**Files:**
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-04-SubMenu/XYSubMenu.cs`
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-04-SubMenu/Interaction/XYSubMenu.Interaction.cs`
- Modify if required: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-04-SubMenu/Styles/XYSubMenuConnector.cs`

**Interfaces:**
- Consumes: the existing `ParentMenu`, `ChildMenu`, `Trigger`, `ParentSubMenu`, `OpenLeft`, `Open`, `Close`, `ChildSubMenus` API.
- Produces: a SubMenu visual containing only the child overlay/connector surface while ParentMenu remains owned by the root host.

- [ ] **Step 1: Implement the smallest visual ownership change**

  Remove the code path that adds `_parent` to `_grid.Children`. Keep `_parent` as a relation source for trigger registration and close notifications. Keep `_child` as the only menu visual surface owned by the SubMenu. Ensure `Build()` never assigns a visual parent to `ParentMenu`.

  Preserve the public properties and existing `OpenLeft` behavior. Do not add a new Popup abstraction or change `XYSplitButton`.

- [ ] **Step 2: Run the focused structural tests**

  Run:

  ```powershell
  dotnet test xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI.Avalonia.Tests.csproj --no-restore --filter "FullyQualifiedName~XYMenuCascadeR1Tests|FullyQualifiedName~XYSubMenuHierarchyTests|FullyQualifiedName~XYUI3StructureTests" --logger "console;verbosity=minimal"
  ```

  Expected: ParentMenu visual-parent assertions pass; any remaining failures identify missing child overlay or trigger registration behavior.

### Task 3: Unify Hover and Click lifecycle and sibling exclusivity

**Files:**
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-02-Menu/Interaction/XYMenuItem.Interaction.cs`
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-02-Menu/XYMenu.cs`
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-04-SubMenu/Interaction/XYSubMenu.Interaction.cs`

**Interfaces:**
- Consumes: `XYMenuItem.Activate`, `OpenSubMenu`, `SubMenuRequested`, `XYMenu.SubMenus`, `XYSubMenu.ParentSubMenu`.
- Produces: one canonical open/close route for PointerEntered, Click/Invoke, sibling switching, ancestor close, and leaf dispatch.

- [ ] **Step 1: Add/adjust failing lifecycle tests for both triggers**

  Verify `PointerEntered` and `Activate()` both open the same attached SubMenu; verify a SubMenu item never executes a leaf Command path. Verify opening `surface` closes `line` and all descendants, while opening a child keeps its ancestors effective-visible.

- [ ] **Step 2: Implement one canonical SubMenu open route**

  Route `XYMenuItem.Activate()` and `XYMenu.OnItemPointerEntered()` through the attached `XYSubMenu.Open()` behavior. Keep `Command` execution only in the no-SubMenu branch. Use the existing parent menu registration list to close sibling branches before opening the requested branch.

- [ ] **Step 3: Implement recursive close invariants**

  Ensure `XYSubMenu.Close()` closes every `ChildSubMenu`, hides its ChildMenu, clears parent selection, and emits `Closed` once for an open branch. Ensure `XYMenu.Close()` reaches every registered direct submenu and their descendants.

- [ ] **Step 4: Run interaction tests**

  Run:

  ```powershell
  dotnet test xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI.Avalonia.Tests.csproj --no-restore --filter "FullyQualifiedName~XYMenuCascadeR1Tests|FullyQualifiedName~XYUI3InteractionTests|FullyQualifiedName~XYSubMenuHierarchyTests" --logger "console;verbosity=minimal"
  ```

  Expected: Hover, Click/Invoke, sibling switching, recursive close, and leaf execution tests pass.

### Task 4: Make `XYMenu.FromModels()` produce a usable multi-sibling tree

**Files:**
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-02-Menu/XYMenu.cs`
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-02-Menu/XYMenuItem.cs` only if the relation setter needs a minimal synchronization fix
- Test: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYMenuCascadeR1Tests.cs`

**Interfaces:**
- Consumes: `XYMenuItemModel.Children`, `XYMenuItem.SubMenu`, `XYSubMenu.Trigger`.
- Produces: a Root with three independent SubMenu relations, stable Trigger references, and usable leaf Commands without reparenting Root.

- [ ] **Step 1: Add the model-driven failing case**

  Build models for `点/线/面`, each with one child, call `XYMenu.FromModels(models)`, and assert the three returned root items have non-null `SubMenu` objects whose child menus contain the expected leaf.

- [ ] **Step 2: Fix construction order and ownership**

  Construct the Root first, create each item and child menu, assign the relation and Trigger after the item belongs to the Root item collection, and never insert a SubMenu's ParentMenu as a second visual child. Preserve all model fields (`Id`, `Icon`, `Shortcut`, enabled/check/destructive state).

- [ ] **Step 3: Verify model-driven interaction**

  Activate the model-generated `线` item, assert only its SubMenu opens, activate `道路`, assert exactly one leaf Command execution, and assert the branch closes.

### Task 5: Update Gallery and canonical documentation

**Files:**
- Modify: `xyui/avalonia/gallery/XYUI.Avalonia.Gallery/XYUI3LiveExamplesFactory.SubMenu.cs`
- Modify: `xyui/avalonia/gallery/XYUI.Avalonia.Gallery/XYSubMenuHierarchyDebugPreview.cs` only if required by the new visual ownership model
- Modify: `xyui/avalonia/gallery/XYUI.Avalonia.Gallery/XYUI3DocumentationCatalog.SubMenu.cs`
- Modify: the existing XYUI canonical/menu documentation file identified by `rg -l "XYUI-3-3.04|SubMenu" xyui docs`

**Interfaces:**
- Consumes: the final `XYSubMenu` ownership and lifecycle API from Tasks 2–4.
- Produces: Gallery examples that demonstrate fixed ParentMenu ownership, Hover/Click behavior, sibling switching, and multi-level close rules.

- [ ] **Step 1: Update the Gallery example**

  Keep the current official `ParentMenu + ChildMenu` example and add a three-sibling root example using the final API. The preview must place the Root visual once and expose three independent child relationships without creating an Engine-specific Popup workaround.

- [ ] **Step 2: Update the SubMenu documentation**

  State that `ParentMenu` is a relation/lifecycle source and is rendered once by its host; document `Trigger`, `ParentSubMenu`, Hover/Click opening, sibling exclusivity, and explicit `OpenLeft`.

- [ ] **Step 3: Run Gallery construction tests**

  Run:

  ```powershell
  dotnet test xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI.Avalonia.Tests.csproj --no-restore --filter "FullyQualifiedName~XYSubMenuHierarchyTests|FullyQualifiedName~XYUI3StructureTests" --logger "console;verbosity=minimal"
  ```

### Task 6: Full XYUI gates and delivery checkpoint

**Files:**
- No new production files; review all changed files from Tasks 1–5.

**Interfaces:**
- Consumes: passing structural, interaction, model-driven, and Gallery tests.
- Produces: a reviewable XYUI-only commit and evidence for later Engine re-entry.

- [ ] **Step 1: Run the XYUI build**

  ```powershell
  dotnet build xyui/avalonia/src/XYUI.Avalonia/XYUI.Avalonia.csproj --no-restore -p:UseSharedCompilation=false -maxcpucount:1
  ```

- [ ] **Step 2: Run the complete XYUI test project**

  ```powershell
  dotnet test xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI.Avalonia.Tests.csproj --no-restore --logger "console;verbosity=minimal"
  ```

- [ ] **Step 3: Run repository-local static gates**

  ```powershell
  powershell -ExecutionPolicy Bypass -File scripts/arch-a-guard.ps1
  git diff --check
  ```

  Also verify every changed hand-written `.cs`, `.axaml`, and `.js` file is at most 100 lines and confirm no Engine files changed.

- [ ] **Step 4: Review the diff against the frozen scope**

  ```powershell
  git diff --name-only
  git diff --stat
  rg -n "XuanYu\.Editor\.UI|DrawMenuPopup|OpenDrawingSubMenu" xyui/avalonia/src xyui/avalonia/tests xyui/avalonia/gallery
  ```

  Expected: only XYUI source/tests/Gallery/docs are changed; no Engine ContextToolbar workaround exists in the R1 diff.

- [ ] **Step 5: Commit only after all gates pass**

  ```powershell
  git add xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3 xyui/avalonia/tests/XYUI.Avalonia.Tests xyui/avalonia/gallery docs
  git commit -m "feat(xyui): add fixed-root cascading menus"
  ```

  Do not push, rebase, force-push, or modify Engine integration in this plan unless separately authorized by the project Git rules. After this checkpoint, stop and request the next Engine integration round.
