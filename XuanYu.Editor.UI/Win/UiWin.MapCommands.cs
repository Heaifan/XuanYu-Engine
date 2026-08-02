using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace XuanYu.Editor.UI;

// MAP-A-R1-D5-A：地图文件选择器（打开/保存 .xymap），镜像场景文件命令模式。
public partial class UiWin
{
    static readonly FilePickerFileType MapFileType = new("玄域地图")
    {
        Patterns = ["*.xymap"]
    };

    async Task RunMapCommand(string command)
    {
        if (DataContext is not UiVm vm) return;
        if (command == "新建地图") { vm.NewMap(); return; }
        if (command == "卸载地图") { vm.UnloadMapFromEditor(); return; }
        if (command == "聚焦地图") { vm.FocusMap(); return; }
        if (command == "打开地图") { await OpenMap(vm); return; }
        if (command == "保存地图") { await SaveMapExistingOrPick(vm); return; }
    }

    async Task OpenMap(UiVm vm)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "打开玄域地图",
            AllowMultiple = false,
            FileTypeFilter = [MapFileType]
        });
        var path = files.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(path)) await vm.OpenMapAsync(path);
    }

    async Task<bool> SaveMapExistingOrPick(UiVm vm) =>
        !string.IsNullOrWhiteSpace(vm.MapPath)
            ? await vm.SaveMapAsync(vm.MapPath)
            : await SaveMapAs(vm);

    async Task<bool> SaveMapAs(UiVm vm)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "保存玄域地图",
            SuggestedFileName = $"{vm.MapName}.xymap",
            FileTypeChoices = [MapFileType]
        });
        var path = file?.TryGetLocalPath();
        return !string.IsNullOrWhiteSpace(path) && await vm.SaveMapAsync(path);
    }
}
