# FluidWarfare-old 旧仓库考古报告

创建时间：2026-06-10

## 来源

旧仓库：`https://github.com/Heaifan/FluidWarfare-old`

该公开仓库被作为历史资料审阅。

可见顶层结构包括：

```text
.dotnet_home/.dotnet
.vscode
Docs
Prj_Graphics
Prj_UI
FluidWarfare.slnx
get_tree.bat
```

GitHub 显示该仓库主要由 C#、GLSL 和 Batchfile 构成。

## 旧仓库目标

FluidWarfare-old 似乎是早期 C# 方向探索。

它包含 Windows 编辑器、图形栈、Avalonia UI、OpenGL 风格渲染、Shader 资源、地形或 DEM 方向，以及编辑器布局实验。

## 旧仓库模块结构

| 区域 | 观察到的职责 |
|---|---|
| `Docs` | 历史设计笔记与项目文档 |
| `Prj_Graphics` | 图形实验与渲染层概念 |
| `Prj_UI` | Windows UI / 编辑器探索 |
| `.vscode` | 本地开发配置 |
| `.dotnet_home/.dotnet` | 本地工具或运行时支持材料 |
| `FluidWarfare.slnx` | 旧解决方案容器 |
| `get_tree.bat` | 目录树或报告辅助脚本 |

## 可继承思想

1. C# 继续作为主要实现语言。
2. Avalonia 可作为 Windows Editor 的合理方向，但只能用于编辑器。
3. 图形职责应与 UI 职责解耦。
4. 渲染层不应感知高层业务对象。
5. FileSystem、Viewport、Inspector、Console 四区编辑器布局值得保留。
6. Grid、Axis、Gizmo 调试可视化思路有价值。
7. 地形与 DEM 概念适合作为未来研究方向，而不是 Phase 1 第一优先级。
8. 文档先行的开发意识应继续保留。

## 不应继承的实现

1. 旧 OpenGL 具体实现。
2. 旧 `Prj_Graphics` 与 `Prj_UI` 工程结构。
3. `cls_`、`fuc_` 或类似前缀命名。
4. 以 Terrain 或 DEM 作为第一核心的开发顺序。
5. 旧 Renderer 代码和旧 OpenGL Control 代码。

## 与新 FluidWarfare 的差异

新仓库从更严格的架构开始。

新结构包含 `FluidWarfare.Core`、`FluidWarfare.Ecs`、`FluidWarfare.World`、`FluidWarfare.Simulation`、`FluidWarfare.Data`、渲染抽象、Vulkan 后端、平台运行时、编辑器和导出器。

Vulkan 替代 OpenGL 成为目标渲染后端。

Avalonia 仍只用于 Windows Editor。

## Phase 1 吸收策略

Phase 1 可以吸收概念、布局经验、命名教训和文档意识。

不得复制源代码。

不得迁移 OpenGL 实现。

不得保留旧工程命名。

不得继续在旧仓库开发。
