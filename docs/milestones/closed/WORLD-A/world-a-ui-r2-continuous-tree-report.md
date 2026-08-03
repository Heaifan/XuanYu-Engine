# WORLD-A-UI-R2 Continuous Tree + Icon Refresh

版本：`v0.2.18.21-fix`

## 当前裁定

`WORLD-A-UI-R1` 功能实现已完成，但人工视觉验收不通过：树线只是逐行短竖线，父级树干没有从第一个子节点连续延伸到最后一个子节点中心。本轮进入 `WORLD-A-UI-R2`，只修 Project Tree / Hierarchy Tree 连续树线与确认图标，不改变 `GlobalWorld`、`Partition`、`WorldQuery`、`SpatialIndex`、Selection、Picking、Move、Undo 或 Redo 事实语义。

## 完成项

| 项 | 结果 |
| --- | --- |
| 共享组件 | Project Tree 与 Hierarchy Tree 共用 `TreeGuide` / `TreeGuideSegment` / `TreeGuideBuilder` |
| 连续树线数据 | 可视节点具备 `GuideSegments`、`HasChildren`、`IsExpanded`、`CanToggle`、`IsCollapsed` |
| 祖先 Guide | 非末祖先绘制贯穿整行 `Full` 竖线；末祖先后续行绘制 `Blank` |
| 当前节点 Guide | 中间节点绘制 `Tee`，末节点绘制 `Elbow`，折叠后重新计算可视线段 |
| 尺寸冻结 | 缩进 20 px、箭头 16 px、图标 16x16、行高 28 px、图标文字间距 7 px |
| 颜色冻结 | 树线 `#C7D7EA` / 1 px；图标 `#2F80C9` / 2.2 |
| 构建节点 | `构建` 显示名改为 `构建配置`，继续作为资源树正式分类显示 |
| 图标替换 | 项目、图标、材质、脚本、相机、地面、区域、实体、世界、文件夹、构建配置统一使用 SVG Path / StreamGeometry 资源 |

## 图标 Path 数据来源

| 图标 | 文件 | 来源 |
| --- | --- | --- |
| 项目 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `ProjectIcon` | 用户确认 SVG：四方框项目图标 |
| 图标/图片 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `ImageIcon` | 用户确认 SVG：图片框、圆点、山线 |
| 材质 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `MaterialIcon` | 用户确认 SVG 语义：外框与双材质块，按 Avalonia Path 资源落库 |
| 脚本 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `ScriptIcon` | 用户确认 SVG：窗口框、标题线、左右代码括号 |
| 相机 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `CameraIcon` | 用户确认 SVG：机身、镜头、状态点 |
| 地面 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `GroundIcon` | 用户确认 SVG：透视地面网格 |
| 区域 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `RegionIcon` | 用户确认 SVG：中心区块、四角锚点、内部连接线 |
| 实体 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `EntityIcon` | 用户确认 SVG：立方体 |
| 世界 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `WorldIcon` | 按本轮规范归一化到 24x24 / `#2F80C9` / 2.2 |
| 文件夹 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `FolderIcon` | 按本轮规范归一化到 24x24 / `#2F80C9` / 2.2 |
| 构建配置 | `XuanYu.Editor.UI/Icons/EditorIcons.axaml` / `BuildIcon` | 按本轮“构建配置”语义使用扳手图标并归一化 |

## 禁止项确认

- 未通过加高短线、负 Margin 或调间距伪造连续效果。
- 未复制两套 Project / Hierarchy XAML 树模板。
- 未使用 Emoji、Unicode 字符或字体 Glyph 充当图标。
- 未修改 dotnet build 命令或构建系统逻辑。
- 未改变 `GlobalWorld`、`Partition`、`WorldQuery`、`SpatialIndex`、Selection、Picking、Move、Undo、Redo、Region 迁移语义。

## 验收要求

| 验收项 | 结果 |
| --- | --- |
| Project Tree 全展开 | 真机截图：`world-a-ui-r2-08-project-expanded-selection-toggle.jpg` |
| Project Tree 部分折叠 | 真机截图：`world-a-ui-r2-09-project-world-collapsed-selection-toggle.jpg`；`世界` 子节点隐藏，后续 `资源` 树线无残线 |
| Hierarchy Tree 全展开 | 真机截图：`world-a-ui-r2-10-hierarchy-expanded.jpg`；区域 0,0,0 与 1,0,0 分支连续 |
| Region 迁移后父级变化 | 自动 Gate 覆盖跨 Region 后 Hierarchy / Inspector / Render 不丢；本轮未取得拖拽迁移后的额外真机截图，不据此宣布人工 PASS |
| 选中态 | 真机截图：`world-a-ui-r2-11-hierarchy-selected-entity.jpg`；选中 `基础测试实体` 后 Inspector 显示 EntityId、Region、Activity、GlobalPosition 与 Transform |
| Hover 状态 | 真机截图：`world-a-ui-r2-09-project-world-collapsed-selection-toggle.jpg` 与 `world-a-ui-r2-11-hierarchy-selected-entity.jpg` 含鼠标所在行 Hover 高亮 |
| 图标放大对照 | `docs/world-a-ui-r2-continuous-tree.svg` 固化源码图；真机截图展示 16x16 实际落地效果 |

## 自动验证

| 验证项 | 结果 |
| --- | --- |
| 文件计数 | `rg --files -g '!**/bin/**' -g '!**/obj/**'` = 420；`file-tree.md` = 420 |
| 5+100 | 本轮新增 / 修改 `.cs` 与 `.axaml` 无新增超 100 行文件；历史测试文件 `WorldPartitionR1Tests.cs` / `WorldPartitionTests.cs` 保持既有超限 |
| SVG XML | `docs/world-a-ui-r2-continuous-tree.svg` 可被 XML 解析 |
| 版本残留 | `run.bat` 与主窗口标题均为 `v0.2.18.21-fix`，未发现 `v0.2.18.20-fix` 运行入口残留 |
| 构建 | `dotnet build .\XuanYu.Engine.slnx --no-restore -p:UseSharedCompilation=false -maxcpucount:1`：7 项目，0 warning / 0 error |
| 测试 | `dotnet test .\XuanYu.Engine.slnx --no-restore --no-build -p:UseSharedCompilation=false -maxcpucount:1`：151 passed / 0 failed / 0 skipped |
| 守卫 | `scripts/arch-a-guard.ps1`：PASS |
| Whitespace | `git diff --check`：PASS |

## Git 证据

- 实现提交：`b52bb78d603b67167d460542ff76e782f6242099`
- 实现提交父节点：`38a92dbec2f1881860e33fb7697f28506fdf4d77`
- 证据回填：本节由后续文档证据提交记录，避免自引用 hash 悖论。
