using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

internal readonly record struct ScreenAxisProjection(double X, double Y, double PixelsPerWorldUnit)
{
    public bool IsValid => PixelsPerWorldUnit > 1e-9 &&
                           double.IsFinite(X) &&
                           double.IsFinite(Y) &&
                           double.IsFinite(PixelsPerWorldUnit);

    public double DistanceFrom(double startX, double startY, double currentX, double currentY) =>
        ((currentX - startX) * X + (currentY - startY) * Y) / PixelsPerWorldUnit;
}
