namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _isDiagnosticMode;

    public bool IsDiagnosticMode
    {
        get => _isDiagnosticMode;
        private set => Set(ref _isDiagnosticMode, value);
    }

    bool TryToggleDiagnosticMode(string name)
    {
        if (name != "诊断模式") return false;
        IsDiagnosticMode = !IsDiagnosticMode;
        return true;
    }
}
