# Input 输入知识

## K-INP-001 同一 Pointer 手势必须只有一个实时 Owner

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Pointer、Arbitration、Gesture Ownership、Region、Gizmo、Navigation
**适用范围**：Native Viewport 中所有会争夺同一鼠标/触控手势的工具。

**首次关键确认**：2026-08-10 11:48:28（UTC+08:00）
**版本**：`v0.2.25.9-fix`
**Commit**：`d621755`
**后续验证**：`v0.2.25.15-stab` · 2026-08-10 14:22:43 · `751da52`
**来源**：`changelog.md`

### 问题

当 Region、Navigation Gizmo、Selection、Scene Tool 都直接监听 LeftDown/Move/Up，并各自判断“这次是不是我的”，消息到达顺序就会变成隐式业务逻辑。某个工具先消费 Down，另一个工具又抢 Move，会产生误加点、拖动中断和残留会话。

### 工程规则

一次 Pointer 手势从 Down 到 Up/Cancel 必须先经过统一 Arbitration，并分配唯一 Owner：

```text
PointerDown
   ↓
Arbitration
   ↓
Owner = Gizmo / Region / Navigation / Selection / ...
   ↓
PointerMove → 同一 Owner
   ↓
PointerUp / Cancel → Owner 释放
```

Owner 生命周期未结束前，其它工具不得中途截获同一手势。

### 真实历史示例

`v0.2.25.9-fix` 确认 Region Tool 激活时 Native LeftDown 会先被 Region 消费；Gizmo 会话 Move 又会被 Region Preview 抢路。修复后 HostDetach、CaptureLost、CancelMode、KillFocus 统一清理 Gizmo 会话。`v0.2.25.15-stab` 继续统一 Navigation Gizmo 可见端点/轴线命中与手势所有权。

### 未来应用示例

新增 Terrain Brush 后，不能让：

```text
Terrain.HandleLeftDown()
Region.HandleLeftDown()
Gizmo.HandleLeftDown()
Selection.HandleLeftDown()
```

四条链并行猜测是否处理。应扩展 Arbitration 的 Consumer 枚举/优先级，并给整个手势一个确定 Owner。

### 禁止做法

- 只修“某个 Down 先后顺序”，不管 Move/Up/Cancel。
- 工具状态清了，但 Capture/Owner 仍残留。
- 通过 `if (toolActive)` 散落在多个 WndProc 分支里实现优先级。

### 验证方法

至少覆盖：单击、拖动、工具往返、CaptureLost、KillFocus、CancelMode；探针记录每次手势的 Owner 与生命周期，单次 Down 不得产生两个正式 Consumer。

**关联 Incident**：INC-2026-08-10-001
**关联 Knowledge**：K-INP-002

---

## K-INP-002 Win32 Mouse Capture 必须统一管理完整释放生命周期

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Win32、SetCapture、ReleaseCapture、GetCapture、WM_CANCELMODE
**适用范围**：Native Vulkan Viewport 的相机拖动、Gizmo 拖动、任何 Win32 Capture。

**确认日期**：2026-06-26（原始 changelog 未记录时分）
**关键 Commit 时间**：2026-06-26 09:42:31（UTC+08:00）
**版本**：`v0.1.8.10-fix`
**Commit**：`8d6e7fd9ef6f430c0888f83e3dd8b1901501d741`；changelog 另登记后续 `a48ecfd`
**来源**：`docs/archive/changelog/changelog-2026-06.md`、Git Commit

### 问题

Win32 Capture 是操作系统真实状态，不等于 C# 内部 `_captured` 布尔值。历史实现中 `WM_MBUTTONUP` 只清内部状态却没有 `ReleaseCapture()`，导致 Native Viewport 继续吞鼠标消息；表现可包括 UI 点击无反应、Gizmo hover 变黄但拖不动、窗口关闭卡顿。

### 工程规则

`SetCapture` / `ReleaseCapture` 必须集中到一个拥有明确生命周期的组件。释放判断以 Win32 `GetCapture()` 为最终事实，内部状态只作为缓存/诊断，不能成为唯一依据。

必须覆盖至少：

```text
ButtonUp
WM_CANCELMODE
WM_KILLFOCUS
WM_DESTROY / DestroyNativeControlCore
Dispose
WM_CAPTURECHANGED（只同步，不递归 Release）
```

### 真实历史示例

`v0.1.8.10-fix` 将所有捕获收口到 `NativeViewportMouseCapture`，`Release` 使用 `GetCapture()` 核对真实窗口；加入 WM_CANCELMODE、KillFocus、Destroy 等兜底。`WM_CAPTURECHANGED` 从 `lParam/wParam` 同步新捕获 HWND，仅清内部状态，不再递归 Release。

### 未来应用示例

如果日志显示 `_captured=false`，但 `GetCapture()` 仍返回当前 Vulkan HWND，系统依旧处于捕获状态。修复不能再写一次 `_captured=false`；必须走统一 Release 路径并记录真实调用结果。

### 禁止做法

- 其它模块直接 P/Invoke `SetCapture` / `ReleaseCapture`。
- ButtonUp 只重置业务 Drag 状态。
- 在 `WM_CAPTURECHANGED` 里无条件再次 Release，引发递归或错误释放别的窗口 Capture。

### 验证方法

Probe 至少记录：来源、按钮、owner HWND、GetCapture 当前值、释放原因、是否真实调用 ReleaseCapture。回归中组合中键相机、Gizmo 拖动、焦点丢失和取消消息。

**关联 Incident**：INC-2026-06-26-001
**关联 Knowledge**：K-INP-001
