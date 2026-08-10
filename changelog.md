# changelog

## 归档规则

- 每个自然月执行一次 changelog 归档（宪法第五十三条《月度归档》）。
- 当前自然月记录保留在本文件；已结束月份按自然月归档至 `docs/archive/changelog/changelog-YYYY-MM.md`（单一归档位置，不为每个轮次单独建文件）。
- 归档内容原则上原样迁移，保留版本、日期、验证与遗留事项；版本历史不丢失，按下方索引可定位。

## 历史归档索引

| 月份 | 归档文件 | 条目范围 |
|---|---|---|
| 2026-07 | `docs/archive/changelog/changelog-2026-07.md` | v0.2.1.1-rz ～ v0.2.23.0-rz |
| 2026-06 | `docs/archive/changelog/changelog-2026-06.md` | v0.1.1.5-rz ～ v0.1.8.10-fix |
| 2026-05 | `docs/archive/changelog/changelog-2026-05.md` | v0.1.1.1-rz ～ v0.1.1.4-rz |

> 历史审计注记（SHR-2026-08-R2）：7 月归档内存在 3 处同一版本号分配给两个不同轮次的历史缺陷（v0.2.16.2-rz、v0.2.17.8-rz、v0.2.20.19-fix 各 2 条，内容不同）；按归档原则保留原文不篡改，追溯时以 Commit Hash 为准。版本号与日期顺序另有 18 处非单调，为历史既成事实，登记不重排。

---

## v0.2.25.6-fix
MAP-A-R3-D2-F1-C2 地图相机语义（2026-08-10 09:38:15）：地图编辑器模式下“查看全部”按 MapBounds 构图；“聚焦”按 Draft AABB → Selected Entity → 相机不变执行；新增正交地图取景、Draft 最小可视半径与右侧地图模式状态；补充 C2-R01～C2-R09 自动回归。
- 验证：C2 专项 9/9 PASS；解决方案 Build 0W0E；Core 348/348、World 985/985、WarCore 22/22 PASS；ARCH-A、5+100、版本一致性、git diff --check PASS。
- 状态：MAP-A-R3-D2-F1 继续 OPEN；F1-C2 等待用户执行 C2-M01～C2-M04 真机确认；F1-V 未开始；A03～A06 BLOCKED，D3 禁止启动，F2 未创建。
- Hash：5c23ea7。

## v0.2.25.5-fix
MAP-A-R3-D2-F1-C 稳定性与日志收口（2026-08-10 09:09:01）：聚焦在区域草稿或无选中实体时保持相机不变；新增 TryProjectWorldPoint 并让 Region PointerMoved 对投影失败安全降级；移除 F1TRACE 运行时取证、临时文件和高频 PointerMoved/Ray/Mouse/Render 日志；补充 C-R01～C-R06 回归测试。
- 验证：解决方案 Build 0W0E；Core 348/348、World 976/976、WarCore 22/22 PASS；ARCH-A、5+100、git diff --check PASS。
- 状态：MAP-A-R3-D2-F1 继续 OPEN；S02 修复确认 PASS；F1-C 等待用户真机确认 Focus 与日志；F1-V 未开始；A03～A06 BLOCKED，D3 禁止启动，F2 未创建。
- Hash：5739c31。

## v0.2.25.4-fix
MAP-A-R3-D2-F1 根因收口（2026-08-10）：修复首点草稿生成零长度 primitive、区域 world-space 绘制误用零缩放，以及拾取未遵守地图中心原点边界合同；新增 MapPoint↔World 直接映射合同与首点资源 Vulkan 合法性回归。
- 验证：解决方案 Build 0W0E；Core 346/346、World 971/971、WarCore 22/22 PASS；R17～R20 聚焦测试 5/5 PASS；ARCH-A、5+100、git diff --check PASS。
- 状态：MAP-A-R3-D2-F1 继续 OPEN；既有 S01/S02 失败证据已确认，不重复要求用户证明旧失败；修复后的真机确认仍待用户执行；A03～A06 BLOCKED，D3 禁止启动，F2 未创建。
- Hash：44280d7。

## v0.2.25.3-rz
MAP-A-R3-F1-FINAL 原生输入与区域绘制返工（2026-08-09）：修复区域工具在释放鼠标后不更新预览的问题；修复正式区域与 Draft 资源 primitive 范围为空/固定颜色导致不可见的问题；增加 App/UI 版本溯源及 A-E 阶段运行时取证，取证同时写入底部日志与临时文件；F2 保持冻结。验证：App 构建 0 Warning / 0 Error，World.Tests PASS，ARCH-A PASS，git diff --check PASS。Hash：5dffd70。遗留：真机验收仍由用户执行，未宣布 CLOSED。

## 2026-08（当前自然月）
## v0.2.25.3-rz
MAP-A-R3-F1 真机失败返工（2026-08-09 22:52:30）：截图显示点击后既无命中反馈也无 Draft 顶点；审计发现 VulkanViewport 初始化提示层未禁止命中测试，存在遮断 VulkanNativeHost 输入链的风险。修复 FallbackLayer `IsHitTestVisible=False`，并为区域绘制拾取未命中增加状态栏证据；World.Tests 968/968 PASS，解决方案复制 UI DLL 阶段因运行中的 XuanYu.Editor.App 锁文件未完成。提交 `fb5af08`。F2 未启动，等待重新真机验收。
### v0.2.25.1-rz
MAP-A-R3-D1：既有 Region 合同审计与加固（2026-08-09 20:35:37）
- 变化：复用 R2 的 MapRegion/MapRegionDraft/MapPoint/Region Layer；新增非相邻边相交、接触、重叠拒绝；新增 MapEditSession Region Create/Delete 正式提交入口，保持单历史条目与相同 RegionId 的 Undo/Redo；同步 R3 backlog、file-tree 与四处版本号。
- 验证：解决方案 Build 0 Warning / 0 Error；Core 344/344；World 943/943；WarCore 22/22；ARCH-A、5+100、git diff --check PASS。
- Hash：dcb4b91。
- 遗留：D1 不包含绘制 UI、Picking 接入、Renderer、LayerPanel、Inspector、持久化、GIS、DGD、Hole、MultiPolygon 或 Polygon Boolean；D1 完成后停止，D2 另行批准。

### v0.2.25.3-rz
MAP-A-R3-D2-F1：Region Tool Integration & Selected-State Regression（2026-08-09，返工后待真机复验）。
- D2 真机裁定：A01 FAIL；A02～A06 BLOCKED / NOT EXECUTED；L4 FAIL；D2 OPEN；D3 禁止启动。
- 修复：Region Drawing 从 Top/App-level 入口移入 Map Editor 的“地图工具”区；补齐 Normal/Hover/Selected/Selected+Hover 深色 Foreground；新增真实 Headless Runtime RED→GREEN 与静态归属契约。
- 验证：F1 真实工具链 Runtime/静态测试 5/5 PASS；Solution Build 0W0E；Core 345/345；World 952/952；WarCore 22/22；ARCH-A、5+100、diff-check PASS。
- 遗留：当前仅 READY FOR USER ACCEPTANCE，先执行 S01/S02（即 D2-A01a/A01b）；通过后才恢复 A02～A06。无 F2 轮次。
- 本轮实际落地（2026-08-09 21:50:48）：顶部第二行通用工具栏移除“区域绘制”；真实右侧“地图编辑器→地图→地图工具”挂载区域绘制控件；运行时测试改为验证真实 Right/MapEditorPanel 祖先链与选中态深色文字。Picking、Draft、Preview、Renderer、Region 未修改。
- F1-A-UI03 返工（2026-08-09 22:01:32）：确认原实现仅约束 ToggleButton 父控件，子级文本未被四态状态选择器锁定；新增 `mapToolLabel` 四态深色文字约束，并冻结浅色 Normal/Hover/Selected/Selected+Hover 背景与边框。运行时验证 Normal/Selected 最终文本颜色均为 `#243744`，静态覆盖合同补齐四态 selector；Picking、Draft、Preview、Renderer、Region 未修改。
- UI 可读性与布局返工（2026-08-09 22:10:04）：顶部通用工具栏选中态内部文字统一锁定 `Color.Text.Primary`，避免浅色选中背景上的白字；区域绘制按钮显式水平/垂直居中。Runtime Focus 7/7；Solution Build 0W0E；Core 345/345、World 952/952、WarCore 22/22；ARCH-A、diff-check PASS。
- MAP-A-R3-F1-B（2026-08-09 22:21:04）：复用现有 `VulkanNativeHost → UiVm → MapSurfacePicker → ViewProjection/WorldRayFactory` 链路；区域绘制命中只记录真实 `MapPoint` 并反馈底部状态，不创建 Draft、不提交 Region。B01～B07 与既有 Runtime 聚焦测试通过；F1-C 未启动。
- MAP-A-R3-F1 收口（2026-08-09 22:36:56）：真实命中恢复 Draft 首点、连续顶点、光标预览边、Enter 闭合提交、Esc 取消与正式 Region 快照渲染；补充 DPI 1.75 逻辑坐标回归与完整 Runtime 合同。98e3728 保留为失败中间提交；本提交完成 F1 代码收口，等待用户完整真机验收，F2 未启动。
- F1 收口补充（2026-08-09）：新增 Resize Runtime 回归 R16；最终 World 968/968，Core 345/345，WarCore 22/22，Build 0W0E，ARCH-A 与 diff-check PASS。
## v0.2.25.2-rz
MAP-A-R3-D2：Region Drawing 实装与真机验收前收口（2026-08-09）。
- 变化：新增区域绘制工具入口；复用既有相机投影完成地图表面拾取；左键添加顶点、移动预览边、首点闭合候选、Esc 取消；闭合调用 `MapEditSession.CreateRegion`，正式区域与临时草稿进入现有静态模型渲染路径。
- 验证：解决方案 Build 0 Warning / 0 Error；Core 345/345；World 947/947；WarCore 22/22；ARCH-A PASS；5+100 PASS；`git diff --check` PASS。L1 静态 UI PASS，L2 Headless PASS，L3 Visual Regression NOT ENABLED，L4 真机验收 PENDING。
- Hash：以本轮最终提交为准。
- 遗留：等待用户执行 D2-A01..D2-A06 真机 IPO 验收；未进入 D3。
## v0.2.24.50-fix
MAP-A-R2-D5-F5：LayerPanel 根因收口与 Runtime UI Gate 首次落地（2026-08-09 19:42:41）
- 变化：新增 Avalonia.Headless 12.0.4（仅 World.Tests）；建立可复用 Headless Fixture/Host；LayerPanel 改为 Auto/Auto/* Grid，修复冷启动与增层宽度稳定性；将 Layer/Top/Foot 状态覆盖收口到模板 Presenter 与项目 Token；新增 7 项 Runtime UI 门禁。
- 验证：解决方案 Build 0 Warning / 0 Error；Core 344/344、World 938/938、WarCore 22/22；Runtime UI 7/7；Visual Regression：NOT ENABLED；F5 真机验收 8/8 PASS；UI D6 CLOSED。
- Hash：60fd339。遗留：R2 未完成需求已转入 `docs/milestones/current/MAP-A/R3-backlog.md`；状态 MAP-A-R2：CLOSED，下一阶段 MAP-A-R3。

## v0.2.24.49-fix
MAP-A-R2-D5-F4：图层列表测量与拖拽热区修复（2026-08-09 16:18:16）
- 根因：图层页外层 ScrollViewer 未禁止横向无限测量，Inspector 隐藏时名称 `*` 列失去可用宽度；拖拽事件直接绑定 14 DIP Path，命中区过小。
- 修复：图层页禁用横向滚动并保持页面、面板、ListBox 横向拉伸；拖拽改由 24×28 DIP 透明 Border 作为实际 Pointer 热区，图标仅作视觉子元素。
- 验证：解决方案 Build 0W0E；Core 344/344、World 931/931、WarCore 22/22；架构守卫 PASS；`git diff --check` PASS。真机验收待用户执行。
- 范围：未修改 Gizmo、世界原点 Overlay、Vulkan 日志策略、Inspector 其他页、顶栏、地图渲染、Schema、宪法。
- 状态：**MAP-A-R2-D5-F4：READY FOR USER ACCEPTANCE**；不得视为 CLOSED，等待用户执行 F4-01～F4-07 真机验收。

## v0.2.24.48-fix
MAP-A-R2-D5-F3：图层行与手柄拖拽修复（2026-08-09 15:58:47）
- F2 真机裁定：`MAP-A-R2-D5-F2` 为 FAIL；Gizmo、世界原点 Overlay、日志降噪通过，图层拖拽与图层视觉阻塞，保持未 CLOSED。
- 根因：图层网格未把名称作为可扩展主体；`TargetAt` 以未转换的条目边界命中 `LayerList` 坐标，拖拽经过条目时目标索引可能为空；上一轮捕获状态也未记录源索引或解绑事件。
- 修复：图层行固定为「手柄 / 类型 / 名称 / 可见 / 锁定」，名称占 `*` 主体列，ListBoxItem 拉伸到面板宽度，选中态继续使用浅背景，排序提示降级为次要帮助文字；拖拽只从手柄启动，记录 source index，按转换后的条目中心计算 target index，释放时一次提交并完整清理捕获/事件状态。
- 测试：F3 聚焦合同与拖拽/领域回归 **10/10 PASS**；UI F3/D6 聚焦合同 **19/19 PASS**；完整解决方案 Build **0W0E**；Core **344/344**、World **931/931**、WarCore **22/22** PASS；架构守卫 PASS；`git diff --check` PASS。
- 范围：未修改 Navigation Gizmo、世界原点 Overlay、Vulkan 日志策略、Schema、依赖或宪法；新增 `UiF3LayerRowContractTests`，同步 `file-tree.md`。
- 状态：**MAP-A-R2-D5-F3：READY FOR USER ACCEPTANCE**；不得视为 CLOSED，等待用户执行 F3-01～F3-06 真机验收。

## v0.2.24.47-fix
MAP-A-R2-D5-F2：Navigation Gizmo DIP、局部图层拖拽与日志降噪重做（2026-08-06，Commit 本轮落库为准）
- T1：普通 revert 撤销 F1，保留世界原点 Overlay、焦点框修复与 PowerShell 守卫兼容；恢复提交 `ecb9134` 已推送。
- T2：Navigation Gizmo 使用独立 `gizmoParams.w=RenderScaling`，Shader 在 DIP 空间计算，物理 viewport/scissor 不变；CPU/Shader 不重复缩放。
- T3：移除全局 DragDrop，改为六点手柄局部 Pointer Capture；不替换 ItemsSource、不移除源行、不禁用窗口；NoRebuild、周期选择投影与命令缓冲摘要不再刷屏。
- 状态：MAP-A-R2-D5-F2：READY FOR USER ACCEPTANCE；D6 顺延至 v0.2.24.48-rz，未创建 Tag/Release。

MAP-A-R2-D5：焦点框作用域与世界原点 Overlay 修复（2026-08-06，Commit 本轮落库为准）
- **焦点框**：视口原生宿主设为不可聚焦并清除 FocusAdorner；布局分隔器不显示焦点装饰；Button 保留正式 `2 DIP` 焦点框与 `1 DIP` 外偏移合同。
- **世界原点**：DrawPlan 调整为实体/轮廓之后绘制；WorldOrigin 管线关闭深度测试并保持关闭深度写入；片元 Shader 不再写入深度，中心标记保持屏幕恒定尺寸，模型或地面不再遮挡。
- **测试**：新增原点 Overlay 深度与 Shader 合同；Core **340/340**、World **928/928**、WarCore **22/22** 全部通过；完整解决方案 Build **0W0E**；`git diff --check` PASS。
- **架构守卫**：仅为 `scripts/arch-a-guard.ps1` 补 UTF-8 BOM 以兼容 Windows PowerShell 5.1；解码后脚本文本、命令与逻辑不变，随后守卫 EXIT=0，5+100 PASS。
- **治理**：版本 v0.2.24.44-rz → **v0.2.24.45-fix**（四处同步）；无 Schema/依赖/Tag/Release 变更；file-tree 无结构变化，无需更新。
- **状态**：**MAP-A-R2-D5：READY FOR USER ACCEPTANCE**；等待用户真机复验焦点框四边、100%～200% DPI 与世界原点遮挡场景。

## v0.2.24.44-rz
ARCH-UI-SPEC-R1-D6：DPI、键盘/可访问性、减少动画、日志性能与剩余 UI 债务复核（2026-08-06，Commit 本轮落库为准）
- **DPI/缩放合同**：新增 `UiDpiContract`，冻结 100%/125%/150%/175%/200% 桌面缩放清单；主窗口最小/推荐 DIP 尺寸与检查器/地图表单宽度阈值保持 DIP 口径，不做物理像素补偿。
- **键盘与可访问性**：新增 `UiAutomationNamer` 与窗口打开后的自动补名；地图页、地图表单、图层工具、日志筛选/回到底部等 D4/D5 新增交互控件显式声明 `AutomationProperties.Name`；自动补名拒绝导出 ARCH/D6 等内部治理代号。
- **减少动画合同**：新增 `UiMotionPreference`/`UiMotionContract`，Reduce 模式下非必要 Hover/Dialog 动效时长归零；Default 模式保持 80/120/180ms 短反馈；未新增 Token。
- **日志与通知性能**：`EditorLogBuffer.MaxEntries` 升为公开合同常量并保持 500 条尾窗；D6 测试覆盖日志上限与相邻重复项压缩；通知自动消失策略独立为 `UiVm.NotificationLifetime`。
- **范围收口**：未接入地图持久化；未新增业务功能；未新增 Token；未触碰 Render/Vulkan/Shader/Gizmo/WarCore/AI 宪法/本地技能。D5-DEFER-01 仍归未来独立地图持久化专项。
- 验证：全解决方案 `--no-incremental` 串行 Build **0W0E**；Core **339/339**、World **928/928**、WarCore **22/22**，合计 **1289/1289 PASS**；启动冒烟 PASS（`XuanYu.Editor.App.exe` 存活 8 秒）；arch-a-guard PASS；git diff --check PASS。
- 治理：版本 v0.2.24.43-rz → **v0.2.24.44-rz**（四处同步）；file-tree 登记新增 D6 文件；未创建 Tag/Release。
- 状态：**ARCH-UI-SPEC-R1-D6：READY FOR USER ACCEPTANCE**（自动化通过不等于 CLOSED；等待用户按 D6-A1 真机验收）。
- 保留：D5-DEFER-01 地图「保存并新建」仍等待未来地图持久化专项；D6-A1 真机验收未执行。

## v0.2.24.43-rz
ARCH-UI-SPEC-R1-D5：控件状态、表单、弹窗、通知、空状态与日志治理（2026-08-06，Commit 本轮落库为准）
- **按钮治理**（D5-FIX-01 统一处理）：`Design/UiStyles.D5.axaml` 新增——Button 内容水平/垂直居中（HorizontalContentAlignment/VerticalContentAlignment=Center，禁止逐按钮 Margin 偏移修补）；完整状态 Normal/Hover（Color.Hover.Bg+Border.Strong）/Pressed（边框 Accent）/Focused（Color.Focus 边框 1px，不跳动）/Disabled（Text.Disabled+Bg.Control）；危险按钮 `Button.uiDanger`（Color.Danger 底 + 白字）；全局 Button 色迁移正式 Token（Bg.Panel/Border.Default/Text.Primary），基线 -5。
- **表单状态**：TextBox 完整状态样式（Normal/Hover/Focus/Disabled，全部 Token）+ `TextBox.error`/`TextBox.warning` 边框类；地图属性表单 6 个输入框绑定 `IsMapFormError`；**错误反馈非仅颜色**（错误图标 ErrorIcon + 说明文字 + 输入框红色边框三重表达，规范 §11.2）；提交反馈通知（应用地图属性成功→Success 通知/失败→Error 通知 + 表单错误区）；表单错误行在无错误时隐藏。
- **弹窗系统**（新）：UiWin 内置 DialogHost（遮罩 + 卡片，XAML 模板化）；`ShowMessage`/`ShowConfirm`/`ShowDanger` 三形态；**危险弹窗默认按钮=取消（非危险），Enter 触发默认按钮、Escape 取消**；未保存确认重构为 DialogHost（保存=默认/不保存=危险/取消=Escape），删除代码构建 Window（基线 -11）；**新建地图**走危险确认（替换地图属性+清空历史，不可撤销）；**删除图层**走危险确认（UiVm `DangerousCommandConfirmRequested` 事件 + `ConfirmDangerousCommand`，未注入处理器时保持直接执行兼容既有测试）。
- **通知系统**（新）：UiVm 四级通知状态机（`UiNotificationLevel` Info/Success/Warning/Error；`NotifyInfo/Success/Warning/Error`）；**不刷屏**——只保留最新一条（序列号递增）；技术详情由调用方写入既有日志系统；Footer 通知条（级别图标 + 单行省略 + 完整 Tooltip，四级色 Token）；真实触发点：地图属性应用成功/失败、日志复制（既有）。
- **空状态**：日志空状态区分两类——初次/无数据（「暂无日志」）与**筛选无结果**（「没有匹配的日志」+「清空筛选」入口，`ShowNoFilterResults`）；检查器空状态保持（D4）。
- **日志治理**：自动跟随与用户上滚互不冲突（既有 `LogAutoScrollPolicy` 保留）；**「回到底部」按钮**（用户离开底部时右下角显示，点击恢复跟随并隐藏——控制器新增 `TailStateChanged` 事件）；级别视觉/筛选/行高/列宽保持（D4/D3 验收基础）；搜索框占位保持（不扩张）。
- **Token 迁移**：Manifest 保持 **112 Frozen / 0 PendingReview**（未新增 Token——按钮/表单状态色全部映射现有 Token：Color.Hover.Bg/Focus/Danger/Bg.Control/Bg.Panel 等）；**债务基线 159 → 143（-16）**：Ui.axaml Button 状态色×5 + UiWin.UnsavedDialog 代码 Window 颜色×11（真实代码迁移）；未用 Locator/AllowedCount 掩盖。
- 验证：全解决方案 `--no-incremental` 串行 Build **0W0E**（落盘 /tmp/d5-final-build.log）；Core 339/339、World 852/852（+29 D5 测试）、WarCore 22/22，合计 **1213/1213 PASS**；启动冒烟 PASS；arch-a-guard PASS（版本一致性检查有效）；git diff --check PASS。
- 治理：版本 v0.2.24.42-fix → **v0.2.24.43-rz**（四处同步）；未创建 Tag/Release。
- **审查纠偏（REVIEW BLOCKED → 修复，同版本 v0.2.24.43-rz 不升版，2026-08-06）**：
  - **危险操作 fail-closed（硬阻塞修复）**：删除图层等危险操作——确认处理器缺失时**阻止执行并记录错误**（不再为测试兼容绕过安全流程）；只有用户明确确认（`ConfirmDangerousCommand`）才执行；取消（`CancelDangerousCommand`）不执行；既有测试改为显式注入批准确认服务。
  - **新建地图未保存流程（硬阻塞修复）**：无未保存修改 → 直接新建（不弹窗）；有修改 → **保存并新建 / 不保存并新建 / 取消**（`HasUnsavedMapChanges` 以表单与地图值一致性判定，非 IsDirty）；危险按钮写**具体动作**（「不保存并新建」「删除图层」），不以「继续」代替。
  - **字段级校验（硬阻塞修复）**：`MapWidthError/MapDepthError/MapBaseHeightError` 每输入框只绑定自身错误（不再统一全局染红）；**ValidateOnInput**（输入即清除）/ **ValidateOnLostFocus**（失焦单字段校验）/ **ValidateOnSubmit**（提交全校验）；`FirstInvalidField` 提交后自动聚焦第一处错误；`FormErrorSummary` 页面汇总；校验失败不清空输入。
  - **日志空态互斥（硬阻塞修复）**：`ShowInitialLogEmpty`（「全部」筛选且无日志）与 `ShowNoFilterResults`（非全部且无结果）严格互斥；筛选空态提供「清空筛选」入口。
  - **焦点合同**：官方 **FocusAdorner** 焦点环（2 DIP 焦点框 + 1 DIP 外偏移，`Button:focus-visible` 模板化，不占布局/不改变控件尺寸/不裁切；Setter 值用 `<Template>` 包裹避免运行时 Setter-Control 异常）；Hover/Pressed/Focused 形态互不相同；弹窗 **Tab/Shift+Tab 焦点陷阱**（`DialogFocusTrap` 纯逻辑可测）与**关闭后焦点返回原控件**。
  - **通知合并/关闭/优先级**：同类同文案合并计数（「保存成功 ×5」，`NotificationCount/ShowNotificationCount`）；`DismissNotification` 可关闭；**优先级 Error(3) > Warning(2) > Success(1) > Info(0)**——高优先级不被低优先级覆盖；`CreatedAt` 生命周期（自动消失策略归 D6）。
  - **加载/失败/重试**：打开场景失败弹窗改为 **ShowRetryAsync**（重试/取消，重试重新加载同一路径，循环安全）；错误/警告弹窗宿主化到 DialogHost（ErrorIcon/WarningIcon 图标，非仅颜色）；无真实加载流程的场景（地图持久化 D6 接入）在报告中登记为 D6 触发项。
  - **日志视觉 Token 化**：Foot.axaml 全部原始色迁移正式 Token（logFilter/logHead/logMono/logList 选中/FooterMode/搜索图标/日志边框底色/RepeatText→Log.RepeatText），基线 -12；UiWin.Dialogs 代码 Window 颜色全部清除（宿主化），基线 -9。
  - **5+100 拆分**：UiVm.Logging 按职责拆（State/Refresh）、UiVm.MapEditor.Validation 独立、UiWin.DialogHost.Danger 独立、DialogFocusTrap 独立；恢复多行书写（无单行压缩逃避）；Foot 日志区 99 行、SceneCommands 97 行。
  - **Inter 零残留**：删除 `.WithInterFont()`（Avalonia.Fonts.Inter）；FontFamily 冻结链修正为 `Microsoft YaHei UI, Segoe UI, Noto Sans CJK SC`（D1 规范，禁止 Inter）。
  - 验证：新增 **UiD5CorrectionBehaviorTests（7 项）+ UiD5CorrectionNotifyTests（5 项）+ UiD5CorrectionStructureTests（7 项）**，合计 **19 项纠偏测试**；World **871/871**（852+19）；全量 `--no-incremental` **0W0E**（落盘 /tmp/d5-fix-final-build3.log）；**Core 339 + World 871 + WarCore 22 = 1232/1232 PASS**；启动冒烟 PASS（含 FocusAdorner 模板化运行时验证——首次直跑捕获 Setter-Control 崩溃并已修复）；arch-a-guard PASS；git diff --check PASS。
  - 债务基线 **143 → 122（-21）**；Manifest 112 Frozen / 0 Pending（未新增 Token；FocusAdorner 基于 Color.Focus 派生）。
- **二次审查纠偏（REVIEW BLOCKED → 修复，同版本 v0.2.24.43-rz 不升版，2026-08-06，按用户方案逐字执行）**：
  - **硬阻塞一：未保存地图判断（按用户方案）**——`HasPendingMapFormChanges`（表单值与当前模型不一致）+ `HasUnsavedMapChanges = MapSession.IsDirty || HasPendingMapFormChanges`（图层/显隐/锁定/已应用未落盘全部捕获）；**默认地图基线修正**：MapEditSession 新增 `MarkBaseline()`（内存基线保存点，不动路径），IsDirty 改为保存点判定（`SavedStateId is null || CurrentStateId != SavedStateId`）——初始未修改的默认地图不误判为未保存（新建不弹窗）；新建（CreateNewMap 清空保存点）与任何修改仍为未保存。
  - **「保存并新建」停止上报**：地图持久化（真实保存到资产文件）尚未接入，归未来独立地图持久化专项——**不使用「保存并新建」文案，禁止用「应用属性」冒充保存**；未保存弹窗为「不保存并新建」（危险/具体动作，明确丢弃）/「取消」（默认焦点）；仅 discard 放行新建，取消 → 原地图与全部修改保持不变；未来专项接入真实保存后恢复三选（登记）。
  - **硬阻塞二：输入阶段真实校验（按用户方案）**——`ValidateMapFieldOnInput` 替换「只清错」逻辑：输入阶段执行轻量规则（非法字符/NaN/Infinity/**明显超界** 100~1000000 米，边界与领域 MapDefinitionValidator 一致）；**值仍非法时错误不得消失**（继续输入非法值错误保持）；合法值错误立即清除；**输入中态**（空/-/./1./1e- 等临时文本）不清除已有错误；失焦执行完整单字段校验（格式+范围）；提交执行全部字段+跨字段（MapSession 业务兜底）+ 定位第一处错误 + 页面汇总；校验失败不清空输入；基础高度仅有限数字（领域 ValidateSurface 无范围）。
  - 验证：新增 **UiD5InputValidationTests（8 项）+ UiD5UnsavedFlowTests（8 项）**，World **887/887**（871+16）；全量 `--no-incremental` **0W0E**（落盘 /tmp/d5-fix2-final-build.log）；**Core 339 + World 887 + WarCore 22 = 1248/1248 PASS**；启动冒烟 PASS；arch-a-guard PASS；git diff --check PASS；既有测试适配 4 处（默认地图状态「已保存」、范围错误字段级拦截、新建地图流程断言、MapStatusText 基线语义）。
  - 债务基线 122 保持；Manifest 112 Frozen / 0 Pending。
- **D5-FINAL 最终语义纠偏（同版本 v0.2.24.43-rz 不升版，2026-08-06）**：
  - **地图状态四态**：无路径+无修改 → `未落盘`；无路径+有修改 → `未保存`；有路径+无修改 → `已保存`；有路径+有修改 → `有未保存修改`（内存基线 ≠ 已保存到磁盘；`MarkBaseline()` 保留且不改变文件路径）。刷新点全覆盖：表单输入（三个 setter 刷新 MapStatusText）、应用属性/图层增删排序显隐锁定/Undo/Redo/新建（`DirtyChanged` 订阅 `OnMapDirtyChanged`）、打开场景加载地图后同步表单文本（`SyncPropertyTexts`，避免误判待提交修改）。
  - **弹窗去内部编号**：正式文案「当前地图有未保存的修改。当前版本暂不支持保存地图后新建。请选择取消，或不保存并新建。」（无 D5/D6/MAP-A/ARCH-UI-SPEC-R1）；按钮严格「取消 / 不保存并新建」；默认焦点=取消；Enter 只触发默认按钮（取消）；Esc/关闭=取消；仅明确点击「不保存并新建」才允许丢弃修改；确认服务缺失/异常时不新建（`return choice == "discard"` 天然 fail-closed）；不调用场景保存、不调用不存在的地图保存、不用「应用地图属性」冒充保存。
  - **延期登记 D5-DEFER-01**：地图「保存并新建」暂缓——归属未来独立的地图持久化专项（**不归入 D6**）；专项必须补：保存成功后才新建/失败不新建/取消路径不新建/防重复提交/成功后更新路径与状态/写盘失败保留地图图层历史。
  - 验证：新增 **UiD5MapStatusTests（8 项）+ UiD5UnsavedDialogTests（8 项）+ UiD5UnsavedDialogBehaviorTests（9 项）**，共 25 项覆盖计划 26 个断言点；World **912/912**（887+25）；全量 `--no-incremental` **0W0E**（落盘 /tmp/d5-final-build.log）；**Core 339 + World 912 + WarCore 22 = 1273/1273 PASS**；启动冒烟 PASS；arch-a-guard PASS；git diff --check PASS；D3/D4/D5 回归 **156/156 PASS**。
  - 债务基线 122 保持；Manifest 112 Frozen / 0 Pending。
- **D5-A1 真机验收收口（同版本不升版，2026-08-06 用户正式裁决）**：ARCH-UI-SPEC-R1-D5-A1：PASS；D5 指令组 22~30 全部通过；按钮居中、控件状态、表单字段校验、危险弹窗、通知、日志跟随、日志空态、地图状态四态与未保存新建流程均通过真机复验。D5 → **COMPLETE**。
- **D5-DEFER-01 保留**：地图「保存并新建」暂缓，归属未来独立地图持久化专项；不归入 D6，不允许以「应用地图属性」冒充保存。
- 状态：**ARCH-UI-SPEC-R1-D5：COMPLETE**；当前阶段进入 **ARCH-UI-SPEC-R1-D6**（DPI、键盘/可访问性、减少动画、性能、剩余 UI 债务收口）；剩余轮次 D6 + A1。
- 保留（D6 范围）：G02 焦点框系统与 DPI/减少动画/屏幕阅读器；日志搜索实现仍为占位；加载/进度长任务无真实流程，不虚构。


## v0.2.24.42-fix
ARCH-UI-SPEC-R1-D4-F1：单行属性行、文本溢出与字体统一修复（2026-08-06，Commit 本轮落库为准）
- **真机问题定位**（D4 真机验收未通过 → D4-F1）：只读字段被整体竖排（<360 上下）、展示型长文本换行、字体不统一。D4-F1 将响应式拆为两类：**只读键值行始终单行双列**；**可编辑表单行（真实输入控件）才在 <360 整组上下**。
- **规范修订**（`docs/ui/玄域引擎_UI规范_1.0.md` §7.1.1 新增 + §23 变更历史登记）：ReadonlyKeyValueRow（始终同一行；默认标签列 80；组件允许 72～96：地图摘要 72/检查器 80/调试页 96；间距 8；值列 `*` 可收缩；NoWrap+CharacterEllipsis+MaxLines=1；Tooltip 完整值）；EditableFormRow（标准 96/128；仅真实输入控件 <360 整组上下；不得套用只读属性）；展示型动态文本默认（NoWrap+Ellipsis+MaxLines1+完整值 Tooltip）；显式多行例外清单（日志正文/错误详情/帮助说明/空状态说明/多行 TextBox/详情区域）。**未修改任何冻结 Token 数值**（112 Frozen / 0 Pending 不变）。
- **检查器**（InspectorPanel.axaml/.cs）：删除 WideFields/NarrowFields 双布局树（D4-F1 只读字段不再有窄模式）；单套水平 Grid（标签 80 + 值 `*`，MinWidth 0）+ uiLabel/uiValue 公共样式 + 值 Tooltip 完整 Value；分组标题独占一行（uiSection）；空状态说明走 uiMultiline；`InspectorPanel.axaml.cs` 移除模式切换（无输入控件则无响应式）。
- **调试页**（Right.axaml + DebugText.cs + UiVm.Scene.cs + UiVm.InteractionPointer.cs + UiVm.cs）：当前选择/当前工具/拾取状态/日志策略/类型/对象 ID/选中来源/PointerId/起点/当前/位移/Preview次数 全部从拼接字符串结构化为 `InspectorFieldRow`（Label/Value）；三个 ListBox 改为 ItemsControl 键值行模板（Grid 96 列 + uiLabel/uiValue + Tooltip 完整值）；交互事务 Grid（阶段/Owner/Preview）值加单行省略 + Tooltip；按钮文字统一 12。
- **地图编辑器**（MapPagePanel.axaml/.cs + MapEditorPanel.axaml）：资产摘要（名称/路径/MapId/尺寸/状态）始终单行双列（72 列），不随宽度上下拆行；路径/名称/尺寸/状态加完整值 Tooltip；MapId 保持「前 8…后 6」+ 完整 Tooltip + 完整复制；地图属性输入表单（宽度/深度/基础高度）标准 96 列、`<360` 整组上下（`EditableFormLayoutModel` 统一 360 阈值，替代原 320 紧凑模型——`MapEditorLayoutModel.cs` 删除）；MapEditError 走 uiMultiline；环境占位页标题 uiSection。
- **图层**（LayerPanel.axaml + LayerInspectorPanel.axaml）：图层名单行省略（NoWrap+Ellipsis+MaxLines1）+ Tooltip 完整名称；图层属性（类型/顺序/图层 ID）单行双列 96 列 + uiLabel/uiValue + Tooltip；名称输入框保持可编辑；**Layer Token、眼睛/锁、选择、拖动、插入线、Cancel、Undo/Redo、日志与系统保护零改动**。
- **公共语义样式**（Ui.axaml）：新增 `uiLabel`（Label12+Secondary）、`uiValue`（Body13+Primary+NoWrap+Ellipsis+MaxLines1）、`uiSingleLine`、`uiMultiline`、`uiSection`（Section14 SemiBold Primary）、`uiTextButton`（Label12+紧凑高 24），全部引用正式 Token；页面局部 key/value 样式删除（统一公共样式）；sideTab/caption 裸 FontSize 迁移 Token 引用（值不变）；无新增 Token、无新文件超 100 行（Ui.axaml 压缩为项目既有单行 Style 惯例后 26 行）。
- **测试**：新增 UiD4F1LayoutModelTests（只读行 300~480 均水平 + 表单 359/360 阈值）、UiD4F1TextOverflowContractTests（uiValue 默认/检查器/调试/地图/图层省略与 Tooltip/多行专用类/按钮）、UiD4F1TypographyContractTests（公共样式 Token/无裸 FontSize/无局部 FontFamily/Manifest 112 Frozen）；更新 UiD4LayoutModelTests（EditableFormLayoutModel）、UiD4InspectorContractTests（80 列单行）、UiD4MapEditorContractTests（MapId NoWrap+MaxLines1/表单 360）、UiD4LayerContractTests（uiValue/无局部 key）、UiD4DebtClearedTests（新文件清单）、UiLayerVisualContractTests V05/V06（Token 引用）。World 759 → **811（+16）**。
- 验证：全解决方案 `--no-incremental` 串行 Build **0W0E**（落盘 /tmp/d4f1-final-build.log）；Core 339/339、World 811/811、WarCore 22/22，合计 **1172/1172 PASS**；启动冒烟 PASS（XuanYu.Editor.App.exe 存活）；arch-a-guard PASS；git diff --check PASS。
- 治理：版本 v0.2.24.41-rz → **v0.2.24.42-fix**（四处同步）；基线维持 **159 条**（未新增债务、未改允许项）；未创建 Tag/Release。
- 状态：**ARCH-UI-SPEC-R1-D4-F1：READY FOR USER RE-ACCEPTANCE**（尚未获得用户真机复验；复验通过后 D4 改 COMPLETE，失败则建 D4-F2 只修真实失败项）。
- **交付前审查纠偏（REVIEW BLOCKED → 修复，同版本 v0.2.24.42-fix 不升版，2026-08-06）**：
  - **恢复双模型并存**：`MapEditorLayoutModel`（<320 面板紧凑密度：根水平留白 12/8、分组间距 12/8、字段行距 6/4，MapPagePanel 密度接线）+ `EditableFormLayoutModel`（<360 输入表单上下）职责分离、互不替代（319/320 与 359/360 两组边界分别生效，组合测试 6 组）；只读资产摘要始终单行双列；
  - **Ui.axaml 恢复可读性**：放弃 26 行压缩版，恢复多行格式（每个 Style/Setter 正常分行）；D4-F1 公共样式（uiLabel/uiValue/uiSingleLine/uiMultiline/uiSection/uiTextButton）拆分至新 `Design/UiStyles.D4F1.axaml`（75→拆分后 Ui.axaml 76 行 + 样式文件 59 行，全部 ≤100，无压缩单行 Style）；Ui.axaml 仅聚合（StyleInclude 一次）；
  - **按钮真实接线**：`uiTextButton` 提供 ContentTemplate（TextBlock NoWrap+Ellipsis+MaxLines1）与 Tooltip=完整按钮名（绑定 Content）；地图 7 按钮（新建/打开/保存/聚焦/应用修改/撤销/重做）+ 调试 4 按钮（开始/预览/提交/取消）全部真实引用；
  - **按钮布局 v2**：地图属性按钮由横向 StackPanel 改为 `Grid *,*`（应用修改跨两列第一行、撤销/重做第二行等宽，高 28/间距 6）；地图资产按钮为真 2×2 等宽 UniformGrid（Stretch/MinWidth 0/MinHeight 28）；约 300 DIP 下每列约 139 DIP 无裁切；
  - **拆分 MapFormPanel**（新 UserControl：地图属性表单方向切换，职责单一）；测试新增 UiD4F1ButtonContractTests + 边界组合测试，World 811 → **823**；
  - **Stash 说明**：当前设备本地 `git stash list` 为 0（纠偏前后一致）；先前 D2/D3 轮次登记的「2 个历史 Stash」位于另一开发设备，无法在当前设备复现，本设备从未 stash/pop 操作。
- 验证（纠偏后最终代码状态）：全解决方案 `--no-incremental` 串行 Build **0W0E**（落盘 /tmp/d4f1-fix-final-build2.log）；Core 339/339、World 823/823、WarCore 22/22，合计 **1184/1184 PASS**；启动冒烟 PASS；arch-a-guard PASS；git diff --check PASS；版本四处一致（v0.2.24.42-fix 不变）；基线维持 159；Manifest 112 Frozen / 0 Pending。
- 状态：**ARCH-UI-SPEC-R1-D4：COMPLETE**（2026-08-06 用户正式裁决：D4-F1 真机复验 F1-1~F1-9 全部通过；截图「按钮内容未居中」登记 **D5-FIX-01**：按钮内容水平、垂直居中统一——用户批准延期的已知缺陷，不阻塞 D4，由 D5 第一项统一处理，禁止逐按钮 Margin 偏移修补）。

## D4-A1 文档收口（2026-08-06，不升版）
- **D5-FIX-01 登记**（用户正式裁决）：`按钮内容未居中`——截图确认按钮文字/图标未水平垂直居中；适用范围：全部标准按钮；处置：D5 第一项统一处理（按钮内容居中 + 完整状态），禁止逐按钮 Margin 偏移修补；当前 D4 不阻塞。
- D4/D4-F1 全部收口完成：K02/K03/K04 复验 PASS；真机 IPO 组 1~21 + F1-1~F1-9 通过。



## v0.2.24.41-rz
ARCH-UI-SPEC-R1-D4：检查器、地图编辑器与图层工作面板治理（2026-08-06，Commit 本轮落库为准）
- **检查器治理**（K02/G03/W37~W42/W44）：检查器页签内容拆分至新 `Right/InspectorPanel.axaml`（+code-behind）；字号全 Token 化（面板标题 Title16 / 分组标题 Section14 / 字段标签 Label12 / 字段值 Body13 / 空状态标题 Section14）；`InspectorFields` 从字符串拼接改为结构化 `InspectorFieldRow(Label/Value/IsGroupHeader)`（对应 6 个既有测试同步更新为结构断言）；**响应式双模式**（纯逻辑 `InspectorLayoutModel`：内容宽 ≥360 左右布局/标签列 96/字段最小 128，<360 整组上下，同一数据源双布局树切换，无逐字段宽度判断）；空状态保留单一主入口；密度合同（全宽分组标题+1 DIP 分隔线、去卡片嵌套、字段行距 4~6）；调试页标签列 70→96（W44）。
- **地图编辑器治理**（W48/补充裁决）：地图页拆分至新 `Right/MapPagePanel.axaml`（+code-behind）；**只读资产摘要紧凑化**（标签列 72 组件级例外、单行高 24、空路径显示 —）；**MapId 显示压缩**（纯逻辑 `MapIdDisplayFormat`：>18 字符显示「前 8+…+后 6」，TextWrapping=NoWrap + CharacterEllipsis，Tooltip 完整 ID，复制按钮走 Clipboard 复制未经截断的完整 MapId）；地图属性编辑表单标签列 96 + **紧凑模式**（纯逻辑 `MapEditorLayoutModel`：内容宽 <320 整组标签上字段下，标签→字段 2、字段组 6~8，关键操作保留）；操作按钮组间距 6（2×2 均匀网格，按钮文字不换行）；每页单一纵向滚动容器；错误色 Token（W47）。
- **图层面板治理**（K03/K04/W50~W52）：状态图标视口 14→**16**（Icon.Size.Standard）、笔画 1.5、热区 26×24（登记组件例外不变）；可见/隐藏、锁定/未锁定保持形态+颜色双重表达（VisibleIcon/HiddenIcon/LockedIcon/UnlockedIcon 四个正式矢量图标）；全部状态色迁移 Layer.* 冻结 Token（Visible/Hidden/Locked/LockedBg/Unlocked/VisibleBg、Kind.Region.*/Kind.System.*，System.Text 值 #687582→#5D6F7C）；**插入线 → Layer.DropLine #5B8DB8**（2 DIP，用户冻结）；活动标记 → Color.Accent；**选中行显式样式**（Color.Selection.Bg + 按开关类型保留状态色）；类型标签文字（区域/系统）+ 形态 + 低饱和色三重区分；工具栏按钮 24 紧凑档（W50）；图层属性表单标签列 96/字段值 13（W53/W54）；系统图层保护与图层选择/显隐/锁定/拖动/插入线/Cancel/Undo/Redo 业务合同零改动（既有行为测试 795 全绿为证）。
- **守卫缺陷修复**（ARCH-UI-SPEC-R1-D4 发现）：①`arch-a-guard-warcore.ps1` 无条件重新初始化 `$failures`，**清空主守卫在子守卫源入前累积的全部失败**（版本一致性检查恰在其前，失败被吞，门禁长期假 PASS）——改为条件初始化；被源入时不提前 exit（`InvocationName -eq '.'` 时 return，统一由主守卫收尾）。②`arch-a-guard.ps1` 5+100 自验证样本 `"a


b
"`（a+2 连续空行+b=4 行）期望误写 3（SHR-2026-08-D2 引入，同样被吞）——修正为 4。修复后门禁如实检出版本不一致（验证通过后随本条目版本同步消除）。
- **Token 迁移**：Manifest 保持 **112 Frozen / 0 PendingReview**（未新增 Token）；**债务基线 226 → 159 条（-67）**：Right.axaml（W41/W37/W40/W51 等 12 条）、MapEditorPanel.axaml（W45/W46/W47/W49 等 16 条）、LayerPanel.axaml（W50/W51/W52 等 30 条）、LayerInspectorPanel.axaml（W53/W54 等 6 条）、Ui.axaml section（W14 字号+颜色 2 条）全部真实代码迁移；保留 2 条登记组件例外（activeMark 圆角 1.5、dropLine 圆角 1，规范 §5.4）；未用 Locator/AllowedCount 掩盖。
- 验证：全解决方案 `--no-incremental` 串行 Build 0W0E（落盘 /tmp/d4-final-build.log）；Core 339/339、World 795/795（+36 D4 测试）、WarCore 22/22，合计 **1156/1156 PASS**；启动冒烟 PASS；arch-a-guard PASS（含守卫缺陷修复后真实版本一致性）；git diff --check PASS。
- 治理：版本 v0.2.24.40-rz → v0.2.24.41-rz（四处同步）；未创建 Tag/Release。
- 状态：**ARCH-UI-SPEC-R1-D4：READY FOR USER ACCEPTANCE**（尚未获得用户真机裁决；真机通过后 D4 改 COMPLETE，失败则建 D4-F1 只修真实失败项）。
- 保留（审计矩阵归属 D4 但本轮范围外，报告已说明）：K02 真机复验项随 IPO 组 1~4 验收；地图环境页内容补齐（D5 后续）；日志区/弹窗/状态语义（D5）；焦点与 DPI 全量（D6）。

## v0.2.24.40-rz
ARCH-UI-SPEC-R1-D3：主窗口、顶层页签与滚动治理（2026-08-06，Commit 本轮落库为准）
- **主窗口与四区外壳**（W15/W16/W18/W19/W20/W21）：初始 1400×820 → **1360×820**；Min 1100×720 → **1024×640**；左列 Min 200 → **220**；右列 Min 260 → **300**；视口列 Min 360 → **480**（视口最小可用区域 480×320 合同）；RootGrid Margin 12→6 + MinWidth 980→1012（1024 窗口下 6+220+6+480+6+300=1012 恰好满足全部面板最小，无遮挡无溢出）；日志折叠态 MinHeight 32 保留、展开态 120~420（既有 ClampLogRow 登记对齐规范 §7.1）。
- **外壳 Token 迁移**（清除 4 条基线）：UiWin 背景 `#e9eef5` → `Color.Bg.Application`（W17）；UiRoot 分隔条 `#dce4ef/#9fb5d6` → `Color.Border.Strong`/`Color.Hover.Bg`（规范 §9.1 可调整边界 / §9.3 悬停）；视口 1 DIP 边框 `#C9D2DC` → `Color.Border.Default`。**旧债务基线 230 → 226 条，只减不增**；每项下降对应真实代码迁移，未用 Locator/AllowedCount 掩盖。
- **顶层页签单行溢出系统**（W43/G01，合同 §10.1 15 条中本轮范围）：新增 `Right/TopTabStripTemplate.axaml` 页签宿主模板（单行 ScrollViewer + ItemsPresenter 横向 StackPanel，禁止换行；Hidden 滚动条——宽度充足无滚动控件）；左右箭头（宽度不足显示、到达边界禁用、单击步进 96=Token Size.Width.96）；左右边缘低饱和渐隐（基础命名色 White/Transparent，非业务色值）；**滚轮横向路由**（页签条 Grid 隧道消费 e.Handled=true，上滚=向左；离开页签条滚轮自然回到内容区；到达边界后剩余增量不传递——内容区/日志区不是页签条祖先，树结构隔离）；当前页签自动完整可见（SelectionChanged/ScrollChanged 双路径，窗口缩放后保持）；「全部页签」入口（真实页签动态读取，当前项半粗+Accent 标记，点击跳转并自动显露；当前架构无关闭能力，入口只负责发现与跳转，未扩张关闭系统）；首次溢出一次性提示（文案「滚动鼠标滚轮或点击箭头查看更多页签。」，仅当前用户环境首次触发，状态持久化 %APPDATA%\XuanYuEngine\ui-once.json，本会话不重复）。全部新视觉值引用正式 Token 或规范允许值；无每帧/PointerMoved/Hover 日志。
- **分析器门禁完善**（对应正式测试）：`{StaticResource}` 正式 Token 引用在 Setter/内联属性豁免数值与颜色检查（未登记字面量仍 FAIL，正反例见 UiSourceContractAnalyzerTokenRefTests）。
- 验证：全解决方案 `--no-incremental` 串行 Build 0W0E（落盘 /tmp/d3-final-build.log）；Core 339/339、World 759/759（+20 D3 测试）、WarCore 22/22，合计 **1120/1120 PASS**；启动冒烟 PASS（进程存活 10s 无崩溃）；arch-a-guard PASS；git diff --check PASS。
- 治理：版本 v0.2.24.39-rz → v0.2.24.40-rz（四处同步）；未创建 Tag/Release。
- **补充修复（同版本不升版，2026-08-06）**：run.bat 新增 `[0/3]` 清理段（Build 前 `taskkill /IM XuanYu.Editor.App.exe /T /F` + `dotnet build-server shutdown` + 1s 等待，失败以 `|| ver >nul` 兜底不中断流程），根治「上次编辑器实例/MSBuild 节点残留导致 `XuanYu.Editor.UI.pdb` 文件锁 CS2012」；只针对本编辑器进程与 MSBuild 服务器，**禁止 `taskkill /IM dotnet.exe`**（避免误杀其他 .NET 任务）；`timeout` 用 `%SystemRoot%\System32\timeout.exe` 全路径（避免 MSYS PATH 下命中 GNU timeout）；注释全英文（.bat 在 GBK 代码页下 UTF-8 中文会乱码成命令）。验证：干净 cmd 环境 run.bat 全流程 [0/3]→[3/3] PASS（构建 0W0E，编辑器进程启动存活，关闭后无残留）。
- **D3-A1 真机验收收口（同版本不升版，2026-08-06 用户正式裁决）**：真机验收 15 组中 **1～5、7～15 全部通过**；第 6 项（左右滚动箭头）在验收尺寸下未出现——用户明确裁定「滚轮横向导航、渐隐提示、当前页签显露和全部页签入口已覆盖发现与跳转需求」，**批准通过**；最大化/恢复/初始非最大化/最小窗口/连续缩放全部通过；页签、地图面板与日志滚动无穿透串扰；顶层页签保持单行、窗口缩窄后仍可访问全部页签。**登记用户批准偏离项 `D3-EX-01：右侧顶层页签不显示左右滚动箭头`**（适用范围仅限当前右侧顶层页签组件；当前导航合同=滚轮横向滚动+边缘渐隐+当前页签自动显露+全部页签入口；不自动修改 UI Spec 通用规则，D6/A1 若出现键盘可访问性或页签数量增长问题再重新审查）。K01/K05/K06 复验 PASS；D3 → **COMPLETE**。
- 状态：**ARCH-UI-SPEC-R1-D3：COMPLETE（用户真机验收通过，2026-08-06）**。
- 保留（审计矩阵归属 D3 但本轮范围外，报告已说明）：W28 菜单复核（合规无改动）、W29 leftTab 字号（Left.Styles 不在本轮允许范围）、W36 删除菜单色（Left.axaml 不在范围，留 D5）、G02 键盘焦点（留 D6）、G04 面板紧凑/折叠（留后续轮）、K07 日志六档复验（D5）。


## v0.2.24.39-rz
ARCH-UI-SPEC-R1-D2：Token 基础设施与自动化门禁（2026-08-05，Commit 本轮落库为准）
- **Token 基础设施**：新增 `XuanYu.Editor.UI/Design/` 8 个 Token 文件（UiTokens.Fonts / Colors.Core / Colors.Components / Spacing / Controls / Icons / Motion + UiTokens 聚合入口），数值、类型与命名全部直接来自 UI Spec 1.0（112 个 Token 键，无临时/兼容 Token）；`Ui.axaml` 仅新增资源聚合（Styles.Resources 合并 Design/UiTokens.axaml），**未修改任何现有页面视觉、布局、交互或业务行为**（当前编辑器视觉与 D1 基线一致）。
- **Token 合同测试**（`XuanYu.World.Tests/UiTokens/`，复用既有 UI 源码合同测试承载点，无新项目/新依赖）：键全局唯一、代码键集合==规范合同清单（UiTokenContractCatalog 112 键，无缺失无额外）、数值与规范一致（Fonts 21/Colors 38/Sizes 41 项断言）、字号与行高一一配对、聚合入口含 7 子文件、无循环引用、8 个 Token 文件全部 ≤100 行、应用资源已合并聚合入口。
- **源码违规分析器 + 旧债务基线门禁**：测试侧 `UiSourceContractAnalyzer`（HexColor/FontSize/CornerRadius/ControlHeight/BoxShadow/StrokeThickness/EmojiIcon/CsHexColor 八规则，限定 UI 控件语义，Path 图标尺寸/布局容器/CornerRadius 0 不误报）；旧债务细粒度基线 173 条指纹（相对路径+规则类型+规范化值+允许次数，映射 W 编号，自动生成自真实源码现状值）：已知债务允许、新增债务（含同文件第二处同值）测试失败、债务减少允许、基线不自动增长；扫描范围排除 Design/ 与渲染目录。
- **门禁自验证**：10 项正反例（合法 Token 引用 PASS、未登记色/字号 15/圆角 5/高度 34/Emoji 图标/BoxShadow/笔画 2.2 FAIL、布局与图标值不误报、cs 色构造 FAIL）全部通过。
- 审计矩阵新增「六、验证方式标注」：W/G/K 标注自动门禁/真机/混合/暂不可自动化及原因；不宣称 W01~W71 已整改。债务登记更新 D2 状态与基线规则。
- 验证：全解决方案串行 Build 0W0E；Core 339/339、World 809/809（+123 UiTokens）、WarCore 22/22，合计 1170/1170 PASS；arch-a-guard PASS；git diff --check PASS。
- 治理：版本 v0.2.24.38-rz → v0.2.24.39-rz（四处同步）；未创建 Tag/Release。
- 状态：ARCH-UI-SPEC-R1-D2 COMPLETE；D3 主窗口/顶层页签/滚动治理待用户批准启动。

### D2-F1 Token 门禁可靠性纠偏（2026-08-05，Commit 本轮落库为准；同版本不升版）
- 用户复核裁定 D2 REVIEW BLOCKED 四项：基线可换位绕过、测试侧第二套事实源、Token 值覆盖不全（112 键仅 100 项值断言）、扫描范围不足（固定 cs 清单/Design 外 Token/Emoji 图标位置）。
- **基线升级为细粒度指纹**：每条含 W 编号/相对路径/稳定定位（Style Selector → x:Name → 元素类型；code-behind 为 类型名.成员名）/属性名/规则类型/规范化值/允许次数；匹配 Path+Locator+Kind+Property+Value 全部参与。225 条基线重新生成（含 Locator，注释剥离）。10 项绕过反例全部通过：原位置保留 PASS、删除 PASS、同选择器第二处 FAIL、异控件/异 Style/异 x:Name/异属性/异 Kind 换位 FAIL、注释漂移不影响基线、基线不自动增长。
- **单一机器事实源**：新增 `Design/UiTokenManifest.json`（112 条：Key/Type/Value/Category/SpecSection/Purpose/SpecStatus）；`scripts/generate-ui-tokens.py`（≤100 行，无第三方依赖）确定性生成 8 个 Token XAML（带"生成文件"头，幂等验证通过）；删除测试侧手写键清单与 100 项手写期望值表（UiTokenContractCatalog/Fonts/Colors/Sizes 测试），改为 Manifest↔XAML 双向合同（112/112 键、类型、值全覆盖，无重复/缺失/大小写漂移）。
- **SpecStatus 核查（D2-F1）**：97 条 Frozen（规范已冻结具体值，含 Log.Accent.*/DocStatus.*/LogTable.Columns/Tree.Guide 等）；**15 条 PendingReview**（规范只有区间或方向，值待规范审订）：Motion.HoverMs/ExpandMs（规范仅 80～120/120～160 区间，此前取中值为执行 AI 自选，已停止数值冻结）、Layer.Kind.* 6 色、Layer.State.* 6 色、Layer.DropLine（规范 §12.2 仅交互规则无色值）。缺失项已报告，等待规范审订裁决。
- **递归扫描全部 UI code-behind**：删除固定 CsVisualSources；递归 `XuanYu.Editor.UI/**/*.cs`（排除 bin/obj/生成文件）；确认仅 5 个视觉文件含 hex；新增"新 UI .cs 加入原始颜色必被报告"测试。
- **Design 外 Token 声明检测**：非 Design/ AXAML 的 SolidColorBrush/x:Double/x:String/Thickness/CornerRadius/FontWeight/FontFamily x:Key 声明即违规（EditorIcons.axaml 的 StreamGeometry 图标资源合法不误报）。
- **Emoji/Unicode 图标检测**：按钮 Content、切换按钮 Content、图标 TextBlock（Classes/x:Name 特征）Text 与元素内容、Path/PathIcon Data；中文按钮/工具提示/StreamGeometry 不误判（Path 数据用 Unicode 符号区检测，兼容 F1 填充标记）。
- **ResourceInclude 完整图**：聚合含 7 个批准文件、目标存在、子文件不反向引用、应用只合并一次。
- 验证：全解决方案串行 Build 0W0E；Core 339/339、World 719/719（含 UiTokens 33 项）、WarCore 22/22，合计 1080/1080 PASS；启动冒烟 PASS；arch-a-guard PASS；git diff --check PASS。
- 事实表述（修正后）：Token 门禁已实现"已知债务允许（细粒度定位）、任何新增债务（含换位）失败"；112/112 键、类型、值与 Manifest 一致（Manifest 为唯一机器事实源）；15 条组件值待规范审订，未声称全部与规范数值一致；W01～W71 未清零。

### D2-F2 缺失参数冻结与门禁最终加固（2026-08-05，Commit 本轮落库为准；同版本不升版）
- 用户复核裁定 D2-F1 仍 BLOCKED 四项：① 15 个 PendingReview Token 在运行时生效（数值未冻结却已加载）；② AXAML 指纹末级仅元素类型（匿名控件可换位）+ 颜色属性统一记 Color（Foreground/Background 不可区分）+ 2 个 code-behind Unknown；③ code-behind 只扫 hex 字符串（Colors.*/FromRgb/FromArgb/SolidColorBrush/0x 常量未覆盖）；④ 首次 Build 出现 1 个 Warning 后复跑消除且未定位来源。
- **15 项缺失参数由用户正式裁决并写入 UI Spec 1.0**：新增 §12.2.1 图层组件 Token 表（Layer.Kind.Region/System 6 色、Layer.State 6 色、Layer.DropLine）与 §15.3 动效默认 Token（Motion.HoverMs=100 / ExpandMs=140）；其中两项修正执行 AI 旧值：`Layer.Kind.System.Text` #687582→**#5D6F7C**（对比度 4.64:1）、`Layer.DropLine` #7FA8C6→**#5B8DB8**（选中背景对比约 3.04:1）；变更历史登记"用户 D2-F2 缺失参数补充裁决"。Manifest 全部冻结：**112 Frozen / 0 PendingReview**，生成器重新生成 XAML（幂等验证通过，全部 ≤100 行）。
- **AXAML 稳定定位升级 v3（父链定位）**：匿名元素 Locator = `Path:<最近命名祖先|ROOT>/<父类型链>/<类型>:<同父序号>`（如 `Path:Name:LogList/ListBox/DataTemplate/Grid/Border:1`）；颜色违规记录真实属性名（Background/Foreground/BorderBrush/Fill/Stroke/Color 等）；**基线 Unknown Locator = 0**（cs 成员正则补齐 async/无修饰符/const 字段/显式接口实现）。新增 7 项反例：同 Style Foreground→Background FAIL、Background→BorderBrush FAIL、匿名 Border/TextBlock 换位 FAIL、不同父级同类型换位 FAIL、空白/注释/无关属性变化 PASS。基线重生成 **230 条**（父链定位 v3）。
- **code-behind 八类颜色写法全覆盖**：`#RRGGBB`/`#AARRGGBB`、`Colors.*`、`Color.FromRgb/FromArgb/Parse`、`new SolidColorBrush`、`0xRRGGBB`/`0xAARRGGBB` 常量——每种至少一个 FAIL 样例（含 const 字段 Locator）；递归扫描全部 UI .cs 维持。允许清单按"路径+规则类型+API 模式+原因"登记：TreeGuide.cs Render（树引导线渲染色，ALLOW-RENDER）、Win32ViewportHost.cs（Win32 样式常量非颜色，ALLOW-WIN32）；渲染/宿主代码不误报。
- **上一轮 Warning 追溯**：D2-F1 首次 Build 的完整 stdout/stderr 未落盘（仅冒烟日志），**无法追溯警告来源；流程不合规（正式 build 输出必须落盘）——如实登记**。D2-F2 终验复跑时再次出现 1 个警告，本次**成功定位**：`UiTokenManifestGraphTests.cs(32,9) xUnit2013: Assert.Equal(1, Matches.Count) 应改用 Assert.Single`——xUnit 分析器仅在**全量编译**时触发（增量构建跳过分析器重跑，此前"偶发 1 警告"实为增量掩盖）。已修复（Assert.Single），**`--no-incremental` 干净全量重建落盘验证 0 警告 0 错误（d2f2-final-build.log）**。
- 验证：全解决方案串行 Build 首次执行 0W0E（落盘 d2f2-build.log）；Core 339/339、World 739/739（含 UiTokens 53 项）、WarCore 22/22，合计 1100/1100 PASS；启动冒烟 PASS；arch-a-guard PASS；git diff --check PASS。
- 事实表述（D2-F2 修正后）：UI Spec 1.0 已正式冻结全部 112 个 Token；Manifest 112 Frozen / 0 PendingReview；112/112 键、类型、值一致；Locator 无 Unknown；同值在属性/匿名控件/父级之间换位均失败；全部 UI code-behind 颜色构造被覆盖；W01～W71 未清零。
- 治理：TODO 同步纪律经用户明确授权写入宪法第十九条与三个技能（xuanyu-engine-dev / xuanyu-engine-development / xuan-yu-engine-development）；宪法变更记录授权来源。

## v0.2.24.38-rz
ARCH-UI-SPEC-R1-D1：正式规范冻结（2026-08-05，Commit 本轮落库为准）
- `docs/ui/玄域引擎_UI规范_1.0.md` 由 WORKING DRAFT 转为**正式规范（UI Spec 1.0，唯一 UI 规范事实源）**，24 节结构：身份与适用范围、条款分级、字体字号（回退链/字号表/字重/行高）、颜色背景（四级背景/语义色/日志色/文档状态色）、间距尺寸圆角、控件高度与热区、窗口阈值、图标、边框阴影焦点、页签菜单弹窗、表单错误、树列表拖拽、日志加载空状态、键盘可访问、DPI 性能、游戏 UI 边界、Token 层级命名、自动化矩阵、允许清单、受控例外、变更流程、真机验收、版本历史、文档关系。
- **16 项待审订全部裁决**（无待定项）：分组标题=Section 14（列头=Label 12，废止 13 三套并存）；字重四档适用表；回退链 Avalonia 三级+平台兜底（禁 Inter）；行高组合定义+单行控件不套行高；日志级别色=Log.Accent.* 组件 Token；文档状态色=DocStatus 三态组件 Token；渲染/Shader/Gizmo/网格/数据可视化=允许清单边界；宽度等级例外（表格列宽/热区/分隔条=组件级）；控件高度 24/28/32+现状迁移目标表（34→28/30→28/25→24/42→公式）；圆角 3/6/10 场景分配表；阴影（面板禁阴影+悬浮层 4/12/14%+Popup 系统阴影不叠加）；焦点五态区分+焦点框 2/外偏移 1+与选中并存规则；顶层页签 15 条合同入规范；菜单顺序与弹窗按钮顺序冻结；游戏 UI 基础强约束/艺术弱约束边界；D2 自动检查矩阵+允许清单格式+例外八要素。
- **事实源治理**：`docs/governance/ui-spec.md` 降级为历史讨论决策记录（顶部标注「不再作为实施合同」）；审计矩阵与真机清单补充正式规范引用；债务登记 `arch-ui-spec-debts.md` 状态「待立项」→「治理中」（记录 D0/D1 COMPLETE、W01~W71/G01~G08/K01~K07、下一步 D2、暂停新增 UI 功能、未经批准不得创建例外）。
- 本轮未修改任何 UI 视觉、布局、交互或业务行为实现；仅为版本同步修改 `UiWin.axaml` 与 `UiVm.SceneDocument.cs` 中的版本字符串，并同步修改 `run.bat`。未创建 Token；未修改渲染、地图、WarCore、Scene、持久化、宪法与技能；未创建 Tag/Release。
- 治理：版本 v0.2.24.37-rz → v0.2.24.38-rz（四处同步，升版依据：治理文档正式状态变化按项目惯例升版）；未创建 Tag/Release。
- 状态：ARCH-UI-SPEC-R1-D1 COMPLETE；D2 Token 基础设施待用户批准启动。

### D1-F1 治理层级与事实表述纠偏（2026-08-05，Commit 本轮落库为准；同版本不升版）
- 用户复核裁定 D1 REVIEW BLOCKED 两项：① 规范 1.4 治理优先级将 UI Spec 置于代码宪法与开发硬规则之上，且遗漏 AI_DEVELOPMENT_RULES.md 与已冻结架构/领域合同；② changelog 的 D1 条目曾声称文件层面零 UI 修改，与实际（版本字符串修改了 UiWin.axaml / UiVm.SceneDocument.cs）不符。
- ① 规范 1.4 重写为五层治理层级：第一层 AI 开发宪法；第二层 CODE_CONSTITUTION / AI_DEVELOPMENT_RULES / dev-rules 及已冻结架构·领域·持久化合同；第三层 UI Spec 1.0（**UI 设计规则领域内唯一事实源**）；第四层阶段治理计划；第五层阶段局部 UI 合同。明确：UI Spec 不得覆盖第一、二层；第二层内部跨领域冲突不得由 UI Spec 裁定，必须停止实施并上报；冲突时停止并上报、不得自行选择。
- ② changelog 事实表述修正为：「本轮未修改任何 UI 视觉、布局、交互或业务行为实现；仅为版本同步修改 UiWin.axaml 与 UiVm.SceneDocument.cs 中的版本字符串，并同步修改 run.bat。未创建 Token」。明确区分「文件发生修改」与「UI 行为没有发生修改」。
- 全仓错误表述零残留检查：当前文档仅归档 changelog-2026-07（历史记录，按归档原则不改）；changelog.md / 规范 / debts / docs-index / file-tree 已清零。
- 状态：ARCH-UI-SPEC-R1-D1-F1 完成；D1 COMPLETE；D2 Token 基础设施待用户批准启动。

## v0.2.24.37-rz
ARCH-UI-SPEC-R1-D0 启动：UI 治理基线（2026-08-05，Commit 本轮落库为准）
- 执行过程中曾提出 TODO 治理建议（每轮任务列任务清单并同步执行情况），因未获授权且超出 D0 范围，已撤销；是否纳入宪法留待独立审订（2026-08-05 用户复核裁定，见下方 D0-F1 纠偏）。
- 依据用户批准的《ARCH-UI-SPEC-R1 治理实施计划》启动 D0 基线冻结与全量审计（只审计、不整改）：
  - 新增 `docs/ui/玄域引擎_UI规范_1.0.md`：审订工作副本（首批冻结参数 + 26 章条款审订状态 + 16 项待审订清单）；
  - 新增 `docs/ui/玄域引擎_旧UI审计矩阵.md`：全量审计（16 界面 + 5 处 code-behind 视觉源；违规 71 项 W01~W71 + 结构性缺口 8 项 G01~G08，逐项标注整改轮次）；
  - 新增 `docs/ui/玄域引擎_UI真机基线清单.md`：20 组中文 IPO 验收清单 + DPI/窗口覆盖矩阵 + 已知问题 K01~K07 登记。
- 关键审计发现：旧蓝强调色系（#185aa6/#edf4ff/#8cb2e2/#2F80C9 等）与 Accent #326F8A 冲突；全局无字号默认（正文落 12 vs Body=13）；圆角 4/5/7/9 大量越界（规范只允许 3/6/10）；面板阴影 0 14 30 违反「普通面板禁阴影」；窗口 1400×820/1100×720 vs 规范 1360×820/1024×640；顶层页签溢出管理 15 条合同未实现；树图标笔画 2.2 vs 1.5。
- `docs-index.md` 登记 docs/ui/；`file-tree.md` 目录树与职责索引同步。
- 治理：版本 v0.2.24.36-rz → v0.2.24.37-rz（四处同步）；未创建 Tag/Release。
- 状态：ARCH-UI-SPEC-R1-D0 完成；D1 规范冻结为下一轮；MAP-A 功能开发按计划暂停至治理收口。

### D0-F1 治理纠偏（2026-08-05，Commit 本轮落库为准；同版本不升版）
- 用户复核裁定 D0 REVIEW BLOCKED 三项：① 执行 AI 越权修改开发宪法与三个 skills；② 正式测试门禁未执行；③ 汇报 SVG 违反项目浅色规范。进入 ARCH-UI-SPEC-R1-D0-F1 纠偏。
- ① 越权恢复：宪法恢复至 a84fd2e 前内容（第十九条 TODO 条款已撤销，未触碰其他条款）；三个本地 skills（xuanyu-engine-dev / xuanyu-engine-development / xuan-yu-engine-development）恢复原状（技能库不在 Git 仓库，恢复结果无法以仓库证据验证，如实说明）；changelog「用户已裁定 TODO 入宪」表述已删除。
- ② 正式门禁补齐：全解决方案串行 Build 0W0E；Core.Tests / World.Tests / WarCore.Tests 全量通过（真实数量见本轮验证段）；arch-a-guard PASS；git diff --check PASS。
- ③ 浅色 SVG：重新生成浅色版（白/极浅蓝灰底、深蓝灰文字、低饱和强调、无渐变），仅临时汇报不入库。
- file-tree 纠偏：删除重复迷你 docs 树节点（目录树 docs 唯一）；docs/ui/ 层级正确；修复版本号规范路径八进制乱码；16 个 docs 文件职责全部补齐（非 docs 的 126 处职责待补为历史欠账，登记后续治理轮）。
- 状态：ARCH-UI-SPEC-R1-D0-F1 完成；D0 COMPLETE；D1 规范冻结待启动。

## v0.2.24.36-rz
UI 规范 1.0 讨论初稿落库（2026-08-05，Commit 本轮落库为准）
- 新增 `docs/governance/ui-spec.md`：ARCH-UI-SPEC-R1 讨论汇总初稿（45 项关键决策）落库，覆盖规范定位与例外机制、三级 Token 体系、颜色/文字/间距/布局、页签/图标/反馈/表单/键盘/拖拽/菜单/弹窗/空状态/高密度组件/DPI/动效、游戏 UI 边界、执行闭环、当前局部基线（MAP-A-R2-D4-F3）与待补充项；状态待审订，Token 数值/测试矩阵/整改计划未冻结（不得由实施 AI 自行决定）。
- `docs-index.md` 登记新文档；`file-tree.md` 目录树与职责索引同步。
- 治理：版本 v0.2.24.35-fix → v0.2.24.36-rz（四处同步）；未创建 Tag/Release。

## v0.2.24.35-fix
MAP-A-R2-D4-F3 预验收补丁：图层状态反馈与通知时序修正（2026-08-05，Commit 本轮落库为准）
- P1 状态图标真实切换：`MapLayerRowViewModel` 增加派生状态 `IsHidden`/`IsUnlocked`（IsVisible/IsLocked 变化时同步通知）；行模板分别显示 VisibleIcon/HiddenIcon、LockedIcon/UnlockedIcon（形状与颜色共同表达状态），保留 F3 配色合同与 26×24 热区。
- P2 拖动插入线通知：`IsDropBefore` 从无通知自动属性改为 backing field + `Set` 通知；插入线补 `Grid.ColumnSpan="6"` 与 `ZIndex="10"`（整行宽、置顶）；`SetDropTarget(null)` 清理所有行插入线。
- P3 同位置拖动 No-op：`CommitLayerDrag` 在 `before == after` 时立即返回——不写日志、不改 FooterMessage、不 Dirty、不增历史（会话层本就 No-op，UI 层补齐静默）。
- P4 通知时序：`EditorLogSummary.ChooseLatest` 由"先扫全部 Error/Warning 再扫 Editor"改为单次逆序扫描（最新一条 Error/Warning/Editor/Project 即返回）——旧警告不再永久霸占底部通知，新操作可取代已处理的旧警告，新警告仍覆盖旧操作；Vulkan/Render Info 在完整日志中按真实时间保留。
- P5 拖动异步收口：`DragCandidate_PointerMoved` 改 `async void` + `await DragDrop.DoDragDropAsync`，`finally` 中清理插入线（拖动结束才清理，异常不成为未观察任务）；无 Sleep/Timer/fire-and-forget。
- 验证：全解决方案 Rebuild 0W0E；Core 339/339、World 686/686（+8：A/B/C/D/E/F/G/H）、WarCore 22/22；arch-a-guard PASS；git diff --check PASS。
- 遗留/下一步：F3 真机补验（F3-A01~A16 + P-A01~P-A06）→ A5 全量复验 → D4 COMPLETE；ARCH-UI-SPEC-R1 立项讨论；D5 区域绘制。

## v0.2.24.34-fix
MAP-A-R2-D4-F3 图层视觉、拖动排序与通知收口（2026-08-05，Commit 本轮落库为准）
- F3-01 状态图标重做：取消强蓝实心底，改为图标本体为主浅色底辅助（可见 #326F8A/#EAF3F7/#BDD5DF、隐藏 #8995A2；锁定 #7A6238/#F4EFE5/#DCCDAE、未锁定 #7B8794）；热区 26×24 DIP、图标 14、圆角 4、独立 ToolTip。
- F3-02 类型标签区分：区域 #E8F3F6/#326B7B/#B9D7DE（蓝青），系统 #F0F2F4/#687582/#D5DBE0（灰蓝）；系统层不显示拖动手柄，区域层显示六点手柄。
- F3-03 右侧字号收敛：顶层页签 15→13（Ui.axaml sideTab）、地图二级页签新增 layerSubTab 14 选中半粗、字段标签 12 / 字段值 13 / 按钮 12；不扩张全局主题。
- F3-04 区域图层拖动排序：`MapLayerStack.MoveRegionToIndex`（纯函数，targetIndex 0=最上区域层，Ground/Boundary 固定 Order 0/1，区域层 Order 连续唯一，越界/同位置安全返回原集合）；`MapEditSession.MoveLayerToRegionIndex` 单历史节点命令（系统层/未知/越界失败零污染，同位置 No-op 不 Dirty 不增历史）；UI 用 Avalonia DragDrop（DataTransfer 文本载荷 LayerId、4 DIP 启动阈值、2 DIP 插入线 #7FA8C6、仅区域行接受 Drop、一次 Drop 一次提交一次日志）；上移/下移按钮保留。
- F3-05 通知优先级（方案 B）：Vulkan 日志经 Dispatcher.Post 异步到达（根因确认）覆盖用户通知；`EditorLogSummary` 改为选择策略——最新 Error/Warning > 最新 Editor/Project 动作 > 最新兜底；完整日志面板仍按真实时间保留 Vulkan 记录；无 Sleep/计时器。
- F3-06 登记 `ARCH-UI-SPEC-R1`（docs/governance/debts/arch-ui-spec-debts.md，17 项范围，待立项，不展开实施）。
- 验证：全解决方案 Rebuild 0W0E；Core 339/339、World 678/678（+33：T01-T08、H01-H06、U01-U06、V01-V06、L01-L05）、WarCore 22/22；arch-a-guard PASS；git diff --check PASS。
- 遗留/下一步：F3 真机补验（F3-A01~A16）→ A5 全量复验 → D4 COMPLETE；ARCH-UI-SPEC-R1 立项讨论；D5 区域绘制。

## v0.2.24.33-fix
MAP-A-R2-D4-F2 顶部菜单与右侧冗余 UI 收敛（2026-08-05，Commit 本轮落库为准）
- F2-01 顶部"添加"菜单扁平化：删除"基础实体"级联层，"立方体"成为"添加"直接子项（Top.axaml）；与"文件"共用同一 Menu/MenuItem 样式（背景/边框/行高/留白/悬停反馈/字号全一致），无横向级联箭头；命令与 CommandParameter（添加立方体）零改动，未新建第二套命令。
- F2-02 删除右侧顶层"偏好"页签及占位面板（布局保存/主题/快捷键占位）；同步清理仅服务该页的绑定字段：`PropertyItems` 属性（UiVm.cs）与 `UiText.PropertyItems` 字段整体删除。
- F2-03 删除右侧顶层"模式"页签（非可选模式、无控件、制造无意义空白）；右侧顶层收敛为 检查器 | 地图编辑器 | 调试，地图编辑器二级导航（地图/图层/环境）直接贴近顶层页签；`SnapMode` 属性保留（工具状态读取，注释登记暂无 UI 消费者）。
- F2-04 图层锁定日志细化：新增专用帮助函数 `LogLayerLockChanged(layer, before, after)`（不再用 FormatBoolean 拼"是/否"）；消息列"锁定图层：区域 1（区域）/解锁图层：边界（系统）"，详情列 `LayerId=<完整 ID>；状态：未锁定 → 已锁定`；仅状态真实变化记录一次（同值 No-op 不记录），失败日志保留原因；LogLayer 增加详情列参数重载。
- 测试：新增 `UiMapLayerLockLogTests`（L01 锁定区域图层/L02 解锁/L03 系统层带（系统）/L04 详情含 LayerId 与状态变化/L05 同值 No-op 无日志/L06 一次点击一条日志 + C01 添加立方体单次创建）；`UiMapLayoutContractTests` 扩展 U01（无基础实体）/U02（立方体为添加直接子项）/U03（无偏好页签）/U04（无模式页签）+ 既有 U05/U06；`UiMapLayerPanelTests.Behavior` 锁定断言更新为"锁定图层：区域 1（区域）"与解锁断言。
- 验证：全解决方案 Rebuild 0 Warning / 0 Error；Core 339/339、World 645/645、WarCore 22/22 全 PASS；arch-a-guard PASS（依赖边界+5+100）；git diff --check PASS；无新增 NuGet；无 Schema 改动；Vulkan 生命周期零改动；NavGizmo CS8602 已在前轮（v0.2.24.32-fix）独立修复，本轮未触碰。
- 遗留：真机 F2-A01～A12 验收（菜单展开样式对比、偏好/模式消失、锁定日志、滚动布局、可见性/重命名回归）。
- 治理：版本 v0.2.24.32-fix → v0.2.24.33-fix（四处同步）；file-tree 重建（新增 UiMapLayerLockLogTests.cs）；未创建 Tag/Release。

## v0.2.24.32-fix
VK-WARN-NAVGIZMO-R1 导航 Gizmo 空引用警告消除，恢复全解决方案 0W0E（2026-08-05，Commit 本轮落库为准）
- 根因（只读调查确认）：`VulkanNativeHost.NavGizmo.cs` 第 19~22 行三元表达式 `vm.NavigationCamera is null ? null : NavigationGizmoHitTest.Hit(...)` 使 `hit` 推断为可空 `GizmoHitResult?`（`Hit` 本身返回非空类型），第 24 行 `hit.IsEndpoint` 触发 CS8602——相机在启动/重建/销毁期允许为 null（情况 B：生命周期期间可空），原代码该路径若可达即为真实 NRE。
- 修复（最小，仅 1 个生产文件，+2 行）：捕获相机局部快照 `var camera = vm.NavigationCamera;`，`camera is null → return false`（相机未就绪时无法计算 Gizmo 方向，不消费事件，继续走实体 Picking，与"区域外不捕获"语义一致）；`hit` 恢复非空类型；第 27 行条件简化为 `_navGizmoEndpoint is null`（guard 后相机非空已保证，行为等价）。未使用 `!`/pragma/NoWarn/`?.`，未改 Vulkan 生命周期、Gizmo 尺寸/颜色/命中半径，无新增日志。
- 存量警告合规修正（Rebuild 暴露，与本轮无关但 0W0E 出口必需，语义等价）：`ReferenceGridScaleTests` xUnit2000（Assert.Equal 参数交换 expected=2.4）、`SaveTransactionTests` xUnit2013（Assert.Equal(1, Count) → Assert.Single）。
- 验证：全解决方案 Rebuild 0 Warning / 0 Error（含 `-warnaserror` 与 `-p:WarningsAsErrors=CS8602` 双口径）；Core 339/339、World 636/636、WarCore 22/22 全 PASS；NavigationGizmo 聚焦 16/16；arch-a-guard PASS；git diff --check PASS；无新增 NuGet；无 Schema 改动。
- 遗留：真机 WN-A01～WN-A08 冒烟（Gizmo 显示/Hover/轴向/缩放/启动/关闭）。
- 治理：版本 v0.2.24.31-rz → v0.2.24.32-fix（四处同步）；file-tree 无结构变化仅校验不重建；未创建 Tag/Release。

## v0.2.24.31-rz
MAP-A-R2-D4-F1 图层 UI 归位：迁入右侧地图编辑器二级导航（2026-08-05，Commit 本轮落库为准）
- 撤回 D4 左侧"图层"页签（信息架构修正）：`MapLayer` 属于地图资产，不是项目资源/场景层级/全局功能，不应与"项目、层级"并列；左侧全局导航恢复仅"项目 | 层级"。
- `MapEditorPanel` 内部新增二级导航（地图 / 图层 / 环境）："地图"= 地图资产+地图属性（原内容），"图层"= 图层列表+图层属性，"环境"= 环境占位；每页独立 ScrollViewer 整页滚动（禁止内容穿透固定标题、禁止嵌套滚动）。
- `LayerPanel`（git mv Left/ → Right/）去掉内层 ScrollViewer，列表全量展示由页面滚动接管；`LayerInspectorPanel` 精简为"图层属性"（名称/类型/顺序/图层 ID/设为当前图层；可见/锁定开关保留在列表行内），与图层列表同页位于下方。
- 右侧全局"检查器"移除图层面板嵌入，`IsEmptySelection` 恢复为 `!HasSelection`（图层选中不再送到全局检查器）。
- 领域/命令/撤销重做/显隐渲染逻辑零改动（沿用 D4 实现）。
- 测试：新增 `UiMapLayoutContractTests` 4 项（左侧仅项目/层级且无图层页签、地图编辑器含地图/图层/环境三页、图层 UI 位于地图编辑器图层页、全局检查器无图层面板），源码合同模式防回归。
- 验证：Core 339/339、World 636/636 全 PASS；arch-a-guard PASS（依赖边界+5+100）；全解决方案 build 0 error；git diff --check PASS；无新增 NuGet；无 .xymap schema 改动。
- 遗留：真机 A5 复验（图层页签位置、滚动、穿透）。
- 治理：版本 v0.2.24.30-rz → v0.2.24.31-rz（四处同步）；未创建 Tag/Release。

## v0.2.24.30-rz
MAP-A-R2-D4 图层管理与可见性闭环（2026-08-05，Commit 本轮落库为准）
- 领域（World）：`MapLayerKind` 迁移为 Ground/Boundary/Region（值 0/1/2 不变：Base→Ground 同值、Custom→Boundary 同值，零持久化风险）；新增 `MapLayerRules`（名称校验 1~32 字符禁控制字符、系统层/最后区域层删除保护、区域层排序边界保护、自动命名"区域 N"按最小可用序号）与 `MapLayerStack`（纯函数顺序操作：区域层间交换 Order、系统层顺序固定、显隐/锁定/改名保身份）；`MapLayerValidator` 升级（Ground 恰 1 且 Order 0、Boundary 恰 1 且 Order 1、Region ≥1 且 Order ≥2）；`MapRegionValidator` 区域仅可挂载 Region 图层；默认地图 = 地面/边界/区域 1（区域 1 可见未锁定）。
- 编辑（Editor）：`MapEditSession` 新增六类图层内容命令（AddRegionLayer/RenameLayer/RemoveLayer/MoveLayerUp/Down/SetLayerVisibility/SetLayerLocked）全部走既有 CommitMapChange 管线（单历史节点、失败零污染、同值 No-op 无历史），MapEditReason 扩展 6 项；活动区域图层为会话临时状态（`ActiveRegionLayerId` + 事件，不 Dirty 不进历史），添加自动设为活动、删除自动转移相邻、内容变化自动规范化到有效区域层（H10）；撤销恢复相同 MapLayerId。
- 渲染：`MapRenderSnapshot` 增 ShowGround/ShowBoundary（渲染过滤，不删除领域数据）；投影从图层取系统层可见性；`RenderDrawPlan` 拆分 MapGround/MapBounds 两绘制项（主文件拆 partial 控 100 行），隐藏=跳过对应绘制项，网格/原点/轴/Gizmo 不受影响；显隐不进 `MapSurfaceResourceKey`（R06：显隐切换 NoRebuild 不重建 GPU 资源）；Vulkan Draw.cs 按 MapGround/MapBounds 分发。
- UI：左侧新增"图层"页签（LayerPanel：添加/上移/下移/删除 + 行内可见/锁定开关 + 系统标签 + 活动左标记，路径图标体系）；右侧检查器选中图层显示 LayerInspectorPanel（名称 Enter/失焦提交、类型/可见/锁定/顺序/ID 只读、设为当前图层）；命令路由 5 项（添加/上移/下移/删除图层/设为当前图层）；中文日志 9 类（添加图层：名称=… / 重命名图层：… → … / 图层可见性：…=隐藏 / 图层锁定：…=是 / 调整图层顺序：…，上移 / 设置当前图层：… / 删除图层：… / 图层删除失败：至少保留一个区域图层）。
- 测试：新增 MapLayerRulesTests/MapLayerStackTests(+Order)/MapLayerSessionTests(+Behavior)/UiMapLayerPanelTests(+Behavior)/MapSurfaceLayerVisibilityTests；更新 MapLayerTests(+Base)/MapRegionTests(+Strictness)/MapDefaultMapTests/MapEditSession* 等默认图层结构断言（区域层索引 2）。
- 验证：Core 339/339、World 632/632 全 PASS；arch-a-guard PASS（依赖边界+5+100）；全解决方案 build 0 error；git diff --check PASS；无新增 NuGet；无 .xymap schema 改动；Vulkan 生命周期零改动。
- 遗留：① 区域图层隐藏的消费（D5 绘制区域时读取 IsVisible）；② 锁定状态阻止编辑行为由 D5 接入；③ 图层保存/重新打开归 D6；④ 拖拽排序/混合模式/图层组明确不做。
- 治理：版本 v0.2.24.29-fix → v0.2.24.30-rz（四处同步）；未创建 Tag/Release。

## v0.2.24.29-fix
VK-PERF-R1 空闲渲染帧率与资源占用收敛（2026-08-04 21:13:49，Commit 本轮落库为准）
- 根因（只读调查 + 线程级采样证实）：`VulkanSwapchainCapabilities.ChoosePresentMode` 默认优先 `MailboxKhr`（无 vsync 上限），`VulkanPresentLoop.RunFrames` 有投影时全速 Acquire→Submit→Present 无帧率限制，`AcquireNextImage` 超时后 `continue` 立即重试形成忙循环——线程级采样显示单线程（Present）4 秒内消耗 3125ms（≈78% 单核当量），UI 线程接近 0；最小化窗口后占用不变（Swapchain 仍被消费）。
- 修复（第一层，最小修改）：`ChoosePresentMode` 改为 **FIFO（垂直同步）首选**（遍历找 `FifoKhr`，Vulkan 规范保证必被支持；保留原安全回退），Mailbox 不再作为默认；不新增 UI 开关，不改 Swapchain 自愈流程，不清除 `_hasRenderProjection` 语义，无逐帧日志（启动日志保留创建/重建时一次「呈现模式=…」）。
- 实测数据（本机 i5-10400F + RTX 3060 12GB + 144Hz 显示器；GPU 3D 用 Windows 计数器 `\GPU Engine(*)\Utilization Percentage`（任务管理器同源），CPU 为进程 CPU 时间差单核当量，任务管理器进程页约等于该值 ÷12）：基线（Mailbox）默认窗口静止 GPU 3D 19.4~21.3% / CPU 73~81%，最小化 GPU 17~21% / CPU 70~78%；FIFO 后默认窗口静止 GPU 3D 稳态 7~8%（峰值 19.4% 为启动过渡）/ CPU 稳态 10~11%（排除首样本初始化误差），最小化 GPU 7.6~8.1% / CPU 17~24%。按计划判定表「GPU 已持续 ≤40% → 停止扩展」，FIFO 即最终方案，**未启用第二层 60 FPS 节流**。注：基线 GPU 数值与 F4 轮记录的 91~96%（任务管理器口径）存在口径/采样时机差异，本轮以修改前后同口径对比为准。
- 测试：`VulkanPresentModeSelectionTests` 4 项（FIFO 优先/乱序优先/Mailbox 非默认/选择确定性）、`VulkanPresentLoopContractTests` 5 项（无投影受控等待/无逐帧日志/投影语义不变/模式日志只在创建重建/无新依赖），World 574→583（+9）。
- 验证：Core 334/334、World 583/583、WarCore 22/22 全 PASS；arch-a-guard PASS（依赖边界+5+100）；全解决方案 build 0 error；git diff --check PASS；地图/Shader/相机/Gizmo/输入/Swapchain 自愈/Avalonia UI 零改动；无新增 NuGet；临时采样脚本（Temp 目录 hermes-verify-*）不入库。
- 遗留：① 最小化后 GPU/CPU 未降至近零（FIFO 后仍 ~8%/~20%）→ 后续窗口可见性/遮挡暂停轮（P1-A7）；② 显示器 144Hz 时 FIFO 提交率=144fps，CPU 单核当量 ~10%，如需更低可启用 60 FPS deadline 节流（预留方向，本轮未启用）；③ Resize 多代际重建为既有问题，不属本轮。
- 治理：版本 v0.2.24.28-fix → v0.2.24.29-fix（四处同步）；未创建 Tag/Release。

## v0.2.24.28-fix
MAP-A-R2-D3-F4 日志面板垂直尺寸自适应修复（2026-08-04 15:50:00，Commit 本轮落库为准）
- 根因（A4 真机裁定）：日志区被裁切不是滚动问题而是**外部布局边界**——`UiRoot.axaml` Row3 日志区 `Auto+MaxHeight=420`（Auto 行优先按内容期望满额 420）与 Row1 主工作区 `*+MinHeight=320` 的最小和，加上工具栏与分隔条后超过矮窗口可用高度（约 1400×820 窗口可用仅 ~369 < 420）→ 日志区被窗口底部裁切，ScrollIntoView 只能控制内部滚动位置救不了外部边界；约 1032 高窗口可容纳（与截图矩阵 820 失败/1032 正常/最大化正常完全吻合）。
- 修复（不再堆滚动算法）：`UiRoot.axaml.cs` 新增 `ClampLogRow()`——监听 `IsLogOpen`（DataContext PropertyChanged）与窗口 `SizeChanged`，日志展开时把 Row3 设为像素行 `Math.Clamp(420, 120, 可用高度)`（可用 = 窗口高度 − 根 Margin 24 − 分隔条 6 − 主区最小 320 − 工具栏实际高），折叠时回 `GridLength.Auto`（只占标题栏）；极端矮窗口可用 ≤0 时保持现状由 `MinHeight=32` 兜底。`Foot.axaml` 日志展开 Border `MinHeight=180 → 0`（解除矮窗口下阻止列表 Viewport 缩小的最低高度，改由外层像素行约束）。
- 测试：`UiRootLogRowContractTests` 5 项（Row3 MaxHeight=420 存在/主区 MinHeight=320 存在/代码含 GridLength.Auto+Math.Clamp+IsLogOpen 自适应/日志 Border 不再 MinHeight=180/可优雅缩小 MinHeight=0）；几何级验证由 F4-A1~A8 真机复验承担（合同测试已注明）。
- 验证：Core 334/334、World 574/574（+5）、WarCore 22/22 全 PASS；arch-a-guard PASS（依赖边界+5+100）；全解决方案 build 0 error；git diff --check PASS；5+100 手写复核（UiRoot.axaml.cs 81 / Foot.axaml 99 / 合同测试 52 行全合规）；无地图代码/中文文案/滚动策略主体改动。
- 性能诊断（只读采样，独立轮处理，不进本轮提交）：PresentMode 优先 `MailboxKhr`（无 vsync 上限）；主循环 `RunFrames` 有投影时全速 Acquire→Submit→Present 无帧率限制（仅无投影时 `Thread.Sleep(16)`）；`_hasRenderProjection` 一旦为 true 不消费清除 → 空闲场景 GPU 91%~96% 根因与用户第一嫌疑吻合；建议独立性能轮：PresentMode 改 FIFO 或主循环节流 60 FPS，再测空闲/最小化/遮挡/网格开关采样矩阵。
- 治理：版本 v0.2.24.27-fix → v0.2.24.28-fix（四处同步）；未创建 Tag/Release。

## v0.2.24.27-fix
MAP-A-R2-D3-F3 日志面板尾项完整显示修复（2026-08-04 15:10:00，Commit 本轮落库为准）
- 真实尾部安全区：`Foot.axaml` 日志列表 ItemsPanel 改为 `VirtualizingStackPanel Margin="0,0,0,12"`（12 DIP 进入滚动 Extent——Avalonia MeasureCore 将 Margin 计入 DesiredSize，ScrollContentPresenter.ComputeExtent 基于内容 Bounds 计算），移除仅承担视觉间距、不进滚动范围的 ListBox `Padding="0,0,0,8"`；虚拟化保持（ItemsPanelTemplate 仍是 VirtualizingStackPanel，未退化为 StackPanel）。
- 两阶段尾项定位：`LogListAutoScrollController` 重写并拆 partial（主文件 84 行 + Follow.cs 61 行 + Layout.cs 27 行）——唯一入口 `RequestLatestItemVisibility`（新日志/分类切换/清空/布局变化统一经过）；`_requestVersion` 请求合并（高频日志不堆积 Dispatcher 任务，旧请求执行时版本不一致即退出）；第一阶段 `ScrollIntoView(最后一项)`（Render 优先级）；第二阶段 `ContainerFromItem/ContainerFromIndex → BringIntoView` + 读取最终 `Extent/Viewport` 修正 `Offset`（Background 优先级，`_tailCorrectionScheduled` 保证每请求最多一次，无递归无定时器）；修正保留水平偏移（`new Vector(Offset.X, maximumY)`），`_programmaticCorrection` 防止程序滚动被误判为用户滚动。
- 阅读状态保持（计划 8.1 关键修正）：`_atTail` 只由用户滚动（OffsetDelta≠0）维护——新日志增大 Extent 时不得用新最大滚动值重算（否则底部被误判为已离开、跟随失效）；ScrollChanged 集中处理 Extent/Viewport 变化（Resize/展开折叠/水平滚动条出现/DPI 重测），仅跟随态安排合并修正；清空日志取消旧请求并恢复跟随。
- 测试：`FootAxamlTailContractTests` 3 项（AXAML 合同：虚拟化 ItemsPanel 保持/12 DIP 尾距/旧 Padding 移除）、`LogListAutoScrollControllerContractTests` 9 项（控制器合同：最后一项为滚动目标/两阶段 Render+Background/读取最终滚动范围/第二阶段至多一次/保留水平偏移/请求合并失效/程序化修正保护/无递归定时器/无 EditorLogBus 引用）、`LogAutoScrollPolicyTests` 新增 1 项（阈值外 20.1 DIP 不跟随）；仓库无 Avalonia Headless 基础设施，几何级验证由 A4 真机承担（合同测试已注明）。
- 验证：Core 334/334、World 569/569（+13）、WarCore 22/22 全 PASS；arch-a-guard PASS（依赖边界+5+100）；全解决方案 build 0 error；git diff --check PASS；无 Debug.WriteLine/EditorLogBus/地图代码/中文文案/新增依赖；5+100 手写复核 84/61/27/99/33/79/38 行全合规。
- 治理：版本 v0.2.24.26-rz → v0.2.24.27-fix（四处同步）；未创建 Tag/Release。

## v0.2.24.26-rz
REPO-GOV-R1 目录职责分类与命名去里程碑化（2026-08-04 14:20:00，Commit 本轮落库为准）
- 生产目录按职责拆分子目录（宪法 5+100 的 5 规则落地映射）：`Vm/` → Camera/Map/Scene/Selection/Transform{Move,Rotate,Scale}/Logging/Inspector/History/Tree（根只留 UiVm.cs）；`Render.Vulkan/Render/` → ClearFrame/Grid/Map/Scene/StaticModels/Present；`Editor/Assets/` → Import/Gltf、Hosting{Planning,Transactions}、StaticModels、Catalog、Identity；`Core/Gizmo/` → Common/Move/Rotate/Scale；移动不改 namespace（SDK-style csproj 自动包含，零代码改动）。
- 测试镜像生产代码：`World.Tests/` 按领域目录（Map{Editing}/Scene/Spatial/Camera/Selection/Transform{Move,Rotate,Scale}/WorldPartition/Assets/Logging/Tree），`Core.Tests/Render/` 按 Map/Grid/NavigationGizmo/StaticModels/DrawPlan/Camera；命名去里程碑化（`WorldCR4D3F1ValidatorTests` → `StaticModelValidatorTests`，含 partial 切片后缀 .R4R2 → 职责后缀 .DragState/.ToolSwitch/.Preview/.AxisUniform，测试方法名 `R5R1_*` 一并清理）。
- docs 只保留当前事实源（184→16 文件）：删除 closed/**、历史验收/审计/计划/报告、全部里程碑 SVG、superseded/**；仍有效结论并入 `ENGINE_ARCHITECTURE.md`（世界事实/坐标合同/编辑器边界）；`map-contract.md` 就位；`AGENTS.md` 仓库入口文件入库（索引+红线摘要，唯一权威仍为宪法）。
- file-tree.md 从 git ls-files 全量重建，每个 tracked 文件一句话职责（730/730 全覆盖），无版本号/阶段号/职责索引。
- 验证：Core 334/334、World 556/556、WarCore 22/22 全 PASS；arch-a-guard PASS（依赖边界+5+100）；--no-incremental 全量 0 error；git diff --check PASS；ad-hoc 16 项断言（file-tree↔ls-files 全路径一致/无里程碑前缀残留/目录根瘦身/docs 16 事实源/禁用词/路径合同）ALL PASS；代码行为零改动。
- 治理：版本 v0.2.24.25-fix → v0.2.24.26-rz（四处同步）；未创建 Tag/Release。

## v0.2.24.25-fix
MAP-A-R2-D3-F2 日志中文化与日志面板尾部显示修复（2026-08-04 13:27:18，Commit 本轮落库为准）
- 日志全部中文化（用户可见键名与状态值）：命令/宽度输入/深度输入/基础高度输入/地图标识/原尺寸/原基础高度/候选尺寸/候选基础高度/历史状态/变更序号/新尺寸/新基础高度/可撤销/可重做/原因/序号/尺寸/基础高度/地表/错误类型/错误说明/当前尺寸/状态保持不变/处理/资源键已变化/地面顶点/索引/边界顶点/接收序号/已消费序号；内部枚举与错误码保持英文，显示映射集中在 `UiVm.MapDiagnostics.Format.cs`（FormatMapEditReason/FormatSurfaceKind/FormatErrorCode/FormatBoolean）与 `Render.Abstractions/MapSurfaceResourceUpdateText.cs`（三态决策中文，Vulkan 层无 UI 依赖引用），未反写任何领域类型。
- 日志面板尾部显示修复：`LogListAutoScrollController` 重写为尾部跟随规则（纯策略 `LogAutoScrollPolicy`：距底 ≤20 DIP 视为底部附近）——位于底部时新日志自动跟随、用户向上阅读旧日志不强制拉回、滚到底恢复跟随、切换日志分类（ForceFollow）定位最新、清空日志滚动范围归零自动回跟随态；列表底部加 8 DIP 安全间距（ListBox Padding），最后一行不再被底边裁切。
- 测试：`UiMapLogChineseTests` 5 项（成功/失败/撤销日志中文键与状态值断言 + 无英文键断言 + 显示映射全覆盖）、`LogAutoScrollPolicyTests` 4 项（底部跟随/阈值内跟随/远离不跟随/无滚动范围恒跟随）。
- 验证：Core 334/334、World 556/556（+9）、WarCore 22/22 全 PASS；arch-a-guard PASS（依赖边界+5+100）；--no-incremental 全量重编译 0 error（首次因真机编辑器进程 dll 锁报 MSB3021，进程退出后重跑 0 error；1 个既有 warning 如实记录）；git diff --check PASS；Shader/地图/历史/渲染/相机/Vulkan 生命周期零改动。
- 治理：版本 v0.2.24.24-fix → v0.2.24.25-fix（四处同步；file-tree 按新治理不含版本号，仅插入新增文件行）；未创建 Tag/Release。
- 状态：**MAP-A-R2-D3-F2：已修复并落库**；MAP-A-R2-D3-A3：NOT RUN；MAP-A-R2-D3：IN PROGRESS。

## v0.2.24.24-fix
MAP-A-R2-D3-F1 地图面板真实命令路由与验收日志修复（2026-08-04 11:59:52，Commit 本轮落库为准）
- 修复地图面板 RunCommand 未路由到地图编辑命令的问题：新增 `UiVm.MapCommandRouting`（TryRouteMapCommand 在通用兜底之前匹配新建/聚焦/应用属性/撤销/重做），面板按钮 → RunCommand → 地图命令 → MapSession 全链打通；`UiWin.RunMapCommand` 精简为仅快捷键可达的新建/聚焦（打开/保存/卸载分支移除）；修复「聚焦地图」未发布相机快照（FrameMapCamera 补 PublishSceneRenderSnapshot，与 FrameSelectedCamera 同模式）。
- 增加低频验收日志（复用既有日志总线，不建第二套 Logger）：地图命令收到、属性提交开始/成功/失败（含 Code/StateId/ChangeSequence/StateUnchanged）、撤销/重做成功/失败、渲染快照已发布（Reason/Sequence/Size/Surface）、Vulkan 资源更新决策（Recreate/NoRebuild/RejectStale 三态）、资源重建完成（顶点数/尺寸/BaseHeight/Sequence）；每帧/Hover/Getter 不记录。
- 增加真实入口自动测试：`UiMapCommandRoutingTests` 8 项——从 `RunCommand.Execute("应用地图属性"/"撤销地图修改"/"重做地图修改")` 出发验证会话/快照/输入框/历史状态；非法尺寸与 NaN/Infinity/-Infinity 零污染；命令路由合同（新建/聚焦/未知命令兜底）；日志链含命令收到/提交开始/提交成功/快照发布。
- 验证：Core 334/334、World 547/547（+8）、WarCore 22/22 全 PASS；arch-a-guard PASS（依赖边界+5+100）；--no-incremental 全量重编译 0 error（1 个既有 warning 如实记录）；git diff --check PASS；Shader 未修改。
- 治理：版本 v0.2.24.23-rz → v0.2.24.24-fix（四处同步；file-tree 按新治理不含版本号，仅插入新增文件行）；未创建 Tag/Release。
- 状态：**MAP-A-R2-D3-F1：已修复并落库**；MAP-A-R2-D3-A2：NOT RUN；MAP-A-R2-D3：IN PROGRESS。

## v0.2.24.23-rz
MAP-A-R2-D3 A1 入口补接（2026-08-04 11:20:14，Commit 本轮落库为准）
- 地图面板新增「撤销地图修改 / 重做地图修改」按钮：分别调用 `MapSession.Undo/Redo`（地图独立历史实例，不触碰场景实体历史；全局 Ctrl+Z 的焦点上下文规则未设计，D3 用显式按钮最安全）；`IsEnabled` 绑定 `CanUndo/CanRedo`（由 `HistoryAvailabilityChanged` 驱动刷新）；成功后同步宽/深/高文本与状态文字，渲染快照由 `ContentChanged` 自动更新（无历史时防御性报错，按钮禁用态下不可达）。
- 测试：新增 `UiMapHistoryTests` 4 项（撤销/重做恢复三字段 + 文本同步 + 按钮可用性翻转 + World 查询随会话恢复 + 快照经事件驱动更新）；Rename 不重建由自动测试负责（ResourceKey/策略），不进真机清单。
- 治理登记（非阻断，待用户正式治理修订落库）：`file-tree.md` 退出版本号同步源，版本同步由五处调整为**四处**（changelog/run.bat/UiWin.axaml/UiVm.SceneDocument.cs）。
- 已知限制登记：**GentleHillsV1 地表视觉渲染尚未支持**（World 高度查询为起伏、画面为 Flat 平面）——D3 真机只验收 Flat；非 Flat 地形视觉渲染归后续地形轮，不得静默宣称显示正确。
- 验证：Core 334/334、World 539/539（+4）、WarCore 22/22 全 PASS；arch-a-guard PASS；--no-incremental 全量重编译 0 error（1 个既有 warning 如实记录）；git diff --check PASS。
- 治理：版本 v0.2.24.22-rz → v0.2.24.23-rz（四处同步；file-tree 按新治理不含版本号）；未创建 Tag/Release。
- 状态：**MAP-A-R2-D3：等待真机验收**；MAP-A-R2-D3-A1：NOT RUN。

## v0.2.24.22-rz
MAP-A-R2-D3 A1 前收口（2026-08-04 10:51:34，Commit 本轮落库为准）
- 修正地图 GPU 资源判等：新增 `MapSurfaceResourceKey`（MapId/尺寸/BaseHeight/地表参数/可见性，**不含 ChangeSequence**）+ `MapSurfaceResourceUpdatePolicy` 纯策略（旧序号拒绝/同键不重建/异键重建）；Vulkan `SetMapSurface` 改为策略驱动（`_lastConsumedMapSequence` 与资源键分离），Rename 等非几何变化不再重建地面与边界缓冲。
- 地图属性改为单次原子提交：`MapEditSession.UpdateMapProperties`（一次 CommitMapChange = 单历史节点/单次 ChangeSequence/单次 ContentChanged），失败整体拒绝零污染（NaN/Infinity/尺寸越界/区域冲突）；UI「应用修改」只调用组合命令（删除 ResizeMap+SetBaseHeight 连续调用），单字段命令保留供未来 Inspector/自动化 API。
- 默认首帧地图快照验证：新增 `UiMapInitialProjectionTests`——构造后首个 RenderProjection 即携带 10 km×10 km Flat 默认地图快照，无需新建地图。
- 重建 `file-tree.md`：从 `git ls-files` 全量生成当前树（882 个跟踪文件全覆盖、零缺失、零重复），删除全部版本化「职责索引」与迁移记录；历史仅保留于 changelog。
- 验证：Core 334/334（+13 资源键/策略）、World 535/535（+11 原子提交/首帧投影/错误消息同步）、WarCore 22/22 全 PASS；arch-a-guard PASS（依赖边界+5+100）；--no-incremental 全量重编译 0 error（1 个既有 warning 如实记录）；git diff --check PASS；Shader 本轮未修改，字节码 --verify 复验一致未污染。
- 治理：版本 v0.2.24.21-rz → v0.2.24.22-rz（五处同步）；未创建 Tag/Release。
- 状态：**MAP-A-R2-D3：等待真机验收**；MAP-A-R2-D3-A1：NOT RUN。

## v0.2.24.21-rz
MAP-A-R2-D3 有限地图地面、边界与渲染快照（2026-08-04 10:24:50，Commit 本轮落库为准）
- **渲染唯一输入**：`MapRenderSnapshot` 迁至 `XuanYu.Render.Abstractions`（MapId/尺寸/地表/BaseHeight/Seed/SourceChangeSequence/IsVisible + Min/Max；**无 Name**——Rename 不引发 GPU 资源重建）；由 `MapRenderSnapshotProjection`（Editor.UI）从 `MapEditSession.CurrentMap` 投影，首次组装生成初始快照，仅响应 `ContentChanged` 低频事件（相机/Hover/选择不重建）；ChangeSequence 单调去重，禁在 Render 自增。
- **有限地面常量几何**：`MapSurfaceGeometryBuilder`（Render.Abstractions）Flat 地面固定 **4 顶点 6 索引**（左下→右下→右上→左上，Z=BaseHeight；10 km/20 km/百万米均为 4 顶点，尺寸只进顶点坐标）；删除按米细分的 `MapTerrainMeshBuilder`（4225 顶点，C 类退役）与 `MapBoundsMeshBuilder`（48 顶点）；新 `MapBoundsGeometryBuilder` 四条边细条四边形 **24 顶点**（世界宽度 clamp(尺寸×0.001, 1, 50) 米 + 渲染抬升 0.05，真机验证远近后决定是否屏幕恒宽 Pass）。
- **Vulkan 绘制**：`VulkanClearFrameOwner.MapTerrain.cs` → `MapSurface.cs`（`SetMapSurface`，值相等去重；地面索引 draw kind -14 + 边界 -15 分支复用）；`scene.vert` 地表基色土绿 → **低饱和豆青灰 (0.52,0.60,0.55)**、边界亮琥珀 → **淡金褐 (0.85,0.76,0.55)**（glslc 重生成 ShaderBytecode.Vert.cs 7430 词/GridFrag.cs 1315 词，--verify 逐字一致）。
- **网格对齐地图**（计划 9.1）：参考网格 Pass push 176B → **192B**（新增 vec4 mapBounds：半宽/半深/BaseHeight/边缘淡出宽度=min(尺寸)×0.08）；`editor_reference_grid.frag` 求交平面 Z=0 → **Z=BaseHeight** + 地图矩形外平滑淡出（无地图 w=0 保持无限网格）；ViewPlaneGrid 共用 FillGridPushConstants 且自行覆盖 [44..47]，F3-F4 正交视图不受影响。
- **权威统一**：UiVm 渲染数据源 `MapWorld.BuildRenderSnapshot()`（R1 旧链）→ `_mapRenderSnapshot`（会话直出）；`MapDocumentWorldBridge` 退役，新增 `WorldMapState.From(MapDefinition)`（World 同层投影，环境默认 ClearDay）；`MapDocumentAggregateBridge`（v1 DTO → 聚合，场景 mapReference 保活链：加载→投影→`ReplaceCurrentMap(markSaved:true)`，D2 预留入口）；**保存/打开按钮禁用**（v1 DTO 双权威分叉风险，持久化 D6 接入）；"卸载地图"按钮/命令移除（D2 会话语义=恒有默认地图）；场景引用失效时显示错误 + 会话默认地图保持（非 R1"未加载"空状态）。
- **真机入口**（计划目标 3）：地图编辑器面板"基础地表"区升级为**地图属性**区——宽度/深度/基础高度编辑框 + 应用修改（全解析合法后 `ResizeMap`+`SetBaseHeight` 提交，非法输入中文错误/不产生历史/不部分更新）+ 地表类型 Flat 只读；聚焦地图复用 `FrameMapAllWithCenter`（**已含动态 Far**=max(100, distance+depth×4)，Near 0.05，70% 占用率，正交版 F3-F4 兼容），角点 Z 取 BaseHeight。
- 验证：新增 MapSurfaceGeometryTests（9 项：4/6 常量、坐标对称、BaseHeight、边界 24 顶点/宽度公式/抬升）+ MapRenderSnapshotProjectionTests（5 项：默认/Resize/地表/会话驱动/Rename 不重建）+ UiMapEditorTests 重写 7 项（会话默认地图/应用/非法/非数字/取景 Far）+ SceneMapReferenceTests 适配 4 项 + MapDocumentAggregateBridgeTests 5 项；Core 321/321（312→321）、World 524/524（528→524，删 2 文件+旧 4 用例）、WarCore 22/22 全 PASS；arch-a-guard PASS（含依赖边界+5+100）；--no-incremental 全量重编译 0 error（1 个既有 warning 如实记录）；git diff --check PASS。
- 治理：版本 v0.2.24.20-rz → v0.2.24.21-rz（五处同步）；登记：百万米地图远距深度精度（~17 m@85 km）为后续大世界问题；边界屏幕恒宽待真机验证后决定；未创建 Tag/Release。
- 状态：**MAP-A-R2-D3：等待真机验收**（自动测试通过 ≠ COMPLETE）。

## v0.2.24.20-rz
MAP-A-R2-D2 地图编辑会话与状态权威（2026-08-03，Commit 本轮落库为准）
- **D1 遗留小修**：`MapDefinition` 移除 `Revision`（领域聚合纯净不可变，版本/游标语义由编辑会话持有）；`MapLayerKind`/`MapRegionKind` 枚举值合法性检查（`Enum.IsDefined`，未知角色不得默认为可承载层）+ UnknownLayerKindRejected/UnknownRegionKindRejected 测试。
- **历史方案（审计裁定：方案 A 直接复用）**：现有 `EditorHistoryOwner` 已通用（PushEntry(object)/TryUndoAny/TryRedoAny/CurrentRevision Undo 回退/新编辑清 Redo 分支），地图直接复用同一 Core 实现（独立实例），不建第二套历史系统；CurrentStateId=历史游标（可回退旧节点）、ChangeSequence=单调递增（事件/去重，不可回退）、SavedStateId=保存点；IsDirty=路径空 ∥ 保存点空 ∥ 状态不一致（随 Undo/Redo 回到保存点）。
- **MapEditSession（XuanYu.Editor/MapEditing/，11 文件）**：唯一地图权威 CurrentMap: MapDefinition；统一 `CommitMapChange` 管线（纯修改→No-op 检测→领域校验→记录历史→替换，失败零污染）；命令 RenameMap/ResizeMap/SetBaseHeight/CreateNewMap/ReplaceCurrentMap/MarkSaved；Undo/Redo/分支清除；选择（None/Map/Layer/Region 只存稳定 ID + 变更后规范化）；低频事件（ContentChanged/SelectionChanged/DirtyChanged/HistoryAvailabilityChanged）；写线程保护（注入 `Func<bool>`，复用现有判断器）。
- **错误合同**：复用 Core.EngineResult/EngineError（未新建 MapEditResult 等容器）；错误码 NotOnWriteThread/InvalidMapName/InvalidMapSize/RegionWouldBeOutOfBounds（缩小致区域越界整体拒绝，不裁剪不移动）/NoUndoAvailable/NoRedoAvailable/UnknownLayer/UnknownRegion；No-op=成功且无状态变化。
- **组装**：UiVm 增加 `MapSession`（同一写线程判断器注入，headless 测试兼容）；无 Vulkan/UI 视觉改动。
- 验证：新增 MapEditSession 测试 7 文件 36 用例（创建/命令/历史/Dirty/选择/验证/线程）；Core 312/312、World 528/528（490→528）、WarCore 22/22 全 PASS；arch-a-guard PASS；5+100 全合规（守卫 ReadAllLines 口径：UiVm 压至 99、SelectionTests 100）；--no-incremental 全量重编译 0 error（3 个既有 warning 如实记录）。
- 治理：版本 v0.2.24.19-rz → v0.2.24.20-rz（五处同步）；未创建 Tag/Release。

## v0.2.24.19-rz
MAP-A-R2-D1-F1 架构与领域合同修正（REVISE 裁定）（2026-08-03，Commit 本轮落库为准）
- **审查裁定**：D1 三个阻断问题（领域权威层错误/无完整地图聚合/区域合法性过弱）全部修复；版本后缀修正为 -rz（正常开发轮，F 修复轮才用 -fix）。
- **F1-1 架构边界恢复**：图层/区域/边界/验证/聚合全部迁至 `XuanYu.World/Map/`（地图权威层，World 仅依赖 Core）；Editor 删除 12 个迁移文件，`MapDocument` 回归纯 DTO（.xymap v1 持久化模型）；`MapId` 同步迁移（`SceneDocument/MapReference.cs` 与 `SceneDocumentValidator.MapReference.cs` 引用修复）；Editor 仅保留 DTO/JSON/存储/校验与桥接。
- **F1-2 完整地图聚合**：新增 `MapDefinition`（MapId/DisplayName/尺寸/坐标系统/地表/图层/区域/Revision）为唯一权威根；`MapDefaultDefinition.CreateDefault()` 一次创建完整地图（10 km × 10 km Flat + 基础地图层 + 区域层 + 空区域）；D2 起 CurrentMap/Undo/Dirty 围绕单一聚合；持久化 schema v2 仍属 D6。
- **F1-3 领域合法性收紧**：`MapRegion` 移除 `IsClosed`（正式区域天然闭合，顶点不重复保存首尾）+ 新增 `MapRegionDraft`（绘制中草稿，CanClose/Close 提交）；`MapLayerKind`（Base/Region/Custom）稳定角色标识替代中文名；新增检查：ID 合法性（MapId/LayerId/RegionId）、图层 Order 唯一、基础层必须且仅有一个且位于第 0 位、区域不得挂载 Base 层、相邻重复点（含首尾）、至少三个不同顶点、非零面积（鞋带公式，共线三点拒绝）；自交检测明确归 D5（绘制轮），不在 F1 默默放行。
- **修正内部缺陷**：`MapDefinitionValidator` 的 `??` 短路 bug（MapValidationResult 为引用类型，Ok() 非 null 导致区域验证永不执行）→ 显式 if 链；`MapLayerValidator` 基础层先查唯一性再查顺序。
- 验证：新增 MapDefinitionTests/MapRegionDraftTests/MapLayerTests.Base + 区域严格性用例；Core 312/312、World 490/490（470→490）、WarCore 22/22 全 PASS；arch-a-guard PASS；5+100 全合规（MapDocumentValidator 压缩至 99 行、MapRegionTests 拆 Strictness 分部）；--no-incremental 全量重编译 0 error（3 个既有 warning 如实记录）。
- 治理：版本 v0.2.24.18-fix → v0.2.24.19-rz（五处同步；后缀修正为正常开发轮）；未创建 Tag/Release。

## v0.2.24.18-fix
MAP-A-R2-D1 地图领域合同：图层/区域模型与验证（2026-08-03，Commit 本轮落库为准）
- **R2 范围裁定**：R1 后半（地图尺寸/边界/地表/区域/图层/保存）按用户纠正转入 MAP-A-R2；`.xymap` ZIP 封装与 DGD 衔接整体后移，不抢在地图本体之前开发。
- **稳定 ID**：新增 `MapLayerId`/`MapRegionId`（32 位十六进制，与 MapId 同族）；名称可改、ID 不变，不依赖列表序号/UI 索引。
- **图层领域模型**：`MapLayer`（ID/名称/顺序/可见/锁定/固定层）+ `MapDefaultLayers` 默认工厂（"基础地图"固定层 + "区域"层）。
- **区域领域模型**：`MapRegion`（ID/所属图层/类型/顶点/闭合）+ `MapRegionKind`（Generic/Playable/Restricted/Deployment/Objective）；顶点只存水平面 X/Y（沿用已冻结 Z-Up 合同，高度由地表采样取得）。
- **边界合同**：`MapBounds` 中心原点闭区间（X/Y ∈ [-W/2, W/2]），与 `WorldMapState.Contains` 语义一致。
- **验证器**：`MapLayerValidator`（ID 唯一/名称非空/顺序非负/固定层至多一个）、`MapRegionValidator`（闭合/≥3 顶点/≤1024 顶点/引用图层存在/有限数值/边界内），结构化结果不抛来源不明异常。
- **默认工厂**：`MapDocument.CreateDefault()`（"未命名地图" 10000×10000 Flat）；`CreateNew` 默认值按 R2 合同调整（10000×10000、`DefaultFlat`）；最大尺寸 10000 → 1000000（R2 测试 02 需 20000，上限仅为输入保护）。
- 分层裁定：Layers/Regions 本轮为独立领域模型，不挂载 MapDocument（.xymap v1 强制 layerReferences 空数组，schema v2 升级属 D6）；无 Editor.UI/Vulkan 依赖。
- 验证：新增 MapBoundsTests/MapLayerTests/MapRegionTests(+Helpers)/MapDefaultMapTests 4 文件 35 用例；Core 312/312、World 470/470（435→470）、WarCore 22/22 全 PASS；arch-a-guard PASS；5+100 全合规；`--no-incremental` 全量重编译 0 error（3 个既有 warning 如实记录：xUnit2000/xUnit2013/CS8602）。
- 治理：版本 v0.2.24.17-fix → v0.2.24.18-fix（五处同步）；F3-F4 视觉冒烟仍待用户真机验收；未创建 Tag/Release。

## v0.2.24.17-fix
MAP-A-R1-D5-R1-F3-F4 正交投影 + 视图平面网格（2026-08-03，Commit 本轮落库为准）
- **F3-F3 正式 PASS/CLOSED**：用户补充真机验收事实（此前未同步），本轮不重验、不补跑。
- **正交投影链路**：`CameraState` 新增 `ProjectionMode`（Perspective/Orthographic）+ `OrthographicScale`（>0 校验）；`ViewProjectionState.Create` 正交矩阵分支（`CreateOrthographic`，与透视同族深度 [0,1]）；`RenderCameraProjection` 透传模式；世界射线（`WorldRayFactory` 逆 VP 反投影）正交下自动成立。
- **标准视图正交化（用户冻结语义）**：六方向标准视图（±X/±Y/±Z）自动进入正交投影（正交尺度=当前透视可见高度，切换视觉连续）；自由环绕（Orbit）从正交视图开始自动恢复透视并退出标准视图；正交 Dolly=缩放 OrthographicScale（禁距离模拟缩放）；正交 Pan 保持正交（每像素世界距离=尺度/视口高）；正交取景（FrameAll/Selected 保持正交，尺度按包围范围适配）。
- **视图平面网格（用户冻结语义）**：±X→YZ 平面、±Y→XZ 平面（世界原点基准、自适应间距/LOD、屏幕恒定线宽、深度偏移、距离淡出）；±Z 复用现有地面网格（Z=0 即 XY 平面）；独立 Pass（复用 GridVert + 新 `editor_view_plane_grid.frag` + 192B PushConstant 含平面法线；glslc 字节码生成前完成工具链一致性复验 MATCH）；`RenderDrawKind.EditorViewPlaneGrid` + DrawPlan 条目（启用时替代地面网格）。
- **正交配套**：Gizmo 屏幕尺寸正交分支（Move/Rotate/Scale：worldHeight=OrthographicScale 恒定）；网格 LOD 采样正交化（wmpp=尺度/视口高解析式，规避侧视正交射线求交退化）。
- 验证：新增 CameraOrthographicTests 7 + CameraOrthographicNavigationTests 5 + ViewPlaneGridFor 映射 7 + DrawPlan 2；Core 312/312、World 435/435、WarCore 22/22 全 PASS；arch-a-guard PASS；5+100 全合规（Draw.cs 拆 PipelineBind 分部、RenderDrawPlan 枚举压缩）；glslc 工具链一致性复验 MATCH。
- 视觉冒烟：**未执行**（本环境无法操作画面，留真机验收）；请用户重点复验：六方向正交视图、正交滚轮缩放（尺度）、Orbit 恢复透视、±X/±Y 视图平面网格、±Z 地面网格。
- 治理：版本 v0.2.24.16-fix → v0.2.24.17-fix（五处同步）；F3-F3 正式 CLOSED；未创建 Tag/Release。

## v0.2.24.16-fix
MAP-A-R1-D5-R1-F3-F3 Blender 风格导航视图收尾（2026-08-03，Commit 本轮落库为准）
- F3-F2 真机复验：相机崩溃修复基本通过；**普通 Orbit 地平线滚转 FAIL + 导航 Gizmo 视觉 FAIL + 侧视表现体验 FAIL**（数学正确，但缺正交视图/视图平面网格）。
- 根因（本轮源码确认）：F3-F2 的 TryOrbit 以 start.Up 为 PreferredUp——顶视（Up=+Y）后 Orbit 继承 +Y，画面整体转 90°（Roll）；Gizmo 为 88 DIP 调试图形（六端点全绘无正对处理、轴线穿过中心球、标签层级弱）。
- 修复（本轮目标 1/2；目标 3 正交投影审计为无 → 拆 F3-F4）：
  - **无 Roll Orbit**：TryOrbit 改用世界 +Z 重建基（Right=Forward×WorldUp、Up=Right×Forward），顶/底视平行时 CameraBasis 自动回退最不平行世界轴（+Y/+X），Up 永不下翻、连续环绕不累积倾斜；Dolly/Pan 保留 start.Up 语义；删除无调用点的 Result 死代码；
  - **Gizmo 视觉重做（Blender 结构）**：控件 88→96 DIP、边距 12→14、轴投影 25→27、负端点 5.5→5；七层绘制（后轴→后端点→中心球→前轴→前端点→标签→Hover 环）；轴线从中心球边缘开始（不穿过球）；轴正对相机（投影 <6 DIP）时隐藏背向端点与轴线、朝向端点置于中心球中央；标签仅正方向且朝向时显示（11 DIP 半粗）；新配色 X #C4874F / Y #5684A8 / Z #8EA8C2、球 #D7DEE6、描边 #66788B、背向 30% Alpha；Hover 亮环；editor_nav_gizmo.frag 重写并经 glslc 重新生成 ShaderBytecode.NavGizmoFrag.cs（120 词/行，38 行）；
  - **Gizmo 视觉终版（用户直接提供 shader，同轮替换）**：保持 80B Push/96 DIP/14 边距/CPU 命中热区 13 DIP 不变，视觉半径与命中半径分离——中心球缩至 9.5 DIP 轻量球（径向渐变+左上高光+细描边，不再是大白圆盘）；轴线 1.25 DIP、从球边缘开始；端点正 7.5/背 3.8/正对 8.5 DIP；正对相机只保留一个前方端点；X/Y/Z 文字绘制在端点圆内部（含正对负方向的轴字母）；五层合成（背向轴/端点→中心球→朝向轴/端点→端点内部标签→单一 Hover 环），预乘合成适配 SrcAlpha 混合；新配色 X #C66A5E 珊瑚红 / Y #6B9F84 豆青 / Z #628EC2 钢蓝；editor_nav_gizmo.frag 经 glslc 重新生成 ShaderBytecode.NavGizmoFrag.cs（120 词/行，58 行）；OverlayContractTests 合同断言同步至新结构；NavigationGizmoLayout/HitTest/相机/DrawPlan/Pipeline 零改动；
  - **正交投影/视图平面网格**：审计确认仓库无 Orthographic（仅透视 FOV）→ 按计划冻结为 F3-F4。
- 验证：新增 CameraNavigationRollTests（斜视 Orbit 后 Up 保持 +Z 主导且无水平横移、100 次环绕不累积倾斜、顶/底视 Orbit 稳定）+ Gizmo 正对合同测试（.Facing.cs）；红→绿：修正 4 处测试期望（均为合同变更/测试数据错误：85° 俯角 fallback +Y 是稳定态、斜视 up 自然倾斜、顶视基测试数据）；Core 全量 291/291、World 435/435、WarCore 22/22。
- 视觉冒烟：**未执行**（本环境无法操作画面），如实记录——自动测试通过、真机待用户验收；不宣布 F3-F3 视觉通过、不关闭阶段。
- 治理：版本 v0.2.24.15-fix → v0.2.24.16-fix（五处同步）；新增 CameraNavigationRollTests.cs/NavigationGizmoLayoutTests.Facing.cs；CameraState 严格合同未放宽；未创建 Tag/Release。

## v0.2.24.15-fix
MAP-A-R1-D5-R1-F3-F2 相机正交基不变量与导航组合链崩溃修复（2026-08-03，Commit 本轮落库为准）
- F3-F1 真机验收：**FAIL**。故障：滚轮 Dolly 构造 CameraState 时 up 非法，ArgumentOutOfRangeException 逃出 Win32 消息循环，编辑器进程退出。
- 根因（失败测试先行 + 源码确认，非计划推测）：`CameraNavigation.Result()` 硬编码 `Up=Vector3d.UnitZ` 拼接新 Forward——点击导航 Gizmo 顶视（Forward=-Z）/底视（Forward=+Z）后，任何 Dolly/Orbit/Pan 都令 Forward 与 UnitZ 平行，触发 CameraState 第 24 行合同（`Forward.Cross(up).Length<1e-6` 抛异常）。另确认两项伴随缺陷：标准视角命令未同步 `_observationCenter`；底视 Up=+Y 导致屏幕右方向为 -X（镜像）。
- 修复：新增 `XuanYu.Editor/Camera/CameraBasis.cs`（唯一正交基生成器：Forward 有限非零 → PreferredUp 优先（|dot|<0.98）→ 平行时回退世界轴 +Z/+Y/+X 最不平行者 → Right=Forward×Ref、Up=Right×Forward，输出前正交验证；不进入 Core）；`CameraNavigation` 拆 `CameraNavigation.Try.cs`，TryDolly/TryOrbit/TryPan/TryResult 统一走 CameraBasis（PreferredUp=start.Up 保留 Up 语义），同步版 API 保留；UiVm 失败安全：Try* 成功才替换相机/中心/Revision，失败保留旧状态并记录「相机 Dolly 失败」错误日志，异常不再逃出输入循环；标准视角同步 `_observationCenter`；底视 Up 修正为 `-Y`（Right 保持 +X，防镜像，计划八合同）。
- 验证：新增 CameraBasisTests 9 项（零/NaN/平行/顶底视/重合失败/超大坐标正交）+ CameraNavigationSequenceTests 11 项（六方向后 Dolly/Orbit/Pan 链）+ CameraNavigationUiSequenceTests 8 项（顶视→Orbit→Pan→Dolly、底视→Resize→Dolly、Gizmo Commit/Cancel 后 Dolly、失败保留状态、不 Dirty/Undo）+ CameraNavigationStressTests（100 次循环正交保持）；红→绿：修复前 9 项 FAIL（含崩溃复现），修复后聚焦 49/49；Core 相机/视口/Gizmo 151/151、World 435/435、Core 全量 267/267。
- 视觉冒烟：**未执行**（本环境无法操作画面），按计划如实记录——自动测试通过、真机待用户验收；不宣布 F3-F2 视觉通过、不关闭阶段。
- 治理：版本 v0.2.24.14-fix → v0.2.24.15-fix（五处同步）；新增 CameraBasis.cs/CameraNavigation.Try.cs/4 个测试文件（file-tree 已登记）；CameraState 严格合同未放宽；未创建 Tag/Release。

## v0.2.24.14-fix
SHR-2026-08-R2 全盘阶段考核：文档事实源审计与 docs 分类治理（2026-08-03，Commit 本轮落库为准）
- **file-tree.md 重建**（885→843 行）：删除全部按轮次职责索引（宪法第五十五条禁止的每轮快照）；以真实 `git ls-files` 树为准重建，修正 ARCH-WORLD 迁移后失效路径（Scene/World/History 等不再误写 Core），补齐 WarCore/Map/Assets/StaticModel/F2-F3 新文件职责，全部条目一行职责、无历史流水账。
- **changelog 审计**：条目守恒 310（296 归档 + 14 当前）；审计发现 7 月归档 3 处同一版本号分配给两个不同轮次的历史缺陷（v0.2.16.2-rz/v0.2.17.8-rz/v0.2.20.19-fix）与 18 处版本号-日期非单调——按不篡改历史原则登记注记不重写；归档结构调整为 `docs/archive/changelog/` 子目录。
- **docs 分类迁移**：根目录 178 文件 → 3 个入口（+docs-index.md 新增）；建立 governance（版本/债务/审计）、architecture（引擎架构/坐标合同）、milestones/current/MAP-A、milestones/closed/{ARCH-A,ARCH-B,ARCH-C,ARCH-WORLD,WORLD-A,WORLD-B,WORLD-C,RZ-VK,M1}、archive/{changelog,superseded} 分类；全部 `git mv` 保留历史；修复 4 处失效路径引用（dev-rules×2、map 合同、债务登记、changelog 活跃条目×2）。
- **代码架构语义审计**：依赖图与宪法/arch-a-guard 一致（Core 零依赖、Editor.UI 无 Vulkan、App=组合根、Win=宿主）；Core 无地球/经纬/国家语义；无第二套 EntityRegistry/空间索引；地图文档=MapDocumentOwner(Editor)、运行时=WorldMapStateOwner(World)、渲染只消费 MapRenderSnapshot；高频路径无日志/全量扫描。结论无 BLOCKER；2 观察项（地图元素尚未实体化须走 EntityId；地图文档编辑入 Undo 链待正式地图编辑器明确）。
- 验证：D6 交叉核对 10 问全 PASS（路径/归档/分类/守恒/残留/重复/版本）；正式串行门禁见本轮报告。
- 治理：版本 v0.2.24.13-fix → v0.2.24.14-fix（五处同步）；未创建 Tag/Release；SHR-2026-08 重新 CLOSED。

## v0.2.24.13-fix
SHR-2026-08 阶段健康考核与治理收敛（2026-08-03，Commit 本轮落库为准）
- P0：宪法 2.0 独立入库（`docs: 生效玄域引擎AI开发宪法2.0`，b99e087，消除双事实源）；修复 arch-a-guard 5+100 行数统计漏检（PS 5.1 `Measure-Object -Line` 实测失真 109→96，改用 `[System.IO.File]::ReadAllLines` 确定性统计 + 8 样本门禁自验证，检查范围对齐宪法第十三条 .cs/.axaml/.js）；治理 3 个超限文件（WorldRotateTransformUiTests.R4R2.cs 109→66+Helpers 52、WorldToolStateHighlightUiTests.cs 105→85+Selection 26、Left.axaml 101→89+Left.Styles.axaml 16，真实拆分不压行）。
- P1：10 个 catch 逐处分类治理（B 类清理 best-effort 语义注释 3 处、C 类 UI 生命周期竞态注释 1 处、D 类 Gizmo 投影退化类型化+回退语义 3 处，另 3 处复核为正常业务处理不变）；dev-rules §17 失效"宪法第二十八章"引用改条款号+标题、版本规范"宪法第十六章"改第四十二条《版本一致性》、Editor.App=组合根/Editor.Win=Windows 平台宿主职责描述修正；changelog 月度归档 5/6/7 月 → `docs/archive/changelog-2026-{05,06,07}.md`（4436→200 行，含归档规则+历史索引）。
- P2：docs 分类框架落地（docs/archive/ 历史归档分类）；其余约 175 个历史文档平铺分类登记为渐进治理事项（每月一个逻辑簇，不阻断 MAP-A）。
- 验证：World.Tests/Editor.UI 快速编译 0 错误；修复后 arch-a-guard 全量 PASS（含自验证，此前同门禁对 3 个超限文件误报 PASS）；正式串行门禁见本轮最终报告。
- 治理：版本 v0.2.24.12-fix → v0.2.24.13-fix（五处同步）；未创建 Tag/Release；`IDEA.md` 已删除（无有效内容）。

## v0.2.24.12-fix
MAP-A-R1-D5-R1-F3-F1 世界原点屏幕空间标记 + 导航 Gizmo 移入 Vulkan Overlay Pass（2026-08-03 16:10:00，Commit 本轮落库为准）
- F3-A1（v0.2.24.11-fix）：**FAIL**。用户真机验收：
  1. 世界原点退化为黄色地面面片（旧实现贴 Z=0 世界空间面片，低角度透视被压扁成梯形）；
  2. 导航 Gizmo 真机零像素（Avalonia 覆盖层被 NativeControlHost 承载的 WS_CHILD 原生子窗口遮挡——airspace 问题，ZIndex/Margin/Opacity 均无效）。
- 修复（本版本，按用户指定方向——先调查层级后实现，不再调 XAML）：
  - **F3-F1-A 世界原点重写**（editor_world_origin.frag）：去掉射线求交与贴地投影；改为世界原点 (0,0,0) 投影到屏幕后画**恒定屏幕尺寸**的细十字线 + 小空心圆 + 中心点（蓝灰描边 #718096、中心淡金褐点 #C18A55、十字半长 8px/圆环半径 5px≈10~16 DIP）；相机后方/屏幕外 discard；深度保持原点平面深度（实体近则自然遮挡）；不再随视角压扁、不与地平线混同；
  - **F3-F1-B 导航 Gizmo → Vulkan 屏幕空间 Overlay Pass**：新增 editor_nav_gizmo.vert/.frag + ShaderBytecode.NavGizmoVert/Frag；新增 VulkanClearFrameOwner.NavGizmo.cs（80B push：cameraRight/Up/Forward + 视口 + DPI + gizmo 参数 + hover 索引）；CreateFullscreenPass 增加 depthTest 参数（Gizmo 用 DepthTest=Off/DepthWrite=Off）；GridPipelineSet 增加 NavGizmo 管线；DrawPlan 恒以 NavigationGizmo 收尾（RenderDrawKind 新增）；右上角 12 DIP 边距 88 DIP 区域；中心球 #CDD6DF + 三轴（X #C18A55/Y #5F87A7/Z #A9B8C7）+ 六端点（背向 40% Alpha 小点、朝向 100% 大点带 X/Y/Z 标签）+ 深度排序 + hover 高亮；
  - **F3-F1-C 命中走原生指针流**：Avalonia ViewGizmo/ViewNavigationGizmo 控件删除（UiRoot 移除引用）；VulkanNativeHost.NavGizmo.cs 在 OnNativePointerMessage 中先判右上角区域（视口→Gizmo 局部坐标），端点点击 → StandardViewResolver 标准视角命令，中心球/空白拖动 → 复用 UiVm 相机会话 Orbit（4 DIP 阈值区分点击/拖动）；CaptureLost/取消正常结束；控件区域外不截获（实体 Picking/框选/变换 Gizmo 不受影响）；导航不进入 Dirty/Undo；
  - DPI 链路：RenderProjection 增加 ViewportDpiScale；UiVm.UpdateViewportDpi（LayoutSync 调用）；RenderCameraProjection 增加 Right 计算属性。
- 验证：聚焦 NavigationGizmo/StandardViewResolver/ViewportChrome/OverlayContract 33/33；Core 258/258、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；git diff --check OK；glslc 字节码三文件逐字 MATCH。
- 视觉冒烟：**未执行**（沿用用户决定，留真机验收）；请重点复验：原点不再贴地压扁（十字+空心圆+中心点）、右上角 Gizmo 可见且随相机旋转、六方向点击/拖动、顶底视图无滚转。
- 治理：版本 v0.2.24.11-fix → v0.2.24.12-fix（五处同步）；无新增依赖/项目；不创建 Tag/Release。

## v0.2.24.11-fix
MAP-A-R1-D5-R1-F3 视口黑边移除 + Blender 风格导航 Gizmo（2026-08-03 15:20:00，Commit 本轮落库为准）
- F3 问题（用户验收反馈）：
  1. 视口外层存在黑色粗边框和厚重圆角（两层深色容器：VulkanViewport.axaml `#0b1220`/`#31405d` + UiRoot 中央 `#101827`/Padding=18/圆角8/BoxShadow）；
  2. 右上角仍是白色占位块（ViewGizmo.axaml 3×3 按钮 + `#dce6f2` 圆角卡片），缺少正式导航 Gizmo。
- 修复（本版本）：
  - **F3-D1 去黑边**：VulkanViewport 与 UiRoot 中央容器改为浅灰 1 DIP 分隔（`#C9D2DC`）、无圆角、无 Padding、无深色背景、无 BoxShadow；Fallback 层改浅色 `#E8EEF5`；ClipToBounds 保留；
  - **F3-D2 Blender 风格导航 Gizmo**：替换白色占位为透明 88×88 覆盖层（右上 12 DIP）——中心球（`#CDD6DF`/描边 `#718096`）+ 三根世界轴 + 六正负端点 + X/Y/Z 标签；玄域低饱和配色（X `#C18A55` 淡金褐、Y `#5F87A7` 蓝灰、Z `#A9B8C7` 浅钢灰）；背向端点 40% Alpha 小圆点、侧向 78%、朝向 100% 大端点带标签；按深度升序绘制（背向先、朝向后）；轴正对相机时端点收缩中心无 NaN；控件完全透明无底板；
  - **F3-D3 交互**：点击六端点 → 标准视角命令（+X/-X/+Y/-Y/顶/底视图，保留 Pivot 与距离，Up 合同：±X/±Y=+Z、顶/底=+Y 防滚转）；中心球/空白拖动 → 复用 UiVm 相机会话 Orbit（同一灵敏度/俯仰限制/Pivot）；点击/拖动阈值 4 DIP；Hover 亮环 + Hand 光标；PointerCaptureLost 正常取消；控件 88×88 外不截获输入（实体 Picking/框选/变换 Gizmo 不受影响）；导航不进入 Dirty/Undo/场景文件；
  - 拆分职责（5+100）：ViewNavigationGizmo.cs（属性状态）/ .Layout.cs（投影纯数学）/ .Render.cs（绘制）/ .HitTest.cs（命中）/ .Input.cs（输入命令）；StandardViewResolver.cs（六方向解析 + 端点名映射）；UiVm.NavigationCamera 快照（相机变化统一通知 Gizmo）。
- 验证：聚焦 NavigationGizmo/StandardViewResolver/ViewportChrome 29/29；Core 254/254、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；git diff --check OK；XAML 加载由构建编译验证。
- 视觉冒烟：**未执行**（沿用上轮用户决定——冒烟留用户真机验收 F3 十一项清单）；请用户重点复验：黑边消失、白色卡片消失、Gizmo 六方向与网格一致、点击/拖动、顶底视图无滚转。
- 治理：版本 v0.2.24.10-fix → v0.2.24.11-fix（五处同步）；无新增第三方依赖/项目；不创建 Tag/Release。

## v0.2.24.10-fix
MAP-A-R1-D5-R1-F2-R3-R2 背景颜色移到片元级（2026-08-03 14:10:00，Commit 本轮落库为准）
- F2-R3-A3（v0.2.24.9-fix）：**PARTIAL FAIL——中性灰参考地面 FAIL**。现象：截图下半部分仍是偏蓝背景（比天空略暗），未形成明确中性灰地面；网格线宽统一与 LOD 缩放观感 PASS。
- 根因（用户验收确认，与开发记录一致）：
  1. 颜色计算在顶点着色器 `backgroundVertex()`——全屏三角形仅 3 个顶点计算视线方向并判断天空/地面，中间像素全靠插值，地平线与灰地被插值冲淡成整片蓝灰渐变；
  2. 两个 smoothstep 参数反写（`smoothstep(0.0, -0.06, dir.z)` 与 `smoothstep(-0.06, -0.5, -dir.z)`，edge0 > edge1 未定义行为），第二个还有符号错误——地面远近基本不变化；
  3. 自动测试只能证明"灰色数字写进 Shader 且可编译"，不能证明颜色出现在视口地面区域。
- 修复（本版本）：
  - **背景颜色移到片元级**：`scene.vert` backgroundVertex 只输出全屏三角形位置与 NDC（哨兵 (2,2) 表示非背景分支）；`scene.frag` 每像素用 flat 传入的 `vInvViewProjection` 重建世界视线（invVP 每顶点算一次避免每像素求逆），`dir.z >= 0` 画天空、`dir.z < 0` 画灰色参考地面；
  - **smoothstep 方向修正**：`belowHorizon = 1 - smoothstep(-0.06, 0.0, dir.z)`、`groundNearness = smoothstep(0.06, 0.50, -dir.z)`——全部 edge0 < edge1，地平线过渡与地面远近（远处灰 → 近处深灰）正确；
  - **配色拉开对比**（用户建议）：天空顶部 `#A6C0DF` → 天空近地平线 `#B3C6DA` → 地平线 `#9CA6AF` → 远处地面 `#858B91` → 近处地面 `#747A80`；
  - 太阳圆盘/辉光保留（D1 合同 sunDirection 不变）；背景仍不写深度、不进地图/场景/拾取/碰撞；
  - 未触碰：网格 Shader、线宽 0.82、1/2/5 LOD、世界轴、原点、DrawPlan、地图、地形、相机。
- 验证：聚焦 83/83；Core 225/225、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；glslc SceneVert/SceneFrag 逐字 MATCH；git diff --check OK。
- 视觉冒烟：**未执行**（沿用上轮用户决定——冒烟留用户真机验收）；本轮修复点明确（顶点→片元 + smoothstep 方向），请用户按 F2-R3-A3 复验，重点：默认斜视（天空/灰地/地平线分离）、压低视角（地平线平滑、无硬切线）、有地图（地图覆盖灰地）。
- 治理：版本 v0.2.24.9-fix → v0.2.24.10-fix（五处同步）；无新增文件/依赖；不创建 Tag/Release。

## v0.2.24.9-fix
MAP-A-R1-D5-R1-F2-R3 网格线宽统一 + Unity 风格灰色参考地面（2026-08-03 13:05:00，Commit 本轮落库为准）
- F2-R2-A2（v0.2.24.8-fix）：**FAIL**。现象：缩放时 Fine/Coarse 使用不同线宽（0.70px vs 1.00px）且重合处直接相加，部分网格线看起来忽粗忽细、层级交界出现明暗脉冲；编辑器参考地面整体偏蓝，与天空层次不足。
- 修复（本版本）：
  - **R3-A 唯一像素线宽**：删除 FineWidthPixels/CoarseWidthPixels 双宽度，统一 `GRID_LINE_WIDTH_PX = 0.82`（硬合同 0.78~0.90，≤1.0；世界轴保持 1.25px > 网格）；
  - **R3-A 非累加合成**：`gridAlpha = max(fineContribution, coarseContribution)`（禁止 fine+coarse 相加 → 无双重 Alpha、无粗黑线）；颜色按贡献加权归一化混合（total 仅用于颜色，Alpha 仍为 max）；
  - **R3-A 配色收敛**：Fine `#5D6670` α0.16 / Coarse `#525C67` α0.24（差 0.08 ≤ 0.10，克制深浅差防"深色=更粗"错觉）；
  - **R3-B 中性灰参考地面**：scene.vert backgroundVertex 程序化背景扩展为 天空顶部 `#9DBBE0` → 天空近地平线 `#AEC4DC` → 地平线混合区 `#9DA5AD` → 远处地面 `#8B9299` → 近处地面 `#7B8289`；地平线过渡按视线方向 dir.z（[-0.06,0] 柔和混合），地面远近按 dir.z ∈ [-0.06,-0.5] 轻微渐变；不写深度、不进地图/场景/拾取/碰撞，地图与实体自然覆盖；
  - 未触碰：ReferenceGridScale 1/2/5 选级、48px 目标、相机求交、LOD 权重、方向性抗摩尔纹、深度偏移、世界轴/原点架构、地图/地形/光照。
- 验证：聚焦 ReferenceGrid/VisualStyle/ShaderContract/DrawPlan 82/82；Core 224/224、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100，ShaderBytecode.Vert.cs 保持原 120 词/行紧凑格式 76 行）；glslc 字节码 GridFrag/SceneVert 逐字 MATCH；git diff --check OK。
- 视觉冒烟：**未执行**（用户选择跳过，图像待用户真机验收 F2-R3-A3 十项清单——不得视为 PASS）。
- 治理：版本 v0.2.24.8-fix → v0.2.24.9-fix（五处同步）；新增 ReferenceGridVisualStyleTests.cs；无新增第三方依赖/项目；不创建 Tag/Release。

## v0.2.24.8-fix
MAP-A-R1-D5-R1-F2-R2 统一网格尺度与轴线修复（2026-08-03 11:40:00，Commit 本轮落库为准）
- F2-A1（v0.2.24.7-fix）：**FAIL**。现象：逐屏幕位置 LOD 导致横向密度分区（近 0.1/中 1/远 10 单位并存）；近处摩尔纹与灰色叠块；世界轴出现楔形；网格 Shader 与独立 WorldAxes Pass 存在轴线重复绘制。
- 修复（本版本）：
  - 取消逐 Fragment LOD——每帧 CPU 由视口中心射线与 Z=0 求交（中心±1px 世界距离取 max）得参考世界每像素，整帧统一 Fine/Coarse 层级；
  - 1/2/5 十进制序列（0.01/0.02/0.05/0.1/0.2/0.5/1/2/5/10…），目标 48px/格，对数域相位 + smoothstep 互补交叉淡化（FineWeight+CoarseWeight≈1，边界旧 Coarse=新 Fine 无缝）；
  - 求交失败回退：中心 → 视口偏下 60% → 上一帧合法尺度（禁止重置为 1）；
  - 方向性抗摩尔纹：X/Y 各自按单元屏幕间距淡出（<6px 隐藏、6~12 渐入、>12 正常）；
  - 轴线单一事实源：网格 Shader 删除 X/Y 轴与原点绘制；新增独立 WorldAxes 全屏 Pass（金 X=世界 Y=0、蓝 Y=世界 X=0，各自方向导数固定 1.25px 屏幕宽度）与 WorldOrigin 全屏 Pass（琥珀原点标记 ≤4px 半径）；三个 Pass 开关（ShowGrid/ShowWorldAxes/ShowOrigin）完全独立；
  - 深度偏移有界化：clamp(fwidth(depth)×0.5, 1e-7, 2e-5)；
  - DrawPlan 顺序（方案 12）：背景 → 地形(MapBounds) → 网格 → 原点 → 世界轴 → 实体填充 → 轮廓 → Gizmo。
- 验证：聚焦 ReferenceGrid/WorldAxes/DrawPlan/Shader 合同测试 73/73；Core 215/215、World 435/435、WarCore 22/22；arch-a-guard PASS（含 5+100）；glslc 字节码四文件逐字一致；git diff --check OK。
- 视觉冒烟：**仅完成启动冒烟，图像待用户验收**（本环境无 computer_use 工具无法读取编辑器截图，按宪法不得写视觉 PASS）。启动冒烟实测：编辑器进程启动后存活 72.9s 无崩溃、无 Vulkan 会话回滚（三全屏 Pass 管线创建成功）；F2-A2 三张截图（默认斜视/拉近/拉远）待用户执行。
- 治理：版本 v0.2.24.7-fix → v0.2.24.8-fix（五处同步）；新增 ReferenceGridScale.cs（纯数学）、WorldAxes/WorldOrigin Shader + 字节码、GridPipelineSet.cs、GridScale.cs、ShaderContractTests/ReferenceGridScaleTests；无新增第三方依赖；不创建 Tag/Release。
- F2-A2 真机验收清单已交付（9 项：默认斜视/拉近/拉远/平移/环绕/独立开关/实体遮挡/有地图无地图/窗口尺寸），待用户执行。

## v0.2.24.7-fix
MAP-A-R1-D5-R1-F2 无限参考网格稳定性修复（2026-08-03 10:40:00，Commit 本轮落库为准）
- 任务目标：修复截图中"普通网格几乎不可见、只剩两条坐标轴"问题——稳定的缩放自适应层级、普通网格可见、远处无闪烁/地平线无噪声、有地图时网格不受地图边界裁剪；不修改天空/光照/地形/视角 Gizmo/地图编辑器/Schema。
- 根因（代码调查，非计划推测）：
  1. **线宽公式参数反转（主因）**：`gridLine` 内 `smoothstep(vec2(0.5), edge, f)` 中 `edge = 0.5 - d×linePixels/2 < 0.5`，edge0 > edge1 属 GLSL 未定义行为，实际线宽 = `1/d - linePixels` 像素——远处 d→0 时线宽爆炸为数十像素宽的淡带，近处趋近 0，普通网格视觉上消失；
  2. **层级目标间距 20px 过小**：desiredStep = worldMetersPerPixel×20，量化后细格屏幕间隔平均仅 ~8px，过密成噪声，且权重窗口 0.25~0.75 互补导致细格常被完全压掉（仅剩 0.18 基础 α）；
  3. **地图矩形内 discard**：F2A 为规避 Z-fighting 在 shader 内按地图矩形裁剪网格，有地图时视野内网格全部消失，违背"无限网格不受有限地图边界裁剪"；
  4. **坐标轴过强**：α0.78/2.5px 压过网格，且 X/Y 颜色与方案相反。
- 修复（editor_reference_grid.frag 重写 + DrawPlan 顺序调整）：
  - 线宽改用方案 4.6 标准公式 `1 - smoothstep(w-0.5, w+0.5, 像素距离)`：细 0.75px / 主 1.10px / 轴 1.35px，屏幕恒定不再随距离爆炸；
  - 目标间距 36px/格（`worldMetersPerPixel×36`，合法层级 0.1~10000 钳制）；细格权重 `1-smoothstep(0.5,1.0,phase)` 1→0、主格加深权重 `smoothstep(0.0,0.5,phase)` 0→1；
  - **跨级透明度连续**：主格线位置是细格子集，细格基础 α0.20 + 主格加深 α0.18，同组线跨级时从主格 0.18 平滑过渡为细格 0.20（差 ≤0.02），不跳格不闪烁；
  - **移除地图矩形 discard**：网格为无限参考平面，不再按地图裁剪；共面稳定改由 `gl_FragDepth = depth - max(fwidth(depth)×1.5, 1e-7)` 像素级深度偏移实现（实体/凸起地形仍正常遮挡，符合方案八）；
  - 配色按方案：细格 #566A82 α0.20、主格 #344A63（基础 0.20+加深 0.18）、X 轴 #AD8550 α0.62（世界 Y=0 线）、Y 轴 #557C9E α0.62（世界 X=0 线）、原点 #D1AE69 α0.70；坐标轴 1.35px 不再抢眼；
  - 掠射角淡出窗口 0.015~0.080（方案七）；距离淡出保持 0.45~0.75 far、gridMaxDistance=far×0.75（基于 far 约定，未硬编码米数）；
  - **DrawPlan 顺序修正**：网格从"天空之后"移到"地形/实体之后、轮廓/Gizmo 之前"（RenderDrawPlan.GetFrameDrawPlan），实体可遮挡网格、平坦地形上经深度偏移稳定显示；有/无地图、有/无实体均保留网格；
  - PushConstant 从 192B 缩为 160B（移除 mapParams/mapParams2，40 float），vert/frag/C# 三处同步，管线 maxPushConstantsSize 校验同步；
  - 相机相对坐标：本轮保持绝对 float32（与实体同机制，地图 2000m 量级内精度足够）；大尺度世界相机相对化属全局渲染原点架构问题，按方案九不强行扩围，另行登记。
- 测试：`ReferenceGridAdaptiveTests` 重写（×36 目标、两级相邻+权重区间、跨级 α 差 ≤0.02 连续性、phase=0.5 峰值 0.38、距离/掠射角曲线）；`ReferenceGridDrawPlanTests` 新增 4 组合（有/无地图×有/无实体）+顺序断言（实体后、Gizmo 前）+关闭开关缺席；`ViewportAssistDrawPlanTests`/`MapRenderDrawPlanTests` 顺序断言同步；Core 189/189、World 435/435、WarCore 22/22；arch-a-guard PASS；glslc 字节码逐字一致（GridVert 336、GridFrag 1379 词）。
- 治理：版本 v0.2.24.6-rz → v0.2.24.7-fix（五处同步）；无新增项目/依赖；不创建 Tag/Release。
- 状态：MAP-A-R1-D5-R1-F2 自动门禁全绿，真机验收待用户执行（IPO 清单见报告）；通过前不宣布 D5-R1 CLOSED，不进入 D5-R2。

## v0.2.24.6-rz
MAP-A-R1-D5-R1-F2/F2A Blender 风格自适应参考网格（2026-08-03 00:30:55，Commit 909b6fd 之后待收口）
- 任务目标：废弃 42 条世界空间粗四边形网格，改为独立全屏 Pass + 片元解析世界 Z=0 平面，实现 Blender 式无限自适应参考网格；只动网格，不处理 Gizmo/天空/取景。
- 独立渲染管线：新增 `editor_reference_grid.vert/.frag` + `VulkanGraphicsPipelineOwner.Grid.cs`（独立 192B PushConstant，创建时校验设备 maxPushConstantsSize；DepthTest=On/LessOrEqual、DepthWrite=Off、AlphaBlend=On）+ `VulkanClearFrameOwner.Grid.cs`（VP/InvVP/相机/视口/far/地图参数填充）；`RenderDrawKind.EditorGrid` → `EditorReferenceGrid`（顶点数 252→3 全屏三角形）；scene.vert 移除 gridVertex 与 -10.5 魔法分支；DrawAssist 不再处理网格。
- 自适应分级：`desiredStep = worldMetersPerPixel × 20`（合法层级 0.1/1/10/100/1000/10000，钳制 0.1~10000）；只混合相邻两个十进制层级，权重和=1，平滑交叉淡入（细格 1px α0.18、主格 2px α0.32）。
- 淡出：距离淡出 0~45% far 完整 / 45~75% 平滑 / >75% 隐藏；掠射角淡出 abs(dot(N,V))<0.03 隐藏 / 0.03~0.12 淡入 / >0.12 完整。
- 主轴与地图：X 轴（世界 Y=0，#5A7FA3 α0.78）、Y 轴（世界 X=0，#B68B54 α0.78）、原点标记（#D1AE69 α0.85），屏幕恒定 ~2.5px 贯穿可见平面；地图矩形内逐片元 discard（feather=像素×1.5 或 0.05），地图外网格继续显示，卸载后完整恢复。
- 配色（玄域浅色编辑器，禁高饱和/荧光/红绿工程轴）：细格 #7E8FA1 α0.18、主格 #607487 α0.32。
- 测试：`ReferenceGridRayIntersectionTests`（G1 射线求交 7 项）+ `ReferenceGridAdaptiveTests`（层级选择/权重和/钳制/淡出曲线/裁切，28 项）+ `ReferenceGridDrawPlanTests`（有/无地图网格存在+顺序）；Core 183/183、World 435/435、WarCore 22/22；arch-a-guard PASS；glslc 字节码逐字一致（GridVert 348、GridFrag 1559、scene.vert 7864 词）。
- 已知基线说明：909b6fd 的 scene.frag 源码（78 词透传版）与内嵌 ShaderBytecode.Frag.cs（113 词 F1 版）本身不一致（基线遗留，本轮未触碰 scene.frag/其字节码，超出 F2A 冻结范围）。
- 治理：版本 v0.2.24.5-rz → v0.2.24.6-rz（五处同步）；无新增项目/依赖；不创建 Tag/Release。
- 状态：MAP-A-R1-D5-R1 网格专项真机验收通过（用户授权收口 push）；后续 Gizmo/天空/取景按 F2 纪律单独轮次处理。

## v0.2.24.5-rz
MAP-A-R1-D5-R1 视口参照与导航（2026-08-02 22:57:00，Commit 2fdf470 之后待收口）
- 任务目标：按用户最新真机裁定修正视口参照与导航——视觉无限参考网格、地图外网格延伸、右上角视角 Gizmo 真实可见、程序化天空渐变、自动取景屏幕占用率 65~75%。
- 视觉无限 EditorReferenceGrid：`scene.vert gridVertex` 重构——网格重心跟随相机（worldPosition.xy 对齐间距）、间距按相机高度分级 0.1/1/10/100/1000/10000 米、线长覆盖 step×12、主次线分级宽度；`RenderDrawPlan` 取消 HasMap 时移除 EditorGrid（地图存在时网格保留，地图矩形由 shader 裁切避免穿透地表与 Z-Fighting，卸载后网格继续存在）；`VulkanClearFrameOwner.DrawAssist` EditorGrid 分支传相机位置 + 地图半宽/半深（entityScale.xy 复用，push constant 128B 不扩容）。
- 视角 Gizmo 真实可见：根因是 ViewGizmo 位于 VulkanViewport Grid 内被嵌入 Win32 原生窗口遮挡——移至 `UiRoot.axaml` 视口 Border 外层 Grid（Avalonia 覆盖层，位于原生渲染窗口之上），六方向按钮 + 当前朝向琥珀描边。
- 程序化天空增强：天顶饱和蓝 (0.22,0.45,0.85) → 地平线更雾白 (0.88,0.92,0.97)，pow 0.55→0.35 渐变更快集中；仍为独立 Sky Pipeline（DepthTest/Write=Off、Z-Up 读 dir.z、只依赖相机旋转）。
- 地图自动取景：`FrameMapAllWithCenter` 改为按目标屏幕占用率（垂直投影约 70%，透视补偿 ×1.55，实测 d≈2850 时占用率≈69%、最大视锥角 28.5°<30°）计算距离，地图不再过小；新增 `WorldCameraFramingOccupancyTests`（NDC 投影包围盒 65%~80%）。
- 世界坐标轴颜色：X=浅蓝灰 (0.55,0.62,0.70)、Y=冷钢蓝 (0.42,0.52,0.64)、Z=柔和琥珀 (0.78,0.66,0.42)，禁止高饱和红绿轴。
- ShaderBytecode：glslc -O 重新生成（8762 词，83 行）逐字比对一致。
- 测试：`WorldCameraFramingOccupancyTests` 新增（占用率 65~75%）；`MapRenderDrawPlanTests.With_map_grid_kept_and_bounds_added` 更新（D5-R1 需求变更：网格保留而非移除）；World 435/435、Core 148/148、WarCore 22/22；arch-a-guard PASS。
- 治理：版本 v0.2.24.4-rz → v0.2.24.5-rz（五处同步）；无新增项目/依赖；不创建 Tag/Release。
- 状态：MAP-A-R1-D5-R1 真机人工验收待用户执行；通过后进入 D5-R2 真实参数编辑。

## v0.2.24.4-rz
MAP-A-R1 D4 视觉收口 + D5 正式地图编辑器/场景引用（2026-08-02 22:32:36，Commit 5fcd02b 之后待收口）
- 任务目标：把 D4 真机视觉缺陷收口（程序化天空、视角 Gizmo、正式地图编辑器、场景地图引用），完成 MAP-A-R1 功能闭环；D4/D5 各轮独立提交推送。
- EDITOR-VIEW-R1 视角 Gizmo：`UiVm.ViewGizmo.cs` + `ViewGizmo.axaml`——视口右上角 3×3 网格六方向按钮（顶/底/前/后/左/右）+ 中心当前朝向琥珀描边；Z-Up 坐标合同冻结（顶=+Z 看向 -Z、前=-Y 看向 +Y 等）；保持观察中心（选中实体→地图中心→原点）与距离只改朝向；浅蓝灰主体、白字，无红绿蓝三轴配色；不建第二套 CameraState；测试 3 项（六方向朝向/中心距离保持/选择保持）。
- D5-A 正式地图编辑器：右侧一级「地图编辑器」Tab（与检查器平级）——地图资产区（名称/路径/MapId/尺寸/状态）+ 新建/打开/保存/卸载/聚焦五命令；复用 D2 MapDocumentOwner/MapStorageService 与 D3 WorldMapStateOwner；打开失败保持原地图；第二排「加载测试地图/卸载地图」临时按钮已删除；测试 4 项。
- D5-B 场景地图引用：`.xyscene` schema v3→v4 新增可选 `mapReference{mapId, assetPath}`（只存引用不复制地图数据）；旧场景无引用正常打开；缺失/损坏时场景主体打开 + 显示「引用失效」+ 路径原因 + 不自动建默认地图；保存附加引用、打开自动加载；测试 4 项 + schema v4 断言更新。
- 验证结果（D6 最终门禁）：全解决方案强制重编译 0 error / 1 既有 warning（xUnit2013，非本轮引入）；Core 148/148、World 434/434、WarCore 22/22；arch-a-guard PASS；glslc 字节码 8293 词逐字一致；git diff --check PASS；5+100 本轮文件全过（3 个既有超限文件非本轮范围，守卫口径 PASS）。
- 治理：版本 v0.2.24.3-rz → v0.2.24.4-rz（五处同步：changelog/file-tree/UiVm.SceneDocument.cs/UiWin.axaml/run.bat）；无新增项目/依赖；不创建 Tag/Release。
- 状态：MAP-A-R1-D4/D5 真机人工验收待用户执行（IPO 清单见报告）；全部通过后 MAP-A-R1 CLOSED，进入 MAP-A-R2 区域与图层。

## v0.2.24.3-rz
MAP-A-R1-D4 有限地表渲染与自动取景（2026-08-02 21:46:52，Commit 9d1f2c9 之后待收口）
- 任务目标：让地图以可观察、可编辑的战场方式出现在视口——有限地表网格、缓丘明暗、程序化天空、地图边界、加载后斜上方自动取景；D4 真机修复收口。
- D4 主体（基线 9d1f2c9 已含）：`MapTerrainMeshBuilder`（唯一采样源 MapSurfaceSampler 的渲染侧消费方，4225 顶点/24576 索引，CPU 数值差分法线 + 预计算亮度）、`MapBoundsMeshBuilder`（48 顶点琥珀色边界线）、`RenderDrawPlan` 地图绘制（EditorBackground 天空 → WorldOrigin/Axes → MapBounds 地形+边界 → EntityFill → Gizmo，HasMap 时移除 EditorGrid）、`RenderProjection.Map` 携带 `MapRenderSnapshot` 传播链、shader kind=-14 地表 / -15 边界分支、F1 临时加载/卸载按钮、F2 绘制顺序修复（地表在天空之后）。
- F3 真机修复（本轮）：Lambert 方向语义与 D1 合同对齐——`sunDirection` = 指向光源方向（Z>0 朝上），`MapTerrainMeshBuilder.Brightness` 不再取反（修复前 toLight 指向地面下方，平面 ndl=-0.75→0，地表只剩环境光 0.35，视觉为灰蒙暗绿）；`WorldMapState` 默认 SunDirectionZ 同步 +0.75；`MapRenderSnapshot`/`MapDocumentWorldBridge` 注释同步合同语义。
- F4 可读性（本轮）：`EditorCameraFraming.FrameMapAllWithCenter` 地图取景 45° 斜上方俯视（Forward.Z=-0.707，完整容纳四角 + 安全边距）；`Brightness` 合成降为 `ambient×0.3×hemi + sun×0.85×ndl`（clamp [0,1]），避免全部顶点被 shader 钳制同色，缓丘受光/背光差 ≈0.086 肉眼可辨；scene.vert 天空顶部加深蓝 (0.45,0.56,0.74)、地平线更雾白 (0.88,0.90,0.94)，ShaderBytecode 由 glslc -O 重新生成并逐字比对。
- 测试（XuanYu.World.Tests/Map/ 与 /World/）：`MapTerrainBrightnessTests` 新增（Flat 亮度稳定∈[0.5,0.9]、缓丘明暗差>0.03、方向光贡献>0.05）；`WorldCameraFramingTests` 新增（45° 俯视 + 四角完整容纳）；`MapTerrainMeshBuilderTests` 亮度断言按 F4 合成公式更新。
- 治理：版本 v0.2.24.2-rz → v0.2.24.3-rz（五处同步：changelog/file-tree/UiVm.SceneDocument.cs/UiWin.axaml/run.bat）；无新增项目/依赖；ShaderBytecode 为生成物，行数 78（≤100 守卫口径通过）；第二排「加载测试地图/卸载地图」为 D4 临时验收入口，D5 移入右侧「地图编辑器」一级模块。
- F5 程序化天空（用户真机截图裁定：D4 视觉验收 FAIL 后追加，D4 保持 IN PROGRESS，15c9a0e 保留不回滚）：重建 Unity/Godot 风格程序化天空——天顶清晰蓝 (0.28,0.50,0.85) → 地平线浅蓝雾白 (0.78,0.87,0.96) → 地平线以下轻微大气泛光 (0.42,0.48,0.56)；上半球渐变改用 pow(dir.z, 0.55) 集中；新增最小太阳圆盘（方向与 D1 合同 sunDirection 一致，仅圆盘+微弱辉光，无耀斑/体积光）；ClearColor 改为浅蓝失败回退 (0.35,0.55,0.80)，不再用灰色掩盖天空失败；天空失败日志保留（ShaderModule/PipelineLayout/GraphicsPipelines 三处明确记录）；绘制仍为独立 Sky Pipeline（DepthTest/Write=Off、先于地表、只依赖相机旋转不依赖平移，Z-Up 读 dir.z）。
- 验证结果（F5 追加）：串行 build 12 项目 0 error / 1 warning（既有 xUnit2013）；Core 148/148、World 423/423、WarCore 22/22；arch-a-guard PASS；glslc 重新生成 ShaderBytecode（8293 词，79 行）逐字比对一致，新天空色与太阳常量全部在字节码中。
- D5-A 正式地图编辑器（用户真机裁定后追加，独立轮次）：右侧一级模块新增「地图编辑器」Tab（检查器之后，与检查器平级）——地图资产区（名称/路径/MapId/尺寸/状态：未加载/已保存/未保存）+ 新建/打开/保存/卸载/聚焦五命令；新建默认 TestBattlefield 2000×2000，复用 D2 MapDocumentOwner/MapStorageService（候选加载+原子保存）与 D3 WorldMapStateOwner，无第二套系统；打开失败保持原地图不变；第二排「加载测试地图/卸载地图」临时按钮已删除；基础地表/环境编辑组留 D5 后续补齐。新增 UiVm.MapEditor.cs（文档状态+命令，100 行）、UiWin.MapCommands.cs（.xymap 文件选择器）、MapEditorPanel.axaml（面板，Right.axaml 引用）；测试 UiMapEditorTests 4 项（新建入 World+Dirty、保存/打开 Round-trip、卸载清空、打开失败不污染）。
- 验证结果（D5-A 追加）：串行 build 12 项目 0 error；Core 148/148、World 430/430（含 D5-A 新增 4 项）、WarCore 22/22；arch-a-guard PASS（5+100 全过）；git diff --check PASS。
- D5-B 场景地图引用（独立轮次）：`.xyscene` schema v3 → v4，新增可选 `mapReference{mapId, assetPath}`（只存 mapId + 相对场景目录路径，不复制地图尺寸/地表/环境参数）；旧场景无 mapReference 正常打开；Validator 校验 mapId 合法 + 路径安全（非法拒绝），无效引用场景主体仍打开、地图编辑器显示「引用失效」+ 路径原因、不自动创建默认地图。新增 MapReference.cs、SceneDocumentValidator.MapReference.cs（校验拆分，Validator 保持 100 行）；SceneDocumentJson/Mapper/Snapshot 双向映射；UiVm.SceneDocumentMapRef.cs（保存附加引用 + 打开解析加载）；测试 SceneMapReferenceTests 4 项（保存携带/打开恢复/旧场景兼容/缺失失效）。
- 验证结果（D5-B 追加）：串行 build 12 项目 0 error；Core 148/148、World 434/434（含 D5-B 新增 4 项 + schema v4 断言更新 3 处）、WarCore 22/22；arch-a-guard PASS；git diff --check PASS。
- 状态：MAP-A-R1-D4 真机人工验收待用户执行（IPO 清单见报告）；验收通过后 D4 CLOSED，进入 MAP-A-R1-D5 正式地图编辑器与场景引用。

## v0.2.24.2-rz
MAP-A-R1-D3 World 地表能力（2026-08-02 18:24:41）
- 任务目标：把地图文档转化为 World 可查询的确定性地表能力——有限边界、唯一地表采样器、世界 X/Y → 地表 Z、加载/切换/卸载、最小渲染快照；本轮不渲染、不做 UI 与场景引用。
- 新增 `XuanYu.Core/Map/`：`MapSurfaceKind`（Flat/GentleHillsV1）、`MapSurfaceSampler`（唯一采样源：Flat 固定高度；GentleHillsV1 双正交正弦叠加，相位由 seed 固定派生，输出 [base−amp, base+amp]，纯算术确定性）、`MapRenderSnapshot`（供 D4 Render 消费的最小快照：尺寸+地表参数+MapId，卸载后 Empty）。
- 新增 `XuanYu.World/Map/`：`WorldMapState`（纯数据+有限边界判断+高度查询；世界 X 横向/Y 纵向/Z 高度，Z-Up 直写无映射层；闭区间边界，边界点属于地图；地图外不钳制不返回虚假零高度）、`WorldMapStateOwner`（当前地图状态：Load/Unload/Switch、TryGetSurfaceHeight(X,Y,out Z)、BuildRenderSnapshot）。
- 桥接：`XuanYu.Editor/MapDocument/MapDocumentWorldBridge.ToWorldState`（MapDocument → WorldMapState，字符串 kind → 枚举映射，对齐 SceneDocumentWorldBridge 模式）。
- 测试（XuanYu.World.Tests/Map/，新增 4 文件 32 项）：Flat/GentleHills 确定性（同坐标多次一致、200 点扫描）、幅度范围、seed/位置差异；边界闭区间（中心/四边/角在内，外 0.001 米拒绝）；Owner 加载/切换/卸载/快照清空不残留；桥接字段完整与端到端查询一致。
- 治理：版本 v0.2.24.1-rz → v0.2.24.2-rz（五处同步）；无新增项目/依赖；Core 新增纯数学 Map 类型（非 Scene/World/Picking/Gizmo 禁区）；World → Core 仅、Editor 桥接不反向依赖。
- 验证结果：串行 build 12 项目 0 error / 1 warning（既有 xUnit2013）；Core Tests 145/145；World Tests 411/411（含地图新增 32 项）；WarCore Tests 22/22；arch-a-guard PASS；glslc PASS；git diff --check PASS；5+100 全仓扫描 PASS（守卫口径与 wc 均 ≤100）。
- 状态：MAP-A-R1-D3 完成（无 UI/视口，验收以自动测试为准），等待批准后进入 MAP-A-R1-D4 有限地表、天空和光照。

## v0.2.24.1-rz
MAP-A-R1-D2 .xymap 地图存储闭环（2026-08-02 18:15:25）
- 任务目标：地图资产可靠创建、严格校验、保存、关闭并重新读取；本轮不渲染、不查询、不做 UI 与场景引用。
- 新增 `XuanYu.Editor/MapDocument/`：`MapDocument`（SchemaVersion/MapId/Name/SizeMeters/CoordinateSystem/Surface/Environment/LayerReferences）、`MapId`（32 位十六进制，创建后稳定）、`MapSize`/`MapCoordinateSystem`/`MapSurfaceDefinition`/`MapEnvironmentDefinition`/`MapVector3` 值对象、`MapDocumentValidator`（结构化 Issue 校验：尺寸 100–10000、坐标 Z-Up 米制零原点、地表仅 Flat/GentleHillsV1、环境参数有限非负、layerReferences 必须为空、未知类型拒绝）、`MapDocumentResult<T>`（对齐 SceneDocumentResult 模式）。
- 存储闭环：`MapJsonSerializer`（严格 JSON：字段大小写敏感 + 未知字段拒绝 + JsonPropertyName 固定 camelCase + JsonPropertyOrder 确定性输出 + UTF-8）、`MapJsonMapper`、`MapStorageService`（候选加载=解析→验证→成功才返回；原子保存=同目录临时文件→完整写入→File.Move 替换→失败清理并保留旧文件）、`MapDocumentOwner`（CurrentMap/CurrentPath/IsDirty 最小状态机：New→Dirty、Load→Clean、Modify→Dirty、Save→Clean、Unload→清空；失败不污染）。
- 路径合同：`Maps/<MapName>/map.xymap`；不存绝对路径；目录按需创建。D1 合同修正：`mapId` 口径更新为纯 32 位十六进制（无 `map_` 前缀，D2 §5.2 明确），docs/milestones/current/MAP-A/map-a-r1-d1-map-contracts.md 已同步。
- 测试（XuanYu.World.Tests/Map/，新增 9 文件）：MapId 格式/稳定性、尺寸边界与拒绝、坐标合同、地表/环境参数、图层引用空约束、JSON Round-trip 与确定性、大小写/未知字段/类型/损坏拒绝、候选加载失败不污染、原子保存与临时文件清理、Owner 状态链闭环。
- 治理：版本 v0.2.24.0-rz → v0.2.24.1-rz（五处同步）；无新增项目/依赖；不触碰 SceneDocument、WarCore、渲染与 UI。
- 验证结果：串行 build 12 项目 0 error / 1 warning（既有 xUnit2013）；Core Tests 145/145；World Tests 379/379（含地图新增 65 项）；WarCore Tests 22/22；arch-a-guard PASS；glslc PASS；git diff --check PASS；5+100 全仓扫描 PASS（守卫口径与 wc 均 ≤100）。
- 文件级验收（临时目录真实文件）：首次保存→重新读取 Round-trip 全字段一致（mapId/尺寸/坐标/地表/环境）；损坏 JSON 拒绝且不替换；保存失败无临时文件残留、不破坏旧文件。
- 状态：MAP-A-R1-D2 完成（无 UI/视口，验收以自动测试 + 真实文件检查为准），等待批准后进入 MAP-A-R1-D3 World 地表能力。

## v0.2.24.0-rz
MAP-A-R1-D1 地图合同冻结（2026-08-02 17:42:55）
- 任务目标：只读核查现有 SceneDocument / World Snapshot / 渲染地面 / 右侧模块结构后，冻结 `.xymap` 第一版 Schema 与 `.xyscene` mapReference 合同；本轮零产品代码，不重构旧代码。
- 坐标裁定（用户拍板，方案 B）：`.xymap` 语义与世界轴直写——X 横向（世界 X）、Z 高度（世界 Z=Up）、Y 纵向（世界 Y），与官方坐标合同 WORLD-A-R0（Z-Up、XY 水平）一致；不引入映射层；查询合同为「输入世界 X/Y 水平面坐标 → 输出地表 Z 高度」。
- 合同冻结（docs/milestones/current/MAP-A/map-a-r1-d1-map-contracts.md）：`.xymap` schemaVersion=1，mapId=`map_`+32hex，尺寸 100–10000 米，surface 仅 Flat/GentleHillsV1（确定性采样），environment 仅 ClearDayV1 + 方向光/环境光；保存路径 `Maps/<Name>/map.xymap`，原子替换，候选完整验证；`.xyscene` 升 v4 增可选 `mapReference{mapId, assetPath}`（项目相对路径，场景不复制地图数据），旧场景兼容，引用缺失明确报「引用失效」。
- 核查事实：无限灰网格=RenderDrawKind.EditorGrid（252 顶点，scene.vert gridVertex，±10 米 21×21 线，z=0 平面）；天空=EditorBackground+深度不写第二管线（WORLD-D 成品，直接复用）；光照=shader 硬编码固定方向光+半球环境光；右侧模块 Right.axaml=检查器/调试/偏好/模式四 Tab（MAP-A 收为检查器+地图编辑器）；全库无任何地图类型；版本源五处一致。
- 治理：新里程碑 MAP-A（模块 24），新分支 feat/MAP-A-map；版本 v0.2.23.0-rz → v0.2.24.0-rz（五处同步）；基线 HEAD cbb694b = origin tip，ahead/behind 0/0；已知偏差 untracked `IDEA.md` 与残留 `XuanYu.Editor.Avalonia/` bin 目录未处理。
- 状态：MAP-A-R1-D1 合同冻结完成，等待批准后进入 D1 域类型编码（MapId/MapDocument/MapSurfaceDefinition/字段验证）。
