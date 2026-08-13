# XYUI-7 Source Audit & Inventory / 源审计与清点

- 阶段：`XYUI-PILOT-R7 · FAST-CLOSE + XYUI CORE CROSS-AUDIT · T3 Inventory`
- 依据：`xyui/source/XYUI7/XYUI-7.md`（不可变证据源）
- **原稿出处（本次为真实磁盘文件，非转录）**：`D:\MyDoc\doc-Obsidian\我的知识库\XYUI-7.md`（用户原始文件，2026-08-13 21:02，UTF-8）
  - 原始 SHA-256：`9d84373aad6c830ee688a307c52bb6c9698994fdf0783eb83a207af399db77a1`（141,712 bytes，4,565 行）
  - 附件副本（`XuanyuEngine-XYUI/.hermes/desktop-attachments/XYUI-7.md`）与原始文件字节级一致（cmp identical）
- 冻结 SHA-256：`8fb6be6cf096b248779cd2d7f79e49f32effd323d7b8f8f32d0c869c59e5040e`（141,652 bytes，4,565 行；仅清理 15 处行尾空白，语义零改动）
- 上游：A2 Foundation Registry（VALIDATED + AMEND-A/B）+ XYUI-1/2/3/4/5/6/8 canonical

## 16/16 组件清点

| ID | 名称 | 行范围 | 存在 | 责任摘要 |
|---|---|---|---|---|
| 7.01 | Dialog / 对话框 | 1–201 | ✅ | 短暂阻塞式确认；紧凑基线；危险后果表达；不塞大型表单 |
| 7.02 | Popover / 气泡浮层 | 202–471 | ✅ | 就地对象摘要与轻量操作；非阻塞；Inspector Popover 为主 |
| 7.03 | Tooltip / 提示浮层 | 472–715 | ✅ | 数据检查瞬时提示；移动端不依赖 Hover；不替代 Popover |
| 7.04 | Context Menu / 上下文菜单 | 716–981 | ✅ | 对象上下文命令；高频不藏入；桌面右键/移动 Long Press |
| 7.05 | Toast / Snackbar / 短暂反馈 | 982–1259 | ✅ | 操作结果即时反馈；Undo Snackbar；Persistent Error；四方案全采 |
| 7.06 | Drawer / Side Sheet / 抽屉侧滑 | 1260–1545 | ✅ | 中复杂度持续编辑；Inspector/Task/Peek 三变体；可 Pin 升级 Panel |
| 7.07 | Window / Floating Window / 独立浮窗 | 1546–1796 | ✅ | 独立生命周期窗口；四变体均可选能力；移动端不照搬 |
| 7.08 | Docking & Window Management / 停靠管理 | 1797–2153 | ✅ | Blender 式运行时工作区重组；Split/Join/Detach/Reattach/Restore |
| 7.09 | Lightbox & Fullscreen Preview / 聚焦全屏 | 2154–2403 | ✅ | Focus Preview 核心；复用原内容交互与状态；Media/Compare 可选 |
| 7.10 | Command Palette / 命令面板 | 2404–2700 | ✅ | 全局命令搜索；Action First；Stable Command；统一 Command Contract |
| 7.11 | Notification Center / 通知中心 | 2701–2961 | ✅ | 重要事件历史；可重新处理；与 Log 边界清晰；克制原则 |
| 7.12 | Progress Overlay & Task Monitor / 进度任务 | 2962–3264 | ✅ | 非阻塞优先；真实进度；Critical Transaction 严格限定；取消语义 |
| 7.13 | Spotlight & Coachmark / 聚焦引导 | 3265–3523 | ✅ | One-shot Coachmark 为主；Spotlight 仅重大新能力；不强制教程 |
| 7.14 | Drag & Drop Overlay / 拖放覆盖层 | 3524–3845 | ✅ | Drop 前验证；Intent 预览；Invalid 提前反馈；移动 Placement Mode |
| 7.15 | File Dialog & Resource Picker / 文件资源选择 | 3846–4136 | ✅ | 系统文件桥 + 项目资源选择器 + 导入前校验；选文件≠写项目 |
| 7.16 | Overlay Stack & Focus Management / 浮层级与焦点 | 4137–4565 | ✅ | 语义层级、焦点陷阱/恢复、Overlay 父子关系、Esc 顺序、Safe Area |

- 编号连续 7.01~7.16，无缺失、无重复：**16/16 清点完成**。
- 全部 16 项为 `NEW`（XYUI-7 浮层与窗口层），与 1~6/8 大量组合。
- 预判跨组件裁定点（进入 T4 对账）：
  - **7.04 Context Menu vs XYUI-3 3.03 ContextMenu**（上游已拥有菜单结构 → REF；7-04 只拥有 Overlay 承载/上下文调用）
  - **7.10 Command Palette vs XYUI-3 3.18 CommandPalette**（上游已拥有命令搜索契约 → REF；7-10 只拥有 Overlay 表面与响应式承载）
  - 7.03 vs XYUI-1 Tooltip（1.19）+ Foundation XY.Tooltip.\*（ShowDelay 400ms/MaxWidth 280/InteractiveContent Forbidden 等基础合同）
  - 7.08 vs XYUI-5 WorkspaceLayout（5.12 布局拓扑/persistence）+ 5.04 Dock + Foundation ResizeSplitter
  - 7.16 vs Foundation 五 Host ZIndex 体系 + XYUI-5 5.07 OverlayLayout Planes + 5.17 PortalHost
  - 7.14 vs Foundation DragDrop（Ghost/Cursor 机制）
  - 7.05 vs XYUI-4 InlineFeedback/Loading/Progress（不能互相替代）
  - 7.12 vs XYUI-4 ProgressBar/LoadingIndicator（进度视觉 REF）
  - 7.09 vs XYUI-8 Visualization Interaction（交互继承，不建第二套手势）
  - 7.15 vs XYUI-6 资源列表/虚拟化 + 平台原生文件 Provider

## 状态

- `XYUI-7 · INVENTORY 16/16 · RECONCILED · CANONICAL 16/16（969 行）· MAPPING 16/16（134 refs）· GAPS 1（NON-BLOCKING）· SECOND TRUTH 0 · BROKEN REF 0 · → READY FOR USER ACCEPTANCE`
- 收口审计：`xyui/audit/XYUI7/reconciliation.md`；全局 Cross Audit：`xyui/audit/cross-audit.md`
