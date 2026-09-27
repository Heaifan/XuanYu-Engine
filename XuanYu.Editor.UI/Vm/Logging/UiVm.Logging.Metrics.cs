namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public int LogCollectionRefreshCount { get; private set; }
    public int LogProjectionRefreshCount => _logItems.RefreshCount;
    public int LogProjectionRebuildCount => _logItems.RebuildCount;
}
