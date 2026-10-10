# XYE-R2 WP3-C：Windows Native HWND Input P3 IPO

状态：`P3 PARTIAL / NOT PASS` / `Native input NOT RUN` / `P4 PENDING`
基线：`recovery/editor-center-navigation-r1` @ `4cecf8c8ef1edd91be583362ac77c4f5c2ca4d8b`（启动审计时 Parent Bootstrap 报告 clean；本文件不代表后续工作区仍 clean）
Owner：Parent 负责独占 Build/Test/Windows App/Win32/Vulkan Runtime；WP3-C 负责本 IPO 与只读接线审计。
范围：仅验证 Native HWND 上真实鼠标输入进入现有生产 Router，并到达 Map/Region/Road/Marker 的产品动作和终止清理。本文不授权改生产代码或增加 Harness。

## 结论与证据边界

静态源码显示一条预期的生产路径：

```text
真实 Win32 HWND
→ Win32ViewportHost RouteWndProc / NativePointerMessage
→ VulkanNativeHost.OnNativePointerMessage
→ NativeViewportInputForwarder
→ NativePointerEventAdapter
→ UiVm.ViewportInput.Sink
→ 唯一 ViewportInputComposition / ViewportInputRouter
→ Capture Coordinator + Region/Road/Marker Consumer
→ UiVmMapBackend
→ UiVm 绘制/放置入口与 GroundPick
```

相关实现位置：`XuanYu.Editor.UI/Viewport/Vulkan/Win32ViewportHost.Input.cs`、`VulkanNativeHost.cs`、`VulkanNativeHost.Pointer.cs`、`VulkanNativeHost.InputLifecycle.cs`、`XuanYu.Editor.UI/Viewport/Input/NativeViewportInputForwarder.cs`、`NativePointerEventAdapter.cs`、`XuanYu.Editor.UI/Input/UiVmViewportInputComposition.cs`、`XuanYu.Editor/Input/ViewportInputComposition.cs`、`ViewportInputCaptureCoordinator.cs`、`XuanYu.Editor/Input/Map/{Region,Road,Marker}InputConsumer.cs`、`XuanYu.Editor.UI/Input/UiVmMapBackend.cs`。

以上只是源代码接线证据，不证明真实 HWND 收到事件、Adapter 收到的字段正确、Router 实际裁决该事件、真实 Capture 生命周期正确、GroundPick 命中、Consumer 成功提交，或 Vulkan/窗口运行成功。未启动 App；真实 P3 当前为 `NOT RUN`，产品根因 `UNKNOWN`。

已有 `CAuditR1FlowAHeadlessSyntheticInput`、`HeadlessPointerInputCutoverTests`、`NativePointerEventAdapterTests`、`NativeSourceParityTests` 等证据属于 Synthetic / Unit / Headless / Contract 范围。它们可以检查字段转换、事件合约或测试构造的 Forwarder，不是 OS 产生的 HWND 消息，不得记作 Native P3。

## 可执行 IPO

执行条件：Parent 明确分配独占 Windows App + Win32 + Vulkan 时段，并确认要运行的构建产物。执行前经仓库入口核对实际 Git 身份和 Dirty；使用 `scripts/xye-bootstrap.ps1`、`scripts/resolve-dotnet.ps1` 与 `scripts/xye-dotnet.ps1`。运行与 Build/Test 不并发。不得以已有 Headless/Harness PASS 替代下列步骤。

| # | 路径 | 输入 I | 过程 P | 输出 O / 通过观察点 |
|---|---|---|---|---|
| 1 | Windows App / 新场景 | 按批准流程启动实际 App；创建 New Scene，添加合法 Layer；记录场景和地图身份。 | 确认可见 Native Viewport 对应真实非零 HWND；确认窗口进程、构建产物和 Vulkan backend。 | 保存运行身份、窗口/HWND 与设备信息。无真实 HWND 或未成功创建即 `BLOCKED`，不得用 `_hwnd=0` Headless 顶替。 |
| 2 | Region 单击与结束 | 选择 Region 绘制；在地图上选择三个可命中的位置，依次真实 Move/Down/Up；按产品流程完成绘制。 | 逐跳关联原生消息、适配事件、Router owner/capture、Surface/GroundPick、Region draft 和 Commit。确认同一手势只有一个 Owner。 | Region 点位/最终实体与预期一致；提交前后地图身份、Undo 记录和状态可追踪。无误命中或静默丢失。 |
| 3 | Road 多节点 | 选择 Road 工具；在可命中位置逐点点击形成多节点，按产品流程完成。 | 逐跳记录每个 Down/Up、Router owner、Surface 命中、节点数、完成动作及终止清理。 | 道路节点顺序与点击一致；最终持久实体可通过 Undo/Redo 验证。 |
| 4 | Marker 放置 | 选择 Marker 工具，在一个可命中位置真实点击。 | 逐跳记录 HWND→Adapter→Router→Marker Consumer→GroundPick/放置结果；检查是否发生额外提交或误消费。 | Marker 仅在预期位置产生一次；撤销/重做结果一致。 |
| 5 | 终止与 Capture | 分别在 Region/Road 活跃过程触发 Esc、Tool Change、Mode Change、焦点丢失、CancelMode；另测试拖动到窗口外再释放。 | 同时观察 Router gesture Owner/epoch、OS `GetCapture()`、`WM_CAPTURECHANGED`、业务 Draft/Transaction、地图持久选择和 transient 状态。检查取消是否幂等。 | 终止后 Owner 与 OS Capture 均按预期释放；未提交 Draft 按既定产品策略处理；持久实体和选择不得被误删。Tool/Mode 新状态生效。 |
| 6 | DPI 与边界复查 | 在支持的 DPI 缩放设置下重做至少一个 Region 命中；再测试视口边缘、非地图区域和窗口外释放。 | 记录物理像素、客户区坐标、DPI scale、Adapter 逻辑坐标及实际命中点。 | 坐标映射一致；无越界错误命中；若环境不支持 DPI 切换，标 `BLOCKED/NOT RUN` 并记环境。 |

每一项都记录原始事件与对应处理阶段；只有最终实体正确不足以声称逐跳完整。无法观察某一跳时，标记 `UNKNOWN` 并保留缺证，不得根据下游结果推断该跳一定正确。

### 建议的阶段探针记录字段

探针只在获 Parent 授权后使用；优先使用现有无侵入诊断能力，禁止未经审查新增常驻热路径日志或改生产文件。对同一事件使用关联 ID 或可审计的时间顺序，字段至少包含：

- 原生来源：进程/窗口标识、真实 HWND（脱敏/仅本机证据）、Win32 message、`wParam/lParam` 原值、消息时间、client 坐标、wheel 屏幕坐标转换结果。
- OS Capture：事件前后 `GetCapture()`、`SetCapture`/`ReleaseCapture` 是否实际调用及结果、CaptureChanged 目标 HWND、释放原因。
- Adapter 输入/输出：NativePointerMessage 字段；DPI scale；`EditorPointerEvent` kind、Position、Buttons、Modifiers、WheelDelta、PointerId、Source。
- Router：交互 Epoch、dispatch 结果、选定 Owner、Captured 状态、消费者标识、Begin/Update/Commit/Cancel 次数与终止原因。
- 地图语义：Viewport/Projection 身份、GroundPick valid/invalid、命中坐标与 surface binding/revision、draft 节点数、最终实体 ID、提交/Undo/Redo 结果。遵守敏感数据最小化。
- 运行身份：见下节；每张截图、日志和结构化记录都应能映射到同一 Run ID。

## 运行产物身份与结果封存

每次尝试（包括失败、超时和启动失败）独立编号，不覆盖旧证据。至少采集：

- `RunId`、Capability/IPO 步骤、开始/结束时间及时区、持续时间、Result（`PASS|FAIL|TIMEOUT|FLAKY|BLOCKED_BY|UNCLASSIFIED`）、失败阶段和 Evidence Path。
- 完整 Commit SHA、Branch、Version、Dirty State、Bootstrap 结果；明确实际启动的 EXE 完整路径、文件版本/哈希、构建时间和关联 DLL 路径/哈希。不能把当前源码 HEAD 自动当成运行 DLL 的来源。
- OS/Windows build、进程 PID、窗口标识、实际 Native HWND、窗口尺寸、DPI/RenderScaling。
- Vulkan backend/loader、GPU/Adapter、Physical Device、Logical Device、Swapchain 尺寸/格式、Present/首帧证据；启动失败要保留 Harness/环境状态，不能归为产品 FAIL。
- 每项场景/工具和输入序列、事件阶段记录、屏幕录像/截图路径、实体/Undo 检查结果、Capture 释放结果及缺失字段。
- 自动化/XYT 记录身份及正式登记状态（若存在）。不得临时编造 `testId`；如需正式 P3 Harness 或测试身份，Parent 另行授权并完成 XYT 登记。

仓库现有 `docs/governance/xyt-runtime-harness.md` 的 P3-01..04 关注 Vulkan Runtime capability，并不能单独证明本任务鼠标消息链路。其证据字段和 P3/P4 区分可复用，Native Input 结果需单独报告，不得复用不匹配的 Capability 声称 Native P3 PASS。

## RED 分类与停止边界

- `Source 未到达`：真实用户动作后没有对应 HWND/Win32 message 证据；先确认窗口焦点、HWND 和事件前提，根因保持 UNKNOWN。
- `Adapter / payload`：Source 有记录而适配事件缺失或坐标、按钮、修饰键、滚轮、PointerId、Source 不一致。
- `Router / arbitration / capture`：适配事件存在但 Owner 错误/缺失、同一手势多 Owner、路由中断、OS Capture 与 Router/生命周期不一致，或终止后状态残留。
- `GroundPick / Surface miss`：Router 到达正确 Consumer，但地表查询无效、位置偏移或 Surface 身份不符。WP2 Surface Authority 是相关依赖；不得由本 Lane 改相机/地表。
- `Consumer / domain`：有效输入与命中已到达 Consumer，随后 Draft、Commit、撤销或实体结果异常。
- `Vulkan/runtime/environment`：窗口、Device、Swapchain、Present 或运行环境未建立。按 Harness/环境失败分类，不能伪装为输入产品失败或 PASS。

如发现 RED，保留可重现步骤、逐跳事件、截图、身份和原始失败；停止在本 Lane 的只读审计边界并交 Parent。任何产品修复必须由 Parent 建立单项 RED→GREEN、明确唯一产品文件 Writer/WriteScope，并与其他 Lane 的共享文件隔离。不得在此文档阶段修改生产源、Harness、csproj、版本、XYK 或 Git 状态。

## 5+100 / SRP 静态审视

本轮只读审视相关现有代码职责，不执行 Build/Test，不改代码。事件职责大致分布于 Win32 消息采集、Host forwarding、Adapter 规范化、Router arbitration、Capture Coordinator、Consumer 到 UiVm backend。后续如需修复，应按职责 Owner 定位单一最小文件范围，不能把 HWND 语义引入领域 Consumer，也不能让 UI/Renderer成为地图事实源。5+100 通过状态为 `NOT RUN`；本次没有新增 `.cs`/`.axaml`/`.js`。

## P3 / P4 状态

- 源码接线审计：已执行，只能说明存在预期调用路径。
- Synthetic / Adapter / Headless 合同：源码可见，不能代表本轮运行；其历史测试执行结果不在本 Lane 重跑，也不把它们升格。
- 真实 Windows App/窗口启动：已部分执行；真实 Windows HWND Operator Input P3：`NOT RUN`，本轮总体 `PARTIAL / NOT PASS`。
- Vulkan LogicalDevice/Swapchain/Present、真实消费/提交、OS/Router Capture 生命周期：`UNKNOWN` / 尚无充分运行证据。
- 用户真实操作、视觉/交互和产品任务验收 P4：`PENDING`，只能由用户裁决。
- Build/Test/XYT Gate、产品版本事件、提交/推送：本 Lane 均未执行；本文件不触发产品版本事件，不提交。

## XYK CANDIDATE

```text
XYK CANDIDATE
Domain: Input / Native P3 evidence
Candidate: 现有 Native Adapter、Route parity、Headless Synthetic 与生产 HWND Runtime 是不同证据层。只有记录真实非零 HWND 上的 OS 消息，并关联实际运行产物身份、生产 Router/Owner/Capture、Consumer 与领域结果，才能支持 Native Input P3；Headless synthetic 或构造 NativePointerMessage 不能替代。
Evidence: 本次源码审计 + 当前 WP3-C IPO；真实运行证据 NOT RUN。
Status: Candidate only; 请求 ChatGPT 搜索现有 K-INP-003、K-NATIVE-001、K-VAL-002、K-XYT-P3-* 后裁定 UPDATE / STRENGTHEN / CREATE / NO DEPOSIT。不得由本 Lane 写入 XYK。
```

## ChatGPT 审计记录（2026-10-10 UTC+08:00）

**审计范围**：本 IPO、仓库入口状态、当前 Editor 进程可见性、对应 XYK 条目与 P3 证据边界。产品代码保持只读；未启动 Editor、未构造输入事件、未执行 Build/Test。

**当前仓库观察**：Bootstrap 在本次审计开始时报告仓库 `E:\MyDoc\project-VSCode\XuanYuEngine`、分支 `recovery/editor-center-navigation-r1`、HEAD `15f119875ce2c880bbe4670150d622353ec92954`、WorkingTree `clean`、SDK `10.0.400`、Resolver `scripts/resolve-dotnet.ps1`、状态 `READY`。随后只读状态检查发现 WriteScope 外出现未跟踪文件 `XuanYu.World.Tests/Camera/XYEPR2NavigationSafetyFarPlaneTests.cs` 与 `XuanYu.World.Tests/Camera/XYEPR2SurfaceAuthorityQueryTests.cs`；来源未知，本轮未读取、修改或清理。该状态漂移使先前 Bootstrap clean 不能代表当前工作区仍 clean。

**Runtime 前提**：IPO 将 Windows App / Win32 / Vulkan Runtime 独占执行权分配给 Parent，并要求 Parent 明确分配独占时段。本轮可见上下文和仓库记录中没有该时段的分配事实；进程枚举也未发现名称匹配 `XuanYu`、`Editor` 或 `玄域` 的运行进程。因此真实 Native P3 本次为 `NOT RUN`，不是 `PASS`、`FAIL` 或 `BLOCKED` 产品结论。没有实际启动 EXE、HWND、Vulkan Device/Swapchain、场景操作、鼠标输入、截图或运行日志；逐跳链路、Region/Road/Marker、终止/Capture、DPI/边缘命中均保持 `UNKNOWN`，P4 保持 `PENDING`。

**XYK 裁定**：PLANNING 与 EXECUTION Knowledge Preflight 均命中现有 K-INP-001～005、K-NATIVE-001/002、K-VAL-001/002；另直接核对 K-XYT-P3-001/002。候选的证据边界已由 K-INP-003（真实生产 Source 接线）、K-XYT-P3-001（权威构建/真实运行身份）及 K-XYT-P3-002（构建准备与 Runtime Probe 分离）覆盖；K-INP-001/002/004 和 K-NATIVE-002 覆盖 Owner、Capture 与坐标/DPI 相关约束。当前没有真实 P3 新证据可用于 UPDATE/STRENGTHEN，也没有发现需要 CREATE 的知识缺口，裁定 `NO DEPOSIT`。本裁定只记录在本 WriteScope 文档，不直接写入 XYK。

**继续条件**：Parent 分配独占 Windows App/Win32/Vulkan Runtime 时段并确认运行产物后，按上方 IPO 执行；每次真实尝试单独保存完整运行身份和逐跳证据。如发现产品 RED，另交 Parent 建立独立 FIX，不在本 Lane 修改产品行为。

## Runtime 槽位记录（2026-10-10，UTC+08:00）

Run ID：`XYE-R2-WP3-C-20261010-102930`（本地 IPO 尝试编号，不是正式 XYT `testId`）
结果：`PARTIAL / NOT PASS`。只完成现有 App 启动与窗口/模块观察；没有 Operator-generated Native input，也没有完成完整 IPO。

### Bootstrap 与运行产物身份

- 命令：`& '.\scripts\xye-bootstrap.ps1'`；退出码 `0`，State `READY`，SDK `10.0.400`，Resolver `scripts/resolve-dotnet.ps1`。
- 仓库：`E:\MyDoc\project-VSCode\XuanYuEngine`；分支 `recovery/editor-center-navigation-r1`；HEAD `15f119875ce2c880bbe4670150d622353ec92954`。
- Bootstrap / `git status --short` 均显示 WorkingTree `DIRTY`。本次启动前观察到的 Dirty paths：
  - `Directory.Build.props`
  - `XuanYu.Editor.UI/Vm/Camera/UiVm.CameraDolly.cs`
  - `XuanYu.Editor.UI/Vm/Map/MapRenderSnapshotProjection.cs`
  - `XuanYu.Editor.UI/Vm/Map/UiVm.MapRender.cs`
  - `XuanYu.Editor.UI/Vm/Scene/UiVm.RenderProjection.cs`
  - `XuanYu.Editor/MapEditing/MapEditSession.cs`
  - `XuanYu.Render.Abstractions/MapRenderSnapshot.cs`
  - `XuanYu.Render.Abstractions/MapSurfaceResourceUpdatePolicy.cs`
  - `XuanYu.Render.Vulkan/Render/Map/VulkanClearFrameOwner.MapSurface.cs`
  - `XuanYu.World.Tests/Camera/XYEPR2SurfaceAuthorityCandidateTests.cs`
  - `XuanYu.World.Tests/Render/XYEPR2GenerationPolicyTests.cs`
  - `changelog.md`
  - `docs/knowledge/architecture.md`
  - `docs/knowledge/rendering.md`
  - `docs/qa/XYE-R2-WP3-NATIVE-P3.md`
  - untracked `XuanYu.World.Tests/Camera/XYEPR2NavigationSafetyFarPlaneTests.cs`
  - untracked `XuanYu.World.Tests/Camera/XYEPR2SurfaceAuthorityQueryTests.cs`
  - untracked `XuanYu.World.Tests/Render/XYEPR2GenerationWave2EpochPolicyTests.cs`
- 产物为已有输出，不在本 Lane 构建：
  - EXE：`XuanYu.Editor.App/bin/Debug/net10.0/XuanYu.Editor.App.exe`，SHA-256 `9A1BEBAF62794E4C177F1873FD9DC8C92135ABA8A12F2D3342CAD07B662CFA61`。
  - App DLL：`XuanYu.Editor.App/bin/Debug/net10.0/XuanYu.Editor.App.dll`，SHA-256 `21281F531E9469EAA60B3170136E8D004B4AB42A62540B6DD733B671C3D89C04`。
  - App 标题报告 `v0.3.0.12-fix+15f119875ce2c880bbe4670150d622353ec92954 - 未命名场景`。
  - 输出目录还包含 `XuanYu.Editor.UI.dll`、`XuanYu.Editor.dll`、`XuanYu.Render.Vulkan.dll`、`XuanYu.Render.Abstractions.dll`、`XuanYu.World.dll`；当时 DLL 写入时间在相关源码修改时间之后。它们证明运行目录有装配产物，不替代由本轮重新 Build 得到的可复现性声明。

### 实际启动与可观察事实

- 启动命令：`Start-Process -FilePath <上述 EXE 完整路径> -WorkingDirectory <EXE 所在目录> -PassThru`；命令退出码 `0`。
- PID `30724`，启动时间 `2026-10-10 10:29:30 +08:00`，进程仍 Responding；主标题见上。首次进程窗口处于最小化状态；使用 Win32 `ShowWindow(SW_SHOWNORMAL)`、`SetWindowPos` 与 `SetForegroundWindow` 恢复/置前成功，随后主窗口矩形为 `(100,100)-(1380,900)`，DPI `96`。
- 真实窗口：主 HWND `0x406A4`。枚举到可见子 HWND `0x280608`，Window Class `XuanYuVulkanViewport`。截图显示 Editor UI 和 Native Viewport。
- Windows 设备枚举：`NVIDIA GeForce RTX 3060`，驱动 `32.0.15.9186`；另有 `GameViewer Virtual Display Adapter`。
- 进程模块中可见 `vulkan-1.dll`、`Avalonia.Vulkan.dll`、`Silk.NET.Vulkan.dll`、`Silk.NET.Vulkan.Extensions.KHR.dll` 与 `XuanYu.Render.Vulkan.dll`。这只证明相关模块已加载；没有取得应用实际 Physical/Logical Device、Swapchain 或 Present 证据，不能声明 Vulkan P3 PASS。
- 截图：`docs/qa/evidence/XYE-R2-WP3-C/20261010-run01/02-editor-window-before-input.png`，1280×800，SHA-256 `AB5E2231A0C4391B6FB84D2B35D562A7117F23719B286A7451354D3435983DB7`。较早的 `01-editor-window-before-input.png` 是最小化窗口的 160×28 黑图，不作为 UI 证据。
- App 窗口内显示日志区域 `全部 41 / 信息 41 / 警告 0 / 错误 0`，但未导出逐条日志；运行目录未发现 `.log` 文件。

### 未执行 / 未确认

- 没有操作者实际 Move/Down/Up；没有将一笔真实鼠标事件从 OS HWND 逐跳关联到 Adapter、Production Router、Owner/Capture、GroundPick、Region/Road/Marker Consumer 或 Domain Commit。
- 没有新建场景并添加合法 Layer 后执行 Region/Road/Marker 全流程；没有 Undo/Redo、Esc、Tool/Mode Change、CaptureLost、KillFocus、窗口外释放或 DPI 交互证据。
- `GetCapture()` 在真实手势前后、`SetCapture/ReleaseCapture` 结果、Vulkan Device/Swapchain/Present 与帧输出均未记录，保持 `UNKNOWN`。
- 本轮未运行任何 Build/Test/XYT；未修改生产代码、Harness、csproj、XYT 映射、XYK 或断言；未执行 Git 写操作，未提交。
- 因缺少操作者输入和关键 Vulkan 运行证据，Run 结果保持 `PARTIAL / NOT PASS`；不得归因为产品 PASS 或产品 FAIL。用户 P4 仍为 `PENDING`。

本次 App 保持打开以便后续获授权的操作者实际完成鼠标 IPO；当前 UI 状态和启动本身不能代替后续交互证据。
