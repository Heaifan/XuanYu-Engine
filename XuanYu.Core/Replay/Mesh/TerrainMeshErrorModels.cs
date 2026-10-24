namespace XuanYu.Core.Space.Diagnostics;

/// <summary>Raster values and NoData mask use row-major indexing.</summary>
public sealed record TerrainErrorGrid(int Width,int Height,IReadOnlyList<double> Values,IReadOnlyList<bool> NoData)
{
    public bool IsValid(int x,int y) =>
        x>=0&&y>=0&&x<Width&&y<Height&&
        !NoData[y*Width+x]&&double.IsFinite(Values[y*Width+x]);
    public double At(int x,int y) => Values[y*Width+x];
}
/// <summary>Chunk sample coordinates are inclusive and expressed in render-grid indices.</summary>
public readonly record struct TerrainErrorChunk(int ChunkX,int ChunkY,int StartX,
    int StartY,int CellCountX,int CellCountY,int Lod,int Stride);
public sealed record TerrainErrorOptions(double VerticalExaggeration=1,int ExcludedSourceEdgeSamples=0,
    double? EstimatedPixelsPerWorldUnit=null,double? EstimatedPixelsPerMeter=null,bool SourceRowsSouthToNorth=false);
public sealed record TerrainErrorStatistics(int ValidCount,int NoDataCount,int EdgeExcludedCount,
    double P50Meters,double P95Meters,double P99Meters,double MaxMeters,
    double P50VisualMeters,double P95VisualMeters,double P99VisualMeters,double MaxVisualMeters,
    double? EstimatedP95Pixels,double? EstimatedMaxPixels);
public sealed record TerrainErrorChunkResult(int ChunkX,int ChunkY,int Lod,int Stride,TerrainErrorStatistics Statistics)
{
    /// <summary>Source-grid residuals against this chunk's final LOD surface; null for legacy producers.</summary>
    public TerrainErrorStatistics? SourceToFinalMesh { get; init; }
}
/// <summary>SourceToRender uses bilinear raster reconstruction; mesh comparisons use piecewise BL-TR triangles.</summary>
public sealed record TerrainMeshErrorReport(TerrainErrorStatistics SourceToRender,
    IReadOnlyList<TerrainErrorChunkResult> RenderToMeshByChunkLod,TerrainErrorStatistics SourceToFinalMesh,
    int SkirtVerticesExcluded,string ScreenErrorClassification);
