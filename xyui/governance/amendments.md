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
