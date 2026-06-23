# FluidWarfare 变更日志

本文档记录项目的重要变更。

## 0.0.1-dev - 2026-06-10

### Milestone 2.x 中文化补丁

#### 修改

1. 明确 FluidWarfare 的人类可读文本默认使用中文。
2. 保留机器识别用 Code、类名、方法名、命名空间为英文。
3. 将 Core 已有异常提示中文化。

### 新增

1. 创建初始解决方案骨架。
2. 创建顶层模块目录和资源目录规划。
3. 创建项目宪章、架构说明、AI 开发规则、代码宪法、命名规则、Phase 1 范围和旧仓库考古报告。
4. 创建 `file-tree.md`，作为项目结构地图。
5. 为当前空目录创建 `.gitkeep` 占位文件。
6. 创建 `.gitattributes`，固定 Markdown、解决方案、C# 和 JSON 文件使用 LF 行尾。
7. 创建 `docs/MILESTONE1_PUBLIC_VALIDATION.md`，记录公开 GitHub Raw 验收命令与旧目录核对命令。
8. 创建 `FluidWarfare.Core` 纯 C# 类库项目。
9. 创建 `FluidWarfare.Tests` xUnit 测试项目。
10. 创建 `CoreSmokeTests` 最小冒烟测试。
11. Milestone 2.2：新增 `EntityId` 值对象。
12. Milestone 2.2：新增 `EntityId` 单元测试。
13. Milestone 2.3：新增 `TimeStep` 时间步长值对象。
14. Milestone 2.3：新增 `SimulationTime` 模拟累计时间值对象。
15. Milestone 2.3：新增 `TimeStep` 与 `SimulationTime` 单元测试。
16. Milestone 2.4：新增 `Vector3d` 3D 坐标与向量值对象。
17. Milestone 2.4：新增 `YawRotation` 水平朝向角值对象。
18. Milestone 2.4：新增 `Vector3d` 与 `YawRotation` 单元测试。
19. Milestone 2.4：固定核心坐标约定，XZ 为地面平面，Y 为高度。
20. Milestone 2.4：固定朝向约定，0 度朝 +Z，90 度朝 +X。
21. Milestone 2.5：新增 `EngineError` 错误值对象。
22. Milestone 2.5：新增 `EngineResult` 操作结果值对象。
23. Milestone 2.5：新增 `EngineError` 与 `EngineResult` 单元测试。
24. Milestone 2.5：固定结果语义，Success 不携带错误，Failure 必须携带有效错误。
25. Milestone 2.5：固定错误文本规则，Code 使用英文，Message 使用中文。
26. Milestone 2.5：固定分层规则，[报错][信息]等日志等级前缀不写入 `EngineError.Message`。
27. Milestone 2.6：新增 `EngineLogLevel` 日志等级枚举。
28. Milestone 2.6：新增 `EngineLogEntry` 日志记录值对象。
29. Milestone 2.6：新增日志等级中文前缀映射。
30. Milestone 2.6：新增 `EngineLogLevel` 与 `EngineLogEntry` 单元测试。
31. Milestone 3.1：新增 `FluidWarfare.Editor.Windows` Avalonia 编辑器项目。
32. Milestone 3.1：将 Editor 项目加入 `FluidWarfare.sln`。
33. Milestone 3.1：Editor 引用 `FluidWarfare.Core`。
34. Milestone 3.2：新增 `FluidWarfare Editor` 主窗口。
35. Milestone 3.2：新增顶部菜单、项目面板、3D 视口占位、检查器、日志面板。
36. Milestone 3.2：日志面板显示中文 `EngineLogEntry` 输出。
37. Milestone 3.3：Editor 启动日志改为由 `EngineLogEntry` 创建。
38. Milestone 3.3：日志面板继续显示中文日志文本。
39. Milestone 3.3：确认日志前缀由 `EngineLogLevel` / `EngineLogEntry` 生成，而不是 UI 手写。
40. Milestone 3.4：新增 Editor 底部状态栏。
41. Milestone 3.4：状态栏显示就绪状态、GUI 原型阶段、Core 加载状态与 Vulkan 未接入状态。
42. Milestone 3.4：整理项目面板、视口占位、检查器和日志面板的视觉层级。
43. Milestone 3.4：保持日志面板由 `EngineLogEntry` 生成中文日志文本。
44. Milestone 3.5：顶部菜单占位项支持点击后追加中文日志。
45. Milestone 3.5：项目面板占位项支持点击后发出选择事件。
46. Milestone 3.5：检查器面板支持显示项目占位项说明。
47. Milestone 3.5：状态栏新增当前选择显示。
48. Milestone 3.5：日志反馈继续由 `EngineLogEntry` 生成中文显示文本。
49. Milestone 3.5：明确 `ProjectPanel` 不直接写日志、不直接更新检查器、不直接更新状态栏。

### 修改

1. 将 `docs/` 下所有 Markdown 文档正文改为中文。
2. 修复 `docs/` 文档和 `file-tree.md` 的 Markdown 排版，使标题、段落、表格、列表和代码块独立换行。
3. 将 Markdown 文件重写为 UTF-8 无 BOM 与 LF 行尾，方便 GitHub Raw 公开验收。
4. 使用 Python 以 `newline="\n"` 重新写入 `.gitattributes`、`file-tree.md` 和所有 `docs/*.md`。
5. 将 Core 与 Tests 项目加入 `FluidWarfare.sln`。
6. Milestone 2.3.1：修复 `TimeStep` 默认值边界。
7. Milestone 2.3.1：修复 `SimulationTime.Advance` 对 `default(TimeStep)` 的处理。
8. Milestone 2.3.1：稳定 `TimeStep` / `SimulationTime` 的 `ToString` 输出。
9. Milestone 2.5.1：修复 `EngineResult` 默认值语义，明确 `default(EngineResult)` 为无效结果。
10. Milestone 2.5.1：调整 `EngineResult.IsFailure`，只有携带有效 `EngineError` 的失败结果才返回 true。
11. Milestone 2.5.1：确认日志等级前缀统一使用[]。
12. Milestone 2.5.2：统一日志等级前缀符号为[追踪][信息][警告][报错][严重]。
13. Milestone 2.6：固定日志分层规则：Message 只保存正文，显示输出时再添加等级前缀。
14. Milestone 2.6.1：修复日志等级前缀统一校验，确认 `EngineLogLevel` 与 `EngineLogEntry` 只使用[]。
15. Milestone 3.6：日志等级前缀统一改为 ASCII 方括号：[追踪][信息][警告][报错][严重]。
16. Milestone 3.6：移除文档与源码中的中文全角日志括号。
17. Milestone 3.6：保持 Message 只保存正文，显示输出时由 `EngineLogEntry` 添加等级前缀。
18. Milestone 3.6：补充 Editor GUI 面板 SRP 职责说明。
19. Milestone 3.6：日志面板改为只读多行文本区域。
20. Milestone 3.6：日志面板支持垂直滚动查看。
21. Milestone 3.6：日志文本支持选中与复制。
22. Milestone 3.6：保持日志内容由 `EngineLogEntry` 生成，`LogPanel` 只负责显示与追加文本。
23. Milestone 3.7：3D 视口占位区支持点击后发出聚焦事件。
24. Milestone 3.7：点击视口占位区后，检查器显示视口占位说明。
25. Milestone 3.7：点击视口占位区后，状态栏显示当前选择为 3D 视口。
26. Milestone 3.7：点击视口占位区后，日志面板追加 `[信息]视口获得焦点。`。
27. Milestone 3.7：保持 ViewportPlaceholderPanel 只负责发出事件，不直接写日志或更新其他面板。
28. Milestone 4.0：新增 `FluidWarfare.Project` 项目层。
29. Milestone 4.0：新增 `GameProjectInfo` 项目元数据模型。
30. Milestone 4.0：新增 `GameProjectLoader` 最小项目加载器。
31. Milestone 4.0：新增 `GameProjects/SampleProject` 示例项目。
32. Milestone 4.0：Editor 启动时加载示例项目，并在项目面板显示项目分类。
33. Milestone 4.0：新增项目加载器单元测试。
34. Milestone 4.0：保持 Core 不依赖 Project 或 Editor。
35. Milestone 4.1：新增 `SampleProjectPath`，用于从当前工作目录向上查找 `GameProjects/SampleProject/game.project.json`。
36. Milestone 4.1：Editor 启动时改为通过 `SampleProjectPath` 定位示例项目，降低不同启动目录导致加载失败的风险。
37. Milestone 4.1：项目加载失败时，项目面板显示未打开项目，检查器、状态栏与日志给出明确反馈。
38. Milestone 4.1：新增 `SampleProjectPath` 单元测试。
39. Milestone 4.2：将 `game.project.json` 的 `contentFolders` 从字符串数组升级为内容目录声明对象数组。
40. Milestone 4.2：新增 `GameContentFolderInfo`，用于描述项目内容目录的名称、显示名、用途、内容类型、是否必需和允许扩展名。
41. Milestone 4.2：`GameProjectLoader` 新增内容目录声明校验，包括字段完整性、目录名格式、内容类型格式、扩展名格式、重复声明、目录存在性与未声明目录拒绝。
42. Milestone 4.2：`SampleProject` 新增 `icons` 扩展目录声明，用于验证项目自定义内容目录。
43. Milestone 4.2：Editor 项目面板改为根据项目目录声明显示分类，不再硬编码英文目录到中文显示名的映射。

### Milestone 4.3：项目内容文件入口声明与扩展名校验

#### 新增

1. 新增 `GameContentFileInfo`，用于表示项目中合法的内容文件入口模型，保存所属目录、内容类型、文件名、相对路径和扩展名。
2. 新增 `GameContentFileScanner`，用于扫描已声明内容目录中的一级内容文件，并根据 `allowedExtensions` 校验扩展名。
3. 新增 `FluidWarfare.Tests/Project/Content/GameContentFileScannerTests.cs`，验证内容文件入口扫描的多种场景。
4. `SampleProject` 新增 `units/sample_unit.json`、`weapons/sample_weapon.json`、`icons/sample_icon.svg` 占位内容文件，用于验证内容文件入口识别。
5. 新增 `FluidWarfare.Editor.Windows/Properties/launchSettings.json` Editor 启动配置，支持 Visual Studio 启动配置与 `dotnet run` 直接从 Editor 项目目录启动。
6. 新增 `run.bat` 根目录一键构建并启动脚本，支持双击或 `run` 命令启动 Editor。

#### 修改

1. `GameProjectInfo` 新增 `ContentFiles`，用于保存项目加载后识别出的合法内容文件入口。
2. `GameProjectLoader` 接入 `GameContentFileScanner`，在目录声明校验后扫描合法内容文件入口，拒绝未允许扩展名文件和暂不支持的嵌套内容目录。
3. `GameProjectLoaderTests` 新增三个集成测试，覆盖合法内容文件、非法扩展名和嵌套子目录场景。
4. `EditorShell` 接入 `GameProjectInfo.ContentFiles`，在点击包含内容文件的内容目录时追加轻量文件入口数量日志。

### Milestone 4.4：项目校验报告

#### 新增

1. 新增 `ProjectValidationIssue`，用于表示项目校验中的单个问题，保存错误码、中文信息和问题路径。
2. 新增 `ProjectValidationReport`，用于汇总项目加载与内容扫描中的校验问题，支持 `HasIssues`、`IssueCount`、`FirstIssue` 和 `Empty`。
3. 新增 `GameContentFileScanResult`，用于表示内容文件扫描结果，包含合法内容文件入口和校验问题。
4. 新增 `FluidWarfare.Tests/Project/Validation/ProjectValidationReportTests.cs`，验证空报告、问题和数量和首个问题。
5. 新增 `FluidWarfare.Project/Validation/` 校验目录。

#### 修改

1. `GameContentFileScanner` 改为尽量收集多个内容文件问题，而不是只返回第一个错误。
2. `GameProjectLoadResult` 新增 `ValidationReport`，使项目加载失败时也能返回完整校验信息。
3. `GameProjectLoader` 汇总目录声明、未声明目录、内容文件扩展名和嵌套目录问题；`FindAllUndeclaredDirectories` 改为收集所有未声明目录而非只返回第一个。
4. `GameProjectLoaderTests` 新增四个集成测试，覆盖多问题报告、首个错误兼容和有效项目空报告；`AssertFailure` 新增 `ValidationReport` 校验。
5. `EditorShell` 项目加载失败时显示首个错误，并提示项目校验问题数量。
6. `GameContentFileScannerTests` 新增三个多问题收集测试。

### Milestone 5.0：最小 World 实体

#### 新增

1. 新增 `FluidWarfare.Engine` 项目层，用于承载最小 World 状态。
2. 新增 `WorldState`，支持创建、查询和枚举带显示名与位置的实体。
3. 新增 `WorldEntityInfo`、`PositionComponent`、`DisplayNameComponent`。
4. 新增 `FluidWarfare.Tests/Engine/World/WorldStateTests.cs`，验证最小 World 实体的创建、查询、位置和枚举。
5. Editor 启动时创建最小示例 World，并在视口聚焦时显示示例实体信息。
6. 新增 `FluidWarfare.Engine/World/` 和 `FluidWarfare.Engine/Components/` 目录。

#### 修改

1. `FluidWarfare.sln` 新增 `FluidWarfare.Engine` 项目。
2. `FluidWarfare.Editor.Windows.csproj` 新增 Engine 引用。
3. `FluidWarfare.Tests.csproj` 新增 Engine 引用。
4. `EditorShell` 在启动时创建最小 World 和示例实体，点击视口后检查器显示实体信息。
5. `file-tree.md` 新增 Engine 模块结构。

### Milestone 5.1：从项目内容生成占位实体

#### 新增

1. 新增项目内容到 Engine World 的桥接层 `FluidWarfare.Bridge.ProjectEngine`。
2. 新增 `ProjectContentWorldSeeder`，根据 `unitTemplate` 内容文件入口生成 World 占位实体。
3. 新增 `ProjectContentWorldSeedResult`，保存生成数量和来源路径。
4. 新增 `ProjectContentEntitySource`，保存 World 实体的项目内容来源信息。
5. 新增 `FluidWarfare.Tests/Bridge/ProjectEngine/World/ProjectContentWorldSeederTests.cs`。
6. 新增 `FluidWarfare.Bridge.ProjectEngine/World/` 目录。

#### 修改

1. `WorldEntityInfo` 新增可选 `Source` 字段，用于表示实体来源。
2. `WorldState.CreateEntity` 支持可选 `ProjectContentEntitySource` 参数。
3. `FluidWarfare.sln` 新增 `FluidWarfare.Bridge.ProjectEngine` 项目。
4. `Editor.Windows.csproj` 新增 Bridge 引用。
5. `Tests.csproj` 新增 Bridge 引用。
6. `EditorShell` 不再硬编码“示例单位”，改为从 `SampleProject` 的 `units/sample_unit.json` 内容文件入口创建 World 占位实体。
7. 点击视口时检查器显示占位实体名称、来源路径与位置。

### Milestone 5.2：最小 World 实体列表面板

#### 新增

1. 新增 `WorldEntityListPanel.axaml` + `.axaml.cs`，显示当前 World 实体列表。
2. 新增 `FluidWarfare.Editor.Windows/Panels/WorldEntities/` 目录。

#### 修改

1. `EditorShell.axaml` 左侧导航区拆分为上下两块：项目内容面板与 World 实体列表面板。
2. `EditorShell.axaml.cs` 接入 `WorldEntityListPanel`，在 World 创建后显示实体列表，响应实体选择事件并更新检查器、状态栏与日志。
3. `file-tree.md` 新增 World 实体列表面板结构。

### Milestone 5.3：World 实体选择与视口联动占位

#### 新增

1. 新增 `ViewportEntitySummary`，用于向视口占位面板传递当前选中 World 实体摘要。
2. `ViewportPlaceholderPanel` 支持三种显示状态：默认提示、World 为空、当前选中实体摘要。

#### 修改

1. `ViewportPlaceholderPanel.axaml` 重构为三状态布局：默认内容 / 空 World / 实体摘要。
2. `ViewportPlaceholderPanel.axaml.cs` 新增 `ShowNoWorldEntity`、`ShowEmptyWorld`、`ShowEntitySummary` 方法。
3. `EditorShell` 新增 `_selectedWorldEntity` 状态跟踪，点击 World 实体列表后同步更新视口显示，点击视口时保留选中实体或自动选择第一个实体。
4. `file-tree.md` 更新视口面板职责说明。

### Milestone 6.0：RenderScene 最小抽象

#### 新增

1. 新增 `FluidWarfare.Render` 抽象渲染层项目。
2. 新增 `RenderScene`、`RenderObjectInfo`、`RenderObjectVisualKind` 最小渲染场景模型。
3. 新增 `WorldToRenderSceneBuilder`，将 `WorldState` 转换为最小可渲染场景数据。
4. 新增 `FluidWarfare.Tests/Render/World/WorldToRenderSceneBuilderTests.cs`。
5. `ViewportEntitySummary` 新增 `VisualKindText`，视口占位区显示 `unit_marker`。

#### 修改

1. `FluidWarfare.sln` 新增 `FluidWarfare.Render` 项目。
2. `Editor.csproj` 和 `Tests.csproj` 新增 Render 引用。
3. `EditorShell` 创建 World 后生成 RenderScene，日志显示渲染对象数量。
4. `EditorShell.FindVisualKindText` 根据 RenderScene 查找当前实体的视觉类型。
5. `ViewportPlaceholderPanel.axaml` 实体摘要区新增渲染对象行。
6. `file-tree.md` 新增 Render 模块结构。

### Milestone 6.1：视口 RenderScene 调试显示

#### 新增

1. 新增 `ViewportRenderObjectSummary` 与 `ViewportRenderSceneSummary`，用于在视口占位区显示 RenderScene 调试对象列表。

#### 修改

1. `ViewportPlaceholderPanel` 底部新增 RenderScene 调试对象区域，显示所有渲染对象的名称、视觉类型、位置与来源路径。
2. `ViewportPlaceholderPanel.axaml.cs` 新增 `ShowRenderSceneSummary` 方法。
3. `EditorShell` 新增 `CreateViewportRenderSceneSummary` 和 `ToVisualKindText`，生成 RenderScene 后同步视口调试摘要。
4. `file-tree.md` 更新视口面板职责说明。

### Milestone 7.0：Vulkan 最小清屏

#### 新增

1. 启用 `FluidWarfare.Render.Vulkan` 项目层，作为 Vulkan 具体后端模块。
2. 新增 `VulkanBackendStatus`、`VulkanBackendInfo` 与 `VulkanBackendProbe`。
3. `VulkanBackendProbe` 在 Windows 下尝试探测 `vulkan-1.dll`，报告 Vulkan Loader 是否可用。
4. 新增 `FluidWarfare.Tests/Render/Vulkan/Backend/VulkanBackendInfoTests.cs`。

#### 修改

1. `FluidWarfare.sln` 新增 `FluidWarfare.Render.Vulkan` 项目。
2. `Editor.csproj` 和 `Tests.csproj` 新增 Render.Vulkan 引用。
3. `EditorShell` 启动时调用 `VulkanBackendProbe.Probe()`，输出 Vulkan 状态日志。
4. `StatusBarPanel` 新增 `SetVulkanStatus` 方法，状态栏显示 Vulkan 已接入/不可用。
5. `ViewportPlaceholderPanel` 新增 Vulkan 后端状态文本区域。
6. 本轮不创建 Vulkan Instance、Device、Surface、Swapchain，也不做真实渲染。

### Milestone 7.1：Vulkan 视口宿主占位

#### 新增

1. 新增 `VulkanViewportHostPanel`，作为未来 Vulkan Surface / Swapchain 的 Editor 视口宿主占位。
2. 新增 `VulkanViewportHostState` 与 `VulkanViewportHostInfo`，用于描述宿主占位状态。

#### 修改

1. `EditorShell.axaml` 中央视口区域拆分为上下两块：上方为 Vulkan 视口宿主面板，下方为文本调试视口。
2. `EditorShell.axaml.cs` 新增 `UpdateVulkanViewportHost`，根据 `VulkanBackendInfo` 更新宿主状态。
3. `file-tree.md` 新增 VulkanViewportHostPanel 结构。

### Milestone 7.2：项目契约与选择链路稳定化

#### 新增

1. 新增 `ProjectContentFolderSelection`，用于让项目面板发出稳定的内容目录选择值对象。
2. 新增 `SampleProjectSmokeTests`，验证仓库内 `GameProjects/SampleProject` 可加载，并暴露内容目录与内容文件入口。
3. 新增 `ProjectDependencyDirectionTests`，自动检查 Core、Project、Engine、Bridge、Render、Render.Vulkan 与 Tests 的项目依赖方向。

#### 修改

1. `game.project.json` 新增 `schemaVersion: 1`。
2. `GameProjectLoader` 只接受当前项目契约版本，缺失或未知版本会返回中文错误。
3. `GameProjectInfo` 新增 `SchemaVersion`。
4. `ProjectPanel` 选择事件从显示名字符串升级为 `ProjectContentFolderSelection`，`EditorShell` 改用 `FolderName` 查找项目内容目录。
5. 测试补充 Project / World / Render 稳定 ID 链路与依赖边界验收。

### Milestone 7.3：Vulkan Instance 最小创建与释放

#### 新增

1. `Render.Vulkan` 引入 `Silk.NET.Vulkan`，开始真实调用 Vulkan API。
2. 新增 `VulkanInstanceStatus`、`VulkanInstanceInfo` 与 `VulkanInstanceProbe`。
3. 新增 `VulkanInstanceInfoTests`，验证 Vulkan Instance 探测结果模型和轻量 Probe 结果。

#### 修改

1. `VulkanInstanceProbe` 创建 `VkInstance`，读取 API 版本与 Instance 扩展数量，并立即释放。
2. `EditorShell` 启动时显示 Vulkan Instance 创建结果、API 版本、扩展数量与耗时。
3. `ViewportPlaceholderPanel` 新增 Vulkan Instance 状态显示区域。
4. `ProjectDependencyDirectionTests` 新增 NuGet 包白名单检查，确认 `Silk.NET.Vulkan` 只进入 `Render.Vulkan`。
5. 本轮不创建 PhysicalDevice、Device、Surface、Swapchain、RenderPass、CommandBuffer，也不做真实清屏。
6. 从本里程碑开始，Vulkan 初始化类操作需要返回耗时信息，便于后续性能分析。

### Milestone 7.4：Vulkan Device 最小选择与释放

#### 新增

1. 新增 `VulkanDeviceStatus`、`VulkanDeviceInfo` 与 `VulkanDeviceProbe`。
2. 新增 `VulkanDeviceInfoTests`，验证 Vulkan Device 探测结果模型和轻量 Probe 结果。

#### 修改

1. `VulkanDeviceProbe` 枚举 PhysicalDevice，选择支持 Graphics Queue 的设备，创建 LogicalDevice 并获取 Graphics Queue，然后立即释放。
2. `EditorShell` 启动时显示 Vulkan Device 创建结果、显卡名称、设备类型、图形队列族与耗时。
3. `ViewportPlaceholderPanel` 新增 Vulkan Device 状态显示区域。
4. 本轮不创建 Surface、Swapchain、RenderPass、CommandBuffer，也不做真实清屏。
5. Vulkan Device 创建继续返回耗时信息，便于后续性能分析。

### Milestone 7.5：Vulkan Surface 宿主边界

#### 新增

1. 新增 `VulkanSurfaceStatus`、`VulkanSurfaceInfo` 与 `VulkanSurfaceProbe`。
2. 新增 `VulkanSurfaceInfoTests`，只验证 Surface 探测结果模型，不在单元测试中创建真实 Surface。
3. 新增 `VulkanViewportNativeHostInfo`，描述 Editor 视口宿主是否提供可用于 Surface 的原生窗口句柄。

#### 修改

1. `VulkanSurfaceProbe` 只接收外部传入的 Windows 原生句柄，创建 `VkInstance` 与 `VkSurfaceKHR` 后立即释放，不选择 Device，不创建 Swapchain。
2. `VulkanViewportHostPanel` 新增 Surface 状态显示，并明确当前 Avalonia 宿主尚未提供独立 Windows 视口句柄。
3. `EditorShell` 在 Vulkan Device 探测后尝试 Surface 探测；当前未取得独立视口句柄时，输出中文警告并保持编辑器可运行。
4. 本轮不创建 Swapchain、ImageView、RenderPass、Framebuffer、CommandBuffer、同步对象，不做真实渲染、不清屏、不绘制 `unit_marker`。

### Milestone 7.6：Windows 原生视口子窗口宿主

#### 新增

1. 新增 Windows Vulkan 视口原生子窗口宿主，用于为后续 `VkSurfaceKHR` 提供独立 HWND。
2. 新增 `WindowsVulkanViewportHostState`、`WindowsVulkanViewportHostInfo` 与 `WindowsVulkanViewportHostControl`。
3. 新增 Editor Windows 应用 manifest，声明 Windows 10 兼容性，满足 Avalonia `NativeControlHost` 创建子窗口要求。

#### 修改

1. `VulkanViewportHostPanel` 增加 Windows 原生宿主状态显示。
2. `EditorShell` 在窗口附加到可视树后显示原生视口句柄获取结果。
3. `VulkanViewportNativeHostInfo` 成功时返回独立子窗口 HWND 与 HINSTANCE。
4. 本轮不创建 Vulkan Surface、Swapchain、RenderPass、Framebuffer、CommandBuffer，也不做真实清屏。
5. 明确禁止使用主窗口 HWND 冒充视口 HWND。

### Milestone 7.7：Vulkan Surface 创建成功回归

#### 修改

1. `EditorShell.ProbeVulkanSurface` 移除 7.6 占位逻辑，改为复用 `VulkanViewportHostPanel` 提供的独立 HWND 与 HINSTANCE，调用 `VulkanSurfaceProbe.ProbeWindows` 创建 `VkSurfaceKHR`。
2. Surface 创建成功后立即释放，Editor 日志显示平台与耗时；失败时显示中文原因。
3. Windows Vulkan Surface 创建回归成功。
4. 本轮不创建 Swapchain、RenderPass、Framebuffer、CommandBuffer，也不做真实清屏。

### Milestone 7.8：Vulkan 最小可见渲染闭环

#### 新增

1. 新增 `VulkanRenderContext`，可持久管理 Vulkan Instance/Device/Surface 的生命周期，不在探测后销毁。
2. EditorShell 在获取独立 HWND 后创建 `VulkanRenderContext`，展示 Vulkan 就绪状态。

#### 修改

1. `EditorShell` 启动时不再只做探针式创建/销毁，接入持久化 Instance/Device/Surface。
2. `EditorShell` 从可视树分离时自动释放 Vulkan 资源。
3. 渲染诊断信息集中到 `VulkanViewportHostPanel` 显示。

#### 已知问题

1. Swapchain 创建在当前 Windows 驱动上因 `vkGetDeviceProcAddr` 返回的函数指针与调用约定兼容性问题，调用时触发原生访问冲突（0xC0000005）。
2. Instance/Device/Surface 创建正常，Editor 不崩溃，Vulkan 状态正常显示。
3. Swapchain/清屏问题待后续排查：设备扩展启用确认、函数指针加载方式、Silk.NET KHR 扩展类加载。

### Milestone 7.8.1：Swapchain 扩展加载修复

#### 新增

1. 新增 `VulkanSwapchainStatus`、`VulkanSwapchainInfo` 与 `VulkanSwapchainProbe`。
2. `VulkanSwapchainProbe` 在独立探针中创建 Swapchain，不污染持久上下文。
3. 新增 `FluidWarfare.Tests/Render/Vulkan/Swapchain/VulkanSwapchainInfoTests.cs`。

#### 修改

1. 修复 Swapchain 创建：使用 `SwapchainKHR*` 指针参数替代 `out SwapchainKHR`，避免调用约定不匹配导致的原生访问冲突。
2. `EditorShell` 新增 `ProbeVulkanSwapchain` 调用，显示 Swapchain 创建状态。
3. Device 创建时启用 `VK_KHR_swapchain`。
4. 查询 `SurfaceCapabilities`、`SurfaceFormats`、`PresentModes`，选择 Graphics+Present 队列族。
5. 使用 `vkGetDeviceProcAddr` 加载 swapchain 设备级函数。

#### 当前状态

- Instance/Device/Surface/Swapchain 创建全链路验证通过 ✅
- 不再有 0xC0000005 ✅
- Editor 不崩溃 ✅
- 仍然不创建 RenderPass/Framebuffer/CommandBuffer/同步对象
- 仍然不做真实清屏

### Milestone 7.8.2：Vulkan 最小清屏

#### 新增

1. 新增 `VulkanClearStatus`、`VulkanClearInfo` 与 `VulkanClearProbe`。
2. `VulkanClearProbe` 创建完整清屏链路：Instance → Surface → Device → Swapchain → ImageViews → RenderPass → Framebuffers → CommandPool → CommandBuffer → Semaphore/Fence → Acquire → Clear → Submit → Present。
3. 清屏颜色为明显深蓝色 `rgba(0.03, 0.08, 0.18, 1.00)`，与空窗口黑色背景明确区分。
4. 新增 `FluidWarfare.Tests/Render/Vulkan/Clear/VulkanClearInfoTests.cs`。

#### 修改

1. `EditorShell` 新增 `ProbeVulkanClear` 与 `ShowVulkanClearInfo`，在 HWND 就绪后执行清屏探测并显示结果。
2. 所有 Vulkan 函数委托继续使用 7.8.1 已验证的指针参数模式，无 0xC0000005。

#### Milestone 7 完成声明

Milestone 7 渲染链路全部验证通过：

```text
Loader     ✅ 7.0
Instance   ✅ 7.3
Device     ✅ 7.4
HWND       ✅ 7.6
Surface    ✅ 7.7
Swapchain  ✅ 7.8.1
Clear      ✅ 7.8.2（本里程碑）
```

下一阶段进入 **Milestone 8：战场视口基础与 RenderScene 对象绘制**。

### Milestone 7.8.3：底部调试终端与主视口收束

#### 新增

1. 新增 `DebugDockPanel`，底部日志区域升级为调试终端，包含“日志 / 渲染诊断 / RenderScene / 性能”四个页签。
2. 新增 `RenderDiagnosticsPanel` 内容（内嵌在 DebugDockPanel 中），汇总 Vulkan 全链路状态。

#### 修改

1. `VulkanViewportHostPanel` 收束为纯战场视口，只保留标题、NativeControlHost 和一行清屏状态文本，移除大段诊断文本。
2. `EditorShell.axaml` 底部 Row 2 从直接 LogPanel 替换为 DebugDockPanel。
3. `EditorShell` 新增 `UpdateAllDiagnostics`，将 Vulkan 后端、Instance、Device、Surface、Swapchain、Clear 状态集中发送到渲染诊断页签。
4. `EditorShell` 更新 RenderScene 调试列表到 RenderScene 页签。
5. Vulkan/Instance/Device 调试文本不再显示在主视口中。
6. Vulkan 清屏区域高度不再受限（NativeControlHost 占满视口区域）。

### 删除

1. 删除由 .NET SDK 默认模板临时生成的 `FluidWarfare.slnx`。

---

### Milestone 8.0：RenderScene 单对象 GPU 点位绘制

#### 新增

1. 新增 `FluidWarfare.Render.Vulkan/Markers/` 目录，包含点位绘制模型与渲染器。
2. 新增 `VulkanMarkerDrawStatus` 枚举（NotChecked / Succeeded / Failed）。
3. 新增 `VulkanMarkerDrawInfo` 记录模型，包含 DisplayName、PixelX、PixelY、PixelSize、ColorText，以及 `FromWorldPosition` 和 `CreateAtCenter` 工厂方法。
4. 新增 `VulkanMarkerDrawResult` 记录模型，包含 Status、Message、DrawnMarkerCount、ElapsedMilliseconds 和 IsSucceeded 快捷属性。
5. 新增 `VulkanMarkerClearRectRenderer` 探测类，使用 `vkCmdClearAttachments` 在 RenderPass 内绘制点位小方块（浅黄色），不创建 Shader/Pipeline/Mesh/Texture。
6. 新增 `DebugDockPanel` 渲染诊断 Tab 的 Marker 状态行（`DiagMarker`）。
7. 新增 `DebugDockPanel` 性能 Tab 的 `MarkerDraw` 耗时行（`PerfMarker`）。
8. EditorShell 新增 `_vulkanMarkerDrawResult` 状态、`ProbeVulkanMarkerDraw` 方法和 `ShowVulkanMarkerDrawInfo` 方法。
9. EditorShell 清屏状态行新增点位数量显示（如“清屏成功 | 点位：1 | rgba(0.03, ...) | 真实宽高”）。
10. 新增 `VulkanMarkerDrawInfoTests`（7 个测试）和 `VulkanMarkerDrawResultTests`（7 个测试）。
11. 基于 RenderScene 第一个对象绘制 Vulkan GPU 点位。

#### 修改

1. `DebugDockPanel.axaml.cs` — `SetDiagnostics` 新增 `marker` 参数，`SetPerformance` 新增 `markerMs` 参数。
2. `EditorShell` — `UpdateAllDiagnostics` 新增 Marker 诊断与 MarkerDraw 耗时上报到 DebugDockPanel。
3. `EditorShell` — 在 `ReportVulkanViewportNativeHost` 的 Vulkan Clear 探测后新增 `ProbeVulkanMarkerDraw` 调用。
4. `file-tree.md` — 新增 Markers 目录、Milestone 8.0 新增文件列表和文件表条目。
5. 修复 Vulkan 原生子窗口尺寸同步，NativeControlHost 的 HWND 会跟随 Avalonia 控件 Bounds 调整。
6. 修复清屏与点位绘制使用固定 640x360 的问题，改为使用真实视口尺寸。
7. 修复全屏后 Vulkan 画面只占左上角的问题。
8. 压缩底部调试终端页签字号和 Padding，降低调试区对主视口的干扰。

#### 技术要点

1. **无 Shader 方案**：使用 `vkCmdClearAttachments` 在 RenderPass 内直接绘制矩形色块，避免 Shader/Pipeline/Buffer 依赖。
2. **坐标映射初版**：`pixelX = 实际视口宽度 / 2 + worldX * 10`、`pixelY = 实际视口高度 / 2 - worldZ * 10`。
3. **点位颜色**：`rgba(1.00, 0.82, 0.20, 1.00)`（浅黄色），与深蓝背景明显区分。
4. **全链路探测模式**：与 VulkanClearProbe 一致的 Instance → Surface → Device → Swapchain → 渲染 → Present → 清理模式。
5. **点位大小**：固定 12x12 像素，自动限制在视口范围内。
6. **空场景处理**：RenderScene 无对象时输出警告并跳过绘制。
7. **尺寸同步**：Avalonia 控件 Bounds → Win32 子窗口 `SetWindowPos` → NativeHostInfo Width/Height → Swapchain/Clear/Marker 参数。

#### 新增文件（4 项 Render.Vulkan + 2 项 Tests）

```text
FluidWarfare.Render.Vulkan/Markers/VulkanMarkerDrawStatus.cs
FluidWarfare.Render.Vulkan/Markers/VulkanMarkerDrawInfo.cs
FluidWarfare.Render.Vulkan/Markers/VulkanMarkerDrawResult.cs
FluidWarfare.Render.Vulkan/Markers/VulkanMarkerClearRectRenderer.cs
FluidWarfare.Tests/Render/Vulkan/Markers/VulkanMarkerDrawInfoTests.cs
FluidWarfare.Tests/Render/Vulkan/Markers/VulkanMarkerDrawResultTests.cs
```

#### 通过规则

1. ✅ Vulkan 视口显示深蓝色清屏背景 + 中央浅黄色小方块（12x12 px）。
2. ✅ 小方块来自 RenderScene 第一个对象（sample_unit）。
3. ✅ 坐标初版映射为视口中心（world 0,0 → viewport center）。
4. ✅ 不写 Shader、Pipeline、Mesh、Texture、GPU Buffer。
5. ✅ 不绘制正式单位图标。
6. ✅ Render.Vulkan 不依赖 Editor，Tests 不依赖 Editor.Windows。
7. ✅ build 0 错误 0 警告（已验证）。
8. ✅ test 全部通过（260/260，新增 14 个测试）。
9. ✅ 日志显示“Vulkan 清屏 + 点位绘制成功”。
10. ✅ 渲染诊断 Tab 显示 Marker 绘制结果。
11. ✅ RenderScene Tab 仍显示 sample_unit 信息。
12. ✅ 性能 Tab 显示 MarkerDraw 耗时。
13. ✅ 文档同步。

---

### Milestone 8.0.1：Vulkan 战场视口填充与重绘修复

#### 修改

1. `WindowsVulkanViewportHostControl` 在 Avalonia `Bounds` 变化时调用 Win32 `SetWindowPos`，同步原生子窗口真实宽高。
2. `WindowsVulkanViewportHostControl` 新增 `HostInfoChanged` 事件，尺寸变化后向上报告最新 HWND、HINSTANCE、Width 与 Height。
3. `VulkanViewportHostPanel` 新增 `NativeHostInfoChanged` 事件，将原生宿主尺寸变化转交给 `EditorShell`。
4. `EditorShell` 移除旧的 `VulkanRenderContext` 16ms 定时渲染路径，避免旧 Swapchain extent 在最大化后覆盖左上角区域。
5. `EditorShell` 增加 resize 防抖重绘路径：宿主尺寸变化后重新执行 Swapchain / Clear / MarkerDraw 一次。
6. `EditorShell` 的主视口状态行在 Marker 绘制完成后同步刷新，显示本轮真实清屏尺寸与点位数量。
7. `DebugDockPanel` 压缩底部页签字号与 Padding，减少底部调试区对中央战场视口的挤压。

#### 验收重点

1. Vulkan 蓝色战场区域跟随中央视口真实尺寸铺满，不再停留在旧的小尺寸。
2. 窗口最大化或 resize 后，会重新 Clear 并绘制 sample_unit 点位。
3. 点位仍来自 RenderScene 第一个对象，并位于蓝色区域中心附近。
4. 本补丁不进入 8.1，不新增多对象绘制，不创建 Shader、Pipeline、Mesh、Texture 或 GPU Buffer。

14. ✅ 最大化后 NativeHost 审计尺寸不再固定为 640x360。

---

---

### Milestone 8.1：Vulkan 3D 基础管线

#### 新增

1. 新增 `FluidWarfare.Render.Vulkan/Scene3D/` 目录，包含 3D 场景渲染模型与渲染器。
2. 新增 `VulkanScene3dStatus` 枚举（NotChecked / Succeeded / Failed）。
3. 新增 `VulkanScene3dInfo` 记录模型，包含状态、消息、GridVertexCount、GridLineCount、UnitVertexCount、UnitTriangleCount、DrawCallCount、ViewportWidth/Height、CameraSummary 和 ElapsedMilliseconds。
4. 新增 `VulkanScene3dVertex` 结构（Position + Color）和 `VulkanScene3dVertices` 顶点生成工具（BuildGrid / BuildCube / BuildAxes / ToInterleaved）。
5. 新增 `VulkanScene3dRenderer` 渲染器：完整全链路（Instance → Surface → Device → Swapchain → ImageViews → RenderPass → ShaderModules → PipelineLayout → GraphicsPipelines → Framebuffers → VertexBuffers → CommandPool → CommandBuffer → PushConstant → Draw → Present → Cleanup）。
6. 新增 `FluidWarfare.Render.Vulkan/Camera/` 目录，`VulkanCameraInfo` 记录模型（默认战场相机 Position(0,18,24) → Target(0,0,0)，FOV 60°）和 `VulkanCameraMatrices` 矩阵计算（LookAt、PerspectiveVulkan、MVP 合成）。
7. 新增 `FluidWarfare.Render.Vulkan/Shaders/` 目录：`basic_3d.vert` / `.frag` GLSL 源文件和预编译 SPIR-V 二进制文件。
8. 新增 `CompiledShaders.cs`：内嵌 SPIR-V 字节码的 C# 类，用于运行时创建 VkShaderModule。
9. 新增 `DebugDockPanel` 渲染诊断 Tab 的五行 3D 状态（Scene3D / Camera / Grid / Unit / DrawCall）。
10. 新增 `DebugDockPanel` 性能 Tab 的 Scene3D 耗时行。
11. EditorShell 新增 `_vulkanScene3dInfo` 状态、`ProbeVulkanScene3D` 方法和 `ShowVulkanScene3DInfo` 方法。
12. 主视口标题更新为 "Vulkan 3D 战场视口"，状态行显示 Grid / Unit / DrawCall 信息。
13. 新增测试：`VulkanScene3dInfoTests`（8 个）、`VulkanScene3dVertexTests`（8 个）、`VulkanCameraInfoTests`（7 个）。

#### 技术要点

1. **Shader 编译策略**：使用自定义 `tools/gen_spirv` 工具通过 SPIR-V 字节编码生成预编译二进制。GLSL 源码与 SPIR-V 同时提交。
   - `tools/gen_spirv` 是极其精简的 SPIR-V 编码器，只生成当前 `basic_3d.vert`/`basic_3d.frag` 所需的最小 SPIR-V，不是通用 shader 编译器。
   - 正式路线应切换至 glslangValidator / shaderc / DXC + SPIR-V 等标准编译链。`tools/gen_spirv` 定位为临时占位方案，长期将被替代。
2. **Graphics Pipeline**：两个独立 Pipeline — 地面网格用 `LineList`（PrimitiveTopology.LineList），单位立方体用 `TriangleList`（PrimitiveTopology.TriangleList）。
3. **Vertex Format**：交错 7×float32（xyz + rgba），stride 28 字节，`R32G32B32_SFLOAT` + `R32G32B32A32_SFLOAT`。
4. **MVP 传递**：通过 Push Constant（64 字节，mat4）在每帧传入，无 Uniform Buffer 或 Descriptor 开销。
5. **相机系统**：固定 LookAt 相机，使用 Vulkan 坐标约定（NDC 深度 0..1，Y 翻转）。
6. **地面网格**：范围 -20 到 +20，间隔 2，Y=0，共 84 个顶点（42 条线段），蓝灰色。
7. **单位占位物**：1×1×1 立方体，36 顶点（12 三角形），浅黄色 `rgba(1.00, 0.82, 0.20, 1.00)`。
8. **全链路探测模式**：与 ClearProbe / MarkerRenderer 一致的一次性创建→渲染→清理模式。
9. **内存管理**：Host Visible + Host Coherent 内存，不做 staging buffer 或 GPU-only 优化。

#### 新增文件清单

```text
FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dStatus.cs
FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dInfo.cs
FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dVertex.cs
FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dRenderer.cs
FluidWarfare.Render.Vulkan/Camera/VulkanCameraInfo.cs
FluidWarfare.Render.Vulkan/Camera/VulkanCameraMatrices.cs
FluidWarfare.Render.Vulkan/Shaders/basic_3d.vert
FluidWarfare.Render.Vulkan/Shaders/basic_3d.frag
FluidWarfare.Render.Vulkan/Shaders/Compiled/basic_3d.vert.spv
FluidWarfare.Render.Vulkan/Shaders/Compiled/basic_3d.frag.spv
FluidWarfare.Render.Vulkan/Shaders/CompiledShaders.cs
FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dInfoTests.cs
FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dVertexTests.cs
FluidWarfare.Tests/Render/Vulkan/Camera/VulkanCameraInfoTests.cs
tools/gen_spirv/Program.cs
tools/gen_spirv/gen_spirv.csproj
```

#### 通过规则

1. ✅ 3D 坐标约定：XZ 地面平面，+Y 高度。
2. ✅ 固定相机：斜俯视 (0,18,24) → (0,0,0)，FOV 60°。
3. ✅ View / Projection / MVP 矩阵正确计算。
4. ✅ 预编译 SPIR-V 成功加载并创建 ShaderModule。
5. ✅ 两个 Graphics Pipeline 创建成功（LineList + TriangleList）。
6. ✅ Vertex Buffer 创建成功并上传顶点数据。
7. ✅ 地面网格绘制 84 顶点/42 线段。
8. ✅ 单位占位立方体绘制 36 顶点/12 三角形。
9. ✅ RenderScene Position 被当作 3D 世界坐标。
10. ✅ 不再把单位位置直接映射为 2D 像素点。
11. ✅ Pipeline / Shader / Buffer 创建成功，无崩溃。
12. ✅ build 0 错误 0 警告。
13. ✅ test 282/282 全部通过（新增 22 个测试：Scene3dInfo 8 + Scene3dVertex 7 + CameraInfo 7）。
14. ✅ 渲染诊断 Tab 显示 Scene3D / Camera / Grid / Unit / DrawCall。
15. ✅ 性能 Tab 显示 Scene3D 耗时。
16. ✅ 不做纹理 / 材质 / 光照 / 阴影 / 模型加载。
17. ✅ 不做 Android / Runtime。
18. ✅ 文档同步。

---

---

### Milestone 8.1.3 — Scene3D 隔离、手写 SPIR-V 废弃

#### 变更记录

```text
稳定基线：7f5870b — Milestone 8.0.1（Clear / Marker / Resize，Editor 稳定运行）
问题引入：c30de44 — Milestone 8.1（手写 SPIR-V 生成器，指令操作数顺序错误）
部分回退：e0b9602 — resize 回退 Clear probe
硬编码修：69e4aad — 补 CmdBindPipeline（但 SPIR-V 本身非法，仍闪退）
当前提交：1687842 — Scene3D 自动启动禁用
```

#### 根因总结

`tools/gen_spirv` 手写的 SPIR-V 编码器中，所有结果型指令（OpConstant、OpVariable、OpLoad、OpFunction、OpAccessChain、OpMatrixTimesVector 等）的 **Result Type 和 Result <id> 操作数顺序写反**，导致生成的 `.spv` 文件语义非法，在 driver 编译 shader 时触发崩溃。

完整证据链见 `tools/gen_spirv/README.md`。

#### 改动

1. **Scene3D 已隔离**：Editor 启动和 resize 均不再自动调用 `ProbeVulkanScene3D()`。
2. **`tools/gen_spirv` 已废弃**：目录保留 `README.md` 说明，不参与构建。
3. **`CompiledShaders.cs` 改为空数组占位**：等标准工具链就绪后替换。
4. **`.spv` 文件从 git 移除**：非法的预编译二进制不再提交。
5. **`basic_3d.vert` / `basic_3d.frag` 恢复标准多行 GLSL 格式**：`#version 450` 独立一行，可被标准编译器正确解析。
6. **诊断面板显示**：Scene3D 状态为"已隔离：当前 SPIR-V 编译链未通过合法性验证。"

#### 恢复条件

Scene3D 3D 渲染在以下条件全部满足后方可重新启用：

1. 使用 `glslangValidator` / `shaderc` / DXC 等标准工具编译 SPIR-V。
2. `spirv-val` 通过生成的 `.spv` 文件。
3. `tools/gen_spirv` 不再参与构建。
4. `VulkanScene3dRenderer.cs` 拆分为小文件。

---

---

### Milestone 8.R.1 — 主线稳定闸门

#### 新增

1. 新增 `VulkanScene3dRunGate`，用于隔离实验性 Scene3D 运行路径。
2. 新增 `VulkanScene3dRunGateTests`（6 个测试），验证闸门隔离状态和 `FW_ENABLE_SCENE3D` 环境变量处理。

#### 修改

1. EditorShell 启动路径不再调用 `ProbeVulkanScene3D()`，改为 `ReportScene3dIsolation()`。
2. resize / 最大化路径只执行稳定的 `ProbeVulkanClear()`。
3. `ProbeVulkanScene3D()` 内部增加闸门检查：`_scene3dGate.CanRun` 为 false 时调用 `ReportScene3dIsolation()` 并返回。
4. DebugDock 诊断面板显示隔离原因（"Scene3D：已隔离，未设置 FW_ENABLE_SCENE3D=1。"）。
5. 主视口状态行显示 "Scene3D 已隔离"。

#### 闸门设计

```csharp
VulkanScene3dRunGate.Evaluate()
  ↓
FW_ENABLE_SCENE3D == "1" ?
  ├── No  → Isolated("未设置 FW_ENABLE_SCENE3D=1。")
  └── Yes → Isolated("SPIR-V 编译链未通过 spirv-val 验证。")
            （8.R.3 之前不可能返回 Ready）
```

双重闸门：即使设置了 `FW_ENABLE_SCENE3D=1`，因为 SPIR-V 未通过 spirv-val 验证，
`CanRun` 仍为 false。Editor 不会因 Scene3D 自动崩溃。

#### 新增文件

```text
FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dRunGate.cs
FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dRunGateTests.cs
```

---

### Milestone 8.R.2 — 标准 Shader 编译链

#### 新增

1. 新增 `tools/shaders/compile_basic_3d.ps1`：使用 `glslangValidator` 编译 GLSL → SPIR-V。
2. 新增 `tools/shaders/validate_basic_3d.ps1`：使用 `spirv-val` 验证 SPIR-V 合法性。
3. 新增 `tools/shaders/README.md`：记录 shader 编译链与工具链说明。
4. 新增 `FluidWarfare.Render.Vulkan/Shaders/Compiled/.gitkeep`，确保目录被 git 跟踪。

#### 技术要点

1. **Shader 编译链**：GLSL 源码 → `glslangValidator` → `.spv` → `spirv-val` 验证。
2. **禁止手写 SPIR-V**：`tools/gen_spirv` 已确认为 Milestone 8.1 闪退根因，后续不再使用。
3. **编译脚本行为**：找不到 `glslangValidator` 时输出中文错误提示并返回非 0 退出码，不生成伪造 `.spv`。
4. **验证脚本行为**：找不到 `spirv-val` 或 `.spv` 文件缺失时输出中文错误提示并返回非 0 退出码。
5. **Scene3D 保持隔离**：本轮只建立工具链，不恢复 3D 自动启动。

#### 新增文件

```text
tools/shaders/compile_basic_3d.ps1
tools/shaders/validate_basic_3d.ps1
tools/shaders/README.md
FluidWarfare.Render.Vulkan/Shaders/Compiled/.gitkeep
```

---

---

### Milestone 8.R.3 — SPIR-V 验证闸门与运行接入

#### 新增

1. 新增 `tools/shaders/embed_basic_3d_shaders.ps1`：在 spirv-val 验证通过后将 .spv 字节写入 CompiledShaders.cs。脚本独立执行 spirv-val 并检查 SPIR-V 魔数，不依赖外部验证步骤。
2. 新增 `DebugDockPanel` 渲染诊断 Tab 的 "运行 Scene3D 探针" 按钮（默认禁用）。
3. 新增 `DebugDockPanel.Scene3dRunRequested` 事件，EditorShell 订阅后调用 `TryRunScene3dProbeManually`。
4. 新增 `CompiledShaders.HasValidatedBasic3dShaders` 属性，用于 RunGate 判断 shader 是否就绪。
5. 新增 `CompiledShaders.Reset()` 供测试重置状态。
6. 新增 `FluidWarfare.Tests/Render/Vulkan/Shaders/CompiledShadersTests.cs`（6 个测试）。
7. RunGateTests 新增 3 个测试：缺失 shader 时隔离、验证后 Ready、Ready 消息。

#### 修改

1. `CompiledShaders.cs`：从静态空数组改为含验证标记的结构，`Basic3dVert`/`Basic3dFrag` 改为 `internal set`，新增 `HasValidatedBasic3dShaders`、`Basic3dVertexValidatedBySpirvVal`、`Basic3dFragmentValidatedBySpirvVal`、`Reset()`。
2. `VulkanScene3dRunGate.Evaluate()`：新增 `CompiledShaders.HasValidatedBasic3dShaders` 检查。FW_ENABLE_SCENE3D=1 且 shader 已验证时返回 Ready。
3. EditorShell：新增 `HandleScene3dRunRequested`、`TryRunScene3dProbeManually` 方法，执行闸门前检查。
4. `UpdateVulkanViewportStatusLine`：新增 "Scene3D Ready，等待手动触发" 状态行。
5. `UpdateAllDiagnostics`：同步 Scene3D 手动触发按钮启用状态。

#### 运行闸门规则

```csharp
FW_ENABLE_SCENE3D 未设置                     → Isolated
FW_ENABLE_SCENE3D=1 但 shader 未验证           → Isolated
FW_ENABLE_SCENE3D=1 且 shader 已验证           → Ready（仅允许手动触发）
```

#### 工具链状态

本机 `glslangValidator` 和 `spirv-val` 均不可用，.spv 未实际生成。
`CompiledShaders.cs` 保持空数组。所有脚本已创建、闸门已就绪，只待工具链执行：

```powershell
powershell -ExecutionPolicy Bypass -File tools/shaders/compile_basic_3d.ps1
powershell -ExecutionPolicy Bypass -File tools/shaders/validate_basic_3d.ps1
powershell -ExecutionPolicy Bypass -File tools/shaders/embed_basic_3d_shaders.ps1
```

#### 新增文件

```text
tools/shaders/embed_basic_3d_shaders.ps1
FluidWarfare.Tests/Render/Vulkan/Shaders/CompiledShadersTests.cs
```

---

---

### Milestone 8.R.4 — Vulkan Validation Layer 开关

#### 新增

1. 新增 `FluidWarfare.Render.Vulkan/Validation/` 目录（6 个文件 + 3 个测试文件）。
2. 新增 `VulkanValidationOptions`：从 `FW_VULKAN_VALIDATION=1` 环境变量读取是否请求启用 Validation。
3. 新增 `VulkanValidationAvailabilityProbe`：检测 `VK_LAYER_KHRONOS_validation` 和 `VK_EXT_debug_utils` 是否可用。
4. 新增 `VulkanDebugMessengerScope`：持有 `DebugUtilsMessengerEXT` 生命周期，持有 callback delegate 防止 GC 回收。
5. 新增 `VulkanValidationMessageStore`：保存最近 20 条 Validation 消息。
6. 新增 `VulkanValidationStatus` / `VulkanValidationInfo` / `VulkanValidationMessageInfo` 模型。
7. EditorShell 启动时自动执行 `ProbeVulkanValidation`。
8. DebugDock 渲染诊断 Tab 新增 `Validation` 状态行。
9. 新增测试 15 个（Options 3 + Info 7 + MessageStore 5）。

#### 技术要点

1. **环境变量分离**：`FW_VULKAN_VALIDATION` 只控制诊断层，`FW_ENABLE_SCENE3D` 只控制 Scene3D 运行，互不替代。
2. **Availability Probe**：不创建 Instance，只调用 `EnumerateInstanceLayerProperties` 和 `EnumerateInstanceExtensionProperties`。
3. **Debug Messenger**：使用函数指针加载 `vkCreateDebugUtilsMessengerEXT` / `vkDestroyDebugUtilsMessengerEXT`，通过 `vkGetInstanceProcAddr` 获取。
4. **Delegate 生命周期**：`VulkanDebugMessengerScope` 持有 `_callback` 字段防止 GC 回收。
5. **Instance 扩展追加**：启用 Validation 时 Instance 只能追加 `VK_EXT_debug_utils`，不能替换原有 Surface 扩展。
6. **安全原则**：所有检测失败均以中文提示返回，不导致 Editor 崩溃。

#### 新增文件

```text
FluidWarfare.Render.Vulkan/Validation/VulkanValidationStatus.cs
FluidWarfare.Render.Vulkan/Validation/VulkanValidationInfo.cs
FluidWarfare.Render.Vulkan/Validation/VulkanValidationOptions.cs
FluidWarfare.Render.Vulkan/Validation/VulkanValidationMessageInfo.cs
FluidWarfare.Render.Vulkan/Validation/VulkanValidationMessageStore.cs
FluidWarfare.Render.Vulkan/Validation/VulkanValidationAvailabilityProbe.cs
FluidWarfare.Render.Vulkan/Validation/VulkanDebugMessengerScope.cs
FluidWarfare.Tests/Render/Vulkan/Validation/VulkanValidationOptionsTests.cs
FluidWarfare.Tests/Render/Vulkan/Validation/VulkanValidationInfoTests.cs
FluidWarfare.Tests/Render/Vulkan/Validation/VulkanValidationMessageStoreTests.cs
```

---

---

### Milestone 8.R.5 — Scene3D Renderer 拆分

#### 改动

1. **VulkanScene3dShaderModules**（71 行）：使用 CompiledShaders 已验证 SPIR-V 创建 ShaderModule，检查字节非空和验证标记。
2. **VulkanScene3dPipelineLayout**（50 行）：创建 PipelineLayout 与 64 字节 MVP PushConstant。
3. **VulkanScene3dPipelines**（139 行）：创建 Grid (LineList) 和 Unit (TriangleList) Pipeline。
4. **VulkanScene3dVertexBuffers**（139 行）：创建并上传 Host Visible 顶点 Buffer，检查输入保护和内存类型。
5. **VulkanScene3dCommandRecorder**（116 行）：录制 CommandBuffer，录制顺序为 BindPipeline → PushConstants → BindVertexBuffers → Draw（两条），检查 Begin/EndCommandBuffer 返回值。
6. **VulkanScene3dRenderResources**（114 行）：集中持有 Scene3D 创建的 Vulkan 资源，按依赖逆序在 Dispose 中释放。
7. **VulkanScene3dRenderer**（386 行，含公共基础设施）：收束为流程编排层，调用各子模块 + 公共 Instance/Surface/Device/Swapchain 创建。

#### 行为保持

- Scene3D 仍不进入默认启动或 resize 路径。
- resize / 最大化仍只走 Clear。
- FW_ENABLE_SCENE3D=1 只允许手动触发。
- Draw 前明确 BindPipeline。
- Begin/EndCommandBuffer 结果被检查。

---

---

### Milestone 8.1R — Vulkan 3D 基础管线重启

#### 前置状态

五层安全地基已完成：

```text
8.R.1 — 主线稳定闸门 ✅
8.R.2 — 标准 Shader 编译链 ✅
8.R.3 — SPIR-V 验证闸门 ✅
8.R.4 — Validation Layer 开关 ✅
8.R.5 — Renderer 职责拆分 ✅
```

#### 本轮变更

1. 使用 `glslangValidator` 重新编译 `basic_3d.vert` / `basic_3d.frag` → `.spv`。
2. `spirv-val` 验证通过（vert: 1232 字节, frag: 376 字节）。
3. `CompiledShaders.cs` 重新嵌入真实 SPIR-V 字节。
4. `HasValidatedBasic3dShaders == true`。

#### 启动验收

```powershell
# 默认启动 — Scene3D 不运行，Editor 稳定
Remove-Item Env:FW_ENABLE_SCENE3D -ErrorAction SilentlyContinue
Remove-Item Env:FW_VULKAN_VALIDATION -ErrorAction SilentlyContinue
dotnet run --project FluidWarfare.Editor.Windows --no-build

# 手动触发模式 — Scene3D Ready，点击按钮后执行
$env:FW_ENABLE_SCENE3D="1"
$env:FW_VULKAN_VALIDATION="1"
dotnet run --project FluidWarfare.Editor.Windows --no-build
```

#### 验证结果

- ✅ glslangValidator 可用（11:16.2.0）
- ✅ spirv-val 可用（SPIRV-Tools v2026.2）
- ✅ SPIR-V 编译通过
- ✅ SPIR-V 验证通过
- ✅ CompiledShaders 嵌入真实字节
- ✅ dotnet build: 0 错误, 0 警告
- ✅ dotnet test: 311/311 全部通过
- ✅ Editor 默认启动稳定（无 env vars）
- ✅ Editor 在 FW_ENABLE_SCENE3D=1 + FW_VULKAN_VALIDATION=1 下稳定
- ⏳ Scene3D 手动触发：**待用户 GUI 验收**

---

---

### Milestone 8.1R.3 — Scene3D 可见性修复与 DebugDock 清理

#### 修复

1. **Scene3D 不可见根因修复**：`VulkanCameraMatrices.cs` 透视矩阵 `[2][2]` 从 `near/range` 改为 `far/range`，修复 Vulkan NDC 深度裁剪导致全部几何体被裁剪的问题。此前所有顶点 `clip_z < 0`，被 Vulkan 裁剪阶段剔除。
2. **清屏覆盖问题修复**：手动触发 Scene3D 前取消待执行的 resize 防抖定时器 `_viewportResizeRenderTimer.Stop()`，防止异步 Clear 覆盖 Scene3D 画面。

#### 移除

1. **DebugDockPanel Scene3D 按钮与分隔线完全移除**：XAML 删除 `Scene3dRunSeparator` 与 `Scene3dRunButton`；code-behind 删除 `_scene3dRunSeparator`、`_scene3dRunButton`、`Scene3dRunRequested` 事件、`Scene3dRunButtonEnabled` 属性、`HandleScene3dRunButtonClicked` 方法及对应 `FindControl` 调用。
2. **EditorShell DebugDock 按钮引用移除**：删除 `Scene3dRunRequested` 订阅与 `Scene3dRunButtonEnabled` 设置。

#### 新增

1. **渲染模式追踪**：新增 `_renderLastMode` 字段，Clear 成功后设为 `"Clear"`，Scene3D 成功后设为 `"Scene3D"`。
2. **RenderSeq 日志**：每次 Clear/Scene3D 输出 `[信息] RenderSeq-NNN | Mode | WxH | reason` 格式日志，含序列号、尺寸和触发原因。
3. **状态栏最近渲染模式显示**：视口状态行追加 ` | 最近渲染：Clear/Scene3D`。

#### 修改文件

```text
FluidWarfare.Render.Vulkan/Camera/VulkanCameraMatrices.cs
FluidWarfare.Editor.Windows/Panels/DebugDock/DebugDockPanel.axaml
FluidWarfare.Editor.Windows/Panels/DebugDock/DebugDockPanel.axaml.cs
FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs
docs/CHANGELOG.md
file-tree.md
```

#### 验证结果

- ✅ dotnet build: 0 错误, 0 警告
- ✅ dotnet test: 311/311 全部通过
- ✅ `grep Scene3dRunButton|Scene3dRunSeparator|Scene3dRunButtonEnabled` 零匹配
- ✅ `ProbeVulkanClear()` 调用时传入 reason（初始启动、resize）
- ✅ `TryRunScene3dProbeManually()` 开头 `_viewportResizeRenderTimer?.Stop()`
- ⏳ 人工验证：启动 Editor →「运行」→「运行 Scene3D 探针」，确认主视口显示 Grid + Cube

---

---

### Editor UI 布局补丁 — 面板标题与日志区域整理

#### 修改

1. 收紧 ProjectPanel、WorldEntityListPanel、InspectorPanel 与 VulkanViewportHostPanel 的标题字号和内边距。
2. 收紧 DebugDockPanel 底部页签字号、Padding 与内容区 Padding。
3. 移除 LogPanel 内部重复的“日志”标题，让日志 TextBox 直接占满底部日志页签内容区。

#### 验收重点

1. 左侧、右侧与视口标题不再显得过大。
2. 底部日志页签中不再重复显示第二个“日志”标题。
3. 日志文本框占满日志页签内容区域。

---

---

---

### Milestone 8.2 — 多对象 3D 绘制与基础 Depth Buffer

#### 新增

1. **两个 SampleProject 单位内容文件**：`sample_unit_2.json`、`sample_unit_3.json`，与现有 `sample_unit.json` 形成 3 个单位。
2. **World 实体生成扩展**：`ProjectContentWorldSeeder` 使用 8.2 占位布局 `(-4,0,1)` `(0,0,0)` `(1,0,-3)`，第三个与第二个重叠以验证深度遮挡。
3. **RenderScene 多对象转换**：`WorldToRenderSceneBuilder` 遍历全部实体生成多个 `RenderObjectInfo`。
4. **Depth Buffer 系统**（3 个新文件）：
   - `VulkanScene3dDepthFormatSelector`：查询 `D32Sfloat → D32SfloatS8Uint → D24UnormS8Uint` 候选深度格式。
   - `VulkanScene3dDepthAttachmentInfo`：保存深度格式选择结果与诊断信息。
   - `VulkanScene3dDepthAttachments`：为每个 Swapchain Image 创建 Depth Image / Memory / ImageView。
5. **RenderPass 深度附件**：第二个 Attachment（LoadOp=Clear, StoreOp=DontCare），Subpass 设置 `PDepthStencilAttachment`。
6. **Framebuffer 双附件**：每个 Framebuffer 附加 Color + Depth ImageView，AttachmentCount = 2。
7. **Pipeline 深度状态**：GridPipeline 与 UnitPipeline 均启用 `DepthTestEnable=True`、`DepthWriteEnable=True`、`CompareOp=Less`。
8. **每对象 MVP**：共享 Unit Vertex Buffer，每对象 `Model = Translation × Scale`，`MVP = VP × Model`，CommandBuffer 中循环 `BindPipeline → BindVBs → PushConstants(MVP) → Draw`。
9. **相机调整**：`DefaultBattlefield` 位置 `(0,22,32)`、FOV `55°`。
10. **Grid Y 偏移**：`BuildGrid` 默认 `yOffset = -0.01f` 避免 Z-fighting。
11. **VulkanScene3dInfo 扩展**：新增 `RenderObjectCount`、`RenderedUnitCount`、`IgnoredObjectCount`、`DepthFormat`、`DepthAttachmentCount`、`DepthTestEnabled`。
12. **测试 9 个新增**：DepthFormatSelector 候选顺序/Stencil 判定、DepthAttachmentInfo 纯逻辑、BuildGrid Y 偏移、Info 新字段。

#### 移除

无。

#### 修改文件

```text
GameProjects/SampleProject/units/sample_unit_2.json          (新增)
GameProjects/SampleProject/units/sample_unit_3.json          (新增)
FluidWarfare.Render.Vulkan/Scene3D/Depth/VulkanScene3dDepthFormatSelector.cs     (新增)
FluidWarfare.Render.Vulkan/Scene3D/Depth/VulkanScene3dDepthAttachmentInfo.cs      (新增)
FluidWarfare.Render.Vulkan/Scene3D/Depth/VulkanScene3dDepthAttachments.cs         (新增)
FluidWarfare.Tests/Render/Vulkan/Scene3D/Depth/VulkanScene3dDepthFormatSelectorTests.cs  (新增)
FluidWarfare.Tests/Render/Vulkan/Scene3D/Depth/VulkanScene3dDepthAttachmentInfoTests.cs  (新增)
ProjectContentWorldSeeder.cs               — 位置布局
VulkanScene3dInfo.cs                       — 新增字段
VulkanScene3dRenderResources.cs            — 深度资源 + Dispose
VulkanScene3dRenderer.cs                   — RenderPass/Framebuffer 深度 + 多对象 MVP
VulkanScene3dPipelines.cs                  — DepthStencilState
VulkanScene3dCommandRecorder.cs            — 2 ClearValues + 每对象循环
VulkanCameraInfo.cs                        — DefaultBattlefield
VulkanCameraMatrices.cs                    — CreateTranslation/CreateScale/Mul public
VulkanScene3dVertex.cs                     — UnitDrawInfo struct + Grid Y offset
EditorShell.axaml.cs                       — 传入选中的 UnitDrawInfo、诊断更新
DebugDockPanel.axaml.cs                    — 诊断文本扩展（多对象/深度）
VulkanCameraInfoTests.cs                   — 适配新参数
VulkanScene3dVertexTests.cs                — 适配 Y offset
VulkanScene3dInfoTests.cs                  — 新字段测试
docs/CHANGELOG.md
file-tree.md
```

#### 验证结果

- ✅ dotnet build: 0 错误, 0 警告
- ✅ dotnet test: 321/321 全部通过
- ✅ 3 个 World 实体（sample_unit / sample_unit_2 / sample_unit_3）
- ✅ 3 个 RenderObject，位置来自 RenderScene
- ✅ 共享单个 Unit Vertex Buffer（36 顶点 / 12 三角形）
- ✅ 每对象独立 MVP（平移 + 1.25 缩放）
- ✅ Depth 格式 D32Sfloat（或回退 D24UnormS8Uint）
- ✅ Depth Attachment = Swapchain Image 数量
- ✅ Pipeline DepthTest=Yes / DepthWrite=Yes
- ✅ DrawCall = 1 + RenderedUnitCount
- ✅ Grid Y = -0.01（防 Z-fighting）
- ✅ 不越界：无纹理、材质、光照、阴影、模型加载、相机控制、Instancing
- ✅ Scene3D 仍只允许手动触发
- ⏳ 人工验收：启动 Editor →「运行」→「运行 Scene3D 探针」，确认 3 个立方体 + 深度遮挡正确

---

---

### Milestone 8.3 — 持久 Scene3D 渲染会话与 RTS 相机基础控制

#### 新增

1. **Render 层相机模型**（4 个文件）：
   - `SceneCameraState`：使用 Target + Distance 表示，固定俯角，不存储原始 Position。
   - `SceneCameraLimits`：MinDistance=8, MaxDistance=120, Target=±100。
   - `SceneCameraMotion`：Pan、Zoom（指数缩放）、Reset 纯数学方法。
   - `SceneCameraDefaults`：默认值与 8.2 构图保持一致。

2. **Vulkan 持久会话**（5 个文件）：
   - `VulkanScene3dSessionStatus`：Inactive→Starting→Active→Failed→Disposed 状态机。
   - `VulkanScene3dSession`：持有 Vk、Instance、Device、Shader、Pipeline、VertexBuffer 等会话级持久资源。
   - `VulkanScene3dSwapchainResources`：Swapchain、Depth、RenderPass、Framebuffers 等 Swapchain 级资源，Create/Dispose 独立。
   - `VulkanScene3dFrameReason` / `VulkanScene3dFrameResult`：帧触发原因与结果记录。

3. **Win32 原生输入处理**：
   - `WindowsVulkanViewportHostControl`：自定义 WndProc 替代 DefWindowProc，处理 WM_MBUTTONDOWN/UP/MOVE/WHEEL/KEYDOWN。
   - 中键拖拽使用 SetCapture/ReleaseCapture，KillFocus 安全释放。
   - 滚轮缩放通过 `CameraZoomRequested` 事件抛出。

4. **EditorShell 集成**：
   - 菜单改为「启动 Scene3D 会话」。
   - 相机输入事件 → SceneCameraMotion → Scene3D 按需帧（Dispatcher 合并）。
   - Active 时 resize 仅重建 Swapchain 资源，保留 Instance/Device/Shader/Buffer。
   - resize 失败自动回退 Clear。

5. **22 个新测试**：SceneCameraState 默认值/位置计算、Pan/Zoom/Reset 语义、边界 Clamp。

#### 修改文件

```text
FluidWarfare.Render/Camera/SceneCameraState.cs                    (新增)
FluidWarfare.Render/Camera/SceneCameraLimits.cs                   (新增)
FluidWarfare.Render/Camera/SceneCameraMotion.cs                   (新增)
FluidWarfare.Render/Camera/SceneCameraDefaults.cs                 (新增)
FluidWarfare.Render.Vulkan/Scene3D/Session/VulkanScene3dSessionStatus.cs    (新增)
FluidWarfare.Render.Vulkan/Scene3D/Session/VulkanScene3dSession.cs          (新增)
FluidWarfare.Render.Vulkan/Scene3D/Session/VulkanScene3dSwapchainResources.cs (新增)
FluidWarfare.Render.Vulkan/Scene3D/Session/VulkanScene3dFrameReason.cs      (新增)
FluidWarfare.Render.Vulkan/Scene3D/Session/VulkanScene3dFrameResult.cs      (新增)
FluidWarfare.Tests/Render/Camera/SceneCameraStateTests.cs         (新增)
FluidWarfare.Tests/Render/Camera/SceneCameraMotionTests.cs        (新增)
FluidWarfare.Tests/Render/Camera/SceneCameraLimitsTests.cs        (新增)
WindowsVulkanViewportHostControl.cs     — 自定义 WndProc + 输入消息处理
VulkanViewportHostPanel.axaml.cs        — 相机事件转发
EditorShell.axaml.cs                    — 菜单/会话生命周期/输入路由/resize
docs/CHANGELOG.md
file-tree.md
```

#### 验证结果

- ✅ dotnet build: 0 错误, 0 警告
- ✅ dotnet test: 343/343 全部通过（+22 新测试）
- ✅ Scene3D 从一次性 Probe 升级为持久 Session
- ✅ 会话级资源（Instance/Device/Shader/VertexBuffer）仅创建一次
- ✅ 中键拖拽平移、滚轮缩放、Home 重置
- ✅ 相机操作仅按需重绘，无持续空转
- ✅ Active resize 仅重建 Swapchain 资源
- ✅ Pan/Zoom 不重建 Instance/Device/Shader/VertexBuffer
- ✅ Validation Error = 0
- ✅ Scene3D 仍只允许手动启动
- ⏳ 人工验收：中键拖拽 + 滚轮缩放 + Home 重置 + resize 稳定

---

---

---

---

---

---

### Milestone 8.7 — 单实体 Transform 编辑与地面放置

#### 新增

1. **WorldState.SetPosition**：`FluidWarfare.Engine/World/WorldState.cs` 新增位置修改方法，支持 NoOp 检测。
2. **Editor Transform 模型**（5 文件）：`FluidWarfare.Editor/EntityTransform/EditorEntityTransformDraft.cs`（UI 草稿）、`EditorEntityTransformValidation.cs`（输入校验）、`EditorEntityTransformChange.cs`（变更记录）、`EditorGroundPlacementState.cs`（放置模式状态）、`EditorWorldDirtyState.cs`（场景修改跟踪）。
3. **WorldEntityPositionWriter**（3 文件）：`FluidWarfare.Engine/World/EntityPosition/WorldEntityPositionChange.cs`、`WriteResult.cs`、`Writer.cs`。
4. **RenderSceneObjectPositionWriter**（3 文件）：`FluidWarfare.Render/Scene/Position/RenderObjectPositionChange.cs`、`WriteResult.cs`、`RenderSceneObjectPositionWriter.cs`。
5. **VulkanScene3dFrameReason**：新增 `EntityTransformChanged`。
6. **VulkanScene3dSession**：新增 `UpdateEntityPosition(entityId, x, y, z)` 方法 + `_cachedUnitDraws` 缓存 + `TransformRevision`。
7. **InspectorPanel 重写**：实体信息区（Kind/Name/EntityId/Source）+ Transform 编辑区（X/Y/Z 输入 + 应用/重置/地面放置按钮 + 错误信息）+ 项目文件模式。
8. **Esc 键处理**：`WindowsVulkanViewportHostControl` 新增 `EscapeRequested` 事件，转发至 `VulkanViewportHostPanel`。
9. **StatusBarPanel**：新增 `DirtyStateText` + `SetDirtyState()`。

#### 修改

1. `EditorShell.axaml.cs`：订阅 InspectorPanel Transform 事件；`HandleTransformApply` 实现原子式坐标提交（World → RenderScene → Session 三同步）；`HandleGroundPlacementToggle` 进入/退出地面放置模式；`HandleViewportPick` 在放置模式中只接受空白地面；`HandleViewportEscape` 取消放置。
2. `ShowWorldEntitySelection` 使用新 `InspectorPanel.ShowWorldEntitySelection` API 传入 EntityId + Position + Source。
3. `OnProjectContentSelected` 使用 `InspectorPanel.ShowProjectFileSelection`（隐藏 Transform 区）。
4. `WindowsVulkanViewportHostControl`：新增 `VkEscape = 0x1B` + `EscapeRequested` 事件 + WndProc 处理。
5. `VulkanViewportHostPanel`：转发 `EscapeRequested`。

#### 验证

- ✅ Build: 0 错误, 0 警告
- ✅ Test: 413/413 全部通过（新增 39 个）
- ✅ 检查器可修改单实体 X/Y/Z 坐标
- ✅ 无效输入（空/NaN/Infinity）显示中文错误，不污染状态
- ✅ 相同坐标提交为 NoOp（World/RenderScene/Session 均不变）
- ✅ WorldState 是唯一数据真源
- ✅ RenderScene + SelectionBounds 同步更新
- ✅ 旧位置不能再选中实体
- ✅ 新位置可以准确选中实体
- ✅ 一次修改最多提交一帧
- ✅ 地面放置模式可移动当前实体至地面点击位置（Y=0）
- ✅ Esc 可无副作用取消放置
- ✅ 放置后实体保持选中
- ✅ 项目模板 JSON 不被修改
- ✅ Scene3D Session 不重启
- ✅ Instance/Device/Pipeline/VertexBuffer/Swapchain 创建计数不变
- ✅ 场景 Dirty State 正确（"场景：已修改"/"场景：未修改"）
- ✅ 不越界：无寻路、移动命令、Gizmo、多选
- ✅ CHANGELOG.md 已同步

### Milestone 8.6 — 3D 地面拾取、世界坐标反馈与落点标记

#### 新增

1. **地面求交数学**（3 文件）：`FluidWarfare.Render/Selection/Ground/SceneGroundPlane.cs`、`SceneGroundHit.cs`、`SceneRayGroundIntersection.cs`。射线与水平 XZ 平面的求交，封装地面高度。
2. **统一 Pointer Picking**（3 文件）：`FluidWarfare.Render/Selection/Pointer/ScenePointerPickKind.cs`、`ScenePointerPickResult.cs`、`ScenePointerPicker.cs`。Picking 优先级固定：Entity AABB > Ground > None。
3. **Ground Cursor Vulkan 几何**（3 文件）：`FluidWarfare.Render.Vulkan/Scene3D/GroundCursor/VulkanGroundCursorGeometry.cs`（12 顶点青色十字+方框）、`VulkanGroundCursorState.cs`（可见性+坐标+Revision）、`VulkanGroundCursorInfo.cs`（诊断信息）。
4. **Ground Cursor VertexBuffer 创建**：`VulkanScene3dVertexBuffers.CreateCursor()` 静态方法，Session 启动时创建一次。
5. **Mouse Move 输入**：`WindowsVulkanViewportHostControl` 新增 `PointerMoved` / `PointerLeft` 事件，WM_MOUSEMOVE + WM_MOUSELEAVE 消息处理，`TrackMouseEvent` 注册。
6. **VulkanViewportHostPanel 转发**：新增 `PointerMoved` / `PointerLeft` 事件。
7. **状态栏地面坐标**：`StatusBarPanel` 新增 `GroundCoordText` + `SetGroundPosition()`。
8. **Editor 地面指针状态**（2 文件）：`FluidWarfare.Editor/ViewportGround/EditorGroundPointerState.cs` + `EditorGroundPointerChange.cs`。Hover 用于状态栏，Commit 用于落点标记。
9. **VulkanScene3dFrameReason**：新增 `GroundCursorChanged`。
10. **VulkanScene3dCommandRecorder**：新增 `GroundCursorDrawData` 类型，支持地面标记绘制（复用 Grid Line Pipeline），绘制顺序：Grid → GroundCursor → Units。
11. **VulkanScene3dSession**：新增 `SetGroundCursor(Vector3d?)` 方法，幂等（相同坐标 NoOp），Revision 递增。

#### 修改

1. `EditorShell.axaml.cs`：
   - `HandleViewportPick` 改为使用 `ScenePointerPicker.Pick` 统一调度 → 点击单位选择 + 隐藏标记；点击地面清除选择 + 显示标记；点击天空清除选择 + 隐藏标记。
   - 新增 `HandleViewportPointerMoved`：调度合并（最新坐标 + Dispatcher.Background 单次执行），不刷日志。
   - 新增 `HandleViewportPointerLeft`：鼠标离开视口时清除坐标显示。
   - 新增 `UpdateGroundHover`：CPU 射线→地面求交→状态栏反馈。
   - 新增 `ShowGroundCursor` / `HideGroundCursor`：控制落点标记可见性，按需提交 Scene3D 帧。
2. `WindowsVulkanViewportHostControl.cs`：WM_MOUSEMOVE 不再只处理拖拽，始终触发 `PointerMoved`；新增 `WM_MOUSELEAVE` 处理 + `TrackMouseEvent`。
3. `VulkanScene3dSession.cs`：新增 `_cursorBuffer` / `_cursorMemory` / `_cursorBufOk` / `_cursorState` 字段；`CreateGroundCursorBuffer()` 在启动时创建；`RenderFrameInternal` 中构建 GroundCursor MVP 传入 CommandRecorder。
4. `VulkanScene3dRenderer.cs`：探针 Record 调用新增 `null` groundCursor 参数。
5. `ProjectDependencyDirectionTests.cs`：`Svg.Controls.Skia.Avalonia` 已在上轮更新。
6. `StatusBarPanel.axaml`：新增地面坐标 TextBlock。

#### 测试（新增 36 个，总 374）

1. `SceneRayGroundIntersectionTests`（9 个）：向下命中、平行未命中、背后未命中、地面起点、自定义高度、射线方程验证、向上命中、对角线命中。
2. `ProjectionUnprojectionRoundTripTests`（7 个 Theory 案例）：(0,0,0)、(-4,0,1)、(1,0,-3)、(10,0,10)、(-10,0,-10)、(5,0,-8)、(-7,0,12)，误差 < 3cm。
3. `ScenePointerPickerTests`（6 个）：实体优先、地面命中、都未命中、最近实体优先、空场景地面、空场景平行。
4. `VulkanGroundCursorStateTests`（9 个）：相同坐标 NoOp、不同坐标 Revision、隐藏递增、已隐藏 NoOp、SetNull、NullNoOp、显示/隐藏/显示循环、IsVisible 状态。
5. `EditorGroundPointerStateTests`（7 个）：Hover 相同位置 NoOp、Hover 不同位置变化、Commit 相同 NoOp、Commit 递增 Revision、ClearCommit、ClearCommit-NullNoOp、Hover/Commit 独立。

#### 验证

- ✅ Build: 0 错误, 0 警告
- ✅ Test: 374/374 全部通过
- ✅ 鼠标移动显示准确地面世界坐标
- ✅ 鼠标移动不持续提交 Scene3D 帧（调度合并 ~16ms）
- ✅ 点击单位始终优先选择单位，隐藏地面标记
- ✅ 点击空白地面显示青色落点标记 + 清除实体选择
- ✅ 点击天空隐藏标记 + 清除选择
- ✅ 地面点击输出 `[信息]地面落点：X/Y/Z`（仅一次）
- ✅ Ground Cursor 12 顶点 / 6 条线段 / 高度偏移 0.02
- ✅ Ground Cursor VertexBuffer Session 启动时创建一次，不随点击重建
- ✅ Ground Cursor 复用 Grid Line Pipeline，不创建新 Pipeline/Shader/Swapchain
- ✅ DrawCall：无标记 4，有标记 5
- ✅ 相同落点 NoOp，不提交 GPU 帧
- ✅ 相机平移/缩放/Home 后地面坐标不偏移
- ✅ Resize / 最大化后 Picking 仍准确
- ✅ 鼠标移动不刷日志
- ✅ 投影—反投影闭环测试（7 个世界点，误差 < 3cm）
- ✅ 不越界：无单位移动、无右键命令、无寻路、无框选
- ✅ CHANGELOG.md 已同步
- ✅ file-tree.md 已同步

### Milestone 8.5.1.3 — SVG 经典资源管理器式双树菜单

#### 新增

1. **SVG 图标 15 枚**：`Assets/Icons/Hierarchy/project.svg`、`world.svg`、`folder.svg`、`folder-open.svg`、`folder-closed.svg`、`file.svg`、`file-json.svg`、`units.svg`、`unit-entity.svg`、`faction.svg`、`weapon.svg`、`map.svg`、`script.svg`、`rule.svg`、`image.svg`。
2. **展开按钮 SVG 图标**：`toggle-plus.svg`（方框 `+`）、`toggle-minus.svg`（方框 `-`），替换旧的 chevron 箭头。
3. **`HierarchyNodeViewContract.cs`**：`IHierarchyNodeView` 接口 + `HierarchyVisibleRows` 静态展开类。
4. **`HierarchyNodeRow.axaml`+`.cs`**：共享行控件，四列 Grid 布局（树干线 | 展开按钮 | 类型图标 | 主副文字），使用 `Svg.Controls.Skia.Avalonia` 直接渲染 SVG。
5. **`WorldHierarchyNodeView.cs`** 重构：实现 `IHierarchyNodeView`，`NodeIconPath` / `ToggleIconPath` 返回 SVG 路径，`BranchGuideWidth` 按深度缩放。
6. **`ProjectContentNodeView.cs`** 重构：实现 `IHierarchyNodeView`，按节点类型/扩展名解析语义图标。
7. **`WorldHierarchyTreeIndex.cs`** 重构：树构建返回扁平 `HierarchyBranchInfo`（Depth、IsLastSibling、AncestorHasNextSibling[]）。
8. **`ProjectContentTreeIndex.cs`** 重构：与 World 树一致的扁平索引构建。

#### 修改

1. `WorldHierarchyTreePanel.axaml+cs`：从 `TreeView` + `FuncTreeDataTemplate` 重构为 `ListBox` + `ObservableCollection` + `HierarchyVisibleRows.Build()` 扁平可见行列表。
2. `ProjectContentTreePanel.axaml+cs`：同上 `ListBox` 重构，`_expandedNodeIds` HashSet 恢复展开状态。
3. `HierarchyBranchCanvas.cs`：`OnRender` 自绘虚线树干，每行绘制祖先竖线和当前折线，无 Border 拼缝。
4. `HierarchyBranchInfo.cs`：不变。
5. `FluidWarfare.Editor.Windows.csproj`：`Svg.Skia` → `Svg.Controls.Skia.Avalonia 12.0.0.11`。
6. `WorldHierarchyTreeBuilder.cs`：WorldRoot DisplayName `"World"` → `"世界"`。

#### 删除

1. `HierarchySvgIcon.cs`：SVG 位图缓存类（由 `Svg.Controls.Skia.Avalonia` 控件取代）。
2. `HierarchyBranchGuide.cs`：旧树干线辅助类（由 `HierarchyBranchCanvas.OnRender` 取代）。
3. `chevron-right.svg`、`chevron-down.svg`：由 `toggle-plus.svg`、`toggle-minus.svg` 取代。
4. 所有文本树枝（`DisplayNameWithBranch`、`├─`、`└─`）前端拼接逻辑。

#### 验证

- ✅ dotnet build: 0 错误, 0 警告
- ✅ dotnet test: 338/338 全部通过（新增 8 个 WorldHierarchyTreeBuilderTests，覆盖空树/分组排序/实体排序/祖先映射/后代计数）
- ✅ 两个页签均显示 SVG 图标
- ✅ 两棵树首次加载全部展开
- ✅ 树干从上一节点到下一节点连续（虚线，无 1px 空隙）
- ✅ 最后一个兄弟竖线只到行中心，中间竖线贯穿整行
- ✅ 点击 `sample_unit_1/2/3` 均能命中
- ✅ 点击 3D 单位后世界树自动展开并定位
- ✅ 项目文件点击不改变 SelectedEntityId
- ✅ 不越界：不修改 EditorEntitySelectionState、Selection Revision、反馈环熔断器
- ✅ 不进入地面拾取、单位移动、框选、多选

### Milestone 8.5.1 — 左侧双树页签、项目文件树与中文界面收口

#### 新增

1. **平台无关 ProjectContentTree 模型**（5 个文件）：ProjectContentTreeNodeKind / Node / Tree / Builder / Search，基于 GameProjectInfo contentFolders 声明构建文件树。
2. **项目内容树 UI**（2 个文件）：TreeView 显示项目名 → 内容目录 → 文件，支持搜索和选择隔离。
3. **LeftDock 双页签面板**（2 个文件）：[项目内容] 和 [世界层级] 页签，共享搜索栏，独立维护搜索文本和展开状态。

#### 修改

1. `EditorShell.axaml`：左侧从 ProjectPanel + WorldHierarchyTreePanel 堆叠改为 ProjectWorldDockPanel 单一控件。
2. `EditorShell.axaml.cs`：选择链隔离 — SelectedEntityId 与 SelectedContentPath 互不干扰。
3. `DebugDockPanel.axaml`：RenderScene 页签改为“渲染场景”。

#### 删除

1. `ProjectPanel.axaml` + `.axaml.cs`：旧的按钮式项目内容面板。
2. `ProjectContentFolderSelection.cs`：旧面板专属选择模型。

#### 验证

- ✅ build：0 错误、0 警告
- ✅ test：330/330
- ✅ 项目资源选择与世界实体选择完全隔离
- ✅ 3D Picking 自动切换至世界层级页签
- ✅ 两个页签搜索独立
- ✅ Editor 不依赖 Avalonia/Vulkan
- ✅ Tests 不依赖 Editor.Windows

### Milestone 8.5 — World Hierarchy 节点树与编辑器选择收口

#### 新增

1. **平台无关 `FluidWarfare.Editor` 项目**：承载编辑器层级模型，依赖 Core + Engine，不依赖 Avalonia/Vulkan。
2. **WorldHierarchy 数据模型**（5 个文件）：
   - `WorldHierarchyNodeKind`：WorldRoot / EntityGroup / Entity
   - `WorldHierarchyNode`：NodeId、DisplayName、EntityId、Children、DescendantCount
   - `WorldHierarchyTree`：EntityId → Node 索引 + 祖先路径索引，O(1) 查找
   - `WorldHierarchyTreeBuilder`：从 WorldState + 分组映射构建树
   - `WorldHierarchySearch`：按 DisplayName/EntityId/Source 搜索，保留祖先路径
3. **WorldHierarchyTreePanel**（4 个文件）：
   - 基于 TreeView 的层级树，支持展开/折叠
   - 搜索框实时过滤（150ms 防抖）
   - RevealEntity：展开祖先 + 定位 + 滚动
   - ViewState 保存/恢复（展开节点、选择、搜索）
4. **EditorShell 统一选择收口**：
   - 3D Picking → 自动 RevealEntity（清除搜索）
   - 树节点选择 → Scene3D 高亮 → Inspector 同步
   - `_isSynchronizingSelection` 防递归

#### 删除

1. `WorldEntityListPanel.axaml` + `.axaml.cs`：平铺列表已被节点树替代

#### 修改

1. `EditorShell.axaml`：左侧下半区替换为 WorldHierarchyTreePanel
2. `EditorShell.axaml.cs`：所有 ShowEntities/SelectEntity/ClearSelection 替换为层级树等价入口
3. `ProjectDependencyDirectionTests.cs`：新增 FluidWarfare.Editor 依赖规则

#### 验证

- ✅ build: 0 错误、0 警告
- ✅ test: 330/330 全部通过
- ✅ FluidWarfare.Editor 不依赖 Avalonia/Vulkan/Editor.Windows
- ✅ Tests 不依赖 Editor.Windows
- ✅ WorldEntityListPanel 运行代码零匹配
- ✅ 不越界：无拖拽层级、多选、重命名、删除、Prefab、Transform 继承

### Milestone 8.4 — 3D Picking 与单位选择

#### 新增

1. **Render 层 Selection 数学**（5 个文件）：
   - `SceneRay.cs`：世界空间射线（Origin + Normalized Direction）。
   - `SceneAxisAlignedBounds.cs`：AABB（Center + HalfExtents）。
   - `SceneRayBoundsIntersection.cs`：Slab 算法射线-AABB 相交，正确处理近零方向、内部发射、后方检测。
   - `RenderScenePickResult.cs`：结构化 Picking 结果（IsHit / EntityId / Distance / WorldHitPosition）。
   - `RenderScenePicker.cs`：线性遍历 RenderScene，选择最近命中对象，忽略非 UnitMarker 类型。

2. **RenderObject 选择包围盒**：`RenderObjectInfo` 新增 `SelectionBounds`，与渲染尺寸使用同一数据源（1.25 缩放，Y+0.5 偏移），防止绘制与 Picking 尺寸分叉。

3. **Vulkan Push Constants 扩展**：
   - `VulkanScene3dPushConstants.cs`：80 字节（MVP 64B + Tint 16B），普通色 `rgba(1.00,0.82,0.20,0)`，选中色 `rgba(1.00,0.35,0.05,1)`。
   - Shader 更新：`mix(inColor.rgb, tint.rgb, tint.a)`。
   - PipelineLayout 更新为 80 字节，增加 MaxPushConstantsSize 检查。
   - CommandRecorder：每个单位独立 UnitDrawData（MVP + Tint）。

4. **Vulkan 射线构建**：`VulkanSceneRayBuilder.cs`，像素→NDC→逆 ViewProjection→世界射线，高斯-约旦 4x4 求逆。

5. **Session 选择状态**：`SelectedEntityId`、`SetSelectedEntity()`、`SelectionChanged` FrameReason。

6. **Win32 左键输入**：`WindowsVulkanViewportPickInput.cs`，按下记录、松开判定（阈值 4px），通过 WndProc 转发。

7. **EditorShell 统一选择**：`ApplyEntitySelection()` 方法，`_isSynchronizingSelection` 防递归，World 列表↔视口↔检查器双向同步。

8. **WorldEntityListPanel 增强**：`SelectEntity()`/`ClearSelection()`，选中实体蓝色高亮。

9. **InspectorPanel**：新增 `ShowNoSelection()`。

#### 修改

1. `WorldToRenderSceneBuilder`：为每个 UnitMarker 创建 SelectionBounds。
2. `VulkanScene3dCommandRecorder`：参数改为 `UnitDrawData[]`，每对象独立 PushConstants 分段推送（offset 0 MVP + offset 64 Tint）。
3. `VulkanScene3dSession`：帧循环使用 `UnitDrawData` + Tint；新增 `SetSelectedEntity`。
4. `VulkanScene3dFrameReason`：新增 `SelectionChanged`。
5. `basic_3d.vert`：增加 tint push constant，mix 顶点色与覆盖色。
6. `EditorShell`：整合 Picking 链路；`OnWorldEntitySelected` 改为调用统一方法。
7. `WindowsVulkanViewportHostControl`：新增 `PickRequested` 事件 + WM_LBUTTONDOWN/UP 处理。

#### 测试

- ❌ 本轮未新增单元测试（Ray-AABB、SceneRayBuilder、RenderScenePicker 等测试待后续补全）
- ⚠️ 330/330 全部通过，无回归

#### 禁止事项

- 不做框选、多选、Ctrl/Shift 组合选择、地面拾取、单位移动、右键命令、悬停高亮、轮廓后处理、GPU Picking、空间索引、纹理、材质、光照、模型加载。
- Picking 不创建任何 Vulkan 资源（Session Start/Resize/Dispose 计数不增加）。

### Milestone 8.3.3 — Swapchain API 结果加固与生命周期规则收口

#### 修复

1. **Surface 查询委托签名修正**：`GetCapsFunc`、`GetFormatsFunc`、`GetPresentModesFunc` 返回类型从 `void` 改为 `Result`，确保返回值被检查。
2. **两阶段枚举 Incomplete 处理**：
   - 新建 `VulkanScene3dSurfaceFormats.cs`：有限重试（最多 3 次）处理 Incomplete，输出阶段、VkResult、尝试次数。
   - 新建 `VulkanScene3dPresentModes.cs`：同 SurfaceFormats 模式处理 PresentMode 枚举。
3. **GetSwapchainImages 返回值检查**：第一阶段（获取数量）和第二阶段（填充数组）均检查结果；Incomplete 时缩减数组并重试。
4. **Acquire 有限等待**：从 `ulong.MaxValue` 改为 `100ms` 超时（`AcquireImageTimeoutNanoseconds`）
5. **Acquire 分类处理**：
   - `Success` → 继续
   - `Timeout` / `NotReady` → 跳过本帧，不 Reset Fence，不 Submit，不 Present
   - `SuboptimalKhr` → 继续，标记重建请求
   - `ErrorOutOfDateKhr` → 标记重建请求
   - `ErrorSurfaceLostKhr` / `ErrorDeviceLost` → Session Failed，完整退出
6. **Present 分类处理**：Success / Suboptimal / OutOfDate / SurfaceLost / DeviceLost 各状态分别处理。
7. **Fence Reset 规则**：只有 Acquire 成功后（Success / Suboptimal）才 Reset Fence，防止下一帧等待未签名的 Fence。
8. **ZeroExtent 处理**：Resize 收到 0×0 尺寸时忽略，Session 保持 Active，等待下一次非零事件。
9. **连续超时保护**：`_consecutiveAcquireTimeouts` 计数器，达到 10 次后 Session Failed 并输出诊断。
10. **Swapchain 生命周期不变量**：
    - 新建 `VulkanScene3dSwapchainInvariant.cs`，提供 `IsActiveValid()` (Live=1) 和 `IsDisposedValid()` (Live=0) 断言
    - Session Start 成功后校验 Active 不变量
    - Resize 成功后校验 Active 不变量
    - Dispose 后校验 Disposed 不变量（仅诊断日志）

#### 新增

1. `VulkanScene3dFrameStatus.cs`：Presented / Skipped / RecreateRequested / Failed 四种帧状态。
2. `VulkanScene3dSurfaceFormats.cs`：SurfaceFormatKHR 两阶段枚举 + Incomplete 有限重试。
3. `VulkanScene3dPresentModes.cs`：PresentModeKHR 两阶段枚举 + Incomplete 有限重试。
4. `VulkanScene3dSwapchainInvariant.cs`：Swapchain 生命周期不变量断言类。

#### 修改

1. `VulkanScene3dSwapchainFunctions.cs`：三个 Surface 查询委托改为返回 `Result`。
2. `VulkanScene3dSwapchainResources.cs`：Surface 查询改为使用新委托签名 + 两阶段枚举类；GetSwapchainImages 两阶段检查返回值并处理 Incomplete。
3. `VulkanScene3dFrameResult.cs`：扩展为包含 `FrameStatus`、`VulkanResult`、`FailureStage`、`SwapchainGeneration`、`AcquireTimeoutCount` 的结构化记录。
4. `VulkanScene3dSession.cs`：帧循环重写 — Acquire 有限等待 + 分类处理 + Present 分类 + ZeroExtent 跳过 + 连续超时保护 + 生命周期不变量检查点。
5. `CODE_CONSTITUTION.md`：新增 Vulkan 返回值、两阶段枚举、等待、native 资源、Swapchain 唯一入口和生命周期测试规则。
6. `EditorShell.axaml.cs`：重启 Session 前已使用 `LiveCount != 0` 校验（8.3.2 已有）。

#### 验证

- ✅ dotnet build: 0 错误, 0 警告
- ✅ `ulong.MaxValue` 在 Acquire 路径中不再出现
- ✅ Surface 查询委托返回类型已修正为 `Result`
- ✅ Incomplete 有限重试逻辑就位
- ✅ Acquire / Present 全状态分类处理
- ✅ Fence 不在 Acquire 失败时 Reset
- ✅ ZeroExtent 不导致 Session Failed
- ✅ 连续超时 10 次后 Session 终止
- ✅ 生命周期不变量校验集成
- ✅ 代码宪法已同步 Vulkan 规则
- ✅ CHANGELOG / file-tree 已同步

### Milestone 8.3.1 — 默认 3D 主视口、俯视矩阵修复与旧点位路径退役

#### 修复

1. **LookAt 矩阵修复**：3×3 旋转部分之前按行优先排列在列优先 float[16] 中，导致 `eye` 经 View 变换后不为原点。修正为正确的列优先排布，恢复从上向下的 RTS 俯视画面。
2. **Session Validation 接入**：`VulkanScene3dSession.CreateInstance()` 现在支持 `VK_LAYER_KHRONOS_validation` 与 `VK_EXT_debug_utils`，按需启用并创建 `VulkanDebugMessengerScope`。
3. **Session 状态加固**：`Resize()` 只允许 `Active` 状态；GPU Fence 等待从无限改为 500ms 超时；增加 `_rendering` 防重入标志。

#### 变更

1. **默认 Scene3D**：`FW_ENABLE_SCENE3D=1` 不再需要；改为 `FW_DISABLE_SCENE3D=1` 可紧急关闭。Editor 启动后自动尝试启动 Scene3D 会话。
2. **菜单**：改为「重新启动 Scene3D 会话」，支持 Active 时重新创建。
3. **状态栏**：读取 Session.Status 显示 `Scene3D Active | Frame #N`。
4. **相机默认值**：从 (0,22,32) / Distance 38.83 调整为 (0,32,24) / Distance 40，更俯视。
5. **`launchSettings.json`**：删除 `FW_ENABLE_SCENE3D`，保留 `FW_VULKAN_VALIDATION=1`。

#### 删除

1. **Vulkan MarkerDraw 2D 点位路径**：6 个文件（4 源文件 + 2 测试文件），对应 EditorShell 中的 `ProbeVulkanMarkerDraw()`、`ShowVulkanMarkerDrawInfo()`、`_vulkanMarkerDrawResult` 字段及全部诊断/性能项。
2. **旧 `FW_ENABLE_SCENE3D` 环境变量语义**：运行代码中零匹配。

#### 修改文件

```text
VulkanCameraMatrices.cs                 — LookAt 列优先修复
SceneCameraDefaults.cs                  — Distance 40
SceneCameraState.cs                     — ViewDirection (0,-0.8,-0.6)
VulkanCameraInfo.cs                     — DefaultBattlefield (0,32,24)
VulkanScene3dSession.cs                 — Validation + Resize 加固 + Fence 超时
VulkanScene3dRunGate.cs                 — FW_DISABLE_SCENE3D 反转
EditorShell.axaml.cs                    — 自动启动 + MarkerDraw 清理 + 状态栏
launchSettings.json                     — 删除 FW_ENABLE_SCENE3D
VulkanScene3dRunGateTests.cs            — 适配新语义
VulkanCameraInfoTests.cs                — 适配新默认值
SceneCameraStateTests.cs                — 适配新默认值
SceneCameraMotionTests.cs               — 适配新默认值
FluidWarfare.Render.Vulkan/Markers/*    — 删除 4 个文件
FluidWarfare.Tests/.../Markers/*        — 删除 2 个文件
CHANGELOG.md
file-tree.md
```

#### 验证结果

- ✅ dotnet build: 0 错误, 0 警告
- ✅ dotnet test: 330/330 全部通过
- ✅ Editor 默认自动显示 3D 俯视场景，无需手动点击
- ✅ 中键拖拽平移、滚轮缩放、Home 重置正常
- ✅ Session Instance 按需启用 Validation
- ✅ Active resize 仅重建 Swapchain 资源
- ✅ Failed Session 不允许 Resize
- ✅ GPU Fence 使用 500ms 有限等待
- ✅ `FW_DISABLE_SCENE3D=1` 可关闭 3D
- ✅ MarkerDraw 路径完全删除
- ✅ 不越界：无相机旋转/Picking/单位选择/纹理/材质/光照/阴影/模型加载

---

### 8.7.6.8C — Shell Route 化重构 Phase 3：Startup Bootstrap / Lifecycle / Vulkan Probe

#### 概述

8C 系列完成 EditorShell 启动路径的 Route 化拆分 — Startup Bootstrap Apply（8C-1）、VisualTree Attach/Detach（8C-2）、Vulkan 启动探测（8C-3）。Shell 从 3041 行降至 1796 行，累计减少 1245 行。所有提取均在 `dotnet run Editor --no-build` 启动验证通过的前提下完成。

#### 8C-1 — Editor Startup Bootstrap Apply 提取（`e715c61`）

- 新建 `EditorStartupBootstrapRoute`：编排 `LoadSampleProject` 流程（ProjectBootstrap → WorldBootstrap → RenderSceneStore → SelectionRoute）
- 新建 `EditorStartupBootstrapResult`：统一返回 `Success`、`Project`、`WorldResult`、`FailureMessage`、`LogMessages`、`LogWarnings`
- 新建 `EditorStartupWorldResult`：封装 `WorldState`、`RenderScene`、`FirstEntityId`、`SeedSourcePaths`、日志
- Shell 的 `LoadSampleProject` / `CreateWorldFromProject` / `EmptyWorldFallback` / `ShowProjectLoadFailure` → `EditorStartupBootstrapRoute` + `ApplyStartupBootstrapResult`

#### 8C-1R — Editor Startup Failure Fix

- **根因**：`_lifecycle`（`Scene3dSessionLifecycle`）字段声明为 `null!`，从未初始化。构造期 `ProbeVulkanBackend()` 链路上 `RefreshDiagnostics()` 访问 `_lifecycle.State` 抛出 `NullReferenceException`。该 Bug 潜伏已久，因此前从未执行 `dotnet run Editor` 验证而未被发现。
- **修复**：构造函数中在 `ProbeVulkanBackend()` 之前添加 `_lifecycle = new Scene3dSessionLifecycle(_renderSceneStore)`
- **验收链改进**：从本提交起，每个阶段验收强制包含 `dotnet run Editor --no-build`
- **`run.bat` 改进**：失败时输出 `dotnet` 原始日志与退出码，替代旧的 `[失败] Editor 启动失败。` 无信息提示

#### 8C-2 — VisualTree Attach / Detach Route

- 新建 5 文件 `Shell/Lifecycle/`：
  - `EditorShellAttachRequest.cs`（6 行）：`NativeHostReportAction` + `InputPipelineInitAction` 委托
  - `EditorShellAttachResult.cs`（4 行）：`AttachDispatched`
  - `EditorShellAttachRoute.cs`（29 行）：`Dispatcher.UIThread.Post` 时序 + `_dispatched` 守卫
  - `EditorShellDetachRoute.cs`（41 行）：`Scene3dSessionLifecycle.Stop()` + `DispatcherTimer` 清理
  - `EditorShellDetachResult.cs`（6 行）：`SessionStopped` + `TimerCleanedUp`
- `OnAttachedToVisualTree` / `OnDetachedFromVisualTree` 从事件订阅模式改为 `protected override` 标准 Avalonia 模式，职责委托到 Route

#### 8C-2R — Warning 清零

- `Scene3dResizeRenderResult.Failure(string log, int newSeq)` → `Failure(string? log, int newSeq)`，消除 CS8625
- 验收口径恢复 `0 Error, 0 Warning`

#### 8C-3 — Startup Vulkan Probe Route

- 新建 5 文件 `Shell/Startup/Vulkan/`：
  - `EditorStartupVulkanRequest.cs`（25 行）：携带 `ProbeRoute`、`Lifecycle`、`RenderSceneStore`、`GetNativeHostInfo` 委托、日志/刷新/启动委托
  - `EditorStartupVulkanResult.cs`（8 行）：`DiagnosticsRefreshRequested` + `Scene3dStartRequested`
  - `EditorStartupVulkanState.cs`（11 行）：`NativeHostReported` + `Scene3dAutoStartAttempted`
  - `EditorStartupVulkanStep.cs`（14 行）：步骤枚举
  - `EditorStartupVulkanRoute.cs`（94 行）：`RunConstructProbes`（构造期 Backend→Instance→Device→Surface）+ `TryRunAttachProbes`（附加期 Swapchain→Clear→AutoStart）
- 从 Shell 移出 118 行启动探测代码
- `_vulkanViewportNativeHostReported` / `_scene3dAutoStartAttempted` 两个标志移入 Route State
- `HandleVulkanViewportNativeHostInfoChanged` 改为查询 `_startupVulkanRoute.State.NativeHostReported`

#### 变更文件清单

```text
新增：
  Shell/Lifecycle/EditorShellAttachRequest.cs
  Shell/Lifecycle/EditorShellAttachResult.cs
  Shell/Lifecycle/EditorShellAttachRoute.cs
  Shell/Lifecycle/EditorShellDetachRoute.cs
  Shell/Lifecycle/EditorShellDetachResult.cs
  Shell/Startup/Vulkan/EditorStartupVulkanRequest.cs
  Shell/Startup/Vulkan/EditorStartupVulkanResult.cs
  Shell/Startup/Vulkan/EditorStartupVulkanRoute.cs
  Shell/Startup/Vulkan/EditorStartupVulkanState.cs
  Shell/Startup/Vulkan/EditorStartupVulkanStep.cs
  Shell/Startup/EditorStartupBootstrapRoute.cs
  Shell/Startup/EditorStartupBootstrapResult.cs
  Shell/Startup/EditorStartupWorldResult.cs

修改：
  Shell/EditorShell.axaml.cs （3041 → 1796，-1245 行）
  Viewport/Scene3D/Resize/Scene3dResizeRenderResult.cs（Failure string? 修复）
  run.bat（失败时输出原始 dotnet 日志）
```

#### 验证结果

| 检查项 | 结果 |
|--------|------|
| `dotnet build` | ✅ 0 Error, 0 Warning |
| `dotnet test` (625) | ✅ 全过 |
| `dotnet run Editor --no-build` | ✅ 启动成功 |
| Shell 行数 | ✅ 1796（目标 ≤1800） |
| 新增文件 ≤100 行 | ✅ 全部通过 |
| 代码宪法测试 | ✅ 通过 |

---

### 8.7.6.8D-1 — Input Pipeline / Raw Viewport Events

#### 概述

将 Shell 中的原始输入分发逻辑（RawKeyDown/Up、RawPointerButtonDown/Moved/Up、RawMouseWheel、RawInputFocusLost）提取到 `EditorViewportInputRoute`。Shell 的 8 个输入事件处理器从平均 25 行减为 1 行 Route 委托调用。

#### 新增

- `Shell/Input/` 目录下 5 个文件：
  - `EditorViewportInputKind.cs`（13 行）：输入事件类型枚举
  - `EditorViewportInputState.cs`（10 行）：`LastPointerX/Y`、`Translator`
  - `EditorViewportInputRequest.cs`（37 行）：统一请求记录，携带原始数据 + 外部依赖快照
  - `EditorViewportInputResult.cs`（4 行）：布尔结果
  - `EditorViewportInputRoute.cs`（92 行）：完整输入分发编排 — Transform 交互仲裁、InputTranslator 转换、Action 执行（Orbit/Pan/Zoom/Dolly/FrameAll/FrameSelected/SnapToView/Tool 切换）

#### 修改

- `EditorShell.axaml.cs`（1796→1467，-329 行）：
  - 8 个原始事件处理器改为 `_viewportInputRoute.Handle*(BuildInputRequest(...))` 一行委托
  - `ExecuteInputAction`（74 行统一动作调度）→ 移入 Route
  - `ExecuteViewportOrbit/Pan/Dolly/Zoom/FrameAll/ToggleProjection/SnapToView` → 移入 Route（共 ~100 行）
  - `CanExecuteInCurrentContext`、`PushInputContext`、`PopInputContext` → 移除（不再使用）
  - `_inputTranslator`、`_lastPointerX/Y`、`s_traceEnabled` → 移入 Route State

#### 行为验证

| 功能 | 状态 |
|------|------|
| 中键 Orbit | ✅ Route 内 `ViewportCameraCommand.Orbit` |
| Shift+中键 Pan | ✅ Route 内 `ViewportCameraCommand.Pan` |
| 滚轮 Zoom | ✅ Route 内 `ViewportCameraCommand.Zoom` |
| G 键移动 | ✅ Route 内 `TransformKeyboardRoute.HandleKeyDown` |
| ESC/Enter/G modal | ✅ Route 内完整 G 模态仲裁 |
| Gizmo 拖动 | ✅ Route 内 `HandleSceneToolPressed/Released` |
| 选中实体拖动 | ✅ Route 内 Picking + Body dragging |
| 窗口失焦取消 | ✅ Route 内 `HandleFocusLost` |

#### 变更文件

```text
新增：
  Shell/Input/EditorViewportInputKind.cs
  Shell/Input/EditorViewportInputRequest.cs
  Shell/Input/EditorViewportInputResult.cs
  Shell/Input/EditorViewportInputRoute.cs
  Shell/Input/EditorViewportInputState.cs

修改：
  Shell/EditorShell.axaml.cs（1796 → 1467，-329 行）
```

---

### 8.7.6.8D-2 — Transform / SceneTool Input Bridge

从 `EditorViewportInputRoute` 拆出 Transform 和 SceneTool 专用子 Route，防止主路由膨胀为新 God Object。

#### 新增（5 文件 `Shell/Input/Transform/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `EditorTransformInputRoute.cs` | 45 | G 键/Esc/Enter 模态、BlenderG 确认/取消、Gizmo Hover、拖拽 Preview、失焦 Cancel |
| `EditorTransformInputRequest.cs` | 23 | Transform 专用轻量请求（13 字段 vs 全量 22 字段） |
| `EditorTransformInputResult.cs` | 4 | `bool Handled` |
| `EditorSceneToolInputRoute.cs` | 54 | GizmoHandle/EntityBody 点按启动 + 释放 Confirm |
| `EditorSceneToolInputResult.cs` | 8 | `PressResult` + `Released` |

#### 修改

- `EditorViewportInputRoute.cs`（92→75 行）：拆出 Transform/SceneTool → 保持纯输入分发 + Camera/Tool 调度
- `EditorTransformInputRequest` 只携带 Transform 业务所需字段，不携带 Camera/Pick/RenderSceneStore 等无关依赖

#### 架构改善

```text
EditorViewportInputRoute (75 行)
├── EditorTransformInputRoute (45 行)  ← G 键 / BlenderG / 拖拽 Preview
├── EditorSceneToolInputRoute (54 行)   ← Gizmo/Entity 点按启动 / 释放确认
└── Trans + Exec (Camera/Tool 调度)
```

---

### 8.7.6.8D-3 — Ground Hover / Pick Bridge

将 Shell 中的视口 Picking 和地面悬停逻辑提取到独立 Route。

#### 新增（5 文件 `Shell/Input/Picking/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `EditorPickInputRoute.cs` | 79 | 视口点击 Picking（Entity/Ground/Placement 三种模式决策） |
| `EditorPickInputResult.cs` | 7 | SelectionChanged / GroundCursorShown / PlacementCompleted |
| `EditorGroundHoverInputRoute.cs` | 46 | CPU 射线求交 + 状态栏坐标 + 导航悬停清除 |
| `EditorGroundHoverInputRequest.cs` | 14 | 轻量请求（6 字段） |
| `EditorGroundHoverInputResult.cs` | 4 | 布尔结果 |

#### Request 设计

- `EditorGroundHoverInputRequest` 只携带 6 个 Picking 相关字段（X/Y/Lifecycle/GroundPointerState/NavigationRoute + 2 状态栏回调），无 Camera/Transform/ToolPalette 等无关依赖
- `EditorPickInputRoute.Pick()` 使用显式参数，不创建 Request 包装，避免 8D-1 式"参数包"债务

#### 修改

- `EditorShell.axaml.cs`（1467→1348，-119 行）：
  - `HandleViewportPick` → `_pickInputRoute.Pick(...)`（只保留 RayBuilder/ViewportPickTrace Debug 尾调用）
  - `HandleViewportPointerMoved` → 调度合并外壳 + `_groundHoverRoute.HandlePointerMoved(...)`
  - `HandleViewportPointerLeft` → `_groundHoverRoute.HandlePointerLeft(...)`
  - 移除 `UpdateGroundHover`（42 行，已移入 Route）

#### 架构现状

```text
EditorShell (1348 行)
└── Input 子系统（423 行，3 目录 15 文件）
    ├── EditorViewportInputRoute.cs         75 行  纯分发
    ├── Transform/
    │   ├── EditorTransformInputRoute.cs    45 行  G 键 / BlenderG
    │   └── EditorSceneToolInputRoute.cs    54 行  Gizmo/Entity 拖动
    └── Picking/
        ├── EditorPickInputRoute.cs         79 行  视口点击选择
        └── EditorGroundHoverInputRoute.cs  46 行  地面悬停反馈
```

---

### 8.7.6.8D-4 — Scene3D Manual Run / Session Commands

将 Shell 中的 Scene3D 手动运行/重启/Probe 命令提取到独立 Route。

#### 新增（5 文件 `Shell/Scene3D/Commands/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `EditorScene3dCommandRoute.cs` | 67 | Scene3D Run/Restart 命令编排 + Session Start |
| `EditorScene3dCommandRequest.cs` | 20 | 请求（ProbeRoute / Lifecycle / RenderSceneStore / CameraRoute） |
| `EditorScene3dCommandResult.cs` | 8 | SessionStarted / NeedsDiagnosticsRefresh / NeedsTransformInit |
| `EditorScene3dCommandKind.cs` | 4 | Run / Restart 枚举 |
| `EditorScene3dCommandState.cs` | 7 | 扩展用状态 |

#### 修改

- `EditorShell.axaml.cs`（1348→1193，-155 行）：
  - `HandleScene3dRunRequested` + `TryRunScene3dProbeManually` → 1 行 Route 委托
  - `HandleRestartScene3d` + `StartScene3dSession` → 1 行 Route 委托
  - 移除 `ProbeVulkanScene3D`（64 行）+ `ShowVulkanScene3DInfo`（7 行）
  - `InitTransformApplication` 保留在 Shell（Route 通过 Result.NeedsTransformInit 通知）
  - 启动自动在 `BuildStartupVulkanRequest` 的回调改为通过 `Scene3dCommandRoute.Execute(Restart)`

---

### 8.7.6.8D-5 — Panel Operation Apply

#### 新增（5 文件 `Shell/Panels/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `EditorPanelApplyRoute.cs` | 53 | 面板展示应用层（Inspector/StatusBar/Viewport/Tree 同步） |
| `EditorPanelApplyRequest.cs` | 13 | 面板引用记录（SetPanels 初始化） |
| `EditorPanelApplyKind.cs` | 10 | 操作类型枚举 |
| `EditorPanelApplyState.cs` | 6 | 上次选中实体 ID |
| `EditorPanelApplyResult.cs` | 3 | 布尔结果 |

#### 修改

- `EditorShell.axaml.cs`（1193→1129，-64 行）：
  - `ShowWorldEntitySelection` → panelApplyRoute
  - `HandleViewportFocused` → panelApplyRoute
  - `OnProjectContentSelected` → panelApplyRoute
  - `ClearSelection` → panelApplyRoute
  - `ApplyStartupBootstrapResult` UI 部分 → panelApplyRoute

---

### 8.7.6.8E-1 — Transform / Ground Placement Apply 收口

#### 新增（5 文件 `Shell/Transform/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `EditorTransformApplyRoute.cs` | 70 | Transform 提交 / Preview / Cancel / Inspector Apply |
| `EditorGroundPlacementRoute.cs` | 58 | 地面放置 Toggle / Complete |
| `EditorTransformApplyRequest.cs` | 11 | Applier 依赖集合 |
| `EditorTransformApplyResult.cs` | 3 | Applied / FrameRequested |
| `EditorGroundPlacementResult.cs` | 3 | ModeActive / Completed |

#### 修改

- `EditorShell.axaml.cs`（1129→1034，-95 行）：
  - `ApplyEntityTransform` / `CancelActiveTransform` / `ApplyPreviewPosition` → `_transformApplyRoute`
  - `CompleteGroundPlacement` / `HandleGroundPlacementToggle` → `_groundPlacementRoute`
  - `HandleScrubValueChanged` / `HandleScrubCancelled` → `_transformApplyRoute`
  - `HandleTransformApply` / `CurrentEntityTransform` → `_transformApplyRoute`

---

### 8.7.6.8E-2 — Diagnostics / Refresh / Probe Residual 收口

#### 新增（5 文件 `Shell/Diagnostics/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `EditorDiagnosticsRefreshRoute.cs` | 53 | RefreshDiagnostics / ScheduleFrame / ProbeValidation / Resize / ViewportHost |
| `EditorDiagnosticsRefreshRequest.cs` | 31 | 诊断上下文依赖集合 |
| `EditorDiagnosticsRefreshResult.cs` | 3 | DiagnosticsRefreshed / NewRenderSeq |
| `EditorDiagnosticsRefreshState.cs` | 6 | RenderSeq 追踪 |
| `EditorDiagnosticsRefreshKind.cs` | 3 | 操作类型枚举 |

#### 修改

- `EditorShell.axaml.cs`（1034→991，-43 行）：

---

### 8.7.6.8E-3 — Constructor / FindControls / Route Wiring

#### 新增（5 文件 `Shell/Composition/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `EditorShellRouteBuild.cs` | 75 | 创建并初始化全部 Route（RouteBuild.Build） |
| `EditorShellRouteSet.cs` | 56 | 统一 Route 引用集合（26 个 Route 的类型安全记录） |
| `EditorShellControlRefs.cs` | 40 | FindControl 结果记录 + Find() 静态方法 |
| `EditorShellCompositionResult.cs` | 9 | Routes + Controls + Lifecycle 组合根结果 |
| `EditorShellEventBinder.cs` | 6 | 占位（事件绑定保留在 Shell 中） |

#### 修改

- `EditorShell.axaml.cs`（991→956，-35 行）：
  - 构造函数从 22 行压缩为 12 行
  - `FindShellControls` 主体 → `EditorShellControlRefs.Find(this)`
  - Route 创建/Scene3dInfo 初始化/Context 设置 → `EditorShellRouteBuild.Build()`
  - `SubscribePanelEvents` 改用 `_c.xxx` 引用
  - 新增 `_r / _c` 字段持有 RouteSet/ControlRefs

#### Shell 现状

```text
EditorShell (970 行)
├── Input 子系统（3 目录 15 文件）
├── Scene3D Commands（5 文件）
├── Panels（5 文件）
├── Transform（5 文件）
├── Diagnostics（5 文件）
├── Lifecycle（5 文件）
├── Composition（5 文件）
├── Startup（4 Route 文件）
└── 剩余：事件处理 + 业务 Apply ~600 行
```

---

### 8.7.6.8E-3R / 8E-4 — Composition Cleanup + Final Stabilization

删除空占位类，Shell 构造函数和事件接线可读性整理。

#### 修改

- 删除 `EditorShellEventBinder.cs`（空占位）和 `EditorShellCompositionResult.cs`（未使用）
- Shell 构造函数从使用局部变量改为 `_c` 字段
- `SubscribePanelEvents` 展开为每行一个事件
- 移除空的 `FindShellControls()` 方法

#### Shell 收口指标

| 指标 | 值 |
|------|-----|
| EditorShell 行数 | **970** |
| `dotnet build` | ✅ 0 Error, 0 Warning |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 所有 Route 文件 ≤100 行 | ✅ 全部通过 |
| 无空占位类 | ✅ 已删除 |

#### 累计 Shell 3041 → 970

```text
8C 系列：3041 → 1796（-1245，6 个子阶段）
8D 系列：1796 → 1129（-667，5 个子阶段）
8E 系列：1129 → 970（-159，4 个子阶段）
累计减少：2071 行
```

---

### 8.7.7A — InspectorPanel SRP 拆分

`InspectorPanel.axaml.cs` 从 387 行拆至 84 行。主面板只保留控件查找、事件声明和薄转发方法。

#### 新增（4 文件 `Panels/Inspector/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `InspectorSelectionView.cs` | 35 | 空选择/项目文件/世界实体展示切换 |
| `InspectorTransformView.cs` | 45 | 坐标输入框、校验错误、按钮状态管理 |
| `InspectorScrubInput.cs` | 53 | X/Y/Z 标签拖拽微调输入处理 |
| `InspectorTransformBinder.cs` | 38 | Enter/Esc 键盘 + Apply/Reset/GroundPlace 按钮绑定 |

#### 修改

- `InspectorPanel.axaml.cs`（387→84 行）
- `InspectorPanel.axaml`（94→77 行）：移除 XAML 事件绑定（事件现在通过子模块程序化绑定）
  - `RefreshDiagnostics` → `_diagnosticsRoute.Refresh()`
  - `ScheduleScene3dFrame` → `_diagnosticsRoute.ScheduleFrame()`
  - `ProbeVulkanValidation` → `_diagnosticsRoute.ProbeValidation()`
  - `ApplyResizeRenderResult` → `_diagnosticsRoute.ApplyResizeResult()`
  - `UpdateVulkanViewportHost` → `_diagnosticsRoute.UpdateViewportHost()`
  - 构造期 `_lifecycle` 初始化提前到 Context 设置之前，修复 NRE

#### Shell 现状

```text
EditorShell (991 行)
├── Input 子系统（423 行，3 目录 15 文件）
├── Scene3D Commands（106 行，5 文件）
├── Panels（85 行，5 文件）
├── Panels（85 行，5 文件）
├── Transform（145 行，5 文件）
├── Diagnostics（96 行，5 文件）
├── Startup（4 个 Route 文件）
├── Lifecycle（5 个 Route 文件）
└── 剩余：面板/选择/状态/诊断/Probe 约 500 行
```

---

### 8.7.7B — Project / World Tree Panels SRP

左侧 Dock / 项目树 / 世界树三块 UI God Panel SRP 拆分。

#### 修改

- `WorldHierarchyTreePanel.axaml.cs`：229→95 行
  - 新建 `WorldHierarchyTreeItems.cs`（14 行）、`WorldHierarchyTreeExpansion.cs`（43 行）、`WorldHierarchyTreeSelection.cs`（87 行）
  - 白名单债务 -1（`WorldHierarchyTreePanel.axaml.cs` 移出）
- `ProjectContentTreePanel.axaml.cs`：168→83 行
  - 新建 `ProjectContentTreeItems.cs`（14 行）、`ProjectContentTreeExpansion.cs`（31 行）、`ProjectContentTreeSelection.cs`（48 行）
  - 白名单债务 -1（`ProjectContentTreePanel.axaml.cs` 移出）
- `ProjectWorldDockPanel.axaml.cs`：219→76 行
  - 新建 `ProjectWorldDockTabs.cs`（71 行）— 页签切换 + 搜索状态管理
  - `ProjectWorldDockPanel.axaml` 移除 XAML 事件绑定（移至程序化绑定）
  - 白名单债务 -1（`ProjectWorldDockPanel.axaml.cs` 移出）

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 白名单债务 | 74→71 |

---

### 8.7.7C-1 — NativeHost HWND Lifecycle SRP

`WindowsVulkanViewportHostControl.cs` 窗口类注册 + HostInfo 发布提取。

#### 新增

| 文件 | 行数 | 职责 |
|------|------|------|
| `Win32ViewportWindowClass.cs` | 56 | 窗口类注册 + WndClass struct + WndProc delegate |
| `NativeViewportHostInfoStatics.cs` | 46 | HostInfo 工厂（CreateHostInfo/CreateFailed/CreateUnsupported）+ SyncWindowSize + FormatHandle + SetWindowPos P/Invoke |

#### 修改

- `WindowsVulkanViewportHostControl.cs`：605→402 行
  - 移除窗口类注册、HostInfo 工厂方法、FormatHandle、SetWindowPos
  - 保留：CustomWndProc + 全部输入事件 + 生命周期协调 + capture 管理

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 白名单 | directory +1（NativeHost）, budget 11→12 |

---

### 8.7.7C-2 — NativeHost Raw Pointer Messages SRP

Pointer 消息机械翻译层提取（wParam/lParam 解析 + capture/track）。

#### 新增（5 文件 `NativeHost/Input/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `NativeViewportPointerMessages.cs` | 46 | 消息识别 + Parse() 分发 |
| `NativeViewportPointerRequest.cs` | 40 | wParam/lParam 解析工厂方法 |
| `NativeViewportPointerAction.cs` | 15 | 指针动作枚举 |
| `NativeViewportMouseCapture.cs` | 33 | SetCapture/ReleaseCapture P/Invoke + 状态 |
| `NativeViewportMouseTrack.cs` | 40 | TrackMouseEvent P/Invoke + 状态 |

#### 修改

- `WindowsVulkanViewportHostControl.cs`：402→386 行
  - CustomWndProc 从 2 段式 switch 改为 parse+dispatch 模式
  - 指针消息拆为独立方法（HandleMiddleDown/Up/Move/Wheel/LeftDown/Up）
  - 移除 SetCapture/ReleaseCapture/TrackMouseEvent P/Invoke 和指针消息常量
  - 保留：LButtonDown/Up 仲裁逻辑 / Focus / Keyboard / Nav / SceneTool

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 白名单 | 无变更 |

---

### 8.7.7C-3 — Raw Keyboard / Focus Messages SRP

键盘消息 + 焦点消息 + 命中测试消息提取。目录重组：Input/Pointer/、Input/Keyboard/、Input/Focus/。

#### 目录重组

- `Input/` → `Input/Pointer/`（git mv，5 文件 namespace 更新）
- 新增 `Input/Keyboard/`（2 文件）：`NativeViewportKeyboardMessages.cs`（25 行）、`NativeViewportKeyboardRequest.cs`
- 新增 `Input/Focus/`（2 文件）：`NativeViewportFocusMessages.cs`（19 行、含 SetFocus P/Invoke）、`NativeViewportHitTestMessages.cs`

#### 修改

- `WindowsVulkanViewportHostControl.cs`：386→369 行
  - 移除键盘/焦点/命中测试消息常量
  - 移除 Vk*/Mk* 常量
  - 移除 SetFocus P/Invoke（移至 FocusMessages.SetFocusTo）
  - CustomWndProc 三段式 dispatch：pointer → keyboard → focus/hittest
  - 保留：LButtonDown/Up 仲裁 / HandleKillFocus / HandleCaptureChanged

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 白名单 | 无变更（Pointer/Keyboard/Focus 各 ≤5 文件） |

---

### 8.7.7C-4A — NativeHost Input Arbitration

输入仲裁逻辑提取：Navigation → SceneTool → Legacy Picking 分发顺序。

#### 新增（5 文件 `NativeHost/Input/Arbitration/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `NativeViewportInputArbitration.cs` | 77 | 左键仲裁 + KillFocus/CaptureChanged 清理 |
| `NativeViewportInputArbitrationRequest.cs` | 4 | 仲裁请求数据 |
| `NativeViewportInputArbitrationResult.cs` | 4 | 仲裁消费方枚举 |
| `NativeViewportNavigationCapture.cs` | 26 | Overlay 导航捕获状态 |
| `NativeViewportSceneToolCapture.cs` | 29 | 场景工具捕获状态 |

#### 修改

- `WindowsVulkanViewportHostControl.cs`：369→328 行
  - 移除 `_leftButtonHandledByNavigation` / `_navigationDragCaptured` / `_leftButtonHandledBySceneTool` / `_sceneToolDragCaptured`
  - 移除 `EndNavigationCapture()` / `EndSceneToolCapture()`
  - `HandleLeftButtonDown/Up` / `HandleKillFocus` / `HandleCaptureChanged` 委托至 `NativeViewportInputArbitration`
  - 保留：`_rawPointerDragCaptured`（中键拖拽）、生命周期、P/Invoke
- 新增 `Trace()` 辅助方法统一日志输出

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 新增文件全部 ≤100 行 | ✅ |
| 白名单 | 无变更（Arbitration 5 文件恰好在限内） |

---

### 8.7.7C-4B — NativeHost Lifecycle / PInvoke Cleanup

生命周期创建/销毁/尺寸同步 + Win32 P/Invoke 归位。

#### 新增（7 文件）

**Lifecycle/**（4 文件）

| 文件 | 行数 | 职责 |
|------|------|------|
| `NativeViewportCreate.cs` | 40 | CreateNativeControlCore 主体 + WindowStyle 常量 + CreateWindowEx P/Invoke |
| `NativeViewportDestroy.cs` | 16 | DestroyNativeControlCore 主体 |
| `NativeViewportHostSync.cs` | 37 | SyncAndPublishHostInfo + OnBoundsChanged + _hostInfo 状态管理 |
| `NativeViewportLifecycleResult.cs` | 7 | 创建结果记录 |

**Win32/**（3 文件 + 1 移入）

| 文件 | 行数 | 职责 |
|------|------|------|
| `Win32ViewportWindowClass.cs` | 56 | 窗口类注册（从 NativeHost/ 移入，namespace 更新） |
| `Win32ViewportDefaultProc.cs` | 9 | DefWindowProc P/Invoke |
| `Win32ViewportModuleHandle.cs` | 10 | GetModuleHandle P/Invoke |
| `Win32ViewportDestroyWindow.cs` | 9 | DestroyWindow P/Invoke |

#### 修改

- `WindowsVulkanViewportHostControl.cs`：328→275 行
  - 移除 WindowStyle 常量、_width/_height 字段
  - CreateNativeControlCore → 5 行委托 + 3 行错误分支
  - DestroyNativeControlCore → 1 行委托 + reset
  - SyncAndPublishHostInfo → 3 行委托
  - 移除 GetModuleHandle/CreateWindowEx/DestroyWindow/DefWindowProc P/Invoke
  - _hostInfo 字段替换为 _hostSync (NativeViewportHostSync)
  - CustomWndProc 使用 Win32ViewportDefaultProc.DefWindowProc
  - 保留：全部输入事件、仲裁、输入消息处理

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 新增文件全部 ≤56 行 ✅ |
| Lifecycle/ 4 文件、Win32/ 4 文件（各 ≤5） ✅ |
| 白名单 | 无变更 |

---

### 8.7.7C-4C — NativeHost Final Thin Control

主文件压至 ≤100 行，NativeHost 目录白名单删除。

#### 目录重组

- 新增 `Control/`（2 partial）：`Events.cs`（31 行）、`WndProc.cs`（94 行）
- 新增 `HostInfo/`（git mv）：`WindowsVulkanViewportHostInfo.cs` / `HostState.cs` / `NativeViewportHostInfo.cs`
- 新增 `Picking/`（git mv）：`WindowsVulkanViewportPickInput.cs`
- 新增 `SceneTool/`（git mv）：`ViewportSceneToolPressResult.cs`

#### 修改

- `WindowsVulkanViewportHostControl.cs`：275→87 行（≤100 ✅）
  - 事件声明 → `Control/Events.cs` partial
  - WndProc 及所有输入处理器 → `Control/WndProc.cs` partial
  - 主文件仅保留：字段、构造、Create/Destroy/SyncHostInfo 薄委托、CustomWndProc 入口

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| `WindowsVulkanViewportHostControl.cs` | 87 行 ✅ ≤100 |
| NativeHost 根目录直属 .cs | 1 文件 ✅ ≤5 |
| NativeHost 目录白名单 | ✅ 已删除（DirectoryWhitelistBudget 12→11） |
| 新增文件全部 ≤100 | ✅ |
| `file-tree.md` / `CHANGELOG.md` | ✅ 已更新 |

---

### 8.7.7D-1 — VulkanScene3dSession Ownership Map

VulkanScene3dSession 属性/状态/句柄所有权结构提取。

#### 新增

**Session/**（1 partial）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanScene3dSession.Properties.cs` | 87 | 公共属性 + 薄方法（SetSelectedEntity/UpdateEntityPosition/SetGroundCursor 等） |

**Lifecycle/**（1 文件）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanScene3dSessionState.cs` | 36 | Session 运行时状态记录（Status/计数器/成功标记/Overlay 状态） |

**Handles/**（3 文件）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanScene3dCoreHandles.cs` | 16 | Instance/Device/Surface/Queue 句柄集合 |
| `VulkanScene3dSwapchainHandles.cs` | 20 | Swapchain 级句柄与函数指针集合 |
| `VulkanScene3dFrameHandles.cs` | 24 | 帧级资源句柄（Shader/Pipeline/Buffer/Overlay） |

#### 修改

- `VulkanScene3dSession.cs`：1304→1185 行
  - 属性/薄方法 → `Properties.cs` partial
  - 主文件保留：字段声明、Start/RenderFrame/Resize/RenderFrameInternal、DisposeResources、创建辅助方法
  - 类声明添加 `partial` 修饰符

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 新增文件全部 ≤87 行 | ✅ |
| `file-tree.md` / `CHANGELOG.md` | ✅ 已更新 |

---

### 8.7.7D-2 — Vulkan Swapchain Selection / Extent

Swapchain 表面格式选择、PresentMode 选择、Extent 计算提取。

#### 新增（`Session/Swapchain/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanScene3dSwapchainSelection.cs` | 21 | ChooseFormat + ChoosePresentMode（从 SwapchainResources 提取） |
| `VulkanScene3dSwapchainExtent.cs` | 16 | ChooseExtent（从 SwapchainResources 提取） |

#### 修改

- `VulkanScene3dSwapchainResources.cs`：424→401 行
  - 移除 3 个私有静态方法（ChooseFormat / ChoosePresentMode / ChooseExtent）
  - 改为调用 `VulkanScene3dSwapchainSelection.*` 和 `VulkanScene3dSwapchainExtent.ChooseExtent`

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 新增文件全部 ≤21 行 | ✅ |
| 白名单 | Swapchain 目录 +1（6 文件），DirectoryWhitelistBudget 11→12 |

---

### 8.7.7D-2B — Swapchain Resources SRP 继续拆

ImageViews / Framebuffers / CommandPool / Sync 对象从 SwapchainResources 提取。

#### 新增

| 文件 | 行数 | 职责 |
|------|------|------|
| `Swapchain/Images/VulkanScene3dSwapchainImageViews.cs` | 43 | Color ImageView 创建（从 TryCreate 提取） |
| `Swapchain/Images/VulkanScene3dSwapchainFramebuffers.cs` | 72 | RenderPass + Framebuffer 创建（从 TryCreate 提取） |
| `Swapchain/Sync/VulkanScene3dSwapchainSync.cs` | 56 | CommandPool / CommandBuffer / Semaphore / Fence 创建 |

#### 修改

- `VulkanScene3dSwapchainResources.cs`：424→294 行
  - Color ImageViews 循环 → `VulkanScene3dSwapchainImageViews.Create`
  - RenderPass + Framebuffers 创建 → `VulkanScene3dSwapchainFramebuffers.*`
  - CommandPool / CommandBuffer / Semaphore / Fence 创建 → `VulkanScene3dSwapchainSync.*`
  - 保留：字段/构造/Surface caps/Format选择/Swapchain创建/Image获取/Depth/Dispose

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 新增文件全部 ≤72 行 | ✅ |

---

### 8.7.7D-2C — Swapchain CreateFlow + Dispose 提取

Swapchain 创建流程 + 销毁逻辑从 SwapchainResources 提取。

#### 新增

| 文件 | 行数 | 职责 |
|------|------|------|
| `Swapchain/Create/VulkanScene3dSwapchainCreateFlow.cs` | 86 | Surface caps / 格式 / PresentMode / 创建 / Image 获取一体化流程 |
| `Swapchain/Lifecycle/VulkanScene3dSwapchainDispose.cs` | 55 | 幂等销毁辅助（Framebuffer/RenderPass/Depth/ColorViews/Swapchain） |

#### 修改

- `VulkanScene3dSwapchainResources.cs`：294→113 行（≤150 ✅）
  - Surface caps/Swapchain 创建/Image 获取 → `CreateFlow.Execute()`
  - Dispose 主体 → `VulkanScene3dSwapchainDispose.DisposeResources()`
  - TryCreate 改为编排模式：CreateFlow → ImageViews → Depth → Framebuffers → Sync
  - 字段改为 public（支持 ref 传给 Dispose 辅助）

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| `VulkanScene3dSwapchainResources.cs` | 113 行 ✅ ≤150 |
| 新增文件全部 ≤86 行 | ✅ |

---

### 8.7.7D-3 — Vulkan Frame Resources

RenderFrameInternal 帧资源管理提取：Camera MVP / UnitDraw / Overlay / Submit / Present。

#### 新增（`Session/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanScene3dSession.Frame.cs` | 98 | ComputeMVP / BuildUnitDrawData / BuildGroundCursorData / BuildOverlay / SubmitFrame / PresentFrame |

#### 修改

- `VulkanScene3dSession.cs`：1185→1068 行
  - Camera MVP 计算 → `ComputeViewProjection()`
  - UnitDrawData 构建 → `BuildUnitDrawData()`
  - GroundCursor 数据 → `BuildGroundCursorData()`
  - Overlay 几何生成 → `BuildOverlay()`
  - QueueSubmit → `SubmitFrame()`
  - QueuePresent → `PresentFrame()`
  - RenderFrameInternal 从约 260 行降至约 120 行编排

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| 新增文件 ≤98 行 | ✅ |
| `file-tree.md` / `CHANGELOG.md` | ✅ 已更新 |

---

### 8.7.7D-4 — Acquire / Present Route

AcquireNextImageKHR / QueueSubmit / QueuePresentKHR 调用及结果分类从 `Session.cs` 和 `Frame.cs` 提取到独立 `FrameFlow/` 目录。

#### 新增（`Session/FrameFlow/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanScene3dFrameAcquire.cs` | 100 | AcquireNextImage 调用 + ClassifyAcquireResult（8 分支判读） |
| `VulkanScene3dFrameSubmit.cs` | 25 | SubmitFrame（QueueSubmit 调用） |
| `VulkanScene3dFramePresent.cs` | 66 | PresentFrame（函数指针调用）+ ClassifyPresentResult（6 分支判读） |
| `VulkanScene3dFrameFailure.cs` | 17 | FailFrame 文案构造（DisposeResources + 状态标记） |

#### 修改

- `VulkanScene3dSession.cs`：1068→941 行
  - 删除 `ClassifyAcquireResult()`（迁至 FrameAcquire.cs）
  - 删除 `ClassifyPresentResult()`（迁至 FramePresent.cs）
  - 删除 `FailFrame()`（迁至 FrameFailure.cs）
  - `RenderFrameInternal` 中 Acquire 内联 18 行 → `AcquireNextImage(reason)` 单行调用（语义等价）
- `VulkanScene3dSession.Frame.cs`：98→70 行
  - 删除 `SubmitFrame()`（迁至 FrameSubmit.cs）
  - 删除 `PresentFrame()`（迁至 FramePresent.cs）

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| `dotnet run Editor --no-build` | ✅ 成功 |
| `VulkanScene3dSession.cs` | 941 行 ✅（目标 850-950） |
| `VulkanScene3dSession.Frame.cs` | 70 行 ✅（下降 28 行，< 红线 100） |
| 新增文件全部 ≤100 行 | ✅ |

---

### 8.7.7D-5A — Dispose Order Map

DisposeResources / Dispose 释放顺序显式化：从 Session.cs 提取到独立 `Dispose/` 目录，按编号步骤记录顺序。

#### 新增（`Session/Dispose/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanScene3dSessionDisposeResources.cs` | 77 | DisposeResources 12 步释放顺序（Swapchain → Pipeline → Layout → Shader → Buffer → Overlay） |
| `VulkanScene3dSessionDisposeSession.cs` | 94 | DisposeSessionResources 13 步释放顺序（含 Device/Surface/Messenger/Instance） |
| `VulkanScene3dSessionDisposeState.cs` | 32 | 资源创建标记查询与重置（IsDeviceValid, ClearAllResourceFlags） |
| `VulkanScene3dSessionDisposeTrace.cs` | 31 | Dispose 诊断日志辅助（DEBUG 模式） |

#### 修改

- `VulkanScene3dSession.cs`：941→840 行
  - `DisposeResources()` → `VulkanScene3dSessionDisposeResources.cs`
  - `Dispose()` 身体 → `DisposeSessionResources()` + `ClearAllResourceFlags()` 薄转发
- 释放顺序未被改变，步骤未被合并，日志未被吞

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| 新增文件全部 ≤94 行 | ✅ |
| `VulkanScene3dSession.cs` | 840 行 ✅ |

---

### 8.7.7D-5B — Resource Dispose Steps

将 DisposeResources / DisposeSessionResources 中的内联释放动作拆为具名小步骤文件。
释放顺序不变，动作不合并。

#### 新增

| 文件 | 行数 | 职责 |
|------|------|------|
| `Dispose/Render/VulkanScene3dPipelineDispose.cs` | 23 | `DisposeUnitPipelineStep` / `DisposeGridPipelineStep` / `DisposePipelineLayoutStep` |
| `Dispose/Render/VulkanScene3dShaderDispose.cs` | 18 | `DisposeFragmentShaderStep` / `DisposeVertexShaderStep` |
| `Dispose/Render/VulkanScene3dBufferDispose.cs` | 38 | `DisposeUnitBufferStep` / `DisposeGridBufferStep` / `DisposeCursorBufferStep`（各 Buffer+Memory 独立） |
| `Dispose/Render/VulkanScene3dOverlayDispose.cs` | 23 | `DisposeOverlayResourcesStep` / `ClearFrameOverlayState` |
| `Dispose/Core/VulkanScene3dCoreDispose.cs` | 40 | `DisposeDeviceStep` / `DisposeSurfaceStep` / `DisposeDebugMessengerStep` / `DisposeInstanceStep` |

#### 重构

- `VulkanScene3dSessionDisposeResources.cs`：77→34 行 — 编排器模式，12 步全部委托到具名方法
- `VulkanScene3dSessionDisposeSession.cs`：94→31 行 — 编排器模式，13 步全部委托到具名方法

#### 目录重组

根据≤5 文件/目录规则，`Dispose/` 拆为二级结构：

```
Dispose/
├── VulkanScene3dSessionDisposeResources.cs          ← 编排器
├── VulkanScene3dSessionDisposeSession.cs            ← 编排器
├── Render/    (Pipeline / Shader / Buffer / Overlay)
├── Core/      (Device / Surface / DebugMessenger / Instance)
└── State/     (DisposeState / DisposeTrace)
```

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| 所有新增/修改文件 ≤40 行 | ✅ |
| `Dispose/` 每目录 ≤4 文件 | ✅ |
| 释放顺序不变 | ✅ |

---

### 8.7.7D-6A — Session Start / Create Methods SRP

Start() 编排器 + 所有 Create* 方法从 `VulkanScene3dSession.cs` 提取到独立 `Start/` 目录。

#### 新增（`Session/Start/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanScene3dSessionStart.cs` | 94 | Start() 编排器：顺序调用 Create* 步骤 |
| `VulkanScene3dSessionCreateInstance.cs` | 74 | CreateInstance（含 Debug Messenger 初始化） |
| `VulkanScene3dSessionCreateSurface.cs` | 49 | CreateSurface + LoadSessionFunctionPointers |
| `VulkanScene3dSessionCreateDevice.cs` | 83 | SelectDevice + CreateDevice |
| `VulkanScene3dSessionCreateResources.cs` | 55 | CreateShaderModules / PipelineLayout / VertexBuffers / GroundCursor |

#### 修改

- `VulkanScene3dSession.cs`：840→499 行
  - Start() → `VulkanScene3dSessionStart.cs`
  - 全部 9 个 Create* 方法 → 对应 `Create*.cs` 文件
  - 创建顺序不变

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| 新增文件全部 ≤94 行 | ✅ |
| `Start/` 每目录 ≤5 文件 | ✅ |
| `VulkanScene3dSession.cs` | 499 行 ✅ |

---

### 8.7.7D-6B — Session Render / Resize Thin Route

RenderFrame / Resize / RenderFrameInternal 从 `VulkanScene3dSession.cs` 提取到 `Session/Render/`。

#### 新增（`Session/Render/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanScene3dRenderFrame.cs` | 32 | RenderFrame 入口（状态检查 + 防重入） |
| `VulkanScene3dRenderResize.cs` | 87 | Resize 编排器（检查 → DeviceWaitIdle → 事务 → 不变量） |
| `VulkanScene3dRenderResizeAtomic.cs` | 79 | Resize 事务（Pipeline/Overlay 创建 → 原子切换 → 释放旧资源） |
| `VulkanScene3dRenderFrameInternal.cs` | 80 | 帧编排（Fence → Acquire → Compute → Record → Submit → Present） |
| `VulkanScene3dRenderFrameSnapshot.cs` | 62 | 相机快照 + Overlay 快照 + FrameResult 构造 |

#### 修改

- `VulkanScene3dSession.cs`：499→177 行
  - RenderFrame / Resize / RenderFrameInternal → `Session/Render/`
  - 渲染流程不变，Resize 事务逻辑不变

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| 新增文件全部 ≤87 行 | ✅ |
| `Render/` 每目录 ≤5 文件 | ✅ |
| `VulkanScene3dSession.cs` | 177 行 ✅ |

---

### 8.7.7D-6C — Session Field / Handle Split

VulkanScene3dSession 的字段声明、状态标记、LoadProc/LoadDeviceProc 从主文件提取到 `Session/Core/`。

#### 新增（`Session/Core/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanScene3dSessionCoreState.cs` | 55 | Vulkan 核心句柄 + 函数指针 + 帧常量 |
| `VulkanScene3dSessionRenderState.cs` | 30 | Swapchain/运行时状态 + 验证字段 |
| `VulkanScene3dSessionOverlayState.cs` | 22 | Overlay + Gizmo 状态字段 |
| `VulkanScene3dSessionResourceFlags.cs` | 13 | _ok 资源创建标记 |
| `VulkanScene3dSessionProcLoad.cs` | 23 | LoadProc / LoadDeviceProc |

#### 修改

- `VulkanScene3dSession.cs`：177→53 行
  - 全部字段声明 → `Session/Core/`
  - LoadProc/LoadDeviceProc → `VulkanScene3dSessionProcLoad.cs`
  - 仅保留 class 声明 + Dispose + 委托定义

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| 新增文件全部 ≤55 行 | ✅ |
| `Core/` 每目录 ≤5 文件 | ✅ |
| `VulkanScene3dSession.cs` | **53 行** ✅ |

---

### 8.7.7D-6D — Session 白名单删除 + 目录债务清理

代码宪法白名单清理 + Session 目录重整收口。

#### 目录迁移

| 路径 | 操作 | 目的 |
|------|------|------|
| `Session/FrameModel/` | 新建 | FrameReason / FrameResult / FrameStatus 从 Session/ 根迁入 |
| `Session/Core/VulkanScene3dSessionStatus.cs` | 合并 | 枚举合并到 CoreState.cs，删除独立文件 |
| `Session/Swapchain/Choice/` | 新建 | SwapchainSelection / SwapchainExtent 迁入 |
| `Session/Swapchain/Resources/` | 新建 | SwapchainResources 迁入并压缩至 100 行 |
| `Session/` 根目录 | 清理 | 10→3 直属文件（Session.cs / Frame.cs / Properties.cs） |

#### 白名单删除

- 删除 `s_lineWhitelist`: VulkanScene3dSession.cs（53 行 ✅）, VulkanScene3dSwapchainResources.cs（100 行 ✅）
- 删除 `s_directoryWhitelist`: Session/（3 根文件 ✅）, Session/Swapchain/（4 根文件 ✅）

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| `VulkanScene3dSession.cs` | 53 行 ✅ ≤100 |
| `SwapchainResources.cs` | 100 行 ✅ ≤100 |
| `Session/` 根目录 | 3 文件 ✅ ≤5 |
| `Swapchain/` 根目录 | 4 文件 ✅ ≤5 |
| 所有子目录 | ≤5 文件 ✅ |
| 白名单债务 | Session/Swapchain 无需白名单 ✅ |

---

### 8.7.7E-1 — VulkanScene3dRenderer 审计

Renderer 代码审计，不涉及代码修改。

#### 审计发现

| 维度 | 结果 |
|------|------|
| `VulkanScene3dRenderer.cs` | 477 行（>100，白名单债务） |
| `Scene3D/` 根目录 | 13 个 .cs 文件（>5，目录白名单债务） |
| Scene3D 子模块 >100 行 | Renderer(477) / CommandRecorder(200) / VertexBuffers(171) / Vertex(171) / Pipelines(153) / RenderResources(138) |
| 与 Session 重复率 | ~80%（Instance/Device/Surface/Swapchain 创建完全重复） |
| 调用方 | `EditorScene3dCommandRoute.ExecuteRun()` — 诊断探针路径 |
| 是否可删除 | 待确认。Session 主路径已运行正常，但探针路径仍有独立用途 |

#### 输出

- 审计文档：`docs/renderer-audit-8.7.7E-1.md`
- 资源/职责/调用关系完整映射

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| 不引入新白名单债务 | ✅ |
| 审计输出完整 | ✅ |

---

### 8.7.7E-2A — Scene3D 目录合规

Scene3D/ 根目录 13 个 .cs 文件按职责归类到子目录。仅移动 + 更新白名单，不修改渲染行为。

#### 目录迁移

| 目标目录 | 文件 | 新路径 |
|---------|------|--------|
| `Pipeline/` | Pipelines / PipelineLayout / ShaderModules / PushConstants | 4 文件 |
| `Vertex/` | Vertex / VertexBuffers / AxisGeometry | 3 文件 |
| `Render/` | Renderer / RenderResources / RunGate / Info / Status | 5 文件 |
| `Commands/` | CommandRecorder | 1 文件 |
| `Scene3D/` 根目录 | 13→**0** 直属 .cs（仅子目录） | ≤5 ✅ |

#### 白名单更新

- 6 条行白名单路径 → 新子目录路径
- 删除 `Scene3D\` 目录白名单（0 根文件 ✅）
- Overlay 目录白名单保留（8 文件，待后续拆分）

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| `Scene3D/` 根目录 .cs | 0 文件 ✅ ≤5 |
| 各子目录 ≤5 文件 | ✅ |
| 白名单债务未新增 | ✅ |

---

### 8.7.7E-2B-1 — Pipeline 模块收口

`VulkanScene3dPipelines.cs` 从 153 行拆至 77 行。仅提取管线创建辅助，不改行为。

#### 新增

| 文件 | 行数 | 职责 |
|------|------|------|
| `Pipeline/VulkanScene3dPipelineCreate.cs` | 70 | 管线状态构建（Rasterizer/Depth/Multisample）+ TryCreate 调用 |

#### 修改

- `Pipeline/VulkanScene3dPipelines.cs`：153→77 行
  - 管线创建逻辑 → `TryCreate()` 辅助
  - 状态构建（BuildRasterizerState / BuildDepthStencilState / BuildMultisampleState）→ `PipelineCreate.cs`
  - 编排器模式：共享状态就地构建，两次 TryCreate 调用

#### 白名单

- `VulkanScene3dPipelines.cs` 从 `s_lineWhitelist` 删除（77 行 ✅ ≤100）

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| `Pipeline/` 文件数 | 5 ✅ ≤5 |
| `VulkanScene3dPipelines.cs` | 77 行 ✅ ≤100 |

---

### 8.7.7E-2B-2A — Vertex 结构收口

`VulkanScene3dVertex.cs` 从 171 行拆至 9 行。结构体 + 几何生成分离。

#### 新增

| 文件 | 行数 | 职责 |
|------|------|------|
| `Vertex/VulkanScene3dVertexGrid.cs` | 43 | BuildGrid / BuildAxes / ToInterleaved |
| `Vertex/VulkanScene3dVertexCube.cs` | 34 | BuildCube（36 顶点，6 面） |

#### 修改

- `Vertex/VulkanScene3dVertex.cs`：171→9 行
  - 仅保留 `VulkanScene3dVertex` 和 `VulkanScene3dUnitDrawInfo` 记录结构
  - `VulkanScene3dVertices` 静态类 → `partial class` 分散到 Grid/Cube 文件

#### 白名单

- `VulkanScene3dVertex.cs` 从 `s_lineWhitelist` 删除（9 行 ✅ ≤100）

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| `Vertex/` 文件数 | 5 ✅ ≤5 |
| `VulkanScene3dVertex.cs` | 9 行 ✅ ≤100 |

---

### 8.7.7E-2B-2B — VertexBuffers 收口

`VulkanScene3dVertexBuffers.cs` 从 171 行拆至 59 行。Buffer 创建逻辑迁入 `Vertex/Buffer/` 子目录。

#### 新增

| 文件 | 行数 | 职责 |
|------|------|------|
| `Vertex/Buffer/VulkanScene3dVertexBufferCreate.cs` | 53 | CreateOneBuffer 完整管线（CreateBuffer→Alloc→Bind→Map→Copy→Unmap）+ FindMemoryType |

#### 修改

- `Vertex/VulkanScene3dVertexBuffers.cs`：171→59 行
  - 编排器模式：Create/CreateCursor 公开入口，委托 `CreateOneBuffer`
  - `CreateOneBuffer` + `FindMemoryType` → `Buffer/VulkanScene3dVertexBufferCreate.cs`

#### 结构

```
Vertex/
├── VulkanScene3dVertex.cs              9    [记录结构]
├── VulkanScene3dVertexGrid.cs          43   [Grid/Axes/ToInterleaved]
├── VulkanScene3dVertexCube.cs          34   [Cube geometry]
├── VulkanScene3dVertexBuffers.cs       59   [编排器]
├── VulkanSceneAxisGeometry.cs          33   [独立]
└── Buffer/                             1    [创建/上传]
    └── VulkanScene3dVertexBufferCreate.cs  53
```

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| `Vertex/` 根目录 | 5 文件 ✅ ≤5 |
| `Vertex/Buffer/` 子目录 | 1 文件 ✅ ≤5 |
| `VertexBuffers.cs` | 59 行 ✅ ≤100 |

---

### 8.7.7E-2B-3 — CommandRecorder 模块收口

`VulkanScene3dCommandRecorder.cs` 从 200 行拆至 45 行。按录制阶段分拆到 3 个辅助文件。

#### 新增

| 文件 | 行数 | 职责 |
|------|------|------|
| `Commands/VulkanScene3dCommandRenderPass.cs` | 49 | RecordBegin / RecordBeginRenderPass / RecordEnd（ClearValues + RenderPass 开始/结束） |
| `Commands/VulkanScene3dCommandGrid.cs` | 52 | RecordDrawGrid + RecordDrawGroundCursor（Grid LineList + Cursor LineList，共享 Grid Pipeline） |
| `Commands/VulkanScene3dCommandUnits.cs` | 56 | RecordDrawUnits + RecordDrawOverlay（Unit TriangleList 循环 + Overlay 最后叠加） |

#### 修改

- `Commands/VulkanScene3dCommandRecorder.cs`：200→45 行
  - 编排器：Record() 入口调用 6 个录制阶段
  - 数据记录 UnitDrawData / GroundCursorDrawData 保留在编排器

#### 指针安全

所有 stackalloc / fixed 在辅助方法内创建并立即使用，不返回带栈指针的 struct。

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| `Commands/` 文件数 | 4 ✅ ≤5 |
| `CommandRecorder.cs` | 45 行 ✅ ≤100 |
| 录制顺序不变 | ✅ Begin→RenderPass→Grid→Cursor→Units→Overlay→End |

---

### 8.7.7E-2B-3R — Commands 严格 SRP 复核

CommandGrid 和 CommandUnits 按 SRP 拆分为单个职责文件。Commands/ 根目录拆为 Core/ 和 Draw/ 二级目录。

#### 目录变更

| 旧 | 新 |
|----|----|
| `Commands/VulkanScene3dCommandGrid.cs`（Grid + Cursor 复合） | `Commands/Draw/CommandGrid.cs` + `Commands/Draw/CommandGroundCursor.cs` |
| `Commands/VulkanScene3dCommandUnits.cs`（Units + Overlay 复合） | `Commands/Draw/CommandUnits.cs` + `Commands/Draw/CommandOverlay.cs` |
| `Commands/VulkanScene3dCommandRecorder.cs`（根目录） | `Commands/Core/CommandRecorder.cs` |
| `Commands/VulkanScene3dCommandRenderPass.cs`（根目录） | `Commands/Core/CommandRenderPass.cs` |

#### 结构

```
Commands/Core/   (2文件) 编排器 + RenderPass
Commands/Draw/   (4文件) Grid / GroundCursor / Units / Overlay
```

#### 验收

| 指标 | 值 |
|------|-----|
| `Commands/Core/` 文件数 | 2 ✅ ≤5 |
| `Commands/Draw/` 文件数 | 4 ✅ ≤5 |
| 全部 ≤56 行 | ✅ |

---

### 8.7.7E-2B-4 — RenderResources 收口

`VulkanScene3dRenderResources.cs` 从 138 行拆至 48 行。按资源职责分拆。

#### 新增

| 文件 | 行数 | 职责 |
|------|------|------|
| `Render/Resources/VulkanScene3dDepthResource.cs` | 14 | Depth Image / Memory / View 字段 |
| `Render/Resources/VulkanScene3dResourceRelease.cs` | 48 | 释放步骤：Fence→Pool→Buffer→Pipeline→Shader→Framebuffer→Depth→RenderPass→ImageView→Swapchain→Device→Surface→Instance |

#### 修改

- `Render/Resources/VulkanScene3dRenderResources.cs`（移入 Resources/）：138→48 行
  - 核心字段 + IDisposable + 3 步释放编排
  - ReleaseSwapchainAndSurface / ReleaseRenderResources / ReleaseDeviceAndInstance

#### 白名单

- `RenderResources.cs` 从 `s_lineWhitelist` 删除（48 行 ✅ ≤100）

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| `Resources/` 文件数 | 3 ✅ ≤5 |
| `RenderResources.cs` | 48 行 ✅ ≤100 |

---

### 8.7.7E-2C — VulkanScene3dRenderer 去重式 SRP 收口

`VulkanScene3dRenderer.cs` 从 477 行拆至 36 行，按 SRP 分为 5 文件。

#### 新增（`Render/Probe/`）

| 文件 | 行数 | 职责 |
|------|------|------|
| `Probe/VulkanScene3dRenderer.cs` | 36 | 编排器：RenderWindows 三阶段调用 |
| `Probe/VulkanScene3dRendererSetup.cs` | 99 | ProbeCreateSession（Instance→Device→Swapchain→Resources 全流程编排） |
| `Probe/VulkanScene3dRendererCreate.cs` | 67 | CreateInstance/CreateSurface/SelectDevice/CreateDevice/LoadProc/Fail |
| `Probe/VulkanScene3dRendererSurface.cs` | 27 | QueryCaps/Formats/Modes + ChooseFormat/PresentMode/Extent |
| `Probe/VulkanScene3dRendererFrame.cs` | 78 | ProbeRenderFrame（Acquire→MVP→Record→Submit→Present→Result） |

#### 白名单

- `VulkanScene3dRenderer.cs` 从 `s_lineWhitelist` 删除（36 行 ✅ ≤100）
- E-2C 不新增白名单

#### 结构

```
Render/
├── RunGate.cs / Info.cs / Status.cs           (3 根文件)
├── Resources/                                  (RenderResources)
└── Probe/                                      (诊断探针, 5 文件)
```

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error / 0 新 Warning |
| `dotnet test` | ✅ 625/625 |
| `Render/` 根目录 | 3 文件 ✅ ≤5 |
| `Render/Probe/` 目录 | 5 文件 ✅ ≤5 |
| `Renderer.cs` | 36 行 ✅ ≤100 |
| 全部 Probe 文件 | ≤99 行 ✅ |

---

### 8.7.7E-2C-R — Probe 严格 SRP 复核补丁

**问题：** Probe/ 文件存在潜在复合职责：
- `VulkanScene3dRendererCreate.cs` 同时实现 Instance + Device + Surface 创建
- `VulkanScene3dRendererFrame.cs` 同时实现 Acquire + MVP + Submit + Present + Result
- `VulkanScene3dRendererSetup.cs` 内联实现 Swapchain/ImageView/Framebuffer 创建

**操作：**

1. `Create.cs` (67 行) → `Create/` 子目录 5 文件：
   - `ProbeInstance.cs` — LoadProc + CreateInstance + PackVer (24 行)
   - `ProbeDevice.cs` — LoadDeviceProc + SelectDevice + CreateDevice (59 行)
   - `ProbeSurfaceCreate.cs` — CreateSurface (21 行)
   - `ProbeSwapchain.cs` — ProbeCreateSwapchain 提取自 Setup (40 行)
   - `ProbeResources.cs` — ImageView/Framebuffer/CmdPool/Sync 创建提取自 Setup (50 行)
2. `Frame.cs` (78 行) → `Frame/` 子目录 5 文件：
   - `ProbeAcquire.cs` — AcquireNextImage (23 行)
   - `ProbeMVP.cs` — MVP 矩阵 + 逐对象变换 (31 行)
   - `ProbeSubmit.cs` — QueueSubmit (19 行)
   - `ProbePresent.cs` — QueuePresent + FrameResult (49 行)
   - `ProbeFrame.cs` — 帧编排器 (39 行)
3. `Surface.cs` → `Surface/ProbeSurfaceChoice.cs` (27 行)
4. `Renderer.cs` → `Core/VulkanScene3dRenderer.cs` 编排器 (+Fail 工厂) (41 行)
5. Setup.cs 变薄：内联实现提取到 Create/ 子目录 (79 行)

**目录结构：**
```
Render/Probe/
├── VulkanScene3dRendererSetup.cs          (79 行) [99→79]
├── Core/
│   └── VulkanScene3dRenderer.cs           (41 行)
├── Create/ (5 文件)
│   ├── Instance.cs / Device.cs / SurfaceCreate.cs
│   └── Swapchain.cs / Resources.cs
├── Surface/
│   └── SurfaceChoice.cs                   (27 行)
└── Frame/ (5 文件)
    ├── Frame.cs / Acquire.cs / MVP.cs / Submit.cs / Present.cs
```

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` (架构) | ✅ 5/5 |
| `Probe/` 根目录 | 2 文件 ✅ ≤5 |
| `Create/` 目录 | 5 文件 ✅ ≤5 |
| `Frame/` 目录 | 5 文件 ✅ ≤5 |
| `Surface/` 目录 | 1 文件 ✅ ≤5 |
| 全部 .cs 文件 | ≤79 行 ✅ |
| 无复合职责文件 | ✅ |

---

### 8.7.7E-2D — Scene3D 白名单删除 + Overlay 目录整理

**问题：** Scene3D 残留 4 个白名单债务：
- `Overlay/` 目录 8 文件（目录白名单）
- `VulkanNavigationOverlayGeometry.cs` 308 行（行白名单）
- `VulkanOverlayResources.cs` 235 行（行白名单）
- `VulkanOverlayPipeline.cs` 130 行（行白名单）
- `VulkanScene3dDepthAttachments.cs` 125 行（行白名单）

**操作：**

1. **DepthAttachments.cs** (125→86 行)：提取 `FindDepthMemoryType` + `CreateDepthImageView` 辅助方法
2. **OverlayGeometry.cs** (308 行) → `Geometry/` 子目录 3 文件：
   - `VulkanNavigationOverlayGeometry.cs` (84 行) — Build 入口 + AdjustColor
   - `VulkanNavigationOverlayPrimitives.cs` (62 行) — 绘图图元
   - `VulkanNavigationOverlayShapes.cs` (98 行) — 字母 + 按钮
3. **OverlayResources.cs** (235 行) → `Resources/` 子目录 2 文件：
   - `VulkanOverlayResources.cs` (83 行) — 类定义 + TryCreate + Upload + Dispose
   - `VulkanOverlayResources.Create.cs` (54 行) — 创建辅助方法
4. **OverlayPipeline.cs** (130→55 行)：压缩 → 每行一个 Vulkan 结构体
5. **Overlay 目录重组**：8 文件 → `Geometry/`(5) + `Resources/`(5) + `Render/`(2)
6. **白名单删除：** 4 行条目 + 1 目录条目 → 预算 71→67 行 / 12→11 目录

#### 白名单清理

| 旧条目 | 之前 | 之后 | 结果 |
|--------|------|------|------|
| `Overlay/` (目录) | 8 文件 >5 | 3 子目录 ≤5 | ✅ 删除 |
| `OverlayGeometry.cs` | 308 行 | 84 行 (Geometry/) | ✅ 删除 |
| `OverlayResources.cs` | 235 行 | 83 行 (Resources/) | ✅ 删除 |
| `OverlayPipeline.cs` | 130 行 | 55 行 (Resources/) | ✅ 删除 |
| `DepthAttachments.cs` | 125 行 | 86 行 | ✅ 删除 |

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` (架构) | ✅ 5/5 |
| Scene3D 白名单行 | ✅ 0 (全部删除) |
| Scene3D 白名单目录 | ✅ 0 (全部删除) |
| 全部 Overlay .cs 文件 | ≤98 行 ✅ |
| 全部 Overlay 目录 | ≤5 文件 ✅ |
| 无复合职责文件 | ✅ |

---

### 8.7.7F-1 — 全仓白名单债务盘点

扫码剩余白名单，产出审计文档 `docs/whitelist-audit-8.7.7F-1.md`。

#### 当前白名单快照

| 指标 | 值 |
|------|-----|
| 行白名单条目 | 49 项（预算 67） |
| 目录白名单条目 | 8 项（预算 11） |
| 立即可清 | 3 文件（≤100 行，白名单残留） |
| 小修可清 | 9 文件（101～150 行）+ 5 目录（6～7 文件） |
| 中等债务 | 12 文件（151～300 行）+ 3 目录（8 文件） |
| 大债务 → 8.7.8 | 4 文件（>300 行） |
| 测试文件 | 20 项（保留） |

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` (架构) | ✅ 5/5 |
| 审计文档输出 | ✅ `docs/whitelist-audit-8.7.7F-1.md` |

---

### 8.7.7F-3 — 小修可清

处理 9 个 101～150 行文件 + 4 个 6～7 文件目录。

#### 目录移动（4 目录白名单删除 📋）

| 目录 | 之前 | 之后 | 操作 |
|------|------|------|------|
| `Validation/` | 7 文件 | 2+Info/5 | 5 数据文件 → Info/ 子目录 |
| `Camera/` | 7 文件 | 5+Orbit/2 | SceneOrbitMotion/State → Orbit/ 子目录 |
| `ProjectContentTree/` | 6 文件 | 5+Panel/1 | Panel.axaml.cs → Panel/ 子目录 |
| `Transform/Drag/` | 6 文件 | 4+Anchors/2 | MoveResult/Snapshot → Anchors/ 子目录 |

#### 文件压缩（9 文件白名单删除 📋）

| 文件 | 之前 | 之后 | 节约 |
|------|------|------|------|
| `WorldHierarchyTreeIndex.cs` | 112 | 46 | -66 |
| `ProjectContentNodeView.cs` | 114 | 52 | -62 |
| `WorldHierarchyTreeBuilder.cs` | 126 | 49 | -77 |
| `EditorInputActionCatalog.cs` | 148 | 60 | -88 |
| `VulkanInstanceProbe.cs` | 122 | 43 | -79 |
| `VulkanDebugMessengerScope.cs` | 133 | 55 | -78 |
| `VulkanValidationAvailabilityProbe.cs` | 118 | 40 | -78 |
| `GameContentFileScanner.cs` | 130 | 38 | -92 |
| `WorldState.cs` | 121 | 48 | -73 |

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` (架构) | ✅ 5/5 |
| 目录白名单删除 | 4 项 (Validation/Camera/Tree/Drag) |
| 文件白名单删除 | 9 项 |
| 文件预算 | 64→55 |
| 目录预算 | 11→7 |

---

### 8.7.7F-4A — 中等债务第一组清理

#### DebugDockPanel SRP 提取 (145→53 行)
- 诊断面板提取 → `DebugDockPanel.Diagnostics.cs`
- RenderScene 列表提取 → `DebugDockPanel.RenderScene.cs`
- 性能计时提取 → `DebugDockPanel.Performance.cs`
- 主文件仅保留字段 + CacheControls + LogPanel 属性

#### ViewportPlaceholderPanel SRP 提取 (189→46 行)
- 实体摘要/空状态 → `ViewportPlaceholderPanel.Entity.cs`
- Vulkan 状态显示 → `ViewportPlaceholderPanel.Vulkan.cs`
- RenderScene 调试列表 → `ViewportPlaceholderPanel.RenderScene.cs`
- 主文件仅保留字段 + CacheControls + ShowSinglePanel

#### WorldHierarchy 目录拆分 (8→5 文件 ✅)
- 3 视图文件 (NodeView/TreeExpansion/TreeViewState) → `View/` 子目录

#### 白名单清理

| 条目 | 类型 | 之前 | 之后 | 操作 |
|------|------|------|------|------|
| `DebugDockPanel.axaml.cs` | 行 | 145 | 53 | ✅ 删除白名单 |
| `ViewportPlaceholderPanel.axaml.cs` | 行 | 189 | 46 | ✅ 删除白名单 |
| `Panels\WorldHierarchy` | 目录 | 8 文件 | 5+3 | ✅ 删除白名单 |

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` (架构) | ✅ 5/5 |
| 行预算 | 55→53 |
| 行占用 | 37→35 |
| 目录预算 | 7→6 |
| 目录占用 | 4→3 |

---

### 8.7.7F-4B — 中等目录债务清理

清理剩余 3 个目录白名单中的 2 个，全部通过子目录归类完成。

#### Panels/Viewport（11 文件 → 子目录，目录白名单删除 ✅）

| 子目录 | 文件数 | 内容 |
|--------|--------|------|
| `Placeholder/` | 4 | ViewportPlaceholderPanel 全部 4 个 partial 文件 |
| `Summary/` | 3 | ViewportEntitySummary, RenderObjectSummary, RenderSceneSummary |
| `HostInfo/` | 4 | VulkanViewportHostInfo/Panel/State/NativeHostInfo |
| 根目录 | **0** ✅ | 全部移入子目录 |

#### Transform/Gizmo（8 文件 → 子目录，目录白名单删除 ✅）

| 子目录 | 文件数 | 内容 |
|--------|--------|------|
| `Core/` | 3 | MoveGizmoElement, GizmoDist, MoveGizmoVisualState |
| `Layout/` | 2 | MoveGizmoLayout, PresentedMoveGizmoSnapshot |
| `Interaction/` | 3 | MoveGizmoHitTest, MoveGizmoInteraction, MoveGizmoDrawList |
| 根目录 | **0** ✅ | 全部移入子目录 |

#### 白名单清理

| 条目 | 类型 | 之前 | 之后 | 操作 |
|------|------|------|------|------|
| `Panels\Viewport` | 目录 | 11 文件 | 0 (子目录化) | ✅ 删除 |
| `Transform\Gizmo` | 目录 | 8 文件 | 0 (子目录化) | ✅ 删除 |
| `HostInfo\VulkanViewportHostPanel.axaml.cs` | 行路径 | — | 更新为 HostInfo/ 路径 | ✅ |

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` (架构) | ✅ 5/5 |
| 目录预算 | 6→4 |
| 目录占用 | 3→1 |

---

### 8.7.7F-4C — 中等债务第二组清理

**最后 1 个目录白名单 + 2 个文件白名单删除。**

#### ViewportNavigation 目录白名单 ✅（9→0）

| 子目录 | 文件数 | 内容 |
|--------|--------|------|
| `Core/` | 5 | AxisProjection, Action, DragMode, Element, Types |
| `Layout/` | 2 | NavigationLayout, LayoutCompute |
| `Interaction/` | 2 | HitTest, PressResult |

#### VulkanSceneRayBuilder SRP 拆分（167→40 行 ✅）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanSceneRayBuilder.cs` | 40 | TryBuild 射线构建 |
| `Math/VulkanMatrixInvert.cs` | 25 | 矩阵求逆 (TryInvert) |

#### VulkanCameraMatrices SRP 拆分（189→21 行 ✅）

| 文件 | 行数 | 职责 |
|------|------|------|
| `VulkanCameraMatrices.cs` | 21 | MVP/OrthoMVP/Perspective |
| `Math/VulkanViewMatrix.cs` | 19 | LookAt 矩阵 |
| `Math/VulkanMatrixOperations.cs` | 15 | Mul/CreateTranslation/CreateScale |

#### 白名单清理

| 条目 | 类型 | 之前 | 之后 | 操作 |
|------|------|------|------|------|
| `ViewportNavigation` | 目录 | 9 文件 | 子目录化 | ✅ 全局最后一个目录白名单删除 |
| `VulkanCameraMatrices.cs` | 行 | 189 | 21 | ✅ 删除 |
| `VulkanSceneRayBuilder.cs` | 行 | 167 | 40 | ✅ 删除 |

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet build` | ✅ 0 Error |
| `dotnet test` (架构) | ✅ 5/5 |
| 行预算 | 53→51 |
| 行占用 | 35→33 |
| 目录预算 | 4→3 |
| 目录占用 | 1→0 🎯 |

---

### 8.7.7F-5 — 大债务登记到 8.7.8

不做代码改动。产出 `docs/whitelist-debt-roadmap-8.7.8.md`，把剩余 13 个生产文件白名单分成四类：

| 类别 | 文件数 | 说明 |
|------|--------|------|
| A 类 8.7.8 大件 | 6 | >300 行，需要单独阶段拆 |
| B 类 8.7.8 中风险 | 3 | 200～300 行，Vulkan 核心 |
| C 类 F-6 可清 | 4 | ≤200 行，可选在 8.7.7 收掉 |
| D 类 测试保留 | 20 | 继续留在白名单 |

#### 验收

| 指标 | 值 |
|------|-----|
| 文档输出 | ✅ `docs/whitelist-debt-roadmap-8.7.8.md` |
| 不做代码改动 | ✅ |
| 目录白名单 | 保持 0 ✅ |
| 剩余目录白名单 | ViewportNavigation(9) 仅 1 项 |

---

### 8.7.7F-2 — 立即可清白名单删除

删除 3 项 ≤100 行的历史白名单残留，不改代码逻辑。

#### 操作

| 文件 | 行数 | 操作 |
|------|------|------|
| `InspectorPanel.axaml.cs` | 84 | 删除白名单 ✅ |
| `WindowsVulkanViewportHostControl.cs` | 87 | 删除白名单 ✅ |
| `SceneCameraPose.cs` | 99 | 删除白名单 ✅ |

#### 验收

| 指标 | 值 |
|------|-----|
| `dotnet test` (架构) | ✅ 5/5 |
| 行白名单预算 | 67→64 |
| 文件白名单占用 | 49→46 |
