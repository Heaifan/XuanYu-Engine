# XuanYu Engine 文件树

> 本文仅描述当前仓库结构与文件职责，不记录版本历史、阶段过程、迁移记录或测试统计。

```text
XuanYuEngine/
├── .gitattributes
├── .gitignore
├── NuGet.Config：NuGet 源配置
├── XuanYu.Core
│   ├── .gitkeep
│   ├── Diagnostics
│   │   └── CoreSelfTest.cs
│   ├── Gizmo
│   │   ├── MoveGizmoAxis.cs
│   │   ├── MoveGizmoDragConstraint.Axes.cs
│   │   ├── MoveGizmoDragConstraint.cs
│   │   ├── MoveGizmoLayout.Hit.cs
│   │   ├── MoveGizmoLayout.Plane.cs
│   │   ├── MoveGizmoLayout.cs
│   │   ├── MoveGizmoPlane.cs
│   │   ├── MoveGizmoScreenSize.cs：移动 Gizmo 屏幕尺寸合同
│   │   ├── MoveGizmoSegment.cs
│   │   ├── RotateGizmoAxis.cs
│   │   ├── RotateGizmoDrag.Math.cs
│   │   ├── RotateGizmoDrag.cs
│   │   ├── RotateGizmoLayout.cs
│   │   ├── RotateGizmoRing.cs
│   │   ├── RotateGizmoScreenRadius.cs：旋转 Gizmo 屏幕半径合同
│   │   ├── ScaleGizmoAxis.cs
│   │   ├── ScaleGizmoDrag.cs
│   │   ├── ScaleGizmoHitTester.cs
│   │   ├── ScaleGizmoLayout.cs
│   │   ├── ScaleGizmoScreenSize.cs：缩放 Gizmo 屏幕尺寸合同
│   │   └── ScreenPoint.cs
│   ├── History
│   │   ├── EditorHistoryOwner.cs：通用编辑历史游标与 Undo/Redo 栈
│   │   └── TransformHistoryEntry.cs
│   ├── Identity
│   │   └── EntityId.cs
│   ├── Logging
│   │   ├── EngineLogEntry.cs
│   │   └── EngineLogLevel.cs
│   ├── Map
│   │   ├── MapSurfaceKind.cs
│   │   ├── MapSurfaceSampler.cs：地图地表采样唯一源（世界 X/Y → Z）
│   │   └── MapTerrainVertex.cs：地形网格顶点布局（pos+normal+亮度）
│   ├── Math
│   │   ├── Vector3d.cs
│   │   └── YawRotation.cs
│   ├── Picking
│   │   ├── ViewportPickingRequest.cs
│   │   ├── ViewportPickingResult.cs
│   │   └── ViewportPickingService.cs
│   ├── Properties
│   │   └── AssemblyInfo.cs
│   ├── Results
│   │   ├── EngineError.cs：结构化错误（码 + 中文消息）
│   │   └── EngineResult.cs：结构化操作结果（成功/失败合同）
│   ├── Scene
│   │   ├── CommittedTransform.cs
│   │   ├── ISceneRenderSnapshotSource.cs
│   │   ├── SceneEntitySnapshot.cs
│   │   ├── SceneRenderSnapshot.cs：场景渲染快照合同
│   │   └── SceneTransformCommitResult.cs
│   ├── Space
│   │   ├── CameraState.cs：相机状态（透视/正交、裁剪面、Revision）
│   │   ├── DefaultEditorCamera.cs：默认编辑器相机工厂
│   │   ├── ProjectionMode.cs：相机投影模式（透视/正交）
│   │   ├── ViewProjectionState.cs：视图投影状态（矩阵族）
│   │   ├── ViewportState.cs
│   │   ├── WorldRay.cs：世界射线（逆投影反求）
│   │   └── WorldRayFactory.cs：世界射线工厂（透视/正交兼容）
│   ├── Spatial
│   │   ├── RayAabbHit.cs
│   │   ├── RayAabbIntersection.cs
│   │   ├── SpatialAabb.cs
│   │   ├── SpatialBounds.cs
│   │   ├── SpatialQueryCategory.cs
│   │   ├── SpatialQueryResult.cs
│   │   ├── SpatialQueryStats.cs
│   │   ├── SpatialRayAabb.cs
│   │   ├── SpatialRayQuery.cs
│   │   ├── SpatialRaycastHit.cs
│   │   ├── SpatialRaycastResult.cs
│   │   └── SpatialRaycastStats.cs
│   ├── Time
│   │   ├── SimulationTime.cs
│   │   └── TimeStep.cs
│   ├── Transform
│   │   ├── PreviewTransform.cs
│   │   └── TransformStartSnapshot.cs
│   └── XuanYu.Core.csproj
├── XuanYu.Core.Tests
│   ├── Camera
│   │   ├── CameraBasisTests.cs
│   │   ├── CameraNavigationRollTests.cs
│   │   ├── CameraNavigationSequenceTests.cs
│   │   ├── CameraNavigationStressTests.cs
│   │   ├── CameraNavigationTests.cs
│   │   ├── CameraNavigationUiSequenceTests.Safety.cs
│   │   ├── CameraNavigationUiSequenceTests.cs
│   │   └── CameraOrthographicNavigationTests.cs
│   ├── CoreSmokeTests.cs
│   ├── EditorTool
│   │   └── EditorTransformCapturePolicyTests.cs
│   ├── Gizmo
│   │   ├── MoveGizmoDragConstraintTests.cs
│   │   ├── MoveGizmoLayoutG1Tests.cs
│   │   ├── MoveGizmoLayoutPlaneTests.cs
│   │   ├── MoveGizmoLayoutTests.cs
│   │   ├── MoveGizmoLayoutVulkanTests.cs
│   │   ├── MoveGizmoScreenSizeTests.cs
│   │   ├── RotateGizmoLayoutTests.cs
│   │   ├── ScaleGizmoTests.Drag.cs
│   │   ├── ScaleGizmoTests.DragSafety.cs
│   │   ├── ScaleGizmoTests.Helpers.cs
│   │   ├── ScaleGizmoTests.R5R1.cs
│   │   └── ScaleGizmoTests.cs
│   ├── History
│   │   ├── EditorHistoryOwnerTests.cs
│   │   ├── EditorHistoryRedoTests.cs
│   │   ├── TransformHistoryIntegrationTests.cs
│   │   └── TransformHistoryRedoIntegrationTests.cs
│   ├── Picking
│   │   └── ViewportPickingServiceTests.cs
│   ├── Render
│   │   ├── CubeRenderDrawPlanTests.cs
│   │   ├── FrameExecutionPolicyTests.cs
│   │   ├── MapRenderDrawPlanTests.cs
│   │   ├── MapSurfaceGeometryTests.cs
│   │   ├── NavigationGizmoLayoutTests.Facing.cs
│   │   ├── NavigationGizmoLayoutTests.cs
│   │   ├── NavigationGizmoOverlayContractTests.cs
│   │   ├── ReferenceGridAdaptiveTests.cs
│   │   ├── ReferenceGridDrawPlanTests.cs
│   │   ├── ReferenceGridRayIntersectionTests.cs
│   │   ├── ReferenceGridScaleTests.cs
│   │   ├── ReferenceGridShaderContractTests.cs
│   │   ├── ReferenceGridVisualStyleTests.cs
│   │   ├── RenderDrawPlanTests.cs
│   │   ├── SceneRenderProjectionAdapterTests.Rotation.cs
│   │   ├── SceneRenderProjectionAdapterTests.Selection.cs
│   │   ├── SceneRenderProjectionAdapterTests.cs
│   │   ├── StandardViewResolverTests.cs
│   │   ├── StaticModelDepthRegressionTests.cs
│   │   ├── StaticModelRenderContractTests.cs
│   │   ├── ViewportAssistDrawPlanTests.cs
│   │   └── ViewportChromeContractTests.cs
│   ├── Space
│   │   ├── CameraOrthographicTests.cs
│   │   ├── CameraStateTests.cs
│   │   ├── DefaultEditorCameraTests.cs
│   │   ├── SpaceAssert.cs
│   │   ├── ViewProjectionStateTests.cs
│   │   ├── ViewportStateTests.cs
│   │   ├── WorldRayFactoryTests.cs
│   │   └── WorldRayTests.cs
│   ├── Spatial
│   │   ├── RayAabbIntersectionTests.cs
│   │   ├── SpatialBoundsTests.cs
│   │   └── SpatialTestData.cs
│   └── XuanYu.Core.Tests.csproj
├── XuanYu.Editor
│   ├── Assets
│   │   ├── AssetId.cs
│   │   ├── GlbContainer.cs
│   │   ├── GlbImportService.cs
│   │   ├── GltfAccessorReader.cs
│   │   ├── GltfCoordinatePolicy.cs
│   │   ├── GltfJsonAccess.cs
│   │   ├── GltfNodeTransform.cs
│   │   ├── GltfStaticModelImporter.cs
│   │   ├── HostedSceneAsset.cs
│   │   ├── ImportStop.cs
│   │   ├── ModelAssetRuntimeState.cs
│   │   ├── SceneAssetHostingError.cs
│   │   ├── SceneAssetHostingPlan.cs
│   │   ├── SceneAssetHostingPlanner.cs
│   │   ├── SceneAssetHostingState.cs
│   │   ├── SceneAssetHostingTransaction.Activate.cs
│   │   ├── SceneAssetHostingTransaction.Complete.cs
│   │   ├── SceneAssetHostingTransaction.Rollback.cs
│   │   ├── SceneAssetHostingTransaction.cs
│   │   ├── SceneAssetPathPolicy.cs：场景资产相对路径安全策略
│   │   ├── SceneStaticModelBinding.cs
│   │   ├── SceneStaticModelCatalog.cs
│   │   ├── StaticModelAuthoringService.cs
│   │   ├── StaticModelBuilder.cs
│   │   ├── StaticModelColor.cs
│   │   ├── StaticModelData.cs
│   │   ├── StaticModelImportCodes.cs
│   │   ├── StaticModelImportResult.cs
│   │   ├── StaticModelImportWarning.cs
│   │   ├── StaticModelPrimitive.cs
│   │   └── StaticModelVertex.cs
│   ├── Camera
│   │   ├── CameraBasis.cs：相机正交基生成器（PreferredUp 回退）
│   │   ├── CameraFrameResult.cs
│   │   ├── CameraNavigation.Try.cs：相机导航 Try* 失败安全实现
│   │   ├── CameraNavigation.cs：相机导航（Orbit/Dolly/Pan 安全 API）
│   │   ├── EditorCameraFraming.Orthographic.cs：相机取景（正交分支）
│   │   ├── EditorCameraFraming.cs：相机取景（全部/选中/地图，透视）
│   │   └── OrthographicViewFactory.cs：正交视图构造
│   ├── MapDocument
│   │   ├── MapDocument.cs：地图持久化 DTO（.xymap v1 模型）
│   │   ├── MapDocumentAggregateBridge.cs：v1 DTO → 领域聚合投影（场景引用链）
│   │   ├── MapDocumentJson.cs：地图 DTO JSON 形态
│   │   ├── MapDocumentOwner.cs：地图文档持有者（路径/Dirty/错误）
│   │   ├── MapDocumentResult.cs：地图存储操作结果
│   │   ├── MapDocumentValidator.cs：地图 DTO 校验
│   │   ├── MapEnvironmentDefinition.cs：环境定义（天空预设/太阳/环境光）
│   │   ├── MapJsonMapper.cs：地图 JSON 双向映射
│   │   ├── MapJsonSerializer.cs：地图 JSON 序列化
│   │   └── MapStorageService.cs：地图文件读写（原子替换）
│   ├── MapEditing
│   │   ├── MapEditEvents.cs：低频事件参数（内容/选择/Dirty/历史可用）
│   │   ├── MapEditReason.cs：编辑原因枚举（含原子属性变更）
│   │   ├── MapEditSession.Commands.cs：地图编辑命令（属性/尺寸/高度/原子属性）
│   │   ├── MapEditSession.Commit.cs：地图内容统一提交管线（No-op/校验/历史）
│   │   ├── MapEditSession.Document.cs：文档生命周期（新建/替换/标记已保存）
│   │   ├── MapEditSession.History.cs：Undo/Redo 与事件广播
│   │   ├── MapEditSession.Selection.cs：选择状态（稳定 ID + 规范化）
│   │   ├── MapEditSession.cs：地图编辑会话状态权威
│   │   ├── MapHistoryEntry.cs：历史条目（Before/After 不可变快照）
│   │   ├── MapSelection.cs：地图选择模型（None/Map/Layer/Region）
│   │   └── MapSelectionKind.cs：地图选择类型
│   ├── SceneDocument
│   │   ├── MapReference.cs：场景地图引用记录
│   │   ├── SceneDocumentAsset.cs
│   │   ├── SceneDocumentEntity.cs
│   │   ├── SceneDocumentJson.cs：场景文档 JSON 形态
│   │   ├── SceneDocumentLoadTransaction.cs：场景加载事务（候选构建）
│   │   ├── SceneDocumentMapper.cs：场景文档 JSON 双向映射
│   │   ├── SceneDocumentResult.cs
│   │   ├── SceneDocumentSaveTransaction.cs：场景保存事务（临时文件+原子替换）
│   │   ├── SceneDocumentSession.cs：场景文档会话（Dirty/Revision）
│   │   ├── SceneDocumentSnapshot.cs：场景文档快照（含可选地图引用）
│   │   ├── SceneDocumentValidator.MapReference.cs：mapReference 校验（可空）
│   │   ├── SceneDocumentValidator.cs：场景文档校验（含 mapReference）
│   │   ├── SceneDocumentWorldBridge.cs
│   │   ├── SceneLoadCandidate.cs
│   │   ├── SceneSaveOutcome.cs
│   │   └── SceneStorageService.cs：场景文件读写（原子保存）
│   ├── Transform
│   │   ├── TransformSession.Rotate.cs
│   │   ├── TransformSession.Scale.cs
│   │   └── TransformSession.cs
│   └── XuanYu.Editor.csproj
├── XuanYu.Editor.App
│   ├── EditorCompositionRoot.cs
│   ├── Program.cs
│   └── XuanYu.Editor.App.csproj
├── XuanYu.Editor.UI
│   ├── Bootstrap
│   │   ├── App.axaml
│   │   ├── App.axaml.cs
│   │   └── Program.cs
│   ├── Dialogs
│   │   ├── IEditorDialogService.cs
│   │   └── NullEditorDialogService.cs
│   ├── EditorState
│   │   ├── EditorInteractionChangedResult.cs
│   │   ├── EditorInteractionCommand.cs
│   │   ├── EditorInteractionPointerSnapshot.cs
│   │   ├── EditorInteractionSnapshot.cs
│   │   ├── EditorSelectionCommand.cs
│   │   ├── EditorSelectionSnapshot.cs
│   │   ├── EditorStateChangedResult.cs
│   │   ├── EditorStateOwner.Interaction.cs
│   │   ├── EditorStateOwner.Tool.cs
│   │   ├── EditorStateOwner.cs
│   │   ├── EditorToolChangedResult.cs
│   │   ├── EditorToolCommand.cs
│   │   ├── EditorToolId.cs
│   │   ├── EditorToolSnapshot.cs
│   │   ├── EditorToolText.cs
│   │   └── EditorTransformCapturePolicy.cs
│   ├── Foot
│   │   ├── Foot.axaml
│   │   ├── Foot.axaml.cs
│   │   ├── LogDetailPanel.axaml
│   │   ├── LogDetailPanel.axaml.cs
│   │   └── LogListAutoScrollController.cs
│   ├── Icons
│   │   └── EditorIcons.axaml
│   ├── Left
│   │   ├── InlineRenameActivation.cs
│   │   ├── Left.EntityCommands.cs
│   │   ├── Left.Styles.axaml
│   │   ├── Left.axaml
│   │   └── Left.axaml.cs
│   ├── Main
│   │   ├── Main.axaml
│   │   └── Main.axaml.cs
│   ├── NativeHostResizeCoalescer.cs
│   ├── NativeHostResizeSnapshot.cs
│   ├── NativeHostSurfaceContract.cs
│   ├── RelayCommand.cs
│   ├── Right
│   │   ├── MapEditorPanel.axaml：地图编辑器面板（资产区 + 属性编辑区）
│   │   ├── MapEditorPanel.axaml.cs
│   │   ├── Right.axaml：右侧面板（检查器/地图编辑器等 Tab）
│   │   └── Right.axaml.cs
│   ├── Root
│   │   ├── UiRoot.axaml：编辑器根布局
│   │   └── UiRoot.axaml.cs
│   ├── Top
│   │   ├── Top.axaml
│   │   └── Top.axaml.cs
│   ├── TreeGuide.cs
│   ├── TreeGuideSegment.cs
│   ├── Ui.axaml
│   ├── Viewport
│   │   ├── ViewNavigationGizmo.HitTest.cs
│   │   ├── ViewNavigationGizmo.Layout.cs
│   │   └── Vulkan
│   │       ├── NativePointerMessage.cs
│   │       ├── VulkanNativeHost.AvaloniaCamera.cs
│   │       ├── VulkanNativeHost.AvaloniaPointer.cs
│   │       ├── VulkanNativeHost.Bridge.cs
│   │       ├── VulkanNativeHost.CameraPointer.cs
│   │       ├── VulkanNativeHost.Dpi.cs
│   │       ├── VulkanNativeHost.Gizmo.cs
│   │       ├── VulkanNativeHost.LayoutSync.cs
│   │       ├── VulkanNativeHost.Log.cs
│   │       ├── VulkanNativeHost.NavGizmo.cs
│   │       ├── VulkanNativeHost.Picking.cs
│   │       ├── VulkanNativeHost.Pointer.cs
│   │       ├── VulkanNativeHost.cs
│   │       ├── VulkanViewport.axaml：视口宿主布局
│   │       ├── VulkanViewport.axaml.cs
│   │       ├── Win32ViewportHost.Input.cs
│   │       └── Win32ViewportHost.cs
│   ├── ViewportNativeHostRoute.cs
│   ├── Vm
│   │   ├── CameraSessionMode.cs
│   │   ├── CameraSessionSnapshot.cs
│   │   ├── D2StaticModelDemo.cs
│   │   ├── DebugText.cs
│   │   ├── EditorDisplayText.cs
│   │   ├── EditorLogCategory.cs
│   │   ├── EditorLogLevel.cs
│   │   ├── EditorLogSource.cs
│   │   ├── EditorTreeNode.cs
│   │   ├── LogEntry.cs
│   │   ├── Logging
│   │   │   ├── EditorLogBuffer.cs
│   │   │   ├── EditorLogBus.cs
│   │   │   ├── EditorLogClipboardText.cs
│   │   │   ├── EditorLogFilter.cs
│   │   │   ├── EditorLogFilterQuery.cs
│   │   │   ├── EditorLogNoiseFilter.cs
│   │   │   ├── EditorLogRepeatKey.cs
│   │   │   └── EditorLogSummary.cs
│   │   ├── MapRenderSnapshotProjection.cs：MapDefinition → 渲染快照纯投影
│   │   ├── SampleLogEntries.cs
│   │   ├── SceneHistoryEntry.cs
│   │   ├── SceneRenderProjectionAdapter.cs：场景快照 → 渲染投影适配（含地图）
│   │   ├── StandardViewResolver.cs
│   │   ├── StaticModelRenderAdapter.cs
│   │   ├── TreeGuideBuilder.cs
│   │   ├── UiText.cs
│   │   ├── UiVm.Camera.Framing.cs：相机取景命令
│   │   ├── UiVm.Camera.cs：相机操作与投影发布
│   │   ├── UiVm.CameraNavigation.cs：相机导航命令接入
│   │   ├── UiVm.DocumentStatus.cs
│   │   ├── UiVm.EntityCommands.cs
│   │   ├── UiVm.History.Entities.cs
│   │   ├── UiVm.History.cs
│   │   ├── UiVm.InputGuards.cs
│   │   ├── UiVm.Inspector.cs
│   │   ├── UiVm.InspectorInput.Parse.cs
│   │   ├── UiVm.InspectorInput.cs
│   │   ├── UiVm.Interaction.cs
│   │   ├── UiVm.InteractionCancel.cs
│   │   ├── UiVm.InteractionPointer.cs
│   │   ├── UiVm.Logging.cs
│   │   ├── UiVm.MapEditor.cs：地图属性入口（宽/深/基础高度原子应用）
│   │   ├── UiVm.MapRender.cs：会话 ContentChanged → 渲染快照适配
│   │   ├── UiVm.MapWorld.cs：World 地图查询状态与取景
│   │   ├── UiVm.MoveGizmo.cs
│   │   ├── UiVm.MoveGizmoLogging.cs
│   │   ├── UiVm.MoveGizmoScreenSize.cs
│   │   ├── UiVm.NativeHostLifecycle.cs
│   │   ├── UiVm.Picking.cs
│   │   ├── UiVm.RenderProjection.cs：渲染投影创建（含地图快照）
│   │   ├── UiVm.RotateGizmo.cs
│   │   ├── UiVm.ScaleGizmo.cs
│   │   ├── UiVm.Scene.cs：场景命令分发与渲染快照发布
│   │   ├── UiVm.SceneDocument.New.cs
│   │   ├── UiVm.SceneDocument.cs：场景文档状态与窗口标题
│   │   ├── UiVm.SceneDocumentLog.cs
│   │   ├── UiVm.SceneDocumentMapRef.cs：场景地图引用闭环（保存附加/打开恢复）
│   │   ├── UiVm.SceneDocumentSave.cs
│   │   ├── UiVm.Selection.cs
│   │   ├── UiVm.SelectionProjection.cs
│   │   ├── UiVm.SelectionTrace.cs
│   │   ├── UiVm.SelectionValidity.cs
│   │   ├── UiVm.StaticModelImport.cs
│   │   ├── UiVm.Tool.cs
│   │   ├── UiVm.TreeCommands.cs
│   │   ├── UiVm.ViewGizmo.cs：视角 Gizmo 状态与观察中心
│   │   ├── UiVm.ViewportAssist.cs：视口辅助开关命令
│   │   ├── UiVm.ViewportSelection.cs
│   │   ├── UiVm.WorldProjection.cs
│   │   ├── UiVm.cs：组合根视图模型（会话/命令/快照发布）
│   │   └── ViewportPickingLogFormatter.cs
│   ├── Win
│   │   ├── UiWin.Dialogs.cs
│   │   ├── UiWin.EntityShortcuts.cs
│   │   ├── UiWin.MapCommands.cs：地图命令分发（新建/聚焦/应用属性）
│   │   ├── UiWin.SceneCommands.cs
│   │   ├── UiWin.UnsavedDialog.cs
│   │   ├── UiWin.axaml：主窗口（标题含版本号）
│   │   └── UiWin.axaml.cs
│   ├── XuanYu.Editor.UI.csproj
│   └── app.manifest
├── XuanYu.Editor.Win
│   ├── MainForm.cs
│   └── XuanYu.Editor.Win.csproj
├── XuanYu.Engine.slnx：解决方案入口
├── XuanYu.Render.Abstractions
│   ├── EditorViewPlaneGridKind.cs：视图平面网格类型（YZ/XZ）
│   ├── EditorViewportAssistState.cs：视口辅助状态合同
│   ├── FrameExecutionPolicy.cs
│   ├── INativeHostSurfaceBridge.cs
│   ├── INativeHostSurfaceBridgeFactory.cs
│   ├── IRenderProjectionSource.cs
│   ├── MapBoundsGeometry.cs：地图边界几何（四条边细条四边形）
│   ├── MapRenderSnapshot.cs：地图渲染不可变快照（唯一渲染输入）
│   ├── MapSurfaceGeometry.cs：有限地面常量几何（4 顶点 6 索引）
│   ├── NativeHostHandleSnapshot.cs
│   ├── NativeHostLifecycleLogFormatter.cs
│   ├── NativeHostLifecycleProbe.cs
│   ├── NativeHostLifecycleState.cs
│   ├── NativeHostSurfaceHandle.cs
│   ├── ReferenceGridScale.cs
│   ├── RenderCameraProjection.cs：相机渲染投影合同
│   ├── RenderDrawPlan.cs：帧绘制计划（顺序合同）
│   ├── RenderEntityProjection.cs：实体渲染投影合同
│   ├── RenderEntityType.cs
│   ├── RenderProjection.cs：渲染投影（相机/实体/Gizmo/地图快照）
│   ├── RenderProjectionResult.cs
│   ├── RenderStaticModelKey.cs
│   ├── RenderStaticModelPrimitive.cs
│   ├── RenderStaticModelResource.cs：静态模型渲染资源合同
│   ├── RenderStaticModelVertex.cs
│   └── XuanYu.Render.Abstractions.csproj
├── XuanYu.Render.Vulkan
│   ├── Bridge
│   │   ├── VulkanBridgeDeviceAttachStep.cs
│   │   ├── VulkanBridgePhysicalDeviceAttachStep.cs
│   │   ├── VulkanBridgeRenderSessionAttachStep.cs
│   │   └── VulkanBridgeSwapchainAttachStep.cs
│   ├── Device
│   │   ├── VulkanDeviceOwner.Physical.cs
│   │   ├── VulkanDeviceOwner.cs
│   │   ├── VulkanPhysicalDeviceInfo.cs
│   │   ├── VulkanPhysicalDeviceSelection.cs
│   │   ├── VulkanPhysicalDeviceSelector.cs
│   │   └── VulkanQueueFamilySelection.cs
│   ├── Diagnostic
│   │   └── VulkanResizeTracer.cs
│   ├── Pipeline
│   │   ├── ShaderBytecode.Frag.cs：scene.frag 字节码（生成物）
│   │   ├── ShaderBytecode.GridFrag.cs：参考网格片元字节码（生成物）
│   │   ├── ShaderBytecode.GridVert.cs：网格顶点字节码（生成物）
│   │   ├── ShaderBytecode.NavGizmoFrag.cs：导航 Gizmo 片元字节码（生成物）
│   │   ├── ShaderBytecode.NavGizmoVert.cs：导航 Gizmo 顶点字节码（生成物）
│   │   ├── ShaderBytecode.Vert.cs：scene.vert 字节码（生成物）
│   │   ├── ShaderBytecode.ViewPlaneGridFrag.cs：视图平面网格片元字节码（生成物）
│   │   ├── ShaderBytecode.WorldAxesFrag.cs：世界轴片元字节码（生成物）
│   │   ├── ShaderBytecode.WorldOriginFrag.cs：世界原点片元字节码（生成物）
│   │   ├── VulkanGraphicsPipelineOwner.Depth.cs
│   │   ├── VulkanGraphicsPipelineOwner.Fullscreen.cs：全屏 Pass 泛化创建
│   │   ├── VulkanGraphicsPipelineOwner.Grid.cs：网格 Pass 管线创建
│   │   ├── VulkanGraphicsPipelineOwner.Sky.cs：天空管线创建
│   │   ├── VulkanGraphicsPipelineOwner.StaticModelInput.cs
│   │   ├── VulkanGraphicsPipelineOwner.cs：主管线创建（scene.vert/frag）
│   │   ├── VulkanPipelineLogFormatter.cs
│   │   ├── VulkanScenePushConstants.cs：场景 PushConstants 布局
│   │   └── VulkanShaderModuleOwner.cs
│   ├── Render
│   │   ├── StaticModels
│   │   │   ├── VulkanStaticModelBuffer.cs
│   │   │   ├── VulkanStaticModelCache.cs
│   │   │   ├── VulkanStaticModelFailureTracker.cs
│   │   │   ├── VulkanStaticModelLog.cs
│   │   │   ├── VulkanStaticModelResource.cs
│   │   │   ├── VulkanStaticModelValidator.cs
│   │   │   └── VulkanStaticModelVertex.cs
│   │   ├── VulkanClearFrameLogFormatter.cs
│   │   ├── VulkanClearFrameOwner.Commands.cs
│   │   ├── VulkanClearFrameOwner.Draw.cs：帧绘制分发（DrawPlan 消费）
│   │   ├── VulkanClearFrameOwner.DrawAssist.cs
│   │   ├── VulkanClearFrameOwner.DrawGizmo.cs
│   │   ├── VulkanClearFrameOwner.DrawStaticBounds.cs
│   │   ├── VulkanClearFrameOwner.DrawStaticModel.cs
│   │   ├── VulkanClearFrameOwner.Grid.cs：参考网格 Pass（192B push）
│   │   ├── VulkanClearFrameOwner.GridScale.cs：网格尺度计算与 push 填充
│   │   ├── VulkanClearFrameOwner.Lifecycle.cs：GPU 资源生命周期（释放）
│   │   ├── VulkanClearFrameOwner.MapSurface.cs：地图地面/边界 GPU 资源与绘制（策略判等）
│   │   ├── VulkanClearFrameOwner.Matrix.cs
│   │   ├── VulkanClearFrameOwner.NavGizmo.cs
│   │   ├── VulkanClearFrameOwner.PipelineBind.cs：DrawKind → 管线绑定
│   │   ├── VulkanClearFrameOwner.PushConstants.cs
│   │   ├── VulkanClearFrameOwner.Resources.cs
│   │   ├── VulkanClearFrameOwner.Scene.cs：场景投影注入与网格尺度更新
│   │   ├── VulkanClearFrameOwner.Trace.cs
│   │   ├── VulkanClearFrameOwner.ViewPlaneGrid.cs：视图平面网格 Pass
│   │   ├── VulkanClearFrameOwner.WorldAxes.cs
│   │   ├── VulkanClearFrameOwner.cs：清屏/场景绘制所有者
│   │   ├── VulkanDepthAttachment.cs
│   │   ├── VulkanPresentLoop.Frame.cs
│   │   ├── VulkanPresentLoop.Lifecycle.cs
│   │   └── VulkanPresentLoop.cs
│   ├── Session
│   │   ├── GridPipelineSet.cs：网格管线组（参考网格/轴/原点/视图平面）
│   │   ├── VulkanRenderSession.Lifecycle.cs
│   │   ├── VulkanRenderSession.Recover.cs
│   │   ├── VulkanRenderSession.Resize.cs
│   │   └── VulkanRenderSession.cs
│   ├── Shaders
│   │   ├── editor_nav_gizmo.frag：导航 Gizmo 片元（用户权威终版）
│   │   ├── editor_nav_gizmo.vert：导航 Gizmo 顶点
│   │   ├── editor_reference_grid.frag：参考网格片元（地图对齐/边缘淡出）
│   │   ├── editor_reference_grid.vert：参考网格顶点（全屏三角形）
│   │   ├── editor_view_plane_grid.frag：视图平面网格片元
│   │   ├── editor_world_axes.frag：世界轴片元
│   │   ├── editor_world_origin.frag：世界原点屏幕空间标记片元
│   │   ├── scene.frag：场景片元着色器（天空/参考地面）
│   │   └── scene.vert：场景顶点着色器（地表/边界/Gizmo 分支）
│   ├── Swapchain
│   │   ├── VulkanSwapchainBuilder.cs
│   │   ├── VulkanSwapchainCapabilities.cs
│   │   ├── VulkanSwapchainLogFormatter.cs
│   │   ├── VulkanSwapchainOwner.Accessors.cs
│   │   └── VulkanSwapchainOwner.cs
│   ├── VulkanApiProbe.cs
│   ├── VulkanBridgeLogFormatter.cs
│   ├── VulkanDeviceInfo.cs
│   ├── VulkanInstanceCreateInfoBuilder.cs
│   ├── VulkanInstanceExtensions.cs
│   ├── VulkanInstanceLogFormatter.cs
│   ├── VulkanInstanceOwner.cs
│   ├── VulkanInstanceResult.cs
│   ├── VulkanNativeHostSurfaceBridge.Attach.cs
│   ├── VulkanNativeHostSurfaceBridge.Lifecycle.cs
│   ├── VulkanNativeHostSurfaceBridge.Resize.cs
│   ├── VulkanNativeHostSurfaceBridge.Scene.cs
│   ├── VulkanNativeHostSurfaceBridge.cs
│   ├── VulkanNativeHostSurfaceBridgeFactory.cs
│   ├── VulkanProbeLogFormatter.cs
│   ├── VulkanProbeResult.cs
│   ├── VulkanSurfaceLogFormatter.cs
│   ├── VulkanSurfaceOwner.cs
│   ├── VulkanSurfaceResult.cs
│   └── XuanYu.Render.Vulkan.csproj
├── XuanYu.WarCore
│   ├── Identity
│   │   ├── FactionId.cs
│   │   ├── MilitaryIdentity.cs
│   │   ├── OrganizationId.cs
│   │   ├── UnitId.cs
│   │   └── UnitKind.cs
│   ├── State
│   │   └── SoldierState.cs
│   └── XuanYu.WarCore.csproj
├── XuanYu.WarCore.Tests
│   ├── Identity
│   │   └── MilitaryIdentityTests.cs
│   ├── State
│   │   └── SoldierStateTests.cs
│   ├── WarCoreDependencyTests.cs
│   └── XuanYu.WarCore.Tests.csproj
├── XuanYu.World
│   ├── EntityRegistry.Authoring.cs
│   ├── EntityRegistry.Replace.cs
│   ├── EntityRegistry.cs
│   ├── GlobalWorld.Authoring.cs
│   ├── GlobalWorld.Query.cs
│   ├── GlobalWorld.Snapshot.cs
│   ├── GlobalWorld.cs
│   ├── GridWorldPartitionStrategy.cs
│   ├── IWorldPartitionStrategy.cs
│   ├── Map
│   │   ├── MapBounds.cs：地图边界合同（中心原点闭区间）
│   │   ├── MapDefaultDefinition.cs：默认地图工厂（10 km×10 km Flat）
│   │   ├── MapDefinition.cs：地图领域聚合根（唯一权威）
│   │   ├── MapDefinitionValidator.cs：地图聚合严格校验
│   │   ├── MapGeometry.cs：地图几何值类型（尺寸/坐标系统/点）
│   │   ├── MapId.cs：地图稳定唯一标识（32 位十六进制）
│   │   ├── MapLayer.cs：地图图层领域模型
│   │   ├── MapLayerId.cs：图层稳定唯一标识
│   │   ├── MapLayerKind.cs：图层角色（Base/Region/Custom）
│   │   ├── MapLayerValidator.cs：图层集合校验（基础层唯一且第 0 位）
│   │   ├── MapRegion.cs：正式闭合区域领域模型
│   │   ├── MapRegionDraft.cs：绘制中区域草稿（CanClose/Close）
│   │   ├── MapRegionId.cs：区域稳定唯一标识
│   │   ├── MapRegionKind.cs：区域类型（Generic/Playable 等）
│   │   ├── MapRegionValidator.cs：区域严格校验（顶点/面积/图层/边界）
│   │   ├── MapSurfaceDefinition.cs：地表定义（Flat/GentleHillsV1）
│   │   ├── MapValidationResult.cs：领域验证结构化结果
│   │   ├── WorldMapState.cs：World 地图状态（边界判断/高度查询/聚合投影）
│   │   └── WorldMapStateOwner.cs：当前 World 地图状态所有者
│   ├── RegionKey.cs
│   ├── Scene
│   │   ├── SceneSpatialBoundsProjection.cs
│   │   ├── SceneStateOwner.Lifecycle.cs
│   │   ├── SceneStateOwner.Seeding.cs
│   │   ├── SceneStateOwner.StaticModel.cs
│   │   ├── SceneStateOwner.Transform.cs
│   │   ├── SceneStateOwner.cs
│   │   └── SceneWorldProjection.cs
│   ├── Spatial
│   │   ├── DynamicAabbTree.Insert.cs
│   │   ├── DynamicAabbTree.Node.cs
│   │   ├── DynamicAabbTree.Query.cs
│   │   ├── DynamicAabbTree.Refit.cs
│   │   ├── DynamicAabbTree.Remove.cs
│   │   ├── DynamicAabbTree.cs
│   │   ├── ISpatialIndex.cs
│   │   ├── SpatialIndexOwner.cs
│   │   └── SpatialRaycastResolver.cs
│   ├── WorldEntityActivity.cs
│   ├── WorldEntityName.cs
│   ├── WorldEntitySnapshot.cs
│   ├── WorldEntityType.cs
│   ├── WorldPartitionEntry.cs
│   ├── WorldPartitionMembership.cs
│   ├── WorldQuery.cs
│   └── XuanYu.World.csproj
├── XuanYu.World.Tests
│   ├── Assets
│   │   ├── HostingTestEnv.cs
│   │   ├── ScenePersistenceEnv.cs
│   │   ├── WorldCR4D4DialogTests.cs
│   │   ├── WorldCR4D4HostingCompleteTests.cs
│   │   ├── WorldCR4D4HostingPlannerRejectTests.cs
│   │   ├── WorldCR4D4HostingPlannerTests.cs
│   │   ├── WorldCR4D4HostingRollbackTests.cs
│   │   ├── WorldCR4D4HostingSaveAsTests.cs
│   │   ├── WorldCR4D4HostingTransactionTests.cs
│   │   ├── WorldCR4D4LoadStructureErrorTests.cs
│   │   ├── WorldCR4D4LoadTransactionTests.cs
│   │   ├── WorldCR4D4SaveAsTests.cs
│   │   ├── WorldCR4D4SaveTransactionTests.cs
│   │   └── WorldCR4D4SchemaCompatibilityTests.cs
│   ├── Map
│   │   ├── MapBoundsTests.cs
│   │   ├── MapCoordinateValidationTests.cs
│   │   ├── MapDefaultMapTests.cs
│   │   ├── MapDefinitionTests.cs
│   │   ├── MapDocumentAggregateBridgeTests.cs
│   │   ├── MapDocumentOwnerChainTests.cs
│   │   ├── MapDocumentOwnerTests.cs
│   │   ├── MapEnvironmentValidationTests.cs
│   │   ├── MapIdTests.cs
│   │   ├── MapJsonRoundTripTests.cs
│   │   ├── MapJsonStrictnessTests.cs
│   │   ├── MapLayerTests.Base.cs
│   │   ├── MapLayerTests.cs
│   │   ├── MapRegionDraftTests.cs
│   │   ├── MapRegionTests.Helpers.cs
│   │   ├── MapRegionTests.Strictness.cs
│   │   ├── MapRegionTests.cs
│   │   ├── MapSizeValidationTests.cs
│   │   ├── MapStorageFailureTests.cs
│   │   ├── MapStorageTests.cs
│   │   ├── MapSurfaceSamplerTests.cs
│   │   ├── MapSurfaceValidationTests.cs
│   │   ├── WorldMapStateOwnerTests.cs
│   │   └── WorldMapStateTests.cs
│   ├── MapEditing
│   │   ├── MapEditSessionCommandTests.cs
│   │   ├── MapEditSessionCreationTests.cs
│   │   ├── MapEditSessionDirtyTests.cs
│   │   ├── MapEditSessionHistoryTests.cs
│   │   ├── MapEditSessionSelectionTests.cs
│   │   ├── MapEditSessionThreadTests.cs
│   │   ├── MapEditSessionValidationTests.cs
│   │   └── MapRenderSnapshotProjectionTests.cs
│   ├── Spatial
│   │   ├── SceneStateOwnerSpatialTests.cs
│   │   ├── SpatialIndexOwnerLifecycleTests.cs
│   │   ├── SpatialIndexOwnerRevisionTests.cs
│   │   ├── SpatialIndexScaleTests.cs
│   │   ├── SpatialRayQueryLifecycleTests.cs
│   │   ├── SpatialRayQueryTests.cs
│   │   ├── SpatialRaycastNearestTests.cs
│   │   ├── SpatialRaycastRevisionTests.cs
│   │   ├── SpatialRaycastScaleTests.cs
│   │   └── SpatialTestData.cs
│   ├── Transform
│   │   └── TransformSessionTests.cs
│   ├── World
│   │   ├── EntityRegistryTests.cs
│   │   ├── GlobalWorldTests.cs
│   │   ├── SceneMapReferenceTests.cs
│   │   ├── UiMapEditorTests.cs
│   │   ├── UiViewGizmoTests.cs
│   │   ├── WorldCR2CameraDocumentTests.cs
│   │   ├── WorldCR2DocumentTests.cs
│   │   ├── WorldCR2EntityTests.cs
│   │   ├── WorldCR2InlineRenameTests.cs
│   │   ├── WorldCR2UiHistoryTests.cs
│   │   ├── WorldCR3R3CommandSmokeTests.cs
│   │   ├── WorldCR3R4GlobalGizmoTests.cs
│   │   ├── WorldCR3ViewportAssistTests.cs
│   │   ├── WorldCR4D0AssetContractTests.cs
│   │   ├── WorldCR4D1GlbFactory.cs
│   │   ├── WorldCR4D1GlbImportTests.cs
│   │   ├── WorldCR4D3AuthoringServiceTests.cs
│   │   ├── WorldCR4D3CatalogTests.cs
│   │   ├── WorldCR4D3F1BaseVertexTests.cs
│   │   ├── WorldCR4D3F1FailureTrackerTests.cs
│   │   ├── WorldCR4D3F1GlbFactory.cs
│   │   ├── WorldCR4D3F1ValidatorTests.cs
│   │   ├── WorldCR4D3ProjectionTests.cs
│   │   ├── WorldCR4D3StaticModelUiTests.cs
│   │   ├── WorldCSceneDocumentTests.R1R1.cs
│   │   ├── WorldCSceneDocumentTests.R1SaveFeedback.cs
│   │   ├── WorldCSceneDocumentTests.cs
│   │   ├── WorldCameraFramingOccupancyTests.cs
│   │   ├── WorldCameraFramingTests.cs
│   │   ├── WorldCameraNavigationUiTests.cs
│   │   ├── WorldDR1EnvironmentTests.cs
│   │   ├── WorldEntityBoundsSemanticsTests.cs
│   │   ├── WorldMoveTransformPlaneUiTests.cs
│   │   ├── WorldMoveTransformRegionUiTests.cs
│   │   ├── WorldMoveTransformSessionUiTests.cs
│   │   ├── WorldMoveTransformUiTests.cs
│   │   ├── WorldPartitionR1Tests.Activity.cs
│   │   ├── WorldPartitionR1Tests.cs
│   │   ├── WorldPartitionR2Tests.cs
│   │   ├── WorldPartitionTests.PartitionStrategy.cs
│   │   ├── WorldPartitionTests.cs
│   │   ├── WorldPartitionUiTests.cs
│   │   ├── WorldR1FinalSceneTests.cs
│   │   ├── WorldR1FinalSelectionTests.cs
│   │   ├── WorldR4InspectorInputTests.cs
│   │   ├── WorldR4TransformFoundationTests.cs
│   │   ├── WorldR4TransformInputTests.cs
│   │   ├── WorldRotateTransformUiTests.R4R1.cs
│   │   ├── WorldRotateTransformUiTests.R4R2.Helpers.cs
│   │   ├── WorldRotateTransformUiTests.R4R2.cs
│   │   ├── WorldRotateTransformUiTests.R4R3R1.cs
│   │   ├── WorldRotateTransformUiTests.cs
│   │   ├── WorldScaleTransformUiTests.Helpers.cs
│   │   ├── WorldScaleTransformUiTests.History.cs
│   │   ├── WorldScaleTransformUiTests.Pointer.cs
│   │   ├── WorldScaleTransformUiTests.R5R1.cs
│   │   ├── WorldScaleTransformUiTests.Target.cs
│   │   ├── WorldScaleTransformUiTests.cs
│   │   ├── WorldSceneConsumptionTests.cs
│   │   ├── WorldSceneIsolationTests.cs
│   │   ├── WorldSceneMultiEntityGateTests.cs
│   │   ├── WorldSceneSelectionReentryTests.cs
│   │   ├── WorldSceneSingleAuthorityTests.cs
│   │   ├── WorldSelectionToolStateUiTests.cs
│   │   ├── WorldSpatialQueryGovernanceTests.cs
│   │   ├── WorldSpatialQueryTests.Geometry.cs
│   │   ├── WorldSpatialQueryTests.cs
│   │   ├── WorldSpatialR1LifecycleTests.cs
│   │   ├── WorldSpatialR1Oracle.cs
│   │   ├── WorldSpatialR1RebuildTests.cs
│   │   ├── WorldToolStateHighlightUiTests.Selection.cs
│   │   ├── WorldToolStateHighlightUiTests.cs
│   │   ├── WorldUiHierarchyConnectorTests.cs
│   │   ├── WorldUiTreeGuideTests.cs
│   │   └── WorldUiTreeToggleTests.cs
│   └── XuanYu.World.Tests.csproj
├── changelog.md：变更记录（版本条目，按自然月归档）
├── docs
│   ├── CODE_CONSTITUTION.md：代码与架构硬规则
│   ├── architecture
│   │   ├── ENGINE_ARCHITECTURE.md：引擎总体架构
│   │   └── world-a-r0-coordinate-contract.md：官方坐标合同（Z-Up、XY 水平）
│   ├── archive
│   │   ├── changelog
│   │   │   ├── changelog-2026-05.md
│   │   │   ├── changelog-2026-06.md
│   │   │   └── changelog-2026-07.md
│   │   └── superseded
│   │       ├── AI_DEVELOPMENT_RULES.md
│   │       └── LEGACY_FLUIDWARFARE_OLD_AUDIT.md
│   ├── dev-rules.md：开发硬规则执行手册
│   ├── docs-index.md：docs 分类索引
│   ├── governance
│   │   ├── NAMING_RULES.md
│   │   ├── debts
│   │   │   └── arch-world-debts.md
│   │   ├── dev-rules-understanding.md
│   │   ├── diagnostic-safety.md
│   │   ├── naming-XuanYu-Engine.md
│   │   ├── shr-2026-08-closure.svg
│   │   └── 版本号规范与历史映射.md：版本格式与历史编号映射
│   ├── milestones
│   │   ├── closed
│   │   │   ├── ARCH-A
│   │   │   │   └── arch-a-plan.md
│   │   │   ├── ARCH-B
│   │   │   │   └── arch-b-plan.md
│   │   │   ├── ARCH-C
│   │   │   │   ├── arch-c-overview.svg
│   │   │   │   ├── arch-c-plan.md
│   │   │   │   ├── arch-c-r2-current-route.svg
│   │   │   │   ├── arch-c-r2-entry-audit.md
│   │   │   │   ├── arch-c-r2-spatial-query.svg
│   │   │   │   ├── arch-c-r2b-closure.svg
│   │   │   │   ├── arch-c-r2b-space-fact.svg
│   │   │   │   ├── arch-c-r2c-closure.svg
│   │   │   │   ├── arch-c-r2c-render-space.svg
│   │   │   │   ├── arch-c-r2d-spatial-index.svg
│   │   │   │   ├── arch-c-r2e-ray-hit.svg
│   │   │   │   ├── arch-c-r2f-pointer-picking.svg
│   │   │   │   ├── arch-c-r3-selection.svg
│   │   │   │   ├── arch-c-r3-timeout-fix.svg
│   │   │   │   ├── arch-c-r4-move-gizmo.svg
│   │   │   │   ├── arch-c-r4-r1-gizmo-hit.svg
│   │   │   │   ├── arch-c-r5-to-r8-route.svg
│   │   │   │   ├── arch-c-r5-transform-session.md
│   │   │   │   ├── arch-c-r5-transform-session.svg
│   │   │   │   ├── arch-c-r7-log-copy-fix.svg
│   │   │   │   ├── arch-c-r7-undo.svg
│   │   │   │   ├── arch-c-r8-acceptance.md
│   │   │   │   ├── arch-c-r8-final-acceptance-report.md
│   │   │   │   ├── arch-c-r8-final-acceptance-status.svg
│   │   │   │   ├── arch-c-r8-integration-acceptance.svg
│   │   │   │   ├── arch-c-r8-stage-acceptance-report.md
│   │   │   │   └── arch-c-r8-stage-acceptance-status.svg
│   │   │   ├── ARCH-WORLD
│   │   │   │   ├── arch-world-layer-attribution.md
│   │   │   │   ├── arch-world-layer-attribution.svg
│   │   │   │   ├── arch-world-r1-acceptance.md
│   │   │   │   ├── arch-world-r1-acceptance.svg
│   │   │   │   ├── arch-world-r2-g1-audit.md
│   │   │   │   ├── arch-world-r2-manual-checklist.html
│   │   │   │   ├── arch-world-r2-single-spatial-authority.md
│   │   │   │   ├── arch-world-r2-status.md
│   │   │   │   ├── arch-world-r3-scene-truth-audit.md
│   │   │   │   ├── arch-world-r4-editor-boundary.svg
│   │   │   │   ├── arch-world-r4-editor-pollution-audit.md
│   │   │   │   ├── arch-world-r4-gate2-acceptance.md
│   │   │   │   ├── arch-world-r5-final-closure.md
│   │   │   │   ├── arch-world-r5-final-closure.svg
│   │   │   │   ├── arch-world-r5-r0a-render-contract-audit.md
│   │   │   │   ├── arch-world-r5-r0a-render-contract.svg
│   │   │   │   ├── arch-world-r6-exit-gate.md
│   │   │   │   └── arch-world-r6-exit-gate.svg
│   │   │   ├── M1
│   │   │   │   ├── MILESTONE1_PUBLIC_VALIDATION.md
│   │   │   │   ├── PHASE1_SCOPE.md
│   │   │   │   ├── PROJECT_CHARTER.md
│   │   │   │   ├── audit-EditorShellV2-9.1A-1.md
│   │   │   │   ├── audit-EditorShellV2-freeze-9.1A-Freeze.md
│   │   │   │   ├── audit-EditorShellV2-input-9.1A-2.md
│   │   │   │   ├── audit-EditorShellV2-input-9.1A-2R.md
│   │   │   │   ├── audit-EditorShellV2-picking-gizmo-9.1A-3.md
│   │   │   │   ├── audit-EditorShellV2-picking-gizmo-9.1A-3R.md
│   │   │   │   ├── audit-EditorShellV2-plan-9.1A-0.md
│   │   │   │   ├── audit-NativeViewportMouseCapture-lifecycle-9.0X.md
│   │   │   │   ├── audit-RZ-New-0-onboarding.md
│   │   │   │   ├── audit-RZ-VK1-vulkan-probe.md
│   │   │   │   ├── audit-RZ-VK2-R1-nativehost-resize-coalesce.md
│   │   │   │   ├── audit-RZ-VK2-R2-nativehost-resize-coalesce-verify.md
│   │   │   │   ├── audit-RZ-VK2-native-host-lifecycle.md
│   │   │   │   ├── audit-gizmo-chain-9.0Y-1.md
│   │   │   │   ├── audit-gizmo-chain-9.0Y-2.md
│   │   │   │   ├── audit-gizmo-chain-9.0Y-3.md
│   │   │   │   ├── audit-gizmo-stash-9.0Y-0.md
│   │   │   │   ├── audit-input-lifecycle-9.0X-1.md
│   │   │   │   ├── audit-input-lifecycle-9.0X-2.md
│   │   │   │   ├── audit-input-lifecycle-9.0X-3.md
│   │   │   │   ├── audit-inspector-transform-9.0C-0.md
│   │   │   │   ├── editor-top-area-target-9.1B.md
│   │   │   │   ├── editor-top-svg-icons-9.1C-R.md
│   │   │   │   ├── editor-top-svg-icons-9.1C.md
│   │   │   │   ├── editor-ui-terms-9.1B.md
│   │   │   │   ├── gizmo_drag_audit_2026-06-25.md
│   │   │   │   ├── gizmo_drag_audit_probe.log
│   │   │   │   ├── plan-9.0D-move-gizmo-final.md
│   │   │   │   ├── project-baseline-audit-org-1-r1.md
│   │   │   │   └── project-baseline-audit-org-1.md
│   │   │   ├── RZ-VK
│   │   │   │   ├── log-ux-1-r2-autoscroll.svg
│   │   │   │   ├── log-ux-r8-tail-noise-fix.svg
│   │   │   │   ├── log-ux-window-copy-focus-fix.svg
│   │   │   │   ├── rz-vk3-closure.md
│   │   │   │   ├── rz-vk3-surface-lifecycle-plan.md
│   │   │   │   ├── rz-vk4-c-r1-audit-plan.md
│   │   │   │   ├── rz-vk4-c-swapchain-plan.md
│   │   │   │   ├── rz-vk4-closure.md
│   │   │   │   ├── rz-vk4-d-plan.md
│   │   │   │   ├── rz-vk4-plan.md
│   │   │   │   ├── rz-vk5-a-plan.md
│   │   │   │   ├── rz-vk5-c-plan.md
│   │   │   │   ├── rz-vk5-e-plan.md
│   │   │   │   ├── rz-vk5-plan.md
│   │   │   │   ├── vk4-c-r1-swapchain-fix.svg
│   │   │   │   ├── vulkan-lifecycle-plan.md
│   │   │   │   └── vulkan-preflight-audit-RZ-Fix3-0.md
│   │   │   ├── WORLD-A
│   │   │   │   ├── world-a-r0-coordinate-chain.svg
│   │   │   │   ├── world-a-r0-r1-tool-history-fix.svg
│   │   │   │   ├── world-a-r0-r2-transform-route-fix.svg
│   │   │   │   ├── world-a-r0-r3-gizmo-visibility.svg
│   │   │   │   ├── world-a-r1-entity-registry.svg
│   │   │   │   ├── world-a-r1-final-closure-report.md
│   │   │   │   ├── world-a-r1-final-closure.svg
│   │   │   │   ├── world-a-r1-r1-scene-consumption-audit.md
│   │   │   │   ├── world-a-r1-r1-scene-consumption.svg
│   │   │   │   ├── world-a-r1-r2-final-gate.md
│   │   │   │   ├── world-a-r1-r2-multi-entity-gate.svg
│   │   │   │   ├── world-a-r1-r2-r1-acceptance-report.md
│   │   │   │   ├── world-a-r1-r2-r1-acceptance.svg
│   │   │   │   ├── world-a-r1-r2-runtime-fix.svg
│   │   │   │   ├── world-a-r2-global-partition-report.md
│   │   │   │   ├── world-a-r2-global-partition.svg
│   │   │   │   ├── world-a-r2-r1-migration-activity-report.md
│   │   │   │   ├── world-a-r2-r1-migration-activity.svg
│   │   │   │   ├── world-a-r2-r2-partition-consistency-report.md
│   │   │   │   ├── world-a-r2-r2-partition-consistency.svg
│   │   │   │   ├── world-a-r2-r3-inspector-manual-gate-report.md
│   │   │   │   ├── world-a-r2-r3-inspector-manual-gate.svg
│   │   │   │   ├── world-a-r2-r4-camera-framing-report.md
│   │   │   │   ├── world-a-r2-r4-camera-framing.svg
│   │   │   │   ├── world-a-r3-r1-spatial-consistency-report.md
│   │   │   │   ├── world-a-r3-r1-spatial-consistency.svg
│   │   │   │   ├── world-a-r3-spatial-query-report.md
│   │   │   │   ├── world-a-r3-spatial-query.svg
│   │   │   │   ├── world-a-ui-r1-display-cleanup-report.md
│   │   │   │   ├── world-a-ui-r1-display-cleanup.svg
│   │   │   │   ├── world-a-ui-r2-continuous-tree-report.md
│   │   │   │   └── world-a-ui-r2-continuous-tree.svg
│   │   │   ├── WORLD-B
│   │   │   │   ├── world-b-r0-editor-interaction-audit.md
│   │   │   │   ├── world-b-r0-editor-interaction-audit.svg
│   │   │   │   ├── world-b-r1-camera-acceptance-closure.md
│   │   │   │   ├── world-b-r1-camera-acceptance-closure.svg
│   │   │   │   ├── world-b-r1-camera-operation-report.md
│   │   │   │   ├── world-b-r1-camera-operation.svg
│   │   │   │   ├── world-b-r2-selection-tool-state-report.md
│   │   │   │   ├── world-b-r2-selection-tool-state.svg
│   │   │   │   ├── world-b-r3-move-transform-closure.md
│   │   │   │   ├── world-b-r3-move-transform-closure.svg
│   │   │   │   ├── world-b-r5-scale-transform-report.md
│   │   │   │   └── world-b-r5-scale-transform.svg
│   │   │   └── WORLD-C
│   │   │       ├── world-c-r0-scene-document-contract.md
│   │   │       ├── world-c-r0-scene-document-contract.svg
│   │   │       ├── world-c-r1-closure-report.md
│   │   │       ├── world-c-r1-closure.svg
│   │   │       ├── world-c-r2-implementation-acceptance.md
│   │   │       ├── world-c-r2-ipo-manual-checklist.md
│   │   │       ├── world-c-r2-status.svg
│   │   │       ├── world-c-r3-viewport-reference-report.md
│   │   │       ├── world-c-r3-viewport-reference.svg
│   │   │       ├── world-c-r4-d0-asset-contracts.md
│   │   │       ├── world-c-r4-d1-glb-import-core.md
│   │   │       ├── world-c-r4-d2-f1-ipo-checklist.md
│   │   │       ├── world-c-r4-d2-static-model-rendering.md
│   │   │       ├── world-c-r4-d3-static-model-authoring-report.md
│   │   │       ├── world-c-r4-d4-i1-hosted-assets-report.md
│   │   │       └── world-c-r4-d4-static-model-persistence-report.md
│   │   └── current
│   │       └── MAP-A
│   │           ├── map-a-r1-d1-map-contracts.md：地图合同冻结（.xymap/.xyscene v4）
│   │           ├── map-a-r1-d5-r1-f2-grid-stabilize.svg
│   │           ├── map-a-r1-d5-r1-f2-r2-unified-grid-lod.svg
│   │           ├── map-a-r1-d5-r1-f2-r3-grid-ground-visual.svg
│   │           ├── map-a-r1-d5-r1-f2-r3-r2-per-pixel-background.svg
│   │           ├── map-a-r1-d5-r1-f3-f1-overlay-gizmo.svg
│   │           ├── map-a-r1-d5-r1-f3-f2-camera-basis-recovery.svg
│   │           ├── map-a-r1-d5-r1-f3-f3-gizmo-recovery.svg
│   │           └── map-a-r1-d5-r1-f3-viewport-navigation-gizmo.svg
│   └── 玄域引擎_AI开发宪法.md：最高开发治理规则（唯一宪法事实源）
├── file-tree.md：当前仓库结构与文件职责（本文档）
├── run.bat：编辑器启动脚本（窗口标题带版本号）
├── samples
│   └── world-c-r1-ten-triangles.xyscene
└── scripts
    ├── arch-a-guard-editor.ps1
    ├── arch-a-guard-render.ps1
    ├── arch-a-guard-warcore.ps1
    ├── arch-a-guard-world.ps1
    └── arch-a-guard.ps1：架构守卫（依赖边界 + 5+100 门禁）
```
