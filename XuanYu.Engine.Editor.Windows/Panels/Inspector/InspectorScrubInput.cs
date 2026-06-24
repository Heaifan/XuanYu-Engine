using Avalonia.Controls;
using Avalonia.Input;
using FluidWarfare.Editor.Transform;
using FluidWarfare.Editor.Transform.Scrub;
using FluidWarfare.Editor.Windows.Panels.Inspector.Transform;

namespace FluidWarfare.Editor.Windows.Panels.Inspector;

/// <summary>Inspector 数值拖拽输入处理。X/Y/Z 标签拖拽微调坐标值。</summary>
public sealed class InspectorScrubInput
{
    readonly TextBlock? _lx, _ly, _lz;
    readonly TransformAxisScrubState _state = new();
    readonly Func<string> _getEntityId;
    readonly Func<TextBlock?, string?> _getText;
    readonly Action<string, TransformPositionAxis, double> _onChanged;
    readonly Action<string, TransformPositionAxis, double> _onCompleted;
    readonly Action<string, TransformPositionAxis, double> _onCancelled;

    public InspectorScrubInput(TextBlock? lx, TextBlock? ly, TextBlock? lz,
        Func<string> getEntityId, Func<TextBlock?, string?> getText,
        Action<string, TransformPositionAxis, double> onChanged,
        Action<string, TransformPositionAxis, double> onCompleted,
        Action<string, TransformPositionAxis, double> onCancelled)
    { _lx = lx; _ly = ly; _lz = lz; _getEntityId = getEntityId; _getText = getText;
      _onChanged = onChanged; _onCompleted = onCompleted; _onCancelled = onCancelled; }

    public void Attach()
    {
        foreach (var lbl in new[] { _lx, _ly, _lz })
        { if (lbl is null) continue; lbl.PointerPressed += OnPressed; lbl.PointerMoved += OnMoved; lbl.PointerReleased += OnReleased; lbl.PointerCaptureLost += OnCaptureLost; }
    }

    static double Sensitivity(KeyModifiers km)
    { if ((km & KeyModifiers.Shift) != 0) return TransformAxisScrubState.FineSensitivity; if ((km & KeyModifiers.Control) != 0) return TransformAxisScrubState.CoarseSensitivity; return TransformAxisScrubState.NormalSensitivity; }

    void OnPressed(object? s, PointerPressedEventArgs e)
    {
        if (s is not TextBlock lbl) return; var eid = _getEntityId(); if (string.IsNullOrEmpty(eid)) return;
        var axis = lbl == _lx ? TransformPositionAxis.X : lbl == _ly ? TransformPositionAxis.Y : TransformPositionAxis.Z;
        var txt = _getText(lbl); if (!double.TryParse(txt, out var v)) return;
        e.Pointer.Capture(lbl); _state.Begin((int)axis, v, e.GetPosition((InputElement)lbl.Parent!).X, Sensitivity(e.KeyModifiers));
    }

    void OnMoved(object? s, PointerEventArgs e)
    { if (!_state.IsScrubbing) return; _state.Update(e.GetPosition((InputElement)((Control)s!).Parent!).X, Sensitivity(e.KeyModifiers)); _onChanged(_getEntityId(), (TransformPositionAxis)_state.Axis, _state.CurrentValue); }

    void OnReleased(object? s, PointerReleasedEventArgs e)
    { if (!_state.IsScrubbing) return; var v = _state.CurrentValue; _state.Complete(); _onCompleted(_getEntityId(), (TransformPositionAxis)_state.Axis, v); }

    void OnCaptureLost(object? s, PointerCaptureLostEventArgs e)
    { if (!_state.IsScrubbing) return; var iv = _state.InitialValue; _state.Cancel(); _onCancelled(_getEntityId(), (TransformPositionAxis)_state.Axis, iv); }
}
