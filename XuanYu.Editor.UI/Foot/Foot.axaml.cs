using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

// LOG-UX-1-R3：日志面板自动滚动（R2 因 PropertyChanged→LayoutUpdated 时序不可靠未生效，
// 本版改用 Dispatcher.InvokeAsync(Render)，确保布局完成后再滚到底）。
// 边界：只改本文件 UI 行为，不碰 Vulkan / Render.Vulkan / NativeHost / 日志数据模型。
public partial class Foot : UserControl
{
    ScrollViewer? _logScroll;
    bool _followTail = true;   // 用户在底部时跟随最新；上翻历史时暂停
    bool _scrollHooked;
    bool _vmHooked;

    public Foot()
    {
        InitializeComponent();
        AttachedToVisualTree += (_, _) => TryHook();
        DataContextChanged += (_, _) => TryHook();
    }

    void TryHook()
    {
        _logScroll ??= LogList.FindDescendantOfType<ScrollViewer>();
        if (_logScroll is not null && !_scrollHooked)
        {
            _logScroll.ScrollChanged += OnScrollChanged;
            _scrollHooked = true;
            ScrollToTail(); // 首次附着对齐到底部
        }
        if (DataContext is UiVm vm && !_vmHooked)
        {
            vm.PropertyChanged += OnVmPropertyChanged;
            _vmHooked = true;
        }
    }

    // 核心修复：LogItems 变更后用 InvokeAsync(Render) 延迟滚到底。
    void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(UiVm.LogItems)) return;
        if (!_followTail || _logScroll is null) return;
        Dispatcher.InvokeAsync(ScrollToTail, DispatcherPriority.Render);
    }

    void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (_logScroll is null || Math.Abs(e.OffsetDelta.Y) < 0.5) return;
        _followTail = _logScroll.Offset.Y + _logScroll.Viewport.Height >= _logScroll.Extent.Height - 2.0;
    }

    void ScrollToTail()
    {
        if (_logScroll is null) return;
        try { _logScroll.ScrollToEnd(); } catch { /* 控件可能已卸载 */ }
    }

    void LogList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox lb && DataContext is UiVm vm)
            vm.SetSelectedEntries(lb.SelectedItems?.OfType<LogEntry>().ToArray() ?? []);
    }

    async void LogList_KeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not ListBox lb || DataContext is not UiVm vm) return;
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        if (e.Key == Key.A) { lb.SelectAll(); e.Handled = true; return; }
        if (e.Key == Key.C && vm.HasSelectedEntries)
        {
            var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
            if (clipboard is not null)
            {
                try { await clipboard.SetTextAsync(vm.SelectedEntriesClipboardText); }
                catch { /* 剪贴板不可用 */ }
            }
            vm.NotifyLogCopied();
            e.Handled = true;
        }
    }
}
