using Avalonia;
using XuanYu.Editor.UI.Diagnostic;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class DiagnosticPlacementPolicyEdgesTests
{
    static readonly Rect Client = new(0, 0, 800, 600);

    [Fact]
    public void Toolbar_target_uses_a_non_overlapping_candidate_after_clamp()
    {
        var target = new Rect(490, 20, 300, 40);
        var result = Place(DiagnosticPlacementTargetKind.SmallControl,
            target, new Size(220, 120));

        Assert.False(result.CardBounds.Intersects(target));
        Assert.False(result.UsedFallback);
        Assert.Equal(DiagnosticPlacementKind.Left, result.Placement);
    }

    [Fact]
    public void Small_control_targets_near_each_client_edge_remain_visible_and_clear()
    {
        var targets = new[] { new Rect(0, 0, 96, 32), new Rect(704, 0, 96, 32),
            new Rect(0, 568, 96, 32), new Rect(704, 568, 96, 32) };
        foreach (var target in targets)
        {
            var result = Place(DiagnosticPlacementTargetKind.SmallControl,
                target, new Size(120, 80));
            Assert.False(result.CardBounds.Intersects(target));
            AssertInside(result.CardBounds);
        }
    }

    [Fact]
    public void Huge_canvas_uses_local_hotspot_instead_of_forbidding_canvas()
    {
        var canvas = new Rect(0, 0, 1500, 800);
        var result = Place(DiagnosticPlacementTargetKind.LargeSurface,
            canvas, new Size(220, 120), new Point(700, 350));

        Assert.True(result.CardBounds.Intersects(canvas));
        Assert.False(result.UsedFallback);
    }

    [Fact]
    public void Pointer_edges_are_inside_and_deterministic()
    {
        var points = new[] { new Point(0, 0), new Point(800, 0),
            new Point(0, 600), new Point(800, 600) };
        foreach (var point in points)
        {
            var first = Place(DiagnosticPlacementTargetKind.PointerAnchored,
                new Rect(), new Size(220, 120), point);
            var second = Place(DiagnosticPlacementTargetKind.PointerAnchored,
                new Rect(), new Size(220, 120), point);
            Assert.Equal(first, second);
            AssertInside(first.CardBounds);
        }
    }

    static DiagnosticPlacementResult Place(DiagnosticPlacementTargetKind kind,
        Rect target, Size card, Point pointer = default) =>
        DiagnosticPlacementPolicy.Place(new(kind, target, pointer, card, Client));

    static void AssertInside(Rect value)
    {
        Assert.True(value.Left >= Client.Left && value.Top >= Client.Top);
        Assert.True(value.Right <= Client.Right && value.Bottom <= Client.Bottom);
    }
}
