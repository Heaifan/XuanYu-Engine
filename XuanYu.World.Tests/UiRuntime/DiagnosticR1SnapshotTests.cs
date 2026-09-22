using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticR1SnapshotTests
{
    [Theory]
    [InlineData("XYText", "XYUI1 · XYText")]
    [InlineData("XYButton", "XYUI2 · XYButton")]
    [InlineData("XYSplitButton", "XYUI2 · XYSplitButton")]
    [InlineData("XYTextField", "XYUI2 · XYTextField")]
    public void Canonical_xyui_type_resolves_to_stable_user_index(string typeName, string expected)
    {
        var control = Control(typeName);
        Assert.Equal(expected, DiagnosticXyuiResolver.Resolve(control).DisplayIndex);
    }

    [Fact]
    public void Native_control_is_explicitly_unmapped()
    {
        var identity = DiagnosticXyuiResolver.Resolve(new Button());
        Assert.False(identity.IsMapped);
        Assert.Equal("无", identity.DisplayIndex);
    }

    [Fact]
    public void Debug_id_is_not_required_for_real_type_and_path()
    {
        var target = new TextBlock { Name = "InnerText", Text = "保存" };
        var result = DiagnosticProbeResolver.Resolve(target, deepVisual: true);
        var snapshot = DiagnosticElementSnapshot.Capture(result);
        Assert.Equal("TextBlock", snapshot.ComponentType);
        Assert.Contains("InnerText", snapshot.VisualPath);
        Assert.Equal("N/A", snapshot.DebugId);
    }

    static Control Control(string typeName) => typeName switch
    {
        "XYText" => new XYText(),
        "XYButton" => new XYButton(),
        "XYSplitButton" => new XYSplitButton(),
        "XYTextField" => new XYTextField(),
        _ => new Control()
    };
}
