# ARCH-UI-SPEC-R1 受控待办登记

> 本文件登记 UI 规范待办与治理状态（MAP-A-R2-D4-F3 裁定登记）。
> ARCH-UI-SPEC-R1 已立项（2026-08-05 D1 正式规范冻结）；治理期间 UI 收敛以
> `docs/ui/玄域引擎_UI规范_1.0.md`（UI Spec 1.0）为唯一执行依据。

## ARCH-UI-SPEC-R1：玄域引擎 UI 规范 1.0

- **状态**：**治理中**（2026-08-06 更新：D0/D1/D2/D3/D4/D5 COMPLETE；当前阶段 **ARCH-UI-SPEC-R1-D6 READY FOR USER ACCEPTANCE**。**D5-DEFER-01：地图「保存并新建」暂缓**——归属未来独立的地图持久化专项（不归入 D6）。专项必须补：保存成功后才新建；保存失败不新建；取消路径选择不新建；保存期间防重复提交；保存成功后更新路径和状态；写盘失败时保留地图、图层和历史。）

**D5 控件状态/表单/弹窗/通知/空状态/日志治理 COMPLETE**（2026-08-06 用户真机验收 D5-A1 PASS；二次审查两项硬阻塞已按用户方案修复：未保存判断 = IsDirty || 待提交表单（默认地图基线 MarkBaseline 修正）；输入阶段真实校验 ValidateMapFieldOnInput（错误不消失/输入中态/明显超界）；「保存并新建」因无真实地图持久化停止上报并登记 D5-DEFER-01；债务基线 122。）
- **范围**：字体、字重、颜色、间距、控件、图标、布局、导航、日志通知、
  状态语义、拖拽、空状态、最小点击热区、最小窗口、自适应、Tooltip、
  菜单与可访问性规范
- **正式规范**：`docs/ui/玄域引擎_UI规范_1.0.md`（UI Spec 1.0，唯一 UI 规范事实源）
- **当前追踪**：
  - 旧 UI 审计矩阵：违规 W01～W71、结构性缺口 G01～G08（清零属 D6）；
  - 真机基线：已知问题 K01～K07（整改轮复验）；
  - **Token 基础设施（D2 已建）**：`XuanYu.Editor.UI/Design/`（UiTokens.Fonts / Colors.Core / Colors.Components / Spacing / Controls / Icons / Motion + UiTokens 聚合入口，8 文件）；**唯一机器事实源 `Design/UiTokenManifest.json`**（112 条，**D2-F2 起全部 Frozen / 0 PendingReview**，含 §12.2.1 图层 Token 与 §15.3 动效默认 Token 的用户正式裁决值），XAML 由 `scripts/generate-ui-tokens.py` 确定性生成（生成文件禁手改）；
  - **自动化门禁（D2-F2 已建，D3/D4 完善）**：Token 合同测试（Manifest↔XAML 双向）+ 源码违规分析器（AXAML 父链定位 v3 + cs 八类颜色写法；**D3 起 `{StaticResource}` 正式 Token 引用豁免**）+ 旧债务细粒度基线（`XuanYu.World.Tests/UiTokens/`，**159 条指纹**：路径+稳定定位 Locator+规则类型+真实属性名+值+允许次数，**Unknown=0**，映射审计矩阵 W 编号）；分析器允许字号/圆角/高度/笔画从 Manifest 读取；
  - 旧债务基线规则：已知债务允许（父链细粒度定位）、新增债务（含同 Style 属性换位/匿名控件换位/父级换位）使正式测试失败、债务减少允许、基线不自动增长（增加基线项必须独立治理批准）；D6 清零后切换零容忍；
  - **D3 完成（2026-08-06，D3-A1 用户真机验收通过 → COMPLETE）**：主窗口 1360×820 / 最小 1024×640；左 220 / 右 300 / 视口 480×320 / 日志 120~420；顶层页签单行溢出系统；滚动边界树结构隔离；**基线 230 → 226（W17×1 + W21×3，真实代码迁移）**；K01/K05/K06 复验 PASS；**偏离项 `D3-EX-01`：右侧顶层页签不显示左右滚动箭头**（经用户批准，当前组件导航合同=滚轮横向滚动+边缘渐隐+当前页签自动显露+全部页签入口，不要求箭头；适用范围仅限当前右侧顶层页签组件，不修改 UI Spec 通用规则）；保留项：W28 复核合规、W29/W36（Left 不在范围）、G02（D6）、G04（D4 紧凑模式部分）、K07（D5）；
  - **D4 完成（2026-08-06，用户真机验收未通过 → D4-F1）**：检查器（字号 Token 层级/结构化字段/全宽分组去卡片/调试页 96 列）；地图编辑器（只读摘要 72 列/MapId 前 8…后 6+Tooltip+复制）；图层面板（图标 16/热区 26×24/Layer.* Token/选中样式/三重区分/插入线 DropLine #5B8DB8）；**基线 226 → 159（-67，真实迁移）**；守卫缺陷修复（warcore $failures 重置吞失败 + 5+100 self-check 期望笔误）；**D4-F1（2026-08-06，READY FOR USER RE-ACCEPTANCE）**：只读键值行始终单行双列（规范 §7.1.1 定稿：默认标签列 80、72~96 组件范围、单行省略+完整 Tooltip）、可编辑表单行仅真实输入控件 <360 整组上下（EditableFormLayoutModel 统一 360，删除 320 紧凑模型）、调试页四列表结构化（DebugText/BuildDebug* → InspectorFieldRow）、公共语义样式 uiLabel/uiValue/uiSingleLine/uiMultiline/uiSection/uiTextButton（Ui.axaml，全部 Token 引用）、sideTab/caption 裸 FontSize 迁移 Token（值不变）、图层名/属性单行省略+Tooltip；基线维持 159（未新增债务）；
  - **允许清单（按路径+规则类型+API 模式+原因登记）**：TreeGuide.cs Render（树引导线渲染色 ALLOW-RENDER）、Win32ViewportHost.cs（Win32 样式常量非颜色 ALLOW-WIN32）、activeMark/dropLine 圆角 1.5/1（组件例外，规范 §5.4，基线保留 2 条）；
  - **D4-A1 收口（2026-08-06，用户正式裁决 D4 COMPLETE）**：F1-1~F1-9 全部通过；K02/K03/K04 复验 PASS；登记 **D5-FIX-01**（按钮内容未居中，D5 第一项统一处理）；
  - **D5 完成（2026-08-06，D5-A1 用户真机验收通过 → COMPLETE）**：按钮（D5-FIX-01 内容居中 + 完整状态 + uiDanger，基线 -5）；表单（TextBox 状态 + error/warning 非仅颜色 + 提交反馈）；弹窗（DialogHost 普通/危险/未保存，危险非默认焦点 Enter=Escape 安全，未保存重构删代码 Window 基线 -11，新建地图/删除图层危险确认）；通知（四级状态机单条不刷屏 + Footer 通知条）；空状态（日志初次/筛选无结果区分 + 清空筛选）；日志（回到底部按钮 + TailStateChanged，自动跟随/用户上滚保留）；D5-FINAL 后地图状态四态与未保存新建流程通过复验；**债务基线 122**；
  - **D6 代码完成（2026-08-06，READY FOR USER ACCEPTANCE）**：DPI/缩放合同（100/125/150/175/200，DIP 阈值）、键盘与基础可访问性（AutomationProperties.Name + 自动补名且不导出内部编号）、减少动画偏好（Reduce 归零非必要动效）、日志性能（500 条尾窗 + 相邻重复项压缩）、剩余旧 UI 债务复核；未新增 Token，债务基线 122 保持，Manifest 112 Frozen / 0 Pending；等待 D6-A1 用户真机验收；
  - 治理完成前暂停新增 UI 功能（编辑器、游戏 UI、地图功能面板、工具面板）；
  - 未经用户批准不得创建受控例外。
- **前置约束**：
  - 不得与 `MapLayer` 领域合同混为一谈（图层归属地图资产，见 D4-F1 信息架构裁决）；
  - 不得借规范立项修改 AI 开发宪法；
  - 立项前禁止建立全局主题/设计系统代码（已解除——D1 已正式冻结规范，D2 起允许建立 Token）。
- **近期局部合同参考**（各轮已落地，规范立项时应收敛为统一文档）：
  - D4-F3 右侧字号合同：顶层页签 13 / 二级页签 14 / 分组标题 13 半粗 /
    字段标签 12 / 字段值 13 / 按钮 12 / 类型标签 10～11；
  - D4-F3 状态图标配色：可见 #326F8A/#EAF3F7、隐藏 #8995A2、
    锁定 #7A6238/#F4EFE5、未锁定 #7B8794，热区 26×24 DIP；
  - D4-F3 类型标签：区域 #E8F3F6/#326B7B/#B9D7DE，
    系统 #F0F2F4/#687582/#D5DBE0；
  - F3 拖动插入线：2 DIP 低饱和蓝 #7FA8C6。
