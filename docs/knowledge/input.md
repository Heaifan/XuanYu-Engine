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

---

## WAVE-2.5 Production Input Final Freeze

WAVE-2.5 E5 的最终生产输入拓扑为：

```text
Native HWND / Avalonia fallback
            ↓
         Adapter
            ↓
   Production Input Sink
            ↓
 ViewportInputComposition ×1
            ↓
    ViewportInputRouter ×1
            ↓
Camera / Picking / Gizmo / MapEdit / Region / Road / Marker
```

Native HWND 存在时 Native 是 authoritative source；没有 Native HWND 时 Avalonia 是 authoritative source。两者不得对同一物理事件双投递。`ViewportInputComposition` 是生产唯一 Composition Root，统一持有 Router、Lifecycle、CaptureCoordinator 和七个 Canonical Consumer；Snap 只作为 Helper / Constraint，不拥有手势。

生产 Host 只能进入 Production Input Sink，不得直接调用 Consumer、UiVm 输入业务方法或创建第二套 Router、Lifecycle、CaptureCoordinator、Composition。Canonical Consumer 层不得依赖 HWND、WndProc、Avalonia event args、Native message 或 Host control 类型。

E5 验收基线：World.Tests 22 个已登记 baseline failure，New Regression = 0；最终验收 HEAD 记录于 E5 freeze 提交报告。

---

## K-INP-003 Input Router 只有接入真实生产 Source 才算完成

**状态**：Active
**优先级**：P0
**证据等级**：E2
**适用范围**：Native HWND、Avalonia Pointer、Production Input Sink、ViewportInputRouter、Consumer 装配。

Router、Consumer 和测试可以独立存在，但这不等于生产输入已经统一。完成判定必须证明：

```text
Real Platform Event → Real Adapter → Editor Event → Production Router → Arbitration → Owner → Consumer → Domain/Core
```

禁止以“Router 可实例化”“Helper PASS”或“测试有 Router”代替生产接线证据；必须审计真实 Native/Avalonia Source 是否仍直接调用 UiVm 或 Consumer。

---

## K-INP-004 平台输入编码必须在 Adapter 边界正规化

**状态**：Active
**优先级**：P0
**证据等级**：E2
**适用范围**：Win32/Avalonia 键盘、修饰键、鼠标按钮、滚轮、Pointer ID、DPI、坐标。

平台编码只能在 Adapter 内解释；Router、Consumer 和领域 Core 只消费统一语义，例如 `EditorKey.Alt` 与 `EditorPointerModifiers.Alt`。`0x0020` 不得凭经验映射为 Alt，必须依据平台合同区分 XBUTTON1 等真实含义。

验证必须覆盖真实平台 Enum / Message Contract，再验证 Adapter 输出的统一语义。

---

## K-INP-005 交互终止必须绑定 Interaction Epoch

**状态**：Active
**优先级**：P0
**证据等级**：E1（单次 scoped 工程证据；非 Integration Evidence）
**K Coordinator 裁决**：ADOPT · 2026-10-03（FIX-L3 原写入无 `docs/knowledge/**` Ownership；本次由 K Coordinator 审计采纳）
**标签**：Interaction Epoch、Terminal Deduplication、ToolChanged、Late Commit
**适用范围**：Viewport Router、Editor interaction session、Gizmo direct entry、Tool lifecycle 与 transient cleanup。

**首次关键确认**：2026-10-03
**Process Version**：`v0.3.0.6-fix`
**Branch**：`feat/v0.3-world-authoring-r1`
**Commit**：`03a8003e8e5dbb5beb7ee0460c190e3ecef1f83b`（验证时 HEAD；本轮工作区 Dirty=YES）
**来源**：DIRTY-CONVERGENCE-R1 / FIX-L3

### 问题

仅覆盖 Router terminal dispatch 的测试，不能证明完整 lifecycle 已闭合：Router 可以进入 Idle，而真实 Editor interaction 仍可接受 Commit。若去重状态只在 Router 收到 Pressed 时复位，绕过 Router Pressed 的合法 direct interaction begin 会继承上一 epoch 的 `terminalHandled=true`。后续 Escape、Cancel、CaptureLost、FocusLost、WindowDeactivated 或 ViewportDisposed 可能不再关闭真实 Editor interaction，晚到 Commit 因而有机会提交已终止的会话。

ToolChanged 还有独立的顺序要求：旧 Tool 的生命周期必须在 ToolSnapshot 改变前终止。Consumer 的 ToolChanged Cancel 若会重入 SelectTool，就可能抢先改变旧 Tool 上下文；无 draft 的绘制 transaction 也必须完成关闭。

### 工程规则

```text
BEGIN INTERACTION
→ open a new interaction epoch
→ terminalHandled = false

TERMINAL
→ close the current Editor interaction and transient state once
→ terminalHandled = true

NEXT BEGIN
→ open a new epoch
→ terminalHandled = false
```

epoch 必须在成功建立 interaction 时开启，不得依赖某一种输入传输路径。所有 Commit 仍须由当前真实 Editor session 接受；Router Idle 本身不能证明 interaction 已终止。

ToolChanged 的顺序是：旧 Tool terminal intent → Router / Editor interaction closure → owner、transaction、hover 与 snap cleanup → 新 Tool state。Terminal cleanup 保留持久地图选择，并清除 `_regionVertexSnap`。

### 禁止做法

- 只在 Router Pressed 或特定 Gizmo 方法内复位 terminal dedup。
- terminal 后只检查 Router Idle，未检查 Editor interaction/session 是否仍可 Commit。
- 改 Tool state 后才尝试用旧 Tool / owner 上下文清理生命周期。
- 通用 terminal 通过 `ClearMapGeometrySelection(...)` 清除持久 Region、Road、Marker 或新建选择。
- 将尚待产品决策的 Draft Retention Policy 混入 terminal 修复。

### 验证方法

- direct Move、Rotate、Scale begin 后，逐一触发 Escape、Cancel、CaptureLost、FocusLost、WindowDeactivated、ViewportDisposed；所有旧 session late Commit 必须失败。
- 重复 terminal 在同一 epoch 只通知一次；下一 epoch 必须重新允许 terminal 通知。
- ToolChanged 覆盖 Region owner、零节点 transaction、Region hover preview、Road owner/draft 与持久 selection。

### 本轮验证证据

DIRTY-CONVERGENCE-R1 / FIX-L3 提交给 K 的 scoped 验证记录：原报告的 8 个失败用例 8/8 PASS；direct terminal late-commit 回归 18/18 PASS；Region/Road lifecycle 11/11 PASS；UnifiedTerminalCoordinatorTests 11/11 PASS；InputIntegration 83/83 PASS；Viewport / Transform UI / Region / Road scoped set 258/258 PASS；受影响 Editor.UI Build 0 warning / 0 error；ARCH-A 与 5+100 PASS。此为 E1 单次 scoped 工程证据，不是 Integration Evidence、完整 Solution / Integration Gate 或远端验收。验证时 Branch=`feat/v0.3-world-authoring-r1`、HEAD=`03a8003e8e5dbb5beb7ee0460c190e3ecef1f83b`、Process Version=`v0.3.0.6-fix`、共享 Workspace Dirty=YES。

**适用边界**：本条定义终止 epoch 与 lifecycle ordering，不改变 Region / Road Draft Retention Policy，也不替代真实 Native / UI 验收。

**关联 Knowledge**：K-INP-001、K-INP-002
**关联 Incident / ERR / EXP**：本轮未发现需记录的 Agent ERR；无 EXP 写回。

---
