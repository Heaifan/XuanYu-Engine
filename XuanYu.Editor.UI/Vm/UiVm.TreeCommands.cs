namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    void ToggleTreeNode(EditorTreeNode node, HashSet<string> collapsed, string propertyName)
    {
        if (!node.CanToggle) return;
        if (!collapsed.Add(node.Key)) collapsed.Remove(node.Key);
        OnPropertyChanged(propertyName);
    }

    bool TryRequestFileCommand(string name)
    {
        if (name is not ("新建" or "打开" or "保存" or "另存为" or "导入 GLB")) return false;
        FileCommandRequested?.Invoke(name);
        return true;
    }
}
