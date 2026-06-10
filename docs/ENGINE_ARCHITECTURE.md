# FluidWarfare 引擎架构

创建时间：2026-06-10

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

## Phase 1 架构目标

Phase 1 需要完成以下架构验证：

1. 显示地面。
2. 显示红蓝编队标记。
3. 使用基础相机。
4. 读取 JSON 场景数据。
5. 支持基础 ECS 检查。
6. 支持固定 Tick 模拟。
7. 保持 Windows 与 Android 运行时路径分离。
