using XuanYu.Core.Math;

namespace XuanYu.Core.Scene;

public sealed class SceneStateOwner : ISceneRenderSnapshotSource
{
    SceneRenderSnapshot _snapshot = SceneRenderSnapshot.TestEntityAtOrigin;

    public SceneRenderSnapshot RenderSnapshot => _snapshot;

    public event Action<SceneRenderSnapshot>? RenderSnapshotChanged;

    public void CommitPosition(Vector3d position)
    {
        var current = _snapshot.Entity;
        _snapshot = new SceneRenderSnapshot(current with
        {
            Transform = new CommittedTransform(position)
        });
        RenderSnapshotChanged?.Invoke(_snapshot);
    }
}
