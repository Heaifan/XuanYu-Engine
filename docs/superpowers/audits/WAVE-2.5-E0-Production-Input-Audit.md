# WAVE-2.5-E0 Production Input Audit

**审计基线**：`0264c66c9a64fa98ee1cd8f53c37c3c41738de20`（Consumer Integration HEAD）  
**审计日期**：2026-09-24  
**范围**：仅审计；不修改产品代码。FIX10 当前未提交改动仅作为冲突边界读取。

## 任务结论

**READY**。E 可以按本审计进入实施；当前生产输入尚未接入 Router，但缺口已定位，且不需要再次全盘搜索。READY 不表示 E 已实现，也不表示 run.bat 已通过真机验收。

## 当前 Production Input 图

当前真实链路存在两条生产路径：

```text
Native HWND
  -> Win32ViewportHost.RouteWndProc
  -> NativePointerMessage
  -> VulkanNativeHost.OnNativePointerMessage
  -> NativePointerRoutePolicy
  -> UiVm.*（直接 Consumer）

Avalonia NativeControlHost
  -> VulkanNativeHost.OnPointerPressed/Moved/Released/Wheel/CaptureLost
  -> UiVm.*（直接 Consumer）

UiWin
  -> Deactivated / Window_KeyDown(Escape)
  -> UiVm.CancelInteractionFrom*

已存在但未接线的目标链：
Native/Avalonia Adapter -> EditorPointerEvent -> ViewportInputRouter
  -> explicit arbitration -> GestureOwner -> IViewportInputConsumer
  -> ViewportGestureLifecycle
```

直接证据：

- `XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.Input.cs:24-33` 调用 sink，并在 Native down 时 `SetCapture`。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs:46-47` 将 sink 直接设为 `OnNativePointerMessage`。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs:36-82` 直接调用 `UiVm` 的 MapGeometry、Drawing、Gizmo、Picking、Camera 方法。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.AvaloniaPointer.cs:7-83` 直接处理 Avalonia 全部指针事件与捕获丢失。
- `XuanYu.Editor/Input/ViewportInputRouter.cs:12-22` 只有 Router 类及 `Dispatch`；`git grep` 显示冻结 HEAD 的生产代码没有 Router 装配点，只有测试实例化点。

## Native 链路审计

| 阶段 | 当前事实 | 结论 |
|---|---|---|
| Win32ViewportHost | `RouteWndProc` 识别 WM_MOUSE、WM_CAPTURECHANGED、WM_KILLFOCUS、WM_CANCELMODE；构造 `NativePointerMessage` | Source 已有，但消息仍交给 Host sink |
| WndProc | `DefWindowProc` 始终继续调用；sink 是 `Action<NativePointerMessage>` | Host 仍是生产分发入口 |
| NativePointerMessage | 有坐标、buttons、capture before/after/target、Alt/Meta、PointerId、WheelDelta | 可作为 Adapter 输入；无 timestamp/device/horizontal delta |
| VulkanNativeHost | `SetInputSink(_hwnd, OnNativePointerMessage)`；dispose 时移除 | E2 应改为 source adapter + router sink |
| UiVm 私有入口 | Camera、Gizmo、Picking、MapGeometry、Region、Road、Marker、Snap 由 Host 直接选择并调用 | 全部属于 Legacy Bypass，逐项迁移 |

## Avalonia 链路审计

| 事件 | 当前直接路径 | E 目标 |
|---|---|---|
| PointerPressed | `OnPointerPressed` 中按 Camera → NavGizmo → MapGeometry → Drawing → Gizmo → Picking 顺序直接调用 VM | `FromPressed` 后 `Router.Dispatch`；优先级移入 Consumer |
| PointerMoved | `OnPointerMoved` 中直接调用 NavGizmo、Drawing、MapGeometry、Viewport、Camera | `FromMove` 后由 active owner 更新；idle observer/consumer 按优先级 |
| PointerReleased | 直接调用 NavGizmo、Viewport、MapGeometry、Camera 并释放 Avalonia capture | `FromReleased`；Router commit；Coordinator release |
| PointerWheelChanged | `VulkanNativeHost.CameraPointer.cs:37-41` 直接 `vm.DollyCamera` | Adapter `Wheel` 后由 Camera consumer 消费 |
| PointerCaptureLost | 直接 `CancelNavGizmo`、`CancelMapGeometryPointer`、`CancelInteractionFromNativePointer` | Adapter/生命周期事件进入 Router 的 `CaptureLost` |
| Key / Focus | Host KeyDown/KeyUp 只刷新 Snap modifier；UiWin tunnel Escape 直达 VM | Escape/FocusLost 进入统一 Cancel；Snap modifier 作为事件修饰符/辅助状态保留 |

## Window 链路审计

- `UiWin.axaml.cs:22` 的 `Deactivated` 直接调用 `CancelInteractionFromWindowDeactivated`，绕过 Router。
- `UiWin.Shortcuts.cs:25-26` 的 Escape 直接调用 `CancelInteractionFromEscape`，绕过 Router。
- `UiVm.Tool.cs:38-68,88-97` 在工具切换中直接取消 Region/Road 草稿并更新工具状态；未向 Router 发送 `ToolChanged`。
- `UiVm.Mode.cs:21-40` 在模式切换中直接 `CancelActiveInput`、取消 Region/Road，再切换模式；未向 Router 发送 `ModeChanged`。
- `VulkanNativeHost.cs:58-75` detach/dispose 目前调用 `CancelNativeInput` 或仅销毁 native host；没有 `ViewportDisposed` Router 事件。

## Legacy Bypass 清单

| 功能 | 当前生产入口 | 当前直接 Consumer / UiVm 方法 | 是否绕过 Router | 目标 Router Consumer | 需要修改文件 | 风险 | 测试入口 |
|---|---|---|---|---|---|---|---|
| Camera orbit/pan | Native middle down/move/up；Avalonia pressed/moved/released | `BeginCameraNavigation`, `PreviewCameraNavigation`, `EndCameraNavigation`, `CancelCameraNavigation` | 是 | `GestureOwner.Camera` / `CameraViewportInputConsumer` | `VulkanNativeHost.Pointer.cs`, `AvaloniaPointer.cs`, `CameraPointer.cs`, Composition Root | 两源重复会话、pointer id/capture 不一致 | `ViewportInputRouterDispatchTests`, camera navigation tests |
| Camera wheel | Native WM_MOUSEWHEEL；Avalonia Wheel | `DollyCamera` | 是 | Camera non-captured wheel consumer | `Pointer.cs`, `CameraPointer.cs`, adapters | 双重缩放、水平 wheel 丢失 | adapter tests + wheel regression |
| Picking | Native/Avalonia left press fallback | `PickViewportPointer` | 是 | `GestureOwner.Picking` | `VulkanNativeHost.Picking.cs`, source wiring, `UiVm.Picking.cs` only if adapter contract needs it | Picking 与 Gizmo/Map 抢占顺序改变 | `MapPickingRoundTripTests`, router arbitration |
| Navigation Gizmo | Native/Avalonia press/move/release | `TryNavGizmoPress/Move/Release`; then camera VM | 是 | `GestureOwner.Camera` or dedicated Navigation Gizmo consumer under explicit priority | `VulkanNativeHost.NavGizmo.cs`, consumers | endpoint click vs orbit drag语义回归 | NavigationGizmo tests + runtime input |
| Transform Gizmo | Native/Avalonia left press; tool decides Move/Rotate/Scale | `TryBegin*GizmoCapture`, `PreviewViewportPointer`, `CommitViewportPointer` | 是 | `GestureOwner.Gizmo` | `VulkanNativeHost.Gizmo.cs`, Gizmo consumer, UiVm transform adapters | active tool and lifecycle capture分裂 | Gizmo tests + router lifecycle |
| Map Geometry | Native/Avalonia press/move/release | `TryBeginMapGeometry*`, `PreviewMapGeometryPointer`, `CommitMapGeometryPointer`, `CancelMapGeometryPointer` | 是 | `GestureOwner.MapEdit` | `VulkanNativeHost.Pointer.cs`, `AvaloniaPointer.cs`, Map consumer bridge | vertex select 与 drag 误判 | Map geometry tests + arbitration |
| Region | Native/Avalonia left press/move | `RegionDrawingPointerPressed/Moved`, `CancelRegionDrawingFromEscape` | 是 | `GestureOwner.Region` | `VulkanNativeHost.Picking.cs`, `RegionSnapInput.cs`, Region consumer | Snap modifier/preview 高频路径回归 | region drawing/snap tests |
| Road | Native/Avalonia left press/move | `RoadDrawingPointerPressed/Moved`, `CancelRoadDrawingFromEscape` | 是 | `GestureOwner.Road` | `VulkanNativeHost.Picking.cs`, Road consumer | road draft与Region priority冲突 | road drawing tests |
| Marker | Native/Avalonia left press | `MarkerPlacementPointerPressed` | 是 | `GestureOwner.Marker` | `VulkanNativeHost.Picking.cs`, Marker consumer | placement click被Picking吞掉 | marker placement tests |
| Snap | Alt KeyDown/KeyUp + last pointer | `RegionDrawingPointerMoved(..., snapSuppressed)` | 是（辅助入口） | Region/Road consumer modifier state；不作为独立 owner，除非已有 `SnapInteractionHelper` 明确实现 | `RegionSnapInput.cs`, adapters, map consumers | stale coordinate、Alt切换重型副作用 | snap tests + Alt runtime |
| CaptureLost | WM_CAPTURECHANGED、Avalonia PointerCaptureLost | `CancelNativeInput`, `CancelMapGeometryPointer`, `CancelInteractionFromNativePointer`, `CancelNavGizmo` | 是 | Router `CaptureLost` → Lifecycle cancel | `Pointer.Cancel.cs`, `AvaloniaPointer.cs`, Coordinator | 双 cancel、release 递归、状态残留 | `ViewportInputRouterLifecycleTests`, capture regression |
| FocusLost | WM_KILLFOCUS | `CancelNativeInput(...WindowFocusLost)` | 是 | Router `FocusLost` | `Pointer.cs`, source bridge | native/Avalonia focus顺序差异 | focus cancel test |
| WindowDeactivated | UiWin.Deactivated | `CancelInteractionFromWindowDeactivated` | 是 | Router `WindowDeactivated` | `UiWin.axaml.cs`, UiWin input partial, composition root | window 与 viewport cancel 不一致 | lifecycle integration |
| Escape | UiWin tunnel KeyDown、多个 panel Escape | `CancelInteractionFromEscape` | 是（视口相关部分） | Router `Escape`; 非视口文本编辑仍留在自身控件 | `UiWin.Shortcuts.cs`, viewport input bridge | 全局 Escape 误取消 Inspector/对话框 | Escape routing regression |
| ToolChanged | `UiVm.Tool.cs` | 直接取消草稿/状态 | 是 | Router `ToolChanged` before/with tool state mutation | `UiVm.Tool.cs`, composition root | 工具状态先后序、旧 owner残留 | tool change cancel |
| ModeChanged | `UiVm.Mode.cs` | `CancelActiveInput`, Region/Road cancel | 是 | Router `ModeChanged` | `UiVm.Mode.cs`, composition root | mode toggle中重复取消 | mode change cancel |
| ViewportDisposed | Host detach/destroy | `CancelNativeInput` 或无 Router 事件 | 是 | Router `ViewportDisposed` | `VulkanNativeHost.cs`, lifecycle bridge | native销毁后 active owner悬挂 | disposal lifecycle |

## Consumer 对应关系

冻结 HEAD 已有 `GestureOwner`：Camera、Picking、Gizmo、MapEdit、Region、Road、Marker、SnapInteractionHelper。已存在的 Consumer/Backend 主要是 Router 测试和 D1 输入集；生产 UiVm 入口仍是旧执行面。E 不应把 Diagnostic observer 加入 consumer 列表。

建议正式映射：

```text
Camera             -> CameraViewportInputConsumer -> UiVm Camera Core
Picking            -> PickingViewportInputConsumer -> UiVm Picking Core
Gizmo              -> GizmoViewportInputConsumer -> Move/Rotate/Scale UiVm Core
MapEdit            -> MapGeometryInputConsumer -> UiVm MapGeometry Core
Region             -> RegionInputConsumer -> UiVm Region Core
Road               -> RoadInputConsumer -> UiVm Road Core
Marker             -> MarkerInputConsumer -> UiVm Marker Core
SnapInteractionHelper -> 仅作为 Region/Road 辅助状态，不独立抢占 pointer owner
```

## Cancellation 接线

统一规则：所有取消源只生成一个 `EditorPointerEvent`，由 Router 调用 `ViewportInputRouter.Cancellation.cs`，再由 `ViewportGestureLifecycle.Cancel` 调用当前唯一 consumer 的 `Cancel`，最后由 Coordinator release 平台 capture。旧的 VM cancel 方法只能作为 consumer/backend 的执行 core，不再由 Host/Window 直接调用。

```text
WM_CAPTURECHANGED / PointerCaptureLost -> CaptureLost
WM_KILLFOCUS                         -> FocusLost
UiWin.Deactivated                     -> WindowDeactivated
UiWin Escape（视口命中）               -> Escape
Tool change                           -> ToolChanged
Mode change                           -> ModeChanged
Host detach/destroy                  -> ViewportDisposed
WM_CANCELMODE / explicit              -> Cancel
```

Diagnostic Overlay/Native bridge 只能订阅或记录 source snapshot；不得调用 Router.Dispatch，也不得作为 consumer 或 capture coordinator。

## Capture 接线

当前有三层状态，不是一个 owner：

1. 平台 Native capture：`Win32ViewportHost.Input.cs:27` 在 left/middle down `SetCapture(hWnd)`；`ReleaseMouseCapture` 调用 `ReleaseCapture`。
2. 平台 Avalonia capture：`VulkanNativeHost.AvaloniaPointer.cs:17,19,31,36` 调 `e.Pointer.Capture(this)`；release 用 `Capture(null)`。
3. 业务状态：Host 的 `_nativeDragActive`、`_mapGeometryDragActive`、`_nativeCameraActive`、`_expectedCaptureRelease`，另有 `EditorState` 的 `HasCapture`。
4. 目标生命周期：`ViewportGestureLifecycle` 已通过 `IViewportPointerCaptureCoordinator.Capture/Release` 定义统一业务捕获，但当前没有生产实现/装配。

E 完成后的唯一逻辑 Capture Owner：`ViewportGestureLifecycle.Current.Owner + PointerId`。Native/Avalonia 只作为同一 Coordinator 的平台后端；平台 capture lost 只发一次 `CaptureLost`，不得再直接调用 VM cancel。正常 release 由 Lifecycle 触发，平台回调通过 coordinator 的 expected-release 防抖，不产生第二次业务取消。

## 六项 C GAP 判定

| GAP | 判定 | 直接代码证据 | 结论理由 |
|---|---|---|---|
| Delta | `NOT_REQUIRED_FOR_E` | `EditorPointerEvent` 已有 `WheelDelta`；两个 Adapter 都填充 wheel delta | E 的 pointer/wheel 接线可以复用现有单轴字段；若业务要平滑/累积再进 Final Gate |
| X1/X2 | `NOT_REQUIRED_FOR_E` | `NativePointerMessage` 仅有 `PhysicalX/PhysicalY`；Adapter 统一为 `Position(X,Y)` | 当前目标链是二维 viewport pointer，不存在生产 consumer 读取 X1/X2 的证据 |
| Horizontal Wheel | `REQUIRED_ONLY_FOR_FINAL_GATE` | Native 只从 `Buttons >> 16` 读取垂直 `WheelDelta`；Avalonia只取 `Delta.Y` | 不阻塞 E 的统一接线；横向滚轮若产品验收要求，再扩展 schema/adapter并单独 gate |
| Avalonia Capture Source | `REQUIRED_FOR_E` | `OnPointerCaptureLost` 当前直达 VM；`EditorPointerEventKind` 已有 `CaptureLost`；Lifecycle 有 Coordinator | 若不纳入 E，Avalonia capture 与业务 lifecycle 仍是第二条取消路径 |
| Timestamp | `REQUIRED_ONLY_FOR_FINAL_GATE` | `EditorPointerEvent` record 无 timestamp；现有 Router/Consumers 未读取 timestamp | E 可保持现有同步 dispatch；排序、延迟诊断或跨源时序验收再要求 |
| Device Type | `NOT_REQUIRED_FOR_E` | `EditorPointerEvent` 无 DeviceType，现有 Native PointerId 固定 1，Avalonia使用 `args.Pointer.Id` | 当前 scope 仅 Native/Avalonia 统一入口与 owner；没有 consumer 按设备类型分支的直接证据 |

## 与 FIX10 文件冲突

**0 个写入冲突**。本轮只新增本目录三份文档；未修改或删除：

- `DiagnosticOverlayHost*`
- `DiagnosticNative*`
- `DiagnosticPlacement*`
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs`
- Native Pointer Diagnostic bridge

这些文件已作为只读证据边界处理。

## 建议 E 开工基线

优先以 `0264c66c9a64fa98ee1cd8f53c37c3c41738de20` 为设计/实施基线。当前分支 `feat/XYUI-ENGINE-AREA-A-CD` 比该基线超前 4 个提交且包含 FIX10 未提交改动；E 实施前应先在不带 FIX10 生产改动的干净节点，或由负责人明确合并顺序后再开始。不得把当前 Diagnostic 工作区改动当成 E 的实现基线。

## Git 事实（审计开始时）

- Branch: `feat/XYUI-ENGINE-AREA-A-CD`
- Start HEAD: `444edbb9 fix(diag): preserve native viewport ownership and placement`
- Frozen audit HEAD: `0264c66c9a64fa98ee1cd8f53c37c3c41738de20`
- Working Tree: dirty；FIX10 Diagnostic 改动 7 个已修改/新增文件；本轮不触碰

