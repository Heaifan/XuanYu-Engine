# 8.7.8E-1 — VulkanClearProbe 拆分审计

审计日期：2026-06-23
目标文件：`FluidWarfare.Render.Vulkan/Clear/VulkanClearProbe.cs`
目标行数：416 行

---

## 1. 当前文件状态

| 维度 | 值 |
|------|-----|
| **行数** | 416 行 |
| **类型** | `public static unsafe class` — 纯静态，混合 Silk.NET 直接调用 + 函数指针 |
| **调用点** | 1 处 — `VulkanViewportProbeRoute.cs:72`，`ProbeWindows(hInstance, hWnd, w, h)` |
| **Clear/ 目录** | 3 文件（Status + Info + Probe），余量 +2，可用 Probe/ 子目录扩容 |
| **白名单** | ✅ 在 `CodeFileBudgetTests` 中（到期 8.7.7C） |

## 2. 职责拆解

### ProbeWindows 流程（17 个步骤 + finally）

```
 1  Instance 创建        (line 49)    临时 VkInstance（VK_KHR_surface + win32）
 2  Surface 创建         (line 55)    Win32 SurfaceKHR
 3  PhysicalDevice 选择  (line 59)    Graphics + Present 双队列
 4  Device 创建          (line 62)    临时 LogicalDevice（VK_KHR_swapchain）
 5  函数指针加载         (lines 52,66-82) Instance + Device 两层，7 个 ptr
 6  Surface 查询         (lines 85-92) caps / formats / modes + 格式选择
 7  Swapchain 创建       (lines 94-118) SwapchainKHR + 获取 Image 数组
 8  ImageViews 创建      (lines 120-133) 每个 Swapchain Image 一个 ImageView
 9  RenderPass 创建      (lines 135-148) 单附件 Clear→Store→PresentSrc
10  Framebuffers 创建    (lines 150-158) 每个 ImageView 一个 Framebuffer
11  CommandPool 创建     (lines 160-164)
12  CommandBuffer 分配   (lines 166-169)
13  同步对象创建         (lines 171-178) 2×Semaphore + 1×Fence
14  AcquireNextImage     (lines 180-189) 获取可用的 Swapchain Image 索引
15  Record CommandBuffer (lines 191-206) BeginRenderPass → Clear → End
16  QueueSubmit          (lines 208-225) 提交渲染命令
17  QueuePresent         (lines 227-240) 呈现结果
    DeviceWaitIdle       (line 243)
    finally 清理         (lines 252-274) 8+ 资源逆序释放
```

### 子方法职责表

| # | 职责 | 方法 | 行数 |
|---|------|------|------|
| 1 | **Instance 创建** | `CreateInstance` | 14 |
| 2 | **Surface 创建** | `CreateSurface` | 14 |
| 3 | **Device 选择** | `SelectDevice` | 28 |
| 4 | **Device 创建** | `CreateDevice` | 14 |
| 5 | **函数指针加载** | `LoadProc` / `LoadDeviceProc` | 5 |
| 6 | **Surface 查询** | `QueryCaps` / `QueryFormats` / `QueryModes` | 27 |
| 7 | **格式选择** | `ChooseFormat` / `ChoosePresentMode` / `ChooseExtent` | 6 |
| 8 | **ProbeWindows 编排** | 主体（steps 1-17 + 结果构建） | ~230 |
| 9 | **finally 清理** | 8+ 资源逆序释放 | 23 |
| 10 | **Fail / PackVer** | 工具方法 | 2 |
| 11 | **Delegate 定义** | 11 个 P/Invoke 委托 | 14 |

### 与 SwapchainProbe 的对比

| 维度 | SwapchainProbe (301→4) | ClearProbe (416) | 差异 |
|------|----------------------|-------------------|------|
| Instance/Surface/Device | ✅ | ✅ | 相同 |
| 函数指针 | 8 个 (全 KHR) | 7 个 KHR + Silk.NET 直调 | ClearProbe 使用 Silk.NET 直调更多 |
| Swapchain | ✅ | ✅ | 相同 |
| ImageViews | ❌ | ✅ | **新增** |
| RenderPass | ❌ | ✅ | **新增** |
| Framebuffers | ❌ | ✅ | **新增** |
| CommandPool/Buffer | ❌ | ✅ | **新增** |
| Sync (Semaphore/Fence) | ❌ | ✅ | **新增** |
| Acquire/Submit/Present | ❌ | ✅ | **新增** |
| Cleanup resources | 4 | 8+ | **翻倍** |

**ClearProbe = SwapchainProbe + 完整渲染管线。** 复杂度在 SwapchainProbe 基础上增加了约 60%。

## 3. 关键差异：Silk.NET 直调 vs 函数指针

这是 ClearProbe 与 SwapchainProbe **最重要的区别**：

| 调用方式 | SwapchainProbe | ClearProbe |
|----------|---------------|------------|
| Instance/Surface/Device 创建 | 指针 delegate | 指针 delegate |
| Surface 查询 | 指针 delegate | 指针 delegate |
| Swapchain 操作 | 指针 delegate | 指针 delegate |
| ImageView/RenderPass/Framebuffer | ❌ | **Silk.NET 直调** |
| CommandPool/Buffer | ❌ | **Silk.NET 直调** |
| Semaphore/Fence | ❌ | **Silk.NET 直调** |
| QueueSubmit/Present | ❌ | `QueueSubmit`=Silk.NET, `Present`=指针 |

这意味着 ClearProbe 的拆分比 SwapchainProbe **更容易** — 渲染管线部分使用标准 Silk.NET API，不需要传递函数指针，Scope 封装更自然。

## 4. 风险点

| 风险 | 级别 | 说明 |
|------|------|------|
| **finally 清理** | **高** | 8+ 资源（ImageView[]、Framebuffer[]、RenderPass、CommandPool、Semaphore×2、Fence、Swapchain、Device、Surface、Instance），释放顺序敏感 |
| **Probe 主体过大** | 高 | ~230 行编排逻辑混在一起，提取时需要小心保留每个步骤的依赖顺序 |
| **函数指针生命周期** | 中 | 7 个 KHR 函数指针（DestroySurface/CreateSwapchain/DestroySwapchain/GetImages/Acquire/Present + SurfaceSupport），跨 scope 传递 |
| **与 SwapchainProbe 重复** | 中 | Instance/Surface/Device 创建代码几乎相同，但**不可直接复用**（扩展/生命周期不同） |
| **Silk.NET 直调拆分** | 低 | 渲染管线部分（ImageView/RP/FB/Command/Sync）使用标准 API，Scope 封装无难度 |
| **Clear/ 目录容量** | 中 | 当前 3 文件，预计拆 5 个 → 超标。需先建 `Clear/Probe/` 子目录 |

## 5. 推荐拆分方案

### 先做目录整理（类似 D-2A）

```
整理前：                   整理后：
Clear/                     Clear/
├── VulkanClearInfo.cs     ├── VulkanClearInfo.cs
├── VulkanClearProbe.cs    ├── VulkanClearStatus.cs
└── VulkanClearStatus.cs   └── Probe/
                             ├── VulkanClearProbe.cs
                             ├── VulkanClearProbeContextScope.cs
                             ├── VulkanClearProbeDeviceSelector.cs
                             ├── VulkanClearProbeSurfaceQuery.cs
                             └── VulkanClearProbeRenderScope.cs
```

- `Clear/` 根：2 文件（Info + Status）≤5 ✅
- `Clear/Probe/`：5 文件（=上限）✅

### 预计 5 文件方案

| # | 文件 | 预计行数 | 职责 |
|---|------|----------|------|
| 1 | `VulkanClearProbe.cs` | ≤100 | **门面**：ProbeWindows 主编排 + Acquire/Submit/Present + 结果构建 |
| 2 | `VulkanClearProbeContextScope.cs` | ≤100 | **生命周期**：Instance+Surface+Device+函数指针 (IDisposable) |
| 3 | `VulkanClearProbeDeviceSelector.cs` | ≤50 | **设备选择**：PhysicalDevice 选择（Graphics+Present） |
| 4 | `VulkanClearProbeSurfaceQuery.cs` | ≤70 | **Surface 查询**：caps/formats/modes + ChooseFormat/Mode/Extent |
| 5 | `VulkanClearProbeRenderScope.cs` | ≤100 | **渲染管线**：Swapchain+ImageViews+RenderPass+Framebuffers+CommandPool/Buffer+Sync (IDisposable) |

### 门面逻辑示意

```
ProbeWindows(hinstance, hwnd, fw, fh):
  sw.Start()
  using var ctx = new VulkanClearProbeContextScope()
  ctx.CreateInstance() / CreateSurface() / CreateDevice()
  
  var selector = new VulkanClearProbeDeviceSelector()
  selector.TrySelect(...)
  
  var query = new VulkanClearProbeSurfaceQuery()
  var caps/formats/modes = query.Query(...)
  var extent = query.ChooseExtent(...)
  
  using var render = new VulkanClearProbeRenderScope()
  render.CreateSwapchain(ctx.Device, extent, chosenFmt, chosenMode, caps)
  render.CreateImageViews(ctx.Vk, ctx.Device)
  render.CreateRenderPass(ctx.Vk, ctx.Device, chosenFmt)
  render.CreateFramebuffers(ctx.Vk, ctx.Device, extent)
  render.CreateCommandPool(ctx.Vk, ctx.Device, qi)
  render.AllocateCommandBuffer(ctx.Vk, ctx.Device)
  render.CreateSyncObjects(ctx.Vk, ctx.Device)
  
  render.Acquire(ctx.Device, fnAcquire, swapchain)
  render.Record(extent, clearColor)   // Begin/End RenderPass
  render.Submit(ctx.Device, qi)       // QueueSubmit
  render.Present(ctx.Device, fnQueuePresent, swapchain, imgIndex)
  vk.DeviceWaitIdle(ctx.Device)
  
  return VulkanClearInfo(...)
  // render.Dispose() → Swapchain→ImageViews→RenderPass→Framebuffers→CommandPool→Sync
  // ctx.Dispose() → Device→Surface→Instance
```

### 为什么 RenderScope 能 ≤100 行？

与 SwapchainProbe 不同，ClearProbe 的渲染管线使用 **Silk.NET 直调**，不需要加载/传递函数指针。所有 `vkCreateImageView`、`vkCreateRenderPass` 等调用直接使用 `vk.CreateImageView(...)`，代码量更少、结构更清晰。RenderScope 封装的是标准的创建+释放生命周期，没有指针传递的样板代码。

## 6. 结论

| 维度 | 结论 |
|------|------|
| **是否可以拆** | ✅ **可以拆** |
| **风险** | **中高** — finally 清理 8+ 资源，顺序敏感 |
| **优势** | 渲染管线使用 Silk.NET 直调，比 SwapchainProbe 的函数指针模式更易拆分 |
| **推荐方案** | 5 文件（门面 + ContextScope + DeviceSelector + SurfaceQuery + RenderScope） |
| **先决条件** | 先建 `Clear/Probe/` 子目录解决目录容量（类似 8.7.8D-2A） |
| **下一轮建议** | E-2A：目录容量整理 → E-2B：执行拆分 |
