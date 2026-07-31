using XuanYu.Core.Identity;
using XuanYu.Core.Math;

namespace XuanYu.Render.Abstractions;

public readonly record struct RenderEntityProjection(
    EntityId Key,
    Vector3d Position,
    Vector3d Rotation,
    Vector3d Scale,
    bool IsSelected = false,
    RenderEntityType EntityType = RenderEntityType.LegacyMinimalTriangle);
