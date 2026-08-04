using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-F3：两阶段尾项定位——第一阶段目标式滚动（Render），
// 第二阶段布局完成后按最终滚动范围修正（Background，只执行一次）。
public sealed partial class LogListAutoScrollController
{
    const double TailEpsilon = 0.5;

    // 第一阶段：目标式滚动——把最后一项滚入视口，再安排一次最终修正。
    void RunPrimaryScroll()
    {
        _primaryPending = false;
        if (_disposed || _scroll is null) return;
        if (!_atTail && !_forceNext) return;
        var version = _requestVersion;
        var lastItem = GetLatestItem();
        if (lastItem is null || !ReferenceEquals(lastItem, _pendingLastItem)) return;
        _listBox.ScrollIntoView(lastItem);
        ScheduleFinalCorrection(version, lastItem);
    }

    // 第二阶段只安排一次；新请求到来时重置标志，允许新一轮修正。
    void ScheduleFinalCorrection(long version, object lastItem)
    {
        if (_tailCorrectionScheduled) return;
        _tailCorrectionScheduled = true;
        Dispatcher.UIThread.Post(
            () => RunFinalCorrection(version, lastItem), DispatcherPriority.Background);
    }

    // 第二阶段：布局完成后读取最终滚动范围，垂直修正到尾部，保留水平偏移。
    void RunFinalCorrection(long version, object lastItem)
    {
        _tailCorrectionScheduled = false;
        if (!IsCurrentRequest(version)) return;
        if (_disposed || _scroll is null) return;
        if (!_atTail && !_forceNext) return;
        if (!ReferenceEquals(lastItem, GetLatestItem())) return; // 尾项已变化，不再定位旧对象

        var container = _listBox.ContainerFromItem(lastItem)
            ?? _listBox.ContainerFromIndex(_listBox.ItemCount - 1);
        container?.BringIntoView();

        var maximumY = Math.Max(0, _scroll.Extent.Height - _scroll.Viewport.Height);
        if (maximumY - _scroll.Offset.Y > TailEpsilon)
        {
            _programmaticCorrection = true;
            try { _scroll.Offset = new Vector(_scroll.Offset.X, maximumY); }
            finally { _programmaticCorrection = false; }
            _atTail = true; // 程序化修正落底：跟随态确定，且不再强制
            _forceNext = false;
        }
    }

    bool IsCurrentRequest(long version) => !_disposed && version == _requestVersion;
}
