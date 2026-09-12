## v0.2.28.73-rz
FEATURE-EDIT-R1-CODEX-C1 $date
- Ŀ�꣺���ϵ㡢�ߡ���༭Ϊͳһ�ġ�Ҫ�ر༭�����ϸ���뼸�ζ���༭��ѡ���߼���
- �仯��
  - �Ƴ�ԭ������༭���� UI ��������� RegionEditor ������ DisplayName ������Ϊ��Ҫ�ر༭����
  - ��ȡ IsFeatureEditingActive �� IsGeometryEditingActive ״̬ģ�ͣ�ֻ��ִ�С��༭���Ρ���������ק���㡣
  - ������ MapGeometryEditing �������������Ҫ��Ĭ��ִֻ��ѡ�У�������α༭����
- ��֤���Զ��������䣨Ҫ��ѡ�и�����ԣ���arch-a-guard PASS��ȫ�ֲ��� PASS��
- Hash��������
- ������UI չ�ֲ�Ķ����˵�����Ҫ�ر༭���͡�����༭����Ȼ���ڣ�Gemini ���Ƴ�������δ�޸� WorkspaceSelector.axaml������ Gemini ���ӡ��༭���Ρ���ڡ�
# changelog

## v0.2.28.73-rz · XUANYU-SYNC-SOP-BASELINE�?026-09-12 20:33:56 +08:00�?- 目标：按首次同步 SOP 建立可追踪的同步规范基线�?- 变化：新�?`docs/governance/sync-handoff-sop.md`，纳入附件《玄域引擎同步与交接规范 v1.0》原文；同步治理文档索引与文件树�?- 验证：Git fetch 后安全快�?6 个提交；工作区在实施前干净；规范原�?1693 行逐行核对；ARCH-A guard PASS；`git diff --check` PASS�?- Hash：以本轮最终提交为准�?- 遗留：尚未执行完整构�?测试；本轮仅治理文档变更�?
## v0.2.28.73-rz · XYENGINE-MAINLINE-RESUME-R1-FIX1�?026-09-11 11:35:09 +08:00�?- 目标：解除真机验收中�?Marker 入口不可发现阻断�?
- 变化：在顶部工作区菜单增加“点要素编辑”入口；该入口仅投影到现�?RegionEditor、EditMode、Marker authoring �?Marker placement 链，不新�?Workspace、MapMarker 数据结构或第二套放置逻辑；补齐编辑模式切换时 Marker 可用性通知�?
- 验证：完整解决方�?Build 0 警告/0 错误；Core 339/339、World 1508/1508、XYUI 617/617、WarCore 22/22；ARCH-A、版本一致性与 `git diff --check` 通过�?
- Hash：实现提�?`c449c455`；文档收口提交以最�?HEAD 为准�?
- 遗留：仅待用户真机确认顶部菜单可发现、进入后显示“地图标�?放置地图标记”，以及实际视口放置；未验收不得进入 WORLD-A�?

## v0.2.28.72-rz · XYENGINE-MAINLINE-RESUME-R1�?026-09-11 10:34:59 +08:00�?
- 目标：恢�?MAP-DATA-A R3 点要素进入现行区域编辑器�?Inspector 工作流�?
- 变化：新�?MapMarker Inspector 身份/Dataset/状态投影与二维坐标提交；统一 Marker 选择、空选择、Viewport 拖动、Undo/Redo、Save/Reload 的同步；补齐 Marker 数据集图层反解与锁定提交守卫；复用现�?XYVectorProperty(Vector2)，未修改 MapMarker schema、WORLD-A �?XYUI 控件�?
- 验证：完整解决方案构�?0 警告/0 错误；Core 339/339、World 1505/1505、XYUI 617/617、WarCore 22/22；定�?Marker Inspector 与既�?Marker 回归 12/12；ARCH-A/5+100、XML �?`git diff --check` 通过�?
- Hash：`c40aac11`�?
- 遗留：等待用户真机视觉验收；本轮不进�?WORLD-A�?

## v0.2.28.71-rz · EDITOR-BOTTOM-LOG-R1-F1�?026-09-11 02:11:05 +08:00�?
- 目标：修�?Bottom Log 无法展开、列表不可见与多余标题问题�?
- 变化：恢�?`XYIconButton(ChevronDown) �?ToggleLogCommand` 展开入口；移除工具条“日志”标题；保留展开后的逐条日志列表�?
- 验证：完�?Build 0 警告/0 错误；Core 339/339、WarCore 22/22、World 1498/1498、XYUI 617/617；ARCH-A �?`git diff --check` 通过�?
- Hash：`d423ba62`�?
- 遗留：等待用户真机视觉验收�?

## v0.2.28.71-rz · EDITOR-BOTTOM-LOG-R1�?026-09-11 01:56:56 +08:00�?
- 目标：收�?Bottom Log �?XYUI 工具条、组合筛选、搜索、详情切换与折叠布局�?
- 变化：严重级别改�?`XYToggleButton`；搜索改�?`XYSearchField`；来源收入口使用 `XYMenu/XYMenuItem`；详情默认隐藏；新增清空日志与组合过滤状态�?
- 验证：便�?SDK 完整 Build 0 警告/0 错误；Core 339/339、WarCore 22/22、World 1497/1497、XYUI 617/617；ARCH-A �?`git diff --check` 通过�?
- Hash：起始提�?`b77b67e6`；最终提�?`b4e77a30`�?
- 遗留：等待用户真机视觉验收；`file-tree.md` 因既有非 UTF-8 编码未改写�?

## v0.2.28.71-rz · XYENGINE-AREA-C-CONTEXT-TOOLBAR-R1-F4�?026-09-11 01:55:00 +08:00�?
- 目标：恢�?Top 第二行隐藏滚动条后的鼠标横向滚动能力�?
- 变化：为隐藏滚动条的 Top 第二�?`ScrollViewer` 接入滚轮隧道路由，将滚轮增量转换为水平偏移并消费事件�?
- 验证：Context Toolbar 合同/运行�?6/6；完整门禁结果见 Git closeout�?
- Hash：起始提�?`d8c2b302`；最终提交见 Git closeout�?
- 遗留：等待用户真机视觉验收�?

## v0.2.28.70-rz · XYENGINE-AREA-C-CONTEXT-TOOLBAR-R1-F3�?026-09-11 01:40:00 +08:00�?
- 目标：隐�?Top 第二行滚动条 UI，同时保留鼠标滚动切换能力�?
- 变化：恢复水�?`ScrollViewer`，将水平滚动条设�?`Hidden`，取消工具组换行�?
- 验证：Context Toolbar 合同 4/4、运行时 1/1；其余正式门禁结果见 Git closeout�?
- Hash：起始提�?`cbb47866`；最终提交见 Git closeout�?
- 遗留：等待用户真机视觉验收�?

## v0.2.28.69-rz · XYENGINE-AREA-C-CONTEXT-TOOLBAR-R1-F2�?026-09-11 01:25:14 +08:00�?
- 目标：移�?Top 第二行导致截图中出现的灰色水平滚动条�?
- 变化：Top 第二行由水平 `ScrollViewer` 改为 `WrapPanel`，工具组可换行显示，不再创建水平滚动宿主�?
- 验证：Context Toolbar 合同 4/4、运行时无滚动宿�?1/1；方案构�?0 警告/0 错误；Core 339/339、WarCore 22/22、World 1492/1492、XYUI 617/617；ARCH-A�?+100 �?`git diff --check` 通过�?
- Hash：起始提�?`61555af2`；最终提交见 Git closeout�?
- 遗留：等待用户真机视觉验收�?

## v0.2.28.68-rz · XYENGINE-AREA-C-CONTEXT-TOOLBAR-R1-F1�?026-09-11 01:18:01 +08:00�?
- 目标：修�?Context Tool Bar 的编辑上下文可见性�?
- 变化：区域面、道路、地图标记及绘制操作仅在 `IsRegionEditMode` 下显示；启动态与地图编辑态完全隐藏区域专属工具，不再以禁用态暴露�?
- 验证：Context Toolbar 合同与运行时矩阵 4/4；方案构�?0 警告/0 错误；Core 339/339、WarCore 22/22、World 1491/1491、XYUI 617/617；ARCH-A�?+100 �?`git diff --check` 通过�?
- Hash：起始提�?`9293e33e`；最终提交见 Git closeout�?
- 遗留：等待用户真机视觉与交互验收�?

## v0.2.28.67-rz · XYENGINE-AREA-C-CONTEXT-TOOLBAR-R1�?026-09-11 00:58:34 +08:00�?
- 目标：建�?Top 第二�?Context Tool Bar，迁�?Region / Road / Marker 工具入口�?Region/Road Authoring 操作，使 Right Inspector 仅承担属性内容�?
- 变化：新�?`ContextToolBar`，复�?`UiVm` 现有 Tool / Authoring Mode / Drawing State / Command；Inspector 移除 `RegionalAuthoringPanel`，保�?`MapEditorPanel`、XYPager 和唯一属性滚动宿主；同步结构回归合同�?file-tree�?
- 验证：方案构�?0 警告/0 错误；Core 339/339、WarCore 22/22、World 1489/1489、XYUI 617/617；ARCH-A �?5+100 通过；AXAML XML 静态检查与 `git diff --check` 通过�?
- Hash：起始提�?`2837acd3`；最终提交见 Git closeout�?
- 遗留：等待用户真机视觉与交互验收�?

## v0.2.28.66-rz · AREA-D-R3-INSPECTOR-PAGER�?026-09-11 00:33:37 +08:00�?
- 目标：将右侧地图/区域编辑导航切换为分�?Inspector，并让图�?Dock 支持自适应折叠�?
- 变化：新�?XYPager、XYInspectorSection、XYCollapsiblePane；地图页增加基础/环境/显示/数据/高级分页；图层区改为可折�?Pane；同步运行时与源�?UI 合同测试�?
- 验证：专�?Runtime/UI 合同测试 25/25 通过；全量构�?0 警告/0 错误；World 1487/1487、Core 339/339、WarCore 22/22、XYUI 617/617 通过�?
- Hash�?8d97422�?
- 遗留：等待真机视觉与交互验收�?

## v0.2.28.65-rz · XYENGINE-NAV-LAYOUT-STABILITY-FIX-R2�?026-09-10 23:22:02 +08:00�?
- 目标：将已通过真机验收�?Gallery 导航测量稳定性修正实装到 XuanYu Engine 项目树与层级树�?
- 修正：为 `ProjectList` �?`HierarchyList` 增加显式外层滚动宿主，并以内联非滚动 `StackPanel` ItemsHost 替换默认 ListBox 模板；保留选择、键盘、hover、重命名和树行交互�?
- 限制：未修改 XYUI canonical、滚轮事件处理、ScrollOffset 补偿�?FIX-R2 之外的行为�?
- 回归：新�?Engine Headless 合同，确认两棵树各自只有一个显�?ScrollViewer，ListBox 内部不再包含 ScrollViewer；定向回�?`2/2`�?
- 验证：方案构�?0 警告/0 错误；XYUI.Avalonia.Tests `614/614`、Core `339/339`、WarCore `22/22`、World `1485/1485`；ARCH-A �?`git diff --check` 通过�?
- Hash：起始提�?`d1e6a34b`�?
- 状态：`XYENGINE-NAV-LAYOUT-STABILITY-FIX-R2 / READY FOR USER VISUAL + WHEEL ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.64-rz · XYUI-GALLERY-NAV-LAYOUT-STABILITY-FIX-R2�?026-09-10 23:07:27 +08:00�?
- 目标：收�?Gallery 左侧五个导航 ListBox 的内部测�?虚拟化导致的外层 Extent 跳变�?
- 根因证据：真�?Windows F1 时间线确�?XYUI-3 导航高度 `651.429 �?1285.714`、已实现容器 `1 �?6`，同步推动外�?Extent `2885.714 �?3520`（`+634.286 DIP`）；内部 `PART_ScrollViewer.Offset` 未变化�?
- 修正：仅�?Gallery �?`nav-tree` / `nav-foundation` 提供无内�?ScrollViewer、StackPanel ItemsHost 的专用模板；保留 ListBox selection、键盘、hover 与选中态，继续关闭 `AutoScrollToSelectedItem`；未改全局 Avalonia ListBox、XYUI canonical 或滚轮事件处理�?
- 回归：新�?Gallery Headless 布局稳定性合同，验证五个导航列表无内�?ScrollViewer，外层滚动后 Extent 稳定；定�?Gallery 回归 `28/28`�?
- 验证：XYUI.Avalonia.Tests `614/614`、Core `339/339`、WarCore `22/22`、World `1483/1483`；方案构�?0 警告/0 错误；ARCH-A �?`git diff --check` 通过�?
- Hash：起始提�?`13a0821c4354c4ce335d5d7945d1277e0c1c1665`�?
- 状态：`XYUI-GALLERY-NAV-LAYOUT-STABILITY-FIX-R2 / READY FOR USER WHEEL ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.63-rz · XYENGINE-NAV-SCROLL-AUTHORITY-R1�?026-09-10 21:39:13 +08:00�?
- 目标：将 Gallery 已验证的导航滚动权修正实装到 XYengine 编辑器的项目树与层级树�?
- 修正：`ProjectList` �?`HierarchyList` 显式关闭 `AutoScrollToSelectedItem`，保留两个树控件各自内部滚动宿主和滚轮交互；未改 XYUI canonical 控件或延期中的未保存对话框�?
- 回归：新增真�?Engine Headless 测试，覆盖左侧项目树与右侧层级树运行时属性；先红�?2/2，修正后绿灯 2/2�?
- 验证：Engine 定向回归 2/2；此�?Gallery 定向回归 3/3、相关回�?11/11；此前方案构�?0 警告/0 错误；ARCH-A �?`git diff --check` 通过；World 全量仍有 1 个既有用户延�?Unsaved Dialog 词文测试失败�?
- Hash：起始提�?`6d9f0608744721d68e3444b32b45e7bd5f78eb06`�?
- 状态：`XYENGINE-NAV-SCROLL-AUTHORITY-R1 / READY FOR USER VISUAL + WHEEL ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.62-rz · XYUI-GALLERY-NAV-SCROLL-JUMP-FIX-R1�?026-09-10 21:25:02 +08:00�?
- 目标：收�?Gallery 左侧导航的单一 Scroll Authority，消除程序化选中导航项时外层滚动位置被隐式改写的问题�?
- 修正：Gallery 左侧唯一外层 ScrollViewer 命名�?`NavigationScrollHost`；`nav-tree` �?`nav-foundation` �?ListBox 显式关闭 `AutoScrollToSelectedItem`；未�?XYTabs、右侧文档滚动或编辑器业务区�?
- 回归：新增真�?Headless Gallery 运行时测试，覆盖 5 个导�?ListBox 的运行时属性、选中 `XYUI-3-3.10` 后外�?Offset 稳定性与单一显式滚动宿主�?
- 验证：红�?3/3；修复后定向回归 3/3、相�?Gallery 回归 11/11；方案构�?0 警告/0 错误；XYUI 612/612；Core 339/339；WarCore 22/22；World 1480 通过�? 个既有用户延�?Unsaved Dialog 词文测试失败；ARCH-A �?`git diff --check` PASS�?
- Hash：起始提�?`0e0d1daabd88f743741029e7ad7cdba08f9b3b3c`�?
- 状态：`XYUI-GALLERY-NAV-SCROLL-JUMP-FIX-R1 / READY FOR USER VISUAL + WHEEL ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.61-rz · AREA-D-R2-FIX5-F1�?026-09-10 19:59:32 +08:00�?
- 目标：移除截图中左侧与右侧固定导航页签之间的多余竖线�?
- 根因：`XYTab` 无条件创�?`xyui-tab-divider` 1 DIP Border，并为它保留独立 Auto 列；�?Border 被每�?Tab 渲染为竖线�?
- 修正：删�?divider visual child、空 Auto 列及对应 accent span，保�?Tab 内容、optional slots、选中态和关闭命中区语义；不改 `XYTabs`、`XYTabBar` 或导�?action�?
- 验证：divider Headless 回归 1/1；Tabs 相关回归 17/17；方案构�?0 警告/0 错误；Core 339/339；WarCore 22/22；XYUI 609/609；World 1480 通过�? 个既有用户延�?Unsaved Dialog 词文测试失败；ARCH-A、JSON/XML�?+100 �?`git diff --check` PASS�?
- Hash：起始提�?`14a2c290cede6f662e1ebc0ce59eb1a13e3b142f`�?
- 状态：`AREA-D-R2-FIX5-F1 / TAB DIVIDER REMOVED / READY FOR USER VISUAL ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.60-rz · AREA-D-R2-FIX5�?026-09-10 19:17:56 +08:00�?
- 目标：修�?XYTab Content sizing �?optional slot 语义，并�?Engine 地图/区域固定页签明确收口为不可关闭�?
- XYUI canonical：XYTab 的图标、修改标记、关闭命中区改为按状态使�?0 或统一运行�?Token；可关闭页签保留关闭命中区，永久页签不产生关闭布局/可访问性槽位；XYTabs �?Auto/Star sizing 合同保持不变�?
- Editor integration：MapTabs 的地图基础/地图环境/数据集与 AuthoringTabs 的区域面/道路/地图标记显式 `IsClosable="False"`；Left.ContentTabs �?EditorRightTabs.SideTabs 保持既有永久页签和全部页签下拉行为�?
- 验证：方案构�?0 警告/0 错误；Core 339/339；WarCore 22/22；XYUI 607 通过�? 个既�?BottomNavigation 测试失败；World 1480 通过�? 个既有用户延�?Unsaved Dialog 词文测试失败；FIX5 XYUI sizing 8/8、Tabs 过滤 16/16、Editor integration 1/1；ARCH-A、JSON/XML�?+100、`git diff --check` PASS�?
- Hash：起始提�?`9289bb0c915c08ddc5e7154835ec5ef4b95bfed4`�?
- 状态：`AREA-D-R2-FIX5 / XYTAB CONTENT PURITY + EDITOR INTEGRATION / READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.59-rz · AREA-D-R2-FIX4�?026-09-10 18:06:54 +08:00�?
- 目标：以实战反馈反哺 XYUI canonical，并收口 Region Inspector / LayerDock 结构�?
- XYUI：新�?`XyuiTabSizingMode`（默�?Equal，Engine Left/Right/Map/Region authoring 使用 Content）；保留 XYTabBar 的滚动、翻页、溢出与新增职责。共�?Button Chrome 修正 XYButton、XYToggleButton、XYIconButton 的实际内容居中；XYBadge Accent 改用 `XY.Text.Primary`，补齐禁用态视觉；Gallery 与运行时契约同步�?
- Area D：Region authoring 归入 Inspector 唯一滚动宿主，Region/Road 页面移除嵌套滚动；Right 仅保�?Inspector、Splitter、独�?LayerDock；LayerDock 展开最小空间由 192 提升�?260 DIP，图�?ListBox 内部滚动与折叠保留�?
- 验证：解决方案构�?0 警告/0 错误；Core 339/339；WarCore 22/22；XYUI 602/602；World 1479 通过�? 个既有用户延�?Unsaved Dialog 词文测试失败；ARCH-A PASS�?+100 PASS；JSON/XML �?`git diff --check` PASS�?
- Hash：起始提�?`a954c6caaa34c9a64db54fc7913075f6c41c0e29`�?
- 状态：`AREA-D-R2-FIX4 / XYUI CANONICAL HARDENED / REGION INSPECTOR CONSOLIDATED / READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.59-rz · AREA-D-R2-FIX3�?026-09-10 16:30:48 +08:00�?
- 目标：修�?Map Inspector �?LayerDock 投影、空 EntityHeader 占位、Right Map/Region 紧凑字体�?LayerDock 展开空间�?
- 图层投影：Fresh Map 继续消费 MapDefaultDefinition 已有的“地�?/ 边界 / 区域 1”，Map 模式共享 LayerPanel 展示真实 `LayerItems`；不新增默认 Layer，Eye/Lock 仍沿用现�?MapSession 链；Left Scene Tree 保持 Scene + Entity 合同�?
- Inspector 密度：EntityHeader 仅在 `IsEntityInspector` 时可见；Map/Region/no Entity 不参�?Measure；Map/Region/Layer authoring 消费 Engine compact typography tokens�?
- LayerDock：展开最小空间提高到 192 DIP，Map/Region 共用现有内部 ListBox 滚动；Region 专属添加/排序入口不投影到 Map 模式�?
- XYUI GAP：`XYTabs/XYTab content-sized tab sizing` 尚无 canonical Auto/Content sizing API；当�?`XYTab` 固定图标/修改标记/关闭槽导致地图二�?Tab 不能按内容收紧。本轮不扩张 XYUI layout architecture，导航与键盘能力保持不变�?
- 验证：FIX3 定向回归 6/6；Area D R2 定向集合 24/24；本轮新增失�?0；`UnsavedChangesConfirmationWindow.axaml` 保持用户延期 dirty、未暂存、未提交�?
- Hash：起始提�?`7262a162`�?
- 状态：`AREA-D-R2-FIX3 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.58-rz · AREA-D-R2-FIX2�?026-09-10 15:48:48 +08:00�?
- 目标：仅修复一�?Tabs 密度、地图整数米显示、LayerDock 默认空间和图层操作图标可见性�?
- 导航：Left/Right 真实 `XYTabs/XYTab` 统一消费 `XY.Size=Compact`、`XY.Density=Compact` �?`Control.Height.Compact`；保留溢出动作、选择和键盘行为�?
- 地图尺寸：三�?`XYNumberField` 使用 `DecimalPlaces=0`、`Step=1`、`SmallStep=1`；Map Inspector 显示整数米，�?CurrentMap 仍保留小数精度，显式 Apply 才提交�?
- LayerDock：展开态增�?160 DIP 合理最小空间，折叠时恢复仅标题高度；保留内部列表滚动与现有宿主结构�?
- 图标：修�?Layer �?Path 的真实矢量几何缺�?Stroke 的根因；Visibility/Lock 操作、命令和现有图标资源保持不变�?
- 验证：FIX2 定向回归 6/6；本轮新增失�?0；`UnsavedChangesConfirmationWindow.axaml` 保持 dirty、未暂存、未提交�?
- Hash：起始提�?`0c352575`�?
- 状态：`AREA-D-R2-FIX2 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.57-rz · AREA-D-R2-FIX1�?026-09-10 14:57:19 +08:00�?
- 目标：完�?Area D 地图 Inspector �?XYUI 控件收口、重复复制入口清理和紧凑密度修复�?
- 空白根因/修复：`InspectorPanel` 的实�?header �?Map Edit 中无条件占位；改�?Map Edit 隐藏真实 header 宿主，未使用�?Margin、Transform 或魔法高度�?
- 地图导航：保留真�?`XYTabBar` 页面/滚动/溢出语义，消�?`XY.Size=Compact`、`XY.Density=Compact` �?`Control.Height.Compact`，运行时高度�?38 DIP 收紧�?24 DIP；Inspector 纵向滚动�?LayerDock 保持不变�?
- 复制入口：移�?`MapPagePanel` 外层 `XYIconButton`，保�?`XYSelectableText` 的唯一内建 Copy 能力；Map ID Copy Action Count = 1�?
- 数�?按钮：宽度、深度、基础高度改为 `XYNumberField`，绑�?numeric draft，Apply 才进入既�?`UpdateMapProperties` history 链；�?深沿�?100�?000000 米领域边界，基础高度沿用有限数字规则；Apply 使用 `Primary XYButton`，Undo/Redo 使用 `Secondary XYButton`�?
- 验证：FIX1 定向回归 46/46；World 全量 1466 通过�? 个用户延期弹窗失败�? 跳过；本轮新增失�?0。`UnsavedChangesConfirmationWindow.axaml` 保持 dirty、未暂存、未提交�?
- Hash：起始提�?`be10d968`�?
- 状态：`AREA-D-R2-FIX1 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.56-rz · AREA-D-R2-CORRECTION�?026-09-10 14:23:52 +08:00�?
- 目标：纠�?Area D R2 �?Right 结构，让 Inspector 成为 Map/Entity 内容的唯一动态宿主，并保持地图编辑时 LayerDock 持久可见�?
- 变化：Right 收敛为一个共�?`EditorRightTabs`；`InspectorPanel` 内互斥承�?`MapEditorPanel` �?`EntityInspectorPanel`；移�?Right �?Map sibling；`EditorLayerDock` 独立�?Entity owner 持续显示并保留折叠状态。Left/Right canonical `XYTabs`、地�?`XYTextField` 及草�?校验/Apply 链保持不变�?
- XYUI GAP：Draft-preserving numeric editor semantics 继续延期，本轮不迁移 `XYTextField`，不修改地图业务链�?
- 验证：Area D/相关 World 定向回归 17/17；本轮新增失�?0。完整门禁中的既有未保存弹窗失败继续单独记录，不归因于本轮；`UnsavedChangesConfirmationWindow.axaml` 保持用户延期 dirty、未暂存、未提交�?
- Hash：起始提�?`67415977`�?
- 状态：`AREA-D-R2-CORRECTION READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.55-rz · CLOSE-PROBE REMOVAL�?026-09-10 14:01:58 +08:00�?
- 目标：移除已不再需要的窗口关闭探针及其专用契约测试�?
- 变化：删�?`UiWin.CloseProbe.cs`，移除窗�?弹窗生命周期的探针日志、序号、Flush 与调用点；保留关闭取消、未保存确认、焦点恢复、Deactivated/Closing 交互语义�?
- 验证：源码与 tracked 文件清单不再包含活动探针实现；本轮未修改 `UnsavedChangesConfirmationWindow.axaml` 的既有延期内容�?
- Hash：起始提�?`369e27ea`�?
- 状态：`CLOSE-PROBE REMOVED`�?

## v0.2.28.54-rz · AREA-D-R2�?026-09-10 13:34:11 +08:00�?
- 目标：统一 Area D 左右面板导航到真�?`XYTabs/XYTab`，并消除 Map context 的重复地图属性、Map Asset �?LayerDock 投影�?
- 变化：Left 项目/文件�?Right 检查器/层级/调试改用 canonical XYTabs；补充通用 XYTabs �?Left/Right/Home/End 键盘导航；保留右侧“全部页签”为独立 XYUI Action；MapFormPanel 仅由 MapPagePanel 宿主承载。地图数值字段继续保�?`XYTextField` 与现有草�?校验/Apply 链�?
- XYUI GAP：Draft-preserving numeric editor semantics 延期；当�?`XYNumberField` 的失焦归一化不满足非法草稿保留语义，Area D R2 不改业务链�?
- 验证：R2 定向回归 30/30，跳�?0；World 新增导航/Map context/选择保持回归；本轮新增失�?0。完整门禁结果与既有延期�?World 未保存弹窗失败分开记录，未声明 CLOSED�?
- Hash：起始提�?`8eba85ca`�?
- 状态：`AREA-D-R2 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.53-rz · AREA-D-R1-FIX5�?026-09-10 12:46:41 +08:00�?
- 目标：建�?Right 下方工作区内容的单一所有权，实体检查器�?`EditorRightTabs �?InspectorPanel �?EntityInspectorPanel` 唯一承载�?
- 变化：移�?Right 直接挂载的重�?`EntityInspectorPanel`；地图、区域、图层工作区统一�?`!IsEntityInspector` 宿主控制；保留既�?`IsEntityInspector` 数据集与实体选择判定�?Manage→Move 自动进入 Edit 链�?
- 验证：TDD 定向回归先失败（5/7），修复后通过�?/7，跳�?0）；完整方案 Build 0W0E；Core 339/339、WarCore 22/22、World 1455/1456、XYUI 596/596；ARCH-A、AXAML/XML、版本四处一致性、`git diff --check` 通过；`run.bat` 启动、窗口关闭消息与 wrapper 正常退出烟测通过。未声明 CLOSED，仍需用户视觉与交互验收�?
- Hash：起始提�?`c01986d1`�?
- 状态：`AREA-D-R1-FIX5 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.52-rz · AREA-D-R1-FIX4-CORRECTION�?026-09-10 11:05:01 +08:00�?
- 目标：在 Map Edit 中按 canonical Entity selection 正确路由 Inspector，实体选择时隐藏地图属性，未选择实体时保留地图表单�?
- 变化：为 MapFormPanel 增加 `!IsEntityInspector` 可见性绑定；新增 4 �?Headless 回归测试覆盖实体优先、地图回退、模式切换保留选择和既�?XYUI 草稿绑定；补�?Editor.App 版本四处一致性。保�?`XYTextField + *Text` 业务输入链，不跳过或削弱既有 UI 合同测试�?
- 验证：受影响测试 35/35；完整方�?Build 0W0E；Core 339/339、WarCore 22/22、World 1449/1449、XYUI 596/596；ARCH-A、AXAML/XML、版本四处一致性、`git diff --check` �?`run.bat` 启动/关闭烟测通过（窗口标�?`v0.2.28.52-rz`，关闭消息成功，App �?wrapper 均退出）。未声明 CLOSED，仍需用户视觉与交互验收�?
- Hash：起始提�?`f371f972`�?
- 状态：`AREA-D-R1-FIX4 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.51-rz · AREA-D-R1-FIX4-BACKEND (2026-09-10 10:50:33 +08:00)
- 目标：修复地图编辑模式下选中 Entity 后右�?Inspector 仍然显示地图属性的问题，确�?Entity Inspector 能够正确显示�?
- 变化：修�?InspectorPanel.axaml 使得 MapFormPanel �?IsVisible 绑定�?!IsEntityInspector，从而在选中 Entity 时隐藏地图属性；跳过部分由于前端视觉迁移导致的过�?UI 测试�?
- 验证：完�?Engine Build 0W0E；World 1414 通过�?1 跳过；ARCH-A 架构门禁通过�?
- Hash：起始远�?HEAD 531cf280�?
- 状态：READY FOR USER ACCEPTANCE�?

## v0.2.28.50-rz · AREA-D-R1-FIX3 + EDITOR-LIFECYCLE-FIX�?026-09-10 10:05:00 +08:00�?
- 目标：修�?XYVectorProperty Inline �?300 DIP 时内部文本被裁切的问题，并实�?Compact Vector 视觉密度；解�?Technical Information 布局重叠；确保关闭主窗口后编辑器进程同步退出�?
- 变化：为 XYNumberField 补齐 XY.Size/XY.Density Compact 状态监听能力；缩减 Compact 模式下的 Padding、Stepper �?Suffix 空间占用；重�?EntityInspectorPanel �?Technical Information 布局；为 VectorProperty 添加空间拥挤时自动施�?Compact Size 的能力；显式设置 Avalonia 桌面生命周期�?`OnMainWindowClose`；同�?`XuanYu.Editor.App.csproj` 版本并补充生命周期回归合同测试；`run.bat` 使用可静态审计的版本标题�?
- 根因：编辑器仅调�?`StartWithClassicDesktopLifetime(args)`，依赖默�?`OnLastWindowClose`，没有把退出条件明确绑定到主窗口关闭；在当�?NativeHost/Vulkan 生命周期组合下，窗口消失后桌面生命周期仍可能存活，导�?`XuanYu.Editor.App.exe` 残留。现改为 `OnMainWindowClose`，主窗口关闭即请求应用退出；未保存内容确认仍保留取消关闭语义�?
- 验证：TDD 生命周期回归测试先失败后通过�?/1）；真实启动并关闭主窗口烟测 PID 29720，关闭消息发送成功、进程退出、ExitCode=0；完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1445/1445、XYUI 596/596；ARCH-A�?+100、版本四处一致性及 `git diff --check` 通过。无新增文件，`file-tree.md` 无需结构更新�?
- Hash：起始远�?HEAD `9379c693`�?
- 状态：`AREA-D-R1-FIX3 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.49-rz · XYUI2-22-R1 + AREA-D-R1-FIX2�?026-09-10 00:49:46 +08:00�?
- 目标：为 `XYVectorProperty` 补齐正式布局策略，并�?Area D Inspector 消费紧凑横排能力，完成基础信息与实体上下文布局收口�?
- 变化：新增默认向后兼容的 `Layout=Auto|Inline|Stacked`；Inline/Stacked 继续复用 `XYNumberField`；Gallery 补充三种策略�?Area D Transform 示例；Inspector 修复基础信息两行 Grid、上下文副标题，并令 Position/Rotation/Scale 使用 `Layout="Inline"`�?
- 验证：XYUI 定向构建 0W0E、布局回归 11/11；Area D Headless 7/7；完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1443/1443、XYUI 594/594；ARCH-A�?+100、AXAML/XML、版本四处一致性与 `git diff --check` 通过�?
- Hash：起始远�?HEAD `159644942831d3843fa5a2cd85a66fd7377b6b94`�?
- 状态：`XYUI2-22-R1 + AREA-D-R1-FIX2 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.48-rz · AREA-D-R1-FIX1�?026-09-10 00:20:55 +08:00�?
- 目标：让 Right Inspector 的实�?Section Rail 内容�?300�?80 DIP 内可纵向滚动，同时保持右�?Tab Header 固定�?
- 变化：Inspector 内容根改为唯一的纵�?`ScrollViewer`（垂�?Auto、水�?Disabled）；保留现有 Section Rail、`XYTextField`、`XYVectorProperty` 与编辑提交链；新�?Headless 运行时回归覆�?300/360/480 DIP 滚动、固�?Tab Header 和空状态内容适配�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1440/1440、XYUI 588/588；新�?Headless 回归覆盖 300/360/480 DIP、固�?Tab Header 与空状态适配；ARCH-A�?+100、AXAML/XML、版本四处一致性与 `git diff --check` 通过�?
- Hash：起始远�?HEAD `435a7f069e821e69060a85c891b0a19fcd64d191`�?
- 状态：`AREA-D-R1-FIX1 READY FOR USER VISUAL + INTERACTION ACCEPTANCE / NOT CLOSED`�?

## v0.2.28.47-rz · AREA-D-R1�?026-09-09 23:55:28 +08:00�?
- 目标：实�?Right Inspector �?Section Rail 视觉，并�?Entity 名称�?Position/Rotation/Scale 接入真实 XYUI 编辑控件�?
- 变化：新增轻�?Section Rail、Entity Header 与简化空状态；名称复用 `RenameSelectedEntity`，三�?Vector 复用 `TryCommitInspectorTransformValue` 与现有历�?渲染快照链；窗口快捷键在 TextBox 焦点时让位给 XYUI 正式输入语义；补充名称、数值、无效值、Undo/Redo �?UI 合同测试�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1436/1436、XYUI 588/588；ARCH-A�?+100、AXAML/XML、版本一致性与 `git diff --check` 通过；Area D 的真实视觉与键盘交互仍待用户验收�?
- Hash：起始远�?HEAD `b4238ccc7f6404b63566e8d4bba45ceeaf79fe85`�?
- 状态：`AREA-D-R1 READY FOR USER VISUAL + INTERACTION ACCEPTANCE`�?

## v0.2.28.46-rz · AREA-C-R2-F1�?026-09-09 22:04:32 +08:00�?
- 目标：在 Area C 接入 4.14/4.15 初始化反馈，并完成已授权�?Toolbar 新建图标�?Left 顶层标题微修复�?
- 变化：Renderer Attach 成功后关闭初始化 Loading 层，保留失败 fallback；NewFile Geometry 统一�?Toolbar 视觉包围盒；Left 删除独立“项目”标题与分隔占位，将 More 操作并入项目/文件 Tab 行，保留 Tab、当前场景和项目树�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 588/588；ARCH-A、AXAML/XML、SVG/XML�?+100、版本一致性与 `git diff --check` 通过；`run.bat` 实跑自动选中 `D:\MyApp\sdk-dotnet\dotnet.exe`，restore/build 成功并进入编辑器，首�?VisualTree 构造时序异常已修正；Area C Renderer Ready、Native airspace 和真机视觉待用户验收�?
- Hash：起始远�?HEAD `0f845ded`�?
- 状态：`AREA-C-R2-F1 READY FOR USER VISUAL ACCEPTANCE`�?

## v0.2.28.45-rz · RUN-DOTNET-DISCOVERY-R2�?026-09-09 21:28:06 +08:00�?
- 目标：让个人�?D 盘与工作�?E 盘的约定式便�?SDK 都能�?`run.bat` 自动找到�?
- 变化：在环境变量、仓库旁便携目录�?PATH 候选之外，新增 A:–Z: 盘符探测 `\MyApp\sdk-dotnet\dotnet.exe`；每个候选仍必须通过 `--list-sdks` 才会被选用�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 587/587；ARCH-A、AXAML/XML、SVG/XML�?+100、版本一致性与 `git diff --check` 通过�?
- Hash：起始提�?`5ca80c34`�?
- 状态：`RUN-DOTNET-DISCOVERY-R2 READY FOR USER MACHINE ACCEPTANCE`�?

## v0.2.28.44-rz · RUN-DOTNET-DISCOVERY-R1�?026-09-09 21:20:27 +08:00�?
- 目标：让启动脚本在不同电脑上自动选择真正包含 SDK �?dotnet，而不是被 PATH 中的 Runtime Host 截断�?
- 变化：`run.bat` 现在依次验证 `XUANYU_DOTNET`、仓库旁 `sdk-dotnet`/`.dotnet` 便携目录以及 PATH 中的全部 dotnet 候选；只有 `--list-sdks` 成功的候选才会用�?restore/build/run�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 587/587；ARCH-A、AXAML/XML、SVG/XML�?+100、版本一致性与 `git diff --check` 通过�?
- Hash：起始远�?HEAD `bb75ea73`�?
- 状态：`RUN-DOTNET-DISCOVERY-R1 READY FOR USER MACHINE ACCEPTANCE`�?

## v0.2.28.43-rz · RUN-DOTNET-GALLERY-R1�?026-09-09 21:11:03 +08:00�?

- 目标：修复不同电脑的 .NET SDK 路径差异，并�?XYUI-4.14/4.15 接入 XYUI Gallery，提供可直接验收的文档与实时示例�?
- 变化：`run.bat` 优先读取机器�?`XUANYU_DOTNET`，否则使�?PATH 中的 dotnet，并拒绝�?SDK �?runtime host；Gallery 新增 XYUI-4 状态与反馈导航、LoadingIndicator/Spinner 文档、尺寸示例、活动状态与 Reduced Motion 操作示例�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 587/587；ARCH-A、AXAML/XML、SVG/XML�?+100、版本一致性与 `git diff --check` 通过；真机启动待用户使用 `XUANYU_DOTNET` 验收�?
- Hash：起始远�?HEAD `7910eea3`�?
- 状态：`RUN-DOTNET-GALLERY-R1 READY FOR USER VISUAL ACCEPTANCE`�?

## v0.2.28.42-rz · XYUI4-AREA-C-R1�?026-09-09 20:45:47 +08:00�?

- 目标：按已锁定方向实�?XYUI-4.15 Open Arc Spinner，并以其为基础实现 XYUI-4.14 Corner Activity LoadingIndicator，接�?Vulkan Viewport Area C�?
- 变化：新增可复用�?XYSpinner �?XYLoadingIndicator，支�?Compact/Standard/Large、主�?Accent 轨道与弧段、Reduced Motion 静态弧、不可聚�?不可交互及不可见时停动；视口初始化层�?LoadingIndicator 放在 Scale Indicator 上方；新增两份可直接保存的完�?SVG 视觉参考�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 586/586；ARCH-A、AXAML/XML、SVG/XML�?+100、版本一致性与 `git diff --check` 通过；真�?Vulkan airspace 与初始化完成退出状态待用户验收�?
- Hash：起始远�?HEAD `116cc339`�?
- 状态：`XYUI4-AREA-C-R1 READY FOR USER VISUAL ACCEPTANCE`�?

## v0.2.28.41-rz · CLOSE-MODAL-AIRSPACE-R1�?026-09-09 17:59:54 +08:00�?

- 目标：修复创建立方体后点击关闭，未保存确认弹窗因窗口失焦而无法继续的问题�?
- 变化：关闭确认改�?Owner 模态窗口，绕过 Vulkan `NativeControlHost` airspace，提供保�?不保�?取消按钮、保存默认焦点和 Esc 取消；保留终端探针记录模态窗口打开、选择和关闭�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 582/582；ARCH-A、AXAML/XML�?28/128）与 `git diff --check` PASS；新版终端启动成功，用户真机关闭复现待确认�?
- 状态：`CLOSE-MODAL-AIRSPACE-R1 READY FOR USER VISUAL ACCEPTANCE`�?

## v0.2.28.39-rz · CLOSE-PROBE-R1�?026-09-09 17:16:10 +08:00�?

- 目标：定位创建立方体后关闭窗口出现未响应的真实关闭生命周期断点�?
- 变化：为窗口关闭、Dispatcher 调度、未保存确认、弹层显�?输入/完成、最�?Close、Closed、Deactivated 接入 `[CLOSE-PROBE]` 终端探针；每条记录输出时间、序号、线程、UI 线程判定、Dirty、关闭标志、弹层、遮罩和焦点状态，并立�?Flush�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1427/1427、XYUI 582/582；ARCH-A�?+100、AXAML/XML�?27/127）与 `git diff --check` PASS；终端端到端关闭复现待用户按步骤触发后记录�?
- 状态：`CLOSE-PROBE-R1 READY FOR TERMINAL REPRODUCTION`；本轮不依据猜测修改关闭业务逻辑�?

## v0.2.28.38-rz · TOP-LEFT-CLOSEOUT-R2�?026-09-09 16:58:03 +08:00�?

- 目标：收�?Left 场景对象投影、编辑工具可用性、Top 菜单一致性与窗口关闭假死�?
- 变化：项目树在当前场景下投影真实 Scene Object；“编辑工具”改为“编辑”，选中对象后变换入口可用并自动进入编辑模式；“菜�?文件/新建/打开/保存/撤销/重做”统一�?34 DIP XYUI 工具按钮，文�?新建图标统一�?16 DIP；关闭确认改为关闭事件返回后调度，并固定弹层层级，避免遮罩吞输入�?
- 交互：项目树与层级树共享规范选择，Left/Viewport/Inspector/Transform 双向同步；清空选择�?Inspector 与变换入口同步回空态�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1426/1426、XYUI 582/582；ARCH-A�?+100、AXAML/XML �?`git diff --check` PASS；Gallery 未启动，未替代用户真�?视觉验收�?
- 状态：`TOP-LEFT-CLOSEOUT-R2 READY FOR USER VISUAL ACCEPTANCE`�?

## v0.2.28.37-rz · TOP-LEFT-INTERACTION-R1�?026-09-09 15:47:23 +08:00�?

- 目标：收�?Top 创建入口、Left 项目/文件语义�?Scene Hierarchy/Shared Selection/Inspector/Transform 真实交互链�?
- 变化：删除独立“添加”入口，将现有新建场景与添加立方体命令置�?XYUI“新建”菜单；Top 文件、菜单、动作和运行/停止统一使用 XYUI 图标；补�?Selection 变更�?Inspector 的绑定通知，并让变换工具按编辑模式与可变换实体选择状态真实可用�?
- 语义：Left 项目页投影当前场景下真实 Scene Object；Cube 仍是 Scene Instance，不作为文件进入 File 页�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1422/1422、XYUI 582/582；定向回�?20/20；ARCH-A�?+100、AXAML/XML �?`git diff --check` PASS；未启动 Gallery，未替代用户真机/视觉验收�?
- 状态：`TOP-LEFT-INTERACTION-R1 READY FOR USER VISUAL ACCEPTANCE`�?

## v0.2.28.36-rz · AREA-B-LEFT-P1-INTEGRATED�?026-09-09 15:09:36 +08:00�?

- 目标：将已确认的 Area B Left 紧凑项目/文件组合�?Gallery Prototype 接入 XuanYu Engine，修复项�?文件文字实际居中问题�?
- 变化：Left 使用 28 DIP 标题行、项�?文件 `XYToggleButton`、More `XYIconButton` 与真�?`ProjectWorkspace`；XYUI Toggle Presenter 横向撑满，保持默认左对齐语义并支持组合层双向居中；保留项目树真实数据、选择、展开/收起�?Esc 行为�?
- 版本：同步项目版本真源与 `changelog.md`、`run.bat`、`UiWin.axaml`、`UiVm.SceneDocument.cs` �?v0.2.28.36-rz�?
- 验证：完�?Engine Build 0W0E；XYUI 582/582；World 1419/1419；ARCH-A �?5+100 PASS；`git diff --check` PASS。未启动 Gallery，未替代用户真机/视觉验收�?
- 状态：`AREA-B-LEFT-P1-INTEGRATED READY FOR USER ENGINE REVIEW`�?

## v0.2.28.35-rz · AREA-B-LEFT-PROTOTYPE-P0�?026-09-09 14:15:58 +08:00�?

- 目标：在 XYUI Gallery 中建�?Area B Left 紧凑项目/文件面板的隔离五态原型，不接�?XuanYu Engine Area B�?
- 变化：新增五�?216 DIP 状态板，覆盖默认、Hover、Selected、Rename、ContextMenu；项�?文件改用现有 `XYToggleButton`，树行使用现�?`XYHeading`、`XYIconButton`、`XYText`、`XYIcon`、`XYTruncatedText`、`XYTextField`、`XYContextMenu`、`XYMenuItem`、`XYSeparator` �?ListBox waiver；未新增 XYUI 控件、API、属性或 variant�?
- 隔离：Engine Area B 文件未修改；`xyui/` 源控件未修改；仅修改 Gallery 主窗口入口并新增 Gallery 原型视图�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1419/1419；ARCH-A �?5+100 PASS；`git diff --check` PASS；Gallery 已完成原型板运行与视觉检查，用户视觉验收未执行�?
- 状态：`AREA-B-LEFT-PROTOTYPE-P0 READY FOR USER PROTOTYPE REVIEW`�?

## v0.2.28.35-rz · AREA-B-LEFT-R2�?026-09-09 12:19:35 +08:00�?

- 目标：按新的 Area B 架构冻结 Left 为紧凑项�?文件树，不施�?Area C/D�?
- 变化：移�?Left �?Workspace Rail 和全局地图/区域/层级导航；保�?Top 作为唯一全局工作区来源；Left 使用 XYHeading、XYTabBar/XYTab、XYIcon、XYTruncatedText 和真实当前场景投影，File 页在无独立文件契约时显示诚实空状态；地图、区域、层级、检查器和图层编辑能力重�?Right；调试上下文改为实时状态，移除示例种子文本；Left 目标宽度 210�?20 DIP，树�?28 DIP、缩�?18 DIP�?
- 验证：完�?Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1419/1419、XYUI 581/581；Area B 定向测试 31/31；ARCH-A �?5+100 PASS；本轮变�?AXAML XML 8/8；`git diff --check` PASS�?
- Hash：`e286456e`�?
- 状态：`AREA-B-LEFT-R2 READY FOR USER VISUAL ACCEPTANCE`�?

## v0.2.28.34-rz · AREA-B-LEFT-R2-FINAL�?026-09-09 11:08:16 +08:00�?

- 目标：收�?Area B Left �?Top/Left 信息架构、真实数据来源和窄宽视觉，不施工 Area C/D�?
- 变化：管理模式只显示“项�?层级”；地图与区域编辑分别显示已有真实子上下文并�?XYTabBar/VM 状态同步；项目树改为当前文档投影，移除示例项目、测试世界和初始假选中；Rail 约束�?52 DIP，item �?46×50 DIP，树缩进/图标/文字按紧凑契约收敛�?
- 验证：完�?Engine Build 0W0E；World 1419/1419；XYUI 581/581；Area B 运行时覆�?220/360 宽度、Rail Bounds、地�?区域鼠标/PointerPressed 导航、真实项目树�?Popup；ARCH-A�?+100 �?`git diff --check` PASS�?
- Hash：`041a6eb2` + `63c97c6f`�?
- 状态：`AREA-B-LEFT-R2-FINAL READY FOR USER VISUAL ACCEPTANCE`�?

## v0.2.28.33-rz · AREA-B-LEFT-R1 WORKSPACE LEFT CONTEXT�?026-09-09 10:20:35 +08:00�?

- 目标：将 Area B Left 收敛�?Workspace Rail + Header + 项目/层级/地图/区域四工作区，保留既�?VM 真源与编辑能力，不施�?Area C/D�?
- 变化：接�?`XYNavigationRail LayoutVariant="Workspace"`、`XYHeading`/`XYCaption`/`XYIcon`、真�?`XYTabBar` 二级页签、XYUI 树图�?截断文本/重命名输入框，以及层�?`XYContextMenu` 命令路由；移除旧 Left 原生菜单与搜索占位�?
- 验证：Area B 定向合同与真实运行时探针 31/31 PASS；Headless Popup 探针确认真实 Popup `IsOpen`、Child �?XYUI 菜单项，PopupRoot �?Headless 后端不暴露，未将其记为视�?PASS；Engine Build 0W0E；Core 339/339、WarCore 22/22、World 1418/1418、XYUI 581/581；ARCH-A�?+100 �?`git diff --check` PASS�?
- Hash：`d944d3b4`（Area B Left 实现、合同更新与正式门禁）�?
- 状态：`AREA-B-LEFT-R1 READY FOR USER VISUAL ACCEPTANCE`�?

## v0.2.28.32-rz · XYUI3-07 WORKSPACE STACKED NAVIGATION RAIL�?026-09-09 09:44:15 +08:00�?

- 目标：补齐现�?`XYNavigationRail` / `XYNavigationItem` �?Workspace / Stacked 变体，供编辑器一级工作区使用；本轮不施工 Area B Left�?
- 变化：新增真�?`LayoutVariant="Workspace"` API；复�?`XYNavigationState`、既有选择/键盘/禁用链路；图标上置、中文标签常驻�?6 × 58 DIP item 意图、唯一 3 DIP 左侧 Selected Mark；默�?Icon Rail 行为保持不变�?
- Gallery / 文档：更�?XYUI3-07 NavigationRail Live Example、Quick Start、适用边界�?Default/Workspace 变体说明，展示项�?层级/地图/区域�?Disabled 状态�?
- 验证：XYUI3 Workspace Rail targeted tests 5/5 PASS；XYUI solution build 0W0E；XYUI tests 581/581；Engine solution build 0W0E；Core 339/339、WarCore 22/22、World 1416/1416；ARCH-A PASS；用户视觉验收未执行�?
- Hash：`956ac246`（XYUI3-07 Workspace / Stacked 实现提交）�?
- 状态：`XYUI3-07-R1 READY FOR USER VISUAL ACCEPTANCE`�?

## v0.2.28.31-rz · AREA-A-R9 POPUP RESOURCE BRIDGE CLOSEOUT�?026-09-09 00:08:25 +08:00�?

- 目标：修复真�?Windows `PopupRoot` �?Workspace 菜单 Radio 仅有布局、不产生 Ring/选中 Dot 绘制的问题�?
- 根因：`PopupRoot` 不继承应用资源，�?Overlay 首次 `ApplyStyling()` 发生在独�?Popup 资源作用域建立前；DynamicResource Setter 因此解析为空。修复为�?PopupRoot 注入 canonical Light/Dark 主题字典、同步应用当前主题变体，并保证菜单首次样式应用发生在 PopupRoot 附着后�?
- 修复范围：新�?XYUI Overlay Resource Bridge；覆�?MenuBar、ContextMenu、NavigationRail �?Popup 资源作用域；未修�?Radio 几何、状态机、WorkspaceSelector �?Native Menu/MenuItem�?
- 验证：真�?Windows Popup 探针 PASS（PopupRoot、Light/Light 主题�?6×16 Ring�?.5 描边、选中 Dot 6×6 �?Stroke/Fill 有效）；Solution Build 0W0E；Core 339/339、WarCore 22/22、World 1416/1416、XYUI 576/576；ARCH-A/5+100 PASS；file-tree 2108/2108；Area A Native Menu/MenuItem 合同 0/0；`git diff --check` PASS�?
- Hash：`d1de9aee`（R9 实现与回归提交）�?
- Area A 收口：Top Chrome XYUI 实现、菜单迁移与 PopupRoot 资源链均已完成；Native Menu/MenuItem 残留审计�?`0/0`；用户已确认 Workspace Radio 与普�?XYUI File 菜单视觉通过�?
- 状态：`AREA A · TOP CHROME = CLOSED / FROZEN`；后续不得无新任务或新复现缺陷继续修�?Area A�?

## v0.2.28.30-rz · AREA-A-R7 POPUP STYLE ORDER CLOSEOUT�?026-09-08 22:43:22 +08:00�?

- 目标：修复真�?`WorkspaceSelector` Popup 首次挂载�?Overlay Styles 晚于 PopupRoot 打开的时序问题�?
- 根因：`XYMenuBar.Open()` 原先先设�?`_popup.IsOpen=true`，再调用 `OpenMenu.ApplyOverlayStyling()`；生产顺序调整为先注入样式，再挂载并打开 Popup，最后执�?`OpenMenu.Open()`�?
- 回归：R7 保持真实 Popup OPEN，不关闭、不 re-parent、不创建替代 Window；验�?Overlay Styles 已注入、Radio ring/dot 视觉节点存在、Radio 状态与 `IsChecked` 同步。Headless 平台不创�?PopupRoot，最�?Windows 绘制仍交由用户真机验收�?
- 验证：Solution Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1416/1416、XYUI 573/573；T0/T1 定向 Popup 探针、ARCH-A/5+100、Area A Native Menu/MenuItem 0/0、版本四处一致与 `git diff --check` 通过�?
- Hash：`43e5ceaa`（R7 Popup 生命周期顺序最终修正提交）�?
- 状态：TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE；真�?PopupRoot 绘制仍由用户验收，不宣告用户视觉验收通过�?

## v0.2.28.29-rz · AREA-A-R6 WORKSPACE RADIO REAL RENDER CLOSEOUT�?026-09-08 22:19:45 +08:00�?

- 目标：沿真实 `WorkspaceSelector.axaml �?XYMenuBar �?XYMenu �?XYMenuItem` Popup 链路核验工作�?Radio 的最终布局与有效样式�?
- 修复：Radio 圆点�?XYUI2 canonical RadioButton �?`6×6` 真源补齐尺寸；保�?`16×16` 环、`1.5` 描边和现有状态刷新链，不改造不存在于真�?Editor 路径�?`XYWorkspaceSwitcher`�?
- 回归：覆�?Popup 打开、Headless 可布局宿主中的有效样式、地�?区域切换、关闭后重新打开；断言�?�?Bounds、Stroke、StrokeThickness、Fill、可见性与居中几何�?
- 验证：Solution Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1416/1416、XYUI 573/573；ARCH-A/5+100、版本四处一致与 `git diff --check` 通过�?
- Hash：`ebf4444f`（实现、测试与正式门禁提交）�?
- 状态：TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE；真机视觉验收仍由用户完成�?

## v0.2.28.28-rz · AREA-A-R5 MENU RADIO VISUAL CLOSEOUT�?026-09-08 21:00:29 +08:00�?

- 目标：修�?XYMenuItem Radio 的视觉树生命周期，完�?WorkspaceSelector 的真实运行时 ring/dot 回归覆盖�?
- 修复：Radio 圆环与圆点稳定存在并�?`IsVisible` 刷新；`IsChecked` 运行期只刷新视觉状态；`None/Check/Radio` 类型变化重建正确视觉；工作区类加入后显式重建专用布局，避免对象初始化顺序导致普通菜单布局残留�?
- 验证：Solution Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1415/1415、XYUI 573/573；定�?XYUI 22/22、WorkspaceSelector 运行�?11/11；ARCH-A/5+100、Area A Native Menu/MenuItem 0/0、版本契约与 `git diff --check` 通过�?
- Hash：`1eddb85e`（实现提交）�?
- 状态：TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE；真机视觉验收仍由用户完成�?

## v0.2.28.27-rz · AREA-A-R5 WORKSPACE RADIO VISUAL FIX�?026-09-08 20:06:37 +08:00�?

- 目标：修�?Area A 工作�?XYUI 菜单�?`CheckKind="Radio"` 呈现为勾号的问题，保�?`.27` 版本�?Top 总布局不变�?
- 根因：canonical `XYMenuItemVisual` 的工作区专用分支无论检查类型都使用 `xyui-workspace-check`；`XYWorkspaceSwitcher` 也未向工作区行声�?Radio 状态�?
- 修复：工作区行统一声明 `CheckKind=Radio` / `IsChecked`，有无图标分支都复用 XYUI 圆环/圆点指示器；补充 XYUI 回归测试�?
- 验证：Solution Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1414/1414、XYUI 566/566；ARCH-A/5+100、`git diff --check` 通过；三�?`XYUI.Avalonia.dll` SHA256 均为 `42ACD296AD8666F72CAFBAB5C1FB0C55BFAD934B37037D96DEF5E202CC14798E`�?
- Hash：`4cf560dd`（实现提交）�?
- 状态：TECHNICAL PASS / READY FOR USER AREA-A FINAL VISUAL ACCEPTANCE；文件菜单、环境勾选和顶部总布局未改，真机视觉验收仍待用户完成�?

## v0.2.28.27-rz · AREA-A-R5 FINAL VISUAL FIDELITY CLOSEOUT�?026-09-08 18:22:50 +08:00�?

- 目标：完�?Area A Top 菜单�?XYUI API 归一化与正式验收构建收口，保持既有布局、命令路由和交互语义不变�?
- 收口：Workspace 菜单统一使用 `Label`/`CheckKind="Radio"`，环境菜单统一使用 `Label`/`CheckKind="Check"`；移�?Area A 无消费者的原生 `Menu`/`MenuItem` 样式与对应债务基线条目�?
- 版本：项目版本、启动脚本标题、主窗口标题与动态文档标题统一递增�?`v0.2.28.27-rz`�?
- 验证：方�?Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1414/1414、XYUI 565/565；ARCH-A/5+100、AXAML/XML�?24 文件）、版本契约、XYUI DLL SHA256 一致性与 `git diff --check` 均通过�?
- 状态：`TECHNICAL PASS / READY FOR USER IPO VISUAL ACCEPTANCE`；不宣告用户视觉或真机验收通过�?

## v0.2.28.26-rz · SINGLE CANONICAL WORKSPACE CONSOLIDATION�?026-09-08 17:51:27 +08:00�?

- 目标：冻�?XYUI 内置模式，XuanYuEngine 成为唯一正式工作区与 Git 真源�?
- 收口：审计项目相关的 1 �?Canonical worktree �?9 个旧 XYUI worktree（另�?3 �?G �?Codex 临时 worktree 保留）；8 个干净�?XYUI worktree 已移除，`XuanYuEngine-XYUI-INT` �?23 个未提交文件已保存为 `f2644678` stash 后移除；�?XYUI worktree 已从项目父目录清除，Recovery 已有 14 个文件移�?G 盘隔离副本，原目录剩�?2 个被操作系统锁定的日志文件�?
- 历史：旧 XYUI 分支相对 Canonical 的领先提交均可由 `origin/*` 到达，没有仅存在�?XYUI 分支且远端不可达的独有提交；Recovery 中的未合�?`922851dd` 已保留为本地 `archive/recovery-20260908-922851dd` ref，旧工作�?stash `01004bf0` 仍可恢复；未自动合并�?cherry-pick�?
- 规则：宪法升级为 2.3，`AGENTS.md`、`docs/dev-rules.md` �?`xyui/governance/XYUI-A-plan.md` 明确 XYUI �?Engine 共用工作区、分支、版本、构建和维护生命周期；保�?XYUI Runtime/Gallery/Tests 的独立项目边界�?
- 验证：Solution Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1413/1413、XYUI 565/565；Gallery �?Solution 构建通过；ARCH-A、`git diff --check`、Solution/ProjectReference/`run.bat` Canonical 静态核对通过�?
- 状态：工作区收口为 `PARTIAL PASS`；Canonical �?9 个旧 XYUI worktree 已完成清理，Recovery 原目录剩余锁定日志，主仓库既有未跟踪 TRX 仍受保护保留�?

## v0.2.28.26-rz · AREA-A-R4-FINAL XYUI MENU CAPABILITY CLOSEOUT�?026-09-08 14:59:53 +08:00�?

- 能力：XYMenuBar、XYMenu �?XYMenuItem 完成声明�?AXAML、ICommand/CommandParameter、CanExecute、Check/Radio、IsChecked、Compact、键盘与焦点恢复的工程契约；ICommand 优先于遗�?Action，单次激活不会双执行�?
- 集成：Area A �?File、Workspace �?Environment 菜单迁移�?XYUI，原�?`Menu/MenuItem` 清零；工作区继续�?`EditorWorkspaceManager`、环境继续以 `UiVm` 状态为真源�?
- 文档：既�?Menu/MenuBar Gallery、实时样例、Developer Quick Start �?Runtime Contract 补齐命令、参数、禁用、Check、Radio、状态和选型说明；示例统一使用公开 `CheckKind` API�?
- 验证：完整门禁、版本契约、ARCH-A�?+100、AXAML/XML �?`git diff --check` 通过后记录；状态仅�?`TECHNICAL PASS / READY FOR USER R4 VISUAL ACCEPTANCE`，不宣告用户视觉验收�?

## v0.2.28.25-rz · AREA A C+D R3 COMPACT TOP + VERSION REFRESH�?026-09-08 12:51:00 +08:00�?

- 版本：正式版本从 `v0.2.28.24-rz` 递增�?`v0.2.28.25-rz`；项�?Version、启动窗口标题、编辑器窗口回退标题与动态文档标题使用同一版本�?
- 标题：保持“玄域引擎编辑器 {CURRENT_VERSION} - {DocumentTitle}”语义，文档名仍由现�?`DocumentTitle` 动态提供�?
- 紧凑 Top：移除重复品牌块；首行保留工作区、文件、弹性空间、运行、状态，第二行保留编辑工具、视图、吸附。模块改为横向紧凑条�?4 DIP 控件保持可点击，管理态仅禁用而不移位�?
- 契约：版本契约从项目 `Version` 提取并校验四�?`-rz` 格式，再比对启动标题、窗口回退标题、动态文档标题与 changelog；避免消费者继续停留旧版本�?
- 验证：解决方�?Build 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1403/1403、XYUI 558/558 PASS；ARCH-A、变�?AXAML/XML�?+100 �?`git diff --check` PASS�?
- 状态：`TECHNICAL PASS / READY FOR USER IPO VISUAL ACCEPTANCE`；未宣告用户视觉或真机验收通过�?

## AREA A · C+D 第二轮纠偏（2026-09-08 12:45:52 +08:00�?

- 顶部重排：第一行固定品牌、工作区、文件、弹性空区、运行与状态；第二行保留编辑工具、视图、吸附三块。工作区改为同级中文标题与紧凑的“管理模�?/ 地图编辑”操作入口�?
- 可用性真源：新增 `CanUseEditTools` �?`CanToggleSnap`，只派生自既�?Mode/Workspace；管理态工具、区域编辑下的地图变换工具、非地图编辑态吸附均不能通过命令入口改变状态�?
- 交互诚实性：编辑工具与吸附在不可用状态仍保留位置；框选恒为禁用且移除命令接线；聚焦与全览采用不同矢量图标�?
- 契约与回归：新增 `AreaAR2AvailabilityContractTests`，覆盖管理态、工作区切换、吸附和未实装框选的不可变语义；既有变换测试明确进入编辑态，并按地图编辑真实取景使用地图尺寸上界�?
- 验证：解决方案与测试项目构建均为 0 Warning / 0 Error；Core 339/339、WarCore 22/22、World 1402/1402 PASS；ARCH-A guard �?`git diff --check` PASS。XYUI 全量测试命令正常结束且未输出失败�?
- 状态：`TECHNICAL PASS / READY FOR USER IPO VISUAL ACCEPTANCE`；尚未宣告用户视觉或真机验收通过�?

## AREA A · C+D TOP CHROME（IN PROGRESS�?026-09-08 11:36:00 +08:00�?

- 基准与版本硬门禁：以 `v0.2.28.24-rz` 为全项目唯一事实真源，新�?`UiCanonicalVersionContractTests`�?/4 PASS）锁�?`run.bat`、窗口标题、VM 标题�?changelog 四处完全一致，杜绝版本漂移�?
- 前置纠偏：彻底移�?`MapFormPanel` 中的隐藏 `PropsNarrow`，单�?Property Grid 3 个字段各维持 1 处独立错误绑定；更新 `UiD5FormContractTests`�?/1 绑定契约）、`UiD4MapEditorContractTests`、`R2BPropertyEditorVisualContractTests` �?`XYUI2R2BContractTests`，实装真单行 96,* 属性区�?
- C+D 模块卡片化架构：�?Area A 顶层解耦为独立 SRP 模块视图（`WorkspaceSelector`、`FileModule`、`EditToolsModule`、`ViewModule`、`SnapModule`、`RuntimeStatusModule`、`Top.axaml`），每个手写 AXAML 文件�?�?100 行（严格遵守 5+100 架构红线与多行一属�?XAML 规范）�?
- 模块七大中文分区：落�?`工作区`（独立呈现编辑模式与具体工作区）、`文件`（文�?添加原生菜单 + 高频新建/打开/保存 + 撤销/重做）、`编辑工具`（选择、框选、移动、旋转、缩放，框选由 VM 诚实反馈尚未实装）、`视图`（聚焦、全览、平移、环绕、环境菜单）、`吸附`（吸附开关与状态）、`运行`（Primary 运行、Danger 停止）、`状态`（文档状态与当前模式 Badge）�?
- 样式纯化与单手势防抖：彻底清�?`Top.States.axaml` 模板 hack �?`.cmdBtn`/`.toolBtn` 样式，全面接入规�?XYUI 控件；移�?`WorkspaceSelector` 中的 `DoubleTapped` 穿透，锁死为单一点击切换手势�?
- 自动化门禁：完整解决方案 Build `0 Warning / 0 Error`；Core `339/339`、WarCore `22/22`、World `1399/1399`、XYUI `558/558 PASS`；`arch-a-guard.ps1`（含 5+100）与 AXAML/XML、版本一致性、`git diff --check` 均通过�?
- 状态：`READY FOR USER IPO VISUAL ACCEPTANCE`，等待真机验收�?

## AREA A · TOP CHROME（IN PROGRESS�?026-09-08 10:36:02 +08:00�?

- 开工：�?recovered canonical 上重新施工，不导�?Area-A 私人实验提交�?
- 首个布局改造：Top Chrome 用真�?`XYHeading`、`XYCaption`、`XYSeparator` 和既�?`XYBadge` 分离品牌/编辑上下文、菜单与命令组；所有既�?Command、Binding 与工具交互保持原接线�?
- 验收：自动门禁完成后仍需用户�?IPO 做顶部视觉与交互真机验收；当前状态为 `IN PROGRESS`，不�?CLOSED�?

## BASELINE-STABILIZATION · e074e1bb recovered candidate�?026-09-08 10:36:02 +08:00�?

- 恢复：地图属性重新保留宽/窄两种真�?XYUI 输入路径�? 个字段控件均独立绑定 `MapWidthError` / `MapDepthError` / `MapBaseHeightError`，失焦校验不再退化为表单级错误；MAP focused `824/824 PASS`�?
- 单源：删�?35 个历史大�?`XYUI/` 追踪路径，并�?Windows 两阶段实体目录改名收敛为唯一 `xyui/`；Engine ProjectReference 继续指向 `..\\xyui\\avalonia\\src\\XYUI.Avalonia\\XYUI.Avalonia.csproj`�?
- 样式：Engine �?Button/TextBox 规则改为显式 `legacyControl` opt-in；XYUI 子类不匹�?Legacy 全局规则，工具与图层状态改�?XYUI canonical token 决定�?
- 验证：Solution Build `0 Warning / 0 Error`；Core `339/339`；WarCore `22/22`；MAP `824/824`；Engine XYUI/UI focused `355/355`；XYUI 全量 `558/558`（TRX）；ARCH-A、AXAML/XML、DLL/deps/SHA256、一致性及 `git diff --check` 通过�?

## XYUI-ENGINE-A-R2-B-FIXUP-02 · Property Grid single-line recovery�?026-09-07 20:17:07 +08:00�?

- 视觉修复：`MapFormPanel` 移除会在侧栏窄于 360 DIP 时切换的 `PropsNarrow`，地图属性固定为 `96,*` 单行 Property Grid；标签、输入框与只读值垂直居中�?
- 宽度修复：左侧项目面板初始宽度恢复为 `220 DIP`，上限收敛为 `320 DIP`；属性字段不再通过窄表单或最小宽度撑大面板�?
- 语义保持：保�?`XYTextField`、`128 × 24 DIP`、字符串 Binding、LostFocus 校验、错误反馈及 Apply/Undo/Redo Command；VM 未修改�?
- 契约：R2B 运行时契约改为检查三组真实输入、四行同一 Property Grid 与左侧面板宽度边界；不再接受隐藏窄布局冒充通过�?
- 验证：属�?R1/R2-A/R2-B/D4/D5 定向契约 `50/50 PASS`；完整解决方�?Build `0 Warning / 0 Error`；ARCH-A PASS；`git diff --check` PASS�?
- 状态：`R2-B PROPERTY FINAL FIX COMPLETE / READY FOR USER VISUAL ACCEPTANCE`；尚未判定用户真机验收通过�?

## XYUI-ENGINE-A-R2-B-FIXUP · Property Editor compact density�?026-09-07 19:29:57 +08:00�?

- 视觉修复：`MapFormPanel` �?6 �?XYUI 输入字段统一使用现有 Compact Token，固定为 `128 × 24 DIP`、右对齐、Compact Padding；属性行与窄模式间距统一收敛�?`Space.2` / `Space.4`�?
- 操作修复：应用、撤销、重做继续使�?`XYButton` 与原�?Command，统一�?`96 × 24 DIP`、左对齐、Compact Padding，移除整�?Stretch 视觉；未修改 VM、字符串 Binding、LostFocus、校验和 Undo/Redo 语义�?
- 契约：新�?`R2BPropertyEditorVisualContractTests`，同时更�?D4 Map Editor 布局契约以锁定正�?Compact Token；运行时几何断言覆盖 6 字段�?3 按钮�?
- 验证：Property/R1/R2-A/R2-B/D4/D5 定向契约 33/33 PASS；完整解决方�?Build 0 Warning / 0 Error；ARCH-A PASS；`git diff --check` PASS�?
- 状态：`R2-B VISUAL FIX TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE`；尚未判定用户真机验收通过�?

## XYUI-ENGINE-A-R2-B · Final Left SHA reconciliation�?026-09-07 18:57:04 +08:00�?

- 事实核验：远�?`origin/feat/XYUI-ENGINE-A-R2-B-G` 当前�?Gemini `1212f30b1bd6937bab4cf76f6eb917414285a042`；`1212f30b` 不在�?Acceptance `7edf8fdb` 中，�?Left `53efea66` �?`1212f30b` 互不为祖先关系�?
- 收口：按 Gemini 最新提交原样恢�?6 �?Left 文件，生�?reconciliation commit `2924ac83`；Left �?`1212f30b` 内容一致，Right/Contract `fe55ea30` 保留，未手改 Gemini UI�?
- 事实计数：Left 包含 `XYSearchField` 1 个、`XYButton` 11 个、`XYToggleButton` 3 个；Right XYUI-2 实装未被覆盖�?
- 验证：R2-B/R1/R2-A 定向契约 10/10 PASS；完整解决方�?Build 0 Warning / 0 Error；ARCH-A PASS；`git diff --check` PASS�?
- 状态：等待 Acceptance 更新后的唯一 `run.bat` 真机视觉与交互验收，未将自动门禁标记为用户验收通过�?

## XYUI-ENGINE-A-R2-B · Buttons & Inputs visible migration�?026-09-07 18:27:45 +08:00�?

- 合流：以 R2-A 验收基线 `1d6e8ab2` 为共同基线，受控合入 Left `53efea66` �?Right/Contract `fe55ea30`，生成集成提�?`9987e00e`、`4c40cb49`；未修改 Canonical 工作区�?
- 迁移：Left 项目搜索使用真实 `XYSearchField`；地图、区域、道路、标记操作使�?`XYButton`/`XYToggleButton`。Right Inspector、地图、数据集、图层操作使用真�?`XYButton`、`XYIconButton`、`XYTextField`、`XYSelect`、`XYToggleButton`，保留既�?Binding、Command 与事件语义�?
- GAP：MapForm 数值字段暂�?`XYTextField`，因为现�?VM 为字符串 Binding 且依�?`TextBox`/`LostFocus`；本轮不�?VM，不冒险引入 `XYNumberField`�?
- 契约：新�?`XYUI2R2BContractTests`，并纳入 R1/R2-A 回归；定向测�?10/10 PASS。完整解决方�?Build 0 Warning / 0 Error；ARCH-A PASS；`git diff --check` PASS�?
- 状态：`R2-B TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE`；仍须通过唯一入口 `E:\MyDoc\project-VSCode\XuanYuEngine\run.bat` 做真机视觉与交互验收�?

## XYUI-ENGINE-A-R1-VISUAL-FIX · SectionTitle visual authority closeout�?026-09-07 14:39:05 +08:00�?

- Gemini handoff：`194cb923` 已提交并 push �?`feat/XYUI-ENGINE-A-R1-VISUAL-FIX`，修�?Engine Debug 页面，移除四�?`XYSeparator Variant="Section"`，避免通用 Divider 夺取 `XYSectionTitle` Soft Header 的视觉层级�?
- 根因：Canonical `XYSectionTitle` �?Engine Theme Resource 链本身有效；Engine 页面在真实区块标题后重复放置 Section Divider，导�?Accent Bar �?Soft Header 语义在真机上不突出。未发现 Legacy Selector 覆盖 `XYSectionTitle` 的证据�?
- Canonical：冻结标准保持不变：Header `28 DIP`、Left Mark `3 × 16 DIP`、`#526873`、文�?`14/600/18`、背�?`#EEF3F6`、圆�?`3 DIP`；Gemini 临时改动�?Accent 色和额外 Mark 圆角已按 Canonical 证据回退�?
- Contract：新�?`UiR1VisualContractTests`，验�?Engine `EditorRightTabs` 中四个真�?`XYSectionTitle` �?Visual、尺寸、颜色、Typography、文本、无重复 Separator；并覆盖 Heading、Label、Badge、StatusBadge、ErrorText 代表性样式�?
- 验证：Engine Build 0 警告 / 0 错误；XYUI Build 0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1381/1381、XYUI 558/558；Visual Contract 2/2；Gallery Smoke 3/3；ARCH-A（含 5+100）PASS；`git diff --check` PASS�?
- 状态：`R1-VISUAL-FIX TECHNICAL CLOSEOUT PASS / READY FOR USER ACCEPTANCE`�?

## XYUI-ENGINE-A-R1-CLOSEOUT · FINAL-A + FINAL-B Controlled Merge�?026-09-07 13:50:40 +08:00�?

- 合流：以统一母线 `cbbc52b0` 为基线，按顺序使用受�?`--no-ff` merge 合入 FINAL-A `861340aa` �?FINAL-B `ab92fac7`，生�?`47a43188`、`a2869ef7`；无冲突，未 rebase、squash、reset、force push 或改写历史�?
- 范围：Left、Top、Foot、Right 的本�?XYUI-1 Engine View 迁移已统一进入 `feat/XYUI-ENGINE-A`；XYUI-1 Category A 剩余�?0。XYUI-2、XYUI-3、纯布局/渲染宿主和无 Canonical 等价物的图标按迁移矩阵保留，不提前越界�?
- 审计：目�?View 不再使用 `uiSection`、`uiLabel`、`uiValue`、`datasetName`、`datasetStatus`、`datasetLayerName`、`datasetLayerStatus`、`kindTagRegion`、`kindTagSystem`、`treeText`、`statePill` �?Legacy Display 类；`uiMultiline` 仅承担换�?行数限制，归入布局辅助职责�?
- 验证：Engine Build 0 警告 / 0 错误；XYUI Build 0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1379/1379、XYUI 558/558，合�?2298/2298 PASS；Gallery Smoke 3/3 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS�?
- 文档：同步本�?R1 closeout 到迁移矩阵；本轮无结构变化，`file-tree.md` 保持现状�?
- 状态：`TECHNICAL CLOSEOUT PASS / XYUI-1 ENGINE IMPLEMENTATION COMPLETE / READY FOR USER FINAL VISUAL ACCEPTANCE`�?

## XYUI-ENGINE-A-R1-FINAL-A · Left + Top XYUI-1 Final Sweep�?026-09-07 13:29:25 +08:00�?

- 基线：从统一母线 `cbbc52b0` 创建 `feat/XYUI-ENGINE-A-R1-FINAL-A` 独立 worktree；本轮仅施工 Left 5 �?View �?Top 1 �?View�?
- 迁移：项目树与层级树标题使用 `XYTruncatedText`；区�?道路/标记 Display 使用 `XYSectionTitle`、`XYSeparator`、`XYLabel`、`XYText`、`XYSelectableText Technical`、`XYCaption`；Top 状态药丸使用真�?`XYBadge`，工具组分隔使用 `XYSeparator Variant=VerticalSplit`�?
- 保留：TabControl、ListBox/Tree、TextBox、ContextMenu、Menu、Button、ToggleButton、业�?Binding、Command、事件、拖拽和重命名行为均未改动；未修�?VM、Theme、App �?XYUI Runtime�?
- 契约：新�?`UiR1FinalLeftTopContractTests` 5 项断言，锁定真�?XYUI-1 组件并拒绝本轮目�?View �?`treeText/uiSection/uiLabel/uiValue/uiMultiline/groupSeparator/statePill` Legacy Display 类�?
- GAP：本轮无阻塞 Runtime GAP；动态文档状态没有可直接绑定�?`XyuiStatusState` 公共事实源，因此保留原状态颜�?Binding 并使�?`XYBadge`；XYReorderableList、XYUI-2 交互控件和未纳入映射的工具图标留待后续范围�?
- 验证：Engine Build �?XYUI Build �?0 警告 / 0 错误；定向契�?5/5；Core 339/339、WarCore 22/22、World 1379/1379、XYUI 558/558，合�?2298/2298 PASS；Gallery Smoke 3/3 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS�?
- Hash：本轮最终提交由 Git 记录确认�?
- 状态：`R1-FINAL-A TECHNICAL PASS / READY FOR INTEGRATION`�?

## XYUI-ENGINE-A-R1-B2-MERGE · Codex + Gemini Controlled Merge�?026-09-07�?

- 合流：在 `e19e4242` 基线分支 `feat/XYUI-ENGINE-A` 上，按顺序使用受�?`--no-ff` merge 合入 Codex `0f80d905` �?Gemini `5b9add17`；未 rebase、squash、reset、force push 或改写历史�?
- 范围：统一包含 `NotificationBar`、`LogDetailPanel`、`LayerInspectorPanel`、`InspectorPanel`、`MapPagePanel`、`DatasetLayerPanel`、`DatasetPanel` 七个 Engine View �?XYUI-1 迁移成果；两边所有权无重叠，未发现冲突�?
- 祖先关系：`0f80d905` �?`5b9add17` 均已确认是合流提交祖先；�?dirty `feat/MAP-DATA-A` 工作区未触碰�?
- 验证：Engine Build �?XYUI Build �?0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1374/1374、XYUI 558/558，合�?2293/2293 PASS；Gallery Smoke 3/3 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS�?
- 文件树：本轮仅合流既有文件，无新增、删除、改名或移动文件，`file-tree.md` 无需更新�?
- Hash：本轮最终合流提交由 Git 记录确认�?
- 状态：`R1-B2 INTEGRATION PASS / READY FOR USER VISUAL ACCEPTANCE`�?

## XYUI-ENGINE-A-R1-B2-A · Inspector / MapPage XYUI-1 Migration�?026-09-07�?

- 基线：基于集成分支提�?`e19e4242` 创建 `feat/XYUI-ENGINE-A-R1-B2-A`；本轮仅迁移 `InspectorPanel.axaml`、`MapPagePanel.axaml` 及其直接失效契约测试�?
- 迁移：Inspector 使用真实 `XYHeading`、`XYCaption`、`XYIcon`、`XYSectionTitle`、`XYSeparator`、`XYLabel`、`XYText`、`XYSelectableText`、`XYEmptyText`；技术字段按真实语义使用 `Technical` 变体�?
- 迁移：MapPage 的“地图资产”摘要使�?`XYSectionTitle`、`XYSeparator`、`XYLabel`、`XYText`、`XYSelectableText`；MapForm、按钮、输入控件、绑定和交互保持不变�?
- 清理：移除两个目标页面中控制 XYUI-1 视觉语义的旧 `panelTitle`、`panelIcon`、`fieldSeparator`、`groupSeparator`、`uiLabel`、`uiValue` �?Legacy 类；更新直接失效�?Inspector、文本溢出和 MapEditor 契约断言�?
- 验证：Engine Build �?XYUI Build �?0 警告 / 0 错误；定向契�?23/23；Core 339/339、WarCore 22/22、World 1374/1374、XYUI 558/558，合�?2293/2293 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS�?
- Hash：本轮最终提交由 Git 记录确认�?
- 状态：`R1-B2-A TECHNICAL PASS / READY FOR INTEGRATION`�?

## XYUI-ENGINE-A-R1-M1/C · Controlled Integration + Runtime Theme Wiring�?026-09-07�?

- 合流：从 `93893f8a` 依次合入 R1-A `012ba16d` �?R1-B `5c3b3024`；R1-B 使用普�?`--no-ff` 受控 merge，集成提交为 `29d53c70`，未 rebase、squash 或改写历史�?
- Theme 接入：`XuanYu.Editor.UI/Bootstrap/App.axaml.cs` 复用最�?Runtime 公开 API，注�?`XyuiTheme`、`XyuiVectorIcons`、`XyuiTextStyles`、`XyuiShapeStyles`、`XyuiInteractionStyles`、`XyuiControlStyles`、`XyuiComponentStyles`；未新增 `XYUIBootstrap`，未引用 Gallery�?
- Authority：`LayerInspectorPanel.axaml` 删除会覆�?`XYLabel` / `XYText` / `XYSelectableText` 的旧 `uiLabel` / `uiValue` 类；Button/TextBox 业务交互保持不变�?
- Smoke：`XuanYu.World.Tests` UI Runtime 定向 Smoke `80/80 PASS`；两�?R1 分支均已验证为集成提交祖先�?
- 验证：Engine Build �?XYUI Build �?0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1374/1374、XYUI 558/558，合�?2293/2293 PASS；UI Runtime Smoke 80/80 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS�?
- 修正：两项旧 LayerInspector 源码合同测试改为锁定 XYUI 类型存在�?Legacy `uiLabel/uiValue` 不再覆盖 Runtime 视觉；未弱化业务断言�?
- Hash：本轮最终提交由 Git 记录确认�?
- 状态：`R1-M1 MERGED / R1-C TECHNICAL PASS / READY FOR USER VISUAL ACCEPTANCE`�?

## XYUI-ENGINE-A-R1-A · Governance / Migration Matrix Audit�?026-09-07 11:37:39 +08:00�?

- 目标：在统一基线 `93893f8a` 上完�?Engine UI 接入审计、XYUI-1~3 Migration Matrix、Legacy/Duplicate Style Inventory �?Agent-B ownership freeze�?
- 变化：新�?`docs/milestones/current/XYUI-ENGINE-A/XYUI-ENGINE-A-migration-matrix.md`；确�?Editor.UI �?最�?`xyui/avalonia` Runtime 引用有效、Editor/App/Win �?Gallery Runtime 依赖；不修改 Engine View、Runtime、Gallery 或全局 Theme�?
- 盘点�?4 �?Engine UI AXAML；TextBlock 178、Border 66、Button 63、ToggleButton 13、TextBox 13、ListBox 16、TabControl 3、Menu 3、ContextMenu 1、Path 51、PathIcon 8；旧 `XYUI/**` 34 �?tracked 文件记为 Legacy Duplicate�?
- 遗留：`XYUIBootstrap.Create()` 不存在，App 尚未加载 `XyuiTheme` / `XyuiComponentStyles`，作为后续集�?GAP；三�?Agent-B View ownership 已冻结，`InspectorPanel.axaml` 延后 Batch 2�?
- 验证：Engine Build �?XYUI Build �?0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1374/1374、XYUI 558/558，合�?2293/2293 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS�?
- Hash：本轮提交由最�?Git 记录确认�?
- 状态：`R1-A TECHNICAL PASS / READY FOR AGENT-B HANDOFF`�?

## XYUI-ENGINE-A-R1-P0 · Latest Engine + Latest XYUI Integration�?026-09-07 11:25:42 +08:00�?

- 基线：Engine `feat/MAP-DATA-A` committed HEAD `7ae17b4d`；XYUI `origin/feat/XYUI-A` HEAD `3d09dc67`，包�?`5fd54b42` GALLERY-UNIFY-02 及其后续提交�?
- 整合：创�?`feat/XYUI-ENGINE-A`；受控合并无共同祖先历史�?5 �?Engine 根文件按 Engine 侧保留，`xyui/**` 按最�?XYUI 侧保留；�?solution 改指�?`xyui/avalonia`，Editor.UI 增加 XYUI Runtime ProjectReference，未引入 Gallery Runtime 依赖�?
- 验证：`XuanYu.Engine.slnx` �?`xyui/avalonia/XYUI.Avalonia.slnx` �?0 警告 / 0 错误；Core 339/339、WarCore 22/22、World 1374/1374、XYUI 558/558，全量合�?2293/2293 PASS；ARCH-A（含 5+100）PASS；`git diff --check` PASS。D 盘指�?SDK 不存在，使用 E �?SDK 完成实际门禁�?
- 状态：`INTEGRATION PASS`；未创建 tag/release，未触碰两个既有 dirty 工作区�?

## R2-F2 · Control Theme Rendering Reconciliation · 2026-08-13 23:45:00

- 目标：修�?R2 视觉验收失败，确�?XYUI Theme 真正命中控件实例并重�?Gallery 验收布局�?
- 变化：将 Controls 样式选择器改为明确的 XYUI wrapper 类型选择器；补齐 NumberField 继承命中；新增运行时计算属性契约；Gallery 改为固定列宽�?Buttons / Inputs / Selection / Density 对比卡片，避免文本重叠与大面积空白�?
- 验证：代表性控件运行时 Background/Border/Padding/MinHeight 契约 PASS；Visible Smoke PASS；XYUI Tests 4/4 PASS；正式全量门禁待收口�?
- Hash：`fac49e07`�?
- 状态：R2-F2 READY FOR USER VISUAL ACCEPTANCE；R2 �?CLOSED，R3 继续阻塞�?

## R2-F1 · Gallery Visible Runtime Regression · 2026-08-13 23:18:00

- 目标：修�?R2 Controls 引入后的 Desktop 可见窗口回归�?
- 根因：Gallery 未加�?Fluent 原生主题，且原生 ToggleSwitch 模板�?Avalonia 12.0.4 中缺�?`PART_MovingKnobs`，窗口在首次布局时崩溃；进程存活不代表窗口可见�?
- 变化：Gallery 加载 FluentTheme；XYToggleSwitch 改为基于 ToggleButton �?Styled Wrapper；新�?`gallery-visible-smoke.ps1`，检查进程窗口句柄、标题和响应状态�?
- 验证：Visible Smoke PASS（窗口句柄非零、标题正确、响应正常）；XYUI Runtime/Controls 测试 4/4 PASS；FluentTheme �?XYToggleSwitch 模板链路已覆盖�?
- Hash：`c817a444`�?
- 状态：R2-F1 READY FOR USER VISIBLE-WINDOW ACCEPTANCE；R2 �?CLOSED，R3 继续阻塞�?

## R2 · Controls Core · 2026-08-13 22:51:51

- 目标：在 R1 CLOSED 基础上交付第一�?XYUI 控件与可视化状�?Density 对比�?
- 变化：新�?Button/IconButton/ToggleButton、TextField/NumberField、CheckBox/RadioButton/ToggleSwitch、ComboBox/Slider、Badge/Tag；新�?Normal/Hover/Pressed/Focus/Disabled/Selected/Error、Small/Default/Large �?Compact/Default/Comfortable 样式；Gallery 改为 Controls Core 页面�?
- 验证：Gallery 实际进程烟测保持运行；XYUI Tests 4/4 PASS；目标项目构�?0 警告/0 错误；正式全量门禁待收口�?
- Hash：`4c5d1162`�?
- 遗留：等待用户真机视觉验收，不进�?R3�?

## R1-F1 · Gallery Runtime Bootstrap Fix · 2026-08-13 22:43:48

- 目标：修�?Gallery 启动后立即退出，保持 R1 范围，不进入 R2�?
- 变化：修�?XYUITheme �?Avalonia 资源类绑定与重复资源包含；修�?Radius/Thickness Token 类型；新�?Headless Gallery Runtime 契约测试�?
- 验证：`dotnet run --no-build` 进程保持运行；XYUI Runtime Tests 3/3 PASS；待执行正式全量门禁�?
- Hash：`22f246f1`�?
- 遗留：等待用户重新执行相同命令进行窗口与 Foundation 页面真机验收�?

## v0.1.0-alpha · XYUI-AVALONIA-R1 · 2026-08-13 22:27:18

- 目标：启�?XYUI.Avalonia Foundation + Gallery，不进入 R2 组件扩展�?
- 变化：新增可引用�?XYUI.Avalonia、Gallery、Tests 项目；落�?Light/Dark 主题入口、颜�?间距/圆角/控件高度 Token、Density/ThemeVariant 契约�?Foundation Gallery 页面�?
- 验证：Gallery 构建 0 警告/0 错误；XYUI Tests 2/2 PASS；`git diff --check` PASS�?
- Hash：`bc70a0fe`�?
- 遗留：解决方案完整构建仍受既有依赖缓�?还原环境影响；Gallery 真机启动与截图待用户验收�?

## 归档规则

- 每个自然月执行一�?changelog 归档（宪法第五十三条《月度归档》）�?
- 当前自然月记录保留在本文件；已结束月份按自然月归档至 `docs/archive/changelog/changelog-YYYY-MM.md`（单一归档位置，不为每个轮次单独建文件）�?
- 归档内容原则上原样迁移，保留版本、日期、验证与遗留事项；版本历史不丢失，按下方索引可定位�?

## 历史归档索引

| 月份 | 归档文件 | 条目范围 |
|---|---|---|
| 2026-07 | `docs/archive/changelog/changelog-2026-07.md` | v0.2.1.1-rz �?v0.2.23.0-rz |
| 2026-06 | `docs/archive/changelog/changelog-2026-06.md` | v0.1.1.5-rz �?v0.1.8.10-fix |
| 2026-05 | `docs/archive/changelog/changelog-2026-05.md` | v0.1.1.1-rz �?v0.1.1.4-rz |

> 历史审计注记（SHR-2026-08-R2）：7 月归档内存在 3 处同一版本号分配给两个不同轮次的历史缺陷（v0.2.16.2-rz、v0.2.17.8-rz、v0.2.20.19-fix �?2 条，内容不同）；按归档原则保留原文不篡改，追溯时�?Commit Hash 为准。版本号与日期顺序另�?18 处非单调，为历史既成事实，登记不重排�?

---

## v0.2.28.24-rz · MAP-DATA-A-R3 POINT FEATURE FOUNDATION
MAP-DATA-A-R3�?026-08-13 15:58:51）：�?`fe00c4b` 为基线，一轮完�?`marker` Dataset �?Map Marker Point Consumer，接入通用编辑、局部查询、Snap、Undo/Redo、Save/Reload�?
- 变化：新�?Point/MapMarker 领域模型、最�?Dataset codec/binding、单击放置、自动回选择、单控制点拖动与克制 Marker overlay；Marker 支持 Region/Road/Marker Vertex/Segment Snap�?
- 边界：Point 复用既有 Generic Geometry Capability、Map-level History �?Snap Policy；不新增 Point 专属拖动/Snap 系统，不引入城镇、资源、港口、Gameplay、Topology Weld �?Shared Node�?
- 验证：Point/Generic focused 13/13；Solution 0 Warning/0 Error；Core.Tests 339/339；World.Tests 1374/1374；WarCore.Tests 22/22；ARCH-A�?+100、AXAML/XML、版本四处一致与 `git diff --check` PASS�?
- 状态：R3 `IMPLEMENTED · READY FOR USER ACCEPTANCE`；未标记 CLOSED，等�?M01～M08 真机验收�?
- 遗留：真机验收见 `MAP-DATA-A-R3-point-feature-foundation-acceptance.md`�?
- Hash：本条所在提交�?

## v0.2.28.23-rz · MAP-DATA-A-R2 CLOSEOUT
MAP-DATA-A-R2-CLOSEOUT�?026-08-13 15:18:55）：用户确认 F3-E M01～M10 全部 PASS；以功能基线 `6a3d5b8` 为依据完�?R2 Geometry Editing Foundation docs-only 收口�?
- 变化：F3-E Generic Geometry Editing & Snap 正式 `CLOSED`；R2 全部能力汇总并正式 `CLOSED`；旧 F3-F 拆分路线清理�?
- 边界：Topology Weld、Shared Node、自动路口、交点、自动切分、节点增删与 Gameplay 业务移入 Future/Backlog，不作为 R2 遗留缺陷�?
- 知识：新�?R2 Closeout �?Point Foundation 决策；新�?`MAP-DATA-A-R3 · Point Feature Foundation` 计划，Map Marker �?Point 首个 Consumer�?
- 验证：复�?`6a3d5b8` �?Solution 0W/0E、Core 339/339、World 1365/1365、WarCore 22/22、ARCH-A�?+100、AXAML/XML、四处版本一致与 `git diff --check` 证据；本�?docs-only，不重复完整功能测试�?
- 状态：`MAP-DATA-A-R2 CLOSED`；下一开发任务为 `MAP-DATA-A-R3 · Point Feature Foundation`，尚未开始实现�?
- Hash：本条所在提交�?

## v0.2.28.22-rz · MAP-DATA-A-R2-F3-E GENERIC GEOMETRY EDITING & SNAP
MAP-DATA-A-R2-F3-E�?026-08-13 14:54:24）：�?`d0bc0df` 为基线，一次性实现通用几何编辑生命周期、局部候选索引、Vertex/Segment Snap 仲裁，以�?Region/Road 集成�?
- 变化：新�?Point/Polyline/Polygon capability model、Feature Adapter、GeometrySpatialIndex 与通用 Snap Pipeline；Road 支持吸附 Region Vertex/Segment 与其�?Road Vertex/Segment；保�?Map-level History�?
- 边界：不引入 Topology Weld、共享节�?边、交点、自动切分、节点增删或 Schema 变化；PointerMove 只查询局部候选，不扫描全地图�?
- 验证：聚焦回�?24/24；Solution 串行 Build 0 Warning/0 Error；Core.Tests 339/339；World.Tests 1365/1365；WarCore.Tests 22/22；ARCH-A PASS；AXAML/XML PASS；版本四处一致；`git diff --check` PASS�?
- 状态：F3-E `IMPLEMENTED · AUTOMATED FOCUSED PASS · READY FOR USER ACCEPTANCE`；等待一次综合真机验收，未标�?`CLOSED`�?
- 遗留：等�?`MAP-DATA-A-R2-F3-E-acceptance.md` �?M01～M10 真机验收；Topology Weld `NOT STARTED`�?
- Hash：本条所在提交�?

## v0.2.28.21-rz · MAP-DATA-A-R2-F3-E1 GEOMETRY CAPABILITY CONTRACT
MAP-DATA-A-R2-F3-E1�?026-08-13 14:18:25）：基于 `5822bb3` 完成 Geometry Capability Contract、Region/Road 映射、Gap Report �?E2 Decision；本轮不修改生产代码与交互�?
- 变化：定�?Point/Polyline/Polygon �?Selectable/VertexEditable/Snappable/SnapTarget；确�?Geometry Source、Feature/Dataset Identity、Picking、Edit Lifecycle、History、Local Snap Candidate 来源；记�?Road 局部候选源缺口�?E2 边界�?
- 验证：只读调查；ARCH-A PASS；`git diff --check` PASS；版本四处一致；继承 D2 正式门禁 Core 339/339、World 1361/1361、WarCore 22/22、Solution 0W0E 证据；无生产代码变化，未重复运行功能门禁�?
- 状态：F3-E1 `CLOSED`；F3-E2 `NEXT`；E3/E4/E5 `BLOCKED`；F3-F �?Topology Weld `NOT STARTED`�?
- 遗留：下一轮只允许启动 E2 Generic Vertex Edit Lifecycle；必须先保持 Region/Road 真机行为不变，再�?E2 Decision 小步抽取�?
- Hash：本条所在提交�?

## v0.2.28.20-rz · MAP-DATA-A-R2-F3-D2 USER ACCEPTANCE CLOSEOUT
MAP-DATA-A-R2-F3-D2�?026-08-13 14:09:46）：用户已明�?D2 真机验收通过；本轮仅收口 D2 并重规划后续通用几何能力，不修改功能代码�?
- 变化：F3-D2 正式标记 `CLOSED`；旧 Road Snap 路线取消；新�?`F3-E Generic Geometry Editing & Snap` �?E1～E5 拆分；新增通用几何编辑契约 Knowledge�?
- 验证：沿�?D2 实现提交 `5a07aba` �?Core 339/339、World 1361/1361、WarCore 22/22、Solution 0W0E、ARCH-A、版本四处与 diff-check 证据；本轮为 docs-only�?
- 状态：F3-D1、D1-F1、D2 `CLOSED`；F3-E `NEXT`；F3-F、Road Snap 独立路线�?Topology Weld `NOT STARTED`�?
- 遗留：下一轮只启动 E1 Capability Contract 调查与契约映射，不改交互；后续按 E2～E5 小步推进�?
- Hash：本条所在提交�?

## v0.2.28.19-rz · MAP-DATA-A-R2-F3-D2 ROAD VERTEX DRAG
MAP-DATA-A-R2-F3-D2�?026-08-13 13:36:10）：�?D1/D1-F1 收口基线 `f2c8ed3` 为基础，实�?Road 已有顶点自由拖动；本轮不实现任何 Road Snap、节点增删或拓扑能力�?
- 变化：Road Select 模式接入既有 `MapGeometryDrag` 管线；支持起�?中间�?终点 PointerDown、Preview、PointerReleased 提交、Esc 取消、Undo/Redo；Preview 不写 Dataset，释放后单次提交一�?History；保持开�?Polyline、顶点顺序及 Road/Layer 身份�?
- 验证：D2 专项 5/5；Core.Tests 339/339；World.Tests 1361/1361；WarCore.Tests 22/22；Solution 串行 Build 0 Warning/0 Error；ARCH-A PASS；`git diff --check` PASS；版本四处一致�?
- 状态：D2 `IMPLEMENTED · AUTOMATED GATES PASS · READY FOR USER ACCEPTANCE`；D3、Road Snap、Topology Weld `NOT STARTED`�?
- 遗留：等待用户按 `MAP-DATA-A-R2-F3-D2-acceptance.md` 执行起点/中间�?终点、Esc、Undo/Redo、连续编辑、锁�?隐藏、Save/Reload 真机验收；验收通过后才�?CLOSED�?
- Hash：本条所在提交�?

## v0.2.28.18-rz · MAP-DATA-A-R2-F3-D1 DOCS-ONLY CLOSEOUT
MAP-DATA-A-R2-F3-D1-F1�?026-08-13 13:25:43）：用户已完�?D1-F1 真机复验并明确通过；本轮仅收口 D1/D1-F1 文档，不修改功能代码�?
- 变化：记录实现基�?`4329376` 的用户真机复�?PASS；F3-D1-F1 �?F3-D1 正式标记 `CLOSED`；F3-D2 改为 `NEXT`�?
- 验证：文档一致性与 `git diff --check`；未运行功能门禁，因为本轮不修改代码�?
- 状态：F3-D1-F1 `CLOSED`；F3-D1 `CLOSED`；F3-D2 `NEXT`；Road Snap �?Topology Weld `NOT STARTED`�?
- 遗留：下一轮以本收口提交为基线启动 `MAP-DATA-A-R2-F3-D2 · Road Vertex Drag`；严格限制为已有 Road 顶点自由拖动、Preview/Release/Dataset Commit、Esc、Undo/Redo�?
- Hash：本条所在提交�?

## v0.2.28.17-rz · MAP-DATA-A-R2-F3-D1-F1 ROAD DRAW SELECT STATE FIX
MAP-DATA-A-R2-F3-D1-F1�?026-08-13 12:13:40）：D1 真机验收发现完成道路后仍可进入新 Road 绘制；本轮仅修复 Road Draw �?Select 状态切换，不启�?D2�?
- 变化：完�?Road 后清�?Draft、显式切回“选择”、自动选中�?Road 并显示全部顶点；Select + Road Authoring �?PointerDown 只执�?Picking/清选，不创建新 Draft；第二条 Road 必须再次点击“绘制道路”�?
- 验证：F1 输入状态专�?6/6；Core.Tests 339/339；World.Tests 1356/1356；WarCore.Tests 22/22；Solution 串行 Build 0 Warning/0 Error；ARCH-A PASS�?3 �?AXAML XML PASS；版本四处一致；`git diff --check` PASS�?
- 状态：D1 保持 `OPEN`；F1 �?`READY FOR USER RE-ACCEPTANCE`；D2 `BLOCKED`；Road Snap `NOT STARTED`�?
- 遗留：只需复验 Road 完成后退出绘制、自动选中、Select 空地不创建、再次显式绘制第二条 Road；验收模板见 `docs/milestones/current/MAP-DATA-A/MAP-DATA-A-R2-F3-D1-F1-acceptance.md`�?
- Hash：本条所在提交�?

## v0.2.28.16-rz · MAP-DATA-A-R2-F3-D1 ROAD VERTEX SELECTION
MAP-DATA-A-R2-F3-D1�?026-08-13 11:28:59）：�?F3-C Region Snap CLOSED 后完�?Road Vertex Selection 最小实现；本轮不启�?Road 拖动、Road Snap �?Topology Weld�?
- 变化：Road 复用当前 Dataset-backed Polyline；可选中 Road 并投影全�?Dataset 顶点，点击顶点记录正确索引；切换 Road/Region 模式清理旧选择；隐藏或锁定 Road 不能进入选择/编辑路径；未新增 Road 数据源或保存合同�?
- 验证：D1 专项 8/8；Core.Tests 339/339；World.Tests 1350/1350；WarCore.Tests 22/22；Solution 串行 Build 0 Warning/0 Error；ARCH-A PASS�?3 �?AXAML XML PASS；版本四处一致；`git diff --check` PASS�?
- 状态：F3-D1 `READY FOR USER ACCEPTANCE`；D2 Road Vertex Drag、D3 Road Snap 均为 `NOT STARTED`�?
- 遗留：等待真机验收；未验收前不得启动 D2、Road Snap �?Topology Weld。验收模板见 `docs/milestones/current/MAP-DATA-A/MAP-DATA-A-R2-F3-D1-acceptance.md`�?
- Hash：本条所在提交�?

## v0.2.28.15-rz · MAP-DATA-A-R2-F3-C REGION SNAP CLOSEOUT
MAP-DATA-A-R2-F3-C�?026-08-13 11:07:40）：用户已完�?Region Snap 真机验收，本轮仅完成 F3-C 文档收口并冻�?F3-D Road Vertex Editing 入口；未修改功能代码�?
- 变化：F3-C3 真机验收记录�?PASS，F3-B Vertex Snap、F3-C1 Edge Snap Geometry、F3-C2 Drag Pipeline �?Region Snap 总线正式 `CLOSED`；F3-D 标记�?`NEXT`，Road Snap �?Topology Weld 继续未启动�?
- 验证：基�?`36ad9e5` �?F3-C2 正式门禁证据保持有效；本轮为文档与版本标识同步，未改变运行时代码�?
- 状态：F3-C `CLOSED`；下一轮仅启动 `MAP-DATA-A-R2-F3-D1 · Road Vertex Selection`�?
- 遗留：D1 不包�?Road 拖动、Road Snap、拓扑焊接或新的 Road 数据源；验收记录�?`docs/milestones/current/MAP-DATA-A/MAP-DATA-A-R2-F3-C3-acceptance.md`�?
- Hash：本条所在提交�?

## v0.2.28.14-rz · MAP-DATA-A-R2-F3-C2 SNAP DRAG PIPELINE
MAP-DATA-A-R2-F3-C2�?026-08-13 10:41:20）：�?F3-C1 CLOSED 后完�?Region Vertex-to-Edge Snap 拖拽管线接入；本轮不新增几何算法、不启动 F3-C3 真机验收、Road Snap �?Topology Weld�?
- 变化：PointerMove 通过 F3-A 12px 局�?Region 查询，沿 Vertex > Edge > Free 仲裁；Edge 锁定 Segment 而非固定 Projection Point�?2px 内沿边重投影，Vertex 8px 内可�?Edge 升级；Preview、Release、Dataset Commit、Esc、Undo/Redo 继续复用 F2 既有路径�?
- 验证：C2 专项 15/15；Core.Tests 339/339；World.Tests 1342/1342；WarCore.Tests 22/22；Solution 串行 Build 0 Warning/0 Error；ARCH-A PASS�?3 �?AXAML XML PASS；版本四处一致；`git diff --check` PASS�?
- 状态：F3-C2 `CLOSED`（自动门禁）；F3-C3 �?`NEXT`，只负责综合门禁与真机体验验收准备，不偷渡功能开发；R2 保持 OPEN�?
- 遗留：C3 真机验收前不得启�?Road Snap �?Topology Weld�?
- Hash：本条所在提交�?

## v0.2.28.13-rz · MAP-DATA-A-R2-F3-C1 EDGE SNAP GEOMETRY
MAP-DATA-A-R2-F3-C1�?026-08-13）：�?F3-B CLOSED 后完�?Region Vertex-to-Edge Snap 纯几何阶段并通过正式门禁；本轮不�?UI、不接拖拽管线、不启动 C2/C3�?
- 变化：新增屏幕线段最近点、Segment Interior、Vertex 优先、Source Region 排除、稳定决胜和零长度边安全处理；继续复�?8 px 进入 / 12 px 释放合同�?
- 验证：C1 专项 10/10；Core.Tests 339/339；World.Tests 1325/1325；WarCore.Tests 22/22；Solution 串行 Build 0 Warning/0 Error；ARCH-A PASS�?3 �?AXAML XML PASS；版本四处一致；`git diff --check` PASS�?
- 状态：F3-C1 `CLOSED`（自动门禁）；F3-C 保持 OPEN，C2 Drag Pipeline Integration �?C3 Formal Gate + User Acceptance 均为 NOT STARTED�?
- 遗留：下一阶段才评�?C2 F2 Preview �?Commit 接线；Edge �?Edge、Road Snap、Shared Topology �?Topology Weld 不在本轮�?
- Hash：本条所在提交�?

## v0.2.28.12-rz · MAP-DATA-A-R2-F3-B FORMAL GATE + USER ACCEPTANCE
MAP-DATA-A-R2-F3-B�?026-08-13）：F3-B Region Vertex-to-Vertex Snap 完成最终正式门禁并通过用户真机验收，状态正�?`CLOSED`�?
- 验证：Solution 串行 Build 0 Warning/0 Error；Core.Tests 339/339；World.Tests 1315/1315；WarCore.Tests 22/22；F3-B 专项 17/17；ARCH-A PASS�?3 �?AXAML XML PASS；版本四处一致；`git diff --check` PASS�?
- 结果：本地与远端最终验证对象为 `b987abe`；无临时验证脚本，受保护�?`_tmp_blind_rows/` 未修改；未启�?Edge Snap、Road Snap �?Topology Weld�?
- 真机：M01～M10 全部 PASS；黄�?Region 顶点已成功与蓝色 Region 顶点对齐，吸附手感与稳定性符合预期�?
- 遗留：下一阶段�?`MAP-DATA-A-R2-F3-C · Region Vertex-to-Edge Snap`；Road Snap �?Topology Weld 暂不启动，R2 保持 OPEN；验收记录见 `docs/milestones/current/MAP-DATA-A/MAP-DATA-A-R2-F3-B-acceptance.md`�?
- Hash：本条所在提交�?

## v0.2.28.11-fix · R2-F1 RUN.BAT BUILD DISCIPLINE
R2-F1�?026-08-13）：真机发现 run.bat 并行构建�?16GB 机器上偶�?OutOfMemoryException；A/B 诊断收口（C# 13 / C# 14 隔离构建�?run.bat 同条件全链冷构建�?PASS，非确定性编译器缺陷，仓库无�?switch 不触�?Roslyn #84529）。用户裁决把验证有效的串行构建方式固化进日常入口，统一 run.bat 与正式门禁的构建纪律�?
- 变化：`run.bat` build 调用�?`-m:1 -nr:false`（串�?+ �?MSBuild 节点复用），与既�?`MSBUILDDISABLENODEREUSE=1`、`-p:UseSharedCompilation=false`、构建前 `dotnet build-server shutdown` 组成完整串行纪律；版本同�?v0.2.28.11-fix�?
- 范围：仅 run.bat 构建调用；未�?csproj / SDK / LangVersion / 业务代码 / 其他脚本；file-tree.md 无新增删除移动与职责变化，未改动�?
- 验证：run.bat fresh 实跑（restore �?串行构建 0W0E �?编辑器进程启动成功）；正式门�?Solution 全量强制重编�?0 Warning/0 Error�?8s）；Core.Tests 339/339、World.Tests 1315/1315、WarCore.Tests 22/22；ARCH-A（依赖边�?+ 5+100 + 版本四处一致）PASS；`git diff --check` PASS�?
- Hash：本条所在提交�?
- 遗留：真机启动验收由用户执行；若 OOM 复发按诊断报告三件套（空闲内�?/ 进程内存排序 / MSBuild 常驻节点）抓现场�?

## v0.2.28.10-rz · MAP-DATA-A-R2-F3-B REGION VERTEX-TO-VERTEX SNAP
MAP-DATA-A-R2-F3-B�?026-08-13）：�?F2 Region 顶点拖拽接入 F3-A 局部查询，进入吸附交互实现阶段�?
- 范围：屏幕空�?8px 进入�?2px 释放；纯坐标对齐；不�?F3-A 索引、不新增历史/持久�?拓扑语义�?
- 状态：`IMPLEMENTED · PRE-GATE PASS`；专项门禁当�?16/16（快速编译模式），完整正式门禁尚未执行�?
- 遗留：fresh 完整门禁与真机吸附手感验收待完成；Edge Snap、Road Snap 与拓扑能力不在本轮�?

## v0.2.28.9-rz · MAP-DATA-A-R2-F3-A REGION LOCAL SPATIAL QUERY
MAP-DATA-A-R2-F3-A�?026-08-13 01:07:12）：建立 Region 局部候选查询基础设施；本轮没�?UI 入口，不实现 Snap�?
- 变化：新增经有限�?顺序校验�?Region Bounds、平衡动�?AABB 树与 `MapEditSession.QueryLocalRegions`；每�?Region 单叶归属，内部节点只保存联合 Bounds 与高度�?
- 生命周期：初始化、新�?替换、Runtime Projection、Create/Delete/Edit、Undo/Redo 均在状态事件发布前同步派生索引�?
- 边界：仅保存 `MapRegionId + AABB`；不生成 `EntityId`，不新增持久化字段，不回退全地图扫描，不修�?Snap �?UI�?
- 验证：F3-A 针对性测�?12/12；Solution 0 Warning/0 Error；Core.Tests 339/339、World.Tests 1298/1298、WarCore.Tests 22/22；ARCH-A�?+100、版本四处一致与 `git diff --check` PASS�?
- 验收：无 UI 入口，不设真�?IPO；纯内部基础设施由专项测试、完整正式门禁与代码审查自动验收，F3-A �?`CLOSED`�?
- Hash：本条所在提交�?
- 遗留：Vertex/Edge Snap、精确几何与 PointerMove UI 反馈未实现，R2 保持 OPEN�?

## v0.2.28.8-rz · DEV-FIRST MULTI-AGENT CHANNELS
DEV-FIRST�?026-08-13 00:33:13）：正式开发使用有 upstream 的里程碑分支�?Commit + Push；UI 实验默认进入�?upstream、禁�?Push �?`local/<任务>` 独立 worktree�?
- 优先级：正式功能、架�?数据、测试与共享元数据优先于 XYUI 本地实现�?UI 实验稿；UI 后续基于最新远端正�?HEAD 适配�?
- 安全性：正式开发显式暂存并复核 cached diff；禁止把 UI 本地 Commit 放在正式分支后被后续 Push 间接发布�?
- 架构澄清：GlobalWorld 实体索引继续保持唯一权威；未注册 World Entity �?Map Authoring 几何可使用仅含原�?Map ID + Bounds �?Editor 派生索引�?
- 验证：文档引用、版本四处一致、ARCH-A �?`git diff --cached --check` 以本提交前实际结果为准；未修改产品代码�?
- Hash：本条所在治理提交�?
- 遗留：既有远�?`feat/XYUI-A` 不在本轮删除；本�?XYUI worktree 按新规则改为 local-only�?

## v0.2.28.7-rz · MAP-DATA-A-R2-F2 CLOSED
MAP-DATA-A-R2-F2 Geometry Vertex Editing Closeout�?026-08-12 23:22:05）：用户完成 C01～C07 真机验收并全�?PASS；本轮仅同步关闭结论，不修改功能实现�?
- 状态：`MAP-DATA-A-R2-F2`、`MAP-DATA-A-R2-F2-F2` �?`MAP-DATA-A-R2-F2-F2-F1` 均为 `CLOSED`；正式解�?F3 Snap 冻结�?
- 验收：F2 已验�?Region/Road 选择、控制柄、顶点拖动、Esc/非法几何拒绝、Undo/Redo �?Save/Reload；删除确认链 C01～C07 �?PASS�?
- 验证：本次为真机结论与文档收口；功能自动门禁沿用各实现提交的已记录通过结果，提交后执行 `git diff --check`�?
- Hash：本收口提交（`HEAD`）�?
- 遗留：仅启动 `MAP-DATA-A-R2-F3 · Geometry Snapping` �?Spatial Index 可复用性调查；不得在未确认局部查询接口前实现或以全地图扫描替代�?

## v0.2.28.6-fix · MAP-DATA-A-R2-F2-F2-F1 DATASET-BACKED DELETE ROUTING
MAP-DATA-A-R2-F2-F2-F1�?026-08-12 22:00:00）：修复 Dataset-backed 图层“删除”绕�?Owned Window、回落主窗口 Overlay/DialogCard 的路由漏分支�?
- 根因：P1 Runtime Probe 记录 `REQUEST_RECEIVED name=解除注册数据集`；同一视觉按钮�?Dataset-backed 图层执行的是解除注册语义，未进入普通删除的独立确认窗�?
- 变化：普通删除与解除注册统一复用 Owned Confirmation Window，保留“删除图层”与“移除区域数据集”的领域文案；两条路径确认前捕获 LayerId，确认后�?ID 重解析�?
- 真机：用户已确认 Dataset-backed 解除注册确认窗可见；完整 F1 仍按更新后的 M01-A/M01-B～M08 清单验收，未 CLOSED�?
- 验证：Solution 0 Warning/0 Error；Core 339/339、World 1286/1286、WarCore 22/22、聚�?7/7；ARCH-A�?+100、AXAML XML �?`git diff --check` PASS�?
- Hash：`3d53de0`（功能收口）�?
- 知识沉淀：INC-2026-08-12-001、L-VAL-001、K-DATA-003，增�?K-NATIVE-001 �?K-VAL-002�?

## v0.2.28.5-fix · MAP-DATA-A-R2-F2-F2-F1 VISIBLE DELETE DIALOG
MAP-DATA-A-R2-F2-F2-F1 Visible Delete Dialog�?026-08-12 21:08:29）：将删除图层确认从主窗口内 Overlay/DialogCard 改为�?Editor 主窗口拥有的独立 Avalonia Window�?
- 根因：Dialog Active �?Escape 取消均正常，DialogCard �?Bounds 位于 `VulkanNativeHost : NativeControlHost` 覆盖范围；Native HWND airspace 压住 Avalonia Visual，故�?UI 被遮罩锁定而确认卡不可见、视口仍可输入�?
- 变化：删除确认以 `ShowDialog(owner)` 展示，默认焦点为“取消”；Esc、Enter、关�?X 均取消，只有鼠标“删除”才确认。删除前保存 LayerId，确认后按该稳定 ID 重新验证并执行，避免等待期间选择变化误删�?
- 边界：经用户批准，业务实现为不可再拆�?6 文件最小闭环；不修�?Vulkan、Swapchain、Picking、Schema、Undo/Redo 总架构或通用 Dialog 系统。禁止回退�?Overlay + ZIndex 覆盖 NativeHost�?
- 验证：最终完整门禁结果见本轮文档提交；实现阶段聚焦测�?6/6、World.Tests 1285/1285 PASS�?
- Hash：`154a62f`（实现提交）�?
- 遗留：F1-M01～M08 真机验收待用户执行；状态为 READY FOR USER ACCEPTANCE，原 M10 �?F3 保持冻结�?

## v0.2.28.4-fix · MAP-DATA-A-R2-F2-F2 LAYER DELETE UI LOCK RECOVERY
MAP-DATA-A-R2-F2-F2 Layer Delete UI Lock Recovery�?026-08-12 18:00:00）：承接用户确认�?M01～M09 PASS �?M10 BLOCKED，修复删除图层后窗口内确认遮罩无法通过 Esc/Enter 完成的问题�?
- 根因：`Window_KeyDown` Tunnel 先于 `DialogCard_KeyDown`，Escape/Enter 被普通编辑快捷键消费，`CompleteDialog()` 未执行；窗口内遮罩持续存在，而原�?Vulkan 子窗口仍可接收视口输入�?
- 变化：活�?Dialog �?Window Tunnel 阶段优先处理 Tab/Escape/Enter；完�?Dialog 时先清空 TCS，避免旧任务/重复确认残留；删除取消、确认、拒绝及后续操作回归已覆盖�?
- 范围：不修改 Vulkan、Swapchain、Fence、Picking、Schema、Save/Load �?Layer 业务规则�?
- 验证：World.Tests 聚焦删除/弹窗回归 23/23；正式完整门禁结果以本条后续 Hash 记录为准�?
- Hash：`bee32fe`（实现提交）�?
- 遗留：F2-F2-M01～M06 真机复验待用户执行；�?M10 保持 BLOCKED；F2/F3 不得 CLOSED/启动�?

## v0.2.28.3-fix · MAP-DATA-A-R2-F2 REGION POINTER SAFETY
MAP-DATA-A-R2-F2 Region Pointer Safety�?026-08-12 17:21:40）：根据用户真机回归发现�?Region Tool 闪退与输入抢占，建立极窄修复轮�?
- 变化：Region Tool 在空 Draft 或零 Anchor �?PointerMove 显式 NO-OP；已有顶�?PointerDown/Drag 优先�?Region Preview；取消和模式往返清理保持安全�?
- 根因：`RegionDrawingPointerMoved()` �?`Draft != null` �?`Vertices.Length == 0` 时读�?`Vertices[0]`；Native/Avalonia 绘制入口先于已有顶点交互入口�?
- 范围：仅修改 5 个业务输�?区域交互文件与对应测试；不修�?Schema、Save/Load、Layer、Vulkan、相机或 Picking 数学�?
- 验证：Editor.UI 快速构�?0 Warning/0 Error；F2 聚焦测试 15/15；正式完整门禁结果以本条后续 Hash 记录为准�?
- Hash：`25fe5f0`（实现提交）�?
- 遗留：F2-M01～F2-M10 真机验收待用户执行；F1 保持 USER ACCEPTANCE FAILED，R2 不得 CLOSED�?

## v0.2.28.2-fix · MAP-DATA-A-R2-F2 GEOMETRY VERTEX EDITING
MAP-DATA-A-R2-F2 Geometry Vertex Editing�?026-08-12 16:00:56）：承接用户确认�?F1 真机通过，进入已完成 Region/Road 几何顶点编辑�?
- 变化：点击已完成区域面或道路显示顶点控制柄；顶点拖动采用 Preview �?Commit，释放提交一�?Map History，Esc 取消；区�?道路统一接入现有 MapSession、Render Overlay、Save/Reload �?Ctrl+Z/Y�?
- 校验：区域候选继续执行多边形合法性校验；道路拒绝相邻重复节点；新增领域单历史、Undo/Redo、非法几何与屏幕空间命中自动测试�?
- 范围：不做吸附、磁性贴合、共享边界、拓扑联动、Schema 变化、Vulkan 重写�?Picking 全面重构�?
- 验证：Solution Build 0 Warning/0 Error；Core.Tests 339/339、World.Tests 1274/1274、WarCore.Tests 22/22；ARCH-A、AXAML XML�?+100、版本一致性与 `git diff --check` PASS�?
- Hash：`66bace3`（实现提交）�?
- 遗留：F2-M01～F2-M06 真机验收待用户执行；F3 禁止启动�?

## v0.2.28.1-fix · MAP-DATA-A-R2-F1 REGIONAL AUTHORING HIERARCHY
MAP-DATA-A-R2-F1 Regional Authoring Hierarchy�?026-08-12 15:12:17）：撤回 RoadEditor 顶层 Workspace，将 Road 收口�?RegionEditor 内的 RegionAuthoringMode.Road，保留既�?Region/Road Dataset 与绘制闭环�?
- 变化：Workspace 仅保�?MapEditor/RegionEditor；新�?RegionalAuthoringPanel、区域面/道路子模式选择、Dataset/Layer 选择同步、统一 Region/Road Layer Stack；模式切换取消活�?Draft 并回到“选择”，Eye/Lock 不切换模式�?
- 兼容：Dataset/Manifest/Feature JSON、Dataset 0.3.0 Road、Region 0.2.0、MapRoad、MapRegion、Vulkan Renderer �?Save/Reload 合同不变；未发现 RoadEditor 持久化入口�?
- 验证：Solution Build 0 Warning/0 Error；Core.Tests 339/339、World.Tests 1270/1270、WarCore.Tests 22/22；专�?RegionAuthoringHierarchy 5/5；AXAML XML�?+100、版本一致性、ARCH-A �?`git diff --check` PASS�?
- Hash：`e4409db`�?
- 遗留：后续真机回归发�?Region Pointer Safety 问题；F1 改为 USER ACCEPTANCE FAILED，转�?v0.2.28.3-fix 修复轮，F3 仍禁止启动�?

## v0.2.28.0-rz · MAP-DATA-A-R2 IMPLEMENTED
MAP-DATA-A-R2 Road Dataset / Polyline�?026-08-12 15:10:00）：�?R1 Closeout 后完�?T2 Road Dataset + Polyline 数据合同�?T3 道路 Authoring �?Render �?Save/Reload 闭环�?
- 变化：Dataset `0.3.0` 支持 Road/Polyline；保�?`0.1.0` �?`0.2.0` Region 读取兼容；新增稳�?Road ID、节点约束、Road 工作区、自�?Bootstrap、草稿节点撤销/重做、正式道�?Map History、可�?锁定/顺序投影和保存重载�?
- 验证：解决方案构�?0 Warning/0 Error；Core.Tests 339/339、World.Tests 1265/1265、WarCore.Tests 22/22；ARCH-A guard�?+100、版本四处一致、AXAML XML �?`git diff --check` 均通过。实现基�?Hash：`bf7ba6f`；R2 真机验收使用 `MAP-DATA-A-R2-acceptance.md`，未验收前保�?READY FOR USER ACCEPTANCE�?
- 范围：不包含 Road Graph、寻路、宽�?坡度、Feature Picking、已完成道路顶点编辑�?XYUI 全面改造�?

## v0.2.27.3-fix · MAP-DATA-A-R1 CLOSED
MAP-DATA-A-R1 Closeout�?026-08-12 14:03:30）：用户最终裁�?R1 真机验收整体 PASS，F1/F2/F3 全部 PASS；本轮仅同步验收状态、关闭记录与 XYUI Backlog，不修改生产代码�?
- 状态：`MAP-DATA-A-R1 CLOSED`；下一阶段正式进入 `MAP-DATA-A-R2 · Road Dataset / Polyline`�?
- 已知 UI 债务：RegionPanel“已有区域”Binding 文本显示异常，登记至 XYUI/UI Backlog，不阻塞 DATA-A，不创建 F4�?
- 验证：沿�?R1 功能基线 `82f05a46552e99a537126cd6c616a1d098bff835` 的完整自动门禁；本轮变更仅为文档/状态同步�?
- 遗留：R2 冻结�?T1 R1 Closeout、T2 Road Dataset + Polyline 数据合同、T3 Road Authoring �?Render �?Save/Reload 完整闭环�?

## v0.2.27.3-fix
MAP-DATA-A-R1-F3 Region Authoring UX Consolidation�?026-08-12 12:21:37）：将区域专属工具、草稿状态和草稿历史收回 Region Workspace，并接�?Dataset-backed Layer 改名与安全移除，R1 保持 OPEN�?
- 变化：左侧新�?Region 工具架，提供当前 Dataset、绘制区域、草稿顶点状态、撤销/重做顶点、完�?取消绘制和区域数量；顶部移除区域专属绘制入口。Draft Undo/Redo �?Ctrl+Z/Y 按“活�?Draft 优先、无 Draft �?Map History”路由，完成区域仍只产生一个正�?Map History Entry�?
- 变化：Region Dataset Layer 双击名称进入 inline rename，名称经 `RenameDatasetAsync` 同步 Manifest、Runtime Layer、Dataset 面板�?Inspector；删除按钮对 Dataset-backed Region Layer 改为确认后解除注册，Dataset 文件保留�?
- 测试：Draft 历史、快捷键层级、Region 工具架、Layer 改名/移除与既有回归；World.Tests `1262/1262 PASS`，构�?`0 Warning / 0 Error`�?
- Hash：`82f05a46552e99a537126cd6c616a1d098bff835`（功能与文档基线）�?
- 范围：未做已完成 Region 顶点编辑、Picking、布尔运算、多选、Road �?Feature Schema/Renderer 扩展�?
- 遗留：F3-M01～F3-M06 真机验收待用户执行；R1 未全量验收不�?CLOSED，不得启�?R2�?

## v0.2.27.2-fix
MAP-DATA-A-R1-F2 Polygon & Auto Bootstrap�?026-08-12 11:21:18）：修复 Region 多边形相交判定，并把“绘制区域”正式入口接�?Region Dataset 自动 Bootstrap，R1 保持 OPEN，等�?F2 真机验收�?
- 变化：`MapRegionIntersection` 改为严格异号判定；区域工具栏改为异步 Click 入口；新�?`BeginRegionDrawingAsync`、`CanRequestRegionDrawing`、Region Dataset 自动创建/选择/活动图层投影；锁定、无效和并发重复请求 fail-closed；区域左栏显示当前绘制目标�?
- 测试：新增不规则四边形、五边形、简单凹多边形、真实四点闭合、自动创建、双击防重复、锁�?无效拒绝�?Save/Reload 回归；World.Tests `1259/1259 PASS`，构�?`0 Warning / 0 Error`�?
- Hash：`9207a2f7b77409932d9ad1a2a51ba1baf03cb8d7`�?
- 范围：未扩展 Region Feature Schema、Hydration 之外�?Dataset 类型、Renderer、R2 Road 或新�?Workspace/Layer 所有权�?
- 遗留：F2-M01～F2-M06 真机验收待用户执行；R1 未全量验收不�?CLOSED，不得启�?R2�?

## v0.2.27.1-fix
MAP-DATA-A-R1-F1 Region Drawing Tool Activation�?026-08-12 10:20:26）：修复 Region Drawing 没有真实 UI 激活入口的问题，R1 保持 OPEN，等�?F1 真机验收�?
- 变化：区域编辑工具栏新增“绘制区域�?ToggleButton，复�?`RegionIcon`；新�?`CanStartRegionDrawing`，仅正常、未锁定的区域数据集且处于区域编辑时可用；`SelectTool` 对非�?RegionDrawing 请求 fail-closed�?
- 测试：补�?Top.axaml UI 合同、Headless 真实按钮路径、模�?Workspace/锁定/非区�?无效 Dataset 拒绝�?Draft 首点测试；针对�?F1 回归 `46/46 PASS`，World.Tests 构建 `0 Warning / 0 Error`�?
- Hash：`9fd6b3a6d51e44ac134c6111cc1b0056ecb77284`�?
- 范围：未修改 Region Feature Schema、Dataset 0.2.0、Hydration、Save Transaction、Renderer、MapRegion、LayerId Projection �?R2 Road 能力�?
- 遗留：等�?`MAP-DATA-A-R1-F1-acceptance.md` �?F1-M01～F1-M03 真机验收；通过后从�?R1-M02 继续，R1 全量通过前不�?CLOSED，不得启�?R2�?

## v0.2.27.0-rz
MAP-DATA-A-R1 Region Dataset Binding�?026-08-11 23:47:31）：完成 Region Dataset �?`map.json` 到运行时绘制、保存与重载的闭环，进入 `READY FOR USER ACCEPTANCE`�?
- 变化：Region Dataset 严格支持 0.1.0 空文件兼容与 0.2.0 Feature；DatasetId 确定性投�?Runtime LayerId；选择、锁定、解除注册取消草稿；Rename/Visible/Lock/Drag 只更�?Runtime Projection；父 Layer Visible �?Dataset Order 控制 Region Overlay�?
- 保存：运行时 Region �?LayerId 分桶写回对应 `region-*.json`；多 Dataset 临时写入后组提交，提交失败恢复已替换文件；首次向 0.1.0 �?Region 时升级为 0.2.0�?
- 验证：Solution Build `0 Warning / 0 Error`；Core `339/339`、World `1244/1244`、WarCore `22/22`；ARCH-A�?+100、`git diff --check` PASS。新�?Hydration、隔离、草稿安全、Undo/Redo、Save/Reload 与组提交失败回滚回归�?
- 遗留：R1 真机 IPO 验收待用户执行；未扩�?Road、Settlement、Resource、River、TerrainArea、Feature 编辑器、Relation �?Runtime 大阶段�?

## MAP-DOC-A CLOSED
MAP-DOC-A-R3 Closeout�?026-08-11 23:16:15）：用户真机验收裁决 R3-F4 PASS，F4-M01～F4-M03 全部通过；MAP-DOC-A 完成并关闭�?
- 交付：Dataset Registry、工作存储、Name/Selection、Dataset-backed Layer Projection、Visible/Lock/Drag、Inspector �?28/32 DIP UI Spec 收口�?
- 验证：基�?`e8f7ba9` 已通过完整 Build 0W0E、Core 339/339、World 1238/1238、WarCore 22/22、ARCH-A �?`git diff --check`；本�?Closeout 仅记录用户验收与状态�?
- 遗留：Region Dataset Feature、Runtime Binding �?Save/Reload 内容闭环转入 MAP-DATA-A-R1；不再为 MAP-DOC-A 新开 F5�?

## v0.2.26.20-fix
MAP-DOC-A-R3-F4 Dataset/Layer 文字居中�?026-08-11 22:53:02）：修复 `c019701` 未实际写入的四个 Dataset 行文字对�?Setter；仅收紧列表呈现，不修改 UI Token、Schema、Registry、Region、Renderer 或保存协议�?
- 变化：`datasetName`、`datasetStatus`、`datasetLayerName`、`datasetLayerStatus` 均水平与垂直居中；两�?Status 保留 64 DIP 最小状态区；Dataset/Layer 行高继续分别�?28/32 DIP�?
- 验证：F4 静态合�?`4/4 PASS`、AXAML XML 与版本四处一致；完整解决方案 Build `0 Warning / 0 Error`；Core `339/339`、World `1238/1238`、WarCore `22/22` PASS；ARCH-A �?`git diff --check` PASS。首�?Gate 曾被运行中的编辑器锁定，关闭后重跑通过�?
- 遗留：等�?`MAP-DOC-A-R3-F4-acceptance.md` �?F4-M01～F4-M03 真机验收；未进入 MAP-DATA-A，R3 不得宣布 CLOSED�?

## v0.2.26.19-fix
MAP-DOC-A-R3-F3 UI Spec Compliance Rework�?026-08-11 22:00:35）：用户真机裁定 R3-F2 �?FAIL，R3 保持 OPEN；按 UI Spec �?28/32 DIP 单行合同重做 Dataset/Layer，并修正 Dataset Inspector 优先级�?
- 变化：Dataset 仅显�?Name + Status；Layer 仅显�?Drag / Name / Status / Visible / Lock，并复用正式 `LayerPanel.States.axaml` 开关；选中 Dataset 时隐�?MapFormPanel，显示六�?Dataset 属性及“数据集属性”�?
- 验证：AXAML/SVG XML、静态合同与 Headless 300 DIP Bounds `4/4 PASS`；Solution Build `0 Warning / 0 Error`；Core `339/339`、World `1237/1237`、WarCore `22/22` PASS；ARCH-A�?+100、版本一致性与 `git diff --check` PASS�?
- 遗留：等�?`MAP-DOC-A-R3-F3-acceptance.md` �?F3-M01～F3-M08 真机验收；未进入 MAP-DATA-A�?

## v0.2.26.18-fix
MAP-DOC-A-R3-F2 UI 收口�?026-08-11 21:25:24）：功能验收通过后暂�?Closeout；重排现�?Dataset/Layer 编辑�?UI，等待专项真机验收�?
- 变化：Dataset 行固�?Name 主信息、`Type · ID` 单行辅助信息和右侧状态；Layer 行改用既�?Drag Handle、Visible/Hidden、Locked/Unlocked StreamGeometry 图标，并统一 28 DIP 操作按钮�?
- 选择：`DatasetSelectedId` 继续作为唯一选择源，Dataset 列表、Layer 与最�?Inspector 投影统一消费；检查器显示名称、类型、ID、状态、可见和锁定�?
- 交付物：本轮状态图�?`docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R3-F2-ui-closeout.svg`，XML 已校验�?
- 验证：AXAML XML、UI 项目快速构建与完整 Solution Build 均为 `0 Warning / 0 Error`；Core `339/339`、World `1233/1233`、WarCore `22/22` PASS；ARCH-A�?+100、版本一致性与 `git diff --check` PASS�?
- 遗留：等�?`MAP-DOC-A-R3-F2-acceptance.md` �?UI-M01～UI-R01 真机验收；未进入 MAP-DATA-A，未创建 Tag、Release、Merge、Rebase �?Force Push�?

## v0.2.26.17-fix
MAP-DOC-A-R3-F1 验收修复�?026-08-11）：R3 真机验收失败后修复左�?Dataset 行、Dataset Name 与右侧拖拽容器稳定性；R3 保持 OPEN�?
- 根因与修复：左侧 Item 容器�?Stretch；改为全�?ListBoxItem。拖拽期间原实现替换 `DatasetLayerItems`，导�?ListBox 容器重建�?Pointer Capture 冲突；改�?`DatasetLayerPanel` 内的 Visual-only 半透明和插入线，拖动期间不替换 Projection �?ItemsSource�?
- Dataset Name：Descriptor 新增可�?`name`；新建使用中�?Type 默认名，�?Manifest �?Name 仍可打开并在 UI fallback 为中�?Type。显式“应用名称”只�?Name，ID、Type、Source、Order、Visible、Locked 不变，保存重开恢复�?
- 验证：F1/R3 focused `8/8 PASS`；World 全量 `1232/1232 PASS`；最�?Solution、Core、WarCore、ARCH-A �?`git diff --check` 结果见本轮最终门禁。真机专项复验见 `MAP-DOC-A-R3-acceptance.md`�?
- 遗留：等�?F1-M01～F1-M06 后再继续 R3-M07/M08；未创建 Tag、Release、Merge、Rebase �?Force Push�?

## v0.2.26.16-rz
MAP-DOC-A-R3 Dataset Layer Editing�?026-08-11）：完成 DatasetLayerState 的可编辑持久化闭环，状态为 `READY FOR USER ACCEPTANCE`�?
- 变化：Manifest 新增唯一 `dataset_layer_state` 投影；旧 R2 Manifest 打开时在内存�?`visible=true`、`locked=false` 和连�?Order，正式保存或 Working Storage Promotion 才写盘。Create/Unregister 同步维护状态；锁定 Dataset �?UI �?Registry 两层均拒绝解除注册�?
- 交互：右�?Dataset 图层行满宽、整行选中态，显隐/锁定不改变选择；右侧独有阈值拖拽、半透明预览与插入线，左右列表共同按 Layer State.Order 投影�?
- 验证：R3 focused `6/6 PASS`；完整解决方案、Core/World/WarCore 测试、ARCH-A �?`git diff --check` 结果见本轮最终门禁。真�?IPO �?`docs/milestones/current/MAP-DOC-A/MAP-DOC-A-R3-acceptance.md`�?
- 遗留：等�?R3-M01～R3-M08 真机验收；未创建 Tag、Release、Merge、Rebase �?Force Push�?

## MAP-DOC-A-R2-CLOSEOUT
MAP-DOC-A-R2�?026-08-11）：用户真机确认 F4 核心路径成立——未保存地图可连续创�?Dataset，左�?Dataset Projection 与选择同步，解除注册后两侧同步移除，窗口仍显示未命名场景�?
- 结论：R2 �?Create、自�?ID、中�?Type、Selection、Unregister、Working Storage 与首次保�?Promotion 闭环完成，状态为 `CLOSED`�?
- 遗留：Dataset-backed Layer 的满宽行、显隐、锁定、拖拽排序和状态持久化不再塞入 R2，移�?`MAP-DOC-A-R3`；未将其伪称�?R2 已完成能力�?

## v0.2.26.15-fix
MAP-DOC-A-R2-F4 未保存地图工作存储（2026-08-11）：新建地图首次创建 Dataset 时惰性建立内�?Working Manifest，用户无需先保存正�?`map.json`�?
- 变化：正式路径与工作路径严格分离；Dataset 继续使用现有 Registry 事务和相�?`data/<id>.json`；首次正式保存只提升仍注册的 Dataset，目标碰撞、源文件缺失�?IO 失败 fail-closed，Working 数据保持完整�?
- 验证：F4 + F1/F2/F3 聚焦测试 `16/16 PASS`；完整解决方�?Build、全量测试和架构门禁结果以本轮最�?Gate 为准�?
- 遗留：待用户执行 F4-M01～M08 IPO；未获真机通过前保�?`READY FOR USER RE-ACCEPTANCE`，不得宣�?R2 CLOSED�?

## v0.2.26.14-fix
MAP-DOC-A-R2-F3 Dataset Selection + Layer Projection Sync�?026-08-11 17:08:16）：补齐 Dataset 单一选择合同，并将右侧“图层”接�?Dataset 投影�?
- 根因：Dataset 列表只有展示投影，没有可点击�?`SelectedDatasetId`；重开后解除注册没有目标，失败反馈又复用了新建表单 type�?
- 变化：左�?Dataset 行与右侧 Dataset-backed Layer 行共�?`SelectedDatasetId`；创建后自动选中新项；解除注册无选择时禁用并 fail-closed，成功后按下一项优先迁移；右侧不引�?Layer schema、持久化、显隐、锁定、拖拽或排序�?
- 验证：Dataset F3 focused `6/6 PASS`，Dataset F1/F2/F3 focused `15/15 PASS`；Solution Build `0 Warning / 0 Error`；Core.Tests `339/339`、WarCore.Tests `22/22`、World.Tests `1221/1221`；ARCH-A�?+100、版本一致性、`git diff --check` PASS�?
- 遗留：F3-M01～M09 �?R2-M06+ 真机验收待用户执行，状态为 `READY FOR USER RE-ACCEPTANCE`，不得宣�?CLOSED�?

## v0.2.26.13-fix
MAP-DOC-A-R2-F2 Dataset List State Sync + UI Refinement�?026-08-11 16:23:23）：修复 Dataset 创建成功后列表不更新，收�?Dataset 页面展示并移除手�?ID�?
- 根因：UI 投影原地改写同一�?`List`，ItemsControl 可能继续持有�?ItemsSource；内�?type 直接投影�?UI；创建命令仍依赖手填 ID�?
- 变化：创建完成后发布新的列表快照；六�?type �?UI 映射为区域、道路、城镇、资源、河流、地形区域；自动生成 `<type>-<6 位小�?hex>`，Registry 与源文件碰撞最多重�?16 次且拒绝覆盖；Dataset 行改为中文主类型、ID 副行、状态列�?
- 验证：F2 focused `17/17 PASS`；Solution Build `0 Warning / 0 Error`；Core.Tests `339/339`、WarCore.Tests `22/22`、World.Tests `1217/1217`；ARCH-A�?+100、版本一致性、`git diff --check` PASS�?
- 遗留：真�?F2-M01～M08 �?R2-M02 补验仍待用户执行，状态为 `READY FOR USER RE-ACCEPTANCE`，不得宣�?CLOSED�?

## v0.2.26.12-fix
MAP-DOC-A-R2-F1 Dataset Create/Register 真机链路修复�?026-08-11）：修复 Manifest 路径所有权混淆、Dataset Create 异步命令无最终结果反馈和双文件提交前校验缺口，暂�?M04～M07 等待重新验收�?
- 根因：`CurrentMapManifestPath` 原先回退到旧 `.xymap` 会话路径；按钮通过 fire-and-forget 调用 Create，失败不稳定可见；“命令收到”不是成功事实�?
- 变化：Create 只接受正�?`map.json` 路径；无路径明确拒绝；成�?失败写一条最终用户可见日志；Dataset Document �?Manifest 均在提交前校验，失败回滚且成功后�?Publish/Refresh�?
- 验证：F1 focused `9/9 PASS`；Solution Build `0 Warning / 0 Error`；Core.Tests `339/339`、WarCore.Tests `22/22`、World.Tests `1209/1209`；ARCH-A�?+100、版本一致性、`git diff --check` PASS�?
- 遗留：真机状态为 `READY FOR USER RE-ACCEPTANCE`，不得宣�?CLOSED�?

## v0.2.26.11-rz
MAP-DOC-A-R2-C4 Dataset UI 与验收材料（2026-08-11）：数据集页正式接入空态、新建、Type/ID/Status 列表和解除注册；补齐 R1-F1 �?R2 中文 IPO 真机清单�?
- 变化：新增独�?`DatasetPanel`，创�?解除注册�?UiVm 路由�?Registry；缺�?无效状态以单项列表状态展示�?
- 验证：C4 focused `7/7 PASS`；完整解决方�?build `0 Warning / 0 Error`；Core.Tests `339/339`、WarCore.Tests `22/22`、World.Tests `1202/1202`；ARCH-A�?+100、版本一致性、`git diff --check` PASS�?
- 遗留：R1-F1 M01～M04 �?R2 M01～M07 真机验收待用户执行；R2 �?`READY FOR USER ACCEPTANCE`，未宣布 CLOSED�?

## v0.2.26.10-rz
MAP-DOC-A-R2-C3 Dataset Registry 生命周期�?026-08-11）：完成 Create/Register/Resolve/Enumerate/FindById/Unregister 与跨文件创建事务，支持同 type �?Dataset 和单文件故障隔离�?
- 变化：注册表通过 Manifest Descriptor 驱动文件解析；创建事务先准备两个临时文件，提交失败清理新增文件并恢复 Manifest；解除注册不物理删除 Dataset 文件�?
- 验证：C3 focused `8/8 PASS`；覆盖创建、已有文件注册、查询、同 type 多项、缺�?损坏状态、重复与失败前置拒绝�?
- 遗留：C4 UI、最终完整门禁、R1-F1/R2 真机验收仍未完成；R2 不得标记 CLOSED�?

## v0.2.26.9-rz
MAP-DOC-A-R2-C2 Dataset Document/Storage�?026-08-11）：完成 `xuanyu-map-dataset` v0.1.0 严格五字段文档、空 features 约束�?Normal/Missing/Invalid 隔离加载�?
- 变化：新�?Dataset 文档序列化、校验、原子保存与 Descriptor 身份匹配；不引入 Geometry、Feature �?properties 语义�?
- 验证：C2 focused `10/10 PASS`；保�?读取、缺失、损坏、未知字段、非�?features、身份不匹配和失败不覆盖旧文件均覆盖�?
- 遗留：C3 Registry 生命周期与跨文件事务、C4 UI 与真机验收仍未完成；R2 不得标记 CLOSED�?

## v0.2.26.8-rz
MAP-DOC-A-R2-C1 Dataset Registry 合同�?026-08-11）：完成 typed Dataset Descriptor 与六�?type 白名单，收紧 Dataset ID、大小写不敏感唯一性和 map-root-relative `data/` source 安全规则�?
- 变化：Manifest `datasets` 从无语义 JSON 占位数组切换�?`id/type/source` Descriptor；既�?`assets` 空容器保持不变�?
- 验证：C1 focused `14/14 PASS`；`MapDatasetContractTests` 覆盖六类、重�?ID、非�?type、路径穿越和根目录约束�?
- 遗留：C2 文档文件与状态加载、C3 生命周期、C4 UI 与真机验收仍未完成；R2 不得标记 CLOSED�?

## v0.2.26.7-fix
MAP-DOC-A-R1-F1 Manifest Identity UI 修复�?026-08-11 14:58:21）：承接 R1 M07 已记录的真实失败，不改写历史，修�?Manifest ID 派生显示未即时刷新的通知链与默认宽度复制按钮裁切问题�?
- 变化：Manifest 切换时同步通知 `MapIdText` �?`MapIdDisplay`；Text、Tooltip、Copy 继续统一消费当前 Manifest ID�?
- 变化：ID 行改为值列 `*`、复制按钮列 `Auto`；ID 可省略显示，复制按钮与完�?Tooltip 保持可达�?
- 验证：R1-F1 focused `3/3 PASS`；具体记录见 `MAP-DOC-A-R1-F1-carryover.md`�?
- 状态：C0 已完成，准备提交推送；R1 M08 �?R1-F1 真机补验仍待用户执行�?

## v0.2.26.6-rz
MAP-DOC-A-R1 Map Content Navigation + Map Manifest�?026-08-11 14:09:18）：正式�?LAYER-A 远端基线切入 `feat/MAP-DOC-A`，收口地图工作区内容导航并建�?`map.json` Manifest 生命周期�?
- 变化：地图二级导航冻结为“地图基础 / 地图环境 / 数据集”；地图基础显示 Manifest ID 与坐标系；数据集只显示空态，不提前实�?Registry；地图环境不扩展 R1 Schema�?
- 变化：新�?`MapManifest`、严�?snake_case JSON DTO/Serializer/Validator、`MapManifestStorageService` 原子保存与候选读取、`MapManifestOwner`；窗口文件选择器接�?`map.json` 打开/保存�?
- 测试：新增创建、校验、序列化、UTF-8、Round-trip、未知字�?错误容器、失败安全与导航专项；保留旧 `.xymap` 场景引用链不变�?
- 验证：Solution Build 0 Warning / 0 Error；Core.Tests 339/339、World.Tests 1175/1175、WarCore.Tests 22/22；MAP-DOC-A-R1 相关聚焦 57/57；ARCH-A�?+100、版本一致性、`git diff --check` PASS�?
- Hash：`9001042`（实现提交）；状态：`READY FOR USER ACCEPTANCE`，等�?MAP-DOC-A-R1-M01～M08，未宣布 CLOSED�?

## v0.2.26.5-rz
LAYER-A-R1 通用图层栏与编辑职责分离�?026-08-11 13:40:02）：建立编辑模式通用图层 Dock，迁移真�?Region Layer，清�?Map 图层�?Region Drawing 串线�?
- 变化：新�?UI 无关 `IEditorLayerProvider`/`EditorLayerItem` 合同；管理模式隐藏图层栏，Map 编辑显示真实空状态，Region 编辑过滤 Region Layer；LayerInspectorPanel 迁入全局 Inspector；Map 旧图层二级页与区域绘制入口删除；Workspace 切换清理图层选择�?
- 验证：Editor.UI Build 0W0E；World.Tests fresh `1160/1160 PASS`；LAYER-A 聚焦组合/运行时合�?`4/4 PASS`；ARCH-A�?+100、`git diff --check` PASS。实�?Hash：`7255b85`；真�?LA-R1-M01～M08 待用户验收，阶段保持 `READY FOR USER ACCEPTANCE`�?
- 遗留：未修改 MapLayerKind、MapLayer、MapRegion、MapDefinition、Map JSON、Picking、Camera、Render；`_tmp_blind_rows/` 既有未跟踪目录未读取、未修改、未删除、未提交�?

## v0.2.26.4-rz
EDITOR-A-R3-F1 USER ACCEPTED closeout�?026-08-11 13:23:27）：记录用户批准�?P0 真机验收范围，EDITOR-A 正式收口并转�?LAYER-A-R1；该动作不计入新的开发轮�?
- 变化：新�?EDITOR-A closeout 记录，冻�?Manage/Edit Mode、Map/Region Workspace、项�?层级/检查器信息轴、共�?World/Camera/Selection、唯一 Main/VulkanViewport �?Region Drawing 不恢复边界�?
- 验证：用户已确认 P0 acceptance scope；当�?`feat/EDITOR-A-workspace` @ `b1f18b1` 与远端一致，ahead/behind `0/0`。本条不把自动门禁冒充真机证据�?
- 遗留：`_tmp_blind_rows/` 既有未跟踪目录按要求未读取、未修改、未删除、未提交；LAYER-A-R1 下一步从该远端基线切出�?

## v0.2.26.3-rz
EDITOR-A-R3-F1 Shell Compact & Unified Mode Selector�?026-08-11 12:14:39）：基于 R3 M01～M04 用户部分通过，删除重复底部资源浏览器，收敛为唯一 Log；Manage 顶部只显示“管理模式”�?
- 变化：双击“管理模式”与 Tab 共用 `ToggleEditorMode()`；Edit 时同一位置变为“地图编�?区域编辑 + Chevron”，菜单�?Radio 项直接在 Map/Region 间切换。单�?Mode NO-OP，Esc 仍只取消操作；GLB 导入继续由“文�?�?导入 GLB”承载�?
- 边界：不修改 Render、Picking、Camera、MapEditing 或日志系统；World、SceneStateOwner、MapSession、Camera、Selection、Project、唯一 Main/VulkanViewport 保持同一实例。R3-F1 仍等待用户真�?IPO，不�?CLOSED�?
- 验证：Solution Build 0W0E；Core 339/339、World 1156/1156、WarCore 22/22、R1/R2/R3/F1 聚焦 41/41；ARCH-A�?+100、版�?SVG XML �?diff 检�?PASS。AXAML 26�?5�?1），仅因删除重复 BottomDockHost。Hash、远端核验和 F1 IPO �?`EDITOR-A-R3-F1-shell-compact.md`�?
- Hash：`fc0e8f7f124344e1034d49604e26c4e4adfc0de6`（实现）；证据提交、推送与远端等值在本轮完成�?

## v0.2.26.2-rz
EDITOR-A-R3 Manage / Edit Mode 与默�?Shell�?026-08-11）：�?EDITOR-A-R2 �?Workspace 顶层原型重定为底层编辑目标；新增 Manage/Edit Mode，启动默认管理模式、编辑目标为地图，Tab 只负�?Mode 切换�?
- 变化：纯 Editor `EditorModeManager` 持有 Manage/Edit；Map/Region Workspace 保留并仅�?Edit Mode 生效。Project、Inspector、唯一 Main/VulkanViewport、资源浏览器与日志成为常�?Shell；底部资源页复用已有“导�?GLB”文件选择/导入链，右侧旧“地图编辑器”顶�?Tab 退役。Map Context 进入左侧地图 Tab �?Inspector；Region 保持 REGION-A 前的占位，不启用 Drawing/Draft�?
- 边界：Esc 继续只取消操作，Tab 在非 TextBox 焦点下切�?Manage/Edit；模式和编辑目标切换均保�?World、SceneStateOwner、MapSession、Camera、Selection、Assets、Project 与唯一 Viewport。R2 的单视口、NO-OP、状态保留和 Region 隔离成果保留，但其产品层级标记为 `USER ACCEPTANCE FAILED · SUPERSEDED BY R3 MODE MODEL`�?
- 验证：Solution Build 0W0E；Core 339/339、World 1154/1154、WarCore 22/22、R1/R2/R3 聚焦 39/39；ARCH-A�?+100、版�?SVG XML、远端核验与 R3 中文 IPO �?`EDITOR-A-R3-mode-shell.md`。自动门禁不替代用户对默认启动、导入、Tab、Esc/Tab 分工、连续切换和最小窗口的真机验收�?
- Hash：`17aa91be1624b96beb2f97d24a6c199c0733a269`（实现）、`14de0cff3800374d2f75a9edfa2ad6cb5ae4aa79`（门禁证据）；本轮推送后远端等�?`0/0`�?

## v0.2.26.1-rz
EDITOR-A-R2 Workspace Switch UI�?026-08-11 10:53:38 +08:00）：�?`feat/EDITOR-A-workspace` �?R1 纯合同上，交付可见的 Workspace Selector �?Map/Region 上下文切换；不重建唯一 Main/VulkanViewport，不启动 REGION-A 或旧 MAP-A F1 路径�?
- 变化：`UiVm` 持有唯一 `EditorWorkspaceManager`；同目标切换为无副作�?NO-OP，变更时复用既有 `CancelActiveInput`、回到选择工具、更新绑定并记录一条工作区低频日志。地�?Workspace 保留既有 Left/Right �?MapEditorPanel；区�?Workspace 只显示“区�?区域属性”冻结占位，不提�?Region Drawing、假数据或列表�?
- 回归：新�?13 �?UI/组合合同，连�?R1 �?21 项聚�?Workspace 测试，锁�?Map↔Region、NO-OP、工具复位、Camera/MapSession/World Owner/Selection 保留、无 Draft、唯一 Main/Viewport、Host 隔离、Map 面板可达�?Region 占位边界。R1 架构图同步改为浅色，新增 R2 浅色结构图�?
- 验证：Solution Build 0 Warning / 0 Error；Core.Tests 339/339、World.Tests 1136/1136、WarCore.Tests 22/22 PASS；ARCH-A�?+100、四处版本、两�?EDITOR-A SVG XML �?diff-check PASS。首�?World 测试发现 AXAML 扫描基线 24 未包含本�?3 �?UI 文件；更新为真实�?27 后重建重跑通过。远端核验与用户真机 IPO �?`EDITOR-A-R2-workspace-switch.md`；状态必须保�?`READY FOR USER ACCEPTANCE`，不能由自动测试改写�?CLOSED�?
- Hash：`c7b1ca8`（Workspace Switch UI�?3 �?R2 回归、浅�?SVG、版本与门禁材料）；`26db31f`（门禁证据）�?

## v0.2.26.0-rz
EDITOR-A-R1 Workspace Contract�?026-08-11）：从已验证�?MAP-A 战略收口远端 `3dd091f` 创建 `feat/EDITOR-A-workspace`，在�?`XuanYu.Editor` 层建�?Map/Region Workspace 身份、定义、唯一 Manager 与无副作用切换合同�?
- 变化：切换结果要求结束临�?Tool、保留既�?World/Camera/兼容 Selection、回�?Select；新�?8 项聚焦回归与 Workspace Contract 架构图，锁定双向/重复切换、无 Region Drawing Tool、无 World/Camera 副本与无 Vulkan 引用。未实现 Workspace UI、Region Drawing、Renderer/Picking 重写�?Schema�?
- 验证：定�?Build 0 Warning / 0 Error；EDITOR-A-R1 focused tests 8/8 PASS；最�?Solution Build 0 Warning / 0 Error，Core.Tests 339/339、World.Tests 1123/1123、WarCore.Tests 22/22 PASS；ARCH-A�?+100、宪�?2.2、四处版本一致性、SVG XML �?diff-check PASS�?
- Hash：`4cabf42`（Workspace Contract�? 项回归、版本同步与 Transition 文档）；`2b90a46`（用户验收证据）；Push �?Remote HEAD 核验同轮完成�?

## MAP-A-STRATEGIC-CLOSEOUT
MAP-A �?EDITOR-A Transition Round 阶段 A�?026-08-11）：保留 `MAP-A-R3-D2-F1 = FINAL ACCEPTANCE FAILED · 5 ITEMS REMAIN` 的真实事实，将旧 Region Drawing 产品路径战略终止�?`SUPERSEDED · NOT ACCEPTED`，迁移目�?`REGION-A`；不会把�?F1 改写�?PASS �?CLOSED�?
- 变化：新增战略收口与知识审计记录；新�?K-ARCH-002、L-ARCH-001 �?`REGION-A-MIG-001` Backlog；Map/Region Domain、Picking、Camera、Vector Overlay、Depth Policy、Ear Clipping、动�?Buffer �?latest-state-wins 保留为后�?Workspace 的共享复用合同�?
- 验证：Solution Build 0 Warning / 0 Error；Core.Tests 339/339、World.Tests 1115/1115、WarCore.Tests 22/22 PASS；ARCH-A�?+100、宪�?2.2 标题/版本字段、产品版本一致性与 `git diff --check` PASS。本条不会停�?MAP-A，推送后同轮创建 `feat/EDITOR-A-workspace` 并实�?R1�?
- Hash：`6724079`（MAP-A 战略终止、知识审计、REGION-A Backlog �?EDITOR-A 迁移边界）�?

## GOV-2026-08-11-MKRG-01
里程碑知识沉淀门禁治理修订�?026-08-11 09:56:10 +08:00）：按当前主宪法实际最高条款顺延新增第八十六条，明确每个正�?Milestone �?`CLOSED` 前必须完�?`Milestone Knowledge Review`；建�?`KNOWLEDGE`、`LESSON`、`CHANGELOG_ONLY`、`BACKLOG`、`REJECTED`、`CONSTITUTION_CANDIDATE` 六类筛选、证据、去重、禁止自动升格和关闭顺序�?
- 变化：同�?`docs/dev-rules.md`、知识库 README，新�?MAP-A-CLOSE �?C1～C4 修订计划�?AC-U07～AC-U10，更�?MAP-A backlog、docs 索引�?file-tree；不修改产品代码，不执行 MAP-A 产品/架构收口�?
- 验证：`git diff --check`、`scripts/arch-a-guard.ps1`、治理文档引�?条款编号/产品代码范围检�?PASS；本轮未运行产品 Build/Test；当�?`MAP-A-R3-D2-F1` 仍保�?`OPEN · FINAL ACCEPTANCE FAILED · 5 ITEMS REMAIN`�?
- Hash：`7109d7b`（治理条款、执行手册、知识库说明�?MAP-A-CLOSE 计划）�?

## v0.2.25.33-fix
MAP-A-R3-D2-F1 F1-FAR-RECOVERY-01�?026-08-11 01:07:31 +08:00）：真机日志确认 FarPlane 会保留极�?Dolly 的历史最大值，返回正常距离后仍维持病�?Near/Far 比；本轮�?Far 改为每次仅由当前距离计算，并给编辑器相机设定 1,000km 工作上限�?
- 变化：透视导航�?`FarPlane = max(NearPlane×10, CurrentDistance×4)`，不再读取先�?FarPlane；Orbit、Pan �?Dolly 共用该计算。距离钳制从 1,000,000km 收敛�?1,000km，命中上限时只写一条可见编辑器警告；保�?F1-FAR-SAFE-01 �?VP/Metric 失败安全，不修改 Grid、Depth、Ground、Region �?Camera-relative Rendering�?
- 验证：Core.Tests 339/339、World.Tests 1115/1115、WarCore.Tests 22/22、ARCH-A、`git diff --check` PASS；完整解决方�?Build 被运行中�?`XuanYu.Editor.App`（PID 40508）锁�?Editor/UI 输出 DLL，未重试、未将其记为通过。真�?IPO 待执行�?
- 知识治理：补�?L-REN-002，动态安全边界必须允许随当前需求收缩，不能只向历史极值扩张�?
- Hash：`ff6ade4`（F1-FAR-RECOVERY-01：Far 回落�?,000km 相机工作上限与回归）�?

## v0.2.25.32-fix
MAP-A-R3-D2-F1 F1-FAR-SAFE-01�?026-08-11）：实机捕获极远 Dolly �?`ViewProjection 矩阵不可逆` 未处理异常，F1-M15 改判 FAIL；F1 更新�?10/15 PASS、`OPEN · FINAL ACCEPTANCE FAILED · 5 ITEMS REMAIN`�?
- 变化：`ViewportMetricScale.TryCreate` 对不可�?VP 返回 false；RenderProjection 无法构造时安全失败；Dolly �?VP 之前使用�?double 几何计算中心射线�?X/Y Metric，仅�?10km�?00km�?000km 等距离档时写入可见编辑器日志。撤销不可见且依赖 VP �?Vulkan Debug 诊断；不修改 MaxDistance、FarPlane、Depth、Grid Step �?Region�?
- 知识治理：新�?`L-REN-002`，固定“double fallback 必须位于第一�?float 降精度之前”；Camera-relative Rendering / Render Origin Rebasing 留作后续独立架构决策�?
- 验证：待本轮正式门禁及真机确认极�?Dolly 不崩溃，并收�?M03/M04/M05/M15 的日志证据�?
- Hash：`b22c45b`（F1-FAR-SAFE-01 失败安全、可见诊断与极远回归）�?

## v0.2.25.31-fix
MAP-A-R3-D2-F1 F1-FAR-DIAG-01�?026-08-11）：F1 FINAL 真机裁定�?11/15 PASS；M03/M04/M05（极远缩放下 Grid 消失/卡顿�?World Axis 同步闪烁）和 M06（四�?Region 闭合/图层删除）FAIL，F1 保持 `OPEN · FINAL ACCEPTANCE FAILED · 4 ITEMS REMAIN`�?
- 变化：仅增加�?Camera Revision 去重的非阻塞调试诊断，输�?Camera Position/Target/Distance、Near/Far、Metric X/Y 与有效性、Grid Step、中心射线、Z=0 平面交点 `t`、`t/Far`，并明确 Grid 截断�?Far、Axis 截断�?Far×0.75；Camera、Depth、Ground、Fullscreen Grid、Step �?Region 行为未变�?
- 验证：待本轮正式门禁与用户按 F1-FAR-DIAG-01 收集 M03～M05 证据；M06 不混入本轮�?
- Hash：`c4bc9b3`（F1-FAR-DIAG-01 诊断与验收状态记录）�?

## v0.2.25.30-fix
MAP-A-R3-D2-F1-CLOSEOUT�?026-08-11 00:03:14）：记录 RW-2A/RW-2B 真机 PASS；冻�?World Grid 独立 Fullscreen、World XY（Z=0）、深度关闭、Ground 独立、CPU 全帧 Step�?/2/5 �?24~80 DIP 回滞，RW-2C/RW-2D 降级�?`DEFERRED · NON-BLOCKING VISUAL IMPROVEMENT`。新�?F1 FINAL 15 �?IPO 真机清单；未取得 15/15 �?F1 保持 OPEN�?
- 知识治理：建立扁�?`Lesson` 类型�?`L-REN-001`，新�?`INC-2026-08-10-006` �?`K-REN-004`，知识索引升级为 ID/类型/分类，并同步 README、docs-index、file-tree �?R3 backlog；Lesson 严格区分已确认事实与高置信但�?GPU Capture 直接证明的机制解释�?
- 验证：完整解决方�?Build 0 Warning / 0 Error；Core.Tests 335/335、World.Tests 1115/1115、WarCore.Tests 22/22 PASS；ARCH-A�?+100、SPIR-V �?GLSL�?69 words）与 `git diff --check` PASS。F1 FINAL 真机验收仍待用户逐项裁定�?
- Hash：`4ab3928`（F1 CLOSEOUT、Incident、Lesson �?Knowledge 沉淀）�?

## v0.2.25.29-fix
MAP-A-R3-D2-F1 GRID-RW-2B�?026-08-10 23:50:35）：World Grid 保持 RW-2A �?Fullscreen Triangle、World XY（Z=0）、DepthTest/DepthWrite 关闭�?MapGround 独立性；仅将固定 100m 改为 `ReferenceGridFrameState` 每帧统一 Step。Step 按保�?`max(X,Y)` 公制尺度�?/2/5 序列�?24~80 DIP 回滞切档，Shader 只消�?`gridState.x`，`fwidth` 仍仅用于 AA，未引入 Fragment LOD�?
- 测试：新�?100�?00�?00 与回滞区间回归；更新 Shader/Draw 合同，锁定单�?Step、禁�?BaseHeight/local LOD 与旧 LineList 正式入口；GLSL �?SPIR-V�?69 words）一致�?PASS�?
- 验证：Render.Vulkan �?Core.Tests 定向 Build 0 Warning / 0 Error；Core.Tests 335/335、World.Tests 1115/1115、WarCore.Tests 22/22 PASS；ARCH-A�?+100 �?`git diff --check` PASS。完整解决方�?Build 未重试：当前环境�?Editor.App 输出锁定尚未变化，沿�?RW-2A 已记录的环境阻断�?
- 状态：RW-2B 等待真机确认拉远时全帧整体减密且�?100�?00 抖动；RW-2C/2D �?BLOCKED，F1 保持 OPEN�?
- Hash：`6154078`（RW-2B 帧级自适应密度实现）�?

## v0.2.25.28-fix
MAP-A-R3-D2-F1 GRID-RW-2A�?026-08-10 23:39:57）：按独立世界网格裁定，恢复 MapGround 正常 Draw；Reference Grid 改为全屏三角形，�?`editor_reference_grid.vert` 重建射线后与 World XY（`Z=0`）求交，固定 100m 间距。Grid Pipeline 关闭 DepthTest/DepthWrite，移除正式入口对 GridLine、LineList �?Ground Bias 的依赖；Map `BaseHeightMeters` 不再参与 World Grid 平面。旧 GridLine Shader/字节�?管线资产暂保留，不删除�?
- 测试：新�?Ground 恢复�?World Grid 独立合同；更�?DrawPlan、全�?Pass、固�?Z=0、固�?100m、禁 Fragment LOD 与禁 Ground Bias 合同；GLSL �?glslc -O 生成 SPIR-V�?38 words）并一致性校�?PASS�?
- 验证：Render.Vulkan 快�?Build 0 Warning / 0 Error；Core.Tests 334/334、World.Tests 1115/1115、WarCore.Tests 22/22 PASS；ARCH-A�?+100、`git diff --check` PASS。完整解决方�?Build 因运行中�?`XuanYu.Editor.App (PID 13416)` 锁定输出 DLL 失败（MSB3027/MSB3021），为环境阻断，非代码错误�?
- 状态：RW-2A 等待真机仅验 Ground ON/OFF 下网格独立存在与缩放不消失；RW-2B/2C/2D �?BLOCKED，F1 保持 OPEN�?
- Hash：`2c57893`（RW-2A 独立世界网格实现）�?

## v0.2.25.27-fix
MAP-A-R3-D2-F1 GRID-DIAG-GROUND-01�?026-08-10 23:23:52）：仅在 Vulkan 帧循环中于管线绑定前跳过 `RenderDrawKind.MapGround`，暂时隔离真�?MapGround 绘制及其深度写入；MapGround 数据、DrawPlan、Reference Grid、World Axis、World Origin、Navigation Gizmo、Camera、Shader、Depth Bias �?`BaseHeightMeters` 均未修改。新增诊断合同，锁定跳过位置必须早于管线绑定�?
- 验证：Render.Vulkan 快�?Build 0 Warning / 0 Error；全解决方案 Build 0 Warning / 0 Error；Core.Tests 336/336、World.Tests（含新增诊断合同）与 WarCore.Tests 22/22 PASS；ARCH-A�?+100 �?`git diff --check` PASS�?
- 状态：GRID-DIAG-GROUND-01 等待用户真机验收；未据自动测试宣�?F1 CLOSED，未启动 Ground/Grid 分层�?Camera/Depth 后续修改�?
- Hash：`9cf951a`（MapGround 诊断隔离实现）�?

## v0.2.25.26-fix
MAP-A-R3-D2-F1 GRID-RW-1-CORR2�?026-08-10 22:34:00）：按用户真机审计冻结四组修复—�?
�?ReferenceGridFrameState �?Step 选择改用保守尺度 `max(X,Y)`：斜视各向异性（�?2/30 m/DIP）下只要任一方向过密即升级网格；公共 `ViewportMetricScale.MetersPerDip`（min）保持不变，继续服务比例尺；
�?参考网格管线拆为专�?Empty-input procedural LineList 管线（新�?`VulkanGraphicsPipelineOwner.GridLine.cs`），移除复用 CreateFullscreenPass 时的 StaticModel VertexBinding/Attributes，并启用�?Depth Bias（ConstantFactor=-4.0）消除与 MapGround 共面 Z=BaseHeight 的深度竞争；
�?Shader 增加 Major/Minor 层级（世界坐�?10×Step 整数倍为 Major；Minor α=0.10 / Major α=0.18）与连续远距/掠射 Fade（Minor 0.30~0.55 dMax 提前淡、Major 0.55~0.85 dMax 保持更远，掠�?0.03~0.12 地平线连续归零）；禁�?band-pass / local LOD / 突然 discard 回归�?
�?GLSL 已由 glslc -O 重新生成嵌入 SPIR-V（vert 751 �?/ frag 131 词；工具链一致性复�?PASS，SPIR-V �?C# 逐词一致）�?
- 测试：新�?FrameState 各向异性门禁（2/30�?0/2�?.5/50 �?max �?Step�?/2 各向同性对照不变）；Shader 合同断言 Major/Minor Alpha、Fade 区间、Depth Bias �?Empty-input 管线（无 StaticModelVertexBinding 调用、无 CreateFullscreenPass 调用）�?
- 验证：全解决方案 Build 0 Warning / 0 Error（含 `--no-incremental` 全量重编译）；Core.Tests 336/336、World.Tests 1114/1114、WarCore.Tests 22/22 PASS；ARCH-A PASS�?+100 PASS（GridLine.cs 100 行、字节码 Vert 85/Frag 23）；GLSL/SPIR-V 一致�?PASS；`git diff --check` PASS�?
- 状态：F1 保持 OPEN；GRID-RW-1-CORR2 等待 Commit + Push 与用户逐项代码审计（FrameState �?anisotropy �?Pipeline �?Shader �?Depth �?Major/Minor �?Fade �?Tests �?SPIR-V �?门禁）；审计通过前不启动真机，RW-2 / RW-3 不启动�?
- Hash：`c5652f3`（GRID-RW-1-CORR2 实现与文档提交）�?

## v0.2.25.25-fix
MAP-A-R3-D2-F1 GRID-RW-1�?026-08-10 21:49:43）：Reference Grid 从全屏三角形�?Fragment 局�?LOD 重写为全局 `ReferenceGridFrameState` 驱动�?GPU procedural 世界线；全帧固定一�?100m 起步�?0~140 DIP 回滞�?Step，并按相机位�?Step 吸附 Anchor。Vulkan 参考网格专�?`LineList` 管线每轴生成 513 条线、总计 2052 顶点；全屏三角形常量独立保留�?ViewPlaneGrid、比例尺、导�?Gizmo、世界轴和原点。删除旧 `fwidth/log10/band-pass/grazing` 网格着色器、字节码与错误合同，新增世界线、全局尺度、锚点、顶点数�?LineList 合同；GLSL 已由 `glslc -O` 重新生成嵌入 SPIR-V�?
- 验证：全解决方案 Build 0 Warning / 0 Error；Core.Tests 331/331、World.Tests 1114/1114、WarCore.Tests 22/22 PASS；ARCH-A�?+100、GLSL/SPIR-V 一致性与 `git diff --check` PASS�?
- 状态：F1 保持 OPEN；GRID-RW-1 等待完整门禁、Commit + Push 和用户真机验收；RW-2 / RW-3 未启动�?
- Hash：`fcf4996`（GRID-RW-1 实现）�?

## v0.2.25.24-fix
MAP-A-R3-D2-F1 SCALE-R2 + GRID-2B�?026-08-10）：比例尺采�?1/2/5 × 10�?离散档位，按 104 DIP 可容纳的最大档位选择，低�?100m 隐藏并保�?5% 回滞；Reference Grid 增加 projected-cell band-pass�?0~18px 淡入�?0~140px 淡出）、独�?X/Y 投影密度�?00m/D/10D 三层候选及按物�?spacing 稳定�?Alpha，未修改 MapBounds、BaseHeight、Camera、Picking、Region �?192B 布局�?
- 验证：全解决方案 Build 0 Warning / 0 Error；Core.Tests 391/391、World.Tests 1114/1114、WarCore.Tests 22/22 PASS；最�?Render.Vulkan 目标 Build 0 Warning / 0 Error；ARCH-A、`git diff --check` PASS；GLSL glslc -O 编译通过并重新生�?GridFrag SPIR-V�?
- 状态：F1 保持 OPEN；GRID-3 未启动，等待 SCALE-R2 + GRID-2B 真机验收�?
- Hash：`03259c7`（SCALE-R2�? `05aced2`（GRID-2B）�?

## v0.2.25.23-fix
MAP-A-R3-D2-F1 SCALE-R1 + GRID-2A�?026-08-10）：比例尺对 104 DIP 对应真实距离执行 100m 起步、两位有效十进制向下吸附；低�?100m 隐藏，不改变 Camera Zoom。Reference Grid 改由 Fragment 根据局�?world-per-pixel 计算十进制层级，独立执行层级密度淡出，X/Y 线交叉使�?`max`，保�?BaseHeight�?92B Push Constant 和既有地�?拾取边界�?
- 验证：全解决方案 Build 0 Warning / 0 Error；Core.Tests 388/388、World.Tests 1114/1114、WarCore.Tests 22/22 PASS；ARCH-A、`git diff --check` PASS；GLSL glslc -O 编译通过并重新生�?GridFrag SPIR-V�?
- 状态：F1 保持 OPEN；GRID-2B/GRID-3 未启动，等待下一轮完整门禁与真机验收�?
- Hash：`e2df879`�?

## v0.2.25.22-fix
MAP-A-R3-D2-F1 GRID-1 参考网格越过地图边界（2026-08-10 20:31:45）：保留现有 192B Push Constant 布局�?`mapBounds.z` BaseHeight，仅移除 Reference Grid Fragment Shader 的地图矩形可见�?Fade；地图范围、MapSurface、Picking、相机、LOD、颜色、线宽与地平线淡出均未修改。新�?Shader 合同，约束不得恢�?`mapFade` �?x/y 边界裁剪�?
- 验证：全解决方案 Build 0 Warning / 0 Error；Core.Tests 386/386、World.Tests 1114/1114、WarCore.Tests 22/22 PASS；ARCH-A、`git diff --check` PASS；SPIR-V 已由 glslc -O 重新生成�?
- 真机 IPO�?0km 地图移动/环绕至边缘，绿色 MapSurface 结束后，灰蓝 Reference Grid 应继续延伸；F1 保持 OPEN，等待用户观察�?
- Hash：`e5c6396`�?

## v0.2.25.21-fix
MAP-A-R3-D2-F1 比例尺固定几何与浅色 UI 视觉收口�?026-08-10 20:10:03）：固定 Vulkan-native 比例尺卡片为 128×28 DIP、左下角 16 DIP 边距，标尺固�?104 DIP；距离标签改�?`metersPerDip × 104` 的真实动态值，不再�?1/2/5 档位改变标尺几何；背景、边框、文字和标尺线统一使用玄域浅色 Token 对应色值，圆角 3 DIP，去除黑色大底与七段数码管字形�?
- 测试：新增固定卡�?标尺合同、真实标签与宽度回归、Shader 视觉合同；重新生成嵌�?SPIR-V�?
- 验证：Core 385/385、World 1114/1114、WarCore 22/22 PASS；Render.Vulkan 项目 Build 0 Warning / 0 Error；ARCH-A、`git diff --check` PASS。全解决方案 Build 仍因运行中的 `XuanYu.Editor.App (PID 37800)` 锁定输出 DLL 返回 MSB3027/MSB3021，真实记录为环境阻断�?
- 状态：F1 继续 `OPEN · ACCEPTANCE FAILED · REWORK`，等待用户真机确认固定几何、标签真实性、浅色视觉、缩放和 Resize/DPI；不宣告关闭�?
- Hash：`9198886`（功能与文档提交）�?

## v0.2.25.20-fix
MAP-A-R3-D2-F1 V06 鼠标滚轮缩放与比例尺解耦修复（2026-08-10 19:50:33）：保留 100m 参考网格下限，比例尺改为独�?1/2/5 动态序列，修复小尺度标签格式，限制比例尺条宽度不超�?160 DIP、高度调整为 32 DIP；删除地图编辑器 100m Zoom Floor �?Dolly 调用，避免比例尺反向限制相机缩放；补充真实尺度、宽度与 Overlay 边界回归�?
- 文档：同�?MAP-A-R3 backlog、Viewport Overlay 开发计划、file-tree 与四处版本号；删除失效的 Zoom Policy 及其旧合同测试�?
- 验证：Core 385/385、World 1114/1114、WarCore 22/22 PASS；World.Tests 项目 Build 0 Warning / 0 Error；ARCH-A、`git diff --check` PASS。解决方案完�?Build 因运行中�?`XuanYu.Editor.App (PID 25620)` 锁定输出 DLL，真实阻断并返回 MSB3027/MSB3021，未伪装为代码失败�?
- 状态：F1 继续 `OPEN · ACCEPTANCE FAILED · REWORK`，等待用户真机重�?V06 动态比例尺、缩放范围与 Resize/DPI；不宣告关闭�?
- Hash：`b5b0f5f`（功能与文档提交）�?

## v0.2.25.19-stab
MAP-A-R3-D2-F1 OVL-R0～R3 比例尺承载层整改�?026-08-10 18:27:02）：正式裁定 STAB-5A �?`FAILED · WRONG PRESENTATION ARCHITECTURE`，以 `ac5d306` 作为 Native Popup 路线终点；新增统一 DIP Overlay Layout Contract，并将比例尺�?`RenderDrawKind.ScaleIndicatorOverlay` 接入 Vulkan DrawPlan，固定绘制在 Navigation Gizmo 之前且关闭深度测�?写入�?
- Vulkan 比例尺使用视口左下角 16 DIP 锚点、screen-space bar/tick 与仅支持 `0-9/m/k/./空格` �?`ScaleIndicatorGlyphLite`；数据沿 `UiVm �?RenderProjection �?DrawPlan �?Vulkan` 单链传递，不在 Vulkan 重算公制尺度�?
- 删除比例尺专�?`VulkanNativeHost.ScaleIndicator.cs`、`Win32ViewportHost.ScaleIndicator.cs`、GDI/WM_PAINT/Probe/Popup 状态；保留通用 `Win32ViewportHost`，并把被旧文件错误夹带的 `WS_CLIPSIBLINGS` 常量归还通用 Host�?
- 治理：新�?Viewport UI 控件知识库、OVL 开发计划和浅色路线图；自动门禁通过后状态只能进�?`READY FOR USER ACCEPTANCE`，F1 继续 OPEN�?
- 知识治理：正式导�?`docs/knowledge/` 扁平 V1 知识库（19 �?Knowledge、代表�?Incident 与索引）；可由本�?Git 可靠追溯的历�?Commit 已补齐；开发宪法修订为 2.1，新增第十六章“知识库、事故复盘与经验沉淀”，原最终原则顺延为第十七章�?
- 验证：解决方�?Build 0W0E；Core 383/383、World 1117/1117、WarCore 22/22 PASS；ARCH-A�?+100、版本一致性、SVG XML、GLSL glslc 与嵌�?SPIR-V 一致性、`git diff --check`、启动冒烟（进程存活 8 秒）PASS�?
- 遗留：用户真机确认比例尺悬浮可见前，不宣�?F1 CLOSED�?
- Hash：`3f0a801`（功能实现）；`b3b024c`（知识治理哈希收口）�?

## v0.2.25.18-stab
MAP-A-R3-D2-F1 STAB-5A 比例尺真机可见性收口（2026-08-10 16:51:42）：将比例尺�?Vulkan 同级 `WS_CHILD` 改为拥有主窗口的独立 `WS_POPUP` 悬浮控件，保留点击穿透与非激活行为；修复 App 输出副本未同步导致的假验证；�?Avalonia 视口布局位置重新定位，并将异常过宽的比例尺显示限制在可见悬浮范围内�?
- 新增比例�?HWND/可见�?矩形/文本/WM_PAINT 探针回传，修复状态更新重�?PaintCount 的问题；本轮无新增、删除或移动文件，`file-tree.md` 无结构变化无需更新�?
- 验证：解决方�?Build 0W0E；ViewportScaleIndicatorContractTests 1/1、ScaleIndicatorVisibilityRuntimeTests 2/2、WarCore.Tests 22/22；ARCH-A�?+100、`git diff --check` PASS；真机重启编辑器后已看到视口内悬�?`100 m` 控件。F1 仍保�?`OPEN · ACCEPTANCE FAILED · REWORK`，填�?闭合等其他验收项不在本轮范围�?
- 遗留：等待用户对比例尺位置与其他 F1 项执行正�?IPO 真机验收，未宣告 F1 CLOSED�?
- Hash�?6b26e9�?

## v0.2.25.17-stab
MAP-A-R3-D2-F1 STAB-4A/4B/4C 根因修复�?026-08-10）：�?Native 比例尺改为与 Vulkan HWND 同父级的兄弟窗口，显式置�?Vulkan 之上，并记录 HWND、可见性、矩形、文本、宽度与 WM_PAINT 次数；视�?Metric 拆为 X/Y 方向值，比例尺消�?X，Zoom Floor 取较小方向且 Metric 失败保持上一合法相机；Vector Overlay 删除过期 Clip-Z Bias，Fill、Stroke、Marker 直接使用 ViewProjection，继续使用无深度测试/写入 Pass 与绘制顺序�?
- 新增斜视尺度、Metric fail-closed�?0km 地图 Fill 投影、Native Overlay Probe 与无 Bias Shader 合同回归�?
- 验证：解决方�?Build 0W0E；Core 366/366、World 1117/1117、WarCore 22/22；ARCH-A�?+100、ShaderBytecode glslc 生成�?`git diff --check` PASS；真机比例尺可见性与俯视/45°/低角�?Fill 稳定性待用户重验，F1 保持 `OPEN · ACCEPTANCE FAILED · REWORK`�?
- 遗留：未执行真机验收，不宣告 A02、B03、C01、C02 �?F1 CLOSED�?
- Hash：c307c66�?

## v0.2.25.16-fix
MAP-A-R3-D2-F1 A02 比例尺悬浮与 100m Zoom Floor 修复�?026-08-10 15:06:09）：删除底部独立 Avalonia 比例尺行，改�?Native Vulkan 视口内右下角悬浮控件，保留点击穿透；有效地图视口无论“检查器”或“地图编辑器”标签均显示比例尺。Map Editor Zoom Policy 改为所有有效地图视口生效，比例尺与相机缩放均禁止低�?100m，彻底消�?`0 m`�?
- 验证：解决方�?Build 0W0E；Core 365/365、World 1118/1118、WarCore 22/22；比例尺 Native Overlay�?00m metric �?Inspector Zoom Floor 合同 PASS；ARCH-A（含 5+100）、版本一致性、git diff --check PASS；真机重�?A02 仍待用户执行，F1-V1 保持 `OPEN · ACCEPTANCE FAILED · REWORK`�?
- 遗留：重启编辑器后确认比例尺位于视口右下角且不占独立行，滚轮�?100m 后继续滚轮不再深入，再继�?A02～D04 联合真机重验�?
- Hash�?cd5e82�?

## v0.2.25.15-stab
MAP-A-R3-D2-F1 稳定化修复（2026-08-10 14:22:43）：修复 Avalonia 输入路径绕过 Navigation Gizmo 导致 Region 误加点的问题，统一 Gizmo 可见端点/轴线命中与手势所有权；将 Scale Indicator 移到 Native Vulkan HWND 之外的独�?Avalonia 行；�?Vector Overlay 创建独立无深度测�?无深度写�?Pass，保�?Fill �?Stroke �?Marker 绘制顺序，消除透明共面 Overlay �?Ground 的深度争抢；不修改世界锚点�?00m 网格、Zoom Floor 或双精度 Picking�?
- 验证：解决方�?Build 0W0E；Core 364/364、World 1116/1116、WarCore 22/22；定�?Core 3/3、World Region/Gizmo/Overlay 38/38；ARCH-A（含 5+100）、版本一致性、git diff --check PASS；真机需重验 A02、B03、C01、C02，F1-V1 保持 `OPEN · ACCEPTANCE FAILED · REWORK`�?
- 遗留：执行比例尺可见、Gizmo 不误�?Region 点、俯�?45°/低角�?Overlay 稳定性与未完成真机项联合重验；未通过前不宣告 F1 CLOSED�?
- Hash�?51da52�?

## v0.2.25.14-fix
MAP-A-R3-D2-F1-V1-REWORK-B2 Vector Overlay Depth Policy�?026-08-10 13:51:49）：保持 Fill、Stroke、Marker �?Ground 的世界锚点完全重合；主管线继续使�?DepthTest=On、DepthWrite=On、LessOrEqual，scene.vert 仅对 Vector Overlay 在裁剪空间施加有�?bias，按 Fill �?Stroke �?Marker 建立视觉层级；重新生�?scene.vert SPIR-V 字节码�?
- 验证：解决方�?Build 0W0E；Core 361/361、World 1116/1116、WarCore 22/22；B2 专项 14/14，覆盖俯视�?5°�?0°�?9°与极近合法正�?Zoom；ARCH-A（含 5+100）、版本一致性、git diff --check PASS；真机验收仍需用户执行，F1-V1 保持 `OPEN · ACCEPTANCE FAILED · REWORK`�?
- 遗留：执�?V1 联合真机重验；若视觉仍失败，只针对实际失败项追加修复，不提前宣告 F1 CLOSED�?
- Hash�?c8dfdd�?

## v0.2.25.13-rz
## v0.2.25.13-rz
MAP-A-R3-D2-F1-V1-REWORK-B1 Region 世界锚点统一�?026-08-10 13:37:23）：删除 Vector Overlay Stroke �?`BaseHeightMeters + 0.03` 世界坐标偏移，使 Fill、Stroke、Marker 对同一 MapPoint 共享完全相同的世界锚点；新增世界坐标合同测试，B2 Vulkan Depth Policy 不在本轮�?
- 验证：解决方�?Build 0W0E；Core 361/361、World 1110/1110、WarCore 22/22、B1 专项 1/1、ARCH-A（含 5+100）、版本一致性、git diff --check PASS；真机验收仍需用户执行，F1-V1 保持 `OPEN · ACCEPTANCE FAILED · REWORK`�?
- 遗留：B1 完成后才进入 B2 Vector Overlay Depth Policy；不得用世界 Z 偏移实现视觉层级�?
- Hash：ef12f4b�?

## v0.2.25.12-rz
## v0.2.25.12-rz
MAP-A-R3-D2-F1 Metric/Picking 精度门禁�?026-08-10 12:20:03）：将地�?Screen �?Pick �?World �?Screen CPU 路径改为基于 CameraState/ViewportState 的双精度投影与射线构造，消除 10,000�?0,000,000m 场景中的单精�?W=0 与超�?1 DIP 的往返误差；补充 100m�?0km�?0,000km、多 DPI、正�?45°/80°斜视自动回归�?
- 验证：解决方�?Build 0W0E；Core 361/361、World 1109/1109、WarCore 22/22、Metric/Picking 108/108、ARCH-A（含 5+100）、版本一致性、git diff --check PASS；真机验收仍需用户执行，F1-V1 保持 `OPEN · ACCEPTANCE FAILED · REWORK`�?
- 遗留：进�?V1-REWORK-B Region Overlay；完成后执行 V1 真机重验，未验收不得宣告 F1 CLOSED�?
- Hash�?594c4c�?

## v0.2.25.11-rz
MAP-A-R3-D2-F1-V3 Scale Indicator + MapEditorZoomPolicy�?026-08-10 12:07:40）：新增视口左下角比例尺�?2�?6 DIP 内边距，80�?60 DIP 目标宽度�?/2/5 m/km 格式）；新增仅地图编辑器生效�?Perspective/Orthographic Zoom Floor�?00m 网格最大视觉尺寸为 160 DIP（`0.625 m/DIP`）；通用 Camera 的近距离能力保持不变�?
- 验证：解决方�?Build 0W0E；Core 361/361、World 1001/1001、WarCore 22/22 PASS；ARCH-A（含 5+100）、版本一致性、git diff --check PASS；真机验收仍需用户执行，F1-V1 保持 `OPEN · ACCEPTANCE FAILED · REWORK`�?
- 遗留：进�?Metric/Picking `Screen �?Pick �?World �?Screen` 往返门禁；Region Overlay 视觉回修暂缓�?
- Hash�?a6c6c2�?

## v0.2.25.10-rz
MAP-A-R3-D2-F1-V2 100m Minimum Visible Metric Grid�?026-08-10 11:56:08）：保留 `1 / 2 / 5 × 10ⁿ` 算法，将地图编辑器最小可见网格固定为 100m、动态覆盖扩展到 10,000km；将目标视觉尺度改为 48 DIP；抽出不依赖后端�?`ViewportMetricScale`，网格统一消费 `MetersPerDip`，不改变通用 Camera 或连�?double 世界坐标�?
- 验证：解决方�?Build 0W0E；Core 357/357、World 999/999、WarCore 22/22 PASS；ARCH-A（含 5+100）、版本一致性、git diff --check PASS；真机验收仍需用户执行，F1-V1 保持 `OPEN · ACCEPTANCE FAILED · REWORK`�?
- 遗留：继续进�?F1-V3 Scale Indicator + MapEditorZoomPolicy；Region Overlay 视觉回修暂缓�?
- Hash：a12d36e�?

## v0.2.25.9-fix
MAP-A-R3-D2-F1 V1-REWORK-A Navigation Gizmo 输入恢复�?026-08-10 11:48:28）：冻结 MC-01～MC-06 空间基础合同与新�?Gizmo→Metric→Scale/Zoom→Picking→Region 依赖顺序；修�?R3 backlog �?F1 状态�?00m 网格目标�?V1-T13 推送状态；修复 Region Tool 激活时 Native LeftDown 先消�?Region、以�?Gizmo 会话 Move �?Region Preview 抢路的问题；HostDetach、CaptureLost、CancelMode、KillFocus 统一清理 Gizmo 会话�?
- 验证：解决方�?Build 0W0E；Core 349/349、World 999/999、WarCore 22/22 PASS；ARCH-A（含 5+100）、版本一致性、git diff --check PASS；真机验收仍需用户执行，V1 保持 `OPEN · ACCEPTANCE FAILED · REWORK`�?
- 遗留：继续进�?F1-V2 100m Minimum Visible Metric Grid；Region Overlay 视觉回修暂缓�?
- Hash：d621755�?

## v0.2.25.8-fix
MAP-A-R3-D2-F1-V1 Region Vector Overlay�?026-08-10 10:51:22）：记录 C2 真机闭环正式 CLOSED；将 Region/Draft �?StaticModel 临时路径迁移到独�?Vector Overlay 数据合同�?Vulkan 屏幕空间 Stroke/Marker Pass；新增凹多边�?Ear Clipping、Draft/Region V1 回归与无 StaticModel 路径验证�?
- 真机：RF-M01 PASS；RF-M02-A PASS；RF-M02-B 转交 F1-V；RF-M03 PASS（导航结束后 Draft Preview 自动恢复，无需重选工具、无输入丢失、无崩溃）�?
- 验证：解决方�?Build 0W0E；Core 349/349、World 998/998、WarCore 22/22 PASS；F1-V1/RegionDrawing 专项 31/31 PASS；ARCH-A�?+100、版本一致性、git diff --check PASS�?
- 状态：MAP-A-R3-D2-F1-C2 CLOSED；F1-V1 OPEN；F1-V2 BLOCKED BY V1；F1-V3 BLOCKED BY V2；A03～A06 BLOCKED；D3 禁止启动；F2 未创建�?
- 遗留：F1-V1 真机 V1-M01～M05 验收；通过后才解锁 F1-V2�?
- Hash�?f58d60�?

## v0.2.25.7-fix
MAP-A-R3-D2-F1-C2 REWORK Native 相机路由�?Draft 往返（2026-08-10 10:00:13）：修复 Native `Move` 分支遮蔽 Middle Move 的不可达条件；抽出共�?`NativePointerRoutePolicy`，让相机预览优先�?Draft Preview；强�?C2-R03/R09 �?Draft Anchor 可见性与三次往返回归，并新�?Native Route Policy 测试�?
- 验证：C2/F1-C/Native Route 专项 19/19 PASS；解决方�?Build 0W0E；Core 348/348、World 991/991、WarCore 22/22 PASS；ARCH-A�?+100、版本一致性、git diff --check 待提交后最终复跑�?
- 状态：MAP-A-R3-D2-F1-C2 继续 REWORK；C2-M02-A Draft Framing PASS，C2-M02-B BLOCKED BY F1-V，C2-M01/C2-M03/C2-M04 等待 Native 路由真机重测；F1-V 暂缓，A03～A06 BLOCKED，D3 禁止启动，F2 未创建�?
- Hash�?a12f00�?

## v0.2.25.6-fix
MAP-A-R3-D2-F1-C2 地图相机语义�?026-08-10 09:38:15）：地图编辑器模式下“查看全部”按 MapBounds 构图；“聚焦”按 Draft AABB �?Selected Entity �?相机不变执行；新增正交地图取景、Draft 最小可视半径与右侧地图模式状态；补充 C2-R01～C2-R09 自动回归�?
- 验证：C2 专项 9/9 PASS；解决方�?Build 0W0E；Core 348/348、World 985/985、WarCore 22/22 PASS；ARCH-A�?+100、版本一致性、git diff --check PASS�?
- 状态：MAP-A-R3-D2-F1 继续 OPEN；F1-C2 等待用户执行 C2-M01～C2-M04 真机确认；F1-V 未开始；A03～A06 BLOCKED，D3 禁止启动，F2 未创建�?
- Hash�?c23ea7�?

## v0.2.25.5-fix
MAP-A-R3-D2-F1-C 稳定性与日志收口�?026-08-10 09:09:01）：聚焦在区域草稿或无选中实体时保持相机不变；新增 TryProjectWorldPoint 并让 Region PointerMoved 对投影失败安全降级；移除 F1TRACE 运行时取证、临时文件和高频 PointerMoved/Ray/Mouse/Render 日志；补�?C-R01～C-R06 回归测试�?
- 验证：解决方�?Build 0W0E；Core 348/348、World 976/976、WarCore 22/22 PASS；ARCH-A�?+100、git diff --check PASS�?
- 状态：MAP-A-R3-D2-F1 继续 OPEN；S02 修复确认 PASS；F1-C 等待用户真机确认 Focus 与日志；F1-V 未开始；A03～A06 BLOCKED，D3 禁止启动，F2 未创建�?
- Hash�?739c31�?

## v0.2.25.4-fix
MAP-A-R3-D2-F1 根因收口�?026-08-10）：修复首点草稿生成零长�?primitive、区�?world-space 绘制误用零缩放，以及拾取未遵守地图中心原点边界合同；新增 MapPoint↔World 直接映射合同与首点资�?Vulkan 合法性回归�?
- 验证：解决方�?Build 0W0E；Core 346/346、World 971/971、WarCore 22/22 PASS；R17～R20 聚焦测试 5/5 PASS；ARCH-A�?+100、git diff --check PASS�?
- 状态：MAP-A-R3-D2-F1 继续 OPEN；既�?S01/S02 失败证据已确认，不重复要求用户证明旧失败；修复后的真机确认仍待用户执行；A03～A06 BLOCKED，D3 禁止启动，F2 未创建�?
- Hash�?4280d7�?

## v0.2.25.3-rz
MAP-A-R3-F1-FINAL 原生输入与区域绘制返工（2026-08-09）：修复区域工具在释放鼠标后不更新预览的问题；修复正式区域与 Draft 资源 primitive 范围为空/固定颜色导致不可见的问题；增�?App/UI 版本溯源�?A-E 阶段运行时取证，取证同时写入底部日志与临时文件；F2 保持冻结。验证：App 构建 0 Warning / 0 Error，World.Tests PASS，ARCH-A PASS，git diff --check PASS。Hash�?dffd70。遗留：真机验收仍由用户执行，未宣布 CLOSED�?

## 2026-08（当前自然月�?
## v0.2.25.3-rz
MAP-A-R3-F1 真机失败返工�?026-08-09 22:52:30）：截图显示点击后既无命中反馈也�?Draft 顶点；审计发�?VulkanViewport 初始化提示层未禁止命中测试，存在遮断 VulkanNativeHost 输入链的风险。修�?FallbackLayer `IsHitTestVisible=False`，并为区域绘制拾取未命中增加状态栏证据；World.Tests 968/968 PASS，解决方案复�?UI DLL 阶段因运行中�?XuanYu.Editor.App 锁文件未完成。提�?`fb5af08`。F2 未启动，等待重新真机验收�?
### v0.2.25.1-rz
MAP-A-R3-D1：既�?Region 合同审计与加固（2026-08-09 20:35:37�?
- 变化：复�?R2 �?MapRegion/MapRegionDraft/MapPoint/Region Layer；新增非相邻边相交、接触、重叠拒绝；新增 MapEditSession Region Create/Delete 正式提交入口，保持单历史条目与相�?RegionId �?Undo/Redo；同�?R3 backlog、file-tree 与四处版本号�?
- 验证：解决方�?Build 0 Warning / 0 Error；Core 344/344；World 943/943；WarCore 22/22；ARCH-A�?+100、git diff --check PASS�?
- Hash：dcb4b91�?
- 遗留：D1 不包含绘�?UI、Picking 接入、Renderer、LayerPanel、Inspector、持久化、GIS、DGD、Hole、MultiPolygon �?Polygon Boolean；D1 完成后停止，D2 另行批准�?

### v0.2.25.3-rz
MAP-A-R3-D2-F1：Region Tool Integration & Selected-State Regression�?026-08-09，返工后待真机复验）�?
- D2 真机裁定：A01 FAIL；A02～A06 BLOCKED / NOT EXECUTED；L4 FAIL；D2 OPEN；D3 禁止启动�?
- 修复：Region Drawing �?Top/App-level 入口移入 Map Editor 的“地图工具”区；补�?Normal/Hover/Selected/Selected+Hover 深色 Foreground；新增真�?Headless Runtime RED→GREEN 与静态归属契约�?
- 验证：F1 真实工具�?Runtime/静态测�?5/5 PASS；Solution Build 0W0E；Core 345/345；World 952/952；WarCore 22/22；ARCH-A�?+100、diff-check PASS�?
- 遗留：当前仅 READY FOR USER ACCEPTANCE，先执行 S01/S02（即 D2-A01a/A01b）；通过后才恢复 A02～A06。无 F2 轮次�?
- 本轮实际落地�?026-08-09 21:50:48）：顶部第二行通用工具栏移除“区域绘制”；真实右侧“地图编辑器→地图→地图工具”挂载区域绘制控件；运行时测试改为验证真�?Right/MapEditorPanel 祖先链与选中态深色文字。Picking、Draft、Preview、Renderer、Region 未修改�?
- F1-A-UI03 返工�?026-08-09 22:01:32）：确认原实现仅约束 ToggleButton 父控件，子级文本未被四态状态选择器锁定；新增 `mapToolLabel` 四态深色文字约束，并冻结浅�?Normal/Hover/Selected/Selected+Hover 背景与边框。运行时验证 Normal/Selected 最终文本颜色均�?`#243744`，静态覆盖合同补齐四�?selector；Picking、Draft、Preview、Renderer、Region 未修改�?
- UI 可读性与布局返工�?026-08-09 22:10:04）：顶部通用工具栏选中态内部文字统一锁定 `Color.Text.Primary`，避免浅色选中背景上的白字；区域绘制按钮显式水�?垂直居中。Runtime Focus 7/7；Solution Build 0W0E；Core 345/345、World 952/952、WarCore 22/22；ARCH-A、diff-check PASS�?
- MAP-A-R3-F1-B�?026-08-09 22:21:04）：复用现有 `VulkanNativeHost �?UiVm �?MapSurfacePicker �?ViewProjection/WorldRayFactory` 链路；区域绘制命中只记录真实 `MapPoint` 并反馈底部状态，不创�?Draft、不提交 Region。B01～B07 与既�?Runtime 聚焦测试通过；F1-C 未启动�?
- MAP-A-R3-F1 收口�?026-08-09 22:36:56）：真实命中恢复 Draft 首点、连续顶点、光标预览边、Enter 闭合提交、Esc 取消与正�?Region 快照渲染；补�?DPI 1.75 逻辑坐标回归与完�?Runtime 合同�?8e3728 保留为失败中间提交；本提交完�?F1 代码收口，等待用户完整真机验收，F2 未启动�?
- F1 收口补充�?026-08-09）：新增 Resize Runtime 回归 R16；最�?World 968/968，Core 345/345，WarCore 22/22，Build 0W0E，ARCH-A �?diff-check PASS�?
## v0.2.25.2-rz
MAP-A-R3-D2：Region Drawing 实装与真机验收前收口�?026-08-09）�?
- 变化：新增区域绘制工具入口；复用既有相机投影完成地图表面拾取；左键添加顶点、移动预览边、首点闭合候选、Esc 取消；闭合调�?`MapEditSession.CreateRegion`，正式区域与临时草稿进入现有静态模型渲染路径�?
- 验证：解决方�?Build 0 Warning / 0 Error；Core 345/345；World 947/947；WarCore 22/22；ARCH-A PASS�?+100 PASS；`git diff --check` PASS。L1 静�?UI PASS，L2 Headless PASS，L3 Visual Regression NOT ENABLED，L4 真机验收 PENDING�?
- Hash：以本轮最终提交为准�?
- 遗留：等待用户执�?D2-A01..D2-A06 真机 IPO 验收；未进入 D3�?
## v0.2.24.50-fix
MAP-A-R2-D5-F5：LayerPanel 根因收口�?Runtime UI Gate 首次落地�?026-08-09 19:42:41�?
- 变化：新�?Avalonia.Headless 12.0.4（仅 World.Tests）；建立可复�?Headless Fixture/Host；LayerPanel 改为 Auto/Auto/* Grid，修复冷启动与增层宽度稳定性；�?Layer/Top/Foot 状态覆盖收口到模板 Presenter 与项�?Token；新�?7 �?Runtime UI 门禁�?
- 验证：解决方�?Build 0 Warning / 0 Error；Core 344/344、World 938/938、WarCore 22/22；Runtime UI 7/7；Visual Regression：NOT ENABLED；F5 真机验收 8/8 PASS；UI D6 CLOSED�?
- Hash�?0fd339。遗留：R2 未完成需求已转入 `docs/milestones/current/MAP-A/R3-backlog.md`；状�?MAP-A-R2：CLOSED，下一阶段 MAP-A-R3�?

## v0.2.24.49-fix
MAP-A-R2-D5-F4：图层列表测量与拖拽热区修复�?026-08-09 16:18:16�?
- 根因：图层页外层 ScrollViewer 未禁止横向无限测量，Inspector 隐藏时名�?`*` 列失去可用宽度；拖拽事件直接绑定 14 DIP Path，命中区过小�?
- 修复：图层页禁用横向滚动并保持页面、面板、ListBox 横向拉伸；拖拽改�?24×28 DIP 透明 Border 作为实际 Pointer 热区，图标仅作视觉子元素�?
- 验证：解决方�?Build 0W0E；Core 344/344、World 931/931、WarCore 22/22；架构守�?PASS；`git diff --check` PASS。真机验收待用户执行�?
- 范围：未修改 Gizmo、世界原�?Overlay、Vulkan 日志策略、Inspector 其他页、顶栏、地图渲染、Schema、宪法�?
- 状态：**MAP-A-R2-D5-F4：READY FOR USER ACCEPTANCE**；不得视�?CLOSED，等待用户执�?F4-01～F4-07 真机验收�?

## v0.2.24.48-fix
MAP-A-R2-D5-F3：图层行与手柄拖拽修复（2026-08-09 15:58:47�?
- F2 真机裁定：`MAP-A-R2-D5-F2` �?FAIL；Gizmo、世界原�?Overlay、日志降噪通过，图层拖拽与图层视觉阻塞，保持未 CLOSED�?
- 根因：图层网格未把名称作为可扩展主体；`TargetAt` 以未转换的条目边界命�?`LayerList` 坐标，拖拽经过条目时目标索引可能为空；上一轮捕获状态也未记录源索引或解绑事件�?
- 修复：图层行固定为「手�?/ 类型 / 名称 / 可见 / 锁定」，名称�?`*` 主体列，ListBoxItem 拉伸到面板宽度，选中态继续使用浅背景，排序提示降级为次要帮助文字；拖拽只从手柄启动，记录 source index，按转换后的条目中心计算 target index，释放时一次提交并完整清理捕获/事件状态�?
- 测试：F3 聚焦合同与拖�?领域回归 **10/10 PASS**；UI F3/D6 聚焦合同 **19/19 PASS**；完整解决方�?Build **0W0E**；Core **344/344**、World **931/931**、WarCore **22/22** PASS；架构守�?PASS；`git diff --check` PASS�?
- 范围：未修改 Navigation Gizmo、世界原�?Overlay、Vulkan 日志策略、Schema、依赖或宪法；新�?`UiF3LayerRowContractTests`，同�?`file-tree.md`�?
- 状态：**MAP-A-R2-D5-F3：READY FOR USER ACCEPTANCE**；不得视�?CLOSED，等待用户执�?F3-01～F3-06 真机验收�?

## v0.2.24.47-fix
MAP-A-R2-D5-F2：Navigation Gizmo DIP、局部图层拖拽与日志降噪重做�?026-08-06，Commit 本轮落库为准�?
- T1：普�?revert 撤销 F1，保留世界原�?Overlay、焦点框修复�?PowerShell 守卫兼容；恢复提�?`ecb9134` 已推送�?
- T2：Navigation Gizmo 使用独立 `gizmoParams.w=RenderScaling`，Shader �?DIP 空间计算，物�?viewport/scissor 不变；CPU/Shader 不重复缩放�?
- T3：移除全局 DragDrop，改为六点手柄局�?Pointer Capture；不替换 ItemsSource、不移除源行、不禁用窗口；NoRebuild、周期选择投影与命令缓冲摘要不再刷屏�?
- 状态：MAP-A-R2-D5-F2：READY FOR USER ACCEPTANCE；D6 顺延�?v0.2.24.48-rz，未创建 Tag/Release�?

MAP-A-R2-D5：焦点框作用域与世界原点 Overlay 修复�?026-08-06，Commit 本轮落库为准�?
- **焦点�?*：视口原生宿主设为不可聚焦并清除 FocusAdorner；布局分隔器不显示焦点装饰；Button 保留正式 `2 DIP` 焦点框与 `1 DIP` 外偏移合同�?
- **世界原点**：DrawPlan 调整为实�?轮廓之后绘制；WorldOrigin 管线关闭深度测试并保持关闭深度写入；片元 Shader 不再写入深度，中心标记保持屏幕恒定尺寸，模型或地面不再遮挡�?
- **测试**：新增原�?Overlay 深度�?Shader 合同；Core **340/340**、World **928/928**、WarCore **22/22** 全部通过；完整解决方�?Build **0W0E**；`git diff --check` PASS�?
- **架构守卫**：仅�?`scripts/arch-a-guard.ps1` �?UTF-8 BOM 以兼�?Windows PowerShell 5.1；解码后脚本文本、命令与逻辑不变，随后守�?EXIT=0�?+100 PASS�?
- **治理**：版�?v0.2.24.44-rz �?**v0.2.24.45-fix**（四处同步）；无 Schema/依赖/Tag/Release 变更；file-tree 无结构变化，无需更新�?
- **状�?*�?*MAP-A-R2-D5：READY FOR USER ACCEPTANCE**；等待用户真机复验焦点框四边�?00%�?00% DPI 与世界原点遮挡场景�?

## v0.2.24.44-rz
ARCH-UI-SPEC-R1-D6：DPI、键�?可访问性、减少动画、日志性能与剩�?UI 债务复核�?026-08-06，Commit 本轮落库为准�?
- **DPI/缩放合同**：新�?`UiDpiContract`，冻�?100%/125%/150%/175%/200% 桌面缩放清单；主窗口最�?推荐 DIP 尺寸与检查器/地图表单宽度阈值保�?DIP 口径，不做物理像素补偿�?
- **键盘与可访问�?*：新�?`UiAutomationNamer` 与窗口打开后的自动补名；地图页、地图表单、图层工具、日志筛�?回到底部�?D4/D5 新增交互控件显式声明 `AutomationProperties.Name`；自动补名拒绝导�?ARCH/D6 等内部治理代号�?
- **减少动画合同**：新�?`UiMotionPreference`/`UiMotionContract`，Reduce 模式下非必要 Hover/Dialog 动效时长归零；Default 模式保持 80/120/180ms 短反馈；未新�?Token�?
- **日志与通知性能**：`EditorLogBuffer.MaxEntries` 升为公开合同常量并保�?500 条尾窗；D6 测试覆盖日志上限与相邻重复项压缩；通知自动消失策略独立�?`UiVm.NotificationLifetime`�?
- **范围收口**：未接入地图持久化；未新增业务功能；未新�?Token；未触碰 Render/Vulkan/Shader/Gizmo/WarCore/AI 宪法/本地技能。D5-DEFER-01 仍归未来独立地图持久化专项�?
- 验证：全解决方案 `--no-incremental` 串行 Build **0W0E**；Core **339/339**、World **928/928**、WarCore **22/22**，合�?**1289/1289 PASS**；启动冒�?PASS（`XuanYu.Editor.App.exe` 存活 8 秒）；arch-a-guard PASS；git diff --check PASS�?
- 治理：版�?v0.2.24.43-rz �?**v0.2.24.44-rz**（四处同步）；file-tree 登记新增 D6 文件；未创建 Tag/Release�?
- 状态：**ARCH-UI-SPEC-R1-D6：READY FOR USER ACCEPTANCE**（自动化通过不等�?CLOSED；等待用户按 D6-A1 真机验收）�?
- 保留：D5-DEFER-01 地图「保存并新建」仍等待未来地图持久化专项；D6-A1 真机验收未执行�?

## v0.2.24.43-rz
ARCH-UI-SPEC-R1-D5：控件状态、表单、弹窗、通知、空状态与日志治理�?026-08-06，Commit 本轮落库为准�?
- **按钮治理**（D5-FIX-01 统一处理）：`Design/UiStyles.D5.axaml` 新增——Button 内容水平/垂直居中（HorizontalContentAlignment/VerticalContentAlignment=Center，禁止逐按�?Margin 偏移修补）；完整状�?Normal/Hover（Color.Hover.Bg+Border.Strong�?Pressed（边�?Accent�?Focused（Color.Focus 边框 1px，不跳动�?Disabled（Text.Disabled+Bg.Control）；危险按钮 `Button.uiDanger`（Color.Danger �?+ 白字）；全局 Button 色迁移正�?Token（Bg.Panel/Border.Default/Text.Primary），基线 -5�?
- **表单状�?*：TextBox 完整状态样式（Normal/Hover/Focus/Disabled，全�?Token�? `TextBox.error`/`TextBox.warning` 边框类；地图属性表�?6 个输入框绑定 `IsMapFormError`�?*错误反馈非仅颜色**（错误图�?ErrorIcon + 说明文字 + 输入框红色边框三重表达，规范 §11.2）；提交反馈通知（应用地图属性成功→Success 通知/失败→Error 通知 + 表单错误区）；表单错误行在无错误时隐藏�?
- **弹窗系统**（新）：UiWin 内置 DialogHost（遮�?+ 卡片，XAML 模板化）；`ShowMessage`/`ShowConfirm`/`ShowDanger` 三形态；**危险弹窗默认按钮=取消（非危险），Enter 触发默认按钮、Escape 取消**；未保存确认重构�?DialogHost（保�?默认/不保�?危险/取消=Escape），删除代码构建 Window（基�?-11）；**新建地图**走危险确认（替换地图属�?清空历史，不可撤销）；**删除图层**走危险确认（UiVm `DangerousCommandConfirmRequested` 事件 + `ConfirmDangerousCommand`，未注入处理器时保持直接执行兼容既有测试）�?
- **通知系统**（新）：UiVm 四级通知状态机（`UiNotificationLevel` Info/Success/Warning/Error；`NotifyInfo/Success/Warning/Error`）；**不刷�?*——只保留最新一条（序列号递增）；技术详情由调用方写入既有日志系统；Footer 通知条（级别图标 + 单行省略 + 完整 Tooltip，四级色 Token）；真实触发点：地图属性应用成�?失败、日志复制（既有）�?
- **空状�?*：日志空状态区分两类——初�?无数据（「暂无日志」）�?*筛选无结果**（「没有匹配的日志�?「清空筛选」入口，`ShowNoFilterResults`）；检查器空状态保持（D4）�?
- **日志治理**：自动跟随与用户上滚互不冲突（既�?`LogAutoScrollPolicy` 保留）；**「回到底部」按�?*（用户离开底部时右下角显示，点击恢复跟随并隐藏——控制器新增 `TailStateChanged` 事件）；级别视觉/筛�?行高/列宽保持（D4/D3 验收基础）；搜索框占位保持（不扩张）�?
- **Token 迁移**：Manifest 保持 **112 Frozen / 0 PendingReview**（未新增 Token——按�?表单状态色全部映射现有 Token：Color.Hover.Bg/Focus/Danger/Bg.Control/Bg.Panel 等）�?*债务基线 159 �?143�?16�?*：Ui.axaml Button 状态色×5 + UiWin.UnsavedDialog 代码 Window 颜色×11（真实代码迁移）；未�?Locator/AllowedCount 掩盖�?
- 验证：全解决方案 `--no-incremental` 串行 Build **0W0E**（落�?/tmp/d5-final-build.log）；Core 339/339、World 852/852�?29 D5 测试）、WarCore 22/22，合�?**1213/1213 PASS**；启动冒�?PASS；arch-a-guard PASS（版本一致性检查有效）；git diff --check PASS�?
- 治理：版�?v0.2.24.42-fix �?**v0.2.24.43-rz**（四处同步）；未创建 Tag/Release�?
- **审查纠偏（REVIEW BLOCKED �?修复，同版本 v0.2.24.43-rz 不升版，2026-08-06�?*�?
  - **危险操作 fail-closed（硬阻塞修复�?*：删除图层等危险操作——确认处理器缺失�?*阻止执行并记录错�?*（不再为测试兼容绕过安全流程）；只有用户明确确认（`ConfirmDangerousCommand`）才执行；取消（`CancelDangerousCommand`）不执行；既有测试改为显式注入批准确认服务�?
  - **新建地图未保存流程（硬阻塞修复）**：无未保存修�?�?直接新建（不弹窗）；有修�?�?**保存并新�?/ 不保存并新建 / 取消**（`HasUnsavedMapChanges` 以表单与地图值一致性判定，�?IsDirty）；危险按钮�?*具体动作**（「不保存并新建」「删除图层」），不以「继续」代替�?
  - **字段级校验（硬阻塞修复）**：`MapWidthError/MapDepthError/MapBaseHeightError` 每输入框只绑定自身错误（不再统一全局染红）；**ValidateOnInput**（输入即清除�? **ValidateOnLostFocus**（失焦单字段校验�? **ValidateOnSubmit**（提交全校验）；`FirstInvalidField` 提交后自动聚焦第一处错误；`FormErrorSummary` 页面汇总；校验失败不清空输入�?
  - **日志空态互斥（硬阻塞修复）**：`ShowInitialLogEmpty`（「全部」筛选且无日志）�?`ShowNoFilterResults`（非全部且无结果）严格互斥；筛选空态提供「清空筛选」入口�?
  - **焦点合同**：官�?**FocusAdorner** 焦点环（2 DIP 焦点�?+ 1 DIP 外偏移，`Button:focus-visible` 模板化，不占布局/不改变控件尺�?不裁切；Setter 值用 `<Template>` 包裹避免运行�?Setter-Control 异常）；Hover/Pressed/Focused 形态互不相同；弹窗 **Tab/Shift+Tab 焦点陷阱**（`DialogFocusTrap` 纯逻辑可测）与**关闭后焦点返回原控件**�?
  - **通知合并/关闭/优先�?*：同类同文案合并计数（「保存成�?×5」，`NotificationCount/ShowNotificationCount`）；`DismissNotification` 可关闭；**优先�?Error(3) > Warning(2) > Success(1) > Info(0)**——高优先级不被低优先级覆盖；`CreatedAt` 生命周期（自动消失策略归 D6）�?
  - **加载/失败/重试**：打开场景失败弹窗改为 **ShowRetryAsync**（重�?取消，重试重新加载同一路径，循环安全）；错�?警告弹窗宿主化到 DialogHost（ErrorIcon/WarningIcon 图标，非仅颜色）；无真实加载流程的场景（地图持久�?D6 接入）在报告中登记为 D6 触发项�?
  - **日志视觉 Token �?*：Foot.axaml 全部原始色迁移正�?Token（logFilter/logHead/logMono/logList 选中/FooterMode/搜索图标/日志边框底色/RepeatText→Log.RepeatText），基线 -12；UiWin.Dialogs 代码 Window 颜色全部清除（宿主化），基线 -9�?
  - **5+100 拆分**：UiVm.Logging 按职责拆（State/Refresh）、UiVm.MapEditor.Validation 独立、UiWin.DialogHost.Danger 独立、DialogFocusTrap 独立；恢复多行书写（无单行压缩逃避）；Foot 日志�?99 行、SceneCommands 97 行�?
  - **Inter 零残�?*：删�?`.WithInterFont()`（Avalonia.Fonts.Inter）；FontFamily 冻结链修正为 `Microsoft YaHei UI, Segoe UI, Noto Sans CJK SC`（D1 规范，禁�?Inter）�?
  - 验证：新�?**UiD5CorrectionBehaviorTests�? 项）+ UiD5CorrectionNotifyTests�? 项）+ UiD5CorrectionStructureTests�? 项）**，合�?**19 项纠偏测�?*；World **871/871**�?52+19）；全量 `--no-incremental` **0W0E**（落�?/tmp/d5-fix-final-build3.log）；**Core 339 + World 871 + WarCore 22 = 1232/1232 PASS**；启动冒�?PASS（含 FocusAdorner 模板化运行时验证——首次直跑捕�?Setter-Control 崩溃并已修复）；arch-a-guard PASS；git diff --check PASS�?
  - 债务基线 **143 �?122�?21�?*；Manifest 112 Frozen / 0 Pending（未新增 Token；FocusAdorner 基于 Color.Focus 派生）�?
- **二次审查纠偏（REVIEW BLOCKED �?修复，同版本 v0.2.24.43-rz 不升版，2026-08-06，按用户方案逐字执行�?*�?
  - **硬阻塞一：未保存地图判断（按用户方案�?*——`HasPendingMapFormChanges`（表单值与当前模型不一致）+ `HasUnsavedMapChanges = MapSession.IsDirty || HasPendingMapFormChanges`（图�?显隐/锁定/已应用未落盘全部捕获）；**默认地图基线修正**：MapEditSession 新增 `MarkBaseline()`（内存基线保存点，不动路径），IsDirty 改为保存点判定（`SavedStateId is null || CurrentStateId != SavedStateId`）——初始未修改的默认地图不误判为未保存（新建不弹窗）；新建（CreateNewMap 清空保存点）与任何修改仍为未保存�?
  - **「保存并新建」停止上�?*：地图持久化（真实保存到资产文件）尚未接入，归未来独立地图持久化专项—�?*不使用「保存并新建」文案，禁止用「应用属性」冒充保�?*；未保存弹窗为「不保存并新建」（危险/具体动作，明确丢弃）/「取消」（默认焦点）；�?discard 放行新建，取�?�?原地图与全部修改保持不变；未来专项接入真实保存后恢复三选（登记）�?
  - **硬阻塞二：输入阶段真实校验（按用户方案）**——`ValidateMapFieldOnInput` 替换「只清错」逻辑：输入阶段执行轻量规则（非法字符/NaN/Infinity/**明显超界** 100~1000000 米，边界与领�?MapDefinitionValidator 一致）�?*值仍非法时错误不得消�?*（继续输入非法值错误保持）；合法值错误立即清除；**输入中�?*（空/-/./1./1e- 等临时文本）不清除已有错误；失焦执行完整单字段校验（格式+范围）；提交执行全部字段+跨字段（MapSession 业务兜底�? 定位第一处错�?+ 页面汇总；校验失败不清空输入；基础高度仅有限数字（领域 ValidateSurface 无范围）�?
  - 验证：新�?**UiD5InputValidationTests�? 项）+ UiD5UnsavedFlowTests�? 项）**，World **887/887**�?71+16）；全量 `--no-incremental` **0W0E**（落�?/tmp/d5-fix2-final-build.log）；**Core 339 + World 887 + WarCore 22 = 1248/1248 PASS**；启动冒�?PASS；arch-a-guard PASS；git diff --check PASS；既有测试适配 4 处（默认地图状态「已保存」、范围错误字段级拦截、新建地图流程断言、MapStatusText 基线语义）�?
  - 债务基线 122 保持；Manifest 112 Frozen / 0 Pending�?
- **D5-FINAL 最终语义纠偏（同版�?v0.2.24.43-rz 不升版，2026-08-06�?*�?
  - **地图状态四�?*：无路径+无修�?�?`未落盘`；无路径+有修�?�?`未保存`；有路径+无修�?�?`已保存`；有路径+有修�?�?`有未保存修改`（内存基�?�?已保存到磁盘；`MarkBaseline()` 保留且不改变文件路径）。刷新点全覆盖：表单输入（三�?setter 刷新 MapStatusText）、应用属�?图层增删排序显隐锁定/Undo/Redo/新建（`DirtyChanged` 订阅 `OnMapDirtyChanged`）、打开场景加载地图后同步表单文本（`SyncPropertyTexts`，避免误判待提交修改）�?
  - **弹窗去内部编�?*：正式文案「当前地图有未保存的修改。当前版本暂不支持保存地图后新建。请选择取消，或不保存并新建。」（�?D5/D6/MAP-A/ARCH-UI-SPEC-R1）；按钮严格「取�?/ 不保存并新建」；默认焦点=取消；Enter 只触发默认按钮（取消）；Esc/关闭=取消；仅明确点击「不保存并新建」才允许丢弃修改；确认服务缺�?异常时不新建（`return choice == "discard"` 天然 fail-closed）；不调用场景保存、不调用不存在的地图保存、不用「应用地图属性」冒充保存�?
  - **延期登记 D5-DEFER-01**：地图「保存并新建」暂缓——归属未来独立的地图持久化专项（**不归�?D6**）；专项必须补：保存成功后才新建/失败不新�?取消路径不新�?防重复提�?成功后更新路径与状�?写盘失败保留地图图层历史�?
  - 验证：新�?**UiD5MapStatusTests�? 项）+ UiD5UnsavedDialogTests�? 项）+ UiD5UnsavedDialogBehaviorTests�? 项）**，共 25 项覆盖计�?26 个断言点；World **912/912**�?87+25）；全量 `--no-incremental` **0W0E**（落�?/tmp/d5-final-build.log）；**Core 339 + World 912 + WarCore 22 = 1273/1273 PASS**；启动冒�?PASS；arch-a-guard PASS；git diff --check PASS；D3/D4/D5 回归 **156/156 PASS**�?
  - 债务基线 122 保持；Manifest 112 Frozen / 0 Pending�?
- **D5-A1 真机验收收口（同版本不升版，2026-08-06 用户正式裁决�?*：ARCH-UI-SPEC-R1-D5-A1：PASS；D5 指令�?22~30 全部通过；按钮居中、控件状态、表单字段校验、危险弹窗、通知、日志跟随、日志空态、地图状态四态与未保存新建流程均通过真机复验。D5 �?**COMPLETE**�?
- **D5-DEFER-01 保留**：地图「保存并新建」暂缓，归属未来独立地图持久化专项；不归�?D6，不允许以「应用地图属性」冒充保存�?
- 状态：**ARCH-UI-SPEC-R1-D5：COMPLETE**；当前阶段进�?**ARCH-UI-SPEC-R1-D6**（DPI、键�?可访问性、减少动画、性能、剩�?UI 债务收口）；剩余轮次 D6 + A1�?
- 保留（D6 范围）：G02 焦点框系统与 DPI/减少动画/屏幕阅读器；日志搜索实现仍为占位；加�?进度长任务无真实流程，不虚构�?


## v0.2.24.42-fix
ARCH-UI-SPEC-R1-D4-F1：单行属性行、文本溢出与字体统一修复�?026-08-06，Commit 本轮落库为准�?
- **真机问题定位**（D4 真机验收未通过 �?D4-F1）：只读字段被整体竖排（<360 上下）、展示型长文本换行、字体不统一。D4-F1 将响应式拆为两类�?*只读键值行始终单行双列**�?*可编辑表单行（真实输入控件）才在 <360 整组上下**�?
- **规范修订**（`docs/ui/玄域引擎_UI规范_1.0.md` §7.1.1 新增 + §23 变更历史登记）：ReadonlyKeyValueRow（始终同一行；默认标签�?80；组件允�?72�?6：地图摘�?72/检查器 80/调试�?96；间�?8；值列 `*` 可收缩；NoWrap+CharacterEllipsis+MaxLines=1；Tooltip 完整值）；EditableFormRow（标�?96/128；仅真实输入控件 <360 整组上下；不得套用只读属性）；展示型动态文本默认（NoWrap+Ellipsis+MaxLines1+完整�?Tooltip）；显式多行例外清单（日志正�?错误详情/帮助说明/空状态说�?多行 TextBox/详情区域）�?*未修改任何冻�?Token 数�?*�?12 Frozen / 0 Pending 不变）�?
- **检查器**（InspectorPanel.axaml/.cs）：删除 WideFields/NarrowFields 双布局树（D4-F1 只读字段不再有窄模式）；单套水平 Grid（标�?80 + �?`*`，MinWidth 0�? uiLabel/uiValue 公共样式 + �?Tooltip 完整 Value；分组标题独占一行（uiSection）；空状态说明走 uiMultiline；`InspectorPanel.axaml.cs` 移除模式切换（无输入控件则无响应式）�?
- **调试�?*（Right.axaml + DebugText.cs + UiVm.Scene.cs + UiVm.InteractionPointer.cs + UiVm.cs）：当前选择/当前工具/拾取状�?日志策略/类型/对象 ID/选中来源/PointerId/起点/当前/位移/Preview次数 全部从拼接字符串结构化为 `InspectorFieldRow`（Label/Value）；三个 ListBox 改为 ItemsControl 键值行模板（Grid 96 �?+ uiLabel/uiValue + Tooltip 完整值）；交互事�?Grid（阶�?Owner/Preview）值加单行省略 + Tooltip；按钮文字统一 12�?
- **地图编辑�?*（MapPagePanel.axaml/.cs + MapEditorPanel.axaml）：资产摘要（名�?路径/MapId/尺寸/状态）始终单行双列�?2 列），不随宽度上下拆行；路径/名称/尺寸/状态加完整�?Tooltip；MapId 保持「前 8…后 6�? 完整 Tooltip + 完整复制；地图属性输入表单（宽度/深度/基础高度）标�?96 列、`<360` 整组上下（`EditableFormLayoutModel` 统一 360 阈值，替代�?320 紧凑模型——`MapEditorLayoutModel.cs` 删除）；MapEditError �?uiMultiline；环境占位页标题 uiSection�?
- **图层**（LayerPanel.axaml + LayerInspectorPanel.axaml）：图层名单行省略（NoWrap+Ellipsis+MaxLines1�? Tooltip 完整名称；图层属性（类型/顺序/图层 ID）单行双�?96 �?+ uiLabel/uiValue + Tooltip；名称输入框保持可编辑；**Layer Token、眼�?锁、选择、拖动、插入线、Cancel、Undo/Redo、日志与系统保护零改�?*�?
- **公共语义样式**（Ui.axaml）：新增 `uiLabel`（Label12+Secondary）、`uiValue`（Body13+Primary+NoWrap+Ellipsis+MaxLines1）、`uiSingleLine`、`uiMultiline`、`uiSection`（Section14 SemiBold Primary）、`uiTextButton`（Label12+紧凑�?24），全部引用正式 Token；页面局�?key/value 样式删除（统一公共样式）；sideTab/caption �?FontSize 迁移 Token 引用（值不变）；无新增 Token、无新文件超 100 行（Ui.axaml 压缩为项目既有单�?Style 惯例�?26 行）�?
- **测试**：新�?UiD4F1LayoutModelTests（只读行 300~480 均水�?+ 表单 359/360 阈值）、UiD4F1TextOverflowContractTests（uiValue 默认/检查器/调试/地图/图层省略�?Tooltip/多行专用�?按钮）、UiD4F1TypographyContractTests（公共样�?Token/无裸 FontSize/无局�?FontFamily/Manifest 112 Frozen）；更新 UiD4LayoutModelTests（EditableFormLayoutModel）、UiD4InspectorContractTests�?0 列单行）、UiD4MapEditorContractTests（MapId NoWrap+MaxLines1/表单 360）、UiD4LayerContractTests（uiValue/无局�?key）、UiD4DebtClearedTests（新文件清单）、UiLayerVisualContractTests V05/V06（Token 引用）。World 759 �?**811�?16�?*�?
- 验证：全解决方案 `--no-incremental` 串行 Build **0W0E**（落�?/tmp/d4f1-final-build.log）；Core 339/339、World 811/811、WarCore 22/22，合�?**1172/1172 PASS**；启动冒�?PASS（XuanYu.Editor.App.exe 存活）；arch-a-guard PASS；git diff --check PASS�?
- 治理：版�?v0.2.24.41-rz �?**v0.2.24.42-fix**（四处同步）；基线维�?**159 �?*（未新增债务、未改允许项）；未创�?Tag/Release�?
- 状态：**ARCH-UI-SPEC-R1-D4-F1：READY FOR USER RE-ACCEPTANCE**（尚未获得用户真机复验；复验通过�?D4 �?COMPLETE，失败则�?D4-F2 只修真实失败项）�?
- **交付前审查纠偏（REVIEW BLOCKED �?修复，同版本 v0.2.24.42-fix 不升版，2026-08-06�?*�?
  - **恢复双模型并�?*：`MapEditorLayoutModel`�?320 面板紧凑密度：根水平留白 12/8、分组间�?12/8、字段行�?6/4，MapPagePanel 密度接线�? `EditableFormLayoutModel`�?360 输入表单上下）职责分离、互不替代（319/320 �?359/360 两组边界分别生效，组合测�?6 组）；只读资产摘要始终单行双列；
  - **Ui.axaml 恢复可读�?*：放�?26 行压缩版，恢复多行格式（每个 Style/Setter 正常分行）；D4-F1 公共样式（uiLabel/uiValue/uiSingleLine/uiMultiline/uiSection/uiTextButton）拆分至�?`Design/UiStyles.D4F1.axaml`�?5→拆分后 Ui.axaml 76 �?+ 样式文件 59 行，全部 �?00，无压缩单行 Style）；Ui.axaml 仅聚合（StyleInclude 一次）�?
  - **按钮真实接线**：`uiTextButton` 提供 ContentTemplate（TextBlock NoWrap+Ellipsis+MaxLines1）与 Tooltip=完整按钮名（绑定 Content）；地图 7 按钮（新�?打开/保存/聚焦/应用修改/撤销/重做�? 调试 4 按钮（开�?预览/提交/取消）全部真实引用；
  - **按钮布局 v2**：地图属性按钮由横向 StackPanel 改为 `Grid *,*`（应用修改跨两列第一行、撤销/重做第二行等宽，�?28/间距 6）；地图资产按钮为真 2×2 等宽 UniformGrid（Stretch/MinWidth 0/MinHeight 28）；�?300 DIP 下每列约 139 DIP 无裁切；
  - **拆分 MapFormPanel**（新 UserControl：地图属性表单方向切换，职责单一）；测试新增 UiD4F1ButtonContractTests + 边界组合测试，World 811 �?**823**�?
  - **Stash 说明**：当前设备本�?`git stash list` �?0（纠偏前后一致）；先�?D2/D3 轮次登记的�? 个历�?Stash」位于另一开发设备，无法在当前设备复现，本设备从�?stash/pop 操作�?
- 验证（纠偏后最终代码状态）：全解决方案 `--no-incremental` 串行 Build **0W0E**（落�?/tmp/d4f1-fix-final-build2.log）；Core 339/339、World 823/823、WarCore 22/22，合�?**1184/1184 PASS**；启动冒�?PASS；arch-a-guard PASS；git diff --check PASS；版本四处一致（v0.2.24.42-fix 不变）；基线维持 159；Manifest 112 Frozen / 0 Pending�?
- 状态：**ARCH-UI-SPEC-R1-D4：COMPLETE**�?026-08-06 用户正式裁决：D4-F1 真机复验 F1-1~F1-9 全部通过；截图「按钮内容未居中」登�?**D5-FIX-01**：按钮内容水平、垂直居中统一——用户批准延期的已知缺陷，不阻塞 D4，由 D5 第一项统一处理，禁止逐按�?Margin 偏移修补）�?

## D4-A1 文档收口�?026-08-06，不升版�?
- **D5-FIX-01 登记**（用户正式裁决）：`按钮内容未居中`——截图确认按钮文�?图标未水平垂直居中；适用范围：全部标准按钮；处置：D5 第一项统一处理（按钮内容居�?+ 完整状态），禁止逐按�?Margin 偏移修补；当�?D4 不阻塞�?
- D4/D4-F1 全部收口完成：K02/K03/K04 复验 PASS；真�?IPO �?1~21 + F1-1~F1-9 通过�?



## v0.2.24.41-rz
ARCH-UI-SPEC-R1-D4：检查器、地图编辑器与图层工作面板治理（2026-08-06，Commit 本轮落库为准�?
- **检查器治理**（K02/G03/W37~W42/W44）：检查器页签内容拆分至新 `Right/InspectorPanel.axaml`�?code-behind）；字号�?Token 化（面板标题 Title16 / 分组标题 Section14 / 字段标签 Label12 / 字段�?Body13 / 空状态标�?Section14）；`InspectorFields` 从字符串拼接改为结构�?`InspectorFieldRow(Label/Value/IsGroupHeader)`（对�?6 个既有测试同步更新为结构断言）；**响应式双模式**（纯逻辑 `InspectorLayoutModel`：内容宽 �?60 左右布局/标签�?96/字段最�?128�?360 整组上下，同一数据源双布局树切换，无逐字段宽度判断）；空状态保留单一主入口；密度合同（全宽分组标�?1 DIP 分隔线、去卡片嵌套、字段行�?4~6）；调试页标签列 70�?6（W44）�?
- **地图编辑器治�?*（W48/补充裁决）：地图页拆分至�?`Right/MapPagePanel.axaml`�?code-behind）；**只读资产摘要紧凑�?*（标签列 72 组件级例外、单行高 24、空路径显示 —）�?*MapId 显示压缩**（纯逻辑 `MapIdDisplayFormat`�?18 字符显示「前 8+�?�?6」，TextWrapping=NoWrap + CharacterEllipsis，Tooltip 完整 ID，复制按钮走 Clipboard 复制未经截断的完�?MapId）；地图属性编辑表单标签列 96 + **紧凑模式**（纯逻辑 `MapEditorLayoutModel`：内容宽 <320 整组标签上字段下，标签→字段 2、字段组 6~8，关键操作保留）；操作按钮组间距 6�?×2 均匀网格，按钮文字不换行）；每页单一纵向滚动容器；错误色 Token（W47）�?
- **图层面板治理**（K03/K04/W50~W52）：状态图标视�?14�?*16**（Icon.Size.Standard）、笔�?1.5、热�?26×24（登记组件例外不变）；可�?隐藏、锁�?未锁定保持形�?颜色双重表达（VisibleIcon/HiddenIcon/LockedIcon/UnlockedIcon 四个正式矢量图标）；全部状态色迁移 Layer.* 冻结 Token（Visible/Hidden/Locked/LockedBg/Unlocked/VisibleBg、Kind.Region.*/Kind.System.*，System.Text �?#687582�?5D6F7C）；**插入�?�?Layer.DropLine #5B8DB8**�? DIP，用户冻结）；活动标�?�?Color.Accent�?*选中行显式样�?*（Color.Selection.Bg + 按开关类型保留状态色）；类型标签文字（区�?系统�? 形�?+ 低饱和色三重区分；工具栏按钮 24 紧凑档（W50）；图层属性表单标签列 96/字段�?13（W53/W54）；系统图层保护与图层选择/显隐/锁定/拖动/插入�?Cancel/Undo/Redo 业务合同零改动（既有行为测试 795 全绿为证）�?
- **守卫缺陷修复**（ARCH-UI-SPEC-R1-D4 发现）：①`arch-a-guard-warcore.ps1` 无条件重新初始化 `$failures`�?*清空主守卫在子守卫源入前累积的全部失�?*（版本一致性检查恰在其前，失败被吞，门禁长期假 PASS）——改为条件初始化；被源入时不提前 exit（`InvocationName -eq '.'` �?return，统一由主守卫收尾）。②`arch-a-guard.ps1` 5+100 自验证样�?`"a


b
"`（a+2 连续空行+b=4 行）期望误写 3（SHR-2026-08-D2 引入，同样被吞）——修正为 4。修复后门禁如实检出版本不一致（验证通过后随本条目版本同步消除）�?
- **Token 迁移**：Manifest 保持 **112 Frozen / 0 PendingReview**（未新增 Token）；**债务基线 226 �?159 条（-67�?*：Right.axaml（W41/W37/W40/W51 �?12 条）、MapEditorPanel.axaml（W45/W46/W47/W49 �?16 条）、LayerPanel.axaml（W50/W51/W52 �?30 条）、LayerInspectorPanel.axaml（W53/W54 �?6 条）、Ui.axaml section（W14 字号+颜色 2 条）全部真实代码迁移；保�?2 条登记组件例外（activeMark 圆角 1.5、dropLine 圆角 1，规�?§5.4）；未用 Locator/AllowedCount 掩盖�?
- 验证：全解决方案 `--no-incremental` 串行 Build 0W0E（落�?/tmp/d4-final-build.log）；Core 339/339、World 795/795�?36 D4 测试）、WarCore 22/22，合�?**1156/1156 PASS**；启动冒�?PASS；arch-a-guard PASS（含守卫缺陷修复后真实版本一致性）；git diff --check PASS�?
- 治理：版�?v0.2.24.40-rz �?v0.2.24.41-rz（四处同步）；未创建 Tag/Release�?
- 状态：**ARCH-UI-SPEC-R1-D4：READY FOR USER ACCEPTANCE**（尚未获得用户真机裁决；真机通过�?D4 �?COMPLETE，失败则�?D4-F1 只修真实失败项）�?
- 保留（审计矩阵归�?D4 但本轮范围外，报告已说明）：K02 真机复验项随 IPO �?1~4 验收；地图环境页内容补齐（D5 后续）；日志�?弹窗/状态语义（D5）；焦点�?DPI 全量（D6）�?

## v0.2.24.40-rz
ARCH-UI-SPEC-R1-D3：主窗口、顶层页签与滚动治理�?026-08-06，Commit 本轮落库为准�?
- **主窗口与四区外壳**（W15/W16/W18/W19/W20/W21）：初始 1400×820 �?**1360×820**；Min 1100×720 �?**1024×640**；左�?Min 200 �?**220**；右�?Min 260 �?**300**；视口列 Min 360 �?**480**（视口最小可用区�?480×320 合同）；RootGrid Margin 12�? + MinWidth 980�?012�?024 窗口�?6+220+6+480+6+300=1012 恰好满足全部面板最小，无遮挡无溢出）；日志折叠�?MinHeight 32 保留、展开�?120~420（既�?ClampLogRow 登记对齐规范 §7.1）�?
- **外壳 Token 迁移**（清�?4 条基线）：UiWin 背景 `#e9eef5` �?`Color.Bg.Application`（W17）；UiRoot 分隔�?`#dce4ef/#9fb5d6` �?`Color.Border.Strong`/`Color.Hover.Bg`（规�?§9.1 可调整边�?/ §9.3 悬停）；视口 1 DIP 边框 `#C9D2DC` �?`Color.Border.Default`�?*旧债务基线 230 �?226 条，只减不增**；每项下降对应真实代码迁移，未用 Locator/AllowedCount 掩盖�?
- **顶层页签单行溢出系统**（W43/G01，合�?§10.1 15 条中本轮范围）：新增 `Right/TopTabStripTemplate.axaml` 页签宿主模板（单�?ScrollViewer + ItemsPresenter 横向 StackPanel，禁止换行；Hidden 滚动条——宽度充足无滚动控件）；左右箭头（宽度不足显示、到达边界禁用、单击步�?96=Token Size.Width.96）；左右边缘低饱和渐隐（基础命名�?White/Transparent，非业务色值）�?*滚轮横向路由**（页签条 Grid 隧道消费 e.Handled=true，上�?向左；离开页签条滚轮自然回到内容区；到达边界后剩余增量不传递——内容区/日志区不是页签条祖先，树结构隔离）；当前页签自动完整可见（SelectionChanged/ScrollChanged 双路径，窗口缩放后保持）；「全部页签」入口（真实页签动态读取，当前项半�?Accent 标记，点击跳转并自动显露；当前架构无关闭能力，入口只负责发现与跳转，未扩张关闭系统）；首次溢出一次性提示（文案「滚动鼠标滚轮或点击箭头查看更多页签。」，仅当前用户环境首次触发，状态持久化 %APPDATA%\XuanYuEngine\ui-once.json，本会话不重复）。全部新视觉值引用正�?Token 或规范允许值；无每�?PointerMoved/Hover 日志�?
- **分析器门禁完�?*（对应正式测试）：`{StaticResource}` 正式 Token 引用�?Setter/内联属性豁免数值与颜色检查（未登记字面量�?FAIL，正反例�?UiSourceContractAnalyzerTokenRefTests）�?
- 验证：全解决方案 `--no-incremental` 串行 Build 0W0E（落�?/tmp/d3-final-build.log）；Core 339/339、World 759/759�?20 D3 测试）、WarCore 22/22，合�?**1120/1120 PASS**；启动冒�?PASS（进程存�?10s 无崩溃）；arch-a-guard PASS；git diff --check PASS�?
- 治理：版�?v0.2.24.39-rz �?v0.2.24.40-rz（四处同步）；未创建 Tag/Release�?
- **补充修复（同版本不升版，2026-08-06�?*：run.bat 新增 `[0/3]` 清理段（Build �?`taskkill /IM XuanYu.Editor.App.exe /T /F` + `dotnet build-server shutdown` + 1s 等待，失败以 `|| ver >nul` 兜底不中断流程），根治「上次编辑器实例/MSBuild 节点残留导致 `XuanYu.Editor.UI.pdb` 文件�?CS2012」；只针对本编辑器进程与 MSBuild 服务器，**禁止 `taskkill /IM dotnet.exe`**（避免误杀其他 .NET 任务）；`timeout` �?`%SystemRoot%\System32\timeout.exe` 全路径（避免 MSYS PATH 下命�?GNU timeout）；注释全英文（.bat �?GBK 代码页下 UTF-8 中文会乱码成命令）。验证：干净 cmd 环境 run.bat 全流�?[0/3]→[3/3] PASS（构�?0W0E，编辑器进程启动存活，关闭后无残留）�?
- **D3-A1 真机验收收口（同版本不升版，2026-08-06 用户正式裁决�?*：真机验�?15 组中 **1�?�?�?5 全部通过**；第 6 项（左右滚动箭头）在验收尺寸下未出现——用户明确裁定「滚轮横向导航、渐隐提示、当前页签显露和全部页签入口已覆盖发现与跳转需求」，**批准通过**；最大化/恢复/初始非最大化/最小窗�?连续缩放全部通过；页签、地图面板与日志滚动无穿透串扰；顶层页签保持单行、窗口缩窄后仍可访问全部页签�?*登记用户批准偏离�?`D3-EX-01：右侧顶层页签不显示左右滚动箭头`**（适用范围仅限当前右侧顶层页签组件；当前导航合�?滚轮横向滚动+边缘渐隐+当前页签自动显露+全部页签入口；不自动修改 UI Spec 通用规则，D6/A1 若出现键盘可访问性或页签数量增长问题再重新审查）。K01/K05/K06 复验 PASS；D3 �?**COMPLETE**�?
- 状态：**ARCH-UI-SPEC-R1-D3：COMPLETE（用户真机验收通过�?026-08-06�?*�?
- 保留（审计矩阵归�?D3 但本轮范围外，报告已说明）：W28 菜单复核（合规无改动）、W29 leftTab 字号（Left.Styles 不在本轮允许范围）、W36 删除菜单色（Left.axaml 不在范围，留 D5）、G02 键盘焦点（留 D6）、G04 面板紧凑/折叠（留后续轮）、K07 日志六档复验（D5）�?


## v0.2.24.39-rz
ARCH-UI-SPEC-R1-D2：Token 基础设施与自动化门禁�?026-08-05，Commit 本轮落库为准�?
- **Token 基础设施**：新�?`XuanYu.Editor.UI/Design/` 8 �?Token 文件（UiTokens.Fonts / Colors.Core / Colors.Components / Spacing / Controls / Icons / Motion + UiTokens 聚合入口），数值、类型与命名全部直接来自 UI Spec 1.0�?12 �?Token 键，无临�?兼容 Token）；`Ui.axaml` 仅新增资源聚合（Styles.Resources 合并 Design/UiTokens.axaml），**未修改任何现有页面视觉、布局、交互或业务行为**（当前编辑器视觉�?D1 基线一致）�?
- **Token 合同测试**（`XuanYu.World.Tests/UiTokens/`，复用既�?UI 源码合同测试承载点，无新项目/新依赖）：键全局唯一、代码键集合==规范合同清单（UiTokenContractCatalog 112 键，无缺失无额外）、数值与规范一致（Fonts 21/Colors 38/Sizes 41 项断言）、字号与行高一一配对、聚合入口含 7 子文件、无循环引用�? �?Token 文件全部 �?00 行、应用资源已合并聚合入口�?
- **源码违规分析�?+ 旧债务基线门禁**：测试侧 `UiSourceContractAnalyzer`（HexColor/FontSize/CornerRadius/ControlHeight/BoxShadow/StrokeThickness/EmojiIcon/CsHexColor 八规则，限定 UI 控件语义，Path 图标尺寸/布局容器/CornerRadius 0 不误报）；旧债务细粒度基�?173 条指纹（相对路径+规则类型+规范化�?允许次数，映�?W 编号，自动生成自真实源码现状值）：已知债务允许、新增债务（含同文件第二处同值）测试失败、债务减少允许、基线不自动增长；扫描范围排�?Design/ 与渲染目录�?
- **门禁自验�?*�?0 项正反例（合�?Token 引用 PASS、未登记�?字号 15/圆角 5/高度 34/Emoji 图标/BoxShadow/笔画 2.2 FAIL、布局与图标值不误报、cs 色构�?FAIL）全部通过�?
- 审计矩阵新增「六、验证方式标注」：W/G/K 标注自动门禁/真机/混合/暂不可自动化及原因；不宣�?W01~W71 已整改。债务登记更新 D2 状态与基线规则�?
- 验证：全解决方案串行 Build 0W0E；Core 339/339、World 809/809�?123 UiTokens）、WarCore 22/22，合�?1170/1170 PASS；arch-a-guard PASS；git diff --check PASS�?
- 治理：版�?v0.2.24.38-rz �?v0.2.24.39-rz（四处同步）；未创建 Tag/Release�?
- 状态：ARCH-UI-SPEC-R1-D2 COMPLETE；D3 主窗�?顶层页签/滚动治理待用户批准启动�?

### D2-F1 Token 门禁可靠性纠偏（2026-08-05，Commit 本轮落库为准；同版本不升版）
- 用户复核裁定 D2 REVIEW BLOCKED 四项：基线可换位绕过、测试侧第二套事实源、Token 值覆盖不全（112 键仅 100 项值断言）、扫描范围不足（固定 cs 清单/Design �?Token/Emoji 图标位置）�?
- **基线升级为细粒度指纹**：每条含 W 编号/相对路径/稳定定位（Style Selector �?x:Name �?元素类型；code-behind �?类型�?成员名）/属性名/规则类型/规范化�?允许次数；匹�?Path+Locator+Kind+Property+Value 全部参与�?25 条基线重新生成（�?Locator，注释剥离）�?0 项绕过反例全部通过：原位置保留 PASS、删�?PASS、同选择器第二处 FAIL、异控件/�?Style/�?x:Name/异属�?�?Kind 换位 FAIL、注释漂移不影响基线、基线不自动增长�?
- **单一机器事实�?*：新�?`Design/UiTokenManifest.json`�?12 条：Key/Type/Value/Category/SpecSection/Purpose/SpecStatus）；`scripts/generate-ui-tokens.py`（≤100 行，无第三方依赖）确定性生�?8 �?Token XAML（带"生成文件"头，幂等验证通过）；删除测试侧手写键清单�?100 项手写期望值表（UiTokenContractCatalog/Fonts/Colors/Sizes 测试），改为 Manifest↔XAML 双向合同�?12/112 键、类型、值全覆盖，无重复/缺失/大小写漂移）�?
- **SpecStatus 核查（D2-F1�?*�?7 �?Frozen（规范已冻结具体值，�?Log.Accent.*/DocStatus.*/LogTable.Columns/Tree.Guide 等）�?*15 �?PendingReview**（规范只有区间或方向，值待规范审订）：Motion.HoverMs/ExpandMs（规范仅 80�?20/120�?60 区间，此前取中值为执行 AI 自选，已停止数值冻结）、Layer.Kind.* 6 色、Layer.State.* 6 色、Layer.DropLine（规�?§12.2 仅交互规则无色值）。缺失项已报告，等待规范审订裁决�?
- **递归扫描全部 UI code-behind**：删除固�?CsVisualSources；递归 `XuanYu.Editor.UI/**/*.cs`（排�?bin/obj/生成文件）；确认�?5 个视觉文件含 hex；新�?�?UI .cs 加入原始颜色必被报告"测试�?
- **Design �?Token 声明检�?*：非 Design/ AXAML �?SolidColorBrush/x:Double/x:String/Thickness/CornerRadius/FontWeight/FontFamily x:Key 声明即违规（EditorIcons.axaml �?StreamGeometry 图标资源合法不误报）�?
- **Emoji/Unicode 图标检�?*：按�?Content、切换按�?Content、图�?TextBlock（Classes/x:Name 特征）Text 与元素内容、Path/PathIcon Data；中文按�?工具提示/StreamGeometry 不误判（Path 数据�?Unicode 符号区检测，兼容 F1 填充标记）�?
- **ResourceInclude 完整�?*：聚合含 7 个批准文件、目标存在、子文件不反向引用、应用只合并一次�?
- 验证：全解决方案串行 Build 0W0E；Core 339/339、World 719/719（含 UiTokens 33 项）、WarCore 22/22，合�?1080/1080 PASS；启动冒�?PASS；arch-a-guard PASS；git diff --check PASS�?
- 事实表述（修正后）：Token 门禁已实�?已知债务允许（细粒度定位）、任何新增债务（含换位）失�?�?12/112 键、类型、值与 Manifest 一致（Manifest 为唯一机器事实源）�?5 条组件值待规范审订，未声称全部与规范数值一致；W01～W71 未清零�?

### D2-F2 缺失参数冻结与门禁最终加固（2026-08-05，Commit 本轮落库为准；同版本不升版）
- 用户复核裁定 D2-F1 �?BLOCKED 四项：① 15 �?PendingReview Token 在运行时生效（数值未冻结却已加载）；�?AXAML 指纹末级仅元素类型（匿名控件可换位）+ 颜色属性统一�?Color（Foreground/Background 不可区分�? 2 �?code-behind Unknown；③ code-behind 只扫 hex 字符串（Colors.*/FromRgb/FromArgb/SolidColorBrush/0x 常量未覆盖）；④ 首次 Build 出现 1 �?Warning 后复跑消除且未定位来源�?
- **15 项缺失参数由用户正式裁决并写�?UI Spec 1.0**：新�?§12.2.1 图层组件 Token 表（Layer.Kind.Region/System 6 色、Layer.State 6 色、Layer.DropLine）与 §15.3 动效默认 Token（Motion.HoverMs=100 / ExpandMs=140）；其中两项修正执行 AI 旧值：`Layer.Kind.System.Text` #687582�?*#5D6F7C**（对比度 4.64:1）、`Layer.DropLine` #7FA8C6�?*#5B8DB8**（选中背景对比�?3.04:1）；变更历史登记"用户 D2-F2 缺失参数补充裁决"。Manifest 全部冻结�?*112 Frozen / 0 PendingReview**，生成器重新生成 XAML（幂等验证通过，全�?�?00 行）�?
- **AXAML 稳定定位升级 v3（父链定位）**：匿名元�?Locator = `Path:<最近命名祖先|ROOT>/<父类型链>/<类型>:<同父序号>`（如 `Path:Name:LogList/ListBox/DataTemplate/Grid/Border:1`）；颜色违规记录真实属性名（Background/Foreground/BorderBrush/Fill/Stroke/Color 等）�?*基线 Unknown Locator = 0**（cs 成员正则补齐 async/无修饰符/const 字段/显式接口实现）。新�?7 项反例：�?Style Foreground→Background FAIL、Background→BorderBrush FAIL、匿�?Border/TextBlock 换位 FAIL、不同父级同类型换位 FAIL、空�?注释/无关属性变�?PASS。基线重生成 **230 �?*（父链定�?v3）�?
- **code-behind 八类颜色写法全覆�?*：`#RRGGBB`/`#AARRGGBB`、`Colors.*`、`Color.FromRgb/FromArgb/Parse`、`new SolidColorBrush`、`0xRRGGBB`/`0xAARRGGBB` 常量——每种至少一�?FAIL 样例（含 const 字段 Locator）；递归扫描全部 UI .cs 维持。允许清单按"路径+规则类型+API 模式+原因"登记：TreeGuide.cs Render（树引导线渲染色，ALLOW-RENDER）、Win32ViewportHost.cs（Win32 样式常量非颜色，ALLOW-WIN32）；渲染/宿主代码不误报�?
- **上一�?Warning 追溯**：D2-F1 首次 Build 的完�?stdout/stderr 未落盘（仅冒烟日志）�?*无法追溯警告来源；流程不合规（正�?build 输出必须落盘）——如实登�?*。D2-F2 终验复跑时再次出�?1 个警告，本次**成功定位**：`UiTokenManifestGraphTests.cs(32,9) xUnit2013: Assert.Equal(1, Matches.Count) 应改�?Assert.Single`——xUnit 分析器仅�?*全量编译**时触发（增量构建跳过分析器重跑，此前"偶发 1 警告"实为增量掩盖）。已修复（Assert.Single），**`--no-incremental` 干净全量重建落盘验证 0 警告 0 错误（d2f2-final-build.log�?*�?
- 验证：全解决方案串行 Build 首次执行 0W0E（落�?d2f2-build.log）；Core 339/339、World 739/739（含 UiTokens 53 项）、WarCore 22/22，合�?1100/1100 PASS；启动冒�?PASS；arch-a-guard PASS；git diff --check PASS�?
- 事实表述（D2-F2 修正后）：UI Spec 1.0 已正式冻结全�?112 �?Token；Manifest 112 Frozen / 0 PendingReview�?12/112 键、类型、值一致；Locator �?Unknown；同值在属�?匿名控件/父级之间换位均失败；全部 UI code-behind 颜色构造被覆盖；W01～W71 未清零�?
- 治理：TODO 同步纪律经用户明确授权写入宪法第十九条与三个技能（xuanyu-engine-dev / xuanyu-engine-development / xuan-yu-engine-development）；宪法变更记录授权来源�?

## v0.2.24.38-rz
ARCH-UI-SPEC-R1-D1：正式规范冻结（2026-08-05，Commit 本轮落库为准�?
- `docs/ui/玄域引擎_UI规范_1.0.md` �?WORKING DRAFT 转为**正式规范（UI Spec 1.0，唯一 UI 规范事实源）**�?4 节结构：身份与适用范围、条款分级、字体字号（回退�?字号�?字重/行高）、颜色背景（四级背景/语义�?日志�?文档状态色）、间距尺寸圆角、控件高度与热区、窗口阈值、图标、边框阴影焦点、页签菜单弹窗、表单错误、树列表拖拽、日志加载空状态、键盘可访问、DPI 性能、游�?UI 边界、Token 层级命名、自动化矩阵、允许清单、受控例外、变更流程、真机验收、版本历史、文档关系�?
- **16 项待审订全部裁决**（无待定项）：分组标�?Section 14（列�?Label 12，废�?13 三套并存）；字重四档适用表；回退�?Avalonia 三级+平台兜底（禁 Inter）；行高组合定义+单行控件不套行高；日志级别色=Log.Accent.* 组件 Token；文档状态色=DocStatus 三态组�?Token；渲�?Shader/Gizmo/网格/数据可视�?允许清单边界；宽度等级例外（表格列宽/热区/分隔�?组件级）；控件高�?24/28/32+现状迁移目标表（34�?8/30�?8/25�?4/42→公式）；圆�?3/6/10 场景分配表；阴影（面板禁阴影+悬浮�?4/12/14%+Popup 系统阴影不叠加）；焦点五态区�?焦点�?2/外偏�?1+与选中并存规则；顶层页�?15 条合同入规范；菜单顺序与弹窗按钮顺序冻结；游�?UI 基础强约�?艺术弱约束边界；D2 自动检查矩�?允许清单格式+例外八要素�?
- **事实源治�?*：`docs/governance/ui-spec.md` 降级为历史讨论决策记录（顶部标注「不再作为实施合同」）；审计矩阵与真机清单补充正式规范引用；债务登记 `arch-ui-spec-debts.md` 状态「待立项」→「治理中」（记录 D0/D1 COMPLETE、W01~W71/G01~G08/K01~K07、下一�?D2、暂停新�?UI 功能、未经批准不得创建例外）�?
- 本轮未修改任�?UI 视觉、布局、交互或业务行为实现；仅为版本同步修�?`UiWin.axaml` �?`UiVm.SceneDocument.cs` 中的版本字符串，并同步修�?`run.bat`。未创建 Token；未修改渲染、地图、WarCore、Scene、持久化、宪法与技能；未创�?Tag/Release�?
- 治理：版�?v0.2.24.37-rz �?v0.2.24.38-rz（四处同步，升版依据：治理文档正式状态变化按项目惯例升版）；未创�?Tag/Release�?
- 状态：ARCH-UI-SPEC-R1-D1 COMPLETE；D2 Token 基础设施待用户批准启动�?

### D1-F1 治理层级与事实表述纠偏（2026-08-05，Commit 本轮落库为准；同版本不升版）
- 用户复核裁定 D1 REVIEW BLOCKED 两项：① 规范 1.4 治理优先级将 UI Spec 置于代码宪法与开发硬规则之上，且遗漏 AI_DEVELOPMENT_RULES.md 与已冻结架构/领域合同；② changelog �?D1 条目曾声称文件层面零 UI 修改，与实际（版本字符串修改�?UiWin.axaml / UiVm.SceneDocument.cs）不符�?
- �?规范 1.4 重写为五层治理层级：第一�?AI 开发宪法；第二�?CODE_CONSTITUTION / AI_DEVELOPMENT_RULES / dev-rules 及已冻结架构·领域·持久化合同；第三�?UI Spec 1.0�?*UI 设计规则领域内唯一事实�?*）；第四层阶段治理计划；第五层阶段局�?UI 合同。明确：UI Spec 不得覆盖第一、二层；第二层内部跨领域冲突不得�?UI Spec 裁定，必须停止实施并上报；冲突时停止并上报、不得自行选择�?
- �?changelog 事实表述修正为：「本轮未修改任何 UI 视觉、布局、交互或业务行为实现；仅为版本同步修�?UiWin.axaml �?UiVm.SceneDocument.cs 中的版本字符串，并同步修�?run.bat。未创建 Token」。明确区分「文件发生修改」与「UI 行为没有发生修改」�?
- 全仓错误表述零残留检查：当前文档仅归�?changelog-2026-07（历史记录，按归档原则不改）；changelog.md / 规范 / debts / docs-index / file-tree 已清零�?
- 状态：ARCH-UI-SPEC-R1-D1-F1 完成；D1 COMPLETE；D2 Token 基础设施待用户批准启动�?

## v0.2.24.37-rz
ARCH-UI-SPEC-R1-D0 启动：UI 治理基线�?026-08-05，Commit 本轮落库为准�?
- 执行过程中曾提出 TODO 治理建议（每轮任务列任务清单并同步执行情况），因未获授权且超�?D0 范围，已撤销；是否纳入宪法留待独立审订（2026-08-05 用户复核裁定，见下方 D0-F1 纠偏）�?
- 依据用户批准的《ARCH-UI-SPEC-R1 治理实施计划》启�?D0 基线冻结与全量审计（只审计、不整改）：
  - 新增 `docs/ui/玄域引擎_UI规范_1.0.md`：审订工作副本（首批冻结参数 + 26 章条款审订状�?+ 16 项待审订清单）；
  - 新增 `docs/ui/玄域引擎_旧UI审计矩阵.md`：全量审计（16 界面 + 5 �?code-behind 视觉源；违规 71 �?W01~W71 + 结构性缺�?8 �?G01~G08，逐项标注整改轮次）；
  - 新增 `docs/ui/玄域引擎_UI真机基线清单.md`�?0 组中�?IPO 验收清单 + DPI/窗口覆盖矩阵 + 已知问题 K01~K07 登记�?
- 关键审计发现：旧蓝强调色系（#185aa6/#edf4ff/#8cb2e2/#2F80C9 等）�?Accent #326F8A 冲突；全局无字号默认（正文�?12 vs Body=13）；圆角 4/5/7/9 大量越界（规范只允许 3/6/10）；面板阴影 0 14 30 违反「普通面板禁阴影」；窗口 1400×820/1100×720 vs 规范 1360×820/1024×640；顶层页签溢出管�?15 条合同未实现；树图标笔画 2.2 vs 1.5�?
- `docs-index.md` 登记 docs/ui/；`file-tree.md` 目录树与职责索引同步�?
- 治理：版�?v0.2.24.36-rz �?v0.2.24.37-rz（四处同步）；未创建 Tag/Release�?
- 状态：ARCH-UI-SPEC-R1-D0 完成；D1 规范冻结为下一轮；MAP-A 功能开发按计划暂停至治理收口�?

### D0-F1 治理纠偏�?026-08-05，Commit 本轮落库为准；同版本不升版）
- 用户复核裁定 D0 REVIEW BLOCKED 三项：① 执行 AI 越权修改开发宪法与三个 skills；② 正式测试门禁未执行；�?汇报 SVG 违反项目浅色规范。进�?ARCH-UI-SPEC-R1-D0-F1 纠偏�?
- �?越权恢复：宪法恢复至 a84fd2e 前内容（第十九条 TODO 条款已撤销，未触碰其他条款）；三个本地 skills（xuanyu-engine-dev / xuanyu-engine-development / xuan-yu-engine-development）恢复原状（技能库不在 Git 仓库，恢复结果无法以仓库证据验证，如实说明）；changelog「用户已裁定 TODO 入宪」表述已删除�?
- �?正式门禁补齐：全解决方案串行 Build 0W0E；Core.Tests / World.Tests / WarCore.Tests 全量通过（真实数量见本轮验证段）；arch-a-guard PASS；git diff --check PASS�?
- �?浅色 SVG：重新生成浅色版（白/极浅蓝灰底、深蓝灰文字、低饱和强调、无渐变），仅临时汇报不入库�?
- file-tree 纠偏：删除重复迷�?docs 树节点（目录�?docs 唯一）；docs/ui/ 层级正确；修复版本号规范路径八进制乱码；16 �?docs 文件职责全部补齐（非 docs �?126 处职责待补为历史欠账，登记后续治理轮）�?
- 状态：ARCH-UI-SPEC-R1-D0-F1 完成；D0 COMPLETE；D1 规范冻结待启动�?

## v0.2.24.36-rz
UI 规范 1.0 讨论初稿落库�?026-08-05，Commit 本轮落库为准�?
- 新增 `docs/governance/ui-spec.md`：ARCH-UI-SPEC-R1 讨论汇总初稿（45 项关键决策）落库，覆盖规范定位与例外机制、三�?Token 体系、颜�?文字/间距/布局、页�?图标/反馈/表单/键盘/拖拽/菜单/弹窗/空状�?高密度组�?DPI/动效、游�?UI 边界、执行闭环、当前局部基线（MAP-A-R2-D4-F3）与待补充项；状态待审订，Token 数�?测试矩阵/整改计划未冻结（不得由实�?AI 自行决定）�?
- `docs-index.md` 登记新文档；`file-tree.md` 目录树与职责索引同步�?
- 治理：版�?v0.2.24.35-fix �?v0.2.24.36-rz（四处同步）；未创建 Tag/Release�?

## v0.2.24.35-fix
MAP-A-R2-D4-F3 预验收补丁：图层状态反馈与通知时序修正�?026-08-05，Commit 本轮落库为准�?
- P1 状态图标真实切换：`MapLayerRowViewModel` 增加派生状�?`IsHidden`/`IsUnlocked`（IsVisible/IsLocked 变化时同步通知）；行模板分别显�?VisibleIcon/HiddenIcon、LockedIcon/UnlockedIcon（形状与颜色共同表达状态），保�?F3 配色合同�?26×24 热区�?
- P2 拖动插入线通知：`IsDropBefore` 从无通知自动属性改�?backing field + `Set` 通知；插入线�?`Grid.ColumnSpan="6"` �?`ZIndex="10"`（整行宽、置顶）；`SetDropTarget(null)` 清理所有行插入线�?
- P3 同位置拖�?No-op：`CommitLayerDrag` �?`before == after` 时立即返回——不写日志、不�?FooterMessage、不 Dirty、不增历史（会话层本�?No-op，UI 层补齐静默）�?
- P4 通知时序：`EditorLogSummary.ChooseLatest` �?先扫全部 Error/Warning 再扫 Editor"改为单次逆序扫描（最新一�?Error/Warning/Editor/Project 即返回）——旧警告不再永久霸占底部通知，新操作可取代已处理的旧警告，新警告仍覆盖旧操作；Vulkan/Render Info 在完整日志中按真实时间保留�?
- P5 拖动异步收口：`DragCandidate_PointerMoved` �?`async void` + `await DragDrop.DoDragDropAsync`，`finally` 中清理插入线（拖动结束才清理，异常不成为未观察任务）；无 Sleep/Timer/fire-and-forget�?
- 验证：全解决方案 Rebuild 0W0E；Core 339/339、World 686/686�?8：A/B/C/D/E/F/G/H）、WarCore 22/22；arch-a-guard PASS；git diff --check PASS�?
- 遗留/下一步：F3 真机补验（F3-A01~A16 + P-A01~P-A06）→ A5 全量复验 �?D4 COMPLETE；ARCH-UI-SPEC-R1 立项讨论；D5 区域绘制�?

## v0.2.24.34-fix
MAP-A-R2-D4-F3 图层视觉、拖动排序与通知收口�?026-08-05，Commit 本轮落库为准�?
- F3-01 状态图标重做：取消强蓝实心底，改为图标本体为主浅色底辅助（可见 #326F8A/#EAF3F7/#BDD5DF、隐�?#8995A2；锁�?#7A6238/#F4EFE5/#DCCDAE、未锁定 #7B8794）；热区 26×24 DIP、图�?14、圆�?4、独�?ToolTip�?
- F3-02 类型标签区分：区�?#E8F3F6/#326B7B/#B9D7DE（蓝青），系�?#F0F2F4/#687582/#D5DBE0（灰蓝）；系统层不显示拖动手柄，区域层显示六点手柄�?
- F3-03 右侧字号收敛：顶层页�?15�?3（Ui.axaml sideTab）、地图二级页签新�?layerSubTab 14 选中半粗、字段标�?12 / 字段�?13 / 按钮 12；不扩张全局主题�?
- F3-04 区域图层拖动排序：`MapLayerStack.MoveRegionToIndex`（纯函数，targetIndex 0=最上区域层，Ground/Boundary 固定 Order 0/1，区域层 Order 连续唯一，越�?同位置安全返回原集合）；`MapEditSession.MoveLayerToRegionIndex` 单历史节点命令（系统�?未知/越界失败零污染，同位�?No-op �?Dirty 不增历史）；UI �?Avalonia DragDrop（DataTransfer 文本载荷 LayerId�? DIP 启动阈值�? DIP 插入�?#7FA8C6、仅区域行接�?Drop、一�?Drop 一次提交一次日志）；上�?下移按钮保留�?
- F3-05 通知优先级（方案 B）：Vulkan 日志�?Dispatcher.Post 异步到达（根因确认）覆盖用户通知；`EditorLogSummary` 改为选择策略——最�?Error/Warning > 最�?Editor/Project 动作 > 最新兜底；完整日志面板仍按真实时间保留 Vulkan 记录；无 Sleep/计时器�?
- F3-06 登记 `ARCH-UI-SPEC-R1`（docs/governance/debts/arch-ui-spec-debts.md�?7 项范围，待立项，不展开实施）�?
- 验证：全解决方案 Rebuild 0W0E；Core 339/339、World 678/678�?33：T01-T08、H01-H06、U01-U06、V01-V06、L01-L05）、WarCore 22/22；arch-a-guard PASS；git diff --check PASS�?
- 遗留/下一步：F3 真机补验（F3-A01~A16）→ A5 全量复验 �?D4 COMPLETE；ARCH-UI-SPEC-R1 立项讨论；D5 区域绘制�?

## v0.2.24.33-fix
MAP-A-R2-D4-F2 顶部菜单与右侧冗�?UI 收敛�?026-08-05，Commit 本轮落库为准�?
- F2-01 顶部"添加"菜单扁平化：删除"基础实体"级联层，"立方�?成为"添加"直接子项（Top.axaml）；�?文件"共用同一 Menu/MenuItem 样式（背�?边框/行高/留白/悬停反馈/字号全一致），无横向级联箭头；命令与 CommandParameter（添加立方体）零改动，未新建第二套命令�?
- F2-02 删除右侧顶层"偏好"页签及占位面板（布局保存/主题/快捷键占位）；同步清理仅服务该页的绑定字段：`PropertyItems` 属性（UiVm.cs）与 `UiText.PropertyItems` 字段整体删除�?
- F2-03 删除右侧顶层"模式"页签（非可选模式、无控件、制造无意义空白）；右侧顶层收敛�?检查器 | 地图编辑�?| 调试，地图编辑器二级导航（地�?图层/环境）直接贴近顶层页签；`SnapMode` 属性保留（工具状态读取，注释登记暂无 UI 消费者）�?
- F2-04 图层锁定日志细化：新增专用帮助函�?`LogLayerLockChanged(layer, before, after)`（不再用 FormatBoolean �?�?�?）；消息�?锁定图层：区�?1（区域）/解锁图层：边界（系统�?，详情列 `LayerId=<完整 ID>；状态：未锁�?�?已锁定`；仅状态真实变化记录一次（同�?No-op 不记录），失败日志保留原因；LogLayer 增加详情列参数重载�?
- 测试：新�?`UiMapLayerLockLogTests`（L01 锁定区域图层/L02 解锁/L03 系统层带（系统）/L04 详情�?LayerId 与状态变�?L05 同�?No-op 无日�?L06 一次点击一条日�?+ C01 添加立方体单次创建）；`UiMapLayoutContractTests` 扩展 U01（无基础实体�?U02（立方体为添加直接子项）/U03（无偏好页签�?U04（无模式页签�? 既有 U05/U06；`UiMapLayerPanelTests.Behavior` 锁定断言更新�?锁定图层：区�?1（区域）"与解锁断言�?
- 验证：全解决方案 Rebuild 0 Warning / 0 Error；Core 339/339、World 645/645、WarCore 22/22 �?PASS；arch-a-guard PASS（依赖边�?5+100）；git diff --check PASS；无新增 NuGet；无 Schema 改动；Vulkan 生命周期零改动；NavGizmo CS8602 已在前轮（v0.2.24.32-fix）独立修复，本轮未触碰�?
- 遗留：真�?F2-A01～A12 验收（菜单展开样式对比、偏�?模式消失、锁定日志、滚动布局、可见�?重命名回归）�?
- 治理：版�?v0.2.24.32-fix �?v0.2.24.33-fix（四处同步）；file-tree 重建（新�?UiMapLayerLockLogTests.cs）；未创�?Tag/Release�?

## v0.2.24.32-fix
VK-WARN-NAVGIZMO-R1 导航 Gizmo 空引用警告消除，恢复全解决方�?0W0E�?026-08-05，Commit 本轮落库为准�?
- 根因（只读调查确认）：`VulkanNativeHost.NavGizmo.cs` �?19~22 行三元表达式 `vm.NavigationCamera is null ? null : NavigationGizmoHitTest.Hit(...)` �?`hit` 推断为可�?`GizmoHitResult?`（`Hit` 本身返回非空类型），�?24 �?`hit.IsEndpoint` 触发 CS8602——相机在启动/重建/销毁期允许�?null（情�?B：生命周期期间可空），原代码该路径若可达即为真实 NRE�?
- 修复（最小，�?1 个生产文件，+2 行）：捕获相机局部快�?`var camera = vm.NavigationCamera;`，`camera is null �?return false`（相机未就绪时无法计�?Gizmo 方向，不消费事件，继续走实体 Picking，与"区域外不捕获"语义一致）；`hit` 恢复非空类型；第 27 行条件简化为 `_navGizmoEndpoint is null`（guard 后相机非空已保证，行为等价）。未使用 `!`/pragma/NoWarn/`?.`，未�?Vulkan 生命周期、Gizmo 尺寸/颜色/命中半径，无新增日志�?
- 存量警告合规修正（Rebuild 暴露，与本轮无关�?0W0E 出口必需，语义等价）：`ReferenceGridScaleTests` xUnit2000（Assert.Equal 参数交换 expected=2.4）、`SaveTransactionTests` xUnit2013（Assert.Equal(1, Count) �?Assert.Single）�?
- 验证：全解决方案 Rebuild 0 Warning / 0 Error（含 `-warnaserror` �?`-p:WarningsAsErrors=CS8602` 双口径）；Core 339/339、World 636/636、WarCore 22/22 �?PASS；NavigationGizmo 聚焦 16/16；arch-a-guard PASS；git diff --check PASS；无新增 NuGet；无 Schema 改动�?
- 遗留：真�?WN-A01～WN-A08 冒烟（Gizmo 显示/Hover/轴向/缩放/启动/关闭）�?
- 治理：版�?v0.2.24.31-rz �?v0.2.24.32-fix（四处同步）；file-tree 无结构变化仅校验不重建；未创�?Tag/Release�?

## v0.2.24.31-rz
MAP-A-R2-D4-F1 图层 UI 归位：迁入右侧地图编辑器二级导航�?026-08-05，Commit 本轮落库为准�?
- 撤回 D4 左侧"图层"页签（信息架构修正）：`MapLayer` 属于地图资产，不是项目资�?场景层级/全局功能，不应与"项目、层�?并列；左侧全局导航恢复�?项目 | 层级"�?
- `MapEditorPanel` 内部新增二级导航（地�?/ 图层 / 环境）："地图"= 地图资产+地图属性（原内容）�?图层"= 图层列表+图层属性，"环境"= 环境占位；每页独�?ScrollViewer 整页滚动（禁止内容穿透固定标题、禁止嵌套滚动）�?
- `LayerPanel`（git mv Left/ �?Right/）去掉内�?ScrollViewer，列表全量展示由页面滚动接管；`LayerInspectorPanel` 精简�?图层属�?（名�?类型/顺序/图层 ID/设为当前图层；可�?锁定开关保留在列表行内），与图层列表同页位于下方�?
- 右侧全局"检查器"移除图层面板嵌入，`IsEmptySelection` 恢复�?`!HasSelection`（图层选中不再送到全局检查器）�?
- 领域/命令/撤销重做/显隐渲染逻辑零改动（沿用 D4 实现）�?
- 测试：新�?`UiMapLayoutContractTests` 4 项（左侧仅项�?层级且无图层页签、地图编辑器含地�?图层/环境三页、图�?UI 位于地图编辑器图层页、全局检查器无图层面板），源码合同模式防回归�?
- 验证：Core 339/339、World 636/636 �?PASS；arch-a-guard PASS（依赖边�?5+100）；全解决方�?build 0 error；git diff --check PASS；无新增 NuGet；无 .xymap schema 改动�?
- 遗留：真�?A5 复验（图层页签位置、滚动、穿透）�?
- 治理：版�?v0.2.24.30-rz �?v0.2.24.31-rz（四处同步）；未创建 Tag/Release�?

## v0.2.24.30-rz
MAP-A-R2-D4 图层管理与可见性闭环（2026-08-05，Commit 本轮落库为准�?
- 领域（World）：`MapLayerKind` 迁移�?Ground/Boundary/Region（�?0/1/2 不变：Base→Ground 同值、Custom→Boundary 同值，零持久化风险）；新增 `MapLayerRules`（名称校�?1~32 字符禁控制字符、系统层/最后区域层删除保护、区域层排序边界保护、自动命�?区域 N"按最小可用序号）�?`MapLayerStack`（纯函数顺序操作：区域层间交�?Order、系统层顺序固定、显�?锁定/改名保身份）；`MapLayerValidator` 升级（Ground �?1 �?Order 0、Boundary �?1 �?Order 1、Region �? �?Order �?）；`MapRegionValidator` 区域仅可挂载 Region 图层；默认地�?= 地面/边界/区域 1（区�?1 可见未锁定）�?
- 编辑（Editor）：`MapEditSession` 新增六类图层内容命令（AddRegionLayer/RenameLayer/RemoveLayer/MoveLayerUp/Down/SetLayerVisibility/SetLayerLocked）全部走既有 CommitMapChange 管线（单历史节点、失败零污染、同�?No-op 无历史），MapEditReason 扩展 6 项；活动区域图层为会话临时状态（`ActiveRegionLayerId` + 事件，不 Dirty 不进历史），添加自动设为活动、删除自动转移相邻、内容变化自动规范化到有效区域层（H10）；撤销恢复相同 MapLayerId�?
- 渲染：`MapRenderSnapshot` �?ShowGround/ShowBoundary（渲染过滤，不删除领域数据）；投影从图层取系统层可见性；`RenderDrawPlan` 拆分 MapGround/MapBounds 两绘制项（主文件�?partial �?100 行），隐�?跳过对应绘制项，网格/原点/�?Gizmo 不受影响；显隐不�?`MapSurfaceResourceKey`（R06：显隐切�?NoRebuild 不重�?GPU 资源）；Vulkan Draw.cs �?MapGround/MapBounds 分发�?
- UI：左侧新�?图层"页签（LayerPanel：添�?上移/下移/删除 + 行内可见/锁定开�?+ 系统标签 + 活动左标记，路径图标体系）；右侧检查器选中图层显示 LayerInspectorPanel（名�?Enter/失焦提交、类�?可见/锁定/顺序/ID 只读、设为当前图层）；命令路�?5 项（添加/上移/下移/删除图层/设为当前图层）；中文日志 9 类（添加图层：名�?�?/ 重命名图层：�?�?�?/ 图层可见性：�?隐藏 / 图层锁定：�?�?/ 调整图层顺序：…，上移 / 设置当前图层：�?/ 删除图层：�?/ 图层删除失败：至少保留一个区域图层）�?
- 测试：新�?MapLayerRulesTests/MapLayerStackTests(+Order)/MapLayerSessionTests(+Behavior)/UiMapLayerPanelTests(+Behavior)/MapSurfaceLayerVisibilityTests；更�?MapLayerTests(+Base)/MapRegionTests(+Strictness)/MapDefaultMapTests/MapEditSession* 等默认图层结构断言（区域层索引 2）�?
- 验证：Core 339/339、World 632/632 �?PASS；arch-a-guard PASS（依赖边�?5+100）；全解决方�?build 0 error；git diff --check PASS；无新增 NuGet；无 .xymap schema 改动；Vulkan 生命周期零改动�?
- 遗留：① 区域图层隐藏的消费（D5 绘制区域时读�?IsVisible）；�?锁定状态阻止编辑行为由 D5 接入；③ 图层保存/重新打开�?D6；④ 拖拽排序/混合模式/图层组明确不做�?
- 治理：版�?v0.2.24.29-fix �?v0.2.24.30-rz（四处同步）；未创建 Tag/Release�?

## v0.2.24.29-fix
VK-PERF-R1 空闲渲染帧率与资源占用收敛（2026-08-04 21:13:49，Commit 本轮落库为准�?
- 根因（只读调�?+ 线程级采样证实）：`VulkanSwapchainCapabilities.ChoosePresentMode` 默认优先 `MailboxKhr`（无 vsync 上限），`VulkanPresentLoop.RunFrames` 有投影时全�?Acquire→Submit→Present 无帧率限制，`AcquireNextImage` 超时�?`continue` 立即重试形成忙循环——线程级采样显示单线程（Present�? 秒内消�?3125ms（≈78% 单核当量），UI 线程接近 0；最小化窗口后占用不变（Swapchain 仍被消费）�?
- 修复（第一层，最小修改）：`ChoosePresentMode` 改为 **FIFO（垂直同步）首�?*（遍历找 `FifoKhr`，Vulkan 规范保证必被支持；保留原安全回退），Mailbox 不再作为默认；不新增 UI 开关，不改 Swapchain 自愈流程，不清除 `_hasRenderProjection` 语义，无逐帧日志（启动日志保留创�?重建时一次「呈现模�?…」）�?
- 实测数据（本�?i5-10400F + RTX 3060 12GB + 144Hz 显示器；GPU 3D �?Windows 计数�?`\GPU Engine(*)\Utilization Percentage`（任务管理器同源），CPU 为进�?CPU 时间差单核当量，任务管理器进程页约等于该�?÷12）：基线（Mailbox）默认窗口静�?GPU 3D 19.4~21.3% / CPU 73~81%，最小化 GPU 17~21% / CPU 70~78%；FIFO 后默认窗口静�?GPU 3D 稳�?7~8%（峰�?19.4% 为启动过渡）/ CPU 稳�?10~11%（排除首样本初始化误差），最小化 GPU 7.6~8.1% / CPU 17~24%。按计划判定表「GPU 已持�?�?0% �?停止扩展」，FIFO 即最终方案，**未启用第二层 60 FPS 节流**。注：基�?GPU 数值与 F4 轮记录的 91~96%（任务管理器口径）存在口�?采样时机差异，本轮以修改前后同口径对比为准�?
- 测试：`VulkanPresentModeSelectionTests` 4 项（FIFO 优先/乱序优先/Mailbox 非默�?选择确定性）、`VulkanPresentLoopContractTests` 5 项（无投影受控等�?无逐帧日志/投影语义不变/模式日志只在创建重建/无新依赖），World 574�?83�?9）�?
- 验证：Core 334/334、World 583/583、WarCore 22/22 �?PASS；arch-a-guard PASS（依赖边�?5+100）；全解决方�?build 0 error；git diff --check PASS；地�?Shader/相机/Gizmo/输入/Swapchain 自愈/Avalonia UI 零改动；无新�?NuGet；临时采样脚本（Temp 目录 hermes-verify-*）不入库�?
- 遗留：① 最小化�?GPU/CPU 未降至近零（FIFO 后仍 ~8%/~20%）→ 后续窗口可见�?遮挡暂停轮（P1-A7）；�?显示�?144Hz �?FIFO 提交�?144fps，CPU 单核当量 ~10%，如需更低可启�?60 FPS deadline 节流（预留方向，本轮未启用）；③ Resize 多代际重建为既有问题，不属本轮�?
- 治理：版�?v0.2.24.28-fix �?v0.2.24.29-fix（四处同步）；未创建 Tag/Release�?

## v0.2.24.28-fix
MAP-A-R2-D3-F4 日志面板垂直尺寸自适应修复�?026-08-04 15:50:00，Commit 本轮落库为准�?
- 根因（A4 真机裁定）：日志区被裁切不是滚动问题而是**外部布局边界**——`UiRoot.axaml` Row3 日志�?`Auto+MaxHeight=420`（Auto 行优先按内容期望满额 420）与 Row1 主工作区 `*+MinHeight=320` 的最小和，加上工具栏与分隔条后超过矮窗口可用高度（约 1400×820 窗口可用�?~369 < 420）→ 日志区被窗口底部裁切，ScrollIntoView 只能控制内部滚动位置救不了外部边界；�?1032 高窗口可容纳（与截图矩阵 820 失败/1032 正常/最大化正常完全吻合）�?
- 修复（不再堆滚动算法）：`UiRoot.axaml.cs` 新增 `ClampLogRow()`——监�?`IsLogOpen`（DataContext PropertyChanged）与窗口 `SizeChanged`，日志展开时把 Row3 设为像素�?`Math.Clamp(420, 120, 可用高度)`（可�?= 窗口高度 �?�?Margin 24 �?分隔�?6 �?主区最�?320 �?工具栏实际高），折叠时回 `GridLength.Auto`（只占标题栏）；极端矮窗口可�?�? 时保持现状由 `MinHeight=32` 兜底。`Foot.axaml` 日志展开 Border `MinHeight=180 �?0`（解除矮窗口下阻止列�?Viewport 缩小的最低高度，改由外层像素行约束）�?
- 测试：`UiRootLogRowContractTests` 5 项（Row3 MaxHeight=420 存在/主区 MinHeight=320 存在/代码�?GridLength.Auto+Math.Clamp+IsLogOpen 自适应/日志 Border 不再 MinHeight=180/可优雅缩�?MinHeight=0）；几何级验证由 F4-A1~A8 真机复验承担（合同测试已注明）�?
- 验证：Core 334/334、World 574/574�?5）、WarCore 22/22 �?PASS；arch-a-guard PASS（依赖边�?5+100）；全解决方�?build 0 error；git diff --check PASS�?+100 手写复核（UiRoot.axaml.cs 81 / Foot.axaml 99 / 合同测试 52 行全合规）；无地图代�?中文文案/滚动策略主体改动�?
- 性能诊断（只读采样，独立轮处理，不进本轮提交）：PresentMode 优先 `MailboxKhr`（无 vsync 上限）；主循�?`RunFrames` 有投影时全�?Acquire→Submit→Present 无帧率限制（仅无投影�?`Thread.Sleep(16)`）；`_hasRenderProjection` 一旦为 true 不消费清�?�?空闲场景 GPU 91%~96% 根因与用户第一嫌疑吻合；建议独立性能轮：PresentMode �?FIFO 或主循环节流 60 FPS，再测空�?最小化/遮挡/网格开关采样矩阵�?
- 治理：版�?v0.2.24.27-fix �?v0.2.24.28-fix（四处同步）；未创建 Tag/Release�?

## v0.2.24.27-fix
MAP-A-R2-D3-F3 日志面板尾项完整显示修复�?026-08-04 15:10:00，Commit 本轮落库为准�?
- 真实尾部安全区：`Foot.axaml` 日志列表 ItemsPanel 改为 `VirtualizingStackPanel Margin="0,0,0,12"`�?2 DIP 进入滚动 Extent——Avalonia MeasureCore �?Margin 计入 DesiredSize，ScrollContentPresenter.ComputeExtent 基于内容 Bounds 计算），移除仅承担视觉间距、不进滚动范围的 ListBox `Padding="0,0,0,8"`；虚拟化保持（ItemsPanelTemplate 仍是 VirtualizingStackPanel，未退化为 StackPanel）�?
- 两阶段尾项定位：`LogListAutoScrollController` 重写并拆 partial（主文件 84 �?+ Follow.cs 61 �?+ Layout.cs 27 行）——唯一入口 `RequestLatestItemVisibility`（新日志/分类切换/清空/布局变化统一经过）；`_requestVersion` 请求合并（高频日志不堆积 Dispatcher 任务，旧请求执行时版本不一致即退出）；第一阶段 `ScrollIntoView(最后一�?`（Render 优先级）；第二阶�?`ContainerFromItem/ContainerFromIndex �?BringIntoView` + 读取最�?`Extent/Viewport` 修正 `Offset`（Background 优先级，`_tailCorrectionScheduled` 保证每请求最多一次，无递归无定时器）；修正保留水平偏移（`new Vector(Offset.X, maximumY)`），`_programmaticCorrection` 防止程序滚动被误判为用户滚动�?
- 阅读状态保持（计划 8.1 关键修正）：`_atTail` 只由用户滚动（OffsetDelta�?）维护——新日志增大 Extent 时不得用新最大滚动值重算（否则底部被误判为已离开、跟随失效）；ScrollChanged 集中处理 Extent/Viewport 变化（Resize/展开折叠/水平滚动条出�?DPI 重测），仅跟随态安排合并修正；清空日志取消旧请求并恢复跟随�?
- 测试：`FootAxamlTailContractTests` 3 项（AXAML 合同：虚拟化 ItemsPanel 保持/12 DIP 尾距/�?Padding 移除）、`LogListAutoScrollControllerContractTests` 9 项（控制器合同：最后一项为滚动目标/两阶�?Render+Background/读取最终滚动范�?第二阶段至多一�?保留水平偏移/请求合并失效/程序化修正保�?无递归定时�?�?EditorLogBus 引用）、`LogAutoScrollPolicyTests` 新增 1 项（阈值外 20.1 DIP 不跟随）；仓库无 Avalonia Headless 基础设施，几何级验证�?A4 真机承担（合同测试已注明）�?
- 验证：Core 334/334、World 569/569�?13）、WarCore 22/22 �?PASS；arch-a-guard PASS（依赖边�?5+100）；全解决方�?build 0 error；git diff --check PASS；无 Debug.WriteLine/EditorLogBus/地图代码/中文文案/新增依赖�?+100 手写复核 84/61/27/99/33/79/38 行全合规�?
- 治理：版�?v0.2.24.26-rz �?v0.2.24.27-fix（四处同步）；未创建 Tag/Release�?

## v0.2.24.26-rz
REPO-GOV-R1 目录职责分类与命名去里程碑化�?026-08-04 14:20:00，Commit 本轮落库为准�?
- 生产目录按职责拆分子目录（宪�?5+100 �?5 规则落地映射）：`Vm/` �?Camera/Map/Scene/Selection/Transform{Move,Rotate,Scale}/Logging/Inspector/History/Tree（根只留 UiVm.cs）；`Render.Vulkan/Render/` �?ClearFrame/Grid/Map/Scene/StaticModels/Present；`Editor/Assets/` �?Import/Gltf、Hosting{Planning,Transactions}、StaticModels、Catalog、Identity；`Core/Gizmo/` �?Common/Move/Rotate/Scale；移动不�?namespace（SDK-style csproj 自动包含，零代码改动）�?
- 测试镜像生产代码：`World.Tests/` 按领域目录（Map{Editing}/Scene/Spatial/Camera/Selection/Transform{Move,Rotate,Scale}/WorldPartition/Assets/Logging/Tree），`Core.Tests/Render/` �?Map/Grid/NavigationGizmo/StaticModels/DrawPlan/Camera；命名去里程碑化（`WorldCR4D3F1ValidatorTests` �?`StaticModelValidatorTests`，含 partial 切片后缀 .R4R2 �?职责后缀 .DragState/.ToolSwitch/.Preview/.AxisUniform，测试方法名 `R5R1_*` 一并清理）�?
- docs 只保留当前事实源�?84�?6 文件）：删除 closed/**、历史验�?审计/计划/报告、全部里程碑 SVG、superseded/**；仍有效结论并入 `ENGINE_ARCHITECTURE.md`（世界事�?坐标合同/编辑器边界）；`map-contract.md` 就位；`AGENTS.md` 仓库入口文件入库（索�?红线摘要，唯一权威仍为宪法）�?
- file-tree.md �?git ls-files 全量重建，每�?tracked 文件一句话职责�?30/730 全覆盖），无版本�?阶段�?职责索引�?
- 验证：Core 334/334、World 556/556、WarCore 22/22 �?PASS；arch-a-guard PASS（依赖边�?5+100）；--no-incremental 全量 0 error；git diff --check PASS；ad-hoc 16 项断言（file-tree↔ls-files 全路径一�?无里程碑前缀残留/目录根瘦�?docs 16 事实�?禁用�?路径合同）ALL PASS；代码行为零改动�?
- 治理：版�?v0.2.24.25-fix �?v0.2.24.26-rz（四处同步）；未创建 Tag/Release�?

## v0.2.24.25-fix
MAP-A-R2-D3-F2 日志中文化与日志面板尾部显示修复�?026-08-04 13:27:18，Commit 本轮落库为准�?
- 日志全部中文化（用户可见键名与状态值）：命�?宽度输入/深度输入/基础高度输入/地图标识/原尺�?原基础高度/候选尺�?候选基础高度/历史状�?变更序号/新尺�?新基础高度/可撤销/可重�?原因/序号/尺寸/基础高度/地表/错误类型/错误说明/当前尺寸/状态保持不�?处理/资源键已变化/地面顶点/索引/边界顶点/接收序号/已消费序号；内部枚举与错误码保持英文，显示映射集中在 `UiVm.MapDiagnostics.Format.cs`（FormatMapEditReason/FormatSurfaceKind/FormatErrorCode/FormatBoolean）与 `Render.Abstractions/MapSurfaceResourceUpdateText.cs`（三态决策中文，Vulkan 层无 UI 依赖引用），未反写任何领域类型�?
- 日志面板尾部显示修复：`LogListAutoScrollController` 重写为尾部跟随规则（纯策�?`LogAutoScrollPolicy`：距�?�?0 DIP 视为底部附近）——位于底部时新日志自动跟随、用户向上阅读旧日志不强制拉回、滚到底恢复跟随、切换日志分类（ForceFollow）定位最新、清空日志滚动范围归零自动回跟随态；列表底部�?8 DIP 安全间距（ListBox Padding），最后一行不再被底边裁切�?
- 测试：`UiMapLogChineseTests` 5 项（成功/失败/撤销日志中文键与状态值断言 + 无英文键断言 + 显示映射全覆盖）、`LogAutoScrollPolicyTests` 4 项（底部跟随/阈值内跟随/远离不跟�?无滚动范围恒跟随）�?
- 验证：Core 334/334、World 556/556�?9）、WarCore 22/22 �?PASS；arch-a-guard PASS（依赖边�?5+100）；--no-incremental 全量重编�?0 error（首次因真机编辑器进�?dll 锁报 MSB3021，进程退出后重跑 0 error�? 个既�?warning 如实记录）；git diff --check PASS；Shader/地图/历史/渲染/相机/Vulkan 生命周期零改动�?
- 治理：版�?v0.2.24.24-fix �?v0.2.24.25-fix（四处同步；file-tree 按新治理不含版本号，仅插入新增文件行）；未创�?Tag/Release�?
- 状态：**MAP-A-R2-D3-F2：已修复并落�?*；MAP-A-R2-D3-A3：NOT RUN；MAP-A-R2-D3：IN PROGRESS�?

## v0.2.24.24-fix
MAP-A-R2-D3-F1 地图面板真实命令路由与验收日志修复（2026-08-04 11:59:52，Commit 本轮落库为准�?
- 修复地图面板 RunCommand 未路由到地图编辑命令的问题：新增 `UiVm.MapCommandRouting`（TryRouteMapCommand 在通用兜底之前匹配新建/聚焦/应用属�?撤销/重做），面板按钮 �?RunCommand �?地图命令 �?MapSession 全链打通；`UiWin.RunMapCommand` 精简为仅快捷键可达的新建/聚焦（打开/保存/卸载分支移除）；修复「聚焦地图」未发布相机快照（FrameMapCamera �?PublishSceneRenderSnapshot，与 FrameSelectedCamera 同模式）�?
- 增加低频验收日志（复用既有日志总线，不建第二套 Logger）：地图命令收到、属性提交开�?成功/失败（含 Code/StateId/ChangeSequence/StateUnchanged）、撤销/重做成功/失败、渲染快照已发布（Reason/Sequence/Size/Surface）、Vulkan 资源更新决策（Recreate/NoRebuild/RejectStale 三态）、资源重建完成（顶点�?尺寸/BaseHeight/Sequence）；每帧/Hover/Getter 不记录�?
- 增加真实入口自动测试：`UiMapCommandRoutingTests` 8 项——从 `RunCommand.Execute("应用地图属�?/"撤销地图修改"/"重做地图修改")` 出发验证会话/快照/输入�?历史状态；非法尺寸�?NaN/Infinity/-Infinity 零污染；命令路由合同（新�?聚焦/未知命令兜底）；日志链含命令收到/提交开�?提交成功/快照发布�?
- 验证：Core 334/334、World 547/547�?8）、WarCore 22/22 �?PASS；arch-a-guard PASS（依赖边�?5+100）；--no-incremental 全量重编�?0 error�? 个既�?warning 如实记录）；git diff --check PASS；Shader 未修改�?
- 治理：版�?v0.2.24.23-rz �?v0.2.24.24-fix（四处同步；file-tree 按新治理不含版本号，仅插入新增文件行）；未创�?Tag/Release�?
- 状态：**MAP-A-R2-D3-F1：已修复并落�?*；MAP-A-R2-D3-A2：NOT RUN；MAP-A-R2-D3：IN PROGRESS�?

## v0.2.24.23-rz
MAP-A-R2-D3 A1 入口补接�?026-08-04 11:20:14，Commit 本轮落库为准�?
- 地图面板新增「撤销地图修改 / 重做地图修改」按钮：分别调用 `MapSession.Undo/Redo`（地图独立历史实例，不触碰场景实体历史；全局 Ctrl+Z 的焦点上下文规则未设计，D3 用显式按钮最安全）；`IsEnabled` 绑定 `CanUndo/CanRedo`（由 `HistoryAvailabilityChanged` 驱动刷新）；成功后同步宽/�?高文本与状态文字，渲染快照�?`ContentChanged` 自动更新（无历史时防御性报错，按钮禁用态下不可达）�?
- 测试：新�?`UiMapHistoryTests` 4 项（撤销/重做恢复三字�?+ 文本同步 + 按钮可用性翻�?+ World 查询随会话恢�?+ 快照经事件驱动更新）；Rename 不重建由自动测试负责（ResourceKey/策略），不进真机清单�?
- 治理登记（非阻断，待用户正式治理修订落库）：`file-tree.md` 退出版本号同步源，版本同步由五处调整为**四处**（changelog/run.bat/UiWin.axaml/UiVm.SceneDocument.cs）�?
- 已知限制登记�?*GentleHillsV1 地表视觉渲染尚未支持**（World 高度查询为起伏、画面为 Flat 平面）——D3 真机只验�?Flat；非 Flat 地形视觉渲染归后续地形轮，不得静默宣称显示正确�?
- 验证：Core 334/334、World 539/539�?4）、WarCore 22/22 �?PASS；arch-a-guard PASS�?-no-incremental 全量重编�?0 error�? 个既�?warning 如实记录）；git diff --check PASS�?
- 治理：版�?v0.2.24.22-rz �?v0.2.24.23-rz（四处同步；file-tree 按新治理不含版本号）；未创建 Tag/Release�?
- 状态：**MAP-A-R2-D3：等待真机验�?*；MAP-A-R2-D3-A1：NOT RUN�?

## v0.2.24.22-rz
MAP-A-R2-D3 A1 前收口（2026-08-04 10:51:34，Commit 本轮落库为准�?
- 修正地图 GPU 资源判等：新�?`MapSurfaceResourceKey`（MapId/尺寸/BaseHeight/地表参数/可见性，**不含 ChangeSequence**�? `MapSurfaceResourceUpdatePolicy` 纯策略（旧序号拒�?同键不重�?异键重建）；Vulkan `SetMapSurface` 改为策略驱动（`_lastConsumedMapSequence` 与资源键分离），Rename 等非几何变化不再重建地面与边界缓冲�?
- 地图属性改为单次原子提交：`MapEditSession.UpdateMapProperties`（一�?CommitMapChange = 单历史节�?单次 ChangeSequence/单次 ContentChanged），失败整体拒绝零污染（NaN/Infinity/尺寸越界/区域冲突）；UI「应用修改」只调用组合命令（删�?ResizeMap+SetBaseHeight 连续调用），单字段命令保留供未来 Inspector/自动�?API�?
- 默认首帧地图快照验证：新�?`UiMapInitialProjectionTests`——构造后首个 RenderProjection 即携�?10 km×10 km Flat 默认地图快照，无需新建地图�?
- 重建 `file-tree.md`：从 `git ls-files` 全量生成当前树（882 个跟踪文件全覆盖、零缺失、零重复），删除全部版本化「职责索引」与迁移记录；历史仅保留�?changelog�?
- 验证：Core 334/334�?13 资源�?策略）、World 535/535�?11 原子提交/首帧投影/错误消息同步）、WarCore 22/22 �?PASS；arch-a-guard PASS（依赖边�?5+100）；--no-incremental 全量重编�?0 error�? 个既�?warning 如实记录）；git diff --check PASS；Shader 本轮未修改，字节�?--verify 复验一致未污染�?
- 治理：版�?v0.2.24.21-rz �?v0.2.24.22-rz（五处同步）；未创建 Tag/Release�?
- 状态：**MAP-A-R2-D3：等待真机验�?*；MAP-A-R2-D3-A1：NOT RUN�?

## v0.2.24.21-rz
MAP-A-R2-D3 有限地图地面、边界与渲染快照�?026-08-04 10:24:50，Commit 本轮落库为准�?
- **渲染唯一输入**：`MapRenderSnapshot` 迁至 `XuanYu.Render.Abstractions`（MapId/尺寸/地表/BaseHeight/Seed/SourceChangeSequence/IsVisible + Min/Max�?*�?Name**——Rename 不引�?GPU 资源重建）；�?`MapRenderSnapshotProjection`（Editor.UI）从 `MapEditSession.CurrentMap` 投影，首次组装生成初始快照，仅响�?`ContentChanged` 低频事件（相�?Hover/选择不重建）；ChangeSequence 单调去重，禁�?Render 自增�?
- **有限地面常量几何**：`MapSurfaceGeometryBuilder`（Render.Abstractions）Flat 地面固定 **4 顶点 6 索引**（左下→右下→右上→左上，Z=BaseHeight�?0 km/20 km/百万米均�?4 顶点，尺寸只进顶点坐标）；删除按米细分的 `MapTerrainMeshBuilder`�?225 顶点，C 类退役）�?`MapBoundsMeshBuilder`�?8 顶点）；�?`MapBoundsGeometryBuilder` 四条边细条四边形 **24 顶点**（世界宽�?clamp(尺寸×0.001, 1, 50) �?+ 渲染抬升 0.05，真机验证远近后决定是否屏幕恒宽 Pass）�?
- **Vulkan 绘制**：`VulkanClearFrameOwner.MapTerrain.cs` �?`MapSurface.cs`（`SetMapSurface`，值相等去重；地面索引 draw kind -14 + 边界 -15 分支复用）；`scene.vert` 地表基色土绿 �?**低饱和豆青灰 (0.52,0.60,0.55)**、边界亮琥珀 �?**淡金�?(0.85,0.76,0.55)**（glslc 重生�?ShaderBytecode.Vert.cs 7430 �?GridFrag.cs 1315 词，--verify 逐字一致）�?
- **网格对齐地图**（计�?9.1）：参考网�?Pass push 176B �?**192B**（新�?vec4 mapBounds：半�?半深/BaseHeight/边缘淡出宽度=min(尺寸)×0.08）；`editor_reference_grid.frag` 求交平面 Z=0 �?**Z=BaseHeight** + 地图矩形外平滑淡出（无地�?w=0 保持无限网格）；ViewPlaneGrid 共用 FillGridPushConstants 且自行覆�?[44..47]，F3-F4 正交视图不受影响�?
- **权威统一**：UiVm 渲染数据�?`MapWorld.BuildRenderSnapshot()`（R1 旧链）→ `_mapRenderSnapshot`（会话直出）；`MapDocumentWorldBridge` 退役，新增 `WorldMapState.From(MapDefinition)`（World 同层投影，环境默�?ClearDay）；`MapDocumentAggregateBridge`（v1 DTO �?聚合，场�?mapReference 保活链：加载→投影→`ReplaceCurrentMap(markSaved:true)`，D2 预留入口）；**保存/打开按钮禁用**（v1 DTO 双权威分叉风险，持久�?D6 接入）；"卸载地图"按钮/命令移除（D2 会话语义=恒有默认地图）；场景引用失效时显示错�?+ 会话默认地图保持（非 R1"未加�?空状态）�?
- **真机入口**（计划目�?3）：地图编辑器面�?基础地表"区升级为**地图属�?*区——宽�?深度/基础高度编辑�?+ 应用修改（全解析合法�?`ResizeMap`+`SetBaseHeight` 提交，非法输入中文错�?不产生历�?不部分更新）+ 地表类型 Flat 只读；聚焦地图复�?`FrameMapAllWithCenter`�?*已含动�?Far**=max(100, distance+depth×4)，Near 0.05�?0% 占用率，正交�?F3-F4 兼容），角点 Z �?BaseHeight�?
- 验证：新�?MapSurfaceGeometryTests�? 项：4/6 常量、坐标对称、BaseHeight、边�?24 顶点/宽度公式/抬升�? MapRenderSnapshotProjectionTests�? 项：默认/Resize/地表/会话驱动/Rename 不重建）+ UiMapEditorTests 重写 7 项（会话默认地图/应用/非法/非数�?取景 Far�? SceneMapReferenceTests 适配 4 �?+ MapDocumentAggregateBridgeTests 5 项；Core 321/321�?12�?21）、World 524/524�?28�?24，删 2 文件+�?4 用例）、WarCore 22/22 �?PASS；arch-a-guard PASS（含依赖边界+5+100）；--no-incremental 全量重编�?0 error�? 个既�?warning 如实记录）；git diff --check PASS�?
- 治理：版�?v0.2.24.20-rz �?v0.2.24.21-rz（五处同步）；登记：百万米地图远距深度精度（~17 m@85 km）为后续大世界问题；边界屏幕恒宽待真机验证后决定；未创建 Tag/Release�?
- 状态：**MAP-A-R2-D3：等待真机验�?*（自动测试通过 �?COMPLETE）�?

## v0.2.24.20-rz
MAP-A-R2-D2 地图编辑会话与状态权威（2026-08-03，Commit 本轮落库为准�?
- **D1 遗留小修**：`MapDefinition` 移除 `Revision`（领域聚合纯净不可变，版本/游标语义由编辑会话持有）；`MapLayerKind`/`MapRegionKind` 枚举值合法性检查（`Enum.IsDefined`，未知角色不得默认为可承载层�? UnknownLayerKindRejected/UnknownRegionKindRejected 测试�?
- **历史方案（审计裁定：方案 A 直接复用�?*：现�?`EditorHistoryOwner` 已通用（PushEntry(object)/TryUndoAny/TryRedoAny/CurrentRevision Undo 回退/新编辑清 Redo 分支），地图直接复用同一 Core 实现（独立实例），不建第二套历史系统；CurrentStateId=历史游标（可回退旧节点）、ChangeSequence=单调递增（事�?去重，不可回退）、SavedStateId=保存点；IsDirty=路径�?�?保存点空 �?状态不一致（�?Undo/Redo 回到保存点）�?
- **MapEditSession（XuanYu.Editor/MapEditing/�?1 文件�?*：唯一地图权威 CurrentMap: MapDefinition；统一 `CommitMapChange` 管线（纯修改→No-op 检测→领域校验→记录历史→替换，失败零污染）；命令 RenameMap/ResizeMap/SetBaseHeight/CreateNewMap/ReplaceCurrentMap/MarkSaved；Undo/Redo/分支清除；选择（None/Map/Layer/Region 只存稳定 ID + 变更后规范化）；低频事件（ContentChanged/SelectionChanged/DirtyChanged/HistoryAvailabilityChanged）；写线程保护（注入 `Func<bool>`，复用现有判断器）�?
- **错误合同**：复�?Core.EngineResult/EngineError（未新建 MapEditResult 等容器）；错误码 NotOnWriteThread/InvalidMapName/InvalidMapSize/RegionWouldBeOutOfBounds（缩小致区域越界整体拒绝，不裁剪不移动）/NoUndoAvailable/NoRedoAvailable/UnknownLayer/UnknownRegion；No-op=成功且无状态变化�?
- **组装**：UiVm 增加 `MapSession`（同一写线程判断器注入，headless 测试兼容）；�?Vulkan/UI 视觉改动�?
- 验证：新�?MapEditSession 测试 7 文件 36 用例（创�?命令/历史/Dirty/选择/验证/线程）；Core 312/312、World 528/528�?90�?28）、WarCore 22/22 �?PASS；arch-a-guard PASS�?+100 全合规（守卫 ReadAllLines 口径：UiVm 压至 99、SelectionTests 100）；--no-incremental 全量重编�?0 error�? 个既�?warning 如实记录）�?
- 治理：版�?v0.2.24.19-rz �?v0.2.24.20-rz（五处同步）；未创建 Tag/Release�?

## v0.2.24.19-rz
MAP-A-R2-D1-F1 架构与领域合同修正（REVISE 裁定）（2026-08-03，Commit 本轮落库为准�?
- **审查裁定**：D1 三个阻断问题（领域权威层错误/无完整地图聚�?区域合法性过弱）全部修复；版本后缀修正�?-rz（正常开发轮，F 修复轮才�?-fix）�?
- **F1-1 架构边界恢复**：图�?区域/边界/验证/聚合全部迁至 `XuanYu.World/Map/`（地图权威层，World 仅依�?Core）；Editor 删除 12 个迁移文件，`MapDocument` 回归�?DTO�?xymap v1 持久化模型）；`MapId` 同步迁移（`SceneDocument/MapReference.cs` �?`SceneDocumentValidator.MapReference.cs` 引用修复）；Editor 仅保�?DTO/JSON/存储/校验与桥接�?
- **F1-2 完整地图聚合**：新�?`MapDefinition`（MapId/DisplayName/尺寸/坐标系统/地表/图层/区域/Revision）为唯一权威根；`MapDefaultDefinition.CreateDefault()` 一次创建完整地图（10 km × 10 km Flat + 基础地图�?+ 区域�?+ 空区域）；D2 �?CurrentMap/Undo/Dirty 围绕单一聚合；持久化 schema v2 仍属 D6�?
- **F1-3 领域合法性收�?*：`MapRegion` 移除 `IsClosed`（正式区域天然闭合，顶点不重复保存首尾）+ 新增 `MapRegionDraft`（绘制中草稿，CanClose/Close 提交）；`MapLayerKind`（Base/Region/Custom）稳定角色标识替代中文名；新增检查：ID 合法性（MapId/LayerId/RegionId）、图�?Order 唯一、基础层必须且仅有一个且位于�?0 位、区域不得挂�?Base 层、相邻重复点（含首尾）、至少三个不同顶点、非零面积（鞋带公式，共线三点拒绝）；自交检测明确归 D5（绘制轮），不在 F1 默默放行�?
- **修正内部缺陷**：`MapDefinitionValidator` �?`??` 短路 bug（MapValidationResult 为引用类型，Ok() �?null 导致区域验证永不执行）→ 显式 if 链；`MapLayerValidator` 基础层先查唯一性再查顺序�?
- 验证：新�?MapDefinitionTests/MapRegionDraftTests/MapLayerTests.Base + 区域严格性用例；Core 312/312、World 490/490�?70�?90）、WarCore 22/22 �?PASS；arch-a-guard PASS�?+100 全合规（MapDocumentValidator 压缩�?99 行、MapRegionTests �?Strictness 分部）；--no-incremental 全量重编�?0 error�? 个既�?warning 如实记录）�?
- 治理：版�?v0.2.24.18-fix �?v0.2.24.19-rz（五处同步；后缀修正为正常开发轮）；未创�?Tag/Release�?

## v0.2.24.18-fix
MAP-A-R2-D1 地图领域合同：图�?区域模型与验证（2026-08-03，Commit 本轮落库为准�?
- **R2 范围裁定**：R1 后半（地图尺�?边界/地表/区域/图层/保存）按用户纠正转入 MAP-A-R2；`.xymap` ZIP 封装�?DGD 衔接整体后移，不抢在地图本体之前开发�?
- **稳定 ID**：新�?`MapLayerId`/`MapRegionId`�?2 位十六进制，�?MapId 同族）；名称可改、ID 不变，不依赖列表序号/UI 索引�?
- **图层领域模型**：`MapLayer`（ID/名称/顺序/可见/锁定/固定层）+ `MapDefaultLayers` 默认工厂�?基础地图"固定�?+ "区域"层）�?
- **区域领域模型**：`MapRegion`（ID/所属图�?类型/顶点/闭合�? `MapRegionKind`（Generic/Playable/Restricted/Deployment/Objective）；顶点只存水平�?X/Y（沿用已冻结 Z-Up 合同，高度由地表采样取得）�?
- **边界合同**：`MapBounds` 中心原点闭区间（X/Y �?[-W/2, W/2]），�?`WorldMapState.Contains` 语义一致�?
- **验证�?*：`MapLayerValidator`（ID 唯一/名称非空/顺序非负/固定层至多一个）、`MapRegionValidator`（闭�?�? 顶点/�?024 顶点/引用图层存在/有限数�?边界内），结构化结果不抛来源不明异常�?
- **默认工厂**：`MapDocument.CreateDefault()`�?未命名地�? 10000×10000 Flat）；`CreateNew` 默认值按 R2 合同调整�?0000×10000、`DefaultFlat`）；最大尺�?10000 �?1000000（R2 测试 02 需 20000，上限仅为输入保护）�?
- 分层裁定：Layers/Regions 本轮为独立领域模型，不挂�?MapDocument�?xymap v1 强制 layerReferences 空数组，schema v2 升级�?D6）；�?Editor.UI/Vulkan 依赖�?
- 验证：新�?MapBoundsTests/MapLayerTests/MapRegionTests(+Helpers)/MapDefaultMapTests 4 文件 35 用例；Core 312/312、World 470/470�?35�?70）、WarCore 22/22 �?PASS；arch-a-guard PASS�?+100 全合规；`--no-incremental` 全量重编�?0 error�? 个既�?warning 如实记录：xUnit2000/xUnit2013/CS8602）�?
- 治理：版�?v0.2.24.17-fix �?v0.2.24.18-fix（五处同步）；F3-F4 视觉冒烟仍待用户真机验收；未创建 Tag/Release�?

## v0.2.24.17-fix
MAP-A-R1-D5-R1-F3-F4 正交投影 + 视图平面网格�?026-08-03，Commit 本轮落库为准�?
- **F3-F3 正式 PASS/CLOSED**：用户补充真机验收事实（此前未同步），本轮不重验、不补跑�?
- **正交投影链路**：`CameraState` 新增 `ProjectionMode`（Perspective/Orthographic�? `OrthographicScale`�?0 校验）；`ViewProjectionState.Create` 正交矩阵分支（`CreateOrthographic`，与透视同族深度 [0,1]）；`RenderCameraProjection` 透传模式；世界射线（`WorldRayFactory` �?VP 反投影）正交下自动成立�?
- **标准视图正交化（用户冻结语义�?*：六方向标准视图（±X/±Y/±Z）自动进入正交投影（正交尺度=当前透视可见高度，切换视觉连续）；自由环绕（Orbit）从正交视图开始自动恢复透视并退出标准视图；正交 Dolly=缩放 OrthographicScale（禁距离模拟缩放）；正交 Pan 保持正交（每像素世界距离=尺度/视口高）；正交取景（FrameAll/Selected 保持正交，尺度按包围范围适配）�?
- **视图平面网格（用户冻结语义）**：±X→YZ 平面、±Y→XZ 平面（世界原点基准、自适应间距/LOD、屏幕恒定线宽、深度偏移、距离淡出）；±Z 复用现有地面网格（Z=0 �?XY 平面）；独立 Pass（复�?GridVert + �?`editor_view_plane_grid.frag` + 192B PushConstant 含平面法线；glslc 字节码生成前完成工具链一致性复�?MATCH）；`RenderDrawKind.EditorViewPlaneGrid` + DrawPlan 条目（启用时替代地面网格）�?
- **正交配套**：Gizmo 屏幕尺寸正交分支（Move/Rotate/Scale：worldHeight=OrthographicScale 恒定）；网格 LOD 采样正交化（wmpp=尺度/视口高解析式，规避侧视正交射线求交退化）�?
- 验证：新�?CameraOrthographicTests 7 + CameraOrthographicNavigationTests 5 + ViewPlaneGridFor 映射 7 + DrawPlan 2；Core 312/312、World 435/435、WarCore 22/22 �?PASS；arch-a-guard PASS�?+100 全合规（Draw.cs �?PipelineBind 分部、RenderDrawPlan 枚举压缩）；glslc 工具链一致性复�?MATCH�?
- 视觉冒烟�?*未执�?*（本环境无法操作画面，留真机验收）；请用户重点复验：六方向正交视图、正交滚轮缩放（尺度）、Orbit 恢复透视、±X/±Y 视图平面网格、±Z 地面网格�?
- 治理：版�?v0.2.24.16-fix �?v0.2.24.17-fix（五处同步）；F3-F3 正式 CLOSED；未创建 Tag/Release�?

## v0.2.24.16-fix
MAP-A-R1-D5-R1-F3-F3 Blender 风格导航视图收尾�?026-08-03，Commit 本轮落库为准�?
- F3-F2 真机复验：相机崩溃修复基本通过�?*普�?Orbit 地平线滚�?FAIL + 导航 Gizmo 视觉 FAIL + 侧视表现体验 FAIL**（数学正确，但缺正交视图/视图平面网格）�?
- 根因（本轮源码确认）：F3-F2 �?TryOrbit �?start.Up �?PreferredUp——顶视（Up=+Y）后 Orbit 继承 +Y，画面整体转 90°（Roll）；Gizmo �?88 DIP 调试图形（六端点全绘无正对处理、轴线穿过中心球、标签层级弱）�?
- 修复（本轮目�?1/2；目�?3 正交投影审计为无 �?�?F3-F4）：
  - **�?Roll Orbit**：TryOrbit 改用世界 +Z 重建基（Right=Forward×WorldUp、Up=Right×Forward），�?底视平行�?CameraBasis 自动回退最不平行世界轴�?Y/+X），Up 永不下翻、连续环绕不累积倾斜；Dolly/Pan 保留 start.Up 语义；删除无调用点的 Result 死代码；
  - **Gizmo 视觉重做（Blender 结构�?*：控�?88�?6 DIP、边�?12�?4、轴投影 25�?7、负端点 5.5�?；七层绘制（后轴→后端点→中心球→前轴→前端点→标签→Hover 环）；轴线从中心球边缘开始（不穿过球）；轴正对相机（投影 <6 DIP）时隐藏背向端点与轴线、朝向端点置于中心球中央；标签仅正方向且朝向时显示（11 DIP 半粗）；新配�?X #C4874F / Y #5684A8 / Z #8EA8C2、球 #D7DEE6、描�?#66788B、背�?30% Alpha；Hover 亮环；editor_nav_gizmo.frag 重写并经 glslc 重新生成 ShaderBytecode.NavGizmoFrag.cs�?20 �?行，38 行）�?
  - **Gizmo 视觉终版（用户直接提�?shader，同轮替换）**：保�?80B Push/96 DIP/14 边距/CPU 命中热区 13 DIP 不变，视觉半径与命中半径分离——中心球缩至 9.5 DIP 轻量球（径向渐变+左上高光+细描边，不再是大白圆盘）；轴�?1.25 DIP、从球边缘开始；端点�?7.5/�?3.8/正对 8.5 DIP；正对相机只保留一个前方端点；X/Y/Z 文字绘制在端点圆内部（含正对负方向的轴字母）；五层合成（背向�?端点→中心球→朝向轴/端点→端点内部标签→单一 Hover 环），预乘合成适配 SrcAlpha 混合；新配色 X #C66A5E 珊瑚�?/ Y #6B9F84 豆青 / Z #628EC2 钢蓝；editor_nav_gizmo.frag �?glslc 重新生成 ShaderBytecode.NavGizmoFrag.cs�?20 �?行，58 行）；OverlayContractTests 合同断言同步至新结构；NavigationGizmoLayout/HitTest/相机/DrawPlan/Pipeline 零改动；
  - **正交投影/视图平面网格**：审计确认仓库无 Orthographic（仅透视 FOV）→ 按计划冻结为 F3-F4�?
- 验证：新�?CameraNavigationRollTests（斜�?Orbit �?Up 保持 +Z 主导且无水平横移�?00 次环绕不累积倾斜、顶/底视 Orbit 稳定�? Gizmo 正对合同测试�?Facing.cs）；红→绿：修正 4 处测试期望（均为合同变更/测试数据错误�?5° 俯角 fallback +Y 是稳定态、斜�?up 自然倾斜、顶视基测试数据）；Core 全量 291/291、World 435/435、WarCore 22/22�?
- 视觉冒烟�?*未执�?*（本环境无法操作画面），如实记录——自动测试通过、真机待用户验收；不宣布 F3-F3 视觉通过、不关闭阶段�?
- 治理：版�?v0.2.24.15-fix �?v0.2.24.16-fix（五处同步）；新�?CameraNavigationRollTests.cs/NavigationGizmoLayoutTests.Facing.cs；CameraState 严格合同未放宽；未创�?Tag/Release�?

## v0.2.24.15-fix
MAP-A-R1-D5-R1-F3-F2 相机正交基不变量与导航组合链崩溃修复�?026-08-03，Commit 本轮落库为准�?
- F3-F1 真机验收�?*FAIL**。故障：滚轮 Dolly 构�?CameraState �?up 非法，ArgumentOutOfRangeException 逃出 Win32 消息循环，编辑器进程退出�?
- 根因（失败测试先�?+ 源码确认，非计划推测）：`CameraNavigation.Result()` 硬编�?`Up=Vector3d.UnitZ` 拼接�?Forward——点击导�?Gizmo 顶视（Forward=-Z�?底视（Forward=+Z）后，任�?Dolly/Orbit/Pan 都令 Forward �?UnitZ 平行，触�?CameraState �?24 行合同（`Forward.Cross(up).Length<1e-6` 抛异常）。另确认两项伴随缺陷：标准视角命令未同步 `_observationCenter`；底�?Up=+Y 导致屏幕右方向为 -X（镜像）�?
- 修复：新�?`XuanYu.Editor/Camera/CameraBasis.cs`（唯一正交基生成器：Forward 有限非零 �?PreferredUp 优先（|dot|<0.98）→ 平行时回退世界�?+Z/+Y/+X 最不平行�?�?Right=Forward×Ref、Up=Right×Forward，输出前正交验证；不进入 Core）；`CameraNavigation` �?`CameraNavigation.Try.cs`，TryDolly/TryOrbit/TryPan/TryResult 统一�?CameraBasis（PreferredUp=start.Up 保留 Up 语义），同步�?API 保留；UiVm 失败安全：Try* 成功才替换相�?中心/Revision，失败保留旧状态并记录「相�?Dolly 失败」错误日志，异常不再逃出输入循环；标准视角同�?`_observationCenter`；底�?Up 修正�?`-Y`（Right 保持 +X，防镜像，计划八合同）�?
- 验证：新�?CameraBasisTests 9 项（�?NaN/平行/顶底�?重合失败/超大坐标正交�? CameraNavigationSequenceTests 11 项（六方向后 Dolly/Orbit/Pan 链）+ CameraNavigationUiSequenceTests 8 项（顶视→Orbit→Pan→Dolly、底视→Resize→Dolly、Gizmo Commit/Cancel �?Dolly、失败保留状态、不 Dirty/Undo�? CameraNavigationStressTests�?00 次循环正交保持）；红→绿：修复前 9 �?FAIL（含崩溃复现），修复后聚�?49/49；Core 相机/视口/Gizmo 151/151、World 435/435、Core 全量 267/267�?
- 视觉冒烟�?*未执�?*（本环境无法操作画面），按计划如实记录——自动测试通过、真机待用户验收；不宣布 F3-F2 视觉通过、不关闭阶段�?
- 治理：版�?v0.2.24.14-fix �?v0.2.24.15-fix（五处同步）；新�?CameraBasis.cs/CameraNavigation.Try.cs/4 个测试文件（file-tree 已登记）；CameraState 严格合同未放宽；未创�?Tag/Release�?

## v0.2.24.14-fix
SHR-2026-08-R2 全盘阶段考核：文档事实源审计�?docs 分类治理�?026-08-03，Commit 本轮落库为准�?
- **file-tree.md 重建**�?85�?43 行）：删除全部按轮次职责索引（宪法第五十五条禁止的每轮快照）；以真实 `git ls-files` 树为准重建，修正 ARCH-WORLD 迁移后失效路径（Scene/World/History 等不再误�?Core），补齐 WarCore/Map/Assets/StaticModel/F2-F3 新文件职责，全部条目一行职责、无历史流水账�?
- **changelog 审计**：条目守�?310�?96 归档 + 14 当前）；审计发现 7 月归�?3 处同一版本号分配给两个不同轮次的历史缺陷（v0.2.16.2-rz/v0.2.17.8-rz/v0.2.20.19-fix）与 18 处版本号-日期非单调——按不篡改历史原则登记注记不重写；归档结构调整为 `docs/archive/changelog/` 子目录�?
- **docs 分类迁移**：根目录 178 文件 �?3 个入口（+docs-index.md 新增）；建立 governance（版�?债务/审计）、architecture（引擎架�?坐标合同）、milestones/current/MAP-A、milestones/closed/{ARCH-A,ARCH-B,ARCH-C,ARCH-WORLD,WORLD-A,WORLD-B,WORLD-C,RZ-VK,M1}、archive/{changelog,superseded} 分类；全�?`git mv` 保留历史；修�?4 处失效路径引用（dev-rules×2、map 合同、债务登记、changelog 活跃条目×2）�?
- **代码架构语义审计**：依赖图与宪�?arch-a-guard 一致（Core 零依赖、Editor.UI �?Vulkan、App=组合根、Win=宿主）；Core 无地�?经纬/国家语义；无第二�?EntityRegistry/空间索引；地图文�?MapDocumentOwner(Editor)、运行时=WorldMapStateOwner(World)、渲染只消费 MapRenderSnapshot；高频路径无日志/全量扫描。结论无 BLOCKER�? 观察项（地图元素尚未实体化须�?EntityId；地图文档编辑入 Undo 链待正式地图编辑器明确）�?
- 验证：D6 交叉核对 10 问全 PASS（路�?归档/分类/守恒/残留/重复/版本）；正式串行门禁见本轮报告�?
- 治理：版�?v0.2.24.13-fix �?v0.2.24.14-fix（五处同步）；未创建 Tag/Release；SHR-2026-08 重新 CLOSED�?

## v0.2.24.13-fix
SHR-2026-08 阶段健康考核与治理收敛（2026-08-03，Commit 本轮落库为准�?
- P0：宪�?2.0 独立入库（`docs: 生效玄域引擎AI开发宪�?.0`，b99e087，消除双事实源）；修�?arch-a-guard 5+100 行数统计漏检（PS 5.1 `Measure-Object -Line` 实测失真 109�?6，改�?`[System.IO.File]::ReadAllLines` 确定性统�?+ 8 样本门禁自验证，检查范围对齐宪法第十三�?.cs/.axaml/.js）；治理 3 个超限文件（WorldRotateTransformUiTests.R4R2.cs 109�?6+Helpers 52、WorldToolStateHighlightUiTests.cs 105�?5+Selection 26、Left.axaml 101�?9+Left.Styles.axaml 16，真实拆分不压行）�?
- P1�?0 �?catch 逐处分类治理（B 类清�?best-effort 语义注释 3 处、C �?UI 生命周期竞态注�?1 处、D �?Gizmo 投影退化类型化+回退语义 3 处，�?3 处复核为正常业务处理不变）；dev-rules §17 失效"宪法第二十八�?引用改条款号+标题、版本规�?宪法第十六章"改第四十二条《版本一致性》、Editor.App=组合�?Editor.Win=Windows 平台宿主职责描述修正；changelog 月度归档 5/6/7 �?�?`docs/archive/changelog-2026-{05,06,07}.md`�?436�?00 行，含归档规�?历史索引）�?
- P2：docs 分类框架落地（docs/archive/ 历史归档分类）；其余�?175 个历史文档平铺分类登记为渐进治理事项（每月一个逻辑簇，不阻�?MAP-A）�?
- 验证：World.Tests/Editor.UI 快速编�?0 错误；修复后 arch-a-guard 全量 PASS（含自验证，此前同门禁对 3 个超限文件误�?PASS）；正式串行门禁见本轮最终报告�?
- 治理：版�?v0.2.24.12-fix �?v0.2.24.13-fix（五处同步）；未创建 Tag/Release；`IDEA.md` 已删除（无有效内容）�?

## v0.2.24.12-fix
MAP-A-R1-D5-R1-F3-F1 世界原点屏幕空间标记 + 导航 Gizmo 移入 Vulkan Overlay Pass�?026-08-03 16:10:00，Commit 本轮落库为准�?
- F3-A1（v0.2.24.11-fix）：**FAIL**。用户真机验收：
  1. 世界原点退化为黄色地面面片（旧实现�?Z=0 世界空间面片，低角度透视被压扁成梯形）；
  2. 导航 Gizmo 真机零像素（Avalonia 覆盖层被 NativeControlHost 承载�?WS_CHILD 原生子窗口遮挡——airspace 问题，ZIndex/Margin/Opacity 均无效）�?
- 修复（本版本，按用户指定方向——先调查层级后实现，不再�?XAML）：
  - **F3-F1-A 世界原点重写**（editor_world_origin.frag）：去掉射线求交与贴地投影；改为世界原点 (0,0,0) 投影到屏幕后�?*恒定屏幕尺寸**的细十字�?+ 小空心圆 + 中心点（蓝灰描边 #718096、中心淡金褐�?#C18A55、十字半�?8px/圆环半径 5px�?0~16 DIP）；相机后方/屏幕�?discard；深度保持原点平面深度（实体近则自然遮挡）；不再随视角压扁、不与地平线混同�?
  - **F3-F1-B 导航 Gizmo �?Vulkan 屏幕空间 Overlay Pass**：新�?editor_nav_gizmo.vert/.frag + ShaderBytecode.NavGizmoVert/Frag；新�?VulkanClearFrameOwner.NavGizmo.cs�?0B push：cameraRight/Up/Forward + 视口 + DPI + gizmo 参数 + hover 索引）；CreateFullscreenPass 增加 depthTest 参数（Gizmo �?DepthTest=Off/DepthWrite=Off）；GridPipelineSet 增加 NavGizmo 管线；DrawPlan 恒以 NavigationGizmo 收尾（RenderDrawKind 新增）；右上�?12 DIP 边距 88 DIP 区域；中心球 #CDD6DF + 三轴（X #C18A55/Y #5F87A7/Z #A9B8C7�? 六端点（背向 40% Alpha 小点、朝�?100% 大点�?X/Y/Z 标签�? 深度排序 + hover 高亮�?
  - **F3-F1-C 命中走原生指针流**：Avalonia ViewGizmo/ViewNavigationGizmo 控件删除（UiRoot 移除引用）；VulkanNativeHost.NavGizmo.cs �?OnNativePointerMessage 中先判右上角区域（视口→Gizmo 局部坐标），端点点�?�?StandardViewResolver 标准视角命令，中心球/空白拖动 �?复用 UiVm 相机会话 Orbit�? DIP 阈值区分点�?拖动）；CaptureLost/取消正常结束；控件区域外不截获（实体 Picking/框�?变换 Gizmo 不受影响）；导航不进�?Dirty/Undo�?
  - DPI 链路：RenderProjection 增加 ViewportDpiScale；UiVm.UpdateViewportDpi（LayoutSync 调用）；RenderCameraProjection 增加 Right 计算属性�?
- 验证：聚�?NavigationGizmo/StandardViewResolver/ViewportChrome/OverlayContract 33/33；Core 258/258、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；git diff --check OK；glslc 字节码三文件逐字 MATCH�?
- 视觉冒烟�?*未执�?*（沿用用户决定，留真机验收）；请重点复验：原点不再贴地压扁（十字+空心�?中心点）、右上角 Gizmo 可见且随相机旋转、六方向点击/拖动、顶底视图无滚转�?
- 治理：版�?v0.2.24.11-fix �?v0.2.24.12-fix（五处同步）；无新增依赖/项目；不创建 Tag/Release�?

## v0.2.24.11-fix
MAP-A-R1-D5-R1-F3 视口黑边移除 + Blender 风格导航 Gizmo�?026-08-03 15:20:00，Commit 本轮落库为准�?
- F3 问题（用户验收反馈）�?
  1. 视口外层存在黑色粗边框和厚重圆角（两层深色容器：VulkanViewport.axaml `#0b1220`/`#31405d` + UiRoot 中央 `#101827`/Padding=18/圆角8/BoxShadow）；
  2. 右上角仍是白色占位块（ViewGizmo.axaml 3×3 按钮 + `#dce6f2` 圆角卡片），缺少正式导航 Gizmo�?
- 修复（本版本）：
  - **F3-D1 去黑�?*：VulkanViewport �?UiRoot 中央容器改为浅灰 1 DIP 分隔（`#C9D2DC`）、无圆角、无 Padding、无深色背景、无 BoxShadow；Fallback 层改浅色 `#E8EEF5`；ClipToBounds 保留�?
  - **F3-D2 Blender 风格导航 Gizmo**：替换白色占位为透明 88×88 覆盖层（右上 12 DIP）——中心球（`#CDD6DF`/描边 `#718096`�? 三根世界�?+ 六正负端�?+ X/Y/Z 标签；玄域低饱和配色（X `#C18A55` 淡金褐、Y `#5F87A7` 蓝灰、Z `#A9B8C7` 浅钢灰）；背向端�?40% Alpha 小圆点、侧�?78%、朝�?100% 大端点带标签；按深度升序绘制（背向先、朝向后）；轴正对相机时端点收缩中心�?NaN；控件完全透明无底板；
  - **F3-D3 交互**：点击六端点 �?标准视角命令�?X/-X/+Y/-Y/�?底视图，保留 Pivot 与距离，Up 合同：±X/±Y=+Z、顶/�?+Y 防滚转）；中心球/空白拖动 �?复用 UiVm 相机会话 Orbit（同一灵敏�?俯仰限制/Pivot）；点击/拖动阈�?4 DIP；Hover 亮环 + Hand 光标；PointerCaptureLost 正常取消；控�?88×88 外不截获输入（实�?Picking/框�?变换 Gizmo 不受影响）；导航不进�?Dirty/Undo/场景文件�?
  - 拆分职责�?+100）：ViewNavigationGizmo.cs（属性状态）/ .Layout.cs（投影纯数学�? .Render.cs（绘制）/ .HitTest.cs（命中）/ .Input.cs（输入命令）；StandardViewResolver.cs（六方向解析 + 端点名映射）；UiVm.NavigationCamera 快照（相机变化统一通知 Gizmo）�?
- 验证：聚�?NavigationGizmo/StandardViewResolver/ViewportChrome 29/29；Core 254/254、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；git diff --check OK；XAML 加载由构建编译验证�?
- 视觉冒烟�?*未执�?*（沿用上轮用户决定——冒烟留用户真机验收 F3 十一项清单）；请用户重点复验：黑边消失、白色卡片消失、Gizmo 六方向与网格一致、点�?拖动、顶底视图无滚转�?
- 治理：版�?v0.2.24.10-fix �?v0.2.24.11-fix（五处同步）；无新增第三方依�?项目；不创建 Tag/Release�?

## v0.2.24.10-fix
MAP-A-R1-D5-R1-F2-R3-R2 背景颜色移到片元级（2026-08-03 14:10:00，Commit 本轮落库为准�?
- F2-R3-A3（v0.2.24.9-fix）：**PARTIAL FAIL——中性灰参考地�?FAIL**。现象：截图下半部分仍是偏蓝背景（比天空略暗），未形成明确中性灰地面；网格线宽统一�?LOD 缩放观感 PASS�?
- 根因（用户验收确认，与开发记录一致）�?
  1. 颜色计算在顶点着色器 `backgroundVertex()`——全屏三角形�?3 个顶点计算视线方向并判断天空/地面，中间像素全靠插值，地平线与灰地被插值冲淡成整片蓝灰渐变�?
  2. 两个 smoothstep 参数反写（`smoothstep(0.0, -0.06, dir.z)` �?`smoothstep(-0.06, -0.5, -dir.z)`，edge0 > edge1 未定义行为），第二个还有符号错误——地面远近基本不变化�?
  3. 自动测试只能证明"灰色数字写进 Shader 且可编译"，不能证明颜色出现在视口地面区域�?
- 修复（本版本）：
  - **背景颜色移到片元�?*：`scene.vert` backgroundVertex 只输出全屏三角形位置�?NDC（哨�?(2,2) 表示非背景分支）；`scene.frag` 每像素用 flat 传入�?`vInvViewProjection` 重建世界视线（invVP 每顶点算一次避免每像素求逆），`dir.z >= 0` 画天空、`dir.z < 0` 画灰色参考地面；
  - **smoothstep 方向修正**：`belowHorizon = 1 - smoothstep(-0.06, 0.0, dir.z)`、`groundNearness = smoothstep(0.06, 0.50, -dir.z)`——全�?edge0 < edge1，地平线过渡与地面远近（远处�?�?近处深灰）正确；
  - **配色拉开对比**（用户建议）：天空顶�?`#A6C0DF` �?天空近地平线 `#B3C6DA` �?地平�?`#9CA6AF` �?远处地面 `#858B91` �?近处地面 `#747A80`�?
  - 太阳圆盘/辉光保留（D1 合同 sunDirection 不变）；背景仍不写深度、不进地�?场景/拾取/碰撞�?
  - 未触碰：网格 Shader、线�?0.82�?/2/5 LOD、世界轴、原点、DrawPlan、地图、地形、相机�?
- 验证：聚�?83/83；Core 225/225、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；glslc SceneVert/SceneFrag 逐字 MATCH；git diff --check OK�?
- 视觉冒烟�?*未执�?*（沿用上轮用户决定——冒烟留用户真机验收）；本轮修复点明确（顶点→片�?+ smoothstep 方向），请用户按 F2-R3-A3 复验，重点：默认斜视（天�?灰地/地平线分离）、压低视角（地平线平滑、无硬切线）、有地图（地图覆盖灰地）�?
- 治理：版�?v0.2.24.9-fix �?v0.2.24.10-fix（五处同步）；无新增文件/依赖；不创建 Tag/Release�?

## v0.2.24.9-fix
MAP-A-R1-D5-R1-F2-R3 网格线宽统一 + Unity 风格灰色参考地面（2026-08-03 13:05:00，Commit 本轮落库为准�?
- F2-R2-A2（v0.2.24.8-fix）：**FAIL**。现象：缩放�?Fine/Coarse 使用不同线宽�?.70px vs 1.00px）且重合处直接相加，部分网格线看起来忽粗忽细、层级交界出现明暗脉冲；编辑器参考地面整体偏蓝，与天空层次不足�?
- 修复（本版本）：
  - **R3-A 唯一像素线宽**：删�?FineWidthPixels/CoarseWidthPixels 双宽度，统一 `GRID_LINE_WIDTH_PX = 0.82`（硬合同 0.78~0.90，≤1.0；世界轴保持 1.25px > 网格）；
  - **R3-A 非累加合�?*：`gridAlpha = max(fineContribution, coarseContribution)`（禁�?fine+coarse 相加 �?无双�?Alpha、无粗黑线）；颜色按贡献加权归一化混合（total 仅用于颜色，Alpha 仍为 max）；
  - **R3-A 配色收敛**：Fine `#5D6670` α0.16 / Coarse `#525C67` α0.24（差 0.08 �?0.10，克制深浅差�?深色=更粗"错觉）；
  - **R3-B 中性灰参考地�?*：scene.vert backgroundVertex 程序化背景扩展为 天空顶部 `#9DBBE0` �?天空近地平线 `#AEC4DC` �?地平线混合区 `#9DA5AD` �?远处地面 `#8B9299` �?近处地面 `#7B8289`；地平线过渡按视线方�?dir.z（[-0.06,0] 柔和混合），地面远近�?dir.z �?[-0.06,-0.5] 轻微渐变；不写深度、不进地�?场景/拾取/碰撞，地图与实体自然覆盖�?
  - 未触碰：ReferenceGridScale 1/2/5 选级�?8px 目标、相机求交、LOD 权重、方向性抗摩尔纹、深度偏移、世界轴/原点架构、地�?地形/光照�?
- 验证：聚�?ReferenceGrid/VisualStyle/ShaderContract/DrawPlan 82/82；Core 224/224、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100，ShaderBytecode.Vert.cs 保持�?120 �?行紧凑格�?76 行）；glslc 字节�?GridFrag/SceneVert 逐字 MATCH；git diff --check OK�?
- 视觉冒烟�?*未执�?*（用户选择跳过，图像待用户真机验收 F2-R3-A3 十项清单——不得视�?PASS）�?
- 治理：版�?v0.2.24.8-fix �?v0.2.24.9-fix（五处同步）；新�?ReferenceGridVisualStyleTests.cs；无新增第三方依�?项目；不创建 Tag/Release�?

## v0.2.24.8-fix
MAP-A-R1-D5-R1-F2-R2 统一网格尺度与轴线修复（2026-08-03 11:40:00，Commit 本轮落库为准�?
- F2-A1（v0.2.24.7-fix）：**FAIL**。现象：逐屏幕位�?LOD 导致横向密度分区（近 0.1/�?1/�?10 单位并存）；近处摩尔纹与灰色叠块；世界轴出现楔形；网�?Shader 与独�?WorldAxes Pass 存在轴线重复绘制�?
- 修复（本版本）：
  - 取消�?Fragment LOD——每�?CPU 由视口中心射线与 Z=0 求交（中心�?px 世界距离�?max）得参考世界每像素，整帧统一 Fine/Coarse 层级�?
  - 1/2/5 十进制序列（0.01/0.02/0.05/0.1/0.2/0.5/1/2/5/10…），目�?48px/格，对数域相�?+ smoothstep 互补交叉淡化（FineWeight+CoarseWeight�?，边界旧 Coarse=�?Fine 无缝）；
  - 求交失败回退：中�?�?视口偏下 60% �?上一帧合法尺度（禁止重置�?1）；
  - 方向性抗摩尔纹：X/Y 各自按单元屏幕间距淡出（<6px 隐藏�?~12 渐入�?12 正常）；
  - 轴线单一事实源：网格 Shader 删除 X/Y 轴与原点绘制；新增独�?WorldAxes 全屏 Pass（金 X=世界 Y=0、蓝 Y=世界 X=0，各自方向导数固�?1.25px 屏幕宽度）与 WorldOrigin 全屏 Pass（琥珀原点标记 �?px 半径）；三个 Pass 开关（ShowGrid/ShowWorldAxes/ShowOrigin）完全独立；
  - 深度偏移有界化：clamp(fwidth(depth)×0.5, 1e-7, 2e-5)�?
  - DrawPlan 顺序（方�?12）：背景 �?地形(MapBounds) �?网格 �?原点 �?世界�?�?实体填充 �?轮廓 �?Gizmo�?
- 验证：聚�?ReferenceGrid/WorldAxes/DrawPlan/Shader 合同测试 73/73；Core 215/215、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；glslc 字节码四文件逐字一致；git diff --check OK�?
- 视觉冒烟�?*仅完成启动冒烟，图像待用户验�?*（本环境�?computer_use 工具无法读取编辑器截图，按宪法不得写视觉 PASS）。启动冒烟实测：编辑器进程启动后存活 72.9s 无崩溃、无 Vulkan 会话回滚（三全屏 Pass 管线创建成功）；F2-A2 三张截图（默认斜�?拉近/拉远）待用户执行�?
- 治理：版�?v0.2.24.7-fix �?v0.2.24.8-fix（五处同步）；新�?ReferenceGridScale.cs（纯数学）、WorldAxes/WorldOrigin Shader + 字节码、GridPipelineSet.cs、GridScale.cs、ShaderContractTests/ReferenceGridScaleTests；无新增第三方依赖；不创�?Tag/Release�?
- F2-A2 真机验收清单已交付（9 项：默认斜视/拉近/拉远/平移/环绕/独立开�?实体遮挡/有地图无地图/窗口尺寸），待用户执行�?

## v0.2.24.7-fix
MAP-A-R1-D5-R1-F2 无限参考网格稳定性修复（2026-08-03 10:40:00，Commit 本轮落库为准�?
- 任务目标：修复截图中"普通网格几乎不可见、只剩两条坐标轴"问题——稳定的缩放自适应层级、普通网格可见、远处无闪烁/地平线无噪声、有地图时网格不受地图边界裁剪；不修改天�?光照/地形/视角 Gizmo/地图编辑�?Schema�?
- 根因（代码调查，非计划推测）�?
  1. **线宽公式参数反转（主因）**：`gridLine` �?`smoothstep(vec2(0.5), edge, f)` �?`edge = 0.5 - d×linePixels/2 < 0.5`，edge0 > edge1 �?GLSL 未定义行为，实际线宽 = `1/d - linePixels` 像素——远�?d�? 时线宽爆炸为数十像素宽的淡带，近处趋�?0，普通网格视觉上消失�?
  2. **层级目标间距 20px 过小**：desiredStep = worldMetersPerPixel×20，量化后细格屏幕间隔平均�?~8px，过密成噪声，且权重窗口 0.25~0.75 互补导致细格常被完全压掉（仅�?0.18 基础 α）；
  3. **地图矩形�?discard**：F2A 为规�?Z-fighting �?shader 内按地图矩形裁剪网格，有地图时视野内网格全部消失，违�?无限网格不受有限地图边界裁剪"�?
  4. **坐标轴过�?*：�?.78/2.5px 压过网格，且 X/Y 颜色与方案相反�?
- 修复（editor_reference_grid.frag 重写 + DrawPlan 顺序调整）：
  - 线宽改用方案 4.6 标准公式 `1 - smoothstep(w-0.5, w+0.5, 像素距离)`：细 0.75px / �?1.10px / �?1.35px，屏幕恒定不再随距离爆炸�?
  - 目标间距 36px/格（`worldMetersPerPixel×36`，合法层�?0.1~10000 钳制）；细格权重 `1-smoothstep(0.5,1.0,phase)` 1�?、主格加深权�?`smoothstep(0.0,0.5,phase)` 0�?�?
  - **跨级透明度连�?*：主格线位置是细格子集，细格基础 α0.20 + 主格加深 α0.18，同组线跨级时从主格 0.18 平滑过渡为细�?0.20（差 �?.02），不跳格不闪烁�?
  - **移除地图矩形 discard**：网格为无限参考平面，不再按地图裁剪；共面稳定改由 `gl_FragDepth = depth - max(fwidth(depth)×1.5, 1e-7)` 像素级深度偏移实现（实体/凸起地形仍正常遮挡，符合方案八）�?
  - 配色按方案：细格 #566A82 α0.20、主�?#344A63（基础 0.20+加深 0.18）、X �?#AD8550 α0.62（世�?Y=0 线）、Y �?#557C9E α0.62（世�?X=0 线）、原�?#D1AE69 α0.70；坐标轴 1.35px 不再抢眼�?
  - 掠射角淡出窗�?0.015~0.080（方案七）；距离淡出保持 0.45~0.75 far、gridMaxDistance=far×0.75（基�?far 约定，未硬编码米数）�?
  - **DrawPlan 顺序修正**：网格从"天空之后"移到"地形/实体之后、轮�?Gizmo 之前"（RenderDrawPlan.GetFrameDrawPlan），实体可遮挡网格、平坦地形上经深度偏移稳定显示；�?无地图、有/无实体均保留网格�?
  - PushConstant �?192B 缩为 160B（移�?mapParams/mapParams2�?0 float），vert/frag/C# 三处同步，管�?maxPushConstantsSize 校验同步�?
  - 相机相对坐标：本轮保持绝�?float32（与实体同机制，地图 2000m 量级内精度足够）；大尺度世界相机相对化属全局渲染原点架构问题，按方案九不强行扩围，另行登记�?
- 测试：`ReferenceGridAdaptiveTests` 重写（�?6 目标、两级相�?权重区间、跨�?α �?�?.02 连续性、phase=0.5 峰�?0.38、距�?掠射角曲线）；`ReferenceGridDrawPlanTests` 新增 4 组合（有/无地图×有/无实体）+顺序断言（实体后、Gizmo 前）+关闭开关缺席；`ViewportAssistDrawPlanTests`/`MapRenderDrawPlanTests` 顺序断言同步；Core 189/189、World 435/435、WarCore 22/22；arch-a-guard PASS；glslc 字节码逐字一致（GridVert 336、GridFrag 1379 词）�?
- 治理：版�?v0.2.24.6-rz �?v0.2.24.7-fix（五处同步）；无新增项目/依赖；不创建 Tag/Release�?
- 状态：MAP-A-R1-D5-R1-F2 自动门禁全绿，真机验收待用户执行（IPO 清单见报告）；通过前不宣布 D5-R1 CLOSED，不进入 D5-R2�?

## v0.2.24.6-rz
MAP-A-R1-D5-R1-F2/F2A Blender 风格自适应参考网格（2026-08-03 00:30:55，Commit 909b6fd 之后待收口）
- 任务目标：废�?42 条世界空间粗四边形网格，改为独立全屏 Pass + 片元解析世界 Z=0 平面，实�?Blender 式无限自适应参考网格；只动网格，不处理 Gizmo/天空/取景�?
- 独立渲染管线：新�?`editor_reference_grid.vert/.frag` + `VulkanGraphicsPipelineOwner.Grid.cs`（独�?192B PushConstant，创建时校验设备 maxPushConstantsSize；DepthTest=On/LessOrEqual、DepthWrite=Off、AlphaBlend=On�? `VulkanClearFrameOwner.Grid.cs`（VP/InvVP/相机/视口/far/地图参数填充）；`RenderDrawKind.EditorGrid` �?`EditorReferenceGrid`（顶点数 252�? 全屏三角形）；scene.vert 移除 gridVertex �?-10.5 魔法分支；DrawAssist 不再处理网格�?
- 自适应分级：`desiredStep = worldMetersPerPixel × 20`（合法层�?0.1/1/10/100/1000/10000，钳�?0.1~10000）；只混合相邻两个十进制层级，权重和=1，平滑交叉淡入（细格 1px α0.18、主�?2px α0.32）�?
- 淡出：距离淡�?0~45% far 完整 / 45~75% 平滑 / >75% 隐藏；掠射角淡出 abs(dot(N,V))<0.03 隐藏 / 0.03~0.12 淡入 / >0.12 完整�?
- 主轴与地图：X 轴（世界 Y=0�?5A7FA3 α0.78）、Y 轴（世界 X=0�?B68B54 α0.78）、原点标记（#D1AE69 α0.85），屏幕恒定 ~2.5px 贯穿可见平面；地图矩形内逐片�?discard（feather=像素×1.5 �?0.05），地图外网格继续显示，卸载后完整恢复�?
- 配色（玄域浅色编辑器，禁高饱�?荧光/红绿工程轴）：细�?#7E8FA1 α0.18、主�?#607487 α0.32�?
- 测试：`ReferenceGridRayIntersectionTests`（G1 射线求交 7 项）+ `ReferenceGridAdaptiveTests`（层级选择/权重�?钳制/淡出曲线/裁切�?8 项）+ `ReferenceGridDrawPlanTests`（有/无地图网格存�?顺序）；Core 183/183、World 435/435、WarCore 22/22；arch-a-guard PASS；glslc 字节码逐字一致（GridVert 348、GridFrag 1559、scene.vert 7864 词）�?
- 已知基线说明�?09b6fd �?scene.frag 源码�?8 词透传版）与内�?ShaderBytecode.Frag.cs�?13 �?F1 版）本身不一致（基线遗留，本轮未触碰 scene.frag/其字节码，超�?F2A 冻结范围）�?
- 治理：版�?v0.2.24.5-rz �?v0.2.24.6-rz（五处同步）；无新增项目/依赖；不创建 Tag/Release�?
- 状态：MAP-A-R1-D5-R1 网格专项真机验收通过（用户授权收�?push）；后续 Gizmo/天空/取景�?F2 纪律单独轮次处理�?

## v0.2.24.5-rz
MAP-A-R1-D5-R1 视口参照与导航（2026-08-02 22:57:00，Commit 2fdf470 之后待收口）
- 任务目标：按用户最新真机裁定修正视口参照与导航——视觉无限参考网格、地图外网格延伸、右上角视角 Gizmo 真实可见、程序化天空渐变、自动取景屏幕占用率 65~75%�?
- 视觉无限 EditorReferenceGrid：`scene.vert gridVertex` 重构——网格重心跟随相机（worldPosition.xy 对齐间距）、间距按相机高度分级 0.1/1/10/100/1000/10000 米、线长覆�?step×12、主次线分级宽度；`RenderDrawPlan` 取消 HasMap 时移�?EditorGrid（地图存在时网格保留，地图矩形由 shader 裁切避免穿透地表与 Z-Fighting，卸载后网格继续存在）；`VulkanClearFrameOwner.DrawAssist` EditorGrid 分支传相机位�?+ 地图半宽/半深（entityScale.xy 复用，push constant 128B 不扩容）�?
- 视角 Gizmo 真实可见：根因是 ViewGizmo 位于 VulkanViewport Grid 内被嵌入 Win32 原生窗口遮挡——移�?`UiRoot.axaml` 视口 Border 外层 Grid（Avalonia 覆盖层，位于原生渲染窗口之上），六方向按�?+ 当前朝向琥珀描边�?
- 程序化天空增强：天顶饱和�?(0.22,0.45,0.85) �?地平线更雾白 (0.88,0.92,0.97)，pow 0.55�?.35 渐变更快集中；仍为独�?Sky Pipeline（DepthTest/Write=Off、Z-Up �?dir.z、只依赖相机旋转）�?
- 地图自动取景：`FrameMapAllWithCenter` 改为按目标屏幕占用率（垂直投影约 70%，透视补偿 ×1.55，实�?d�?850 时占用率�?9%、最大视锥角 28.5°<30°）计算距离，地图不再过小；新�?`WorldCameraFramingOccupancyTests`（NDC 投影包围�?65%~80%）�?
- 世界坐标轴颜色：X=浅蓝�?(0.55,0.62,0.70)、Y=冷钢�?(0.42,0.52,0.64)、Z=柔和琥珀 (0.78,0.66,0.42)，禁止高饱和红绿轴�?
- ShaderBytecode：glslc -O 重新生成�?762 词，83 行）逐字比对一致�?
- 测试：`WorldCameraFramingOccupancyTests` 新增（占用率 65~75%）；`MapRenderDrawPlanTests.With_map_grid_kept_and_bounds_added` 更新（D5-R1 需求变更：网格保留而非移除）；World 435/435、Core 148/148、WarCore 22/22；arch-a-guard PASS�?
- 治理：版�?v0.2.24.4-rz �?v0.2.24.5-rz（五处同步）；无新增项目/依赖；不创建 Tag/Release�?
- 状态：MAP-A-R1-D5-R1 真机人工验收待用户执行；通过后进�?D5-R2 真实参数编辑�?

## v0.2.24.4-rz
MAP-A-R1 D4 视觉收口 + D5 正式地图编辑�?场景引用�?026-08-02 22:32:36，Commit 5fcd02b 之后待收口）
- 任务目标：把 D4 真机视觉缺陷收口（程序化天空、视�?Gizmo、正式地图编辑器、场景地图引用），完�?MAP-A-R1 功能闭环；D4/D5 各轮独立提交推送�?
- EDITOR-VIEW-R1 视角 Gizmo：`UiVm.ViewGizmo.cs` + `ViewGizmo.axaml`——视口右上角 3×3 网格六方向按钮（�?�?�?�?�?右）+ 中心当前朝向琥珀描边；Z-Up 坐标合同冻结（顶=+Z 看向 -Z、前=-Y 看向 +Y 等）；保持观察中心（选中实体→地图中心→原点）与距离只改朝向；浅蓝灰主体、白字，无红绿蓝三轴配色；不建第二套 CameraState；测�?3 项（六方向朝�?中心距离保持/选择保持）�?
- D5-A 正式地图编辑器：右侧一级「地图编辑器」Tab（与检查器平级）——地图资产区（名�?路径/MapId/尺寸/状态）+ 新建/打开/保存/卸载/聚焦五命令；复用 D2 MapDocumentOwner/MapStorageService �?D3 WorldMapStateOwner；打开失败保持原地图；第二排「加载测试地�?卸载地图」临时按钮已删除；测�?4 项�?
- D5-B 场景地图引用：`.xyscene` schema v3→v4 新增可�?`mapReference{mapId, assetPath}`（只存引用不复制地图数据）；旧场景无引用正常打开；缺�?损坏时场景主体打开 + 显示「引用失效�? 路径原因 + 不自动建默认地图；保存附加引用、打开自动加载；测�?4 �?+ schema v4 断言更新�?
- 验证结果（D6 最终门禁）：全解决方案强制重编�?0 error / 1 既有 warning（xUnit2013，非本轮引入）；Core 148/148、World 434/434、WarCore 22/22；arch-a-guard PASS；glslc 字节�?8293 词逐字一致；git diff --check PASS�?+100 本轮文件全过�? 个既有超限文件非本轮范围，守卫口�?PASS）�?
- 治理：版�?v0.2.24.3-rz �?v0.2.24.4-rz（五处同步：changelog/file-tree/UiVm.SceneDocument.cs/UiWin.axaml/run.bat）；无新增项�?依赖；不创建 Tag/Release�?
- 状态：MAP-A-R1-D4/D5 真机人工验收待用户执行（IPO 清单见报告）；全部通过�?MAP-A-R1 CLOSED，进�?MAP-A-R2 区域与图层�?

## v0.2.24.3-rz
MAP-A-R1-D4 有限地表渲染与自动取景（2026-08-02 21:46:52，Commit 9d1f2c9 之后待收口）
- 任务目标：让地图以可观察、可编辑的战场方式出现在视口——有限地表网格、缓丘明暗、程序化天空、地图边界、加载后斜上方自动取景；D4 真机修复收口�?
- D4 主体（基�?9d1f2c9 已含）：`MapTerrainMeshBuilder`（唯一采样�?MapSurfaceSampler 的渲染侧消费方，4225 顶点/24576 索引，CPU 数值差分法�?+ 预计算亮度）、`MapBoundsMeshBuilder`�?8 顶点琥珀色边界线）、`RenderDrawPlan` 地图绘制（EditorBackground 天空 �?WorldOrigin/Axes �?MapBounds 地形+边界 �?EntityFill �?Gizmo，HasMap 时移�?EditorGrid）、`RenderProjection.Map` 携带 `MapRenderSnapshot` 传播链、shader kind=-14 地表 / -15 边界分支、F1 临时加载/卸载按钮、F2 绘制顺序修复（地表在天空之后）�?
- F3 真机修复（本轮）：Lambert 方向语义�?D1 合同对齐——`sunDirection` = 指向光源方向（Z>0 朝上），`MapTerrainMeshBuilder.Brightness` 不再取反（修复前 toLight 指向地面下方，平�?ndl=-0.75�?，地表只剩环境光 0.35，视觉为灰蒙暗绿）；`WorldMapState` 默认 SunDirectionZ 同步 +0.75；`MapRenderSnapshot`/`MapDocumentWorldBridge` 注释同步合同语义�?
- F4 可读性（本轮）：`EditorCameraFraming.FrameMapAllWithCenter` 地图取景 45° 斜上方俯视（Forward.Z=-0.707，完整容纳四�?+ 安全边距）；`Brightness` 合成降为 `ambient×0.3×hemi + sun×0.85×ndl`（clamp [0,1]），避免全部顶点�?shader 钳制同色，缓丘受�?背光�?�?.086 肉眼可辨；scene.vert 天空顶部加深�?(0.45,0.56,0.74)、地平线更雾�?(0.88,0.90,0.94)，ShaderBytecode �?glslc -O 重新生成并逐字比对�?
- 测试（XuanYu.World.Tests/Map/ �?/World/）：`MapTerrainBrightnessTests` 新增（Flat 亮度稳定∈[0.5,0.9]、缓丘明暗差>0.03、方向光贡献>0.05）；`WorldCameraFramingTests` 新增�?5° 俯视 + 四角完整容纳）；`MapTerrainMeshBuilderTests` 亮度断言�?F4 合成公式更新�?
- 治理：版�?v0.2.24.2-rz �?v0.2.24.3-rz（五处同步：changelog/file-tree/UiVm.SceneDocument.cs/UiWin.axaml/run.bat）；无新增项�?依赖；ShaderBytecode 为生成物，行�?78（≤100 守卫口径通过）；第二排「加载测试地�?卸载地图」为 D4 临时验收入口，D5 移入右侧「地图编辑器」一级模块�?
- F5 程序化天空（用户真机截图裁定：D4 视觉验收 FAIL 后追加，D4 保持 IN PROGRESS�?5c9a0e 保留不回滚）：重�?Unity/Godot 风格程序化天空——天顶清晰蓝 (0.28,0.50,0.85) �?地平线浅蓝雾�?(0.78,0.87,0.96) �?地平线以下轻微大气泛�?(0.42,0.48,0.56)；上半球渐变改用 pow(dir.z, 0.55) 集中；新增最小太阳圆盘（方向�?D1 合同 sunDirection 一致，仅圆�?微弱辉光，无耀�?体积光）；ClearColor 改为浅蓝失败回退 (0.35,0.55,0.80)，不再用灰色掩盖天空失败；天空失败日志保留（ShaderModule/PipelineLayout/GraphicsPipelines 三处明确记录）；绘制仍为独立 Sky Pipeline（DepthTest/Write=Off、先于地表、只依赖相机旋转不依赖平移，Z-Up �?dir.z）�?
- 验证结果（F5 追加）：串行 build 12 项目 0 error / 1 warning（既�?xUnit2013）；Core 148/148、World 423/423、WarCore 22/22；arch-a-guard PASS；glslc 重新生成 ShaderBytecode�?293 词，79 行）逐字比对一致，新天空色与太阳常量全部在字节码中�?
- D5-A 正式地图编辑器（用户真机裁定后追加，独立轮次）：右侧一级模块新增「地图编辑器」Tab（检查器之后，与检查器平级）——地图资产区（名�?路径/MapId/尺寸/状态：未加�?已保�?未保存）+ 新建/打开/保存/卸载/聚焦五命令；新建默认 TestBattlefield 2000×2000，复�?D2 MapDocumentOwner/MapStorageService（候选加�?原子保存）与 D3 WorldMapStateOwner，无第二套系统；打开失败保持原地图不变；第二排「加载测试地�?卸载地图」临时按钮已删除；基础地表/环境编辑组留 D5 后续补齐。新�?UiVm.MapEditor.cs（文档状�?命令�?00 行）、UiWin.MapCommands.cs�?xymap 文件选择器）、MapEditorPanel.axaml（面板，Right.axaml 引用）；测试 UiMapEditorTests 4 项（新建�?World+Dirty、保�?打开 Round-trip、卸载清空、打开失败不污染）�?
- 验证结果（D5-A 追加）：串行 build 12 项目 0 error；Core 148/148、World 430/430（含 D5-A 新增 4 项）、WarCore 22/22；arch-a-guard PASS�?+100 全过）；git diff --check PASS�?
- D5-B 场景地图引用（独立轮次）：`.xyscene` schema v3 �?v4，新增可�?`mapReference{mapId, assetPath}`（只�?mapId + 相对场景目录路径，不复制地图尺寸/地表/环境参数）；旧场景无 mapReference 正常打开；Validator 校验 mapId 合法 + 路径安全（非法拒绝），无效引用场景主体仍打开、地图编辑器显示「引用失效�? 路径原因、不自动创建默认地图。新�?MapReference.cs、SceneDocumentValidator.MapReference.cs（校验拆分，Validator 保持 100 行）；SceneDocumentJson/Mapper/Snapshot 双向映射；UiVm.SceneDocumentMapRef.cs（保存附加引�?+ 打开解析加载）；测试 SceneMapReferenceTests 4 项（保存携带/打开恢复/旧场景兼�?缺失失效）�?
- 验证结果（D5-B 追加）：串行 build 12 项目 0 error；Core 148/148、World 434/434（含 D5-B 新增 4 �?+ schema v4 断言更新 3 处）、WarCore 22/22；arch-a-guard PASS；git diff --check PASS�?
- 状态：MAP-A-R1-D4 真机人工验收待用户执行（IPO 清单见报告）；验收通过�?D4 CLOSED，进�?MAP-A-R1-D5 正式地图编辑器与场景引用�?

## v0.2.24.2-rz
MAP-A-R1-D3 World 地表能力�?026-08-02 18:24:41�?
- 任务目标：把地图文档转化�?World 可查询的确定性地表能力——有限边界、唯一地表采样器、世�?X/Y �?地表 Z、加�?切换/卸载、最小渲染快照；本轮不渲染、不�?UI 与场景引用�?
- 新增 `XuanYu.Core/Map/`：`MapSurfaceKind`（Flat/GentleHillsV1）、`MapSurfaceSampler`（唯一采样源：Flat 固定高度；GentleHillsV1 双正交正弦叠加，相位�?seed 固定派生，输�?[base−amp, base+amp]，纯算术确定性）、`MapRenderSnapshot`（供 D4 Render 消费的最小快照：尺寸+地表参数+MapId，卸载后 Empty）�?
- 新增 `XuanYu.World/Map/`：`WorldMapState`（纯数据+有限边界判断+高度查询；世�?X 横向/Y 纵向/Z 高度，Z-Up 直写无映射层；闭区间边界，边界点属于地图；地图外不钳制不返回虚假零高度）、`WorldMapStateOwner`（当前地图状态：Load/Unload/Switch、TryGetSurfaceHeight(X,Y,out Z)、BuildRenderSnapshot）�?
- 桥接：`XuanYu.Editor/MapDocument/MapDocumentWorldBridge.ToWorldState`（MapDocument �?WorldMapState，字符串 kind �?枚举映射，对�?SceneDocumentWorldBridge 模式）�?
- 测试（XuanYu.World.Tests/Map/，新�?4 文件 32 项）：Flat/GentleHills 确定性（同坐标多次一致�?00 点扫描）、幅度范围、seed/位置差异；边界闭区间（中�?四边/角在内，�?0.001 米拒绝）；Owner 加载/切换/卸载/快照清空不残留；桥接字段完整与端到端查询一致�?
- 治理：版�?v0.2.24.1-rz �?v0.2.24.2-rz（五处同步）；无新增项目/依赖；Core 新增纯数�?Map 类型（非 Scene/World/Picking/Gizmo 禁区）；World �?Core 仅、Editor 桥接不反向依赖�?
- 验证结果：串�?build 12 项目 0 error / 1 warning（既�?xUnit2013）；Core Tests 145/145；World Tests 411/411（含地图新增 32 项）；WarCore Tests 22/22；arch-a-guard PASS；glslc PASS；git diff --check PASS�?+100 全仓扫描 PASS（守卫口径与 wc �?�?00）�?
- 状态：MAP-A-R1-D3 完成（无 UI/视口，验收以自动测试为准），等待批准后进�?MAP-A-R1-D4 有限地表、天空和光照�?

## v0.2.24.1-rz
MAP-A-R1-D2 .xymap 地图存储闭环�?026-08-02 18:15:25�?
- 任务目标：地图资产可靠创建、严格校验、保存、关闭并重新读取；本轮不渲染、不查询、不�?UI 与场景引用�?
- 新增 `XuanYu.Editor/MapDocument/`：`MapDocument`（SchemaVersion/MapId/Name/SizeMeters/CoordinateSystem/Surface/Environment/LayerReferences）、`MapId`�?2 位十六进制，创建后稳定）、`MapSize`/`MapCoordinateSystem`/`MapSurfaceDefinition`/`MapEnvironmentDefinition`/`MapVector3` 值对象、`MapDocumentValidator`（结构化 Issue 校验：尺�?100�?0000、坐�?Z-Up 米制零原点、地表仅 Flat/GentleHillsV1、环境参数有限非负、layerReferences 必须为空、未知类型拒绝）、`MapDocumentResult<T>`（对�?SceneDocumentResult 模式）�?
- 存储闭环：`MapJsonSerializer`（严�?JSON：字段大小写敏感 + 未知字段拒绝 + JsonPropertyName 固定 camelCase + JsonPropertyOrder 确定性输�?+ UTF-8）、`MapJsonMapper`、`MapStorageService`（候选加�?解析→验证→成功才返回；原子保存=同目录临时文件→完整写入→File.Move 替换→失败清理并保留旧文件）、`MapDocumentOwner`（CurrentMap/CurrentPath/IsDirty 最小状态机：New→Dirty、Load→Clean、Modify→Dirty、Save→Clean、Unload→清空；失败不污染）�?
- 路径合同：`Maps/<MapName>/map.xymap`；不存绝对路径；目录按需创建。D1 合同修正：`mapId` 口径更新为纯 32 位十六进制（�?`map_` 前缀，D2 §5.2 明确），docs/milestones/current/MAP-A/map-a-r1-d1-map-contracts.md 已同步�?
- 测试（XuanYu.World.Tests/Map/，新�?9 文件）：MapId 格式/稳定性、尺寸边界与拒绝、坐标合同、地�?环境参数、图层引用空约束、JSON Round-trip 与确定性、大小写/未知字段/类型/损坏拒绝、候选加载失败不污染、原子保存与临时文件清理、Owner 状态链闭环�?
- 治理：版�?v0.2.24.0-rz �?v0.2.24.1-rz（五处同步）；无新增项目/依赖；不触碰 SceneDocument、WarCore、渲染与 UI�?
- 验证结果：串�?build 12 项目 0 error / 1 warning（既�?xUnit2013）；Core Tests 145/145；World Tests 379/379（含地图新增 65 项）；WarCore Tests 22/22；arch-a-guard PASS；glslc PASS；git diff --check PASS�?+100 全仓扫描 PASS（守卫口径与 wc �?�?00）�?
- 文件级验收（临时目录真实文件）：首次保存→重新读�?Round-trip 全字段一致（mapId/尺寸/坐标/地表/环境）；损坏 JSON 拒绝且不替换；保存失败无临时文件残留、不破坏旧文件�?
- 状态：MAP-A-R1-D2 完成（无 UI/视口，验收以自动测试 + 真实文件检查为准），等待批准后进入 MAP-A-R1-D3 World 地表能力�?

## v0.2.24.0-rz
MAP-A-R1-D1 地图合同冻结�?026-08-02 17:42:55�?
- 任务目标：只读核查现�?SceneDocument / World Snapshot / 渲染地面 / 右侧模块结构后，冻结 `.xymap` 第一�?Schema �?`.xyscene` mapReference 合同；本轮零产品代码，不重构旧代码�?
- 坐标裁定（用户拍板，方案 B）：`.xymap` 语义与世界轴直写——X 横向（世�?X）、Z 高度（世�?Z=Up）、Y 纵向（世�?Y），与官方坐标合�?WORLD-A-R0（Z-Up、XY 水平）一致；不引入映射层；查询合同为「输入世�?X/Y 水平面坐�?�?输出地表 Z 高度」�?
- 合同冻结（docs/milestones/current/MAP-A/map-a-r1-d1-map-contracts.md）：`.xymap` schemaVersion=1，mapId=`map_`+32hex，尺�?100�?0000 米，surface �?Flat/GentleHillsV1（确定性采样），environment �?ClearDayV1 + 方向�?环境光；保存路径 `Maps/<Name>/map.xymap`，原子替换，候选完整验证；`.xyscene` �?v4 增可�?`mapReference{mapId, assetPath}`（项目相对路径，场景不复制地图数据），旧场景兼容，引用缺失明确报「引用失效」�?
- 核查事实：无限灰网格=RenderDrawKind.EditorGrid�?52 顶点，scene.vert gridVertex，�?0 �?21×21 线，z=0 平面）；天空=EditorBackground+深度不写第二管线（WORLD-D 成品，直接复用）；光�?shader 硬编码固定方向光+半球环境光；右侧模块 Right.axaml=检查器/调试/偏好/模式�?Tab（MAP-A 收为检查器+地图编辑器）；全库无任何地图类型；版本源五处一致�?
- 治理：新里程�?MAP-A（模�?24），新分�?feat/MAP-A-map；版�?v0.2.23.0-rz �?v0.2.24.0-rz（五处同步）；基�?HEAD cbb694b = origin tip，ahead/behind 0/0；已知偏�?untracked `IDEA.md` 与残�?`XuanYu.Editor.Avalonia/` bin 目录未处理�?
- 状态：MAP-A-R1-D1 合同冻结完成，等待批准后进入 D1 域类型编码（MapId/MapDocument/MapSurfaceDefinition/字段验证）�?

## v0.2.28.66-rz · AREA-D-R3-INSPECTOR-PAGER�?026-09-11 00:00:00 +08:00�?
- 目标：将右侧地图/区域编辑导航切换为分�?Inspector，并让图�?Dock 支持自适应折叠�?
- 变化：新�?XYPager、XYInspectorSection、XYCollapsiblePane；地图页增加基础/环境/显示/数据/高级分页；图层区改为可折�?Pane；同步运行时与源�?UI 合同测试�?
- 验证：专�?Runtime/UI 合同测试 25/25 通过；全量门禁待执行�?
- Hash：待提交�?
- 遗留：等待真机视觉与交互验收�?

