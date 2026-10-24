using XuanYu.Core.Scene;

namespace XuanYu.Core.Tests.Scene.Render;

public sealed class RenderSourceTests
{
    sealed class Source : ISceneRenderSnapshotSource
    {
        public SceneRenderSnapshot RenderSnapshot { get; private set; } =
            SceneRenderSnapshot.Empty;

        public event Action<SceneRenderSnapshot>? RenderSnapshotChanged;

        public void Publish(SceneRenderSnapshot snapshot)
        {
            RenderSnapshot = snapshot;
            RenderSnapshotChanged?.Invoke(snapshot);
        }
    }

    [Fact]
    public void PublishesSnapshot()
    {
        var source = new Source();
        SceneRenderSnapshot? observed = null;
        source.RenderSnapshotChanged += snapshot => observed = snapshot;
        var next = SceneRenderSnapshot.TestEntityAtOrigin;

        source.Publish(next);

        Assert.Equal(next, source.RenderSnapshot);
        Assert.True(observed.HasValue);
        Assert.Equal(next, observed.Value);
    }
}
