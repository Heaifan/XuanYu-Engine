# MAP-DATA-A-R2 真机验收清单

状态：`CLOSED`。R2 最终真机收口见 `MAP-DATA-A-R2-closeout.md`；F3-E M01～M10 已由用户确认 PASS。

| 编号 | 路径 | 输入 | 过程 | 输出 |
|---|---|---|---|---|
| R2-M01 | 地图编辑 → 区域编辑 → 道路 | 打开含多个 Dataset 的地图 | 进入区域编辑并选择“道路”，检查道路 Dataset 与图层列表 | Workspace、图层、Dataset 身份正确隔离 |
| R2-M02 | 区域编辑 → 道路 → 绘制道路 | 无 Road Dataset 或选中正常 Road Dataset | 点击“绘制道路”，连续点击至少两个地图点 | 自动创建/选中 Road Dataset，出现 Polyline 草稿 |
| R2-M03 | 区域编辑 → 道路 → 草稿历史 | 三个以上草稿节点 | Ctrl+Z、Ctrl+Y，观察节点数 | 仅撤销/重做草稿节点，不提前写入正式内容 |
| R2-M04 | 区域编辑 → 道路 → 完成道路 | 有效 Polyline 草稿 | 按 Enter 或“完成道路” | 生成一条正式道路，渲染为不闭合折线 |
| R2-M05 | 区域编辑 → 道路 → 图层控制 | 多个 Road Dataset | 分别切换可见、锁定、顺序 | 状态和顺序只影响对应 Road Dataset |
| R2-M06 | 地图保存 → 重新打开 | 已完成道路 | 保存 Manifest，关闭/重新打开 | Road Feature 的 ID、节点、name/kind、归属 Dataset 保持一致 |
| R2-M07 | 区域编辑 → 道路 → 地图历史 | 已完成道路 | Ctrl+Z、Ctrl+Y | 以一条正式道路为单位撤销/重做，草稿节点不混入 Map History |

验收结论：________；日期：________；设备/版本：________；备注：________。
