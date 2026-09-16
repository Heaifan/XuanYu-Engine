# CONTEXT-TOOLBAR-R2 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 将 Context Toolbar 的点/线/面绘制入口收敛为 Split Button，并将完成、取消和撤销节点统一显示为活动编辑事务条。

**Architecture:** 复用 XYUI 现有 `XYSplitButton`，由 `ContextToolBar` 持有菜单和菜单项；`UiVm` 增加会话级最近绘制工具、统一绘制入口协调和活动事务投影。Road/Region 的既有 Bootstrap、Draft、Commit、Cancel 链保持权威，Toolbar 不定义几何合法性。

**Tech Stack:** C# 10/.NET 10, Avalonia Headless, xUnit, XYUI.Avalonia controls.

**Spec:** 用户冻结任务书 `CONTEXT-TOOLBAR-R2 — 绘制 Split Button + 全局编辑事务条`（本轮对话附件）。

## Global Constraints

- 不修改 World Schema、Feature Schema、地图数据格式、Selection 大结构、Undo/Redo、Road/Region 几何提交或新增依赖。
- Active Transaction 时禁止隐式启动第二个绘制事务，不新增确认弹窗。
- 主按钮首次点击且没有 `LastDrawTool` 时只打开绘制菜单；选择 Road/Region 后主按钮直接重复最近工具。
- 完成/取消事务条无活动事务时 `IsVisible = false`；完成继续调用既有提交链，取消继续调用既有取消链。
- 所有新增手写 `.cs`/`.axaml` 文件不超过 100 行；保留当前仓库 5+100 规则。
- 自动测试不替代 L4 真机视觉和输入验收；最终报告保持 `UI REAL ACCEPTANCE: PENDING`。

### Task 1: Establish failing regression contract

**Files:**
- Create: `XuanYu.World.Tests/UiRuntime/ContextToolbarR2RuntimeTests.cs`
- Modify: `XuanYu.World.Tests/UiTokens/AreaCR1ContextToolbarContractTests.cs`

- [x] Add runtime tests for no persistent transaction controls, direct Draw→Road/Area startup, active transaction visibility, completion/cancellation state, last-tool repeat, and second-transaction blocking.
- [x] Add source contract assertions for `XYSplitButton`, menu hierarchy, unified `完成`, and absence of `完成道路`/`完成闭合`/persistent start buttons.
- [x] Run the focused tests and record the expected RED failures before any production edit.

### Task 2: Add minimal ViewModel transaction and last-tool coordination

**Files:**
- Create: `XuanYu.Editor.UI/Vm/Workspace/UiVm.ContextToolbarDrawing.cs`
- Modify: `XuanYu.Editor.UI/Vm/Workspace/UiVm.RegionAuthoring.cs`
- Modify: `XuanYu.Editor.UI/Vm/Map/UiVm.MapDataset.RoadBootstrap.cs`
- Modify: `XuanYu.Editor.UI/Vm/Map/UiVm.MapDataset.DrawingBootstrap.cs`
- Modify: relevant drawing binding raisers in `UiVm.RoadDrawing.History.cs` and `UiVm.RegionDrawing.DraftHistory.cs`

- [x] Add a session-only `LastDrawTool` value and display state without persistence or schema changes.
- [x] Add commands/methods that map `道路` and `区域面` directly to the existing Bootstrap methods and reject requests while either draft is active.
- [x] Make completion and cancellation refresh the unified transaction projection while preserving the existing commit/cancel calls.
- [x] Ensure authoring-mode changes cannot silently discard an active transaction.
- [x] Run the focused runtime tests and fix only failures caused by this contract.

### Task 3: Replace Toolbar markup with Split Button and transaction bar

**Files:**
- Modify: `XuanYu.Editor.UI/Top/ContextToolBar.axaml`
- Modify: `XuanYu.Editor.UI/Top/ContextToolBar.axaml.cs`

- [x] Replace persistent 点/线/面 menu bars and start buttons with `XYSplitButton` and a host-owned two-level `XYMenu` Popup hierarchy.
- [x] Bind the main zone to the session last-tool command and the menu zone to the popup-opening command; no fallback tool is selected before the first menu choice.
- [x] Render one activity section with `道路绘制中` or `区域绘制中`, count, `撤销节点`, `完成`, and `取消`, using existing CanExecute properties.
- [x] Keep marker capability available under the point submenu without making it the default drawing tool.
- [x] Run static contract and Avalonia Headless runtime tests.

### Task 4: Verification and delivery evidence

- [x] Run focused `XuanYu.World.Tests` regression set and affected `XuanYu.Editor.UI` build.
- [x] Run ARCH-A, 5+100, and `git diff --check`.
- [ ] Record every failure by full test name and provide parent-commit baseline evidence before calling any failure pre-existing; current report labels the 16 full-suite failures as baseline-unconfirmed and does not call them pre-existing.
- [x] Review diff scope and exact per-commit file +/- statistics.
- [ ] Commit atomically, push to the current remote branch, and verify local/remote HEAD equality.
- [ ] Report UI as pending user real-machine acceptance with the Chinese IPO script from the task book.
