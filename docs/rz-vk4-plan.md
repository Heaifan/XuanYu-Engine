# rz-vk4-plan.md

## 目标
RZ-VK4-Plan 只规划**最小渲染闭环**（PhysicalDevice → LogicalDevice → Queue → Swapchain → ClearFrame → 组合根接线），在现有 Instance + Surface 基础上接出可呈现的画面；本轮不写任何 Vulkan 实装代码。

## 阶段边界（再次强调）
VK4 只做最小渲染闭环，不夹带**真实渲染循环**（持续动画 / 场景提交 / 多重 Pass）。持续渲染循环留给 VK5+。禁止 VK4 夹带场景绘制。

## 五问规划

### 1. 物理设备谁选
- 复用 VK1 探针已枚举的能力：由 `XuanYu.Render.Vulkan` 内新增 `VulkanPhysicalDeviceSelector` 选择，输入来自 Instance + Surface（需校验 Surface 呈现支持）。
- 选择标准：图形队列族可用、Surface 呈现支持、离散 GPU 优先（RTX 3060 已验证可识别）。
- 输出 `VulkanPhysicalDeviceResult`（物理设备句柄 + 图形队列族索引 + 呈现队列族索引 + 设备属性）；**不持有 VkDevice**。

### 2. 逻辑设备与队列谁建
- 由 `VulkanDeviceOwner`（Render.Vulkan）创建 VkDevice，启用必要队列族（图形 + 呈现，族相同则合并）。
- 创建后取出 `VkQueue`（图形队列、呈现队列），自管生命周期；Dispose 幂等释放并清空句柄。

### 3. Swapchain 谁持有
- 由 `VulkanSwapchainOwner`（Render.Vulkan）创建 VkSwapchainKHR，取交换链图像与 ImageView；生命周期绑定 RenderSession 而非 Surface 的 Attach/Detach。
- 创建以 Instance + Device + Surface + 队列为输入；Dispose 幂等。
- **Resize 仅重建 Swapchain，不重建 Surface**（Surface 仍只随 NativeHost Attach/Detach）。

### 4. 最小 Clear Frame 怎么做
- 由 `VulkanClearFrame` 建立最小 RenderPass（仅 Clear 附加载）、Framebuffer 与 CommandBuffer，提交一帧 clear + present。
- 不引入持续渲染循环；首帧呈现即证明 Swapchain 闭环打通（黑屏 → 单色清屏）。

### 5. 组合根怎么接线（VK4-E）
- 新增 `VulkanRenderSession`（Render.Vulkan）组合 Selector + DeviceOwner + SwapchainOwner + ClearFrame。
- Attach 顺序：Instance（已有）→ Surface（已有）→ Selector → DeviceOwner → SwapchainOwner → ClearFrame；Detach 逆序释放（ClearFrame → Swapchain → Device → Surface → Instance）。
- UI 经 Abstractions 新增契约 `IRenderSession`（仅 Attach/Resize/Detach/Present 抽象）；Editor.UI 组合根 `VulkanRenderSessionProvider` 装配具体实现，UI 宿主只认契约，不直接认识 Vulkan 类型。

## 目标依赖方向
```
Editor.UI → Render.Abstractions(IRenderSession 契约) → VulkanRenderSession(Render.Vulkan)
                                      ↑
Render.Vulkan: Selector / DeviceOwner / SwapchainOwner / ClearFrame（均不外露给 UI）
NativeHost 仍只给 HWND/尺寸，经组合根接线，不直接碰 Vulkan。
```

## 阶段分解（建议）
- **VK4-A**：VulkanPhysicalDeviceSelector（选择 + 队列族 + Surface 呈现校验）。
- **VK4-B**：VulkanDeviceOwner（VkDevice + 队列）。
- **VK4-C**：VulkanSwapchainOwner（VkSwapchainKHR + 图像/视图；Resize 仅重建 Swapchain）。
- **VK4-D**：VulkanClearFrame（RenderPass + Framebuffer + CommandBuffer + 单帧 clear/present）。
- **VK4-E**：VulkanRenderSession + IRenderSession 契约 + VulkanRenderSessionProvider 组合根接线。

## 防回潮门禁（VK4 实装时）
- Resize 不重建 Surface，只允许触发 Swapchain 重建策略（跳过 0 尺寸 / 重复尺寸）。
- 不把探针 VulkanClearSession 搬进正式路径（只参考设计意图，禁止复制实现）。
- Editor.UI 不得新增 `Silk.NET.Vulkan` 使用点；不得持有 Vk/Instance/Device/Surface/Swapchain。
- 所有新增 `.cs/.axaml` ≤100 行；单职责；目录核心文件 ≤5。
- VkResult 必须保存并分类处理；UI 线程禁无限等待。
- 持续渲染循环禁止在 VK4 引入。

## 规划态验收
- 本阶段只产出此文档 + VK3 收口文档，无代码改动。
- restore/build/test 仅确认仓库处于可构建状态（无独立测试项目，如实记录）。
