using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticProbeResolverTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticProbeResolverTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void TextBlock_inside_button_resolves_to_button()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var button = new Button { Name = "ApplyButton", Content = "Apply" }; host.Show(button);
            return DiagnosticProbeResolver.Resolve(button.GetVisualDescendants().OfType<TextBlock>().Single());
        });
        Assert.Equal("Button", result.ControlType);
    }

    [Fact]
    public void ContentPresenter_inside_combo_box_resolves_to_combo_box()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var combo = new ComboBox { Name = "ModeSelector", ItemsSource = new[] { "A", "B" } }; host.Show(combo);
            return DiagnosticProbeResolver.Resolve(combo.GetVisualDescendants().OfType<ContentPresenter>().First());
        });
        Assert.Equal("ComboBox", result.ControlType);
    }

    [Fact]
    public void Named_interactable_wins_over_plain_visual()
    {
        var input = new TextBox { Name = "RoadStateSelector" };
        var result = DiagnosticProbeResolver.Resolve(input);
        Assert.Same(input, result.SemanticTarget); Assert.Equal("RoadStateSelector", result.Name);
    }

    [Fact]
    public void Alt_mode_returns_deepest_raw_visual()
    {
        var border = new Border { Child = new TextBlock() };
        var result = DiagnosticProbeResolver.Resolve((Visual)border.Child!, deepVisual: true);
        Assert.Same(border.Child, result.DeepVisual); Assert.Equal(DiagnosticProbeMode.DeepVisual, result.ProbeMode);
    }

    [Fact]
    public void Nearest_ancestor_debug_id_is_parent_debug_id()
    {
        using var host = new UiRuntimeTestHost(_fixture);
        var result = host.Run(() =>
        {
            var root = new Border { Child = new StackPanel() }; var target = new TextBlock { Name = "StateText" };
            ((Panel)root.Child).Children.Add(target); XYDiagnostic.SetDebugId(root, "XYE.INSPECTOR.ROAD.STATE"); host.Show(root);
            return DiagnosticProbeResolver.Resolve(target);
        });
        Assert.Equal("TextBlock", result.SemanticTarget?.GetType().Name);
        Assert.Equal("XYE.INSPECTOR.ROAD.STATE", result.ParentDebugId); Assert.Equal("N/A", result.DebugId);
    }
}
