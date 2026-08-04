# XuanYu Engine（玄域引擎）架构

创建时间：2026-06-10 ｜ 最近修订：2026-08-04（治理轮合并已关闭里程碑的仍然有效裁定）

> 原标题为「FluidWarfare 引擎架构」；FluidWarfare 为历史开发代号，逐步废弃（见 governance/naming-XuanYu-Engine.md）。

## 架构原则

FluidWarfare 按职责与平台边界拆分。

核心模拟模块必须独立于编辑器 UI、运行时外壳和具体渲染后端。

## 模块边界

| 模块 | 职责 | 平台规则 |
|---|---|---|
| FluidWarfare.Core | 数学、时间、结果、日志、身份等基础类型 | 不依赖 UI、Runtime、Vulkan、Windows 或 Android |
| FluidWarfare.Ecs | ECS-lite 实体、组件、系统、查询 | 仅在需要时依赖 Core |
| FluidWarfare.World | 3D 世界、地面、边界、相机出生点等数据 | 不依赖渲染后端 |
| FluidWarfare.Simulation | 固定 Tick、暂停、单步、模拟世界 | 与渲染帧率分离 |
| FluidWarfare.Combat | 未来战斗领域 | Phase 1 仅保留模块 |
| FluidWarfare.AI | 未来战术 AI、编队 AI、战略 AI | Phase 1 仅保留模块 |
| FluidWarfare.Data | 场景与资源数据读取 | 数据层，不写 UI 或渲染逻辑 |
| FluidWarfare.Render | 渲染抽象契约 | 不绑定 Vulkan |
| FluidWarfare.Render.Vulkan | Vulkan 后端实现 | 只写 Vulkan 相关代码 |
| FluidWarfare.Runtime.Windows | Windows 游戏运行时 | 不依赖 Avalonia |
| FluidWarfare.Runtime.Android | Android 游戏运行时 | 不依赖 Avalonia |
| FluidWarfare.Editor.Windows | Windows 编辑器 | 仅此处允许使用 Avalonia |
| FluidWarfare.Exporter | 构建与打包输出 | 协调打包资源 |

## 依赖方向

Core 位于最底层。

ECS、World、Simulation、Data、Combat、AI 和 Render 抽象可以构建在 Core 之上。

平台运行时、编辑器、导出器和具体 Vulkan 后端是外层模块。

Vulkan 必须被隔离在渲染后端内，不得泄漏到 Simulation 或 World。

## 平台边界

Windows Runtime 用于运行导出的 Windows 游戏。

Android Runtime 用于运行导出的 Android 游戏。

Windows Editor 用于开发、编辑、调试、查看 ECS、查看日志和启动预览。

Android 不做编辑器。

Avalonia 不进入 Android Runtime。

Avalonia 不进入 Windows Runtime。

## Phase 1 架构目标

Phase 1 需要完成以下架构验证：

1. 显示地面。
2. 显示红蓝编队标记。
3. 使用基础相机。
4. 读取 JSON 场景数据。
5. 支持基础 ECS 检查。
6. 支持固定 Tick 模拟。
7. 保持 Windows 与 Android 运行时路径分离。

## 当前骨架状态

当前只创建目录、文档和解决方案。

各模块目录暂时只有 `.gitkeep`。

`FluidWarfare.sln` 暂无项目引用。

下一步进入 Core 前，才创建 `FluidWarfare.Core.csproj` 和 `FluidWarfare.Tests.csproj`。
## 演进后的关键架构裁定（2026-07 归档自 ARCH-WORLD / ARCH-C 关闭里程碑）

### 世界事实与空间权威（ARCH-WORLD R2/R3 CLOSED 裁定）

- `GlobalWorld` 是**世界唯一事实源 + 唯一写链**：`Create` / `Destroy` / `UpdateTransform` / `RebuildSpatialIndexFromWorld` 全部同步内部 `WorldQuery`。
- 空间索引只有一份（`SpatialIndexOwner`，可重建的加速结构）；Scene、Picking、Streaming 共用同一权威查询答案，同一 `EntityId` 在世界中的空间状态只允许一个权威答案。
- `SceneStateOwner` 不保有第二套实体/空间真相：状态仅三类——`_world`（真相引用）、`_snapshot`（World 投影的渲染快照缓存）、`_activeEntityKey`（编辑器选择态游标）。Scene 是 World 的**门面（Facade）+ 投影层**。
- 渲染快照双语义层：`SceneStateOwner` 返回基础 World/Scene 投影（无 Selection/Preview/Gizmo/Camera），`UiVm` 返回叠加编辑器语义的组合投影；生产渲染端只把 `UiVm` 作为 `ISceneRenderSnapshotSource` 注入。
- 受控债务（R4/R5 登记，P2 渐进）：`SceneRenderSnapshot` 自带编辑器语义（`IsSelected`/`PreviewTransform`/`ShowMoveGizmo`/`Camera ?? DefaultEditorCamera.Create(0)` 后门）位于 Core.Scene；`DefaultEditorCamera`/`EditorCameraFraming` 位于 Core.Space；`TransformSession`（含 Core.Gizmo 语义）暂居 World.Transform。

### 坐标系与渲染转换（WORLD-A-R0 合同，详细见 architecture/world-a-r0-coordinate-contract.md）

- 世界空间右手笛卡尔：`+Z = Up`、`XY = 水平面`、`X × Y = Z`；世界 X/Y 是固定水平轴，不定义唯一 Forward。
- Vulkan 唯一 Y 转换发生在 `Render.Vulkan` 组装 Push Constant 的边界副本：`VulkanProjection = FlipClipY(CoreProjection)`，不回写 Core Projection。
- 显示、命中、拖动约束与 Picking 共用同一 `ViewProjectionState`，不得各自维护坐标补丁。

### 编辑器边界（ARCH-WORLD R4 + 宪法硬红线）

- `Editor.UI` 不直接依赖 Vulkan；`Render.Abstractions` 不引用 `Silk.NET.Vulkan`；arch-a-guard 守卫。
