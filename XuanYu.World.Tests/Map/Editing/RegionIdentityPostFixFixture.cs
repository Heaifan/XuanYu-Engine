using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests.Map.Editing;

static class RegionIdentityPostFixFixture
{
    public static async Task<(UiVm Vm, string Root, string Path)> CreateAsync()
    {
        var root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-identity-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, "map.json");
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        Assert.True(await vm.SaveMapManifestAsync(path));
        vm.ToggleEditorMode();
        vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
        return (vm, root, path);
    }

    public static void AssertIdentity(UiVm vm)
    {
        Assert.True(vm.MapSession.CurrentMap.MapId.IsValid);
        Assert.False(string.IsNullOrWhiteSpace(vm.CurrentMapManifest.Id));
        Assert.Equal(vm.MapSession.CurrentMap.MapId.Value, vm.CurrentMapManifest.Id);
    }

    public static void Cleanup(string root)
    {
        try { if (Directory.Exists(root)) Directory.Delete(root, true); }
        catch (IOException) { }
    }
}
