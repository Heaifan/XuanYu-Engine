using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Render.Selection;

/// <summary>
/// Picking 操作结果。NoHit 表示未选中任何对象。
/// </summary>
public sealed record RenderScenePickResult(
    bool IsHit,
    EntityId? EntityId,
    string? ObjectName,
    double Distance,
    Vector3d? WorldHitPosition,
    int TestedObjectCount)
{
    public static readonly RenderScenePickResult NoHit = new(
        false, null, null, 0, null, 0);

    /// <summary>
    /// 创建一个命中结果。
    /// </summary>
    public static RenderScenePickResult Hit(
        EntityId entityId,
        string objectName,
        double distance,
        Vector3d worldHitPosition,
        int testedObjectCount) =>
        new(true, entityId, objectName, distance, worldHitPosition, testedObjectCount);
}
