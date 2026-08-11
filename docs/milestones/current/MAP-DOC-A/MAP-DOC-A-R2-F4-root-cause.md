# MAP-DOC-A-R2-F4 · 未保存地图工作存储根因

状态：`IMPLEMENTED · READY FOR USER RE-ACCEPTANCE`。范围只处理未保存地图的 Dataset 可写工作区与首次保存提升；不改 Map/Dataset Schema、Renderer、Picking、图层持久化或自动保存。

## 根因

原 `EnsureDatasetRegistryAsync()` 将 `CurrentMapManifestPath` 为空直接等价为“不能创建 Dataset”。这不是 Dataset Create 事务失败，而是把“用户尚未保存正式地图”错误等同为“编辑器没有可写 Map Workspace”。

## 修复

- `MapWorkingStorage` 仅负责 `Ensure`、`Promote`、`Discard`；首次 Dataset 创建时才在 `%TEMP%/XuanYuEngine/map-working/<guid>/map.json` 写入内部 Manifest。
- `MapManifestOwner.CurrentPath` 仍为空；临时路径不进入标题、Save/Save As 或 UI 状态。
- Promotion 只以当前 Manifest 的注册 Descriptor 为清单校验、复制和提交 Dataset；源/目标路径必须通过 `MapDatasetPathPolicy`，且目标不得碰撞。
- Dataset 先以临时文件复制到目标，再提交 Dataset 文件，最后保存正式 Manifest；失败删除本轮新目标文件并保留 Working Workspace。

## 自动证据

F4/F1/F2/F3 聚焦回归 `16/16 PASS`。真机验收和完整 Gate 结论以本轮收口记录为准。
