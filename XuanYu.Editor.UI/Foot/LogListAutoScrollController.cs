using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-F2：日志列表自动跟随——底部附近跟随、阅读旧日志不强制拉回、滚到底恢复。
// 分类切换（ForceFollow）定位最新；清空日志时滚动范围归零自动回到跟随态。
public sealed class LogListAutoScrollController : IDisposable
{
    readonly ListBox _listBox;
    readonly EventHandler<TemplateAppliedEventArgs> _onTemplateApplied;
    ScrollViewer? _scroll;
    bool _pendingScroll;
    bool _resolved;
    bool _disposed;
    bool _atTail = true;
    bool _forceNext;

    public LogListAutoScrollController(ListBox listBox)
    {
        _listBox = listBox;
        _onTemplateApplied = (_, _) => Resolve();
        _listBox.TemplateApplied += _onTemplateApplied;
        Resolve();
    }

    void Resolve()
    {
        if (_resolved) return;
        var sv = _listBox.FindDescendantOfType<ScrollViewer>();
        if (sv is null) return;
        _scroll = sv;
        _scroll.ScrollChanged += OnScrollChanged;
        _resolved = true;
        ScrollToTail();
    }

    void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        var max = _scroll!.Extent.Height - _scroll.Viewport.Height;
        _atTail = LogAutoScrollPolicy.ShouldFollow(_scroll.Offset.Y, max);
        if (_atTail) _forceNext = false; // 用户滚到底 → 恢复跟随并清除强制
    }

    public void OnLogItemsChanged()
    {
        if (_scroll is null || _pendingScroll) return;
        if (!_atTail && !_forceNext) return; // 阅读旧日志时新日志不强制拉回
        _pendingScroll = true;
        Dispatcher.UIThread.InvokeAsync(ScrollToTail, DispatcherPriority.Render);
    }

    public void ForceFollow()
    {
        _forceNext = true;
        OnLogItemsChanged();
    }

    void ScrollToTail()
    {
        _pendingScroll = false;
        if (_scroll is null) return;
        // 列表模板重建/分离期间滚动目标失效（控件生命周期竞态）；已优先用 _scroll 空检查缩窄，此异常可安全忽略。
        try { _scroll.ScrollToEnd(); }
        catch (InvalidOperationException) { }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _listBox.TemplateApplied -= _onTemplateApplied;
        if (_scroll is not null) _scroll.ScrollChanged -= OnScrollChanged;
    }
}
