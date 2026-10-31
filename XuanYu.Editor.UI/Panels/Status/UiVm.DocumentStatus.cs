namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    readonly DocumentStatusPresenter _documentStatusPresenter;

    public string DocumentStatusText => _documentStatusPresenter.DocumentStatusText;
    public string DocumentStatusBackground => _documentStatusPresenter.DocumentStatusBackground;
    public string DocumentStatusBorderBrush => _documentStatusPresenter.DocumentStatusBorderBrush;
    public string DocumentStatusForeground => _documentStatusPresenter.DocumentStatusForeground;
    public bool IsSaveButtonHighlighted => _documentStatusPresenter.IsSaveButtonHighlighted;
    public bool CanRunSaveCommand => _documentStatusPresenter.CanRunSaveCommand;
    public string SaveButtonBackground => _documentStatusPresenter.SaveButtonBackground;
    public string SaveButtonBorderBrush => _documentStatusPresenter.SaveButtonBorderBrush;

    void SetSceneBusy(bool busy) => _documentStatusPresenter.SetSceneBusy(busy);
    void ClearTransientDocumentStatusForDirty() => _documentStatusPresenter.ClearTransientForDirty();
    void ShowTemporaryDocumentStatus(string text) => _documentStatusPresenter.ShowTemporary(text);
    void RaiseDocumentStatusChanged() => _documentStatusPresenter.RefreshBindings();
}
