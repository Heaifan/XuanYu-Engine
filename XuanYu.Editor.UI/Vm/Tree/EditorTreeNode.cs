using System.ComponentModel;
using System.Runtime.CompilerServices;
using Avalonia;

namespace XuanYu.Editor.UI;

public sealed class EditorTreeNode : INotifyPropertyChanged
{
    public EditorTreeNode(string key, string title, string type, string path, int level, string icon)
    {
        Key = key;
        Update(title, type, path, level, icon);
    }

    public string Key { get; }
    public string Title { get; private set; } = "";
    public string Type { get; private set; } = "";
    public string Path { get; private set; } = "";
    public int Level { get; private set; }
    public string Icon { get; private set; } = "";
    public IReadOnlyList<TreeGuideSegment> GuideSegments { get; private set; } = [];
    public bool HasChildren { get; private set; }
    public bool IsExpanded { get; private set; }
    public bool IsRenaming { get; private set; }
    public string RenameText { get; set; } = "";
    public event PropertyChangedEventHandler? PropertyChanged;

    public void Update(string title, string type, string path, int level, string icon)
    {
        Title = title;
        Type = type;
        Path = path;
        Level = level;
        Icon = icon;
    }

    public Thickness Indent => new(Level * 16, 0, 0, 0);
    public double GuideWidth => Math.Max(0, Level * 20);
    public bool HasConnector => Level > 0;
    public bool IsRoot => Level == 0;
    public bool IsCategory => Type == "分类";
    public bool CanToggle => HasChildren;
    public bool IsCollapsed => HasChildren && !IsExpanded;
    public bool IsRegion => Type == "Region";
    public bool IsWorld => Type == "世界";
    public bool IsAsset => Type == "资源分类";
    public bool IsEntity => Icon == "entity";
    public bool IsCamera => Type == "相机";
    public bool IsGround => Icon == "ground";
    public bool IsFolder => Icon == "folder";
    public bool IsScript => Icon == "script";
    public bool IsBuild => Icon == "build";

    public void SetGuide(IReadOnlyList<TreeGuideSegment> segments) => GuideSegments = segments;

    public void SetTreeState(bool hasChildren, bool isExpanded)
    {
        HasChildren = hasChildren;
        IsExpanded = isExpanded;
    }

    public void BeginRename()
    {
        RenameText = Title;
        IsRenaming = true;
        Changed(nameof(RenameText));
        Changed(nameof(IsRenaming));
    }

    public void EndRename()
    {
        IsRenaming = false;
        Changed(nameof(IsRenaming));
    }

    void Changed([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
