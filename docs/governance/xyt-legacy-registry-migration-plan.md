# XYT-G-INTEGRATION-PREP / Legacy Registry Migration Plan

状态：PREPARED WITH T0 GOVERNANCE BLOCKER  
任务：`XYT-G-INTEGRATION-PREP`  
范围：只生成迁移计划；不写入 `docs/governance/test-registry.json`，不修改测试代码，不删除测试。

## 1. 结论

本计划将审计明细中的 696 个旧测试源单元分为三个候选迁移批次：

| Wave | 明细行数 | 入口条件 | 当前结论 |
|---|---:|---|---|
| WAVE-HIGH | 132 | 先处理 DOWNGRADE/RENAME、运行时证据冒充、Vulkan/Swapchain/Churn/Performance/Native/Runtime/Integration 等命名与证据不一致 | BLOCKED UNTIL REVIEW |
| WAVE-MEDIUM | 237 | 证据层级基本可定位，但需补身份证、能力键、产品不变量或 blockingScope | BLOCKED UNTIL METADATA REVIEW |
| WAVE-LOW | 327 | KEEP 且语义与实际证据一致，仅可在门禁后批量登记 | CANDIDATE ONLY |
| 合计 | 696 | 必须逐条保持 Test ID 与审计证据可追溯 | 不能直接写 Registry |

### T0 阻断：审计摘要与逐行事实不一致

`xyt-legacy-test-audit.md` 摘要写 `LOW=330`，但其 Exhaustive test-source audit 逐行解析为 `HIGH=132, MEDIUM=237, LOW=327`，合计 696。动作计数 `DOWNGRADE=37, RENAME=95, KEEP=564` 与逐行明细一致。

本计划不擅自把 327 改成 330，也不制造 3 条测试。正式迁移前必须由 G Audit 修正摘要或补充缺失明细，并重新核对 696 的恒等式。这是 T0 级治理数据一致性阻断，不是测试代码失败，也不是允许猜测登记的理由。

## 2. 输入事实

| 输入 | 事实 |
|---|---|
| `docs/governance/xyt-legacy-test-audit.md` | 696 个 test-bearing C# 文件、2552 个 attributed tests；逐行 ID 为 `XYT-G-0001` 至 `XYT-G-0696` |
| `docs/governance/test-registry-policy.md` | Registry 必须有稳定 `testId`；P0-P4 是证据登记等级，T0-T3 是 Incident Severity，二者不得混用 |
| `docs/governance/test-capability-map.json` | 当前只有 3 个能力键：`CAP-CORE-MATH`、`CAP-WORLD-HEADLESS`、`CAP-XYUI-RUNTIME` |
| `docs/governance/test-registry.json` | 当前仅有 3 条既有 Registry 记录；本任务未修改它 |

审计逐行表没有给出旧 Registry 的 current evidenceLevel；因此本计划的 `Current P-Level` 统一写为 `UNREGISTERED`，绝不从测试名称反推旧等级。`Target P-Level` 才是审计提出的候选等级，仍需按 Registry policy 与实际证据复核。

## 3. 批次规则

### WAVE-HIGH

纳入规则：逐行 `Risk=HIGH`；其中包含全部 DOWNGRADE 37 与 RENAME 95。尤其优先处理名字含有 `Vulkan`、`Swapchain`、`Churn`、`Performance`、`Native`、`Runtime`、`Integration` 的记录。逐行扫描显示这些命名命中项全部不是 P3；它们不得因名称升级为 P3。

审计没有发现 P3 或 P4 的逐行候选。`Runtime`、`Vulkan` 等词在名称中最多形成复核信号，不能替代 `REAL_RUNTIME`、真实 HWND/surface/swapchain/present/GPU 或产品验收证据。不存在 `EndToEnd` / `Real` 命中项；不得为补齐分类而创建或推测记录。

Action 约束：DOWNGRADE 只表示目标证据等级需要下降或重新确认；RENAME 只表示消除语义过度声明。两者都不允许在本计划阶段改测试代码或写 Registry。

### WAVE-MEDIUM

纳入规则：逐行 `Risk=MEDIUM`。这些记录可保留审计提出的 P0/P1/P2 候选，但必须在正式登记前补齐稳定身份证、能力键、产品不变量、前置条件、blockingScope、evidenceType、时限、测试集版本、路径、状态和 owner。能力不在当前能力映射表中的，保持 `UNMAPPED`，不得猜成现有能力键。

### WAVE-LOW

纳入规则：逐行 `Risk=LOW` 且 `Action=KEEP`。这是登记候选批次，不是已登记事实。只有 G-CONVERGENCE=CLOSED、逐行字段校验通过、能力键可解析、路径存在且 evidenceLevel/evidenceType 一致后，才允许启动 `XYT-G-INTEGRATION` 正式写 Registry。

## 4. 不可违反的迁移原则

- Unknown、UNREGISTERED 或缺失字段保持未知；不得批量猜成任何 P 级。
- `Runtime`、`Vulkan`、`Native`、`Performance`、`EndToEnd`、`Real` 等词不是 P3 证据。
- 不自动删除测试，不自动修改测试代码，不把重命名当成证据升级。
- `testId` 必须稳定；文件移动、方法重命名或实现重写不得自动换号。
- 能力键只能来自 `test-capability-map.json`。当前审计能力标签如 `Core/domain logic`、`World/editor domain`、`Render/Terrain contract`、`UI/input/integration` 不得未经治理映射直接伪装成现有 `CAP-*`。
- P3 只能在真实运行证据满足时登记；P4 仍需独立产品验收，不得由任何旧测试名称推出。
- T0 是事故严重度，不是 `evidenceLevel`；本计划的 T0 指审计数据一致性阻断。
- 在 `G-CONVERGENCE = CLOSED` 之前，本文件是候选迁移计划，不是 Registry 写入授权。

## 5. 行级迁移清单

下表完整覆盖审计 Exhaustive table 的 696 行。`Current P-Level` 有意保持 `UNREGISTERED`；目标等级、能力和风险均直接取审计逐行字段。`Registry Action` 只是后续动作，不表示本轮已经执行。

| Wave | Test ID | Current Name | Current P-Level | Target P-Level | Action | Capability | Risk | Registry Action |
|---|---|---|---|---|---|---|---|---|
| WAVE-LOW | XYT-G-0001 | CameraBasisTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0002 | CameraFarRecoveryTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0003 | CameraNavigationRollTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0004 | CameraNavigationSequenceTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0005 | CameraNavigationStressTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0006 | CameraNavigationTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0007 | CameraNavigationUiSequenceTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0008 | CameraNavigationUiSequenceTests.Safety | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0009 | CameraOrthographicNavigationTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0010 | FarProjectionSafetyTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0011 | ReverseZProjectionPrototypeTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0012 | CoreSmokeTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0013 | EditorTransformCapturePolicyTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0014 | MoveGizmoDragConstraintTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0015 | MoveGizmoLayoutG1Tests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0016 | MoveGizmoLayoutPlaneTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0017 | MoveGizmoLayoutTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0018 | MoveGizmoLayoutVulkanTests | UNREGISTERED | P1 | DOWNGRADE | Render/Terrain contract | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0019 | MoveGizmoScreenSizeTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0020 | RotateGizmoLayoutTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0021 | ScaleGizmoTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0022 | ScaleGizmoTests.Drag | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0023 | ScaleGizmoTests.DragSafety | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0024 | ScaleGizmoTests.R5R1 | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0025 | EditorHistoryOwnerTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0026 | EditorHistoryRedoTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0027 | TransformHistoryIntegrationTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0028 | TransformHistoryRedoIntegrationTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0029 | ViewportPickingServiceTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0030 | FarViewBackgroundShaderContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0031 | StandardViewResolverTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0032 | RenderLogNoiseContractTests | UNREGISTERED | P0 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0033 | CubeRenderDrawPlanTests | UNREGISTERED | P0 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0034 | FrameExecutionPolicyTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0035 | RenderDrawPlanTests | UNREGISTERED | P1 | DOWNGRADE | Render/Terrain contract | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0036 | SceneRenderProjectionAdapterTests | UNREGISTERED | P0 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0037 | SceneRenderProjectionAdapterTests.Rotation | UNREGISTERED | P2 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0038 | SceneRenderProjectionAdapterTests.Selection | UNREGISTERED | P2 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0039 | ViewportAssistDrawPlanTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0040 | ViewportChromeContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0041 | ViewportScaleIndicatorContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0042 | BlenderStyleGridTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0043 | ReferenceGridDrawPlanTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0044 | ReferenceGridFrameStateTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0045 | ReferenceGridShaderContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0046 | ReverseZWorldGridContractTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0047 | ScaleIndicatorMetricTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0048 | ViewportMetricScaleTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0049 | WorldGridStartupLifecycleContractTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0050 | WorldReferenceGridFarFadeContractTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0051 | LatestRenderProjectionQueueTests | UNREGISTERED | P2 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0052 | MapRegionDrawPlanTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0053 | MapRenderDrawPlanTests | UNREGISTERED | P0 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0054 | MapSurfaceGeometryTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0055 | MapSurfaceLayerVisibilityTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0056 | MapSurfaceResourceKeyTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0057 | MapSurfaceResourceUpdatePolicyTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0058 | NavigationGizmoDipContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0059 | NavigationGizmoDirectionContractTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0060 | NavigationGizmoInputIsolationTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0061 | NavigationGizmoLayoutTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0062 | NavigationGizmoLayoutTests.Facing | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0063 | NavigationGizmoOverlayContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0064 | NavigationGizmoOverlayContractTests.Shader | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0065 | ScaleIndicatorGlyphLiteTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0066 | ViewportOverlayLayoutTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0067 | RegionModelTransformContractTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0068 | StaticModelDepthRegressionTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0069 | StaticModelRenderContractTests | UNREGISTERED | P2 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0070 | TerrainVisibilitySelectorTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0071 | CameraOrthographicTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0072 | CameraStateTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0073 | DefaultEditorCameraTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0074 | ReverseZOrthographicProjectionTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0075 | ReverseZPerspectiveProjectionTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0076 | ReverseZPrecisionRegressionTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0077 | ViewportStateTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0078 | ViewProjectionStateTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0079 | WorldRayFactoryTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0080 | WorldRayTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0081 | RayAabbIntersectionTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0082 | SpatialBoundsTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0083 | GridScaleBarConvergenceTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0084 | MilitaryIdentityTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0085 | SoldierStateTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0086 | WarCoreDependencyTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0087 | WorldRenderDependencyBoundaryTests | UNREGISTERED | P0 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0088 | AssetContractTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0089 | AssetDialogTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0090 | GlbImportTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0091 | HostingCompleteTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0092 | HostingPlannerRejectTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0093 | HostingPlannerTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0094 | HostingRollbackTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0095 | HostingSaveAsTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0096 | HostingTransactionTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0097 | LoadStructureErrorTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0098 | LoadTransactionTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0099 | SaveAsTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0100 | SaveTransactionTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0101 | SchemaCompatibilityTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0102 | StaticModelAuthoringServiceTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0103 | StaticModelBaseVertexTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0104 | StaticModelCatalogTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0105 | StaticModelFailureTrackerTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0106 | StaticModelProjectionTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0107 | StaticModelUiTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0108 | StaticModelValidatorTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0109 | CameraC2DraftFramingTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0110 | CameraC2MapFramingTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0111 | CameraDocumentTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0112 | CameraFramingOccupancyTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0113 | CameraFramingTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0114 | CameraNavigationUiTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0115 | CameraNavigationUiTests.Focus | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0116 | CameraOrbitCaptureRegressionTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0117 | EmptySceneCameraContractTests | UNREGISTERED | P2 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0118 | UiViewGizmoTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0119 | GeographicWorldMappingTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0120 | EditorLogFilterStateTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0121 | FootAxamlTailContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0122 | LogAutoScrollPolicyTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0123 | LogListAutoScrollControllerContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0124 | LogPerformanceGovernanceTests | UNREGISTERED | P0 | RENAME | Core/domain logic | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0125 | UiMapLogChineseTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0126 | UiRootLogRowContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0127 | MapLayerSessionTests.Behavior | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0128 | MapLayerSessionTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0129 | MapLayerSessionTests.Drag | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0130 | MapLayerSessionTests.Drag.History | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0131 | UiLayerStateFeedbackTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0132 | UiLayerVisualContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0133 | UiLogSummaryPriorityTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0134 | UiLogSummaryTimingTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0135 | UiMapCommandRoutingTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0136 | UiMapDatasetContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0137 | UiMapDatasetF1AcceptanceTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0138 | UiMapDatasetF1Tests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0139 | UiMapDatasetF2Tests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0140 | UiMapDatasetF3ContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0141 | UiMapDatasetF3Tests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0142 | UiMapDatasetLayerR3Tests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0143 | UiMapDatasetM04Tests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0144 | UiMapDatasetRegionBootstrapPersistenceTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0145 | UiMapDatasetRegionBootstrapTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0146 | UiMapDatasetRegionLayerF3Tests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0147 | UiMapDatasetRegionRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0148 | UiMapDatasetRegionToolActivationTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0149 | UiMapDatasetRegionToolInvalidTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0150 | UiMapEditorTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0151 | UiMapHistoryTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0152 | UiMapInitialProjectionTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0153 | UiMapLayerDeleteLockRecoveryTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0154 | UiMapLayerDragTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0155 | UiMapLayerLockLogTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0156 | UiMapLayerPanelTests.Behavior | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0157 | UiMapLayerPanelTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0158 | UiMapLayoutContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0159 | UiMapManifestIdentityTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0160 | UiMapManifestNavigationTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0161 | MapBoundsTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0162 | MapCoordinateValidationTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0163 | MapDatasetContractTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0164 | MapDatasetDocumentTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0165 | MapDatasetLayerStateTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0166 | MapDatasetRegistryF1FailureTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0167 | MapDatasetRegistryF2Tests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0168 | MapDatasetRegistryFailureTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0169 | MapDatasetRegistryLifecycleTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0170 | MapDatasetStorageContractTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0171 | MapDefaultMapTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0172 | MapDefinitionTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0173 | MapDocumentAggregateBridgeTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0174 | MapDocumentOwnerChainTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0175 | MapDocumentOwnerTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0176 | MapEnvironmentValidationTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0177 | MapIdTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0178 | MapJsonRoundTripTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0179 | MapJsonStrictnessTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0180 | MapLayerRulesTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0181 | MapLayerStackTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0182 | MapLayerStackTests.Drag | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0183 | MapLayerStackTests.Order | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0184 | MapLayerTests.Base | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0185 | MapLayerTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0186 | MapManifestCreationTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0187 | MapManifestSerializationTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0188 | MapManifestStorageTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0189 | MapManifestValidationTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0190 | MapRegionDatasetContractTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0191 | MapRegionDatasetRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0192 | MapRegionDraftTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0193 | MapRegionTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0194 | MapRegionTests.Geometry | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0195 | MapRegionTests.Strictness | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0196 | MapRoadDatasetContractTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0197 | MapSizeValidationTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0198 | MapStorageFailureTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0199 | MapStorageTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0200 | MapSurfaceSamplerTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0201 | MapSurfaceValidationTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0202 | MapWorkingStorageTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0203 | SceneMapReferenceTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0204 | WorldMapStateOwnerTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0205 | WorldMapStateTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0206 | GenericGeometryCapabilityTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0207 | MapCoordinateContractTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0208 | MapEditSessionCommandTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0209 | MapEditSessionCreationTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0210 | MapEditSessionDirtyTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0211 | MapEditSessionGeometryTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0212 | MapEditSessionHistoryTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0213 | MapEditSessionMapPropertiesTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0214 | MapEditSessionObjectCommandTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0215 | MapEditSessionRegionStyleTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0216 | MapEditSessionRegionTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0217 | MapEditSessionSelectionTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0218 | MapEditSessionThreadTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0219 | MapEditSessionValidationTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0220 | MapGeometryContextHitTesterTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0221 | MapGeometryContextMenuSpecTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0222 | MapGeometryHitTesterTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0223 | MapObjectNameAllocatorTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0224 | MapPickingRoundTripTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0225 | MapRenderSnapshotProjectionTests | UNREGISTERED | P1 | DOWNGRADE | Render/Terrain contract | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0226 | MapSurfacePickerTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0227 | PointFeatureFoundationTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0228 | PolygonVisualCenterTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0229 | RegionDrawingF3HistoryTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0230 | RegionDrawingStateTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0231 | RegionEdgeSnapGeometryTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0232 | RegionEdgeSnapResolverTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0233 | RegionSnapPipelineContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0234 | RegionSnapPipelineLockTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0235 | RegionSnapPipelineTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0236 | RegionSnapStateTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0237 | RegionSpatialIndexLifecycleTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0238 | RegionSpatialIndexScaleTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0239 | RegionSpatialIndexTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0240 | RegionVertexSnapIntegrationContractTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0241 | RegionVertexSnapResolverBoundaryTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0242 | RegionVertexSnapResolverTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0243 | RegionVertexSnapScaleTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0244 | RegionVertexSnapStateTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0245 | AreaAR2AvailabilityContractTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0246 | EditorModeManagerTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0247 | EditorModeUiCompositionTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0248 | EditorModeUiTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0249 | ReverseZDepthContractPrototypeTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0250 | ReverseZDepthPipelineContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0251 | TerrainChunkDrawContractTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0252 | TerrainMultiTileRenderContractTests | UNREGISTERED | P0 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0253 | TerrainPreviewLightingContractTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0254 | VulkanPresentLoopContractTests | UNREGISTERED | P0 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0255 | VulkanPresentModeSelectionTests | UNREGISTERED | P1 | DOWNGRADE | Render/Terrain contract | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0256 | VulkanResizeContractTests | UNREGISTERED | P1 | DOWNGRADE | Render/Terrain contract | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0257 | VulkanSwapchainChurnTests | UNREGISTERED | P0 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0258 | WorldGridIndependenceContractTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0259 | WorldGridStartupContractTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0260 | WorldReferenceGridDepthContractTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0261 | CommandSmokeTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0262 | EditorEnvironmentTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0263 | EntityBoundsSemanticsTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0264 | EntityRegistryTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0265 | EntityTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0266 | FinalSceneTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0267 | GlobalWorldTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0268 | SceneConsumptionTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0269 | SceneDocumentPersistenceTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0270 | SceneDocumentTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0271 | SceneDocumentTests.Opening | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0272 | SceneDocumentTests.SaveFeedback | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0273 | SceneIsolationTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0274 | SceneMultiEntityGateTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0275 | SceneSelectionReentryTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0276 | SceneSingleAuthorityTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0277 | UiHistoryTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0278 | UiHistoryTests.InlineRename | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0279 | FinalSelectionTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0280 | SelectionToolStateUiTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0281 | ToolStateHighlightUiTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0282 | ToolStateHighlightUiTests.Selection | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0283 | SceneStateOwnerSpatialTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0284 | SpatialIndexEditLifecycleTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0285 | SpatialIndexOwnerLifecycleTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0286 | SpatialIndexOwnerRevisionTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0287 | SpatialIndexRebuildTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0288 | SpatialIndexScaleTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0289 | SpatialQueryGovernanceTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0290 | SpatialQueryTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0291 | SpatialRaycastNearestTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0292 | SpatialRaycastRevisionTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0293 | SpatialRaycastScaleTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0294 | SpatialRayQueryLifecycleTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0295 | SpatialRayQueryTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0296 | EsriAsciiGridTerrainReaderStreamingTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0297 | TerrainChunkMeshBuilderTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0298 | TerrainChunkNormalTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0299 | TerrainChunkPartitionerTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0300 | TerrainChunkQueryPreservationTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0301 | TerrainCut1UiContractTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0302 | TerrainElevationContractTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0303 | TerrainElevationTileRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | Render/Terrain contract | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-MEDIUM | XYT-G-0304 | TerrainFix1NotificationContractTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0305 | TerrainHgtImportAcceptanceTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0306 | TerrainHgtWorldAcceptanceTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0307 | TerrainImportFix1BUiContractTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0308 | TerrainImportProgressAcceptanceTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0309 | TerrainMeshNormalTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0310 | TerrainMultiSourceImportTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0311 | TerrainMultiSourceUiTests | UNREGISTERED | P2 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0312 | TerrainRenderContractTests | UNREGISTERED | P0 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0313 | TerrainRevisionTests | UNREGISTERED | P2 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0314 | TerrainSourceImportTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0315 | TerrainTileSetTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0316 | TerrainWorldTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0317 | MoveTransformUiTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0318 | MoveTransformUiTests.Plane | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0319 | MoveTransformUiTests.Region | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0320 | MoveTransformUiTests.Session | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0321 | RotateTransformUiTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0322 | RotateTransformUiTests.DragState | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0323 | RotateTransformUiTests.Preview | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0324 | RotateTransformUiTests.ToolSwitch | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0325 | ScaleGizmoGlobalModeTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0326 | ScaleTransformUiTests.AxisUniform | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0327 | ScaleTransformUiTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0328 | ScaleTransformUiTests.History | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0329 | ScaleTransformUiTests.Target | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0330 | TransformFoundationTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0331 | TransformFoundationTests.Input | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0332 | TransformFoundationTests.Inspector | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0333 | TransformSessionTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0334 | ViewportAssistTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0335 | UiHierarchyConnectorTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0336 | UiTreeGuideTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0337 | UiTreeToggleTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0338 | AreaAR4MenuRuntimeTests.Contracts | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0339 | AreaAR4MenuRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0340 | AreaAR5MenuRadioVisualTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0341 | AreaAR6WorkspaceRadioRenderTests | UNREGISTERED | P2 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0342 | AreaBLeftWorkspaceRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0343 | AreaBLeftWorkspaceRuntimeTests.R2 | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0344 | AreaCR1ContextToolbarRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0345 | AreaDR1Fix4InspectorRoutingTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0346 | AreaDR1Fix5RightContentOwnershipTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0347 | AreaDR2CorrectionInstanceRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0348 | AreaDR2Fix1MapInspectorRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0349 | AreaDR2Fix2CompactNavLayerDockRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0350 | AreaDR2Fix3ProjectionDensityRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0351 | AreaDR2Fix4RegionInspectorRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0352 | AreaDR2Fix5TabIntegrationRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0353 | AreaDR2NavigationAndMapContextRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0354 | AreaDR3InspectorPagerRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0355 | ContextToolbarGeometryRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0356 | ContextToolbarNativeDismissRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0357 | ContextToolbarPopupClickTrackingTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0358 | ContextToolbarPopupDiagnosticIdentityTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0359 | ContextToolbarPopupHostRuntimeTests.Contracts | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0360 | ContextToolbarPopupHostRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0361 | ContextToolbarR2RuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0362 | DatasetLayerPanelRuntimeLayoutTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0363 | DiagnosticAutoIdRegressionTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0364 | DiagnosticBoundsRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0365 | DiagnosticCardPlacementTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0366 | DiagnosticClickToTrackHeadlessTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0367 | DiagnosticClickToTrackTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0368 | DiagnosticFix2Tests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0369 | DiagnosticFloatingCardXyuiTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0370 | DiagnosticIdentityTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0371 | DiagnosticLockedControlNativeMoveTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0372 | DiagnosticMappedDisplayTests | UNREGISTERED | P2 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0373 | DiagnosticNativeCoordinateMappingTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0374 | DiagnosticNativeDialogEdgeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0375 | DiagnosticNativeDialogEdgeTests.Cycles | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0376 | DiagnosticNativeDialogLifecycleTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0377 | DiagnosticNativeLockedClickTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0378 | DiagnosticNativeOverlayRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0379 | DiagnosticNativePointerProbeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0380 | DiagnosticNativeTargetOwnershipTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0381 | DiagnosticOverlayRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0382 | DiagnosticOwnerActivationRestoreTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0383 | DiagnosticOwnerActivationRetryTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0384 | DiagnosticOwnerActivationWaitTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0385 | DiagnosticPlacementPolicyEdgesTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0386 | DiagnosticPlacementPolicyFallbackTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0387 | DiagnosticPlacementPolicyTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0388 | DiagnosticPopupBoundsRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0389 | DiagnosticProbeFix1Tests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0390 | DiagnosticProbeInteractionTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0391 | DiagnosticProbeOverlayRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0392 | DiagnosticProbeResolverLocatorTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0393 | DiagnosticProbeResolverTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0394 | DiagnosticR1FloatingRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0395 | DiagnosticR1IdentityCompletionTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0396 | DiagnosticR1PopupRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0397 | DiagnosticR1ReportTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0398 | DiagnosticR1SnapshotTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0399 | DiagnosticRegionSelectionRegressionTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0400 | DiagnosticRegistrationRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0401 | DiagnosticSnapshotTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0402 | DiagnosticTargetBoundsRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0403 | DiagnosticTrackedHideTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0404 | DiagnosticTrackedIdentityTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0405 | DiagnosticTrackedLifecycleTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0406 | DiagnosticViewportInputPassthroughTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0407 | DiagnosticWorkspaceSelectorIdentityTests | UNREGISTERED | P2 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0408 | EngineNavigationLayoutStabilityTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0409 | EngineNavigationScrollAuthorityTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0410 | FeatureEditCR1InspectorRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0411 | FeatureEditCR1RuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0412 | FeatureEditInspectorContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0413 | FeatureEditSelectionResetTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0414 | FeatureEditSelectionResetTests.RoadVertices | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0415 | FeatureEditUiContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0416 | FeatureEditWorkflowRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0417 | GenericMarkerSnapIntegrationTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0418 | GenericRoadSnapIntegrationTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0419 | HeadlessInputInfrastructureTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0420 | InspectorEntityEditTargetTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0421 | InspectorPropertyMutabilityTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0422 | InspectorPropertyMutabilityTests.Header | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0423 | InspectorPropertyMutabilityTests.Navigation | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0424 | InspectorPropertyNavigationTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0425 | InspectorPropertyTargetTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0426 | InspectorRegionColorPreviewTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0427 | InspectorRegionColorTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0428 | InspectorSectionRailLayoutRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0429 | InspectorSectionRailScrollRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0430 | InspectorSelectionContractTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0431 | InspectorSingleFocusSectionTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0432 | LayerARuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0433 | LayerPanelRuntimeLayoutTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0434 | LayerPanelRuntimeStateTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0435 | MapLabelRasterizationAlphaRegressionTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0436 | MapLabelRasterizerTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0437 | MapMarkerInspectorPanelRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0438 | MapMarkerInspectorPersistenceTests | UNREGISTERED | P2 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0439 | MapMarkerInspectorViewportTests | UNREGISTERED | P2 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0440 | MapMarkerInspectorWorkflowTests | UNREGISTERED | P2 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0441 | MapMarkerPlacementTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0442 | MapRegionLabelProjectionTests | UNREGISTERED | P2 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0443 | MapVectorOverlayAnalyticStrokeRegressionTests | UNREGISTERED | P2 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0444 | MapVectorOverlayAnchorContractTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0445 | MapVectorOverlayDepthPolicyTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0446 | MapVectorOverlayStrokeContractTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0447 | MapVectorOverlayV1Tests.Colors | UNREGISTERED | P2 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0448 | MapVectorOverlayV1Tests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0449 | PointFeatureEntryRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0450 | R2BPropertyEditorVisualContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0451 | RegionDrawingF1ActivationRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0452 | RegionDrawingF1BTests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0453 | RegionDrawingF1BTests.VertexCount | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0454 | RegionDrawingF1CStabilityTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0455 | RegionDrawingF1FullRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0456 | RegionDrawingF1FullRuntimeTests.Names | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-MEDIUM | XYT-G-0457 | RegionDrawingF1HeadlessTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0458 | RegionDrawingF1RenderContractTests | UNREGISTERED | P2 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0459 | RegionDrawingF1ResizeTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0460 | RegionDrawingF1RuntimeRedTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0461 | RegionDrawingF2PolygonTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0462 | RegionDrawingSnapInputChainTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0463 | RegionDrawingSnapInputChainTests.Edge | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0464 | RegionDrawingSnapRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0465 | RegionDrawingSnapRuntimeTests.Edge | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0466 | RegionDrawingSnapRuntimeTests.History | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0467 | RegionDrawingSnapRuntimeTests.Persistence | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0468 | RegionPointerSafetyF2Tests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0469 | RightTabsVisibilityRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0470 | RoadDrawingSelectionF1Tests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0471 | RoadDrawingSelectionF1Tests.Names | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0472 | RoadVertexDragD2Tests | UNREGISTERED | P2 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0473 | RoadVertexSelectionD1Tests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0474 | ScaleIndicatorVisibilityRuntimeTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0475 | TerrainAutoFrameD1Tests | UNREGISTERED | P2 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0476 | TerrainAutoFrameD1Tests.Reimport | UNREGISTERED | P2 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0477 | TerrainAutoFrameLongRangeTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0478 | TerrainContextLeafHoverFixTests | UNREGISTERED | P2 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0479 | TerrainContextRuntimeFixTests | UNREGISTERED | P2 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0480 | TerrainHotpathTests | UNREGISTERED | P2 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0481 | TerrainImportOrchestrationR1Tests | UNREGISTERED | P2 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0482 | TerrainInspectorRuntimeTests | UNREGISTERED | P0 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0483 | TerrainRenderIntegrationTests | UNREGISTERED | P2 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0484 | TerrainTopContextRuntimeTests | UNREGISTERED | P2 | RENAME | Render/Terrain contract | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0485 | TopLeftInteractionR1Tests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0486 | UiR1VisualContractTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0487 | UiR1VisualFixContractTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0488 | UiRuntimeRiskTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0489 | WorkspaceSelectorR2ContractTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0490 | XYUI2R2BContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0491 | AreaCR1ContextToolbarContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0492 | CanonicalToolchainResolverTests | UNREGISTERED | P0 | RENAME | Core/domain logic | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0493 | EditorScrollAuditContractTests | UNREGISTERED | P0 | RENAME | Core/domain logic | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0494 | InspectorFix1ContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0495 | InspectorFix2ContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0496 | LayerAUiCompositionTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0497 | PointFeatureEntryContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0498 | TerrainTopContextContractTests | UNREGISTERED | P0 | KEEP | Render/Terrain contract | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0499 | TopWorkspaceSelectorR1Tests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0500 | UiCanonicalVersionContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0501 | UiCloseLifecycleContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0502 | UiCsColorRulesTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0503 | UiD2F1RegionToolActivationContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0504 | UiD2F1RegionToolContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0505 | UiD3DebtClearedTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0506 | UiD4DebtClearedTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0507 | UiD4F1ButtonContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0508 | UiD4F1LayoutModelTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0509 | UiD4F1TextOverflowContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0510 | UiD4F1TypographyContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0511 | UiD4InspectorContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0512 | UiD4LayerContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0513 | UiD4LayoutModelTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0514 | UiD4MapEditorContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0515 | UiD5ButtonContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0516 | UiD5CorrectionBehaviorTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0517 | UiD5CorrectionNotifyTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0518 | UiD5CorrectionStructureTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0519 | UiD5DangerFlowTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0520 | UiD5DialogAndLogContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0521 | UiD5FormContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0522 | UiD5InputValidationTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0523 | UiD5MapStatusTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0524 | UiD5NotificationTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0525 | UiD5UnsavedDialogBehaviorTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0526 | UiD5UnsavedDialogTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0527 | UiD5UnsavedFlowTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0528 | UiD6AccessibilityContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0529 | UiD6DpiContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0530 | UiD6LogPerformanceTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0531 | UiD6MotionContractTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0532 | UiDebtBaselineBypassF2Tests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0533 | UiDebtBaselineBypassTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0534 | UiDebtBaselineTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0535 | UiF3LayerRowContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0536 | UiLayerDeleteDialogContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0537 | UiR1FinalLeftTopContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0538 | UiSourceContractAnalyzerTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0539 | UiSourceContractAnalyzerTokenRefTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0540 | UiTokenManifestGraphTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0541 | UiTokenManifestTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0542 | UiTopTabStripContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0543 | UiTopTabStripModelHintAndListTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0544 | UiTopTabStripModelTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0545 | XyeToolbarTextPrimitiveContractTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0546 | AvaloniaPointerEventAdapterTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0547 | AvaloniaViewportInputCutoverTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0548 | CameraWheelAdapterIntegrationTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0549 | CameraWheelInputIntegrationTests | UNREGISTERED | P2 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0550 | ConsumerLifecycleIntegrationTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0551 | D1ConsumerCancellationTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0552 | D1ConsumerMigrationTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0553 | NativeKeyboardInputTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0554 | NativeViewportCoordinateContractTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0555 | NativeViewportInputForwarderTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0556 | NavigationGizmoConsumerContractTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0557 | PointerSemanticClosureTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0558 | ProductionInputCompositionTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0559 | ViewportInputConvergenceIntegrationTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0560 | ViewportGestureLifecycleTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0561 | ViewportGestureTerminalTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0562 | MapContextMenuRouterTests | UNREGISTERED | P2 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0563 | MapEditingTemporaryStateTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0564 | MapInputCancellationIntegrationTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0565 | MapInputConsumerContractTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0566 | MapInputConsumerRegressionTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0567 | NativePointerEventAdapterTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0568 | NativePointerRoutePolicyTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0569 | NativeSourceParityTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-MEDIUM | XYT-G-0570 | RegionDrawingInputModifierTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0571 | UnifiedPointerModelTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0572 | UnifiedPointerReadinessContractTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0573 | ViewportInputRouterDispatchTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0574 | ViewportInputRouterLifecycleTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0575 | ViewportInputRouterMapArbitrationTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0576 | EditorWorkspaceManagerTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0577 | EditorWorkspaceUiCompositionTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0578 | EditorWorkspaceUiTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0579 | RegionAuthoringHierarchyTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0580 | WorldPartitionInvariantTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0581 | WorldPartitionMigrationTests.Activity | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0582 | WorldPartitionMigrationTests | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0583 | WorldPartitionTests | UNREGISTERED | P0 | KEEP | World/editor domain | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0584 | WorldPartitionTests.PartitionStrategy | UNREGISTERED | P1 | KEEP | World/editor domain | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0585 | WorldPartitionUiTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0586 | AdaptiveLayoutRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0587 | BadgeRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0588 | BrushRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-MEDIUM | XYT-G-0589 | CanonicalAlignmentTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0590 | CatalogSourceTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0591 | CodeTextRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0592 | ConsumerApiTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0593 | ControlSurfaceTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0594 | DensityRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0595 | FoundationFacadeRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-MEDIUM | XYT-G-0596 | GalleryInteractionContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0597 | GalleryNavigationLayoutStabilityTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0598 | GalleryNavigationScrollAuthorityTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0599 | GallerySmokeTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0600 | GalleryThemeConstructionTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0601 | IconographyRulesTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0602 | IconRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0603 | InputRadiusCanonicalTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0604 | InspectorWorkflowControlTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0605 | MonoTextResponsiveTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0606 | MonoTextRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0607 | NavigationCollapseTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0608 | Phase1CFeedbackRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0609 | Phase1CSeparatorRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0610 | Phase1CShortcutHintRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0611 | Phase1DSearchTruncatedRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0612 | Phase1DSelectableEmptyRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0613 | Phase1DTooltipRichTextRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0614 | R5F4F1AlignmentTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0615 | R5F4FidelityTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0616 | SearchHighlightRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-MEDIUM | XYT-G-0617 | SecondTruthTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0618 | SelectableTextRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-HIGH | XYT-G-0619 | ShapeRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0620 | SkeletonTests | UNREGISTERED | P1 | KEEP | Core/domain logic | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0621 | SpatialTokenTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0622 | StatusAndIconLabelRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0623 | TerrainCut1SemanticIconTests | UNREGISTERED | P1 | KEEP | Render/Terrain contract | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0624 | ThemeRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-HIGH | XYT-G-0625 | TypographyRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-MEDIUM | XYT-G-0626 | TypographyTokenTests | UNREGISTERED | P0 | KEEP | Core/domain logic | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0627 | XYFoundationRuntimeTests | UNREGISTERED | P1 | DOWNGRADE | UI/input/integration | HIGH | HOLD — review evidence and preserve identity before Registry write |
| WAVE-LOW | XYT-G-0628 | XYUI08ShapeContractTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0629 | XYUI10StateResolverTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0630 | XYUI1CoverageTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0631 | XYUI1DocumentationTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0632 | XYUI1FidelityTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-HIGH | XYT-G-0633 | XYUI1TextRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0634 | XYUI1TextVerticalLayoutContractTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0635 | XYUI2AlignmentContractTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0636 | XYUI2BadgeContrastContractTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0637 | XYUI2Batch01ReconcileTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0638 | XYUI2BoolPropertyTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0639 | XYUI2ButtonRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0640 | XYUI2ButtonVisualStateTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0641 | XYUI2ChoiceControlsTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0642 | XYUI2ColorPickerEditLifecycleTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0643 | XYUI2ColorPickerTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0644 | XYUI2ComboBoxTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0645 | XYUI2ComponentReconcileTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0646 | XYUI2DatePickerInteractionReworkTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0647 | XYUI2DatePickerTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0648 | XYUI2DropDownButtonLayoutTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0649 | XYUI2DropDownButtonRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0650 | XYUI2DropDownButtonVisualStateTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0651 | XYUI2GhostToggleRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0652 | XYUI2GhostToggleVisualStateTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0653 | XYUI2InkAlignmentAuditTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0654 | XYUI2InputControlsTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0655 | XYUI2NumberFieldTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0656 | XYUI2PasswordFieldTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0657 | XYUI2Phase2AContractTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0658 | XYUI2Phase2ARegressionTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0659 | XYUI2Phase2BContractTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0660 | XYUI2Phase2CContractTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0661 | XYUI2Phase2CIdentityTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0662 | XYUI2Phase2DContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0663 | XYUI2PropertyControlsTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0664 | XYUI2QuickStartNormalizationTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0665 | XYUI2SearchFieldTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0666 | XYUI2SelectTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0667 | XYUI2SliderTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0668 | XYUI2SplitButtonRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-LOW | XYT-G-0669 | XYUI2SplitButtonVisualStateTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0670 | XYUI2TextAreaFocusTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0671 | XYUI2TextAreaTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0672 | XYUI2TextInputInteractionTests | UNREGISTERED | P2 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0673 | XYUI2TimePickerInteractionReworkTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0674 | XYUI2TimePickerTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0675 | XYUI2VectorPropertyLayoutStrategyTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0676 | XYUI2VectorPropertyLayoutTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0677 | XYUI3ContextToolbarLifecycleTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0678 | XYUI3MenuCapabilityTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0679 | XYUI3MenuGalleryCloseoutTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0680 | XYUI3NavigationRailWorkspaceStateTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0681 | XYUI3NavigationRailWorkspaceTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0682 | XYUI3OverlayResourceBridgeTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0683 | XYUI3Round4GalleryContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-MEDIUM | XYT-G-0684 | XYUI3TabDividerContractTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0685 | XYUI3TabSizingContractTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-MEDIUM | XYT-G-0686 | XYUI4DragFeedbackGalleryTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0687 | XYUI4DragFeedbackTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-HIGH | XYT-G-0688 | XYUI4GalleryRuntimeTests | UNREGISTERED | P0 | RENAME | UI/input/integration | HIGH | HOLD — rename only after identity and semantic review |
| WAVE-MEDIUM | XYT-G-0689 | XYUI4GalleryTests | UNREGISTERED | P0 | KEEP | UI/input/integration | MEDIUM | HOLD — complete identity/capability/blocking metadata |
| WAVE-LOW | XYT-G-0690 | XYUI4HoverStateTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0691 | XYUI4LoadingIndicatorTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0692 | XYUI4ProgressBarTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0693 | XYUI4SpinnerTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0694 | XYUI4TypographyGalleryContractTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0695 | XYUICompositionReuseTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
| WAVE-LOW | XYT-G-0696 | XYUIVectorViewportTests | UNREGISTERED | P1 | KEEP | UI/input/integration | LOW | BATCH-CANDIDATE — register after gate |
## 6. Formal integration gate

只有以下条件全部满足，才能启动 `XYT-G-INTEGRATION` 并正式修改 `docs/governance/test-registry.json`：

1. `G-CONVERGENCE = CLOSED`，并有可核验的收口证据。
2. T0 风险已关闭：摘要 LOW 数与逐行 696 条明细一致，或有正式修订记录解释差异。
3. 每条记录完成稳定 `XYT-B-NNNN` 身份分配，不复用 `XYT-G-*` 审计引用作为 Registry 身份，除非治理明确批准映射。
4. `targetCapability` 全部存在于能力映射；无法映射的记录留在 HIGH/MEDIUM，不得写入猜测值。
5. `evidenceLevel` 与 `evidenceType`、测试路径和实际执行机制一致；名称不得提升等级。
6. 每条记录补齐 Registry policy 要求的全部字段，并通过 JSON、路径、重复 ID、能力键和阻塞范围校验。
7. 正式迁移按 Wave 分批，保留本计划、审计文件和执行证据的可追溯关系；本计划不得被当作执行结果。

## 7. KNOWLEDGE / EXPERIENCE AUDIT HANDOFF

SEARCH EXISTING → UPDATE / STRENGTHEN / CREATE / NO DEPOSIT

施工事实：本轮只新增 `docs/governance/xyt-legacy-registry-migration-plan.md`；未修改 `docs/governance/test-registry.json`、测试代码或产品代码。Candidate Lesson：迁移前必须先验证审计摘要与逐行清单的数量恒等式；测试名称不能替代运行证据；P0-P4 与 T0-T3 必须严格分离；G-CONVERGENCE 未 CLOSED 时计划不得转化为 Registry 写入。请 ChatGPT 审计既有 Knowledge/Experience 后决定 CREATE / UPDATE / STRENGTHEN / NO DEPOSIT。

CHATGPT KNOWLEDGE AUDIT REQUIRED
