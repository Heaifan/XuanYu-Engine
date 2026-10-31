using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（纠偏）：危险操作确认接线（fail-closed——确认处理器缺失/取消均不执行；
// 危险按钮非默认焦点，Enter=取消；按钮文案为具体动作）。
public partial class UiWin
{
    const string LayerDeleteConfirmationIntent =
        "删除图层将移除该图层及其中的对象，此操作不可撤销。是否删除？";

    async void OnDangerousCommandRequested(string name)
    {
        if (_attachedVm is null) return;
        if (name is "删除图层" or "解除注册数据集")
        {
            await ConfirmLayerDeleteAsync(_attachedVm, name);
            return;
        }
        var (message, action) = name switch
        {
            _ => ($"执行「{name}」将丢弃相关修改且不可撤销。是否继续？", "继续")
        };
        if (await ShowDanger("危险操作", message, action) == "ok")
            _attachedVm.ConfirmDangerousCommand(name);
        else
            _attachedVm.CancelDangerousCommand(name);
    }

    async Task ConfirmLayerDeleteAsync(UiVm vm, string command)
    {
        var layer = vm.SelectedLayer;
        if (layer is null) { vm.CancelDangerousCommand(command); return; }
        var (title, action, intent) = command == "解除注册数据集"
            ? ("移除区域数据集", "移除", "确定从当前地图移除此区域数据集？Dataset 文件仍保留在磁盘。")
            : ("删除图层", "删除", LayerDeleteConfirmationIntent);
        var confirmed = false;
        try { confirmed = await LayerDeleteConfirmationWindow.ShowAsync(this, layer.Name,
            layer.KindTagText, intent, title, action); }
        catch (Exception ex) { Debug.WriteLine($"[LayerDeleteDialog] {ex}"); }
        if (confirmed) vm.ConfirmDangerousCommand(command);
        else vm.CancelDangerousCommand(command);
    }
}
