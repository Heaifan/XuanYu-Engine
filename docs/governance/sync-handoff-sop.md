# 玄域引擎同步与交接规范 v1.1

**XuanYu Engine Sync & Handoff Protocol**

状态：正式规范  
适用对象：ChatGPT、Codex、Gemini 及其他参与玄域引擎开发的 AI Agent  
适用场景：换电脑、换聊天、换 Agent、长时间中断后恢复开发、多人/多 Agent 交接

> v1.1 核心修订：正式确立 **Single Canonical Workspace + Sequential Handoff**。普通开发与 Agent 交接不再默认创建 Codex/Gemini/Integration 长期 worktree；同一功能链默认顺序写入唯一正式工作区。任何与本规则冲突的旧 Prompt、旧任务书、Agent 习惯或临时方案，以本 SOP 为准。

---

# 1. 目标

玄域引擎开发不得依赖某一台电脑、某一个聊天窗口或某一个 Agent 的临时记忆。

任何开发终端都必须能够依靠：

- GitHub 正式代码基线
- 仓库内开发规范
- 当前交接快照
- 产品/架构决策记录
- 最新开发进度

恢复完整开发上下文。

同时，玄域正式成果不得散落在多个长期 XYengine 副本中。

每台开发电脑在同一时刻只能存在一个用户可见的 **Canonical Workspace / 唯一正式工作区** 作为正式开发、验证、运行和交接基线。

用户在新电脑或新聊天中只需要输入：

> 请你开始同步吧

AI 即进入标准同步接管程序。

同步完成前不得开始新的产品开发。

---

# 2. 总原则

玄域交接必须遵守：

```text
GitHub
=
代码事实源

Canonical Workspace
=
当前电脑唯一正式施工与运行工作区

Current Handoff
=
当前状态事实源

Decision Log
=
已批准产品/架构决策事实源

Development Workflow
=
工作方式事实源

Development Constitution
=
最高开发约束
```

聊天记录不得作为唯一事实源。

本地电脑不得作为唯一事实源。

Agent 自己的记忆不得作为事实源。

临时 worktree、临时 branch、Agent 私有目录不得成为正式成果事实源。

---

# 3. Single Canonical Workspace · 唯一正式工作区

## 3.1 定义

Canonical Workspace 是当前电脑上唯一允许作为以下行为正式基线的 XuanYu Engine 工作区：

- 正式代码修改
- 完整 Build / Tests / ARCH-A / 5+100 等门禁
- 正式 commit / push
- `run.bat` 启动
- 用户真机验收
- Agent 顺序交接
- Current Handoff 更新

本机绝对路径可因电脑而变化，因此 SOP 不硬编码 `C:\ / D:\ / E:\`。

每次 SYNC 必须先发现并报告真实 Canonical Workspace。

## 3.2 禁止长期副本

普通开发不得创建或长期保留：

```text
XuanyuEngine-CODEX-*
XuanyuEngine-GEMINI-*
XuanyuEngine-INTEGRATION-*
XuanyuEngine-ACCEPTANCE
XuanyuEngine-AREA-*
_tmp-xuanyu-*
```

或其他承担“第二正式 XYengine”角色的目录。

不得为了 Agent 分工方便，把每个 Owner 都变成一个长期 worktree。

## 3.3 run.bat

`run.bat` 必须始终直接运行 Canonical Workspace 的最新正式成果。

不得把用户验收导向临时 worktree、Integration worktree 或旧构建物。

---

# 4. 标准生命周期

玄域完整开发循环统一定义为：

```text
SYNC
↓
UNDERSTAND
↓
DECIDE
↓
TASK FREEZE
↓
DEVELOP
↓
VERIFY
↓
COMMIT / PUSH
↓
USER ACCEPTANCE
↓
HANDOFF
```

其中：

## SYNC

确认 Canonical Workspace 与远端一致。

## UNDERSTAND

恢复工作流、当前阶段、关键决策和上一轮进度。

## DECIDE

确认用户目标以及产品/交互/架构决定。

## TASK FREEZE

冻结本轮允许修改的范围、Owner、写入顺序和交接点。

## DEVELOP

Codex / Gemini 按 Owner 责任域执行。

## VERIFY

在具备正式环境的 Canonical Workspace 运行 Build、Tests、ARCH-A、5+100 等门禁。

## COMMIT / PUSH

形成可追踪 Git 基线。

## USER ACCEPTANCE

由用户执行真机、视觉、交互验收。

## HANDOFF

把当前状态写回仓库，允许下一 Agent、下一聊天或下一电脑无损接管。

---

# 5. 仓库标准文件

仓库应具备以下结构：

```text
/
├─ AI-BOOTSTRAP.md
│
├─ docs/
│  └─ governance/
│     ├─ development-constitution.md
│     ├─ development-workflow.md
│     ├─ sync-handoff-sop.md
│     ├─ current-handoff.md
│     └─ decision-log.md
│
├─ skills/
│  └─ xuanyu-sync/
│     └─ SKILL.md
│
├─ scripts/
│  ├─ handoff-start.ps1
│  └─ handoff-close.ps1
│
├─ changelog.md
└─ file-tree.md
```

文件职责严格区分。

`sync-handoff-sop.md` 是同步、Owner 切换、工作区与交接程序的唯一正式真源。

Skill、Agent Prompt、聊天任务书和脚本不得另外复制一套冲突规则。

---

# 6. AI-BOOTSTRAP

任何新 AI 接管玄域时必须先：

1. 定位 XuanYu Engine Repository。
2. 确认 Canonical Workspace。
3. 阅读 `docs/governance/sync-handoff-sop.md`。
4. 完成 Git Safety Audit。
5. 阅读 Development Constitution / Workflow / Current Handoff / Decision Log。
6. 按需读取 CHANGELOG / file-tree。
7. 输出 XUANYU HANDOFF ACK。
8. 满足全部接管门禁后才允许输出 `SYNC READY`。

在 `SYNC READY` 前：

- 禁止修改业务代码
- 禁止创建新功能
- 禁止启动新的 Codex / Gemini 正式施工
- 禁止自行推断当前开发阶段
- 禁止根据旧聊天直接继续施工

---

# 7. SYNC-01 · Repository Discovery

首先确定真实仓库和唯一正式工作区。

确认：

```text
Repository Root
Canonical Workspace Physical Path
Git Remote
Current Branch
```

必须执行：

```text
git worktree list
```

如果发现多个 XuanYu Engine worktree，必须分类：

```text
CANONICAL
TEMPORARY
UNKNOWN
```

未知 worktree 不得自动删除，也不得自动作为正式开发入口。

正式规范和任务书优先使用 repo-relative path；本机绝对路径只用于本次事实审计。

---

# 8. SYNC-02 · Git Safety Audit

在任何同步或开发操作之前检查：

```text
Current Branch
Local HEAD
Working Tree
Remote
Origin Branch
Origin HEAD
Ahead
Behind
```

必须先 fetch，再判断状态。

禁止只看本地 Git 状态后直接开始开发。

---

# 9. Git 状态分类

## CASE A — ALIGNED

```text
Working Tree = CLEAN
Ahead = 0
Behind = 0
```

结果：

```text
BASELINE ALIGNED
```

## CASE B — SAFE BEHIND

```text
Working Tree = CLEAN
Ahead = 0
Behind > 0
```

允许安全 Fast-Forward 同步。

同步后必须重新满足：

```text
Ahead = 0
Behind = 0
Working Tree = CLEAN
```

## CASE C — DIRTY

```text
Working Tree = DIRTY
```

立即：

```text
SYNC BLOCKED
```

必须报告修改文件、未跟踪文件、branch、HEAD。

禁止擅自：

```text
git reset
git clean
git stash
git checkout -- .
覆盖文件
删除文件
```

## CASE D — LOCAL AHEAD

```text
Ahead > 0
Behind = 0
```

状态：

```text
SYNC BLOCKED
```

必须先确认本地提交来源和合法性，不得因为“看起来正常”就自行 push。

## CASE E — DIVERGED

```text
Ahead > 0
Behind > 0
```

定义为：

```text
DIVERGED
SYNC BLOCKED
```

禁止自动：

```text
merge
rebase
force push
reset
cherry-pick
```

必须先进行正式 Git 基线治理。

---

# 10. SYNC-03 · Code Baseline

Git 对齐以后必须明确记录：

```text
Repository
Canonical Workspace
Branch
HEAD
Origin HEAD
Version
Working Tree
Ahead / Behind
Other Worktrees
```

禁止使用“应该同步好了”“好像是最新版”等模糊表述。

Git 状态必须使用真实 SHA 和数值。

---

# 11. SYNC-04 · Context Recovery

代码同步完成后不得立即开发。

按顺序恢复：

1. Development Constitution：绝对不能做什么。
2. Development Workflow：当前正式流程。
3. Current Handoff：当前阶段、进行中任务、阻塞、NEXT、Known Good Baseline、Agent 状态、用户验收状态。
4. Decision Log：仍有效的产品/架构批准决策。
5. CHANGELOG：最近及当前任务相关记录。
6. file-tree：模块位置、文件职责、责任域。

禁止 Agent 因个人偏好覆盖已有批准决策。

---

# 12. SYNC-05 · Execution State Recovery

Current Handoff 应明确：

```text
CODEX:
IDLE / ACTIVE / TECH COMPLETE / BLOCKED

GEMINI:
IDLE / ACTIVE / TECH COMPLETE / BLOCKED
```

如 Active，还必须记录：

```text
Task ID
Owner
Write Scope
Current Status
Last Commit
Next Action
Canonical Workspace
```

新 Agent 不得重复启动已经存在的同一任务。

---

# 13. 统一任务状态

任务使用：

```text
PLANNED
IMPLEMENTING
TECH COMPLETE
USER ACCEPTED
BLOCKED
SUPERSEDED
```

`TECH COMPLETE` 必须同时意味着：

- 正式代码位于 Canonical Workspace / 正式远端基线
- 适用 Build / Tests / 治理门禁已执行
- 合法 commit + push 已完成
- Working Tree / Ahead / Behind 已明确

临时 worktree 中“代码写完但正式环境没测”的状态不得标记 TECH COMPLETE。

---

# 14. 技术完成与用户验收分离

必须分别记录：

```text
Technical Status
User Acceptance Status
```

Build PASS、Tests PASS、自动截图、UI 自动化都不能替代用户真机视觉/交互验收。

只有用户明确批准时才能标记：

```text
USER ACCEPTED
```

---

# 15. SYNC-06 · XUANYU HANDOFF ACK

接管摘要固定包含：

```text
【Git 基线】
Repository:
Canonical Workspace:
Branch:
HEAD / Origin:
Working Tree:
Ahead / Behind:
Version:
Other Worktrees:

【当前阶段】
...

【最近完成】
...

【当前正在进行】
...

【最新关键决策】
...

【当前用户验收状态】
...

【Codex 状态】
...

【Gemini 状态】
...

【NEXT】
...

【我确认的工作流】
...
```

只有在 Git baseline 对齐、工作区身份明确、当前阶段和 NEXT 已恢复、Agent 状态已恢复且不存在未处理同步冲突时，才能输出：

```text
SYNC READY
```

---

# 16. SYNC READY 是硬门禁

`SYNC READY` 之前禁止：

```text
业务代码修改
UI 实装
新增功能
任务扩围
正式 Agent 施工
```

允许：

```text
读取
审计
Git 安全同步
恢复状态
输出报告
```

---

# 17. Development Workflow

玄域正式开发统一采用：

```text
USER INTENT
↓
APPROVED DECISION
↓
TASK FREEZE
↓
OWNER + WRITE ORDER
↓
IMPLEMENTATION UNDERSTANDING
↓
DEVELOP
↓
VERIFY
↓
COMMIT / PUSH
↓
HANDOFF GATE
↓
NEXT OWNER（如有）
↓
USER ACCEPTANCE
↓
HANDOFF
```

当同一功能链存在多个 Owner 时，`NEXT OWNER` 不得绕过前一 Owner 的 HANDOFF GATE。

---

# 18. USER INTENT

每项任务必须首先用普通中文说明用户真正要解决的问题。

禁止把技术实现方案伪装成用户目标。

---

# 19. APPROVED DECISION

模式划分、导航结构、顶层入口、工具归属、对象分类、保存行为、Inspector 分类、用户操作流程、产品术语、核心架构边界等变化必须来自用户已批准决策。

缺失必要决策时：

```text
DECISION REQUIRED
```

并停止相关产品层扩展。

---

# 20. TASK FREEZE

正式开发任务必须包含：

```text
USER INTENT
APPROVED UX / ARCHITECTURE
OWNER
WRITE ORDER
WRITE SCOPE
READ SCOPE
FORBIDDEN SCOPE
DELIVERABLES
ACCEPTANCE
GATES
HANDOFF TARGET
```

禁止 Opportunistic Refactor：

```text
顺手重构
顺手改名
顺手修其他模块
顺手改变交互
顺手引入依赖
```

---

# 21. OWNER 制度

Codex 与 Gemini 均可成为独立 Owner。

禁止默认形成：

```text
Gemini 写 → Codex 修
Codex 写 → Gemini 重做
```

每个 Owner 对自己的实现、测试、门禁、Git 和报告负责到底。

但 **Independent Ownership 不等于 Independent Workspace**。

Owner 独立指责任边界独立，不代表默认创建一套独立 XuanYuEngine 工作区。

---

# 22. Single Writer Rule · 单写入者规则

同一个 XuanYu Engine Repository 的普通开发默认采用：

```text
ONE CANONICAL WORKSPACE
+
ONE ACTIVE WRITER
```

同一时刻只有一个 Owner 可以对 Canonical Workspace 进行正式写入。

其他 Agent 可以并行执行：

- 只读审计
- 方案分析
- 文件定位
- 测试设计
- UI 原型评审
- 不产生仓库修改的研究

其他 Agent 不得在前一 Owner 未完成 HANDOFF GATE 时自行开始正式写代码。

---

# 23. Sequential Owner Handoff · 顺序 Owner 交接

同一功能链或存在接口依赖的任务必须默认顺序施工。

标准方式：

```text
Owner A
↓
Canonical Workspace 开发
↓
VERIFY
↓
COMMIT + PUSH
↓
HEAD = Origin
Working Tree = CLEAN
Ahead / Behind = 0 / 0
↓
HANDOFF CONTRACT
↓
Owner B
↓
同一个 Canonical Workspace
同步 Owner A 最新正式 HEAD
↓
继续施工
```

典型例子：

```text
Codex：State / Commands / Tests
↓
正式闭环
↓
Gemini：AXAML / XYUI / Context Toolbar
```

这种任务 **不得** 默认拆成两个同时写入的 worktree，再增加 Integration 阶段。

普通 Owner 交接不需要 Integration Agent，也不需要 Integration Workspace。

---

# 24. Parallel Work Classification · 并行任务分类

## 24.1 可直接并行

满足以下条件可并行：

```text
只读
无共享写状态
不修改仓库
```

或用户明确批准的完全独立外部任务。

## 24.2 默认不得并行写入

出现以下任一情况时，必须顺序执行：

- 修改同一 Repository
- 共享 ViewModel / Contract / State
- 后一任务依赖前一任务接口
- 同一 Feature / Area / UI workflow
- 最终需要人工 Integration 才能工作

## 24.3 禁止“为了并行而并行”

节省少量施工时间不能作为创建额外 XYengine、增加 Integration 成本、增加 Git 风险的理由。

---

# 25. Temporary Worktree Exception · 临时 worktree 例外

临时 worktree 不是常规交接方式，只是例外工具。

只有同时满足以下条件才可使用：

1. 用户已明确批准本轮使用临时 worktree；
2. 任务确实可以独立写入，或必须隔离高风险实验；
3. 创建前报告 path / branch / base HEAD / write scope；
4. 临时 worktree 不作为正式运行或用户验收入口；
5. 当轮必须把合法成果安全回流 Canonical Workspace；
6. 在 Canonical Workspace 重新运行正式门禁；
7. 正式 commit + push 完成后移除临时 worktree；
8. 不得留下新的长期用户可见 XYengine 副本。

如果临时 worktree 缺少 .NET SDK、依赖、运行环境或完整门禁能力：

```text
NOT TECH COMPLETE
```

不得因为静态检查通过就宣布任务完成。

## 25.1 清理安全

移除临时 worktree 前必须确认：

```text
无未提交修改
有价值成果已安全进入正式基线
Canonical 已验证
Canonical 已 push
```

禁止未经审计：

```text
git worktree remove --force
rm -rf
Remove-Item -Recurse -Force
```

处理未知或含未提交内容的 worktree。

---

# 26. Integration Workspace Policy

普通交接默认：

```text
NO INTEGRATION WORKSPACE
NO INTEGRATION AGENT
```

只有用户明确批准的复杂多分支集成任务，才允许建立一次性 Integration 环境。

任何 Agent 不得自行决定：

> “为了避免冲突，我创建一个 Integration worktree。”

如果任务设计导致必须依赖 Integration 才能完成，应先报告：

```text
HANDOFF DESIGN CONFLICT
```

重新规划 Owner / Write Order，而不是自动扩张工作区。

---

# 27. Agent Orchestration Boundary · Agent 调度边界

收到明确 Owner 任务的 Agent，不得自行把自己升级为总调度器。

例如 Gemini 收到 `G1 UI` 任务时，不得自行：

- 启动 Codex C1 子 Agent
- 重排 Codex → Gemini 的既定顺序
- 创建新的 Codex/Gemini worktree
- 把顺序任务改成并行任务
- 自行增加 Integration 阶段

Codex 同理。

只有任务书明确授予“Orchestrator / 调度 Owner”职责时，Agent 才能创建或调度其他 Agent；即便如此仍必须服从 Single Writer Rule 与 Canonical Workspace Policy。

---

# 28. Environment Mutation Boundary · 环境修改边界

Agent 不得因普通任务自行修改机器级或用户级全局环境，例如：

```text
git config --global ...
系统 PATH
全局 SDK 配置
全局凭据
系统策略
```

若确实遇到环境阻塞，必须先报告：

```text
ENVIRONMENT CHANGE REQUIRED
```

说明原因、影响范围和可逆性，并取得用户明确批准后再执行。

仓库局部、任务内可逆配置仍需遵守 Task Freeze。

---

# 28.1 .NET Toolchain Resolution

所有 Agent、Build/Test 门禁与 `run.bat` 必须共享仓库内唯一的 .NET SDK discovery authority：

```text
scripts/resolve-dotnet.ps1
```

需要执行 .NET 命令时优先通过：

```text
scripts/xuanyu-dotnet.ps1
```

Agent 在报告 `.NET SDK unavailable` 前，必须先调用 Resolver 或 Wrapper。禁止因直接执行 `dotnet --info` 失败就立即阻断；只有仓库 Resolver 确认没有有效 SDK 时，才允许报告 `DOTNET SDK BLOCKED`。

`run.bat` 不得维护第二套 SDK 候选路径、盘符扫描或 PATH 搜索算法；Resolver 成功时 stdout 只能输出真实绝对路径，候选必须通过 `dotnet --list-sdks` 验证。

---

# 29. Implementation Understanding

任何正式施工前，Owner 必须先输出简短理解：

```text
我理解本轮最终用户可见行为为：
1. ...
2. ...
3. ...

本轮不会：
1. ...
2. ...
```

目的在于暴露误解，不是重新要求用户批准已经冻结的决策。

---

# 30. Owner Handoff Contract

当一个 Owner 向下一个 Owner 交接时，必须报告：

```text
Canonical Workspace
Branch
Final HEAD
Origin HEAD
Ahead / Behind
Working Tree
Version
Task Status
Changed Files
Public Binding / Contract
Tests / Gates
Next Owner
Next Allowed Write Scope
```

只有满足任务书规定的门禁后，下一 Owner 才开始写入。

不得使用：

```text
“我写完了，留在独立 branch 给后续集成”
```

作为普通交接完成条件。

临时 branch 上的 commit 若未进入正式基线，只能记录为临时成果，不得标记正式交接完成。

---

# 31. Agent 报告协议

完工报告固定顺序：

```text
A. 这轮用户要解决什么
B. 实际做了什么
C. 用户现在会看到什么变化
D. 为什么这样做
E. 是否存在任务书之外的新决定
F. 用户怎么验收
G. 技术验证
H. Git 基线
I. Handoff Contract（若有下一 Owner）
```

报告必须包含：

```text
任务书之外的产品/架构决定：
NONE
```

如果不是 NONE，必须明确列出。

---

# 32. User Acceptance

视觉和真实交互默认由用户验收。

Agent 的 Build PASS、Tests PASS、自动截图、UI 自动化不能替代：

```text
USER VISUAL ACCEPTANCE
USER INTERACTION ACCEPTANCE
```

只有用户明确批准时，才能标记 `USER ACCEPTED`。

用户验收必须针对 Canonical Workspace / 正式构建物，而不是临时 worktree。

---

# 33. current-handoff.md

该文件不是历史记录，只回答：

> 现在在哪里？

至少记录：

```text
Updated
Git Baseline
Canonical Workspace identity（不把机器路径当跨机规则）
Known Good Baseline
Current Phase
Recently Completed
Current Product / Architecture Decisions
Active Tasks
User Acceptance Pending
Blockers
NEXT
```

Active Tasks 对每个 Agent 至少记录：

```text
Status
Task
Owner
Write Scope
Write Order
Last Commit
Next Action
```

NEXT 只允许一个最主要的下一动作，并且必须具体到新 Agent 不需要猜。

---

# 34. decision-log.md

Decision Log 只记录具有长期约束意义的批准决策，不写每日流水账。

决策改变时旧决策不得删除，应标记：

```text
SUPERSEDED BY:
DEC-...
```

Agent 不得把个人实现偏好写成产品决策。

---

# 35. HANDOFF-CLOSE

用户表达“请进行交接收尾”或等价意图时进入：

```text
XUANYU-HANDOFF-CLOSE
```

流程：

```text
检查当前任务
↓
确认 Technical / Acceptance 状态
↓
执行必要 Gates
↓
Git Audit
↓
完成合法 Commit / Push
↓
确认 origin
↓
更新必要 Decision Log
↓
更新 CHANGELOG / file-tree
↓
更新 current-handoff
↓
写唯一 NEXT
↓
检查其他 worktree 是否存在及是否合法
↓
最终 Git Audit
↓
HANDOFF READY
```

正常交接建议满足：

```text
Canonical Working Tree = CLEAN
Ahead = 0
Behind = 0
Current Handoff 已更新
NEXT 已存在
当前任务状态已明确
无未经批准的长期 XYengine 副本
```

然后才能输出：

```text
HANDOFF READY
```

---

# 36. WIP Handoff

用户明确要求在未完成状态下换电脑时，可以 WIP 交接。

必须记录：

```text
STATUS: WIP
Last Known Good
WIP Commit
Completed
Not Completed
Known Broken
NEXT
```

不得留下：

```text
大量未提交本地修改
+
没有远端备份
+
直接换电脑
```

WIP 交接同样不得把临时 worktree 冒充 Canonical Workspace。

---

# 37. 跨电脑与多 Agent 禁止事项

默认禁止：

1. 电脑 A 存在未交接修改时，电脑 B 在同一 branch 继续开发。
2. 两台电脑同时主动开发同一 branch。
3. Behind > 0 时继续施工。
4. 依赖聊天记忆判断 Git 状态。
5. 新 Agent 在没有 `SYNC READY` 时直接改代码。
6. 为同步方便自动执行 reset / force push / rebase / clean / stash。
7. 同一功能链的 Codex/Gemini 同时写不同 worktree 后再依赖 Integration 收口。
8. Agent 未经任务授权自行调度另一个 Owner。
9. 把临时 worktree 的静态检查 PASS 宣布为正式 TECH COMPLETE。
10. 保留多个长期用户可见 XuanYuEngine 目录作为“验收/集成/下一轮开发”环境。

---

# 38. Repo-local Skill

推荐：

```text
skills/xuanyu-sync/SKILL.md
```

Skill 不复制 SOP，只引用本文件作为真源。

任何 Skill 若包含与 Single Canonical Workspace / Sequential Handoff 冲突的旧规则，必须视为失效并修正。

---

# 39. 脚本职责

`handoff-start.ps1` / `handoff-close.ps1` 等脚本只负责机器可验证事实，例如：

```text
Repo
Canonical Path
Worktree List
Branch
HEAD
Origin HEAD
Status
Ahead
Behind
Version
```

脚本不得判断：

```text
产品是否合理
当前交互是否批准
某功能是否 USER ACCEPTED
下一步产品方向
```

---

# 40. 最终原则

玄域开发必须达到：

```text
换电脑 ≠ 重新恢复记忆
换聊天 ≠ 重新解释项目
换 Agent ≠ 新建一套工作区
Owner 分工 ≠ 工作区分裂
并行思考 ≠ 并行写入
```

正式模型是：

```text
GitHub
+
Single Canonical Workspace
+
Governance Documents
+
Sequential Owner Handoff
+
Explicit User Approval for Exceptions
```

任何新开发终端都通过：

```text
SYNC
↓
UNDERSTAND
↓
SYNC READY
```

恢复完整上下文。

任何同一功能链的 Owner 切换都通过：

```text
OWNER A
↓
VERIFY
↓
COMMIT / PUSH
↓
HANDOFF GATE
↓
OWNER B
```

而不是：

```text
OWNER A WORKTREE
+
OWNER B WORKTREE
+
INTEGRATION WORKTREE
```

只有完成这些过程，才进入下一阶段正式开发。
