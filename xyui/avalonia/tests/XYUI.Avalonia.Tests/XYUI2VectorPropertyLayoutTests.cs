using Avalonia.Controls;
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
        Assert.Equal(3, p.AxisPanelPart!.ColumnDefinitions.Count); AssertNoOverflow(p);
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

    static XYVectorProperty Show(double width, XYVectorDimension dimension)
    {
        var p = new XYVectorProperty { Width = width, Dimension = dimension, X = 12.5, Y = 0, Z = -4.8, W = 1 }; XyuiBatchTestHost.Show(p); return p;
    }

    static void AssertNoOverflow(XYVectorProperty property) => Assert.All(property.AxisHosts.Where(host => host.IsVisible), host => Assert.True(host.Bounds.Right <= property.Bounds.Width + 0.1));
}
