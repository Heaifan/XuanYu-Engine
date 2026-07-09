using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace XuanYu.Editor.UI;

// LOG-UX-1-R4（用户称 R2）：自动滚动 + 种子清理 + 控制台去重。
// 自动滚动根因：R3 的 TryHook 在 ListBox 模板未应用时 FindDescendantOfType<ScrollViewer> 返回 null 且不再重试，_logScroll 永远为 null → 滚动死。
// 本版延迟解析 + TemplateApplied 重试，直接 ScrollToEnd 控 Offset。边界：只改本文件 + UiVm.Logging（去种子）+ 低层 Vulkan Log 辅助（去 Console.WriteLine，单出口留 VulkanBridgeLogFormatter）。
public partial class Foot : UserControl
{
    ScrollViewer? _logScroll;
    bool _followTail = true;
    bool _scrollHooked;
    bool _vmHooked;

    public Foot()
    {
        InitializeComponent();
        AttachedToVisualTree += (_, _) => ResolveScrollViewer();
        DataContextChanged += (_, _) => HookVm();
        LogList.TemplateApplied += (_, _) => ResolveScrollViewer();
    }

    void ResolveScrollViewer()
    {
        _logScroll ??= LogList.FindDescendantOfType<ScrollViewer>();
        if (_logScroll is not null && !_scrollHooked)
        {
            _logScroll.ScrollChanged += OnScrollChanged;
            _scrollHooked = true;
            ScrollToTail();
        }
        else if (_logScroll is null && !_scrollHooked)
        {
            Dispatcher.InvokeAsync(ResolveScrollViewer, DispatcherPriority.Loaded); // 模板未应用则重试
        }
    }

    void HookVm()
    {
        if (DataContext is UiVm vm && !_vmHooked)
        { vm.PropertyChanged += OnVmPropertyChanged; _vmHooked = true; }
    }

    void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(UiVm.LogItems)) return;
        if (!_followTail) return;
        ResolveScrollViewer();
        if (_logScroll is null) return;
        Dispatcher.InvokeAsync(ScrollToTail, DispatcherPriority.Render); // 布局完成后滚到底
    }

    void OnScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        if (_logScroll is null || Math.Abs(e.OffsetDelta.Y) < 0.5) return;
        _followTail = _logScroll.Offset.Y + _logScroll.Viewport.Height >= _logScroll.Extent.Height - 2.0;
    }

    void ScrollToTail()
    {
        if (_logScroll is null) { ResolveScrollViewer(); if (_logScroll is null) return; }
        try { _logScroll.ScrollToEnd(); }
        catch (Exception ex) { Debug.WriteLine($"[Foot] ScrollToEnd: {ex.Message}"); }
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
