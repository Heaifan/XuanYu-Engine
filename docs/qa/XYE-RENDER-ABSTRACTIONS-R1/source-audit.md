# XYE Render.Abstractions R1 · 5+100 架构审计

- 基线：d834f905f4d3b4bbccdd8e264ee47d7ad5044d68（Core 收口后）。
- 模块：XuanYu.Render.Abstractions；生产源码 91 个，Tests 项目 5 个 C# 文件。
- 原目录问题：60 个 C# 文件位于项目根；Diagnostics 下 7 个直接子目录；Diagnostics/Performance 下 7 个 C# 文件。
- 原源码行数：全部 ≤100，最大 95 行；测试文件位于独立 Tests 项目。
- 引用边界：Render.Abstractions 仅引用 Core；Editor.App、Editor.UI、Render.Vulkan、Render.Abstractions.Tests 消费本模块；无 Avalonia/Vulkan 类型依赖。
- Knowledge Planning/Execution 预检命中 Rendering 条目；本轮实际读取 K-REN-001/002、K-REN-003/004、K-NATIVE-001/002、K-ARCH-001 与 EXP-ARCH-001。保持领域坐标与渲染策略分离；本轮仅移动文件，不调整深度、绘制顺序、坐标或运行时行为。

## 目录整改方案

```text
Contracts   不可变渲染契约、DTO、接口
Geometry    纯几何/尺度算法与地形网格构建
Runtime     帧策略、最新投影队列、MapSurface 更新决策
Diagnostics 诊断事件、探针、性能样本与记录
```

各职责域内部继续按 Projection、Scene、Surface、Terrain、Host、Events、Performance 等单一职责拆分；目录深度最多三级，每个目录最多 5 个直接 C# 文件，每个父目录最多 5 个直接子目录。所有移动保持 namespace、类型名、成员签名和行为不变。测试文件统一改为 `test_` 前缀。

## 验收记录

### 目录与 5+100

- 重整后 4 个一级职责目录：Contracts（渲染契约）、Geometry（几何算法）、Runtime（运行协调）、Diagnostics（只读诊断）。
- 共 91 个生产 C# 文件；每个目录最多 5 个直接 C# 文件，每个父目录最多 5 个直接子目录；无目录/文件超限。
- 最大源码文件 95 行，91/91 均 ≤100。
- Tests 项目保留 5 个测试源码，全部在独立项目，文件名全部以 test_ 开头。
- 源码文件物理移动 84 个，7 个已在目标语义目录；测试文件重命名 5 个。namespace、类型名、成员签名、序列化字段与行为不变。
- 层级最深为职责目录 + 子职责目录 + 文件；没有继续无界嵌套。无新增 public API 或依赖。

### 文件职责、重复逻辑与状态所有权审计

- Contracts 仅定义跨项目使用的数据、接口和结果类型；不引用 Avalonia 或 Vulkan。Geometry 是纯计算与构建，Runtime 持有局部队列/更新决策，Diagnostics 负责观测和文件输出。
- 地形职责拆成全量网格 TerrainMeshBuilder、分块/LOD/裙边 TerrainChunkMeshBuilder、分区 TerrainChunkPartitioner、法线 TerrainNormalBuilder；两类网格路径共享法线算法，不重复复制；全量与分块输入/输出语义不同。
- MapBoundsGeometry 生成边界网格，MapSurfaceGeometry 生成地面表面；ReferenceGridScale 计算网格 spacing/权重，ViewportMetricScale 测量世界距离/DIP，ScaleIndicatorMetric 选择显示距离单位，三者量纲和用途不同。
- RendererEventSession 是诊断事件通道的生命周期 Owner；RendererEventWriter 管理队列写入，RendererEventFileSink 管理文件轮转，Session 的 FrameEvents/Lifecycle partial 分别处理帧事件和生命周期发布。TerrainChunkMeshBuilder.Skirts partial 只承载边界裙边生成；RenderDrawPlan.Typed partial 只承载强类型绘制计划。上述拆分对应可单独定位的问题域，不是按 100 行机械拆分。
- 本轮不改公共类型名；相机/Map/Terrain/Host/Render DTO 名称跨多个消费项目且承担明确类型区分，改名会形成公共 API 迁移。本轮目录与文件名表达用途，Tests 文件按规则统一前缀。
- 未发现新的事实 Owner 或渲染策略变化。渲染快照、缓存键、地形资源和 Host 句柄均为 DTO/标识；写入/释放状态仍由 World、Runtime 或 Vulkan Owner 持有。Native↔Avalonia 坐标、Overlay Depth、World Z 和 Draw Order 约束均未修改。

### 跨项目引用与源路径

- 唯一项目依赖为 XuanYu.Core；直接消费方包含 Editor.App、Editor.UI、Render.Vulkan 与 Render.Abstractions.Tests，World.Tests 通过 friend assembly 访问内部内容。
- file-tree.md 已同步 91 个源路径及 5 个测试文件名；全仓类型引用依 namespace 保持有效。测试未发现基于源文件物理路径的硬编码调用。
- 表格的调用项目来自全仓 C# 类型名扫描；它给出项目级使用方，不伪装成方法级精确调用图。编译和本模块完整测试用于验证真实引用。

### 生产文件逐文件清单

| 文件 | 行 | 职责 | 类型 | 方法/构造函数 | 属性/记录参数 | 字段/常量 | 调用项目 |
|---|---:|---|---|---|---|---|---|
| XuanYu.Render.Abstractions/Contracts/Host/Bridge/INativeHostSurfaceBridge.cs | 14 | Native Host surface bridge 接口 | INativeHostSurfaceBridge |  |  |  | XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Host/Bridge/INativeHostSurfaceBridgeFactory.cs | 9 | Native Host surface bridge 接口 | INativeHostSurfaceBridgeFactory |  |  |  | XuanYu.Editor.App, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Host/Lifecycle/NativeHostHandleSnapshot.cs | 13 | Native Host 句柄与生命周期状态 DTO | NativeHostHandleSnapshot |  | State, Hwnd, Width, Height, DpiScale, IsValid, Version, CapturedAt |  | XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Host/Lifecycle/NativeHostLifecycleState.cs | 14 | Native Host 句柄与生命周期状态 DTO | NativeHostLifecycleState |  |  |  | XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Host/Lifecycle/NativeHostSurfaceHandle.cs | 12 | Native Host 句柄与生命周期状态 DTO | NativeHostSurfaceHandle |  | Hwnd, Hinstance, Width, Height, DpiScale |  | XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Projection/IRenderProjectionSource.cs | 7 | 相机/渲染投影数据与提供者契约 | IRenderProjectionSource |  |  |  | XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Projection/RenderCameraProjection.cs | 28 | 相机/渲染投影数据与提供者契约 | RenderCameraProjection | ToViewProjection | Right, Position, Forward, Up, VerticalFovDegrees, NearPlane, FarPlane, Revision, Mode, OrthographicScale |  | tools, XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Projection/RenderProjection.cs | 41 | 相机/渲染投影数据与提供者契约 | RenderProjection |  | EntityCount, AssistState, StaticModelResources, VectorOverlayResources, HasMap, TerrainResources, HasTerrain, HasReferencePlane, EffectiveReferencePlane, EffectiveTerrainTransform, Camera, Entities, GizmoVisible, GizmoPosition, RotateGizmoVisible, RotateGizmoWorldRadius, ScaleGizmoVisible, ScaleGizmoWorldRadius, GizmoRotation, Assist, MoveGizmoWorldRadius, StaticModels, VectorOverlays, Map, ViewportDpiScale, ScaleIndicator, Terrain, TerrainTransform, Terrains, ReferencePlane |  | tools, XuanYu.Core, XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Projection/RenderProjectionResult.cs | 13 | 相机/渲染投影数据与提供者契约 | RenderProjectionResult | Ok, Fail | Success, Projection, FailureReason |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/Draw/RenderDrawPlan.Typed.cs | 42 | 场景绘制计划、实体投影和值类型 | RenderDrawPlan, Entry | GetTypedDrawPlan | EntityType, VertexCount, IsOutline |  | tools, XuanYu.Core, XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World, XuanYu.World.Tests, xyui |
| XuanYu.Render.Abstractions/Contracts/Scene/Draw/RenderDrawPlan.cs | 95 | 场景绘制计划、实体投影和值类型 | RenderDrawPlan, FrameEntry, RenderDrawKind | GetFrameDrawPlan | Kind, VertexCount, EntityIndex, EntityType | FillVertexCount, OutlineRibbonVertexCount, CubeFillVertexCount, CubeOutlineRibbonVertexCount, MoveGizmoVertexCount, RotateGizmoVertexCount, ScaleGizmoVertexCount, BackgroundVertexCount, FullscreenTriangleVertexCount, ReferenceGridLineCountPerAxis, ReferenceGridLineVertexCount, OriginVertexCount, WorldAxesVertexCount, MapBoundsVertexCount, MapGroundIndexCount | XuanYu.Core.Tests, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/Draw/RenderEntityProjection.cs | 13 | 场景绘制计划、实体投影和值类型 | RenderEntityProjection |  | Key, Position, Rotation, Scale, IsSelected, EntityType, StaticModelKey |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Contracts/Scene/Draw/RenderEntityType.cs | 8 | 场景绘制计划、实体投影和值类型 | RenderEntityType |  |  |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/Labels/RenderLabelBitmap.cs | 14 | GPU 标签位图数据 | RenderLabelPixelFormat, RenderLabelBitmap |  | CacheKey, Width, Height, Stride, PixelFormat, Pixels |  | XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/StaticModel/RenderStaticModelKey.cs | 7 | 静态模型键、图元、资源、变换和顶点 DTO | RenderStaticModelKey | ToString | IsValid, Value |  | XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/StaticModel/RenderStaticModelPrimitive.cs | 12 | 静态模型键、图元、资源、变换和顶点 DTO | RenderStaticModelPrimitive, RenderStaticModelColor |  | Neutral, FirstIndex, IndexCount, BaseVertex, BaseColor, R, G, B, A |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/StaticModel/RenderStaticModelResource.cs | 11 | 静态模型键、图元、资源、变换和顶点 DTO | RenderStaticModelResource |  | Key, Revision, Vertices, Indices, Primitives, LocalBounds |  | XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/StaticModel/RenderStaticModelTransform.cs | 10 | 静态模型键、图元、资源、变换和顶点 DTO | RenderStaticModelTransform |  | Identity, Position, Rotation, Scale |  | XuanYu.Core.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/StaticModel/RenderStaticModelVertex.cs | 9 | 静态模型键、图元、资源、变换和顶点 DTO | RenderStaticModelVertex |  | Position, Normal, U, V |  | XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/VectorOverlay/RenderVectorOverlayKey.cs | 6 | 矢量覆盖物键、标签、图元、资源和顶点 DTO | RenderVectorOverlayKey |  | IsValid, Value |  | XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Contracts/Scene/VectorOverlay/RenderVectorOverlayLabel.cs | 11 | 矢量覆盖物键、标签、图元、资源和顶点 DTO | RenderVectorOverlayLabel |  | Text, CacheKey, Anchor, FontSizeDip, DpiScale, Color |  | XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/VectorOverlay/RenderVectorOverlayPrimitive.cs | 17 | 矢量覆盖物键、标签、图元、资源和顶点 DTO | RenderVectorOverlayPrimitiveKind, RenderVectorOverlayPrimitive |  | FirstIndex, IndexCount, BaseVertex, Kind, Color, WidthDip, RadiusDip |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/VectorOverlay/RenderVectorOverlayResource.cs | 17 | 矢量覆盖物键、标签、图元、资源和顶点 DTO | RenderVectorOverlayResource |  | LabelInstances, LabelBitmapResources, Key, Revision, Vertices, Indices, Primitives, WorldBounds, Labels, LabelBitmaps |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Scene/VectorOverlay/RenderVectorOverlayVertex.cs | 6 | 矢量覆盖物键、标签、图元、资源和顶点 DTO | RenderVectorOverlayVertex |  | Position, Secondary, U, V |  | XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Surface/Grid/EditorViewPlaneGridKind.cs | 11 | 编辑器参考网格类别与帧状态 DTO | EditorViewPlaneGridKind |  |  |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Contracts/Surface/Grid/ReferenceGridFrameState.cs | 50 | 编辑器参考网格类别与帧状态 DTO | ReferenceGridFrameState | Create | StepMeters, AnchorX, AnchorY, BaseHeightMeters | MinStepMeters, MaxStepMeters, MinCellDip, MaxCellDip | XuanYu.Core.Tests |
| XuanYu.Render.Abstractions/Contracts/Surface/Map/MapRenderSnapshot.cs | 39 | 地图渲染快照与地表资源身份 DTO | MapRenderSnapshot |  | Empty, HasMap, MinX, MaxX, MinY, MaxY, MapId, WidthMeters, DepthMeters, SurfaceKind, BaseHeightMeters, AmplitudeMeters, WavelengthMeters, Seed, SourceChangeSequence, IsVisible, ShowGround, ShowBoundary, SessionGeneration |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Surface/Map/MapSurfaceResourceKey.cs | 29 | 地图渲染快照与地表资源身份 DTO | MapSurfaceResourceKey | From | MapId, WidthMeters, DepthMeters, BaseHeightMeters, SurfaceKind, AmplitudeMeters, WavelengthMeters, Seed, IsVisible |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Surface/ReferencePlane/ReferencePlaneRenderSnapshot.cs | 7 | 参考平面渲染快照 DTO | ReferencePlaneRenderSnapshot |  | Default, ElevationMeters |  | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Render.Abstractions/Contracts/Surface/Scale/ScaleBarGroundProjection.cs | 64 | 比例尺地面投影、度量与绘制数据 DTO | ScaleBarGroundMetric, ScaleBarGroundProjection | TryCreate | IsValid, MetersPerDip, ReferenceY, EndpointDistanceMeters |  | XuanYu.Core.Tests, XuanYu.Editor.UI |
| XuanYu.Render.Abstractions/Contracts/Surface/Scale/ScaleIndicatorGlyphLite.cs | 25 | 比例尺地面投影、度量与绘制数据 DTO | ScaleIndicatorGlyphLite | Encode, EncodeLabel |  | MaxGlyphs, Space | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Contracts/Surface/Scale/ScaleIndicatorMetric.cs | 54 | 比例尺地面投影、度量与绘制数据 DTO | ScaleIndicatorMetric | FromMetersPerDip | IsVisible, DistanceMeters, WidthDip, Label | FixedBarWidthDip, HysteresisRatio | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Surface/Scale/ScaleIndicatorOverlayProjection.cs | 12 | 比例尺地面投影、度量与绘制数据 DTO | ScaleIndicatorOverlayProjection |  | Hidden, Visible, Label, BarWidthDip | CardWidthDip, CardHeightDip | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Contracts/Surface/Viewport/EditorViewportAssistState.cs | 16 | 视口辅助状态与覆盖锚点 DTO | EditorViewportAssistState |  | Default, ShowGrid, ShowOrigin, ShowWorldAxes, ShowEditorBackground, NavGizmoHoverIndex, NavGizmoActiveIndex, NavGizmoPressedIndex, NavGizmoCenterHover, ViewPlaneGrid |  | XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Surface/Viewport/ViewportOverlayAnchor.cs | 26 | 视口辅助状态与覆盖锚点 DTO | ViewportOverlayAnchor, ViewportOverlayRect, ViewportOverlayLayoutRequest |  | Right, Bottom, X, Y, Width, Height, ViewportWidthDip, ViewportHeightDip, DesiredWidthDip, DesiredHeightDip, MarginXDip, MarginYDip, Anchor |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Contracts/Terrain/Chunk/TerrainChunkDescriptor.cs | 39 | 地形分块身份、绘制输入与 LOD DTO | TerrainChunkBounds, TerrainChunkDescriptor | WithVerticalExaggeration, Matches, HasValidSampleRange | SampleCountX, SampleCountY, MinX, MinY, MinZ, MaxX, MaxY, MaxZ, TerrainId, Revision, ChunkX, ChunkY, StartSampleX, StartSampleY, CellCountX, CellCountY, WorldBounds, MinElevation, MaxElevation |  | XuanYu.Core, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Terrain/Chunk/TerrainChunkDraw.cs | 4 | 地形分块身份、绘制输入与 LOD DTO | TerrainChunkDraw |  | ChunkX, ChunkY, Lod, TriangleCount, IndexCount |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Terrain/Chunk/TerrainChunkGpuKey.cs | 10 | 地形分块身份、绘制输入与 LOD DTO | TerrainChunkGpuKey | TerrainChunkGpuKey(构造函数) | TerrainId, Revision, VerticalExaggeration, ChunkX, ChunkY, Lod |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Terrain/Chunk/TerrainLodLevel.cs | 15 | 地形分块身份、绘制输入与 LOD DTO | TerrainLodLevel, TerrainLodLevelExtensions | Stride |  |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Terrain/Render/TerrainRenderMetadata.cs | 17 | 地形渲染资源、元数据、统计与变换 DTO | TerrainRenderExtent, TerrainRenderMetadata |  | Empty, West, South, East, North, Width, Height, ResolutionX, ResolutionY, MinElevation, MaxElevation, NoData, NoDataCount, WorldExtent |  | XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Terrain/Render/TerrainRenderResource.cs | 25 | 地形渲染资源、元数据、统计与变换 DTO | TerrainRenderAvailability, TerrainRenderResource |  | Metadata, CellSizeYMeters, TriangleIndexCount, IsAvailable, TerrainId, Revision, Heightfield, CellSizeMeters, WorldOrigin, Availability |  | tools, XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Terrain/Render/TerrainRenderStats.cs | 16 | 地形渲染资源、元数据、统计与变换 DTO | TerrainRenderStats | From | VisibleChunks, CulledChunks, LOD0Chunks, LOD1Chunks, LOD2Chunks, LOD3Chunks, LOD4Chunks, TerrainDrawCalls, TerrainTriangles, TerrainIndices, TerrainResidentGpuBytes, MeshBuildCount, VertexUploadCount, IndexUploadCount, BufferCreateCount |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Terrain/Render/TerrainRenderTransform.cs | 8 | 地形渲染资源、元数据、统计与变换 DTO | TerrainRenderTransform | VisualHeight, LogicalHeight | Default, VerticalExaggeration |  | XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Terrain/Surface/TerrainChunkMesh.cs | 11 | 地形高度场与网格 DTO | TerrainChunkMesh |  | SurfaceVertices, Vertices, Indices, SurfaceVertexCount, SampleStride |  | 本项目内部/未发现外部类型引用 |
| XuanYu.Render.Abstractions/Contracts/Terrain/Surface/TerrainHeightfield.cs | 31 | 地形高度场与网格 DTO | TerrainHeightfield | ElevationAt, TerrainHeightfield(构造函数) | Width, Height, ElevationMeters, NoDataMask, Metadata, CellSizeMeters, CellSizeYMeters |  | tools, XuanYu.Core.Tests, XuanYu.Editor.UI, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Contracts/Terrain/Surface/TerrainMesh.cs | 4 | 地形高度场与网格 DTO | TerrainMeshVertex, TerrainMesh |  | X, Y, Z, Nx, Ny, Nz, Vertices, Indices |  | XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/Identity/RendererBuildIdentity.cs | 22 | 诊断事件构建身份 | RendererBuildIdentity | Current |  |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/Queries/RendererEventReader.cs | 33 | 诊断事件读取/查询 | RendererEventReader | Read |  |  | XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/RendererEvent.cs | 31 | 事件类型、通道、代码、级别与 JSON 编码 | RendererEvent |  | EventCode, Channel, Level, RunId, SessionId, TimestampUtc, BuildIdentity, FrameId, ImageIndex, CameraRevision, DroppedCount, Generation, RequestedWidth, RequestedHeight, ActualWidth, ActualHeight, ImageViewCount, VulkanResult, Reason, TerrainId, TerrainRevision, ChunkX, ChunkY, Lod, Result, FrameSlot, FramesInFlight, SwapchainGeneration |  | XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/RendererEventChannel.cs | 7 | 事件类型、通道、代码、级别与 JSON 编码 | RendererEventChannel |  |  |  | XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/RendererEventCodes.cs | 36 | 事件类型、通道、代码、级别与 JSON 编码 | RendererEventCodes |  |  | SessionStarted, SessionClosed, FrameSubmitted, FramePresented, QueueSubmitFailed, QueuePresentFailed, QueuePresentOutOfDate, DroppedEventsSummary, ResizeStarted, ResizeCompleted, ResizeSkipped, ResizeFailed, PresentLoopStarted, PresentLoopStopped, PresentLoopStopTimeout, AcquireOutOfDate, AcquireSuboptimal, AcquireFailed, PresentSuboptimal, PresentRecoveryRequested, VulkanOperationFailed, PresentFatal, TerrainChunkObserved, TerrainResourcePresented, TerrainProbeUnavailable, SwapchainCreating, SwapchainCreated, SwapchainRecreated, SwapchainSkipped, SwapchainFailed, SwapchainReleased | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/RendererEventJson.cs | 17 | 事件类型、通道、代码、级别与 JSON 编码 | RendererEventJson |  | Options |  | XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/RendererEventLevel.cs | 10 | 事件类型、通道、代码、级别与 JSON 编码 | RendererEventLevel |  |  |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/Sessions/RendererEventSession.FrameEvents.cs | 20 | 诊断事件会话、生命周期与发布路由 | RendererEventSession | PublishFrameEvent |  |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/Sessions/RendererEventSession.Lifecycle.cs | 25 | 诊断事件会话、生命周期与发布路由 | RendererEventSession | PublishLifecycleEvent, PublishTerrainFrame |  |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/Sessions/RendererEventSession.cs | 87 | 诊断事件会话、生命周期与发布路由 | RendererEventSession | Create, Publish, DisposeAsync | SessionId, RuntimeFilePath, ProbeFilePath, RuntimeDroppedCount, ProbeDroppedCount, RuntimeWriteFailureCount, ProbeWriteFailureCount, ProbeEnabled |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/Sessions/RendererEventSessionFactory.cs | 24 | 诊断事件会话、生命周期与发布路由 | RendererEventSessionFactory | TryCreate |  |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/Sessions/RendererEventSessionOptions.cs | 11 | 诊断事件会话、生命周期与发布路由 | RendererEventSessionOptions |  | Directory, RunId, BuildIdentity, ProbeEnabled, RuntimeCapacity, ProbeCapacity, SegmentBytes, RetainedSegments |  | XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/Storage/RendererEventFileSink.cs | 67 | 诊断事件文件分段、写入与轮转 | RendererEventFileSink | WriteAsync, DisposeAsync, RendererEventFileSink(构造函数) | InitialPath |  | XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Events/Storage/RendererEventWriter.cs | 84 | 诊断事件文件分段、写入与轮转 | RendererEventWriter | TryWrite, DisposeAsync, RendererEventWriter(构造函数) | DroppedCount, WriteFailureCount |  | XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Ground/GroundProbeChain.cs | 59 | 参考平面与地面诊断探针 | GroundProbeChain | Snapshot, FrameSeen, Patch, DrawPlan, DrawCommand, FrameSummary | SequenceId |  | XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Diagnostics/Ground/ReferencePlaneDiagnosticProbe.cs | 66 | 参考平面与地面诊断探针 | ReferencePlaneDiagnosticProbe, Sample | Measure | NameId, X, Y, Origin, Direction, Parallel, Far, Valid, Distance, World |  | tools, XuanYu.Core, XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Abstractions.Tests, XuanYu.World, XuanYu.World.Tests, xyui |
| XuanYu.Render.Abstractions/Diagnostics/Ground/ReferencePlaneProbeLogger.cs | 6 | 参考平面与地面诊断探针 | ReferencePlaneProbeLogger |  |  |  | 本项目内部/未发现外部类型引用 |
| XuanYu.Render.Abstractions/Diagnostics/Host/NativeHostLifecycleLogFormatter.cs | 28 | Native Host 生命周期诊断探针与日志 | NativeHostLifecycleLogFormatter | Message, Detail, MergedMessage |  |  | XuanYu.Editor.UI |
| XuanYu.Render.Abstractions/Diagnostics/Host/NativeHostLifecycleProbe.cs | 11 | Native Host 生命周期诊断探针与日志 | NativeHostLifecycleProbe | Capture |  |  | XuanYu.Editor.UI |
| XuanYu.Render.Abstractions/Diagnostics/Performance/Samples/PerformanceFrameSample.cs | 40 | 性能帧样本、计时与地形计数 | PerformanceFrameIdentity, PerformanceOperationOutcome, PerformanceFrameSample | Valid, Unavailable | IsValid, SessionId, BuildIdentity, SourceFingerprint, FrameId, CameraRevision, DataRevision, SwapchainGeneration, FrameMode, FrameSlot, FramesInFlight, ImageIndex, Identity, Metric, Value, Unit, Status, StartThreadId, EndThreadId, StartTimestamp, EndTimestamp, ClockFrequency, TimestampPeriodNanoseconds, TimestampValidBits, Detail, SampleSequence, OperationOutcome |  | XuanYu.Render.Abstractions.Tests, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Performance/Samples/PerformanceTerrainCounters.cs | 41 | 性能帧样本、计时与地形计数 | PerformanceTerrainCounters, PerformanceTerrainSamples | Create | SelectedNodes, SubmittedChunks, SubmittedTriangles, LodBudgetSaturated, CacheHits, CacheMisses, ResourceCreates, ResourceEvictions, ResourceRetirements, UploadBytes, CacheBytes, CacheHighWaterBytes |  | XuanYu.Render.Abstractions.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Performance/Samples/PerformanceTiming.cs | 82 | 性能帧样本、计时与地形计数 | PerformanceClock, PerformanceTimingScope, GpuTimestampFrameToken, GpuTimestampConverter | GetTimestamp, ElapsedNanoseconds, Begin, Dispose, CreateSample, PerformanceTimingScope(构造函数) | Frequency, Identity, StartQueryIndex, EndQueryIndex |  | XuanYu.Render.Abstractions.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Performance/Statistics/PerformanceFenceWaitAdapter.cs | 62 | 性能样本缓冲、统计与 FenceWait 适配 | PerformanceFenceWaitTarget, RawRendererFrameIdentity, RawFenceWaitObservation, PerformanceRunIdentity, PerformanceFenceWaitAdapter | Convert | SessionId, FrameId, CameraRevision, FrameSlot, ImageIndex, SwapchainGeneration, FramesInFlight, Identity, WaitTarget, StartTimestamp, EndTimestamp, ClockFrequency, StartThreadId, EndThreadId, VulkanResult, BuildIdentity, SourceFingerprint, DataRevision, FrameMode |  | XuanYu.Render.Abstractions.Tests, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Diagnostics/Performance/Statistics/PerformanceMetric.cs | 20 | 性能样本缓冲、统计与 FenceWait 适配 | PerformanceFrameMode, PerformanceUnit, PerformanceSampleStatus, PerformanceMetric |  |  |  | XuanYu.Render.Abstractions.Tests, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Performance/Statistics/PerformanceSampleBuffer.cs | 49 | 性能样本缓冲、统计与 FenceWait 适配 | PerformanceSampleBuffer | TryWrite, Drain, PerformanceSampleBuffer(构造函数) | Capacity, Enabled, DroppedCount |  | XuanYu.Render.Abstractions.Tests, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Diagnostics/Performance/Statistics/PerformanceStatistics.cs | 39 | 性能样本缓冲、统计与 FenceWait 适配 | PerformanceMetricStatistics, PerformanceStatistics | Calculate | Metric, SampleCount, ValidCount, MissingCount, UnsupportedCount, InvalidCount, TimeoutCount, ExceptionCount, OperationTimeoutCount, OperationFailureCount, DeviceLostCount, DroppedCount, P50, P95, P99 |  | XuanYu.Render.Abstractions.Tests |
| XuanYu.Render.Abstractions/Diagnostics/Terrain/TerrainProbeLogger.cs | 81 | 地形诊断日志与快照指纹 | TerrainProbeLogger | Center, Chunk |  |  | XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Diagnostics/Terrain/TerrainSnapshotFingerprint.cs | 23 | 地形诊断日志与快照指纹 | TerrainSnapshotFingerprint | Compute |  |  | 本项目内部/未发现外部类型引用 |
| XuanYu.Render.Abstractions/Geometry/Map/MapBoundsGeometry.cs | 55 | 地图边界和地表网格生成算法 | MapBoundsGeometryBuilder, Xy | StripWidthMeters, Build | X, Y | VertexCount | tools, XuanYu.Core, XuanYu.Core.Tests, XuanYu.Editor, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World, XuanYu.World.Tests, xyui |
| XuanYu.Render.Abstractions/Geometry/Map/MapSurfaceGeometry.cs | 53 | 地图边界和地表网格生成算法 | MapSurfaceGeometry, MapSurfaceGeometryBuilder | Build, BuildPatch | Vertices, Indices | VertexCount, IndexCount | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Geometry/ReferencePlane/ReferenceGridScale.cs | 89 | 参考网格尺度、平面足迹、地平线覆盖和网格块放置算法 | ReferenceGridLevels, ReferenceGridScale | FromIdealSpacing, IdealSpacing, Compute, LargestNiceSpacingAtMost, NextNiceSpacing | FineSpacing, CoarseSpacing, FineWeight, CoarseWeight | TargetCellDip, MinSpacing, MaxSpacing | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Geometry/ReferencePlane/ReferencePlaneFootprint.cs | 51 | 参考网格尺度、平面足迹、地平线覆盖和网格块放置算法 | ReferencePlaneFootprint | Contains, TryCreate | Width, Depth, CenterX, CenterY, MinX, MaxX, MinY, MaxY |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Geometry/ReferencePlane/ReferencePlaneHorizonCoverage.cs | 30 | 参考网格尺度、平面足迹、地平线覆盖和网格块放置算法 | ReferencePlaneHorizonCoverage | VisualGroundDistance, Surrogate |  | ParallelEpsilon | XuanYu.Core.Tests |
| XuanYu.Render.Abstractions/Geometry/ReferencePlane/ReferencePlanePatchPlacement.cs | 46 | 参考网格尺度、平面足迹、地平线覆盖和网格块放置算法 | ReferencePlanePatchPlacement | Contains, From | MinX, MaxX, MinY, MaxY, CenterX, CenterY, WidthMeters, DepthMeters |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Geometry/Terrain/Build/TerrainChunkMeshBuilder.Skirts.cs | 44 | 地形分块、LOD 网格、裙边和法线构建算法 | TerrainChunkMeshBuilder |  |  |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Geometry/Terrain/Build/TerrainChunkMeshBuilder.cs | 65 | 地形分块、LOD 网格、裙边和法线构建算法 | TerrainChunkMeshBuilder | Build |  |  | XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Geometry/Terrain/Build/TerrainChunkPartitioner.cs | 49 | 地形分块、LOD 网格、裙边和法线构建算法 | TerrainChunkPartitioner | Partition |  | CellsPerChunk | tools, XuanYu.Editor.UI, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Geometry/Terrain/Build/TerrainMeshBuilder.cs | 33 | 地形分块、LOD 网格、裙边和法线构建算法 | TerrainMeshBuilder | Build |  |  | XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Geometry/Terrain/Build/TerrainNormalBuilder.cs | 52 | 地形分块、LOD 网格、裙边和法线构建算法 | TerrainNormalBuilder | Build |  |  | 本项目内部/未发现外部类型引用 |
| XuanYu.Render.Abstractions/Geometry/Viewport/ViewportMetricScale.cs | 50 | 视口公制尺度与 overlay 布局算法 | ViewportMetricScale | TryCreate | MetersPerDip, MetersPerPhysicalPixel, MetersPerPhysicalPixelX, MetersPerPhysicalPixelY, MetersPerDipX, MetersPerDipY, DpiScale |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Geometry/Viewport/ViewportOverlayLayoutResolver.cs | 36 | 视口公制尺度与 overlay 布局算法 | ViewportOverlayLayoutResolver | Resolve |  |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan |
| XuanYu.Render.Abstractions/Runtime/FrameExecutionPolicy.cs | 23 | 帧执行策略与最新渲染投影队列 | FrameExecutionPolicy, FrameStep |  | Order |  | XuanYu.Core.Tests |
| XuanYu.Render.Abstractions/Runtime/LatestRenderProjectionQueue.cs | 22 | 帧执行策略与最新渲染投影队列 | LatestRenderProjectionQueue | Publish, TryConsume |  |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Runtime/MapSurface/MapSurfaceResourceUpdatePolicy.cs | 41 | MapSurface 渲染资源更新决策与文本格式化 | MapSurfaceResourceUpdateKind, MapSurfaceResourceUpdate, MapSurfaceResourceUpdatePolicy | Decide | Kind, Key |  | XuanYu.Core.Tests, XuanYu.Render.Vulkan, XuanYu.World.Tests |
| XuanYu.Render.Abstractions/Runtime/MapSurface/MapSurfaceResourceUpdateText.cs | 14 | MapSurface 渲染资源更新决策与文本格式化 | MapSurfaceResourceUpdateText | Of |  |  | XuanYu.Render.Vulkan |
#### Tests 文件（独立测试项目）

| 文件 | 行 | 测试类 |
|---|---:|---|
| XuanYu.Render.Abstractions.Tests/Performance/test_PerformanceFenceWaitAdapterTests.cs | 95 | PerformanceFenceWaitAdapterTests |
| XuanYu.Render.Abstractions.Tests/Performance/test_PerformanceSampleBufferTests.cs | 40 | PerformanceSampleBufferTests |
| XuanYu.Render.Abstractions.Tests/Performance/test_PerformanceSampleContractTests.cs | 88 | PerformanceSampleContractTests |
| XuanYu.Render.Abstractions.Tests/Performance/test_PerformanceStatisticsTests.cs | 76 | PerformanceStatisticsTests |
| XuanYu.Render.Abstractions.Tests/Performance/test_PerformanceTerrainSamplesTests.cs | 36 | PerformanceTerrainSamplesTests |

## 验证记录（2026-10-25）

- Bootstrap：仓库根与 Git common directory 均为规范工作目录；分支 `codex/渲染器架构重建`，HEAD `d834f905f4d3b4bbccdd8e264ee47d7ad5044d68`，SDK 10.0.400，Resolver 状态 READY。
- 5+100：生产源码 91 个；直接源码文件每目录最多 5 个；直接子目录每父目录最多 5 个；最长源码 95 行。独立测试 5 个，均使用 `test_` 前缀。`file-tree.md` 收录本模块 98 个 C# / csproj 路径，缺失 0、过期 0。
- ARCH-A：`scripts/arch-a-guard.ps1` 退出码 0。
- 全解决方案构建：`scripts/xye-dotnet.ps1 build XuanYu.Engine.slnx -m:1 -nr:false -p:BuildInParallel=false -p:UseSharedCompilation=false`，退出码 0，0 warnings / 0 errors。日志：`.xye-workspace/Diagnostics/682f48e1f7a24e668a232c4ed35d3b97/stdout.log`。
- 定向测试：`XuanYu.Render.Abstractions.Tests.csproj`，18 passed / 0 failed / 0 skipped，退出码 0。日志：`.xye-workspace/Diagnostics/027b478d893848a69ea6e35e1d22f9d7/stdout.log`。
- 源码移动与测试重命名均为路径整理；无 namespace、公共类型、签名或运行逻辑变更。未执行 Vulkan/UI 视觉验收，本模块不涉及此类验收。
