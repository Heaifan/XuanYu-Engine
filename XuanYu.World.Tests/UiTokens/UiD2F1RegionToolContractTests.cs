namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD2F1RegionToolContractTests
{
    static string Read(string path) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", path));

    [Fact]
    public void Region_tool_is_owned_by_map_editor_not_top_navigation()
    {
        var top = Read("Top/Top.axaml");
        Assert.Contains("Classes=\"toolBtn\"", top);
        Assert.Contains("CommandParameter=\"区域绘制\"", top);
    }

    [Fact]
    public void Region_tool_selected_states_keep_primary_foreground()
    {
        var top = Read("Top/Top.axaml");
        Assert.Contains("ToggleButton.toolBtn:checked:pointerover", Read("Top/Top.States.axaml"));
        Assert.Contains("Color.Text.Primary", Read("Ui.axaml"));
        Assert.Contains("CommandParameter=\"区域绘制\"", top);
    }
}
