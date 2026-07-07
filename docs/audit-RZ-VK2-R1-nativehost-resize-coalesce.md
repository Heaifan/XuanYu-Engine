# 审计：RZ-VK2-R1 NativeHost 尺寸变化日志合并

日期：2026-07-07
执行人：新人接手（RZ-New-0 验收通过后执行）
范围：只修 NativeHost Resize 高频日志泄漏，不进入 Surface / Swapchain。
对应任务书 RZ-VK2-R1。

---

## 1. 问题

底部日志曾出现：

```text
【NativeHost】尺寸变化
重复 138 次
```

根因：`XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs` 的 `OnSizeChanged`
每次尺寸变化都调用 `Report` → `ViewportNativeHostRoute.Report` → `UiVm.LogNativeHostLifecycle`，
对**每一次**事件写一条 `EditorLogBus` 日志并 `RefreshLogBindings()`。
窗口拖拽时 `OnSizeChanged` 高频触发，导致同一日志反复进入总线（显示层折叠只是保护，不是源头治理）。

违反 `docs/dev-rules.md` §4「高频事件先在源头合并，日志总线只接收低频事实」。

## 2. 实现

- 新增 `XuanYu.Editor.UI/NativeHostResizeSnapshot.cs`：只保存尺寸数据（宽/高/DPI/是否有效/HWND）。
- 新增 `XuanYu.Editor.UI/NativeHostResizeCoalescer.cs`：250ms debounce；连续 `OnResize` 只更新快照与合并计数，
  250ms 内无新变化后才捕获一条 `NativeHostHandleSnapshot` 并回调，生成一条低频合并日志；`Cancel()` 安全停止 pending。
- `XuanYu.Editor.UI/ViewportNativeHostRoute.cs`：增加 `ReportMerged` 薄入口。
- `XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs`：增加 `LogNativeHostResizedMerged`，
  合并日志含最终宽度、高度、DPI、生命周期版本、合并次数；无效句柄只写一条低频失效日志。
- `XuanYu.Render.Vulkan/NativeHostLifecycleLogFormatter.cs`：增加 `MergedMessage` 中文合并日志格式。
- `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`：`OnSizeChanged` 改为走 `Coalescer`；
  `OnDetachedFromVisualTree` / `DestroyNativeControlCore` 调用 `_resizer.Cancel()`，Dispose 后不补写日志。
- 中央视口文案 `Vulkan Clear Probe` 改为 `NativeHost Probe`（`Main.axaml`）与 `Vulkan Probe`（`VulkanViewport.axaml`）。

## 3. 职责拆分（满足「不允许一个文件承担四种职责」）

- Snapshot：只保存尺寸数据。
- Coalescer：只负责合并连续尺寸变化。
- LogFormatter：只负责中文日志格式化。
- Route：只负责 UI 层事件转发。
- VulkanNativeHost：只负责 HWND 生命周期与尺寸事件入口，不再直写日志。

## 4. 验收（对照任务书 18 项）

| # | 验收项 | 结果 |
|---|--------|------|
| 1 | NativeHost 创建日志正常 | ✅ 未改动 Created 路径（仍走立即 Report） |
| 2 | 附加到可视树日志正常 | ✅ Attached 未改动 |
| 3 | HWND 可用日志正常 | ✅ HandleAvailable 未改动 |
| 4 | Resize 后不再出现「重复 XXX 次」 | ✅ 源头合并，不再逐条进总线 |
| 5 | Resize 后只出现 1 条合并日志 | ✅ 250ms 稳定后 emit 一次 |
| 6 | 合并日志含宽/高/DPI/版本/合并次数 | ✅ `MergedMessage` 全字段 |
| 7 | 工具切换日志仍正常 | ✅ 未触碰 |
| 8 | 日志过滤仍正常 | ✅ 未触碰 |
| 9 | 日志详情仍正常 | ✅ 未触碰 |
| 10 | 没有创建 Surface | ✅ 未改 `VulkanClearSession` |
| 11 | 没有创建 Swapchain | ✅ 未改 `VulkanClearSession` |
| 12 | 没有创建 LogicalDevice | ✅ 未改 `VulkanClearSession` |
| 13 | 没有接入真实渲染循环 | ✅ 未触碰 |
| 14 | 没有修改输入链路 | ✅ 仅 `OnSizeChanged` 日志路径 |
| 15 | 所有 .cs/.axaml ≤100 行 | ✅ 最大 VulkanNativeHost.cs=72 |
| 16 | file-tree.md 已同步 | ✅ 见文末 |
| 17 | audit 文档已新增 | ✅ 本文档 |
| 18 | changelog.md 已更新 | ✅ 见 `changelog.md` |

## 5. 禁止项确认

- [x] 未创建 Vulkan Surface
- [x] 未创建 Swapchain
- [x] 未创建 LogicalDevice
- [x] 未创建 CommandPool / CommandBuffer
- [x] 未接入真实渲染循环
- [x] 未做 Vulkan 清屏
- [x] 未改顶部菜单、左侧项目树、右侧检查器、底部日志整体布局
- [x] 未改输入逻辑
- [x] 未把 PointerMoved / Hover / DragPreview / RenderFrame 接入日志
- [x] 未用日志折叠掩盖高频事件（改为源头合并）

## 6. 构建 / 测试

- `dotnet restore` 通过。
- `dotnet build --no-restore` 通过：**0 Warning / 0 Error**。
- `dotnet test` 退出正常，仓库无独立测试项目（已知状态，与 RZ-VK2 记录一致）。

## 7. 后续

- RZ-VK2-R2：依赖方向预审计（确认 `Editor.UI` 对 `Render.Vulkan` / `Silk.NET.Vulkan` 的直接认识范围）。
- RZ-VK2-R3：经 `Render.Abstractions` / `Editor.Win` 组合根收口，使 UI 只认抽象契约。
- RZ-VK3：正式 Surface 生命周期最小接入（重建 `VulkanSurfaceOwner`，不扩展现有探针）。
