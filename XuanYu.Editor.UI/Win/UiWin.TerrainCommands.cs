using Avalonia.Platform.Storage;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    static readonly FilePickerFileType TerrainFileType = new("HGT Terrain Source") { Patterns = ["*.hgt"] };

    async Task ImportTerrain(UiVm vm)
    {
        var files = await RunNativePicker(() => StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "导入 DEM 地形源", AllowMultiple = false, FileTypeFilter = [TerrainFileType]
        }));
        var path = files.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(path)) await vm.ImportTerrainSourceAsync(path);
    }
}
