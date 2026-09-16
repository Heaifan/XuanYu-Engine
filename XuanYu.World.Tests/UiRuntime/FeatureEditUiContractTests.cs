using System.IO;
using Xunit;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class FeatureEditUiContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(
        System.AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Context_toolbar_shows_draw_categories_without_breadcrumb_duplication()
    {
        var toolbar = Read("Top/ContextToolBar.axaml");
        var code = Read("Top/ContextToolBar.axaml.cs");
        
        Assert.Contains("IsVisible=\"{Binding IsEditMode}\"", toolbar);
        Assert.DoesNotContain("地图编辑", toolbar);
        Assert.DoesNotContain("要素编辑", toolbar);
        Assert.Contains("XYSplitButton", toolbar);
        Assert.Contains("Category(\"点\"", code);
        Assert.Contains("Category(\"线\"", code);
        Assert.Contains("Category(\"面\"", code);
        
        Assert.Contains("Category(\"点\"", code);
        Assert.Contains("Category(\"线\"", code);
        Assert.Contains("Category(\"面\"", code);
    }

    [Fact]
    public void Workspace_selector_semantics_updated()
    {
        var selector = Read("Workspace/WorkspaceSelector.axaml");
        
        Assert.Contains("要素编辑", selector);
        Assert.DoesNotContain("区域编辑", selector);
        Assert.DoesNotContain("点要素编辑", selector);
    }
}
