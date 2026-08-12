# MAP-DATA-A-R1-F2 · Polygon & Auto Bootstrap 真机验收

状态：`READY FOR USER ACCEPTANCE`。自动测试不替代人工验收；F2 六项全部 PASS 后继续 R1-M02～R1-M07，R1 保持 OPEN。

| 序号 | 路径 | 输入 I | 过程 P | 输出 O |
|---|---|---|---|---|
| F2-M01 | 编辑模式 → 区域编辑 → 绘制区域 | 当前没有区域数据集 | 点击“绘制区域” | 自动创建一个区域 Dataset，自动选中并设为活动 Region 图层；按钮进入选中态；左侧显示当前目标与状态。 |
| F2-M02 | 区域编辑 → 视口 → 绘制区域 | 一个正常区域 Dataset | 依次点击四个不共线地面点，按 Enter 闭合 | 生成一个四顶点 Region；四边形、五边形及简单凹多边形均可正常闭合；区域边不发生误判。 |
| F2-M03 | 区域编辑 → 视口 → 绘制区域 | 连续点击形成自交、非相邻接触或重叠边 | 尝试闭合非法 Polygon | 闭合被拒绝，保留明确错误反馈；不写入非法 Region。 |
| F2-M04 | 区域编辑 → 数据集图层 → 绘制区域 | 锁定 Region Dataset 或 Dataset 文件无效 | 点击“绘制区域” | 明确提示不可绘制；不创建第二个 Dataset，不进入区域绘制工具。 |
| F2-M05 | 区域编辑 → 顶部工具栏 → 绘制区域 | 无区域 Dataset，快速连续点击两次 | 连续触发“绘制区域” | 只创建一个 Region Dataset；Bootstrap 完成后仅保留一个正式绘制目标。 |
| F2-M06 | 地图保存 → 重新打开 | F2-M01 自动创建 Dataset，并完成四点 Region | 保存 `map.json`，关闭后重新打开 | Dataset、RegionId、四个顶点及归属全部恢复；R1 仍显示为 OPEN，未启动 R2。 |

验收结论：用户填写 `PASS / FAIL`。F2 未全部 PASS 前，不得标记 R1 CLOSED；R1 全量通过前不得启动 R2。
