namespace XuanYu.Editor.UI;

// ARCH-UI-SPEC-R1-D4（补充裁决 §四）：MapId 显示压缩——超过 18 字符显示「前 8 + … + 后 6」。
// 只压缩显示层，完整 MapId 由 Tooltip 与复制命令提供；纯逻辑可脱离 GPU 测试。
public static class MapIdDisplayFormat
{
    public const int MaxPlainLength = 18;
    public const int HeadLength = 8;
    public const int TailLength = 6;

    public static string Format(string id) =>
        id.Length > MaxPlainLength ? $"{id[..HeadLength]}…{id[^TailLength..]}" : id;
}
