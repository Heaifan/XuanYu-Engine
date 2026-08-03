# ARCH-WORLD-R2-G1 只读审计：Move Gizmo HitTest 输入抢占

> 状态：**只读审计完成，未改动任何生产代码、未缩小任何常量。**
> 范围：仅 Editor/Gizmo 输入层缺陷轮。R2 主链
>（`GlobalWorld` → `WorldQuery` → 唯一 `SpatialIndexOwner`）、Camera、`FrameSelected`、
>`ViewProjection` **不在本轮回填范围**，证据已将其排除（见下文“故障域隔离”）。
> 版本保持 `v0.2.19.3-rz`。

---

## 0. 故障域隔离结论（证据闭环）

真机三图对比已证明：

| 工具 | 点击实体 | 日志 | 结论 |
|---|---|---|---|
| 选择 | 实体04 / 05 | `视口拾取完成；结果=实体编号(4/5)` → `选择已提交` | **Picking 整链正常** |
| 移动 | 偏离可见轴的位置 | `变换捕获开始` → `提交捕获` → `移动工具会话结束`（无 Picking 日志） | **Gizmo 吞掉了点击** |

同一相机、同一画面，切换工具即复现差异 ⇒ 故障域收敛到：

```
Editor Input
  ↓
Move Gizmo HitTest
  ↓
Transform Capture
```

`Camera / FrameSelected / ViewProjection / WorldQuery / SpatialIndex` 在“选择”工具下已被
证伪为病因，**禁止**为 G1 去动它们（除非 G1 修完后仍复现坐标偏移）。

---

## 1. 输入路由（Gizmo 优先于 Picking，命中即抢占）

`XuanYu.Editor.UI/Viewport/Vulkan/VulkanNativeHost.Pointer.cs`

```csharp
protected override void OnPointerPressed(PointerPressedEventArgs e)
{
    ...
    if (TryBeginGizmo(vm, e.Pointer.Id, point.Position.X, point.Position.Y))
    {
        e.Pointer.Capture(this); e.Handled = true; return;   // ← 命中即 return，Picking 不再执行
    }
    ReportPointerPicking(vm, point.Position.X, point.Position.Y); // ← 仅 Gizmo 未命中才走到这里
}
```

`TryBeginGizmo`（`VulkanNativeHost.Gizmo.cs:5-9`）→ `TryBeginMoveGizmoCapture`
（`UiVm.MoveGizmo.cs:13-51`）：

```csharp
var axis = layout.HitTest(x, y) ?? layout.GuardHitTest(x, y);  // 18px 命中，未中则 48px 守卫兜底
if (axis is null) return false;                                // 未命中 → 返回 false → 走 Picking
_transformSession.Begin(...);                                   // 命中 → 抢占，Picking 被吞
```

**结论**：Gizmo HitTest 一旦返回非空轴，PointerDown 即被 `return` 消费，Viewport Picking
永远拿不到这次点击。这正是“黄色实体点不到、被 Gizmo 抢走”的直接机制。

---

## 2. 四个问题的回答

### Q1. 每个轴的 Hit Shape 到底是什么？

**有限线段胶囊（capsule）**，不是无限线、不是整屏。

- 几何：`MoveGizmoLayout.Project`（`MoveGizmoLayout.cs:16-25`）把每根轴
  `origin → origin + AxisVector*AxisLength(=1.2 世界单位)` 投影成屏幕 `MoveGizmoSegment`。
- 命中：对每段用 `Distance`（点到**有限段**的最近距离，见 Q3）做垂直距离判定，
  半径 = `width` 像素；再叠加 `Alignment >= 0`（轴前方 90° 半球）过滤。

所以是“从原点出发、到箭头尖端为止、半径 `width` 像素的 3 个粗胶囊（星形）”。

### Q2. Hit tolerance 是多少像素？

`MoveGizmoLayout.cs:8-10`：

```csharp
public const double AxisLength = 1.2;
public const double HitWidth   = 18.0;   // 主命中半径
public const double GuardWidth = 48.0;   // 守卫兜底半径
```

- 主路径 `HitTest` 用 **18px**；
- `UiVm.MoveGizmo.cs:27` 的 `?? layout.GuardHitTest(x, y)` 让**未命中 18px 时再试 48px**。
  即**有效捕获半径最高达 48px**，且对 3 个轴的整条星形都生效。
- 可见轴体渲染约 **2–3px** 线宽。

**事实：命中区是可见视觉的 6–16 倍；48px 守卫是最严重的“隐形光环”——它本意是“差点没点中轴也别误选场景”，
但 48px 在缩放拉远 / 实体密集时足以罩住相邻实体，直接抢走它们的 Picking。** 这就是 P0。

### Q3. HitTest 是否只限垂直距离、却没限轴向长度（无限射线 bug）？

**不是无限射线。轴向长度已被限制。**

`MoveGizmoLayout.cs:60-70` 的 `Distance`：

```csharp
var t = (((x - segment.Start.X) * dx) + ((y - segment.Start.Y) * dy)) / length2;
t = Clamp(t, 0, 1);                 // ← 关键：投影参数 clamp 到 [0,1]
var px = segment.Start.X + (t * dx);
var py = segment.Start.Y + (t * dy);
return Sqrt((x - px)^2 + (y - py)^2);
```

- 点在箭头尖端之外沿轴继续延伸：`t` 被 clamp 成 1，最近点 = 箭头尖端，
  垂直距离 = “点到尖端的距离”；若超过 `width` ⇒ **MISS**。
- 因此“点在轴延长线很远仍 HIT”的无限射线 bug **不存在**。Q3 可以排除，
  不必为它写额外算法——问题纯粹是 Q2 的容差过大。

### Q4. HitTest 用的屏幕坐标尺度是否正确（DPI / Physical vs DIP）？

**一致，是 DIP，无尺度 bug。**

- Avalonia 路径：`point.Position.X/Y` 是相对控件的 **DIP** 坐标
  （`VulkanNativeHost.Pointer.cs:62,65`）。
- Native 路径：`x = message.PhysicalX / dpi; y = message.PhysicalY / dpi`
  （`VulkanNativeHost.Pointer.cs:12-14`）——明确折回 **DIP**。
- 视口状态：`CaptureViewportState`（`VulkanNativeHost.Picking.cs:20-36`）
  取 `width = (int)Bounds.Width`（Avalonia 控件 DIP 尺寸），
  `LogicalX=0, LogicalY=0, LogicalWidth=width, LogicalHeight=height`。
- 投影：`ViewProjectionState.ProjectWorldPoint`（`ViewProjectionState.cs:73-82`）
  用 `Viewport.LogicalX/Y/Width/Height` 生成 `ScreenPoint`，即同一 DIP 空间。
- 而 Picking 路径（`PickViewportPointer` → `ViewportPickingService.Pick`）用的也是
  **同一组 logical x/y + 同一 ViewportState**。

⇒ Gizmo HitTest 与 Viewport Picking **同源 DIP**，不会出现“一边 DIP 一边 Physical”的漂移；
这也解释了为何“选择”工具 Picking 永远精准。**Q4 排除**，与“禁止动 Camera/ViewProjection”一致。

---

## 3. P0 根因（一句话）

`HitWidth=18` + `?? GuardHitTest(48)` 让选中实体周围形成半径最高 48px 的 3 叶命中星，
而可见轴只有 2–3px；缩放拉远 / 实体靠近时，这圈隐形光环罩住邻居并抢占其 Picking。

## 4. P1 根因（零位移单击仍 Commit）

`UiVm.Interaction.cs:47-67` `CommitInteraction`：

```csharp
var transformCommitted = _transformSession.TryCommit(snap.SessionId, _sceneState, out var commit);
if (transformCommitted) { RecordTransformHistory(commit); ... }   // 提交即写历史
```

`TransformSession.TryCommit`（`TransformSession.cs:42-54`）：

```csharp
var position = Preview?.Position ?? StartSnapshot.Transform.Position;  // 没拖动 → Preview=Start
End();
commit = scene.CommitPositionWithResult(position);                    // 无“是否真移动”闸门
return true;
```

按下→松开（无 PointerMove）：`Preview` 停在 `Begin` 时设的 `Start` 位置，
`position == Start` ⇒ 提交**未变化的同一坐标** ⇒ `RecordTransformHistory` 写入一条无用 Undo。
**没有任何“位移 ≥ ε 才提交”的判定**，故零位移单击也会产生 Transform Commit + 历史记录。

---

## 5. 修复方向（仅建议，待用户拍板后执行；本轮不动代码）

### P0：命中几何与视觉几何同源

原则（用户已定）：**看得到的地方 ≈ 点得到的地方**，禁止“另一套差不多的数学”。

1. 引入单一视觉线宽真源 `GizmoVisualLineWidth`（需到 Vulkan 渲染层定位实际绘制线宽，
   初判 2–3px），命中半径 = `GizmoVisualLineWidth + HitMargin`，
   `HitMargin` 为小额 UX 容差（建议 4–5px，最终由用户拍板）。
2. **移除或大幅收窄 `?? GuardHitTest(48)` 兜底**：守卫只应是视觉线宽附近的微小保险，
   不应是 48px 抓取半径。候选：删除该 fallback，或 `GuardWidth` 收到 ~10px 且语义改为
   “贴着可见轴也别误选场景”，不再形成大星。
3. 可选增强：为箭头头部（cone/box）增加与绘制形状对齐的显式 Hit Shape，
   使“点箭头尖”与“点轴杆”行为一致。

### P1：零位移 No-op

在 `TransformSession.TryCommit` 提交前比较 `position` 与 `StartSnapshot.Transform.Position`：
- 若 `Distance(position, Start) < Epsilon` ⇒ 改走 `TryCancel`（或返回 false、不调用
  `CommitPositionWithResult`、不 `RecordTransformHistory`），即 No-op。
- 保持变更集中在 `TransformSession`（World 层，D1 债务范畴），便于 `World.Tests` 单测。

### 测试（补 4 + 1）

- 现有 `XuanYu.Core.Tests/Gizmo/MoveGizmoLayoutTests.cs:14-15` 锁死了 `18/48`，
  `Guard_hit_keeps_visible_axis_from_falling_to_scene_picking` 锁死了 48px 守卫行为
  ——改容差时必须同步更新这两处。
- 新增（`MoveGizmoLayoutTests` 同源）：
  1. 轴本体（线段中点）命中期望轴；
  2. 轴外明显超过容差 ⇒ MISS ⇒ 调用方应继续 Viewport Picking（即 `TryBeginMoveGizmoCapture` 返回 false）；
  3. 轴延长线之外（超过箭头尖端）⇒ MISS（抓“误把延长线当地轴”）；
  4. Resize / Frame Selected / Frame All 后，同一 Gizmo 的 Hit Shape 与视觉位置仍一致。
- 零位移（`XuanYu.World.Tests/Transform/TransformSessionTests.cs` 同源）：
  5. `Begin` 后无有效移动即 `TryCommit` ⇒ 不产生有效 commit / 不写历史（应 No-op/Cancel）。

---

## 6. 待办（下一步，需用户批准才执行）

- [ ] 定位 Vulkan Gizmo 实际绘制线宽作为 `GizmoVisualLineWidth` 真源；
- [ ] 按上述 P0 方案改 `MoveGizmoLayout` 容差 + 去 48px 守卫兜底；
- [ ] 按 P1 方案改 `TransformSession.TryCommit` 零位移 No-op；
- [ ] 更新/新增上述测试，确保 Build 0W0E + 测试绿 + `arch-a-guard` 通过；
- [ ] 真机复测移动/Undo/Redo 后 Picking 与 A/B 实体 Bounds 探针（沿用 R2 清单第 4/6/7 项重点盯防）。

---

## 7. 修复实现与验证结果（R0B，2026-07-25 追加，原证据第 0–6 节未改动）

> 本节为 G1 最小 P0 修复落地记录，仅追加，不修改上方只读审计证据（第 0–6 节）。

### 7.1 修复原则落地
- **命中几何与可见几何同源**：新增可见线宽真源 `GizmoVisualLineWidth = 2.0`（DIP，与 Vulkan 顶点着色器生成的 Gizmo 几何同尺度，审计实测约 2–3px）；新增有限、显式交互容差 `HitMargin = 5.0`（DIP，仅补偿指针精度）。
- **命中半径派生，非魔法数字**：`HitWidth = (GizmoVisualLineWidth / 2.0) + HitMargin = 6.0`。看得到 ≈ 点得到；不再存在第二套半径。
- **移除 48px 隐形大范围抢占**：删除 `GuardWidth = 48.0` 常量与 `GuardHitTest` 方法；`UiVm.MoveGizmo.TryBeginMoveGizmoCapture` 的输入分流由 `layout.HitTest(x, y) ?? layout.GuardHitTest(x, y)` 改为仅 `layout.HitTest(x, y)`。命中失败即 `return false`，PointerDown 落到 `ReportPointerPicking` → 场景 Picking 正常进行。

### 7.2 修改范围（仅 G1 P0，未越禁区）
- `XuanYu.Core/Gizmo/MoveGizmoLayout.cs`：删 `GuardWidth` / `GuardHitTest`；增 `GizmoVisualLineWidth` / `HitMargin`；`HitWidth` 改为派生常量。
- `XuanYu.Editor.UI/Vm/UiVm.MoveGizmo.cs`：第 27 行移除 `?? GuardHitTest(x, y)` 兜底。
- `XuanYu.Core.Tests/Gizmo/MoveGizmoLayoutTests.cs`：锁 `HitWidth` 派生关系 + `< 12` 防回归上限，替换原 48/Guard 断言；将 `Hit_radius_follows_visible_geometry_with_explicit_margin` 移入新建部分类以保持 5+100。
- `XuanYu.Core.Tests/Gizmo/MoveGizmoLayoutG1Tests.cs`（新建，部分类）：`Removed_wide_guard_no_longer_captures_far_off_axis_clicks`（轴外 40px 旧守卫点现 MISS）、`Far_from_gizmo_misses_so_click_falls_through_to_picking`（视口角落 MISS → 落 Picking）、`Hit_radius_follows_visible_geometry_with_explicit_margin`（可见中心必命中、容差内仍命中、超容差 MISS）。

### 7.3 明确未做（守禁区）
未改 `WorldQuery` / 空间索引 / `Region` / `EntityRegistry` / 实体 Picking 算法；未重构输入系统；未改 Gizmo 外观（Vulkan 绘制线宽未动）；未处理 P1 零位移 Undo；未碰 Vulkan / Editor.UI 旧债；未宣布 R2 CLOSED。

### 7.4 验证结果
- `dotnet build XuanYu.Engine.slnx`：9 项目 **0 warning / 0 error**。
- `dotnet test XuanYu.Core.Tests`：**69 passed / 0 failed**（含 2 个新增 G1 回归测试与替换后的容差测试）。
- `dotnet test XuanYu.World.Tests`：**99 passed / 0 failed**（R2 回归无回潮）。
- `scripts/arch-a-guard.ps1`：**EXIT=0**（含 5+100：4 个改动文件均 ≤100 行）。
- `git diff --check`：通过（无尾随空白 / 冲突标记）。
- 状态：自动验证全绿；**待用户真机验收**（沿用 R2 清单，重点盯移动后 / Undo 后 / Redo 后 Picking 与相邻实体不再被 Gizmo 光环抢占）后，G1 方可视为 CLOSED。

### 7.5 提交
- Commit：见 `changelog.md` 对应 `###` 条目（含真实 Commit Hash）。
- 本追加节与代码、测试同批落库（同一次修复提交）。
