# 9.1C-R：TopArea SVG 视觉细修

> 状态：已完成
> 范围：仅顶部区域（TopArea）视觉样式，不碰底部日志、Inspector、Docking、Gizmo、输入链路。

## 背景

9.1C 将顶部工具栏图标替换为自定义 SVG 后，功能方向正确，但视觉比例仍偏粗：

- 图标偏大（24px），压迫感强；
- 普通状态图标过白，抢占 Viewport 注意力；
- 工具按钮边界和 hover/selected 状态不够统一；
- Play/Stop 的绿色强调和 Reset Layout 的蓝色过于抢眼。

## 目标

让顶部从“功能正确但偏粗”提升到“克制、统一、有编辑器质感”。

## 修改内容

### 1. 图标尺寸统一降至 16px

- 所有顶部 PathIcon 从 `Width/Height="24"` 改为 `Width/Height="16"`。
- 这是用户指定上限（18px）之下的默认尺寸，保证精致感。

### 2. 普通状态降亮度

- 普通图标：`#9CA3B0`（浅灰）。
- 普通文字：`#D8DDE4`（浅灰白）。
- hover 图标：`#C4CBD6`（轻微提亮）。
- 选中工具：白色图标 + 白色文字 + 蓝色按钮背景 `#2563EB`，hover `#3B82F6`，pressed `#1D4ED8`。

### 3. 统一按钮感

- Row 1 命令按钮与 Row 2 工具按钮统一圆角 `CornerRadius="3"`。
- 统一高度：padding 改为 `8,4`（Row 1）和 `6,4`（Row 2），内容高度约 24px，适配 29px 行高。
- 移除局部 `Foreground="#FFFFFF"`，改由样式集中控制图标颜色。

### 4. 分组间距取代竖线

- Row 2 移除原来的 `|` 分隔符。
- 改为外层 `Spacing="12"` + 内层组 `Spacing="3"`，用留白做分组：
  - 选择/移动/旋转/缩放
  - 世界/本地
  - 吸附/网格
  - 运行/停止（靠右）

### 5. Play/Stop 与 Reset Layout 收敛

- `SimulationButton` 样式移除，`运行` 与 `停止` 统一使用 `TopCommandButton`，视觉重量与左侧命令按钮一致。
- `重置布局` 保留图标 + 文字，但图标降为浅灰，不再像主 CTA。

### 6. Row 2 工具统一为 `EditorToolButton`

- 世界/本地、吸附、网格从 `TopCommandButton` 改为 `EditorToolButton`，与选择/移动/旋转/缩放同一套按钮语义。

## 修改文件

- `XuanYu.Engine.Editor.Windows/Panels/TopArea/EditorTopArea.axaml`
- `XuanYu.Engine.Editor.Windows/UI/Styles/TopAreaButtonStyles.axaml`

## 验证

- `dotnet build XuanYu.Engine.sln`：通过，0 警告 0 错误。
- `dotnet test XuanYu.Engine.Tests --filter "FullyQualifiedName~Architecture"`：17 项通过。
- 全量测试：`723` 通过，`1` 失败（`WorldHierarchyTreeBuilderTests.Build_MultipleEntities_OrdersByGroupThenDisplayName`，与本次修改无关，为既有排序测试问题）。
- 手动启动编辑器并截图：`artifacts/verify-top-icons-9.1C-R.png`。

## 遗留风险

- 若用户后续认为 16px 图标过小，可在 `TopAreaButtonStyles.axaml` 中统一调整到 18px，无需改多处 XAML。
- 当前未引入运行时主题切换，所有颜色为硬编码常量；后续如需主题系统，可把颜色抽取为资源字典。
