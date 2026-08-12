namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool TryRouteLayerDeleteCommand()
    {
        if (SelectedLayer is { } selected && IsDatasetBackedRegionLayer(selected.LayerId))
        {
            if (DangerousCommandConfirmRequested is null)
            {
                _logBus.Error(EditorLogSource.Editor, EditorLogCategory.Command,
                    "解除注册区域数据集已阻止：缺少确认处理器", "区域图层移除必须在用户确认后执行。");
                RefreshLogBindings();
                return true;
            }
            RequestDangerousConfirmation("解除注册数据集");
            return true;
        }
        if (DangerousCommandConfirmRequested is null)
        {
            _logBus.Error(EditorLogSource.Editor, EditorLogCategory.Command,
                "危险操作“删除图层”已阻止：缺少确认处理器", "危险操作必须在用户确认后才可执行。");
            RefreshLogBindings();
            return true;
        }
        if (SelectedLayer is { } layer)
            RequestDangerousConfirmation("删除图层", layer.LayerId);
        return true;
    }
}
