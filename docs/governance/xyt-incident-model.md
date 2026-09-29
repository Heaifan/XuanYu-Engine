# XYT-E Incident & Lock Control

## Scope

本模型只负责 XYT T0~T3 事故严重度、局部隔离、依赖锁定与恢复判定；不修改 `xyt.ps1`、Runner、Executor、Registry、Report 或产品代码。

## Severity

`T0` 表示系统性影响，立即通报并默认 `T0-LOCK`；`T1` 表示明显返工或重要模块受影响，当前报告顶部通报；`T2` 为普通问题，当前任务解决；`T3` 为轻微观察。

T1 遇到 `REWORK`、`SCOPE_EXPANSION`、`PARALLEL_BLOCK`、`FALSE_PASS`、`BAD_FIX_CHAIN` 或 `MAINLINE_IMPACT` 时自动升级为 T0。

## Lock decisions

依赖判定必须同时保留 File Ownership、Project Dependency、Capability Mapping、Actual Diff 证据。任一正向证据产生 `DEPENDENCY-LOCK`；证据完整且均为独立产生 `CONTINUE`；无法可靠判断产生 `DEPENDENCY-UNCERTAIN`，立即通报用户且不得自行放行或扩大锁定。

T0 默认局部隔离，普通施工禁止写入；Incident Investigation 只读；只有 Incident Fix Task 可写锁定区。用户明确授权输出 `USER OVERRIDE`，但仍需审计。

根因确认、受控修复、最小充分测试 PASS 后可解除 T0-LOCK。上游解除后，若无接口/行为/共享契约变化自动恢复；否则继续锁定受影响任务。开发阶段基础设施失败为 Fail-Open，产品状态为 `UNVERIFIED`；正式收口为 Fail-Closed。

## Usage

```powershell
pwsh -File scripts/governance/xyt-incident.ps1 -Action evaluate -InputPath incident.json
pwsh -File scripts/governance/xyt-incident.selftest.ps1
```
