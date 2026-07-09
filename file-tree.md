# 项目文件树 — XuanYu Engine

## 视口 UI 收口快照 (2026-07-09)
移除视口内部 overlay，只留纯 Vulkan 视口。
- `XuanYu.Editor.UI/Main/Main.axaml` 20→6：移除 Grid 顶部（透视 / NativeHost Probe）与底部（左键选择 / 中键环绕 / 右键平移 / 工具：选择）两组 pill，内容简化为 `<local:VulkanViewport/>`；`x:DataType` 移除（绑定已删）。`VulkanViewport` 交互逻辑未动。
- 同轮：`RZ-VK5-A-R1` 关闭释放顺序静态验证通过（无代码改动）。

## RZ-VK5-A 实装快照 (2026-07-09)
在 VK4-D Clear+Present 闭环上新增最小 Graphics Pipeline 创建/释放能力（不 Draw、不画三角形）。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.Vert.cs` 25：内嵌顶点 SPIR-V `uint[]`（glslangValidator 本地编译，passthrough，entry main）。
- `XuanYu.Render.Vulkan/Pipeline/ShaderBytecode.Frag.cs` 18：内嵌片元 SPIR-V `uint[]`（输出固定色，entry main）。
- `XuanYu.Render.Vulkan/Pipeline/VulkanShaderModuleOwner.cs` 29：`unsafe` 助手，用 `uint[]` 建/销 vert+frag 两个 ShaderModule。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.cs` 96：建空 PipelineLayout + 绑 RenderPass 的 GraphicsPipeline（动态 viewport/scissor、空 vertex input、TriangleList）；建 Pipeline 后立即释放 ShaderModule（短生命周期）；Dispose 释放 Pipeline→Layout。
- `XuanYu.Render.Vulkan/Pipeline/VulkanPipelineLogFormatter.cs` 13：中文日志格式器（经 `Action<string> log`，日志单出口）。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs` 93→94：+1 只读 getter `RenderPass => _renderPass`。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 59→63：+pipeline 字段；`Create` 中 ClearFrame 之后建 Pipeline；`Dispose` 中最先释放 Pipeline。

## RZ-VK5-A-Plan 文档快照 (2026-07-09)
本轮仅新增规划文档，不改任何代码（`.cs` / `.axaml` / `.csproj` 均未动）。
- `docs/rz-vk5-a-plan.md`  # RZ-VK5-A 规划：在 VK4-D Clear+Present 闭环上接入 ShaderModule + PipelineLayout + GraphicsPipeline 最小方案（只规划不实装）。10 项输出：当前 Vulkan 文件职责 / VK5-A 新增(4 文件 `Pipeline/`)+修改(ClearFrameOwner +1 RenderPass getter、RenderSession +pipeline 接线)清单 / ShaderModule·PipelineLayout·GraphicsPipeline 创建释放顺序 / RenderPass·Swapchain·Framebuffer·Pipeline 依赖（RenderPass 构造时建一次、Resize 不重建→Pipeline Resize 稳定）/ ≤100 拆分 / 禁止事项 / 验收 / 风险与回滚；3 决策点（内嵌 SPIR-V byte[]、动态 viewport-scissor、ShaderModule 持有到会话结束）。关键结论：PresentLoop 提交 ClearFrameOwner 录好的 CommandBuffer，VK5-A/B 加 BindPipeline+Draw 零改动 PresentLoop。

## VK4-Closure + VK5-Plan 文档快照 (2026-07-09)
本轮仅新增文档，不改任何代码（`.cs` / `.axaml` / `.csproj` 均未动）。
- `docs/rz-vk4-closure.md`  # VK4 阶段正式收口确认：VK4-A/B/C/D + VIEWPORT-RESIZE-R2 逐项收口表、已验证清单、跨阶段长期硬规则、已知债务、下一阶段指向 VK5。
- `docs/rz-vk5-plan.md`  # VK5 最小几何渲染规划（只规划不实装）：VK5-A Shader+Pipeline / VK5-B 固定三角形（gl_VertexIndex，暂不建 VertexBuffer）/ VK5-C Resize 兼容 / VK5-D 渲染命令边界；12 条红线、资源创建/释放顺序、文件结构、SVG 规划图。

## VIEWPORT-RESIZE-R2 快照 (2026-07-09)
VIEWPORT-RESIZE-R2（Editor.UI 侧）已收口：修复 R1 的 DPI 错配——`SyncFinalSize` 把 Avalonia 逻辑尺寸 ×DPI 换算成物理像素再喂 `Win32ViewportHost.Resize`，`_bridge.Resize` 仍收逻辑尺寸；探针补「目标物理」字段。全部 ≤100 行，双项目 0W0E。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs` 98：`partial`；`OnSizeChanged`/`Coalescer` 路径原样保留（拖动窗口仍走它）。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.LayoutSync.cs` 38→49：`SyncFinalSize` 先 `physicalW=max(1,round(logicalW*GetDpiScale()))`、`physicalH=max(1,round(logicalH*GetDpiScale()))`；`Win32ViewportHost.Resize(_hwnd, physicalW, physicalH)`（物理像素）；`_bridge.Resize(logicalW, logicalH)`（逻辑）；`GetClientSize` 取实际物理供探针。
- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs` 67：`Resize` 裸 `SetWindowPos` 不乘 DPI（收物理像素）；`GetClientSize(hwnd)` 取子窗口物理像素。
- `XuanYu.Editor.UI/ViewportNativeHostRoute.cs` 18：`ReportProbe(vm, open, logicalW, logicalH, dpi, targetW, targetH, clientW, clientH)`。
- `XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs` 38：`LogNativeHostProbe(... dpi, targetW, targetH, clientW, clientH)` 输出 目标物理 + 子窗口实际。
VK4-D-R3（Render.Vulkan 侧）已收口：修改 6 个 Render.Vulkan 文件，全部 ≤100 行，双项目 0W0E。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs` 99：OutOfDate 优雅降级（`ErrorOutOfDateKhr` 仅记一次 `OutOfDatePaused()` 后 break，不刷屏）；`Start()` 重置 `_outOfDateLogged`；`Stop()` 局部捕获线程防 NRE。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 59：Resize 日志顺序收口（Rebuilt 在 Start 前，用 `_swapchainOwner.Extent` 物理像素）。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainCapabilities.cs` 81：能力日志打印请求逻辑尺寸 + Surface CurrentExtent（物理像素）+ 选择 extent（物理像素）。
VK4-D-R3（Render.Vulkan 侧）已收口：修改 6 个 Render.Vulkan 文件，全部 ≤100 行，双项目 0W0E。
- `XuanYu.Render.Vulkan/Render/VulkanPresentLoop.cs` 92→99：OutOfDate 优雅降级（`ErrorOutOfDateKhr` 仅记一次 `OutOfDatePaused()` 后 break，不刷屏）；`Start()` 重置 `_outOfDateLogged`；`Stop()` 局部捕获线程防 NRE。
- `XuanYu.Render.Vulkan/Session/VulkanRenderSession.cs` 59：Resize 日志顺序收口（Rebuilt 在 Start 前，用 `_swapchainOwner.Extent` 物理像素）。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainCapabilities.cs` 80→81：能力日志打印请求逻辑尺寸 + Surface CurrentExtent（物理像素）+ 选择 extent（物理像素）。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainLogFormatter.cs` 13→15：`Created`/`Recreated` 收 `Extent2D` 打印物理像素。
- `XuanYu.Render.Vulkan/Render/VulkanClearFrameLogFormatter.cs` 14→17：`Rebuilt(Extent2D, uint)` 打印物理像素；新增 `OutOfDatePaused()`。
- `XuanYu.Render.Vulkan/Swapchain/VulkanSwapchainOwner.cs` 88：`Created`/`Recreated` 改传实际 `_extent`。

最近更新：VK3 已收口（验收通过，2026-07-08）：新增 docs/rz-vk3-closure.md（收口确认）+ docs/rz-vk4-plan.md（VK4 规划，只规划不实装）；VK4 目标为最小渲染闭环（PhysicalDevice→LogicalDevice→Queue→Swapchain→ClearFrame→RenderSession），红线：Resize 不重建 Surface、不搬探针、UI 不持 Vulkan、每步 5+100。VK3 既有链路（VK3-A..VK3-C2-R1）不变。VK4-A 已实装（2026-07-08）：在 Instance+Surface 之后新增 PhysicalDevice 选择链路（XuanYu.Render.Vulkan/Device/ 下 3 个新文件，桥 Attach 后调用，仅枚举/选择/中文日志，未创建设备/队列/Swapchain）。VK4-A-R1 已收口（2026-07-08）：VK4-A 后 VulkanNativeHostSurfaceBridge.cs 由 93→110 行越过 100 行红线，已将内联选择逻辑迁出至 XuanYu.Render.Vulkan/Bridge/VulkanBridgePhysicalDeviceAttachStep.cs（23 行），Bridge 压回 96 行，仅保留生命周期编排；行为不变。VK4-B 已实装（2026-07-08）：基于 VK4-A 选择结果新增 LogicalDevice + Graphics/Present 队列（`Device/VulkanPhysicalDeviceSelection.cs` 抽为独立文件并补 PhysicalDevice 句柄、`Device/VulkanDeviceOwner.cs` 创建 VkDevice+队列、`Bridge/VulkanBridgeDeviceAttachStep.cs` 在 Instance+Surface 就绪后链式驱动创建设备）；未建 Swapchain/ImageView/RenderPass/CommandBuffer、未清屏/Present、UI 未接触 Silk.NET.Vulkan 类型；Bridge Detach 逆序释放 Device→Surface→Instance。VK4-C 已实装（2026-07-08）：在 VK4-B 的 LogicalDevice+Queue 之后新增 Swapchain + Swapchain Images + ImageViews 链路（`XuanYu.Render.Vulkan/Swapchain/` 下 4 文件：VulkanSwapchainCapabilities 80 / VulkanSwapchainBuilder 74 / VulkanSwapchainOwner 86 / VulkanSwapchainLogFormatter 13；`Bridge/VulkanBridgeSwapchainAttachStep.cs` 32 在设备 step 后链式驱动；均 ≤100 行）；Bridge 改写 98 行（接近 100 红线不再膨胀，仅编排生命周期，Swapchain 逻辑全在独立 owner/step）；Resize 只重建 Swapchain+ImageViews（不重建 Surface/Instance/Device/Queue）；Dispose 顺序 ImageViews→Swapchain→LogicalDevice→Surface→Instance；红线守住：未建 RenderPass/Framebuffer/CommandPool/CommandBuffer、未 Clear/Present（仍黑屏为预期）、UI 零改动不接触 Silk.NET.Vulkan、未复制 VulkanClearSession 旧探针；两项目 0W0E。VK4-C 已补运行前置修正（2026-07-08）：`VulkanDeviceOwner` 创建设备时启用 `VK_KHR_swapchain` 设备扩展（扩展名由 `VulkanSwapchainOwner.DeviceExtensionName` 传入，DeviceOwner 96→99 行）、`VulkanSwapchainOwner.Recreate` 加 0 尺寸跳过（77→86 行）、暴露 `Format`/`Extent`/`ImageViews` 只读供 VK4-D；均 ≤100，仍不出画面。状态：VK4-C 代码完成，待 VK4-C-R1 真机运行验证，未完全收口；VK4-D 已实装（D1+D2+D3+VK4-D-R1 修复），VK4-D-R2（2026-07-09）修复 Present 泵后台线程日志回调线程派发导致的闪退——VulkanNativeHost 新增 ReportVulkanMessage / ReportVulkanMessageOnUiThread 经 Dispatcher.UIThread 切回 UI 线程访问 DataContext / UiVm；VulkanPresentLoop.Log 加 try/catch 防御；双项目 0W0E，VulkanNativeHost 95 / VulkanPresentLoop 96 行；VK4-D 仍待真机验收。

统计口径：当前工作区真实文件快照，排除 `.git/`、`bin/`、`obj/` 生成目录。

当前文件总数：118

```

## RZ-VK3-A-R1 / RZ-VK3-A Surface 契约层快照 (2026-07-08)

新增独立契约工程 `XuanYu.Render.Abstractions`（net10.0，零 Silk.NET / Avalonia / Editor.Win / Render.Vulkan 引用），承载 UI 与 Vulkan 之间的纯窗口交接通道。

### XuanYu.Render.Abstractions/
- `XuanYu.Render.Abstractions.csproj`  # 纯契约工程，无包引用。
- `NativeHostSurfaceHandle.cs`  # HWND / Hinstance / 尺寸 / DPI 交接句柄（VK3-A）。
- `INativeHostSurfaceBridge.cs`  # Attach / Resize / Detach 交接契约接口（VK3-A）。
- `NativeHostLifecycleState.cs`  # 生命周期状态枚举（VK3-A-R1 从 Render.Vulkan 迁入）。
- `NativeHostHandleSnapshot.cs`  # 生命周期快照记录（VK3-A-R1 从 Render.Vulkan 迁入）。
- `NativeHostLifecycleProbe.cs`  # 生命周期探针（VK3-A-R1 从 Render.Vulkan 迁入）。
- `NativeHostLifecycleLogFormatter.cs`  # 中文生命周期日志格式器（VK3-A-R1 从 Render.Vulkan 迁入）。

### XuanYu.Render.Vulkan/（已移除 4 个生命周期类型）
- 删除 `NativeHostLifecycleState.cs` / `NativeHostHandleSnapshot.cs` / `NativeHostLifecycleProbe.cs` / `NativeHostLifecycleLogFormatter.cs`（迁入 Abstractions）。
- 保留 `VulkanApiProbe.cs` / `VulkanProbeResult.cs` / `VulkanProbeLogFormatter.cs` / `VulkanDeviceInfo.cs`（Vulkan 环境探针，仍属 Render.Vulkan）。
- 新增 `VulkanInstanceOwner.cs` / `VulkanInstanceLogFormatter.cs` / `VulkanInstanceResult.cs`（VK3-B1：Vulkan Instance 持有者；VK3-B1-R1 将 VulkanInstanceOwner 由 98 行拆至 66 行，抽出 `VulkanInstanceExtensions.cs` 最小扩展名集合与 `VulkanInstanceCreateInfoBuilder.cs` InstanceCreateInfo 构造；仍仅创建/释放 Instance 并启用 VK_KHR_surface + VK_KHR_win32_surface；Dispose 幂等且释放后清空句柄；中文生命周期日志；禁止 Surface / PhysicalDevice / LogicalDevice / Queue / Swapchain / RenderFrame）。
- 新增 `VulkanInstanceExtensions.cs` / `VulkanInstanceCreateInfoBuilder.cs`（VK3-B1-R1：前者存 Instance 启用的最小扩展名集合，后者在 fixed 作用域内构造 InstanceCreateInfo 并交给回调，均不直接调用 Vulkan）。
- 新增 `VulkanSurfaceOwner.cs` / `VulkanSurfaceResult.cs` / `VulkanSurfaceLogFormatter.cs`（VK3-B2：Vulkan Surface 持有者，仅创建/释放 VkSurfaceKHR-Win32，生命周期绑定 NativeHost Attach/Detach，不绑定 Resize；创建经 KhrWin32Surface.CreateWin32Surface，销毁经 KhrSurface.DestroySurface；取 NativeHostSurfaceHandle 的 Hwnd/Hinstance；Dispose 幂等且释放后清空句柄；中文生命周期日志；禁止 PhysicalDevice/LogicalDevice/Queue/Swapchain/RenderFrame）。
- 新增 `VulkanNativeHostSurfaceBridge.cs` / `VulkanBridgeLogFormatter.cs`（VK3-C1：前者实现 INativeHostSurfaceBridge，Attach 经 VulkanInstanceOwner.Create + VulkanSurfaceOwner.Create 串起 Instance+Surface，Detach/Dispose 先释放 Surface 再释放 Instance，Resize 只记日志不重建 Surface；后者为纯中文生命周期日志格式器；均不接 UI 组合根、不碰 PhysicalDevice/LogicalDevice/Queue/Swapchain/RenderFrame）。
- 新增 `Device/VulkanPhysicalDeviceInfo.cs` / `Device/VulkanQueueFamilySelection.cs` / `Device/VulkanPhysicalDeviceSelector.cs`（VK4-A：PhysicalDevice 选择链路，落在 `XuanYu.Render.Vulkan/Device/` 子目录；仅枚举设备、检查 Graphics/Present 队列族与 Surface 呈现支持、优先独显、返回纯数据结果（`VulkanPhysicalDeviceInfo` / `VulkanQueueFamilySelection` / `VulkanPhysicalDeviceSelection`）、输出中文日志；`VulkanNativeHostSurfaceBridge.Attach` 在 Instance+Surface 就绪后调用选择 step；红线：未创建 LogicalDevice/Queue/Swapchain/ImageView、未清屏/Present、未让 UI 引用 Vulkan 类型、未复制旧探针 `VulkanClearSession`、新增文件均 ≤100 行）。
- VK4-B 抽出 `Device/VulkanPhysicalDeviceSelection.cs`（12 行，原内联在选择器末尾的记录独立成文件，并补 `PhysicalDevice Handle` 字段供 VK4-B 复用、不泄漏 UI）；新增 `Device/VulkanDeviceOwner.cs`（99 行，基于 `VulkanPhysicalDeviceSelection` 创建 VkDevice 与 Graphics/Present 队列、Dispose 幂等释放；输出中文日志；VK4-C 修正启用 VK_KHR_swapchain 设备扩展）；`VulkanPhysicalDeviceSelector.cs` 由 99→93 行（移除内联记录、捕获 `bestDevice` 随结果返回句柄），仅负责枚举与选择。
- 新增 `Bridge/VulkanBridgePhysicalDeviceAttachStep.cs`（VK4-A-R1：在 Instance+Surface 就绪后调用 `VulkanPhysicalDeviceSelector.Select`、把选择结果（现返回 `VulkanPhysicalDeviceSelection?`）与中文日志写入面板的薄委托层；选择异常仅记日志、不影响已附加的 Instance+Surface；`VulkanNativeHostSurfaceBridge` 不再内联选择逻辑，仅保留生命周期编排，已压回 96 行；目录 `Bridge/` 现 2 文件，未越过 5-7 文件上限）。
- VK4-B 新增 `Bridge/VulkanBridgeDeviceAttachStep.cs`（29 行，在 VK4-A 选择成功后基于 `VulkanPhysicalDeviceSelection` 调用 `VulkanDeviceOwner.Create` 创建 LogicalDevice；选择失败（`sel` 为 null 或 `!Success`）则跳过、异常仅记日志，不影响已附加的 Instance+Surface+已选中设备）。`VulkanNativeHostSurfaceBridge.Attach` 现链式执行「选择 step → 设备 step」，Detach 逆序释放 Device→Surface→Instance。
- VK4-C 新增 `Swapchain/` 子目录（4 文件）：`VulkanSwapchainCapabilities.cs`（80 行，查 Surface caps/formats/present modes/extent 并输出纯数据 `SwapchainCaps`/`VulkanSwapchainCapabilitiesResult`）、`VulkanSwapchainBuilder.cs`（75 行，串 Query→CreateSwapchain→GetSwapchainImages→CreateImageViews）、`VulkanSwapchainOwner.cs`（86 行，经 `vk.TryGetDeviceExtension(instance, deviceOwner.LogicalDevice, out KhrSwapchain? khr)` 创建 Swapchain+Images+ImageViews，`Recreate(width,height)` 只重建（含 0 尺寸跳过）、`Dispose` 先 ImageView 后 Swapchain，暴露 `Format`/`Extent`/`ImageViews` 只读供 VK4-D）、`VulkanSwapchainLogFormatter.cs`（13 行，中文生命周期日志）。均不建 RenderPass/Framebuffer/CommandPool/CommandBuffer、不 Clear/Present。
- VK4-C 新增 `Bridge/VulkanBridgeSwapchainAttachStep.cs`（32 行，在设备 step 后链式驱动 `VulkanSwapchainOwner.Create`；前置 null/Success 检查跳过、异常仅记日志）。`VulkanNativeHostSurfaceBridge.cs`（VK4-C 改写 98 行，接近 100 红线）`Attach` 串「选择→设备→Swapchain」、`Resize` 转发 `_swapchainOwner?.Recreate(width,height)`、`Detach` 首行 `_swapchainOwner?.Dispose()`、`Emit` 迁入 `VulkanBridgeLogFormatter.Emit`；`VulkanBridgeLogFormatter.cs`（VK4-C 改写 35 行，新增 `Emit(Action<string>?, string)` 统一日志出口）。
- VK4-D 实装（D1+D2+D3 同轮）+ VK4-D-R1 审计修复：新增 `Render/` 子目录——`VulkanClearFrameLogFormatter.cs`（14 行，中文清屏日志 + 首帧 Present 成功日志）、`VulkanClearFrameOwner.cs`（93 行：RenderPass+CommandPool+CommandBuffer[] 每 Swapchain 图像一张静态 clear 录制+Framebuffer[]，clear 颜色 0.25/0.45/0.70 明显蓝灰，Resize 只重建 Framebuffer+重录）、`VulkanPresentLoop.cs`（92 行：独立后台线程跑 Acquire→Submit→Present 单帧闭环、单 in-flight 帧+单栅栏、`_submitted` 守卫避免首帧 WaitForFences 空等、把 `SuboptimalKhr` 当成功码处理、Acquire/Present 失败记录中文错误、首帧 Present 成功只打印一次、Detach/Resize 先 Stop 再重建；`using Semaphore = Silk.NET.Vulkan.Semaphore` 消歧义）；`Session/VulkanRenderSession.cs`（59 行薄组合根，只装配 ClearFrame+PresentLoop，Resize 统一负责 Stop→Swapchain 重建→Framebuffer 重建→Start）；`Bridge/VulkanBridgeRenderSessionAttachStep.cs`（15 行，Bridge 仅委托）。`VulkanSwapchainOwner.cs` 补 `Swapchain`/`Khr` 只读 getter（88 行）。`VulkanNativeHostSurfaceBridge.cs` 压回 83 行：Attach 经独立 step 创建 `_renderSession`、Resize 仅转发 `_renderSession?.Resize`（不再二次重建 Swapchain）、Detach 首行 `_renderSession?.Dispose()`（顺序 ClearFrame→Swapchain→Device→Surface→Instance）。全 .cs ≤100；双项目 0W0E。

### XuanYu.Editor.UI/（依赖收口）
- `NativeHostSurfaceContract.cs`  # 仅 `using XuanYu.Render.Abstractions`（VK3-A）。
- `NativeHostResizeCoalescer.cs` / `ViewportNativeHostRoute.cs` / `Vm/UiVm.NativeHostLifecycle.cs` / `Viewport/Vulkan/VulkanNativeHost.cs`  # 生命周期链路改用 `using XuanYu.Render.Abstractions`（VK3-A-R1）；VK4-D-R2 新增 ReportVulkanMessage / ReportVulkanMessageOnUiThread，把 Vulkan 日志回调经 Dispatcher.UIThread 切回 UI 线程（后台 Present 泵线程安全）。
- `VulkanProbeRoute.cs` / `Vm/UiVm.VulkanProbe.cs`  # 仍 `using XuanYu.Render.Vulkan`（Vulkan 探针类型），故 Editor.UI.csproj 保留对 Render.Vulkan 的工程级引用。
- `Viewport/Vulkan/VulkanSurfaceBridgeProvider.cs`  # VK3-C2 组合根：装配 INativeHostSurfaceBridge 具体实现，UI 宿主只认 Abstractions 契约（故 Editor.UI.csproj 仍引用 Render.Vulkan）。


XuanYuEngine/
│
├── NuGet.Config  # NuGet 源配置。
├── changelog.md  # 项目变更记录，按时间倒序记录阶段性修改。
├── codex_log_xuanyu_handoff_20260705-2102.zip  # Codex 交接日志压缩包。
├── file-tree.md  # 当前文件树与文件职责说明。
├── run.bat  # Windows 启动脚本。
│
├── docs/
│   ├── AI_DEVELOPMENT_RULES.md  # AI 协作开发规则。
│   ├── CODE_CONSTITUTION.md  # 代码宪法与结构约束。
│   ├── ENGINE_ARCHITECTURE.md  # 引擎架构说明。
│   ├── LEGACY_FLUIDWARFARE_OLD_AUDIT.md  # 旧 FluidWarfare 项目审计记录。
│   ├── MILESTONE1_PUBLIC_VALIDATION.md  # 里程碑 1 公开验证说明。
│   ├── NAMING_RULES.md  # 命名规则。
│   ├── PHASE1_SCOPE.md  # Phase 1 范围定义。
│   ├── PROJECT_CHARTER.md  # 项目章程。
│   ├── audit-EditorShellV2-9.1A-1.md  # EditorShellV2 9.1A 第一轮审计。
│   ├── audit-EditorShellV2-freeze-9.1A-Freeze.md  # EditorShellV2 冻结问题审计。
│   ├── audit-EditorShellV2-input-9.1A-2.md  # EditorShellV2 输入链路审计。
│   ├── audit-EditorShellV2-input-9.1A-2R.md  # EditorShellV2 输入链路复审。
│   ├── audit-EditorShellV2-picking-gizmo-9.1A-3.md  # EditorShellV2 Picking / Gizmo 审计。
│   ├── audit-EditorShellV2-picking-gizmo-9.1A-3R.md  # EditorShellV2 Picking / Gizmo 复审。
│   ├── audit-EditorShellV2-plan-9.1A-0.md  # EditorShellV2 9.1A 审计计划。
│   ├── audit-NativeViewportMouseCapture-lifecycle-9.0X.md  # Native Viewport 鼠标捕获生命周期审计。
│   ├── audit-gizmo-chain-9.0Y-1.md  # Gizmo 链路审计 9.0Y-1。
│   ├── audit-gizmo-chain-9.0Y-2.md  # Gizmo 链路审计 9.0Y-2。
│   ├── audit-gizmo-chain-9.0Y-3.md  # Gizmo 链路审计 9.0Y-3。
│   ├── audit-gizmo-stash-9.0Y-0.md  # Gizmo 暂存状态审计。
│   ├── audit-input-lifecycle-9.0X-1.md  # 输入生命周期审计 9.0X-1。
│   ├── audit-input-lifecycle-9.0X-2.md  # 输入生命周期审计 9.0X-2。
│   ├── audit-input-lifecycle-9.0X-3.md  # 输入生命周期审计 9.0X-3。
│   ├── audit-inspector-transform-9.0C-0.md  # Inspector / Transform 同步审计。
│   ├── diagnostic-safety.md  # 诊断日志、底部日志准入与 UI 调度安全规范。
│   ├── editor-top-area-target-9.1B.md  # 顶部区域目标说明。
│   ├── editor-top-svg-icons-9.1C-R.md  # 顶部 SVG 图标细修说明。
│   ├── editor-top-svg-icons-9.1C.md  # 顶部 SVG 图标替换说明。
│   ├── editor-ui-terms-9.1B.md  # 编辑器 UI 术语说明。
│   ├── gizmo_drag_audit_2026-06-25.md  # Gizmo 拖动审计报告。
│   ├── gizmo_drag_audit_probe.log  # Gizmo 拖动审计探针日志。
│   ├── naming-XuanYu-Engine.md  # XuanYu Engine 命名迁移说明。
│   └── plan-9.0D-move-gizmo-final.md  # Move Gizmo 最终验收计划。
│
├── codex_log/
│   ├── README.md  # Codex 日志目录说明。
│   ├── raw_sessions/
│   │   ├── rollout-2026-06-24T21-20-22-019ef9c9-df07-7dc1-b8e3-ff99f4382fdc.jsonl  # Codex 原始会话日志。
│   │   ├── rollout-2026-06-25T11-37-38-019efcda-b9b4-7201-876a-09bbd3d169f3.jsonl  # Codex 原始会话日志。
│   │   ├── rollout-2026-06-25T22-42-25-019eff3b-4db5-74a1-aadb-0f30d30ddfac.jsonl  # Codex 原始会话日志。
│   │   ├── rollout-2026-06-27T19-35-50-019f08dd-3fff-7b92-a37b-ac0ace4f39a1.jsonl  # Codex 原始会话日志。
│   │   ├── rollout-2026-07-03T11-56-19-019f261e-b69a-7790-9a25-bc9afd0ffbca.jsonl  # Codex 原始会话日志。
│   │   ├── rollout-2026-07-05T11-39-54-019f305c-6407-7f50-95be-51b18d0219a5.jsonl  # Codex 原始会话日志。
│   │   └── rollout-2026-07-05T20-05-18-019f322b-2ff6-7f70-8f17-388a2e5a7e1d.jsonl  # Codex 原始会话日志。
│   └── rollout_summaries/
│       ├── 2026-06-24T13-20-17-hWSo-xuan_yu_engine_log_copy_and_gizmo_hit_test_fix.md  # 日志复制与 Gizmo 命中修复摘要。
│       ├── 2026-06-25T03-37-33-v23F-move_gizmo_cadence_debug_probe_ui_io_fix.md  # Move Gizmo 高频探针与 UI IO 修复摘要。
│       ├── 2026-06-25T14-42-17-Dgjq-gizmo_drag_audit_and_middle_button_capture_fix.md  # Gizmo 拖动审计与中键捕获修复摘要。
│       ├── 2026-06-27T11-35-45-ffSF-editor_move_flow_blue_marker_debug.md  # 编辑器移动流程与蓝色标记调试摘要。
│       └── 2026-07-03T03-56-14-ks8q-xuanyueditor_ui_short_names_layout_skeleton.md  # XuanYu.Editor.UI 短命名布局骨架摘要。
│
├── XuanYu.Core/
│   ├── XuanYu.Core.csproj  # 核心类库项目文件。
│   ├── Diagnostics/
│   │   └── CoreSelfTest.cs  # Core 自检入口。
│   ├── Identity/
│   │   └── EntityId.cs  # 实体 ID 值对象。
│   ├── Logging/
│   │   ├── EngineLogEntry.cs  # 引擎日志条目。
│   │   └── EngineLogLevel.cs  # 引擎日志等级。
│   ├── Math/
│   │   ├── Vector3d.cs  # 三维向量值对象。
│   │   └── YawRotation.cs  # Yaw 旋转值对象。
│   ├── Results/
│   │   ├── EngineError.cs  # 引擎错误值对象。
│   │   └── EngineResult.cs  # 引擎结果类型。
│   └── Time/
│       ├── SimulationTime.cs  # 模拟时间值对象。
│       └── TimeStep.cs  # 时间步长值对象。
│
├── XuanYu.Editor.Win/
│   ├── XuanYu.Editor.Win.csproj  # WinForms 编辑器项目文件。
│   ├── MainForm.cs  # WinForms 主窗口。
│   └── Program.cs  # WinForms 启动入口。
│
└── XuanYu.Editor.UI/
    ├── XuanYu.Editor.UI.csproj  # Avalonia 编辑器 UI 外壳项目。
    ├── RelayCommand.cs  # ICommand 简易实现。
    ├── Ui.axaml  # 全局 UI 样式资源。
    ├── app.manifest  # Windows 应用清单。
    ├── Bootstrap/
    │   ├── App.axaml  # Avalonia 应用资源入口。
    │   ├── App.axaml.cs  # 应用启动与主窗口挂载。
    │   └── Program.cs  # Avalonia 桌面启动入口（含 AttachConsole(-1) 继承父控制台使 Console.WriteLine 可见）。
    ├── Foot/
    │   ├── Foot.axaml  # 底部全局日志栏：摘要、过滤、搜索框、日志列表与右侧详情入口。
    │   ├── LogDetailPanel.axaml  # 日志详情面板：点击选中日志后显示详情并提供复制入口。
    │   ├── Foot.axaml.cs  # 底部栏代码后置。64 行：LOG-UX-2 只做接线——创建 LogListAutoScrollController(LogList)、SelectionChanged 详情选中、Ctrl+A/C 多选复制。自动滚动状态机已拆入 LogListAutoScrollController.cs。
    │   ├── LogListAutoScrollController.cs  # 74 行：LOG-UX-2 独立自动滚动控制器。单次解析 ScrollViewer（TemplateApplied 后 FindDescendantOfType 一次，不每条日志遍历）；节流（_pendingScroll 连续多条只滚一次）；防重入（_isProgrammaticScroll 区分程序/用户滚动）；_followTail 用户上翻暂停、回底恢复（容差 12px）。不碰 Vulkan/UiVm.Logging。
    │   └── LogDetailPanel.axaml.cs  # 日志详情复制按钮与剪贴板桥接。
    ├── Icons/
    │   └── EditorIcons.axaml  # SVG / PathData 图标集中资源。
    ├── Left/
    │   ├── Left.axaml  # 左侧项目 / 层级页签。
    │   └── Left.axaml.cs  # 左侧面板代码后置。
    ├── Main/
    │   ├── Main.axaml  # 中央深色视口占位。
    │   └── Main.axaml.cs  # 中央视口代码后置。
    ├── Right/
    │   ├── Right.axaml  # 右侧检查器 / 调试 / 偏好 / 模式页签，调试页只显示状态快照。
    │   └── Right.axaml.cs  # 右侧面板代码后置。
    ├── Root/
    │   ├── UiRoot.axaml  # 主布局：顶部、左侧、视口、右侧、底部。
    │   └── UiRoot.axaml.cs  # 主布局代码后置。
    ├── Top/
    │   ├── Top.axaml  # 顶部两行工具区：主命令分组、编辑工具分组、状态与当前工具。
    │   └── Top.axaml.cs  # 顶部工具栏代码后置。
    ├── Vm/
    │   ├── DebugText.cs  # 右侧调试页状态快照示例数据。
    │   ├── EditorLogCategory.cs  # 编辑器日志分类枚举。
    │   ├── EditorLogLevel.cs  # 编辑器日志等级枚举。
    │   ├── EditorLogSource.cs  # 编辑器日志来源枚举。
    │   ├── LogEntry.cs  # 编辑器日志条目模型。
    │   ├── SampleLogEntries.cs  # 底部日志栏示例数据。
    │   ├── UiText.cs  # 静态中文 UI 文案与检查器示例数据。
    │   ├── UiVm.Logging.cs  # UI ViewModel 的日志总线、Buffer、过滤绑定与低频日志入口。
    │   ├── UiVm.cs  # UI ViewModel：工具选择、状态、日志展开、检查器/日志/调试绑定。
    │   └── Logging/
    │       ├── EditorLogBuffer.cs  # 编辑器内存日志缓冲区，最多保留最近 500 条并合并连续重复。
    │       ├── EditorLogBus.cs  # 编辑器低频日志入口：Info / Warning / Error。
    │       ├── EditorLogClipboardText.cs  # 单条日志详情的结构化中文复制文本格式化。
    │       ├── EditorLogFilter.cs  # 底部日志过滤枚举与中文按钮映射。
    │       ├── EditorLogFilterQuery.cs  # 日志过滤匹配规则。
    │       ├── EditorLogRepeatKey.cs  # 重复日志折叠键。
    │       └── EditorLogSummary.cs  # 日志摘要计算：错误数、警告数、最近事件。
    └── Win/
        ├── UiWin.axaml  # Avalonia 主窗口壳。
        └── UiWin.axaml.cs  # 主窗口代码后置。
```
# 项目文件树 — XuanYu Engine

最近更新：RZ-Fix3-A 新增中央 Vulkan Host 前置验证，接入 NativeControlHost、Win32 Surface、Device 与 Swapchain 生命周期；本轮新增 `XuanYu.Editor.UI/Viewport/Vulkan/`。

统计口径：当前工作区真实文件快照，排除 `.git/`、`bin/`、`obj/`、`artifacts/` 生成目录。

当前文件总数：99

新增 Vulkan 视口文件：
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.Device.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.Swapchain.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.Dispose.cs`

## RZ-VK2 / RZ-VK1 current snapshot (2026-07-07)
Current file count: 117
New Vulkan / NativeHost files:
- XuanYu.Render.Vulkan/XuanYu.Render.Vulkan.csproj
- XuanYu.Render.Vulkan/VulkanApiProbe.cs
- XuanYu.Render.Vulkan/VulkanDeviceInfo.cs
- XuanYu.Render.Vulkan/VulkanProbeLogFormatter.cs
- XuanYu.Render.Vulkan/VulkanProbeResult.cs
- XuanYu.Render.Vulkan/NativeHostHandleSnapshot.cs
- XuanYu.Render.Vulkan/NativeHostLifecycleState.cs
- XuanYu.Render.Vulkan/NativeHostLifecycleProbe.cs
- XuanYu.Render.Vulkan/NativeHostLifecycleLogFormatter.cs
- XuanYu.Editor.UI/VulkanProbeRoute.cs
- XuanYu.Editor.UI/ViewportNativeHostRoute.cs
- XuanYu.Editor.UI/Vm/UiVm.VulkanProbe.cs
- XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs
- docs/audit-RZ-VK1-vulkan-probe.md
- docs/audit-RZ-VK2-native-host-lifecycle.md

## 开发规范理解文档补充 (2026-07-07)
新增开发规范两份（由 AI 接手理解验收提炼，经人工校正 5+100 / 依赖隔离 / 日志边界 / VK 阶段边界表述）：
- `docs/dev-rules.md`  # 开发硬规则执行手册：接手红线清单 + 5+100 + 依赖方向硬隔离 + 高频链路纪律 + 日志边界 + VK 阶段边界 + 中文化 + 范围结构 + 命名品牌 + 构建测试审计门禁。
- `docs/dev-rules-understanding.md`  # 开发规范「为什么这样规定」：事故来源、设计动机、历史坑、ShellV2 冻结 / Gizmo 卡顿 / Vulkan 生命周期教训、常见误读速查。

## RZ-VK2-R1 NativeHost Resize 日志合并 (2026-07-07)
结构性新增（已同步）：
- `XuanYu.Editor.UI/NativeHostResizeSnapshot.cs`  # 尺寸变化快照，只保存尺寸数据。
- `XuanYu.Editor.UI/NativeHostResizeCoalescer.cs`  # 250ms debounce，合并连续尺寸变化，稳定后生成一条低频合并日志；Detach/Dispose 安全停止 pending。
- `docs/audit-RZ-VK2-R1-nativehost-resize-coalesce.md`  # RZ-VK2-R1 审计与验收文档。
- `docs/audit-RZ-VK2-R2-nativehost-resize-coalesce-verify.md`  # RZ-VK2-R2 验证/收口轮：确认合并边界干净、未牵连 Vulkan 生命周期。
- `docs/rz-vk3-surface-lifecycle-plan.md`  # RZ-VK3-Plan：正式 VK3 Surface 生命周期规划（只规划不写实装）。
修改（职责收口，未改布局/输入）：
- `XuanYu.Editor.UI/ViewportNativeHostRoute.cs`  # 增加 ReportMerged 薄入口。
- `XuanYu.Render.Vulkan/NativeHostLifecycleLogFormatter.cs`  # 增加 MergedMessage 中文合并日志格式。
- `XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs`  # 增加 LogNativeHostResizedMerged。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`  # OnSizeChanged 走 Coalescer；Detach/Dispose 调 Cancel。
- `XuanYu.Editor.UI/Main/Main.axaml` 与 `XuanYu.Editor.UI/Viewport/Vulkan/VulkanViewport.axaml`  # 视口文案 Vulkan Clear Probe 改为 NativeHost Probe / Vulkan Probe。

## RZ-New-0 接手验收审计 (2026-07-07)
- `docs/audit-RZ-New-0-onboarding.md`  # RZ-New-0 接手验收审计：10 项清单、真实状态（含 Editor.UI 直接引用 Vulkan 过渡债务与探针已超 VK3 的发现）。
- `docs/dev-rules.md` / `docs/dev-rules-understanding.md`  # 开发规范执行手册与解释（登记见「开发规范理解文档补充」一节）。

## VK3 收口 + VK4 规划文档 (2026-07-08)
- `docs/rz-vk3-closure.md`  # VK3 收口确认：验收项表格、已完成阶段（VK3-A..VK3-C2-R1）、红线遵守确认、已知债务（UI 对 Render.Vulkan 工程级引用移交 VK4）、收口日期。VK3 结论：NativeHost HWND 生命周期已正式接入 Vulkan Instance + Surface；Swapchain 留 VK4。
- `docs/rz-vk4-plan.md`  # VK4 规划（只规划不实装）：最小渲染闭环 PhysicalDevice→LogicalDevice→Queue→Swapchain→ClearFrame→RenderSession 五问规划、目标依赖方向、阶段分解 VK4-A..VK4-E、防回潮门禁（Resize 不重建 Surface、不搬探针、UI 不持 Vulkan、每步 5+100）。
- `docs/rz-vk4-c-swapchain-plan.md`  # VK4-C-Plan（2026-07-08，只规划不实装）：Swapchain + Images + ImageViews 生命周期规划；边界（不建 RenderPass/Framebuffer/CommandPool/CommandBuffer、不 Clear/Present）、Resize 只重建 Swapchain+ImageViews、Dispose 顺序 ImageViews→Swapchain→LogicalDevice→Surface→Instance、独立 owner/attach step 文件结构、命名与 100 行红线。
- `docs/rz-vk4-c-r1-audit-plan.md`  # VK4-C-R1 审计与运行验证计划（2026-07-08）：只审计不新增能力、不进 VK4-D；静态审计结论 + 运行验证清单（14 项）+ Codex 指令。
- `docs/vk4-c-r1-swapchain-fix.svg`  # VK4-C-R1 Swapchain 重建修复对比图：修复前未设 OldSwapchain→失败 / 修复后传旧句柄→成功（透明背景、规范箭头，符合 Visualizer 约束）。
- `docs/log-ux-1-r2-autoscroll.svg`  # LOG-UX-1-R2 日志自动滚动可视化：跟随状态机（FOLLOW/PAUSED）+ 滚动时序（LogItems→LayoutUpdated→ScrollToEnd，规避 Extent 增长误判竞态）。
