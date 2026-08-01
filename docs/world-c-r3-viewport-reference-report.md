# WORLD-C-R3 编辑器空间参照层实施报告

版本：`v0.2.21.11-fix`
日期：2026-08-01
分支：`feat/WORLD-C-scene-authoring`

## 状态

`WORLD-C-R3-R3：真机视觉修复轮，等待真机重新验收`。

本轮在 R3-R2 自动验证 PASS 基线上继续修复真机视觉缺陷：顶部两行 UI 按已确认 SVG 方案重构为浅色命令栏；Transform Gizmo 改为统一的细轴/细环/小立方体视觉语言；交互、命中、拖动、提交、撤销和场景文档合同保持不变。

## 架构合同

- 辅助状态归属：`UiVm` 运行会话状态，世界坐标轴默认关闭。
- 辅助快照：`EditorViewportAssistState` 随 `RenderProjection` 进入渲染输入。
- 绘制位置：`RenderDrawPlan` 在正式实体前安排 Background / Grid / Origin / Axes；Axes 仅在用户开启后进入计划；Transform Gizmo 顶点数只表达可见几何，不改变命中热区。
- 绘制实现：Vulkan `scene.vert` 程序化生成，不引入纹理、天空盒或正式资源。
- Picking 边界：辅助 DrawKind 没有 EntityIndex 和 EntityId，不进入 Picking / Selection。
- 场景边界：不进入 SceneDocument、EntityRegistry、History、Dirty 或 `.xyscene`。

## 实施范围

- 背景：低饱和蓝灰渐变，只属于编辑器视口。
- 网格：世界 XY 平面、Z=0、主次线、固定世界坐标。
- 原点：世界 `(0,0,0)` 小型低对比标识，不承担坐标轴职责。
- 世界短轴：默认关闭；开启后为世界原点处有限细短 X/Y/Z 轴，Z 向上，不替代右上角方向控件，也不表现为 Transform Gizmo。
- 相机：新建/空场景使用默认斜俯视；打开非空场景调用现有 Frame All。
- 显示：顶部“显示”菜单内四项运行期切换，不持久化；菜单文本使用统一勾选列。
- 菜单：顶部主命令栏与视口工具栏使用两条浅色 rail；顶部菜单和层级右键菜单使用统一菜单项行高与悬停风格。
- Gizmo：未选择、选择工具和框选工具均不显示 Transform Gizmo；移动、旋转、缩放只显示当前工具对应 Gizmo；取消选择立即隐藏。
- Move：细轴 + 箭头 + 中性中心手柄；平面手柄缩小并改为低饱和角标色。
- Rotate：三条低饱和细圆环 + 中性中心手柄。
- Scale：细轴、小型低饱和端点立方体和白色 Uniform 中心立方体。

## 自动验证

- `dotnet build XuanYu.Engine.slnx -m:1 -nr:false -p:BuildInParallel=false -p:UseSharedCompilation=false`：10 项目 0 warning / 0 error。
- `dotnet test XuanYu.Core.Tests/XuanYu.Core.Tests.csproj --no-build --no-restore`：138 passed / 0 failed / 0 skipped。
- `dotnet test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore`：197 passed / 0 failed / 0 skipped。
- GLSL：`scene.vert` 已由 `glslc` 编译并同步 `ShaderBytecode.Vert.cs`。

## 等待真机验收

WORLD-C-R3 尚未 CLOSED。必须完成 R2 真机复测 01-05 并由用户明确裁定 PASS 后，才能正式收口；本轮不创建 Tag 或 Release。
