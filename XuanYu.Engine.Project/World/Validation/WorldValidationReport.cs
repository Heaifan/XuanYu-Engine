namespace XuanYu.Engine.Project.World.Validation;

/// <summary>World 校验报告。</summary>
public sealed record WorldValidationReport(
    IReadOnlyList<WorldValidationError> Errors)
{
    public bool IsValid => Errors.Count == 0;

    public static readonly WorldValidationReport Empty = new([]);
}
