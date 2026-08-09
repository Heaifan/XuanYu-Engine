namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD2F1RegionToolContractTests
{
    static string Read(string path) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", path));

    [Fact]
    public void Region_tool_is_owned_by_map_editor_not_top_navigation()
    {
        var top = Read("Top/Top.axaml");
        var map = Read("Right/MapPagePanel.axaml");
        Assert.DoesNotContain("CommandParameter=\"区域绘制\"", top);
        Assert.Contains("x:Name=\"RegionDrawingTool\"", map);
        Assert.Contains("Text=\"地图工具\"", map);
    }

    [Fact]
    public void Region_tool_selected_states_keep_primary_foreground()
    {
        var map = Read("Right/MapPagePanel.axaml");
        Assert.Contains("ToggleButton.mapTool:checked:pointerover", map);
        Assert.Contains("ToggleButton.mapTool:pointerover TextBlock.mapToolLabel", map);
        Assert.Contains("ToggleButton.mapTool:checked TextBlock.mapToolLabel", map);
        Assert.Contains("ToggleButton.mapTool:checked:pointerover TextBlock.mapToolLabel", map);
        Assert.Contains("Color.Text.Primary", map);
        Assert.Contains("Color.Hover.Bg", map);
        Assert.Contains("Color.Selection.Bg", map);
        Assert.Contains("Color.Border.Strong", map);
        Assert.Contains("HorizontalContentAlignment=\"Center\"", map);
        Assert.Contains("VerticalContentAlignment=\"Center\"", map);
    }
}
