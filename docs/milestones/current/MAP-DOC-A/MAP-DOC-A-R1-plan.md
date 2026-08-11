# MAP-DOC-A-R1 · Map Content Navigation + Map Manifest

状态：`READY FOR USER ACCEPTANCE`（自动门禁通过后等待 M01～M08 真机验收）

## 基线与职责审计

- 远端基线：`feat/LAYER-A-layer-shell @ 454da48`；本轮工作分支：`feat/MAP-DOC-A`。
- 既有 `XuanYu.Editor/MapDocument` 是 MAP-A 的 `.xymap` 领域链，继续供场景地图引用使用；本轮不改其 Schema、World、Renderer、Picking 或环境字段。
- R1 新增独立 `MapManifest` 合同，文件名固定为 `map.json`，只包含 `format`、`version`、`id`、`name`、`coordinate_system`、`datasets`、`assets`。
- `datasets` 与 `assets` 是空容器；Dataset Registry、Geometry、Asset Reference 分别留在 R2、R3、R4。
- Editor State（Camera、Workspace、面板尺寸、折叠状态、当前 Tab）不进入 Manifest。

## Reuse Matrix

| 责任 | 处理 | 说明 |
| --- | --- | --- |
| 地图内容权威 | 复用 `MapEditSession` / `MapDefinition` | Manifest 只投影身份与 R1 容器 |
| 旧 `.xymap` 场景引用 | 保留 `MapStorageService` | 不与 `map.json` 混用 |
| JSON | 复用 `System.Text.Json` | 不新增第三方依赖 |
| 原子保存 | 新增 `MapManifestStorageService` | 候选读取、校验后替换 |
| 左侧导航 | 复用 `MapEditorPanel` | 收口为地图基础、地图环境、数据集 |
| Dataset Registry | 不实现 | MAP-DOC-A-R2 |

## R1 已实现合同

1. Create / Read / Validate / Save / Round-trip。
2. Fail Closed：错误 JSON、未知字段、错误格式、错误版本、非法 ID、非法坐标系、错误容器均拒绝。
3. 保存先校验、同目录临时文件写入并 Flush，再替换目标；失败清理临时文件，不替换当前状态。
4. UI 最小接线：地图基础显示 Manifest ID 与坐标系；数据集显示“当前无数据集”；地图环境继续显示既有占位能力。
5. 真机未通过前，R1 不标记 `CLOSED`，不启动 R2。
