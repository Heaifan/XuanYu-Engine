using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using FluidWarfare.Editor.Input.Bindings;
using AM = Avalonia.Media;

namespace FluidWarfare.Editor.Windows.Preferences;

sealed class EditorPreferencesCapture
{
    enum State { Idle, Waiting, Conflict }
    State _state;
    string _actionId = "";
    string _slot = "primary";
    EditorInputGesture? _pendingGesture;
    Button? _button;
    public string? ConflictActionId, ConflictSlot;
    public EditorInputGesture? PendingGesture => _pendingGesture;
    public bool WasCancelled { get; private set; }
    public bool WasCleared { get; private set; }
    public bool HasPendingConflict => _state == State.Conflict && _pendingGesture is not null && ConflictActionId is not null;
    public bool HasPendingApply => _state == State.Idle && _pendingGesture is not null && ConflictActionId is null;
    public bool HasAcceptedConflict => _state == State.Idle && _pendingGesture is not null && ConflictActionId is not null;
    public bool IsActive => _state != State.Idle;
    public string ActionId => _actionId;
    public string Slot => _slot;

    public void Begin(string a, string s, Button b) { Cancel(); _state = State.Waiting; _actionId = a; _slot = s; _button = b; SetBtn("按下按键或鼠标…", AM.Color.Parse("#FFF"), AM.Color.Parse("#5A3F3F")); }
    public void Cancel() { if (_state == State.Idle) return; _state = State.Idle; _actionId = ""; _pendingGesture = null; ConflictActionId = ConflictSlot = null; _button = null; }
    public void ApplyAndReset() { _state = State.Idle; _button = null; }
    public void SignalConflict(string cId, string cSlot) { _state = State.Conflict; ConflictActionId = cId; ConflictSlot = cSlot; SetBtn($"冲突：{cId} → 替换？", AM.Color.Parse("#FF6B6B"), null); }

    public bool HandleKey(KeyEventArgs e, EditorInputGesture? other)
    {
        WasCancelled = WasCleared = false;
        if (_state == State.Idle) return false;
        if (_state == State.Conflict) { e.Handled = true; if (e.Key == Key.Escape) { Cancel(); WasCancelled = true; return true; } if (e.Key == Key.Enter) { _state = State.Idle; return true; } return true; }
        e.Handled = true;
        if (e.Key == Key.Escape) { Cancel(); WasCancelled = true; return true; }
        if (e.Key is Key.Back or Key.Delete) { _state = State.Idle; _button = null; WasCleared = true; return true; }
        var code = EditorPreferencesKeyMapper.KeyToCode(e.Key);
        if (code is null) return false;
        return TryComplete(new EditorInputGesture(EditorInputDevice.Keyboard, code, EditorPreferencesKeyMapper.ToMods(e.KeyModifiers)), other);
    }

    public bool HandlePointer(PointerPressedEventArgs e, EditorInputGesture? other)
    {
        WasCancelled = WasCleared = false;
        if (_state != State.Waiting) return false;
        var p = e.GetCurrentPoint(null).Properties;
        string? code = null;
        if (p.IsLeftButtonPressed) code = "Left";
        else if (p.IsMiddleButtonPressed) code = "Middle";
        else if (p.IsRightButtonPressed) code = "Right";
        else if (p.IsXButton1Pressed) code = "X1";
        else if (p.IsXButton2Pressed) code = "X2";
        if (code is null) return false;
        e.Handled = true;
        if (code is "Left" or "Right")
        { SetBtn("左/右键已预留，请用组合键或中键", AM.Color.Parse("#FF6B6B"), null); return false; }
        if (EditorInputConflictDetector.IsReservedGesture(new EditorInputGesture(EditorInputDevice.Mouse, code, EditorPreferencesKeyMapper.ToMods(e.KeyModifiers), EditorInputGestureKind.MouseDrag))) return false;
        return TryComplete(new EditorInputGesture(EditorInputDevice.Mouse, code, EditorPreferencesKeyMapper.ToMods(e.KeyModifiers), EditorInputGestureKind.MouseDrag), other);
    }

    public bool HandleWheel(PointerWheelEventArgs e, EditorInputGesture? other)
    { WasCancelled = WasCleared = false; if (_state != State.Waiting) return false; e.Handled = true; return TryComplete(new EditorInputGesture(EditorInputDevice.Wheel, "Y", EditorPreferencesKeyMapper.ToMods(e.KeyModifiers), EditorInputGestureKind.MouseWheel), other); }

    bool TryComplete(EditorInputGesture g, EditorInputGesture? other)
    {
        if (other is not null && other.Signature == g.Signature)
        { SetBtn("主/备用绑定不能相同", AM.Color.Parse("#FF6B6B"), null); return false; }
        _pendingGesture = g; ConflictActionId = ConflictSlot = null; _state = State.Idle; return true;
    }

    void SetBtn(string text, in AM.Color? fg, in AM.Color? bg)
    { if (_button is null) return; _button.Content = text; if (fg.HasValue) _button.Foreground = new SolidColorBrush(fg.Value); if (bg.HasValue) _button.Background = new SolidColorBrush(bg.Value); }
}
