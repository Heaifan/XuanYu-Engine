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
        OnPropertyChanged(nameof(AllCount));
        OnPropertyChanged(nameof(InfoCount));
        OnPropertyChanged(nameof(WarningCount));
        OnPropertyChanged(nameof(ErrorCount));
        OnPropertyChanged(nameof(LogSourceFilter));
        OnPropertyChanged(nameof(LogSourceFilterText));
        OnPropertyChanged(nameof(LogSearchText));
        OnPropertyChanged(nameof(IsLogDetailsOpen));
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
    }
}
