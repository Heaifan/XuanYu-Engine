namespace XuanYu.Core.Scene;

public interface ISceneRenderSnapshotSource
{
    SceneRenderSnapshot RenderSnapshot { get; }

    event Action<SceneRenderSnapshot>? RenderSnapshotChanged;
}
