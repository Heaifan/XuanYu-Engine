using XuanYu.Editor.MapDocument;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Map.Editing;

public sealed class UiMapManifestIdentityPostFixTests
{
    [Fact]
    public async Task Auto_dataset_initialization_produces_one_consistent_identity()
    {
        var (vm, root, _) = await RegionIdentityPostFixFixture.CreateAsync();
        try
        {
            Assert.True(await vm.BeginRegionDrawingAsync());
            RegionIdentityPostFixFixture.AssertIdentity(vm);
            var result = MapManifestIdentityValidator.Validate(
                vm.MapSession.CurrentMap.MapId, vm.CurrentMapManifest);
            Assert.True(result.Succeeded);
        }
        finally { RegionIdentityPostFixFixture.Cleanup(root); }
    }

    [Fact]
    public async Task Save_reload_and_recovery_preserve_identity()
    {
        var (vm, root, path) = await RegionIdentityPostFixFixture.CreateAsync();
        try
        {
            var before = vm.MapSession.CurrentMap;
            await File.WriteAllTextAsync(Path.Combine(root, "bad.json"), "{bad");
            Assert.False(await vm.OpenMapManifestAsync(Path.Combine(root, "bad.json")));
            Assert.Equal(before, vm.MapSession.CurrentMap);
            Assert.False(string.IsNullOrWhiteSpace(vm.FooterMessage));
            Assert.True(await vm.OpenMapManifestAsync(path));
            RegionIdentityPostFixFixture.AssertIdentity(vm);
        }
        finally { RegionIdentityPostFixFixture.Cleanup(root); }
    }

    [Fact]
    public async Task Region_entry_exit_and_first_three_points_keep_identity()
    {
        var (vm, root, _) = await RegionIdentityPostFixFixture.CreateAsync();
        try
        {
            Assert.True(await vm.BeginRegionDrawingAsync());
            var expected = vm.CurrentMapManifest.Id;
            var viewport = new XuanYu.Core.Space.ViewportState(0, 0, 800, 600, 800, 600, 1, 1);
            foreach (var point in new[] { (100d, 100d), (700d, 100d), (700d, 500d) })
            {
                Assert.True(vm.RegionDrawingPointerPressed(point.Item1, point.Item2, viewport));
                Assert.Equal(expected, vm.CurrentMapManifest.Id);
                Assert.Equal(expected, vm.MapSession.CurrentMap.MapId.Value);
            }
            Assert.True(vm.CancelRegionDrawingFromEscape());
            Assert.Equal(expected, vm.CurrentMapManifest.Id);
            Assert.Equal(expected, vm.MapSession.CurrentMap.MapId.Value);
        }
        finally { RegionIdentityPostFixFixture.Cleanup(root); }
    }
}
