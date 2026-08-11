# MAP-DOC-A-R2-F3 · Dataset / Layer 真机验收模板

状态：`READY FOR USER RE-ACCEPTANCE`。右侧“图层”本轮只是 Dataset-backed Projection，不代表正式 Layer 系统完成。

| 序号 | 路径 | 输入 | 过程 | 输出 / 判定 |
|---|---|---|---|---|
| F3-M01 | 数据集页 | 已有两个 Dataset | 点击左侧第一行 | 整行出现轻量选中态；`解除注册` 可用 |
| F3-M02 | 数据集页 | 已选第一项 | 点击第二行 | 左侧仅第二项选中，第一项取消选中 |
| F3-M03 | 数据集 → 新建 | 选择“河流” | 点击“新建数据集” | 新生成 `river-xxxxxx`，创建后自动选中 |
| F3-M04 | 数据集 → 解除注册 | 选中 `region-xxxxxx` | 点击“解除注册” | 只移除选中 ID；日志目标与该 ID 一致 |
| F3-M05 | 右侧图层 | 地图编辑模式，已有 Dataset | 观察右侧“图层” | 显示与 Dataset 列表相同的中文类型、ID、状态 |
| F3-M06 | 左 → 右 | 左侧点击 road | 观察右侧 | 右侧 road 同步进入选中态 |
| F3-M07 | 右 → 左 | 右侧点击 river | 观察左侧 | 左侧 river 同步进入选中态 |
| F3-M08 | 解除后同步 | 选中 road | 解除注册 | 左右两侧同时消失；Manifest descriptor 消失；源文件保留 |
| F3-M09 | 保存 → 重开 | 至少两个 Dataset | 保存并重开同一 Manifest | 左右投影数量与 ID 一致；无 dangling selection |

前置阻塞项：若地图未保存 Manifest，先按 F2-M01/M02 建立正式 Dataset Registry。自动测试和本轮静态取证不替代上述用户真机结论。
