# 审计：RZ-New-0 新人接手规则审计

日期：2026-07-07
执行人：新人接手（按 `docs/dev-rules.md` 红线清单执行）
范围：接手验收，不做任何功能改动。对应任务书 RZ-New-0。

---

## 1. 当前 Git 分支与工作区状态

- 当前分支：`fix/RZ-Fix2-ui-baseline`
- 工作区存在未提交改动（含前序轮次产物）：
  - 已修改：`XuanYu.Editor.UI/app.manifest`、`file-tree.md`
  - 未跟踪：`docs/dev-rules.md`、`docs/dev-rules-understanding.md`、`.workbuddy/`、`qizheng-mvp-fixed/`
- 本轮 RZ-New-0 **只新增本文档，不改动任何代码、布局、输入或 Vulkan 逻辑**。

## 2. 当前 .cs / .axaml 是否全部 ≤100 行

- 检查口径：排除 `obj/`、`bin/`、`artifacts/` 生成目录后，统计 4 个生产项目的全部 `.cs` / `.axaml`。
- 结果：**全部 ≤100 行，最大为 `XuanYu.Editor.UI/Foot/Foot.axaml`（93 行）**。
- 结论：5+100 形态门禁当前达标。后续新增文件仍须守住单文件 ≤100 行，接近上限即拆分。

## 3. 是否发现 Editor.UI 直接引用 Silk.NET.Vulkan

- **发现：是（过渡期冲突，与设计目标依赖方向不一致）。**
- 位置：`XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.cs` 及其 partial 文件
  （`VulkanClearSession.Device.cs`、`VulkanClearSession.Swapchain.cs`、`VulkanClearSession.Dispose.cs`）
  直接 `using Silk.NET.Vulkan;` / `using Silk.NET.Vulkan.Extensions.KHR;`。
- 这说明 `Editor.UI` 当前直接持有 Vulkan 实现对象（`Instance` / `Device` / `SurfaceKHR` / `SwapchainKHR` 等字段），
  与 `docs/dev-rules.md` §2「`Editor.UI` 不得直接引用 `Silk.NET.Vulkan`、不得持有 Vulkan 对象」相悖。
- 性质：属 VK1/VK2 探针期的历史残留，需在 RZ-VK2-R3 经 `Render.Abstractions` / 平台组合根收口。

## 4. 是否发现 Editor.UI 直接引用 XuanYu.Render.Vulkan

- **发现：是（过渡期冲突）。**
- 位置（直接 `using XuanYu.Render.Vulkan;` 或引用其类型）：
  - `XuanYu.Editor.UI/ViewportNativeHostRoute.cs`
  - `XuanYu.Editor.UI/VulkanProbeRoute.cs`
  - `XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`
  - `XuanYu.Editor.UI/Vm/UiVm.VulkanProbe.cs`
  - `XuanYu.Editor.UI/Vm/UiVm.NativeHostLifecycle.cs`
- 结论：UI 层当前直接认识 Vulkan 实现。依 `docs/dev-rules.md` §2，后续应改为
  `Editor.UI` 只经 `Render.Abstractions` 认识抽象契约，Vulkan 后端由 `Editor.Win` / 组合根注入。

## 5. 当前 Vulkan 进度停在 VK 几

- 按正式里程碑文档 `docs/vulkan-lifecycle-plan.md`：仅 VK0（架构边界）/ VK1（能力探针）已规划与文档化。
- **但工作树实际状态已超出该里程碑表述**：`XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession`
  作为「Vulkan Clear Probe」已实现了 Instance / Surface / Device / Swapchain 的创建与释放，
  对应 `changelog.md` 的 `RZ-Fix3-A`（中央视口最小 Vulkan 接入前置验证）。
- 性质：这是 `vulkan-lifecycle-plan.md` §12 明确警告「禁止把旧 VulkanClearProbe 直接搬回正式路径」的探针，
  **不是正式的 VK3/VK4 生命周期架构**（`VulkanSurfaceOwner` / `VulkanSwapchainOwner` 尚未建立）。
- 结论：物理代码已越过 VK3/VK4 的对外能力，但正式生命周期架构未建立；新人不应将此探针视为 VK3 交付物，更不得扩展它。

## 6. 当前是否已经创建 Surface

- **是（在 `VulkanClearSession` 探针内）。**
- `VulkanClearSession.CreateSurface` 通过 `KhrWin32Surface.CreateWin32Surface` 创建 `SurfaceKHR`，
  位于 `XuanYu.Editor.UI/Viewport/Vulkan/VulkanClearSession.cs:70-75`。
- 该 Surface 属探针实现，非正式 `VulkanSurfaceOwner` 生命周期。

## 7. 当前是否已经创建 Swapchain

- **是（在 `VulkanClearSession` 探针内）。**
- `VulkanClearSession.CreateSwapchain` 创建 `SwapchainKHR`（`VulkanClearSession.Swapchain.cs:8-26`）；
  且 `Resize(uint, uint)` 在尺寸变化时直接 `DestroySwapchain` + `CreateSwapchain`
  （`VulkanClearSession.cs:44-50`）——这正是 `vulkan-lifecycle-plan.md` §8 所反对的
  「每次 Resize 即重建 Swapchain」模式，目前仅存在于探针内。
- 正式 `ViewportResizeGate` / `VulkanSwapchainOwner` 统一重建入口尚未建立。

## 8. 当前是否已经创建 LogicalDevice

- **是（在 `VulkanClearSession` 探针内）。**
- `VulkanClearSession.CreateDevice` 创建 `Device`（`VulkanClearSession.Device.cs:36-48`），并选取支持 Present 的队列族。

## 9. NativeHost 日志是否存在高频 SizeChanged 直接进 EditorLogBus 的风险

- **确认存在（与截图现象一致）。**
- 路径：`XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.cs`
  - `OnSizeChanged`（行 34-41）每次尺寸变化都调用 `Report(...)`；
  - `Report` → `ViewportNativeHostRoute.Report` → `UiVm.LogNativeHostLifecycle`
    （`Vm/UiVm.NativeHostLifecycle.cs:7-17`）；
  - 该函数对**每次**事件调用 `_logBus.Info/Warning(...)` 且 `_logBus` 之后还 `RefreshLogBindings()`。
- 后果：窗口拖拽时 `OnSizeChanged` 高频触发，每次都写一条 `【NativeHost】尺寸变化` 日志并刷新绑定，
  即截图中的「重复 138 次」。此属 `docs/dev-rules.md` §4「高频事件须在源头合并，总线只收低频事实」的违规。
- 注意：当前 `EditorLogBuffer` 的「连续相同折叠」只是显示层保护，**不能作为允许高频事件进总线的理由**。
  必须在 `VulkanNativeHost.OnSizeChanged` 源头做 debounce / coalesce（见后续建议与 RZ-VK2-R1）。

## 10. 本轮后续建议

1. **立即修**：RZ-VK2-R1 合并 NativeHost 尺寸变化日志（250ms debounce，稳定后只写 1 条合并日志，含宽/高/DPI/生命周期版本/合并次数；Detach/Dispose 安全停 pending；0 尺寸/无效句柄只更新状态不写普通成功日志）。不碰 Surface / Swapchain / LogicalDevice / 渲染循环。
2. **预审计**：RZ-VK2-R2 确认 `Editor.UI` 对 `Render.Vulkan` / `Silk.NET.Vulkan` 的直接认识范围，设计经 `Render.Abstractions` / `Editor.Win` 组合根装配方案。
3. **修正依赖**：RZ-VK2-R3 把 `Editor.UI` 对 Vulkan 实现的直接认识降到最低，UI 只认抽象契约。
4. **不提前进 VK3/VK4**：正式 Surface / Swapchain 生命周期应按 `VulkanSurfaceOwner` / `VulkanSwapchainOwner` 重构，而非扩展现有 `VulkanClearSession` 探针。
5. **文案**：中央视口 `Vulkan Clear Probe` 字样应改为 `NativeHost Probe` 或 `Vulkan Probe`（已纳入 RZ-VK2-R1）。

---

## 禁止项确认（RZ-New-0）

- [x] 未改 UI 布局
- [x] 未改输入逻辑
- [x] 未创建 Surface
- [x] 未创建 Swapchain
- [x] 未创建 LogicalDevice
- [x] 未重构项目依赖
- [x] 未删除历史代码
- [x] 未新增 Vulkan 功能
- [x] 只提交文档，未做功能改动

## 构建 / 测试

- 本轮无代码改动，未触发 build。
- 仓库当前无独立测试项目（`CodeFileBudgetTests` 等为历史测试，不属于本 checkout）；后续 `dotnet test` 会提示无测试项目，属已知状态。
