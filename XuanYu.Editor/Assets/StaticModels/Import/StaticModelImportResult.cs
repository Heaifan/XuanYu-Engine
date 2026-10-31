namespace XuanYu.Editor.Assets;

public sealed record StaticModelImportResult(
    bool Succeeded,
    StaticModelData? Model,
    StaticModelImportErrorCode ErrorCode,
    string UserMessage,
    string TechnicalDetail)
{
    public static StaticModelImportResult Success(StaticModelData model) =>
        new(true, model, StaticModelImportErrorCode.None, "", "");

    public static StaticModelImportResult Fail(
        StaticModelImportErrorCode code,
        string message,
        string detail = "") =>
        new(false, null, code, message, detail);
}
