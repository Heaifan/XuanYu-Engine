# ARCH-WORLD-R2 实施状态与真机验收

版本：`v0.2.19.3-rz`｜分支：`refactor/ARCH-WORLD-layer-boundary`｜方案 Gate：`docs/arch-world-r2-single-spatial-authority.md`

## 一、裁定（项目负责人，2026-07-24）

R2 唯一权威主体 PASS，但暂缓 CLOSED，先执行 **R2-R1 最小架构修正**：

| 项 | 裁定 |
| --- | --- |
| 双轨 SpatialIndex 删除 | ✅ PASS |
| `GlobalWorld` 唯一事实 + 唯一写链 | ✅ PASS |
| `SceneStateOwner` 查询委托唯一 `WorldQuery` | ✅ PASS |
| 自动测试 / 架构 Guard 基础 | ✅ PASS |
| **World Bounds 语义** | ✅ R2-R1 已修（实体显式 Bounds，WorldQuery 只消费不发明） |
| **唯一 Writer 访问控制** | ✅ internal + 调用点守卫（机器约束，非仅当前调用链） |
| **唯一索引全局 Guard** | ✅ 全 World 禁第二索引 + 写调用点白名单 |
| 真机验收 | ✅ PASS（13/13：1–12 真机通过，13 自动测试覆盖 UI N/A，Undo 日志 PASS） |

核心问题：初版把 `Position ± 0.5` 硬编码进 `WorldQuery.ToBounds`，等于"World 底层替所有实体发明 1×1×1 尺寸"——与"地球坐标写死进 Core"同一类错误。R2-R1 把尺寸归还给实体自身（显式 `SpatialBounds`），`WorldQuery` 只消费、不发明。

## 二、R2-R1 修正完成项（代码 + 自动验证）

| 项 | 状态 |
| --- | --- |
| `WorldEntitySnapshot` 增加本地 `Extent`（相对位置的盒）+ 绝对 `Bounds` 属性 | ✅ |
| `SpatialAabb.Translate` 纯几何辅助（Core） | ✅ |
| `EntityRegistry`/`GlobalWorld.Create` 透传显式 `extent`；缺省 = 零尺寸点（World 不发明尺寸） | ✅ |
| `WorldQuery.ToBounds` 改为消费 `entity.Bounds`，删除 `PointBounds` ±0.5 硬编码 | ✅ |
| `WorldQuery.Insert`/`Update`/`Remove`/`Rebuild` 收 `internal`（仅 `GlobalWorld` 权威链可写） | ✅ |
| `SceneStateOwner` 以占位实体工厂身份显式给 ±0.5 拾取代理；`CreateEntity` 增可选 `extent` 参数 | ✅ |
| 源码守卫升级：整个 `XuanYu.World/**` 禁 `new SpatialIndexOwner`，唯独白名单 `WorldQuery.cs` | ✅ |
| 测试 Oracle 诚实化：`WorldSpatialR1Oracle.Bounds` / `BruteBounds` 改用 `e.Bounds.WorldBounds`；`WorldSpatialQueryTests`/`RebuildTests` 显式给测试实体 ±0.5（尺寸属测试数据，非 World 默认） | ✅ |

### 收尾补丁（最终钉死唯一 Writer 与 Bounds 语义，2026-07-24）

R2-R1 主体已修"WorldQuery 不发明尺寸"，但 `internal` 仅挡跨程序集、挡不住 `XuanYu.World` 内第二个调用方；且需把 ±0.5 归属与 extent=0 语义正式钉死。本补丁不做任何生产行为改动，只加机器约束与语义测试：

| 项 | 状态 |
| --- | --- |
| 唯一 Writer 机器约束：`_query.Insert/Update/Remove/Rebuild` 调用点仅白名单 `GlobalWorld.cs`/`GlobalWorld.Query.cs`/`WorldQuery.cs`，其余 `XuanYu.World/**` 直接 guard fail | ✅ |
| 两个 Bounds 语义测试 `WorldEntityBoundsSemanticsTests`：默认点（`Min==Max==Position` + QueryBounds 点语义）/ 显式 ±0.5 绝对盒 | ✅ |
| ±0.5 归属裁定：占位实体自身 Bounds（情况 A 正确），非 Picking 容差伪装；未来 Pick Proxy 分离登记为非阻断选项 | ✅ |
| extent=0 语义冻结：零尺寸**点状空间足迹**，非"无空间信息"；保持"默认点 + 显式盒"两极，不引入 HasBounds/Optional | ✅ |

**±0.5 归属裁定（正式）：** `SceneStateOwner.MinimalSceneEntityExtent = ±0.5` 属**情况 A**——它是占位实体创建点为自己单位尺寸最小场景对象声明的**实体自身空间 Bounds**，不是 `WorldQuery` 替所有实体发明的通用默认，也不是单纯 Picking 容差伪装。代码注释已明确：若未来"拾取容差"与"空间 Bounds"分歧，那是 **Pick Proxy** 关注点，不属于 World Bounds；当前不分离（非阻断），仅登记。

**extent=0 语义冻结：** 缺省 `Extent = default(SpatialAabb)` = `Min==Max==Position`，表示"具有点状空间足迹的实体"，**不是**"该实体没有空间信息"。未来若需区分"点状实体"与"不参与空间查询的实体"，不在此轮提前引入 `HasBounds`/Optional/接口。

**冻结的 `QueryBounds` 正式语义**：返回"实体显式 `SpatialBounds` 与查询区域相交"的实体集合。不再因实现变化而改动 Oracle 掩盖语义——Oracle 直接引用实体真实 `Bounds`。

## 三、自动验证结果（R2-R1 后）

- `dotnet build` 9 项目 **0 warning / 0 error**
- `dotnet test`：**Core.Tests 67 passed / World.Tests 99 passed**（含 R2 新用例 + R2-R1 修正 + 收尾 2 语义测试）
- `scripts/arch-a-guard.ps1`：**EXIT=0**
- 5+100：所有改动文件 ≤100 行
- Picking 回归 `Moved_entity_hits_new_position_not_old_position`：**通过**（实体 ±0.5 由占位工厂提供，Picking 行为保持）

## 四、真机验收清单（用户执行，GUI 不可由 AI 运行）

逐项在 `run.bat` 启动的编辑器（Windows + Vulkan + RTX 3060）中确认：

1. ✅ Hierarchy 选择正常（真机）
2. ✅ Viewport Picking 命中正确实体（真机；G1 去 48px 守卫后相邻实体不再被 Gizmo 光环抢占）
3. ✅ Move Gizmo Preview（拖拽中不提交）
4. ✅ Move Gizmo Commit（释放后落实）
5. ✅ Escape Cancel（拖拽放弃，位置不变）
6. ✅ Undo 恢复上一位置（用户日志 PASS：15:17:00 撤销已执行）
7. ✅ Redo 重做下一位置
8. ✅ 跨 Region 移动后新位置正确、旧 Region 无残留
9. ✅ Frame Selected 框选目标
10. ✅ Frame All 框选全部
11. ✅ Resize / Swapchain 多代际重建无崩溃
12. ✅ 关闭生命周期干净释放
13. ✅ Create / Destroy 后 Picking 与 Query 一致 —— **UI N/A**：当前编辑器未开放实体创建/删除 UI 入口，无真机 UI 操作；由自动测试覆盖（见"四之一、第 13 项只读核查"），记为 **PASS（自动测试覆盖，UI N/A）**

**重点盯防（R2 核心价值）：**
- 实体从 A 移到 B 后，**点击旧 A 位置不可再选中**，**点击 B 位置必须选中**；
- Undo 后 Picking 跟随回到 A；Redo 后 Picking 跟随回到 B。

## 四之一、第 13 项只读核查（ARCH-WORLD-R2-R0D，2026-07-25）

背景：当前编辑器未开放"创建/删除实体"UI 入口（顶部"新建"为场景级命令、Hierarchy 无新增按钮、无右键删除菜单），10 个测试实体为启动预置。故第 13 项不要求真机 UI 操作，改为自动测试验证底层能力。本轮仅做只读核查，未改动任何生产代码。

| 核查点 | 证据 | 结论 |
| --- | --- | --- |
| 1. 实体注册/删除 API | `EntityRegistry.Create/Destroy`（`EntityRegistry.cs:17/39`）；`GlobalWorld.Create/Destroy`（`GlobalWorld.cs:28/41`） | ✅ 存在 |
| 2. 对应自动测试 | `EntityRegistryTests.Create_get_exists_and_destroy_single_entity`；`GlobalWorldTests.Global_world_owns_registry_lifecycle` + `Thousand_entity_smoke_keeps_stable_keys` + `Destroyed_entity_key_is_not_reused_by_next_create` | ✅ 存在 |
| 3. 删除后从空间索引/查询移除 | `GlobalWorld.Destroy`(:43-45) 顺序调用 `_registry.Destroy` → `_partition.Remove` → `_query.Remove`；`_query` 即 R2 收敛后的唯一 `SpatialIndexOwner` | ✅ 移除 |
| 4. 旧位置不可继续命中 | `WorldSceneSingleAuthorityTests.Case4_destroy_removes_entity_from_spatial_query`(:58-68)：销毁后 `RaycastSpatial(旧位).HasHit==false` 且 `QuerySpatial(旧位).Candidates` 为空；`Case2_move_must_not_leave_ghost_at_old_position`(:26-37) 覆盖移动幽灵 | ✅ 不可命中 |

**第 13 项裁定：PASS（自动测试覆盖，UI N/A）。** 底层 `EntityRegistry`/`GlobalWorld` 的"注册-删除-空间索引清除-查询失效"链路完整，且 `World.Tests` 99 passed/0 failed（含上述用例）在当前 HEAD 全绿。实体创建/删除编辑器 UI 作为独立功能轮开发，不在本轮范围、不为验收临时新增。

## 五、当前状态

**R2 CLOSED（2026-07-25，ARCH-WORLD-R2-R0D 收口提交）：主体 + R2-R1 修正代码完成、自动测试全绿、架构守卫通过；真机验收 13 项全部 PASS（1–12 真机操作通过、第 13 项自动测试覆盖 UI N/A），Undo 经用户日志 PASS。G1 P0（去 48px 守卫）作为第 2/13 项真机验收基础，随 R2 一并 CLOSED。**

停手条件（实施中已核查，均未触发）：为删 B 须大规模重写 Picking → 否；须改 Render Snapshot → 否；须重定义 Preview 语义 → 否；发现第三套空间事实 → 否；R2-R1 仅把尺寸职责从 `WorldQuery` 归还实体，未引入投机接口、未动 D1/D2/D3/O1/Camera/Large World/Streaming。
