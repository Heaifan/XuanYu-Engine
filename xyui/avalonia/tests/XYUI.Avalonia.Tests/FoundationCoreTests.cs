using Avalonia;
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
}
