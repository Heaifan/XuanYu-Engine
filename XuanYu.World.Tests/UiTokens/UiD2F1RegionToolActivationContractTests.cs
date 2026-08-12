namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD2F1RegionToolActivationContractTests
{
    static string ReadTop() => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Top", "Top.axaml"));

    [Fact]
    public void Top_exposes_region_drawing_only_in_region_edit_mode()
    {
        var top = ReadTop();
        Assert.Contains("IsVisible=\"{Binding IsRegionEditMode}\"", top);
        Assert.Contains("IsEnabled=\"{Binding CanStartRegionDrawing}\"", top);
        Assert.Contains("IsChecked=\"{Binding IsRegionDrawingTool, Mode=OneWay}\"", top);
        Assert.Contains("Command=\"{Binding SelectToolCommand}\"", top);
        Assert.Contains("CommandParameter=\"区域绘制\"", top);
        Assert.Contains("Data=\"{StaticResource RegionIcon}\"", top);
        Assert.Contains("Text=\"绘制区域\"", top);
    }
}
