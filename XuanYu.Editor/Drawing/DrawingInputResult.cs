namespace XuanYu.Editor.Drawing;

public sealed record DrawingInputResult(
    DrawingInputDisposition Disposition,
    string Code,
    string? UserMessage,
    bool SessionCanContinue,
    RecoveryPolicy? Recovery)
{
    public static DrawingInputResult Accepted(string code = "Accepted") =>
        new(DrawingInputDisposition.Accepted, code, null, true, null);

    public static DrawingInputResult Rejected(string code, string? userMessage = null) =>
        new(DrawingInputDisposition.Rejected, code, userMessage, true, null);

    public static DrawingInputResult Blocked(
        string code, string? userMessage, RecoveryPolicy recovery) =>
        new(DrawingInputDisposition.Blocked, code, userMessage, false, recovery);
}
