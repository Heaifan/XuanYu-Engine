# MAP-DOC-A-R3-F2 · Dataset / Layer UI 收口真机验收

状态：`USER ACCEPTANCE FAIL · 2026-08-11`，不是 `CLOSED`。保留 F2 历史，不覆盖其实现记录；根因是双行 Dataset/Layer 列表与自定义 28×28 开关偏离正式 UI Spec 的 28/32 DIP 单行合同，且 Dataset 选中时 MapFormPanel 未隐藏。修复转入 R3-F3。

| 序号 | 路径 | 输入 I | 过程 P | 输出 O |
|---|---|---|---|---|
| UI-M01 | 地图编辑器 → 数据集 | 已有名称 Dataset | 观察 Dataset 行 | Name 为主信息，右侧状态清晰可见。 |
| UI-M02 | 地图编辑器 → 数据集 | 长 Dataset ID | 观察辅助行 | `Type · ID` 单行省略，不碎裂为多行。 |
| UI-M03 | 地图编辑器 → 图层 | 至少一个 Dataset | 观察并操作图标 | 拖动、显示和锁定均为项目 StreamGeometry 图标，尺寸一致。 |
| UI-M04 | 地图编辑器 → 图层 | 长名称、长 ID Dataset | 观察行布局 | Name、Type/ID、Status 清晰且不拥挤。 |
| UI-M05 | 数据集 → 图层 | 至少两个 Dataset | 点击左侧任一 Dataset | 右侧同 ID Layer 同时选中，Inspector 显示该 Dataset。 |
| UI-M06 | 图层 → 数据集 | 至少两个 Dataset | 点击右侧任一 Dataset 行 | 左侧同 ID Dataset 同时选中，Inspector 显示该 Dataset。 |
| UI-M07 | 检查器 | 已在任一侧选中 Dataset | 观察检查器 | 名称、类型、ID、状态、可见和锁定均属于当前唯一 Dataset Selection。 |
| UI-R01 | 数据集 / 图层 | 已有多个 Dataset | Visible、Lock、Drag Order、Rename、Create、Unregister 后保存重开 | 既有能力均保持有效，左右顺序与选择一致。 |

结论：本表未通过，R3 保持 OPEN；MAP-DATA-A 禁止进入。自动测试不替代真机结论。
