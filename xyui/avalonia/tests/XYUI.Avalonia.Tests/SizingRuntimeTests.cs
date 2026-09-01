using Avalonia.Controls;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Tests;

public sealed class SizingRuntimeTests
{
    [Fact]
    public void SizeRole_Uses_One_Inherited_Height_For_Input_Family()
    {
        var root = new Panel();
        var controls = new Control[] { new XYButton(), new XYTextField(), new XYSelect(), new XYNumberField() };
        foreach (var control in controls) root.Children.Add(control);
        XyuiSizingScope.SetSizeRole(root, XyuiSizeRole.Compact);
        Assert.All(controls, control => Assert.Equal(28d, control.Height));
        XyuiSizingScope.SetSizeRole(root, XyuiSizeRole.Comfortable);
        Assert.All(controls, control => Assert.Equal(36d, control.Height));
    }

    [Fact]
    public void SizeRole_Separates_Icon_Visual_Size_And_Hit_Target()
    {
        var button = new XYIconButton();
        XyuiSizingScope.SetSizeRole(button, XyuiSizeRole.Touch);
        Assert.Equal(44d, button.Width);
        Assert.Equal(24d, XyuiSizingScope.GetMetrics(XyuiSizeRole.Touch).IconSize);
        Assert.Equal(44d, button.MinWidth);
        Assert.Equal(44d, button.MinHeight);
    }

    [Fact]
    public void SizeRole_Default_Is_Content_Width_And_Local_Override_Wins()
    {
        var root = new Panel();
        var button = new XYButton();
        root.Children.Add(button);
        XyuiSizingScope.SetSizeRole(root, XyuiSizeRole.Compact);
        XyuiSizingScope.SetSizeRole(root, XyuiSizeRole.Default);
        Assert.Equal(32d, button.Height);
        Assert.True(double.IsNaN(button.Width));
        XyuiSizingScope.SetSizeRole(button, XyuiSizeRole.Touch);
        Assert.Equal(44d, button.Height);
    }
}
