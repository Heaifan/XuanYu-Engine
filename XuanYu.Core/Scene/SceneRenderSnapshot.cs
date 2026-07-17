using XuanYu.Core.Identity;

namespace XuanYu.Core.Scene;

public readonly record struct SceneRenderSnapshot(SceneEntitySnapshot Entity)
{
    public static SceneRenderSnapshot TestEntityAtOrigin { get; } = new(
        new SceneEntitySnapshot(
            EntityId.FromInt(1),
            "ARCH-C-R1 Test Entity",
            "MinimalSceneEntity",
            CommittedTransform.Identity));
}
