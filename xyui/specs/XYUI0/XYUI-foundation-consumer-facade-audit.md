# XYUI Foundation Consumer Facade API Audit

状态：RUNTIME IMPLEMENTED（Gallery 未改；等待联合验收）。本文记录已裁决的 Facade 范围、Runtime 接线与验证事实。

## 基线

```text
Physical Path: D:\MyDoc\project-vsCode\XYUI
Branch: feat/XYUI-A
HEAD: c6724fa858b514f0120cd883f0f271cb6cc5ed86
Ahead / Behind: 0 / 0
Working Tree: 非 clean；保留既有 Gemini Gallery 与 Codex Runtime 修改
```

## Existing / Missing

| Facade | Status | Current fact |
| --- | --- | --- |
| `xy:XY.Size` | EXISTS | `XY.Size` attached property，继承，驱动控件高度/图标尺寸 |
| `xy:XY.Density` | EXISTS | `XY.Density` attached property，继承，驱动密度 scope |
| `xy:XY.Foreground` / `xy:XY.Surface` | IMPLEMENTED | 明确区分文本/图标前景与表面背景，统一 Canonical Color resolver |
| `xy:XY.Font` / `xy:XY.Typography` | IMPLEMENTED | 统一引用既有 Font/Type token，禁止重复数字 |
| `xy:XY.Padding` | IMPLEMENTED (scoped) | 仅 `Border` / `TemplatedControl`，不模拟任意 Panel |
| `xy:XY.Gap` | IMPLEMENTED (scoped) | 第一阶段仅 `StackPanel.Spacing` |
| `xy:XY.Margin` | IMPLEMENTED | 映射 `Control.Margin`，由 Canonical spacing resolver 解析 |
| `xy:XY.Radius` | IMPLEMENTED (scoped) | 仅 `Border` / `TemplatedControl`，消费既有 Radius token |
| `xy:XY.Border` | IMPLEMENTED (scoped) | 仅 `Border`，同时写入 Canonical brush 与 thickness |

## Confirmed facts

### Color

`XyuiColorTokens` 同时包含 `XY.Color.*` Core Palette、`XY.Text.*`、`XY.Icon.*`、`XY.Surface.*`、`XY.Border.*`、`XY.Accent.*` 和状态语义色。它们的视觉通道不同：

- Text controls / `XYHeading` / `XYText` / `XYCaption` / `XYLabel`：通常映射到 Foreground。
- `XYIcon`：映射到 Stroke，而不是 Foreground。
- `XYButton` / `XYIconButton`：已有 Variant/state 语义，Color override 需要定义与 Variant、Focus、Disabled 的优先级。
- `Border`：需要 Background/BorderBrush/Fill 的拆分，不能由单一 `XY.Color` 猜测。

因此直接冻结 `xy:XY.Color` 为“对所有目标自动猜通道”会制造不稳定 API。当前结论：**COLOR FACADE SEMANTIC SPLIT REQUIRED**。建议先设计 `XY.Foreground`、`XY.IconColor`、`XY.Surface` 或等价的明确语义，而不是偷偷实现模糊的 Color。

### Font / Typography

`XY.Font.*` Resource 已存在，当前组件通过样式消费。`XY.Type.*` 角色 Registry 尚不存在；`XYHeading`、`XYText`、`XYCaption`、`XYLabel` 已分别绑定排版事实。直接增加字符串 `xy:XY.Typography` 前，必须把角色、字体、字号、行高、字重和组件默认值接到同一 resolver，否则会形成第二套 Typography 真值。

### Padding

`StackPanel` / `Panel` 没有可统一设置的原生 Padding。以下方案均不应未经裁决实施：

- 隐形 Border：改变 Logical/Visual Tree、Measure/Arrange、HitTest 语义。
- 遍历子项写 Margin：覆盖业务 Margin、处理动态 children 困难，并产生第三套布局系统。
- 对任意 Panel 强行反射：不可预测且不符合 Avalonia 属性模型。

当前结论：**PADDING FACADE TARGET SCOPE BLOCKER**。应先限定支持的 XYUI 容器，或增加明确的 XYUI 容器控件；不能宣称任意 `StackPanel` 已支持 `xy:XY.Padding`。

### Gap

`XYToolbar` 已有真实内部 Gap：Compact=2、非 Compact=4；普通 `StackPanel` 原生支持 `Spacing`。可以设计一个仅作用于 `StackPanel` 的 Facade，但必须明确 unsupported target，不得通过 child Margin hack 泛化。

### Margin

`Margin` 可以安全映射到 `Control.Margin`，但本轮尚无统一 typed spacing reference/parser。应复用同一个 Canonical spacing resolver，并定义单值如何转换为 `Thickness`；不能散落字符串比较。

### Radius / Border

`XY.Radius.*` 已有 `CornerRadius` Resource，`xyui-border-*` 已有完整 Border Brush/Thickness/Radius 语义。Facade 目标可限定为 `Border` 和 `TemplatedControl`，显式 Facade 优先于组件默认样式；不应声称任意 Avalonia object 都支持。

## Proposed implementation order after裁决

1. 先建立 typed canonical reference/parser，统一解析 Color、Font、Typography、Spacing、Radius、Border 名称。
2. 先实现无语义歧义的 `Margin`、`Gap(StackPanel only)`、`Radius(Border/TemplatedControl)`、`Border(Border only)`。
3. 建立 Typography role registry 后再实现 Font/Typography Facade，并让 convenience controls 消费同一 resolver。
4. 明确 Color 通道拆分后再实现颜色 Facade。
5. Padding 最后实现，且仅在明确支持范围内，不改变树结构或用 Margin 模拟。
6. Size/Density 只做 resolver/测试复用，不改公共 API。

## Runtime implementation facts

Runtime 位于 `xyui/avalonia/src/XYUI.Avalonia/Facade/`，由一个集中 resolver 和职责分离的 partial API 文件组成。Facade 字符串只在入口出现；Canonical token 解析集中在 resolver。Unsupported target 通过 `Trace.TraceWarning` 明确诊断，不创建隐藏 Border、不遍历子项写 Margin。

Consumer XAML 形态：

```xml
<Border xy:XY.Surface="XY.Surface.Panel"
        xy:XY.Padding="XY.Panel.Padding"
        xy:XY.Radius="XY.Radius.Panel"
        xy:XY.Border="XY.Border.Default">
  <StackPanel xy:XY.Gap="XY.Space.3">
    <TextBlock xy:XY.Foreground="XY.Text.Link"
               xy:XY.Typography="XY.Type.Body" />
  </StackPanel>
</Border>
```

验证：核心库构建通过；Facade Runtime 3 项事实测试通过；ARCH-A 与 `git diff --check` 通过。全量测试命令的宿主未返回汇总，需在清理测试进程后单独复跑；本轮不改 Gallery、不 commit、不 push。

```text
FACADE DESIGN BLOCKER
RUNTIME CHANGES = 0
PUBLIC API CHANGES = 0
GALLERY CHANGES = 0
DO NOT COMMIT
DO NOT PUSH
```
