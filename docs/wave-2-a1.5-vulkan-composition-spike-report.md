# WAVE-2 / GATE-0：A1.5 Vulkan 离屏 + Avalonia GPU Composition Spike

## 结论

**FAIL（当前 Avalonia 12.0.4 Windows 默认 backend）**。Spike 已真实走到 `CompositionSurfaceVisual`、Vulkan 外部 image 创建和 `ImportImage` 前的能力检查，但运行时发现当前 Composition backend 不声明 `VulkanOpaqueNtHandle`，因此没有把 Vulkan image 提交给 Avalonia，不能宣称 GPU→GPU Composition PASS。

这不是 NativeControlHost、WS_CHILD、Native Popup 或 CPU Readback 失败后的替代方案；本 spike 没有创建这些对象。

## 审计到的生产路径

- `VulkanViewport.axaml` 直接包含 `VulkanNativeHost`。
- `VulkanNativeHost` 继承 Avalonia `NativeControlHost`，在 `CreateNativeControlCore` 调用 `Win32ViewportHost.CreateChild`。
- `Win32ViewportHost` 创建带 `WS_CHILD | WS_VISIBLE` 的原生子 HWND，并接收 native pointer message。
- `VulkanNativeHost.Bridge.cs` 注入 `INativeHostSurfaceBridge`；Render.Vulkan bridge 依次创建 Vulkan Instance、Win32 `VkSurfaceKHR`、Device、Swapchain 和 render session。
- 当前生产关系是：Avalonia 视觉树拥有 NativeControlHost；NativeControlHost 拥有子 HWND；Vulkan Surface/Swapchain 绑定该 HWND。它不是 Avalonia Composition image。

## 本 spike 技术路径

`Avalonia Control → ElementComposition.SetElementChildVisual → CompositionSurfaceVisual → CompositionDrawingSurface → ICompositionGpuInterop.ImportImage → Vulkan external-memory image`。

Vulkan 端创建 device-local RGBA image，使用 `VK_KHR_external_memory_win32` 导出 `VulkanOpaqueNtHandle`，GPU 清色后以 `UpdateAsync` 提交；没有 CPU 读回。覆盖层是普通 Avalonia `Border`、`TextBlock`、`Button`，置于同一 Avalonia Window 的后续视觉层。

## 运行证据

实际运行日志：

```text
[A1.5] FAIL：Vulkan GPU Composition：System.InvalidOperationException: Avalonia 不支持 VulkanOpaqueNtHandle
[A1.5] SPIKE：Composition Resize=928, 539
```

因此：

- Native child HWND：spike **没有**。
- Extra PopupRoot / Window：**没有**，只有 spike 的单一 Avalonia 主窗口。
- 第二独立 UI window：**没有**。
- Overlay Hover/Click：控件事件接线是原生 Avalonia，但由于 Vulkan 合成首帧未建立，本轮不把它们算作覆盖 Vulkan 输出的证据。
- Resize：Avalonia Bounds Resize 路径已接入 GPU resource recreate；运行日志证明 Resize 事件到达，但不能证明 Vulkan image 成功呈现。

## 风险与建议

本机 Avalonia 包版本固定为 12.0.4；官方示例还使用更新 API 的 Vulkan image layout 信息，而本地 12.0.4 的 `PlatformGraphicsExternalImageProperties` 没有该字段。官方已记录部分 NVIDIA 驱动在外部 image layout/首帧数据方面的风险。

建议 WAVE-2 暂停 Viewport Production Migration。下一步只能在明确批准 Avalonia backend/版本策略后，重新验证 Vulkan backend 的 `VulkanOpaqueNtHandle` 能力与显式 layout 同步；不得退回 NativeControlHost、Native Popup 或 CPU Readback。
