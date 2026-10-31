using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace XuanYu.Editor.UI;

public sealed class DocumentStatusPresenter(Func<bool> isDirty) : INotifyPropertyChanged
{
    string _footerState = "状态：就绪";
    bool _isSceneBusy;
    int _statusToken;

    public event PropertyChangedEventHandler? PropertyChanged;
    public string FooterState => _footerState;
    public bool IsSceneBusy => _isSceneBusy;
    public string DocumentStatusText =>
        FooterState == "状态：就绪" && isDirty() ? "状态：未保存" : FooterState;
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
    public bool IsSaveButtonHighlighted => isDirty() && !_isSceneBusy;
    public bool CanRunSaveCommand => !_isSceneBusy;
    public string SaveButtonBackground => IsSaveButtonHighlighted ? "#fff6dd" : "Transparent";
    public string SaveButtonBorderBrush => IsSaveButtonHighlighted ? "#d9ad43" : "Transparent";

    public void SetFooterState(string value)
    {
        if (_footerState == value) return;
        _footerState = value;
        OnPropertyChanged(nameof(FooterState));
        RefreshBindings();
    }

    public void SetSceneBusy(bool busy) { _isSceneBusy = busy; RefreshBindings(); }

    public void ClearTransientForDirty()
    {
        if (!isDirty() || !IsTransientSuccessStatus(FooterState)) return;
        _statusToken++;
        SetFooterState("状态：就绪");
    }

    public async void ShowTemporary(string text)
    {
        var token = ++_statusToken;
        SetFooterState(text);
        RefreshBindings();
        await Task.Delay(1800);
        if (token != _statusToken || FooterState != text) return;
        SetFooterState("状态：就绪");
        RefreshBindings();
    }

    public void RefreshBindings()
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

    static bool IsTransientSuccessStatus(string text) =>
        text is "状态：保存成功" or "状态：另存为成功" or "状态：场景加载成功";

    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
