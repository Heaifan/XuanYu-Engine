using System;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace XuanYu.Editor.UI;

// LOG-UX-1-R5A：止血修复——彻底禁用日志自动滚动，恢复编辑器稳定启动。
// 根因：自动滚动状态机（TemplateApplied 解析 ScrollViewer + ScrollChanged 跟随 + Dispatcher 自动 ScrollToTail）
// 在 Vulkan Attach 同步执行于 UI 线程期间触发视觉树遍历/Dispatcher 堆积，导致主窗口「未响应」、退出码 0xCFFFFFFF。
// 保留：Ctrl+A/Ctrl+C 多行复制、详情选中、AttachConsole、控制台去重、种子清理（均在其它文件，不受影响）。
// 自动滚动后续由独立控制器重新设计（LOG-UX-2：LogListAutoScrollController.cs），不再在本文件硬顶。
public partial class Foot : UserControl
{
    public Foot()
    {
        InitializeComponent();
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
