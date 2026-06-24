using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Render.Selection.Pointer;

namespace FluidWarfare.Editor.Windows.Viewport.Picking;

/// <summary>
/// ViewportPointerPickRoute.Pick 的输出结果。
/// 包含拾取类型和对应数据，不含数学细节。
/// </summary>
public sealed record ViewportPickResult(
    ViewportPickKind Kind,
    EntityId? EntityId,
    Vector3d? GroundPosition)
{
    public static readonly ViewportPickResult None =
        new(ViewportPickKind.None, null, null);
    public static ViewportPickResult FromEntity(EntityId id) =>
        new(ViewportPickKind.Entity, id, null);
    public static ViewportPickResult FromGround(Vector3d pos) =>
        new(ViewportPickKind.Ground, null, pos);

    public static ViewportPickResult FromPointerResult(ScenePointerPickResult r) => r.Kind switch
    {
        ScenePointerPickKind.Entity when r.EntityId is not null =>
            FromEntity(r.EntityId.Value),
        ScenePointerPickKind.Ground when r.GroundPosition is not null =>
            FromGround(r.GroundPosition.Value),
        _ => None,
    };
}

/// <summary>拾取结果类型。</summary>
public enum ViewportPickKind
{
    None,
    Entity,
    Ground,
    Failure,
}
