# 8.7.7E-1 — VulkanScene3dRenderer 审计报告

审计日期：2026-06-22
审计版本：`aac69b6`（D-6D 收口后）

## 1. 文件行数统计

| 文件 | 行数 | ≤100 | 白名单 |
|------|------|------|--------|
| `VulkanScene3dRenderer.cs` | **477** | ❌ | `s_lineWhitelist` |
| `VulkanScene3dCommandRecorder.cs` | 200 | ❌ | `s_lineWhitelist` |
| `VulkanScene3dVertexBuffers.cs` | 171 | ❌ | `s_lineWhitelist` |
| `VulkanScene3dVertex.cs` | 171 | ❌ | `s_lineWhitelist` |
| `VulkanScene3dPipelines.cs` | 153 | ❌ | `s_lineWhitelist` |
| `VulkanScene3dRenderResources.cs` | 138 | ❌ | `s_lineWhitelist` |
| `VulkanScene3dShaderModules.cs` | 74 | ✅ | — |
| `VulkanScene3dPipelineLayout.cs` | 67 | ✅ | — |
| `VulkanScene3dRunGate.cs` | 58 | ✅ | — |
| `VulkanScene3dPushConstants.cs` | 38 | ✅ | — |
| `VulkanSceneAxisGeometry.cs` | 33 | ✅ | — |
| `VulkanScene3dInfo.cs` | 31 | ✅ | — |
| `VulkanScene3dStatus.cs` | 11 | ✅ | — |

**Scene3D/ 根目录：13 个 .cs 文件**（需 ≤5，当前在 `s_directoryWhitelist`）

## 2. Renderer 职责块分解

Renderer 只有一个公开方法 `RenderWindows()`，内部执行以下 21 个阶段：

| 阶段 | 行号 | 职责 | 与 Session 重叠？ |
|------|------|------|-------------------|
| 1. Instance | 39-42 | CreateInstance | ✅ **完全重叠** — Session.CreateInstance |
| 2. Surface | 44-46 | CreateSurface | ✅ **完全重叠** — Session.CreateSurface |
| 3. PhysicalDevice | 48-51 | SelectDevice | ✅ **完全重叠** — Session.SelectDevice |
| 4. LogicalDevice | 52-54 | CreateDevice | ✅ **完全重叠** — Session.CreateDevice |
| 5. 函数指针加载 | 56-71 | LoadDeviceProc ×5 | ✅ **完全重叠** — Session.LoadSessionFunctionPointers |
| 6. Swapchain 创建 | 72-104 | Query/Swapchain/Images | ✅ **完全重叠** — Swapchain/CreateFlow |
| 7. Color ImageViews | 106-119 | Create ImageViews | ✅ **完全重叠** — Swapchain/ImageViews |
| 8. Depth | 121-139 | Format + Attachments | ✅ **完全重叠** — Depth/ |
| 9. RenderPass | 142-181 | Create RenderPass | ✅ **完全重叠** — Swapchain/Framebuffers |
| 10. Shader Modules | 185-189 | 委托 VulkanScene3dShaderModules | ✅ **共享** |
| 11. Pipeline Layout | 191-195 | 委托 VulkanScene3dPipelineLayout | ✅ **共享** |
| 12. Pipelines | 197-204 | 委托 VulkanScene3dPipelines | ✅ **共享** |
| 13. VertexBuffers | 206-213 | 委托 VulkanScene3dVertexBuffers | ✅ **共享** |
| 14. Framebuffers | 215-234 | Create Framebuffers | ✅ **完全重叠** — Swapchain/Framebuffers |
| 15. CommandPool/Buffer | 236-244 | CreatePool + AllocBuffer | ✅ **完全重叠** — Swapchain/Sync |
| 16. Sync Objects | 246-253 | Semaphores + Fence | ✅ **完全重叠** — Swapchain/Sync |
| 17. Acquire Image | 255-263 | AcquireNextImageKHR | ✅ **完全重叠** — FrameFlow/Acquire |
| 18. MVP Compute | 265-267 | ViewProjection | ✅ **重叠** — Frame.cs ComputeViewProjection |
| 19. Unit MVP | 269-281 | Per-object MVP | ✅ **重叠** — Frame.cs BuildUnitDrawData |
| 20. Command Record | 283-297 | 委托 VulkanScene3dCommandRecorder | ✅ **共享** |
| 21. Submit + Present | 299-330 | QueueSubmit + QueuePresent | ✅ **完全重叠** — FrameFlow/Submit + Present |
| 22. DeviceWaitIdle | 332 | 等待完成 | 🔶 **独有** — 旧探针路径特点 |
| 23. 结果构造 | 335-352 | VulkanScene3dInfo 构建 | 🔶 **类似** — 但使用 Info 而非 FrameResult |

**结论：Renderer 约 80% 的代码与 Session 重叠。它是旧探针代码。**

## 3. Renderer 子模块调用关系

```
VulkanScene3dRenderer.RenderWindows()
 ├── VulkanScene3dShaderModules.Create()         [共享, 74行]
 ├── VulkanScene3dPipelineLayout.Create()        [共享, 67行]
 ├── VulkanScene3dPipelines.Create()             [共享, 153行, >100]
 ├── VulkanScene3dVertexBuffers.Create()         [共享, 171行, >100]
 ├── VulkanScene3dDepthAttachments.Create()      [共享, Depth/]
 ├── VulkanScene3dDepthFormatSelector.Select()   [共享, Depth/]
 ├── VulkanScene3dCommandRecorder.Record()       [共享, 200行, >100]
 │
 ├── VulkanScene3dRenderResources (持有+释放)     [138行, >100]
 │
 └── 调用方: EditorScene3dCommandRoute.ExecuteRun()
      → 诊断探针路径（旧），非 Session 主路径
```

## 4. 调用方分析

**唯一调用方：** `EditorScene3dCommandRoute.ExecuteRun()` (EditorScene3dCommandRoute.cs:37)

该路径是"探针 Run"路径，与主 `Session` 路径（`EditorScene3dCommandRoute.ExecuteRestart()`）并行存在：

```
EditorScene3dCommandRoute
 ├── ExecuteRun()      → VulkanScene3dRenderer.RenderWindows()  ← 探针（旧）
 └── ExecuteRestart()  → VulkanScene3dSession.Start()           ← 主路径
```

## 5. 风险评估

| 风险 | 级别 | 说明 |
|------|------|------|
| 代码重复 | 🔴 高 | ~300 行创建逻辑与 Session 重复 |
| 资源泄漏风险 | 🟡 中 | RenderResources.Dispose() 在 finally 中执行，但探针中途失败时资源追踪靠 flag |
| 探针与 Session 混淆 | 🟡 中 | 两个路径创建同样的 Vulkan 对象，修改一个必须同步另一个 |
| 废弃可能性 | 🟢 低 | 如果后续 Session 完成替代，Renderer 探针可以移除 |

## 6. E-2 建议拆法

### 推荐方案：先拆共享模块，最后收 Renderer

```
E-2A — Scene3D 目录文件数合规
      核心思路：Scene3D/ 根目录 13 文件 → 拆到子目录
      子目录建议：
        Scene3D/Pipeline/     Pipelines.cs + PipelineLayout.cs + ShaderModules.cs + PushConstants.cs
        Scene3D/Vertex/       Vertex.cs + VertexBuffers.cs + AxisGeometry.cs
        Scene3D/Render/       Renderer.cs + RenderResources.cs + RenderGate.cs + Info.cs + Status.cs
        Scene3D/Commands/     CommandRecorder.cs
  
E-2B — 共享模块收口
      确保 Session 和 Renderer 探针都使用同一份 Pipeline/Shader/Vertex 创建逻辑
      不修改创建逻辑，只确保路径统一
  
E-2C — Renderer 文件级拆分 （仅如果探针仍需要保留）
      将 RenderWindows 的 21 个阶段按子步骤拆到显式方法
      （例如 CreateSwapchainForProbe / AcquireForProbe / SubmitForProbe）
  
E-2D — 白名单删除
      Scene3D/ 目录白名单清理
      Renderer.cs 行白名单清理
```

### 重要判定

**Renderer 探针是否可以删除？**
- 如果 `ExecuteRestart()`（Session 路径）已经完全替代 `ExecuteRun()`（探针路径），则可以删除 Renderer
- 这需要 Editor 团队确认
- **E-2A 阶段不建议删除**，只做目录合规和白名单清理

## 7. 当前 Scene3D/ 白名单债务

| 债务类型 | 条目 | 当前值 | 目标 |
|---------|------|--------|------|
| 目录白名单 | `Scene3D\` | 13 文件 | ≤5 |
| 行白名单 | `VulkanScene3dRenderer.cs` | 477 行 | ≤100 |
| 行白名单 | `VulkanScene3dCommandRecorder.cs` | 200 行 | ≤100 |
| 行白名单 | `VulkanScene3dVertexBuffers.cs` | 171 行 | ≤100 |
| 行白名单 | `VulkanScene3dVertex.cs` | 171 行 | ≤100 |
| 行白名单 | `VulkanScene3dPipelines.cs` | 153 行 | ≤100 |
| 行白名单 | `VulkanScene3dRenderResources.cs` | 138 行 | ≤100 |

总计白名单预算占用：目录 1 + 行 6 = 7/83 预算
