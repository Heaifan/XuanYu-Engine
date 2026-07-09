using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

// LOG-UX-2：日志自动滚动独立控制器。从 Foot.axaml.cs 拆出，避免状态机塞进 UI 代码后置文件。
// 节流：连续多条日志只安排一次滚动（_pendingScroll），100 条日志也只滚一次。
// 防重入：程序滚动期间 _isProgrammaticScroll=true，ScrollChanged 不重算跟随态。
// 单次解析：ScrollViewer 仅在构造期尝试 + TemplateApplied 时解析一次，不每条日志遍历视觉树。
public sealed class LogListAutoScrollController : IDisposable
{
    readonly ListBox _listBox;
    readonly EventHandler<TemplateAppliedEventArgs> _onTemplateApplied;
    ScrollViewer? _scroll;
    bool _followTail = true;
    bool _pendingScroll;
    bool _isProgrammaticScroll;
    bool _resolved;
    bool _disposed;

    public LogListAutoScrollController(ListBox listBox)
    {
        _listBox = listBox;
        _onTemplateApplied = (_, _) => Resolve();
        _listBox.TemplateApplied += _onTemplateApplied;
        Resolve(); // 模板可能已应用
    }

    void Resolve()
    {
        if (_resolved) return;
        var sv = _listBox.FindDescendantOfType<ScrollViewer>();
        if (sv is null) return; // 模板未就绪则等 TemplateApplied；不循环重试、不遍历视觉树
        _scroll = sv;
        _resolved = true;
        _scroll.ScrollChanged += OnScrollChanged;
        if (_followTail) ScrollToTail(); // 解析完成即对齐到底部
    }

    public void OnLogItemsChanged()
    {
        // 用户在看历史 / 尚未解析到 ScrollViewer / 已安排过滚动 → 跳过（节流）
        if (!_followTail || _scroll is null || _pendingScroll) return;
        _pendingScroll = true;
        Dispatcher.UIThread.InvokeAsync(ScrollToTail, DispatcherPriority.Render);
    }

    void ScrollToTail()
    {
        _pendingScroll = false;
        if (_scroll is null || !_followTail) return;
        _isProgrammaticScroll = true;
        try { _scroll.ScrollToEnd(); }
        finally { Dispatcher.UIThread.InvokeAsync(() => _isProgrammaticScroll = false, DispatcherPriority.Loaded); }
    }

    void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (_isProgrammaticScroll || _scroll is null || Math.Abs(e.OffsetDelta.Y) < 0.5) return;
        _followTail = _scroll.Offset.Y + _scroll.Viewport.Height >= _scroll.Extent.Height - 12;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _listBox.TemplateApplied -= _onTemplateApplied;
        if (_scroll is not null) _scroll.ScrollChanged -= OnScrollChanged;
    }
}
