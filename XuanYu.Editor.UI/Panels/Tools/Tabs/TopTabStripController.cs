using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D3：顶层页签条控制器（模板 TopTabStripTemplate.axaml）。
// 滚轮横向路由走页签条隧道消费（e.Handled=true）；内容区/日志区不是页签条祖先，
// 树结构隔离保证不穿透。纯计算在 TopTabStripModel。
public sealed partial class TopTabStripController : IDisposable
{
    readonly TabControl _tabs;
    readonly TopTabStripModel _model = new();
    Grid? _bar;
    ScrollViewer? _scroller;
    Button? _leftBtn, _rightBtn, _allBtn;
    Border? _fadeLeft, _fadeRight;
    Popup? _hintPopup;
    bool _attached, _disposed;

    public TopTabStripController(TabControl tabs)
    {
        _tabs = tabs;
        _tabs.TemplateApplied += (_, _) => Attach();
        Attach();
    }

    void Attach()
    {
        if (_attached || _disposed) return;
        var named = _tabs.GetVisualDescendants().Where(d => d.Name is not null)
            .ToDictionary(d => d.Name!, d => d);
        _bar = Get<Grid>(named, "TabStripBar");
        _scroller = Get<ScrollViewer>(named, "TabScroller");
        _leftBtn = Get<Button>(named, "TabScrollLeft");
        _rightBtn = Get<Button>(named, "TabScrollRight");
        _allBtn = Get<Button>(named, "TabAllTabs");
        _fadeLeft = Get<Border>(named, "TabFadeLeft");
        _fadeRight = Get<Border>(named, "TabFadeRight");
        _hintPopup = Get<Popup>(named, "OverflowHintPopup");
        if (_bar is null || _scroller is null || _leftBtn is null || _rightBtn is null
            || _allBtn is null || _fadeLeft is null || _fadeRight is null || _hintPopup is null)
            return;
        _attached = true;
        _scroller.ScrollChanged += OnScrollChanged;
        _tabs.SelectionChanged += OnSelectionChanged;
        _bar.AddHandler(InputElement.PointerWheelChangedEvent, OnBarWheel, RoutingStrategies.Tunnel);
        _leftBtn.Click += (_, _) => ScrollTo(_model.ScrollLeft());
        _rightBtn.Click += (_, _) => ScrollTo(_model.ScrollRight());
        _allBtn.Click += (_, _) => OpenAllTabs();
        _hintPopup.PlacementTarget = _bar;
        Refresh();
        EnsureSelectedVisible();
    }

    static T? Get<T>(IReadOnlyDictionary<string, Visual> named, string key) where T : class =>
        named.TryGetValue(key, out var v) ? v as T : null;

    void OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => EnsureSelectedVisible();

    void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        _model.Update(_scroller!.Extent.Width, _scroller.Viewport.Width, _scroller.Offset.X);
        Refresh();
        EnsureSelectedVisible();
        TryShowHintOnce();
    }

    void OnBarWheel(object? sender, PointerWheelEventArgs e)
    {
        if (_scroller is null || !_model.Overflowing) return;
        ScrollTo(_scroller.Offset.X + _model.WheelDelta(e.Delta.Y));
        e.Handled = true; // 页签栏滚轮只滚动页签；到达边界后剩余增量不传递
    }

    void ScrollTo(double offset)
    {
        if (_scroller is null) return;
        _scroller.Offset = new Vector(_model.Clamp(offset), 0);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (!_attached) return;
        _scroller!.ScrollChanged -= OnScrollChanged;
        _tabs.SelectionChanged -= OnSelectionChanged;
        if (_bar is not null) _bar.RemoveHandler(InputElement.PointerWheelChangedEvent, OnBarWheel);
    }
}
