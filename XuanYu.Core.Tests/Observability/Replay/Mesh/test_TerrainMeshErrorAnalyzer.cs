using XuanYu.Core.Space.Diagnostics;

namespace XuanYu.Core.Tests.Observability.Replay.Mesh;

public sealed class TerrainMeshErrorAnalyzerTests
{
    [Fact]
    public void Flat_grid_has_zero_error_and_reports_chunk_lod()
    {
        var grid = Make(5, 5, (_, _) => 10);
        var report = TerrainMeshErrorAnalyzer.Analyze(grid, grid,
            [new(0, 0, 0, 0, 4, 4, 0, 1)], new(VerticalExaggeration: 3,
                EstimatedPixelsPerMeter: 2), skirtVerticesExcluded: 16);

        Assert.Equal(0, report.SourceToRender.MaxMeters);
        Assert.Equal(0, report.SourceToFinalMesh.MaxVisualMeters);
        Assert.Equal((0, 0, 1), (report.RenderToMeshByChunkLod[0].ChunkX,
            report.RenderToMeshByChunkLod[0].Lod, report.RenderToMeshByChunkLod[0].Stride));
        Assert.Equal(16, report.SkirtVerticesExcluded);
        Assert.Equal("estimated", report.ScreenErrorClassification);
        Assert.Equal(0, report.SourceToFinalMesh.EstimatedMaxPixels);
        Assert.Equal(0, report.RenderToMeshByChunkLod[0].SourceToFinalMesh!.MaxMeters);
    }

    [Fact]
    public void Lod_surface_uses_renderer_diagonal_and_excludes_nodata_and_edges()
    {
        // Saddle isolates the chosen diagonal: BL-TR has zero center deviation for this witness.
        var grid = Make(5, 5, (x, y) => x == 2 && y == 2 ? 100 : 0,
            (x, y) => x == 0 && y == 0);
        var report = TerrainMeshErrorAnalyzer.Analyze(grid, grid,
            [new(2, 3, 0, 0, 4, 4, 1, 2)], new(ExcludedSourceEdgeSamples: 1));

        var chunk = Assert.Single(report.RenderToMeshByChunkLod);
        Assert.True(chunk.Statistics.NoDataCount > 0);
        Assert.True(chunk.Statistics.ValidCount > 0);
        Assert.True(chunk.Statistics.MaxMeters > 0);
        Assert.True(report.SourceToRender.EdgeExcludedCount > 0);
        Assert.Equal("unavailable", report.ScreenErrorClassification);
    }

    [Fact]
    public void South_to_north_source_rows_are_flipped_for_both_source_comparisons()
    {
        var source = Make(3, 3, (_, y) => y * 10);
        var render = Make(3, 3, (_, y) => (2 - y) * 10);
        var chunk = new TerrainErrorChunk(0, 0, 0, 0, 2, 2, 0, 1);

        var unflipped = TerrainMeshErrorAnalyzer.Analyze(source, render, [chunk]);
        var flipped = TerrainMeshErrorAnalyzer.Analyze(source, render, [chunk],
            new(SourceRowsSouthToNorth: true));

        Assert.True(unflipped.SourceToRender.MaxMeters > 0);
        Assert.Equal(0, flipped.SourceToRender.MaxMeters);
        Assert.True(unflipped.SourceToFinalMesh.MaxMeters > 0);
        Assert.Equal(0, flipped.SourceToFinalMesh.MaxMeters);
        Assert.True(unflipped.RenderToMeshByChunkLod[0].SourceToFinalMesh!.MaxMeters > 0);
        Assert.Equal(0, flipped.RenderToMeshByChunkLod[0].SourceToFinalMesh!.MaxMeters);
        Assert.Equal(0, flipped.RenderToMeshByChunkLod[0].Statistics.MaxMeters);
    }

    [Fact]
    public void Source_to_render_bilinear_error_is_separate_from_triangle_mesh_error()
    {
        var source = Make(3, 3, (x, y) => 25d * x * y);
        var render = Make(2, 2, (x, y) => x == 1 && y == 1 ? 100 : 0);
        var report = TerrainMeshErrorAnalyzer.Analyze(source, render,
            [new(0, 0, 0, 0, 1, 1, 0, 1)]);

        Assert.Equal(0, report.SourceToRender.MaxMeters);
        Assert.Equal(0, report.RenderToMeshByChunkLod[0].Statistics.MaxMeters);
        Assert.Equal(25, report.SourceToFinalMesh.MaxMeters);
        Assert.Equal(25, report.RenderToMeshByChunkLod[0].SourceToFinalMesh!.MaxMeters);
    }

    static TerrainErrorGrid Make(int w, int h, Func<int, int, double> value,
        Func<int, int, bool>? noData = null)
    {
        var values = new double[w*h]; var mask = new bool[w*h];
        for(var y=0;y<h;y++) for(var x=0;x<w;x++) { values[y*w+x]=value(x,y); mask[y*w+x]=noData?.Invoke(x,y)??false; }
        return new(w,h,values,mask);
    }
}
