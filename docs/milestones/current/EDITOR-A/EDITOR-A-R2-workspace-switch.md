# EDITOR-A-R2 · Workspace Switch UI

**状态**：IMPLEMENTED · AUTOMATED GATES PASS · READY FOR USER ACCEPTANCE

**分支**：`feat/EDITOR-A-workspace`

**版本**：`v0.2.26.1-rz`

**结构图**：[editor-a-r2-workspace-switch.svg](editor-a-r2-workspace-switch.svg)

## 1. 实装结果

第二行工具栏起始位置提供标准 Menu Workspace Selector：地图编辑、区域编辑；Map 为默认选中，区域为标准单选状态。`UiVm` 持有唯一 `EditorWorkspaceManager`，`CurrentWorkspace`、显示名和 Map/Region 布尔绑定均由该实例派生。

目标未变时立即返回：不取消输入、相机、工具、Selection、MapSession、World 或 Viewport，且不写日志。目标改变时复用 `CancelActiveInput("切换工作区")`，调用 Manager，再以无重复工具日志方式选择“选择”，更新绑定并记录一条工作区低频日志。

## 2. 布局与不变量

`UiRoot` 的 Left/Right 改为 `WorkspaceLeftHost`/`WorkspaceRightHost`；Map 仍嵌入既有 `Left`/`Right`（包含 MapEditorPanel）。Region Left 只显示“区域 / 当前仅开放 Workspace 切换 / 区域列表将在 REGION-A 接入”；Right 只显示“区域属性 / 当前未选择区域 / REGION-A 接入后显示正式属性”。

`Main` 仍只在 `UiRoot` 创建一次，`Main.axaml` 仍只创建一次 `VulkanViewport`；两个 Host 不含 Main 或 Viewport。切换保留 Camera position/right/up/forward/revision、MapSession、SceneStateOwner 与 Selection，且不会激活 Region Drawing 或 Draft。

Region Workspace 仅开放选择、聚焦、查看全部、平移、环绕与环境；框选、移动、旋转、缩放、吸附仅在 Map Workspace 显示。它不实现 REGION-A、区域列表、区域属性编辑、Region Drawing、map.json、Dataset Registry 或任何 Renderer/Picking 改写。

## 3. 自动回归

`EditorWorkspaceUiTests`（7 项）覆盖默认 Map、双向切换、NO-OP、选择工具复位、Camera/MapSession/World Owner 保留、Selection 往返与 Region 无 Draft。

`EditorWorkspaceUiCompositionTests`（6 项）覆盖唯一 Main、唯一 VulkanViewport、Host 无 Main/Viewport、Map Left/Right 可达、Region 占位边界及选择器/Map-only 工具可见性。加上 R1 `EditorWorkspaceManagerTests` 8 项，聚焦总数为 21。

## 4. 真机验收 IPO

| 序号 | 路径 | 输入 I | 过程 P | 输出 O |
|---|---|---|---|---|
| EA-R2-01 | 顶部第二行 | 点击“地图编辑”菜单 | 观察初始选中与菜单 | 地图编辑为初始单选状态。 |
| EA-R2-02 | 地图 → 区域 | 选择“区域编辑” | 检查左右区和工具栏 | 左右只显示冻结占位；Map-only 工具隐藏；视口未闪烁/重建。 |
| EA-R2-03 | 区域 → 地图 | 选择“地图编辑” | 检查项目/层级、检查器、地图编辑器 Tab | 既有 Map UI 完整恢复可用。 |
| EA-R2-04 | 重复选择 | 在当前工作区再次选择当前项 | 观察相机、选择、Footer/日志 | 无相机复位、无工具变化、无新增工作区日志。 |
| EA-R2-05 | 视口状态 | 先选择对象、移动相机、切到区域再切回 | 观察对象/视角/导航 Gizmo | Selection 与相机位置/朝向保持；Main/Viewport 不重建。 |
| EA-R2-06 | 区域边界 | 在 Region Workspace 操作工具栏和左右区 | 检查可见项 | 不出现 Region Drawing、草稿、虚假区域列表或属性编辑。 |

用户真机验收尚未执行；自动门禁不能替代上述 IPO，也不能把本轮标为 CLOSED。

## 5. 门禁证据

- Solution Build：0 Warning / 0 Error；
- Core.Tests：339/339 PASS；World.Tests：1136/1136 PASS；WarCore.Tests：22/22 PASS；
- 聚焦 Workspace：21/21 PASS（R1 8 + R2 13）；
- ARCH-A、5+100、四处 `v0.2.26.1-rz` 一致性、两个 EDITOR-A SVG XML 与 `git diff --check`：PASS；
- 首次 World 全量门禁仅因扫描范围计数尚为 24 而报 24≠27；经确认新增 Selector/LeftHost/RightHost 三个 AXAML 均在扫描范围内，将基线更新为 27 后重建重跑 1136/1136 PASS。

提交、推送与远端 tip 核验会在本轮紧随这些门禁完成。`_tmp_blind_rows/` 为本轮开始前已存在的未跟踪目录，未读取、未修改、未纳入提交。
