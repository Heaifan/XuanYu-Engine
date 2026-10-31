using XYUI.Avalonia.Controls;

namespace XuanYu.Editor.UI;

public partial class HierarchyWorkspace
{
    readonly XYContextMenu _contextMenu = new() { ContextType = "层级" };
    readonly XYMenuItem _addItem = new() { Label = "添加立方体" };
    readonly XYMenuItem _renameItem = new() { Label = "重命名" };
    readonly XYMenuItem _deleteItem = new() { Label = "删除", IsDestructive = true };

    public new XYContextMenu ContextMenu => _contextMenu;

    void InitializeContextMenu()
    {
        _contextMenu.Menu = new XYMenu(_addItem, XYMenu.Separator(), _renameItem, _deleteItem);
        _contextMenu.AttachTo(HierarchyList);
        _contextMenu.Opened += ContextMenu_Opened;
    }

    void ContextMenu_Opened(object? sender, EventArgs e)
    {
        if (DataContext is not UiVm vm) return;
        var node = vm.SelectedHierarchyItem;
        _contextMenu.ContextName = node?.Title ?? "未选择对象";
        _addItem.Command = new RelayCommand(_ => vm.AddCubeEntity());
        _renameItem.Command = new RelayCommand(_ => vm.BeginRenameFromHierarchyContext(), _ => node?.IsEntity == true);
        _deleteItem.Command = new RelayCommand(_ => vm.DeleteSelectedEntity(), _ => node?.IsEntity == true);
    }
}
