using XuanYu.Editor.UI;

using Avalonia.Platform.Storage;

namespace XuanYu.Editor.UI;

// MAP-DOC-A-R1：UiWin 承担地图新建/聚焦与 map.json 文件选择器；状态操作仍走 UiVm。
// 面板按钮命令统一走 UiVm.RunCommand → UiWin 文件选择器 → MapManifestStorageService。
public partial class UiWin
{
    static readonly FilePickerFileType MapManifestFileType = new("玄域地图") { Patterns = ["map.json"] };

    async Task RunMapCommand(string command)
    {
        if (DataContext is not UiVm vm) return;
        if (command == "新建地图") { vm.NewMap(); return; }
        if (command == "聚焦地图") { vm.FocusMap(); return; }
        if (command == "打开地图")
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "打开玄域地图 Manifest",
                AllowMultiple = false,
                FileTypeFilter = [MapManifestFileType]
            });
            var path = files.FirstOrDefault()?.TryGetLocalPath();
            if (!string.IsNullOrWhiteSpace(path)) await vm.OpenMapManifestAsync(path);
            return;
        }
        if (command == "保存地图")
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "保存玄域地图 Manifest",
                SuggestedFileName = "map.json",
                FileTypeChoices = [MapManifestFileType]
            });
            var path = file?.TryGetLocalPath();
            if (!string.IsNullOrWhiteSpace(path)) await vm.SaveMapManifestAsync(path);
        }
    }
}
