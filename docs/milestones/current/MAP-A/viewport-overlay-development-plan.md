# MAP-A-R3 · Viewport Overlay System / Scale Indicator 开发计划

> 当前基线：`feat/MAP-A-R3 @ ac5d306`
> 当前版本：`v0.2.25.18-stab`
> 当前状态：F1 `OPEN · ACCEPTANCE FAILED · REWORK`
> 本计划目的：终止 Native Scale HWND / Popup 路线；复用 Navigation Gizmo 成功模式，将比例尺迁移为 Vulkan-native Viewport Overlay，并沉淀最小可复用 Overlay 基础合同。
> 不扩大范围：不开发完整 Font Engine，不改 Region Fill，不改 Picking，不改 Metric/Zoom Floor，不进入 D3。

---

# 一、冻结裁定

## 1. 技术路线

旧路线：

```text
Scale Indicator
→ Avalonia
→ Native Child
→ Sibling HWND
→ WS_POPUP
```

正式停止。

新路线：

```text
ScaleIndicatorMetric
        ↓
Viewport Overlay Layout
        ↓
Scale Indicator Geometry
        ↓
Vulkan Overlay Pass
        ↓
Viewport
```

## 2. 复用原则

以现有 `NavigationGizmo` 为成功参考：

- Viewport-relative。
- RenderDrawPlan 管理 Draw Order。
- Depth Off。
- DPI / Resize 与 Viewport 同源。
- 不新增 HWND。

## 3. 当前保留

这些成果不回退：

- `ViewportMetricScale`
- `MetersPerDipX/Y`
- `ScaleIndicatorMetric`
- 100m Zoom Floor
- Metric fail-closed
- 双精度 Picking
- Region World Anchor
- Region Depth-Off Overlay

---

# 二、任务冻结

整个工作拆为 4 个独立小轮。

```text
OVL-R0 文档/路线冻结
↓
OVL-R1 Overlay Layout Contract
↓
OVL-R2 Vulkan Scale Indicator
↓
OVL-R3 删除 Native Scale HWND
↓
用户真机验收
↓
F1 Final
```

每轮必须独立 Commit + Push。

---

# 三、OVL-R0 · 治理冻结

目标：

只修改计划 / 知识库 / backlog，不改功能代码。

TODO：

- [x] R0-T01 将本知识文档纳入 docs 知识库。
- [x] R0-T02 在 R3 backlog 标记 `STAB-5A = FAILED · WRONG PRESENTATION ARCHITECTURE`。
- [x] R0-T03 冻结 `STAB-5B = Vulkan-native Scale Indicator`。
- [x] R0-T04 明确禁止继续修 `WS_POPUP` 坐标。
- [x] R0-T05 记录 `ac5d306` 为旧 Native Popup 路线终点。

通过条件：

- docs / backlog 一致。
- `git diff --check` PASS。
- Commit + Push。
- local HEAD == remote HEAD。

本轮不更新功能版本，除非仓库版本治理要求文档轮同步版本。

---

# 四、OVL-R1 · Viewport Overlay Layout Contract

目标：

建立一个**小型纯数学布局合同**，不是创建“大型 UI Framework”。

建议位置：

```text
XuanYu.Render.Abstractions/
    ViewportOverlayAnchor.cs
    ViewportOverlayLayout.cs
```

受 5+100 限制可进一步拆分。

建议数据：

```text
ViewportOverlayAnchor
- TopLeft
- TopRight
- BottomLeft
- BottomRight
- Center
```

```text
ViewportOverlayLayoutRequest
- ViewportWidthDip
- ViewportHeightDip
- DesiredWidthDip
- DesiredHeightDip
- MarginXDip
- MarginYDip
- Anchor
```

输出：

```text
ViewportOverlayRect
- X
- Y
- Width
- Height
```

硬合同：

```text
0 <= X
0 <= Y
Right <= ViewportWidthDip
Bottom <= ViewportHeightDip
```

## R1 TODO

- [x] R1-T01 新增 Anchor / Rect / Resolver 纯合同。
- [x] R1-T02 Navigation Gizmo 现有布局不迁移，只建立兼容性测试，证明同一 Anchor 语义可表达 Gizmo 的 TopRight 位置。
- [x] R1-T03 新增 Scale Indicator BottomLeft / BottomRight 布局用例。
- [x] R1-T04 DPI 只在渲染边界做 DIP→physical pixel；Resolver 本身只处理 DIP。
- [x] R1-T05 测试 1024×640、1360×820、1920×1080。
- [x] R1-T06 测试 DPI 1.0 / 1.25 / 1.5 / 2.0 下 DIP 布局不变。
- [x] R1-T07 完整门禁 + Commit + Push。

禁止：

- 不修改 Native Scale HWND。
- 不增加 RenderDrawKind。
- 不改 Vulkan Shader。
- 不动 Region。

---

# 五、OVL-R2 · Vulkan-native Scale Indicator

目标：

让比例尺真正成为 Viewport Render Overlay。

## 1. RenderDrawKind

新增类似：

```text
ScaleIndicatorOverlay
```

建议顺序：

```text
MapVectorOverlay
WorldOrigin / Transform Gizmo
ScaleIndicatorOverlay
NavigationGizmo
```

Navigation Gizmo 继续最后。

## 2. 数据源

复用：

```text
ScaleIndicatorMetric
```

不重新计算比例尺。

Render Projection / Assist State 只传：

```text
Visible
Label / EncodedGlyphs
BarWidthDip
```

不要让 Vulkan 重新做公制算法。

## 3. Geometry

比例尺最小视觉：

```text
100 m
├────────────┤
```

包含：

- 主横线
- 左 Tick
- 右 Tick
- 文本

全部 Screen-space。

## 4. 文本

本阶段只实现 `ScaleIndicatorGlyphLite`。

允许字符：

```text
0 1 2 3 4 5 6 7 8 9
m
k
.
空格
```

即使实际 1/2/5 序列目前主要使用 0/1/2/5，也建议一次补齐 0-9，避免格式化未来变化立即返工。

技术建议：

### 推荐：小型 Vector Glyph

优点：

- 无字体文件。
- 无外部依赖。
- 纯几何。
- DPI 可控。

限制：

- 只用于比例尺。
- 明确命名 `ScaleIndicatorGlyph...`，不得假装通用 Text Renderer。

## R2 TODO

- [x] R2-T01 新增 ScaleIndicator Render Projection DTO。
- [x] R2-T02 新增 `ScaleIndicatorOverlay` DrawKind。
- [x] R2-T03 新增 screen-space bar/tick geometry。
- [x] R2-T04 新增受限 glyph geometry。
- [x] R2-T05 接入 Vulkan Overlay Pipeline，DepthTest Off / DepthWrite Off。
- [x] R2-T06 使用 OVL-R1 LayoutRect 定位。
- [x] R2-T07 Scale Indicator 与 Navigation Gizmo Draw Order 合同。
- [x] R2-T08 Resize/DPI 更新合同。
- [x] R2-T09 聚焦测试。
- [x] R2-T10 完整正式门禁 + Commit + Push。

建议版本：

```text
v0.2.25.23-fix
```

具体版本由仓库当轮版本治理确认。

---

# 六、OVL-R3 · 删除 Native Scale HWND 技术债

目标：

Vulkan 比例尺自动验证完成后，删除旧路线。

删除/清理对象预计包括：

```text
VulkanNativeHost.ScaleIndicator.cs
Win32ViewportHost.ScaleIndicator.cs
Win32ViewportHost.ScaleIndicator.Paint.cs
Scale HWND Probe
ScaleProbeSink
WM_PAINT 比例尺逻辑
WS_POPUP 比例尺逻辑
GDI DrawText 比例尺逻辑
```

注意：

如果文件还承载其他职责，不允许整文件盲删，只移除 Scale-specific 内容。

## R3 TODO

- [x] R3-T01 删除 Native Scale 创建/销毁链。
- [x] R3-T02 删除 Popup / GDI 比例尺状态。
- [x] R3-T03 删除旧源码字符串合同测试。
- [x] R3-T04 用 Vulkan Overlay 合同替代。
- [x] R3-T05 更新 file-tree。
- [x] R3-T06 更新 changelog / backlog。
- [x] R3-T07 全量门禁。
- [x] R3-T08 Commit + Push，确认 local==remote、worktree clean。

禁止：

- 不删除通用 `Win32ViewportHost`。
- 不影响 Vulkan Native Host 生命周期。
- 不影响 Navigation Gizmo。
- 不顺手开发通用字体系统。

---

# 七、真机验收

自动门禁完成后只能：

```text
READY FOR USER ACCEPTANCE
```

不得 CLOSED。

验收模板：

```text
MAP-A-R3 · Vulkan Scale Indicator 真机验收
基线：<HEAD> / <VERSION>

V01 初始位置
结果：通过 / 不通过
比例尺是否位于 Viewport 内：
位置：
显示值：

V02 Resize
结果：通过 / 不通过
拖动编辑器窗口尺寸后是否仍固定：

V03 最大化 / 恢复
结果：通过 / 不通过
最大化：
恢复：

V04 Inspector 宽度变化
结果：通过 / 不通过
Viewport 改宽后是否仍固定：

V05 日志区域展开 / 收起
结果：通过 / 不通过

V06 Zoom
结果：通过 / 不通过
显示值是否随尺度变化：
最细是否保持 100 m：

V07 DPI
结果：通过 / 不通过
如设备可测，视觉尺寸是否合理：

V08 Navigation Gizmo 回归
结果：通过 / 不通过
Gizmo 是否正常：
两者是否互相遮挡：

V09 Region / Picking 回归
结果：通过 / 不通过
Region 输入：
最大 Zoom 点击：
```

全 PASS 后：

```text
Scale Indicator ACCEPTED
```

然后才允许进入 F1 Final。

---

# 八、正式门禁

每个功能实现轮：

```text
dotnet build solution
0 Warning
0 Error

Core.Tests
ALL PASS

World.Tests
ALL PASS

WarCore.Tests
ALL PASS

Focused Tests
ALL PASS

ARCH-A
PASS

5+100
PASS

版本一致性
PASS

git diff --check
PASS
```

禁止以聚焦 1/1、2/2 代替 Core / World 全量。

---

# 九、失败升级规则

## 第一次 FAIL

只针对失败项最小修复。

## 第二次同类 FAIL

停止编码，执行：

```text
Presentation Architecture Review
Layout Contract Review
Input/Visual Contract Review
```

## 不允许第三次盲补

如果同一问题准备第三次改：

> 必须由 ChatGPT 重新审查架构路线。

---

# 十、Git / 版本规则

每个独立轮必须：

```text
Commit
Push
local HEAD == origin/feat/MAP-A-R3
ahead = 0
behind = 0
worktree clean
```

禁止：

- “先完成，稍后一起推”
- force push
- rebase
- tag / release（用户真机 CLOSED 前）

---

# 十一、代码范围预估

可能涉及：

```text
XuanYu.Render.Abstractions/
    RenderDrawPlan.cs
    ViewportOverlayAnchor.cs
    ViewportOverlayLayout.cs
    ScaleIndicator overlay DTO

XuanYu.Render.Vulkan/
    Render/...
    Pipeline/...
    Shaders/...

XuanYu.Editor.UI/
    Vm/Camera/UiVm.ScaleIndicator.cs
    Viewport/Vulkan/... 旧 Native Scale 清理

XuanYu.Core.Tests/
    Render/Overlay/...

XuanYu.World.Tests/
    UiRuntime/...

docs/knowledge/ui/...
docs/milestones/current/MAP-A/R3-backlog.md
changelog.md
file-tree.md
```

实际修改前必须先定位现有 Navigation Gizmo Overlay 实现，不得复制一套新的 viewport-size / DPI 算法。

---

# 十二、当前项目进度

当前阶段：

```text
MAP-A-R3-D2-F1
Scale Indicator Presentation Architecture Rework
```

已完成：

- 100m Metric Grid
- Directional Metric
- Zoom Floor
- Double Precision Picking
- Region World Anchor
- Gizmo 主链恢复

当前自动实现状态：

```text
Vulkan-native Scale Indicator 已完成固定几何与浅色 Token 视觉收口，等待真机验收
```

下一步：

```text
OVL-R0
→ OVL-R1
→ OVL-R2
→ OVL-R3
→ 真机验收
→ F1 Final
```

阻塞：

- 用户尚未执行固定几何、动态真实标签、缩放范围与 Resize/DPI 真机验收。

Git：

```text
feat/MAP-A-R3
OVL 实现基线 = b3b024c
最终远端状态 = 本轮文档收口 Push 后复核
```

整体百分比：

不以比例尺返工轮数虚增完成度；F1 在视觉验收通过前继续 OPEN。

---

# 十三、OVL-R0 结论

- `STAB-5A = FAILED · WRONG PRESENTATION ARCHITECTURE`。
- `ac5d306` 是 Native Popup 路线终点，禁止继续修改 Popup Screen Rect。
- `STAB-5B = FROZEN · Vulkan-native Scale Indicator`。
- OVL-R1～R3 已按冻结路线实现，不再保留 Native Popup 双轨。
- 自动门禁完成后状态只进入 `READY FOR USER ACCEPTANCE`；F1 继续 OPEN。
