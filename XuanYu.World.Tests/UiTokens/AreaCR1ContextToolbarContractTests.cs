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
        var code = Read("Top/ContextToolBar.axaml.cs");
        var inspector = Read("Right/InspectorPanel.axaml");

        Assert.Contains("<local:ContextToolBar", top);
        Assert.Contains("DrawButtonLabel", toolbar);
        Assert.Contains("BeginContextDrawingAsync", code);
        Assert.Contains("UndoDrawingVertex_Click", code);
        Assert.Contains("CompleteDrawing_Click", code);
        Assert.Contains("CancelDrawing_Click", code);
        Assert.DoesNotContain("RegionalAuthoringPanel", inspector);
        Assert.DoesNotContain("RegionPanel", inspector);
        Assert.DoesNotContain("RoadPanel", inspector);
        Assert.DoesNotContain("MarkerPanel", inspector);
    }

    [Fact]
    public void Context_toolbar_r2_uses_split_draw_entry_and_unified_transaction_labels()
    {
        var toolbar = Read("Top/ContextToolBar.axaml");
        var code = Read("Top/ContextToolBar.axaml.cs");
        Assert.Contains("XYSplitButton", toolbar);
        Assert.Contains("DrawButtonLabel", toolbar);
        Assert.Contains("XYSubMenu", code);
        Assert.DoesNotContain("DrawSubMenuPopup", code);
        Assert.Contains("道路", code);
        Assert.Contains("区域", code);
        Assert.Contains("Content=\"完成\"", toolbar);
        Assert.Contains("CanCompleteDrawing", toolbar);
        Assert.DoesNotContain("Content=\"完成道路\"", toolbar);
        Assert.DoesNotContain("Content=\"完成闭合\"", toolbar);
        Assert.DoesNotContain("Content=\"绘制道路\"", toolbar);
        Assert.DoesNotContain("Content=\"开始绘制\"", toolbar);
    }

    [Fact]
    public void Inspector_keeps_fixed_navigation_and_one_content_scroll_host()
    {
        var inspector = Read("Right/InspectorPanel.axaml");
        Assert.Contains("SearchBox", inspector);
        Assert.Contains("XYNavigationRail", inspector);
        Assert.Equal(1, inspector.Split("<ScrollViewer", StringSplitOptions.None).Length - 1);
        Assert.Contains("InspectorPropertyRow", inspector);
        Assert.DoesNotContain("开始绘制", inspector);
        Assert.DoesNotContain("撤销顶点", inspector);
    }

    [Fact]
    public void Context_toolbar_is_present_in_editor_mode_without_duplicate_breadcrumb()
    {
        var toolbar = Read("Top/ContextToolBar.axaml");
        Assert.Contains("<Border x:Name=\"ContextRoot\" IsVisible=\"{Binding IsEditMode}\"", toolbar);
        Assert.DoesNotContain("地图编辑", toolbar);
        Assert.DoesNotContain("要素编辑", toolbar);
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
