# AREA-D-R3 Inspector Pager + Adaptive LayerDock Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 将 Engine 右侧 Inspector 从单一长滚动属性区改为功能分页，并让 LayerDock 成为可收起/展开的独立 Pane。

**Architecture:** 在 XYUI Runtime 新增三个小型通用控件：`XYPager`、`XYInspectorSection`、`XYCollapsiblePane`。Editor 只负责把现有业务内容按功能域装入这些控件；业务字段、选择和命令继续由现有 `UiVm` 提供。Pager 页面内部最多保留一个纵向 ScrollViewer，LayerDock 的折叠状态由其通用 Pane 状态驱动。

**Tech Stack:** C# 10/.NET 10, Avalonia, XAML, xUnit Headless runtime tests.

**Spec:** User-provided AREA-D-R3 · Inspector Pager + Adaptive LayerDock handoff.

## Global Constraints

- 手写 `.cs` / `.axaml` 每个文件不超过 100 行。
- 不修改 `XYTabBar`、Gallery 滚轮修复、XYUI canonical 的既有滚动行为。
- 不新增 Inspector 搜索、Pin、多对象编辑、动画、Dock Engine 或数据模型。
- 现有 `EditorRightTabs` 的“检查器 / 层级 / 调试”一级导航保持不变。
- 所有测试与 dotnet 门禁串行执行；最终必须无临时日志、无未跟踪构建产物、无未提交改动。

---

### Task 1: Freeze contracts with failing runtime tests

**Files:**
- Create: `xyui/avalonia/tests/XYUI.Avalonia.Tests/InspectorWorkflowControlTests.cs`
- Create: `XuanYu.World.Tests/UiRuntime/AreaDR3InspectorPagerRuntimeTests.cs`

**Interfaces:**
- Tests describe `XYPager`, `XYInspectorSection`, `XYCollapsiblePane` public properties and transitions before implementation.
- Editor tests require existing top tabs and a pager host without changing selected scene state.

- [ ] Write tests for default first page, id selection, previous/next bounds and single `SelectionChanged` notification.
- [ ] Write tests for section collapse visibility and non-collapsible behavior.
- [ ] Write tests for pane collapsed/expanded state and persistent content instance.
- [ ] Write Editor runtime tests for top-level tabs, pager presence, page switching and LayerDock collapse/expand.
- [ ] Run the focused tests and verify they fail because the new controls/contracts do not exist.

### Task 2: Implement XYPager runtime

**Files:**
- Create: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-10-Pager/UI/XYPager.cs`
- Create: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-10-Pager/UI/XYPagerPage.cs`
- Create: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-10-Pager/UI/XYPager.Navigation.cs`
- Modify: XYUI control inventory/registration only if the repository requires an explicit entry.

**Interfaces:**
- `XYPagerPage(string id, string label, Control content)` exposes `Id`, `Label`, `Content`.
- `XYPager` exposes `IList<XYPagerPage> Pages`, `SelectedId`, `SelectedIndex`, `IsPreviousEnabled`, `IsNextEnabled`, `Select(string)`, `Previous()`, `Next()`, and `SelectionChanged`.

- [ ] Implement non-cyclic selection with first-page default and no timer/animation.
- [ ] Build a compact header with previous/next buttons, page labels and `current / total` indicator.
- [ ] Keep page content mounted and switch visibility instead of recreating controls.
- [ ] Run XYUI pager tests and confirm all pager cases pass.

### Task 3: Implement XYInspectorSection and XYCollapsiblePane

**Files:**
- Create: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-10-Pager/UI/XYInspectorSection.cs`
- Create: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI3/XYUI3-10-Pager/UI/XYCollapsiblePane.cs`
- Modify: XYUI styles/tokens only for compact header and existing color/border resources.

**Interfaces:**
- `XYInspectorSection` exposes `Header`, `Content`, `IsCollapsible`, `IsExpanded`.
- `XYCollapsiblePane` exposes `Header`, `Content`, `IsCollapsible`, `IsCollapsed`, `PaneState` and `Collapse/Expand` transitions.

- [ ] Implement section header toggle with content visibility and no parent/page side effects.
- [ ] Implement Pane header toggle while preserving the same content control instance.
- [ ] Use existing XYUI light compact visual resources; do not add large cards or arbitrary visual effects.
- [ ] Run focused component tests and keep them green.

### Task 4: Migrate Inspector content into functional pages

**Files:**
- Modify: `XuanYu.Editor.UI/Right/InspectorPanel.axaml`
- Modify: `XuanYu.Editor.UI/Right/InspectorPanel.axaml.cs`
- Modify: `XuanYu.Editor.UI/Right/MapEditorPanel.axaml`
- Modify: `XuanYu.Editor.UI/Right/RegionalAuthoringPanel.axaml`
- Add small Editor page/content files only when required to keep each hand-written file under 100 lines.

**Interfaces:**
- Existing `UiVm` selection, field and command bindings remain the data source.
- Inspector creates stable page ids such as `basic`, `transform`, `environment`, `display`, `data`, `advanced` according to the actual content available.

- [ ] Preserve existing entity header, layer selection, map editing, region/road/marker authoring and empty states.
- [ ] Group existing controls into functional pages rather than one-property pages.
- [ ] Add at most one vertical ScrollViewer per current page; Pager itself remains non-scrolling.
- [ ] Store the selected page id in the Editor control and restore it when the selected object supports that page.
- [ ] Run Editor headless tests for page switching and object selection preservation.

### Task 5: Integrate adaptive LayerDock

**Files:**
- Modify: `XuanYu.Editor.UI/Right/EditorLayerDock.axaml`
- Modify: `XuanYu.Editor.UI/Right/EditorLayerDock.axaml.cs`
- Modify: `XuanYu.Editor.UI/Right/Right.axaml` only if the existing splitter needs a named stable host.

**Interfaces:**
- `EditorLayerDock` uses `XYCollapsiblePane` and binds its existing `LayerPanel` content.
- The existing row splitter remains the height allocation boundary.

- [ ] Replace local boolean-only content hiding with real Pane state and header text `收起`/`展开`.
- [ ] Keep LayerDock visible and usable when expanded; collapse only the content area to header height.
- [ ] Verify collapse/expand does not change Inspector page or selected object.

### Task 6: Add visual/runtime regression coverage

**Files:**
- Modify: `xyui/avalonia/tests/XYUI.Avalonia.Tests/InspectorWorkflowControlTests.cs`
- Modify: `XuanYu.World.Tests/UiRuntime/AreaDR3InspectorPagerRuntimeTests.cs`
- Modify: `file-tree.md`

- [ ] Assert top-level `检查器 / 层级 / 调试` remains present.
- [ ] Assert pager page count/indicator, current-page visibility and one vertical scroll authority.
- [ ] Assert LayerDock collapse keeps header and expand restores the same content.
- [ ] Run focused tests, then the full test projects serially.

### Task 7: Formal gates, docs, commit and launch

**Files:**
- Modify: `changelog.md`, `run.bat`, `XuanYu.Editor.App/XuanYu.Editor.App.csproj`, `XuanYu.Editor.UI/Win/UiWin.axaml`, `XuanYu.Editor.UI/Vm/Scene/UiVm.SceneDocument.cs`.

- [ ] Bump the current version once and keep all version locations consistent.
- [ ] Run one complete solution build, all required test projects with `--no-build`, ARCH-A, line-count and `git diff --check` gates.
- [ ] Stage only AREA-D-R3 files, commit and push `feat/XYUI-ENGINE-AREA-A-CD`.
- [ ] Verify clean worktree and upstream `0/0`.
- [ ] Launch the latest `run.bat`/Engine output and stop at `READY FOR USER VISUAL + INTERACTION ACCEPTANCE`.

