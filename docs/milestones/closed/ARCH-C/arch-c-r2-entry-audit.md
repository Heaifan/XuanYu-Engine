# ARCH-C-R2-A：坐标与相机入口门审计

版本：v0.2.17.6-rz  
日期：2026-07-18 20:44:10  
类型：入口门审计  
范围：只读审计与规则冻结，不实现 Picking、Selection、Gizmo 或 Transform Preview。

## 1. 审计结论

`ARCH-C-R2` 当前不能直接进入鼠标 Picking 实装。仓库尚未存在长期正确的 `World -> View -> Projection -> NDC` 坐标事实，也没有渲染后端无关的 `ViewportState / CameraState` 契约。

因此禁止使用 Clip Space 或 Vulkan viewport 偏移反推命中的临时破解。R2 下一步必须先建立最小、长期正确、渲染后端无关的视口 / 相机变换契约。

## 2. 已读证据

- `ShaderBytecode.Vert.cs`：顶点着色器仍由 `gl_VertexIndex` 生成固定三角形，没有模型矩阵、视图矩阵、投影矩阵或顶点缓冲输入。
- `VulkanClearFrameOwner.Commands.cs`：R1 的 Position 同步通过 Vulkan viewport `X/Y` 偏移实现，不是真实世界坐标到裁剪空间变换。
- `VulkanNativeHost.Pointer.cs`：Pointer 输入能从物理像素除以 DPI 得到逻辑坐标，但只进入交互事务，不生成世界射线。
- `VulkanNativeHost.LayoutSync.cs`：NativeHost 已维护逻辑尺寸、DPI、物理尺寸和 Resize 合并，但这些事实没有形成 Picking 可消费的统一 `ViewportState`。
- 全仓扫描：未发现正式 `Camera`、`ViewMatrix`、`ProjectionMatrix`、`Ray`、`AABB` 或 `ViewportRevision` 契约。

## 3. Entry Gate 判定

| 入口条件 | 当前状态 | 裁定 |
| --- | --- | --- |
| R1 已封版 | 已通过并 Push | PASS |
| EntityKey 稳定 | `EntityId(1)` 真机稳定 | PASS |
| Scene 生命周期独立于 Vulkan | Resize / 关闭验收已证明 | PASS |
| 长期空间查询规则已冻结 | 本轮同步到宪法、dev-rules、arch-c-plan | PASS |
| Render / Picking 共用空间事实 | 尚不存在 | BLOCK |
| ViewportRevision / CameraState | 尚不存在 | BLOCK |

## 4. 受控架构债务

当前限制：渲染图像中的三角形位置不来自真实世界矩阵，而来自 viewport 偏移。

暂留原因：R1 目标是证明 SceneStateOwner 到 RenderSnapshot 的单向状态流，未承诺相机系统。

允许存在范围：R1 可继续作为低频测试实体渲染探针。

禁止扩散范围：不得用该 viewport 偏移路径实现 R2 Picking、R3 Selection、R4 Gizmo 或 R5 Transform Preview。

必须解决阶段：`ARCH-C-R2` 的 Picking 实装前。

阻断内容：屏幕坐标转世界射线、Ray-AABB 命中、Resize 后 Picking 一致性、最近命中判定。

## 5. R2 下一步最小正确实现

先建立渲染后端无关的空间事实契约：

```text
ViewportState
CameraState
ViewportRevision
WorldRay
SpatialBounds
SpatialQueryService
```

随后再进入：

```text
SceneStateOwner
-> Spatial Query Index
-> WorldRay
-> Ray-AABB
-> PickingResult
```

## 6. 测试风险

仓库当前没有正式测试项目。空间索引和坐标数学缺少自动测试宿主，这是 R2 风险。新增最小测试项目属于解决方案结构变更，需用户批准后才能执行。

建议测试覆盖：树插入、删除、移动更新、Ray 命中、Ray 未命中、最近命中、移动后旧 Bounds 失效、退化射线、NaN / Infinity、索引 Revision。
