using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

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
    private TextBlock? _entityVisualKindText;
    private TextBlock? _debugListEmptyText;
    private StackPanel? _debugListPanel;
    private TextBlock? _vulkanBackendStatusText;

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
        _entityVisualKindText = this.FindControl<TextBlock>("EntityVisualKindText");
        _debugListEmptyText = this.FindControl<TextBlock>("DebugListEmptyText");
        _debugListPanel = this.FindControl<StackPanel>("DebugListPanel");
        _vulkanBackendStatusText = this.FindControl<TextBlock>("VulkanBackendStatusText");
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

        if (_entityVisualKindText is not null)
        {
            _entityVisualKindText.Text = summary.VisualKindText;
        }

        ShowSinglePanel(_entitySummaryContent);
    }

    /// <summary>
    /// 显示 Vulkan 后端状态文本。
    /// </summary>
    public void ShowVulkanBackendStatus(string statusText)
    {
        if (_vulkanBackendStatusText is not null)
        {
            _vulkanBackendStatusText.Text = statusText;
        }
    }

    /// <summary>
    /// 显示 RenderScene 调试对象列表。
    /// 不覆盖当前实体摘要，只更新底部调试区域。
    /// </summary>
    public void ShowRenderSceneSummary(ViewportRenderSceneSummary summary)
    {
        if (_debugListEmptyText is null || _debugListPanel is null)
        {
            return;
        }

        if (summary.Objects.Count == 0)
        {
            _debugListEmptyText.IsVisible = true;
            _debugListPanel.IsVisible = false;
            return;
        }

        _debugListEmptyText.IsVisible = false;
        _debugListPanel.IsVisible = true;
        _debugListPanel.Children.Clear();

        for (int i = 0; i < summary.Objects.Count; i++)
        {
            var obj = summary.Objects[i];
            var sourceInfo = obj.SourcePath is not null ? $" | {obj.SourcePath}" : "";
            var text = $"{i + 1}. {obj.DisplayName} | {obj.VisualKindText} | {obj.PositionText}{sourceInfo}";

            var textBlock = new TextBlock
            {
                Text = text,
                Foreground = new Avalonia.Media.SolidColorBrush(Color.Parse("#C9D1D9")),
                TextWrapping = TextWrapping.Wrap
            };

            _debugListPanel.Children.Add(textBlock);
        }
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
