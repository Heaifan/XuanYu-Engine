using Avalonia.Controls;
using XuanYu.Engine.Editor.Windows.Inspector.TransformEdit;

namespace XuanYu.Engine.Editor.Windows.Panels.Inspector;

/// <summary>Inspector Transform 输入区管理。包含位置/旋转/缩放输入框、校验错误和按钮状态。</summary>
public sealed class InspectorTransformView
{
    readonly TextBox? _px, _py, _pz, _rx, _ry, _rz, _sx, _sy, _sz;
    readonly TextBlock? _error;
    readonly Button? _apply, _reset, _groundPlace;

    public InspectorTransformView(TextBox? px, TextBox? py, TextBox? pz,
        TextBox? rx, TextBox? ry, TextBox? rz,
        TextBox? sx, TextBox? sy, TextBox? sz,
        TextBlock? error, Button? apply, Button? reset, Button? groundPlace)
    { _px = px; _py = py; _pz = pz; _rx = rx; _ry = ry; _rz = rz;
      _sx = sx; _sy = sy; _sz = sz;
      _error = error; _apply = apply; _reset = reset; _groundPlace = groundPlace; }

    public bool _isUpdating;
    public bool IsUpdating => _isUpdating;

    public void SetSnapshot(TransformInspectorSnapshot snap)
    {
        _isUpdating = true;
        try
        {
            SetText(_px, snap.Position.X); SetText(_py, snap.Position.Y); SetText(_pz, snap.Position.Z);
            SetText(_rx, snap.RotationDegrees.X); SetText(_ry, snap.RotationDegrees.Y); SetText(_rz, snap.RotationDegrees.Z);
            SetText(_sx, snap.Scale.X); SetText(_sy, snap.Scale.Y); SetText(_sz, snap.Scale.Z);
        }
        finally { _isUpdating = false; }
        ClearError(); SetTransformDraftState(false, false, null);
    }

    public (string Px, string Py, string Pz, string Rx, string Ry, string Rz, string Sx, string Sy, string Sz) GetAllTexts() =>
        (_px?.Text ?? "", _py?.Text ?? "", _pz?.Text ?? "",
         _rx?.Text ?? "", _ry?.Text ?? "", _rz?.Text ?? "",
         _sx?.Text ?? "", _sy?.Text ?? "", _sz?.Text ?? "");

    static void SetText(TextBox? tb, double v) { if (tb is not null) tb.Text = v.ToString("F3", Culture); }
    static readonly System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

    public void SetTransformDraftState(bool canApply, bool canReset, string? error)
    { if (_apply is not null) _apply.IsEnabled = canApply; if (_reset is not null) _reset.IsEnabled = canReset; if (_error is not null) { _error.IsVisible = !string.IsNullOrWhiteSpace(error); _error.Text = error ?? ""; } }

    public void ClearError() { if (_error is not null) { _error.IsVisible = false; _error.Text = ""; } }
    public void ShowError(string msg) => SetTransformDraftState(false, true, msg);

    public void SetGroundPlaceEnabled(bool enabled) { if (_groundPlace is not null) _groundPlace.IsEnabled = enabled; }
    public void SetPlacementMode(bool isActive)
    { if (_groundPlace is not null) { _groundPlace.Content = isActive ? "放置中… Esc 取消" : "在地面放置"; _groundPlace.IsEnabled = !isActive; } }
}
