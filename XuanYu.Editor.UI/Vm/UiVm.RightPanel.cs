namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    int _rightTabIndex;

    public int RightTabIndex
    {
        get => _rightTabIndex;
        set
        {
            if (!Set(ref _rightTabIndex, value)) return;
            OnPropertyChanged(nameof(IsMapEditorMode));
            UpdateScaleIndicator();
        }
    }

    public bool IsMapEditorMode => RightTabIndex == 1;
}
