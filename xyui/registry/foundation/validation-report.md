# XYUI-A2-R4 · Foundation Registry Validation & Closeout Report

> 本报告是 XYUI-A2（Foundation Registry）的收口证据。看完这份报告即可回答：
> **「XYUI0 Registry 到底靠不靠谱？」**
>
> 状态：`XYUI-A2-R4 · READY FOR USER ACCEPTANCE`。

## 1. 全链一致性（R4-G01/G02）

```text
Source Items     = 44  （evidence-index.json）
Decision Items   = 44  （decision-classification.json）
Identity Items   = 44  （identity-map.json）
Registry Items   = 44  （foundation-registry.json）
四集合完全一致，missing = 0，extra = 0，mismatch = 0
```

Source Item ID（`XYUI0-0.19`）与 Canonical ID（`XYUI.Foundation.Density`）1:1 对应，无双向漂移。

## 2. Relationship（R4-G03~G06）

```text
relationship total   = 35   （relationship-map ↔ registry 集合完全一致）
child_of             = 12   （0.2 九子 + 0.3 三子）
graph cycle          = 0    （child_of/maps_to/alias/derived_from/overrides 有向图 DFS 无环）
自指                  = 0
```

## 3. 0.8 ↔ 0.19 重复值源防护（R4-G07/G08）

```text
XY.Size.TreeRow/Toolbar/Input      = Base Sizing canonical
XY.Density.Compact.*               = maps_to（六项与 R2 完全一致）
XY.Density.Comfortable.*           = overrides
Duplicate Canonical Value Source   = 0
```

`XY.Density.Compact.Gap/SectionGap` 为 Density 独有参数（0.8 无对应），非重复值源（A2-R3 已裁定）。

## 4. Source Formatting Defect（R4-G09）

```text
XYUI-SFD-001  Shadow.Panel / Shadow.Control 独立同级    ✅
XYUI-SFD-002  XY.Font.Mono 独立 Font Token             ✅
XYUI-SFD-003  TooltipHost.Role=Tooltip 正确归属        ✅
（Immutable Source 0 修改，SHA 未变）
```

## 5. TOKEN_LAYER_OVERLAP（R4-G10）

```text
XYUI-TLO-001/002 保留。
CorePalette（Base）→ Surface（Semantic）→ SurfaceRole（Role）三层链完整。
XY.Color.App / XY.Surface.App 均保留，未删除、未改名、未合并、未改值。
（Primitive/Semantic 收敛留 A3。）
```

## 6. Provenance（R4-G11/G12）

```text
checked references   = 519 条（rules + tokens 全部 evidence_refs 逐一反查）
broken provenance    = 0
orphan rule          = 0
```

每条 Canonical Rule/Token 均可反查：Registry → Decision → Evidence → Source 行号（行号 ≤ 源文件总行数校验通过）。

## 7. Engineering Mapping Coverage（R4-G13）

```text
TOKEN                  31 项
POLICY                 10 项
INTERACTION_CONTRACT    9 项
COMPONENT_RULE          6 项
STATE                   6 项
ACCEPTANCE_RULE         5 项
────────────────────────────
合计 67 个映射 / 44 项（每项 ≥1，mapping empty = 0）
```

规则总数 93 条，Token 总数 426 项。

## 8. Implementation Tuning（R4-G14）

```text
XY.Density.Auto.EnterThreshold  IMPLEMENTATION_TUNING_REQUIRED（无伪造数值）
XY.Density.Auto.ExitThreshold   IMPLEMENTATION_TUNING_REQUIRED（无伪造数值）
Density decision_status         = CONFIRMED_APPROVED（未污染）
```

## 9. Schema 三验 + 负例（R4-G15~G18）

```text
Schema self validation    PASS（Draft 7）
Registry validation       PASS（0 errors）
Example validation        PASS（0 errors）

负例 8/8 拒绝：
  1 illegal relationship type      ✅ 拒绝
  2 missing provenance             ✅ 拒绝
  3 empty engineering mapping      ✅ 拒绝
  4 invalid decision status        ✅ 拒绝
  5 invalid source quality         ✅ 拒绝
  6 bad SFD reference format       ✅ 拒绝
  7 duplicate canonical id         ✅ 检出
  8 TLO reference missing          ✅ 检出
```

## 10. 发现项（非门禁，登记备 A3 处理）

**Source 命名重叠**：以下 token_id 在多个 Item 中重复出现（Registry 忠实转录 Source，未擅自合并）：

```text
XY.Text.Primary / XY.Text.Secondary   CorePalette 与 TextColor 同值重复定义
XY.Text.Link                          TextColor 与 Accent 同值重复定义
XY.State.Selected / XY.State.Dragging InteractionStateColor（颜色值）与 InteractionState（状态语义值）同名
XY.Border.Strong / Focus / Selected   Border（宽度值）与 BorderDivider（颜色值）同名
```

这是 XYUI0 Source 自身的命名层重叠（同 token_id 承载不同层语义）。Registry 记录原样；**A3 Token System 必须解决 canonical 归属**（例如 Border 宽度与颜色拆分为不同 Token 命名空间）。本轮未做任何收敛。

## 11. Git

```text
Registry commit     db425762
Ahead / Behind      0 / 0
Working Tree        clean
```

## 12. A2 结论

```text
XYUI-0.md（Immutable Design Evidence）
        ↓
Foundation Registry（Canonical Machine Source of Truth，VALIDATED）
```

**XYUI-0 已从 XMind 规范完成第一阶段工程化**，44 项全部具备机器身份、工程映射与可反查出处。

## 13. 本轮禁止核对

未生成 Token Export / AXAML / C# / Theme；未实现组件/Gallery；未读取 XYUI1/2；未修改 XuanYu；未修改设计参数；未重命名 Canonical ID；未修改 Source/Audit 历史证据。
