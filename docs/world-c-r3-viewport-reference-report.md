# WORLD-C-R3 编辑器空间参照层实施报告

版本：`v0.2.21.9-rz`
日期：2026-08-01 10:21:46
分支：`feat/WORLD-C-scene-authoring`

## 状态

`WORLD-C-R3-R1：自动实现完成，等待真机验收`。

本轮完成编辑器视口辅助参照层：编辑器背景、XY 构造网格、世界原点、X/Y/Z 世界短轴、显示菜单四开关、空场景默认相机和非空场景打开 Frame All 接入。

## 架构合同

- 辅助状态归属：`UiVm` 运行会话状态。
- 辅助快照：`EditorViewportAssistState` 随 `RenderProjection` 进入渲染输入。
- 绘制位置：`RenderDrawPlan` 在正式实体前安排 Background / Grid / Origin / Axes。
- 绘制实现：Vulkan `scene.vert` 程序化生成，不引入纹理、天空盒或正式资源。
- Picking 边界：辅助 DrawKind 没有 EntityIndex 和 EntityId，不进入 Picking / Selection。
- 场景边界：不进入 SceneDocument、EntityRegistry、History、Dirty 或 `.xyscene`。

## 实施范围

- 背景：低饱和蓝灰渐变，只属于编辑器视口。
- 网格：世界 XY 平面、Z=0、主次线、固定世界坐标。
- 原点：世界 `(0,0,0)` 小标识，不是地面中心或地图中心。
- 世界短轴：有限 X/Y/Z 轴，Z 向上，不替代右上角方向控件。
- 相机：新建/空场景使用默认斜俯视；打开非空场景调用现有 Frame All。
- 显示：顶部“显示”菜单内四项运行期切换，不持久化。

## 自动验证

- `dotnet build XuanYu.Engine.slnx -m:1 -nr:false -p:BuildInParallel=false -p:UseSharedCompilation=false`：10 项目 0 warning / 0 error。
- `dotnet test XuanYu.Core.Tests/XuanYu.Core.Tests.csproj --no-build --no-restore`：137 passed / 0 failed / 0 skipped。
- `dotnet test XuanYu.World.Tests/XuanYu.World.Tests.csproj --no-build --no-restore`：193 passed / 0 failed / 0 skipped。
- GLSL：`scene.vert` 已由 `glslc` 编译并同步 `ShaderBytecode.Vert.cs`。

## 等待真机验收

WORLD-C-R3 尚未 CLOSED。必须完成计划中的 01-07 真机测试并由用户明确裁定 PASS 后，才能正式收口；本轮不创建 Tag 或 Release。
