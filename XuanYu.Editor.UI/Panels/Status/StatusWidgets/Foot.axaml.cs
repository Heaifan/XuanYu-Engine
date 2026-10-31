using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using XYUI.Avalonia.Controls;

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
        _autoScroll.TailStateChanged += atTail => ScrollToBottomButton.IsVisible = !atTail; // D5
        DataContextChanged += (_, _) => HookVm();
        Unloaded += (_, _) => _autoScroll.Dispose();
        BuildSourceMenu();
    }

    // D5：用户离开底部时按钮可见；点击恢复自动跟随并隐藏
    void ScrollToBottom_Click(object? sender, RoutedEventArgs e)
    {
        _autoScroll.ForceFollow();
        ScrollToBottomButton.IsVisible = false;
    }

    void SourceFilter_Click(object? sender, RoutedEventArgs e)
    {
        SourceFilterPopup.PlacementTarget = SourceFilterButton;
        SourceFilterPopup.IsOpen = !SourceFilterPopup.IsOpen;
    }

    void BuildSourceMenu()
    {
        var items = new[] { ("全部", (EditorLogSource?)null), ("编辑器", (EditorLogSource?)EditorLogSource.Editor), ("项目", (EditorLogSource?)EditorLogSource.Project), ("构建", (EditorLogSource?)EditorLogSource.Build), ("任务", (EditorLogSource?)EditorLogSource.Task), ("输入", (EditorLogSource?)EditorLogSource.Input), ("渲染", (EditorLogSource?)EditorLogSource.Render) };
        SourceMenu.Items = items.Select(item => new XYMenuItem { Label = item.Item1, IsChecked = item.Item2 is null, CheckKind = XyuiMenuCheckKind.Check }).ToArray();
        foreach (var item in SourceMenu.Items.OfType<XYMenuItem>()) item.Invoked += (_, _) => SelectSource(item.Label);
    }

    void SelectSource(string label)
    {
        var source = label switch { "编辑器" => EditorLogSource.Editor, "项目" => EditorLogSource.Project, "构建" => EditorLogSource.Build, "任务" => EditorLogSource.Task, "输入" => EditorLogSource.Input, "渲染" => EditorLogSource.Render, _ => (EditorLogSource?)null };
        if (DataContext is UiVm vm) vm.SetLogSource(source);
        SourceFilterPopup.IsOpen = false;
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
