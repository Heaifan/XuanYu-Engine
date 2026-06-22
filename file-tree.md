# 项目文件树 - FluidWarfare

当前阶段：Phase 1

当前版本：0.0.1-dev

创建时间：2026-06-10

最后编辑：2026-06-23 23:00

本文档用于记录 FluidWarfare 项目目录结构、模块职责、关键文件职责、未发布变更和模块依赖方向。

每次新增、删除、重命名或移动文件与目录时，都必须同步更新本文档。

## 1. 未发布变更日志

最后编辑：2026-06-23 23:00（8.7.7E-2C-R + E-2D + F-1）

### 新增（Milestone 8.7.7F：全仓白名单债务清算）

1. **8.7.7F-1**：创建 `docs/whitelist-audit-8.7.7F-1.md` — 全仓白名单债务盘点，49 项文件 + 8 项目录白名单分层审计

### 新增（Milestone 8.7.6 — 8.7.7：EditorShell SRP 重构 + 面板拆分）

以下变更覆盖 Milestone 8.7.6 至 8.7.7 的 SRP（单一职责原则）重构，总计 62 个提交，将 EditorShell.axaml.cs 从 3,041 行降至 970 行，并对多个面板进行了职责拆分。

**EditorShell 提取 Route 类（8.7.6.1 — 8.7.6.8E）：**
1. 8.7.6.1：创建 `FluidWarfare.Editor.Windows/Shell/Scene3D/Frame/Scene3dFrameRoute.cs`、`Scene3dFrameState.cs`、`Scene3dDrawListBuilder.cs`、`Scene3dPresentedState.cs`— 提取 Scene3D 帧路径。
2. 8.7.6.2：创建 `FluidWarfare.Editor.Windows/Viewport/Picking/ViewportPointerPickRoute.cs`、`ViewportPickRequest.cs`、`ViewportPickResult.cs`、`ViewportPickFailure.cs`、`ViewportPickTrace.cs`— 提取视口指针拾取路由。
3. 8.7.6.3A：创建 `FluidWarfare.Editor.Windows/Viewport/Transform/Interaction/TransformInteractionState.cs`、`TransformInteractionResult.cs`、`TransformPointerRoute.cs`、`TransformKeyboardRoute.cs`、`TransformStartRequest.cs`— 提取变换交互。
4. 8.7.6.3B：创建 `FluidWarfare.Editor.Windows/Viewport/Transform/Application/EntityTransformPreview.cs`、`EntityTransformCommit.cs`、`EntityTransformCancel.cs`、`TransformApplyResult.cs`— 提取变换应用层。
5. 8.7.6.3C：创建 `FluidWarfare.Editor.Windows/Viewport/Transform/Gizmo/MoveGizmoDrawList.cs`、`MoveGizmoElement.cs`、`MoveGizmoHitTest.cs`、`MoveGizmoInteraction.cs`、`MoveGizmoLayout.cs`、`MoveGizmoVisualState.cs`、`PresentedMoveGizmoSnapshot.cs` 等 — 提取 Gizmo 呈现。
6. 8.7.6.4A：创建 `Scene3dFrameSubmitRoute.cs`、`Scene3dFrameSubmitInput.cs`、`Scene3dPickSnapshotSource.cs`、`Scene3dGizmoSubmitSource.cs`— 提取帧提交。
7. 8.7.6.4B：创建 `Scene3dSessionLifecycle.cs`、`Scene3dSessionState.cs`、`Scene3dSessionStartRequest.cs`、`Scene3dSessionStartResult.cs`、`Scene3dSessionRestartReason.cs`— 提取 Session 生命周期。
8. 8.7.6.4C：创建 `VulkanViewportProbeRoute.cs`、`VulkanViewportProbeState.cs`、`VulkanViewportProbeResult.cs`、`Scene3dDiagnosticSnapshot.cs`、`Scene3dDiagnosticText.cs`— 提取 Diagnostics。
9. 8.7.6.5A：创建 `ViewportSelectionPresenter.cs`、`EditorSelectionPresenter.cs`、`WorldEntitySelectionPresenter.cs`、`ProjectContentSelectionPresenter.cs`、`SelectionPresentationResult.cs`— 提取选择呈现。
10. 8.7.6.5B：创建 `EditorSelectionRoute.cs`、`EditorSelectionRequest.cs`、`EditorSelectionState.cs`、`EditorSelectionRouteResult.cs`、`EditorSelectionReason.cs`— 提取选择路由与状态。
11. 8.7.6.5C：创建 `FluidWarfare.Tests/Architecture/CodeFileBudgetTests.cs`— 架构扫描测试（代码宪法自动化）。
12. 8.7.6.6A：创建 `ProjectBootstrapRoute.cs`、`ProjectBootstrapResult.cs`— 提取项目引导。
13. 8.7.6.6B：创建 `WorldBootstrapRoute.cs`、`WorldBootstrapInput.cs`、`WorldBootstrapResult.cs`、`WorldBootstrapEntitySeed.cs`、`WorldBootstrapRenderSeed.cs`— 提取世界引导。
14. 8.7.6.7A：创建 `ViewportCameraRoute.cs`、`ViewportCameraCommand.cs`、`ViewportCameraResult.cs`、`ViewportCameraFocusTarget.cs`— 提取相机路由。
15. 8.7.6.7B：创建 `ViewportNavigationRoute.cs`、`ViewportNavigationResponse.cs`— 提取导航路由。
16. 8.7.6.8A：创建 `EditorRunMenuRoute.cs`— 提取运行菜单。创建 `EditorDiagnosticsRefreshRoute.cs`、`EditorDiagnosticsRefreshRequest.cs`、`EditorDiagnosticsRefreshResult.cs`、`EditorDiagnosticsRefreshState.cs`、`EditorDiagnosticsRefreshKind.cs`— 提取 Diagnostics 刷新。
17. 8.7.6.8B-1：创建 `ViewportFocusSelectionRoute.cs`、`ViewportFocusSelectionResult.cs`— 提取焦点选择。
18. 8.7.6.8B-2：创建 `Scene3dResizeRenderRoute.cs`、`Scene3dResizeRenderRequest.cs`、`Scene3dResizeRenderResult.cs`— 提取 Resize。
19. 8.7.6.8B-3：创建 `EditorShellWindowRoute.cs`、`EditorShellWindowCommand.cs`、`EditorShellWindowResult.cs`— 提取窗口命令。
20. 8.7.6.8C-1：创建 `EditorStartupBootstrapRoute.cs`、`EditorStartupBootstrapResult.cs`、`EditorStartupWorldResult.cs`、`EditorStartupVulkanRoute.cs`、`EditorStartupVulkanRequest.cs`、`EditorStartupVulkanResult.cs`、`EditorStartupVulkanState.cs`、`EditorStartupVulkanStep.cs`— 提取启动引导。
21. 8.7.6.8D-1：创建 `EditorViewportInputRoute.cs`、`EditorViewportInputRequest.cs`、`EditorViewportInputResult.cs`、`EditorViewportInputState.cs`、`EditorViewportInputKind.cs`— 提取输入管线。
22. 8.7.6.8D-2：创建 `EditorTransformInputRoute.cs`、`EditorTransformInputRequest.cs`、`EditorTransformInputResult.cs`、`EditorSceneToolInputRoute.cs`、`EditorSceneToolInputResult.cs`— 提取变换/工具输入桥。
23. 8.7.6.8D-3：创建 `EditorGroundHoverInputRoute.cs`、`EditorGroundHoverInputRequest.cs`、`EditorGroundHoverInputResult.cs`、`EditorPickInputRoute.cs`、`EditorPickInputResult.cs`— 提取地面悬停/拾取桥。
24. 8.7.6.8D-4：创建 `EditorScene3dCommandRoute.cs`、`EditorScene3dCommandRequest.cs`、`EditorScene3dCommandResult.cs`、`EditorScene3dCommandState.cs`、`EditorScene3dCommandKind.cs`— 提取 Scene3D 命令。
25. 8.7.6.8D-5：创建 `EditorPanelApplyRoute.cs`、`EditorPanelApplyRequest.cs`、`EditorPanelApplyResult.cs`、`EditorPanelApplyState.cs`、`EditorPanelApplyKind.cs`— 提取面板操作应用。
26. 8.7.6.8E-1：创建 `EditorTransformApplyRoute.cs`、`EditorTransformApplyRequest.cs`、`EditorTransformApplyResult.cs`、`EditorGroundPlacementRoute.cs`、`EditorGroundPlacementResult.cs`— 提取变换应用收口。
27. 8.7.6.8E-2：创建 `EditorFeedbackRoute.cs`— 提取反馈路由。
28. 8.7.6.8E-3：创建 `EditorShellControlRefs.cs`、`EditorShellRouteBuild.cs`、`EditorShellRouteSet.cs`、`EditorShellAttachRoute.cs`、`EditorShellAttachRequest.cs`、`EditorShellAttachResult.cs`、`EditorShellDetachRoute.cs`、`EditorShellDetachResult.cs`— 提取构造/FindControls/路由接线收口。
29. 8.7.6.8E-3R/4：创建 `Composition/` 目录，合并 EditorShellRouteSet（聚合 ~26 个 Route 引用）。

**EditorShell 提取前后对比：**
- EditorShell.axaml.cs：3,041 行 → 970 行（-2,071 行）
- 新增 26+ Route 类，所有 Route 类 ≤100 行
- 白名单债务初始 74 项

**面板 SRP 拆分（8.7.7）：**
30. 8.7.7A：InspectorPanel SRP 拆分
    - InspectorPanel.axaml.cs：387→84 行
    - 新增 `InspectorSelectionView.cs`、`InspectorTransformView.cs`、`InspectorScrubInput.cs`、`InspectorTransformBinder.cs`、`Transform/TransformPositionAxis.cs`
31. 8.7.7B-1：WorldHierarchyTreePanel SRP 拆分
    - WorldHierarchyTreePanel.axaml.cs：229→95 行
    - 新增 `WorldHierarchy/` 目录（8 个文件）：`WorldHierarchyNodeView.cs`、`WorldHierarchyTreeSelection.cs`、`WorldHierarchyTreeExpansion.cs`、`WorldHierarchyTreeIndex.cs`、`WorldHierarchyTreeItems.cs`、`WorldHierarchyTreeViewState.cs`、`WorldHierarchyProgrammaticSelection.cs`
32. 8.7.7B-2：ProjectContentTreePanel / ProjectWorldDockPanel SRP 拆分
    - ProjectContentTreePanel.axaml.cs：168→83 行
    - ProjectWorldDockPanel.axaml.cs：219→76 行
    - 新增 `ProjectContentTree/` 目录（6 个文件）
    - 新增 `LeftDock/` 目录（ProjectWorldDockPanel + ProjectWorldDockTabs）
33. 8.7.7C-1：NativeHost HWND Lifecycle SRP 拆分
    - WindowsVulkanViewportHostControl.cs：605→386 行
    - 新增 `NativeHost/Win32ViewportWindowClass.cs`、`NativeViewportHostInfo.cs`
    - 新增 `NativeHost/Input/` 目录（5 个文件）：`NativeViewportPointerMessages.cs`、`NativeViewportMouseCapture.cs`、`NativeViewportMouseTrack.cs`、`NativeViewportPointerAction.cs`、`NativeViewportPointerRequest.cs`
    - 新增 `WindowsVulkanViewportPickInput.cs`、`ViewportSceneToolPressResult.cs`
34. 8.7.7C-2：NativeHost Raw Pointer Messages SRP 拆分（最新）

**平台无关 Editor 项目创建：**
35. 创建 `FluidWarfare.Editor/FluidWarfare.Editor.csproj` — 平台无关编辑器项目。
36. 创建 `FluidWarfare.Editor/EntityTransform/`（5 文件）：EditorEntityTransformChange/Draft/Validation、EditorGroundPlacementState、EditorWorldDirtyState。
37. 创建 `FluidWarfare.Editor/Input/`（13 文件）：Actions（4）、Bindings（5）、Runtime（4）、Settings（4）。
38. 创建 `FluidWarfare.Editor/ProjectContentTree/`（5 文件）：ProjectContentTree/Builder/Node/NodeKind/Search。
39. 创建 `FluidWarfare.Editor/Selection/`（4 文件）。
40. 创建 `FluidWarfare.Editor/Transform/`（13 文件）：Data、Edit（5）、Scrub、Translation/Axis/Plane/Constraint。
41. 创建 `FluidWarfare.Editor/ViewportGround/`（2 文件）。
42. 创建 `FluidWarfare.Editor/WorldHierarchy/`（5 文件）。

**新面板/功能区域：**
43. 新增 `Panels/DebugDock/DebugDockPanel.axaml` + `.axaml.cs` — 渲染调试页签（RenderScene/渲染场景/性能）。
44. 新增 `Panels/HierarchyVisual/`（4 个文件）— 层级可视化画布。
45. 新增 `Panels/Viewport/Input/WindowsViewportInputTranslator.cs`（284 行）— 原始事件转 EditorInputMatch。
46. 新增 `Panels/Viewport/Input/Win32KeyCodeMapper.cs`— Win32 键码映射（⚠️ 与 Editor 项目重复，待清理）。
47. 新增 `Panels/Viewport/Tools/`（3 个文件）— 工具面板。
48. 新增 `Assets/Icons/Hierarchy/`（17 个 SVG 图标）— 项目树和世界树图标。
49. 新增 `About/AboutFluidWarfareWindow.axaml` + `.axaml.cs`— 关于窗口。
50. 新增 `Preferences/EditorPreferencesWindow.axaml` + `.axaml.cs`— 偏好设置窗口。

**Engine 层扩展：**
51. 新增 `FluidWarfare.Engine/World/EntityPosition/`（3 文件）：WorldEntityPositionChange/WriteResult/Writer。
52. 新增 `FluidWarfare.Project/World/Transform/`（4 文件）：SceneTransform/Defaults/Matrix/Validation。
53. 新增 `FluidWarfare.Render/Scene/Position/`（3 文件）：RenderObjectPositionChange/WriteResult/RenderSceneObjectPositionWriter。
54. 新增 `FluidWarfare.Render/Camera/Navigation/`（4 文件）。
55. 新增 `FluidWarfare.Render/Selection/Ground/`、`Pointer/`、`Presented/`、`Screen/` — 选择系统扩展。
56. 新增 `FluidWarfare.Render/ViewportNavigation/`（10 文件）— 视口导航元素。
57. 新增 `FluidWarfare.Render.Vulkan/Camera/PresentedCameraSnapshot.cs`、`SceneRayBuildStatus.cs`。
58. 新增 `FluidWarfare.Render.Vulkan/Scene3D/GroundCursor/`（3 文件）— 地面光标几何/信息/状态。
59. 新增 `FluidWarfare.Render.Vulkan/Scene3D/Overlay/`（7 文件）— 导航覆盖层渲染。

**测试扩展：**
60. 新增 `FluidWarfare.Tests/Architecture/CodeFileBudgetTests.cs`（240 行）— 自动化代码宪法测试（文件行数、目录文件数、禁用命名检查）。
61. 新增 `FluidWarfare.Tests/Editor/` — Editor 模块测试：EntityTransform（2）、Input（6）、Transform（3）、ViewportGround（1）、WorldHierarchy（1）。
62. 新增 `FluidWarfare.Tests/Render/Camera/Navigation/`（2 测试文件）。
63. 新增 `FluidWarfare.Tests/Render/Scene/Position/`（1 测试文件）。
64. 新增 `FluidWarfare.Tests/Render/Selection/`（4 测试文件）。
65. 新增 `FluidWarfare.Tests/Render/Vulkan/Scene3D/GroundCursor/`（1 测试文件）。
66. 新增 `FluidWarfare.Tests/Render/ViewportNavigation/`（1 测试文件）。
67. 新增 `FluidWarfare.Tests/Render/Vulkan/Camera/PerspectiveOrthographicPickingTests.cs`、`ProjectionUnprojectionRoundTripTests.cs`。
68. 新增 `FluidWarfare.Tests/Render/Vulkan/Shaders/CompiledShadersCollection.cs`、`CompiledShadersTests.cs`。
69. 新增 `GameProjects/SampleProject/units/sample_unit_2.json`、`sample_unit_3.json`。

### 新增（初始版本 — Milestone 1 — 8.7.5）
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
72j. Milestone 8.0：新增 `FluidWarfare.Render.Vulkan/Markers/VulkanMarkerDrawStatus.cs`。
72k. Milestone 8.0：新增 `FluidWarfare.Render.Vulkan/Markers/VulkanMarkerDrawInfo.cs`。
72l. Milestone 8.0：新增 `FluidWarfare.Render.Vulkan/Markers/VulkanMarkerDrawResult.cs`。
72m. Milestone 8.0：新增 `FluidWarfare.Render.Vulkan/Markers/VulkanMarkerClearRectRenderer.cs`。
72n. Milestone 8.0：新增 `FluidWarfare.Tests/Render/Vulkan/Markers/VulkanMarkerDrawInfoTests.cs`。
72o. Milestone 8.0：新增 `FluidWarfare.Tests/Render/Vulkan/Markers/VulkanMarkerDrawResultTests.cs`。
72p. Milestone 8.1：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dStatus.cs`。
72q. Milestone 8.1：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dInfo.cs`。
72r. Milestone 8.1：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dVertex.cs`。
72s. Milestone 8.1：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dRenderer.cs`。
72t. Milestone 8.1：新增 `FluidWarfare.Render.Vulkan/Camera/VulkanCameraInfo.cs`。
72u. Milestone 8.1：新增 `FluidWarfare.Render.Vulkan/Camera/VulkanCameraMatrices.cs`。
72v. Milestone 8.1：新增 `FluidWarfare.Render.Vulkan/Shaders/basic_3d.vert` 和 `.frag` 着色器源文件。
72w. Milestone 8.1：新增 `FluidWarfare.Render.Vulkan/Shaders/Compiled/basic_3d.vert.spv` 和 `.frag.spv` 预编译 SPIR-V。
72x. Milestone 8.1：新增 `FluidWarfare.Render.Vulkan/Shaders/CompiledShaders.cs` 内嵌 SPIR-V 字节码。
72x2. Milestone 8.1.3：`CompiledShaders.cs` 改为空数组占位，`Compiled/` 目录废弃，`.spv` 文件从 git 移除。
72y. Milestone 8.R.1：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dRunGate.cs`。
72z. Milestone 8.R.1：新增 `FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dRunGateTests.cs`。
72α. Milestone 8.R.2：新增 `tools/shaders/compile_basic_3d.ps1`。
72β. Milestone 8.R.2：新增 `tools/shaders/validate_basic_3d.ps1`。
72γ. Milestone 8.R.2：新增 `tools/shaders/README.md`。
72δ. Milestone 8.R.2：新增 `FluidWarfare.Render.Vulkan/Shaders/Compiled/.gitkeep`。
72ε. Milestone 8.R.3：新增 `tools/shaders/embed_basic_3d_shaders.ps1`。
72ζ. Milestone 8.R.3：新增 `FluidWarfare.Tests/Render/Vulkan/Shaders/CompiledShadersTests.cs`。
72η. Milestone 8.R.4：新增 `FluidWarfare.Render.Vulkan/Validation/VulkanValidationStatus.cs`。
72θ. Milestone 8.R.4：新增 `FluidWarfare.Render.Vulkan/Validation/VulkanValidationInfo.cs`。
72ι. Milestone 8.R.4：新增 `FluidWarfare.Render.Vulkan/Validation/VulkanValidationOptions.cs`。
72κ. Milestone 8.R.4：新增 `FluidWarfare.Render.Vulkan/Validation/VulkanValidationMessageInfo.cs`。
72λ. Milestone 8.R.4：新增 `FluidWarfare.Render.Vulkan/Validation/VulkanValidationMessageStore.cs`。
72μ. Milestone 8.R.4：新增 `FluidWarfare.Render.Vulkan/Validation/VulkanValidationAvailabilityProbe.cs`。
72ν. Milestone 8.R.4：新增 `FluidWarfare.Render.Vulkan/Validation/VulkanDebugMessengerScope.cs`。
72ξ. Milestone 8.R.4：新增 `FluidWarfare.Tests/Render/Vulkan/Validation/VulkanValidationOptionsTests.cs`。
72ο. Milestone 8.R.4：新增 `FluidWarfare.Tests/Render/Vulkan/Validation/VulkanValidationInfoTests.cs`。
72π. Milestone 8.R.4：新增 `FluidWarfare.Tests/Render/Vulkan/Validation/VulkanValidationMessageStoreTests.cs`。
72ρ. Milestone 8.R.5：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dShaderModules.cs`。
72σ. Milestone 8.R.5：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dPipelineLayout.cs`。
72τ. Milestone 8.R.5：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dPipelines.cs`。
72υ. Milestone 8.R.5：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dVertexBuffers.cs`。
72φ. Milestone 8.R.5：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dCommandRecorder.cs`。
72χ. Milestone 8.R.5：新增 `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dRenderResources.cs`。
72y. Milestone 8.1：新增 `FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dInfoTests.cs`。
72z. Milestone 8.1：新增 `FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dVertexTests.cs`。
72α. Milestone 8.1：新增 `FluidWarfare.Tests/Render/Vulkan/Camera/VulkanCameraInfoTests.cs`。
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
110. Milestone 8.0.1：WindowsVulkanViewportHostControl 根据 Avalonia Bounds 调用 SetWindowPos 同步 HWND 尺寸。
111. Milestone 8.0.1：EditorShell 的 Swapchain / Clear / Marker 绘制改用真实 NativeHost 宽高。
112. Milestone 8.0.1：DebugDockPanel 压缩底部页签字号与 Padding。
113. Milestone 8.0.1：WindowsVulkanViewportHostControl 在尺寸变化后上报 HostInfoChanged。
114. Milestone 8.0.1：VulkanViewportHostPanel 向 EditorShell 透传 NativeHostInfoChanged。
115. Milestone 8.0.1：EditorShell 移除旧 VulkanRenderContext 定时渲染路径，resize 后防抖重建 Swapchain / Clear / MarkerDraw。
116. Editor UI 布局补丁：收紧面板标题字号、底部页签高度与日志区域内边距。
117. Editor UI 布局补丁：移除 LogPanel 内部重复的“日志”标题，让日志 TextBox 占满日志页签内容区。
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

当前执行 Milestone 8.7.7：EditorShell SRP 重构 + GUI 面板职责拆分。

### 已完成的架构重构

**EditorShell（8.7.6.1 — 8.7.6.8E）：**
- EditorShell.axaml.cs：3,041 行 → **970 行**（-2,071 行）
- 提取了 **26+ Route 类**（每个 ≤100 行），涵盖：
  - Scene3D 帧路径、帧提交、Session 生命周期
  - 变换交互、Transform 应用层、Gizmo 呈现
  - 选择路由与呈现（3D 选择 ↔ 面板同步）
  - 输入管线（Raw 事件 → EditorInputMatch → Route 分发）
  - 启动引导（Vulkan 初始化、项目/世界 Bootstrap）
  - 面板操作应用、窗口命令、Diagnostics
  - 构造/FindControls/Route 接线重构
- 新增 `Shell/Composition/` 目录，`EditorShellRouteSet` 聚合所有 Route
- 代码宪法自动化测试 `CodeFileBudgetTests.cs` 已上线

**GUI 面板 SRP 拆分（8.7.7）：**
- InspectorPanel：387→84 行（提取 5 个职责文件）
- WorldHierarchyTreePanel：229→95 行（提取 8 个文件）
- ProjectContentTreePanel：168→83 行（提取 6 个文件）
- ProjectWorldDockPanel：219→76 行（提取 2 个文件）
- WindowsVulkanViewportHostControl：605→386 行（提取 10 个文件）

**平台无关 Editor 项目：**
- 创建 `FluidWarfare.Editor/` — 无 Avalonia/Vulkan 依赖
- 包含 Input、Selection、Transform、ProjectContentTree、WorldHierarchy 等模块

### 当前管线

```
8.7.7A InspectorPanel                 ✅
8.7.7B Project / World Tree Panels    ✅
8.7.7C WindowsVulkanViewportHostControl ▶ 进行中
  ├─ C-1 HWND Lifecycle               ✅
  ├─ C-2 Raw Pointer Messages         ✅ (最新)
  ├─ C-3 Resize/Events                ⏳
  └─ C-4 Cleanup                      ⏳
8.7.7D VulkanScene3dSession           ✅
  ├─ D-1 Session Ownership            ✅
  ├─ D-2 Swapchain Lifecycle          ✅
  ├─ D-3 Vulkan Frame Resources       ✅
  ├─ D-4 Acquire / Present Route      ✅
  ├─ D-5A Dispose Order Map           ✅
  ├─ D-5B Resource Dispose Steps      ✅
  ├─ D-6A Session Start/Create SRP    ✅
  ├─ D-6B Session Render/Resize Thin  ✅
  ├─ D-6C Session Field/Handle Split  ✅
  └─ D-6D 白名单删除 + 目录债务清理   ✅
8.7.7E VulkanScene3dRenderer          ✅
  ├─ E-1 Renderer 审计               ✅
  ├─ E-2A Scene3D 目录合规            ✅
  ├─ E-2B-1 Pipeline 模块收口         ✅
  ├─ E-2B-2A Vertex 结构收口          ✅
  ├─ E-2B-2B VertexBuffers 收口       ✅
  ├─ E-2B-3 CommandRecorder 收口      ✅
  ├─ E-2B-3R Commands SRP 复核        ✅
  ├─ E-2B-4 RenderResources 收口      ✅
  └─ E-2C Renderer 去重式 SRP 收口    ✅ (最新)
8.7.7F 全仓白名单债务清算             ⏳
8.7.7F 白名单债务清算 + CHANGELOG     ⏳
```

**当前状态：**
- Build 0 Error / 0 Warning / Tests 625+ 全通过
- Editor 启动正常，Vulkan 视口正确渲染
- 编辑器功能完整：3D 场景、Picking、变换编辑、层级树、检查器
- 白名单债务：74 → ~70（持续下降）
- 所有提交推送至 GitHub（codex/transform-clean-rewrite 分支）

## 3. 顶层目录结构

当前真实顶层结构如下（总计 ~557 个跟踪文件，不含 `bin/` `obj/`）：

```text
FluidWarfare/
|-- .gitattributes
|-- .gitignore
|-- FluidWarfare.sln
|-- file-tree.md
|-- run.bat
|
|-- FluidWarfare.Core/
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
|
|-- FluidWarface.Project/
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
|   |-- Validation/
|   |   |-- ProjectValidationIssue.cs
|   |   `-- ProjectValidationReport.cs
|   `-- World/Transform/
|       |-- SceneTransform.cs
|       |-- SceneTransformDefaults.cs
|       |-- SceneTransformMatrix.cs
|       `-- SceneTransformValidation.cs
|
|-- FluidWarfare.Bridge.ProjectEngine/
|   |-- FluidWarfare.Bridge.ProjectEngine.csproj
|   `-- World/
|       |-- ProjectContentWorldSeedResult.cs
|       `-- ProjectContentWorldSeeder.cs
|
|-- FluidWarfare.Engine/
|   |-- FluidWarfare.Engine.csproj
|   |-- Components/
|   |   |-- DisplayNameComponent.cs
|   |   `-- PositionComponent.cs
|   `-- World/
|       |-- EntityPosition/
|       |   |-- WorldEntityPositionChange.cs
|       |   |-- WorldEntityPositionWriteResult.cs
|       |   `-- WorldEntityPositionWriter.cs
|       |-- ProjectContentEntitySource.cs
|       |-- WorldEntityInfo.cs
|       `-- WorldState.cs
|
|-- FluidWarfare.Editor/                    ← 新增：平台无关编辑器
|   |-- FluidWarfare.Editor.csproj
|   |-- EntityTransform/
|   |   |-- EditorEntityTransformChange.cs
|   |   |-- EditorEntityTransformDraft.cs
|   |   |-- EditorEntityTransformValidation.cs
|   |   |-- EditorGroundPlacementState.cs
|   |   `-- EditorWorldDirtyState.cs
|   |-- Input/
|   |   |-- Actions/
|   |   |   |-- EditorInputActionCatalog.cs
|   |   |   |-- EditorInputActionContext.cs
|   |   |   |-- EditorInputActionDefinition.cs
|   |   |   `-- EditorInputValueKind.cs
|   |   |-- Bindings/
|   |   |   |-- EditorInputBinding.cs
|   |   |   |-- EditorInputBindingSet.cs
|   |   |   |-- EditorInputConflictDetector.cs
|   |   |   |-- EditorInputGesture.cs
|   |   |   `-- Win32KeyCodeMapper.cs   (← 规范副本)
|   |   |-- Runtime/
|   |   |   |-- EditorInputBindingSnapshot.cs
|   |   |   |-- EditorInputEvent.cs
|   |   |   |-- EditorInputMatch.cs
|   |   |   `-- EditorInputService.cs
|   |   `-- Settings/
|   |       |-- EditorSettingsDocument.cs
|   |       |-- EditorSettingsPath.cs
|   |       |-- EditorSettingsReader.cs
|   |       `-- EditorSettingsWriter.cs
|   |-- ProjectContentTree/
|   |   |-- ProjectContentTree.cs
|   |   |-- ProjectContentTreeBuilder.cs
|   |   |-- ProjectContentTreeNode.cs
|   |   |-- ProjectContentTreeNodeKind.cs
|   |   `-- ProjectContentTreeSearch.cs
|   |-- Selection/
|   |   |-- EditorEntitySelectionChange.cs
|   |   |-- EditorEntitySelectionOrigin.cs
|   |   |-- EditorEntitySelectionState.cs
|   |   `-- EditorSelectionDiagnostics.cs
|   |-- Transform/
|   |   |-- Data/EntitySceneTransformAccess.cs
|   |   |-- Edit/
|   |   |   |-- TransformEditKind.cs
|   |   |   |-- TransformEditResult.cs
|   |   |   |-- TransformEditSession.cs
|   |   |   |-- TransformEditSessionStart.cs
|   |   |   `-- TransformEditSnapshot.cs
|   |   |-- Scrub/TransformAxisScrubState.cs
|   |   |-- Translation/
|   |   |   |-- Axis/
|   |   |   |   |-- AxisScreenMetric.cs
|   |   |   |   |-- AxisTranslationAnchor.cs
|   |   |   |   |-- AxisTranslationMode.cs
|   |   |   |   |-- AxisTranslationSolver.cs
|   |   |   |   `-- AxisTranslationStart.cs
|   |   |   |-- Constraint/
|   |   |   |   |-- TransformOrientation.cs
|   |   |   |   |-- TranslationAxis.cs
|   |   |   |   |-- TranslationConstraint.cs
|   |   |   |   |-- TranslationConstraintText.cs
|   |   |   |   `-- TranslationPlane.cs
|   |   |   `-- Plane/
|   |   |       |-- PlaneTranslationAnchor.cs
|   |   |       |-- PlaneTranslationMode.cs
|   |   |       |-- PlaneTranslationSolver.cs
|   |   |       `-- PlaneTranslationStart.cs
|   |-- ViewportGround/
|   |   |-- EditorGroundPointerChange.cs
|   |   `-- EditorGroundPointerState.cs
|   `-- WorldHierarchy/
|       |-- WorldHierarchyNode.cs
|       |-- WorldHierarchyNodeKind.cs
|       |-- WorldHierarchySearch.cs
|       |-- WorldHierarchyTree.cs
|       `-- WorldHierarchyTreeBuilder.cs
|
|-- FluidWarfare.Editor.Windows/           ← Avalonia Windows 编辑器
|   |-- FluidWarfare.Editor.Windows.csproj
|   |-- Program.cs
|   |-- App.axaml / App.axaml.cs
|   |-- MainWindow.axaml / MainWindow.axaml.cs
|   |-- app.manifest
|   |-- Properties/launchSettings.json
|   |-- About/
|   |   |-- AboutFluidWarfareWindow.axaml
|   |   `-- AboutFluidWarfareWindow.axaml.cs
|   |-- Preferences/
|   |   |-- EditorPreferencesWindow.axaml
|   |   `-- EditorPreferencesWindow.axaml.cs
|   |-- Assets/Icons/Hierarchy/
|   |   `-- (17 SVG icons: faction, file, folder, image, map, project, unit, weapon, world, etc.)
|   |-- Panels/
|   |   |-- DebugDock/
|   |   |   |-- DebugDockPanel.axaml
|   |   |   `-- DebugDockPanel.axaml.cs     (145 行, 渲染调试页签)
|   |   |-- HierarchyVisual/
|   |   |   |-- HierarchyBranchCanvas.cs
|   |   |   |-- HierarchyBranchInfo.cs
|   |   |   |-- HierarchyNodeRow.axaml / .cs
|   |   |   `-- HierarchyNodeViewContract.cs
|   |   |-- Inspector/
|   |   |   |-- InspectorPanel.axaml
|   |   |   |-- InspectorPanel.axaml.cs     (84 行, 薄转发层)
|   |   |   |-- InspectorSelectionView.cs   (35 行, 面板可见性)
|   |   |   |-- InspectorScrubInput.cs       (53 行, 拖拽微调)
|   |   |   |-- InspectorTransformBinder.cs  (38 行, 键盘绑定)
|   |   |   |-- InspectorTransformView.cs    (45 行, 变换输入框)
|   |   |   `-- Transform/
|   |   |       `-- TransformPositionAxis.cs (11 行)
|   |   |-- LeftDock/
|   |   |   |-- ProjectWorldDockPanel.axaml / .cs (76 行)
|   |   |   `-- ProjectWorldDockTabs.cs      (71 行)
|   |   |-- Logging/
|   |   |   |-- LogPanel.axaml
|   |   |   `-- LogPanel.axaml.cs
|   |   |-- ProjectContentTree/
|   |   |   |-- ProjectContentNodeView.cs    (114 行)
|   |   |   |-- ProjectContentTreeExpansion.cs
|   |   |   |-- ProjectContentTreeIndex.cs   (92 行)
|   |   |   |-- ProjectContentTreeItems.cs
|   |   |   |-- ProjectContentTreePanel.axaml / .cs (83 行)
|   |   |   `-- ProjectContentTreeSelection.cs
|   |   |-- Status/
|   |   |   |-- StatusBarPanel.axaml
|   |   |   `-- StatusBarPanel.axaml.cs
|   |   |-- Viewport/
|   |   |   |-- Input/
|   |   |   |   |-- Win32KeyCodeMapper.cs   (← 重复副本, 待清理)
|   |   |   |   `-- WindowsViewportInputTranslator.cs (284 行)
|   |   |   |-- NativeHost/
|   |   |   |   |-- Input/
|   |   |   |   |   |-- Pointer/
|   |   |   |   |   |   |-- NativeViewportMouseCapture.cs (33 行)
|   |   |   |   |   |   |-- NativeViewportMouseTrack.cs   (40 行)
|   |   |   |   |   |   |-- NativeViewportPointerAction.cs
|   |   |   |   |   |   |-- NativeViewportPointerMessages.cs (46 行)
|   |   |   |   |   |   `-- NativeViewportPointerRequest.cs (40 行)
|   |   |   |   |   |-- Keyboard/
|   |   |   |   |   |   |-- NativeViewportKeyboardMessages.cs (25 行)
|   |   |   |   |   |   `-- NativeViewportKeyboardRequest.cs
|   |   |   |   |   `-- Focus/
|   |   |   |   |       |-- NativeViewportFocusMessages.cs (19 行)
|   |   |   |   |       `-- NativeViewportHitTestMessages.cs
|   |   |   |   |   `-- Arbitration/
|   |   |   |   |       |-- NativeViewportInputArbitration.cs (77 行)
|   |   |   |   |       |-- NativeViewportInputArbitrationRequest.cs
|   |   |   |   |       |-- NativeViewportInputArbitrationResult.cs
|   |   |   |   |       |-- NativeViewportNavigationCapture.cs (26 行)
|   |   |   |   |       `-- NativeViewportSceneToolCapture.cs (29 行)
|   |   |   |   |-- Control/
|   |   |   |   |   |-- WindowsVulkanViewportHostControl.Events.cs (31 行)
|   |   |   |   |   `-- WindowsVulkanViewportHostControl.WndProc.cs (94 行)
|   |   |   |   |-- Lifecycle/
|   |   |   |   |   |-- NativeViewportCreate.cs (40 行)
|   |   |   |   |   |-- NativeViewportDestroy.cs (16 行)
|   |   |   |   |   |-- NativeViewportHostSync.cs (37 行)
|   |   |   |   |   `-- NativeViewportLifecycleResult.cs
|   |   |   |   |-- Win32/
|   |   |   |   |   |-- Win32ViewportWindowClass.cs (56 行)
|   |   |   |   |   |-- Win32ViewportDefaultProc.cs
|   |   |   |   |   |-- Win32ViewportModuleHandle.cs
|   |   |   |   |   `-- Win32ViewportDestroyWindow.cs
|   |   |   |   |-- HostInfo/
|   |   |   |   |   |-- WindowsVulkanViewportHostInfo.cs
|   |   |   |   |   |-- WindowsVulkanViewportHostState.cs
|   |   |   |   |   `-- NativeViewportHostInfo.cs (46 行)
|   |   |   |   |-- Picking/
|   |   |   |   |   `-- WindowsVulkanViewportPickInput.cs (53 行)
|   |   |   |   |-- SceneTool/
|   |   |   |   |   `-- ViewportSceneToolPressResult.cs
|   |   |   |   `-- WindowsVulkanViewportHostControl.cs (87 行)
|   |   |   |-- Tools/
|   |   |   |   |-- ViewportEditorTool.cs
|   |   |   |   |-- ViewportToolPalette.axaml / .cs
|   |   |   |-- ViewportEntitySummary.cs
|   |   |   |-- ViewportPlaceholderPanel.axaml (183 行) / .cs (189 行)
|   |   |   |-- ViewportRenderObjectSummary.cs
|   |   |   |-- ViewportRenderSceneSummary.cs
|   |   |   |-- VulkanViewportHostInfo.cs
|   |   |   |-- VulkanViewportHostPanel.axaml / .cs (158 行)
|   |   |   |-- VulkanViewportHostState.cs
|   |   |   `-- VulkanViewportNativeHostInfo.cs
|   |   `-- WorldHierarchy/
|   |       |-- WorldHierarchyNodeView.cs (91 行)
|   |       |-- WorldHierarchyProgrammaticSelection.cs
|   |       |-- WorldHierarchyTreeExpansion.cs
|   |       |-- WorldHierarchyTreeIndex.cs (112 行, 超线)
|   |       |-- WorldHierarchyTreeItems.cs
|   |       |-- WorldHierarchyTreePanel.axaml / .cs (95 行)
|   |       |-- WorldHierarchyTreeSelection.cs (87 行)
|   |       `-- WorldHierarchyTreeViewState.cs
|   |-- Shell/
|   |   |-- EditorShell.axaml
|   |   |-- EditorShell.axaml.cs (970 行, 原 3,041→重构后)
|   |   |-- EditorSelection.cs
|   |   |-- Composition/
|   |   |   |-- EditorShellControlRefs.cs
|   |   |   |-- EditorShellRouteBuild.cs
|   |   |   `-- EditorShellRouteSet.cs      (26+ Route 聚合)
|   |   |-- Diagnostics/
|   |   |   |-- EditorDiagnosticsRefreshKind.cs
|   |   |   |-- EditorDiagnosticsRefreshRequest.cs
|   |   |   |-- EditorDiagnosticsRefreshResult.cs
|   |   |   |-- EditorDiagnosticsRefreshRoute.cs
|   |   |   `-- EditorDiagnosticsRefreshState.cs
|   |   |-- Feedback/
|   |   |   `-- EditorFeedbackRoute.cs
|   |   |-- Input/
|   |   |   |-- EditorViewportInputKind.cs
|   |   |   |-- EditorViewportInputRequest.cs
|   |   |   |-- EditorViewportInputResult.cs
|   |   |   |-- EditorViewportInputRoute.cs
|   |   |   |-- EditorViewportInputState.cs
|   |   |   |-- Picking/
|   |   |   |   |-- EditorGroundHoverInputRequest.cs / Result.cs / Route.cs
|   |   |   |   `-- EditorPickInputResult.cs / Route.cs
|   |   |   `-- Transform/
|   |   |       |-- EditorSceneToolInputResult.cs / Route.cs
|   |   |       |-- EditorTransformInputRequest.cs / Result.cs / Route.cs
|   |   |-- Lifecycle/
|   |   |   |-- EditorShellAttachRequest.cs / Result.cs / Route.cs
|   |   |   |-- EditorShellDetachResult.cs / Route.cs
|   |   |-- Menu/
|   |   |   `-- EditorRunMenuRoute.cs
|   |   |-- Panels/
|   |   |   |-- EditorPanelApplyKind.cs / Request.cs / Result.cs / Route.cs / State.cs
|   |   |-- Scene3D/Commands/
|   |   |   |-- EditorScene3dCommandKind.cs / Request.cs / Result.cs / Route.cs / State.cs
|   |   |-- Startup/
|   |   |   |-- EditorStartupBootstrapResult.cs / Route.cs
|   |   |   |-- EditorStartupWorldResult.cs
|   |   |   `-- Vulkan/
|   |   |       |-- EditorStartupVulkanRequest.cs / Result.cs / Route.cs / State.cs / Step.cs
|   |   |-- Transform/
|   |   |   |-- EditorGroundPlacementResult.cs / Route.cs
|   |   |   `-- EditorTransformApplyRequest.cs / Result.cs / Route.cs
|   |   `-- Windows/
|   |       |-- EditorShellWindowCommand.cs / Result.cs / Route.cs
|   |-- Viewport/
|   |   |-- Camera/
|   |   |   |-- ViewportCameraCommand.cs / FocusTarget.cs / Result.cs / Route.cs
|   |   |-- Navigation/
|   |   |   |-- ViewportNavigationResponse.cs / Route.cs
|   |   |-- Picking/
|   |   |   |-- ViewportPickFailure.cs / Request.cs / Result.cs / Trace.cs
|   |   |   `-- ViewportPointerPickRoute.cs
|   |   |-- Project/
|   |   |   |-- ProjectBootstrapResult.cs / Route.cs
|   |   |-- Scene3D/
|   |   |   |-- Diagnostics/
|   |   |   |   |-- Scene3dDiagnosticSnapshot.cs / Text.cs
|   |   |   |   |-- VulkanViewportProbeResult.cs / Route.cs / State.cs
|   |   |   |-- Frame/
|   |   |   |   |-- Scene3dDrawListBuilder.cs
|   |   |   |   |-- Scene3dFrameRoute.cs / State.cs / PresentedState.cs
|   |   |   |-- Lifecycle/
|   |   |   |   |-- Scene3dSessionLifecycle.cs / RestartReason.cs
|   |   |   |   |-- Scene3dSessionStartRequest.cs / Result.cs / State.cs
|   |   |   |-- Resize/
|   |   |   |   |-- Scene3dResizeRenderRequest.cs / Result.cs / Route.cs
|   |   |   `-- Submit/
|   |   |       |-- Scene3dFrameSubmitInput.cs / Route.cs
|   |   |       |-- Scene3dGizmoSubmitSource.cs / PickSnapshotSource.cs
|   |   |-- Selection/
|   |   |   |-- Focus/
|   |   |   |   |-- ViewportFocusSelectionResult.cs / Route.cs
|   |   |   |-- Presentation/
|   |   |   |   |-- EditorSelectionPresenter.cs
|   |   |   |   |-- ProjectContentSelectionPresenter.cs
|   |   |   |   |-- SelectionPresentationResult.cs
|   |   |   |   |-- ViewportSelectionPresenter.cs
|   |   |   |   `-- WorldEntitySelectionPresenter.cs
|   |   |   `-- Route/
|   |   |       |-- EditorSelectionReason.cs / Request.cs / Route.cs / RouteResult.cs / State.cs
|   |   |-- Transform/
|   |   |   |-- Application/
|   |   |   |   |-- Capabilities/
|   |   |   |   |   |-- InspectorTransformDisplay.cs
|   |   |   |   |   |-- Scene3dEntityPositionWriter.cs
|   |   |   |   |   `-- WorldTransformWriter.cs
|   |   |   |   |-- EntityTransformCancel.cs / Commit.cs / Preview.cs
|   |   |   |   `-- TransformApplyResult.cs / ViewportRenderSceneStore.cs
|   |   |   |-- Drag/
|   |   |   |   |-- AxisDragAnchorBuilder.cs / PlaneDragAnchorBuilder.cs
|   |   |   |   |-- TransformDragKind.cs / MoveResult.cs / Route.cs / StartSnapshot.cs
|   |   |   |-- Gizmo/
|   |   |   |   |-- GizmoDist.cs
|   |   |   |   |-- HitTest/PlaneHandleHitTest.cs
|   |   |   |   |-- Layout/Measure.cs / MoveGizmoPlaneLayout.cs
|   |   |   |   |-- Visual/MoveGizmoAxisVertices.cs / PlaneVertices.cs
|   |   |   |   |-- MoveGizmoDrawList.cs / Element.cs / HitTest.cs
|   |   |   |   |-- MoveGizmoInteraction.cs / Layout.cs / VisualState.cs
|   |   |   |   `-- PresentedMoveGizmoSnapshot.cs
|   |   |   |-- Interaction/
|   |   |   |   |-- TransformInteractionResult.cs / State.cs
|   |   |   |   |-- TransformKeyboardRoute.cs / PointerRoute.cs / StartRequest.cs
|   |   |   `-- Presentation/
|   |   |       |-- MoveGizmoFrameInput.cs / Result.cs / Source.cs / Visibility.cs
|   |   `-- World/Bootstrap/
|   |       |-- WorldBootstrapEntitySeed.cs / Input.cs / RenderSeed.cs / Result.cs / Route.cs
|
|-- FluidWarfare.Ecs/                    `-- .gitkeep (待实现)
|-- FluidWarfare.World/                 `-- .gitkeep (待实现)
|-- FluidWarfare.Simulation/            `-- .gitkeep (待实现)
|-- FluidWarfare.Combat/                `-- .gitkeep (待实现)
|-- FluidWarfare.AI/                    `-- .gitkeep (待实现)
|-- FluidWarfare.Data/                  `-- .gitkeep (待实现)
|-- FluidWarfare.Runtime.Windows/       `-- .gitkeep (待实现)
|-- FluidWarfare.Runtime.Android/       `-- .gitkeep (待实现)
|-- FluidWarfare.Exporter/              `-- .gitkeep (待实现)
|
|-- FluidWarfare.Render/                   ← 抽象渲染层
|   |-- FluidWarfare.Render.csproj
|   |-- Camera/
|   |   |-- SceneCameraDefaults.cs / Limits.cs / Motion.cs / Pose.cs / State.cs
|   |   |-- Navigation/
|   |   |   |-- SceneNavigationCameraMotion.cs / SceneNavigationView.cs
|   |   |   |-- SceneOrthographicProjection.cs / SceneProjectionMode.cs
|   |   |   `-- SceneOrbitCameraMotion.cs / SceneOrbitCameraState.cs
|   |-- Coordinates/
|   |   |-- WorldCoordinateConvention.cs
|   |   `-- YUpToZUpPosition.cs
|   |-- Scene/
|   |   |-- Position/
|   |   |   |-- RenderObjectPositionChange.cs
|   |   |   |-- RenderObjectPositionWriteResult.cs
|   |   |   `-- RenderSceneObjectPositionWriter.cs
|   |   |-- RenderObjectInfo.cs
|   |   |-- RenderObjectVisualKind.cs
|   |   |-- RenderScene.cs
|   |   `-- RenderUnitPlacement.cs
|   |-- Selection/
|   |   |-- Ground/
|   |   |   |-- SceneGroundHit.cs / SceneGroundPlane.cs
|   |   |   `-- SceneRayGroundIntersection.cs
|   |   |-- Pointer/
|   |   |   |-- ScenePointerPickKind.cs / ScenePointerPickResult.cs
|   |   |   `-- ScenePointerPicker.cs
|   |   |-- Presented/
|   |   |   |-- PresentedEntityBounds.cs
|   |   |   |-- PresentedScenePickSnapshot.cs / Builder.cs
|   |   |   |-- RenderScenePickResult.cs / RenderScenePicker.cs
|   |   |-- SceneAxisAlignedBounds.cs / SceneRay.cs / SceneRayBoundsIntersection.cs
|   |   `-- Screen/
|   |       |-- ScreenBoundsProjection.cs / ScreenEntityPicker.cs / ScreenPickTolerance.cs
|   |-- ViewportNavigation/
|   |   |-- AxisProjection.cs
|   |   |-- ViewportNavigationAction.cs / DragMode.cs / Element.cs / HitTest.cs
|   |   |-- ViewportNavigationLayout.cs / LayoutCompute.cs
|   |   |-- ViewportNavigationPressResult.cs / Types.cs
|   `-- World/
|       `-- WorldToRenderSceneBuilder.cs
|
|-- FluidWarfare.Render.Vulkan/             ← Vulkan 渲染后端
|   |-- FluidWarfare.Render.Vulkan.csproj
|   |-- Backend/     (VulkanBackendInfo/Probe/Status)
|   |-- Context/     (VulkanRenderContext)
|   |-- Device/      (VulkanDeviceInfo/Probe/Status)
|   |-- Instance/    (VulkanInstanceInfo/Probe/Status)
|   |-- Surface/     (VulkanSurfaceInfo/Probe/Status)
|   |-- Swapchain/   (VulkanSwapchainInfo/Probe/Status)
|   |-- Clear/       (VulkanClearInfo/Probe/Status)
|   |-- Markers/     (VulkanMarkerDrawInfo/Result/Status, VulkanMarkerClearRectRenderer)
|   |-- Camera/
|   |   |-- PresentedCameraSnapshot.cs
|   |   |-- SceneRayBuildStatus.cs
|   |   |-- VulkanCameraInfo.cs / Matrices.cs / SceneRayBuilder.cs
|   |-- Scene3D/
|   |   |-- Pipeline/
|   |   |   |-- VulkanScene3dPipelines.cs (77 行) [153→77 ✅]
|   |   |   |-- VulkanScene3dPipelineCreate.cs (70 行)
|   |   |   |-- VulkanScene3dPipelineLayout.cs (67 行)
|   |   |   |-- VulkanScene3dShaderModules.cs (74 行)
|   |   |   `-- VulkanScene3dPushConstants.cs (38 行)
|   |   |-- Vertex/
|   |   |   |-- VulkanScene3dVertex.cs (9 行) [171→9 ✅]
|   |   |   |-- VulkanScene3dVertexGrid.cs (43 行)
|   |   |   |-- VulkanScene3dVertexCube.cs (34 行)
|   |   |   |-- VulkanScene3dVertexBuffers.cs (59 行) [171→59 ✅]
|   |   |   |-- Buffer/
|   |   |   |   `-- VulkanScene3dVertexBufferCreate.cs (53 行)
|   |   |   `-- VulkanSceneAxisGeometry.cs (33 行)
|   |   |-- Render/
|   |   |   |-- VulkanScene3dRunGate.cs (58 行)
|   |   |   |-- VulkanScene3dInfo.cs (31 行)
|   |   |   |-- VulkanScene3dStatus.cs (11 行)
|   |   |   |-- Probe/                           [E-2C-R: SRP 复核]
|   |   |   |   |-- VulkanScene3dRendererSetup.cs (79 行)
|   |   |   |   |-- Core/
|   |   |   |   |   `-- VulkanScene3dRenderer.cs (41 行)
|   |   |   |   |-- Create/
|   |   |   |   |   |-- VulkanScene3dRendererProbeInstance.cs (24 行)
|   |   |   |   |   |-- VulkanScene3dRendererProbeDevice.cs (59 行)
|   |   |   |   |   |-- VulkanScene3dRendererProbeSurfaceCreate.cs (21 行)
|   |   |   |   |   |-- VulkanScene3dRendererProbeSwapchain.cs (40 行)
|   |   |   |   |   `-- VulkanScene3dRendererProbeResources.cs (50 行)
|   |   |   |   |-- Surface/
|   |   |   |   |   `-- VulkanScene3dRendererProbeSurfaceChoice.cs (27 行)
|   |   |   |   `-- Frame/
|   |   |   |       |-- VulkanScene3dRendererProbeFrame.cs (39 行)
|   |   |   |       |-- VulkanScene3dRendererProbeAcquire.cs (23 行)
|   |   |   |       |-- VulkanScene3dRendererProbeMVP.cs (31 行)
|   |   |   |       |-- VulkanScene3dRendererProbeSubmit.cs (19 行)
|   |   |   |       `-- VulkanScene3dRendererProbePresent.cs (49 行)
|   |   |   |-- Resources/
|   |   |-- Commands/
|   |   |   |-- VulkanScene3dCommandRecorder.cs (45 行) [200→45 ✅]
|   |   |   |-- VulkanScene3dCommandRenderPass.cs (49 行)
|   |   |   |-- VulkanScene3dCommandGrid.cs (52 行)
|   |   |   `-- VulkanScene3dCommandUnits.cs (56 行)
|   |   |-- Depth/
|   |   |   |-- VulkanScene3dDepthFormatSelector.cs
|   |   |   |-- VulkanScene3dDepthAttachmentInfo.cs / DepthAttachments.cs
|   |   |-- GroundCursor/
|   |   |   |-- VulkanGroundCursorGeometry.cs / Info.cs / State.cs
|   |   |-- Overlay/                          [E-2D: 严格 SRP 子目录]
|   |   |   |-- Geometry/
|   |   |   |   |-- VulkanNavigationOverlayGeometry.cs (84 行)
|   |   |   |   |-- VulkanNavigationOverlayPrimitives.cs (62 行)
|   |   |   |   |-- VulkanNavigationOverlayShapes.cs (98 行)
|   |   |   |   |-- VulkanNavigationOverlayInfo.cs (14 行)
|   |   |   |   `-- VulkanOverlayVertex.cs (41 行)
|   |   |   |-- Resources/
|   |   |   |   |-- VulkanOverlayResources.cs (83 行)
|   |   |   |   |-- VulkanOverlayResources.Create.cs (54 行)
|   |   |   |   |-- VulkanOverlayPipeline.cs (55 行)
|   |   |   |   |-- VulkanOverlayPipelineLayout.cs (45 行)
|   |   |   `-- Render/
|   |   |       |-- VulkanOverlayCommandRecorder.cs (35 行)
|   |   |       `-- PresentedNavigationOverlaySnapshot.cs (17 行)
|   |   |-- Session/
|   |   |   |-- VulkanScene3dSession.cs (53 行) [≤100 ✅ 白名单已删除]
|   |   |   |-- VulkanScene3dSession.Frame.cs (70 行)
|   |   |   |-- VulkanScene3dSession.Properties.cs (87 行)
|   |   |   |-- FrameModel/   [FrameReason / FrameResult / FrameStatus]
|   |   |   |-- Dispose/
|   |   |   |   |-- VulkanScene3dSessionDisposeResources.cs (34 行)
|   |   |   |   |-- VulkanScene3dSessionDisposeSession.cs (31 行)
|   |   |   |   |-- Render/
|   |   |   |   |   |-- VulkanScene3dPipelineDispose.cs (23 行)
|   |   |   |   |   |-- VulkanScene3dShaderDispose.cs (18 行)
|   |   |   |   |   |-- VulkanScene3dBufferDispose.cs (38 行)
|   |   |   |   |   `-- VulkanScene3dOverlayDispose.cs (23 行)
|   |   |   |   |-- Core/
|   |   |   |   |   `-- VulkanScene3dCoreDispose.cs (40 行)
|   |   |   |   `-- State/
|   |   |   |       |-- VulkanScene3dSessionDisposeState.cs (32 行)
|   |   |   |       `-- VulkanScene3dSessionDisposeTrace.cs (31 行)
|   |   |   |-- Core/
|   |   |   |   |-- VulkanScene3dSessionCoreState.cs (55 行)
|   |   |   |   |-- VulkanScene3dSessionRenderState.cs (30 行)
|   |   |   |   |-- VulkanScene3dSessionOverlayState.cs (22 行)
|   |   |   |   |-- VulkanScene3dSessionResourceFlags.cs (13 行)
|   |   |   |   `-- VulkanScene3dSessionProcLoad.cs (23 行)
|   |   |   |-- Start/
|   |   |   |   |-- VulkanScene3dSessionStart.cs (94 行)
|   |   |   |   |-- VulkanScene3dSessionCreateInstance.cs (74 行)
|   |   |   |   |-- VulkanScene3dSessionCreateSurface.cs (49 行)
|   |   |   |   |-- VulkanScene3dSessionCreateDevice.cs (83 行)
|   |   |   |   `-- VulkanScene3dSessionCreateResources.cs (55 行)
|   |   |   |-- Render/
|   |   |   |   |-- VulkanScene3dRenderFrame.cs (32 行)
|   |   |   |   |-- VulkanScene3dRenderResize.cs (87 行)
|   |   |   |   |-- VulkanScene3dRenderResizeAtomic.cs (79 行)
|   |   |   |   |-- VulkanScene3dRenderFrameInternal.cs (80 行)
|   |   |   |   `-- VulkanScene3dRenderFrameSnapshot.cs (62 行)
|   |   |   |-- FrameFlow/
|   |   |   |   |-- VulkanScene3dFrameAcquire.cs (100 行)
|   |   |   |   |-- VulkanScene3dFrameSubmit.cs (25 行)
|   |   |   |   |-- VulkanScene3dFramePresent.cs (66 行)
|   |   |   |   `-- VulkanScene3dFrameFailure.cs (17 行)
|   |   |   |-- Lifecycle/
|   |   |   |   `-- VulkanScene3dSessionState.cs (36 行)
|   |   |   |-- Handles/
|   |   |   |   |-- VulkanScene3dCoreHandles.cs (16 行)
|   |   |   |   |-- VulkanScene3dSwapchainHandles.cs (20 行)
|   |   |   |   `-- VulkanScene3dFrameHandles.cs (24 行)
|   |   |   |-- Surface/VulkanScene3dSurfaceFormats.cs / PresentModes.cs
|   |   |   `-- Swapchain/
|   |   |       |-- VulkanScene3dSwapchainFunctions.cs / CreateResult.cs
|   |   |       |-- VulkanScene3dSwapchainStage.cs / Invariant.cs
|   |   |       |-- Choice/
|   |   |       |   |-- VulkanScene3dSwapchainSelection.cs (21 行)
|   |   |       |   `-- VulkanScene3dSwapchainExtent.cs (16 行)
|   |   |       |-- Resources/
|   |   |       |   `-- VulkanScene3dSwapchainResources.cs (100 行) [≤100 ✅ 白名单已删除]
|   |   |       |-- Create/
|   |   |       |   `-- VulkanScene3dSwapchainCreateFlow.cs
|   |   |       |-- Images/
|   |   |       |   |-- VulkanScene3dSwapchainImageViews.cs (43 行)
|   |   |       |   `-- VulkanScene3dSwapchainFramebuffers.cs (72 行)
|   |   |       |-- Sync/
|   |   |       |   `-- VulkanScene3dSwapchainSync.cs
|   |   |       `-- Lifecycle/
|   |   |           `-- VulkanScene3dSwapchainDispose.cs
|   |   |-- VulkanSceneAxisGeometry.cs
|   |-- Shaders/
|   |   |-- basic_3d.frag / basic_3d.vert
|   |   |-- viewport_overlay.frag / viewport_overlay.vert
|   |   |-- Compiled/.gitkeep
|   |   `-- CompiledShaders.cs
|   `-- Validation/
|       |-- VulkanDebugMessengerScope.cs
|       |-- VulkanValidationAvailabilityProbe.cs / Info.cs / MessageInfo.cs
|       |-- VulkanValidationMessageStore.cs / Options.cs / Status.cs
|
|-- FluidWarfare.Tests/
|   |-- FluidWarfare.Tests.csproj
|   |-- CoreSmokeTests.cs
|   |-- Architecture/
|   |   |-- CodeFileBudgetTests.cs          (240 行, 代码宪法自动化)
|   |   `-- ProjectDependencyDirectionTests.cs
|   |-- Core/ (Identity/Logging/Math/Results/Time — 每个模块对应测试文件)
|   |-- Project/ (Content/Loading/Paths/Validation — 5 个测试文件)
|   |-- Bridge/ProjectEngine/World/ProjectContentWorldSeederTests.cs
|   |-- Engine/World/ (WorldStateTests, WorldEntityPositionWriterTests)
|   |-- Editor/
|   |   |-- EntityTransform/ (2 测试)
|   |   |-- Input/ (6 测试: BindingSet, ConflictDetector, Gesture, Service, ContextChain, Win32KeyCodeMapper)
|   |   |-- Transform/ (2 测试: TransformApplyResult, TransformDragRoute, MoveGizmoHitTest)
|   |   |-- ViewportGround/EditorGroundPointerStateTests.cs
|   |   `-- WorldHierarchy/WorldHierarchyTreeBuilderTests.cs
|   |-- Render/
|   |   |-- Camera/ (SceneCameraLimits/Motion/State/OrbitCameraMotion + Navigation 2 测试)
|   |   |-- Scene/Position/RenderSceneObjectPositionWriterTests.cs
|   |   |-- Selection/ (4 测试: Ground/Pointer/Presented/Screen)
|   |   |-- ViewportNavigation/ViewportNavigationLayoutTests.cs
|   |   |-- Vulkan/ (VulkanBackendInfo/DeviceInfo/InstanceInfo/SurfaceInfo/SwapchainInfo/
|   |   |   |         ClearInfo/Validation/Markers/Shaders/Scene3D/Camera 等 — 20+ 测试)
|   |   `-- World/WorldToRenderSceneBuilderTests.cs
|
|-- tools/shaders/
|   |-- README.md
|   |-- compile_basic_3d.ps1
|   |-- validate_basic_3d.ps1
|   `-- embed_basic_3d_shaders.ps1
|
|-- GameProjects/SampleProject/
|   |-- game.project.json
|   |-- factions/ (.gitkeep)
|   |-- units/ (sample_unit.json, sample_unit_2.json, sample_unit_3.json)
|   |-- weapons/ (sample_weapon.json)
|   |-- maps/ (.gitkeep)
|   |-- scenarios/ (.gitkeep)
|   |-- rules/ (.gitkeep)
|   `-- icons/ (sample_icon.svg)
|
|-- game_data/ (.gitkeep)
|-- assets/ (.gitkeep)
|-- shaders/ (.gitkeep)
|-- replays/ (.gitkeep)
|
`-- docs/
    |-- PROJECT_CHARTER.md
    |-- ENGINE_ARCHITECTURE.md
    |-- CODE_CONSTITUTION.md
    |-- NAMING_RULES.md
    |-- PHASE1_SCOPE.md
    |-- AI_DEVELOPMENT_RULES.md
    |-- CHANGELOG.md
    |-- LEGACY_FLUIDWARFARE_OLD_AUDIT.md
    `-- MILESTONE1_PUBLIC_VALIDATION.md
```

说明：`docs/AI_DEVELOPMENT_RULES.md` 要求每次结构变更后更新本文件（本版本由 Claude Code 于 2026-06-22 批量更新）。```

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
| FluidWarfare.Editor | 平台无关的编辑器模型层，承载 WorldHierarchy 层级树、搜索与选择状态，不依赖 Avalonia 或 Vulkan | 测试通过 |
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
| `FluidWarfare.Render.Vulkan/Markers/VulkanMarkerDrawStatus.cs` | Vulkan 点位绘制状态枚举 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Markers/VulkanMarkerDrawInfo.cs` | 点位绘制信息模型，包含显示名、像素坐标、尺寸与颜色，支持基于真实视口宽高的 FromWorldPosition 坐标映射 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Markers/VulkanMarkerDrawResult.cs` | 点位绘制结果模型，保存状态、中文说明、绘制点位数与耗时 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Markers/VulkanMarkerClearRectRenderer.cs` | 使用 vkCmdClearAttachments 在 RenderPass 内绘制点位小方块，不创建 Shader/Pipeline/Mesh/Texture | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Markers/VulkanMarkerDrawInfoTests.cs` | 验证点位绘制信息模型的坐标映射、颜色与尺寸 | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Markers/VulkanMarkerDrawResultTests.cs` | 验证点位绘制结果模型的基础语义 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dStatus.cs` | Vulkan 3D 场景渲染状态枚举 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dInfo.cs` | 3D 场景渲染结果模型，保存顶点数、线段数、三角形数、DrawCall 数、视口尺寸与耗时 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dVertex.cs` | 3D 场景顶点结构（位置+颜色），包含 BuildGrid/BuildCube/BuildAxes/ToInterleaved 顶点生成工具 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dRenderer.cs` | Scene3D 渲染流程编排层，调用各子模块 + Instance/Surface/Device/Swapchain 创建 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dShaderModules.cs` | 使用 CompiledShaders 已验证 SPIR-V 字节创建 ShaderModule | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dPipelineLayout.cs` | 创建 PipelineLayout 与 MVP PushConstantRange | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dPipelines.cs` | 创建 Grid LineList Pipeline 与 Unit TriangleList Pipeline | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dVertexBuffers.cs` | 创建并上传 Grid/Unit 顶点 Buffer | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dCommandRecorder.cs` | 录制 Scene3D CommandBuffer，顺序为 Grid/GroundCursor/Units | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/GroundCursor/VulkanGroundCursorGeometry.cs` | Ground Cursor 顶点数据（青色十字+方框，12 顶点 6 线段） | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/GroundCursor/VulkanGroundCursorState.cs` | Ground Cursor 运行时状态（可见性+世界坐标+Revision） | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/GroundCursor/VulkanGroundCursorInfo.cs` | Ground Cursor 诊断信息 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dRenderResources.cs` | 持有 Scene3D 创建的 Vulkan 资源句柄，按依赖逆序 Dispose | 测试通过 |
| `FluidWarfare.Render.Vulkan/Camera/VulkanCameraInfo.cs` | 固定 3D 相机参数，含 DefaultBattlefield 默认战场相机 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Camera/VulkanCameraMatrices.cs` | 3D 矩阵计算，LookAt、PerspectiveVulkan（NDC 0..1）与 MVP 合成 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Shaders/basic_3d.vert` | 基础 3D 顶点着色器 GLSL 源文件，position+color 输入，MVP push constant | 已生成 |
| `FluidWarfare.Render.Vulkan/Shaders/basic_3d.frag` | 基础 3D 片段着色器 GLSL 源文件，颜色传递 | 已生成 |
| `FluidWarfare.Render.Vulkan/Shaders/Compiled/basic_3d.vert.spv` | 由 glslangValidator 编译，1232 字节，spirv-val 验证通过 | 已验证 |
| `FluidWarfare.Render.Vulkan/Shaders/Compiled/basic_3d.frag.spv` | 由 glslangValidator 编译，376 字节，spirv-val 验证通过 | 已验证 |
| `FluidWarfare.Render.Vulkan/Shaders/CompiledShaders.cs` | 含 HasValidatedBasic3dShaders 验证标记，由 embed_basic_3d_shaders.ps1 写入真实 SPIR-V 字节 | 已验证 |
| `tools/shaders/compile_basic_3d.ps1` | 使用 glslangValidator 编译 basic_3d.vert/frag 到 SPIR-V，找不到工具时输出中文提示并失败 | 可运行 |
| `tools/shaders/validate_basic_3d.ps1` | 使用 spirv-val 验证 basic_3d.vert/frag.spv，找不到工具或文件时输出中文提示并失败 | 可运行 |
| `tools/shaders/README.md` | Shader 编译链文档，包含工具安装步骤、编译/验证命令和废弃 gen_spirv 说明 | 已创建 |
| `tools/shaders/embed_basic_3d_shaders.ps1` | 在 spirv-val 通过后将 .spv 写入 CompiledShaders.cs，独立执行 spirv-val 并检查魔数 | 可运行 |
| `FluidWarfare.Tests/Render/Vulkan/Shaders/CompiledShadersTests.cs` | 验证 CompiledShaders 的字节存在性、SPIR-V 魔数和验证状态 | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dInfoTests.cs` | 验证 3D 场景渲染结果模型的基础语义 | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dVertexTests.cs` | 验证网格生成、立方体生成、坐标轴生成和交错格式转换 | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Scene3D/GroundCursor/VulkanGroundCursorStateTests.cs` | 验证 Ground Cursor 状态：相同坐标 NoOp、Revision、显示/隐藏 | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Camera/ProjectionUnprojectionRoundTripTests.cs` | 投影→反投影闭环测试：7 个已知地面点，误差 < 3cm | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Scene3D/VulkanScene3dRunGateTests.cs` | 验证 Scene3D 运行闸门的隔离状态、提示文本和 Ready/Isolated 语义 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dRunGate.cs` | Scene3D 实验渲染路径运行闸门，当前用于阻止未验证 SPIR-V/Pipeline 路径进入 Editor 启动流程 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/Session/VulkanScene3dFrameStatus.cs` | 帧状态枚举：Presented / Skipped / RecreateRequested / Failed | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/VulkanScene3dPushConstants.cs` | Push Constant 布局（MVP 64B + Tint 16B = 80B）与选中/普通色常量 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Camera/VulkanSceneRayBuilder.cs` | 从视口像素坐标经逆 ViewProjection 生成世界空间射线 | 测试通过 |
| `FluidWarfare.Render/Selection/SceneRay.cs` | 世界空间射线（Origin + Normalized Direction） | 测试通过 |
| `FluidWarfare.Render/Selection/SceneAxisAlignedBounds.cs` | 轴对齐包围盒（Center + HalfExtents） | 测试通过 |
| `FluidWarfare.Render/Selection/SceneRayBoundsIntersection.cs` | Slab 法射线-AABB 相交检测 | 测试通过 |
| `FluidWarfare.Render/Selection/RenderScenePickResult.cs` | Picking 结构化结果（IsHit / EntityId / Distance / WorldHitPosition） | 测试通过 |
| `FluidWarfare.Render/Selection/RenderScenePicker.cs` | CPU 线性遍历 RenderScene 选择最近命中 UnitMarker | 测试通过 |
| `FluidWarfare.Render/Selection/Ground/SceneGroundPlane.cs` | 水平地面平面定义（Height），默认 Y=0 | 测试通过 |
| `FluidWarfare.Render/Selection/Ground/SceneGroundHit.cs` | 地面求交结果（IsHit / Distance / WorldPosition） | 测试通过 |
| `FluidWarfare.Render/Selection/Ground/SceneRayGroundIntersection.cs` | 水平地面射线求交数学（t = (H - Oy) / Dy），不依赖 Avalonia/Win32/Vulkan | 测试通过 |
| `FluidWarfare.Render/Selection/Pointer/ScenePointerPickKind.cs` | Picking 结果类型枚举：None / Entity / Ground | 测试通过 |
| `FluidWarfare.Render/Selection/Pointer/ScenePointerPickResult.cs` | 统一 Pointer Picking 结果（Kind + EntityResult + GroundHit） | 测试通过 |
| `FluidWarfare.Render/Selection/Pointer/ScenePointerPicker.cs` | 统一 Picking 调度器：Entity AABB 优先→Ground→None | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportPickInput.cs` | Win32 左键点击检测（阈值 4px），转发 PickRequested 事件 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/Session/Swapchain/VulkanScene3dSwapchainInvariant.cs` | Swapchain 生命周期不变量断言（Active Live=1 / Disposed Live=0） | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/Session/Surface/VulkanScene3dSurfaceFormats.cs` | SurfaceFormatKHR 两阶段枚举 + Incomplete 有限重试（最多 3 次） | 测试通过 |
| `FluidWarfare.Render/Scene/Position/RenderObjectPositionChange.cs` | RenderObject 位置变更记录 | 测试通过 |
| `FluidWarfare.Render/Scene/Position/RenderObjectPositionWriteResult.cs` | RenderObject 位置写入结果 | 测试通过 |
| `FluidWarfare.Render/Scene/Position/RenderSceneObjectPositionWriter.cs` | 替换 RenderScene 中指定 EntityId 的位置 + SelectionBounds | 测试通过 |
| `FluidWarfare.Engine/World/EntityPosition/WorldEntityPositionChange.cs` | World 实体位置变更记录 | 测试通过 |
| `FluidWarfare.Engine/World/EntityPosition/WorldEntityPositionWriteResult.cs` | World 实体位置写入结果 | 测试通过 |
| `FluidWarfare.Engine/World/EntityPosition/WorldEntityPositionWriter.cs` | 对 WorldState 执行位置修改（检查 EntityId + NoOp） | 测试通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/Session/Surface/VulkanScene3dPresentModes.cs` | PresentModeKHR 两阶段枚举 + Incomplete 有限重试（最多 3 次） | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Camera/VulkanCameraInfoTests.cs` | 验证默认相机参数和自定义相机 | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Camera/ProjectionUnprojectionRoundTripTests.cs` | 投影→反投影闭环测试：7 个已知地面点，误差 < 3cm | 测试通过 |
| `FluidWarfare.Tests/Render/Selection/Ground/SceneRayGroundIntersectionTests.cs` | 验证地面求交：向下/平行/背后/起点/自定义高度/射线方程/对角线 | 测试通过 |
| `FluidWarfare.Tests/Render/Selection/Pointer/ScenePointerPickerTests.cs` | 验证统一 Picking：实体优先、地面命中、都未命中、最近实体优先 | 测试通过 |
| `FluidWarfare.Tests/Editor/ViewportGround/EditorGroundPointerStateTests.cs` | 验证地面指针状态：Hover/Commit 独立、相同 NoOp、Revision 递增 | 测试通过 |
| `FluidWarfare.Tests/Editor/EntityTransform/EditorEntityTransformValidationTests.cs` | 验证 Transform 输入校验：合法/空/NaN/Infinity→中文错误 | 测试通过 |
| `FluidWarfare.Tests/Editor/EntityTransform/EditorGroundPlacementStateTests.cs` | 验证放置模式状态：Begin/Complete/Cancel/Revision | 测试通过 |
| `FluidWarfare.Tests/Editor/EntityTransform/EditorWorldDirtyStateTests.cs` | 验证场景修改状态：MarkDirty/Reset/Revision | 测试通过 |
| `FluidWarfare.Tests/Render/Scene/Position/RenderSceneObjectPositionWriterTests.cs` | 验证 RenderScene 位置修改：同步 SelectionBounds/NoOp/其他实体不变 | 测试通过 |
| `FluidWarfare.Tests/Engine/World/EntityPosition/WorldEntityPositionWriterTests.cs` | 验证 World 实体位置写入：存在/不存在/相同位置 NoOp | 测试通过 |
| `FluidWarfare.Render.Vulkan/Validation/VulkanValidationStatus.cs` | Vulkan Validation 启用状态枚举 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Validation/VulkanValidationInfo.cs` | Validation 状态信息，含状态、中文消息和消息数量 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Validation/VulkanValidationOptions.cs` | 从 FW_VULKAN_VALIDATION 环境变量读取是否请求启用 Validation | 测试通过 |
| `FluidWarfare.Render.Vulkan/Validation/VulkanValidationMessageInfo.cs` | 单条 Validation 消息（严重级别/类型/文本） | 测试通过 |
| `FluidWarfare.Render.Vulkan/Validation/VulkanValidationMessageStore.cs` | 保存最近 20 条 Validation 消息，线程安全 | 测试通过 |
| `FluidWarfare.Render.Vulkan/Validation/VulkanValidationAvailabilityProbe.cs` | 检测 VK_LAYER_KHRONOS_validation 和 VK_EXT_debug_utils | 测试通过 |
| `FluidWarfare.Render.Vulkan/Validation/VulkanDebugMessengerScope.cs` | 持有 DebugUtilsMessengerEXT 生命周期和 callback delegate | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Validation/VulkanValidationOptionsTests.cs` | 验证环境变量读取 | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Validation/VulkanValidationInfoTests.cs` | 验证 Validation 状态模型 | 测试通过 |
| `FluidWarfare.Tests/Render/Vulkan/Validation/VulkanValidationMessageStoreTests.cs` | 验证消息存储的上限和快照 | 测试通过 |
| `FluidWarfare.Engine/Components/PositionComponent.cs` | 实体位置组件，包装 Vector3d | 测试通过 |
| `FluidWarfare.Engine/Components/DisplayNameComponent.cs` | 实体显示名组件，保存用于 Editor 显示的名称 | 测试通过 |
| `FluidWarfare.Tests/Engine/World/WorldStateTests.cs` | 验证最小 World 实体创建、查询、位置读写与枚举 | 测试通过 |
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
| `FluidWarfare.Tests/Editor/WorldHierarchy/WorldHierarchyTreeBuilderTests.cs` | 验证 WorldHierarchyTreeBuilder 的空树/分组排序/实体排序/祖先映射/后代计数 | 测试通过 |
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
| `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs` | 编辑器布局壳后台逻辑，协调项目加载、World 创建、RenderScene 生成、Vulkan 后端/Instance/Device 探测、Windows 原生视口宿主状态、真实视口尺寸绘制、resize 防抖重绘、实体选择与 UI 显示 | 测试通过 |
| `FluidWarfare.Editor.Windows/Shell/EditorSelection.cs` | 编辑器 GUI 占位选择信息值对象，用于在项目面板、检查器和状态栏之间传递当前选择 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Project/ProjectPanel.axaml` | 编辑器项目面板 UI，使用收紧标题与间距显示当前示例项目名称和外部传入的项目分类 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Project/ProjectContentFolderSelection.cs` | 项目内容目录选择值对象，保存稳定 FolderName、DisplayName 和 ContentKind | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Project/ProjectPanel.axaml.cs` | 项目面板后台逻辑，只负责显示项目名、显示内容目录，并在点击时发出 ProjectContentFolderSelection | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportEntitySummary.cs` | 视口占位显示模型，保存当前选中实体的名称、EntityId、来源路径、位置文本与视觉类型 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportRenderObjectSummary.cs` | 视口 RenderScene 调试列表中的单个渲染对象显示摘要 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportRenderSceneSummary.cs` | 视口 RenderScene 调试列表显示模型，保存多个渲染对象摘要 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportPlaceholderPanel.axaml` | 3D 视口占位界面，显示默认提示、World 为空提示、当前选中实体摘要、Vulkan 后端状态、Vulkan Instance 状态、Vulkan Device 状态与 RenderScene 调试对象列表 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/ViewportPlaceholderPanel.axaml.cs` | 视口占位面板逻辑，显示 Vulkan 后端状态、Vulkan Instance 状态、Vulkan Device 状态、实体摘要与 RenderScene 调试对象 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostState.cs` | Vulkan 视口宿主占位状态枚举 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostInfo.cs` | Vulkan 视口宿主占位显示信息 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostState.cs` | Windows Vulkan 视口子窗口宿主状态枚举 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostInfo.cs` | Windows Vulkan 视口子窗口宿主信息模型，保存状态、平台、HWND、HINSTANCE、真实宽高与中文说明 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs` | 使用 Avalonia NativeControlHost 创建并持有 Windows 原生子窗口 HWND，在 Bounds 变化时同步子窗口尺寸并上报 HostInfoChanged | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportNativeHostInfo.cs` | Vulkan 视口宿主原生窗口句柄信息，描述平台、句柄可用性、HWND、HINSTANCE、真实宽高与中文说明 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostPanel.axaml` | Vulkan 视口宿主显示区域，使用收紧标题栏，包含可伸展原生子窗口、Windows 原生子窗口状态与清屏状态 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/VulkanViewportHostPanel.axaml.cs` | 显示 Vulkan 视口宿主状态，查询 Windows 原生子窗口句柄和真实尺寸，透传 NativeHostInfoChanged，并显示清屏状态 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/LeftDock/ProjectWorldDockPanel.axaml` | 左侧双页签面板 UI：[世界层级] [项目内容] 页签 + 共享搜索栏 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/LeftDock/ProjectWorldDockPanel.axaml.cs` | 双页签切换、搜索独立维护、实体 Reveal 自动切换页签 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/HierarchyVisual/HierarchyNodeViewContract.cs` | IHierarchyNodeView 接口 + HierarchyVisibleRows 静态展开方法 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/HierarchyVisual/HierarchyBranchInfo.cs` | 树干位置记录：Depth、IsLastSibling、AncestorHasNextSibling[] | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/HierarchyVisual/HierarchyBranchCanvas.cs` | OnRender 自绘虚线树干，连续竖线 + 折线，无 Border 拼缝 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/HierarchyVisual/HierarchyNodeRow.axaml` | 共享行 UI：四列 Grid（树干线｜展开按钮｜SVG 图标｜主副文字），使用 Svg.Controls.Skia.Avalonia | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/HierarchyVisual/HierarchyNodeRow.axaml.cs` | 展开按钮 Bubble 事件处理 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyNodeView.cs` | 世界树视图模型，实现 IHierarchyNodeView，按节点类型解析 SVG 图标路径 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyTreeIndex.cs` | 世界树扁平索引构建，返回 NodeViewsById、EntityViewsById、AncestorViewsByEntityId + HierarchyBranchInfo | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyTreePanel.axaml` | 世界层级 ListBox：hierarchyList 样式 + HierarchyNodeRow DataTemplate | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyTreePanel.axaml.cs` | 世界树 ListBox 后台：扁平可见行 / 展开状态恢复 / RevealEntity / 程序化选择熔断 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyTreeViewState.cs` | 世界树状态快照：ExpandedNodeIds、SelectedEntityId、SearchText | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyProgrammaticSelection.cs` | 程序化选择一次性令牌，防止反馈环 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/ProjectContentTree/ProjectContentNodeView.cs` | 项目内容树视图模型，实现 IHierarchyNodeView，按节点类型/扩展名解析语义 SVG 图标 | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/ProjectContentTree/ProjectContentTreeIndex.cs` | 项目内容树扁平索引构建，返回 NodeViewsById、FileViewsByPath + HierarchyBranchInfo | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/ProjectContentTree/ProjectContentTreePanel.axaml` | 项目内容 ListBox：hierarchyList 样式 + HierarchyNodeRow DataTemplate | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/ProjectContentTree/ProjectContentTreePanel.axaml.cs` | 项目内容树 ListBox 后台：扁平可见行 / 展开状态恢复 / 搜索过滤 | 测试通过 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/project.svg` | 项目根 SVG 图标（24×24 viewBox，stroke 矢量） | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/world.svg` | 世界根 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/folder.svg` | 文件夹 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/folder-open.svg` | 展开文件夹 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/folder-closed.svg` | 折叠文件夹 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/file.svg` | 通用文件 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/file-json.svg` | JSON 文件 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/units.svg` | 单位分组 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/unit-entity.svg` | 实体 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/faction.svg` | 派系目录 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/weapon.svg` | 武器目录 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/map.svg` | 地图/地形目录 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/script.svg` | 脚本目录/脚本文件 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/rule.svg` | 规则/触发器目录 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/image.svg` | 图片目录/图片文件 SVG 图标 | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/toggle-plus.svg` | 展开按钮 `+` SVG 图标（方框 24×24） | 已创建 |
| `FluidWarfare.Editor.Windows/Assets/Icons/Hierarchy/toggle-minus.svg` | 折叠按钮 `-` SVG 图标（方框 24×24） | 已创建 |
| `FluidWarfare.Editor/ViewportGround/EditorGroundPointerState.cs` | 平台无关地面指针状态：HoverHit（状态栏）+ CommittedHit（Scene3D 标记）+ Revision | 测试通过 |
| `FluidWarfare.Editor/ViewportGround/EditorGroundPointerChange.cs` | 地面指针状态变更结果（IsChanged / IsCommit） | 测试通过 |
| `FluidWarfare.Editor/EntityTransform/EditorEntityTransformDraft.cs` | 检查器 Transform 草稿（EntityId + X/Y/Z Text + IsDirty） | 测试通过 |
| `FluidWarfare.Editor/EntityTransform/EditorEntityTransformValidation.cs` | Transform 输入校验（空/NaN/Infinity→中文错误） | 测试通过 |
| `FluidWarfare.Editor/EntityTransform/EditorEntityTransformChange.cs` | 正式实体位置修改记录（Previous/Current + Origin + Revision） | 测试通过 |
| `FluidWarfare.Editor/EntityTransform/EditorGroundPlacementState.cs` | 地面放置模式状态（IsActive/TargetEntityId） | 测试通过 |
| `FluidWarfare.Editor/EntityTransform/EditorWorldDirtyState.cs` | 场景修改状态跟踪（IsDirty/LastChangedEntityId/Revision） | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/WorldEntities/WorldEntityListPanel.axaml` | World 实体列表面板 UI，使用收紧标题与间距显示当前 World 实体列表 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/WorldEntities/WorldEntityListPanel.axaml.cs` | World 实体列表面板后台逻辑，接收 WorldEntityInfo 列表并在点击时发出 EntitySelected 事件 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorPanel.axaml` | 检查器面板占位，使用收紧标题与间距显示未选择对象或当前选择详情 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorPanel.axaml.cs` | 检查器面板 code-behind | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Logging/LogPanel.axaml` | 编辑器日志面板 UI，去掉内部重复标题，使用占满页签内容区的只读文本区域显示中文日志，支持滚动查看与复制 | 可运行 |
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
| `FluidWarfare.Editor.Windows/Shell/EditorShell.axaml.cs` | **Editor 主壳（重构后）** 3,041→970 行，移除 -2,071 行职责到 26+ Route 类 | 测试通过 / Build 0 Error |
| `FluidWarfare.Editor.Windows/Shell/Composition/EditorShellRouteSet.cs` | 聚合 ~26 个 Route 引用，EditorShell 的唯一 Route 容器 | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Composition/EditorShellRouteBuild.cs` | 构造 Route 的 Factory，负责 Route 初始化顺序 | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Composition/EditorShellControlRefs.cs` | FindControls 结果的容器，保存所有 Axaml 控件引用 | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Lifecycle/EditorShellAttachRoute.cs` | EditorShell 挂载事件处理（OnAttachedToVisualTree 等） | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Startup/EditorStartupBootstrapRoute.cs` | 编辑器启动引导路由：项目加载→World 引导→Vulkan 初始化 | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Input/EditorViewportInputRoute.cs` | 视口输入路由：原始事件 → EditorInputMatch → 分发给各 Route | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Input/Transform/EditorTransformInputRoute.cs` | 变换工具输入路由：导航/场景工具/原始拾取仲裁 | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Input/Picking/EditorPickInputRoute.cs` | 3D 拾取输入路由：左键点击 → CPU Ray-AABB Picking | 可运行 |
| `FluidWarfare.Editor.Windows/Viewport/Selection/Route/EditorSelectionRoute.cs` | 编辑器全局选择路由：3D Picking / 面板点击 / 程序化选择 | 可运行 |
| `FluidWarfare.Editor.Windows/Viewport/Scene3D/Frame/Scene3dFrameRoute.cs` | Scene3D 帧路由：DrawList → Submit → Present | 可运行 |
| `FluidWarfare.Editor.Windows/Viewport/Scene3D/Lifecycle/Scene3dSessionLifecycle.cs` | Scene3D Session 生命周期管理 | 可运行 |
| `FluidWarfare.Editor.Windows/Viewport/Transform/Interaction/TransformPointerRoute.cs` | 变换拖拽交互路由（鼠标 + Gizmo HitTest） | 可运行 |
| `FluidWarfare.Editor.Windows/Viewport/Transform/Drag/TransformDragRoute.cs` | 变换拖拽路由（轴/平面锚定 + 约束 + 求解） | 可运行 |
| `FluidWarfare.Editor.Windows/Viewport/Camera/ViewportCameraRoute.cs` | 视口相机路由（缩放/旋转/平移/帧选） | 可运行 |
| `FluidWarfare.Editor.Windows/Viewport/Navigation/ViewportNavigationRoute.cs` | 视口导航路由（导航覆盖层交互） | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Scene3D/Commands/EditorScene3dCommandRoute.cs` | Scene3D 命令路由（开始/停止 Session） | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Windows/EditorShellWindowRoute.cs` | 窗口命令路由（最小化/最大化/关闭） | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Transform/EditorTransformApplyRoute.cs` | 变换应用路由（Apply/Reset/GroundPlace） | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Transform/EditorGroundPlacementRoute.cs` | 地面放置路由 | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Panels/EditorPanelApplyRoute.cs` | 面板操作应用路由 | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Diagnostics/EditorDiagnosticsRefreshRoute.cs` | Diagnostics 刷新路由 | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Feedback/EditorFeedbackRoute.cs` | 简短反馈路由（状态栏消息） | 可运行 |
| `FluidWarfare.Editor.Windows/Shell/Menu/EditorRunMenuRoute.cs` | 运行菜单路由 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorPanel.axaml.cs` | **InspectorPanel（重构后）** 387→84 行 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorSelectionView.cs` | Inspector 选择状态面板可见性控制 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorTransformView.cs` | Inspector Transform 输入框组（X/Y/Z + Apply/Reset/GroundPlace） | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorTransformBinder.cs` | Inspector Transform 键盘事件绑定（Enter/Esc） | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Inspector/InspectorScrubInput.cs` | Inspector 拖拽微调输入处理 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyTreePanel.axaml.cs` | **WorldHierarchy（重构后）** 229→95 行 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyNodeView.cs` | 世界层级节点视图模型（图标/名称/实体 ID） | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyTreeSelection.cs` | 世界层级树选择状态管理 + 程序化选择防回环 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyTreeIndex.cs` | 世界层级扁平索引（EntityId ↔ NodeId O(1) 查询） | 可运行 (112 行，超线) |
| `FluidWarfare.Editor.Windows/Panels/WorldHierarchy/WorldHierarchyTreeExpansion.cs` | 世界层级展开/折叠状态管理 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/ProjectContentTree/ProjectContentTreePanel.axaml.cs` | **ProjectContent（重构后）** 168→83 行 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/ProjectContentTree/ProjectContentNodeView.cs` | 项目内容节点视图模型（SVG 图标路径解析） | 可运行 (114 行，超线) |
| `FluidWarfare.Editor.Windows/Panels/ProjectContentTree/ProjectContentTreeIndex.cs` | 项目内容树扁平索引（路径 ↔ NodeId 查询） | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/LeftDock/ProjectWorldDockPanel.axaml.cs` | **ProjectWorldDock（重构后）** 219→76 行 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/LeftDock/ProjectWorldDockTabs.cs` | 项目/世界页签切换抽象 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs` | **NativeHost（重构后）** 605→386 行，待继续拆分 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/Input/NativeViewportPointerMessages.cs` | Win32 消息转 PointerRequest 解析器（WM_MOUSEMOVE/WM_LBUTTONDOWN 等） | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/Input/NativeViewportMouseCapture.cs` | SetCapture/ReleaseCapture P/Invoke 封装 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/Input/NativeViewportMouseTrack.cs` | TrackMouseEvent P/Invoke 封装 | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/Win32ViewportWindowClass.cs` | Win32 窗口类注册（RegisterClass + WndProc 委托） | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/NativeHost/WindowsVulkanViewportPickInput.cs` | 视口点击检测（拖拽阈值过滤） | 可运行 |
| `FluidWarfare.Editor.Windows/Viewport/Transform/Gizmo/MoveGizmoHitTest.cs` | Move Gizmo 命中检测（轴/平面） | 可运行 |
| `FluidWarfare.Editor.Windows/Viewport/Transform/Gizmo/MoveGizmoInteraction.cs` | Gizmo 交互状态机（悬停/拖拽/释放） | 可运行 |
| `FluidWarfare.Editor.Windows/Viewport/Transform/Presentation/MoveGizmoFrameSource.cs` | Gizmo 帧输入源（Gizmo 可见性/悬停结果/交互状态） | 可运行 |
| `FluidWarfare.Editor.Windows/Panels/Viewport/Input/WindowsViewportInputTranslator.cs` | Win32 原始事件 → EditorInputMatch 翻译器（284 行，最大输入文件） | 可运行 |
| `FluidWarfare.Editor/Input/Runtime/EditorInputService.cs` | 编辑器输入服务（Action/Context/Binding → Match 管线） | 测试通过 |
| `FluidWarfare.Editor/Input/Bindings/EditorInputBindingSet.cs` | 输入绑定集合（Action ↔ Gesture 映射） | 测试通过 |
| `FluidWarfare.Editor/Input/Actions/EditorInputActionCatalog.cs` | 输入动作目录（所有可绑定动作定义） | 已创建 |
| `FluidWarfare.Editor/Selection/EditorEntitySelectionState.cs` | 编辑器选择状态（EntityId/Origin/SourcePanel） | 测试通过 |
| `FluidWarfare.Editor/Transform/Edit/TransformEditSession.cs` | 变换编辑 Session（轻量状态机: Idle→Started→Committing/Cancelling） | 测试通过 |
| `FluidWarfare.Editor/Transform/Translation/Axis/AxisTranslationSolver.cs` | 轴平移求解器（屏幕度量 + 锚定 + 约束 → 增量） | 测试通过 |
| `FluidWarfare.Editor.Windows/Panels/DebugDock/DebugDockPanel.axaml.cs` | 渲染调试页签（Render Diagnostics / RenderScene / 性能） | 可运行 |
| `FluidWarfare.Tests/Architecture/CodeFileBudgetTests.cs` | **代码宪法自动化测试：** 扫描所有 .cs 文件行数 ≤100、目录文件数 ≤5、禁用 Manager/Helper/Utils 命名 | 625+ 全通过 |
| `FluidWarfare.Render.Vulkan/Scene3D/Overlay/Geometry/VulkanNavigationOverlayGeometry.cs` | 导航覆盖层几何体（Build 入口，308→84 ✅） | 可运行 |
| `FluidWarfare.Render.Vulkan/Scene3D/GroundCursor/VulkanGroundCursorState.cs` | 地面光标状态（鼠标跟踪 + 手动放置两种来源） | 可运行 |
| `GameProjects/SampleProject/units/sample_unit_2.json` | 第二个示例单位占位文件（测试多文件排序稳定性） | 占位 |
| `GameProjects/SampleProject/units/sample_unit_3.json` | 第三个示例单位占位文件 | 占位 |

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
