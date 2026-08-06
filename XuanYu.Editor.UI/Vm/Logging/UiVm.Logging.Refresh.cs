namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D5（纠偏）：日志绑定刷新通知（拆分多行，不压缩单行）。
public sealed partial class UiVm
{
    void RefreshLogBindings()
    {
        OnPropertyChanged(nameof(LogItems));
        OnPropertyChanged(nameof(ProblemItems));
        OnPropertyChanged(nameof(BuildItems));
        OnPropertyChanged(nameof(TaskItems));
        OnPropertyChanged(nameof(LogSummary));
        OnPropertyChanged(nameof(HasNoLogItems));
        OnPropertyChanged(nameof(ShowInitialLogEmpty));
        OnPropertyChanged(nameof(ShowNoFilterResults));
        OnPropertyChanged(nameof(SelectedLogEntry));
        OnPropertyChanged(nameof(HasSelectedLogEntry));
        OnPropertyChanged(nameof(SelectedLogClipboardText));
        OnPropertyChanged(nameof(IsLogFilterAll));
        OnPropertyChanged(nameof(IsLogFilterInfo));
        OnPropertyChanged(nameof(IsLogFilterWarning));
        OnPropertyChanged(nameof(IsLogFilterError));
        OnPropertyChanged(nameof(IsLogFilterBuild));
        OnPropertyChanged(nameof(IsLogFilterTask));
        OnPropertyChanged(nameof(IsLogFilterInput));
        OnPropertyChanged(nameof(IsLogFilterRender));
    }
}
