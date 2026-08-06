using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D3：页签条视觉状态刷新与当前页签自动显露（合同 §10.1-2/3/5/6/7/9）。
public sealed partial class TopTabStripController
{
    void Refresh()
    {
        var show = _model.CanScrollLeft || _model.CanScrollRight;
        _leftBtn!.IsVisible = show;      // 宽度充足时不显示滚动控件（合同 2）
        _rightBtn!.IsVisible = show;     // 宽度不足时显示左右箭头（合同 3）
        _leftBtn.IsEnabled = _model.CanScrollLeft;   // 到达边界禁用（合同 7）
        _rightBtn.IsEnabled = _model.CanScrollRight;
        _fadeLeft!.IsVisible = _model.FadeLeft;      // 隐藏方向轻微渐隐（合同 5）
        _fadeRight!.IsVisible = _model.FadeRight;
    }

    void EnsureSelectedVisible()
    {
        if (_scroller is null || _tabs.SelectedItem is not TabItem tab || tab.Bounds.Width <= 0) return;
        _model.Update(_scroller.Extent.Width, _scroller.Viewport.Width, _scroller.Offset.X);
        if (_model.IsFullyVisible(tab.Bounds.X, tab.Bounds.Width)) return;
        ScrollTo(_model.OffsetForVisible(tab.Bounds.X, tab.Bounds.Width));
    }
}
