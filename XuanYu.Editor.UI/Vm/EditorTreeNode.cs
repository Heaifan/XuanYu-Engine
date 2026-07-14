using Avalonia;

namespace XuanYu.Editor.UI;

public sealed record EditorTreeNode(
    string Key,
    string Title,
    string Type,
    string Path,
    int Level,
    string Icon)
{
    public Thickness Indent => new(Level * 16, 0, 0, 0);
    public bool IsRoot => Level == 0;
    public bool IsCategory => Type == "分类";
    public bool IsWorld => Type == "世界";
    public bool IsAsset => Type == "资源分类";
    public bool IsEntity => Type == "实体";
    public bool IsCamera => Type == "相机";
    public bool IsFolder => Icon == "folder";
}
