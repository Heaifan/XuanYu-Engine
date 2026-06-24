using XuanYu.Engine.Project.Validation;

namespace XuanYu.Engine.Project.Content;

/// <summary>
/// 表示内容文件入口扫描结果，包含合法内容文件入口和扫描中的校验问题。
/// </summary>
public sealed record GameContentFileScanResult(
    IReadOnlyList<GameContentFileInfo> ContentFiles,
    IReadOnlyList<ProjectValidationIssue> Issues)
{
    public bool HasIssues => Issues.Count > 0;
}
