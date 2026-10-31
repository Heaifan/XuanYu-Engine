namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public void ToggleProjectNode(EditorTreeNode node)
    {
        if (_projectTreePresenter.Toggle(node))
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(ProjectItems)));
    }
    public void ToggleHierarchyNode(EditorTreeNode node)
    {
        if (_projectTreePresenter.ToggleHierarchy(node))
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(HierarchyItems)));
    }

    bool TryRequestFileCommand(string name)
    {
        if (name is not ("新建" or "打开" or "保存" or "另存为" or "导入 GLB"
            or "新建地图" or "打开地图" or "保存地图" or "卸载地图" or "聚焦地图")) return false;
        FileCommandRequested?.Invoke(name);
        return true;
    }
}
