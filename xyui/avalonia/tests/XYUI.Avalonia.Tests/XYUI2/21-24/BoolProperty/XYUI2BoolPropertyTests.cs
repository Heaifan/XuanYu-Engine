using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2BoolPropertyTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2BoolPropertyTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Bool_property_reuses_switch_and_respects_readonly_disabled() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var property = new XYBoolProperty { Width = 420, Label = "显示网格", Value = false }; var window = XyuiBatchTestHost.Show(property);
        Assert.Equal(34, property.Bounds.Height); var sw = Assert.IsType<XYSwitch>(property.SwitchPart); Assert.Same(sw, property.GetVisualDescendants().OfType<XYSwitch>().Single());
        var changes = 0; property.ValueChanged += (_, _) => changes++; sw.IsChecked = true; Dispatcher.UIThread.RunJobs(); Assert.True(property.Value); Assert.Equal(1, changes);
        property.IsReadOnly = true; property.ToggleValue(); Assert.True(property.Value); Assert.False(sw.IsEnabled); property.IsEnabled = false; property.ToggleValue(); Assert.True(property.Value);
        var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-20"); Assert.Equal(2, preview.GetVisualDescendants().OfType<XYBoolProperty>().Count()); window.Close();
    });
}
