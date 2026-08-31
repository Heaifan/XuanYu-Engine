using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

public class ControlSurfaceTests
{
    [Fact]
    public void Button_Uses_Canonical_Variant_And_Class()
    {
        var button = new XYButton { Variant = XyuiButtonVariant.Danger };

        Assert.Equal(XyuiButtonVariant.Danger, button.Variant);
        Assert.Contains("xyui-button", button.Classes);
    }

    [Fact]
    public void Existing_Control_Classes_Are_Stable()
    {
        Assert.Contains("xyui-icon-button", new XYIconButton().Classes);
        Assert.Contains("xyui-toggle-button", new XYToggleButton().Classes);
        Assert.Contains("xyui-checkbox", new XYCheckbox().Classes);
        Assert.Contains("xyui-radio-button", new XYRadioButton().Classes);
        Assert.Contains("xyui-switch", new XYSwitch().Classes);
        Assert.Contains("xyui-text-field", new XYTextField().Classes);
    }
}
