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
        var approvedSemantic = XyuiMetricGate.Analyze("Spacing=2 // XY.Gap.ToolItem");
        Assert.Equal(XyuiMetricClassification.Tokenized, approvedSemantic[0].Classification);
        var businessOverride = XyuiMetricGate.Analyze("<XYToolGroup Spacing=\"7\" />");
        Assert.Equal(XyuiMetricClassification.UnjustifiedMagicNumber, businessOverride[0].Classification);
    }

    [Fact]
    public void DensitySemanticMapping_Changes_And_Opens_Touch_SpacingOnly()
    {
        var compact = XyuiDensity.CreateResolvedSemanticResources(XyuiDensityMode.Compact);
        var comfortable = XyuiDensity.CreateResolvedSemanticResources(XyuiDensityMode.Comfortable);
        var touch = XyuiDensity.CreateResolvedSemanticResources(XyuiDensityMode.Touch);
        Assert.Equal(2d, compact["XY.Gap.ToolItem"]);
        Assert.Equal(4d, comfortable["XY.Gap.ToolItem"]);
        Assert.Equal(6d, touch["XY.Gap.ToolItem"]);
        Assert.Equal(20d, touch["XY.Padding.Panel"]);
        Assert.False(XyuiDensity.TryGetMetrics(XyuiDensityMode.Touch, out _));
    }

    [Fact]
    public void SemanticContainers_Consume_Density_Gaps_And_Padding()
    {
        var root = new Panel();
        var tools = new XYToolGroup();
        var toolbar = new XYToolbar();
        root.Children.Add(tools);
        root.Children.Add(toolbar);
        XyuiDensityScope.SetMode(root, XyuiDensityMode.Compact);
        Assert.Equal(global::Avalonia.Layout.Orientation.Horizontal, tools.Orientation);
        Assert.Equal(2d, tools.Spacing);
        Assert.Equal(6d, toolbar.Spacing);

        var fields = new XYFieldGroup();
        var sections = new XYSectionGroup();
        var panel = new XYPanel();
        XyuiDensityScope.SetMode(fields, XyuiDensityMode.Touch);
        XyuiDensityScope.SetMode(sections, XyuiDensityMode.Touch);
        XyuiDensityScope.SetMode(panel, XyuiDensityMode.Touch);
        Assert.Equal(12d, fields.Spacing);
        Assert.Equal(20d, sections.Spacing);
        Assert.Equal(20d, panel.Padding.Top);
    }

    [Fact]
    public void CompositionMetrics_Are_Available_For_All_Recipes()
    {
        foreach (var recipe in Enum.GetValues<XyuiLayoutRecipe>())
        {
            Assert.True(XyuiLayoutContracts.TryMetrics(recipe, XyuiDensityMode.Compact, out var metrics));
            Assert.Equal(recipe, metrics.Recipe);
            Assert.Equal(6d, metrics.Gap);
        }
        Assert.False(XyuiLayoutContracts.TryMetrics(XyuiLayoutRecipe.Toolbar, XyuiDensityMode.Touch, out _));
    }
}
