namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _isDiagnosticMode;

    public bool IsDiagnosticMode
    {
        get => _isDiagnosticMode;
        private set => Set(ref _isDiagnosticMode, value);
    }

    bool _isDiagnosticRegionBoundsMode;
    public bool IsDiagnosticRegionBoundsMode
    {
        get => _isDiagnosticRegionBoundsMode;
        private set => Set(ref _isDiagnosticRegionBoundsMode, value);
    }

    bool TryToggleDiagnosticMode(string name)
    {
        if (name != "诊断模式") return false;
        IsDiagnosticMode = !IsDiagnosticMode;
        if (!IsDiagnosticMode)
        {
            IsDiagnosticRegionBoundsMode = false;
        }
        return true;
    }

    bool TryToggleDiagnosticRegionBounds(string name)
    {
        if (name != "区域边界") return false;
        if (!IsDiagnosticMode) return true;
        IsDiagnosticRegionBoundsMode = !IsDiagnosticRegionBoundsMode;
        return true;
    }

}
