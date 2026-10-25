namespace XuanYu.Render.Abstractions;

public static class ScaleIndicatorGlyphLite
{
    public const int MaxGlyphs = 8;
    public const int Space = 13;

    public static int Encode(char glyph) => glyph switch
    {
        >= '0' and <= '9' => glyph - '0',
        'm' => 10,
        'k' => 11,
        '.' => 12,
        ' ' => Space,
        _ => Space
    };

    public static int EncodeLabel(string label, Span<int> destination)
    {
        var length = Math.Min(Math.Min(label.Length, destination.Length), MaxGlyphs);
        for (var i = 0; i < length; i++) destination[i] = Encode(label[i]);
        for (var i = length; i < destination.Length; i++) destination[i] = Space;
        return length;
    }
}
