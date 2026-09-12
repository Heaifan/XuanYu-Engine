# 玄域引擎同步与交接规范 v1.0

**XuanYu Engine Sync & Handoff Protocol**

状态：正式规范  
适用对象：ChatGPT、Codex、Gemini 及其他参与玄域引擎开发的 AI Agent  
适用场景：换电脑、换聊天、换 Agent、长时间中断后恢复开发、多人/多 Agent 交接

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

---

# 3. 标准生命周期

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

确认代码和远端一致。

## UNDERSTAND

恢复工作流、当前阶段、关键决策和上一轮进度。

## DECIDE

确认用户目标以及产品/交互/架构决定。

## TASK FREEZE

冻结本轮允许修改的范围。

## DEVELOP

Codex / Gemini 按 Owner 责任域执行。

## VERIFY

运行 Build、Tests、ARCH-A、5+100 等门禁。

## COMMIT / PUSH

形成可追踪 Git 基线。

## USER ACCEPTANCE

由用户执行真机、视觉、交互验收。

## HANDOFF

把当前状态写回仓库，允许其他电脑或 Agent 无损接管。

---

# 4. 仓库标准文件

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

---

# 5. AI-BOOTSTRAP.md

此文件位于仓库根目录。

它是任何新 AI 接管玄域时的唯一入口。

建议内容如下：

```markdown
# XuanYu Engine · AI Bootstrap

本文件是玄域引擎 AI 接管入口。

如果用户表达以下或等价意图：

- 请你开始同步吧
- 开始同步
- 接管项目
- 恢复开发环境
- 换电脑继续开发
- 同步一下玄域
- 看看现在做到哪里了并继续

立即进入 XUANYU-SYNC。

## 禁止

在完成 XUANYU-SYNC 前：

- 禁止修改业务代码
- 禁止创建新功能
- 禁止开始新的 Codex / Gemini 开发任务
- 禁止自行推断当前开发阶段
- 禁止根据旧聊天直接继续施工

## 执行顺序

1. 定位当前 XuanYu Engine Git Repository
2. 阅读：

   docs/governance/sync-handoff-sop.md

3. 按 SOP 完成 Git 与项目状态恢复
4. 阅读：

   docs/governance/development-constitution.md
   docs/governance/development-workflow.md
   docs/governance/current-handoff.md
   docs/governance/decision-log.md

5. 按需读取：

   changelog.md
   file-tree.md

6. 输出 XUANYU HANDOFF ACK
7. 只有满足全部接管门禁后才允许输出：

   SYNC READY

SYNC READY 之前禁止进入开发阶段。
```

---

# 6. sync-handoff-sop.md

这是同步与交接程序的唯一正式真源。

Skill、Agent Prompt、脚本不得另外复制一套不同规则。

建议正文如下。

---

## 6.1 XUANYU-SYNC 触发条件

当用户表达：

```text
请你开始同步吧
开始同步
接管项目
恢复开发
换电脑继续
```

或具有相同含义的请求时，立即执行本 SOP。

不得要求用户重新解释当前项目进度，除非 SOP 执行后仍存在无法从仓库判断的冲突。

---

# 7. SYNC-01 · Repository Discovery

首先确定真实仓库。

检查：

```text
.git
AI-BOOTSTRAP.md
```

确认：

```text
Repository Root
Physical Path
Git Remote
```

本机绝对路径只用于本次审计。

禁止将：

```text
D:\
E:\
C:\
```

等机器路径作为跨电脑工作流依据。

正式规范和任务书优先使用 repo-relative path。

---

# 8. SYNC-02 · Git Safety Audit

在任何同步操作之前检查：

```text
Current Branch
Local HEAD
Working Tree
Remote
Origin Branch
Ahead
Behind
```

必须先执行 fetch，再判断状态。

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

继续后续接管。

---

## CASE B — SAFE BEHIND

```text
Working Tree = CLEAN
Ahead = 0
Behind > 0
```

说明本机仅落后远端。

允许执行安全 Fast-Forward 同步。

同步后重新检查：

```text
Ahead = 0
Behind = 0
Working Tree = CLEAN
```

满足后继续。

---

## CASE C — DIRTY

出现：

```text
Working Tree = DIRTY
```

立即停止自动同步。

状态：

```text
SYNC BLOCKED
```

必须向用户报告：

- 修改文件
- 未跟踪文件
- 当前 branch
- 当前 HEAD

禁止擅自执行：

```text
git reset
git clean
git stash
git checkout -- .
覆盖文件
删除文件
```

---

## CASE D — LOCAL AHEAD

出现：

```text
Ahead > 0
Behind = 0
```

状态：

```text
SYNC BLOCKED
```

说明本机存在远端没有的提交。

AI 不得因为“看起来是正常提交”而擅自推送。

必须首先确认这些提交属于合法未交接成果。

---

## CASE E — DIVERGED

出现：

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

必须先进行人工或正式 Git 基线治理。

---

# 10. SYNC-03 · Code Baseline

Git 对齐以后，必须明确记录：

```text
Repository
Branch
HEAD
Origin HEAD
Version
Working Tree
Ahead / Behind
```

例如：

```text
Repository:
XuanYuEngine

Branch:
feat/...

HEAD:
xxxxxxxx

Origin HEAD:
xxxxxxxx

Version:
vX.X.X

Working Tree:
CLEAN

Ahead / Behind:
0 / 0
```

禁止使用：

```text
应该同步好了
好像是最新版
应该和 GitHub 一样
```

等模糊表述。

Git 状态必须使用真实 SHA 和数值。

---

# 11. SYNC-04 · Context Recovery

代码同步完成后不得立即开发。

必须按以下顺序恢复项目认知。

---

## 11.1 Development Constitution

回答：

> 什么绝对不能做？

包括但不限于：

- 架构边界
- 5+100
- SRP
- Task Freeze
- Git 禁令
- Scope Expansion 禁令
- Canonical XYUI 访问规则
- Build / Test 门禁

---

## 11.2 Development Workflow

回答：

> 玄域现在按照什么流程开发？

必须理解：

```text
用户目标
↓
产品/交互/架构决策
↓
必要时 SVG
↓
Task Freeze
↓
Owner 分配
↓
Implementation Understanding
↓
开发
↓
自动门禁
↓
Commit + Push
↓
用户真机验收
↓
交接
```

---

## 11.3 Current Handoff

回答：

> 项目现在做到哪里？

必须获得：

- 当前阶段
- 最近完成内容
- 当前进行中任务
- 当前阻塞
- NEXT
- Known Good Baseline
- Codex 状态
- Gemini 状态
- 用户验收状态

---

## 11.4 Decision Log

回答：

> 为什么当前产品和架构是这样？

只恢复仍然有效或与当前任务有关的关键决策。

禁止 Agent 因个人偏好覆盖已有批准决策。

---

## 11.5 CHANGELOG

优先读取：

- 最近若干轮
- Current Handoff 引用的版本附近记录
- 当前任务相关记录

无需每次全文重读历史日志。

---

## 11.6 file-tree

用于确认：

- 当前模块位置
- 文件职责
- 项目结构
- 新任务涉及的责任域

---

# 12. SYNC-05 · Execution State Recovery

必须恢复当前 Agent 状态。

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
```

新 Agent 不得重复启动已经存在的同一任务。

---

# 13. 统一任务状态

以后禁止只写：

```text
完成
```

任务必须使用以下状态之一：

```text
PLANNED
IMPLEMENTING
TECH COMPLETE
USER ACCEPTED
BLOCKED
SUPERSEDED
```

含义如下。

## PLANNED

已规划，尚未施工。

## IMPLEMENTING

正在开发。

## TECH COMPLETE

代码、测试、门禁已完成。

不代表用户已经认可产品行为。

## USER ACCEPTED

用户已经完成实际验收。

## BLOCKED

存在阻塞，不允许继续。

## SUPERSEDED

被新的产品决策或任务替代。

---

# 14. 技术完成与用户验收必须分离

必须分别记录：

```text
Technical Status
User Acceptance Status
```

例如：

```text
Marker Inspector

Technical:
TECH COMPLETE

User Acceptance:
NEEDS REVISION
```

禁止：

```text
Build PASS
=
用户认可
```

禁止：

```text
Tests PASS
=
交互设计正确
```

---

# 15. SYNC-06 · XUANYU HANDOFF ACK

完成 Git 和上下文恢复后，新 AI 必须输出普通中文接管摘要。

固定包含：

```text
【Git 基线】

Repository:
Branch:
HEAD / Origin:
Working Tree:
Ahead / Behind:
Version:


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

最后只有在以下条件全部满足时：

```text
Git baseline 已对齐
Working Tree 状态明确
当前阶段已恢复
最新关键决策已恢复
NEXT 已明确
Agent 状态已恢复
不存在未处理同步冲突
```

才能输出：

```text
SYNC READY
```

---

# 16. SYNC READY 是硬门禁

在 `SYNC READY` 之前：

```text
禁止业务代码修改
禁止 UI 实装
禁止新增功能
禁止任务扩围
禁止启动 Codex / Gemini 正式施工
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

玄域正式开发统一采用以下工作流。

```text
USER INTENT
↓
APPROVED DECISION
↓
TASK FREEZE
↓
OWNER
↓
IMPLEMENTATION UNDERSTANDING
↓
DEVELOP
↓
VERIFY
↓
REPORT
↓
COMMIT / PUSH
↓
USER ACCEPTANCE
↓
HANDOFF
```

---

# 18. USER INTENT

每项任务必须首先用普通中文说明：

> 用户到底想解决什么问题？

例如：

```text
用户目标：

地图编辑需要统一处理点、线、面，
不希望 Marker 被拆成独立顶层编辑模式。
```

禁止把技术实现方案伪装成用户目标。

---

# 19. APPROVED DECISION

所有影响以下内容的变化均属于产品/架构决策：

- 模式划分
- 导航结构
- 顶层入口
- 工具归属
- 对象分类
- 保存行为
- Inspector 分类
- 用户操作流程
- 产品术语
- 核心架构边界

任务书未明确批准时，Codex / Gemini 不得自行决定。

如果发现必要决策缺失：

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

WRITE SCOPE

READ SCOPE

FORBIDDEN SCOPE

DELIVERABLES

ACCEPTANCE

GATES
```

禁止 Opportunistic Refactor。

禁止：

```text
顺手重构
顺手改名
顺手修其他模块
顺手改变交互
顺手引入依赖
```

---

# 21. OWNER 制度

Codex 与 Gemini 均为独立 Owner。

禁止默认形成：

```text
Gemini 写
↓
Codex 修
```

或：

```text
Codex 写
↓
Gemini 重做
```

正确模式：

```text
TASK-A → CODEX OWNER

TASK-B → GEMINI OWNER
```

每个 Owner 对自己的：

- 实现
- 测试
- 门禁
- Git
- 报告

负责到底。

---

# 22. Independent Ownership

并行任务必须尽量满足：

```text
不同文件域
不同职责域
不同 Write Scope
```

允许读取共同 Contract。

禁止两个 Agent 未经明确安排同时修改同一核心文件。

---

# 23. Implementation Understanding

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

目的不是重新请求用户确认。

目的在于暴露 Agent 是否误解任务。

如果理解明显偏离已批准决策，不得进入施工。

---

# 24. Agent 报告协议

完工报告首先服务于项目负责人理解，而不是服务于 Agent 自己。

固定顺序：

```text
A. 这轮用户要解决什么

B. 实际做了什么

C. 用户现在会看到什么变化

D. 为什么这样做

E. 是否存在任务书之外的新决定

F. 用户怎么验收

G. 技术验证

H. Git 基线
```

---

# 25. DEVIATION

报告必须包含：

```text
任务书之外的产品/架构决定：
NONE
```

如果不是 NONE：

必须列出。

未经批准的重要产品层偏移不得静默进入正式基线。

---

# 26. User Acceptance

视觉和真实交互默认由用户验收。

Agent 的：

```text
Build PASS
Tests PASS
自动截图
UI 自动化
```

不能替代：

```text
USER VISUAL ACCEPTANCE
USER INTERACTION ACCEPTANCE
```

只有用户明确批准时，才能标记：

```text
USER ACCEPTED
```

---

# 27. current-handoff.md

该文件不是历史记录。

它只回答：

> 现在在哪里？

必须保持短、准、最新。

标准模板：

```markdown
# XuanYu Current Handoff

Updated:
YYYY-MM-DD HH:mm

## Git Baseline

Branch:

HEAD:

Origin HEAD:

Version:

Working Tree:

Ahead / Behind:


## Known Good Baseline

Commit:

Version:

Build:

Tests:

Architecture Gates:

User Acceptance:


## Current Phase

...


## Recently Completed

- ...
- ...
- ...


## Current Product / Architecture Decisions

- ...
- ...
- ...


## Active Tasks

### Codex

Status:
IDLE

Task:
NONE


### Gemini

Status:
IDLE

Task:
NONE


## User Acceptance Pending

- ...


## Blockers

NONE


## NEXT

只允许一个最主要的下一动作。

例如：

修正地图编辑信息架构，
将点 / 线 / 面统一纳入批准的地图编辑工作区。


## Notes

只记录下一台电脑真正需要知道的额外信息。
```

---

# 28. NEXT 规则

NEXT 必须具体。

禁止：

```text
继续优化编辑器
继续开发
做下一阶段
```

应写：

```text
NEXT:

移除未经批准的独立“点要素编辑”顶层模式，
将 Marker 编辑重新纳入统一地图编辑工作区。
```

原则：

> 新电脑上的 AI 看完 NEXT 后，不需要猜下一步是什么。

---

# 29. decision-log.md

Decision Log 只记录具有长期约束意义的批准决策。

禁止写成每日流水账。

模板：

```markdown
# XuanYu Decision Log

## DEC-YYYY-MM-DD-NN

### 主题

...

### 状态

ACTIVE

### 决定

...

### 原因

...

### 影响范围

...

### 禁止推导

Agent 不得据此额外推导未经批准的新产品结构。

### 被替代

NONE
```

如果以后决策改变：

旧决策不得删除。

改为：

```text
SUPERSEDED BY:
DEC-...
```

---

# 30. 典型 Decision 示例

```markdown
## DEC-2026-09-12-01

### 主题

地图编辑模式与几何类型关系

### 状态

ACTIVE

### 决定

地图编辑采用统一工作区。

点、线、面属于该工作区中的不同 Geometry Tool / Authoring Tool，
而不是三个独立的顶层编辑模式。

Marker 等点对象不得因为 Inspector 或编辑能力需要，
自行新增“点要素编辑”顶层模式。

### 原因

点、线、面属于地图要素几何类型差异，
不应因此增加用户顶层认知模式和入口数量。

### 影响范围

- Top Context Toolbar
- Editor Mode
- Marker Editing
- Road Editing
- Region Editing
- Inspector Entry

### 禁止推导

未经用户批准不得新增：

- 点要素编辑模式
- 道路编辑模式
- 其他按 Geometry Type 划分的顶层 Edit Mode
```

---

# 31. 离开电脑：HANDOFF-CLOSE

用户表达：

> 请进行交接收尾

或等价意图时，进入：

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
更新 Decision Log（仅必要时）
↓
更新 CHANGELOG / file-tree（按治理规则）
↓
更新 current-handoff
↓
写唯一 NEXT
↓
最终 Git Audit
↓
HANDOFF READY
```

---

# 32. HANDOFF READY 门禁

正常交接建议满足：

```text
Working Tree = CLEAN
Ahead = 0
Behind = 0
Current Handoff 已更新
NEXT 已存在
当前任务状态已明确
```

然后输出：

```text
HANDOFF READY
```

---

# 33. WIP Handoff

如果用户明确要求在未完成状态下换电脑，可以进行 WIP 交接。

但必须记录：

```text
STATUS:
WIP

Last Known Good:
...

WIP Commit:
...

Completed:
...

Not Completed:
...

Known Broken:
YES / NO

NEXT:
...
```

不得留下：

```text
大量未提交本地修改
+
没有远端备份
+
直接换电脑
```

---

# 34. 跨电脑禁止事项

以下行为默认禁止。

## 禁止 A

电脑 A 存在未交接修改时，电脑 B 在同一 branch 继续开发。

## 禁止 B

两台电脑同时主动开发同一 branch。

## 禁止 C

Behind > 0 时继续施工。

## 禁止 D

依赖聊天记忆判断 Git 状态。

## 禁止 E

新 Agent 在没有 `SYNC READY` 时接受“继续吧”并直接改代码。

## 禁止 F

为了同步方便自动：

```text
reset
force push
rebase
clean
stash
```

未经明确授权不得使用这些破坏性或隐式状态改变操作。

---

# 35. Repo-local Skill

推荐建立：

```text
skills/xuanyu-sync/SKILL.md
```

Skill 不复制 SOP。

内容保持极简：

```markdown
# XuanYu Sync Skill

## Trigger

当用户要求：

- 开始同步
- 接管玄域
- 换电脑继续
- 恢复开发环境
- 查看当前进度并继续

执行本 Skill。

## Procedure

1. Locate XuanYu Engine repository.
2. Read repository root `AI-BOOTSTRAP.md`.
3. Follow the Sync/Handoff SOP referenced by Bootstrap.
4. Treat the SOP as the single source of truth.
5. Do not modify product code before `SYNC READY`.
6. If Git state is unsafe, output `SYNC BLOCKED` and stop.
```

---

# 36. 脚本职责

推荐：

```text
scripts/handoff-start.ps1
scripts/handoff-close.ps1
```

脚本只负责机器可验证事实。

例如：

```text
Repo
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
某个功能是否 USER ACCEPTED
下一步产品方向
```

这些属于 AI + Governance Documents。

---

# 37. AI 与脚本职责分离

完整结构：

```text
用户
 │
 ▼
XuanYu Sync Skill
 │
 ▼
AI-BOOTSTRAP.md
 │
 ▼
sync-handoff-sop.md
 │
 ├───────────────┐
 ▼               ▼
Git Script        AI Context Recovery
 │               │
机器事实          项目认知
 │               │
 └───────┬───────┘
         ▼
 XUANYU HANDOFF ACK
         ↓
     SYNC READY
```

---

# 38. 启动命令

以后用户换电脑只需要：

> 请你开始同步吧。

Agent 自动执行：

```text
XUANYU-SYNC
```

用户无需重新介绍：

- Branch
- HEAD
- 当前进度
- 工作流
- Codex 状态
- Gemini 状态
- 最新产品决策

这些内容应由仓库恢复。

---

# 39. 收尾命令

用户离开当前电脑时只需要：

> 请进行交接收尾。

Agent 自动执行：

```text
XUANYU-HANDOFF-CLOSE
```

并最终输出：

```text
HANDOFF READY
```

---

# 40. 最终原则

玄域开发必须达到：

```text
换电脑
≠
重新恢复记忆

换聊天
≠
重新解释项目

换 Agent
≠
重新培训工作方式
```

而应当是：

```text
仓库
=
代码
+
规则
+
工作流
+
决策
+
进度
+
交接状态
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

只有完成这一过程，才进入正式开发。
