using XuanYu.Core.Scene;

namespace XuanYu.Render.Vulkan;

public sealed partial class VulkanNativeHostSurfaceBridge
{
    void OnSceneRenderSnapshotChanged(SceneRenderSnapshot snapshot)
    {
        _renderSession?.UpdateScene(snapshot);
        Emit($"ARCH-C-R1 scene snapshot committed: {snapshot.Entity.EntityKey} {snapshot.Entity.Transform.Position}");
    }
}
