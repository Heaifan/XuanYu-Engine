using XuanYu.Render.Vulkan.Session;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanTerrainPipelineLifecycleTests
{
    [Fact]
    public void Create_failure_does_not_inject_transfer_or_dispose()
    {
        var injected = false;
        var transferred = false;
        var disposeCount = 0;

        var result = VulkanRenderSession.TryTransferTerrainPipeline<TestPipeline>(
            pipeline: null,
            inject: _ => injected = true,
            transfer: _ => transferred = true,
            dispose: _ => disposeCount++);

        Assert.False(result);
        Assert.False(injected);
        Assert.False(transferred);
        Assert.Equal(0, disposeCount);
    }

    [Fact]
    public void Injection_failure_disposes_local_exactly_once_without_session_transfer()
    {
        var pipeline = new TestPipeline();
        var transferred = false;

        var error = Assert.Throws<InvalidOperationException>(() =>
            VulkanRenderSession.TryTransferTerrainPipeline(
                pipeline,
                inject: _ => throw new InvalidOperationException("rerecord failed"),
                transfer: _ => transferred = true,
                dispose: value => value.Dispose()));

        Assert.Equal("rerecord failed", error.Message);
        Assert.False(transferred);
        Assert.Equal(1, pipeline.DisposeCount);
    }

    [Fact]
    public void Successful_injection_transfers_session_ownership_without_local_dispose()
    {
        var pipeline = new TestPipeline();
        TestPipeline? sessionOwner = null;

        var result = VulkanRenderSession.TryTransferTerrainPipeline(
            pipeline,
            inject: _ => { },
            transfer: value => sessionOwner = value,
            dispose: value => value.Dispose());

        Assert.True(result);
        Assert.Same(pipeline, sessionOwner);
        Assert.Equal(0, pipeline.DisposeCount);

        sessionOwner!.Dispose();
        Assert.Equal(1, pipeline.DisposeCount);
    }

    private sealed class TestPipeline
    {
        public int DisposeCount { get; private set; }

        public void Dispose() => DisposeCount++;
    }
}
