using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

// LOG-UX-1-R1：日志面板自动滚动到最新。
// 边界：只改本文件 UI 行为，不碰 Vulkan / Render.Vulkan / NativeHost 生命周期 / 日志数据模型。
public partial class Foot : UserControl
{
    ScrollViewer? _logScroll;
    bool _followTail = true;   // 用户在底部时跟随最新；上翻历史时暂停
    bool _pendingScroll;       // LogItems 变化且当时在底部，待布局后滚到底
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
            _logScroll.ScrollChanged += LogScroll_OnScrollChanged;
            LogList.LayoutUpdated += LogList_OnLayoutUpdated;
            _scrollHooked = true;
            _pendingScroll = _followTail; // 首次附着即对齐到底部（若在底部跟随）
        }
        if (DataContext is UiVm vm && !_vmHooked)
        {
            vm.PropertyChanged += Vm_OnPropertyChanged;
            _vmHooked = true;
        }
    }

    void Vm_OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UiVm.LogItems))
            _pendingScroll = _followTail;
    }

    void LogList_OnLayoutUpdated(object? sender, EventArgs e)
    {
        if (!_pendingScroll || _logScroll is null) return;
        _pendingScroll = false;
        _logScroll.ScrollToEnd(); // 布局完成后再滚，确保新项已测量
    }

    void LogScroll_OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        // 仅当用户主动滚动（Offset 明显变化）才重判跟随态；Extent 增长不误判
        if (_logScroll is null || Math.Abs(e.OffsetDelta.Y) < 0.5) return;
        _followTail = _logScroll.Offset.Y + _logScroll.Viewport.Height >= _logScroll.Extent.Height - 2.0;
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
                catch (Exception ex) { Debug.WriteLine($"[LogList] 复制失败: {ex}"); }
            }
            vm.NotifyLogCopied();
            e.Handled = true;
        }
    }
}
