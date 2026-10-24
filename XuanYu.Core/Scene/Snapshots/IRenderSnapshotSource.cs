namespace XuanYu.Core.Scene;

public interface IRenderSnapshotSource
{
    SceneRenderSnapshot RenderSnapshot { get; }

    event Action<SceneRenderSnapshot>? RenderSnapshotChanged;
}
