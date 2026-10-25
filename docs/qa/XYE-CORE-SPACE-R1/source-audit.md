# XYE-CORE-SPACE-R1 · 源码与 WP2 只读/修复边界记录

> 本文前半段记录电脑接管前的 Candidate 静态状态；Core 的本机最终审计和验证结果见文末“Core 最终架构审计”。

- Baseline: 9a4d5f9adc3fc1f2123144536626c95b0ab527ec
- Candidate branch: codex/core-space-wp2-audit-fixes-20261025-r1
- Version: v0.3.0.37-fix
- Camera source files: 10 (Projection 5 / Rays 2 / State 3)
- Core source tree after changes: 99 C# files, five first-level responsibilities, no directory with more than five direct C# source files.
- Committed changes: camera short names and aliases, robust projection and ray intersection, near-plane LOD fallback, NoData mesh and Vulkan empty-index protection, diagnostic honesty, optional high-detail rendering.
- Remaining WP2 blockers: baseline 513 samples still active; no real Windows/Vulkan before-after capture, no complete GPU performance or near-ground validation.
- Validation performed: GitHub source/tree inspection and static line counts only. Build, tests, GPU, native input and Integration NOT VERIFIED.
- Regression sources: test_ViewStateBoundaryTests.cs, test_RayAabbIntersectionTests.cs, test_TerrainVisibilitySelectorTests.cs, TerrainChunkMeshBuilderTests.cs, test_CameraFarRecoveryTests.cs.

## Core 最终架构审计（2026-10-25）

确认版本：v0.3.0.37-fix（治理与测试引用收口不推进产品版本）

### 目录与硬约束

- 审计对象：git ls-files XuanYu.Core/**/*.cs 的 99 个受版本控制源码文件；排除 bin/obj、生成文件和测试项目。
- 一级职责目录：Foundation（基础值与契约）、Scene（场景快照/历史/地图表面）、Interaction（拾取与变换工具）、Space（相机/空间查询/地形可见性）、Diagnostics（诊断与探针），共 5 个。
- 每个父目录最多 5 个直接子目录；每个源码目录最多 5 个直接 C# 文件。未发现超限项。
- 物理行数：99/99 ≤100；最大 98 行（Foundation/Math/Vector3d.cs）。
- 目录按能力和所有权分组，未发现为凑行数而增加的纯转发拆分。ViewState*.cs、EditorHistoryOwner*.cs 是同一类型的 partial 分区，分别承载投影/反投影和快照/历史操作；需要持续防止 partial 跨职责扩张。

### 职责与重复审计结论

- Foundation：数值、时间、日志和错误/结果契约；无 UI、World 或 Vulkan 依赖。
- Scene：实体快照、预览/提交变换、Undo/Redo 历史、Render Snapshot 与地图表面采样。历史状态由 EditorHistoryOwner 持有；Core 不复制 World 持久状态。
- Interaction：Move/Rotate/Scale 几何约束、布局、拖动和命中计算；PickingService 组合相机投影与射线查询，不拥有 UI 输入生命周期。
- Space：Camera/Viewport 状态、Reverse-Z 投影、反投影、屏幕射线、AABB/空间查询、地形裁剪与 LOD 选择。
- Diagnostics：只读自检、工作区路径解析、地形误差/SSE 分析与回放选项、视口帧身份/探针和 PNG 编码；不作为产品状态 Owner。
- 射线盒交集核心集中于 RayBoxInterval.TryIntersect；RayAabbIntersection 和 SpatialRayAabb 是不同结果语义的 API 入口。ReverseZProjection、WorldRayFactory、DefaultEditorCamera 与 ViewProjectionState 是现存兼容入口/契约，本轮保留以避免无产品收益的公共 API 破坏。ScaleGizmoScreenSize.cs 注释已同步到 RotateScreenRadius 短名。
- 术语：snapshot=快照，owner=状态所有者，projection/unprojection=投影/反投影，picking=拾取，culling=剔除，LOD=细节层级，probe=探针，replay=回放。

### 跨项目引用核对

- 旧符号扫描覆盖全仓 C# 调用点：SceneEntitySnapshot、ISceneRenderSnapshotSource、MapSurfaceSampler、RotateGizmoScreenRadius、MoveGizmoScreenSize 均无 C# 引用。
- 本机全解构建成功，证明 Core API、World、Editor、Editor.UI、Render.Abstractions、Render.Vulkan 及测试项目的编译引用闭合；受影响的 World Map/Move/Rotate 测试 41/41 通过。
- IRenderSnapshotSource 的实现位于 Editor.UI；SurfaceSampler 由 World Map bridge 消费；Move/Rotate 短名由 World.Tests 覆盖；EntitySnapshot 由 Scene snapshot 和渲染链消费。

### 逐文件职责、类型、成员与调用项目

每行对应一个 Core 源文件。职责列按职责目录说明其唯一工作；类型列列出声明；方法/属性列列出静态成员清单；引用项目由全仓 C# 类型名搜索得到，表示项目级调用方，不等同于精确方法调用图。反射、序列化和动态调用需结合源码判断；编译与定向测试验证调用闭合。

| 文件 | 行 | 职责 | 类型 | 方法 | 属性 | 引用项目 |
| XuanYu.Core/Diagnostics/System/CoreSelfTest.cs | 71 | Core 自检与基础契约/数值/时间/日志冒烟检查 | CoreSelfTest, CoreSelfTestItem, CoreSelfTestReport | Run | IsPassed, Name, Passed, Detail, Items | samples, XuanYu.Core.Tests, XuanYu.Editor.Win |
| XuanYu.Core/Diagnostics/System/XyeWorkspacePaths.cs | 69 | 解析并限制诊断输出路径到项目工作区 | XyeWorkspacePaths | ResolveProbeDirectory, EnsureInsideWorkspace |  | XuanYu.Editor.Control.Cli, XuanYu.Render.Abstractions, XuanYu.World.Tests |
| XuanYu.Core/Diagnostics/Terrain/MeshError/TerrainMeshErrorAnalyzer.cs | 73 | 源高程与渲染网格误差分析/统计 | TerrainMeshErrorAnalyzer | Analyze |  | tools, XuanYu.Core.Tests |
| XuanYu.Core/Diagnostics/Terrain/MeshError/TerrainMeshErrorModels.cs | 25 | 源高程与渲染网格误差分析/统计 | TerrainErrorGrid, TerrainErrorChunk, TerrainErrorOptions, TerrainErrorStatistics, TerrainErrorChunkResult, TerrainMeshErrorReport | IsValid, At | SourceToFinalMesh, Width, Height, Values, NoData, ChunkX, ChunkY, StartX, StartY, CellCountX, CellCountY, Lod, Stride, VerticalExaggeration, ExcludedSourceEdgeSamples, EstimatedPixelsPerWorldUnit, EstimatedPixelsPerMeter, SourceRowsSouthToNorth, ValidCount, NoDataCount, EdgeExcludedCount, P50Meters, P95Meters, P99Meters, MaxMeters, P50VisualMeters, P95VisualMeters, P99VisualMeters, MaxVisualMeters, EstimatedP95Pixels, EstimatedMaxPixels, Statistics, SourceToRender, RenderToMeshByChunkLod, SkirtVerticesExcluded, ScreenErrorClassification | tools, XuanYu.Core.Tests |
| XuanYu.Core/Diagnostics/Terrain/MeshError/TerrainMeshErrorStatisticsBuilder.cs | 23 | 源高程与渲染网格误差分析/统计 | TerrainErrorStatisticsBuilder | Add, Build, Validate |  | Core 内部/未静态发现 |
| XuanYu.Core/Diagnostics/Terrain/MeshError/TerrainMeshErrorSurface.cs | 38 | 源高程与渲染网格误差分析/统计 | TerrainErrorSurface | Contains, At, BilinearAt, TerrainErrorSurface(构造函数) | Chunk | Core 内部/未静态发现 |
| XuanYu.Core/Diagnostics/Terrain/Replay/TerrainReplayCameraOptions.cs | 22 | 地形回放参数、相机选项与误差表 | TerrainReplayOptions | ApplyCamera |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Terrain/Replay/TerrainReplayOptions.cs | 67 | 地形回放参数、相机选项与误差表 | TerrainReplayVariant, TerrainReplayOptions | TryParseVariant, TryParseFarPlane | Enabled, FarPlaneOverride, Variant, ChunkCellsOverride | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Terrain/Replay/TerrainReplayVariantOptions.cs | 23 | 地形回放参数、相机选项与误差表 | TerrainReplayOptions | ShouldCull, SseBudgetPixels, SelectLod |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Terrain/Replay/TerrainSseReplayErrorTable.cs | 47 | 地形回放参数、相机选项与误差表 | TerrainSseReplayError, TerrainSseReplayErrorDocument, TerrainSseReplayErrorTable | TryGet | LoadStatus, ChunkX, ChunkY, Lod, MaxVisualErrorMeters, Schema, TerrainId, Revision, Samples, Errors | XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Terrain/Sse/TerrainSseCandidateModels.cs | 30 | 屏幕空间误差测量与 LOD 候选选择 | TerrainSseLodMeasurement, TerrainSseErrorClassification, TerrainSseSelectionReason, TerrainSseCandidateSelection |  | Lod, EstimatedMaxPixels, ChosenLod, Reason, Classification | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Terrain/Sse/TerrainSseCandidateSelector.cs | 91 | 屏幕空间误差测量与 LOD 候选选择 | TerrainSseCandidateSelector | Select, SelectMeasured, Result, IsValid |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Terrain/Sse/TerrainSseProjectionEstimator.cs | 45 | 屏幕空间误差测量与 LOD 候选选择 | TerrainSseProjectionEstimator | CameraInside, NearPlaneIntersects, TryEstimateMaxPixelError |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Viewport/Frames/ViewportPresentedFrame.cs | 4 | 呈现帧身份、探针帧数据与 PNG 编码 | ViewportPresentedFrame |  | FrameId, ImageIndex, CameraRevision, CaptureHeld | XuanYu.Editor.UI |
| XuanYu.Core/Diagnostics/Viewport/Frames/ViewportProbeFrame.cs | 42 | 呈现帧身份、探针帧数据与 PNG 编码 | ViewportProbe | CameraSnapshot, CpuGpuSnapshot |  | XuanYu.Editor.App, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Viewport/Frames/ViewportProbeIdentity.cs | 20 | 呈现帧身份、探针帧数据与 PNG 编码 | ViewportProbeIdentity | Current, Format |  | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Core/Diagnostics/Viewport/Frames/VulkanPngEncoder.cs | 73 | 呈现帧身份、探针帧数据与 PNG 编码 | VulkanPngEncoder | EncodeBgra, EncodeRgba |  | tools, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Core/Diagnostics/Viewport/Probe/ViewportProbe.GpuCapture.cs | 43 | 视口探针、帧呈现关联、输入与 GPU 捕获 | ViewportProbe | LogGpuReadbackCapability, IsCaptureTargetFrame, WriteGpuCapture |  | XuanYu.Editor.App, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Viewport/Probe/ViewportProbe.Present.cs | 74 | 视口探针、帧呈现关联、输入与 GPU 捕获 | ViewportProbe | NextPresentedFrameId, ArmFrameCapture, ReleaseCapturedFrame, FrameSubmitted, FramePresented, WaitForPresentedFrameAsync | LastPresented, LastPresentedFrame, LastPresentedCameraRevision | XuanYu.Editor.App, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Viewport/Probe/ViewportProbe.cs | 79 | 视口探针、帧呈现关联、输入与 GPU 捕获 | ViewportProbe | Initialize, BeginFrame, SetWheel, NewWheel, NewNativeInput, SetNativeInput, RestoreNativeInput, RefreshLoadedAssemblies, Log | Enabled, CurrentFrameId, CurrentWheelEventId, CurrentNativeInputId | XuanYu.Editor.App, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan |
| XuanYu.Core/Diagnostics/Viewport/Probe/ViewportProbeInput.cs | 32 | 视口探针、帧呈现关联、输入与 GPU 捕获 | ViewportProbe | BeginWheel, Operation, EndWheel, CameraWriter |  | XuanYu.Editor.App, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan |
| XuanYu.Core/Foundation/Contracts/EngineError.cs | 36 | 错误、结果、实体标识契约 | EngineError | Create, ToString, EngineError(构造函数) | Code, Message, IsValid | samples, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Editor.Win |
| XuanYu.Core/Foundation/Contracts/EngineResult.cs | 40 | 错误、结果、实体标识契约 | EngineResult | Success, Fail, ToString, EngineResult(构造函数) | IsSuccess, IsFailure, Error, IsValid | samples, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Editor.Win |
| XuanYu.Core/Foundation/Contracts/EntityId.cs | 30 | 错误、结果、实体标识契约 | EntityId | FromInt, ToString, EntityId(构造函数) | Value, IsValid, None | samples, XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.Tests, XuanYu.Editor.UI, XuanYu.Editor.Win, XuanYu.Render.Abstractions, XuanYu.WarCore, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Foundation/Logging/EngineLogEntry.cs | 90 | 日志等级与日志条目校验/格式化 | EngineLogEntry | Create, ToDisplayString, ToString, ContainsLogLevelPrefix, EngineLogEntry(构造函数) | SimulationSeconds, Level, Category, Message | samples, XuanYu.Core.Tests, XuanYu.Editor.Win |
| XuanYu.Core/Foundation/Logging/EngineLogLevel.cs | 32 | 日志等级与日志条目校验/格式化 | EngineLogLevel, EngineLogLevelExtensions | ToChineseLabel |  | samples, XuanYu.Core.Tests, XuanYu.Editor.Win |
| XuanYu.Core/Foundation/Math/Vector3d.cs | 98 | 双精度三维向量与偏航旋转 | Vector3d | DistanceTo, DistanceSquaredTo, Normalize, Dot, Cross, ToString, Vector3d(构造函数) | X, Y, Z, Length, LengthSquared, IsZero, Zero, UnitX, UnitY, UnitZ, MaxAbsComponent | samples, tools, XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.Tests, XuanYu.Editor.UI, XuanYu.Editor.Win, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Foundation/Math/YawRotation.cs | 54 | 双精度三维向量与偏航旋转 | YawRotation | FromDegrees, FromRadians, ToString, NormalizeDegrees, YawRotation(构造函数) | Degrees, Radians, RotatedUnitXOnXYPlane, RotatedUnitYOnXYPlane | samples, XuanYu.Core.Tests, XuanYu.Editor.Win |
| XuanYu.Core/Foundation/Metadata/AssemblyInfo.cs | 3 | 测试可见性元数据 |  |  |  | Core 内部/未静态发现 |
| XuanYu.Core/Foundation/Time/SimulationTime.cs | 60 | 模拟时间与正时间步长 | SimulationTime | FromSeconds, FromMilliseconds, Advance, ToString, SimulationTime(构造函数) | Seconds, Milliseconds, IsZero, Zero | samples, XuanYu.Core.Tests, XuanYu.Editor.Win |
| XuanYu.Core/Foundation/Time/TimeStep.cs | 48 | 模拟时间与正时间步长 | TimeStep | FromSeconds, FromMilliseconds, ToString, TimeStep(构造函数) | Seconds, Milliseconds, IsPositive | samples, XuanYu.Core.Tests, XuanYu.Editor.Win |
| XuanYu.Core/Interaction/Gizmo/MoveDrag/MoveDragConstraint.Axes.cs | 29 | 移动拖拽轴/平面约束与求解 | MoveDragConstraint |  |  | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Core/Interaction/Gizmo/MoveDrag/MoveDragConstraint.cs | 90 | 移动拖拽轴/平面约束与求解 | MoveDragConstraint | Plane, Solve, MoveDragConstraint(构造函数) | Axis, WorldAxisLength, PointerX, PointerY, ScreenX, ScreenY, ScreenLengthSquared, PlaneScreenX, PlaneScreenY | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Core/Interaction/Gizmo/MoveDrag/MoveGizmoAxis.cs | 11 | 移动拖拽轴/平面约束与求解 | MoveGizmoAxis |  |  | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/MoveDrag/MoveGizmoPlane.cs | 46 | 移动拖拽轴/平面约束与求解 | MoveGizmoPlane | ContainsVisible, Contains, CenterDistanceSquared | Axis, A, B, C, D, HitA, HitB, HitC, HitD | XuanYu.Core.Tests |
| XuanYu.Core/Interaction/Gizmo/MoveLayout/MoveGizmoLayout.Hit.cs | 39 | 移动操纵器布局、屏幕尺寸与命中几何 | MoveGizmoLayout |  |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/MoveLayout/MoveGizmoLayout.Plane.cs | 34 | 移动操纵器布局、屏幕尺寸与命中几何 | MoveGizmoLayout |  |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/MoveLayout/MoveGizmoLayout.cs | 91 | 移动操纵器布局、屏幕尺寸与命中几何 | MoveGizmoLayout | Project, HitTest | WorldAxisLength, Segments, Planes | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/MoveLayout/MoveGizmoSegment.cs | 11 | 移动操纵器布局、屏幕尺寸与命中几何 | MoveGizmoSegment |  | Length, Axis, Start, End | XuanYu.Core.Tests |
| XuanYu.Core/Interaction/Gizmo/MoveLayout/MoveScreenSize.cs | 30 | 移动操纵器布局、屏幕尺寸与命中几何 | MoveScreenSize | ComputeWorldAxisLength |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/RotateDrag/RotateGizmoAxis.cs | 8 | 旋转拖拽几何与数学 | RotateGizmoAxis |  |  | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/RotateDrag/RotateGizmoDrag.Math.cs | 49 | 旋转拖拽几何与数学 | RotateGizmoDrag | TryInitialize |  | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Core/Interaction/Gizmo/RotateDrag/RotateGizmoDrag.cs | 96 | 旋转拖拽几何与数学 | RotateGizmoDrag | Solve, RotateGizmoDrag(构造函数) |  | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Core/Interaction/Gizmo/RotateLayout/RotateGizmoLayout.cs | 96 | 旋转环布局与屏幕半径 | RotateGizmoLayout | Project, HitTest | Rings | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/RotateLayout/RotateGizmoRing.cs | 43 | 旋转环布局与屏幕半径 | RotateGizmoRing | NearestDistance | IsEdgeOn, Axis, Center, Points, ScreenRadius | Core 内部/未静态发现 |
| XuanYu.Core/Interaction/Gizmo/RotateLayout/RotateScreenRadius.cs | 32 | 旋转环布局与屏幕半径 | RotateScreenRadius | ComputeWorldRadius |  | XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/Scale/ScaleGizmoAxis.cs | 18 | 缩放操纵器布局、拖拽与命中 | ScaleGizmoAxis, ScaleGizmoHandle |  |  | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/Scale/ScaleGizmoDrag.cs | 73 | 缩放操纵器布局、拖拽与命中 | ScaleGizmoDrag | Solve, ScaleGizmoDrag(构造函数) |  | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Core/Interaction/Gizmo/Scale/ScaleGizmoLayout.cs | 57 | 缩放操纵器布局、拖拽与命中 | ScaleGizmoLayout | Project | IsValid, Center, AxisEnd, HandleSizeDip, CenterSizeDip | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/Scale/ScaleGizmoScreenSize.cs | 31 | 缩放操纵器布局、拖拽与命中 | ScaleGizmoScreenSize | ComputeWorldAxisLength |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Interaction/Gizmo/Scale/ScaleHitTester.cs | 62 | 缩放操纵器布局、拖拽与命中 | ScaleHitTester | HitTest |  | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Core/Interaction/Picking/PickingService.cs | 52 | 视口拾取请求/结果及射线求交服务 | PickingService | Pick |  | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Core/Interaction/Picking/ViewportPickingRequest.cs | 37 | 视口拾取请求/结果及射线求交服务 | ViewportPickingRequest | ViewportPickingRequest(构造函数) | RequestSequence, Viewport, Camera, LogicalX, LogicalY, Mask, SpatialRevision | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Core/Interaction/Picking/ViewportPickingResult.cs | 15 | 视口拾取请求/结果及射线求交服务 | ViewportPickingResult |  | HasHit, EntityKey, RequestSequence, ViewportRevision, SpatialRevision, Raycast | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Core/Interaction/Screen/ScreenPoint.cs | 3 | 屏幕坐标值 | ScreenPoint |  | X, Y | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Scene/Entities/CommittedTransform.cs | 55 | 实体变换、实体快照与提交结果 | CommittedTransform | WithPosition, WithRotation, WithScale, CommittedTransform(构造函数) | Position, Rotation, Scale, Identity | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.Tests, XuanYu.Editor.UI, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Scene/Entities/EntitySnapshot.cs | 12 | 实体变换、实体快照与提交结果 | EntitySnapshot |  | IsValid, EntityKey, Name, Type, Transform | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Scene/Entities/TransformCommit.cs | 9 | 实体变换、实体快照与提交结果 | TransformCommit |  | EntityKey, Before, After, Changed | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Scene/History/EditorHistoryOwner.Snapshots.cs | 46 | Undo/Redo 状态、历史快照与变换历史项 | EditorHistoryOwner | TryPeekUndoAny, TryPeekRedoAny, TryCompleteUndo, TryCompleteRedo |  | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Scene/History/EditorHistoryOwner.cs | 94 | Undo/Redo 状态、历史快照与变换历史项 | EditorHistoryOwner, HistoryItem | Push, PushEntry, TryPeekUndo, TryUndo, TryRedo, TryUndoAny, TryRedoAny, Clear | Count, RedoCount, CurrentRevision, Entry, BeforeRevision, AfterRevision | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.Tests, XuanYu.Editor.UI, XuanYu.World.Tests, xyui |
| XuanYu.Core/Scene/History/HistorySnapshot.cs | 24 | Undo/Redo 状态、历史快照与变换历史项 | HistorySnapshot | IdentifiesSameHistoryItem, HistorySnapshot(构造函数) | Entry, BeforeRevision, AfterRevision, CurrentRevision, HistoryItemIdentity | XuanYu.Editor.UI |
| XuanYu.Core/Scene/History/TransformHistoryEntry.cs | 9 | Undo/Redo 状态、历史快照与变换历史项 | TransformHistoryEntry |  | EntityKey, Before, After | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Scene/Map/MapSurfaceKind.cs | 8 | 地图表面类型、顶点及高度采样 | MapSurfaceKind |  |  | XuanYu.Core.Tests, XuanYu.Render.Abstractions, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Scene/Map/MapTerrainVertex.cs | 11 | 地图表面类型、顶点及高度采样 | MapTerrainVertex |  | X, Y, Z, Nx, Ny, Nz, Brightness, Pad | XuanYu.Render.Abstractions, XuanYu.Render.Vulkan |
| XuanYu.Core/Scene/Map/SurfaceSampler.cs | 43 | 地图表面类型、顶点及高度采样 | SurfaceSampler | SampleHeight |  | XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Scene/Snapshots/IRenderSnapshotSource.cs | 8 | 渲染快照读取契约与不可变场景快照 | IRenderSnapshotSource |  |  | XuanYu.Editor.UI, XuanYu.World |
| XuanYu.Core/Scene/Snapshots/SceneRenderSnapshot.cs | 60 | 渲染快照读取契约与不可变场景快照 | SceneRenderSnapshot | TransformFor, PositionFor | Empty, TestEntityAtOrigin, HasEntity, Entities, CameraState, RenderTransform, RenderPosition, Entity, IsSelected, PreviewTransform, ShowMoveGizmo, RenderEntities, Camera, ShowRotateGizmo, ShowScaleGizmo | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Scene/Transform/PreviewTransform.cs | 19 | 变换预览与变换起始快照 | PreviewTransform | PreviewTransform(构造函数) | Transform, Position | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Scene/Transform/TransformStartSnapshot.cs | 8 | 变换预览与变换起始快照 | TransformStartSnapshot |  | EntityKey, Transform | XuanYu.Editor |
| XuanYu.Core/Space/Camera/Projection/ProjectionMode.cs | 8 | Reverse-Z 投影矩阵与 View/Screen 投影反投影 | ProjectionMode |  |  | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.World.Tests |
| XuanYu.Core/Space/Camera/Projection/ReverseZ.cs | 58 | Reverse-Z 投影矩阵与 View/Screen 投影反投影 | ReverseZ, ReverseZProjection | Perspective, Orthographic, CreatePerspective, CreateOrthographic |  | XuanYu.Core.Tests |
| XuanYu.Core/Space/Camera/Projection/ViewState.Project.cs | 85 | Reverse-Z 投影矩阵与 View/Screen 投影反投影 | ViewProjectionState | ProjectWorldPoint, TryProjectWorldPoint, Project, TryProject |  | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Core/Space/Camera/Projection/ViewState.Unproject.cs | 23 | Reverse-Z 投影矩阵与 View/Screen 投影反投影 | ViewProjectionState | ToWorld, TransformPointToWorld |  | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Core/Space/Camera/Projection/ViewState.cs | 85 | Reverse-Z 投影矩阵与 View/Screen 投影反投影 | ViewProjectionState | Create, TryCreate | Camera, Viewport, RenderOrigin, View, Projection, ViewProjection, InverseViewProjection | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Core/Space/Camera/Rays/RayFactory.cs | 68 | 世界射线值与视口射线构造 | RayFactory, WorldRayFactory | FromViewportPoint, FromScreen |  | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Core/Space/Camera/Rays/WorldRay.cs | 28 | 世界射线值与视口射线构造 | WorldRay | WorldRay(构造函数) | Origin, Direction | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Render.Abstractions, XuanYu.World.Tests |
| XuanYu.Core/Space/Camera/State/CameraState.cs | 94 | 相机/视口状态及默认编辑相机 | CameraState | WithFarPlane, CameraState(构造函数) | Position, Forward, Right, Up, VerticalFovDegrees, NearPlane, FarPlane, Revision, Mode, OrthographicScale | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.World.Tests |
| XuanYu.Core/Space/Camera/State/EditorCamera.cs | 27 | 相机/视口状态及默认编辑相机 | EditorCamera, DefaultEditorCamera | Create | Position, Target, Up | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Core/Space/Camera/State/ViewportState.cs | 69 | 相机/视口状态及默认编辑相机 | ViewportState | WithRevision, ViewportState(构造函数) | LogicalX, LogicalY, LogicalWidth, LogicalHeight, PhysicalWidth, PhysicalHeight, DpiScale, Revision | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Core/Space/Spatial/Bounds/SpatialAabb.cs | 57 | 空间 AABB 与统一 Bounds 值 | SpatialAabb | Intersects, Union, Translate, SpatialAabb(构造函数) | Min, Max, SurfaceArea | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Abstractions, XuanYu.Render.Vulkan, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Space/Spatial/Bounds/SpatialBounds.cs | 24 | 空间 AABB 与统一 Bounds 值 | SpatialBounds | SpatialBounds(构造函数) | EntityKey, WorldBounds, Category | XuanYu.Core.Tests, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Space/Spatial/Intersections/RayAabbHit.cs | 23 | 射线-AABB 交点/区间计算 | RayAabbHit | RayAabbHit(构造函数) | Distance, Point | Core 内部/未静态发现 |
| XuanYu.Core/Space/Spatial/Intersections/RayAabbIntersection.cs | 17 | 射线-AABB 交点/区间计算 | RayAabbIntersection | TryHit |  | XuanYu.Core.Tests, XuanYu.World |
| XuanYu.Core/Space/Spatial/Intersections/RayBoxInterval.cs | 25 | 射线-AABB 交点/区间计算 | RayBoxInterval | TryIntersect |  | Core 内部/未静态发现 |
| XuanYu.Core/Space/Spatial/Intersections/SpatialRayAabb.cs | 6 | 射线-AABB 交点/区间计算 | SpatialRayAabb | Intersects |  | XuanYu.Core.Tests, XuanYu.World |
| XuanYu.Core/Space/Spatial/Queries/SpatialQueryCategory.cs | 12 | 空间查询请求、类别、结果及统计 | SpatialQueryCategory |  |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Space/Spatial/Queries/SpatialQueryResult.cs | 15 | 空间查询请求、类别、结果及统计 | SpatialQueryResult | SpatialQueryResult(构造函数) | Candidates, Stats | XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Space/Spatial/Queries/SpatialQueryStats.cs | 13 | 空间查询请求、类别、结果及统计 | SpatialQueryStats | ToChineseProbe | SpatialRevision, TotalEntityCount, VisitedNodeCount, CandidateCount | XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Space/Spatial/Queries/SpatialRayQuery.cs | 22 | 空间查询请求、类别、结果及统计 | SpatialRayQuery | SpatialRayQuery(构造函数) | Ray, MaxDistance | XuanYu.Core.Tests, XuanYu.World, XuanYu.World.Tests |
| XuanYu.Core/Space/Spatial/Raycast/SpatialRaycastHit.cs | 13 | 射线命中值、结果与诊断统计 | SpatialRaycastHit |  | IsHit, EntityKey, HitDistance, HitPoint, SpatialRevision | XuanYu.World |
| XuanYu.Core/Space/Spatial/Raycast/SpatialRaycastResult.cs | 16 | 射线命中值、结果与诊断统计 | SpatialRaycastResult | SpatialRaycastResult(构造函数) | Hit, HasHit, Stats | XuanYu.World |
| XuanYu.Core/Space/Spatial/Raycast/SpatialRaycastStats.cs | 15 | 射线命中值、结果与诊断统计 | SpatialRaycastStats | ToChineseProbe | SpatialRevision, TotalEntityCount, VisitedNodeCount, CandidateCount, NarrowPhaseTestCount, HitCount | XuanYu.World |
| XuanYu.Core/Space/Terrain/Bounds/TerrainChunkBounds.cs | 18 | 地形分块范围、可见性与裁剪指标 | TerrainChunkBounds | TerrainChunkBounds(构造函数) | ChunkId, Bounds, CellCount | XuanYu.Render.Abstractions, XuanYu.World.Tests |
| XuanYu.Core/Space/Terrain/Bounds/TerrainChunkVisibility.cs | 33 | 地形分块范围、可见性与裁剪指标 | TerrainChunkVisibility | Select |  | XuanYu.Core.Tests |
| XuanYu.Core/Space/Terrain/Bounds/TerrainClipBoundsMetrics.cs | 53 | 地形分块范围、可见性与裁剪指标 | TerrainScreenBounds, TerrainClipBoundsMetrics | Measure | MinX, MinY, MaxX, MaxY, FrontFacingCornerCount, FarOutsideCornerCount, NearOutsideCornerCount, MinDepthRatio, MaxDepthRatio, ProjectedBounds | XuanYu.Core.Tests, XuanYu.Render.Abstractions |
| XuanYu.Core/Space/Terrain/Bounds/TerrainVisibilityTypes.cs | 16 | 地形分块范围、可见性与裁剪指标 | TerrainLodSelection, TerrainVisibilityStats, TerrainChunkVisibilityResult, TerrainChunkVisibilityEntry |  | Lod, ProjectedPixels, BaseCellPixels, VisibleChunkCount, CulledChunkCount, Lod0Count, Lod1Count, Lod2Count, Lod3Count, Lod4Count, Entries, Stats, ChunkId, Culled, Selection | XuanYu.Render.Abstractions, XuanYu.Render.Vulkan |
| XuanYu.Core/Space/Terrain/Culling/TerrainFrustumCuller.cs | 59 | 地形视锥剔除 | TerrainFrustumCuller | Intersects, TryGetRejectPlane, Transform, Corner |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Core/Space/Terrain/Lod/TerrainLodSelector.cs | 87 | 地形细节层级选择与上一帧状态缓存 | TerrainLodSelector | Select |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Core/Space/Terrain/Lod/TerrainLodStateCache.cs | 45 | 地形细节层级选择与上一帧状态缓存 | TerrainLodStateKey, TerrainLodStateCache | PreviousLod, Remember, Select, RetainOnly, Clear | Count, TerrainId, Revision, ChunkX, ChunkY | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
