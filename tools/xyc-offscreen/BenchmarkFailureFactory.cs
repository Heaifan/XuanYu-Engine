internal static class BenchmarkFailureFactory
{
    public static void CheckMemoryBudget(int samples)
    {
        var reserve = (long)samples * samples * 32 + 128L * 1024 * 1024;
        if (reserve > GC.GetGCMemoryInfo().TotalAvailableMemoryBytes * 0.75)
            throw new OutOfMemoryException($"采样分辨率 {samples} 的内存预算预检未通过：{reserve} bytes");
    }

    public static BenchmarkExecution Create(int samples, string pose,
        XuanYu.Render.Abstractions.RenderCameraProjection? camera, double sampleMs,
        double totalWallMs, string path, Exception error) => new(new()
    {
        Samples = samples, Pose = pose,
        Status = error is OutOfMemoryException || error.Message.Contains("DEVICE_LOST", StringComparison.OrdinalIgnoreCase)
            ? "BLOCKED_RESOURCES" : error.Message.Contains("目标 GPU", StringComparison.Ordinal) ? "BLOCKED_DEVICE" : "FAILED",
        FailureReason = error.ToString(), TerrainSampleMs = sampleMs,
        TotalWallMs = totalWallMs, RendererSetupMs = 0,
        ColdFrameMs = 0, ColdGpuRenderMs = null, ColdEncodeMs = 0,
        CpuMeshBuildMs = 0, CpuUploadMs = 0, EstimatedGpuBytes = 0,
        MeasuredGpuBytes = "UNKNOWN", CameraInsideChunkAabb = false,
        ChunkIdentity = "UNKNOWN", Camera = camera ?? new object(), VisibleChunks = 0,
        CulledChunks = 0, Lod0 = 0, Lod1 = 0, Lod2 = 0, Lod3 = 0, Lod4 = 0,
        DrawCalls = 0, Triangles = 0, TerrainPixels = 0, TerrainCoverage = 0,
        HeightErrorWorldP95Meters = null, HeightErrorVisualP95Meters = null,
        ComparisonToPreviousResolution = null, Png = path, PngSha256 = "UNKNOWN",
        ValidGpuSamples = 0, GpuMsP50 = null, GpuMsP95 = null,
        WallMsP50 = null, WallMsP95 = null, WallMsP99 = null, GpuMsP99 = null,
        SamplesData = [], ChunkProbes = []
    }, null);
}
