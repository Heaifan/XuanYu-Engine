# RZ-VK5-E 规划 · 清理 VulkanClearSession 死代码（债务 B）

> 轮次性质：**先规划、确认后再实装**。本轮产出规划文档，不实装。
> 分支：`fix/RZ-VK3-A-surface-contract`；基线 HEAD：`139c748`（RZ-VK5-C 封版）。

## 0. 一句话结论

经源码审计，`VulkanClearSession`（Editor.UI 中 4 个 partial 文件）是 VK3-A 之前的早期探针实现，
已被 `VulkanRenderSession` 正式链路完全取代。当前**无任何 `.cs` 外部引用或调用方**，属确定无运行时职责的死代码。
VK5-E 目标：**删除这 4 个文件**，收口债务 B。

---

## 1. 是否仍有真实引用？（确认死代码）

- 全仓 grep `VulkanClearSession`：命中**仅** 4 个 partial 文件自身 + 历史 markdown 文档
  （changelog / file-tree / docs/audit-* / docs/rz-*/ docs/vulkan-preflight-*）。
- 无任何 `.cs` 外部引用：无 `new VulkanClearSession`、无字段声明、无 `TryCreate` 调用方、无 `using` 别名引用。
- `TryCreate`（VulkanClearSession.cs:38）仅定义，全仓无调用点 → 入口已死。
- 结论：**确定无引用、无运行时职责**。

## 2. 文件、调用方、替代链路、删除影响

**文件清单**（均在 `XuanYu.Editor.UI/Viewport/Vulkan/`）：
- `VulkanClearSession.cs` — 主 partial：Instance/Surface/Device 装配 + `TryCreate` + `Resize` + `Check`
- `VulkanClearSession.Device.cs` — `PickDevice` / `TryPickQueue` / `CreateDevice`
- `VulkanClearSession.Swapchain.cs` — `CreateSwapchain` / `PickFormat` / `DestroySwapchain`
- `VulkanClearSession.Dispose.cs` — `Dispose`：释放 Device/Surface/Instance

**调用方**：**无**（仅类内部互调，4 文件 partial 合并为同一 `VulkanClearSession` 类）。

**替代链路（现正式）**：
`VulkanNativeHostSurfaceBridge`（Render.Vulkan）→ `VulkanBridgeRenderSessionAttachStep.Run`
→ `VulkanRenderSession.Create`（组合 ClearFrameOwner + PresentLoop + PipelineOwner + SwapchainOwner）。
Editor.UI 侧经 `VulkanSurfaceBridgeProvider` 注入契约，UI 宿主只认 `INativeHostSurfaceBridge` 抽象，不碰具体 Vulkan 类型。

**删除影响**：仅移除一组自包含的早期实现；不影响 `VulkanRenderSession` 任何能力；
不改变三角形 / Resize 自愈 / PresentLoop / Pipeline 生命周期；不影响 UI；
Editor.UI 仍引用 `VulkanSurfaceBridgeProvider` 等正式桥接，编译不受影响（已无类型依赖）。

## 3. 正式链路由 VulkanRenderSession 承担（确认）

- `VulkanRenderSession` 是 VK4-D 起的正式组合根：持有 ClearFrameOwner / PresentLoop / PipelineOwner / SwapchainOwner，负责 Attach / Resize / Detach。
- 它在 `VulkanBridgeRenderSessionAttachStep.Run`（Render.Vulkan/Bridge）被创建，由 `VulkanNativeHostSurfaceBridge.Attach` 驱动
  （`VulkanNativeHostSurfaceBridge.cs:18` 持有 `_renderSession`）。
- `VulkanClearSession` 与 `VulkanRenderSession` 在**类型、命名空间、引用上零交集**：
  前者 Editor.UI 自管 Silk.NET（早期直连），后者 Render.Vulkan 经 `XuanYu.Render.Abstractions` 契约接入 UI。

## 4. 只删确定无引用、无运行时职责的死代码

- 4 个文件整体删除（`git rm`），**不改动任何现存功能代码**。
- 不触碰 `VulkanRenderSession` / `VulkanClearFrameOwner` / `VulkanPresentLoop` / `VulkanGraphicsPipelineOwner` / `VulkanSwapchainOwner` / Bridge 链路。
- 删除后 Editor.UI 编译不受影响（已无类型依赖，grep 已证）。

## 5. 不改现有三角形绘制、Resize 自愈、PresentLoop、Pipeline 生命周期

红线：不进 VK5-E 之外任何功能；不动 Render.Vulkan 功能代码；不动 UI。

## 6. 不新增功能

纯删除，零新增类型 / 方法。

## 7. 全 .cs ≤100

删除文件自然满足；现存文件不受影响，维持 ≤100。

## 8. 双项目 0W0E

删除后 `dotnet build` 应 0W0E（无残留引用）。实装时用低内存构建验证。

## 9. 实装步骤（确认后再执行）

1. `git rm XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.cs(.Device/.Swapchain/.Dispose).cs` 四个文件。
2. 低内存构建验证（沙箱防 OOM）：
   `MSBUILDDISABLENODEREUSE=1 dotnet build XuanYu.Editor.UI/XuanYu.Editor.UI.csproj --configfile NuGet.Config -nologo -maxCpuCount:1 -p:UseSharedCompilation=false --no-incremental`
   与 Render.Vulkan 同理，确认 0W0E；若报“编辑器进程锁 DLL”（MSB3027），`Stop-Process -Id <pid> -Force` 后重构建。
3. 更新 `changelog.md` + `file-tree.md`（登记 VK5-E 实装快照）。
4. 独立 commit（债务 B 收口，不混入其他轮），push 到 origin 并附 hash。

## 10. 风险与回滚

- 风险：极低（已确认零引用）。残留隐患仅可能是某处字符串 / 反射引用——grep 已排除。
- 回滚：`git revert` 该 commit 即可恢复 4 文件。

## 11. 红线（本轮守住，不突破）

- 只删死代码，不新增 / 不修改功能代码。
- 不碰 `VulkanRenderSession` 链路 / UI / NativeHost / LOG-UX。
- 不扩大 `Editor.UI → Render.Vulkan` 引用（本次反而减少历史耦合）。
- 双项目 0W0E；全 `.cs` ≤100。

## 12. 进度指针

VK5-E 实装后，VK5 阶段既定收口项全部完成（VK5-A 建 Pipeline → VK5-B 三角形 → VK5-C 验证收口 → VK5-D 职责边界 → VK5-E 清死代码）。
后续进入相机 / 投影（宽高比修正）等全新阶段，由独立规划开启，不纳入 VK5 收口范畴。
