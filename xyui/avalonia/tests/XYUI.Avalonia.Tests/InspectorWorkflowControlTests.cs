using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

public sealed class InspectorWorkflowControlTests
{
    [Fact]
    public void Pager_selects_first_page_and_navigates_without_wrap()
    {
        var pager = new XYPager(
            new XYPagerPage("basic", "基础", new TextBlock()),
            new XYPagerPage("environment", "环境", new TextBlock()),
            new XYPagerPage("advanced", "高级", new TextBlock()));

        Assert.Equal("basic", pager.SelectedId);
        Assert.False(pager.IsPreviousEnabled);
        Assert.True(pager.IsNextEnabled);
        pager.Next();
        Assert.Equal("environment", pager.SelectedId);
        pager.Next();
        pager.Next();
        Assert.Equal("advanced", pager.SelectedId);
        Assert.False(pager.IsNextEnabled);
    }

    [Fact]
    public void Inspector_section_hides_content_when_collapsed()
    {
        var section = new XYInspectorSection { Header = "环境设置", Content = new TextBlock() };
        Assert.True(section.IsExpanded);
        section.IsExpanded = false;
        Assert.False(section.Content!.IsVisible);
    }

    [Fact]
    public void Collapsible_pane_preserves_content_when_toggled()
    {
        var content = new TextBlock { Text = "图层" };
        var pane = new XYCollapsiblePane { Header = "图层", Content = content };
        pane.IsCollapsed = true;
        Assert.Same(content, pane.Content);
        Assert.False(content.IsVisible);
        pane.IsCollapsed = false;
        Assert.True(content.IsVisible);
    }
}
