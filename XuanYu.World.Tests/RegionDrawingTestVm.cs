using XuanYu.Editor.UI;
using XuanYu.Editor.Workspace;

namespace XuanYu.World.Tests;

static class RegionDrawingTestVm
{
    public static UiVm Create()
    {
        var root = Path.Combine(Path.GetTempPath(), $"xuanyu-region-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(root);
        try
        {
            var vm = new UiVm(null, () => true, seedInitialScene: false);
            var path = Path.Combine(root, "map.json");
            vm.SaveMapManifestAsync(path).GetAwaiter().GetResult();
            vm.DatasetCreateType = "region";
            vm.CreateDatasetAsync().GetAwaiter().GetResult();
            var id = vm.DatasetSelectedId!;
            vm.ToggleEditorMode(); vm.SelectDataset(id);
            vm.SwitchWorkspaceCommand.Execute(EditorWorkspaceId.RegionEditor);
            return vm;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }
}
