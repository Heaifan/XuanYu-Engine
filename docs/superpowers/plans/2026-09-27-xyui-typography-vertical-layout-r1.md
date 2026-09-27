# XYUI Typography Vertical Layout Contract R1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 让 XYUI 文本原子组件共享单行固定高度下的视觉垂直排版契约，同时保持多行说明、显式 Top 和输入控件行为不变。

**Architecture:** 在 `XyuiTextComponent` 中提供共享的单行文字几何偏移核心；`XYText`、`XYLabel`、`XYCaption`、`XYHeading` 通过同一基类行为获得一致结果。Typography Tokens 继续只负责字体属性；XYE Toolbar 的图标文字迁移到 `XYLabel`。

**Tech Stack:** Avalonia 12.1.3, C#, xUnit, XYUI Gallery, XuanYu Editor UI.

**Spec:** User-provided “XYUI Typography Vertical Layout Contract R1” specification in the current conversation.

## Global Constraints

- 不修改 `XyuiTypographyTokens.cs` 的字体、字号、字重、行高、颜色职责。
- 不对所有原生 `TextBlock` 设置全局 Center。
- 多行文本、显式 `VerticalAlignment=Top`、TextBox/SelectableTextBlock 保持原行为。
- 不修改用户既有非 XYUI 未提交内容；不提交、不推送 GitHub。

## Review Focus

- 单行中文/英文/中英混排在固定高度容器内视觉居中。
- 多行文本继续使用 Top，不被几何偏移破坏。
- XYText 现有行为不回归。
- XYE Toolbar 图标与文字中心对齐。
- XYUI4-01 Gallery 与组件测试覆盖真实控件树。

---

### Task 1: Shared text layout core

**Files:**
- Modify: `xyui/avalonia/src/XYUI.Avalonia/Controls/XYUI1/_Shared/Base/XyuiTextComponent.cs`
- Test: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI1TextVerticalLayoutContractTests.cs`

- [ ] Write failing tests for XYCaption, XYLabel, XYText, XYHeading single-line offsets and multi-line no-offset behavior.
- [ ] Run the focused tests and verify failure is caused by missing shared layout behavior.
- [ ] Extract the existing XYText geometry calculation into the shared primitive base and apply only to single-line, non-wrapping text with extra height.
- [ ] Run the focused matrix and verify all assertions pass.

### Task 2: Gallery and component regression

**Files:**
- Modify: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI4HoverStateTests.cs`
- Test: `xyui/avalonia/tests/XYUI.Avalonia.Tests/XYUI4TypographyGalleryContractTests.cs`

- [ ] Add real Gallery assertions for XYUI4-01 caption placement and Source Han Sans SC mixed text.
- [ ] Verify Core Rules and How To Use explicit Top layout remains unchanged.
- [ ] Run the focused Gallery tests.

### Task 3: XYE Toolbar primitive migration

**Files:**
- Modify: `XuanYu.Editor.UI/Top/EditToolsModule.axaml`
- Modify: `XuanYu.Editor.UI/Top/FileModule.axaml`
- Modify: `XuanYu.Editor.UI/Top/ViewModule.axaml`
- Modify: `XuanYu.Editor.UI/Top/SnapModule.axaml`
- Test: `XuanYu.World.Tests` or existing Editor UI contract test location.

- [ ] Replace only single-line Toolbar labels with `XYLabel`; leave long messages and diagnostic text as native TextBlock.
- [ ] Add/extend structure tests proving Toolbar labels use XYUI text primitives and no input/multiline path changed.
- [ ] Run affected Editor UI tests and build.

### Task 4: Final verification

- [ ] Run XYUI focused tests.
- [ ] Run affected XYE build/tests.
- [ ] Run `git diff --check` and 5+100/scope checks.
- [ ] Visually inspect Gallery and XYE Top Toolbar; report automated vs human acceptance separately.
