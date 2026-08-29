using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2ColorPickerTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2ColorPickerTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Color_picker_has_real_panel_hex_validation_and_alpha() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var picker = new XYColorPicker { Width = 300, Color = Color.FromArgb(140, 50, 111, 138) }; var window = XyuiBatchTestHost.Show(picker);
        Assert.Equal(32, picker.Bounds.Height); Assert.Equal("#326F8A · 55%", picker.ValuePart!.Text); Assert.Equal(28, picker.SwatchPart!.Bounds.Width); Assert.Equal(20, picker.SwatchPart.Bounds.Height);
        var changes = 0; picker.ColorChanged += (_, _) => changes++; picker.IsOpen = true; Dispatcher.UIThread.RunJobs();
        Assert.True(picker.PopupPart!.IsOpen); Assert.NotNull(picker.ColorArea); Assert.NotNull(picker.HueSlider); Assert.NotNull(picker.AlphaSlider);
        picker.AlphaSlider!.Value = 255; Dispatcher.UIThread.RunJobs(); Assert.Equal(255, picker.Color.A); Assert.Equal("#326F8A · 100%", picker.ValuePart.Text); Assert.True(changes > 0);
        picker.HexField!.Text = "#FF000080"; picker.OnHexCommitted(); Dispatcher.UIThread.RunJobs(); Assert.Equal(Color.FromArgb(128, 255, 0, 0), picker.Color);
        picker.HexField.Text = "不是颜色"; picker.OnHexCommitted(); Assert.Equal(Color.FromArgb(128, 255, 0, 0), picker.Color); Assert.True(picker.ErrorPart!.IsVisible);
        Assert.IsType<StackPanel>(XYUI2GalleryCatalog.CreatePreview("XYUI-2-19")); window.Close();
    });
}
