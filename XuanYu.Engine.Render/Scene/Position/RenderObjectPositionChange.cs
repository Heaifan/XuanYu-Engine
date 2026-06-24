using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;

namespace FluidWarfare.Render.Scene.Position;

/// <summary>
/// RenderObject 位置变更记录。
/// </summary>
public sealed record RenderObjectPositionChange(
    EntityId EntityId,
    Vector3d OldPosition,
    Vector3d NewPosition);
