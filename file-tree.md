# 项目文件树 - FluidWarfare

当前阶段：Phase 1
当前版本：0.0.1-dev
创建时间：2026-06-10
最后编辑：2026-06-10 17:12

本文档用于记录 FluidWarfare 项目目录结构、模块职责、关键文件职责、未发布变更和模块依赖方向。每次新增、删除、重命名或移动文件与目录时，都必须同步更新本文档。

## 1. 未发布变更日志

### 新增

1. 创建 `FluidWarfare.sln`。
2. 创建 Core、ECS、World、Simulation、Combat、AI、Data、Render、Vulkan 渲染后端、运行时、编辑器、导出器和测试等顶层模块目录。
3. 创建资源目录：`game_data`、`assets`、`shaders` 和 `replays`。
4. 创建初始文档目录 `docs` 及其中的项目文档。
5. 为当前空目录创建 `.gitkeep` 占位文件，确保模块目录和资源目录能提交到 Git。

### 修改

1. 将 `docs/` 下所有 Markdown 文档正文改为中文。
2. 将 `file-tree.md` 正文改为中文。

### 删除

1. 删除由 .NET SDK 默认模板临时生成的 `FluidWarfare.slnx`，因为项目计划要求使用 `FluidWarfare.sln`。

### 重命名

无。

## 2. 当前阶段目标

Phase 1 证明最小闭环：Windows Editor 创建简单 3D 场景，保存为 JSON，Windows Runtime 和 Android Runtime 读取同一份数据，Exporter 打包运行时输出。

## 3. 顶层目录结构

```text
FluidWarfare/
|-- FluidWarfare.Core/
|-- FluidWarfare.Ecs/
|-- FluidWarfare.World/
|-- FluidWarfare.Simulation/
|-- FluidWarfare.Combat/
|-- FluidWarfare.AI/
|-- FluidWarfare.Data/
|-- FluidWarfare.Render/
|-- FluidWarfare.Render.Vulkan/
|-- FluidWarfare.Runtime.Windows/
|-- FluidWarfare.Runtime.Android/
|-- FluidWarfare.Editor.Windows/
|-- FluidWarfare.Exporter/
|-- FluidWarfare.Tests/
|-- game_data/
|-- assets/
|-- shaders/
|-- replays/
|-- docs/
|-- FluidWarfare.sln
`-- file-tree.md
```

## 4. 模块职责说明

| 模块 | 职责 | 状态 |
|---|---|---|
| FluidWarfare.Core | 数学、时间、结果、日志和身份等基础类型 | 计划中 |
| FluidWarfare.Ecs | ECS-lite 实体、组件、系统和查询 | 计划中 |
| FluidWarfare.World | 地面、边界、相机出生点和空间场景数据 | 计划中 |
| FluidWarfare.Simulation | 固定 Tick、暂停、单步和模拟世界 | 计划中 |
| FluidWarfare.Combat | 未来的接敌、士气、伤亡和战斗日志 | 计划中 |
| FluidWarfare.AI | 未来的战术 AI、编队 AI 和战略 AI | 计划中 |
| FluidWarfare.Data | 场景 JSON 与资源数据读取 | 计划中 |
| FluidWarfare.Render | 渲染抽象契约 | 计划中 |
| FluidWarfare.Render.Vulkan | Vulkan 后端实现 | 计划中 |
| FluidWarfare.Runtime.Windows | Windows 游戏运行时 | 计划中 |
| FluidWarfare.Runtime.Android | Android 游戏运行时 | 计划中 |
| FluidWarfare.Editor.Windows | Windows Avalonia 编辑器 | 计划中 |
| FluidWarfare.Exporter | Windows 与 Android 导出流程 | 计划中 |
| FluidWarfare.Tests | 单元测试和聚焦集成测试 | 计划中 |

## 5. 关键文件职责

| 文件 | 职责 | 状态 |
|---|---|---|
| `FluidWarfare.sln` | 解决方案容器 | 已创建 |
| `docs/PROJECT_CHARTER.md` | 项目目标和第一阶段闭环 | 已创建 |
| `docs/ENGINE_ARCHITECTURE.md` | 模块边界和依赖方向 | 已创建 |
| `docs/AI_DEVELOPMENT_RULES.md` | AI 辅助开发规则 | 已创建 |
| `docs/CODE_CONSTITUTION.md` | 代码结构与架构规则 | 已创建 |
| `docs/NAMING_RULES.md` | C# 与资源命名规则 | 已创建 |
| `docs/PHASE1_SCOPE.md` | Phase 1 范围和排除项 | 已创建 |
| `docs/LEGACY_FLUIDWARFARE_OLD_AUDIT.md` | 旧仓库只读考古报告 | 已创建 |
| `docs/CHANGELOG.md` | 版本历史 | 已创建 |
| `file-tree.md` | 项目结构地图 | 已创建 |
| `*/.gitkeep` | 保留当前尚未写入代码或资源的目录 | 已创建 |

## 6. 模块依赖方向

Core 是基础层。ECS、World、Simulation、Data、Combat、AI 和 Render 抽象可以向内依赖，但不能反向依赖外层。Runtime、Editor、Exporter 和具体 Vulkan 代码属于外层。

## 7. 文件命名与目录纪律

C# 代码使用 `FluidWarfare.*` 命名空间。不得使用 `BingWuChangShiEngine`、`Bwc.*`、`cls_`、`fuc_`、泛化工具目录或编号 Part 文件。

## 8. 当前不做的内容

第一轮不实现 ECS、Vulkan、Android、Avalonia UI 细节、战斗系统或 AI。

## 9. 版本历史索引

详见 `docs/CHANGELOG.md`。
