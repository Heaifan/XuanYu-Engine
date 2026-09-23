# XYE-VIEWPORT-W1-A1-COMPOSITION-SPIKE

## 结论

**PARTIAL / FAIL：当前生产式 NativeControlHost 路径不能证明目标成立。**

本 spike 使用真实 `VulkanNativeHost` 作为 Vulkan 输出层，在同一个 Avalonia
Visual Tree 中声明 Button、TextBlock、Border。它验证了 XAML 层级和事件接线，
但 Vulkan 输出仍由 `NativeControlHost` 承载，实际创建了 Win32 `WS_CHILD` HWND。
该 HWND 具有 airspace，Avalonia Visual 的 Z-order 不能可靠覆盖它，因此不能满足
“Avalonia overlay 在 Vulkan 上方可见并接收 Hover/Click，且无额外 UI HWND”的 PASS 条件。

## 当前生产 HWND / Surface 链

`VulkanViewport.axaml` 放置 `VulkanNativeHost`；`VulkanNativeHost` 继承
`NativeControlHost`。Avalonia 调用 `CreateNativeControlCore(parent)`，随后
`Win32ViewportHost.CreateChild(parent.Handle)` 创建 `WS_CHILD | WS_VISIBLE`
子窗口。`VulkanNativeHost.Bridge.cs` 将该 HWND 封装为
`NativeHostSurfaceHandle`，再由 `VulkanNativeHostSurfaceBridge` 创建 Vulkan
Instance、Win32 Surface、Logical Device、Swapchain、Render Session 和 Present。
Resize 继续通过子 HWND 的物理像素尺寸驱动 Swapchain 路径。

## 组合探针

`CompositionSpikeView` 只新增隔离 XAML 视图，不替换生产 Viewport。其结构为：

```text
Grid
├─ VulkanNativeHost  (真实 Vulkan NativeControlHost)
└─ Border
   ├─ TextBlock
   └─ Button
```

没有 `Popup`、`PopupRoot`、`Window`、Owned Window、Topmost 或 Diagnostic Native
Popup。Button 的 Hover/Click 处理器是普通 Avalonia 事件处理器；但在真实窗口上
是否能穿过 Native HWND 被观察到，必须以真机运行结果为准，不能由 XAML 声明推断。

## 明确检查

| 检查项 | 结果 |
| --- | --- |
| 组合技术 | Avalonia Visual overlay + 现有 Vulkan NativeControlHost |
| Native child HWND | 有，`WS_CHILD` |
| HWND 是否阻塞 overlay | 是，存在 airspace 风险且不能满足目标保证 |
| 额外 PopupRoot/Window | 无 |
| 独立第二 UI 窗口 | 无 |
| 生产 Viewport 是否修改 | 否 |
| 结果 | PARTIAL / FAIL |

## WAVE-2 建议

不要把该路径作为 WAVE-2 的 composition 基线。下一轮应单独评估真正的
Avalonia compositor / GPU image 共享方案：Vulkan 负责离屏渲染，Avalonia 负责
最终 Visual 合成，且必须在实现前冻结跨 API image、同步、Resize、DPI 和设备丢失
合同。若仍依赖 NativeControlHost，则应明确接受 airspace，不能宣称本目标 PASS。
