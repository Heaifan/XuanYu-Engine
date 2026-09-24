# XYE-WAVE2-A-A1.5-FIX1：Vulkan GPU → Avalonia Composition 重测

## 任务结论

**PASS（Spike 技术门）**。本机已证明 Vulkan device-local external image 经 Avalonia 12.1.3 Vulkan backend、`ImportImage`、`CompositionDrawingSurface.UpdateWithSemaphoresAsync`、`CompositionSurfaceVisual` 后可在同一 Avalonia Window 中显示，并由普通 Avalonia overlay 覆盖和接收输入。

`UpdateAsync` 在 12.1.3 Vulkan 路径明确抛出 `NotSupportedException`；本重测改用该版本官方 Vulkan interop 所需的 semaphore 版本 `UpdateWithSemaphoresAsync`，没有退回 CPU 或 native window 路线。

## Git 与边界

- Branch：`spike/viewport-vulkan-avalonia-gpu-composition-fix1`
- Start HEAD：`88c5a5808d1f5560f29e3814fad1928f1ee89443`
- Base：`feat/wave-1-unified-integration` 的冻结集成 HEAD
- 仅修改 Composition Spike、Spike tests 和本报告；未修改生产 `VulkanViewport`、`VulkanNativeHost`、`Win32ViewportHost` 或 map/domain 功能。

## 生产路径审计

生产 `VulkanViewport.axaml` 包含 `VulkanNativeHost`；后者继承 Avalonia `NativeControlHost`，通过 `Win32ViewportHost.CreateChild` 创建带 `WS_CHILD` 的 native child HWND。生产 Vulkan Surface/Swapchain 绑定该 HWND。因此生产路径是 Avalonia Visual → NativeControlHost → child HWND → Vulkan Surface/Swapchain，不是 Composition image 路径。本 Spike 未复用或修改这条生产链。

## 实验环境与技术路径

- Avalonia：Spike-only `12.1.3`；生产版本未升级。
- Backend：显式 `Win32RenderingMode.Vulkan`；启动日志为 `RenderingSubsystem=Skia`，同时 Composition GPU interop 和 Vulkan external handle 能力已由 Vulkan 路径建立并验证。
- GPU：`NVIDIA GeForce RTX 3060`
- Driver：`2480242688`
- Vulkan API：`4211013`
- 技术路径：Vulkan device-local RGBA image → `VK_KHR_external_memory_win32` NT handle → `ICompositionGpuInterop.ImportImage` → `CompositionDrawingSurface.UpdateWithSemaphoresAsync` → `CompositionSurfaceVisual`。
- 同步：两个 `VK_KHR_external_semaphore_win32` NT handle；GPU clear 后 signal，Avalonia 等待并 signal 返回。
- 图像 layout：显式 `TransferSrcOptimal`。
- 初始 0×0 resource 被禁止创建；有效尺寸为 `928×539`，Resize 后为 `1072×640`。

## GPU Interop 日志

```text
[A1.5] Avalonia=12.1.3.0; RenderingSubsystem=Skia
[A1.5] CompositionGpuInterop=Avalonia.Rendering.Composition.CompositionInterop
[A1.5] SupportedImageHandleTypes=VulkanOpaqueNtHandle,VulkanOpaqueKmtHandle
[A1.5] SupportedSemaphoreTypes=VulkanOpaqueNtHandle,VulkanOpaqueKmtHandle
[A1.5] VulkanOpaqueNtHandle=YES
[A1.5] PhysicalDevice=NVIDIA GeForce RTX 3060; Driver=2480242688 / Vulkan 4211013; QueueFamily=0
[A1.5] ExternalMemory=VulkanOpaqueNtHandle; MemorySize=2375680; ImageSize=928, 539; ImageLayout=TransferSrcOptimal; Sync=VulkanOpaqueNtHandle
[A1.5] GPU Composition Resize 完成=1072, 640
```

## 五道门

- G1：**PASS**。Spike 显式选择 `Win32RenderingMode.Vulkan`，Vulkan interop 可用，且日志确认 Vulkan image/semaphore 能力。
- G2：**PASS**。`SupportedImageHandleTypes` 含 `VulkanOpaqueNtHandle`；semaphore types 同样含该类型。
- G3：**PASS**。真实 GPU clear image 经 `ImportImage` 和 Composition surface update 显示；截图中 viewport 背景为 Vulkan clear 色，不是 CPU bitmap。
- G4：**PASS**。同一 Avalonia Window 的 Border、TextBlock、Button 真实覆盖 Vulkan 输出；日志为 `Hover=原生 Avalonia Button`、`Click=原生 Avalonia Button；次数=1`。
- G5：**PASS**。窗口改为 `1120×760` 后 Composition bounds 为 `1072×640`，重建 image、重新 Import/Update；日志与 Resize 截图均显示 GPU 输出和 overlay 仍在。

## 原生窗口与覆盖层审查

- Spike Native child HWND：**没有**。
- `NativeControlHost` / `WS_CHILD`：**没有**。
- Extra `PopupRoot` / Native Popup / 第二独立 UI Window：**没有**。
- Overlay Hover/Click：**普通 Avalonia Button 原生事件**，不是 native popup 或桌面 topmost workaround。

## 人工/运行证据

- 初始截图：[a15-fix1-runtime.png](C:/Users/Heai/.codex/visualizations/2026/09/24/a15-fix1-runtime.png)
- Resize 截图：[a15-fix1-resize.png](C:/Users/Heai/.codex/visualizations/2026/09/24/a15-fix1-resize.png)
- Hover：鼠标移入 Button 后日志确认 `Hover=原生 Avalonia Button`。
- Click：点击后日志确认 `Click=原生 Avalonia Button；次数=1`。
- Resize：`928×539` → `1072×640`，GPU clear 色、overlay、Button 均保留。

## 自动验证与裁决

- Spike Build：PASS，0 Warning / 0 Error。
- Spike Tests：PASS，3/3。
- ARCH-A / ARCH-VIEWPORT-R1：PASS。
- 5+100：PASS，所有检查范围内 `.cs/.axaml/.js` ≤100 行。
- `git diff --check`：PASS。

技术结果与自动门禁均达到 A1.5 PASS 条件。WAVE-2 Viewport Production Migration 可进入独立评审，但本任务本身不启动生产迁移。
