using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-F3：日志列表尾项定位——两阶段（Render 目标滚动 + Background 布局后修正），
// 请求合并（_requestVersion 使旧任务失效，防高频日志堆积）；阅读旧日志不强制拉回。
public sealed partial class LogListAutoScrollController : IDisposable
{
    readonly ListBox _listBox;
    readonly EventHandler<TemplateAppliedEventArgs> _onTemplateApplied;
    ScrollViewer? _scroll;
    bool _resolved;
    bool _disposed;
    bool _atTail = true;
    bool _forceNext;
    long _requestVersion;
    object? _pendingLastItem;
    bool _primaryPending;
    bool _tailCorrectionScheduled;
    bool _programmaticCorrection;

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
        RequestLatestItemVisibility(forceFollow: true); // 初始定位到底
    }

    public void OnLogItemsChanged() => RequestLatestItemVisibility(forceFollow: false);

    public void ForceFollow() => RequestLatestItemVisibility(forceFollow: true);

    // 唯一入口：新增日志 / 分类切换 / 清空后重现 / 布局变化都经此。
    void RequestLatestItemVisibility(bool forceFollow)
    {
        if (_disposed || _scroll is null) return;
        if (!forceFollow && !_atTail && !_forceNext) return; // 阅读旧日志不强制拉回
        if (forceFollow) _forceNext = true;
        var lastItem = GetLatestItem();
        if (lastItem is null) // 空集合：恢复跟随，不安排滚动
        {
            _requestVersion++;
            _atTail = true;
            _forceNext = false;
            _tailCorrectionScheduled = false;
            return;
        }
        _requestVersion++;
        _pendingLastItem = lastItem;
        _tailCorrectionScheduled = false;
        if (_primaryPending) return; // 已有任务排队：执行时读取最新请求
        _primaryPending = true;
        Dispatcher.UIThread.Post(RunPrimaryScroll, DispatcherPriority.Render);
    }

    object? GetLatestItem() =>
        _listBox.ItemsSource is System.Collections.IList { Count: > 0 } items
            ? items[items.Count - 1] : null;

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _requestVersion++; // 使未执行任务失效
        _listBox.TemplateApplied -= _onTemplateApplied;
        if (_scroll is not null) _scroll.ScrollChanged -= OnScrollChanged;
    }
}
