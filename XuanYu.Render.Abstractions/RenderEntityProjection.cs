using XuanYu.Core.Identity;
using XuanYu.Core.Math;

namespace XuanYu.Render.Abstractions;

public readonly record struct RenderEntityProjection(
    EntityId Key,
    Vector3d Position);
