namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD2F1RegionToolContractTests
{
    static string Read(string path) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", path));

    [Fact]
    public void Region_tool_is_owned_by_map_editor_not_top_navigation()
    {
        var page = Read("Right/MapPagePanel.axaml");
        var top = Read("Top/Top.axaml");
        Assert.Contains("Classes=\"mapTool uiTextButton\"", page);
        Assert.Contains("CommandParameter=\"区域绘制\"", page);
        Assert.DoesNotContain("CommandParameter=\"区域绘制\"", top);
    }

    [Fact]
    public void Region_tool_selected_states_keep_primary_foreground()
    {
        var page = Read("Right/MapPagePanel.axaml");
        Assert.Contains("ToggleButton.mapTool:checked", page);
        Assert.Contains("ToggleButton.mapTool:checked:pointerover", page);
        Assert.Contains("Color.Text.Primary", page);
    }
}
