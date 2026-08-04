using System;
using Avalonia.Controls;

namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-F3：布局变化统一处理——ScrollChanged 集中覆盖
// Extent/Viewport 变化（Resize、展开折叠、水平滚动条出现、DPI 重测），
// 跟随态下安排合并后的尾项修正；程序化滚动期间不重复安排。
public sealed partial class LogListAutoScrollController
{
    void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (_disposed || _scroll is null) return;
        // _atTail 只由用户滚动（Offset 变化）维护——新日志增大 Extent 时
        // 不得用新最大滚动值重算（否则底部会被误判为已离开，计划 8.1）。
        if (e.OffsetDelta.Y != 0 && !_programmaticCorrection)
        {
            var max = Math.Max(0, _scroll.Extent.Height - _scroll.Viewport.Height);
            _atTail = LogAutoScrollPolicy.ShouldFollow(_scroll.Offset.Y, max);
            if (_atTail) _forceNext = false; // 用户滚到底 → 恢复跟随并清除强制
        }
        if (_programmaticCorrection) return; // 程序自己的 Offset 变化不误判为用户滚动
        if (e.ExtentDelta.Y == 0 && e.ViewportDelta.Y == 0) return;
        if (!_atTail && !_forceNext) return; // 阅读旧日志时布局变化不强制回底部
        RequestLatestItemVisibility(forceFollow: false);
    }
}
