using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Windows.Shell;

namespace FluidWarfare.Editor.Windows.Panels.Inspector;

public sealed partial class InspectorPanel : UserControl
{
    // ─── 控件引用 ──────────────────────────────────────────────────
    private TextBlock? _emptySelectionText;
    private StackPanel? _selectionDetails;
    private TextBlock? _selectionKindText;
    private TextBlock? _selectionNameText;
    private TextBlock? _selectionEntityIdText;
    private TextBlock? _selectionSourceText;
    private StackPanel? _transformSection;
    private TextBox? _transformXText;
    private TextBox? _transformYText;
    private TextBox? _transformZText;
    private TextBlock? _transformErrorText;
    private Button? _applyButton;
    private Button? _resetButton;
    private Button? _groundPlaceButton;

    // ─── 数值拖拽 ────────────────────────────────────────────────
    private TextBlock? _scrubLabelX;
    private TextBlock? _scrubLabelY;
    private TextBlock? _scrubLabelZ;
    private readonly Transform.TransformAxisScrubState _scrubState = new();
    private string _scrubEntityId = string.Empty;

    /// <summary>数值拖拽时触发。</summary>
    public event Action<string, Transform.TransformPositionAxis, double>? ScrubValueChanged;

    /// <summary>数值拖拽完成时触发。</summary>
    public event Action<string, Transform.TransformPositionAxis, double>? ScrubCompleted;

    /// <summary>数值拖拽取消时触发。</summary>
    public event Action<string, Transform.TransformPositionAxis, double>? ScrubCancelled;

    /// <summary>获取当前选中的实体 ID（EditorShell 设置）。</summary>
    public string ScrubEntityId { get => _scrubEntityId; set => _scrubEntityId = value; }

    // ─── 状态 ──────────────────────────────────────────────────────
    private bool _isUpdatingTransformTexts;

    // ─── 事件 ──────────────────────────────────────────────────────

    /// <summary>用户修改了 Transform 输入草稿（每次输入变化时触发）。</summary>
    public event Action<string, string, string>? TransformDraftChanged;

    /// <summary>用户点击"应用坐标"。</summary>
    public event Action<string, string, string>? TransformApplyRequested;

    /// <summary>用户点击"重置"。</summary>
    public event Action? TransformResetRequested;

    /// <summary>用户点击"在地面放置"。</summary>
    public event Action? GroundPlacementRequested;

    public InspectorPanel()
    {
        InitializeComponent();
        CacheControls();
        AttachKeyboardHandlers();
    }

    private void CacheControls()
    {
        _emptySelectionText = this.FindControl<TextBlock>("EmptySelectionText");
        _selectionDetails = this.FindControl<StackPanel>("SelectionDetails");
        _selectionKindText = this.FindControl<TextBlock>("SelectionKindText");
        _selectionNameText = this.FindControl<TextBlock>("SelectionNameText");
        _selectionEntityIdText = this.FindControl<TextBlock>("SelectionEntityIdText");
        _selectionSourceText = this.FindControl<TextBlock>("SelectionSourceText");
        _transformSection = this.FindControl<StackPanel>("TransformSection");
        _transformXText = this.FindControl<TextBox>("TransformXText");
        _transformYText = this.FindControl<TextBox>("TransformYText");
        _transformZText = this.FindControl<TextBox>("TransformZText");
        _transformErrorText = this.FindControl<TextBlock>("TransformErrorText");
        _applyButton = this.FindControl<Button>("ApplyButton");
        _resetButton = this.FindControl<Button>("ResetButton");
        _groundPlaceButton = this.FindControl<Button>("GroundPlaceButton");
        _scrubLabelX = this.FindControl<TextBlock>("ScrubLabelX");
        _scrubLabelY = this.FindControl<TextBlock>("ScrubLabelY");
        _scrubLabelZ = this.FindControl<TextBlock>("ScrubLabelZ");
    }

    private void AttachKeyboardHandlers()
    {
        // Enter → apply, Esc → reset when TextBox is focused
        void HandleKeyDown(TextBox tb, object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var (x, y, z) = GetTransformTexts();
                TransformApplyRequested?.Invoke(x, y, z);
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                TransformResetRequested?.Invoke();
                e.Handled = true;
            }
        }

        if (_transformXText is not null)
            _transformXText.KeyDown += (s, e) => HandleKeyDown(_transformXText, s, e);
        if (_transformYText is not null)
            _transformYText.KeyDown += (s, e) => HandleKeyDown(_transformYText, s, e);
        if (_transformZText is not null)
            _transformZText.KeyDown += (s, e) => HandleKeyDown(_transformZText, s, e);
    }

    // ─── 公共方法 ──────────────────────────────────────────────────

    public void ShowSelection(EditorSelection selection)
    {
        ShowProjectFileSelection(selection);
    }

    public void ShowNoSelection()
    {
        SetEmptyVisible(true);
        SetDetailsVisible(false);
        SetTransformVisible(false);
    }

    public void ShowProjectFileSelection(EditorSelection selection)
    {
        ShowSelectionCore(selection);
        SetTransformVisible(false);
    }

    /// <summary>
    /// 显示世界实体选择（含 Transform 编辑区）。
    /// 程序化设置输入框，不触发 TransformDraftChanged。
    /// </summary>
    public void ShowWorldEntitySelection(
        EditorSelection selection,
        string? entityId,
        Vector3d? position,
        string? sourcePath)
    {
        ShowSelectionCore(selection);
        SetTransformVisible(true);

        if (_selectionEntityIdText is not null)
            _selectionEntityIdText.Text = $"EntityId：{entityId ?? "无"}";

        if (_selectionSourceText is not null)
            _selectionSourceText.IsVisible = sourcePath is not null;
        if (_selectionSourceText is not null)
            _selectionSourceText.Text = sourcePath is not null ? $"来源：{sourcePath}" : string.Empty;

        // 程序化填值，抑制草稿事件
        if (position is not null)
            SetTransformTexts(
                position.Value.X.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
                position.Value.Y.ToString("F3", System.Globalization.CultureInfo.InvariantCulture),
                position.Value.Z.ToString("F3", System.Globalization.CultureInfo.InvariantCulture));
        else
            SetTransformTexts(string.Empty, string.Empty, string.Empty);

        ClearTransformError();
        SetTransformDraftState(false, false, null);
    }

    /// <summary>
    /// 设置地面放置模式状态。
    /// </summary>
    public void SetPlacementMode(bool isActive)
    {
        if (_groundPlaceButton is null) return;
        _groundPlaceButton.Content = isActive ? "放置中… Esc 取消" : "在地面放置";
        _groundPlaceButton.IsEnabled = !isActive;
    }

    /// <summary>
    /// 程序化更新 Transform 输入框内容。
    /// 使用 _isUpdatingTransformTexts 抑制 TransformDraftChanged 事件。
    /// </summary>
    public void SetTransformTexts(string x, string y, string z)
    {
        _isUpdatingTransformTexts = true;
        try
        {
            if (_transformXText is not null) _transformXText.Text = x;
            if (_transformYText is not null) _transformYText.Text = y;
            if (_transformZText is not null) _transformZText.Text = z;
        }
        finally
        {
            _isUpdatingTransformTexts = false;
        }

        ClearTransformError();
        SetTransformDraftState(false, false, null);
    }

    /// <summary>
    /// 获取当前 Transform 输入文本。
    /// </summary>
    public (string X, string Y, string Z) GetTransformTexts() =>
        (_transformXText?.Text ?? string.Empty,
         _transformYText?.Text ?? string.Empty,
         _transformZText?.Text ?? string.Empty);

    public void SetGroundPlaceEnabled(bool enabled)
    {
        if (_groundPlaceButton is not null)
            _groundPlaceButton.IsEnabled = enabled;
    }

    // ─── 统一的 Transform 草稿状态 ────────────────────────────────

    /// <summary>
    /// 统一设置 Transform 草稿状态。外部调用方（EditorShell）在每次
    /// TransformDraftChanged 后调用此方法更新按钮和错误。
    /// </summary>
    /// <param name="canApply">是否可应用（坐标合法且与正式值不同）。</param>
    /// <param name="canReset">是否可重置（草稿与正式值不同）。</param>
    /// <param name="error">校验错误信息，null 表示无错误。</param>
    public void SetTransformDraftState(bool canApply, bool canReset, string? error)
    {
        if (_applyButton is not null)
            _applyButton.IsEnabled = canApply;

        if (_resetButton is not null)
            _resetButton.IsEnabled = canReset;

        if (_transformErrorText is not null)
        {
            var hasError = !string.IsNullOrWhiteSpace(error);
            _transformErrorText.IsVisible = hasError;
            _transformErrorText.Text = error ?? string.Empty;
        }
    }

    // ─── UI 事件处理 ──────────────────────────────────────────────

    private void OnTransformTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_isUpdatingTransformTexts)
            return;

        ClearTransformError();
        SetTransformDraftState(false, false, null);
        var (x, y, z) = GetTransformTexts();
        TransformDraftChanged?.Invoke(x, y, z);
    }

    private void OnApplyClicked(object? sender, RoutedEventArgs e)
    {
        var (x, y, z) = GetTransformTexts();
        TransformApplyRequested?.Invoke(x, y, z);
    }

    private void OnResetClicked(object? sender, RoutedEventArgs e)
    {
        TransformResetRequested?.Invoke();
    }

    private void OnGroundPlaceClicked(object? sender, RoutedEventArgs e)
    {
        GroundPlacementRequested?.Invoke();
    }

    // ─── 数值拖拽（X/Y/Z 标签拖拽微调）───────────────────────────

    private void OnScrubPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not TextBlock label) return;
        if (string.IsNullOrEmpty(_scrubEntityId)) return;

        var axis = label == _scrubLabelX ? Transform.TransformPositionAxis.X
                 : label == _scrubLabelY ? Transform.TransformPositionAxis.Y
                 : label == _scrubLabelZ ? Transform.TransformPositionAxis.Z
                 : Transform.TransformPositionAxis.X;

        var text = axis switch
        {
            Transform.TransformPositionAxis.X => _transformXText?.Text,
            Transform.TransformPositionAxis.Y => _transformYText?.Text,
            _ => _transformZText?.Text
        };

        if (!double.TryParse(text, out var value)) return;

        e.Pointer.Capture(label);
        _scrubState.Begin(axis, value, e.GetPosition(this).X, e.KeyModifiers);
    }

    private void OnScrubPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_scrubState.IsScrubbing) return;
        _scrubState.Update(e.GetPosition(this).X, e.KeyModifiers);

        var text = _scrubState.CurrentValue.ToString("F3");
        switch (_scrubState.Axis)
        {
            case Transform.TransformPositionAxis.X: if (_transformXText is not null) _transformXText.Text = text; break;
            case Transform.TransformPositionAxis.Y: if (_transformYText is not null) _transformYText.Text = text; break;
            case Transform.TransformPositionAxis.Z: if (_transformZText is not null) _transformZText.Text = text; break;
        }

        ScrubValueChanged?.Invoke(_scrubEntityId, _scrubState.Axis, _scrubState.CurrentValue);
    }

    private void OnScrubPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (!_scrubState.IsScrubbing) return;
        var value = _scrubState.CurrentValue;
        _scrubState.Complete();
        ScrubCompleted?.Invoke(_scrubEntityId, _scrubState.Axis, value);
    }

    private void OnScrubCaptureLost(object? sender, PointerCaptureLostEventArgs e)
    {
        if (!_scrubState.IsScrubbing) return;
        var initialValue = _scrubState.InitialValue;
        _scrubState.Cancel();
        ScrubCancelled?.Invoke(_scrubEntityId, _scrubState.Axis, initialValue);
    }

    // ─── 内部辅助 ──────────────────────────────────────────────────

    private void ShowSelectionCore(EditorSelection selection)
    {
        SetEmptyVisible(false);
        SetDetailsVisible(true);

        if (_selectionKindText is not null)
            _selectionKindText.Text = $"类型：{selection.Kind}";
        if (_selectionNameText is not null)
            _selectionNameText.Text = $"名称：{selection.DisplayName}";
    }

    private void SetEmptyVisible(bool visible)
    {
        if (_emptySelectionText is not null)
        {
            _emptySelectionText.IsVisible = visible;
            if (visible) _emptySelectionText.Text = "未选择对象";
        }
    }

    private void SetDetailsVisible(bool visible)
    {
        if (_selectionDetails is not null)
            _selectionDetails.IsVisible = visible;
    }

    private void SetTransformVisible(bool visible)
    {
        if (_transformSection is not null)
            _transformSection.IsVisible = visible;
    }

    private void ClearTransformError()
    {
        if (_transformErrorText is not null)
        {
            _transformErrorText.IsVisible = false;
            _transformErrorText.Text = string.Empty;
        }
    }

    /// <summary>
    /// 显示 Transform 校验错误信息（临时兼容，逐步迁移到 SetTransformDraftState）。
    /// </summary>
    public void ShowTransformError(string errorMessage)
    {
        SetTransformDraftState(false, true, errorMessage);
    }
}
