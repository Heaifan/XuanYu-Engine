using Avalonia.Controls;
using Avalonia.Input;

namespace XuanYu.Engine.Editor.Windows.Panels.Inspector;

/// <summary>Inspector Transform 键盘与按钮事件绑定。Enter/Esc/Apply/Reset/GroundPlace。</summary>
public sealed class InspectorTransformBinder
{
    readonly TextBox? _x, _y, _z;
    readonly Button? _apply, _reset, _groundPlace;
    readonly Func<(string, string, string)> _getTexts;
    readonly Action<string, string, string> _onApply;
    readonly Action _onReset;
    readonly Action _onGroundPlace;

    public InspectorTransformBinder(TextBox? x, TextBox? y, TextBox? z,
        Button? apply, Button? reset, Button? groundPlace,
        Func<(string, string, string)> getTexts,
        Action<string, string, string> onApply,
        Action onReset, Action onGroundPlace)
    { _x = x; _y = y; _z = z; _apply = apply; _reset = reset; _groundPlace = groundPlace;
      _getTexts = getTexts; _onApply = onApply; _onReset = onReset; _onGroundPlace = onGroundPlace; }

    public void Attach()
    {
        foreach (var tb in new[] { _x, _y, _z })
            if (tb is not null) tb.KeyDown += OnKeyDown;
        if (_apply is not null) _apply.Click += (_, _) => { var (x, y, z) = _getTexts(); _onApply(x, y, z); };
        if (_reset is not null) _reset.Click += (_, _) => _onReset();
        if (_groundPlace is not null) _groundPlace.Click += (_, _) => _onGroundPlace();
    }

    void OnKeyDown(object? s, KeyEventArgs e)
    {
        if (e.Key == Key.Enter) { var (x, y, z) = _getTexts(); _onApply(x, y, z); e.Handled = true; }
        else if (e.Key == Key.Escape) { _onReset(); e.Handled = true; }
    }
}
