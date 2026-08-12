# XYUI Core Pack 0.1 · AGENT-GUIDE

> 给 XYLab Agent 的消费指南。做任何 UI 前先读本文件。

## 1. 强制流程

1. **做 UI 前先查 XYUI Core Pack**（本 pack）。
2. **Foundation 视觉值不得自行硬编码**——颜色/字号/字重/行高/间距/圆角/边框/Surface/State 色/Icon/Motion/Focus/HitTarget/DPI 一律引用 XYUI-0 Canonical Token。
3. **已有 XYUI Component Spec 时不得另造视觉语言**——XYUI-1 的 24 个文本组件已有 canonical spec（`xyui/specs/XYUI1/XYUI-1.canonical.md`）。
4. **缺组件时记录 `XYUI_GAP`**——不要在 XYLab 私自形成长期 UI 规范。
5. **允许项目临时实现行为**，但长期视觉规范必须回流 XYUI。

## 2. 优先级（0.1 Design Principles，冲突时高覆盖低）

```text
Correctness > SemanticClarity > EditingEfficiency > InformationDensity > VisualConsistency > Decoration
```

## 3. 禁止项（Guard 将检查）

```text
Cardification       禁止无意义 Card 套 Card
RandomSpacing       禁止零散间距（只用 XY.Space.* 档位）
MagicNumber         禁止未经 Token 化的硬编码数值
ColorOnlyState      禁止仅靠颜色表达状态（Error/Warning/Status 必须文字+颜色双通道）
DecorativeMotion    禁止装饰性动效（Motion 必须承担职责）
ArbitraryOverride   禁止绕过 Foundation Token 的任意覆盖
```

## 4. Canonical Token 引用规则

- 只能引用 `token-canonical-map.json` 中的 **canonical_token_id**。
- 旧命名已废弃（如 `XY.Border.Strong` 作为颜色 → 现为 `XY.Border.Color.Strong`；`XY.Border.Subtle` → `XY.Border.Color.Subtle`）。
- `canonical_token_id = null` 的条目是 Composite Rule（如 0.10 的 Border 结构规则），不是 Scalar Token，不得作为值引用。
- 组件独有参数必须用组件 Token 命名（如 `XY.Text.Default.FontSize`），不得伪装成 Foundation Token。

## 5. 当前可用资源

```text
XYUI-0 Foundation     ✅ VALIDATED（44 项，426 token）
XYUI-1 Text&Info      ✅ canonical（24 组件）
XYUI-2 Controls       ❌ SOURCE_MISSING（Source 到位后整理）
XYUI-3 Navigation     ❌ SOURCE_MISSING（Source 到位后整理）
```

## 6. 已知 GAP

```text
XYUI1-GAP-001  Icon glyph registry 未建立（glyph 名暂用组件级常量，如 InfoCircle）
XYUI2_SOURCE_MISSING / XYUI3_SOURCE_MISSING  → 阻塞项，等人类提供 Source
```

## 7. 版本与溯源

- Foundation Registry SHA：`33c2bd8b…`（manifest 中固定）
- 本 pack git commit：以 `xyui/packs/core-0.1/manifest.json` 的 `git_commit` 为准
- 所有规则可经 Registry → Evidence → Source 反查（broken provenance = 0）
