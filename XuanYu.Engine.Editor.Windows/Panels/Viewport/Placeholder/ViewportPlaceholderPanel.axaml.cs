using Avalonia.Controls;
using Avalonia.Input;

namespace FluidWarfare.Editor.Windows.Panels.Viewport;

/// <summary>视口占位面板。未创建 Vulkan Surface 时显示文本状态。</summary>
public sealed partial class ViewportPlaceholderPanel : UserControl
{
    StackPanel? _defaultContent, _emptyWorldContent, _entitySummaryContent;
    TextBlock? _entityTypeLabel, _entityNameText, _entityIdText, _entitySourceText;
    TextBlock? _entityPositionText, _entityVisualKindText;
    TextBlock? _debugListEmptyText;
    StackPanel? _debugListPanel;
    TextBlock? _vulkanBackendStatusText, _vulkanInstanceStatusText, _vulkanDeviceStatusText;

    public event EventHandler? ViewportFocused;

    public ViewportPlaceholderPanel() { InitializeComponent(); CacheControls(); }

    void CacheControls()
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
        _vulkanInstanceStatusText = this.FindControl<TextBlock>("VulkanInstanceStatusText");
        _vulkanDeviceStatusText = this.FindControl<TextBlock>("VulkanDeviceStatusText");
    }

    void ShowSinglePanel(StackPanel? active)
    {
        if (_defaultContent is not null) _defaultContent.IsVisible = _defaultContent == active;
        if (_emptyWorldContent is not null) _emptyWorldContent.IsVisible = _emptyWorldContent == active;
        if (_entitySummaryContent is not null) _entitySummaryContent.IsVisible = _entitySummaryContent == active;
    }

    void HandleViewportPointerPressed(object? sender, PointerPressedEventArgs e) => ViewportFocused?.Invoke(this, EventArgs.Empty);
}
