using System.Text.Json;

internal static class BenchmarkFrameSampleWriter
{
    public static void Write(string path, IReadOnlyList<BenchmarkRun> runs)
    {
        using var output = new StreamWriter(path, false);
        foreach (var run in runs)
        {
            long sequence = 0;
            foreach (var config in run.Configurations)
            foreach (var frame in config.SamplesData)
            {
                var identity = new
                {
                    run.SessionId, run.BuildIdentity, run.SourceFingerprint,
                    frame.FrameId, CameraRevision = config.Pose,
                    DataRevision = run.SourceSha256, FrameSlot = (int?)null,
                    FramesInFlight = 1, ImageIndex = (uint?)null,
                    SwapchainGeneration = (uint?)null, FrameMode = "OffscreenSingle"
                };
                WriteMetric(output, identity, ++sequence,
                    "OffscreenRenderFenceReadbackWall", frame.FenceReadbackWallMs,
                    "ms", "Valid",
                    "CPU wall interval around RenderRgba including synchronous fence/readback");
                WriteMetric(output, identity, ++sequence, "GpuRenderPass", frame.GpuRenderMs,
                    "ms", frame.GpuRenderMs.HasValue ? "Valid" : "Missing",
                    "Vulkan timestamp query; missing samples are never emitted as zero");
            }
        }
    }

    static void WriteMetric(StreamWriter output, object identity, long sampleSequence,
        string metric, double? value, string unit, string status, string details)
    {
        output.WriteLine(JsonSerializer.Serialize(new
        {
            Identity = identity, SampleSequence = sampleSequence, Metric = metric,
            Value = value, Unit = unit, Status = status, Details = details
        }));
    }
}
