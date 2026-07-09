# XuanYuEngine 项目长期记忆（MEMORY.md）

## 项目身份
- 玄域引擎 / XuanYu Engine：C#/.NET 3D 策略战术引擎（RTS/RTT/GSG），曾用代号 FluidWarfare（2026-06-24 更名）。
- 实际命名空间：`XuanYu.Core` / `XuanYu.Editor.UI` / `XuanYu.Editor.Win` / `XuanYu.Render.Abstractions`（VK3-A 契约层）/ `XuanYu.Render.Vulkan`。
- ⚠️ 文档命名空间混乱：NAMING_RULES 写 `FluidWarfare.*`、naming 文档写 `XuanYu.Engine.*`，实际是 `XuanYu.*`。以实际代码为准。

## 架构分层（依赖向下）
1. XuanYu.Core（平台无关，最底层）
2. XuanYu.Render.Abstractions（UI↔Vulkan 纯契约：HWND/尺寸/Attach/Detach，不引用 Silk.NET）
3. XuanYu.Editor.UI（Avalonia 12.0.4 跨平台 UI；因历史债仍保留对 Render.Vulkan 的 ProjectReference 与 `using Silk.NET.Vulkan`）
4. XuanYu.Editor.Win（WinForms 宿主 / 原生 HWND 桥接，组合根）
5. XuanYu.Render.Vulkan（Vulkan 后端）

## 编码宪法核心
- 文件 ≤100 行（复杂 ≤150）；目录 ≤5-7 文件；单一职责。中文写人话、英文写机器标识符。
- 平台隔离；Vulkan 返回值必检；生命周期需测试。

## Vulkan 接入状态（速查）
- VK3 / VK4-A / VK4-B / VK4-C 均已收口（真机验证）。黑屏预期已在 VK4-D 解除。
- **VK4-D 进行中**：最小 Clear+Present 单色清屏闭环已实装（D1 RenderPass/CommandPool/Framebuffer；D2 静态 clear CommandBuffer[]；D3 后台 Present 泵 Semaphore/Fence+Acquire→Submit→Present）。薄组合根 `VulkanRenderSession` + Bridge `VulkanBridgeRenderSessionAttachStep`。
- **当前阶段 VK4-D-R3（Render.Vulkan 侧，进行中）**：修 ①半屏蓝灰 ②QueuePresent 反复 ErrorOutOfDateKhr ③Resize 时序竞争。已改：OutOfDate 优雅降级（低频记 OutOfDatePaused() 后 break）；Resize 日志顺序（Rebuilt 在 Start 前，用 `_swapchainOwner.Extent` 物理像素）；能力/创建日志打印物理像素 extent。
- **下一阶段 VIEWPORT-RESIZE-R1（Editor.UI 侧）**：日志详情栏展开/收起后 NativeHost 视口尺寸同步慢半拍 → 先加中文 probe 确认时序，再布局稳定后主动同步最终尺寸；拖动仍走 Coalescer。
- 红线：VK4-D 只做最小清屏，不进场景渲染/相机/网格/材质/Gizmo/UI 叠加；不碰日志 UX；Bridge 不内联 VK4-D 细节；全 .cs ≤100。

## VK4 行数/职责红线（临界）
- `VulkanNativeHostSurfaceBridge.cs` ~83 行（仅委托 RenderSession，不二次重建 Swapchain）。
- `VulkanDeviceOwner.cs` ~99 行（只 CreateDevice/GetQueue/DisposeDevice）。
- `VulkanClearFrameOwner.cs` 93 / `VulkanPresentLoop.cs` ~96 / `VulkanRenderSession.cs` ~58 / `VulkanBridgeRenderSessionAttachStep.cs` 15 / `VulkanClearFrameLogFormatter.cs` ~15（均 ≤100）。
- 命名：`Silk.NET.Vulkan.Device` 用 `VulkanDevice` 别名；owner=`VulkanDeviceOwner`；属性=`LogicalDevice`；禁 `Device` 作属性名。

## 横切约束
- Preview/Commit 分离：高频拖拽只改预览；Commit 才写 WorldState。
- 诊断安全：非阻塞 `DiagnosticSink.TryWrite`；高频路径禁 UI 阻塞。

## 构建入口
- `run.bat`：`dotnet restore` → `dotnet build --no-restore` → `dotnet run` XuanYu.Editor.UI。NuGet 源 `NuGet.Config`。
- 低内存构建（沙箱直 build 会 OOM）：`MSBUILDDISABLENODEREUSE=1 dotnet build <csproj> --configfile NuGet.Config -nologo -maxCpuCount:1 -p:UseSharedCompilation=false --no-incremental`。
- 残留编辑器进程锁 DLL 报 MSB3027 → `Stop-Process -Id <pid> -Force` 强杀后重构建。

## 工作流约定（用户明确要求）
- 每轮阶段完成：更新 `changelog.md` + `file-tree.md`（二者必须进仓库）；验收后 push 当前分支到 origin 并附 commit hash。
- 不写 token/密码/密钥进仓库；Git 鉴权细节不进仓库。
- **每次 commit 后必须 push 到 origin；每次 push/交付都要给 commit hash。**

## 技术陷阱（Silk.NET.Vulkan 2.22.0 已实测）
- `KhrSwapchain`/`KhrSurface` 方法名**无 KHR 后缀**（`AcquireNextImage`/`QueuePresent`/`CreateSwapchain`/`GetSwapchainImages`/`GetPhysicalDeviceSurfaceCapabilities`/`...Formats`/`...PresentModes`）；枚举用非弃用短名（`ImageUsageFlags.ColorAttachmentBit`/`ColorSpaceKHR.SpaceSrgbNonlinearKhr`/`PresentModeKHR.MailboxKhr`/`FifoKhr`/`CompositeAlphaFlagsKHR.OpaqueBitKhr`/`ImageAspectFlags.ColorBit`）；`Vk.TryGetDeviceExtension<T>` 需 4 参 `(Instance, Device, out T, string?)`。
- **`Result.SuboptimalKhr` 是成功码**（正值），`ErrorOutOfDateKhr` 才是错误 → `res != Success && res != SuboptimalKhr` 判失败；OutOfDate 应优雅降级（记一次后 break 等 Resize 重建）而非刷屏。
- **Swapchain 重建必须传 `OldSwapchain`**：顺序「用旧句柄建新 → 成功后再 Destroy 旧 ImageView + 旧 Swapchain（先 ImageView 后 Swapchain）」，否则 Windows 报 `VK_ERROR_NATIVE_WINDOW_IN_USE_KHR`。
- **Win32 `SurfaceCapabilitiesKHR.CurrentExtent` 是物理像素**（DPI 1.75 时 1248x961 = 713x549 × 1.75）；`ChooseExtent` 在其有效时直接返回它，忽略传入逻辑尺寸。Swapchain/Framebuffer/RenderArea extent 三者同源物理像素。
- **VK_KHR_swapchain 设备扩展**：LogicalDevice 创建须 `EnabledExtensionCount`+`PpEnabledExtensionNames` 启用，否则 `CreateSwapchainKHR` 运行时失败。
- **Vulkan 控制台日志单出口**：统一经 `VulkanBridgeLogFormatter.Emit` 内唯一 `Console.WriteLine`；低层 `Log(log,m)` 与 Bridge AttachStep 只 `log?.Invoke(m)`，**禁各自 `Console.WriteLine`**。`VulkanInstanceOwner`/`VulkanSurfaceOwner` 仅直接 `Console.WriteLine`（已单现不重复）。

## 技术陷阱（Avalonia / 工具）
- **后台线程日志回调须回 UI 线程（VK4-D-R2 事故，退出码 -532462766）**：`Render.Vulkan` 后台线程经 `Action<string> log` 回调若访问 Avalonia `DataContext`/`UiVm`/`ObservableCollection` → `InvalidOperationException` 闪退。消费方须 `Dispatcher.UIThread.CheckAccess()` 判断，非 UI 线程 `Dispatcher.UIThread.Post(...)` 切回；`Render.Vulkan` 只持有 `Action<string> log`、禁引用 Avalonia；`VulkanPresentLoop.Log` 须 `try/catch` 吞异常。
- **UI 线程高频路径禁视觉树遍历**：日志自动滚动拆为独立 `Foot/LogListAutoScrollController.cs`（LOG-UX-2 已落地，四机制：单次解析/节流/防重入/零遍历），禁硬写 `Foot.axaml.cs`；用 `Dispatcher.InvokeAsync(cb, Render)` 而非 `LayoutUpdated`，订阅 `ListBox.TemplateApplied` + 延迟重试拿到 ScrollViewer。
- **Avalonia 12.0.4**：`SelectionMode` 无 `Extended` 成员，旧语义由 `Multiple` 提供（含 Shift 范围+Ctrl 切换）；XAML 须 `xmlns:av="using:Avalonia.Controls"` + `{x:Static av:SelectionMode.Multiple}`。
- **WinExe 控制台输出**：`<OutputType>WinExe</OutputType>` 进程不继承父控制台 → `Program.cs Main()` 首行 `[DllImport("kernel32")] AttachConsole(-1)` 继承父终端。
- `git diff` 对 `.axaml` 报「LF→CRLF」仅 autocrlf 噪声；PowerShell `Reflection.Assembly.LoadFrom` 与 Bash 内嵌 PowerShell 被沙箱拦截，查包成员用包内 `.xml` 文档。
- **生产不注入种子/示例日志**：`UiVm.Logging.InitLogs` 禁 `_logBuffer.Seed(SampleLogEntries.All)` 或过期文案，空状态用「暂无日志」占位。
