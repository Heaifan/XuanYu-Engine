# ARCH-WORLD-R2 实施状态与真机验收

版本：`v0.2.19.3-rz`｜分支：`refactor/ARCH-WORLD-layer-boundary`｜方案 Gate：`docs/arch-world-r2-single-spatial-authority.md`

## 一、实施完成项（代码 + 自动验证）

| 项 | 状态 |
| --- | --- |
| `SceneStateOwner._spatialIndex` 字段删除 | ✅ |
| 4 处双写删除（构造 / `ApplyTransform` / `CreateEntity` / `DestroyEntity`） | ✅ |
| `SceneStateOwner.QuerySpatial` / `RaycastSpatial` / `SpatialRevision` 经 `_world` 兼容门面读唯一索引 | ✅ |
| `WorldQuery` 暴露 `Query(SpatialAabb/ray)` public + `Raycast(ray)` + `SpatialRevision` | ✅ |
| `GlobalWorld` 成为世界空间查询门面（委托 `_query`） | ✅ |
| 唯一索引实体 AABB 半长 0 → 0.5（与旧 Scene B 一致，保 Picking 行为） | ✅ |
| 源码守卫：`XuanYu.World/Scene/*` 禁止 `new SpatialIndexOwner` | ✅ |
| 自动测试 `WorldSceneSingleAuthorityTests`（6 用例） | ✅ |

## 二、自动验证结果

- `dotnet build` 9 项目 **0 warning / 0 error**
- `dotnet test`：**Core.Tests 67 passed / World.Tests 97 passed**（含 R2 新用例）
- `scripts/arch-a-guard.ps1`：**EXIT=0**
- 5+100：所有改动文件 ≤100 行
- Picking 回归 `Moved_entity_hits_new_position_not_old_position`：**通过**（R2 核心场景）

## 三、真机验收清单（用户执行，GUI 不可由 AI 运行）

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
10. Resize / Swapchain 多代际重建无崩溃
11. 关闭生命周期干净释放

**重点盯防（R2 核心价值）：**
- 实体从 A 移到 B 后，**点击旧 A 位置不可再选中**，**点击 B 位置必须选中**；
- Undo 后 Picking 跟随回到 A；Redo 后 Picking 跟随回到 B。

## 四、当前状态

**R2 代码完成、自动测试全绿、架构守卫通过。待用户真机验收（第三节）13 项 PASS 后，正式 CLOSED 并补收口提交。**

停手条件（实施中已核查，均未触发）：为删 B 须大规模重写 Picking → 否；须改 Render Snapshot → 否；须重定义 Preview 语义 → 否；发现第三套空间事实 → 否。
