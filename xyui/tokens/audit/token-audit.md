# XYUI-A3-R1 · Token Namespace Audit Report

> A3 Design Token System 第一轮：**Token Namespace Audit & Collision Classification**。
> 本轮只审计、分类、登记；**不重命名、不合并、不选胜者、不生成正式 Token Source、不导出 Avalonia**。
>
> 状态：`READY FOR USER ACCEPTANCE`（STOP，等人工验收后进入 A3-R2）。

## 1. 扫描范围与结果

```text
扫描来源            foundation-registry.json（44 项 Canonical Registry）
Token Occurrences   426 条（全量 TOKEN engineering mapping）
Distinct Token IDs  417 个
同 ID 多 Occurrence 8 组
```

## 2. Collision 分类（5 类，本轮实测）

```text
IDENTICAL_REDECLARATION       3 组
NAMESPACE_TYPE_COLLISION      5 组
VALUE_COLLISION               0 组
SEMANTIC_OVERLAP              0 组（本轮同 ID 聚合无此型；跨 ID 语义重叠以 ALIAS_CANDIDATE 呈现）
ALIAS_CANDIDATE              35 组
```

### 2.1 NAMESPACE_TYPE_COLLISION（5 组 —— 最危险）

同 ID 承载不同 value type / 工程角色。**A3-R2 必须解决**：

```text
XY.Border.Strong     BorderDivider=颜色 #95A7B3/#687B88  ↔  Border=宽度样式 2 DIP/Solid
XY.Border.Focus      BorderDivider=颜色 #5C8FB4/#699CC0  ↔  Border=宽度样式 2 DIP/Solid
XY.Border.Selected   BorderDivider=颜色 #3E78A4/#80B1D5  ↔  Border=宽度样式 2 DIP/Solid
XY.State.Selected    InteractionStateColor=颜色 #D8E7F2/#35536A ↔ InteractionState=状态语义 PersistentBaseState
XY.State.Dragging    InteractionStateColor=颜色 #E5E9EC/#303B43 ↔ InteractionState=状态语义 Independent
```

这与你人工复核的结论一致：`XY.Border.*` 同时是「边框颜色 Token」与「边框宽度/样式 Token」；`XY.State.*` 同时是「状态颜色」与「状态行为语义」。

### 2.2 IDENTICAL_REDECLARATION（3 组 —— 应收敛，但本轮不动）

同 ID 同类型同值，跨章节重复声明：

```text
XY.Text.Primary      CorePalette 与 TextColor 同值 #2B3A44/#DEE6EA
XY.Text.Secondary    CorePalette 与 TextColor 同值 #647681/#B3BFC6
XY.Text.Link         TextColor 与 Accent 同值 #4A789E/#82A9C5
```

A3-R2 应收敛为单 Canonical Token（一处 canonical + 引用），**不得保留成多个未来真值源**。

### 2.3 VALUE_COLLISION（0 组）

同 ID 同类型异值的情况**不存在**——XYUI0 内部一致性良好。

### 2.4 ALIAS_CANDIDATE（35 组 —— 只登记候选，不建 Alias）

不同 ID 共享同值。分两类（报告如实区分，不替你做语义裁决）：

**A. 语义疑似 Alias（颜色族，人工重点看）**：

```text
#4A789E/#82A9C5   XY.Accent.Default  ↔  XY.Text.Link
#D8E7F2/#35536A   XY.Accent.Soft / XY.Color.Selected / XY.State.Selected / XY.Surface.Selected / XY.Tag.Accent（5 个！）
#356C99/#7FB0D5   XY.Accent.Strong / XY.Button.Primary / XY.Tool.Active
#5C8FB4/#699CC0   XY.Border.Focus  ↔  XY.State.Focus
#4E7B66/#76A58A   XY.Color.Success  ↔  XY.Semantic.Success.Text
#A57634/#D0A05C   XY.Color.Warning / XY.Editor.Dirty / XY.Semantic.Warning.Text
#B34F58/#D4767D   XY.Color.Error  ↔  XY.Semantic.Error.Text
#A8B2B8/#697983   XY.State.Disabled.Text  ↔  XY.Text.Disabled
#647681/#B3BFC6   XY.State.ReadOnly.Text  ↔  XY.Text.Secondary
#F9FBFC/#222E36   XY.Surface.PanelAlt  ↔  XY.Surface.Toolbar
#FFFFFF/#2A3842   XY.Surface.Input  ↔  XY.Surface.Raised
```

**B. 数值巧合（尺寸族，多数只是同数字，语义独立）**：

```text
28 DIP   XY.HitTarget.Icon.Min / XY.Size.Control.S / XY.Size.TreeRow
16 DIP   XY.FontSize.Body / XY.Icon.Size.M / XY.Indent.PerLevel / XY.Size.Checkbox / ...
8 DIP    8 个 Token 同值（XY.Space.2 / XY.Panel.Padding / ...）
14 DIP   XY.FontSize.Body 与 XY.Icon.Size.S / XY.Size.Icon.S
10 DIP   XY.Scrollbar.Width  ↔  XY.Size.Scrollbar
0 ms     XY.Cursor.SwitchDelay  ↔  XY.Motion.Instant
180/250/100 DIP  XY.Layout.* 与 XY.Resize.* 的默认尺寸（语义同源，A2 已有 related_to）
```

**数值巧合 ≠ 语义 Alias**。A3-R2 只允许对语义同源的组合建立 alias；纯数字相同（如 16 DIP 既是指令体字号又是缩进）不得强行合并。

## 3. 已知重点案例核对（用户 A3-R1 指令列出）

| 案例 | 结果 |
|---|---|
| XY.Border.Strong = Color vs Width/Style | ✅ NAMESPACE_TYPE_COLLISION 确认 |
| XY.Border.Focus / Selected | ✅ NAMESPACE_TYPE_COLLISION 确认 |
| XY.State.Selected = 状态色 vs 状态语义 | ✅ NAMESPACE_TYPE_COLLISION 确认 |
| XY.Text.* 重复声明 | ✅ IDENTICAL_REDECLARATION ×3 确认 |
| XY.Size.* ↔ XY.Density.Compact.* | ✅ 已由 A2-R2 maps_to 裁定（不计入本轮碰撞） |

## 4. 门禁核对

```text
所有 TOKEN mapping 已扫描         ✅ 426/426
occurrence provenance broken      ✅ 0（每条带 evidence_refs）
同 ID 多 occurrence 有分类         ✅ 8/8 组
NAMESPACE_TYPE_COLLISION 未自动合并 ✅
VALUE_COLLISION 无自动选胜者       ✅（且 0 组）
IDENTICAL_REDECLARATION 未保留多真值 ✅（登记待 A3-R2 收敛）
ALIAS_CANDIDATE 只记候选           ✅（35 组均为 PROPOSED 性质）
A2 Registry 未修改                ✅
Source/Audit 未修改               ✅
无 XYUI1/2、无 AXAML/C#、无玄域代码 ✅
```

## 5. 产物

```text
xyui/tokens/audit/token-occurrences.json       ← 426 条全量 Inventory（含 light/dark/unit/type/provenance）
xyui/tokens/audit/token-collision-matrix.json  ← 8 碰撞 + 35 Alias 候选
xyui/tokens/audit/token-audit.md               ← 本报告
```

## 6. STOP 条件

### XYUI.Avalonia Theme Reconciliation 增补

Gallery 主题运行时新增一个跨组件 Semantic Token：

```text
XY.Icon.Mark = #526873/#D2E0E6
```

该 Token 专用于语义图标 / Code Mark，独立于 `XY.Text.*`；Foundation Runtime、Canonical Mapping、Avalonia Theme Dictionary、Gallery 和测试已同步消费。

A3-R1 结束。**A3-R2（Canonical Token Architecture）** 将基于本报告处理：

1. 5 组 NAMESPACE_TYPE_COLLISION 的拆分方案（如 `XY.Border.Color.*` / `XY.Border.Width.*` / `XY.StateColor.*`，**最终命名由你裁决**）
2. 3 组 IDENTICAL_REDECLARATION 的 canonical 收敛
3. 35 组 ALIAS_CANDIDATE 的语义甄别（A 组 alias / B 组独立）
