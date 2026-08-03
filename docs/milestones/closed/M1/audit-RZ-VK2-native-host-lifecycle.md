# RZ-VK2: NativeHost / HWND 生命周期收口审计

日期：2026-07-07

## 结论

本轮仅收口 NativeHost / HWND 生命周期，没有创建 Vulkan Surface，也没有进入真实渲染。

## 本轮改动

- `XuanYu.Render.Vulkan/NativeHostHandleSnapshot.cs`
- `XuanYu.Render.Vulkan/NativeHostLifecycleState.cs`
- `XuanYu.Render.Vulkan/NativeHostLifecycleProbe.cs`
- `XuanYu.Render.Vulkan/NativeHostLifecycleLogFormatter.cs`
- `XuanYu.Editor.UI/ViewportNativeHostRoute.cs`
- `XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`

## 生命周期

```text
创建宿主控件
  -> 附加到可视树
  -> 原生句柄可用
  -> 尺寸变化
  -> 从可视树移除
  -> 释放宿主控件
  -> 原生句柄失效
```

## 结果

- HWND 能在 NativeHost 创建后获取。
- Resize 会记录尺寸变化。
- Detach / Dispose 会记录句柄失效。
- UI 布局未修改。
- 输入逻辑未修改。
- 未创建 Surface。
- 未创建 Swapchain。
- 未创建 LogicalDevice。
- 未进入真实渲染循环。

## 日志

日志字段保持中文：

- `【NativeHost】创建宿主控件`
- `【NativeHost】附加到可视树`
- `【NativeHost】原生句柄可用`
- `【NativeHost】尺寸变化`
- `【NativeHost】从可视树移除`
- `【NativeHost】释放宿主控件`
- `【NativeHost】原生句柄失效`

## 下一步

RZ-VK3 再单独接 Vulkan Surface，不提前混入 Swapchain 或渲染循环。
