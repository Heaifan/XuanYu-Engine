# EDITOR-A-R3-F1 · Shell Compact & Unified Mode Selector

**状态**：IMPLEMENTED · AUTOMATED GATES PASS · READY FOR USER ACCEPTANCE

**分支**：`feat/EDITOR-A-workspace`

**版本**：`v0.2.26.3-rz`

## 1. 冻结结果

Manage 顶部仅保留一个“管理模式”控件：单击 NO-OP，双击与非 TextBox 焦点的 Tab 共同调用 `ToggleEditorMode()`，进入当前 Workspace。Edit 时，同一位置原地显示“地图编辑”或“区域编辑”及 `EditorIcons.axaml` 的 Chevron；主区域双击返回 Manage，Chevron 菜单以 Radio 项直接切换 Map/Region，不先退回 Manage。

`BottomDockHost` 已删除，`UiRoot` 底部只保留现有 `Foot`。日志默认折叠，保留既有筛选、列表、详情与 GridSplitter 合同；“文件 → 导入 GLB”不受影响。Mode 与 Workspace 继续共享 World、SceneStateOwner、MapSession、Camera、Selection、Project、唯一 Main 和唯一 VulkanViewport。

## 2. 自动回归

R1/R2/R3/F1 聚焦回归当前为 41/41 PASS，覆盖：底部资源浏览器退役、唯一 Foot/Main/Viewport、默认 Manage/Map、无重复模式入口、Edit 菜单及 Chevron、Tab/双击共同路由、Esc 与 TextBox Tab 合同、Map/Region 直接切换、Tool=Select、World/Camera/Selection/MapSession 保留与 Region 无 Draft。

## 3. 用户真机验收 IPO

| 序号 | 路径 | 输入 I | 过程 P | 输出 O |
|---|---|---|---|---|
| F1-U01 | 启动编辑器 | 默认启动 | 检查 Shell | 仅见“管理模式”、项目/层级、Viewport、检查器/调试和 Log；无资源浏览器、编辑目标或额外 Mode 按钮。 |
| F1-U02 | 底栏 | 默认启动 | 观察空间 | Log 约 32 DIP，Viewport 获得最大剩余高度。 |
| F1-U03 | 管理模式 | 单击、双击 | 先单击再双击控件 | 单击 NO-OP；双击进入当前 Map 编辑且 Camera/Viewport 不重建。 |
| F1-U04 | 地图编辑 | 点击 Chevron | 选择“区域编辑” | 直接进入区域编辑，仍为 Edit；菜单含 Map/Region Radio 选中态。 |
| F1-U05 | 地图/区域编辑 | 双击主区域 | 双击当前 Mode 文本 | 返回 Manage；当前 Workspace 仅内部保留。 |
| F1-U06 | 编辑输入 | Esc、Tab、TextBox Tab | 依次操作 | Esc 不退出 Edit；Tab 返回 Manage；TextBox Tab 不切 Mode。 |
| F1-U07 | 连续切换 | Map↔Manage 10 次、Map↔Region 10 次、Region↔Manage 10 次 | 重复切换 | 无黑屏、闪烁、Camera/Selection 丢失、Viewport 重建、Panel 重复或 Log 爆量。 |
| F1-U08 | 文件 | 选择“文件 → 导入 GLB” | 打开菜单并点击 | 进入原有 GLB 文件选择/导入链。 |
| F1-U09 | 1024×640 | 缩小窗口 | 检查三栏与底栏 | Left、Viewport、Inspector 可用，Log 紧凑，顶部受控且 Viewport 保持最大剩余区。 |
| F1-U10 | Region 编辑 | 进入 Region | 检查上下文 | 仅 Region 占位；无 Drawing、Draft、顶点、列表或假属性。 |

## 4. 门禁证据

- Solution Build：0 Warning / 0 Error；Core.Tests：339/339 PASS；World.Tests：1156/1156 PASS；WarCore.Tests：22/22 PASS；R1/R2/R3/F1 聚焦：41/41 PASS。
- ARCH-A、5+100、四处 `v0.2.26.3-rz` 一致性、三个 EDITOR-A SVG XML 与 `git diff --check`：PASS。受控 AXAML 扫描事实由 26 变为 25（-1），原因是删除重复 `BottomDockHost.axaml`。
- 提交、推送与远端等值将在本轮结束时以真实 Hash 回填。自动门禁不代替上表真机验收；通过前不得 CLOSED。`_tmp_blind_rows/` 是既有未跟踪目录，本轮不读取、不修改、不提交。
