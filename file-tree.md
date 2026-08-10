# 玄域引擎文件树（自动重建）

> 由 `git ls-files` 全量重建（每轮收口时）；职责为一句摘要，非权威规范。

## 目录树

```
├─ .gitattributes
├─ .gitignore
├─ AGENTS.md
├─ NuGet.Config
├─ XuanYu.Core/
│  ├─ .gitkeep
│  ├─ Diagnostics/
│  │  └─ CoreSelfTest.cs
│  ├─ Gizmo/
│  │  ├─ Common/
│  │  │  └─ ScreenPoint.cs
│  │  ├─ Move/
│  │  │  ├─ MoveGizmoAxis.cs
│  │  │  ├─ MoveGizmoDragConstraint.Axes.cs
│  │  │  ├─ MoveGizmoDragConstraint.cs
│  │  │  ├─ MoveGizmoLayout.Hit.cs
│  │  │  ├─ MoveGizmoLayout.Plane.cs
│  │  │  ├─ MoveGizmoLayout.cs
│  │  │  ├─ MoveGizmoPlane.cs
│  │  │  ├─ MoveGizmoScreenSize.cs
│  │  │  └─ MoveGizmoSegment.cs
│  │  ├─ Rotate/
│  │  │  ├─ RotateGizmoAxis.cs
│  │  │  ├─ RotateGizmoDrag.Math.cs
│  │  │  ├─ RotateGizmoDrag.cs
│  │  │  ├─ RotateGizmoLayout.cs
│  │  │  ├─ RotateGizmoRing.cs
│  │  │  └─ RotateGizmoScreenRadius.cs
│  │  └─ Scale/
│  │     ├─ ScaleGizmoAxis.cs
│  │     ├─ ScaleGizmoDrag.cs
│  │     ├─ ScaleGizmoHitTester.cs
│  │     ├─ ScaleGizmoLayout.cs
│  │     └─ ScaleGizmoScreenSize.cs
│  ├─ History/
│  │  ├─ EditorHistoryOwner.cs
│  │  └─ TransformHistoryEntry.cs
│  ├─ Identity/
│  │  └─ EntityId.cs
│  ├─ Logging/
│  │  ├─ EngineLogEntry.cs
│  │  └─ EngineLogLevel.cs
│  ├─ Map/
│  │  ├─ MapSurfaceKind.cs
│  │  ├─ MapSurfaceSampler.cs
│  │  └─ MapTerrainVertex.cs
│  ├─ Math/
│  │  ├─ Vector3d.cs
│  │  └─ YawRotation.cs
│  ├─ Picking/
│  │  ├─ ViewportPickingRequest.cs
│  │  ├─ ViewportPickingResult.cs
│  │  └─ ViewportPickingService.cs
│  ├─ Properties/
│  │  └─ AssemblyInfo.cs
│  ├─ Results/
│  │  ├─ EngineError.cs
│  │  └─ EngineResult.cs
│  ├─ Scene/
│  │  ├─ CommittedTransform.cs
│  │  ├─ ISceneRenderSnapshotSource.cs
│  │  ├─ SceneEntitySnapshot.cs
│  │  ├─ SceneRenderSnapshot.cs
│  │  └─ SceneTransformCommitResult.cs
│  ├─ Space/
│  │  ├─ CameraState.cs
│  │  ├─ DefaultEditorCamera.cs
│  │  ├─ ProjectionMode.cs
│  │  ├─ ViewProjectionState.cs
│  │  ├─ ViewProjectionState.Projection.cs
│  │  ├─ ViewportState.cs
│  │  ├─ WorldRay.cs
│  │  └─ WorldRayFactory.cs
│  ├─ Spatial/
│  │  ├─ RayAabbHit.cs
│  │  ├─ RayAabbIntersection.cs
│  │  ├─ SpatialAabb.cs
│  │  ├─ SpatialBounds.cs
│  │  ├─ SpatialQueryCategory.cs
│  │  ├─ SpatialQueryResult.cs
│  │  ├─ SpatialQueryStats.cs
│  │  ├─ SpatialRayAabb.cs
│  │  ├─ SpatialRayQuery.cs
│  │  ├─ SpatialRaycastHit.cs
│  │  ├─ SpatialRaycastResult.cs
│  │  └─ SpatialRaycastStats.cs
│  ├─ Time/
│  │  ├─ SimulationTime.cs
│  │  └─ TimeStep.cs
│  ├─ Transform/
│  │  ├─ PreviewTransform.cs
│  │  └─ TransformStartSnapshot.cs
│  └─ XuanYu.Core.csproj
├─ XuanYu.Core.Tests/
│  ├─ Camera/
│  │  ├─ CameraBasisTests.cs
│  │  ├─ CameraNavigationRollTests.cs
│  │  ├─ CameraNavigationSequenceTests.cs
│  │  ├─ CameraNavigationStressTests.cs
│  │  ├─ CameraNavigationTests.cs
│  │  ├─ CameraNavigationUiSequenceTests.Safety.cs
│  │  ├─ CameraNavigationUiSequenceTests.cs
│  │  └─ CameraOrthographicNavigationTests.cs
│  ├─ CoreSmokeTests.cs
│  ├─ EditorTool/
│  │  └─ EditorTransformCapturePolicyTests.cs
│  ├─ Gizmo/
│  │  ├─ MoveGizmoDragConstraintTests.cs
│  │  ├─ MoveGizmoLayoutG1Tests.cs
│  │  ├─ MoveGizmoLayoutPlaneTests.cs
│  │  ├─ MoveGizmoLayoutTests.cs
│  │  ├─ MoveGizmoLayoutVulkanTests.cs
│  │  ├─ MoveGizmoScreenSizeTests.cs
│  │  ├─ RotateGizmoLayoutTests.cs
│  │  ├─ ScaleGizmoTests.Drag.cs
│  │  ├─ ScaleGizmoTests.DragSafety.cs
│  │  ├─ ScaleGizmoTests.Helpers.cs
│  │  ├─ ScaleGizmoTests.R5R1.cs
│  │  └─ ScaleGizmoTests.cs
│  ├─ History/
│  │  ├─ EditorHistoryOwnerTests.cs
│  │  ├─ EditorHistoryRedoTests.cs
│  │  ├─ TransformHistoryIntegrationTests.cs
│  │  └─ TransformHistoryRedoIntegrationTests.cs
│  ├─ Picking/
│  │  └─ ViewportPickingServiceTests.cs
│  ├─ Render/
│  │  ├─ Camera/
│  │  │  └─ StandardViewResolverTests.cs
│  │  ├─ Diagnostics/
│  │  │  └─ RenderLogNoiseContractTests.cs
│  │  ├─ DrawPlan/
│  │  │  ├─ CubeRenderDrawPlanTests.cs
│  │  │  ├─ FrameExecutionPolicyTests.cs
│  │  │  ├─ RenderDrawPlanTests.cs
│  │  │  ├─ SceneRenderProjectionAdapterTests.Rotation.cs
│  │  │  ├─ SceneRenderProjectionAdapterTests.Selection.cs
│  │  │  ├─ SceneRenderProjectionAdapterTests.cs
│  │  │  ├─ ViewportAssistDrawPlanTests.cs
│  │  │  ├─ ViewportChromeContractTests.cs
│  │  │  └─ ViewportScaleIndicatorContractTests.cs
│  │  ├─ Grid/
│  │  │  ├─ ReferenceGridDrawPlanTests.cs
│  │  │  ├─ ReferenceGridFrameStateTests.cs
│  │  │  ├─ ScaleIndicatorMetricTests.cs
│  │  │  ├─ ReferenceGridShaderContractTests.cs
│  │  │  └─ ViewportMetricScaleTests.cs
│  │  ├─ Overlay/
│  │  │  ├─ ScaleIndicatorGlyphLiteTests.cs
│  │  │  └─ ViewportOverlayLayoutTests.cs
│  │  ├─ Map/
│  │  │  ├─ MapRenderDrawPlanTests.cs
│  │  │  ├─ MapSurfaceGeometryTests.cs
│  │  │  ├─ MapSurfaceLayerVisibilityTests.cs
│  │  │  ├─ MapSurfaceResourceKeyTests.cs
│  │  │  └─ MapSurfaceResourceUpdatePolicyTests.cs
│  │  ├─ NavigationGizmo/
│  │  │  ├─ NavigationGizmoLayoutTests.Facing.cs
│  │  │  ├─ NavigationGizmoLayoutTests.cs
│  │  │  ├─ NavigationGizmoOverlayContractTests.cs
│  │  │  ├─ NavigationGizmoDipContractTests.cs
│  │  │  └─ NavigationGizmoInputIsolationTests.cs
│  │  └─ StaticModels/
│  │     ├─ StaticModelDepthRegressionTests.cs
│  │     └─ StaticModelRenderContractTests.cs
│  ├─ Space/
│  │  ├─ CameraOrthographicTests.cs
│  │  ├─ CameraStateTests.cs
│  │  ├─ DefaultEditorCameraTests.cs
│  │  ├─ SpaceAssert.cs
│  │  ├─ ViewProjectionStateTests.cs
│  │  ├─ ViewportStateTests.cs
│  │  ├─ WorldRayFactoryTests.cs
│  │  └─ WorldRayTests.cs
│  ├─ Spatial/
│  │  ├─ RayAabbIntersectionTests.cs
│  │  ├─ SpatialBoundsTests.cs
│  │  └─ SpatialTestData.cs
│  └─ XuanYu.Core.Tests.csproj
├─ XuanYu.Editor/
│  ├─ Assets/
│  │  ├─ Catalog/
│  │  │  └─ SceneStaticModelCatalog.cs
│  │  ├─ Hosting/
│  │  │  ├─ HostedSceneAsset.cs
│  │  │  ├─ ModelAssetRuntimeState.cs
│  │  │  ├─ Planning/
│  │  │  │  ├─ SceneAssetHostingPlan.cs
│  │  │  │  └─ SceneAssetHostingPlanner.cs
│  │  │  ├─ SceneAssetHostingError.cs
│  │  │  ├─ SceneAssetHostingState.cs
│  │  │  ├─ SceneAssetPathPolicy.cs
│  │  │  └─ Transactions/
│  │  │     ├─ SceneAssetHostingTransaction.Activate.cs
│  │  │     ├─ SceneAssetHostingTransaction.Complete.cs
│  │  │     ├─ SceneAssetHostingTransaction.Rollback.cs
│  │  │     └─ SceneAssetHostingTransaction.cs
│  │  ├─ Identity/
│  │  │  └─ AssetId.cs
│  │  ├─ Import/
│  │  │  └─ Gltf/
│  │  │     ├─ GlbContainer.cs
│  │  │     ├─ GlbImportService.cs
│  │  │     ├─ GltfAccessorReader.cs
│  │  │     ├─ GltfCoordinatePolicy.cs
│  │  │     ├─ GltfJsonAccess.cs
│  │  │     ├─ GltfNodeTransform.cs
│  │  │     ├─ GltfStaticModelImporter.cs
│  │  │     └─ ImportStop.cs
│  │  └─ StaticModels/
│  │     ├─ SceneStaticModelBinding.cs
│  │     ├─ StaticModelAuthoringService.cs
│  │     ├─ StaticModelBuilder.cs
│  │     ├─ StaticModelColor.cs
│  │     ├─ StaticModelData.cs
│  │     ├─ StaticModelImportCodes.cs
│  │     ├─ StaticModelImportResult.cs
│  │     ├─ StaticModelImportWarning.cs
│  │     ├─ StaticModelPrimitive.cs
│  │     └─ StaticModelVertex.cs
│  ├─ Camera/
│  │  ├─ CameraBasis.cs
│  │  ├─ CameraFarProjectionDiagnostic.cs
│  │  ├─ CameraFrameResult.cs
│  │  ├─ CameraNavigation.Try.cs
│  │  ├─ CameraNavigation.cs
│  │  ├─ EditorCameraFraming.Orthographic.cs
│  │  ├─ EditorCameraFraming.Draft.cs
│  │  ├─ EditorCameraFraming.MapOrthographic.cs
│  │  ├─ EditorCameraFraming.cs
│  │  └─ OrthographicViewFactory.cs
│  ├─ MapDocument/
│  │  ├─ MapDocument.cs
│  │  ├─ MapDocumentAggregateBridge.cs
│  │  ├─ MapDocumentJson.cs
│  │  ├─ MapDocumentOwner.cs
│  │  ├─ MapDocumentResult.cs
│  │  ├─ MapDocumentValidator.cs
│  │  ├─ MapEnvironmentDefinition.cs
│  │  ├─ MapJsonMapper.cs
│  │  ├─ MapJsonSerializer.cs
│  │  └─ MapStorageService.cs
│  ├─ MapEditing/
│  │  ├─ MapEditEvents.cs
│  │  ├─ MapEditReason.cs
│  │  ├─ MapEditSession.Regions.cs
│  │  ├─ MapEditSession.ActiveLayer.cs
│  │  ├─ MapEditSession.Commands.cs
│  │  ├─ MapEditSession.Commit.cs
│  │  ├─ MapEditSession.Document.cs
│  │  ├─ MapEditSession.History.cs
│  │  ├─ MapEditSession.Layers.cs
│  │  ├─ MapEditSession.Selection.cs
│  │  ├─ MapEditSession.cs
│  │  ├─ MapHistoryEntry.cs
│  │  ├─ MapSelection.cs
│  │  └─ MapSelectionKind.cs
│  ├─ SceneDocument/
│  │  ├─ MapReference.cs
│  │  ├─ SceneDocumentAsset.cs
│  │  ├─ SceneDocumentEntity.cs
│  │  ├─ SceneDocumentJson.cs
│  │  ├─ SceneDocumentLoadTransaction.cs
│  │  ├─ SceneDocumentMapper.cs
│  │  ├─ SceneDocumentResult.cs
│  │  ├─ SceneDocumentSaveTransaction.cs
│  │  ├─ SceneDocumentSession.cs
│  │  ├─ SceneDocumentSnapshot.cs
│  │  ├─ SceneDocumentValidator.MapReference.cs
│  │  ├─ SceneDocumentValidator.cs
│  │  ├─ SceneDocumentWorldBridge.cs
│  │  ├─ SceneLoadCandidate.cs
│  │  ├─ SceneSaveOutcome.cs
│  │  └─ SceneStorageService.cs
│  ├─ Transform/
│  │  ├─ TransformSession.Rotate.cs
│  │  ├─ TransformSession.Scale.cs
│  │  └─ TransformSession.cs
│  └─ XuanYu.Editor.csproj
├─ XuanYu.Editor.App/
│  ├─ EditorCompositionRoot.cs
│  ├─ Program.cs
│  └─ XuanYu.Editor.App.csproj
├─ XuanYu.Editor.UI/
│  ├─ Accessibility/
│  │  ├─ UiAutomationNamer.cs
│  │  ├─ UiDpiContract.cs
│  │  └─ UiMotionPreference.cs
│  ├─ Bootstrap/
│  │  ├─ App.axaml
│  │  ├─ App.axaml.cs
│  │  └─ Program.cs
│  ├─ Design/
│  │  ├─ UiTokens.Colors.Components.axaml
│  │  ├─ UiTokens.Colors.Core.axaml
│  │  ├─ UiTokens.Controls.axaml
│  │  ├─ UiTokens.Fonts.axaml
│  │  ├─ UiTokens.Icons.axaml
│  │  ├─ UiTokens.Motion.axaml
│  │  ├─ UiTokens.Spacing.axaml
│  │  ├─ UiTokenManifest.json
│  │  └─ UiTokens.axaml
│  ├─ Dialogs/
│  │  ├─ IEditorDialogService.cs
│  │  └─ NullEditorDialogService.cs
│  ├─ EditorState/
│  │  ├─ EditorInteractionChangedResult.cs
│  │  ├─ EditorInteractionCommand.cs
│  │  ├─ EditorInteractionPointerSnapshot.cs
│  │  ├─ EditorInteractionSnapshot.cs
│  │  ├─ EditorSelectionCommand.cs
│  │  ├─ EditorSelectionSnapshot.cs
│  │  ├─ EditorStateChangedResult.cs
│  │  ├─ EditorStateOwner.Interaction.cs
│  │  ├─ EditorStateOwner.Tool.cs
│  │  ├─ EditorStateOwner.cs
│  │  ├─ EditorToolChangedResult.cs
│  │  ├─ EditorToolCommand.cs
│  │  ├─ EditorToolId.cs
│  │  ├─ EditorToolSnapshot.cs
│  │  ├─ EditorToolText.cs
│  │  └─ EditorTransformCapturePolicy.cs
│  ├─ Foot/
│  │  ├─ Foot.axaml
│  │  ├─ Foot.axaml.cs
│  │  ├─ LogAutoScrollPolicy.cs
│  │  ├─ LogDetailPanel.axaml
│  │  ├─ LogDetailPanel.axaml.cs
│  │  ├─ LogListAutoScrollController.Follow.cs
│  │  ├─ LogListAutoScrollController.Layout.cs
│  │  └─ LogListAutoScrollController.cs
│  ├─ Icons/
│  │  └─ EditorIcons.axaml
│  ├─ Left/
│  │  ├─ InlineRenameActivation.cs
│  │  ├─ Left.EntityCommands.cs
│  │  ├─ Left.Styles.axaml
│  │  ├─ Left.axaml
│  │  └─ Left.axaml.cs
│  ├─ Main/
│  │  ├─ Main.axaml
│  │  └─ Main.axaml.cs
│  ├─ NativeHostResizeCoalescer.cs
│  ├─ NativeHostResizeSnapshot.cs
│  ├─ NativeHostSurfaceContract.cs
│  ├─ RelayCommand.cs
│  ├─ Right/
│  │  ├─ LayerInspectorPanel.axaml
│  │  ├─ LayerInspectorPanel.axaml.cs
│  │  ├─ LayerPanel.DragDrop.cs
│  │  ├─ LayerPanel.axaml
│  │  ├─ LayerPanel.axaml.cs
│  │  ├─ MapEditorPanel.axaml
│  │  ├─ MapEditorPanel.axaml.cs
│  │  ├─ MapIdDisplayFormat.cs
│  │  ├─ MapPagePanel.axaml
│  │  ├─ MapPagePanel.axaml.cs
│  │  ├─ MapFormPanel.axaml
│  │  ├─ MapFormPanel.axaml.cs
│  │  ├─ EditableFormLayoutModel.cs
│  │  ├─ MapEditorLayoutModel.cs
│  │  ├─ InspectorPanel.axaml
│  │  ├─ InspectorPanel.axaml.cs
│  │  ├─ Right.axaml
│  │  ├─ Right.axaml.cs
│  │  ├─ TopTabStripController.AllTabs.cs
│  │  ├─ TopTabStripController.Hint.cs
│  │  ├─ TopTabStripController.Visible.cs
│  │  ├─ TopTabStripController.cs
│  │  ├─ TopTabStripModel.cs
│  │  └─ TopTabStripTemplate.axaml
│  ├─ Root/
│  │  ├─ UiRoot.axaml
│  │  └─ UiRoot.axaml.cs
│  ├─ Top/
│  │  ├─ Top.axaml
│  │  └─ Top.axaml.cs
│  ├─ TreeGuide.cs
│  ├─ TreeGuideSegment.cs
│  ├─ Ui.axaml
│  ├─ Viewport/
│  │  ├─ ViewNavigationGizmo.HitTest.cs
│  │  ├─ ViewNavigationGizmo.Layout.cs
│  │  └─ Vulkan/
│  │     ├─ NativePointerMessage.cs
│  │     ├─ VulkanNativeHost.AvaloniaCamera.cs
│  │     ├─ VulkanNativeHost.AvaloniaPointer.cs
│  │     ├─ VulkanNativeHost.Bridge.cs
│  │     ├─ VulkanNativeHost.CameraPointer.cs
│  │     ├─ VulkanNativeHost.Dpi.cs
│  │     ├─ VulkanNativeHost.Gizmo.cs
│  │     ├─ VulkanNativeHost.LayoutSync.cs
│  │     ├─ VulkanNativeHost.Log.cs
│  │     ├─ VulkanNativeHost.NavGizmo.cs
│  │     ├─ VulkanNativeHost.Picking.cs
│  │     ├─ VulkanNativeHost.Pointer.cs
│  │     ├─ NativePointerRoutePolicy.cs
│  │     ├─ VulkanNativeHost.cs
│  │     ├─ VulkanViewport.axaml
│  │     ├─ VulkanViewport.axaml.cs
│  │     ├─ Win32ViewportHost.Input.cs
│  │     └─ Win32ViewportHost.cs
│  ├─ ViewportNativeHostRoute.cs
│  ├─ Vm/
│  │  ├─ Camera/
│  │  │  ├─ CameraSessionMode.cs
│  │  │  ├─ CameraSessionSnapshot.cs
│  │  │  ├─ StandardViewResolver.cs
│  │  │  ├─ UiVm.Camera.Framing.cs
│  │  │  ├─ UiVm.Camera.Framing.Draft.cs
│  │  │  ├─ UiVm.Camera.cs
│  │  │  ├─ UiVm.CameraDolly.cs
│  │  │  ├─ UiVm.FarProjectionDiagnostic.cs
│  │  │  ├─ UiVm.CameraNavigation.cs
│  │  │  ├─ UiVm.ScaleIndicator.cs
│  │  │  └─ UiVm.ViewGizmo.cs
│  │  ├─ History/
│  │  │  ├─ UiVm.EntityCommands.cs
│  │  │  ├─ UiVm.History.Entities.cs
│  │  │  └─ UiVm.History.cs
│  │  ├─ Inspector/
│  │  │  ├─ UiVm.Inspector.cs
│  │  │  ├─ UiVm.InspectorInput.Parse.cs
│  │  │  └─ UiVm.InspectorInput.cs
│  │  ├─ Logging/
│  │  │  ├─ DebugText.cs
│  │  │  ├─ EditorDisplayText.cs
│  │  │  ├─ EditorLogBuffer.cs
│  │  │  ├─ EditorLogBus.cs
│  │  │  ├─ EditorLogCategory.cs
│  │  │  ├─ EditorLogClipboardText.cs
│  │  │  ├─ EditorLogFilter.cs
│  │  │  ├─ EditorLogFilterQuery.cs
│  │  │  ├─ EditorLogLevel.cs
│  │  │  ├─ EditorLogNoiseFilter.cs
│  │  │  ├─ EditorLogRepeatKey.cs
│  │  │  ├─ EditorLogSource.cs
│  │  │  ├─ EditorLogSummary.cs
│  │  │  ├─ LogEntry.cs
│  │  │  ├─ SampleLogEntries.cs
│  │  │  ├─ UiText.cs
│  │  │  └─ UiVm.Logging.cs
│  │  ├─ Map/
│  │  │  ├─ MapLayerRowViewModel.cs
│  │  │  ├─ MapRegionRenderProjection.cs
│  │  │  ├─ MapRenderSnapshotProjection.cs
│  │  │  ├─ MapVectorOverlayBuilder.Finalize.cs
│  │  │  ├─ MapVectorOverlayBuilder.cs
│  │  │  ├─ MapVectorOverlayTriangulation.cs
│  │  │  ├─ UiVm.RegionDrawing.Commit.cs
│  │  │  ├─ UiVm.RegionDrawing.Input.cs
│  │  │  ├─ UiVm.RegionDrawing.Logging.cs
│  │  │  ├─ UiVm.RegionDrawing.cs
│  │  │  ├─ UiVm.MapCommandRouting.cs
│  │  │  ├─ UiVm.MapDiagnostics.Format.cs
│  │  │  ├─ UiVm.MapDiagnostics.cs
│  │  │  ├─ UiVm.MapEditor.cs
│  │  │  ├─ UiVm.MapEditor.Display.cs
│  │  ├─ UiVm.MapEditor.Validation.cs
│  │  ├─ UiVm.MapEditor.Validation.Rules.cs
│  │  ├─ UiVm.MapDanger.cs
│  │  │  ├─ UiVm.MapHistory.cs
│  │  │  ├─ UiVm.MapLayerDiagnostics.cs
│  │  │  ├─ UiVm.MapLayerDrag.cs
│  │  │  ├─ UiVm.MapLayerInspector.cs
│  │  │  ├─ UiVm.MapLayerSelection.cs
│  │  │  ├─ UiVm.MapLayers.cs
│  │  │  ├─ UiVm.MapRender.cs
│  │  │  └─ UiVm.MapWorld.cs
│  │  ├─ UiVm.RightPanel.cs
│  │  ├─ Scene/
│  │  │  ├─ D2StaticModelDemo.cs
│  │  │  ├─ SceneHistoryEntry.cs
│  │  │  ├─ SceneRenderProjectionAdapter.cs
│  │  │  ├─ StaticModelRenderAdapter.cs
│  │  │  ├─ UiVm.DocumentStatus.cs
│  │  │  ├─ UiVm.RenderProjection.cs
│  │  │  ├─ UiVm.Scene.cs
│  │  │  ├─ UiVm.SceneDocument.New.cs
│  │  │  ├─ UiVm.SceneDocument.cs
│  │  │  ├─ UiVm.SceneDocumentLog.cs
│  │  │  ├─ UiVm.SceneDocumentMapRef.cs
│  │  │  ├─ UiVm.SceneDocumentSave.cs
│  │  │  ├─ UiVm.StaticModelImport.cs
│  │  │  └─ UiVm.WorldProjection.cs
│  │  ├─ Selection/
│  │  │  ├─ UiVm.Picking.cs
│  │  │  ├─ UiVm.Selection.cs
│  │  │  ├─ UiVm.SelectionProjection.cs
│  │  │  ├─ UiVm.SelectionTrace.cs
│  │  │  ├─ UiVm.SelectionValidity.cs
│  │  │  ├─ UiVm.ViewportSelection.cs
│  │  │  └─ ViewportPickingLogFormatter.cs
│  │  ├─ Transform/
│  │  │  ├─ Move/
│  │  │  │  ├─ UiVm.MoveGizmo.cs
│  │  │  │  ├─ UiVm.MoveGizmoLogging.cs
│  │  │  │  └─ UiVm.MoveGizmoScreenSize.cs
│  │  │  ├─ Rotate/
│  │  │  │  └─ UiVm.RotateGizmo.cs
│  │  │  ├─ Scale/
│  │  │  │  └─ UiVm.ScaleGizmo.cs
│  │  │  ├─ UiVm.InputGuards.cs
│  │  │  ├─ UiVm.Interaction.cs
│  │  │  ├─ UiVm.InteractionCancel.cs
│  │  │  ├─ UiVm.InteractionPointer.cs
│  │  │  ├─ UiVm.Tool.cs
│  │  │  └─ UiVm.ViewportAssist.cs
│  │  ├─ Tree/
│  │  │  ├─ EditorTreeNode.cs
│  │  │  ├─ TreeGuideBuilder.cs
│  │  │  └─ UiVm.TreeCommands.cs
│  │  ├─ UiVm.NativeHostLifecycle.cs
│  │  ├─ UiVm.NotificationLifetime.cs
│  │  └─ UiVm.cs
│  ├─ Win/
│  │  ├─ UiWin.Accessibility.cs
│  │  ├─ UiWin.Dialogs.cs
│  │  ├─ UiWin.EntityShortcuts.cs
│  │  ├─ UiWin.MapCommands.cs
│  │  ├─ UiWin.SceneCommands.cs
│  │  ├─ UiWin.UnsavedDialog.cs
│  │  ├─ UiWin.axaml
│  │  ├─ UiWin.axaml.cs
│  │  └─ UiWin.Shortcuts.cs
│  ├─ XuanYu.Editor.UI.csproj
│  └─ app.manifest
├─ XuanYu.Editor.Win/
│  ├─ MainForm.cs
│  └─ XuanYu.Editor.Win.csproj
├─ XuanYu.Engine.slnx
├─ XuanYu.Render.Abstractions/
│  ├─ EditorViewPlaneGridKind.cs
│  ├─ EditorViewportAssistState.cs
│  ├─ FrameExecutionPolicy.cs
│  ├─ INativeHostSurfaceBridge.cs
│  ├─ INativeHostSurfaceBridgeFactory.cs
│  ├─ IRenderProjectionSource.cs
│  ├─ MapBoundsGeometry.cs
│  ├─ MapRenderSnapshot.cs
│  ├─ MapSurfaceGeometry.cs
│  ├─ MapSurfaceResourceKey.cs
│  ├─ MapSurfaceResourceUpdatePolicy.cs
│  ├─ LatestRenderProjectionQueue.cs
│  ├─ MapSurfaceResourceUpdateText.cs
│  ├─ NativeHostHandleSnapshot.cs
│  ├─ NativeHostLifecycleLogFormatter.cs
│  ├─ NativeHostLifecycleProbe.cs
│  ├─ NativeHostLifecycleState.cs
│  ├─ NativeHostSurfaceHandle.cs
│  ├─ ReferenceGridFrameState.cs
│  ├─ ReferenceGridScale.cs
│  ├─ ScaleIndicatorGlyphLite.cs
│  ├─ ScaleIndicatorMetric.cs
│  ├─ ScaleIndicatorOverlayProjection.cs
│  ├─ RenderCameraProjection.cs
│  ├─ ViewportMetricScale.cs
│  ├─ RenderDrawPlan.Typed.cs
│  ├─ RenderDrawPlan.cs
│  ├─ RenderEntityProjection.cs
│  ├─ RenderEntityType.cs
│  ├─ RenderProjection.cs
│  ├─ RenderProjectionResult.cs
│  ├─ RenderStaticModelKey.cs
│  ├─ RenderStaticModelPrimitive.cs
│  ├─ RenderStaticModelResource.cs
│  ├─ RenderStaticModelVertex.cs
│  ├─ RenderVectorOverlayKey.cs
│  ├─ RenderVectorOverlayPrimitive.cs
│  ├─ RenderVectorOverlayResource.cs
│  ├─ RenderVectorOverlayVertex.cs
│  ├─ ViewportOverlayAnchor.cs
│  ├─ ViewportOverlayLayoutResolver.cs
│  └─ XuanYu.Render.Abstractions.csproj
├─ XuanYu.Render.Vulkan/
│  ├─ Bridge/
│  │  ├─ VulkanBridgeDeviceAttachStep.cs
│  │  ├─ VulkanBridgePhysicalDeviceAttachStep.cs
│  │  ├─ VulkanBridgeRenderSessionAttachStep.cs
│  │  └─ VulkanBridgeSwapchainAttachStep.cs
│  ├─ Device/
│  │  ├─ VulkanDeviceOwner.Physical.cs
│  │  ├─ VulkanDeviceOwner.cs
│  │  ├─ VulkanPhysicalDeviceInfo.cs
│  │  ├─ VulkanPhysicalDeviceSelection.cs
│  │  ├─ VulkanPhysicalDeviceSelector.cs
│  │  └─ VulkanQueueFamilySelection.cs
│  ├─ Diagnostic/
│  │  └─ VulkanResizeTracer.cs
│  ├─ Pipeline/
│  │  ├─ ShaderBytecode.Frag.cs
│  │  ├─ ShaderBytecode.GridLineFrag.cs
│  │  ├─ ShaderBytecode.GridLineVert.cs
│  │  ├─ ShaderBytecode.GridVert.cs
│  │  ├─ ShaderBytecode.WorldReferenceGridFrag.cs
│  │  ├─ ShaderBytecode.NavGizmoFrag.cs
│  │  ├─ ShaderBytecode.NavGizmoVert.cs
│  │  ├─ ShaderBytecode.ScaleIndicatorFrag.cs
│  │  ├─ ShaderBytecode.Vert.cs
│  │  ├─ ShaderBytecode.ViewPlaneGridFrag.cs
│  │  ├─ ShaderBytecode.WorldAxesFrag.cs
│  │  ├─ ShaderBytecode.WorldOriginFrag.cs
│  │  ├─ VulkanGraphicsPipelineOwner.Depth.cs
│  │  ├─ VulkanGraphicsPipelineOwner.Fullscreen.cs
│  │  ├─ VulkanGraphicsPipelineOwner.Grid.cs
│  │  ├─ VulkanGraphicsPipelineOwner.GridLine.cs
│  │  ├─ VulkanGraphicsPipelineOwner.Sky.cs
│  │  ├─ VulkanGraphicsPipelineOwner.StaticModelInput.cs
│  │  ├─ VulkanGraphicsPipelineOwner.cs
│  │  ├─ VulkanPipelineLogFormatter.cs
│  │  ├─ VulkanScenePushConstants.cs
│  │  └─ VulkanShaderModuleOwner.cs
│  ├─ Render/
│  │  ├─ ClearFrame/
│  │  │  ├─ VulkanClearFrameLogFormatter.cs
│  │  │  ├─ VulkanClearFrameOwner.Commands.cs
│  │  │  ├─ VulkanClearFrameOwner.Lifecycle.cs
│  │  │  ├─ VulkanClearFrameOwner.Matrix.cs
│  │  │  ├─ VulkanClearFrameOwner.PipelineBind.cs
│  │  │  ├─ VulkanClearFrameOwner.PushConstants.cs
│  │  │  ├─ VulkanClearFrameOwner.Resources.cs
│  │  │  ├─ VulkanClearFrameOwner.Trace.cs
│  │  │  ├─ VulkanClearFrameOwner.VectorOverlayPipeline.cs
│  │  │  └─ VulkanClearFrameOwner.cs
│  │  ├─ Grid/
│  │  │  ├─ VulkanClearFrameOwner.Grid.cs
│  │  │  ├─ VulkanClearFrameOwner.GridScale.cs
│  │  │  ├─ VulkanClearFrameOwner.NavGizmo.cs
│  │  │  ├─ VulkanClearFrameOwner.ScaleIndicator.cs
│  │  │  ├─ VulkanClearFrameOwner.ViewPlaneGrid.cs
│  │  │  └─ VulkanClearFrameOwner.WorldAxes.cs
│  │  ├─ Map/
│  │  │  └─ VulkanClearFrameOwner.MapSurface.cs
│  │  ├─ Present/
│  │  │  ├─ VulkanPresentLoop.Frame.cs
│  │  │  ├─ VulkanPresentLoop.Lifecycle.cs
│  │  │  └─ VulkanPresentLoop.cs
│  │  ├─ Scene/
│  │  │  ├─ VulkanClearFrameOwner.Draw.cs
│  │  │  ├─ VulkanClearFrameOwner.DrawAssist.cs
│  │  │  ├─ VulkanClearFrameOwner.DrawGizmo.cs
│  │  │  └─ VulkanClearFrameOwner.Scene.cs
│  │  ├─ StaticModels/
│  │  │  ├─ VulkanClearFrameOwner.DrawStaticBounds.cs
│  │  │  ├─ VulkanClearFrameOwner.DrawStaticModel.cs
│  │  │  ├─ VulkanStaticModelBuffer.cs
│  │  │  ├─ VulkanStaticModelCache.cs
│  │  │  ├─ VulkanStaticModelFailureTracker.cs
│  │  │  ├─ VulkanStaticModelLog.cs
│  │  │  ├─ VulkanStaticModelResource.cs
│  │  │  ├─ VulkanStaticModelValidator.cs
│  │  │  └─ VulkanStaticModelVertex.cs
│  │  ├─ VectorOverlay/
│  │  │  ├─ VulkanClearFrameOwner.DrawVectorOverlay.cs
│  │  │  ├─ VulkanVectorOverlayBufferReusePolicy.cs
│  │  │  ├─ VulkanVectorOverlayCache.cs
│  │  │  ├─ VulkanVectorOverlayResource.cs
│  │  │  ├─ VulkanVectorOverlayValidator.cs
│  │  │  └─ VulkanVectorOverlayVertex.cs
│  │  └─ VulkanDepthAttachment.cs
│  ├─ Session/
│  │  ├─ GridPipelineSet.cs
│  │  ├─ VulkanRenderSession.Lifecycle.cs
│  │  ├─ VulkanRenderSession.Recover.cs
│  │  ├─ VulkanRenderSession.Resize.cs
│  │  ├─ VulkanRenderSession.VectorOverlay.cs
│  │  └─ VulkanRenderSession.cs
│  ├─ Shaders/
│  │  ├─ editor_nav_gizmo.frag
│  │  ├─ editor_nav_gizmo.vert
│  │  ├─ editor_scale_indicator.frag
│  │  ├─ editor_reference_grid_line.frag
│  │  ├─ editor_reference_grid_line.vert
│  │  ├─ editor_reference_grid.vert
│  │  ├─ editor_world_reference_grid.frag
│  │  ├─ editor_view_plane_grid.frag
│  │  ├─ editor_world_axes.frag
│  │  ├─ editor_world_origin.frag
│  │  ├─ scene.frag
│  │  └─ scene.vert
│  ├─ Swapchain/
│  │  ├─ VulkanSwapchainBuilder.cs
│  │  ├─ VulkanSwapchainCapabilities.cs
│  │  ├─ VulkanSwapchainLogFormatter.cs
│  │  ├─ VulkanSwapchainOwner.Accessors.cs
│  │  └─ VulkanSwapchainOwner.cs
│  ├─ VulkanApiProbe.cs
│  ├─ VulkanBridgeLogFormatter.cs
│  ├─ VulkanDeviceInfo.cs
│  ├─ VulkanInstanceCreateInfoBuilder.cs
│  ├─ VulkanInstanceExtensions.cs
│  ├─ VulkanInstanceLogFormatter.cs
│  ├─ VulkanInstanceOwner.cs
│  ├─ VulkanInstanceResult.cs
│  ├─ VulkanNativeHostSurfaceBridge.Attach.cs
│  ├─ VulkanNativeHostSurfaceBridge.Lifecycle.cs
│  ├─ VulkanNativeHostSurfaceBridge.Resize.cs
│  ├─ VulkanNativeHostSurfaceBridge.Scene.cs
│  ├─ VulkanNativeHostSurfaceBridge.cs
│  ├─ VulkanNativeHostSurfaceBridgeFactory.cs
│  ├─ VulkanProbeLogFormatter.cs
│  ├─ VulkanProbeResult.cs
│  ├─ VulkanSurfaceLogFormatter.cs
│  ├─ VulkanSurfaceOwner.cs
│  ├─ VulkanSurfaceResult.cs
│  └─ XuanYu.Render.Vulkan.csproj
├─ XuanYu.WarCore/
│  ├─ Identity/
│  │  ├─ FactionId.cs
│  │  ├─ MilitaryIdentity.cs
│  │  ├─ OrganizationId.cs
│  │  ├─ UnitId.cs
│  │  └─ UnitKind.cs
│  ├─ State/
│  │  └─ SoldierState.cs
│  └─ XuanYu.WarCore.csproj
├─ XuanYu.WarCore.Tests/
│  ├─ Identity/
│  │  └─ MilitaryIdentityTests.cs
│  ├─ State/
│  │  └─ SoldierStateTests.cs
│  ├─ WarCoreDependencyTests.cs
│  └─ XuanYu.WarCore.Tests.csproj
├─ XuanYu.World/
│  ├─ EntityRegistry.Authoring.cs
│  ├─ EntityRegistry.Replace.cs
│  ├─ EntityRegistry.cs
│  ├─ GlobalWorld.Authoring.cs
│  ├─ GlobalWorld.Query.cs
│  ├─ GlobalWorld.Snapshot.cs
│  ├─ GlobalWorld.cs
│  ├─ GridWorldPartitionStrategy.cs
│  ├─ IWorldPartitionStrategy.cs
│  ├─ Map/
│  │  ├─ MapBounds.cs
│  │  ├─ MapDefaultDefinition.cs
│  │  ├─ MapDefinition.cs
│  │  ├─ MapDefinitionValidator.cs
│  │  ├─ MapGeometry.cs
│  │  ├─ MapId.cs
│  │  ├─ MapLayer.cs
│  │  ├─ MapLayerId.cs
│  │  ├─ MapLayerKind.cs
│  │  ├─ MapLayerRules.cs
│  │  ├─ MapLayerStack.cs
│  │  ├─ MapLayerValidator.cs
│  │  ├─ MapRegion.cs
│  │  ├─ MapRegionIntersection.cs
│  │  ├─ MapRegionDraft.cs
│  │  ├─ MapRegionId.cs
│  │  ├─ MapRegionKind.cs
│  │  ├─ MapRegionValidator.cs
│  │  ├─ MapSurfaceDefinition.cs
│  │  ├─ MapValidationResult.cs
│  │  ├─ WorldMapState.cs
│  │  └─ WorldMapStateOwner.cs
│  ├─ RegionKey.cs
│  ├─ Scene/
│  │  ├─ SceneSpatialBoundsProjection.cs
│  │  ├─ SceneStateOwner.Lifecycle.cs
│  │  ├─ SceneStateOwner.Seeding.cs
│  │  ├─ SceneStateOwner.StaticModel.cs
│  │  ├─ SceneStateOwner.Transform.cs
│  │  ├─ SceneStateOwner.cs
│  │  └─ SceneWorldProjection.cs
│  ├─ Spatial/
│  │  ├─ DynamicAabbTree.Insert.cs
│  │  ├─ DynamicAabbTree.Node.cs
│  │  ├─ DynamicAabbTree.Query.cs
│  │  ├─ DynamicAabbTree.Refit.cs
│  │  ├─ DynamicAabbTree.Remove.cs
│  │  ├─ DynamicAabbTree.cs
│  │  ├─ ISpatialIndex.cs
│  │  ├─ SpatialIndexOwner.cs
│  │  └─ SpatialRaycastResolver.cs
│  ├─ WorldEntityActivity.cs
│  ├─ WorldEntityName.cs
│  ├─ WorldEntitySnapshot.cs
│  ├─ WorldEntityType.cs
│  ├─ WorldPartitionEntry.cs
│  ├─ WorldPartitionMembership.cs
│  ├─ WorldQuery.cs
│  └─ XuanYu.World.csproj
├─ XuanYu.World.Tests/
│  ├─ Assets/
│  │  ├─ AssetContractTests.cs
│  │  ├─ AssetDialogTests.cs
│  │  ├─ GlbFactory.cs
│  │  ├─ GlbImportTests.cs
│  │  ├─ GlbMultiPrimitiveFactory.cs
│  │  ├─ HostingCompleteTests.cs
│  │  ├─ HostingPlannerRejectTests.cs
│  │  ├─ HostingPlannerTests.cs
│  │  ├─ HostingRollbackTests.cs
│  │  ├─ HostingSaveAsTests.cs
│  │  ├─ HostingTestEnv.cs
│  │  ├─ HostingTransactionTests.cs
│  │  ├─ LoadStructureErrorTests.cs
│  │  ├─ LoadTransactionTests.cs
│  │  ├─ SaveAsTests.cs
│  │  ├─ SaveTransactionTests.cs
│  │  ├─ ScenePersistenceEnv.cs
│  │  ├─ SchemaCompatibilityTests.cs
│  │  ├─ StaticModelAuthoringServiceTests.cs
│  │  ├─ StaticModelBaseVertexTests.cs
│  │  ├─ StaticModelCatalogTests.cs
│  │  ├─ StaticModelFailureTrackerTests.cs
│  │  ├─ StaticModelProjectionTests.cs
│  │  ├─ StaticModelUiTests.cs
│  │  └─ StaticModelValidatorTests.cs
│  ├─ Camera/
│  │  ├─ CameraDocumentTests.cs
│  │  ├─ CameraFramingOccupancyTests.cs
│  │  ├─ CameraFramingTests.cs
│  │  ├─ CameraC2DraftFramingTests.cs
│  │  ├─ CameraC2MapFramingTests.Helpers.cs
│  │  ├─ CameraC2MapFramingTests.cs
│  │  ├─ CameraNavigationUiTests.cs
│  │  ├─ CameraNavigationUiTests.Focus.cs
│  │  └─ UiViewGizmoTests.cs
│  ├─ Logging/
│  │  ├─ FootAxamlTailContractTests.cs
│  │  ├─ LogAutoScrollPolicyTests.cs
│  │  ├─ LogListAutoScrollControllerContractTests.cs
│  │  ├─ UiMapLogChineseTests.cs
│  │  └─ UiRootLogRowContractTests.cs
│  ├─ Map/
│  │  ├─ Editing/
│  │  │  ├─ MapLayerSessionTests.Behavior.cs
│  │  │  ├─ MapLayerSessionTests.Drag.History.cs
│  │  │  ├─ MapLayerSessionTests.Drag.cs
│  │  │  ├─ MapLayerSessionTests.cs
│  │  │  ├─ UiLayerStateFeedbackTests.cs
│  │  │  ├─ UiLayerVisualContractTests.cs
│  │  │  ├─ UiLogSummaryPriorityTests.cs
│  │  │  ├─ UiLogSummaryTimingTests.cs
│  │  │  ├─ UiMapCommandRoutingTests.cs
│  │  │  ├─ UiMapEditorTests.cs
│  │  │  ├─ UiMapHistoryTests.cs
│  │  │  ├─ UiMapInitialProjectionTests.cs
│  │  │  ├─ UiMapLayerDragTests.cs
│  │  │  ├─ UiMapLayerLockLogTests.cs
│  │  │  ├─ UiMapLayerPanelTests.Behavior.cs
│  │  │  ├─ UiMapLayerPanelTests.cs
│  │  │  └─ UiMapLayoutContractTests.cs
│  │  ├─ MapBoundsTests.cs
│  │  ├─ MapCoordinateValidationTests.cs
│  │  ├─ MapDefaultMapTests.cs
│  │  ├─ MapDefinitionTests.cs
│  │  ├─ MapDocumentAggregateBridgeTests.cs
│  │  ├─ MapDocumentOwnerChainTests.cs
│  │  ├─ MapDocumentOwnerTests.cs
│  │  ├─ MapEnvironmentValidationTests.cs
│  │  ├─ MapIdTests.cs
│  │  ├─ MapJsonRoundTripTests.cs
│  │  ├─ MapJsonStrictnessTests.cs
│  │  ├─ MapLayerRulesTests.cs
│  │  ├─ MapLayerStackTests.Drag.cs
│  │  ├─ MapLayerStackTests.Order.cs
│  │  ├─ MapLayerStackTests.cs
│  │  ├─ MapLayerTests.Base.cs
│  │  ├─ MapLayerTests.cs
│  │  ├─ MapRegionDraftTests.cs
│  │  ├─ MapRegionTests.Geometry.cs
│  │  ├─ MapRegionTests.Helpers.cs
│  │  ├─ MapRegionTests.Strictness.cs
│  │  ├─ MapRegionTests.cs
│  │  ├─ MapSizeValidationTests.cs
│  │  ├─ MapStorageFailureTests.cs
│  │  ├─ MapStorageTests.cs
│  │  ├─ MapSurfaceSamplerTests.cs
│  │  ├─ MapSurfaceValidationTests.cs
│  │  ├─ SceneMapReferenceTests.cs
│  │  ├─ WorldMapStateOwnerTests.cs
│  │  └─ WorldMapStateTests.cs
│  ├─ MapEditing/
│  │  ├─ MapEditSessionCommandTests.cs
│  │  ├─ MapPickingRoundTripTests.cs
│  │  ├─ MapEditSessionRegionTests.cs
│  │  ├─ MapEditSessionCreationTests.cs
│  │  ├─ MapEditSessionDirtyTests.cs
│  │  ├─ MapEditSessionHistoryTests.cs
│  │  ├─ MapEditSessionMapPropertiesTests.cs
│  │  ├─ MapEditSessionSelectionTests.cs
│  │  ├─ MapEditSessionThreadTests.cs
│  │  ├─ MapEditSessionValidationTests.cs
│  │  └─ MapRenderSnapshotProjectionTests.cs
│  ├─ Render/
│  │  ├─ WorldGridIndependenceContractTests.cs
│  │  ├─ VulkanPresentLoopContractTests.cs
│  │  └─ VulkanPresentModeSelectionTests.cs
│  ├─ Scene/
│  │  ├─ CommandSmokeTests.cs
│  │  ├─ EditorEnvironmentTests.cs
│  │  ├─ EntityBoundsSemanticsTests.cs
│  │  ├─ EntityRegistryTests.cs
│  │  ├─ EntityTests.cs
│  │  ├─ FinalSceneTests.cs
│  │  ├─ GlobalWorldTests.cs
│  │  ├─ SceneConsumptionTests.cs
│  │  ├─ SceneDocumentPersistenceTests.cs
│  │  ├─ SceneDocumentTests.Opening.cs
│  │  ├─ SceneDocumentTests.SaveFeedback.cs
│  │  ├─ SceneDocumentTests.cs
│  │  ├─ SceneIsolationTests.cs
│  │  ├─ SceneMultiEntityGateTests.cs
│  │  ├─ SceneSelectionReentryTests.cs
│  │  ├─ SceneSingleAuthorityTests.cs
│  │  ├─ UiHistoryTests.InlineRename.cs
│  │  └─ UiHistoryTests.cs
│  ├─ Selection/
│  │  ├─ FinalSelectionTests.cs
│  │  ├─ SelectionToolStateUiTests.cs
│  │  ├─ ToolStateHighlightUiTests.Selection.cs
│  │  └─ ToolStateHighlightUiTests.cs
│  ├─ Spatial/
│  │  ├─ SceneStateOwnerSpatialTests.cs
│  │  ├─ SpatialIndexEditLifecycleTests.cs
│  │  ├─ SpatialIndexOwnerLifecycleTests.cs
│  │  ├─ SpatialIndexOwnerRevisionTests.cs
│  │  ├─ SpatialIndexRebuildTests.cs
│  │  ├─ SpatialIndexScaleTests.cs
│  │  ├─ SpatialQueryGovernanceTests.cs
│  │  ├─ SpatialQueryOracle.cs
│  │  ├─ SpatialQueryTests.Geometry.cs
│  │  ├─ SpatialQueryTests.cs
│  │  ├─ SpatialRayQueryLifecycleTests.cs
│  │  ├─ SpatialRayQueryTests.cs
│  │  ├─ SpatialRaycastNearestTests.cs
│  │  ├─ SpatialRaycastRevisionTests.cs
│  │  ├─ SpatialRaycastScaleTests.cs
│  │  └─ SpatialTestData.cs
│  ├─ Transform/
│  │  ├─ Move/
│  │  │  ├─ MoveTransformUiTests.Plane.cs
│  │  │  ├─ MoveTransformUiTests.Region.cs
│  │  │  ├─ MoveTransformUiTests.Session.cs
│  │  │  └─ MoveTransformUiTests.cs
│  │  ├─ Rotate/
│  │  │  ├─ RotateTransformUiTests.DragState.cs
│  │  │  ├─ RotateTransformUiTests.Helpers.cs
│  │  │  ├─ RotateTransformUiTests.Preview.cs
│  │  │  ├─ RotateTransformUiTests.ToolSwitch.cs
│  │  │  └─ RotateTransformUiTests.cs
│  │  ├─ Scale/
│  │  │  ├─ ScaleGizmoGlobalModeTests.cs
│  │  │  ├─ ScaleTransformUiTests.AxisUniform.cs
│  │  │  ├─ ScaleTransformUiTests.Helpers.cs
│  │  │  ├─ ScaleTransformUiTests.History.cs
│  │  │  ├─ ScaleTransformUiTests.Pointer.cs
│  │  │  ├─ ScaleTransformUiTests.Target.cs
│  │  │  └─ ScaleTransformUiTests.cs
│  │  ├─ TransformFoundationTests.Input.cs
│  │  ├─ TransformFoundationTests.Inspector.cs
│  │  ├─ TransformFoundationTests.cs
│  │  ├─ TransformSessionTests.cs
│  │  └─ ViewportAssistTests.cs
│  ├─ Tree/
│  │  ├─ UiHierarchyConnectorTests.cs
│  │  ├─ UiTreeGuideTests.cs
│  │  └─ UiTreeToggleTests.cs
│  ├─ UiTokens/
│  │  ├─ UiCsColorRulesTests.cs
│  │  ├─ UiDebtBaseline.Colors.Axaml1.cs
│  │  ├─ UiDebtBaseline.Colors.Axaml2.cs
│  │  ├─ UiDebtBaseline.Colors.Cs.cs
│  │  ├─ UiDebtBaseline.Typography.cs
│  │  ├─ UiDebtBaseline.cs
│  │  ├─ UiDebtBaselineBypassF2Tests.cs
│  │  ├─ UiDebtBaselineBypassTests.cs
│  │  ├─ UiDebtBaselineTests.cs
│  │  ├─ UiD6AccessibilityContractTests.cs
│  │  ├─ UiD6DpiContractTests.cs
│  │  ├─ UiD6LogPerformanceTests.cs
│  │  ├─ UiD6MotionContractTests.cs
│  │  ├─ UiSourceContractAnalyzer.CsRules.cs
│  │  ├─ UiSourceContractAnalyzer.Icon.cs
│  │  ├─ UiSourceContractAnalyzer.Inline.cs
│  │  ├─ UiSourceContractAnalyzer.Structure.cs
│  │  ├─ UiSourceContractAnalyzer.cs
│  │  ├─ UiSourceContractAnalyzerTests.cs
│  │  ├─ UiSourceContractAnalyzerTokenRefTests.cs
│  │  ├─ UiD3DebtClearedTests.cs
│  │  ├─ UiD4DebtClearedTests.cs
│  │  ├─ UiD4F1LayoutModelTests.cs
│  │  ├─ UiD4F1TextOverflowContractTests.cs
│  │  ├─ UiD4F1TypographyContractTests.cs
│  │  ├─ UiD4InspectorContractTests.cs
│  │  ├─ UiD4LayerContractTests.cs
│  │  ├─ UiF3LayerRowContractTests.cs
│  │  ├─ UiD4LayoutModelTests.cs
│  │  ├─ UiD4MapEditorContractTests.cs
│  │  ├─ UiD5ButtonContractTests.cs
│  │  ├─ UiD5DangerFlowTests.cs
│  │  ├─ UiD5DialogAndLogContractTests.cs
│  │  ├─ UiD5FormContractTests.cs
│  │  ├─ UiD5NotificationTests.cs
│  │  ├─ UiD5CorrectionBehaviorTests.cs
│  │  ├─ UiD5CorrectionNotifyTests.cs
│  │  ├─ UiD5CorrectionStructureTests.cs
│  │  ├─ UiD5InputValidationTests.cs
│  │  ├─ UiD5UnsavedFlowTests.cs
│  │  ├─ UiD5MapStatusTests.cs
│  │  ├─ UiD5UnsavedDialogTests.cs
│  │  ├─ UiD5UnsavedDialogBehaviorTests.cs
│  │  ├─ UiTokenManifestGraphTests.cs
│  │  ├─ UiTokenManifestTests.cs
│  │  ├─ UiTopTabStripContractTests.cs
│  │  ├─ UiTopTabStripModelHintAndListTests.cs
│  │  └─ UiTopTabStripModelTests.cs
│  ├─ WorldPartition/
│  │  ├─ WorldPartitionInvariantTests.cs
│  │  ├─ WorldPartitionMigrationTests.Activity.cs
│  │  ├─ WorldPartitionMigrationTests.cs
│  │  ├─ WorldPartitionTests.PartitionStrategy.cs
│  │  ├─ WorldPartitionTests.cs
│  │  └─ WorldPartitionUiTests.cs
│  └─ XuanYu.World.Tests.csproj
├─ changelog.md
├─ docs/
│  ├─ CODE_CONSTITUTION.md
│  ├─ architecture/
│  │  ├─ ENGINE_ARCHITECTURE.md
│  │  └─ world-a-r0-coordinate-contract.md
│  ├─ archive/
│  │  └─ changelog/
│  │     ├─ changelog-2026-05.md
│  │     ├─ changelog-2026-06.md
│  │     └─ changelog-2026-07.md
│  ├─ dev-rules.md
│  ├─ docs-index.md
│  ├─ governance/
│  │  ├─ NAMING_RULES.md
│  │  ├─ debts/
│  │  │  ├─ arch-ui-spec-debts.md
│  │  │  └─ arch-world-debts.md
│  │  ├─ dev-rules-understanding.md
│  │  ├─ diagnostic-safety.md
│  │  ├─ naming-XuanYu-Engine.md
│  │  └─ ui-spec.md
│  ├─ knowledge/
│  │  ├─ README.md
│  │  ├─ architecture.md
│  │  ├─ data.md
│  │  ├─ engineering.md
│  │  ├─ incidents.md
│  │  ├─ input.md
│  │  ├─ knowledge-index.md
│  │  ├─ lessons.md
│  │  ├─ performance.md
│  │  ├─ rendering.md
│  │  └─ ui.md
│  ├─ milestones/
│  │  └─ current/
│  │     └─ MAP-A/
│  │        ├─ R3-C2-closure.md
│  │        ├─ R3-F1-closeout.md
│  │        ├─ R3-backlog.md
│  │        ├─ map-contract.md
│  │        ├─ viewport-overlay-development-plan.md
│  │        └─ viewport-overlay-roadmap.svg
│  └─ ui/
│     ├─ 玄域引擎_UI真机基线清单.md
│     ├─ 玄域引擎_UI规范_1.0.md
│     └─ 玄域引擎_旧UI审计矩阵.md
├─ file-tree.md
├─ run.bat
├─ samples/
│  └─ world-c-r1-ten-triangles.xyscene
└─ scripts/
   ├─ arch-a-guard-editor.ps1
   ├─ arch-a-guard-render.ps1
   ├─ arch-a-guard-warcore.ps1
   ├─ arch-a-guard-world.ps1
   └─ arch-a-guard.ps1
```

## 文件职责索引

- `.gitattributes` — （职责待补）
- `.gitignore` — （职责待补）
- `AGENTS.md` — （职责待补）
- `NuGet.Config` — api.nuget.org/v3/index.json" />
- `XuanYu.Core.Tests/Camera/CameraBasisTests.cs` — F3-F2：唯一相机正交基生成器合同——成功结果必须三轴单位正交，失败必须明确原因。
- `XuanYu.Core.Tests/Camera/FarProjectionSafetyTests.cs` — F1-FAR-SAFE-01：极远 Metric 失败安全与纯 double 诊断回归。
- `XuanYu.Core.Tests/Camera/CameraFarRecoveryTests.cs` — F1-FAR-RECOVERY-01：Far 随当前距离回落及编辑器距离上限回归。
- `XuanYu.Core.Tests/Camera/CameraNavigationRollTests.cs` — F3-F3：Orbit 地平线合同——普通环绕保持世界 +Z Up、无 Roll、不累积倾斜。
- `XuanYu.Core.Tests/Camera/CameraNavigationSequenceTests.cs` — F3-F2：导航组合链崩溃回归——顶/底视后任何导航不得再抛 CameraState 参数异常。
- `XuanYu.Core.Tests/Camera/CameraNavigationStressTests.cs` — F3-F2（计划 14.4）：重复导航压力测试——固定序列循环 100 次，检测累积误差与逐步失去正交。
- `XuanYu.Core.Tests/Camera/CameraNavigationTests.cs` — sealed class CameraNavigationTests
- `XuanYu.Core.Tests/Camera/CameraNavigationUiSequenceTests.Safety.cs` — F3-F2（计划 14.5/14.6）：失败安全与状态合同——取消恢复、非法输入拒绝、导航不 Dirty/Undo。
- `XuanYu.Core.Tests/Camera/CameraNavigationUiSequenceTests.cs` — F3-F2：UiVm 相机导航组合序列——标准视角/Orbit/Pan/Dolly/Resize 任意组合不抛异常且基合法。
- `XuanYu.Core.Tests/Camera/CameraOrthographicNavigationTests.cs` — F3-F4：正交导航语义（Dolly 缩放尺度不动位置、Pan 保持正交、Orbit 恢复透视）+ 正交视图工厂。
- `XuanYu.Core.Tests/CoreSmokeTests.cs` — sealed class CoreSmokeTests
- `XuanYu.Core.Tests/EditorTool/EditorTransformCapturePolicyTests.cs` — sealed class EditorTransformCapturePolicyTests
- `XuanYu.Core.Tests/Gizmo/MoveGizmoDragConstraintTests.cs` — sealed class MoveGizmoDragConstraintTests
- `XuanYu.Core.Tests/Gizmo/MoveGizmoLayoutG1Tests.cs` — sealed partial class MoveGizmoLayoutTests
- `XuanYu.Core.Tests/Gizmo/MoveGizmoLayoutPlaneTests.cs` — sealed partial class MoveGizmoLayoutTests
- `XuanYu.Core.Tests/Gizmo/MoveGizmoLayoutTests.cs` — 命中半径必须由“可见几何 + 显式容差”派生，禁止再开大半径
- `XuanYu.Core.Tests/Gizmo/MoveGizmoLayoutVulkanTests.cs` — sealed partial class MoveGizmoLayoutTests
- `XuanYu.Core.Tests/Gizmo/MoveGizmoScreenSizeTests.cs` — sealed class MoveGizmoScreenSizeTests
- `XuanYu.Core.Tests/Gizmo/RotateGizmoLayoutTests.cs` — 命中半径必须由“可见环几何 + 显式容差”派生，禁止再开大半径
- `XuanYu.Core.Tests/Gizmo/ScaleGizmoTests.Drag.cs` — sealed partial class ScaleGizmoTests
- `XuanYu.Core.Tests/Gizmo/ScaleGizmoTests.DragSafety.cs` — sealed partial class ScaleGizmoTests
- `XuanYu.Core.Tests/Gizmo/ScaleGizmoTests.Helpers.cs` — sealed partial class ScaleGizmoTests
- `XuanYu.Core.Tests/Gizmo/ScaleGizmoTests.R5R1.cs` — sealed partial class ScaleGizmoTests
- `XuanYu.Core.Tests/Gizmo/ScaleGizmoTests.cs` — R5：Scale Gizmo 纯函数契约测试 —— 单轴只改对应分量、Uniform 三轴同倍、倍率恒正且不穿过零。
- `XuanYu.Core.Tests/History/EditorHistoryOwnerTests.cs` — sealed class EditorHistoryOwnerTests
- `XuanYu.Core.Tests/History/EditorHistoryRedoTests.cs` — sealed class EditorHistoryRedoTests
- `XuanYu.Core.Tests/History/TransformHistoryIntegrationTests.cs` — sealed class TransformHistoryIntegrationTests
- `XuanYu.Core.Tests/History/TransformHistoryRedoIntegrationTests.cs` — sealed class TransformHistoryRedoIntegrationTests
- `XuanYu.Core.Tests/Picking/ViewportPickingServiceTests.cs` — sealed class ViewportPickingServiceTests
- `XuanYu.Core.Tests/Render/Camera/StandardViewResolverTests.cs` — F3-D3：六方向标准视角解析测试（计划 11.4——Pivot/距离保留、Up 合同、无滚转/镜像）。
- `XuanYu.Core.Tests/Render/Diagnostics/RenderLogNoiseContractTests.cs` — 日志高频噪声边界合同测试。
- `XuanYu.Core.Tests/Render/DrawPlan/CubeRenderDrawPlanTests.cs` — sealed class CubeRenderDrawPlanTests
- `XuanYu.Core.Tests/Render/DrawPlan/FrameExecutionPolicyTests.cs` — R4-R3-R2：验证 Vulkan Present 循环帧执行顺序：
- `XuanYu.Core.Tests/Render/DrawPlan/RenderDrawPlanTests.cs` — R4-R3-R2：验证绘制计划——未选中仅 Fill(3)，选中 Fill(3) + OutlineRibbon(18)，
- `XuanYu.Core.Tests/Render/DrawPlan/SceneRenderProjectionAdapterTests.Rotation.cs` — sealed partial class SceneRenderProjectionAdapterTests
- `XuanYu.Core.Tests/Render/DrawPlan/SceneRenderProjectionAdapterTests.Selection.cs` — R4-R3：轮廓高亮目标必须等价于“当前选中实体”，且与工具/层级树来源无关。
- `XuanYu.Core.Tests/Render/DrawPlan/SceneRenderProjectionAdapterTests.cs` — sealed partial class SceneRenderProjectionAdapterTests
- `XuanYu.Core.Tests/Render/DrawPlan/ViewportAssistDrawPlanTests.cs` — F3-F1：导航 Gizmo 恒为最后一项（Overlay Pass 收尾）。
- `XuanYu.Core.Tests/Render/DrawPlan/ViewportChromeContractTests.cs` — F3-D1：视口黑边合同测试（计划 11.1）——XAML 防退化：
- `XuanYu.Core.Tests/Render/DrawPlan/ViewportScaleIndicatorContractTests.cs` — OVL-R2/R3：比例尺 Vulkan DrawKind、Depth Off、顺序与 Native Popup 删除合同。
- `XuanYu.Core.Tests/Render/Grid/ReferenceGridDrawPlanTests.cs` — MAP-A-R1-D5-R1-F2-R2：DrawPlan 合同——顺序（方案 12）与开关独立（方案 11.2）。
- `XuanYu.Core.Tests/Render/Grid/ReferenceGridFrameStateTests.cs` — GRID-RW-2B：1/2/5 全帧 Step 与 24~80 DIP 回滞合同。
- `XuanYu.Core.Tests/Render/Grid/ScaleIndicatorMetricTests.cs` — MAP-A-R3-D2-F1-V3/A02：比例尺 1/2/5 距离选择、100m 最小距离与目标宽度合同。
- `XuanYu.Core.Tests/Render/Overlay/ScaleIndicatorGlyphLiteTests.cs` — OVL-R2：比例尺受限字符编码合同。
- `XuanYu.Core.Tests/Render/Overlay/ViewportOverlayLayoutTests.cs` — OVL-R1：Anchor/Rect/DIP 布局与边界合同。
- `XuanYu.Core.Tests/Render/Grid/ViewportMetricScaleTests.cs` — MAP-A-R3-D2-F1-V2：Perspective/Orthographic 的 DIP 与 physical pixel 尺度合同（1.00/1.25/1.50/2.00 DPI）。
- `XuanYu.Core.Tests/Render/Grid/ReferenceGridShaderContractTests.cs` — GRID-RW-2B：World XY 全帧 Step、全屏 Pass 与无深度依赖合同。
- `XuanYu.Core.Tests/Render/Map/MapRenderDrawPlanTests.cs` — MAP-A-R1-D4/D5-R1（F2-R2/D4）：RenderProjection 携带地图快照后，参考网格保留（无限参考平面，
- `XuanYu.Core.Tests/Render/Map/MapSurfaceGeometryTests.cs` — MAP-A-R2-D3：有限地面常量几何合同——固定 4 顶点 6 索引，尺寸只进顶点坐标。
- `XuanYu.Core.Tests/Render/Map/MapSurfaceLayerVisibilityTests.cs` — MAP-A-R2-D4（R06）：图层显隐不进 GPU 资源判等键——显隐切换只推进序号，不重建资源。
- `XuanYu.Core.Tests/Render/Map/MapSurfaceResourceKeyTests.cs` — MAP-A-R2-D3-A1：GPU 资源判等键合同——Rename 不重建、几何变化必重建、Sequence 不进键。
- `XuanYu.Core.Tests/Render/Map/MapSurfaceResourceUpdatePolicyTests.cs` — MAP-A-R2-D3-A1：地图 GPU 资源更新决策（纯策略）——旧序号拒绝、同键不重建、异键重建。
- `XuanYu.Core.Tests/Render/NavigationGizmo/NavigationGizmoLayoutTests.Facing.cs` — F3-F3：导航 Gizmo 正对相机合同——轴正对时只显示朝向端点、隐藏背向端点、命中优先端点。
- `XuanYu.Core.Tests/Render/NavigationGizmo/NavigationGizmoLayoutTests.cs` — F3-D2/D3/F3-F3：导航 Gizmo 布局投影与命中测试（96 DIP 区域；正对合同见 .Facing.cs）。
- `XuanYu.Core.Tests/Render/NavigationGizmo/NavigationGizmoOverlayContractTests.cs` — F3-F1：导航 Gizmo Overlay Pass 与屏幕空间原点标记合同测试。
- `XuanYu.Core.Tests/Render/NavigationGizmo/NavigationGizmoDipContractTests.cs` — 导航 Gizmo DIP 尺寸与 DPI 缩放合同测试。
- `XuanYu.Core.Tests/Render/NavigationGizmo/NavigationGizmoInputIsolationTests.cs` — STAB-1：可见 Gizmo 端点/轴线命中，空白区域不消费 Region 输入。
- `XuanYu.Core.Tests/Render/StaticModels/StaticModelDepthRegressionTests.cs` — sealed class StaticModelDepthRegressionTests
- `XuanYu.Core.Tests/Render/StaticModels/RegionModelTransformContractTests.cs` — 区域 world-space 静态模型单位变换合同测试。
- `XuanYu.Core.Tests/Render/StaticModels/StaticModelRenderContractTests.cs` — sealed class StaticModelRenderContractTests
- `XuanYu.Core.Tests/Space/CameraOrthographicTests.cs` — F3-F4：正交投影契约（模式校验/射线/尺度投影/往返/深度/Fov 无关）。
- `XuanYu.Core.Tests/Space/CameraStateTests.cs` — sealed class CameraStateTests
- `XuanYu.Core.Tests/Space/DefaultEditorCameraTests.cs` — sealed class DefaultEditorCameraTests
- `XuanYu.Core.Tests/Space/SpaceAssert.cs` — （职责待补）
- `XuanYu.Core.Tests/Space/ViewProjectionStateTests.cs` — sealed class ViewProjectionStateTests
- `XuanYu.Core.Tests/Space/ViewportStateTests.cs` — sealed class ViewportStateTests
- `XuanYu.Core.Tests/Space/WorldRayFactoryTests.cs` — sealed class WorldRayFactoryTests
- `XuanYu.Core.Tests/Space/WorldRayTests.cs` — sealed class WorldRayTests
- `XuanYu.Core.Tests/Spatial/RayAabbIntersectionTests.cs` — sealed class RayAabbIntersectionTests
- `XuanYu.Core.Tests/Spatial/SpatialBoundsTests.cs` — sealed class SpatialBoundsTests
- `XuanYu.Core.Tests/Spatial/SpatialTestData.cs` — （职责待补）
- `XuanYu.Core.Tests/XuanYu.Core.Tests.csproj` — （职责待补）
- `XuanYu.Core/.gitkeep` — （职责待补）
- `XuanYu.Core/Diagnostics/CoreSelfTest.cs` — static class CoreSelfTest
- `XuanYu.Core/Gizmo/Common/ScreenPoint.cs` — （职责待补）
- `XuanYu.Core/Gizmo/Move/MoveGizmoAxis.cs` — enum MoveGizmoAxis
- `XuanYu.Core/Gizmo/Move/MoveGizmoDragConstraint.Axes.cs` — （职责待补）
- `XuanYu.Core/Gizmo/Move/MoveGizmoDragConstraint.cs` — （职责待补）
- `XuanYu.Core/Gizmo/Move/MoveGizmoLayout.Hit.cs` — sealed partial class MoveGizmoLayout
- `XuanYu.Core/Gizmo/Move/MoveGizmoLayout.Plane.cs` — sealed partial class MoveGizmoLayout
- `XuanYu.Core/Gizmo/Move/MoveGizmoLayout.cs` — 可见轴杆线宽（DIP）。与 Vulkan 顶点着色器生成的 Gizmo 几何同尺度（审计实测约 2–3px）。
- `XuanYu.Core/Gizmo/Move/MoveGizmoPlane.cs` — （职责待补）
- `XuanYu.Core/Gizmo/Move/MoveGizmoScreenSize.cs` — Move Gizmo 的屏幕恒定尺寸真源。CPU 布局与 Vulkan 绘制共用同一世界轴长。
- `XuanYu.Core/Gizmo/Move/MoveGizmoSegment.cs` — （职责待补）
- `XuanYu.Core/Gizmo/Rotate/RotateGizmoAxis.cs` — enum RotateGizmoAxis
- `XuanYu.Core/Gizmo/Rotate/RotateGizmoDrag.Math.cs` — 旋转解算的纯静态数学辅助，与实例状态分离的 partial。
- `XuanYu.Core/Gizmo/Rotate/RotateGizmoDrag.cs` — 旋转拖拽解算：将指针在"垂直于旋转轴的平面"上的投影角度变化，映射为
- `XuanYu.Core/Gizmo/Rotate/RotateGizmoLayout.cs` — 旋转环世界半径默认值（与 MoveGizmo AxisLength=1.2 同尺度）。
- `XuanYu.Core/Gizmo/Rotate/RotateGizmoRing.cs` — 一条旋转环的屏幕折线几何。命中以"指针到折线最近距离"为唯一真源，
- `XuanYu.Core/Gizmo/Rotate/RotateGizmoScreenRadius.cs` — 旋转环屏幕空间恒定尺寸换算：将目标 DIP 半径按相机深度与视口逻辑高度换算为世界半径。
- `XuanYu.Core/Gizmo/Scale/ScaleGizmoAxis.cs` — 单轴缩放手柄：修改实体自身 TRS 的局部 X / Y / Z 分量。
- `XuanYu.Core/Gizmo/Scale/ScaleGizmoDrag.cs` — Scale Gizmo 拖拽解算：指数映射，倍率恒为正、不穿过零，且不逐帧累乘。
- `XuanYu.Core/Gizmo/Scale/ScaleGizmoHitTester.cs` — CPU 命中布局与 Vulkan 绘制共用 ScaleGizmoLayout，保证“看见的位置 = 实际命中位置”。
- `XuanYu.Core/Gizmo/Scale/ScaleGizmoLayout.cs` — Scale Gizmo 屏幕空间布局：三轴末端控制柄 + 中心等比控制柄。
- `XuanYu.Core/Gizmo/Scale/ScaleGizmoScreenSize.cs` — Scale Gizmo 屏幕空间恒定尺寸换算（与 RotateGizmoScreenRadius 同思路）。
- `XuanYu.Core/History/EditorHistoryOwner.cs` — sealed class EditorHistoryOwner
- `XuanYu.Core/History/TransformHistoryEntry.cs` — （职责待补）
- `XuanYu.Core/Identity/EntityId.cs` — （职责待补）
- `XuanYu.Core/Logging/EngineLogEntry.cs` — （职责待补）
- `XuanYu.Core/Logging/EngineLogLevel.cs` — enum EngineLogLevel
- `XuanYu.Core/Map/MapSurfaceKind.cs` — MAP-A-R1-D3：R1 地表类型。与 .xymap 合同 surface.kind 对应（Editor 桥接负责字符串映射）。
- `XuanYu.Core/Map/MapSurfaceSampler.cs` — MAP-A-R1-D3：唯一地表采样源。
- `XuanYu.Core/Map/MapTerrainVertex.cs` — MAP-A-R1-D4：地形网格顶点。布局与 Vulkan 侧 StaticModelVertex 一致：
- `XuanYu.Core/Math/Vector3d.cs` — （职责待补）
- `XuanYu.Core/Math/YawRotation.cs` — （职责待补）
- `XuanYu.Core/Picking/ViewportPickingRequest.cs` — （职责待补）
- `XuanYu.Core/Picking/ViewportPickingResult.cs` — （职责待补）
- `XuanYu.Core/Picking/ViewportPickingService.cs` — static class ViewportPickingService
- `XuanYu.Core/Properties/AssemblyInfo.cs` — （职责待补）
- `XuanYu.Core/Results/EngineError.cs` — （职责待补）
- `XuanYu.Core/Results/EngineResult.cs` — （职责待补）
- `XuanYu.Core/Scene/CommittedTransform.cs` — （职责待补）
- `XuanYu.Core/Scene/ISceneRenderSnapshotSource.cs` — interface ISceneRenderSnapshotSource
- `XuanYu.Core/Scene/SceneEntitySnapshot.cs` — （职责待补）
- `XuanYu.Core/Scene/SceneRenderSnapshot.cs` — （职责待补）
- `XuanYu.Core/Scene/SceneTransformCommitResult.cs` — （职责待补）
- `XuanYu.Core/Space/CameraState.cs` — （职责待补）
- `XuanYu.Core/Space/DefaultEditorCamera.cs` — static class DefaultEditorCamera
- `XuanYu.Core/Space/ProjectionMode.cs` — F3-F4：相机投影模式。透视=自由观察默认；正交=标准方向视图（±X/±Y/±Z）。
- `XuanYu.Core/Space/ViewProjectionState.cs` — sealed class ViewProjectionState
- `XuanYu.Core/Space/ViewProjectionState.Projection.cs` — 世界点严格投影与失败安全 Try 投影 API。
- `XuanYu.Core/Space/ViewportState.cs` — （职责待补）
- `XuanYu.Core/Space/WorldRay.cs` — （职责待补）
- `XuanYu.Core/Space/WorldRayFactory.cs` — 基于 CameraState 与 ViewportState 的双精度世界射线构造。
- `XuanYu.Core/Spatial/RayAabbHit.cs` — （职责待补）
- `XuanYu.Core/Spatial/RayAabbIntersection.cs` — static class RayAabbIntersection
- `XuanYu.Core/Spatial/SpatialAabb.cs` — （职责待补）
- `XuanYu.Core/Spatial/SpatialBounds.cs` — （职责待补）
- `XuanYu.Core/Spatial/SpatialQueryCategory.cs` — enum SpatialQueryCategory
- `XuanYu.Core/Spatial/SpatialQueryResult.cs` — sealed class SpatialQueryResult
- `XuanYu.Core/Spatial/SpatialQueryStats.cs` — （职责待补）
- `XuanYu.Core/Spatial/SpatialRayAabb.cs` — static class SpatialRayAabb
- `XuanYu.Core/Spatial/SpatialRayQuery.cs` — （职责待补）
- `XuanYu.Core/Spatial/SpatialRaycastHit.cs` — （职责待补）
- `XuanYu.Core/Spatial/SpatialRaycastResult.cs` — sealed class SpatialRaycastResult
- `XuanYu.Core/Spatial/SpatialRaycastStats.cs` — （职责待补）
- `XuanYu.Core/Time/SimulationTime.cs` — （职责待补）
- `XuanYu.Core/Time/TimeStep.cs` — （职责待补）
- `XuanYu.Core/Transform/PreviewTransform.cs` — （职责待补）
- `XuanYu.Core/Transform/TransformStartSnapshot.cs` — （职责待补）
- `XuanYu.Core/XuanYu.Core.csproj` — （职责待补）
- `XuanYu.Editor.App/EditorCompositionRoot.cs` — static class EditorCompositionRoot
- `XuanYu.Editor.App/Program.cs` — （职责待补）
- `XuanYu.Editor.App/XuanYu.Editor.App.csproj` — （职责待补）
- `XuanYu.Editor.UI/Bootstrap/App.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Bootstrap/App.axaml.cs` — sealed class App
- `XuanYu.Editor.UI/Bootstrap/Program.cs` — WinExe 进程默认无控制台；AttachConsole(-1) 继承父终端（dotnet run 控制台），
- `XuanYu.Editor.UI/Accessibility/UiAutomationNamer.cs` — D6：启动后为缺失自动化名称的基础控件补名，并阻止 ARCH/D6 等内部治理代号进入自动化名称。
- `XuanYu.Editor.UI/Accessibility/UiDpiContract.cs` — D6：桌面缩放与 DIP 阈值合同（100/125/150/175/200，最小/推荐窗口与表单阈值）。
- `XuanYu.Editor.UI/Accessibility/UiMotionPreference.cs` — D6：减少动画偏好合同（Reduce 归零非必要动效，Default 保持短反馈）。
- `XuanYu.Editor.UI/Design/UiTokens.Colors.Components.axaml` — UI Token 组件色（日志/文档状态/图层，UI Spec 1.0 §4.3/§4.4/§12.2）
- `XuanYu.Editor.UI/Design/UiTokens.Colors.Core.axaml` — UI Token 核心语义色（四级背景/文字/强调/状态/对象，§4.1/§4.2）
- `XuanYu.Editor.UI/Design/UiTokens.Controls.axaml` — UI Token 控件尺寸（高度/宽度等级/热区/边框/焦点/阴影/日志列宽，§5.3/§6/§9/§13）
- `XuanYu.Editor.UI/Design/UiTokens.Fonts.axaml` — UI Token 字体（回退链/8 级字号行高/字重，§3.1/§3.2/§3.4）
- `XuanYu.Editor.UI/Design/UiTokens.Icons.axaml` — UI Token 图标（视口/笔画，§8.1）
- `XuanYu.Editor.UI/Design/UiTokens.Motion.axaml` — UI Token 动效时长（悬停/展开，§15.3）
- `XuanYu.Editor.UI/Design/UiTokens.Spacing.axaml` — UI Token 间距/内边距/圆角（§5.1/§5.2/§5.4）
- `XuanYu.Editor.UI/Design/UiStyles.D4F1.axaml` — D4-F1（纠偏）：公共语义样式独立文件（uiLabel/uiValue/uiSingleLine/uiMultiline/uiSection/uiTextButton，Setter 正常分行，全部 Token 引用；由 Ui.axaml 聚合一次）。
- `XuanYu.Editor.UI/Design/UiTokenManifest.json` — UI Token 唯一机器事实源（112 条：Key/Type/Value/Category/SpecSection/Purpose/SpecStatus；D2-F1）
- `XuanYu.Editor.UI/Design/UiTokens.axaml` — UI Token 聚合入口（合并 7 个 Token 文件；由 UiTokenManifest.json 生成，禁手改）
- `XuanYu.Editor.UI/Dialogs/IEditorDialogService.cs` — D4：最小错误弹窗服务。只用于用户主动操作失败（导入 GLB / 打开场景 / 部分资源缺失）。
- `XuanYu.Editor.UI/Dialogs/NullEditorDialogService.cs` — D4：无窗口环境的空实现（测试 / 无 UI 宿主），避免 NRE。
- `XuanYu.Editor.UI/EditorState/EditorInteractionChangedResult.cs` — enum EditorInteractionChangeKind
- `XuanYu.Editor.UI/EditorState/EditorInteractionCommand.cs` — sealed record BeginInteractionCommand
- `XuanYu.Editor.UI/EditorState/EditorInteractionPointerSnapshot.cs` — （职责待补）
- `XuanYu.Editor.UI/EditorState/EditorInteractionSnapshot.cs` — enum EditorInteractionPhase
- `XuanYu.Editor.UI/EditorState/EditorSelectionCommand.cs` — sealed record SelectEditorItemCommand
- `XuanYu.Editor.UI/EditorState/EditorSelectionSnapshot.cs` — sealed record EditorSelectionSnapshot
- `XuanYu.Editor.UI/EditorState/EditorStateChangedResult.cs` — enum EditorStateChangeKind
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.Interaction.cs` — sealed partial class EditorStateOwner
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.Tool.cs` — sealed partial class EditorStateOwner
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.cs` — sealed partial class EditorStateOwner
- `XuanYu.Editor.UI/EditorState/EditorToolChangedResult.cs` — sealed record EditorToolChangedResult
- `XuanYu.Editor.UI/EditorState/EditorToolCommand.cs` — sealed record ChangeEditorToolCommand
- `XuanYu.Editor.UI/EditorState/EditorToolId.cs` — enum EditorToolId
- `XuanYu.Editor.UI/EditorState/EditorToolSnapshot.cs` — sealed record EditorToolSnapshot
- `XuanYu.Editor.UI/EditorState/EditorToolText.cs` — static class EditorToolText
- `XuanYu.Editor.UI/EditorState/EditorTransformCapturePolicy.cs` — static class EditorTransformCapturePolicy
- `XuanYu.Editor.UI/Foot/Foot.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Foot/Foot.States.axaml` — Foot 日志选中状态的模板 Presenter 样式覆盖。
- `XuanYu.Editor.UI/Foot/Foot.axaml.cs` — LOG-UX-2：Foot.axaml.cs 只做接线——自动滚动 controller、日志选中、Ctrl+A/Ctrl+C。
- `XuanYu.Editor.UI/Foot/LogAutoScrollPolicy.cs` — MAP-A-R2-D3-F2：日志自动跟随纯策略——底部附近跟随、远离不强制拉回、滚到底恢复。
- `XuanYu.Editor.UI/Foot/LogDetailPanel.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Foot/LogDetailPanel.axaml.cs` — partial class LogDetailPanel
- `XuanYu.Editor.UI/Foot/LogListAutoScrollController.Follow.cs` — MAP-A-R2-D3-F3：两阶段尾项定位——第一阶段目标式滚动（Render），
- `XuanYu.Editor.UI/Foot/LogListAutoScrollController.Layout.cs` — MAP-A-R2-D3-F3：布局变化统一处理——ScrollChanged 集中覆盖
- `XuanYu.Editor.UI/Foot/LogListAutoScrollController.cs` — MAP-A-R2-D3-F3：日志列表尾项定位——两阶段（Render 目标滚动 + Background 布局后修正），
- `XuanYu.Editor.UI/Icons/EditorIcons.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Left/InlineRenameActivation.cs` — static class InlineRenameActivation
- `XuanYu.Editor.UI/Left/Left.EntityCommands.cs` — partial class Left
- `XuanYu.Editor.UI/Left/Left.Styles.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Left/Left.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Left/Left.axaml.cs` — partial class Left
- `XuanYu.Editor.UI/Main/Main.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Main/Main.axaml.cs` — partial class Main
- `XuanYu.Editor.UI/NativeHostResizeCoalescer.cs` — / <summary>
- `XuanYu.Editor.UI/NativeHostResizeSnapshot.cs` — / <summary>
- `XuanYu.Editor.UI/NativeHostSurfaceContract.cs` — VK3-A：把现有 NativeHost 生命周期快照映射为渲染层交接句柄。
- `XuanYu.Editor.UI/RelayCommand.cs` — sealed class RelayCommand
- `XuanYu.Editor.UI/Right/LayerInspectorPanel.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Right/LayerInspectorPanel.axaml.cs` — MAP-A-R2-D4：图层检查器（名称 Enter/失焦提交；开关/按钮走绑定，无额外逻辑）。
- `XuanYu.Editor.UI/Right/LayerPanel.DragDrop.cs` — MAP-A-R2-D4-F3：区域图层拖动（code-behind 只处理指针/Drop；手柄按下 ≥4 DIP 启动；仅区域行接受；一次交给 UiVm）。
- `XuanYu.Editor.UI/Right/LayerPanel.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Right/LayerPanel.States.axaml` — 图层行选中、可见与锁定状态的最终渲染样式。
- `XuanYu.Editor.UI/Right/LayerPanel.axaml.cs` — MAP-A-R2-D4：图层面板（左侧"图层"页签内容，纯绑定；无 code-behind 逻辑）。
- `XuanYu.Editor.UI/Right/MapEditorPanel.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Right/MapEditorPanel.axaml.cs` — MAP-A-R1-D5-A：地图编辑器面板（二级页签宿主：地图/图层/环境，每页独立滚动）。
- `XuanYu.Editor.UI/Right/MapIdDisplayFormat.cs` — D4：MapId 显示压缩纯逻辑（>18 字符「前 8+…+后 6」）。
- `XuanYu.Editor.UI/Right/MapPagePanel.axaml` — D4：地图页（只读资产摘要 72 列紧凑 / MapId 压缩+复制 / 属性表单 96 列 + 紧凑模式双布局 / 按钮组）。
- `XuanYu.Editor.UI/Right/MapPagePanel.axaml.cs` — D4/D4-F1（纠偏）：地图页密度切换（<320 紧凑：根留白/分组间距/行距）+ MapId 完整值复制（Clipboard）。
- `XuanYu.Editor.UI/Right/MapFormPanel.axaml` — D4-F1（纠偏拆分）：地图属性输入表单（96 列标准 / <360 窄模式双布局 / 按钮 Grid *,* 跨列布局）。
- `XuanYu.Editor.UI/Right/MapFormPanel.axaml.cs` — D4-F1（纠偏）：地图属性表单方向切换（EditableFormLayoutModel <360 整组上下）。
- `XuanYu.Editor.UI/Right/MapEditorLayoutModel.cs` — ARCH-UI-SPEC-R1-D4/D4-F1（纠偏恢复）：面板紧凑密度纯逻辑（<320 紧凑：根留白/分组间距/字段行距；与 EditableFormLayoutModel 并存互不替代）。
- `XuanYu.Editor.UI/Right/EditableFormLayoutModel.cs` — ARCH-UI-SPEC-R1-D4/D4-F1：可编辑表单行（EditableFormRow）布局模式纯逻辑（仅真实输入控件在内容宽 <360 整组上下；96 标签列/128 字段最小；只读键值行不参与）。
- `XuanYu.Editor.UI/Right/InspectorPanel.axaml` — D4：检查器面板（字号 Token 层级 / 宽窄双布局树 / 全宽分组+分隔线 / 空状态）。
- `XuanYu.Editor.UI/Right/InspectorPanel.axaml.cs` — D4：检查器模式切换（<360 窄模式，同一 InspectorFields 数据源）。
- `XuanYu.Editor.UI/Right/Right.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Right/Right.axaml` — 顶层右侧页签，向 UiVm 同步当前页签以区分地图编辑模式。
- `XuanYu.Editor.UI/Right/Right.axaml.cs` — partial class Right（D3：挂载 TopTabStripController 顶层页签控制器）
- `XuanYu.Editor.UI/Right/TopTabStripModel.cs` — ARCH-UI-SPEC-R1-D3：顶层页签条纯布局状态机（溢出/箭头/渐隐/滚轮路由/可见性/提示门/全部页签列表，无 Avalonia 依赖）
- `XuanYu.Editor.UI/Right/TopTabStripController.cs` — D3：页签条控制器（模板元素接线/滚轮隧道消费/刷新）
- `XuanYu.Editor.UI/Right/TopTabStripController.AllTabs.cs` — D3：「全部页签」入口（真实页签列表/当前项标记/跳转自动显露）
- `XuanYu.Editor.UI/Right/TopTabStripController.Hint.cs` — D3：首次溢出一次性提示（用户环境持久化 %APPDATA%\XuanYuEngine\ui-once.json）
- `XuanYu.Editor.UI/Right/TopTabStripController.Visible.cs` — D3：箭头/渐隐状态刷新与当前页签自动可见
- `XuanYu.Editor.UI/Right/TopTabStripTemplate.axaml` — D3：顶层页签单行溢出宿主模板（ScrollViewer 单行/箭头/渐隐/全部页签/提示 Popup）
- `XuanYu.Editor.UI/Root/UiRoot.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Root/UiRoot.axaml.cs` — Row1 主工作区最低高度（与 axaml MinHeight 一致）
- `XuanYu.Editor.UI/Top/Top.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Top/Top.States.axaml` — 顶部工具 ToggleButton 状态的模板 Presenter 样式覆盖。
- `XuanYu.Editor.UI/Top/Top.axaml.cs` — partial class Top
- `XuanYu.Editor.UI/TreeGuide.cs` — sealed class TreeGuide
- `XuanYu.Editor.UI/TreeGuideSegment.cs` — enum TreeGuideSegmentKind
- `XuanYu.Editor.UI/Ui.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Viewport/ViewNavigationGizmo.HitTest.cs` — F3-D3/F3-F3/STAB-1：导航 Gizmo 端点、轴线与中心球命中统一事实源。
- `XuanYu.Editor.UI/Viewport/ViewNavigationGizmo.Layout.cs` — F3-D2/F3-F3：导航 Gizmo 布局纯数学——六个世界方向投影到 Gizmo 屏幕平面。
- `XuanYu.Editor.UI/Viewport/Vulkan/NativePointerMessage.cs` — （职责待补）
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.AvaloniaCamera.cs` — sealed partial class VulkanNativeHost
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.AvaloniaPointer.cs` — STAB-1：Avalonia 指针路径先交给 Navigation Gizmo，再进入 Region/Picking，并捕获/释放手势。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Bridge.cs` — sealed partial class VulkanNativeHost
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.CameraPointer.cs` — sealed partial class VulkanNativeHost
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Dpi.cs` — sealed partial class VulkanNativeHost
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Gizmo.cs` — sealed partial class VulkanNativeHost
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.LayoutSync.cs` — VIEWPORT-RESIZE-R2：修复 R1 引入的 DPI 错配。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Log.cs` — VK4-D-R2：后台 Present 泵日志必须回 UI 线程访问 DataContext / UiVm。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.NavGizmo.cs` — F3-F1/STAB-1：Native 指针流的 Gizmo 命中、轴线/端点手势所有权与 Region 隔离。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Picking.cs` — sealed partial class VulkanNativeHost
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs` — sealed partial class VulkanNativeHost
- `XuanYu.Editor.UI/Viewport/Vulkan/NativePointerRoutePolicy.cs` — F1-C2 REWORK：Native 中键/区域预览/左键拖动路由优先级纯逻辑合同。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs` — sealed partial class VulkanNativeHost
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml` — A02：Native Vulkan Host 覆盖整个视口，比例尺不再占用 Avalonia 独立行。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml.cs` — partial class VulkanViewport
- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.Input.cs` — （职责待补）
- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs` — 通用 Vulkan 子 HWND 生命周期、尺寸与窗口过程，不承载 Viewport Overlay。
- `XuanYu.Editor.UI/ViewportNativeHostRoute.cs` — static class ViewportNativeHostRoute
- `XuanYu.Editor.UI/Vm/Camera/CameraSessionMode.cs` — enum CameraSessionMode
- `XuanYu.Editor.UI/Vm/Camera/CameraSessionSnapshot.cs` — sealed record CameraSessionSnapshot
- `XuanYu.Editor.UI/Vm/Camera/StandardViewResolver.cs` — F3-D3：六方向标准视角解析（计划 8.1 命名：+X 视图/-X 视图/+Y 视图/-Y 视图/顶视图/底视图）。
- `XuanYu.Editor.UI/Vm/Camera/UiVm.Camera.Framing.cs` — F3-F4：取景命令。正交模式保持正交（尺度按包围范围适配），透视模式沿用距离构图。
- `XuanYu.Editor.UI/Vm/Camera/UiVm.Camera.Framing.Draft.cs` — F1-C2：按 Draft 顶点 AABB 与最小可视半径聚焦草稿。
- `XuanYu.Editor.UI/Vm/Camera/UiVm.Camera.cs` — F3-D2：导航 Gizmo 相机快照（Right/Up/Forward 投影输入；不含平移）。
- `XuanYu.Editor.UI/Vm/Camera/UiVm.CameraDolly.cs` — 地图编辑 Dolly 入口，在候选相机阶段触发极远安全诊断。
- `XuanYu.Editor.UI/Vm/Camera/UiVm.FarProjectionDiagnostic.cs` — F1-FAR：跨距离档写入纯 double 诊断，并报告一次相机工作上限。
- `XuanYu.Editor.UI/Vm/Camera/UiVm.CameraNavigation.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Camera/UiVm.ScaleIndicator.cs` — MAP-A-R3-D2-F1-V3/A02：比例尺展示状态消费统一 ViewportMetricScale，并保持 100m 最小层级。
- `XuanYu.World.Tests/UiRuntime/ScaleIndicatorVisibilityRuntimeTests.cs` — A02：检查器标签下比例尺可见且 Dolly 不能越过 100m Zoom Floor。
- `XuanYu.Editor.UI/Vm/Camera/UiVm.ViewGizmo.cs` — F3-D3：六方向标准视角命令（计划 8.1 命名；复用现有 ApplyViewFaceCommand 相机逻辑）。
- `XuanYu.Editor.UI/Vm/History/UiVm.EntityCommands.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/History/UiVm.History.Entities.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/History/UiVm.History.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Inspector/UiVm.Inspector.cs` — sealed partial class UiVm（D4：BuildInspectorFields 输出结构化 InspectorFieldRow）
- `XuanYu.Editor.UI/Vm/Inspector/InspectorFieldRow.cs` — D4：检查器字段行结构（Label/Value/IsGroupHeader，替代字符串拼接）
- `XuanYu.Editor.UI/Vm/Inspector/UiVm.InspectorInput.Parse.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Inspector/UiVm.InspectorInput.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Logging/DebugText.cs` — static class DebugText
- `XuanYu.Editor.UI/Vm/Logging/EditorDisplayText.cs` — static class EditorDisplayText
- `XuanYu.Editor.UI/Vm/Logging/EditorLogBuffer.cs` — sealed class EditorLogBuffer
- `XuanYu.Editor.UI/Vm/Logging/EditorLogBus.cs` — sealed class EditorLogBus
- `XuanYu.Editor.UI/Vm/Logging/EditorLogCategory.cs` — enum EditorLogCategory
- `XuanYu.Editor.UI/Vm/Logging/EditorLogClipboardText.cs` — static class EditorLogClipboardText
- `XuanYu.Editor.UI/Vm/Logging/EditorLogFilter.cs` — enum EditorLogFilter
- `XuanYu.Editor.UI/Vm/Logging/EditorLogFilterQuery.cs` — static class EditorLogFilterQuery
- `XuanYu.Editor.UI/Vm/Logging/EditorLogLevel.cs` — enum EditorLogLevel
- `XuanYu.Editor.UI/Vm/Logging/EditorLogNoiseFilter.cs` — static class EditorLogNoiseFilter
- `XuanYu.Editor.UI/Vm/Logging/EditorLogRepeatKey.cs` — （职责待补）
- `XuanYu.Editor.UI/Vm/Logging/EditorLogSource.cs` — enum EditorLogSource
- `XuanYu.Editor.UI/Vm/Logging/EditorLogSummary.cs` — 底部"最近"通知（F3 方案 B）：完整日志仍按真实时间保留在面板；
- `XuanYu.Editor.UI/Vm/Logging/LogEntry.cs` — sealed record LogEntry
- `XuanYu.Editor.UI/Vm/Logging/SampleLogEntries.cs` — static class SampleLogEntries
- `XuanYu.Editor.UI/Vm/Logging/UiText.cs` — static class UiText
- `XuanYu.Editor.UI/Vm/Logging/UiVm.Logging.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Map/MapLayerRowViewModel.cs` — MAP-A-R2-D4：图层行显示模型（面板行绑定；写操作转发会话命令，不直接持有领域状态）。
- `XuanYu.Editor.UI/Vm/Map/MapRegionRenderProjection.cs` — 将正式区域和绘制草稿投影为独立 Vector Overlay 资源。
- `XuanYu.Editor.UI/Vm/Map/MapVectorOverlayBuilder.cs` — F1-V1/B1：构建共享 BaseHeightMeters 世界锚点的 Fill、屏幕空间 Stroke 与 Marker 几何。
- `XuanYu.Editor.UI/Vm/Map/MapVectorOverlayBuilder.Finalize.cs` — Vector Overlay AABB 与稳定 revision 计算。
- `XuanYu.Editor.UI/Vm/Map/MapVectorOverlayTriangulation.cs` — F1-V1：Ear Clipping 凹多边形三角化。
- `XuanYu.Editor.UI/Vm/Map/MapRenderSnapshotProjection.cs` — MAP-A-R2-D3/D4：MapDefinition → MapRenderSnapshot 纯投影（渲染唯一输入）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapCommandRouting.cs` — MAP-A-R2-D3-F1：地图面板命令真实路由（UiVm.RunCommand → 地图命令）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapDiagnostics.Format.cs` — MAP-A-R2-D3-F2：地图日志显示映射（纯函数，内部枚举/错误码保持英文）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapDiagnostics.cs` — MAP-A-R2-D3-F2：地图命令低频诊断日志（复用既有日志总线，字段名/状态值全部中文显示）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapEditor.cs` — MAP-A-R2-D3：地图属性入口（唯一数据源 = MapSession；保存/打开按钮禁用防 v1 双权威，D6 恢复）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapEditor.Display.cs` — D4：MapIdDisplay（前 8…后 6 压缩）与 MapPathDisplay（空路径 —）显示层属性。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapHistory.cs` — MAP-A-R2-D3-A1 入口补接：地图撤销/重做（独立历史实例，不触碰场景实体历史）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapLayerDiagnostics.cs` — MAP-A-R2-D4/D4-F2：图层操作低频中文日志（复用既有日志总线）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapLayerDrag.cs` — MAP-A-R2-D4-F3：区域图层拖动排序（UI 层入口）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapLayerInspector.cs` — MAP-A-R2-D4：图层检查器入口（右侧检查器选中图层时显示）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapLayerSelection.cs` — MAP-A-R2-D4：图层选择状态与列表刷新（选中是 UI 临时状态；内容/活动变化后重建列表）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapLayers.cs` — MAP-A-R2-D4：图层列表与工具栏命令入口（唯一数据源 = MapSession.CurrentMap）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapRender.cs` — MAP-A-R2-D3：MapSession → 渲染快照 适配（唯一渲染输入）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapWorld.cs` — MAP-A-R2-D3：World 地图查询状态持有者（高度查询/边界判断权威，由会话 ContentChanged 同步）。
- `XuanYu.Editor.UI/Vm/UiVm.RightPanel.cs` — F1-C2：右侧页签状态与地图编辑模式判定。
- `XuanYu.Editor.UI/Vm/Scene/D2StaticModelDemo.cs` — （职责待补）
- `XuanYu.Editor.UI/Vm/Scene/SceneHistoryEntry.cs` — （职责待补）
- `XuanYu.Editor.UI/Vm/Scene/SceneRenderProjectionAdapter.cs` — static class SceneRenderProjectionAdapter
- `XuanYu.Editor.UI/Vm/Scene/StaticModelRenderAdapter.cs` — static class StaticModelRenderAdapter
- `XuanYu.Editor.UI/Vm/Scene/UiVm.DocumentStatus.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Scene/UiVm.RenderProjection.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Scene/UiVm.Scene.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Scene/UiVm.SceneDocument.New.cs` — D4：新建场景（5+100 拆分自 UiVm.SceneDocument.cs）。
- `XuanYu.Editor.UI/Vm/Scene/UiVm.SceneDocument.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Scene/UiVm.SceneDocumentLog.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Scene/UiVm.SceneDocumentMapRef.cs` — MAP-A-R1-D5-B：场景与地图引用的双向闭环。
- `XuanYu.Editor.UI/Vm/Scene/UiVm.SceneDocumentSave.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Scene/UiVm.StaticModelImport.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Scene/UiVm.WorldProjection.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Selection/UiVm.Picking.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Selection/UiVm.Selection.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Selection/UiVm.SelectionProjection.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Selection/UiVm.SelectionTrace.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Selection/UiVm.SelectionValidity.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Selection/UiVm.ViewportSelection.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Selection/ViewportPickingLogFormatter.cs` — static class ViewportPickingLogFormatter
- `XuanYu.Editor.UI/Vm/Transform/Move/UiVm.MoveGizmo.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Transform/Move/UiVm.MoveGizmoLogging.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Transform/Move/UiVm.MoveGizmoScreenSize.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Transform/Rotate/UiVm.RotateGizmo.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Transform/Scale/UiVm.ScaleGizmo.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Transform/UiVm.InputGuards.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Transform/UiVm.Interaction.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Transform/UiVm.InteractionCancel.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Transform/UiVm.InteractionPointer.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Transform/UiVm.Tool.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Transform/UiVm.ViewportAssist.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/Tree/EditorTreeNode.cs` — sealed class EditorTreeNode
- `XuanYu.Editor.UI/Vm/Tree/TreeGuideBuilder.cs` — static class TreeGuideBuilder
- `XuanYu.Editor.UI/Vm/Tree/UiVm.TreeCommands.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Vm/UiVm.cs` — sealed partial class UiVm
- `XuanYu.Editor.UI/Win/UiWin.Dialogs.cs` — D4：UiWin 错误/警告弹窗实现。复用 UiWin.UnsavedDialog 的窗口构建风格，
- `XuanYu.Editor.UI/Win/UiWin.EntityShortcuts.cs` — partial class UiWin
- `XuanYu.Editor.UI/Win/UiWin.MapCommands.cs` — MAP-A-R2-D3-F1：UiWin 地图命令仅保留快捷键可达的窗口无关命令（新建/聚焦）。
- `XuanYu.Editor.UI/Win/UiWin.SceneCommands.cs` — partial class UiWin
- `XuanYu.Editor.UI/Win/UiWin.UnsavedDialog.cs` — partial class UiWin
- `XuanYu.Editor.UI/Win/UiWin.axaml` — github.com/avaloniaui"
- `XuanYu.Editor.UI/Win/UiWin.axaml.cs` — partial class UiWin
- `XuanYu.Editor.UI/XuanYu.Editor.UI.csproj` — （职责待补）
- `XuanYu.Editor.UI/app.manifest` — （职责待补）
- `XuanYu.Editor.Win/MainForm.cs` — （职责待补）
- `XuanYu.Editor.Win/XuanYu.Editor.Win.csproj` — （职责待补）
- `XuanYu.Editor/Assets/Catalog/SceneStaticModelCatalog.cs` — D3：场景静态模型绑定目录。Editor 层唯一事实源：实体 → 资产 → 模型数据。
- `XuanYu.Editor/Assets/Hosting/HostedSceneAsset.cs` — D4-I1：托管资产项。SourcePath 是 D3 导入时记录的规范化绝对路径（运行时来源）；
- `XuanYu.Editor/Assets/Hosting/ModelAssetRuntimeState.cs` — enum ModelAssetRuntimeState
- `XuanYu.Editor/Assets/Hosting/Planning/SceneAssetHostingPlan.cs` — D4-I1：托管规划。Assets 按 AssetId.Value 稳定排序；所有绝对路径已 GetFullPath；
- `XuanYu.Editor/Assets/Hosting/Planning/SceneAssetHostingPlanner.cs` — D4-I1：托管规划生成。只计算路径与规划，不写磁盘。
- `XuanYu.Editor/Assets/Hosting/SceneAssetHostingError.cs` — D4-I1：托管事务错误码。复用 SceneDocumentResult 的 ErrorCode 字符串约定，
- `XuanYu.Editor/Assets/Hosting/SceneAssetHostingState.cs` — D4-I1：托管事务状态机。
- `XuanYu.Editor/Assets/Hosting/SceneAssetPathPolicy.cs` — static class SceneAssetPathPolicy
- `XuanYu.Editor/Assets/Hosting/Transactions/SceneAssetHostingTransaction.Activate.cs` — D4-I1：Activate 将 staging 激活为正式 .xyassets，同时保留旧目录为备份。
- `XuanYu.Editor/Assets/Hosting/Transactions/SceneAssetHostingTransaction.Complete.cs` — D4-I1：Complete 在后续场景文件保存成功后调用，删除备份并收尾。
- `XuanYu.Editor/Assets/Hosting/Transactions/SceneAssetHostingTransaction.Rollback.cs` — D4-I1：Rollback 恢复旧目录。旧数据安全优先于清理整洁。
- `XuanYu.Editor/Assets/Hosting/Transactions/SceneAssetHostingTransaction.cs` — D4-I1：托管资源事务。Prepare 只写 staging；Activate 激活正式 .xyassets 并保留备份；
- `XuanYu.Editor/Assets/Identity/AssetId.cs` — （职责待补）
- `XuanYu.Editor/Assets/Import/Gltf/GlbContainer.cs` — （职责待补）
- `XuanYu.Editor/Assets/Import/Gltf/GlbImportService.cs` — sealed class GlbImportService
- `XuanYu.Editor/Assets/Import/Gltf/GltfAccessorReader.cs` — （职责待补）
- `XuanYu.Editor/Assets/Import/Gltf/GltfCoordinatePolicy.cs` — static class GltfCoordinatePolicy
- `XuanYu.Editor/Assets/Import/Gltf/GltfJsonAccess.cs` — （职责待补）
- `XuanYu.Editor/Assets/Import/Gltf/GltfNodeTransform.cs` — （职责待补）
- `XuanYu.Editor/Assets/Import/Gltf/GltfStaticModelImporter.cs` — （职责待补）
- `XuanYu.Editor/Assets/Import/Gltf/ImportStop.cs` — （职责待补）
- `XuanYu.Editor/Assets/StaticModels/SceneStaticModelBinding.cs` — D3：场景内实体 → 托管资产的最小绑定记录。
- `XuanYu.Editor/Assets/StaticModels/StaticModelAuthoringService.cs` — sealed record StaticModelAuthorResult
- `XuanYu.Editor/Assets/StaticModels/StaticModelBuilder.cs` — （职责待补）
- `XuanYu.Editor/Assets/StaticModels/StaticModelColor.cs` — （职责待补）
- `XuanYu.Editor/Assets/StaticModels/StaticModelData.cs` — sealed record StaticModelData
- `XuanYu.Editor/Assets/StaticModels/StaticModelImportCodes.cs` — enum StaticModelImportErrorCode
- `XuanYu.Editor/Assets/StaticModels/StaticModelImportResult.cs` — sealed record StaticModelImportResult
- `XuanYu.Editor/Assets/StaticModels/StaticModelImportWarning.cs` — sealed record StaticModelImportWarning
- `XuanYu.Editor/Assets/StaticModels/StaticModelPrimitive.cs` — （职责待补）
- `XuanYu.Editor/Assets/StaticModels/StaticModelVertex.cs` — （职责待补）
- `XuanYu.Editor/Camera/CameraBasis.cs` — F3-F2：唯一相机正交基生成器（Editor 相机规则；不进入 Core，不持有 UiVm/Vulkan）。
- `XuanYu.Editor/Camera/CameraFarProjectionDiagnostic.cs` — F1-FAR-SAFE-01：不依赖 ViewProjection 的双精度中心射线与屏幕公制诊断。
- `XuanYu.Editor/Camera/CameraFrameResult.cs` — （职责待补）
- `XuanYu.Editor/Camera/CameraNavigation.Far.cs` — F1-FAR-RECOVERY-01：编辑器相机距离上限与按当前距离回落的 FarPlane 公式。
- `XuanYu.Editor/Camera/CameraNavigation.Try.cs` — F3-F2/F1-FAR：失败安全导航入口，全部透视导航以当前距离重算 FarPlane。
- `XuanYu.Editor/Camera/CameraNavigation.cs` — （职责待补）
- `XuanYu.Editor/Camera/EditorCameraFraming.Orthographic.cs` — F3-F4：正交取景。保持当前正交模式与观察方向，尺度按包围范围适配
- `XuanYu.Editor/Camera/EditorCameraFraming.Draft.cs` — F1-C2：Draft 最小焦距半径的透视取景。
- `XuanYu.Editor/Camera/EditorCameraFraming.MapOrthographic.cs` — F1-C2：地图范围的正交取景，保持 Orthographic 模式。
- `XuanYu.Editor/Camera/EditorCameraFraming.cs` — MAP-A-R1-D4-F4：地图取景使用 45° 斜上方俯视，保证看得到地表内部。
- `XuanYu.Editor/Camera/OrthographicViewFactory.cs` — F3-F4：正交视图生成。六方向标准视图（±X/±Y/±Z）切换为正交投影时，
- `XuanYu.Editor/MapDocument/MapDocument.cs` — MAP-A-R1-D2：地图文档 DTO（.xymap v1 持久化模型）。表达地图文件数据，
- `XuanYu.Editor/MapDocument/MapDocumentAggregateBridge.cs` — MAP-A-R2-D3：.xymap v1 DTO → 领域聚合投影（场景 mapReference 保活链）。
- `XuanYu.Editor/MapDocument/MapDocumentJson.cs` — MAP-A-R1-D2：.xymap v1 严格 JSON 模型。
- `XuanYu.Editor/MapDocument/MapDocumentOwner.cs` — MAP-A-R1-D2：当前地图状态所有者（最小状态机）。
- `XuanYu.Editor/MapDocument/MapDocumentResult.cs` — MAP-A-R1-D2：地图操作结构化结果（对齐 SceneDocumentResult 模式，语义独立）。
- `XuanYu.Editor/MapDocument/MapDocumentValidator.cs` — MAP-A-R1-D2：地图文档 DTO（.xymap v1）严格校验。领域合法性（尺寸范围）单一事实源在 World.MapDefinitionValidator。
- `XuanYu.Editor/MapDocument/MapEnvironmentDefinition.cs` — MAP-A-R1-D2：环境定义。D2 只保存与校验，不渲染。
- `XuanYu.Editor/MapDocument/MapJsonMapper.cs` — MAP-A-R1-D2：MapDocument ↔ MapDocumentJson 双向映射。
- `XuanYu.Editor/MapDocument/MapJsonSerializer.cs` — MAP-A-R1-D2：.xymap 严格 JSON 读写。字段大小写敏感、未知字段拒绝、确定性输出、UTF-8。
- `XuanYu.Editor/MapDocument/MapStorageService.cs` — MAP-A-R1-D2：地图文件存储。候选加载 + 同目录临时文件原子保存，不直接替换任何状态。
- `XuanYu.Editor/MapEditing/MapEditEvents.cs` — MAP-A-R2-D2：地图编辑低频事件参数（禁止记录鼠标移动/Hover/每帧渲染）。
- `XuanYu.Editor/MapEditing/MapEditReason.cs` — MAP-A-R2-D2/D3-A1/D4：地图编辑原因（内容变更事件携带）。
- `XuanYu.Editor/MapEditing/MapEditSession.Regions.cs` — MAP-A-R3-D1：区域正式 Create/Delete 入口，复用地图候选校验、单历史条目与 Undo/Redo 快照恢复。
- `XuanYu.Editor/MapEditing/MapEditSession.ActiveLayer.cs` — MAP-A-R2-D4：活动区域图层（会话临时状态：不进历史、不设 Dirty、不产生内容变更事件）。
- `XuanYu.Editor/MapEditing/MapEditSession.Commands.cs` — MAP-A-R2-D2：地图基础属性编辑命令（D2 只实现地图级修改，图层/区域命令属 D4/D5）。
- `XuanYu.Editor/MapEditing/MapEditSession.Commit.cs` — MAP-A-R2-D2：统一提交管线。所有地图内容修改必须经过本方法：
- `XuanYu.Editor/MapEditing/MapEditSession.Document.cs` — MAP-A-R2-D2：文档生命周期（新建/替换/标记已保存）。
- `XuanYu.Editor/MapEditing/MapEditSession.History.cs` — MAP-A-R2-D2：Undo/Redo 与事件广播。历史游标移动恢复对应 MapDefinition；
- `XuanYu.Editor/MapEditing/MapEditSession.Layers.cs` — MAP-A-R2-D4：图层内容修改命令（走 CommitMapChange：单历史节点、失败零污染）。
- `XuanYu.Editor/MapEditing/MapEditSession.Selection.cs` — MAP-A-R2-D2：选择状态。只保存稳定 ID；选择不产生 Dirty、不写入历史。
- `XuanYu.Editor/MapEditing/MapEditSession.cs` — MAP-A-R2-D2：地图编辑会话（唯一状态权威）。
- `XuanYu.Editor/MapEditing/MapHistoryEntry.cs` — MAP-A-R2-D2：地图历史条目（不可变快照）。MapDefinition 与 ImmutableArray
- `XuanYu.Editor/MapEditing/MapSelection.cs` — MAP-A-R2-D2：地图选择状态。只保存稳定 ID，不保存 UI 控件/列表下标/中文名。
- `XuanYu.Editor/MapEditing/MapSelectionKind.cs` — MAP-A-R2-D2：地图选择类型（未选择/地图/图层/区域）。
- `XuanYu.Editor/SceneDocument/MapReference.cs` — MAP-A-R1-D5-B：场景对地图的可选引用（D1 合同冻结）。
- `XuanYu.Editor/SceneDocument/SceneDocumentAsset.cs` — D4：场景资产记录（D0 合同字段）。只描述托管来源，不含顶点/索引/GPU 数据。
- `XuanYu.Editor/SceneDocument/SceneDocumentEntity.cs` — （职责待补）
- `XuanYu.Editor/SceneDocument/SceneDocumentJson.cs` — （职责待补）
- `XuanYu.Editor/SceneDocument/SceneDocumentLoadTransaction.cs` — D4：加载候选阶段。只读构建候选，不修改当前 World/Catalog/Selection/History/Dirty。
- `XuanYu.Editor/SceneDocument/SceneDocumentMapper.cs` — （职责待补）
- `XuanYu.Editor/SceneDocument/SceneDocumentResult.cs` — sealed record SceneDocumentResult
- `XuanYu.Editor/SceneDocument/SceneDocumentSaveTransaction.cs` — D4：保存完整事务。候选构建 → Hosting Prepare/Activate → 原子写 .xyscene
- `XuanYu.Editor/SceneDocument/SceneDocumentSession.cs` — sealed class SceneDocumentSession
- `XuanYu.Editor/SceneDocument/SceneDocumentSnapshot.cs` — sealed record SceneDocumentSnapshot
- `XuanYu.Editor/SceneDocument/SceneDocumentValidator.MapReference.cs` — MAP-A-R1-D5-B：mapReference 校验（可空；空=旧场景无引用，正常打开）。
- `XuanYu.Editor/SceneDocument/SceneDocumentValidator.cs` — （职责待补）
- `XuanYu.Editor/SceneDocument/SceneDocumentWorldBridge.cs` — static class SceneDocumentWorldBridge
- `XuanYu.Editor/SceneDocument/SceneLoadCandidate.cs` — D4：加载候选。候选阶段构建，提交阶段一次性替换 World/Catalog。
- `XuanYu.Editor/SceneDocument/SceneSaveOutcome.cs` — D4：保存事务结果。SavedSnapshot 带 v3 Assets；HostedSourcePaths 是
- `XuanYu.Editor/SceneDocument/SceneStorageService.cs` — sealed class SceneStorageService
- `XuanYu.Editor/Transform/TransformSession.Rotate.cs` — 旋转起始：与 Begin（移动）互斥，复用同一会话生命周期与提交/取消路径。
- `XuanYu.Editor/Transform/TransformSession.Scale.cs` — 缩放起始：与 Begin（移动）/ BeginRotate（旋转）互斥，复用同一会话生命周期与提交/取消路径。
- `XuanYu.Editor/Transform/TransformSession.cs` — sealed partial class TransformSession
- `XuanYu.Editor/XuanYu.Editor.csproj` — （职责待补）
- `XuanYu.Engine.slnx` — （职责待补）
- `XuanYu.Render.Abstractions/EditorViewPlaneGridKind.cs` — F3-F4：正交标准视图的视图平面网格类型。None=不显示；
- `XuanYu.Render.Abstractions/EditorViewportAssistState.cs` — F3-F1：导航 Gizmo 悬停索引（-1=无；0..5=六个端点）——UI 指针流更新，Overlay Pass 高亮。
- `XuanYu.Render.Abstractions/FrameExecutionPolicy.cs` — R4-R3-R2：Vulkan Present 循环帧执行顺序策略，供 VulkanPresentLoop 实现与测试共同使用。
- `XuanYu.Render.Abstractions/INativeHostSurfaceBridge.cs` — NativeHost 生命周期到 Surface 生命周期的交接契约。
- `XuanYu.Render.Abstractions/INativeHostSurfaceBridgeFactory.cs` — ARCH-A-R1：NativeHost 渲染桥的最小装配契约。
- `XuanYu.Render.Abstractions/IRenderProjectionSource.cs` — interface IRenderProjectionSource
- `XuanYu.Render.Abstractions/MapBoundsGeometry.cs` — MAP-A-R2-D3：地图边界几何——四条边各一条细条四边形（每边 6 顶点 = 2 三角形），
- `XuanYu.Render.Abstractions/MapRenderSnapshot.cs` — MAP-A-R2-D3/D4：地图渲染快照（唯一渲染输入；渲染层/Vulkan 只读，禁止反向访问编辑会话）。
- `XuanYu.Render.Abstractions/MapSurfaceGeometry.cs` — MAP-A-R2-D3：有限 Flat 地面常量几何——固定 4 顶点 / 6 索引（两个三角形），
- `XuanYu.Render.Abstractions/MapSurfaceResourceKey.cs` — MAP-A-R2-D3-A1 收口：GPU 地图资源判等键。
- `XuanYu.Render.Abstractions/MapSurfaceResourceUpdatePolicy.cs` — MAP-A-R2-D3-A1 收口：地图 GPU 资源更新决策（纯策略，不依赖 Vulkan，可独立测试）。
- `XuanYu.Render.Abstractions/MapSurfaceResourceUpdateText.cs` — MAP-A-R2-D3-F2：地图资源更新决策的显示文本（日志中文化）。
- `XuanYu.Render.Abstractions/LatestRenderProjectionQueue.cs` — PointerMoved 高频发布只保留最新 RenderProjection 的线程安全邮箱。
- `XuanYu.Render.Abstractions/RenderVectorOverlayKey.cs` — F1-V1：Vector Overlay 稳定资源键。
- `XuanYu.Render.Abstractions/RenderVectorOverlayPrimitive.cs` — F1-V1：Fill/Stroke/Marker primitive 与 DIP 尺寸合同。
- `XuanYu.Render.Abstractions/RenderVectorOverlayResource.cs` — F1-V1：Vector Overlay 顶点、索引、primitive 与世界包围盒合同。
- `XuanYu.Render.Abstractions/RenderVectorOverlayVertex.cs` — F1-V1：世界端点、Secondary 端点与屏幕偏移顶点合同。
- `XuanYu.Render.Abstractions/NativeHostHandleSnapshot.cs` — VK3-A-R1：从 XuanYu.Render.Vulkan 迁入的纯生命周期快照。
- `XuanYu.Render.Abstractions/NativeHostLifecycleLogFormatter.cs` — VK3-A-R1：从 XuanYu.Render.Vulkan 迁入的纯生命周期日志格式器。
- `XuanYu.Render.Abstractions/NativeHostLifecycleProbe.cs` — VK3-A-R1：从 XuanYu.Render.Vulkan 迁入的纯生命周期探针。
- `XuanYu.Render.Abstractions/NativeHostLifecycleState.cs` — VK3-A-R1：从 XuanYu.Render.Vulkan 迁入的纯生命周期状态枚举。
- `XuanYu.Render.Abstractions/NativeHostSurfaceHandle.cs` — NativeHost 交给渲染层的窗口交接句柄。
- `XuanYu.Render.Abstractions/ReferenceGridFrameState.cs` — GRID-RW-2B：按 1/2/5 和 24~80 DIP 回滞选择的每帧唯一 World Grid Step。
- `XuanYu.Render.Abstractions/ReferenceGridScale.cs` — MAP-A-R3-D2-F1-V2：100m 起步、10,000km 覆盖的 1/2/5 公制参考网格尺度。
- `XuanYu.Render.Abstractions/ScaleIndicatorMetric.cs` — MAP-A-R3-D2-F1-V3：比例尺漂亮距离选择与 m/km 文本格式化。
- `XuanYu.Render.Abstractions/ScaleIndicatorGlyphLite.cs` — OVL-R2：比例尺专用 0-9/m/k/点/空格字符编码。
- `XuanYu.Render.Abstractions/ScaleIndicatorOverlayProjection.cs` — OVL-R2：比例尺可见性、标签与 DIP 宽度渲染投影 DTO。
- `XuanYu.Render.Abstractions/ViewportMetricScale.cs` — 计算视口 X/Y 方向公制尺度；不可逆 VP 时返回失败而不抛异常。
- `XuanYu.Render.Abstractions/RenderCameraProjection.cs` — （职责待补）
- `XuanYu.Render.Abstractions/RenderDrawPlan.Typed.cs` — R4-R3-R2：实体绘制计划提取（typed 部分），供 Vulkan 与测试共同使用。
- `XuanYu.Render.Abstractions/RenderDrawPlan.cs` — R4-R3-R2：实体绘制计划提取（帧级），供 Vulkan 与测试共同使用。
- `XuanYu.Render.Abstractions/RenderEntityProjection.cs` — （职责待补）
- `XuanYu.Render.Abstractions/RenderEntityType.cs` — enum RenderEntityType
- `XuanYu.Render.Abstractions/RenderProjection.cs` — 渲染帧投影快照，携带相机、观察中心与各类渲染资源。
- `XuanYu.Render.Abstractions/RenderProjectionResult.cs` — （职责待补）
- `XuanYu.Render.Abstractions/ViewportOverlayAnchor.cs` — OVL-R1：Viewport Overlay Anchor、Rect 与布局请求纯合同。
- `XuanYu.Render.Abstractions/ViewportOverlayLayoutResolver.cs` — OVL-R1：DIP-only Overlay Rect 解析与视口边界钳制。
- `XuanYu.Render.Abstractions/RenderStaticModelKey.cs` — （职责待补）
- `XuanYu.Render.Abstractions/RenderStaticModelPrimitive.cs` — （职责待补）
- `XuanYu.Render.Abstractions/RenderStaticModelTransform.cs` — 静态模型位置、旋转与缩放变换合同，提供单位变换。
- `XuanYu.Render.Abstractions/RenderStaticModelResource.cs` — sealed record RenderStaticModelResource
- `XuanYu.Render.Abstractions/RenderStaticModelVertex.cs` — （职责待补）
- `XuanYu.Render.Abstractions/XuanYu.Render.Abstractions.csproj` — （职责待补）
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgeDeviceAttachStep.cs` — VK4-B：在 VK4-A 物理设备选择成功后，基于其选择结果创建 LogicalDevice（VkDevice + 队列）。
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgePhysicalDeviceAttachStep.cs` — VK4-A-R1：将 Attach 后的 PhysicalDevice 选择与中文日志从 VulkanNativeHostSurfaceBridge 迁出，
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgeRenderSessionAttachStep.cs` — VK4-D：把 RenderSession 创建从 Bridge 抽离，Bridge 只委托，不内联 VK4-D 细节。
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgeSwapchainAttachStep.cs` — VK4-C：在设备 step 之后链式驱动 Swapchain 创建（Swapchain + Images + ImageViews）。
- `XuanYu.Render.Vulkan/Device/VulkanDeviceOwner.Physical.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Device/VulkanDeviceOwner.cs` — VK4-B：LogicalDevice 持有者。基于 VK4-A 的 VulkanPhysicalDeviceSelection 创建 VkDevice 与队列。
- `XuanYu.Render.Vulkan/Device/VulkanPhysicalDeviceInfo.cs` — VK4-A：纯数据物理设备信息。仅描述候选设备，不持有任何 Vulkan 句柄（VkPhysicalDevice 不外露）。
- `XuanYu.Render.Vulkan/Device/VulkanPhysicalDeviceSelection.cs` — VK4-A：物理设备选择结果（纯数据，渲染层）。Success 为 true 时 Handle / Device / Queue 非空。
- `XuanYu.Render.Vulkan/Device/VulkanPhysicalDeviceSelector.cs` — VK4-A：物理设备选择器。在已有 Instance + Surface 前提下枚举并选择可用于渲染/呈现的设备。
- `XuanYu.Render.Vulkan/Device/VulkanQueueFamilySelection.cs` — VK4-A：纯数据队列族选择结果。索引为 -1 表示未找到对应能力。
- `XuanYu.Render.Vulkan/Diagnostic/VulkanResizeTracer.cs` — RZ-VK5-D-R1：Resize / Present 慢半拍全链路诊断追踪器。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.Frag.cs` — MAP-A-R1-D5-R1-F2-R3-R2: generated by glslc -O from scene.frag.
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.GridLineFrag.cs` — GRID-RW-1：由世界线片元 GLSL 生成的 SPIR-V 字节码。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.GridLineVert.cs` — GRID-RW-1：由世界线顶点 GLSL 生成的 SPIR-V 字节码。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.GridVert.cs` — MAP-A-R1-D5-R1-F2-R2: generated by glslc -O from editor_reference_grid.vert.
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.WorldReferenceGridFrag.cs` — GRID-RW-2A：独立 World XY 固定网格片元 GLSL 的 SPIR-V 字节码。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.NavGizmoFrag.cs` — MAP-A: generated by glslc -O from navgizmo_frag.spv.
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.NavGizmoVert.cs` — AUTO-GENERATED from editor_nav_gizmo.vert / editor_nav_gizmo.frag / editor_world_origin.frag (glslc -O)
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.ScaleIndicatorFrag.cs` — OVL-R2：由 glslc -O 生成的比例尺片元 SPIR-V。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.Vert.cs` — STAB-4C：由 glslc -O 从 scene.vert 生成的直接 ViewProjection 字节码。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.ViewPlaneGridFrag.cs` — F3-F4: generated by glslc -O from editor_view_plane_grid.frag.
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.WorldAxesFrag.cs` — MAP-A-R1-D5-R1-F2-R2: generated by glslc -O from editor_world_axes.frag.
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.WorldOriginFrag.cs` — AUTO-GENERATED from editor_nav_gizmo.vert / editor_nav_gizmo.frag / editor_world_origin.frag (glslc -O)
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Depth.cs` — STAB-3：主场景与 Vector Overlay 可分别配置深度测试/写入策略。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Fullscreen.cs` — 程序化 Pass 管线创建，支持全屏三角形与 LineList 拓扑。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Grid.cs` — GRID-RW-2A：World Grid 全屏 Pass 入口；关闭深度测试，旧 GridLine 不再是正式入口。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.GridLine.cs` — GRID-RW-1-CORR2：参考网格专用 Empty-input LineList 管线（无顶点绑定、负 Depth Bias）。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Sky.cs` — WORLD-D-R1：天空专用管线。与主管线共用 Shader、顶点输入与 RenderPass，
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.StaticModelInput.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Pipeline/VulkanPipelineLogFormatter.cs` — VK5-A：GraphicsPipeline 资源中文日志格式器。仅生成字符串，经注入的 Action<string> log 回调输出（日志单出口）。
- `XuanYu.Render.Vulkan/Pipeline/VulkanScenePushConstants.cs` — std140 布局：
- `XuanYu.Render.Vulkan/Pipeline/VulkanShaderModuleOwner.cs` — VK5-A：ShaderModule 创建助手。创建后由 GraphicsPipelineOwner 在管道建好后立即释放（短生命周期，不持有到会话结束）。
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameLogFormatter.cs` — VK4-D：单色清屏日志格式化（统一经 Bridge 的 Emit 单出口）。
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Commands.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Lifecycle.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Matrix.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.PipelineBind.cs` — STAB-3：按绘制类型绑定主场景、Vector Overlay 与全屏 Pass 管线。
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.VectorOverlayPipeline.cs` — STAB-3：持有独立 Vector Overlay 管线并在命令缓冲重录时注入。
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.PushConstants.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Resources.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Trace.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/Grid/VulkanClearFrameOwner.Grid.cs` — GRID-RW-2B：以全屏三角形绘制帧级统一 Step 的 World XY 网格。
- `XuanYu.Render.Vulkan/Render/Grid/VulkanClearFrameOwner.GridScale.cs` — GRID-RW-2A：网格公制计算固定消费 World XY 的 Z=0 平面。
- `XuanYu.Render.Vulkan/Render/Grid/VulkanClearFrameOwner.NavGizmo.cs` — MAP-A-R1-D5-R1-F3-F1：导航 Gizmo Overlay Pass —— 屏幕空间、深度测试/写入关闭、最后绘制。
- `XuanYu.Render.Vulkan/Render/Grid/VulkanClearFrameOwner.ScaleIndicator.cs` — OVL-R2：解析 BottomLeft LayoutRect、编码 glyph 并绘制比例尺 Overlay。
- `XuanYu.Render.Vulkan/Render/Grid/VulkanClearFrameOwner.ViewPlaneGrid.cs` — F3-F4：正交标准视图的视图平面网格绘制（±X→YZ / ±Y→XZ，以世界原点为基准）。
- `XuanYu.Render.Vulkan/Render/Grid/VulkanClearFrameOwner.WorldAxes.cs` — MAP-A-R1-D5-R1-F2-R2：世界轴 / 世界原点独立全屏 Pass。
- `XuanYu.Render.Vulkan/Render/Map/VulkanClearFrameOwner.MapSurface.cs` — MAP-A-R2-D3：有限 Flat 地面（4 顶点 6 索引）+ 四条边界（24 顶点细条）；资源判等用 ResourceKey（Rename 不重建）。
- `XuanYu.Render.Vulkan/Render/Present/VulkanPresentLoop.Frame.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/Present/VulkanPresentLoop.Lifecycle.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/Present/VulkanPresentLoop.cs` — VK-LIFE-1：Present 泵必须确认停止成功后，才允许释放同步对象。
- `XuanYu.Render.Vulkan/Render/Scene/VulkanClearFrameOwner.Draw.cs` — GRID-DIAG-GROUND-01：在管线绑定前暂时跳过 MapGround，隔离地面绘制与深度写入供真机诊断。
- `XuanYu.Render.Vulkan/Render/Scene/VulkanClearFrameOwner.DrawAssist.cs` — D4：地图地面/边界已改由 Draw.cs 按 MapGround/MapBounds 分项分发；
- `XuanYu.Render.Vulkan/Render/Scene/VulkanClearFrameOwner.DrawGizmo.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/Scene/VulkanClearFrameOwner.Scene.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/StaticModels/VulkanClearFrameOwner.DrawStaticBounds.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/StaticModels/VulkanClearFrameOwner.DrawStaticModel.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/StaticModels/VulkanStaticModelBuffer.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/StaticModels/VulkanStaticModelCache.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/StaticModels/VulkanStaticModelFailureTracker.cs` — D3-F1：静态模型 GPU 资源创建失败去重。
- `XuanYu.Render.Vulkan/Render/StaticModels/VulkanStaticModelLog.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/StaticModels/VulkanStaticModelResource.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/StaticModels/VulkanStaticModelValidator.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/StaticModels/VulkanStaticModelVertex.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Render/VectorOverlay/VulkanClearFrameOwner.DrawVectorOverlay.cs` — F1-V1/STAB-4C：Vector Overlay Vulkan Pass 绘制、DIP 尺寸与 primitive 模式注入。
- `XuanYu.Render.Vulkan/Render/VectorOverlay/VulkanVectorOverlayBufferReusePolicy.cs` — F1-V1：动态缓冲容量复用纯策略。
- `XuanYu.Render.Vulkan/Render/VectorOverlay/VulkanVectorOverlayCache.cs` — F1-V1：GPU 资源缓存与容量足够时缓冲复用。
- `XuanYu.Render.Vulkan/Render/VectorOverlay/VulkanVectorOverlayResource.cs` — F1-V1：Vector Overlay GPU 资源持有者。
- `XuanYu.Render.Vulkan/Render/VectorOverlay/VulkanVectorOverlayValidator.cs` — F1-V1：索引与 primitive 范围校验。
- `XuanYu.Render.Vulkan/Render/VectorOverlay/VulkanVectorOverlayVertex.cs` — F1-V1：端点/Secondary/屏幕偏移顶点格式。
- `XuanYu.Render.Vulkan/Render/VulkanDepthAttachment.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Session/GridPipelineSet.cs` — 全屏 Pass 管线组合（网格、轴、原点、比例尺、Navigation Gizmo）。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Lifecycle.cs` — sealed partial class VulkanRenderSession
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Recover.cs` — sealed partial class VulkanRenderSession
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Resize.cs` — sealed partial class VulkanRenderSession
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.VectorOverlay.cs` — STAB-3：创建并挂接无深度测试/无深度写入的 Vector Overlay 管线。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` — VK-LIFE-1：组合根负责失败回滚，不把半初始化资源留给 Bridge。
- `XuanYu.Render.Vulkan/Shaders/editor_nav_gizmo.frag` — 玄域编辑器：Blender 风格导航 Gizmo
- `XuanYu.Render.Vulkan/Shaders/editor_nav_gizmo.vert` — MAP-A-R1-D5-R1-F3-F1：导航 Gizmo Overlay Pass —— 顶点着色器。
- `XuanYu.Render.Vulkan/Shaders/editor_scale_indicator.frag` — OVL-R2：screen-space 比例尺 bar/tick、背景与 GlyphLite 片元绘制。
- `XuanYu.Render.Vulkan/Shaders/editor_reference_grid_line.frag` — GRID-RW-1：固定颜色与 Alpha 的世界线片元 Shader。
- `XuanYu.Render.Vulkan/Shaders/editor_reference_grid_line.vert` — GRID-RW-1：按 gl_VertexIndex 生成相机吸附世界线的顶点 Shader。
- `XuanYu.Render.Vulkan/Shaders/editor_reference_grid.vert` — MAP-A-R1-D5-R1-F2：独立编辑器参考网格 Pass —— 顶点着色器。
- `XuanYu.Render.Vulkan/Shaders/editor_world_reference_grid.frag` — GRID-RW-2B：世界射线与 Z=0 平面求交、CPU 全帧 Step、fwidth 仅抗锯齿。
- `XuanYu.Render.Vulkan/Shaders/editor_view_plane_grid.frag` — F3-F4：正交标准视图的视图平面网格（YZ/XZ 平面，以世界原点为基准）。
- `XuanYu.Render.Vulkan/Shaders/editor_world_axes.frag` — MAP-A-R1-D5-R1-F2-R2：X/Y 世界轴独立全屏 Pass —— 片元着色器。
- `XuanYu.Render.Vulkan/Shaders/editor_world_origin.frag` — MAP-A-R1-D5-R1-F3-F1：世界原点标记独立全屏 Pass —— 片元着色器（屏幕空间版）。
- `XuanYu.Render.Vulkan/Shaders/scene.frag` — MAP-A-R1-D5-R1-F2-R3-R2：每像素程序化编辑器环境（天空 + 中性灰参考地面）。
- `XuanYu.Render.Vulkan/Shaders/scene.vert` — F1-REWORK-B2：Vector Overlay 世界坐标保持不变，按 primitive 在裁剪空间建立有界深度层级。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainBuilder.cs` — VK4-C：Swapchain 构建细节（创建 Swapchain + 取 Images + 建 ImageViews）。纯逻辑，不持有状态。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainCapabilities.cs` — VK4-C：Swapchain 能力查询（纯数据，不创建 Swapchain）。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainLogFormatter.cs` — VK4-C：Swapchain 中文生命周期日志格式器。纯文本，无副作用。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.Accessors.cs` — （职责待补）
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs` — VK4-C：Swapchain 持有者（创建/重建/释放）。RZ-VK5-D-R1：Recreate 内部加 T+ 阶段日志。
- `XuanYu.Render.Vulkan/VulkanApiProbe.cs` — （职责待补）
- `XuanYu.Render.Vulkan/VulkanBridgeLogFormatter.cs` — VK3-C1/C2-R1：NativeHost → Instance+Surface 桥接中文生命周期日志格式器。纯文本，无副作用。
- `XuanYu.Render.Vulkan/VulkanDeviceInfo.cs` — sealed record VulkanDeviceInfo
- `XuanYu.Render.Vulkan/VulkanInstanceCreateInfoBuilder.cs` — VK3-B1：Instance 创建信息构造辅助。仅构造 InstanceCreateInfo（含最小扩展集），不直接调用 Vulkan。
- `XuanYu.Render.Vulkan/VulkanInstanceExtensions.cs` — VK3-B1：Instance 启用的最小扩展名集合（仅 surface 相关，以 null 结尾字节序列）。
- `XuanYu.Render.Vulkan/VulkanInstanceLogFormatter.cs` — VK3-B1：Vulkan Instance 生命周期中文日志格式器。纯文本生成，无副作用。
- `XuanYu.Render.Vulkan/VulkanInstanceOwner.cs` — VK3-B1 / C1-R2：Vulkan Instance 持有者。仅创建/释放 Instance，启用 VK_KHR_surface 与 VK_KHR_win32_surface。
- `XuanYu.Render.Vulkan/VulkanInstanceResult.cs` — VK3-B1：Vulkan Instance 创建结果。Owner 非空表示创建成功。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.Attach.cs` — sealed partial class VulkanNativeHostSurfaceBridge
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.Lifecycle.cs` — sealed partial class VulkanNativeHostSurfaceBridge
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.Resize.cs` — sealed partial class VulkanNativeHostSurfaceBridge
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.Scene.cs` — sealed partial class VulkanNativeHostSurfaceBridge
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.cs` — VK-LIFE-1：Attach 全成功后才写入字段；失败按现有释放顺序回滚。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridgeFactory.cs` — ARCH-A-R1：Vulkan 侧开始适配抽象装配契约。
- `XuanYu.Render.Vulkan/VulkanProbeLogFormatter.cs` — static class VulkanProbeLogFormatter
- `XuanYu.Render.Vulkan/VulkanProbeResult.cs` — sealed record VulkanProbeResult
- `XuanYu.Render.Vulkan/VulkanSurfaceLogFormatter.cs` — VK3-B2：Vulkan Surface 生命周期中文日志格式器。纯文本生成，无副作用。
- `XuanYu.Render.Vulkan/VulkanSurfaceOwner.cs` — VK3-B2 / C1-R2：Vulkan Surface 持有者。仅创建/释放 VkSurfaceKHR（Win32），
- `XuanYu.Render.Vulkan/VulkanSurfaceResult.cs` — VK3-B2：Vulkan Surface 创建结果。Owner 非空表示创建成功。
- `XuanYu.Render.Vulkan/XuanYu.Render.Vulkan.csproj` — （职责待补）
- `XuanYu.WarCore.Tests/Identity/MilitaryIdentityTests.cs` — WARCORE-A-R1-D1：身份生成与校验契约测试。
- `XuanYu.WarCore.Tests/State/SoldierStateTests.cs` — WARCORE-A-R1-D1：士兵状态边界与隔离契约测试。
- `XuanYu.WarCore.Tests/WarCoreDependencyTests.cs` — WARCORE-A-R1-D1：WarCore 程序集依赖方向契约测试。
- `XuanYu.WarCore.Tests/XuanYu.WarCore.Tests.csproj` — （职责待补）
- `XuanYu.WarCore/Identity/FactionId.cs` — / <summary>
- `XuanYu.WarCore/Identity/MilitaryIdentity.cs` — / <summary>
- `XuanYu.WarCore/Identity/OrganizationId.cs` — / <summary>
- `XuanYu.WarCore/Identity/UnitId.cs` — / <summary>
- `XuanYu.WarCore/Identity/UnitKind.cs` — / <summary>
- `XuanYu.WarCore/State/SoldierState.cs` — / <summary>
- `XuanYu.WarCore/XuanYu.WarCore.csproj` — （职责待补）
- `XuanYu.World.Tests/Assets/AssetContractTests.cs` — sealed class AssetContractTests
- `XuanYu.World.Tests/Assets/AssetDialogTests.cs` — sealed class AssetDialogTests
- `XuanYu.World.Tests/Assets/GlbFactory.cs` — （职责待补）
- `XuanYu.World.Tests/Assets/GlbImportTests.cs` — sealed class GlbImportTests
- `XuanYu.World.Tests/Assets/GlbMultiPrimitiveFactory.cs` — D3-F1：确定性多 Primitive / 越界索引测试 GLB 工厂（5+100 拆分自 GlbFactory）。
- `XuanYu.World.Tests/Assets/HostingCompleteTests.cs` — sealed class HostingCompleteTests
- `XuanYu.World.Tests/Assets/HostingPlannerRejectTests.cs` — sealed class HostingPlannerRejectTests
- `XuanYu.World.Tests/Assets/HostingPlannerTests.cs` — sealed class HostingPlannerTests
- `XuanYu.World.Tests/Assets/HostingRollbackTests.cs` — sealed class HostingRollbackTests
- `XuanYu.World.Tests/Assets/HostingSaveAsTests.cs` — sealed class HostingSaveAsTests
- `XuanYu.World.Tests/Assets/HostingTestEnv.cs` — D4-I1：托管事务测试辅助。所有测试使用独立临时目录，测试结束清理；
- `XuanYu.World.Tests/Assets/HostingTransactionTests.cs` — sealed class HostingTransactionTests
- `XuanYu.World.Tests/Assets/LoadStructureErrorTests.cs` — D4：结构错误事务（拆分自 LoadTransactionTests，5+100）。
- `XuanYu.World.Tests/Assets/LoadTransactionTests.cs` — sealed class LoadTransactionTests
- `XuanYu.World.Tests/Assets/SaveAsTests.cs` — D4：另存为与重复保存（拆分自 SaveTransactionTests，5+100）。
- `XuanYu.World.Tests/Assets/SaveTransactionTests.cs` — sealed class SaveTransactionTests
- `XuanYu.World.Tests/Assets/ScenePersistenceEnv.cs` — D4 测试辅助：独立临时目录 + 保存/加载事务 + Fake Dialog 计数。
- `XuanYu.World.Tests/Assets/SchemaCompatibilityTests.cs` — sealed class SchemaCompatibilityTests
- `XuanYu.World.Tests/Assets/StaticModelAuthoringServiceTests.cs` — sealed class StaticModelAuthoringServiceTests
- `XuanYu.World.Tests/Assets/StaticModelBaseVertexTests.cs` — sealed class StaticModelBaseVertexTests
- `XuanYu.World.Tests/Assets/StaticModelCatalogTests.cs` — 确定性 AssetId，保证字典序固定：…00 < …01。
- `XuanYu.World.Tests/Assets/StaticModelFailureTrackerTests.cs` — sealed class StaticModelFailureTrackerTests
- `XuanYu.World.Tests/Assets/StaticModelProjectionTests.cs` — sealed class StaticModelProjectionTests
- `XuanYu.World.Tests/Assets/StaticModelUiTests.cs` — sealed class StaticModelUiTests
- `XuanYu.World.Tests/Assets/StaticModelValidatorTests.cs` — SharpGLTF 边界会先拒绝索引越界 GLB（ParserFailure）；
- `XuanYu.World.Tests/Camera/CameraDocumentTests.cs` — sealed class CameraDocumentTests
- `XuanYu.World.Tests/Camera/CameraFramingOccupancyTests.cs` — MAP-A-R1-D5-R1：地图取景屏幕占用率（65%~75%）。
- `XuanYu.World.Tests/Camera/CameraFramingTests.cs` — sealed class CameraFramingTests
- `XuanYu.World.Tests/Camera/CameraNavigationUiTests.cs` — sealed class CameraNavigationUiTests
- `XuanYu.World.Tests/Camera/CameraNavigationUiTests.Focus.cs` — 无选中实体时聚焦保持相机与观察中心不变的回归测试
- `XuanYu.World.Tests/Camera/CameraC2MapFramingTests.cs` — F1-C2：地图查看全部、无实体、投影模式与往返取景回归。
- `XuanYu.World.Tests/Camera/CameraC2MapFramingTests.Helpers.cs` — F1-C2：地图范围、Draft 构造与相机有限性测试辅助。
- `XuanYu.World.Tests/Camera/CameraC2DraftFramingTests.cs` — F1-C2：Draft 三点/一点聚焦及 PointerMoved 稳定性回归。
- `XuanYu.World.Tests/Viewport/NativePointerRoutePolicyTests.cs` — F1-C2 REWORK：MiddleDown/Move/Shift+Middle/Up 与 Region Preview 互斥路由回归。
- `XuanYu.World.Tests/Camera/UiViewGizmoTests.cs` — EDITOR-VIEW-R1：视角 Gizmo 六方向相机命令——朝向正确、观察中心与距离保持。
- `XuanYu.World.Tests/Logging/FootAxamlTailContractTests.cs` — MAP-A-R2-D3-F3：源码合同——AXAML 尾部安全区与控制器两阶段定位结构。
- `XuanYu.World.Tests/Logging/LogAutoScrollPolicyTests.cs` — MAP-A-R2-D3-F2：日志自动跟随纯策略——底部附近跟随、远离不强制拉回、滚到底恢复。
- `XuanYu.World.Tests/Logging/LogListAutoScrollControllerContractTests.cs` — MAP-A-R2-D3-F3：控制器源码合同——两阶段尾项定位结构与副作用禁令。
- `XuanYu.World.Tests/Logging/UiMapLogChineseTests.cs` — MAP-A-R2-D3-F2：日志中文化——字段名/状态值中文，内部枚举保持英文。
- `XuanYu.World.Tests/Logging/UiRootLogRowContractTests.cs` — MAP-A-R2-D3-F4：日志区垂直尺寸自适应源码合同。
- `XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.Behavior.cs` — MAP-A-R2-D4：图层命令会话行为（T02 默认活动图层 + H07～H10、活动转移、No-op）。
- `XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.Drag.History.cs` — MAP-A-R2-D4-F3：拖动排序会话行为（H04 No-op / H05 失败零污染 / H06 活动图层保持）。
- `XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.Drag.cs` — MAP-A-R2-D4-F3：拖动排序会话命令（H01～H03 单历史节点与 Undo/Redo）。
- `XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.cs` — MAP-A-R2-D4：图层命令接入 MapEditSession（H01～H06 撤销/重做；T02 见 Behavior）。
- `XuanYu.World.Tests/Map/Editing/UiLayerStateFeedbackTests.cs` — 状态图标消费与插入线反馈合同（A-D/E）
- `XuanYu.World.Tests/Map/Editing/UiLayerVisualContractTests.cs` — MAP-A-R2-D4-F3：图层视觉合同（V01～V06，源码合同模式）——状态样式/类型标签/热区/字号层级。
- `XuanYu.World.Tests/Map/Editing/UiLogSummaryPriorityTests.cs` — MAP-A-R2-D4-F3：底部通知优先级（L01/L02/L04/L05）——Error/Warning > Editor 动作 > Render 兜底。
- `XuanYu.World.Tests/Map/Editing/UiLogSummaryTimingTests.cs` — 通知时序（F/G/H）
- `XuanYu.World.Tests/Map/Editing/UiMapCommandRoutingTests.cs` — MAP-A-R2-D3-F1：真实按钮链测试（RunCommand.Execute → MapSession）。
- `XuanYu.World.Tests/Map/Editing/UiMapEditorTests.cs` — MAP-A-R2-D3：地图属性入口——会话恒有默认地图、应用修改、非法输入保护、取景数据源。
- `XuanYu.World.Tests/Map/Editing/UiMapHistoryTests.cs` — MAP-A-R2-D3-A1 入口补接：地图撤销/重做按钮路由到 MapSession 独立历史。
- `XuanYu.World.Tests/Map/Editing/UiMapInitialProjectionTests.cs` — MAP-A-R2-D3-A1：默认地图初始快照进入首帧 RenderProjection（无需新建地图）。
- `XuanYu.World.Tests/Map/Editing/UiMapLayerDragTests.cs` — MAP-A-R2-D4-F3：区域图层拖动 UI 入口（U01～U06 + L03 通知）。
- `XuanYu.World.Tests/Map/Editing/UiMapLayerLockLogTests.cs` — MAP-A-R2-D4-F2：图层锁定日志细化（L01～L06）+ 添加立方体单次创建（C01）。
- `XuanYu.World.Tests/Map/Editing/UiMapLayerPanelTests.Behavior.cs` — MAP-A-R2-D4：图层面板行为——显隐/锁定/删除/排序/活动图层/撤销重做（真实命令链）。
- `XuanYu.World.Tests/Map/Editing/UiMapLayerPanelTests.cs` — MAP-A-R2-D4：图层面板 ViewModel——默认列表/添加/按钮状态/系统层只读/重命名（真实命令链）。
- `XuanYu.World.Tests/Map/Editing/UiMapLayoutContractTests.cs` — MAP-A-R2-D4-F1：图层 UI 归位合同——左侧仅项目/层级，图层管理迁入右侧地图编辑器二级页。
- `XuanYu.World.Tests/Map/MapBoundsTests.cs` — MAP-A-R2-D1：有限地图边界合同（中心原点、闭区间、尺寸变化同步）。
- `XuanYu.World.Tests/Map/MapCoordinateValidationTests.cs` — MAP-A-R1-D2：坐标合同 / 图层引用 / schema / 名称校验。
- `XuanYu.World.Tests/Map/MapDefaultMapTests.cs` — MAP-A-R2-D1-F1：默认地图工厂合同（完整聚合 + DTO 默认值一致）。
- `XuanYu.World.Tests/Map/MapDefinitionTests.cs` — MAP-A-R2-D1-F1：地图聚合验证（尺寸/坐标/地表/图层/区域组合入口）。
- `XuanYu.World.Tests/Map/MapDocumentAggregateBridgeTests.cs` — MAP-A-R2-D3：.xymap v1 DTO → 领域聚合桥接（场景 mapReference 保活链）与端到端查询一致。
- `XuanYu.World.Tests/Map/MapDocumentOwnerChainTests.cs` — MAP-A-R1-D2：状态链闭环与失败不污染。
- `XuanYu.World.Tests/Map/MapDocumentOwnerTests.cs` — MAP-A-R1-D2：当前地图状态所有者（New/Load/Modify/Save/Unload 基础状态）。
- `XuanYu.World.Tests/Map/MapEnvironmentValidationTests.cs` — MAP-A-R1-D2：环境定义与参数校验。
- `XuanYu.World.Tests/Map/MapIdTests.cs` — MAP-A-R1-D2：MapId 与地图合同校验（纯内存）。
- `XuanYu.World.Tests/Map/MapJsonRoundTripTests.cs` — MAP-A-R1-D2：.xymap 严格 JSON Round-trip 与确定性。
- `XuanYu.World.Tests/Map/MapJsonStrictnessTests.cs` — MAP-A-R1-D2：严格 JSON 拒绝路径（大小写 / 未知字段 / 类型 / 损坏）。
- `XuanYu.World.Tests/Map/MapLayerRulesTests.cs` — MAP-A-R2-D4：图层操作规则（T05 名称校验、T06 系统层保护、T07/T08 删除保护、T04 自动命名）。
- `XuanYu.World.Tests/Map/MapLayerStackTests.Drag.cs` — MAP-A-R2-D4-F3：区域图层拖动排序领域合同（T01～T08）。
- `XuanYu.World.Tests/Map/MapLayerStackTests.Order.cs` — MAP-A-R2-D4：图层顺序边界与状态操作（T10 系统层顺序固定、显隐/锁定/改名保身份）。
- `XuanYu.World.Tests/Map/MapLayerStackTests.cs` — MAP-A-R2-D4：图层顺序与状态操作（T03/T09 区域层内排序、T11/T12 显隐锁定保身份）。
- `XuanYu.World.Tests/Map/MapLayerTests.Base.cs` — MAP-A-R2-D4：系统图层合同（地面层恰好一个 Order 0、边界层恰好一个 Order 1、区域层 Order ≥ 2）。
- `XuanYu.World.Tests/Map/MapLayerTests.cs` — MAP-A-R2-D4：图层领域模型与验证（默认图层/稳定 ID/唯一性）。
- `XuanYu.World.Tests/Map/MapRegionDraftTests.cs` — MAP-A-R2-D1-F1：绘制草稿合同（未闭合草稿 → 提交为天然闭合正式区域）。
- `XuanYu.World.Tests/Map/MapRegionTests.Helpers.cs` — sealed partial class MapRegionTests
- `XuanYu.World.Tests/Map/MapRegionTests.Strictness.cs` — MAP-A-R2-D1-F1：区域严格性（相邻重复点/首尾规则/三不同顶点/非零面积）。
- `XuanYu.World.Tests/Map/MapRegionTests.cs` — MAP-A-R2-D1：区域验证（闭合/顶点数/引用图层/边界/有限数值）。
- `XuanYu.World.Tests/Map/MapRegionTests.Geometry.cs` — MAP-A-R3-D1：Simple Polygon 自相交、非相邻 touch 与 overlap 拒绝合同。
- `XuanYu.World.Tests/Map/MapSizeValidationTests.cs` — MAP-A-R1-D2：地图尺寸与坐标合同校验。
- `XuanYu.World.Tests/Map/MapStorageFailureTests.cs` — MAP-A-R1-D2：加载失败保护 / 非法合同拒绝 / 保存失败不写坏文件。
- `XuanYu.World.Tests/Map/MapStorageTests.cs` — MAP-A-R1-D2：候选加载 / 原子保存（真实文件，临时目录）。
- `XuanYu.World.Tests/Map/MapSurfaceSamplerTests.cs` — MAP-A-R1-D3：唯一地表采样器——确定性、范围与参数语义。
- `XuanYu.World.Tests/Map/MapSurfaceValidationTests.cs` — MAP-A-R1-D2：地表定义与参数校验。
- `XuanYu.World.Tests/Map/SceneMapReferenceTests.cs` — MAP-A-R1-D5-B（D3 适配）：.xyscene mapReference 闭环——保存携带、打开恢复、缺失失效、旧场景兼容。
- `XuanYu.World.Tests/Map/WorldMapStateOwnerTests.cs` — MAP-A-R1-D3：World 地图状态所有者——加载/切换/卸载/查询/渲染快照。
- `XuanYu.World.Tests/Map/WorldMapStateTests.cs` — MAP-A-R1-D3：World 地图状态——有限边界（闭区间）与高度查询。
- `XuanYu.World.Tests/MapEditing/MapEditSessionCommandTests.cs` — MAP-A-R2-D2：地图基础编辑命令（改名/尺寸/基础高度/No-op/非法输入）。
- `XuanYu.World.Tests/MapEditing/MapPickingRoundTripTests.cs` — MAP-A-R3-D2-F1 Metric/Picking：100m、10km、10,000km 与多 DPI/斜视下 Screen → Pick → World → Screen 误差不超过 1 DIP。
- `XuanYu.World.Tests/MapEditing/MapEditSessionRegionTests.cs` — MAP-A-R3-D1：Region Create/Delete 单历史条目及相同 ID Undo/Redo 合同。
- `XuanYu.World.Tests/MapEditing/MapEditSessionCreationTests.cs` — MAP-A-R2-D2：默认会话与根状态合同。
- `XuanYu.World.Tests/MapEditing/MapEditSessionDirtyTests.cs` — MAP-A-R2-D2：Saved/Dirty 合同（Dirty 随 Undo/Redo 回到保存点）。
- `XuanYu.World.Tests/MapEditing/MapEditSessionHistoryTests.cs` — MAP-A-R2-D2：Undo/Redo、分支清除与 ChangeSequence 单调递增。
- `XuanYu.World.Tests/MapEditing/MapEditSessionMapPropertiesTests.cs` — MAP-A-R2-D3-A1：地图属性原子提交（单历史节点/失败零污染）。
- `XuanYu.World.Tests/MapEditing/MapEditSessionSelectionTests.cs` — MAP-A-R2-D2：选择状态（稳定 ID/存在性/不产生 Dirty/规范化）。
- `XuanYu.World.Tests/MapEditing/MapEditSessionThreadTests.cs` — MAP-A-R2-D2：写线程保护（非法线程拒绝且状态完全不变）。
- `XuanYu.World.Tests/MapEditing/MapEditSessionValidationTests.cs` — MAP-A-R2-D2：候选校验与失败不污染（缩小越界整体拒绝/无效替换拒绝）。
- `XuanYu.World.Tests/MapEditing/MapRenderSnapshotProjectionTests.cs` — MAP-A-R2-D3：MapDefinition → MapRenderSnapshot 投影合同（渲染唯一输入）。
- `XuanYu.World.Tests/Render/WorldGridIndependenceContractTests.cs` — GRID-RW-2A：锁定 MapGround 恢复与 World Grid 的 Z=0 独立性。
- `XuanYu.World.Tests/Render/VulkanPresentLoopContractTests.cs` — VK-PERF-R1：Present 循环合同测试——防性能轮回归：
- `XuanYu.World.Tests/Render/VulkanPresentModeSelectionTests.cs` — VK-PERF-R1：Present Mode 选择合同——FIFO（垂直同步）为首选，Mailbox 不再是默认。
- `XuanYu.World.Tests/Scene/CommandSmokeTests.cs` — sealed class CommandSmokeTests
- `XuanYu.World.Tests/Scene/EditorEnvironmentTests.cs` — WORLD-D-R1：编辑器环境（天空/光照）契约测试。
- `XuanYu.World.Tests/Scene/EntityBoundsSemanticsTests.cs` — R2-R1 final patch: lock the two spatial-bounds semantics so a future change cannot
- `XuanYu.World.Tests/Scene/EntityRegistryTests.cs` — sealed class EntityRegistryTests
- `XuanYu.World.Tests/Scene/EntityTests.cs` — sealed class EntityTests
- `XuanYu.World.Tests/Scene/FinalSceneTests.cs` — sealed class FinalSceneTests
- `XuanYu.World.Tests/Scene/GlobalWorldTests.cs` — sealed class GlobalWorldTests
- `XuanYu.World.Tests/Scene/SceneConsumptionTests.cs` — sealed class SceneConsumptionTests
- `XuanYu.World.Tests/Scene/SceneDocumentPersistenceTests.cs` — sealed class SceneDocumentPersistenceTests
- `XuanYu.World.Tests/Scene/SceneDocumentTests.Opening.cs` — sealed partial class SceneDocumentTests
- `XuanYu.World.Tests/Scene/SceneDocumentTests.SaveFeedback.cs` — sealed partial class SceneDocumentTests
- `XuanYu.World.Tests/Scene/SceneDocumentTests.cs` — sealed partial class SceneDocumentTests
- `XuanYu.World.Tests/Scene/SceneIsolationTests.cs` — sealed class SceneIsolationTests
- `XuanYu.World.Tests/Scene/SceneMultiEntityGateTests.cs` — sealed class SceneMultiEntityGateTests
- `XuanYu.World.Tests/Scene/SceneSelectionReentryTests.cs` — sealed class SceneSelectionReentryTests
- `XuanYu.World.Tests/Scene/SceneSingleAuthorityTests.cs` — sealed class SceneSingleAuthorityTests
- `XuanYu.World.Tests/Scene/UiHistoryTests.InlineRename.cs` — sealed partial class UiHistoryTests
- `XuanYu.World.Tests/Scene/UiHistoryTests.cs` — sealed partial class UiHistoryTests
- `XuanYu.World.Tests/Selection/FinalSelectionTests.cs` — sealed class FinalSelectionTests
- `XuanYu.World.Tests/Selection/SelectionToolStateUiTests.cs` — sealed class SelectionToolStateUiTests
- `XuanYu.World.Tests/Selection/ToolStateHighlightUiTests.Selection.cs` — sealed partial class ToolStateHighlightUiTests
- `XuanYu.World.Tests/Selection/ToolStateHighlightUiTests.cs` — sealed partial class ToolStateHighlightUiTests
- `XuanYu.World.Tests/Spatial/SceneStateOwnerSpatialTests.cs` — sealed class SceneStateOwnerSpatialTests
- `XuanYu.World.Tests/Spatial/SpatialIndexEditLifecycleTests.cs` — sealed class SpatialIndexEditLifecycleTests
- `XuanYu.World.Tests/Spatial/SpatialIndexOwnerLifecycleTests.cs` — sealed class SpatialIndexOwnerLifecycleTests
- `XuanYu.World.Tests/Spatial/SpatialIndexOwnerRevisionTests.cs` — sealed class SpatialIndexOwnerRevisionTests
- `XuanYu.World.Tests/Spatial/SpatialIndexRebuildTests.cs` — sealed class SpatialIndexRebuildTests
- `XuanYu.World.Tests/Spatial/SpatialIndexScaleTests.cs` — sealed class SpatialIndexScaleTests
- `XuanYu.World.Tests/Spatial/SpatialQueryGovernanceTests.cs` — sealed class SpatialQueryGovernanceTests
- `XuanYu.World.Tests/Spatial/SpatialQueryOracle.cs` — （职责待补）
- `XuanYu.World.Tests/Spatial/SpatialQueryTests.Geometry.cs` — sealed partial class SpatialQueryTests
- `XuanYu.World.Tests/Spatial/SpatialQueryTests.cs` — sealed partial class SpatialQueryTests
- `XuanYu.World.Tests/Spatial/SpatialRayQueryLifecycleTests.cs` — sealed class SpatialRayQueryLifecycleTests
- `XuanYu.World.Tests/Spatial/SpatialRayQueryTests.cs` — sealed class SpatialRayQueryTests
- `XuanYu.World.Tests/Spatial/SpatialRaycastNearestTests.cs` — sealed class SpatialRaycastNearestTests
- `XuanYu.World.Tests/Spatial/SpatialRaycastRevisionTests.cs` — sealed class SpatialRaycastRevisionTests
- `XuanYu.World.Tests/Spatial/SpatialRaycastScaleTests.cs` — sealed class SpatialRaycastScaleTests
- `XuanYu.World.Tests/Spatial/SpatialTestData.cs` — （职责待补）
- `XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.Plane.cs` — sealed partial class MoveTransformUiTests
- `XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.Region.cs` — sealed partial class MoveTransformUiTests
- `XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.Session.cs` — sealed partial class MoveTransformUiTests
- `XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.cs` — sealed partial class MoveTransformUiTests
- `XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.DragState.cs` — sealed partial class RotateTransformUiTests
- `XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.Helpers.cs` — sealed partial class RotateTransformUiTests
- `XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.Preview.cs` — R4-R3-R1：旋转预览必须是实时的，且“选中轮廓”改用单 Draw 重心坐标边缘高亮后，
- `XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.ToolSwitch.cs` — R4-R2：旋转工具激活时点击其他实体必须立即切换选择，且工具保持 Rotate；
- `XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.cs` — sealed partial class RotateTransformUiTests
- `XuanYu.World.Tests/Transform/Scale/ScaleGizmoGlobalModeTests.cs` — sealed class ScaleGizmoGlobalModeTests
- `XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.AxisUniform.cs` — sealed partial class ScaleTransformUiTests
- `XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.Helpers.cs` — sealed partial class ScaleTransformUiTests
- `XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.History.cs` — sealed partial class ScaleTransformUiTests
- `XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.Pointer.cs` — sealed partial class ScaleTransformUiTests
- `XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.Target.cs` — sealed partial class ScaleTransformUiTests
- `XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.cs` — R5：Scale Gizmo 缩放变换闭环集成测试。复用既有 SelectionKey / TransformSession / History 体系，
- `XuanYu.World.Tests/Transform/TransformFoundationTests.Input.cs` — sealed partial class TransformFoundationTests
- `XuanYu.World.Tests/Transform/TransformFoundationTests.Inspector.cs` — sealed partial class TransformFoundationTests
- `XuanYu.World.Tests/Transform/TransformFoundationTests.cs` — sealed partial class TransformFoundationTests
- `XuanYu.World.Tests/Transform/TransformSessionTests.cs` — sealed class TransformSessionTests
- `XuanYu.World.Tests/Transform/ViewportAssistTests.cs` — sealed class ViewportAssistTests
- `XuanYu.World.Tests/Tree/UiHierarchyConnectorTests.cs` — sealed class UiHierarchyConnectorTests
- `XuanYu.World.Tests/Tree/UiTreeGuideTests.cs` — sealed class UiTreeGuideTests
- `XuanYu.World.Tests/Tree/UiTreeToggleTests.cs` — sealed class UiTreeToggleTests
- `XuanYu.World.Tests/UiTokens/UiDebtBaseline.Colors.Axaml1.cs` — 旧 UI 债务基线（AXAML 色值 1/2，D2 自动生成）
- `XuanYu.World.Tests/UiRuntime/UiHeadlessFixture.cs` — 可复用 Avalonia Headless 会话与 UI 线程调度夹具。
- `XuanYu.World.Tests/UiRuntime/UiRuntimeTestHost.cs` — Headless Window、布局和 Visual 树查询辅助。
- `XuanYu.World.Tests/UiRuntime/LayerPanelRuntimeLayoutTests.cs` — LayerPanel 冷启动与增层布局运行时门禁。
- `XuanYu.World.Tests/UiRuntime/LayerPanelRuntimeStateTests.cs` — LayerPanel 选中、可见和锁定状态运行时门禁。
- `XuanYu.World.Tests/UiRuntime/UiRuntimeRiskTests.cs` — Top/Foot Fluent 状态覆盖风险运行时门禁。
- `XuanYu.World.Tests/UiRuntime/UiRuntimeCollection.cs` — Headless UI 测试串行集合定义。
- `XuanYu.World.Tests/UiRuntime/UiTestAppBuilder.cs` — 正式 Editor.UI App 的 Headless AppBuilder 配置。
- `XuanYu.World.Tests/UiTokens/UiDebtBaseline.Colors.Axaml2.cs` — 旧 UI 债务基线（AXAML 色值 2/2，D2 自动生成）
- `XuanYu.World.Tests/UiTokens/UiDebtBaseline.Colors.Cs.cs` — 旧 UI 债务基线（code-behind 视觉源色值，D2 自动生成）
- `XuanYu.World.Tests/UiTokens/UiDebtBaseline.Typography.cs` — 旧 UI 债务基线（字号/圆角/高度/阴影/笔画，D2 自动生成）
- `XuanYu.World.Tests/UiTokens/UiDebtBaseline.cs` — 旧 UI 债务基线匹配逻辑（226 条指纹：路径+Locator+规则+属性+值+允许次数；D3 清除 4 条）
- `XuanYu.World.Tests/UiTokens/UiDebtBaselineTests.cs` — 基线门禁：真实扫描（递归 axaml+cs）vs 细粒度基线
- `XuanYu.World.Tests/UiTokens/UiDebtBaselineBypassTests.cs` — 基线绕过反例 10 项（换位/换选择器/换 x:Name/换属性/注释漂移/增长禁止）
- `XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzer.cs` — UI 源码违规分析器（允许值从 UiTokenManifest.json 读取；cs 递归分析；D2-F1）
- `XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzer.Inline.cs` — 分析器 AXAML 规则入口（Style 块/内联元素，D2-F2；D3 起 {StaticResource} Token 引用豁免）
- `XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzer.Structure.cs` — 分析器 AXAML 结构索引（父链定位 v3：命名祖先/父类型链/同父序号，D2-F2）
- `XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzer.CsRules.cs` — 分析器 code-behind 八类颜色写法规则（Hex/Colors/ColorAPI/Brush/Uint，D2-F2）
- `XuanYu.World.Tests/UiTokens/UiCsColorRulesTests.cs` — code-behind 八类颜色写法正反例（每种 FAIL 样例 + 注释/无颜色 PASS，D2-F2）
- `XuanYu.World.Tests/UiTokens/UiDebtBaselineBypassF2Tests.cs` — AXAML 属性/位置换位反例（同 Style 属性换位/匿名控件换位/父级换位，D2-F2）
- `XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzer.Icon.cs` — 分析器图标位置 Emoji/Unicode 与 Design 外 Token 声明规则（D2-F1）
- `XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzerTests.cs` — 门禁自验证正反例（合法引用/未登记值/Emoji 正反例/Design 外 Token/cs 构造，D2-F1）
- `XuanYu.World.Tests/UiTokens/UiTokenManifestTests.cs` — Token 合同测试（Manifest↔XAML 双向 112/112 键类型值，D2-F1）
- `XuanYu.World.Tests/UiTokens/UiTokenManifestGraphTests.cs` — Token 资源引用图检查（目标存在/无循环/聚合批准文件/应用单次合并，D2-F1）
- `XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzerTokenRefTests.cs` — D3：{StaticResource} Token 引用豁免正反例（豁免合法引用、未登记字面量仍 FAIL）
- `XuanYu.World.Tests/UiTokens/UiTopTabStripModelTests.cs` — D3：页签状态机测试（溢出/箭头边界/渐隐/滚轮/钳制/步进 Token 对齐/可见性）
- `XuanYu.World.Tests/UiTokens/UiTopTabStripModelHintAndListTests.cs` — D3：一次性提示门与全部页签列表测试
- `XuanYu.World.Tests/UiTokens/UiTopTabStripContractTests.cs` — D3：外壳尺寸与页签宿主结构合同（§7.1/§10.1：窗口/面板/视口/单行/箭头/渐隐/文案/真实页签集合）
- `XuanYu.World.Tests/UiTokens/UiD3DebtClearedTests.cs` — D3：债务清零断言（基线只减不增，226 上限）+ 新文件 5+100 防回归
- `XuanYu.World.Tests/UiTokens/UiD4DebtClearedTests.cs` — D4：债务清零断言（基线 226→159，保留 2 条组件例外）+ 新文件 5+100 防回归
- `XuanYu.World.Tests/UiTokens/UiD4InspectorContractTests.cs` — D4：检查器结构合同（字号 Token/双模式/96/128/无卡片/调试页 96 列）
- `XuanYu.World.Tests/UiTokens/UiD4LayerContractTests.cs` — D4：图层面板结构合同（图标 16/热区/笔画/Layer.* Token/插入线/选中样式/三重区分）
- `XuanYu.World.Tests/UiTokens/UiF3LayerRowContractTests.cs` — MAP-A-R2-D5-F3：图层行主体布局、拉伸和手柄入口合同
- `XuanYu.World.Tests/UiTokens/UiD4LayoutModelTests.cs` — D4/D4-F1：纯布局逻辑测试（可编辑表单 360 阈值、MapId 压缩、字段行结构）
- `XuanYu.World.Tests/UiTokens/UiD4F1LayoutModelTests.cs` — D4-F1（纠偏）：319/320 密度与 359/360 表单边界 + 6 组独立组合 + 只读行水平
- `XuanYu.World.Tests/UiTokens/UiD4F1ButtonContractTests.cs` — D4-F1（纠偏）：uiTextButton 真实接线（地图 7+调试 4）+ Grid *,* 跨列 + 2×2 等宽网格
- `XuanYu.World.Tests/UiTokens/UiD4F1TextOverflowContractTests.cs` — D4-F1：展示型文本默认（NoWrap+Ellipsis+MaxLines1）+ 完整值 Tooltip + 多行专用类
- `XuanYu.World.Tests/UiTokens/UiD4F1TypographyContractTests.cs` — D4-F1：公共语义样式 Token / 无裸 FontSize / 无局部 FontFamily / Manifest 112 Frozen
- `XuanYu.World.Tests/UiTokens/UiD4MapEditorContractTests.cs` — D4：地图页结构合同（72 列摘要/MapId 不换行+复制/96 列表单/紧凑模式/单滚动/按钮组）
- `XuanYu.World.Tests/WorldPartition/WorldPartitionInvariantTests.cs` — sealed class WorldPartitionInvariantTests
- `XuanYu.World.Tests/WorldPartition/WorldPartitionMigrationTests.Activity.cs` — sealed partial class WorldPartitionMigrationTests
- `XuanYu.World.Tests/WorldPartition/WorldPartitionMigrationTests.cs` — sealed partial class WorldPartitionMigrationTests
- `XuanYu.World.Tests/WorldPartition/WorldPartitionTests.PartitionStrategy.cs` — sealed partial class WorldPartitionTests
- `XuanYu.World.Tests/WorldPartition/WorldPartitionTests.cs` — sealed partial class WorldPartitionTests
- `XuanYu.World.Tests/WorldPartition/WorldPartitionUiTests.cs` — sealed class WorldPartitionUiTests
- `XuanYu.World.Tests/XuanYu.World.Tests.csproj` — （职责待补）
- `XuanYu.World/EntityRegistry.Authoring.cs` — sealed partial class EntityRegistry
- `XuanYu.World/EntityRegistry.Replace.cs` — sealed partial class EntityRegistry
- `XuanYu.World/EntityRegistry.cs` — sealed partial class EntityRegistry
- `XuanYu.World/GlobalWorld.Authoring.cs` — sealed partial class GlobalWorld
- `XuanYu.World/GlobalWorld.Query.cs` — sealed partial class GlobalWorld
- `XuanYu.World/GlobalWorld.Snapshot.cs` — sealed partial class GlobalWorld
- `XuanYu.World/GlobalWorld.cs` — sealed partial class GlobalWorld
- `XuanYu.World/GridWorldPartitionStrategy.cs` — sealed class GridWorldPartitionStrategy
- `XuanYu.World/IWorldPartitionStrategy.cs` — interface IWorldPartitionStrategy
- `XuanYu.World/Map/MapBounds.cs` — MAP-A-R2-D1：有限地图边界（米）。地图中心为世界原点，范围 X/Y ∈ [-W/2, W/2]。
- `XuanYu.World/Map/MapCoordinateContract.cs` — MapPoint 与世界 XY 的唯一直接映射合同。
- `XuanYu.World/Map/MapDefaultDefinition.cs` — MAP-A-R2-D4：默认地图工厂。一次性创建完整地图聚合：
- `XuanYu.World/Map/MapDefinition.cs` — MAP-A-R2-D1-F1：完整地图领域聚合（权威根）。只描述地图内容（纯净、不可变），
- `XuanYu.World/Map/MapDefinitionValidator.cs` — MAP-A-R2-D1-F1：地图聚合严格校验（领域权威层）。
- `XuanYu.World/Map/MapGeometry.cs` — MAP-A-R1-D2：地图尺寸（米）。width 对应世界 X，depth 对应世界 Y，Z-Up 下高度沿 Z。
- `XuanYu.World/Map/MapId.cs` — MAP-A-R2-D1/D1-F1：地图稳定唯一标识（领域权威层）。D1 合同冻结格式：32 位十六进制，无前缀。
- `XuanYu.World/Map/MapLayer.cs` — MAP-A-R2-D1：图层领域模型（领域权威层）。用于组织地图元素，不承担渲染管线功能。
- `XuanYu.World/Map/MapLayerId.cs` — MAP-A-R2-D1：图层稳定唯一标识（领域权威层）。与 MapId 同族格式（32 位十六进制，无前缀）。
- `XuanYu.World/Map/MapLayerKind.cs` — MAP-A-R2-D4：图层角色（稳定标识，不依赖中文名称识别）。
- `XuanYu.World/Map/MapLayerRules.cs` — MAP-A-R2-D4：图层操作规则（名称校验、系统层保护、最后区域层保护、自动命名）。
- `XuanYu.World/Map/MapLayerStack.cs` — MAP-A-R2-D4：图层顺序与领域操作（纯函数，返回新不可变集合）。
- `XuanYu.World/Map/MapLayerValidator.cs` — MAP-A-R2-D4：图层集合严格校验（领域权威层）。
- `XuanYu.World/Map/MapRegion.cs` — MAP-A-R2-D1：区域领域模型（领域权威层）。地图上的二维闭合多边形（水平面坐标）。
- `XuanYu.World/Map/MapRegionIntersection.cs` — MAP-A-R3-D1：非相邻区域边的相交、接触与重叠检测。
- `XuanYu.World/Map/MapRegionDraft.cs` — MAP-A-R2-D1-F1：绘制中的区域草稿（未闭合顶点序列）。D5 绘制流程使用；
- `XuanYu.World/Map/MapRegionId.cs` — MAP-A-R2-D1：区域稳定唯一标识（领域权威层）。与 MapId 同族格式（32 位十六进制，无前缀）。
- `XuanYu.World/Map/MapRegionKind.cs` — MAP-A-R2-D1：区域类型（领域权威层）。R2 仅承载几何与基础元数据，不解释战斗含义。
- `XuanYu.World/Map/MapRegionValidator.cs` — MAP-A-R2-D1/D4：区域集合严格校验（领域权威层）。
- `XuanYu.World/Map/MapSurfaceDefinition.cs` — MAP-A-R1-D2：地表定义。支持 Flat 与 GentleHillsV1（确定性参数化起伏）。
- `XuanYu.World/Map/MapValidationResult.cs` — MAP-A-R2-D1-F1：地图领域验证结构化结果（不抛出来源不明的异常）。
- `XuanYu.World/Map/WorldMapState.cs` — MAP-A-R1-D3/D4：World 地图状态（纯数据 + 有限边界 + 高度查询 + 环境参数）。
- `XuanYu.World/Map/WorldMapStateOwner.cs` — MAP-A-R1-D3/D4：当前 World 地图状态所有者。加载/切换/卸载，暴露高度查询与渲染快照。
- `XuanYu.World/RegionKey.cs` — （职责待补）
- `XuanYu.World/Scene/SceneSpatialBoundsProjection.cs` — static class SceneSpatialBoundsProjection
- `XuanYu.World/Scene/SceneStateOwner.Lifecycle.cs` — sealed partial class SceneStateOwner
- `XuanYu.World/Scene/SceneStateOwner.Seeding.cs` — sealed partial class SceneStateOwner
- `XuanYu.World/Scene/SceneStateOwner.StaticModel.cs` — D3：静态模型只是 World 的一种普通实体类型。World 不接收 AssetId、
- `XuanYu.World/Scene/SceneStateOwner.Transform.cs` — sealed partial class SceneStateOwner
- `XuanYu.World/Scene/SceneStateOwner.cs` — Placeholder scene entities declare their OWN spatial extent (1
- `XuanYu.World/Scene/SceneWorldProjection.cs` — static class SceneWorldProjection
- `XuanYu.World/Spatial/DynamicAabbTree.Insert.cs` — sealed partial class DynamicAabbTree
- `XuanYu.World/Spatial/DynamicAabbTree.Node.cs` — sealed partial class DynamicAabbTree
- `XuanYu.World/Spatial/DynamicAabbTree.Query.cs` — sealed partial class DynamicAabbTree
- `XuanYu.World/Spatial/DynamicAabbTree.Refit.cs` — sealed partial class DynamicAabbTree
- `XuanYu.World/Spatial/DynamicAabbTree.Remove.cs` — sealed partial class DynamicAabbTree
- `XuanYu.World/Spatial/DynamicAabbTree.cs` — sealed partial class DynamicAabbTree
- `XuanYu.World/Spatial/ISpatialIndex.cs` — interface ISpatialIndex
- `XuanYu.World/Spatial/SpatialIndexOwner.cs` — sealed class SpatialIndexOwner
- `XuanYu.World/Spatial/SpatialRaycastResolver.cs` — sealed class SpatialRaycastResolver
- `XuanYu.World/WorldEntityActivity.cs` — enum WorldEntityActivity
- `XuanYu.World/WorldEntityName.cs` — static class WorldEntityName
- `XuanYu.World/WorldEntitySnapshot.cs` — （职责待补）
- `XuanYu.World/WorldEntityType.cs` — enum WorldEntityType
- `XuanYu.World/WorldPartitionEntry.cs` — （职责待补）
- `XuanYu.World/WorldPartitionMembership.cs` — sealed class WorldPartitionMembership
- `XuanYu.World/WorldQuery.cs` — Mutation is reserved
- `XuanYu.World/XuanYu.World.csproj` — （职责待补）
- `changelog.md` — 已发生有效变化日志（版本+日期+验证，月度归档）
- `docs/CODE_CONSTITUTION.md` — 代码与架构硬规则
- `docs/architecture/ENGINE_ARCHITECTURE.md` — 引擎总体架构说明
- `docs/architecture/world-a-r0-coordinate-contract.md` — 官方坐标合同（Z-Up、XY 水平、X×Y=Z）
- `docs/archive/changelog/changelog-2026-05.md` — 2026-05 changelog 月度归档
- `docs/archive/changelog/changelog-2026-06.md` — 2026-06 changelog 月度归档
- `docs/archive/changelog/changelog-2026-07.md` — 2026-07 changelog 月度归档
- `docs/dev-rules.md` — 开发硬规则执行手册（接手红线清单）
- `docs/docs-index.md` — docs 目录分类索引（哪类文档在哪里）
- `docs/governance/NAMING_RULES.md` — 命名与品牌规范
- `docs/governance/debts/arch-ui-spec-debts.md` — UI 规范受控待办登记（ARCH-UI-SPEC-R1）
- `docs/governance/debts/arch-world-debts.md` — 架构受控债务登记
- `docs/governance/dev-rules-understanding.md` — dev-rules 规则动机解释
- `docs/governance/diagnostic-safety.md` — 诊断日志与 UI 调度安全规范
- `docs/governance/naming-XuanYu-Engine.md` — 玄域引擎命名与品牌规范
- `docs/governance/ui-spec.md` — UI 规范 45 项讨论决策的历史记录（D1 起不再作为实施合同，正式规则以 UI 规范 1.0 为准）
- `docs/governance/版本号规范与历史映射.md` — 版本格式与历史编号映射
- `docs/knowledge/ui/viewport-ui-control-development-guide.md` — Viewport UI 控件承载层与开发验收知识库。
- `docs/knowledge/lessons.md` — 类型化 Lesson、停止条件与错误前提复盘。
- `docs/milestones/current/MAP-A/map-contract.md` — MAP-A 地图合同与当前轮验收材料
- `docs/milestones/closed/MAP-A/R2-closeout.md` — MAP-A-R2 CLOSED 收口报告、交付能力盘点与关闭证据。
- `docs/milestones/current/MAP-A/R3-backlog.md` — MAP-A-R3 冻结前候选方向与范围约束。
- `docs/milestones/current/MAP-A/R3-F1-closeout.md` — F1 FINAL 15 项真机 IPO 收口清单。
- `docs/milestones/current/MAP-A/viewport-overlay-development-plan.md` — OVL-R0～R3 比例尺架构整改计划与状态。
- `docs/milestones/current/MAP-A/viewport-overlay-roadmap.svg` — Viewport Overlay / Scale Indicator 浅色路线图。
- `docs/ui/玄域引擎_UI真机基线清单.md` — UI 真机验收共用 IPO 清单与 D0 基线登记（ARCH-UI-SPEC-R1）
- `docs/ui/玄域引擎_UI规范_1.0.md` — UI 规范 1.0 正式规范（唯一 UI 规范事实源，UI Spec 1.0，D1 冻结）
- `docs/ui/玄域引擎_旧UI审计矩阵.md` — 旧 UI 全量审计矩阵：违规 71 项 W01~W71 与结构性缺口 G01~G08 及清零追踪
- `docs/玄域引擎_AI开发宪法.md` — 最高长期治理规则（唯一宪法事实源）
- `file-tree.md` — （职责待补）
- `run.bat` — （职责待补）
- `samples/world-c-r1-ten-triangles.xyscene` — （职责待补）
- `XuanYu.Editor.UI/Design/UiStyles.D5.axaml` — ARCH-UI-SPEC-R1-D5：按钮与表单状态样式（D5-FIX-01 内容居中、Normal/Hover/Pressed/Focus/Disabled、uiDanger、TextBox 状态与 error/warning；全部 Token）。
- `XuanYu.Editor.UI/Vm/UiVm.Notification.cs` — D5：四级通知状态机（Info/Success/Warning/Error，单条覆盖不刷屏，纯逻辑）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapDanger.cs` — D5：危险操作确认流（DangerousCommandConfirmRequested 事件 + ConfirmDangerousCommand，未注入处理器保持直接执行）。
- `XuanYu.Editor.UI/Win/UiWin.DialogHost.cs` — D5：弹窗宿主（ShowMessage/ShowConfirm/ShowDanger；危险按钮非默认焦点，Enter=默认/非危险，Escape=取消）+ 危险操作确认接线。
- `XuanYu.Editor.UI/Foot/NotificationBar.axaml` — D5：四级通知条（图标+单行省略+完整 Tooltip）。
- `XuanYu.Editor.UI/Foot/NotificationBar.axaml.cs` — D5：通知条 code-behind（纯绑定，无逻辑）。
- `XuanYu.World.Tests/UiTokens/UiD5ButtonContractTests.cs` — D5：按钮居中/状态/Token 迁移合同。
- `XuanYu.World.Tests/UiTokens/UiD5DangerFlowTests.cs` — D5：危险确认流（注入/未注入/确认执行/忽略）+ 接线断言。
- `XuanYu.World.Tests/UiTokens/UiD5DialogAndLogContractTests.cs` — D5：弹窗宿主结构 + 日志空状态/回到底部合同。
- `XuanYu.World.Tests/UiTokens/UiD5FormContractTests.cs` — D5：表单状态/错误非仅颜色合同。
- `XuanYu.World.Tests/UiTokens/UiD5NotificationTests.cs` — D5：通知状态机测试。
- `XuanYu.Editor.UI/Vm/Logging/UiVm.Logging.State.cs` — D5 纠偏：日志空态互斥（ShowInitialLogEmpty/ShowNoFilterResults）。
- `XuanYu.Editor.UI/Vm/Logging/UiVm.Logging.Refresh.cs` — D5 纠偏：日志绑定刷新通知（多行拆分）。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapEditor.Validation.cs` — D5 纠偏：地图表单字段级校验（三字段错误/输入即清除/失焦与提交校验/FirstInvalidField/解析）。
- `XuanYu.Editor.UI/Win/UiWin.DialogHost.Danger.cs` — D5 纠偏：危险操作确认接线（fail-closed，具体动作文案）。
- `XuanYu.Editor.UI/Win/UiWin.Dialogs.cs` — D5 纠偏：错误/警告/重试弹窗宿主化（ErrorIcon/WarningIcon + ShowRetryAsync）。
- `XuanYu.Editor.UI/Win/DialogFocusTrap.cs` — D5 纠偏：弹窗焦点陷阱纯逻辑（Tab 环形循环）。
- `XuanYu.World.Tests/UiTokens/UiD5CorrectionBehaviorTests.cs` — D5 纠偏：fail-closed/字段校验/场景重试行为测试。
- `XuanYu.World.Tests/UiTokens/UiD5CorrectionNotifyTests.cs` — D5 纠偏：空态互斥与通知合并/关闭/优先级测试。
- `XuanYu.World.Tests/UiTokens/UiD5CorrectionStructureTests.cs` — D5 纠偏：焦点环/焦点陷阱/日志零原始色/Inter 零残留/无压缩行测试。
- `XuanYu.Editor.UI/Vm/Map/UiVm.MapEditor.Validation.Rules.cs` — D5 二次纠偏：字段校验规则（解析/范围 100~1000000/输入中态，边界与领域一致）。
- `XuanYu.Editor/MapEditing/MapEditSession.Document.cs`（MarkBaseline）— D5 二次纠偏：内存基线保存点（默认地图初始不误判未保存）。
- `XuanYu.World.Tests/UiTokens/UiD5InputValidationTests.cs` — D5 二次纠偏：输入阶段真实校验 8 项测试。
- `XuanYu.World.Tests/UiTokens/UiD5UnsavedFlowTests.cs` — D5 二次纠偏：未保存判断 8 项测试（IsDirty 捕获/停止上报）。
- `XuanYu.World.Tests/UiTokens/UiD5MapStatusTests.cs` — D5-FINAL：地图状态四态测试（未落盘/未保存/已保存/有未保存修改 + Undo 回保存点 + MarkBaseline 不动路径）。
- `XuanYu.World.Tests/UiTokens/UiD5UnsavedDialogTests.cs` — D5-FINAL：未保存地图弹窗测试（正式文案无内部编号/按钮严格/默认焦点取消/Enter 不危险/Esc=取消）。
- `XuanYu.World.Tests/UiTokens/UiD5UnsavedDialogBehaviorTests.cs` — D5-FINAL：弹窗行为合同测试（无修改直接新建/仅 discard 放行/不调用任何保存/服务缺失不新建）。
- `XuanYu.Editor.UI/Vm/UiVm.NotificationLifetime.cs` — D6：通知自动消失时间合同（基于 CreatedAt 与 lifetime，纯逻辑）。
- `XuanYu.Editor.UI/Win/UiWin.Accessibility.cs` — D6：窗口打开后应用自动化名称补全。
- `XuanYu.World.Tests/UiTokens/UiD6AccessibilityContractTests.cs` — D6：可访问性名称覆盖与内部编号不外泄合同。
- `XuanYu.World.Tests/UiTokens/UiD6DpiContractTests.cs` — D6：DPI/缩放与 DIP 阈值合同测试。
- `XuanYu.World.Tests/UiTokens/UiD6LogPerformanceTests.cs` — D6：日志 500 条尾窗与重复项压缩合同测试。
- `XuanYu.World.Tests/UiTokens/UiD6MotionContractTests.cs` — D6：减少动画偏好与默认短反馈合同测试。
- `scripts/arch-a-guard-editor.ps1` — （职责待补）
- `scripts/arch-a-guard-render.ps1` — （职责待补）
- `scripts/arch-a-guard-warcore.ps1` — WarCore 子守卫（D4 修复：$failures 条件初始化避免清空主守卫失败列表；被源入时不提前 exit）
- `scripts/arch-a-guard-world.ps1` — （职责待补）
- `scripts/arch-a-guard.ps1` — （职责待补）
- `XuanYu.Editor/MapEditing/MapSurfacePicker.cs` — 复用 ViewProjection 与 WorldRayFactory，按中心原点合同拾取地图平面 MapPoint。
- `XuanYu.Editor/MapEditing/RegionDrawingState.cs` — 区域绘制临时草稿、光标与首点闭合候选状态。
- `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.Commit.cs` — 区域 Draft 闭合、提交成功与错误反馈。
- `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.Input.cs` — 区域绘制视口边界判断与地图表面拾取。
- `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.Logging.cs` — 区域绘制开始、成功、取消与错误的低频中文日志。
- `XuanYu.Editor.UI/Vm/Map/UiVm.RegionDrawing.cs` — 区域绘制地面命中、Draft 顶点与失败安全预览输入。
- `XuanYu.Editor.UI/Win/UiWin.Shortcuts.cs` — 窗口快捷键路由，包含区域绘制 Enter 闭合与 Esc 取消入口。
- `XuanYu.Editor.UI/Vm/Map/MapRegionRenderProjection.cs` — 将正式区域和绘制草稿投影为静态模型渲染资源。
- `XuanYu.Render.Vulkan/Render/StaticModels/VulkanClearFrameOwner.DrawRegionModel.cs` — 复用静态模型管线绘制地图区域资源。
- `XuanYu.World.Tests/MapEditing/MapSurfacePickerTests.cs` — 地图表面拾取边界与中心命中测试。
- `XuanYu.World.Tests/MapEditing/MapCoordinateContractTests.cs` — MapPoint 与世界坐标直接映射往返测试。
- `XuanYu.World.Tests/MapEditing/RegionDrawingStateTests.cs` — 绘制草稿顶点、闭合候选与取消测试。
- `XuanYu.Core.Tests/Render/Map/MapRegionDrawPlanTests.cs` — 区域渲染资源进入帧绘制计划的合同测试。
- `XuanYu.Core.Tests/Render/LatestRenderProjectionQueueTests.cs` — PointerMoved 多次发布时只消费最新渲染投影的合同测试。
- `XuanYu.World.Tests/UiRuntime/MapVectorOverlayV1Tests.cs` — F1-V1：点、线、凹多边形、屏幕空间尺寸、缓冲复用与无 StaticModel 路径合同。
- `XuanYu.World.Tests/UiRuntime/MapVectorOverlayAnchorContractTests.cs` — F1-REWORK-B1：Fill、Stroke、Marker 对同一 MapPoint 使用完全一致的 BaseHeightMeters 世界锚点。
- `XuanYu.World.Tests/UiRuntime/MapVectorOverlayDepthPolicyTests.cs` — F1-REWORK-B2/STAB-3：极端视角深度层级、主/Overlay 管线深度策略、shader 与 draw order 合同。
- `docs/milestones/current/MAP-A/R3-C2-closure.md` — C2 RF-M01～RF-M03 真机 IPO 收口记录。
- `XuanYu.Editor.UI/Right/MapPagePanel.axaml` — 地图编辑器地图页及内部地图工具入口，含 Region Drawing 归属与 Selected 状态样式。
- `XuanYu.World.Tests/UiRuntime/RegionDrawingF1RuntimeRedTests.cs` — D2-F1 Headless Runtime RED/GREEN：Map Editor 归属与选中态深色文字。
- `XuanYu.World.Tests/UiRuntime/RegionDrawingF1CStabilityTests.cs` — F1-C：Draft 聚焦保护、相机导航/指针稳定性与低频日志合同。
- `XuanYu.World.Tests/UiRuntime/RegionDrawingF1BTests.cs` — F1-B Ground Hit Runtime：工具开关、命中坐标差异、miss、切换去重与单次输入契约。
- `XuanYu.World.Tests/UiRuntime/RegionDrawingF1FullRuntimeTests.cs` — F1 完整 Runtime：Draft 顶点、预览快照、Enter 闭合、Esc 取消与 DPI 命中回归。
- `XuanYu.World.Tests/UiRuntime/RegionDrawingF1ResizeTests.cs` — F1 Resize Runtime：视口尺寸变化后区域绘制输入继续命中并累积 Draft 顶点。
- `XuanYu.World.Tests/UiRuntime/RegionDrawingF1RenderContractTests.cs` — F1 渲染合同：首点 Draft primitive 合法且通过 Vulkan 资源校验。
- `XuanYu.World.Tests/UiTokens/UiD2F1RegionToolContractTests.cs` — D2-F1 静态 UI 归属与 Selected/Selected+Hover 样式契约。
