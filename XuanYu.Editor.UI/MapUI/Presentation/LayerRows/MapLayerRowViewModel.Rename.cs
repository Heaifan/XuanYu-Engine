namespace XuanYu.Editor.UI;

public sealed partial class MapLayerRowViewModel
{
    bool _isRenaming;
    string _renameText = "";

    public bool CanRename => IsRegion && !IsLocked;
    public bool IsRenaming { get => _isRenaming; private set => Set(ref _isRenaming, value); }
    public string RenameText { get => _renameText; set => Set(ref _renameText, value); }

    public void BeginRename()
    {
        if (!CanRename) return;
        RenameText = Name;
        IsRenaming = true;
    }

    public void CancelRename() => IsRenaming = false;
    public void CompleteRename() => IsRenaming = false;
}
