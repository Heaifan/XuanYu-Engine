namespace XuanYu.Editor.UI;

public sealed record MapDatasetRow(string Name, string Type, string Id, string Status, string Source, bool IsSelected = false,
    bool IsVisible = true, bool IsLocked = false, int Order = 0)
{
    public string VisibilityActionText => IsVisible ? "隐藏" : "显示";
    public string LockActionText => IsLocked ? "解锁" : "锁定";
    public string TypeDisplay => MapDatasetTypePresentation.Display(Type);
    public string TypeIdText => $"{TypeDisplay} · {Id}";
}
