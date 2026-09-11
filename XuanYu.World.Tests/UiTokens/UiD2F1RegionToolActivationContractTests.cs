namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD2F1RegionToolActivationContractTests
{
    static string ReadTop() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Top", "Top.axaml"));
    static string ReadLeft() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Left", "Left.axaml"));
    static string ReadToolbar() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Top", "ContextToolBar.axaml"));

    [Fact]
    public void Top_exposes_region_drawing_only_in_region_edit_mode()
    {
        var top = ReadTop(); var left = ReadLeft(); var toolbar = ReadToolbar();
        Assert.Contains("ContextToolBar", top);
        Assert.DoesNotContain("RegionWorkspace", left);
        var inspector = File.ReadAllText(Path.Combine(
            AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));
        Assert.DoesNotContain("RegionalAuthoringPanel", inspector);
        Assert.Contains("IsEnabled=\"{Binding CanRequestRegionDrawing}\"", toolbar);
        Assert.Contains("Click=\"BeginRegionDrawing_Click\"", toolbar);
        Assert.Contains("CanUndoRegionDrawingVertex", toolbar);
        Assert.Contains("CanCompleteRegionDrawing", toolbar);
        Assert.Contains("CanCancelRegionDrawing", toolbar);
    }
}
