# rz-vk4-c-swapchain-plan.md — VK4-C Swapchain 生命周期规划

> 规划态文档：本轮只规划 **Swapchain + Swapchain Images + ImageViews** 的生命周期边界与文件结构，**不写任何 Vulkan 实装代码**。
> VK4-C 实装通过后，仍然可以黑屏（不 RenderPass / 不 Clear / 不 Present）；真正出颜色等 VK4-D。

## 1. 背景与已收口状态

- **VK4-A 完成**：Instance + Surface 之后新增 PhysicalDevice 选择链路（`Device/VulkanPhysicalDeviceSelector` 等），输出 `VulkanPhysicalDeviceSelection`（含 `PhysicalDevice Handle`）。
- **VK4-B 完成**：基于 `VulkanPhysicalDeviceSelection` 创建 `LogicalDevice`（VkDevice）+ Graphics/Present 队列（`Device/VulkanDeviceOwner` + `Bridge/VulkanBridgeDeviceAttachStep`）。
- **VK4-B-R1 完成**：Detach 释放顺序已运行时验证为 `LogicalDevice → Surface → Instance`，VK4-B 正式完全收口。
- **日志链路**：`LOG-UX-1`（日志多选/复制/详情换行）保留；`LOG-UX-2`（临时落盘）已按用户要求回退删除。

当前已建链（Attach 方向）：

```
NativeHost HWND
   → Vulkan Instance
   → Vulkan Surface
   → PhysicalDevice Selection
   → LogicalDevice
   → Graphics Queue / Present Queue
```

Detach 方向（已验证）：`LogicalDevice → Surface → Instance`。

VK4-C 在 `LogicalDevice` 之后插入 `Swapchain → ImageViews`，把链延长，但仍停在「拿到可呈现的图像视图」，不进入绘制。

## 2. VK4-C 核心目标（只做这些）

| 做 ✅ | 不做 ❌ |
|---|---|
| 创建 `VkSwapchainKHR`（Swapchain） | 创建 `RenderPass` |
| 取 Swapchain Images | 创建 `Framebuffer` |
| 为每张 Image 创建 `ImageView` | 创建 `CommandPool` / `CommandBuffer` |
| 查询 Surface capabilities / format / present mode / extent | `Clear`（清屏） |
| Resize 时重建 Swapchain + ImageViews | `Present`（呈现） |
| 中文生命周期日志 | 持续渲染循环 |

> **VK4-C 完成后仍应黑屏**：无 RenderPass / Framebuffer / CommandBuffer / Clear / Present，看不到颜色是预期。验收靠「Swapchain 创建成功 + ImageViews 创建成功 + Resize 重建成功」的日志证据。

## 3. 资源持有者与文件结构（建议，均 ≤100 行）

沿用 VK4-A/B 的「独立 owner + 独立 attach step」同构，绝不把 Swapchain 逻辑塞进 `VulkanNativeHostSurfaceBridge`（Bridge 已接近 100 行红线，不再膨胀，禁止再增 Swapchain 逻辑）。

建议在 `XuanYu.Render.Vulkan` 下新增子目录 `Swapchain/`：

- `Swapchain/VulkanSwapchainCapabilities.cs`（≤100）
  - 职责：查询 `VkSurfaceCapabilitiesKHR`；按偏好选择 `SurfaceFormatKHR`（优先 B8G8R8A8 + SRGB，或设备支持的最佳）、`PresentModeKHR`（优先 `FIFO`，可选 `MAILBOX`）、`Extent2D`（与窗口尺寸对齐，处理 0 尺寸）。
  - 输出纯数据结果（`VulkanSwapchainCapabilities`），**不创建** Swapchain。
  - 复用 `VulkanPhysicalDeviceSelection.Handle` 与 `Surface`，不重枚举设备。

- `Swapchain/VulkanSwapchainOwner.cs`（≤100）
  - 职责：基于 `VulkanDeviceOwner.LogicalDevice` + `Surface` + `VulkanSwapchainCapabilities` 创建 `VkSwapchainKHR`；取交换链图像；为每张 Image 创建 `ImageView`。
  - 仅持有 `Swapchain` + `Image[]` + `ImageView[]`；`Dispose` 幂等，释放顺序 **ImageViews → Swapchain**（见第 5 节）。
  - 中文生命周期日志：`开始创建 Swapchain` / `Swapchain 创建成功` / `ImageView 创建成功 N 张` / `Swapchain 释放成功` 等。

- `Swapchain/VulkanSwapchainLogFormatter.cs`（≤100）
  - 纯中文日志格式器（与 `VulkanBridgeLogFormatter` / `VulkanInstanceLogFormatter` 同构）。

- `Bridge/VulkanBridgeSwapchainAttachStep.cs`（≤100）
  - 职责：在「选择 step → 设备 step」之后链式驱动 `VulkanSwapchainOwner.Create`；设备创建失败（`_deviceOwner` 为 null）则跳过、仅记日志，不影响已附加的 Instance+Surface+Device。
  - 返回 `VulkanSwapchainOwner?`，供 Bridge 在 Detach 时逆序释放。

> **目录文件数约束**：`Swapchain/` 子目录目标 3 文件（capabilities / owner / formatter），`Bridge/` 现 3 文件（PhysicalDevice / Device / Swapchain attach step），均不越过 5–7 文件上限。

## 4. Attach / Detach 生命周期扩展

### Attach 顺序（在 VK4-B 之后追加）

```
Instance（已有）
   → Surface（已有）
   → PhysicalDevice Selection（已有）
   → LogicalDevice（已有）
   → Swapchain（VK4-C 新增）
   → ImageViews（VK4-C 新增，随 Swapchain 创建）
```

`VulkanNativeHostSurfaceBridge.Attach` 现链式：选择 step → 设备 step → **Swapchain attach step**；三步任一前置失败均只跳过后续、不影响已建部分。

### Detach 顺序（VK4-C 扩展后）

```
ImageViews          ← 最先释放
   → Swapchain
   → LogicalDevice
   → Surface
   → Instance        ← 最后释放
```

即：`_swapchainOwner?.Dispose()`（内部先 ImageView 后 Swapchain）→ `_deviceOwner?.Dispose()` → `_surfaceOwner?.Dispose()` → `_instanceOwner?.Dispose()`。

> Bridge 已接近 100 行红线，Detach 仅追加一行 `_swapchainOwner?.Dispose()` 即可（仍 100 或微调），**不得内联 Swapchain 创建/重建逻辑**。若 Bridge 越过 100 行，须把 Bridge 现有某段（如日志/合并）再迁出，而非内联 Swapchain。

## 5. Dispose 释放顺序硬约束

VK4-C 引入新资源后，完整释放链必须为：

```
ImageViews → Swapchain → LogicalDevice → Surface → Instance
```

红线：
- 禁止先释放 Swapchain 再释放 ImageViews（ImageView 依赖 Swapchain Image，顺序反了会悬空）。
- 禁止先释放 LogicalDevice 再释放 Swapchain（Swapchain/ImageView 依赖 Device）。
- 禁止先释放 Surface 再释放 Swapchain（Swapchain 创建依赖 Surface）。
- 禁止 Dispose 中抛异常导致后续资源泄漏；所有释放包 `try/catch` 或确保幂等，异常仅记日志。

## 6. Resize 生命周期（VK4-C 的核心风险点）

VK4-B 的 Resize 已实现「接收尺寸变化 / 不重建 Surface / 不重建 Device / 不重建 Queue」。VK4-C 把 Resize 的响应权交给 Swapchain：

### Resize 只允许

```
收到新尺寸
   → 跳过 0 尺寸 / 与当前相同的重复尺寸
   → 释放旧 ImageViews
   → 释放旧 Swapchain
   → 用新 Extent 重建 Swapchain
   → 取新 Images
   → 创建新 ImageViews
   → 记中文日志（尺寸、新旧 extent、新 ImageView 数量）
```

### Resize 禁止

```
重建 Surface
重建 Instance
重建 LogicalDevice
重取 Graphics / Present Queue
```

> 即 Resize **只重建 Swapchain + ImageViews**，其余全不动。这与 VK4-A/B 的 Surface/Device/Queue 不重建红线一脉相承。

### Resize 与 Bridge 的接线

`VulkanNativeHostSurfaceBridge` 已有 `Resize` 入口（当前仅记「不重建 Surface」日志）。VK4-C 在该入口新增：调用 `_swapchainOwner?.Recreate(newExtent)`，由 owner 内部完成第 6 节的释放+重建。**Bridge 的 Resize 方法只做转发与日志，不内联 Swapchain 重建细节。**

## 7. 命名与行数红线（继承 VK4-B-R1）

- `Silk.NET.Vulkan.Device` 类型一律用 `VulkanDevice` 类型别名。
- 业务 owner = `VulkanDeviceOwner`（Device 侧，不增职责）；Swapchain 侧 = `VulkanSwapchainOwner`。
- 业务属性命名：`LogicalDevice`（Device 侧）；Swapchain 侧建议 `Swapchain`（VkSwapchainKHR 别名类型）与 `ImageViews`（只读集合），**禁止用 `Device` 作属性名**。
- 所有新增 `.cs` 文件 ≤100 行；单职责；`Swapchain/` 子目录文件数 ≤5。
- `VulkanDeviceOwner` 只负责 `CreateDevice / GetQueue / DisposeDevice`，**VK4-C 禁止顺手塞 Swapchain/CommandPool/RenderPass**。

## 8. UI / 依赖红线（继承全局约束）

- `Editor.UI` 不得新增 `Silk.NET.Vulkan` 类型使用点；不得持有 Vk/Swapchain/ImageView。
- UI 经 `Render.Abstractions` 契约与 Vulkan 交互；Swapchain 细节不外露给 UI。
- 禁止把旧探针 `VulkanClearSession`（含 `VulkanClearSession.Swapchain.cs`）复制进正式路径——只参考设计意图，禁止复制实现。
- VkResult 必须保存并分类处理；UI 线程禁无限等待。

## 9. 验收与后续阶段

### VK4-C 验收（实装后）
- 启动编辑器：日志出现 `开始创建 Swapchain` → `Swapchain 创建成功` → `ImageView 创建成功 N 张`。
- 缩放窗口：出现 `Swapchain 重建成功；新 extent=...；新 ImageView=N` 类日志；**无** Surface/Device/Queue 重建日志。
- 关闭编辑器：Detach 顺序出现 `ImageViews 释放 → Swapchain 释放 → LogicalDevice 释放 → Surface 释放 → Instance 释放`。
- 仍黑屏（无 RenderPass/Clear/Present），属预期。
- `XuanYu.Render.Vulkan` 与 `XuanYu.Editor.UI` 均 0W0E；所有新增 `.cs` ≤100 行。

### 后续阶段（不在本轮）
- **VK4-C-R1**：Resize 重建 Swapchain 生命周期审计（重点：Resize 只重建 Swapchain+ImageViews、不回潮 Surface/Device/Queue、释放顺序无悬空）。
- **VK4-D**：`VulkanClearFrame` — RenderPass + Framebuffer + CommandBuffer + 单帧 Clear/Present（出单色画面）。
- **VK4-E**：`VulkanRenderSession` + `IRenderSession` 契约 + 组合根接线。

## 10. 防回潮门禁（VK4-C 实装时）
- Resize 不重建 Surface；只允许 Swapchain 重建策略（跳过 0 尺寸 / 重复尺寸）。
- 不把探针 `VulkanClearSession` 搬进正式路径。
- `Editor.UI` 不新增 `Silk.NET.Vulkan` 使用点；不持有 Vk/Swapchain/ImageView。
- 所有新增 `.cs` ≤100 行；单职责；`Swapchain/` 目录核心文件 ≤5。
- 持续渲染循环禁止在 VK4 引入。

## 11. 规划态验收
- 本阶段只产出此文档 + changelog / file-tree 同步，**无代码改动**。
- 规划通过后，再开 `VK4-C`（Swapchain + ImageViews 实装）与 `VK4-C-R1`（Resize 审计）。

## 12. 规划图（SVG 源，代码框）

> 生命周期与边界示意；以 raw SVG 贴在代码框内便于评审与复刻（不在文档内渲染）。

```svg
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 680 440" font-family="Segoe UI, sans-serif" font-size="13">
  <rect x="0" y="0" width="680" height="440" fill="#0f1420"/>
  <text x="20" y="28" fill="#7fd1ff" font-size="15" font-weight="bold">VK4-C Attach 链（已建 → 新增）</text>
  <g fill="#1b3a5c" stroke="#3f7fb5" stroke-width="1.5">
    <rect x="40" y="48" width="120" height="34" rx="6"/>
    <rect x="40" y="96" width="120" height="34" rx="6"/>
    <rect x="40" y="144" width="120" height="34" rx="6"/>
    <rect x="40" y="192" width="120" height="34" rx="6"/>
    <rect x="40" y="240" width="120" height="34" rx="6" fill="#2a5c3a" stroke="#5fbf7f"/>
    <rect x="40" y="288" width="120" height="34" rx="6" fill="#2a5c3a" stroke="#5fbf7f"/>
  </g>
  <g fill="#dbe7f2" text-anchor="middle">
    <text x="100" y="70">Instance</text>
    <text x="100" y="118">Surface</text>
    <text x="100" y="166">PhysicalDevice</text>
    <text x="100" y="214">LogicalDevice</text>
    <text x="100" y="262" fill="#bdf5cf">Swapchain</text>
    <text x="100" y="310" fill="#bdf5cf">ImageViews</text>
  </g>
  <g stroke="#5f9fd0" stroke-width="2" marker-end="url(#arrow)">
    <line x1="100" y1="82" x2="100" y2="94"/>
    <line x1="100" y1="130" x2="100" y2="142"/>
    <line x1="100" y1="178" x2="100" y2="190"/>
    <line x1="100" y1="226" x2="100" y2="238"/>
    <line x1="100" y1="274" x2="100" y2="286"/>
  </g>
  <text x="360" y="28" fill="#ff9f7f" font-size="15" font-weight="bold">Detach 释放顺序（逆序）</text>
  <g fill="#3a2030" stroke="#b5654f" stroke-width="1.5">
    <rect x="380" y="48" width="160" height="30" rx="6"/>
    <rect x="380" y="90" width="160" height="30" rx="6"/>
    <rect x="380" y="132" width="160" height="30" rx="6"/>
    <rect x="380" y="174" width="160" height="30" rx="6"/>
    <rect x="380" y="216" width="160" height="30" rx="6"/>
  </g>
  <g fill="#f3d6cc" text-anchor="middle">
    <text x="460" y="68">① ImageViews</text>
    <text x="460" y="110">② Swapchain</text>
    <text x="460" y="152">③ LogicalDevice</text>
    <text x="460" y="194">④ Surface</text>
    <text x="460" y="236">⑤ Instance</text>
  </g>
  <g stroke="#c97f6a" stroke-width="2" marker-end="url(#arrowR)">
    <line x1="460" y1="78" x2="460" y2="88"/>
    <line x1="460" y1="120" x2="460" y2="130"/>
    <line x1="460" y1="162" x2="460" y2="172"/>
    <line x1="460" y1="204" x2="460" y2="214"/>
  </g>
  <text x="20" y="356" fill="#ffd27f" font-size="14" font-weight="bold">VK4-C 边界</text>
  <text x="20" y="380" fill="#bdf5cf" font-size="12">做：Swapchain + Images + ImageViews</text>
  <text x="20" y="400" fill="#f3a0a0" font-size="12">不做：RenderPass / Framebuffer / CommandPool / CommandBuffer / Clear / Present</text>
  <text x="20" y="420" fill="#9fd0ff" font-size="12">Resize：只重建 Swapchain + ImageViews（不重建 Surface/Instance/Device/Queue）</text>
  <defs>
    <marker id="arrow" markerWidth="10" markerHeight="10" refX="6" refY="3" orient="auto"><path d="M0,0 L6,3 L0,6 Z" fill="#5f9fd0"/></marker>
    <marker id="arrowR" markerWidth="10" markerHeight="10" refX="6" refY="3" orient="auto"><path d="M0,0 L6,3 L0,6 Z" fill="#c97f6a"/></marker>
  </defs>
</svg>
```
