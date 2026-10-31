namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    bool _isSceneBusy;

    public string DocumentStatusText =>
        FooterState == "状态：就绪" && IsSceneDirty ? "状态：未保存" : FooterState;

    public string DocumentStatusBackground => DocumentStatusText switch
    {
        "状态：未保存" => "#fff7df",
        "状态：保存失败" or "状态：加载失败" => "#fdeeee",
        "状态：保存成功" or "状态：另存为成功" or "状态：场景加载成功" => "#eef7f1",
        _ => "#eef7f1"
    };

    public string DocumentStatusBorderBrush => DocumentStatusText switch
    {
        "状态：未保存" => "#e7c66d",
        "状态：保存失败" or "状态：加载失败" => "#e2aaaa",
        _ => "#c9e3d0"
    };

    public string DocumentStatusForeground => DocumentStatusText switch
    {
        "状态：未保存" => "#8a6417",
        "状态：保存失败" or "状态：加载失败" => "#a43f3f",
        _ => "#1f7a4d"
    };

    public bool IsSaveButtonHighlighted => IsSceneDirty && !_isSceneBusy;
    public bool CanRunSaveCommand => !_isSceneBusy;
    public string SaveButtonBackground => IsSaveButtonHighlighted ? "#fff6dd" : "Transparent";
    public string SaveButtonBorderBrush => IsSaveButtonHighlighted ? "#d9ad43" : "Transparent";

    void SetFooterState(string value)
    {
        if (Set(ref _footerState, value)) RaiseDocumentStatusChanged();
    }

    void SetSceneBusy(bool busy)
    {
        _isSceneBusy = busy;
        RaiseDocumentStatusChanged();
    }

    void ClearTransientDocumentStatusForDirty()
    {
        if (!IsSceneDirty || !IsTransientSuccessStatus(FooterState)) return;
        _documentStatusToken++;
        FooterState = "状态：就绪";
    }

    async void ShowTemporaryDocumentStatus(string text)
    {
        var token = ++_documentStatusToken;
        FooterState = text;
        RaiseDocumentStatusChanged();
        await Task.Delay(1800);
        if (token != _documentStatusToken || FooterState != text) return;
        FooterState = "状态：就绪";
        RaiseDocumentStatusChanged();
    }

    int _documentStatusToken;

    static bool IsTransientSuccessStatus(string text) =>
        text is "状态：保存成功" or "状态：另存为成功" or "状态：场景加载成功";

    void RaiseDocumentStatusChanged()
    {
        OnPropertyChanged(nameof(DocumentStatusText));
        OnPropertyChanged(nameof(DocumentStatusBackground));
        OnPropertyChanged(nameof(DocumentStatusBorderBrush));
        OnPropertyChanged(nameof(DocumentStatusForeground));
        OnPropertyChanged(nameof(IsSaveButtonHighlighted));
        OnPropertyChanged(nameof(CanRunSaveCommand));
        OnPropertyChanged(nameof(SaveButtonBackground));
        OnPropertyChanged(nameof(SaveButtonBorderBrush));
    }
}
