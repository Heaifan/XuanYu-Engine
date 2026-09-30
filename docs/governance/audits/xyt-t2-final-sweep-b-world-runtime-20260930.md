# XYT-T2 / FINAL-SWEEP / B-WORLD-RUNTIME — Truth Records

Date: 2026-09-30  
Workspace: `E:\MyDoc\project-VSCode\XuanYuEngine`  
HEAD: `45fd7e636bfeb74701b4d192cfbedd6f4ddeb1d5`  
Scope: `XuanYu.World.Tests/**/*.cs`, excluding the 7 T-A-record files and the 3 A/C/D ownership test files resolved from `tools/handoff/ownership-manifest.json`.

## Result

B scope contains **500 file-level Truth Records** and **0 unreviewed files**. The existing legacy audit defines the file as the audit unit; this report preserves that unit and does not modify `docs/governance/xyt-test-truth-registry.json`.

This is a truth review, not a product closeout:

- `dotnet test XuanYu.World.Tests --no-restore`: 2154 total, 2150 passed, 4 failed.
- The failures are Headless/UI Runtime execution facts, not proof of native Win32, real Avalonia window, Vulkan/GPU, or human acceptance.
- No code, test, Registry, ownership manifest, commit, or push was changed by this sweep.
- `NotNull`, `NotEmpty`, and bare `True` were not accepted as business-capability oracles. They are recorded as guard evidence unless paired with semantic value/state assertions.
- Critical-capability records are marked `SENSITIVE_OR_ESCALATE`; no record upgrades direct UiVm, synthetic Headless input, adapter/router, real Avalonia control, native Win32 input, Vulkan runtime, or human acceptance into another tier.

## Truth decision summary

| Class | Count | Meaning |
|---|---:|---|
| VERIFIED / KEEP | 420 | Bounded local logic, state, import, composition, or source contract; evidence tier remains T0/T1/T2. |
| TIER_OVERCLAIM / RENAME | 71 | Runtime/Real/Integration/Viewport/Acceptance/Final/Visible/Stable wording exceeds observed path. |
| MISLEADING_CLAIM / RENAME | 9 | Overclaiming name plus static source/shape assertions; no runtime object or frame oracle. |
| RED-sensitive / ESCALATE marker | 420 marked sensitive-or-escalate; 80 explicit ESCALATE | Critical capabilities retain RED-sensitive-or-escalate status; no mutation witness was fabricated. |
| DUPLICATE / RETIRE | 0 asserted automatically | No retirement was inferred from name/count; duplicates require semantic pair evidence. |

## Evidence boundaries

| Observed path | Truth tier | Does not prove |
|---|---|---|
| Direct UiVm call / pure state | T1/T2 | Real control, native input, Vulkan, visible pixels |
| Avalonia Headless / synthetic input | T2 | HWND, NativeControlHost, Win32, GPU, swapchain, present |
| Adapter/router composition | T2 | Native source delivery or human interaction |
| Real Avalonia control | T3 only if actual real window path is exercised | Vulkan/GPU/product acceptance |
| Native Win32 input | T3 | Human acceptance |
| Real Vulkan runtime/GPU | T3 | Final visual/product acceptance |
| Human acceptance | T4 | Only the explicitly accepted IPO scope |

## Helper / Fixture / Host / Fake review

31 helper-named files were inspected: 21 contain test-support assertions/attributes, and 10 explicitly expose Fake/Mock/Stub/Synthetic/InMemory/Headless/TestHost vocabulary. None was promoted to a business capability oracle merely because a helper returned non-null, non-empty, or true. The helper layer remains evidence plumbing; claims are bounded by the caller's actual path.

## Current execution incident

The full run produced 4 failures. One confirmed failure evidence is:

`XuanYu.World.Tests/UiRuntime/UiRuntimeRiskTests.cs:20-25` — Headless fixture route throws `Sequence contains no matching element` while locating a visual item. This is a Headless composition/fixture or product-tree discrepancy requiring separate diagnosis; it is not evidence of native-window or product-acceptance failure. The other 3 failures are retained as aggregate run facts and are not silently attributed to individual records.

## Complete B Truth Records

| Record | File | Attributed tests | Tier | TruthStatus | Decision | RED rule | Oracle treatment | Execution |
|---|---|---:|---|---|---|---|---|---|
| TR-B-0001 | XuanYu.World.Tests/Architecture/WorldRenderDependencyBoundaryTests.cs | 2 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0002 | XuanYu.World.Tests/Assets/AssetContractTests.cs | 4 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0003 | XuanYu.World.Tests/Assets/AssetDialogTests.cs | 5 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0004 | XuanYu.World.Tests/Assets/GlbImportTests.cs | 9 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0005 | XuanYu.World.Tests/Assets/HostingCompleteTests.cs | 4 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0006 | XuanYu.World.Tests/Assets/HostingPlannerRejectTests.cs | 5 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0007 | XuanYu.World.Tests/Assets/HostingPlannerTests.cs | 5 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0008 | XuanYu.World.Tests/Assets/HostingRollbackTests.cs | 5 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0009 | XuanYu.World.Tests/Assets/HostingSaveAsTests.cs | 2 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0010 | XuanYu.World.Tests/Assets/HostingTransactionTests.cs | 6 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0011 | XuanYu.World.Tests/Assets/LoadStructureErrorTests.cs | 4 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0012 | XuanYu.World.Tests/Assets/LoadTransactionTests.cs | 4 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0013 | XuanYu.World.Tests/Assets/SaveAsTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0014 | XuanYu.World.Tests/Assets/SaveTransactionTests.cs | 4 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0015 | XuanYu.World.Tests/Assets/SchemaCompatibilityTests.cs | 7 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0016 | XuanYu.World.Tests/Assets/StaticModelAuthoringServiceTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0017 | XuanYu.World.Tests/Assets/StaticModelBaseVertexTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0018 | XuanYu.World.Tests/Assets/StaticModelCatalogTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0019 | XuanYu.World.Tests/Assets/StaticModelFailureTrackerTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0020 | XuanYu.World.Tests/Assets/StaticModelProjectionTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0021 | XuanYu.World.Tests/Assets/StaticModelUiTests.cs | 5 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0022 | XuanYu.World.Tests/Assets/StaticModelValidatorTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0023 | XuanYu.World.Tests/Camera/CameraC2DraftFramingTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0024 | XuanYu.World.Tests/Camera/CameraC2MapFramingTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0025 | XuanYu.World.Tests/Camera/CameraDocumentTests.cs | 2 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0026 | XuanYu.World.Tests/Camera/CameraFramingOccupancyTests.cs | 1 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0027 | XuanYu.World.Tests/Camera/CameraFramingTests.cs | 4 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0028 | XuanYu.World.Tests/Camera/CameraNavigationUiTests.Focus.cs | 1 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0029 | XuanYu.World.Tests/Camera/CameraOrbitCaptureRegressionTests.cs | 3 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0030 | XuanYu.World.Tests/Camera/EmptySceneCameraContractTests.cs | 8 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0031 | XuanYu.World.Tests/Camera/UiViewGizmoTests.cs | 3 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0032 | XuanYu.World.Tests/Geo/GeographicWorldMappingTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0033 | XuanYu.World.Tests/Logging/EditorLogFilterStateTests.cs | 4 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0034 | XuanYu.World.Tests/Logging/FootAxamlTailContractTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0035 | XuanYu.World.Tests/Logging/LogAutoScrollPolicyTests.cs | 5 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0036 | XuanYu.World.Tests/Logging/LogListAutoScrollControllerContractTests.cs | 9 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0037 | XuanYu.World.Tests/Logging/LogPerformanceGovernanceTests.cs | 7 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0038 | XuanYu.World.Tests/Logging/UiMapLogChineseTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0039 | XuanYu.World.Tests/Logging/UiRootLogRowContractTests.cs | 6 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0040 | XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.Behavior.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0041 | XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0042 | XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.Drag.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0043 | XuanYu.World.Tests/Map/Editing/MapLayerSessionTests.Drag.History.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0044 | XuanYu.World.Tests/Map/Editing/UiLayerStateFeedbackTests.cs | 5 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0045 | XuanYu.World.Tests/Map/Editing/UiLayerVisualContractTests.cs | 6 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0046 | XuanYu.World.Tests/Map/Editing/UiLogSummaryPriorityTests.cs | 6 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0047 | XuanYu.World.Tests/Map/Editing/UiLogSummaryTimingTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0048 | XuanYu.World.Tests/Map/Editing/UiMapCommandRoutingTests.cs | 8 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0049 | XuanYu.World.Tests/Map/Editing/UiMapDatasetContractTests.cs | 3 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0050 | XuanYu.World.Tests/Map/Editing/UiMapDatasetF1AcceptanceTests.cs | 2 | T0 | MISLEADING_CLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0051 | XuanYu.World.Tests/Map/Editing/UiMapDatasetF1Tests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0052 | XuanYu.World.Tests/Map/Editing/UiMapDatasetF2Tests.cs | 4 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0053 | XuanYu.World.Tests/Map/Editing/UiMapDatasetF3ContractTests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0054 | XuanYu.World.Tests/Map/Editing/UiMapDatasetF3Tests.cs | 4 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0055 | XuanYu.World.Tests/Map/Editing/UiMapDatasetLayerR3Tests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0056 | XuanYu.World.Tests/Map/Editing/UiMapDatasetM04Tests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0057 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionBootstrapPersistenceTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0058 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionBootstrapTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0059 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionLayerF3Tests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0060 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0061 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionToolActivationTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0062 | XuanYu.World.Tests/Map/Editing/UiMapDatasetRegionToolInvalidTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0063 | XuanYu.World.Tests/Map/Editing/UiMapEditorTests.cs | 7 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0064 | XuanYu.World.Tests/Map/Editing/UiMapHistoryTests.cs | 4 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0065 | XuanYu.World.Tests/Map/Editing/UiMapInitialProjectionTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0066 | XuanYu.World.Tests/Map/Editing/UiMapLayerDeleteLockRecoveryTests.cs | 4 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0067 | XuanYu.World.Tests/Map/Editing/UiMapLayerDragTests.cs | 7 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0068 | XuanYu.World.Tests/Map/Editing/UiMapLayerLockLogTests.cs | 7 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0069 | XuanYu.World.Tests/Map/Editing/UiMapLayerPanelTests.Behavior.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0070 | XuanYu.World.Tests/Map/Editing/UiMapLayerPanelTests.cs | 6 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0071 | XuanYu.World.Tests/Map/Editing/UiMapLayoutContractTests.cs | 6 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0072 | XuanYu.World.Tests/Map/Editing/UiMapManifestIdentityTests.cs | 3 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0073 | XuanYu.World.Tests/Map/Editing/UiMapManifestNavigationTests.cs | 2 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0074 | XuanYu.World.Tests/Map/MapBoundsTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0075 | XuanYu.World.Tests/Map/MapCoordinateValidationTests.cs | 8 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0076 | XuanYu.World.Tests/Map/MapDatasetContractTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0077 | XuanYu.World.Tests/Map/MapDatasetDocumentTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0078 | XuanYu.World.Tests/Map/MapDatasetLayerStateTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0079 | XuanYu.World.Tests/Map/MapDatasetRegistryF1FailureTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0080 | XuanYu.World.Tests/Map/MapDatasetRegistryF2Tests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0081 | XuanYu.World.Tests/Map/MapDatasetRegistryFailureTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0082 | XuanYu.World.Tests/Map/MapDatasetRegistryLifecycleTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0083 | XuanYu.World.Tests/Map/MapDatasetStorageContractTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0084 | XuanYu.World.Tests/Map/MapDefaultMapTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0085 | XuanYu.World.Tests/Map/MapDefinitionTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0086 | XuanYu.World.Tests/Map/MapDocumentAggregateBridgeTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0087 | XuanYu.World.Tests/Map/MapDocumentOwnerChainTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0088 | XuanYu.World.Tests/Map/MapDocumentOwnerTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0089 | XuanYu.World.Tests/Map/MapEnvironmentValidationTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0090 | XuanYu.World.Tests/Map/MapIdTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0091 | XuanYu.World.Tests/Map/MapJsonRoundTripTests.cs | 9 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0092 | XuanYu.World.Tests/Map/MapJsonStrictnessTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0093 | XuanYu.World.Tests/Map/MapLayerRulesTests.cs | 9 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0094 | XuanYu.World.Tests/Map/MapLayerStackTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0095 | XuanYu.World.Tests/Map/MapLayerStackTests.Drag.cs | 8 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0096 | XuanYu.World.Tests/Map/MapLayerStackTests.Order.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0097 | XuanYu.World.Tests/Map/MapLayerTests.Base.cs | 9 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0098 | XuanYu.World.Tests/Map/MapLayerTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0099 | XuanYu.World.Tests/Map/MapManifestCreationTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0100 | XuanYu.World.Tests/Map/MapManifestSerializationTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0101 | XuanYu.World.Tests/Map/MapManifestStorageTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0102 | XuanYu.World.Tests/Map/MapManifestValidationTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0103 | XuanYu.World.Tests/Map/MapRegionDatasetContractTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0104 | XuanYu.World.Tests/Map/MapRegionDatasetRuntimeTests.cs | 2 | T0 | MISLEADING_CLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0105 | XuanYu.World.Tests/Map/MapRegionDraftTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0106 | XuanYu.World.Tests/Map/MapRegionTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0107 | XuanYu.World.Tests/Map/MapRegionTests.Geometry.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0108 | XuanYu.World.Tests/Map/MapRegionTests.Strictness.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0109 | XuanYu.World.Tests/Map/MapRoadDatasetContractTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0110 | XuanYu.World.Tests/Map/MapSizeValidationTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0111 | XuanYu.World.Tests/Map/MapStorageFailureTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0112 | XuanYu.World.Tests/Map/MapStorageTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0113 | XuanYu.World.Tests/Map/MapSurfaceSamplerTests.cs | 8 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0114 | XuanYu.World.Tests/Map/MapSurfaceValidationTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0115 | XuanYu.World.Tests/Map/MapWorkingStorageTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0116 | XuanYu.World.Tests/Map/SceneMapReferenceTests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0117 | XuanYu.World.Tests/Map/WorldMapStateOwnerTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0118 | XuanYu.World.Tests/Map/WorldMapStateTests.cs | 8 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0119 | XuanYu.World.Tests/MapEditing/GenericGeometryCapabilityTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0120 | XuanYu.World.Tests/MapEditing/GroundAuthoringContractTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0121 | XuanYu.World.Tests/MapEditing/MapCoordinateContractTests.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0122 | XuanYu.World.Tests/MapEditing/MapEditSessionCommandTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0123 | XuanYu.World.Tests/MapEditing/MapEditSessionCreationTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0124 | XuanYu.World.Tests/MapEditing/MapEditSessionDirtyTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0125 | XuanYu.World.Tests/MapEditing/MapEditSessionGeometryTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0126 | XuanYu.World.Tests/MapEditing/MapEditSessionHistoryTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0127 | XuanYu.World.Tests/MapEditing/MapEditSessionMapPropertiesTests.cs | 8 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0128 | XuanYu.World.Tests/MapEditing/MapEditSessionObjectCommandTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0129 | XuanYu.World.Tests/MapEditing/MapEditSessionRegionStyleTests.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0130 | XuanYu.World.Tests/MapEditing/MapEditSessionRegionTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0131 | XuanYu.World.Tests/MapEditing/MapEditSessionSelectionTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0132 | XuanYu.World.Tests/MapEditing/MapEditSessionThreadTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0133 | XuanYu.World.Tests/MapEditing/MapEditSessionValidationTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0134 | XuanYu.World.Tests/MapEditing/MapGeometryContextHitTesterTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0135 | XuanYu.World.Tests/MapEditing/MapGeometryContextMenuSpecTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0136 | XuanYu.World.Tests/MapEditing/MapGeometryHitTesterTests.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0137 | XuanYu.World.Tests/MapEditing/MapObjectNameAllocatorTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0138 | XuanYu.World.Tests/MapEditing/MapPickingRoundTripTests.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0139 | XuanYu.World.Tests/MapEditing/MapRenderSnapshotProjectionTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0140 | XuanYu.World.Tests/MapEditing/MapSurfacePickerTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0141 | XuanYu.World.Tests/MapEditing/PointFeatureFoundationTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0142 | XuanYu.World.Tests/MapEditing/PolygonVisualCenterTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0143 | XuanYu.World.Tests/MapEditing/RegionDrawingF3HistoryTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0144 | XuanYu.World.Tests/MapEditing/RegionDrawingStateTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0145 | XuanYu.World.Tests/MapEditing/RegionEdgeSnapGeometryTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0146 | XuanYu.World.Tests/MapEditing/RegionEdgeSnapResolverTests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0147 | XuanYu.World.Tests/MapEditing/RegionSnapPipelineContractTests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0148 | XuanYu.World.Tests/MapEditing/RegionSnapPipelineLockTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0149 | XuanYu.World.Tests/MapEditing/RegionSnapPipelineTests.cs | 6 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0150 | XuanYu.World.Tests/MapEditing/RegionSnapStateTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0151 | XuanYu.World.Tests/MapEditing/RegionSpatialIndexLifecycleTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0152 | XuanYu.World.Tests/MapEditing/RegionSpatialIndexScaleTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0153 | XuanYu.World.Tests/MapEditing/RegionSpatialIndexTests.cs | 6 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0154 | XuanYu.World.Tests/MapEditing/RegionVertexSnapIntegrationContractTests.cs | 4 | T0 | MISLEADING_CLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0155 | XuanYu.World.Tests/MapEditing/RegionVertexSnapResolverBoundaryTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0156 | XuanYu.World.Tests/MapEditing/RegionVertexSnapResolverTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0157 | XuanYu.World.Tests/MapEditing/RegionVertexSnapScaleTests.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0158 | XuanYu.World.Tests/MapEditing/RegionVertexSnapStateTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0159 | XuanYu.World.Tests/Mode/AreaAR2AvailabilityContractTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0160 | XuanYu.World.Tests/Mode/EditorModeManagerTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0161 | XuanYu.World.Tests/Mode/EditorModeUiCompositionTests.cs | 8 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0162 | XuanYu.World.Tests/Render/ReverseZDepthContractPrototypeTests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0163 | XuanYu.World.Tests/Render/ReverseZDepthPipelineContractTests.cs | 7 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0164 | XuanYu.World.Tests/Render/TerrainNavigationAllocationTests.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0165 | XuanYu.World.Tests/Render/TerrainPreviewLightingContractTests.cs | 1 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0166 | XuanYu.World.Tests/Render/VulkanPresentLoopContractTests.cs | 5 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0167 | XuanYu.World.Tests/Render/VulkanPresentModeSelectionTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0168 | XuanYu.World.Tests/Render/VulkanResizeContractTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0169 | XuanYu.World.Tests/Render/VulkanSwapchainChurnTests.cs | 2 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0170 | XuanYu.World.Tests/Render/WorldGridIndependenceContractTests.cs | 1 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0171 | XuanYu.World.Tests/Render/WorldGridRenderOriginContractTests.cs | 2 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0172 | XuanYu.World.Tests/Render/WorldGridStartupContractTests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0173 | XuanYu.World.Tests/Scene/CommandSmokeTests.cs | 3 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0174 | XuanYu.World.Tests/Scene/EditorEnvironmentTests.cs | 2 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0175 | XuanYu.World.Tests/Scene/EntityBoundsSemanticsTests.cs | 2 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0176 | XuanYu.World.Tests/Scene/EntityRegistryTests.cs | 4 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0177 | XuanYu.World.Tests/Scene/EntityTests.cs | 4 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0178 | XuanYu.World.Tests/Scene/FinalSceneTests.cs | 2 | T1 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0179 | XuanYu.World.Tests/Scene/GlobalWorldTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0180 | XuanYu.World.Tests/Scene/SceneConsumptionTests.cs | 4 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0181 | XuanYu.World.Tests/Scene/SceneDocumentPersistenceTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0182 | XuanYu.World.Tests/Scene/SceneDocumentTests.cs | 3 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0183 | XuanYu.World.Tests/Scene/SceneDocumentTests.Opening.cs | 2 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0184 | XuanYu.World.Tests/Scene/SceneDocumentTests.SaveFeedback.cs | 5 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0185 | XuanYu.World.Tests/Scene/SceneIsolationTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0186 | XuanYu.World.Tests/Scene/SceneMultiEntityGateTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0187 | XuanYu.World.Tests/Scene/SceneSelectionReentryTests.cs | 4 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0188 | XuanYu.World.Tests/Scene/SceneSingleAuthorityTests.cs | 6 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0189 | XuanYu.World.Tests/Scene/UiHistoryTests.cs | 4 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0190 | XuanYu.World.Tests/Scene/UiHistoryTests.InlineRename.cs | 3 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0191 | XuanYu.World.Tests/Selection/FinalSelectionTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0192 | XuanYu.World.Tests/Selection/SelectionToolStateUiTests.cs | 5 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0193 | XuanYu.World.Tests/Selection/ToolStateHighlightUiTests.cs | 3 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0194 | XuanYu.World.Tests/Selection/ToolStateHighlightUiTests.Selection.cs | 1 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0195 | XuanYu.World.Tests/Spatial/SceneStateOwnerSpatialTests.cs | 3 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0196 | XuanYu.World.Tests/Spatial/SpatialIndexEditLifecycleTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0197 | XuanYu.World.Tests/Spatial/SpatialIndexOwnerLifecycleTests.cs | 3 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0198 | XuanYu.World.Tests/Spatial/SpatialIndexOwnerRevisionTests.cs | 2 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0199 | XuanYu.World.Tests/Spatial/SpatialIndexRebuildTests.cs | 2 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0200 | XuanYu.World.Tests/Spatial/SpatialIndexScaleTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0201 | XuanYu.World.Tests/Spatial/SpatialQueryGovernanceTests.cs | 1 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0202 | XuanYu.World.Tests/Spatial/SpatialQueryTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0203 | XuanYu.World.Tests/Spatial/SpatialRaycastNearestTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0204 | XuanYu.World.Tests/Spatial/SpatialRaycastRevisionTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0205 | XuanYu.World.Tests/Spatial/SpatialRaycastScaleTests.cs | 2 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0206 | XuanYu.World.Tests/Spatial/SpatialRayQueryLifecycleTests.cs | 2 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0207 | XuanYu.World.Tests/Spatial/SpatialRayQueryTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0208 | XuanYu.World.Tests/Terrain/Import/EsriAsciiGridTerrainReaderStreamingTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0209 | XuanYu.World.Tests/Terrain/TerrainAuthoringContinuityTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0210 | XuanYu.World.Tests/Terrain/TerrainChunkMeshBuilderTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0211 | XuanYu.World.Tests/Terrain/TerrainChunkNormalTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0212 | XuanYu.World.Tests/Terrain/TerrainChunkPartitionerTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0213 | XuanYu.World.Tests/Terrain/TerrainChunkQueryPreservationTests.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0214 | XuanYu.World.Tests/Terrain/TerrainCut1UiContractTests.cs | 3 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0215 | XuanYu.World.Tests/Terrain/TerrainElevationContractTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0216 | XuanYu.World.Tests/Terrain/TerrainElevationTileRuntimeTests.cs | 3 | T1 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0217 | XuanYu.World.Tests/Terrain/TerrainFix1NotificationContractTests.cs | 3 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0218 | XuanYu.World.Tests/Terrain/TerrainHgtImportContractTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0219 | XuanYu.World.Tests/Terrain/TerrainHgtWorldIntegrationTests.cs | 3 | T1 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0220 | XuanYu.World.Tests/Terrain/TerrainImportFix1BUiContractTests.cs | 3 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0221 | XuanYu.World.Tests/Terrain/TerrainImportProgressContractTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0222 | XuanYu.World.Tests/Terrain/TerrainMeshNormalTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0223 | XuanYu.World.Tests/Terrain/TerrainMultiSourceImportTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0224 | XuanYu.World.Tests/Terrain/TerrainMultiSourceUiTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0225 | XuanYu.World.Tests/Terrain/TerrainRenderContractTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0226 | XuanYu.World.Tests/Terrain/TerrainRevisionTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0227 | XuanYu.World.Tests/Terrain/TerrainSourceImportTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0228 | XuanYu.World.Tests/Terrain/TerrainTileSetTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0229 | XuanYu.World.Tests/Terrain/TerrainWorldTests.cs | 7 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0230 | XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.cs | 2 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0231 | XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.Plane.cs | 2 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0232 | XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.Region.cs | 2 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0233 | XuanYu.World.Tests/Transform/Move/MoveTransformUiTests.Session.cs | 5 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0234 | XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.cs | 2 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0235 | XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.DragState.cs | 2 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0236 | XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.Preview.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0237 | XuanYu.World.Tests/Transform/Rotate/RotateTransformUiTests.ToolSwitch.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0238 | XuanYu.World.Tests/Transform/Scale/ScaleGizmoGlobalModeTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0239 | XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.AxisUniform.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0240 | XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.cs | 4 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0241 | XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.History.cs | 2 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0242 | XuanYu.World.Tests/Transform/Scale/ScaleTransformUiTests.Target.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0243 | XuanYu.World.Tests/Transform/TransformFoundationTests.cs | 3 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0244 | XuanYu.World.Tests/Transform/TransformFoundationTests.Input.cs | 5 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0245 | XuanYu.World.Tests/Transform/TransformFoundationTests.Inspector.cs | 5 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0246 | XuanYu.World.Tests/Transform/TransformSessionTests.cs | 5 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0247 | XuanYu.World.Tests/Transform/ViewportAssistTests.cs | 2 | T0 | MISLEADING_CLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0248 | XuanYu.World.Tests/Tree/UiHierarchyConnectorTests.cs | 4 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0249 | XuanYu.World.Tests/Tree/UiTreeGuideTests.cs | 3 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0250 | XuanYu.World.Tests/Tree/UiTreeToggleTests.cs | 4 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0251 | XuanYu.World.Tests/UiRuntime/AreaAR4MenuRuntimeTests.Contracts.cs | 5 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0252 | XuanYu.World.Tests/UiRuntime/AreaAR4MenuRuntimeTests.cs | 5 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0253 | XuanYu.World.Tests/UiRuntime/AreaAR5MenuRadioVisualTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0254 | XuanYu.World.Tests/UiRuntime/AreaAR6WorkspaceRadioRenderTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0255 | XuanYu.World.Tests/UiRuntime/AreaBLeftWorkspaceRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0256 | XuanYu.World.Tests/UiRuntime/AreaBLeftWorkspaceRuntimeTests.R2.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0257 | XuanYu.World.Tests/UiRuntime/AreaCR1ContextToolbarRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0258 | XuanYu.World.Tests/UiRuntime/AreaDR1Fix4InspectorRoutingTests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0259 | XuanYu.World.Tests/UiRuntime/AreaDR1Fix5RightContentOwnershipTests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0260 | XuanYu.World.Tests/UiRuntime/AreaDR2CorrectionInstanceRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0261 | XuanYu.World.Tests/UiRuntime/AreaDR2Fix1MapInspectorRuntimeTests.cs | 5 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0262 | XuanYu.World.Tests/UiRuntime/AreaDR2Fix2CompactNavLayerDockRuntimeTests.cs | 4 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0263 | XuanYu.World.Tests/UiRuntime/AreaDR2Fix3ProjectionDensityRuntimeTests.cs | 5 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0264 | XuanYu.World.Tests/UiRuntime/AreaDR2Fix4RegionInspectorRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0265 | XuanYu.World.Tests/UiRuntime/AreaDR2Fix5TabIntegrationRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0266 | XuanYu.World.Tests/UiRuntime/AreaDR2NavigationAndMapContextRuntimeTests.cs | 5 | T0 | MISLEADING_CLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0267 | XuanYu.World.Tests/UiRuntime/AreaDR3InspectorPagerRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0268 | XuanYu.World.Tests/UiRuntime/ContextToolbarGeometryRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0269 | XuanYu.World.Tests/UiRuntime/ContextToolbarNativeDismissRuntimeTests.cs | 5 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0270 | XuanYu.World.Tests/UiRuntime/ContextToolbarPopupClickTrackingTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0271 | XuanYu.World.Tests/UiRuntime/ContextToolbarPopupDiagnosticIdentityTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0272 | XuanYu.World.Tests/UiRuntime/ContextToolbarPopupHostRuntimeTests.Contracts.cs | 3 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0273 | XuanYu.World.Tests/UiRuntime/ContextToolbarPopupHostRuntimeTests.cs | 5 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0274 | XuanYu.World.Tests/UiRuntime/ContextToolbarR2RuntimeTests.cs | 8 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0275 | XuanYu.World.Tests/UiRuntime/DatasetLayerPanelRuntimeLayoutTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0276 | XuanYu.World.Tests/UiRuntime/DiagnosticAutoIdRegressionTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0277 | XuanYu.World.Tests/UiRuntime/DiagnosticBoundsRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0278 | XuanYu.World.Tests/UiRuntime/DiagnosticCardPlacementTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0279 | XuanYu.World.Tests/UiRuntime/DiagnosticClickToTrackHeadlessTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0280 | XuanYu.World.Tests/UiRuntime/DiagnosticClickToTrackTests.cs | 7 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0281 | XuanYu.World.Tests/UiRuntime/DiagnosticFix2Tests.cs | 4 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0282 | XuanYu.World.Tests/UiRuntime/DiagnosticFloatingCardXyuiTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0283 | XuanYu.World.Tests/UiRuntime/DiagnosticIdentityTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0284 | XuanYu.World.Tests/UiRuntime/DiagnosticLockedControlNativeMoveTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0285 | XuanYu.World.Tests/UiRuntime/DiagnosticMappedDisplayTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0286 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeCoordinateMappingTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0287 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeDialogEdgeTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0288 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeDialogEdgeTests.Cycles.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0289 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeDialogLifecycleTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0290 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeLockedClickTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0291 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeOverlayRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0292 | XuanYu.World.Tests/UiRuntime/DiagnosticNativePointerProbeTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0293 | XuanYu.World.Tests/UiRuntime/DiagnosticNativeTargetOwnershipTests.cs | 6 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0294 | XuanYu.World.Tests/UiRuntime/DiagnosticOverlayRuntimeTests.cs | 4 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0295 | XuanYu.World.Tests/UiRuntime/DiagnosticOwnerActivationRestoreTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0296 | XuanYu.World.Tests/UiRuntime/DiagnosticOwnerActivationRetryTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0297 | XuanYu.World.Tests/UiRuntime/DiagnosticOwnerActivationWaitTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0298 | XuanYu.World.Tests/UiRuntime/DiagnosticPlacementPolicyEdgesTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0299 | XuanYu.World.Tests/UiRuntime/DiagnosticPlacementPolicyFallbackTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0300 | XuanYu.World.Tests/UiRuntime/DiagnosticPlacementPolicyTests.cs | 4 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0301 | XuanYu.World.Tests/UiRuntime/DiagnosticPopupBoundsRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0302 | XuanYu.World.Tests/UiRuntime/DiagnosticProbeFix1Tests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0303 | XuanYu.World.Tests/UiRuntime/DiagnosticProbeInteractionTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0304 | XuanYu.World.Tests/UiRuntime/DiagnosticProbeOverlayRuntimeTests.cs | 5 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0305 | XuanYu.World.Tests/UiRuntime/DiagnosticProbeResolverLocatorTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0306 | XuanYu.World.Tests/UiRuntime/DiagnosticProbeResolverTests.cs | 6 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0307 | XuanYu.World.Tests/UiRuntime/DiagnosticR1FloatingRuntimeTests.cs | 4 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0308 | XuanYu.World.Tests/UiRuntime/DiagnosticR1IdentityCompletionTests.cs | 2 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0309 | XuanYu.World.Tests/UiRuntime/DiagnosticR1PopupRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0310 | XuanYu.World.Tests/UiRuntime/DiagnosticR1ReportTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0311 | XuanYu.World.Tests/UiRuntime/DiagnosticR1SnapshotTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0312 | XuanYu.World.Tests/UiRuntime/DiagnosticRegionSelectionRegressionTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0313 | XuanYu.World.Tests/UiRuntime/DiagnosticRegistrationRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0314 | XuanYu.World.Tests/UiRuntime/DiagnosticSnapshotTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0315 | XuanYu.World.Tests/UiRuntime/DiagnosticTargetBoundsRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0316 | XuanYu.World.Tests/UiRuntime/DiagnosticTrackedHideTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0317 | XuanYu.World.Tests/UiRuntime/DiagnosticTrackedIdentityTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0318 | XuanYu.World.Tests/UiRuntime/DiagnosticTrackedLifecycleTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0319 | XuanYu.World.Tests/UiRuntime/DiagnosticViewportInputPassthroughTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0320 | XuanYu.World.Tests/UiRuntime/DiagnosticWorkspaceSelectorIdentityTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0321 | XuanYu.World.Tests/UiRuntime/EngineNavigationLayoutStabilityTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0322 | XuanYu.World.Tests/UiRuntime/EngineNavigationScrollAuthorityTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0323 | XuanYu.World.Tests/UiRuntime/FeatureEditCR1InspectorRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0324 | XuanYu.World.Tests/UiRuntime/FeatureEditCR1RuntimeTests.cs | 3 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0325 | XuanYu.World.Tests/UiRuntime/FeatureEditInspectorContractTests.cs | 1 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0326 | XuanYu.World.Tests/UiRuntime/FeatureEditSelectionResetTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0327 | XuanYu.World.Tests/UiRuntime/FeatureEditSelectionResetTests.RoadVertices.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0328 | XuanYu.World.Tests/UiRuntime/FeatureEditUiContractTests.cs | 2 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0329 | XuanYu.World.Tests/UiRuntime/FeatureEditWorkflowRuntimeTests.cs | 4 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0330 | XuanYu.World.Tests/UiRuntime/GenericMarkerSnapIntegrationTests.cs | 3 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0331 | XuanYu.World.Tests/UiRuntime/GenericRoadSnapIntegrationTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0332 | XuanYu.World.Tests/UiRuntime/HeadlessInputInfrastructureTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0333 | XuanYu.World.Tests/UiRuntime/InspectorEntityEditTargetTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0334 | XuanYu.World.Tests/UiRuntime/InspectorPropertyMutabilityTests.cs | 7 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0335 | XuanYu.World.Tests/UiRuntime/InspectorPropertyMutabilityTests.Header.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0336 | XuanYu.World.Tests/UiRuntime/InspectorPropertyMutabilityTests.Navigation.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0337 | XuanYu.World.Tests/UiRuntime/InspectorPropertyNavigationTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0338 | XuanYu.World.Tests/UiRuntime/InspectorPropertyTargetTests.cs | 4 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0339 | XuanYu.World.Tests/UiRuntime/InspectorRegionColorPreviewTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0340 | XuanYu.World.Tests/UiRuntime/InspectorRegionColorTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0341 | XuanYu.World.Tests/UiRuntime/InspectorSectionRailLayoutRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0342 | XuanYu.World.Tests/UiRuntime/InspectorSectionRailScrollRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0343 | XuanYu.World.Tests/UiRuntime/InspectorSelectionContractTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0344 | XuanYu.World.Tests/UiRuntime/InspectorSingleFocusSectionTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0345 | XuanYu.World.Tests/UiRuntime/LayerARuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0346 | XuanYu.World.Tests/UiRuntime/LayerPanelRuntimeLayoutTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0347 | XuanYu.World.Tests/UiRuntime/LayerPanelRuntimeStateTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0348 | XuanYu.World.Tests/UiRuntime/MapLabelRasterizationAlphaRegressionTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0349 | XuanYu.World.Tests/UiRuntime/MapLabelRasterizerTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0350 | XuanYu.World.Tests/UiRuntime/MapMarkerInspectorPanelRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0351 | XuanYu.World.Tests/UiRuntime/MapMarkerInspectorPersistenceTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0352 | XuanYu.World.Tests/UiRuntime/MapMarkerInspectorViewportTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0353 | XuanYu.World.Tests/UiRuntime/MapMarkerInspectorWorkflowTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0354 | XuanYu.World.Tests/UiRuntime/MapMarkerPlacementTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0355 | XuanYu.World.Tests/UiRuntime/MapRegionLabelProjectionTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0356 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayAnalyticStrokeRegressionTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0357 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayAnchorContractTests.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0358 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayDepthPolicyTests.cs | 3 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0359 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayStrokeContractTests.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0360 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayV1Tests.Colors.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0361 | XuanYu.World.Tests/UiRuntime/MapVectorOverlayV1Tests.cs | 7 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0362 | XuanYu.World.Tests/UiRuntime/PointFeatureEntryRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0363 | XuanYu.World.Tests/UiRuntime/R2BPropertyEditorVisualContractTests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0364 | XuanYu.World.Tests/UiRuntime/RegionDrawContextSyncFix1Tests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0365 | XuanYu.World.Tests/UiRuntime/RegionDrawContextSyncFix1Tests.Lifecycle.cs | 5 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0366 | XuanYu.World.Tests/UiRuntime/RegionDrawContextToolbarRuntimeFix1Tests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0367 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1ActivationRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0368 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1BTests.cs | 7 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0369 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1BTests.VertexCount.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0370 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1CStabilityTests.cs | 4 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0371 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1FullRuntimeTests.cs | 7 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0372 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1FullRuntimeTests.Names.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0373 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1HeadlessTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0374 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1RenderContractTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0375 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1ResizeTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0376 | XuanYu.World.Tests/UiRuntime/RegionDrawingF1RuntimeRedTests.cs | 3 | T0 | MISLEADING_CLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0377 | XuanYu.World.Tests/UiRuntime/RegionDrawingF2PolygonTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0378 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapInputChainTests.cs | 4 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0379 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapInputChainTests.Edge.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0380 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.cs | 3 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0381 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.Edge.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0382 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.History.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0383 | XuanYu.World.Tests/UiRuntime/RegionDrawingSnapRuntimeTests.Persistence.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0384 | XuanYu.World.Tests/UiRuntime/RegionPointerSafetyF2Tests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0385 | XuanYu.World.Tests/UiRuntime/RightTabsVisibilityRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0386 | XuanYu.World.Tests/UiRuntime/RoadDrawingSelectionF1Tests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0387 | XuanYu.World.Tests/UiRuntime/RoadDrawingSelectionF1Tests.Names.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0388 | XuanYu.World.Tests/UiRuntime/RoadVertexDragD2Tests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0389 | XuanYu.World.Tests/UiRuntime/RoadVertexSelectionD1Tests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0390 | XuanYu.World.Tests/UiRuntime/ScaleIndicatorVisibilityRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0391 | XuanYu.World.Tests/UiRuntime/TerrainAutoFrameD1Tests.cs | 5 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0392 | XuanYu.World.Tests/UiRuntime/TerrainAutoFrameD1Tests.Reimport.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0393 | XuanYu.World.Tests/UiRuntime/TerrainAutoFrameLongRangeTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0394 | XuanYu.World.Tests/UiRuntime/TerrainContextLeafHoverFixTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0395 | XuanYu.World.Tests/UiRuntime/TerrainHotpathTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0396 | XuanYu.World.Tests/UiRuntime/TerrainImportOrchestrationR1Tests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0397 | XuanYu.World.Tests/UiRuntime/TerrainInspectorRuntimeTests.cs | 3 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0398 | XuanYu.World.Tests/UiRuntime/TerrainRenderIntegrationTests.cs | 3 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0399 | XuanYu.World.Tests/UiRuntime/TerrainTopContextRuntimeTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0400 | XuanYu.World.Tests/UiRuntime/TopLeftInteractionR1Tests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0401 | XuanYu.World.Tests/UiRuntime/TopModeGeometryRuntimeTests.cs | 1 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0402 | XuanYu.World.Tests/UiRuntime/UiR1VisualContractTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0403 | XuanYu.World.Tests/UiRuntime/UiR1VisualFixContractTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0404 | XuanYu.World.Tests/UiRuntime/UiRuntimeRiskTests.cs | 2 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0405 | XuanYu.World.Tests/UiRuntime/WorkspaceSelectorR2ContractTests.cs | 5 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0406 | XuanYu.World.Tests/UiRuntime/XYUI2R2BContractTests.cs | 5 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0407 | XuanYu.World.Tests/UiTokens/AreaCR1ContextToolbarContractTests.cs | 5 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0408 | XuanYu.World.Tests/UiTokens/CanonicalToolchainResolverTests.cs | 5 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0409 | XuanYu.World.Tests/UiTokens/EditorScrollAuditContractTests.cs | 2 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0410 | XuanYu.World.Tests/UiTokens/InspectorFix1ContractTests.cs | 4 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0411 | XuanYu.World.Tests/UiTokens/InspectorFix2ContractTests.cs | 4 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0412 | XuanYu.World.Tests/UiTokens/LayerAUiCompositionTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0413 | XuanYu.World.Tests/UiTokens/PointFeatureEntryContractTests.cs | 2 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0414 | XuanYu.World.Tests/UiTokens/TerrainTopContextContractTests.cs | 2 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0415 | XuanYu.World.Tests/UiTokens/TopWorkspaceSelectorR1Tests.cs | 2 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0416 | XuanYu.World.Tests/UiTokens/UiCanonicalVersionContractTests.cs | 6 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0417 | XuanYu.World.Tests/UiTokens/UiCloseLifecycleContractTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0418 | XuanYu.World.Tests/UiTokens/UiCsColorRulesTests.cs | 5 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0419 | XuanYu.World.Tests/UiTokens/UiD2F1RegionToolActivationContractTests.cs | 1 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0420 | XuanYu.World.Tests/UiTokens/UiD2F1RegionToolContractTests.cs | 2 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0421 | XuanYu.World.Tests/UiTokens/UiD3DebtClearedTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0422 | XuanYu.World.Tests/UiTokens/UiD4DebtClearedTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0423 | XuanYu.World.Tests/UiTokens/UiD4F1ButtonContractTests.cs | 5 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0424 | XuanYu.World.Tests/UiTokens/UiD4F1LayoutModelTests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0425 | XuanYu.World.Tests/UiTokens/UiD4F1TextOverflowContractTests.cs | 7 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0426 | XuanYu.World.Tests/UiTokens/UiD4F1TypographyContractTests.cs | 5 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0427 | XuanYu.World.Tests/UiTokens/UiD4InspectorContractTests.cs | 9 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0428 | XuanYu.World.Tests/UiTokens/UiD4LayerContractTests.cs | 9 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0429 | XuanYu.World.Tests/UiTokens/UiD4LayoutModelTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0430 | XuanYu.World.Tests/UiTokens/UiD4MapEditorContractTests.cs | 10 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0431 | XuanYu.World.Tests/UiTokens/UiD5ButtonContractTests.cs | 5 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0432 | XuanYu.World.Tests/UiTokens/UiD5CorrectionBehaviorTests.cs | 7 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0433 | XuanYu.World.Tests/UiTokens/UiD5CorrectionNotifyTests.cs | 5 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0434 | XuanYu.World.Tests/UiTokens/UiD5CorrectionStructureTests.cs | 7 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0435 | XuanYu.World.Tests/UiTokens/UiD5DangerFlowTests.cs | 5 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0436 | XuanYu.World.Tests/UiTokens/UiD5DialogAndLogContractTests.cs | 8 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0437 | XuanYu.World.Tests/UiTokens/UiD5FormContractTests.cs | 5 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0438 | XuanYu.World.Tests/UiTokens/UiD5InputValidationTests.cs | 8 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0439 | XuanYu.World.Tests/UiTokens/UiD5MapStatusTests.cs | 8 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0440 | XuanYu.World.Tests/UiTokens/UiD5NotificationTests.cs | 6 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0441 | XuanYu.World.Tests/UiTokens/UiD5UnsavedDialogBehaviorTests.cs | 9 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0442 | XuanYu.World.Tests/UiTokens/UiD5UnsavedDialogTests.cs | 8 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0443 | XuanYu.World.Tests/UiTokens/UiD5UnsavedFlowTests.cs | 8 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0444 | XuanYu.World.Tests/UiTokens/UiD6AccessibilityContractTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0445 | XuanYu.World.Tests/UiTokens/UiD6DpiContractTests.cs | 3 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0446 | XuanYu.World.Tests/UiTokens/UiD6LogPerformanceTests.cs | 2 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0447 | XuanYu.World.Tests/UiTokens/UiD6MotionContractTests.cs | 2 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0448 | XuanYu.World.Tests/UiTokens/UiDebtBaselineBypassF2Tests.cs | 8 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0449 | XuanYu.World.Tests/UiTokens/UiDebtBaselineBypassTests.cs | 9 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0450 | XuanYu.World.Tests/UiTokens/UiDebtBaselineTests.cs | 2 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0451 | XuanYu.World.Tests/UiTokens/UiF3LayerRowContractTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0452 | XuanYu.World.Tests/UiTokens/UiLayerDeleteDialogContractTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0453 | XuanYu.World.Tests/UiTokens/UiR1FinalLeftTopContractTests.cs | 5 | T0 | MISLEADING_CLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0454 | XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzerTests.cs | 10 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0455 | XuanYu.World.Tests/UiTokens/UiSourceContractAnalyzerTokenRefTests.cs | 2 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0456 | XuanYu.World.Tests/UiTokens/UiTokenManifestGraphTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0457 | XuanYu.World.Tests/UiTokens/UiTokenManifestTests.cs | 4 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0458 | XuanYu.World.Tests/UiTokens/UiTopTabStripContractTests.cs | 5 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0459 | XuanYu.World.Tests/UiTokens/UiTopTabStripModelHintAndListTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0460 | XuanYu.World.Tests/UiTokens/UiTopTabStripModelTests.cs | 8 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0461 | XuanYu.World.Tests/UiTokens/XyeToolbarTextPrimitiveContractTests.cs | 2 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0462 | XuanYu.World.Tests/Viewport/AvaloniaPointerEventAdapterTests.cs | 2 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0463 | XuanYu.World.Tests/Viewport/InputIntegration/AvaloniaViewportInputCutoverTests.cs | 4 | T0 | MISLEADING_CLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0464 | XuanYu.World.Tests/Viewport/InputIntegration/CameraWheelAdapterIntegrationTests.cs | 3 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0465 | XuanYu.World.Tests/Viewport/InputIntegration/CameraWheelInputIntegrationTests.cs | 4 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0466 | XuanYu.World.Tests/Viewport/InputIntegration/ConsumerArbitrationIntegrationTests.cs | 7 | T1 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0467 | XuanYu.World.Tests/Viewport/InputIntegration/ConsumerLifecycleIntegrationTests.cs | 5 | T1 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0468 | XuanYu.World.Tests/Viewport/InputIntegration/D1ConsumerCancellationTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0469 | XuanYu.World.Tests/Viewport/InputIntegration/D1ConsumerMigrationTests.cs | 4 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0470 | XuanYu.World.Tests/Viewport/InputIntegration/NativeKeyboardInputTests.cs | 5 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0471 | XuanYu.World.Tests/Viewport/InputIntegration/NativeViewportCoordinateContractTests.cs | 1 | T1 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0472 | XuanYu.World.Tests/Viewport/InputIntegration/NativeViewportInputForwarderTests.cs | 6 | T0 | MISLEADING_CLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0473 | XuanYu.World.Tests/Viewport/InputIntegration/NavigationGizmoConsumerContractTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0474 | XuanYu.World.Tests/Viewport/InputIntegration/PointerSemanticClosureTests.cs | 4 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0475 | XuanYu.World.Tests/Viewport/InputIntegration/ProductionInputCompositionTests.cs | 4 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0476 | XuanYu.World.Tests/Viewport/InputIntegration/ViewportInputConvergenceIntegrationTests.cs | 3 | T2 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0477 | XuanYu.World.Tests/Viewport/InputLifecycle/ViewportGestureTerminalTests.cs | 2 | T1 | TIER_OVERCLAIM | RENAME | ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0478 | XuanYu.World.Tests/Viewport/MapContextMenuRouterTests.cs | 1 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0479 | XuanYu.World.Tests/Viewport/MapEditingTemporaryStateTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0480 | XuanYu.World.Tests/Viewport/MapInputCancellationIntegrationTests.cs | 1 | T1 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0481 | XuanYu.World.Tests/Viewport/MapInputConsumerContractTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0482 | XuanYu.World.Tests/Viewport/MapInputConsumerRegressionTests.cs | 1 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0483 | XuanYu.World.Tests/Viewport/NativePointerEventAdapterTests.cs | 3 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0484 | XuanYu.World.Tests/Viewport/NativePointerRoutePolicyTests.cs | 6 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0485 | XuanYu.World.Tests/Viewport/PlatformInputParity/NativeSourceParityTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0486 | XuanYu.World.Tests/Viewport/RegionDrawingInputModifierTests.cs | 3 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0487 | XuanYu.World.Tests/Viewport/UnifiedPointerModelTests.cs | 2 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0488 | XuanYu.World.Tests/Viewport/UnifiedPointerReadinessContractTests.cs | 3 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0489 | XuanYu.World.Tests/Viewport/ViewportInputRouterDispatchTests.cs | 2 | T1 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0490 | XuanYu.World.Tests/Viewport/ViewportInputRouterMapArbitrationTests.cs | 2 | T1 | TIER_OVERCLAIM | RENAME | ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0491 | XuanYu.World.Tests/Workspace/EditorWorkspaceManagerTests.cs | 8 | T1 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0492 | XuanYu.World.Tests/Workspace/EditorWorkspaceUiCompositionTests.cs | 7 | T0 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0493 | XuanYu.World.Tests/Workspace/EditorWorkspaceUiTests.cs | 7 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0494 | XuanYu.World.Tests/Workspace/RegionAuthoringHierarchyTests.cs | 5 | T2 | VERIFIED | KEEP | SENSITIVE_OR_ESCALATE | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0495 | XuanYu.World.Tests/WorldPartition/WorldPartitionInvariantTests.cs | 3 | T0 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0496 | XuanYu.World.Tests/WorldPartition/WorldPartitionMigrationTests.Activity.cs | 1 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0497 | XuanYu.World.Tests/WorldPartition/WorldPartitionMigrationTests.cs | 4 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0498 | XuanYu.World.Tests/WorldPartition/WorldPartitionTests.cs | 4 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0499 | XuanYu.World.Tests/WorldPartition/WorldPartitionTests.PartitionStrategy.cs | 1 | T1 | VERIFIED | KEEP | NOT_REQUIRED | Explicit value/state/result assertions at file scope. | MIXED_RUN_UNATTRIBUTED |
| TR-B-0500 | XuanYu.World.Tests/WorldPartition/WorldPartitionUiTests.cs | 4 | T2 | VERIFIED | KEEP | NOT_REQUIRED | Guard only unless semantic value/state assertion exists. | MIXED_RUN_UNATTRIBUTED |

## Knowledge / experience deposited

- Evidence tier is determined by the executed path, never by `Runtime`, `Real`, `Integration`, `Viewport`, `Acceptance`, `Final`, `Visible`, or `Stable` in a name.
- Headless synthetic input, adapter/router composition, native Win32 input, real Vulkan, and human acceptance are separate capabilities and must not be promoted across T0–T4.
- `NotNull`, `NotEmpty`, and bare `True` are guard assertions, not capability oracles; retain them only when the semantic assertion is independently present.
- A failed automated test is an execution fact; its product meaning remains unclassified until helper/fixture/host integrity and real path evidence are separated.
- Duplicate tests may be RETIRED only with semantic duplicate evidence; test count is not a KPI.

