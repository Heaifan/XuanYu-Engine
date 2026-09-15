namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _isDiagnosticMode;

    public bool IsDiagnosticMode
    {
        get => _isDiagnosticMode;
        private set => Set(ref _isDiagnosticMode, value);
    }

    bool _isDiagnosticProbeMode;
    public bool IsDiagnosticProbeMode
    {
        get => _isDiagnosticProbeMode;
        private set => Set(ref _isDiagnosticProbeMode, value);
    }

    bool TryToggleDiagnosticMode(string name)
    {
        if (name != "诊断模式") return false;
        IsDiagnosticMode = !IsDiagnosticMode;
        return true;
    }

    bool TryToggleDiagnosticProbe(string name)
    {
        if (name != "元素拾取") return false;
        if (!IsDiagnosticMode) return true;
        IsDiagnosticProbeMode = !IsDiagnosticProbeMode;
        return true;
    }

    public void ExitDiagnosticProbe() => IsDiagnosticProbeMode = false;
}
