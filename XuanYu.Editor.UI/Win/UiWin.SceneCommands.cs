using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    static readonly FilePickerFileType SceneFileType = new("玄域场景")
    {
        Patterns = ["*.xyscene"]
    };

    static readonly FilePickerFileType GlbFileType = new("glTF 二进制模型")
    {
        Patterns = ["*.glb"]
    };

    async Task<bool> HandleSceneShortcut(KeyEventArgs e)
    {
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Control)) return false;
        var shift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);
        var command = e.Key switch
        {
            Key.N => "新建",
            Key.O => "打开",
            Key.S when shift => "另存为",
            Key.S => "保存",
            _ => ""
        };
        if (command == "") return false;
        e.Handled = true;
        await RunSceneCommand(command);
        return true;
    }

    async Task RunSceneCommand(string command)
    {
        if (DataContext is not UiVm vm) return;
        if (command is "新建地图" or "聚焦地图")
        {
            if (command == "新建地图")
            {
                // D5：新建地图会替换当前地图属性并清空修改历史——危险确认
                var proceed = await ShowDanger("新建地图",
                    "新建地图将替换当前地图属性并清空地图修改历史，此操作不可撤销。是否继续？");
                if (proceed != "ok") return;
            }
            await RunMapCommand(command);
            return;
        }
        if (command is "新建" or "打开" && !await ConfirmUnsavedBeforeContinue(vm)) return;
        if (command == "新建") { vm.NewBlankScene(); return; }
        if (command == "打开") { await OpenScene(vm); return; }
        if (command == "导入 GLB") { await ImportGlb(vm); return; }
        if (command == "保存" && await SaveExistingOrPick(vm)) return;
        if (command == "另存为") await SaveSceneAs(vm);
    }

    async Task ImportGlb(UiVm vm)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "导入 GLB 模型",
            AllowMultiple = false,
            FileTypeFilter = [GlbFileType]
        });
        var path = files.FirstOrDefault()?.TryGetLocalPath();
        if (string.IsNullOrWhiteSpace(path)) return;
        vm.ImportStaticModel(path);
    }

    async Task OpenScene(UiVm vm)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "打开玄域场景",
            AllowMultiple = false,
            FileTypeFilter = [SceneFileType]
        });
        var path = files.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrWhiteSpace(path)) await vm.OpenSceneAsync(path);
    }

    async Task<bool> SaveExistingOrPick(UiVm vm) =>
        !string.IsNullOrWhiteSpace(vm.CurrentScenePath)
            ? await vm.SaveSceneAsync()
            : await SaveSceneAs(vm);

    async Task<bool> SaveSceneAs(UiVm vm)
    {
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "保存玄域场景",
            SuggestedFileName = "untitled.xyscene",
            FileTypeChoices = [SceneFileType]
        });
        var path = file?.TryGetLocalPath();
        return !string.IsNullOrWhiteSpace(path) && await vm.SaveSceneAsync(path, saveAs: true);
    }
}
