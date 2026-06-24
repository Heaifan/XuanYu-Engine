using FluidWarfare.Project.Content;

namespace FluidWarfare.Project.Metadata;

public sealed record GameProjectInfo(
    int SchemaVersion,
    string ProjectId,
    string DisplayName,
    string Description,
    IReadOnlyList<GameContentFolderInfo> ContentFolders,
    IReadOnlyList<GameContentFileInfo> ContentFiles);
