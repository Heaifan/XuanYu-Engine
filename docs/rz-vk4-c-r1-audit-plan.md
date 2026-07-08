# VK4-C-R1 审计与运行验证计划（只审计不新增能力）

日期：2026-07-08
分支：fix/RZ-VK3-A-surface-contract
上游：VK4-C 代码完成（含运行前置修正，见 changelog `[VK4-C]` / `[VK4-C-Fix]`）

## 定位

VK4-C 代码已按报告合格，但**不能判完全收口**——Swapchain 是第一个真正容易在运行时炸的 Vulkan 阶段，`build 通过 ≠ 运行时能创建成功`。本阶段只做**审计与运行验证**，不新增任何 Vulkan 能力，不进入 VK4-D。

## 已完成的静态审计结论（代码层）

| 项目 | 判断 |
| --- | --- |
| 只做 Swapchain + Images + ImageViews | ✅ 合格 |
| 未做 RenderPass / Framebuffer | ✅ 合格 |
| 未做 CommandPool / CommandBuffer | ✅ 合格 |
| 未 Clear / Present | ✅ 合格 |
| Resize 只重建 Swapchain + ImageViews | ✅ 合格（先建新 → DestroyImagesAndViews 先 ImageView 后 Swapchain → 赋值新） |
| Dispose 顺序 ImageViews → Swapchain | ✅ 方向正确（Bridge.Detach 先 `_swapchainOwner?.Dispose()`） |
| Bridge 98 行 | ✅ 合格但贴边 |
| 所有文件 ≤100 行 | ✅ 合格（最大 DeviceOwner 99） |
| 两项目 0W0E | ✅ 合格 |
| 文档同步 | ✅ 合格 |

## VK4-C-Fix 已补的运行前置缺口（代码层，非 R1 新增能力）

1. **`VK_KHR_swapchain` 设备扩展已启用**：`VulkanDeviceOwner.Create` 现设置 `DeviceCreateInfo.EnabledExtensionCount=1` + `PpEnabledExtensionNames`，扩展名由调用方 `VulkanSwapchainOwner.DeviceExtensionName` 传入。否则运行时 `CreateSwapchainKHR` 会失败。
2. **0 尺寸 Resize 跳过**：`VulkanSwapchainOwner.Recreate` 在 `width<=0 || height<=0` 时记 `Skipped` 并 return，不崩溃。
3. **格式/范围只读暴露**：`VulkanSwapchainOwner` 暴露 `Format` / `Extent` / `ImageViews`，供 VK4-D 直接使用。

R1 必须在真机确认：扩展确实启用且 Swapchain 创建成功（静态已修，运行时复核）。

## 运行时验证清单（真机 / Codex 执行）

1. LogicalDevice 创建时是否启用了 `VK_KHR_swapchain` 设备扩展（R1 第一条，最致命）。
2. 启动编辑器后，Swapchain 是否创建成功。
3. 日志是否显示 Surface capabilities / format / present mode / extent（`【VulkanSwapchain】能力查询成功；格式=...；呈现模式=...；extent=...`）。
4. 日志是否显示 Swapchain Images 数量（`Created(N)`）。
5. 日志是否显示 ImageViews 创建成功（同一条 `Created(N)`）。
6. Resize 时只重建 Swapchain + ImageViews（日志 `Recreated(w,h,N)`）。
7. Resize 不重建 Surface / Instance / LogicalDevice / Queue。
8. Resize 遇到 0 宽或 0 高时跳过创建（日志 `Skipped(0 尺寸跳过重建 ...)`），不崩溃。
9. 关闭时释放顺序必须是：ImageViews → Swapchain → LogicalDevice → Surface → Instance（日志 `Disposed` → `LogicalDevice 释放成功` → `Surface 已释放` → `Instance 已销毁`）。
10. 仍然黑屏是预期。
11. 不出现 RenderPass / Framebuffer / CommandPool / CommandBuffer / Clear / Present 实装。
12. UI 不新增 Silk.NET.Vulkan 引用。
13. 所有 .cs 文件 ≤100 行。
14. XuanYu.Render.Vulkan 与 XuanYu.Editor.UI 构建 0W0E。

## 严禁

- 不要进入 VK4-D。
- 不要为了测试而加 Clear / Present。
- 不要把 Swapchain 逻辑塞回 Bridge。
- 不要让 DeviceOwner 增加 Swapchain 职责。

## 给 Codex 的 VK4-C-R1 指令（可直接发）

```md
进入 VK4-C-R1，只做审计与运行验证，不新增 Vulkan 能力，不进入 VK4-D。

重点验证：
1. LogicalDevice 创建时是否启用了 VK_KHR_swapchain 设备扩展。
2. 启动编辑器后，Swapchain 是否创建成功。
3. 日志是否显示 Surface capabilities / format / present mode / extent。
4. 日志是否显示 Swapchain Images 数量。
5. 日志是否显示 ImageViews 创建成功。
6. Resize 时只重建 Swapchain + ImageViews。
7. Resize 不重建 Surface / Instance / LogicalDevice / Queue。
8. Resize 遇到 0 宽或 0 高时跳过创建，不崩溃。
9. 关闭时释放顺序必须是：
   ImageViews → Swapchain → LogicalDevice → Surface → Instance。
10. 仍然黑屏是预期。
11. 不出现 RenderPass / Framebuffer / CommandPool / CommandBuffer / Clear / Present 实装。
12. UI 不新增 Silk.NET.Vulkan 引用。
13. 所有 .cs 文件 ≤100 行。
14. XuanYu.Render.Vulkan 与 XuanYu.Editor.UI 构建 0W0E。

严禁：
- 不要进入 VK4-D。
- 不要为了测试而加 Clear / Present。
- 不要把 Swapchain 逻辑塞回 Bridge。
- 不要让 DeviceOwner 增加 Swapchain 职责。
```

## 收口判定

三项全过（Swapchain 创建成功 / Resize 重建正确 / Detach 顺序正确）→ VK4-C 正式收口 → 开 VK4-D（ClearFrame 出画面）。
