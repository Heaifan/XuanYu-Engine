# XYUI-5 Source Audit & Inventory / 源审计与清点

- 阶段：`XYUI-PILOT-R5 · FAST-CLOSE · T2 Inventory`
- 依据：`xyui/source/XYUI5/XYUI-5.md`（不可变证据源）
- 原稿出处：Obsidian `XYUI-5 布局与容器.md`（2026-08-13 12:08，252,543 bytes）
- 冻结 SHA-256：`45734e3f4d305e173983f09d6348405d81ec5c97ff8cabe68bb77c641434c523`（原始稿 SHA `57c1ca7d…`，仅清理 18 处行尾空白，语义零改动，原始 SHA 记入 commit message）
- 上游：A2 Foundation Registry（VALIDATED + AMEND-A/B）+ XYUI-1/2/3/4 canonical

## 20 组件清点

| 编号 | 组件 | 行范围 | 存在 |
|---|---|---|---|
| 5.01 | Stack / 堆叠布局 | 1–376 | ✅ |
| 5.02 | Grid / 网格布局 | 377–783 | ✅ |
| 5.03 | Wrap / 流式换行布局 | 784–1133 | ✅ |
| 5.04 | Dock / 停靠布局 | 1134–1582 | ✅ |
| 5.05 | ScrollArea / 滚动区域 | 1583–2013 | ✅ |
| 5.06 | SplitPane / 分栏面板 | 2014–2484 | ✅ |
| 5.07 | OverlayLayout / 叠层布局 | 2485–2941 | ✅ |
| 5.08 | AspectContainer / 比例容器 | 2942–3279 | ✅ |
| 5.09 | AnchorLayout / 锚定布局 | 3280–3650 | ✅ |
| 5.10 | StickyRegion / 滚动吸附区域 | 3651–3883 | ✅ |
| 5.11 | AdaptiveLayout / 自适应布局 | 3884–4293 | ✅ |
| 5.12 | WorkspaceLayout / 工作区布局 | 4294–4737 | ✅ |
| 5.13 | LayoutPersistence / 布局持久化 | 4738–5091 | ✅ |
| 5.14 | VirtualizedLayout / 虚拟化布局 | 5092–5520 | ✅ |
| 5.15 | CanvasLayout / 自由坐标布局 | 5521–5916 | ✅ |
| 5.16 | ViewportContainer / 视口容器 | 5917–6250 | ✅ |
| 5.17 | PortalHost / 跨层承载容器 | 6251–6538 | ✅ |
| 5.18 | MasonryLayout / 瀑布流布局 | 6539–6916 | ✅ |
| 5.19 | LayoutDiagnostics / 布局诊断 | 6917–7305 | ✅ |
| 5.20 | LayoutCompositionRules / 布局组合规则 | 7306–7839 | ✅ |

- 编号连续 5.01~5.20，无缺失、无重复：**20/20 清点完成**。
- 上游对账基线（附件 XYUI-5 Final Reconciliation）：20/20 保留，0 删除，5 项 RE-SCOPE（5.04 Dock / 5.07 OverlayLayout / 5.12 WorkspaceLayout / 5.17 PortalHost / 5.20 LayoutCompositionRules）；无新增 Foundation Amendment。

## 状态

`INVENTORY COMPLETE · READY FOR RECONCILIATION & CANONICALIZATION`
