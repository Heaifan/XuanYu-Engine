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

### 删除

1. 删除由 .NET SDK 默认模板临时生成的 `FluidWarfare.slnx`。
