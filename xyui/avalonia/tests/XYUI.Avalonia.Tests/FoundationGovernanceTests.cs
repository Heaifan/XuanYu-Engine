using Avalonia.Controls;
using XYUI.Avalonia.Density;
using XYUI.Avalonia.Foundation;
using XYUI.Avalonia.Governance;
using XYUI.Avalonia.Layout;
using XYUI.Avalonia.Spatial;

namespace XYUI.Avalonia.Tests;

public sealed class FoundationGovernanceTests
{
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
    public void DensityMapping_Rejects_Touch_And_Separates_Policy()
    {
        Assert.True(XyuiDensity.TryGetMetrics(XyuiDensityMode.Compact, out var compact));
        Assert.Equal(XyuiSizeTokens.ControlS, compact.ControlSize);
        Assert.True(XyuiDensity.TryGetMetrics(XyuiDensityMode.Comfortable, out var comfortable));
        Assert.NotEqual(compact.ControlSize, comfortable.ControlSize);
        Assert.False(XyuiDensity.TryGetMetrics(XyuiDensityMode.Touch, out _));
        var control = new Border();
        XyuiDensityScope.SetPolicy(control, XyuiDensityPolicy.ManualLock);
        Assert.Equal(XyuiDensityPolicy.ManualLock, XyuiDensityScope.GetPolicy(control));
    }

    [Fact]
    public void MetricGate_Classifies_Values_And_Protects_Exceptions()
    {
        var findings = XyuiMetricGate.Analyze("<Border Padding=\"{DynamicResource XY.Padding.Panel}\" Height=\"7\" />");
        Assert.Equal(XyuiMetricClassification.Tokenized, findings[0].Classification);
        Assert.Equal(XyuiMetricClassification.UnjustifiedMagicNumber, findings[1].Classification);
        var geometry = XyuiMetricGate.Analyze("Height=7", "Editor/Geometry/Path.cs");
        Assert.Equal(XyuiMetricClassification.AllowedException, geometry[0].Classification);
    }

    [Fact]
    public void CompositionMetrics_Are_Available_For_All_Recipes()
    {
        foreach (var recipe in Enum.GetValues<XyuiLayoutRecipe>())
        {
            Assert.True(XyuiLayoutContracts.TryMetrics(recipe, XyuiDensityMode.Compact, out var metrics));
            Assert.Equal(recipe, metrics.Recipe);
            Assert.Equal(XyuiSpatialTokens.FieldRowGap, metrics.Gap);
        }
        Assert.False(XyuiLayoutContracts.TryMetrics(XyuiLayoutRecipe.Toolbar, XyuiDensityMode.Touch, out _));
    }
}
