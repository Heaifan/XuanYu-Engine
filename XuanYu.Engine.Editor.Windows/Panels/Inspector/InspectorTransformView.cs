using Avalonia.Controls;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Windows.Shell;

namespace XuanYu.Engine.Editor.Windows.Panels.Inspector;

/// <summary>Inspector Transform 输入区管理。包含坐标输入框、校验错误和按钮状态。</summary>
public sealed class InspectorTransformView
{
    readonly TextBox? _x, _y, _z;
    readonly TextBlock? _error;
    readonly Button? _apply, _reset, _groundPlace;

    public InspectorTransformView(TextBox? x, TextBox? y, TextBox? z, TextBlock? error,
        Button? apply, Button? reset, Button? groundPlace)
    { _x = x; _y = y; _z = z; _error = error; _apply = apply; _reset = reset; _groundPlace = groundPlace; }

    private bool _isUpdating;

    public void SetTexts(string x, string y, string z)
    {
        _isUpdating = true;
        try { if (_x is not null) _x.Text = x; if (_y is not null) _y.Text = y; if (_z is not null) _z.Text = z; }
        finally { _isUpdating = false; }
        ClearError(); SetTransformDraftState(false, false, null);
    }

    public bool IsUpdating => _isUpdating;
    public (string X, string Y, string Z) GetTexts() =>
        (_x?.Text ?? "", _y?.Text ?? "", _z?.Text ?? "");

    public void SetTransformDraftState(bool canApply, bool canReset, string? error)
    { if (_apply is not null) _apply.IsEnabled = canApply; if (_reset is not null) _reset.IsEnabled = canReset; if (_error is not null) { _error.IsVisible = !string.IsNullOrWhiteSpace(error); _error.Text = error ?? ""; } }

    public void ClearError() { if (_error is not null) { _error.IsVisible = false; _error.Text = ""; } }

    public void ShowError(string msg) => SetTransformDraftState(false, true, msg);

    public void SetGroundPlaceEnabled(bool enabled) { if (_groundPlace is not null) _groundPlace.IsEnabled = enabled; }

    public void SetPlacementMode(bool isActive)
    { if (_groundPlace is not null) { _groundPlace.Content = isActive ? "放置中… Esc 取消" : "在地面放置"; _groundPlace.IsEnabled = !isActive; } }

    public void SetSectionVisible(bool visible) { /* handled by parent */ }
}
