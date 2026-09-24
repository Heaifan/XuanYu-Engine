using Avalonia;

namespace XuanYu.Editor.UI.Diagnostic;

public static class DiagnosticPlacementPolicy
{
    public static DiagnosticPlacementResult Place(DiagnosticPlacementRequest request)
    {
        var tooLarge = request.CardSize.Width > request.AvailableBounds.Width ||
            request.CardSize.Height > request.AvailableBounds.Height;
        foreach (var candidate in Candidates(request))
        {
            var constrained = Constrain(candidate.Bounds, request.AvailableBounds);
            if (request.TargetKind == DiagnosticPlacementTargetKind.SmallControl &&
                constrained.Bounds.Intersects(request.TargetBounds)) continue;
            if (AvoidsPointerHotZone(request) &&
                constrained.Bounds.Intersects(PointerHotZone(request))) continue;
            if (tooLarge || !Fits(constrained.Bounds, request.AvailableBounds)) continue;
            return Result(constrained, candidate.Kind, false);
        }
        var fallback = Constrain(new Rect(request.TargetBounds.TopLeft, request.CardSize),
            request.AvailableBounds);
        return Result(fallback, DiagnosticPlacementKind.Fallback, true, tooLarge);
    }

    static Candidate[] Candidates(DiagnosticPlacementRequest request) =>
        request.TargetKind == DiagnosticPlacementTargetKind.SmallControl
            ? SmallCandidates(request) : PointerCandidates(request);

    static Candidate[] SmallCandidates(DiagnosticPlacementRequest r) => new[]
    {
        C(new Point(r.TargetBounds.Right + r.SafeDistance, r.TargetBounds.Top),
            DiagnosticPlacementKind.Right, r.CardSize),
        C(new Point(r.TargetBounds.Left - r.SafeDistance - r.CardSize.Width,
            r.TargetBounds.Top), DiagnosticPlacementKind.Left, r.CardSize),
        C(new Point(r.TargetBounds.Left, r.TargetBounds.Bottom + r.SafeDistance),
            DiagnosticPlacementKind.Bottom, r.CardSize),
        C(new Point(r.TargetBounds.Left, r.TargetBounds.Top - r.SafeDistance - r.CardSize.Height),
            DiagnosticPlacementKind.Top, r.CardSize)
    };

    static Candidate[] PointerCandidates(DiagnosticPlacementRequest r) => new[]
    {
        C(new Point(r.Pointer.X + r.SafeDistance, r.Pointer.Y + r.SafeDistance),
            DiagnosticPlacementKind.RightBottom, r.CardSize),
        C(new Point(r.Pointer.X + r.SafeDistance, r.Pointer.Y - r.SafeDistance - r.CardSize.Height),
            DiagnosticPlacementKind.RightTop, r.CardSize),
        C(new Point(r.Pointer.X - r.SafeDistance - r.CardSize.Width, r.Pointer.Y + r.SafeDistance),
            DiagnosticPlacementKind.LeftBottom, r.CardSize),
        C(new Point(r.Pointer.X - r.SafeDistance - r.CardSize.Width,
            r.Pointer.Y - r.SafeDistance - r.CardSize.Height),
            DiagnosticPlacementKind.LeftTop, r.CardSize)
    };

    static bool AvoidsPointerHotZone(DiagnosticPlacementRequest request) =>
        request.TargetKind is DiagnosticPlacementTargetKind.Viewport or
            DiagnosticPlacementTargetKind.PointerAnchored;

    static Rect PointerHotZone(DiagnosticPlacementRequest request)
    {
        var radius = Math.Max(16, request.SafeDistance);
        return new Rect(request.Pointer.X - radius, request.Pointer.Y - radius,
            radius * 2, radius * 2);
    }

    static Candidate C(Point location, DiagnosticPlacementKind kind, Size size) =>
        new(new Rect(location, size), kind);

    static DiagnosticPlacementResult Result(Constrained value,
        DiagnosticPlacementKind kind, bool usedFallback, bool doesNotFit = false) =>
        new(value.Bounds, kind, value.WasClamped, usedFallback, doesNotFit);

    static bool Fits(Rect value, Rect available) => value.Left >= available.Left &&
        value.Top >= available.Top && value.Right <= available.Right &&
        value.Bottom <= available.Bottom;

    static Constrained Constrain(Rect value, Rect available)
    {
        var x = Clamp(value.X, available.Left, Math.Max(available.Left,
            available.Right - value.Width));
        var y = Clamp(value.Y, available.Top, Math.Max(available.Top,
            available.Bottom - value.Height));
        return new(new Rect(x, y, value.Width, value.Height),
            !AreEqual(x, value.X) || !AreEqual(y, value.Y));
    }

    static double Clamp(double value, double min, double max) => Math.Min(max, Math.Max(min, value));
    static bool AreEqual(double left, double right) => Math.Abs(left - right) < 0.001;
    readonly record struct Candidate(Rect Bounds, DiagnosticPlacementKind Kind);
    readonly record struct Constrained(Rect Bounds, bool WasClamped);
}
