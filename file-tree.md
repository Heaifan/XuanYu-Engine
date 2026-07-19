版本：v0.2.17.23-fix
# XuanYu Engine 文件树

文件总数：310

## 根目录

- `NuGet.Config`：NuGet 源配置。
- `XuanYu.Engine.slnx`：解决方案入口，组织当前各项目。
- `changelog.md`：项目变更记录，按时间倒序记录阶段性修改。
- `file-tree.md`：当前文件树与每个文件职责说明。
- `run.bat`：Windows 启动脚本，负责 restore、build 并运行编辑器。

## scripts

- `scripts/arch-a-guard.ps1`：ARCH-A 自动守卫脚本，检查依赖边界、启动入口、版本一致性和 5+100 等约束。

## docs

- `docs/AI_DEVELOPMENT_RULES.md`：AI 协作开发规则，保留作历史参考。
- `docs/CODE_CONSTITUTION.md`：代码宪法与结构约束，保留作治理参考。
- `docs/ENGINE_ARCHITECTURE.md`：引擎总体架构说明。
- `docs/LEGACY_FLUIDWARFARE_OLD_AUDIT.md`：旧 FluidWarfare 项目审计记录。
- `docs/MILESTONE1_PUBLIC_VALIDATION.md`：里程碑 1 公开验证说明。
- `docs/NAMING_RULES.md`：命名规则文档。
- `docs/PHASE1_SCOPE.md`：Phase 1 范围定义。
- `docs/PROJECT_CHARTER.md`：项目章程。
- `docs/arch-a-plan.md`：ARCH-A 规划文档，记录 UI 与 Vulkan 依赖边界。
- `docs/arch-b-plan.md`：ARCH-B 规划文档，记录编辑器状态所有权与交互事务边界。
- `docs/arch-c-overview.svg`：ARCH-C 规划总览图。
- `docs/arch-c-plan.md`：ARCH-C 真实场景编辑交互闭环规划文档。
- `docs/arch-c-r2-entry-audit.md`：ARCH-C-R2 坐标与相机入口门审计；不实现 Picking，只记录阻断证据和下一步契约边界。
- `docs/arch-c-r2-spatial-query.svg`：ARCH-C-R2 空间查询架构图；不承载运行时代码，仅用于人工验收与规划沟通。
- `docs/arch-c-r2b-space-fact.svg`：ARCH-C-R2-B 统一空间事实架构图；用于说明 Camera / Viewport / ViewProjection / WorldRay 的共享关系，不承载运行时代码。
- `docs/arch-c-r2b-closure.svg`：ARCH-C-R2-B 正式封版状态图；用于说明数学契约已通过、下一步转入渲染接入统一空间事实，不承载运行时代码。
- `docs/arch-c-r2-current-route.svg`：ARCH-C-R2 当前阶段路线图；用于说明 R2-A / R2-B 已完成以及 R2-C 渲染接入统一空间事实的下一步，不承载运行时代码。
- `docs/arch-c-r2c-render-space.svg`：ARCH-C-R2-C 渲染接入统一空间事实架构图；用于说明世界位置、统一 ViewProjection 与 Vulkan push constant 的关系，不承载运行时代码。
- `docs/arch-c-r2c-closure.svg`：ARCH-C-R2-C 正式封版状态图；用于说明真机渲染、坐标契约、Resize、自愈和释放链均已通过，不承载运行时代码。
- `docs/arch-c-r2d-spatial-index.svg`：ARCH-C-R2-D 空间索引架构图；用于说明场景事实、增量维护、动态索引和候选查询关系，不承载运行时代码。
- `docs/arch-c-r2e-ray-hit.svg`：ARCH-C-R2-E 精确命中架构图；用于说明 WorldRay、Broad Phase、Ray-AABB Narrow Phase 和最近命中的关系，不承载运行时代码。
- `docs/arch-c-r2f-pointer-picking.svg`：ARCH-C-R2-F 真实 Pointer Picking 架构图；用于说明 PointerPressed 到 EntityKey / NoHit 的最小闭环，不承载运行时代码。
- `docs/arch-c-r3-selection.svg`：ARCH-C-R3 真实 Selection 架构图；说明 Picking 结果经唯一 Owner 同步到 Tree 与 Inspector，不承载运行时代码。
- `docs/arch-c-r3-timeout-fix.svg`：R3 真机收口 Timeout 修复图；说明 Acquire 超时按可恢复空帧处理、其他错误仍保持致命语义，不承载运行时代码。
- `docs/arch-c-r4-move-gizmo.svg`：R4 Move Gizmo 架构图；说明统一相机、三轴投影、输入优先级和 Capture 唯一所有权，不承载运行时代码。
- `docs/arch-c-r4-r1-gizmo-hit.svg`：R4-R1 Move Gizmo 命中收口图；说明真机点击容错、Gizmo 优先级和 Scene Picking 回落边界，不承载运行时代码。
- `docs/audit-EditorShellV2-9.1A-1.md`：EditorShellV2 9.1A 第一轮审计。
- `docs/audit-EditorShellV2-freeze-9.1A-Freeze.md`：EditorShellV2 冻结问题审计。
- `docs/audit-EditorShellV2-input-9.1A-2.md`：EditorShellV2 输入链路审计。
- `docs/audit-EditorShellV2-input-9.1A-2R.md`：EditorShellV2 输入链路复审。
- `docs/audit-EditorShellV2-picking-gizmo-9.1A-3.md`：EditorShellV2 Picking / Gizmo 审计。
- `docs/audit-EditorShellV2-picking-gizmo-9.1A-3R.md`：EditorShellV2 Picking / Gizmo 复审。
- `docs/audit-EditorShellV2-plan-9.1A-0.md`：EditorShellV2 9.1A 审计计划。
- `docs/audit-NativeViewportMouseCapture-lifecycle-9.0X.md`：Native Viewport 鼠标捕获生命周期审计。
- `docs/audit-RZ-New-0-onboarding.md`：RZ-New-0 接手与初始化审计。
- `docs/audit-RZ-VK1-vulkan-probe.md`：RZ-VK1 Vulkan Probe 审计。
- `docs/audit-RZ-VK2-R1-nativehost-resize-coalesce.md`：NativeHost Resize 合并第一轮审计。
- `docs/audit-RZ-VK2-R2-nativehost-resize-coalesce-verify.md`：NativeHost Resize 合并验证审计。
- `docs/audit-RZ-VK2-native-host-lifecycle.md`：NativeHost 生命周期审计。
- `docs/audit-gizmo-chain-9.0Y-1.md`：Gizmo 链路审计 9.0Y-1。
- `docs/audit-gizmo-chain-9.0Y-2.md`：Gizmo 链路审计 9.0Y-2。
- `docs/audit-gizmo-chain-9.0Y-3.md`：Gizmo 链路审计 9.0Y-3。
- `docs/audit-gizmo-stash-9.0Y-0.md`：Gizmo 暂存状态审计。
- `docs/audit-input-lifecycle-9.0X-1.md`：输入生命周期审计 9.0X-1。
- `docs/audit-input-lifecycle-9.0X-2.md`：输入生命周期审计 9.0X-2。
- `docs/audit-input-lifecycle-9.0X-3.md`：输入生命周期审计 9.0X-3。
- `docs/audit-inspector-transform-9.0C-0.md`：Inspector / Transform 同步审计。
- `docs/dev-rules-understanding.md`：开发规则理解与执行说明。
- `docs/dev-rules.md`：开发规则文档。
- `docs/diagnostic-safety.md`：诊断日志、底部日志准入与 UI 调度安全规范。
- `docs/editor-top-area-target-9.1B.md`：顶部区域目标说明。
- `docs/editor-top-svg-icons-9.1C-R.md`：顶部 SVG 图标细修说明。
- `docs/editor-top-svg-icons-9.1C.md`：顶部 SVG 图标替换说明。
- `docs/editor-ui-terms-9.1B.md`：编辑器 UI 术语说明。
- `docs/gizmo_drag_audit_2026-06-25.md`：Gizmo 拖动审计报告。
- `docs/gizmo_drag_audit_probe.log`：Gizmo 拖动审计探针日志。
- `docs/log-ux-1-r2-autoscroll.svg`：LOG-UX 自动滚动设计图。
- `docs/naming-XuanYu-Engine.md`：XuanYu Engine 命名迁移说明。
- `docs/plan-9.0D-move-gizmo-final.md`：Move Gizmo 最终验收计划。
- `docs/project-baseline-audit-org-1-r1.md`：ORG-1-R1 项目基线审计修正版。
- `docs/project-baseline-audit-org-1.md`：ORG-1 项目真实基线审计。
- `docs/rz-vk3-closure.md`：RZ-VK3 阶段收口文档。
- `docs/rz-vk3-surface-lifecycle-plan.md`：RZ-VK3 Surface 生命周期规划。
- `docs/rz-vk4-c-r1-audit-plan.md`：RZ-VK4-C-R1 审计计划。
- `docs/rz-vk4-c-swapchain-plan.md`：RZ-VK4-C Swapchain 规划。
- `docs/rz-vk4-closure.md`：RZ-VK4 阶段收口文档。
- `docs/rz-vk4-d-plan.md`：RZ-VK4-D 规划文档。
- `docs/rz-vk4-plan.md`：RZ-VK4 总规划文档。
- `docs/rz-vk5-a-plan.md`：RZ-VK5-A 规划文档。
- `docs/rz-vk5-c-plan.md`：RZ-VK5-C 规划文档。
- `docs/rz-vk5-e-plan.md`：RZ-VK5-E 规划文档。
- `docs/rz-vk5-plan.md`：RZ-VK5 总规划文档。
- `docs/vk4-c-r1-swapchain-fix.svg`：VK4-C-R1 Swapchain 修复示意图。
- `docs/vulkan-lifecycle-plan.md`：Vulkan 生命周期规划。
- `docs/vulkan-preflight-audit-RZ-Fix3-0.md`：Vulkan 前置审计文档。
- `docs/版本号规范与历史映射.md`：版本号规范与历史编号映射。
- `docs/玄域引擎_AI开发宪法.md`：玄域引擎 AI 开发宪法，总治理文档。

## XuanYu.Core

- `XuanYu.Core/Gizmo/MoveGizmoAxis.cs`：Move Gizmo 世界轴身份；只定义 X/Y/Z，不承担 Transform 或渲染状态。
- `XuanYu.Core/Gizmo/MoveGizmoLayout.cs`：Move Gizmo 屏幕投影、精确命中与 R4-R2 Guard 命中；消费统一 ViewProjection，不访问 Scene SpatialIndex、Selection 或 Vulkan。
- `XuanYu.Core/Gizmo/MoveGizmoSegment.cs`：单根 Gizmo 轴的屏幕线段值对象；只提供投影长度，不持有交互生命周期。
- `XuanYu.Core/Gizmo/ScreenPoint.cs`：后端无关屏幕逻辑坐标值对象；不依赖 Avalonia、Win32 或 Vulkan。

- `XuanYu.Core/XuanYu.Core.csproj`：核心类库项目文件。
- `XuanYu.Core/Properties/AssemblyInfo.cs`：Core 程序集内部可见性声明；仅允许 `XuanYu.Core.Tests` 访问内部测试入口，不承载生产行为或运行时依赖。
- `XuanYu.Core/Diagnostics/CoreSelfTest.cs`：Core 自检入口。
- `XuanYu.Core/Identity/EntityId.cs`：实体 ID 值对象。
- `XuanYu.Core/Logging/EngineLogEntry.cs`：引擎日志条目。
- `XuanYu.Core/Logging/EngineLogLevel.cs`：引擎日志等级。
- `XuanYu.Core/Math/Vector3d.cs`：三维向量值对象。
- `XuanYu.Core/Math/YawRotation.cs`：Yaw 旋转值对象。
- `XuanYu.Core/Picking/ViewportPickingRequest.cs`：视口拾取请求值对象；负责携带请求序号、ViewportState、CameraState、逻辑坐标、查询掩码和 SpatialRevision，不执行射线命中。
- `XuanYu.Core/Picking/ViewportPickingResult.cs`：视口拾取结果值对象；负责表达 EntityKey / NoHit、ViewportRevision、SpatialRevision 和 Raycast 统计，不写 Selection。
- `XuanYu.Core/Picking/ViewportPickingService.cs`：视口拾取 Core 服务；负责把视口点转换为 WorldRay 并调用空间 Raycast，同时校验 ViewportRevision / SpatialRevision，不依赖 Avalonia 或 Vulkan。
- `XuanYu.Core/Space/CameraState.cs`：渲染后端无关的相机状态契约；负责校验位置、方向、Up、FOV、裁剪面和 Revision，不负责渲染资源、输入事件或 Picking 命中。
- `XuanYu.Core/Space/ViewportState.cs`：渲染后端无关的视口状态契约；负责记录逻辑区域、物理尺寸、DPI 和 Revision，不等同于 Vulkan Swapchain。
- `XuanYu.Core/Space/ViewProjectionState.cs`：统一观察事实构建器；负责从 Camera / Viewport 生成 View、Projection、ViewProjection 和逆矩阵，不负责实体筛选或空间索引。
- `XuanYu.Core/Space/DefaultEditorCamera.cs`：默认编辑器斜视相机合同；从固定 Position/Target/Up 派生 CameraState，供 Render、Picking、Gizmo 共用，不改变世界坐标语义。
- `XuanYu.Core/Space/WorldRay.cs`：世界射线值对象；负责保存有限 Origin 和归一化 Direction，不负责命中测试或实体选择。
- `XuanYu.Core/Space/WorldRayFactory.cs`：视口点到世界射线的转换入口；负责 NDC 与逆矩阵反投影，不负责 Ray-AABB、Picking、Selection 或 Gizmo。
- `XuanYu.Core/Spatial/DynamicAabbTree.cs`：动态 AABB 树索引入口；负责 Insert、Remove、Update 和 Query 调度，不暴露内部节点给调用方。
- `XuanYu.Core/Spatial/DynamicAabbTree.Insert.cs`：动态 AABB 树插入分部；负责寻找兄弟节点和接入叶节点，不负责场景事实所有权。
- `XuanYu.Core/Spatial/DynamicAabbTree.Node.cs`：动态 AABB 树内部节点模型；只在索引内部保存父子关系和包围盒，不作为公共契约。
- `XuanYu.Core/Spatial/DynamicAabbTree.Query.cs`：动态 AABB 树候选查询分部；负责 AABB / WorldRay Broad Phase 节点裁剪和统计访问节点数，不做最近命中或最终 Picking。
- `XuanYu.Core/Spatial/DynamicAabbTree.Refit.cs`：动态 AABB 树回填分部；负责实体增删改后的父级 AABB 更新，不负责平衡策略外露。
- `XuanYu.Core/Spatial/DynamicAabbTree.Remove.cs`：动态 AABB 树删除分部；负责移除叶节点并接回兄弟节点，不负责实体生命周期决策。
- `XuanYu.Core/Spatial/ISpatialIndex.cs`：空间索引抽象契约；负责屏蔽具体索引实现并提供 AABB / WorldRay 候选查询，不绑定 DynamicAabbTree、UI 或 Vulkan。
- `XuanYu.Core/Spatial/RayAabbHit.cs`：Ray-AABB 命中值对象；负责保存命中距离与命中点，不保存实体选择状态。
- `XuanYu.Core/Spatial/RayAabbIntersection.cs`：实体 AABB Narrow Phase 数学；负责正向、最大距离、盒内起点、擦边和轴平行命中规则，不执行空间索引遍历。
- `XuanYu.Core/Spatial/SpatialAabb.cs`：世界空间 AABB 值对象；负责有限性、大小关系、相交和合并计算，不保存实体状态。
- `XuanYu.Core/Spatial/SpatialBounds.cs`：实体空间边界值对象；负责绑定 EntityKey、WorldBounds 和 QueryCategory，不成为第二份场景数据库。
- `XuanYu.Core/Spatial/SpatialIndexOwner.cs`：空间索引生命周期所有者；负责增量维护索引、SpatialRevision、AABB / WorldRay 查询统计，不拥有正式 Transform。
- `XuanYu.Core/Spatial/SpatialRayAabb.cs`：空间射线与 AABB 的 Broad Phase 相交计算；只服务候选裁剪，不裁定最近命中。
- `XuanYu.Core/Spatial/SpatialRayQuery.cs`：有界 WorldRay 查询值对象；负责携带射线和最大查询距离，不绑定 Picking 或 Selection。
- `XuanYu.Core/Spatial/SpatialRaycastHit.cs`：空间射线最近命中值对象；负责携带 EntityKey、距离、命中点和 SpatialRevision，不触发 Selection。
- `XuanYu.Core/Spatial/SpatialRaycastResolver.cs`：空间射线命中解析器；负责对 Broad Phase 候选执行 O(k) Ray-AABB、前后校验 SpatialRevision 并按距离 / EntityKey 稳定选最近，不扫描全场景。
- `XuanYu.Core/Spatial/SpatialRaycastResult.cs`：空间射线命中结果；负责表达 Hit / NoHit 与统计信息，不包含材质、法线、Mesh 三角形或 UI 状态。
- `XuanYu.Core/Spatial/SpatialRaycastStats.cs`：空间射线命中诊断统计；负责记录总实体、访问节点、候选数、精确检测数和真实命中数，并生成低频中文探针文本。
- `XuanYu.Core/Spatial/SpatialQueryCategory.cs`：空间查询分类掩码；负责长期扩展场景实体、地形、Gizmo 和编辑器辅助对象分类。
- `XuanYu.Core/Spatial/SpatialQueryResult.cs`：空间候选查询结果；负责携带候选 Bounds 与统计信息，不裁定最近命中。
- `XuanYu.Core/Spatial/SpatialQueryStats.cs`：空间查询诊断统计；负责记录 Revision、总实体、访问节点和候选数，并生成低频中文探针文本。
- `XuanYu.Core/Scene/CommittedTransform.cs`：已提交 Transform 值对象，当前保存正式 Position。
- `XuanYu.Core/Scene/ISceneRenderSnapshotSource.cs`：场景渲染快照源抽象，向渲染侧发布只读快照。
- `XuanYu.Core/Scene/SceneEntitySnapshot.cs`：最小场景实体快照，包含 EntityKey、名称、类型和 Transform。
- `XuanYu.Core/Scene/SceneRenderSnapshot.cs`：渲染侧消费的场景快照，当前包含单个最小实体。
- `XuanYu.Core/Scene/SceneStateOwner.cs`：场景状态所有者，负责提交 Position、同步派生空间索引并发布渲染快照；空间索引不是第二份场景真相。
- `XuanYu.Core/Results/EngineError.cs`：引擎错误值对象。
- `XuanYu.Core/Results/EngineResult.cs`：引擎结果类型。
- `XuanYu.Core/Time/SimulationTime.cs`：模拟时间值对象。
- `XuanYu.Core/Time/TimeStep.cs`：时间步长值对象。

## XuanYu.Core.Tests

- `XuanYu.Core.Tests/Gizmo/MoveGizmoLayoutTests.cs`：三轴投影、X/Y/Z 命中、R4-R2 Guard 容错、Miss 和确定性裁决测试；不验证 Vulkan 像素输出。

- `XuanYu.Core.Tests/XuanYu.Core.Tests.csproj`：Core 长期自动测试宿主项目文件；只负责引用测试依赖和 `XuanYu.Core`，不向生产项目传递测试依赖或工具链。
- `XuanYu.Core.Tests/CoreSmokeTests.cs`：Core 测试宿主最小烟雾测试；验证测试发现、执行链路和基础 Core 行为，不负责 R2-B 空间数学覆盖。
- `XuanYu.Core.Tests/Picking/ViewportPickingServiceTests.cs`：视口拾取 Core 测试；负责中心命中、空白 NoHit、移动后新旧位置、DPI 逻辑坐标和代际过期拒绝覆盖。
- `XuanYu.Core.Tests/Space/CameraStateTests.cs`：CameraState 自动测试；负责合法相机、退化方向、共线 Up、非法 FOV / Near / Far / 非有限数覆盖，不负责渲染画面验收。
- `XuanYu.Core.Tests/Space/SpaceAssert.cs`：空间数学测试辅助断言；只负责局部近似比较，不进入生产项目。
- `XuanYu.Core.Tests/Space/ViewportStateTests.cs`：ViewportState 自动测试；负责合法尺寸、DPI、Revision、幂等和非法尺寸覆盖，不负责平台窗口尺寸同步。
- `XuanYu.Core.Tests/Space/ViewProjectionStateTests.cs`：ViewProjectionState 自动测试；负责已知 View 矩阵、投影宽高比和矩阵可逆性覆盖，不负责 Vulkan 投影落地。
- `XuanYu.Core.Tests/Space/DefaultEditorCameraTests.cs`：默认斜视相机 Forward 派生与中心射线合同测试；不修改世界轴约定。
- `XuanYu.Core.Tests/Space/WorldRayFactoryTests.cs`：WorldRay 自动测试；负责中心点、角落、Resize、稳定复现和非法输入覆盖，不负责实体 Picking。
- `XuanYu.Core.Tests/Space/WorldRayTests.cs`：WorldRay 值对象自动测试；负责非法 Origin / Direction 失败边界，不负责射线命中或空间查询。
- `XuanYu.Core.Tests/Spatial/SpatialBoundsTests.cs`：空间边界测试；负责 AABB 非法输入、相交和合并行为覆盖，不测试 Picking。
- `XuanYu.Core.Tests/Spatial/RayAabbIntersectionTests.cs`：Ray-AABB 数学测试；负责正面命中、miss、背向、负方向、盒内起点、平行轴、擦边、擦角和最大距离覆盖。
- `XuanYu.Core.Tests/Spatial/SceneStateOwnerSpatialTests.cs`：SceneStateOwner 空间索引集成测试；负责初始化 Insert、Position Update、EntityKey 稳定和 Revision 幂等覆盖。
- `XuanYu.Core.Tests/Spatial/SpatialIndexOwnerLifecycleTests.cs`：空间索引生命周期测试；负责 Insert、Remove、Update、重复实体和分类掩码覆盖。
- `XuanYu.Core.Tests/Spatial/SpatialIndexOwnerRevisionTests.cs`：空间索引 Revision 测试；负责 SpatialRevision 增长、幂等更新和中文探针统计覆盖。
- `XuanYu.Core.Tests/Spatial/SpatialIndexScaleTests.cs`：空间索引规模回归测试；负责 1k / 10k 实体查询统计、连续移动和批量删除一致性覆盖。
- `XuanYu.Core.Tests/Spatial/SpatialRayQueryLifecycleTests.cs`：WorldRay 候选查询生命周期与规模测试；负责 Update、Remove、1k / 10k Ray Query 统计覆盖。
- `XuanYu.Core.Tests/Spatial/SpatialRayQueryTests.cs`：WorldRay 候选查询边界测试；负责命中、空查询、Mask、起点在盒内、平行轴、背向和最大距离覆盖。
- `XuanYu.Core.Tests/Spatial/SpatialRaycastNearestTests.cs`：最近命中测试；负责多实体最近命中、候选顺序变化、等距 EntityKey 稳定裁决和 Broad 候选必须经过 Narrow 才能发布命中的责任分离覆盖。
- `XuanYu.Core.Tests/Spatial/SpatialRaycastRevisionTests.cs`：射线命中 Revision 测试；负责命中 / 未命中结果携带同一 SpatialRevision，以及 Narrow Phase 期间变代会被最终校验拒绝的覆盖。
- `XuanYu.Core.Tests/Spatial/SpatialRaycastScaleTests.cs`：射线命中规模回归测试；负责 1k / 10k Broad 到 Narrow 端到端统计和不全量扫描约束。
- `XuanYu.Core.Tests/Spatial/SpatialTestData.cs`：空间索引测试数据工厂；负责确定性网格实体和查询 AABB 构造，不进入生产项目。

## XuanYu.Render.Abstractions

- `XuanYu.Render.Abstractions/XuanYu.Render.Abstractions.csproj`：渲染抽象项目文件。
- `XuanYu.Render.Abstractions/INativeHostSurfaceBridge.cs`：NativeHost Surface 桥接抽象。
- `XuanYu.Render.Abstractions/INativeHostSurfaceBridgeFactory.cs`：NativeHost Surface 桥接工厂抽象。
- `XuanYu.Render.Abstractions/NativeHostHandleSnapshot.cs`：NativeHost 句柄快照。
- `XuanYu.Render.Abstractions/NativeHostLifecycleLogFormatter.cs`：NativeHost 生命周期日志格式化器。
- `XuanYu.Render.Abstractions/NativeHostLifecycleProbe.cs`：NativeHost 生命周期探针数据。
- `XuanYu.Render.Abstractions/NativeHostLifecycleState.cs`：NativeHost 生命周期状态枚举。
- `XuanYu.Render.Abstractions/NativeHostSurfaceHandle.cs`：NativeHost Surface 句柄值对象。

## XuanYu.Render.Vulkan

- `XuanYu.Render.Vulkan/XuanYu.Render.Vulkan.csproj`：Vulkan 渲染实现项目文件。
- `XuanYu.Render.Vulkan/VulkanApiProbe.cs`：Vulkan API 可用性探针。
- `XuanYu.Render.Vulkan/VulkanBridgeLogFormatter.cs`：Vulkan 桥接日志格式化器。
- `XuanYu.Render.Vulkan/VulkanDeviceInfo.cs`：Vulkan 设备信息模型。
- `XuanYu.Render.Vulkan/VulkanInstanceCreateInfoBuilder.cs`：Vulkan Instance 创建参数构建器。
- `XuanYu.Render.Vulkan/VulkanInstanceExtensions.cs`：Vulkan Instance 扩展辅助。
- `XuanYu.Render.Vulkan/VulkanInstanceLogFormatter.cs`：Vulkan Instance 日志格式化器。
- `XuanYu.Render.Vulkan/VulkanInstanceOwner.cs`：Vulkan Instance 生命周期持有者。
- `XuanYu.Render.Vulkan/VulkanInstanceResult.cs`：Vulkan Instance 创建结果。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.Attach.cs`：Vulkan NativeHost 桥接 Attach 分部。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.Lifecycle.cs`：Vulkan NativeHost 桥接生命周期分部。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.Resize.cs`：Vulkan NativeHost 桥接 Resize 分部。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.Scene.cs`：Vulkan NativeHost 桥接场景快照订阅分部。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridge.cs`：Vulkan NativeHost Surface 桥接主体。
- `XuanYu.Render.Vulkan/VulkanNativeHostSurfaceBridgeFactory.cs`：Vulkan NativeHost Surface 桥接工厂。
- `XuanYu.Render.Vulkan/VulkanProbeLogFormatter.cs`：Vulkan 探针日志格式化器。
- `XuanYu.Render.Vulkan/VulkanProbeResult.cs`：Vulkan 探针结果。
- `XuanYu.Render.Vulkan/VulkanSurfaceLogFormatter.cs`：Vulkan Surface 日志格式化器。
- `XuanYu.Render.Vulkan/VulkanSurfaceOwner.cs`：Vulkan Surface 生命周期持有者。
- `XuanYu.Render.Vulkan/VulkanSurfaceResult.cs`：Vulkan Surface 创建结果。
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgeDeviceAttachStep.cs`：Vulkan 桥接逻辑设备 Attach 步骤。
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgePhysicalDeviceAttachStep.cs`：Vulkan 桥接物理设备 Attach 步骤。
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgeRenderSessionAttachStep.cs`：Vulkan 桥接渲染 Session Attach 步骤。
- `XuanYu.Render.Vulkan/Bridge/VulkanBridgeSwapchainAttachStep.cs`：Vulkan 桥接 Swapchain Attach 步骤。
- `XuanYu.Render.Vulkan/Device/VulkanDeviceOwner.cs`：Vulkan 逻辑设备生命周期持有者。
- `XuanYu.Render.Vulkan/Device/VulkanPhysicalDeviceInfo.cs`：Vulkan 物理设备信息。
- `XuanYu.Render.Vulkan/Device/VulkanPhysicalDeviceSelection.cs`：Vulkan 物理设备选择结果。
- `XuanYu.Render.Vulkan/Device/VulkanPhysicalDeviceSelector.cs`：Vulkan 物理设备选择器。
- `XuanYu.Render.Vulkan/Device/VulkanQueueFamilySelection.cs`：Vulkan 队列族选择结果。
- `XuanYu.Render.Vulkan/Diagnostic/VulkanResizeTracer.cs`：Vulkan Resize 追踪诊断工具。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.Frag.cs`：片元着色器 SPIR-V 字节码。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.Vert.cs`：顶点着色器 SPIR-V 字节码。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.cs`：Vulkan 图形管线生命周期持有者。
- `XuanYu.Render.Vulkan/Pipeline/VulkanPipelineLogFormatter.cs`：Vulkan 管线日志格式化器。
- `XuanYu.Render.Vulkan/Pipeline/VulkanScenePushConstants.cs`：Vulkan 场景 push constant 布局常量；负责统一 shader、PipelineLayout 与命令录制的字节大小，不负责资源生命周期。
- `XuanYu.Render.Vulkan/Pipeline/VulkanShaderModuleOwner.cs`：Vulkan ShaderModule 生命周期持有者。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameLogFormatter.cs`：Vulkan ClearFrame 日志格式化器。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Commands.cs`：Vulkan ClearFrame 命令录制分部。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Draw.cs`：Vulkan ClearFrame 绘制分部；负责把场景 World Position 与统一 ViewProjection 写入 push constant 并发起 Draw，不负责 Picking、Selection 或生命周期重构。
- `XuanYu.Render.Vulkan/Shaders/scene.vert`：场景三角形与最小 Move Gizmo 三轴顶点着色器源码；由 glslc 生成内嵌 SPIR-V，不负责命中测试。
- `XuanYu.Render.Vulkan/Shaders/scene.frag`：场景与 Move Gizmo 顶点颜色片元着色器源码；不负责 Selection 或 Pipeline 生命周期。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Lifecycle.cs`：Vulkan ClearFrame 生命周期分部。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Resources.cs`：Vulkan ClearFrame 资源创建分部。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs`：Vulkan ClearFrame 资源持有主体。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.Frame.cs`：Vulkan Present 单帧执行分部。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.Lifecycle.cs`：Vulkan Present 泵生命周期分部。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs`：Vulkan Present 泵主体；有限 Acquire 等待超时只跳过当前帧，其他结果交由既有错误与自愈合同处理，不负责 Selection 或 Gizmo。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Lifecycle.cs`：Vulkan 渲染 Session 生命周期分部。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Recover.cs`：Vulkan 渲染 Session 自愈分部。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.Resize.cs`：Vulkan 渲染 Session Resize 分部。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs`：Vulkan 渲染 Session 主体。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainBuilder.cs`：Vulkan Swapchain 创建参数构建器。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainCapabilities.cs`：Vulkan Swapchain 能力查询结果。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainLogFormatter.cs`：Vulkan Swapchain 日志格式化器。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.Accessors.cs`：Vulkan Swapchain 只读访问器分部。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs`：Vulkan Swapchain 生命周期持有者。

## XuanYu.Editor.App

- `XuanYu.Editor.App/XuanYu.Editor.App.csproj`：编辑器应用组装入口项目文件。
- `XuanYu.Editor.App/EditorCompositionRoot.cs`：编辑器依赖组装根。
- `XuanYu.Editor.App/Program.cs`：编辑器应用启动入口。

## XuanYu.Editor.Win

- `XuanYu.Editor.Win/XuanYu.Editor.Win.csproj`：旧 WinForms 编辑器壳项目文件。
- `XuanYu.Editor.Win/MainForm.cs`：旧 WinForms 主窗体。

## XuanYu.Editor.UI

- `XuanYu.Editor.UI/XuanYu.Editor.UI.csproj`：Avalonia 编辑器 UI 项目文件。
- `XuanYu.Editor.UI/NativeHostResizeCoalescer.cs`：NativeHost Resize 合并器。
- `XuanYu.Editor.UI/NativeHostResizeSnapshot.cs`：NativeHost Resize 快照。
- `XuanYu.Editor.UI/NativeHostSurfaceContract.cs`：NativeHost Surface 合约。
- `XuanYu.Editor.UI/RelayCommand.cs`：ICommand 简易实现。
- `XuanYu.Editor.UI/Ui.axaml`：全局 UI 样式资源。
- `XuanYu.Editor.UI/ViewportNativeHostRoute.cs`：视口 NativeHost 路由入口。
- `XuanYu.Editor.UI/app.manifest`：Windows 应用清单。
- `XuanYu.Editor.UI/Bootstrap/App.axaml`：Avalonia 应用资源入口。
- `XuanYu.Editor.UI/Bootstrap/App.axaml.cs`：Avalonia 应用启动与主窗口挂载。
- `XuanYu.Editor.UI/Bootstrap/Program.cs`：Avalonia 桌面启动入口。
- `XuanYu.Editor.UI/EditorState/EditorInteractionChangedResult.cs`：交互事务状态变更结果。
- `XuanYu.Editor.UI/EditorState/EditorInteractionCommand.cs`：交互事务命令定义。
- `XuanYu.Editor.UI/EditorState/EditorInteractionPointerSnapshot.cs`：交互事务 Pointer 快照。
- `XuanYu.Editor.UI/EditorState/EditorInteractionSnapshot.cs`：交互事务只读快照。
- `XuanYu.Editor.UI/EditorState/EditorSelectionCommand.cs`：编辑器选择命令定义。
- `XuanYu.Editor.UI/EditorState/EditorSelectionSnapshot.cs`：编辑器选择只读快照。
- `XuanYu.Editor.UI/EditorState/EditorStateChangedResult.cs`：编辑器选择状态变更结果。
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.Interaction.cs`：EditorStateOwner 交互事务分部。
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.Tool.cs`：EditorStateOwner 工具状态分部。
- `XuanYu.Editor.UI/EditorState/EditorStateOwner.cs`：EditorStateOwner 主体与选择状态所有权。
- `XuanYu.Editor.UI/EditorState/EditorToolChangedResult.cs`：工具状态变更结果。
- `XuanYu.Editor.UI/EditorState/EditorToolCommand.cs`：工具切换命令定义。
- `XuanYu.Editor.UI/EditorState/EditorToolId.cs`：编辑器工具身份枚举。
- `XuanYu.Editor.UI/EditorState/EditorToolSnapshot.cs`：编辑器工具只读快照。
- `XuanYu.Editor.UI/EditorState/EditorToolText.cs`：工具身份与中文文案映射。
- `XuanYu.Editor.UI/Foot/Foot.axaml`：底部日志栏界面。
- `XuanYu.Editor.UI/Foot/Foot.axaml.cs`：底部日志栏代码后置。
- `XuanYu.Editor.UI/Foot/LogDetailPanel.axaml`：日志详情面板界面。
- `XuanYu.Editor.UI/Foot/LogDetailPanel.axaml.cs`：日志详情面板代码后置。
- `XuanYu.Editor.UI/Foot/LogListAutoScrollController.cs`：日志列表自动滚动控制器。
- `XuanYu.Editor.UI/Icons/EditorIcons.axaml`：编辑器图标资源。
- `XuanYu.Editor.UI/Left/Left.axaml`：左侧项目与层级面板界面。
- `XuanYu.Editor.UI/Left/Left.axaml.cs`：左侧面板代码后置。
- `XuanYu.Editor.UI/Main/Main.axaml`：中央主视口区域界面。
- `XuanYu.Editor.UI/Main/Main.axaml.cs`：中央主视口区域代码后置。
- `XuanYu.Editor.UI/Right/Right.axaml`：右侧检查器与调试面板界面。
- `XuanYu.Editor.UI/Right/Right.axaml.cs`：右侧面板代码后置。
- `XuanYu.Editor.UI/Root/UiRoot.axaml`：主布局根界面。
- `XuanYu.Editor.UI/Root/UiRoot.axaml.cs`：主布局根代码后置。
- `XuanYu.Editor.UI/Top/Top.axaml`：顶部工具栏界面。
- `XuanYu.Editor.UI/Top/Top.axaml.cs`：顶部工具栏代码后置。
- `XuanYu.Editor.UI/Viewport/Vulkan/NativePointerMessage.cs`：Win32 原生 Pointer 消息快照。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Bridge.cs`：Vulkan NativeHost 桥接分部。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Dpi.cs`：Vulkan NativeHost DPI 辅助分部。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.LayoutSync.cs`：Vulkan NativeHost 布局同步分部。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Log.cs`：Vulkan NativeHost 日志转发分部。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Picking.cs`：Vulkan NativeHost 拾取接线分部；负责把当前 Bounds、DPI、物理尺寸和 ViewportRevision 送入 UiVm，不执行 Selection 或 Vulkan 修改。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Gizmo.cs`：NativeHost Gizmo 命中入口；复用 Picking 的 ViewportState 捕获并调用 UiVm，命中时阻断 Scene Picking，不拥有 Capture。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs`：Vulkan NativeHost Pointer 输入分部。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`：Vulkan NativeHost 主体。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml`：Vulkan 视口控件界面。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml.cs`：Vulkan 视口控件代码后置。
- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.Input.cs`：Win32 子窗口输入路由分部。
- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs`：Win32 子窗口宿主主体。
- `XuanYu.Editor.UI/Vm/DebugText.cs`：右侧调试页状态快照示例数据。
- `XuanYu.Editor.UI/Vm/EditorLogCategory.cs`：编辑器日志分类枚举。
- `XuanYu.Editor.UI/Vm/EditorLogLevel.cs`：编辑器日志等级枚举。
- `XuanYu.Editor.UI/Vm/EditorLogSource.cs`：编辑器日志来源枚举。
- `XuanYu.Editor.UI/Vm/EditorTreeNode.cs`：编辑器树节点 UI 投影模型；按图标身份提供显示分类，不把实体显示类型当作 Selection 或场景事实。
- `XuanYu.Editor.UI/Vm/LogEntry.cs`：编辑器日志条目模型。
- `XuanYu.Editor.UI/Vm/SampleLogEntries.cs`：底部日志栏示例数据。
- `XuanYu.Editor.UI/Vm/UiText.cs`：静态中文 UI 文案与树节点投影数据；真实场景节点使用稳定 EntityKey，不拥有 Selection 状态，也不依赖 Vulkan。
- `XuanYu.Editor.UI/Vm/UiVm.Interaction.cs`：UiVm 交互事务入口分部。
- `XuanYu.Editor.UI/Vm/UiVm.InteractionPointer.cs`：UiVm Pointer 交互转换分部。
- `XuanYu.Editor.UI/Vm/UiVm.Logging.cs`：UiVm 日志绑定与日志入口分部。
- `XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs`：UiVm NativeHost 生命周期日志分部。
- `XuanYu.Editor.UI/Vm/UiVm.Picking.cs`：UiVm 视口拾取分部；负责构造 Picking 请求、调用 Core 服务、写低频日志并把结果交给既有 Selection 命令链，不直接修改 Tree、Inspector 或 Vulkan。
- `XuanYu.Editor.UI/Vm/UiVm.MoveGizmo.cs`：Selection 到 Move Gizmo 精确/Guard Hit 与 Capture 的适配分部；命中后提交既有 Interaction Begin 并阻断 Scene Picking，不修改 Transform、SpatialIndex 或 Undo。
- `XuanYu.Editor.UI/Vm/UiVm.Scene.cs`：UiVm 场景命令分部，提交 R1 测试实体 Position 并刷新调试对象信息。
- `XuanYu.Editor.UI/Vm/UiVm.Selection.cs`：UiVm Selection 命令适配与 Snapshot 投影分部；把视口或树入口统一提交给 EditorStateOwner，再同步 Tree 和 Inspector 通知，不持有第二份 Selection 真相。
- `XuanYu.Editor.UI/Vm/UiVm.Tool.cs`：UiVm 工具切换分部。
- `XuanYu.Editor.UI/Vm/UiVm.ViewportSelection.cs`：视口 Picking 到既有 Selection 命令的适配分部；校验命中实体并选择或清空，不持有状态、不直接操作 Tree/Inspector。
- `XuanYu.Editor.UI/Vm/UiVm.cs`：UiVm 主体与 UI 绑定状态。
- `XuanYu.Editor.UI/Vm/ViewportPickingLogFormatter.cs`：视口拾取日志格式化器；负责生成 R2-F 中文摘要和详情文本，不持有状态。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogBuffer.cs`：编辑器内存日志缓冲区。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogBus.cs`：编辑器低频日志入口。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogClipboardText.cs`：日志复制文本格式化器。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogFilter.cs`：日志过滤枚举与中文映射。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogFilterQuery.cs`：日志过滤匹配规则。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogRepeatKey.cs`：重复日志折叠键。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogSummary.cs`：日志摘要统计。
- `XuanYu.Editor.UI/Win/UiWin.axaml`：Avalonia 主窗口界面定义。
- `XuanYu.Editor.UI/Win/UiWin.axaml.cs`：Avalonia 主窗口代码后置。
