# rz-vk4-d-plan.md — VK4-D 最小清屏闭环规划（RenderPass + Framebuffer + CommandBuffer + 单帧 Clear + Present）

> 规划态文档：本轮只规划 **VK4-D 最小清屏闭环** 的资源顺序、帧同步、Resize 边界与文件结构，**不写任何 Vulkan 实装代码**。
> 实装后编辑器第一次从「黑屏」变为「单色清屏画面」；仍不引入任何场景绘制。

## 0. 阶段收口确认（规划前提）

经用户真机验证，以下项目**正式收口**（2026-07-09）：

| 项目 | 结论 |
|---|---|
| VK4-A PhysicalDevice 选择 | ✅ 完成 |
| VK4-B LogicalDevice + Queue | ✅ 完成 |
| VK4-C Swapchain + Images + ImageViews | ✅ 完成（含 VK4-C-R1 Resize 重建审计） |
| LOG-UX-1 多选复制 / 详情换行 | ✅ 保留 |
| LOG-UX-2 自动滚动重设计（独立 `LogListAutoScrollController`） | ✅ 真机通过 |
| 控制台 Vulkan 日志单出口去重 | ✅ 通过 |
| 旧 21:32 种子/假日志清理 | ✅ 通过 |

已成立链路（Attach 方向）：

```
NativeHost HWND
   → Vulkan Instance
   → Vulkan Surface
   → PhysicalDevice Selection
   → LogicalDevice + Queue
   → Swapchain + Images + ImageViews
   → （VK4-D 新增）RenderPass / Framebuffer / CommandPool / CommandBuffer / Sync → 单帧 Clear + Present
```

Detach 释放顺序（已运行时验证）：`ImageViews → Swapchain → LogicalDevice → Surface → Instance`。VK4-D 须在 Swapchain 之前插入 ClearFrame 的释放（见第 5 节）。

## 1. VK4-D 核心目标（只做这些）

| 做 ✅ | 不做 ❌ |
|---|---|
| 创建 `RenderPass`（仅 1 个颜色附件，loadOp=CLEAR，storeOp=STORE） | 场景渲染 / 多 Pass |
| 为每张 Swapchain ImageView 创建 `Framebuffer` | 相机 / 视图矩阵 |
| 创建 `CommandPool` + 1 个 `CommandBuffer`（primary） | 网格 / 顶点缓冲 |
| 创建同步原语：`Semaphore×2` + `Fence×1` | 材质 / 着色器 / Pipeline 状态对象 |
| `AcquireNextImage` → 录制 Clear → `QueueSubmit` → `QueuePresent` | Gizmo / 拾取 / 选中高亮 |
| 最小 present 泵（驱动每帧 Clear+Present） | UI 叠加 / HUD / ImGui |
| 中文生命周期日志 | 持续动画 / 场景提交循环（VK5+） |

> **VK4-D 验收标志：窗口从黑屏变为单色清屏**（如深蓝 `#102030`）。看不到网格/物体是预期。

## 2. 资源创建顺序（Attach，在 Swapchain 之后）

```
Swapchain + ImageViews（已有）
   → RenderPass              （尺寸无关，仅依赖 swapchain.Format）
   → CommandPool            （graphics 队列族）
   → CommandBuffer          （primary，从 CommandPool 分配）
   → Sync：imageAvailable 信号量 + renderFinished 信号量 + inFlight 栅栏
   → Framebuffers[]         （每张 ImageView 一个，依赖 extent + ImageView）
   → Present 泵启动         （独立线程/Timer，非 UI 线程）
```

硬约束：

- **RenderPass 只建一次**，不随 Resize 重建（格式由 Swapchain.Format 决定，Resize 不换格式）。
- **CommandPool 只建一次**，Resize 不重建；CommandBuffer 每次帧前 `Reset` 复用。
- **Framebuffers 尺寸相关**，Resize 必须重建（见第 4 节）。
- **Sync 原语只建一次**，Resize 不重建。

## 3. 帧同步设计（每帧 RenderFrame）

必须走标准双信号量 + 栅栏，禁止无栅栏提交（GPU/CPU 竞态、丢帧）：

```
1. AcquireNextImageKHR(timeout=UINT64_MAX, imageAvailableSem, null) → imageIndex
   （VkResult 须保存；VK_ERROR_OUT_OF_DATE_KHR / SUBOPTIMAL_KHR 见第 4 节边界）
2. WaitForFences(inFlight, true, UINT64_MAX); ResetFences(inFlight)
3. ResetCommandBuffer; BeginCommandBuffer
4. BeginRenderPass：
      framebuffer = Framebuffers[imageIndex]
      renderArea  = (0,0,extent)
      clearValue  = 选定单色（如 {0.063,0.125,0.188,1.0}）
      loadOp=CLEAR, storeOp=STORE
5. （无 draw call）
6. EndRenderPass; EndCommandBuffer
7. QueueSubmit(graphicsQueue)：
      waitSemaphores=[imageAvailable], waitStages=[COLOR_ATTACHMENT_OUTPUT]
      commandBuffers=[cmd]
      signalSemaphores=[renderFinished]
      fence=inFlight
8. QueuePresentKHR(presentQueue)：
      waitSemaphores=[renderFinished], swapchains=[swapchain], imageIndices=[imageIndex]
```

- **栅栏初值 = SIGNALED**：首帧 `WaitForFences` 直接通过，避免首帧死等。
- **单 in-flight 帧**：每帧复用同一栅栏，天然串行化，足够最小清屏（不追求并行多帧）。
- **Present 泵必须运行在非 UI 线程**：用 `System.Threading.Timer` 或专用渲染线程回调触发 `RenderFrame()`，**禁止在 UI 线程循环 Acquire/Submit/Present**（违反 UI 线程禁无限等待红线，且会卡 UI）。
- **VkResult 必须保存并分类处理**：Acquire/Submit/Present 任一非 `Success` 须记中文日志，异常仅记日志不抛。

## 4. Resize 边界（VK4-D 的核心风险点）

VK4-C 已确认 Resize 只重建 Swapchain+ImageViews（不回潮 Surface/Device/Queue）。VK4-D 在 Resize 链路追加：

```
收到新尺寸
   → 跳过 0 尺寸（Swapchain 跳过；ClearFrame 同步跳过，停泵/跳过帧）
   → _swapchainOwner.Recreate(w,h)        （VK4-C 已有；内部释放旧 ImageView→Swapchain，建新）
   → _clearFrame.Resize(w, h, swapchain.ImageViews)
        · 释放旧 Framebuffers[]
        · 按新 extent + 新 ImageViews 重建 Framebuffers[]
        · RenderPass / CommandPool / CommandBuffer / Sync 原样不动
   → 中文日志（新 extent、新 Framebuffer 数量）
```

边界硬规则：

- **Resize 不重建** RenderPass / CommandPool / CommandBuffer / Sync。
- **Resize 不重建** Surface / Instance / LogicalDevice / Queue（沿用 VK4-A/B/C 红线）。
- **Acquire 返回 `OUT_OF_DATE_KHR` / `SUBOPTIMAL_KHR`**：不当帧 Present，仅记日志；交由下一次 UI Resize 事件触发 Swapchain 重建（最小实现不在此处自触发重建，避免渲染线程回调 UI 资源）。
- **Swapchain 重建失败返回 null**：ClearFrame 进入「无 Framebuffer」挂起态，`RenderFrame()` 守卫 `Framebuffers.Length == ImageViews.Length` 不成立则跳过本帧，等待下次成功 Resize。
- **0 尺寸**：Swapchain 跳过重建（`VulkanSwapchainOwner.Recreate` 已内置）；ClearFrame 同步跳过重建 Framebuffer，Present 泵空转跳过。

## 5. Dispose / Detach 释放顺序硬约束

VK4-D 引入新资源后，完整释放链必须为：

```
ClearFrame（先停泵）
   → Framebuffers[]   ← ClearFrame.Dispose 内部最先释放
   → CommandPool      ← 释放即释放其下 CommandBuffer
   → RenderPass
   → 2× Semaphore + Fence
   → Swapchain（ImageViews → Swapchain）   ← 现有释放链
   → LogicalDevice
   → Surface
   → Instance
```

即 Bridge.Detach 顺序调整为：

```
_clearFrame?.Dispose();   // 内部停泵 + Framebuffers→CommandPool→RenderPass→Sync
_swapchainOwner?.Dispose();
_deviceOwner?.Dispose();
_surfaceOwner?.Dispose();
_instanceOwner?.Dispose();
```

ClearFrame.Dispose 内部顺序（逆创建序）：停 Present 泵 → 销毁 Framebuffers → 销毁 CommandPool（连带 CommandBuffer） → 销毁 RenderPass → 销毁 2 信号量 + 栅栏。

红线：

- 禁止先释放 RenderPass / CommandPool 再释放 Framebuffers（Framebuffer 依赖二者）。
- 禁止先释放 Swapchain 再释放 ClearFrame（Framebuffer 依赖 Swapchain ImageView）。
- 禁止 Dispose 抛异常；所有释放包 `try/catch` 或幂等，异常仅记日志。

## 6. 文件结构与接线（均 ≤100 行，目录核心文件 ≤5–7）

沿用 VK4-A/B/C「独立 owner + 独立 attach 接线」同构。VK4-D 引入 `Render/` 子目录（与 `Device/`、`Swapchain/`、`Bridge/` 并列）：

- `Render/VulkanClearFrame.cs`（≤100，若超则拆）
  - 职责：建 RenderPass + CommandPool + CommandBuffer + Sync + Framebuffers[]；暴露 `RenderFrame()`、`Resize(w,h,imageViews)`、`Dispose()`；内部持有 Vk 引用来自 `VulkanDeviceOwner` 与 `VulkanSwapchainOwner`（只读取 `LogicalDevice`/`GraphicsQueue`/`PresentQueue`/`Format`/`Extent`/`ImageViews`）。
  - 中文日志：`RenderPass 创建成功` / `Framebuffer 创建成功 N 张` / `帧同步原语创建成功` / `首帧 Clear+Present 成功` / `ClearFrame 释放成功`。
- `Render/VulkanPresentLoop.cs`（≤100，建议拆分）
  - 职责：最小 present 泵（`System.Threading.Timer` 或渲染线程），`Start()`/`Stop()`；仅调 `ClearFrame.RenderFrame()`，不持 Vulkan 类型细节。
  - 红线：只在非 UI 线程运行；`Stop()` 幂等并等待在途帧结束。
- `Render/VulkanClearFrameLogFormatter.cs`（≤100）
  - 纯中文日志格式器（与 `VulkanBridgeLogFormatter` / `VulkanSwapchainLogFormatter` 同构）。

**Bridge 红线处理（⚠️ 关键）**：`VulkanNativeHostSurfaceBridge.cs` 当前 98 行，已近 100 红线。VK4-D 若直接内联 ClearFrame 创建/Resize/Detach 转发，必越线。

> **决策：VK4-D 实装时顺带引入薄组合根 `VulkanRenderSession`（原 rz-vk4-plan 的 VK4-E 范围），Bridge 委托给它。**
> - `Render/VulkanRenderSession.cs`（≤100）：组合 `VulkanDeviceOwner` + `VulkanSwapchainOwner` + `VulkanClearFrame` + `VulkanPresentLoop`，暴露 `Attach/Resize/Detach`；Bridge 仅持有 `_session` 并转发三方法（约 3–4 行），守住 Bridge ≤100 红线与「契约优先 / UI 不直接认识 Vulkan 类型」设计。
> - 这把 VK4-E 的「组合根」提前落地，但**不实现任何场景渲染**，不违反 VK4-D 红线；VK4-E 后续仅补 `IRenderSession` 抽象契约与 `VulkanRenderSessionProvider`（Editor.UI 侧装配），不再改动渲染资源本身。
> - 若坚持严格分阶段，则 Bridge 仅新增 `_clearFrame` 字段 + 创建/Resize/Detach 3 行转发，允许 Bridge 临时微调至 ≤100（不内联任何 ClearFrame 细节）。

## 7. 命名与行数红线（继承 VK4-C）

- `Silk.NET.Vulkan.Device` 类型仍用 `VulkanDevice` 别名；队列属性 `GraphicsQueue`/`PresentQueue`（沿用 DeviceOwner）。
- 业务 owner：`VulkanClearFrame`（渲染帧资源）；present 泵 `VulkanPresentLoop`。
- 所有新增 `.cs ≤100` 行；单职责；`Render/` 子目录核心文件 ≤5。
- `VulkanDeviceOwner` 仍只管 `LogicalDevice/Queue`，**VK4-D 禁止往里塞 RenderPass/CommandPool/ClearFrame**。
- `VulkanSwapchainOwner` 已暴露 `Format/Extent/ImageViews`（只读），VK4-D 直接消费，不反向改它。

## 8. UI / 依赖红线（继承全局约束）

- `Editor.UI` **不得**新增 `Silk.NET.Vulkan` 使用点；**不得**持有 Vk/Device/Swapchain/ImageView/RenderPass/CommandBuffer。
- UI 仅经 `Render.Abstractions` 契约与渲染交互；ClearFrame / PresentLoop 细节不外露给 UI。
- Present 泵运行在 Render.Vulkan 内部线程，**不触碰 Avalonia / UI 线程**。
- VkResult 必须保存并分类处理；UI 线程禁无限等待。
- 持续渲染循环（场景提交 / 动画）**禁止在 VK4 引入**，VK5+ 再做。

## 9. 防回潮门禁（VK4-D 实装时）

- Resize 不重建 Surface / Instance / LogicalDevice / Queue / RenderPass / CommandPool / Sync。
- 不在 `Foot.axaml.cs` 或任何 UI 代码后置里写渲染/滚动/Vulkan 逻辑；自动滚动已独立为 `LogListAutoScrollController`，勿回潮。
- 不把旧探针 `VulkanClearSession` 复制进正式路径（只参考设计意图）。
- `Editor.UI` 不新增 `Silk.NET.Vulkan` 使用点。
- 所有新增 `.cs ≤100` 行；单职责；`Render/` 目录核心文件 ≤5。
- 帧同步必须用信号量 + 栅栏；禁止无栅栏提交。
- 单色清屏验证通过前，禁止追加任何 draw / pipeline / 网格逻辑。

## 10. 验收（实装后）

- 启动编辑器：日志出现 `RenderPass 创建成功` → `Framebuffer 创建成功 N 张` → `帧同步原语创建成功` → `首帧 Clear+Present 成功`；窗口由黑屏变为**单色清屏**。
- 缩放窗口：出现 `Framebuffer 重建成功` 类日志；**无** RenderPass/CommandPool/Device/Queue 重建日志；画面随窗口尺寸变化仍单色（无撕裂/无崩溃）。
- 关闭编辑器：Detach 顺序出现 `ClearFrame 释放成功 → Swapchain 释放 → LogicalDevice 释放 → Surface 释放 → Instance 释放`。
- 控制台 Vulkan 日志每条仅一次（单出口去重保持）；无 21:32 种子假日志回归。
- `XuanYu.Render.Vulkan` 与 `XuanYu.Editor.UI` 均 0W0E；所有新增 `.cs ≤100` 行。
- UI 线程不卡死（自动滚动仍稳定、Present 泵在独立线程）。

## 11. 规划态验收

- 本阶段只产出此文档 + changelog / file-tree 同步，**无代码改动**。
- 规划通过后，再开 `VK4-D`（最小清屏闭环实装）。VK4-D 完成后再开 `VK4-E`（补 `IRenderSession` 契约 + `VulkanRenderSessionProvider` 组合根接线，若本轮未提前引入）。

## 12. 规划图（SVG 源，代码框）

```svg
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 680 460" font-family="Segoe UI, sans-serif" font-size="13">
  <rect x="0" y="0" width="680" height="460" fill="#0f1420"/>
  <text x="20" y="28" fill="#7fd1ff" font-size="15" font-weight="bold">VK4-D Attach 链（已建 → 新增）</text>
  <g fill="#1b3a5c" stroke="#3f7fb5" stroke-width="1.5">
    <rect x="40" y="48" width="120" height="30" rx="6"/>
    <rect x="40" y="86" width="120" height="30" rx="6"/>
    <rect x="40" y="124" width="120" height="30" rx="6"/>
    <rect x="40" y="162" width="120" height="30" rx="6"/>
    <rect x="40" y="200" width="120" height="30" rx="6" fill="#2a5c3a" stroke="#5fbf7f"/>
    <rect x="40" y="248" width="120" height="30" rx="6" fill="#3a2a5c" stroke="#9f7fff"/>
  </g>
  <g fill="#dbe7f2" text-anchor="middle">
    <text x="100" y="68">LogicalDevice</text>
    <text x="100" y="106">Queue</text>
    <text x="100" y="144">Swapchain</text>
    <text x="100" y="182">ImageViews</text>
    <text x="100" y="220" fill="#bdf5cf">ClearFrame</text>
    <text x="100" y="268" fill="#d9c2ff">Present 泵</text>
  </g>
  <g stroke="#5f9fd0" stroke-width="2" marker-end="url(#arrow)">
    <line x1="100" y1="78" x2="100" y2="84"/>
    <line x1="100" y1="116" x2="100" y2="122"/>
    <line x1="100" y1="154" x2="100" y2="160"/>
    <line x1="100" y1="192" x2="100" y2="198"/>
    <line x1="100" y1="230" x2="100" y2="246"/>
  </g>
  <text x="360" y="28" fill="#ff9f7f" font-size="15" font-weight="bold">ClearFrame 资源创建序</text>
  <g fill="#2a2f45" stroke="#6f8fb5" stroke-width="1.2">
    <rect x="360" y="48" width="160" height="26" rx="5"/>
    <rect x="360" y="80" width="160" height="26" rx="5"/>
    <rect x="360" y="112" width="160" height="26" rx="5"/>
    <rect x="360" y="144" width="160" height="26" rx="5"/>
    <rect x="360" y="176" width="160" height="26" rx="5"/>
  </g>
  <g fill="#cfe0f2" text-anchor="middle">
    <text x="440" y="66">① RenderPass</text>
    <text x="440" y="98">② CommandPool</text>
    <text x="440" y="130">③ CommandBuffer</text>
    <text x="440" y="162">④ Sem×2 + Fence</text>
    <text x="440" y="194">⑤ Framebuffers[]</text>
  </g>
  <g stroke="#c97f6a" stroke-width="2" marker-end="url(#arrowR)">
    <line x1="440" y1="74" x2="440" y2="78"/>
    <line x1="440" y1="106" x2="440" y2="110"/>
    <line x1="440" y1="138" x2="440" y2="142"/>
    <line x1="440" y1="170" x2="440" y2="174"/>
  </g>
  <text x="20" y="320" fill="#ffd27f" font-size="14" font-weight="bold">VK4-D 边界</text>
  <text x="20" y="344" fill="#bdf5cf" font-size="12">做：RenderPass / Framebuffer / CommandPool / CommandBuffer / Sync / 单帧 Clear+Present</text>
  <text x="20" y="366" fill="#f3a0a0" font-size="12">不做：场景渲染 / 相机 / 网格 / 材质 / Gizmo / UI 叠加 / 持续动画</text>
  <text x="20" y="388" fill="#9fd0ff" font-size="12">Resize：只重建 Framebuffers（RenderPass/CP/CB/Sync 不动）</text>
  <text x="20" y="410" fill="#9fd0ff" font-size="12">Detach：ClearFrame → Swapchain → Device → Surface → Instance</text>
  <text x="20" y="432" fill="#9fd0ff" font-size="12">Present 泵：独立线程，禁在 UI 线程</text>
  <defs>
    <marker id="arrow" markerWidth="10" markerHeight="10" refX="6" refY="3" orient="auto"><path d="M0,0 L6,3 L0,6 Z" fill="#5f9fd0"/></marker>
    <marker id="arrowR" markerWidth="10" markerHeight="10" refX="6" refY="3" orient="auto"><path d="M0,0 L6,3 L0,6 Z" fill="#c97f6a"/></marker>
  </defs>
</svg>
```
