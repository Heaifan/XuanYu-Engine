# XYE / XYUI Dual-Lane Development Model

**类型：** DEC  
**状态：** ACTIVE  
**批准日期：** 2026-09-27  
**适用范围：** XuanYuEngine canonical workspace、XYUI `xyui/`、Handoff、Git/GitHub 与共享编译验证

## 决策

玄域引擎采用 One Workspace / One Current Branch / Dual Lane：

```text
P0   XYE FAST LANE
P0.5 XYE × XYUI INTEGRATION
P1   XYUI CONTROLLED SUPPORT LANE
```

优先级为 `XYE > INTEGRATION > XYUI`。XYUI 继续开发、审计、进入同一 Git/GitHub；普通 XYUI 失败不得阻断 XYE。只有 XYUI Runtime / Public Contract 实际破坏 XYE Consumer 时，才升级为 P0.5 Integration Blocker。

## 合同

- 共享 Workspace 只有一个当前分支和一个 HEAD；普通 Agent 不得切换 / 创建 Branch，不得创建额外 Worktree。
- XYE 为永久 P0 FAST LANE；XYUI 默认只拥有 `xyui/**`，默认禁止自行 Commit、Push、Branch mutation。
- Integration 必须声明 XYE Consumer、XYUI Runtime/Public Contract 与 Ownership 文件表。
- Lane-scoped Build/Test 允许并行，但共享资源发生冲突时 XYE 优先；Compile-RED 必须恢复 Green。
- Convergence 时 Coordinator 独占 Workspace，其他 Lane Freeze；XYUI 经 Audit PASS 后由 Coordinator 批量 Stage、原子 Commit、Push、Remote Verify。
- ForeignDirty 不等于 OwnershipConflict；只要写入范围不重叠，其他 Lane dirty 不阻断 JOIN。

## Rationale

该模型拒绝四种会放大返工的做法：拒绝把 XYUI 排除在正式 Git 之外，拒绝用双 Branch / 多 Worktree 隐藏真实冲突，拒绝让普通 XYUI 失败阻断 XYE，拒绝把视觉问题未经 Consumer 证据升级为 Integration。它保留 XYUI 的独立迭代速度，同时让跨 Lane 公共契约、共享编译和真实 Consumer 破坏拥有明确升级路径。

## 事实源

Lane 的 Priority、Ownership、Allowed、Prohibited、Audit、Convergence 和 Cross-Lane Escalation 以 `docs/governance/development-lanes.md` 为唯一事实源；Handoff 的机器状态以 `.git/xye-handoff/state.json` 为事实源。
