internal sealed record BenchmarkSummaryRow
{
    public required int Samples { get; init; }
    public required string Pose { get; init; }
    public required int Replicates { get; init; }
    public required int ValidGpuSamples { get; init; }
    public required double? GpuMsP50 { get; init; }
    public required double? GpuMsP95 { get; init; }
    public required double WallMsP50 { get; init; }
    public required double WallMsP95 { get; init; }
    public required double CpuImportMs { get; init; }
    public required double TerrainSampleMs { get; init; }
    public required double TotalWallMs { get; init; }
    public required double CpuMeshBuildMs { get; init; }
    public required double UploadMs { get; init; }
    public required double ColdFrameMs { get; init; }
    public required double EstimatedGpuBytes { get; init; }
    public required string MeasuredGpuBytes { get; init; }
    public required double VisibleChunks { get; init; }
    public required double CulledChunks { get; init; }
    public required double[] LodMeans { get; init; }
    public required double DrawCalls { get; init; }
    public required double Triangles { get; init; }
    public required double TerrainPixels { get; init; }
    public required double TerrainCoverage { get; init; }
    public required double? HeightErrorWorldP95 { get; init; }
    public required double? HeightErrorVisualP95 { get; init; }
    public required bool RepeatPngHashStable { get; init; }
    public required string Status { get; init; }
}
