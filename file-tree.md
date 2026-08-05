# XuanYu Engine 文件树

> 本文仅描述当前仓库结构与文件职责，不记录版本历史、阶段过程、迁移记录或测试统计。

```text
XuanYuEngine/
├── XuanYu.Core
│   ├── Diagnostics
│   │   ├── CoreSelfTest.cs：CoreSelfTest.cs
│   ├── Gizmo
│   │   ├── Common
│   │   │   ├── ScreenPoint.cs：ScreenPoint.cs
│   │   ├── Move
│   │   │   ├── MoveGizmoAxis.cs：MoveGizmoAxis.cs
│   │   │   ├── MoveGizmoDragConstraint.Axes.cs：MoveGizmoDragConstraint.Axes.cs
│   │   │   ├── MoveGizmoDragConstraint.cs：MoveGizmoDragConstraint.cs
│   │   │   ├── MoveGizmoLayout.Hit.cs：MoveGizmoLayout.Hit.cs
│   │   │   ├── MoveGizmoLayout.Plane.cs：MoveGizmoLayout.Plane.cs
│   │   │   ├── MoveGizmoLayout.cs：MoveGizmoLayout.cs
│   │   │   ├── MoveGizmoPlane.cs：MoveGizmoPlane.cs
│   │   │   ├── MoveGizmoScreenSize.cs：MoveGizmoScreenSize.cs
│   │   │   ├── MoveGizmoSegment.cs：MoveGizmoSegment.cs
│   │   ├── Rotate
│   │   │   ├── RotateGizmoAxis.cs：RotateGizmoAxis.cs
│   │   │   ├── RotateGizmoDrag.Math.cs：RotateGizmoDrag.Math.cs
│   │   │   ├── RotateGizmoDrag.cs：RotateGizmoDrag.cs
│   │   │   ├── RotateGizmoLayout.cs：RotateGizmoLayout.cs
│   │   │   ├── RotateGizmoRing.cs：RotateGizmoRing.cs
│   │   │   ├── RotateGizmoScreenRadius.cs：RotateGizmoScreenRadius.cs
│   │   └── Scale
│   │   ├── ScaleGizmoAxis.cs：ScaleGizmoAxis.cs
│   │   ├── ScaleGizmoDrag.cs：ScaleGizmoDrag.cs
│   │   ├── ScaleGizmoHitTester.cs：ScaleGizmoHitTester.cs
│   │   ├── ScaleGizmoLayout.cs：ScaleGizmoLayout.cs
│   │   ├── ScaleGizmoScreenSize.cs：ScaleGizmoScreenSize.cs
│   ├── History
│   │   ├── EditorHistoryOwner.cs：EditorHistoryOwner.cs
│   │   ├── TransformHistoryEntry.cs：TransformHistoryEntry.cs
│   ├── Identity
│   │   ├── EntityId.cs：EntityId.cs
│   ├── Logging
│   │   ├── EngineLogEntry.cs：EngineLogEntry.cs
│   │   ├── EngineLogLevel.cs：EngineLogLevel.cs
│   ├── Map
│   │   ├── MapSurfaceKind.cs：MapSurfaceKind.cs
│   │   ├── MapSurfaceSampler.cs：MapSurfaceSampler.cs
│   │   ├── MapTerrainVertex.cs：MapTerrainVertex.cs
│   ├── Math
│   │   ├── Vector3d.cs：Vector3d.cs
│   │   ├── YawRotation.cs：YawRotation.cs
│   ├── Picking
│   │   ├── ViewportPickingRequest.cs：ViewportPickingRequest.cs
│   │   ├── ViewportPickingResult.cs：ViewportPickingResult.cs
│   │   ├── ViewportPickingService.cs：ViewportPickingService.cs
│   ├── Properties
│   │   ├── AssemblyInfo.cs：AssemblyInfo.cs
│   ├── Results
│   │   ├── EngineError.cs：EngineError.cs
│   │   ├── EngineResult.cs：EngineResult.cs
│   ├── Scene
│   │   ├── CommittedTransform.cs：CommittedTransform.cs
│   │   ├── ISceneRenderSnapshotSource.cs：ISceneRenderSnapshotSource.cs
│   │   ├── SceneEntitySnapshot.cs：SceneEntitySnapshot.cs
│   │   ├── SceneRenderSnapshot.cs：SceneRenderSnapshot.cs
│   │   ├── SceneTransformCommitResult.cs：SceneTransformCommitResult.cs
│   ├── Space
│   │   ├── CameraState.cs：CameraState.cs
│   │   ├── DefaultEditorCamera.cs：DefaultEditorCamera.cs
│   │   ├── ProjectionMode.cs：ProjectionMode.cs
│   │   ├── ViewProjectionState.cs：ViewProjectionState.cs
│   │   ├── ViewportState.cs：ViewportState.cs
│   │   ├── WorldRay.cs：WorldRay.cs
│   │   ├── WorldRayFactory.cs：WorldRayFactory.cs
│   ├── Spatial
│   │   ├── RayAabbHit.cs：RayAabbHit.cs
│   │   ├── RayAabbIntersection.cs：RayAabbIntersection.cs
│   │   ├── SpatialAabb.cs：SpatialAabb.cs
│   │   ├── SpatialBounds.cs：SpatialBounds.cs
│   │   ├── SpatialQueryCategory.cs：SpatialQueryCategory.cs
│   │   ├── SpatialQueryResult.cs：SpatialQueryResult.cs
│   │   ├── SpatialQueryStats.cs：SpatialQueryStats.cs
│   │   ├── SpatialRayAabb.cs：SpatialRayAabb.cs
│   │   ├── SpatialRayQuery.cs：SpatialRayQuery.cs
│   │   ├── SpatialRaycastHit.cs：SpatialRaycastHit.cs
│   │   ├── SpatialRaycastResult.cs：SpatialRaycastResult.cs
│   │   ├── SpatialRaycastStats.cs：SpatialRaycastStats.cs
│   ├── Time
│   │   ├── SimulationTime.cs：SimulationTime.cs
│   │   ├── TimeStep.cs：TimeStep.cs
│   └── Transform
│   ├── PreviewTransform.cs：PreviewTransform.cs
│   ├── TransformStartSnapshot.cs：TransformStartSnapshot.cs
│   ├── .gitkeep：.gitkeep
│   ├── XuanYu.Core.csproj：XuanYu.Core.csproj
├── XuanYu.Core.Tests
│   ├── Camera
│   │   ├── CameraBasisTests.cs：CameraBasisTests.cs
│   │   ├── CameraNavigationRollTests.cs：CameraNavigationRollTests.cs
│   │   ├── CameraNavigationSequenceTests.cs：CameraNavigationSequenceTests.cs
│   │   ├── CameraNavigationStressTests.cs：CameraNavigationStressTests.cs
│   │   ├── CameraNavigationTests.cs：CameraNavigationTests.cs
│   │   ├── CameraNavigationUiSequenceTests.Safety.cs：CameraNavigationUiSequenceTests.Safety.cs
│   │   ├── CameraNavigationUiSequenceTests.cs：CameraNavigationUiSequenceTests.cs
│   │   ├── CameraOrthographicNavigationTests.cs：CameraOrthographicNavigationTests.cs
│   ├── EditorTool
│   │   ├── EditorTransformCapturePolicyTests.cs：EditorTransformCapturePolicyTests.cs
│   ├── Gizmo
│   │   ├── MoveGizmoDragConstraintTests.cs：MoveGizmoDragConstraintTests.cs
│   │   ├── MoveGizmoLayoutG1Tests.cs：MoveGizmoLayoutG1Tests.cs
│   │   ├── MoveGizmoLayoutPlaneTests.cs：MoveGizmoLayoutPlaneTests.cs
│   │   ├── MoveGizmoLayoutTests.cs：MoveGizmoLayoutTests.cs
│   │   ├── MoveGizmoLayoutVulkanTests.cs：MoveGizmoLayoutVulkanTests.cs
│   │   ├── MoveGizmoScreenSizeTests.cs：MoveGizmoScreenSizeTests.cs
│   │   ├── RotateGizmoLayoutTests.cs：RotateGizmoLayoutTests.cs
│   │   ├── ScaleGizmoTests.Drag.cs：ScaleGizmoTests.Drag.cs
│   │   ├── ScaleGizmoTests.DragSafety.cs：ScaleGizmoTests.DragSafety.cs
│   │   ├── ScaleGizmoTests.Helpers.cs：ScaleGizmoTests.Helpers.cs
│   │   ├── ScaleGizmoTests.R5R1.cs：ScaleGizmoTests.R5R1.cs
│   │   ├── ScaleGizmoTests.cs：ScaleGizmoTests.cs
│   ├── History
│   │   ├── EditorHistoryOwnerTests.cs：EditorHistoryOwnerTests.cs
│   │   ├── EditorHistoryRedoTests.cs：EditorHistoryRedoTests.cs
│   │   ├── TransformHistoryIntegrationTests.cs：TransformHistoryIntegrationTests.cs
│   │   ├── TransformHistoryRedoIntegrationTests.cs：TransformHistoryRedoIntegrationTests.cs
│   ├── Picking
│   │   ├── ViewportPickingServiceTests.cs：ViewportPickingServiceTests.cs
│   ├── Render
│   │   ├── Camera
│   │   │   ├── StandardViewResolverTests.cs：StandardViewResolverTests.cs
│   │   ├── DrawPlan
│   │   │   ├── CubeRenderDrawPlanTests.cs：CubeRenderDrawPlanTests.cs
│   │   │   ├── FrameExecutionPolicyTests.cs：FrameExecutionPolicyTests.cs
│   │   │   ├── RenderDrawPlanTests.cs：RenderDrawPlanTests.cs
│   │   │   ├── SceneRenderProjectionAdapterTests.Rotation.cs：SceneRenderProjectionAdapterTests.Rotation.cs
│   │   │   ├── SceneRenderProjectionAdapterTests.Selection.cs：SceneRenderProjectionAdapterTests.Selection.cs
│   │   │   ├── SceneRenderProjectionAdapterTests.cs：SceneRenderProjectionAdapterTests.cs
│   │   │   ├── ViewportAssistDrawPlanTests.cs：ViewportAssistDrawPlanTests.cs
│   │   │   ├── ViewportChromeContractTests.cs：ViewportChromeContractTests.cs
│   │   ├── Grid
│   │   │   ├── ReferenceGridAdaptiveTests.cs：ReferenceGridAdaptiveTests.cs
│   │   │   ├── ReferenceGridDrawPlanTests.cs：ReferenceGridDrawPlanTests.cs
│   │   │   ├── ReferenceGridRayIntersectionTests.cs：ReferenceGridRayIntersectionTests.cs
│   │   │   ├── ReferenceGridScaleTests.cs：ReferenceGridScaleTests.cs
│   │   │   ├── ReferenceGridShaderContractTests.cs：ReferenceGridShaderContractTests.cs
│   │   │   ├── ReferenceGridVisualStyleTests.cs：ReferenceGridVisualStyleTests.cs
│   │   ├── Map
│   │   │   ├── MapRenderDrawPlanTests.cs：MapRenderDrawPlanTests.cs
│   │   │   ├── MapSurfaceGeometryTests.cs：MapSurfaceGeometryTests.cs
│   │   │   ├── MapSurfaceLayerVisibilityTests.cs：MapSurfaceLayerVisibilityTests.cs
│   │   │   ├── MapSurfaceResourceKeyTests.cs：MapSurfaceResourceKeyTests.cs
│   │   │   ├── MapSurfaceResourceUpdatePolicyTests.cs：MapSurfaceResourceUpdatePolicyTests.cs
│   │   ├── NavigationGizmo
│   │   │   ├── NavigationGizmoLayoutTests.Facing.cs：NavigationGizmoLayoutTests.Facing.cs
│   │   │   ├── NavigationGizmoLayoutTests.cs：NavigationGizmoLayoutTests.cs
│   │   │   ├── NavigationGizmoOverlayContractTests.cs：NavigationGizmoOverlayContractTests.cs
│   │   └── StaticModels
│   │   ├── StaticModelDepthRegressionTests.cs：StaticModelDepthRegressionTests.cs
│   │   ├── StaticModelRenderContractTests.cs：StaticModelRenderContractTests.cs
│   ├── Space
│   │   ├── CameraOrthographicTests.cs：CameraOrthographicTests.cs
│   │   ├── CameraStateTests.cs：CameraStateTests.cs
│   │   ├── DefaultEditorCameraTests.cs：DefaultEditorCameraTests.cs
│   │   ├── SpaceAssert.cs：SpaceAssert.cs
│   │   ├── ViewProjectionStateTests.cs：ViewProjectionStateTests.cs
│   │   ├── ViewportStateTests.cs：ViewportStateTests.cs
│   │   ├── WorldRayFactoryTests.cs：WorldRayFactoryTests.cs
│   │   ├── WorldRayTests.cs：WorldRayTests.cs
│   └── Spatial
│   ├── RayAabbIntersectionTests.cs：RayAabbIntersectionTests.cs
│   ├── SpatialBoundsTests.cs：SpatialBoundsTests.cs
│   ├── SpatialTestData.cs：SpatialTestData.cs
│   ├── CoreSmokeTests.cs：CoreSmokeTests.cs
│   ├── XuanYu.Core.Tests.csproj：XuanYu.Core.Tests.csproj
├── XuanYu.Editor
│   ├── Assets
│   │   ├── Catalog
│   │   │   ├── SceneStaticModelCatalog.cs：SceneStaticModelCatalog.cs
│   │   ├── Hosting
│   │   │   ├── Planning
│   │   │   │   ├── SceneAssetHostingPlan.cs：SceneAssetHostingPlan.cs
│   │   │   │   ├── SceneAssetHostingPlanner.cs：SceneAssetHostingPlanner.cs
│   │   │   └── Transactions
│   │   │   ├── SceneAssetHostingTransaction.Activate.cs：SceneAssetHostingTransaction.Activate.cs
│   │   │   ├── SceneAssetHostingTransaction.Complete.cs：SceneAssetHostingTransaction.Complete.cs
│   │   │   ├── SceneAssetHostingTransaction.Rollback.cs：SceneAssetHostingTransaction.Rollback.cs
│   │   │   ├── SceneAssetHostingTransaction.cs：SceneAssetHostingTransaction.cs
│   │   │   ├── HostedSceneAsset.cs：HostedSceneAsset.cs
│   │   │   ├── ModelAssetRuntimeState.cs：ModelAssetRuntimeState.cs
│   │   │   ├── SceneAssetHostingError.cs：SceneAssetHostingError.cs
│   │   │   ├── SceneAssetHostingState.cs：SceneAssetHostingState.cs
│   │   │   ├── SceneAssetPathPolicy.cs：SceneAssetPathPolicy.cs
│   │   ├── Identity
│   │   │   ├── AssetId.cs：AssetId.cs
│   │   ├── Import
│   │   │   └── Gltf
│   │   │   ├── GlbContainer.cs：GlbContainer.cs
│   │   │   ├── GlbImportService.cs：GlbImportService.cs
│   │   │   ├── GltfAccessorReader.cs：GltfAccessorReader.cs
│   │   │   ├── GltfCoordinatePolicy.cs：GltfCoordinatePolicy.cs
│   │   │   ├── GltfJsonAccess.cs：GltfJsonAccess.cs
│   │   │   ├── GltfNodeTransform.cs：GltfNodeTransform.cs
│   │   │   ├── GltfStaticModelImporter.cs：GltfStaticModelImporter.cs
│   │   │   ├── ImportStop.cs：ImportStop.cs
│   │   └── StaticModels
│   │   ├── SceneStaticModelBinding.cs：SceneStaticModelBinding.cs
│   │   ├── StaticModelAuthoringService.cs：StaticModelAuthoringService.cs
│   │   ├── StaticModelBuilder.cs：StaticModelBuilder.cs
│   │   ├── StaticModelColor.cs：StaticModelColor.cs
│   │   ├── StaticModelData.cs：StaticModelData.cs
│   │   ├── StaticModelImportCodes.cs：StaticModelImportCodes.cs
│   │   ├── StaticModelImportResult.cs：StaticModelImportResult.cs
│   │   ├── StaticModelImportWarning.cs：StaticModelImportWarning.cs
│   │   ├── StaticModelPrimitive.cs：StaticModelPrimitive.cs
│   │   ├── StaticModelVertex.cs：StaticModelVertex.cs
│   ├── Camera
│   │   ├── CameraBasis.cs：CameraBasis.cs
│   │   ├── CameraFrameResult.cs：CameraFrameResult.cs
│   │   ├── CameraNavigation.Try.cs：CameraNavigation.Try.cs
│   │   ├── CameraNavigation.cs：CameraNavigation.cs
│   │   ├── EditorCameraFraming.Orthographic.cs：EditorCameraFraming.Orthographic.cs
│   │   ├── EditorCameraFraming.cs：EditorCameraFraming.cs
│   │   ├── OrthographicViewFactory.cs：OrthographicViewFactory.cs
│   ├── MapDocument
│   │   ├── MapDocument.cs：MapDocument.cs
│   │   ├── MapDocumentAggregateBridge.cs：MapDocumentAggregateBridge.cs
│   │   ├── MapDocumentJson.cs：MapDocumentJson.cs
│   │   ├── MapDocumentOwner.cs：MapDocumentOwner.cs
│   │   ├── MapDocumentResult.cs：MapDocumentResult.cs
│   │   ├── MapDocumentValidator.cs：MapDocumentValidator.cs
│   │   ├── MapEnvironmentDefinition.cs：MapEnvironmentDefinition.cs
│   │   ├── MapJsonMapper.cs：MapJsonMapper.cs
│   │   ├── MapJsonSerializer.cs：MapJsonSerializer.cs
│   │   ├── MapStorageService.cs：MapStorageService.cs
│   ├── MapEditing
│   │   ├── MapEditEvents.cs：MapEditEvents.cs
│   │   ├── MapEditReason.cs：MapEditReason.cs
│   │   ├── MapEditSession.ActiveLayer.cs：MapEditSession.ActiveLayer.cs
│   │   ├── MapEditSession.Commands.cs：MapEditSession.Commands.cs
│   │   ├── MapEditSession.Commit.cs：MapEditSession.Commit.cs
│   │   ├── MapEditSession.Document.cs：MapEditSession.Document.cs
│   │   ├── MapEditSession.History.cs：MapEditSession.History.cs
│   │   ├── MapEditSession.Layers.cs：MapEditSession.Layers.cs
│   │   ├── MapEditSession.Selection.cs：MapEditSession.Selection.cs
│   │   ├── MapEditSession.cs：MapEditSession.cs
│   │   ├── MapHistoryEntry.cs：MapHistoryEntry.cs
│   │   ├── MapSelection.cs：MapSelection.cs
│   │   ├── MapSelectionKind.cs：MapSelectionKind.cs
│   ├── SceneDocument
│   │   ├── MapReference.cs：MapReference.cs
│   │   ├── SceneDocumentAsset.cs：SceneDocumentAsset.cs
│   │   ├── SceneDocumentEntity.cs：SceneDocumentEntity.cs
│   │   ├── SceneDocumentJson.cs：SceneDocumentJson.cs
│   │   ├── SceneDocumentLoadTransaction.cs：SceneDocumentLoadTransaction.cs
│   │   ├── SceneDocumentMapper.cs：SceneDocumentMapper.cs
│   │   ├── SceneDocumentResult.cs：SceneDocumentResult.cs
│   │   ├── SceneDocumentSaveTransaction.cs：SceneDocumentSaveTransaction.cs
│   │   ├── SceneDocumentSession.cs：SceneDocumentSession.cs
│   │   ├── SceneDocumentSnapshot.cs：SceneDocumentSnapshot.cs
│   │   ├── SceneDocumentValidator.MapReference.cs：SceneDocumentValidator.MapReference.cs
│   │   ├── SceneDocumentValidator.cs：SceneDocumentValidator.cs
│   │   ├── SceneDocumentWorldBridge.cs：SceneDocumentWorldBridge.cs
│   │   ├── SceneLoadCandidate.cs：SceneLoadCandidate.cs
│   │   ├── SceneSaveOutcome.cs：SceneSaveOutcome.cs
│   │   ├── SceneStorageService.cs：SceneStorageService.cs
│   └── Transform
│   ├── TransformSession.Rotate.cs：TransformSession.Rotate.cs
│   ├── TransformSession.Scale.cs：TransformSession.Scale.cs
│   ├── TransformSession.cs：TransformSession.cs
│   ├── XuanYu.Editor.csproj：XuanYu.Editor.csproj
├── XuanYu.Editor.App
│   ├── EditorCompositionRoot.cs：EditorCompositionRoot.cs
│   ├── Program.cs：Program.cs
│   ├── XuanYu.Editor.App.csproj：XuanYu.Editor.App.csproj
├── XuanYu.Editor.UI
│   ├── Bootstrap
│   │   ├── App.axaml：App.axaml
│   │   ├── App.axaml.cs：App.axaml.cs
│   │   ├── Program.cs：Program.cs
│   ├── Dialogs
│   │   ├── IEditorDialogService.cs：IEditorDialogService.cs
│   │   ├── NullEditorDialogService.cs：NullEditorDialogService.cs
│   ├── EditorState
│   │   ├── EditorInteractionChangedResult.cs：EditorInteractionChangedResult.cs
│   │   ├── EditorInteractionCommand.cs：EditorInteractionCommand.cs
│   │   ├── EditorInteractionPointerSnapshot.cs：EditorInteractionPointerSnapshot.cs
│   │   ├── EditorInteractionSnapshot.cs：EditorInteractionSnapshot.cs
│   │   ├── EditorSelectionCommand.cs：EditorSelectionCommand.cs
│   │   ├── EditorSelectionSnapshot.cs：EditorSelectionSnapshot.cs
│   │   ├── EditorStateChangedResult.cs：EditorStateChangedResult.cs
│   │   ├── EditorStateOwner.Interaction.cs：EditorStateOwner.Interaction.cs
│   │   ├── EditorStateOwner.Tool.cs：EditorStateOwner.Tool.cs
│   │   ├── EditorStateOwner.cs：EditorStateOwner.cs
│   │   ├── EditorToolChangedResult.cs：EditorToolChangedResult.cs
│   │   ├── EditorToolCommand.cs：EditorToolCommand.cs
│   │   ├── EditorToolId.cs：EditorToolId.cs
│   │   ├── EditorToolSnapshot.cs：EditorToolSnapshot.cs
│   │   ├── EditorToolText.cs：EditorToolText.cs
│   │   ├── EditorTransformCapturePolicy.cs：EditorTransformCapturePolicy.cs
│   ├── Foot
│   │   ├── Foot.axaml：Foot.axaml
│   │   ├── Foot.axaml.cs：Foot.axaml.cs
│   │   ├── LogAutoScrollPolicy.cs：LogAutoScrollPolicy.cs
│   │   ├── LogDetailPanel.axaml：LogDetailPanel.axaml
│   │   ├── LogDetailPanel.axaml.cs：LogDetailPanel.axaml.cs
│   │   ├── LogListAutoScrollController.Follow.cs：LogListAutoScrollController.Follow.cs
│   │   ├── LogListAutoScrollController.Layout.cs：LogListAutoScrollController.Layout.cs
│   │   ├── LogListAutoScrollController.cs：LogListAutoScrollController.cs
│   ├── Icons
│   │   ├── EditorIcons.axaml：EditorIcons.axaml
│   ├── Left
│   │   ├── InlineRenameActivation.cs：InlineRenameActivation.cs
│   │   ├── Left.EntityCommands.cs：Left.EntityCommands.cs
│   │   ├── Left.Styles.axaml：Left.Styles.axaml
│   │   ├── Left.axaml：Left.axaml
│   │   ├── Left.axaml.cs：Left.axaml.cs
│   ├── Main
│   │   ├── Main.axaml：Main.axaml
│   │   ├── Main.axaml.cs：Main.axaml.cs
│   ├── Right
│   │   ├── LayerInspectorPanel.axaml：LayerInspectorPanel.axaml
│   │   ├── LayerInspectorPanel.axaml.cs：LayerInspectorPanel.axaml.cs
│   │   ├── LayerPanel.axaml：图层面板（工具栏+图层列表，位于地图编辑器图层页）
│   │   ├── LayerPanel.axaml.cs：图层面板代码后置（无逻辑）
│   │   ├── MapEditorPanel.axaml：MapEditorPanel.axaml
│   │   ├── MapEditorPanel.axaml.cs：MapEditorPanel.axaml.cs
│   │   ├── Right.axaml：Right.axaml
│   │   ├── Right.axaml.cs：Right.axaml.cs
│   ├── Root
│   │   ├── UiRoot.axaml：UiRoot.axaml
│   │   ├── UiRoot.axaml.cs：UiRoot.axaml.cs
│   ├── Top
│   │   ├── Top.axaml：Top.axaml
│   │   ├── Top.axaml.cs：Top.axaml.cs
│   ├── Viewport
│   │   └── Vulkan
│   │   ├── NativePointerMessage.cs：NativePointerMessage.cs
│   │   ├── VulkanNativeHost.AvaloniaCamera.cs：VulkanNativeHost.AvaloniaCamera.cs
│   │   ├── VulkanNativeHost.AvaloniaPointer.cs：VulkanNativeHost.AvaloniaPointer.cs
│   │   ├── VulkanNativeHost.Bridge.cs：VulkanNativeHost.Bridge.cs
│   │   ├── VulkanNativeHost.CameraPointer.cs：VulkanNativeHost.CameraPointer.cs
│   │   ├── VulkanNativeHost.Dpi.cs：VulkanNativeHost.Dpi.cs
│   │   ├── VulkanNativeHost.Gizmo.cs：VulkanNativeHost.Gizmo.cs
│   │   ├── VulkanNativeHost.LayoutSync.cs：VulkanNativeHost.LayoutSync.cs
│   │   ├── VulkanNativeHost.Log.cs：VulkanNativeHost.Log.cs
│   │   ├── VulkanNativeHost.NavGizmo.cs：VulkanNativeHost.NavGizmo.cs
│   │   ├── VulkanNativeHost.Picking.cs：VulkanNativeHost.Picking.cs
│   │   ├── VulkanNativeHost.Pointer.cs：VulkanNativeHost.Pointer.cs
│   │   ├── VulkanNativeHost.cs：VulkanNativeHost.cs
│   │   ├── VulkanViewport.axaml：VulkanViewport.axaml
│   │   ├── VulkanViewport.axaml.cs：VulkanViewport.axaml.cs
│   │   ├── Win32ViewportHost.Input.cs：Win32ViewportHost.Input.cs
│   │   ├── Win32ViewportHost.cs：Win32ViewportHost.cs
│   │   ├── ViewNavigationGizmo.HitTest.cs：ViewNavigationGizmo.HitTest.cs
│   │   ├── ViewNavigationGizmo.Layout.cs：ViewNavigationGizmo.Layout.cs
│   ├── Vm
│   │   ├── Camera
│   │   │   ├── CameraSessionMode.cs：CameraSessionMode.cs
│   │   │   ├── CameraSessionSnapshot.cs：CameraSessionSnapshot.cs
│   │   │   ├── StandardViewResolver.cs：StandardViewResolver.cs
│   │   │   ├── UiVm.Camera.Framing.cs：UiVm.Camera.Framing.cs
│   │   │   ├── UiVm.Camera.cs：UiVm.Camera.cs
│   │   │   ├── UiVm.CameraNavigation.cs：UiVm.CameraNavigation.cs
│   │   │   ├── UiVm.ViewGizmo.cs：UiVm.ViewGizmo.cs
│   │   ├── History
│   │   │   ├── UiVm.EntityCommands.cs：UiVm.EntityCommands.cs
│   │   │   ├── UiVm.History.Entities.cs：UiVm.History.Entities.cs
│   │   │   ├── UiVm.History.cs：UiVm.History.cs
│   │   ├── Inspector
│   │   │   ├── UiVm.Inspector.cs：UiVm.Inspector.cs
│   │   │   ├── UiVm.InspectorInput.Parse.cs：UiVm.InspectorInput.Parse.cs
│   │   │   ├── UiVm.InspectorInput.cs：UiVm.InspectorInput.cs
│   │   ├── Logging
│   │   │   ├── DebugText.cs：DebugText.cs
│   │   │   ├── EditorDisplayText.cs：EditorDisplayText.cs
│   │   │   ├── EditorLogBuffer.cs：EditorLogBuffer.cs
│   │   │   ├── EditorLogBus.cs：EditorLogBus.cs
│   │   │   ├── EditorLogCategory.cs：EditorLogCategory.cs
│   │   │   ├── EditorLogClipboardText.cs：EditorLogClipboardText.cs
│   │   │   ├── EditorLogFilter.cs：EditorLogFilter.cs
│   │   │   ├── EditorLogFilterQuery.cs：EditorLogFilterQuery.cs
│   │   │   ├── EditorLogLevel.cs：EditorLogLevel.cs
│   │   │   ├── EditorLogNoiseFilter.cs：EditorLogNoiseFilter.cs
│   │   │   ├── EditorLogRepeatKey.cs：EditorLogRepeatKey.cs
│   │   │   ├── EditorLogSource.cs：EditorLogSource.cs
│   │   │   ├── EditorLogSummary.cs：EditorLogSummary.cs
│   │   │   ├── LogEntry.cs：LogEntry.cs
│   │   │   ├── SampleLogEntries.cs：SampleLogEntries.cs
│   │   │   ├── UiText.cs：UiText.cs
│   │   │   ├── UiVm.Logging.cs：UiVm.Logging.cs
│   │   ├── Map
│   │   │   ├── MapLayerRowViewModel.cs：MapLayerRowViewModel.cs
│   │   │   ├── MapRenderSnapshotProjection.cs：MapRenderSnapshotProjection.cs
│   │   │   ├── UiVm.MapCommandRouting.cs：UiVm.MapCommandRouting.cs
│   │   │   ├── UiVm.MapDiagnostics.Format.cs：UiVm.MapDiagnostics.Format.cs
│   │   │   ├── UiVm.MapDiagnostics.cs：UiVm.MapDiagnostics.cs
│   │   │   ├── UiVm.MapEditor.cs：UiVm.MapEditor.cs
│   │   │   ├── UiVm.MapHistory.cs：UiVm.MapHistory.cs
│   │   │   ├── UiVm.MapLayerDiagnostics.cs：UiVm.MapLayerDiagnostics.cs
│   │   │   ├── UiVm.MapLayerInspector.cs：UiVm.MapLayerInspector.cs
│   │   │   ├── UiVm.MapLayerSelection.cs：UiVm.MapLayerSelection.cs
│   │   │   ├── UiVm.MapLayers.cs：UiVm.MapLayers.cs
│   │   │   ├── UiVm.MapRender.cs：UiVm.MapRender.cs
│   │   │   ├── UiVm.MapWorld.cs：UiVm.MapWorld.cs
│   │   ├── Scene
│   │   │   ├── D2StaticModelDemo.cs：D2StaticModelDemo.cs
│   │   │   ├── SceneHistoryEntry.cs：SceneHistoryEntry.cs
│   │   │   ├── SceneRenderProjectionAdapter.cs：SceneRenderProjectionAdapter.cs
│   │   │   ├── StaticModelRenderAdapter.cs：StaticModelRenderAdapter.cs
│   │   │   ├── UiVm.DocumentStatus.cs：UiVm.DocumentStatus.cs
│   │   │   ├── UiVm.RenderProjection.cs：UiVm.RenderProjection.cs
│   │   │   ├── UiVm.Scene.cs：UiVm.Scene.cs
│   │   │   ├── UiVm.SceneDocument.New.cs：UiVm.SceneDocument.New.cs
│   │   │   ├── UiVm.SceneDocument.cs：UiVm.SceneDocument.cs
│   │   │   ├── UiVm.SceneDocumentLog.cs：UiVm.SceneDocumentLog.cs
│   │   │   ├── UiVm.SceneDocumentMapRef.cs：UiVm.SceneDocumentMapRef.cs
│   │   │   ├── UiVm.SceneDocumentSave.cs：UiVm.SceneDocumentSave.cs
│   │   │   ├── UiVm.StaticModelImport.cs：UiVm.StaticModelImport.cs
│   │   │   ├── UiVm.WorldProjection.cs：UiVm.WorldProjection.cs
│   │   ├── Selection
│   │   │   ├── UiVm.Picking.cs：UiVm.Picking.cs
│   │   │   ├── UiVm.Selection.cs：UiVm.Selection.cs
│   │   │   ├── UiVm.SelectionProjection.cs：UiVm.SelectionProjection.cs
│   │   │   ├── UiVm.SelectionTrace.cs：UiVm.SelectionTrace.cs
│   │   │   ├── UiVm.SelectionValidity.cs：UiVm.SelectionValidity.cs
│   │   │   ├── UiVm.ViewportSelection.cs：UiVm.ViewportSelection.cs
│   │   │   ├── ViewportPickingLogFormatter.cs：ViewportPickingLogFormatter.cs
│   │   ├── Transform
│   │   │   ├── Move
│   │   │   │   ├── UiVm.MoveGizmo.cs：UiVm.MoveGizmo.cs
│   │   │   │   ├── UiVm.MoveGizmoLogging.cs：UiVm.MoveGizmoLogging.cs
│   │   │   │   ├── UiVm.MoveGizmoScreenSize.cs：UiVm.MoveGizmoScreenSize.cs
│   │   │   ├── Rotate
│   │   │   │   ├── UiVm.RotateGizmo.cs：UiVm.RotateGizmo.cs
│   │   │   └── Scale
│   │   │   ├── UiVm.ScaleGizmo.cs：UiVm.ScaleGizmo.cs
│   │   │   ├── UiVm.InputGuards.cs：UiVm.InputGuards.cs
│   │   │   ├── UiVm.Interaction.cs：UiVm.Interaction.cs
│   │   │   ├── UiVm.InteractionCancel.cs：UiVm.InteractionCancel.cs
│   │   │   ├── UiVm.InteractionPointer.cs：UiVm.InteractionPointer.cs
│   │   │   ├── UiVm.Tool.cs：UiVm.Tool.cs
│   │   │   ├── UiVm.ViewportAssist.cs：UiVm.ViewportAssist.cs
│   │   └── Tree
│   │   ├── EditorTreeNode.cs：EditorTreeNode.cs
│   │   ├── TreeGuideBuilder.cs：TreeGuideBuilder.cs
│   │   ├── UiVm.TreeCommands.cs：UiVm.TreeCommands.cs
│   │   ├── UiVm.NativeHostLifecycle.cs：UiVm.NativeHostLifecycle.cs
│   │   ├── UiVm.cs：UiVm.cs
│   └── Win
│   ├── UiWin.Dialogs.cs：UiWin.Dialogs.cs
│   ├── UiWin.EntityShortcuts.cs：UiWin.EntityShortcuts.cs
│   ├── UiWin.MapCommands.cs：UiWin.MapCommands.cs
│   ├── UiWin.SceneCommands.cs：UiWin.SceneCommands.cs
│   ├── UiWin.UnsavedDialog.cs：UiWin.UnsavedDialog.cs
│   ├── UiWin.axaml：UiWin.axaml
│   ├── UiWin.axaml.cs：UiWin.axaml.cs
│   ├── NativeHostResizeCoalescer.cs：NativeHostResizeCoalescer.cs
│   ├── NativeHostResizeSnapshot.cs：NativeHostResizeSnapshot.cs
│   ├── NativeHostSurfaceContract.cs：NativeHostSurfaceContract.cs
│   ├── RelayCommand.cs：RelayCommand.cs
│   ├── TreeGuide.cs：TreeGuide.cs
│   ├── TreeGuideSegment.cs：TreeGuideSegment.cs
│   ├── Ui.axaml：Ui.axaml
│   ├── ViewportNativeHostRoute.cs：ViewportNativeHostRoute.cs
│   ├── XuanYu.Editor.UI.csproj：XuanYu.Editor.UI.csproj
│   ├── app.manifest：app.manifest
├── XuanYu.Editor.Win
│   ├── MainForm.cs：MainForm.cs
│   ├── XuanYu.Editor.Win.csproj：XuanYu.Editor.Win.csproj
├── XuanYu.Render.Abstractions
│   ├── EditorViewPlaneGridKind.cs：EditorViewPlaneGridKind.cs
│   ├── EditorViewportAssistState.cs：EditorViewportAssistState.cs
│   ├── FrameExecutionPolicy.cs：FrameExecutionPolicy.cs
│   ├── INativeHostSurfaceBridge.cs：INativeHostSurfaceBridge.cs
│   ├── INativeHostSurfaceBridgeFactory.cs：INativeHostSurfaceBridgeFactory.cs
│   ├── IRenderProjectionSource.cs：IRenderProjectionSource.cs
│   ├── MapBoundsGeometry.cs：MapBoundsGeometry.cs
│   ├── MapRenderSnapshot.cs：MapRenderSnapshot.cs
│   ├── MapSurfaceGeometry.cs：MapSurfaceGeometry.cs
│   ├── MapSurfaceResourceKey.cs：MapSurfaceResourceKey.cs
│   ├── MapSurfaceResourceUpdatePolicy.cs：MapSurfaceResourceUpdatePolicy.cs
│   ├── MapSurfaceResourceUpdateText.cs：MapSurfaceResourceUpdateText.cs
│   ├── NativeHostHandleSnapshot.cs：NativeHostHandleSnapshot.cs
│   ├── NativeHostLifecycleLogFormatter.cs：NativeHostLifecycleLogFormatter.cs
│   ├── NativeHostLifecycleProbe.cs：NativeHostLifecycleProbe.cs
│   ├── NativeHostLifecycleState.cs：NativeHostLifecycleState.cs
│   ├── NativeHostSurfaceHandle.cs：NativeHostSurfaceHandle.cs
│   ├── ReferenceGridScale.cs：ReferenceGridScale.cs
│   ├── RenderCameraProjection.cs：RenderCameraProjection.cs
│   ├── RenderDrawPlan.Typed.cs：RenderDrawPlan.Typed.cs
│   ├── RenderDrawPlan.cs：RenderDrawPlan.cs
│   ├── RenderEntityProjection.cs：RenderEntityProjection.cs
│   ├── RenderEntityType.cs：RenderEntityType.cs
│   ├── RenderProjection.cs：RenderProjection.cs
│   ├── RenderProjectionResult.cs：RenderProjectionResult.cs
│   ├── RenderStaticModelKey.cs：RenderStaticModelKey.cs
│   ├── RenderStaticModelPrimitive.cs：RenderStaticModelPrimitive.cs
│   ├── RenderStaticModelResource.cs：RenderStaticModelResource.cs
│   ├── RenderStaticModelVertex.cs：RenderStaticModelVertex.cs
│   ├── XuanYu.Render.Abstractions.csproj：XuanYu.Render.Abstractions.csproj
├── XuanYu.Render.Vulkan
│   ├── Bridge
│   │   ├── VulkanBridgeDeviceAttachStep.cs：VulkanBridgeDeviceAttachStep.cs
│   │   ├── VulkanBridgePhysicalDeviceAttachStep.cs：VulkanBridgePhysicalDeviceAttachStep.cs
│   │   ├── VulkanBridgeRenderSessionAttachStep.cs：VulkanBridgeRenderSessionAttachStep.cs
│   │   ├── VulkanBridgeSwapchainAttachStep.cs：VulkanBridgeSwapchainAttachStep.cs
│   ├── Device
│   │   ├── VulkanDeviceOwner.Physical.cs：VulkanDeviceOwner.Physical.cs
│   │   ├── VulkanDeviceOwner.cs：VulkanDeviceOwner.cs
│   │   ├── VulkanPhysicalDeviceInfo.cs：VulkanPhysicalDeviceInfo.cs
│   │   ├── VulkanPhysicalDeviceSelection.cs：VulkanPhysicalDeviceSelection.cs
│   │   ├── VulkanPhysicalDeviceSelector.cs：VulkanPhysicalDeviceSelector.cs
│   │   ├── VulkanQueueFamilySelection.cs：VulkanQueueFamilySelection.cs
│   ├── Diagnostic
│   │   ├── VulkanResizeTracer.cs：VulkanResizeTracer.cs
│   ├── Pipeline
│   │   ├── ShaderBytecode.Frag.cs：ShaderBytecode.Frag.cs
│   │   ├── ShaderBytecode.GridFrag.cs：ShaderBytecode.GridFrag.cs
│   │   ├── ShaderBytecode.GridVert.cs：ShaderBytecode.GridVert.cs
│   │   ├── ShaderBytecode.NavGizmoFrag.cs：ShaderBytecode.NavGizmoFrag.cs
│   │   ├── ShaderBytecode.NavGizmoVert.cs：nav gizmo vert
│   │   ├── ShaderBytecode.Vert.cs：ShaderBytecode.Vert.cs
│   │   ├── ShaderBytecode.ViewPlaneGridFrag.cs：ShaderBytecode.ViewPlaneGridFrag.cs
│   │   ├── ShaderBytecode.WorldAxesFrag.cs：ShaderBytecode.WorldAxesFrag.cs
│   │   ├── ShaderBytecode.WorldOriginFrag.cs：world origin frag
│   │   ├── VulkanGraphicsPipelineOwner.Depth.cs：VulkanGraphicsPipelineOwner.Depth.cs
│   │   ├── VulkanGraphicsPipelineOwner.Fullscreen.cs：VulkanGraphicsPipelineOwner.Fullscreen.cs
│   │   ├── VulkanGraphicsPipelineOwner.Grid.cs：VulkanGraphicsPipelineOwner.Grid.cs
│   │   ├── VulkanGraphicsPipelineOwner.Sky.cs：VulkanGraphicsPipelineOwner.Sky.cs
│   │   ├── VulkanGraphicsPipelineOwner.StaticModelInput.cs：VulkanGraphicsPipelineOwner.StaticModelInput.cs
│   │   ├── VulkanGraphicsPipelineOwner.cs：VulkanGraphicsPipelineOwner.cs
│   │   ├── VulkanPipelineLogFormatter.cs：VulkanPipelineLogFormatter.cs
│   │   ├── VulkanScenePushConstants.cs：VulkanScenePushConstants.cs
│   │   ├── VulkanShaderModuleOwner.cs：VulkanShaderModuleOwner.cs
│   ├── Render
│   │   ├── ClearFrame
│   │   │   ├── VulkanClearFrameLogFormatter.cs：VulkanClearFrameLogFormatter.cs
│   │   │   ├── VulkanClearFrameOwner.Commands.cs：VulkanClearFrameOwner.Commands.cs
│   │   │   ├── VulkanClearFrameOwner.Lifecycle.cs：VulkanClearFrameOwner.Lifecycle.cs
│   │   │   ├── VulkanClearFrameOwner.Matrix.cs：VulkanClearFrameOwner.Matrix.cs
│   │   │   ├── VulkanClearFrameOwner.PipelineBind.cs：VulkanClearFrameOwner.PipelineBind.cs
│   │   │   ├── VulkanClearFrameOwner.PushConstants.cs：VulkanClearFrameOwner.PushConstants.cs
│   │   │   ├── VulkanClearFrameOwner.Resources.cs：VulkanClearFrameOwner.Resources.cs
│   │   │   ├── VulkanClearFrameOwner.Trace.cs：VulkanClearFrameOwner.Trace.cs
│   │   │   ├── VulkanClearFrameOwner.cs：VulkanClearFrameOwner.cs
│   │   ├── Grid
│   │   │   ├── VulkanClearFrameOwner.Grid.cs：VulkanClearFrameOwner.Grid.cs
│   │   │   ├── VulkanClearFrameOwner.GridScale.cs：VulkanClearFrameOwner.GridScale.cs
│   │   │   ├── VulkanClearFrameOwner.NavGizmo.cs：VulkanClearFrameOwner.NavGizmo.cs
│   │   │   ├── VulkanClearFrameOwner.ViewPlaneGrid.cs：VulkanClearFrameOwner.ViewPlaneGrid.cs
│   │   │   ├── VulkanClearFrameOwner.WorldAxes.cs：VulkanClearFrameOwner.WorldAxes.cs
│   │   ├── Map
│   │   │   ├── VulkanClearFrameOwner.MapSurface.cs：VulkanClearFrameOwner.MapSurface.cs
│   │   ├── Present
│   │   │   ├── VulkanPresentLoop.Frame.cs：VulkanPresentLoop.Frame.cs
│   │   │   ├── VulkanPresentLoop.Lifecycle.cs：VulkanPresentLoop.Lifecycle.cs
│   │   │   ├── VulkanPresentLoop.cs：VulkanPresentLoop.cs
│   │   ├── Scene
│   │   │   ├── VulkanClearFrameOwner.Draw.cs：VulkanClearFrameOwner.Draw.cs
│   │   │   ├── VulkanClearFrameOwner.DrawAssist.cs：VulkanClearFrameOwner.DrawAssist.cs
│   │   │   ├── VulkanClearFrameOwner.DrawGizmo.cs：VulkanClearFrameOwner.DrawGizmo.cs
│   │   │   ├── VulkanClearFrameOwner.Scene.cs：VulkanClearFrameOwner.Scene.cs
│   │   └── StaticModels
│   │   ├── VulkanClearFrameOwner.DrawStaticBounds.cs：VulkanClearFrameOwner.DrawStaticBounds.cs
│   │   ├── VulkanClearFrameOwner.DrawStaticModel.cs：VulkanClearFrameOwner.DrawStaticModel.cs
│   │   ├── VulkanStaticModelBuffer.cs：VulkanStaticModelBuffer.cs
│   │   ├── VulkanStaticModelCache.cs：VulkanStaticModelCache.cs
│   │   ├── VulkanStaticModelFailureTracker.cs：VulkanStaticModelFailureTracker.cs
│   │   ├── VulkanStaticModelLog.cs：VulkanStaticModelLog.cs
│   │   ├── VulkanStaticModelResource.cs：VulkanStaticModelResource.cs
│   │   ├── VulkanStaticModelValidator.cs：VulkanStaticModelValidator.cs
│   │   ├── VulkanStaticModelVertex.cs：VulkanStaticModelVertex.cs
│   │   ├── VulkanDepthAttachment.cs：VulkanDepthAttachment.cs
│   ├── Session
│   │   ├── GridPipelineSet.cs：GridPipelineSet.cs
│   │   ├── VulkanRenderSession.Lifecycle.cs：VulkanRenderSession.Lifecycle.cs
│   │   ├── VulkanRenderSession.Recover.cs：VulkanRenderSession.Recover.cs
│   │   ├── VulkanRenderSession.Resize.cs：VulkanRenderSession.Resize.cs
│   │   ├── VulkanRenderSession.cs：VulkanRenderSession.cs
│   ├── Shaders
│   │   ├── editor_nav_gizmo.frag：玄域编辑器：Blender 风格导航 Gizmo
│   │   ├── editor_nav_gizmo.vert：导航 Gizmo Overlay Pass —— 顶点着色器。
│   │   ├── editor_reference_grid.frag：Blender 式统一尺度参考网格 —— 片元着色器。
│   │   ├── editor_reference_grid.vert：独立编辑器参考网格 Pass —— 顶点着色器。
│   │   ├── editor_view_plane_grid.frag：F3-F4：正交标准视图的视图平面网格（YZ/XZ 平面，以世界原点为基准）。
│   │   ├── editor_world_axes.frag：X/Y 世界轴独立全屏 Pass —— 片元着色器。
│   │   ├── editor_world_origin.frag：世界原点标记独立全屏 Pass —— 片元着色器（屏幕空间版）。
│   │   ├── scene.frag：每像素程序化编辑器环境（天空 + 中性灰参考地面）。
│   │   ├── scene.vert：F2-R3-R2：每像素背景——invVP（flat，每个顶点算一次）与背景 NDC（哨兵 (2,2) 表示非背景）。
│   └── Swapchain
│   ├── VulkanSwapchainBuilder.cs：VulkanSwapchainBuilder.cs
│   ├── VulkanSwapchainCapabilities.cs：VulkanSwapchainCapabilities.cs
│   ├── VulkanSwapchainLogFormatter.cs：VulkanSwapchainLogFormatter.cs
│   ├── VulkanSwapchainOwner.Accessors.cs：VulkanSwapchainOwner.Accessors.cs
│   ├── VulkanSwapchainOwner.cs：VulkanSwapchainOwner.cs
│   ├── VulkanApiProbe.cs：VulkanApiProbe.cs
│   ├── VulkanBridgeLogFormatter.cs：VulkanBridgeLogFormatter.cs
│   ├── VulkanDeviceInfo.cs：VulkanDeviceInfo.cs
│   ├── VulkanInstanceCreateInfoBuilder.cs：VulkanInstanceCreateInfoBuilder.cs
│   ├── VulkanInstanceExtensions.cs：VulkanInstanceExtensions.cs
│   ├── VulkanInstanceLogFormatter.cs：VulkanInstanceLogFormatter.cs
│   ├── VulkanInstanceOwner.cs：VulkanInstanceOwner.cs
│   ├── VulkanInstanceResult.cs：VulkanInstanceResult.cs
│   ├── VulkanNativeHostSurfaceBridge.Attach.cs：VulkanNativeHostSurfaceBridge.Attach.cs
│   ├── VulkanNativeHostSurfaceBridge.Lifecycle.cs：VulkanNativeHostSurfaceBridge.Lifecycle.cs
│   ├── VulkanNativeHostSurfaceBridge.Resize.cs：VulkanNativeHostSurfaceBridge.Resize.cs
│   ├── VulkanNativeHostSurfaceBridge.Scene.cs：VulkanNativeHostSurfaceBridge.Scene.cs
│   ├── VulkanNativeHostSurfaceBridge.cs：VulkanNativeHostSurfaceBridge.cs
│   ├── VulkanNativeHostSurfaceBridgeFactory.cs：VulkanNativeHostSurfaceBridgeFactory.cs
│   ├── VulkanProbeLogFormatter.cs：VulkanProbeLogFormatter.cs
│   ├── VulkanProbeResult.cs：VulkanProbeResult.cs
│   ├── VulkanSurfaceLogFormatter.cs：VulkanSurfaceLogFormatter.cs
│   ├── VulkanSurfaceOwner.cs：VulkanSurfaceOwner.cs
│   ├── VulkanSurfaceResult.cs：VulkanSurfaceResult.cs
│   ├── XuanYu.Render.Vulkan.csproj：XuanYu.Render.Vulkan.csproj
├── XuanYu.WarCore
│   ├── Identity
│   │   ├── FactionId.cs：FactionId.cs
│   │   ├── MilitaryIdentity.cs：MilitaryIdentity.cs
│   │   ├── OrganizationId.cs：OrganizationId.cs
│   │   ├── UnitId.cs：UnitId.cs
│   │   ├── UnitKind.cs：UnitKind.cs
│   └── State
│   ├── SoldierState.cs：SoldierState.cs
│   ├── XuanYu.WarCore.csproj：XuanYu.WarCore.csproj
├── XuanYu.WarCore.Tests
│   ├── Identity
│   │   ├── MilitaryIdentityTests.cs：MilitaryIdentityTests.cs
│   └── State
│   ├── SoldierStateTests.cs：SoldierStateTests.cs
│   ├── WarCoreDependencyTests.cs：WarCoreDependencyTests.cs
│   ├── XuanYu.WarCore.Tests.csproj：XuanYu.WarCore.Tests.csproj
├── XuanYu.World
│   ├── Map
│   │   ├── MapBounds.cs：MapBounds.cs
│   │   ├── MapDefaultDefinition.cs：MapDefaultDefinition.cs
│   │   ├── MapDefinition.cs：MapDefinition.cs
│   │   ├── MapDefinitionValidator.cs：MapDefinitionValidator.cs
│   │   ├── MapGeometry.cs：MapGeometry.cs
│   │   ├── MapId.cs：MapId.cs
│   │   ├── MapLayer.cs：MapLayer.cs
│   │   ├── MapLayerId.cs：MapLayerId.cs
│   │   ├── MapLayerKind.cs：MapLayerKind.cs
│   │   ├── MapLayerRules.cs：MapLayerRules.cs
│   │   ├── MapLayerStack.cs：MapLayerStack.cs
│   │   ├── MapLayerValidator.cs：MapLayerValidator.cs
│   │   ├── MapRegion.cs：MapRegion.cs
│   │   ├── MapRegionDraft.cs：MapRegionDraft.cs
│   │   ├── MapRegionId.cs：MapRegionId.cs
│   │   ├── MapRegionKind.cs：MapRegionKind.cs
│   │   ├── MapRegionValidator.cs：MapRegionValidator.cs
│   │   ├── MapSurfaceDefinition.cs：MapSurfaceDefinition.cs
│   │   ├── MapValidationResult.cs：MapValidationResult.cs
│   │   ├── WorldMapState.cs：WorldMapState.cs
│   │   ├── WorldMapStateOwner.cs：WorldMapStateOwner.cs
│   ├── Scene
│   │   ├── SceneSpatialBoundsProjection.cs：SceneSpatialBoundsProjection.cs
│   │   ├── SceneStateOwner.Lifecycle.cs：SceneStateOwner.Lifecycle.cs
│   │   ├── SceneStateOwner.Seeding.cs：SceneStateOwner.Seeding.cs
│   │   ├── SceneStateOwner.StaticModel.cs：SceneStateOwner.StaticModel.cs
│   │   ├── SceneStateOwner.Transform.cs：SceneStateOwner.Transform.cs
│   │   ├── SceneStateOwner.cs：SceneStateOwner.cs
│   │   ├── SceneWorldProjection.cs：SceneWorldProjection.cs
│   └── Spatial
│   ├── DynamicAabbTree.Insert.cs：DynamicAabbTree.Insert.cs
│   ├── DynamicAabbTree.Node.cs：DynamicAabbTree.Node.cs
│   ├── DynamicAabbTree.Query.cs：DynamicAabbTree.Query.cs
│   ├── DynamicAabbTree.Refit.cs：DynamicAabbTree.Refit.cs
│   ├── DynamicAabbTree.Remove.cs：DynamicAabbTree.Remove.cs
│   ├── DynamicAabbTree.cs：DynamicAabbTree.cs
│   ├── ISpatialIndex.cs：ISpatialIndex.cs
│   ├── SpatialIndexOwner.cs：SpatialIndexOwner.cs
│   ├── SpatialRaycastResolver.cs：SpatialRaycastResolver.cs
│   ├── EntityRegistry.Authoring.cs：EntityRegistry.Authoring.cs
│   ├── EntityRegistry.Replace.cs：EntityRegistry.Replace.cs
│   ├── EntityRegistry.cs：EntityRegistry.cs
│   ├── GlobalWorld.Authoring.cs：GlobalWorld.Authoring.cs
│   ├── GlobalWorld.Query.cs：GlobalWorld.Query.cs
│   ├── GlobalWorld.Snapshot.cs：GlobalWorld.Snapshot.cs
│   ├── GlobalWorld.cs：GlobalWorld.cs
│   ├── GridWorldPartitionStrategy.cs：GridWorldPartitionStrategy.cs
│   ├── IWorldPartitionStrategy.cs：IWorldPartitionStrategy.cs
│   ├── RegionKey.cs：RegionKey.cs
│   ├── WorldEntityActivity.cs：WorldEntityActivity.cs
│   ├── WorldEntityName.cs：WorldEntityName.cs
│   ├── WorldEntitySnapshot.cs：WorldEntitySnapshot.cs
│   ├── WorldEntityType.cs：WorldEntityType.cs
│   ├── WorldPartitionEntry.cs：WorldPartitionEntry.cs
│   ├── WorldPartitionMembership.cs：WorldPartitionMembership.cs
│   ├── WorldQuery.cs：WorldQuery.cs
│   ├── XuanYu.World.csproj：XuanYu.World.csproj
├── XuanYu.World.Tests
│   ├── Assets
│   │   ├── AssetContractTests.cs：AssetContractTests.cs
│   │   ├── AssetDialogTests.cs：AssetDialogTests.cs
│   │   ├── GlbFactory.cs：GlbFactory.cs
│   │   ├── GlbImportTests.cs：GlbImportTests.cs
│   │   ├── GlbMultiPrimitiveFactory.cs：GlbMultiPrimitiveFactory.cs
│   │   ├── HostingCompleteTests.cs：HostingCompleteTests.cs
│   │   ├── HostingPlannerRejectTests.cs：HostingPlannerRejectTests.cs
│   │   ├── HostingPlannerTests.cs：HostingPlannerTests.cs
│   │   ├── HostingRollbackTests.cs：HostingRollbackTests.cs
│   │   ├── HostingSaveAsTests.cs：HostingSaveAsTests.cs
│   │   ├── HostingTestEnv.cs：HostingTestEnv.cs
│   │   ├── HostingTransactionTests.cs：HostingTransactionTests.cs
│   │   ├── LoadStructureErrorTests.cs：LoadStructureErrorTests.cs
│   │   ├── LoadTransactionTests.cs：LoadTransactionTests.cs
│   │   ├── SaveAsTests.cs：SaveAsTests.cs
│   │   ├── SaveTransactionTests.cs：SaveTransactionTests.cs
│   │   ├── ScenePersistenceEnv.cs：ScenePersistenceEnv.cs
│   │   ├── SchemaCompatibilityTests.cs：SchemaCompatibilityTests.cs
│   │   ├── StaticModelAuthoringServiceTests.cs：StaticModelAuthoringServiceTests.cs
│   │   ├── StaticModelBaseVertexTests.cs：StaticModelBaseVertexTests.cs
│   │   ├── StaticModelCatalogTests.cs：StaticModelCatalogTests.cs
│   │   ├── StaticModelFailureTrackerTests.cs：StaticModelFailureTrackerTests.cs
│   │   ├── StaticModelProjectionTests.cs：StaticModelProjectionTests.cs
│   │   ├── StaticModelUiTests.cs：StaticModelUiTests.cs
│   │   ├── StaticModelValidatorTests.cs：StaticModelValidatorTests.cs
│   ├── Camera
│   │   ├── CameraDocumentTests.cs：CameraDocumentTests.cs
│   │   ├── CameraFramingOccupancyTests.cs：CameraFramingOccupancyTests.cs
│   │   ├── CameraFramingTests.cs：CameraFramingTests.cs
│   │   ├── CameraNavigationUiTests.cs：CameraNavigationUiTests.cs
│   │   ├── UiViewGizmoTests.cs：UiViewGizmoTests.cs
│   ├── Logging
│   │   ├── FootAxamlTailContractTests.cs：FootAxamlTailContractTests.cs
│   │   ├── LogAutoScrollPolicyTests.cs：LogAutoScrollPolicyTests.cs
│   │   ├── LogListAutoScrollControllerContractTests.cs：LogListAutoScrollControllerContractTests.cs
│   │   ├── UiMapLogChineseTests.cs：UiMapLogChineseTests.cs
│   │   ├── UiRootLogRowContractTests.cs：UiRootLogRowContractTests.cs
│   ├── Map
│   │   └── Editing
│   │   ├── MapLayerSessionTests.Behavior.cs：MapLayerSessionTests.Behavior.cs
│   │   ├── MapLayerSessionTests.cs：MapLayerSessionTests.cs
│   │   ├── UiMapCommandRoutingTests.cs：UiMapCommandRoutingTests.cs
│   │   ├── UiMapEditorTests.cs：UiMapEditorTests.cs
│   │   ├── UiMapHistoryTests.cs：UiMapHistoryTests.cs
│   │   ├── UiMapInitialProjectionTests.cs：UiMapInitialProjectionTests.cs
│   │   ├── UiMapLayerPanelTests.Behavior.cs：UiMapLayerPanelTests.Behavior.cs
│   │   ├── UiMapLayerPanelTests.cs：UiMapLayerPanelTests.cs
│   │   ├── UiMapLayoutContractTests.cs：图层 UI 归位源码合同测试
│   │   ├── MapBoundsTests.cs：MapBoundsTests.cs
│   │   ├── MapCoordinateValidationTests.cs：MapCoordinateValidationTests.cs
│   │   ├── MapDefaultMapTests.cs：MapDefaultMapTests.cs
│   │   ├── MapDefinitionTests.cs：MapDefinitionTests.cs
│   │   ├── MapDocumentAggregateBridgeTests.cs：MapDocumentAggregateBridgeTests.cs
│   │   ├── MapDocumentOwnerChainTests.cs：MapDocumentOwnerChainTests.cs
│   │   ├── MapDocumentOwnerTests.cs：MapDocumentOwnerTests.cs
│   │   ├── MapEnvironmentValidationTests.cs：MapEnvironmentValidationTests.cs
│   │   ├── MapIdTests.cs：MapIdTests.cs
│   │   ├── MapJsonRoundTripTests.cs：MapJsonRoundTripTests.cs
│   │   ├── MapJsonStrictnessTests.cs：MapJsonStrictnessTests.cs
│   │   ├── MapLayerRulesTests.cs：MapLayerRulesTests.cs
│   │   ├── MapLayerStackTests.Order.cs：MapLayerStackTests.Order.cs
│   │   ├── MapLayerStackTests.cs：MapLayerStackTests.cs
│   │   ├── MapLayerTests.Base.cs：MapLayerTests.Base.cs
│   │   ├── MapLayerTests.cs：MapLayerTests.cs
│   │   ├── MapRegionDraftTests.cs：MapRegionDraftTests.cs
│   │   ├── MapRegionTests.Helpers.cs：MapRegionTests.Helpers.cs
│   │   ├── MapRegionTests.Strictness.cs：MapRegionTests.Strictness.cs
│   │   ├── MapRegionTests.cs：MapRegionTests.cs
│   │   ├── MapSizeValidationTests.cs：MapSizeValidationTests.cs
│   │   ├── MapStorageFailureTests.cs：MapStorageFailureTests.cs
│   │   ├── MapStorageTests.cs：MapStorageTests.cs
│   │   ├── MapSurfaceSamplerTests.cs：MapSurfaceSamplerTests.cs
│   │   ├── MapSurfaceValidationTests.cs：MapSurfaceValidationTests.cs
│   │   ├── SceneMapReferenceTests.cs：SceneMapReferenceTests.cs
│   │   ├── WorldMapStateOwnerTests.cs：WorldMapStateOwnerTests.cs
│   │   ├── WorldMapStateTests.cs：WorldMapStateTests.cs
│   ├── MapEditing
│   │   ├── MapEditSessionCommandTests.cs：MapEditSessionCommandTests.cs
│   │   ├── MapEditSessionCreationTests.cs：MapEditSessionCreationTests.cs
│   │   ├── MapEditSessionDirtyTests.cs：MapEditSessionDirtyTests.cs
│   │   ├── MapEditSessionHistoryTests.cs：MapEditSessionHistoryTests.cs
│   │   ├── MapEditSessionMapPropertiesTests.cs：MapEditSessionMapPropertiesTests.cs
│   │   ├── MapEditSessionSelectionTests.cs：MapEditSessionSelectionTests.cs
│   │   ├── MapEditSessionThreadTests.cs：MapEditSessionThreadTests.cs
│   │   ├── MapEditSessionValidationTests.cs：MapEditSessionValidationTests.cs
│   │   ├── MapRenderSnapshotProjectionTests.cs：MapRenderSnapshotProjectionTests.cs
│   ├── Render
│   │   ├── VulkanPresentLoopContractTests.cs：VulkanPresentLoopContractTests.cs
│   │   ├── VulkanPresentModeSelectionTests.cs：VulkanPresentModeSelectionTests.cs
│   ├── Scene
│   │   ├── CommandSmokeTests.cs：CommandSmokeTests.cs
│   │   ├── EditorEnvironmentTests.cs：EditorEnvironmentTests.cs
│   │   ├── EntityBoundsSemanticsTests.cs：EntityBoundsSemanticsTests.cs
│   │   ├── EntityRegistryTests.cs：EntityRegistryTests.cs
│   │   ├── EntityTests.cs：EntityTests.cs
│   │   ├── FinalSceneTests.cs：FinalSceneTests.cs
│   │   ├── GlobalWorldTests.cs：GlobalWorldTests.cs
│   │   ├── SceneConsumptionTests.cs：SceneConsumptionTests.cs
│   │   ├── SceneDocumentPersistenceTests.cs：SceneDocumentPersistenceTests.cs
│   │   ├── SceneDocumentTests.Opening.cs：SceneDocumentTests.Opening.cs
│   │   ├── SceneDocumentTests.SaveFeedback.cs：SceneDocumentTests.SaveFeedback.cs
│   │   ├── SceneDocumentTests.cs：SceneDocumentTests.cs
│   │   ├── SceneIsolationTests.cs：SceneIsolationTests.cs
│   │   ├── SceneMultiEntityGateTests.cs：SceneMultiEntityGateTests.cs
│   │   ├── SceneSelectionReentryTests.cs：SceneSelectionReentryTests.cs
│   │   ├── SceneSingleAuthorityTests.cs：SceneSingleAuthorityTests.cs
│   │   ├── UiHistoryTests.InlineRename.cs：UiHistoryTests.InlineRename.cs
│   │   ├── UiHistoryTests.cs：UiHistoryTests.cs
│   ├── Selection
│   │   ├── FinalSelectionTests.cs：FinalSelectionTests.cs
│   │   ├── SelectionToolStateUiTests.cs：SelectionToolStateUiTests.cs
│   │   ├── ToolStateHighlightUiTests.Selection.cs：ToolStateHighlightUiTests.Selection.cs
│   │   ├── ToolStateHighlightUiTests.cs：ToolStateHighlightUiTests.cs
│   ├── Spatial
│   │   ├── SceneStateOwnerSpatialTests.cs：SceneStateOwnerSpatialTests.cs
│   │   ├── SpatialIndexEditLifecycleTests.cs：SpatialIndexEditLifecycleTests.cs
│   │   ├── SpatialIndexOwnerLifecycleTests.cs：SpatialIndexOwnerLifecycleTests.cs
│   │   ├── SpatialIndexOwnerRevisionTests.cs：SpatialIndexOwnerRevisionTests.cs
│   │   ├── SpatialIndexRebuildTests.cs：SpatialIndexRebuildTests.cs
│   │   ├── SpatialIndexScaleTests.cs：SpatialIndexScaleTests.cs
│   │   ├── SpatialQueryGovernanceTests.cs：SpatialQueryGovernanceTests.cs
│   │   ├── SpatialQueryOracle.cs：SpatialQueryOracle.cs
│   │   ├── SpatialQueryTests.Geometry.cs：SpatialQueryTests.Geometry.cs
│   │   ├── SpatialQueryTests.cs：SpatialQueryTests.cs
│   │   ├── SpatialRayQueryLifecycleTests.cs：SpatialRayQueryLifecycleTests.cs
│   │   ├── SpatialRayQueryTests.cs：SpatialRayQueryTests.cs
│   │   ├── SpatialRaycastNearestTests.cs：SpatialRaycastNearestTests.cs
│   │   ├── SpatialRaycastRevisionTests.cs：SpatialRaycastRevisionTests.cs
│   │   ├── SpatialRaycastScaleTests.cs：SpatialRaycastScaleTests.cs
│   │   ├── SpatialTestData.cs：SpatialTestData.cs
│   ├── Transform
│   │   ├── Move
│   │   │   ├── MoveTransformUiTests.Plane.cs：MoveTransformUiTests.Plane.cs
│   │   │   ├── MoveTransformUiTests.Region.cs：MoveTransformUiTests.Region.cs
│   │   │   ├── MoveTransformUiTests.Session.cs：MoveTransformUiTests.Session.cs
│   │   │   ├── MoveTransformUiTests.cs：MoveTransformUiTests.cs
│   │   ├── Rotate
│   │   │   ├── RotateTransformUiTests.DragState.cs：RotateTransformUiTests.DragState.cs
│   │   │   ├── RotateTransformUiTests.Helpers.cs：RotateTransformUiTests.Helpers.cs
│   │   │   ├── RotateTransformUiTests.Preview.cs：RotateTransformUiTests.Preview.cs
│   │   │   ├── RotateTransformUiTests.ToolSwitch.cs：RotateTransformUiTests.ToolSwitch.cs
│   │   │   ├── RotateTransformUiTests.cs：RotateTransformUiTests.cs
│   │   └── Scale
│   │   ├── ScaleGizmoGlobalModeTests.cs：ScaleGizmoGlobalModeTests.cs
│   │   ├── ScaleTransformUiTests.AxisUniform.cs：ScaleTransformUiTests.AxisUniform.cs
│   │   ├── ScaleTransformUiTests.Helpers.cs：ScaleTransformUiTests.Helpers.cs
│   │   ├── ScaleTransformUiTests.History.cs：ScaleTransformUiTests.History.cs
│   │   ├── ScaleTransformUiTests.Pointer.cs：ScaleTransformUiTests.Pointer.cs
│   │   ├── ScaleTransformUiTests.Target.cs：ScaleTransformUiTests.Target.cs
│   │   ├── ScaleTransformUiTests.cs：ScaleTransformUiTests.cs
│   │   ├── TransformFoundationTests.Input.cs：TransformFoundationTests.Input.cs
│   │   ├── TransformFoundationTests.Inspector.cs：TransformFoundationTests.Inspector.cs
│   │   ├── TransformFoundationTests.cs：TransformFoundationTests.cs
│   │   ├── TransformSessionTests.cs：TransformSessionTests.cs
│   │   ├── ViewportAssistTests.cs：ViewportAssistTests.cs
│   ├── Tree
│   │   ├── UiHierarchyConnectorTests.cs：UiHierarchyConnectorTests.cs
│   │   ├── UiTreeGuideTests.cs：UiTreeGuideTests.cs
│   │   ├── UiTreeToggleTests.cs：UiTreeToggleTests.cs
│   └── WorldPartition
│   ├── WorldPartitionInvariantTests.cs：WorldPartitionInvariantTests.cs
│   ├── WorldPartitionMigrationTests.Activity.cs：WorldPartitionMigrationTests.Activity.cs
│   ├── WorldPartitionMigrationTests.cs：WorldPartitionMigrationTests.cs
│   ├── WorldPartitionTests.PartitionStrategy.cs：WorldPartitionTests.PartitionStrategy.cs
│   ├── WorldPartitionTests.cs：WorldPartitionTests.cs
│   ├── WorldPartitionUiTests.cs：WorldPartitionUiTests.cs
│   ├── XuanYu.World.Tests.csproj：XuanYu.World.Tests.csproj
├── docs
│   ├── architecture
│   │   ├── ENGINE_ARCHITECTURE.md：ENGINE_ARCHITECTURE.md
│   │   ├── world-a-r0-coordinate-contract.md：world-a-r0-coordinate-contract.md
│   ├── archive
│   │   └── changelog
│   │   ├── changelog-2026-05.md：changelog-2026-05.md
│   │   ├── changelog-2026-06.md：changelog-2026-06.md
│   │   ├── changelog-2026-07.md：changelog-2026-07.md
│   ├── governance
│   │   └── debts
│   │   ├── arch-world-debts.md：arch-world-debts.md
│   │   ├── NAMING_RULES.md：NAMING_RULES.md
│   │   ├── dev-rules-understanding.md：dev-rules-understanding.md
│   │   ├── diagnostic-safety.md：启动期将 UI 日志回调注入纯逻辑层
│   │   ├── naming-XuanYu-Engine.md：naming-XuanYu-Engine.md
│   │   ├── 版本号规范与历史映射.md：版本号规范与历史映射.md
│   └── milestones
│   └── current
│   └── MAP-A
│   ├── map-contract.md：map-contract.md
│   ├── CODE_CONSTITUTION.md：CODE_CONSTITUTION.md
│   ├── dev-rules.md：dev-rules.md
│   ├── docs-index.md：docs-index.md
│   ├── 玄域引擎_AI开发宪法.md：玄域引擎_AI开发宪法.md
├── samples
│   ├── world-c-r1-ten-triangles.xyscene：world-c-r1-ten-triangles.xyscene
└── scripts
├── arch-a-guard-editor.ps1：arch-a-guard-editor.ps1
├── arch-a-guard-render.ps1：arch-a-guard-render.ps1
├── arch-a-guard-warcore.ps1：arch-a-guard-warcore.ps1
├── arch-a-guard-world.ps1：arch-a-guard-world.ps1
├── arch-a-guard.ps1：arch-a-guard.ps1
├── .gitattributes：Git 属性规则
├── .gitignore：Git 忽略规则
├── AGENTS.md：AGENTS.md
├── NuGet.Config：配置文件
├── XuanYu.Engine.slnx：解决方案文件
├── changelog.md：changelog
├── file-tree.md：XuanYu Engine 文件树
├── run.bat：启动脚本
```
