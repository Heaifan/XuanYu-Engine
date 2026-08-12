namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD2F1RegionToolActivationContractTests
{
    static string ReadTop() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Top", "Top.axaml"));
    static string ReadLeft() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Left", "Left.axaml"));
    static string ReadRegionPanel() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Left", "RegionPanel.axaml"));

    [Fact]
    public void Top_exposes_region_drawing_only_in_region_edit_mode()
    {
        var top = ReadTop(); var left = ReadLeft(); var region = ReadRegionPanel();
        Assert.DoesNotContain("Text=\"绘制区域\"", top);
        Assert.Contains("IsVisible=\"{Binding IsRegionEditMode}\"", left);
        Assert.Contains("IsEnabled=\"{Binding CanRequestRegionDrawing}\"", region);
        Assert.Contains("Click=\"RegionDrawing_Click\"", region);
        Assert.Contains("Content=\"绘制区域\"", region);
        Assert.Contains("CanUndoRegionDrawingVertex", region);
        Assert.Contains("CanRedoRegionDrawingVertex", region);
        Assert.Contains("CanCompleteRegionDrawing", region);
        Assert.Contains("CanCancelRegionDrawing", region);
    }
}
