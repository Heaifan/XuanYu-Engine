namespace XuanYu.Render.Abstractions.Diagnostics.Performance;

public sealed record PerformanceMetricStatistics(
    PerformanceMetric Metric, int SampleCount, int ValidCount, int MissingCount,
    int UnsupportedCount, int InvalidCount, int TimeoutCount, int ExceptionCount,
    int OperationTimeoutCount, int OperationFailureCount, int DeviceLostCount,
    long DroppedCount, double? P50, double? P95, double? P99);

public static class PerformanceStatistics
{
    public static PerformanceMetricStatistics Calculate(PerformanceMetric metric,
        IEnumerable<PerformanceFrameSample> samples, long droppedCount)
    {
        var rows = samples.Where(sample => sample.Metric == metric).ToArray();
        var values = rows.Where(sample => sample.IsValid).Select(sample => sample.Value!.Value)
            .Order().ToArray();
        return new(metric, rows.Length, values.Length,
            Count(rows, PerformanceSampleStatus.Missing),
            Count(rows, PerformanceSampleStatus.Unsupported),
            rows.Count(sample => sample.Status == PerformanceSampleStatus.Invalid ||
                sample.Status == PerformanceSampleStatus.Valid && !sample.IsValid),
            Count(rows, PerformanceSampleStatus.Timeout),
            Count(rows, PerformanceSampleStatus.Exception),
            CountOutcome(rows, PerformanceOperationOutcome.Timeout),
            CountOutcome(rows, PerformanceOperationOutcome.Failed),
            CountOutcome(rows, PerformanceOperationOutcome.DeviceLost), droppedCount,
            Rank(values, .50), Rank(values, .95), Rank(values, .99));
    }

    static int Count(PerformanceFrameSample[] rows, PerformanceSampleStatus status) =>
        rows.Count(sample => sample.Status == status);

    static int CountOutcome(PerformanceFrameSample[] rows,
        PerformanceOperationOutcome outcome) =>
        rows.Count(sample => sample.OperationOutcome == outcome);

    static double? Rank(double[] values, double percentile) => values.Length == 0 ? null :
        values[(int)Math.Ceiling(percentile * values.Length) - 1];
}
