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

    [Fact]
    public void SizeRole_Propagates_Through_Real_Toolbar_And_ToolGroup()
    {
        var icon = new XYIconButton();
        var tool = new XYButton { Content = "工具" };
        var group = new XYToolGroup(tool, icon);
        var toolbar = new XYToolbar(group);
        var root = new Panel { Children = { toolbar } };
        XyuiSizingScope.SetSizeRole(root, XyuiSizeRole.Comfortable);
        Assert.Equal(36d, toolbar.Height);
        Assert.Equal(36d, group.Height);
        Assert.Equal(36d, tool.Height);
        Assert.Equal(36d, icon.Height);
        Assert.Equal(36d, icon.Width);
    }

    [Fact]
    public void Select_Preserves_Legacy_Default_Until_Explicit_Default_Role()
    {
        var select = new XYSelect { ItemsSource = new[] { "一", "二" } };
        Assert.True(double.IsNaN(select.Height));
        XyuiSizingScope.SetSizeRole(select, XyuiSizeRole.Default);
        Assert.Equal(32d, select.Height);
    }

    [Fact]
    public void SizeRole_Geometry_Matches_The_Frozen_Contract()
    {
        var expected = new[] { (28d, 14d, 28d), (32d, 16d, 32d), (36d, 20d, 36d), (44d, 24d, 44d) };
        foreach (var (role, value) in Enum.GetValues<XyuiSizeRole>().Zip(expected))
        {
            var metrics = XyuiSizingScope.GetMetrics(role);
            Assert.Equal(value.Item1, metrics.ControlHeight);
            Assert.Equal(value.Item2, metrics.IconSize);
            Assert.Equal(value.Item3, metrics.MinimumHitTarget);
        }
    }
}
