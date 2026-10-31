using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Metadata;

namespace XYUI.Avalonia.Controls;

public sealed partial class XYMenuBar : Border
{
    readonly AvaloniaList<XYMenuBarItem> _items = [];

    public static readonly StyledProperty<bool> ShowDividerProperty =
        AvaloniaProperty.Register<XYMenuBar, bool>(nameof(ShowDivider), true);

    public bool ShowDivider
    {
        get => GetValue(ShowDividerProperty);
        set => SetValue(ShowDividerProperty, value);
    }

    [Content]
    public IList<XYMenuBarItem> Items
    {
        get => _items;
        set { _items.Clear(); if (value is not null) _items.AddRange(value); }
    }

    public string? OpenMenuId { get; private set; }
    public XYMenu? OpenMenu { get; private set; }

    public XYMenuBar()
    {
        Classes.Add("xyui-menu-bar");
        _items.CollectionChanged += (_, _) => Build();
        Build();
        InitializeInteraction();
    }

    public XYMenuBar(params XYMenuBarItem[] items) : this() => _items.AddRange(items);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ShowDividerProperty) Build();
    }

    void Build()
    {
        var items = new StackPanel { Orientation = Orientation.Horizontal, Classes = { "xyui-menu-bar-items" } };
        foreach (var item in _items)
        {
            if (item.Parent is Panel p) p.Children.Remove(item);
            item.Activated -= OnItemActivated; item.Activated += OnItemActivated;
            item.PointerEntered -= OnItemPointerEntered; item.PointerEntered += OnItemPointerEntered;
            items.Children.Add(item);
        }
        if (Child is Panel oldChild) oldChild.Children.Clear();
        if (ShowDivider)
        {
            var grid = new Grid { RowDefinitions = new RowDefinitions("Auto,1") };
            grid.Children.Add(items);
            var divider = new XYSeparator { Variant = XyuiSeparatorVariant.Header };
            grid.Children.Add(divider); Grid.SetRow(divider, 1); Child = grid;
        }
        else Child = items;
    }
}
