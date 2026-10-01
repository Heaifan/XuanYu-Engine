# XuanYuEngine Workstation Handoff Protocol

## 1. Purpose

本协议是 XuanYuEngine 每个开发会话的强制启动入口。

每个 Workspace / Development Wave 由 Coordinator 执行一次 `prepare`；同一 Workspace 的其他并行 Session 执行只读 `join`。`join` 允许工作区因其他 Ownership Task 而 dirty，不执行 Git 同步。

模式入口：

```text
tools\handoff\handoff.cmd prepare  # 新 Wave / 跨电脑接管，可同步 Git
tools\handoff\handoff.cmd join --scope xye|xyui|integration|governance
                                     # 并行 Session，只读，允许 dirty
tools\handoff\handoff.cmd advance --scope xye
                                     # Coordinator 在 commit + push 后推进 Active Wave baseline
tools\handoff\handoff.cmd status   # 只读状态
tools\handoff\handoff.cmd close    # 收口后关闭 Active Wave
tools\handoff\handoff.ps1 -Mode maintenance -Scope governance # Control Plane 修复前置检查
```

无参数等价于 `join --scope xye`。状态登记在 `.git\xye-handoff\state.json`，不进入 Git；JOIN 不在缺失 state 时创建它。

### 1.3 Task Flight Plan / ACTIVE TASK LEAK

任务级 Registry 使用 `.git\xye-handoff\task-registry.json`，由 `task-flight-plan.ps1` 维护；`task-reports.json` 记录本波次任务报告状态。JOIN、STATUS、ADVANCE 会显示 `ACTIVE TASKS`、`RELEASED TASKS`、Dirty Classifier 分类与 `FinalEvidenceEligibility`。`COMPLETE`、`IMPLEMENTATION_COMPLETE` 或 `HANDOFF_READY` 报告若仍对应 Registry `ACTIVE`，即为 `TASK_STATE_LEAK`，Gate 为 `BLOCKED`，`COMMIT_ELIGIBILITY = NO`；不会自动猜测 release。正常完成必须先 `ACTIVE -> RELEASED`，异常遗留只能由 Coordinator 显式 `reap`。

### 1.1 Active Wave 内的连续任务

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

### 1.2 R3 Authority / Lane State

R3 将 Lane 状态与 Coordinator 全局结论分离。Lane 状态只能沿以下单向序列迁移，默认起点为 `JOINED`，`FROZEN` 为终态：

```text
JOINED → ACTIVE → IMPLEMENTATION_COMPLETE → IMPLEMENTATION_HANDOFF_READY
→ DEPENDENCY_RELEASED → REGRESSION_REQUIRED → PRODUCT_ACCEPTANCE_PENDING
→ ACCEPTED → FROZEN
```

阻塞/异常是独立表达，不是 Lane 主状态：`BLOCKED`、`CONFLICT`、`UNKNOWN_DIRTY`、`CANDIDATE_MISMATCH`、`EVIDENCE_STALE`、`HARNESS_FAILURE`。Lane 只可写 `IMPLEMENTATION STATUS`、`OWN-SCOPE TEST STATUS`、`BUILD STATUS`、`HANDOFF READINESS`、`PRODUCT ACCEPTANCE STATUS` 及上述异常记录。

Lane 不得写入或输出 `GATE STATUS`、`PRODUCT REGRESSION`、`UNRESOLVED UNKNOWN`、`CANDIDATE TREE MATCH`、`COMMIT ELIGIBILITY`、`GLOBAL PASS`、`RELEASE READY`；这些字段只能由 `coordinatorScope` 对应的 Coordinator 写入。所有状态值中的 PASS 必须带类型：`IMPLEMENTATION_PASS`、`OWN_SCOPE_PASS`、`REGRESSION_PASS`、`PRODUCT_ACCEPTANCE_PASS` 或 `INTEGRATION_PASS`。裸 `PASS`、`ALL PASS`、`GLOBAL PASS`、`COMMIT ELIGIBLE` 必须明确拒绝。

R2 的 `active=true` 且 `coordinatorScope=null` 状态仍可 JOIN、status 和继续使用，不得被直接废掉。显式 `migrate-active -CoordinatorScope <scope>` 执行 R2 → R3 迁移，保留 Wave、HEAD、dirty fingerprint 与已有 Lane 状态，写入 `schemaVersion=XYE-HANDOFF/3` 和 `authorityModel=R3-LANE-COORDINATOR`；迁移前不得覆盖已有 Coordinator Scope。

## 2. Canonical Workspaces

两台电脑允许使用不同绝对路径：

```text
私人电脑
D:\MyDoc\project-vsCode\XuanyuEngine

工作电脑
E:\MyDoc\project-VSCode\XuanYuEngine
```

盘符不同是预期行为。要求是每台机器只使用自己的一个 canonical workspace。

## 3. Cross-machine authority

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

如果本地没有独有正式 Commit，但落后于当前 branch upstream：

```text
Ahead = 0
Behind > 0
clean → 只允许 fast-forward
dirty → HANDOFF BLOCKED
```

程序必须遵守：

- 禁止 `reset --hard`、`clean -fd`、stash 或其它会丢弃本地内容的自动操作；
- working tree clean 时只允许 `merge --ff-only <upstream>`；
- working tree dirty 时返回 `DIRTY_BEHIND_REMOTE`，保留全部 tracked / untracked 内容并等待人工处理；
- fast-forward 后再次验证 Ahead / Behind = 0 / 0；
- 分支历史若无法 fast-forward，则阻断，不自动 merge / rebase。

### 3.3 Local 与 Remote 同一 Tip（仅 PREPARE）

如果 Ahead = 0、Behind = 0，但工作区仍 dirty，则 PREPARE 禁止静默删除：

```text
same tip + dirty
→ HANDOFF BLOCKED
```

这是为了避免 PREPARE 删除当前机器尚未跨设备交接的即时工作；JOIN 不受此规则阻断。

## 4. Active Branch

当前权威开发分支来自 live Git：`git branch --show-current`；远端事实来自该 branch 的 upstream，默认回退为 `origin/<CurrentBranch>`。Ahead/Behind 永远比较 `HEAD...upstream`，不得比较持久化的旧 ActiveBranch 或旧 WIP 分支。

## 5. SDK / Resolver

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

## 6. JOIN 基线与 Lane 规则

JOIN 必须只读取 Git 事实：`rev-parse`、`branch --show-current`、`status`、`worktree list` 及正式 Resolver / Bootstrap。它不得执行 fetch、pull、reset、clean、checkout、switch、merge、rebase、stash、commit 或 push；Git 成败只看真实 Exit Code。

JOIN 必须允许 dirty；Active Wave 只有在 Branch 改变或 `baselineHead` 不是当前 HEAD 的祖先时，才以 `BASELINE_MOVED` 阻断。缺失或已关闭 state 时，只要 canonical workspace、当前 branch upstream、HEAD/Remote 0/0、无 staged conflict 与 ownership conflict，即返回 `HANDOFF JOIN PASS` 和 `Wave: STATELESS / READY`。输出必须包含 `DirtyFiles`、`Scope`、`OwnDirty`、`ForeignDirty` 与 `GitMutation: NONE`。XYUI / XYE / GOVERNANCE 可按路径归属报告 OwnDirty 与 ForeignDirty；INTEGRATION 无法可靠推断时必须输出 `OwnDirty: UNKNOWN`、`ForeignDirty: INFORMATIONAL`，不得把 ForeignDirty 当作 OwnershipConflict。

development 下所有 Lane JOIN 可通过 dirty。convergence 且 `coordinatorScope=xye` 时，`join --scope xyui` 返回 `HANDOFF BLOCKED` 与 `CONVERGENCE_EXCLUSIVE`；`join --scope xye` 与 XYE Coordinator status 允许。

## 7. PREPARE / LANE CLOSE / GLOBAL CLOSE

Active Wave 存在且未过期时 PREPARE 返回 `ACTIVE_WAVE`，不得同步。旧 Closed Wave 不阻塞新的普通 PREPARE。

Lane 完成与 Workspace Wave 关闭是两个不同动作。普通 Lane 必须使用 `handoff.cmd lane-close --scope <lane>`；它只将该 Lane 置为 `FROZEN`，释放该 Lane 的 Task、Ownership 和 Work Release，不得写入 Workspace `active=false`，也不得影响其他 Lane。重复 `lane-close` 返回幂等成功；Wave 已关闭时返回 `NO_ACTIVE_WAVE`，不得篡改历史。

全局 `handoff.cmd close --scope <coordinator>` 只允许 `state.coordinatorScope` 匹配的 Coordinator。关闭前必须确认无 Active Lane、Active Task、未关闭 Task Registration、Ownership/Work Release、Commit Mutex；任一仍存在均拒绝。非 Coordinator 返回 `COORDINATOR_REQUIRED`，仍有 Lane 返回 `ACTIVE_LANES_EXIST`，仍有 Task 返回 `ACTIVE_TASKS_EXIST`，资源未释放返回 `OWNERSHIP_NOT_RELEASED`，锁被持有返回 `COMMIT_MUTEX_HELD`。只有全部资源释放且 working tree clean 时才将 Active 标记为 false；dirty 时返回 `DIRTY_ON_CLOSE`。

`prepare` 默认登记 `mode=development`；WIP Resume 必须通过本次命令显式提供 `-SourceBranch` 与 `-TargetBranch`，并只登记到 state。Control Plane Maintenance 仅在 canonical、clean、当前 branch/upstream 0/0 时通过，且只能修复 `tools/handoff/**` 与治理文档/测试。Active Wave 期间再次 PREPARE 永远阻断，已满足 expiry condition 的 WIP 除外。

## 8. Legacy worktrees

历史 worktree 是治理债务，不是当前跨电脑交接阻断条件。

交接程序只报告 worktree 数量，不自动删除。

禁止为新的并行开发继续创建额外 worktree；当前并行开发使用：

```text
一个 canonical workspace
+
文件 ownership / mutex 边界
```

## 9. Self Test / Dry Test

`powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File tools\handoff\handoff.selftest.ps1` 在系统临时目录创建隔离 Git fixture，覆盖祖先 baseline 下的并行 JOIN PASS、Commit Mutex 争抢/owner 校验/安全解锁、commit 后 advance 自动释放、dirty 保留、XYUI convergence `CONVERGENCE_EXCLUSIVE`、JOIN/status 无 Git mutation、Active Wave prepare BLOCKED、dirty close BLOCKED。`state-lifecycle.selftest.ps1` 额外覆盖 live upstream、旧 WIP expiry、stateless JOIN、Clean Fast-Forward、Dirty-Behind 安全阻断、Local Ahead 安全保护与 maintenance gate；测试结束删除 fixture，不污染 canonical Workspace。

### 9.1 Candidate-Scoped Final Evidence Gate

`ACTIVE TASKS`、Repository `TEMP` 与 Repository `UNKNOWN TEMP` 仅为信息字段，不得单独阻断 Candidate。若 `.git\xye-handoff\candidate.json` 存在，Handoff 以其中的 Candidate Files、Build/Test/Runtime Dependencies、Truth/Registry/Harness Files 构造 Dependency Closure；只有命中该闭包的 ACTIVE writer、Consumed Active Foreign TEMP、UNKNOWN TEMP、OWNERSHIP CONFLICT、未吸收的 FRIENDLY RELEASED TEMP 或 Freeze Fingerprint 变化才可使 `FINAL EVIDENCE ELIGIBILITY` / `CERTIFICATION ALLOWED` 为 `NO`。Candidate 指纹只覆盖该闭包。未配置 manifest 时只输出 `NOT_CONFIGURED`，不得把仓库计数伪装成 Candidate 结论。

用户层使用 `TEMP FILES`、`OWNED TEMP`、`FRIENDLY ACTIVE TEMP`、`FRIENDLY RELEASED TEMP`、`LEGACY TEMP`、`UNKNOWN TEMP`、`CONFLICT TEMP`；`DirtyFiles`、`ForeignDirty`、`UnknownDirty` 仅作为兼容字段，不拥有最终 Candidate Gate 裁决权。Handoff 不代表 XYT Truth-Reviewed、XYT Certified 或 XYT Pass。

## 10. Successful final state

`HANDOFF JOIN PASS` 必须满足：

- 当前目录属于登记的 canonical workspace；
- 当前分支 = state.json 登记 Branch；
- Local HEAD = state.json 登记 baselineHead；
- Working Tree 可 clean 或 dirty；
- SDK 已由正式 Resolver 解析；
- `dotnet --version` 可执行；
- Repository Bootstrap = READY。

任何一项不满足都不得宣称交接完成。
