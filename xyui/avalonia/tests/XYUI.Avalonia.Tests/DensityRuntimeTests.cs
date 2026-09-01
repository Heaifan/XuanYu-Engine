using Avalonia.Controls;
using XYUI.Avalonia;
using XYUI.Avalonia.Density;

namespace XYUI.Avalonia.Tests;

public sealed class DensityRuntimeTests
{
    [Fact]
    public void Default_density_is_default_and_uses_existing_spacing()
    {
        var panel = new StackPanel();
        Assert.Equal(XYDensity.Default, XY.GetDensity(panel));
        Assert.Equal(XyuiDensityMetrics.For(XYDensity.Default), XyuiDensityMetrics.For(XY.GetDensity(panel)));
    }

    [Theory]
    [InlineData(XYDensity.Compact)]
    [InlineData(XYDensity.Default)]
    [InlineData(XYDensity.Comfortable)]
    public void Density_has_only_the_three_contract_values(XYDensity density)
    {
        var panel = new StackPanel();
        XY.SetDensity(panel, density);
        Assert.Equal(density, XY.GetDensity(panel));
        Assert.Equal(XyuiDensityMetrics.For(density), XyuiDensityMetrics.For(XY.GetDensity(panel)));
    }

    [Fact]
    public void Parent_scope_inherits_through_nested_panels()
    {
        var root = new StackPanel();
        var nested = new StackPanel();
        var child = new Border();
        root.Children.Add(nested); nested.Children.Add(child);
        XY.SetDensity(root, XYDensity.Compact);
        Assert.Equal(XYDensity.Compact, XY.GetDensity(nested));
        Assert.Equal(XYDensity.Compact, XY.GetDensity(child));
    }

    [Fact]
    public void Child_override_does_not_change_sibling()
    {
        var root = new StackPanel();
        var overridden = new Border(); var sibling = new Border();
        root.Children.Add(overridden); root.Children.Add(sibling);
        XY.SetDensity(root, XYDensity.Compact);
        XY.SetDensity(overridden, XYDensity.Comfortable);
        Assert.Equal(XYDensity.Comfortable, XY.GetDensity(overridden));
        Assert.Equal(XYDensity.Compact, XY.GetDensity(sibling));
    }

    [Fact]
    public void Density_metrics_are_spacing_compositions_only()
    {
        var compact = XyuiDensityMetrics.For(XYDensity.Compact);
        Assert.Equal(4, compact.RowGap);
        Assert.Equal(8, compact.SectionGap);
        Assert.Equal(8, compact.PanelPadding);
        Assert.DoesNotContain("Touch", Enum.GetNames<XYDensity>());
    }

    [Fact]
    public void Density_is_orthogonal_to_control_size_typography_and_hit_target()
    {
        var button = new Button { Height = 32, Width = 160, FontSize = 14, MinHeight = 44 };
        var font = button.FontSize; var height = button.Height; var width = button.Width; var hit = button.MinHeight;
        XY.SetDensity(button, XYDensity.Compact);
        Assert.Equal(font, button.FontSize); Assert.Equal(height, button.Height);
        Assert.Equal(width, button.Width); Assert.Equal(hit, button.MinHeight);
    }
}
