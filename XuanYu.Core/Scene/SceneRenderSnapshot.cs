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
    CameraState? Camera = null,
    bool ShowRotateGizmo = false,
    bool ShowScaleGizmo = false)
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
            "基础测试实体",
            "MinimalSceneEntity",
            CommittedTransform.Identity));

    public bool HasEntity => Entity.IsValid;
    public IReadOnlyList<SceneEntitySnapshot> Entities =>
        RenderEntities ?? (HasEntity ? [Entity] : []);

    public CameraState CameraState => Camera ?? DefaultEditorCamera.Create(0);

    public CommittedTransform RenderTransform => PreviewTransform?.Transform ?? Entity.Transform;
    public Vector3d RenderPosition => RenderTransform.Position;
    public CommittedTransform TransformFor(SceneEntitySnapshot entity)
    {
        if (PreviewTransform is not null && entity.EntityKey == Entity.EntityKey)
        {
            return PreviewTransform.Value.Transform;
        }

        return entity.Transform;
    }

    public Vector3d PositionFor(SceneEntitySnapshot entity)
    {
        if (PreviewTransform is not null && entity.EntityKey == Entity.EntityKey)
        {
            return PreviewTransform.Value.Position;
        }

        return entity.Transform.Position;
    }
}
