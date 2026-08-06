# 玄域引擎 旧 UI 审计矩阵（ARCH-UI-SPEC-R1-D0）

> **治理编号**：ARCH-UI-SPEC-R1-D0
> **生成日期**：2026-08-05
> **审计基线**：`feat/MAP-A-map` HEAD=0380192（v0.2.24.36-rz 落库后），工作区 clean
> **依据**：《玄域引擎 UI 规范 1.0》（正式规范，`docs/ui/玄域引擎_UI规范_1.0.md`，UI Spec 1.0，D1 冻结）+ 治理实施计划第三章《首批冻结参数》。D0 审计时依据的讨论初稿（`docs/governance/ui-spec.md`）已由 D1 审订为正式规范，本矩阵违规项以正式规范章节为整改目标。
> **性质**：D0 只审计、不整改。违规项清零属 D6，整改归属轮次已在每项标注。
> **方法**：逐个读取全部 16 个 `.axaml` 真实内容 + 5 处 code-behind 视觉源，对照冻结参数逐项判定；不凭文件名猜测。

---

## 一、界面清单总览

| # | 界面 | 路径 | 主密度 | 现有字号 | 现有控件高度 | 滚动结构 | 图标来源 | 违规项 | 整改归属 |
|---|---|---|---|---|---|---|---|---|---|
| 1 | 主窗口 | `Win/UiWin.axaml` | — | 继承全局 | — | — | — | 3 | D3 |
| 2 | 全局样式 | `Ui.axaml` | — | 12 默认 | 30/34 | — | — | 14 | D2/D3 |
| 3 | 应用引导 | `Bootstrap/App.axaml` | — | — | — | — | — | 0（浅色主题显式 ✓） | — |
| 4 | 主布局 | `Root/UiRoot.axaml` | — | — | — | 主区+日志区 | — | 4 | D3 |
| 5 | 顶部菜单/工具条 | `Top/Top.axaml` | 紧凑 | 继承 | 32 | — | EditorIcons | 6 | D3 |
| 6 | 左侧树（项目/层级） | `Left/Left.axaml` + `Left.Styles.axaml` | 紧凑 | 15/13 | 28 | 单列表 | EditorIcons | 8 | D3 |
| 7 | 右侧工作面板 | `Right/Right.axaml` | 标准 | 15/12 | 30 | 调试页独立 | EditorIcons | 8 | D4 |
| 8 | 地图编辑器 | `Right/MapEditorPanel.axaml` | 标准 | 14/13/12/10 | 32 | 每页独立 | EditorIcons | 5 | D4 |
| 9 | 图层面板 | `Right/LayerPanel.axaml` | 紧凑 | 13/12/10 | 25/32 | 整页滚动（F1 合同 ✓） | EditorIcons | 3 | D4 |
| 10 | 图层属性 | `Right/LayerInspectorPanel.axaml` | 标准 | 12（值缺省） | — | — | — | 2 | D4 |
| 11 | 底部日志 | `Foot/Foot.axaml` | 紧凑 | 16/12 | 28 | 虚拟化+自动跟随 ✓ | EditorIcons | 5 | D5 |
| 12 | 日志详情 | `Foot/LogDetailPanel.axaml` | 标准 | 12 | 42 | 独立详情滚动 | — | 3 | D5 |
| 13 | 渲染视口 | `Viewport/Vulkan/VulkanViewport.axaml` | — | — | — | — | — | 2 | D3 |
| 14 | 主区装配 | `Main/Main.axaml` | — | — | — | — | — | 0 | — |
| 15 | 图标资产 | `Icons/EditorIcons.axaml` | — | — | — | — | 35 个 StreamGeometry | 2 | D2 |
| 16 | 弹窗/状态 code-behind | `Win/UiWin.Dialogs.cs`、`UnsavedDialog.cs`、`UiVm.DocumentStatus.cs`、`LogEntry.cs`、`TreeGuide.cs` | — | — | — | — | — | 6 | D5 |

**合计**：16 界面 + 5 处 code-behind 视觉源，**违规项 71**（含需 Token 化/登记项）。全部为零后进入 D6 清零验收。

---

## 二、违规明细（编号 W01 起，D6 按编号逐项清零）

### 2.1 全局样式 `Ui.axaml`

| 编号 | 类别 | 位置 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|---|
| W01 | 字号 | `Window` 样式 | 未设 FontSize，全应用正文落默认 12 | 正文 Body=13；字号必须 Token 化 | D2/D3 |
| W02 | 颜色 | `Window` Foreground | `#172033` | Text.Primary `#243744` | D2 |
| W03 | 字体 | `Window` FontFamily | `Microsoft YaHei UI, Segoe UI, Inter` | 回退链 `Microsoft YaHei UI → Segoe UI → Noto Sans CJK SC → 系统无衬线`（无 Inter） | D2 |
| W04 | 阴影 | `Border.panel` BoxShadow | `0 14 30 0 #160f172a`（偏移14/模糊30/25%黑） | 普通面板禁阴影；悬浮层统一 `垂直4 / 模糊12 / 约14%黑` | D2/D3 |
| W05 | 颜色 | `Border.panel` | bg `#fbfcff` / 边框 `#d9e0ec` | Bg.Panel `#F8FAFB` / Border.Default `#D5DEE4` | D2 |
| W06 | 圆角 | `Border.pill` | `5` | 只允许 3/6/10 | D2 |
| W07 | 间距 | `Button` | Padding `10,7`、MinWidth `52` | 内边距档位 6×2 / 8×4 / 12×6；宽度等级 64/96/128/160/240 | D2 |
| W08 | 颜色 | `Button` / `:pointerover` | bg `#eef3fa` / fg `#26324a`；hover `#e4edf8`/`#9fb5d6` | Bg.Control `#FFFFFF` / Text.Primary `#243744`；Hover.Bg `#EEF4F6` | D2 |
| W09 | 高度 | `TabItem.sideTab` MinHeight | `30` | 控件高度只允许 24/28/32 | D2 |
| W10 | 圆角 | `TabItem.sideTab` | `5` | 只允许 3/6/10 | D2 |
| W11 | 颜色 | `sideTab:selected` | fg `#185aa6` / bg `#edf4ff` / 边 `#8cb2e2` | Accent `#326F8A` / Selection.Bg `#E5F0F4`（旧蓝强调系整体替换） | D2 |
| W12 | 高度 | `ListBoxItem` MinHeight | `34` | 只允许 24/28/32 | D2 |
| W13 | 间距 | `ListBoxItem` Padding | `10,7` | 内边距档位 | D2 |
| W14 | 字号/颜色 | ~~`TextBlock.section` 12 SemiBold / `#40516f`~~ → Section 14 + Color.Text.Primary Token（D4 已清零，规范 §3.3 定稿） | 分组标题应为 Section=14 或登记为 Label=12（F3 合同曾定 13 半粗，需审订定夺）；色 `#40516f` 需 Token 化 | D1 审订（D4 清零） |

### 2.2 主窗口 `Win/UiWin.axaml`

| 编号 | 类别 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|
| W15 | 窗口 | ~~`1400×820`~~ → `1360×820`（D3 已清零） | 推荐初始 `1360×820`（≤工作区可用范围） | D3 |
| W16 | 窗口 | ~~MinWidth `1100` / MinHeight `720`~~ → `1024×640`（D3 已清零） | 应用最小窗口 `1024×640` | D3 |
| W17 | 颜色 | ~~`#e9eef5`~~ → `Color.Bg.Application`（D3 已清零） | Bg.Application `#F3F6F8` | D2 |

### 2.3 主布局 `Root/UiRoot.axaml`

| 编号 | 类别 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|
| W18 | 布局 | ~~RootGrid MinWidth `980`（+Margin 24）~~ → `1012`（+Margin 12，D3 已清零） | 应用最小窗口 `1024×640`（对齐） | D3 |
| W19 | 布局 | ~~左列 MinWidth `200`~~ → `220`（D3 已清零） | 左侧层级树最小 `220` | D3 |
| W20 | 布局 | ~~右列 MinWidth `260`~~ → `300`（D3 已清零） | 右侧工作面板最小 `300` | D3 |
| W21 | 布局 | ~~日志行 MinHeight `32`~~ → 折叠态 32 保留、展开态 120~420（D3 登记对齐） | 底部日志最小 `120`（F4 折叠态 32 + ClampLogRow code-behind 展开态 420；需与规范对齐并登记） | D3 |

> 合规项：右列标准宽度 `340` ✓；主区 MinHeight `320` = 视口最小 480×320 高度维 ✓；视口列 MinWidth `480`（D3 对齐）✓；分隔条 6 DIP ✓；视口 1 DIP 浅灰边框（D3 起 Token `Color.Border.Default`）✓；分隔条色（D3 起 `Color.Border.Strong`/`Color.Hover.Bg`）✓。

### 2.4 顶部 `Top/Top.axaml`

| 编号 | 类别 | 位置 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|---|
| W22 | 圆角 | `commandRail` | `9` | 只允许 3/6/10（10 仅大容器） | D2 |
| W23 | 圆角 | `statePill` | `7` | 只允许 3/6/10 | D2 |
| W24 | 圆角 | `cmdBtn`/`toolBtn` | `4` | 只允许 3/6/10 | D2 |
| W25 | 间距 | `commandRail`/`MenuItem`/`cmdBtn`/`toolBtn` Padding | `10,5`/`11,5`/`7,4` | 内边距档位 6×2 / 8×4 / 12×6（工具栏按钮允许组件级 Token） | D2 |
| W26 | 颜色 | `toolBtn:checked`、`cmdBtn:pressed`、MenuItem | `#eef5ff`/`#94b9e8`/`#185aa6`、`#dfeaf8`、fg `#2f3d52` | Accent `#326F8A` / Hover.Bg `#EEF4F6`（旧蓝系替换） | D2 |
| W27 | 图标 | `Path.topIcon` StrokeThickness | `1.6` | 线性图标标准笔画 `1.5` | D2 |
| W28 | 菜单 | 顶层菜单 | 仅「文件/添加」 | 标准结构「文件→编辑→视图→地图→工具→窗口→帮助」只在功能存在时显示；当前功能集下不凑空菜单（合规），D3 复核命名一致性与右键菜单 | D3（D3 复核结论：合规——菜单/按钮/右键命名一致（新建/打开/保存/另存为/添加立方体），右键只放对象相关操作且危险操作已用 Separator 分组，无代码改动） |

> 合规项：工具按钮 32×32 ✓；图标 16×16 ✓；无文字图标按钮均带文字标签 ✓（无需 Tooltip 兜底项）。

### 2.5 左侧树 `Left/Left.axaml` + `Left.Styles.axaml`

| 编号 | 类别 | 位置 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|---|
| W29 | 字号 | `leftTab` | `15` | 顶层页签 `13` | D3 |
| W30 | 颜色 | `leftTab:selected` | bg `#edf4ff` / fg `#185aa6` / 边 `#8cb2e2` | Selection.Bg `#E5F0F4` / Accent `#326F8A` | D2 |
| W31 | 圆角 | `leftTab` / `treeRow` | `5` | 只允许 3/6/10 | D2 |
| W32 | 图标 | `Path.treeIcon` StrokeThickness | `2.2`（全树图标） | 标准笔画 `1.5` | D2 |
| W33 | 颜色 | `treeIcon` Stroke | `#2F80C9` | Accent 系 `#326F8A`（或登记对象色） | D2 |
| W34 | 颜色 | `treeRow.selected` | `#e7f1ff` | Selection.Bg `#E5F0F4` | D2 |
| W35 | 颜色 | `selectedText` | `#165ca8` | Accent `#326F8A` | D2 |
| W36 | 其他 | 层级菜单「删除」 | fg `#9b2f2f` | Danger `#A53F43`；危险操作需与普通操作分组 | D2/D5 |

> 合规项：树行 28 ✓；树文本 13 ✓；图标 16×16 ✓；搜索框 24 ✓；重命名内联编辑 ✓。

### 2.6 右侧工作面板 `Right/Right.axaml`

| 编号 | 类别 | 位置 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|---|
| W37 | 字号 | ~~`panelTitle` 15~~ → Title 16 Token（D4 已清零） | Title=16 或 Section=14（不得出现 15） | D2（D4 清零） |
| W38 | 字号 | ~~`value` 无 FontSize（落默认 12）~~ → Body 13 Token（D4 已清零） | 字段值 Body=13 | D2（D4 清零） |
| W39 | 高度 | ~~`kvRow` MinHeight `30`~~ → 行高 28（D4 已清零） | 只允许 24/28/32 | D2（D4 清零） |
| W40 | 图标 | ~~`panelIcon` StrokeThickness `1.6`~~ → 1.5 Token（D4 已清零） | 标准笔画 `1.5` | D2（D4 清零） |
| W41 | 颜色 | ~~`key`/`value`/`panelTitle`/`emptyTitle` 旧色~~ → Text.Secondary/Primary Token（D4 已清零） | Text.Secondary `#5D6F7C` / Text.Primary `#243744` | D2（D4 清零） |
| W42 | 间距 | ~~`infoPanel` Padding `10`~~ → 分组去卡片、内容 Padding 8（D4 已清零） | 内边距档位 | D2（D4 清零） |
| W43 | 页签 | `sideTabs`（检查器/地图编辑器/调试） | 无单行溢出/横向滚动/箭头/全部页签入口/管理模式 | 顶层页签强制合同 15 条全部未实现 | D3（D3 已清零，2026-08-06 用户真机验收通过：单行/横向滚动/渐隐/当前可见/一次性提示/全部页签入口/滚轮路由/禁拖动已实现；**经用户批准（D3-EX-01），当前组件采用滚轮、渐隐、自动显露和全部页签入口，不要求左右箭头**；关闭/排序/偏好因架构无关闭能力留待正式规范具备后） |
| W44 | 布局 | ~~调试页 Grid 标签列 `70`~~ → `96`（D4 已清零） | 检查器标签列默认 `96` | D4 |

> 合规项：空选择状态 ✓（图标+标题+说明，单一主入口）；调试页独立滚动 ✓；字段标签 12 ✓。

### 2.7 地图编辑器 `Right/MapEditorPanel.axaml`

| 编号 | 类别 | 位置 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|---|
| W45 | 颜色 | ~~`layerSubTab:selected` 旧蓝系~~ → Accent/Selection.Bg Token（D4 已清零） | Accent `#326F8A` / Selection.Bg `#E5F0F4` | D2（D4 清零） |
| W46 | 圆角 | ~~`layerSubTab` `5`~~ → Radius.Standard 6 Token（D4 已清零） | 只允许 3/6/10 | D2（D4 清零） |
| W47 | 颜色 | ~~`MapEditError` `#C0392B`~~ → Color.Error Token（D4 已清零） | Error `#B14A4A` | D2（D4 清零） |
| W48 | 布局 | ~~地图资产/属性 Grid 标签列 `90`~~ → 资产摘要 72（组件级例外，补充裁决）/属性表单 96（D4 已清零） | 检查器标签列默认 `96` | D4 |
| W49 | 间距 | ~~`layerSubTab` Padding `9,4`~~ → `10,4`（页签组件级，D4 已清零） | 内边距档位 | D2（D4 清零） |

> 合规项：二级页签 14 ✓；字段值 13 ✓；按钮 12 ✓；类型标签 10 ✓；禁用按钮带 Tooltip ✓；错误信息字段附近展示 ✓。

### 2.8 图层面板 `Right/LayerPanel.axaml`

| 编号 | 类别 | 位置 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|---|
| W50 | 高度 | ~~`layerTool` MinHeight `25`~~ → 24 紧凑档 Token（D4 已清零） | 只允许 24/28/32 | D2（D4 清零） |
| W51 | 圆角 | ~~`layerSwitch`/`layerLockSwitch`/`activeMark`/`dropLine` `4`/`4`/`1.5`/`1`~~ → 开关 3 Token（D4 已清零）；activeMark 1.5 / dropLine 1 保留登记组件例外 | 只允许 3/6/10（装饰标记可登记例外或并入组件 Token） | D2（D4 清零；2 条例外保留基线） |
| W52 | 颜色 | ~~`activeMark` 等旧色~~ → Layer.* / Color.Accent Token（D4 已清零） | 需 Token 化（选中语义）或登记例外 | D2（D4 清零） |

> 合规项（D4-F3 合同已落地，D4 升级）：状态热区 26×24 ✓；可见/隐藏/锁定/解锁形态+颜色双表达 ✓；区域/系统类型标签双色+文字 ✓；拖动插入线 2 DIP `Layer.DropLine #5B8DB8` ✓；行高 32 ✓；图标 16×16 笔画 1.5 ✓（D4 升级）；行内操作悬停/选中显示 ✓；拖动事务式（DragDrop.cs）✓；选中行显式样式（Selection.Bg + 状态色保留）✓（D4 新增）。

### 2.9 图层属性 `Right/LayerInspectorPanel.axaml`

| 编号 | 类别 | 位置 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|---|
| W53 | 布局 | ~~Grid 标签列 `70`~~ → `96`（D4 已清零） | 检查器标签列默认 `96` | D4 |
| W54 | 字号 | ~~`TextBlock.value` 无 FontSize（落默认 12）~~ → Body 13 Token（D4 已清零） | 字段值 Body=13 | D2（D4 清零） |

### 2.10 底部日志 `Foot/Foot.axaml`

| 编号 | 类别 | 位置 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|---|
| W55 | 字号 | 空状态标题 | `16` | 需 Token 化（Title=16 语义可映射，不得裸写） | D2 |
| W56 | 颜色 | `logFilter.selected` | bg `#edf4ff` / fg `#185aa6` | Selection.Bg `#E5F0F4` / Accent `#326F8A` | D2 |
| W57 | 圆角 | `logSummary`/`logFilter`/日志行 Accent 圆点 | `4`/`4`/`2` | 只允许 3/6/10（圆点可登记组件例外） | D2 |
| W58 | 颜色 | `logMono`/`logHead`/选中行/RepeatText | `#27354a`/`#64748b`/`#eaf3ff`/`#7a5a19` | 语义色 Token 化（重复计数属 Warning 系） | D2 |
| W59 | 魔法值 | 日志列宽 `4,72,56,72,92,*,82` | 固定列宽 | 建立组件级 Token 或登记；长文本省略 ✓ | D2/D5 |

> 合规项：日志行 28 ✓；虚拟化 ✓；自动跟随策略已实现（LogAutoScroll）✓；过滤按钮 ✓；搜索框 ✓；优先级通知（ChooseLatest）✓。

### 2.11 日志详情 `Foot/LogDetailPanel.axaml`

| 编号 | 类别 | 位置 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|---|
| W60 | 高度 | `detailBody` MinHeight | `42` | 只允许 24/28/32 | D2 |
| W61 | 圆角 | 详情信息卡 | `5` | 只允许 3/6/10 | D2 |
| W62 | 颜色 | 详情卡 `#edf4ff` 等 | 旧蓝系 | Selection/信息块语义 Token | D2 |

### 2.12 渲染视口 `Viewport/Vulkan/VulkanViewport.axaml`

| 编号 | 类别 | 位置 | 现值 | 冻结参数 | 归属 |
|---|---|---|---|---|---|
| W63 | 颜色 | FallbackLayer（初始化占位） | `#E8EEF5`/`#4A5A70`/`#6B7688` | Token 化（面板/次要文字语义） | D2 |
| W64 | 间距 | FallbackLayer Padding | `18` | 宽松档 16/24 或登记 | D2 |

> 合规项：视口 1 DIP `#C9D2DC` 浅灰分隔 ✓（F3 合同）；无深色粗框 ✓。

### 2.13 图标资产 `Icons/EditorIcons.axaml`

| 编号 | 类别 | 说明 | 归属 |
|---|---|---|---|
| W65 | 笔画统一 | 引用样式笔画 1.5（layerIcon）/1.6（topIcon/panelIcon）/2.2（treeIcon）三套并存 | D2 |
| W66 | 视觉中心 | 无视觉中心校正证据；需 D2 图标合同测试（几何/视觉中心采样） | D2 |

> 合规项：35 个图标全部线性 StreamGeometry ✓；无 Emoji/Unicode/系统字体符号 ✓；单一来源文件 ✓；24 网格坐标体系一致 ✓；状态图标（可见/隐藏/锁定/解锁）形态+颜色双表达 ✓。

### 2.14 弹窗与状态 code-behind（D5 整改，D0 登记）

| 编号 | 位置 | 现值 | 冻结参数/要求 | 归属 |
|---|---|---|---|---|
| W67 | `Win/UiWin.Dialogs.cs` | 错误/警告弹窗底色 `#fdeeee`/`#fff7df`、边框 `#e2aaaa`/`#e7c66d`、文字 `#a43f3f`/`#8a6417` | Error/Warning 语义 Token 化 | D2/D5 |
| W68 | `Win/UiWin.Dialogs.cs` | 主按钮 `#e9f2ff`/`#94b9e8`/`#185aa6`（旧蓝） | Accent `#326F8A`；弹窗按钮顺序「取消→次要→主要」、危险按钮非默认焦点、Esc 取消 | D5 |
| W69 | `Win/UiWin.UnsavedDialog.cs` | `#243447`/`#64748b`/`#fbfdff` | 语义 Token 化；丢弃未保存内容必须确认 ✓（已存在） | D2/D5 |
| W70 | `Vm/Logging/LogEntry.cs` | 级别色 Error `#c75b5b`/Warning `#d89b32`/Info `#4f7fb8`/Debug `#6b7a90`/Trace `#8b96a8` | 状态语义色 Token 化（浅底行首圆点用）或登记为数据可视化允许清单 | D2 |
| W71 | `Vm/Scene/UiVm.DocumentStatus.cs` | 文档状态三态 × 3 色（未保存 `#fff7df/#e7c66d/#8a6417`、失败 `#fdeeee/#e2aaaa/#a43f3f`、成功 `#eef7f1/#c9e3d0/#1f7a4d`） | Warning/Error/Success 语义 Token 化 | D2 |
| — | `TreeGuide.cs` | 树引导线 `#C7D7EA` | 进入 D2 允许清单（领域视觉/装饰线）或 Token 化 | D2 |

---

## 三、结构性缺口（非单点违规，需整组能力）

| 编号 | 缺口 | 现状 | 冻结要求 | 归属 |
|---|---|---|---|---|
| G01 | 顶层页签溢出管理 | Right sideTabs 与 Left leftTabs 均为普通 TabControl，无横向溢出/箭头/渐隐/全部入口/管理模式 | 15 条强制合同（单行/横向滚动/箭头/渐隐/当前页签可见/一次性提示/全部入口/禁拖动/管理模式/偏好保存） | D3（D3 已清零，2026-08-06 用户真机验收通过：Right sideTabs 溢出系统已实现，见 W43；**经用户批准（D3-EX-01）当前组件不要求左右箭头**；Left leftTabs 仅 2 页签暂不溢出，纳入 D6 复核） |
| G02 | 键盘焦点视觉 | 全仓库无 Focus 样式定义（无 FocusAdorner/焦点边框），焦点态不可辨识 | 焦点框 2 DIP 外偏移 1；不得与选中态混淆；不得被裁切 | D3/D6 |
| G03 | 检查器响应式双模式 | 检查器字段为 ListBox 渲染（InspectorFields），无「宽=左右、窄=上下」切换逻辑 | 切换阈值 内容宽 <360 DIP 整组切换；标签列 96；字段最小 128 | D4（D4 已清零并收窄：检查器只读键值行**始终单行双列**（§7.1.1，D4-F1 用户复验裁决），不参与 360 切换；可编辑表单行（真实输入控件）由 EditableFormLayoutModel 统一 360 阈值） |
| G04 | 面板紧凑/折叠 | 无紧凑模式切换、无面板折叠与恢复入口 | 内容宽 <320 进紧凑；折叠次要区/隐藏低优先级面板且保留恢复入口 | D3（D4 部分落地；**D4-F1 纠偏后**：MapEditorLayoutModel 恢复——<320 紧凑密度控制根留白 12/8、分组间距 12/8、字段行距 6/4；只读摘要不参与切换；面板折叠/恢复入口留后续轮） |
| G05 | 空状态体系 | 检查器/日志有局部空状态，无「筛选无结果/权限不足/加载失败」区分 | 完整空状态体系（原因说明 + 单一主入口） | D5 |
| G06 | 屏幕阅读器/可识别名称 | 未发现 AutomationProperties 使用 | 所有交互控件可识别名称；基础结构 D6 建立，高级项登记待办 | D6 |
| G07 | 减少动画偏好 | 无动画自定义（FluentTheme 默认），未接入系统 Reduce Motion | 支持系统「减少动画」设置 | D6 |
| G08 | 菜单结构一致性 | 顶部仅「文件/添加」；上下文菜单（层级树）与顶部菜单命名/分组未统一审计 | 同一功能在菜单/按钮/Tooltip/日志/文档名称一致；右键菜单只放对象相关操作 | D5 |

---

## 四、按整改轮次汇总

| 轮次 | 范围 | 违规项 |
|---|---|---|
| D2 | Token 基础设施（颜色/字号/间距/圆角/笔画/阴影语义化） | W01-W14、W17、W22-W27、W30-W35、W37-W42、W45-W47、W49-W52、W54-W58、W60-W66、W67/W69-W71（约 45 项） |
| D3 | 主窗口/布局/页签/滚动/焦点 | W15、W16、W18-W21、W28、W43、G01（**D3 已清零**；W29/W36（部分）/G02/G04 本轮范围外保留，见各条目） |
| D4 | 右侧面板/检查器/图层 | W44、W48、W53、G03（**D4 已清零**）+ W37-W42、W45-W52、W54、W14（D4 文件内 Token 迁移清零）——**D4 已 COMPLETE**（2026-08-06 用户真机裁决，F1-1~F1-9 通过） |
| D5 | 状态/表单/菜单/弹窗/日志 | W36、W59、W68、G05、G08（**D5-A1 用户真机验收通过，D5 COMPLETE**：地图状态四态（未落盘/未保存/已保存/有未保存修改）+ 全刷新点；弹窗去内部编号正式文案；按钮状态/字段校验/危险确认/通知/空态/回到底部均通过；D5-DEFER-01 地图持久化暂缓，独立专项，不归 D6；G02 焦点系统完整化/自动消失策略留 D6） |
| D6 | DPI/键盘/性能/全量清零 | 全部剩余项 + G06、G07 + 审计矩阵清零复核 |

> 注：D2 与 D3-D6 存在依赖（先 Token 后整改），违规项按「数值语义化」与「布局改造」双轨归属，实际执行时以轮次计划为准。

---

## 五、合规确认清单（本次审计判为合规的事实）

1. 浅色主题显式声明（`App.axaml` RequestedThemeVariant=Light）✓
2. 全部尺寸使用 DIP 逻辑单位，无物理像素手工补偿 ✓
3. 图标全部为线性 StreamGeometry，无 Emoji/Unicode/系统字体符号 ✓
4. 图层状态（可见/隐藏/锁定/解锁）形态 + 颜色双表达 ✓
5. 区域/系统类型标签双色 + 文字区分，不依赖单一颜色 ✓
6. 拖动插入线 2 DIP、事务式拖拽（预览/提交/取消/单历史/Dirty 规则）✓
7. 日志虚拟化 + 自动跟随 + 优先级通知（错误/警告 > 操作 > 技术日志）✓
8. 禁用控件提供 Tooltip 或禁用原因说明 ✓（打开地图/保存地图、图层按钮）
9. 视口贴合 1 DIP 浅灰分隔，无深色粗框 ✓
10. 弹窗分级确认已存在（危险/未保存对话框 code-behind）✓

---

## 六、验证方式标注（ARCH-UI-SPEC-R1-D2 补充；D2-F1 细化；D2-F2 最终）

每项 W/G/K 的验证方式如下；D2-F2 已建立最终自动门禁（`XuanYu.World.Tests/UiTokens/`），D3~D6 按轮次将自动项清零。

### 6.1 自动门禁（已接入正式测试，159 条稳定定位基线指纹，Unknown=0；D3 清除 4 条、D4 清除 67 条）

违规值已登记**稳定定位基线**（相对路径 + 稳定定位 + 规则类型 + 真实属性名 + 规范化值 + 允许次数）：

- AXAML 稳定定位（父链 v3）：Setter → `Style:<Selector>`；命名元素 → `Name:<x:Name>`；匿名元素 → `Path:<最近命名祖先|ROOT>/<父类型链>/<类型>:<同父序号>`（如 `Path:Name:LogList/ListBox/DataTemplate/Grid/Border:1`）；
- code-behind 稳定定位：完整类型名 + 方法/属性/字段名（补齐 async/无修饰符/const 字段/显式接口实现），**Locator=Unknown 条目 = 0**；
- 颜色违规记录真实属性名（Background/Foreground/BorderBrush/Fill/Stroke/Color 等），不再统一记 `Color`；
- 匹配 Path + Locator + Kind + Property + Value 全部参与——同 Style 属性换位（Foreground→Background→BorderBrush）、匿名同类型控件换位、不同父级同类型换位均使测试失败；空白/注释/无关属性变化不造成漂移；基线不自动增长。

| 规则类型 | 覆盖 W 项 | 基线数量 |
|---|---|---|
| AXAML 十六进制色值（HexColor，真实属性名） | W02/W05/W08/W10/W11/W14/W17/W21/W22/W26/W30/W33/W34/W35/W36/W41/W45/W47/W51/W52/W53/W56/W58/W61/W62/W63/W71-GEN | 124 |
| code-behind 色值（CsHexColor：Hex/Colors/ColorAPI/Brush/Uint 五类写法） | W67/W69/W70/W71/W71-ALLOW + ALLOW-RENDER/ALLOW-WIN32 | 41 |
| 字号（FontSize） | W29/W37 等（按 Locator 细分） | 19 |
| 圆角（CornerRadius） | W10/W22/W31/W46/W51/W57/W61 等 | 27 |
| 控件高度（ControlHeight） | W09/W12/W50/W60 等（保留真实属性名 Height/MinHeight） | 14 |
| 阴影（BoxShadow） | W04 | 1 |
| 图标笔画（StrokeThickness） | W27/W32/W40 等 | 4 |

### 6.2 真机验收（不可自动化或需视觉判断）

- **W 类**：W01（全局字号默认）、W15/W16/W18~W21（窗口与布局阈值）、W38/W54（字段值字号缺失）、W55（空状态标题 Token 化）、W66（图标视觉中心）、W39（kvRow 30 属 Border 复合容器高度，暂不可稳定自动化）；
- **G01~G08**：全部为结构性/交互能力缺口，真机验收 + D3~D5 按轮次以自动化结构测试逐步覆盖（当前不伪装已自动化）；
- **K01~K07**：全部为真机已知问题，整改轮复验。

### 6.3 自动 + 真机（混合）

- 颜色对比度与状态辨识（W 色值类自动拦截 + 真机判断可读性）；
- 页签/滚动/拖拽行为（自动化结构合同测试 + 真机手感）。

### 6.4 允许清单（按路径+规则类型+API 模式+原因登记，D2-F2）

- `TreeGuide.cs` Render：`#C7D7EA` / `Color.Parse` / `new SolidColorBrush` —— 树引导线渲染（D4 允许清单组件，色值 = `Tree.Guide` Token）；
- `Win32ViewportHost.cs`：`0x` 常量 —— Win32 窗口样式宏（WS_CHILD 等），非颜色；
- 禁止整文件/整目录/整扩展名放行。

> 说明：W 编号为主关联项，具体色值归属以审计矩阵第二节明细为准；基线数据由脚本按分析器同逻辑自动生成自真实 UI 源码现状值（D2-F2，父链定位 v3 + cs 八类规则），整改时同步删除对应条目。

---

*文档结束（ARCH-UI-SPEC-R1-D0 输出物之一；整改清零状态以 D6 报告为准）*
