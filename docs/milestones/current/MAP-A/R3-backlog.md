# MAP-A-R3 Backlog

R2 已关闭。本文件登记 R3 当前裁定与候选方向；每轮先冻结目标和范围。

## 当前裁定

- D1：CLOSED。
- D2：OPEN。真机 A01 FAIL；A02～A06 BLOCKED / NOT EXECUTED；L4 FAIL。
- D3：禁止启动。
- 当前轮次：`MAP-A-R3-D2-F1 — Region Tool Integration & Selected-State Regression`。
- 本轮之后不创建 F2；F1 完成后只回到 D2-A01a/A01b 复验，再决定是否恢复 A02～A06。

## D2-F1 范围

只修两个已确认根因：

1. Region Drawing 必须是 Map Editor 内部 tool/mode，入口位于地图编辑器的“地图工具”区，不作为 Top/App-level 并列功能。
2. Region Drawing 的 Normal、Hover、Selected、Selected+Hover 文字必须使用正式深色正文 token，不得被 FluentTheme 覆盖为白色。

禁止修改 Region domain、Validator、Create/Delete、History、Picking、Renderer、LayerPanel、Inspector；禁止全局 UI 重构。

## D2-F1 证据

- RED：真实 Headless Runtime 复现 Region Drawing 不在 Map Editor 树内，以及 Selected Foreground 为 White。
- GREEN：修复后 Region Drawing 归属与 Selected 深色文字 Runtime 4/4；静态归属/状态契约通过。
- L4：F1 完成后仍需用户重新执行 D2-A01a/A01b；通过后才恢复 A02～A06。

## 候选主题

- Inspector 完整编辑闭环。
- 地图数据落盘与 `.xymap` 持久化。
- 区域绘制与地形表达的后续真实闭环。
- DGD 衔接最小真实入口。
