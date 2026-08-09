# MAP-A-R2 收口报告

状态：**CLOSED**
收口类型：只盘点、转移未完成需求、确认交付证据；本次不开发功能。
收口基线：`a6540b9` / `v0.2.24.50-fix` / `feat/MAP-A-map`

## R2 已交付能力

- 地图领域合同、默认地图聚合、尺寸/坐标/地表/边界模型。
- `MapEditSession` 单一状态权威、Selection、Command、History、Dirty 与事件提交管线。
- 地图编辑器 UI、属性输入与校验、日志/通知反馈、窗口与 DPI/可访问性治理。
- 图层列表、系统层保护、用户层增删改名、可见/隐藏、锁定/解锁。
- 图层拖拽排序、插入线、撤销/重做与真机验收闭环。
- Gizmo、参考网格、世界原点/坐标轴及地图渲染投影的既有收口成果。
- Runtime UI Gate：Avalonia Headless Fixture/Host、LayerPanel 布局/状态、Top/Foot 风险回归。

## 关闭证据

| 项目 | 结果 |
|---|---|
| F5 真机验收 | CLOSED，图层 8/8 PASS |
| UI D6 | CLOSED |
| 解决方案 Build | 0 Warning / 0 Error |
| Core.Tests | 344/344 PASS |
| World.Tests | 938/938 PASS |
| WarCore.Tests | 22/22 PASS |
| Runtime UI | 7/7 PASS |
| ARCH-A | PASS |
| 5+100 | PASS |
| git diff --check | PASS |
| Git | `a6540b9`，远端同步，工作树 clean |

## 明确不属于 R2 的内容

Inspector 完整编辑闭环、地图持久化、区域绘制/地形表达、DGD 衔接及其他新游戏开发能力不再回补 R2，统一转入 [MAP-A-R3 Backlog](../../current/MAP-A/R3-backlog.md)。

R2 关闭后禁止以“补齐旧 D5 编号”为理由重新打开本阶段；只有数据损坏、程序无法启动或已验收能力回归，才允许建立独立阻断审查。

## 下一阶段

正式进入 `MAP-A-R3` 规划。R3 必须先冻结一个新的主目标，再开始开发；不得把 R2 leftovers 作为默认主线。
