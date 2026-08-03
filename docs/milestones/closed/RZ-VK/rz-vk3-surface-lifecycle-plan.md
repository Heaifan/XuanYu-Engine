# rz-vk3-surface-lifecycle-plan.md

## 目标
RZ-VK3-Plan 只规划**正式 Vulkan Surface 生命周期**，替代当前探针状态；本轮不写任何 Vulkan 实装代码。

## 阶段边界（再次强调）
VK3 只做 Surface 创建/销毁，不碰 Swapchain / LogicalDevice / 真实渲染循环。Swapchain 留给 VK4（`VulkanSwapchainOwner`）。禁止 VK3 夹带 Swapchain。

## 五问规划

### 1. Surface 谁创建
- **禁止**：普通 UI 文件（`XuanYu.Editor.UI` 下任何 `.cs/.axaml`）直接创建 Surface。
- **正式**：由 `XuanYu.Render.Vulkan` 内部的 `VulkanSurfaceOwner` 创建，输入是来自 NativeHost 的窗口句柄（HWND）。
- 创建走 `VkWin32SurfaceCreateInfoKHR`（Windows），不依赖任何 UI 类型。

### 2. Surface 谁持有
- 集中到 `VulkanSurfaceOwner`（单一职责对象，位于 `XuanYu.Render.Vulkan`）：
  - `Create(HWND)` → 持有 `VkSurfaceKHR`；`Dispose()` 幂等销毁。
  - 自管生命周期，不外露句柄给 UI 层。
- `Editor.UI` 不持有、不缓存 Surface。

### 3. NativeHost 与 Surface 怎么交接
- NativeHost 只负责：窗口句柄、尺寸、Attach/Detach 生命周期事件；**完全不懂 Vulkan**。
- 交接方式：NativeHost 通过薄事件/快照把 HWND 与尺寸交给**组合根**（`Editor.Win` 或 App 级装配），组合根经 `Render.Abstractions` 契约把 HWND 传给 `VulkanSurfaceOwner`。
- 依赖方向：`Editor.UI → Render.Abstractions（契约）`；`Render.Vulkan → Render.Abstractions`；组合根注入 `VulkanSurfaceOwner`。UI 不直接认识 Vulkan 实现。
- 现有 `ViewportNativeHostRoute` 只做 UI 层事件转发，**不得在此创建 Surface**。

### 4. Surface 销毁时机
- 跟随 NativeHost 生命周期：`OnDetachedFromVisualTree` / `DestroyNativeControlCore` 时，组合根通知 `VulkanSurfaceOwner.Dispose`。
- **禁止**随日志、VM 状态、Resize、Hover、RenderFrame 等高频或间接路径销毁/重建 Surface。
- Resize 只改变尺寸快照，不触发 Surface 重建（Surface 与窗口尺寸解耦，直到 VK4 Swapchain 才响应尺寸）。

### 5. 探针代码怎么处理
- `VulkanClearSession.*` 是 VK1 探针参考，把 Instance/Surface/Device/Swapchain 挤在一处，违反单职责与平台隔离。
- 处理原则：**只能参考，不能直接搬**——提取设计意图（HWND → `VkWin32SurfaceCreateInfoKHR` 映射、销毁顺序），禁止把探针实现移入正式路径。
- VK3 只取 Surface 创建/销毁这一段并移入 `VulkanSurfaceOwner`；Device/Swapchain 段留给 VK4/VK5，不得随 VK3 一起实现。

## 目标依赖方向
```
Editor.UI → Render.Abstractions(契约) → VulkanSurfaceOwner(Render.Vulkan) → Vulkan Surface
NativeHost 只给 HWND/尺寸，经组合根接线，不直接碰 Vulkan。
```

## 防回潮门禁（VK3 实装时）
- 禁止在 `Editor.UI` 新增 `Silk.NET.Vulkan` 使用点。
- 禁止在 NativeHost / `ViewportNativeHostRoute` 内 `new` Surface。
- 禁止 VK3 夹带 Swapchain（VK3 只 Surface）。
- 所有新增 `.cs/.axaml` ≤100 行；`VulkanSurfaceOwner` 单职责。

## 规划态验收
- 本阶段只产出此文档，无代码改动。
- restore/build/test 仅确认仓库处于可构建状态（无独立测试项目，如实记录）。
