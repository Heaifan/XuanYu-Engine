# rz-vk4-closure.md — VK4 阶段正式收口确认（PhysicalDevice → LogicalDevice → Queue → Swapchain → Clear + Present）

## 验收结论

VK4 全阶段验收通过，正式收口。

> **VK4 已完成：编辑器视口从「黑屏」正式进入「单色清屏画面」。**
> Vulkan 已经跑通「Instance → Surface → PhysicalDevice → LogicalDevice → Queue → Swapchain → RenderPass → Framebuffer → CommandBuffer → 帧同步 → Acquire → Submit → Present」的完整最小闭环。
> 但**尚未进入几何渲染**——画面里没有任何图元/网格/物体是预期结果；那属于 VK5。

收口日期：2026-07-09。分支：`fix/RZ-VK3-A-surface-contract`。

## 1. 阶段收口清单

| 阶段 | 内容 | 结论 |
|---|---|---|
| VK4-A | PhysicalDevice 选择（枚举设备 / 查 Graphics+Present 队列族 / 查 Surface 呈现支持 / 优先独显） | ✅ 完成（含 VK4-A-R1 Bridge 压回 96 行） |
| VK4-B | LogicalDevice + Graphics/Present 队列创建 | ✅ 完成 |
| VK4-C | Swapchain + Swapchain Images + ImageViews（含 VK4-C-R1 Resize 重建审计、启用 VK_KHR_swapchain 设备扩展） | ✅ 完成 |
| VK4-D | Clear + Present 单色清屏闭环（RenderPass / CommandPool / Framebuffer / CommandBuffer[] / Sem×2 + Fence / 后台 Present 泵） | ✅ 完成 |
| VK4-D-R1 | Clear+Present 运行审计 + Resize 去重 | ✅ 完成 |
| VK4-D-R2 | 修复后台 Present 泵日志回调跨线程访问 Avalonia 导致的闪退（退出码 -532462766） | ✅ 完成 |
| VK4-D-R3 | Present 泵 OutOfDate 优雅降级 + Resize 日志顺序 + 物理像素诚实日志 | ✅ 完成 |
| VIEWPORT-RESIZE-R1 | 日志详情栏展开/收起后 NativeHost 最终尺寸主动同步（不再慢半拍） | ✅ 完成 |
| VIEWPORT-RESIZE-R2 | 修复 R1 的 DPI 逻辑尺寸 / 物理像素尺寸错配（`physical = round(logical × DPI)`） | ✅ 完成 |

## 2. 已成立链路

Attach（创建）方向：

```
NativeHost HWND
   → Vulkan Instance
   → Vulkan Surface
   → PhysicalDevice Selection        (VK4-A)
   → LogicalDevice + Graphics/Present Queue   (VK4-B)
   → Swapchain + Images + ImageViews  (VK4-C)
   → RenderPass / CommandPool / Framebuffer[] / CommandBuffer[]   (VK4-D)
   → Sem×2 + Fence → Acquire → Submit → Present（后台泵，单帧清屏）  (VK4-D)
```

Detach（释放）方向（真机验证）：

```
Present 泵停止
   → ClearFrame 释放（Framebuffers → CommandPool → RenderPass → Sync）
   → Swapchain 释放（ImageViews → Swapchain）
   → LogicalDevice 释放
   → Surface 释放
   → Instance 销毁
   → 分离完成
```

## 3. 当前已验证（用户真机，2026-07-09）

| 验收项 | 结果 |
|---|---|
| 首帧 Present 成功 | ✅ |
| 蓝灰色（clear 0.25/0.45/0.70）覆盖整个 NativeHost 区域 | ✅（VIEWPORT-RESIZE-R2 修好半屏黑） |
| Resize 后恢复（画面随窗口尺寸变化仍单色，无撕裂/无崩溃） | ✅ |
| 详情栏展开/收起视口立即同步，不慢半拍 | ✅（VIEWPORT-RESIZE-R1） |
| Win32 子窗口物理尺寸 = 逻辑尺寸 × DPI（DPI=1.75 时 713×188 → 1248×330） | ✅（VIEWPORT-RESIZE-R2） |
| Swapchain / Framebuffer / RenderArea extent 与 Win32 子窗口物理尺寸同源 | ✅（VK4-D-R3 同源物理像素） |
| 控制台 Vulkan 日志单出口去重（每条仅一次） | ✅ |
| 无闪退 / 无未响应（后台泵日志回调经 Dispatcher 切回 UI 线程） | ✅（VK4-D-R2 修） |
| `XuanYu.Render.Vulkan` + `XuanYu.Editor.UI` 双项目 0 warning / 0 error | ✅ |
| 全部相关 `.cs` ≤100 行 | ✅ |

关键日志（来自编辑器面板 / 控制台，示意）：

```text
【VulkanBridge】附加成功：Instance + Surface 已创建
【Vulkan】已选择物理设备：NVIDIA GeForce RTX 3060（独显优先）
【Vulkan】逻辑设备与队列创建成功（Graphics + Present）
【Vulkan】Swapchain 创建成功；extent（物理像素）：1248x330
【Vulkan】RenderPass 创建成功
【Vulkan】Framebuffer 创建成功 N 张
【Vulkan】帧同步原语创建成功
【Vulkan】首帧 Clear+Present 成功
```

## 4. 收口确认（VK4 红线全程守住）

- 未进入场景渲染 / 相机 / 网格 / 材质 / Gizmo / UI 叠加 / 拾取 / 持续动画（均属 VK5+）。
- Resize 只重建 Swapchain + ImageViews + Framebuffer[]；**不重建** Surface / Instance / LogicalDevice / Queue / RenderPass / CommandPool / Sync。
- `Editor.UI` 不新增 `Silk.NET.Vulkan` 使用点（本阶段仅动 Win32 `user32` P/Invoke 与 Avalonia 调度）。
- Present 泵运行在 `Render.Vulkan` 内部后台线程，日志回调经 `Dispatcher.UIThread` 切回 UI 线程，不在 UI 线程做无限等待。
- 帧同步用信号量 + 栅栏；栅栏初值 SIGNALED；`SuboptimalKhr` 当成功码，`ErrorOutOfDateKhr` 才当错误并优雅降级。
- 控制台 Vulkan 日志单出口（统一经 `VulkanBridgeLogFormatter.Emit`）。
- 全部新增/修改 `.cs` ≤100 行；`Render/`、`Swapchain/`、`Device/`、`Bridge/`、`Session/` 各子目录核心文件数受控。

## 5. 仍需长期注意（跨 VK5+ 的硬规则）

1. **Avalonia `Bounds` 是逻辑尺寸**（DPI 无关的设备无关像素）。
2. **Win32 子窗口 / Vulkan Surface 是物理像素尺寸**（`SurfaceCapabilitiesKHR.CurrentExtent` 为物理像素）。
3. **`physical = round(logical × DPI)` 是 NativeHost 同步的硬规则**——凡把逻辑尺寸喂给裸 `SetWindowPos` / Swapchain 的地方都必须先乘 DPI；VIEWPORT-RESIZE-R2 就是漏了这一步导致半屏黑。
4. **Swapchain / Framebuffer / RenderArea extent 三者同源物理像素**，且优先采用 `caps.CurrentExtent`（Win32 上有效时忽略传入逻辑尺寸）。
5. **Swapchain 重建必须传 `OldSwapchain`**：先用旧句柄建新，成功后再销毁旧 ImageView → 旧 Swapchain（先 ImageView 后 Swapchain），否则 Windows 报 `VK_ERROR_NATIVE_WINDOW_IN_USE_KHR`。
6. **`Render.Vulkan` 不允许引用 Avalonia**；只能持有 `Action<string> log` 回调，回调消费方（UI 侧）负责线程切回。
7. **`Editor.UI` 不允许直接接触 `Silk.NET.Vulkan` 类型**；只经 `Render.Abstractions` 契约与组合根接线。遗留的 `VulkanClearSession.*` 属死代码/非活跃链路，后续应清理，不得复用进正式路径。
8. **控制台 Vulkan 日志单出口**：低层与 Bridge step 只 `log?.Invoke(m)`，禁各自 `Console.WriteLine`。
9. **生产不注入种子/示例日志**；空状态用「暂无日志」占位。

## 6. 已知债务（收口不消除，移交后续）

- `XuanYu.Editor.UI` 仍因历史 Vulkan 探针（`VulkanProbeRoute` / `VulkanClearSession` 等）保留对 `Render.Vulkan` 的工程级引用与直接 `using Silk.NET.Vulkan`。目标方向仍为 `Editor.UI → Abstractions`、`Editor.Win → Vulkan`。VK5 不得扩大 UI 对 Vulkan 的直接认识；死代码 `VulkanClearSession.*` 建议择机清理。

## 7. 下一阶段

VK4 收口后进入 **VK5 最小几何渲染**（见 `docs/rz-vk5-plan.md`）。VK5 第一步不是场景渲染，而是「固定三角形 / 最小 Pipeline / 最小 Draw」。
