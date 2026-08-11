# MAP-DOC-A-R1-F1 · Manifest Identity UI 真机验收

状态：`PENDING USER ACCEPTANCE`。本表不改写 R1 M07 历史事实：此前 M01～M06 通过，M07 发现 inline ID 文本未随 Manifest 切换即时刷新，M08 当时未完成。

| 序号 | 路径 | 输入 | 过程 | 输出 / 判定 |
|---|---|---|---|---|
| R1F1-M01 | 地图基础 → ID | 打开地图 A | 记录 ID 文本、Tooltip、复制结果 | 三者均为 A 的完整 ID；待真机填写 |
| R1F1-M02 | 地图基础 → 打开地图 | 依次打开 A、B、C、再回到 B | 每次观察 ID 行 | 文本、Tooltip、复制立即同步；待真机填写 |
| R1F1-M03 | 地图基础 → ID | 使用超长 ID 地图 | 缩窄右侧面板并悬停/复制 | 文本省略但 Tooltip 与复制保留完整值；待真机填写 |
| R1F1-M04 | 地图基础 → ID | 复制按钮 | 点击复制并粘贴到可见文本框 | 粘贴值等于当前完整 MapId；待真机填写 |

自动证据：`UiMapManifestIdentityTests` focused `3/3 PASS`。自动测试不替代上述真机结论。
