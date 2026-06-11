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

下一阶段进入 **Milestone 8.2：多对象 3D 绘制与基础深度测试**。
