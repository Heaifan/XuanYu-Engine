# WAVE-2.5 Phase-1 Integration Report

任务：`WAVE-2.5 Phase-1 Integration`

冻结 HEAD：`17b8e09db685add70593ad51a395ec3f83536368`

集成提交：A `481b5c34e261e1b763080eb785f3209afe81cd09`；B `7abc72929879cfe9d4458863be4f5c9dc296041e`；C `634a2a5aea6f70aaa1878c920975c800cabbce3e`。

## 收敛结果

- Router：`ViewportInputRouter` 只负责平台无关事件分发与 Owner arbitration。
- Gesture Owner：唯一来源为 `ViewportGestureContext.Owner: GestureOwner`。
- Gesture Lifecycle：`ViewportGestureLifecycle.Current` 是唯一 Active 状态；Router 的 `State` 只是投影。
- Cancellation：Router 将 Cancel、CaptureLost、FocusLost、WindowDeactivated 映射到同一 `Lifecycle.Cancel(reason)`。
- Capture：Lifecycle 唯一调用 `IViewportPointerCaptureCoordinator.Capture/Release`；Router 不再直接释放。
- Platform Adapter：Native/Avalonia 继续在 Adapter 边界生成 `EditorPointerEvent`；核心 Router/Lifecycle 无 Native/Avalonia 分支。

## 冲突处理

| 项目 | 冲突 | 收敛处理 |
|---|---|---|
| Owner | A 的 `GestureOwner` 与 B 的字符串 Owner 重复 | B `ViewportGestureContext.Owner` 改为 `GestureOwner` |
| 状态 | A 的 Router.State 与 B 的 Lifecycle.Current 并行 | B `Current` 为事实源，Router.State 为只读投影 |
| 生命周期 | A 直接 End，B 独立 Commit/Cancel | Router 只触发 Lifecycle；终止顺序由 Lifecycle 统一执行 |
| Capture | A 直接调用 Capture/Release，B 使用独立 Action | Lifecycle 持有 A 的 coordinator，统一 Capture/Release |
| Cancel | Router 直接清空状态，Lifecycle 另行 Cancel | 所有终端事件进入唯一 `Lifecycle.Cancel(reason)` |
| 平台分支 | C 的 Native source mapping | 保留在 Native/Avalonia Adapter；核心层只收 `EditorPointerEvent` |

## C 路剩余 GAP

| GAP | BLOCK D1 | BLOCK D2 | BLOCK Final Gate | 说明 |
|---|---:|---:|---:|---|
| Delta | YES | NO | YES | Camera/Gizmo Consumer 需要明确移动增量契约 |
| X1 / X2 | NO | NO | YES | 当前 Phase-1 Consumer 不依赖侧键字段 |
| Horizontal Wheel | NO | NO | YES | 当前只冻结垂直 Dolly notch 语义 |
| Avalonia Capture Source | YES | YES | YES | 平台 Capture 来源仍需在 Consumer migration 前接入 |
| Timestamp | NO | NO | YES | 当前生命周期不依赖时间排序 |
| Device Type | NO | NO | YES | 当前 Consumer 不按设备类型分支 |

这些 GAP 保留在 `EditorPointerEvent` 设计审计中，本轮没有偷偷扩展模型。它们不构成当前 Phase-1 集成的 New Regression，但在 Final 25/25 READY 前必须逐项关闭或由正式决策豁免。

## 边界

- Consumer Migration：未启动。
- Production Input Route：未切换。
- Camera、Picking、Gizmo、Map Geometry、Region、Road、Marker、Snap：未修改。
- Vulkan、Composition、NativeControlHost、Win32ViewportHost：未重构或删除。
- WAVE-3：未启动。

一键入口：`scripts/test-wave-2.5-input-convergence.ps1`
