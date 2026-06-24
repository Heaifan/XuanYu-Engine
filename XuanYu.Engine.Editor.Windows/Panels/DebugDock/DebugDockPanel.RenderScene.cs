using Avalonia.Controls;
using Avalonia.Media;

namespace FluidWarfare.Editor.Windows.Panels.DebugDock;

/// <summary>Partial：RenderScene 列表更新。</summary>
sealed partial class DebugDockPanel
{
    public void SetRenderSceneSummary(string title, IReadOnlyList<string> entries)
    {
        if (_renderSceneTitle is not null) _renderSceneTitle.Text = title;
        if (_renderSceneEmpty is null || _renderSceneList is null) return;
        if (entries.Count == 0) { _renderSceneEmpty.IsVisible = true; _renderSceneList.IsVisible = false; return; }
        _renderSceneEmpty.IsVisible = false;
        _renderSceneList.IsVisible = true;
        _renderSceneList.Children.Clear();
        foreach (var entry in entries)
            _renderSceneList.Children.Add(new SelectableTextBlock
            { Text = entry, Foreground = new SolidColorBrush(Color.Parse("#C9D1D9")), TextWrapping = TextWrapping.Wrap });
    }
}
