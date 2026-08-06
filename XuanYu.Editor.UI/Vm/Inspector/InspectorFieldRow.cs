namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4（G03）：检查器字段行——结构化「标签/值」替代拼接字符串。
// IsGroupHeader=true 表示全宽分组标题行（如「变换」「标记」），不参与标签列布局。
public sealed record InspectorFieldRow(string Label, string Value, bool IsGroupHeader = false);
