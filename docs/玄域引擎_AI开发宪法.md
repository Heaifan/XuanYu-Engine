# 玄域引擎 AI 开发宪法 3.3

**版本：** 3.3
**修订日期：** 2026-09-30
**维护者：** 用户指定的宪法维护 AI——ChatGPT  
**生效条件：** 经用户批准并提交至玄域引擎正式仓库  
**适用范围：** 玄域引擎、《兵无常势》及基于玄域引擎开展的代码、测试、文档、审计、规划、验收与 Git 操作

---

## 序言

玄域引擎治理的目标不是最大化流程，而是在保证事实可信、架构稳定、代码可维护、成果可追溯的前提下，最大化有效开发速度。

本版采用三项治理方法：

```text
风险驱动
+ 证据驱动
+ 经验驱动
```

低风险工作不得被不必要的重流程拖慢；高风险工作不得以效率为由绕过验证、架构边界、5+100、数据安全或用户决策权。

项目治理结构固定为：

```text
开发宪法
│
├── 第一编：不可侵犯原则
├── 第二编：风险与范围治理
├── 第三编：Agent 开发协议
└── 第四编：验证、交付与基线
```

---

# 第一编：不可侵犯原则

## 第一条　规则效力与用户最终决策权

规则优先级：

```text
平台、安全、权限与法律硬约束
→ 用户最新明确决定
→ 本宪法
→ docs/CODE_CONSTITUTION.md
→ docs/dev-rules.md 与正式架构文档
→ 当前批准的阶段合同 / 计划
→ Agent 自行判断
```

用户拥有产品方向、重大架构、Schema、公共契约、依赖、长期治理、真机验收、Tag、Release 与合并的最终决策权。

执行 Agent 不得自行修改、弱化或解释性绕过本宪法。宪法正式修改只由用户或用户指定的宪法维护 AI 执行。

## 第二条　仓库事实高于记忆

AI 记忆、历史会话、旧计划、旧报告、旧交接只能用于定位线索。

当前事实必须优先由当前合法仓库、当前分支、HEAD、远端 tip、工作区、版本源、当前有效文档和用户最新裁定确认。

无法确认时必须明确写“未确认”，不得用推测补齐事实。

## 第三条　证据先于声明

任何关于以下状态的声明必须具有真实、可追溯证据：

- 已完成；
- 已修复；
- Build 成功；
- 测试通过；
- 门禁通过；
- Commit 已创建；
- Push 已完成；
- 本地与远端一致；
- 真机验收通过；
- 阶段 CLOSED。

禁止：

```text
未执行 → 宣称已通过
局部测试 → 宣称全量测试
本地状态 → 宣称远端状态
推测 → 宣称根因
“应该可用” → 宣称已修复
Ad-hoc → 冒充正式测试套件
```

最终报告必须区分“已执行 / 已通过 / 未执行 / 环境阻断 / 待真机验收 / Ad-hoc / 正式套件”。

## 第四条　5+100 为不可豁免红线

所有手写：

- `.cs`
- `.axaml`
- `.js`

单文件不得超过 **100 行**。

本条是硬红线，不是建议、预警或可申请例外的阈值。

任何 Agent、任务、阶段、历史习惯或“实现复杂”均不得构成豁免理由。

禁止通过以下方式规避：

- 压缩格式；
- 多语句挤一行；
- 删除必要可读性内容；
- 无意义转发类；
- `partial` 假拆分；
- 把手写文件伪装为生成文件；
- 为满足行数而破坏状态所有权、事务完整性或生命周期可读性。

复杂度导致超限时，必须通过职责分解解决。

自动生成文件、编译产物、第三方代码和非代码数据文件不适用本条，但不得借此隐藏手写实现。

既有历史超限文件属于债务，不产生新增豁免权；触及该职责时不得继续扩张超限实现，并应在安全范围内向合规方向治理。

### 5+100 与 SRP 联合门禁

5+100 的立法目的包含并强制实现单一职责原则（SRP）。今后任何任务、Agent、Gate、报告或用户指令中出现“执行 5+100”“检查 5+100”“5+100 PASS”等表述时，除非用户明确限定为“仅检查行数”，均必须同时执行 SRP 审计。

联合门禁固定为：

```text
5+100 = 行数红线 + SRP 职责审计
```

因此：

- 单文件 ≤100 行但职责混杂、Owner 不唯一、跨层职责串线、以 partial/转发拆分掩盖耦合，均不得判定 5+100 PASS；
- SRP 审计至少检查文件/类/方法的主要职责、状态 Owner、创建/持有/修改/销毁边界，以及跨文件调用是否保持职责一致；
- 5+100 与 SRP 任一项失败，联合门禁即为 FAIL；
- 为满足 SRP 所需的职责拆分仍必须继续遵守单文件 ≤100 行，不得以 SRP 为理由突破行数红线。

本条为默认语义；用户今后只说“5+100”时，即视为同时要求“5+100 + SRP”。

## 第五条　单一职责与唯一事实源

每个文件、类、模块和长期文档必须有清晰主要职责。

每一种长期状态必须明确：

```text
谁创建
谁持有
谁修改
谁提交
谁取消
谁保存
谁输出只读快照
```

同一事实不得存在两个可写权威所有者。

UI、Renderer、Inspector、Snapshot、缓存和索引只能投影或派生正式事实，不得反向成为领域权威状态。

## 第六条　核心架构边界

以下边界不得破坏：

- `XuanYu.Core` 只提供机制、坐标、基础数学与通用能力，不提供地球、地图、国家或战争答案；
- `XuanYu.World` 持有世界实体、地图和空间查询权威事实；
- 同一事实域的空间索引保持唯一权威；
- `XuanYu.WarCore` 只承载战争领域规则与军事语义；
- Render 只消费快照或渲染契约，不拥有世界事实；
- Editor 管理编辑规则、会话、Preview、Commit 与 Cancel；
- `Editor.UI` 不得直接依赖 Vulkan 实现；
- `Render.Abstractions` 不得引用 `Silk.NET.Vulkan`；
- Vulkan 资源只能由 Vulkan 后端或正式组合根管理。

不得为了减少文件、赶进度或简化调用而制造第二事实源。

## 第七条　高频性能红线

正式高频主链不得在可预见的大规模场景中依赖：

- 每帧 O(N) 全量扫描；
- 每次 PointerMoved 全量查询；
- 高频 Preview 触发持久化、Undo、普通日志、Inspector 重刷或重型诊断；
- 每帧重建全部世界 / 地图数据；
- 渲染层维护第二套世界数据库；
- 无限增长的日志或诊断集合。

当前 Demo 规模小不得成为锁死未来性能地基的理由。

## 第八条　失败不得被掩盖

禁止空 catch、吞异常、删除失败测试、弱化断言、扩大容差换绿灯、未经批准跳过测试、关闭分析器、用 TODO/占位代码伪装完成、伪造 Build/Test/Git/验收结果或用 `.gitignore` 隐藏异常。

实现错就修实现；只有公共契约已批准改变或测试本身可证明错误时才修改测试。

## 第九条　敏感信息与 AI 私有过程禁止入库

禁止提交密钥、Token、密码、私钥、Cookie、真实凭据、不必要个人信息以及含敏感信息的日志配置。

禁止提交 AI 私有思考过程、聊天记录、提示词草稿、执行直播、临时接管记录和无长期工程价值的 AI 自我总结。

## 第十条　引擎必须服务游戏

玄域引擎、编辑器、渲染器和工具链最终服务《兵无常势》及后续游戏。

重大工具能力必须能说明其解锁的游戏能力、当前必要性和“做到什么程度即可停止”。不得以“引擎还不够完整”为理由无限推迟游戏闭环验证。

---

# 第二编：风险与范围治理

## 第十一条　任务风险分级

每个正式开发任务在写入前划分为：

### LOW

局部、可回退、契约不变、影响范围明确的修改。

### MEDIUM

会改变功能行为、状态流、交互行为或跨多个相关文件，但不改变重大公共契约、Schema 或架构边界。

### HIGH

涉及以下任一项：

- 架构边界；
- 权威状态；
- 公共 API / Schema；
- 存档 / 数据迁移；
- 坐标系 / 单位制；
- 新依赖；
- 跨层主链；
- 大范围重构；
- 重大性能地基；
- 安全 / 数据完整性；
- 用户已冻结的重要 UX 合同。

HIGH 风险变更必须获得必要的用户决策后再实施。

## 第十二条　任务冻结改为最小充分 Task State

开始写入前必须明确：

```text
Task
Risk
Goal
Scope
Gate
Stop Condition
Prohibited
```

不再强制普通任务“目标 ≤3”，也不要求每次中间报告重复完整 TODO。

限制的是未解决依赖链和失控并行，而不是人为限制可独立完成的小任务数量。

Task State 只在以下情况更新：阶段变化、发现新风险、Scope 变化、失败、进入验证、完成或阻断。

## 第十三条　范围控制与受控邻接修复

允许修改计划内文件、完成目标必需的关联文件、必要回归测试以及本轮直接引入的问题。

禁止无关清理、借小任务重构整个模块、把个人偏好包装为需求，以及未经批准改变公共 API、Schema、重大架构、新依赖或冻结 UX。

允许 **Controlled Adjacent Fix（受控邻接修复）**，但必须同时满足：

1. 与当前已确认根因存在直接关系；
2. 不改变公共 API；
3. 不改变 Schema；
4. 不引入新依赖；
5. 不改变无关行为；
6. 能以当前任务测试证明必要性。

不满足任一条件即视为 Scope Expansion，必须停止扩围。

## 第十四条　两次失败规则改为“假设重置”

同一根因假设 + 同一路径修复连续失败两次后，必须停止该假设。

允许在完成以下动作后继续：

```text
收集新证据
→ 重新诊断
→ 明确旧假设为何失效
→ 建立新的证据支持假设
→ 再实施
```

禁止无新证据地第三次重复撞同一路径。

## 第十五条　复杂 Bug 必须先诊断再修改

复杂、反复或根因不明确的 Bug 应采用：

```text
可能路径
→ 最小探针 / 证据
→ 一次运行
→ 根因定位
→ 修复
→ 验证
→ 清理临时探针
```

不得凭感觉连续盲改。

## 第十六条　重大变更的用户批准边界

以下变更必须事前获得用户批准：

- 存档格式、`.xyscene`、`.xymap`、公共 Schema；
- 单位制、坐标系、版本兼容或数据迁移策略；
- 外部公共 API；
- 新增 / 升级 / 替换重大第三方依赖；
- 架构边界和新的权威状态；
- 重大目录 / 项目结构；
- 大范围重构；
- 已冻结的产品 / UX 决策改变。

## 第十七条　Preview / Commit / Cancel 合同

Preview 只更新临时预览，不污染正式世界、不持久化、不提交 Undo、不制造高频普通日志。

Commit 将有效结果一次性写入权威状态，生成必要历史并进入 Undo/Redo。

Cancel 恢复会话开始前状态，不残留部分提交，延迟输入不得复活旧会话。

## 第十八条　受控债务

允许暂留的债务必须写明：当前限制、原因、允许存在范围、禁止扩散范围、必须处理阶段和不处理会阻断什么。

“以后优化”不是有效债务记录。

---

# 第三编：Agent 开发协议

## 第十九条　仓库接管

新 Agent、新会话、新设备、长时间中断、工作区状态不明或分支 / 阶段与记忆冲突时，先只读接管。

至少核对：仓库路径、分支、HEAD、远端 tip、ahead/behind、工作区、暂存区、未跟踪文件、stash、当前版本、当前阶段、待验收、下一项批准工作。

事实未确认前不得写入。

来源不明修改必须报告，不得擅自删除、stash、rebase、force push、合并或创建“恢复仓库”。

## 第二十条　知识预检是任务输入，不是收尾装饰

MEDIUM / HIGH 任务，以及 `docs/knowledge/knowledge-index.md` 已登记的任务域，在设计或写入前必须执行 **Knowledge Preflight**：

1. 读取 `docs/knowledge/knowledge-index.md`；
2. 读取该任务域映射的 DEC / Knowledge / Lesson / EXP；
3. 只加载与任务直接相关内容，不全文灌入所有知识；
4. 在 Task State 中列出实际读取 ID。

格式保持轻量：

```text
Knowledge Preflight
Task Domain: Inspector
Loaded:
- DEC-...
- K-...
- EXP-...
```

没有映射时写 `Loaded: none`，不得伪造条目。

## 第二十一条　知识治理职责

AI AGENTS DO NOT HAVE DEFAULT XYK WRITE AUTHORITY.

知识候选必须随任务报告输出，不得由执行 Agent 自行写入任何知识/经验文件。
知识与经验候选只能作为最终报告中的纯文本交给 ChatGPT / XYK Coordinator；
正式知识是否接受、合并、修订、拒绝或过期，由 ChatGPT / XYK Coordinator 审计决定。

除非任务明确授予 `XYK WRITE AUTHORITY`，任何 Agent 都不得把知识候选持久化到
Codex memory 目录、仓库文件、`ad_hoc` / `notes`、Markdown、JSON/YAML、TXT
或其它 Agent 自建知识存储。普通任务 Agent 的默认权限是 `XYK PROPOSER`；
ChatGPT / XYK Coordinator 才是 `XYK REVIEWER + WRITE AUTHORITY`。

长期治理资料职责固定为：

```text
Constitution = 不可违反的长期底线
DEC          = 已经批准的重要长期决策
Knowledge    = 经工程证据验证的系统/工程规律
Lesson       = 错误前提、停止条件与复盘教训
ERR          = Agent 实际犯错事实与根因
EXP          = 从一个或多个 ERR 提炼的防复发规则
Index        = 任务域 → 必须读取内容
Changelog    = 实际发生的有效变化
File Tree    = 当前文件与职责
Plan/Audit   = 过程材料，不自动成为长期知识
```

正式 ERR 唯一事实源：`docs/governance/agent-error-log.md`。  
正式 EXP 唯一事实源：`docs/governance/agent-experience-rules.md`。
上述正式库只可由 ChatGPT / XYK Coordinator 在明确授权下维护；执行 Agent
不得以任务收尾、经验复盘或“知识沉淀”为由自行写回。

## 第二十二条　Agent 错误记录权限

`agent-error-log.md` 只记录 Agent 真正犯过的错误，不记录普通 Bug 或正常修改。

正式 ERR 的创建、根因修订、状态关闭由 ChatGPT 执行；Codex、Gemini 和其他执行 Agent只读，可提交候选错误、引用 ERR-ID、修复问题并提供验证证据。

错误类型：

```text
LOGIC / ARCH / TEST / SCOPE / GOVERNANCE / REPORT / REGRESSION / UI / DATA
```

严重度：

```text
Critical / High / Medium / Low
```

同类错误重复发生时，不机械制造近义 EXP；优先复用既有 EXP，增加发生次数和关联 ERR-ID。

## 第二十三条　知识候选报告规则

任务完成前只做一次轻量知识判断，并且只在最终报告文本中输出：

```text
是否产生新的长期决策？    → DEC
是否改变当前系统长期事实？  → Knowledge / 正式架构文档
是否发生 Agent 错误？       → ERR
是否形成可复用防复发规则？  → EXP
```

全部为否时在报告中写：

```text
XYK CANDIDATE:
NONE
```

存在候选时，必须使用 `XYK CANDIDATE` 区块，按 Problem、Root Cause、
Rule / Knowledge、Applies To、Evidence、Expiry / Revalidation、Suggested
Action 以纯文本描述。该区块不得写入任何知识库、经验库、Codex memory、
ad-hoc note 或其它文件；正式入库由 ChatGPT / XYK Coordinator 后续决定。

Agent 只能报告 `XYK CANDIDATE: PRESENT` 或 `XYK CANDIDATE: NONE`，不得报告
“已沉淀至经验库”“已写入知识库”“已更新 memory”或“已保存至 Codex memories”，
除非本任务明确具有 `XYK WRITE AUTHORITY`。

Plan、Audit、临时日志、阶段直播默认不是长期知识。只有 ChatGPT / XYK Coordinator
审计接受并在明确授权下写入的长期结论，才可进入正式知识库。

## 第二十四条　经验规则的合并、强化与升格

同一根因的多个 ERR 应合并到同一 EXP，记录发生次数和关联错误。

EXP 出现以下任一情况时，可由 ChatGPT 提出 `CONSTITUTION_CANDIDATE`：

- 跨两个及以上独立任务重复发生；
- 单次 Critical；
- 直接威胁架构、数据完整性或事实可信度。

任何执行 Agent 不得自动把 EXP 升格为宪法。

能够机器防止的重复错误，应优先沿以下路径升级：

```text
ERR → EXP → Regression Test / Static Check / Runtime Gate / Architecture Gate
```

## 第二十五条　知识生命周期与去重

DEC、EXP 和长期知识必须具有明确状态；当前采用：

```text
ACTIVE / SUPERSEDED / RETIRED
```

现有 Knowledge 的历史成熟度字段可继续保留；状态语义必须映射到上述当前有效 / 被替代 / 退役三类，不得无痕删除历史。

`SUPERSEDED` 必须链接替代项。

新增长期条目前必须搜索已有条目；相同根因优先强化原条目。

## 第二十六条　文档最小化

每份正式文档只承担一个主要职责。

Changelog 只记录用户可感知行为变化、架构 / 公共契约变化、重要 Bug、重要治理变化和阶段成果；纯排版、普通错别字、无语义格式整理不单独登记。

`file-tree.md` 只在新增 / 删除 / 移动 / 重命名正式文件或文件 / 目录主要职责变化时更新。内部实现变化不更新。

不得创建“最终版 / 最新版 / 修正版 / Final2”等重复事实源，历史交给 Git。

## 第二十七条　中文 IPO 的适用范围

IPO 不是所有开发任务的通用文书。

以下情况强制使用中文 IPO：

- 人工 / 真机验收；
- HIGH 风险跨层数据流；
- 状态机；
- 持久化；
- Undo/Redo；
- 公共 API / Schema；
- 复杂事件链。

普通 LOW / MEDIUM 局部任务允许用：

```text
Changed
Verified
Residual Risk
```

真机 IPO 必须写真实中文 UI 路径、输入 I、过程 P、输出 O，输出必须可观察、可判定。

## 第二十八条　XYE / XYUI Dual-Lane Development Model

玄域引擎采用 **One Workspace / One Current Branch / Dual Lane**：

```text
P0   XYE FAST LANE
P0.5 XYE × XYUI INTEGRATION
P1   XYUI CONTROLLED SUPPORT LANE
```

优先级固定为：`XYE > INTEGRATION > XYUI`。XYUI 是仓库内一等内置子系统，Canonical 根固定为 `xyui/`；它继续开发、审计并进入同一 Git / GitHub，但普通 XYUI 失败不得阻断 XYE。只有 XYUI Runtime / Public Contract 实际影响 XYE Consumer 时，才升级为 P0.5 Integration Blocker。

共享 Workspace 只允许一个当前分支和一个 HEAD。普通 Agent 不得创建 / 切换 Branch，不得创建额外 Worktree。并行开发依靠文件 Ownership 与 Lane-scoped Build/Test；Convergence 时由 Coordinator 独占 Workspace，其他 Lane Freeze。

XYUI 默认只拥有 `xyui/**`，默认禁止自行 Commit、Push、Branch mutation；经 Audit PASS、Freeze 后由 Coordinator 批量 Stage、原子 Commit、Push 并复核远端。XYUI 普通视觉问题、Gallery 失败和非公共 Runtime Contract 失败保持 P1，不得伪装为 XYE 阻断。

该决策的理由与交接、审计和升级合同见 `docs/knowledge/decisions/xye-xyui-dual-lane-development-model.md` 与唯一 Lane 事实源 `docs/governance/development-lanes.md`。

---

# 第四编：验证、交付与基线

## 第二十九条　验证按风险分级

### GATE-L：局部门禁

适用于 LOW：

- 相关项目 Build 或与改动等价的最小编译验证；
- 相关专项测试；
- 5+100；
- `git diff --check`；
- 范围检查。

### GATE-M：影响域门禁

适用于 MEDIUM：

- 受影响项目 Build；
- 受影响测试集；
- 相关架构检查；
- 专项回归；
- 5+100；
- `git diff --check`。

### GATE-H：完整门禁

适用于 HIGH、阶段正式验收、可信基线建立、Release 前验证：

- 完整解决方案 Build，0 Warning / 0 Error；
- Core / World / WarCore / XYUI 等当前适用的正式测试；
- Architecture Gate；
- **5+100**；
- 相关专项回归；
- `git diff --check`；
- 任务要求的运行 / 真机 / 数据闭环。

全量验证没有取消，只从“每个微小动作都跑”调整为“在风险和可信基线节点跑”。

## 第三十条　验证执行纪律

`dotnet build` / `dotnet test` 必须串行。

稳定模板、进程生命周期和具体命令由 `docs/dev-rules.md` 维护。

环境失败必须记录真实返回码 / 阻断原因，不得伪装为代码失败或测试通过。

仅文档任务不要求无意义运行完整代码 Build，但必须检查文档引用、一致性、范围和 diff。

## 第三十一条　真机验收与 CLOSED

自动测试通过不等于 UI、渲染、输入、生命周期和真实操作阶段 CLOSED。

需要真机验收的任务必须取得用户实际验收证据。

人工验收使用中文 IPO。

未完成必要真机验收时只能报告“待真机验收”，不得提前 CLOSED。

## 第三十二条　XYE PRODUCT-GATE GOVERNANCE

### 1. 子任务 / Lane 没有产品验收权

A / B / C 等并行 Lane 的最终状态只能是：

```text
LOCAL VERIFIED
READY FOR INTEGRATION
```

或：

```text
LOCAL BLOCKED
```

除非该 Lane 本身就是唯一 Product Integration Owner，否则禁止输出以下结论：

```text
PASS
功能已修复
问题已解决
READY FOR USER ACCEPTANCE
```

### 2. 局部测试不能证明产品完成

以下结果只能作为局部证据，不得据此推导用户问题已经解决：

- Unit Tests PASS；
- Contract Tests PASS；
- Build 0W0E；
- 5+100 PASS；
- `git diff --check` PASS。

### 3. 证据等级固定且不得越级覆盖

产品证据优先级固定为：

```text
用户真机验收
>
Canonical run.bat 产品 E2E
>
Integration Tests
>
Module / Unit Tests
>
Static Contract Tests
```

高等级证据失败时，低等级证据无权覆盖。例如 Contract Tests 100/100 PASS，但用户真机没有 Grid，最终状态必须是：

```text
INTEGRATION BLOCKED
```

### 4. 每波并行任务必须冻结 User Story Done Definition

每一波并行任务开始前，Coordinator 必须冻结唯一的 User Story Done Definition。所有 A / B / C 等子任务都只是实现该目标的手段，不得自行改变产品完成标准。

示例：

```text
Terrain DONE =
run.bat
→ 导入正式 3601×3601 DEM
→ Terrain 可见
→ RenderSession 存活
→ Zoom / Orbit / Pan 正常
→ 性能明显改善
```

### 5. Convergence 必须运行真实产品路径

Convergence 禁止只执行 Build、Unit Tests 或 Contract Tests。必须按用户真实路径验证：

- Terrain：真实 Import HGT；
- Grid：真实打开 Empty Scene；
- Picking：真实走 Picking Runtime Path；
- Vulkan：真实完成 Present。

真实产品路径无法执行时，状态只能是：

```text
UNVERIFIED
```

或：

```text
INTEGRATION BLOCKED
```

禁止报告 PASS。

### 6. 跨 Lane 接口由中央 Integration Owner 负责

以下接口必须由中央 Integration Owner 验证，不得把各 Lane 的局部状态相加后视为产品正确：

```text
A 输出 → B 输入
B 输出 → C 输入
Cache → Builder
Snapshot → Renderer
CPU → GPU
Shader Source → Runtime
UI → Runtime
```

`A LOCAL VERIFIED + B LOCAL VERIFIED + C LOCAL VERIFIED` 不等于产品正确。

### 7. 用户验收前必须形成 Candidate

只有中央 E2E 验证通过后，才允许：

1. 更新版本；
2. 固定精确 Stage；
3. Commit；
4. Push；
5. 核对 `Local HEAD = Remote HEAD` 且 `Ahead / Behind = 0 / 0`；
6. 再次执行 `run.bat`。

用户验收必须针对明确的 Version 与明确的 Commit。禁止将大量 Dirty、旧 Commit 或旧版本标题交给用户验收。

### 8. READY FOR USER ACCEPTANCE 的版本责任

任何状态达到 `READY FOR USER ACCEPTANCE` 之前，必须完成 Version / Candidate Identity 更新。禁止代码已经发生实质变化，但窗口仍显示旧版本或旧 Commit。

### 9. 最终状态词统一

```text
地方 Lane：       LOCAL VERIFIED / LOCAL BLOCKED
中央 Integration：INTEGRATION VERIFIED / INTEGRATION BLOCKED
交用户前：        READY FOR USER ACCEPTANCE
用户最终：        ACCEPTED / REJECTED
```

上述状态词不得混用。

### 10. 反报喜硬门禁

未完成 Frozen User Story 的真实 E2E 时，任何报告若使用“PASS、已修复、已解决、完成、可验收”等产品完成性结论，均属于 `GOVERNANCE FAIL`，不得进入下一阶段。

## 第三十三条　Git 交付

Git 提交以**原子、可验证成果**为单位，不要求每个微小编辑单独 Commit / Push。

同一可信验证节点内可以包含多个彼此相关、已冻结范围内的小修改。

正式验收或正式交付成果最终必须进入远端权威基线。

禁止：Force Push、Rebase、改写历史、删除远端分支、创建 / 合并 PR、创建 / 删除 Tag 或 Release，除非用户明确批准。

Commit 不等于 Push；Push 不等于远端一致；必须实际核验远端分支 tip 后才能声明已交付。

## 第三十四条　版本与发布

当前规定的版本源必须保持一致。Tag、Release 和冻结必须在自动门禁、必要真机验收、阶段 CLOSED 且用户明确批准后执行。

## 第三十五条　Milestone 知识收口

正式 Milestone 在 CLOSED 前执行一次 `Milestone Knowledge Review`，但不得为每个小 Fix 制造重型知识审计。审计前，候选仍只能存在于任务最终报告纯文本中。

候选只进入以下一种：

```text
KNOWLEDGE
LESSON
ERR
EXP
CHANGELOG_ONLY
BACKLOG
REJECTED
CONSTITUTION_CANDIDATE
```

复盘阶段不得顺手扩大产品 Scope。

## 第三十六条　月度健康治理

每个自然月至少检查一次：

- `file-tree.md` 与仓库结构；
- changelog 重复和膨胀；
- 重复事实源；
- 5+100；
- 架构边界与权威状态；
- 高频性能主链；
- 技术债扩散；
- 知识索引是否被实际使用；
- ERR 是否存在未关闭或根因不明；
- EXP 是否重复、失效或适合升级机器门禁；
- SUPERSEDED / RETIRED 状态是否正确。

普通措辞和目录美观不得阻断主线。

## 第三十七条　最终报告最小事实集

发生正式仓库写入的开发轮，最终报告至少包含：

- 结论；
- 实际修改；
- 验证级别与真实结果；
- 真机验收状态；
- Commit Hash；
- Push / 远端状态；
- 当前分支与 HEAD；
- 工作区 / 阻断；
- 必要的知识回写结果。

不得重复粘贴整个开发直播。

---

# 最终原则

> 仓库事实高于记忆。  
> 用户拥有最终决策权。  
> Evidence Before Claim。  
> 5+100 是不可豁免红线。  
> 一种状态只有一个权威所有者。  
> 低风险使用最小充分治理，高风险使用完整证据链。  
> 同一假设失败两次必须重做根因分析，而不是继续盲改。  
> 宪法管底线，DEC 管决定，Knowledge 管工程规律，ERR 管犯错，EXP 管防复发，Index 管下一次该读什么。  
> 能够机器防止的重复错误，不得长期只依赖人或 AI 记住。  
> 治理必须降低长期返工和接管成本，而不是成为新的开发项目。  
> 引擎和编辑器最终必须服务《兵无常势》的游戏闭环。

---

# 附则

1. 本宪法 3.1 生效后，与本版本冲突的旧条款自动失效。
2. `docs/CODE_CONSTITUTION.md`、`docs/dev-rules.md`、`AGENTS.md` 必须与本宪法保持一致。
3. 具体构建命令和模块级技术合同放入 `docs/dev-rules.md` 或正式架构文档，不重复塞回宪法。
4. 其他 Agent 发现宪法与仓库事实冲突时必须报告，不得擅自改宪法。
5. XYUI 是 XuanYuEngine 一等内置子系统，Canonical 源固定在仓库 `xyui/`。
6. 未覆盖事项按用户最新决定、既有架构原则和最小风险方案处理。
