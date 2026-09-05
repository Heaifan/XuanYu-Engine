using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2VectorPropertyLayoutTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2VectorPropertyLayoutTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Wide_uses_equal_star_columns_and_real_number_fields() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = Show(620, XYVectorDimension.Vector3);
        Assert.IsType<Grid>(p.AxisPanelPart); Assert.Equal(3, p.AxisPanelPart!.ColumnDefinitions.Count);
        Assert.All(p.AxisPanelPart.ColumnDefinitions, column => Assert.Equal(GridUnitType.Star, column.Width.GridUnitType));
        Assert.All(p.AxisFields, field => Assert.IsType<XYNumberField>(field)); AssertNoOverflow(p);
    });

    [Fact]
    public void Medium_stacks_label_and_uses_equal_star_columns() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = Show(420, XYVectorDimension.Vector3);
        Assert.Equal(2, p.RowPart!.RowDefinitions.Count); Assert.Equal(1, Grid.GetRow(p.AxisPanelPart!));
        Assert.Equal(3, p.AxisPanelPart!.ColumnDefinitions.Count); Assert.Equal(6, p.AxisPanelPart.Margin.Top); Assert.Equal(TextWrapping.Wrap, p.LabelPart!.TextWrapping); Assert.Equal(TextTrimming.None, p.LabelPart.TextTrimming); AssertNoOverflow(p);
    });

    [Fact]
    public void Compact_stacks_axes_and_vector4_switches_when_columns_are_too_narrow() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = Show(280, XYVectorDimension.Vector3);
        Assert.Equal(3, p.AxisPanelPart!.RowDefinitions.Count); Assert.Empty(p.AxisPanelPart.ColumnDefinitions); AssertNoOverflow(p);
        var four = Show(620, XYVectorDimension.Vector4); Assert.Equal(4, four.AxisPanelPart!.RowDefinitions.Count); AssertNoOverflow(four);
    });

    [Fact]
    public void Relayout_preserves_values_and_numberfield_behavior() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = Show(620, XYVectorDimension.Vector3); p.X = 12.5; p.Y = 8; p.Z = -4.8;
        p.Width = 280; Dispatcher.UIThread.RunJobs();
        Assert.Equal(12.5, p.X); Assert.Equal(8, p.Y); Assert.Equal(-4.8, p.Z); Assert.All(p.AxisFields, field => Assert.True(field.IsScrubEnabled));
    });

    [Fact]
    public void Vector4_reflows_wide_medium_compact_without_losing_label_or_values() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = Show(760, XYVectorDimension.Vector4); p.Label = "Position"; p.W = 4;
        Assert.All(p.AxisHosts, host => Assert.True(host.IsVisible)); Assert.Equal(4, p.AxisPanelPart!.ColumnDefinitions.Count);
        Resize(p, 420); Assert.Equal(2, p.RowPart!.RowDefinitions.Count); Assert.Equal(4, p.AxisPanelPart.RowDefinitions.Count);
        Resize(p, 280); Assert.Equal(4, p.AxisPanelPart.RowDefinitions.Count); Assert.Equal(4, p.AxisFields.Count);
        Assert.Equal("Position", p.LabelPart!.Text); Assert.Equal(4, p.W); Assert.All(p.AxisFields, field => Assert.IsType<XYNumberField>(field)); AssertNoOverflow(p);
    });

    static XYVectorProperty Show(double width, XYVectorDimension dimension)
    {
        var p = new XYVectorProperty { Width = width, Dimension = dimension, X = 12.5, Y = 0, Z = -4.8, W = 1 }; XyuiBatchTestHost.Show(p); return p;
    }

    static void Resize(XYVectorProperty property, double width)
    {
        property.Width = width; property.Measure(new Size(width, double.PositiveInfinity)); property.Arrange(new Rect(0, 0, width, property.DesiredSize.Height));
        property.UpdateLayoutMode();
    }

    static void AssertNoOverflow(XYVectorProperty property) => Assert.All(property.AxisHosts.Where(host => host.IsVisible), host => Assert.True(host.Bounds.Right <= property.Bounds.Width + 0.1));
}
