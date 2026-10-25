namespace XuanYu.Render.Abstractions.Diagnostics.Performance;

public enum PerformanceFenceWaitTarget
{
    FrameSlotGraphicsFence, AcquiredImageOwnerGraphicsFence,
    SwapchainImagePresentFence
}

public sealed record RawRendererFrameIdentity(
    string? SessionId, long FrameId, long CameraRevision, int? FrameSlot,
    uint? ImageIndex, uint? SwapchainGeneration, int FramesInFlight);

public sealed record RawFenceWaitObservation(
    RawRendererFrameIdentity Identity, string WaitTarget,
    long StartTimestamp, long EndTimestamp, long ClockFrequency,
    int StartThreadId, int EndThreadId, string VulkanResult);

public sealed record PerformanceRunIdentity(string SessionId, string BuildIdentity,
    string SourceFingerprint, string DataRevision, PerformanceFrameMode FrameMode);

public static class PerformanceFenceWaitAdapter
{
    public static PerformanceFrameSample Convert(RawFenceWaitObservation raw,
        PerformanceRunIdentity run)
    {
        ArgumentNullException.ThrowIfNull(raw);
        ArgumentNullException.ThrowIfNull(run);
        var identity = CreateIdentity(raw.Identity, run);
        var outcome = MapOutcome(raw.VulkanResult);
        var detail = $"WaitTarget={raw.WaitTarget};VulkanResult={raw.VulkanResult}";
        if (!Enum.TryParse<PerformanceFenceWaitTarget>(raw.WaitTarget, out var target) ||
            !Enum.IsDefined(target))
            return Invalid(identity, detail + ";UnknownWaitTarget", outcome);
        var value = PerformanceClock.ElapsedNanoseconds(
            raw.StartTimestamp, raw.EndTimestamp, raw.ClockFrequency);
        if (!value.HasValue) return Invalid(identity, detail, outcome);
        return PerformanceFrameSample.Valid(identity, PerformanceMetric.FenceWait,
            value.Value, PerformanceUnit.Nanoseconds, raw.StartThreadId,
            raw.EndThreadId, raw.StartTimestamp, raw.EndTimestamp,
            raw.ClockFrequency, 0, 0, outcome, detail: detail);
    }

    static PerformanceFrameIdentity CreateIdentity(RawRendererFrameIdentity raw,
        PerformanceRunIdentity run) => new(raw.SessionId ?? run.SessionId,
            run.BuildIdentity, run.SourceFingerprint, raw.FrameId, raw.CameraRevision,
            run.DataRevision, raw.SwapchainGeneration, run.FrameMode,
            raw.FrameSlot, raw.FramesInFlight, raw.ImageIndex);

    static PerformanceFrameSample Invalid(PerformanceFrameIdentity identity,
        string detail, PerformanceOperationOutcome outcome) =>
        PerformanceFrameSample.Unavailable(identity, PerformanceMetric.FenceWait,
            PerformanceSampleStatus.Invalid, detail, outcome);

    static PerformanceOperationOutcome MapOutcome(string result) => result switch
    {
        "Success" => PerformanceOperationOutcome.Succeeded,
        "Timeout" => PerformanceOperationOutcome.Timeout,
        "ErrorDeviceLost" => PerformanceOperationOutcome.DeviceLost,
        "SuboptimalKhr" => PerformanceOperationOutcome.Suboptimal,
        _ => PerformanceOperationOutcome.Failed
    };
}
