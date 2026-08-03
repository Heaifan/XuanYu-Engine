# Milestone 9.0D — Move Gizmo 最终验收

## 一、阶段目标

本阶段目标是让 Move Gizmo 的 **X / Y / Z 三轴拖动达到可验收的稳定性和准确性**，尤其是 Z 轴移动必须从屏幕投影方案升级为射线约束方案。

核心需求一句话：

```text
鼠标在屏幕上移动 1 像素 → 无论视角如何 → Z 轴上的位移量应与 X/Y 轴一致。
```

---

## 二、当前状态（9.0C 审计结论）

### 当前实现：ScreenProjection（屏幕投影法）

所有三个轴共用同一算法：

```text
鼠标屏幕点
→ 投影轴方向到屏幕（单位向量 + 每世界单位像素数）
→ dot(鼠标增量, 屏幕方向) / 每世界单位像素数 = 世界位移
→ 初始位置 + 轴 × 世界位移 = 新位置
```

### 存在的问题

| 问题 | 说明 |
|---|---|
| Z 轴各向异性灵敏度 | 透视视角下 Z 轴投影长度远小于 X/Y，同样鼠标位移对应更大 Z 世界位移 |
| 无深度感知 | 屏幕投影无视 3D 深度信息，Z 轴不能利用深度约束 |
| `DragPlane` 模式未实现 | `AxisTranslationMode.DragPlane` 已定义枚举值但不被任何实际代码使用 |
| `AxisTranslationStart` 死代码 | 替代锚点构建器已定义但从不调用，且存在 ppu 计算不一致 |
| Plane 拖动工作正常 | XY/XZ/YZ 平面拖动已使用射线-平面求交，数学正确 |

---

## 三、推荐方案：射线约束轴拖动 (Ray-Constrained Axis Drag)

### 核心思路

对 X/Y/Z 轴拖动改用以下方案替代现有屏幕投影：

```text
鼠标屏幕点
→ 构造世界射线（现有 VulkanSceneRayBuilder 可用）
→ 构造约束平面：经过实体枢轴点，法线 = 视线方向（相机 Forward）
→ 射线-平面求交 → 得到 3D 交点
→ 将交点投影到目标轴上 → 得到轴上的位移分量
→ 初始位置 + 轴 × 分量 = 新位置
```

### 为什么这样做

1. **Z 轴与其他轴对称**：所有轴使用同一算法，不再依赖屏幕投影长度
2. **深度感知**：3D 交点携带深度信息，鼠标移动自然地转化为 3D 位移
3. **已有基础设施**：`VulkanSceneRayBuilder`、`PlaneTranslationStart` 中的射线-平面求交、投影逻辑均可复用
4. **Plane 拖动已验证**：XY/XZ/YZ 平面拖动使用同样的射线-平面求交原理，工程上已证明可行

### 核心数学：轴约束平面

不要用 `cameraForward` 直接作为平面法线——这会构造屏幕平面，轴不一定落在平面内。

正确做法：**约束平面必须包含目标轴，且尽量面向摄像机**。

```
axis = Normalize(targetAxis)          // UnitX / UnitY / UnitZ
view = Normalize(cameraForward)

// Gram-Schmidt：从视线中剔除沿轴的分量 → 垂直于轴、尽量朝向视线的法线
planeNormal = view - Dot(view, axis) × axis

// 如果 planeNormal 接近零（摄像机几乎沿轴观察），fallback 到 cameraRight / cameraUp
if Length(planeNormal) < ε:
    planeNormal = cameraRight - Dot(cameraRight, axis) × axis
    if Length(planeNormal) < ε:
        planeNormal = cameraUp - Dot(cameraUp, axis) × axis

planeNormal = Normalize(planeNormal)

// 约束平面：穿过 pivot，法线 = planeNormal
// 此平面必然包含 axis（因为 Dot(planeNormal, axis) == 0）
```

### 完整求解流程

```
鼠标屏幕点
→ 构造世界射线（VulkanSceneRayBuilder）
→ 构造轴约束平面（上述公式）
→ 射线与约束平面求交 → 得到当前 3D 交点
→ 从起始交点得到世界空间 delta = currentHit - startHit
→ delta 投影到目标轴 → axisDelta = Dot(delta, axis)
→ 新位置 = initialPosition + axis × axisDelta
```

关键优势：**位移量来自 3D 世界空间的射线-平面求交，与视角无关。约束平面包含目标轴，保证轴上的位移分量总是真实有效的**。

---

## 四、注意事项

### 边界情况

| 情况 | 处理 |
|---|---|
| 视线与轴垂直 | 射线与约束平面总能相交（除非射线平行于平面且不经过 pivot） |
| 透视 / 正交相机 | 射线构造已支持两种相机模式 |
| 射线与平面不相交（极罕见） | 返回无位移（NoChange） |
| 轴与视线完全对齐 | 射线-平面交点投影到轴上时可能接近零 → 此时等效于屏幕投影退化情况 |
| 多视图 / 不同视角 | 需要测试多个相机角度下的行为一致性 |

---

## 五、实施步骤

### 9.0D-0：审计当前 Move Gizmo 数学链路

已完成（见本节上面）。

### 9.0D-1：实现 Axis Drag 射线约束求解器

#### 任务

新增或修改以下文件：

```text
XuanYu.Engine.Editor/Transform/Translation/Axis/
├─ AxisTranslationSolver.cs              ← 修改：新增 DragPlane 求解
└─ AxisTranslationAnchor.cs              ← 修改：新增 DragPlane 锚点字段（planeNormal/planeOrigin）
```

或者新文件：

```text
XuanYu.Engine.Editor/Transform/Translation/Axis/
└─ AxisPlaneTranslationSolver.cs         ← 新增：纯函数，射线-平面求交 → 投影到轴
```

如果 `AxisTranslationSolver` 超过 100 行或逻辑复杂度上升，建议拆出独立求解器。

#### 求解器输入/输出

```csharp
public static class AxisPlaneTranslationSolver
{
    public static Vector3d Solve(
        Vector3d initialPosition,    // 初始实体位置
        Vector3d axis,               // 目标轴（UnitX/UnitY/UnitZ）
        Vector3d pivot,              // 枢轴点
        Ray3d startRay,              // 起始鼠标射线
        Ray3d currentRay,            // 当前鼠标射线
        Vector3d cameraForward,      // 视线方向
        Vector3d cameraRight,        // 视线右方（fallback 用）
        Vector3d cameraUp);          // 视线上方（fallback 用）

    // 内部：
    //   planeNormal = cameraForward - Dot(cameraForward, axis) × axis
    //   if too short: fallback to cameraRight / cameraUp
    //   startHit = startRay × plane(pivot, planeNormal)
    //   currentHit = currentRay × plane(pivot, planeNormal)
    //   deltaOnAxis = Dot(currentHit - startHit, axis)
    //   return initialPosition + axis × deltaOnAxis
}
```

#### 验收

* 所有视角下 X/Y/Z 三轴拖动手感一致
* 鼠标横向移动对应 X 轴位移
* 鼠标纵向移动对应 Y/Z 轴位移（取决于视角）
* Z 轴不再有异常灵敏度
* 不破坏现有 Plane 拖动（XY/XZ/YZ）

---

### 9.0D-2：修改轴拖动锚点构建器使用 DragPlane

#### 任务

修改 `AxisDragAnchorBuilder`：

```text
当前：构建 ScreenProjection 模式锚点
改为：构建 DragPlane 模式锚点
```

#### 改动点

`AxisDragAnchorBuilder.Build()`：

```csharp
// 改为使用 DragPlane 模式
var ray = VulkanSceneRayBuilder.TryBuild(...);
if (ray is null) return null;
return new AxisTranslationAnchor(
    initialPosition, axis, pivot,
    pixelsPerWorldUnit: 0,       // DragPlane 不使用像素比例
    screenDirection: default,    // DragPlane 不使用屏幕方向
    startPointerX: 0, startPointerY: 0,
    mode: AxisTranslationMode.DragPlane,
    // 新增字段:
    cameraForward: camera.Forward,
    startRay: ray);
```

#### 验收

* 拖动 X 轴：实体沿 X 轴移动
* 拖动 Y 轴：实体沿 Y 轴移动
* 拖动 Z 轴：实体沿 Z 轴移动
* 在视角旋转 90° 后：各轴行为仍然正确
* 正交视角下：各轴行为正常

---

### 9.0D-3：接入 WorldState + Inspector 刷新

#### 说明

当前管道已经完整：

```text
Gizmo 拖动 → TransformDragRoute.Move → AxisTranslationSolver.Solve
→ EntityTransformPreview（视觉层）
→ 松开鼠标 → EntityTransformCommit（WorldState + Dirty + 视口刷新）
→ Inspector 同步（通过 WorldState 变化自动触发）
```

9.0D 不需要改管道，只需要确认拖动结束后 Inspector 显示最新 Transform。

#### 验收

* Gizmo 拖动结束后，Inspector 的 Position 值刷新为最新值
* 保存 World 后 `.world.json` 包含 Gizmo 最后写入的位置
* 重新加载 World 后位置一致

---

### 9.0D-4：多视角验收测试

#### 必须新增的测试

| 测试 | 覆盖 |
|---|---|
| `AxisPlaneSolver_AllAxes_FromTopDown` | 俯视视角：X/Y/Z 各轴移动 |
| `AxisPlaneSolver_AllAxes_From45Deg` | 45° 视角：各轴移动 |
| `AxisPlaneSolver_AllAxes_FromSide` | 侧视视角：各轴移动 |
| `AxisPlaneSolver_AllAxes_Orthographic` | 正交相机 |
| `AxisPlaneSolver_ViewAlignedAxis` | 视线与轴对齐的退化情况 |
| `AxisPlaneSolver_SensitivityConsistent` | 同一鼠标位移在不同视角下得到的轴位移量一致 |

#### 验证方法

测试 `AxisPlaneTranslationSolver` 纯函数：

```csharp
// 给定固定的鼠标射线和相机方向
var result = AxisPlaneTranslationSolver.Solve(
    initialPosition, axis, pivot, ray, cameraForward);

// 验证位移方向正确
Assert.True(Vector3d.Dot(result - initialPosition, axis) > 0);

// 验证位移量合理
Assert.True((result - initialPosition).Length < maxExpected);
```

---

### 9.0D-5：清理死代码

#### 说明

审计发现以下死代码：

```text
AxisTranslationStart.cs — 从未被调用的替代锚点构建器
```

如果 9.0D 确认 `DragPlane` 方案可行，可以安全删除此文件。

#### 注意

删除前需确认没有其他代码间接引用此类。如果删除后有编译错误，说明它并未真正死亡。

---

## 六、本阶段明确不做

```text
Rotate Gizmo（9.0E）
Scale Gizmo（9.0E）
Inspector 数据链路改造（9.0C 已完成）
World 保存/加载改造（9.0B 已完成）
```

---

## 七、验收标准

```text
✅ X/Y/Z 三轴拖动使用射线约束方案（非屏幕投影）
✅ 约束平面使用 Gram-Schmidt 法线（包含轴、面向摄像机），非简单 cameraForward
✅ cameraForward 与 axis 接近共线时 fallback 到 cameraRight/cameraUp
✅ Z 轴不再有各向异性灵敏度问题
✅ 透视视角下拖 X/Y/Z 方向稳定
✅ 正交视角下拖 X/Y/Z 方向稳定
✅ 顶视图拖 X/Y 正常
✅ 前视图拖 X/Z 正常
✅ 侧视图拖 Y/Z 正常
✅ 倾斜 45° 视角拖 Z 正常
✅ 轴接近朝向摄像机时不跳变、不爆位移（可降灵敏度）
✅ Plane 拖动不受影响
✅ Gizmo 拖动结束后 Inspector Position 同步
✅ 保存 World → 加载 World 后位置一致
✅ 死代码已清理或标记
✅ 新增多视角验收测试
✅ build 0 error
✅ 有效验收通过，预存故障非本阶段引入
✅ 生产文件 ≤100 行
✅ 测试文件 ≤180 行
✅ 单目录 ≤5 文件
```

---

## 八、提交建议

### commit 1

```text
feat(gizmo): 实现射线约束轴拖动求解器
```

内容：

```text
AxisPlaneTranslationSolver
AxisTranslationAnchor 扩展 DragPlane 字段
AxisDragAnchorBuilder 切换为 DragPlane 模式
多视角测试
```

### commit 2

```text
chore(gizmo): 清理 AxisTranslationStart 死代码
```

内容：

```text
删除未使用的 AxisTranslationStart.cs
确认无编译错误
```

### commit 3

```text
test(gizmo): 添加多视角轴拖动验收测试
```

内容：

```text
各视角各轴位移测试
灵敏度一致性测试
退化情况测试
```

如果 commit 1 中已包含测试，可以不单独拆 commit 3。

---

## 九、执行提示

开始执行 Milestone 9.0D — Move Gizmo 最终验收。

本阶段核心目标：
将 Move Gizmo 的 X/Y/Z 轴拖动从屏幕投影方案（ScreenProjection）升级为射线约束方案（DragPlane），使 Z 轴在各视角下具备与 X/Y 轴一致的拖动手感。

核心改动点：
1. 新增或修改轴拖动求解器，使用射线-轴约束平面（包含轴、面向摄像机）求交后投影到目标轴。
2. 修改 AxisDragAnchorBuilder 使用 DragPlane 模式。
3. 确认现有 Plane 拖动不受影响。
4. 清理 AxisTranslationStart 死代码。
5. 新增多视角测试，覆盖各相机角度下的轴行为。

轴约束平面核心公式（非 cameraForward 平面）：
```
axisDir = Normalize(axis)
view = Normalize(cameraForward)
planeNormal = Normalize(view - Dot(view, axisDir) × axisDir)
// 长度过小时 fallback 到 cameraRight / cameraUp 做 Gram-Schmidt
```

范围限制：
不碰 Rotate Gizmo、Scale Gizmo、Inspector、World 保存/加载。

工程约束：
1. 单文件 ≤100 行。
2. 单目录 ≤5 文件。
3. 禁止 Manager/Helper/Utils/Processor/Factory/Creator。
4. 错误提示、测试说明尽量中文化。
5. build/test 后提交并推送。
6. 验收报告写："有效验收通过：X/Y，剩余预存故障非本阶段引入。"
