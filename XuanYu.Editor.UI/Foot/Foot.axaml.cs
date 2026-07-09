using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;

namespace XuanYu.Editor.UI;

// LOG-UX-2：Foot.axaml.cs 只做接线——创建自动滚动 controller、日志选中、Ctrl+A/Ctrl+C。
// 自动滚动状态机已拆入 LogListAutoScrollController.cs（节流 + 防重入 + 单次解析）。
public partial class Foot : UserControl
{
    readonly LogListAutoScrollController _autoScroll;
    bool _vmHooked;

    public Foot()
    {
        InitializeComponent();
        _autoScroll = new LogListAutoScrollController(LogList);
        DataContextChanged += (_, _) => HookVm();
        Unloaded += (_, _) => _autoScroll.Dispose();
    }

    void HookVm()
    {
        if (DataContext is UiVm vm && !_vmHooked)
        {
            vm.PropertyChanged += OnVmPropertyChanged;
            _vmHooked = true;
        }
    }

    void OnVmPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(UiVm.LogItems))
            _autoScroll.OnLogItemsChanged();
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
