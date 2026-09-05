using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2Phase2DContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2Phase2DContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Controls_strictly_reuse_canonical_xyui_controls() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var num = new XYNumberProperty(); var vec = new XYVectorProperty { Dimension = XYVectorDimension.Vector3 };
        var enu = new XYEnumProperty { ItemsSource = new[] { "A", "B" } }; var boo = new XYBoolProperty();
        var refer = new XYReferenceProperty();
        var host = new StackPanel { Children = { num, vec, enu, boo, refer } };
        var window = XyuiBatchTestHost.Show(host);

        Assert.IsType<XYNumberField>(num.ValueFieldPart);
        Assert.All(vec.AxisFields, axis => Assert.IsType<XYNumberField>(axis));
        Assert.IsType<XYSelect>(enu.SelectPart);
        Assert.IsType<XYSwitch>(boo.SwitchPart);
        Assert.All(new[] { refer.LocatePart!, refer.BrowsePart!, refer.ClearPart! }, b => Assert.IsType<XYIconButton>(b));
        window.Close();
    });

    [Fact]
    public void Phase2D_popup_controls_safely_close_on_lifecycle_transitions() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var colorPicker = new XYColorPicker { Width = 300 };
        var refProperty = new XYReferenceProperty { Width = 400, ReferencePickerContent = new TextBlock { Text = "Picker" } };
        var host = new StackPanel { Children = { colorPicker, refProperty } };
        var window = XyuiBatchTestHost.Show(host);

        colorPicker.IsOpen = true;
        Dispatcher.UIThread.RunJobs();
        Assert.True(colorPicker.PopupPart?.IsOpen);

        refProperty.BrowsePart?.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        Dispatcher.UIThread.RunJobs();
        Assert.True(refProperty.IsPickerOpen);

        // Document switch / visual detachment lifecycle
        host.Children.Clear();
        Dispatcher.UIThread.RunJobs();

        Assert.False(colorPicker.IsOpen);
        Assert.False(refProperty.IsPickerOpen);
        window.Close();
    });

    [Fact]
    public void Vector_and_reference_property_visual_contracts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var vec = new XYVectorProperty { Width = 620, Dimension = XYVectorDimension.Vector3 };
        var refer = new XYReferenceProperty { Width = 400 };
        var host = new StackPanel { Children = { vec, refer } };
        var window = XyuiBatchTestHost.Show(host);

        Assert.Contains("xyui-vector-wide", vec.Classes);
        vec.Width = 420; vec.Measure(new Size(420, double.PositiveInfinity)); vec.Arrange(new Rect(0, 0, 420, vec.DesiredSize.Height)); vec.UpdateLayoutMode(); Assert.Contains("xyui-vector-medium", vec.Classes);
        vec.Width = 280; vec.Measure(new Size(280, double.PositiveInfinity)); vec.Arrange(new Rect(0, 0, 280, vec.DesiredSize.Height)); vec.UpdateLayoutMode(); Assert.Contains("xyui-vector-compact", vec.Classes);

        refer.ReferenceState = XYReferenceState.Missing; Dispatcher.UIThread.RunJobs();
        var missingColor = XyuiBatchTestHost.ColorOf(refer.ReferenceFieldPart!.BorderBrush);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Semantic.Error.Border"), missingColor);

        refer.ReferenceState = XYReferenceState.TypeMismatch; Dispatcher.UIThread.RunJobs();
        var mismatchColor = XyuiBatchTestHost.ColorOf(refer.ReferenceFieldPart.BorderBrush);
        Assert.Equal(XyuiBatchTestHost.Token("XY.Semantic.Warning.Border"), mismatchColor);
        Assert.NotEqual(missingColor, mismatchColor);
        window.Close();
    });
}
