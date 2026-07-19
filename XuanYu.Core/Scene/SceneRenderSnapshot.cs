using XuanYu.Core.Identity;
using XuanYu.Core.Math;
using XuanYu.Core.Transform;

namespace XuanYu.Core.Scene;

public readonly record struct SceneRenderSnapshot(
    SceneEntitySnapshot Entity,
    bool IsSelected = false,
    PreviewTransform? PreviewTransform = null)
{
    public static SceneRenderSnapshot TestEntityAtOrigin { get; } = new(
        new SceneEntitySnapshot(
            EntityId.FromInt(1),
            "ARCH-C-R1 Test Entity",
            "MinimalSceneEntity",
            CommittedTransform.Identity));

    public Vector3d RenderPosition => PreviewTransform?.Position ?? Entity.Transform.Position;
}
