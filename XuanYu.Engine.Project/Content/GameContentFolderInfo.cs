namespace XuanYu.Engine.Project.Content;

public sealed record GameContentFolderInfo(
    string FolderName,
    string DisplayName,
    string Description,
    string ContentKind,
    bool IsRequired,
    IReadOnlyList<string> AllowedExtensions);
