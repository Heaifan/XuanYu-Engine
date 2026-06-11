# 项目文件树 - FluidWarfare

当前阶段：Phase 1

当前版本：0.0.1-dev

创建时间：2026-06-10

最后编辑：2026-06-11 05:45

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
31. Milestone 3.4：新增 `FluidWarfare.Editor.Windows/Panels/Status/StatusBarPanel.axaml`。
32. Milestone 3.4：新增 `FluidWarfare.Editor.Windows/Panels/Status/StatusBarPanel.axaml.cs`。
33. Milestone 3.5：新增 `FluidWarfare.Editor.Windows/Shell/EditorSelection.cs`。
34. Milestone 4.3：新增 `FluidWarfare.Project/Content/GameContentFileInfo.cs`。
35. Milestone 4.3：新增 `FluidWarfare.Project/Content/GameContentFileScanner.cs`。
36. Milestone 4.3：新增 `FluidWarfare.Tests/Project/Content/GameContentFileScannerTests.cs`。
37. Milestone 4.3：新增 `GameProjects/SampleProject/units/sample_unit.json`。
38. Milestone 4.3：新增 `GameProjects/SampleProject/weapons/sample_weapon.json`。
39. Milestone 4.3：新增 `GameProjects/SampleProject/icons/sample_icon.svg`。
40. Milestone 4.3：新增 `FluidWarfare.Editor.Windows/Properties/launchSettings.json`。
41. Milestone 4.3：新增 `run.bat`。
42. Milestone 4.4：新增 `FluidWarfare.Project/Validation/ProjectValidationIssue.cs`。
43. Milestone 4.4：新增 `FluidWarfare.Project/Validation/ProjectValidationReport.cs`。
44. Milestone 4.4：新增 `FluidWarfare.Project/Content/GameContentFileScanResult.cs`。
45. Milestone 4.4：新增 `FluidWarfare.Tests/Project/Validation/ProjectValidationReportTests.cs`。
46. Milestone 5.0：新增 `FluidWarfare.Engine/FluidWarfare.Engine.csproj`。
47. Milestone 5.0：新增 `FluidWarfare.Engine/World/WorldState.cs`。
48. Milestone 5.0：新增 `FluidWarfare.Engine/World/WorldEntityInfo.cs`。
49. Milestone 5.0：新增 `FluidWarfare.Engine/Components/PositionComponent.cs`。
50. Milestone 5.0：新增 `FluidWarfare.Engine/Components/DisplayNameComponent.cs`。
51. Milestone 5.0：新增 `FluidWarfare.Tests/Engine/World/WorldStateTests.cs`。
52. Milestone 5.1：新增 `FluidWarfare.Bridge.ProjectEngine/FluidWarfare.Bridge.ProjectEngine.csproj`。
53. Milestone 5.1：新增 `FluidWarfare.Bridge.ProjectEngine/World/ProjectContentWorldSeeder.cs`。
54. Milestone 5.1：新增 `FluidWarfare.Bridge.ProjectEngine/World/ProjectContentWorldSeedResult.cs`。
55. Milestone 5.1：新增 `FluidWarfare.Engine/World/ProjectContentEntitySource.cs`。
56. Milestone 5.1：新增 `FluidWarfare.Tests/Bridge/ProjectEngine/World/ProjectContentWorldSeederTests.cs`。
57. Milestone 5.2：新增 `FluidWarfare.Editor.Windows/Panels/WorldEntities/WorldEntityListPanel.axaml`。
58. Milestone 5.2：新增 `FluidWarfare.Editor.Windows/Panels/WorldEntities/WorldEntityListPanel.axaml.cs`。
59. Milestone 5.3：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportEntitySummary.cs`。
60. Milestone 6.0：新增 `FluidWarfare.Render/FluidWarfare.Render.csproj`。
61. Milestone 6.0：新增 `FluidWarfare.Render/Scene/RenderObjectVisualKind.cs`。
62. Milestone 6.0：新增 `FluidWarfare.Render/Scene/RenderObjectInfo.cs`。
63. Milestone 6.0：新增 `FluidWarfare.Render/Scene/RenderScene.cs`。
64. Milestone 6.0：新增 `FluidWarfare.Render/World/WorldToRenderSceneBuilder.cs`。
65. Milestone 6.0：新增 `FluidWarfare.Tests/Render/World/WorldToRenderSceneBuilderTests.cs`。
66. Milestone 6.1：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportRenderObjectSummary.cs`。
67. Milestone 6.1：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportRenderSceneSummary.cs`。
68. Milestone 7.0：新增 `FluidWarfare.Render.Vulkan/FluidWarfare.Render.Vulkan.csproj`。
69. Milestone 7.0：新增 `FluidWarfare.Render.Vulkan/Backend/VulkanBackendStatus.cs`。
70. Milestone 7.0：新增 `FluidWarfare.Render.Vulkan/Backend/VulkanBackendInfo.cs`。
71. Milestone 7.0：新增 `FluidWarfare.Render.Vulkan/Backend/VulkanBackendProbe.cs`。
72a. Milestone 7.8：新增 `FluidWarfare.Render.Vulkan/Context/VulkanRenderContext.cs`。
72b. Milestone 7.8.1：新增 `FluidWarfare.Render.Vulkan/Swapchain/VulkanSwapchainStatus.cs`。
72c. Milestone 7.8.1：新增 `FluidWarfare.Render.Vulkan/Swapchain/VulkanSwapchainInfo.cs`。
72d. Milestone 7.8.1：新增 `FluidWarfare.Render.Vulkan/Swapchain/VulkanSwapchainProbe.cs`。
72e. Milestone 7.8.1：新增 `FluidWarfare.Tests/Render/Vulkan/Swapchain/VulkanSwapchainInfoTests.cs`。
72f. Milestone 7.8.2：新增 `FluidWarfare.Render.Vulkan/Clear/VulkanClearStatus.cs`。
72g. Milestone 7.8.2：新增 `FluidWarfare.Render.Vulkan/Clear/VulkanClearInfo.cs`。
72h. Milestone 7.8.2：新增 `FluidWarfare.Render.Vulkan/Clear/VulkanClearProbe.cs`。
72i. Milestone 7.8.2：新增 `FluidWarfare.Tests/Render/Vulkan/Clear/VulkanClearInfoTests.cs`。
72j. Milestone 7.8.3：新增 `FluidWarfare.Editor.Windows/Panels/DebugDock/DebugDockPanel.axaml`。
72k. Milestone 7.8.3：新增 `FluidWarfare.Editor.Windows/Panels/DebugDock/DebugDockPanel.axaml.cs`。
72. Milestone 7.0：新增 `FluidWarfare.Tests/Render/Vulkan/Backend/VulkanBackendInfoTests.cs`。
73. Milestone 7.1：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostPanel.axaml`。
74. Milestone 7.1：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostPanel.axaml.cs`。
75. Milestone 7.1：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostState.cs`。
76. Milestone 7.1：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostInfo.cs`。
77. Milestone 7.2：新增 `FluidWarfare.Editor.Windows/Panels/Project/ProjectContentFolderSelection.cs`。
78. Milestone 7.2：新增 `FluidWarfare.Tests/Project/Loading/SampleProjectSmokeTests.cs`。
79. Milestone 7.2：新增 `FluidWarfare.Tests/Architecture/ProjectDependencyDirectionTests.cs`。
80. Milestone 7.3：新增 `FluidWarfare.Render.Vulkan/Instance/VulkanInstanceStatus.cs`。
81. Milestone 7.3：新增 `FluidWarfare.Render.Vulkan/Instance/VulkanInstanceInfo.cs`。
82. Milestone 7.3：新增 `FluidWarfare.Render.Vulkan/Instance/VulkanInstanceProbe.cs`。
83. Milestone 7.3：新增 `FluidWarfare.Tests/Render/Vulkan/Instance/VulkanInstanceInfoTests.cs`。
84. Milestone 7.4：新增 `FluidWarfare.Render.Vulkan/Device/VulkanDeviceStatus.cs`。
85. Milestone 7.4：新增 `FluidWarfare.Render.Vulkan/Device/VulkanDeviceInfo.cs`。
86. Milestone 7.4：新增 `FluidWarfare.Render.Vulkan/Device/VulkanDeviceProbe.cs`。
87. Milestone 7.4：新增 `FluidWarfare.Tests/Render/Vulkan/Device/VulkanDeviceInfoTests.cs`。
88. Milestone 7.5：新增 `FluidWarfare.Render.Vulkan/Surface/VulkanSurfaceStatus.cs`。
89. Milestone 7.5：新增 `FluidWarfare.Render.Vulkan/Surface/VulkanSurfaceInfo.cs`。
90. Milestone 7.5：新增 `FluidWarfare.Render.Vulkan/Surface/VulkanSurfaceProbe.cs`。
91. Milestone 7.5：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportNativeHostInfo.cs`。
92. Milestone 7.5：新增 `FluidWarfare.Tests/Render/Vulkan/Surface/VulkanSurfaceInfoTests.cs`。
93. Milestone 7.6：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostState.cs`。
94. Milestone 7.6：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostInfo.cs`。
95. Milestone 7.6：新增 `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs`。
96. Milestone 7.6：新增 `FluidWarfare.Editor.Windows/app.manifest`。

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
17. Milestone 2.5.1：确认日志等级前缀统一使用[]。
18. Milestone 2.5.2：统一日志等级前缀符号为[]。
19. Milestone 2.6：新增 EngineLogLevel。
20. Milestone 2.6：新增 EngineLogEntry。
21. Milestone 2.6：新增日志等级与日志记录单元测试。
22. Milestone 2.6.1：修复日志等级前缀统一校验，确认 EngineLogLevel 与 EngineLogEntry 只使用[]。
23. Milestone 3.1：将 FluidWarfare.Editor.Windows 加入 FluidWarfare.sln，并引用 FluidWarfare.Core。
24. Milestone 3.2：实现顶部菜单、项目面板、3D 视口占位、检查器和日志面板。
25. Milestone 3.3：Editor 日志面板接入 EngineLogEntry，启动日志由 Core 日志对象生成。
26. Milestone 3.4：新增 Editor 状态栏面板。
27. Milestone 3.4：整理 Editor 五区 GUI 面板视觉层级。
28. Milestone 3.5：新增顶部菜单占位点击日志反馈。
29. Milestone 3.5：新增项目面板占位项点击事件。
30. Milestone 3.5：检查器面板支持显示项目占位项信息。
31. Milestone 3.5：状态栏支持显示当前选择。
32. Milestone 3.5：明确 ProjectPanel 只负责发出选择事件，不直接写日志或更新其他面板。
33. Milestone 3.6：日志等级前缀统一改为 ASCII 方括号格式。
34. Milestone 3.6：清理中文全角日志括号。
35. Milestone 3.6：补充 Editor GUI 面板 SRP 职责说明。
36. Milestone 3.6：日志面板改为可滚动、可选中、可复制的只读文本区域。
37. Milestone 3.7：新增 3D 视口占位区点击反馈。
38. Milestone 3.7：视口占位区点击后更新检查器、状态栏与日志面板。
39. Milestone 3.7：明确 ViewportPlaceholderPanel 只负责显示占位区并发出视口聚焦事件。
40. Milestone 4.0：新增 FluidWarfare.Project 项目层。
41. Milestone 4.0：新增 GameProjects/SampleProject 示例项目。
42. Milestone 4.0：Editor 启动时加载示例项目。
43. Milestone 4.0：项目面板显示真实项目名与内容分类。
44. Milestone 4.0：新增 GameProjectLoader 单元测试。
45. Milestone 4.1：新增 SampleProjectPath，稳定定位示例项目路径。
46. Milestone 4.1：Editor 启动时通过路径定位结果加载 SampleProject。
47. Milestone 4.1：项目加载失败时更新项目面板、检查器、状态栏与日志。
48. Milestone 4.2：contentFolders 升级为内容目录声明对象数组。
49. Milestone 4.2：新增 GameContentFolderInfo。
50. Milestone 4.2：项目加载器拒绝未声明一级内容目录。
51. Milestone 4.2：SampleProject 新增 icons 扩展目录声明。
52. Milestone 4.2：Editor 根据项目声明显示内容目录。
53. Milestone 4.3：GameProjectInfo 新增 ContentFiles。
54. Milestone 4.3：GameProjectLoader 接入 GameContentFileScanner，拒绝未允许扩展名文件与嵌套内容目录。
55. Milestone 4.3：GameProjectLoaderTests 新增内容文件入口扫描集成测试。
56. Milestone 4.3：EditorShell 接入 ContentFiles，点击内容目录时追加文件入口数量日志。
57. Milestone 4.4：GameProjectLoadResult 新增 ValidationReport。
58. Milestone 4.4：GameContentFileScanner 改为收集多个问题而非中断。
59. Milestone 4.4：GameProjectLoader 汇总目录声明、未声明目录、文件扩展名和嵌套目录问题。
60. Milestone 4.4：GameProjectLoaderTests 新增四个校验报告集成测试，AssertFailure 新增 ValidationReport 校验。
61. Milestone 4.4：GameContentFileScannerTests 新增三个多问题收集测试。
62. Milestone 4.4：EditorShell 加载失败时显示问题数量警告。
63. Milestone 5.0：FluidWarfare.sln 新增 Engine 项目。
64. Milestone 5.0：Editor.csproj 新增 Engine 引用。
65. Milestone 5.0：Tests.csproj 新增 Engine 引用。
66. Milestone 5.0：EditorShell 启动时创建最小 World 与示例实体，点击视口后检查器显示实体信息。
67. Milestone 5.1：WorldEntityInfo 新增 Source 字段。
68. Milestone 5.1：WorldState.CreateEntity 支持 Source 参数。
69. Milestone 5.1：FluidWarfare.sln 新增 Bridge.ProjectEngine 项目。
70. Milestone 5.1：Editor 和 Tests csproj 新增 Bridge 引用。
71. Milestone 5.1：EditorShell 改为从项目内容文件生成 World 占位实体。
72. Milestone 5.2：EditorShell.axaml 左侧导航区拆分为项目内容面板和 World 实体列表面板。
73. Milestone 5.2：EditorShell 接入 WorldEntityListPanel，创建 World 后显示实体列表并响应选择事件。
74. Milestone 5.3：ViewportPlaceholderPanel 重构为三状态布局，支持实体摘要显示。
75. Milestone 5.3：EditorShell 新增 _selectedWorldEntity 状态，选择实体后同步更新视口显示。
76. Milestone 6.0：FluidWarfare.sln 新增 Render 项目。
77. Milestone 6.0：Editor.csproj 和 Tests.csproj 新增 Render 引用。
78. Milestone 6.0：EditorShell 创建 World 后生成 RenderScene 并记录对象数量。
79. Milestone 6.0：ViewportEntitySummary 新增 VisualKindText。
80. Milestone 6.1：ViewportPlaceholderPanel 新增 RenderScene 调试对象区域。
81. Milestone 6.1：EditorShell 新增 CreateViewportRenderSceneSummary。
82. Milestone 7.0：FluidWarfare.sln 新增 Render.Vulkan 项目。
83. Milestone 7.0：Editor.csproj 和 Tests.csproj 新增 Render.Vulkan 引用。
84. Milestone 7.0：EditorShell 启动时探测 Vulkan 后端并输出状态日志。
85. Milestone 7.0：StatusBarPanel 新增 SetVulkanStatus 方法。
86. Milestone 7.0：ViewportPlaceholderPanel 新增 Vulkan 后端状态文本区域。
87. Milestone 7.1：EditorShell.axaml 中央视口区域拆分为 Vulkan 视口宿主面板和调试视口。
88. Milestone 7.1：EditorShell 新增 UpdateVulkanViewportHost 方法。
89. Milestone 7.2：SampleProject 清单新增 schemaVersion。
90. Milestone 7.2：GameProjectLoader 只接受当前项目契约版本，缺失或未知版本返回中文错误。
91. Milestone 7.2：GameProjectInfo 新增 SchemaVersion。
92. Milestone 7.2：ProjectPanel 选择事件改为发出 ProjectContentFolderSelection，不再用 DisplayName 作为查找键。
93. Milestone 7.2：EditorShell 改用 FolderName 查找项目内容目录，DisplayName 只负责 UI 显示。
94. Milestone 7.2：测试新增 SampleProject 冒烟验收与 csproj 依赖方向自动检查。
95. Milestone 7.3：Render.Vulkan 引入 Silk.NET.Vulkan，用于最小 Vulkan API 调用。
96. Milestone 7.3：VulkanInstanceProbe 创建并释放 VkInstance，读取 API 版本、Instance 扩展数量和创建耗时。
97. Milestone 7.3：EditorShell 启动时探测 Vulkan Instance 并输出中文日志。
98. Milestone 7.3：ViewportPlaceholderPanel 新增 Vulkan Instance 状态显示。
99. Milestone 7.3：ProjectDependencyDirectionTests 新增 NuGet 包白名单检查。
100. Milestone 7.4：VulkanDeviceProbe 枚举 PhysicalDevice，选择支持 Graphics Queue 的设备，创建 LogicalDevice 并获取 Graphics Queue。
101. Milestone 7.4：VulkanDeviceProbe 立即释放 LogicalDevice 与 VkInstance。
102. Milestone 7.4：EditorShell 启动时探测 Vulkan Device 并输出显卡名称、设备类型、图形队列族和耗时。
103. Milestone 7.4：ViewportPlaceholderPanel 新增 Vulkan Device 状态显示。
104. Milestone 7.5：VulkanSurfaceProbe 接收 Windows 原生句柄，创建并立即释放 VkSurfaceKHR 与 VkInstance。
105. Milestone 7.5：VulkanViewportHostPanel 新增 Surface 状态显示。
106. Milestone 7.5：EditorShell 在 Device 探测后尝试 Surface 探测；当前未取得独立视口句柄时输出中文警告。
107. Milestone 7.6：VulkanViewportHostPanel 嵌入 Windows 原生视口子窗口宿主。
108. Milestone 7.6：EditorShell 在窗口附加后报告独立 HWND 获取结果。
109. Milestone 7.6：7.6 阶段不调用 VulkanSurfaceProbe，不创建 VkSurfaceKHR。
110. Milestone 7.7：EditorShell 移除 7.6 占位逻辑，真正调用 VulkanSurfaceProbe.ProbeWindows 创建 VkSurfaceKHR。
111. Milestone 7.8.2：EditorShell 新增 ProbeVulkanClear 与 ShowVulkanClearInfo。
112. Milestone 7.8.3：EditorShell.axaml 底部替换为 DebugDockPanel。
113. Milestone 7.8.3：EditorShell 新增 UpdateAllDiagnostics，集中管理调试信息。
114. Milestone 7.8.3：VulkanViewportHostPanel 收束为纯视口，移除诊断文本。

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

当前执行 Milestone 7.8.3：底部调试终端与主视口收束。

本轮只让 Editor 中的 VulkanViewportHostPanel 通过 Avalonia NativeControlHost 创建独立 Windows 原生子窗口 HWND，并显示 HWND / HINSTANCE 获取状态。不创建 Vulkan Surface、Swapchain、RenderPass、Framebuffer、CommandBuffer，不做真实清屏，不做 Runtime，不做 Android。

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
|-- FluidWarfare.Project/
|   |-- FluidWarfare.Project.csproj
|   |-- Content/
|   |   |-- GameContentFileInfo.cs
|   |   |-- GameContentFileScanResult.cs
|   |   |-- GameContentFileScanner.cs
|   |   `-- GameContentFolderInfo.cs
|   |-- Loading/
|   |   |-- GameProjectLoader.cs
|   |   `-- GameProjectLoadResult.cs
|   |-- Metadata/
|   |   `-- GameProjectInfo.cs
|   |-- Paths/
|   |   `-- SampleProjectPath.cs
|   `-- Validation/
|       |-- ProjectValidationIssue.cs
|       `-- ProjectValidationReport.cs
|-- FluidWarfare.Bridge.ProjectEngine/
|   |-- FluidWarfare.Bridge.ProjectEngine.csproj
|   `-- World/
|       |-- ProjectContentWorldSeedResult.cs
|       `-- ProjectContentWorldSeeder.cs
|-- FluidWarfare.Engine/
|   |-- FluidWarfare.Engine.csproj
|   |-- Components/
|   |   |-- DisplayNameComponent.cs
|   |   `-- PositionComponent.cs
|   `-- World/
|       |-- ProjectContentEntitySource.cs
|       |-- WorldEntityInfo.cs
|       `-- WorldState.cs
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
|   |-- FluidWarfare.Render.csproj
|   |-- Scene/
|   |   |-- RenderObjectInfo.cs
|   |   |-- RenderObjectVisualKind.cs
|   |   `-- RenderScene.cs
|   `-- World/
|       `-- WorldToRenderSceneBuilder.cs
|-- FluidWarfare.Render.Vulkan/
|   |-- FluidWarfare.Render.Vulkan.csproj
|   |-- Backend/
|   |   |-- VulkanBackendInfo.cs
|   |   |-- VulkanBackendProbe.cs
|   |   `-- VulkanBackendStatus.cs
|   |-- Device/
|   |   |-- VulkanDeviceInfo.cs
|   |   |-- VulkanDeviceProbe.cs
|   |   `-- VulkanDeviceStatus.cs
|   |-- Instance/
|       |-- VulkanInstanceInfo.cs
|       |-- VulkanInstanceProbe.cs
|       `-- VulkanInstanceStatus.cs
|   `-- Surface/
|       |-- VulkanSurfaceInfo.cs
|       |-- VulkanSurfaceProbe.cs
|       `-- VulkanSurfaceStatus.cs
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
|   |-- app.manifest
|   |-- Properties/
|   |   `-- launchSettings.json
|   |-- Panels/
|   |   |-- Inspector/
|   |   |   |-- InspectorPanel.axaml
|   |   |   `-- InspectorPanel.axaml.cs
|   |   |-- Logging/
|   |   |   |-- LogPanel.axaml
|   |   |   `-- LogPanel.axaml.cs
|   |   |-- Project/
|   |   |   |-- ProjectPanel.axaml
|   |   |   |-- ProjectContentFolderSelection.cs
|   |   |   `-- ProjectPanel.axaml.cs
|   |   |-- Status/
|   |   |   |-- StatusBarPanel.axaml
|   |   |   `-- StatusBarPanel.axaml.cs
|   |   `-- Viewport/
|   |       |-- ViewportEntitySummary.cs
|   |       |-- ViewportPlaceholderPanel.axaml
|   |       |-- ViewportPlaceholderPanel.axaml.cs
|   |       |-- ViewportRenderObjectSummary.cs
|   |       |-- ViewportRenderSceneSummary.cs
|   |       |-- NativeHost/
|   |       |   |-- WindowsVulkanViewportHostControl.cs
|   |       |   |-- WindowsVulkanViewportHostInfo.cs
|   |       |   `-- WindowsVulkanViewportHostState.cs
|   |       |-- VulkanViewportHostInfo.cs
|   |       |-- VulkanViewportNativeHostInfo.cs
|   |       |-- VulkanViewportHostPanel.axaml
|   |       |-- VulkanViewportHostPanel.axaml.cs
|   |       |-- VulkanViewportHostState.cs
|   |   `-- WorldEntities/
|   |       |-- WorldEntityListPanel.axaml
|   |       `-- WorldEntityListPanel.axaml.cs
|   `-- Shell/
|       |-- EditorShell.axaml
|       |-- EditorShell.axaml.cs
|       `-- EditorSelection.cs
|-- FluidWarfare.Exporter/
|   `-- .gitkeep
|-- FluidWarfare.Tests/
|   |-- .gitkeep
|   |-- Architecture/
|   |   `-- ProjectDependencyDirectionTests.cs
|   |-- Bridge/
|   |-- Render/
|   |   |-- Vulkan/
|   |   |   |-- Backend/
|   |   |   |   `-- VulkanBackendInfoTests.cs
|   |   |   |-- Device/
|   |   |   |   `-- VulkanDeviceInfoTests.cs
|   |   |   |-- Instance/
|   |   |       `-- VulkanInstanceInfoTests.cs
|   |   |   `-- Surface/
|   |   |       `-- VulkanSurfaceInfoTests.cs
|   |   `-- World/
|   |       `-- WorldToRenderSceneBuilderTests.cs
|   |   `-- ProjectEngine/
|   |       `-- World/
|   |           `-- ProjectContentWorldSeederTests.cs
|   |-- Core/
|   |-- Engine/
|   |   `-- World/
|   |       `-- WorldStateTests.cs
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
|   |-- Project/
|   |   |-- Content/
|   |   |   `-- GameContentFileScannerTests.cs
|   |   |-- Loading/
|   |   |   |-- GameProjectLoaderTests.cs
|   |   |   `-- SampleProjectSmokeTests.cs
|   |   |-- Paths/
|   |   |   `-- SampleProjectPathTests.cs
|   |   `-- Validation/
|   |       `-- ProjectValidationReportTests.cs
|   |-- CoreSmokeTests.cs
|   `-- FluidWarfare.Tests.csproj
|-- GameProjects/
|   `-- SampleProject/
|       |-- game.project.json
|       |-- factions/
|       |   `-- .gitkeep
|       |-- units/
|       |   |-- .gitkeep
|       |   `-- sample_unit.json
|       |-- weapons/
|       |   |-- .gitkeep
|       |   `-- sample_weapon.json
|       |-- maps/
|       |   `-- .gitkeep
|       |-- scenarios/
|       |   `-- .gitkeep
|       |-- rules/
|       |   `-- .gitkeep
|       `-- icons/
|           |-- .gitkeep
|           `-- sample_icon.svg
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
|-- file-tree.md
`-- run.bat
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
| FluidWarfare.Project | 游戏项目元数据与最小项目加载层，负责读取 game.project.json，不依赖 Editor 或 Avalonia | 测试通过 |
| FluidWarfare.Bridge.ProjectEngine | Project 层与 Engine 层的桥接模块，用于把项目内容入口转换为 Engine World 占位实体 | 测试通过 |
| FluidWarfare.Engine | 引擎运行层，负责 World、实体、组件与模拟状态。当前仅实现最小 World 实体 | 测试通过 |
| FluidWarfare.Ecs | ECS-lite 实体、组件、系统和查询 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.World | 地面、边界、相机出生点和空间场景数据 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Simulation | 固定 Tick、暂停、单步和模拟世界 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Combat | 未来的接敌、士气、伤亡和战斗日志 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.AI | 未来的战术 AI、编队 AI 和战略 AI | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Data | 场景 JSON 与资源数据读取 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Render | 抽象渲染层，负责渲染场景数据结构与 World 到 RenderScene 的转换，不依赖具体渲染后端 | 测试通过 |
| FluidWarfare.Render.Vulkan | Vulkan 具体渲染后端模块。当前实现 Vulkan Loader 探测、VkInstance 最小创建释放、PhysicalDevice / LogicalDevice 最小选择释放与 Editor 状态显示 | 测试通过 |
| FluidWarfare.Runtime.Windows | Windows 游戏运行时 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Runtime.Android | Android 游戏运行时 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Editor.Windows | Windows 桌面编辑器，使用 Avalonia 构建 GUI，启动时加载示例项目，只用于开发、调试和导出，不进入 Android Runtime | 可运行 |
| FluidWarfare.Exporter | Windows 与 Android 导出流程 | 已创建 / 仅 `.gitkeep` |
| FluidWarfare.Tests | 单元测试和聚焦集成测试 | 测试通过 |

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
| `FluidWarfare.Core/Results/EngineError.cs` | 引擎错误值对象，承载稳定英文错误代码与中文可读错误信息，不包含[报错]等日志等级前缀 | 测试通过 |
| `FluidWarfare.Core/Results/EngineResult.cs` | 引擎操作结果值对象，统一表达成功或失败，要求失败结果携带有效 EngineError，并明确默认值无效 | 测试通过 |
| `FluidWarfare.Core/Logging/EngineLogLevel.cs` | 引擎日志等级枚举与中文等级标签映射，统一[追踪][信息][警告][报错][严重]显示前缀 | 测试通过 |
| `FluidWarfare.Core/Logging/EngineLogEntry.cs` | 引擎日志记录值对象，保存模拟时间、日志等级、分类和中文日志内容，并提供基础中文显示输出 | 测试通过 |
| `FluidWarfare.Project/FluidWarfare.Project.csproj` | Project 项目层项目文件，引用 Core，不引用 Editor 或 Avalonia | 测试通过 |
| `FluidWarfare.Project/Content/GameContentFileInfo.cs` | 项目内容文件入口模型，保存所属目录、内容类型、文件名、相对路径和扩展名，不读取文件内容 | 测试通过 |
| `FluidWarfare.Project/Content/GameContentFileScanResult.cs` | 内容文件扫描结果模型，保存合法内容文件入口和扫描中的校验问题 | 测试通过 |
| `FluidWarfare.Project/Content/GameContentFileScanner.cs` | 扫描已声明内容目录中的一级内容文件，返回合法文件入口并收集文件级校验问题 | 测试通过 |
| `FluidWarfare.Project/Content/GameContentFolderInfo.cs` | 项目内容目录声明模型，保存目录名、显示名、说明、内容类型、是否必需与允许扩展名 | 测试通过 |
| `FluidWarfare.Project/Metadata/GameProjectInfo.cs` | 游戏项目元数据模型，保存项目契约版本、项目编号、显示名称、说明、内容目录声明列表和合法内容文件入口列表 | 测试通过 |
| `FluidWarfare.Project/Loading/GameProjectLoader.cs` | 从项目目录读取 game.project.json，校验项目契约版本、项目元数据与内容目录声明，协调内容文件入口扫描，拒绝未声明一级内容目录、未允许扩展名文件与嵌套内容目录 | 测试通过 |
| `FluidWarfare.Project/Loading/GameProjectLoadResult.cs` | 项目加载结果模型，组合 EngineResult、可选 GameProjectInfo 与项目校验报告 | 测试通过 |
| `FluidWarfare.Project/Validation/ProjectValidationIssue.cs` | 项目校验问题模型，保存错误码、中文信息和问题路径，不读取文件不写日志 | 测试通过 |
| `FluidWarfare.Project/Validation/ProjectValidationReport.cs` | 项目校验报告模型，汇总项目加载与内容扫描中的校验问题，支持空报告 | 测试通过 |
| `FluidWarfare.Engine/FluidWarfare.Engine.csproj` | Engine 引擎层项目文件，引用 Core | 测试通过 |
| `FluidWarfare.Engine/World/ProjectContentEntitySource.cs` | 保存 World 实体的项目内容来源路径与内容类型，不读取文件不解析 JSON | 测试通过 |
| `FluidWarfare.Engine/World/WorldState.cs` | 最小世界状态，支持创建、查询和枚举带显示名、位置与可选来源的实体 | 测试通过 |
| `FluidWarfare.Engine/World/WorldEntityInfo.cs` | World 实体显示信息模型，保存 EntityId、显示名与可选项目内容来源 | 测试通过 |
| `FluidWarfare.Bridge.ProjectEngine/FluidWarfare.Bridge.ProjectEngine.csproj` | Bridge 桥接层项目文件，依赖 Core + Engine + Project | 测试通过 |
| `FluidWarfare.Bridge.ProjectEngine/World/ProjectContentWorldSeeder.cs` | 根据 GameContentFileInfo 中的 unitTemplate 文件入口，在 WorldState 中创建占位实体 | 测试通过 |
| `FluidWarfare.Bridge.ProjectEngine/World/ProjectContentWorldSeedResult.cs` | 保存项目内容生成 World 占位实体的结果，包括创建数量和来源路径 | 测试通过 |
| `FluidWarfare.Tests/Bridge/ProjectEngine/World/ProjectContentWorldSeederTests.cs` | 验证 Seeder 的 unitTemplate 过滤、命名、稳定排序和多文件创建 | 测试通过 |
| `FluidWarfare.Render/FluidWarfare.Render.csproj` | Render 抽象渲染层项目文件，依赖 Core + Engine | 测试通过 |
| `FluidWarfare.Render/Scene/RenderScene.cs` | 最小渲染场景模型，保存可渲染对象列表 | 测试通过 |
| `FluidWarfare.Render/Scene/RenderObjectInfo.cs` | 渲染对象信息模型，保存 EntityId、显示名、位置、视觉类型与来源路径 | 测试通过 |
| `FluidWarfare.Render/Scene/RenderObjectVisualKind.cs` | 渲染对象视觉类型枚举，当前仅包含 UnitMarker | 测试通过 |
| `FluidWarfare.Render/World/WorldToRenderSceneBuilder.cs` | 将 Engine.WorldState 转换为 RenderScene | 测试通过 |
| `FluidWarfare.Tests/Render/World/WorldToRenderSceneBuilderTests.cs` | 验证 World 到 RenderScene 的最小转换逻辑 | 测试通过 |
| `FluidWarfare.Render.Vulkan/FluidWarfare.Render.Vulkan.csproj` | Vulkan 后端项目文件，依赖 Core、Render 与 Silk.NET.Vulkan | 测试通过 |
| `FluidWarfare.Render.Vulkan/Instance/VulkanInstanceStatus.cs` | Vulkan Instance 创建探测状态枚举 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Instance/VulkanInstanceInfo.cs` | Vulkan Instance 创建探测结果模型，保存状态、中文说明、API 版本、扩展数量与耗时 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Instance/VulkanInstanceProbe.cs` | 创建并释放 Vulkan Instance，读取 API 版本与扩展数量，不创建 Device 或 Surface | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Instance/VulkanInstanceInfoTests.cs` | 验证 Vulkan Instance 探测结果模型的基础语义与轻量 Probe 输出 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Device/VulkanDeviceStatus.cs` | Vulkan Device 创建探测状态枚举 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Device/VulkanDeviceInfo.cs` | Vulkan Device 创建探测结果模型，保存状态、中文说明、显卡名称、设备类型、图形队列族索引与耗时 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Device/VulkanDeviceProbe.cs` | 枚举 PhysicalDevice，选择支持 Graphics Queue 的设备，创建并释放 LogicalDevice，不创建 Surface 或 Swapchain | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Device/VulkanDeviceInfoTests.cs` | 验证 Vulkan Device 探测结果模型的基础语义与轻量 Probe 输出 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Surface/VulkanSurfaceStatus.cs` | Vulkan Surface 创建探测状态枚举 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Surface/VulkanSurfaceInfo.cs` | Vulkan Surface 创建探测结果模型，保存状态、中文说明、平台、原生句柄状态与耗时 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Surface/VulkanSurfaceProbe.cs` | 接收 Windows 原生句柄，创建并立即释放 VkSurfaceKHR 与 VkInstance，不创建 Device 或 Swapchain | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Surface/VulkanSurfaceInfoTests.cs` | 验证 Vulkan Surface 探测结果模型的基础语义，不创建真实 Surface | 测试通过 |
| `FluidWarfare.Engine/Components/PositionComponent.cs` | 实体位置组件，包装 Vector3d | 测试通过 |
| `FluidWarfare.Engine/Components/DisplayNameComponent.cs` | 实体显示名组件，保存用于 Editor 显示的名称 | 测试通过 |
| `FluidWarfare.Tests/Engine/World/WorldStateTests.cs` | 验证最小 World 实体创建、查询、位置读取与枚举 | 测试通过 |
| `FluidWarfare.Project/Paths/SampleProjectPath.cs` | 从指定起始目录向上查找 GameProjects/SampleProject/game.project.json，用于稳定定位示例项目路径 | 测试通过 |
| `FluidWarfare.Tests/FluidWarfare.Tests.csproj` | xUnit 测试项目，引用 Core 与 Project | 已创建 |
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
| `FluidWarfare.Tests/Project/Content/GameContentFileScannerTests.cs` | 验证内容文件入口扫描，包括合法扩展名、非法扩展名、.gitkeep、嵌套目录、大/小写扩展名、空 allowedExtensions、多目录、隐藏文件和多问题收集 | 测试通过 |
| `FluidWarfare.Tests/Project/Loading/GameProjectLoaderTests.cs` | 验证最小项目加载器的有效项目、缺失目录、缺失清单、无效 JSON、必要字段缺失、内容目录声明校验、未声明目录拒绝、内容文件入口扫描集成、嵌套目录拒绝和校验报告多问题收集 | 测试通过 |
| `FluidWarfare.Tests/Project/Loading/SampleProjectSmokeTests.cs` | 验证仓库内 SampleProject 可加载，且内容目录、内容文件入口与校验报告正常 | 测试通过 |
| `FluidWarfare.Tests/Architecture/ProjectDependencyDirectionTests.cs` | 自动检查项目依赖方向和 NuGet 包白名单，防止 Project、Engine、Bridge、Render、Render.Vulkan 与 Tests 出现反向依赖或越界包引用 | 测试通过 |
| `FluidWarfare.Tests/Project/Validation/ProjectValidationReportTests.cs` | 验证空报告、问题数量和首个问题 | 测试通过 |
| `FluidWarfare.Tests/Project/Paths/SampleProjectPathTests.cs` | 验证示例项目路径定位逻辑，包括根目录、嵌套目录、缺失项目与空起始目录 | 测试通过 |
| `FluidWarfare.Editor.Windows/FluidWarfare.Editor.Windows.csproj` | Windows Editor Avalonia 项目文件，引用 Core、Project、Engine、Bridge、Render、Render.Vulkan，并声明 Avalonia 桌面依赖与应用 manifest | 测试通过 |
| `FluidWarfare.Editor.Windows/app.manifest` | Editor Windows 应用 manifest，声明 Windows 10 兼容性，支持 Avalonia NativeControlHost 创建原生子窗口 | 测试通过 |
| `FluidWarfare.Editor.Windows/Program.cs` | Editor 进程入口，配置 Avalonia 桌面生命周期 | 可运行 |
| `FluidWarfare.Editor.Windows/App.axaml` | Editor 应用 XAML 根对象，加载 Fluent 主题 | 可运行 |
| `FluidWarfare.Editor.Windows/App.axaml.cs` | Editor 应用启动逻辑，创建主窗口 | 可运行 |
| `FluidWarfare.Editor.Windows/MainWindow.axaml` | 编辑器主窗口 XAML 容器，承载 EditorShell | 可运行 |
| `FluidWarfare.Editor.Windows/MainWindow.axaml.cs` | 编辑器主窗口 code-behind | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml` | 编辑器布局壳，组织菜单栏、项目内容面板、World 实体列表面板、视口占位、检查器、日志面板与状态栏 | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs` | 编辑器布局壳后台逻辑，协调项目加载、World 创建、RenderScene 生成、Vulkan 后端/Instance/Device 探测、Windows 原生视口宿主状态、实体选择与 UI 显示 | 测试通过 |
| `FluidWarfare.Editor.Windows/Shell/EditorSelection.cs` | 编辑器 GUI 占位选择信息值对象，用于在项目面板、检查器和状态栏之间传递当前选择 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Project/ProjectPanel.axaml` | 编辑器项目面板 UI，显示当前示例项目名称与外部传入的项目分类 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Project/ProjectContentFolderSelection.cs` | 项目内容目录选择值对象，保存稳定 FolderName、DisplayName 和 ContentKind | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Project/ProjectPanel.axaml.cs` | 项目面板后台逻辑，只负责显示项目名、显示内容目录，并在点击时发出 ProjectContentFolderSelection | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportEntitySummary.cs` | 视口占位显示模型，保存当前选中实体的名称、EntityId、来源路径、位置文本与视觉类型 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportRenderObjectSummary.cs` | 视口 RenderScene 调试列表中的单个渲染对象显示摘要 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportRenderSceneSummary.cs` | 视口 RenderScene 调试列表显示模型，保存多个渲染对象摘要 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportPlaceholderPanel.axaml` | 3D 视口占位界面，显示默认提示、World 为空提示、当前选中实体摘要、Vulkan 后端状态、Vulkan Instance 状态、Vulkan Device 状态与 RenderScene 调试对象列表 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportPlaceholderPanel.axaml.cs` | 视口占位面板逻辑，显示 Vulkan 后端状态、Vulkan Instance 状态、Vulkan Device 状态、实体摘要与 RenderScene 调试对象 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostState.cs` | Vulkan 视口宿主占位状态枚举 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostInfo.cs` | Vulkan 视口宿主占位显示信息 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportNativeHostInfo.cs` | Vulkan 视口宿主原生窗口句柄信息，描述平台、句柄可用性与中文说明 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostState.cs` | Windows Vulkan 视口子窗口宿主状态枚举 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostInfo.cs` | Windows Vulkan 视口子窗口宿主信息模型，保存状态、平台、HWND、HINSTANCE 与中文说明 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs` | 使用 Avalonia NativeControlHost 创建并持有 Windows 原生子窗口 HWND，作为未来 Vulkan Surface 宿主 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostPanel.axaml` | Vulkan 视口宿主显示区域，包含宿主状态、Windows 原生子窗口状态与 Surface 状态 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostPanel.axaml.cs` | 显示 Vulkan 视口宿主状态，查询 Windows 原生子窗口句柄，并显示 Surface 状态 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/WorldEntities/WorldEntityListPanel.axaml` | World 实体列表面板 UI，显示当前 World 实体列表 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/WorldEntities/WorldEntityListPanel.axaml.cs` | World 实体列表面板后台逻辑，接收 WorldEntityInfo 列表并在点击时发出 EntitySelected 事件 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorPanel.axaml` | 检查器面板占位，显示未选择对象 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorPanel.axaml.cs` | 检查器面板 code-behind | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Logging/LogPanel.axaml` | 编辑器日志面板 UI，使用只读文本区域显示中文日志，支持滚动查看与复制 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Logging/LogPanel.axaml.cs` | 编辑器日志面板后台逻辑，只负责设置、追加和刷新外部传入的日志文本，不创建 EngineLogEntry | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Status/StatusBarPanel.axaml` | 编辑器底部状态栏 UI，显示当前状态、阶段、Core 加载状态与 Vulkan 接入状态 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Status/StatusBarPanel.axaml.cs` | 编辑器底部状态栏后台逻辑，提供静态状态显示初始化并显示当前选择 | 可运行 |
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
| `GameProjects/SampleProject/game.project.json` | FluidWarfare 示例项目清单，声明 schemaVersion 与内容目录，用于验证最小项目系统与内容文件入口扫描 | 可加载 |
| `GameProjects/SampleProject/units/sample_unit.json` | SampleProject 单位内容入口占位文件，仅用于验证内容文件扫描，不代表正式单位 schema | 占位 |
| `GameProjects/SampleProject/weapons/sample_weapon.json` | SampleProject 武器内容入口占位文件，仅用于验证内容文件扫描，不代表正式武器 schema | 占位 |
| `GameProjects/SampleProject/icons/sample_icon.svg` | SampleProject 图标内容入口占位文件，仅用于验证内容文件扫描，不代表正式图标加载 | 占位 |
| `GameProjects/SampleProject/icons/.gitkeep` | SampleProject 图标扩展目录占位文件，用于验证项目自定义内容目录声明 | 可加载 |
| `*/.gitkeep` | 保留当前尚未写入代码或资源的目录 | 已创建 |

## 6. 模块依赖方向

Core 是基础层。

Engine 位于 Core 之上，负责 World、实体与组件状态。

Project 位于 Core 之上，负责项目元数据、内容目录声明、示例项目路径定位与最小项目加载。

ECS、World、Simulation、Data、Combat、AI 和 Render 抽象可以向内依赖 Core。

Runtime、Editor、Exporter 和具体 Vulkan 代码属于外层。

外层模块不得反向污染内层模块。

Vulkan 依赖只能进入 `FluidWarfare.Render.Vulkan`。

Avalonia 依赖只能进入 `FluidWarfare.Editor.Windows`。

依赖方向：

```text
Core  ←  Engine  ←  Editor
Core  ←  Project ←  Editor
Engine    不依赖   Project
Engine    不依赖   Editor
Project   不依赖   Engine
Project   不依赖   Editor
```

## 7. 文件命名与目录纪律

C# 代码使用 `FluidWarfare.*` 命名空间。

不得使用以下旧命名或泛化命名：

```text
BingWuChangShiEngine
Bwc.*
cls_
fuc_
泛化工具命名
泛化辅助命名
泛化总管命名
泛化处理器命名
编号拆分文件名
```

数据与资源文件可以使用领域前缀，例如 `scn_`、`cfg_`、`dat_`、`mesh_`、`tex_`、`mat_` 和 `shd_`。

## 8. 日志前缀规则

日志等级前缀统一使用：

```text
[追踪]
[信息]
[警告]
[报错]
[严重]
```

禁止使用中文全角日志括号和旧式日志括号。

## 9. Editor GUI SRP 规则

ProjectPanel：
只负责显示项目名称和项目内容目录显示名，并发出 ProjectItemSelected 事件。
不得读取路径。
不得扫描目录。
不得读取 game.project.json。
不得创建 EngineLogEntry。
不得调用 LogPanel。
不得调用 InspectorPanel。
不得调用 StatusBarPanel。

InspectorPanel：
只负责显示传入的 EditorSelection。
不得监听 ProjectPanel。
不得创建日志。
不得更新状态栏。

LogPanel：
只负责显示和追加外部传入的日志文本。
负责提供滚动查看能力。
负责提供文本选中与复制能力。
不得创建 EngineLogEntry。
不得判断菜单或项目点击来源。

ViewportPlaceholderPanel：
只负责显示 3D 视口占位区。
只负责发出 ViewportFocused 事件。
不得创建 EngineLogEntry。
不得调用 LogPanel。
不得调用 InspectorPanel。
不得调用 StatusBarPanel。
不得实现 Vulkan、真实 3D 渲染、摄像机或鼠标拖拽。

StatusBarPanel：
只负责显示状态文本和当前选择文本。
不得判断项目项含义。
不得创建日志。

EditorShell：
当前阶段允许作为轻量协调层，负责接收菜单与项目项事件，并调用日志、检查器和状态栏面板。
当前阶段允许协调 SampleProjectPath、GameProjectLoader、ProjectPanel、InspectorPanel、StatusBarPanel 和 LogPanel。
当前阶段允许将 GameContentFolderInfo 转换为 ProjectPanel 显示项，并根据 GameContentFolderInfo 更新检查器、状态栏与日志。
当前阶段允许读取 GameProjectInfo.ContentFiles 做轻量文件入口数量日志反馈。
当前阶段允许读取 ValidationReport 显示首个错误和问题数量。
不得承载真实项目系统。
不得硬编码英文目录到中文显示名的映射。
不得解析内容文件。
不得显示资源预览。
不得实现资源管理器。
不得负责项目校验。
不得承载 ECS。
不得承载 Vulkan。
不得变成长期业务总管。

## 10. 当前不做的内容

当前已经进入 Milestone 5.1 从项目内容生成占位实体任务。

本轮不做以下内容：

1. 完整 ECS 调度系统。
2. Query / Archetype / Chunk。
3. 并行调度。
4. 事件系统。
5. 序列化。
6. 从 sample_unit.json 生成实体。
7. 单位 JSON 业务字段解析。
2. 武器 JSON 业务字段解析。
3. 地图 JSON 业务字段解析。
4. 剧本 JSON 业务字段解析。
5. 规则 JSON 业务字段解析。
6. SVG / PNG / WEBP 内容解析。
7. 图片加载。
8. 图标预览。
9. 资源浏览器完整 UI。
10. 内容计数作为主线。
11. 内容数据库。
12. SQLite。
13. ECS 实现。
14. Entity 实现。
15. Component 实现。
16. World 实现。
17. Runtime.Windows 实现。
18. Android 实现。
19. Vulkan 接入。
20. 项目创建向导。
21. 文件选择器。
22. 脚本执行。
23. 第三方 JSON 包。
24. 第三方日志库。
25. 图片解析库。
26. SVG 解析库。

## 11. 版本历史索引

详见 `docs/CHANGELOG.md`。
