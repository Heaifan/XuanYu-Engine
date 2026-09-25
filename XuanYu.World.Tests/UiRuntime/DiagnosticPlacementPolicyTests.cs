using Avalonia;
using XuanYu.Editor.UI.Diagnostic;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class DiagnosticPlacementPolicyTests
{
    static readonly Rect Client = new(0, 0, 1600, 900);
    static readonly Size Card = new(220, 120);

    [Fact]
    public void Small_control_prefers_a_non_overlapping_nearby_side()
    {
        var target = new Rect(400, 300, 96, 32);
        var result = DiagnosticPlacementPolicy.Place(Request(
            DiagnosticPlacementTargetKind.SmallControl, target));

        Assert.False(result.CardBounds.Intersects(target));
        Assert.False(result.UsedFallback);
        Assert.Equal(DiagnosticPlacementKind.Right, result.Placement);
    }

    [Fact]
    public void Small_control_uses_target_bounds_even_when_pointer_is_elsewhere()
    {
        var target = new Rect(400, 300, 96, 32);
        var result = DiagnosticPlacementPolicy.Place(Request(
            DiagnosticPlacementTargetKind.SmallControl, target, new Point(1200, 700)));

        Assert.Equal(new Point(target.Right + 8, target.Top), result.CardBounds.TopLeft);
    }

    [Fact]
    public void Viewport_may_overlap_target_and_stays_inside_client_bounds()
    {
        var viewport = new Rect(40, 40, 1500, 800);
        var result = DiagnosticPlacementPolicy.Place(Request(
            DiagnosticPlacementTargetKind.Viewport, viewport,
            new Point(800, 400)));

        Assert.True(result.CardBounds.Intersects(viewport));
        Assert.True(Client.Contains(result.CardBounds.TopLeft));
        Assert.True(Client.Contains(result.CardBounds.BottomRight));
        Assert.False(result.UsedFallback);
    }

    [Fact]
    public void Viewport_click_location_changes_pointer_anchored_card_position()
    {
        var viewport = new Rect(40, 40, 1500, 800);
        var first = DiagnosticPlacementPolicy.Place(Request(
            DiagnosticPlacementTargetKind.Viewport, viewport, new Point(300, 300)));
        var second = DiagnosticPlacementPolicy.Place(Request(
            DiagnosticPlacementTargetKind.Viewport, viewport, new Point(1100, 600)));

        Assert.NotEqual(first.CardBounds.TopLeft, second.CardBounds.TopLeft);
    }

    static DiagnosticPlacementRequest Request(
        DiagnosticPlacementTargetKind kind, Rect target,
        Point pointer = default) => new(kind, target, pointer, Card, Client);
}
