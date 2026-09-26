# Rendering 渲染知识

## K-REN-005 图形功能必须以语义正确的最终像素和 Runtime Visual Gate 收口

**状态**：Active
**优先级**：P0
**证据等级**：E1
**标签**：Vulkan、Visual Gate、Semantic Test、Analytic Stroke、Text Coverage、Runtime
**适用范围**：地图 Vector Overlay、GPU 文字、Shader、Texture Upload、Screen-space 几何、任何“自动合同通过但最终画面仍可能错误”的图形功能。

**首次确认**：2026-09-26（UTC+08:00）
**来源任务**：MAP-VECTOR-VISUAL-R1
**关键 Commit**：`0212fea4`（初版 analytic stroke）、`15812ae0`（修复退化 Stroke）、`8d83efc5`（Label alpha/upload 修复）、`ed3e0df5`（灰度 coverage 收口）
**任务最终收口**：`12f86a6a`

### 已确认事实

本轮出现两类典型“自动合同成立，但用户画面错误”：

1. Stroke 已有独立 Pipeline、Shader、Primitive 与定向合同，但终点侧顶点把同一线段的方向从 A→B 反成 B→A，导致屏幕空间 quad 退化；自动合同没有验证真实三角形面积和端点覆盖范围，真机只看到 Marker、看不到连线。
2. Region Label 已完成 Raster、Bitmap、VkImage、Descriptor、Sampler 和 Screen-space Quad，测试也证明像素中同时存在 0 与非 0 coverage，但没有验证“背景必须为 0、字形必须为正 coverage”。真机因此出现整块浅色矩形和反相字形。

这些失败证明：

```text
Pipeline Created
Descriptor Bound
Texture Uploaded
Vertex Count Correct
Contract PASS
≠
Final Pixels Semantically Correct
```

### 工程规则

图形功能的 Definition of Done 必须同时覆盖“结构存在”和“视觉语义”：

```text
Resource / Pipeline Contract
→ Geometry Semantic Test
→ Coverage / Pixel Semantic Test
→ Runtime Visual Gate
→ User-visible Acceptance
```

对于几何：

- 不只检查顶点/索引数量，还要验证三角形非退化、面积非零、端点/边界覆盖范围符合语义；
- Screen-space Stroke 必须验证水平、斜线、闭合、多段、selected/unselected 和 DPI；
- 同一 segment 的 CPU 顶点语义与 Shader 局部坐标语义必须保持一致。

对于纹理/文字：

- 不只检查“有像素”“有 Alpha”，还要验证背景与前景的 coverage 方向；
- Padding/四角应验证为背景 coverage，字形 bounding box 不得覆盖整张纹理；
- Upload 的 Stride / RowLength / PixelFormat 必须与 Raster DTO 一致；
- 最终必须在真实 Vulkan Viewport 中检查文字是否可读、边缘是否正确、是否存在矩形底板或反相。

### 当前推荐分层示例

当 UI 框架具备成熟字体排版能力，但最终视觉必须出现在 Vulkan Native Viewport 中时，可采用：

```text
Editor.UI
  Platform text raster / shaping
        ↓
Render.Abstractions
  pure bitmap / label DTO
        ↓
Vulkan
  texture cache / upload / draw
```

边界要求：

- Vulkan 不反向依赖 Editor.UI；
- Render DTO 不包含 Avalonia 类型；
- 领域名称事实仍只有一份；
- Camera Pan/Zoom 不应因为实例位置变化而重新栅格或重复上传相同 CacheKey。

这是一种适用于当前 Native HWND 过渡架构的已验证模式，不自动推广为所有文字系统的唯一终局。

### 禁止做法

- 仅凭 Shader 编译、Pipeline 创建或 Descriptor 绑定成功宣布视觉功能 DONE；
- 只断言“顶点数正确”而不验证几何非退化；
- 只断言 coverage 中“有 0、有非 0”而不验证前景/背景方向；
- 用全局 MSAA、FXAA、世界坐标偏移等大范围补偿掩盖局部几何/coverage 语义错误；
- 把内部 Vulkan 资源步骤当成用户验收目标，导致任务偏离“用户最终看到什么”。

### 正确做法

1. 开工时先写用户可见验收句，例如“区域内能看到名称”“道路/区域斜线连续且平滑”；
2. 再将其分解成资源、几何、coverage、runtime 四级证据；
3. 自动测试至少覆盖一个会让错误实现失败的语义断言；
4. 图形任务没有 Runtime Visual Gate 证据时，只能报 PARTIAL；
5. 真机发现反例后，先更新遗漏的语义测试，再修实现，避免同类假绿灯复发。

### 验证方法

- 几何：非零面积、端点覆盖、闭合段数、斜线、多段、DPI；
- Coverage：四角/背景=0、字形区域>0、非零 bounding box 与占比；
- GPU：PixelFormat、Stride/RowLength、CacheKey、Upload 次数；
- Runtime：正式 `run.bat`、真实 Vulkan Viewport、Pan/Zoom/Rename/Geometry edit；
- 最终报告明确区分自动 PASS 与真机 PASS。

**关联 Knowledge**：K-VAL-001、K-VAL-002、K-REN-001、K-REN-002
**关联 Lesson**：L-REN-003

---

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

### 2026-08-12 追加：Avalonia Visual 覆盖 Native HWND 同样受 Airspace 限制

逻辑 `ZIndex` 只排序 Avalonia Visual，不能可靠跨越 `NativeControlHost` 的真实 HWND。当确认框、菜单或关键提示必须覆盖 NativeHost 范围时，不得以主窗口 Overlay/Card 或提高 `Panel.ZIndex` 作为正式方案；优先真实 TopLevel / Owned Window，并以真机可见与输入验证。

Layer Delete Confirmation 的 DialogCard 状态正常却被 Vulkan HWND 压住；最终采用 Editor Owned Avalonia Window，并修正 Dataset-backed 路由后真机显示成功。

**关联 Incident**：INC-2026-08-12-001

---

## K-NATIVE-002 Native↔Avalonia 坐标必须显式跨空间转换

**状态**：Active
**优先级**：P0
**证据等级**：E2
**标签**：Native、Avalonia、Screen Space、DPI、Coordinate Contract
**适用范围**：Native Viewport、Popup、Diagnostic、Overlay、Pointer、窗口布局。

Native Client、Screen、Owner/TopLevel、Avalonia Logical/DIP 不是同一坐标空间。任何跨 Native/Avalonia 的点都必须声明 Source Space 和 Target Space，并沿明确链路转换：

```text
Native Client → Screen → Owner/TopLevel → Avalonia Logical/DIP
```

禁止把只有 `X/Y` 的数值直接当作另一宿主的坐标。验证至少包含 DPI、窗口移动、Owner 偏移和往返误差。
