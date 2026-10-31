# XYE-EDITOR-UI-02 final directory tree

Actual 509-file target layout. Five source responsibility roots; C# over-limit directories 0; direct-child over-limit directories 0; depth violations 0.

```text
XuanYu.Editor.UI/
├── Infrastructure/
│   ├── Controls/
│   │   ├── Capture/
│   │   │   ├── XycUiCaptureCommand.cs
│   │   │   ├── XycUiNativeCapture.cs
│   │   │   ├── XycUiRuntime.Capture.cs
│   │   │   └── XycUiRuntime.CaptureAvalonia.cs
│   │   ├── ControlSurface/
│   │   │   ├── XycUiContracts.cs
│   │   │   ├── XycUiControlDiscovery.cs
│   │   │   ├── XycUiControlLookup.cs
│   │   │   ├── XycUiRuntime.Controls.cs
│   │   │   └── XycUiWindowNative.cs
│   │   ├── Runtime/
│   │   │   ├── XycUiRuntime.cs
│   │   │   ├── XycUiRuntime.Region.cs
│   │   │   ├── XycUiRuntime.State.cs
│   │   │   ├── XycUiRuntime.Theme.cs
│   │   │   └── XycUiRuntime.Window.cs
│   │   ├── XycSceneCore/
│   │   │   └── UiVm.XycScene.cs
│   │   └── XycSceneState/
│   │       ├── UiVm.XycScene.Guards.cs
│   │       ├── UiVm.XycScene.History.cs
│   │       ├── UiVm.XycScene.HistoryTracking.cs
│   │       ├── UiVm.XycScene.Projection.cs
│   │       └── UiVm.XycScene.Session.cs
│   ├── Diagnostics/
│   │   ├── DiagnosticAutoBinder.cs
│   │   ├── DiagnosticBadge.axaml
│   │   ├── DiagnosticBadge.axaml.cs
│   │   ├── DiagnosticDetailPanel.axaml
│   │   ├── DiagnosticDetailPanel.axaml.cs
│   │   ├── DiagnosticOverlayHost.ToolWindowState.cs
│   │   ├── OverlayBase/
│   │   │   ├── DiagnosticOverlayHost.axaml
│   │   │   ├── DiagnosticOverlayHost.axaml.cs
│   │   │   ├── DiagnosticOverlayHost.Bounds.cs
│   │   │   ├── DiagnosticOverlayHost.FloatingLayer.cs
│   │   │   ├── DiagnosticOverlayHost.Registry.cs
│   │   │   └── DiagnosticOverlayHost.TargetKind.cs
│   │   ├── OverlayBehavior/
│   │   │   ├── DiagnosticOverlayHost.Interaction.cs
│   │   │   ├── DiagnosticOverlayHost.Lifecycle.cs
│   │   │   ├── DiagnosticOverlayHost.Restore.cs
│   │   │   ├── DiagnosticOverlayHost.Sync.cs
│   │   │   └── DiagnosticOverlayHost.Tracking.cs
│   │   ├── OverlayNative/
│   │   │   ├── DiagnosticOverlayHost.NativeDialog.cs
│   │   │   ├── DiagnosticOverlayHost.NativeOverlay.cs
│   │   │   ├── DiagnosticOverlayHost.NativeViewportProbe.cs
│   │   │   ├── DiagnosticOverlayHost.ToolWindow.cs
│   │   │   └── DiagnosticOverlayHost.ToolWindowPlacement.cs
│   │   ├── OverlayProbe/
│   │   │   ├── DiagnosticOverlayHost.PopupLayers.cs
│   │   │   ├── DiagnosticOverlayHost.PopupProbe.cs
│   │   │   ├── DiagnosticOverlayHost.Probe.cs
│   │   │   ├── DiagnosticOverlayHost.ProbeBounds.cs
│   │   │   └── DiagnosticOverlayHost.ProbeCard.cs
│   │   └── OverlayState/
│   │       ├── UiVm.DiagnosticIdentity.cs
│   │       └── UiVm.DiagnosticMode.cs
│   ├── Logging/
│   │   ├── DataStructures/
│   │   │   ├── EditorLogBuffer.cs
│   │   │   ├── EditorLogBus.cs
│   │   │   ├── EditorLogCategory.cs
│   │   │   ├── EditorLogLevel.cs
│   │   │   └── LogEntry.cs
│   │   ├── EditorLogSource.cs
│   │   ├── Filtering/
│   │   │   ├── EditorLogFilter.cs
│   │   │   ├── EditorLogFilterQuery.cs
│   │   │   ├── EditorLogFilterState.cs
│   │   │   └── EditorLogNoiseFilter.cs
│   │   ├── Presentation/
│   │   │   ├── EditorDisplayText.cs
│   │   │   ├── EditorLogClipboardText.cs
│   │   │   ├── EditorLogRepeatKey.cs
│   │   │   ├── EditorLogSummary.cs
│   │   │   └── LogProjectionCache.cs
│   │   ├── Refresh/
│   │   │   ├── UiVm.Logging.Refresh.cs
│   │   │   └── UiVm.Logging.State.cs
│   │   └── ViewModel/
│   │       ├── UiText.cs
│   │       ├── UiVm.DebugProjection.cs
│   │       ├── UiVm.Logging.cs
│   │       ├── UiVm.Logging.Filters.cs
│   │       └── UiVm.Logging.Metrics.cs
│   ├── Resources/
│   │   ├── Accessibility/
│   │   │   ├── UiAutomationNamer.cs
│   │   │   ├── UiDpiContract.cs
│   │   │   └── UiMotionPreference.cs
│   │   ├── app.manifest
│   │   ├── Bootstrap/
│   │   │   ├── App.axaml
│   │   │   ├── App.axaml.cs
│   │   │   └── Program.cs
│   │   ├── Capture/
│   │   │   ├── SilentCaptureEvidence.cs
│   │   │   ├── XycSilentApplicationStartup.cs
│   │   │   └── XycSilentCaptureCommands.cs
│   │   ├── Host/
│   │   │   ├── SilentHostDiagnostics.cs
│   │   │   ├── SilentNativeWindow.cs
│   │   │   ├── SilentVulkanHost.cs
│   │   │   └── UiVmXycRuntime.Diagnostics.cs
│   │   └── Theme/
│   │       ├── EditorIcons.axaml
│   │       ├── RelayCommand.cs
│   │       ├── Ui.axaml
│   │       ├── UiStyles.D4F1.axaml
│   │       ├── UiStyles.D5.axaml
│   │       ├── UiTokenManifest.json
│   │       ├── UiTokens.axaml
│   │       ├── UiTokens.Colors.Components.axaml
│   │       ├── UiTokens.Colors.Core.axaml
│   │       ├── UiTokens.Controls.axaml
│   │       ├── UiTokens.Fonts.axaml
│   │       ├── UiTokens.Icons.axaml
│   │       ├── UiTokens.Motion.axaml
│   │       └── UiTokens.Spacing.axaml
│   └── Services/
│       ├── Camera/
│       │   ├── UiVmXycCamera.cs
│       │   ├── UiVmXycRuntime.Dolly.cs
│       │   ├── XycCameraValidator.cs
│       │   └── XycDollyCommand.cs
│       ├── Commands/
│       │   ├── XycCommandProcessor.cs
│       │   ├── XycStdioHost.cs
│       │   └── XycUiCommandProcessor.cs
│       ├── IXycRuntime.cs
│       ├── Notifications/
│       │   ├── BottomRightNotificationHost.axaml
│       │   └── BottomRightNotificationHost.axaml.cs
│       ├── Scene/
│       │   ├── UiVmXycRuntime.Scene.cs
│       │   ├── UiVmXycScene.cs
│       │   ├── XycSceneCommandProcessor.cs
│       │   └── XycSceneContracts.cs
│       ├── UiVmXycRuntime.cs
│       ├── XycAppSession.cs
│       ├── XycModels.cs
│       └── XycTestDemPolicy.cs
├── MapUI/
│   ├── Data/
│   │   ├── Catalog/
│   │   │   ├── DatasetLayerPanel.axaml
│   │   │   ├── DatasetLayerPanel.axaml.cs
│   │   │   ├── DatasetLayerPanel.Drag.cs
│   │   │   ├── DatasetPanel.axaml
│   │   │   ├── DatasetPanel.axaml.cs
│   │   │   ├── MapDatasetRow.cs
│   │   │   └── MapDatasetTypePresentation.cs
│   │   ├── DatasetAuthoring/
│   │   │   ├── UiVm.MapDataset.DrawingBootstrap.cs
│   │   │   ├── UiVm.MapDataset.DrawingTarget.cs
│   │   │   ├── UiVm.MapDataset.MarkerBootstrap.cs
│   │   │   └── UiVm.MapDataset.RoadBootstrap.cs
│   │   ├── DatasetCommands/
│   │   │   ├── UiVm.MapDataset.Commands.cs
│   │   │   ├── UiVm.MapDataset.Routing.cs
│   │   │   └── UiVm.MapDataset.Selection.cs
│   │   ├── DatasetPresentation/
│   │   │   ├── UiVm.MapDataset.Inspector.cs
│   │   │   ├── UiVm.MapDataset.LayerBridge.cs
│   │   │   ├── UiVm.MapDataset.RegionPresentation.cs
│   │   │   └── UiVm.MapDataset.RoadPresentation.cs
│   │   └── DatasetState/
│   │       ├── UiVm.MapDataset.cs
│   │       ├── UiVm.MapDataset.Logging.cs
│   │       ├── UiVm.MapDataset.Name.cs
│   │       ├── UiVm.MapManifest.cs
│   │       └── UiVm.MapWorld.cs
│   ├── Editing/
│   │   ├── Commands/
│   │   │   ├── UiVm.MapCommandRouting.Danger.cs
│   │   │   ├── UiVm.MapDanger.cs
│   │   │   ├── UiVm.MapLayerDrag.cs
│   │   │   ├── UiVm.MapLayerSelection.cs
│   │   │   └── UiVm.RoadTool.cs
│   │   ├── Editor/
│   │   │   ├── UiVm.MapEditor.cs
│   │   │   ├── UiVm.MapEditor.Display.cs
│   │   │   ├── UiVm.MapEditor.NumericDraft.cs
│   │   │   ├── UiVm.MapEditor.Validation.cs
│   │   │   └── UiVm.MapEditor.Validation.Rules.cs
│   │   ├── Geometry/
│   │   │   ├── UiVm.MapGeometryEditing.Bindings.cs
│   │   │   ├── UiVm.MapGeometryEditing.Helpers.cs
│   │   │   ├── UiVm.MapGeometryEditing.Inspector.cs
│   │   │   ├── UiVm.MapGeometryEditing.Points.cs
│   │   │   └── UiVm.MapGeometryEditing.RoadVertices.cs
│   │   ├── History/
│   │   │   └── UiVm.MapHistory.cs
│   │   ├── LayerPanels/
│   │   │   ├── LayerPanel.axaml
│   │   │   ├── LayerPanel.axaml.cs
│   │   │   ├── LayerPanel.DragDrop.cs
│   │   │   ├── LayerPanel.Rename.cs
│   │   │   └── LayerPanel.States.axaml
│   │   ├── UiVm.MapCommandRouting.cs
│   │   ├── UiVm.MapGeometryEditing.cs
│   │   └── UiVm.MapGeometryEditing.State.cs
│   ├── Interaction/
│   │   ├── ContextMenu/
│   │   │   ├── MapContextMenuInputConsumer.cs
│   │   │   └── UiVm.MapContextMenu.cs
│   │   ├── Marker/
│   │   │   ├── UiVm.MapMarkerInspector.cs
│   │   │   └── UiVm.MapMarkerPlacement.cs
│   │   ├── RegionCommit/
│   │   │   ├── UiVm.RegionDrawing.Close.cs
│   │   │   ├── UiVm.RegionDrawing.Commit.cs
│   │   │   ├── UiVm.RegionDrawing.DraftHistory.cs
│   │   │   └── UiVm.RegionDrawing.Logging.cs
│   │   ├── RegionInput/
│   │   │   ├── UiVm.RegionDrawing.Capture.cs
│   │   │   ├── UiVm.RegionDrawing.cs
│   │   │   ├── UiVm.RegionDrawing.Input.cs
│   │   │   └── UiVm.RegionDrawing.Snap.cs
│   │   └── Road/
│   │       ├── UiVm.MapRoadSelection.cs
│   │       ├── UiVm.RoadDrawing.Commit.cs
│   │       ├── UiVm.RoadDrawing.cs
│   │       ├── UiVm.RoadDrawing.History.cs
│   │       └── UiVm.RoadDrawing.Logging.cs
│   ├── Presentation/
│   │   ├── EditorPanels/
│   │   │   ├── EditorLayerDock.axaml
│   │   │   ├── EditorLayerDock.axaml.cs
│   │   │   ├── MapEditorPanel.axaml
│   │   │   ├── MapEditorPanel.axaml.cs
│   │   │   ├── MapFormPanel.axaml
│   │   │   └── MapFormPanel.axaml.cs
│   │   ├── Inspectors/
│   │   │   ├── FeatureInspectorPanel.axaml
│   │   │   ├── FeatureInspectorPanel.axaml.cs
│   │   │   ├── MarkerInspectorPanel.axaml
│   │   │   ├── MarkerInspectorPanel.axaml.cs
│   │   │   └── UiVm.MapLayerInspector.cs
│   │   ├── LayerRows/
│   │   │   ├── EditorLayerProviderAdapter.cs
│   │   │   ├── MapLayerRowViewModel.cs
│   │   │   ├── MapLayerRowViewModel.Rename.cs
│   │   │   ├── UiVm.LayerContext.cs
│   │   │   └── UiVm.MapLayers.cs
│   │   ├── Layout/
│   │   │   ├── EditableFormLayoutModel.cs
│   │   │   └── MapEditorLayoutModel.cs
│   │   ├── MapIdDisplayFormat.cs
│   │   └── MapToolPanels/
│   │       ├── MapPagePanel.axaml
│   │       ├── MapPagePanel.axaml.cs
│   │       ├── MarkerPanel.axaml
│   │       ├── MarkerPanel.axaml.cs
│   │       ├── RegionalAuthoringPanel.axaml
│   │       ├── RegionalAuthoringPanel.axaml.cs
│   │       ├── RegionPanel.axaml
│   │       ├── RegionPanel.axaml.cs
│   │       ├── RoadPanel.axaml
│   │       └── RoadPanel.axaml.cs
│   └── Rendering/
│       ├── Diagnostics/
│       │   ├── UiVm.MapDiagnostics.cs
│       │   ├── UiVm.MapDiagnostics.Format.cs
│       │   └── UiVm.MapLayerDiagnostics.cs
│       ├── Geometry/
│       │   ├── MapVectorOverlayBuilder.cs
│       │   ├── MapVectorOverlayBuilder.Geometry.cs
│       │   ├── MapVectorOverlayBuilder.Labels.cs
│       │   ├── MapVectorOverlayBuilder.Road.cs
│       │   └── MapVectorOverlayBuilder.Surface.cs
│       ├── LabelCache/
│       │   ├── MapLabelBitmapCache.cs
│       │   ├── MapLabelCacheKey.cs
│       │   └── MapLabelRasterizer.cs
│       ├── Labels/
│       │   └── UiVm.MapLabelCache.cs
│       ├── MapVectorOverlayBuilder.Finalize.cs
│       ├── MapVectorOverlayTriangulation.cs
│       ├── Projection/
│       │   ├── MapRegionColorPreviewProjection.cs
│       │   ├── MapRegionLabelProjection.cs
│       │   ├── MapRegionRenderProjection.cs
│       │   ├── MapRegionRenderStyle.cs
│       │   └── MapRenderSnapshotProjection.cs
│       └── UiVm.MapRender.cs
├── Panels/
│   ├── Inspector/
│   │   ├── EditorRightTabs.axaml
│   │   ├── EditorRightTabs.axaml.cs
│   │   ├── InspectorInput/
│   │   │   ├── UiVm.EntityInspector.cs
│   │   │   ├── UiVm.InspectorFeatureEditing.cs
│   │   │   ├── UiVm.InspectorInput.cs
│   │   │   ├── UiVm.InspectorInput.Parse.cs
│   │   │   └── UiVm.InspectorNavigation.Commands.cs
│   │   ├── InspectorNavigation/
│   │   │   ├── UiVm.Inspector.cs
│   │   │   ├── UiVm.InspectorNavigation.cs
│   │   │   ├── UiVm.InspectorRegionColorPreview.cs
│   │   │   ├── UiVm.InspectorSections.cs
│   │   │   └── UiVm.TerrainInspector.cs
│   │   ├── InspectorPanels/
│   │   │   ├── EditorCompactStyles.axaml
│   │   │   ├── EntityInspectorPanel.axaml
│   │   │   ├── EntityInspectorPanel.axaml.cs
│   │   │   ├── InspectorPanel.axaml
│   │   │   ├── InspectorPanel.axaml.cs
│   │   │   ├── LayerInspectorPanel.axaml
│   │   │   ├── LayerInspectorPanel.axaml.cs
│   │   │   ├── Right.axaml
│   │   │   └── Right.axaml.cs
│   │   ├── InspectorPropertySearch.cs
│   │   ├── InspectorReadOnlyValuePresenter.axaml
│   │   ├── InspectorReadOnlyValuePresenter.axaml.cs
│   │   ├── InspectorRecentStore.cs
│   │   ├── InspectorSelection.cs
│   │   ├── PropertyModels/
│   │   │   ├── InspectorCategoryItem.cs
│   │   │   ├── InspectorColorValue.cs
│   │   │   ├── InspectorDescriptors.cs
│   │   │   └── InspectorEditTarget.cs
│   │   └── PropertyRows/
│   │       ├── InspectorFieldRow.cs
│   │       ├── InspectorPanel.RegionColor.cs
│   │       ├── InspectorPropertyDescriptor.cs
│   │       ├── InspectorPropertyRow.cs
│   │       └── RegionFillColorPreview.cs
│   ├── Navigation/
│   │   ├── Hierarchy/
│   │   │   ├── HierarchyWorkspace.axaml
│   │   │   ├── HierarchyWorkspace.axaml.cs
│   │   │   ├── HierarchyWorkspace.Context.cs
│   │   │   └── HierarchyWorkspace.Rename.cs
│   │   ├── LeftWorkspace/
│   │   │   ├── Left.axaml
│   │   │   ├── Left.axaml.cs
│   │   │   └── Left.Styles.axaml
│   │   ├── ProjectTree/
│   │   │   ├── InlineRenameActivation.cs
│   │   │   ├── ProjectWorkspace.axaml
│   │   │   ├── ProjectWorkspace.axaml.cs
│   │   │   ├── UiVm.ProjectTree.cs
│   │   │   └── UiVm.TreeCommands.cs
│   │   └── Tree/
│   │       ├── EditorTreeNode.cs
│   │       ├── TreeGuide.cs
│   │       ├── TreeGuideBuilder.cs
│   │       └── TreeGuideSegment.cs
│   ├── Status/
│   │   ├── LogControls/
│   │   │   ├── LogAutoScrollPolicy.cs
│   │   │   ├── LogListAutoScrollController.cs
│   │   │   ├── LogListAutoScrollController.Follow.cs
│   │   │   └── LogListAutoScrollController.Layout.cs
│   │   ├── LogDetails/
│   │   │   ├── LogDetailPanel.axaml
│   │   │   └── LogDetailPanel.axaml.cs
│   │   ├── StatusWidgets/
│   │   │   ├── Foot.axaml
│   │   │   ├── Foot.axaml.cs
│   │   │   ├── Foot.States.axaml
│   │   │   ├── NotificationBar.axaml
│   │   │   └── NotificationBar.axaml.cs
│   │   └── UiVm.DocumentStatus.cs
│   ├── Tools/
│   │   ├── ContextTools/
│   │   │   ├── ContextToolBar.axaml
│   │   │   ├── ContextToolBar.axaml.cs
│   │   │   ├── ContextToolbarDiagnosticBridge.cs
│   │   │   ├── EditToolsModule.axaml
│   │   │   ├── EditToolsModule.axaml.cs
│   │   │   ├── SnapModule.axaml
│   │   │   └── SnapModule.axaml.cs
│   │   ├── Tabs/
│   │   │   ├── TopTabStripController.AllTabs.cs
│   │   │   ├── TopTabStripController.cs
│   │   │   ├── TopTabStripController.Hint.cs
│   │   │   ├── TopTabStripController.Visible.cs
│   │   │   ├── TopTabStripModel.cs
│   │   │   └── TopTabStripTemplate.axaml
│   │   ├── ViewTools/
│   │   │   ├── ViewModule.axaml
│   │   │   ├── ViewModule.axaml.cs
│   │   │   ├── ViewOptionsMenu.axaml
│   │   │   └── ViewOptionsMenu.axaml.cs
│   │   └── WorkspaceModules/
│   │       ├── FileModule.axaml
│   │       ├── FileModule.axaml.cs
│   │       ├── RuntimeStatusModule.axaml
│   │       ├── RuntimeStatusModule.axaml.cs
│   │       ├── Top.axaml
│   │       └── Top.axaml.cs
│   └── Workspace/
│       ├── TerrainModel/
│       │   ├── TerrainDiagnosticSamples.cs
│       │   ├── TerrainImportState.cs
│       │   ├── TerrainRevisionLedger.cs
│       │   ├── TerrainWorldBounds.cs
│       │   └── TerrainWorldPlacement.cs
│       ├── TerrainRendering/
│       │   ├── TerrainRenderContentFingerprint.cs
│       │   ├── TerrainRenderGroundSurface.cs
│       │   ├── TerrainRenderProjectionAdapter.cs
│       │   ├── UiVm.TerrainRenderCache.cs
│       │   └── UiVm.TerrainRevision.cs
│       ├── UiVm.EntityCommands.cs
│       ├── UiVm.History.cs
│       ├── UiVm.History.Entities.cs
│       ├── UiVm.History.Transaction.cs
│       ├── WorkspaceCommands/
│       │   ├── UiVm.ContextToolbarDrawing.cs
│       │   ├── UiVm.RegionAuthoring.cs
│       │   ├── UiVm.TerrainContext.cs
│       │   ├── UiVm.TerrainImport.cs
│       │   └── UiVm.TerrainImportRuntime.cs
│       ├── WorkspaceLifecycle/
│       │   ├── UiVm.TerrainAutoFraming.cs
│       │   ├── UiVm.TerrainPresentationHook.cs
│       │   └── UiVm.Workspace.cs
│       ├── WorkspaceSelector.axaml
│       └── WorkspaceSelector.axaml.cs
├── Shell/
│   ├── Composition/
│   │   ├── Application/
│   │   │   ├── Main.axaml
│   │   │   ├── Main.axaml.cs
│   │   │   ├── UiRoot.axaml
│   │   │   └── UiRoot.axaml.cs
│   │   └── DialogServices/
│   │       ├── IEditorDialogService.cs
│   │       └── NullEditorDialogService.cs
│   ├── Documents/
│   │   ├── ImportAndState/
│   │   │   ├── D2StaticModelDemo.cs
│   │   │   ├── SceneHistoryEntry.cs
│   │   │   ├── UiVm.Scene.cs
│   │   │   ├── UiVm.SceneSnapshotTrace.cs
│   │   │   └── UiVm.StaticModelImport.cs
│   │   ├── Persistence/
│   │   │   ├── UiVm.SceneDocument.cs
│   │   │   ├── UiVm.SceneDocument.New.cs
│   │   │   ├── UiVm.SceneDocumentLog.cs
│   │   │   ├── UiVm.SceneDocumentMapRef.cs
│   │   │   └── UiVm.SceneDocumentSave.cs
│   │   └── Rendering/
│   │       ├── SceneRenderProjectionAdapter.cs
│   │       ├── StaticModelRenderAdapter.cs
│   │       ├── UiVm.RenderProjection.cs
│   │       └── UiVm.WorldProjection.cs
│   ├── Navigation/
│   │   └── UiVm.Mode.cs
│   ├── ViewModels/
│   │   ├── Application/
│   │   │   ├── EditorApplicationState.cs
│   │   │   ├── UiVm.ApplicationState.cs
│   │   │   └── UiVm.Composition.cs
│   │   ├── Diagnostics/
│   │   │   ├── UiVm.DiagnosticReplayLogs.cs
│   │   │   └── UiVm.LogShell.cs
│   │   ├── Notifications/
│   │   │   ├── UiVm.Notification.cs
│   │   │   └── UiVm.NotificationLifetime.cs
│   │   ├── Panels/
│   │   │   └── UiVm.RightPanel.cs
│   │   ├── UiVm.cs
│   │   └── Viewport/
│   │       ├── UiVm.NativeHostLifecycle.cs
│   │       └── UiVm.ViewportInput.cs
│   └── Windowing/
│       ├── Commands/
│       │   ├── UiWin.EntityShortcuts.cs
│       │   ├── UiWin.MapCommands.cs
│       │   ├── UiWin.NativePicker.cs
│       │   ├── UiWin.SceneCommands.cs
│       │   └── UiWin.TerrainCommands.cs
│       ├── DialogLifecycle/
│       │   ├── UiWin.DialogHost.cs
│       │   ├── UiWin.DialogHost.Danger.cs
│       │   ├── UiWin.DialogHost.Input.cs
│       │   ├── UiWin.Dialogs.cs
│       │   └── UiWin.UnsavedDialog.cs
│       ├── Shortcuts/
│       │   ├── UiWin.Shortcuts.cs
│       │   ├── UnsavedChangesConfirmationWindow.axaml
│       │   └── UnsavedChangesConfirmationWindow.axaml.cs
│       └── WindowLifecycle/
│           ├── DialogFocusTrap.cs
│           ├── LayerDeleteConfirmationWindow.axaml
│           ├── LayerDeleteConfirmationWindow.axaml.cs
│           ├── UiWin.Accessibility.cs
│           ├── UiWin.axaml
│           ├── UiWin.axaml.cs
│           └── UiWin.CloseLifecycle.cs
├── ViewportUI/
│   ├── Camera/
│   │   ├── FarProjection/
│   │   │   ├── SceneFarBounds.cs
│   │   │   ├── TerrainFarPlanePolicy.cs
│   │   │   ├── UiVm.FarProjectionDiagnostic.cs
│   │   │   └── UiVm.TerrainFarBounds.cs
│   │   ├── Framing/
│   │   │   ├── UiVm.Camera.Framing.cs
│   │   │   ├── UiVm.Camera.Framing.Draft.cs
│   │   │   ├── UiVm.CameraDolly.cs
│   │   │   ├── UiVm.CameraDollyTerrainConstraint.cs
│   │   │   └── UiVm.ScaleIndicator.cs
│   │   ├── Navigation/
│   │   │   ├── UiVm.Camera.cs
│   │   │   ├── UiVm.CameraNavigation.cs
│   │   │   └── UiVm.CameraNavigation.Orbit.cs
│   │   ├── Session/
│   │   │   ├── CameraSessionMode.cs
│   │   │   ├── CameraSessionSnapshot.cs
│   │   │   └── OrbitPivotAuthority.cs
│   │   └── ViewModes/
│   │       ├── OrbitProbe.cs
│   │       ├── StandardViewResolver.cs
│   │       └── UiVm.ViewGizmo.cs
│   ├── Diagnostics/
│   │   ├── DiagnosticNativePointerProbe.cs
│   │   ├── DiagnosticNativeViewportEvent.cs
│   │   ├── DiagnosticPopupHost.cs
│   │   ├── DiagnosticProbeResolver.cs
│   │   ├── DiagnosticProbeResolver.Semantic.cs
│   │   ├── Inspection/
│   │   │   ├── DiagnosticClipboard.cs
│   │   │   ├── DiagnosticElementFormatter.cs
│   │   │   ├── DiagnosticElementSnapshot.cs
│   │   │   ├── DiagnosticReportFormatter.cs
│   │   │   └── DiagnosticSnapshot.cs
│   │   ├── NativeViewport/
│   │   │   ├── DiagnosticNativeCoordinateMapping.cs
│   │   │   ├── DiagnosticNativeViewportTarget.cs
│   │   │   ├── TerrainCameraReplayRunner.Actions.cs
│   │   │   ├── TerrainCameraReplayRunner.cs
│   │   │   └── TerrainNativeInputReplay.cs
│   │   ├── Placement/
│   │   │   ├── DiagnosticCardPlacementMode.cs
│   │   │   ├── DiagnosticFloatingCard.axaml
│   │   │   ├── DiagnosticFloatingCard.axaml.cs
│   │   │   ├── DiagnosticFloatingToolWindow.cs
│   │   │   ├── DiagnosticPlacementModels.cs
│   │   │   └── DiagnosticPlacementPolicy.cs
│   │   ├── Probing/
│   │   │   ├── DiagnosticProbeResult.cs
│   │   │   ├── DiagnosticXyuiIdentity.cs
│   │   │   ├── DiagnosticXyuiResolver.cs
│   │   │   ├── TerrainReplayCapture.cs
│   │   │   └── TerrainReplayScaleCapture.cs
│   │   └── Registry/
│   │       ├── DiagnosticId.cs
│   │       ├── DiagnosticRegistry.cs
│   │       ├── DiagnosticSnapshotFactory.cs
│   │       ├── FeatureDiagnosticIds.cs
│   │       └── XYDiagnostic.cs
│   ├── Host/
│   │   ├── Context/
│   │   │   ├── VulkanNativeHost.ContextMenu.cs
│   │   │   ├── VulkanNativeHost.Dpi.cs
│   │   │   ├── VulkanNativeHost.LayoutSync.cs
│   │   │   └── VulkanNativeHost.RegionSnapInput.cs
│   │   ├── Diagnostics/
│   │   │   ├── VulkanNativeHost.DiagnosticPointer.cs
│   │   │   ├── VulkanNativeHost.Log.cs
│   │   │   └── VulkanNativeHost.Probe.cs
│   │   ├── Input/
│   │   │   ├── VulkanNativeHost.AvaloniaKeyboard.cs
│   │   │   ├── VulkanNativeHost.AvaloniaPointer.cs
│   │   │   ├── VulkanNativeHost.CameraPointer.cs
│   │   │   ├── VulkanNativeHost.Keyboard.cs
│   │   │   └── VulkanNativeHost.Pointer.cs
│   │   ├── Lifecycle/
│   │   │   ├── VulkanNativeHost.AvaloniaLifecycle.cs
│   │   │   ├── VulkanNativeHost.Bridge.cs
│   │   │   ├── VulkanNativeHost.ContextMenuLifecycle.cs
│   │   │   ├── VulkanNativeHost.InputLifecycle.cs
│   │   │   └── VulkanNativeHost.SurfaceAttach.cs
│   │   ├── NativeHostResizeCoalescer.cs
│   │   ├── NativeHostResizeSnapshot.cs
│   │   ├── NativeHostSurfaceContract.cs
│   │   ├── ViewportNativeHostRoute.cs
│   │   ├── VulkanViewport.axaml
│   │   ├── VulkanViewport.axaml.cs
│   │   └── Win32Host/
│   │       ├── VulkanNativeHost.cs
│   │       ├── Win32ViewportHost.cs
│   │       ├── Win32ViewportHost.Input.cs
│   │       ├── Win32ViewportHost.Input.Diagnostic.cs
│   │       └── Win32ViewportHost.Keyboard.cs
│   ├── Input/
│   │   ├── AvaloniaAdapters/
│   │   │   ├── AvaloniaKeyboardEventAdapter.cs
│   │   │   ├── AvaloniaPointerEventAdapter.cs
│   │   │   ├── AvaloniaViewportInputForwarder.cs
│   │   │   └── AvaloniaViewportKeyboardInputForwarder.cs
│   │   ├── NativeAdapters/
│   │   │   ├── NativeKeyboardEventAdapter.cs
│   │   │   ├── NativePointerEventAdapter.cs
│   │   │   ├── NativePointerSourceBoundary.cs
│   │   │   ├── NativeViewportInputForwarder.cs
│   │   │   └── NativeViewportKeyboardInputForwarder.cs
│   │   ├── NativeKeyMessage.cs
│   │   ├── NativePointerMessage.cs
│   │   ├── NativePointerRoutePolicy.cs
│   │   ├── Routing/
│   │   │   ├── UiVmD1Handler.cs
│   │   │   ├── UiVmMapBackend.cs
│   │   │   ├── UiVmNavigationViewportInputHandler.cs
│   │   │   └── UiVmViewportInputComposition.cs
│   │   ├── Selection/
│   │   │   ├── UiVm.Picking.cs
│   │   │   ├── UiVm.Selection.cs
│   │   │   ├── UiVm.SelectionProjection.cs
│   │   │   ├── UiVm.SelectionValidity.cs
│   │   │   └── UiVm.ViewportSelection.cs
│   │   └── SelectionTrace/
│   │       ├── UiVm.SelectionTrace.cs
│   │       └── ViewportPickingLogFormatter.cs
│   └── Interaction/
│       ├── Gesture/
│       │   ├── UiVm.Interaction.Begin.cs
│       │   ├── UiVm.Interaction.cs
│       │   ├── UiVm.InteractionCancel.cs
│       │   ├── UiVm.InteractionPointer.cs
│       │   └── UiVm.InteractionTransientCleanup.cs
│       ├── MoveGizmo/
│       │   ├── UiVm.MoveGizmo.cs
│       │   ├── UiVm.MoveGizmoLogging.cs
│       │   └── UiVm.MoveScreenSize.cs
│       ├── NavigationGizmo/
│       │   ├── ViewNavigationGizmo.HitTest.cs
│       │   └── ViewNavigationGizmo.Layout.cs
│       ├── OtherGizmos/
│       │   ├── UiVm.RotateGizmo.cs
│       │   └── UiVm.ScaleGizmo.cs
│       └── TransformTools/
│           ├── UiVm.InputGuards.cs
│           ├── UiVm.Tool.cs
│           ├── UiVm.TransformAvailability.cs
│           └── UiVm.ViewportAssist.cs
└── XuanYu.Editor.UI.csproj
```
