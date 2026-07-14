namespace XuanYu.Editor.UI;

public sealed record EditorToolSnapshot(
    long Revision,
    EditorToolId ActiveTool,
    EditorToolCaptureState CaptureState)
{
    public static EditorToolSnapshot Initial { get; } =
        new(1, EditorToolId.Select, EditorToolCaptureState.None);

    public string ActiveToolText => EditorToolText.ToText(ActiveTool);

    public string CaptureStateText => CaptureState switch
    {
        EditorToolCaptureState.None => "捕获：无",
        _ => throw new ArgumentOutOfRangeException()
    };
}
