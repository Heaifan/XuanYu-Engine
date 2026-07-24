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
| **World Bounds 语义** | ⚠ **NEED FIX（R2-R1 已修）** |
| **唯一 Writer 访问控制** | ⚠ **NEED VERIFY（R2-R1 已修 internal）** |
| **唯一索引全局 Guard** | ⚠ **NEED STRENGTHEN（R2-R1 已锁全 World）** |
| 真机验收 | ⏸ 暂缓（待 R2-R1 落地后执行 13 项） |

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

**冻结的 `QueryBounds` 正式语义**：返回"实体显式 `SpatialBounds` 与查询区域相交"的实体集合。不再因实现变化而改动 Oracle 掩盖语义——Oracle 直接引用实体真实 `Bounds`。

## 三、自动验证结果（R2-R1 后）

- `dotnet build` 9 项目 **0 warning / 0 error**
- `dotnet test`：**Core.Tests 67 passed / World.Tests 97 passed**（含 R2 新用例 + R2-R1 修正）
- `scripts/arch-a-guard.ps1`：**EXIT=0**
- 5+100：所有改动文件 ≤100 行
- Picking 回归 `Moved_entity_hits_new_position_not_old_position`：**通过**（实体 ±0.5 由占位工厂提供，Picking 行为保持）

## 四、真机验收清单（用户执行，GUI 不可由 AI 运行）

逐项在 `run.bat` 启动的编辑器（Windows + Vulkan + RTX 3060）中确认：

1. Hierarchy 选择正常
2. Viewport Picking 命中正确实体
3. Move Gizmo Preview（拖拽中不提交）
4. Move Gizmo Commit（释放后落实）
5. Escape Cancel（拖拽放弃，位置不变）
6. Undo 恢复上一位置
7. Redo 重做下一位置
8. 跨 Region 移动后新位置正确、旧 Region 无残留
9. Frame Selected 框选目标
10. Frame All 框选全部（本轮补，修正原"13 项/实列 11 项"不一致）
11. Resize / Swapchain 多代际重建无崩溃
12. 关闭生命周期干净释放
13. Create / Destroy 后 Picking 与 Query 结果一致（本轮补：新建/删除实体后，空间查询与拾取立即反映，无幽灵/缺失）

**重点盯防（R2 核心价值）：**
- 实体从 A 移到 B 后，**点击旧 A 位置不可再选中**，**点击 B 位置必须选中**；
- Undo 后 Picking 跟随回到 A；Redo 后 Picking 跟随回到 B。

## 五、当前状态

**R2 主体 + R2-R1 修正代码完成、自动测试全绿、架构守卫通过。待用户真机验收（第四节）13 项 PASS 后，正式 CLOSED 并补收口提交。**

停手条件（实施中已核查，均未触发）：为删 B 须大规模重写 Picking → 否；须改 Render Snapshot → 否；须重定义 Preview 语义 → 否；发现第三套空间事实 → 否；R2-R1 仅把尺寸职责从 `WorldQuery` 归还实体，未引入投机接口、未动 D1/D2/D3/O1/Camera/Large World/Streaming。
