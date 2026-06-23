# 8.7.8D-1 — VulkanSwapchainProbe 拆分审计

审计日期：2026-06-23
目标文件：`FluidWarfare.Render.Vulkan/Swapchain/VulkanSwapchainProbe.cs`
目标行数：301 行

---

## 1. 当前文件状态

| 维度 | 值 |
|------|-----|
| **文件路径** | `FluidWarfare.Render.Vulkan/Swapchain/VulkanSwapchainProbe.cs` |
| **行数** | 301 行 |
| **类型** | `public static unsafe class` — 纯静态，全部指针操作 |
| **依赖目录** | `Swapchain/` 当前 3 文件（Status + Info + Probe），余量 +2 |
| **白名单状态** | ✅ 在 `CodeFileBudgetTests` 白名单中（到期 8.7.7C） |

## 2. 调用点

| 位置 | 签名 |
|------|------|
| `VulkanViewportProbeRoute.cs:64` | `VulkanSwapchainProbe.ProbeWindows(hInstance, hWnd, width, height)` |

**仅 1 处调用**，与 SurfaceProbe / DeviceProbe 共享同一个 ProbeRoute 编排。

## 3. 职责拆解

### ProbeWindows 主体流程（~92 行，13-104）

```
Instance 创建（line 29）    → 临时 VkInstance（带 surface 扩展）
Surface 创建（line 33）     → Win32 SurfaceKHR
PhysicalDevice 选择（line 36） → Graphics+Present 双支持
Device 创建（line 38）      → 临时 LogicalDevice（带 VK_KHR_swapchain）
Surface 查询（lines 42-44） → caps / formats / present modes（Instance 层 proc）
Swapchain 创建（lines 73-89）→ 实际 SwapchainKHR
图像查询（lines 92-94）     → vkGetSwapchainImagesKHR
结果构建（lines 97-103）    → VulkanSwapchainInfo
finally 清理（lines 106-113）→ 按逆序释放 4 个资源
```

### 子方法职责表

| # | 职责 | 方法 | 行数 | 占比 |
|---|------|------|------|------|
| 1 | **函数指针加载** | `LoadInstanceProc` / `LoadDeviceProc` | 16 | 5% |
| 2 | **Instance 创建** | `CreateInstance` | 16 | 5% |
| 3 | **Surface 创建** | `CreateSurface` | 14 | 5% |
| 4 | **Surface 查询** | `QuerySurfaceCaps` / `QuerySurfaceFormats` / `QueryPresentModes` | 27 | 9% |
| 5 | **PhysicalDevice 选择** | `SelectPhysicalDevice` | 34 | 11% |
| 6 | **Device 创建** | `CreateDevice` | 14 | 5% |
| 7 | **格式/模式选择** | `ChooseFormat` / `ChoosePresentMode` / `ChooseExtent` | 18 | 6% |
| 8 | **Probe 编排 + Swapchain 创建** | `ProbeWindows` 主体 | 92 | 30% |
| 9 | **finally 清理** | `ProbeWindows` finally | 8 | 3% |
| 10 | **Delegate 类型** | 8 个 P/Invoke 委托 | 26 | 9% |
| 11 | **工具方法** | `Fail` / `PackApiVersion` | 2 | 1% |
| — | using / 注释 / 空行 | — | ~34 | 11% |

### 职责与已拆文件的对比

| 职责 | 对应已拆 Probe | 能否复用 |
|------|----------------|----------|
| Instance 创建 | `VulkanSurfaceInstanceScope` | **不可直接复用** — 扩展相同但 scope 生命周期不同（Swapchain 需同时管理 Instance+Surface+Device） |
| Surface 创建 | `VulkanSurfaceInstanceScope` | **不可直接复用** — Swapchain 的 surface 需要跨更多步骤存活 |
| Device 选择 | `VulkanDeviceSelector` | **不可直接复用** — Swapchain 需要 Graphics+Present 双队列，DeviceProbe 只查 Graphics |
| Device 创建 | `VulkanDeviceInstanceScope` | **不可直接复用** — 需要 `VK_KHR_swapchain` 扩展 |

## 4. 关键差异：SwapchainProbe ≠ SurfaceProbe + DeviceProbe

| 维度 | SurfaceProbe | DeviceProbe | SwapchainProbe |
|------|-------------|-------------|----------------|
| Instance 扩展 | surface + win32 | **无** | surface + win32 |
| Surface 创建 | ✅ | ❌ | ✅ |
| Device 选择 | ❌ | Graphics 单条件 | Graphics + **Present** 双条件 |
| Device 扩展 | ❌ | 无 | **VK_KHR_swapchain** |
| Swapchain 创建 | ❌ | ❌ | ✅ |
| 函数指针加载 | inline | inline | Instance + Device 两级加载 |

**SwapchainProbe 是 SurfaceProbe 的超集 + DeviceProbe 的超集。** 它不能直接复用已有的 scope 类，因为 scope 的生命周期和扩展需求不同。

## 5. 风险点

| 风险 | 级别 | 说明 |
|------|------|------|
| **函数指针加载** | **高** | 8 个函数指针通过 `vkGetInstanceProcAddr` / `vkGetDeviceProcAddr` 动态加载，是全文最脆弱的部分。拆分时必须保证指针作用域和生命周期正确 |
| **finally 清理顺序** | 中 | Swapchain→Device→Surface→Instance，顺序敏感。Scope 化后必须保证 Dispose 顺序 |
| **Device 选择逻辑** | 中 | 需要 Present 支持，比 DeviceProbe 多一个条件，提取时必须保留 |
| **Swapchain 非 Probe 类型** | 中 | 当前目录还有 `Scene3D/Session/Swapchain/` 下的正式 Swapchain 类型，拆分时避免命名冲突 |
| **与 Surface/Device 代码重复** | 低~中 | 虽然有重复（Instance 创建、Surface 创建），但因扩展/生命周期不同，**不建议强行复用** |
| **测试覆盖** | 低 | 只有 `VulkanSwapchainInfoTests`（结果模型测试），没有真正的 Probe 测试 |

## 6. 推荐拆分方案

### 方案 A：3 文件（推荐，最务实）

| # | 新文件 | 预计行数 | 职责 |
|---|--------|----------|------|
| 1 | `VulkanSwapchainProbe.cs` **（门面）** | ≤100 | `ProbeWindows()` 编排 + 结果构建 + Swapchain 创建 |
| 2 | `VulkanSwapchainContextScope.cs` **（新增）** | ≤100 | Instance+Surface+Device+函数的完整生命周期（IDisposable） |
| 3 | + `SelectPhysicalDevice` 留在门面或独立 | — | — |

但这个方案的问题：`SelectPhysicalDevice`（34行）+ `ChooseFormat`/`ChooseMode`/`ChooseExtent`（18行）+ `QuerySurface*`（27行）+ 函数指针（16行）= ~95 行，加上 Swapchain 创建（~20行），门面会超过 100。

### 方案 B：4 文件（更干净）

| # | 新文件 | 预计行数 | 职责 |
|---|--------|----------|------|
| 1 | `VulkanSwapchainProbe.cs` **（门面）** | ≤100 | `ProbeWindows()` 编排 + 结果构建 |
| 2 | `VulkanSwapchainContextScope.cs` **（新增）** | ≤100 | Instance+Surface+Device 生命周期 + 函数指针加载 + 清理 |
| 3 | `VulkanSwapchainSelector.cs` **（新增）** | ≤80 | PhysicalDevice 选择（Graphics+Present）+ 格式/模式/尺寸选择 |
| 4 | `VulkanSwapchainQuery.cs` **（新增）** | ≤60 | SurfaceCaps/Formats/PresentModes 查询 + 函数指针转换 |

### 方案 C：不拆（放弃，维持现状）

理由：SwapchainProbe 的函数指针加载模式与常规 Vulkan API 调用差异大，拆分引入的抽象可能比原文件更难维护。

## 7. 结论

| 维度 | 结论 |
|------|------|
| **是否可以拆** | ✅ **可以拆，但复杂度比 SurfaceProbe/DeviceProbe 高** |
| **推荐方案** | **方案 B（4 文件）** — 门面 + ContextScope + Selector + Query |
| **预计剩余行数** | 门面约 90 行 + Scope 约 90 行 + Selector 约 80 行 + Query 约 60 行 ≈ 320 行（比原 301 行略多，但职责清楚） |
| **关键约束** | **函数指针生命周期** — Query 类必须使用已经加载的函数指针，不能自行加载 |
| **清理顺序** | 必须严格按 Swapchain→Device→Surface→Instance 释放 |
| **Swapchain/ 目录** | 3→6 文件（含 Info/Status），**超过 5 文件上限** → 需要将 `VulkanSwapchainInfo` 或 `VulkanSwapchainStatus` 移出或合并 |
| **建议** | **下一轮再审计 `VulkanClearProbe`（416 行）对比风险**，或决定是否进入 D-2 拆分。SwapchainProbe 的函数指针模式使其成为 A 类中**风险最高的拆分目标**之一 |

### 风险评估总结

```
复杂度：    ████████░░  (8/10)  — 函数指针 + 多资源生命周期
重复度：    █████░░░░░  (5/10)  — 与 Surface/Device Probe 有重复但不可直接复用
测试覆盖：  ██░░░░░░░░  (2/10)  — 无 Probe 测试
调用方影响： ░░░░░░░░░░  (0/10)  — 仅 1 处调用，签名不变
目录容量：  ████████░░  (8/10)  — 3→6 文件超限，需先清理目录
```
