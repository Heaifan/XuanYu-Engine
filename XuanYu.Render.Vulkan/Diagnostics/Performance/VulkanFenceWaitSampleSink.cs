using XuanYu.Render.Abstractions.Diagnostics;
using XuanYu.Render.Abstractions.Diagnostics.Performance;
using XuanYu.Render.Vulkan.Render.Frame;

namespace XuanYu.Render.Vulkan.Diagnostics.Performance;

public sealed class VulkanFenceWaitSampleSink
{
    readonly PerformanceSampleBuffer _buffer = new(8192);
    readonly PerformanceRunIdentity _run;

    public VulkanFenceWaitSampleSink(string sessionId)
    {
        _run = new PerformanceRunIdentity(sessionId, RendererBuildIdentity.Current(),
            Environment.GetEnvironmentVariable("XYE_SOURCE_FINGERPRINT") ?? "NOT AVAILABLE",
            "NOT AVAILABLE", PerformanceFrameMode.Single);
    }

    public long DroppedCount => _buffer.DroppedCount;

    public void Record(VulkanFenceWaitObservation observation)
    {
        ArgumentNullException.ThrowIfNull(observation);
        var id = observation.Identity;
        var raw = new RawFenceWaitObservation(new RawRendererFrameIdentity(id.SessionId,
            id.FrameId, id.CameraRevision, id.FrameSlot, id.ImageIndex,
            id.SwapchainGeneration, id.FramesInFlight), observation.WaitTarget.ToString(),
            observation.StartTimestamp, observation.EndTimestamp,
            observation.ClockFrequency, observation.StartThreadId,
            observation.EndThreadId, observation.Result.ToString());
        var run = _run with { FrameMode = id.FramesInFlight == 2 ?
            PerformanceFrameMode.Double : PerformanceFrameMode.Single };
        _buffer.TryWrite(PerformanceFenceWaitAdapter.Convert(raw, run));
    }

    public IReadOnlyList<PerformanceFrameSample> Drain() => _buffer.Drain();
}
