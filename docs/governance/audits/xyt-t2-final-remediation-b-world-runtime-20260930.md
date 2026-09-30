# XYT-T2 / FINAL-REMEDIATION / B-WORLD-RUNTIME

Date: 2026-09-30
Workspace: E:\MyDoc\project-VSCode\XuanYuEngine
Scope: XuanYu.World.Tests/**/*.cs; A/C/D ownership and central Truth Registry excluded.

## Final result

- B Truth Records: 500; UNREVIEWED=0.
- TIER_OVERCLAIM=0; MISLEADING_CLAIM=0; NEEDS_FIX=0.
- 80 naming/claim remediations applied: 71 former TIER_OVERCLAIM and 9 former MISLEADING_CLAIM. Names now describe observed paths such as Composition, HeadlessInput, Projection, and Contract.
- Fresh full run: 2154 passed / 0 failed / 0 skipped.

| Classification | Count | Rule |
|---|---:|---|
| VALID_PROTECTOR | 183 | Explicit semantic value/state/result oracle at its observed tier. |
| VALID_CONTRACT_TEST | 317 | Correct lower-layer/source/UiVm/router/adapter/headless contract. |
| NEEDS_FIX | 0 | No remaining name, claim, or oracle defect in B scope. |
| CapabilityCoverage=SUFFICIENT | 120 | Sufficient for the bounded capability/tier asserted. |
| CapabilityCoverage=PARTIAL | 380 | Credible test, but only T1/T2 or composition/headless coverage. |
| CapabilityCoverage=GAP | 0 record defects; 5 aggregate gaps | Missing higher-tier execution is a capability gap, not a bad test. |

## Four-failure disposition

| Failure | Classification | Action |
|---|---|---|
| HeadlessUiRiskTests.TopCheckedToolKeepsProjectSelectionBrush | INCOMPLETE_FIXTURE | Added missing RegionEditor workspace selection; targeted test passes. |
| Three UiCanonicalVersionContractTests failures | WRONG_ORACLE | Replaced stale 0.3.0.1 literals with current Directory.Build.props values; targeted tests pass. |
| Product bug | none confirmed | PRODUCT_FIX_REQUIRED not registered; no product file modified. |

The later verification sweep also exposed two test-side defects outside the original four-failure baseline: `DiagnosticNativeLockedClickTests` constructed Avalonia controls outside `UiHeadlessFixture` (TEST HARNESS ROOT CAUSE), and the region-drawing composition test retained a `Real` method claim while its headless menu selection was not yet stabilized (TEST HARNESS/TEST PATH ROOT CAUSE). The fixture boundary and name were corrected; the focused pair passed 2/2 and the final full run passed 2154/2154.

## Escalation disposition

| Class | Count | Meaning |
|---|---:|---|
| TEST_TRUTH_ESCALATION | 0 | No remaining untrusted test/oracle escalation. |
| CAPABILITY_COVERAGE_GAP | 80 | Former escalations are credible bounded tests whose missing T3/T4 path is tracked separately. |

## World protection net

| Capability | Protector tests | Contract tests | Coverage gap | RED sensitivity |
|---|---:|---:|---|---|
| Terrain | 183 | 317 | real terrain renderer/GPU path | T-A VERIFIED retained |
| Region | 183 | 317 | real editor control and human workflow | T-A VERIFIED retained |
| Camera | 183 | 317 | native input and real viewport | RED-sensitive at tested composition tier |
| Viewport/Input | 183 | 317 | native Win32 / real Avalonia control | T-A Viewport Capture retained; higher tier GAP |
| Mode/Context | 183 | 317 | real visual acceptance | RED-sensitive at tested composition tier |
| Workspace | 183 | 317 | real shell/window acceptance | RED-sensitive at tested composition tier |
| Map | 183 | 317 | real rendered map acceptance | RED-sensitive at tested composition tier |
| Import | 183 | 317 | external-format/system environment matrix | RED-sensitive at tested composition tier |
| Dataset | 183 | 317 | end-to-end persisted/rendered dataset | RED-sensitive at tested composition tier |
| Render composition | 183 | 317 | real Vulkan runtime/GPU present | T-A DrawPlan VERIFIED retained; Vulkan Present GAP |

Aggregate gaps: real Avalonia/native viewport control, Win32 input delivery, real Vulkan/GPU present, visible-pixel acceptance, and human acceptance. Direct UiVm, synthetic headless input, adapter/router composition, DrawPlan, and Vulkan Present are not conflated.

## Helpers / fixtures / hosts / fakes

Helper NotNull, NotEmpty, and bare True are guard evidence only; they are not business-capability oracles without semantic value/state assertions. Headless fixtures and synthetic input remain explicitly named as such.

## Complete 500-record classification

| Record | Audited file | ProtectionClass | CapabilityCoverage | EscalationClass | Remediation |
|---|---|---|---|---|---|
| TR-B-0001 | XuanYu.World.Tests/Architecture/WorldRenderDependencyBoundaryTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0002 | XuanYu.World.Tests/Assets/AssetContractTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0003 | XuanYu.World.Tests/Assets/AssetDialogTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0004 | XuanYu.World.Tests/Assets/GlbImportTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0005 | XuanYu.World.Tests/Assets/HostingCompleteTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0006 | XuanYu.World.Tests/Assets/HostingPlannerRejectTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0007 | XuanYu.World.Tests/Assets/HostingPlannerTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0008 | XuanYu.World.Tests/Assets/HostingRollbackTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0009 | XuanYu.World.Tests/Assets/HostingSaveAsTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0010 | XuanYu.World.Tests/Assets/HostingTransactionTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0011 | XuanYu.World.Tests/Assets/LoadStructureErrorTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0012 | XuanYu.World.Tests/Assets/LoadTransactionTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0013 | XuanYu.World.Tests/Assets/SaveAsTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0014 | XuanYu.World.Tests/Assets/SaveTransactionTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0015 | XuanYu.World.Tests/Assets/SchemaCompatibilityTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0016 | XuanYu.World.Tests/Assets/StaticModelAuthoringServiceTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0017 | XuanYu.World.Tests/Assets/StaticModelBaseVertexTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0018 | XuanYu.World.Tests/Assets/StaticModelCatalogTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0019 | XuanYu.World.Tests/Assets/StaticModelFailureTrackerTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0020 | XuanYu.World.Tests/Assets/StaticModelProjectionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0021 | XuanYu.World.Tests/Assets/StaticModelUiTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0022 | XuanYu.World.Tests/Assets/StaticModelValidatorTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0023 | XuanYu.World.Tests/Camera/CameraC2DraftFramingTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0024 | XuanYu.World.Tests/Camera/CameraC2MapFramingTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0025 | XuanYu.World.Tests/Camera/CameraDocumentTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0026 | XuanYu.World.Tests/Camera/CameraFramingOccupancyTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0027 | XuanYu.World.Tests/Camera/CameraFramingTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0028 | XuanYu.World.Tests/Camera/CameraNavigationUiTests.Focus.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0029 | XuanYu.World.Tests/Camera/CameraOrbitCaptureRegressionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0030 | XuanYu.World.Tests/Camera/EmptySceneCameraContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0031 | XuanYu.World.Tests/Camera/UiViewGizmoTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0032 | XuanYu.World.Tests/Geo/GeographicWorldMappingTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0033 | XuanYu.World.Tests/Logging/EditorLogFilterStateTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0034 | XuanYu.World.Tests/Logging/FootAxamlTailContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0035 | XuanYu.World.Tests/Logging/LogAutoScrollPolicyTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0036 | XuanYu.World.Tests/Logging/LogListAutoScrollControllerContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0037 | XuanYu.World.Tests/Logging/LogPerformanceGovernanceTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0038 | XuanYu.World.Tests/Logging/UiMapLogChineseTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0039 | XuanYu.World.Tests/Logging/UiRootLogRowContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0040 | XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.Behavior.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0041 | XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0042 | XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.Drag.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0043 | XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.Drag.History.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0044 | XuanYu.World.Tests/Map/Editing/UiLayerStateFeedbackTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0045 | XuanYu.World.Tests/Map/Editing/UiLayerVisualContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0046 | XuanYu.World.Tests/Map/Editing/UiLogSummaryPriorityTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0047 | XuanYu.World.Tests/Map/Editing/UiLogSummaryTimingTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0048 | XuanYu.World.Tests/Map/Editing/UiMapCommandRoutingTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0049 | XuanYu.World.Tests/Map/Editing/UiMapDatasetContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0050 | XuanYu.World.Tests/Map/Editing/UiMapDatasetF1AcceptanceTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0051 | XuanYu.World.Tests/Map/Editing/UiMapDatasetF1Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0052 | XuanYu.World.Tests/Map/Editing/UiMapDatasetF2Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0053 | XuanYu.World.Tests/Map/Editing/UiMapDatasetF3ContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0054 | XuanYu.World.Tests/Map/Editing/UiMapDatasetF3Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0055 | XuanYu.World.Tests/Map/Editing/UiMapDatasetLayerR3Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0056 | XuanYu.World.Tests/Map/Editing/UiMapDatasetM04Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0057 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionBootstrapPersistenceTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0058 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionBootstrapTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0059 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionLayerF3Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0060 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0061 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionToolActivationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0062 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionToolInvalidTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0063 | XuanYu.World.Tests/Map/Editing/UiMapEditorTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0064 | XuanYu.World.Tests/Map/Editing/UiMapHistoryTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0065 | XuanYu.World.Tests/Map/Editing/UiMapInitialProjectionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0066 | XuanYu.World.Tests/Map/Editing/UiMapLayerDeleteLockRecoveryTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0067 | XuanYu.World.Tests/Map/Editing/UiMapLayerDragTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0068 | XuanYu.World.Tests/Map/Editing/UiMapLayerLockLogTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0069 | XuanYu.World.Tests/Map/Editing/UiMapLayerPanelTests.Behavior.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0070 | XuanYu.World.Tests/Map/Editing/UiMapLayerPanelTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0071 | XuanYu.World.Tests/Map/Editing/UiMapLayoutContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0072 | XuanYu.World.Tests/Map/Editing/UiMapManifestIdentityTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0073 | XuanYu.World.Tests/Map/Editing/UiMapManifestNavigationTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0074 | XuanYu.World.Tests/Map/MapBoundsTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0075 | XuanYu.World.Tests/Map/MapCoordinateValidationTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0076 | XuanYu.World.Tests/Map/MapDatasetContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0077 | XuanYu.World.Tests/Map/MapDatasetDocumentTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0078 | XuanYu.World.Tests/Map/MapDatasetLayerStateTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0079 | XuanYu.World.Tests/Map/MapDatasetRegistryF1FailureTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0080 | XuanYu.World.Tests/Map/MapDatasetRegistryF2Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0081 | XuanYu.World.Tests/Map/MapDatasetRegistryFailureTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0082 | XuanYu.World.Tests/Map/MapDatasetRegistryLifecycleTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0083 | XuanYu.World.Tests/Map/MapDatasetStorageContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0084 | XuanYu.World.Tests/Map/MapDefaultMapTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0085 | XuanYu.World.Tests/Map/MapDefinitionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0086 | XuanYu.World.Tests/Map/MapDocumentAggregateBridgeTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0087 | XuanYu.World.Tests/Map/MapDocumentOwnerChainTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0088 | XuanYu.World.Tests/Map/MapDocumentOwnerTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0089 | XuanYu.World.Tests/Map/MapEnvironmentValidationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0090 | XuanYu.World.Tests/Map/MapIdTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0091 | XuanYu.World.Tests/Map/MapJsonRoundTripTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0092 | XuanYu.World.Tests/Map/MapJsonStrictnessTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0093 | XuanYu.World.Tests/Map/MapLayerRulesTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0094 | XuanYu.World.Tests/Map/MapLayerStackTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0095 | XuanYu.World.Tests/Map/MapLayerStackTests.Drag.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0096 | XuanYu.World.Tests/Map/MapLayerStackTests.Order.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0097 | XuanYu.World.Tests/Map/MapLayerTests.Base.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0098 | XuanYu.World.Tests/Map/MapLayerTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0099 | XuanYu.World.Tests/Map/MapManifestCreationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0100 | XuanYu.World.Tests/Map/MapManifestSerializationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0101 | XuanYu.World.Tests/Map/MapManifestStorageTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0102 | XuanYu.World.Tests/Map/MapManifestValidationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0103 | XuanYu.World.Tests/Map/MapRegionDatasetContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0104 | XuanYu.World.Tests/Map/MapRegionDatasetRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0105 | XuanYu.World.Tests/Map/MapRegionDraftTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0106 | XuanYu.World.Tests/Map/MapRegionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0107 | XuanYu.World.Tests/Map/MapRegionTests.Geometry.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0108 | XuanYu.World.Tests/Map/MapRegionTests.Strictness.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0109 | XuanYu.World.Tests/Map/MapRoadDatasetContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0110 | XuanYu.World.Tests/Map/MapSizeValidationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0111 | XuanYu.World.Tests/Map/MapStorageFailureTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0112 | XuanYu.World.Tests/Map/MapStorageTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0113 | XuanYu.World.Tests/Map/MapSurfaceSamplerTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0114 | XuanYu.World.Tests/Map/MapSurfaceValidationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0115 | XuanYu.World.Tests/Map/MapWorkingStorageTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0116 | XuanYu.World.Tests/Map/SceneMapReferenceTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0117 | XuanYu.World.Tests/Map/WorldMapStateOwnerTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0118 | XuanYu.World.Tests/Map/WorldMapStateTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0119 | XuanYu.World.Tests/MapEditing/GenericGeometryCapabilityTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0120 | XuanYu.World.Tests/MapEditing/GroundAuthoringContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0121 | XuanYu.World.Tests/MapEditing/MapCoordinateContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0122 | XuanYu.World.Tests/MapEditing/MapEditSessionCommandTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0123 | XuanYu.World.Tests/MapEditing/MapEditSessionCreationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0124 | XuanYu.World.Tests/MapEditing/MapEditSessionDirtyTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0125 | XuanYu.World.Tests/MapEditing/MapEditSessionGeometryTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0126 | XuanYu.World.Tests/MapEditing/MapEditSessionHistoryTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0127 | XuanYu.World.Tests/MapEditing/MapEditSessionMapPropertiesTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0128 | XuanYu.World.Tests/MapEditing/MapEditSessionObjectCommandTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0129 | XuanYu.World.Tests/MapEditing/MapEditSessionRegionStyleTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0130 | XuanYu.World.Tests/MapEditing/MapEditSessionRegionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0131 | XuanYu.World.Tests/MapEditing/MapEditSessionSelectionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0132 | XuanYu.World.Tests/MapEditing/MapEditSessionThreadTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0133 | XuanYu.World.Tests/MapEditing/MapEditSessionValidationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0134 | XuanYu.World.Tests/MapEditing/MapGeometryContextHitTesterTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0135 | XuanYu.World.Tests/MapEditing/MapGeometryContextMenuSpecTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0136 | XuanYu.World.Tests/MapEditing/MapGeometryHitTesterTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0137 | XuanYu.World.Tests/MapEditing/MapObjectNameAllocatorTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0138 | XuanYu.World.Tests/MapEditing/MapPickingRoundTripTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0139 | XuanYu.World.Tests/MapEditing/MapRenderSnapshotProjectionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0140 | XuanYu.World.Tests/MapEditing/MapSurfacePickerTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0141 | XuanYu.World.Tests/MapEditing/PointFeatureFoundationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0142 | XuanYu.World.Tests/MapEditing/PolygonVisualCenterTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0143 | XuanYu.World.Tests/MapEditing/RegionDrawingF3HistoryTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0144 | XuanYu.World.Tests/MapEditing/RegionDrawingStateTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0145 | XuanYu.World.Tests/MapEditing/RegionEdgeSnapGeometryTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0146 | XuanYu.World.Tests/MapEditing/RegionEdgeSnapResolverTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0147 | XuanYu.World.Tests/MapEditing/RegionSnapPipelineContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0148 | XuanYu.World.Tests/MapEditing/RegionSnapPipelineLockTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0149 | XuanYu.World.Tests/MapEditing/RegionSnapPipelineTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0150 | XuanYu.World.Tests/MapEditing/RegionSnapStateTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0151 | XuanYu.World.Tests/MapEditing/RegionSpatialIndexLifecycleTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0152 | XuanYu.World.Tests/MapEditing/RegionSpatialIndexScaleTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0153 | XuanYu.World.Tests/MapEditing/RegionSpatialIndexTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0154 | XuanYu.World.Tests/MapEditing/RegionVertexSnapIntegrationContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0155 | XuanYu.World.Tests/MapEditing/RegionVertexSnapResolverBoundaryTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0156 | XuanYu.World.Tests/MapEditing/RegionVertexSnapResolverTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0157 | XuanYu.World.Tests/MapEditing/RegionVertexSnapScaleTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0158 | XuanYu.World.Tests/MapEditing/RegionVertexSnapStateTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0159 | XuanYu.World.Tests/Mode/AreaAR2AvailabilityContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0160 | XuanYu.World.Tests/Mode/EditorModeManagerTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0161 | XuanYu.World.Tests/Mode/EditorModeUiCompositionTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0162 | XuanYu.World.Tests/Render/ReverseZDepthContractPrototypeTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0163 | XuanYu.World.Tests/Render/ReverseZDepthPipelineContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0164 | XuanYu.World.Tests/Render/TerrainNavigationAllocationTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0165 | XuanYu.World.Tests/Render/TerrainPreviewLightingContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0166 | XuanYu.World.Tests/Render/VulkanPresentLoopContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0167 | XuanYu.World.Tests/Render/VulkanPresentModeSelectionTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0168 | XuanYu.World.Tests/Render/VulkanResizeContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0169 | XuanYu.World.Tests/Render/VulkanSwapchainChurnTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0170 | XuanYu.World.Tests/Render/WorldGridIndependenceContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0171 | XuanYu.World.Tests/Render/WorldGridRenderOriginContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0172 | XuanYu.World.Tests/Render/WorldGridStartupContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0173 | XuanYu.World.Tests/Scene/CommandSmokeTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0174 | XuanYu.World.Tests/Scene/EditorEnvironmentTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0175 | XuanYu.World.Tests/Scene/EntityBoundsSemanticsTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0176 | XuanYu.World.Tests/Scene/EntityRegistryTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0177 | XuanYu.World.Tests/Scene/EntityTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0178 | XuanYu.World.Tests/Scene/FinalSceneTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0179 | XuanYu.World.Tests/Scene/GlobalWorldTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0180 | XuanYu.World.Tests/Scene/SceneConsumptionTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0181 | XuanYu.World.Tests/Scene/SceneDocumentPersistenceTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0182 | XuanYu.World.Tests/Scene/SceneDocumentTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0183 | XuanYu.World.Tests/Scene/SceneDocumentTests.Opening.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0184 | XuanYu.World.Tests/Scene/SceneDocumentTests.SaveFeedback.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0185 | XuanYu.World.Tests/Scene/SceneIsolationTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0186 | XuanYu.World.Tests/Scene/SceneMultiEntityGateTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0187 | XuanYu.World.Tests/Scene/SceneSelectionReentryTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0188 | XuanYu.World.Tests/Scene/SceneSingleAuthorityTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0189 | XuanYu.World.Tests/Scene/UiHistoryTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0190 | XuanYu.World.Tests/Scene/UiHistoryTests.InlineRename.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0191 | XuanYu.World.Tests/Selection/FinalSelectionTests.cs | VALID_PROTECTOR | SUFFICIENT | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0192 | XuanYu.World.Tests/Selection/SelectionToolStateUiTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0193 | XuanYu.World.Tests/Selection/ToolStateHighlightUiTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0194 | XuanYu.World.Tests/Selection/ToolStateHighlightUiTests.Selection.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0195 | XuanYu.World.Tests/Spatial/SceneStateOwnerSpatialTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0196 | XuanYu.World.Tests/Spatial/SpatialIndexEditLifecycleTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0197 | XuanYu.World.Tests/Spatial/SpatialIndexOwnerLifecycleTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0198 | XuanYu.World.Tests/Spatial/SpatialIndexOwnerRevisionTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0199 | XuanYu.World.Tests/Spatial/SpatialIndexRebuildTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0200 | XuanYu.World.Tests/Spatial/SpatialIndexScaleTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0201 | XuanYu.World.Tests/Spatial/SpatialQueryGovernanceTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0202 | XuanYu.World.Tests/Spatial/SpatialQueryTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0203 | XuanYu.World.Tests/Spatial/SpatialRaycastNearestTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0204 | XuanYu.World.Tests/Spatial/SpatialRaycastRevisionTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0205 | XuanYu.World.Tests/Spatial/SpatialRaycastScaleTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0206 | XuanYu.World.Tests/Spatial/SpatialRayQueryLifecycleTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0207 | XuanYu.World.Tests/Spatial/SpatialRayQueryTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0208 | XuanYu.World.Tests/Terrain/Import/EsriAsciiGridTerrainReaderStreamingTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0209 | XuanYu.World.Tests/Terrain/TerrainAuthoringContinuityTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0210 | XuanYu.World.Tests/Terrain/TerrainChunkMeshBuilderTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0211 | XuanYu.World.Tests/Terrain/TerrainChunkNormalTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0212 | XuanYu.World.Tests/Terrain/TerrainChunkPartitionerTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0213 | XuanYu.World.Tests/Terrain/TerrainChunkQueryPreservationTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0214 | XuanYu.World.Tests/Terrain/TerrainCut1UiContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0215 | XuanYu.World.Tests/Terrain/TerrainElevationContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0216 | XuanYu.World.Tests/Terrain/TerrainElevationTileRuntimeTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0217 | XuanYu.World.Tests/Terrain/TerrainFix1NotificationContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0218 | XuanYu.World.Tests/Terrain/TerrainHgtImportContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0219 | XuanYu.World.Tests/Terrain/TerrainHgtWorldIntegrationTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0220 | XuanYu.World.Tests/Terrain/TerrainImportFix1BUiContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0221 | XuanYu.World.Tests/Terrain/TerrainImportProgressContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0222 | XuanYu.World.Tests/Terrain/TerrainMeshNormalTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0223 | XuanYu.World.Tests/Terrain/TerrainMultiSourceImportTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0224 | XuanYu.World.Tests/Terrain/TerrainMultiSourceUiTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0225 | XuanYu.World.Tests/Terrain/TerrainRenderContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0226 | XuanYu.World.Tests/Terrain/TerrainRevisionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0227 | XuanYu.World.Tests/Terrain/TerrainSourceImportTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0228 | XuanYu.World.Tests/Terrain/TerrainTileSetTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0229 | XuanYu.World.Tests/Terrain/TerrainWorldTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0230 | XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0231 | XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.Plane.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0232 | XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.Region.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0233 | XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.Session.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0234 | XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0235 | XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.DragState.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0236 | XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.Preview.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0237 | XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.ToolSwitch.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0238 | XuanYu.World.Tests/Transform/Scale/ScaleGizmoGlobalModeTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0239 | XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.AxisUniform.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0240 | XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0241 | XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.History.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0242 | XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.Target.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0243 | XuanYu.World.Tests/Transform/TransformFoundationTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0244 | XuanYu.World.Tests/Transform/TransformFoundationTests.Input.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0245 | XuanYu.World.Tests/Transform/TransformFoundationTests.Inspector.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0246 | XuanYu.World.Tests/Transform/TransformSessionTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0247 | XuanYu.World.Tests/Transform/ViewportAssistTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0248 | XuanYu.World.Tests/Tree/UiHierarchyConnectorTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0249 | XuanYu.World.Tests/Tree/UiTreeGuideTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0250 | XuanYu.World.Tests/Tree/UiTreeToggleTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0251 | XuanYu.World.Tests/UiRuntime/AreaAR4MenuRuntimeTests.Contracts.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0252 | XuanYu.World.Tests/UiRuntime/AreaAR4MenuRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0253 | XuanYu.World.Tests/UiRuntime/AreaAR5MenuRadioVisualTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0254 | XuanYu.World.Tests/UiRuntime/AreaAR6WorkspaceRadioRenderTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0255 | XuanYu.World.Tests/UiRuntime/AreaBLeftWorkspaceRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0256 | XuanYu.World.Tests/UiRuntime/AreaBLeftWorkspaceRuntimeTests.R2.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0257 | XuanYu.World.Tests/UiRuntime/AreaCR1ContextToolbarRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0258 | XuanYu.World.Tests/UiRuntime/AreaDR1Fix4InspectorRoutingTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0259 | XuanYu.World.Tests/UiRuntime/AreaDR1Fix5RightContentOwnershipTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0260 | XuanYu.World.Tests/UiRuntime/AreaDR2CorrectionInstanceRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0261 | XuanYu.World.Tests/UiRuntime/AreaDR2Fix1MapInspectorRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0262 | XuanYu.World.Tests/UiRuntime/AreaDR2Fix2CompactNavLayerDockRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0263 | XuanYu.World.Tests/UiRuntime/AreaDR2Fix3ProjectionDensityRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0264 | XuanYu.World.Tests/UiRuntime/AreaDR2Fix4RegionInspectorRuntimeTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0265 | XuanYu.World.Tests/UiRuntime/AreaDR2Fix5TabIntegrationRuntimeTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0266 | XuanYu.World.Tests/UiRuntime/AreaDR2NavigationAndMapContextRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0267 | XuanYu.World.Tests/UiRuntime/AreaDR3InspectorPagerRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0268 | XuanYu.World.Tests/UiRuntime/ContextToolbarGeometryRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0269 | XuanYu.World.Tests/UiRuntime/ContextToolbarNativeDismissRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0270 | XuanYu.World.Tests/UiRuntime/ContextToolbarPopupClickTrackingTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0271 | XuanYu.World.Tests/UiRuntime/ContextToolbarPopupDiagnosticIdentityTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0272 | XuanYu.World.Tests/UiRuntime/ContextToolbarPopupHostRuntimeTests.Contracts.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0273 | XuanYu.World.Tests/UiRuntime/ContextToolbarPopupHostRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0274 | XuanYu.World.Tests/UiRuntime/ContextToolbarR2RuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0275 | XuanYu.World.Tests/UiRuntime/DatasetLayerPanelRuntimeLayoutTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0276 | XuanYu.World.Tests/UiRuntime/DiagnosticAutoIdRegressionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0277 | XuanYu.World.Tests/UiRuntime/DiagnosticBoundsRuntimeTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0278 | XuanYu.World.Tests/UiRuntime/DiagnosticCardPlacementTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0279 | XuanYu.World.Tests/UiRuntime/DiagnosticClickToTrackHeadlessTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0280 | XuanYu.World.Tests/UiRuntime/DiagnosticClickToTrackTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0281 | XuanYu.World.Tests/UiRuntime/DiagnosticFix2Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0282 | XuanYu.World.Tests/UiRuntime/DiagnosticFloatingCardXyuiTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0283 | XuanYu.World.Tests/UiRuntime/DiagnosticIdentityTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0284 | XuanYu.World.Tests/UiRuntime/DiagnosticLockedControlNativeMoveTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0285 | XuanYu.World.Tests/UiRuntime/DiagnosticMappedDisplayTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0286 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeCoordinateMappingTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0287 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeDialogEdgeTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0288 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeDialogEdgeTests.Cycles.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0289 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeDialogLifecycleTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0290 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeLockedClickTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0291 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeOverlayRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0292 | XuanYu.World.Tests/UiRuntime/DiagnosticNativePointerProbeTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0293 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeTargetOwnershipTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0294 | XuanYu.World.Tests/UiRuntime/DiagnosticOverlayRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0295 | XuanYu.World.Tests/UiRuntime/DiagnosticOwnerActivationRestoreTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0296 | XuanYu.World.Tests/UiRuntime/DiagnosticOwnerActivationRetryTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0297 | XuanYu.World.Tests/UiRuntime/DiagnosticOwnerActivationWaitTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0298 | XuanYu.World.Tests/UiRuntime/DiagnosticPlacementPolicyEdgesTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0299 | XuanYu.World.Tests/UiRuntime/DiagnosticPlacementPolicyFallbackTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0300 | XuanYu.World.Tests/UiRuntime/DiagnosticPlacementPolicyTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0301 | XuanYu.World.Tests/UiRuntime/DiagnosticPopupBoundsRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0302 | XuanYu.World.Tests/UiRuntime/DiagnosticProbeFix1Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0303 | XuanYu.World.Tests/UiRuntime/DiagnosticProbeInteractionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0304 | XuanYu.World.Tests/UiRuntime/DiagnosticProbeOverlayRuntimeTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0305 | XuanYu.World.Tests/UiRuntime/DiagnosticProbeResolverLocatorTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0306 | XuanYu.World.Tests/UiRuntime/DiagnosticProbeResolverTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0307 | XuanYu.World.Tests/UiRuntime/DiagnosticR1FloatingRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0308 | XuanYu.World.Tests/UiRuntime/DiagnosticR1IdentityCompletionTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0309 | XuanYu.World.Tests/UiRuntime/DiagnosticR1PopupRuntimeTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0310 | XuanYu.World.Tests/UiRuntime/DiagnosticR1ReportTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0311 | XuanYu.World.Tests/UiRuntime/DiagnosticR1SnapshotTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0312 | XuanYu.World.Tests/UiRuntime/DiagnosticRegionSelectionRegressionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0313 | XuanYu.World.Tests/UiRuntime/DiagnosticRegistrationRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0314 | XuanYu.World.Tests/UiRuntime/DiagnosticSnapshotTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0315 | XuanYu.World.Tests/UiRuntime/DiagnosticTargetBoundsRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0316 | XuanYu.World.Tests/UiRuntime/DiagnosticTrackedHideTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0317 | XuanYu.World.Tests/UiRuntime/DiagnosticTrackedIdentityTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0318 | XuanYu.World.Tests/UiRuntime/DiagnosticTrackedLifecycleTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0319 | XuanYu.World.Tests/UiRuntime/DiagnosticViewportInputPassthroughTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0320 | XuanYu.World.Tests/UiRuntime/DiagnosticWorkspaceSelectorIdentityTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0321 | XuanYu.World.Tests/UiRuntime/EngineNavigationLayoutStabilityTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0322 | XuanYu.World.Tests/UiRuntime/EngineNavigationScrollAuthorityTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0323 | XuanYu.World.Tests/UiRuntime/FeatureEditCR1InspectorRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0324 | XuanYu.World.Tests/UiRuntime/FeatureEditCR1RuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0325 | XuanYu.World.Tests/UiRuntime/FeatureEditInspectorContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0326 | XuanYu.World.Tests/UiRuntime/FeatureEditSelectionResetTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0327 | XuanYu.World.Tests/UiRuntime/FeatureEditSelectionResetTests.RoadVertices.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0328 | XuanYu.World.Tests/UiRuntime/FeatureEditUiContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0329 | XuanYu.World.Tests/UiRuntime/FeatureEditWorkflowRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0330 | XuanYu.World.Tests/UiRuntime/GenericMarkerSnapIntegrationTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0331 | XuanYu.World.Tests/UiRuntime/GenericRoadSnapIntegrationTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0332 | XuanYu.World.Tests/UiRuntime/HeadlessInputInfrastructureTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0333 | XuanYu.World.Tests/UiRuntime/InspectorEntityEditTargetTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0334 | XuanYu.World.Tests/UiRuntime/InspectorPropertyMutabilityTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0335 | XuanYu.World.Tests/UiRuntime/InspectorPropertyMutabilityTests.Header.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0336 | XuanYu.World.Tests/UiRuntime/InspectorPropertyMutabilityTests.Navigation.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0337 | XuanYu.World.Tests/UiRuntime/InspectorPropertyNavigationTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0338 | XuanYu.World.Tests/UiRuntime/InspectorPropertyTargetTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0339 | XuanYu.World.Tests/UiRuntime/InspectorRegionColorPreviewTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0340 | XuanYu.World.Tests/UiRuntime/InspectorRegionColorTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0341 | XuanYu.World.Tests/UiRuntime/InspectorSectionRailLayoutRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0342 | XuanYu.World.Tests/UiRuntime/InspectorSectionRailScrollRuntimeTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0343 | XuanYu.World.Tests/UiRuntime/InspectorSelectionContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0344 | XuanYu.World.Tests/UiRuntime/InspectorSingleFocusSectionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0345 | XuanYu.World.Tests/UiRuntime/LayerARuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0346 | XuanYu.World.Tests/UiRuntime/LayerPanelRuntimeLayoutTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0347 | XuanYu.World.Tests/UiRuntime/LayerPanelRuntimeStateTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0348 | XuanYu.World.Tests/UiRuntime/MapLabelRasterizationAlphaRegressionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0349 | XuanYu.World.Tests/UiRuntime/MapLabelRasterizerTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0350 | XuanYu.World.Tests/UiRuntime/MapMarkerInspectorPanelRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0351 | XuanYu.World.Tests/UiRuntime/MapMarkerInspectorPersistenceTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0352 | XuanYu.World.Tests/UiRuntime/MapMarkerInspectorViewportTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0353 | XuanYu.World.Tests/UiRuntime/MapMarkerInspectorWorkflowTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0354 | XuanYu.World.Tests/UiRuntime/MapMarkerPlacementTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0355 | XuanYu.World.Tests/UiRuntime/MapRegionLabelProjectionTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0356 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayAnalyticStrokeRegressionTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0357 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayAnchorContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0358 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayDepthPolicyTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0359 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayStrokeContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0360 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayV1Tests.Colors.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0361 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayV1Tests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0362 | XuanYu.World.Tests/UiRuntime/PointFeatureEntryRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0363 | XuanYu.World.Tests/UiRuntime/R2BPropertyEditorVisualContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0364 | XuanYu.World.Tests/UiRuntime/RegionDrawContextSyncFix1Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0365 | XuanYu.World.Tests/UiRuntime/RegionDrawContextSyncFix1Tests.Lifecycle.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0366 | XuanYu.World.Tests/UiRuntime/RegionDrawContextToolbarRuntimeFix1Tests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0367 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1ActivationRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0368 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1BTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0369 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1BTests.VertexCount.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0370 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1CStabilityTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0371 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1FullRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0372 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1FullRuntimeTests.Names.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0373 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1HeadlessTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0374 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1RenderContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0375 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1ResizeTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0376 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1RuntimeRedTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0377 | XuanYu.World.Tests/UiRuntime/RegionDrawingF2PolygonTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0378 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapInputChainTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0379 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapInputChainTests.Edge.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0380 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0381 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.Edge.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0382 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.History.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0383 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.Persistence.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0384 | XuanYu.World.Tests/UiRuntime/RegionPointerSafetyF2Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0385 | XuanYu.World.Tests/UiRuntime/RightTabsVisibilityRuntimeTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0386 | XuanYu.World.Tests/UiRuntime/RoadDrawingSelectionF1Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0387 | XuanYu.World.Tests/UiRuntime/RoadDrawingSelectionF1Tests.Names.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0388 | XuanYu.World.Tests/UiRuntime/RoadVertexDragD2Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0389 | XuanYu.World.Tests/UiRuntime/RoadVertexSelectionD1Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0390 | XuanYu.World.Tests/UiRuntime/ScaleIndicatorVisibilityRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0391 | XuanYu.World.Tests/UiRuntime/TerrainAutoFrameD1Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0392 | XuanYu.World.Tests/UiRuntime/TerrainAutoFrameD1Tests.Reimport.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0393 | XuanYu.World.Tests/UiRuntime/TerrainAutoFrameLongRangeTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0394 | XuanYu.World.Tests/UiRuntime/TerrainContextLeafHoverFixTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0395 | XuanYu.World.Tests/UiRuntime/TerrainHotpathTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0396 | XuanYu.World.Tests/UiRuntime/TerrainImportOrchestrationR1Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0397 | XuanYu.World.Tests/UiRuntime/TerrainInspectorRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0398 | XuanYu.World.Tests/UiRuntime/TerrainRenderIntegrationTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0399 | XuanYu.World.Tests/UiRuntime/TerrainTopContextRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0400 | XuanYu.World.Tests/UiRuntime/TopLeftInteractionR1Tests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0401 | XuanYu.World.Tests/UiRuntime/TopModeGeometryRuntimeTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0402 | XuanYu.World.Tests/UiRuntime/UiR1VisualContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0403 | XuanYu.World.Tests/UiRuntime/UiR1VisualFixContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0404 | XuanYu.World.Tests/UiRuntime/UiRuntimeRiskTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0405 | XuanYu.World.Tests/UiRuntime/WorkspaceSelectorR2ContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0406 | XuanYu.World.Tests/UiRuntime/XYUI2R2BContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0407 | XuanYu.World.Tests/UiTokens/AreaCR1ContextToolbarContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0408 | XuanYu.World.Tests/UiTokens/CanonicalToolchainResolverTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0409 | XuanYu.World.Tests/UiTokens/EditorScrollAuditContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0410 | XuanYu.World.Tests/UiTokens/InspectorFix1ContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0411 | XuanYu.World.Tests/UiTokens/InspectorFix2ContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0412 | XuanYu.World.Tests/UiTokens/LayerAUiCompositionTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0413 | XuanYu.World.Tests/UiTokens/PointFeatureEntryContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0414 | XuanYu.World.Tests/UiTokens/TerrainTopContextContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0415 | XuanYu.World.Tests/UiTokens/TopWorkspaceSelectorR1Tests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0416 | XuanYu.World.Tests/UiTokens/UiCanonicalVersionContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0417 | XuanYu.World.Tests/UiTokens/UiCloseLifecycleContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0418 | XuanYu.World.Tests/UiTokens/UiCsColorRulesTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0419 | XuanYu.World.Tests/UiTokens/UiD2F1RegionToolActivationContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0420 | XuanYu.World.Tests/UiTokens/UiD2F1RegionToolContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0421 | XuanYu.World.Tests/UiTokens/UiD3DebtClearedTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0422 | XuanYu.World.Tests/UiTokens/UiD4DebtClearedTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0423 | XuanYu.World.Tests/UiTokens/UiD4F1ButtonContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0424 | XuanYu.World.Tests/UiTokens/UiD4F1LayoutModelTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0425 | XuanYu.World.Tests/UiTokens/UiD4F1TextOverflowContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0426 | XuanYu.World.Tests/UiTokens/UiD4F1TypographyContractTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0427 | XuanYu.World.Tests/UiTokens/UiD4InspectorContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0428 | XuanYu.World.Tests/UiTokens/UiD4LayerContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0429 | XuanYu.World.Tests/UiTokens/UiD4LayoutModelTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0430 | XuanYu.World.Tests/UiTokens/UiD4MapEditorContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0431 | XuanYu.World.Tests/UiTokens/UiD5ButtonContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0432 | XuanYu.World.Tests/UiTokens/UiD5CorrectionBehaviorTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0433 | XuanYu.World.Tests/UiTokens/UiD5CorrectionNotifyTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0434 | XuanYu.World.Tests/UiTokens/UiD5CorrectionStructureTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0435 | XuanYu.World.Tests/UiTokens/UiD5DangerFlowTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0436 | XuanYu.World.Tests/UiTokens/UiD5DialogAndLogContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0437 | XuanYu.World.Tests/UiTokens/UiD5FormContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0438 | XuanYu.World.Tests/UiTokens/UiD5InputValidationTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0439 | XuanYu.World.Tests/UiTokens/UiD5MapStatusTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0440 | XuanYu.World.Tests/UiTokens/UiD5NotificationTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0441 | XuanYu.World.Tests/UiTokens/UiD5UnsavedDialogBehaviorTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0442 | XuanYu.World.Tests/UiTokens/UiD5UnsavedDialogTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0443 | XuanYu.World.Tests/UiTokens/UiD5UnsavedFlowTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0444 | XuanYu.World.Tests/UiTokens/UiD6AccessibilityContractTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0445 | XuanYu.World.Tests/UiTokens/UiD6DpiContractTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0446 | XuanYu.World.Tests/UiTokens/UiD6LogPerformanceTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0447 | XuanYu.World.Tests/UiTokens/UiD6MotionContractTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0448 | XuanYu.World.Tests/UiTokens/UiDebtBaselineBypassF2Tests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0449 | XuanYu.World.Tests/UiTokens/UiDebtBaselineBypassTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0450 | XuanYu.World.Tests/UiTokens/UiDebtBaselineTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0451 | XuanYu.World.Tests/UiTokens/UiF3LayerRowContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0452 | XuanYu.World.Tests/UiTokens/UiLayerDeleteDialogContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0453 | XuanYu.World.Tests/UiTokens/UiR1FinalLeftTopContractTests.cs | VALID_PROTECTOR | SUFFICIENT | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0454 | XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzerTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0455 | XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzerTokenRefTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0456 | XuanYu.World.Tests/UiTokens/UiTokenManifestGraphTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0457 | XuanYu.World.Tests/UiTokens/UiTokenManifestTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0458 | XuanYu.World.Tests/UiTokens/UiTopTabStripContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0459 | XuanYu.World.Tests/UiTokens/UiTopTabStripModelHintAndListTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0460 | XuanYu.World.Tests/UiTokens/UiTopTabStripModelTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0461 | XuanYu.World.Tests/UiTokens/XyeToolbarTextPrimitiveContractTests.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0462 | XuanYu.World.Tests/Viewport/AvaloniaPointerEventAdapterTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0463 | XuanYu.World.Tests/Viewport/InputIntegration/AvaloniaViewportInputCutoverTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0464 | XuanYu.World.Tests/Viewport/InputIntegration/CameraWheelAdapterIntegrationTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0465 | XuanYu.World.Tests/Viewport/InputIntegration/CameraWheelInputIntegrationTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0466 | XuanYu.World.Tests/Viewport/InputIntegration/ConsumerArbitrationIntegrationTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0467 | XuanYu.World.Tests/Viewport/InputIntegration/ConsumerLifecycleIntegrationTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0468 | XuanYu.World.Tests/Viewport/InputIntegration/D1ConsumerCancellationTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0469 | XuanYu.World.Tests/Viewport/InputIntegration/D1ConsumerMigrationTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0470 | XuanYu.World.Tests/Viewport/InputIntegration/NativeKeyboardInputTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0471 | XuanYu.World.Tests/Viewport/InputIntegration/NativeViewportCoordinateContractTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0472 | XuanYu.World.Tests/Viewport/InputIntegration/NativeViewportInputForwarderTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0473 | XuanYu.World.Tests/Viewport/InputIntegration/NavigationGizmoConsumerContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0474 | XuanYu.World.Tests/Viewport/InputIntegration/PointerSemanticClosureTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0475 | XuanYu.World.Tests/Viewport/InputIntegration/ProductionInputCompositionTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0476 | XuanYu.World.Tests/Viewport/InputIntegration/ViewportInputConvergenceIntegrationTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0477 | XuanYu.World.Tests/Viewport/InputLifecycle/ViewportGestureTerminalTests.cs | VALID_CONTRACT_TEST | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0478 | XuanYu.World.Tests/Viewport/MapContextMenuRouterTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0479 | XuanYu.World.Tests/Viewport/MapEditingTemporaryStateTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0480 | XuanYu.World.Tests/Viewport/MapInputCancellationIntegrationTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0481 | XuanYu.World.Tests/Viewport/MapInputConsumerContractTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0482 | XuanYu.World.Tests/Viewport/MapInputConsumerRegressionTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0483 | XuanYu.World.Tests/Viewport/NativePointerEventAdapterTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0484 | XuanYu.World.Tests/Viewport/NativePointerRoutePolicyTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0485 | XuanYu.World.Tests/Viewport/PlatformInputParity/NativeSourceParityTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0486 | XuanYu.World.Tests/Viewport/RegionDrawingInputModifierTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0487 | XuanYu.World.Tests/Viewport/UnifiedPointerModelTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0488 | XuanYu.World.Tests/Viewport/UnifiedPointerReadinessContractTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0489 | XuanYu.World.Tests/Viewport/ViewportInputRouterDispatchTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0490 | XuanYu.World.Tests/Viewport/ViewportInputRouterMapArbitrationTests.cs | VALID_PROTECTOR | PARTIAL | CAPABILITY_COVERAGE_GAP | RENAMED_AND_RECLAIMED |
| TR-B-0491 | XuanYu.World.Tests/Workspace/EditorWorkspaceManagerTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0492 | XuanYu.World.Tests/Workspace/EditorWorkspaceUiCompositionTests.cs | VALID_PROTECTOR | PARTIAL | NONE | RETAINED |
| TR-B-0493 | XuanYu.World.Tests/Workspace/EditorWorkspaceUiTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0494 | XuanYu.World.Tests/Workspace/RegionAuthoringHierarchyTests.cs | VALID_CONTRACT_TEST | PARTIAL | NONE | RETAINED |
| TR-B-0495 | XuanYu.World.Tests/WorldPartition/WorldPartitionInvariantTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0496 | XuanYu.World.Tests/WorldPartition/WorldPartitionMigrationTests.Activity.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0497 | XuanYu.World.Tests/WorldPartition/WorldPartitionMigrationTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0498 | XuanYu.World.Tests/WorldPartition/WorldPartitionTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |
| TR-B-0499 | XuanYu.World.Tests/WorldPartition/WorldPartitionTests.PartitionStrategy.cs | VALID_PROTECTOR | SUFFICIENT | NONE | RETAINED |
| TR-B-0500 | XuanYu.World.Tests/WorldPartition/WorldPartitionUiTests.cs | VALID_CONTRACT_TEST | SUFFICIENT | NONE | RETAINED |

