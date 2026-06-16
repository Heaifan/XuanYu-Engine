using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

public readonly record struct TranslationResult(
    string EntityId,
    Vector3d InitialPosition,
    Vector3d? FinalPosition,
    bool IsConfirmed,
    bool IsCancelled,
    bool InitialWasDirty)
{
    public bool HasPositionChanged =>
        FinalPosition is not null &&
        InitialPosition.DistanceSquaredTo(FinalPosition.Value) > 1e-12;
}
