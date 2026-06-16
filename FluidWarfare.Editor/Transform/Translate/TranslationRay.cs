using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

public readonly record struct TranslationRay(Vector3d Origin, Vector3d Direction)
{
    public Vector3d At(double t) => Origin + Direction * t;
}
