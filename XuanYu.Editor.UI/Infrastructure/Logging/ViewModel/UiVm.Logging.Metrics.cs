namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public int LogCollectionRefreshCount => _logPresenter.CollectionRefreshCount;
    public int LogProjectionRefreshCount => _logPresenter.RefreshCount;
    public int LogProjectionRebuildCount => _logPresenter.RebuildCount;
}
