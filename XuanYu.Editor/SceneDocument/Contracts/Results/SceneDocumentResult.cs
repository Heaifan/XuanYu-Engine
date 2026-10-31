namespace XuanYu.Editor.SceneDocument;

public sealed record SceneDocumentResult<T>(
    bool Succeeded,
    T? Value,
    string ErrorCode,
    string Message,
    string Stage = "",
    string Detail = "")
{
    public static SceneDocumentResult<T> Ok(T value) =>
        new(true, value, "", "");

    public static SceneDocumentResult<T> Fail(
        string code,
        string message,
        string stage = "",
        string detail = "") =>
        new(false, default, code, message, stage, detail);
}
