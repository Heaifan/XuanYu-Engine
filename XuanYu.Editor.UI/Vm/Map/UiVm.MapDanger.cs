using System;
using XuanYu.World.Map;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（纠偏）：危险操作确认流——**fail-closed**。
// 确认处理器缺失 / 用户未确认 / 未响应 → 一律不执行（并记录错误）；
// 只有用户明确确认（ConfirmDangerousCommand）才执行实际操作。
public sealed partial class UiVm
{
    public event Action<string>? DangerousCommandConfirmRequested;

    string? _pendingDangerousCommand;
    MapLayerId? _pendingDangerousLayerId;

    public bool IsDangerousCommandPending(string name) => _pendingDangerousCommand == name;

    public void ConfirmDangerousCommand(string name)
    {
        if (_pendingDangerousCommand != name) return;
        var layerId = _pendingDangerousLayerId;
        _pendingDangerousCommand = null;
        _pendingDangerousLayerId = null;
        ExecutePendingDangerous(name, layerId);
    }

    public void CancelDangerousCommand(string name)
    {
        if (_pendingDangerousCommand != name) return;
        _pendingDangerousCommand = null;
        _pendingDangerousLayerId = null;
        _logBus.Info(EditorLogSource.Editor, EditorLogCategory.Command,
            $"危险操作「{name}」已取消", "用户取消确认，操作未执行。");
        RefreshLogBindings();
    }

    void RequestDangerousConfirmation(string name, MapLayerId? layerId = null)
    {
        _pendingDangerousCommand = name;
        _pendingDangerousLayerId = layerId;
        DangerousCommandConfirmRequested?.Invoke(name);
    }

    void ExecutePendingDangerous(string name, MapLayerId? layerId)
    {
        if (name == "删除图层" && layerId is { } id) DeleteLayer(id);
        if (name == "解除注册数据集" && layerId is { } datasetLayerId &&
            TryGetDatasetIdForLayer(datasetLayerId, out var datasetId))
            _ = UnregisterDatasetAsync(datasetId);
    }
}
