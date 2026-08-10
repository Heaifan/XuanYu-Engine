using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;

namespace XuanYu.Core.Tests.Render;

public sealed class LatestRenderProjectionQueueTests
{
    [Fact]
    public void R11_consumes_only_latest_projection()
    {
        var queue = new LatestRenderProjectionQueue();
        queue.Publish(RenderProjectionResult.Ok(new(default, [], false, new(1, 0, 0))));
        queue.Publish(RenderProjectionResult.Ok(new(default, [], false, new(2, 0, 0))));

        Assert.True(queue.TryConsume(out var result));
        Assert.Equal(2, result.Projection.GizmoPosition.X);
        Assert.False(queue.TryConsume(out _));
    }
}
