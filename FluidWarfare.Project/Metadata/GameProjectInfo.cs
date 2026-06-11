namespace FluidWarfare.Project.Metadata;

public sealed record GameProjectInfo(
    string ProjectId,
    string DisplayName,
    string Description,
    IReadOnlyList<string> ContentFolders);
