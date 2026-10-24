# XYE Core Space｜静态审计与测试命名 R1

日期：2026-10-24。仓库分支：`审计XYE架构`。基线：`6f8cb7614d00fe08305a93dd1a3c3868d6af2f8a`。

## 结论与施工边界

- 生产源码 **30 文件**；Space 下四大组：Camera（相机）、Queries（空间查询）、Surface（地表）、Terrain（地形）；按现有二级/三级目录无每夹超过 5 项的问题。
- 测试原目录 `XuanYu.Core.Tests/Space` 有 12 文件平铺，违反每夹 ≤5 的既定上限。改为 Projection（4）、State（4）、Rays（2）、Visibility（1）、Support（1）。Space 的直接子目录为 5 个，低于一级警戒线 10；测试目录维持二级，不额外增加 Camera/Terrain 中间层。
- 11 份 xUnit 测试类源码统一迁移并使用 `test_` 前缀，优化 9 个测试类名与 45 个测试方法名；1 份 SpaceAssert 公用测试辅助只移动文件，不改声明。
- 代码变化只包含测试文件路径、测试类名和测试函数名，**测试断言、数据、辅助方法、命名空间、生产源码与公开 API 全部保留**。所有 12 份文件物理行数 ≤100，`test_ViewProjection.cs` 正好 100 行，后续增加内容前应拆分职责。
- 此轮执行能力仅为静态编辑/核查。Build / xUnit / 真正的 Windows/Vulkan 回放均 NOT RUN，不得称 Integration PASS。

## 生产源码逐文件清单（含方法、属性、关键字段）

术语：Reverse-Z（反向深度）、NDC（归一化设备坐标）、AABB（轴对齐包围盒）、LOD（细节层次）、Frustum（视锥体）、FOV（视场角）。

| 相对 `XuanYu.Core/Space` 的路径 | 行数 | 类型、属性、方法、用途（括号为中文释义） |
| --- | ---: | --- |
| `Camera/Projection/ProjectionMode.cs` | 8 | ProjectionMode（投影模式）枚举；Perspective（透视）、Orthographic（正交） |
| `Camera/Projection/ReverseZProjection.cs` | 50 | CreatePerspective（创建反向Z透视矩阵）；CreateOrthographic（创建反向Z正交矩阵）；ValidatePerspective（验证视角与宽高比）；ValidatePlanes（验证裁剪面）；ValidatePositive（验证正数） |
| `Camera/Projection/ViewProjectionState.Projection.cs` | 82 | ViewProjectionState partial（视图投影部分类型）；ProjectWorldPoint（世界坐标投影屏幕）；TryProjectWorldPoint（安全尝试投影） |
| `Camera/Projection/ViewProjectionState.cs` | 99 | Camera（相机）、Viewport（视口）、RenderOrigin（渲染原点）、View（视图矩阵）、Projection（投影矩阵）、ViewProjection（视投影矩阵）、InverseViewProjection（逆视投影矩阵）；Create/TryCreate（构造/尝试构造）；TransformPointToWorld（屏幕归一化坐标转世界）；ToVector3（精度转换）；RequiresRenderOrigin（需要相机相对原点） |
| `Camera/Rays/WorldRay.cs` | 28 | WorldRay（世界射线）构造；Origin（起点）、Direction（方向）；ValidateFinite（有限数值校验） |
| `Camera/Rays/WorldRayFactory.cs` | 59 | FromViewportPoint（从视口坐标建射线）；ToNdcX/ToNdcY（归一化设备坐标转换）；RequiresDoublePrecision（判断双精度需求） |
| `Camera/State/CameraState.cs` | 94 | CameraState（相机状态）构造；Position（位置）、Forward（前方向）、Right（右方向）、Up（上方向）、VerticalFovDegrees（垂直视角）、NearPlane/FarPlane（近/远裁剪面）、Revision（修订号）、Mode（模式）、OrthographicScale（正交尺度）；WithFarPlane（替换远裁剪面）；ValidateFinite（校验有限向量） |
| `Camera/State/DefaultEditorCamera.cs` | 19 | DefaultEditorCamera（默认编辑器相机）；Position（位置）、Target（注视目标）、Up（上方向）；Create（创建默认相机） |
| `Camera/State/ViewportState.cs` | 69 | ViewportState（视口状态）构造；LogicalX/LogicalY/LogicalWidth/LogicalHeight（逻辑视口坐标与尺寸）、PhysicalWidth/PhysicalHeight（物理尺寸）、DpiScale（像素缩放）、Revision（修订号）；WithRevision（替换修订号）；ValidateFinite/ValidatePositive（数值校验） |
| `Queries/Bounds/SpatialAabb.cs` | 57 | SpatialAabb（轴对齐包围盒）构造；Min/Max（最小/最大点）、SurfaceArea（表面积）；Intersects（重叠检查）、Union（合并包围盒）、Translate（平移）、Validate（有效性检查） |
| `Queries/Bounds/SpatialBounds.cs` | 22 | SpatialBounds（实体空间范围）构造；EntityKey（实体ID）、WorldBounds（世界包围盒）、Category（查询类别） |
| `Queries/Intersection/RayAabbHit.cs` | 21 | RayAabbHit（射线包围盒交点）构造；Distance（命中距离）、Point（命中点） |
| `Queries/Intersection/RayAabbIntersection.cs` | 37 | TryHit（计算射线与AABB的最近交点）；Axis（逐坐标轴求交） |
| `Queries/Intersection/SpatialRayAabb.cs` | 31 | Intersects（射线与AABB相交检查）；Axis（逐坐标轴裁剪） |
| `Queries/Raycast/SpatialRayQuery.cs` | 21 | SpatialRayQuery（空间射线查询）构造；Ray（射线）、MaxDistance（最大距离） |
| `Queries/Raycast/SpatialRaycastHit.cs` | 13 | SpatialRaycastHit（精确命中记录）：EntityKey（实体ID）、HitDistance（距离）、HitPoint（命中点）、SpatialRevision（索引代际）；IsHit（是否命中） |
| `Queries/Raycast/SpatialRaycastResult.cs` | 16 | SpatialRaycastResult（命中查询结果）构造；Hit（可选命中）、HasHit（命中标志）、Stats（统计数据） |
| `Queries/Raycast/SpatialRaycastStats.cs` | 15 | SpatialRaycastStats（射线查询统计）：SpatialRevision、TotalEntityCount（实体数）、VisitedNodeCount（节点数）、CandidateCount（候选数）、NarrowPhaseTestCount（精确测试数）、HitCount（命中数）；ToChineseProbe（输出中文探针信息） |
| `Queries/Results/SpatialQueryCategory.cs` | 12 | SpatialQueryCategory（空间查询分类）枚举 |
| `Queries/Results/SpatialQueryResult.cs` | 14 | SpatialQueryResult（候选查询结果）构造；Candidates（候选集合）、Stats（统计数据） |
| `Queries/Results/SpatialQueryStats.cs` | 13 | SpatialQueryStats（空间查询统计）：SpatialRevision、TotalEntityCount、VisitedNodeCount、CandidateCount；ToChineseProbe（中文探针摘要） |
| `Surface/MapSurfaceKind.cs` | 8 | MapSurfaceKind（地图地表类别）枚举 |
| `Surface/MapSurfaceSampler.cs` | 43 | SampleHeight（采样地表高程）；GentleHills（程序化平缓丘陵函数） |
| `Surface/MapTerrainVertex.cs` | 11 | MapTerrainVertex（地图地形顶点）：X/Y/Z（坐标）、Nx/Ny/Nz（法线）、Brightness（亮度）、Pad（填充）；Stride（顶点字节跨度） |
| `Terrain/Lod/TerrainLodSelector.cs` | 68 | TargetSampleSpacingPixels（目标像素采样间距）、HysteresisRatio（滞回比）、MinLod/MaxLod（LOD范围）；Select×2（选择LOD）；Candidate（候选级别）、Stabilize（滞回稳定）、ProjectedPixels（屏幕投影尺寸） |
| `Terrain/Lod/TerrainLodStateCache.cs` | 45 | TerrainLodStateKey（LOD缓存键：地形ID/修订/块坐标）；_previous（历史LOD字典）；Count（条目数）、PreviousLod（历史级别）、Remember（保存）、Select×2（选择并缓存）、RetainOnly（清退旧资源）、Clear（清空） |
| `Terrain/Visibility/TerrainChunkBounds.cs` | 18 | TerrainChunkBounds（块边界）构造；ChunkId（块ID）、Bounds（空间范围）、CellCount（网格单元数） |
| `Terrain/Visibility/TerrainChunkVisibility.cs` | 43 | TerrainLodSelection（选级结果）、TerrainVisibilityStats（可见性统计）、TerrainChunkVisibilityResult（输出结果）、TerrainChunkVisibilityEntry（单块记录）；TerrainChunkVisibility.Select（选取可见块并分类LOD） |
| `Terrain/Visibility/TerrainClipBoundsMetrics.cs` | 50 | TerrainScreenBounds（屏幕范围：MinX/MinY/MaxX/MaxY）；TerrainClipBoundsMetrics（前向角数、近/远裁剪外角数、深度范围、投影范围）；Measure（测量）、Corner（取AABB角点） |
| `Terrain/Visibility/TerrainFrustumCuller.cs` | 59 | Intersects（视锥是否相交）、TryGetRejectPlane（获取拒绝平面）、Transform×2（坐标变换）、Corner（包围盒角点）、Outside（视锥外侧检查） |

以上范围为 Space 中直接声明的类型、属性与函数；`record struct` 的主构造参数同时作为数据成员列明。继承的 .NET 内置成员不重复列出。生产文件含无须跨模块改名的公共契约，不作“只改文件名、不改类型名”的半套重构。

## 测试迁移 / 方法标识符核对

路径均以 `XuanYu.Core.Tests/Space/` 为前缀。表内列出新方法标识符；每个旧方法仅被替换一次，旧名与新名逐项可由 Git diff 查验。

| 旧文件名 | 新相对路径 | 新测试类 | 新方法（英文标识符） |
| --- | --- | --- | --- |
| `CameraOrthographicTests.cs` | `Projection/test_OrthographicCamera.cs` | `OrthographicCameraTests` | RequiresScaleAndDefaultsPerspective, CenterRayUsesNearPlane, CornerRaysKeepAxes, ProjectionRespectsScale, NdcMatchesWorldExtents, DepthMapsNearAndFar, IgnoresFov |
| `ReverseZOrthographicProjectionTests.cs` | `Projection/test_ReverseZOrtho.cs` | `ReverseZOrthoTests` | NearMapsToOne, FarMapsToZero, DepthIsLinear, KeepsScaleAndAspect, NdcMatchesExtents |
| `ReverseZPerspectiveProjectionTests.cs` | `Projection/test_ReverseZPerspective.cs` | `ReverseZPerspectiveTests` | NearMapsToOne, FarMapsToZero, DepthDecreases, KeepsFovAndAspect, KeepsCenterRay, NdcMatchesDistances, ExtremeRangeIsFinite |
| `ReverseZPrecisionRegressionTests.cs` | `Projection/test_ReverseZPrecision.cs` | `ReverseZPrecisionTests` | ImprovesLongRangeFloatDepth |
| `CameraStateTests.cs` | `State/test_CameraState.cs` | `CameraStateTests` | NormalizesAxes, OrthonormalizesUp, RejectsDegenerateAxes, RejectsInvalidParameters |
| `DefaultEditorCameraTests.cs` | `State/test_DefaultCamera.cs` | `DefaultCameraTests` | ForwardPointsAtTarget, ResizeKeepsCenterRay |
| `ViewProjectionStateTests.cs` | `State/test_ViewProjection.cs` | `ViewProjectionTests` | CreatesKnownView, CreatesInvertibleProjection, UpProjectsToTop, MatchesScreenAndDepth, ProjectsFrontPoint, RejectsBehindPoint, KeepsLargeWorldPrecision |
| `ViewportStateTests.cs` | `State/test_Viewport.cs` | `ViewportTests` | AcceptsDimensionsAndDpi, RevisionChangesIdentity, RejectsInvalidDimensions |
| `WorldRayFactoryTests.cs` | `Rays/test_RayFactory.cs` | `RayFactoryTests` | CenterRayMatchesForward, CornersFollowAxisConvention, RoundTripKeepsDirection, OffsetViewportUsesLocalCenter, ResizePreservesCenter, StableAndRejectsInvalid |
| `WorldRayTests.cs` | `Rays/test_WorldRay.cs` | `WorldRayTests` | RejectsInvalidDirection |
| `TerrainFrustumCullReasonTests.cs` | `Visibility/test_FrustumCull.cs` | `FrustumCullTests` | RejectsOutsideRightPlane, FarPlaneChangesClipDepth |
| `SpaceAssert.cs` | `Support/test_SpaceAssert.cs` | `SpaceAssert（辅助类保持原名）` | Near(double, double)（标量误差断言）、Near(Vector3d, Vector3d)（向量断言） |

11 类共有 45 个测试方法，外加辅助 `Near` 重载 2 项（名称未变）。新方法语义仍以所属类和断言共同限定，不为压缩名称删除区分所需的动词与关键名词。

## 结构审计、发现与风险

1. **确定性违规：** 原测试层 Space 根目录有 12 个 `.cs` 文件；已按职责拆分。
2. **暂不施工：** `ViewProjectionState` 以 `ViewProjectionState.cs` 与 `ViewProjectionState.Projection.cs` 两个 partial 文件分责，其中主文件 99 行。这是已有分责方式，不应为短名随意拆公共 API。
3. **维护性观察：** `TerrainChunkVisibility.cs` 包含 4 个数据记录类型及 1 个选择器类，虽不足 100 行，未来更改数据合同较多时可考虑按数据类型/算法职责拆分；不是本轮必须修复的错误。
4. **目录/命名空间差异：** `Space/Queries` 代码使用 `XuanYu.Core.Spatial` 命名空间，`Space/Surface` 使用 `XuanYu.Core.Map`。目前仅记录风险，不进行全仓引用更名，避免当前静态工作方式引入跨项目回归。
5. **下阶段验收：** 可编译环境恢复后，应运行 Core.Tests 定向、编译全量、5+100 静态和架构守卫；特别关注测试发现数（应保持 45 个测试方法）、同命名空间的 SpaceAssert 引用和跨模块调用。

## 本轮静态门禁

- 生产变更 0；生产公开签名变更 0。
- 测试文件 12 个重新归档，源路径 12 个移除、新路径 12 个建立；其中 11 份更名类/方法，1 份只改路径。
- 测试 `[Fact]` / `[Theory]` / `Assert.` 代码调用数经内容比较保持不变；并非执行测试。
- 每份 `.cs` 文件 ≤100 行；每个测试职责目录 ≤5 文件。所有新测试源码使用 `test_` 前缀。
- Build：NOT RUN；xUnit：NOT RUN；Integration：NOT PASS。
