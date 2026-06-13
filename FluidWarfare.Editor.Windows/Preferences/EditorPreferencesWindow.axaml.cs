using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Bindings;
using FluidWarfare.Editor.Input.Runtime;
using FluidWarfare.Editor.Input.Settings;
using AM = Avalonia.Media;

namespace FluidWarfare.Editor.Windows.Preferences;

public sealed partial class EditorPreferencesWindow : Window
{
    // ─── 捕获状态机 ──────────────────────────────────────

    private enum CaptureState { Idle, WaitingForInput, ConflictConfirmation }

    private CaptureState _captureState = CaptureState.Idle;
    private string _captureActionId = string.Empty;
    private string _captureSlot = "primary";
    private EditorInputGesture? _capturedGesture;
    private Button? _captureButton;
    private string? _conflictActionId;
    private string? _conflictSlot;

    // ─── 草稿模型 ────────────────────────────────────────
    // _draftBindingSet: 当前窗口编辑的完整绑定集（已保存 + 本次改动）
    // _originalBindingSet: 打开窗口时的快照（用于判断是否有未保存改动）

    private EditorInputBindingSet _originalBindingSet = null!;
    private EditorInputBindingSet _draftBindingSet = null!;

    public EditorPreferencesWindow()
    {
        InitializeComponent();
        Title = "偏好设置";
        LoadDraftFromService();
    }

    private void LoadDraftFromService()
    {
        var service = EditorInputService.Instance;
        _originalBindingSet = service.GetCurrentBindingSet();
        _draftBindingSet = _originalBindingSet with
        {
            Overrides = _originalBindingSet.Overrides.ToArray()
        };
    }

    private void OnOpened(object? sender, EventArgs e)
    {
        PopulateBindings();
    }

    // ─── 绑定列表生成 ─────────────────────────────────────

    private void PopulateBindings(string? searchFilter = null)
    {
        if (BindingsContainer is null) return;
        BindingsContainer.Children.Clear();

        var categories = new[] { "全局", "视口导航", "标准视图", "Transform", "工具" };
        var actions = EditorInputActionCatalog.All.Where(a => a.IsUserConfigurable);

        if (!string.IsNullOrWhiteSpace(searchFilter))
        {
            var filter = searchFilter.Trim().ToLowerInvariant();
            actions = actions.Where(a =>
                a.DisplayName.ToLowerInvariant().Contains(filter) ||
                a.Id.ToLowerInvariant().Contains(filter));
        }

        foreach (var cat in categories)
        {
            var catActions = actions.Where(a => a.Category == cat).ToList();
            if (catActions.Count == 0) continue;

            BindingsContainer.Children.Add(new TextBlock
            {
                Text = cat,
                Foreground = new SolidColorBrush(AM.Color.Parse("#999")),
                Margin = new Thickness(0, 10, 0, 2),
                FontSize = 13,
                FontWeight = FontWeight.Bold
            });

            foreach (var def in catActions)
                BindingsContainer.Children.Add(CreateBindingRow(def));
        }

        UpdateButtonStates();
    }

    private Border CreateBindingRow(EditorInputActionDefinition def)
    {
        var row = new Border
        {
            Background = new SolidColorBrush(AM.Color.Parse("#2A2F36")),
            BorderBrush = new SolidColorBrush(AM.Color.Parse("#414852")),
            BorderThickness = new Thickness(0, 0, 0, 1),
            Padding = new Thickness(8, 4),
            Margin = new Thickness(0, 1)
        };

        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("160,Auto,8,Auto,8,Auto") };

        grid.Children.Add(new TextBlock
        {
            Text = def.DisplayName,
            VerticalAlignment = VerticalAlignment.Center,
            Foreground = new SolidColorBrush(AM.Color.Parse("#DDD")),
            FontSize = 12
        });
        Grid.SetColumn(grid.Children[^1], 0);

        var primaryBtn = CreateBindingButton(def.Id, "primary", true);
        Grid.SetColumn(primaryBtn, 1);
        grid.Children.Add(primaryBtn);

        var secondaryBtn = CreateBindingButton(def.Id, "secondary", false);
        Grid.SetColumn(secondaryBtn, 3);
        grid.Children.Add(secondaryBtn);

        var restoreBtn = new Button
        {
            Content = "恢复",
            Tag = def.Id,
            Padding = new Thickness(6, 2),
            Background = new SolidColorBrush(Colors.Transparent),
            BorderThickness = new Thickness(0),
            Foreground = new SolidColorBrush(AM.Color.Parse("#888")),
            FontSize = 11,
            IsVisible = HasEffectiveOverride(def.Id)
        };
        restoreBtn.Click += OnRestoreSingleClicked;
        Grid.SetColumn(restoreBtn, 5);
        grid.Children.Add(restoreBtn);

        row.Child = grid;
        return row;
    }

    private Button CreateBindingButton(string actionId, string slot, bool isPrimary)
    {
        var gesture = GetEffectiveGesture(actionId, slot);
        var btn = new Button
        {
            Content = FormatGestureText(gesture),
            Tag = (actionId, slot),
            MinWidth = isPrimary ? 130 : 110,
            Padding = new Thickness(8, 3),
            Background = new SolidColorBrush(AM.Color.Parse("#353B44")),
            BorderThickness = new Thickness(1),
            BorderBrush = new SolidColorBrush(AM.Color.Parse("#555")),
            Foreground = new SolidColorBrush(
                gesture is null ? AM.Color.Parse("#666") : AM.Color.Parse("#CCC")),
            FontSize = 11
        };
        btn.Click += OnBindingButtonClicked;
        return btn;
    }

    // ─── 手势文本格式化 ───────────────────────────────────

    private static string FormatGestureText(EditorInputGesture? gesture)
    {
        if (gesture is null) return "未绑定";

        var mods = gesture.Modifiers;
        var modText = mods switch
        {
            EditorInputModifiers.None => "",
            EditorInputModifiers.Shift => "Shift+",
            EditorInputModifiers.Control => "Ctrl+",
            EditorInputModifiers.Alt => "Alt+",
            EditorInputModifiers.Shift | EditorInputModifiers.Control => "Ctrl+Shift+",
            EditorInputModifiers.Shift | EditorInputModifiers.Alt => "Alt+Shift+",
            EditorInputModifiers.Control | EditorInputModifiers.Alt => "Alt+Ctrl+",
            EditorInputModifiers.Shift | EditorInputModifiers.Control | EditorInputModifiers.Alt => "Alt+Ctrl+Shift+",
            _ => ""
        };

        var keyText = gesture.Device switch
        {
            EditorInputDevice.Keyboard => gesture.Code switch
            {
                "Escape" => "Esc", "Space" => "空格", "Enter" => "回车",
                "Back" => "退格", "Delete" => "Del", "Insert" => "Ins",
                "PageUp" => "PgUp", "PageDown" => "PgDn",
                "Left" => "←", "Right" => "→", "Up" => "↑", "Down" => "↓",
                "Decimal" => "小键盘.", "Comma" => ",", "Period" => ".",
                "Slash" => "/", "Semicolon" => ";", "Quote" => "'",
                "Minus" => "-", "Equals" => "=", "Backtick" => "`",
                "Backslash" => "\\", "LeftBracket" => "[", "RightBracket" => "]",
                _ => gesture.Code
            },
            EditorInputDevice.Mouse => gesture.Code switch
            {
                "Left" => "左键", "Right" => "右键", "Middle" => "中键",
                "X1" => "侧键1", "X2" => "侧键2",
                _ => gesture.Code
            },
            EditorInputDevice.Wheel => "滚轮",
            _ => gesture.Code
        };

        var kindText = gesture.Kind switch
        {
            EditorInputGestureKind.MouseDrag => "拖动",
            EditorInputGestureKind.MouseWheel => "",
            _ => ""
        };

        return $"{modText}{keyText}{kindText}";
    }

    // ─── 草稿查询 ─────────────────────────────────────────
    // _draftBindingSet.Overrides 是唯一的数据源。

    private EditorInputGesture? GetEffectiveGesture(string actionId, string slot)
    {
        var ov = _draftBindingSet.Overrides
            .FirstOrDefault(o => o.ActionId == actionId && o.Slot == slot);
        if (ov is not null)
            return ov.Gesture; // null = cleared
        return GetBlenderDefaultGesture(actionId, slot);
    }

    private static EditorInputGesture? GetBlenderDefaultGesture(string actionId, string slot)
    {
        var blenderBindings = EditorInputActionCatalog.BlenderDefaultBindings;
        var binding = blenderBindings.FirstOrDefault(b => b.ActionId == actionId);
        if (binding is null) return null;
        return slot == "primary" ? binding.PrimaryGesture : binding.SecondaryGesture;
    }

    private bool HasEffectiveOverride(string actionId)
    {
        return _draftBindingSet.Overrides.Any(o => o.ActionId == actionId);
    }

    private bool HasAnyChanges()
        => !_draftBindingSet.HasSameEffectiveBindingsAs(_originalBindingSet);

    // ─── 草稿修改方法 ─────────────────────────────────────

    private void SetDraftOverride(string actionId, string slot, EditorInputGesture? gesture)
    {
        var list = _draftBindingSet.Overrides.ToList();
        list.RemoveAll(o => o.ActionId == actionId && o.Slot == slot);
        list.Add(new EditorInputBindingOverride
        {
            ActionId = actionId, Slot = slot, Gesture = gesture
        });
        _draftBindingSet = _draftBindingSet with { Overrides = list.ToArray() };
    }

    private void RemoveDraftOverrides(string actionId)
    {
        _draftBindingSet = _draftBindingSet with
        {
            Overrides = _draftBindingSet.Overrides
                .Where(o => o.ActionId != actionId).ToArray()
        };
    }

    private void ClearAllDraftOverrides()
    {
        _draftBindingSet = _draftBindingSet with
        {
            Overrides = Array.Empty<EditorInputBindingOverride>()
        };
    }

    // ─── 绑定按钮点击 → 进入捕获 ─────────────────────────

    private void OnBindingButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.Tag is not (string actionId, string slot)) return;
        BeginCapture(actionId, slot, btn);
    }

    private void BeginCapture(string actionId, string slot, Button button)
    {
        CancelCapture();
        _captureState = CaptureState.WaitingForInput;
        _captureActionId = actionId;
        _captureSlot = slot;
        _captureButton = button;
        _captureButton.Content = "按下按键或鼠标…";
        _captureButton.Background = new SolidColorBrush(AM.Color.Parse("#5A3F3F"));
        _captureButton.Foreground = new SolidColorBrush(AM.Color.Parse("#FFF"));
        Focus();
    }

    private void CancelCapture()
    {
        if (_captureState == CaptureState.Idle) return;
        _captureState = CaptureState.Idle;
        var btn = _captureButton;
        var aId = _captureActionId;
        var slot = _captureSlot;
        _captureActionId = string.Empty;
        _captureSlot = "primary";
        _capturedGesture = null;
        _conflictActionId = null;
        _conflictSlot = null;
        _captureButton = null;
        if (btn is not null && !string.IsNullOrEmpty(aId))
        {
            btn.Background = new SolidColorBrush(AM.Color.Parse("#353B44"));
            btn.Foreground = new SolidColorBrush(AM.Color.Parse("#CCC"));
            btn.Content = FormatGestureText(GetEffectiveGesture(aId, slot));
        }
    }

    private void CompleteCapture(EditorInputGesture gesture)
    {
        if (string.IsNullOrEmpty(_captureActionId)) return;
        var otherSlot = _captureSlot == "primary" ? "secondary" : "primary";
        var otherGesture = GetEffectiveGesture(_captureActionId, otherSlot);
        if (otherGesture is not null && otherGesture.Signature == gesture.Signature)
        {
            if (_captureButton is not null)
            {
                _captureButton.Content = "主/备用绑定不能相同";
                _captureButton.Foreground = new SolidColorBrush(AM.Color.Parse("#FF6B6B"));
            }
            return;
        }
        if (DetectConflict(_captureActionId, gesture, out var conflictActionId, out var conflictSlot))
        {
            _capturedGesture = gesture;
            _conflictActionId = conflictActionId;
            _conflictSlot = conflictSlot;
            _captureState = CaptureState.ConflictConfirmation;
            if (_captureButton is not null)
            {
                _captureButton.Content = $"冲突：{conflictActionId} → 替换？";
                _captureButton.Foreground = new SolidColorBrush(AM.Color.Parse("#FF6B6B"));
            }
            return;
        }
        ApplyCapture(gesture);
    }

    private void ApplyCapture(EditorInputGesture gesture)
    {
        if (string.IsNullOrEmpty(_captureActionId)) return;
        SetDraftOverride(_captureActionId, _captureSlot, gesture);
        UpdateButtonStates();
        PopulateBindings();
        _captureState = CaptureState.Idle;
        _captureButton = null;
    }

    private void ClearBinding(string actionId, string slot)
    {
        SetDraftOverride(actionId, slot, null);
        UpdateButtonStates();
        PopulateBindings();
    }

    // ─── 冲突检测 ─────────────────────────────────────────

    private bool DetectConflict(string actionId, EditorInputGesture gesture,
        out string? conflictActionId, out string? conflictSlot)
    {
        conflictActionId = null;
        conflictSlot = null;
        return EditorInputConflictDetector.DetectConflict(
            BuildEffectiveBindingList(), actionId, gesture,
            out conflictActionId, out conflictSlot);
    }

    private List<EditorInputBinding> BuildEffectiveBindingList()
    {
        var blenderBindings = EditorInputActionCatalog.BlenderDefaultBindings;
        var result = new List<EditorInputBinding>();
        foreach (var bb in blenderBindings)
        {
            var aId = bb.ActionId;
            var pOv = _draftBindingSet.Overrides
                .FirstOrDefault(o => o.ActionId == aId && o.Slot == "primary");
            var sOv = _draftBindingSet.Overrides
                .FirstOrDefault(o => o.ActionId == aId && o.Slot == "secondary");
            result.Add(bb with
            {
                PrimaryGesture = pOv is not null ? pOv.Gesture : bb.PrimaryGesture,
                SecondaryGesture = sOv is not null ? sOv.Gesture : bb.SecondaryGesture
            });
        }
        return result;
    }

    // ─── 窗口级输入捕获 ───────────────────────────────────

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (_captureState != CaptureState.WaitingForInput &&
            _captureState != CaptureState.ConflictConfirmation)
            return;
        e.Handled = true;

        if (_captureState == CaptureState.ConflictConfirmation)
        {
            if (e.Key == Key.Escape) { CancelCapture(); return; }
            if (e.Key == Key.Enter)
            {
                if (_capturedGesture is not null && _conflictActionId is not null && _conflictSlot is not null)
                    SetDraftOverride(_conflictActionId, _conflictSlot, null);
                ApplyCapture(_capturedGesture!);
                return;
            }
            return;
        }

        if (e.Key == Key.Escape) { CancelCapture(); return; }
        if (e.Key == Key.Back || e.Key == Key.Delete)
        {
            ClearBinding(_captureActionId, _captureSlot);
            _captureState = CaptureState.Idle;
            return;
        }

        var mods = GetModifiersFromAvalonia(e.KeyModifiers);
        var code = AvaloniaKeyToCode(e.Key);
        if (code is null) return;
        CompleteCapture(new EditorInputGesture(EditorInputDevice.Keyboard, code, mods));
    }

    private void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_captureState != CaptureState.WaitingForInput) return;
        var point = e.GetCurrentPoint(this);
        var props = point.Properties;
        string? btnCode = null;
        if (props.IsLeftButtonPressed) btnCode = "Left";
        else if (props.IsMiddleButtonPressed) btnCode = "Middle";
        else if (props.IsRightButtonPressed) btnCode = "Right";
        else if (props.IsXButton1Pressed) btnCode = "X1";
        else if (props.IsXButton2Pressed) btnCode = "X2";
        if (btnCode is null) return;
        e.Handled = true;
        if (btnCode == "Left" || btnCode == "Right")
        {
            _captureButton!.Content = "左/右键已预留，请用组合键或中键";
            _captureButton.Foreground = new SolidColorBrush(AM.Color.Parse("#FF6B6B"));
            return;
        }
        var mods = GetModifiersFromAvalonia(e.KeyModifiers);
        var gesture = new EditorInputGesture(
            EditorInputDevice.Mouse, btnCode, mods, EditorInputGestureKind.MouseDrag);
        if (!EditorInputConflictDetector.IsReservedGesture(gesture))
            CompleteCapture(gesture);
    }

    private void OnWindowPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (_captureState != CaptureState.WaitingForInput) return;
        e.Handled = true;
        var mods = GetModifiersFromAvalonia(e.KeyModifiers);
        CompleteCapture(new EditorInputGesture(
            EditorInputDevice.Wheel, "Y", mods, EditorInputGestureKind.MouseWheel));
    }

    private static EditorInputModifiers GetModifiersFromAvalonia(KeyModifiers km)
    {
        var result = EditorInputModifiers.None;
        if ((km & KeyModifiers.Shift) != 0) result |= EditorInputModifiers.Shift;
        if ((km & KeyModifiers.Control) != 0) result |= EditorInputModifiers.Control;
        if ((km & KeyModifiers.Alt) != 0) result |= EditorInputModifiers.Alt;
        return result;
    }

    private static string? AvaloniaKeyToCode(Key key) => key switch
    {
        Key.A => "A", Key.B => "B", Key.C => "C", Key.D => "D",
        Key.E => "E", Key.F => "F", Key.G => "G", Key.H => "H",
        Key.I => "I", Key.J => "J", Key.K => "K", Key.L => "L",
        Key.M => "M", Key.N => "N", Key.O => "O", Key.P => "P",
        Key.Q => "Q", Key.R => "R", Key.S => "S", Key.T => "T",
        Key.U => "U", Key.V => "V", Key.W => "W", Key.X => "X",
        Key.Y => "Y", Key.Z => "Z",
        Key.D0 => "0", Key.D1 => "1", Key.D2 => "2", Key.D3 => "3",
        Key.D4 => "4", Key.D5 => "5", Key.D6 => "6", Key.D7 => "7",
        Key.D8 => "8", Key.D9 => "9",
        Key.NumPad0 => "Numpad0", Key.NumPad1 => "Numpad1",
        Key.NumPad2 => "Numpad2", Key.NumPad3 => "Numpad3",
        Key.NumPad4 => "Numpad4", Key.NumPad5 => "Numpad5",
        Key.NumPad6 => "Numpad6", Key.NumPad7 => "Numpad7",
        Key.NumPad8 => "Numpad8", Key.NumPad9 => "Numpad9",
        Key.F1 => "F1", Key.F2 => "F2", Key.F3 => "F3", Key.F4 => "F4",
        Key.F5 => "F5", Key.F6 => "F6", Key.F7 => "F7", Key.F8 => "F8",
        Key.F9 => "F9", Key.F10 => "F10", Key.F11 => "F11", Key.F12 => "F12",
        Key.Escape => "Escape", Key.Space => "Space", Key.Enter => "Enter",
        Key.Tab => "Tab", Key.Back => "Back", Key.Delete => "Delete",
        Key.Insert => "Insert", Key.Home => "Home", Key.End => "End",
        Key.PageUp => "PageUp", Key.PageDown => "PageDown",
        Key.Left => "Left", Key.Right => "Right", Key.Up => "Up", Key.Down => "Down",
        Key.OemPlus => "Equals", Key.OemMinus => "Minus",
        Key.OemPeriod => "Period", Key.OemComma => "Comma",
        Key.OemQuestion => "Slash", Key.OemSemicolon => "Semicolon",
        Key.OemQuotes => "Quote", Key.OemTilde => "Backtick",
        Key.OemBackslash => "Backslash", Key.OemOpenBrackets => "LeftBracket",
        Key.OemCloseBrackets => "RightBracket",
        _ => null
    };

    // ─── 恢复 / 搜索 ────────────────────────────────────

    private void OnRestoreSingleClicked(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string actionId)
        {
            RemoveDraftOverrides(actionId);
            UpdateButtonStates();
            PopulateBindings();
        }
    }

    private void OnRestoreAllClicked(object? sender, RoutedEventArgs e)
    {
        ClearAllDraftOverrides();
        UpdateButtonStates();
        PopulateBindings();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        PopulateBindings(SearchBox?.Text);
    }

    // ─── 保存 / 应用 / 取消 ─────────────────────────────

    private void OnCancelClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnApplyClicked(object? sender, RoutedEventArgs e)
    {
        if (!HasAnyChanges()) return;
        FlushDraft(closeAfterSave: false);
    }

    private void OnSaveClicked(object? sender, RoutedEventArgs e)
    {
        if (!HasAnyChanges()) { Close(); return; }
        FlushDraft(closeAfterSave: true);
    }

    private void FlushDraft(bool closeAfterSave)
    {
        var doc = new EditorSettingsDocument { Input = _draftBindingSet };
        if (!EditorSettingsWriter.TrySave(doc, out var error))
        {
            ShowError($"保存设置失败：{error}");
            return;
        }
        if (!EditorInputService.Instance.TryApplyNewBindingSet(_draftBindingSet, out var applyError))
        {
            ShowError($"应用绑定失败：{applyError}");
            return;
        }
        _originalBindingSet = _draftBindingSet;
        _draftBindingSet = _draftBindingSet with { Overrides = _draftBindingSet.Overrides.ToArray() };
        UpdateButtonStates();
        if (closeAfterSave) Close();
    }

    private void UpdateButtonStates()
    {
        var hasChanges = HasAnyChanges();
        if (ApplyButton is not null) ApplyButton.IsEnabled = hasChanges;
        if (SaveButton is not null) SaveButton.IsEnabled = hasChanges;
    }

    private void ShowError(string message)
    {
        Title = $"⚠ {message}";
    }
}
