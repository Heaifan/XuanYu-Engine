using Avalonia.Controls;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticProbeResolverLocatorTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticProbeResolverLocatorTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Named_locator_is_rooted_at_nearest_debug_id()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var root = new Border { Child = new StackPanel() }; var target = new TextBox { Name = "RoadStateSelector" };
            ((Panel)root.Child).Children.Add(target); XYDiagnostic.SetDebugId(root, "XYE.INSPECTOR.ROAD.STATE"); host.Show(root);
            return DiagnosticProbeResolver.Resolve(target);
        });
        Assert.Equal("XYE.INSPECTOR.ROAD.STATE/RoadStateSelector", result.RuntimeLocator);
    }

    [Fact]
    public void Unnamed_locator_uses_zero_based_sibling_type_index()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var root = new Border { Child = new StackPanel() }; var panel = (Panel)root.Child;
            panel.Children.Add(new Border { Child = new TextBlock { Text = "A" } });
            var target = new Border { Child = new TextBlock { Text = "B" } }; panel.Children.Add(target);
            XYDiagnostic.SetDebugId(root, "XYE.TEST.ROOT"); host.Show(root); return DiagnosticProbeResolver.Resolve(target);
        });
        Assert.Equal("XYE.TEST.ROOT", result.ParentDebugId);
        Assert.Equal("XYE.TEST.ROOT/StackPanel[0]/Border[1]", result.RuntimeLocator);
    }

    [Fact]
    public void Missing_data_returns_na_without_throwing()
    {
        var result = DiagnosticProbeResolver.Resolve(new Border());
        Assert.Equal("N/A", result.DebugId); Assert.Equal("N/A", result.ParentDebugId);
        Assert.Equal("N/A", result.Name); Assert.Equal("N/A", result.Text); Assert.Equal("N/A", result.RuntimeLocator);
    }
}
