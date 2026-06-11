namespace FluidWarfare.Editor.Windows.Panels.Project;

/// <summary>
/// 项目内容目录选择值对象，FolderName 用于稳定查找，DisplayName 只用于显示。
/// </summary>
public sealed record ProjectContentFolderSelection(
    string FolderName,
    string DisplayName,
    string ContentKind);
