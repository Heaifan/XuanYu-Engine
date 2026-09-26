using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class TerrainImportOrchestrationR1Tests
{
    [Fact]
    public async Task ImportRejectsNonHgtSource()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.Equal(TerrainImportState.Idle, vm.TerrainImportState);
        var path = Path.Combine(Path.GetTempPath(), $"terrain-{Guid.NewGuid():N}.txt");
        await File.WriteAllTextAsync(path, "not HGT");

        try
        {
            Assert.False(await vm.ImportTerrainSourceAsync(path));
            Assert.Equal(TerrainImportState.Error, vm.TerrainImportState);
            Assert.NotEqual(TerrainImportState.Completed, vm.TerrainImportState);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ImportCompletesWithTerrainWorldAndRealProgress()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-{Guid.NewGuid():N}.hgt");
        await File.WriteAllBytesAsync(path, new byte[8]);

        try
        {
            Assert.True(await vm.ImportTerrainSourceAsync(path));
            Assert.Equal(TerrainImportState.Completed, vm.TerrainImportState);
            Assert.False(vm.IsTerrainImporting);
            Assert.NotNull(vm.TerrainWorld);
            Assert.Equal(100, vm.TerrainImportPercentage);
            Assert.NotEqual("—", vm.TerrainProbeText);
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public async Task ImportCancellationDoesNotComplete()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-{Guid.NewGuid():N}.hgt");
        using (var stream = File.Create(path)) stream.SetLength(3601L * 3601 * 2);

        try
        {
            var import = vm.ImportTerrainSourceAsync(path);
            for (var attempt = 0; attempt < 100 && !vm.IsTerrainImporting; attempt++)
                await Task.Delay(1);
            Assert.True(vm.IsTerrainImporting);
            Assert.False(await vm.ImportTerrainSourceAsync(path));
            vm.CancelTerrainImportCommand.Execute(null);
            Assert.False(await import);
            Assert.Equal(TerrainImportState.Cancelled, vm.TerrainImportState);
            Assert.NotEqual(TerrainImportState.Completed, vm.TerrainImportState);
        }
        finally { File.Delete(path); }
    }
}
