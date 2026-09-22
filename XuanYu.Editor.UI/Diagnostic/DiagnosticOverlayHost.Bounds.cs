using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public partial class DiagnosticOverlayHost
{
    readonly List<Border> _targetBounds = [];
    public int ActiveRectangleCount => _targetBounds.Count;

    void RenderTargetBounds()
    {
        ClearTargetBounds();
        foreach (var target in DiagnosticRegistry.Targets.Values)
        {
            if (!target.IsEffectivelyVisible || TopLevel.GetTopLevel(target) != _topLevel) continue;
            if (target.TranslatePoint(default, BoundsOwner) is not { } origin) continue;
            var border = new Border { Width = target.Bounds.Width, Height = target.Bounds.Height,
                BorderBrush = Brushes.Gold, BorderThickness = new Thickness(2), IsHitTestVisible = false };
            Canvas.SetLeft(border, origin.X); Canvas.SetTop(border, origin.Y);
            BoundsOwner.Children.Add(border); _targetBounds.Add(border);
        }
    }

    void ClearTargetBounds()
    {
        BoundsOwner.Children.Clear(); _targetBounds.Clear();
    }
}
