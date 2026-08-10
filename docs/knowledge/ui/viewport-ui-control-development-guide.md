# 玄域引擎 UI 控件开发规范
## XuanYu Engine UI / Viewport Control Development Guide

> 文档级别：项目知识库 / 工程指导规范
> 适用项目：玄域引擎 / 《兵无常势》
> 当前基线：`feat/MAP-A-R3 @ ac5d306`
> 形成原因：MAP-A-R3-D2-F1 比例尺连续多轮返工暴露出 Viewport UI 承载层分类错误。
> 核心目标：以后开发 UI 控件时，先确定“它属于哪一层”，再决定技术；禁止为了局部可见性跨越 Avalonia / Win32 HWND / Vulkan 边界反复补丁化。

---

# 1. 核心结论

玄域引擎 UI 控件必须先按**空间归属与交互职责**分类。

最重要的规则：

> **凡是固定在 Vulkan Viewport 内、位置相对 Viewport 固定、需要随 Resize / DPI / 窗口移动自然跟随的视觉元素，默认归属 Vulkan Viewport Overlay。**

除非经过架构审查并证明 Vulkan Overlay 无法满足，否则：

> **禁止为 Viewport 固定视觉元素创建额外 HWND。**

右上角 Navigation Gizmo 是已验证成功的内部范式：它属于 Vulkan RenderDrawPlan 的 Overlay，始终在视口内，不依赖 Avalonia Airspace、Win32 子窗口层级、Popup 坐标或 GDI。

---

# 2. UI 控件四类归属

## 2.1 App UI —— Avalonia

典型内容：

- 顶部菜单
- 工具栏
- Project Tree
- Layer Panel
- Inspector
- 输入框
- 下拉框
- 按钮
- 复杂表单
- 模态 / 非模态对话框

技术归属：

`XuanYu.Editor.UI / Avalonia`

原则：

- 使用 UI Token。
- 使用 DIP。
- 不直接操作 Vulkan。
- 不创建额外 Win32 子窗口来模拟普通 Avalonia 控件。

---

## 2.2 Viewport Fixed Overlay —— Vulkan Overlay

典型内容：

- Navigation Gizmo
- 比例尺
- FPS / 性能角标
- 鼠标世界坐标
- 测距标记
- 框选矩形
- Viewport 操作提示
- 相机状态
- 未来的小型视口快捷控件

技术归属：

`Render.Abstractions + Render.Vulkan`

核心特征：

- 固定在 Viewport 的某个 Anchor。
- 位置不随世界 Camera 平移。
- 随 Viewport Resize / DPI 自然变化。
- 默认使用 Screen-space / DIP 语义。
- 与 Vulkan Frame 同步绘制。
- 不跨 Native Airspace。

---

## 2.3 World-space Visual —— 世界渲染

典型内容：

- 单位头顶标记
- 世界空间路径点
- 战术标记
- 地图 Region
- 道路 / 河流 / 控制区
- 世界坐标锚定的文字或图标

技术归属：

世界坐标 → Render Projection → Vulkan

核心特征：

- 位置属于世界。
- Camera 移动后屏幕位置会变化。
- 不应通过固定 Viewport Overlay 模拟。

---

## 2.4 Complex Viewport-adjacent UI —— Avalonia Panel

当需求需要：

- 文本输入
- 下拉框
- 滚动列表
- 多字段编辑
- Tooltip 中的大段交互
- 复杂键盘焦点

不要把它硬塞进 Vulkan Overlay。

正确方式：

- Viewport 只显示轻量提示或按钮。
- 复杂内容在 Inspector / Side Panel / Popup Panel 中完成。
- 如果必须覆盖 Viewport，先做架构审查，不默认创建独立 HWND。

---

# 3. 决策树

每次新增 UI 前必须回答：

## Q1：这个元素的位置属于世界还是属于屏幕？

如果属于世界：

→ World-space Render。

如果属于屏幕：

继续 Q2。

## Q2：它是否必须固定在 Vulkan Viewport 内？

如果否：

→ Avalonia。

如果是：

继续 Q3。

## Q3：它是否只是视觉信息或轻量交互？

如果是：

→ Vulkan Viewport Overlay。

如果需要复杂表单 / 输入法 / 富文本 / 大型菜单：

→ Avalonia Panel；必要时由 Viewport 触发。

---

# 4. Viewport Overlay 基础合同

未来所有固定 Viewport UI 应共享一个小型基础合同，而不是每个控件重新算坐标。

建议概念：

```text
ViewportOverlay
├─ Anchor
├─ MarginDip
├─ SizeDip / DesiredSizeDip
├─ ZOrder
├─ Visible
├─ HitTestMode
├─ LayoutRect
├─ Draw
└─ HitTest
```

建议 Anchor：

```text
TopLeft
TopRight
BottomLeft
BottomRight
Center
```

禁止业务控件自己散落：

```text
viewportWidth - 83
viewportHeight - 37
+ 12
- 14
```

所有角落定位统一由 Layout Resolver 处理。

---

# 5. 视觉布局与输入必须共享同一个 Rect

强约束：

> **渲染用的 LayoutRect 与输入 HitRect 必须来自同一个布局结果。**

禁止：

```text
VisualLayout.Calculate()
InputHitTester.CalculateAgain()
```

否则会出现：

- 看起来点到 Gizmo，实际 Hit Test 落到 Region。
- Resize 后视觉与点击区不同步。
- DPI 下绘制与输入偏移。

正确结构：

```text
ViewportOverlayLayoutResolver
        │
        ├── LayoutRect → Renderer
        └── LayoutRect → HitTester
```

如果需要扩展命中区：

```text
HitRect = LayoutRect.Inflate(HitSlopDip)
```

但必须显式、可测试。

---

# 6. DIP 与物理像素合同

玄域 Editor UI 统一用 DIP 描述视觉尺寸。

例如：

```text
Margin = 12 DIP
LineWidth = 1.5 DIP
MarkerRadius = 5.5 DIP
GizmoSize = 96 DIP
ScaleBarTargetWidth = 80~160 DIP
```

转换规则：

```text
physicalPixel = DIP × dpiScale
```

只能转换一次。

禁止：

- Left/Top 使用 DIP，Width/Height 使用 physical pixel。
- Avalonia Bounds 与 Win32 Screen coordinates 混用。
- 在业务控件里自行猜测 DPI。

---

# 7. Viewport Overlay 绘制顺序

当前 Navigation Gizmo 已作为 RenderDrawPlan 尾部 Overlay。

建议统一形成：

```text
Scene / Map
↓
Grid / World Assist
↓
Entities
↓
Region / Map Vector Overlay
↓
World Origin / Transform Gizmo
↓
Viewport Informational Overlay
↓
Navigation Gizmo
```

其中：

- Informational Overlay 默认不参与 Depth Test。
- Navigation Gizmo 保持最后绘制。
- 不允许通过 World Z 抬高固定 Viewport UI。

---

# 8. Viewport Overlay 的 Depth 原则

固定 Viewport UI 的深度关系由 Overlay Draw Order 决定。

禁止：

```text
WorldZ += epsilon
ClipZ -= magicBias
根据相机角度调整世界 Z
```

固定 UI 根本不应依赖世界深度。

如果某一 Overlay 必须与场景发生遮挡关系，应重新分类：它可能不是 Fixed Overlay，而是 World-space Visual。

---

# 9. 文本策略

## 9.1 当前阶段

玄域尚未建立完整 Vulkan Text Renderer。

因此当前允许：

### A. 小型受限字形集

用于：

- 比例尺
- 极少量数字状态

例如比例尺只需要：

```text
0 1 2 5
m k
空格
.
```

可以采用：

- Vector glyph
- Small bitmap atlas

必须限定用途，禁止伪装成通用文本系统。

### B. 复杂文字仍由 Avalonia Panel 显示

例如：

- 错误详情
- Region 几何错误
- Inspector 描述
- 大段 Debug 信息

---

## 9.2 后续正式 Text Renderer

当出现大量需求：

- 地名
- 单位标签
- Debug Text
- 战术文字
- 测距文字
- Viewport 状态文字

再单独立项：

```text
Viewport Text Renderer
├─ Font Atlas
├─ Glyph Cache
├─ Text Layout
├─ Quad Batch
└─ DPI Scaling
```

禁止为了一个 `100 m` 提前引入完整字体基础设施。

---

# 10. 禁止的反模式

## 10.1 Viewport UI HWND 化

禁止：

- 为比例尺创建独立 HWND。
- 为 FPS 创建独立 HWND。
- 为坐标显示创建独立 Popup。
- 用 GDI 在 Vulkan Viewport 上拼 UI。

除非架构审查批准。

## 10.2 Visual Tree 坐标冒充 Screen Coordinate

禁止把：

```text
Avalonia Bounds.Position
```

直接传给：

```text
SetWindowPos(WS_POPUP)
```

逻辑坐标与桌面物理坐标不是一个空间。

## 10.3 Magic Offset 修视觉问题

连续出现：

```text
+0.03
+0.003
+epsilon
+300px
margin += 50
```

必须停止并回到合同层检查。

## 10.4 “代码存在”冒充“真机正确”

以下测试只能证明结构存在：

```text
Assert.Contains("WS_POPUP", source)
Assert.Contains("CreateScaleIndicator", source)
```

不能证明：

- 屏幕可见
- 在正确位置
- 输入正确
- DPI 正确

---

# 11. UI 测试分层

## L1：纯数学 / Layout Contract

测试：

- Anchor
- Margin
- DPI
- Viewport Size
- LayoutRect

例如：

```text
BottomLeft + 12 DIP
→ Rect 必须始终包含在 ViewportRect 内
```

## L2：Render Contract

测试：

- DrawPlan 顺序
- Overlay Kind
- Primitive Count
- Pipeline
- Depth Policy
- Shader Contract

## L3：Input Contract

测试：

- Visual Rect == Hit Rect 来源
- HitTest true / false
- Gizmo 操作不穿透 Region
- Capture / Release 正确

## L4：Runtime Contract

测试：

- Resize
- DPI
- Camera
- Tab 切换
- VM State

## L5：真机视觉验收

必须确认：

- 真正显示
- 真正位于 Viewport 内
- Resize 后仍正确
- 最大化/恢复后仍正确
- DPI 后仍正确
- 窗口移动后仍正确
- 不遮挡不该遮挡的输入

自动门禁最多授予：

> READY FOR USER ACCEPTANCE

不能直接授予：

> VISUAL CLOSED

---

# 12. 连续失败升级规则

遵循项目“两次失败即停止扩大”的原则，并补充 UI 特化：

## 第一次真机 FAIL

允许局部修复。

## 第二次同类真机 FAIL

必须检查：

- 承载层是否错误
- 坐标空间是否错误
- Visual/Input 是否分离
- 测试是否测错层

## 第三次之前

必须执行 Architecture Review。

禁止继续 A → A1 → A2 → A3 式补丁。

---

# 13. Viewport UI 新功能开发模板

任何 Viewport UI 开发计划必须填写：

```text
功能名称：
UI 分类：
空间归属：
承载层：
Anchor：
DIP 尺寸：
是否交互：
LayoutRect 来源：
HitRect 来源：
Depth：
Draw Order：
DPI：
Resize 行为：
窗口移动行为：
真机验收项：
```

如果“承载层”填写为 HWND：

> 必须附架构审查理由。

---

# 14. 比例尺事故复盘

需求：

> 在 Viewport 固定显示比例尺。

正确分类：

> Viewport Fixed Overlay。

实际走过的错误路线：

```text
Avalonia Overlay
→ Native Child HWND
→ Sibling HWND
→ WS_POPUP
→ Screen Coordinate / DPI 问题
```

核心错误：

> 把 Viewport Overlay 当成桌面 UI 控件。

右上角 Navigation Gizmo 已经提供内部成功范式：

- RenderDrawPlan Overlay
- Depth Off
- Viewport-relative Layout
- 无额外 HWND
- 与 Vulkan Present 同步

以后类似需求首先复用该范式。

---

# 15. 强制知识库结论

1. **Viewport 内固定视觉元素默认 Vulkan Overlay。**
2. **App UI 默认 Avalonia。**
3. **世界锚定元素走 World-space Render。**
4. **Visual 与 HitTest 共用唯一 LayoutRect。**
5. **视觉尺寸全部使用 DIP。**
6. **Viewport Overlay 禁止依赖 World Z magic offset。**
7. **除非架构审查批准，Viewport 固定 UI 禁止新增 HWND。**
8. **连续两次同类真机 FAIL，第三次前必须回到架构层。**
9. **自动测试 PASS 只能代表 READY FOR USER ACCEPTANCE。**
10. **优先复用仓库已验证的成功范式，而不是重新发明承载系统。**

---

# 16. 正式知识库路径

```text
docs/knowledge/ui/viewport-ui-control-development-guide.md
```

本文件是 Viewport UI 承载层分类与开发流程的长期知识库事实源；阶段状态仍由 `docs/milestones/current/MAP-A/` 维护。
