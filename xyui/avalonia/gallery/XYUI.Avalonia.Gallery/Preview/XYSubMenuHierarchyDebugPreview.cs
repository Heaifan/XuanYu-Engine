using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Gallery;

public sealed class XYSubMenuHierarchyDebugPreview : Border
{
    readonly XYSubMenu _root = Edge("父级", "子级");
    readonly XYSubMenu _child = Edge("子级", "孙级");
    readonly XYSubMenu _grandchild = Edge("孙级", "曾孙级");
    readonly TextBlock _status = new();

    public XYSubMenuHierarchyDebugPreview()
    {
        _child.ParentSubMenu = _root; _grandchild.ParentSubMenu = _child;
        _root.Opened += Sync; _root.Closed += Sync; _child.Opened += Sync; _child.Closed += Sync;
        _grandchild.Opened += Sync; _grandchild.Closed += Sync; _root.Close();
        var actions = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6 };
        actions.Children.Add(Action("打开父级", _root.Open)); actions.Children.Add(Action("打开子级", _child.Open)); actions.Children.Add(Action("打开孙级", _grandchild.Open));
        actions.Children.Add(Action("关闭子级", _child.Close)); actions.Children.Add(Action("关闭祖先", _root.Close)); actions.Children.Add(Action("关闭孙级", _grandchild.Close));
        Child = new StackPanel { Spacing = 8, Children = { new XYCaption { Text = "Hierarchy Runtime · 父 / 子 / 孙 生命周期" }, actions, _status, _root, _child, _grandchild } };
        Sync();
    }
    static XYSubMenu Edge(string parent, string child) => new() { ParentMenu = Menu(parent), ChildMenu = Menu(child) };
    static XYMenu Menu(string label) => new(new XYMenuItem { Label = label });
    static XYButton Action(string label, Action action) { var button = new XYButton { Content = label }; button.Click += (_, _) => action(); return button; }
    void Sync(object? sender = null, EventArgs? e = null)
    {
        _child.IsVisible = _root.EffectiveVisible; _grandchild.IsVisible = _child.EffectiveVisible;
        _status.Text = $"父级={Flag(_root.EffectiveVisible)}  子级={Flag(_child.EffectiveVisible)}  孙级={Flag(_grandchild.EffectiveVisible)}";
    }
    static string Flag(bool value) => value ? "Visible" : "Hidden";
}
