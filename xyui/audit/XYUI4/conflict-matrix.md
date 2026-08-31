# XYUI-4 Conflict Audit / 冲突矩阵（C3）

- 阶段：`XYUI-PILOT R4 · C3 Conflict Audit`
- 方法：20 项逐条对照 A2 Foundation Registry、A3-R2 Token Architecture、XYUI-1/2/3 canonical 的实际内容（非仅依赖外部 Reconciliation 文档）
- 结论分级：`A` 真实语义冲突（需用户裁定） / `B` 组合层重叠（canonical 化可解） / `C` 第二真值映射（canonical 化可解） / `D` 交叉引用（无冲突） / `GAP` Token 缺口

---

## A 类 · 真实语义冲突（需用户裁定，canonical 化不得自行抹平）

### C-A1 · Hover 与 Selected 优先级矛盾

- Foundation（`InteractionState-R02`，XYUI0-0.20 原文）：`Hover.Priority=AboveSelected`——「Hover 优先于普通 Selected 表现」「高优先级状态覆盖低优先级状态」
- XYUI-4（4.01/4.02 原文）：「Hover 必须弱于 Selected」「Selected 仍然保持主体视觉，Hover 只允许非常轻的二级变化」「不得让选中对象在 Hover 时看起来像新的状态」
- 矛盾点：Foundation 的覆盖顺序（Hover 发生时应显示 Hover 视觉）与 XYUI-4 的视觉强度设计（Selected 视觉必须强于 Hover，选中对象 Hover 时保持 Selected 主体）在同一视觉通道上互斥。
- 外部 Reconciliation 文档未覆盖此点。

### C-A2 · Momentary Active 状态归类

- Foundation：`Pressed`（Pointer Down / Key Press 瞬时按压）与 `Dragging`（Drag 生命周期）是独立状态；`XY.State.Color.Active` 仅为视觉 token。
- XYUI-4（4.03 原文）：将 Pointer Down / Button Press / Drag / Resize / Handle Manipulation / Canvas Direct Manipulation 全部归入 `Momentary Active`。
- 依赖面：4.05 / 4.07 / 4.08 / 4.10 / 4.11 共 5 处引用「Momentary Active」概念——4.03 的裁定结果会联动这些条目。
- 关联：`Persistent Active`（当前 Tool / Mode / Toggle Tool）与 XYUI-2 `03 ToggleButton` 已定义的「ON 使用 Active / Selected 语义」「切换并保持某个工具、模式或功能状态」为同一语义族。XYUI-2 已先冻结，XYUI-4 只能引用/扩展，不得重定义。
- 外部 Reconciliation 文档裁定向：删除 Momentary Active、保留 Persistent Active——本审计确认该方向成立，但裁定权归用户。

### C-A3 · Focus 基础规范重定义

- Foundation（`Focus-R02`）：`Control.OutlineWidth=2`、`Control.OutlineColor=XY.Border.Focus`、`DisplayMode=FocusVisible`、`Mouse.PersistentRing=False`、`Control.Resize=Forbidden`
- XYUI-4（4.04 原文）：重新定义了 `Focus.Ring.Color=#326F8A`、`Focus.Ring.Width=2px`、`Focus.Ring.Offset=2px–3px`、`Focus.Border.Color`、`Focus.Border.Width`、Dual Ring 等——与 Foundation 形成第二真值。
- 可保留的增值：Dual Ring、Canvas Handle Halo、输入控件 Border Focus（Foundation 无此三项高级模式）。
- 外部 Reconciliation 文档裁定向：4.04 改为「Selection Context Focus」，基础视觉引用 `XY.Focus.*`——本审计确认成立。

### C-A4 · Drag 输入合同重述 + Direct Manipulation 入口扩展

- Foundation（`DragDrop-R02`）：`Entry=Handle`（Drag Handle 正式拖拽入口）、`Threshold=6`、`CancelKey=Esc`、`CancelSideEffect=Forbidden`、`DropZone=Before|Into|After`
- XYUI-4（4.11 原文）：① 重述了 Drag Cancel / Commit 合同（内容与 Foundation 一致，但形成第二表述）；② `Direct Manipulation` 允许 Canvas 对象本体（Region / Vertex / Handle / Scene Object）直接启动拖动——与 Foundation `Entry=Handle` 构成入口规则扩展。
- 外部 Reconciliation 文档裁定：4.11 只负责 Presentation；另建议 Amendment B 修订 Foundation Drag Entry 范围——**修订 Foundation 属上游变更，须用户批准**。

### C-A5 · 附件 Amendment A（ComposeMode 解释性修订）需动上游

- Foundation（`InteractionState-R02`）：`ComposeMode=Single`、`Layering=Forbidden`
- 外部 Reconciliation 文档建议修订为 `SemanticCoexistence=Allowed / VisualCompose=SingleResolved / ArbitraryLayering=Forbidden`。
- 本审计确认 Foundation 原文确实只写了 Single/Forbidden，多语义事实共存（Selected+Hover+Focus）无明确出口。
- **修订 Foundation Registry 属上游变更，须用户批准**；批准前 XYUI-4 只能按现有 Single/Forbidden 措辞消费，不得在 XYUI-4 内另立解释。

---

## B 类 · 组合层重叠（canonical 化以 Composition 落地，无需语义裁定）

### C-B1 · 4.19 InlineFeedback 与 XYUI-1 六组件重叠

- XYUI-1 已冻结：`16 ErrorText`、`17 WarningText`、`15 HelpText`、`10 StatusBadge`、`11 StatusDot`、`13 IconLabel`（各自含 Component-Specific token，如 `XY.ErrorText.Default.Foreground/Mark.*`）。
- XYUI-4 4.19 原文重新定义了 Error/Warning/Info/Success 的文本、图标、颜色（`Feedback.Error.Border=#B57D78` 等）。
- 裁定方向：4.19 改为 **Composition Pattern**（Target + Severity Border + XYUI-1 组件 + Optional Action）；XYUI-4 保留 Validation Timing / Debounce / Async Validation / Lifecycle / Cross-field Error。
- 颜色映射：Success→`XY.Semantic.Success.*`、Info→`XY.Semantic.Info.*`、Warning→`XY.Semantic.Warning.*`、Error→`XY.Semantic.Error.*`。

### C-B2 · 4.20 EmptyState 与 XYUI-1 EmptyText

- 关系：`XYUI-1 22 EmptyText`（Primitive）→ XYUI-4 EmptyState（Pattern / Composition）。
- 设计稿已自证：「使用 XYUI Button 正式组件」→ 引用 XYUI-2 `01 Button`。
- 无冲突，canonical 化按 Primitive→Pattern 落地。

### C-C1 · 4.03 Persistent Active 与 XYUI-2 ToggleButton ON 状态

- XYUI-2 ToggleButton 已定义 ON/OFF 语义与 `XY.ToggleButton.Background.On = XY.State.Color.Active`。
- XYUI-4 Persistent Active（当前 Tool / Mode / Toggle Tool / Editing Context）必须定位为对该语义的应用层扩展，引用而不重定义。

### C-C2 · 4.14 Button Async Loading

- XYUI-2 Button 无 Loading/Pending 状态定义 → XYUI-4 4.14/4.15 Button Spinner 为 NEW 扩展，无冲突。

---

## C 类 · 第二真值映射清单（canonical 化统一替换，已附映射目标）

| 设计稿原文 | 收敛目标 |
|---|---|
| `Hover.Surface.Background=#E8F0F3` 等 4.01 全部 hex | `XY.State.Color.Hover` 等 Foundation Token（组件表达差异由 XYUI-4 模式层描述） |
| `Selected.Background=#DCEAF0` 等 4.02 全部 hex | `XY.State.Color.Selected` / `XY.Surface.Selected` / `XY.Border.Color.Selected` |
| `Active.*=#CBDDE5/#C3D7E0` 等 4.03 hex | `XY.State.Color.Active` / `XY.State.Color.Pressed` |
| `Focus.Ring.Color=#326F8A` 等 4.04 hex | `XY.Focus.Control.OutlineColor`（=XY.Border.Focus）/ `XY.Focus.Control.OutlineWidth` |
| 4.05 Primary/Secondary hex | `XY.Editor.Selection` / `XY.Editor.MultiSelection` |
| 4.09 `Separation.Color=#F8FAFB` | GAP 登记（见 GAP-1），不得硬编码 |
| 4.10 `Handle.Border=#326F8A` 等 | `XY.Editor.Handle` / `XY.Editor.BoundingBox` / `XY.HitTarget.ResizeHandle` |
| 4.12 `Drop.Target.Border=#326F8A` | `XY.State.Color.DropTarget.Border` |
| 4.12 `Drop.Valid.Border=#6F9C8A` / `Drop.Invalid.Border=#B57D78` | `XY.Semantic.Success.*` / `XY.Semantic.Error.*` |
| 4.13 `Insertion.Anchor.Color=#326F8A` | Accent/Selection 语义 token |
| 4.14/4.15 Spinner/Activity hex | `XY.Accent.*` + Component-Specific 尺寸 |
| 4.16/4.17 Progress hex | `XY.Accent.*` + Component-Specific 高度/描边 |
| 4.18 Skeleton hex | 低对比 Surface token（`XY.Surface.*` 家族），映射后登记 Component-Specific |
| 4.19 Feedback 四色系 | `XY.Semantic.{Success,Info,Warning,Error}.*` |
| 4.20 EmptyState 文字 hex | `XY.Text.*`（Secondary 语义） |
| 全部 78 处 `px` | DIP（值不变，单位统一） |

---

## D 类 · 交叉引用点（无冲突，canonical 化注记即可）

- XYUI-3 `17 CommandBar` 状态含 `Loading`（异步命令处理中）、`13 Pagination` 含 `IsLoading`、`14 Steps` 含 Progress 语义——XYUI-4 4.14~4.17 建立通用 Loading/Progress 语言后，XYUI-3 这些局部语义是未来回引点；本轮不改 XYUI-3。
- 4.13 的 Tree Indent → `XY.Indent.PerLevel=16 DIP`（XYUI-3 已同引用）。
- 4.07/4.08 性能规则（PointerMove 不得 O(N) 全场景扫描 / 空间索引）与玄域引擎 Spatial Index 工程经验一致，无上游冲突。

---

## GAP 登记（不得伪造，待后续 Token Source 裁决）

| Gap ID | 内容 | 类型 |
|---|---|---|
| XYUI4-GAP-001 | `Contrast/SeparationForeground`：4.09 SelectionOutline 的 Separation Stroke（浅色分离描边）无 Foundation token；与 XYUI3-GAP-001（COMPONENT_SPECIFIC_CONTRAST_FOREGROUND）同家族 | MISSING_TOKEN |
| XYUI4-GAP-002 | `Focus Ring Offset`（4.04 `Focus.Ring.Offset 2–3 DIP`）：Foundation Focus 无 Offset token | MISSING_TOKEN |
| XYUI4-GAP-003 | Marquee/Lasso `FillOpacity 0.05–0.12`：Foundation `XY.Opacity.*` 六档无低透明 Accent Fill 档位 | REQUIRES_DECISION |

---

## 20 项 Disposition 核对（附件 Reconciliation vs C3 实际）

| 项 | 附件结论 | C3 核对 | 差异 |
|---|---|---|---|
| 4.01 HoverState | EXTENSION | EXTENSION（+发现 C-A1 优先级冲突） | 附件漏报 C-A1 |
| 4.02 SelectedState | EXTENSION | EXTENSION（+C-A1 关联） | 附件漏报 C-A1 |
| 4.03 ActiveState | CONFLICT·REWRITE | CONFLICT（+发现与 XYUI-2 ToggleButton 重叠 C-C1） | 补充 C-C1 |
| 4.04 FocusState | CONFLICT·RE-SCOPE | CONFLICT·RE-SCOPE | 一致 |
| 4.05 MultiSelection | EXTENSION | EXTENSION（依赖 4.03 裁定） | 补充依赖 |
| 4.06 SelectionGroup | NEW | NEW | 一致 |
| 4.07 MarqueeSelection | NEW | NEW（依赖 4.03） | 补充依赖 |
| 4.08 LassoSelection | NEW | NEW（依赖 4.03） | 补充依赖 |
| 4.09 SelectionOutline | EXTENSION | EXTENSION（GAP-001） | 一致 |
| 4.10 BoundingBox | EXTENSION | EXTENSION（依赖 4.03） | 补充依赖 |
| 4.11 DragFeedback | CONFLICT·RE-SCOPE | CONFLICT（含 Amendment B 上游变更） | 一致 |
| 4.12 DropIndicator | CONFLICT·TOKEN | CONFLICT·TOKEN | 一致 |
| 4.13 InsertionIndicator | EXTENSION | EXTENSION | 一致 |
| 4.14 LoadingIndicator | NEW | NEW | 一致 |
| 4.15 Spinner | NEW | NEW | 一致 |
| 4.16 ProgressBar | NEW | NEW | 一致 |
| 4.17 ProgressRing | NEW | NEW | 一致 |
| 4.18 Skeleton | NEW | NEW | 一致 |
| 4.19 InlineFeedback | COMPOSITION | COMPOSITION | 一致 |
| 4.20 EmptyState | COMPOSITION | COMPOSITION | 一致 |

---

## 状态

`CONFLICT AUDIT COMPLETE · 5 项 A 类冲突待用户裁定 · canonical 化等待放行`
