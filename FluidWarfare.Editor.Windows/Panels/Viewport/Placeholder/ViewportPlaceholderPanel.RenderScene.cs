using Avalonia.Controls;
using Avalonia.Media;

namespace FluidWarfare.Editor.Windows.Panels.Viewport;

/// <summary>Partial：RenderScene 调试对象列表显示。</summary>
sealed partial class ViewportPlaceholderPanel
{
    public void ShowRenderSceneSummary(ViewportRenderSceneSummary summary)
    {
        if (_debugListEmptyText is null || _debugListPanel is null) return;
        if (summary.Objects.Count == 0)
        { _debugListEmptyText.IsVisible = true; _debugListPanel.IsVisible = false; return; }
        _debugListEmptyText.IsVisible = false;
        _debugListPanel.IsVisible = true;
        _debugListPanel.Children.Clear();
        for (int i = 0; i < summary.Objects.Count; i++)
        {
            var obj = summary.Objects[i];
            var src = obj.SourcePath is not null ? $" | {obj.SourcePath}" : "";
            _debugListPanel.Children.Add(new TextBlock
            { Text = $"{i + 1}. {obj.DisplayName} | {obj.VisualKindText} | {obj.PositionText}{src}",
                Foreground = new SolidColorBrush(Color.Parse("#C9D1D9")), TextWrapping = TextWrapping.Wrap });
        }
    }
}
