namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly DocumentStatusPresenter _documentStatusPresenter;
    public DocumentStatusPresenter DocumentStatusPanel => _documentStatusPresenter;

    void SetSceneBusy(bool busy) => _documentStatusPresenter.SetSceneBusy(busy);
    void ClearTransientDocumentStatusForDirty() => _documentStatusPresenter.ClearTransientForDirty();
    void ShowTemporaryDocumentStatus(string text) => _documentStatusPresenter.ShowTemporary(text);
    void RaiseDocumentStatusChanged() => _documentStatusPresenter.RefreshBindings();
}
