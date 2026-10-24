using XuanYu.Core.Math;

namespace XuanYu.Core.Space;

public readonly record struct WorldRay
{
    public WorldRay(Vector3d origin, Vector3d direction)
    {
        ValidateFinite(origin, nameof(origin));
        ValidateFinite(direction, nameof(direction));
        if (direction.IsZero) throw new ArgumentOutOfRangeException(nameof(direction));

        Origin = origin;
        Direction = direction.Normalize();
    }

    public Vector3d Origin { get; }

    public Vector3d Direction { get; }

    static void ValidateFinite(Vector3d vector, string name)
    {
        if (!double.IsFinite(vector.X) || !double.IsFinite(vector.Y) || !double.IsFinite(vector.Z))
        {
            throw new ArgumentOutOfRangeException(name);
        }
    }
}
