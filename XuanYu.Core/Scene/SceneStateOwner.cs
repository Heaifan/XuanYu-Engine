using XuanYu.Core.Math;

namespace XuanYu.Core.Scene;

public sealed class SceneStateOwner : ISceneRenderSnapshotSource
{
    SceneRenderSnapshot _snapshot = SceneRenderSnapshot.TestEntityAtOrigin;

    public SceneRenderSnapshot RenderSnapshot => _snapshot;

    public event Action<SceneRenderSnapshot>? RenderSnapshotChanged;

    public bool CommitPosition(Vector3d position)
    {
        var current = _snapshot.Entity;
        var transform = new CommittedTransform(position);
        if (current.Transform == transform) return false;
        _snapshot = new SceneRenderSnapshot(current with
        {
            Transform = transform
        });
        RenderSnapshotChanged?.Invoke(_snapshot);
        return true;
    }
}
