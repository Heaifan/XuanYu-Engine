using System.IO;
using Xunit;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class FeatureEditUiContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(
        System.AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Feature_edit_mode_shows_tools_and_breadcrumb()
    {
        var toolbar = Read("Top/ContextToolBar.axaml");
        
        Assert.Contains("地图编辑", toolbar);
        Assert.Contains("要素编辑", toolbar);
        Assert.Contains("XYMenuBarItem", toolbar);
        Assert.Contains("Label=\"点\"", toolbar);
        Assert.Contains("Label=\"线\"", toolbar);
        Assert.Contains("Label=\"面\"", toolbar);
        
        Assert.Contains("Label=\"点标记\"", toolbar);
        Assert.Contains("Label=\"道路\"", toolbar);
        Assert.Contains("Label=\"区域\"", toolbar);
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
