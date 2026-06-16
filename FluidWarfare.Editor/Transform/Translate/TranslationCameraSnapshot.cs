using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

public sealed record TranslationCameraSnapshot
{
    public required Vector3d Position { get; init; }

    public required double[] ViewProjection { get; init; }

    public required double[] InverseViewProjection { get; init; }

    public required int ViewportWidth { get; init; }

    public required int ViewportHeight { get; init; }

    public bool IsValid => ViewProjection is { Length: 16 } &&
                           InverseViewProjection is { Length: 16 } &&
                           ViewportWidth > 0 &&
                           ViewportHeight > 0;
}
