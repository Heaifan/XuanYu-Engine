namespace XuanYu.World.Tests.UiTokens;

public sealed class UiD2F1RegionToolContractTests
{
    static string Read(string path) => File.ReadAllText(Path.Combine(TestSourceRoot.Root,  "XuanYu.Editor.UI", path));

    [Fact]
    public void Region_drawing_is_not_exposed_by_map_editor()
    {
        var map = Read("MapUI/Presentation/MapToolPanels/MapPagePanel.axaml");
        Assert.DoesNotContain("区域绘制", map);
        Assert.DoesNotContain("RegionDrawing", map);
        Assert.DoesNotContain("Draft", map);
    }

    [Fact]
    public void Region_drawing_is_not_exposed_by_map_navigation()
    {
        var editor = Read("MapUI/Presentation/EditorPanels/MapEditorPanel.axaml");
        Assert.DoesNotContain("RegionDrawing", editor);
        Assert.DoesNotContain("区域顶点", editor);
    }
}
