using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticR1ReportTests
{
    [Fact]
    public void Index_report_uses_chinese_labels_and_canonical_identity()
    {
        var target = new Button { Name = "SaveButton", Content = "保存" };
        var result = DiagnosticProbeResolver.Resolve(target, deepVisual: true);
        var snapshot = DiagnosticElementSnapshot.Capture(result);
        var text = DiagnosticReportFormatter.FormatIndex(snapshot);
        Assert.Contains("XYUI索引：无", text);
        Assert.Contains("实例名称：SaveButton", text);
        Assert.Contains("调试编号：N/A", text);
        Assert.DoesNotContain("ControlType", text);
    }

    [Fact]
    public void Ai_report_is_structured_plain_text_without_json()
    {
        var target = new Button { Name = "SaveButton", Content = "保存" };
        var snapshot = DiagnosticElementSnapshot.Capture(DiagnosticProbeResolver.Resolve(target, true));
        var text = DiagnosticReportFormatter.FormatAi(snapshot);
        Assert.Contains("[XYEngine UI 诊断报告]", text);
        Assert.Contains("一、元素身份", text);
        Assert.Contains("二、视觉路径", text);
        Assert.Contains("三、布局信息", text);
        Assert.Contains("四、状态信息", text);
        Assert.Contains("五、诊断信息", text);
        Assert.DoesNotContain("{", text);
    }
}
