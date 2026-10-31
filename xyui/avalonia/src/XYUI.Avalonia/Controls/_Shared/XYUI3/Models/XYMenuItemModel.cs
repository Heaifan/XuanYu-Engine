using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public enum XyuiMenuCheckKind { None, Check, Radio }

public sealed record XYMenuItemModel(
    string Id, string Label, XyuiVectorIcon? Icon = null,
    string Shortcut = "", bool IsEnabled = true,
    bool IsChecked = false, XyuiMenuCheckKind CheckKind = XyuiMenuCheckKind.None,
    bool IsDestructive = false, IReadOnlyList<XYMenuItemModel>? Children = null);
