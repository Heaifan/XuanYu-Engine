using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2VectorPropertyLayoutStrategyTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;

    public XYUI2VectorPropertyLayoutStrategyTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Theory]
    [InlineData(280)]
    [InlineData(300)]
    public void Inline_vector3_keeps_axes_horizontal_at_compact_width(double width) => _fx.Run(() =>
    {
        var vector = Show(width, XYVectorDimension.Vector3, XYVectorPropertyLayout.Inline);
        Assert.Equal(3, vector.AxisPanelPart!.ColumnDefinitions.Count);
        Assert.Empty(vector.AxisPanelPart.RowDefinitions); AssertHorizontal(vector, 3);
    });

    [Theory]
    [InlineData(XYVectorDimension.Vector2, 2)]
    [InlineData(XYVectorDimension.Vector4, 4)]
    public void Inline_supports_each_dimension(XYVectorDimension dimension, int axes) => _fx.Run(() =>
    {
        var vector = Show(300, dimension, XYVectorPropertyLayout.Inline);
        Assert.Equal(axes, vector.AxisPanelPart!.ColumnDefinitions.Count); AssertHorizontal(vector, axes);
    });

    [Fact]
    public void Stacked_vector3_remains_vertical_on_a_wide_host() => _fx.Run(() =>
    {
        var vector = Show(620, XYVectorDimension.Vector3, XYVectorPropertyLayout.Stacked);
        Assert.Equal(3, vector.AxisPanelPart!.RowDefinitions.Count); Assert.Empty(vector.AxisPanelPart.ColumnDefinitions);
    });

    [Fact]
    public void Layout_changes_preserve_values_without_value_changed() => _fx.Run(() =>
    {
        var vector = Show(300, XYVectorDimension.Vector3, XYVectorPropertyLayout.Auto);
        var changes = 0; vector.ValueChanged += (_, _) => changes++;
        vector.Layout = XYVectorPropertyLayout.Inline; Dispatcher.UIThread.RunJobs();
        vector.Layout = XYVectorPropertyLayout.Stacked; Dispatcher.UIThread.RunJobs();
        Assert.Equal(0, changes); Assert.Equal((12.5, -3.25, .125), (vector.X, vector.Y, vector.Z));
    });

    static XYVectorProperty Show(double width, XYVectorDimension dimension, XYVectorPropertyLayout layout)
    {
        XyuiBatchTestHost.Prepare(); var vector = new XYVectorProperty { Width = width, Dimension = dimension, Layout = layout, X = 12.5, Y = -3.25, Z = .125, W = 4 };
        XyuiBatchTestHost.Show(vector); return vector;
    }

    static void AssertHorizontal(XYVectorProperty vector, int axes)
    {
        var hosts = vector.AxisHosts.Take(axes).ToArray(); Assert.All(hosts, host => Assert.True(host.Bounds.Width > 0));
        Assert.All(hosts, host => Assert.Equal(hosts[0].Bounds.Y, host.Bounds.Y));
        Assert.True(hosts.Zip(hosts.Skip(1)).All(pair => pair.First.Bounds.Right <= pair.Second.Bounds.X));
    }

    [Fact]
    public void Compact_vector_fields_consume_compact_size() => _fx.Run(() =>
    {
        var vector = Show(300, XYVectorDimension.Vector3, XYVectorPropertyLayout.Inline);
        Assert.All(vector.AxisFields, f => Assert.Equal(XYSize.Compact, XY.GetSize(f)));
    });

    [Fact]
    public void Compact_number_field_preserves_text_visibility() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYNumberField { Width = 71 }; // 71 DIP is the available width for Inline Vector3 at 300 DIP
        XY.SetSize(field, XYSize.Compact);
        XyuiBatchTestHost.Show(field);

        var presenter = field.GetVisualDescendants().OfType<TextBlock>().FirstOrDefault()
            ?? field.GetVisualDescendants().OfType<global::Avalonia.Controls.Presenters.TextPresenter>().FirstOrDefault() as Control;

        // ValueHost has small padding (4) and Stepper has small width (16). Suffix 0. (4+16=20). 71 - 20 = 51.
        // It must have enough width to show value (at least 20 DIP).
        Assert.True(field.ValueHost?.Bounds.Width >= 20);
    });
}
