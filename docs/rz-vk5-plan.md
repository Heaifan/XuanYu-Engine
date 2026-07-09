# rz-vk5-plan.md — VK5 最小几何渲染闭环规划（Shader + Pipeline → 固定三角形 → Resize 兼容 → 渲染命令边界）

> 规划态文档：本轮只规划 **VK5 最小几何渲染** 的阶段边界、资源顺序、Resize 兼容与文件结构，**不写任何 Vulkan 实装代码**。
> VK5 的目标是从「单色清屏」进入「最小图元渲染」——先画一个固定三角形/固定测试图元，**不接入真实场景、不接入 Gizmo、不接入材质系统**。

## 0. 规划前提（VK4 已收口）

见 `docs/rz-vk4-closure.md`。VK4 已跑通完整最小清屏闭环，画面为单色蓝灰，Resize/DPI/释放顺序均真机验证通过，双项目 0W0E，全 `.cs` ≤100。

VK5 在 VK4-D 的 CommandBuffer 录制里，从「只 Clear」扩展到「Clear + Draw 一个固定三角形」。

## 1. VK5 核心目标（只做这些）

> 从"单色清屏"进入"最小图元渲染"，先画一个固定三角形或固定测试图元，**不接入真实场景、不接入 Gizmo、不接入材质系统**。

| 做 ✅ | 不做 ❌ |
|---|---|
| ShaderModule（顶点 + 片元，固定内嵌测试 shader 或明确 shader 文件方案） | 场景树渲染 |
| PipelineLayout（最小，无 descriptor set） | 相机控制 / 视图投影矩阵 |
| GraphicsPipeline（绑定 RenderPass） | 网格导入 / VertexBuffer / IndexBuffer |
| 在现有 CommandBuffer 里 `Draw`（用 `gl_VertexIndex` 画固定三角形） | 材质系统 / 贴图 / descriptor |
| viewport / scissor 与 Swapchain extent 同步 | Gizmo / 拾取 / 选中高亮 |
| Resize 后三角形仍正常显示 | UI 叠加 / HUD / ImGui |
| 中文生命周期日志 | 多帧资源池 / 并行多帧 |

> **VK5 验收标志：蓝灰背景上出现一个固定三角形**，随窗口缩放仍正常显示。看不到网格/物体/场景是预期。

## 2. VK5 分阶段

### VK5-A：Shader + Pipeline
- 新增 `ShaderModule`（顶点 + 片元）。
- 新增 `PipelineLayout`（最小，无 descriptor set / push constant，或仅极少 push constant）。
- 新增 `GraphicsPipeline`（绑定 VK4-D 已有的 `RenderPass`）。
- 使用**固定内嵌测试 shader**（SPIR-V 字节内嵌）或**明确 shader 文件方案**（`.vert`/`.frag` → `.spv` 编译产物随包）——二选一，实装前定死，避免临时决策。
- **不接场景、不接相机、不接网格资源。**
- 本阶段可先出规划/实装，但 Pipeline 建成后未必立刻能在屏幕看到三角形（Draw 在 VK5-B）。

### VK5-B：Draw Triangle
- 在 VK4-D 现有 CommandBuffer 里，从 Clear 扩展到 Clear + Draw。
- 使用 `gl_VertexIndex` 在顶点着色器里**硬编码 3 个顶点位置**画固定三角形，**暂不建 VertexBuffer**。
- 验收：蓝灰背景上出现一个固定三角形。

### VK5-C：Resize 兼容
- Pipeline / viewport / scissor 与 Swapchain extent 同步（推荐 viewport/scissor 走动态状态 `VK_DYNAMIC_STATE_VIEWPORT/SCISSOR`，Resize 时只更新动态状态，不重建 Pipeline；若用静态 viewport 则 Resize 需重建 Pipeline，成本更高，规划时二选一定死）。
- Resize 后三角形仍正常显示（不变形、不越界、不撕裂）。
- **不重建 Surface / Instance / Device / Queue**（继承 VK4 红线）。

### VK5-D：最小渲染命令边界
- 把 Clear / Draw 的职责收进更清晰的 `RenderPass` 组织或 `FrameRenderer` 抽象，明确「录制什么」与「泵什么」的边界。
- **但不进入场景系统**——仍是固定三角形，不接受外部几何输入。
- 目的：为 VK6+ 真实几何/场景预留干净接缝，而不是现在就把场景塞进来。

## 3. 资源创建顺序（Attach，在 VK4-D 之后）

```
RenderPass（VK4-D 已有）
   → ShaderModule（vert + frag）      (VK5-A)
   → PipelineLayout                   (VK5-A)
   → GraphicsPipeline（绑定 RenderPass） (VK5-A)
   → CommandBuffer 录制：Clear + Draw  (VK5-B)
   → viewport/scissor 动态状态同步 extent (VK5-C)
```

硬约束：

- **ShaderModule 可在 Pipeline 建成后销毁**（Pipeline 已内联 SPIR-V），不必长期持有。
- **GraphicsPipeline 尽量只建一次**：viewport/scissor 走动态状态则 Resize 不重建 Pipeline；否则 Resize 需重建（规划时二选一）。
- **PipelineLayout 只建一次**，Resize 不重建。
- **CommandBuffer 录制**：VK4-D 是「每 Swapchain 图像录一次静态 clear」；VK5 增加 Draw 后仍可静态录制（固定三角形无每帧变化），Resize 重建 Framebuffer 后需重录。

## 4. Dispose / Detach 释放顺序（在 VK4-D 之上追加）

```
Present 泵停止（VK4-D）
   → GraphicsPipeline 释放            (VK5 新增，最先)
   → PipelineLayout 释放              (VK5 新增)
   → (ShaderModule 若仍持有则释放)     (VK5 新增)
   → ClearFrame 释放（Framebuffers → CommandPool → RenderPass → Sync）（VK4-D）
   → Swapchain → LogicalDevice → Surface → Instance（VK4-D）
```

红线：

- Pipeline / PipelineLayout 依赖 RenderPass + Device，必须在 RenderPass 与 Device 释放**之前**释放。
- 禁止 Dispose 抛异常；所有释放包 `try/catch` 或幂等，异常仅记日志。

## 5. VK5 红线

1. 不做场景树渲染
2. 不做相机控制
3. 不做网格导入
4. 不做材质系统
5. 不做 Gizmo
6. 不做 UI 叠加
7. 不做编辑器拾取
8. 不做多帧资源池
9. 不把 Bridge 写胖（Bridge 仅委托 `VulkanRenderSession`，新增能力落在独立 owner / step / session）
10. 不让 `Editor.UI` 接触 `Silk.NET.Vulkan`
11. 所有 `.cs` ≤100 行
12. 每个阶段独立 commit

## 6. 文件结构与接线（规划，均 ≤100 行）

沿用 VK4「独立 owner + 独立 attach step + 薄 session 组合根」同构。VK5 建议在 `Render/`（或新 `Pipeline/`）子目录下：

- `Pipeline/VulkanShaderModuleOwner.cs`（≤100）：建/销毁 vert+frag ShaderModule，暴露只读句柄；SPIR-V 来源固定内嵌或文件加载二选一。
- `Pipeline/VulkanGraphicsPipelineOwner.cs`（≤100）：建 PipelineLayout + GraphicsPipeline（绑定 RenderPass），暴露 `Pipeline`/`Layout` 只读；viewport/scissor 走动态状态。
- `Pipeline/VulkanPipelineLogFormatter.cs`（≤100）：中文日志格式器（与既有 formatter 同构）。
- `Render/VulkanClearFrameOwner.cs`（VK4-D 已有）：录制处从「只 Clear」扩展到「Clear + BindPipeline + SetViewport/Scissor + Draw(3,1,0,0)」；若行数逼近 100 则拆出 `VulkanTriangleRecorder.cs`。
- `Session/VulkanRenderSession.cs`（VK4-D 已有薄组合根）：追加装配 Shader + Pipeline，Attach 顺序在 Swapchain/RenderPass 之后、Present 泵启动之前；Resize 只更新动态状态（不重建 Pipeline）。
- `Bridge/VulkanBridgeRenderSessionAttachStep.cs`（VK4-D 已有）：仍仅委托，不内联 VK5 细节。

## 7. 命名与行数红线（继承 VK4）

- 业务 owner：`VulkanShaderModuleOwner` / `VulkanGraphicsPipelineOwner`；日志 `VulkanPipelineLogFormatter`。
- `Silk.NET.Vulkan` 与已有别名冲突时沿用「`Vulkan` 前缀 + 语义属性名」惯例，禁用裸 `Device`/`Pipeline` 作属性名如产生歧义。
- 所有新增 `.cs ≤100` 行；单职责；子目录核心文件 ≤5–7。
- `VulkanDeviceOwner` / `VulkanSwapchainOwner` 职责不变，**VK5 禁止往里塞 Pipeline/Shader**。

## 8. UI / 依赖红线（继承全局约束）

- `Editor.UI` **不得**新增 `Silk.NET.Vulkan` 使用点，**不得**持有 Pipeline/Shader/CommandBuffer 等类型。
- UI 仅经 `Render.Abstractions` 契约交互；Pipeline / Draw 细节不外露给 UI。
- 渲染泵仍在 `Render.Vulkan` 内部后台线程，不触碰 Avalonia / UI 线程；日志回调经消费方切回 UI 线程。
- VkResult 必须保存并分类处理；UI 线程禁无限等待。
- 持续场景提交循环 / 动画 **禁止在 VK5 引入**，VK6+ 再做。

## 9. 防回潮门禁（VK5 实装时）

- Resize 不重建 Surface / Instance / LogicalDevice / Queue / RenderPass / CommandPool / Sync；Pipeline 尽量不随 Resize 重建（动态 viewport/scissor）。
- 不在 `Foot.axaml.cs` 或任何 UI 代码后置里写渲染/Pipeline/Vulkan 逻辑；自动滚动仍在 `LogListAutoScrollController`，勿回潮。
- 不把旧探针 `VulkanClearSession` 复制进正式路径。
- `Editor.UI` 不新增 `Silk.NET.Vulkan` 使用点。
- 所有新增 `.cs ≤100` 行；单职责。
- 固定三角形验证通过前，禁止追加任何 VertexBuffer / 网格 / 材质 / 相机逻辑。

## 10. 验收（实装后，逐阶段）

- **VK5-A**：日志出现 `ShaderModule 创建成功` → `PipelineLayout 创建成功` → `GraphicsPipeline 创建成功`；画面仍单色清屏（Draw 未加）。双项目 0W0E，全 `.cs` ≤100。
- **VK5-B**：蓝灰背景上出现一个固定三角形；日志出现 `首帧 Draw 三角形成功`（或等价）；无闪退。
- **VK5-C**：缩放窗口三角形仍正常显示、不变形；**无** Pipeline/RenderPass/Device/Queue 重建日志（或仅 Framebuffer 重建）。
- **VK5-D**：Clear/Draw 职责收敛到清晰边界；关闭编辑器 Detach 顺序出现 `Pipeline 释放 → Layout 释放 → ClearFrame 释放 → Swapchain → Device → Surface → Instance`。
- 全程控制台 Vulkan 日志单出口去重；UI 线程不卡死。

## 11. 规划态验收

- 本阶段只产出 `docs/rz-vk4-closure.md` + `docs/rz-vk5-plan.md` + `changelog.md` / `file-tree.md` 同步，**无任何代码改动**（不改 `.cs` / `.axaml` / `.csproj`）。
- 规划通过后再开 **VK5-A**（Shader + Pipeline），逐阶段独立 commit。

## 12. 规划图（SVG 源，代码框）

```svg
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 720 220" width="720" height="220" role="img">
  <title>VK5 最小几何渲染路线</title>
  <defs>
    <marker id="arrow" viewBox="0 0 10 10" refX="8" refY="5" markerWidth="6" markerHeight="6" orient="auto">
      <path d="M2 1L8 5L2 9" fill="none" stroke="#52606D" stroke-width="1.5"/>
    </marker>
  </defs>
  <rect x="20" y="30" width="130" height="60" rx="10" fill="#EAF3DE" stroke="#3B6D11"/>
  <text x="85" y="65" text-anchor="middle" font-size="13" fill="#27500A">VK4-D Clear</text>

  <rect x="210" y="30" width="130" height="60" rx="10" fill="#E6F1FB" stroke="#185FA5"/>
  <text x="275" y="65" text-anchor="middle" font-size="13" fill="#0C447C">VK5-A Pipeline</text>

  <rect x="400" y="30" width="130" height="60" rx="10" fill="#E6F1FB" stroke="#185FA5"/>
  <text x="465" y="65" text-anchor="middle" font-size="13" fill="#0C447C">VK5-B Triangle</text>

  <rect x="570" y="30" width="130" height="60" rx="10" fill="#F1EFE8" stroke="#5F5E5A"/>
  <text x="635" y="65" text-anchor="middle" font-size="13" fill="#444441">VK5-C Resize</text>

  <line x1="150" y1="60" x2="208" y2="60" stroke="#52606D" stroke-width="1.5" marker-end="url(#arrow)"/>
  <line x1="340" y1="60" x2="398" y2="60" stroke="#52606D" stroke-width="1.5" marker-end="url(#arrow)"/>
  <line x1="530" y1="60" x2="568" y2="60" stroke="#52606D" stroke-width="1.5" marker-end="url(#arrow)"/>

  <text x="20" y="140" font-size="13" fill="#334155">VK5 只进入最小图元渲染，不进入场景 / 相机 / Gizmo / 材质系统。</text>
  <text x="20" y="166" font-size="12" fill="#64748B">VK5-D：把 Clear / Draw 职责收进清晰的 RenderPass / FrameRenderer 边界，但不进场景系统。</text>
  <text x="20" y="192" font-size="12" fill="#64748B">红线：不建 VertexBuffer（gl_VertexIndex 硬编码 3 顶点）；所有 .cs ≤100；每阶段独立 commit。</text>
</svg>
```
