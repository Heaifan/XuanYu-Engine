# XYE / XYUI Development Lanes

> 唯一 Lane 事实源。本文定义共享 Workspace 内的优先级、Ownership、门禁、Git 与 Convergence；产品功能事实仍由各自领域代码持有。

## 1. Shared Workspace

- 一个 canonical Workspace、一个当前分支、一个 HEAD；并行依靠文件 Ownership，不创建额外 Worktree。
- 所有任务先声明 `Lane: XYE | XYUI | INTEGRATION | GOVERNANCE`，再读取对应 Authority owner 与 Candidate 依赖；不得调用旧 Handoff。
- `ForeignDirty != OwnershipConflict`。仅 Candidate 闭包、WriteScope 或实际依赖消费与 Foreign Dirty 重叠时才阻断；未知归属只暂停受影响路径。
- Coordinator / Convergence 事实由 `tools/governance/coordinator/**` 负责；旧 `prepare` / `join` / `close` 仅属 Legacy/Historical。

## 2. Priority and Ownership

| Lane | Priority | Ownership | Normal outcome |
|---|---:|---|---|
| XYE | P0 | Engine 产品、World/Editor/Renderer/Tests 及其任务明确列出的文件；不默认拥有 `xyui/**` | 永久 FAST LANE，优先恢复 Green |
| INTEGRATION | P0.5 | 明确列出的 XYE Consumer、XYUI Runtime/Public Contract 与必要测试 | 真实 Consumer 破坏才阻断；必须有 Ownership 文件表 |
| XYUI | P1 | 默认仅 `xyui/**`，含 Runtime/Gallery/Tests | Controlled Support；普通失败不得阻断 XYE |
| GOVERNANCE | 过程 Lane | 宪法、规则、Lane 文档、Handoff；不拥有产品实现 | 只改治理范围 |

## 3. Allowed / Prohibited

### XYE

允许修改声明的 Engine 文件、相关测试和必要治理记录；禁止以 XYE 名义覆盖 XYUI dirty、跨越未声明的公共 API/Schema 或制造第二事实源。

### XYUI

允许在 `xyui/**` 开发与审计；默认禁止自行 `git add`、Commit、Push、Branch mutation、额外 Worktree、跨出 `xyui/**` 的产品修改和把视觉推测写成 Consumer Blocker。

### INTEGRATION

必须先列出 Ownership 文件表、Consumer、公共契约与升级证据；不得用 Integration 名称绕过 XYE 优先级、Convergence Freeze 或真实失败证据。

### GOVERNANCE

允许维护本文件、宪法、执行手册、AGENTS 与 Handoff；禁止顺手施工产品代码、`xyui/**`、Terrain/Camera/Inspector/Renderer/XYUI4 或正式 ERR/EXP。

## 4. Build / Test

- Development 可按 Lane-scoped Build/Test 并行；共享编译资源冲突时 XYE 优先。
- `Compile-RED` 是共享编译红灯状态：造成红灯的 Lane 必须恢复 Green；XYUI 的普通失败不阻断 XYE。
- XYE 使用仓库正式 Resolver/Bootstrap 和适用 Gate；XYUI 至少验证 XYUI Build/Test、Gallery/Runtime consistency、5+100 与 diff check。
- Integration 追加受影响 XYE Consumer Build/Test、架构边界、Public Contract Diff 与专项回归。
- 自动门禁不能替代真实 XYE Consumer、Vulkan、输入、生命周期或人工视觉验收。

## 5. XYUI Audit

审计级别定义如下：

- **Audit-L**：Gallery、Visual、非 Public Runtime Contract；检查 Ownership、5+100、XYUI Build/Test、Gallery/Runtime consistency、diff check、Scope。
- **Audit-M**：Runtime 行为改变但无公共 API；追加 affected Runtime Tests 与 XYE Consumer smoke compatibility。
- **Audit-H**：Public API、Default Layout Contract 或全局组件行为改变；追加 Public Contract Diff、XYUI tests、受影响 XYE Consumer Build/Test、架构边界、5+100 与 diff check。
- **Audit-Integration**：Audit-H 已证明真实 XYE Consumer 破坏，或跨 Lane Runtime/Public Contract 无法保持兼容；追加 XYE Consumer Build/Test、Integration Ownership 文件表、Compile-RED 归因与 Coordinator Convergence 决策。

流程固定为：`Develop → Audit PASS → Freeze → Coordinator Stage → Atomic Commit → Push → Remote Verify`。相邻微修改可组成一个 Audit Batch，但不得跨越未声明 Ownership。

## 6. Cross-Lane Escalation

普通 XYUI Gallery/视觉失败保持 P1；Runtime 行为改变但无公共 API保持 P1/M；只有真实 XYE Consumer Build/Test、运行链或公共契约破坏才升级 P0.5。疑似影响先做最小 Consumer smoke compatibility，不以文件相邻或猜测升级。

## 7. Convergence

Coordinator 宣布 Convergence 后，Coordinator Authority 记录 `mode=convergence` 与 `coordinatorScope=xye|integration|governance`；该 Workspace 独占。若 `coordinatorScope=xye`，XYUI 的冲突写入暂停，XYE Coordinator 可继续其有界收口。Convergence 完成后按 Coordinator Authority 的 owner 操作关闭；dirty close 返回 `DIRTY_ON_CLOSE`。

## 8. Compile-RED Rule

共享编译红灯必须按真实失败路径恢复，不得删测试、弱化断言、伪造结果或把 XYUI 失败静默降级为成功。XYE P0 先保持可验证 Green；XYUI 若造成共享红灯，必须在 Coordinator 收口前修复或明确阻断证据。
