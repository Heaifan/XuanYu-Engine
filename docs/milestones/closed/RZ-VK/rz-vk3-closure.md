# rz-vk3-closure.md

## 验收结论
VK3 验收通过，可以收口。

> **VK3 已完成：NativeHost HWND 生命周期已经正式接入 Vulkan Instance + Surface。**
> 但尚不能说"Vulkan 已开始渲染画面"——Swapchain 与渲染循环属于 VK4。

## 验收项（截图 + 面板日志确认）
| 验收项 | 结果 |
|---|---|
| 编辑器正常启动 | ✅ |
| NativeHost 创建成功 | ✅ |
| Vulkan 探针识别 RTX 3060 | ✅ |
| VulkanBridge 日志进入面板 | ✅ |
| Attach 创建 Instance + Surface | ✅ |
| Resize 不重建 Surface | ✅ |
| 旧提示已改为"Surface 已接入，Device/Swapchain 未接入" | ✅ |
| 当前黑屏 | ✅ 预期（Swapchain 未建） |

关键日志（来自编辑器面板）：
```text
【VulkanBridge】附加成功：Instance + Surface 已创建；窗口句柄：0x3C0E8E
【VulkanBridge】尺寸变化已接收：不重建 Surface；宽度：714，高度：555
【VulkanBridge】尺寸变化已接收：不重建 Surface；宽度：714，高度：329
```

## 已完成阶段
- VK3-A：Abstractions 契约层 ✅
- VK3-B1：VulkanInstanceOwner ✅
- VK3-B2：VulkanSurfaceOwner ✅
- VK3-C1：Bridge 生命周期封装 ✅
- VK3-C2：接入 NativeHost 生命周期 ✅
- VK3-C2-R1：日志面板可见性 ✅

## 收口确认（红线遵守）
- 未选 PhysicalDevice、未创 LogicalDevice、未取 Queue、未建 Swapchain、未碰 RenderFrame。
- Resize 不重建 Surface（桥 Resize 仅记中文日志）。
- 未搬 VulkanClearSession 探针到正式路径；旧探针未改动。
- UI 不直接持有 Vulkan 资源；经 Abstractions 契约 + 组合根接线。
- VK3 阶段所有相关 `.cs/.axaml` ≤100 行（阶段审计已确认）。
- `dotnet build` 0W0E（VK3-C2-R1 验收记录）。

## 已知债务（收口不消除，移交 VK4）
`XuanYu.Editor.UI` 仍因历史 Vulkan 探针（`VulkanProbeRoute` / `VulkanClearSession` 等）保留对 `Render.Vulkan` 的工程级引用与直接 `using Silk.NET.Vulkan`。目标方向仍为 `Editor.UI → Abstractions`、`Editor.Win → Vulkan`；VK4 不得扩大 UI 对 Vulkan 的直接认识。

## 收口日期
2026-07-08
