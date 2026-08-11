namespace XuanYu.Editor.Layering;

public sealed record EditorLayerItem(
    string Id,
    string Name,
    string Kind,
    int Order,
    bool IsVisible,
    bool IsLocked,
    bool IsActive,
    bool CanRename,
    bool CanDelete,
    bool CanReorder);

public readonly record struct EditorLayerCommandResult(bool IsSuccess, string Message)
{
    public static EditorLayerCommandResult Success() => new(true, "");
    public static EditorLayerCommandResult Failure(string message) => new(false, message);
}
