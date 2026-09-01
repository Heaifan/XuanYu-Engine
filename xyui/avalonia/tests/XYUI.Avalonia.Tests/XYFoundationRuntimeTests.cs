using Avalonia.Controls;
using XYUI.Avalonia;
using XYUI.Avalonia.Sizing;

namespace XYUI.Avalonia.Tests;

public sealed class XYFoundationRuntimeTests
{
    [Fact]
    public void Size_and_density_have_the_canonical_defaults()
    {
        var panel = new StackPanel();
        Assert.Equal(XYSize.Default, XY.GetSize(panel));
        Assert.Equal(XYDensity.Default, XY.GetDensity(panel));
        Assert.Equal(new XyuiSizingMetrics(32, 16), XyuiSizingMetrics.For(XYSize.Default));
    }

    [Fact]
    public void Size_and_density_inherit_through_nested_panels()
    {
        var root = new StackPanel(); var nested = new StackPanel(); var child = new Border();
        root.Children.Add(nested); nested.Children.Add(child);
        XY.SetSize(root, XYSize.Default); XY.SetDensity(root, XYDensity.Compact);
        Assert.Equal(XYSize.Default, XY.GetSize(child));
        Assert.Equal(XYDensity.Compact, XY.GetDensity(child));
    }

    [Fact]
    public void Child_override_is_local_and_sibling_is_unchanged()
    {
        var root = new StackPanel(); var child = new Border(); var sibling = new Border();
        root.Children.Add(child); root.Children.Add(sibling);
        XY.SetSize(root, XYSize.Compact); XY.SetDensity(root, XYDensity.Compact);
        XY.SetSize(child, XYSize.Touch); XY.SetDensity(child, XYDensity.Comfortable);
        Assert.Equal(XYSize.Touch, XY.GetSize(child)); Assert.Equal(XYDensity.Comfortable, XY.GetDensity(child));
        Assert.Equal(XYSize.Compact, XY.GetSize(sibling)); Assert.Equal(XYDensity.Compact, XY.GetDensity(sibling));
    }

    [Fact]
    public void Size_and_density_are_orthogonal()
    {
        var panel = new StackPanel(); XY.SetSize(panel, XYSize.Default); XY.SetDensity(panel, XYDensity.Compact);
        Assert.Equal(new XyuiSizingMetrics(32, 16), XyuiSizingMetrics.For(XY.GetSize(panel)));
        Assert.Equal(XYDensity.Compact, XY.GetDensity(panel));
        Assert.Equal(new XyuiSizingMetrics(28, 14), XyuiSizingMetrics.For(XYSize.Compact));
        Assert.Equal(new XyuiSizingMetrics(36, 20), XyuiSizingMetrics.For(XYSize.Comfortable));
        Assert.Equal(new XyuiSizingMetrics(44, 24), XyuiSizingMetrics.For(XYSize.Touch));
    }
}
