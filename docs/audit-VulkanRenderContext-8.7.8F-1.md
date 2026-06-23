# 8.7.8F-1 — VulkanRenderContext 拆分审计

审计日期：2026-06-23
目标文件：`FluidWarfare.Render.Vulkan/Context/VulkanRenderContext.cs`
目标行数：476 行

---

## 1. 当前文件状态

| 维度 | 值 |
|------|-----|
| **行数** | 476 行 |
| **类型** | `public sealed unsafe class VulkanRenderContext : IDisposable` — 类实例，非静态 |
| **Context/ 目录** | 仅 1 文件（VulkanRenderContext.cs），余量充足 |
| **白名单** | ✅ 在 `CodeFileBudgetTests` 中（行白名单） |

## 2. 调用点

**实际代码引用：0 处。**

```text
所有 grep 结果只有：
  1. 自身定义
  2. 白名单测试（CodeFileBudgetTests）
```

该文件**没有被任何 Editor / Render / Scene3D 代码实际使用**。
所有 CHANGELOG 中提到的 EditorShell 集成路径已在 8.0.1 中被移除（见 CHANGELOG:536）。

## 3. 核心发现：这是一个"已搁置的类"

### `TryCreateDeviceResources()` 永远返回 false

```csharp
private bool TryCreateDeviceResources()  // line 452
{
    _lastErrorMessage = "Swapchain 创建在当前环境不可用（函数指针兼容性问题），跳过清屏资源。";
    return false;   // ← 硬编码返回 false
}
```

意味着 `Initialize()` 调用后：

```
CreateInstance  →  ✅ 创建成功
SelectPhysicalDevice → ✅ 选择成功
CreateDevice    →  ✅ 创建成功
CreateSurface   →  ✅ 创建成功
TryCreateDeviceResources → ❌ 永远返回 false
```

结果：`_initialized` 保持 false，`RenderFrame()` 不执行任何操作。

### Initialize 的 try/catch

即使 `TryCreateDeviceResources` 返回 false，`catch` 捕获异常并调用 `Cleanup()`，所以最终状态是"干净但未初始化"。

## 4. 职责拆解

| # | 职责 | 方法 | 行数 | 类型 |
|---|------|------|------|------|
| 1 | **VkInstance 创建** | `CreateInstance` | 21 | 活跃代码 |
| 2 | **Instance 函数指针加载** | `LoadInstanceFunctions` | 13 | 活跃代码 |
| 3 | **PhysicalDevice 选择** | `SelectPhysicalDevice` | 23 | 活跃代码 |
| 4 | **LogicalDevice 创建** | `CreateDevice` | 15 | 活跃代码 |
| 5 | **Device 函数指针加载** | `LoadDeviceFunctions` | 13 | 活跃代码 |
| 6 | **Win32 Surface 创建** | `CreateSurface` | 8 | 活跃代码 |
| 7 | **Swapchain 创建** | `CreateSwapchain` + `ChooseFormat`/`ChoosePresentMode` | 58 | **死代码** |
| 8 | **Surface 格式重查** | `GetSurfaceFormat` | 8 | **死代码** |
| 9 | **ImageViews 创建** | `CreateImageViews` | 18 | **死代码** |
| 10 | **RenderPass 创建** | `CreateRenderPass` | 10 | **死代码** |
| 11 | **Framebuffers 创建** | `CreateFramebuffers` | 12 | **死代码** |
| 12 | **CommandPool/Buffer** | `CreateCommandPool` | 7 | **死代码** |
| 13 | **Sync 对象创建** | `CreateSyncObjects` | 10 | **死代码** |
| 14 | **资源创建编排** | `TryCreateDeviceResources` | 10 | **死代码** |
| 15 | **帧渲染** | `RenderFrame` | 60 | **死代码** |
| 16 | **初始化编排** | `Initialize` | 33 | 活跃代码 |
| 17 | **清理** | `Cleanup` | 24 | 活跃代码 |
| 18 | **工具方法** | `GetInstanceProc` / `GetDeviceProc` / `PackApiVersion` / `FormatApiVersion` | 8 | 活跃代码 |
| 19 | **Delegate 定义** | 9 个 P/Invoke 委托 | 10 | 活跃代码 |
| 20 | **字段声明** | 32 个字段 | 32 | 活跃代码 |

### 活跃/死代码比例

| 状态 | 行数 | 占比 |
|------|------|------|
| **活跃代码**（1-6, 16-20） | ~220 行 | **46%** |
| **死代码**（7-15, 被 TryCreateDeviceResources 阻断） | ~210 行 | **44%** |
| 空行/注释/using | ~46 行 | **10%** |

**近一半代码永远不会执行。**

## 5. 与前 3 个已拆文件的对比

| 维度 | ClearProbe (已拆) | SwapchainProbe (已拆) | DeviceProbe (已拆) | VulkanRenderContext |
|------|-------------------|---------------------|-------------------|-------------------|
| 行数 | 416 | 301 | 288 | 476 |
| 类型 | 静态 Probe | 静态 Probe | 静态 Probe | **实例类** |
| 生命周期 | 创建→立即销毁 | 创建→立即销毁 | 创建→立即销毁 | **持久存活** |
| 实际使用 | ✅ Editor 启动调用 | ✅ Editor 启动调用 | ✅ Editor 启动调用 | **❌ 无人调用** |
| 风险 | 函数指针清理顺序 | 函数指针清理顺序 | 设备选型 | **未使用，风险可控** |

## 6. 最大风险

| 风险 | 级别 | 说明 |
|------|------|------|
| **死代码比例 44%** | **高** | ~210 行代码因 `TryCreateDeviceResources` 硬编码 false 永远不执行。拆分时这些代码的去向需要决策 |
| **调用方为 0** | **低** | 无人调用的优点：拆错了不影响任何功能 |
| **函数指针兼容性** | 中 | Creator 注释中描述的函数指针崩溃问题，说明 swapchain 路径存在已知驱动兼容性 bug |
| **逻辑重复** | 中 | Instance/Surface/Device 创建逻辑与已拆的 Probe 文件大量重复，拆分时可考虑统一 |

## 7. 推荐方案

### 方案 A（推荐）：拆分活跃代码 + 死代码分离

| # | 新文件 | 预计行数 | 职责 | 状态 |
|---|--------|----------|------|------|
| 1 | `VulkanRenderContext.cs` | ≤100 | **门面**：Initialize 编排 + Cleanup + public API | 活跃 |
| 2 | `VulkanRenderInstanceScope.cs` | ≤70 | Instance 创建 + 函数指针加载 | 活跃，可复用 Probe 模式 |
| 3 | `VulkanRenderDeviceSelector.cs` | ≤50 | PhysicalDevice 选择 | 活跃 |
| 4 | `VulkanRenderContextSwapchain.cs` | ≤100 | Swapchain + ImageViews + RenderPass + Framebuffers + Command + Sync + RenderFrame | **死代码，先隔离** |
| 5 | `VulkanRenderContextSurface.cs` | ≤80 | Surface 创建 + Surface 查询 + 格式选择 | 活跃 |

但这里有个问题：文件 4 全部是死代码。如果它的函数指针有兼容性问题，拆出来也只是把不会执行的代码搬到新文件。

### 方案 B（更务实）：只拆分活跃代码

既然死代码永远不会执行，直接把活代码拆成 3 个文件，死代码留原文件锁仓：

| # | 新文件 | 预计行数 | 职责 |
|---|--------|----------|------|
| 1 | `VulkanRenderContext.cs` | ≤100 | 门面：Initialize + Cleanup + public API |
| 2 | `VulkanRenderContextSetup.cs` | ≤100 | Instance + Device + Surface 创建 + 函数指针加载 |
| 3 | `VulkanRenderContextSelector.cs` | ≤50 | PhysicalDevice 选择 |

死代码（Swapchain 链）留在 `VulkanRenderContext.cs` 底部，但只保留 `TryCreateDeviceResources`（永远 false 的占位），其余删除或注释。

## 8. 结论

| 维度 | 结论 |
|------|------|
| **是否可以拆** | ✅ **可以拆，且风险极低** |
| **原因** | 无人调用 + 44% 死代码 |
| **最大阻碍** | 死代码与活跃代码交织，需先分开 |
| **推荐方案** | **方案 B（3 文件）** — 门面 + 创建 Scope + 设备选择 |
| **死代码处理** | 锁仓在 `TryCreateDeviceResources` 中，不搬运到新文件 |
| **Context/ 目录** | 1→3 文件 ≤5 ✅，无需子目录 |
| **优先级** | **建议先清理掉这个"无人驾驶"的文件，再碰 EditorPreferences** |
