using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

public sealed class LogListAutoScrollController : IDisposable
{
    readonly ListBox _listBox;
    readonly EventHandler<TemplateAppliedEventArgs> _onTemplateApplied;
    ScrollViewer? _scroll;
    bool _pendingScroll;
    bool _resolved;
    bool _disposed;

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
        _resolved = true;
        ScrollToTail();
    }

    public void OnLogItemsChanged()
    {
        // R8 人工验收优先尾随最新日志；选择/复制旧行不得永久关闭自动滚动。
        if (_scroll is null || _pendingScroll) return;
        _pendingScroll = true;
        Dispatcher.UIThread.InvokeAsync(ScrollToTail, DispatcherPriority.Render);
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
    }
}
