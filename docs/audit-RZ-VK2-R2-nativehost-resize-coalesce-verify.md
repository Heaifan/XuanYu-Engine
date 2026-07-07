# audit-RZ-VK2-R2-nativehost-resize-coalesce-verify.md

## 审计目标
RZ-VK2-R2 验证/收口轮：确认 RZ-VK2-R1（NativeHost Resize 日志合并）没有埋雷，且不新增任何 Vulkan 生命周期能力。

## 1. 工作区状态（git status --short）
```
 M XuanYu.Editor.UI/app.manifest
?? .workbuddy/
?? qizheng-mvp-fixed/
```
相对 HEAD 仅 `app.manifest` 为 tracked modified；`.workbuddy/`、`qizheng-mvp-fixed/` 未跟踪，非本轮内容。

## 2. app.manifest diff 判断
diff 仅在 `<assembly>` 内新增 `<compatibility>` 块（Windows 10/11/8.1/8/7 supportedOS）。
- 该改动出现于 RZ 轮次之前，不属于 RZ-New-0 / RZ-VK2-R1 / RZ-VK2-R2 任何一轮任务。
- 本轮为验证轮，不混入 Vulkan 任务，故**不纳入本提交**。
- 处置建议（二选一，由你决定，非本轮范围）：误改则 `git checkout -- XuanYu.Editor.UI/app.manifest`；若确为必要改动则单独提交说明。

## 3. 四问验证

### Q1：拖动窗口日志是否已从高频逐条变成合并写入？
是。
- `VulkanNativeHost.OnSizeChanged` 不再每帧直写日志，改为 `_resizer.OnResize(...)`（`VulkanNativeHost.cs:46`）。
- `NativeHostResizeCoalescer` 用 `DispatcherTimer`（250ms）合并：连续 SizeChanged 只更新 `_last` 与 `_mergeCount`，稳定后才 emit 一条（`NativeHostResizeCoalescer.cs:27-48`）。
- 合并日志经 `ReportMerged → LogNativeHostResizedMerged → MergedMessage`，仅写 1 条（`ViewportNativeHostRoute.cs:10-11`、`UiVm.NativeHostLifecycle.cs:19-26`）。
- 结论：原"重复 138 次"已消除，稳定后只出 1 条合并日志（含宽/高/DPI/版本/合并次数）。

### Q2：是否仍存在直接 EditorLogBus.Report 的高频路径？
无新增高频路径。
- 尺寸变化（Resized）事件已全部经 Coalescer 合并，不再逐条直写。
- 仍直写 `EditorLogBus` 的仅是非高频单次生命周期事件：Created / Attached / HandleAvailable / Detached / Disposed / Invalidated（`VulkanNativeHost.cs:26/29/35/52/59/60`）。这些在窗口生命周期中至多一次，非 SizeChanged 高频路径。
- `RefreshLogBindings()` 只在合并日志与单次事件时调用，不再随每帧尺寸变化触发。

### Q3：是否改动了 Surface / Swapchain / LogicalDevice？
否，全程未触碰。
- `git diff -- HEAD -- XuanYu.Editor.UI/Viewport/Vulkan/` 为空 → VulkanClearSession.* 零改动。
- 本回合新增/改动的 6 个文件均不引用 `VulkanClearSession`（已 grep 确认）。
- `VulkanNativeHost.OnSizeChanged` 仍每个尺寸变化调用 `Win32ViewportHost.Resize`（仅 `SetWindowPos`，`Win32ViewportHost.cs:20-21`），探针 Swapchain 重建时机与 RZ-VK2-R1 之前完全一致；Coalescer 回调只产生日志，未改变该调用时机。
- 结论：Coalescer 只合并 UI 生命周期日志，未间接影响 VulkanClearSession.Resize / Surface / Swapchain / LogicalDevice。

### Q4：是否仍存在 Editor.UI 直接引用 Vulkan 的过渡债务？
是，债务仍标红，但本轮未扩大。
- 仍直接 `using XuanYu.Render.Vulkan` / `Silk.NET.Vulkan` 的文件（与 RZ-New-0 一致，无新增）：
  - `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.*`（探针级，直接 Silk.NET.Vulkan）
  - `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`
  - `XuanYu.Editor.UI/ViewportNativeHostRoute.cs`
  - `XuanYu.Editor.UI/Vm/UiVm.VulkanProbe.cs`、`UiVm.NativeHostLifecycle.cs`
- RZ-VK2-R2 未新增任何 `Silk.NET.Vulkan` 使用点（确认无新增 `using Silk.NET.Vulkan`）。
- 处置：维持过渡债务，待 RZ-VK2-R3 经 Render.Abstractions / 组合根收口，不在 UI 内继续长出 Vulkan 细节。

## 4. 构建门禁
- `dotnet restore` / `dotnet build --no-restore`：0 Warning / 0 Error（残留编辑器进程锁 DLL 时，结束进程后重建即可，非代码错误）。
- `dotnet test`：命令退出 0，但**仓库无独立测试项目**，故"命令通过 ≠ 测试覆盖通过"，如实记录。
- 5+100：本回合仅新增 1 个 .md 审计文档；既有 .cs/.axaml 均 ≤100 行（最大 VulkanNativeHost.cs=72），未突破。

## 5. 结论
RZ-VK2-R1 日志合并边界干净：合并只作用于日志，未牵连 Vulkan 生命周期；无新增高频直写、无新增 Vulkan 使用点。可放行进 RZ-VK2-R3（依赖方向收口）后再进 VK3。app.manifest 与未跟踪目录由你另行处置，不混入本轮。
