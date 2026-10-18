# XYC-P2-A-R1 场景命令契约

协议继续使用 `XYC-API/1` JSON Lines。请求必须携带当前 `sessionId` 和唯一 `requestId`。错误以 `success=false`、`errorCode` 结构化返回。

| Command | 权限 | Args | 效果 |
|---|---|---|---|
| `scene.entity.list` | READ | `{}` | 返回 SceneStateOwner 实体集合及各实体完整 Transform。 |
| `scene.entity.inspect` | READ | `{ "entityId": "EntityId(n)" }` | 按 EntityId 读取 World/Scene 实体。 |
| `scene.test.inspect` | READ | `{}` | 返回 SceneId/Revision、实体 ID、Hierarchy 投影、RenderSnapshot 投影、Inspector 字段、路径、活动交互、Undo/Redo 深度及测试阶段。 |
| `scene.entity.create-test-cube` | WRITE | `{ "sceneId": "…", "expectedRevision": n }` | 仅在空、未保存、无历史、无交互的独立会话中创建并登记一个测试 Cube。 |
| `scene.entity.set-test-position` | WRITE | `{ "sceneId": "…", "expectedRevision": n, "entityId": "…", "position": {"x":3.5,"y":-2.25,"z":7} }` | 仅修改本 Session 刚创建的测试 Cube Position；拒绝非有限值及绝对值大于 10000 的分量。 |
| `scene.history.undo` | WRITE | `{ "sceneId": "…", "expectedRevision": n, "entityId": "…" }` | 按事务阶段撤销本 Session 的位置提交，最终撤销测试 Cube 创建。 |
| `scene.history.redo` | WRITE | 同上 | 只重做刚刚撤销的位置提交。 |

所有 WRITE 还要求 Editor 以 `--xyc-diagnostic-write` 启动；Session ID 必须与 Runtime 一致。写命令检查 SceneId、预期 SceneRevision、测试 EntityId、状态阶段、History/Redo 深度、区域模式和活动交互。测试不开放保存、删除、导入、任意 Transform、任意命令或其他实体写入。

## 测试阶段

`0 baseline → 1 created(T0) → 2 moved(T1) → 3 undo(T0) → 4 redo(T1) → 5 undo(T0) → 6 creation undone / restored`。

预期 History 深度依次为 `0/0 → 1/0 → 2/0 → 1/1 → 2/0 → 1/1 → 0/2`（Undo / Redo）。World Revision 只要求按每次真实 World 变更更新；Undo 不要求 Revision 倒退。

## 主要错误码

`UNAUTHORIZED`、`DUPLICATE_REQUEST`、`INVALID_INPUT`、`INVALID_ENTITY_ID`、`ENTITY_NOT_FOUND`、`NOT_SUPPORTED`、`SCENE_MISMATCH`、`STALE_REVISION`、`UNSAFE_SCENE`、`UNAUTHORIZED_ENTITY`、`HISTORY_CONFLICT`、`INVALID_STAGE`、`TRANSFORM_REJECTED`、`POSTCONDITION_FAILED`、`NOT_READY`。

命令成功只表示操作后置条件已通过；Entity、Hierarchy、RenderSnapshot、Inspector 与 History 均由实际 Editor 状态读取，而不是按请求内容回显。
