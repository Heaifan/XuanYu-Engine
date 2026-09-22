namespace XuanYu.Editor.UI;

public static class DiagnosticReportFormatter
{
    public static string FormatIndex(DiagnosticElementSnapshot s) => Lines(
        $"XYUI索引：{s.Identity.DisplayIndex}", $"实例名称：{s.InstanceName}",
        $"调试编号：{s.DebugId}");

    public static string FormatSummary(DiagnosticElementSnapshot s) => Lines(
        "[XYEngine 元素摘要]", FormatIndex(s), $"组件来源：{s.Identity.Source}",
        $"尺寸：{s.ActualSize}", $"状态：{s.PointerOver}");

    public static string FormatAi(DiagnosticElementSnapshot s) => Lines(
        "[XYEngine UI 诊断报告]", "", "一、元素身份", "",
        FormatIdentity(s), "", "二、视觉路径", "", s.VisualPath, "",
        "三、布局信息", "", Layout(s), "", "四、状态信息", "", State(s), "",
        "五、诊断信息", "", $"XYUI映射：{(s.Identity.IsMapped ? "成功" : "无")}",
        "异常：未检测到");

    static string FormatIdentity(DiagnosticElementSnapshot s) => Lines(
        $"XYUI索引：{s.Identity.DisplayIndex}", $"XYUI编号：{s.Identity.Number ?? "无"}",
        $"组件名称：{Value(s.Identity.ComponentName)}", $"实例名称：{s.InstanceName}",
        $"调试编号：{s.DebugId}", $"组件来源：{s.Identity.Source}");

    static string Layout(DiagnosticElementSnapshot s) => Lines(
        $"实际尺寸：{s.ActualSize}", $"期望尺寸：{s.DesiredSize}", $"位置：{s.Position}",
        $"外边距：{s.Margin}", $"内边距：{s.Padding}", $"水平对齐：{s.HorizontalAlignment}",
        $"垂直对齐：{s.VerticalAlignment}", "裁剪状态：不可用", "滚动容器：无", "层级：不可用");

    static string State(DiagnosticElementSnapshot s) => Lines(
        $"可见：{s.Visible}", $"启用：{s.Enabled}", $"获得焦点：{s.Focused}",
        $"鼠标悬停：{s.PointerOver}", $"按下：{s.Pressed}", $"选中：{s.Selected}",
        $"展开：{s.Expanded}", $"伪类状态：{s.PseudoClasses}");

    static string Lines(params string[] lines) => string.Join(Environment.NewLine, lines);
    static string Value(string value) => string.IsNullOrWhiteSpace(value) ? "无" : value;
}
