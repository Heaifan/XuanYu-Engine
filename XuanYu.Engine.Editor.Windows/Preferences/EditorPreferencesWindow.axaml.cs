using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using XuanYu.Engine.Editor.Input.Bindings;

using XuanYu.Engine.Editor.Windows.Preferences;
using XuanYu.Engine.Editor.Windows.Preferences;
namespace XuanYu.Engine.Editor.Windows.Preferences;

public sealed partial class EditorPreferencesWindow : Window
{
    readonly EditorPreferencesDraftHandler _draft = new();
    readonly EditorPreferencesCapture _capture = new();
    readonly EditorPreferencesBindingList _bindingList;

    public EditorPreferencesWindow()
    {
        InitializeComponent();
        Title = "偏好设置";
        _draft.LoadFromService();
        EditorPreferencesBindingList? list = null;
        list = new EditorPreferencesBindingList(_draft, BindingsContainer!, (id, slot) =>
        {
            var btn = list!.CreateBindingButton(id, slot);
            btn.Click += OnBindingButtonClicked;
            return btn;
        });
        _bindingList = list;
        _bindingList.RestoreClicked += id => { _draft.RemoveOverrides(id); Refresh(); };
    }

    void OnOpened(object? sender, EventArgs e) => Refresh();
    void Refresh() { _bindingList?.Repopulate(SearchBox?.Text); UpdateButtons(); }

    void OnBindingButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button btn || btn.Tag is not (string a, string s)) return;
        _capture.Begin(a, s, btn);
    }

    void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (!_capture.IsActive) return;
        var other = _draft.GetEffective(_capture.ActionId, _capture.Slot == "primary" ? "secondary" : "primary");
        if (!_capture.HandleKey(e, other)) return;
        if (_capture.WasCleared) { _draft.SetOverride(_capture.ActionId, _capture.Slot, null); Refresh(); }
        else if (_capture.WasCancelled) { Refresh(); }
        else if (_capture.HasAcceptedConflict) { _draft.SetOverride(_capture.ConflictActionId!, _capture.ConflictSlot!, null); _draft.SetOverride(_capture.ActionId, _capture.Slot, _capture.PendingGesture); _capture.ApplyAndReset(); Refresh(); }
        else if (_capture.HasPendingConflict) { _capture.SignalConflict(_capture.ConflictActionId!, _capture.ConflictSlot!); }
        else if (_capture.HasPendingApply) { _draft.SetOverride(_capture.ActionId, _capture.Slot, _capture.PendingGesture); _capture.ApplyAndReset(); Refresh(); }
    }

    void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (!_capture.IsActive) return;
        var other = _draft.GetEffective(_capture.ActionId, _capture.Slot == "primary" ? "secondary" : "primary");
        if (_capture.HandlePointer(e, other) && _capture.HasPendingApply) { _draft.SetOverride(_capture.ActionId, _capture.Slot, _capture.PendingGesture!); _capture.ApplyAndReset(); Refresh(); }
    }

    void OnWindowPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (!_capture.IsActive) return;
        var other = _draft.GetEffective(_capture.ActionId, _capture.Slot == "primary" ? "secondary" : "primary");
        if (_capture.HandleWheel(e, other) && _capture.HasPendingApply) { _draft.SetOverride(_capture.ActionId, _capture.Slot, _capture.PendingGesture!); _capture.ApplyAndReset(); Refresh(); }
    }

    void OnSearchTextChanged(object? sender, TextChangedEventArgs e) => Refresh();
    void OnRestoreSingleClicked(object? sender, RoutedEventArgs e) { if (sender is Button btn && btn.Tag is string id) { _draft.RemoveOverrides(id); Refresh(); } }
    void OnRestoreAllClicked(object? sender, RoutedEventArgs e) { _draft.ClearAll(); Refresh(); }
    void OnCancelClicked(object? sender, RoutedEventArgs e) => Close();
    void OnApplyClicked(object? sender, RoutedEventArgs e) { _draft.Flush(false, ShowError, UpdateButtons, () => { }); }
    void OnSaveClicked(object? sender, RoutedEventArgs e) { _draft.Flush(true, ShowError, UpdateButtons, Close); }

    void UpdateButtons()
    {
        var h = _draft.HasAnyChanges();
        if (ApplyButton is not null) ApplyButton.IsEnabled = h;
        if (SaveButton is not null) SaveButton.IsEnabled = h;
    }

    void ShowError(string msg) => Title = $"⚠ {msg}";
}
