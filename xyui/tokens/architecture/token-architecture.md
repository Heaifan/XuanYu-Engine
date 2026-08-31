# XYUI-A3-R2 · Canonical Token Architecture

> A3 Design Token System 第二轮：确定四层 Token 架构与 Canonical 命名空间。
> 本轮仍**不生成 Light/Dark Token Source、不导出 AXAML、不进入 XYUI1/2**。
>
> 状态：`READY FOR USER ACCEPTANCE`。

## 1. 四层架构（正式采用）

```text
PRIMITIVE          承载基础值（颜色 hex / DIP 尺寸）
        ↓
SEMANTIC           承载设计语义
        ↓
ALIAS / MAPPING    仅 Source 明确依赖时建立
        ↓
COMPOSITE / RULE   组合 Token 或非 Scalar 规则，不得伪装成单值 Token
```

## 2. 五组 NTC 拆分方案（已落实进 canonical map）

### 2.1 Border（0.2-D 颜色 ↔ 0.10 宽度样式）

```text
颜色 Canonical Namespace（0.2-D 迁入）：
  XY.Border.Color.Subtle / Default / Strong / Focus / Selected

宽度命名空间保持（0.10）：
  XY.Border.Width.None / Default / Strong / Focus / Selected

0.10 的复合规则（XY.Border.Container/Control/Strong/Focus/Selected = N DIP/Solid）：
  → COMPOSITE_RULE 层，canonical_token_id = null，不作为 Scalar Token 导出
```

### 2.2 State（0.2-F 颜色 ↔ 0.20 状态语义）

```text
颜色 Canonical Namespace（0.2-F 迁入）：
  XY.State.Color.Hover / Pressed / Selected / Active / Focus / Dragging
  / DropTarget.Background / DropTarget.Border

状态行为语义保持（0.20）：
  XY.State.Selected = PersistentBaseState 等，名称不变（disposition: NTC_STATE_SEMANTICS_KEPT）
```

## 3. 三组 IDENTICAL 收敛

```text
Canonical Owner = 0.2-B TextColor：
  XY.Text.Primary / XY.Text.Secondary / XY.Text.Link

0.2-A（CorePalette）与 0.2-E（Accent）中相同 occurrence：
  → IDENTICAL_REDECLARATION_REF（引用，不产生第二份值源）
```

## 4. 四组显式 Semantic Alias（Source 明确语义来源）

```text
XY.Text.Link      → XY.Accent.Default   （L398 "Link 与基础 Accent 保持一致"）
XY.Tool.Active    → XY.Accent.Strong    （L399 "工具激活使用 AccentStrong"）
XY.Button.Primary → XY.Accent.Strong    （L400 "主按钮使用 AccentStrong"）
XY.Tag.Accent     → XY.Accent.Soft      （L396 "AccentSoft 用于…标签背景"）
```

## 5. 九组显式 Mapping（Source 明确「沿用/保持」）

```text
L1265   0.15 Icon 尺寸         → 0.8 Sizing（14/16/20 DIP）
L1822   0.25 Scrollbar 宽度    → 0.8 Sizing
L1915   0.27 Splitter HitTarget → 0.22 HitTarget（≥8 DIP）
L1917   0.27 布局默认尺寸      → 0.16 Layout（180/250/100）
L1112   0.11 Divider 颜色      → 0.2-D；厚度 → 0.10
L1611   0.21 Focus 颜色        → XY.Border.Focus（→ XY.Border.Color.Focus）
L1563   0.20 状态颜色          → 0.2-F（→ XY.State.Color.*）
L2133   0.31 A11y Focus        → 0.21
L2146   0.31 A11y HitTarget    → 0.22
A2-R2   0.19 Density           → 0.8 Sizing（6 条已裁定）
```

## 6. 同色 ≠ 同义（颜色 Alias 政策）

- **禁止** value-equality-only semantic alias（不因 Light/Dark 值相同就合并 Semantic Token）
- 同色多语义：多个 Semantic Token 引用同一 Primitive Value（10 个 primitive value group 已登记，如 `#D8E7F2/#35536A` 供 5 个语义 Token 共享——Accent.Soft/Color.Selected/State.Color.Selected/Surface.Selected/Tag.Accent）
- 未来 Dark Theme 可独立拆分，不被早期 alias 绑死

## 7. 数值巧合（COINCIDENTAL_EQUALITY）

73 个 occurrence 数值与其他 Token 相同但 Source 无显式依赖 → 语义独立，不建 alias（如 16 DIP 同是指令体字号/图标/缩进）。**不再默认判全部独立**——显式依赖已单独识别为 EXPLICIT_MAPPING/ALIAS（见第 5 节）。

## 8. Canonical Map 统计

```text
entries              426
canonical 后碰撞       0（ID 唯一）
UNCHANGED            291
COINCIDENTAL_EQUALITY 73
PRIMITIVE_SHARED      19
NTC_SPLIT_RENAME      13（Border 5 + State 8）
NTC_COMPOSITE_DEMOTE   5
NTC_STATE_SEMANTICS_KEPT 3
NTC_REFERENCE_NOTE     1
EXPLICIT_MAPPING       9
EXPLICIT_ALIAS         3（+1 owner 兼 alias = 4 组显式 alias）
A2_CONFIRMED           6
IDENTICAL_REDECLARATION_REF 3
CANONICAL_OWNER        3
NTC_REFERENCE_NOTE     1
```

## 9. 产物

```text
xyui/tokens/architecture/token-canonical-map.json   426 条 legacy→canonical 映射（含 migration reason + provenance）
xyui/tokens/architecture/token-architecture.json     四层架构 + 裁定 + primitive 值组 + 显式依赖注册
xyui/tokens/architecture/token-architecture.md       本报告
```

## 10. 边界核对

未创建 primitive/semantic/light/dark.json；无 .axaml/.cs；A2 Registry 未修改；未读取 XYUI1/2；Source/Audit 未修改。
