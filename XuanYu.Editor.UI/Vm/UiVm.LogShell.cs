namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool IsLogOpen { get => _isLogOpen; set => Set(ref _isLogOpen, value); }
    bool _isLogDetailsOpen;

    public bool IsLogDetailsOpen
    {
        get => _isLogDetailsOpen;
        set => Set(ref _isLogDetailsOpen, value);
    }
}
