using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3Round3FixTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3Round3FixTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact] public void Pagination_CurrentPage_DoesNotChangeOverallWidth() => _fx.Run(() =>
    {
        var pagination = new XYPagination { TotalPages = 24, CurrentPage = 1 }; pagination.Measure(new Size(double.PositiveInfinity, 100)); var width = pagination.DesiredSize.Width; var panel = (Panel)pagination.Child!; var slots = panel.Children.Skip(2).Take(7).Select(x => x.Width).ToArray(); pagination.GoTo(12); pagination.Measure(new Size(double.PositiveInfinity, 100));
        var middleSlots = ((Panel)pagination.Child!).Children.Skip(2).Take(7).Select(x => x.Width).ToArray(); Assert.Equal(width, pagination.DesiredSize.Width); Assert.Equal(slots, middleSlots); Assert.All(slots, slotWidth => Assert.True(slotWidth >= 38));
    });

    [Fact] public void Pagination_LargePageNumbers_KeepStableButtonWidth() => _fx.Run(() =>
    {
        var pagination = new XYPagination { TotalPages = 1000, CurrentPage = 1 }; var slots = ((Panel)pagination.Child!).Children.Skip(2).Take(7).Select(x => x.Width).ToArray(); pagination.GoTo(500); var middleSlots = ((Panel)pagination.Child!).Children.Skip(2).Take(7).Select(x => x.Width).ToArray();
        Assert.Equal(slots, middleSlots); Assert.All(slots, width => Assert.True(width >= 60));
    });

    [Fact] public void ToolGroup_CollapsedTrigger_UsesLocalSelectedTool() => _fx.Run(() =>
    {
        var first = new XYToolbarTool { ToolId = "first", Icon = XyuiVectorIcon.Locate, IsSelected = true }; var second = new XYToolbarTool { ToolId = "second", Icon = XyuiVectorIcon.Code, IsSelected = true }; var group = new XYToolGroup(second) { IsCollapsed = true }; var other = new XYToolGroup(first) { IsCollapsed = true };
        Assert.Equal("second", group.ActiveToolId); Assert.Equal("first", other.ActiveToolId); Assert.Equal(XyuiVectorIcon.Code, ((XYIcon)group.CollapsedTrigger.Content!).Icon); Assert.Equal(XyuiVectorIcon.Locate, ((XYIcon)other.CollapsedTrigger.Content!).Icon);
    });

    [Fact] public void ToolGroup_InheritsToolbarCompactModeWithoutClipping() => _fx.Run(() =>
    {
        var tool = new XYToolbarTool { Label = "选择", Icon = XyuiVectorIcon.Locate }; var group = new XYToolGroup(tool); var toolbar = new XYToolbar(group);
        Assert.IsType<XYIcon>(tool.Button.Content); toolbar.IsCompact = false; var labeled = Assert.IsType<StackPanel>(tool.Button.Content); Assert.Contains(labeled.Children.OfType<TextBlock>(), x => x.Text == "选择"); Assert.NotNull(toolbar.Child);
    });

    [Fact] public void CommandBar_TextLabel_DoesNotLeakControlTypeName() => _fx.Run(() =>
    {
        var bar = new XYCommandBar(new XYCommandItem("新建", "new", XYCommandRole.Primary, XyuiVectorIcon.Add)); var item = bar.Items[0]; var content = Assert.IsType<StackPanel>(item.Content); var texts = content.Children.OfType<TextBlock>().Select(x => x.Text).ToArray();
        Assert.Contains("新建", texts); Assert.DoesNotContain(texts, x => x?.Contains("Avalonia.Controls.TextBlock", StringComparison.Ordinal) == true);
    });

    [Fact] public void CommandPalette_NoResults_AndDisabledCommand_AreBlocked() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var palette = new XYCommandPalette(new XYPaletteCommand("禁用") { IsEnabled = false }); var window = XyuiBatchTestHost.Show(palette); palette.SearchBox.Text = "不存在"; Assert.Empty(palette.FilteredCommands); Assert.Contains(palette.GetVisualDescendants().OfType<TextBlock>(), x => x.Text == "无匹配命令"); palette.SearchBox.Text = "禁用"; var executed = 0; palette.ExecuteRequested += (_, _) => executed++; palette.SearchBox.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter });
        Assert.Single(palette.FilteredCommands); Assert.False(palette.FilteredCommands[0].IsEnabled); Assert.Equal(0, executed); window.Close();
    });

    [Fact] public void TreeNode_WarningStatus_ReachesCanonicalBadge() => _fx.Run(() =>
    {
        var node = new XYTreeNode { Badge = "!", Status = XyuiStatusState.Warning }; var badge = node.GetVisualDescendants().OfType<XYStatusBadge>().Single(); Assert.Equal(XyuiStatusState.Warning, badge.State);
    });
}
