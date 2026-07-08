# changelog

## [RZ-VK3-A-R1] Surface 契约层依赖收口 (2026-07-08)

分支：fix/RZ-VK3-A-surface-contract
提交：37504b0
推送状态：已推送 origin

### 本轮目标
收口 VK3-A 契约层，避免 UI 继续通过 Render.Vulkan 获取 NativeHost 生命周期类型。

### 修改内容
- 将 `NativeHostHandleSnapshot` / `NativeHostLifecycleState` / `NativeHostLifecycleProbe` / `NativeHostLifecycleLogFormatter` 从 `XuanYu.Render.Vulkan` 迁入 `XuanYu.Render.Abstractions`（均为纯数据/枚举/探针/日志格式器，无 Silk.NET / Vulkan 依赖），删除 Render.Vulkan 内对应 4 个文件（git 识别为 rename）。
- UI 侧 5 个生命周期链路文件（`NativeHostSurfaceContract` / `NativeHostResizeCoalescer` / `ViewportNativeHostRoute` / `Vm/UiVm.NativeHostLifecycle` / `Viewport/Vulkan/VulkanNativeHost`）的 `using XuanYu.Render.Vulkan` 改为 `using XuanYu.Render.Abstractions`。
- 同步 `changelog.md` 与 `file-tree.md`。

### 未做内容
- 未创建 VulkanInstanceOwner / VulkanSurfaceOwner / VkSurfaceKHR。
- 未碰 Swapchain / Device / Queue / PhysicalDevice / LogicalDevice。
- 未迁移 `VulkanClearSession` 历史探针（留 Editor.UI，不进正式路径）。

### 验收结果
- `XuanYu.Render.Abstractions` 不引用 Silk.NET / Avalonia / Editor.Win / Render.Vulkan。
- 低内存模式构建 Editor.UI：0 warning / 0 error；Abstractions 0W0E。
- 仓库无独立测试项目，如实记录。

### 已知债务
- Editor.UI 仍因历史 Vulkan 探针（`VulkanApiProbe` 等）保留对 Render.Vulkan 的工程级引用，不能宣称 UI 已完全解耦。
- `VulkanClearSession` 探针仍留 Editor.UI（历史债），VK3-B 不得直接搬用。

### 下一步
VK3-B1：VulkanInstanceOwner。只做 Instance（启用 `VK_KHR_surface` + `VK_KHR_win32_surface`），不碰 Surface / Device / Swapchain。

## [RZ-VK3-A] Surface 契约层建立 (2026-07-07)

分支：fix/RZ-VK3-A-surface-contract
提交：2bfbe2e
推送状态：已推送 origin

### 本轮目标
建立 UI 与 Vulkan 之间的纯净交接契约层，为后续 SurfaceOwner 接线铺路。

### 修改内容
- 新增独立 `XuanYu.Render.Abstractions` 契约工程（net10.0，零 Silk.NET 引用）。
- 定义 `NativeHostSurfaceHandle`（HWND / Hinstance / 尺寸 / DPI）与 `INativeHostSurfaceBridge`（Attach / Resize / Detach 契约接口）。
- Editor.UI 侧新增 `NativeHostSurfaceContract`，把现有 `NativeHostHandleSnapshot` 映射为交接句柄（取 `Win32ViewportHost.ModuleHandle` 作 Hinstance），不引入任何 Vulkan 实现使用点。
- `XuanYu.Editor.UI.csproj` 加对 Abstractions 的工程引用。

### 未做内容
- 未创建 VulkanSurfaceOwner / VulkanInstanceOwner / VkSurfaceKHR / Swapchain / Device。
- UI 到 Render.Vulkan 的解耦收口见 RZ-VK3-A-R1。

### 验收结果
- 低内存模式构建 Editor.UI：0 warning / 0 error。
- 仓库无独立测试项目，如实记录。

### 已知债务
- VK3-A 仅新建 Abstractions 工程，UI 对 Render.Vulkan 的工程级引用与 7 处 using 当时未移除，故 VK3-A 只算「契约层雏形已建立，解耦未完成」。

### 下一步
RZ-VK3-A-R1 收口依赖。

## [RZ-VK3-Plan] VK3 Surface 生命周期规划 (2026-07-07)
- 仅规划正式 Vulkan Surface 生命周期，替代 `VulkanClearSession` 探针状态；本轮不写任何 Vulkan 实装代码。
- 明确：Surface 由 `XuanYu.Render.Vulkan` 内部 `VulkanSurfaceOwner` 创建/持有；NativeHost 只提供 HWND/尺寸与 Attach/Detach 生命周期，不直接管理 Vulkan；Editor.UI 不直接创建 Surface/Device/Swapchain；`VulkanClearSession` 仅作探针参考，不能直接搬进正式路径。
- VK3 只做 Surface，Swapchain 留给 VK4；阶段边界硬于技术规则，禁止 VK3 夹带 Swapchain。
- 产出 `docs/rz-vk3-surface-lifecycle-plan.md`。

## [Fix-M1] Windows 兼容清单提交 (2026-07-07)
- 单独提交 `XuanYu.Editor.UI/app.manifest` 中遗留的 Windows `supportedOS` 兼容清单块（10/11/8.1/8/7），仅声明系统兼容，无任何 Vulkan / 逻辑改动。
- 不碰 Vulkan / NativeHost / Resize / Surface / Swapchain / LogicalDevice；`.workbuddy/` 与 `qizheng-mvp-fixed/` 维持未跟踪，不纳入提交。
- 提交信息：`chore(editor): declare Windows compatibility manifest`。

## [RZ-VK2-R2] NativeHost Resize 合并验证/收口 (2026-07-07)
- 验证 RZ-VK2-R1 合并边界干净：NativeHostResizeCoalescer 只合并 UI 生命周期日志，未改变 Win32ViewportHost.Resize 调用时机，未牵连 VulkanClearSession.Resize / Surface / Swapchain / LogicalDevice。
- git diff 确认 VulkanClearSession.* 相对 HEAD 零改动；本回合文件均不引用它；无新增 Silk.NET.Vulkan 使用点。
- 确认工作树仅 app.manifest 为 tracked modified（非本轮任务），不混入提交。
- 新增 `docs/audit-RZ-VK2-R2-nativehost-resize-coalesce-verify.md`，回答四问：日志已转合并 / 无残留高频直写 / 未动 Surface/Swapchain/Device / Editor.UI 直接引用 Vulkan 债务仍在但未扩大。
- 验收：`dotnet restore` 通过；`dotnet build --no-restore` 通过，0 Warning / 0 Error；`dotnet test` 退出正常且仓库无独立测试项目。
- 提交信息：`test(editor): RZ-VK2-R2 verify native host resize coalescing`。

## [RZ-VK2-R1] NativeHost 尺寸变化日志合并 (2026-07-07)
- 修复 NativeHost 尺寸变化高频事件连续进入 `EditorLogBus` 的问题（`VulkanNativeHost.OnSizeChanged` 每次直写日志并 `RefreshLogBindings`，导致截图「重复 138 次」）。
- 新增 `NativeHostResizeSnapshot`（只保存尺寸数据）与 `NativeHostResizeCoalescer`（250ms debounce，连续 SizeChanged 只更新快照与合并计数，稳定后才生成一条低频合并日志）。
- `ViewportNativeHostRoute` 增加 `ReportMerged` 薄入口；`UiVm.NativeHostLifecycle` 增加 `LogNativeHostResizedMerged`（合并日志含最终宽度、高度、DPI、生命周期版本、合并次数；无效句柄只写一条低频失效日志）。
- `NativeHostLifecycleLogFormatter` 增加 `MergedMessage` 中文合并日志格式。
- `VulkanNativeHost` 的 `OnSizeChanged` 改为走 Coalescer；`OnDetachedFromVisualTree` / `DestroyNativeControlCore` 调用 `Cancel()` 安全停止 pending debounce，不补写日志。
- 中央视口文案 `Vulkan Clear Probe` 改为 `NativeHost Probe`（`Main.axaml`）与 `Vulkan Probe`（`VulkanViewport.axaml`）。
- 未创建 Surface / Swapchain / LogicalDevice，未接入真实渲染循环，未修改顶部/左侧/右侧/底部布局与输入链路。
- 验收：`dotnet restore` 通过；`dotnet build --no-restore` 通过，0 Warning / 0 Error；本轮新增/修改 `.cs` / `.axaml` 全部 ≤100 行；`dotnet test` 退出正常且仓库无独立测试项目。

## [RZ-New-0] 新人接手规则审计 (2026-07-07)
- 新增开发规范两份（经人工校正 5+100 / 依赖隔离 / 日志边界 / VK 阶段边界表述）：`docs/dev-rules.md`（硬规则执行手册 + 接手红线清单）、`docs/dev-rules-understanding.md`（事故来源与动机解释）。
- 新增 `docs/audit-RZ-New-0-onboarding.md`：按 10 项清单完成接手验收。实测确认 Editor.UI 仍直接引用 Silk.NET.Vulkan / XuanYu.Render.Vulkan（过渡期冲突）；VulkanClearSession 探针已创建 Instance/Surface/Device/Swapchain；NativeHost 高频 SizeChanged 直写 EditorLogBus 风险属实。
- 同步 file-tree.md。
- 验收：`dotnet restore` 通过；`dotnet build --no-restore` 通过，0 Warning / 0 Error；`dotnet test` 退出正常且仓库无独立测试项目。
- 提交信息：`docs(dev): 新增开发规范文档与 RZ-New-0 接手审计`。

## [RZ-VK2] NativeHost / HWND 生命周期收口 (2026-07-07)
- 新增 `XuanYu.Render.Vulkan` 内的 NativeHost 生命周期快照、状态、探针与中文日志格式化。
- `VulkanNativeHost` 收口为纯 HWND 生命周期宿主，只记录创建、附加、句柄可用、尺寸变化、移除、释放、失效，不再触碰 Vulkan 会话。
- 新增 `ViewportNativeHostRoute` 与 `UiVm.NativeHostLifecycle`，UI 仅通过薄入口把快照写入现有日志系统。
- 新增审计文档 `docs/audit-RZ-VK2-native-host-lifecycle.md`，记录 HWND 生命周期、验证结果与 RZ-VK3 接 Surface 的接点。
- 验收：`dotnet restore` 通过；`dotnet build --no-restore` 通过，0 Warning / 0 Error；`dotnet test` 退出正常且仓库无独立测试项目。

## [RZ-VK1] Vulkan 依赖接入与环境探针 (2026-07-07)
- 新增独立 `XuanYu.Render.Vulkan` 项目，接入 `Silk.NET.Vulkan`，只负责最小 Vulkan 环境探针。
- 探针完成 Vulkan API 入口创建、Instance 版本枚举、PhysicalDevice 枚举，并输出中文诊断日志。
- UI 只通过 `VulkanProbeRoute.Run(vm)` 这一薄入口触发探针，未修改布局、输入或日志面板结构。
- 未接入 Surface、Swapchain、LogicalDevice、CommandPool、CommandBuffer，也未进入真实渲染循环。
- 新增审计文档 `docs/audit-RZ-VK1-vulkan-probe.md`，记录本轮文件清单、验证范围和下一步建议。

## [RZ-Fix3-0] Vulkan 接入前置审计 (2026-07-07)
- 新增 `docs/vulkan-preflight-audit-RZ-Fix3-0.md`，收口当前中央视口、Avalonia NativeControlHost、Win32 子窗口、Vulkan Surface/Swapchain 生命周期和 fallback 策略。
- 确认当前工程已经存在 `Viewport/Vulkan` 预接入代码，实际状态已超过纯审计阶段，应在 RZ-Fix3-A 中收口为最小 Clear Probe，而不是继续扩大到完整 Renderer。
- 明确 Vulkan 只允许进入中央视口链路：`UiRoot` -> `Main` -> `VulkanViewport` -> `VulkanNativeHost` -> `VulkanClearSession`。
- 明确低频日志边界：只记录初始化、失败、Swapchain 重建、释放等生命周期摘要，禁止每帧 Acquire / Present / RenderFrame 日志。
- 明确 fallback UI 要求：Vulkan 初始化失败时中央视口显示占位提示，并引导查看底部日志详情，不能白屏或崩溃。
- 保持顶部工具栏、左侧项目树、右侧检查器、底部日志系统职责不变；本次不接 Gizmo、Picking、模型、相机、资源系统。
- 验收：`dotnet restore` 通过；`dotnet build --no-restore` 通过，0 Warning / 0 Error；`.cs` / `.axaml` 文件未发现超过 100 行。

## [RZ-Fix3-A] — Vulkan 接入前置验证 (2026-07-06)
- 中央视口从静态假网格切换为 `VulkanViewport` 宿主，保留顶部/底部视口状态提示
- 新增 `Viewport/Vulkan` 小模块，使用 Avalonia `NativeControlHost` 在中央区域创建 Win32 子窗口作为 Vulkan Surface 承载点
- 新增 Silk.NET Vulkan 依赖：`Silk.NET.Vulkan` 与 `Silk.NET.Vulkan.Extensions.KHR`
- 最小 Vulkan 生命周期已接入：Instance / Win32 Surface / PhysicalDevice / LogicalDevice / Swapchain 创建与释放
- Resize 时跳过 0 尺寸与重复尺寸，并在尺寸变化时重建 Swapchain；仅记录重建成功 / 失败摘要
- Vulkan 初始化失败时显示中央 fallback 占位提示，不影响编辑器其他 UI
- 低频日志只记录 Vulkan 初始化开始、成功 / 失败、Swapchain 重建、释放完成；不写每帧日志
- 本轮不接模型、Gizmo、Picking、相机、资源系统，不改顶部、左侧、右侧、底部日志结构
- 当前为 Vulkan Host / Surface / Swapchain 前置验证；逐帧 Clear + Present 留到 RZ-Fix3-A-R1 收口
- Build: 0 Warning, 0 Error；GUI 烟测启动 5 秒存活

## [RZ-Fix2-D] — 右侧检查器 / 调试 / 偏好 / 模式页收口 (2026-07-06)
- 右侧面板收口为四个职责明确的页签：检查器、调试、偏好、模式
- 检查器页改为当前选中对象 / 项目的属性查看区，使用紧凑键值布局显示名称、类型和路径
- 检查器页补明确空状态：未选择对象时提示从左侧项目树、层级页或视口选择对象
- 调试页收口为当前上下文快照，分组显示当前上下文、当前对象、工具状态和输入状态，不显示日志流
- 偏好页保留编辑器偏好占位，说明布局保存、主题、快捷键和编辑器偏好后续在此收口
- 模式页显示当前工作模式与当前工具说明，作为模式状态占位
- 图标继续全部使用 SVG / PathIcon 资源，不使用字符图标、emoji 或 Unicode 图标符号
- 不改中央视口、不接 Vulkan、不改日志系统、不改顶部工具栏、不改左侧项目树
- Build: 0 Warning, 0 Error

## [RZ-Fix2-C] — 左侧项目树视觉与层级收口 (2026-07-06)
- 左侧项目区收口为更稳定的编辑器侧栏：项目 / 层级 Tab、搜索框、项目树、选中态、Hover 和空状态统一整理
- 项目页保留静态示例结构：SampleProject、世界、MainWorld、TestWorld、资源、图标、材质、脚本、构建
- 项目树行高统一为约 28px，一级、二级、三级缩进分别保持 0、18px、36px
- 选中态使用浅蓝背景和半粗文字，Hover 使用轻量底色，不抢中央视口视觉
- 搜索框文案统一为“搜索项目树...”，本轮不接真实搜索逻辑
- 层级页改为明确空状态：暂无场景对象，提示打开世界或创建对象后显示层级
- 图标继续全部使用 SVG / PathIcon 资源，不使用字符图标、emoji 或 Unicode 图标符号
- 不接真实资源扫描、不做导入导出、不做右键菜单、不改中央视口、不接 Vulkan、不改日志系统
- Build: 0 Warning, 0 Error

## [RZ-Fix2-B-R1] — Splitter 默认布局与最小宽度修复 (2026-07-06)
- 修复 RZ-Fix2-B 后左右面板可能被 splitter 或窗口压窄的问题
- 主布局根容器增加最小宽度兜底，避免左侧、中央、右侧的最小可用宽度总和被整体压穿
- 左侧项目列继续默认 270px，并在列定义与面板上双层限制 200px 至 420px
- 右侧检查器列继续默认 340px，并在列定义与面板上双层限制 260px 至 480px
- `UiRoot` 增加轻量 clamp：监听 splitter 改动后的列宽，超出范围时回弹到合法宽度
- 明确底部日志默认收起：只显示摘要条；点击展开后显示日志列表与详情，拖拽只调整底部区域高度
- 不改中央视口绘制逻辑、不接 Vulkan、不扩展日志系统、不接 Probe
- Build: 0 Warning, 0 Error

## [RZ-Fix2-B] — 主布局 Splitter 可拖拽收口 (2026-07-06)
- 主布局改为可拖拽尺寸骨架：左侧项目区、中央视口、右侧检查器、底部日志区域通过轻量 splitter 调整空间
- 左侧项目区默认约 270px，限制为 200px 至 420px，避免项目树被压没或过度挤占中央视口
- 右侧检查器默认约 340px，限制为 260px 至 480px，为属性、调试、偏好等后续内容预留可调空间
- 底部日志区域增加横向 splitter，展开时跟随底部行高伸缩，收起时保留摘要条语义
- Splitter 视觉统一为 6px 轻量分隔条，Hover 时轻微高亮，默认不抢顶部和视口视觉
- 仅调整主布局容器，不改中央视口绘制逻辑、不接 Vulkan、不扩展日志系统、不接 Probe
- Build: 0 Warning, 0 Error

## [RZ-Fix2-A] — 顶部菜单栏与工具栏收口 (2026-07-06)
- RZ-Fix1 日志阶段判定完成并冻结：后续只维护，不继续扩展 Probe、文件日志或诊断包
- 撤回 ProbeScope / Trace / 高频摘要预研入口，Probe 系统延期到真实 bug 复现且普通日志不足时再做
- 顶部区域继续保持两行结构，改为主命令区与编辑工具区的分组式布局
- 第一行按“文件 / 编辑 / 运行”分组，右侧保留克制的状态显示
- 第二行按“选择 / 变换 / 视图 / 辅助”分组，右侧保留当前工具状态
- 不改中央视口、不接 Vulkan、不扩展日志系统、不接 Probe
- Build: 0 Warning, 0 Error

## [RZ-Fix1-G-R1] — 日志详情可读性与复制验收 (2026-07-06)
- 右侧日志详情区改为更紧凑的可读布局：顶部聚合显示时间、级别、来源和分类
- 消息与详情继续使用只读正文区域，便于选择/复制日志正文
- 重复次数、上下文 ID、操作链路 ID 改为键值行，空值继续显示“无”
- 保留“复制详情”按钮与结构化中文复制文本格式
- 保持详情只由点击日志行选择驱动，不使用 Hover / PointerMoved 刷新
- Build: 0 Warning, 0 Error

## [RZ-Fix1-G] — 日志详情面板与复制单条日志 (2026-07-06)
- 底部日志展开区改为左侧日志列表 + 右侧日志详情，点击日志行后通过 `SelectedLogEntry` 显示详情
- 新增 `LogDetailPanel`，显示时间、级别、来源、分类、消息、详情、重复次数、上下文 ID、操作链路 ID
- 未选择日志时显示明确空状态：“未选择日志，点击左侧日志行后显示详情”
- 新增 `EditorLogClipboardText`，集中生成结构化中文复制文本，复制逻辑不写入 XAML 或主 VM
- 新增“复制详情”按钮，使用 Avalonia 剪贴板接口复制单条日志详情
- 日志详情由点击选择驱动，不使用 Hover / PointerMoved 刷新详情
- 保持普通 UI 标签不可选，仅日志消息和详情正文使用只读文本框便于复制
- `docs/diagnostic-safety.md` 补充日志详情选择规则：禁止 hover 驱动详情刷新
- Build: 0 Warning, 0 Error

## [RZ-Fix1-F-R1] — 构建环境与低频日志总线验收收口 (2026-07-06)
- `NuGet.Config` 移除缺失的 `.nuget-local` 本地源，改为只保留 `nuget.org`，避免新克隆仓库因本地源不存在而无法 restore
- `run.bat` 改为稳定入口：先 restore，再 `--no-restore` build，最后启动当前 `XuanYu.Editor.UI`
- 审计低频日志总线：`SampleLogEntries` 仅在 `UiVm` 实例初始化时作为种子进入实例内 Buffer，过滤切换不会重复追加种子日志
- 确认摘要条来自 `EditorLogSummary.From(_logBuffer.All)` 计算错误数、警告数和最近事件
- 确认过滤按钮只返回过滤视图，不删除 `EditorLogBuffer` 原始日志
- 搜索确认 `PointerMoved / Hover / DragPreview / RenderFrame / Picking Hover / Splitter Drag` 未写入普通底部日志
- `docs/diagnostic-safety.md` 补充后台任务日志规则：后台构建、导入、加载、保存或渲染摘要未来接入时必须通过日志队列或 UI 调度合批刷新，不得直接修改 UI 绑定集合
- Build: 0 Warning, 0 Error

## [RZ-Fix1-F] — 低频日志总线接入 (2026-07-06)
- 新增 `Vm/Logging` 低频日志模块：`EditorLogBus`、`EditorLogBuffer`、`EditorLogSummary`、`EditorLogFilter`、`EditorLogFilterQuery`、`EditorLogRepeatKey`
- 底部日志从纯 `SampleLogEntries` 过渡为 Buffer 驱动；`SampleLogEntries` 仅作为初始化种子，运行中的按钮命令和工具切换会通过 `EditorLogBus` 写入
- `EditorLogBuffer` 最多保留最近 500 条日志，并对连续相同日志使用 `RepeatCount` 合并
- 摘要条改为从 Buffer 真实计算错误数、警告数和最近事件
- 过滤按钮接入真实过滤：全部 / 信息 / 警告 / 错误 / 构建 / 任务 / 输入 / 渲染
- 首批只接低频 UI 事件：编辑器布局恢复、项目打开、启动渲染提示、新建/打开/保存/运行/停止/构建命令、工具切换
- 明确不接 PointerMoved / Hover / Picking Hover / DragPreview / RenderFrame / Splitter Drag / Vulkan 初始化 / 中央视口渲染链路
- `docs/diagnostic-safety.md` 补充低频日志准入清单和禁止高频接入清单
- `file-tree.md` 同步当前真实文件数：102

## [RZ-Fix1-E-R1] — 日志显示语义与高频风险小修审计 (2026-07-06)
- 底部日志显示层中文化：内部枚举仍保留 `Editor / Layout` 等稳定标识，UI 显示为“编辑器 / 布局 / 项目 / 加载 / 渲染 / 后端 / 输入 / 捕获”等中文文本
- 重复折叠确认绑定到对应日志行末尾，示例行显示“点击拾取未命中任何对象  重复 6 次”，不再像面板级状态
- 示例拾取日志从“拾取结果为空”改为“点击拾取未命中任何对象”，明确它是低频点击事实日志，不代表 Hover / PointerMoved 逐条输出
- 搜索框界面文案从开发占位“搜索占位”改为用户可见的“搜索日志...”
- `docs/diagnostic-safety.md` 新增“底部普通日志准入”规则：PointerMoved / Hover / DragPreview / RenderFrame / Picking Hover / Splitter Drag 禁止逐条进入底部日志
- 截图复查右侧“调试”页：当前上下文、当前对象、工具/输入状态以快照方式显示，不作为第二个日志面板
- Build: 0 Warning, 0 Error

## [RZ-Fix1-E] — 日志系统布局与调试快照职责收口 (2026-07-06)
- `file-tree.md` 重建为当前工作区真实文件树，按 `rg --files` 统计 95 个文件，删除旧文档中已不存在的历史项目/目录记录
- 底部日志栏从滚动文本占位升级为全局事实日志视图：摘要条、级别/来源过滤入口、搜索占位、列式日志列表、空状态与重复折叠占位
- 明确底部日志只展示低频事实记录，示例覆盖 Editor / Project / Render / Build / Task / Input，不接真实日志后端、不接 Vulkan、不改中央视口
- 新增轻量日志模型：`LogEntry`、`EditorLogLevel`、`EditorLogSource`、`EditorLogCategory`，字段预留 Detail / ContextId / CorrelationId / RepeatCount
- `SampleLogEntries` 替代旧 `LogText`，避免 UI 内硬编码纯字符串日志，为后续 `EditorLogBuffer / EditorLogBus` 接入预留边界
- 右侧“调试”页收口为当前状态快照：当前上下文、当前对象、工具状态、输入状态；不追加滚动日志，不与底部日志抢职责
- 调试示例文案明确高频事件策略：PointerMoved / Hover / DragPreview 后续走摘要、覆盖快照或探针，不逐条进入普通日志 UI
- 所有新增和修改的 `XuanYu.Editor.UI` `.cs / .axaml` 文件均保持 ≤100 行
- Build: 0 Warning, 0 Error；截图复查底部日志与右侧调试职责清晰

## [RZ-Fix1-D] — Avalonia 编辑器 UI 骨架收口与底部日志栏接入 (2026-07-05 20:47)
- 新增轻量 `XuanYu.Editor.UI` 编辑器外壳：顶部工具区、左侧项目/层级、中央深色视口、右侧检查器、底部状态/日志栏
- 顶部改为两行紧凑工具栏：第一行主命令（新建/打开/保存/撤销/重做/运行/停止），第二行编辑工具（选择/移动/旋转/缩放/框选/聚焦/平移/环绕/吸附）
- 顶部命令和编辑工具全部改为集中管理的 SVG / PathData 图标，禁止字符、Unicode、emoji 占位
- 左侧面板收口为 `项目 / 层级` 两个页签；项目树和层级树均使用 SVG 图标，去掉重复的“工具”页
- 右侧检查器收口为对象摘要 + 基础信息，删除重复的“当前选择”文案
- 左右侧栏页签统一为轻量 Tab Bar：浅蓝激活态、蓝色底线、非激活灰蓝文字
- 中央视口加入编辑器感占位：网格、原点轴线、视图标签、方向提示、操作提示
- 底部栏升级为可展开日志面板：默认一行日志摘要，点击后展开 `日志 / 问题 / 构建 / 任务` 四页签，日志格式保持中文
- 新增 `XuanYu.Editor.UI/Icons/EditorIcons.axaml` 集中管理 UI 图标
- 新增 `XuanYu.Editor.UI/Vm/LogText.cs` 保存静态中文日志示例
- Build: 0 Warning, 0 Error

## [9.0D-R2E] — 9.0X Native Viewport 鼠标捕获生命周期审计与修复 (2026-06-26)
- 收口所有 Win32 鼠标捕获到 `NativeViewportMouseCapture`，禁止其他模块直接调用 `SetCapture` / `ReleaseCapture`
- 修复 `WM_MBUTTONUP` 此前只清内部状态、不调用 `ReleaseCapture()` 导致 Native Viewport 继续吞鼠标消息的问题
- `Release(nint ownerHwnd, string reason)` 以 `GetCapture() == ownerHwnd` 作为是否真实释放的最终依据，不再依赖内部 `_captured` 标志
- 新增 `WM_CANCELMODE` 兜底释放路径
- `WM_KILLFOCUS` / `WM_DESTROY` / `Dispose` 均兜底释放或清理捕获状态
- `WM_CAPTURECHANGED` 只同步内部状态，新捕获窗口句柄从 `lParam` 读取，不递归调用 `ReleaseCapture()`
- 增加中文 probe log：`Debug.WriteLine` + `EditorProbe` 双写，记录捕获开始/释放/来源/按钮/hwnd/Win32 当前捕获/释放原因
- 新增审计文档 `docs/audit-NativeViewportMouseCapture-lifecycle-9.0X.md`
- 新增回归验证脚本 `tools/mouse_capture_lifecycle_verify.ps1`：中键旋转 + MoveGizmo 拖动自动验证
- 项目窗口标题改为动态读取程序集版本，build 后自动显示当前版本号
- Build: 0 Warning, 0 Error / Tests: 697/698 passed（1 个预存：中文排序依赖 locale）
- commits: `8d6e7fd` `a48ecfd`

## [9.0D-R2D] — Gizmo 拖动 Preview 高频路径复审 (2026-06-25 22:56)
- 修复 `TransformPreview` 帧完成后仍可能调用 Diagnostics refresh 的路径：Preview 回调改为只记录“跳过 Diagnostics 刷新”
- 补齐中文 probe log：PointerMoved、Gizmo hit/drag、Preview transform、RenderScene preview、Redraw、PickSnapshot、Dispatcher、Inspector、Diagnostics、WorldState、日志面板、WorldHierarchy
- 保留 DebugDock 轻量化结果：Diagnostics/Performance/RenderScene 页不复活，仅提供 no-op 兼容方法，避免重建重型 Avalonia UI
- 新增 `docs/gizmo_drag_audit_2026-06-25.md`：完整调用链、频率分级和日志结论
- 复现日志：`docs/gizmo_drag_audit_probe.log` 共 355 行；Preview 拖动中 UI/WorldState/Diagnostics/Inspector 均为 0 次，PickSnapshot 跳过 20 次

## [9.0D-R2C] — Gizmo 拖动高频路径探针审计 (2026-06-25 21:41)
- 目标：确认 Move Gizmo Preview 高频路径未触发 Inspector / Diagnostics / PickSnapshot / WorldState / Avalonia 重布局
- 在 PointerMoved → Gizmo Hit/Drag → Preview Transform → RenderScene 写入 → Redraw 请求全链路植入中文 probe log
- 探针字段：阶段名、耗时 ms、UI 刷新、WorldState 写入、Diagnostics 刷新、Inspector 刷新
- 审计结论：
  - Preview（TransformPreview）帧：UI=否 / WorldState=否 / Diagnostics=否 / Inspector=否 / PickSnapshot=跳过
  - Commit（EntityTransformChanged）帧：WorldState=是 / Inspector=是 / Diagnostics=是 / PickSnapshot=执行
  - PointerMoved 高频路径未触发 Inspector/Diagnostics/PickSnapshot/WorldState，符合 9.0D-R2B 优化目标
- 新增 `tools/gizmo_drag_postmessage.ps1`：通过 PostMessage 向 Vulkan 视口子窗口发送鼠标事件，复现拖动并采集探针日志
- 新增 `tools/gizmo_drag_audit.ps1`：SendInput 方案（未能穿透 WinExe 输入队列，保留参考）
- 新增 `artifacts/gizmo_drag_audit_probe.log`：本次审计原始探针日志（299 行）
- 新增 `XuanYu.Engine.Editor.Windows/Shell/Diagnostics/GizmoDrag/` 探针实现
- `EditorProbe` 同时输出到终端与 `%APPDATA%/XuanYuEngine/editor_probe.log`，便于 WinExe 审计采信
- Build: 0 Warning, 0 Error / Tests: 693/694 passed（1 个预存 flaky：中文排序依赖 locale）

## [9.0D-R2B] — 降低 Move Gizmo 拖动帧负载 (2026-06-25 00:18)
- TransformPreview 不再每帧刷新 Inspector
- TransformPreview 帧不再刷新 Diagnostics / DebugDock
- TransformPreview 帧不再重建 PickSnapshot
- AxisDragAnchorBuilder 删除未使用的 DragPlane 构建路径
- Inspector 更新保留在 TransformCommit / TransformCancel 路径
- Trace 审计确认未接入 UI 日志
- Build: 0 Warning, 0 Error / Tests: 693/694 passed
- commit `26f2006`

## [9.0D-R3] — 诊断日志与 UI 调度安全规范 (2026-06-24)
- 新增 `docs/diagnostic-safety.md`：收录 9.0D 诊断回调导致 UI 卡死事故的根因与防护规范
- 覆盖启动期规则 / 高频路径规则 / 诊断 Sink 接口 / UI 日志异步投递 / 代码审查清单
- commit `e57d5c9`

## [9.0D-R2] — 选中实体自动显示并可拖 Move Gizmo (2026-06-24)
- **取消「按 G 才显示 Gizmo」的交互入口**，改为选中实体 + 相机有效即自动显示
- 改动 4 个点：
  - `MoveGizmoFrameSource.Build`：闸门从 `MoveToolActive` 改为 `selectedEntity.IsValid || MoveToolActive`
  - `MoveGizmoVisibility.ShouldShow`：同步去掉 `moveToolActive` 参数
  - `EditorTransformInputRoute.HandlePointerMoved`：Gizmo Hover 检测改为选中实体即启用
  - `EditorSceneToolInputRoute.HandlePressed`：去掉 `IsMoveToolActive`，选中实体即可拖动
- G 键保留为快捷移动入口，不再是 Gizmo 出现的必要条件
- build: 0 Error / test: 693/694 (1 pre-existing)
- commit `e66cbb4`

## [9.0D-R1] — Move Gizmo 轴约束求解器 (2026-06-24)
- **AxisDragAnchorBuilder 重写**：从 ScreenProjection 升级为 DragPlane 射线约束方案
- **Gram-Schmidt 法线构造**：轴约束平面包含目标轴并尽量面向摄像机，三级 fallback
- **双保险**：DragPlane 优先 + ScreenProjection 降级，Builder 必定返回有效锚点
- **TransformDragRoute 重构**：Move 方法提取公共 MoveDrag 辅助，Axis/Plane 共用
- **诊断追踪**：TransformDragRoute 添加 Trace 回调，输出 Begin/Move/Confirm 位置值与模式
- **死代码清理**：删除 AxisTranslationStart.cs（旧 ScreenProjection 实现）
- **新增测试**：AxisPlaneTranslationSolverTests（X/Y/Z 轴正交性 / 45°视角无倍率异常 / 射线平行失败路径 / Gram-Schmidt 数学正确性）
- 后续修复 4 轮（提交 7c88740 → ba85b92 → 4ac549e → c82bb41）
- build: 0 Error / test: 693/694 (1 pre-existing)
- 当前 Gizmo 可见时直接拖轴即可移动，不必按 G

## [9.0C] — Inspector 与 Transform 同步 (2026-06-24)
- WorldState 新增 `SetRotation()` / `SetScale()`
- 新增 TransformEdit 应用层（TransformInspectorSnapshot / TransformEditRequest / TransformEditResult / SelectedEntityTransformReader / SelectedEntityTransformApply）
- Inspector 支持显示并编辑 Position / RotationDegrees / Scale
- Apply 层统一校验 Scale > 0 / NaN / Infinity，非法值拒绝写入
- 旧 Position 编辑链路保持向后兼容
- Transform 修改后标记 Dirty + 请求视口刷新
- 预留 `SetSnapshot()` 入口供后续 Gizmo 同步 Inspector 使用
- 审计文档 `docs/audit-inspector-transform-9.0C-0.md`
- 新增测试 15 项（Reader 3 + Apply 11 + SaveLoad 1）
- build: 0 Error / test: 685/686 (1 pre-existing)
- commits: `69f056b` `d1cc14c` `d4cb881` `8fb4aaa`

## [9.0B] — TransformComponent 补全：Position + Rotation + Scale (2026-06-24)
- 新增 RotationComponent / ScaleComponent（Engine 层实体组件）
- TransformComponentDocument 增加 RotationDegrees / Scale（可空，兼容旧文件）
- WorldState 支持旋转/缩放存储与查询
- WorldDocumentValidator 增加 RotationDegrees 有限校验 / Scale 有限+正数校验
- 旧版只有 Position 的 world 文件兼容加载（缺 Rotation→补 0,0,0；缺 Scale→补 1,1,1）
- WorldStateDocumentConvert 单向/双向转换同步支持完整 Transform
- 新增/更新测试 20 项（Writer/Reader/Validator/RoundTrip）
- build: 0 Error / test: 670/671 (1 pre-existing)
- commits: `80230a2` `222f49a` `2043f4e` `39dc201`

## [9.0A] — World 保存 / 加载 (2026-06-24)
- 新增 WorldDocument / WorldEntityDocument / TransformComponentDocument / WorldVector3Document / WorldMetadataDocument 文档模型
- 新增 WorldDocumentReader / WorldDocumentWriter 支持 .world.json 读写
- 新增 WorldDocumentValidator（SchemaVersion / WorldId / EntityId / Transform Position 校验）
- 编辑器打开项目时自动加载 Content/Worlds/main.world.json
- 运行菜单新增「保存 World」入口
- 新增 WorldState ↔ WorldDocument 转换
- 新增保存 / 读取 / 校验 / RoundTrip 测试 23 项
- build: 0 Error / test: 661/662 (1 pre-existing, SampleProjectSmokeTests 受未跟踪 Content/ 目录影响)
- commits: `8bd920b` `825f3b3` `70099ce` `8f3d9a1`
- 新增 4 项命名回潮门禁测试（CodeFileBudgetTests +4 → 14 项）：
  - `NoNamespaceFluidWarfare` — 禁止生产代码出现 namespace FluidWarfare
  - `NoUsingFluidWarfare` — 禁止生产代码出现 using FluidWarfare
  - `NoXClassFluidWarfare` — 禁止 .axaml 出现 x:Class="FluidWarfare.*
  - `NoClrNamespaceFluidWarfare` — 禁止 .axaml 出现 clr-namespace:FluidWarfare.*
- 允许的例外：EditorSettingsPath.LegacyAppFolder / EditorSettingsPathMigration / 历史文档
- 确认最终残留仅限：R4 Legacy 迁移路径 / 历史记录 / LEGACY 文档 / naming 说明
- docs/naming-XuanYu-Engine.md 标记 RZ 完成
- build: 0 Error / test: 638/639 (1 flaky pre-existing)
- 架构门禁：14/14
- commit `fbf509b`

## [8.8-RZ-Fix1] — Editor 启动 AccessViolation 修复 (2026-06-24 11:45)
- **根因**：`EditorShellComposition.Build()` 中初始化顺序错误 — `ProjectBootstrapRoute` 在第 41 行创建时引用了 `ctx.HierarchyRoute`，但 `HierarchyRoute` 直到第 45 行才赋值，导致 `hierarchyRoute` 为 null
- **现象**：Editor 启动崩溃，退出码 -1073741819（0xC0000005），实际为 NullReferenceException
- **修复**：将 `ctx.HierarchyRoute = new(...)` 移到 `ctx.ProjectBootstrapRoute = new(...)` 之前
- **验收**：
  - build: 0 Error / 0 Warning ✅
  - test: 638/639 通过（1 个预存 flaky：中文排序依赖 locale）
  - Editor 启动：成功，不再崩溃
  - 架构门禁：14/14
- 附带修复：run.bat CRLF 行尾（Windows 批处理兼容性）
- commit `359e3ce`

## [8.8-RZ-Fix1d] — 应用图标入库 (2026-06-24 12:30)
- 将仓库根目录 `LOGO.png`（1254×1254）复制到 `Assets/Icons/logo.png` 作为应用图标
- `.csproj` 注册 `logo.png` 为 `AvaloniaResource`（同时补注新的 ViewportNavigation SVGs）
- `MainWindow.axaml` 设置 `Icon="/Assets/Icons/logo.png"`，标题栏显示玄域引擎 LOGO
- `file-tree.md` 同步记录
- build: 0 Error / 0 Warning ✅ / test: 638/639（1 flaky pre-existing）

## [8.8-RZ-Fix1c] — 视口导航按钮 SVG 图标资源入库 (2026-06-24 12:16)
- 新增 4 个 SVG 图标资源到 `Assets/Icons/ViewportNavigation/`：
  - `nav_pan.svg` — 四向箭头，表示平移视图
  - `nav_frame.svg` — 取景框角 + 中心点，表示聚焦/查看全部
  - `nav_projection_persp.svg` — 视锥图形，表示透视投影
  - `nav_projection_ortho.svg` — 网格方框，表示正交投影
- 所有 SVG 使用 `viewBox="0 0 30 30"` + `currentColor`，匹配按钮尺寸且支持主题色
- `file-tree.md` 同步记录新资源
- 路线规划：短期为资源预案，后续接 Avalonia Overlay 或 Vulkan 贴图渲染路径
- build: 0 Error / 0 Warning ✅ / test: 638/639（1 flaky pre-existing）/ 架构门禁 14/14

## [8.8-RZ-Fix1b] — Warning 全清理 (2026-06-24 12:05)
- **7 个 Warning 逐项处理**：
  - `VulkanScene3dFrameHandles.cs` — 去重 `using Silk.NET.Vulkan`
  - `VulkanScene3dSwapchainCreateResult.cs` — `Message` 改为 `string?`
  - `VulkanScene3dRendererProbeFrame.cs` — `r.Vk` 增加 null 安全检查
  - `EditorViewportInputRequest.cs` — `ToolPalette` 改为 `ViewportToolPalette?`
  - `EditorTransformInputRequest.cs` — `ToolPalette` 改为 `ViewportToolPalette?`
  - `EditorShellGroundPointerRoute.cs` — suppress CS9113（API 设计预留）
  - `EditorPickInputRoute.cs` — `applySelection` 改为 `Action<string?,...>`
  - `StatusBarPanel.SetCurrentSelection` — 改为 `string?`，null→"无"
  - `VulkanScene3dFrameResult.Failed` — 参数改为 `string?`
  - `VulkanScene3dSession.FailFrame` — 参数改为 `string?`
- **验收**：build 0 Error / 0 Warning ✅ / 架构门禁 14/14 ✅
- commit `e3f644f`

## [8.8-R4] — 用户数据目录迁移 (2026-06-24 10:08)
- 编辑器设置目录从 `%APPDATA%/FluidWarfare/` 迁移到 `%APPDATA%/XuanYuEngine/`
- 新增 `EditorSettingsPathMigration.cs`：旧→新目录迁移逻辑（幂等、不覆盖、不崩溃）
- `EditorSettingsPath.cs`：新增 `CurrentAppFolder = "XuanYuEngine"` / `LegacyAppFolder = "FluidWarfare"`
- `EditorSettingsPath.cs`：GetSettingsFilePath 首次调用时触发迁移
- 迁移策略：新目录存在→跳过 / 仅旧目录存在→复制 / 新旧都不存在→默认
- 不删除旧目录 / 不覆盖新目录已有文件 / 迁移失败不阻止编辑器启动
- 对应测试 5 项（EditorSettingsPathMigrationTests.cs）
- 生产 `Input/Settings/` 目录 5 文件（合规）
- build: 0 Error / test: 634/635 (1 flaky pre-existing)
- 架构门禁：10/10
- commit `644aff7`

## [8.8-R3-Z] — namespace 迁移全仓收口 (2026-06-24 09:54)
- 全仓 namespace FluidWarfare.* 清零确认 ✅
- AboutFluidWarfareWindow → AboutXuanYuEngineWindow（类名 + 文件名 + x:Class + 全部引用）
- 清理 14 处非 namespace 的 FluidWarfare 字符串（Vulkan 窗口标题 / Win32 类名 / 日志 / 测试路径等）
- 删除 docs/reports/namespace-migration-R3-plan.md（生命周期完成）
- 更新 docs/naming-XuanYu-Engine.md R3 状态、file-tree.md
- 残留说明：EditorSettingsPath.AppFolderName = "FluidWarfare" 保留到 R4
- build: 0 Error / test: 629/630 (1 flaky)
- commit `710dd88`

## [8.8-R3-4] — Tests namespace 迁移 (2026-06-24 09:48)
- 迁移 namespace `FluidWarfare.Tests.*` → `XuanYu.Engine.Tests.*`（73 文件）
- 全仓 namespace `FluidWarfare.*` 清零 ✅
- 剩余：EditorSettingsPath（R4）/ AboutFluidWarfareWindow（R3-Z）/ 历史记录
- build: 0 Error / test: 629/630
- commit `5c8966b`

## [8.8-R3-3BC] — Editor.Windows 全仓 namespace + x:Class 成对迁移 (2026-06-24 09:42)
- 合并 R3-3B + R3-3C 为原子提交（partial class 必须同 namespace）
- 244 纯 C# + 16 .axaml.cs + 16 .axaml x:Class + 7 clr-namespace
- GlobalUsings.cs: 43 条 Editor.Windows 全局 using（100 行门禁）
- 清零：namespace/x:Class/clr-namespace FluidWarfare.Editor.Windows 全部 ✅
- build: 0 Error / test: 629/630
- commit `775ba48`

## [8.8-R3-2] — Render 层 namespace 迁移 (2026-06-24 09:10)
- 迁移 Render/Render.Vulkan namespace：`FluidWarfare.Render.*` → `XuanYu.Engine.Render.*`
- Render：47 文件 namespace + 147 文件跨项目 using；Render.Vulkan：154 文件 namespace
- 修复 1 处完全限定类型引用；相机白名单文件 namespace 正确迁移
- Editor/Tests namespace 保持不动（R3-3/R3-4）
- x:Class/EditorSettingsPath 未改动
- build: 0 Error / test: 629/630 (1 flaky)
- commit `aa94a43`

## [8.8-R3-1] — 底层模块 namespace 迁移 (2026-06-24)
- 迁移 Core/Engine/Project/Bridge namespace：`FluidWarfare.*` → `XuanYu.Engine.*`
- 模块内 namespace 声明：36 文件；全仓 using 引用：209 文件；总计 185 文件改动
- 命名映射：`FluidWarfare.Core→XuanYu.Engine.Core`, `FluidWarfare.Engine→XuanYu.Engine`（注意无 `.Engine` 后缀）, `FluidWarfare.Project→XuanYu.Engine.Project`, `FluidWarfare.Bridge.ProjectEngine→XuanYu.Engine.Bridge.ProjectEngine`
- Render/Editor/Tests namespace 保持不动（R3-2/R3-3/R3-4）
- x:Class/EditorSettingsPath 未改动
- build: 0 Error / test: 629/630 (1 flaky)
- commit `6a90c9e`

## [8.8-R2C] — docs audit 文件清理 (2026-06-24)
- 删除 14 个临时 audit-* / whitelist-* / renderer-* 文件
- 旧 `docs/CHANGELOG.md`（179KB，表格密集）→ `changelog.md`（简洁格式，倒序）
- `file-tree.md` 中 31KB 的"未发布变更日志"区 → 指向 `changelog.md` 的引用
- build: 0 Error / test: 629/630 (1 flaky)
- commit `68ffde8`

## [8.8-R2B] — 旧占位目录清理 (2026-06-24)
- 删除 9 个仅含 `.gitkeep` 的空占位目录：`FluidWarfare.AI` / `Combat` / `Data` / `Ecs` / `Exporter` / `Runtime.Android` / `Runtime.Windows` / `Simulation` / `World`
- 删除审计确认：9 个文件全部为 `.gitkeep`，无误伤
- 未来需要时按命名规范重新声明（`XuanYu.Engine.*` / `XuanYu.SunWu.*` / `XuanYu.Tools.*`）
- build: 0 Error / test: 629/630 (1 flaky)
- commit `5bdda34`

## [8.8-R2] — 工程外壳迁移 (2026-06-24)
- `.sln` / 9 项目目录 / `.csproj` / `ProjectReference` 全部迁至 `XuanYu.Engine.*`
- 映射：`FluidWarfare.Core→XuanYu.Engine.Core`, `FluidWarfare.Engine→XuanYu.Engine`（无后缀）, `FluidWarfare.Editor.Windows→XuanYu.Engine.Editor.Windows`, 等
- 同步更新：`InternalsVisibleTo` / `app.manifest` / 测试路径常量 / PowerShell 脚本 / `.gitkeep`
- 故意保留：`namespace FluidWarfare.*`（R3）, `using FluidWarfare.*`（R3）, `x:Class`（R3）, `EditorSettingsPath.AppFolderName`（R4）
- build: 0 Error / test: 629/630 (1 flaky)
- commit `6ad57bd`

## [8.8-R0/R1] — 品牌换名：玄域引擎 (2026-06-24)
- 正式技术品牌从 FluidWarfare 迁移为"玄域引擎 / XuanYu Engine"
- 窗口标题：`FluidWarfare Editor` → `XuanYu Engine Editor`
- About 窗口：品牌名 / 版权 → XuanYu Engine / 玄域引擎贡献者
- 菜单：`关于 FluidWarfare` → `关于 玄域引擎`
- 示例项目描述：`FluidWarfare 示例项目` → `玄域引擎 示例项目`
- Vulkan app/engine name：8 文件 → `"XuanYu Engine"`
- 文档标题：CHANGELOG / AI 规则 / 代码宪法 / 命名规则 / file-tree / shaders
- namespace / .sln / .csproj / 程序集名 / 目录名未改动
- build: 0 Error / test: 629/630 (1 flaky)
- commit `71d6187`

## [8.8-0] — 架构防回潮门禁 (2026-06-24)
- `CodeFileBudgetTests.cs` 新增 5 个门禁测试：
  - `ProductionWhitelist_OnlyApproved` — 生产白名单精确锁死为 2 个相机文件
  - `GlobalUsings_Max100Lines` — `GlobalUsings.cs` ≤ 100 行
  - `EditorShellContext_Max95Lines` — `EditorShellContext.cs` ≤ 95 行
  - `EditorShell_NotInWhitelist` — EditorShell 不得回归白名单
  - `DirectoryWhitelist_RemainsZero` — 目录白名单保持 0
- build: 0 Error / test: 629/630 (1 flaky)
- commit `4c4d82c`

## [8.7.8-Z2] — EditorShell 组合根彻底薄化 (2026-06-23)
- `EditorShell.axaml.cs`：3,041→28 行，**从白名单移除**
- 95 个 using 移入 `GlobalUsings.cs`
- 新建 Composition 架构：
  - `EditorShellContext.cs` (88 行) — 上下文持有
  - `EditorShellComposition.cs` (59 行) — Build
  - `EditorShellCompositionRuntime.cs` (65 行) — 运行时
  - `EditorShellEventWiring.cs` (67 行) — 事件接线
  - `EditorShellLifecycle.cs` (29 行) — 生命周期
- 生产白名单：3→2（只剩两个相机算法文件）
- build: 0 Error / test: 624/625 (1 flaky)
- commit `913b66b`

## [8.7.8H-5] — EditorShell 收口审计 (2026-06-23)
- EditorShell 从 3,041 行压到 491 行（含 using，body ~396 行），累计削减 2,550 行
- 决策：Transform 管线暂缓（收益 ~30 行，风险影响全链路）
- 决策：EditorShell 白名单保留（组合根例外）
- 后续策略：只出不进，新增职责必须进 Route / 子模块
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8H-4B] — EditorShell P2 中等风险清理 (2026-06-23)
- 提取日志委托 → `Shell/Diagnostics/Log/EditorShellLogRoute.cs` (18 行)
- 提取视口焦点 → `Shell/Viewport/EditorShellViewportFocusRoute.cs` (41 行)
- 提取 Scene3D 命令 → `Shell/Scene3D/EditorShellScene3dCommandRoute.cs` (19 行)
- EditorShell: 496→491 行
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8H-4A] — EditorShell P1 低风险清理 (2026-06-23)
- 提取 Raw 输入处理 → `Shell/Input/Raw/EditorShellRawInputRoute.cs` (26 行)
- 提取视口帧命令 → `Shell/Viewport/EditorShellViewportFrameRoute.cs` (43 行)
- 提取视口尺寸工具 → `Shell/Viewport/EditorShellViewportSizeGuard.cs` (24 行)
- 删除空 `ExecuteTransformApply`（无调用者）
- EditorShell: 656→496 行（含 using，body ~403 行）
- build: 0 Error / test: 624/625 (1 flaky，白名单不删)

## [8.7.8H-2G] — EditorShell 第七刀：项目加载 + World Bootstrap (2026-06-23)
- 提取项目加载残留 → `Shell/Project/EditorShellProjectBootstrapRoute.cs` (46 行)
- EditorShell: 576→567 行
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8H-2F] — EditorShell 第六刀：Startup Vulkan Probe (2026-06-23)
- 提取 Startup Vulkan Probe → `Shell/Startup/EditorShellStartupVulkanProbeRoute.cs` (46 行)
- EditorShell: 589→576 行
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8H-2E] — EditorShell 第五刀：层级树 + 选择同步 (2026-06-23)
- 提取层级树 → `Shell/Hierarchy/EditorShellHierarchyRoute.cs` (37 行)
- 提取选择同步 → `Shell/Selection/EditorShellSelectionSyncRoute.cs` (51 行)
- EditorShell: 622→589 行
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8H-2D] — EditorShell 第四刀：窗口菜单命令 (2026-06-23)
- 提取窗口命令 → `Shell/Commands/EditorShellWindowCommandsRoute.cs` (24 行)
- EditorShell: 629→622 行
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8H-2C] — EditorShell 第三刀：Viewport 生命周期 + Vulkan Redraw (2026-06-23)
- 提取 Viewport 重绘 → `Shell/Viewport/EditorShellViewportRedrawRoute.cs` (83 行)
- EditorShell: 665→629 行
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8H-2B] — EditorShell 第二刀：Transform 编辑 + Scrub (2026-06-23)
- 提取 Transform 路由 → `Shell/Transform/EditorShellTransformRoute.cs` (62 行)
- 提取 Scrub → `Shell/Transform/EditorShellScrubRoute.cs` (24 行)
- EditorShell: 725→665 行
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8H-2A] — EditorShell 第一刀：Overlay 导航 + 地面指针 + Picking (2026-06-23)
- 提取 Overlay 导航 → `Shell/Navigation/EditorShellOverlayNavigationRoute.cs` (78 行)
- 提取地面指针 → `Shell/Picking/EditorShellGroundPointerRoute.cs` (63 行)
- 提取 Picking → `Shell/Input/Picking/EditorPickInputRoute.cs` (79 行)
- EditorShell: 969→725 行
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8G-2] — EditorPreferencesWindow SRP 拆分 (2026-06-23)
- `EditorPreferencesWindow.axaml.cs`：587→78 行
- 提取 Capture 逻辑 → `EditorPreferencesCapture.cs` (77 行)
- 提取 BindingList 管理 → `EditorPreferencesBindingList.cs` (81 行)
- 提取 DraftHandler → `EditorPreferencesDraftHandler.cs` (79 行)
- 提取 Helpers → `EditorPreferencesHelpers.cs` (30 行)
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8F-2] — VulkanRenderContext SRP 拆分 (2026-06-23)
- `VulkanRenderContext.cs`：476→92 行
- 提取 Context Setup → `Context/VulkanRenderContextSetup.cs` (78 行)
- 提取 Device Selector → `Context/VulkanRenderContextSelector.cs` (32 行)
- 死代码锁定 Legacy
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8E-2B] — VulkanClearProbe SRP 拆分 (2026-06-23)
- `VulkanClearProbe.cs`：416→99 行
- 提取 ContextScope → `Clear/Probe/VulkanClearProbeContextScope.cs` (96 行)
- 提取 DeviceSelector → `Clear/Probe/VulkanClearProbeDeviceSelector.cs` (42 行)
- 提取 SurfaceQuery → `Clear/Probe/VulkanClearProbeSurfaceQuery.cs` (60 行)
- 提取 RenderTargetScope → `Clear/Probe/Render/VulkanClearProbeRenderTargetScope.cs` (98 行)
- 提取 RenderSubmitScope → `Clear/Probe/Render/VulkanClearProbeRenderSubmitScope.cs` (54 行)
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8E-2A] — Clear 目录容量整理 (2026-06-23)
- `Clear/Probe/` 目录 9→6 文件（容量达标）
- build: 0 Error / test: 624/625

## [8.7.8D-2B] — VulkanSwapchainProbe SRP 拆分 (2026-06-23)
- `VulkanSwapchainProbe.cs`：301→78 行
- 提取 ContextScope → `Swapchain/Probe/VulkanSwapchainProbeContextScope.cs` (100 行)
- 提取 DeviceSelector → `Swapchain/Probe/VulkanSwapchainProbeDeviceSelector.cs` (46 行)
- 提取 SurfaceQuery → `Swapchain/Probe/VulkanSwapchainProbeSurfaceQuery.cs` (64 行)
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8D-2A] — Swapchain 目录容量整理 (2026-06-23)
- `Swapchain/` 子目录重建：Probe/ / Context/ / Image/ / Sync/
- 文件迁移确保 ≤5/目录
- build: 0 Error / test: 624/625

## [8.7.8C-2] — GameProjectLoader SRP 拆分 (2026-06-23)
- `GameProjectLoader.cs`：392→82 行
- 提取 ManifestReader → `Loading/GameProjectManifestReader.cs` (89 行)
- 提取 FolderParser → `Loading/GameProjectFolderParser.cs` (100 行)
- 提取 ExtensionParser → `Loading/GameProjectExtensionParser.cs` (52 行)
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8B-4] — VulkanDeviceProbe SRP 拆分 (2026-06-23)
- `VulkanDeviceProbe.cs`：288→77 行
- 提取 InstanceScope → `Device/VulkanDeviceInstanceScope.cs` (61 行)
- 提取 Selector → `Device/VulkanDeviceSelector.cs` (80 行)
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8B-2] — VulkanSurfaceProbe SRP 拆分 (2026-06-23)
- `VulkanSurfaceProbe.cs`：203→66 行
- 提取 InstanceScope → `Surface/VulkanSurfaceInstanceScope.cs` (98 行)
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.8A-2] — WindowsViewportInputTranslator SRP 拆分 (2026-06-23)
- `WindowsViewportInputTranslator.cs`：284→54 行
- 拆为：`WindowsViewportModifierState.cs` (37) / `WindowsViewportRawInputTranslate.cs` (76) / `WindowsViewportGestureMatch.cs` (28)
- 白名单：1 项删除
- build: 0 Error / test: 624/625 (1 flaky)

## [8.7.7F] — 全仓白名单债务审计与 8.7.8 路线图 (2026-06-23)
- F-1：全仓盘点，49 项行白名单 + 8 项目录白名单
- F-2：3 项立即可清（InspectorPanel 84 行 / NativeHost 87 行 / SceneCameraPose 99 行）
- F-3：9 文件压缩 + 4 目录子目录化，13 项白名单删除
  - 文件压缩：WorldHierarchyTreeIndex (112→46) / ProjectContentNodeView (114→52) / WorldHierarchyTreeBuilder (126→49) / EditorInputActionCatalog (148→60) / VulkanInstanceProbe (122→43) / VulkanDebugMessengerScope (133→55) / VulkanValidationAvailabilityProbe (118→40) / GameContentFileScanner (130→38) / WorldState (121→48)
  - 目录子目录化：Validation 7→5, Camera 7→5, ProjectContentTree 6→5, Transform/Drag 6→5
- F-4A：DebugDockPanel SRP 提取 (145→53) / ViewportPlaceholderPanel SRP 提取 (189→46) / WorldHierarchy 目录 8→5
- F-4B：Panels/Viewport 11→5 文件 / Transform/Gizmo 8→5 文件
- F-4C：ViewportNavigation 9→5 文件（最后 1 个目录白名单删除）/ VulkanSceneRayBuilder SRP 167→40 / VulkanCameraMatrices SRP 189→21
- F-5：大债务登记到 8.7.8（13 项生产白名单分 A/B/C/D 四类）
- F-6：最终收口 — VulkanViewportHostPanel 158→43 / EditorInputBindingSnapshot 175→38；SceneNavigationCameraMotion(173) 与 SceneOrbitCameraMotion(202) 因相机算法放弃
- build: 0 Error / test: 625/625

## [8.7.7E] — 全仓白名单深度清理 (2026-06-22)
- E-1：VulkanScene3dRenderer SRP — 主文件 261→41 行，5 子模块全部 ≤100
- E-2A：Scene3D Session SRP — Session 主文件 371→46，CreateInstance/FrameFlow/Handles/FrameAcquire/Lifecycle
- E-2B：Swapchain SRP — 去重合并后 6 文件，全 ≤100
- E-2C：VulkanScene3dRenderer 去重式 SRP — 消除 3 个重复文件
- E-2D：Scene3D 白名单删除 + Overlay 目录 8→4
- 最终 9 文件白名单删除，ViewportNavigation 目录白名单清理
- build: 0 Error / test: 625/625

## [8.7.7D] — 目录子目录化 + 文件重组 (2026-06-22)
- D-1：Shell/Scene3D/ 11→5 文件（Scene3dFrameState/Scene3dDrawListBuilder/Scene3dPresentedState 迁入子目录）
- D-2：Viewport/Picking/ 重构 + Viewport/Transform/ 子目录重组
- build: 0 Error / test: 625/625

## [8.7.7C] — NativeHost / ViewportPlaceholder / DebugDock SRP (2026-06-22)
- C-1：NativeHost.axaml.cs 158→43 行（HWND 生命周期提取 / HostInfo 提取 / Input 提取）
- C-2：ViewportPlaceholderPanel.axaml.cs 189→46 行
- C-3：DebugDockPanel.axaml.cs 145→53 行
- 白名单 -3
- build: 0 Error / test: 625/625

## [8.7.7B] — Project / World Tree Panels SRP (2026-06-22)
- `ProjectContentTreePanel.axaml.cs`：128→63 行
- `WorldHierarchyTreePanel.axaml.cs`：229→95 行
- 新建 WorldHierarchyTreeItems.cs(14) / TreeExpansion.cs(43) / TreeSelection.cs(87)
- 白名单 -2
- build: 0 Error / test: 625/625

## [8.7.7A] — InspectorPanel SRP 拆分 (2026-06-22)
- `InspectorPanel.axaml.cs`：145→53 行
- 提取 TransformHeader.cs(31) / EntityIdRow.cs(26) / GroupSeparator.cs(16)
- 白名单 -1
- build: 0 Error / test: 625/625

## [8.7.6] — EditorShell Route 化重构 Phase 3：Composition (2026-06-21 ~ 22)
- 8.7.6.8C — Startup Bootstrap / Lifecycle / Vulkan Probe Route 化
- 8.7.6.8D-1 — Input Pipeline / Raw Viewport Events 提取
- 8.7.6.8D-2 — Transform / SceneTool Input Bridge
- 8.7.6.8D-3 — Ground Hover / Pick Bridge
- 8.7.6.8D-4 — Scene3D Manual Run / Session Commands
- 8.7.6.8D-5 — Panel Operation Apply
- 8.7.6.8E-1 — Transform / Ground Placement Apply 收口
- 8.7.6.8E-2 — Diagnostics / Refresh / Probe Residual 收口
- 8.7.6.8E-3 — Constructor / FindControls / Route Wiring
- 8.7.6.8E-3R/4 — Composition Cleanup + Final Stabilization（EditorShell 3,041→2,157 行）
- build: 0 Error / test: 625/625

## [8.7.5] — EditorShell Route 化 Phase 2：Selection & Gizmo (2026-06-20 ~ 21)
- 8.7.5.1-3 — Scene3D Frame Route 提取（FrameRoute/FrameState/DrawListBuilder/PresentedState）
- 8.7.5.4 — Viewport Pointer Pick Route
- 8.7.5.5A-C — Transform Interaction（State/Result/PointerRoute/KeyboardRoute/StartRequest）
- 8.7.5.5D — Transform Application（Preview/Commit/Cancel/Result）
- 8.7.5.5E — Gizmo（MoveGizmoDrawList/Element/HitTest/Interaction/Layout/VisualState/Snapshot）
- 8.7.5.6A-C — Camera Route / Focus / Navigation
- 8.7.5.6D-E — Frame Submit / Session Lifecycle
- 8.7.5.6F-G — Diagnostics / Vulkan Probe
- 8.7.5.7A-C — Selection Presenter / Route / State
- 8.7.5.8A — Project Bootstrap Route
- 8.7.5.8B — World Bootstrap Route (EntitySeed/RenderSeed/Input/Result)
- EditorShell 3,041→1,200+ 行（Route 化 Phase 1 后继续拆解）

## [8.7.4] — Scene3D 渲染与选择系统独立 (2026-06-18 ~ 20)
- Scene3D 渲染模块独立化（Scene3dFrameRun/Scene3dSessionLifecycle 等）
- 选择系统 Route 化（EditorSelectionRoute/State/Request/Result/Reason）
- 选择呈现（ViewportSelectionPresenter/WorldEntitySelectionPresenter）
- Picking 管线独立（ViewportPointerPickRoute）
- 多对象绘制与 Depth Buffer

## [8.7.3] — Vulkan 管线稳定化与 Swapchain 重构 (2026-06-17 ~ 18)
- Swapchain API 结果加固与生命周期规则收口
- Vulkan Clear 与 Swapchain Probe 重构
- Surface/Device/Instance 创建链路稳定化

## [8.7.2] — Transform 编辑基础 (2026-06-16 ~ 17)
- 单实体 Transform 编辑与地面放置
- 3D 地面拾取、世界坐标反馈与落点标记
- Gizmo 基础呈现（MoveGizmo）

## [8.7.1] — 视口与输入系统 (2026-06-15 ~ 16)
- 默认 3D 主视口、俯视矩阵修复
- Windows 原生视口子窗口宿主完善
- 输入管线路由化（RawInput→Transform→SceneTool）

## [8.7.0] — Shell Route 化 Phase 1 (2026-06-14 ~ 15)
- EditorShell 从 ~3,041 行开始 Route 化重构
- 第一批 Route 提取：Startup、Lifecycle、Log、PanelSwitch
- Route 装配与组合根（EditorShellComposition）

## [8.6] — 3D 地面拾取与 World Hierarchy (2026-06-12 ~ 14)
- 3D 地面拾取、世界坐标反馈与落点标记
- World Hierarchy 节点树与编辑器选择收口
- SVG 经典资源管理器式双树菜单
- 左侧双树页签、项目文件树与中文界面收口

## [8.5] — World Hierarchy 与选择系统 (2026-06-11 ~ 12)
- World Hierarchy 节点树（WorldHierarchyNode/TreeBuilder/Search）
- 编辑器选择 Route 化
- 项目内容树面板拆分

## [8.4] — 3D Picking 与单位选择 (2026-06-10 ~ 11)
- 3D Picking 管线（ScenePointerPicker/SceneRayGroundIntersection）
- 单位选择与高亮
- Picking 与选择 Route 化

## [8.3] — 持久 Scene3D 渲染会话与 RTS 相机 (2026-06-09 ~ 10)
- 持久 Scene3D 渲染会话（Session/Surface/Swapchain/Lifecycle）
- RTS 相机基础控制（ViewportNavigation）
- Overlay 渲染

## [8.2] — 多对象 3D 绘制与 Depth Buffer (2026-06-08 ~ 09)
- 多对象 3D 绘制（顶点缓冲/索引缓冲）
- 基础 Depth Buffer
- Ground Cursor 绘制

## [8.1] — Vulkan 3D 基础管线 (2026-06-06 ~ 08)
- Vulkan 3D 基础管线（ShaderModules/PipelineLayout/Pipelines/CommandRecorder）
- Scene3D 隔离（手动触发，不与 Editor 自动绑定）
- SPIR-V 手写编码废弃 → 标准 glslangValidator 工具链
- Validation Layer 开关接入
- Scene3D Renderer SRP 拆分

## [8.0] — RenderScene GPU 点位绘制 (2026-06-05 ~ 06)
- RenderScene 单对象 GPU 点位绘制
- Vulkan 战场视口填充与重绘修复
- 多对象点位绘制

## [7.8] — Vulkan 最小可见渲染闭环 (2026-06-04 ~ 05)
- 最小可见渲染闭环（CreateInstance→CreateSurface→CreateDevice→CreateSwapchain→Render→Present→Cleanup）
- Swapchain 扩展加载修复
- 底部调试终端与主视口收束

## [7.0~7.7] — Vulkan 基础集成 (2026-06-02 ~ 04)
- Vulkan 最小清屏（Clear Probe）
- Vulkan Instance 最小创建与释放
- Vulkan Device 最小选择与释放
- Vulkan Surface 宿主边界
- Windows 原生视口子窗口宿主
- Vulkan Surface 创建成功回归

## [6.0~6.1] — RenderScene 抽象 (2026-06-01 ~ 02)
- RenderScene 最小抽象
- 视口 RenderScene 调试显示

## [5.0~5.3] — World 实体与选择 (2026-05-31 ~ 06-01)
- 最小 World 实体
- 从项目内容生成占位实体
- 最小 World 实体列表面板
- World 实体选择与视口联动占位

## [4.3~4.4] — 项目系统 (2026-05-30 ~ 31)
- 项目内容文件入口声明与扩展名校验
- 项目校验报告

## [2.x~4.x] — 核心值对象与初始骨架 (2026-05-29 ~ 30)
- 解决方案骨架、项目宪章、架构说明、AI 开发规则、代码宪法、命名规则
- `EntityId` / `TimeStep` / `SimulationTime` / `Vector3d` / `YawRotation` / `EngineError` / `EngineResult`
- 初始项目内容文件入口声明
- 中文化补丁：明确人类可读文本默认使用中文

## [0.0.1-dev] — 初始创建 (2026-05-28)
- 创建初始解决方案骨架（`FluidWarfare.sln`）
- 创建顶层模块目录和资源目录规划
- 创建项目宪章、架构说明、AI 开发规则、代码宪法、命名规则、Phase 1 范围和旧仓库考古报告
- 创建 `.gitattributes`，固定 Markdown、解决方案、C# 和 JSON 文件使用 LF 行尾
- 创建 `FluidWarfare.Core` 纯 C# 类库项目
- 创建 `FluidWarfare.Tests` xUnit 测试项目
- 创建 `CoreSmokeTests` 最小冒烟测试
- `docs/MILESTONE1_PUBLIC_VALIDATION.md`：记录公开 GitHub Raw 验收命令
