using System;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5：危险操作确认流——路由层请求 UI 确认（事件），UI 确认后调用
// ConfirmDangerousCommand 执行。未注入确认处理器时保持原行为（直接执行，兼容既有测试）。
public sealed partial class UiVm
{
    public event Action<string>? DangerousCommandConfirmRequested;

    string? _pendingDangerousCommand;

    public bool IsDangerousCommandPending(string name) => _pendingDangerousCommand == name;

    public void ConfirmDangerousCommand(string name)
    {
        if (_pendingDangerousCommand != name) return;
        _pendingDangerousCommand = null;
        ExecutePendingDangerous(name);
    }

    void RequestDangerousConfirmation(string name)
    {
        _pendingDangerousCommand = name;
        DangerousCommandConfirmRequested?.Invoke(name);
    }

    void ExecutePendingDangerous(string name)
    {
        if (name == "删除图层") DeleteLayer();
    }
}
