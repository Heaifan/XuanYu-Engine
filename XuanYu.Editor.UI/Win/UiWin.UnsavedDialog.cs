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
}
