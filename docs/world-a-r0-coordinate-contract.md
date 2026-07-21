# WORLD-A-R0 坐标契约与方向轴审计

版本：`v0.2.18.1-rz`

## 唯一坐标事实

- 世界空间采用右手笛卡尔坐标系：`+Z = Up`、`XY = 水平面`，且 `X × Y = Z`。
- 世界 X/Y 是固定水平轴，但世界空间不定义唯一 Forward。Camera Forward、Object Local Forward、Asset Import Forward 与 Geographic Tangent Forward 必须分别定义。
- Transform 的 Position 与 Move Gizmo 都使用世界 X/Y/Z；沿某轴正向拖动只增加对应分量。
- Camera 的 `Forward` 是世界空间观察方向，不等同于世界 X/Y；`Right = Forward × Up`，构造时把 Up 正交化。右手 View Space 中观察方向落在 `-Z`。
- 屏幕逻辑坐标原点位于左上角：X 向右增加，Y 向下增加。DPI 只负责逻辑/物理尺寸换算，不改变方向。

## Vulkan 转换规则

Core 生成右手 View 与深度范围 `[0, 1]` 的标准 Perspective，不携带 Vulkan 语义。Vulkan 使用正高度 Viewport，因此唯一的 Y 方向转换发生在 `Render.Vulkan` 组装 Push Constant 的边界副本：

```text
VulkanProjection = FlipClipY(CoreProjection)
```

该转换只翻转 Projection 输出的 Clip Y 列，不回写 Core Projection。Vulkan Push Constant 继续使用既有矩阵内存适配；Shader、Viewport、Scissor、Surface、Swapchain 与 Present 生命周期不承担第二次翻转。

## Picking 与 Gizmo

- 屏幕 X 到 NDC：`2u - 1`。
- 屏幕 Y 到 Core NDC：`1 - 2v`；这是左上屏幕原点到右手 NDC 的正常映射，不是 Vulkan 补丁。
- Picking Ray 使用同一 `ViewProjectionState` 的逆矩阵。
- Gizmo 端点使用同一 `ViewProjectionState.ProjectWorldPoint`。因此显示、命中、拖动约束和 Picking 不得各自维护坐标补丁。

## 全球与局部坐标边界

R0 只冻结边界，不实现地球、地图或 Streaming：

```text
带明确 Datum 的经纬度/高程
→ 地心地固全局双精度坐标
→ Region 双精度局部坐标
→ Camera-relative 双精度坐标
→ 边界检查后的 Vulkan float
```

现有 ARCH-C `Vector3d` Scene Position 只代表当前 Region/编辑局部空间，不得解释为“整个地球的单一 float3”。地理坐标转换、Datum、Region 原点和 Floating Origin 策略留给 WORLD-A-R2；在这些事实存在前禁止伪造球形地球转换。

## 审计结论

| 链路 | 修复前实际约定 | R0 预期约定 | 裁定 / 修复 |
| --- | --- | --- | --- |
| World / Math | `YawRotation` 隐含 Y-Up / XZ 平面，并把方向命名为 Forward | RH / Z-Up / XY 水平，不定义世界唯一 Forward | 不一致；改为绕 +Z 旋转显式局部 X/Y 基轴 |
| Transform | Position 的 X/Y/Z 分量直接写世界轴；尚无正式 Rotation / Scale | 现有 Position 服从世界轴；未来 Rotation 正向服从右手规则 | Position 一致；Rotation / Scale 不在本轮伪造 |
| Camera | 默认 Up=`+Y`，Position=`(4,3,-5)` | Camera Forward 显式；默认编辑相机 Up=`+Z` | 不一致；默认相机改为 Z-Up 斜视姿态 |
| View | `CreateLookAt` 右手 View，Camera Forward 落入 View `-Z` | 同左 | 一致；保留并正交化 Right/Up |
| Projection | Core Projection 为标准右手；Vulkan 未做 Clip Y 边界转换 | Core 保持纯净，Vulkan 在 Render Boundary 转换副本 | Vulkan 边界缺失；新增 Render.Vulkan 转换 |
| Screen / NDC | `ProjectWorldPoint` 把 NDC +Y 映射到屏幕下方；Picking 则正确映射到屏幕上方 | 左上屏幕原点与 Core NDC 双向互逆 | 不一致；修正 World→Screen 的 Y 映射 |
| Picking | 与 Render 共用 Camera/ViewProjection；Screen→NDC 本身正确 | 与 Render 使用同一 Core 矩阵事实 | 保留；不加入 Vulkan 翻轴 |
| Gizmo Global | 世界 Unit X/Y/Z，经 ViewProjection 投影 | Global 服从 World Basis | 基轴正确；随统一矩阵修正显示方向 |
| Gizmo Local | 当前 ARCH-C 尚无 Rotation 事实与 Local 模式 | Local 应服从 Entity Local Basis | 能力缺口；禁止在 R0 用假 Rotation / 新移动系统冒充完成 |
| Direction Axis | 当前可见三轴就是选中实体 Move Gizmo，无独立角落方向控件 | 颜色、轴名、空间方向与 Camera/World 同源 | 症状来自 Camera/Projection，不另加 UI 补丁 |

修复前，正高度 Vulkan Viewport 直接消费 Core Projection，导致 Camera Up 显示向下；CPU `ProjectWorldPoint` 也把 NDC +Y 映射到屏幕下方，而 Picking 的 Screen→NDC 映射正确指向 Camera Up，三者不一致。R0 保持 Core Projection 与 Picking 纯净，只修 World→Screen 映射，并在 Vulkan Push Constant 边界转换 Projection 副本。审计还发现 `YawRotation.ForwardOnXZPlane` 把 Y-Up 和模糊 Forward 写入 Math，本轮改为绕世界 `+Z` 在 XY 平面旋转显式局部基轴。

R0 将 Y 翻转收敛到 Projection，并同步 Picking 的 NDC Y 映射。未新增 Ground、Grid、Skybox、Terrain、PBR、世界地图或 WORLD-A-R1 之后的能力。

## 验收证据

自动测试冻结以下事实：

- 世界 `X × Y = Z`、Z-Up 与 XY 水平旋转。
- Camera Forward/Right/Up 正交约定。
- 默认 Z-Up 相机的 Camera Up 投影到屏幕上方。
- 屏幕四角射线、中心射线、Resize、非零 Viewport Origin 与 DPI 保持一致。
- 世界点投影后再反投影，射线仍指向原世界点。
- Move Gizmo 三轴显示、命中和拖动只修改对应世界轴。

Resize / DPI 的数学契约由自动测试覆盖；真实 Vulkan Present、窗口 Resize/DPI、交互拖动及关闭释放链仍需按本文件契约进行真机复验后，才能把 R0 标记为真机验收完成。
