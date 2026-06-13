# FluidWarfare a051352 — Blender 导航 Gizmo 交互修复包

基线：`a051352`

## 已修复的关键根因

### 1. Resize 后 Gizmo 显示位置和 HitTest 坐标分裂

原实现的 Overlay Pipeline 使用静态 Viewport，但 `VulkanScene3dSession.Resize()` 只重建世界 Grid/Unit Pipeline，没有重建 Overlay Pipeline/资源。

结果：

- Overlay 仍按旧 Swapchain 尺寸绘制；
- CPU `ViewportNavigationLayout` 按新尺寸计算；
- Gizmo 会出现在视口上方中间附近，而不是右上角；
- `PresentedNavigationOverlaySnapshot` 尺寸闸门拒绝输入，造成“看得到但点不了”。

修复：Resize 时随新 RenderPass 和新 Extent 事务式创建新的 `VulkanOverlayResources`，切换成功后再释放旧 Overlay 资源，并使旧 Presented Overlay Snapshot 失效。

### 2. 重叠轴端 HitTest 顺序错误

轴端按“背面 → 正面”排序绘制，原 HitTest 却同方向遍历，重叠时背面轴会抢走点击。

修复：HitTest 改为反向遍历，屏幕最前方轴端优先。

### 3. Gizmo Orbit 命中面积过小

原实现只能精确点击中心 12px 小圆才能开始 Orbit。

修复：增加完整 Gizmo 环绕拖动区域，轴端仍保持最高优先级；导航球内部空白区域均可左键拖动 Orbit。

### 4. Win32 左键捕获语义不清

原输入只返回 `bool`，无法区分“一次性按钮点击”和“需要鼠标捕获的拖动”。

修复：新增 `ViewportNavigationPressResult`：

- `NotHandled`：继续世界 Picking；
- `HandledClick`：阻断 Picking，不捕获鼠标；
- `BeginDrag`：阻断 Picking并捕获鼠标。

### 5. 正交模式 Pan/Zoom 不符合视觉尺度

修复：

- 正交 Pan 使用 `OrthographicHeight / viewportHeight`；
- 正交滚轮和拖动缩放修改 `OrthographicHeight`；
- 透视/正交切换按 Pivot 深度处的可见高度换算，减少跳变；
- Pitch 范围改为 `-89..89`，使 `-Z` 底视图真实可用。

## 交互结果

- 六个轴端：点击切换标准正交视图；
- 导航球区域：左键拖动 Orbit；
- 手掌按钮：左键拖动 Pan；
- 放大镜按钮：左键拖动 Zoom；
- 靶心按钮：聚焦所选，无选择时显示全部；
- 投影按钮：切换透视/正交；
- Overlay 命中后阻断实体和地面 Picking；
- Hover/Pressed 状态通过现有 Overlay 状态和按需帧更新；
- Resize 后重新建立 Overlay 资源与 Presented Layout 对齐。

## 新增测试

- 重叠轴端选择前方元素；
- Gizmo 大范围 Orbit HitTest；
- 顶视/底视布局有限值；
- 所有交互元素均有动作；
- 极小视口布局检查；
- `-Z` 底视 Pitch；
- 透视/正交切换视觉范围保持；
- 正交 Zoom 修改正交高度；
- 正交 Pan 不依赖相机 Distance。
