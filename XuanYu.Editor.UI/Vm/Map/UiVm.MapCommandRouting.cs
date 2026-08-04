namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-F1：地图面板命令真实路由（UiVm.RunCommand → 地图命令）。
// 纯编辑命令不依赖 UiWin/文件对话框；打开/保存等窗口命令才走 UiWin。
// 本方法在通用兜底（已执行：X）之前返回 true，杜绝"日志有、命令没执行"。
public sealed partial class UiVm
{
    public bool TryRouteMapCommand(string name)
    {
        if (name == "新建地图" || name == "加载地图")
        {
            LogMapCommandReceived(name);
            NewMap();
            return true;
        }

        if (name == "聚焦地图")
        {
            LogMapCommandReceived(name);
            FocusMap();
            return true;
        }

        if (name == "应用地图属性")
        {
            LogMapCommandReceived(name);
            ApplyMapProperties();
            return true;
        }

        if (name == "撤销地图修改")
        {
            LogMapCommandReceived(name);
            MapUndo();
            return true;
        }

        if (name == "重做地图修改")
        {
            LogMapCommandReceived(name);
            MapRedo();
            return true;
        }

        return false;
    }
}
