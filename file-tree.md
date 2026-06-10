# 项目文件树 - FluidWarfare

当前阶段：Phase 1

当前版本：0.0.1-dev

创建时间：2026-06-10

最后编辑：2026-06-10 22:50

本文档用于记录 FluidWarfare 项目目录结构、模块职责、关键文件职责、未发布变更和模块依赖方向。

每次新增、删除、重命名或移动文件与目录时，都必须同步更新本文档。

## 1. 未发布变更日志

### 新增

1. 创建 `FluidWarfare.sln`。
2. 创建 Core、ECS、World、Simulation、Combat、AI、Data、Render、Vulkan 渲染后端、运行时、编辑器、导出器和测试等顶层模块目录。
3. 创建资源目录：`game_data`、`assets`、`shaders` 和 `replays`。
4. 创建初始文档目录 `docs` 及其中的项目文档。
5. 为当前空目录创建 `.gitkeep` 占位文件，确保模块目录和资源目录能提交到 Git。
6. 创建 `.gitattributes`，固定 Markdown 等文本文件使用 LF 行尾。
7. 创建 `docs/MILESTONE1_PUBLIC_VALIDATION.md`，记录公开 Raw 验收方式。
8. 创建 `FluidWarfare.Core/FluidWarfare.Core.csproj`。
9. 创建 `FluidWarfare.Tests/FluidWarfare.Tests.csproj`。
10. 创建 `FluidWarfare.Tests/CoreSmokeTests.cs`。
11. 创建 `FluidWarfare.Core/Identity/EntityId.cs`。
12. 创建 `FluidWarfare.Tests/Core/Identity/EntityIdTests.cs`。
13. 创建 `FluidWarfare.Core/Time/TimeStep.cs`。
14. 创建 `FluidWarfare.Core/Time/SimulationTime.cs`。
15. 创建 `FluidWarfare.Tests/Core/Time/TimeStepTests.cs`。
16. 创建 `FluidWarfare.Tests/Core/Time/SimulationTimeTests.cs`。
17. 创建 `FluidWarfare.Core/Math/Vector3d.cs`。
18. 创建 `FluidWarfare.Core/Math/YawRotation.cs`。
19. 创建 `FluidWarfare.Tests/Core/Math/Vector3dTests.cs`。
20. 创建 `FluidWarfare.Tests/Core/Math/YawRotationTests.cs`。
21. 创建 `FluidWarfare.Core/Results/EngineError.cs`。
22. 创建 `FluidWarfare.Core/Results/EngineResult.cs`。
23. 创建 `FluidWarfare.Tests/Core/Results/EngineErrorTests.cs`。
24. 创建 `FluidWarfare.Tests/Core/Results/EngineResultTests.cs`。
25. 创建 `FluidWarfare.Core/Logging/EngineLogLevel.cs`。
26. 创建 `FluidWarfare.Core/Logging/EngineLogEntry.cs`。
27. 创建 `FluidWarfare.Tests/Core/Logging/EngineLogLevelTests.cs`。
28. 创建 `FluidWarfare.Tests/Core/Logging/EngineLogEntryTests.cs`。
29. Milestone 3.1：创建 `FluidWarfare.Editor.Windows` Avalonia 编辑器项目。
30. Milestone 3.2：创建 `FluidWarfare Editor` 五区 GUI 最小壳。

### 修改

1. 将 `docs/` 下所有 Markdown 文档正文改为中文。
2. 将 `file-tree.md` 正文改为中文。
3. 重新排版 `docs/*.md` 和 `file-tree.md`，确保标题、段落、表格、列表和代码块独立换行。
4. 核对本地与远端 `origin/main`，确认新仓库当前没有旧项目目录残留。
5. 将 Markdown 文件重写为 UTF-8 无 BOM 与 LF 行尾，方便 GitHub Raw 公开验收。
6. 使用 Python 以 `newline="\n"` 重新写入 `.gitattributes`、`file-tree.md` 和所有 `docs/*.md`。
7. 将 Core 与 Tests 项目加入 `FluidWarfare.sln`。
8. Milestone 2.3.1：修复 TimeStep 默认值边界，明确 default(TimeStep) 为无效时间步。
9. Milestone 2.3.1：SimulationTime.Advance 拒绝无效 TimeStep。
10. Milestone 2.3.1：TimeStep / SimulationTime 的 ToString 改用 InvariantCulture。
11. 中文化规则：明确人类可读报错、日志、提示、验收输出和文档说明默认使用中文。
12. Core 时间类型提示：将 TimeStep / SimulationTime 的异常提示改为中文。
13. EntityId 提示：将实体编号错误提示改为中文。
14. Core 数学类型提示：将 Vector3d / YawRotation 的异常提示改为中文。
15. Milestone 2.5.1：修复 EngineResult 默认值语义，明确 default(EngineResult) 为无效结果。
16. Milestone 2.5.1：调整 EngineResult.IsFailure，仅有效失败结果返回 true。
17. Milestone 2.5.1：确认日志等级前缀统一使用【】。
18. Milestone 2.5.2：统一日志等级前缀符号为【】。
19. Milestone 2.6：新增 EngineLogLevel。
20. Milestone 2.6：新增 EngineLogEntry。
21. Milestone 2.6：新增日志等级与日志记录单元测试。
22. Milestone 2.6.1：修复日志等级前缀统一校验，确认 EngineLogLevel 与 EngineLogEntry 只使用【】。
23. Milestone 3.1：将 FluidWarfare.Editor.Windows 加入 FluidWarfare.sln，并引用 FluidWarfare.Core。
24. Milestone 3.2：实现顶部菜单、项目面板、3D 视口占位、检查器和日志面板。
25. Milestone 3.3：Editor 日志面板接入 EngineLogEntry，启动日志由 Core 日志对象生成。

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

当前执行 Milestone 3.3：Editor 日志面板接入 EngineLogEntry，启动日志由 Core 日志对象生成。

本轮只处理 Editor 启动日志数据源，不实现日志写入器、文件日志、控制台日志、过滤器、ECS、Vulkan、Runtime 或 Android。

## 3. 顶层目录结构

当前真实顶层结构如下：

```text
FluidWarfare/
|-- .git/
|-- .gitattributes
|-- FluidWarfare.Core/
|   |-- .gitkeep
|   |-- FluidWarfare.Core.csproj
|   |-- Identity/
|   |   `-- EntityId.cs
|   |-- Logging/
|   |   |-- EngineLogEntry.cs
|   |   `-- EngineLogLevel.cs
|   |-- Math/
|   |   |-- Vector3d.cs
|   |   `-- YawRotation.cs
|   |-- Results/
|   |   |-- EngineError.cs
|   |   `-- EngineResult.cs
|   `-- Time/
|       |-- SimulationTime.cs
|       `-- TimeStep.cs
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
|   |-- App.axaml
|   |-- App.axaml.cs
|   |-- FluidWarfare.Editor.Windows.csproj
|   |-- MainWindow.axaml
|   |-- MainWindow.axaml.cs
|   |-- Program.cs
|   |-- Panels/
|   |   |-- Inspector/
|   |   |   |-- InspectorPanel.axaml
|   |   |   `-- InspectorPanel.axaml.cs
|   |   |-- Logging/
|   |   |   |-- LogPanel.axaml
|   |   |   `-- LogPanel.axaml.cs
|   |   |-- Project/
|   |   |   |-- ProjectPanel.axaml
|   |   |   `-- ProjectPanel.axaml.cs
|   |   `-- Viewport/
|   |       |-- ViewportPlaceholderPanel.axaml
|   |       `-- ViewportPlaceholderPanel.axaml.cs
|   `-- Shell/
|       |-- EditorShell.axaml
|       `-- EditorShell.axaml.cs
|-- FluidWarfare.Exporter/
|   `-- .gitkeep
|-- FluidWarfare.Tests/
|   |-- .gitkeep
|   |-- Core/
|   |   |-- Identity/
|   |   |   `-- EntityIdTests.cs
|   |   |-- Logging/
|   |   |   |-- EngineLogEntryTests.cs
|   |   |   `-- EngineLogLevelTests.cs
|   |   |-- Math/
|   |   |   |-- Vector3dTests.cs
|   |   |   `-- YawRotationTests.cs
|   |   |-- Results/
|   |   |   |-- EngineErrorTests.cs
|   |   |   `-- EngineResultTests.cs
|   |   `-- Time/
|   |       |-- SimulationTimeTests.cs
|   |       `-- TimeStepTests.cs
|   |-- CoreSmokeTests.cs
|   `-- FluidWarfare.Tests.csproj
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
|   |-- MILESTONE1_PUBLIC_VALIDATION.md
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
| FluidWarfare.Core | 数学、时间、结果、日志和身份等基础类型 | 已创建 / EngineLogLevel 与 EngineLogEntry 测试通过 |
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
| FluidWarfare.Editor.Windows | Windows 桌面编辑器，使用 Avalonia 构建 GUI，只用于开发、调试和导出，不进入 Android Runtime | 可运行 |
| FluidWarfare.Exporter | Windows 与 Android 导出流程 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Tests | 单元测试和聚焦集成测试 | 已创建 / EngineLogLevel 与 EngineLogEntry 测试通过 |

## 5. 关键文件职责

| 文件 | 职责 | 状态 |
|---|---|---|
| `FluidWarfare.sln` | 解决方案容器 | 已创建 / 已引用 Core 与 Tests |
| `FluidWarfare.Core/FluidWarfare.Core.csproj` | Core 纯 C# 类库项目 | 已创建 |
| `FluidWarfare.Core/Identity/EntityId.cs` | 引擎实体唯一标识值对象，封装有效实体编号与 None 无效编号 | 测试通过 |
| `FluidWarfare.Core/Time/TimeStep.cs` | 表示单次模拟推进时间长度，统一秒与毫秒换算，拒绝非正数和非法浮点值 | 测试通过 |
| `FluidWarfare.Core/Time/SimulationTime.cs` | 表示模拟世界累计时间，支持从零开始并通过 TimeStep 推进 | 测试通过 |
| `FluidWarfare.Core/Math/Vector3d.cs` | 引擎核心 3D 坐标与向量值对象，统一 X/Y/Z 坐标、长度、距离、标准化、点积与基础运算 | 测试通过 |
| `FluidWarfare.Core/Math/YawRotation.cs` | 水平朝向角值对象，统一绕 Y 轴的方向约定、角度归一化与 XZ 平面前向向量 | 测试通过 |
| `FluidWarfare.Core/Results/EngineError.cs` | 引擎错误值对象，承载稳定英文错误代码与中文可读错误信息，不包含【报错】等日志等级前缀 | 测试通过 |
| `FluidWarfare.Core/Results/EngineResult.cs` | 引擎操作结果值对象，统一表达成功或失败，要求失败结果携带有效 EngineError，并明确默认值无效 | 测试通过 |
| `FluidWarfare.Core/Logging/EngineLogLevel.cs` | 引擎日志等级枚举与中文等级标签映射，统一【追踪】【信息】【警告】【报错】【严重】显示前缀 | 测试通过 |
| `FluidWarfare.Core/Logging/EngineLogEntry.cs` | 引擎日志记录值对象，保存模拟时间、日志等级、分类和中文日志内容，并提供基础中文显示输出 | 测试通过 |
| `FluidWarfare.Tests/FluidWarfare.Tests.csproj` | xUnit 测试项目，引用 Core | 已创建 |
| `FluidWarfare.Tests/CoreSmokeTests.cs` | 最小 Core 项目可用性测试 | 已创建 |
| `FluidWarfare.Tests/Core/Identity/EntityIdTests.cs` | 验证 EntityId 的有效性、异常、相等比较与稳定字符串输出 | 测试通过 |
| `FluidWarfare.Tests/Core/Time/TimeStepTests.cs` | 验证 TimeStep 的创建、单位换算、非法输入、相等比较与稳定字符串输出 | 测试通过 |
| `FluidWarfare.Tests/Core/Time/SimulationTimeTests.cs` | 验证 SimulationTime 的零点、创建、推进、不变性、非法输入与稳定字符串输出 | 测试通过 |
| `FluidWarfare.Tests/Core/Math/Vector3dTests.cs` | 验证 Vector3d 的静态值、长度、距离、运算符、点积、标准化、相等比较与稳定字符串输出 | 测试通过 |
| `FluidWarfare.Tests/Core/Math/YawRotationTests.cs` | 验证 YawRotation 的角度归一化、弧度换算、前向方向约定、异常输入、相等比较与稳定字符串输出 | 测试通过 |
| `FluidWarfare.Tests/Core/Results/EngineErrorTests.cs` | 验证 EngineError 的创建、非法输入、默认无效值、相等比较、中文 ToString 输出与日志等级前缀隔离 | 测试通过 |
| `FluidWarfare.Tests/Core/Results/EngineResultTests.cs` | 验证 EngineResult 的成功/失败语义、默认值无效、错误携带、默认错误拒绝、相等比较、中文 ToString 输出与日志等级前缀隔离 | 测试通过 |
| `FluidWarfare.Tests/Core/Logging/EngineLogLevelTests.cs` | 验证日志等级到中文显示前缀的映射 | 测试通过 |
| `FluidWarfare.Tests/Core/Logging/EngineLogEntryTests.cs` | 验证日志记录创建、非法输入、日志前缀隔离、中文显示输出与相等比较 | 测试通过 |
| `FluidWarfare.Editor.Windows/FluidWarfare.Editor.Windows.csproj` | Windows Editor Avalonia 项目文件，引用 Core 并声明 Avalonia 桌面依赖 | 可运行 |
| `FluidWarfare.Editor.Windows/Program.cs` | Editor 进程入口，配置 Avalonia 桌面生命周期 | 可运行 |
| `FluidWarfare.Editor.Windows/App.axaml` | Editor 应用 XAML 根对象，加载 Fluent 主题 | 可运行 |
| `FluidWarfare.Editor.Windows/App.axaml.cs` | Editor 应用启动逻辑，创建主窗口 | 可运行 |
| `FluidWarfare.Editor.Windows/MainWindow.axaml` | 编辑器主窗口 XAML 容器，承载 EditorShell | 可运行 |
| `FluidWarfare.Editor.Windows/MainWindow.axaml.cs` | 编辑器主窗口 code-behind | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml` | 编辑器五区布局壳，组织菜单栏、项目面板、视口占位、检查器和日志面板 | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs` | 编辑器五区布局壳的后台逻辑，创建启动日志并传递给日志面板 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Project/ProjectPanel.axaml` | 编辑器项目面板占位，显示当前未打开项目、场景、单位、资源和配置 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Project/ProjectPanel.axaml.cs` | 项目面板 code-behind | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportPlaceholderPanel.axaml` | 3D 视口占位面板，提示 Vulkan 渲染器尚未接入 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportPlaceholderPanel.axaml.cs` | 视口占位面板 code-behind | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorPanel.axaml` | 检查器面板占位，显示未选择对象 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorPanel.axaml.cs` | 检查器面板 code-behind | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Logging/LogPanel.axaml` | 编辑器日志面板，显示中文日志文本 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Logging/LogPanel.axaml.cs` | 编辑器日志面板后台逻辑，接收日志显示文本并提供给 UI 绑定 | 可运行 |
| `.gitattributes` | 固定文本文件行尾规则 | 已创建 |
| `docs/PROJECT_CHARTER.md` | 项目目标和第一阶段闭环 | 已创建 |
| `docs/ENGINE_ARCHITECTURE.md` | 模块边界和依赖方向 | 已创建 |
| `docs/AI_DEVELOPMENT_RULES.md` | AI 辅助开发规则 | 已创建 |
| `docs/CODE_CONSTITUTION.md` | 代码结构与架构规则 | 已创建 |
| `docs/NAMING_RULES.md` | C# 与资源命名规则 | 已创建 |
| `docs/PHASE1_SCOPE.md` | Phase 1 范围和排除项 | 已创建 |
| `docs/LEGACY_FLUIDWARFARE_OLD_AUDIT.md` | 旧仓库只读考古报告 | 已创建 |
| `docs/MILESTONE1_PUBLIC_VALIDATION.md` | 公开 Raw 验收命令与结果记录 | 已创建 |
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

当前已经进入 Milestone 3.3 Editor 日志面板接入 EngineLogEntry 任务。

本轮不做以下内容：

1. EngineLogFormatter 实现。
2. 日志写入器实现。
3. 文件日志实现。
4. 控制台日志实现。
5. 日志过滤实现。
6. 日志等级筛选实现。
7. 复杂 MVVM 框架。
8. ECS 实现。
9. World 实现。
10. Data Loader 实现。
11. Vulkan 接入。
12. Runtime.Windows 实现。
13. Android 实现。
14. 项目打开或保存。
15. 真实场景树。

## 9. 版本历史索引

详见 `docs/CHANGELOG.md`。
