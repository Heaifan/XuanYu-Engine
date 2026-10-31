using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Metadata;
using System.Collections.ObjectModel;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYPager : Border
{
    readonly StackPanel _tabs = new() { Orientation = Orientation.Horizontal, Spacing = 2 };
    readonly StackPanel _content = new();
    readonly TextBlock _indicator = new() { HorizontalAlignment = HorizontalAlignment.Center };
    int _selectedIndex = -1;
    public ObservableCollection<XYPagerPage> Pages { get; } = [];
    [Content] public IList<XYPagerPage> Items => Pages;
    public string? SelectedId => PageAt(_selectedIndex)?.Id;
    public int SelectedIndex => _selectedIndex;
    public bool IsPreviousEnabled => _selectedIndex > 0;
    public bool IsNextEnabled => _selectedIndex >= 0 && _selectedIndex < Pages.Count - 1;
    public event EventHandler<XYPagerPage>? SelectionChanged;

    public XYPager() : this(Array.Empty<XYPagerPage>()) { }
    public XYPager(params XYPagerPage[] pages)
    {
        Classes.Add("xyui-pager"); Pages.CollectionChanged += (_, _) => RebuildPages();
        foreach (var page in pages) Pages.Add(page); Child = Build(); if (Pages.Count > 0) Select(0, false);
    }

    Grid Build()
    {
        var root = new Grid { RowDefinitions = new RowDefinitions("Auto,Auto,*") };
        var header = new Grid { ColumnDefinitions = new ColumnDefinitions("32,*,32") };
        var previous = Action(XyuiVectorIcon.ChevronLeft, "xyui-pager-previous");
        var next = Action(XyuiVectorIcon.ChevronRight, "xyui-pager-next");
        previous.Click += (_, _) => Previous(); next.Click += (_, _) => Next();
        header.Children.Add(previous); Grid.SetColumn(previous, 0);
        header.Children.Add(_tabs); Grid.SetColumn(_tabs, 1);
        header.Children.Add(next); Grid.SetColumn(next, 2);
        root.Children.Add(header); Grid.SetRow(header, 0);
        root.Children.Add(_indicator); Grid.SetRow(_indicator, 1);
        root.Children.Add(_content); Grid.SetRow(_content, 2); return root;
    }

    static XYIconButton Action(XyuiVectorIcon icon, string @class) => new()
    { Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small }, Classes = { @class } };
    XYPagerPage? PageAt(int index) => index >= 0 && index < Pages.Count ? Pages[index] : null;
    void RebuildPages() { if (Pages.Count == 0) { _selectedIndex = -1; return; } Select(Math.Clamp(_selectedIndex < 0 ? 0 : _selectedIndex, 0, Pages.Count - 1), false); }
}
