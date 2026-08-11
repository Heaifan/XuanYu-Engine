# MAP-DOC-A-R2 · Dataset Registry 真机验收

状态：`READY FOR USER RE-ACCEPTANCE`，不是 `CLOSED`。R2-F1 先复验 M02/M03，再按 R2-F2 的自动 ID 与中文列表路径复验 M02/M03；R2-F3 完成 Dataset selection 与右侧 Dataset-backed Layer Projection 后，才能继续 M04～M07；实现边界止于 Dataset Registry 与最小 Dataset 页面；Geometry、Feature 内容、properties、AI 编辑 API、Asset Reference、Layer/Renderer/Picking 重构均不在本轮。

| 序号 | 路径 | 输入 | 过程 | 输出 / 判定 |
|---|---|---|---|---|
| R2-M01 | 地图基础 → 数据集 | 打开无 Dataset 的 map.json | 切换数据集页 | 显示“当前无数据集”和“新建数据集”；待真机填写 |
| R2-M02 | 数据集 → 新建 | 类型“道路”，不填写 ID | 点击新建数据集 | 生成 `data/road-xxxxxx.json`，Manifest 出现注册项；按 F2-M02 待真机填写 |
| R2-M03 | 数据集列表 | 已创建 Dataset | 观察列表 | 显示中文主类型、ID 副行、Status=正常；按 F2-M03 待真机填写 |
| R2-M04 | 数据集文件 | 删除已注册文件后刷新/重开 | 观察列表 | 对应 Status=缺失，其余页面可用；待真机填写 |
| R2-M05 | 数据集文件 | 写入损坏 JSON 后刷新/重开 | 观察列表 | 对应 Status=无效，其余页面可用；待真机填写 |
| R2-M06 | 数据集 → 解除注册 | 选中已注册 Dataset | 点击解除注册 | Manifest 移除注册项，物理 Dataset 文件保留；待真机填写 |
| R2-M07 | 数据集 → 同 type | 两个不同 ID，均为 `road` | 依次创建并观察列表 | 两项均可存在且状态独立；待真机填写 |

自动证据：C1 `14/14`、C2 `10/10`、C3 `8/8`、C4 UI `7/7`、F2 focused `17/17`、F3 focused `6/6` PASS；自动测试不替代上述真机结论。
