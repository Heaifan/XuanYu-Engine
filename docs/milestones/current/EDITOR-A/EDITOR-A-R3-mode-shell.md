# EDITOR-A-R3 · Manage / Edit Mode & Initial Shell Layout

**状态**：IMPLEMENTED · AUTOMATED GATES PASS · READY FOR USER ACCEPTANCE

**分支**：`feat/EDITOR-A-workspace`

**版本**：`v0.2.26.2-rz`

**结构图**：[editor-a-r3-mode-shell.svg](editor-a-r3-mode-shell.svg)

## 1. 架构裁定

Shell 决定工作位置；Mode 决定管理或编辑；Workspace 决定正在编辑的数据类型。`EditorModeManager` 是 Manage/Edit 的唯一 Owner，默认 Manage；`EditorWorkspaceManager` 保留为 Map/Region 编辑目标，默认 Map。Manage 中显示“编辑目标：地图”，不会伪装为“地图编辑”；进入 Edit 后才显示“地图编辑”或“区域编辑”。

R2 的单一 Main/VulkanViewport、Workspace 身份、NO-OP、Camera/World/Selection 保留和 Region Drawing 隔离被复用。R2 的左右栏整体替换产品层级已由用户裁定失败，状态改为 `SUPERSEDED BY R3 MODE MODEL`；不创建 R2-F1。

## 2. Shell 与输入合同

Project/Hierarchy、Inspector、唯一 Main/VulkanViewport、资源浏览器和日志都是全局 Shell。底部资源浏览器默认可见，提供模型、材质、纹理、地图/其他分类文字与“导入 GLB”，直接复用既有文件选择和 GLB 导入链。右侧顶层“地图编辑器”已退役；Map 编辑上下文进入左侧“地图”Tab 和 Inspector 的现有 MapFormPanel。Region 编辑只显示 REGION-A 前的冻结占位。

`Esc` 永远只取消当前操作；`Tab` 在非 TextBox 焦点下切换 Manage/Edit。Mode 切换复用既有 `CancelActiveInput` 与 Region Drawing 取消路径，再回到选择工具，保留 World、SceneStateOwner、MapSession、Camera、Selection、Assets、Project 和唯一 Viewport。TextBox/重命名焦点不拦截 Tab。

## 3. 自动回归

R3 新增 `EditorModeManagerTests`（4 项）、`EditorModeUiTests`（7 项）和 `EditorModeUiCompositionTests`（7 项）。它们与 R1/R2 Workspace 合同共同覆盖：默认 Manage/Map target、Manage↔Map Edit、Region Edit、Esc 不退出、TextBox Tab 路由、默认 Select、Camera/World/MapSession/Selection 保留、唯一 Main/Viewport、常驻 Project/Inspector/Asset Browser、Map Context、Region 无 Draft 与旧右侧 Map Tab 退役。

## 4. 用户真机验收 IPO

| 序号 | 路径 | 输入 I | 过程 P | 输出 O |
|---|---|---|---|---|
| EA-R3-M01 | 启动编辑器 | 默认启动 | 观察顶部、左右与底部 | 显示“管理模式”“编辑目标：地图”、项目、视口、检查器、资源浏览器和日志入口；不显示已进入地图编辑。 |
| EA-R3-M02 | 底部 → 资源 → 导入 GLB | 不按 Tab | 点击“导入 GLB” | 进入既有 GLB 文件选择/导入流程。 |
| EA-R3-M03 | 管理模式 → Tab | 编辑目标为地图 | 按 Tab | 进入“地图编辑”，左侧地图 Context 与 Inspector 地图属性出现；资源、项目和 Viewport 仍存在。 |
| EA-R3-M04 | 地图编辑 → Tab | 已在地图编辑 | 按 Tab | 回到管理模式；Camera、Selection、Project、资源浏览器和 Viewport 不重建。 |
| EA-R3-M05 | 管理模式 → 编辑目标：区域 → Tab | 选择区域 | 按 Tab | 进入区域编辑；仅有 Region 占位，不出现 Drawing、Draft、顶点或假列表。 |
| EA-R3-M06 | 编辑模式 → Esc / Tab | 已进入 Map 或 Region 编辑 | 先按 Esc，再按 Tab | Esc 只取消操作且仍留在编辑模式；Tab 才返回管理模式。 |
| EA-R3-M07 | 连续切换 | 管理↔地图编辑 10 次，再管理↔区域编辑 10 次 | 重复 Tab 与目标选择 | 无黑屏、相机复位、Viewport 重建、面板重复、日志爆量或 Selection 丢失。 |
| EA-R3-M08 | 1024×640 最小窗口 | 将窗口缩至最小值 | 检查各区域 | 项目、Inspector、资源入口可达，Viewport 未被挤死，顶部不异常多行，底部不遮挡主视口。 |

## 5. 门禁证据

- Solution Build：0 Warning / 0 Error；Core.Tests：339/339 PASS；World.Tests：1154/1154 PASS；WarCore.Tests：22/22 PASS。
- 聚焦 R1/R2/R3 Workspace/Mode：39/39 PASS；ARCH-A、5+100、四处 `v0.2.26.2-rz` 一致性、三个 EDITOR-A SVG XML 与 `git diff --check`：PASS。
- 提交、推送与远端等值核验将在本轮结束时补入真实 Hash。自动门禁不替代本报告第 4 节的用户真机 IPO。`_tmp_blind_rows/` 为既有未跟踪目录，本轮未读取、未修改、未纳入提交。
