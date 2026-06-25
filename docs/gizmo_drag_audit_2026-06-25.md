# Gizmo 拖动 Preview 高频路径审计

日期：2026-06-25

## 调用链与分级

| 顺序 | 调用/阶段 | 分级 | 结论 |
|---:|---|---|---|
| 1 | `WindowsVulkanViewportHostControl.WndProc` -> `HandlePointerMove` | 高频安全 | 原生鼠标移动入口，仅转发事件。 |
| 2 | `VulkanViewportHostPanel.RawPointerMoved` -> `EditorShellRawInputRoute.HandleRawPointerMoved` | 高频安全 | 进入编辑器输入路由。 |
| 3 | `EditorViewportInputRoute.HandlePointerMoved` | 高频安全 | 新增 `PointerMoved入口` probe；只记录坐标并路由。 |
| 4 | `EditorTransformInputRoute.HandlePointerMoved` | 可疑，已审计 | 执行 Gizmo hover/hit/drag 路由；probe 显示 Preview 中未触发 UI/WorldState/Diagnostics/Inspector。 |
| 5 | `TransformPointerRoute.UpdateGizmoHover` | 高频安全 | 屏幕空间 hit test，未访问 UI/WorldState。 |
| 6 | `TransformPointerRoute.OnPointerMoved` -> `TransformDragRoute.Move` | 高频安全 | 只计算 `PreviewTransform`。 |
| 7 | `EditorTransformApplyRoute.Preview` | 高频安全 | 只调用 Preview 应用层，不写 WorldState/Inspector。 |
| 8 | `EntityTransformPreview.Apply` | 高频安全 | 只更新 RenderScene 和 Vulkan entity position。 |
| 9 | `ViewportRenderSceneStore.UpdatePosition` | 高频安全 | 只替换内存 RenderScene。 |
| 10 | `EditorDiagnosticsRefreshRoute.ScheduleFrame(TransformPreview)` | 可疑，已修复 | 原先帧完成后无条件 `refresh()`；现 Preview 回调只记录“跳过 Diagnostics”。 |
| 11 | `Scene3dFrameSubmitRoute.Request` | 低频重负载/可疑边界 | 非 Preview 会构建 PickSnapshot；Preview 明确跳过。 |
| 12 | `Scene3dFrameRoute.Request` -> `Dispatcher.UIThread.Post` | 可疑边界 | 有入队/执行 probe；本次未见队列堆积。 |
| 13 | `Scene3dDrawListBuilder.Build` -> `VulkanScene3dSession.RenderFrame` | 低频重负载 | 实际 Preview Render；本次成功完成。 |
| 14 | `onCompleted` | 可疑，已修复 | Preview 只记录“跳过 Diagnostics 刷新”；Commit/Cancel 才允许刷新重负载 UI。 |

## Probe 覆盖点

- `PointerMoved入口`
- `Gizmo hit/drag 路由`
- `Preview transform 计算`
- `RenderScene preview 写入`
- `RenderScene 内存更新`
- `Redraw 请求 reason=TransformPreview`
- `Redraw 请求调度 reason=TransformPreview`
- `PickSnapshot 刷新跳过(Preview)`
- `Dispatcher.UIThread.Post 入队/执行`
- `Preview Render 完成`
- `Preview 帧跳过 Diagnostics 刷新`
- `Inspector 刷新`
- `Diagnostics 刷新`
- `WorldState 写入`
- `日志面板刷新`
- `WorldHierarchy 刷新`

## 本次拖动日志结论

日志文件：`docs/gizmo_drag_audit_probe.log`

- 总日志行数：355
- `UI=是`：0
- `WorldState=是`：0
- `Diagnostics=是`：0
- `Inspector=是`：0
- `PickSnapshot 刷新跳过(Preview)`：20
- `Preview 帧跳过 Diagnostics 刷新`：20
- `Preview Render 完成 success=True`：20

结论：本次 Move Gizmo 拖动 Preview 高频路径没有触发 Inspector、Diagnostics、PickSnapshot 重建、WorldState 写入、WorldHierarchy、日志面板刷新或 Avalonia UI 刷新标记。Preview 当前只更新 RenderScene/Vulkan 预览并请求渲染帧；Commit/Cancel 路径保留 Inspector/WorldState/Diagnostics 刷新能力。
