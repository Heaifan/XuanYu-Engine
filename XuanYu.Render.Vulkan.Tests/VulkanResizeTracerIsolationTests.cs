using System.Diagnostics;
using System.Threading;
using XuanYu.Render.Vulkan.Diagnostic;
using Xunit;

namespace XuanYu.Render.Vulkan.Tests;

public sealed class VulkanResizeTracerIsolationTests
{
    [Fact]
    public async Task Concurrent_sessions_keep_independent_trace_clocks()
    {
        using var barrier = new Barrier(2);
        var slow = Task.Run(() => MeasureTrace(barrier, 90));
        var fast = Task.Run(() => MeasureTrace(barrier, 10));

        var durations = await Task.WhenAll(slow, fast);

        Assert.True(durations[0] > durations[1] + 35,
            $"Trace durations overlapped: slow={durations[0]:F1}ms, fast={durations[1]:F1}ms");
    }

    static double MeasureTrace(Barrier barrier, int delay)
    {
        VulkanResizeTracer.StartTrace();
        barrier.SignalAndWait();
        Thread.Sleep(delay);
        return VulkanResizeTracer.ElapsedMs();
    }
}
