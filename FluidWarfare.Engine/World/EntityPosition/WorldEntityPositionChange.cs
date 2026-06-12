using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;

namespace FluidWarfare.Engine.World.EntityPosition;

/// <summary>
/// World Entity 位置变更记录。
/// </summary>
public sealed record WorldEntityPositionChange(
    EntityId EntityId,
    Vector3d OldPosition,
    Vector3d NewPosition);
