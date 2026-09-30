# Handoff Task Flight Plan Lifecycle Contract

状态：ACTIVE · Lane：GOVERNANCE-C · HANDOFF-TASK-FLIGHT-PLAN-C3

## Canonical workspace authority

`BOOTSTRAP CWD IS NOT EXECUTION AUTHORITY.`

`Repository work must relocate to the canonical workspace before candidate inspection, testing, or modification.` 唯一 canonical workspace 是 `E:\MyDoc\project-VSCode\XuanYuEngine`；Codex Worktree 只能作为非权威参考源。任何来自非 canonical checkout 的文件存在性、Git 状态或测试结果均不得作为 Candidate 证据。

本契约只约束 Task Flight Plan Registry 生命周期与 Dirty provenance；A/B 实现文件和 `tools/handoff/handoff.ps1` 不属于本 lane 所有权。

## Lifecycle contract

```text
REGISTERED -> ACTIVE -> release -> RELEASED -> close -> CLOSED
```

- `Task completion MUST release its ACTIVE ownership.`
- release 后 `ACTIVE TASKS` 不得包含该 Task；`inspect` 必须仍能看到 `RELEASED`。
- RELEASED Task 必须继续解释其 WriteScope 对应的 dirty provenance；本契约使用 `task-dirty-classifier.ps1` 验证 `FRIENDLY_RELEASED_DIRTY`。
- close 只允许吸收 RELEASED candidate；CLOSED Task 必须从 Live Registry 清除，同时在 history 中保留 TaskId、状态事件与 CloseReason。
- `COMPLETE + ACTIVE = TASK_STATE_LEAK`，这是本事故的专门回归条件。

## Interrupted ownership

Agent 中断而未 release 时，Task 必须保持 `ACTIVE` 并继续占用 WriteScope。不得通过 TTL、timeout 或进程退出自动 release。只有显式提供 Coordinator 与 Reason 的 `reap` 才能释放 ownership，并以 `CloseReason=ABANDONED/<reason>` 写入历史；缺少 Coordinator 的 reap 必须拒绝。

## Dependency provenance

当 `SRP` 的 ACTIVE WriteScope 为 `XuanYu.Render.Vulkan/**`，`XYT` 声明依赖该 scope 且 dirty 命中时，分类必须为 `FRIENDLY_ACTIVE_DEPENDENCY`。SRP release 后同一 dirty 必须为 `FRIENDLY_RELEASED_DIRTY`，不得变成 `UNKNOWN_DIRTY`、`OWNERSHIP_CONFLICT` 或 `ORPHAN`。

## Terminal protection

CLOSED Task 不得重新 amend 或 write；拒绝操作不得恢复其 Live Registry 记录，也不得改变历史。RELEASED 是 provenance 状态，不得混入 ActiveTasks 计数。

## UNKNOWN rule

> UNKNOWN modification may only be declared after
> Active/Released Task Registry and dependency ownership
> cannot explain the modification.

因此，Coordinator 必须先检查 Active/Released Registry、WriteScope、Dirty provenance 与 dependency ownership；可由这些事实解释的修改不得宣告 UNKNOWN。

## Verification boundary

独立回归入口：`tools/handoff/task-flight-plan-lifecycle.selftest.ps1`。它必须调用 canonical A/B 真实脚本，不得用 mock 或内嵌 registry model。另行执行 `task-flight-plan.selftest.ps1`、`task-dirty-classifier.selftest.ps1`、PowerShell 5.1 parse、pwsh test、`git diff --check`，并保持 `5+100`：相关 `task-flight-plan*` 文件单文件不超过 100 行。
