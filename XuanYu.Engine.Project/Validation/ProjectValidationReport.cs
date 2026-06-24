namespace FluidWarfare.Project.Validation;

/// <summary>
/// 表示一次项目校验报告，汇总项目加载与内容扫描中的校验问题。
/// 只负责保存和提供问题列表的基本查询。
/// </summary>
public sealed record ProjectValidationReport(
    IReadOnlyList<ProjectValidationIssue> Issues)
{
    public bool HasIssues => Issues.Count > 0;

    public int IssueCount => Issues.Count;

    public ProjectValidationIssue? FirstIssue => Issues.Count > 0 ? Issues[0] : null;

    public static ProjectValidationReport Empty { get; } =
        new ProjectValidationReport([]);
}
