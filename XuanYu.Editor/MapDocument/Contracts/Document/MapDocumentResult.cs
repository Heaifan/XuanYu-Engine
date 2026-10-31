namespace XuanYu.Editor.MapDocument;

// MAP-A-R1-D2：地图操作结构化结果（对齐 SceneDocumentResult 模式，语义独立）。
public sealed record MapDocumentResult<T>(
    bool Succeeded,
    T? Value,
    string ErrorCode,
    string Message,
    string Stage = "",
    string Detail = "")
{
    public static MapDocumentResult<T> Ok(T value) =>
        new(true, value, "", "");

    public static MapDocumentResult<T> Fail(
        string code,
        string message,
        string stage = "",
        string detail = "") =>
        new(false, default, code, message, stage, detail);
}
