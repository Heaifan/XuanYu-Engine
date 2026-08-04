# XuanYu Engine 文件树

> 本文仅描述当前仓库结构与文件职责，不记录版本历史、阶段过程、迁移记录或测试统计。

```text
XuanYuEngine/
├── .gitattributes
├── .gitignore
├── NuGet.Config
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
│   │   ├── MoveGizmoScreenSize.cs
│   │   ├── MoveGizmoSegment.cs
│   │   ├── RotateGizmoAxis.cs
│   │   ├── RotateGizmoDrag.Math.cs
│   │   ├── RotateGizmoDrag.cs
│   │   ├── RotateGizmoLayout.cs
│   │   ├── RotateGizmoRing.cs
│   │   ├── RotateGizmoScreenRadius.cs
│   │   ├── ScaleGizmoAxis.cs
│   │   ├── ScaleGizmoDrag.cs
│   │   ├── ScaleGizmoHitTester.cs
│   │   ├── ScaleGizmoLayout.cs
│   │   ├── ScaleGizmoScreenSize.cs
│   │   └── ScreenPoint.cs
│   ├── History
│   │   ├── EditorHistoryOwner.cs
│   │   └── TransformHistoryEntry.cs
│   ├── Identity
│   │   └── EntityId.cs
│   ├── Logging
│   │   ├── EngineLogEntry.cs
│   │   └── EngineLogLevel.cs
│   ├── Map
│   │   ├── MapSurfaceKind.cs
│   │   ├── MapSurfaceSampler.cs
│   │   └── MapTerrainVertex.cs
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
│   │   ├── EngineError.cs
│   │   └── EngineResult.cs
│   ├── Scene
│   │   ├── CommittedTransform.cs
│   │   ├── ISceneRenderSnapshotSource.cs
│   │   ├── SceneEntitySnapshot.cs
│   │   ├── SceneRenderSnapshot.cs
│   │   └── SceneTransformCommitResult.cs
│   ├── Space
│   │   ├── CameraState.cs
│   │   ├── DefaultEditorCamera.cs
│   │   ├── ProjectionMode.cs
│   │   ├── ViewProjectionState.cs
│   │   ├── ViewportState.cs
│   │   ├── WorldRay.cs
│   │   └── WorldRayFactory.cs
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
│   │   ├── MapSurfaceResourceKeyTests.cs：资源键合同测试（Rename 不重建等）
│   │   ├── MapSurfaceResourceUpdatePolicyTests.cs：资源更新决策纯策略测试
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
│   │   ├── SceneAssetPathPolicy.cs
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
│   │   ├── CameraBasis.cs
│   │   ├── CameraFrameResult.cs
│   │   ├── CameraNavigation.Try.cs
│   │   ├── CameraNavigation.cs
│   │   ├── EditorCameraFraming.Orthographic.cs
│   │   ├── EditorCameraFraming.cs
│   │   └── OrthographicViewFactory.cs
│   ├── MapDocument
│   │   ├── MapDocument.cs
│   │   ├── MapDocumentAggregateBridge.cs
│   │   ├── MapDocumentJson.cs
│   │   ├── MapDocumentOwner.cs
│   │   ├── MapDocumentResult.cs
│   │   ├── MapDocumentValidator.cs
│   │   ├── MapEnvironmentDefinition.cs
│   │   ├── MapJsonMapper.cs
│   │   ├── MapJsonSerializer.cs
│   │   └── MapStorageService.cs
│   ├── MapEditing
│   │   ├── MapEditEvents.cs
│   │   ├── MapEditReason.cs
│   │   ├── MapEditSession.Commands.cs
│   │   ├── MapEditSession.Commit.cs
│   │   ├── MapEditSession.Document.cs
│   │   ├── MapEditSession.History.cs
│   │   ├── MapEditSession.Selection.cs
│   │   ├── MapEditSession.cs
│   │   ├── MapHistoryEntry.cs
│   │   ├── MapSelection.cs
│   │   └── MapSelectionKind.cs
│   ├── SceneDocument
│   │   ├── MapReference.cs
│   │   ├── SceneDocumentAsset.cs
│   │   ├── SceneDocumentEntity.cs
│   │   ├── SceneDocumentJson.cs
│   │   ├── SceneDocumentLoadTransaction.cs
│   │   ├── SceneDocumentMapper.cs
│   │   ├── SceneDocumentResult.cs
│   │   ├── SceneDocumentSaveTransaction.cs
│   │   ├── SceneDocumentSession.cs
│   │   ├── SceneDocumentSnapshot.cs
│   │   ├── SceneDocumentValidator.MapReference.cs
│   │   ├── SceneDocumentValidator.cs
│   │   ├── SceneDocumentWorldBridge.cs
│   │   ├── SceneLoadCandidate.cs
│   │   ├── SceneSaveOutcome.cs
│   │   └── SceneStorageService.cs
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
│   │   ├── MapEditorPanel.axaml
│   │   ├── MapEditorPanel.axaml.cs
│   │   ├── Right.axaml
│   │   └── Right.axaml.cs
│   ├── Root
│   │   ├── UiRoot.axaml
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
│   │       ├── VulkanViewport.axaml
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
│   │   ├── MapRenderSnapshotProjection.cs
│   │   ├── SampleLogEntries.cs
│   │   ├── SceneHistoryEntry.cs
│   │   ├── SceneRenderProjectionAdapter.cs
│   │   ├── StandardViewResolver.cs
│   │   ├── StaticModelRenderAdapter.cs
│   │   ├── TreeGuideBuilder.cs
│   │   ├── UiText.cs
│   │   ├── UiVm.Camera.Framing.cs
│   │   ├── UiVm.Camera.cs
│   │   ├── UiVm.CameraNavigation.cs
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
│   │   ├── UiVm.MapEditor.cs
│   │   ├── UiVm.MapRender.cs
│   │   ├── UiVm.MapWorld.cs
│   │   ├── UiVm.MoveGizmo.cs
│   │   ├── UiVm.MoveGizmoLogging.cs
│   │   ├── UiVm.MoveGizmoScreenSize.cs
│   │   ├── UiVm.NativeHostLifecycle.cs
│   │   ├── UiVm.Picking.cs
│   │   ├── UiVm.RenderProjection.cs
│   │   ├── UiVm.RotateGizmo.cs
│   │   ├── UiVm.ScaleGizmo.cs
│   │   ├── UiVm.Scene.cs
│   │   ├── UiVm.SceneDocument.New.cs
│   │   ├── UiVm.SceneDocument.cs
│   │   ├── UiVm.SceneDocumentLog.cs
│   │   ├── UiVm.SceneDocumentMapRef.cs
│   │   ├── UiVm.SceneDocumentSave.cs
│   │   ├── UiVm.Selection.cs
│   │   ├── UiVm.SelectionProjection.cs
│   │   ├── UiVm.SelectionTrace.cs
│   │   ├── UiVm.SelectionValidity.cs
│   │   ├── UiVm.StaticModelImport.cs
│   │   ├── UiVm.Tool.cs
│   │   ├── UiVm.TreeCommands.cs
│   │   ├── UiVm.ViewGizmo.cs
│   │   ├── UiVm.ViewportAssist.cs
│   │   ├── UiVm.ViewportSelection.cs
│   │   ├── UiVm.WorldProjection.cs
│   │   ├── UiVm.cs
│   │   └── ViewportPickingLogFormatter.cs
│   ├── Win
│   │   ├── UiWin.Dialogs.cs
│   │   ├── UiWin.EntityShortcuts.cs
│   │   ├── UiWin.MapCommands.cs
│   │   ├── UiWin.SceneCommands.cs
│   │   ├── UiWin.UnsavedDialog.cs
│   │   ├── UiWin.axaml
│   │   └── UiWin.axaml.cs
│   ├── XuanYu.Editor.UI.csproj
│   └── app.manifest
├── XuanYu.Editor.Win
│   ├── MainForm.cs
│   └── XuanYu.Editor.Win.csproj
├── XuanYu.Engine.slnx
├── XuanYu.Render.Abstractions
│   ├── EditorViewPlaneGridKind.cs
│   ├── EditorViewportAssistState.cs
│   ├── FrameExecutionPolicy.cs
│   ├── INativeHostSurfaceBridge.cs
│   ├── INativeHostSurfaceBridgeFactory.cs
│   ├── IRenderProjectionSource.cs
│   ├── MapBoundsGeometry.cs
│   ├── MapRenderSnapshot.cs
│   ├── MapSurfaceGeometry.cs
│   ├── MapSurfaceResourceKey.cs：地图 GPU 资源判等键（不含 ChangeSequence）
│   ├── MapSurfaceResourceUpdatePolicy.cs：地图资源更新决策（旧序拒绝/同键不重建）
│   ├── NativeHostHandleSnapshot.cs
│   ├── NativeHostLifecycleLogFormatter.cs
│   ├── NativeHostLifecycleProbe.cs
│   ├── NativeHostLifecycleState.cs
│   ├── NativeHostSurfaceHandle.cs
│   ├── ReferenceGridScale.cs
│   ├── RenderCameraProjection.cs
│   ├── RenderDrawPlan.cs
│   ├── RenderEntityProjection.cs
│   ├── RenderEntityType.cs
│   ├── RenderProjection.cs
│   ├── RenderProjectionResult.cs
│   ├── RenderStaticModelKey.cs
│   ├── RenderStaticModelPrimitive.cs
│   ├── RenderStaticModelResource.cs
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
│   │   ├── ShaderBytecode.Frag.cs
│   │   ├── ShaderBytecode.GridFrag.cs
│   │   ├── ShaderBytecode.GridVert.cs
│   │   ├── ShaderBytecode.NavGizmoFrag.cs
│   │   ├── ShaderBytecode.NavGizmoVert.cs
│   │   ├── ShaderBytecode.Vert.cs
│   │   ├── ShaderBytecode.ViewPlaneGridFrag.cs
│   │   ├── ShaderBytecode.WorldAxesFrag.cs
│   │   ├── ShaderBytecode.WorldOriginFrag.cs
│   │   ├── VulkanGraphicsPipelineOwner.Depth.cs
│   │   ├── VulkanGraphicsPipelineOwner.Fullscreen.cs
│   │   ├── VulkanGraphicsPipelineOwner.Grid.cs
│   │   ├── VulkanGraphicsPipelineOwner.Sky.cs
│   │   ├── VulkanGraphicsPipelineOwner.StaticModelInput.cs
│   │   ├── VulkanGraphicsPipelineOwner.cs
│   │   ├── VulkanPipelineLogFormatter.cs
│   │   ├── VulkanScenePushConstants.cs
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
│   │   ├── VulkanClearFrameOwner.Draw.cs
│   │   ├── VulkanClearFrameOwner.DrawAssist.cs
│   │   ├── VulkanClearFrameOwner.DrawGizmo.cs
│   │   ├── VulkanClearFrameOwner.DrawStaticBounds.cs
│   │   ├── VulkanClearFrameOwner.DrawStaticModel.cs
│   │   ├── VulkanClearFrameOwner.Grid.cs
│   │   ├── VulkanClearFrameOwner.GridScale.cs
│   │   ├── VulkanClearFrameOwner.Lifecycle.cs
│   │   ├── VulkanClearFrameOwner.MapSurface.cs
│   │   ├── VulkanClearFrameOwner.Matrix.cs
│   │   ├── VulkanClearFrameOwner.NavGizmo.cs
│   │   ├── VulkanClearFrameOwner.PipelineBind.cs
│   │   ├── VulkanClearFrameOwner.PushConstants.cs
│   │   ├── VulkanClearFrameOwner.Resources.cs
│   │   ├── VulkanClearFrameOwner.Scene.cs
│   │   ├── VulkanClearFrameOwner.Trace.cs
│   │   ├── VulkanClearFrameOwner.ViewPlaneGrid.cs
│   │   ├── VulkanClearFrameOwner.WorldAxes.cs
│   │   ├── VulkanClearFrameOwner.cs
│   │   ├── VulkanDepthAttachment.cs
│   │   ├── VulkanPresentLoop.Frame.cs
│   │   ├── VulkanPresentLoop.Lifecycle.cs
│   │   └── VulkanPresentLoop.cs
│   ├── Session
│   │   ├── GridPipelineSet.cs
│   │   ├── VulkanRenderSession.Lifecycle.cs
│   │   ├── VulkanRenderSession.Recover.cs
│   │   ├── VulkanRenderSession.Resize.cs
│   │   └── VulkanRenderSession.cs
│   ├── Shaders
│   │   ├── editor_nav_gizmo.frag
│   │   ├── editor_nav_gizmo.vert
│   │   ├── editor_reference_grid.frag
│   │   ├── editor_reference_grid.vert
│   │   ├── editor_view_plane_grid.frag
│   │   ├── editor_world_axes.frag
│   │   ├── editor_world_origin.frag
│   │   ├── scene.frag
│   │   └── scene.vert
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
│   │   ├── MapBounds.cs
│   │   ├── MapDefaultDefinition.cs
│   │   ├── MapDefinition.cs
│   │   ├── MapDefinitionValidator.cs
│   │   ├── MapGeometry.cs
│   │   ├── MapId.cs
│   │   ├── MapLayer.cs
│   │   ├── MapLayerId.cs
│   │   ├── MapLayerKind.cs
│   │   ├── MapLayerValidator.cs
│   │   ├── MapRegion.cs
│   │   ├── MapRegionDraft.cs
│   │   ├── MapRegionId.cs
│   │   ├── MapRegionKind.cs
│   │   ├── MapRegionValidator.cs
│   │   ├── MapSurfaceDefinition.cs
│   │   ├── MapValidationResult.cs
│   │   ├── WorldMapState.cs
│   │   └── WorldMapStateOwner.cs
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
│   │   ├── MapEditSessionMapPropertiesTests.cs：地图属性原子提交测试（单历史/零污染）
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
│   │   ├── UiMapInitialProjectionTests.cs：默认地图首帧投影测试
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
├── changelog.md
├── docs
│   ├── CODE_CONSTITUTION.md
│   ├── architecture
│   │   ├── ENGINE_ARCHITECTURE.md
│   │   └── world-a-r0-coordinate-contract.md
│   ├── archive
│   │   ├── changelog
│   │   │   ├── changelog-2026-05.md
│   │   │   ├── changelog-2026-06.md
│   │   │   └── changelog-2026-07.md
│   │   └── superseded
│   │       ├── AI_DEVELOPMENT_RULES.md
│   │       └── LEGACY_FLUIDWARFARE_OLD_AUDIT.md
│   ├── dev-rules.md
│   ├── docs-index.md
│   ├── governance
│   │   ├── NAMING_RULES.md
│   │   ├── debts
│   │   │   └── arch-world-debts.md
│   │   ├── dev-rules-understanding.md
│   │   ├── diagnostic-safety.md
│   │   ├── naming-XuanYu-Engine.md
│   │   ├── shr-2026-08-closure.svg
│   │   └── 版本号规范与历史映射.md
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
│   │           ├── map-a-r1-d1-map-contracts.md
│   │           ├── map-a-r1-d5-r1-f2-grid-stabilize.svg
│   │           ├── map-a-r1-d5-r1-f2-r2-unified-grid-lod.svg
│   │           ├── map-a-r1-d5-r1-f2-r3-grid-ground-visual.svg
│   │           ├── map-a-r1-d5-r1-f2-r3-r2-per-pixel-background.svg
│   │           ├── map-a-r1-d5-r1-f3-f1-overlay-gizmo.svg
│   │           ├── map-a-r1-d5-r1-f3-f2-camera-basis-recovery.svg
│   │           ├── map-a-r1-d5-r1-f3-f3-gizmo-recovery.svg
│   │           └── map-a-r1-d5-r1-f3-viewport-navigation-gizmo.svg
│   └── 玄域引擎_AI开发宪法.md
├── file-tree.md
├── run.bat
├── samples
│   └── world-c-r1-ten-triangles.xyscene
└── scripts
    ├── arch-a-guard-editor.ps1
    ├── arch-a-guard-render.ps1
    ├── arch-a-guard-warcore.ps1
    ├── arch-a-guard-world.ps1
    └── arch-a-guard.ps1
```
