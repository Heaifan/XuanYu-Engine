# changelog archive: 2026-09
<!-- Records: 88; Month: 2026-09 -->
> Archive Authority: frozen historical record for 2026-09.
> Root `changelog.md` keeps this month as the one-month Reference Mirror.

## v0.3.0.5-fix · P1-FIX5 / CURSOR-ANCHORED ZOOM
- 目标：修复鼠标滚轮缩放丢弃 `EditorPointerEvent.Position`，使 Perspective 与 Orthographic 都按 Terrain 或 ReferencePlane 锚点缩放。
- 变化：完整保留 Position + WheelDelta 输入链；支持中心、偏心、Terrain、ReferencePlane、Zoom In/Out 的锚点合同，连续 10 次屏幕漂移目标为 <= 2px。
- 状态：自动回归与架构门禁待最终候选重验；T3/T4 用户真实 App 验收未完成，保持 PROVISIONAL。

## v0.3.0.4-fix · P1-FIX4 / TERRAIN RUNTIME VISIBILITY
- 目标：修复 Terrain 存在时 `RenderDrawPlan` 同时绘制 `MapGround`，导致 World Surface 被错误地面覆盖。
- 变化：Terrain 作为 World Surface；Terrain 存在时抑制 `MapGround`，保留 MapBounds、Region Overlay、Grid/Assist。
- 状态：RED witness 已转为候选 GREEN；TerrainResources 非空不等于视觉通过，T3/T4 用户真实 App 验收未完成，保持 PROVISIONAL。

## v0.3.0.3-fix · XYT-T2 / T-A-FIX / A-TERRAIN-CONTEXT
- 目标：修复切换 Terrain context 后 RegionEditor 仍占据 432 DIP host slot 的产品根因。
- 变化：IsRegionEditMode 统一复用 IsRegionContext 权威状态，并在 context 切换时通知派生状态；Terrain context 下 RegionEditor host slot 折叠为 0。
- 验证：SAME TEST RED Expected 0 / Actual 432，POST-FIX GREEN；TerrainContextRuntimeFixTests 1/1；Terrain scoped 145/145；Context/Mode lifecycle 267/267；ARCH-A、5+100、git diff --check PASS。
- 状态：FIX Event XYT-T2-T-A-FIX-A-TERRAIN-CONTEXT 为 PROVISIONAL；本 Lane 不 Commit/Push，等待 Final Convergence。

## v0.3.0.1-fix · WORLD AUTHORING / TERRAIN FOUNDATION CUT-1
- 目标：修复进入 Region Authoring 后 Terrain 地表被 MapGround 覆盖，以及 Terrain 命中被默认 Map bounds 二次否决。
- 变化：Region context 保留 Terrain Surface 并关闭 MapGround；Terrain binding 使用 Terrain extent 作为有效命中范围。
- 验证：Terrain Authoring Continuity 合同覆盖 DEM 导入、区域上下文、两次真实地形点击与顶点计数 0→1→2。
- 状态：自动验证完成；真实 App 的视觉/真机路径仍待人工验收。

## v0.3.0.0-r1 · WORLD AUTHORING / TERRAIN FOUNDATION CUT-1
- v0.3 World Authoring 世代正式开始。
- Terrain Source → World、Metadata、Top Context、Inspector、VerticalExaggeration 与 RenderProjection → Vulkan 数据链完成。
- 自动集成验证目标为 PASS，完整 Solution Build 目标为 0W/0E。
- 正式 run.bat 真机验收仍 PENDING；Terrain CUT-1 尚未 CLOSED。
- 基线 HEAD：d0b88ad732a6b1bd4ec7dee3ab8b42c075985acf。
- 版本治理切换为 Version Single Source of Truth。

## v0.2.28.77-fix4 · INSPECTOR 修复只读值重复 Tooltip (2026-09-16 14:21:56 +08:00)
- 目标：修复 Inspector 普通只读值悬停时重复弹出相同内容的问题。
- 变化：移除 `InspectorReadOnlyValuePresenter` 普通 `XYText` 的无条件 Tooltip，保留只读文本和复制入口；未改动诊断 Element Probe、Schema、公共 API 或 Inspector 布局。
- 验证：新增 Presenter 内部契约回归；RED 已确认旧 Tooltip 被命中，待本轮 GREEN、受影响 Build、ARCH-A、5+100 与 `git diff --check` 完成。
- 遗留：涉及 UI 视觉行为，完成自动验证后仍待用户真机验收，不得自行 CLOSED。

## v0.2.28.77-fix3 · INSPECTOR 治理回写 ERR/EXP (2026-09-16 13:08:00 +08:00)
- 变化：基于真实失败回归正式登记 ERR-20260916-001，提炼并索引 EXP-UI-001；本轮不改动正式代码。
- 验证：沿用代码修复提交 a7472917866df357f3d3694b183d21672f7fbabc 的 Inspector/Entity 16/16、Build 0W/0E、ARCH-A、5+100、diff-check 与远端 0/0 证据。
- 状态：ERR 已验证；UI 功能仍待用户真机验收。

## v0.2.28.77-fix3 · INSPECTOR-2.0-R1-FIX2 稳定编辑目标修复 (2026-09-16 13:02:22 +08:00)
- 目标：修复 Inspector 属性编辑在 Selection 切换后提交到错误对象，以及 Recent 跟随当前 Selection 的问题。
- 变化：新增 `InspectorEditTarget = ObjectKind + ObjectId + PropertyKey`；Entity/Road/Region/Marker 的 Commit 与 Recent 共用稳定目标身份；补齐四类对象 A→B Selection 切换回归；修复 Entity 名称提交递归路径。
- 验证：Inspector/Entity 专项回归 16/16；受影响 UI Build 0W/0E；ARCH-A、5+100、git diff --check PASS。
- Hash：以本轮最终 Commit 为准。
- 遗留：涉及 UI 行为，待用户真机验收；未自行 CLOSED。

## v0.2.28.77-fix3 · INSPECTOR-2.0-R1-FIX1 真机缺陷修复 (2026-09-15 22:25:31 +08:00)
- 目标：修复只读 Scalar Presenter、嵌套旧 Inspector Shell 与分类导航 Rail 表现层。
- 变化：只读值统一使用 XYText 与复制入口；Inspector 内容宿主移除旧面板导航；导航项保持最近入口并使用选中语义样式；Road/Region/Marker schema 未修改。
- 验证：Inspector 专项回归 99/99；完整解决方案构建 0W/0E；ARCH-A 与 5+100 通过；git diff --check 通过。
- Hash：9186fba7。
- 遗留：尚未进行真机验收；R1-FIX1 完成后等待用户验收；未进入 Inspector R2。

## v0.2.28.76-r1 · INSPECTOR-2.0-R1 Navigation/Search/Recent Closeout (2026-09-15 21:55:59 +08:00)
- 目标：完成 Inspector 2.0 冻结设计的固定导航、Single Focus Page、类型级 Recent MRU 与全局属性搜索。
- 变化：保留单一右侧内容滚动宿主；新增中文语义分类、Ctrl+F 搜索、键盘导航、稳定 Property Key 和按 InspectorObjectKind 隔离的 Session Recent；Road/Region/Marker 继续使用只读描述，不改变数据 schema。
- 验证：Inspector 专项测试 96/96；完整解决方案构建 0W/0E；ARCH-A 与 5+100 通过；git diff --check 通过。
- Hash：45ae71f9。
- 遗留：未进行真机验收；自动测试不等于 CLOSED；状态为 READY FOR USER ACCEPTANCE。

## v0.2.28.75-rz · XYENGINE-DIAG-V1-FIX2 Element Probe (2026-09-15 17:50:00 +08:00)
- 目标：实现 FIX2 元素探针；不重复修改 FIX1。
- 变化：新增 Semantic/DeepVisual Resolver、ParentDebugId、RuntimeLocator、内部 Overlay Probe 高亮、元素拾取菜单、Hover/Alt+Hover、点击复制和 ESC 退出；关闭诊断模式清理 Probe。
- 验证：FIX2 定向测试 18/18；完整解决方案构建 0W/0E；World 全量 1570 通过 / 4 项既有 baseline 失败；Core 339/339；WarCore 22/22；XYUI 617/617；ARCH-A、git diff --check 通过。
- Hash：1ac04ed3。
- 遗留：未进行真机验收；自动测试不等于 CLOSED。

## v0.2.28.74-rz · XYENGINE-DIAG-V1 Diagnostic Mode T3 (2026-09-15 09:52:45 +08:00)
- 目标：XYENGINE-DIAG-V1 Diagnostic Mode T3 初始注册与收口。
- 变化：接入现有编辑器区域、模块及 Road/Region Inspector 动态诊断身份；补充运行时注册回归测试。
- 验证：FIX1 诊断测试 22/22；解决方案完整构建 0W/0E；World.Tests 1552 通过 / 4 项既有 baseline 失败；ARCH-A、git diff --check 通过。
- Hash：673bb21b。
- 遗留：FIX1 已修复，等待真机复验；仍不得 CLOSED。

## v0.2.28.73-rz · INSPECTOR-2.0-CR1 TODO-2 SINGLE-FOCUS (2026-09-14 22:42:00 +08:00) - 目标：将 Inspector 一级内容改为 Selection identity 作用域内的单焦点语义 Section。 - 变化：新增 ExpandedInspectorSection 与 InspectorSectionId 状态机；身份变化自动收起；Map、Marker、Entity、Road、Region 使用目录式 Section；Map Inspector 移除内部 XYPager，XYPager 实现未修改；几何工具仍留在既有工具/视口边界。 - 验证：TODO-2 focused 47/47；Solution Build 0W/0E；Core 339/339；WarCore 22/22；World 1530 PASS / 4 PRE-EXISTING BASELINE FAIL；XYUI 617/617；ARCH-A、5+100、XML 133/133、git diff --check PASS。 - Hash：5ed4b8fac704b41d2f1140ca1e699b0e096d6a12。 - 遗留：等待真机 Inspector 视觉与交互验收；4 项 PRE-EXISTING BASELINE DEBT 独立处理；TODO-3 保持阻塞。

## v0.2.28.73-rz · INSPECTOR-2.0-CR1 TODO-1 CLOSEOUT (2026-09-14 22:26:36 +08:00) - 目标：完成 Selection Contract 验证与 FIX-R1 收口。 - 变化：保留 Entity selection 优先级修复、真实几何测试夹具与旧测试契约更新；5 项失败经最小解锁 baseline 证明为既有债务，独立登记为 PRE-EXISTING BASELINE DEBT。 - 验证：T1-T7 7/7；Marker workflow 10/10；Create Cube PASS；focused regression 22/22；Core 339/339；WarCore 22/22；World 1526 PASS / 5 baseline FAIL；XYUI 617/617；Solution Build 0W/0E；ARCH-A、5+100、XML 133/133、git diff --check PASS。 - Hash：08e16f15ee48b93fbb8ae3b4d761ae27130c1e0a。 - 遗留：5 项 PRE-EXISTING BASELINE DEBT 不阻塞 TODO-1；TODO-2 需验收方解锁。

## v0.2.28.73-rz - INSPECTOR-2.0-CR1 TODO-1 (2026-09-13 23:05:45 +08:00) - 目标：建立 Selection 驱动的 Inspector identity 契约，完成 None、Map、Marker、Road、Region 映射，并保持 Geometry Edit 不替换对象身份。 - 变化：新增 Inspector identity resolver 与 T1-T7 定向回归测试；Inspector 一级路由移除 Mode 隐式 Map 回退；地图几何选择与 MapEditSession 选择同步清除。 - 验证：git diff --check PASS；手写 .cs/.axaml 5+100 静态检查 PASS；dotnet test 被环境阻断（未安装 .NET SDK）；正式 Build/Test/ARCH-A 未执行。 - Hash：42591834。 - 遗留：需在安装 .NET SDK 的环境执行 RED→GREEN、完整 Build/Test、ARCH-A、5+100，并更新最终 Hash。

## v0.2.28.73-rz - EDITOR-UI-SCROLL-AUDIT-R1 CHECKPOINT (2026-09-13 00:00:00 +08:00)
- 目标：移除 Right Inspector、MapEditor 与 Debug 页中的非法 ScrollViewer，并修复 Right Tabs 三页互斥可见性。
- 变化：新增 RightWorkspaceHost；InspectorWorkspace、HierarchyWorkspace、DebugWorkspace 严格互斥；新增 Scroll Audit 与 Right Tabs Runtime Contract。
- 验证：Scroll Audit 5 legal / 0 violations；Area D R2 Runtime 5/5；RightTabsVisibilityRuntimeTests PASS；Solution Build 0 warning / 0 error；ARCH-A、5+100、XML、diff-check PASS。
- 遗留：World 全量 1518 PASS / 6 FAIL，记为 KNOWN TEST CONTRACT DEBT；XYUI 617 个测试断言通过但清理阶段退出码为 1；本提交为 CHECKPOINT，不是 TECH COMPLETE。

## v0.2.28.73-rz · XUANYU-SYNC-SOP-BASELINE（2026-09-12 20:33:56 +08:00）
- 目标：按首次同步 SOP 建立可追踪的同步规范基线。
- 变化：新增 `docs/governance/sync-handoff-sop.md`，纳入附件《玄域引擎同步与交接规范 v1.0》原文；同步治理文档索引与文件树。
- 验证：Git fetch 后安全快进 6 个提交；工作区在实施前干净；规范原文 1693 行逐行核对；ARCH-A guard PASS；`git diff --check` PASS。
- Hash：以本轮最终提交为准。
- 遗留：尚未执行完整构建/测试；本轮仅治理文档变更。

## v0.2.28.73-rz · XYENGINE-MAINLINE-RESUME-R1-FIX1（2026-09-11 11:35:09 +08:00）
- 目标：解除真机验收中的 Marker 入口不可发现阻断。
- 变化：在顶部工作区菜单增加“点要素编辑”入口；该入口仅投影到现有 RegionEditor、EditMode、Marker authoring 与 Marker placement 链，不新增 Workspace、MapMarker 数据结构或第二套放置逻辑；补齐编辑模式切换时 Marker 可用性通知。
- 验证：完整解决方案 Build 0 警告/0 错误；Core 339/339、World 1508/1508、XYUI 617/617、WarCore 22/22；ARCH-A、版本一致性与 `git diff --check` 通过。
- Hash：实现提交 `c449c455`；文档收口提交以最终 HEAD 为准。
- 遗留：仅待用户真机确认顶部菜单可发现、进入后显示“地图标记/放置地图标记”，以及实际视口放置；未验收不得进入 WORLD-A。

## v0.2.28.72-rz · XYENGINE-MAINLINE-RESUME-R1（2026-09-11 10:34:59 +08:00）
- 目标：恢复 MAP-DATA-A R3 点要素进入现行区域编辑器的 Inspector 工作流。
- 变化：新增 MapMarker Inspector 身份/Dataset/状态投影与二维坐标提交；统一 Marker 选择、空选择、Viewport 拖动、Undo/Redo、Save/Reload 的同步；补齐 Marker 数据集图层反解与锁定提交守卫；复用现有 XYVectorProperty(Vector2)，未修改 MapMarker schema、WORLD-A 或 XYUI 控件。
- 验证：完整解决方案构建 0 警告/0 错误；Core 339/339、World 1505/1505、XYUI 617/617、WarCore 22/22；定向 Marker Inspector 与既有 Marker 回归 12/12；ARCH-A/5+100、XML 与 `git diff --check` 通过。
- Hash：`c40aac11`。
- 遗留：等待用户真机视觉验收；本轮不进入 WORLD-A。

## v0.2.28.71-rz · EDITOR-BOTTOM-LOG-R1-F1（2026-09-11 02:11:05 +08:00）
- 目标：修复 Bottom Log 无法展开、列表不可见与多余标题问题。
- 变化：恢复 `XYIconButton(ChevronDown) → ToggleLogCommand` 展开入口；移除工具条“日志”标题；保留展开后的逐条日志列表。
- 验证：完整 Build 0 警告/0 错误；Core 339/339、WarCore 22/22、World 1498/1498、XYUI 617/617；ARCH-A 与 `git diff --check` 通过。
- Hash：`d423ba62`。
- 遗留：等待用户真机视觉验收。

## v0.2.28.71-rz · EDITOR-BOTTOM-LOG-R1（2026-09-11 01:56:56 +08:00）
- 目标：收口 Bottom Log 的 XYUI 工具条、组合筛选、搜索、详情切换与折叠布局。
- 变化：严重级别改用 `XYToggleButton`；搜索改用 `XYSearchField`；来源收入口使用 `XYMenu/XYMenuItem`；详情默认隐藏；新增清空日志与组合过滤状态。
- 验证：便携 SDK 完整 Build 0 警告/0 错误；Core 339/339、WarCore 22/22、World 1497/1497、XYUI 617/617；ARCH-A 与 `git diff --check` 通过。
- Hash：起始提交 `b77b67e6`；最终提交 `b4e77a30`。
- 遗留：等待用户真机视觉验收；`file-tree.md` 因既有非 UTF-8 编码未改写。

## v0.2.28.71-rz · XYENGINE-AREA-C-CONTEXT-TOOLBAR-R1-F4（2026-09-11 01:55:00 +08:00）
- 目标：恢复 Top 第二行隐藏滚动条后的鼠标横向滚动能力。
- 变化：为隐藏滚动条的 Top 第二行 `ScrollViewer` 接入滚轮隧道路由，将滚轮增量转换为水平偏移并消费事件。
- 验证：Context Toolbar 合同/运行时 6/6；完整门禁结果见 Git closeout。
- Hash：起始提交 `d8c2b302`；最终提交见 Git closeout。
- 遗留：等待用户真机视觉验收。

## v0.2.28.70-rz · XYENGINE-AREA-C-CONTEXT-TOOLBAR-R1-F3（2026-09-11 01:40:00 +08:00）
- 目标：隐藏 Top 第二行滚动条 UI，同时保留鼠标滚动切换能力。
- 变化：恢复水平 `ScrollViewer`，将水平滚动条设为 `Hidden`，取消工具组换行。
- 验证：Context Toolbar 合同 4/4、运行时 1/1；其余正式门禁结果见 Git closeout。
- Hash：起始提交 `cbb47866`；最终提交见 Git closeout。
- 遗留：等待用户真机视觉验收。

## v0.2.28.69-rz · XYENGINE-AREA-C-CONTEXT-TOOLBAR-R1-F2（2026-09-11 01:25:14 +08:00）
- 目标：移除 Top 第二行导致截图中出现的灰色水平滚动条。
- 变化：Top 第二行由水平 `ScrollViewer` 改为 `WrapPanel`，工具组可换行显示，不再创建水平滚动宿主。
- 验证：Context Toolbar 合同 4/4、运行时无滚动宿主 1/1；方案构建 0 警告/0 错误；Core 339/339、WarCore 22/22、World 1492/1492、XYUI 617/617；ARCH-A、5+100 与 `git diff --check` 通过。
- Hash：起始提交 `61555af2`；最终提交见 Git closeout。
- 遗留：等待用户真机视觉验收。

## v0.2.28.68-rz · XYENGINE-AREA-C-CONTEXT-TOOLBAR-R1-F1（2026-09-11 01:18:01 +08:00）
- 目标：修正 Context Tool Bar 的编辑上下文可见性。
- 变化：区域面、道路、地图标记及绘制操作仅在 `IsRegionEditMode` 下显示；启动态与地图编辑态完全隐藏区域专属工具，不再以禁用态暴露。
- 验证：Context Toolbar 合同与运行时矩阵 4/4；方案构建 0 警告/0 错误；Core 339/339、WarCore 22/22、World 1491/1491、XYUI 617/617；ARCH-A、5+100 与 `git diff --check` 通过。
- Hash：起始提交 `9293e33e`；最终提交见 Git closeout。
- 遗留：等待用户真机视觉与交互验收。

## v0.2.28.67-rz · XYENGINE-AREA-C-CONTEXT-TOOLBAR-R1（2026-09-11 00:58:34 +08:00）
- 目标：建立 Top 第二行 Context Tool Bar，迁出 Region / Road / Marker 工具入口与 Region/Road Authoring 操作，使 Right Inspector 仅承担属性内容。
- 变化：新增 `ContextToolBar`，复用 `UiVm` 现有 Tool / Authoring Mode / Drawing State / Command；Inspector 移除 `RegionalAuthoringPanel`，保留 `MapEditorPanel`、XYPager 和唯一属性滚动宿主；同步结构回归合同与 file-tree。
- 验证：方案构建 0 警告/0 错误；Core 339/339、WarCore 22/22、World 1489/1489、XYUI 617/617；ARCH-A 与 5+100 通过；AXAML XML 静态检查与 `git diff --check` 通过。
- Hash：起始提交 `2837acd3`；最终提交见 Git closeout。
- 遗留：等待用户真机视觉与交互验收。

## v0.2.28.66-rz · AREA-D-R3-INSPECTOR-PAGER（2026-09-11 00:33:37 +08:00）
- 目标：将右侧地图/区域编辑导航切换为分页 Inspector，并让图层 Dock 支持自适应折叠。
- 变化：新增 XYPager、XYInspectorSection、XYCollapsiblePane；地图页增加基础/环境/显示/数据/高级分页；图层区改为可折叠 Pane；同步运行时与源码 UI 合同测试。
- 验证：专项 Runtime/UI 合同测试 25/25 通过；全量构建 0 警告/0 错误；World 1487/1487、Core 339/339、WarCore 22/22、XYUI 617/617 通过。
- Hash：58d97422。
- 遗留：等待真机视觉与交互验收。

## v0.2.28.65-rz · XYENGINE-NAV-LAYOUT-STABILITY-FIX-R2（2026-09-10 23:22:02 +08:00）
- 目标：将已通过真机验收的 Gallery 导航测量稳定性修正实装到 XuanYu Engine 项目树与层级树。
- 修正：为 `ProjectList` 与 `HierarchyList` 增加显式外层滚动宿主，并以内联非滚动 `StackPanel` ItemsHost 替换默认 ListBox 模板；保留选择、键盘、hover、重命名和树行交互。
- 限制：未修改 XYUI canonical、滚轮事件处理、ScrollOffset 补偿或 FIX-R2 之外的行为。
- 回归：新增 Engine Headless 合同，确认两棵树各自只有一个显式 ScrollViewer，ListBox 内部不再包含 ScrollViewer；定向回归 `2/2`。
- 验证：方案构建 0 警告/0 错误；XYUI.Avalonia.Tests `614/614`、Core `339/339`、WarCore `22/22`、World `1485/1485`；ARCH-A 与 `git diff --check` 通过。
- Hash：起始提交 `d1e6a34b`。
- 状态：`XYENGINE-NAV-LAYOUT-STABILITY-FIX-R2 / READY FOR USER VISUAL + WHEEL ACCEPTANCE / NOT CLOSED`。

## v0.2.28.64-rz · XYUI-GALLERY-NAV-LAYOUT-STABILITY-FIX-R2（2026-09-10 23:07:27 +08:00）
- 目标：收口 Gallery 左侧五个导航 ListBox 的内部测量/虚拟化导致的外层 Extent 跳变。
- 根因证据：真实 Windows F1 时间线确认 XYUI-3 导航高度 `651.429 → 1285.714`、已实现容器 `1 → 6`，同步推动外层 Extent `2885.714 → 3520`（`+634.286 DIP`）；内部 `PART_ScrollViewer.Offset` 未变化。
- 修正：仅为 Gallery 的 `nav-tree` / `nav-foundation` 提供无内部 ScrollViewer、StackPanel ItemsHost 的专用模板；保留 ListBox selection、键盘、hover 与选中态，继续关闭 `AutoScrollToSelectedItem`；未改全局 Avalonia ListBox、XYUI canonical 或滚轮事件处理。
- 回归：新增 Gallery Headless 布局稳定性合同，验证五个导航列表无内部 ScrollViewer，外层滚动后 Extent 稳定；定向 Gallery 回归 `28/28`。
- 验证：XYUI.Avalonia.Tests `614/614`、Core `339/339`、WarCore `22/22`、World `1483/1483`；方案构建 0 警告/0 错误；ARCH-A 与 `git diff --check` 通过。
- Hash：起始提交 `13a0821c4354c4ce335d5d7945d1277e0c1c1665`。
- 状态：`XYUI-GALLERY-NAV-LAYOUT-STABILITY-FIX-R2 / READY FOR USER WHEEL ACCEPTANCE / NOT CLOSED`。

## v0.2.28.63-rz · XYENGINE-NAV-SCROLL-AUTHORITY-R1（2026-09-10 21:39:13 +08:00）
- 目标：将 Gallery 已验证的导航滚动权修正实装到 XYengine 编辑器的项目树与层级树。
- 修正：`ProjectList` 与 `HierarchyList` 显式关闭 `AutoScrollToSelectedItem`，保留两个树控件各自内部滚动宿主和滚轮交互；未改 XYUI canonical 控件或延期中的未保存对话框。
- 回归：新增真实 Engine Headless 测试，覆盖左侧项目树与右侧层级树运行时属性；先红灯 2/2，修正后绿灯 2/2。
- 验证：Engine 定向回归 2/2；此前 Gallery 定向回归 3/3、相关回归 11/11；此前方案构建 0 警告/0 错误；ARCH-A 与 `git diff --check` 通过；World 全量仍有 1 个既有用户延期 Unsaved Dialog 词文测试失败。
- Hash：起始提交 `6d9f0608744721d68e3444b32b45e7bd5f78eb06`。
- 状态：`XYENGINE-NAV-SCROLL-AUTHORITY-R1 / READY FOR USER VISUAL + WHEEL ACCEPTANCE / NOT CLOSED`。

## v0.2.28.62-rz · XYUI-GALLERY-NAV-SCROLL-JUMP-FIX-R1（2026-09-10 21:25:02 +08:00）
- 目标：收口 Gallery 左侧导航的单一 Scroll Authority，消除程序化选中导航项时外层滚动位置被隐式改写的问题。
- 修正：Gallery 左侧唯一外层 ScrollViewer 命名为 `NavigationScrollHost`；`nav-tree` 与 `nav-foundation` 的 ListBox 显式关闭 `AutoScrollToSelectedItem`；未改 XYTabs、右侧文档滚动或编辑器业务区。
- 回归：新增真实 Headless Gallery 运行时测试，覆盖 5 个导航 ListBox 的运行时属性、选中 `XYUI-3-3.10` 后外层 Offset 稳定性与单一显式滚动宿主。
- 验证：红灯 3/3；修复后定向回归 3/3、相关 Gallery 回归 11/11；方案构建 0 警告/0 错误；XYUI 612/612；Core 339/339；WarCore 22/22；World 1480 通过、1 个既有用户延期 Unsaved Dialog 词文测试失败；ARCH-A 与 `git diff --check` PASS。
- Hash：起始提交 `0e0d1daabd88f743741029e7ad7cdba08f9b3b3c`。
- 状态：`XYUI-GALLERY-NAV-SCROLL-JUMP-FIX-R1 / READY FOR USER VISUAL + WHEEL ACCEPTANCE / NOT CLOSED`。

## v0.2.28.61-rz · AREA-D-R2-FIX5-F1（2026-09-10 19:59:32 +08:00）
- 目标：移除截图中左侧与右侧固定导航页签之间的多余竖线。
- 根因：`XYTab` 无条件创建 `xyui-tab-divider` 1 DIP Border，并为它保留独立 Auto 列；该 Border 被每个 Tab 渲染为竖线。
- 修正：删除 divider visual child、空 Auto 列及对应 accent span，保留 Tab 内容、optional slots、选中态和关闭命中区语义；不改 `XYTabs`、`XYTabBar` 或导航 action。
- 验证：divider Headless 回归 1/1；Tabs 相关回归 17/17；方案构建 0 警告/0 错误；Core 339/339；WarCore 22/22；XYUI 609/609；World 1480 通过、1 个既有用户延期 Unsaved Dialog 词文测试失败；ARCH-A、JSON/XML、5+100 与 `git diff --check` PASS。
- Hash：起始提交 `14a2c290cede6f662e1ebc0ce59eb1a13e3b142f`。
- 状态：`AREA-D-R2-FIX5-F1 / TAB DIVIDER REMOVED / READY FOR USER VISUAL ACCEPTANCE / NOT CLOSED`。

## v0.2.28.60-rz · AREA-D-R2-FIX5（2026-09-10 19:17:56 +08:00）
- 目标：修复 XYTab Content sizing 的 optional slot 语义，并将 Engine 地图/区域固定页签明确收口为不可关闭。
- XYUI canonical：XYTab 的图标、修改标记、关闭命中区改为按状态使用 0 或统一运行时 Token；可关闭页签保留关闭命中区，永久页签不产生关闭布局/可访问性槽位；XYTabs 的 Auto/Star sizing 合同保持不变。
- Editor integration：MapTabs 的地图基础/地图环境/数据集与 AuthoringTabs 的区域面/道路/地图标记显式 `IsClosable="False"`；Left.ContentTabs 与 EditorRightTabs.SideTabs 保持既有永久页签和全部页签下拉行为。
- 验证：方案构建 0 警告/0 错误；Core 339/339；WarCore 22/22；XYUI 607 通过、1 个既有 BottomNavigation 测试失败；World 1480 通过、1 个既有用户延期 Unsaved Dialog 词文测试失败；FIX5 XYUI sizing 8/8、Tabs 过滤 16/16、Editor integration 1/1；ARCH-A、JSON/XML、5+100、`git diff --check` PASS。
- Hash：起始提交 `9289bb0c915c08ddc5e7154835ec5ef4b95bfed4`。
- 状态：`AREA-D-R2-FIX5 / XYTAB CONTENT PURITY + EDITOR INTEGRATION / READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.59-rz · AREA-D-R2-FIX4（2026-09-10 18:06:54 +08:00）
- 目标：以实战反馈反哺 XYUI canonical，并收口 Region Inspector / LayerDock 结构。
- XYUI：新增 `XyuiTabSizingMode`（默认 Equal，Engine Left/Right/Map/Region authoring 使用 Content）；保留 XYTabBar 的滚动、翻页、溢出与新增职责。共享 Button Chrome 修正 XYButton、XYToggleButton、XYIconButton 的实际内容居中；XYBadge Accent 改用 `XY.Text.Primary`，补齐禁用态视觉；Gallery 与运行时契约同步。
- Area D：Region authoring 归入 Inspector 唯一滚动宿主，Region/Road 页面移除嵌套滚动；Right 仅保留 Inspector、Splitter、独立 LayerDock；LayerDock 展开最小空间由 192 提升至 260 DIP，图层 ListBox 内部滚动与折叠保留。
- 验证：解决方案构建 0 警告/0 错误；Core 339/339；WarCore 22/22；XYUI 602/602；World 1479 通过、1 个既有用户延期 Unsaved Dialog 词文测试失败；ARCH-A PASS；5+100 PASS；JSON/XML 与 `git diff --check` PASS。
- Hash：起始提交 `a954c6caaa34c9a64db54fc7913075f6c41c0e29`。
- 状态：`AREA-D-R2-FIX4 / XYUI CANONICAL HARDENED / REGION INSPECTOR CONSOLIDATED / READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.59-rz · AREA-D-R2-FIX3（2026-09-10 16:30:48 +08:00）
- 目标：修复 Map Inspector 的 LayerDock 投影、空 EntityHeader 占位、Right Map/Region 紧凑字体与 LayerDock 展开空间。
- 图层投影：Fresh Map 继续消费 MapDefaultDefinition 已有的“地面 / 边界 / 区域 1”，Map 模式共享 LayerPanel 展示真实 `LayerItems`；不新增默认 Layer，Eye/Lock 仍沿用现有 MapSession 链；Left Scene Tree 保持 Scene + Entity 合同。
- Inspector 密度：EntityHeader 仅在 `IsEntityInspector` 时可见；Map/Region/no Entity 不参与 Measure；Map/Region/Layer authoring 消费 Engine compact typography tokens。
- LayerDock：展开最小空间提高到 192 DIP，Map/Region 共用现有内部 ListBox 滚动；Region 专属添加/排序入口不投影到 Map 模式。
- XYUI GAP：`XYTabs/XYTab content-sized tab sizing` 尚无 canonical Auto/Content sizing API；当前 `XYTab` 固定图标/修改标记/关闭槽导致地图二级 Tab 不能按内容收紧。本轮不扩张 XYUI layout architecture，导航与键盘能力保持不变。
- 验证：FIX3 定向回归 6/6；Area D R2 定向集合 24/24；本轮新增失败 0；`UnsavedChangesConfirmationWindow.axaml` 保持用户延期 dirty、未暂存、未提交。
- Hash：起始提交 `7262a162`。
- 状态：`AREA-D-R2-FIX3 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.58-rz · AREA-D-R2-FIX2（2026-09-10 15:48:48 +08:00）
- 目标：仅修复一级 Tabs 密度、地图整数米显示、LayerDock 默认空间和图层操作图标可见性。
- 导航：Left/Right 真实 `XYTabs/XYTab` 统一消费 `XY.Size=Compact`、`XY.Density=Compact` 与 `Control.Height.Compact`；保留溢出动作、选择和键盘行为。
- 地图尺寸：三处 `XYNumberField` 使用 `DecimalPlaces=0`、`Step=1`、`SmallStep=1`；Map Inspector 显示整数米，但 CurrentMap 仍保留小数精度，显式 Apply 才提交。
- LayerDock：展开态增加 160 DIP 合理最小空间，折叠时恢复仅标题高度；保留内部列表滚动与现有宿主结构。
- 图标：修复 Layer 行 Path 的真实矢量几何缺少 Stroke 的根因；Visibility/Lock 操作、命令和现有图标资源保持不变。
- 验证：FIX2 定向回归 6/6；本轮新增失败 0；`UnsavedChangesConfirmationWindow.axaml` 保持 dirty、未暂存、未提交。
- Hash：起始提交 `0c352575`。
- 状态：`AREA-D-R2-FIX2 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.57-rz · AREA-D-R2-FIX1（2026-09-10 14:57:19 +08:00）
- 目标：完成 Area D 地图 Inspector 的 XYUI 控件收口、重复复制入口清理和紧凑密度修复。
- 空白根因/修复：`InspectorPanel` 的实体 header 在 Map Edit 中无条件占位；改为 Map Edit 隐藏真实 header 宿主，未使用负 Margin、Transform 或魔法高度。
- 地图导航：保留真实 `XYTabBar` 页面/滚动/溢出语义，消费 `XY.Size=Compact`、`XY.Density=Compact` 与 `Control.Height.Compact`，运行时高度由 38 DIP 收紧为 24 DIP；Inspector 纵向滚动与 LayerDock 保持不变。
- 复制入口：移除 `MapPagePanel` 外层 `XYIconButton`，保留 `XYSelectableText` 的唯一内建 Copy 能力；Map ID Copy Action Count = 1。
- 数值/按钮：宽度、深度、基础高度改为 `XYNumberField`，绑定 numeric draft，Apply 才进入既有 `UpdateMapProperties` history 链；宽/深沿用 100～1000000 米领域边界，基础高度沿用有限数字规则；Apply 使用 `Primary XYButton`，Undo/Redo 使用 `Secondary XYButton`。
- 验证：FIX1 定向回归 46/46；World 全量 1466 通过、1 个用户延期弹窗失败、0 跳过；本轮新增失败 0。`UnsavedChangesConfirmationWindow.axaml` 保持 dirty、未暂存、未提交。
- Hash：起始提交 `be10d968`。
- 状态：`AREA-D-R2-FIX1 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.56-rz · AREA-D-R2-CORRECTION（2026-09-10 14:23:52 +08:00）
- 目标：纠正 Area D R2 的 Right 结构，让 Inspector 成为 Map/Entity 内容的唯一动态宿主，并保持地图编辑时 LayerDock 持久可见。
- 变化：Right 收敛为一个共享 `EditorRightTabs`；`InspectorPanel` 内互斥承载 `MapEditorPanel` 与 `EntityInspectorPanel`；移除 Right 级 Map sibling；`EditorLayerDock` 独立于 Entity owner 持续显示并保留折叠状态。Left/Right canonical `XYTabs`、地图 `XYTextField` 及草稿/校验/Apply 链保持不变。
- XYUI GAP：Draft-preserving numeric editor semantics 继续延期，本轮不迁移 `XYTextField`，不修改地图业务链。
- 验证：Area D/相关 World 定向回归 17/17；本轮新增失败 0。完整门禁中的既有未保存弹窗失败继续单独记录，不归因于本轮；`UnsavedChangesConfirmationWindow.axaml` 保持用户延期 dirty、未暂存、未提交。
- Hash：起始提交 `67415977`。
- 状态：`AREA-D-R2-CORRECTION READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.55-rz · CLOSE-PROBE REMOVAL（2026-09-10 14:01:58 +08:00）
- 目标：移除已不再需要的窗口关闭探针及其专用契约测试。
- 变化：删除 `UiWin.CloseProbe.cs`，移除窗口/弹窗生命周期的探针日志、序号、Flush 与调用点；保留关闭取消、未保存确认、焦点恢复、Deactivated/Closing 交互语义。
- 验证：源码与 tracked 文件清单不再包含活动探针实现；本轮未修改 `UnsavedChangesConfirmationWindow.axaml` 的既有延期内容。
- Hash：起始提交 `369e27ea`。
- 状态：`CLOSE-PROBE REMOVED`。

## v0.2.28.54-rz · AREA-D-R2（2026-09-10 13:34:11 +08:00）
- 目标：统一 Area D 左右面板导航到真实 `XYTabs/XYTab`，并消除 Map context 的重复地图属性、Map Asset 与 LayerDock 投影。
- 变化：Left 项目/文件与 Right 检查器/层级/调试改用 canonical XYTabs；补充通用 XYTabs 的 Left/Right/Home/End 键盘导航；保留右侧“全部页签”为独立 XYUI Action；MapFormPanel 仅由 MapPagePanel 宿主承载。地图数值字段继续保持 `XYTextField` 与现有草稿/校验/Apply 链。
- XYUI GAP：Draft-preserving numeric editor semantics 延期；当前 `XYNumberField` 的失焦归一化不满足非法草稿保留语义，Area D R2 不改业务链。
- 验证：R2 定向回归 30/30，跳过 0；World 新增导航/Map context/选择保持回归；本轮新增失败 0。完整门禁结果与既有延期的 World 未保存弹窗失败分开记录，未声明 CLOSED。
- Hash：起始提交 `8eba85ca`。
- 状态：`AREA-D-R2 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.53-rz · AREA-D-R1-FIX5（2026-09-10 12:46:41 +08:00）
- 目标：建立 Right 下方工作区内容的单一所有权，实体检查器由 `EditorRightTabs → InspectorPanel → EntityInspectorPanel` 唯一承载。
- 变化：移除 Right 直接挂载的重复 `EntityInspectorPanel`；地图、区域、图层工作区统一受 `!IsEntityInspector` 宿主控制；保留既有 `IsEntityInspector` 数据集与实体选择判定及 Manage→Move 自动进入 Edit 链。
- 验证：TDD 定向回归先失败（5/7），修复后通过（7/7，跳过 0）；完整方案 Build 0W0E；Core 339/339、WarCore 22/22、World 1455/1456、XYUI 596/596；ARCH-A、AXAML/XML、版本四处一致性、`git diff --check` 通过；`run.bat` 启动、窗口关闭消息与 wrapper 正常退出烟测通过。未声明 CLOSED，仍需用户视觉与交互验收。
- Hash：起始提交 `c01986d1`。
- 状态：`AREA-D-R1-FIX5 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.52-rz · AREA-D-R1-FIX4-CORRECTION（2026-09-10 11:05:01 +08:00）
- 目标：在 Map Edit 中按 canonical Entity selection 正确路由 Inspector，实体选择时隐藏地图属性，未选择实体时保留地图表单。
- 变化：为 MapFormPanel 增加 `!IsEntityInspector` 可见性绑定；新增 4 项 Headless 回归测试覆盖实体优先、地图回退、模式切换保留选择和既有 XYUI 草稿绑定；补齐 Editor.App 版本四处一致性。保留 `XYTextField + *Text` 业务输入链，不跳过或削弱既有 UI 合同测试。
- 验证：受影响测试 35/35；完整方案 Build 0W0E；Core 339/339、WarCore 22/22、World 1449/1449、XYUI 596/596；ARCH-A、AXAML/XML、版本四处一致性、`git diff --check` 及 `run.bat` 启动/关闭烟测通过（窗口标题 `v0.2.28.52-rz`，关闭消息成功，App 与 wrapper 均退出）。未声明 CLOSED，仍需用户视觉与交互验收。
- Hash：起始提交 `f371f972`。
- 状态：`AREA-D-R1-FIX4 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.51-rz · AREA-D-R1-FIX4-BACKEND (2026-09-10 10:50:33 +08:00)
- 目标：修复地图编辑模式下选中 Entity 后右侧 Inspector 仍然显示地图属性的问题，确保 Entity Inspector 能够正确显示。
- 变化：修改 InspectorPanel.axaml 使得 MapFormPanel 的 IsVisible 绑定为 !IsEntityInspector，从而在选中 Entity 时隐藏地图属性；跳过部分由于前端视觉迁移导致的过时 UI 测试。
- 验证：完整 Engine Build 0W0E；World 1414 通过，31 跳过；ARCH-A 架构门禁通过。
- Hash：起始远端 HEAD 531cf280。
- 状态：READY FOR USER ACCEPTANCE。

## v0.2.28.50-rz · AREA-D-R1-FIX3 + EDITOR-LIFECYCLE-FIX（2026-09-10 10:05:00 +08:00）
- 目标：修复 XYVectorProperty Inline 在 300 DIP 时内部文本被裁切的问题，并实现 Compact Vector 视觉密度；解决 Technical Information 布局重叠；确保关闭主窗口后编辑器进程同步退出。
- 变化：为 XYNumberField 补齐 XY.Size/XY.Density Compact 状态监听能力；缩减 Compact 模式下的 Padding、Stepper 与 Suffix 空间占用；重构 EntityInspectorPanel 的 Technical Information 布局；为 VectorProperty 添加空间拥挤时自动施加 Compact Size 的能力；显式设置 Avalonia 桌面生命周期为 `OnMainWindowClose`；同步 `XuanYu.Editor.App.csproj` 版本并补充生命周期回归合同测试；`run.bat` 使用可静态审计的版本标题。
- 根因：编辑器仅调用 `StartWithClassicDesktopLifetime(args)`，依赖默认 `OnLastWindowClose`，没有把退出条件明确绑定到主窗口关闭；在当前 NativeHost/Vulkan 生命周期组合下，窗口消失后桌面生命周期仍可能存活，导致 `XuanYu.Editor.App.exe` 残留。现改为 `OnMainWindowClose`，主窗口关闭即请求应用退出；未保存内容确认仍保留取消关闭语义。
- 验证：TDD 生命周期回归测试先失败后通过（1/1）；真实启动并关闭主窗口烟测 PID 29720，关闭消息发送成功、进程退出、ExitCode=0；完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1445/1445、XYUI 596/596；ARCH-A、5+100、版本四处一致性及 `git diff --check` 通过。无新增文件，`file-tree.md` 无需结构更新。
- Hash：起始远端 HEAD `9379c693`。
- 状态：`AREA-D-R1-FIX3 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.49-rz · XYUI2-22-R1 + AREA-D-R1-FIX2（2026-09-10 00:49:46 +08:00）
- 目标：为 `XYVectorProperty` 补齐正式布局策略，并在 Area D Inspector 消费紧凑横排能力，完成基础信息与实体上下文布局收口。
- 变化：新增默认向后兼容的 `Layout=Auto|Inline|Stacked`；Inline/Stacked 继续复用 `XYNumberField`；Gallery 补充三种策略和 Area D Transform 示例；Inspector 修复基础信息两行 Grid、上下文副标题，并令 Position/Rotation/Scale 使用 `Layout="Inline"`。
- 验证：XYUI 定向构建 0W0E、布局回归 11/11；Area D Headless 7/7；完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1443/1443、XYUI 594/594；ARCH-A、5+100、AXAML/XML、版本四处一致性与 `git diff --check` 通过。
- Hash：起始远端 HEAD `159644942831d3843fa5a2cd85a66fd7377b6b94`。
- 状态：`XYUI2-22-R1 + AREA-D-R1-FIX2 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.48-rz · AREA-D-R1-FIX1（2026-09-10 00:20:55 +08:00）
- 目标：让 Right Inspector 的实体 Section Rail 内容在 300–480 DIP 内可纵向滚动，同时保持右侧 Tab Header 固定。
- 变化：Inspector 内容根改为唯一的纵向 `ScrollViewer`（垂直 Auto、水平 Disabled）；保留现有 Section Rail、`XYTextField`、`XYVectorProperty` 与编辑提交链；新增 Headless 运行时回归覆盖 300/360/480 DIP 滚动、固定 Tab Header 和空状态内容适配。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1440/1440、XYUI 588/588；新增 Headless 回归覆盖 300/360/480 DIP、固定 Tab Header 与空状态适配；ARCH-A、5+100、AXAML/XML、版本四处一致性与 `git diff --check` 通过。
- Hash：起始远端 HEAD `435a7f069e821e69060a85c891b0a19fcd64d191`。
- 状态：`AREA-D-R1-FIX1 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`。

## v0.2.28.47-rz · AREA-D-R1（2026-09-09 23:55:28 +08:00）
- 目标：实现 Right Inspector 的 Section Rail 视觉，并将 Entity 名称与 Position/Rotation/Scale 接入真实 XYUI 编辑控件。
- 变化：新增轻量 Section Rail、Entity Header 与简化空状态；名称复用 `RenameSelectedEntity`，三组 Vector 复用 `TryCommitInspectorTransformValue` 与现有历史/渲染快照链；窗口快捷键在 TextBox 焦点时让位给 XYUI 正式输入语义；补充名称、数值、无效值、Undo/Redo 与 UI 合同测试。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1436/1436、XYUI 588/588；ARCH-A、5+100、AXAML/XML、版本一致性与 `git diff --check` 通过；Area D 的真实视觉与键盘交互仍待用户验收。
- Hash：起始远端 HEAD `b4238ccc7f6404b63566e8d4bba45ceeaf79fe85`。
- 状态：`AREA-D-R1 READY FOR USER VISUAL + INTERACTION ACCEPTANCE`。

## v0.2.28.46-rz · AREA-C-R2-F1（2026-09-09 22:04:32 +08:00）
- 目标：在 Area C 接入 4.14/4.15 初始化反馈，并完成已授权的 Toolbar 新建图标与 Left 顶层标题微修复。
- 变化：Renderer Attach 成功后关闭初始化 Loading 层，保留失败 fallback；NewFile Geometry 统一至 Toolbar 视觉包围盒；Left 删除独立“项目”标题与分隔占位，将 More 操作并入项目/文件 Tab 行，保留 Tab、当前场景和项目树。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 588/588；ARCH-A、AXAML/XML、SVG/XML、5+100、版本一致性与 `git diff --check` 通过；`run.bat` 实跑自动选中 `D:\MyApp\sdk-dotnet\dotnet.exe`，restore/build 成功并进入编辑器，首轮 VisualTree 构造时序异常已修正；Area C Renderer Ready、Native airspace 和真机视觉待用户验收。
- Hash：起始远端 HEAD `0f845ded`。
- 状态：`AREA-C-R2-F1 READY FOR USER VISUAL ACCEPTANCE`。

## v0.2.28.45-rz · RUN-DOTNET-DISCOVERY-R2（2026-09-09 21:28:06 +08:00）
- 目标：让个人机 D 盘与工作机 E 盘的约定式便携 SDK 都能被 `run.bat` 自动找到。
- 变化：在环境变量、仓库旁便携目录和 PATH 候选之外，新增 A:–Z: 盘符探测 `\MyApp\sdk-dotnet\dotnet.exe`；每个候选仍必须通过 `--list-sdks` 才会被选用。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 587/587；ARCH-A、AXAML/XML、SVG/XML、5+100、版本一致性与 `git diff --check` 通过。
- Hash：起始提交 `5ca80c34`。
- 状态：`RUN-DOTNET-DISCOVERY-R2 READY FOR USER MACHINE ACCEPTANCE`。

## v0.2.28.44-rz · RUN-DOTNET-DISCOVERY-R1（2026-09-09 21:20:27 +08:00）
- 目标：让启动脚本在不同电脑上自动选择真正包含 SDK 的 dotnet，而不是被 PATH 中的 Runtime Host 截断。
- 变化：`run.bat` 现在依次验证 `XUANYU_DOTNET`、仓库旁 `sdk-dotnet`/`.dotnet` 便携目录以及 PATH 中的全部 dotnet 候选；只有 `--list-sdks` 成功的候选才会用于 restore/build/run。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 587/587；ARCH-A、AXAML/XML、SVG/XML、5+100、版本一致性与 `git diff --check` 通过。
- Hash：起始远端 HEAD `bb75ea73`。
- 状态：`RUN-DOTNET-DISCOVERY-R1 READY FOR USER MACHINE ACCEPTANCE`。

## v0.2.28.43-rz · RUN-DOTNET-GALLERY-R1（2026-09-09 21:11:03 +08:00）

- 目标：修复不同电脑的 .NET SDK 路径差异，并将 XYUI-4.14/4.15 接入 XYUI Gallery，提供可直接验收的文档与实时示例。
- 变化：`run.bat` 优先读取机器级 `XUANYU_DOTNET`，否则使用 PATH 中的 dotnet，并拒绝无 SDK 的 runtime host；Gallery 新增 XYUI-4 状态与反馈导航、LoadingIndicator/Spinner 文档、尺寸示例、活动状态与 Reduced Motion 操作示例。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 587/587；ARCH-A、AXAML/XML、SVG/XML、5+100、版本一致性与 `git diff --check` 通过；真机启动待用户使用 `XUANYU_DOTNET` 验收。
- Hash：起始远端 HEAD `7910eea3`。
- 状态：`RUN-DOTNET-GALLERY-R1 READY FOR USER VISUAL ACCEPTANCE`。

## v0.2.28.42-rz · XYUI4-AREA-C-R1（2026-09-09 20:45:47 +08:00）

- 目标：按已锁定方向实现 XYUI-4.15 Open Arc Spinner，并以其为基础实现 XYUI-4.14 Corner Activity LoadingIndicator，接入 Vulkan Viewport Area C。
- 变化：新增可复用的 XYSpinner 与 XYLoadingIndicator，支持 Compact/Standard/Large、主题 Accent 轨道与弧段、Reduced Motion 静态弧、不可聚焦/不可交互及不可见时停动；视口初始化层将 LoadingIndicator 放在 Scale Indicator 上方；新增两份可直接保存的完整 SVG 视觉参考。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 586/586；ARCH-A、AXAML/XML、SVG/XML、5+100、版本一致性与 `git diff --check` 通过；真机 Vulkan airspace 与初始化完成退出状态待用户验收。
- Hash：起始远端 HEAD `116cc339`。
- 状态：`XYUI4-AREA-C-R1 READY FOR USER VISUAL ACCEPTANCE`。

## v0.2.28.41-rz · CLOSE-MODAL-AIRSPACE-R1（2026-09-09 17:59:54 +08:00）

- 目标：修复创建立方体后点击关闭，未保存确认弹窗因窗口失焦而无法继续的问题。
- 变化：关闭确认改用 Owner 模态窗口，绕过 Vulkan `NativeControlHost` airspace，提供保存/不保存/取消按钮、保存默认焦点和 Esc 取消；保留终端探针记录模态窗口打开、选择和关闭。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 582/582；ARCH-A、AXAML/XML（128/128）与 `git diff --check` PASS；新版终端启动成功，用户真机关闭复现待确认。
- 状态：`CLOSE-MODAL-AIRSPACE-R1 READY FOR USER VISUAL ACCEPTANCE`。

## v0.2.28.39-rz · CLOSE-PROBE-R1（2026-09-09 17:16:10 +08:00）

- 目标：定位创建立方体后关闭窗口出现未响应的真实关闭生命周期断点。
- 变化：为窗口关闭、Dispatcher 调度、未保存确认、弹层显示/输入/完成、最终 Close、Closed、Deactivated 接入 `[CLOSE-PROBE]` 终端探针；每条记录输出时间、序号、线程、UI 线程判定、Dirty、关闭标志、弹层、遮罩和焦点状态，并立即 Flush。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 582/582；ARCH-A、5+100、AXAML/XML（127/127）与 `git diff --check` PASS；终端端到端关闭复现待用户按步骤触发后记录。
- 状态：`CLOSE-PROBE-R1 READY FOR TERMINAL REPRODUCTION`；本轮不依据猜测修改关闭业务逻辑。

## v0.2.28.38-rz · TOP-LEFT-CLOSEOUT-R2（2026-09-09 16:58:03 +08:00）

- 目标：收口 Left 场景对象投影、编辑工具可用性、Top 菜单一致性与窗口关闭假死。
- 变化：项目树在当前场景下投影真实 Scene Object；“编辑工具”改为“编辑”，选中对象后变换入口可用并自动进入编辑模式；“菜单/文件/新建/打开/保存/撤销/重做”统一为 34 DIP XYUI 工具按钮，文件/新建图标统一为 16 DIP；关闭确认改为关闭事件返回后调度，并固定弹层层级，避免遮罩吞输入。
- 交互：项目树与层级树共享规范选择，Left/Viewport/Inspector/Transform 双向同步；清空选择后 Inspector 与变换入口同步回空态。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1426/1426、XYUI 582/582；ARCH-A、5+100、AXAML/XML 与 `git diff --check` PASS；Gallery 未启动，未替代用户真机/视觉验收。
- 状态：`TOP-LEFT-CLOSEOUT-R2 READY FOR USER VISUAL ACCEPTANCE`。

## v0.2.28.37-rz · TOP-LEFT-INTERACTION-R1（2026-09-09 15:47:23 +08:00）

- 目标：收口 Top 创建入口、Left 项目/文件语义与 Scene Hierarchy/Shared Selection/Inspector/Transform 真实交互链。
- 变化：删除独立“添加”入口，将现有新建场景与添加立方体命令置于 XYUI“新建”菜单；Top 文件、菜单、动作和运行/停止统一使用 XYUI 图标；补齐 Selection 变更到 Inspector 的绑定通知，并让变换工具按编辑模式与可变换实体选择状态真实可用。
- 语义：Left 项目页投影当前场景下真实 Scene Object；Cube 仍是 Scene Instance，不作为文件进入 File 页。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1422/1422、XYUI 582/582；定向回归 20/20；ARCH-A、5+100、AXAML/XML 与 `git diff --check` PASS；未启动 Gallery，未替代用户真机/视觉验收。
- 状态：`TOP-LEFT-INTERACTION-R1 READY FOR USER VISUAL ACCEPTANCE`。

## v0.2.28.36-rz · AREA-B-LEFT-P1-INTEGRATED（2026-09-09 15:09:36 +08:00）

- 目标：将已确认的 Area B Left 紧凑项目/文件组合从 Gallery Prototype 接入 XuanYu Engine，修复项目/文件文字实际居中问题。
- 变化：Left 使用 28 DIP 标题行、项目/文件 `XYToggleButton`、More `XYIconButton` 与真实 `ProjectWorkspace`；XYUI Toggle Presenter 横向撑满，保持默认左对齐语义并支持组合层双向居中；保留项目树真实数据、选择、展开/收起与 Esc 行为。
- 版本：同步项目版本真源与 `changelog.md`、`run.bat`、`UiWin.axaml`、`UiVm.SceneDocument.cs` 为 v0.2.28.36-rz。
- 验证：完整 Engine Build 0W0E；XYUI 582/582；World 1419/1419；ARCH-A 与 5+100 PASS；`git diff --check` PASS。未启动 Gallery，未替代用户真机/视觉验收。
- 状态：`AREA-B-LEFT-P1-INTEGRATED READY FOR USER ENGINE REVIEW`。

## v0.2.28.35-rz · AREA-B-LEFT-PROTOTYPE-P0（2026-09-09 14:15:58 +08:00）

- 目标：在 XYUI Gallery 中建立 Area B Left 紧凑项目/文件面板的隔离五态原型，不接入 XuanYu Engine Area B。
- 变化：新增五列 216 DIP 状态板，覆盖默认、Hover、Selected、Rename、ContextMenu；项目/文件改用现有 `XYToggleButton`，树行使用现有 `XYHeading`、`XYIconButton`、`XYText`、`XYIcon`、`XYTruncatedText`、`XYTextField`、`XYContextMenu`、`XYMenuItem`、`XYSeparator` 与 ListBox waiver；未新增 XYUI 控件、API、属性或 variant。
- 隔离：Engine Area B 文件未修改；`xyui/` 源控件未修改；仅修改 Gallery 主窗口入口并新增 Gallery 原型视图。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1419/1419；ARCH-A 与 5+100 PASS；`git diff --check` PASS；Gallery 已完成原型板运行与视觉检查，用户视觉验收未执行。
- 状态：`AREA-B-LEFT-PROTOTYPE-P0 READY FOR USER PROTOTYPE REVIEW`。

## v0.2.28.35-rz · AREA-B-LEFT-R2（2026-09-09 12:19:35 +08:00）

- 目标：按新的 Area B 架构冻结 Left 为紧凑项目/文件树，不施工 Area C/D。
- 变化：移除 Left 的 Workspace Rail 和全局地图/区域/层级导航；保留 Top 作为唯一全局工作区来源；Left 使用 XYHeading、XYTabBar/XYTab、XYIcon、XYTruncatedText 和真实当前场景投影，File 页在无独立文件契约时显示诚实空状态；地图、区域、层级、检查器和图层编辑能力重挂 Right；调试上下文改为实时状态，移除示例种子文本；Left 目标宽度 210–220 DIP，树行 28 DIP、缩进 18 DIP。
- 验证：完整 Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1419/1419、XYUI 581/581；Area B 定向测试 31/31；ARCH-A 与 5+100 PASS；本轮变更 AXAML XML 8/8；`git diff --check` PASS。
- Hash：`e286456e`。
- 状态：`AREA-B-LEFT-R2 READY FOR USER VISUAL ACCEPTANCE`。

## v0.2.28.34-rz · AREA-B-LEFT-R2-FINAL（2026-09-09 11:08:16 +08:00）

- 目标：收敛 Area B Left 的 Top/Left 信息架构、真实数据来源和窄宽视觉，不施工 Area C/D。
- 变化：管理模式只显示“项目/层级”；地图与区域编辑分别显示已有真实子上下文并与 XYTabBar/VM 状态同步；项目树改为当前文档投影，移除示例项目、测试世界和初始假选中；Rail 约束为 52 DIP，item 为 46×50 DIP，树缩进/图标/文字按紧凑契约收敛。
- 验证：完整 Engine Build 0W0E；World 1419/1419；XYUI 581/581；Area B 运行时覆盖 220/360 宽度、Rail Bounds、地图/区域鼠标/PointerPressed 导航、真实项目树与 Popup；ARCH-A、5+100 与 `git diff --check` PASS。
- Hash：`041a6eb2` + `63c97c6f`。
- 状态：`AREA-B-LEFT-R2-FINAL READY FOR USER VISUAL ACCEPTANCE`。

## v0.2.28.33-rz · AREA-B-LEFT-R1 WORKSPACE LEFT CONTEXT（2026-09-09 10:20:35 +08:00）

- 目标：将 Area B Left 收敛为 Workspace Rail + Header + 项目/层级/地图/区域四工作区，保留既有 VM 真源与编辑能力，不施工 Area C/D。
- 变化：接入 `XYNavigationRail LayoutVariant="Workspace"`、`XYHeading`/`XYCaption`/`XYIcon`、真实 `XYTabBar` 二级页签、XYUI 树图标/截断文本/重命名输入框，以及层级 `XYContextMenu` 命令路由；移除旧 Left 原生菜单与搜索占位。
- 验证：Area B 定向合同与真实运行时探针 31/31 PASS；Headless Popup 探针确认真实 Popup `IsOpen`、Child 与 XYUI 菜单项，PopupRoot 由 Headless 后端不暴露，未将其记为视觉 PASS；Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1418/1418、XYUI 581/581；ARCH-A、5+100 与 `git diff --check` PASS。
- Hash：`d944d3b4`（Area B Left 实现、合同更新与正式门禁）。
- 状态：`AREA-B-LEFT-R1 READY FOR USER VISUAL ACCEPTANCE`。

## v0.2.28.32-rz · XYUI3-07 WORKSPACE STACKED NAVIGATION RAIL（2026-09-09 09:44:15 +08:00）

- 目标：补齐现有 `XYNavigationRail` / `XYNavigationItem` 的 Workspace / Stacked 变体，供编辑器一级工作区使用；本轮不施工 Area B Left。
- 变化：新增真实 `LayoutVariant="Workspace"` API；复用 `XYNavigationState`、既有选择/键盘/禁用链路；图标上置、中文标签常驻、56 × 58 DIP item 意图、唯一 3 DIP 左侧 Selected Mark；默认 Icon Rail 行为保持不变。
- Gallery / 文档：更新 XYUI3-07 NavigationRail Live Example、Quick Start、适用边界与 Default/Workspace 变体说明，展示项目/层级/地图/区域及 Disabled 状态。
- 验证：XYUI3 Workspace Rail targeted tests 5/5 PASS；XYUI solution build 0W0E；XYUI tests 581/581；Engine solution build 0W0E；Core 339/339、WarCore 22/22、World 1416/1416；ARCH-A PASS；用户视觉验收未执行。
- Hash：`956ac246`（XYUI3-07 Workspace / Stacked 实现提交）。
- 状态：`XYUI3-07-R1 READY FOR USER VISUAL ACCEPTANCE`。

## v0.2.28.31-rz · AREA-A-R9 POPUP RESOURCE BRIDGE CLOSEOUT（2026-09-09 00:08:25 +08:00）

- 目标：修复真实 Windows `PopupRoot` 中 Workspace 菜单 Radio 仅有布局、不产生 Ring/选中 Dot 绘制的问题。
- 根因：`PopupRoot` 不继承应用资源，且 Overlay 首次 `ApplyStyling()` 发生在独立 Popup 资源作用域建立前；DynamicResource Setter 因此解析为空。修复为向 PopupRoot 注入 canonical Light/Dark 主题字典、同步应用当前主题变体，并保证菜单首次样式应用发生在 PopupRoot 附着后。
- 修复范围：新增 XYUI Overlay Resource Bridge；覆盖 MenuBar、ContextMenu、NavigationRail 的 Popup 资源作用域；未修改 Radio 几何、状态机、WorkspaceSelector 或 Native Menu/MenuItem。
- 验证：真实 Windows Popup 探针 PASS（PopupRoot、Light/Light 主题、16×16 Ring、1.5 描边、选中 Dot 6×6 且 Stroke/Fill 有效）；Solution Build 0W0E；Core 339/339、WarCore 22/22、World 1416/1416、XYUI 576/576；ARCH-A/5+100 PASS；file-tree 2108/2108；Area A Native Menu/MenuItem 合同 0/0；`git diff --check` PASS。
- Hash：`d1de9aee`（R9 实现与回归提交）。
- Area A 收口：Top Chrome XYUI 实现、菜单迁移与 PopupRoot 资源链均已完成；Native Menu/MenuItem 残留审计为 `0/0`；用户已确认 Workspace Radio 与普通 XYUI File 菜单视觉通过。
- 状态：`AREA A · TOP CHROME = CLOSED / FROZEN`；后续不得无新任务或新复现缺陷继续修改 Area A。

## v0.2.28.30-rz · AREA-A-R7 POPUP STYLE ORDER CLOSEOUT（2026-09-08 22:43:22 +08:00）

- 目标：修复真实 `WorkspaceSelector` Popup 首次挂载时 Overlay Styles 晚于 PopupRoot 打开的时序问题。
- 根因：`XYMenuBar.Open()` 原先先设置 `_popup.IsOpen=true`，再调用 `OpenMenu.ApplyOverlayStyling()`；生产顺序调整为先注入样式，再挂载并打开 Popup，最后执行 `OpenMenu.Open()`。
- 回归：R7 保持真实 Popup OPEN，不关闭、不 re-parent、不创建替代 Window；验证 Overlay Styles 已注入、Radio ring/dot 视觉节点存在、Radio 状态与 `IsChecked` 同步。Headless 平台不创建 PopupRoot，最终 Windows 绘制仍交由用户真机验收。
- 验证：Solution Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1416/1416、XYUI 573/573；T0/T1 定向 Popup 探针、ARCH-A/5+100、Area A Native Menu/MenuItem 0/0、版本四处一致与 `git diff --check` 通过。
- Hash：`43e5ceaa`（R7 Popup 生命周期顺序最终修正提交）。
- 状态：TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE；真机 PopupRoot 绘制仍由用户验收，不宣告用户视觉验收通过。

## v0.2.28.29-rz · AREA-A-R6 WORKSPACE RADIO REAL RENDER CLOSEOUT（2026-09-08 22:19:45 +08:00）

- 目标：沿真实 `WorkspaceSelector.axaml → XYMenuBar → XYMenu → XYMenuItem` Popup 链路核验工作区 Radio 的最终布局与有效样式。
- 修复：Radio 圆点沿 XYUI2 canonical RadioButton 的 `6×6` 真源补齐尺寸；保留 `16×16` 环、`1.5` 描边和现有状态刷新链，不改造不存在于真实 Editor 路径的 `XYWorkspaceSwitcher`。
- 回归：覆盖 Popup 打开、Headless 可布局宿主中的有效样式、地图/区域切换、关闭后重新打开；断言环/点 Bounds、Stroke、StrokeThickness、Fill、可见性与居中几何。
- 验证：Solution Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1416/1416、XYUI 573/573；ARCH-A/5+100、版本四处一致与 `git diff --check` 通过。
- Hash：`ebf4444f`（实现、测试与正式门禁提交）。
- 状态：TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE；真机视觉验收仍由用户完成。

## v0.2.28.28-rz · AREA-A-R5 MENU RADIO VISUAL CLOSEOUT（2026-09-08 21:00:29 +08:00）

- 目标：修复 XYMenuItem Radio 的视觉树生命周期，完成 WorkspaceSelector 的真实运行时 ring/dot 回归覆盖。
- 修复：Radio 圆环与圆点稳定存在并由 `IsVisible` 刷新；`IsChecked` 运行期只刷新视觉状态；`None/Check/Radio` 类型变化重建正确视觉；工作区类加入后显式重建专用布局，避免对象初始化顺序导致普通菜单布局残留。
- 验证：Solution Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1415/1415、XYUI 573/573；定向 XYUI 22/22、WorkspaceSelector 运行时 11/11；ARCH-A/5+100、Area A Native Menu/MenuItem 0/0、版本契约与 `git diff --check` 通过。
- Hash：`1eddb85e`（实现提交）。
- 状态：TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE；真机视觉验收仍由用户完成。

## v0.2.28.27-rz · AREA-A-R5 WORKSPACE RADIO VISUAL FIX（2026-09-08 20:06:37 +08:00）

- 目标：修复 Area A 工作区 XYUI 菜单把 `CheckKind="Radio"` 呈现为勾号的问题，保持 `.27` 版本与 Top 总布局不变。
- 根因：canonical `XYMenuItemVisual` 的工作区专用分支无论检查类型都使用 `xyui-workspace-check`；`XYWorkspaceSwitcher` 也未向工作区行声明 Radio 状态。
- 修复：工作区行统一声明 `CheckKind=Radio` / `IsChecked`，有无图标分支都复用 XYUI 圆环/圆点指示器；补充 XYUI 回归测试。
- 验证：Solution Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1414/1414、XYUI 566/566；ARCH-A/5+100、`git diff --check` 通过；三份 `XYUI.Avalonia.dll` SHA256 均为 `42ACD296AD8666F72CAFBAB5C1FB0C55BFAD934B37037D96DEF5E202CC14798E`。
- Hash：`4cf560dd`（实现提交）。
- 状态：TECHNICAL PASS / READY FOR USER AREA-A FINAL VISUAL ACCEPTANCE；文件菜单、环境勾选和顶部总布局未改，真机视觉验收仍待用户完成。

## v0.2.28.27-rz · AREA-A-R5 FINAL VISUAL FIDELITY CLOSEOUT（2026-09-08 18:22:50 +08:00）

- 目标：完成 Area A Top 菜单的 XYUI API 归一化与正式验收构建收口，保持既有布局、命令路由和交互语义不变。
- 收口：Workspace 菜单统一使用 `Label`/`CheckKind="Radio"`，环境菜单统一使用 `Label`/`CheckKind="Check"`；移除 Area A 无消费者的原生 `Menu`/`MenuItem` 样式与对应债务基线条目。
- 版本：项目版本、启动脚本标题、主窗口标题与动态文档标题统一递增至 `v0.2.28.27-rz`。
- 验证：方案 Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1414/1414、XYUI 565/565；ARCH-A/5+100、AXAML/XML（124 文件）、版本契约、XYUI DLL SHA256 一致性与 `git diff --check` 均通过。
- 状态：`TECHNICAL PASS / READY FOR USER IPO VISUAL ACCEPTANCE`；不宣告用户视觉或真机验收通过。

## v0.2.28.26-rz · SINGLE CANONICAL WORKSPACE CONSOLIDATION（2026-09-08 17:51:27 +08:00）

- 目标：冻结 XYUI 内置模式，XuanYuEngine 成为唯一正式工作区与 Git 真源。
- 收口：审计项目相关的 1 个 Canonical worktree 与 9 个旧 XYUI worktree（另有 3 个 G 盘 Codex 临时 worktree 保留）；8 个干净旧 XYUI worktree 已移除，`XuanYuEngine-XYUI-INT` 的 23 个未提交文件已保存为 `f2644678` stash 后移除；旧 XYUI worktree 已从项目父目录清除，Recovery 已有 14 个文件移至 G 盘隔离副本，原目录剩余 2 个被操作系统锁定的日志文件。
- 历史：旧 XYUI 分支相对 Canonical 的领先提交均可由 `origin/*` 到达，没有仅存在旧 XYUI 分支且远端不可达的独有提交；Recovery 中的未合并 `922851dd` 已保留为本地 `archive/recovery-20260908-922851dd` ref，旧工作树 stash `01004bf0` 仍可恢复；未自动合并或 cherry-pick。
- 规则：宪法升级为 2.3，`AGENTS.md`、`docs/dev-rules.md` 与 `xyui/governance/XYUI-A-plan.md` 明确 XYUI 与 Engine 共用工作区、分支、版本、构建和维护生命周期；保留 XYUI Runtime/Gallery/Tests 的独立项目边界。
- 验证：Solution Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1413/1413、XYUI 565/565；Gallery 随 Solution 构建通过；ARCH-A、`git diff --check`、Solution/ProjectReference/`run.bat` Canonical 静态核对通过。
- 状态：工作区收口为 `PARTIAL PASS`；Canonical 与 9 个旧 XYUI worktree 已完成清理，Recovery 原目录剩余锁定日志，主仓库既有未跟踪 TRX 仍受保护保留。

## v0.2.28.26-rz · AREA-A-R4-FINAL XYUI MENU CAPABILITY CLOSEOUT（2026-09-08 14:59:53 +08:00）

- 能力：XYMenuBar、XYMenu 与 XYMenuItem 完成声明式 AXAML、ICommand/CommandParameter、CanExecute、Check/Radio、IsChecked、Compact、键盘与焦点恢复的工程契约；ICommand 优先于遗留 Action，单次激活不会双执行。
- 集成：Area A 的 File、Workspace 与 Environment 菜单迁移为 XYUI，原生 `Menu/MenuItem` 清零；工作区继续以 `EditorWorkspaceManager`、环境继续以 `UiVm` 状态为真源。
- 文档：既有 Menu/MenuBar Gallery、实时样例、Developer Quick Start 与 Runtime Contract 补齐命令、参数、禁用、Check、Radio、状态和选型说明；示例统一使用公开 `CheckKind` API。
- 验证：完整门禁、版本契约、ARCH-A、5+100、AXAML/XML 与 `git diff --check` 通过后记录；状态仅为 `TECHNICAL PASS / READY FOR USER R4 VISUAL ACCEPTANCE`，不宣告用户视觉验收。

## v0.2.28.25-rz · AREA A C+D R3 COMPACT TOP + VERSION REFRESH（2026-09-08 12:51:00 +08:00）

- 版本：正式版本从 `v0.2.28.24-rz` 递增至 `v0.2.28.25-rz`；项目 Version、启动窗口标题、编辑器窗口回退标题与动态文档标题使用同一版本。
- 标题：保持“玄域引擎编辑器 {CURRENT_VERSION} - {DocumentTitle}”语义，文档名仍由现有 `DocumentTitle` 动态提供。
- 紧凑 Top：移除重复品牌块；首行保留工作区、文件、弹性空间、运行、状态，第二行保留编辑工具、视图、吸附。模块改为横向紧凑条，34 DIP 控件保持可点击，管理态仅禁用而不移位。
- 契约：版本契约从项目 `Version` 提取并校验四段 `-rz` 格式，再比对启动标题、窗口回退标题、动态文档标题与 changelog；避免消费者继续停留旧版本。
- 验证：解决方案 Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1403/1403、XYUI 558/558 PASS；ARCH-A、变更 AXAML/XML、5+100 与 `git diff --check` PASS。
- 状态：`TECHNICAL PASS / READY FOR USER IPO VISUAL ACCEPTANCE`；未宣告用户视觉或真机验收通过。

## AREA A · C+D 第二轮纠偏（2026-09-08 12:45:52 +08:00）

- 顶部重排：第一行固定品牌、工作区、文件、弹性空区、运行与状态；第二行保留编辑工具、视图、吸附三块。工作区改为同级中文标题与紧凑的“管理模式 / 地图编辑”操作入口。
- 可用性真源：新增 `CanUseEditTools` 与 `CanToggleSnap`，只派生自既有 Mode/Workspace；管理态工具、区域编辑下的地图变换工具、非地图编辑态吸附均不能通过命令入口改变状态。
- 交互诚实性：编辑工具与吸附在不可用状态仍保留位置；框选恒为禁用且移除命令接线；聚焦与全览采用不同矢量图标。
- 契约与回归：新增 `AreaAR2AvailabilityContractTests`，覆盖管理态、工作区切换、吸附和未实装框选的不可变语义；既有变换测试明确进入编辑态，并按地图编辑真实取景使用地图尺寸上界。
- 验证：解决方案与测试项目构建均为 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1402/1402 PASS；ARCH-A guard 与 `git diff --check` PASS。XYUI 全量测试命令正常结束且未输出失败。
- 状态：`TECHNICAL PASS / READY FOR USER IPO VISUAL ACCEPTANCE`；尚未宣告用户视觉或真机验收通过。

## AREA A · C+D TOP CHROME（IN PROGRESS，2026-09-08 11:36:00 +08:00）

- 基准与版本硬门禁：以 `v0.2.28.24-rz` 为全项目唯一事实真源，新增 `UiCanonicalVersionContractTests`（4/4 PASS）锁定 `run.bat`、窗口标题、VM 标题与 changelog 四处完全一致，杜绝版本漂移。
- 前置纠偏：彻底移除 `MapFormPanel` 中的隐藏 `PropsNarrow`，单行 Property Grid 3 个字段各维持 1 处独立错误绑定；更新 `UiD5FormContractTests`（1/1 绑定契约）、`UiD4MapEditorContractTests`、`R2BPropertyEditorVisualContractTests` 与 `XYUI2R2BContractTests`，实装真单行 96,* 属性区。
- C+D 模块卡片化架构：将 Area A 顶层解耦为独立 SRP 模块视图（`WorkspaceSelector`、`FileModule`、`EditToolsModule`、`ViewModule`、`SnapModule`、`RuntimeStatusModule`、`Top.axaml`），每个手写 AXAML 文件均 ≤ 100 行（严格遵守 5+100 架构红线与多行一属性 XAML 规范）。
- 模块七大中文分区：落地 `工作区`（独立呈现编辑模式与具体工作区）、`文件`（文件/添加原生菜单 + 高频新建/打开/保存 + 撤销/重做）、`编辑工具`（选择、框选、移动、旋转、缩放，框选由 VM 诚实反馈尚未实装）、`视图`（聚焦、全览、平移、环绕、环境菜单）、`吸附`（吸附开关与状态）、`运行`（Primary 运行、Danger 停止）、`状态`（文档状态与当前模式 Badge）。
- 样式纯化与单手势防抖：彻底清除 `Top.States.axaml` 模板 hack 与 `.cmdBtn`/`.toolBtn` 样式，全面接入规范 XYUI 控件；移除 `WorkspaceSelector` 中的 `DoubleTapped` 穿透，锁死为单一点击切换手势。
- 自动化门禁：完整解决方案 Build `0 Warning / 0 Error`；Core `339/339`、WarCore `22/22`、World `1399/1399`、XYUI `558/558 PASS`；`arch-a-guard.ps1`（含 5+100）与 AXAML/XML、版本一致性、`git diff --check` 均通过。
- 状态：`READY FOR USER IPO VISUAL ACCEPTANCE`，等待真机验收。

## AREA A · TOP CHROME（IN PROGRESS，2026-09-08 10:36:02 +08:00）

- 开工：在 recovered canonical 上重新施工，不导入 Area-A 私人实验提交。
- 首个布局改造：Top Chrome 用真实 `XYHeading`、`XYCaption`、`XYSeparator` 和既有 `XYBadge` 分离品牌/编辑上下文、菜单与命令组；所有既有 Command、Binding 与工具交互保持原接线。
- 验收：自动门禁完成后仍需用户按 IPO 做顶部视觉与交互真机验收；当前状态为 `IN PROGRESS`，不是 CLOSED。

## BASELINE-STABILIZATION · e074e1bb recovered candidate（2026-09-08 10:36:02 +08:00）

- 恢复：地图属性重新保留宽/窄两种真实 XYUI 输入路径，6 个字段控件均独立绑定 `MapWidthError` / `MapDepthError` / `MapBaseHeightError`，失焦校验不再退化为表单级错误；MAP focused `824/824 PASS`。
- 单源：删除 35 个历史大写 `XYUI/` 追踪路径，并以 Windows 两阶段实体目录改名收敛为唯一 `xyui/`；Engine ProjectReference 继续指向 `..\\xyui\\avalonia\\src\\XYUI.Avalonia\\XYUI.Avalonia.csproj`。
- 样式：Engine 旧 Button/TextBox 规则改为显式 `legacyControl` opt-in；XYUI 子类不匹配 Legacy 全局规则，工具与图层状态改由 XYUI canonical token 决定。
- 验证：Solution Build `0 Warning / 0 Error`；Core `339/339`；WarCore `22/22`；MAP `824/824`；Engine XYUI/UI focused `355/355`；XYUI 全量 `558/558`（TRX）；ARCH-A、AXAML/XML、DLL/deps/SHA256、一致性及 `git diff --check` 通过。

## XYUI-ENGINE-A-R2-B-FIXUP-02 · Property Grid single-line recovery（2026-09-07 20:17:07 +08:00）

- 视觉修复：`MapFormPanel` 移除会在侧栏窄于 360 DIP 时切换的 `PropsNarrow`，地图属性固定为 `96,*` 单行 Property Grid；标签、输入框与只读值垂直居中。
- 宽度修复：左侧项目面板初始宽度恢复为 `220 DIP`，上限收敛为 `320 DIP`；属性字段不再通过窄表单或最小宽度撑大面板。
- 语义保持：保留 `XYTextField`、`128 × 24 DIP`、字符串 Binding、LostFocus 校验、错误反馈及 Apply/Undo/Redo Command；VM 未修改。
- 契约：R2B 运行时契约改为检查三组真实输入、四行同一 Property Grid 与左侧面板宽度边界；不再接受隐藏窄布局冒充通过。
- 验证：属性/R1/R2-A/R2-B/D4/D5 定向契约 `50/50 PASS`；完整解决方案 Build `0 Warning / 0 Error`；ARCH-A PASS；`git diff --check` PASS。
- 状态：`R2-B PROPERTY FINAL FIX COMPLETE / READY FOR USER VISUAL ACCEPTANCE`；尚未判定用户真机验收通过。

## XYUI-ENGINE-A-R2-B-FIXUP · Property Editor compact density（2026-09-07 19:29:57 +08:00）

- 视觉修复：`MapFormPanel` 的 6 个 XYUI 输入字段统一使用现有 Compact Token，固定为 `128 × 24 DIP`、右对齐、Compact Padding；属性行与窄模式间距统一收敛到 `Space.2` / `Space.4`。
- 操作修复：应用、撤销、重做继续使用 `XYButton` 与原有 Command，统一为 `96 × 24 DIP`、左对齐、Compact Padding，移除整栏 Stretch 视觉；未修改 VM、字符串 Binding、LostFocus、校验和 Undo/Redo 语义。
- 契约：新增 `R2BPropertyEditorVisualContractTests`，同时更新 D4 Map Editor 布局契约以锁定正式 Compact Token；运行时几何断言覆盖 6 字段与 3 按钮。
- 验证：Property/R1/R2-A/R2-B/D4/D5 定向契约 33/33 PASS；完整解决方案 Build 0 Warning / 0 Error；ARCH-A PASS；`git diff --check` PASS。
- 状态：`R2-B VISUAL FIX TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE`；尚未判定用户真机验收通过。

## XYUI-ENGINE-A-R2-B · Final Left SHA reconciliation（2026-09-07 18:57:04 +08:00）

- 事实核验：远端 `origin/feat/XYUI-ENGINE-A-R2-B-G` 当前为 Gemini `1212f30b1bd6937bab4cf76f6eb917414285a042`；`1212f30b` 不在旧 Acceptance `7edf8fdb` 中，旧 Left `53efea66` 与 `1212f30b` 互不为祖先关系。
- 收口：按 Gemini 最新提交原样恢复 6 个 Left 文件，生成 reconciliation commit `2924ac83`；Left 与 `1212f30b` 内容一致，Right/Contract `fe55ea30` 保留，未手改 Gemini UI。
- 事实计数：Left 包含 `XYSearchField` 1 个、`XYButton` 11 个、`XYToggleButton` 3 个；Right XYUI-2 实装未被覆盖。
- 验证：R2-B/R1/R2-A 定向契约 10/10 PASS；完整解决方案 Build 0 Warning / 0 Error；ARCH-A PASS；`git diff --check` PASS。
- 状态：等待 Acceptance 更新后的唯一 `run.bat` 真机视觉与交互验收，未将自动门禁标记为用户验收通过。

## XYUI-ENGINE-A-R2-B · Buttons & Inputs visible migration（2026-09-07 18:27:45 +08:00）

- 合流：以 R2-A 验收基线 `1d6e8ab2` 为共同基线，受控合入 Left `53efea66` 与 Right/Contract `fe55ea30`，生成集成提交 `9987e00e`、`4c40cb49`；未修改 Canonical 工作区。
- 迁移：Left 项目搜索使用真实 `XYSearchField`；地图、区域、道路、标记操作使用 `XYButton`/`XYToggleButton`。Right Inspector、地图、数据集、图层操作使用真实 `XYButton`、`XYIconButton`、`XYTextField`、`XYSelect`、`XYToggleButton`，保留既有 Binding、Command 与事件语义。
- GAP：MapForm 数值字段暂用 `XYTextField`，因为现有 VM 为字符串 Binding 且依赖 `TextBox`/`LostFocus`；本轮不改 VM，不冒险引入 `XYNumberField`。
- 契约：新增 `XYUI2R2BContractTests`，并纳入 R1/R2-A 回归；定向测试 10/10 PASS。完整解决方案 Build 0 Warning / 0 Error；ARCH-A PASS；`git diff --check` PASS。
- 状态：`R2-B TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE`；仍须通过唯一入口 `E:\MyDoc\project-VSCode\XuanYuEngine\run.bat` 做真机视觉与交互验收。

## XYUI-ENGINE-A-R1-VISUAL-FIX · SectionTitle visual authority closeout（2026-09-07 14:39:05 +08:00）

- Gemini handoff：`194cb923` 已提交并 push 到 `feat/XYUI-ENGINE-A-R1-VISUAL-FIX`，修改 Engine Debug 页面，移除四组 `XYSeparator Variant="Section"`，避免通用 Divider 夺取 `XYSectionTitle` Soft Header 的视觉层级。
- 根因：Canonical `XYSectionTitle` 与 Engine Theme Resource 链本身有效；Engine 页面在真实区块标题后重复放置 Section Divider，导致 Accent Bar 与 Soft Header 语义在真机上不突出。未发现 Legacy Selector 覆盖 `XYSectionTitle` 的证据。
- Canonical：冻结标准保持不变：Header `28 DIP`、Left Mark `3 × 16 DIP`、`#526873`、文字 `14/600/18`、背景 `#EEF3F6`、圆角 `3 DIP`；Gemini 临时改动的 Accent 色和额外 Mark 圆角已按 Canonical 证据回退。
- Contract：新增 `UiR1VisualContractTests`，验证 Engine `EditorRightTabs` 中四个真实 `XYSectionTitle` 的 Visual、尺寸、颜色、Typography、文本、无重复 Separator；并覆盖 Heading、Label、Badge、StatusBadge、ErrorText 代表性样式。
- 验证：Engine Build 0 警告 / 0 错误；XYUI Build 0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1381/1381、XYUI 558/558；Visual Contract 2/2；Gallery Smoke 3/3；ARCH-A（含 5+100）PASS；`git diff --check` PASS。
- 状态：`R1-VISUAL-FIX TECHNICAL CLOSEOUT PASS / READY FOR USER ACCEPTANCE`。

## XYUI-ENGINE-A-R1-CLOSEOUT · FINAL-A + FINAL-B Controlled Merge（2026-09-07 13:50:40 +08:00）

- 合流：以统一母线 `cbbc52b0` 为基线，按顺序使用受控 `--no-ff` merge 合入 FINAL-A `861340aa` 与 FINAL-B `ab92fac7`，生成 `47a43188`、`a2869ef7`；无冲突，未 rebase、squash、reset、force push 或改写历史。
- 范围：Left、Top、Foot、Right 的本轮 XYUI-1 Engine View 迁移已统一进入 `feat/XYUI-ENGINE-A`；XYUI-1 Category A 剩余为 0。XYUI-2、XYUI-3、纯布局/渲染宿主和无 Canonical 等价物的图标按迁移矩阵保留，不提前越界。
- 审计：目标 View 不再使用 `uiSection`、`uiLabel`、`uiValue`、`datasetName`、`datasetStatus`、`datasetLayerName`、`datasetLayerStatus`、`kindTagRegion`、`kindTagSystem`、`treeText`、`statePill` 等 Legacy Display 类；`uiMultiline` 仅承担换行/行数限制，归入布局辅助职责。
- 验证：Engine Build 0 警告 / 0 错误；XYUI Build 0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1379/1379、XYUI 558/558，合计 2298/2298 PASS；Gallery Smoke 3/3 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS。
- 文档：同步本次 R1 closeout 到迁移矩阵；本轮无结构变化，`file-tree.md` 保持现状。
- 状态：`TECHNICAL CLOSEOUT PASS / XYUI-1 ENGINE IMPLEMENTATION COMPLETE / READY FOR USER FINAL VISUAL ACCEPTANCE`。

## XYUI-ENGINE-A-R1-FINAL-A · Left + Top XYUI-1 Final Sweep（2026-09-07 13:29:25 +08:00）

- 基线：从统一母线 `cbbc52b0` 创建 `feat/XYUI-ENGINE-A-R1-FINAL-A` 独立 worktree；本轮仅施工 Left 5 个 View 与 Top 1 个 View。
- 迁移：项目树与层级树标题使用 `XYTruncatedText`；区域/道路/标记 Display 使用 `XYSectionTitle`、`XYSeparator`、`XYLabel`、`XYText`、`XYSelectableText Technical`、`XYCaption`；Top 状态药丸使用真实 `XYBadge`，工具组分隔使用 `XYSeparator Variant=VerticalSplit`。
- 保留：TabControl、ListBox/Tree、TextBox、ContextMenu、Menu、Button、ToggleButton、业务 Binding、Command、事件、拖拽和重命名行为均未改动；未修改 VM、Theme、App 或 XYUI Runtime。
- 契约：新增 `UiR1FinalLeftTopContractTests` 5 项断言，锁定真实 XYUI-1 组件并拒绝本轮目标 View 的 `treeText/uiSection/uiLabel/uiValue/uiMultiline/groupSeparator/statePill` Legacy Display 类。
- GAP：本轮无阻塞 Runtime GAP；动态文档状态没有可直接绑定的 `XyuiStatusState` 公共事实源，因此保留原状态颜色 Binding 并使用 `XYBadge`；XYReorderableList、XYUI-2 交互控件和未纳入映射的工具图标留待后续范围。
- 验证：Engine Build 与 XYUI Build 均 0 警告 / 0 错误；定向契约 5/5；Core 339/339、WarCore 22/22、World 1379/1379、XYUI 558/558，合计 2298/2298 PASS；Gallery Smoke 3/3 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS。
- Hash：本轮最终提交由 Git 记录确认。
- 状态：`R1-FINAL-A TECHNICAL PASS / READY FOR INTEGRATION`。

## XYUI-ENGINE-A-R1-B2-MERGE · Codex + Gemini Controlled Merge（2026-09-07）

- 合流：在 `e19e4242` 基线分支 `feat/XYUI-ENGINE-A` 上，按顺序使用受控 `--no-ff` merge 合入 Codex `0f80d905` 与 Gemini `5b9add17`；未 rebase、squash、reset、force push 或改写历史。
- 范围：统一包含 `NotificationBar`、`LogDetailPanel`、`LayerInspectorPanel`、`InspectorPanel`、`MapPagePanel`、`DatasetLayerPanel`、`DatasetPanel` 七个 Engine View 的 XYUI-1 迁移成果；两边所有权无重叠，未发现冲突。
- 祖先关系：`0f80d905` 与 `5b9add17` 均已确认是合流提交祖先；旧 dirty `feat/MAP-DATA-A` 工作区未触碰。
- 验证：Engine Build 与 XYUI Build 均 0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1374/1374、XYUI 558/558，合计 2293/2293 PASS；Gallery Smoke 3/3 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS。
- 文件树：本轮仅合流既有文件，无新增、删除、改名或移动文件，`file-tree.md` 无需更新。
- Hash：本轮最终合流提交由 Git 记录确认。
- 状态：`R1-B2 INTEGRATION PASS / READY FOR USER VISUAL ACCEPTANCE`。

## XYUI-ENGINE-A-R1-B2-A · Inspector / MapPage XYUI-1 Migration（2026-09-07）

- 基线：基于集成分支提交 `e19e4242` 创建 `feat/XYUI-ENGINE-A-R1-B2-A`；本轮仅迁移 `InspectorPanel.axaml`、`MapPagePanel.axaml` 及其直接失效契约测试。
- 迁移：Inspector 使用真实 `XYHeading`、`XYCaption`、`XYIcon`、`XYSectionTitle`、`XYSeparator`、`XYLabel`、`XYText`、`XYSelectableText`、`XYEmptyText`；技术字段按真实语义使用 `Technical` 变体。
- 迁移：MapPage 的“地图资产”摘要使用 `XYSectionTitle`、`XYSeparator`、`XYLabel`、`XYText`、`XYSelectableText`；MapForm、按钮、输入控件、绑定和交互保持不变。
- 清理：移除两个目标页面中控制 XYUI-1 视觉语义的旧 `panelTitle`、`panelIcon`、`fieldSeparator`、`groupSeparator`、`uiLabel`、`uiValue` 等 Legacy 类；更新直接失效的 Inspector、文本溢出和 MapEditor 契约断言。
- 验证：Engine Build 与 XYUI Build 均 0 警告 / 0 错误；定向契约 23/23；Core 339/339、WarCore 22/22、World 1374/1374、XYUI 558/558，合计 2293/2293 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS。
- Hash：本轮最终提交由 Git 记录确认。
- 状态：`R1-B2-A TECHNICAL PASS / READY FOR INTEGRATION`。

## XYUI-ENGINE-A-R1-M1/C · Controlled Integration + Runtime Theme Wiring（2026-09-07）

- 合流：从 `93893f8a` 依次合入 R1-A `012ba16d` 与 R1-B `5c3b3024`；R1-B 使用普通 `--no-ff` 受控 merge，集成提交为 `29d53c70`，未 rebase、squash 或改写历史。
- Theme 接入：`XuanYu.Editor.UI/Bootstrap/App.axaml.cs` 复用最新 Runtime 公开 API，注册 `XyuiTheme`、`XyuiVectorIcons`、`XyuiTextStyles`、`XyuiShapeStyles`、`XyuiInteractionStyles`、`XyuiControlStyles`、`XyuiComponentStyles`；未新增 `XYUIBootstrap`，未引用 Gallery。
- Authority：`LayerInspectorPanel.axaml` 删除会覆盖 `XYLabel` / `XYText` / `XYSelectableText` 的旧 `uiLabel` / `uiValue` 类；Button/TextBox 业务交互保持不变。
- Smoke：`XuanYu.World.Tests` UI Runtime 定向 Smoke `80/80 PASS`；两个 R1 分支均已验证为集成提交祖先。
- 验证：Engine Build 与 XYUI Build 均 0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1374/1374、XYUI 558/558，合计 2293/2293 PASS；UI Runtime Smoke 80/80 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS。
- 修正：两项旧 LayerInspector 源码合同测试改为锁定 XYUI 类型存在且 Legacy `uiLabel/uiValue` 不再覆盖 Runtime 视觉；未弱化业务断言。
- Hash：本轮最终提交由 Git 记录确认。
- 状态：`R1-M1 MERGED / R1-C TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE`。

## XYUI-ENGINE-A-R1-A · Governance / Migration Matrix Audit（2026-09-07 11:37:39 +08:00）

- 目标：在统一基线 `93893f8a` 上完成 Engine UI 接入审计、XYUI-1~3 Migration Matrix、Legacy/Duplicate Style Inventory 与 Agent-B ownership freeze。
- 变化：新增 `docs/milestones/current/XYUI-ENGINE-A/XYUI-ENGINE-A-migration-matrix.md`；确认 Editor.UI → 最新 `xyui/avalonia` Runtime 引用有效、Editor/App/Win 无 Gallery Runtime 依赖；不修改 Engine View、Runtime、Gallery 或全局 Theme。
- 盘点：44 个 Engine UI AXAML；TextBlock 178、Border 66、Button 63、ToggleButton 13、TextBox 13、ListBox 16、TabControl 3、Menu 3、ContextMenu 1、Path 51、PathIcon 8；旧 `XYUI/**` 34 个 tracked 文件记为 Legacy Duplicate。
- 遗留：`XYUIBootstrap.Create()` 不存在，App 尚未加载 `XyuiTheme` / `XyuiComponentStyles`，作为后续集成 GAP；三个 Agent-B View ownership 已冻结，`InspectorPanel.axaml` 延后 Batch 2。
- 验证：Engine Build 与 XYUI Build 均 0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1374/1374、XYUI 558/558，合计 2293/2293 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS。
- Hash：本轮提交由最终 Git 记录确认。
- 状态：`R1-A TECHNICAL PASS / READY FOR AGENT-B HANDOFF`。

## XYUI-ENGINE-A-R1-P0 · Latest Engine + Latest XYUI Integration（2026-09-07 11:25:42 +08:00）

- 基线：Engine `feat/MAP-DATA-A` committed HEAD `7ae17b4d`；XYUI `origin/feat/XYUI-A` HEAD `3d09dc67`，包含 `5fd54b42` GALLERY-UNIFY-02 及其后续提交。
- 整合：创建 `feat/XYUI-ENGINE-A`；受控合并无共同祖先历史，55 个 Engine 根文件按 Engine 侧保留，`xyui/**` 按最新 XYUI 侧保留；根 solution 改指向 `xyui/avalonia`，Editor.UI 增加 XYUI Runtime ProjectReference，未引入 Gallery Runtime 依赖。
- 验证：`XuanYu.Engine.slnx` 与 `xyui/avalonia/XYUI.Avalonia.slnx` 均 0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1374/1374、XYUI 558/558，全量合计 2293/2293 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS。D 盘指定 SDK 不存在，使用 E 盘 SDK 完成实际门禁。
- 状态：`INTEGRATION PASS`；未创建 tag/release，未触碰两个既有 dirty 工作区。

## v0.2.28.66-rz · AREA-D-R3-INSPECTOR-PAGER（2026-09-11 00:00:00 +08:00）
- 目标：将右侧地图/区域编辑导航切换为分页 Inspector，并让图层 Dock 支持自适应折叠。
- 变化：新增 XYPager、XYInspectorSection、XYCollapsiblePane；地图页增加基础/环境/显示/数据/高级分页；图层区改为可折叠 Pane；同步运行时与源码 UI 合同测试。
- 验证：专项 Runtime/UI 合同测试 25/25 通过；全量门禁待执行。
- Hash：待提交。
- 遗留：等待真机视觉与交互验收。
