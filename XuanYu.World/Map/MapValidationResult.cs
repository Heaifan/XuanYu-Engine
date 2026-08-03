namespace XuanYu.World.Map;

// MAP-A-R2-D1-F1：地图领域验证结构化结果（不抛出来源不明的异常）。
public sealed record MapValidationResult(
    bool Succeeded,
    string ErrorCode,
    string Message,
    string Detail = "")
{
    public static MapValidationResult Ok() => new(true, "", "");

    public static MapValidationResult Fail(string code, string message, string detail = "") =>
        new(false, code, message, detail);
}
