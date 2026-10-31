# XYE-EDITOR-UI-SRP-R1B — 成员索引与提取合同

状态：PARTIAL。已核实 156 个 partial 声明的 Roslyn 符号身份，生成成员声明索引，并完成 Log Presenter、绑定和 P0 风险补证。索引尚未为每个成员填入经核对的用途、共享状态读写、协作者调用、最终职责 owner；因此不满足全成员语义归属要求，不能作为施工 READY。

## 源码身份

- Branch codex/world-r1-migration-20261028；HEAD a88a06aa05608bfc279d9028650749c83bae62e7；Tree 23f983e12b71a4f31321df370f2c94eab6ce1dfc。
- Dirty：1991 paths（67 M / 959 D / 965 ??），与本轮开始相同。R1 记为 1955，当前比之多 36 项；旧记录无逐路径快照，不能归因全部差异。本轮指定目录新增审计产物，Changelog 原本已 Dirty。
- csproj 未显式设置 Compile Include/Remove/Update，按 SDK 默认 glob；Editor.UI 有 443 个手写/正式 C# 文件，另有 1 个生成文件 XuanYu.Editor.UI/.artifacts/ui-obj/Debug/net10.0/XuanYu.Editor.UI.GlobalUsings.g.cs；因此物理全扫数 444，手写源码数仍为 443。R1 的 444 与全量物理数一致，上一轮目录审计 443 对应排除 generated 的手写数；两者差异是生成源码计数口径，不是新增 UiVm partial。AXAML 63 个。csproj 未显式设置 Compile Include/Remove/Update；未执行 MSBuild evaluation，无法只凭静态 csproj 确认 .artifacts/ui-obj 是否由当前有效 Compile 默认排除合同覆盖，因此 Compile membership 对该单文件记 UNVERIFIED，不把它算入 UiVm/生产手写源码索引。
- 全量物理 C# 指纹（含生成文件）EEA76F05E69FD63394003234AAEC6B0646696C1A4790EB0B1478E5AA1AEFB334；手写 443 源码独立指纹 A70B21BA190FE2C7A3979EFD37557879902B486DC37C1DE38E5C6D9BFE324221。生成文件 SHA-256 A3C6048BB4D922B1799A9977F84362A3787B68810CAC67AA86AC3AA23295F227。Roslyn 语法解析包含 444 文件，但生成的 GlobalUsings 文件不含 UiVm 声明，故 156 partial / 1121 成员索引不受该口径差异影响。Git HEAD/Tree 和 Dirty 源码指纹是不同证据。

## UiVm 身份与索引

对 444 C# 文件构造 Roslyn Compilation，使用 SemanticModel.GetDeclaredSymbol 和 SymbolEqualityComparer.Default 确认 156 个 partial 声明指向同一个 UiVm 符号。只引用本机 .NET runtime assemblies，未载入 Avalonia/工程项目引用；此步骤证明 partial 身份和源码声明定位，不表示完整语义编译通过。

- Partial files 156
- Properties 412（24 个 ICommand 属性包括在内，不重复加总）
- Methods 529
- FieldDeclarationSyntax 节点 165；展开变量后字段符号 170
- Events 7（逐源声明复核，已补入 CSV）
- Constructors 3
- ICommand properties 24
- AXAML 顶层绑定名 206（脚本对所有 AXAML token 抽取得到 254 个名字，其中 UiVm 公共属性交集为 206）

完整声明底表：uivm-member-index.csv。共 1121 条请求范围成员记录（412 属性、529 方法、170 字段符号、7 事件、3 构造函数）；含 24 个 ICommand 属性标记，不额外计数。CSV 中 Reads/Writes/Calls 和最终职责类别尚未逐条验证；故是完整声明定位底表，不是完整语义成员索引。它是完整声明定位底表，不是完整语义成员索引。R1 的 156 文件目录分组内部求和一致。

## Log Presenter 状态权威与提取边界

唯一原始日志集合是 EditorLogBuffer 的 _entries，最大 500 条；EditorLogBus 写入同一个 buffer；重复末项合并、超限从头部截断。见 XuanYu.Editor.UI/Vm/Logging/EditorLogBuffer.cs 与 EditorLogBus.cs。UiVm 当前持有 buffer、bus、四个 LogProjectionCache 和 EditorLogFilterState。过滤状态是不可变 record；LogProjectionCache 仅保存 ObservableCollection 投影，不是第二份原始事实。见 Vm/Logging/UiVm.Logging.cs、EditorLogFilterState.cs、LogProjectionCache.cs、UiVm.Logging.Refresh.cs。

建议类型名 LogPresenter。依赖方向：窄日志写入接口/同一个 EditorLogBuffer → LogPresenter（filter state、派生 projection、notify）→ UiVm façade → AXAML。Presenter 不得持有 UiVm、Window、Control 或完整 editor services。UiVm 保留兼容公共接口与命令，并按原名转发 PropertyChanged。

可迁移候选状态：_logFilterState、四个 projection cache、过滤和列表刷新协调、日志投影只读值。原始 buffer 迁入 Presenter 管理但仅一份；_logBus 可由 Presenter 管理同一个 buffer。SelectedLogEntry、selected entries、剪贴板、Foot/Window 输入处理首批留在 UiVm/代码后置，避免扩大到手势生命周期。

兼容面至少包括 LogItems/ProblemItems/BuildItems/TaskItems、LogSummary、IsLogFilterAll/Info/Warning/Error、LogSourceFilter/Text、LogSearchText、AllCount/InfoCount/WarningCount/ErrorCount、HasNoLogItems、ShowInitialLogEmpty、ShowNoFilterResults、三个 projection metrics、ToggleLogCommand、SelectLogFilterCommand、ClearLogsCommand、SetLogSource，以及 SelectedLogEntry、SelectedLogClipboardText、SelectedEntriesClipboardText、HasSelectedEntries、SetSelectedEntries、NotifyLogCopied（后几项被代码后置读取/调用）。

刷新路径：当前各日志写入调用后显式 RefreshLogBindings；刷新方法同步 _logItems 等集合，再依条件发 UiVm.PropertyChanged。至少 31 个 UiVm 源文件引用 RefreshLogBindings，日志 bus 写入调用至少 58 处。不能只搬六个 Logging partial。候选合同二选一：给 buffer 增加 Change 事件，Presenter 订阅同一 buffer 并通过 UI scheduler 更新；或所有写入改走 Presenter 的窄接口并移除散布的重复手工 Refresh。禁止同时保留两套刷新导致重复通知，也禁止 Presenter 回调整个 UiVm。通知线程当前无统一保证，应在施工合同中明确 UI thread scheduler。

## AXAML 绑定合同

逐项绑定表见 log-binding-contract.csv。关键文件 Foot/Foot.axaml、Foot/LogDetailPanel.axaml；代码后置还依赖 UiVm.SelectedLogClipboardText、SelectedEntriesClipboardText、HasSelectedEntries、SetSelectedEntries、NotifyLogCopied。

- ToggleLogCommand、SelectLogFilterCommand（参数：全部/信息/警告/错误/清空筛选）、ClearLogsCommand：命令绑定，保留名称与参数。未见日志命令专属 CanExecute 可用性属性/刷新链。
- IsLogFilter* 使用 IsChecked，未明写 Mode，旁有命令；静态无法确认控件与默认 Mode 的运行交互。
- LogSearchText 是 TwoWay，有 setter，改不可变 filter state 后重算 projection 并通知搜索文本及投影属性。
- LogItems 是 ListBox ItemsSource，SelectedLogEntry 是 SelectedItem（未显式 Mode）；Foot 监听 PropertyChanged(LogItems) 触发自动滚动。
- 空状态属性在 collection refresh 后通知；计数属性在 count refresh 后通知；来源过滤按钮由代码后置调用 SetLogSource 并通知 source 属性。
- LogDetailPanel 绑定 HasSelectedLogEntry 与 SelectedLogEntry 的嵌套 LogEntry 属性。
- 主窗口设置 DataContext；Foot 的代码后置只 Hook 一次 VM PropertyChanged，Unloaded dispose auto-scroll 但未观察到 VM 事件解绑。UI 线程和 Avalonia 默认绑定行为需在授权后的运行回归验证。

## P0 补证

Manifest：MapManifestOwner 与 MapDatasetRegistry 各有可变 CurrentManifest。打开时先 Load manifest、建 Registry、载入 documents、构造 runtime、替换 MapSession，之后更新 Owner 并安装 Registry。Dataset/layer 修改后手工以 Registry manifest 镜像到 Owner。保存先存 feature content，再存 manifest；成功后 Owner.Save、重建 Registry、MapSession.MarkSaved。证据：Vm/Map/UiVm.MapManifest.cs、UiVm.MapDataset*.cs、XuanYu.Editor/MapDocument/Registry/MapDatasetRegistry.cs、Owners/ManifestBaseline/MapManifestOwner.cs。唯一权威候选可选 Registry 为工作状态、Owner 仅管路径/基线/错误；或 Owner 为唯一 manifest、Registry 只读/按需派生。先决策并定义事务失败恢复；此处不改所有权。风险是 feature 已落盘但 manifest 保存失败，或 MapSession 已替换而后续投影/Owner 更新失败。

事件：UiVm.Composition.cs 匿名订阅 SceneState.RenderSnapshotChanged；MapRender.AttachMapSession 订阅 MapSession 五个事件，未见统一 Dispose/detach。UiWin.AttachVm 会解绑命名命令事件，但每次替换时追加匿名 PropertyChanged 闭包；UiRoot.HookVm 的 _vmHooked 不解绑旧 DataContext；Foot 亦未观察到 VM PropertyChanged detach。触发条件为控件/窗口实例复用并替换 DataContext，或状态源寿命长于 UiVm。静态风险非复现泄漏。缺少 DataContext 多次替换后旧 VM 不再通知、Map/Scene 源释放后无回调、重复 attach 不重复通知的测试。

Undo/Redo：UiVm.History.cs 优先级为 Region 模式时：Region drawing draft → Road drawing draft → MapUndo/MapRedo；否则活动交互非 Idle 时拒绝；再从 EditorHistoryOwner 取场景 Undo/Redo 并应用。MapEditSession 有独立地图历史。潜在误路由场景：模式已变但草稿仍 active，或 selection/context 与 RegionEditMode 不一致。需测每个上下文只修改所属 history、模式切换/活动交互边界、保存与 Undo 后 Dirty。候选现有路径 XuanYu.World.Tests/Scene/UiHistoryTests.cs、Map/Editing 的 UI/map 测试；未运行。

## 首批设计（仅一个候选，未授权施工）

- 目标新增 LogPresenter；真实迁出 filter state、投影 cache 和刷新协调；buffer 原始集合只有一份。
- UiVm 保留公共绑定 facade、命令、PropertyChanged 名称；选中与复制状态暂留。
- 变更预计最小新增 1 类型、改动 Logging partial 与 Composition/façade 约 5–7 文件；但 31 个刷新调用文件、58 个日志写入点需逐个依赖评估，实际可能扩大，不能承诺只改 Logging 子目录。
- 必测：初始空态、severity/source/search、clear、append/repeat、500 cap、projection append/rebuild、每条原属性通知、TwoWay search、SelectedItem、命令参数、UI 线程、Foot 自动滚动与 VM 换绑。
- 回滚：同一隔离变更中恢复 UiVm 原实现、移除 Presenter 与其测试；不牵连 Manifest、Undo/Redo 或生命周期改动。
- 目录：拟置 Vm/Logging/LogPresenter.cs，不增加目录层；新增文件 <=100 行，验收按 Editor.UI 根为层级 1，分别核对直接子目录/直接 C# 文件数与深度，复用现行静态守卫。施工前另获批准。

## 限制

未运行 Restore/Build/Test，未运行 UI。完整成员读写/职责矩阵、共享状态读写/协作者依赖及全部成员职责归属尚未完成。P0 风险均为静态潜在风险，不能宣称运行故障或已关闭。



