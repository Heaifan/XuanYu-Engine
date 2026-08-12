# MAP-DATA-A-R2-F2-F2 · Layer Delete UI Lock Recovery

状态：USER ACCEPTANCE FAILED；由 F2-F2-F1 Visible Delete Dialog 修复，F2 尚未 CLOSED；F3 Snap 继续冻结。

## 当前裁定

用户已确认原验收链 M01～M09 PASS；M10 因“删除道路图层后主 UI 保持 Disabled，Esc 无效，视口仍可操作”而 BLOCKED。已通过的 M01～M09 保留，不推倒重测。

## 真实根因

删除图层触发主窗口内 `DialogOverlay/DialogCard`。`Window_KeyDown` 以 Tunnel 路由先于 `DialogCard_KeyDown`，因此 Escape/Enter 被普通编辑快捷键消费，未调用 `CompleteDialog()`；遮罩持续可见，原生 Vulkan 子窗口因 airspace 仍可接收视口输入。

## 冻结目标

- T1：确认窗口内危险弹窗真实锁定源，并让 Window Tunnel 键盘事件优先交给活动 Dialog。
- T2：取消、确认、拒绝/失败、删除当前图层后均恢复可操作 UI 状态与选择/检查器同步。
- T3：完成自动删除回归、正式门禁、Commit/Push；不修改 Vulkan、Swapchain、Fence、Picking、Schema、Save/Load 或 Layer 业务规则。

## 修复边界

只修 `UiWin` DialogHost 键盘完成生命周期及删除回归；不新增第二个业务窗口，不重构输入系统。`CompleteDialog()` 在完成前清空 TCS，避免重复确认或旧任务残留。
