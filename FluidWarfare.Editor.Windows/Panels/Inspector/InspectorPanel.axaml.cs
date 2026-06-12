using Avalonia.Controls;
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

    // ─── 事件 ──────────────────────────────────────────────────────

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
    }

    // ─── 公共方法 ──────────────────────────────────────────────────

    /// <summary>
    /// 兼容旧 API：显示基本选择文本（无 Transform 编辑区）。
    /// </summary>
    public void ShowSelection(EditorSelection selection)
    {
        ShowProjectFileSelection(selection);
    }

    /// <summary>显示"未选择"状态。</summary>
    public void ShowNoSelection()
    {
        SetEmptyVisible(true);
        SetDetailsVisible(false);
        SetTransformVisible(false);
    }

    /// <summary>
    /// 显示项目文件选择（无 Transform 编辑区）。
    /// </summary>
    public void ShowProjectFileSelection(EditorSelection selection)
    {
        ShowSelectionCore(selection);
        SetTransformVisible(false);
    }

    /// <summary>
    /// 显示世界实体选择（含 Transform 编辑区）。
    /// </summary>
    /// <param name="selection">显示信息。</param>
    /// <param name="entityId">实体 ID 字符串。</param>
    /// <param name="position">实体当前位置。</param>
    /// <param name="sourcePath">来源路径（可为 null）。</param>
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

        // 更新 Transform 输入
        if (_transformXText is not null && position is not null)
            _transformXText.Text = position.Value.X.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
        if (_transformYText is not null && position is not null)
            _transformYText.Text = position.Value.Y.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);
        if (_transformZText is not null && position is not null)
            _transformZText.Text = position.Value.Z.ToString("F3", System.Globalization.CultureInfo.InvariantCulture);

        ClearTransformError();
        UpdateApplyButtonState();
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
    /// 更新 Transform 输入框内容（外部程序化修改时调用）。
    /// </summary>
    public void SetTransformTexts(string x, string y, string z)
    {
        if (_transformXText is not null) _transformXText.Text = x;
        if (_transformYText is not null) _transformYText.Text = y;
        if (_transformZText is not null) _transformZText.Text = z;
        ClearTransformError();
        UpdateApplyButtonState();
    }

    /// <summary>
    /// 获取当前 Transform 输入文本。
    /// </summary>
    public (string X, string Y, string Z) GetTransformTexts() =>
        (_transformXText?.Text ?? string.Empty,
         _transformYText?.Text ?? string.Empty,
         _transformZText?.Text ?? string.Empty);

    /// <summary>
    /// 启用或禁用地面放置按钮。
    /// </summary>
    public void SetGroundPlaceEnabled(bool enabled)
    {
        if (_groundPlaceButton is not null)
            _groundPlaceButton.IsEnabled = enabled;
    }

    // ─── UI 事件处理 ──────────────────────────────────────────────

    private void OnTransformTextChanged(object? sender, TextChangedEventArgs e)
    {
        ClearTransformError();
        UpdateApplyButtonState();
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

    // ─── 内部辅助 ──────────────────────────────────────────────────

    private void ShowSelectionCore(EditorSelection selection)
    {
        SetEmptyVisible(false);
        SetDetailsVisible(true);

        if (_selectionKindText is not null)
            _selectionKindText.Text = $"类型：{selection.Kind}";
        if (_selectionNameText is not null)
            _selectionNameText.Text = $"名称：{selection.DisplayName}";
        // Description is replaced by structured fields for world entities
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
    /// 显示 Transform 校验错误信息。
    /// </summary>
    public void ShowTransformError(string errorMessage)
    {
        if (_transformErrorText is not null)
        {
            _transformErrorText.Text = errorMessage;
            _transformErrorText.IsVisible = true;
        }
        if (_applyButton is not null)
            _applyButton.IsEnabled = false;
    }

    /// <summary>
    /// 更新"应用坐标"按钮启用状态（输入与正式值不同时启用）。
    /// </summary>
    public void UpdateApplyButtonState()
    {
        // Button state is managed externally via SetApplyEnabled
    }

    /// <summary>
    /// 设置"应用坐标"按钮启用状态。
    /// </summary>
    public void SetApplyEnabled(bool enabled)
    {
        if (_applyButton is not null)
            _applyButton.IsEnabled = enabled;
    }

    /// <summary>
    /// 设置"重置"按钮启用状态。
    /// </summary>
    public void SetResetEnabled(bool enabled)
    {
        if (_resetButton is not null)
            _resetButton.IsEnabled = enabled;
    }
}
