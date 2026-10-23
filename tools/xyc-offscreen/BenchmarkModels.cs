internal sealed record BenchmarkFrame(int Index, long FrameId, double? GpuRenderMs,
    double FenceReadbackWallMs, double EncodeMs, string ImageSha256);

internal sealed record BenchmarkChunkProbe(string Chunk, string Lod, string Cull,
    double ProjectedPixels, string Result);

internal sealed record BenchmarkConfiguration
{
    public string Device { get; init; } = "UNKNOWN";
    public uint DeviceDriverVersion { get; init; }
    public uint TimestampValidBits { get; init; }
    public double TimestampPeriodNs { get; init; }
    public required int Samples { get; init; }
    public required string Pose { get; init; }
    public required string Status { get; init; }
    public required double TerrainSampleMs { get; init; }
    public required double TotalWallMs { get; init; }
    public required double RendererSetupMs { get; init; }
    public required double ColdFrameMs { get; init; }
    public required double? ColdGpuRenderMs { get; init; }
    public required double ColdEncodeMs { get; init; }
    public string FailureReason { get; init; } = "";
    public required double CpuMeshBuildMs { get; init; }
    public required double CpuUploadMs { get; init; }
    public required double EstimatedGpuBytes { get; init; }
    public required string MeasuredGpuBytes { get; init; }
    public required bool CameraInsideChunkAabb { get; init; }
    public required string ChunkIdentity { get; init; }
    public required object Camera { get; init; }
    public required int VisibleChunks { get; init; }
    public required int CulledChunks { get; init; }
    public required int Lod0 { get; init; }
    public required int Lod1 { get; init; }
    public required int Lod2 { get; init; }
    public required int Lod3 { get; init; }
    public required int Lod4 { get; init; }
    public required int DrawCalls { get; init; }
    public required int Triangles { get; init; }
    public required int TerrainPixels { get; init; }
    public required double TerrainCoverage { get; init; }
    public required double? HeightErrorWorldP95Meters { get; init; }
    public required double? HeightErrorVisualP95Meters { get; init; }
    public required ImageComparison? ComparisonToPreviousResolution { get; init; }
    public required string Png { get; init; }
    public required string PngSha256 { get; init; }
    public required int ValidGpuSamples { get; init; }
    public required double? GpuMsP50 { get; init; }
    public required double? GpuMsP95 { get; init; }
    public required double? GpuMsP99 { get; init; }
    public required double? WallMsP50 { get; init; }
    public required double? WallMsP95 { get; init; }
    public required double? WallMsP99 { get; init; }
    public required List<BenchmarkFrame> SamplesData { get; init; }
    public required List<BenchmarkChunkProbe> ChunkProbes { get; init; }
}

internal sealed record BenchmarkExecution(BenchmarkConfiguration Configuration, byte[]? Pixels);

internal sealed record BenchmarkRun(string BuildIdentity, string Device, uint DriverVersion,
    string OperatingSystem, string Runtime, string CpuIdentifier, string Source,
    string SourceSha256, double ImportMs, uint Width, uint Height, int Replicate,
    double TimestampPeriodNs, uint TimestampValidBits, List<BenchmarkConfiguration> Configurations)
{
    public string SessionId { get; init; } = "UNKNOWN";
    public string SourceFingerprint { get; init; } = "UNKNOWN";
}
