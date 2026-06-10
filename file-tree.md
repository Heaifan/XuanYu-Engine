# 项目文件树 - FluidWarfare

当前阶段：Phase 1

当前版本：0.0.1-dev

创建时间：2026-06-10

最后编辑：2026-06-10 17:25

本文档用于记录 FluidWarfare 项目目录结构、模块职责、关键文件职责、未发布变更和模块依赖方向。

每次新增、删除、重命名或移动文件与目录时，都必须同步更新本文档。

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
3. 重新排版 `docs/*.md` 和 `file-tree.md`，确保标题、段落、表格、列表和代码块独立换行。
4. 核对本地与远端 `origin/main`，确认新仓库当前没有旧项目目录残留。

### 删除

1. 删除由 .NET SDK 默认模板临时生成的 `FluidWarfare.slnx`，因为项目计划要求使用 `FluidWarfare.sln`。

### 重命名

无。

## 2. 当前阶段目标

Phase 1 证明最小闭环。

目标流程如下：

1. Windows Editor 创建简单 3D 场景。
2. 场景保存为 JSON。
3. Windows Runtime 读取同一份数据并运行。
4. Android Runtime 读取同一份数据并运行。
5. Exporter 打包运行时输出。

当前不进入 Core、ECS、Vulkan、Android 或 Avalonia UI 具体实现。

## 3. 顶层目录结构

当前真实顶层结构如下：

```text
FluidWarfare/
|-- .git/
|-- FluidWarfare.Core/
|   `-- .gitkeep
|-- FluidWarfare.Ecs/
|   `-- .gitkeep
|-- FluidWarfare.World/
|   `-- .gitkeep
|-- FluidWarfare.Simulation/
|   `-- .gitkeep
|-- FluidWarfare.Combat/
|   `-- .gitkeep
|-- FluidWarfare.AI/
|   `-- .gitkeep
|-- FluidWarfare.Data/
|   `-- .gitkeep
|-- FluidWarfare.Render/
|   `-- .gitkeep
|-- FluidWarfare.Render.Vulkan/
|   `-- .gitkeep
|-- FluidWarfare.Runtime.Windows/
|   `-- .gitkeep
|-- FluidWarfare.Runtime.Android/
|   `-- .gitkeep
|-- FluidWarfare.Editor.Windows/
|   `-- .gitkeep
|-- FluidWarfare.Exporter/
|   `-- .gitkeep
|-- FluidWarfare.Tests/
|   `-- .gitkeep
|-- game_data/
|   `-- .gitkeep
|-- assets/
|   `-- .gitkeep
|-- shaders/
|   `-- .gitkeep
|-- replays/
|   `-- .gitkeep
|-- docs/
|   |-- AI_DEVELOPMENT_RULES.md
|   |-- CHANGELOG.md
|   |-- CODE_CONSTITUTION.md
|   |-- ENGINE_ARCHITECTURE.md
|   |-- LEGACY_FLUIDWARFARE_OLD_AUDIT.md
|   |-- NAMING_RULES.md
|   |-- PHASE1_SCOPE.md
|   `-- PROJECT_CHARTER.md
|-- FluidWarfare.sln
`-- file-tree.md
```

以下旧项目目录和文件不应存在于新仓库：

```text
.dotnet_home/
Docs/
Prj_Graphics/
Prj_UI/
FluidWarfare.slnx
get_tree.bat
```

本地核对结果：上述旧目录和文件未在新仓库真实结构中保留。

说明：Windows 文件系统大小写不敏感，`Test-Path Docs` 会匹配当前合法目录 `docs`，不能用它单独判断是否存在旧目录 `Docs`。

## 4. 模块职责说明

| 模块 | 职责 | 状态 |
|---|---|---|
| FluidWarfare.Core | 数学、时间、结果、日志和身份等基础类型 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Ecs | ECS-lite 实体、组件、系统和查询 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.World | 地面、边界、相机出生点和空间场景数据 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Simulation | 固定 Tick、暂停、单步和模拟世界 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Combat | 未来的接敌、士气、伤亡和战斗日志 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.AI | 未来的战术 AI、编队 AI 和战略 AI | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Data | 场景 JSON 与资源数据读取 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Render | 渲染抽象契约 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Render.Vulkan | Vulkan 后端实现 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Runtime.Windows | Windows 游戏运行时 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Runtime.Android | Android 游戏运行时 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Editor.Windows | Windows Avalonia 编辑器 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Exporter | Windows 与 Android 导出流程 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Tests | 单元测试和聚焦集成测试 | 已创建 / 仅 `.gitkeep` |

## 5. 关键文件职责

| 文件 | 职责 | 状态 |
|---|---|---|
| `FluidWarfare.sln` | 解决方案容器 | 已创建 / 暂无项目引用 |
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

Core 是基础层。

ECS、World、Simulation、Data、Combat、AI 和 Render 抽象可以向内依赖 Core。

Runtime、Editor、Exporter 和具体 Vulkan 代码属于外层。

外层模块不得反向污染内层模块。

Vulkan 依赖只能进入 `FluidWarfare.Render.Vulkan`。

Avalonia 依赖只能进入 `FluidWarfare.Editor.Windows`。

## 7. 文件命名与目录纪律

C# 代码使用 `FluidWarfare.*` 命名空间。

不得使用以下旧命名或泛化命名：

```text
BingWuChangShiEngine
Bwc.*
cls_
fuc_
utils
helpers
managers
processors
Part1
Part2
```

数据与资源文件可以使用领域前缀，例如 `scn_`、`cfg_`、`dat_`、`mesh_`、`tex_`、`mat_` 和 `shd_`。

## 8. 当前不做的内容

本轮修复只处理 Milestone 1 审计问题。

本轮不做以下内容：

1. Core 类型实现。
2. ECS 实现。
3. Vulkan 实现。
4. Android 实现。
5. Avalonia UI 实现。
6. 战斗系统。
7. AI。
8. 第三方框架引入。

## 9. 版本历史索引

详见 `docs/CHANGELOG.md`。
