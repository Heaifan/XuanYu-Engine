# XuanYuEngine Workstation Handoff Protocol

## 1. Purpose

本协议是 XuanYuEngine 每个开发会话的强制启动入口。

每个 Workspace / Development Wave 由 Coordinator 执行一次 `prepare`；同一 Workspace 的其他并行 Session 执行只读 `join`。`join` 允许工作区因其他 Ownership Task 而 dirty，不执行 Git 同步。

模式入口：

```text
tools\handoff\handoff.cmd prepare  # 新 Wave / 跨电脑接管，可同步 Git
tools\handoff\handoff.cmd join --scope xye|xyui|integration|governance
                                     # 并行 Session，只读，允许 dirty
tools\handoff\handoff.cmd status   # 只读状态
tools\handoff\handoff.cmd close    # 收口后关闭 Active Wave
```

无参数等价于 `join --scope xye`。状态登记在 `.git\xye-handoff\state.json`，不进入 Git；JOIN 不在缺失 state 时创建它。

`state.json` 的 Wave 字段为：`mode=development|convergence`、`coordinatorScope=xye|integration|governance|null`。Coordinator 可在 PREPARE 时登记 Convergence 模式；普通 Agent 不得直接修改 state。

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

如果当前 Local HEAD 相对 Active Remote 存在 Remote 没有的正式 Commit：

```text
Ahead > 0
→ HANDOFF BLOCKED
```

交接程序不得自动覆盖这些 Commit，必须报告独有 Commit 后等待人工处理。

### 3.2 Local Ahead = 0 且 Behind > 0（仅 PREPARE）

如果本地没有独有正式 Commit，但落后于 Active Remote：

```text
Ahead = 0
Behind > 0
→ REMOTE WINS
```

此时本地未提交 tracked 修改和 untracked 残留不得阻断交接。程序必须：

- 丢弃 tracked 未提交修改；
- 删除 non-ignored untracked 文件 / 目录；
- 切换到 Active Branch；
- 将 Local HEAD 精确对齐 `origin/<ActiveBranch>`；
- 再次验证 Ahead / Behind = 0 / 0；
- 验证 working tree clean。

### 3.3 Local 与 Remote 同一 Tip（仅 PREPARE）

如果 Ahead = 0、Behind = 0，但工作区仍 dirty，则 PREPARE 禁止静默删除：

```text
same tip + dirty
→ HANDOFF BLOCKED
```

这是为了避免 PREPARE 删除当前机器尚未跨设备交接的即时工作；JOIN 不受此规则阻断。

## 4. Active Branch

当前权威开发分支由：

```text
tools/handoff/HandoffConfig.psd1
```

中的 `ActiveBranch` 指定。

当前值：

```text
feat/v0.3-world-authoring-r1
```

开发主线迁移时必须在正式 Git 节点中更新此配置。

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

JOIN 必须允许 dirty；只有 Branch 或 HEAD 偏离 `.git\xye-handoff\state.json` 时以 `BASELINE_MOVED` 阻断。输出必须包含 `DirtyFiles`、`Scope`、`OwnDirty`、`ForeignDirty` 与 `GitMutation: NONE`。XYUI / XYE / GOVERNANCE 可按路径归属报告 OwnDirty 与 ForeignDirty；INTEGRATION 无法可靠推断时必须输出 `OwnDirty: UNKNOWN`、`ForeignDirty: INFORMATIONAL`，不得把 ForeignDirty 当作 OwnershipConflict。

development 下所有 Lane JOIN 可通过 dirty。convergence 且 `coordinatorScope=xye` 时，`join --scope xyui` 返回 `HANDOFF BLOCKED` 与 `CONVERGENCE_EXCLUSIVE`；`join --scope xye` 与 XYE Coordinator status 允许。

## 7. PREPARE / CLOSE

Active Wave 存在时 PREPARE 返回 `ACTIVE_WAVE`，不得同步。CLOSE 仅在 working tree clean 时将 Active 标记为 false；dirty 时返回 `DIRTY_ON_CLOSE`。

`prepare` 默认登记 `mode=development`；Convergence Coordinator 可显式登记 `mode=convergence` 和 `coordinatorScope`。Active Wave 期间再次 PREPARE 永远阻断。

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

`powershell.exe -NoLogo -NoProfile -ExecutionPolicy Bypass -File tools\handoff\handoff.selftest.ps1` 在系统临时目录创建隔离 Git fixture，覆盖 dirty JOIN PASS、XYUI development PASS、XYUI convergence `CONVERGENCE_EXCLUSIVE`、无参等价 XYE、JOIN/status 无 Git mutation、Active Wave prepare BLOCKED、dirty close BLOCKED；测试结束删除 fixture，不污染 canonical Workspace。

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
