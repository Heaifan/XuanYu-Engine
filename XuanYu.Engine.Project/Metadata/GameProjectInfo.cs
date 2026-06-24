using XuanYu.Engine.Project.Content;

namespace XuanYu.Engine.Project.Metadata;

public sealed record GameProjectInfo(
    int SchemaVersion,
    string ProjectId,
    string DisplayName,
    string Description,
    IReadOnlyList<GameContentFolderInfo> ContentFolders,
    IReadOnlyList<GameContentFileInfo> ContentFiles);
