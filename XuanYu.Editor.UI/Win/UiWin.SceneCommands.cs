using Avalonia.Input;
using Avalonia.Platform.Storage;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    static readonly FilePickerFileType SceneFileType = new("玄域场景") { Patterns = ["*.xyscene"] };

    static readonly FilePickerFileType GlbFileType = new("glTF 二进制模型") { Patterns = ["*.glb"] };

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
            if (command == "新建地图" && !await ConfirmNewMapUnsaved(vm)) return;
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

    // D5 纠偏：新建地图未保存流程——无修改直接新建；有修改 → 保存并新建/不保存并新建/取消
    async Task<bool> ConfirmNewMapUnsaved(UiVm vm)
    {
        if (!vm.HasUnsavedMapChanges) return true;
        var choice = await ShowUnsavedMapChangesDialog();
        if (choice == "cancel") return false;
        if (choice == "save")
        {
            vm.RunCommand.Execute("应用地图属性");
            return !vm.IsMapFormError; // 应用失败：不新建，表单错误区已提示
        }
        return true; // discard：不保存并新建
    }
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
