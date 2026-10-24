# XYE Core 5+100 子模块审计：Observability/Replay R1

- 日期：2026-10-24；分支：`审计XYE架构`；基线：`8d8c685d38ef909dcbcca2e46fd2ef7cf47aca98`。
- 施工单元：Replay 全部 3 个职责文件夹、11 个生产源码；原有 8 个测试迁移到对应镜像职责目录并补边界测试。
- 目录门禁：`Mesh 4/5`、`Options 3/5`、`Sse 4/5`；测试镜像 `Mesh 2/5`、`Options 3/5`、`Sse 4/5`。
- 文件门禁：每个生产和测试 `.cs` 文件不超过100行；测试文件以 `test_` 命名。
- 只做 GitHub 静态编辑与审查；Build NOT RUN、xUnit NOT RUN、Windows Vulkan NOT RUN、Integration NOT PASS。

## Mesh（网格误差）｜4 个生产文件
### TerrainMeshErrorAnalyzer.cs（误差分析主入口）
- `Analyze(TerrainErrorGrid,TerrainErrorGrid,IReadOnlyList<TerrainErrorChunk>,TerrainErrorOptions?,int)`（计算源网格、渲染栅格及最终三角网格误差）
- `BuildOwners(TerrainErrorGrid,IReadOnlyList<TerrainErrorChunk>)`（建立网格采样点到分块索引的归属表）
- `CompareSourceChunk(TerrainErrorGrid,TerrainErrorGrid,TerrainErrorSurface,TerrainErrorOptions)`（计算源栅格对单个LOD分块的误差）
- `Map(TerrainErrorGrid,TerrainErrorGrid,int,int,TerrainErrorOptions)`（映射源采样坐标至渲染采样坐标）
- 本轮仅换行降低单行复杂度，不调整误差算法/LOD/对角线。

### TerrainMeshErrorModels.cs（输入与结果合同）
- `TerrainErrorGrid.IsValid(int,int)`（判定栅格样本坐标、NoData和有限值）
- `TerrainErrorGrid.At(int,int)`（读取栅格样本值）
- `TerrainErrorGrid`（尺寸/数值/NoData输入记录）
- `TerrainErrorChunk`（分块区域、LOD、stride记录）
- `TerrainErrorOptions`（垂直夸张、边界排除、像素估计、行向约定）
- `TerrainErrorStatistics`（样本数、分位数与最大误差结果）
- `TerrainErrorChunkResult`（单分块LOD统计及SourceToFinalMesh属性）
- `TerrainMeshErrorReport`（三类误差报告与像素估算可用性）
- 本轮仅折行，不变更公共记录字段及属性名称。

### TerrainMeshErrorStatisticsBuilder.cs（误差统计）
- `Add(TerrainErrorGrid,int,int,double?,int,int)`（积累有效/NoData/边缘样本误差）
- `Build()`（生成P50/P95/P99/Max及视觉误差估计）
- `Q(double)`（取指定分位数）
- `Validate(TerrainErrorGrid)`（验证栅格尺寸和数据长度）
- 修改：长度验证乘法改为long，避免 `Width*Height` 的int溢出；空输入明确拒绝。统计规则保持原样。

### TerrainMeshErrorSurface.cs（LOD三角面与双线性采样）
- `TerrainErrorSurface(TerrainErrorGrid,TerrainErrorChunk)`（创建分块采样面并校验LOD/尺寸）
- `Contains(double,double)`（判定采样坐标是否属于分块）
- `At(double,double)`（按现有BL-TR三角剖分采样）
- `BilinearAt(TerrainErrorGrid,double,double)`（双线性栅格插值）
- `Coordinates(int,int,int)`（计算带末点的分块网格坐标）
- `Find(int[],double)`（搜索采样区间）
- 修改：拒绝LOD大于30的位移环绕，用long校验分块终点及stride循环，越界/NaN双线性坐标返回null。未改三角剖分方向。

## Options（回放开关与相机选项）｜3 个生产文件
### TerrainReplayCameraOptions.cs（相机回放覆写）
- `ApplyCamera(CameraState,TerrainReplayVariant)`（应用当前远平面覆写）
- `ApplyCamera(CameraState,TerrainReplayVariant,double?)`（按变体调整远近裁剪面）
- 本轮保持原样；不扩大到真实相机模块。

### TerrainReplayOptions.cs（回放配置）
- `TryParseVariant(string?,out TerrainReplayVariant)`（解析受支持的回放变体）
- `TryParseFarPlane(string?,out double)`（解析有限正数远裁剪面）
- 属性：`Enabled`（启用状态）、`FarPlaneOverride`（远平面覆写）、`Variant`（当前变体）、`ChunkCellsOverride`（Chunk尺寸覆写）；静态字段 `EnabledAtStartup`（启动时启用状态）。
- 枚举 `TerrainReplayVariant`（Baseline、FarPlane、Culling、LOD、SSE预算等选项）；本轮保持原样。

### TerrainReplayVariantOptions.cs（变体决策）
- `ShouldCull(bool,TerrainReplayVariant)`（判断是否剔除）
- `SseBudgetPixels(TerrainReplayVariant)`（获取SSE像素预算）
- `SelectLod(int,TerrainReplayVariant)`（选择回放目标LOD）
- 修改：LodBiasOne在int最大值处不再因加1而溢出；默认语义不变。

## Sse（屏幕空间误差估计）｜4 个生产文件
### TerrainSseCandidateModels.cs（SSE决策数据）
- 记录构造 `TerrainSseLodMeasurement(int,double?)`（单个LOD估计值）
- 记录构造 `TerrainSseCandidateSelection(int,double?,TerrainSseSelectionReason,TerrainSseErrorClassification)`（决策结果）
- 枚举 `TerrainSseErrorClassification`（估计质量分类）、`TerrainSseSelectionReason`（选择原因）；无显式方法。

### TerrainSseCandidateSelector.cs（LOD估计选择）
- `Select(IReadOnlyList<TerrainSseLodMeasurement>,double,int?,double,bool,bool)`（验证输入、应用安全回退并选择LOD）
- `SelectMeasured(TerrainSseLodMeasurement[],double,int?,double)`（预算/滞回选择）
- `Result(TerrainSseLodMeasurement,double,TerrainSseSelectionReason)`（构造带分类的决定）
- `IsValid(double?)`（验证有限且非负的像素估计）
- 本轮保持算法原样；不得把 estimated pixel error 当成 GPU 实测上界。

### TerrainSseProjectionEstimator.cs（视锥与像素误差）
- `CameraInside(ViewProjectionState,SpatialAabb)`（判定相机是否在分块内）
- `NearPlaneIntersects(ViewProjectionState,SpatialAabb)`（近裁剪面是否穿过分块）
- `TryEstimateMaxPixelError(ViewProjectionState,SpatialAabb,double,out double)`（估算误差对应的物理像素）
- 修改：像素二维距离采用稳定Hypot计算；非有限估算失败时out pixels归零。未改采样位置/3×3采样数量。

### TerrainSseReplayErrorTable.cs（误差表导入）
- `TryGet(string,int,int,int,int,out double)`（查询指定地形修订、Chunk和LOD误差）
- `Load()`（按环境变量读取和反序列化误差表）
- 记录构造 `TerrainSseReplayError(int,int,int,double?)`（分块误差记录）
- 记录构造 `TerrainSseReplayErrorDocument(string,string,int,int,IReadOnlyList<TerrainSseReplayError>)`（表格及schema元数据）
- 属性 `LoadStatus`（加载状态）；静态字段 `Document`（延迟加载表）；本轮保持原样。

## 镜像测试（测试类名保留正常C#格式）
- Mesh：`test_TerrainMeshErrorAnalyzer.cs` 中 `Flat_grid_has_zero_error_and_reports_chunk_lod()`（平面误差）、`Lod_surface_uses_renderer_diagonal_and_excludes_nodata_and_edges()`（LOD对角线及NoData）、`South_to_north_source_rows_are_flipped_for_both_source_comparisons()`（数据行向）、`Source_to_render_bilinear_error_is_separate_from_triangle_mesh_error()`（双线性与三角差异）、`Make(...)`（生成测试栅格）。
- Mesh：`test_TerrainMeshBoundaries.cs` 中 `RejectsOverflowedGridCellCount()`（防维度乘法溢出）、`RejectsLodShiftWrap()`（防LOD位移环绕）、`RejectsOverflowedChunkEnd()`（防终点溢出）、`OutsideOrNonFiniteSampleHasNoSurface()`（非法采样返回null）。
- Options：`test_TerrainReplayCameraOptions.cs` 中 `Variant_changes_only_its_requested_camera_decision()`（变体不串改相机）、`Far_plane_threshold_preserves_frozen_camera_pose()`（相机姿态不变）。
- Options：`test_TerrainReplayOptions.cs` 中 `Parses_only_named_diagnostic_variants()`（配置白名单）、`Parses_finite_positive_far_plane_thresholds()`（有限正数）、`Rejects_invalid_far_plane_thresholds()`（拒绝无效远平面）。
- Options：`test_TerrainReplayVariantOptions.cs` 中 `Culling_and_lod_variants_change_only_requested_decisions()`（变体行为）、`Returns_pixel_budget_only_for_sse_variants()`（预算映射）、`Lod_bias_does_not_wrap_large_integer()`（防int最大值溢出）。
- Sse：`test_TerrainSseCandidateHysteresis.cs` 中 `Coarsens_only_when_candidate_is_below_lower_hysteresis_threshold()`（粗化阈值）、`Refines_only_after_previous_estimate_crosses_upper_hysteresis_threshold()`（细化阈值）、`Missing_previous_measurement_does_not_apply_hysteresis()`（无前态）。
- Sse：`test_TerrainSseCandidateSafety.cs` 中 `Inside_camera_forces_finest_even_when_its_estimate_is_missing()`（相机内回退）、`Near_plane_forces_finest_and_keeps_over_budget_classification()`（近裁剪面回退）、`Missing_or_invalid_estimates_fall_back_to_finest_with_explicit_classification()`（估计无效）、`Rejects_invalid_hysteresis()`（非法滞回）、`Incomplete_measurements_retain_previous_valid_lod_or_fall_back_to_finest()`（部分缺测）。
- Sse：`test_TerrainSseCandidateSelection.cs` 中 `Selects_coarsest_candidate_at_each_supported_budget_boundary()`（预算边界）、`Over_budget_candidates_choose_finest_and_remain_estimated()`（超预算回退）、`Rejects_unsupported_budgets_and_duplicate_or_negative_lods()`（非法参数）。
- Sse：`test_TerrainSseProjectionEstimator.cs` 中 `State()`（生成基准相机状态）、`Projects_vertical_error_to_physical_pixel_delta()`（投影像素误差）、`Detects_camera_inside_and_near_plane_intersection()`（相机与近裁剪面）。

## 后续风险与证据
- Mesh原有误差统计的分位数取法、重叠Chunk所有权，以及SSE的估计精度/具体GPU画质：本轮未修改；不能宣称画质通过。
- 部分旧源码仍有较长的密集表达式，文件行数及文件夹数量合规，但可读性不是全满分。
- 核对范围仅限静态源码；编译、xUnit、Windows/Vulkan、性能与WP2集成：NOT RUN。Lane静态整理完成不等于Integration PASS。
