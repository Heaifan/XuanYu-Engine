# XuanYu Engine 文件树

> 本文仅描述当前仓库结构与文件职责，不记录版本历史、阶段过程、迁移记录或测试统计。

```text
XuanYuEngine/
├── .gitattributes：Git 属性规则
├── .gitignore：Git 忽略规则
├── NuGet.Config：配置文件
├── XuanYu.Core
│   ├── .gitkeep：目录占位
│   ├── Diagnostics
│   │   └── CoreSelfTest.cs：CoreSelf测试静态类
│   ├── Gizmo
│   │   ├── MoveGizmoAxis.cs：移动Gizmo轴枚举
│   │   ├── MoveGizmoDragConstraint.Axes.cs：移动Gizmo拖动约束记录
│   │   ├── MoveGizmoDragConstraint.cs：移动Gizmo拖动约束记录
│   │   ├── MoveGizmoLayout.Hit.cs：移动Gizmo布局类
│   │   ├── MoveGizmoLayout.Plane.cs：移动Gizmo布局类
│   │   ├── MoveGizmoLayout.cs：移动Gizmo布局类
│   │   ├── MoveGizmoPlane.cs：移动Gizmo平面记录
│   │   ├── MoveGizmoScreenSize.cs：Move Gizmo 的屏幕恒定尺寸真源。CPU 布局与 Vulkan 绘制共用同一世界轴
│   │   ├── MoveGizmoSegment.cs：移动Gizmo线段记录
│   │   ├── RotateGizmoAxis.cs：旋转Gizmo轴枚举
│   │   ├── RotateGizmoDrag.Math.cs：旋转解算的纯静态数学辅助，与实例状态分离的 partial
│   │   ├── RotateGizmoDrag.cs：旋转拖拽解算：将指针在"垂直于旋转轴的平面"上的投影角度变化，映射为 欧拉度（Commit
│   │   ├── RotateGizmoLayout.cs：旋转Gizmo布局类
│   │   ├── RotateGizmoRing.cs：一条旋转环的屏幕折线几何。命中以"指针到折线最近距离"为唯一真源， 与 MoveGizmo
│   │   ├── RotateGizmoScreenRadius.cs：旋转环屏幕空间恒定尺寸换算：将目标 DIP 半径按相机深度与视口逻辑高度换算为世界半径。
│   │   ├── ScaleGizmoAxis.cs：单轴缩放手柄：修改实体自身 TRS 的局部 X / Y / Z 分量
│   │   ├── ScaleGizmoDrag.cs：Scale Gizmo 拖拽解算：指数映射，倍率恒为正、不穿过零，且不逐帧累乘。 所有倍率
│   │   ├── ScaleGizmoHitTester.cs：尺度Gizmo命中测试器静态类
│   │   ├── ScaleGizmoLayout.cs：Scale Gizmo 屏幕空间布局：三轴末端控制柄 + 中心等比控制柄。 当前没有可见
│   │   ├── ScaleGizmoScreenSize.cs：Scale Gizmo 屏幕空间恒定尺寸换算（与 RotateGizmoScreenRad
│   │   └── ScreenPoint.cs：屏幕点记录
│   ├── History
│   │   ├── EditorHistoryOwner.cs：编辑器历史持有者类
│   │   └── TransformHistoryEntry.cs：变换历史条目记录
│   ├── Identity
│   │   └── EntityId.cs：实体ID记录
│   ├── Logging
│   │   ├── EngineLogEntry.cs：Engine日志条目记录
│   │   └── EngineLogLevel.cs：Engine日志级别枚举
│   ├── Map
│   │   ├── MapSurfaceKind.cs：地表类型。与 .xymap 合同 surface.kind 对应（Editor 桥接负责字
│   │   ├── MapSurfaceSampler.cs：唯一地表采样源。 World 高度查询与 Render 网格生成必须共用本采样器，禁止第二
│   │   └── MapTerrainVertex.cs：地形网格顶点。布局与 Vulkan 侧 StaticModelVertex 一致： pos
│   ├── Math
│   │   ├── Vector3d.cs：Vector3d记录
│   │   └── YawRotation.cs：YawRotation记录
│   ├── Picking
│   │   ├── ViewportPickingRequest.cs：视口拾取Request记录
│   │   ├── ViewportPickingResult.cs：视口拾取结果记录
│   │   └── ViewportPickingService.cs：视口拾取服务静态类
│   ├── Properties
│   │   └── AssemblyInfo.cs：AssemblyInfo（核心机制与坐标数学）
│   ├── Results
│   │   ├── EngineError.cs：Engine错误记录
│   │   └── EngineResult.cs：Engine结果记录
│   ├── Scene
│   │   ├── CommittedTransform.cs：Committed变换记录
│   │   ├── ISceneRenderSnapshotSource.cs：I场景渲染快照来源接口
│   │   ├── SceneEntitySnapshot.cs：场景实体快照记录
│   │   ├── SceneRenderSnapshot.cs：场景渲染快照记录
│   │   └── SceneTransformCommitResult.cs：场景变换提交结果记录
│   ├── Space
│   │   ├── CameraState.cs：相机状态记录
│   │   ├── DefaultEditorCamera.cs：默认编辑器相机静态类
│   │   ├── ProjectionMode.cs：相机投影模式。透视=自由观察默认；正交=标准方向视图（±X/±Y/±Z）
│   │   ├── ViewProjectionState.cs：视图投影状态类
│   │   ├── ViewportState.cs：视口状态记录
│   │   ├── WorldRay.cs：世界Ray记录
│   │   └── WorldRayFactory.cs：世界Ray工厂静态类
│   ├── Spatial
│   │   ├── RayAabbHit.cs：RayAabb命中记录
│   │   ├── RayAabbIntersection.cs：RayAabbIntersection静态类
│   │   ├── SpatialAabb.cs：空间Aabb记录
│   │   ├── SpatialBounds.cs：空间边界记录
│   │   ├── SpatialQueryCategory.cs：空间QueryCategory枚举
│   │   ├── SpatialQueryResult.cs：空间Query结果类
│   │   ├── SpatialQueryStats.cs：空间QueryStats记录
│   │   ├── SpatialRayAabb.cs：空间RayAabb静态类
│   │   ├── SpatialRayQuery.cs：空间RayQuery记录
│   │   ├── SpatialRaycastHit.cs：空间Raycast命中记录
│   │   ├── SpatialRaycastResult.cs：空间Raycast结果类
│   │   └── SpatialRaycastStats.cs：空间RaycastStats记录
│   ├── Time
│   │   ├── SimulationTime.cs：SimulationTime记录
│   │   └── TimeStep.cs：TimeStep记录
│   ├── Transform
│   │   ├── PreviewTransform.cs：预览变换记录
│   │   └── TransformStartSnapshot.cs：变换Start快照记录
│   └── XuanYu.Core.csproj：项目文件
├── XuanYu.Core.Tests
│   ├── Camera
│   │   ├── CameraBasisTests.cs：唯一相机正交基生成器合同——成功结果必须三轴单位正交，失败必须明确原因
│   │   ├── CameraNavigationRollTests.cs：Orbit 地平线合同——普通环绕保持世界 +Z Up、无 Roll、不累积倾斜
│   │   ├── CameraNavigationSequenceTests.cs：导航组合链崩溃回归——顶/底视后任何导航不得再抛 CameraState 参数异常
│   │   ├── CameraNavigationStressTests.cs：-（计划 14.4）：重复导航压力测试——固定序列循环 100 次，检测累积误差与逐步失去
│   │   ├── CameraNavigationTests.cs：相机导航测试类
│   │   ├── CameraNavigationUiSequenceTests.Safety.cs：-（计划 14.5/14.6）：失败安全与状态合同——取消恢复、非法输入拒绝、导航不 Di
│   │   ├── CameraNavigationUiSequenceTests.cs：UiVm 相机导航组合序列——标准视角/Orbit/Pan/Dolly/Resize 任意
│   │   └── CameraOrthographicNavigationTests.cs：正交导航语义（Dolly 缩放尺度不动位置、Pan 保持正交、Orbit 恢复透视）+ 正
│   ├── CoreSmokeTests.cs：CoreSmoke测试类
│   ├── EditorTool
│   │   └── EditorTransformCapturePolicyTests.cs：编辑器变换捕获策略测试类
│   ├── Gizmo
│   │   ├── MoveGizmoDragConstraintTests.cs：移动Gizmo拖动约束测试类
│   │   ├── MoveGizmoLayoutG1Tests.cs：移动Gizmo布局测试类
│   │   ├── MoveGizmoLayoutPlaneTests.cs：移动Gizmo布局测试类
│   │   ├── MoveGizmoLayoutTests.cs：移动Gizmo布局测试类
│   │   ├── MoveGizmoLayoutVulkanTests.cs：移动Gizmo布局测试类
│   │   ├── MoveGizmoScreenSizeTests.cs：移动Gizmo屏幕尺寸测试类
│   │   ├── RotateGizmoLayoutTests.cs：旋转Gizmo布局测试类
│   │   ├── ScaleGizmoTests.Drag.cs：尺度Gizmo测试类
│   │   ├── ScaleGizmoTests.DragSafety.cs：尺度Gizmo测试类
│   │   ├── ScaleGizmoTests.Helpers.cs：尺度Gizmo测试类
│   │   ├── ScaleGizmoTests.R5R1.cs：尺度Gizmo测试类
│   │   └── ScaleGizmoTests.cs：Scale Gizmo 纯函数契约测试 —— 单轴只改对应分量、Uniform 三轴同倍、
│   ├── History
│   │   ├── EditorHistoryOwnerTests.cs：编辑器历史持有者测试类
│   │   ├── EditorHistoryRedoTests.cs：编辑器历史重做测试类
│   │   ├── TransformHistoryIntegrationTests.cs：变换历史Integration测试类
│   │   └── TransformHistoryRedoIntegrationTests.cs：变换历史重做Integration测试类
│   ├── Picking
│   │   └── ViewportPickingServiceTests.cs：视口拾取服务测试类
│   ├── Render
│   │   ├── CubeRenderDrawPlanTests.cs：Cube渲染绘制计划测试类
│   │   ├── FrameExecutionPolicyTests.cs：验证 Vulkan Present 循环帧执行顺序： WaitFence → ApplyP
│   │   ├── MapRenderDrawPlanTests.cs：MAP-A--/-（-）：RenderProjection 携带地图快照后，参考网格保留（
│   │   ├── MapSurfaceGeometryTests.cs：有限地面常量几何合同——固定 4 顶点 6 索引，尺寸只进顶点坐标
│   │   ├── MapSurfaceResourceKeyTests.cs：GPU 资源判等键合同——Rename 不重建、几何变化必重建、Sequence 不进键
│   │   ├── MapSurfaceResourceUpdatePolicyTests.cs：地图 GPU 资源更新决策（纯策略）——旧序号拒绝、同键不重建、异键重建
│   │   ├── NavigationGizmoLayoutTests.Facing.cs：导航 Gizmo 正对相机合同——轴正对时只显示朝向端点、隐藏背向端点、命中优先端点
│   │   ├── NavigationGizmoLayoutTests.cs：-//-：导航 Gizmo 布局投影与命中测试（96 DIP 区域；正对合同见 .Faci
│   │   ├── NavigationGizmoOverlayContractTests.cs：导航 Gizmo Overlay Pass 与屏幕空间原点标记合同测试。 1. DrawP
│   │   ├── ReferenceGridAdaptiveTests.cs：参考网格片元行为合同（CPU 镜像）。 互补交叉淡化（不再允许两权重同时为 1）；方向性密
│   │   ├── ReferenceGridDrawPlanTests.cs：DrawPlan 合同——顺序（方案 12）与开关独立（方案 11.2）。 顺序：地形(M
│   │   ├── ReferenceGridRayIntersectionTests.cs：GRID-G1：世界射线与 Z=0 平面求交的数学合同。 片元着色器逻辑（editor_r
│   │   ├── ReferenceGridScaleTests.cs：每帧全局网格尺度合同（1/2/5 序列 + 互补交叉淡化）。 尺度计算不接收世界位置（AP
│   │   ├── ReferenceGridShaderContractTests.cs：Shader 合同低层门禁（方案 15.5）。 只做防止误删/防退化的字符串检查，不声称视
│   │   ├── ReferenceGridVisualStyleTests.cs：网格视觉样式合同（10.1）与重合合成合同（10.2）。 唯一像素线宽：Fine == C
│   │   ├── RenderDrawPlanTests.cs：验证绘制计划——未选中仅 Fill(3)，选中 Fill(3) + OutlineRibb
│   │   ├── SceneRenderProjectionAdapterTests.Rotation.cs：场景渲染投影适配器测试类
│   │   ├── SceneRenderProjectionAdapterTests.Selection.cs：场景渲染投影适配器测试类
│   │   ├── SceneRenderProjectionAdapterTests.cs：场景渲染投影适配器测试类
│   │   ├── StandardViewResolverTests.cs：六方向标准视角解析测试（计划 11.4——Pivot/距离保留、Up 合同、无滚转/镜像）
│   │   ├── StaticModelDepthRegressionTests.cs：StaticModel深度Regression测试类
│   │   ├── StaticModelRenderContractTests.cs：StaticModel渲染合同测试类
│   │   ├── ViewportAssistDrawPlanTests.cs：导航 Gizmo 恒为最后一项（Overlay Pass 收尾）
│   │   └── ViewportChromeContractTests.cs：视口黑边合同测试（计划 11.1）——XAML 防退化： 视口外层无深色粗边框/大圆角/大
│   ├── Space
│   │   ├── CameraOrthographicTests.cs：正交投影契约（模式校验/射线/尺度投影/往返/深度/Fov 无关）
│   │   ├── CameraStateTests.cs：相机状态测试类
│   │   ├── DefaultEditorCameraTests.cs：默认编辑器相机测试类
│   │   ├── SpaceAssert.cs：SpaceAssert（Core 合同测试）
│   │   ├── ViewProjectionStateTests.cs：视图投影状态测试类
│   │   ├── ViewportStateTests.cs：视口状态测试类
│   │   ├── WorldRayFactoryTests.cs：世界Ray工厂测试类
│   │   └── WorldRayTests.cs：世界Ray测试类
│   ├── Spatial
│   │   ├── RayAabbIntersectionTests.cs：RayAabbIntersection测试类
│   │   ├── SpatialBoundsTests.cs：空间边界测试类
│   │   └── SpatialTestData.cs：空间测试数据（Core 合同测试）
│   └── XuanYu.Core.Tests.csproj：项目文件
├── XuanYu.Editor
│   ├── Assets
│   │   ├── AssetId.cs：资产ID记录
│   │   ├── GlbContainer.cs：GlbContainer（编辑器领域服务）
│   │   ├── GlbImportService.cs：Glb导入服务类
│   │   ├── GltfAccessorReader.cs：GLTFAccessorReader（编辑器领域服务）
│   │   ├── GltfCoordinatePolicy.cs：GLTF坐标策略静态类
│   │   ├── GltfJsonAccess.cs：GLTFJSONAccess（编辑器领域服务）
│   │   ├── GltfNodeTransform.cs：GLTF节点变换（编辑器领域服务）
│   │   ├── GltfStaticModelImporter.cs：GLTFStaticModelImporter（编辑器领域服务）
│   │   ├── HostedSceneAsset.cs：托管资产项。SourcePath 是 导入时记录的规范化绝对路径（运行时来源）； Rela
│   │   ├── ImportStop.cs：导入Stop（编辑器领域服务）
│   │   ├── ModelAssetRuntimeState.cs：Model资产Runtime状态枚举
│   │   ├── SceneAssetHostingError.cs：托管事务错误码。复用 SceneDocumentResult 的 ErrorCode 字符
│   │   ├── SceneAssetHostingPlan.cs：托管规划。Assets 按 AssetId.Value 稳定排序；所有绝对路径已 GetF
│   │   ├── SceneAssetHostingPlanner.cs：托管规划生成。只计算路径与规划，不写磁盘
│   │   ├── SceneAssetHostingState.cs：托管事务状态机
│   │   ├── SceneAssetHostingTransaction.Activate.cs：Activate 将 staging 激活为正式 .xyassets，同时保留旧目录为备份
│   │   ├── SceneAssetHostingTransaction.Complete.cs：Complete 在后续场景文件保存成功后调用，删除备份并收尾
│   │   ├── SceneAssetHostingTransaction.Rollback.cs：Rollback 恢复旧目录。旧数据安全优先于清理整洁
│   │   ├── SceneAssetHostingTransaction.cs：托管资源事务。Prepare 只写 staging；Activate 激活正式 .xyas
│   │   ├── SceneAssetPathPolicy.cs：场景资产路径策略静态类
│   │   ├── SceneStaticModelBinding.cs：场景内实体 → 托管资产的最小绑定记录。 注意：Editor 层不引用 Render.Ab
│   │   ├── SceneStaticModelCatalog.cs：场景静态模型绑定目录。Editor 层唯一事实源：实体 → 资产 → 模型数据。 不存储
│   │   ├── StaticModelAuthoringService.cs：publicsealedrecordStaticModelAuthor结果记录
│   │   ├── StaticModelBuilder.cs：StaticModel构建器（编辑器领域服务）
│   │   ├── StaticModelColor.cs：StaticModel颜色记录
│   │   ├── StaticModelData.cs：publicsealedrecordStaticModel数据记录
│   │   ├── StaticModelImportCodes.cs：StaticModel导入错误Code枚举
│   │   ├── StaticModelImportResult.cs：publicsealedrecordStaticModel导入结果记录
│   │   ├── StaticModelImportWarning.cs：publicsealedrecordStaticModel导入Warning记录
│   │   ├── StaticModelPrimitive.cs：StaticModelPrimitive记录
│   │   └── StaticModelVertex.cs：StaticModel顶点记录
│   ├── Camera
│   │   ├── CameraBasis.cs：唯一相机正交基生成器（Editor 相机规则；不进入 Core，不持有 UiVm/Vulk
│   │   ├── CameraFrameResult.cs：相机取景结果记录
│   │   ├── CameraNavigation.Try.cs：失败安全导航入口（partial）——Try* 成功才输出结果；失败给出原因且不修改任何状
│   │   ├── CameraNavigation.cs：相机导航类
│   │   ├── EditorCameraFraming.Orthographic.cs：正交取景。保持当前正交模式与观察方向，尺度按包围范围适配 （竖直跨度与水平跨度/宽高比取大
│   │   ├── EditorCameraFraming.cs：编辑器相机取景类
│   │   └── OrthographicViewFactory.cs：正交视图生成。六方向标准视图（±X/±Y/±Z）切换为正交投影时， 正交尺度取当前透视相机
│   ├── MapDocument
│   │   ├── MapDocument.cs：地图文档 DTO（.xymap v1 持久化模型）。表达地图文件数据， 不负责文件、UI、
│   │   ├── MapDocumentAggregateBridge.cs：.xymap v1 DTO → 领域聚合投影（场景 mapReference 保活链）。
│   │   ├── MapDocumentJson.cs：地图文档JSON（编辑器领域服务）
│   │   ├── MapDocumentOwner.cs：当前地图状态所有者（最小状态机）。 无地图 / 新建未保存 / 已加载 / 已修改；不负责
│   │   ├── MapDocumentResult.cs：地图操作结构化结果（对齐 SceneDocumentResult 模式，语义独立）
│   │   ├── MapDocumentValidator.cs：地图文档 DTO（.xymap v1）严格校验。领域合法性（尺寸范围）单一事实源在 Wor
│   │   ├── MapEnvironmentDefinition.cs：环境定义。 只保存与校验，不渲染。 sunDirection 指向光源方向（光射来方向），
│   │   ├── MapJsonMapper.cs：地图JSON映射（编辑器领域服务）
│   │   ├── MapJsonSerializer.cs：.xymap 严格 JSON 读写。字段大小写敏感、未知字段拒绝、确定性输出、UTF-8
│   │   └── MapStorageService.cs：地图文件存储。候选加载 + 同目录临时文件原子保存，不直接替换任何状态
│   ├── MapEditing
│   │   ├── MapEditEvents.cs：地图编辑低频事件参数（禁止记录鼠标移动/Hover/每帧渲染）
│   │   ├── MapEditReason.cs：地图编辑原因（内容变更事件携带）
│   │   ├── MapEditSession.Commands.cs：地图基础属性编辑命令（ 只实现地图级修改，图层/区域命令属 /）
│   │   ├── MapEditSession.Commit.cs：统一提交管线。所有地图内容修改必须经过本方法： 纯修改函数 → 候选 → No-op 检测
│   │   ├── MapEditSession.Document.cs：文档生命周期（新建/替换/标记已保存）
│   │   ├── MapEditSession.History.cs：Undo/Redo 与事件广播。历史游标移动恢复对应 MapDefinition； Cha
│   │   ├── MapEditSession.Selection.cs：选择状态。只保存稳定 ID；选择不产生 Dirty、不写入历史
│   │   ├── MapEditSession.cs：地图编辑会话（唯一状态权威）。 CurrentMap 是唯一地图内容；历史直接复用 Cor
│   │   ├── MapHistoryEntry.cs：地图历史条目（不可变快照）。MapDefinition 与 ImmutableArray
│   │   ├── MapSelection.cs：地图选择状态。只保存稳定 ID，不保存 UI 控件/列表下标/中文名。 Region 选择
│   │   └── MapSelectionKind.cs：地图选择类型（未选择/地图/图层/区域）
│   ├── SceneDocument
│   │   ├── MapReference.cs：场景对地图的可选引用（ 合同冻结）。 只保存 mapId + 项目相对 assetPath
│   │   ├── SceneDocumentAsset.cs：场景资产记录（ 合同字段）。只描述托管来源，不含顶点/索引/GPU 数据
│   │   ├── SceneDocumentEntity.cs：场景文档实体记录
│   │   ├── SceneDocumentJson.cs：场景文档JSON（编辑器领域服务）
│   │   ├── SceneDocumentLoadTransaction.cs：加载候选阶段。只读构建候选，不修改当前 World/Catalog/Selection/H
│   │   ├── SceneDocumentMapper.cs：场景文档映射（编辑器领域服务）
│   │   ├── SceneDocumentResult.cs：publicsealedrecord场景文档结果记录
│   │   ├── SceneDocumentSaveTransaction.cs：保存完整事务。候选构建 → Hosting Prepare/Activate → 原子写
│   │   ├── SceneDocumentSession.cs：场景文档会话类
│   │   ├── SceneDocumentSnapshot.cs：publicsealedrecord场景文档快照记录
│   │   ├── SceneDocumentValidator.MapReference.cs：场景文档校验地图引用（编辑器领域服务）
│   │   ├── SceneDocumentValidator.cs：场景文档校验（编辑器领域服务）
│   │   ├── SceneDocumentWorldBridge.cs：场景文档世界桥接静态类
│   │   ├── SceneLoadCandidate.cs：加载候选。候选阶段构建，提交阶段一次性替换 World/Catalog
│   │   ├── SceneSaveOutcome.cs：保存事务结果。SavedSnapshot 带 v3 Assets；HostedSource
│   │   └── SceneStorageService.cs：场景存储服务类
│   ├── Transform
│   │   ├── TransformSession.Rotate.cs：变换会话类
│   │   ├── TransformSession.Scale.cs：变换会话类
│   │   └── TransformSession.cs：变换会话类
│   └── XuanYu.Editor.csproj：项目文件
├── XuanYu.Editor.App
│   ├── EditorCompositionRoot.cs：编辑器Composition根静态类
│   ├── Program.cs：入口（应用组合根）
│   └── XuanYu.Editor.App.csproj：项目文件
├── XuanYu.Editor.UI
│   ├── Bootstrap
│   │   ├── App.axaml：界面布局
│   │   ├── App.axaml.cs：应用Application类
│   │   └── Program.cs：入口（界面与组合）
│   ├── Dialogs
│   │   ├── IEditorDialogService.cs：最小错误弹窗服务。只用于用户主动操作失败（导入 GLB / 打开场景 / 部分资源缺失）。
│   │   └── NullEditorDialogService.cs：Null编辑器Dialog服务（界面与组合）
│   ├── EditorState
│   │   ├── EditorInteractionChangedResult.cs：编辑器交互变更种类BeganPreviewedCommittedCanceled枚举
│   │   ├── EditorInteractionCommand.cs：publicsealedrecordBegin交互命令记录
│   │   ├── EditorInteractionPointerSnapshot.cs：编辑器交互指针快照记录
│   │   ├── EditorInteractionSnapshot.cs：编辑器交互PhaseIdleCaptured枚举
│   │   ├── EditorSelectionCommand.cs：publicsealedrecordSelect编辑器项命令记录
│   │   ├── EditorSelectionSnapshot.cs：publicsealedrecord编辑器选择快照记录
│   │   ├── EditorStateChangedResult.cs：编辑器状态变更种类枚举
│   │   ├── EditorStateOwner.Interaction.cs：编辑器状态持有者类
│   │   ├── EditorStateOwner.Tool.cs：编辑器状态持有者类
│   │   ├── EditorStateOwner.cs：编辑器状态持有者类
│   │   ├── EditorToolChangedResult.cs：publicsealedrecord编辑器工具Changed结果记录
│   │   ├── EditorToolCommand.cs：publicsealedrecord变更编辑器工具命令记录
│   │   ├── EditorToolId.cs：编辑器工具ID枚举
│   │   ├── EditorToolSnapshot.cs：publicsealedrecord编辑器工具快照记录
│   │   ├── EditorToolText.cs：编辑器工具文本静态类
│   │   └── EditorTransformCapturePolicy.cs：编辑器变换捕获策略静态类
│   ├── Foot
│   │   ├── Foot.axaml：界面布局
│   │   ├── Foot.axaml.cs：Foot.axaml.cs 只做接线——自动滚动 controller、日志选中、Ctrl
│   │   ├── LogDetailPanel.axaml：界面布局
│   │   ├── LogDetailPanel.axaml.cs：日志Detail面板UserControl类
│   │   └── LogListAutoScrollController.cs：日志ListAutoScrollControllerIDisposable类
│   ├── Icons
│   │   └── EditorIcons.axaml：界面布局
│   ├── Left
│   │   ├── InlineRenameActivation.cs：Inline重命名Activation静态类
│   │   ├── Left.EntityCommands.cs：Left类
│   │   ├── Left.Styles.axaml：界面布局
│   │   ├── Left.axaml：界面布局
│   │   └── Left.axaml.cs：LeftUserControl类
│   ├── Main
│   │   ├── Main.axaml：界面布局
│   │   └── Main.axaml.cs：主UserControl类
│   ├── NativeHostResizeCoalescer.cs：/ <summary> / 合并连续尺寸变化：连续 SizeChanged 只更新快照与合
│   ├── NativeHostResizeSnapshot.cs：/ <summary> / 尺寸变化快照：只保存尺寸相关数据，不含生命周期日志或合并逻辑。
│   ├── NativeHostSurfaceContract.cs：把现有 NativeHost 生命周期快照映射为渲染层交接句柄。 只搬运 HWND / 尺
│   ├── RelayCommand.cs：Relay命令I命令类
│   ├── Right
│   │   ├── MapEditorPanel.axaml：界面布局
│   │   ├── MapEditorPanel.axaml.cs：地图编辑器面板（地图资产/基础地表/环境三组，DataContext=UiVm）
│   │   ├── Right.axaml：界面布局
│   │   └── Right.axaml.cs：右侧UserControl类
│   ├── Root
│   │   ├── UiRoot.axaml：界面布局
│   │   └── UiRoot.axaml.cs：Ui根UserControl类
│   ├── Top
│   │   ├── Top.axaml：界面布局
│   │   └── Top.axaml.cs：TopUserControl类
│   ├── TreeGuide.cs：树引导Control类
│   ├── TreeGuideSegment.cs：树引导线段种类枚举
│   ├── Ui.axaml：界面布局
│   ├── Viewport
│   │   ├── ViewNavigationGizmo.HitTest.cs：-/-：导航 Gizmo 命中测试——六端点与中心球。 命中半径 ≥13 DIP；重叠时最
│   │   ├── ViewNavigationGizmo.Layout.cs：-/-：导航 Gizmo 布局纯数学——六个世界方向投影到 Gizmo 屏幕平面。 投影：
│   │   └── Vulkan
│   │       ├── NativePointerMessage.cs：原生指针消息记录
│   │       ├── VulkanNativeHost.AvaloniaCamera.cs：Vulkan原生宿主类
│   │       ├── VulkanNativeHost.AvaloniaPointer.cs：Vulkan原生宿主类
│   │       ├── VulkanNativeHost.Bridge.cs：Vulkan原生宿主类
│   │       ├── VulkanNativeHost.CameraPointer.cs：Vulkan原生宿主类
│   │       ├── VulkanNativeHost.Dpi.cs：Vulkan原生宿主类
│   │       ├── VulkanNativeHost.Gizmo.cs：Vulkan原生宿主类
│   │       ├── VulkanNativeHost.LayoutSync.cs：修复 引入的 DPI 错配。 把 Avalonia 逻辑尺寸（Bounds）直接当作 Wi
│   │       ├── VulkanNativeHost.Log.cs：Vulkan原生宿主类
│   │       ├── VulkanNativeHost.NavGizmo.cs：导航 Gizmo 命中——原生指针消息流（Avalonia 覆盖层被原生子窗口遮挡，命中走
│   │       ├── VulkanNativeHost.Picking.cs：Vulkan原生宿主类
│   │       ├── VulkanNativeHost.Pointer.cs：Vulkan原生宿主类
│   │       ├── VulkanNativeHost.cs：Vulkan原生宿主原生Control宿主类
│   │       ├── VulkanViewport.axaml：界面布局
│   │       ├── VulkanViewport.axaml.cs：Vulkan视口UserControl类
│   │       ├── Win32ViewportHost.Input.cs：窗口32视口宿主Input（界面与组合）
│   │       └── Win32ViewportHost.cs：窗口32视口宿主（界面与组合）
│   ├── ViewportNativeHostRoute.cs：视口原生宿主Route静态类
│   ├── Vm
│   │   ├── CameraSessionMode.cs：相机会话Mode枚举
│   │   ├── CameraSessionSnapshot.cs：publicsealedrecord相机会话快照记录
│   │   ├── D2StaticModelDemo.cs：D2StaticModelDemo（界面与组合）
│   │   ├── DebugText.cs：Debug文本静态类
│   │   ├── EditorDisplayText.cs：编辑器Display文本静态类
│   │   ├── EditorLogCategory.cs：编辑器日志Category枚举
│   │   ├── EditorLogLevel.cs：编辑器日志级别枚举
│   │   ├── EditorLogSource.cs：编辑器日志来源枚举
│   │   ├── EditorTreeNode.cs：编辑器树节点INotifyPropertyChanged类
│   │   ├── LogEntry.cs：publicsealedrecord日志条目记录
│   │   ├── Logging
│   │   │   ├── EditorLogBuffer.cs：编辑器日志缓冲类
│   │   │   ├── EditorLogBus.cs：编辑器日志Bus类
│   │   │   ├── EditorLogClipboardText.cs：编辑器日志Clipboard文本静态类
│   │   │   ├── EditorLogFilter.cs：编辑器日志过滤枚举
│   │   │   ├── EditorLogFilterQuery.cs：编辑器日志过滤Query静态类
│   │   │   ├── EditorLogNoiseFilter.cs：编辑器日志Noise过滤静态类
│   │   │   ├── EditorLogRepeatKey.cs：编辑器日志Repeat键记录
│   │   │   └── EditorLogSummary.cs：publicsealedrecord编辑器日志Summary记录
│   │   ├── MapRenderSnapshotProjection.cs：MapDefinition → MapRenderSnapshot 纯投影（渲染唯一输入）
│   │   ├── SampleLogEntries.cs：Sample日志条目静态类
│   │   ├── SceneHistoryEntry.cs：场景历史条目（界面与组合）
│   │   ├── SceneRenderProjectionAdapter.cs：场景渲染投影适配器静态类
│   │   ├── StandardViewResolver.cs：六方向标准视角解析（计划 8.1 命名：+X 视图/-X 视图/+Y 视图/-Y 视图/顶
│   │   ├── StaticModelRenderAdapter.cs：StaticModel渲染适配器静态类
│   │   ├── TreeGuideBuilder.cs：树引导构建器静态类
│   │   ├── UiText.cs：Ui文本静态类
│   │   ├── UiVm.Camera.Framing.cs：取景命令。正交模式保持正交（尺度按包围范围适配），透视模式沿用距离构图
│   │   ├── UiVm.Camera.cs：导航 Gizmo 相机快照（Right/Up/Forward 投影输入；不含平移）
│   │   ├── UiVm.CameraNavigation.cs：UiVm类
│   │   ├── UiVm.DocumentStatus.cs：UiVm类
│   │   ├── UiVm.EntityCommands.cs：UiVm类
│   │   ├── UiVm.History.Entities.cs：UiVm类
│   │   ├── UiVm.History.cs：UiVm类
│   │   ├── UiVm.InputGuards.cs：UiVm类
│   │   ├── UiVm.Inspector.cs：UiVm类
│   │   ├── UiVm.InspectorInput.Parse.cs：UiVm类
│   │   ├── UiVm.InspectorInput.cs：UiVm类
│   │   ├── UiVm.Interaction.cs：UiVm类
│   │   ├── UiVm.InteractionCancel.cs：UiVm类
│   │   ├── UiVm.InteractionPointer.cs：UiVm类
│   │   ├── UiVm.Logging.cs：UiVm类
│   │   ├── UiVm.MapCommandRouting.cs：地图面板命令真实路由（RunCommand → 地图命令，兜底前返回）
│   │   ├── UiVm.MapDiagnostics.cs：地图命令低频诊断日志（命令/提交/撤销/重做节点）
│   │   ├── UiVm.MapEditor.cs：地图属性入口（唯一数据源 = MapSession；保存/打开按钮禁用防 v1 双权威，
│   │   ├── UiVm.MapHistory.cs：入口补接：地图撤销/重做（独立历史实例，不触碰场景实体历史）。 全局 Ctrl+Z 的"焦
│   │   ├── UiVm.MapRender.cs：MapSession → 渲染快照 适配（唯一渲染输入）。 首次组装生成初始快照；后续只响
│   │   ├── UiVm.MapWorld.cs：World 地图查询状态持有者（高度查询/边界判断权威，由会话 ContentChange
│   │   ├── UiVm.MoveGizmo.cs：UiVm类
│   │   ├── UiVm.MoveGizmoLogging.cs：UiVm类
│   │   ├── UiVm.MoveGizmoScreenSize.cs：UiVm类
│   │   ├── UiVm.NativeHostLifecycle.cs：UiVm类
│   │   ├── UiVm.Picking.cs：UiVm类
│   │   ├── UiVm.RenderProjection.cs：UiVm类
│   │   ├── UiVm.RotateGizmo.cs：UiVm类
│   │   ├── UiVm.ScaleGizmo.cs：UiVm类
│   │   ├── UiVm.Scene.cs：UiVm类
│   │   ├── UiVm.SceneDocument.New.cs：新建场景（5+100 拆分自 UiVm.SceneDocument.cs）
│   │   ├── UiVm.SceneDocument.cs：UiVm类
│   │   ├── UiVm.SceneDocumentLog.cs：UiVm类
│   │   ├── UiVm.SceneDocumentMapRef.cs：场景与地图引用的双向闭环。 保存场景时附加当前地图引用（mapId + 相对场景目录路径）
│   │   ├── UiVm.SceneDocumentSave.cs：UiVm类
│   │   ├── UiVm.Selection.cs：UiVm类
│   │   ├── UiVm.SelectionProjection.cs：UiVm类
│   │   ├── UiVm.SelectionTrace.cs：UiVm类
│   │   ├── UiVm.SelectionValidity.cs：UiVm类
│   │   ├── UiVm.StaticModelImport.cs：UiVm类
│   │   ├── UiVm.Tool.cs：UiVm类
│   │   ├── UiVm.TreeCommands.cs：UiVm类
│   │   ├── UiVm.ViewGizmo.cs：六方向标准视角命令（计划 8.1 命名；复用现有 ApplyViewFaceCommand
│   │   ├── UiVm.ViewportAssist.cs：UiVm类
│   │   ├── UiVm.ViewportSelection.cs：UiVm类
│   │   ├── UiVm.WorldProjection.cs：UiVm类
│   │   ├── UiVm.cs：UiVmINotifyPropertyChangedXuanYuCore场景I场景渲染快照来源类
│   │   └── ViewportPickingLogFormatter.cs：视口拾取日志Formatter静态类
│   ├── Win
│   │   ├── UiWin.Dialogs.cs：UiWin 错误/警告弹窗实现。复用 UiWin.UnsavedDialog 的窗口构建风
│   │   ├── UiWin.EntityShortcuts.cs：Ui窗口类
│   │   ├── UiWin.MapCommands.cs：地图命令分发（唯一数据源 = MapSession）。 打开/保存为 v1 DTO 旧链，
│   │   ├── UiWin.SceneCommands.cs：Ui窗口类
│   │   ├── UiWin.UnsavedDialog.cs：Ui窗口类
│   │   ├── UiWin.axaml：界面布局
│   │   └── UiWin.axaml.cs：Ui窗口窗口类
│   ├── XuanYu.Editor.UI.csproj：项目文件
│   └── app.manifest：app.manifest：仓库文件
├── XuanYu.Editor.Win
│   ├── MainForm.cs：主Form（Windows 平台宿主）
│   └── XuanYu.Editor.Win.csproj：项目文件
├── XuanYu.Engine.slnx：解决方案文件
├── XuanYu.Render.Abstractions
│   ├── EditorViewPlaneGridKind.cs：正交标准视图的视图平面网格类型。None=不显示； YZ=±X 视图（YZ 平面）、XZ=
│   ├── EditorViewportAssistState.cs：导航 Gizmo 悬停索引（-1=无；0..5=六个端点）——UI 指针流更新，Overl
│   ├── FrameExecutionPolicy.cs：Vulkan Present 循环帧执行顺序策略，供 VulkanPresentLoop
│   ├── INativeHostSurfaceBridge.cs：NativeHost 生命周期到 Surface 生命周期的交接契约。 由组合根（Edit
│   ├── INativeHostSurfaceBridgeFactory.cs：NativeHost 渲染桥的最小装配契约。 UI 后续只接收该工厂，不直接认识具体 Vu
│   ├── IRenderProjectionSource.cs：I渲染投影来源接口
│   ├── MapBoundsGeometry.cs：地图边界几何——四条边各一条细条四边形（每边 6 顶点 = 2 三角形）， 共 24 顶点
│   ├── MapRenderSnapshot.cs：地图渲染快照（唯一渲染输入；渲染层/Vulkan 只读，禁止反向访问编辑会话）。 由 Ed
│   ├── MapSurfaceGeometry.cs：有限 Flat 地面常量几何——固定 4 顶点 / 6 索引（两个三角形）， 地图尺寸只进
│   ├── MapSurfaceResourceKey.cs：收口：GPU 地图资源判等键。 只包含会改变地面/边界缓冲内容的字段；SourceChan
│   ├── MapSurfaceResourceUpdatePolicy.cs：收口：地图 GPU 资源更新决策（纯策略，不依赖 Vulkan，可独立测试）。 职责分离：
│   ├── NativeHostHandleSnapshot.cs：从 XuanYu.Render.Vulkan 迁入的纯生命周期快照。 不含任何 Vulka
│   ├── NativeHostLifecycleLogFormatter.cs：从 XuanYu.Render.Vulkan 迁入的纯生命周期日志格式器。 仅生成中文生命
│   ├── NativeHostLifecycleProbe.cs：从 XuanYu.Render.Vulkan 迁入的纯生命周期探针。 仅负责按生命周期阶段
│   ├── NativeHostLifecycleState.cs：从 XuanYu.Render.Vulkan 迁入的纯生命周期状态枚举。 不含任何 Vul
│   ├── NativeHostSurfaceHandle.cs：NativeHost 交给渲染层的窗口交接句柄。 只携带创建 Win32 Surface
│   ├── ReferenceGridScale.cs：每帧统一参考网格尺度（1/2/5 十进制序列 + 互补交叉淡化）。 同一帧所有 Fragm
│   ├── RenderCameraProjection.cs：渲染相机投影记录
│   ├── RenderDrawPlan.cs：实体绘制计划提取，供 Vulkan 与测试共同使用
│   ├── RenderEntityProjection.cs：渲染实体投影记录
│   ├── RenderEntityType.cs：渲染实体Type枚举
│   ├── RenderProjection.cs：渲染投影记录
│   ├── RenderProjectionResult.cs：渲染投影结果记录
│   ├── RenderStaticModelKey.cs：渲染StaticModel键记录
│   ├── RenderStaticModelPrimitive.cs：渲染StaticModelPrimitive记录
│   ├── RenderStaticModelResource.cs：publicsealedrecord渲染StaticModel资源记录
│   ├── RenderStaticModelVertex.cs：渲染StaticModel顶点记录
│   └── XuanYu.Render.Abstractions.csproj：项目文件
├── XuanYu.Render.Vulkan
│   ├── Bridge
│   │   ├── VulkanBridgeDeviceAttachStep.cs：在 VK4-A 物理设备选择成功后，基于其选择结果创建 LogicalDevice（VkD
│   │   ├── VulkanBridgePhysicalDeviceAttachStep.cs：将 Attach 后的 PhysicalDevice 选择与中文日志从 VulkanNat
│   │   ├── VulkanBridgeRenderSessionAttachStep.cs：把 RenderSession 创建从 Bridge 抽离，Bridge 只委托，不内联
│   │   └── VulkanBridgeSwapchainAttachStep.cs：在设备 step 之后链式驱动 Swapchain 创建（Swapchain + Imag
│   ├── Device
│   │   ├── VulkanDeviceOwner.Physical.cs：publicsealedunsafepartialclassVulkan设备持有者类
│   │   ├── VulkanDeviceOwner.cs：LogicalDevice 持有者。基于 VK4-A 的 VulkanPhysicalDe
│   │   ├── VulkanPhysicalDeviceInfo.cs：纯数据物理设备信息。仅描述候选设备，不持有任何 Vulkan 句柄（VkPhysicalD
│   │   ├── VulkanPhysicalDeviceSelection.cs：物理设备选择结果（纯数据，渲染层）。Success 为 true 时 Handle / D
│   │   ├── VulkanPhysicalDeviceSelector.cs：物理设备选择器。在已有 Instance + Surface 前提下枚举并选择可用于渲染/
│   │   └── VulkanQueueFamilySelection.cs：纯数据队列族选择结果。索引为 -1 表示未找到对应能力。 仅承载 Graphics / P
│   ├── Diagnostic
│   │   └── VulkanResizeTracer.cs：Resize / Present 慢半拍全链路诊断追踪器。 每次 Resize 或自愈生成
│   ├── Pipeline
│   │   ├── ShaderBytecode.Frag.cs：着色器字节码Frag（Vulkan 渲染实现）
│   │   ├── ShaderBytecode.GridFrag.cs：着色器字节码网格Frag（Vulkan 渲染实现）
│   │   ├── ShaderBytecode.GridVert.cs：着色器字节码网格Vert（Vulkan 渲染实现）
│   │   ├── ShaderBytecode.NavGizmoFrag.cs：着色器字节码导航GizmoFrag（Vulkan 渲染实现）
│   │   ├── ShaderBytecode.NavGizmoVert.cs：着色器字节码导航GizmoVert（Vulkan 渲染实现）
│   │   ├── ShaderBytecode.Vert.cs：着色器字节码Vert（Vulkan 渲染实现）
│   │   ├── ShaderBytecode.ViewPlaneGridFrag.cs：着色器字节码视图平面网格Frag（Vulkan 渲染实现）
│   │   ├── ShaderBytecode.WorldAxesFrag.cs：着色器字节码世界轴Frag（Vulkan 渲染实现）
│   │   ├── ShaderBytecode.WorldOriginFrag.cs：着色器字节码世界原点Frag（Vulkan 渲染实现）
│   │   ├── VulkanGraphicsPipelineOwner.Depth.cs：Vulkan图形管线持有者深度（Vulkan 渲染实现）
│   │   ├── VulkanGraphicsPipelineOwner.Fullscreen.cs：Vulkan图形管线持有者Fullscreen（Vulkan 渲染实现）
│   │   ├── VulkanGraphicsPipelineOwner.Grid.cs：Vulkan图形管线持有者网格（Vulkan 渲染实现）
│   │   ├── VulkanGraphicsPipelineOwner.Sky.cs：Vulkan图形管线持有者天空（Vulkan 渲染实现）
│   │   ├── VulkanGraphicsPipelineOwner.StaticModelInput.cs：Vulkan图形管线持有者StaticModelInput（Vulkan 渲染实现）
│   │   ├── VulkanGraphicsPipelineOwner.cs：Vulkan图形管线持有者（Vulkan 渲染实现）
│   │   ├── VulkanPipelineLogFormatter.cs：Vulkan管线日志Formatter（Vulkan 渲染实现）
│   │   ├── VulkanScenePushConstants.cs：Vulkan场景推Constants（Vulkan 渲染实现）
│   │   └── VulkanShaderModuleOwner.cs：Vulkan着色器Module持有者（Vulkan 渲染实现）
│   ├── Render
│   │   ├── StaticModels
│   │   │   ├── VulkanStaticModelBuffer.cs：VulkanStaticModel缓冲（Vulkan 渲染实现）
│   │   │   ├── VulkanStaticModelCache.cs：VulkanStaticModelCache（Vulkan 渲染实现）
│   │   │   ├── VulkanStaticModelFailureTracker.cs：VulkanStaticModel失败Tracker（Vulkan 渲染实现）
│   │   │   ├── VulkanStaticModelLog.cs：VulkanStaticModel日志（Vulkan 渲染实现）
│   │   │   ├── VulkanStaticModelResource.cs：VulkanStaticModel资源（Vulkan 渲染实现）
│   │   │   ├── VulkanStaticModelValidator.cs：VulkanStaticModel校验（Vulkan 渲染实现）
│   │   │   └── VulkanStaticModelVertex.cs：VulkanStaticModel顶点（Vulkan 渲染实现）
│   │   ├── VulkanClearFrameLogFormatter.cs：单色清屏日志格式化（统一经 Bridge 的 Emit 单出口）
│   │   ├── VulkanClearFrameOwner.Commands.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.Draw.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.DrawAssist.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.DrawGizmo.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.DrawStaticBounds.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.DrawStaticModel.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.Grid.cs：参考网格绘制。 PushConstant 192B（48 float）： mat4 vie
│   │   ├── VulkanClearFrameOwner.GridScale.cs：参考网格每帧全局尺度计算（视口中心射线与 Z=0 求交）。 求交失败回退：中心 → 视口偏
│   │   ├── VulkanClearFrameOwner.Lifecycle.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.MapSurface.cs：有限 Flat 地面（4 顶点 6 索引）+ 四条边界（24 顶点细条）。 A1：资源判等
│   │   ├── VulkanClearFrameOwner.Matrix.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.NavGizmo.cs：导航 Gizmo Overlay Pass —— 屏幕空间、深度测试/写入关闭、最后绘制。
│   │   ├── VulkanClearFrameOwner.PipelineBind.cs：全屏 Pass 管线绑定分发（网格/轴/原点/导航 Gizmo/视图平面网格/天空）
│   │   ├── VulkanClearFrameOwner.PushConstants.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.Resources.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.Scene.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.Trace.cs：publicsealedunsafepartialclassVulkan清屏取景持有者类
│   │   ├── VulkanClearFrameOwner.ViewPlaneGrid.cs：正交标准视图的视图平面网格绘制（±X→YZ / ±Y→XZ，以世界原点为基准）。 Push
│   │   ├── VulkanClearFrameOwner.WorldAxes.cs：世界轴 / 世界原点独立全屏 Pass。 单一轴线事实源：网格 Pass 不再画轴；本 P
│   │   ├── VulkanClearFrameOwner.cs：publicsealedunsafepartialclassVulkan清屏取景持有者IDisposable类
│   │   ├── VulkanDepthAttachment.cs：Vulkan深度附件（Vulkan 渲染实现）
│   │   ├── VulkanPresentLoop.Frame.cs：publicsealedunsafepartialclassVulkan呈现Loop类
│   │   ├── VulkanPresentLoop.Lifecycle.cs：publicsealedunsafepartialclassVulkan呈现Loop类
│   │   └── VulkanPresentLoop.cs：Present 泵必须确认停止成功后，才允许释放同步对象
│   ├── Session
│   │   ├── GridPipelineSet.cs：网格管线Set（Vulkan 渲染实现）
│   │   ├── VulkanRenderSession.Lifecycle.cs：Vulkan渲染会话类
│   │   ├── VulkanRenderSession.Recover.cs：Vulkan渲染会话类
│   │   ├── VulkanRenderSession.Resize.cs：Vulkan渲染会话类
│   │   └── VulkanRenderSession.cs：组合根负责失败回滚，不把半初始化资源留给 Bridge
│   ├── Shaders
│   │   ├── editor_nav_gizmo.frag：着色器源码（片元）
│   │   ├── editor_nav_gizmo.vert：着色器源码（顶点）
│   │   ├── editor_reference_grid.frag：着色器源码（片元）
│   │   ├── editor_reference_grid.vert：着色器源码（顶点）
│   │   ├── editor_view_plane_grid.frag：着色器源码（片元）
│   │   ├── editor_world_axes.frag：着色器源码（片元）
│   │   ├── editor_world_origin.frag：着色器源码（片元）
│   │   ├── scene.frag：着色器源码（片元）
│   │   └── scene.vert：着色器源码（顶点）
│   ├── Swapchain
│   │   ├── VulkanSwapchainBuilder.cs：Swapchain 构建细节（创建 Swapchain + 取 Images + 建 Im
│   │   ├── VulkanSwapchainCapabilities.cs：Swapchain 能力查询（纯数据，不创建 Swapchain）。 复用 VK4-A 已
│   │   ├── VulkanSwapchainLogFormatter.cs：Swapchain 中文生命周期日志格式器。纯文本，无副作用
│   │   ├── VulkanSwapchainOwner.Accessors.cs：publicsealedunsafepartialclassVulkan交换链持有者类
│   │   └── VulkanSwapchainOwner.cs：Swapchain 持有者（创建/重建/释放）。RZ-VK5-D-Recreate 内部加
│   ├── VulkanApiProbe.cs：publicstaticunsafeclassVulkanApiProbe类
│   ├── VulkanBridgeLogFormatter.cs：VK3-C1/C2-NativeHost → Instance+Surface 桥接中文生
│   ├── VulkanDeviceInfo.cs：publicsealedrecordVulkan设备Info记录
│   ├── VulkanInstanceCreateInfoBuilder.cs：Instance 创建信息构造辅助。仅构造 InstanceCreateInfo（含最小扩
│   ├── VulkanInstanceExtensions.cs：Instance 启用的最小扩展名集合（仅 surface 相关，以 null 结尾字节序
│   ├── VulkanInstanceLogFormatter.cs：Vulkan Instance 生命周期中文日志格式器。纯文本生成，无副作用
│   ├── VulkanInstanceOwner.cs：/ C1-Vulkan Instance 持有者。仅创建/释放 Instance，启用 V
│   ├── VulkanInstanceResult.cs：Vulkan Instance 创建结果。Owner 非空表示创建成功。 仅携带创建结果元
│   ├── VulkanNativeHostSurfaceBridge.Attach.cs：Vulkan原生宿主表面桥接类
│   ├── VulkanNativeHostSurfaceBridge.Lifecycle.cs：Vulkan原生宿主表面桥接类
│   ├── VulkanNativeHostSurfaceBridge.Resize.cs：Vulkan原生宿主表面桥接类
│   ├── VulkanNativeHostSurfaceBridge.Scene.cs：Vulkan原生宿主表面桥接类
│   ├── VulkanNativeHostSurfaceBridge.cs：Attach 全成功后才写入字段；失败按现有释放顺序回滚
│   ├── VulkanNativeHostSurfaceBridgeFactory.cs：Vulkan 侧开始适配抽象装配契约
│   ├── VulkanProbeLogFormatter.cs：VulkanProbe日志Formatter静态类
│   ├── VulkanProbeResult.cs：publicsealedrecordVulkanProbe结果记录
│   ├── VulkanSurfaceLogFormatter.cs：Vulkan Surface 生命周期中文日志格式器。纯文本生成，无副作用
│   ├── VulkanSurfaceOwner.cs：/ C1-Vulkan Surface 持有者。仅创建/释放 VkSurfaceKHR（W
│   ├── VulkanSurfaceResult.cs：Vulkan Surface 创建结果。Owner 非空表示创建成功。 仅携带创建结果元数
│   └── XuanYu.Render.Vulkan.csproj：项目文件
├── XuanYu.WarCore
│   ├── Identity
│   │   ├── FactionId.cs：/ <summary> / 阵营编号：0 表示默认未命名阵营，正数表示已分配阵营。 / <
│   │   ├── MilitaryIdentity.cs：/ <summary> / 军事身份：单位编号、显示名称与单位类型。 / 构造时校验编号有
│   │   ├── OrganizationId.cs：/ <summary> / 组织编号：0 表示默认未编组，正数表示已分配编制。 / </s
│   │   ├── UnitId.cs：/ <summary> / 单位编号：WarCore 领域的军事编号（如 S-0001），
│   │   └── UnitKind.cs：/ <summary> / 单位类型： 仅支持士兵，不为未来兵种预造继承体系。 / </s
│   ├── State
│   │   └── SoldierState.cs：/ <summary> / 士兵状态：身体状态、体力、士气、压制，统一 0–100 范围。
│   └── XuanYu.WarCore.csproj：项目文件
├── XuanYu.WarCore.Tests
│   ├── Identity
│   │   └── MilitaryIdentityTests.cs：身份生成与校验契约测试
│   ├── State
│   │   └── SoldierStateTests.cs：士兵状态边界与隔离契约测试
│   ├── WarCoreDependencyTests.cs：WarCore 程序集依赖方向契约测试。 编译期引用由 arch-a-guard-warc
│   └── XuanYu.WarCore.Tests.csproj：项目文件
├── XuanYu.World
│   ├── EntityRegistry.Authoring.cs：实体注册表类
│   ├── EntityRegistry.Replace.cs：实体注册表类
│   ├── EntityRegistry.cs：实体注册表类
│   ├── GlobalWorld.Authoring.cs：Global世界类
│   ├── GlobalWorld.Query.cs：Global世界类
│   ├── GlobalWorld.Snapshot.cs：Global世界类
│   ├── GlobalWorld.cs：Global世界类
│   ├── GridWorldPartitionStrategy.cs：网格世界分区策略I世界分区策略类
│   ├── IWorldPartitionStrategy.cs：I世界分区策略接口
│   ├── Map
│   │   ├── MapBounds.cs：有限地图边界（米）。地图中心为世界原点，范围 X/Y ∈ [-W/2, W/2]。 与 W
│   │   ├── MapDefaultDefinition.cs：默认地图工厂。一次性创建完整地图聚合： 10 km × 10 km Flat 地表 + 基
│   │   ├── MapDefinition.cs：完整地图领域聚合（权威根）。只描述地图内容（纯净、不可变）， 不承担编辑会话版本/Undo
│   │   ├── MapDefinitionValidator.cs：地图聚合严格校验（领域权威层）。 覆盖：MapId/名称、尺寸范围、坐标系统（meter+
│   │   ├── MapGeometry.cs：地图尺寸（米）。width 对应世界 X，depth 对应世界 Y，Z-Up 下高度沿 Z
│   │   ├── MapId.cs：地图稳定唯一标识（领域权威层）。 合同冻结格式：32 位十六进制，无前缀
│   │   ├── MapLayer.cs：图层领域模型（领域权威层）。用于组织地图元素，不承担渲染管线功能。 Kind 标识图层角色
│   │   ├── MapLayerId.cs：图层稳定唯一标识（领域权威层）。与 MapId 同族格式（32 位十六进制，无前缀）。 名
│   │   ├── MapLayerKind.cs：图层角色（稳定标识，不依赖中文名称识别）。 Base=基础地图层（唯一、Order 必须
│   │   ├── MapLayerValidator.cs：图层集合严格校验（领域权威层）。 检查：ID 合法且唯一、名称非空、顺序非负且唯一、基础层
│   │   ├── MapRegion.cs：区域领域模型（领域权威层）。地图上的二维闭合多边形（水平面坐标）。 正式区域天然闭合：顶点
│   │   ├── MapRegionDraft.cs：绘制中的区域草稿（未闭合顶点序列）。 绘制流程使用； 一旦提交（Close）即成为正式 M
│   │   ├── MapRegionId.cs：区域稳定唯一标识（领域权威层）。与 MapId 同族格式（32 位十六进制，无前缀）。 名
│   │   ├── MapRegionKind.cs：区域类型（领域权威层）。 仅承载几何与基础元数据，不解释战斗含义。 Generic=普通区
│   │   ├── MapRegionValidator.cs：区域集合严格校验（领域权威层）。 检查：ID 合法且唯一、引用图层存在且可承载区域（非 B
│   │   ├── MapSurfaceDefinition.cs：地表定义。支持 Flat 与 GentleHillsV1（确定性参数化起伏）
│   │   ├── MapValidationResult.cs：地图领域验证结构化结果（不抛出来源不明的异常）
│   │   ├── WorldMapState.cs：World 地图状态（纯数据 + 有限边界 + 高度查询 + 环境参数）。 世界坐标语义：
│   │   └── WorldMapStateOwner.cs：当前 World 地图状态所有者。加载/切换/卸载，暴露高度查询与渲染快照
│   ├── RegionKey.cs：区域键记录
│   ├── Scene
│   │   ├── SceneSpatialBoundsProjection.cs：场景空间边界投影静态类
│   │   ├── SceneStateOwner.Lifecycle.cs：场景状态持有者类
│   │   ├── SceneStateOwner.Seeding.cs：场景状态持有者类
│   │   ├── SceneStateOwner.StaticModel.cs：场景状态持有者类
│   │   ├── SceneStateOwner.Transform.cs：场景状态持有者类
│   │   ├── SceneStateOwner.cs：场景状态持有者I场景渲染快照来源类
│   │   └── SceneWorldProjection.cs：场景世界投影静态类
│   ├── Spatial
│   │   ├── DynamicAabbTree.Insert.cs：DynamicAabb树类
│   │   ├── DynamicAabbTree.Node.cs：DynamicAabb树类
│   │   ├── DynamicAabbTree.Query.cs：DynamicAabb树类
│   │   ├── DynamicAabbTree.Refit.cs：DynamicAabb树类
│   │   ├── DynamicAabbTree.Remove.cs：DynamicAabb树类
│   │   ├── DynamicAabbTree.cs：DynamicAabb树I空间索引类
│   │   ├── ISpatialIndex.cs：I空间索引接口
│   │   ├── SpatialIndexOwner.cs：空间索引持有者类
│   │   └── SpatialRaycastResolver.cs：空间Raycast解析器类
│   ├── WorldEntityActivity.cs：世界实体Activity枚举
│   ├── WorldEntityName.cs：世界实体Name静态类
│   ├── WorldEntitySnapshot.cs：世界实体快照记录
│   ├── WorldEntityType.cs：世界实体Type枚举
│   ├── WorldPartitionEntry.cs：世界分区条目记录
│   ├── WorldPartitionMembership.cs：世界分区Membership类
│   ├── WorldQuery.cs：世界Query类
│   └── XuanYu.World.csproj：项目文件
├── XuanYu.World.Tests
│   ├── Assets
│   │   ├── HostingTestEnv.cs：Hosting测试Env（World/Editor 领域测试）
│   │   ├── ScenePersistenceEnv.cs：测试辅助：独立临时目录 + 保存/加载事务 + Fake Dialog 计数
│   │   ├── WorldCR4D4DialogTests.cs：世界CR4D4Dialog测试IDisposable类
│   │   ├── WorldCR4D4HostingCompleteTests.cs：世界CR4D4HostingComplete测试IDisposable类
│   │   ├── WorldCR4D4HostingPlannerRejectTests.cs：世界CR4D4HostingPlanner拒绝测试IDisposable类
│   │   ├── WorldCR4D4HostingPlannerTests.cs：世界CR4D4HostingPlanner测试IDisposable类
│   │   ├── WorldCR4D4HostingRollbackTests.cs：世界CR4D4Hosting回滚测试IDisposable类
│   │   ├── WorldCR4D4HostingSaveAsTests.cs：世界CR4D4Hosting保存As测试IDisposable类
│   │   ├── WorldCR4D4HostingTransactionTests.cs：世界CR4D4Hosting事务测试IDisposable类
│   │   ├── WorldCR4D4LoadStructureErrorTests.cs：结构错误事务（拆分自 LoadTransactionTests，5+100）。 非法 JS
│   │   ├── WorldCR4D4LoadTransactionTests.cs：世界CR4D4加载事务测试IDisposable类
│   │   ├── WorldCR4D4SaveAsTests.cs：另存为与重复保存（拆分自 SaveTransactionTests，5+100）
│   │   ├── WorldCR4D4SaveTransactionTests.cs：世界CR4D4保存事务测试IDisposable类
│   │   └── WorldCR4D4SchemaCompatibilityTests.cs：世界CR4D4结构Compatibility测试IDisposable类
│   ├── Map
│   │   ├── MapBoundsTests.cs：有限地图边界合同（中心原点、闭区间、尺寸变化同步）
│   │   ├── MapCoordinateValidationTests.cs：坐标合同 / 图层引用 / schema / 名称校验
│   │   ├── MapDefaultMapTests.cs：默认地图工厂合同（完整聚合 + DTO 默认值一致）
│   │   ├── MapDefinitionTests.cs：地图聚合验证（尺寸/坐标/地表/图层/区域组合入口）
│   │   ├── MapDocumentAggregateBridgeTests.cs：.xymap v1 DTO → 领域聚合桥接（场景 mapReference 保活链）与端
│   │   ├── MapDocumentOwnerChainTests.cs：状态链闭环与失败不污染
│   │   ├── MapDocumentOwnerTests.cs：当前地图状态所有者（New/Load/Modify/Save/Unload 基础状态）
│   │   ├── MapEnvironmentValidationTests.cs：环境定义与参数校验
│   │   ├── MapIdTests.cs：MapId 与地图合同校验（纯内存）
│   │   ├── MapJsonRoundTripTests.cs：.xymap 严格 JSON Round-trip 与确定性
│   │   ├── MapJsonStrictnessTests.cs：严格 JSON 拒绝路径（大小写 / 未知字段 / 类型 / 损坏）
│   │   ├── MapLayerTests.Base.cs：基础层合同（必须且仅有一个、位于第 0 位、稳定角色标识）
│   │   ├── MapLayerTests.cs：图层领域模型与验证（默认图层/稳定 ID/唯一性）
│   │   ├── MapRegionDraftTests.cs：绘制草稿合同（未闭合草稿 → 提交为天然闭合正式区域）
│   │   ├── MapRegionTests.Helpers.cs：地图区域测试类
│   │   ├── MapRegionTests.Strictness.cs：区域严格性（相邻重复点/首尾规则/三不同顶点/非零面积）
│   │   ├── MapRegionTests.cs：区域验证（闭合/顶点数/引用图层/边界/有限数值）
│   │   ├── MapSizeValidationTests.cs：地图尺寸与坐标合同校验
│   │   ├── MapStorageFailureTests.cs：加载失败保护 / 非法合同拒绝 / 保存失败不写坏文件
│   │   ├── MapStorageTests.cs：候选加载 / 原子保存（真实文件，临时目录）
│   │   ├── MapSurfaceSamplerTests.cs：唯一地表采样器——确定性、范围与参数语义
│   │   ├── MapSurfaceValidationTests.cs：地表定义与参数校验
│   │   ├── WorldMapStateOwnerTests.cs：World 地图状态所有者——加载/切换/卸载/查询/渲染快照
│   │   └── WorldMapStateTests.cs：World 地图状态——有限边界（闭区间）与高度查询
│   ├── MapEditing
│   │   ├── MapEditSessionCommandTests.cs：地图基础编辑命令（改名/尺寸/基础高度/No-op/非法输入）
│   │   ├── MapEditSessionCreationTests.cs：默认会话与根状态合同
│   │   ├── MapEditSessionDirtyTests.cs：Saved/Dirty 合同（Dirty 随 Undo/Redo 回到保存点）
│   │   ├── MapEditSessionHistoryTests.cs：Undo/Redo、分支清除与 ChangeSequence 单调递增
│   │   ├── MapEditSessionMapPropertiesTests.cs：地图属性原子提交（单历史节点/失败零污染）
│   │   ├── MapEditSessionSelectionTests.cs：选择状态（稳定 ID/存在性/不产生 Dirty/规范化）
│   │   ├── MapEditSessionThreadTests.cs：写线程保护（非法线程拒绝且状态完全不变）
│   │   ├── MapEditSessionValidationTests.cs：候选校验与失败不污染（缩小越界整体拒绝/无效替换拒绝）
│   │   └── MapRenderSnapshotProjectionTests.cs：MapDefinition → MapRenderSnapshot 投影合同（渲染唯一输入
│   ├── Spatial
│   │   ├── SceneStateOwnerSpatialTests.cs：场景状态持有者空间测试类
│   │   ├── SpatialIndexOwnerLifecycleTests.cs：空间索引持有者Lifecycle测试类
│   │   ├── SpatialIndexOwnerRevisionTests.cs：空间索引持有者修订号测试类
│   │   ├── SpatialIndexScaleTests.cs：空间索引尺度测试类
│   │   ├── SpatialRayQueryLifecycleTests.cs：空间RayQueryLifecycle测试类
│   │   ├── SpatialRayQueryTests.cs：空间RayQuery测试类
│   │   ├── SpatialRaycastNearestTests.cs：空间RaycastNearest测试类
│   │   ├── SpatialRaycastRevisionTests.cs：空间Raycast修订号测试类
│   │   ├── SpatialRaycastScaleTests.cs：空间Raycast尺度测试类
│   │   └── SpatialTestData.cs：空间测试数据（World/Editor 领域测试）
│   ├── Transform
│   │   └── TransformSessionTests.cs：变换会话测试类
│   ├── World
│   │   ├── EntityRegistryTests.cs：实体注册表测试类
│   │   ├── GlobalWorldTests.cs：Global世界测试类
│   │   ├── SceneMapReferenceTests.cs：MAP-A---B（ 适配）：.xyscene mapReference 闭环——保存携带
│   │   ├── UiMapEditorTests.cs：地图属性入口——会话恒有默认地图、应用修改、非法输入保护、取景数据源
│   │   ├── UiMapCommandRoutingTests.cs：真实按钮链测试（RunCommand.Execute → MapSession）
│   │   ├── UiMapHistoryTests.cs：入口补接：地图撤销/重做按钮路由到 MapSession 独立历史
│   │   ├── UiMapInitialProjectionTests.cs：默认地图初始快照进入首帧 RenderProjection（无需新建地图）
│   │   ├── UiViewGizmoTests.cs：视角 Gizmo 六方向相机命令——朝向正确、观察中心与距离保持
│   │   ├── WorldCR2CameraDocumentTests.cs：世界CR2相机文档测试类
│   │   ├── WorldCR2DocumentTests.cs：世界CR2文档测试类
│   │   ├── WorldCR2EntityTests.cs：世界CR2实体测试类
│   │   ├── WorldCR2InlineRenameTests.cs：世界CR2Ui历史测试类
│   │   ├── WorldCR2UiHistoryTests.cs：世界CR2Ui历史测试类
│   │   ├── WorldCR3R3CommandSmokeTests.cs：世界CR3R3命令Smoke测试类
│   │   ├── WorldCR3R4GlobalGizmoTests.cs：世界CR3R4GlobalGizmo测试类
│   │   ├── WorldCR3ViewportAssistTests.cs：世界CR3视口Assist测试类
│   │   ├── WorldCR4D0AssetContractTests.cs：世界CR4D0资产合同测试类
│   │   ├── WorldCR4D1GlbFactory.cs：世界CD1Glb工厂（World/Editor 领域测试）
│   │   ├── WorldCR4D1GlbImportTests.cs：世界CR4D1Glb导入测试类
│   │   ├── WorldCR4D3AuthoringServiceTests.cs：世界CR4D3Authoring服务测试类
│   │   ├── WorldCR4D3CatalogTests.cs：世界CR4D3目录测试类
│   │   ├── WorldCR4D3F1BaseVertexTests.cs：世界CR4D3F1基础顶点测试类
│   │   ├── WorldCR4D3F1FailureTrackerTests.cs：世界CR4D3F1失败Tracker测试类
│   │   ├── WorldCR4D3F1GlbFactory.cs：世界CD3F1Glb工厂（World/Editor 领域测试）
│   │   ├── WorldCR4D3F1ValidatorTests.cs：世界CR4D3F1校验测试类
│   │   ├── WorldCR4D3ProjectionTests.cs：世界CR4D3投影测试类
│   │   ├── WorldCR4D3StaticModelUiTests.cs：世界CR4D3StaticModelUi测试类
│   │   ├── WorldCSceneDocumentTests.R1R1.cs：世界C场景文档测试类
│   │   ├── WorldCSceneDocumentTests.R1SaveFeedback.cs：世界C场景文档测试类
│   │   ├── WorldCSceneDocumentTests.cs：世界C场景文档测试类
│   │   ├── WorldCameraFramingOccupancyTests.cs：地图取景屏幕占用率（65%~75%）
│   │   ├── WorldCameraFramingTests.cs：世界相机取景测试类
│   │   ├── WorldCameraNavigationUiTests.cs：世界相机导航Ui测试类
│   │   ├── WorldDR1EnvironmentTests.cs：编辑器环境（天空/光照）契约测试。 不触碰 GPU：只验证默认材质路径与场景文档边界
│   │   ├── WorldEntityBoundsSemanticsTests.cs：final patch: lock the two spatial-bounds sema
│   │   ├── WorldMoveTransformPlaneUiTests.cs：世界移动变换Ui测试类
│   │   ├── WorldMoveTransformRegionUiTests.cs：世界移动变换Ui测试类
│   │   ├── WorldMoveTransformSessionUiTests.cs：世界移动变换Ui测试类
│   │   ├── WorldMoveTransformUiTests.cs：世界移动变换Ui测试类
│   │   ├── WorldPartitionR1Tests.Activity.cs：世界分区R1测试类
│   │   ├── WorldPartitionR1Tests.cs：世界分区R1测试类
│   │   ├── WorldPartitionR2Tests.cs：世界分区R2测试类
│   │   ├── WorldPartitionTests.PartitionStrategy.cs：世界分区测试类
│   │   ├── WorldPartitionTests.cs：世界分区测试类
│   │   ├── WorldPartitionUiTests.cs：世界分区Ui测试类
│   │   ├── WorldR1FinalSceneTests.cs：世界R1Final场景测试类
│   │   ├── WorldR1FinalSelectionTests.cs：世界R1Final选择测试类
│   │   ├── WorldR4InspectorInputTests.cs：世界R4变换Foundation测试类
│   │   ├── WorldR4TransformFoundationTests.cs：世界R4变换Foundation测试类
│   │   ├── WorldR4TransformInputTests.cs：世界R4变换Foundation测试类
│   │   ├── WorldRotateTransformUiTests.R4R1.cs：世界旋转变换Ui测试类
│   │   ├── WorldRotateTransformUiTests.R4R2.Helpers.cs：世界旋转变换Ui测试类
│   │   ├── WorldRotateTransformUiTests.R4R2.cs：世界旋转变换Ui测试类
│   │   ├── WorldRotateTransformUiTests.R4R3R1.cs：世界旋转变换Ui测试类
│   │   ├── WorldRotateTransformUiTests.cs：世界旋转变换Ui测试类
│   │   ├── WorldScaleTransformUiTests.Helpers.cs：世界尺度变换Ui测试类
│   │   ├── WorldScaleTransformUiTests.History.cs：世界尺度变换Ui测试类
│   │   ├── WorldScaleTransformUiTests.Pointer.cs：世界尺度变换Ui测试类
│   │   ├── WorldScaleTransformUiTests.R5R1.cs：世界尺度变换Ui测试类
│   │   ├── WorldScaleTransformUiTests.Target.cs：世界尺度变换Ui测试类
│   │   ├── WorldScaleTransformUiTests.cs：Scale Gizmo 缩放变换闭环集成测试。复用既有 SelectionKey / Tr
│   │   ├── WorldSceneConsumptionTests.cs：世界场景Consumption测试类
│   │   ├── WorldSceneIsolationTests.cs：世界场景Isolation测试类
│   │   ├── WorldSceneMultiEntityGateTests.cs：世界场景Multi实体Gate测试类
│   │   ├── WorldSceneSelectionReentryTests.cs：世界场景选择Reentry测试类
│   │   ├── WorldSceneSingleAuthorityTests.cs：世界场景SingleAuthority测试类
│   │   ├── WorldSelectionToolStateUiTests.cs：世界选择工具状态Ui测试类
│   │   ├── WorldSpatialQueryGovernanceTests.cs：世界空间QueryGovernance测试类
│   │   ├── WorldSpatialQueryTests.Geometry.cs：世界空间Query测试类
│   │   ├── WorldSpatialQueryTests.cs：世界空间Query测试类
│   │   ├── WorldSpatialR1LifecycleTests.cs：世界空间R1Lifecycle测试类
│   │   ├── WorldSpatialR1Oracle.cs：世界空间Oracle（World/Editor 领域测试）
│   │   ├── WorldSpatialR1RebuildTests.cs：世界空间R1Rebuild测试类
│   │   ├── WorldToolStateHighlightUiTests.Selection.cs：世界工具状态HighlightUi测试类
│   │   ├── WorldToolStateHighlightUiTests.cs：世界工具状态HighlightUi测试类
│   │   ├── WorldUiHierarchyConnectorTests.cs：世界UiHierarchyConnector测试类
│   │   ├── WorldUiTreeGuideTests.cs：世界Ui树引导测试类
│   │   └── WorldUiTreeToggleTests.cs：世界Ui树Toggle测试类
│   └── XuanYu.World.Tests.csproj：项目文件
├── changelog.md：changelog
├── docs
│   ├── CODE_CONSTITUTION.md：XuanYu Engine 代码宪法
│   ├── architecture
│   │   ├── ENGINE_ARCHITECTURE.md：FluidWarfare 引擎架构
│   │   └── world-a-r0-coordinate-contract.md：坐标契约与方向轴审计
│   ├── archive
│   │   ├── changelog
│   │   │   ├── changelog-2026-05.md：changelog 归档：2026-05
│   │   │   ├── changelog-2026-06.md：changelog 归档：2026-06
│   │   │   └── changelog-2026-07.md：changelog 归档：2026-07
│   │   └── superseded
│   │       ├── AI_DEVELOPMENT_RULES.md：XuanYu Engine AI 开发规则
│   │       └── LEGACY_FLUIDWARFARE_OLD_AUDIT.md：FluidWarfare-old 旧仓库考古报告
│   ├── dev-rules.md：玄域引擎 · 开发硬规则（执行手册）
│   ├── docs-index.md：玄域引擎 docs 索引
│   ├── governance
│   │   ├── NAMING_RULES.md：XuanYu Engine 命名规则
│   │   ├── debts
│   │   │   └── arch-world-debts.md：受控债务登记
│   │   ├── dev-rules-understanding.md：玄域引擎 · 开发规范「为什么这样规定」（理解手册）
│   │   ├── diagnostic-safety.md：诊断日志与 UI 调度安全规范
│   │   ├── naming-XuanYu-Engine.md：玄域引擎命名规范
│   │   ├── shr-2026-08-closure.svg：治理示意图
│   │   └── 版本号规范与历史映射.md：版本号规范与历史编号映射
│   ├── milestones
│   │   ├── closed
│   │   │   ├── ARCH-A
│   │   │   │   └── arch-a-plan.md：ARCH-A-Plan：Editor.UI Vulkan 直接依赖边界审计与迁移计划
│   │   │   ├── ARCH-B
│   │   │   │   └── arch-b-plan.md：ARCH-B-Plan：编辑器状态所有权与交互事务边界
│   │   │   ├── ARCH-C
│   │   │   │   ├── arch-c-overview.svg：里程碑示意图
│   │   │   │   ├── arch-c-plan.md：ARCH-C-Plan：真实场景编辑交互闭环规划
│   │   │   │   ├── arch-c-r2-current-route.svg：里程碑示意图
│   │   │   │   ├── arch-c-r2-entry-audit.md：坐标与相机入口门审计
│   │   │   │   ├── arch-c-r2-spatial-query.svg：里程碑示意图
│   │   │   │   ├── arch-c-r2b-closure.svg：里程碑示意图
│   │   │   │   ├── arch-c-r2b-space-fact.svg：里程碑示意图
│   │   │   │   ├── arch-c-r2c-closure.svg：里程碑示意图
│   │   │   │   ├── arch-c-r2c-render-space.svg：里程碑示意图
│   │   │   │   ├── arch-c-r2d-spatial-index.svg：里程碑示意图
│   │   │   │   ├── arch-c-r2e-ray-hit.svg：里程碑示意图
│   │   │   │   ├── arch-c-r2f-pointer-picking.svg：里程碑示意图
│   │   │   │   ├── arch-c-r3-selection.svg：里程碑示意图
│   │   │   │   ├── arch-c-r3-timeout-fix.svg：里程碑示意图
│   │   │   │   ├── arch-c-r4-move-gizmo.svg：里程碑示意图
│   │   │   │   ├── arch-c-r4-r1-gizmo-hit.svg：里程碑示意图
│   │   │   │   ├── arch-c-r5-to-r8-route.svg：里程碑示意图
│   │   │   │   ├── arch-c-r5-transform-session.md：Transform Session 封版记录
│   │   │   │   ├── arch-c-r5-transform-session.svg：里程碑示意图
│   │   │   │   ├── arch-c-r7-log-copy-fix.svg：里程碑示意图
│   │   │   │   ├── arch-c-r7-undo.svg：里程碑示意图
│   │   │   │   ├── arch-c-r8-acceptance.md：综合真机验收与收口判断
│   │   │   │   ├── arch-c-r8-final-acceptance-report.md：最终真机验收报告
│   │   │   │   ├── arch-c-r8-final-acceptance-status.svg：里程碑示意图
│   │   │   │   ├── arch-c-r8-integration-acceptance.svg：里程碑示意图
│   │   │   │   ├── arch-c-r8-stage-acceptance-report.md：阶段性真机验收报告
│   │   │   │   └── arch-c-r8-stage-acceptance-status.svg：里程碑示意图
│   │   │   ├── ARCH-WORLD
│   │   │   │   ├── arch-world-layer-attribution.md：物理分层归属审计（修正版）
│   │   │   │   ├── arch-world-layer-attribution.svg：里程碑示意图
│   │   │   │   ├── arch-world-r1-acceptance.md：真机验收报告
│   │   │   │   ├── arch-world-r1-acceptance.svg：里程碑示意图
│   │   │   │   ├── arch-world-r2-g1-audit.md：只读审计：Move Gizmo HitTest 输入抢占
│   │   │   │   ├── arch-world-r2-manual-checklist.html：arch-world-r2-manual-checklist.html：仓库文件
│   │   │   │   ├── arch-world-r2-single-spatial-authority.md：单一空间权威收敛
│   │   │   │   ├── arch-world-r2-status.md：实施状态与真机验收
│   │   │   │   ├── arch-world-r3-scene-truth-audit.md：Scene Truth 现状审计
│   │   │   │   ├── arch-world-r4-editor-boundary.svg：里程碑示意图
│   │   │   │   ├── arch-world-r4-editor-pollution-audit.md：Editor 污染归属只读审计
│   │   │   │   ├── arch-world-r4-gate2-acceptance.md：Gate 2 真机验收清单（操作手册）
│   │   │   │   ├── arch-world-r5-final-closure.md：最终收口报告
│   │   │   │   ├── arch-world-r5-final-closure.svg：里程碑示意图
│   │   │   │   ├── arch-world-r5-r0a-render-contract-audit.md：-R0A：Render 合同边界只读审计
│   │   │   │   ├── arch-world-r5-r0a-render-contract.svg：里程碑示意图
│   │   │   │   ├── arch-world-r6-exit-gate.md：架构退出门禁
│   │   │   │   └── arch-world-r6-exit-gate.svg：里程碑示意图
│   │   │   ├── M1
│   │   │   │   ├── MILESTONE1_PUBLIC_VALIDATION.md：Milestone 1 公开验收记录
│   │   │   │   ├── PHASE1_SCOPE.md：FluidWarfare Phase 1 范围
│   │   │   │   ├── PROJECT_CHARTER.md：FluidWarfare 项目宪章
│   │   │   │   ├── audit-EditorShellV2-9.1A-1.md：9.1A-1 审计：EditorShellV2 最小骨架
│   │   │   │   ├── audit-EditorShellV2-freeze-9.1A-Freeze.md：9.1A-Freeze：EditorShellV2 冻结文档
│   │   │   │   ├── audit-EditorShellV2-input-9.1A-2.md：9.1A-2 审计：EditorShellV2 接入 Viewport 输入路由
│   │   │   │   ├── audit-EditorShellV2-input-9.1A-2R.md：9.1A-2R 审计：修正 EditorShellV2 输入状态机
│   │   │   │   ├── audit-EditorShellV2-picking-gizmo-9.1A-3.md：9.1A-3 审计：EditorShellV2 接入 Picking + MoveGizm
│   │   │   │   ├── audit-EditorShellV2-picking-gizmo-9.1A-3R.md：9.1A-3R 审计：补齐 MoveGizmo drag preview 与 Esc Ca
│   │   │   │   ├── audit-EditorShellV2-plan-9.1A-0.md：9.1A-0 审计：EditorShellV2 布局重建方案
│   │   │   │   ├── audit-NativeViewportMouseCapture-lifecycle-9.0X.md：9.0X 审计：Native Viewport 鼠标捕获生命周期
│   │   │   │   ├── audit-RZ-New-0-onboarding.md：审计：RZ-New-0 新人接手规则审计
│   │   │   │   ├── audit-RZ-VK1-vulkan-probe.md：Vulkan 依赖接入与环境探针审计
│   │   │   │   ├── audit-RZ-VK2-R1-nativehost-resize-coalesce.md：审计：RZ-VK2- NativeHost 尺寸变化日志合并
│   │   │   │   ├── audit-RZ-VK2-R2-nativehost-resize-coalesce-verify.md：audit-RZ-VK2--nativehost-resize-coalesce-veri
│   │   │   │   ├── audit-RZ-VK2-native-host-lifecycle.md：NativeHost / HWND 生命周期收口审计
│   │   │   │   ├── audit-gizmo-chain-9.0Y-1.md：9.0Y-1 审计：Gizmo 链路审计 — DragPlane 退化 / 状态机 / 可
│   │   │   │   ├── audit-gizmo-chain-9.0Y-2.md：9.0Y-2 审计：Gizmo 链路最小测试补强
│   │   │   │   ├── audit-gizmo-chain-9.0Y-3.md：9.0Y-3 审计：Gizmo 链路封版验证
│   │   │   │   ├── audit-gizmo-stash-9.0Y-0.md：9.0Y-0 审计：Gizmo 链路封版前的工作树与 stash 清点
│   │   │   │   ├── audit-input-lifecycle-9.0X-1.md：9.0X-1 审计：Native Viewport 输入生命周期 — 全量调用点与状态机分
│   │   │   │   ├── audit-input-lifecycle-9.0X-2.md：9.0X-2 审计：Esc 取消 MoveGizmo 后释放 Win32 Capture
│   │   │   │   ├── audit-input-lifecycle-9.0X-3.md：9.0X 输入生命周期封版验证报告
│   │   │   │   ├── audit-inspector-transform-9.0C-0.md：9.0C-0 审计：Inspector / Selection / WorldState
│   │   │   │   ├── editor-top-area-target-9.1B.md：顶部区域设计目标 — 9.1B
│   │   │   │   ├── editor-top-svg-icons-9.1C-R.md：9.1C-R：TopArea SVG 视觉细修
│   │   │   │   ├── editor-top-svg-icons-9.1C.md：顶部 SVG 图标化 — 9.1C
│   │   │   │   ├── editor-ui-terms-9.1B.md：UI 术语表 — 9.1B
│   │   │   │   ├── gizmo_drag_audit_2026-06-25.md：Gizmo 拖动 Preview 高频路径审计
│   │   │   │   ├── gizmo_drag_audit_probe.log：gizmo_drag_audit_probe.log：仓库文件
│   │   │   │   ├── plan-9.0D-move-gizmo-final.md：Milestone 9.0D — Move Gizmo 最终验收
│   │   │   │   ├── project-baseline-audit-org-1-r1.md：项目基线审计（已验收基线）
│   │   │   │   └── project-baseline-audit-org-1.md：项目真实基线审计（已退回，见 ORG-1-）
│   │   │   ├── RZ-VK
│   │   │   │   ├── log-ux-1-r2-autoscroll.svg：里程碑示意图
│   │   │   │   ├── log-ux-r8-tail-noise-fix.svg：里程碑示意图
│   │   │   │   ├── log-ux-window-copy-focus-fix.svg：里程碑示意图
│   │   │   │   ├── rz-vk3-closure.md：rz-vk3-closure.md
│   │   │   │   ├── rz-vk3-surface-lifecycle-plan.md：rz-vk3-surface-lifecycle-plan.md
│   │   │   │   ├── rz-vk4-c-r1-audit-plan.md：审计与运行验证计划（只审计不新增能力）
│   │   │   │   ├── rz-vk4-c-swapchain-plan.md：rz-vk4-c-swapchain-plan.md — VK4-C Swapchain
│   │   │   │   ├── rz-vk4-closure.md：rz-vk4-closure.md — VK4 阶段正式收口确认（PhysicalDevi
│   │   │   │   ├── rz-vk4-d-plan.md：rz-vk4-d-plan.md — VK4-D 最小清屏闭环规划（RenderPass
│   │   │   │   ├── rz-vk4-plan.md：rz-vk4-plan.md
│   │   │   │   ├── rz-vk5-a-plan.md：rz-vk5-a-plan.md — RZ-VK5-A 规划：ShaderModule +
│   │   │   │   ├── rz-vk5-c-plan.md：规划 · viewport/scissor 与 Resize 关系验证收口
│   │   │   │   ├── rz-vk5-e-plan.md：规划 · 清理 VulkanClearSession 死代码（债务 B）
│   │   │   │   ├── rz-vk5-plan.md：rz-vk5-plan.md — VK5 最小几何渲染闭环规划（Shader + Pipe
│   │   │   │   ├── vk4-c-r1-swapchain-fix.svg：里程碑示意图
│   │   │   │   ├── vulkan-lifecycle-plan.md：Vulkan 生命周期与架构边界方案
│   │   │   │   └── vulkan-preflight-audit-RZ-Fix3-0.md：RZ-Fix3-0: Vulkan 接入前置审计
│   │   │   ├── WORLD-A
│   │   │   │   ├── world-a-r0-coordinate-chain.svg：里程碑示意图
│   │   │   │   ├── world-a-r0-r1-tool-history-fix.svg：里程碑示意图
│   │   │   │   ├── world-a-r0-r2-transform-route-fix.svg：里程碑示意图
│   │   │   │   ├── world-a-r0-r3-gizmo-visibility.svg：里程碑示意图
│   │   │   │   ├── world-a-r1-entity-registry.svg：里程碑示意图
│   │   │   │   ├── world-a-r1-final-closure-report.md：FINAL 最终收口报告
│   │   │   │   ├── world-a-r1-final-closure.svg：里程碑示意图
│   │   │   │   ├── world-a-r1-r1-scene-consumption-audit.md：当前事实 Owner 矩阵
│   │   │   │   ├── world-a-r1-r1-scene-consumption.svg：里程碑示意图
│   │   │   │   ├── world-a-r1-r2-final-gate.md：多实体真实闭环与 1K Registry Gate
│   │   │   │   ├── world-a-r1-r2-multi-entity-gate.svg：里程碑示意图
│   │   │   │   ├── world-a-r1-r2-r1-acceptance-report.md：真机验收报告
│   │   │   │   ├── world-a-r1-r2-r1-acceptance.svg：里程碑示意图
│   │   │   │   ├── world-a-r1-r2-runtime-fix.svg：里程碑示意图
│   │   │   │   ├── world-a-r2-global-partition-report.md：Global Coordinate + World Partition 基础轮
│   │   │   │   ├── world-a-r2-global-partition.svg：里程碑示意图
│   │   │   │   ├── world-a-r2-r1-migration-activity-report.md：Migration + Activity 第一阶段
│   │   │   │   ├── world-a-r2-r1-migration-activity.svg：里程碑示意图
│   │   │   │   ├── world-a-r2-r2-partition-consistency-report.md：Partition Scale + Consistency Gate
│   │   │   │   ├── world-a-r2-r2-partition-consistency.svg：里程碑示意图
│   │   │   │   ├── world-a-r2-r3-inspector-manual-gate-report.md：Inspector Manual Gate Fix
│   │   │   │   ├── world-a-r2-r3-inspector-manual-gate.svg：里程碑示意图
│   │   │   │   ├── world-a-r2-r4-camera-framing-report.md：Editor Camera Framing
│   │   │   │   ├── world-a-r2-r4-camera-framing.svg：里程碑示意图
│   │   │   │   ├── world-a-r3-r1-spatial-consistency-report.md：Spatial Consistency
│   │   │   │   ├── world-a-r3-r1-spatial-consistency.svg：里程碑示意图
│   │   │   │   ├── world-a-r3-spatial-query-report.md：Spatial Index + World Query
│   │   │   │   ├── world-a-r3-spatial-query.svg：里程碑示意图
│   │   │   │   ├── world-a-ui-r1-display-cleanup-report.md：Display Cleanup
│   │   │   │   ├── world-a-ui-r1-display-cleanup.svg：里程碑示意图
│   │   │   │   ├── world-a-ui-r2-continuous-tree-report.md：Continuous Tree + Icon Refresh
│   │   │   │   └── world-a-ui-r2-continuous-tree.svg：里程碑示意图
│   │   │   ├── WORLD-B
│   │   │   │   ├── world-b-r0-editor-interaction-audit.md：编辑器基本操作现状审计与合同冻结
│   │   │   │   ├── world-b-r0-editor-interaction-audit.svg：里程碑示意图
│   │   │   │   ├── world-b-r1-camera-acceptance-closure.md：编辑器相机操作验收收口
│   │   │   │   ├── world-b-r1-camera-acceptance-closure.svg：里程碑示意图
│   │   │   │   ├── world-b-r1-camera-operation-report.md：编辑器相机操作实装报告
│   │   │   │   ├── world-b-r1-camera-operation.svg：里程碑示意图
│   │   │   │   ├── world-b-r2-selection-tool-state-report.md：选择与工具状态闭环报告
│   │   │   │   ├── world-b-r2-selection-tool-state.svg：里程碑示意图
│   │   │   │   ├── world-b-r3-move-transform-closure.md：移动变换闭环报告
│   │   │   │   ├── world-b-r3-move-transform-closure.svg：里程碑示意图
│   │   │   │   ├── world-b-r5-scale-transform-report.md：Scale Gizmo 缩放变换闭环报告
│   │   │   │   └── world-b-r5-scale-transform.svg：里程碑示意图
│   │   │   └── WORLD-C
│   │   │       ├── world-c-r0-scene-document-contract.md：场景文档契约冻结
│   │   │       ├── world-c-r0-scene-document-contract.svg：里程碑示意图
│   │   │       ├── world-c-r1-closure-report.md：最小场景保存与打开闭环收口
│   │   │       ├── world-c-r1-closure.svg：里程碑示意图
│   │   │       ├── world-c-r2-implementation-acceptance.md：实施与验收报告
│   │   │       ├── world-c-r2-ipo-manual-checklist.md：中文 IPO 真机验收卡
│   │   │       ├── world-c-r2-status.svg：里程碑示意图
│   │   │       ├── world-c-r3-viewport-reference-report.md：编辑器空间参照层实施报告
│   │   │       ├── world-c-r3-viewport-reference.svg：里程碑示意图
│   │   │       ├── world-c-r4-d0-asset-contracts.md：GLB 资产合同、依赖与场景 Schema 冻结
│   │   │       ├── world-c-r4-d1-glb-import-core.md：GLB 静态模型解析与玄域数据转换
│   │   │       ├── world-c-r4-d2-f1-ipo-checklist.md：中文 IPO 真机验收卡
│   │   │       ├── world-c-r4-d2-static-model-rendering.md：Vulkan 静态模型显示
│   │   │       ├── world-c-r4-d3-static-model-authoring-report.md：真实 GLB 导入闭环报告
│   │   │       ├── world-c-r4-d4-i1-hosted-assets-report.md：`.xyassets` 托管资源事务内核报告
│   │   │       └── world-c-r4-d4-static-model-persistence-report.md：静态模型持久化完整闭环报告
│   │   └── current
│   │       └── MAP-A
│   │           ├── map-a-r1-d1-map-contracts.md：.xymap 地图合同冻结
│   │           ├── map-a-r1-d5-r1-f2-grid-stabilize.svg：里程碑示意图
│   │           ├── map-a-r1-d5-r1-f2-r2-unified-grid-lod.svg：里程碑示意图
│   │           ├── map-a-r1-d5-r1-f2-r3-grid-ground-visual.svg：里程碑示意图
│   │           ├── map-a-r1-d5-r1-f2-r3-r2-per-pixel-background.svg：里程碑示意图
│   │           ├── map-a-r1-d5-r1-f3-f1-overlay-gizmo.svg：里程碑示意图
│   │           ├── map-a-r1-d5-r1-f3-f2-camera-basis-recovery.svg：里程碑示意图
│   │           ├── map-a-r1-d5-r1-f3-f3-gizmo-recovery.svg：里程碑示意图
│   │           └── map-a-r1-d5-r1-f3-viewport-navigation-gizmo.svg：里程碑示意图
│   └── 玄域引擎_AI开发宪法.md：玄域引擎 AI 开发宪法 2.0
├── file-tree.md：XuanYu Engine 文件树
├── run.bat：启动脚本
├── samples
│   └── world-c-r1-ten-triangles.xyscene：场景样例
└── scripts
    ├── arch-a-guard-editor.ps1：PowerShell 脚本
    ├── arch-a-guard-render.ps1：PowerShell 脚本
    ├── arch-a-guard-warcore.ps1：PowerShell 脚本
    ├── arch-a-guard-world.ps1：PowerShell 脚本
    └── arch-a-guard.ps1：PowerShell 脚本
```
