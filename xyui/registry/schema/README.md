# XYUI Foundation Registry Schema

> XYUI-A2-R1 产物（A2-R2 增补关系类型与 dialect 声明）。本文件说明 `foundation-registry.schema.json` 的设计意图、使用规则与验证策略。
>
> 状态：`READY FOR USER ACCEPTANCE`。本轮只定义结构，不录入 44 项（属 A2-R3）。

## 0. JSON Schema Dialect（A2-R2 冻结）

```text
XYUI Foundation Registry Schema
Dialect = JSON Schema Draft 7
```

- 声明依据：`foundation-registry.schema.json` 顶部 `"$schema": "http://json-schema.org/draft-07/schema#"`。
- 验证器必须固定使用 Draft 7（Python `jsonschema.Draft7Validator`），**禁止** Schema 写 Draft 2020-12 语法而验证器用 Draft7Validator 的错配。
- 若未来需要升级 dialect，必须先单独登记（治理轮），不得顺手改写。

## 1. 三层职责（不可混用）

| 层 | 路径 | 职责 |
|---|---|---|
| Immutable Design Evidence | `xyui/source/**` | 人类原始证据（XYUI-0.md，SHA 冻结，永不修改） |
| Evidence + Decision Chain | `xyui/audit/**` | R1 Evidence / 缺陷登记 / R2 五态裁决（历史证据，不覆盖） |
| **Canonical Machine Source of Truth** | `xyui/registry/**` | 机器唯一事实源（本 Schema 定义的结构） |

Source 负责「人类怎么定的」；Registry 负责「机器怎么执行」。Source 的 Formatting Defect 在 Registry 中可规范化语义，但必须保留 `source_defect_ref` 指向。

## 2. 核心设计：一条 Foundation Item 的形态

```json
{
  "id": "XYUI.Foundation.Density",
  "source": { "item_id": "XYUI0-0.19", "decision_status": "CONFIRMED_APPROVED", ... },
  "category": "foundation-density",
  "status": "ACTIVE",
  "engineering_mapping": [ { "type": "TOKEN" }, { "type": "INTERACTION_CONTRACT" } ],
  "rules": [...],
  "tokens": [...],
  "relationships": [...],
  "implementation": { "mapping": [], "tuning_required": [...] },
  "acceptance": [...]
}
```

## 3. 六种 Engineering Mapping（承接 0.1 P8）

XYUI0 不是 key:value Token 大合集。不同项映射不同工程对象：

| Mapping | 承载内容 | 典型项 |
|---|---|---|
| `TOKEN` | 可消费的命名值（颜色/尺寸/间距/透明度…） | 0.2 全系列、0.8、0.9 |
| `COMPONENT_RULE` | 组件结构与变体约束 | 0.17 Panel（五部件组合） |
| `STATE` | 交互状态语义与优先级 | 0.20（Disabled>Pressed>Hover>Selected） |
| `INTERACTION_CONTRACT` | 行为约束（Drag/Focus/Scroll/Resize…） | 0.19 Auto 切换、0.26 Drag |
| `ACCEPTANCE_RULE` | 最终验收可判定规则 | 0.31（双通道状态表达） |
| `POLICY` | 治理性规则（禁止项/强制等级） | 0.1（MagicNumber Forbidden 等） |

一个 Item 可映射多个 Type（如 0.19 Density = TOKEN + INTERACTION_CONTRACT）。

## 4. 三个正交维度（R2 裁定，Registry 不得重新合并）

```text
decision_status   CONFIRMED_APPROVED / PROBABLE_APPROVED / UNRESOLVED / CONFLICT / HISTORICAL_ONLY
source_quality    CLEAR / SOURCE_FORMATTING_DEFECT
relationships     canonical / alias / maps_to / overrides / derived_from / related_to
```

- `SOURCE_FORMATTING_DEFECT` ≠ `UNRESOLVED`：0.13 / 0.3-A / 0.24 设计已裁定，Registry 记录规范化语义 + `source_defect_ref`。
- `TOKEN_LAYER_OVERLAP` ≠ `CONFLICT`：0.2-A（色彩母版）与 0.2-C（Semantic Surface）分层共存，由 0.12 SurfaceRole 决定消费路径。

## 5. Provenance（硬要求）

每条 Canonical 内容必须可反查：

```text
source_id → item_id → decision_id → evidence_refs（R1 行号）
```

`evidence_refs` 格式：`XYUI0-x@L起-止`（与 R2 decision-classification.json 一致）。provenance 为 required 字段，缺失即 Schema 校验失败。

## 6. Alias / Mapping 与双真值防护

0.8 Sizing 与 0.19 Density Compact 数值一致（TreeRow 28 / Toolbar 30 / Input 32），R2 未擅建 alias。Registry 通过以下机制防「两个独立数值源漂移」：

```json
{
  "token_id": "XY.Density.Compact.TreeRow",
  "value": "28 DIP",
  "relation": "maps_to",
  "relation_target": "XY.Size.TreeRow"
}
```

- `canonical`：本 Token 是唯一真值源
- `alias` / `maps_to` / `derived_from`：值由 `relation_target` 决定（本处 value 仅作陈述记录，不构成第二真值源）
- `overrides`：明确覆盖目标 Token

**本轮不裁定 44 项的最终 alias 方向**（用户倾向 XY.Size.* 为 Base Source、XY.Density.Compact.* 为 Semantic Alias，A2-R2 正式裁决）。关系条目 `status` 用 `PROPOSED` 记录候选，`CONFIRMED` 才生效。

### 循环引用防护策略

1. **Schema 层**：`relation_target` 强制 pattern `XY.*`（Token）或 `XYUI.Foundation.*`（Item），且 alias 类关系强制携带 `relation_target`（JSON Schema `if/then` 约束）。
2. **验证器层（A2-R3 实装）**：对全部 `relation ∈ {alias, maps_to, derived_from, overrides}` 构建有向图，DFS 检测环（含自指）；发现环 → 校验 FAIL。canonical 目标不允许自身再挂 alias。
3. **约定层**：A2-R3 录入 44 项时，每项 token 的 canonical 归属必须在注册表级唯一（同一 token_id 只能有一条 canonical 定义，其余均为 alias/maps_to）。

## 7. Implementation Tuning（不污染 Decision）

0.19 Density Hysteresis：设计机制 APPROVED（必须有迟滞），实现参数（进入/退出阈值）需调优。Registry 表达：

```json
"implementation": {
  "mapping": [],
  "tuning_required": [
    { "parameter": "XY.Density.Auto.EnterThreshold", "status": "IMPLEMENTATION_TUNING_REQUIRED",
      "note": "实现调优参数；进入代码前必须 Token/Config/Acceptance Rule 冻结，禁止 C# 内随手写 40" }
  ]
}
```

`decision_status` 保持 `CONFIRMED_APPROVED` 不变。

## 8. 本轮范围边界

- ✅ 定义 Schema + README + 示例（2~3 项）
- ❌ 不录入 44 项（A2-R3）
- ❌ 不生成 Token 正式数据、AXAML、C#
- ❌ 不裁定 alias 方向（A2-R2）
- ❌ 不修改 `xyui/source/**`、不覆盖 `xyui/audit/**` 历史证据
