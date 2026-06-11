using Avalonia.Controls;
using Avalonia.Input;

namespace FluidWarfare.Editor.Windows.Panels.Viewport;

public sealed partial class ViewportPlaceholderPanel : UserControl
{
    private StackPanel? _defaultContent;
    private StackPanel? _emptyWorldContent;
    private StackPanel? _entitySummaryContent;
    private TextBlock? _entityTypeLabel;
    private TextBlock? _entityNameText;
    private TextBlock? _entityIdText;
    private TextBlock? _entitySourceText;
    private TextBlock? _entityPositionText;

    public event EventHandler? ViewportFocused;

    public ViewportPlaceholderPanel()
    {
        InitializeComponent();
        CacheControls();
    }

    private void CacheControls()
    {
        _defaultContent = this.FindControl<StackPanel>("DefaultContent");
        _emptyWorldContent = this.FindControl<StackPanel>("EmptyWorldContent");
        _entitySummaryContent = this.FindControl<StackPanel>("EntitySummaryContent");
        _entityTypeLabel = this.FindControl<TextBlock>("EntityTypeLabel");
        _entityNameText = this.FindControl<TextBlock>("EntityNameText");
        _entityIdText = this.FindControl<TextBlock>("EntityIdText");
        _entitySourceText = this.FindControl<TextBlock>("EntitySourceText");
        _entityPositionText = this.FindControl<TextBlock>("EntityPositionText");
    }

    /// <summary>
    /// 显示未选择 World 实体状态。
    /// </summary>
    public void ShowNoWorldEntity()
    {
        ShowSinglePanel(_defaultContent);
    }

    /// <summary>
    /// 显示 World 为空状态。
    /// </summary>
    public void ShowEmptyWorld()
    {
        ShowSinglePanel(_emptyWorldContent);
    }

    /// <summary>
    /// 显示当前选中实体摘要。
    /// </summary>
    public void ShowEntitySummary(ViewportEntitySummary summary)
    {
        if (_entityTypeLabel is not null)
        {
            _entityTypeLabel.Text = summary.SourcePath is not null
                ? "当前选中 World 占位实体："
                : "当前选中 World 实体：";
        }

        if (_entityNameText is not null)
        {
            _entityNameText.Text = summary.DisplayName;
        }

        if (_entityIdText is not null)
        {
            _entityIdText.Text = summary.EntityIdText;
        }

        if (_entitySourceText is not null)
        {
            _entitySourceText.Text = summary.SourcePath ?? "无";
        }

        if (_entityPositionText is not null)
        {
            _entityPositionText.Text = summary.PositionText;
        }

        ShowSinglePanel(_entitySummaryContent);
    }

    private void ShowSinglePanel(StackPanel? activePanel)
    {
        if (_defaultContent is not null) _defaultContent.IsVisible = _defaultContent == activePanel;
        if (_emptyWorldContent is not null) _emptyWorldContent.IsVisible = _emptyWorldContent == activePanel;
        if (_entitySummaryContent is not null) _entitySummaryContent.IsVisible = _entitySummaryContent == activePanel;
    }

    private void HandleViewportPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ViewportFocused?.Invoke(this, EventArgs.Empty);
    }
}
