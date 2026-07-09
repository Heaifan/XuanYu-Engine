# XuanYuEngine 项目长期记忆（MEMORY.md）

## 项目身份
- 玄域引擎 / XuanYu Engine：C#/.NET 3D 策略/战术引擎（RTS/RTT/GSG），曾用代号 FluidWarfare（2026-06-24 更名）。
- 实际命名空间：`XuanYu.Core` / `XuanYu.Editor.UI` / `XuanYu.Editor.Win` / `XuanYu.Render.Abstractions`（VK3-A 契约层）/ `XuanYu.Render.Vulkan`。
- ⚠️ 文档命名空间混乱：NAMING_RULES/AI_DEVELOPMENT_RULES 写 `FluidWarfare.*`；naming-XuanYu-Engine.md 目标 `XuanYu.Engine.*`；实际是 `XuanYu.*`。以实际代码为准。

## 架构分层（依赖向下）
1. XuanYu.Core（平台无关，最底层）
2. XuanYu.Render.Abstractions（UI↔Vulkan 纯契约层：HWND/尺寸/Attach/Detach，不引用 Silk.NET）
3. XuanYu.Editor.UI（Avalonia 12.0.4 跨平台 UI；仍因历史探针债保留对 Render.Vulkan 的 ProjectReference 与 `using Silk.NET.Vulkan`，未完全解耦）
4. XuanYu.Editor.Win（WinForms 宿主 / 原生 HWND 桥接，组合根）
5. XuanYu.Render.Vulkan（Vulkan 后端）

## 编码宪法核心
- 文件 ≤100 行（复杂 ≤150）；目录 ≤5-7 文件；单一职责。
- 中文写人话、英文写机器标识符。
- 平台隔离；Vulkan 返回值必检；生命周期需测试。

## Vulkan 接入里程碑（速查）
- VK3 全链路已收口：NativeHost HWND 生命周期接入 Vulkan Instance + Surface（VK3-A/B1/B2/C1/C2/C2-R1）。黑屏为预期（无 Swapchain/渲染）。
- VK4-A 已收口：PhysicalDevice 选择链路（仅枚举+选择+中文日志，未创设备）。
- VK4-B（含 R1）已完全收口：基于 VK4-A 选择结果创建 LogicalDevice + Graphics/Present 队列；Detach 顺序 `LogicalDevice → Surface → Instance`。
- **VK4-C 二次 R1 已验 5/8，剩 T5/T6 待补，未完全收口；VK4-D 暂缓**：Swapchain + Images + ImageViews 生命周期（独立 owner/attach step）。二次 R1（2026-07-09 用户真机）Resize 重建已通过（713x549/736x188/412x188/318x188 等均成功，无 ErrorNativeWindowInUseKhr），证明 OldSwapchain 修复生效。⚠️ T6 Detach 顺序日志**已存在于代码**（`VulkanSwapchainOwner.Dispose`→`Swapchain 释放成功`，内部 `DestroyImagesAndViews` 先 ImageView 后 Swapchain；`VulkanDeviceOwner`→`LogicalDevice 释放成功`；`VulkanBridgeLogFormatter`→`Surface 已释放`/`Instance 已销毁`/`分离完成`）—— **不需另开 Vulkan 改动轮**，补 LOG-UX-1-R2 自动滚动后重跑即可见。T5 0尺寸未触发（不阻塞主链路）。红线：未建 RenderPass/Framebuffer/CommandPool/CommandBuffer、未 Clear/Present（仍黑屏）。
- **VK4-C-Fix（已实装）**：① `VulkanDeviceOwner.Create` 启用 `VK_KHR_swapchain` 设备扩展（扩展名由 `VulkanSwapchainOwner.DeviceExtensionName` 传入，DeviceOwner 不硬编码 swapchain 知识）；② `VulkanSwapchainOwner.Recreate` 0 尺寸跳过；③ 暴露 `Format`/`Extent`/`ImageViews` 只读供 VK4-D。

## VK4 审计订正（速查，均非阻塞）
- **GPU 型号非硬规则**：以 VK4-A 最终选择结果为准，禁止硬编码 RTX 3050/3060 等。
- **日志通道区分**：旧探针 `【Vulkan探针】`（历史债，不参与正式链路）；正式 `【VulkanBridge】`/`【VulkanDevice】`。审计须显式区分，勿把旧探针当证据。
- **候选设备含杂质**：枚举混有 D3D12 wrapper / Basic Render Driver / iGPU；VK4-B 创建 LogicalDevice 必须复用 VK4-A 选择结果，不得重枚举选错设备。
- **VK_KHR_swapchain 设备扩展是 Swapchain 运行时命门**：仅拿 `KhrSwapchain` 函数不够，须 `LogicalDevice` 创建时通过 `DeviceCreateInfo.EnabledExtensionCount`+`PpEnabledExtensionNames` 启用，否则 `CreateSwapchainKHR` 运行时失败。
- **Resize 红线**：VK4-B 起 Resize 只接收尺寸不重建 Surface/Device/Queue；VK4-C Resize 只重建 Swapchain+ImageViews。
- **黑屏为预期**直到 VK4-D 出画面。

## VK4 行数/职责红线（当前临界）
- `VulkanNativeHostSurfaceBridge.cs` 98 行（压 100 红线，不得再增，仅编排）。
- `VulkanDeviceOwner.cs` 99 行（仅 `CreateDevice/GetQueue/DisposeDevice`，禁止顺手塞 Swapchain/CommandPool/RenderPass）。
- 命名口径：`Silk.NET.Vulkan.Device` 一律用 `VulkanDevice` 别名；业务 owner=`VulkanDeviceOwner`；属性=`LogicalDevice`；禁止用 `Device` 作属性名。

## 横切约束
- Preview/Commit 分离：高频拖拽只改预览；Commit 才写 WorldState。
- 诊断安全：非阻塞 `DiagnosticSink.TryWrite`；高频路径禁止 UI 阻塞。

## 构建入口
- `run.bat`：`dotnet restore` → `dotnet build --no-restore` → `dotnet run` XuanYu.Editor.UI。NuGet 源 `NuGet.Config`。
- 低内存构建（沙箱直 build 会 OOM）：`MSBUILDDISABLENODEREUSE=1 dotnet build <csproj> --configfile NuGet.Config -nologo -maxCpuCount:1 -p:UseSharedCompilation=false --no-incremental`。

## 工作流约定（用户明确要求）
- 每轮阶段完成：更新 `changelog.md` + `file-tree.md`（二者必须进仓库，跨电脑/跨 AI 接手）；验收后 push 当前分支到 origin 并附 commit hash。
- 不得把 token/密码/密钥/`.git-credentials` 内容写入仓库；Git 鉴权细节不要进仓库。
- **每次更新（commit）后必须 push 到 origin**；**每次 push/交付都要给 commit hash**（用户原话「记得给hash」）。
- 换电脑后需在新机器重新 GitHub 登录/鉴权。

## 技术陷阱（Avalonia / 工具）
- **Avalonia 12.0.4**：`SelectionMode` 枚举**无 `Extended` 成员**；旧 Extended 语义由 `SelectionMode.Multiple` 提供（含 Shift 范围+Ctrl 切换）。XAML 须 `xmlns:av="using:Avalonia.Controls"` + `SelectionMode="{x:Static av:SelectionMode.Multiple}"`。注意 `Multiple` 普通单击是切换该行。
- PowerShell `Reflection.Assembly.LoadFrom` 与 Bash 内嵌 PowerShell 被沙箱安全策略拦截；查 NuGet 包成员改用包内 `.xml` 文档（`grep -oE "F:完整类型名\.成员"`）。
- `git diff` 对 `.axaml` 报「LF 将被替换为 CRLF」仅 autocrlf 噪声，非错误。
- **Silk.NET.Vulkan 2.22.0 真实 API 名（已实测）**：`KhrSurface` 方法无 `KHR` 后缀（`GetPhysicalDeviceSurfaceCapabilities`/`...Formats`/`...PresentModes`）；枚举用非弃用短名 `ImageUsageFlags.ColorAttachmentBit`/`ColorSpaceKHR.SpaceSrgbNonlinearKhr`/`PresentModeKHR.MailboxKhr`/`FifoKhr`/`CompositeAlphaFlagsKHR.OpaqueBitKhr`/`ImageAspectFlags.ColorBit`；`Vk.TryGetDeviceExtension<T>` 需 4 参 `(Instance, Device, out T, string?)`。拿不准用临时反射控制台工程（引 Silk.NET.Vulkan + Extensions.KHR 2.22.0）打印真实名，勿反复试错编译。
- **Swapchain 重建必须传 `OldSwapchain`**：Resize 重建 Swapchain 时，`SwapchainCreateInfoKHR.OldSwapchain` 须设为当前 Swapchain 句柄，否则 Windows 上 `vkCreateSwapchainKHR` 返回 `VK_ERROR_NATIVE_WINDOW_IN_USE_KHR`（窗口被旧 Swapchain 占用）。顺序：用旧句柄建新 Swapchain → 成功后再 Destroy 旧 ImageView + 旧 Swapchain（先 ImageView 后 Swapchain）。
- **Vulkan 控制台日志单出口**：所有 Vulkan 生命周期日志统一经 `VulkanBridgeLogFormatter.Emit` 内的唯一 `Console.WriteLine` 出口；低层 `Log(log, m)` 辅助与 Bridge AttachStep 只 `log?.Invoke(m)`（最终走到 Emit），**禁止再各自 `Console.WriteLine`**，否则经 `Emit` 二次输出（终端每条日志出现两遍）。`VulkanInstanceOwner`/`VulkanSurfaceOwner` 仅直接 `Console.WriteLine`（不走 Emit），终端已单现，不重复。
- **Avalonia ListBox 内部 ScrollViewer 解析时机**：在 `AttachedToVisualTree`/`DataContextChanged` 时 ListBox 模板可能尚未应用，`FindDescendantOfType<ScrollViewer>()` 返回 null 且若不再重试则永远为 null → 自动滚动死。正确做法：订阅 `ListBox.TemplateApplied` 事件 + `Dispatcher.InvokeAsync(Resolve, Loaded)` 延迟重试，确保拿到 ScrollViewer 后再挂 `ScrollChanged` 并 `ScrollToEnd()`；新项后用 `Dispatcher.InvokeAsync(ScrollToEnd, Render)` 确保布局完成后滚。
- **生产不注入种子/示例日志**：`UiVm.Logging.InitLogs` 不得 `_logBuffer.Seed(SampleLogEntries.All)` 或注入过期文案（如「Device/Swapchain 尚未接入」）污染真实运行日志；空状态用 UI「暂无日志」占位。
- **WinExe 控制台输出**：`<OutputType>WinExe</OutputType>` 的 .NET 进程在 Windows 上不继承父控制台，`Console.WriteLine` 写入虚空。修复：`Program.cs Main()` 首行调用 `[DllImport("kernel32")] AttachConsole(-1)`（ATTACH_PARENT_PROCESS）继承父终端；Vulkan 代码已有的 6 处 Console.WriteLine 全部生效，关闭编辑器后终端仍显示 Detach 释放序列。
- **Avalonia 自动滚动可靠方案**：ListBox/ScrollViewer 新日志后自动滚到底，须用 `Dispatcher.InvokeAsync(callback, DispatcherPriority.Render)` 而非 `LayoutUpdated` 事件——后者触发时序与 PropertyChanged 不可靠（虚拟化/延迟布局导致事件可能在 _pendingScroll 设置前已触发过）。
- **⚠️ UI 线程高频路径禁止视觉树遍历**：Vulkan `Attach` 在 `OnAttachedToVisualTree` 内 UI 线程同步执行（~25 条日志），若 `OnPropertyChanged(LogItems)` 每条都调 `FindDescendantOfType<T>()` 遍历视觉树 → UI 线程阻塞 + Dispatcher 堆积 → Windows 判定未响应 → 退出码 0xCFFFFFFF 崩溃（R4 实际事故，R5 修、R5A 彻底移除）。规则：列表自动滚动**禁止硬写在 `Foot.axaml.cs`**——该文件承载日志列表/多选/Ctrl+C/详情选中已接近 100 行红线；应拆为独立 `Foot/LogListAutoScrollController.cs`（LOG-UX-2），由 `Foot.axaml.cs` 仅创建 controller + 交 ListBox + 通知新日志；controller 内部做节流（已安排滚动则不重复安排，等布局完成只滚一次），避免 Dispatcher/ScrollChanged 套娃。
