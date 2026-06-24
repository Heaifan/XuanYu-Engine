# 项目文件树 — XuanYu Engine

玄域引擎（原名 FluidWarfare）。每个 .cs / .axaml 文件均附功能说明。

```
XuanYu.Engine/                              ← 仓库根目录
│
├── changelog.md
├── file-tree.md
├── XuanYu.Engine.sln

├── docs/
│   ├── AI_DEVELOPMENT_RULES.md
│   ├── CODE_CONSTITUTION.md
│   ├── ENGINE_ARCHITECTURE.md
│   ├── LEGACY_FLUIDWARFARE_OLD_AUDIT.md
│   ├── MILESTONE1_PUBLIC_VALIDATION.md
│   ├── NAMING_RULES.md
│   ├── PHASE1_SCOPE.md
│   ├── PROJECT_CHARTER.md
│   └── naming-XuanYu-Engine.md
│   ├── audit-inspector-transform-9.0C-0.md  # 9.0C-0 审计：Inspector / Selection / WorldState / Viewport 链路
│   ├── diagnostic-safety.md  # 诊断日志与 UI 调度安全规范
│   ├── plan-9.0D-move-gizmo-final.md  # 9.0D — Move Gizmo 最终验收开发计划

├── tools/shaders/
│   ├── compile_basic_3d.ps1
│   ├── validate_basic_3d.ps1
│   └── embed_basic_3d_shaders.ps1

├── GameProjects/SampleProject/
│   └── game.project.json
├── XuanYu.Engine.Core/
│   ├── Identity/
│   │   └── EntityId.cs  # struct 类
│   ├── Logging/
│   │   ├── EngineLogEntry.cs  # struct 类
│   │   └── EngineLogLevel.cs  # EngineLogLevelExtensions 类
│   ├── Math/
│   │   ├── Vector3d.cs  # struct 类
│   │   └── YawRotation.cs  # struct 类
│   ├── Results/
│   │   ├── EngineError.cs  # struct 类
│   │   └── EngineResult.cs  # struct 类
│   └── Time/
│       ├── SimulationTime.cs  # struct 类
│       └── TimeStep.cs  # struct 类
├── XuanYu.Engine/
│   ├── Components/
│   │   ├── DisplayNameComponent.cs  # 实体显示名组件，保存用于 Editor 显示的名称。
│   │   └── PositionComponent.cs  # 实体位置组件，包装 Vector3d。
│   └── World/
│       ├── EntityPosition/
│       │   ├── WorldEntityPositionChange.cs  # World Entity 位置变更记录。
│       │   ├── WorldEntityPositionWriter.cs  # 对 WorldState 执行位置修改。检查 EntityId 存在性和位置是否实际变化。
│       │   └── WorldEntityPositionWriteResult.cs  # World Entity 位置写入结果。
│       ├── ProjectContentEntitySource.cs  # 保存 World 实体的项目内容来源路径与内容类型。
│       ├── WorldEntityInfo.cs  # 表示 World 中一个实体的最小可显示信息。
│       └── WorldState.cs  # WorldState — <summary>最小世界状态，支持创建、查询和枚举实体。不读项目文件，不写日志。</summary>
├── XuanYu.Engine.Project/
│   ├── Content/
│   │   ├── GameContentFileInfo.cs  # 表示项目中一个合法内容文件入口。
│   │   ├── GameContentFileScanner.cs  # GameContentFileScanner — <summary>根据 GameContentFolderInfo 扫描项目内容目录的一级内容文件入口。校验扩展名...
│   │   ├── GameContentFileScanResult.cs  # 表示内容文件入口扫描结果，包含合法内容文件入口和扫描中的校验问题。
│   │   └── GameContentFolderInfo.cs  # GameContentFolderInfo 类
│   ├── Loading/
│   │   ├── GameProjectExtensionParser.cs  # GameProjectExtensionParser — <summary>解析 allowedExtensions JSON 数组 + 格式校验。供 GameProjec...
│   │   ├── GameProjectFolderParser.cs  # GameProjectFolderParser 类
│   │   ├── GameProjectLoader.cs  # 项目加载主入口。委托 ManifestReader 加载清单，FolderParser 解析目录，
│   │   ├── GameProjectLoadResult.cs  # GameProjectLoadResult 类
│   │   └── GameProjectManifestReader.cs  # GameProjectManifestReader — <summary>读取并验证 game.project.json 文件。供 GameProjectLoader 内...
│   ├── Metadata/
│   │   └── GameProjectInfo.cs  # GameProjectInfo 类
│   ├── Paths/
│   │   └── SampleProjectPath.cs  # SampleProjectPath 类
│   ├── Validation/
│   │   ├── ProjectValidationIssue.cs  # 表示项目校验中的一个问题。
│   │   └── ProjectValidationReport.cs  # 表示一次项目校验报告，汇总项目加载与内容扫描中的校验问题。
'│   └── World/'
'│       ├── Documents/'
'│       │   ├── TransformComponentDocument.cs  # Transform 组件文档（Position/RotationDegrees/Scale）。'
'│       │   ├── WorldDocument.cs  # World 文档根节点。'
'│       │   ├── WorldEntityDocument.cs  # World 文件中一个实体的记录。'
'│       │   ├── WorldMetadataDocument.cs  # World 文件元数据。'
'│       │   └── WorldVector3Document.cs  # 三维坐标文档模型。'
'│       ├── SaveLoad/'
'│       │   ├── WorldDocumentJsonOptions.cs  # World 文档 JSON 序列化共享选项。'
'│       │   ├── WorldDocumentReadResult.cs  # World 文档读取结果。'
'│       │   ├── WorldDocumentReader.cs  # 从 .world.json 读取 WorldDocument。'
'│       │   ├── WorldDocumentWriteResult.cs  # World 文档写入结果。'
'│       │   └── WorldDocumentWriter.cs  # 将 WorldDocument 保存为 .world.json。'
'│       ├── Transform/'
'│       │   ├── SceneTransform.cs  # 实体 Transform 单一真源。渲染、Picking、Gizmo、检查器全部从此派生。'
'│       │   ├── SceneTransformDefaults.cs  # SceneTransform 默认值和工厂方法。'
'│       │   ├── SceneTransformMatrix.cs  # SceneTransform ↔ 变换矩阵的转换入口。'
'│       │   └── SceneTransformValidation.cs  # SceneTransform 各分量的有效性校验。'
'│       └── Validation/'
'│           ├── WorldDocumentValidator.cs  # World 文档校验器。'
'│           ├── WorldValidationError.cs  # World 校验错误。'
'│           └── WorldValidationReport.cs  # World 校验报告。
├── XuanYu.Engine.Bridge.ProjectEngine/
│   └── World/
│       ├── ProjectContentWorldSeeder.cs  # 把项目内容文件入口转换为 Engine World 的占位实体。
│       └── ProjectContentWorldSeedResult.cs  # 保存项目内容生成 World 占位实体的结果。
├── XuanYu.Engine.Render/
│   ├── Camera/
│   │   ├── Navigation/
│   │   │   ├── SceneNavigationCameraMotion.cs  # 标准视图方向的 Yaw/Pitch 计算及投影模式切换。
│   │   │   ├── SceneNavigationView.cs  # 标准视图方向标识。
│   │   │   ├── SceneOrthographicProjection.cs  # Vulkan 正交投影矩阵计算（Z-Up，深度范围 0..1）。
│   │   │   └── SceneProjectionMode.cs  # 相机投影模式。
│   │   ├── Orbit/
│   │   │   ├── SceneOrbitCameraMotion.cs  # Blender 风格轨道相机运动计算。
│   │   │   └── SceneOrbitCameraState.cs  # Blender 风格轨道相机状态（Z-Up）。
│   │   ├── SceneCameraDefaults.cs  # 默认 Scene3D 相机配置（与当前 VulkanCameraInfo.DefaultBattlefield 等价的 Target+Distance 表示）。
│   │   ├── SceneCameraLimits.cs  # RTS 相机边界常量。
│   │   ├── SceneCameraMotion.cs  # RTS 相机运动计算。
│   │   ├── SceneCameraPose.cs  # 完整的相机姿态 — 渲染、Picking、Ground Hover 的唯一真源。
│   │   └── SceneCameraState.cs  # RTS 战场相机状态。
│   ├── Coordinates/
│   │   ├── WorldCoordinateConvention.cs  # XuanYu Engine 世界坐标宪法。
│   │   └── YUpToZUpPosition.cs  # 旧 Y-Up 到 Z-Up 的一次性迁移工具。
│   ├── Scene/
│   │   ├── Position/
│   │   │   ├── RenderObjectPositionChange.cs  # RenderObject 位置变更记录。
│   │   │   ├── RenderObjectPositionWriteResult.cs  # RenderScene 位置写入结果。
│   │   │   └── RenderSceneObjectPositionWriter.cs  # 替换 RenderScene 中指定 EntityId 的 Position 和 SelectionBounds。
│   │   ├── RenderObjectInfo.cs  # 表示一个可被渲染后端消费的最小对象。
│   │   ├── RenderObjectVisualKind.cs  # 渲染对象视觉类型枚举。
│   │   ├── RenderScene.cs  # 保存一帧或当前状态下的可渲染对象集合。
│   │   └── RenderUnitPlacement.cs  # 单位渲染位置与 Picking 包围盒的单一真源。
│   ├── Selection/
│   │   ├── Ground/
│   │   │   ├── SceneGroundHit.cs  # 地面射线求交结果。未命中时 IsHit = false。
│   │   │   ├── SceneGroundPlane.cs  # 水平地面平面定义（Z-Up）。
│   │   │   └── SceneRayGroundIntersection.cs  # 水平地面（Z-Up，法线 +Z）射线求交。
│   │   ├── Pointer/
│   │   │   ├── ScenePointerPicker.cs  # 统一 Pointer Picking 调度器。
│   │   │   ├── ScenePointerPickKind.cs  # Pointer Picking 结果类型优先级：Entity > Ground > None。
│   │   │   └── ScenePointerPickResult.cs  # 统一 Pointer Picking 结果，包含实体命中、地面命中和未命中三种状态。
│   │   ├── Presented/
│   │   │   ├── PresentedEntityBounds.cs  # 已呈现帧中单个实体的包围盒快照。
│   │   │   ├── PresentedScenePickSnapshot.cs  # 已成功 Present 的场景拾取快照。
│   │   │   └── PresentedScenePickSnapshotBuilder.cs  # 从 RenderScene + CameraRevision 构建 PresentedScenePickSnapshot。
│   │   ├── Screen/
│   │   │   ├── ScreenBoundsProjection.cs  # 将世界空间 AABB 投影到屏幕空间，获取屏幕矩形。
│   │   │   ├── ScreenEntityPicker.cs  # 精确 Ray-AABB Miss 后的屏幕空间容错 Picking。
│   │   │   └── ScreenPickTolerance.cs  # 屏幕空间 Picking 容差参数。
│   │   ├── RenderScenePicker.cs  # 在 RenderScene 中执行 CPU Ray-AABB Picking。
│   │   ├── RenderScenePickResult.cs  # Picking 操作结果。NoHit 表示未选中任何对象。
│   │   ├── SceneAxisAlignedBounds.cs  # 轴对齐包围盒（AABB），用于 Picking 和渲染尺寸统一。
│   │   ├── SceneRay.cs  # 3D 空间射线，用于 Picking 和命中检测。
│   │   └── SceneRayBoundsIntersection.cs  # Slab 法射线与 AABB 相交检测。
│   ├── ViewportNavigation/
│   │   ├── Core/
│   │   │   ├── AxisProjection.cs  # AxisProjection — <summary>轴端在屏幕上的投影。</summary>
│   │   │   ├── ViewportNavigationAction.cs  # Overlay 交互触发的相机动作。
│   │   │   ├── ViewportNavigationDragMode.cs  # （待补充）
│   │   │   ├── ViewportNavigationElement.cs  # 可交互的导航元素标识。
│   │   │   └── ViewportNavigationTypes.cs  # struct 类
│   │   ├── Interaction/
│   │   │   ├── ViewportNavigationHitTest.cs  # ViewportNavigationLayout 的命中测试和动作映射。
│   │   │   └── ViewportNavigationPressResult.cs  # 视口导航 Overlay 对左键按下的处理结果。
│   │   └── Layout/
│   │       ├── ViewportNavigationLayout.cs  # 视口导航 Overlay 布局计算结果。
│   │       └── ViewportNavigationLayoutCompute.cs  # ViewportNavigationLayoutCompute — <summary>ViewportNavigationLayout 的工厂方法。</summary>
│   └── World/
│       └── WorldToRenderSceneBuilder.cs  # 把 Engine.WorldState 转换为 RenderScene (Z-Up)。
├── XuanYu.Engine.Render.Vulkan/
│   ├── Backend/
│   │   ├── VulkanBackendInfo.cs  # 保存 Vulkan 后端探测结果。
│   │   ├── VulkanBackendProbe.cs  # Vulkan 后端探测器。
│   │   └── VulkanBackendStatus.cs  # Vulkan 后端当前状态。
│   ├── Camera/
│   │   ├── Math/
│   │   │   ├── VulkanMatrixInvert.cs  # VulkanMatrixInvert — <summary>4x4 矩阵求逆（列优先 float[]）。使用高斯-约旦消元法。</summary>
│   │   │   ├── VulkanMatrixOperations.cs  # VulkanMatrixOperations — <summary>4x4 矩阵运算（列优先）。用于模型变换与矩阵乘法。</summary>
│   │   │   └── VulkanViewMatrix.cs  # VulkanViewMatrix — <summary>LookAt 矩阵计算（列优先 float[16]）。</summary>
│   │   ├── PresentedCameraSnapshot.cs  # 已成功 Present 的相机快照。
│   │   ├── SceneRayBuildStatus.cs  # 射线构建状态，区分技术失败和有效未命中。
│   │   ├── VulkanCameraInfo.cs  # 固定 3D 相机参数。
│   │   ├── VulkanCameraMatrices.cs  # VulkanCameraMatrices — <summary>3D 相机矩阵计算。提供 View、Projection 和 ViewProjection 矩阵...
│   │   └── VulkanSceneRayBuilder.cs  # VulkanSceneRayBuilder — <summary>从 Vulkan 视口像素坐标生成世界空间 SceneRay。使用已呈现帧的 ViewProje...
│   ├── Clear/
│   │   ├── Probe/
│   │   │   ├── Render/
│   │   │   │   ├── VulkanClearProbeRenderSubmitScope.cs  # VulkanClearProbeRenderSubmitScope 类
│   │   │   │   └── VulkanClearProbeRenderTargetScope.cs  # VulkanClearProbeRenderTargetScope 类
│   │   │   ├── VulkanClearProbe.cs  # VulkanClearProbe 类
│   │   │   ├── VulkanClearProbeContextScope.cs  # VulkanClearProbeContextScope 类
│   │   │   ├── VulkanClearProbeDeviceSelector.cs  # VulkanClearProbeDeviceSelector 类
│   │   │   └── VulkanClearProbeSurfaceQuery.cs  # VulkanClearProbeSurfaceQuery 类
│   │   ├── VulkanClearInfo.cs  # Vulkan 最小清屏结果模型。
│   │   └── VulkanClearStatus.cs  # Vulkan 最小清屏探测状态。
│   ├── Context/
│   │   ├── Legacy/
│   │   │   └── VulkanRenderContextLegacy.cs  # 死代码仓。TryCreateDeviceResources 当前硬编码 return false。
│   │   ├── VulkanRenderContext.cs  # VulkanRenderContext 类
│   │   ├── VulkanRenderContextSelector.cs  # VulkanRenderContextSelector 类
│   │   └── VulkanRenderContextSetup.cs  # VulkanRenderContextSetup 类
│   ├── Device/
│   │   ├── VulkanDeviceInfo.cs  # Vulkan PhysicalDevice / LogicalDevice 探测结果。
│   │   ├── VulkanDeviceInstanceScope.cs  # VulkanDeviceInstanceScope 类
│   │   ├── VulkanDeviceProbe.cs  # 创建临时 Vulkan Instance，选择支持 Graphics Queue 的物理设备，创建并释放 LogicalDevice。
│   │   ├── VulkanDeviceSelector.cs  # VulkanDeviceSelector 类
│   │   └── VulkanDeviceStatus.cs  # Vulkan LogicalDevice 创建探测状态。
│   ├── Instance/
│   │   ├── VulkanInstanceInfo.cs  # Vulkan Instance 创建探测结果。
│   │   ├── VulkanInstanceProbe.cs  # VulkanInstanceProbe 类
│   │   └── VulkanInstanceStatus.cs  # Vulkan Instance 创建探测状态。
│   ├── Scene3D/
│   │   ├── Commands/
│   │   │   ├── Core/
│   │   │   │   ├── VulkanScene3dCommandRecorder.cs  # VulkanScene3dCommandRecorder 类
│   │   │   │   └── VulkanScene3dCommandRenderPass.cs  # CommandBuffer Begin + RenderPass Begin/End 阶段。
│   │   │   └── Draw/
│   │   │       ├── VulkanScene3dCommandGrid.cs  # VulkanScene3dCommandRecorder 类
│   │   │       ├── VulkanScene3dCommandGroundCursor.cs  # VulkanScene3dCommandRecorder 类
│   │   │       ├── VulkanScene3dCommandOverlay.cs  # VulkanScene3dCommandRecorder 类
│   │   │       └── VulkanScene3dCommandUnits.cs  # VulkanScene3dCommandRecorder 类
│   │   ├── Depth/
│   │   │   ├── VulkanScene3dDepthAttachmentInfo.cs  # 深度附件查询与诊断信息。
│   │   │   ├── VulkanScene3dDepthAttachments.cs  # VulkanScene3dDepthAttachments 类
│   │   │   └── VulkanScene3dDepthFormatSelector.cs  # 查询物理设备支持的 Scene3D 深度格式。
│   │   ├── GroundCursor/
│   │   │   ├── VulkanGroundCursorGeometry.cs  # Ground Cursor 几何数据（Z-Up / XY 地面）。
│   │   │   ├── VulkanGroundCursorInfo.cs  # Ground Cursor 诊断信息。
│   │   │   └── VulkanGroundCursorState.cs  # Ground Cursor 运行时状态（可见性 + 世界坐标）。
│   │   ├── Overlay/
│   │   │   ├── Geometry/
│   │   │   │   ├── VulkanNavigationOverlayGeometry.cs  # VulkanNavigationOverlayGeometry — <summary>将 ViewportNavigationLayout 转换为 Overlay 三角形顶点。Bui...
│   │   │   │   ├── VulkanNavigationOverlayInfo.cs  # VulkanNavigationOverlayInfo — <summary>Overlay 诊断信息。</summary>
│   │   │   │   ├── VulkanNavigationOverlayPrimitives.cs  # VulkanNavigationOverlayGeometry — <summary>Overlay 绘图图元：填充圆、圆环、粗线、轴端圆。</summary>
│   │   │   │   ├── VulkanNavigationOverlayShapes.cs  # VulkanNavigationOverlayGeometry — <summary>Overlay Shape 绘制：字母标签 + 导航按钮。</summary>
│   │   │   │   └── VulkanOverlayVertex.cs  # VulkanOverlayVertex 类
│   │   │   ├── Render/
│   │   │   │   ├── PresentedNavigationOverlaySnapshot.cs  # PresentedNavigationOverlaySnapshot 类
│   │   │   │   └── VulkanOverlayCommandRecorder.cs  # VulkanOverlayCommandRecorder 类
│   │   │   └── Resources/
│   │   │       ├── VulkanOverlayPipeline.cs  # VulkanOverlayPipeline 类
│   │   │       ├── VulkanOverlayPipelineLayout.cs  # VulkanOverlayPipelineLayout 类
│   │   │       ├── VulkanOverlayResources.Create.cs  # VulkanOverlayResources 类
│   │   │       └── VulkanOverlayResources.cs  # VulkanOverlayResources 类
│   │   ├── Pipeline/
│   │   │   ├── VulkanScene3dPipelineCreate.cs  # GraphicsPipeline 创建辅助：状态构建 + CreateGraphicsPipelines 调用。
│   │   │   ├── VulkanScene3dPipelineLayout.cs  # 创建 Scene3D PipelineLayout 与 PushConstantRange。
│   │   │   ├── VulkanScene3dPipelines.cs  # Grid (LineList) + Unit (TriangleList) Graphics Pipeline 编排器。
│   │   │   ├── VulkanScene3dPushConstants.cs  # Push Constant 数据布局：MVP (mat4, 64 字节) + Tint (vec4, 16 字节) = 80 字节。
│   │   │   └── VulkanScene3dShaderModules.cs  # 使用 CompiledShaders 中已验证的 SPIR-V 字节创建 Vertex / Fragment ShaderModule。
│   │   ├── Render/
│   │   │   ├── Probe/
│   │   │   │   ├── Core/
│   │   │   │   │   └── VulkanScene3dRenderer.cs  # VulkanScene3dRenderer 类
│   │   │   │   ├── Create/
│   │   │   │   │   ├── VulkanScene3dRendererProbeDevice.cs  # VulkanScene3dRenderer 类
│   │   │   │   │   ├── VulkanScene3dRendererProbeInstance.cs  # VulkanScene3dRenderer 类
│   │   │   │   │   ├── VulkanScene3dRendererProbeResources.cs  # VulkanScene3dRenderer 类
│   │   │   │   │   ├── VulkanScene3dRendererProbeSurfaceCreate.cs  # VulkanScene3dRenderer 类
│   │   │   │   │   └── VulkanScene3dRendererProbeSwapchain.cs  # VulkanScene3dRenderer 类
│   │   │   │   ├── Frame/
│   │   │   │   │   ├── VulkanScene3dRendererProbeAcquire.cs  # VulkanScene3dRenderer 类
│   │   │   │   │   ├── VulkanScene3dRendererProbeFrame.cs  # VulkanScene3dRenderer 类
│   │   │   │   │   ├── VulkanScene3dRendererProbeMVP.cs  # VulkanScene3dRenderer 类
│   │   │   │   │   ├── VulkanScene3dRendererProbePresent.cs  # VulkanScene3dRenderer 类
│   │   │   │   │   └── VulkanScene3dRendererProbeSubmit.cs  # VulkanScene3dRenderer 类
│   │   │   │   ├── Surface/
│   │   │   │   │   └── VulkanScene3dRendererProbeSurfaceChoice.cs  # VulkanScene3dRenderer 类
│   │   │   │   └── VulkanScene3dRendererSetup.cs  # VulkanScene3dRenderer 类
│   │   │   ├── Resources/
│   │   │   │   ├── VulkanScene3dDepthResource.cs  # VulkanScene3dRenderResources 类
│   │   │   │   ├── VulkanScene3dRenderResources.cs  # VulkanScene3dRenderResources 类
│   │   │   │   └── VulkanScene3dResourceRelease.cs  # VulkanScene3dRenderResources 类
│   │   │   ├── VulkanScene3dInfo.cs  # Vulkan 3D 场景渲染结果模型。
│   │   │   ├── VulkanScene3dRunGate.cs  # Scene3D 运行闸门，控制 3D 管线是否允许启动。
│   │   │   └── VulkanScene3dStatus.cs  # Vulkan 3D 场景渲染状态。
│   │   ├── Session/
│   │   │   ├── Core/
│   │   │   │   ├── VulkanScene3dSessionCoreState.cs  # Vulkan 核心句柄 + 函数指针 + 帧常量。
│   │   │   │   ├── VulkanScene3dSessionOverlayState.cs  # Overlay + Gizmo 相关字段。
│   │   │   │   ├── VulkanScene3dSessionProcLoad.cs  # Vulkan 函数指针加载辅助方法。
│   │   │   │   ├── VulkanScene3dSessionRenderState.cs  # Swapchain + 运行时状态 + 验证相关字段。
│   │   │   │   └── VulkanScene3dSessionResourceFlags.cs  # 会话级资源创建标记。
│   │   │   ├── Dispose/
│   │   │   │   ├── Core/
│   │   │   │   │   └── VulkanScene3dCoreDispose.cs  # Device / Surface / DebugMessenger / Instance 释放步骤。
│   │   │   │   ├── Render/
│   │   │   │   │   ├── VulkanScene3dBufferDispose.cs  # Vertex Buffer + DeviceMemory 释放步骤。
│   │   │   │   │   ├── VulkanScene3dOverlayDispose.cs  # Overlay 资源 + 帧状态释放步骤。
│   │   │   │   │   ├── VulkanScene3dPipelineDispose.cs  # Pipeline + PipelineLayout 释放步骤。
│   │   │   │   │   └── VulkanScene3dShaderDispose.cs  # ShaderModule 释放步骤。
│   │   │   │   ├── State/
│   │   │   │   │   ├── VulkanScene3dSessionDisposeState.cs  # Dispose 状态跟踪：资源创建标记的查询与重置。
│   │   │   │   │   └── VulkanScene3dSessionDisposeTrace.cs  # Dispose 诊断日志。
│   │   │   │   ├── VulkanScene3dSessionDisposeResources.cs  # DisposeResources 释放顺序编排器（12 步）。
│   │   │   │   └── VulkanScene3dSessionDisposeSession.cs  # DisposeSessionResources 释放顺序编排器（13 步）。
│   │   │   ├── FrameFlow/
│   │   │   │   ├── VulkanScene3dFrameAcquire.cs  # AcquireNextImageKHR 调用及结果分类。
│   │   │   │   ├── VulkanScene3dFrameFailure.cs  # FailFrame 文案构造。
│   │   │   │   ├── VulkanScene3dFramePresent.cs  # QueuePresentKHR 调用及结果分类。
│   │   │   │   └── VulkanScene3dFrameSubmit.cs  # QueueSubmit 调用。
│   │   │   ├── FrameModel/
│   │   │   │   ├── VulkanScene3dFrameReason.cs  # （待补充）
│   │   │   │   ├── VulkanScene3dFrameResult.cs  # 单帧渲染的结构化结果。
│   │   │   │   └── VulkanScene3dFrameStatus.cs  # 单帧渲染结果状态。
│   │   │   ├── Handles/
│   │   │   │   ├── VulkanScene3dCoreHandles.cs  # VulkanScene3dCoreHandles — <summary>Session 级核心 Vulkan 句柄集合。Instance / Device / Surf...
│   │   │   │   ├── VulkanScene3dFrameHandles.cs  # VulkanScene3dFrameHandles — <summary>帧级资源句柄（Shader / Pipeline / Buffer / Overlay）。</s...
│   │   │   │   └── VulkanScene3dSwapchainHandles.cs  # VulkanScene3dSwapchainHandles — <summary>Swapchain 级句柄与函数指针集合。</summary>
│   │   │   ├── Lifecycle/
│   │   │   │   └── VulkanScene3dSessionState.cs  # VulkanScene3dSessionState — <summary>VulkanScene3dSession 运行时状态数据（状态/计数器/标记）。</summary>
│   │   │   ├── Render/
│   │   │   │   ├── VulkanScene3dRenderFrame.cs  # VulkanScene3dSession 类
│   │   │   │   ├── VulkanScene3dRenderFrameInternal.cs  # RenderFrameInternal 主体编排：Fence → Acquire → Reset → Compute → Build → Record → Submit → Present。
│   │   │   │   ├── VulkanScene3dRenderFrameSnapshot.cs  # 帧结果构建：相机快照 + Overlay 快照 + FrameResult 构造。
│   │   │   │   ├── VulkanScene3dRenderResize.cs  # Resize 编排器：检查 → DeviceWaitIdle → 保存旧资源 → 创建新资源
│   │   │   │   └── VulkanScene3dRenderResizeAtomic.cs  # Resize 事务：创建新 Pipeline/Overlay → 原子切换 → 释放旧资源。
│   │   │   ├── Start/
│   │   │   │   ├── VulkanScene3dSessionCreateDevice.cs  # PhysicalDevice 选择 + LogicalDevice 创建。
│   │   │   │   ├── VulkanScene3dSessionCreateInstance.cs  # Instance 创建（含 Debug Messenger 初始化）。
│   │   │   │   ├── VulkanScene3dSessionCreateResources.cs  # Shader / PipelineLayout / VertexBuffer / GroundCursor 创建步骤。
│   │   │   │   ├── VulkanScene3dSessionCreateSurface.cs  # Surface 创建 + 函数指针加载。
│   │   │   │   └── VulkanScene3dSessionStart.cs  # VulkanScene3dSession 类
│   │   │   ├── Surface/
│   │   │   │   ├── VulkanScene3dPresentModes.cs  # 两阶段枚举 PresentModeKHR，处理 Success / Incomplete，有限重试。
│   │   │   │   └── VulkanScene3dSurfaceFormats.cs  # 两阶段枚举 SurfaceFormatKHR，处理 Success / Incomplete，有限重试。
│   │   │   ├── Swapchain/
│   │   │   │   ├── Choice/
│   │   │   │   │   ├── VulkanScene3dSwapchainExtent.cs  # VulkanScene3dSwapchainExtent — <summary>Swapchain Extent 计算。</summary>
│   │   │   │   │   └── VulkanScene3dSwapchainSelection.cs  # VulkanScene3dSwapchainSelection — <summary>Swapchain 表面格式与 PresentMode 选择。</summary>
│   │   │   │   ├── Create/
│   │   │   │   │   └── VulkanScene3dSwapchainCreateFlow.cs  # VulkanScene3dSwapchainCreateFlow 类
│   │   │   │   ├── Images/
│   │   │   │   │   ├── VulkanScene3dSwapchainFramebuffers.cs  # VulkanScene3dSwapchainFramebuffers 类
│   │   │   │   │   └── VulkanScene3dSwapchainImageViews.cs  # VulkanScene3dSwapchainImageViews 类
│   │   │   │   ├── Lifecycle/
│   │   │   │   │   └── VulkanScene3dSwapchainDispose.cs  # VulkanScene3dSwapchainDispose 类
│   │   │   │   ├── Resources/
│   │   │   │   │   └── VulkanScene3dSwapchainResources.cs  # VulkanScene3dSwapchainResources 类
│   │   │   │   ├── Sync/
│   │   │   │   │   └── VulkanScene3dSwapchainSync.cs  # VulkanScene3dSwapchainSync 类
│   │   │   │   ├── VulkanScene3dSwapchainCreateResult.cs  # Swapchain 创建/重建的结构化结果。
│   │   │   │   ├── VulkanScene3dSwapchainFunctions.cs  # 不可变的 Swapchain 函数集合。
│   │   │   │   ├── VulkanScene3dSwapchainInvariant.cs  # Swapchain 生命周期不变量检查。
│   │   │   │   └── VulkanScene3dSwapchainStage.cs  # Swapchain 创建/重建失败阶段，用于精确诊断。
│   │   │   ├── VulkanScene3dSession.cs  # 持久 Scene3D 渲染会话。
│   │   │   ├── VulkanScene3dSession.Frame.cs  # VulkanScene3dSession 类
│   │   │   └── VulkanScene3dSession.Properties.cs  # VulkanScene3dSession 类
│   │   └── Vertex/
│   │       ├── Buffer/
│   │       │   └── VulkanScene3dVertexBufferCreate.cs  # 单个 Vertex Buffer 创建：CreateBuffer → AllocateMemory → Bind → Map → Copy → Unmap。
│   │       ├── VulkanScene3dVertex.cs  # struct 类
│   │       ├── VulkanScene3dVertexBuffers.cs  # Vertex Buffer 创建编排器。
│   │       ├── VulkanScene3dVertexCube.cs  # VulkanScene3dVertices — <summary>立方体顶点生成（TriangleList）。</summary>
│   │       ├── VulkanScene3dVertexGrid.cs  # VulkanScene3dVertices — <summary>Grid / Axis 顶点生成 + 交错格式转换。</summary>
│   │       └── VulkanSceneAxisGeometry.cs  # 世界 X/Y/Z 主轴几何数据生成（Z-Up）。
│   ├── Shaders/
│   │   └── CompiledShaders.cs  # CompiledShaders 类
│   ├── Surface/
│   │   ├── VulkanSurfaceInfo.cs  # Vulkan Surface 创建探测结果。
│   │   ├── VulkanSurfaceInstanceScope.cs  # VulkanSurfaceInstanceScope 类
│   │   ├── VulkanSurfaceProbe.cs  # 使用外部传入的 Windows 原生句柄创建并立即释放 Vulkan Surface。
│   │   └── VulkanSurfaceStatus.cs  # Vulkan Surface 创建探测状态。
│   ├── Swapchain/
│   │   ├── Probe/
│   │   │   ├── VulkanSwapchainProbe.cs  # 使用临时 Instance + Surface + Device 创建并立即释放 Swapchain。
│   │   │   ├── VulkanSwapchainProbeContextScope.cs  # VulkanSwapchainProbeContextScope 类
│   │   │   ├── VulkanSwapchainProbeDeviceSelector.cs  # VulkanSwapchainProbeDeviceSelector 类
│   │   │   └── VulkanSwapchainProbeSurfaceQuery.cs  # VulkanSwapchainProbeSurfaceQuery 类
│   │   ├── VulkanSwapchainInfo.cs  # Vulkan Swapchain 创建结果模型。
│   │   └── VulkanSwapchainStatus.cs  # Vulkan Swapchain 创建探测状态。
│   └── Validation/
│       ├── Info/
│       │   ├── VulkanValidationInfo.cs  # Vulkan Validation 状态信息。
│       │   ├── VulkanValidationMessageInfo.cs  # 一条 Vulkan Validation 消息。
│       │   ├── VulkanValidationMessageStore.cs  # 保存最近 N 条 Vulkan Validation 消息。
│       │   ├── VulkanValidationOptions.cs  # 从环境变量读取是否请求启用 Vulkan Validation Layer。
│       │   └── VulkanValidationStatus.cs  # Vulkan Validation Layer 启用状态。
│       ├── VulkanDebugMessengerScope.cs  # VulkanDebugMessengerScope 类
│       └── VulkanValidationAvailabilityProbe.cs  # VulkanValidationAvailabilityProbe 类
├── XuanYu.Engine.Editor/
│   ├── EntityTransform/
│   │   ├── EditorEntityTransformChange.cs  # 一次正式实体位置修改。
│   │   ├── EditorEntityTransformDraft.cs  # 检查器当前 Transform 输入草稿。
│   │   ├── EditorEntityTransformValidation.cs  # Transform 输入校验。
│   │   ├── EditorGroundPlacementState.cs  # 一次性地面放置模式状态。
│   │   └── EditorWorldDirtyState.cs  # 场景修改状态跟踪。只提供状态反馈，不实现磁盘保存。
│   ├── Input/
│   │   ├── Actions/
│   │   │   ├── EditorInputActionCatalog.cs  # EditorInputActionCatalog — <summary>全部编辑器动作声明与 Blender 默认绑定。</summary>
│   │   │   ├── EditorInputActionContext.cs  # 输入上下文，决定动作的生效范围。
│   │   │   ├── EditorInputActionDefinition.cs  # 编辑器动作声明。动作 ID 是稳定字符串标识符，不依赖语言或按键。
│   │   │   └── EditorInputValueKind.cs  # 动作输入值的种类。
│   │   ├── Bindings/
│   │   │   ├── EditorInputBinding.cs  # 一个动作的绑定声明：主手势和可选手势。
│   │   │   ├── EditorInputBindingSet.cs  # 完整的绑定集合：包含预设名和覆盖项。
│   │   │   ├── EditorInputConflictDetector.cs  # 绑定冲突检测。
│   │   │   ├── EditorInputGesture.cs  # 平台无关输入手势。
│   │   │   └── Win32KeyCodeMapper.cs  # Win32 虚拟键码 (VK_*) 到抽象手势代码的静态映射。
│   │   ├── Runtime/
│   │   │   ├── EditorInputBindingSnapshot.Build.cs  # EditorInputBindingSnapshot — <summary>Partial：快照构建逻辑。</summary>
│   │   │   ├── EditorInputBindingSnapshot.cs  # EditorInputBindingSnapshot — <summary>运行时绑定快照，O(1) 手势→动作查找。设置变更后重建并原子替换。</summary>
│   │   │   ├── EditorInputEvent.cs  # 标准化的输入事件，由平台适配层产生。
│   │   │   ├── EditorInputMatch.cs  # 输入事件匹配到的动作结果。
│   │   │   └── EditorInputService.cs  # 单例服务，管理运行时绑定快照的生命周期和热更新。
│   │   └── Settings/
│   │       ├── EditorSettingsDocument.cs  # 编辑器设置文档结构。
│   │       ├── EditorSettingsPath.cs  # 编辑器设置文件路径与文件夹名常量。
│   │       ├── EditorSettingsPathMigration.cs  # 将旧 %APPDATA%/FluidWarfare 下的设置文件迁移到 %APPDATA%/XuanYuEngine。
│   │       ├── EditorSettingsReader.cs  # 读取编辑器设置文件。失败时回退默认。
│   │       └── EditorSettingsWriter.cs  # 原子方式保存编辑器设置文件。
│   ├── ProjectContentTree/
│   │   ├── ProjectContentTree.cs  # 项目内容树的只读快照。
│   │   ├── ProjectContentTreeBuilder.cs  # 从 GameProjectInfo 构建 ProjectContentTree。
│   │   ├── ProjectContentTreeNode.cs  # 项目内容树节点。
│   │   ├── ProjectContentTreeNodeKind.cs  # （待补充）
│   │   └── ProjectContentTreeSearch.cs  # 在项目内容树中搜索匹配节点。
│   ├── Selection/
│   │   ├── EditorEntitySelectionChange.cs  # 一次选择状态提交的结果。IsChanged==false 表示相同 EntityId 幂等 NoOp。
│   │   ├── EditorEntitySelectionOrigin.cs  # 选择命令的真实来源。程序同步界面不是选择命令，不应从此 Origin 进入。
│   │   ├── EditorEntitySelectionState.cs  # 唯一选择状态——SelectedEntityId 是实体选择的唯一真源。
│   │   └── EditorSelectionDiagnostics.cs  # 选择系统诊断计数器，用于验证反馈循环和数据流正确性。
│   ├── Transform/
│   │   ├── Data/
│   │   │   └── EntitySceneTransformAccess.cs  # WorldState ↔ SceneTransform 的读写入口。
│   │   ├── Edit/
│   │   │   ├── TransformEditKind.cs  # （待补充）
│   │   │   ├── TransformEditResult.cs  # 编辑事务完成结果。Confirm 或 Cancel 时由 Transaction 发出。
│   │   │   ├── TransformEditSession.cs  # Transform 编辑事务状态机。
│   │   │   ├── TransformEditSessionStart.cs  # 从 Editor 上下文创建 TransformEditSession 的辅助方法。
│   │   │   └── TransformEditSnapshot.cs  # 编辑事务开始时的快照。用于 Cancel 时恢复。
│   │   ├── Scrub/
│   │   │   └── TransformAxisScrubState.cs  # Transform 数值拖拽状态机（平台无关）。
│   │   └── Translation/
│   │       ├── Axis/
│   │       │   ├── AxisScreenMetric.cs  # 计算轴在屏幕上的投影方向和每像素对应世界单位数。
│   │       │   ├── AxisTranslationAnchor.cs  # 轴向平移拖动锚点。一次拖动期间不可变。
│   │       │   ├── AxisTranslationMode.cs  # 轴向平移求解器的执行模式。
│   │       │   ├── AxisTranslationSolver.cs  # 轴向平移求解器。X/Y/Z 共用此 Solver，仅 Axis 不同。
│   │       │   └── AxisTranslationStart.cs  # 从相机快照和实体位置创建 AxisTranslationAnchor 的辅助方法。
│   │       ├── Constraint/
│   │       │   ├── TransformOrientation.cs  # 将约束方向从 Global 转换到当前 Orientation。
│   │       │   ├── TranslationAxis.cs  # TranslationAxisExtensions 类
│   │       │   ├── TranslationConstraint.cs  # 平移约束的两种形式：轴向约束或平面约束。
│   │       │   ├── TranslationConstraintText.cs  # TranslationConstraintText 类
│   │       │   └── TranslationPlane.cs  # TranslationPlaneExtensions 类
│   │       └── Plane/
│   │           ├── PlaneTranslationAnchor.cs  # 平面平移拖动锚点。一次拖动期间不可变。
│   │           ├── PlaneTranslationMode.cs  # （待补充）
│   │           ├── PlaneTranslationSolver.cs  # 平面平移求解器。XY/XZ/YZ/View 平面共用。
│   │           └── PlaneTranslationStart.cs  # 从射线-平面交点创建 PlaneTranslationAnchor 的辅助方法。
│   ├── ViewportGround/
│   │   ├── EditorGroundPointerChange.cs  # 地面指针状态变更结果，用于判断是否需要刷新 UI 或提交 Scene3D 帧。
│   │   └── EditorGroundPointerState.cs  # 平台无关的地面指针状态。
│   └── WorldHierarchy/
│       ├── WorldHierarchyNode.cs  # 层级树中的一个节点。不可变创建，构建后 Children 集合不可修改。
│       ├── WorldHierarchyNodeKind.cs  # 层级节点类型。Root 和 Group 不可选择为实体，Entity 可选择。
│       ├── WorldHierarchySearch.cs  # 在层级树中搜索匹配节点。
│       ├── WorldHierarchyTree.cs  # World 层级树的只读快照。
│       └── WorldHierarchyTreeBuilder.cs  # WorldHierarchyTreeBuilder — <summary>从 WorldState 构建 WorldHierarchyTree。排序稳定：分组按语义顺序，...
├── XuanYu.Engine.Editor.Windows/
│   ├── About/
│   │   ├── AboutXuanYuEngineWindow.axaml  # XuanYu.Engine.Editor.Windows.About.AboutXuanYuEngineWindow
│   │   └── AboutXuanYuEngineWindow.axaml.cs  # AboutXuanYuEngineWindow 类
│   ├── Panels/
│   │   ├── DebugDock/
│   │   │   ├── DebugDockPanel.axaml  # XuanYu.Engine.Editor.Windows.Panels.DebugDock.DebugDockPanel
│   │   │   ├── DebugDockPanel.axaml.cs  # DebugDockPanel — <summary>底部调试终端，包含日志、渲染诊断、RenderScene 和性能页签。</summary>
│   │   │   ├── DebugDockPanel.Diagnostics.cs  # DebugDockPanel — <summary>Partial：渲染诊断文本更新。</summary>
│   │   │   ├── DebugDockPanel.Performance.cs  # DebugDockPanel — <summary>Partial：性能计时更新。</summary>
│   │   │   └── DebugDockPanel.RenderScene.cs  # DebugDockPanel — <summary>Partial：RenderScene 列表更新。</summary>
│   │   ├── HierarchyVisual/
│   │   │   ├── HierarchyBranchCanvas.cs  # 绘制经典文件树的连续虚线树干。
│   │   │   ├── HierarchyBranchInfo.cs  # 一个可见节点的树干位置。
│   │   │   ├── HierarchyNodeRow.axaml  # XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual.HierarchyNodeRow
│   │   │   ├── HierarchyNodeRow.axaml.cs  # HierarchyNodeRow 类
│   │   │   └── HierarchyNodeViewContract.cs  # IHierarchyNodeView 类
│   │   ├── Inspector/
│   │   │   ├── Transform/
│   │   │   │   └── TransformPositionAxis.cs  # Transform 数值拖拽的轴向。
│   │   │   ├── TransformEdit/
│   │   │   │   ├── SelectedEntityTransformApply.cs  # 应用 Inspector Transform 编辑请求到 WorldState。
│   │   │   │   ├── SelectedEntityTransformReader.cs  # 从 WorldState 读取选中实体的完整 Transform。
│   │   │   │   ├── TransformEditRequest.cs  # Inspector 发起的 Transform 编辑请求。
│   │   │   │   ├── TransformEditResult.cs  # Transform 编辑应用结果。
│   │   │   │   └── TransformInspectorSnapshot.cs  # Inspector 显示的选中实体 Transform 快照。
│   │   │   ├── InspectorPanel.axaml  # XuanYu.Engine.Editor.Windows.Panels.Inspector.InspectorPanel
│   │   │   ├── InspectorPanel.axaml.cs  # InspectorPanel 类
│   │   │   ├── InspectorScrubInput.cs  # InspectorScrubInput — <summary>Inspector 数值拖拽输入处理。X/Y/Z 标签拖拽微调坐标值。</summary>
│   │   │   ├── InspectorSelectionView.cs  # InspectorSelectionView — <summary>Inspector 选择区显示管理。空选择/项目文件/世界实体的展示切换。</summary>
│   │   │   ├── InspectorTransformBinder.cs  # InspectorTransformBinder — <summary>Inspector Transform 键盘与按钮事件绑定。Enter/Esc/Apply/Re...
│   │   │   └── InspectorTransformView.cs  # InspectorTransformView — <summary>Inspector Transform 输入区管理。包含坐标输入框、校验错误和按钮状态。</su...
│   │   ├── LeftDock/
│   │   │   ├── ProjectWorldDockPanel.axaml  # XuanYu.Engine.Editor.Windows.Panels.LeftDock.ProjectWorldDockPanel
│   │   │   ├── ProjectWorldDockPanel.axaml.cs  # ProjectWorldDockPanel 类
│   │   │   └── ProjectWorldDockTabs.cs  # ProjectWorldDockTabs 类
│   │   ├── Logging/
│   │   │   ├── LogPanel.axaml  # XuanYu.Engine.Editor.Windows.Panels.Logging.LogPanel
│   │   │   └── LogPanel.axaml.cs  # LogPanel 类
│   │   ├── ProjectContentTree/
│   │   │   ├── Panel/
│   │   │   │   └── ProjectContentTreePanel.axaml.cs  # ProjectContentTreePanel 类
│   │   │   ├── ProjectContentNodeView.cs  # ProjectContentNodeView 类
│   │   │   ├── ProjectContentTreeExpansion.cs  # ProjectContentTreeExpansion — <summary>项目内容树展开/折叠状态管理。</summary>
│   │   │   ├── ProjectContentTreeIndex.cs  # ProjectContentTreeIndex 类
│   │   │   ├── ProjectContentTreeItems.cs  # ProjectContentTreeItems — <summary>项目内容树视图节点和可见行构造。</summary>
│   │   │   ├── ProjectContentTreePanel.axaml  # XuanYu.Engine.Editor.Windows.Panels.ProjectContentTree.ProjectContentTreePanel
│   │   │   └── ProjectContentTreeSelection.cs  # ProjectContentTreeSelection — <summary>项目内容树选择状态和事件管理。</summary>
│   │   ├── Status/
│   │   │   ├── StatusBarPanel.axaml  # XuanYu.Engine.Editor.Windows.Panels.Status.StatusBarPanel
│   │   │   └── StatusBarPanel.axaml.cs  # StatusBarPanel 类
│   │   ├── Viewport/
│   │   │   ├── HostInfo/
│   │   │   │   ├── Panel/
│   │   │   │   │   ├── VulkanViewportHostPanel.axaml.cs  # VulkanViewportHostPanel — <summary>Vulkan 视口宿主面板。持有原生子窗口，转发原始输入事件。</summary>
│   │   │   │   │   ├── VulkanViewportHostPanel.Events.cs  # VulkanViewportHostPanel — <summary>Partial：原生窗口事件转发接线。</summary>
│   │   │   │   │   └── VulkanViewportHostPanel.NativeHost.cs  # VulkanViewportHostPanel — <summary>Partial：原生宿主信息查询与状态更新。</summary>
│   │   │   │   ├── VulkanViewportHostInfo.cs  # 保存 Vulkan 视口宿主占位显示信息。
│   │   │   │   ├── VulkanViewportHostState.cs  # Editor 中 Vulkan 视口宿主的占位状态。
│   │   │   │   └── VulkanViewportNativeHostInfo.cs  # Vulkan 视口宿主原生窗口句柄信息。
│   │   │   ├── Input/
│   │   │   │   ├── Win32KeyCodeMapper.cs  # Win32 虚拟键码 (VK_*) 到抽象手势代码的静态映射。
│   │   │   │   ├── WindowsViewportGestureMatch.cs  # 手势签名构建与按钮名称映射。
│   │   │   │   ├── WindowsViewportInputTranslator.cs  # Win32 原始输入 → EditorInputMatch 的翻译器门面。
│   │   │   │   ├── WindowsViewportModifierState.cs  # 修饰键（Ctrl / Shift / Alt）状态跟踪。
│   │   │   │   └── WindowsViewportRawInputTranslate.cs  # WindowsViewportRawInputTranslate — <summary>原始输入事件 → EditorInputMatch 翻译。通过 EditorInputBindi...
│   │   │   ├── NativeHost/
│   │   │   │   ├── Control/
│   │   │   │   │   ├── WindowsVulkanViewportHostControl.Events.cs  # WindowsVulkanViewportHostControl 类
│   │   │   │   │   └── WindowsVulkanViewportHostControl.WndProc.cs  # WindowsVulkanViewportHostControl 类
│   │   │   │   ├── HostInfo/
│   │   │   │   │   ├── NativeViewportHostInfo.cs  # NativeViewportHostInfoStatics — <summary>NativeHost 状态信息创建与窗口尺寸同步。</summary>
│   │   │   │   │   ├── WindowsVulkanViewportHostInfo.cs  # WindowsVulkanViewportHostInfo 类
│   │   │   │   │   └── WindowsVulkanViewportHostState.cs  # （待补充）
│   │   │   │   ├── Input/
│   │   │   │   │   ├── Arbitration/
│   │   │   │   │   │   ├── NativeViewportInputArbitration.cs  # NativeViewportInputArbitration 类
│   │   │   │   │   │   ├── NativeViewportInputArbitrationRequest.cs  # NativeViewportInputArbitrationRequest — <summary>输入仲裁的原始指针数据。</summary>
│   │   │   │   │   │   ├── NativeViewportInputArbitrationResult.cs  # （待补充）
│   │   │   │   │   │   ├── NativeViewportNavigationCapture.cs  # NativeViewportNavigationCapture — <summary>Override 导航鼠标捕获状态管理。</summary>
│   │   │   │   │   │   └── NativeViewportSceneToolCapture.cs  # NativeViewportSceneToolCapture — <summary>场景工具鼠标捕获状态管理。</summary>
│   │   │   │   │   ├── Focus/
│   │   │   │   │   │   ├── NativeViewportFocusMessages.cs  # NativeViewportFocusMessages — <summary>Win32 焦点消息处理。含 SetFocus P/Invoke 和焦点消息识别。</summary>
│   │   │   │   │   │   └── NativeViewportHitTestMessages.cs  # NativeViewportHitTestMessages — <summary>Win32 命中测试消息识别。</summary>
│   │   │   │   │   ├── Keyboard/
│   │   │   │   │   │   ├── NativeViewportKeyboardMessages.cs  # NativeViewportKeyboardMessages — <summary>Win32 键盘消息解析与识别。不含编辑器业务。</summary>
│   │   │   │   │   │   └── NativeViewportKeyboardRequest.cs  # NativeViewportKeyboardRequest — <summary>从 Win32 wParam 解析的键盘输入数据。</summary>
│   │   │   │   │   └── Pointer/
│   │   │   │   │       ├── NativeViewportMouseCapture.cs  # NativeViewportMouseCapture — <summary>Win32 鼠标捕获管理。封装 SetCapture / ReleaseCapture 和捕获状...
│   │   │   │   │       ├── NativeViewportMouseTrack.cs  # NativeViewportMouseTrack — <summary>Win32 鼠标跟踪管理。封装 TrackMouseEvent 和跟踪状态。</summary>
│   │   │   │   │       ├── NativeViewportPointerAction.cs  # （待补充）
│   │   │   │   │       ├── NativeViewportPointerMessages.cs  # NativeViewportPointerMessages — <summary>Win32 原生指针消息解析与分发。只处理消息翻译，不含编辑器业务。</summary>
│   │   │   │   │       └── NativeViewportPointerRequest.cs  # NativeViewportPointerRequest — <summary>从 Win32 wParam/lParam 解析的原生指针输入数据。</summary>
│   │   │   │   ├── Lifecycle/
│   │   │   │   │   ├── NativeViewportCreate.cs  # NativeViewportCreate — <summary>NativeHost 子窗口创建。</summary>
│   │   │   │   │   ├── NativeViewportDestroy.cs  # NativeViewportDestroy — <summary>NativeHost 子窗口销毁。</summary>
│   │   │   │   │   ├── NativeViewportHostSync.cs  # NativeViewportHostSync — <summary>NativeHost 状态信息同步与大小变更处理。</summary>
│   │   │   │   │   └── NativeViewportLifecycleResult.cs  # NativeViewportLifecycleResult — <summary>NativeHost 创建结果。</summary>
│   │   │   │   ├── Picking/
│   │   │   │   │   └── WindowsVulkanViewportPickInput.cs  # Win32 视口左键点击检测。
│   │   │   │   ├── SceneTool/
│   │   │   │   │   └── ViewportSceneToolPressResult.cs  # 场景工具（移动、旋转等）对鼠标左键按下的响应结果。
│   │   │   │   ├── Win32/
│   │   │   │   │   ├── Win32ViewportDefaultProc.cs  # Win32ViewportDefaultProc 类
│   │   │   │   │   ├── Win32ViewportDestroyWindow.cs  # Win32ViewportDestroyWindow 类
│   │   │   │   │   ├── Win32ViewportModuleHandle.cs  # Win32ViewportModuleHandle 类
│   │   │   │   │   └── Win32ViewportWindowClass.cs  # Win32ViewportWindowClass — <summary>Win32 视口子窗口类注册管理。</summary>
│   │   │   │   └── WindowsVulkanViewportHostControl.cs  # WindowsVulkanViewportHostControl 类
│   │   │   ├── Placeholder/
│   │   │   │   ├── ViewportPlaceholderPanel.axaml.cs  # ViewportPlaceholderPanel — <summary>视口占位面板。未创建 Vulkan Surface 时显示文本状态。</summary>
│   │   │   │   ├── ViewportPlaceholderPanel.Entity.cs  # ViewportPlaceholderPanel — <summary>Partial：实体摘要与空状态显示。</summary>
│   │   │   │   ├── ViewportPlaceholderPanel.RenderScene.cs  # ViewportPlaceholderPanel — <summary>Partial：RenderScene 调试对象列表显示。</summary>
│   │   │   │   └── ViewportPlaceholderPanel.Vulkan.cs  # ViewportPlaceholderPanel — <summary>Partial：Vulkan 后端状态显示。</summary>
│   │   │   ├── Summary/
│   │   │   │   ├── ViewportEntitySummary.cs  # 视口占位显示模型，保存当前选中实体的名称、EntityId、来源路径、位置文本与视觉类型。
│   │   │   │   ├── ViewportRenderObjectSummary.cs  # 视口 RenderScene 调试列表中的单个渲染对象显示摘要。
│   │   │   │   └── ViewportRenderSceneSummary.cs  # 视口 RenderScene 调试列表显示模型，保存多个渲染对象摘要。
│   │   │   ├── Tools/
│   │   │   │   ├── ViewportEditorTool.cs  # 视口编辑工具类型。
│   │   │   │   ├── ViewportToolPalette.axaml  # XuanYu.Engine.Editor.Windows.Panels.Viewport.Tools.ViewportToolPalette
│   │   │   │   └── ViewportToolPalette.axaml.cs  # ViewportToolPalette 类
│   │   │   ├── ViewportPlaceholderPanel.axaml  # XuanYu.Engine.Editor.Windows.Panels.Viewport.ViewportPlaceholderPanel
│   │   │   └── VulkanViewportHostPanel.axaml  # XuanYu.Engine.Editor.Windows.Panels.Viewport.VulkanViewportHostPanel
│   │   └── WorldHierarchy/
│   │       ├── View/
│   │       │   ├── WorldHierarchyNodeView.cs  # WorldHierarchyNodeView 类
│   │       │   ├── WorldHierarchyTreeExpansion.cs  # WorldHierarchyTreeExpansion — <summary>世界层级树展开/折叠状态管理。</summary>
│   │       │   └── WorldHierarchyTreeViewState.cs  # 保存节点树的展开/选择状态，用于树重建时恢复。
│   │       ├── WorldHierarchyProgrammaticSelection.cs  # 程序化选择一次性令牌。
│   │       ├── WorldHierarchyTreeIndex.cs  # WorldHierarchyTreeIndex 类
│   │       ├── WorldHierarchyTreeItems.cs  # WorldHierarchyTreeItems — <summary>世界层级树视图节点和可见行构造。</summary>
│   │       ├── WorldHierarchyTreePanel.axaml  # XuanYu.Engine.Editor.Windows.Panels.WorldHierarchy.WorldHierarchyTreePanel
│   │       ├── WorldHierarchyTreePanel.axaml.cs  # WorldHierarchyTreePanel 类
│   │       └── WorldHierarchyTreeSelection.cs  # WorldHierarchyTreeSelection — <summary>世界层级树选择状态和事件管理。</summary>
│   ├── Preferences/
│   │   ├── Capture/
│   │   │   └── EditorPreferencesCapture.cs  # EditorPreferencesCapture 类
│   │   ├── Helpers/
│   │   │   ├── EditorPreferencesFormatText.cs  # EditorPreferencesFormatText 类
│   │   │   └── EditorPreferencesKeyMapper.cs  # EditorPreferencesKeyMapper 类
│   │   ├── EditorPreferencesBindingList.cs  # EditorPreferencesBindingList 类
│   │   ├── EditorPreferencesDraftHandler.cs  # EditorPreferencesDraftHandler 类
│   │   ├── EditorPreferencesWindow.axaml  # XuanYu.Engine.Editor.Windows.Preferences.EditorPreferencesWindow
│   │   └── EditorPreferencesWindow.axaml.cs  # EditorPreferencesWindow 类
│   ├── Shell/
│   │   ├── Commands/
│   │   │   └── EditorShellWindowCommandsRoute.cs  # EditorShellWindowCommandsRoute — <summary>窗口菜单命令路由。负责 Preferences/About/InputBindings 菜单命令...
│   │   ├── Composition/
│   │   │   ├── Core/
│   │   │   │   ├── EditorShellComposition.cs  # EditorShellComposition — <summary>EditorShell 组合根构建器。负责创建上下文、Route、事件接线和初始化。</summ...
│   │   │   │   ├── EditorShellCompositionRuntime.cs  # EditorShellCompositionRuntime — <summary>EditorShell 运行时辅助方法。Build 方法调用的本地函数提取。</summary>
│   │   │   │   └── EditorShellContext.cs  # EditorShellContext — <summary>EditorShell 组合根上下文。持有所有控件引用、Route 引用和可变状态。</summ...
│   │   │   ├── Wiring/
│   │   │   │   ├── EditorShellEventWiring.cs  # EditorShellEventWiring — <summary>EditorShell 事件接线。集中管理面板事件与 Route 之间的订阅。</summary>
│   │   │   │   └── EditorShellLifecycle.cs  # EditorShellLifecycle — <summary>EditorShell 生命周期路由。处理 Attach/Detach 事件转发。</summary>
│   │   │   ├── EditorShellControlRefs.cs  # EditorShellControlRefs — <summary>EditorShell 的 Avalonia 控件引用。由 Find() 从 VisualTre...
│   │   │   ├── EditorShellRouteBuild.cs  # EditorShellRouteBuild — <summary>创建并初始化所有 Route。Shell 在构造期调用 Build(controls) 获得 R...
│   │   │   └── EditorShellRouteSet.cs  # EditorShellRouteSet — <summary>EditorShell 的全部 Route 引用。通过 RouteBuild.Build() 创...
│   │   ├── Diagnostics/
│   │   │   ├── Log/
│   │   │   │   └── EditorShellLogRoute.cs  # EditorShellLogRoute — <summary>日志路由。负责日志委托和 Diagnostics 薄转发。</summary>
│   │   │   ├── EditorDiagnosticsRefreshKind.cs  # （待补充）
│   │   │   ├── EditorDiagnosticsRefreshRequest.cs  # EditorDiagnosticsContext — <summary>诊断路由的上下文依赖。Shell 在构造后初始化一次。</summary>
│   │   │   ├── EditorDiagnosticsRefreshResult.cs  # EditorDiagnosticsRefreshResult 类
│   │   │   ├── EditorDiagnosticsRefreshRoute.cs  # EditorDiagnosticsRefreshRoute 类
│   │   │   └── EditorDiagnosticsRefreshState.cs  # EditorDiagnosticsRefreshState 类
│   │   ├── Feedback/
│   │   │   └── EditorFeedbackRoute.cs  # EditorFeedbackRoute 类
│   │   ├── Hierarchy/
│   │   │   └── EditorShellHierarchyRoute.cs  # EditorShellHierarchyRoute — <summary>层级树路由。负责世界层级树与项目内容树构建显示。</summary>
│   │   ├── Input/
│   │   │   ├── Picking/
│   │   │   │   ├── EditorGroundHoverInputRequest.cs  # EditorGroundHoverInputRequest — <summary>GroundHoverRoute 的请求。只携带地面悬停所需的字段。</summary>
│   │   │   │   ├── EditorGroundHoverInputResult.cs  # EditorGroundHoverInputResult — <summary>GroundHoverRoute 的结果。</summary>
│   │   │   │   ├── EditorGroundHoverInputRoute.cs  # EditorGroundHoverInputRoute — <summary>地面悬停路由。鼠标移动时执行 CPU 射线-地面求交，更新状态栏坐标。</summary>
│   │   │   │   ├── EditorPickInputResult.cs  # EditorPickInputResult — <summary>PickRoute 的结果。Shell 据此应用选择和地面标记。</summary>
│   │   │   │   └── EditorPickInputRoute.cs  # EditorPickInputRoute — <summary>视口点击 Picking 路由。执行射线-场景求交，决策选择/地面标记/放置模式。</summary>
│   │   │   ├── Raw/
│   │   │   │   └── EditorShellRawInputRoute.cs  # EditorShellRawInputRoute — <summary>原始输入转发路由。负责将原生视口事件的原始参数转发到 ViewportInputRoute。</...
│   │   │   ├── Transform/
│   │   │   │   ├── EditorSceneToolInputResult.cs  # EditorSceneToolInputResult — <summary>SceneTool 输入路由的结果。携带视口事件响应值。</summary>
│   │   │   │   ├── EditorSceneToolInputRoute.cs  # EditorSceneToolInputRoute — <summary>SceneTool 场景工具输入路由。负责 Gizmo 点按 / 实体体拖拽启动和释放确认。</...
│   │   │   │   ├── EditorTransformInputRequest.cs  # EditorTransformInputRequest — <summary>Transform 输入路由的专用请求，比全量 InputRequest 更轻量。</summary>
│   │   │   │   ├── EditorTransformInputResult.cs  # EditorTransformInputResult — <summary>Transform 输入路由的结果。Handled=true 表示事件已被变换逻辑消费。</su...
│   │   │   │   └── EditorTransformInputRoute.cs  # EditorTransformInputRoute — <summary>变换交互输入路由。负责 G 键 / Esc / Enter 模态、Gizmo Hover、拖拽 ...
│   │   │   ├── EditorViewportInputKind.cs  # （待补充）
│   │   │   ├── EditorViewportInputRequest.cs  # EditorViewportInputRequest 类
│   │   │   ├── EditorViewportInputResult.cs  # EditorViewportInputResult — <summary>InputRoute → Shell 的结果。</summary>
│   │   │   ├── EditorViewportInputRoute.cs  # EditorViewportInputRoute 类
│   │   │   └── EditorViewportInputState.cs  # EditorViewportInputState 类
│   │   ├── Lifecycle/
│   │   │   ├── EditorShellAttachRequest.cs  # EditorShellAttachRequest — <summary>Shell → AttachRoute 的请求。NativeHostReport 和 Input...
│   │   │   ├── EditorShellAttachResult.cs  # EditorShellAttachResult — <summary>AttachRoute → Shell 的结果。AttachDispatched 表示是否首次触...
│   │   │   ├── EditorShellAttachRoute.cs  # 管理 EditorShell 首次附加到 VisualTree 后的初始化时序。
│   │   │   ├── EditorShellDetachResult.cs  # EditorShellDetachResult — <summary>DetachRoute → Shell 的结果。Shell 据此恢复状态标志。</summary>
│   │   │   └── EditorShellDetachRoute.cs  # 管理 EditorShell 从 VisualTree 分离后的清理时序。
│   │   ├── Menu/
│   │   │   └── EditorRunMenuRoute.cs  # EditorRunMenuRoute — <summary>运行菜单路由。管理 RunMenuButton Flyout 和 Scene3D 菜单项状态。<...
│   │   ├── Navigation/
│   │   │   └── EditorShellOverlayNavigationRoute.cs  # EditorShellOverlayNavigationRoute — <summary>Overlay 导航事件路由。负责 Overlay 视口导航区的交互转发。</summary>
│   │   ├── Panels/
│   │   │   ├── EditorPanelApplyKind.cs  # （待补充）
│   │   │   ├── EditorPanelApplyRequest.cs  # EditorPanelApplyPanels — <summary>Panel Apply Route 持有的面板引用。Shell 在 FindShellContr...
│   │   │   ├── EditorPanelApplyResult.cs  # EditorPanelApplyResult 类
│   │   │   ├── EditorPanelApplyRoute.cs  # EditorPanelApplyRoute 类
│   │   │   └── EditorPanelApplyState.cs  # EditorPanelApplyState 类
│   │   ├── Picking/
│   │   │   ├── EditorShellGroundPointerRoute.cs  # 鼠标在视口内移动 → 地面射线求交 → 状态栏反馈。
│   │   │   └── EditorShellPickingRoute.cs  # EditorShellPickingRoute — <summary>Picking 路由。负责视口点击拾取、地面标记控制及拾取诊断。</summary>
│   │   ├── Project/
│   │   │   └── EditorShellProjectBootstrapRoute.cs  # EditorShellProjectBootstrapRoute — <summary>项目加载 + World Bootstrap 路由。负责项目加载、加载结果应用、World 初始...
│   │   ├── Scene3D/
│   │   │   ├── Commands/
│   │   │   │   ├── EditorScene3dCommandKind.cs  # （待补充）
│   │   │   │   ├── EditorScene3dCommandRequest.cs  # EditorScene3dCommandRequest — <summary>Shell → Scene3dCommandRoute 的请求。CameraRoute 仅用于 ...
│   │   │   │   ├── EditorScene3dCommandResult.cs  # EditorScene3dCommandResult — <summary>Scene3dCommandRoute → Shell 的结果。</summary>
│   │   │   │   ├── EditorScene3dCommandRoute.cs  # EditorScene3dCommandRoute 类
│   │   │   │   └── EditorScene3dCommandState.cs  # EditorScene3dCommandState — <summary>Scene3D 命令路由的运行追踪状态。当前保留扩展用。</summary>
│   │   │   └── EditorShellScene3dCommandRoute.cs  # EditorShellScene3dCommandRoute — <summary>Scene3D 命令路由。负责 Session 启动/重启命令薄转发。</summary>
│   │   ├── Selection/
│   │   │   └── EditorShellSelectionSyncRoute.cs  # EditorShellSelectionSyncRoute — <summary>选择同步路由。负责实体选择在视口/WorldTree/Inspector/Scene 之间的联动...
│   │   ├── Startup/
│   │   │   ├── Vulkan/
│   │   │   │   ├── EditorStartupVulkanRequest.cs  # EditorStartupVulkanRequest — <summary>Shell → VulkanStartupRoute 的请求。携带 Route 执行探测链所需的...
│   │   │   │   ├── EditorStartupVulkanResult.cs  # EditorStartupVulkanResult — <summary>VulkanStartupRoute → Shell 的结果。Shell 据此应用 UI 更新。...
│   │   │   │   ├── EditorStartupVulkanRoute.cs  # Vulkan 启动探测编排路由。构造期 Backend→Instance→Device→Surface 链 + 附加期 NativeHost→Swapchain→Clear→AutoStart 链。
│   │   │   │   ├── EditorStartupVulkanState.cs  # EditorStartupVulkanState — <summary>Vulkan 启动探测的状态标志。由 Route 内部管理，Shell 通过 Result 和属...
│   │   │   │   └── EditorStartupVulkanStep.cs  # （待补充）
│   │   │   ├── EditorShellStartupVulkanProbeRoute.cs  # EditorShellStartupVulkanProbeRoute — <summary>Startup Vulkan Probe 路由。负责构建启动探测请求、执行转发、结果应用。</s...
│   │   │   ├── EditorStartupBootstrapResult.cs  # EditorStartupBootstrapResult — <summary>启动引导的完整结果。Shell 根据此结果应用 UI。</summary>
│   │   │   ├── EditorStartupBootstrapRoute.cs  # EditorStartupBootstrapRoute — <summary>启动引导编排路由。协调 ProjectBootstrapRoute、WorldBootstrap...
│   │   │   └── EditorStartupWorldResult.cs  # EditorStartupWorldResult — <summary>World 引导结果封装，包含世界状态、RenderScene、首位实体 ID、日志等。</su...
│   │   ├── Transform/
│   │   │   ├── Edit/
│   │   │   │   ├── EditorShellScrubRoute.cs  # EditorShellScrubRoute — <summary>数值拖拽 Scrub 路由。负责 Inspector 数值拖拽事件转发。</summary>
│   │   │   │   └── EditorShellTransformRoute.cs  # EditorShellTransformRoute — <summary>Transform 编辑路由。负责 Inspector Transform 面板事件转发。</s...
│   │   │   ├── EditorGroundPlacementResult.cs  # EditorGroundPlacementResult 类
│   │   │   ├── EditorGroundPlacementRoute.cs  # EditorGroundPlacementRoute 类
│   │   │   ├── EditorTransformApplyRequest.cs  # EditorTransformApplyDeps — <summary>Transform 提交/Preview/Cancel 的运行时依赖。Shell 在 InitT...
│   │   │   ├── EditorTransformApplyResult.cs  # EditorTransformApplyResult 类
│   │   │   └── EditorTransformApplyRoute.cs  # EditorTransformApplyRoute — <summary>Transform 提交 / Preview / Cancel / Inspector 编排。不...
│   │   ├── Viewport/
│   │   │   ├── EditorShellViewportFocusRoute.cs  # EditorShellViewportFocusRoute — <summary>视口焦点路由。负责 Viewport 聚焦事件和世界实体选择展示转发。</summary>
│   │   │   ├── EditorShellViewportFrameRoute.cs  # EditorShellViewportFrameRoute — <summary>视口 Frame Selected / 聚焦所选路由。负责实体聚焦相机命令的构建与执行。</su...
│   │   │   ├── EditorShellViewportRedrawRoute.cs  # EditorShellViewportRedrawRoute — <summary>Viewport 重绘路由。负责 Vulkan redraw 调度、resize 结果应用。</...
│   │   │   └── EditorShellViewportSizeGuard.cs  # EditorShellViewportSizeGuard — <summary>视口尺寸校验工具。检查 NativeHost 尺寸是否可用。</summary>
│   │   ├── Windows/
│   │   │   ├── EditorShellWindowCommand.cs  # （待补充）
│   │   │   ├── EditorShellWindowResult.cs  # EditorShellWindowResult 类
│   │   │   └── EditorShellWindowRoute.cs  # EditorShellWindowRoute — <summary>编辑器窗口命令路由。管理 Preferences / InputBindings / About...
│   │   ├── EditorSelection.cs  # struct 类
│   │   ├── EditorShell.axaml  # XuanYu.Engine.Editor.Windows.Shell.EditorShell
│   │   └── EditorShell.axaml.cs  # EditorShell 类
│   ├── Viewport/
│   │   ├── Camera/
│   │   │   ├── ViewportCameraCommand.cs  # 相机命令的区分联合类型。每种具体命令是一个 sealed record 子类。
│   │   │   ├── ViewportCameraFocusTarget.cs  # 计算 FrameSelected 的聚焦目标（包围盒中心 + 半径）。
│   │   │   ├── ViewportCameraResult.cs  # 相机操作结果。Shell 根据此结果决定是否请求 Scene3D Frame。
│   │   │   └── ViewportCameraRoute.cs  # ViewportCameraRoute — <summary>相机状态和命令路由。拥有 _lastCameraState 和 _cameraRevision，...
│   │   ├── Navigation/
│   │   │   ├── ViewportNavigationResponse.cs  # ViewportNavigationPressResponse — <summary>Overlay Pointer Press 的结果。</summary>
│   │   │   └── ViewportNavigationRoute.cs  # ViewportNavigationRoute — <summary>Viewport 右上角导航 Overlay 输入路由。拥有拖拽/悬停/选中状态，通过 Pres...
│   │   ├── Picking/
│   │   │   ├── ViewportPickFailure.cs  # ViewportPickFailureResult — <summary>Picking 失败结果。</summary>
│   │   │   ├── ViewportPickRequest.cs  # ViewportPointerPickRoute 的输入请求。
│   │   │   ├── ViewportPickResult.cs  # ViewportPointerPickRoute.Pick 的输出结果。
│   │   │   ├── ViewportPickTrace.cs  # Debug Picking 诊断。非 Entity 命中时输出每个实体的位置和 Ray-AABB 结果。
│   │   │   └── ViewportPointerPickRoute.cs  # 视口 Picking 路由。单一职责：将像素坐标转换为拾取结果。
│   │   ├── Project/
│   │   │   ├── ProjectBootstrapResult.cs  # ProjectBootstrapResult — <summary>项目启动结果。Shell 根据结果应用 UI。</summary>
│   │   │   └── ProjectBootstrapRoute.cs  # ProjectBootstrapRoute — <summary>项目加载路由。处理项目发现、加载、校验逻辑。不持有 UI 控件引用。</summary>
│   │   ├── Scene3D/
│   │   │   ├── Diagnostics/
│   │   │   │   ├── Scene3dDiagnosticSnapshot.cs  # Scene3dDiagnosticSnapshot — <summary>Scene3D 诊断快照。Shell 读取后分发到 DebugDockPanel。</summary>
│   │   │   │   ├── Scene3dDiagnosticText.cs  # Scene3dDiagnosticText — <summary>诊断文本格式化。不包含 Shell/面板引用。</summary>
│   │   │   │   ├── VulkanViewportProbeResult.cs  # struct 类
│   │   │   │   ├── VulkanViewportProbeRoute.cs  # VulkanViewportProbeRoute — 日志通过回调注入，Shell 无需知道探测细节。</summary>
│   │   │   │   └── VulkanViewportProbeState.cs  # VulkanViewportProbeState — <summary>Vulkan 探测和 Scene3D 诊断状态的单一所有者。</summary>
│   │   │   ├── Frame/
│   │   │   │   ├── Scene3dDrawListBuilder.cs  # 从 RenderScene 构建 Vulkan 单位绘制列表。
│   │   │   │   ├── Scene3dFrameRoute.cs  # Scene3D 帧路径路由。单一职责：请求、合并、执行帧。
│   │   │   │   ├── Scene3dFrameState.cs  # Scene3D 帧路径的核心状态。
│   │   │   │   └── Scene3dPresentedState.cs  # Gizmo 和 Pick Snapshot 的 Pending/Presented 双缓冲。
│   │   │   ├── Lifecycle/
│   │   │   │   ├── Scene3dSessionLifecycle.cs  # Scene3D 会话生命周期管理。单一职责：启动/停止/重启/Resize 会话。
│   │   │   │   ├── Scene3dSessionRestartReason.cs  # （待补充）
│   │   │   │   ├── Scene3dSessionStartRequest.cs  # struct 类
│   │   │   │   ├── Scene3dSessionStartResult.cs  # Scene3dSessionStartResult — <summary>Scene3D 会话启动结果。不含文案，Message 仅用于日志。</summary>
│   │   │   │   └── Scene3dSessionState.cs  # Scene3dSessionState — <summary>Scene3D 会话运行时状态。三个字段的单一所有权容器。</summary>
│   │   │   ├── Resize/
│   │   │   │   ├── Scene3dResizeRenderRequest.cs  # Scene3dResizeRenderRequest — <summary>渲染重绘请求的输入数据。Shell 在每次触发重绘时创建。</summary>
│   │   │   │   ├── Scene3dResizeRenderResult.cs  # Scene3dResizeRenderResult — <summary>渲染重绘的执行结果。Shell 根据 Action 决定后续 Apply 操作。</summary>
│   │   │   │   └── Scene3dResizeRenderRoute.cs  # Scene3dResizeRenderRoute — <summary>视口 Resize / 重绘渲染路由。管理渲染锁、尺寸校验、Session.Resize 和 C...
│   │   │   └── Submit/
│   │   │       ├── Scene3dFrameSubmitInput.cs  # struct 类
│   │   │       ├── Scene3dFrameSubmitRoute.cs  # Scene3D 帧提交流程编排。组装 Gizmo + PickSnapshot → 调用 Scene3dFrameRoute.Request。
│   │   │       ├── Scene3dGizmoSubmitSource.cs  # Scene3dGizmoSubmitSource — 是唯一被允许调用 SetMoveGizmoVertices 的模块。</summary>
│   │   │       └── Scene3dPickSnapshotSource.cs  # Scene3dPickSnapshotSource — <summary>Pick Snapshot 构建。封装 PresentedScenePickSnapshotBu...
│   │   ├── Selection/
│   │   │   ├── Focus/
│   │   │   │   ├── ViewportFocusSelectionResult.cs  # ViewportFocusSelectionResult — <summary>视口焦点处理的完整输出。Shell 无需分支直接 Apply。</summary>
│   │   │   │   └── ViewportFocusSelectionRoute.cs  # ViewportFocusSelectionRoute — <summary>视口获得焦点时的选择路由。决定应聚焦哪个实体（或无），并返回完整展示结果。</summary>
│   │   │   ├── Presentation/
│   │   │   │   ├── EditorSelectionPresenter.cs  # EditorSelectionPresenter — <summary>基础选择展示工具。默认选择等工厂方法。</summary>
│   │   │   │   ├── ProjectContentSelectionPresenter.cs  # ProjectContentSelectionPresenter — <summary>项目文件选择 → Inspector / StatusBar / Log 展示。纯转换，不接触 ...
│   │   │   │   ├── SelectionPresentationResult.cs  # WorldEntitySelectionResult — <summary>世界实体选择展示结果。Shell Apply 到 Inspector / StatusBar /...
│   │   │   │   ├── ViewportSelectionPresenter.cs  # ViewportSelectionPresenter — <summary>视口摘要展示。从 RenderScene 生成视口列表。</summary>
│   │   │   │   └── WorldEntitySelectionPresenter.cs  # WorldEntitySelectionPresenter — <summary>世界实体选择 → Inspector / StatusBar / Viewport 展示。纯转换...
│   │   │   └── Route/
│   │   │       ├── EditorSelectionReason.cs  # （待补充）
│   │   │       ├── EditorSelectionRequest.cs  # struct 类
│   │   │       ├── EditorSelectionRoute.cs  # 选择路由。决定“选中谁”，不决定“怎么展示”。
│   │   │       ├── EditorSelectionRouteResult.cs  # EditorSelectionRouteResult — <summary>选择路由的结果。Shell 用此结果应用 UI 展示。</summary>
│   │   │       └── EditorSelectionState.cs  # EditorSelectionState — <summary>选择状态的唯一所有者。Shell 通过 Route 间接读取。</summary>
│   │   ├── Transform/
│   │   │   ├── Application/
│   │   │   │   ├── Capabilities/
│   │   │   │   │   ├── InspectorTransformDisplay.cs  # InspectorTransformDisplay — <summary>Inspector Transform 数值显示能力。不暴露 Panel 其他 API。</su...
│   │   │   │   │   ├── Scene3dEntityPositionWriter.cs  # Scene3dEntityPositionWriter — <summary>Scene3D 实体位置更新能力。仅暴露 UpdateEntityPosition，不暴露 Se...
│   │   │   │   │   └── WorldTransformWriter.cs  # WorldTransformWriter — <summary>WorldState Transform 写入能力。负责原子写入 + Dirty 标记。</su...
│   │   │   │   ├── EntityTransformCancel.cs  # Cancel：恢复实体到初始 Transform。还原 RenderScene + Vulkan + Inspector。
│   │   │   │   ├── EntityTransformCommit.cs  # Commit：原子提交。先验证实体存在，再同步视觉层（RenderScene + Vulkan），最后写 WorldState + Dirty。
│   │   │   │   ├── EntityTransformPreview.cs  # Preview：验证 RenderScene 可更新 → 写 Vulkan → 更新 Inspector。
│   │   │   │   ├── TransformApplyResult.cs  # struct 类
│   │   │   │   └── ViewportRenderSceneStore.cs  # ViewportRenderSceneStore — <summary>RenderScene 当前位置的唯一写入所有者。读取仍可通过 Current 属性。</sum...
│   │   │   ├── Drag/
│   │   │   │   ├── Anchors/
│   │   │   │   │   ├── TransformDragMoveResult.cs  # struct 类
│   │   │   │   │   └── TransformStartSnapshot.cs  # 拖动开始时的冻结快照。保存所有外部状态，DragRoute 不持有 Shell 引用。
│   │   │   │   ├── AxisDragAnchorBuilder.cs  # 从相机快照和拖动参数构建 AxisTranslationAnchor。
│   │   │   │   ├── PlaneDragAnchorBuilder.cs  # 从相机快照构建射线，与平面求交后创建 PlaneTranslationAnchor。
│   │   │   │   ├── TransformDragKind.cs  # （待补充）
│   │   │   │   └── TransformDragRoute.cs  # TransformDragRoute 类
│   │   │   ├── Gizmo/
│   │   │   │   ├── Core/
│   │   │   │   │   ├── GizmoDist.cs  # GizmoDist — <summary>Gizmo 命中和视觉用的点线距离计算。</summary>
│   │   │   │   │   ├── MoveGizmoElement.cs  # Move Gizmo 的可交互元素。
│   │   │   │   │   └── MoveGizmoVisualState.cs  # （待补充）
│   │   │   │   ├── HitTest/
│   │   │   │   │   └── PlaneHandleHitTest.cs  # Plane 手柄的四边形命中测试。
│   │   │   │   ├── Interaction/
│   │   │   │   │   ├── MoveGizmoDrawList.cs  # 从 MoveGizmoLayout 生成屏幕空间三角形顶点列表。
│   │   │   │   │   ├── MoveGizmoHitTest.cs  # Move Gizmo 的屏幕空间命中测试。
│   │   │   │   │   └── MoveGizmoInteraction.cs  # Move Gizmo 的交互状态机。
│   │   │   │   ├── Layout/
│   │   │   │   │   ├── Measure.cs  # 从 MoveGizmoLayout 提取结构化的四边形角坐标，供 HitTest 和 DrawList 使用。
│   │   │   │   │   ├── MoveGizmoLayout.cs  # Move Gizmo 的布局计算。
│   │   │   │   │   ├── MoveGizmoPlaneLayout.cs  # Plane 手柄的屏幕四边形顶点（四个角）。
│   │   │   │   │   └── PresentedMoveGizmoSnapshot.cs  # 最近成功渲染的 Move Gizmo 布局快照。
│   │   │   │   └── Visual/
│   │   │   │       ├── MoveGizmoAxisVertices.cs  # 生成 Gizmo 三轴和箭头的三角形顶点。
│   │   │   │       └── MoveGizmoPlaneVertices.cs  # 生成 Gizmo 三个平面块和中心手柄的三角形顶点。
│   │   │   ├── Interaction/
│   │   │   │   ├── TransformInteractionResult.cs  # struct 类
│   │   │   │   ├── TransformInteractionState.cs  # Move 工具和 G 模态的共享状态。
│   │   │   │   ├── TransformKeyboardRoute.cs  # 键盘变换交互路由。只处理 G/Enter/Esc 三键。
│   │   │   │   ├── TransformPointerRoute.cs  # 视口指针变换交互路由。负责：Gizmo HitTest、Drag 调度、G 模态逐出。
│   │   │   │   └── TransformStartRequest.cs  # struct 类
│   │   │   └── Presentation/
│   │   │       ├── MoveGizmoFrameInput.cs  # struct 类
│   │   │       ├── MoveGizmoFrameResult.cs  # struct 类
│   │   │       ├── MoveGizmoFrameSource.cs  # Move Gizmo 帧数据源。纯计算：输入相机/实体/工具状态 → 输出顶点 + Pending Snapshot。
│   │   │       └── MoveGizmoVisibility.cs  # MoveGizmoVisibility — <summary>Move Gizmo 可见性判断。纯计算，不访问 Vulkan/Shell/WorldState...
│   │   └── World/
│   │       └── Bootstrap/
│   │           ├── WorldBootstrapEntitySeed.cs  # WorldBootstrapEntitySeed — <summary>从项目内容文件生成 World 占位实体。纯数据逻辑。</summary>
│   │           ├── WorldBootstrapInput.cs  # struct 类
│   │           ├── WorldBootstrapRenderSeed.cs  # WorldBootstrapRenderSeed — <summary>从 WorldState 生成 RenderScene。纯数据逻辑。</summary>
│   │           ├── WorldBootstrapResult.cs  # WorldBootstrapResult — <summary>World 引导结果。Shell 用此结果更新 Store、Selection、UI。</sum...
│   │           └── WorldBootstrapRoute.cs  # World 引导路由。创建 WorldState → 播种实体 → 生成 RenderScene → 返回结果。
│   ├── Assets/
│   │   └── Icons/
│   │       ├── logo.png                        # 应用图标（LOGO.png 复制入库）
│   │       ├── Hierarchy/
│   │       │   ├── *.svg                       # 层级树与项目内容树图标集
│   │       │   └── ...
│   │       └── ViewportNavigation/
│   │           ├── nav_pan.svg                  # 视口导航：平移按钮图标
│   │           ├── nav_frame.svg                # 视口导航：聚焦 / 查看全部按钮图标
│   │           ├── nav_projection_persp.svg     # 视口导航：当前透视投影状态图标
│   │           └── nav_projection_ortho.svg     # 视口导航：当前正交投影状态图标
│   ├── App.axaml  # XuanYu.Engine.Editor.Windows.App
│   ├── App.axaml.cs  # App 类
│   ├── GlobalUsings.cs  # （待补充）
│   ├── MainWindow.axaml  # XuanYu.Engine.Editor.Windows.MainWindow
│   ├── MainWindow.axaml.cs  # MainWindow 类
│   └── Program.cs  # Program 类
├── XuanYu.Engine.Tests/
│   ├── Architecture/
│   │   ├── CodeFileBudgetTests.cs  # 代码架构宪法测试。逐步推行 100 行硬线 + 每目录 ≤5 直属文件。
│   │   └── ProjectDependencyDirectionTests.cs  # ProjectDependencyDirectionTests 类
│   ├── Bridge/
│   │   └── ProjectEngine/
│   │       └── World/
│   │           └── ProjectContentWorldSeederTests.cs  # ProjectContentWorldSeederTests 类
│   ├── Core/
│   │   ├── Identity/
│   │   │   └── EntityIdTests.cs  # EntityIdTests 类
│   │   ├── Logging/
│   │   │   ├── EngineLogEntryTests.cs  # EngineLogEntryTests 类
│   │   │   └── EngineLogLevelTests.cs  # EngineLogLevelTests 类
│   │   ├── Math/
│   │   │   ├── Vector3dTests.cs  # Vector3dTests 类
│   │   │   └── YawRotationTests.cs  # YawRotationTests 类
│   │   ├── Results/
│   │   │   ├── EngineErrorTests.cs  # EngineErrorTests 类
│   │   │   └── EngineResultTests.cs  # EngineResultTests 类
│   │   └── Time/
│   │       ├── SimulationTimeTests.cs  # SimulationTimeTests 类
│   │       └── TimeStepTests.cs  # TimeStepTests 类
│   ├── Editor/
│   │   ├── EntityTransform/
│   │   │   ├── EditorEntityTransformValidationTests.cs  # EditorEntityTransformValidationTests 类
│   │   │   ├── EditorGroundPlacementStateTests.cs  # EditorGroundPlacementStateTests 类
│   │   │   └── EditorWorldDirtyStateTests.cs  # EditorWorldDirtyStateTests 类
│   │   ├── Input/
│   │   │   ├── Actions/
│   │   │   │   └── EditorInputContextChainTests.cs  # EditorInputContextChainTests 类
│   │   │   ├── Bindings/
│   │   │   │   ├── EditorInputBindingSetTests.cs  # EditorInputBindingSetTests 类
│   │   │   │   ├── EditorInputConflictDetectorTests.cs  # EditorInputConflictDetectorTests 类
│   │   │   │   ├── EditorInputGestureTests.cs  # EditorInputGestureTests 类
│   │   │   │   └── Win32KeyCodeMapperTests.cs  # Win32KeyCodeMapper 映射表测试（纯字典测试，无平台依赖）。
│   │   │   ├── Runtime/
│   │   │   │   ├── EditorInputBindingSnapshotTests.cs  # EditorInputBindingSnapshotTests 类
│   │   │   │   └── EditorInputServiceTests.cs  # EditorInputServiceTests 类
│   │   │   └── Settings/
│   │   │       ├── EditorSettingsPathMigrationTests.cs  # 测试 EditorSettingsPathMigration 的目录迁移逻辑。
│   │   │       └── EditorSettingsWriterTests.cs  # 直接测试 EditorSettingsReader/Writer 序列化逻辑，
│   │   ├── Transform/
│   │   │   ├── Drag/
│   │   │   │   ├── TransformApplyResultTests.cs  # TransformApplyResultTests 类
│   │   │   │   └── TransformDragRouteTests.cs  # TransformDragRouteTests 类
│   │   │   ├── Gizmo/
│   │   │   │   ├── MoveGizmoHitTestTests.cs  # MoveGizmoHitTest 的单元测试。
│   │   │   │   └── PresentedMoveGizmoSnapshotTests.cs  # PresentedMoveGizmoSnapshot 的有效性追踪测试。
│   │   │   └── Translation/
│   │   │       └── Axis/
│   │   │           └── AxisTranslationEventCountTests.cs  # 关键验收测试：1 次与 100 次 PointerMove 必须产生完全相同的结果。
│   │   ├── ViewportGround/
│   │   │   └── EditorGroundPointerStateTests.cs  # EditorGroundPointerStateTests 类
│   │   └── WorldHierarchy/
│   │       └── WorldHierarchyTreeBuilderTests.cs  # WorldHierarchyTreeBuilderTests 类
│   ├── Engine/
│   │   └── World/
│   │       ├── EntityPosition/
│   │       │   └── WorldEntityPositionWriterTests.cs  # WorldEntityPositionWriterTests 类
│   │       └── WorldStateTests.cs  # WorldStateTests 类
│   ├── Project/
│   │   ├── Content/
│   │   │   └── GameContentFileScannerTests.cs  # GameContentFileScannerTests 类
│   │   ├── Loading/
│   │   │   ├── GameProjectLoaderTests.cs  # GameProjectLoaderTests 类
│   │   │   └── SampleProjectSmokeTests.cs  # SampleProjectSmokeTests 类
│   │   ├── Paths/
│   │   │   └── SampleProjectPathTests.cs  # SampleProjectPathTests 类
│   │   └── Validation/
│   │       └── ProjectValidationReportTests.cs  # ProjectValidationReportTests 类
│   ├── Render/
│   │   ├── Camera/
│   │   │   ├── Navigation/
│   │   │   │   ├── SceneNavigationCameraMotionTests.cs  # SceneNavigationCameraMotionTests 类
│   │   │   │   └── SceneOrthographicProjectionTests.cs  # SceneOrthographicProjectionTests 类
│   │   │   ├── SceneCameraLimitsTests.cs  # SceneCameraLimitsTests 类
│   │   │   ├── SceneCameraMotionTests.cs  # SceneCameraMotionTests 类
│   │   │   ├── SceneCameraStateTests.cs  # SceneCameraStateTests 类
│   │   │   └── SceneOrbitCameraMotionTests.cs  # SceneOrbitCameraMotionTests 类
│   │   ├── Scene/
│   │   │   ├── Position/
│   │   │   │   └── RenderSceneObjectPositionWriterTests.cs  # RenderSceneObjectPositionWriterTests 类
│   │   │   └── RenderUnitPlacementTests.cs  # RenderUnitPlacementTests 类
│   │   ├── Selection/
│   │   │   ├── Ground/
│   │   │   │   └── SceneRayGroundIntersectionTests.cs  # SceneRayGroundIntersectionTests 类
│   │   │   ├── Pointer/
│   │   │   │   └── ScenePointerPickerTests.cs  # ScenePointerPickerTests 类
│   │   │   ├── Presented/
│   │   │   │   ├── PresentedScenePickLifecycleTests.cs  # Preview → Confirm → Cancel 对 Picking 状态的影响。
│   │   │   │   └── PresentedScenePickSnapshotTests.cs  # PresentedScenePickSnapshotTests 类
│   │   │   └── Screen/
│   │   │       └── ScreenEntityPickerTests.cs  # ScreenEntityPickerTests 类
│   │   ├── ViewportNavigation/
│   │   │   └── ViewportNavigationLayoutTests.cs  # ViewportNavigationLayoutTests 类
│   │   ├── Vulkan/
│   │   │   ├── Backend/
│   │   │   │   └── VulkanBackendInfoTests.cs  # VulkanBackendInfoTests 类
│   │   │   ├── Camera/
│   │   │   │   ├── PerspectiveOrthographicPickingTests.cs  # 投影-反投影-Picking 闭环测试。
│   │   │   │   ├── ProjectionUnprojectionRoundTripTests.cs  # 投影→反投影闭环测试。
│   │   │   │   └── VulkanCameraInfoTests.cs  # VulkanCameraInfoTests 类
│   │   │   ├── Clear/
│   │   │   │   └── VulkanClearInfoTests.cs  # VulkanClearInfoTests 类
│   │   │   ├── Device/
│   │   │   │   └── VulkanDeviceInfoTests.cs  # VulkanDeviceInfoTests 类
│   │   │   ├── Instance/
│   │   │   │   └── VulkanInstanceInfoTests.cs  # VulkanInstanceInfoTests 类
│   │   │   ├── Scene3D/
│   │   │   │   ├── Depth/
│   │   │   │   │   ├── VulkanScene3dDepthAttachmentInfoTests.cs  # VulkanScene3dDepthAttachmentInfoTests 类
│   │   │   │   │   └── VulkanScene3dDepthFormatSelectorTests.cs  # VulkanScene3dDepthFormatSelectorTests 类
│   │   │   │   ├── GroundCursor/
│   │   │   │   │   └── VulkanGroundCursorStateTests.cs  # VulkanGroundCursorStateTests 类
│   │   │   │   ├── VulkanScene3dInfoTests.cs  # VulkanScene3dInfoTests 类
│   │   │   │   ├── VulkanScene3dRunGateTests.cs  # VulkanScene3dRunGateTests 类
│   │   │   │   └── VulkanScene3dVertexTests.cs  # VulkanScene3dVertexTests 类
│   │   │   ├── Shaders/
│   │   │   │   ├── CompiledShadersCollection.cs  # 确保修改 CompiledShaders 静态状态的测试串行执行。
│   │   │   │   └── CompiledShadersTests.cs  # CompiledShadersTests 类
│   │   │   ├── Surface/
│   │   │   │   └── VulkanSurfaceInfoTests.cs  # VulkanSurfaceInfoTests 类
│   │   │   ├── Swapchain/
│   │   │   │   └── VulkanSwapchainInfoTests.cs  # VulkanSwapchainInfoTests 类
│   │   │   └── Validation/
│   │   │       ├── VulkanValidationInfoTests.cs  # VulkanValidationInfoTests 类
│   │   │       ├── VulkanValidationMessageStoreTests.cs  # VulkanValidationMessageStoreTests 类
│   │   │       └── VulkanValidationOptionsTests.cs  # VulkanValidationOptionsTests 类
│   │   └── World/
│   │       └── WorldToRenderSceneBuilderTests.cs  # WorldToRenderSceneBuilderTests 类
│   └── CoreSmokeTests.cs  # CoreSmokeTests 类
```
