using XuanYu.Core.Scene;

namespace XuanYu.Render.Vulkan.Render;

public sealed unsafe partial class VulkanClearFrameOwner
{
    readonly object _sceneLock = new();
    SceneRenderSnapshot _pendingSceneSnapshot;
    bool _sceneSnapshotPending;

    public void QueueSceneSnapshot(SceneRenderSnapshot snapshot)
    {
        lock (_sceneLock)
        {
            if (!_sceneSnapshotPending && _sceneSnapshot == snapshot) return;
            _pendingSceneSnapshot = snapshot;
            _sceneSnapshotPending = true;
        }
    }

    public bool TryApplyPendingSceneSnapshot()
    {
        SceneRenderSnapshot snapshot;
        lock (_sceneLock)
        {
            if (!_sceneSnapshotPending) return true;
            snapshot = _pendingSceneSnapshot;
            _sceneSnapshotPending = false;
            _sceneSnapshot = snapshot;
        }
        return _views.Length == 0 || RecordCommandBuffers(_views);
    }
}
