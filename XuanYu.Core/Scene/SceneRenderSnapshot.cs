using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Core.Transform;

namespace XuanYu.Core.Scene;

public readonly record struct SceneRenderSnapshot(
    SceneEntitySnapshot Entity,
    bool IsSelected = false,
    PreviewTransform? PreviewTransform = null,
    bool ShowMoveGizmo = false,
    IReadOnlyList<SceneEntitySnapshot>? RenderEntities = null,
    CameraState? Camera = null)
{
    public static SceneRenderSnapshot Empty { get; } = new(
        new SceneEntitySnapshot(
            EntityId.None,
            "",
            "",
            CommittedTransform.Identity));

    public static SceneRenderSnapshot TestEntityAtOrigin { get; } = new(
        new SceneEntitySnapshot(
            EntityId.FromInt(1),
            "ARCH-C-R1 Test Entity",
            "MinimalSceneEntity",
            CommittedTransform.Identity));

    public bool HasEntity => Entity.IsValid;
    public IReadOnlyList<SceneEntitySnapshot> Entities =>
        RenderEntities ?? (HasEntity ? [Entity] : []);

    public CameraState CameraState => Camera ?? DefaultEditorCamera.Create(0);

    public Vector3d RenderPosition => PreviewTransform?.Position ?? Entity.Transform.Position;
    public Vector3d PositionFor(SceneEntitySnapshot entity)
    {
        if (PreviewTransform is not null && entity.EntityKey == Entity.EntityKey)
        {
            return PreviewTransform.Value.Position;
        }

        return entity.Transform.Position;
    }
}
