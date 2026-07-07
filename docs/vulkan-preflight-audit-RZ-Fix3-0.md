# RZ-Fix3-0: Vulkan 接入前置审计

日期：2026-07-07

## 审计结论

当前工程已经不是纯 UI 占位视口状态：中央视口已经挂载 `VulkanViewport`，并且 `Viewport/Vulkan` 下已经存在 Instance、Win32 Surface、Device、Swapchain 的最小生命周期代码。

因此本阶段结论是：

- 可以继续以中央视口为唯一 Vulkan 接入点。
- 不建议继续扩大到完整 Renderer、Gizmo、Picking、相机、模型或资源系统。
- 下一步应把现有代码重新收口为 RZ-Fix3-A 最小 Clear Probe，而不是直接进入完整 3D 渲染器。
- 当前顶部、左侧、右侧、底部日志系统职责不应继续改动。

## 当前接入位置

中央视口路径：

1. `UiRoot.axaml`
2. `Main.axaml`
3. `Viewport/Vulkan/VulkanViewport.axaml`
4. `Viewport/Vulkan/VulkanNativeHost.cs`
5. `Viewport/Vulkan/Win32ViewportHost.cs`
6. `Viewport/Vulkan/VulkanClearSession*.cs`

`UiRoot.axaml` 的中间列承载 `Main`，`Main.axaml` 当前直接承载 `VulkanViewport`。`VulkanViewport` 内部用 `VulkanNativeHost` 创建 Win32 子窗口，作为 Vulkan Surface 的宿主。

## Vulkan 文件清单

| 文件 | 当前职责 | 后续建议 |
|---|---|---|
| `VulkanViewport.axaml` | Vulkan Host 外层和失败 fallback UI | 保留为中央视口唯一入口 |
| `VulkanViewport.axaml.cs` | 控制 fallback 显示/隐藏 | 保持轻量，不放渲染逻辑 |
| `VulkanNativeHost.cs` | Avalonia `NativeControlHost` 生命周期桥接 | 只处理创建、尺寸、销毁和低频日志 |
| `Win32ViewportHost.cs` | Win32 子窗口创建、Resize、Destroy | 保持平台隔离，后续 Windows 专属逻辑放这里 |
| `VulkanClearSession.cs` | Vulkan Instance、Surface、初始化主流程 | RZ-Fix3-A 收口为 Clear Probe 会话 |
| `VulkanClearSession.Device.cs` | PhysicalDevice、Queue、Device | 保持小文件拆分 |
| `VulkanClearSession.Swapchain.cs` | Swapchain 创建、重建、释放 | 只记录重建结果，不记录连续 Resize 噪声 |
| `VulkanClearSession.Dispose.cs` | Vulkan 资源释放 | 保持释放顺序明确 |

## 生命周期流程

```text
UiRoot 中央列
  -> Main
    -> VulkanViewport
      -> VulkanNativeHost.CreateNativeControlCore(parent)
        -> Win32ViewportHost.CreateChild(parent.Handle)
        -> VulkanClearSession.TryCreate(hwnd, width, height, log)
          -> CreateInstance
          -> CreateSurface
          -> PickDevice
          -> CreateDevice
          -> CreateSwapchain
          -> 成功：隐藏 fallback
          -> 失败：显示 fallback，并记录低频日志

尺寸变化：
VulkanNativeHost.OnSizeChanged
  -> Win32ViewportHost.Resize
  -> VulkanClearSession.Resize
    -> 跳过 0 尺寸和重复尺寸
    -> DestroySwapchain
    -> CreateSwapchain
    -> 只记录成功/失败摘要

销毁：
VulkanNativeHost.DestroyNativeControlCore
  -> VulkanClearSession.Dispose
  -> Win32ViewportHost.Destroy
  -> 记录 Vulkan 释放完成
```

## 低频日志接入点

允许记录：

- Vulkan 初始化开始
- Vulkan 初始化成功
- Vulkan 初始化失败
- Swapchain 创建或重建成功
- Swapchain 创建或重建失败
- Vulkan 释放完成

禁止记录：

- 每帧 RenderFrame
- 每次 Acquire
- 每次 Present
- 鼠标移动
- 连续 Resize 明细
- Picking hover
- Gizmo hover/drag preview

当前日志入口为 `UiVm.LogVulkanLifecycle`，分类为 `Render / Backend`。这个入口可以继续用于生命周期摘要，但不应变成逐帧日志通道。

## Fallback UI

当前 `VulkanViewport` 已有 `FallbackLayer`，初始化失败时可以显示占位提示。

RZ-Fix3-A 建议统一失败文案：

```text
Vulkan 初始化失败
当前使用占位视口
请查看底部日志详情
```

fallback 要求：

- Vulkan 初始化失败不能导致编辑器崩溃。
- 中央视口不能白屏。
- 底部日志只记录低频失败摘要和异常详情。
- 失败后不影响顶部、左侧、右侧、底部 UI。

## 风险点

1. 当前代码已经越过“只审计”边界，实际存在 Vulkan Clear Probe 预接入。
2. `Main.axaml` 已直接使用 `VulkanViewport`，如果 Vulkan 环境缺失，会依赖 fallback 是否可靠。
3. Win32 子窗口注册类目前每次创建都会调用 `RegisterClassW`，需要确认重复注册失败是否被安全忽略。
4. Swapchain Resize 只做立即重建，后续如果接入渲染循环，需要处理窗口最小化、尺寸抖动和 device idle 成本。
5. 当前没有逐帧 Clear + Present；已有代码更准确地说是 Surface/Swapchain Probe，不是完整 Clear Renderer。
6. 中文文案在当前终端输出中呈乱码，需要确认源文件编码和编辑器显示是否一致。
7. Git 当前本地分支名仍是 `fix/RZ-Fix1-editor-access-violation`，但跟踪远端为 `origin/fix/RZ-Fix2-ui-baseline`，命名与当前基线不完全一致。

## RZ-Fix3-A 最小 Clear Probe 计划

RZ-Fix3-A 只做以下内容：

1. 保持 Vulkan 仅位于 `Viewport/Vulkan`。
2. 保持中央视口为唯一宿主，不改 Top、Left、Right、Foot。
3. 明确当前 Probe 名称：如果没有实际 Clear + Present，应命名为 Surface/Swapchain Probe；如果进入 Clear Probe，必须实现最小帧提交。
4. 增加最小命令池、命令缓冲、image view、render pass 或 dynamic rendering 所需资源。
5. 只清屏一种固定颜色，不接模型、相机、Gizmo、Picking、材质、资源系统。
6. 渲染循环不得写每帧日志。
7. Resize 只记录重建成功/失败摘要。
8. 初始化失败继续显示 fallback。
9. 所有 `.cs` / `.axaml` 文件继续保持不超过 100 行。
10. 验收必须包含 `dotnet restore`、`dotnet build`，并要求 0 warning / 0 error。

## 验收记录

- `dotnet restore XuanYu.Editor.UI/XuanYu.Editor.UI.csproj`：通过。
- `dotnet build XuanYu.Editor.UI/XuanYu.Editor.UI.csproj --no-restore`：通过，0 warning / 0 error。
- `.cs` / `.axaml` 文件行数：未发现超过 100 行的文件。
