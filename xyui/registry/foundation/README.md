# XYUI Foundation Registry · Identity & Relationship Map

> XYUI-A2-R2 产物。本轮建立 44 项的 Canonical Identity、层级与关系映射；**不录入完整 Registry 数据、不生成 Token**（属 A2-R3）。
>
> 状态：`READY FOR USER ACCEPTANCE`。

## 1. 双身份模型（Source ID ≠ Canonical ID）

```text
Source Item ID    XYUI0-0.19            ← Design Evidence 中的章节号（不可变）
Canonical ID      XYUI.Foundation.Density ← 机器稳定身份（Display Name 可变，此 ID 不变）
```

44/44 项映射见 `identity-map.json`。两个 ID 都保留，Source ID 不得替代 Canonical ID，反之亦然。

## 2. 层级结构（child_of 单向）

```text
XYUI.Foundation.ColorSystem
├─ CorePalette (0.2-A)
├─ TextColor (0.2-B)
├─ Surface (0.2-C)
├─ BorderDivider (0.2-D)
├─ Accent (0.2-E)
├─ InteractionStateColor (0.2-F)
├─ SemanticColor (0.2-G)
├─ DisabledReadOnlyLocked (0.2-H)
└─ EditorColor (0.2-I)

XYUI.Foundation.Typography
├─ FontFamily (0.3-A)
├─ FontWeight (0.3-B)
└─ FontSize (0.3-C)
```

只存 `child_of`（子→父），反向 `parent_of` 由查询计算，禁止双边手写。

## 3. 核心关系裁定（A2-R2，CONFIRMED）

### 3.1 0.8 Sizing ↔ 0.19 Density：消除双真值

```text
XY.Size.*                    = Base Sizing Token（canonical 值源）
XY.Density.Compact.*         = maps_to XY.Size.*   （同值，不复制）
XY.Density.Comfortable.*     = overrides           （独立语义值，显式覆盖）
```

Token 级映射（`relationship-map.json` → `token_mappings`，6 条）：

```text
XY.Density.Compact.TreeRow   maps_to  XY.Size.TreeRow   (28 DIP)
XY.Density.Compact.Toolbar   maps_to  XY.Size.Toolbar   (30 DIP)
XY.Density.Compact.Input     maps_to  XY.Size.Input     (32 DIP)
XY.Density.Comfortable.TreeRow overrides XY.Size.TreeRow (32 DIP)
XY.Density.Comfortable.Toolbar overrides XY.Size.Toolbar (34 DIP)
XY.Density.Comfortable.Input   overrides XY.Size.Input   (36 DIP)
```

A3 生成 Token 时**不得**出现两个独立硬值 `28 / 28`——Compact 必须是映射关系而非复制值。

### 3.2 0.2-A → 0.2-C → 0.12：三层 Token 架构

```text
XYUI.Foundation.ColorSystem.CorePalette   ← Base / Palette 层（0.2-A，TOKEN_LAYER_OVERLAP）
        ↓ related_to（分层共存，保留 TLO-001/002）
XYUI.Foundation.ColorSystem.Surface       ← Semantic 层（0.2-C）
        ↑ maps_to
XYUI.Foundation.SurfaceRole               ← Consumption / Role Mapping 层（0.12）

组件消费路径：Panel → XY.SurfaceRole.Panel → XY.Surface.Panel
（而非 Panel → XY.Color.Panel）
```

不删除、不重命名、不修改任何 Source Token；A3 时决定 XY.Color.App/Panel 保留为 Base/Reference Token 还是重命名 Primitive。

### 3.3 0.20 InteractionState ↔ 0.21 Focus：保持独立

```text
XYUI.Foundation.InteractionState
        related_to（独立，非 child_of）
XYUI.Foundation.Focus
```

原文明确：Focus Outline 不参与 0.20 背景状态覆盖链。Focus 不是 InteractionState 的子项。

### 3.4 Sizing ↔ HitTarget：保持独立

```text
XYUI.Foundation.Sizing
        related_to（独立）
XYUI.Foundation.HitTarget
```

`VisualSizeIndependent = True` 保持：Icon 视觉 16 DIP、热区 28 DIP，不得为触控把 Icon 画成 28。

## 4. PROPOSED 候选关系（18 条，A2-R3 逐条裁定）

R1 possible_relation 的全部候选已登记为 `related_to / PROPOSED`（如 FontSize↔LineHeight、Border↔Divider、Layout↔ResizeSplitter、A11y↔Focus/HitTarget、DPI↔Density 分离等）。A2-R3 录入时逐条裁定升 CONFIRMED 或撤销。

## 5. Implementation Tuning Registry

```text
XY.Density.Auto.EnterThreshold   IMPLEMENTATION_TUNING_REQUIRED
XY.Density.Auto.ExitThreshold    IMPLEMENTATION_TUNING_REQUIRED
```

决策状态保持 `CONFIRMED_APPROVED`（机制已定：必须有迟滞）；阈值是实现调优参数，进入代码前必须 Token/Config/Acceptance Rule 冻结，禁止 C# 随手写 `40`（0.1 MagicNumber Forbidden）。

## 6. Schema Dialect

```text
Dialect = JSON Schema Draft 7
```

`foundation-registry.schema.json` 顶部 `"$schema": "http://json-schema.org/draft-07/schema#"` 已声明；验证器固定 `Draft7Validator`。A2-R2 已把 `child_of` 增补进 relationship enum。

## 7. 验收六件事对照

| # | 检查点 | 状态 |
|---|---|---|
| 1 | 44 项都有稳定 Canonical ID | ✅ identity-map.json 44/44，unique |
| 2 | 0.2/0.3 子节层级正确 | ✅ 12 条 child_of（9+3） |
| 3 | 0.8↔0.19 消除双真值 | ✅ 6 条 token 级 maps_to/overrides |
| 4 | 0.2-A→0.2-C→0.12 三层正确 | ✅ Palette→Semantic→Role + 消费路径 |
| 5 | Focus 未塞进状态覆盖链 | ✅ related_to，非 child_of（断言验证） |
| 6 | Source ID 与 Canonical ID 未混 | ✅ 双字段并存，双向反查 |

## 8. 本轮禁止（未做）

未录入 44 项 Registry 数据、未生成 Token JSON、未生成 AXAML/C#、未实现 Theme、未碰 XYUI1/2、未碰玄域、未修改 Source/Audit 历史证据。
