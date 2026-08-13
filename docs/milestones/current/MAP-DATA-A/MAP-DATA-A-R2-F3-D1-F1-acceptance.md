# MAP-DATA-A-R2-F3-D1-F1 · Road Draw → Select 定向复验

状态：`PENDING USER RE-ACCEPTANCE`；自动门禁基线：`F1 实现提交`；本轮不验收 Road Drag/D2。

| 序号 | 路径 | 输入 | 过程 | 输出 |
|---|---|---|---|---|
| F1-M01 | 区域编辑 → 道路 → 绘制道路 | Road 至少 2 个节点 | 点击“完成道路” | Draft 清除，工具自动回到“选择” |
| F1-M02 | 区域编辑 → 道路 | 刚完成的 Road | 观察视口与当前选择 | 新 Road 自动选中，全部顶点立即显示 |
| F1-M03 | 区域编辑 → 道路 → 选择 | 已完成 Road，点击空地 | PointerDown 空地 | 不创建新 Road，不出现 Road Draft |
| F1-M04 | 区域编辑 → 道路 → 选择 | 已完成 Road | 点击 Road 线 | 执行 Road Picking，保持/切换当前 Road 选择 |
| F1-M05 | 区域编辑 → 道路 → 选择 | 已完成 Road | 连续点击空地两次 | Road 数量不增加，仍不进入绘制态 |
| F1-M06 | 区域编辑 → 道路 | 已完成 Road A | 再次明确点击“绘制道路”，完成 Road B | 仅此时创建第二条 Road，完成后再次回到选择 |

自动证据：F1 专项 6/6；Core 339/339；World 1356/1356；WarCore 22/22；Solution Build 0 Warning / 0 Error；ARCH-A PASS。通过后 D1 才能重新评估关闭，D2 之前保持 BLOCKED。
