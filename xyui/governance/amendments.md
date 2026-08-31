# XYUI Foundation Amendments / 上游修订记录

- 状态：`C3-F1 · Foundation Amendment A/B · 用户批准（2026-08-13）`
- 来源：XYUI-4 C3 Conflict Audit（`xyui/audit/XYUI4/conflict-matrix.md`）的 C-A1/C-A4/C-A5 裁定
- 原则：**不得修改 `xyui/source/XYUI0/XYUI-0.md` 不可变证据源**；只修改正式 Registry / 映射与审计产物。
- 证据源内原文保持原样；Registry 层以下述 Amendment 解释为准，两者不一致处以本文件为 Registry 层权威。

---

## AMEND-A · State Composition 解释修订

- 对象：`XYUI.Foundation.InteractionState`（rules R01/R02）
- 背景：Foundation 原文「同一控件同一时刻只展示一个最终视觉状态」「ComposeMode=Single」「Layering=Forbidden」与 XYUI-4 4.01/4.02 的「Selected 保持主体视觉、Hover 只做二级变化」在同一视觉通道上产生解读冲突（C-A1）。
- 裁定：双轴模型（用户选 (a)）。

### 修订后语义

```text
Render / Composition Priority
    Hover > Selected
    （Hover 可以叠加绘制在 Selected 之上）

Visual Salience / Semantic Ownership
    Selected > Hover
    （Selected 是持久主体状态；Hover 是瞬时交互提示）
```

- `ComposeMode = Single` 解释为：**同一视觉通道（Visual Channel）同一时刻只有一个 Primary Owner**——不是「整个控件一次只表现一个状态」。
- `Layering = Forbidden` 解释为：禁止多个状态争夺同一 Primary Channel 产生不可预测叠色——**不禁** Selected + Hover + Focus 跨通道合法组合。
- 通道分工（Canonical）：

```text
Selected   → 主体 Fill / 主 Accent / 持久选中边界 / Selection identity
Hover      → 瞬时 Overlay / 辅助 Edge / Halo / Cursor proximity feedback
Focus      → 独立 Focus Ring（0.21 独立通道）
Dragging   → Ghost / Drag Indicator（独立状态）
```

- 明文禁令：Hover 不得夺取 Selected 的主体视觉通道；「直接换成 Hover 色」为违规表现。

---

## AMEND-B · Drag Entry 扩展

- 对象：`XYUI.Foundation.DragDrop`（rules R01/R02 + token `XY.Drag.Entry`）
- 背景：Foundation 原文 `Entry=Handle` 与 XYUI-4 4.11 Direct Manipulation（Canvas 对象本体启动拖动）冲突（C-A4）。
- 裁定：**不改成 Anywhere Drag**，正式扩展为两类入口。

### 修订后语义

```text
Drag Entry
├─ Handle
│     显式拖动控制柄
│     Resize Handle / Vertex Handle / Splitter Handle / Slider Thumb
│
└─ DirectTarget
      对象本体即合法操作目标
      Canvas Object / Map Marker / Node / Card / Selected Geometry
```

- Token 变化：`XY.Drag.Entry` = `Handle` → `Handle|DirectTarget`。
- 门控：`DirectTarget` 仅在组件明确声明 `DirectManipulation = Allowed` 时成立；未声明组件（Button / TextField 等）不因本 Amendment 获得本体拖动能力。
- 覆盖对象：玄域地图编辑器现存的 Marker / Region / Road / Canvas Object。

---

## 受影响引用审计

### 已修改（本轮）

| 文件 | 变更 |
|---|---|
| `xyui/registry/foundation/foundation-registry.json` | InteractionState R01/R02 text 重写（AMEND-A）；DragDrop R01/R02 text 重写 + `XY.Drag.Entry` value 更新（AMEND-B）；44 items 结构不变；registry_version 保持 0.1.0（schema const） |
| `xyui/tokens/architecture/token-canonical-map.json` | `XY.Drag.Entry` value → `Handle|DirectTarget`；顶层 note 追加 Amendment 记录；`XY.State.ComposeMode/Layering/Hover.Priority` 枚举值不变（解释由 Registry rules 承载） |
| `xyui/registry/foundation/foundation-registry.manifest.json` | registry_sha256 重算；status 更新；新增 amendments 记录 |

### 保持原样（审计结论）

| 文件 | 原因 |
|---|---|
| `xyui/source/XYUI0/XYUI-0.md` | 不可变证据源，禁止修改 |
| `xyui/audit/XYUI0/evidence-index.json`、`source-audit.md` | A1 证据记录忠实镜像 Source 原文；Amendment 不改 Source，provenance 要求保持原样 |
| `xyui/audit/XYUI4/conflict-matrix.md`、`source-audit.md` | C2/C3 审计时点历史记录 |
| `xyui/specs/XYUI2/XYUI-2.canonical.md`（L25/L1492 `ComposeMode = Single`） | Token 名与枚举值均未变，引用继续合法；解释收紧由 Registry 层传导 |
| `xyui/specs/XYUI3/XYUI-3.canonical.md`（L49 `ComposeMode = XY.State.ComposeMode`） | 同上，纯 token 引用 |
| `xyui/tokens/audit/token-occurrences.json` | A3-R1 审计时点历史记录 |
| `xyui/registry/foundation/validation-report.md`、`identity-map.json`、`relationship-map.json` | A2-R4 历史收口证据，无引用需要更新 |
| `xyui/packs/core-0.1/manifest.json` | 第二段提交补记 SHA 与 commit（两段式，见 Git 记录） |

### 影响引用数量

```text
Registry 修改条目      2 / 44（InteractionState、DragDrop）
Token 修改条目         1 / 426（XY.Drag.Entry）
下游 canonical 受影响  0（XYUI-1/2/3 无文本需改；语义由 Foundation 层传导）
```

---

## 验证

```text
JSON parse             registry / token map / manifests 全部通过
Registry 结构          items=44、字段集合与 schema 0.1.0 一致（未增删字段）
枚举稳定性             ComposeMode=Single、Layering=Forbidden、Hover.Priority=AboveSelected 值不变
SHA 重算               registry 748a0ffa…、token map 6625323a…
```

---

## AMEND-C · XYUI-2-04 Split Button（Soft Partition · R2 细化）

- 对象：`XYUI-2-04 Split Button`（`XY.SplitButton`）
- 背景：Batch 01B 首版实现经用户人工审核被判定「偏成 Button + 附加小按钮」，未体现 Soft Partition。方案本身（方案 2 · Soft Partition）不变，仅细化视觉实现。
- 裁定：维持 Soft Partition，细化下列视觉值（用户批准 2026-08-25）。

### 修订后语义

```text
整体轮廓   单一 Button Chrome 外轮廓（Border 贯通全宽），不出现"两枚按钮"边界
MenuHover 菜单区不再整块背景高亮；独立反馈只落在区内 Chevron 描边
           Hover = XY.Accent.Strong / Pressed = XY.Border.Color.Selected / Disabled = XY.State.Disabled.Text
Divider    Height 18 → 12 DIP（COMPONENT_SPECIFIC）
           Color XY.Divider.Default → XY.Border.Color.Subtle（更淡，软分区提示而非硬分界）
ActionEdge 仍为跨全宽的单条共享边（XY.Accent.Strong），数量与语义不变
Hit Zone   Main/Menu 独立 Hover/Pressed，互不串发；Main 主区保留整块 Hover 洗色
```

- Token 变化：`XY.SplitButton.Divider.Height` 18 → 12 DIP；`XY.SplitButton.Divider.Color` `XY.Divider.Default` → `XY.Border.Color.Subtle`。
- 覆盖对象：仅 `XYUI-2-04`；01/02/03 视觉与契约不受影响。
- 引用更新：`xyui/specs/XYUI2/XYUI-2.canonical.md`（04 Divider 两条）、`XYUI-2.mapping.json`（04 对应 refs）。

---

## AMEND-D · XYUI-2-04 Split Button（MenuZone 状态表面 · R2.1 修正）

- 对象：`XYUI-2-04 Split Button`（`XY.SplitButton`）
- 背景：R2（AMEND-C）后用户人工审核发现 Regression：Gallery 中悬停右侧箭头时菜单区无独立状态表面，且宿主主题（Fluent/Simple）的 `Button:pointerover /template/ ContentPresenter` 会把模板内 ContentPresenter 刷成主题 hover 刷子（Light 浅灰 / 非 Light 近黑），破坏 Soft Partition 视觉与 Hit Zone 独立合同。
- 裁定：修正 AMEND-C 中「菜单区不再整块背景高亮」的语义（用户批准 2026-08-25）。

### 修订后语义（覆盖 AMEND-C 对应行）

```text
MenuHover  ONLY Menu Zone 使用 Hover Surface（XY.State.Color.Hover）
           主区保持 Default（Transparent，透出 Chrome 底）
           Chevron 同步 = XY.Accent.Strong（Soft Partition 独立反馈）
MenuPressed ONLY Menu Zone 使用 Pressed Surface（XY.State.Color.Pressed）
           Chevron = XY.Border.Color.Selected
MenuDisabled 保持 Transparent（Chrome 已整体衰减）
模板表面   Main/Menu 两区同步覆盖模板内 ContentPresenter 的
           :pointerover / :pressed / :disabled 背景为上述 XY Surface，
           阻止宿主主题 hover 刷子（浅灰/近黑）泄漏到分区
```

- 软分区语义不变：单一 Chrome 外轮廓、短淡 Divider、跨全宽共享单条 Action Edge、MenuZone≈36 DIP；菜单区的 Hover Surface 发生在共享轮廓内部，不显示独立边框/边缘，不构成「第二颗按钮」。
- 覆盖对象：仅 `XYUI-2-04`；01/02/03 视觉与契约不受影响；Chevron Y=+1 DIP 光学校准保持不变。
- 契约测试：`Main_and_menu_hover_are_independent_zones` 锁定 MainHover ≠ MenuHover，且两状态只影响各自 Hit Zone（含模板 ContentPresenter 断言）。
