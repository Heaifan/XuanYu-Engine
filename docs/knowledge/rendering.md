# Rendering 渲染知识

## K-REN-004 Editor World Reference Grid 必须独立于 MapGround

**状态**：Active
**优先级**：P0
**证据等级**：E3
**标签**：Vulkan、World Grid、Editor Environment、MapGround、Depth、LOD
**适用范围**：World Reference Grid、编辑器环境辅助层、Map Surface 与 3D 场景共存的网格显示。

**首次确认**：2026-08-10 23:39:57（UTC+08:00）
**最近验证**：2026-08-10 23:50:35（UTC+08:00）
**版本链**：`v0.2.25.28-fix` → `v0.2.25.29-fix`
**Commit**：`2c57893`、`6154078`
**来源**：GRID-RW-2A / GRID-RW-2B。

### 工程规则

```text
World Reference Grid belongs to Editor Environment
World Reference Grid is not Map Surface
```

- Grid Plane 固定为 World XY（Z=0），独立于 `Map.BaseHeightMeters`；
- MapGround 有无不得决定 Grid 是否存在；
- Grid 不使用 World Z Offset、Ground Depth 或 Ground Bias；
- Grid Pass 的 DepthTest/DepthWrite 关闭；
- Step 由 CPU 全帧统一决定，Fragment 不自行决定 Grid LOD；
- `fwidth` 只用于 AA；
- MapGround、World Grid、Region 是不同语义层。

### 禁止做法

- 用真实世界 LineList 与 MapGround 共面，再以 Depth Bias 抢可见性；
- 让 Fragment 自行计算 Step、LOD 或网格层级；
- 把 Map BaseHeight 隐式作为 World Grid 高度。

### 验证方法

- 自动合同：World XY、CPU Step、禁 Fragment LOD、禁 Grid Ground Bias、Ground ON/OFF 独立性；
- 真机：Ground ON/OFF、连续缩放、远距减密、低角度、Resize；
- 完整 F1 FINAL 回归不得以自动测试替代真机结果。

**关联 Incident**：INC-2026-08-10-006
**关联 Lesson**：L-REN-001
**关联 Knowledge**：K-REN-001、K-REN-002

---

## K-REN-001 Editor Overlay 不得用世界坐标偏移制造视觉层级

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Overlay、World Space、Z Offset、Domain Semantics
**适用范围**：Region、边界线、Marker、Gizmo、编辑器辅助图形。

**首次明确冻结**：2026-08-10 13:37:23（UTC+08:00）
**版本**：`v0.2.25.13-rz`
**Commit**：`ef12f4b`
**后续验证**：`v0.2.25.14-fix` / `8c8dfdd` → `v0.2.25.15-stab` / `751da52` → `v0.2.25.17-stab` / `c307c66`
**来源**：`changelog.md`

### 问题

Region 的 Fill、Stroke、Marker 表示同一组地图点。如果为了让 Stroke “浮在地面上”而把它写成 `BaseHeightMeters + 0.03`，渲染需求就偷偷改变了世界数据语义：同一个 MapPoint 在不同视觉组件中变成不同 Z。

### 工程规则

显示层级必须由渲染策略表达，不能通过修改领域坐标表达。对于同一领域锚点：

```text
MapPoint P
├─ Fill(P)
├─ Stroke(P)
└─ Marker(P)
```

三者共享同一世界位置；谁可见、谁在上层由 Render Pass、Depth、Stencil、Blend、Draw Order 等决定。

### 禁止做法

- `+0.01m`、`+0.03m` 作为长期 Z-fighting 解决方案。
- 为了选中反馈把正式实体位置抬高。
- 让 UI/Editor Overlay 的视觉优先级改变 Picking/测距/保存语义。

### 真实历史示例

`v0.2.25.13-rz` 删除 Vector Overlay Stroke 的 `BaseHeightMeters + 0.03`，明确要求 Fill、Stroke、Marker 对同一 MapPoint 使用完全相同的世界锚点，并写下“不得用世界 Z 偏移实现视觉层级”。

### 未来应用示例

未来绘制“国境线 + 控制区填充 + 节点手柄”时，国境线不能为可见性被抬高 0.1m。若需要永远显示在填充上，应建立明确 Overlay pass/draw order。

### 验证方法

- 单元测试直接断言同一 MapPoint 生成的 Fill/Stroke/Marker 世界坐标完全相等；
- 视觉测试分别覆盖俯视、45°、低角度；
- 检查 Picking/保存不读取视觉偏移。

**关联 Incident**：INC-2026-08-10-002
**关联 Knowledge**：K-REN-002、K-SPA-001

---

## K-REN-002 共面 Overlay 应由独立 Depth Policy 与 Draw Order 表达

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Vulkan、Depth、Overlay Pass、Bias、Draw Order
**适用范围**：共面编辑器辅助层、透明 Fill、Stroke、Marker。

**关键收口**：2026-08-10 14:22:43（UTC+08:00）
**版本**：`v0.2.25.15-stab`
**Commit**：`751da52`
**前置尝试**：`v0.2.25.14-fix` · 2026-08-10 13:51:49 · `8c8dfdd`（Clip-Z Bias）
**后续清理**：`v0.2.25.17-stab` · `c307c66`（删除过期 Bias）
**来源**：`changelog.md`

### 问题

共面 Overlay 与 Ground/其它几何共享 Depth 时，会出现深度争抢。早期可以通过 clip-space bias 临时试验，但一旦最终架构已经有独立 Overlay Pass，继续保留 Bias 会形成叠加 workaround，增加不同相机角度下的不确定性。

### 工程规则

当一个视觉层本质属于 Editor Overlay，应优先给它独立、可解释的 Depth Policy，并用明确 Draw Order 表达同层次顺序。例如：

```text
Vector Overlay Pass
DepthTest = Off
DepthWrite = Off
Draw: Fill → Stroke → Marker
```

如果最终策略已经足以表达层级，应删除旧 Bias，而不是同时保留多套补偿。

### 真实历史示例

`v0.2.25.14-fix` 曾在不改变世界锚点的前提下加入有界 clip-space bias。`v0.2.25.15-stab` 随后创建独立无 Depth Test / 无 Depth Write 的 Vector Overlay Pass，并保持 Fill→Stroke→Marker。到 `v0.2.25.17-stab` 删除过期 Clip-Z Bias，让 Layering 只由正式 Pass/Draw Policy 表达。

### 未来应用示例

新增“战线危险区透明填充 + 实线边界 + 锚点”时，如果它属于纯编辑器标注层，应先判断是否放入统一 Overlay Pass，而不是分别给三种图元设置三个不同 Depth Bias。

### 禁止做法

- Bias、世界 Z 偏移、Depth Off 三套机制同时叠加却没有明确主策略。
- 修复一种相机角度后不测 45°/80°/近距离。
- 把临时 workaround 留成无测试的永久行为。

### 验证方法

- Shader 合同确认不存在已废弃 Bias；
- Pipeline 合同确认 Overlay Depth 状态；
- DrawPlan 断言顺序；
- 真机覆盖俯视、45°、低角度和近距离。

**关联 Incident**：INC-2026-08-10-002
**关联 Knowledge**：K-REN-001、K-REN-003

---

## K-REN-003 Background / Sky 必须具有明确且独立的 Depth 语义

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Vulkan、Sky、Background、DepthWrite、Pipeline
**适用范围**：天空、背景全屏三角、编辑环境背景、任何应永远位于场景后方的图元。

**首次事故确认**：2026-08-01 16:56:53（UTC+08:00）
**版本**：`v0.2.21.21-fix`
**Commit**：`e0a994ae11b7d7a2c383d3e4a6e4100385c46ecf`
**最终架构验证**：`v0.2.22.0-rz` · 2026-08-02 15:28:21 · `ac50d1c65fe222ab320f36ececaeed30facec4e5`
**来源**：`docs/archive/changelog/changelog-2026-07.md`

### 问题

背景在颜色意义上“应该在最后面”，但 GPU 并不知道这种语义。如果背景与实体共用会写 Depth 的 Pipeline，它可以先占据深度缓冲，导致后绘制实体被遮挡。

### 真实历史示例

D2 开启 DepthTest/DepthWrite 后，全屏背景三角仍写 `z=0.98`。结果部分相机距离下背景先占深度，静态模型需要继续缩放才显示完整。`v0.2.21.21-fix` 先把背景深度修为 far depth `1.0`；随后 `v0.2.22.0-rz` 建立天空专用 Pipeline，正式使用 `DepthTest=Off`、`DepthWrite=Off`。

### 工程规则

背景/天空必须具有明确的 Depth 语义。若它不应遮挡任何场景对象，就不应依赖“给一个足够远的 z”作为唯一保证；优先使用独立 Pipeline/Pass 关闭 Depth Test/Write，或采用同等明确的机制。

### 未来应用示例

未来加入世界空间星空、天气背景或远景穹顶时，不能简单复用实体主管线。必须明确：它是否写 Depth？是否参与实体遮挡？Swapchain 重建时是否保持独立管线？

### 验证方法

- 断言背景 Pipeline Depth 状态；
- 场景中放置近/中/远多个模型，改变相机距离确认无遮挡；
- Resize/Swapchain 重建后回归。

### 注意

`v0.2.21.21-fix` 同时记录了“不能靠压缩代码格式满足 5+100”的治理纠偏；该经验属于开发规范，不在本条展开。

**关联 Incident**：INC-2026-08-01-001
**关联 Knowledge**：K-REN-002

---

## K-NATIVE-001 Native Overlay 必须验证真实 HWND 层级与绘制状态

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Win32、HWND、Avalonia、Vulkan、WS_CHILD、WS_POPUP、Z-order
**适用范围**：Avalonia + Native Vulkan 混合 UI、原生悬浮控件、比例尺、NativeOverlay。

**关键确认**：2026-08-10 16:51:42（UTC+08:00）
**版本**：`v0.2.25.18-stab`
**Commit**：`06b26e9`
**前置版本**：`v0.2.25.17-stab` · `c307c66`
**来源**：`changelog.md`

### 问题

在 Avalonia + Vulkan NativeHost 混合场景中，逻辑层的 `Visible=true` 不代表用户真的能看到 Native Overlay。真实可见性还取决于 HWND Parent/Owner、窗口样式、Z-order、Rect、裁剪、WM_PAINT、宿主重排与 DPI。

### 工程规则

Native Overlay 的调试对象必须是“真实窗口”，不能只看 ViewModel。至少应能观察：

```text
HWND
Parent / Owner
Window Style
Visible
Rect
Text / State
Z-order
WM_PAINT Count
```

对于覆盖 Vulkan 视口的控件，应在设计阶段明确 `WS_CHILD` / sibling / owned `WS_POPUP` 等窗口模型，而不是靠反复 SetTopMost 试错。

### 真实历史示例

`v0.2.25.17-stab` 把比例尺做成与 Vulkan HWND 同父级 sibling 并显式置顶，仍需真机验证。`v0.2.25.18-stab` 最终将其改为拥有主窗口的独立 `WS_POPUP`，保留 click-through 与 non-activating，并新增 HWND/可见性/矩形/文本/WM_PAINT 探针。真机重启后用户看到 `100 m`。

### 未来应用示例

未来加入 Native FPS/坐标悬浮条，如果测试只断言 `Text="60 FPS"` 与 `Visible=true`，仍不能证明它显示在 Vulkan child 前方。应直接探测窗口层级与 PaintCount，并真机确认。

### 验证方法

- HWND/Parent/Owner/Style 日志；
- Rect 与 Avalonia Viewport 布局坐标对照；
- WM_PAINT 计数；
- 窗口 resize、切 Tab、DPI、失焦/再聚焦；
- 真机视觉。

**关联 Incident**：INC-2026-08-10-004
**关联 Knowledge**：K-VAL-001、K-VAL-002
