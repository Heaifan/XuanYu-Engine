using Avalonia;
using Avalonia.Controls;
using XYUI.Avalonia.Density;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Layout;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Tests;

public sealed class FoundationCoreTests
{
    [Fact]
    public void SemanticResources_Map_To_Existing_Foundation_Values()
    {
        var d = XyuiSpatial.CreateResources();
        Assert.Equal(XyuiSpatialTokens.Space1, d["XY.Gap.Base"]);
        Assert.Equal(new Thickness(XyuiSpatialTokens.PanelPadding), d["XY.Padding.Panel"]);
        Assert.Equal(XyuiSizeTokens.IconM, d["XY.Icon.Size.M"]);
        Assert.Equal(new Thickness(XyuiSpatialTokens.BorderWidthDefault), d["XY.Border.Width.Default"]);
    }

    [Fact]
    public void Density_Exposes_Distinct_Modes_And_Independent_Policies()
    {
        Assert.Equal(3, Enum.GetValues<XyuiDensityMode>().Length);
        Assert.Equal(3, Enum.GetValues<XyuiDensityPolicy>().Length);
        var d = XyuiDensity.CreateResources();
        Assert.Equal(XyuiSizeTokens.TreeRow, d["XY.Density.Compact.TreeRow"]);
        Assert.Equal(32d, d["XY.Density.Comfortable.TreeRow"]);
    }

    [Fact]
    public void Layout_Contract_Gives_Gap_And_Padding_Ownership()
    {
        var contract = XyuiLayoutContracts.For(XyuiLayoutRecipe.Inspector);
        Assert.True(contract.ComponentOwnsPadding);
        Assert.True(contract.ParentOwnsSiblingGap);
        Assert.False(contract.MarginIsSiblingLayoutTool);
    }

    [Fact]
    public void DensityScope_Inherits_And_Allows_Local_Override()
    {
        var parent = new Panel();
        var child = new Border();
        parent.Children.Add(child);
        XyuiDensityScope.SetMode(parent, XyuiDensityMode.Compact);

        Assert.Equal(XyuiDensityMode.Compact, XyuiDensityScope.GetMode(child));
        XyuiDensityScope.SetMode(child, XyuiDensityMode.Comfortable);
        Assert.Equal(XyuiDensityMode.Comfortable, XyuiDensityScope.GetMode(child));
    }

    [Fact]
    public void DensityScope_Maps_Consumable_Metrics_And_Rejects_Touch()
    {
        Assert.True(XyuiDensity.TryGetMetrics(XyuiDensityMode.Compact, out var compact));
        Assert.Equal(XyuiSizeTokens.ControlS, compact.ControlSize);
        Assert.Equal(XyuiSpatialTokens.FieldRowGap, compact.Gap);
        Assert.True(XyuiDensity.TryGetMetrics(XyuiDensityMode.Comfortable, out var comfortable));
        Assert.NotEqual(compact.ControlSize, comfortable.ControlSize);
        Assert.False(XyuiDensity.TryGetMetrics(XyuiDensityMode.Touch, out _));
    }

    [Fact]
    public void DensityScope_Separates_Mode_From_Policy()
    {
        var control = new Border();
        XyuiDensityScope.SetMode(control, XyuiDensityMode.Compact);
        XyuiDensityScope.SetPolicy(control, XyuiDensityPolicy.ManualLock);
        Assert.Equal(XyuiDensityMode.Compact, XyuiDensityScope.GetMode(control));
        Assert.Equal(XyuiDensityPolicy.ManualLock, XyuiDensityScope.GetPolicy(control));
    }
}
