using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;

namespace XuanYu.Editor.UI;

// LOG-UX-2：Foot.axaml.cs 只做接线——自动滚动 controller、日志选中、Ctrl+A/Ctrl+C。
// Ctrl 快捷键走 Foot 隧道路由，避免多选后焦点落在子控件导致 ListBox 局部 KeyDown 收不到。
public partial class Foot : UserControl
{
    readonly LogListAutoScrollController _autoScroll;
    bool _vmHooked;

    public Foot()
    {
        InitializeComponent();
        AddHandler(KeyDownEvent, Foot_KeyDown, RoutingStrategies.Tunnel);
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
        if (e.PropertyName?.StartsWith("IsLogFilter") == true)
            _autoScroll.ForceFollow(); // F2：切换日志分类 → 定位到该分类最新一条
    }

    void LogList_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox lb && DataContext is UiVm vm)
            vm.SetSelectedEntries(lb.SelectedItems?.OfType<LogEntry>().ToArray() ?? []);
    }

    async void Foot_KeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not UiVm vm || !vm.IsLogOpen) return;
        if (!e.KeyModifiers.HasFlag(KeyModifiers.Control)) return;
        if (e.Key == Key.A) { LogList.SelectAll(); e.Handled = true; return; }
        if (e.Key != Key.C || !vm.HasSelectedEntries) return;
        if (await CopySelectedLogs(vm)) e.Handled = true;
    }

    async Task<bool> CopySelectedLogs(UiVm vm)
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null) return false;
        try { await clipboard.SetTextAsync(vm.SelectedEntriesClipboardText); }
        catch (Exception ex) { Debug.WriteLine($"[LogList] copy failed: {ex}"); return false; }
        vm.NotifyLogCopied();
        return true;
    }
}
