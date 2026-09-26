using XuanYu.World.Terrain.Import;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainImportProgressAcceptanceTests
{
    [Fact]
    public void Reader_progress_stays_in_range_and_does_not_regress()
    {
        var values = new TerrainImportAcceptanceFixture.ProgressCapture();
        var reader = (IHgtReader)new HgtTerrainElevationTileReader();
        using var stream = TerrainImportAcceptanceFixture.HgtStream(1, 2, 3, 4);

        reader.Read(stream, "n23e121", values);

        Assert.NotEmpty(values.Values);
        Assert.All(values.Values, value => Assert.InRange(value.Percentage, 0, 100));
        Assert.True(values.Values.Zip(values.Values.Skip(1), (left, right) => right.Percentage >= left.Percentage)
            .All(item => item));
    }
}
