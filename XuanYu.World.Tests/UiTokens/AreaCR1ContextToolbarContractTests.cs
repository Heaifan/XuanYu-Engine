using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class AreaCR1ContextToolbarContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Context_toolbar_owns_feature_tools_and_drawing_actions()
    {
        var top = Read("Top/Top.axaml");
        var toolbar = Read("Top/ContextToolBar.axaml");
        var inspector = Read("Right/InspectorPanel.axaml");

        Assert.Contains("<local:ContextToolBar", top);
        Assert.Contains("SelectRegionAuthoringModeCommand", toolbar);
        Assert.Contains("BeginRegionDrawing_Click", toolbar);
        Assert.Contains("UndoRegionDrawingVertex", toolbar);
        Assert.Contains("CompleteRegionDrawing", toolbar);
        Assert.Contains("CancelRegionDrawing", toolbar);
        Assert.Contains("BeginRoadDrawing_Click", Read("Top/ContextToolBar.axaml.cs"));
        Assert.Contains("BeginMarkerPlacement_Click", Read("Top/ContextToolBar.axaml.cs"));
        Assert.DoesNotContain("RegionalAuthoringPanel", inspector);
        Assert.DoesNotContain("RegionPanel", inspector);
        Assert.DoesNotContain("RoadPanel", inspector);
        Assert.DoesNotContain("MarkerPanel", inspector);
    }

    [Fact]
    public void Inspector_keeps_property_sections_without_total_scroll_host()
    {
        var inspector = Read("Right/InspectorPanel.axaml");
        Assert.DoesNotContain("InspectorScrollViewer", inspector);
        Assert.DoesNotContain("<ScrollViewer", inspector);
        Assert.Contains("MapEditorPanel", inspector);
        Assert.DoesNotContain("开始绘制", inspector);
        Assert.DoesNotContain("撤销顶点", inspector);
    }

    [Fact]
    public void Context_toolbar_is_present_only_in_region_edit_context()
    {
        var toolbar = Read("Top/ContextToolBar.axaml");
        Assert.Contains("<Border x:Name=\"ContextRoot\" IsVisible=\"{Binding IsFeatureEditingActive}\"", toolbar);
    }

    [Fact]
    public void Top_context_row_keeps_hidden_horizontal_scroll_host()
    {
        var top = Read("Top/Top.axaml");
        var code = Read("Top/Top.axaml.cs");
        Assert.Contains("<ScrollViewer x:Name=\"ContextToolScrollHost\"", top);
        Assert.Contains("x:Name=\"ContextToolScrollHost\"", top);
        Assert.Contains("HorizontalScrollBarVisibility=\"Hidden\"", top);
        Assert.DoesNotContain("<WrapPanel", top);
        Assert.Contains("PointerWheelChangedEvent", code);
        Assert.Contains("Offset.X", code);
    }
}
