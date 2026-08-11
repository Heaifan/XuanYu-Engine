# MAP-DOC-A-R2-F1 · Dataset Create/Register 真机链路修复

状态：`READY FOR USER RE-ACCEPTANCE`。本轮暂停 R2-M04～M07，范围只覆盖 Create、Register、Manifest Save、跨文件一致性、成功后的 Registry/UI refresh 与失败反馈。

## T01/T02 取证

用户提供的现场事实是：点击“新建数据集”后日志只有“地图命令收到”，目标 `map.json` 仍为 `datasets: []`；`data/111.json` 未由附件证明。当前 checkout 内没有可复核的 `map.json` 或 `111.json` 样本；真机窗口在本轮取证时处于最小化/外部输入占用状态，未强行代替用户操作。

| 分支 | 文件 | Manifest | 本轮结论 |
|---|---|---|---|
| A | 未确认 | `datasets=[]` | 若实际打开的不是 Manifest 路径，旧实现会把 `.xymap` 会话路径当作 MapRoot；失败还可能没有可见最终反馈。 |
| B | 未观察 | 未注册 | Registry 事务增加 Dataset 写失败回滚测试。 |
| C | 未观察 | 已注册 | 未观察到反向半提交；创建提交后才发布 Registry。 |
| D | 未确认 | 未确认 | 后端直接 Create/重开/状态测试通过；增加真实 RunCommand 异步链测试。 |
| E | 高风险根因 | 磁盘路径不确定 | `CurrentMapManifestPath` 原先回退 `MapSession.CurrentFilePath`，混淆 `.xymap` 与 `map.json` 所有权；现已严格要求正式 Manifest 路径。 |

## 已确认根因与修复

1. MapRoot 所有权错误：Dataset Create 不得使用旧 MapDefinition/`.xymap` 路径。无正式 map.json 路径现在明确拒绝并提示“请先保存地图 Manifest”。
2. UI 命令边界不可靠：按钮原先 fire-and-forget 调用 `CreateDatasetAsync`，异常/失败不能稳定落到用户反馈。现由受控异步路由捕获并显示最终 Failure。
3. 结果语义不足：现在成功显示“数据集创建成功：id（type）”，失败显示明确原因，并写入一条最终结果日志；“命令收到”不再代表提交成功。
4. 提交前校验补齐：Dataset Document 与 Manifest Candidate 均在双文件提交前校验；Dataset 写失败、Manifest 提交失败均回滚外部可观察状态，Registry 只在提交完成后更新。

## 专项证据

- F1-A01～A07、A10～A12：`UiMapDatasetF1Tests`。
- F1-A08～A09：`MapDatasetRegistryF1FailureTests`。
- R2-F1 focused：`9/9 PASS`；最终门禁通过后，真机只需先复验 R2-M02/M03，随后继续 M04～M07。
