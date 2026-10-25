namespace XuanYu.Render.Abstractions.Diagnostics.Performance;

public sealed record PerformanceFrameIdentity(
    string SessionId, string BuildIdentity, string SourceFingerprint, long FrameId,
    long CameraRevision, string DataRevision, uint? SwapchainGeneration,
    PerformanceFrameMode FrameMode, int? FrameSlot = null,
    int? FramesInFlight = null, uint? ImageIndex = null);

public enum PerformanceOperationOutcome
{
    Unknown, NotApplicable, Succeeded, Timeout, Failed, DeviceLost, Suboptimal
}

public sealed record PerformanceFrameSample(
    PerformanceFrameIdentity Identity, PerformanceMetric Metric, double? Value,
    PerformanceUnit Unit, PerformanceSampleStatus Status, int StartThreadId,
    int EndThreadId, long? StartTimestamp, long? EndTimestamp, long ClockFrequency,
    double TimestampPeriodNanoseconds, uint TimestampValidBits, string? Detail,
    long SampleSequence = 0,
    PerformanceOperationOutcome OperationOutcome = PerformanceOperationOutcome.Unknown)
{
    public bool IsValid => Status == PerformanceSampleStatus.Valid &&
        Value.HasValue && double.IsFinite(Value.Value);

    public static PerformanceFrameSample Valid(PerformanceFrameIdentity identity,
        PerformanceMetric metric, double value, PerformanceUnit unit,
        int startThreadId, int endThreadId, long? startTimestamp, long? endTimestamp,
        long clockFrequency, double timestampPeriodNanoseconds, uint timestampValidBits,
        PerformanceOperationOutcome outcome = PerformanceOperationOutcome.Unknown,
        long sampleSequence = 0, string? detail = null) =>
        new(identity, metric, value, unit, PerformanceSampleStatus.Valid, startThreadId,
            endThreadId, startTimestamp, endTimestamp, clockFrequency,
            timestampPeriodNanoseconds, timestampValidBits, detail, sampleSequence, outcome);

    public static PerformanceFrameSample Unavailable(PerformanceFrameIdentity identity,
        PerformanceMetric metric, PerformanceSampleStatus status, string? detail,
        PerformanceOperationOutcome outcome = PerformanceOperationOutcome.Unknown) =>
        new(identity, metric, null, PerformanceUnit.Nanoseconds, status, 0, 0,
            null, null, 0, 0, 0, detail, 0, outcome);
}
