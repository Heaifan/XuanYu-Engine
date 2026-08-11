# MAP-DOC-A-R2-F3 · Dataset Selection + Layer Projection Sync

状态：`READY FOR USER RE-ACCEPTANCE`，不是 `CLOSED`。基线为 `d133bc6`；范围只覆盖 Dataset selection、解除注册目标、Dataset-backed Layer Projection，不改 F2 创建链、Manifest/Dataset schema 或正式 Layer 领域系统。

## T01～T03 取证

| TODO | 证据 | 结论 |
|---|---|---|
| F3-T01 | 本地/远端均为 `d133bc6`，`0/0`；仅保留 `_tmp_blind_rows/` | 基线固定 |
| F3-T02 | 用户截图同时显示 Dataset 行“道路/222/正常”、新建类型“区域”，解除注册失败日志目标为空并显示“（region）” | 列表没有可消费的 selection；失败反馈复用了 Create Form type |
| F3-T03 | 旧 `UnregisterDatasetAsync` 由 `DatasetSelectedId` 取目标，但重开和行点击都不会建立该状态 | 根因是缺少 `SelectedDatasetId` 单一选择合同 |

## T04～T09 实施

- F3-T03/T04：`MapDatasetRow.IsSelected` 仅为 `SelectedDatasetId` 的派生展示；行点击统一调用 `SelectDataset(id)`，不维护第二份 Layer selection。
- F3-T05：创建成功后继续自动设置新 Dataset ID，并由刷新投影将其标记为 selected。
- F3-T06：解除注册只消费 `SelectedDatasetId`；无选择时按钮 disabled、命令 fail-closed；成功后下一项优先、无下一项则上一项，空列表清空选择。
- F3-T07/T08：地图编辑模式右侧新增 Dataset-backed Layer Panel；左、右两侧都消费同一 Dataset 投影和同一 `SelectDataset(id)`，不接入旧 Layer schema。
- F3-T09：重开从 Registry 重建右侧投影，选择不制造持久化字段。
- F3-T10：F3 focused `6/6 PASS`，Dataset F1/F2/F3 focused `15/15 PASS`；Solution Build `0 Warning / 0 Error`；Core.Tests `339/339`、WarCore.Tests `22/22`、World.Tests `1221/1221`；ARCH-A、5+100、版本一致性与 `git diff --check` 均通过。

## 明确不在本轮

不实现 Layer persistence、layers JSON、Layer name、visible/locked、拖拽排序、分组、删除 Layer、Renderer filtering 或 Dataset Inspector。
