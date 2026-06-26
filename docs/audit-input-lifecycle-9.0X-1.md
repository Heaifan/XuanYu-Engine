# 9.0X-1 审计：Native Viewport 输入生命周期 — 全量调用点与状态机分析

审计日期：2026/06/26  
审计目标：只审计，不修复。列出所有鼠标捕获 / 释放 / 丢失捕获的调用点，分析生命周期闭环性，作为 9.0X 输入生命周期封版的地基。  
范围限制：不修改 Vulkan、布局 UI、Gizmo 几何、Transform 数学逻辑；不混入 9.0Y/9.1A/9.1B。

---

## 一、范围文件清单

### 直接审计文件（只读，不修改）

| # | 文件 | 行数 | 职责 |
|---|---|---|---|
| 1 | `Panels/Viewport/NativeHost/Input/Pointer/NativeViewportMouseCapture.cs` | 66 | Win32 SetCapture/ReleaseCapture/GetCapture 唯一封装 |
| 2 | `Panels/Viewport/NativeHost/Input/Arbitration/NativeViewportInputArbitration.cs` | 93 | 左键输入仲裁：导航 / 工具 / 遗留三方裁决 |
| 3 | `Panels/Viewport/NativeHost/Input/Arbitration/NativeViewportNavigationCapture.cs` | 26 | Overlay 导航捕获状态机 |
| 4 | `Panels/Viewport/NativeHost/Input/Arbitration/NativeViewportSceneToolCapture.cs` | 29 | 场景工具（MoveGizmo）捕获状态机 |
| 5 | `Panels/Viewport/NativeHost/Control/WindowsVulkanViewportHostControl.WndProc.cs` | 99 | WndProc 消息分发：所有 WM_ 消息处理 |
| 6 | `Panels/Viewport/NativeHost/Control/WindowsVulkanViewportHostControl.Events.cs` | — | 事件声明 |
| 7 | `Panels/Viewport/NativeHost/WindowsVulkanViewportHostControl.cs` | 93 | NativeControlHost 生命周期：创建/销毁/RequestCapture/RequestReleaseCapture |
| 8 | `Panels/Viewport/NativeHost/Picking/WindowsVulkanViewportPickInput.cs` | 53 | 左键点击检测（非捕获路径） |
| 9 | `Panels/Viewport/NativeHost/Input/Pointer/NativeViewportPointerMessages.cs` | — | Win32 消息常量定义 |
| 10 | `Panels/Viewport/NativeHost/Input/Pointer/NativeViewportPointerAction.cs` | — | 动作枚举 |
| 11 | `Panels/Viewport/HostInfo/Panel/VulkanViewportHostPanel.axaml.cs` | — | Avalonia 面板 → Native Host 委托层 |
| 12 | `Viewport/Transform/Interaction/TransformPointerRoute.cs` | 100 | Gizmo 变换指针交互：OnPointerPressed/Released/Cancel |
| 13 | `Viewport/Transform/Interaction/TransformKeyboardRoute.cs` | 51 | G/Enter/Esc 键盘变换交互 |
| 14 | `Shell/Input/Transform/EditorSceneToolInputRoute.cs` | 66 | Gizmo 点按/实体体拖拽启动和确认 |
| 15 | `Shell/Navigation/EditorShellOverlayNavigationRoute.cs` | 92 | Overlay 导航事件路由 |
| 16 | `Shell/Input/Raw/EditorShellRawInputRoute.cs` | — | 原始输入转发 |

### 不允许修改文件

- 所有 `*.axaml` 文件（UI 布局）
- 所有 `Render.Vulkan/` 文件
- 所有 `Core/` 文件
- 所有 `Gizmo/` 几何/渲染文件

---

## 二、所有 SetCapture / ReleaseCapture / GetCapture 调用点清单

### 2.1 P/Invoke 声明（仅一文件）

| 函数 | 文件 | 行 | 封装形式 |
|---|---|---|---|
| `SetCapture` | `NativeViewportMouseCapture.cs` | 63 | `static extern nint SetCapture(nint hwnd)` |
| `ReleaseCapture` | `NativeViewportMouseCapture.cs` | 64 | `static extern bool ReleaseCapture()` |
| `GetCapture` | `NativeViewportMouseCapture.cs` | 65 | `static extern nint GetCapture()` |

结论：P/Invoke 声明集中且唯一，无分散调用风险。

### 2.2 SetCapture 调用点（全通过 NativeViewportMouseCapture.Capture 封装）

| # | 调用位置 | 方法 | 来源参数 | 按钮 | 触发场景 | 是否高频 |
|---|---|---|---|---|---|---|
| S1 | `WndProc.cs:50` | `HandleMiddleDown` | "中键相机导航" | "中键" | 用户按下中键开始旋转/平移 | 是 |
| S2 | `Arbitration.cs:22` | `HandleLeftDown` → `NavCapture.BeginDrag` | "Overlay导航" | "左键" | 左键在 Overlay 导航 gizmo 上按下并触发拖拽 | 是 |
| S3 | `Arbitration.cs:28` | `HandleLeftDown` → `ToolCapture.BeginDrag` | "MoveGizmo" | "左键" | 左键在 Move Gizmo 轴上按下并触发拖拽 | 是 |
| S4 | `HostControl.cs:37` | `RequestCapture` | "外部请求" | "未指定" | 外部模块主动请求捕获 | 低频 |
| S5 | `VulkanViewportHostPanel.axaml.cs:38` | `RequestCapture` → 委托到 S4 | — | — | Avalonia 层的外部请求转发 | 低频 |

### 2.3 ReleaseCapture 调用点（全通过 NativeViewportMouseCapture.Release 封装）

| # | 调用位置 | 方法 | 原因参数 | 触发场景 | 是否高频 |
|---|---|---|---|---|---|
| R1 | `WndProc.cs:40` | `DispatchWndProc` → WM_KILLFOCUS | "WM_KILLFOCUS" | 窗口失去焦点（Alt+Tab、任务栏点击、其他窗口弹出） | 低频 |
| R2 | `WndProc.cs:56` | `HandleMiddleUp` | "WM_MBUTTONUP" | 用户松开中键 | 是 |
| R3 | `WndProc.cs:97` | `HandleCancelMode` | "WM_CANCELMODE" | 系统弹出菜单 / Alt 激活菜单栏 / UAC 弹窗 | 低频 |
| R4 | `Arbitration.cs:46` | `HandleLeftUp` → Nav 拖拽结束 | "WM_LBUTTONUP" | 左键在导航拖拽中松开 | 是 |
| R5 | `Arbitration.cs:53` | `HandleLeftUp` → Tool 拖拽结束 | "WM_LBUTTONUP" | 左键在工具拖拽中松开 | 是 |
| R6 | `HostControl.cs:38` | `RequestReleaseCapture` | "外部请求" | 外部模块请求释放 | 低频 |
| R7 | `HostControl.cs:62` | `DestroyNativeControlCore` | "WM_DESTROY/DestroyNativeControlCore" | 控件销毁：关闭窗口、Avalonia 重建 | 低频 |

### 2.4 GetCapture 调用点（全在 NativeViewportMouseCapture 内部）

| # | 调用位置 | 方法 | 用途 |
|---|---|---|---|
| G1 | `Capture()` 第 23 行 | 记录日志之前的先前捕获 | 信息记录，不改变逻辑 |
| G2 | `Release()` 第 34 行 | 判断 `GetCapture() == ownerHwnd` | **决定是否调用 ReleaseCapture 的关键依据** |
| G3 | `ClearState()` 第 48 行 | 记录日志 | 信息记录，不改变逻辑 |

结论：GetCapture 只在 `NativeViewportMouseCapture` 内使用，没有被外部模块直接调用来做状态判断。

---

## 三、所有 WM_CAPTURECHANGED 相关路径

| # | 位置 | 行为 | 是否调用 ReleaseCapture | 内部状态是否清理 |
|---|---|---|---|---|
| C1 | `WndProc.cs:25` → `HandleCaptureChanged(lParam)` | Win32 → DispatchWndProc 分发 | **否**（只清理内部状态） | 是（ClearState） |
| C2 | `WndProc.cs:87-91` | `_rawPointerDragCaptured = false` + Arbitration 通知 + `_mouseCapture.ClearState` | **否** | 是 |
| C3 | `Arbitration.cs:76-86` | 清 NavCapture 和 ToolCapture 状态 + 触发 navigationCaptureLost / rawFocusLost 事件 | N/A | 是 |

设计意图：WM_CAPTURECHANGED 是 Win32 在捕获被其他窗口抢走或被释放后发送的通知消息。在此路径中调用 ReleaseCapture() 会导致递归。当前设计正确处理了这一点——只同步内部状态，不递归调用 Win32 API。

**但存在一个状态窗口：** 如果 WM_CAPTURECHANGED 先清掉了 `_mouseCapture` 内部状态（`_captured = false`），然后正常释放路径（如 R2/R4/R5）仍会进入 `Release(ownerHwnd, reason)`，但内部日志会显示"内部状态=已释放"（因为 `_captured` 已被 ClearState 清掉）。第 39 行的日志在清状态前执行 `Log(...)`，但日志中 `_captured` 在此时已经被设为 false 了吗？—— 看代码：`ClearState` 第 50 行设 `_captured = false`，`Release` 第 38-43 行先 ReleaseCapture 再设 false。**如果 WM_CAPTURECHANGED 早于正常释放到达**，正常 Release 会看到 `_captured = false`（日志显示"未捕获"），但 `GetCapture()` 仍等于 ownerHwnd 因此仍会调用 ReleaseCapture()。**这是正确行为**，只是日志状态描述在语义上有点困惑（"未捕获"时却调用了 ReleaseCapture）。

---

## 四、输入捕获状态机表

### 4.1 全局状态

```
                        ┌──────────────────────────────────────┐
                        │                                      │
                        ▼                                      │
    ┌───────┐  Down   ┌──────────┐  Move(M)   ┌────────────┐  │
    │ Idle  │────────►│ Pressed  │───────────►│ DragPreview │  │
    │       │         │ (无Capture│             │ (有Capture) │  │
    └───┬───┘         │ 或Capture)│             └──────┬───────┘  │
        ▲             └─────┬────┘                     │         │
        │                   │                          │         │
        │                   │ ButtonUp(无Drag)         │         │
        │                   ▼                          │         │
        │            ┌───────────┐                     │         │
        │            │  Click    │                     │         │
        │            │ (Pick/Select)                   │         │
        │            └───────────┘                     │         │
        │                                              │         │
        │                     ┌────────────────────────┘         │
        │                     │         │                        │
        │                     ▼         ▼                        │
        │              ButtonUp    Esc/CancelMode                │
        │                     │         │                        │
        │                     ▼         ▼                        │
        │              ┌──────────┐ ┌──────────┐                │
        │              │  Commit  │ │  Cancel  │                │
        │              │ (写World) │ │ (恢复初值)│                │
        │              └────┬─────┘ └────┬─────┘                │
        │                   │            │                      │
        │                   ▼            ▼                      │
        │              ┌────────────────────────┐               │
        │              │     ReleaseCapture     │               │
        │              └───────────┬────────────┘               │
        │                          │                            │
        └──────────────────────────┘                            │
                                                                 │
                    KillFocus/CaptureLost/Destroy ────────────────┘
                    (任何状态→Idle，清理内部状态)
```

### 4.2 各捕获来源状态转换

#### 来源 1：中键相机导航（Orbit / Pan / Dolly）

| 阶段 | 消息 | 动作 | Win32 Capture | 内部状态 |
|---|---|---|---|---|
| Idle | — | — | 无 | `_rawPointerDragCaptured=false` |
| → Pressed | WM_MBUTTONDOWN | `_mouseCapture.Capture(hwnd, "中键相机导航", "中键")` + `_rawPointerDragCaptured=true` | SetCapture | 已捕获 |
| → DragMove | WM_MOUSEMOVE | 导航计算 | 持有 | 已捕获 |
| → Committed | WM_MBUTTONUP | `_mouseCapture.Release(hwnd, "WM_MBUTTONUP")` + `_rawPointerDragCaptured=false` | ReleaseCapture | 已释放 |
| → CaptureLost | WM_CAPTURECHANGED | `ClearState` + 事件通知 | 无（已被系统清掉） | 已同步清除 |
| → FocusLost | WM_KILLFOCUS | `_mouseCapture.Release(hwnd, "WM_KILLFOCUS")` + 清所有状态 | ReleaseCapture | 已释放 |
| → Canceled | WM_CANCELMODE | `_mouseCapture.Release(hwnd, "WM_CANCELMODE")` + 清所有状态 | ReleaseCapture | 已释放 |
| → Destroyed | DestroyNativeControlCore | `_mouseCapture.Release(hwnd, "...")` + `_arbitration.Reset()` | ReleaseCapture | 已释放 |

#### 来源 2：Overlay 导航 Gizmo 左键拖动

| 阶段 | 消息 | 动作 | Win32 Capture | NavCapture 状态 |
|---|---|---|---|---|
| Idle | — | — | 无 | `IsActive=false, DragCaptured=false` |
| → Pressed(无拖拽) | WM_LBUTTONDOWN | arbitration → `NavCapture.SetActive()` | **不捕获** | `IsActive=true, DragCaptured=false` |
| → Pressed(有拖拽) | WM_LBUTTONDOWN | arbitration → `NavCapture.BeginDrag()` + `_mouseCapture.Capture(hwnd, "Overlay导航", "左键")` | SetCapture | `IsActive=true, DragCaptured=true` |
| → DragMove | WM_MOUSEMOVE | 导航计算 | 持有 | 同上 |
| → Released(无拖拽) | WM_LBUTTONUP | `NavCapture.End()` + `navigationReleased()` | **不释放** | `IsActive=false, DragCaptured=false` |
| → Released(有拖拽) | WM_LBUTTONUP | `NavCapture.End()` + `navigationReleased()` + `_mouseCapture.Release(hwnd, "WM_LBUTTONUP")` | ReleaseCapture | `IsActive=false, DragCaptured=false` |
| → CaptureLost | WM_CAPTURECHANGED | `NavCapture.ClearState()` + `navigationCaptureLost` | 无 | 全 false |
| → FocusLost | WM_KILLFOCUS | `NavCapture.ClearState()` + 事件 | ReleaseCapture | 全 false |
| → Canceled | WM_CANCELMODE | `NavCapture.ClearState()` + 事件 | ReleaseCapture | 全 false |
| → Destroyed | DestroyNativeControlCore | `_arbitration.Reset()` | ReleaseCapture | 全 false |

#### 来源 3：Move Gizmo 左键拖动

| 阶段 | 消息 | 动作 | Win32 Capture | ToolCapture 状态 |
|---|---|---|---|---|
| Idle | — | — | 无 | `IsActive=false, DragCaptured=false` |
| → BeginDrag | WM_LBUTTONDOWN | arbitration → `ToolCapture.BeginDrag()` + `_mouseCapture.Capture(hwnd, "MoveGizmo", "左键")` | SetCapture | `IsActive=true, DragCaptured=true` |
| → DragPreview | WM_MOUSEMOVE | `TransformPointerRoute.OnPointerMoved` → `_dragRoute.Move` | 持有 | 同上 |
| → Commit | WM_LBUTTONUP | `TransformPointerRoute.OnPointerReleased` → `_gizmo.EndDrag()` + `_dragRoute.Confirm()` + `_mouseCapture.Release(hwnd, "WM_LBUTTONUP")` | ReleaseCapture | `End()` → 全 false |
| → Cancel | WM_KEYDOWN(ESC) | `TransformKeyboardRoute.HandleKeyDown` → `pointerRoute.Cancel(Escape)` → `_gizmo.EndDrag()` + `_dragRoute.Cancel()` + **但未在此路径释放 Win32 Capture** | **不明**（见风险分析） | `End()` → 全 false |
| → CaptureLost | WM_CAPTURECHANGED | `ToolCapture.ClearState()` + `rawFocusLost` | 无 | 全 false |
| → FocusLost | WM_KILLFOCUS | `ToolCapture.ClearState()` + 事件 | ReleaseCapture | 全 false |
| → Canceled | WM_CANCELMODE | `ToolCapture.ClearState()` + 事件 | ReleaseCapture | 全 false |
| → Destroyed | DestroyNativeControlCore | `_arbitration.Reset()` | ReleaseCapture | 全 false |

---

## 五、风险分析与排序

### 5.1 风险项 R1：Esc 取消拖动未直接释放 Win32 Capture

**文件：** `TransformKeyboardRoute.cs:22-26`  
**问题：** Esc 键处理仅调用 `pointerRoute.Cancel(TransformInteractionReason.Escape)`，而 `TransformPointerRoute.Cancel()` 只做了 `_gizmo.EndDrag()` + `_dragRoute.Cancel()`，**没有显式调用 `_mouseCapture.Release()`**。

**但是**——在正常流程中，WM_LBUTTONUP 会在仲裁层面调用 `_mouseCapture.Release(hwnd, "WM_LBUTTONUP")`，所以 Esc 取消后，如果用户再点一下左键或者拖动结束，可能依赖 WM_LBUTTONUP 来释放。但这存在一个窗口期：取消后到下一次点击之间，Win32 捕获仍被 viewport 持有。

**严重度：** 中  
**建议：** 9.0X-2 中扩展仲裁的 Cancel 路径，统一释放 Win32 Capture。

### 5.2 风险项 R2：WM_CAPTURECHANGED 先到，正常释放路径后到的日志歧义

**文件：** `NativeViewportMouseCapture.cs:32-44`  
**问题：** 当系统先发 WM_CAPTURECHANGED 清理内部状态，再发 WM_MBUTTONUP 或 WM_LBUTTONUP 时，`Release()` 第 39 行日志显示"内部状态=未捕获"，但实际上仍会调用 `ReleaseCapture()`（因为 `GetCapture()` 判断是主要依据）。**行为正确，日志语义易误解。**

**严重度：** 低（仅日志问题）  
**建议：** 修改日志为"内部状态=已同步清除但Win32仍持有"和"内部状态=已同步清除且Win32已释放"。

### 5.3 风险项 R3：G 模态取消路径与左键释放路径的状态不同步

**文件：** `TransformKeyboardRoute.cs:22-26` + `Arbitration.cs:34-57`  
**问题：** Blender G 模态通过键盘启动（模拟点击），但取消时走 `Cancel(Escape)` 路径。如果 G 模态中用户突然点左键再松开，`HandleLeftUp` 中的 `ToolCapture.IsActive` 判断可能进入错误分支。

**严重度：** 低（未确认是否实际触发）  
**建议：** 在 9.0Y 中加强 G 模态的状态隔离。

### 5.4 风险项 R4：External RequestCapture/ReleaseCapture 无来源追踪

**文件：** `HostControl.cs:37-38`  
**问题：** `RequestCapture()` 的来源写死为"外部请求"，`RequestReleaseCapture()` 无来源参数。但当前无外部模块调用这两个方法（仅 ViewportHostPanel 委托中转）。

**严重度：** 低（当前无实际调用者）  
**建议：** 如有新增调用方，必须传入真实来源。

### 5.5 风险项 R5：右键完全未处理

**文件：** `NativeViewportPointerMessages.cs`  
**问题：** `WM_RBUTTONDOWN(0x0204)` 和 `WM_RBUTTONUP(0x0205)` 未定义、未解析、未分发。如果未来引入右键功能，需要重新审查 WndProc 消息分发。

**严重度：** 低（当前右键无功能，不进入捕获路径）  
**建议：** 在 `NativeViewportPointerAction` 中预留 `RightDown`/`RightUp`，设置默认不做任何事。

### 5.6 风险项 R6：TransformPointerRoute.Cancel 与 WM_LBUTTONUP 的时序竞争

**文件：** `TransformPointerRoute.cs:77-83` + `EditorSceneToolInputRoute.cs:55-65`  
**问题：** 如果用户同时触发 Esc 取消（→ Cancel）和左键松开（→ HandleLeftUp → Release），两个路径都会尝试清理状态。`HandleLeftUp` 中 `ToolCapture.End()` 和 `_mouseCapture.Release()` 在 Cancel 已经清理后调用是幂等的（Release 以 GetCapture 判断），但 `sceneToolReleased` 回调可能触发二次 UI 刷新。

**严重度：** 低（当前执行幂等，但 UI 可能多刷新一次）  
**建议：** 无需在本阶段修复。

### 5.7 风险项 R7：窗口失焦后的异步 WM_CAPTURECHANGED

**问题：** Win32 在 Alt+Tab 时先发 `WM_KILLFOCUS`，再发 `WM_CAPTURECHANGED`。当前 `WM_KILLFOCUS` 处理中已释放捕获并清状态，随后 `WM_CAPTURECHANGED` 到达时内部状态已清，**ClearState 幂等执行**。

**严重度：** 低（当前幂等处理正确）

### 5.8 风险项 R8：高频路径的日志与探针写入

**文件：** `NativeViewportMouseCapture.cs:56-61`  
**问题：** 高频路径（鼠标拖动中的每个 Move）不经过 `Capture`/`Release`，因此不会触发日志。但在中键/左键 Down/Up 时，每次都会写 `Debug.WriteLine` 和 `EditorProbe.Write`。编辑器中键旋转每帧不会触发 Down/Up，因此不会刷屏。

**严重度：** 无风险（设计正确）

### 5.9 风险项 R9：WM_CANCELMODE 使用的原始消息值

**文件：** `WndProc.cs:44`  
**问题：** `WM_CANCELMODE(0x001F)` 使用硬编码十六进制，未定义命名常量。其他消息（如 WM_KILLFOCUS）在 `NativeViewportFocusMessages.cs` 中有命名常量。

**严重度：** 低（代码一致性问题）  
**建议：** 在 `NativeViewportFocusMessages` 或新建文件中定义 `WmCancelMode = 0x001F`。

---

## 六、谁负责什么

| 职责 | 类/方法 | 说明 |
|---|---|---|
| **SetCapture** 统一入口 | `NativeViewportMouseCapture.Capture(hwnd, source, button)` | 唯一调用 SetCapture 的地方 |
| **正常 Release** 统一入口 | `NativeViewportMouseCapture.Release(ownerHwnd, reason)` | 唯一调用 ReleaseCapture 的地方（依赖 GetCapture 判断） |
| **异常 Release**（KillFocus） | `WndProc.cs:37-41` → `_arbitration.HandleKillFocus` + `_mouseCapture.Release` | 窗口失焦兜底 |
| **异常 Release**（CancelMode） | `WndProc.cs:93-98` → `HandleCancelMode` + `_mouseCapture.Release` | 系统菜单/UAC 兜底 |
| **异常 Release**（Destroy） | `HostControl.cs:58-70` → `DestroyNativeControlCore` + `_mouseCapture.Release` | 窗口关闭兜底 |
| **内部状态同步**（CaptureChanged） | `WndProc.cs:87-91` → `HandleCaptureChanged` + `_mouseCapture.ClearState` | **仅清内部状态，不调 ReleaseCapture** |
| **NavCapture 状态管理** | `NativeViewportNavigationCapture` | BeginDrag / End / ClearState |
| **ToolCapture 状态管理** | `NativeViewportSceneToolCapture` | BeginDrag / End / ClearState |
| **仲裁决策** | `NativeViewportInputArbitration.HandleLeftDown/Up` | 决定谁消费左键、是否 Capture/Release |
| **Gizmo 拖动取消**（业务层） | `TransformPointerRoute.Cancel(reason)` | 处理 Gizmo.EndDrag + DragRoute.Cancel，**不直接释放 Win32 Capture** |
| **键盘 Esc 取消** | `TransformKeyboardRoute.HandleKeyDown` | Esc → `TransformPointerRoute.Cancel` |

---

## 七、WM_CAPTURECHANGED 的职责确认

**结论：只同步内部状态，不改变拖动业务状态（除了清标志）。**

`HandleCaptureChanged` 方法：
1. 清 `_rawPointerDragCaptured`（WndProc 类级别的原始拖拽标志）
2. 触发 `RawInputFocusLost` 事件
3. 调用 `_arbitration.HandleCaptureChanged`（清 NavCapture 和 ToolCapture 内部状态，触发相应丢失事件）
4. 调用 `_mouseCapture.ClearState("WM_CAPTURECHANGED 新捕获窗口=...")`（清 `_captured`/`_capturedHwnd`/`_source`/`_button`）

**不执行的操作：**
- ❌ 不调用 `ReleaseCapture()`
- ❌ 不通知 Gizmo 业务层（`TransformPointerRoute`）
- ❌ 不修改 UI 状态

**但注意**：WM_CAPTURECHANGED 会触发 `NavigationCaptureLost` 和 `RawInputFocusLost` 事件，这些事件的上游消费者（如 EditorShell 的事件连线）可能会触发 UI 刷新或状态重置。因此虽然 WndProc 层面"只清状态"，但事件链上可能影响更广。

---

## 八、各捕获来源的生命周期闭环评估

| 捕获来源 | Begin | Move | ButtonUp | KillFocus | CaptureChanged | CancelMode | Destroy | Esc取消 | 是否闭环 |
|---|---|---|---|---|---|---|---|---|---|
| 中键相机导航 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | N/A | **🟢 闭环** |
| Overlay导航(左键) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | N/A | **🟢 闭环** |
| MoveGizmo(左键) | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ⚠️ | **🟡 基本闭环** |

**MoveGizmo Esc 取消路径说明：**
- Esc → `TransformPointerRoute.Cancel()` → 清理 Gizmo/Drag 业务状态
- Win32 Capture **在此路径未显式释放**
- 但实际因为 WM_LBUTTONUP 或 WM_CAPTURECHANGED 会在点击后到来，最终仍会释放
- **风险窗口**：Esc 后到下一次点击之间，Win32 Capture 仍持有

---

## 九、风险排序与修复建议（供 9.0X-2 参考）

| 优先级 | 风险项 | 问题 | 建议修复范围 |
|---|---|---|---|
| **P0** | R1 | Esc 取消不释放 Win32 Capture | `TransformPointerRoute.Cancel` 调用链中增加 `_mouseCapture.Release`，或通过 Arbitration 统一路径释放 |
| **P1** | R9 | WM_CANCELMODE 使用硬编码 0x001F | 提取为命名常量 |
| **P2** | R2 | 日志语义在 CaptureChanged 先到时困惑 | 修改日志字符串从"内部状态=已释放/未捕获"为更精确描述 |
| **P3** | R4/R5 | 右键/外部请求无规范 | 预留枚举 + 文档 |
| **P4** | R6/R7/R8 | 幂等执行但可能 UI 多刷新 | 无需修复 |

---

## 十、核心结论

1. **NativeViewportMouseCapture 是目前唯一的 SetCapture/ReleaseCapture/GetCapture 封装**，没有分散的 Win32 调用点。
2. **存在 3 个捕获来源**（中键导航、Overlay 导航左键、MoveGizmo 左键），均通过 Arbitration 或直接调用同一 Capture 入口。
3. **存在 7 个释放/清理路径**（ButtonUp×3 + KillFocus + CancelMode + CaptureChanged + Destroy），其中 6 个实际调用 ReleaseCapture，1 个（CaptureChanged）仅清内部状态。
4. **WM_CAPTURECHANGED 设计正确**：只同步内部状态，不递归调用 ReleaseCapture。
5. **Esc 取消 Gizmo 拖动是唯一未闭环的释放路径**——Win32 Capture 在 Esc 后仍持有，直到下一次点击触发 WM_LBUTTONUP 才释放。
6. **所有模块职责基本清晰**，无"谁负责 Release"的混淆——`NativeViewportMouseCapture.Release` 是唯一出口，依赖 `GetCapture() == ownerHwnd` 做最终判断。
7. **当前代码已处于 9.0X-R1 修复后的状态**（修正了 WM_CAPTURECHANGED 参数和 Release 兜底），9.0X-1 只发现一个明确的待修复点（Esc 取消路径）。

---

## 十一、验收确认

- [x] 能一眼看出所有捕获入口和释放出口
- [x] 能判断是否存在"一处 SetCapture，多处 Release，但状态不同步"的风险 → **不存在，Release 是唯一出口且以 GetCapture 为依据**
- [x] 能判断 WM_CAPTURECHANGED 后拖动状态是否会残留 → **不会，ClearState 清所有内部状态**
- [x] 能判断 Gizmo 拖动卡顿是否可能来自捕获生命周期反复触发 → **Esc 取消路径是唯一未闭环点，但非高频路径不会导致卡顿**
- [x] 本阶段只给结论和修复建议，不要求立刻修复
