# WORLD-A-R1-R1 当前事实 Owner 矩阵

版本：v0.2.18.6-rz

| 模块 | 变更前实体来源 | 本轮目标 | 本轮结果 |
| --- | --- | --- | --- |
| GlobalWorld | EntityRegistry | 唯一实体生命周期 Owner | PASS：继续作为唯一 Create / Destroy / Get / TryGet / Exists 入口 |
| EntityRegistry | 自身字典 | 唯一 Entity State Owner | PASS：新增 Transform 更新与稳定快照列表，不复用已销毁 ID |
| SceneStateOwner | 自有 TestEntity 快照 | 消费 GlobalWorld | PASS：改为持有 GlobalWorld，只承担 Scene 投影、编辑会话与 Spatial 派生 |
| MinimalSceneEntity | SceneRenderSnapshot 静态样例 | WorldEntity 投影 | PASS：默认实体由 GlobalWorld 创建，再投影为 SceneEntitySnapshot |
| Selection | EditorState 字符串快照 | EntityId 引用 | PASS：仍只保存 `EntityId(...)` key，不保存实体对象或 Transform |
| Hierarchy | UiText 静态实体节点 | World 投影 | PASS：实体节点由 `_sceneState.Entities` 动态生成 |
| Inspector | UiText 静态字段 | EntityId 查询 World | PASS：选中实体时通过 EntityId 查询 WorldEntitySnapshot |
| SceneRenderSnapshot | SceneStateOwner 自有实体 | World Fact 单向生成 | PASS：通过 SceneWorldProjection 生成，支持 Empty 防幽灵 |
| Picking | SpatialRaycast EntityKey | 返回稳定 EntityId | PASS：仍返回 EntityId，Spatial 只作派生命中索引 |
| Gizmo | RenderSnapshot 当前实体 | 根据 EntityId 查询 World | PASS：空实体拒绝捕获；有效实体来自 World-backed RenderSnapshot |
| Transform Preview / Commit | SceneStateOwner 自有 Transform | 落同一 World Entity State | PASS：Commit / Restore 均写回 GlobalWorld 后再刷新投影 |
| Undo / Redo | SceneStateOwner Restore | 落同一 World Entity State | PASS：History Entry 的 EntityId 恢复同一个 World Entity |

禁止项确认：未进入 Partition、Spatial Index 正式扩展、Organization、完整 ECS、Terrain、Streaming、Gameplay、Rotation、Scale、Local Gizmo。
