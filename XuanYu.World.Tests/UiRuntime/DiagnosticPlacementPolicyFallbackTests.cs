using Avalonia;
using XuanYu.Editor.UI.Diagnostic;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class DiagnosticPlacementPolicyFallbackTests
{
    [Fact]
    public void Small_control_uses_explicit_fallback_when_all_sides_overlap()
    {
        var client = new Rect(0, 0, 800, 600);
        var request = new DiagnosticPlacementRequest(
            DiagnosticPlacementTargetKind.SmallControl, client,
            new Point(400, 300), new Size(220, 120), client);

        var result = DiagnosticPlacementPolicy.Place(request);

        Assert.True(result.UsedFallback);
        Assert.Equal(DiagnosticPlacementKind.Fallback, result.Placement);
        Assert.False(result.DoesNotFit);
    }

    [Fact]
    public void Card_larger_than_available_bounds_is_not_silently_resized()
    {
        var client = new Rect(0, 0, 180, 90);
        var request = new DiagnosticPlacementRequest(
            DiagnosticPlacementTargetKind.PointerAnchored, new Rect(),
            new Point(90, 45), new Size(220, 120), client);

        var result = DiagnosticPlacementPolicy.Place(request);

        Assert.True(result.DoesNotFit);
        Assert.True(result.UsedFallback);
        Assert.Equal(220, result.CardBounds.Width);
        Assert.Equal(120, result.CardBounds.Height);
        Assert.Equal(client.TopLeft, result.CardBounds.TopLeft);
    }
}
