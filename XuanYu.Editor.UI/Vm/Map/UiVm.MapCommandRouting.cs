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
            // D5：提交反馈通知（成功/失败；失败详情同时进入日志与表单错误区）
            if (string.IsNullOrEmpty(MapEditError)) NotifySuccess("地图属性已应用");
            else NotifyError(MapEditError);
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

        if (name == "添加图层")
        {
            LogMapCommandReceived(name);
            AddLayer();
            return true;
        }

        if (name == "上移图层")
        {
            LogMapCommandReceived(name);
            MoveLayerUp();
            return true;
        }

        if (name == "下移图层")
        {
            LogMapCommandReceived(name);
            MoveLayerDown();
            return true;
        }

        if (name == "删除图层")
        {
            LogMapCommandReceived(name);
            // D5：危险操作先请求 UI 确认（未注入处理器时直接执行，兼容既有测试）
            if (DangerousCommandConfirmRequested is not null) RequestDangerousConfirmation(name);
            else DeleteLayer();
            return true;
        }

        if (name == "设为当前图层")
        {
            LogMapCommandReceived(name);
            SetActiveLayer();
            return true;
        }

        return false;
    }
}
