using Avalonia.Platform.Storage;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3：地图命令分发（唯一数据源 = MapSession）。
// 打开/保存为 v1 DTO 旧链，D3 起按钮禁用防双权威分叉（持久化 D6 接入后恢复）。
public partial class UiWin
{
    async Task RunMapCommand(string command)
    {
        if (DataContext is not UiVm vm) return;
        if (command == "新建地图") { vm.NewMap(); return; }
        if (command == "聚焦地图") { vm.FocusMap(); return; }
        if (command == "应用地图属性") { vm.ApplyMapProperties(); return; }
    }
}
