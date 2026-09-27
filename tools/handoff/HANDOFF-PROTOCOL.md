# XuanYuEngine Workstation Handoff Protocol

## 1. Purpose

本协议是 XuanYuEngine 每个开发会话的强制启动入口。

在 `tools\handoff\handoff.cmd` 返回 `HANDOFF PASS` 之前，禁止开始代码修改、测试、运行、并行任务分配或其他开发操作。

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

### 3.1 Local Ahead > 0

如果当前 Local HEAD 相对 Active Remote 存在 Remote 没有的正式 Commit：

```text
Ahead > 0
→ HANDOFF BLOCKED
```

交接程序不得自动覆盖这些 Commit，必须报告独有 Commit 后等待人工处理。

### 3.2 Local Ahead = 0 且 Behind > 0

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

### 3.3 Local 与 Remote 同一 Tip

如果 Ahead = 0、Behind = 0，但工作区仍 dirty，则禁止静默删除：

```text
same tip + dirty
→ HANDOFF BLOCKED
```

这是为了避免删除当前机器尚未跨设备交接的即时工作。

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

## 6. Legacy worktrees

历史 worktree 是治理债务，不是当前跨电脑交接阻断条件。

交接程序只报告 worktree 数量，不自动删除。

禁止为新的并行开发继续创建额外 worktree；当前并行开发使用：

```text
一个 canonical workspace
+
文件 ownership / mutex 边界
```

## 7. Successful final state

`HANDOFF PASS` 必须同时满足：

- 当前目录属于登记的 canonical workspace；
- 当前分支 = Active Branch；
- Local HEAD = Active Remote HEAD；
- Ahead = 0；
- Behind = 0；
- Working Tree = clean；
- SDK 已由正式 Resolver 解析；
- `dotnet --version` 可执行；
- Repository Bootstrap = READY。

任何一项不满足都不得宣称交接完成。
