# XYUI 文本输入交互审计

日期：2026-08-29

## 结论

XYUI-0 新增 `0.33 · Text Input Interaction` 作为所有可编辑文本入口的基础规则：激活非空文本默认全选，输入直接替换旧值；编辑焦点建立后占位文本立即隐藏，避免输入法预编辑与占位内容重叠。

## XYUI-1 审计（01–24）

XYUI-1 的 24 个组件均为展示 / 信息文本，不是自由编辑入口。01–20、22–24 为展示文本；21 `SelectableText` 允许选择与复制但明确保持 `ReadOnly=True`。因此 XYUI-1 不应自行增加“点击进入编辑”行为，也不需要接入全选替换规则。

## XYUI-2 审计（01–24）

| 组件 | 文本编辑结论 | 规则归属 |
| --- | --- | --- |
| 01–08 Button / Choice | 无文本编辑 | 不适用 |
| 09 TextField | 可编辑 | XYUI-0 · 0.33 |
| 10 NumberField | 可编辑 | 继承 TextField / 0.33 |
| 11 Slider | 内嵌可编辑 NumberField | 继承 NumberField / 0.33 |
| 12 ComboBox | 可编辑文本 + 候选列表 | 内嵌 TextField / 0.33 |
| 13 Select | 固定候选，不允许自由输入 | 不适用 |
| 14 TextArea | 可编辑多行文本 | XYUI-0 · 0.33 |
| 15 SearchField | 规范定义为关键词输入 | 实装时继承 0.33 |
| 16 PasswordField | 规范定义为敏感文本输入 | 实装时继承 0.33 |
| 17 DatePicker | 规范定义为分段编辑 | 实装时继承 0.33 |
| 18 TimePicker | 规范定义为分段编辑 | 实装时继承 0.33 |
| 19 ColorPicker | HEX / Alpha 字段属于文本输入 | 实装时继承 0.33 |
| 20 Bool Property | 复用 Switch，无文本编辑 | 不适用 |
| 21 Number Property | 复用 NumberField | 继承 0.33 |
| 22 Vector Property | 每轴复用 NumberField | 继承 0.33 |
| 23 Enum Property | 复用固定 Select | 不适用 |
| 24 Reference Property | 由未来身份 / 引用交互定义 | 实装文本入口时继承 0.33 |

## 当前实装复核

- `XYTextField`、`XYNumberField`、`XYComboBox` 文本宿主与 `XYTextArea` 统一使用共享可编辑文本基类。
- `XYTextField` 获得编辑焦点后隐藏自定义占位层；`XYTextArea` 的 Placeholder 映射到 Avalonia 原生 TextBox PlaceholderText 合同。
- ReadOnly 文本与 XYUI-1 `SelectableText` 保持只读选择 / 复制语义。
- 自动测试覆盖 TextField 的焦点、鼠标激活、占位层，以及 TextArea 的共享全选合同；ComboBox 继续由 XYUI-2-12 专项测试覆盖。

## 遗留

XYUI-2-15～24 当前只有规范合同，尚无本仓库运行时控件；待各组件实装时必须按本审计表接入 XYUI-0 · 0.33，不得另起一套文本激活语义。
