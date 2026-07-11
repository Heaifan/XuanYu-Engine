# RZ-VK5-C 规划 · viewport/scissor 与 Resize 关系验证收口

> 轮次性质：**只规划、不写码**。结论先行：**VK5-C 无需改代码，改为「验证收口轮」**。
> 分支：`fix/RZ-VK3-A-surface-contract`；基线 HEAD：`c53b7a8`（RZ-VK5-D-R3 封版）。

## 0. 一句话结论

经源码取证，**viewport / scissor 已使用动态状态（DynamicState.Viewport + Scissor）**，
**Resize 后 CommandBuffer 必然重录且取最新 Swapchain extent**，**GraphicsPipeline 不随 Resize 重建**。
用户要求的 3 项诉求在当前 `c53b7a8` 代码中**全部已满足** → VK5-C 不做实装，改为「验证收口轮」：
产出验证报告 + 真机 run-list，不触碰任何 `.cs` / `.axaml` / `.csproj`。

> ⚠️ **边界纠正（审计通过时用户要求）**：viewport / scissor 只决定「绘制区域有多大、裁剪到哪里」，
> **不负责几何宽高比保持**。当前测试三角形使用 NDC 固定坐标，视口宽高比变化时其屏幕外观会**变宽 / 变窄 / 变扁**——
> 这属于**预期行为**，是后续 Camera / Projection（或独立宽高比修正阶段）要解决的问题，**不属于 VK5-C 的 viewport/scissor 生命周期范畴**。
> 因此 VK5-C 结论**禁止声称**「三角形在任何宽高比下都不拉伸变形」。

---

## 1. viewport / scissor 当前在哪里设置？

在 `XuanYu.Render.Vulkan/Render/VulkanClearFrameOwner.cs` 的 `RecordDraw(CommandBuffer cb)` 中录制：

```
VulkanClearFrameOwner.cs:76-83
  Viewport* pVp = stackalloc Viewport[1];
  pVp[0] = new Viewport { X=0, Y=0, Width=_extent.Width, Height=_extent.Height, MinDepth=0, MaxDepth=1 };
  Rect2D* pSc = stackalloc Rect2D[1];
  pSc[0] = new Rect2D { Offset=new Offset2D{X=0,Y=0}, Extent=_extent };
  _vk.CmdBindPipeline(cb, PipelineBindPoint.Graphics, _pipeline);
  _vk.CmdSetViewport(cb, 0, 1, pVp);   // ← viewport 在这里按 _extent 设置
  _vk.CmdSetScissor(cb, 0, 1, pSc);    // ← scissor 在这里按 _extent 设置
  _vk.CmdDraw(cb, 3, 1, 0, 0);
```

- viewport / scissor 在**命令录制期**通过 `CmdSetViewport` / `CmdSetScissor` 设置，值来自 `_extent`。
- 同一个 `_extent` 也用于 `RecordOne` 的 `RenderPassBeginInfo.RenderArea`（`:66`），且 Framebuffer 创建时也用同一 `_extent`（`:45`）。
  → Framebuffer extent == RenderArea extent == viewport/scissor extent，**三者同源、始终对齐**。

## 2. 是否已经使用动态状态？

**是。** `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.cs` 创建管线时显式声明：

```
VulkanGraphicsPipelineOwner.cs:57-59
  var viewportState = new PipelineViewportStateCreateInfo { ..., ViewportCount=1, ScissorCount=1 }; // 不填 PViewports/PScissors
  DynamicState* pDynamic = stackalloc DynamicState[2];
  pDynamic[0] = DynamicState.Viewport; pDynamic[1] = DynamicState.Scissor;
  var dynamicState = new PipelineDynamicStateCreateInfo { DynamicStateCount=2, PDynamicStates=pDynamic };
```

- pipeline 创建信息 `PDynamicState = &dynamicState`（`:70`）→ **Viewport 与 Scissor 为动态状态**。
- 含义：这两个值**不烘焙进 Pipeline**，而是在每次录制时经 `CmdSetViewport` / `CmdSetScissor` 注入。
- 直接推论：Pipeline 对象本身与 extent **解耦**，Resize 改变 extent 时 Pipeline **无需重建**（见 Q5）。

## 3. Resize 后 CommandBuffer 是否必然重录？

**是（真实尺寸变化必然重录）。** `VulkanRenderSession.Resize`：

```
VulkanRenderSession.cs:50-66
  if (_swapchainOwner.Extent == 请求尺寸) { 打「Resize 快速跳过」; return; }   // 同尺寸短路，无变化
  _presentLoop.Stop();
  lock (_rebuildLock) {
      _swapchainOwner.Recreate(width, height, _generation);   // 重建 Swapchain+ImageViews
      _clearFrame.RebuildFramebuffers(_generation);           // ← 内部重录 CommandBuffer
      _generation++;
  }
  _presentLoop.Start();
```

- 真实尺寸变化：`RebuildFramebuffers` 末尾（`:47`）调用 `RecordCommandBuffers(_views)` → 重新分配并录制全部 CommandBuffer。
- 同尺寸快速跳过（VK5-D-R3 引入，`:53-54`）：当请求尺寸 == 当前 Swapchain.Extent 时直接 return，**不**重建、**不**重录——这是正确优化（未发生 Resize）。
- `SwapchainOwner.Recreate` 自身（`:55`）与 `RebuildFramebuffers`（`:39`）也各有同尺寸跳过保护，杜绝冗余重建。

## 4. 每次重录是否取最新 Swapchain extent？

**是。** `VulkanClearFrameOwner.RebuildFramebuffers` 第一步即同步最新 extent：

```
VulkanClearFrameOwner.cs:42
  _extent = _swapchainOwner.Extent;   // 先取最新 Swapchain 物理像素 extent
  ...
  :47  RecordCommandBuffers(_views);  // 再用 _extent 重录
```

- `SwapchainOwner.Recreate`（Q3 步骤）先把 `_swapchainOwner._extent` 更新为新的物理像素 extent，之后才调用 `RebuildFramebuffers`。
- 因此 `_extent = _swapchainOwner.Extent` 取到的**永远是刚重建完的、最新的 Swapchain extent**。
- 录制链路 `RecordCommandBuffers → RecordOne(RenderArea=_extent) → RecordDraw(viewport/scissor=_extent)` 全用这一份最新 `_extent`。
- 自愈路径 `RecoverFromOutOfDate`（`:84`）同样走 `RebuildFramebuffers(_generation)`，共享同一取最新 extent 的逻辑。

## 5. GraphicsPipeline 是否会因 Resize 重建？

**不会。** `VulkanGraphicsPipelineOwner` 在会话生命周期内**只创建一次、只销毁一次**：

```
VulkanRenderSession.cs:40   var pipeline = VulkanGraphicsPipelineOwner.Create(...);  // 创建一次
VulkanRenderSession.cs:41   if (pipeline is not null) clear.SetPipeline(pipeline.Pipeline);
VulkanRenderSession.cs:96   _pipeline?.Dispose();                                    // 仅 Dispose 时销毁
```

- `Resize`（`:50-66`）与 `RecoverFromOutOfDate`（`:68-90`）**均未触碰 `_pipeline`**。
- 注入发生在 `Create` 期（`SetPipeline` → `:28` → 触发一次含 Draw 的重录），之后每次 Resize 重录只是**重新 Bind 同一个 `_pipeline` 句柄**（Q1 的 `CmdBindPipeline`）。
- 由于 viewport/scissor 是动态状态（Q2），Pipeline 与 extent 解耦，Resize 不改变 Pipeline 有效性 → **Resize 不重建 Pipeline，正确**。

## 6. VK5-C 是否真的需要改代码？

**不需要。** 用户三项诉求逐项核对：

| 诉求 | 当前状态 | 证据 |
|---|---|---|
| Resize 后 viewport/scissor 取最新 extent | ✅ 已满足 | Q1 + Q4（`_extent = _swapchainOwner.Extent` 后重录） |
| 已使用动态状态 | ✅ 已满足 | Q2（`DynamicState.Viewport/Scissor`） |
| GraphicsPipeline 不因 Resize 重建 | ✅ 已满足 | Q5（创建一次、Resize 不触碰） |

→ **VK5-C 无代码改动必要。** 继续写码只会引入冗余/回归风险，违背「先冻结、先收口」节奏。

**允许确认 / 禁止声称（审计口径）：**
- ✅ viewport / scissor 已声明为动态状态；
- ✅ Resize 后 CommandBuffer 使用最新 Swapchain extent 重录；
- ✅ RenderArea / Framebuffer / viewport / scissor 使用同一最新 extent；
- ✅ GraphicsPipeline 不因 Resize 重建；
- ✅ Resize 后三角形继续显示且不被错误裁切；
- ❌ **禁止声称**三角形在不同视口宽高比下不会拉伸变形（那属 Camera / Projection 范畴）。

## 7. 如果需要，最小改动文件和边界是什么？

**不适用（Q6=不需要）。** 仅作预案记录：
- 若发现 extent 滞后，最小改动点在 `VulkanClearFrameOwner.RecordDraw` 确保读 `_extent`（当前已正确）；
- 若发现重录顺序错，最小改动点在 `VulkanRenderSession.Resize` 保证 `SwapchainOwner.Recreate` 先于 `RebuildFramebuffers`（当前已正确）。
- 二处均已在红线内且当前实现正确，故不启用。

## 8. 如果不需要，是否应改为「验证收口轮」而非「实装轮」？

**是。** VK5-C 重新定义为 **RZ-VK5-C-Validation（验证收口轮）**：
- 交付物 = 本规划文档（取证结论）+ 真机验证 run-list（下方）。
- **零代码改动**（无 `.cs` / `.axaml` / `.csproj` 变更）。
- 真机验收通过后，VK5-C 正式封版，进度指针推进到 **VK5-E**（清 `VulkanClearSession` 死代码 = 债务 B）。

---

## 9. 验证收口轮交付物与真机 run-list

**静态取证（已完成，本轮）**：Q1–Q5 源码证据齐全，逻辑自洽，无需代码改动。

**真机 run-list（供用户 `run.bat` 验收，VK5-D-R3 基础上追加 2 项）**：
1. 启动：蓝灰清屏背景上出现琥珀色固定三角形（VK5-B 既有）。
2. Resize（拖窗口 / 展开日志栏）：三角形继续显示且**不被错误裁切**（但宽高比变化时屏幕外观会随视口变宽 / 变扁，属预期，非 Resize 链路失败）——证明 viewport/scissor 取最新 extent。
3. Resize 后日志：仅出现 `Swapchain.Recreate` + `Framebuffer.Rebuild` + `Resize完成` 配对；**无** `GraphicsPipeline` 重建 / 创建日志（证明 Pipeline 不随 Resize 重建）。
4. 真机日志含 `[T+... gen=N]` 追踪：无重复同尺寸重建；自愈路径（若有 OutOfDate）重录后三角形仍铺满。
5. 关闭窗口：释放顺序 `Present泵停止→Pipeline释放→ClearFrame释放→Swapchain释放→Device→Surface→Instance`，三角形全程未报丢失。
6. 构建：双项目 `dotnet build` 0W0E；全 `.cs` ≤100（维持）。

## 10. 红线（本轮守住，不突破）

- 不建 VertexBuffer / Index / Uniform / DescriptorSet / Mesh。
- 不接 Camera / Scene / Material / Gizmo。
- 不改 UI / NativeHost / LOG-UX。
- 不清 `VulkanClearSession`（留 VK5-E）。
- 不扩大 `Editor.UI → Render.Vulkan` 引用（handle 仍走 Abstractions 契约）。
- 全 `.cs` ≤100；双项目 0W0E。
- 本轮 0 代码改动（纯验证收口）。

## 11. 风险与回滚

- 风险：极低。源码证据已闭合，无改动面。
- 若真机验收发现 viewport/scissor 异常（极不可能），按 Q7 预案定位 `RecordDraw` / `Resize` 顺序，最小改动后单独 commit，不混入本收口轮。
- 回滚：本论无 commit 代码变更；仅文档/日志登记，可随时 `git revert` 文案提交。
