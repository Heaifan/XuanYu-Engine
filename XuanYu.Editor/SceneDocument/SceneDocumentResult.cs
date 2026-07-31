namespace XuanYu.Editor.SceneDocument;

public sealed record SceneDocumentResult<T>(
    bool Succeeded,
    T? Value,
    string ErrorCode,
    string Message)
{
    public static SceneDocumentResult<T> Ok(T value) =>
        new(true, value, "", "");

    public static SceneDocumentResult<T> Fail(string code, string message) =>
        new(false, default, code, message);
}
