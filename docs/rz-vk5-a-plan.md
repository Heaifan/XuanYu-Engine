# rz-vk5-a-plan.md — RZ-VK5-A 规划：ShaderModule + GraphicsPipeline 最小接入

> 规划态文档（RZ-VK5-A-Plan）：本轮**只规划、不写代码**。在 VK4-D Clear+Present 稳定闭环上，规划 VK5-A 的 ShaderModule + PipelineLayout + GraphicsPipeline 最小接入方案，供用户确认后再进入实装。
> 红线（本轮）：不写任何代码；不新增/修改任何 `.cs` / `.axaml` / `.csproj`。

## 0. 规划前提与必读

- 已读：`docs/rz-vk4-closure.md`（VK4 收口）、`docs/rz-vk5-plan.md`（VK5 总规划 §2/§3/§4/§6/§12）、`file-tree.md`、`docs/dev-rules.md`、`docs/dev-rules-understanding.md`。
- `docs/dev-rules.md` **已存在**（路径 `docs/dev-rules.md`，file-tree 第 247/266 行已登记），本轮**不擅自新建**。
- 已实读当前 Vulkan Clear+Present 源文件：`Session/VulkanRenderSession.cs`、`Render/VulkanClearFrameOwner.cs`、`Render/VulkanPresentLoop.cs`、`Swapchain/VulkanSwapchainOwner.cs`、`Device/VulkanDeviceOwner.cs`、`Bridge/VulkanBridgeRenderSessionAttachStep.cs`。
- VK4-D 现状：单色清屏闭环跑通，画面蓝灰，Resize/DPI/释放顺序真机验证；双项目 0W0E，全 `.cs` ≤100。GraphicsPipeline 尚未存在。

## 1. 当前 Vulkan 文件清单与职责

- `VulkanInstanceOwner` / `VulkanSurfaceOwner`：`VkInstance` + `VkSurfaceKHR` 创建/释放（VK3）。
- `Device/`：`VulkanPhysicalDeviceSelector`（枚举/选择）、`VulkanPhysicalDeviceInfo` / `VulkanPhysicalDeviceSelection`（纯数据结果）、`VulkanDeviceOwner`（LogicalDevice + Graphics/Present 队列，暴露 `LogicalDevice` / `GraphicsQueue` / `PresentQueue`）。
- `Swapchain/`：`VulkanSwapchainCapabilities`（查 caps）、`VulkanSwapchainBuilder`（建/重建）、`VulkanSwapchainOwner`（持有 Swapchain+Image+ImageView，暴露 `Format` / `Extent` / `ImageViews` / `Swapchain` / `Khr`）、`VulkanSwapchainLogFormatter`。
- `Render/`：`VulkanClearFrameOwner`（RenderPass + CommandPool + CommandBuffer[] + Framebuffer[]；暴露 `CommandBuffers` / `Extent`；**RenderPass 在构造时建一次，Resize 只重建 Framebuffer、不重建 RenderPass**）、`VulkanClearFrameLogFormatter`、`VulkanPresentLoop`（独立后台线程 Acquire→Submit→Present，读 `clearFrame.CommandBuffers[idx]` 提交）。
- `Session/VulkanRenderSession`：薄组合根。Attach 顺序 Device→Swapchain→ClearFrame→PresentLoop.Start；Resize = Stop→Swapchain.Recreate→ClearFrame.RebuildFramebuffers→Start；Dispose 逆序。
- `Bridge/VulkanBridgeRenderSessionAttachStep`：仅委托 `VulkanRenderSession.Create`。
- **关键事实**：`PresentLoop` 提交的是 `ClearFrameOwner` 录好的 CommandBuffer；在录制里加 `CmdBindPipeline`+`CmdDraw` 会被自动提交，**PresentLoop 全程不感知 Pipeline → VK5-A/B 无需改动 PresentLoop**。

## 2. VK5-A 拟新增/修改文件清单

新增（`XuanYu.Render.Vulkan/Pipeline/`，均 ≤100）：

- `VulkanShaderBytecode.cs`：静态 `byte[]` 存放顶点+片元 SPIR-V（固定三角形），避免引入着色器编译工具链。
- `VulkanShaderModuleOwner.cs`：用 `byte[]` 建/销 vert+frag 两个 `ShaderModule`，暴露只读句柄。
- `VulkanGraphicsPipelineOwner.cs`：建 PipelineLayout（空，无 descriptor）+ GraphicsPipeline（绑定 RenderPass，动态 viewport/scissor，空 vertex input，TriangleList），暴露 `Pipeline` / `Layout` 只读。
- `VulkanPipelineLogFormatter.cs`：中文日志格式器（与既有 formatter 同构）。

修改（最小改动，守住 ≤100）：

- `Render/VulkanClearFrameOwner.cs`：+1 行只读 getter `public RenderPass RenderPass => _renderPass;`（供 Pipeline 绑定；构造时建一次，Resize 稳定）。[93→94]
- `Session/VulkanRenderSession.cs`：+字段 `VulkanGraphicsPipelineOwner _pipeline`；`Create` 中在 ClearFrame 之后建 Pipeline；`Dispose` 中**最先**释放 Pipeline。[59→~69]

不改：PresentLoop / SwapchainOwner / DeviceOwner / Bridge 各 step / Editor.UI / NativeHost / LOG-UX。

## 3. ShaderModule 创建与释放顺序

创建（LogicalDevice 就绪后）：

1. 取 `VulkanShaderBytecode.Vertex` / `.Fragment` 两个 `byte[]`。
2. 各 `ShaderModuleCreateInfo { SType, CodeSize, PCode = (uint*)byte数组 }` → `vk.CreateShaderModule(LogicalDevice, ...)` ×2，VkResult 必检。
3. 持有 `vert` / `frag` 只读句柄。

释放：VK5-A 选择**持有到会话结束**（最简单、规避「建完即销 ShaderModule 导致 Pipeline 悬挂」的坑）；`Dispose` 中在 GraphicsPipeline 之后、ClearFrame 之前销毁两个 ShaderModule。优化（建完即销）留待 VK5-A 实装后按验证结果决定，不在规划内定死。

## 4. PipelineLayout 创建与释放顺序

创建：LogicalDevice 就绪后，`PipelineLayoutCreateInfo { SType, SetLayoutCount=0, PSetLayouts=null, PushConstantRangeCount=0 }` → `vk.CreatePipelineLayout`。无 descriptor / 无 push constant（与 VK5 红线一致）。

释放：在 GraphicsPipeline 之后销毁。

## 5. GraphicsPipeline 创建与释放顺序

创建（依赖全部就绪后）：`GraphicsPipelineCreateInfo` 关键字段——

- `Layout = pipelineLayout`
- `RenderPass = clearFrame.RenderPass`，`Subpass = 0`
- `Stages`：vert（`ShaderStageFlags.VertexBit`, vertModule, "main"）+ frag（`ShaderStageFlags.FragmentBit`, fragModule, "main"）
- `VertexInputState`：空（`VertexBindingDescriptionCount=0` / `VertexAttributeDescriptionCount=0`）——对应 `gl_VertexIndex`，**不建 VertexBuffer**
- `InputAssemblyState`：`Topology = PrimitiveTopology.TriangleList`
- `ViewportState`：**动态**（`PViewports=null`, `ViewportCount=1`, `PScissors=null`, `ScissorCount=1`）+ `PDynamicStates` 含 `DynamicState.Viewport` + `DynamicState.Scissor`——保证 Resize 不重建 Pipeline
- `RasterizationState` / `MultisampleState` / `ColorBlendState`（单附件，格式同 `Swapchain.Format`，op=Replace）
- `vk.CreateGraphicsPipelines(LogicalDevice, null, 1, &info, null, &pipeline)`，VkResult 必检。

释放：**最先**（在 Layout / ShaderModule / ClearFrame 之前）。

## 6. RenderPass / Swapchain / Framebuffer / Pipeline 依赖关系

- `LogicalDevice`（DeviceOwner）是一切根：`ShaderModule` / `PipelineLayout` / `GraphicsPipeline` 都依赖它。
- `RenderPass`（ClearFrameOwner）依赖 `Swapchain.Format` + `LogicalDevice`；**构造时建一次，Resize 只重建 Framebuffer、不重建 RenderPass** → 绑定该 RenderPass 的 GraphicsPipeline 在 Resize 时**无需重建**（关键结论）。
- `GraphicsPipeline` 依赖：`LogicalDevice` + `RenderPass` + `vert` + `frag` + `PipelineLayout` + `Swapchain.Format`（颜色附件格式）+ viewport/scissor 配置。
- `Framebuffer` 依赖 `RenderPass` + `Swapchain.ImageViews`；Resize 随 Swapchain 重建，但 RenderPass 不变 → Framebuffer 重建不影响 Pipeline。
- `CommandBuffer`（ClearFrameOwner 录制）→ `PresentLoop` 提交；VK5-B 在其中 `CmdBindPipeline(pipeline)+CmdDraw` 后，PresentLoop 自动提交，**零改动**。
- 依赖时序（Attach）：Device → Swapchain → ClearFrame(RenderPass) → ShaderModule(vert/frag) → PipelineLayout → GraphicsPipeline。
- 依赖时序（Detach，逆序，最先释放 Pipeline）：GraphicsPipeline → PipelineLayout → ShaderModule → ClearFrame(Framebuffer→CommandPool→RenderPass→Sync) → Swapchain → Device → Surface → Instance。

## 7. ≤100 行拆分方案

- `VulkanShaderBytecode.cs`：仅静态 `byte[]`（约 30–50 行，含 vert+frag SPIR-V 常量）。
- `VulkanShaderModuleOwner.cs`：建/销 2 个模块，约 60–80 行。
- `VulkanGraphicsPipelineOwner.cs`：建 Layout+Pipeline，约 80–95 行（逼近上限时把「取 SPIR-V 字节」留在 Bytecode 文件，本类只做 Vulkan 调用）。
- `VulkanPipelineLogFormatter.cs`：中文格式，约 15–20 行。
- `VulkanClearFrameOwner.cs`：+1 行 getter（94≤100）。
- `VulkanRenderSession.cs`：+约 10 行（69≤100）。
- 子目录 `Pipeline/` 文件数 = 4，≤7 上限。
- 命名：`VulkanShaderModuleOwner` / `VulkanGraphicsPipelineOwner` / `VulkanPipelineLogFormatter`；禁用裸 `Pipeline`/`Device` 作属性名（用 `Pipeline`/`Layout` 只读或 `GraphicsPipeline` 别名）。

## 8. 本轮（RZ-VK5-A-Plan）禁止事项

- 不写任何代码（本文件仅规划）。
- 不新增/修改任何 `.cs` / `.axaml` / `.csproj`。

## 8b. VK5-A 实装禁止事项（承 red lines，供确认后执行）

- 不 Draw（VK5-A 仅建 Pipeline，不绑定不绘制；绘制在 VK5-B）。
- 不画三角形、不建 VertexBuffer、不建 DescriptorSet。
- 不接 Scene / Camera / Mesh / Material / Gizmo / 拾取 / UI 叠加。
- 不改 UI / NativeHost / LOG-UX / Resize 逻辑。
- 不扩大 `Editor.UI` 对 `Render.Vulkan` 或 `Silk.NET.Vulkan` 的引用（债 A 禁止扩大）。
- 不清理 `VulkanClearSession` 死代码（债 B 排 VK5-E）。
- 不把 Bridge 写胖（Bridge step 仍只委托；Pipeline 逻辑落在 `Pipeline/` 独立 owner）。
- 所有新增 `.cs` ≤100；每阶段独立 commit。

## 9. 验收清单（VK5-A 实装后）

- 构建：XuanYu.Render.Vulkan + XuanYu.Editor.UI 双项目 **0W0E**。
- 日志依次出现：`ShaderModule 创建成功` → `PipelineLayout 创建成功` → `GraphicsPipeline 创建成功`（经 `VulkanPipelineLogFormatter`）。
- 画面仍单色清屏（无 Draw，三角形不可见）。
- Detach 释放顺序：`Pipeline 释放 → Layout 释放 → ClearFrame 释放 → Swapchain → Device → Surface → Instance`。
- 全新增 `.cs` ≤100；子目录 `Pipeline/` ≤7 文件。
- 无 UI / NativeHost / LOG-UX / Resize 改动；无 Editor.UI 新增 `Silk.NET.Vulkan` 引用。
- 控制台日志单出口。

## 10. 风险点与回滚方案

风险：

- R1 SPIR-V 合法性：硬编码三角形 SPIR-V 须为合法模块（entry `main`，正确 capability）。错则 `CreateShaderModule` 失败。→ VkResult 必检，失败仅记日志、Pipeline 置 null、不影响 Clear+Present。
- R2 Silk.NET 动态状态枚举名：`DynamicState.Viewport` / `Scissor` 成员名须与 2.22.0 一致。→ 实装前先确认枚举名；若不确定，VK5-A 退化为**静态 viewport**（写死 extent），Resize 重建 Pipeline 推迟到 VK5-C 再改动态；推荐优先确认动态枚举名、一步到位。
- R3 ShaderModule 生命周期：建完即销可能使 Pipeline 悬挂。→ VK5-A 选择持有到会话结束（见 §3），规避。
- R4 RenderPass 稳定性假设：若某路径重建 RenderPass，绑定它的 Pipeline 会失效。→ 已确认 ClearFrameOwner 仅在构造建 RenderPass、Resize 不重建，假设成立；`RebuildFramebuffers` 不改 RenderPass。

回滚：

- VK5-A 仅新增 4 文件 + 改 2 文件（RenderPass getter + RenderSession 接线），且 Pipeline 在 VK5-A 未被绑定（无 Draw）→ 回滚 = 删除 4 新文件 + 还原 2 处小改动，`git revert` 单 commit 即可，不影响现有 Clear+Present 闭环。
- 独立 commit，不与 VK5-B/C/D 混。

## 11. 决策点（实装前定死，本轮建议）

- **D1 Shader 来源**：选「内嵌 SPIR-V `byte[]`」（`VulkanShaderBytecode.cs`），不引入 `.vert/.frag→.spv` 编译工具链（仓库当前无 glslang 工具链）。若后续需要可换文件方案。
- **D2 viewport/scissor**：选「动态状态」（一步到位，Resize 不重建 Pipeline），前提 R2 枚举名确认。
- **D3 ShaderModule 存活**：选「持有到会话结束」（安全优先）。
