# MAP-DOC-A-R2 · Dataset Registry

## 裁定

正式推进 A 方案：R2 先完成 Dataset Registry 的合同、文档存储、注册表生命周期和最小 Dataset 页面；暂不进入 Geometry、编辑器图形工具、AI 编辑 API、Asset Reference 或 Layer/Renderer/Picking 重构。

## 分段状态

| 段 | 范围 | 状态 |
|---|---|---|
| C0 | R1-F1 Manifest ID 同步与复制布局 | COMPLETE，commit `53d31dd` |
| C1 | Descriptor、六类 type、ID/source/唯一性合同 | COMPLETE，当前提交 |
| C2 | `xuanyu-map-dataset` v0.1.0、空 features、Normal/Missing/Invalid | PENDING |
| C3 | Create/Register/Resolve/Enumerate/FindById/Unregister 与跨文件事务 | PENDING |
| C4 | Dataset 页面、空态、新建、列表、解除注册 | PENDING |

## C1 已冻结合同

- Descriptor 只有 `id`、`type`、`source`；允许 `region`、`road`、`settlement`、`resource`、`river`、`terrain_area`。
- Dataset ID 为小写字母、数字、`-`、`_` 组成的稳定标识，大小写不敏感唯一。
- source 必须是 map 根目录下 `data/` 内的安全相对路径，拒绝盘符、根路径、反斜杠和 `..`。
- 同一 type 可以注册多个 Dataset；本轮不引入 Geometry、Feature、properties 或 Asset Reference。

## 验收边界

R2 全部自动门禁通过后仅标记 `READY FOR USER ACCEPTANCE`。真机验收需另行记录 Dataset 页面空态、创建、列表状态和解除注册的 IPO 结果；自动测试不得替代真机结论。
