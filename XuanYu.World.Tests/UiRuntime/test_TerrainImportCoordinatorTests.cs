using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class test_TerrainImportCoordinatorTests
{
    [Fact]
    public async Task Missing_source_finishes_as_error_without_activation()
    {
        var coordinator = new TerrainImportCoordinator();
        var activated = false;
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"), "missing.hgt");

        var succeeded = await coordinator.RunAsync([path], _ => activated = true);

        Assert.False(succeeded);
        Assert.Equal(TerrainImportState.Error, coordinator.State);
        Assert.False(activated);
    }
}
