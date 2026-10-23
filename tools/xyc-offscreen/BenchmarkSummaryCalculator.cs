using XuanYu.Render.Vulkan.Benchmark;

internal static class BenchmarkSummaryCalculator
{
    public static List<BenchmarkSummaryRow> Summarize(IReadOnlyList<BenchmarkRun> runs) =>
        runs.SelectMany(x => x.Configurations).GroupBy(x => (x.Samples, x.Pose))
            .OrderBy(x => x.Key.Pose).ThenBy(x => x.Key.Samples)
            .Select(group => Build(group.ToArray(), runs)).ToList();

    static BenchmarkSummaryRow Build(BenchmarkConfiguration[] rows, IReadOnlyList<BenchmarkRun> runs)
    {
        var gpu = rows.SelectMany(x => x.SamplesData).Where(x => x.GpuRenderMs.HasValue)
            .Select(x => x.GpuRenderMs!.Value).ToArray();
        var wall = rows.SelectMany(x => x.SamplesData).Select(x => x.FenceReadbackWallMs).ToArray();
        var gpuStats = gpu.Length >= 30 ? VulkanBenchmarkPercentileCalculator.Calculate(gpu) : (VulkanBenchmarkPercentiles?)null;
        var wallStats = wall.Length == 0 ? (VulkanBenchmarkPercentiles?)null :
            VulkanBenchmarkPercentileCalculator.Calculate(wall);
        var importStats = VulkanBenchmarkPercentileCalculator.Calculate(runs.Select(x => x.ImportMs).ToArray());
        var terrainSampleStats = VulkanBenchmarkPercentileCalculator.Calculate(rows.Select(x => x.TerrainSampleMs).ToArray());
        var meshBuildStats = VulkanBenchmarkPercentileCalculator.Calculate(rows.Select(x => x.CpuMeshBuildMs).ToArray());
        var uploadStats = VulkanBenchmarkPercentileCalculator.Calculate(rows.Select(x => x.CpuUploadMs).ToArray());
        double Avg(Func<BenchmarkConfiguration, double> select) => rows.Average(select);
        var allStatus = rows.All(x => x.Status == "PASS") && rows.Length == runs.Count && runs.Count == 3;
        var status = !allStatus ? rows.Any(x => x.Status == "BLOCKED_RESOURCES") ? "BLOCKED_RESOURCES" : "INCOMPLETE" :
            gpuStats is null || rows.Any(x => x.ValidGpuSamples < 30) ? "INSUFFICIENT_SAMPLE" : "PASS";
        return new()
        {
            Samples = rows[0].Samples, Pose = rows[0].Pose, Replicates = rows.Length,
            ValidGpuSamples = gpu.Length, GpuMsP50 = gpuStats?.P50, GpuMsP95 = gpuStats?.P95,
            GpuMsP99 = gpuStats?.P99, WallMsP50 = wallStats?.P50, WallMsP95 = wallStats?.P95,
            WallMsP99 = wallStats?.P99,
            CpuImportMs = runs.Average(x => x.ImportMs),
            CpuImportMsP50 = importStats.P50, CpuImportMsP95 = importStats.P95,
            TerrainSampleMsP50 = terrainSampleStats.P50, TerrainSampleMsP95 = terrainSampleStats.P95,
            TerrainSampleMs = Avg(x => x.TerrainSampleMs),
            TotalWallMs = Avg(x => x.TotalWallMs),
            CpuMeshBuildMs = Avg(x => x.CpuMeshBuildMs), UploadMs = Avg(x => x.CpuUploadMs),
            CpuMeshBuildMsP50 = meshBuildStats.P50, CpuMeshBuildMsP95 = meshBuildStats.P95,
            UploadMsP50 = uploadStats.P50, UploadMsP95 = uploadStats.P95,
            ColdFrameMs = Avg(x => x.ColdFrameMs), EstimatedGpuBytes = Avg(x => x.EstimatedGpuBytes),
            MeasuredGpuBytes = "UNKNOWN", VisibleChunks = Avg(x => x.VisibleChunks),
            CulledChunks = Avg(x => x.CulledChunks), LodMeans = [Avg(x => x.Lod0), Avg(x => x.Lod1),
                Avg(x => x.Lod2), Avg(x => x.Lod3), Avg(x => x.Lod4)],
            DrawCalls = Avg(x => x.DrawCalls), Triangles = Avg(x => x.Triangles),
            TerrainPixels = Avg(x => x.TerrainPixels), TerrainCoverage = Avg(x => x.TerrainCoverage),
            HeightErrorWorldP95 = AverageNullable(rows.Select(x => x.HeightErrorWorldP95Meters)),
            HeightErrorVisualP95 = AverageNullable(rows.Select(x => x.HeightErrorVisualP95Meters)),
            RepeatPngHashStable = rows.Select(x => x.PngSha256).Distinct().Count() == 1, Status = status
        };
    }

    static double? AverageNullable(IEnumerable<double?> values)
    {
        var valid = values.Where(x => x.HasValue).Select(x => x!.Value).ToArray();
        return valid.Length == 0 ? null : valid.Average();
    }
}
