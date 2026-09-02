# XYTEXTFIELD RADIUS DRIFT FINAL REPORT

状态：INPUT RADIUS CANONICALIZED。已按批准方案修改 Runtime；未修改 Gallery，未 commit/push。

## CURRENT

- `XYTextField Actual`：4 DIP。
- `Source`：`XyuiControlStyles.Input(...)` 消费动态资源 `XY.Radius.Input`。
- Template `XYTextField.Template.cs` 只把控件 CornerRadius 绑定到模板根 Border，不产生数值。
- 该 3 DIP helper 还被 `XYNumberField`、`XYTextArea`、`XYSearchField`、`XYPasswordField`、`XYDatePicker`、`XYTimePicker`、`XYColorPicker` 复用。

## CANONICAL

- `RadiusInput`：4 DIP。
- Token：`xyui/avalonia/src/XYUI.Avalonia/Spatial/XyuiSpatialTokens.cs` 的 `RadiusInput`。
- Resource：`xyui/avalonia/src/XYUI.Avalonia/Spatial/XyuiSpatial.cs` 的 `XY.Radius.Input`。
- 规格来源：`xyui/specs/XYUI2/XYUI-2.canonical.md`、`XYUI-2.mapping.json` 将 TextField、NumberField、ComboBox、Select、TextArea、SearchField、PasswordField、DatePicker、TimePicker、ColorPicker、ReferenceProperty 的 Radius 指向 `XY.Radius.Input`。
- `RadiusInput` 是实际 Canonical：输入族顶层样式现在统一消费它，Runtime 第二真值已移除。

## CONSUMERS

| Control | Current | Canonical source | Actually consumes `RadiusInput`? |
| --- | ---: | --- | --- |
| XYTextField | 4 | XY.Radius.Input = 4 | Yes |
| XYNumberField | 4 | XY.Radius.Input = 4 | Yes |
| XYTextArea | 4 | XY.Radius.Input = 4 | Yes |
| XYSearchField | 4 | XY.Radius.Input = 4 | Yes |
| XYPasswordField | 4 | XY.Radius.Input = 4 | Yes |
| XYDatePicker / XYTimePicker | 4 | XY.Radius.Input = 4 | Yes |
| XYColorPicker | 4 | XY.Radius.Input = 4 | Yes |
| XYComboBox | 4 | XY.Radius.Input = 4 | Yes；独立 ComboBox root Setter |
| XYSelect | 4 | XY.Radius.Input = 4 | Yes；独立 Select root Setter |
| XYReferenceProperty | 4 | XY.Radius.Input = 4 | Yes；独立 Property Setter |

ComboBox 内嵌 TextField 另被明确设为 0 DIP，这是复合控件内部拼接边界，不是顶层输入 Radius。

## OPTION A — XYTextField 3 → 4

Pros：符合 Foundation token、XYUI-2 mapping 和现有设计意图；消灭输入族的第二真值；与 Button/Control 的 4 DIP 语义一致。

Cons：会改变所有复用 `Input(...)` 的输入控件外观；若只改 TextField，会留下输入族不一致，因此应按输入族统一迁移，并同步 ComboBox、Select、ReferenceProperty 等独立 3 DIP Setter。

Impact：视觉变化是角部增加 1 DIP，公共 API、模板名称和绑定合同不变；现有专门覆盖 CornerRadius 的复合控件内部值（如 ComboBox embedded=0、Search/Password action cell 局部圆角）需保持局部几何语义。

## OPTION B — RadiusInput 4 → 3

Pros：匹配当前多数输入控件视觉实现，改动表面较小。

Cons：违背已有 Foundation source、XYUI-2 canonical/mapping 和资源命名意图；会把 Button/Control 的 4 DIP 与 Input 语义拆开；需要修改 token、资源、SpatialTokenTests、XYUI-2 规格/映射及所有引用审计。

Impact：不是单纯改一个常量，而是 Canonical 迁移；会改变 token contract，且不能解释现有 4 DIP Input 设计记录。

## RECOMMENDATION

推荐 **Option A：输入族统一到 `XY.Radius.Input = 4 DIP`**。理由是 4 DIP 有完整的 Foundation/XYUI-2 规范证据，3 DIP 只有 Runtime 历史硬编码证据。最终实施范围应覆盖所有顶层 Input-like 控件，并保留复合控件内部为拼接而存在的 0/局部 CornerRadius。

## BREAKING CHANGE

Public API：NO。Template Contract：NO。Visual compatibility：YES，存在 1 DIP 圆角变化；需要人工/截图复验。

## FILES THAT WOULD CHANGE AFTER APPROVAL

- `XyuiControlStyles.InputFamily.cs`：公共 Input helper 改为消费 `XY.Radius.Input`。
- `XyuiControlStyles.ComboBox.cs`、`XyuiControlStyles.Select.cs`、`XyuiControlStyles.PropertyControls.cs`：独立顶层输入样式接回同一资源。
- 相关测试与 07 Facts/Gallery 文案；不修改复合控件内部局部 CornerRadius。

## TEST IMPACT

- 当前测试锁定 `RadiusInput` resource 值，但未锁定所有输入控件的实际 CornerRadius；应新增输入族 runtime assertions。
- 现有截图/人工验收若覆盖输入控件，需要重新复验；未发现公共 API 或模板结构断言会因 3→4 失败。

## FINAL STATE

INPUT RADIUS CANONICALIZED
SECOND TRUTH REMOVED
XY.Radius.Input = 4 DIP
DO NOT COMMIT
DO NOT PUSH
