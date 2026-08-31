# XYUI-5 Reconciliation & Closeout / 全量对账收口

- 状态：`XYUI-5 · RECONCILED · READY FOR USER ACCEPTANCE`
- 阶段：`XYUI-PILOT-R5 · FAST-CLOSE ONE ROUND`
- Source：`xyui/source/XYUI5/XYUI-5.md`（IMMUTABLE，SHA `45734e3f…`，252,543 bytes，7,839 行；原始稿 SHA `57c1ca7d…`，仅清理行尾空白）
- Canonical：`xyui/specs/XYUI5/XYUI-5.canonical.md`（1,316 行，20/20）
- 上游：Foundation Registry（VALIDATED + AMEND-A/B）+ XYUI-1/2/3/4 canonical + 附件 XYUI-5 Final Reconciliation

## 20/20 对账矩阵

| 项 | Source | Canonical | Mapping | 冲突处置 | GAP | 第二真值 |
|---|---|---|---|---|---|---|
| 5.01 Stack | ✅ | ✅ | ✅ | NEW+EXTENSION（消费 Foundation Spacing） | — | 0 |
| 5.02 Grid | ✅ | ✅ | ✅ | NEW（≠ DataGrid） | — | 0 |
| 5.03 Wrap | ✅ | ✅ | ✅ | NEW | — | 0 |
| 5.04 Dock | ✅ | ✅ | ✅ | RE-SCOPE（附件四节：Tab/Drag/Drop/Split 全 REF 上游） | — | 0 |
| 5.05 ScrollArea | ✅ | ✅ | ✅ | 只拥有滚动容器行为（Scrollbar REF Foundation） | — | 0 |
| 5.06 SplitPane | ✅ | ✅ | ✅ | 复用 Foundation Splitter / Resize | — | 0 |
| 5.07 OverlayLayout | ✅ | ✅ | ✅ | HOST BOUNDARY（附件五节：局部 Plane 限于单一 Host） | — | 0 |
| 5.08 AspectContainer | ✅ | ✅ | ✅ | Min/Max/Alignment REF Foundation | — | 0 |
| 5.09 AnchorLayout | ✅ | ✅ | ✅ | Edge Constraint（确定性求解） | — | 0 |
| 5.10 StickyRegion | ✅ | ✅ | ✅ | Scroll Flow Sticky（≠ Fixed Header） | — | 0 |
| 5.11 AdaptiveLayout | ✅ | ✅ | ✅ | Container+Constraint 驱动（≠ 屏幕断点） | — | 0 |
| 5.12 WorkspaceLayout | ✅ | ✅ | ✅ | WORKSPACE OWNERSHIP（附件六节：Role 体系 vs XYUI-3 切换） | — | 0 |
| 5.13 LayoutPersistence | ✅ | ✅ | ✅ | 唯一 Save/Restore/Migration 基础设施 | — | 0 |
| 5.14 VirtualizedLayout | ✅ | ✅ | ✅ | 通用虚拟化（收口 XYUI-3 Tree 大数据） | — | 0 |
| 5.15 CanvasLayout | ✅ | ✅ | ✅ | 逻辑坐标（不拥有 Pan/Zoom） | — | 0 |
| 5.16 ViewportContainer | ✅ | ✅ | ✅ | View Transform（不改 Logical Position） | — | 0 |
| 5.17 PortalHost | ✅ | ✅ | ✅ | MAJOR RE-SCOPE（附件七节：Cross-Layer Placement，Host=Foundation） | — | 0 |
| 5.18 MasonryLayout | ✅ | ✅ | ✅ | 稳定逻辑顺序 + 自适应列宽 | — | 0 |
| 5.19 LayoutDiagnostics | ✅ | ✅ | ✅ | Development Only | — | 0 |
| 5.20 LayoutCompositionRules | ✅ | ✅ | ✅ | SPACING OWNERSHIP（附件八节：Semantic vs Execution Owner） | — | 0 |

## 全量统计

```text
Source accounted        20/20
Canonical accounted     20/20（1,316 行）
Mapping accounted       20/20（147 refs）
GAP reconciled          0/0（设计稿 0 hex / 0 px，全 REF 上游；Spacing 档位旧命名为收敛非 GAP）
A-Class unresolved      0
Second Truth            0
Broken Ref              0（221 引用全部解析）
Source Mutation         0
Duplicate Contract      0
```

## 裁定落地清单（附件 Reconciliation 五项 + 上游引用）

```text
5.04 Dock            RE-SCOPE：Foundation 默认骨架之上的高级 Topology；TabSystem=XYUI-3；
                     Drag/Drop=XYUI-4；Split=5.06；Resize=Foundation；Persistence=5.13
5.07 OverlayLayout   HOST BOUNDARY：Semantic Planes 限于单一 Foundation Host；CrossHostOverride=Forbidden
5.12 WorkspaceLayout WORKSPACE OWNERSHIP：Role 体系 vs XYUI-3 WorkspaceSwitcher 切换；三概念分离
5.17 PortalHost      MAJOR RE-SCOPE：Cross-Layer Placement Infrastructure；Host=Foundation 正式 Host；
                     Scope=InheritedFromFoundationHost
5.20 LayoutCompositionRules  SPACING OWNERSHIP：Semantic Spacing Owner vs Layout Execution Owner；
                     XY.Panel.Padding/Field.RowGap/SectionGap 消费
其余 15 项            全部按设计稿内建 Ownership Check 落实 REF/REUSE/COMPOSE
```

## 状态

```text
XYUI-5 · Layout & Containers
    20/20 CANONICAL COMPLETE
    MAPPING COMPLETE（147 refs）
    GAPS 0（无缺失 Token）
    A-CLASS 0
    SECOND TRUTH 0
    → READY FOR USER ACCEPTANCE
```

唯一未 CLOSED 原因：`XYUI-A-plan.md` 明文规定该阶段须用户最终裁定才能 CLOSED（不得伪造用户验收）。
