using XuanYu.Core.Identity;

namespace XuanYu.Editor.SceneDocument;

public sealed record SceneDocumentSnapshot(
    string SceneId,
    string SceneName,
    IReadOnlyList<SceneDocumentEntity> Entities,
    IReadOnlyList<SceneDocumentAsset>? Assets = null)
{
    public static SceneDocumentSnapshot Empty(string name) =>
        new(Guid.NewGuid().ToString("N"), name, [], []);
}
