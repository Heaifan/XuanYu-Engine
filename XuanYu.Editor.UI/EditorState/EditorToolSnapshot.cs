namespace XuanYu.Editor.UI;

public sealed record EditorToolSnapshot(
    long Revision,
    EditorToolId ActiveTool,
    bool IsSnapEnabled,
    EditorToolCaptureState CaptureState)
{
    public static EditorToolSnapshot Initial { get; } =
        new(1, EditorToolId.Select, false, EditorToolCaptureState.None);

    public string ActiveToolText => EditorToolText.ToText(ActiveTool);
    public string SnapText => IsSnapEnabled ? "吸附：开启" : "吸附：关闭";

    public string CaptureStateText => CaptureState switch
    {
        EditorToolCaptureState.None => "捕获：无",
        _ => throw new ArgumentOutOfRangeException()
    };
}
