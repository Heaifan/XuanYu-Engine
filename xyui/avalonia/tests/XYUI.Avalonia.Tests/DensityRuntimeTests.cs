using Avalonia.Controls;
using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Tests;

public sealed class DensityRuntimeTests
{
    [Fact]
    public void Default_density_is_default_and_uses_existing_spacing()
    {
        var panel = new StackPanel();
        Assert.Equal(XyuiDensity.Default, XyuiDensityScope.GetDensity(panel));
        Assert.Equal(XyuiDensityMetrics.For(XyuiDensity.Default), XyuiDensityScope.GetMetrics(panel));
    }

    [Theory]
    [InlineData(XyuiDensity.Compact)]
    [InlineData(XyuiDensity.Default)]
    [InlineData(XyuiDensity.Comfortable)]
    public void Density_has_only_the_three_contract_values(XyuiDensity density)
    {
        var panel = new StackPanel();
        XyuiDensityScope.SetDensity(panel, density);
        Assert.Equal(density, XyuiDensityScope.GetDensity(panel));
        Assert.Equal(XyuiDensityMetrics.For(density), XyuiDensityScope.GetMetrics(panel));
    }

    [Fact]
    public void Parent_scope_inherits_through_nested_panels()
    {
        var root = new StackPanel();
        var nested = new StackPanel();
        var child = new Border();
        root.Children.Add(nested); nested.Children.Add(child);
        XyuiDensityScope.SetDensity(root, XyuiDensity.Compact);
        Assert.Equal(XyuiDensity.Compact, XyuiDensityScope.GetDensity(nested));
        Assert.Equal(XyuiDensity.Compact, XyuiDensityScope.GetDensity(child));
    }

    [Fact]
    public void Child_override_does_not_change_sibling()
    {
        var root = new StackPanel();
        var overridden = new Border(); var sibling = new Border();
        root.Children.Add(overridden); root.Children.Add(sibling);
        XyuiDensityScope.SetDensity(root, XyuiDensity.Compact);
        XyuiDensityScope.SetDensity(overridden, XyuiDensity.Comfortable);
        Assert.Equal(XyuiDensity.Comfortable, XyuiDensityScope.GetDensity(overridden));
        Assert.Equal(XyuiDensity.Compact, XyuiDensityScope.GetDensity(sibling));
    }

    [Fact]
    public void Density_metrics_are_spacing_compositions_only()
    {
        var compact = XyuiDensityMetrics.For(XyuiDensity.Compact);
        Assert.Equal(4, compact.RowGap);
        Assert.Equal(8, compact.SectionGap);
        Assert.Equal(8, compact.PanelPadding);
        Assert.DoesNotContain("Touch", Enum.GetNames<XyuiDensity>());
    }

    [Fact]
    public void Density_is_orthogonal_to_control_size_typography_and_hit_target()
    {
        var button = new Button { Height = 32, Width = 160, FontSize = 14, MinHeight = 44 };
        var font = button.FontSize; var height = button.Height; var width = button.Width; var hit = button.MinHeight;
        XyuiDensityScope.SetDensity(button, XyuiDensity.Compact);
        Assert.Equal(font, button.FontSize); Assert.Equal(height, button.Height);
        Assert.Equal(width, button.Width); Assert.Equal(hit, button.MinHeight);
    }
}
