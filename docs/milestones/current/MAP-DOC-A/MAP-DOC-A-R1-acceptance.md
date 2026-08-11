# MAP-DOC-A-R1 · 真机验收清单

状态：`READY FOR USER ACCEPTANCE`

| 编号 | 输入 I | 过程 P | 输出 O |
| --- | --- | --- | --- |
| M01 | 打开地图编辑 Workspace | 查看左侧地图内容导航 | 显示“地图基础 / 地图环境 / 数据集” |
| M02 | 点击“地图基础” | 查看内容 | 名称、ID、坐标系、原有地图属性正常，无内容丢失 |
| M03 | 点击“地图环境” | 查看内容 | 既有环境展示正常，未出现未经批准的新 Schema 字段 |
| M04 | 点击“数据集” | 查看内容 | 显示“当前无数据集”，无 R2 新建/删除/编辑入口 |
| M05 | 三项导航反复切换 | 连续切换并观察视口 | Viewport、Camera、World 不重建、不跳动 |
| M06 | 创建并保存 Map Document | 查看目标目录 | 产生合法 UTF-8 `map.json`，不强制创建空 `data/` 或 `assets/` |
| M07 | 保存后重新打开 `map.json` | 对比 Manifest | format、version、id、name、coordinate_system、datasets、assets 领域等价 |
| M08 | 手工破坏 `map.json` | 尝试打开 | 明确拒绝，当前地图与当前 Manifest 不被替换 |

任一项失败：保持 `MAP-DOC-A-R1 OPEN`，只创建对应 F1 修复记录；不得进入 R2。
