using Avalonia.Platform.Storage;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    static readonly FilePickerFileType TerrainFileType = new("HGT Terrain Source") { Patterns = ["*.hgt", "*.zip"] };

    async Task ImportTerrain(UiVm vm)
    {
        var files = await RunNativePicker(() => StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "导入 DEM 地形源", AllowMultiple = true, FileTypeFilter = [TerrainFileType]
        }));
        var paths = files.Select(file => file.TryGetLocalPath())
            .Where(path => !string.IsNullOrWhiteSpace(path)).ToArray();
        if (paths.Length > 0) await vm.ImportTerrainSourcesAsync(paths!);
    }
}
