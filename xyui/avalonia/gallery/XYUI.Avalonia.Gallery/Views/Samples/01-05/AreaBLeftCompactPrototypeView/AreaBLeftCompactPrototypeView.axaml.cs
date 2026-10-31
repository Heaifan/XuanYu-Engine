using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Gallery.Views;

public partial class AreaBLeftCompactPrototypeView : UserControl
{
    public AreaBLeftCompactPrototypeView() { InitializeComponent(); BuildStateBoard(); }

    void BuildStateBoard()
    {
        foreach (var state in Enum.GetValues<PrototypeState>())
        {
            var card = CreateCard(state); StateBoard.Children.Add(card); Grid.SetColumn(card, (int)state);
        }
    }

    Border CreateCard(PrototypeState state)
    {
        var body = new Grid { RowDefinitions = new RowDefinitions("28,1,28,28,*") };
        body.Children.Add(Header()); body.Children.Add(new XYSeparator { Variant = XyuiSeparatorVariant.Header, [Grid.RowProperty] = 1 }); body.Children.Add(LocalSwitch());
        var content = new StackPanel { Spacing = 8 }; Grid.SetRow(content, 3);
        content.Children.Add(new XYText { Text = "当前场景" }); content.Children.Add(TreeState(state)); body.Children.Add(content);
        var card = new Border { Classes = { "prototype-card" }, Child = body }; return card;
    }

    Grid Header() => new()
    {
        ColumnDefinitions = new ColumnDefinitions("*,28"),
        Children = { new XYHeading { Text = "项目", Variant = XyuiHeadingVariant.PanelTitle, VerticalAlignment = VerticalAlignment.Center }, More() }
    };

    XYIconButton More()
    {
        var button = new XYIconButton { Classes = { "prototype-action" } }; ToolTip.SetTip(button, "更多");
        button.Content = new XYIcon { Icon = XyuiVectorIcon.MoreHorizontal, Size = XyuiIconSize.Small }; Grid.SetColumn(button, 1); return button;
    }

    StackPanel LocalSwitch()
    {
        var project = Toggle("项目", true); var file = Toggle("文件", false);
        project.PropertyChanged += (_, e) => { if (e.Property == ToggleButton.IsCheckedProperty && e.GetNewValue<bool?>() == true) file.IsChecked = false; };
        file.PropertyChanged += (_, e) => { if (e.Property == ToggleButton.IsCheckedProperty && e.GetNewValue<bool?>() == true) project.IsChecked = false; };
        return new StackPanel { Orientation = Orientation.Horizontal, [Grid.RowProperty] = 2, Children = { project, file } };
    }

    XYToggleButton Toggle(string text, bool selected) => new()
    {
        Content = text, IsChecked = selected, Classes = { "prototype-toggle" }
    };

    Control TreeState(PrototypeState state)
    {
        var row = TreeRow(state); var list = new ListBox { Classes = { "prototype-tree" }, Width = 200, Height = 28, SelectedIndex = state == PrototypeState.Selected ? 0 : -1 };
        list.Items.Add(new ListBoxItem { Content = row, Padding = new Thickness(0) });
        if (state != PrototypeState.ContextMenu) return list;
        var menu = new XYContextMenu { ContextType = "项目", ContextName = "未命名场景", Width = 196,
            Menu = new XYMenu(new XYMenuItem { Label = "定位到场景" }, new XYMenuItem { Label = "重命名" }, XYMenu.Separator(), new XYMenuItem { Label = "删除", IsDestructive = true }) };
        menu.AttachTo(row); return new StackPanel { Spacing = 8, Children = { list, menu } };
    }

    Border TreeRow(PrototypeState state)
    {
        var row = new Border { Classes = { "prototype-row" } }; if (state == PrototypeState.Hover) row.Classes.Add("prototype-hover"); if (state == PrototypeState.Selected) row.Classes.Add("prototype-selected");
        row.Child = state == PrototypeState.Rename ? RenameRow() : NormalRow(state is PrototypeState.Hover or PrototypeState.Selected); return row;
    }

    Control NormalRow(bool showAction)
    {
        var grid = new Grid { ColumnDefinitions = new ColumnDefinitions("16,16,*,22") };
        grid.Children.Add(new XYIcon { Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Tiny });
        grid.Children.Add(new XYIcon { Icon = XyuiVectorIcon.Browse, Size = XyuiIconSize.Small, [Grid.ColumnProperty] = 1 });
        grid.Children.Add(new XYTruncatedText { Text = "未命名场景", VerticalAlignment = VerticalAlignment.Center, [Grid.ColumnProperty] = 2 });
        if (showAction) grid.Children.Add(More()); return grid;
    }

    Control RenameRow() => new Grid
    {
        ColumnDefinitions = new ColumnDefinitions("16,*,22,22"),
        Children = { new XYIcon { Icon = XyuiVectorIcon.Browse, Size = XyuiIconSize.Small },
            new XYTextField { Text = "未命名场景", Height = 22, Padding = new Thickness(5, 0), [Grid.ColumnProperty] = 1 },
            Action(XyuiVectorIcon.Check, "确认重命名", 2), Action(XyuiVectorIcon.Clear, "取消重命名", 3) }
    };

    XYIconButton Action(XyuiVectorIcon icon, string tip, int column)
    {
        var button = new XYIconButton { Classes = { "prototype-action" }, Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Tiny } }; ToolTip.SetTip(button, tip); Grid.SetColumn(button, column); return button;
    }

    enum PrototypeState { Default, Hover, Selected, Rename, ContextMenu }
}
