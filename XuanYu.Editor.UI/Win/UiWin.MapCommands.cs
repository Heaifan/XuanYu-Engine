using XuanYu.Editor.UI;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-F1：UiWin 地图命令仅保留快捷键可达的窗口无关命令（新建/聚焦）。
// 面板按钮命令统一走 UiVm.RunCommand → UiVm.MapCommandRouting（真实按钮链）。
// 打开/保存文件选择器在持久化（D6）恢复后回到本层。
public partial class UiWin
{
    async Task RunMapCommand(string command)
    {
        if (DataContext is not UiVm vm) return;
        if (command == "新建地图") { vm.NewMap(); return; }
        if (command == "聚焦地图") { vm.FocusMap(); return; }
        await Task.CompletedTask;
    }
}
