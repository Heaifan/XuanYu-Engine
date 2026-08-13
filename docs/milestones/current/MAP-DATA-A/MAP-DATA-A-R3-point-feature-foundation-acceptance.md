# MAP-DATA-A-R3 · Point Feature Foundation 验收

状态：`IMPLEMENTED · AUTOMATED GATES PASS · READY FOR USER ACCEPTANCE`。

## 真机 IPO

| 编号 | 路径 | 输入 | 过程 | 输出 |
|---|---|---|---|---|
| M01 | 区域编辑 → 地图标记 | 点击“放置地图标记”，点击地面 | 完成一次放置 | Marker 创建、自动回“选择”、自动选中并显示单控制点 |
| M02 | 区域编辑 → 地图标记 | 拖动 Marker 控制点 | PointerDown → 移动 → 释放 | 自由拖动，Preview 后 Commit |
| M03 | Marker → Region | 将 Marker 靠近 Region Vertex/Edge | 进入 8px、离开 12px | Vertex/Segment Snap 生效且稳定 |
| M04 | Marker → Road | 将 Marker 靠近 Road Vertex/Segment | 进入 8px、离开 12px | 跨 Feature Snap 生效 |
| M05 | Marker → Marker | 将一个 Marker 靠近另一个 Marker | 小范围抖动与释放 | 目标 Marker Vertex 吸附，不吸自身 |
| M06 | 区域编辑 → 地图标记 | Esc、Undo、Redo | 拖动中取消、提交后回退/重做 | 取消不写入；一次拖动一条历史；身份不变 |
| M07 | Marker ↔ Region ↔ Road | 连续选择并编辑三类几何 | 往返切换并拖动 | 无 stale drag/snap state，无跨对象串写 |
| M08 | 地图保存/重载 | 保存后重新打开地图 | 检查 Marker Dataset/Layer/Feature | 坐标、ID、Dataset/Layer 身份保持 |

自动证据：Point/Generic focused `13/13`；完整正式门禁在本轮收尾执行一次。自动 PASS 不替代用户真机验收，完成 M01～M08 后才可将 R3 标记 CLOSED。
