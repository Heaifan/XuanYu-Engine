# WAVE-2.5-D1 Camera + Picking + Gizmo Consumer Migration

## 结论

`PARTIAL`。

本轮完成了 D1 的统一 Consumer 合同、唯一 Owner 注册顺序、Lifecycle 回调和取消测试；但任务同时禁止切换 Production Input Route，因而现有 `VulkanNativeHost` 的 Native/Avalonia 直连入口未接入新 Consumer。没有把“桥接层完成”冒充为“生产输入迁移完成”。

## D1 Consumer 合同

| Consumer | Owner | 仲裁顺序 | 生命周期 |
|---|---|---|---|
| Gizmo | `GestureOwner.Gizmo` | 1 | Router → Lifecycle → handler |
| Camera | `GestureOwner.Camera` | 2 | Router → Lifecycle → handler |
| Picking | `GestureOwner.Picking` | 3 | Router → Lifecycle → handler |

`ViewportD1ConsumerSet` 是唯一 D1 注册组合；三者均实现 `IViewportInputConsumer`，不维护私有 Owner、Capture 或 Cancel 状态。生产 Camera/Picking/Gizmo 入口仍在 `VulkanNativeHost` 旧直连路径，待解除“Production Input Route 未切换”边界后再接入该组合。

## Owner Arbitration

- Camera vs Picking：Middle press 由 Camera claim；无更高优先级命中的 Left press 才由 Picking claim。
- Camera vs Gizmo：Gizmo 命中优先；Active 后 Router 只向 Gizmo 分发。
- Picking vs Gizmo：Gizmo 命中优先；Picking 不会重复消费同一 Press。

## Cancellation

`Cancel`、`CaptureLost`、`FocusLost`、`WindowDeactivated` 均经过同一 Router → `ViewportGestureLifecycle.Cancel`；重复事件不会二次通知 Consumer，Cancel 后 Owner 为 `None`、Lifecycle 为 `Idle`，下一次 Press 可以重新 Begin。ToolChanged/ModeChanged/ViewportDisposed 仍需由未来统一 source adapter 映射为 `EditorPointerEvent.Cancel`，本轮未扩展事件模型。

## C 路 GAP

Delta、X1/X2、Horizontal Wheel、Avalonia Capture Source、Timestamp、Device Type 均保留为已知 GAP；本轮没有扩展 `EditorPointerEvent`。当前均不阻塞 D1 Consumer 合同测试；不影响 D2 合同，但仍阻塞 Final 25/25 READY。

## 边界

- D2 Map Geometry / Region / Road / Marker / Snap：未修改。
- Production Input Route：未切换。
- NativeControlHost / Win32ViewportHost / Vulkan Composition：未修改。
- Camera/Picking/Gizmo 领域行为：未修改。
- WAVE-3：未启动。
