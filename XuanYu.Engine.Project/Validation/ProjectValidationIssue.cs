namespace FluidWarfare.Project.Validation;

/// <summary>
/// 表示项目校验中的一个问题。
/// 只保存校验问题，不读取文件，不写日志，不依赖 Editor。
/// </summary>
public sealed record ProjectValidationIssue(
    string Code,
    string Message,
    string Path);
