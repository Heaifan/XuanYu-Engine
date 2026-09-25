# Agent 防复发经验规则库

> 路径：`docs/governance/agent-experience-rules.md`  
> 唯一职责：保存从一个或多个真实 ERR 中提炼出的、可复用、可注入未来任务的 Agent 防复发规则。

本文件不是错误事实库。错误事实统一记录在 `agent-error-log.md`。

---

## 1. 权限

- **ChatGPT**：可创建、修改、合并、强化、替代、退役 EXP。
- **Codex / Gemini / 其他执行 Agent**：只读；必须在相关任务中遵守已加载的 EXP，可提交新增 / 修改建议和验证证据。
- **用户**：拥有最终裁定权。

## 2. ID

格式：

```text
EXP-<TYPE>-NNN
```

TYPE 与 ERR 主类型一致：

```text
LOGIC / ARCH / TEST / SCOPE / GOVERNANCE / REPORT / REGRESSION / UI / DATA
```

## 3. 状态

```text
ACTIVE
SUPERSEDED
RETIRED
```

- `ACTIVE`：当前有效，相关任务必须加载。
- `SUPERSEDED`：已被更准确规则替代，必须写明替代 EXP-ID。
- `RETIRED`：当前技术 / 流程已不再适用，保留历史原因。

历史 EXP 不得无痕删除。

## 4. 标准模板

```markdown
## EXP-LOGIC-001 提交分层

状态：ACTIVE
适用范围：Editor / Inspector / Commit Pipeline
触发条件：涉及 Dispatcher、Command Routing、统一提交入口或领域 Commit Core
Occurrences：3

规则：
统一 Dispatcher 只负责路由、校验和记录；领域 Core 只执行领域修改；Core 不得反向调用 Dispatcher。

根因模式：
统一入口与底层执行函数职责倒置或互调，造成递归、重复提交或重复副作用。

禁止：
- Core 回调统一入口；
- 为统一记录而让底层执行层重新进入 Dispatcher。

正确做法：
- Dispatcher → Core 单向调用；
- 记录 / 路由放在入口；
- 领域修改只发生在 Core。

来源 ERR：
- ERR-YYYYMMDD-NNN

任务注入：
涉及上述触发条件时，在 Knowledge Preflight 中加载本规则。

验证 / 自动化：
说明相关回归测试、静态检查或门禁；没有时写“尚无自动门禁”。

Superseded by：
无
```

## 5. 创建规则

只有满足以下条件才创建新 EXP：

1. 至少存在一个真实 ERR；
2. 根因已经有足够证据；
3. 结论具有跨单次修复的复用价值；
4. 与现有 EXP 的根因 / 防复发机制实质不同。

一次性实现失误但无通用规律时，只保留 ERR，不强行创建 EXP。

## 6. 合并与累计

同一根因再次发生：

```text
新 ERR
→ 关联原 EXP
→ Occurrences + 1
→ 补充来源 ERR
→ 必要时强化触发条件 / 验证方式
```

不得为措辞不同但本质相同的问题创建多条近义 EXP。

## 7. 任务注入

EXP 必须能真正进入未来开发上下文。

`docs/knowledge/knowledge-index.md` 负责将任务域映射到相关长期知识；EXP 本身必须写清“适用范围”和“触发条件”。

MEDIUM / HIGH 任务或 Index 已登记任务域在开始前执行 Knowledge Preflight，并加载命中的 ACTIVE EXP。

执行 Agent 不需要全文读取所有 EXP，只加载与任务直接相关的规则。

## 8. 自动化升级

重复发生、Critical、P0 等高风险经验，应优先评估：

```text
EXP
↓
Regression Test
↓
Static Check / Runtime Gate / Architecture Gate
```

能够机器防止的错误，不应长期只依赖 Agent 记忆。

自动化门禁建立后，EXP 保留并记录对应 Gate；机器门禁不会使历史经验失去价值。

## 9. 宪法候选

符合以下任一条件，可由 ChatGPT 提出 `CONSTITUTION_CANDIDATE`：

- 同类经验跨两个及以上独立任务重复发生；
- 单次 Critical 且风险具有普遍性；
- 直接威胁架构、数据完整性或事实可信度。

执行 Agent 不得自行把 EXP 写入宪法。

宪法升级必须由用户批准并由 ChatGPT 正式修改。

## 10. 经验库健康检查

月度治理至少检查：

- ACTIVE EXP 是否仍适用；
- 是否存在重复规则可以合并；
- Occurrences 是否与 ERR 事实一致；
- SUPERSEDED 是否正确链接替代项；
- 是否存在长期依赖人工记忆但适合自动化的 EXP；
- 是否有已失效规则应 RETIRED；
- 是否有高频规则值得提出宪法候选。

---

## 当前规则

## EXP-UI-001 稳定 Inspector 编辑目标

状态：ACTIVE
适用范围：Editor / Inspector / 属性提交 / Recent MRU
触发条件：属性编辑可能在提交前经历 Selection 切换，或同一属性存在 Entity 与 Feature 多条提交入口。
Occurrences：1

规则：
编辑开始时必须捕获 `InspectorEditTarget = ObjectKind + ObjectId + PropertyKey`。Identity、Commit、Recent 必须消费同一个目标；提交不得重新读取当前 Selection 决定目标。

根因模式：
把当前 Selection 当作编辑会话身份，导致 A 开始编辑、切换到 B 后提交到 B；Recent 只按类型而不按对象身份隔离；专用入口重新进入统一入口造成递归。

禁止：
- 用提交瞬间的 `_selectedMapGeometry` 覆盖已捕获目标；
- 只按 `ObjectKind` 存储 Recent；
- 让领域 Commit Core 反向调用统一 Dispatcher；
- 只用单一 Road Happy Path 测试代表全部 Inspector 对象类型。

正确做法：
- 行编辑器或输入框获得焦点时捕获稳定目标；
- Commit 按目标 `ObjectId` 调用对应领域操作；
- Recent 按对象身份隔离；
- 覆盖 Entity、Road、Region、Marker 以及 A→B Selection 切换回归。

来源 ERR：
- ERR-20260916-001

任务注入：
涉及 Inspector、属性提交、Recent 或 Selection 切换时，在 Knowledge Preflight 中加载本规则。

验证 / 自动化：
`InspectorEntityEditTargetTests`、`InspectorPropertyTargetTests` 与 Inspector 专项回归；当前已通过 16/16。后续可继续评估静态检查或更高层运行时门禁。

Superseded by：
无


---

## EXP-ARCH-001 连续局部修复停止线

状态：ACTIVE
适用范围：Architecture / Native UI / Rendering / Viewport / Diagnostic / Hosting
触发条件：同一真机问题已经连续两个针对性 FIX 仍未解决，或多个子系统同时出现相近症状。
Occurrences：2

规则：
同一问题连续两个针对性局部修复仍未收口时，禁止默认进入第三次参数/Placement/Offset/ZIndex/Host 微调。必须先停止实现，重新审计 Ownership、Runtime Route、Coordinate Space、Hosting/Airspace 与 Shared Dependency。

根因模式：
Agent 把症状变化误当成接近根因，在错误承载前提上持续追加 workaround；真正共同依赖被延后审计。

禁止：
- 第三次继续调 Offset、Gravity、ZIndex、Bias、TopMost、Anchor 等参数而不重审承载；
- A/B 两个组件同时异常时分别堆 workaround，不审 Shared Dependency；
- 在 NativeControlHost/Avalonia Airspace 问题上扩建新的长期 Native UI Owner。

正确做法：
1. 列出 UI/Input/Render/Host 的 Owner；
2. 画真实 Runtime Route；
3. 标明每个坐标的 Source/Target Space；
4. 检查 Airspace/Native HWND/Composition 边界；
5. 用一次只排除一个共同变量的实验重新验证前提，再决定是否继续局部修复。

来源 ERR：
- ERR-20260925-001
- ERR-20260925-002

任务注入：
涉及 Diagnostic、Viewport、NativeControlHost、Popup、Overlay、复杂 Rendering 或连续第二次修复失败时，在 Knowledge Preflight 中加载本规则。

验证 / 自动化：
当前已有 Viewport Legacy Allowlist、A1/A1.5 架构治理与相关回归；“连续两次修复失败自动停线”仍主要依赖任务治理，尚无通用机器 Gate。

Superseded by：
无

---

## EXP-TEST-001 平台与生产链测试必须从权威边界进入

状态：ACTIVE
适用范围：Input Adapter / Native / Avalonia / Runtime Wiring / Platform Contract / Production Path
触发条件：测试涉及平台消息、键值、Pointer、Adapter、Router、生产接线或以源码字符串/Helper 证明行为。
Occurrences：2

规则：
平台边界测试必须从真实平台 Enum / Message Contract 开始；生产链测试必须覆盖真实 Adapter → Unified Model → Router → Consumer。Helper PASS、源码 `Assert.Contains`、人工构造理想统一值只能作为 L1/L2 证据，不能单独证明 Production Runtime 行为。

根因模式：
测试输入复制了实现假设，导致错误实现和错误测试共同全绿；或测试绕过 Adapter/生产 Source，只证明内部 helper 自洽。

禁止：
- 用 Win32 `0x12` 冒充 Avalonia Alt 输入；
- 用错误平台常量构造测试再断言实现正确；
- 仅用 `Assert.Contains("message.IsAltDown", source)` 证明实际参数已贯穿运行链；
- Router/Consumer 可实例化就宣布生产接线完成。

正确做法：
- 平台事实 → Adapter → Editor Semantic Model；
- Editor Event → Production Router → Arbitration → Owner → Consumer；
- 对真机相关问题继续保留 Runtime UI / Real-machine 层级验收。

来源 ERR：
- ERR-20260925-001
- ERR-20260925-003
- ERR-20260925-004

任务注入：
涉及 Native/Avalonia Input、Adapter、Production Router、平台常量、Source Contract 或 Runtime Wiring 时必须加载本规则。

验证 / 自动化：
优先建立 `PlatformKeyNormalization`、`ProductionInputNoBypass`、`ProductionPathTestRule`；已有 ProductionInputComposition/E5 输入冻结测试作为基础，但不能替代所有真机输入验收。

Superseded by：
无

---

## EXP-UI-002 Diagnostic Observer Rule

状态：ACTIVE
适用范围：Diagnostic Mode / Element Probe / Overlay / Highlight / Native Viewport
触发条件：诊断系统观察 Pointer、显示 Highlight/Card、读取 Native HWND/Bounds 或接入 Viewport。
Occurrences：1

规则：
Diagnostic 的身份固定为 Observer。它可以观察、记录、格式化和可视化诊断信息，但不得改变被诊断对象的输入所有权、Capture、业务状态或 Window Ownership。

根因模式：
为了解决 Native/Avalonia 可见性、Placement 或点击跟踪，把 Diagnostic 逐步扩成第二套输入/窗口系统，最终诊断工具本身改变产品行为。

禁止：
- Diagnostic 抢 Pointer Capture；
- Diagnostic 将生产 Pointer 标记 handled 从而阻断 Viewport；
- Diagnostic 注册独立 Gesture Owner/业务 Consumer；
- 为覆盖 Native Viewport 持续扩建长期 Native UI Window/Popup 架构；
- 高频诊断同步阻塞 UI/Input/Render。

正确做法：
- Avalonia 使用 tunnel/observer 方式观察而不消费；
- Native 使用旁路 Probe 观察并保持 Production forwarding；
- Highlight 必须 input-transparent；
- Popup/Window 仅在已批准边界内使用，长期目标服从 Avalonia 唯一 UI Owner；
- Diagnostic 关闭后不得留下 Capture/Owner/临时状态。

来源 ERR：
- ERR-20260925-002

任务注入：
所有 Diagnostic、Element Probe、Viewport Highlight、Native Probe、点击跟踪任务必须加载本规则及 K-DIAG-001。

验证 / 自动化：
建立/强化 `DiagnosticInputTransparency` Runtime Gate；保留 Viewport passthrough、self-exclusion、click observation 不 handled 等回归。

Superseded by：
无

---

## EXP-GOVERNANCE-001 Repository Authority First

状态：ACTIVE
适用范围：SDK / Build / Run / Toolchain / Output Path / Acceptance Entry / 多机器开发环境
触发条件：Agent 需要判断当前机器工具链路径、启动入口、编译器、产物目录或“环境是否缺失”。
Occurrences：1

规则：
当前仓库中的权威入口和 Resolver 高于 Agent 历史记忆。事实优先级固定为：
`Repository Current Files → Current Machine Resolver → Git Current State → Agent Memory`。
记忆只能帮助找到权威入口，不能直接作为当前环境结论。

根因模式：
Agent 将另一台电脑或旧阶段的绝对路径当成当前事实，绕过仓库已经建立的多环境解析机制。

禁止：
- 只检查记忆中的 D:/E: 固定路径后宣布 NO SDK；
- 未读取 `run.bat` / Resolver 就判断 Build/Run 环境；
- 用历史输出目录替代当前启动脚本解析出的产物路径；
- 当前仓库规则与记忆冲突时优先相信记忆。

正确做法：
1. 读取 canonical run/build entry；
2. 执行项目 Resolver；
3. 记录 Resolver 实际选择的工具与版本；
4. 再核对当前 Git HEAD / branch / worktree；
5. Resolver 真实失败后才允许报告环境 BLOCKED。

来源 ERR：
- ERR-20260925-005

任务注入：
所有 Build、Run、SDK、Toolchain、环境诊断、交付验收任务必须加载本规则及 K-GOV-003。

验证 / 自动化：
候选 Gate：`CanonicalRunResolver`。玄域引擎当前权威链为 `run.bat → scripts/resolve-dotnet.ps1`。

Superseded by：
无
