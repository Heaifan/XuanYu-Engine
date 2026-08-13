using Avalonia.Controls;
using Xunit;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

public sealed class ControlsContractTests
{
    [Fact]
    public void CoreControlsExposeXYUIStyleClasses()
    {
        var controls = new Control[]
        {
            new XYButton(), new XYIconButton(), new XYToggleButton(),
            new XYTextField(), new XYNumberField(), new XYCheckBox(),
            new XYRadioButton(), new XYToggleSwitch(), new XYComboBox(),
            new XYSlider(), new XYBadge(), new XYTag()
        };

        Assert.All(controls, control =>
            Assert.Contains(control.Classes, name => name.StartsWith("xy-")));
    }
}
