# REVERSED-Z-PROTOTYPE-R1 STATUS

## 结论

Current: `D32Sfloat + Forward-Z`。

Prototype: `D32Sfloat + Reverse-Z`，仅使用 Vulkan NDC `Z=0..1`，未修改正式 Renderer。

Recommendation: **REVERSED-Z RECOMMENDED**，但本报告只批准进入后续分阶段迁移评审，不批准直接切换。

原因是：在 Near=`0.05m`、Far=`max(Near*10, Distance*4)`、D32Sfloat 的实际相邻浮点深度计算中，Reverse-Z 在全部 1/10/20/50/100/200/500km 采样点都取得更小的世界空间深度分辨率。这个结论来自玄域自己的公式、相机范围和浮点量化，不依赖外部引擎结论。

## Prototype Contract

透视 Reverse-Z：`depth = (Far*Near/z - Near)/(Far-Near)`；正交 Reverse-Z：`depth = (Far-z)/(Far-Near)`。

因此 Near→1、Far→0，距离增加时深度严格递减；有限 Far 的端点和 Vulkan 0..1 范围均由测试覆盖。无限 Far 的数学极限是 `depth = Near/z`，可减少有限 Far 端点耦合，但会改变裁剪、拾取和远距语义，本轮 **DEFER**，不是 R1 采用方案。

## Precision Comparison

每个采样点取投影深度转换成 `float` 后的相邻可表示值，再反解为世界距离；Improvement Ratio=`Forward resolution / Reverse resolution`。

| Distance | Forward resolution | Reverse resolution | Improvement Ratio |
|---:|---:|---:|---:|
| 1km | 1.3769128m | 0.0000990267m | 13,904.46x |
| 10km | 110.23522m | 0.000469530m | 234,777.76x |
| 20km | 719.90716m | 0.000658990m | 1,092,439.7x |
| 50km | 1,799.7535m | 0.00236425m | 761,235.61x |
| 100km | 18,196.831m | 0.00672219m | 2,706,980.9x |
| 200km | 75,164.809m | 0.0154381m | 4,868,800.1x |
| 500km | 1,500,000m | 0.0316381m | 47,411,240x |

20km Improvement: `1,092,439.7x`。

50km Improvement: `761,235.61x`。

100km Improvement: `2,706,980.9x`。

200km Improvement: `4,868,800.1x`。

500km Improvement: `47,411,240x`。

这些是该采样策略和单个相邻浮点值的结果，不代表所有几何、MSAA、Early-Z 或屏幕覆盖条件下的最终视觉收益。

## gl_FragDepth Migration Surface

已扫描 `XuanYu.Render.Vulkan/Shaders/*.frag`。当前写入者：

- `editor_world_reference_grid.frag`：由 `viewProjection` 计算深度；Reverse-Z 后公式仍需保持 0..1，但深度方向变为 Reverse-Z。当前无 bias。
- `editor_view_plane_grid.frag`：当前 `gl_FragDepth = depth - bias`；Reverse-Z 后需要改为向更大深度值偏移（通常为 `depth + bias`），并重新验证 clamp 和共面稳定性。
- `editor_world_axes.frag`：同样当前 `depth - bias`，需要反向 bias，并重新验证掠射角/共面效果。

Shader 不在本任务中修改。

## REVERSE-Z MIGRATION SURFACE

### Pipeline / attachment

- `XuanYu.Render.Vulkan/Render/VulkanDepthAttachment.cs`：已是 `Format.D32Sfloat`，格式本身可保留。
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Resources.cs`：深度 attachment load 为 Clear，迁移时需确认 Clear=0。
- `XuanYu.Render.Vulkan/Render/ClearFrame/VulkanClearFrameOwner.Commands.cs`：当前 `Depth = 1.0f`，迁移为 0。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Depth.cs`：LessOrEqual→GreaterOrEqual。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Fullscreen.cs`：全屏深度管线 LessOrEqual→GreaterOrEqual。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.GridLine.cs`：网格管线 LessOrEqual→GreaterOrEqual，并复核 Rasterizer DepthBias。
- `XuanYu.Render.Vulkan/Pipeline/VulkanGraphicsPipelineOwner.Terrain.cs`：Terrain 深度测试 LessOrEqual→GreaterOrEqual。
- 任何其他 `CompareOp.Less*`、`ClearDepth=1`、DepthTest/DepthWrite 组合都必须按 Pass 分类复核，不能只改共享 helper。

### Picking / Unprojection

XY NDC 不变，屏幕射线的 XY 方向不变；透视/正交的世界射线几何方向也不因深度方向反转而改变。改变点是深度端点语义：当前 `WorldRayFactory` 用 NDC Z=0 取 near、Z=1 取 far；Reverse-Z 必须交换为 near=1、far=0。其双精度分支仍应保持相机 Forward 和 NearPlane 的数学定义。

必须回归：`ViewProjectionState.TransformPointToWorld`、`InverseViewProjection` 使用处、`WorldRayFactory`、`ViewportPickingService`、`UiVm.Picking`、`MapSurfacePicker`、Scene projection adapter、Grid/axis/view-plane 的 inverse-view-projection 射线重建，以及 Terrain Probe/导航射线等所有以深度端点反解世界点的路径。

结论：Reverse-Z 不改变 XY NDC 或 Ray Direction，但会改变 Near/Far reconstruction；Picking / Unprojection Impact = **中等，必须专项回归**。

## Migration Plan

| 阶段 | 范围 | 独立验收 |
|---|---|---|
| RZ-A | Projection Math | 透视/正交端点、单调性、有限 Far、逆变换和 0..1 测试通过 |
| RZ-B | Depth Clear / Compare | D32 attachment Clear=0、所有正式深度 Pass GreaterOrEqual，静态契约通过 |
| RZ-C | Procedural gl_FragDepth | 三个 Shader 的方向和 bias 通过 shader contract、共面回归 |
| RZ-D | Picking / Unprojection | 屏幕射线、Picking、Navigation Ray、Terrain Probe 端点与远距往返通过 |
| RZ-E | Full Renderer Regression | 完整 Build/Test、真实 Renderer、Grid/Terrain/Overlay/Swapchain 和运行时视觉验收 |

每阶段都应保持可独立回退；R1 不执行这些正式修改。

## Prototype Tests

`XuanYu.Core.Tests/Camera/ReverseZProjectionPrototypeTests.cs` 覆盖：

- ReversePerspectiveNearMapsToOneTest
- ReversePerspectiveFarMapsToZeroTest
- ReverseOrthographicNearMapsToOneTest
- ReverseOrthographicFarMapsToZeroTest
- ReverseDepthMonotonicityTest
- ReverseD32FarPrecisionBeatsForwardTest
- ReverseProjectionInverseRoundTripTest
- VulkanZeroToOneContractTest

`XuanYu.World.Tests/Render/ReverseZDepthContractPrototypeTests.cs` 覆盖当前 Forward 合同、Vulkan Reverse 合同、全部 Fragment Shader 的 `gl_FragDepth` 写入者，以及 Projection/Inverse/Picking/Ray 审计入口。

## Verification Status

- Perspective: **PASS（数学原型）**
- Orthographic: **PASS（数学原型）**
- Finite Far: **PASS（数学原型）**
- Infinite Far: **DEFER**
- World prototype tests: **PASS**（4 tests）
- Core prototype tests: **PASS**（8 tests）。
- Build: **PARTIAL**，已完成两个受影响测试项目的定向编译/测试；未执行完整 Solution Build，因此不宣称全量 Build 通过。
- `git diff --check`: **PASS**（退出码 0；仅有工作区既有文件的换行提示）。
- Production Files Modified: **0 by this task**（本任务仅新增两个测试文件和本报告；工作区其他 Lane 的生产改动未触碰）。

## Final Status

**REVERSED-Z PROTOTYPE R1 COMPLETE**

原型问题已回答，正式迁移仍须按 RZ-A 至 RZ-E 分阶段评审；本轮没有接入正式 Renderer，也没有修改 CameraState、CameraNavigation、Projection Matrix、Depth Clear、Depth Compare、Shader、Grid、Terrain 或 Swapchain。
