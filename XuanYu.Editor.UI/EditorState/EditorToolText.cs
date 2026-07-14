namespace XuanYu.Editor.UI;

public static class EditorToolText
{
    public static EditorToolId FromText(string? text) => text switch
    {
        null or "" or "选择" => EditorToolId.Select,
        "框选" => EditorToolId.BoxSelect,
        "移动" => EditorToolId.Move,
        "旋转" => EditorToolId.Rotate,
        "缩放" => EditorToolId.Scale,
        "聚焦" => EditorToolId.Focus,
        "平移" => EditorToolId.Pan,
        "环绕" => EditorToolId.Orbit,
        "吸附" => EditorToolId.Snap,
        _ => throw new ArgumentException($"未知工具：{text}")
    };

    public static string ToText(EditorToolId tool) => tool switch
    {
        EditorToolId.Select => "选择",
        EditorToolId.BoxSelect => "框选",
        EditorToolId.Move => "移动",
        EditorToolId.Rotate => "旋转",
        EditorToolId.Scale => "缩放",
        EditorToolId.Focus => "聚焦",
        EditorToolId.Pan => "平移",
        EditorToolId.Orbit => "环绕",
        EditorToolId.Snap => "吸附",
        _ => throw new ArgumentOutOfRangeException(nameof(tool), tool, "未知工具")
    };
}
