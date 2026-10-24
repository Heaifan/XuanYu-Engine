using XuanYu.Core.Space.Diagnostics;

namespace XuanYu.Core.Tests.Observability.Replay.Mesh;

public sealed class TerrainMeshBoundariesTests
{
    static readonly TerrainErrorGrid Small = new(
        3, 3, new double[9], new bool[9]);

    [Fact]
    public void RejectsOverflowedGridCellCount()
    {
        var invalid = new TerrainErrorGrid(
            65536, 65536, Array.Empty<double>(), Array.Empty<bool>());
        Assert.Throws<ArgumentException>(
            () => TerrainErrorStatisticsBuilder.Validate(invalid));
    }

    [Fact]
    public void RejectsLodShiftWrap()
    {
        var chunk = new TerrainErrorChunk(0, 0, 0, 0, 2, 2, 32, 1);
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new TerrainErrorSurface(Small, chunk));
    }

    [Fact]
    public void RejectsOverflowedChunkEnd()
    {
        var chunk = new TerrainErrorChunk(0, 0, int.MaxValue, 0, 2, 2, 0, 1);
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new TerrainErrorSurface(Small, chunk));
    }

    [Fact]
    public void OutsideOrNonFiniteSampleHasNoSurface()
    {
        Assert.Null(TerrainErrorSurface.BilinearAt(Small, -0.1, 1));
        Assert.Null(TerrainErrorSurface.BilinearAt(Small, 3, 1));
        Assert.Null(TerrainErrorSurface.BilinearAt(Small, double.NaN, 1));
    }
}
