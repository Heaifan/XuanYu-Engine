# WAVE-2.5-D2 Map Edit Consumer Migration Design

## Goal

将 Map Geometry、Region、Road、Marker 的地图编辑 Consumer 对齐到 Phase-1 的
`EditorPointerEvent → ViewportInputRouter → Gesture Owner → ViewportGestureLifecycle`
模型；Snap 作为当前业务 Owner 的 Helper/Constraint，不独占手势。

## Frozen boundaries

- 统一事实源只能是现有 `ViewportInputRouter`、`GestureOwner`、
  `ViewportGestureLifecycle.Current`、`IViewportInputConsumer`、
  `IViewportPointerCaptureCoordinator` 和 `EditorPointerEvent`。
- 不创建第二 Router、第二 Owner、第二 Gesture 状态机、第二 Cancel Pipeline 或第二 Capture 生命周期。
- 本轮不修改 Camera、Picking、Gizmo、Vulkan、Composition、`VulkanNativeHost`、
  `Win32ViewportHost`、Production Viewport 或 Production Input Route。
- 本轮不修改 Schema、公共地图数据契约、坐标/单位模型、Delta、X1/X2、Horizontal Wheel、
  Avalonia Capture Source、Timestamp、Device Type；六项 C 路 GAP 只登记，不扩展模型。
- 不启动 WAVE-3。

## Current evidence

在统一基线 `00d8f147b1ffda5eabab2ae3a308f32c00b16258`：

- Phase-1 Router/Lifecycle/Adapter 已存在并有 Router、Lifecycle、Cancellation、Platform Parity 测试。
- `GestureOwner` 已包含 `MapEdit`、`Region`、`Road`、`Marker` 和 `SnapInteractionHelper`。
- Router 当前只接收通用 Consumer，地图 Consumer 尚未注册；地图行为仍由 UiVm 私有 PointerPressed/Moved/Commit/Cancel 方法承载。
- Native/Avalonia Host 是受保护的 D1/Production 路由边界，本轮不得改动。

## Design

### 1. Explicit map arbitration

扩展现有 Router 的 Begin 仲裁语义，使地图 Consumer 能声明“本次事件是否具备资格”，并由显式
Map tool/mode/hit policy 决定候选 Owner。Router 只向被仲裁选中的 Consumer 发送 Begin，不能通过
多个 Consumer 依次 `Handle` 后由第一个 handled 者获胜来隐式决定 Owner。

地图业务 Owner 固定为：

| Consumer | Owner | Begin qualification |
|---|---|---|
| Map Geometry | `GestureOwner.MapEdit` | 当前允许几何顶点/几何拖动且命中目标 |
| Region | `GestureOwner.Region` | Region create/edit tool 与有效地图点 |
| Road | `GestureOwner.Road` | Road tool 与有效地图点/节点目标 |
| Marker | `GestureOwner.Marker` | Marker place/select/drag 条件 |

Snap 不加入业务 Owner 列表。它由 Region、Road、Marker 或 Map Geometry Consumer 在 Update 时调用
现有求解器，产生 `Snap Candidate`/resolved point 并更新对应 Preview。

### 2. Consumer lifecycle

每个 Consumer 实现现有 `IViewportInputConsumer`，只把四阶段委托给已有地图编辑会话：

- Begin：捕获编辑起始快照，创建临时 draft/transaction，并在需要时请求 Pointer Capture。
- Update：只更新 Preview、候选 Snap 和临时几何，不写正式 World/History/Inspector。
- Commit：把有效结果交给现有 MapEditSession/UiVm 提交入口，结束临时状态。
- Cancel：恢复 Begin 快照并清除 Preview、Snap Candidate、Pressed/Hover、临时几何和临时事务。

`ViewportGestureLifecycle` 负责唯一的 Current、Capture、Commit/Cancel terminal callback；Consumer
不得直接操作平台事件、平台 Capture 或另建 cancellation path。

### 3. Cancellation matrix

Router 将 Escape、CaptureLost、FocusLost、WindowDeactivated、ToolChanged、ModeChanged、
ViewportDisposed 全部映射到现有 Lifecycle 的 Cancel；Released 只在当前 Owner 的合法 Begin
之后 Commit。取消不得转化为 Commit，也不得保留 Owner、Capture 或 Preview。

工具/模式切换由 Editor 层向同一个 Router 发送取消事件或调用其已有统一终止入口；不在 Host
增加第二套清理逻辑。

### 4. Production boundary

本轮不把新 Router 接入 `VulkanNativeHost`/`Win32ViewportHost` 的生产事件分支，不改变已有
Camera/Picking/Gizmo 优先级和 Native/Avalonia Capture 来源。D2 通过 Editor 层 Consumer contract、
Router integration test 和现有 Map session tests 证明迁移链条及清理闭环；生产输入接线作为后续
批准阶段，不能在本轮报告为已完成。

## Acceptance

必须有自动测试覆盖：

1. 四类 Consumer 的 Begin/Update/Commit；
2. 四类 Consumer 的 Cancel 与 Preview/Temporary State 清理；
3. Marker place/drag 的合法状态恢复；
4. Snap 在当前 Owner 下工作且不抢 Owner；
5. Explicit arbitration 保证一次 Gesture 只有一个主要 Owner；
6. Escape、CaptureLost、FocusLost、WindowDeactivated、ToolChanged、ModeChanged、ViewportDisposed；
7. 连续 Gesture 不串状态、不同 Consumer 不重复消费；
8. Phase-1 Router/Lifecycle/Cancellation/Platform Parity 回归不新增失败。

自动验证必须分别报告 D2 定向测试、Phase-1 回归、Solution Build、ARCH-A、ARCH-VIEWPORT、
5+100、`git diff --check`；不得把自动测试或 L1/L2 证据写成真机 UI 已验收。

## Non-goals and C-route GAP

六项 C 路 GAP 继续登记，不改变输入模型。若实现中发现某项是 D2 的真实硬阻塞，立即以
`BLOCKED` 报告并停止扩大 Scope；不得偷偷修改 Delta、X1/X2、Horizontal Wheel、Avalonia
Capture Source、Timestamp 或 Device Type。
