using Avalonia.Controls;
using XYUI.Avalonia;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class ConsumerApiTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public ConsumerApiTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void IconButton_Icon_property_builds_canonical_xyicon() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var button = new XYIconButton { Icon = XyuiVectorIcon.Search };
        Assert.Equal(XyuiVectorIcon.Search, Assert.IsType<XYIcon>(button.Content).Icon);
    });

    [Fact]
    public void Button_Icon_and_content_build_one_consumer_composition() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var button = new XYButton { Icon = XyuiVectorIcon.Search, Content = "搜索" };
        var panel = Assert.IsType<StackPanel>(button.Content);
        Assert.IsType<XYIcon>(panel.Children[0]);
        Assert.Equal("搜索", Assert.IsType<TextBlock>(panel.Children[1]).Text);
    });

    [Fact]
    public void Foundation_scopes_are_independent_and_inherited() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var parent = new Border(); var child = new XYIconButton(); var sibling = new XYIconButton();
        parent.Child = new StackPanel { Children = { child, sibling } }; XY.SetSize(parent, XYSize.Compact); XY.SetDensity(parent, XYDensity.Comfortable);
        XyuiBatchTestHost.Show(parent);
        Assert.Equal(XYSize.Compact, XY.GetSize(child)); Assert.Equal(XYSize.Compact, XY.GetSize(sibling));
        Assert.Equal(XYDensity.Comfortable, XY.GetDensity(child)); Assert.Equal(XYDensity.Comfortable, XY.GetDensity(sibling));
        XY.SetSize(child, XYSize.Touch); Assert.Equal(XYSize.Touch, XY.GetSize(child)); Assert.Equal(XYSize.Compact, XY.GetSize(sibling));
    });
}
