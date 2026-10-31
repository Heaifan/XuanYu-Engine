using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D3：「全部页签」入口（合同 §10.1-8/11）：列出所有真实页签、标明当前项、点击跳转并自动显露。
// 当前架构无页签关闭能力，入口只负责发现与跳转，不扩张关闭系统（D3 执行指令）。
public sealed partial class TopTabStripController
{
    ContextMenu? _allMenu;

    void OpenAllTabs()
    {
        if (_allBtn is null) return;
        _allMenu ??= new ContextMenu();
        var headers = _tabs.Items.OfType<TabItem>().Select(t => t.Header?.ToString() ?? "").ToArray();
        var items = TopTabStripModel.BuildTabList(headers, _tabs.SelectedIndex);
        _allMenu.Items.Clear();
        foreach (var it in items)
        {
            var mi = new MenuItem
            {
                Header = it.Header,
                FontWeight = it.IsSelected ? FontWeight.SemiBold : FontWeight.Regular,
                Foreground = it.IsSelected ? FindAccent() : null,
            };
            var index = it.Index;
            mi.Click += (_, _) => SelectTab(index);
            _allMenu.Items.Add(mi);
        }
        _allMenu.Open(_allBtn);
    }

    static IBrush? FindAccent() =>
        Application.Current?.FindResource("Color.Accent") as IBrush;

    void SelectTab(int index)
    {
        _tabs.SelectedIndex = index;
        EnsureSelectedVisible();
    }
}
