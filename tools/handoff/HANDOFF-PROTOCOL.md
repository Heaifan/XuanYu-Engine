# XuanYuEngine Handoff Compatibility Protocol

## 1. Purpose

本文件描述 R2 的兼容边界，不是当前开发流程的启动入口。

当前架构事实：

```text
Handclap = Event Ledger + ACK + Context Transfer = ZERO Authority
Handoff  = Legacy compatibility shell = ZERO Authority
Authority Plane = tools/governance/**
```

Candidate、Coordinator、Ownership、Release、Task、Workspace 的控制权分别由
`tools/governance/` 下对应 Authority owner 持有。Handoff 不创建、推进、关闭、释放、授权、加锁或裁决 Candidate。

当前推荐入口是直接调用对应 Authority owner 与 Handclap；不得把本文件中的 PREPARE、JOIN、ADVANCE、CLOSE 流程当作当前推荐流程。

以下命令仅作为 Legacy / Historical 兼容语义保留：

模式入口：

```text
prepare / join / advance / close / lane-close / migrate-active
commit-lock / commit-unlock / maintenance / repair
```

当前 Handoff Shim 只允许转发 Handclap 的 `event`、`history`、`ack`、`context`、`help`；所有 Authority 命令返回 RETIRED。

### R2 Selftest disposition

- `process-runner.selftest.ps1`: `KEEP_AS_HANDOFF_COMPAT`。
- `r2-contract-integration.selftest.ps1`: `KEEP_AS_HANDOFF_COMPAT`，验证 Shim 驱逐、Handclap 隔离和 runtime legacy caller。
- `adversarial-selftest.ps1`: `HISTORICAL_UNCHANGED`；旧 Authority 集成历史，不作为当前入口。
- `dirty-convergence.selftest.ps1`: `HISTORICAL_UNCHANGED`；旧 PREPARE dirty 语义历史，不作为当前入口。
- `handoff.selftest.ps1`: `HISTORICAL_UNCHANGED`；旧生命周期/Mutex/Candidate 成功断言历史，不作为当前入口。
- `state-lifecycle.selftest.ps1`: `HISTORICAL_UNCHANGED`；旧 state lifecycle 成功断言历史，不作为当前入口。

上述历史文件保留原样用于审计追溯；R2 统一测试不执行其中的 Authority 成功断言。Authority 行为由 `tools/governance/**` 对应 owner selftest 覆盖。

### Legacy / Historical 1.3 Task Flight Plan / ACTIVE TASK LEAK

任务级 Registry 使用 `.git\xye-handoff\task-registry.json`，由 `task-flight-plan.ps1` 维护；`task-reports.json` 记录本波次任务报告状态。JOIN、STATUS、ADVANCE 会显示 `ACTIVE TASKS`、`RELEASED TASKS`、Dirty Classifier 分类与 `FinalEvidenceEligibility`。`COMPLETE`、`IMPLEMENTATION_COMPLETE` 或 `HANDOFF_READY` 报告若仍对应 Registry `ACTIVE`，即为 `TASK_STATE_LEAK`，Gate 为 `BLOCKED`，`COMMIT_ELIGIBILITY = NO`；不会自动猜测 release。正常完成必须先 `ACTIVE -> RELEASED`，异常遗留只能由 Coordinator 显式 `reap`。

### Legacy / Historical 1.1 Active Wave 内的连续任务

标准流程为：

```text
prepare → join → work → commit + push → advance → next join
```

`advance` 不是新的 `prepare`，也不结束 Wave。它只在当前 branch、远端 branch、HEAD 和远端 tip 完全收敛，且旧 baseline 是当前 HEAD 祖先时，将 `baselineHead` 原子推进到当前 HEAD；Active Wave、ForeignDirty 和产品工作区保持不变。推进记录保存在 state.json 的 `baselineAdvance` 字段中，state.json 不进入 Git。

JOIN 允许 `baselineHead == HEAD`，也允许 `baselineHead` 是当前 HEAD 的祖先；这使已 JOIN Session 不会因其他 Session 的合法 fast-forward commit 停工。只有 branch 改变或历史分叉时才阻断 JOIN。

当需要提交时，Agent 必须先获取 Git Commit Mutex：

```text
handoff.cmd commit-lock --scope xye --owner <session>
```

持锁者才可以执行精确路径 stage、commit、push 和 `advance --owner <session>`；禁止 `git add .`。成功 advance 后锁自动释放。无提交时，持锁者可用同 owner 的 `commit-unlock` 释放；HEAD 已前进时不得绕过 push + advance 解锁。

Convergence Coordinator 为 `xye` 时，治理控制面修复可由 `governance` Commit Mutex 持有者执行 `advance --scope governance`；该例外只适用于 `state.mode=convergence`，不放宽其他 Lane 的 Coordinator 或 Ownership 限制。

当 JOIN 报 `BASELINE_MOVED` 时，Agent 必须停止并报告 baseline、HEAD、Remote HEAD、Ahead/Behind、Branch 和 ForeignDirty。不得执行 prepare、手动编辑 state.json、reset、stash 或 clean；先处理真实 branch/分叉异常。

`state.json` 的 Wave 字段为：`mode=development|convergence|WIP_RESUME`、`coordinatorScope=xye|integration|governance|null`。WIP Resume 只能作为临时 state，必须记录 `sourceBranch`、`targetBranch`、`createdAt`、`expiryCondition`；不得写入 Repository Config。当前 Branch 到达 target 且 HEAD 与 target upstream 0/0 后，WIP 自动失效，旧 source 不再参与新的 PREPARE 比较。

### 1.2 Handclap Event Ledger

Handclap 拥有 Event Ledger、ACK 与 Context Transfer。它不拥有 Candidate、Coordinator、Ownership、Release、Task 或 Workspace Authority。

Handoff 不再拥有 Lane 或任务生命周期状态机，不再推进、验证或阻断任何生命周期状态。

Handclap 追加事件到 `.git\xye-handclap\events.jsonl`：

```text
CREATED
STARTED
TRANSFERRED
COMMENTED
COMPLETED_REPORTED
CANCELLED_REPORTED
```

事件是事实记录，不是状态转换。`event` 入口不会修改 Task Registry、Wave state、Lane state、Gate、Candidate 或 Commit 资格。

旧 `state.json` 中的 `laneStates`、状态字段和历史记录仍可读取，用于迁移与审计；新的 Handoff 写入不得再创建或更新这些状态字段。

Legacy / Historical：R2 的 `active=true`、JOIN、status 与 `migrate-active` 迁移语义只用于旧状态审计，不是当前推荐流程。

### 1.3 Handoff Slimming R1 final acceptance

Legacy Control 的最终验收只接受以下两种结果：

- **A. Legacy exists but is non-executable**：旧入口可以作为迁移/审计痕迹保留，但调用必须明确返回 retired/removed 结果，不得写入生命周期状态或重新取得控制权。
- **B. Legacy removed with complete migration record**：旧入口已移除，并且迁移记录完整、可追溯，足以证明其职责已由现行入口承接。

其他状态不得标记为最终收口。特别是“仍可执行但暂时无人使用”不属于合法 Legacy Control 终态。

本轮 Slimming 的架构事实是：**Handclap = Event Ledger + ACK + Context Transfer；Handoff = Legacy compatibility shell + History**。Handoff 只保留兼容转发与审计历史，不授予控制权。

```text
Handoff != Controller
Handoff != Gate
Handoff != Lock
Handoff != Commit Authority
Handoff != Release Authority
```

Task Registry、Wave state、Lane/Ownership、Gate、Candidate、Commit 和 Release 资格继续由各自权威负责，不能由 Handoff 重新取得。

## 2. Canonical Workspaces

两台电脑允许使用不同绝对路径：

```text
私人电脑
D:\MyDoc\project-vsCode\XuanyuEngine

工作电脑
E:\MyDoc\project-VSCode\XuanYuEngine
```

盘符不同是预期行为。要求是每台机器只使用自己的一个 canonical workspace。

## Legacy / Historical 3. Cross-machine authority

本节是历史记录，不是 R2 当前推荐流程。

GitHub Remote 是跨设备正式事实源。

优先级固定为：

```text
1. 本地独有正式 Commit
2. GitHub Remote
3. 本地未提交 tracked 修改 / untracked 文件
```

### 3.1 Local Ahead > 0（仅 PREPARE）

如果当前 Local HEAD 相对当前 branch upstream 存在 Remote 没有的正式 Commit：

```text
Ahead > 0
→ HANDOFF BLOCKED
```

交接程序不得自动覆盖这些 Commit，必须报告独有 Commit 后等待人工处理。

### 3.2 Local Ahead = 0 且 Behind > 0（仅 PREPARE）

> **RETIRED / DATA-SAFETY OVERRIDE:** 本节旧 REMOTE WINS 行为仅保留为历史证据，禁止执行。任何 tracked、untracked、Foreign 或 Unknown dirty 都必须保全；禁止 reset、clean、stash、checkout、restore 或覆盖。当前安全同步规则以 `AGENTS.md` 为准，只允许 clean worktree 上的 fast-forward。

如果本地没有独有正式 Commit，但落后于当前 branch upstream：

```text
Ahead = 0
Behind > 0
→ REMOTE WINS
```

此时本地未提交 tracked 修改和 untracked 残留不得阻断交接。程序必须：

- 丢弃 tracked 未提交修改；
- 删除 non-ignored untracked 文件 / 目录；
- 保持当前 branch 不变；
- 将 Local HEAD 精确对齐当前 branch upstream；
- 再次验证 Ahead / Behind = 0 / 0；
- 验证 working tree clean。

### 3.3 Local 与 Remote 同一 Tip（仅 PREPARE）

如果 Ahead = 0、Behind = 0，但工作区仍 dirty，则 PREPARE 禁止静默删除：

```text
same tip + dirty
→ HANDOFF BLOCKED
```

这是为了避免 PREPARE 删除当前机器尚未跨设备交接的即时工作；JOIN 不受此规则阻断。

## Legacy / Historical 4. Active Branch

当前权威开发分支来自 live Git：`git branch --show-current`；远端事实来自该 branch 的 upstream，默认回退为 `origin/<CurrentBranch>`。Ahead/Behind 永远比较 `HEAD...upstream`，不得比较持久化的旧 ActiveBranch 或旧 WIP 分支。

## Legacy / Historical 5. SDK / Resolver

Agent 不得根据 PATH 猜测 .NET SDK 是否存在。

交接程序优先登记：

```text
D:\MyApp\sdk-dotnet\dotnet.exe
E:\MyApp\sdk-dotnet\dotnet.exe
```

随后必须进入仓库正式 Resolver Chain：

```text
scripts/resolve-dotnet.ps1
→ scripts/xye-bootstrap.ps1
→ scripts/xye-dotnet.ps1
```

只有 Resolver 实际失败后，才允许报告 SDK 不可用。

## Legacy / Historical 6. JOIN 基线与 Lane 规则

JOIN 必须只读取 Git 事实：`rev-parse`、`branch --show-current`、`status`、`worktree list` 及正式 Resolver / Bootstrap。它不得执行 fetch、pull、reset、clean、checkout、switch、merge、rebase、stash、commit 或 push；Git 成败只看真实 Exit Code。

JOIN 必须允许 dirty；Active Wave 只有在 Branch 改变或 `baselineHead` 不是当前 HEAD 的祖先时，才以 `BASELINE_MOVED` 阻断。缺失或已关闭 state 时，只要 canonical workspace、当前 branch upstream、HEAD/Remote 0/0、无 staged conflict 与 ownership conflict，即返回 `HANDOFF JOIN PASS` 和 `Wave: STATELESS / READY`。输出必须包含 `DirtyFiles`、`Scope`、`OwnDirty`、`ForeignDirty` 与 `GitMutation: NONE`。XYUI / XYE / GOVERNANCE 可按路径归属报告 OwnDirty 与 ForeignDirty；INTEGRATION 无法可靠推断时必须输出 `OwnDirty: UNKNOWN`、`ForeignDirty: INFORMATIONAL`，不得把 ForeignDirty 当作 OwnershipConflict。

development 下所有 Lane JOIN 可通过 dirty。convergence 且 `coordinatorScope=xye` 时，`join --scope xyui` 返回 `HANDOFF BLOCKED` 与 `CONVERGENCE_EXCLUSIVE`；`join --scope xye` 与 XYE Coordinator status 允许。

## Legacy / Historical 7. PREPARE / LANE CLOSE / GLOBAL CLOSE

Active Wave 存在且未过期时 PREPARE 返回 `ACTIVE_WAVE`，不得同步。旧 Closed Wave 不阻塞新的普通 PREPARE。

Lane 完成与 Workspace Wave 关闭是两个不同动作。普通 Lane 必须使用 `handoff.cmd lane-close --scope <lane>`；它只将该 Lane 置为 `FROZEN`，释放该 Lane 的 Task、Ownership 和 Work Release，不得写入 Workspace `active=false`，也不得影响其他 Lane。重复 `lane-close` 返回幂等成功；Wave 已关闭时返回 `NO_ACTIVE_WAVE`，不得篡改历史。

全局 `handoff.cmd close --scope <coordinator>` 只允许 `state.coordinatorScope` 匹配的 Coordinator。关闭前必须确认无 Active Lane、Active Task、未关闭 Task Registration、Ownership/Work Release、Commit Mutex；任一仍存在均拒绝。非 Coordinator 返回 `COORDINATOR_REQUIRED`，仍有 Lane 返回 `ACTIVE_LANES_EXIST`，仍有 Task 返回 `ACTIVE_TASKS_EXIST`，资源未释放返回 `OWNERSHIP_NOT_RELEASED`，锁被持有返回 `COMMIT_MUTEX_HELD`。只有全部资源释放且 working tree clean 时才将 Active 标记为 false；dirty 时返回 `DIRTY_ON_CLOSE`。

`prepare` 默认登记 `mode=development`；WIP Resume 必须通过本次命令显式提供 `-SourceBranch` 与 `-TargetBranch`，并只登记到 state。Control Plane Maintenance 仅在 canonical、clean、当前 branch/upstream 0/0 时通过，且只能修复 `tools/handoff/**` 与治理文档/测试。Active Wave 期间再次 PREPARE 永远阻断，已满足 expiry condition 的 WIP 除外。

## Legacy / Historical 8. Legacy worktrees

历史 worktree 是治理债务，不是当前跨电脑交接阻断条件。

交接程序只报告 worktree 数量，不自动删除。

禁止为新的并行开发继续创建额外 worktree；当前并行开发使用：

```text
一个 canonical workspace
+
文件 ownership / mutex 边界
```

## Legacy / Historical 9. Self Test / Dry Test

`powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File tools\handoff\handoff.selftest.ps1` 在系统临时目录创建隔离 Git fixture，覆盖祖先 baseline 下的并行 JOIN PASS、Commit Mutex 争抢/owner 校验/安全解锁、commit 后 advance 自动释放、dirty 保留、XYUI convergence `CONVERGENCE_EXCLUSIVE`、JOIN/status 无 Git mutation、Active Wave prepare BLOCKED、dirty close BLOCKED。`state-lifecycle.selftest.ps1` 额外覆盖 live upstream、旧 WIP expiry、stateless JOIN、Remote Wins、Local Ahead 安全保护与 maintenance gate；测试结束删除 fixture，不污染 canonical Workspace。

### 9.1 Candidate-Scoped Final Evidence Gate

`ACTIVE TASKS`、Repository `TEMP` 与 Repository `UNKNOWN TEMP` 仅为信息字段，不得单独阻断 Candidate。若 `.git\xye-handoff\candidate.json` 存在，Handoff 以其中的 Candidate Files、Build/Test/Runtime Dependencies、Truth/Registry/Harness Files 构造 Dependency Closure；只有命中该闭包的 ACTIVE writer、Consumed Active Foreign TEMP、UNKNOWN TEMP、OWNERSHIP CONFLICT、未吸收的 FRIENDLY RELEASED TEMP 或 Freeze Fingerprint 变化才可使 `FINAL EVIDENCE ELIGIBILITY` / `CERTIFICATION ALLOWED` 为 `NO`。Candidate 指纹只覆盖该闭包。未配置 manifest 时只输出 `NOT_CONFIGURED`，不得把仓库计数伪装成 Candidate 结论。

用户层使用 `TEMP FILES`、`OWNED TEMP`、`FRIENDLY ACTIVE TEMP`、`FRIENDLY RELEASED TEMP`、`LEGACY TEMP`、`UNKNOWN TEMP`、`CONFLICT TEMP`；`DirtyFiles`、`ForeignDirty`、`UnknownDirty` 仅作为兼容字段，不拥有最终 Candidate Gate 裁决权。Handoff 不代表 XYT Truth-Reviewed、XYT Certified 或 XYT Pass。

## Legacy / Historical 10. Successful final state

`HANDOFF JOIN PASS` 必须满足：

- 当前目录属于登记的 canonical workspace；
- 当前分支 = state.json 登记 Branch；
- Local HEAD = state.json 登记 baselineHead；
- Working Tree 可 clean 或 dirty；
- SDK 已由正式 Resolver 解析；
- `dotnet --version` 可执行；
- Repository Bootstrap = READY。

任何一项不满足都不得宣称交接完成。
