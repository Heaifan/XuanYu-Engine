using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Runtime.InteropServices;
using XuanYu.World.Terrain.Import;

internal static class TerrainBenchmarkRunner
{
    const string ExpectedSourceSha = "BEF65347232885016C50F73B9EBDC09D52B82FD5EB4517377220F7B8211340E1";
    static long _nextFrameId;

    public static long NextFrameId() => Interlocked.Increment(ref _nextFrameId);
    static readonly string[] PoseNames = ["far-topdown", "mid-2km", "near-oblique", "chunk-aabb-entry"];

    public static int Run(BenchmarkOptions options)
    {
        if (!File.Exists(options.Source)) throw new FileNotFoundException("NASADEM HGT 不存在", options.Source);
        if (options.Replicate < 1) throw new ArgumentOutOfRangeException(nameof(options.Replicate));
        var sourceHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(options.Source)));
        if (sourceHash != ExpectedSourceSha) throw new InvalidDataException($"n24e121 SHA256 不匹配：{sourceHash}");
        var poses = options.Poses.Contains("all") ? PoseNames : options.Poses;
        if (poses.Any(x => !PoseNames.Contains(x))) throw new ArgumentException("--pose 名称不在固定矩阵中");
        var runDirectory = Path.Combine(options.OutputDirectory, $"run-{options.Replicate}");
        Directory.CreateDirectory(runDirectory);
        Environment.SetEnvironmentVariable("XYE_PROBE_VIEWPORT", "1");
        var import = Stopwatch.StartNew();
        var tile = TerrainSourceImport.ReadMany([options.Source]).Tiles.Single(); import.Stop();
        var results = new List<BenchmarkConfiguration>();
        foreach (var pose in poses)
        {
            byte[]? previous = null; var previousSamples = 0;
            foreach (var samples in options.Samples.Order())
            {
                Console.WriteLine($"B1 RUN={options.Replicate} SAMPLE={samples} POSE={pose}");
                var execution = BenchmarkConfigurationRunner.Run(tile, options, samples, pose, runDirectory);
                var config = execution.Configuration;
                if (previous is not null && execution.Pixels is not null)
                {
                    var diffPath = Path.Combine(runDirectory, $"diff-{previousSamples}-vs-{samples}-{pose}.png");
                    var comparison = BenchmarkImageAnalysis.Compare(previous, execution.Pixels, 1024, 768, diffPath);
                    config = config with { ComparisonToPreviousResolution = comparison };
                }
                results.Add(config);
                previous = execution.Pixels; previousSamples = samples;
                Console.WriteLine($"B1 RESULT={config.Status} GPU_P50={config.GpuMsP50?.ToString("F3") ?? "UNKNOWN"}ms; GPU_P95={config.GpuMsP95?.ToString("F3") ?? "UNKNOWN"}ms; triangles={config.Triangles}");
            }
        }
        var run = new BenchmarkRun(Environment.GetEnvironmentVariable("XYC_BUILD_ID") ?? "UNDECLARED",
            results.FirstOrDefault(x => x.Device != "UNKNOWN")?.Device ?? "UNKNOWN",
            results.FirstOrDefault(x => x.DeviceDriverVersion > 0)?.DeviceDriverVersion ?? 0,
            RuntimeInformation.OSDescription, RuntimeInformation.FrameworkDescription,
            Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "UNKNOWN", options.Source,
            sourceHash, import.Elapsed.TotalMilliseconds, 1024, 768, options.Replicate,
            results.FirstOrDefault(x => x.TimestampPeriodNs > 0)?.TimestampPeriodNs ?? 0,
            results.FirstOrDefault(x => x.TimestampValidBits > 0)?.TimestampValidBits ?? 0, results)
        {
            SessionId = $"{DateTimeOffset.UtcNow:yyyyMMddTHHmmssfffZ}-run-{options.Replicate}",
            SourceFingerprint = Environment.GetEnvironmentVariable("XYE_SOURCE_FINGERPRINT") ?? "UNKNOWN"
        };
        var jsonPath = Path.Combine(runDirectory, "run-manifest.json");
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(run, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"B1 RUN MANIFEST={jsonPath}");
        return results.Any(x => x.Status is "FAILED" or "BLOCKED_RESOURCES" or "BLOCKED_DEVICE" or "INVALID_POSE") ? 1 : 0;
    }
}
