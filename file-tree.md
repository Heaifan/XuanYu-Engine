版本：v0.2.17.9-fix
# XuanYu Engine 文件树

文件总数：244

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

- `XuanYu.Core/XuanYu.Core.csproj`：核心类库项目文件。
- `XuanYu.Core/Diagnostics/CoreSelfTest.cs`：Core 自检入口。
- `XuanYu.Core/Identity/EntityId.cs`：实体 ID 值对象。
- `XuanYu.Core/Logging/EngineLogEntry.cs`：引擎日志条目。
- `XuanYu.Core/Logging/EngineLogLevel.cs`：引擎日志等级。
- `XuanYu.Core/Math/Vector3d.cs`：三维向量值对象。
- `XuanYu.Core/Math/YawRotation.cs`：Yaw 旋转值对象。
- `XuanYu.Core/Space/CameraState.cs`：渲染后端无关的相机状态契约；负责校验位置、方向、Up、FOV、裁剪面和 Revision，不负责渲染资源、输入事件或 Picking 命中。
- `XuanYu.Core/Space/ViewportState.cs`：渲染后端无关的视口状态契约；负责记录逻辑区域、物理尺寸、DPI 和 Revision，不等同于 Vulkan Swapchain。
- `XuanYu.Core/Space/ViewProjectionState.cs`：统一观察事实构建器；负责从 Camera / Viewport 生成 View、Projection、ViewProjection 和逆矩阵，不负责实体筛选或空间索引。
- `XuanYu.Core/Space/WorldRay.cs`：世界射线值对象；负责保存有限 Origin 和归一化 Direction，不负责命中测试或实体选择。
- `XuanYu.Core/Space/WorldRayFactory.cs`：视口点到世界射线的转换入口；负责 NDC 与逆矩阵反投影，不负责 Ray-AABB、Picking、Selection 或 Gizmo。
- `XuanYu.Core/Scene/CommittedTransform.cs`：已提交 Transform 值对象，当前保存正式 Position。
- `XuanYu.Core/Scene/ISceneRenderSnapshotSource.cs`：场景渲染快照源抽象，向渲染侧发布只读快照。
- `XuanYu.Core/Scene/SceneEntitySnapshot.cs`：最小场景实体快照，包含 EntityKey、名称、类型和 Transform。
- `XuanYu.Core/Scene/SceneRenderSnapshot.cs`：渲染侧消费的场景快照，当前包含单个最小实体。
- `XuanYu.Core/Scene/SceneStateOwner.cs`：场景状态所有者，负责提交 Position 并发布渲染快照。
- `XuanYu.Core/Results/EngineError.cs`：引擎错误值对象。
- `XuanYu.Core/Results/EngineResult.cs`：引擎结果类型。
- `XuanYu.Core/Time/SimulationTime.cs`：模拟时间值对象。
- `XuanYu.Core/Time/TimeStep.cs`：时间步长值对象。

## XuanYu.Core.Tests

- `XuanYu.Core.Tests/XuanYu.Core.Tests.csproj`：Core 长期自动测试宿主项目文件；只负责引用测试依赖和 `XuanYu.Core`，不向生产项目传递测试依赖或工具链。
- `XuanYu.Core.Tests/CoreSmokeTests.cs`：Core 测试宿主最小烟雾测试；验证测试发现、执行链路和基础 Core 行为，不负责 R2-B 空间数学覆盖。
- `XuanYu.Core.Tests/Space/CameraStateTests.cs`：CameraState 自动测试；负责合法相机、退化方向、共线 Up、非法 FOV / Near / Far / 非有限数覆盖，不负责渲染画面验收。
- `XuanYu.Core.Tests/Space/SpaceAssert.cs`：空间数学测试辅助断言；只负责局部近似比较，不进入生产项目。
- `XuanYu.Core.Tests/Space/ViewportStateTests.cs`：ViewportState 自动测试；负责合法尺寸、DPI、Revision、幂等和非法尺寸覆盖，不负责平台窗口尺寸同步。
- `XuanYu.Core.Tests/Space/ViewProjectionStateTests.cs`：ViewProjectionState 自动测试；负责已知 View 矩阵、投影宽高比和矩阵可逆性覆盖，不负责 Vulkan 投影落地。
- `XuanYu.Core.Tests/Space/WorldRayFactoryTests.cs`：WorldRay 自动测试；负责中心点、角落、Resize、稳定复现和非法输入覆盖，不负责实体 Picking。
- `XuanYu.Core.Tests/Space/WorldRayTests.cs`：WorldRay 值对象自动测试；负责非法 Origin / Direction 失败边界，不负责射线命中或空间查询。

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
- `XuanYu.Render.Vulkan/Pipeline/VulkanShaderModuleOwner.cs`：Vulkan ShaderModule 生命周期持有者。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameLogFormatter.cs`：Vulkan ClearFrame 日志格式化器。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Commands.cs`：Vulkan ClearFrame 命令录制分部。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Lifecycle.cs`：Vulkan ClearFrame 生命周期分部。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.Resources.cs`：Vulkan ClearFrame 资源创建分部。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs`：Vulkan ClearFrame 资源持有主体。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.Frame.cs`：Vulkan Present 单帧执行分部。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.Lifecycle.cs`：Vulkan Present 泵生命周期分部。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs`：Vulkan Present 泵主体。
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
- `XuanYu.Editor.UI/Vm/EditorTreeNode.cs`：编辑器树节点模型。
- `XuanYu.Editor.UI/Vm/LogEntry.cs`：编辑器日志条目模型。
- `XuanYu.Editor.UI/Vm/SampleLogEntries.cs`：底部日志栏示例数据。
- `XuanYu.Editor.UI/Vm/UiText.cs`：静态中文 UI 文案与示例数据。
- `XuanYu.Editor.UI/Vm/UiVm.Interaction.cs`：UiVm 交互事务入口分部。
- `XuanYu.Editor.UI/Vm/UiVm.InteractionPointer.cs`：UiVm Pointer 交互转换分部。
- `XuanYu.Editor.UI/Vm/UiVm.Logging.cs`：UiVm 日志绑定与日志入口分部。
- `XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs`：UiVm NativeHost 生命周期日志分部。
- `XuanYu.Editor.UI/Vm/UiVm.Scene.cs`：UiVm 场景命令分部，提交 R1 测试实体 Position 并刷新调试对象信息。
- `XuanYu.Editor.UI/Vm/UiVm.Selection.cs`：UiVm 选择提交与清空分部。
- `XuanYu.Editor.UI/Vm/UiVm.Tool.cs`：UiVm 工具切换分部。
- `XuanYu.Editor.UI/Vm/UiVm.cs`：UiVm 主体与 UI 绑定状态。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogBuffer.cs`：编辑器内存日志缓冲区。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogBus.cs`：编辑器低频日志入口。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogClipboardText.cs`：日志复制文本格式化器。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogFilter.cs`：日志过滤枚举与中文映射。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogFilterQuery.cs`：日志过滤匹配规则。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogRepeatKey.cs`：重复日志折叠键。
- `XuanYu.Editor.UI/Vm/Logging/EditorLogSummary.cs`：日志摘要统计。
- `XuanYu.Editor.UI/Win/UiWin.axaml`：Avalonia 主窗口界面定义。
- `XuanYu.Editor.UI/Win/UiWin.axaml.cs`：Avalonia 主窗口代码后置。
