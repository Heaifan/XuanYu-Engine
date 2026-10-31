using Avalonia.Controls;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUICompositionReuseTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUICompositionReuseTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Composite_controls_expose_canonical_xyui_children() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var combo = new XYComboBox();
        var color = new XYColorPicker();
        var property = new XYBoolProperty();
        var window = XyuiBatchTestHost.Show(new StackPanel { Children = { combo, color, property } });

        Assert.IsType<XYTextField>(combo.TextFieldPart);
        Assert.IsType<XYTextField>(color.HexField);
        Assert.IsType<XYNumberField>(color.RedField);
        Assert.IsType<XYNumberField>(color.GreenField);
        Assert.IsType<XYNumberField>(color.BlueField);
        Assert.IsType<XYNumberField>(color.AlphaField);
        Assert.IsType<XYSlider>(color.HueSlider);
        Assert.IsType<XYSlider>(color.AlphaSlider);
        Assert.IsType<XYSwitch>(property.SwitchPart);
        window.Close();
    });

    [Fact]
    public void Canonical_input_and_xyui1_text_inheritance_is_preserved()
    {
        Assert.True(typeof(XYTextField).IsAssignableFrom(typeof(XYNumberField)));
        Assert.True(typeof(XyuiTextComponent).IsAssignableFrom(typeof(XYText)));
        Assert.True(typeof(XyuiTextSurface).IsAssignableFrom(typeof(XYSectionTitle)));
        Assert.True(typeof(XyuiTextSurface).IsAssignableFrom(typeof(XYIconLabel)));
    }
}
