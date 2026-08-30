using Avalonia;
using Avalonia.Controls;

namespace XYUI.Avalonia.Controls;

public sealed class XYMenu : Border
{
    IReadOnlyList<Control> _items = [];
    bool _embedded;
    public IReadOnlyList<Control> Items { get => _items; set { _items = value; Build(); } }
    public bool IsEmbedded { get => _embedded; set { _embedded = value; ApplyMode(); } }
    public XYMenu() { Classes.Add("xyui-menu"); Build(); }
    public XYMenu(params Control[] items) : this() => Items = items;
    public static XYSeparator Separator() => new() { Variant = XyuiSeparatorVariant.Section, Classes = { "xyui-menu-separator" } };
    void Build() { var panel = new StackPanel { Classes = { "xyui-menu-items" } }; foreach (var item in Items) panel.Children.Add(item); Child = panel; ApplyMode(); }
    void ApplyMode() { if (_embedded) Classes.Add("xyui-menu-embedded"); else Classes.Remove("xyui-menu-embedded"); }
}
