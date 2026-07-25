# ARCH-WORLD-R2：单一空间权威收敛

版本：`v0.2.19.3-rz`｜分支：`refactor/ARCH-WORLD-layer-boundary`｜上游 R1：`v0.2.19.2-rz`（已 CLOSED）

## 一、只读审计结论（现状）

- `GlobalWorld`（`GlobalWorld.cs:11`）**已是世界唯一事实 + 唯一写链**：`Create`(:33) / `Destroy`(:41) / `UpdateTransform`(:51) / `RebuildSpatialIndexFromWorld`(:91) 已同步内部 `WorldQuery`（A 索引）。
- `SceneStateOwner` 自己 `new GlobalWorld()`，其内部 `_world` 含一个**冗余 A'**（无外部读取）；同时另维护 `_spatialIndex`（**B 索引**），而 B 才是 Picking / Editor（`UiVm.Picking.cs:25` → `scene.RaycastSpatial`）实际读取的源。
- 双写共 4 处：`SceneStateOwner.cs:25`（构造）、`:62`（`ApplyTransform`）、`SceneStateOwner.Lifecycle.cs:15`（`CreateEntity`）、`:24`（`DestroyEntity`）。
- **真相只有一份**（`GlobalWorld`/`EntityRegistry`），不存在第三套空间事实 → 停手条件 #4 不触发。

## 二、唯一权威目标

```
GlobalWorld（事实）
    │
    ▼
WorldQuery（投影门面）
    │
    ▼
唯一 SpatialIndexOwner（仅加速结构，可重建）
    ▲      ▲      ▲
 Scene  Picking  未来 Streaming
```

> 同一个 `EntityId` 在世界中的空间状态，只允许一个权威查询答案。

## 三、Writer / Reader / Owner / Derived（R2 后）

| 角色 | R2 后 |
| --- | --- |
| **Owner（事实）** | `GlobalWorld` / `EntityRegistry`（不变） |
| **Writer（唯一写链）** | 仅 `GlobalWorld`：`Create` / `Destroy` / `UpdateTransform` / `Rebuild` |
| **Reader（读取）** | `SceneStateOwner.QuerySpatial` / `RaycastSpatial` / `SpatialRevision` 经 `_world` 兼容门面读唯一索引 |
| **Derived** | `SpatialIndexOwner` 仅为加速结构；索引可重建，不持有真相 |

## 四、不动项（R2 严禁扩围）

- D1 `TransformSession` 迁 Editor｜D2 `SceneRenderSnapshot` 治理｜D3 测试项目彻底分层
- Camera Inspector 修复｜DefaultEditorCamera 治理
- Large World｜Position64 迁移｜Streaming｜新 Partition 算法｜10K 压测
- Preview 语义重定义｜Render Snapshot 结构修改
- 把 `ViewportPicking` 搬 Editor（属 R4）——本轮只改它**查询谁**，不改它**属于哪层**

## 五、迁移步骤（最小、行为保持）

1. **`WorldQuery`**：将现有私有 `Query(SpatialAabb, mask)` / `Query(SpatialRayQuery, mask)` 提升为 `public`；新增 `Raycast(SpatialRayQuery, mask) => SpatialRaycastResult`（委托 `_index.Raycast`，并刷新 `LastStats`）。
2. **`GlobalWorld.Query`**：新增 `QuerySpatial(area)` / `QuerySpatial(ray)` / `RaycastSpatial(ray)` / `SpatialRevision` 四个委托，转发到 `_query`。
3. **`SceneStateOwner`**：删除 `_spatialIndex` 字段与全部 `Insert/Update/Remove` 调用；`QuerySpatial` / `RaycastSpatial` / `SpatialRevision` 改为委托 `_world`；删除已无用的 `ToSpatialBounds` 静态助。
4. **`SceneStateOwner.Lifecycle`**：删除 `CreateEntity` 的 `_spatialIndex.Insert` 与 `DestroyEntity` 的 `_spatialIndex.Remove`（`_world.Create/Destroy` 已喂 A'）。
5. **守卫**：`arch-a-guard-world.ps1` 增加 `XuanYu.World/Scene/*` 禁止出现 `new SpatialIndexOwner`，防第二索引回潮。

## 六、行为不变保证（核心）

B 与 A' 同为 `SpatialIndexOwner`，由**完全相同**的 `_world.Create / UpdateTransform / Destroy` 调用序列驱动。R2 只是把 B 的 4 处双写删除，让 Scene 改读 A'。

→ `Insert / Update / Remove` 的调用**顺序与次数逐字节相同** → `SpatialRevision` 编号、Picking 命中位置、Undo/Redo 后的空间答案全部保持。

## 七、自动测试（`WorldSceneSingleAuthorityTests`）

- **Case1 Create**：创建实体 → `RaycastSpatial` / `QuerySpatial` 能查到。
- **Case2 Move（核心）**：A→B 提交后，查 A 不得命中旧位，查 B 必须命中。
- **Case3 Undo/Redo**：P1→P2→Undo P1→Redo P2，每步空间查询一致。
- **Case4 Destroy**：销毁后 `SpatialQuery` 不再返回。
- **Case5 跨 Region**：移出旧 Region，查新位正确、旧 Region 无残留。
- **Case6 单权威（架构守护）**：反射断言 `SceneStateOwner` 不再有 `_spatialIndex` 字段；配合源码守卫。

## 八、验收门（自动）

- `arch-a-guard.ps1` EXIT=0
- `dotnet build` 9 项目 0W0E
- `dotnet test` 全绿（含 R2 新用例）
- 5+100：所有改动文件 ≤100 行

## 九、真机验收（用户执行，13 项）

Hierarchy 选择 / Viewport Picking / Move Preview / Move Commit / Escape Cancel / Undo / Redo / 跨 Region 移动 / Frame Selected / Resize / 关闭生命周期；**重点盯**：移动后 Picking、Undo 后 Picking、Redo 后 Picking（旧位不可选、新位必选）。

**状态：R2 CLOSED（2026-07-25，ARCH-WORLD-R2-R0D 收口）—— 13 项真机验收全部 PASS（1–12 真机通过，第 13 项自动测试覆盖 UI N/A），Undo 经用户日志 PASS，G1 P0 随 R2 一并 CLOSED。**

## 十、停手条件（立即收手并报告）

1. 为删 B 须大规模重写 Picking → 停
2. 须修改 Render Snapshot 结构 → 停（R5）
3. 须重定义 Preview 语义 → 停（单独报告）
4. 发现 `GlobalWorld` 并非真实权威、存在第三套空间事实 → 停

## 十一、版本

`v0.2.19.3-rz`。
