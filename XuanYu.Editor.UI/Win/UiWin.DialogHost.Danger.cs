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
        if (name == "删除图层")
        {
            await ConfirmLayerDeleteAsync(_attachedVm);
            return;
        }
        var (message, action) = name switch
        {
            "解除注册数据集" => ("确定从当前地图移除区域图层吗？对应 Dataset 文件将保留，不会从磁盘删除。", "移除区域图层"),
            _ => ($"执行「{name}」将丢弃相关修改且不可撤销。是否继续？", "继续")
        };
        if (await ShowDanger("危险操作", message, action) == "ok")
            _attachedVm.ConfirmDangerousCommand(name);
        else
            _attachedVm.CancelDangerousCommand(name);
    }

    async Task ConfirmLayerDeleteAsync(UiVm vm)
    {
        var layer = vm.SelectedLayer;
        if (layer is null) { vm.CancelDangerousCommand("删除图层"); return; }
        var confirmed = false;
        try
        {
            confirmed = await LayerDeleteConfirmationWindow.ShowAsync(this, layer.Name,
                layer.KindTagText, LayerDeleteConfirmationIntent);
        }
        catch (Exception ex) { Debug.WriteLine($"[LayerDeleteDialog] {ex}"); }
        if (confirmed) vm.ConfirmDangerousCommand("删除图层");
        else vm.CancelDangerousCommand("删除图层");
    }
}
