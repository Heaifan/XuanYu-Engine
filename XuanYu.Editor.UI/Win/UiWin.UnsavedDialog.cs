using System.Threading.Tasks;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5：未保存内容确认（弹窗宿主化，替代原代码构建 Window）。
// 保存=默认焦点（Enter）；不保存=危险按钮；取消=Escape。
public partial class UiWin
{
    async Task<bool> ConfirmUnsavedBeforeContinue(UiVm vm)
    {
        if (!vm.IsSceneDirty) return true;
        var choice = await ShowUnsavedDialog();
        if (choice == "cancel") return false;
        if (choice == "discard") return true;
        return await SaveExistingOrPick(vm);
    }

    Task<string> ShowUnsavedDialog() =>
        ShowDialogCore("未保存的场景",
            "当前场景有未保存修改。继续操作前请选择保存、放弃修改或取消。",
            [("保存", false, "save"), ("不保存", true, "discard"), ("取消", false, "cancel")], "save");

    // ARCH-UI-SPEC-R1-D5-FINAL：新建地图未保存弹窗——正式用户文案（不含任何内部治理编号）。
    // 行为合同：默认焦点=取消；Enter 不执行危险操作；Esc/关闭=取消；
    // 仅明确点击「不保存并新建」才允许丢弃修改；不得出现「保存并新建」（见 D5-DEFER-01 登记）。
    Task<string> ShowUnsavedMapChangesDialog() =>
        ShowDialogCore("未保存的地图修改",
            "当前地图有未保存的修改。当前版本暂不支持保存地图后新建。请选择取消，或不保存并新建。",
            [("取消", false, "cancel"), ("不保存并新建", true, "discard")], "cancel");
}
