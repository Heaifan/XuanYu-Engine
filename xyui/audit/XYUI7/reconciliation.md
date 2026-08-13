# XYUI-7 Reconciliation & Closeout / 全量对账收口

- 状态：`XYUI-7 · RECONCILED · READY FOR USER ACCEPTANCE`
- 阶段：`XYUI-PILOT-R7 · FAST-CLOSE + XYUI CORE CROSS-AUDIT · ONE ROUND`
- Source：`xyui/source/XYUI7/XYUI-7.md`（IMMUTABLE，SHA `8fb6be6c…`，141,652 bytes，4,565 行）
- Source provenance（如实声明）：原稿为**真实磁盘文件** `D:\MyDoc\doc-Obsidian\我的知识库\XYUI-7.md`（用户原始文件，2026-08-13 21:02，原始 SHA `9d84373a…`，141,712 bytes；`.hermes/desktop-attachments/XYUI-7.md` 附件副本与原始文件字节级一致）。冻结副本仅清理 15 处行尾空白（语义零改动），SHA `8fb6be6c…`。原始磁盘文件 SHA 可随时字节级复核。
- Canonical：`xyui/specs/XYUI7/XYUI-7.canonical.md`（969 行，16/16）
- Mapping：`xyui/specs/XYUI7/XYUI-7.mapping.json`（16/16，134 refs）
- 上游：Foundation Registry（VALIDATED + AMEND-A/B）+ A3-R2 Token Architecture + XYUI-1/2/3/4/5/6/8 canonical

## 16/16 对账矩阵

| 项 | Source | Canonical | Mapping | 冲突处置 | GAP | 第二真值 |
|---|---|---|---|---|---|---|
| 7.01 Dialog | ✅ | ✅ | ✅ | 短暂阻塞式确认表面；Compact Confirm 默认基线；危险在基线上升级内容（后果/数量/恢复）；Modal Layer=ModalHost（经 7.16） | — | 0 |
| 7.02 Popover | ✅ | ✅ | ✅ | 锚定瞬态表面；Inspector 为主 + quickSetting/actions；ResponsiveMode anchored/bottomSheet；危险确认不得用 Popover 替代 Dialog | — | 0 |
| 7.03 Tooltip | ✅ | ✅ | ✅ | TOOLTIP BOUNDARY：基础合同 REF XYUI-1 Tooltip（1.19）+ XY.Tooltip.*；本项拥有 data/hint、8.16 Inspect 联动、Responsive Hint | — | 0 |
| 7.04 Context Menu | ✅ | ✅ | ✅ | CONTEXT MENU BOUNDARY：命令结构 REF XYUI-3 3.03 ContextMenu；本项拥有 invocation/placement/dismiss/adaptation | — | 0 |
| 7.05 Toast | ✅ | ✅ | ✅ | INLINE FEEDBACK BOUNDARY：跨局部区域临时通知表面；XYUI-4 InlineFeedback=内容上下文内反馈；两者不互相替代 | — | 0 |
| 7.06 Drawer | ✅ | ✅ | ✅ | SIDE SURFACE BOUNDARY：inspector/task/peek 三变体；内布局 REF XYUI-5；Pin/Promote 升级 Panel 保持同一内容语义 | — | 0 |
| 7.07 Window | ✅ | ✅ | ✅ | 独立生命周期窗口；四变体均为可选能力；chrome 尺寸档 → GAP | 1 | 0 |
| 7.08 Docking | ✅ | ✅ | ✅ | RUNTIME WORKSPACE BOUNDARY：Layout topology/persistence = REF XYUI-5 5.12 WorkspaceLayout + 5.04 Dock；本项拥有运行时直接操作（Split/Join/Resize/Detach/Reattach/Restore） | — | 0 |
| 7.09 Lightbox | ✅ | ✅ | ✅ | PRESENTATION SHELL BOUNDARY：外壳=7.09，Media/Data/Viz 内容=对应模块；交互继承 REF XYUI-8 8.16；升级路径 7.07 Window | — | 0 |
| 7.10 Command Palette | ✅ | ✅ | ✅ | COMMAND PALETTE BOUNDARY：命令搜索契约 REF XYUI-3 3.18；本项拥有 surface/分组/参数化委托/Recent；不建第二套 Search/List/Menu | — | 0 |
| 7.11 Notification Center | ✅ | ✅ | ✅ | NOTIFICATION BOUNDARY：高价值事件历史可重处理；Log=技术高频事件；Read ≠ Resolved | — | 0 |
| 7.12 Task Progress | ✅ | ✅ | ✅ | PROGRESS BOUNDARY：进度视觉 REF XYUI-4 4.16/4.17/4.14；本项拥有任务状态机/Cancel 语义/Blocking 四条件/Rollback | — | 0 |
| 7.13 Coachmark | ✅ | ✅ | ✅ | 一次性发现引导；不替代 Help/Documentation；Spotlight 仅重大新能力 | — | 0 |
| 7.14 Drag & Drop | ✅ | ✅ | ✅ | DROP OVERLAY BOUNDARY：Drag 机制 REF XYUI.Foundation.DragDrop；本项拥有 Drop Target Overlay/Intent/Invalid/Placement Mode | — | 0 |
| 7.15 File Picker | ✅ | ✅ | ✅ | RESOURCE PICKER BOUNDARY：系统文件桥 REF 平台原生 Provider；项目资源语义；选中 ≠ 写入；导入前校验 | — | 0 |
| 7.16 Overlay Stack | ✅ | ✅ | ✅ | OVERLAY INFRASTRUCTURE：业务 L0~L4 MAP ONTO Foundation 五 Host + XY.ZIndex + XYUI-5 5.07 Planes + 5.17 PortalHost；禁第二套 Z/Plane；Focus Trap/Restore | — | 0 |

## 全量统计

```text
Source accounted        16/16
Canonical accounted     16/16（969 行）
Mapping accounted       16/16（134 refs）
  CANONICAL_REF         38
  NAMESPACE_REF         33
  COMPONENT_SPECIFIC    57
  COMPOSE               5
  GAP                   1（XYUI7-GAP-001：7.07 Window chrome 尺寸档）
GAP reconciled          1（XYUI7-GAP-001，NON-BLOCKING；0 项遗漏 Token 复用）
A-Class unresolved      0（无已 CLOSED 核心合同互斥，无需改 Foundation）
Second Truth            0（hex 0 / rgb 0 / hsl 0 / 旧字体 0 / 硬编码 px 0）
Broken Ref              0（134 引用全部解析；上游 1/2/3/4/5/6/8 组件清单逐项核对）
Source Mutation         0（仅 15 处行尾空白清理，语义零改动）
Duplicate Contract      0
Semantic Ambiguity      0（15 对跨组件语义逐对裁定，见下）
SFD                     0（无内容级源缺陷登记）
```

## 裁定落地清单（14 项所有权划清）

```text
7.03 Tooltip            TOOLTIP BOUNDARY：基础 Tooltip 合同 = REF XYUI-1 Tooltip（1.19）+ XY.Tooltip.*
                        （MaxWidth 280 DIP / ShowDelay 400 ms / ViewportAvoidance / AutoFlip /
                        PointerCapture Forbidden / InteractiveContent Forbidden）；本项只增加 overlay
                        hosting（TooltipHost）、data/hint Variant、8.16 Inspect 联动、Responsive Hint；
                        不建立第二个 Tooltip 核心合同
7.04 Context Menu       CONTEXT MENU BOUNDARY：菜单命令结构/子菜单导航 = REF XYUI-3 3.03 ContextMenu；
                        本项只拥有 invocation context（右键/Context Key/Long Press）、overlay placement、
                        dismiss、object/context adaptation；高频操作不得全部藏进 Context Menu
7.10 Command Palette    COMMAND PALETTE BOUNDARY：命令搜索契约 = REF XYUI-3 3.18 CommandPalette；
                        本项只拥有 palette overlay/surface、统一分组展示、参数化命令第二步委托、Recent/
                        Favorites；与 7.04 共享同一 Command Contract（正式 Command ID 唯一），
                        不成为第二套 Search/List/Menu
7.08 Docking            RUNTIME WORKSPACE BOUNDARY：Layout topology/persistence/split/dock/geometry
                        = REF XYUI-5 5.12 WorkspaceLayout + 5.04 Dock；本项只拥有运行时工作区直接操作
                        （Area Split/Join、Direct Resize、Editor Switch、Detach/Reattach、Workspace
                        Restore/Reset）；不建立第二套 Dock/Layout Engine；Dock Zones/Tab Group 保持
                        Advanced/Optional 不升格
7.16 Overlay Stack      OVERLAY INFRASTRUCTURE：业务 L0~L4 语义 MAP ONTO Foundation 五 Host
                        （ContentHost/OverlayHost/DragHost/ModalHost/TooltipHost）+ XY.ZIndex（禁魔法
                        数字/禁跨 Host 覆盖）+ XYUI-5 5.07 单 Host 内 Semantic Planes + 5.17 PortalHost；
                        禁止第二套 Z 数字/Plane 命名；Focus Trap/Restore 生命周期归 7.16
7.05 Toast              INLINE FEEDBACK BOUNDARY：Toast/Snackbar = 跨局部区域的临时通知表面；XYUI-4
                        InlineFeedback = 内容上下文内反馈；两者不能互相替代；不可逆危险操作 REF 7.01；
                        长期记录 REF Log/Notification Center
7.12 Task Progress      PROGRESS BOUNDARY：进度/加载视觉 = REF XYUI-4 ProgressBar（4.16）/ProgressRing
                        （4.17）/LoadingIndicator（4.14）；本项拥有任务状态机、真实进度规则、Cancel
                        语义、Critical Transaction Blocking 四条件、Rollback；禁止“实现简单就锁全屏”
7.09 Lightbox           PRESENTATION SHELL BOUNDARY：Fullscreen shell/Focus Preview 外壳 = 7.09；
                        Chart/Media/Data 内容 = 对应模块（Chart 全屏 = 7.09 外壳 + XYUI-8 内容）；
                        交互继承 REF XYUI-8 8.16（不建第二套 Chart Gesture）
7.14 Drag & Drop        DROP OVERLAY BOUNDARY：Drag 机制（Ghost/Cursor/Entry）= REF XYUI.Foundation.
                        DragDrop（AMEND-B DirectTarget）；本项只拥有 Drop Target Overlay 语义、Drop
                        Intent 预览、Invalid 提前反馈、移动 Placement Mode、Drop 前验证
7.15 File Picker        RESOURCE PICKER BOUNDARY：系统文件选择 = 平台原生 Provider（禁重复实现
                        Explorer/Finder）；项目资源选择 = 项目业务资源语义（REF XYUI-6 列表/虚拟化）；
                        选文件 ≠ 写项目；导入前 Preview + Validation + Transaction
7.02 Popover vs 7.01    危险确认不得仅用 Popover 替代 Dialog；Popover 非阻塞可随时关闭，Dialog 需明确
                        决策并阻塞后续流程
7.06 Drawer vs 7.07     Drawer = 附着主应用边缘的临时/响应式侧表面；Window = 可独立移动、拥有独立窗口
                        生命周期；Panel = 长期常驻；三者边界固定，Peek Drawer 可 Pin 升级 Panel
7.11 vs Log             Notification Center = 高价值用户级事件（克制、可重处理、可持久化）；Log = 技术
                        高频事件（可成百上千条）；Toast 即时反馈、Log 长期记录、Notification 历史
7.13 vs Help            Coachmark/Spotlight = 一次性发现引导（允许跳过/不再提示）；正式帮助交给
                        Help/Documentation；Coachmark 自身不承担完整教程，不弥补基础 UI 缺陷
```

## 跨组件语义审计（T9 · 15 对）

```text
Popover vs Tooltip         对象摘要+轻量操作（用户主动打开）vs 瞬时信息+低交互（自动消失）；Tooltip 不替代 Popover
Tooltip vs Inspector       瞬时检查当前位置 vs 锁定后的稳定检查（多指标比较/完整对象信息）；可操作内容升级 Popover/Inspector
Popover vs Drawer          快速检查/快速改/短暂 vs 持续检查/中等复杂编辑；典型升级路径 Popover → 查看详细属性 → Drawer
Drawer vs Dialog           Drawer 保持上下文非阻塞为主 vs Dialog 明确决策阻塞为主；需阻塞决策时改用 Dialog
Drawer vs Window           Drawer 附着主应用边缘 vs Window 独立移动独立生命周期；Drawer 不膨胀为第二主界面
Window vs Panel            Window 独立可长期存在 vs Panel 常驻固定区域；Dockable Window 在 Floating 与 Docked Panel 间转换，内容语义不变
Preview vs Window          Preview 短暂依赖原上下文、关闭返回原位置 vs Window 独立长期拥有生命周期；每次放大创建新 Window = 禁止
Toast vs Log               Toast 即时反馈 vs Log 长期记录；Toast 消失不代表事件记录消失；大量后台事件不能全 Toast 化
Toast vs Notification      Toast 即时短暂 vs Notification Center 历史可重处理；操作完成 → Toast → 消失 → 重要事件保留中心
Notification vs Log        Notification 高价值用户级事件（克制/可重新处理）vs Log 技术高频事件（详细诊断）；Log 可以有成百上千条
Context Menu vs Toolbar    上下文补充入口 vs 高频一层直达；Context Menu 不能成为唯一入口，高频操作不得藏入右键
Context Menu vs Palette    对象附近的上下文入口 vs 全局搜索入口；两者引用同一 Command Contract，不建两套命令体系
Progress vs Loading        任务进度（可量化真实进度）vs 加载活动（无法量化）；Spinner 禁止与确定进度长期并列表达同一任务
Critical vs 普通 Loading   Blocking 四条件全满足才允许全局阻塞 vs 默认非阻塞；普通实验运行/导出/加载不适合 Critical Overlay
Palette vs Search          Palette 全局命令/对象/导航统一入口 vs 内容搜索；Palette 结果必须清晰分组，不成为第二套 Search/List
```

## 第二真值扫描（T9）

```text
#hex 硬编码             0（canonical 全部走 XY.Semantic.* / XY.Opacity.* / XY.Shadow.* / XY.State.*）
rgb / rgba              0
hsl                    0
旧字体名                0
px                     0 处硬编码值（Source 尺寸为叙述性档位，canonical 收敛为 DIP 组件档位 + Token 引用）
重复 State / Selection / Layout / Focus / Tooltip / Drag 真值   0
（全部 REF XYUI-4 / XYUI-5 / XYUI-1 / Foundation；唯一例外 = 7.16 Focus Trap/Restore 生命周期，
  为 7.16 独有合同，与 Foundation.Focus 视觉互补不重复）
```

## 状态

```text
XYUI-7 · Overlays & Windows
    16/16 CANONICAL COMPLETE
    MAPPING COMPLETE（134 refs）
    GAPS 1（XYUI7-GAP-001 window chrome metric，NON-BLOCKING）
    SFD 0
    A-CLASS 0
    SECOND TRUTH 0
    BROKEN REF 0
    DUPLICATE CONTRACT 0
    → READY FOR USER ACCEPTANCE
```

唯一未 CLOSED 原因：`XYUI-A-plan.md` 治理要求模块 CLOSED 须用户最终裁定（不得伪造用户验收）。
Cross Audit 结论见 `xyui/audit/cross-audit.md`。
