# LAYER-A-R1 · 通用图层栏与编辑职责分离

**状态**：IMPLEMENTED · AUTOMATED GATES PASS · READY FOR USER ACCEPTANCE

**分支**：`feat/LAYER-A-layer-shell`

**版本**：`v0.2.26.5-rz`

## 1. 冻结裁定

项目回答资产，层级回答世界实例，检查器回答当前选择属性，图层回答当前专业编辑内容的分层组织。图层栏属于编辑模式通用能力；Map 编辑暂不伪造 Map Layer，Region 编辑只提供真实 Region Layer；Region Drawing 不在本轮恢复。

## 2. 实现结果

- `XuanYu.Editor/Layering/` 新增 UI 无关的 `IEditorLayerProvider` 与 `EditorLayerItem` 合同，包含读取、添加、删除、重命名、显隐、锁定、活动层和排序能力。
- 右侧新增编辑模式专用 `EditorLayerDock`：管理模式不显示；编辑模式将检查器/调试与图层按约 60/40 纵向分栏，支持分隔条和图层栏展开/折叠。
- Map 编辑右下显示真实空状态“当前地图暂无独立可编辑图层 / 地图级图层将在地图数据集架构接入”，不显示 Ground、Boundary 或 Region。
- Region 编辑复用现有 `MapEditSession` 和 `LayerPanel`，仅过滤 `MapLayerKind.Region`；检查器承载 `LayerInspectorPanel`，图层栏不再重复显示属性。
- Map 专业导航删除旧“图层”二级页和“区域绘制”入口；Workspace 切换清理旧图层选择。

## 3. 自动证据

- `XuanYu.Editor.UI` 定向 Build：0 Warning / 0 Error。
- `XuanYu.World.Tests` fresh Build 后 `1160/1160 PASS`。
- LAYER-A 聚焦合同：`LayerAUiCompositionTests` 3/3、`LayerARuntimeTests` 1/1；Map/Region provider 过滤、管理/编辑 Dock 可见性、空状态和 Inspector 联动合同通过。
- `scripts/arch-a-guard.ps1`：PASS；5+100 与 Editor/UI → Render 边界通过；`git diff --check`：PASS。
- 实现提交：`7255b85`（`feat(editor): add shared layer dock`），已推送并核验远端等值。

## 4. 真机验收 IPO

| 序号 | 路径 | 输入 I | 过程 P | 输出 O |
|---|---|---|---|---|
| LA-R1-M01 | 启动编辑器 | 默认启动 | 观察三栏与底部 | 管理模式显示项目/层级、世界视口、检查器/调试和日志；不显示图层栏。 |
| LA-R1-M02 | 管理模式 → 地图编辑 | 进入地图编辑 | 观察右侧与左侧地图页 | 右下出现真实 Map 空状态；无区域绘制、区域 1、旧地图图层二级页。 |
| LA-R1-M03 | 地图编辑 → 区域编辑 | 切换编辑目标为区域 | 观察右下图层栏 | 显示真实区域图层；不显示地面、边界。 |
| LA-R1-M04 | 区域图层基础操作 | 区域编辑 | 添加、删除、显隐、锁定、拖动排序 | 所有操作成功，继续走 `MapEditSession`。 |
| LA-R1-M05 | 图层 → 检查器 | 点击右下某区域图层 | 观察右上检查器 | 显示图层属性、名称、类型、顺序、图层 ID；列表下不重复属性表单。 |
| LA-R1-M06 | Workspace 循环 | Map ↔ Region 循环 10 次 | 观察选中态、Inspector、Viewport | 无旧图层残留、串线、重复面板、黑屏、Camera reset 或 Viewport recreate。 |
| LA-R1-M07 | Region 边界 | 区域编辑 | 检查左侧、视口和右侧 | 无区域绘制、Draft、顶点或假 Region List；只有区域图层管理。 |
| LA-R1-M08 | 最小窗口 | 1024×640 | 检查三栏、分隔条、折叠和日志 | 左侧、Viewport、检查器、图层栏均可用，整体布局不被上下分栏挤坏。 |

## 5. 收口条件

本报告的自动证据不替代上述真机验收。取得 LA-R1-M01～M08 全部用户 PASS、完成正式 Solution/Core/World/WarCore/版本/XML 门禁并远端核验后，才能将 LAYER-A-R1 标记为 `CLOSED`。

