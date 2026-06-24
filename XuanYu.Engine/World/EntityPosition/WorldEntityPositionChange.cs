using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.World.EntityPosition;

/// <summary>
/// World Entity 位置变更记录。
/// </summary>
public sealed record WorldEntityPositionChange(
    EntityId EntityId,
    Vector3d OldPosition,
    Vector3d NewPosition);
