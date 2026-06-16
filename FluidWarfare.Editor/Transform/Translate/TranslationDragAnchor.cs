using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

internal sealed record TranslationDragAnchor
{
    public required TranslationConstraint Constraint { get; init; }

    public required Vector3d InitialPosition { get; init; }

    public required Vector3d ReferencePosition { get; init; }

    public required double PointerX { get; init; }

    public required double PointerY { get; init; }

    public required Vector3d StartHit { get; init; }

    public required Vector3d PlaneNormal { get; init; }

    public required ScreenAxisProjection ScreenAxis { get; init; }

    public required TranslationMappingMode Mode { get; init; }
}
