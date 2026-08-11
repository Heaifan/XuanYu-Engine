# EDITOR-A-R1 · Workspace Contract

**状态**：IMPLEMENTED · AUTOMATED GATES PASS · READY FOR USER ACCEPTANCE

**分支**：`feat/EDITOR-A-workspace`（从已验证的 MAP-A 收口 tip `3dd091f` 创建）

**版本**：`v0.2.26.0-rz`

## 1. 目标与范围

R1 只建立纯 `XuanYu.Editor` 的 Workspace 身份、不可变定义、唯一 Manager 和切换结果合同。已注册 Map Editor、Region Editor；Region Editor 仅为身份/插槽合同，不实现 Region Drawing。

允许修改：`XuanYu.Editor/Workspace/`、对应 World.Tests、版本四处、最小里程碑/索引/Backlog/changelog 文档。

禁止：Workspace 可见 UI、Toolbar/左右面板重排、删除旧 MapEditorPanel、Region Drawing、map.json/Dataset Registry、Renderer/Picking 重写、Terrain/Road/Unit Editor、第二份 World/Camera/Selection 权威状态。

## 2. 合同

| 类型 | 职责 |
|---|---|
| `EditorWorkspaceId` | Map Editor / Region Editor 的稳定身份。 |
| `EditorWorkspaceDefinition` | 中文显示名与 Toolbar、Left、Main、Right 插槽身份及默认 Tool。 |
| `EditorWorkspaceDefinitions` | 两个不可变注册定义；无 UI 控件创建。 |
| `EditorWorkspaceManager` | `CurrentWorkspace` 的唯一 Owner；提供 Enter、Leave、Switch。 |
| `EditorWorkspaceTransition` | 要求结束临时 Tool，并保留既有 World、Camera、兼容 Selection；不持有它们。 |

切换不变量：Map → Region、Region → Map 都返回 `Select`；切换与 Leave 由未来 Shell/UI 根据 `EndsTemporaryToolState` 结束临时状态；World、Camera、Selection 保留在其既有 Owner，不由 Workspace Manager 复制。

## 3. 自动回归

`EditorWorkspaceManagerTests` 覆盖：默认 Map Workspace、双向切换、重复切换无新状态、Enter/Leave、Context 保留标志、唯一 Manager 不持有 World/Camera、`XuanYu.Editor` 不引用 Vulkan、Region Workspace 没有 Region Drawing Tool。

## 4. 验收边界

本轮无可见 Workspace UI，故不存在可替代的 Viewport/Pointer 真机 IPO。用户验收以代码、自动测试、依赖边界、版本和 Git 证据为准；未来 Workspace UI 或 REGION-A 功能才建立中文 IPO 真机清单。

## 5. 禁止项确认

- [x] 未实现 Region Drawing 或 Region Workspace UI。
- [x] 未改变 World、Camera、Selection、Renderer、Picking 或 Schema 的权威所有者。
- [x] 未增加 UI / Vulkan 依赖。
- [x] 未将 MAP-A 的旧 F1 FAIL 改写为通过。
- [x] 未创建第二份 World / Camera / Selection 状态。

## 6. 正式门禁

- Solution Build：0 Warning / 0 Error；
- Core.Tests：339/339 PASS；
- World.Tests：1123/1123 PASS（含 EDITOR-A-R1 8/8）；
- WarCore.Tests：22/22 PASS；
- ARCH-A、5+100、宪法 2.2 版本字段、四处 `v0.2.26.0-rz` 一致性与 `git diff --check`：PASS；
- 核心代码 Commit：`4cabf42`；证据 Commit：`2b90a46`；本轮完成 Push 与 Remote HEAD 核验后进入用户验收。

## 7. 用户验收证据 IPO

本轮没有可见 Workspace UI；以下为用户对代码、状态与 Git 事实的核验，不把它伪装为 Viewport/Pointer 真机验收。

| 序号 | 路径 | 输入 I | 过程 P | 输出 O |
|---|---|---|---|---|
| EA-R1-01 | 文档 → MAP-A 战略收口 | 收口报告 | 核对 F1 状态 | 仍为非成功终态，未被改写为 PASS。 |
| EA-R1-02 | 文档 → Knowledge / Lessons / Backlog | 审计记录 | 核对分类 | K-ARCH-002、L-ARCH-001、REGION-A-MIG-001 与 Rejected 均可追溯。 |
| EA-R1-03 | Git → 当前分支 | `git branch --show-current` | 核对开发线 | 输出 `feat/EDITOR-A-workspace`。 |
| EA-R1-04 | 编辑器 → Workspace Contract | `EditorWorkspaceManager` | 读取定义 | Map Editor 与 Region Editor 均存在，默认 Tool 为 Select。 |
| EA-R1-05 | 自动测试 → Workspace | `EditorWorkspaceManagerTests` | 执行聚焦或全量 World.Tests | Map↔Region 双向与重复切换合同通过。 |
| EA-R1-06 | 代码 → Workspace Transition | 切换结果 | 核对标志 | 临时 Tool 结束，World/Camera/Selection 均声明保留。 |
| EA-R1-07 | 架构 → Editor 依赖 | ARCH-A / 回归 | 检查引用 | Workspace 不持有 World/Camera 且 Editor 无 Vulkan 引用。 |
| EA-R1-08 | Git → 本地/远端 | HEAD 与远端 tip | 推送后核对 | Local HEAD == Remote HEAD，Ahead/Behind = 0/0。 |
