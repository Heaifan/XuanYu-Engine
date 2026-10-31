namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
void RefreshLogSelectionBindings()
    {
        OnPropertyChanged(nameof(SelectedLogEntry));
        OnPropertyChanged(nameof(HasSelectedLogEntry));
        OnPropertyChanged(nameof(SelectedLogClipboardText));
    }
}
