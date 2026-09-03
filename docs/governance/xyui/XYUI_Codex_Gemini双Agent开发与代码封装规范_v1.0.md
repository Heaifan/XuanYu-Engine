# XYUI · Codex + Gemini 双 Agent 开发与代码封装规范

**版本：** v1.0  
**日期：** 2026-09-03  
**状态：** Current Working Standard  
**适用范围：** XYUI Canonical Runtime、XYUI.Avalonia.Gallery，以及 XuanYuEngine / XYLab 对 XYUI 的实装  
**固定角色：** 用户 / Codex / Gemini  

---

# 0. 核心结论

XYUI 后续开发固定采用以下职责：

```text
用户
= 需求 / 设计方向 / 最终人工验收 / 最终拍板

Gemini
= Presentation Owner
= UI 代码 + SVG + Gallery 页面 + Gallery 装修实施

Codex
= Runtime / Rules & Architecture Owner
= 代码封装 + 组件封装 + Public API + Interaction + Tests + Architecture Gates
```

一句话：

> **Gemini 负责“怎么展示、怎么装修”；Codex 负责“这个东西到底是什么、怎么封装、怎么复用、怎么保证是真的”。**

---

# 1. 禁止再次混淆的所有权边界

## 1.1 Gemini 拥有

Gemini 的主要所有权：

```text
Gallery View XAML
Gallery Section XAML
Gallery 页面布局
Gallery 响应式布局
Gallery 中文说明
Gallery 示例排版
Gallery SVG / Icon 展示
Gallery 页面视觉装修
Gallery 信息密度
Gallery 示例组合
Gallery 视觉一致性
```

Gemini 可以负责：

- 把已有 Runtime 能力展示得清楚；
- 调整 Gallery 的层级、间距、对齐；
- 设计示例区；
- 设计 live sample；
- 设计 comparison；
- 编写 Gallery 页面 XAML；
- 编写独立 SVG 视觉稿；
- 优化中文说明；
- 优化 Desktop / Mobile 展示；
- 调整 Gallery 页面自身的 Presentation。

---

## 1.2 Codex 拥有

Codex 的主要所有权：

```text
XYUI Runtime src
组件类型
组件继承
组件组合
Attached Property
Facade
Canonical Resolver
Sizing / Density Rule
Token Consumer
Interaction
State
Template Contract
Metadata
Adapter
Registry
Non-visual Helper
Runtime Tests
Architecture Tests
Build / ARCH-A / 5+100
```

Codex 负责回答：

```text
这个能力是否真实存在？
Public API 是什么？
默认值是什么？
继承规则是什么？
Override 规则是什么？
状态如何流转？
复合控件复用了谁？
测试覆盖什么？
Runtime 是否只有一个事实源？
```

---

# 2. 用户拥有最终权

以下事项只有用户可以最终决定：

- UI 方案是否通过；
- Gallery 是否好看；
- 是否允许改变 Public API；
- 是否允许改变 Runtime Contract；
- 是否允许修改 Canonical XYUI；
- 是否接受新的 Token；
- 是否接受新的 Variant；
- 是否进入下一组件；
- 是否最终 Commit / Push；
- 是否冻结某一规范。

Codex 和 Gemini 都不能把：

```text
Build PASS
```

写成：

```text
User Accepted
```

---

# 3. 双 Agent 的真实协作关系

正确关系：

```text
用户提出需求 / UI 方向
↓
Gemini 设计 Presentation / Gallery
↓
用户视觉确认
↓
Codex 封装 Runtime / Component / API
↓
Gemini 用真实 Runtime 能力装修 Gallery
↓
Codex 跑技术门禁
↓
Gemini 做 Presentation 复核
↓
用户最终人工验收
↓
Codex Commit + Push
```

也允许在已有设计时：

```text
用户给既定设计
↓
Codex 封装组件
↓
Gemini 做 Gallery / UI 落地
↓
技术 + 人工验收
```

---

# 4. Gemini 不负责什么

Gemini 默认不得自行：

- 发明新的 Runtime API；
- 发明不存在的 Attached Property；
- 在 Gallery 内复制一套 Runtime 规则；
- 自己实现新的 Sizing Resolver；
- 自己实现新的 Density Resolver；
- 复制 Geometry 算法；
- 写 Gallery-only Token 当正式 Token；
- 修改 Runtime Contract；
- 修改 Runtime Tests；
- 修改 Architecture Guard；
- 用假控件冒充真实 XYUI；
- 为了视觉效果绕过 Canonical Runtime；
- 用手写 `Height / Width / Margin` 模拟本应由 XYUI DSL 提供的能力。

如果 Gallery 需要某个能力但 Runtime 没有：

```text
STOP
→ 报告 Codex / 用户
→ 不允许在 Gallery 伪造
```

---

# 5. Codex 不负责什么

Codex 默认不应：

- 擅自重新设计 Gallery；
- 把 Gallery 装修成自己喜欢的样子；
- 自己替用户做最终视觉拍板；
- 以“Runtime 正确”为由忽视 Gallery 的可读性；
- 顺手重做无关 UI；
- 擅自重构整个 Runtime；
- 因为某个 Gallery 示例难做，就改变既定 Presentation；
- 用 C# 示例掩盖 XAML Facade 没封装完整的问题。

---

# 6. XYUI 三层架构

XYUI 固定分为三层：

```text
XYUI-1
Foundation / Semantic Primitives

XYUI-2
Controls / Interaction / Property Controls

XYUI-3
Composite / Workflow / Specialized Controls
```

基本原则：

```text
Foundation 提供语义能力
↓
Controls 消费 Foundation
↓
Composite 复用 Controls
```

禁止：

```text
Composite
→ 绕过已有 Control
→ 自己复制一套相似控件
```

---

# 7. Runtime 是唯一实现层

这是 XYUI 最核心的封装规则之一。

```text
Runtime
= 唯一真实实现

Gallery
= Runtime Consumer
```

Gallery 只能展示：

- Runtime 已存在的组件；
- Runtime 已存在的 API；
- Runtime 已存在的状态；
- Runtime 已存在的 Token；
- Runtime 已存在的 Sizing；
- Runtime 已存在的 Density；
- Runtime 已存在的 Interaction。

禁止：

```text
Gallery 写了
≠ Runtime 有了
```

---

# 8. Canonical 单一事实源

XYUI 的事实源优先级：

```text
Canonical Runtime
>
Gallery
>
文档 / 任务书
>
截图 / 口述
```

含义：

- Canonical Runtime 决定真实能力；
- Gallery 必须接回 Canonical；
- 文档必须与 Runtime 对齐；
- 截图只能作为视觉证据，不能反向定义 API。

Integration、旧副本、业务项目中的复制代码不能反向定义 Canonical XYUI。

---

# 9. 禁止第二真值

任何规则只能有一个 Canonical Resolver / Source。

禁止同时存在：

```text
Runtime Size Rule
+
Gallery Size Rule
```

禁止：

```text
Runtime Gap
+
Gallery Margin 模拟 Gap
```

禁止：

```text
Runtime Border
+
Template 里偷偷塞第二个 Border
```

禁止：

```text
XY.Size
+
每个控件自己写一份 Height 表
```

目标：

> **任何视觉或行为规则都能追溯到唯一 Canonical 来源。**

---

# 10. XYUI 的目标不是样式库，而是 XAML DSL

XYUI 的公开使用体验目标：

> **短、统一、语义化、可组合、可覆盖。**

不是：

```text
大量内部类型名
大量 Scope 类
大量 C# Consumer
大量原生 Avalonia 属性拼装
```

而是：

```xml
xy:XY.Size="Default"
xy:XY.Density="Compact"
xy:XY.Foreground="..."
xy:XY.Surface="..."
xy:XY.Font="..."
xy:XY.Typography="..."
xy:XY.Padding="..."
xy:XY.Gap="..."
xy:XY.Margin="..."
xy:XY.Radius="..."
xy:XY.Border="..."
```

---

# 11. Public XAML API 命名规则

公开 Facade：

```text
XY.<SemanticName>
```

Attached Property 使用统一 PascalCase。

推荐公开形式：

```xml
xy:XY.Size="..."
xy:XY.Density="..."
xy:XY.Foreground="..."
xy:XY.Surface="..."
xy:XY.Font="..."
xy:XY.Typography="..."
xy:XY.Padding="..."
xy:XY.Gap="..."
xy:XY.Margin="..."
xy:XY.Radius="..."
xy:XY.Border="..."
```

历史内部实现类型可以存在，但普通 Consumer 不应被迫写：

```text
XyuiSizingScope.SizeRole
XyuiDensityScope.Density
```

这类冗长内部名不得继续扩大为主要公开体验。

---

# 12. Facade 必须真的接线

一个 Public API 只有满足以下条件才算“已经封装”：

```text
XAML 可写
↓
Facade 接收
↓
Canonical Resolver 解析
↓
真实控件消费
↓
Runtime 生效
↓
Gallery 可验证
↓
Tests 可验证
```

只声明 Attached Property 但不接真实 Consumer：

> 不算完成。

---

# 13. Consumer 示例必须展示封装后的 XAML

Foundation Gallery 的普通 Consumer 用法：

> **原则上只展示封装后的 XAML。**

禁止为了掩盖 Facade 缺失写：

```text
“这个 API 暂时不好写 XAML，
我们用 C# 设置一下就行。”
```

如果普通使用场景必须靠 C# 才能完成：

> 应优先判断 XYUI Facade 是否没有封装完整。

---

# 14. 默认值 + Override

XYUI DSL 必须同时支持：

```text
Default
+
Inheritance
+
Local Override
```

目标：

- 默认值自动提供；
- 父级可以统一定义；
- 子级可以局部覆盖；
- 不要求每个控件重复写；
- 不被固定 Variant 锁死。

例如：

```xml
<StackPanel
    xy:XY.Size="Compact">

    <xy:XYButton />

    <xy:XYButton
        xy:XY.Size="Touch" />
</StackPanel>
```

含义：

```text
父容器 Compact
↓
子控件默认继承 Compact
↓
单个子控件可 Override Touch
```

---

# 15. XY.Size 统一规则

当前 Size 语义：

```text
Compact
Default
Comfortable
Touch
```

当前已形成的基础矩阵：

| Size | Control Height | Icon |
|---|---:|---:|
| Compact | 28 DIP | 14 DIP |
| Default | 32 DIP | 16 DIP |
| Comfortable | 36 DIP | 20 DIP |
| Touch | 44 DIP | 24 DIP |

原则：

```text
XY.Size
→ Canonical Sizing Rule
→ 组件消费
```

禁止每个控件自己维护：

```text
CompactHeight
DefaultHeight
ComfortableHeight
TouchHeight
```

---

# 16. Width 与 Height 的不同语义

默认：

```text
Height
→ 可由 Size Role 统一管理

Width
→ 保持内容驱动
```

除非组件本身有明确固定宽度语义。

不得因为 Size 有 4 档就机械给所有控件固定 Width。

---

# 17. Visual Size 与 Hit Target

Visual Size 和 Hit Target 是两个不同概念。

即使某一阶段两者默认值相同：

> 代码模型仍不能把它们永久绑定成同一个不可拆概念。

未来 Touch / Accessibility 可以调整 Hit Target，而不必扩大所有视觉尺寸。

---

# 18. XY.Density 规则

Density 用于改变：

- 信息密度；
- Padding；
- Gap；
- 行距；
- 控件组合紧凑程度。

当前基本语义：

```text
Compact
Default
Comfortable
```

Size 与 Density：

> **可以组合，不能互相替代。**

例如：

```xml
xy:XY.Size="Default"
xy:XY.Density="Compact"
```

是合法语义。

---

# 19. Size ≠ Density

必须保持：

```text
Size
= 单个控件视觉 / 交互尺寸等级

Density
= 布局和信息密度
```

禁止让：

```text
Density=Compact
```

偷偷等价成：

```text
Height=28
```

除非 Canonical Rule 明确规定某个具体消费关系。

---

# 20. Foreground / Surface

颜色不能继续依赖各组件自由写 Hex。

公开方向：

```xml
xy:XY.Foreground="..."
xy:XY.Surface="..."
```

颜色必须来自 Semantic Token / Canonical Resolver。

原则：

```text
语义
> 具体 Hex
```

普通 Consumer 不应为了使用 XYUI 被迫重新回到原生 Avalonia Brush 拼装。

---

# 21. Font / Typography

公开方向：

```xml
xy:XY.Font="..."
xy:XY.Typography="..."
```

其中应区分：

```text
Font
= 字体族 / 字体语义

Typography
= 字号 / Weight / LineHeight / 文本层级语义
```

目标：

> 控件消费语义排版，不把字体规则散落到每个控件。

---

# 22. Padding / Gap / Margin

必须语义分离。

```text
Padding
= 容器内部边距

Gap
= 子项之间的间距

Margin
= 元素对外部环境的间距
```

禁止：

```text
用子项 Margin 模拟 Gap
```

尤其 `XY.Gap` 当前只应在真实支持 Gap Resolver 的容器中使用。

已接线规则中：

```text
XY.Gap
→ StackPanel
```

不得在不支持的 Panel 上展示成“好像有效”。

---

# 23. Radius / Border

公开方向：

```xml
xy:XY.Radius="..."
xy:XY.Border="..."
```

要求：

- Radius 来自统一语义；
- Border 来自统一语义；
- Template 不得偷偷增加第二层装饰；
- Gallery 不得为了“更好看”加 Runtime 不存在的 Border。

---

# 24. 禁止 Hidden Border

组件 Template 内如果已有 Canonical Border：

禁止再出现：

```text
外层一个 Border
+
内层一个隐藏 Border
+
Gallery 又加一个展示 Border
```

结果会造成：

- 双边线；
- Radius 不一致；
- Hit Test 不一致；
- Padding 叠加；
- Hover / Focus 状态错位。

原则：

> **一个视觉职责只允许一个真实 Owner。**

---

# 25. Gap 不得用子项 Margin 伪装

这是必须明确检查的封装规则。

错误：

```xml
<StackPanel>
    <Item Margin="0,0,8,0" />
    <Item Margin="0,0,8,0" />
</StackPanel>
```

如果语义本来是：

```xml
<StackPanel
    xy:XY.Gap="...">
```

则应由 Gap Resolver 处理。

原因：

- Last child 不应多 Margin；
- Orientation 切换时语义不同；
- Density 切换困难；
- Responsive 难统一；
- Gallery 容易产生第二真值。

---

# 26. 复合控件必须复用已有 XYUI

这是组件封装硬规则。

如果复合控件内部子控件与已有 XYUI 控件功能 / 交互语义相同：

> **必须组合已有 XYUI 控件，或者继承现有 XYUI 控件。**

例如已有：

```text
XYNumberField
XYSelect
XYButton
XYTextField
```

则复合属性控件不应重新实现：

```text
MiniNumberBox
FakeSelect
InternalButtonStyle
CustomTextBoxLike
```

---

# 27. 禁止伪 XYUI

Gallery / Runtime 都禁止：

```text
普通 Avalonia Button
+ XYUI 类似颜色
= 假装 XYButton
```

判断一个组件是不是 XYUI：

看 Runtime Type / Contract，而不是看截图。

必须真实使用：

```text
XYUI Runtime Component
```

---

# 28. 组件继承优先于复制外观

若语义相同：

```text
Inheritance
或
Composition
```

优先于：

```text
Copy Template
Copy Style
Copy Interaction
```

目的：

- Bug 修复自动继承；
- 状态统一；
- Theme 统一；
- API 统一；
- Test 统一；
- Accessibility 统一。

---

# 29. Interaction 属于 Codex 封装

Gallery 展示交互，但交互本身属于 Runtime Contract。

例如：

- Focus；
- Keyboard；
- Pointer；
- Selection；
- Drag；
- Popup；
- Validation；
- Commit；
- Cancel。

Gemini 可以设计如何在 Gallery 展示这些状态。

Codex 必须封装真实行为。

---

# 30. 可编辑文本统一交互

所有包含可编辑文本的控件：

第一次获得编辑焦点时：

```text
Select All
```

用户可以直接：

```text
输入覆盖
或
Delete
```

已经处于编辑焦点后再次点击：

```text
正常定位 Caret
```

禁止每次点击都重新 Select All。

---

# 31. 数值控件统一交互

用于调节数值的控件，原则上支持：

```text
Text Input
Keyboard
Pointer Scrub
Step
Min / Max / Clamp（按需要）
```

其中：

> 数值文本区域应支持按住鼠标拖动微调。

复合数值控件如果内部已有 `XYNumberField`：

> 必须直接复用。

---

# 32. Template 是组件 Contract 的一部分

Runtime Template 不能只为了“看起来对”。

必须与以下保持一致：

```text
Public API
State
Sizing
Density
Token
Interaction
Accessibility
```

因此：

> Runtime 组件 Template 的最终封装责任属于 Codex。

Gemini 可以提供视觉实现方案、Gallery XAML / SVG 和 Presentation 代码，但不能在 Gallery 里绕过 Runtime Template 造第二实现。

---

# 33. Gemini 的 UI Code 边界

这里的“Gemini 负责 UI 代码”明确指：

```text
Presentation XAML
Gallery XAML
Gallery Section
Gallery Layout
Gallery Example
SVG
Visual Composition
Responsive Presentation
```

如果视觉方案需要 Runtime Template 变化：

```text
Gemini
→ 输出 Presentation / 视觉要求
→ Codex
→ 封装进 Runtime Component
```

这样不会造成：

```text
Gallery 看起来正确
Runtime 实际错误
```

---

# 34. Codex 的组件封装边界

Codex 必须把最终组件封成：

```text
Type
Public API
Defaults
Inheritance
Override
Template Contract
Interaction
State
Token Consumption
Size Consumption
Density Consumption
Tests
```

真正完成后，Gemini 才能像普通 Consumer 一样使用。

---

# 35. 5+100

XYUI 继续执行：

> **5+100 + SRP**

主要源码：

```text
.cs
.axaml
.js
```

原则上单文件控制在约：

```text
≤ 100 行
```

不是机械法律，而是代码职责失控的强信号。

---

# 36. 文件拆分必须按职责

正确：

```text
XYComboBox.cs
XYComboBox.Template.cs
XYComboBox.Keyboard.cs
XYComboBox.Filter.cs
```

正确：

```text
XYDockTab.cs
XYDockTab.Drag.cs
XYDockTab.Template.cs
```

错误：

```text
XYComboBox.Part1.cs
XYComboBox.Part2.cs
XYComboBox.Part3.cs
```

规则：

> **Partial 按责任命名，不按行数切。**

---

# 37. 一个文件一个主要职责

每个文件必须能回答：

```text
“它只负责什么？”
```

如果回答是：

```text
模板 + 键盘 + Drag + Data + Resolver + Test Helper
```

则职责已经过多。

---

# 38. 不允许 Opportunistic Refactor

Codex 封装当前控件时不得顺手：

- 重命名整个 Foundation；
- 改无关 Token；
- 移动无关目录；
- 重构其它组件；
- 修改未授权 Runtime；
- 引入新依赖；
- 改 Schema；
- 改公开 API。

需要扩大 Scope：

```text
STOP
→ 报告用户
```

---

# 39. Runtime 缺能力时停止，不伪造

Gemini 如果在做 Gallery 时发现：

```text
设计需要 XY.Border
但 Runtime 没有
```

正确：

```text
Gemini STOP
→ 报告缺口
→ Codex 封装
→ Gemini 继续消费
```

错误：

```text
Gemini
→ 手写 Border
→ 文档里假装 XY.Border 已实现
```

---

# 40. Gallery 是消费层，不是实验性 Runtime

Gallery 可以：

- 展示；
- 比较；
- 教学；
- Live Lab；
- 状态矩阵；
- Density 对比；
- API 示例。

Gallery 不能：

- 定义正式规则；
- 修补 Runtime；
- 保存第二份 Token；
- 模拟不存在的 API；
- 复制 Canonical Geometry；
- 复制 Canonical Resolver。

---

# 41. Gallery 全中文原则

Gallery 面向展示的普通 UI：

> **全中文。**

以下可保持英文：

```text
组件名
API
XAML
代码
类型名
Token Key
```

例如：

```text
尺寸等级
Compact / Default / Comfortable / Touch
```

是允许的。

---

# 42. Gallery 示例必须真实

Gallery 展示：

```xml
xy:XY.Size="Compact"
```

就必须证明：

- Runtime 已有 XY.Size；
- 控件真实消费；
- UI 变化来自 Runtime；
- 不是 Gallery 手写 Height。

同理：

```xml
xy:XY.Gap="..."
```

必须来自真实 Gap Resolver。

---

# 43. Gallery 不能手写尺寸冒充 Size

错误：

```xml
<xy:XYButton
    Height="28" />
```

然后页面写：

```text
Compact
```

正确：

```xml
<xy:XYButton
    xy:XY.Size="Compact" />
```

---

# 44. Gallery 装修不能污染 Runtime

Gemini 可以为 Gallery 页面自身使用：

- Section Layout；
- Page Grid；
- Demo Frame；
- Preview Background；
- Documentation Typography。

但这些 Presentation 代码：

> 不得反向进入 Runtime Component Contract。

Gallery 装修和 Runtime 默认外观必须分层。

---

# 45. Foundation 示例优先展示短语法

Gallery 的代码示例应优先展示：

```xml
xy:XY.Size="Default"
xy:XY.Density="Compact"
xy:XY.Padding="..."
xy:XY.Gap="..."
```

而不是：

```text
内部 Resolver
内部 Scope
内部 Metadata
内部 C# Helper
```

内部实现只在架构文档中解释。

---

# 46. DynamicResource = 0 的准确表述

如果审计得到：

```text
DynamicResource = 0
```

只能解释为：

> **本次扫描的目标范围内，未发现对应 DynamicResource 使用。**

不得扩大解释为：

```text
整个 XYUI 永远不用 DynamicResource
```

也不得写：

```text
全仓库 Theme 已完全静态化
```

除非真的有覆盖全仓库的独立证据。

---

# 47. 测试属于 Runtime Contract

Codex 新增或修改 Runtime 行为时，必须考虑定向测试。

至少验证：

```text
API
Default
Inheritance
Override
State
Interaction
Sizing
Density
Regression
```

---

# 48. 测试必须被 Runner 发现

创建测试文件不等于有测试。

报告必须能说明：

```text
Created
Discovered
Executed
Passed
```

例如：

```text
新增测试：4
Runner discovered：4
Passed：4 / 4
```

---

# 49. 固定 SDK

XYUI / XuanYuEngine .NET 构建统一使用：

```text
D:\MyApp\sdk-dotnet\dotnet.exe
```

不得默认改用：

```text
C:\Program Files\dotnet\dotnet.exe
```

也不得使用旧路径：

```text
D:\qizheng-interplay\tools\dotnet\dotnet.exe
```

---

# 50. Codex 技术门禁

正式提交人工验收前至少运行：

```text
Build
Tests
ARCH-A
5+100
git diff --check
```

目标报告：

```text
Build          0 Warning / 0 Error
Tests          PASS
ARCH-A         PASS
5+100          PASS
diff-check     PASS
```

---

# 51. Gallery Runtime 验证

仅仅：

```text
Gallery 启动成功
```

不能写成：

```text
Gallery PASS
```

必须实际检查当前页面：

- 内容是否出现；
- 是否真实 Runtime；
- Padding；
- Gap；
- Alignment；
- Text；
- Icon；
- Border；
- Radius；
- Density；
- Size；
- Hover；
- Focus；
- Disabled；
- Popup；
- Interaction；
- Responsive。

---

# 52. 技术通过与人工通过分开

Codex 可以报告：

```text
TECHNICAL PASS
```

Gemini 可以报告：

```text
PRESENTATION IMPLEMENTED
```

但只有用户可以报告：

```text
USER VISUAL ACCEPTED
```

---

# 53. UI 任务标准流程

```text
用户需求
↓
Gemini 出 SVG / Presentation 方案
↓
用户定稿
↓
Codex 定义 / 封装 Runtime Contract
↓
Codex 完成组件封装
↓
Gemini 用真实 Runtime 完成 Gallery
↓
Codex 跑 Build / Tests / ARCH-A / 5+100
↓
用户人工验收
↓
Codex Commit + Push
```

---

# 54. 已有 Runtime、只装修 Gallery 时

```text
用户指出 Gallery 问题
↓
Gemini 审计 Presentation
↓
确认 Runtime 已有真实能力
↓
Gemini 只改 Gallery
↓
Codex 验证没有污染 Runtime / Contract
↓
Build / Tests / Gallery
↓
用户验收
```

---

# 55. Runtime 缺能力时

```text
Gemini 发现 Gallery 无法真实表达设计
↓
STOP
↓
列出 Missing Runtime Capability
↓
Codex 审计
↓
Codex 封装
↓
Tests
↓
Gemini 继续 Gallery
```

不允许 Gemini 临时造假完成页面。

---

# 56. Codex 封装标准检查表

一个 XYUI 能力在 Codex 侧完成前检查：

- [ ] 有明确 Semantic
- [ ] 有真实 Runtime Owner
- [ ] Public API 简短
- [ ] Attached Property 命名统一
- [ ] 有 Default
- [ ] 有 Inheritance 规则
- [ ] 有 Local Override
- [ ] 有 Canonical Resolver
- [ ] 无第二真值
- [ ] 无 Hidden Border
- [ ] Gap 无 Margin 模拟
- [ ] 真实组件消费
- [ ] 复合控件复用已有 XYUI
- [ ] Interaction 封装真实
- [ ] State 封装真实
- [ ] Tests 被 Runner 发现
- [ ] 5+100 / SRP 合格

---

# 57. Gemini Presentation 检查表

Gemini 完成 Gallery 前检查：

- [ ] 使用真实 XYUI Runtime
- [ ] 没有假控件
- [ ] 没有伪 API
- [ ] 没有手写 Height 模拟 XY.Size
- [ ] 没有 Margin 模拟 XY.Gap
- [ ] 没有复制 Runtime Rule
- [ ] 没有 Gallery-only Token 冒充 Canonical
- [ ] 页面中文统一
- [ ] API / 代码示例真实
- [ ] 视觉层级统一
- [ ] 对齐统一
- [ ] 信息密度合理
- [ ] Responsive 合理
- [ ] 所有示例都能追溯到 Runtime

---

# 58. 双 Agent 文件所有权

建议每轮任务先列：

```text
CODEX OWNERSHIP
- src/XYUI.Avalonia/...
- Runtime Tests
- Architecture / Guard
- Runtime helper / resolver

GEMINI OWNERSHIP
- XYUI.Avalonia.Gallery/Views/...
- Gallery Sections
- Gallery Presentation XAML
- SVG / Visual assets
```

双方不得静默跨界修改。

---

# 59. 跨界修改规则

若 Gemini 确实需要改 Runtime：

```text
必须先 STOP
→ 用户批准
→ 明确临时 Ownership
```

若 Codex 确实需要改 Gallery：

```text
仅限为 Runtime 接线 / 编译 / 最小验证所必需
```

大规模 Presentation 调整仍应交回 Gemini。

---

# 60. 两次失败停止

同一问题连续两轮修复仍失败：

```text
STOP
```

重新检查：

- 是否 Root Cause 错；
- 是否旧 DLL；
- 是否旧 Gallery；
- 是否 Theme 覆盖；
- 是否 Binding 错；
- 是否 Runtime / Gallery 所有权混淆；
- 是否第二真值；
- 是否代码没有真实被编译。

禁止继续盲改。

---

# 61. Git 收口

用户人工通过后由 Codex 收口：

```text
git status
git diff
git diff --check
commit
push
remote verify
```

未经用户授权禁止：

- force push；
- rebase；
- merge；
- delete branch；
- rewrite history；
- tag；
- release。

---

# 62. 审计留痕

必要时更新：

```text
changelog.md
file-tree.md
```

分别记录：

```text
本轮改了什么
为什么
Root Cause
哪些文件
测试结果
Gallery 结果
人工验收
Commit
```

---

# 63. Codex 标准报告

```text
# CODEX RUNTIME REPORT

## SCOPE
...

## RUNTIME CONTRACT
...

## COMPONENT ENCAPSULATION
...

## PUBLIC API
...

## REUSE
...

## TESTS
Created:
Discovered:
Passed:

## GATES
Build       PASS
Tests       PASS
ARCH-A      PASS
5+100       PASS
diff-check  PASS

## STATUS
READY FOR GALLERY / USER REVIEW
```

---

# 64. Gemini 标准报告

```text
# GEMINI PRESENTATION REPORT

## PAGE
...

## REAL XYUI COMPONENTS USED
- ...

## PRESENTATION CHANGES
- ...

## RESPONSIVE
...

## RUNTIME GAPS
None
或
- ...

## OWNERSHIP
Gallery-only changes
No Runtime ownership files modified

## STATUS
READY FOR USER VISUAL REVIEW
```

---

# 65. 最终收口状态

只有满足：

```text
Codex Runtime PASS
+
Gemini Presentation 完成
+
Technical Gates PASS
+
User Visual / Interaction ACCEPTED
```

才能：

```text
FINAL CLOSEOUT
```

---

# 66. 双 Agent 最终冻结工作流

今后 XYUI 默认执行：

```text
① 用户定义目标
↓
② Gemini 负责 SVG / UI / Gallery Presentation
↓
③ 用户确认视觉方向
↓
④ Codex 负责 Runtime / API / Component 封装
↓
⑤ Gemini 用真实 Runtime 完成 Gallery 装修
↓
⑥ Codex 跑 Tests / Build / ARCH-A / 5+100 / diff-check
↓
⑦ 用户真实 Gallery 人工验收
↓
⑧ Codex Commit + Push
↓
⑨ changelog / file-tree
↓
⑩ 下一项
```

如果 Runtime 已经完成：

```text
直接从 Gemini Gallery Presentation 开始
```

如果 Gemini 发现 Runtime 缺能力：

```text
停止伪造
→ 退回 Codex 封装
```

---

# 67. 最重要的代码封装十条

以后审计 XYUI，只需要先问这十条：

```text
1. Runtime 是不是唯一真值？
2. Gallery 有没有自己造规则？
3. Public API 是不是短 XAML DSL？
4. Facade 是不是真的接到了 Canonical Resolver？
5. Default / Inheritance / Override 是否完整？
6. Size / Density / Token 是否由组件真实消费？
7. 复合控件有没有直接复用已有 XYUI？
8. 有没有 Hidden Border / Margin 模拟 Gap / 手写 Height？
9. 文件是否按 SRP + 5+100 拆分？
10. 测试、ARCH-A、Build、Gallery、人工验收是否都是真实证据？
```

---

# 68. 一句话规范

> **Gemini 负责把 XYUI 展示正确、装修漂亮；Codex 负责把 XYUI 封装正确、代码可靠。Gallery 永远消费 Runtime，Runtime 永远只有一个 Canonical 真值，任何视觉便利都不能制造第二套规则。**
