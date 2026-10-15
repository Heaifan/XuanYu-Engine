using System.Globalization;

internal static class BenchmarkCsvWriter
{
    public static void Write(string path, IEnumerable<BenchmarkSummaryRow> rows)
    {
        var lines = new List<string> { "samples,pose,replicates,validSamples,gpuMsP50,gpuMsP95,wallMsP50,wallMsP95,cpuImportMsP50,cpuImportMsP95,terrainSampleMsP50,terrainSampleMsP95,cpuBuildMsP50,cpuBuildMsP95,uploadMsP50,uploadMsP95,cpuImportMsAvg,terrainSampleMsAvg,cpuBuildMsAvg,uploadMsAvg,totalWallMsAvg,coldFrameMsAvg,estimatedGpuBytes,measuredGpuBytes,visibleChunks,culledChunks,lod0,lod1,lod2,lod3,lod4,drawCalls,triangles,terrainPixels,terrainCoveragePct,heightErrorWorldP95M,heightErrorVisualP95M,repeatPngHashStable,status" };
        foreach (var x in rows)
            lines.Add(string.Join(',', x.Samples, Q(x.Pose), x.Replicates, x.ValidGpuSamples,
                F(x.GpuMsP50), F(x.GpuMsP95), F(x.WallMsP50), F(x.WallMsP95),
                F(x.CpuImportMsP50), F(x.CpuImportMsP95), F(x.TerrainSampleMsP50), F(x.TerrainSampleMsP95),
                F(x.CpuMeshBuildMsP50), F(x.CpuMeshBuildMsP95), F(x.UploadMsP50), F(x.UploadMsP95), F(x.CpuImportMs),
                F(x.TerrainSampleMs), F(x.CpuMeshBuildMs), F(x.UploadMs), F(x.TotalWallMs), F(x.ColdFrameMs),
                F(x.EstimatedGpuBytes), Q(x.MeasuredGpuBytes), F(x.VisibleChunks), F(x.CulledChunks),
                F(x.LodMeans[0]), F(x.LodMeans[1]), F(x.LodMeans[2]), F(x.LodMeans[3]), F(x.LodMeans[4]),
                F(x.DrawCalls), F(x.Triangles), F(x.TerrainPixels), F(x.TerrainCoverage),
                F(x.HeightErrorWorldP95), F(x.HeightErrorVisualP95), x.RepeatPngHashStable, Q(x.Status)));
        File.WriteAllLines(path, lines);
    }

    static string F(double? value) => value?.ToString("0.###", CultureInfo.InvariantCulture) ?? "UNKNOWN";
    static string Q(string value) => $"\"{value.Replace("\"", "\"\"")}\"";
}
