using Avalonia.Media;

namespace XYUI.Avalonia.Foundation;

// Canonical 颜色 token 记录：token id + Light/Dark 成对值（来源 token-canonical-map.json）
public readonly record struct XyuiColorToken(string TokenId, string LightHex, string DarkHex)
{
    // 从 canonical 的 "LIGHT/DARK" 字面量解析
    public static XyuiColorToken Parse(string tokenId, string pair)
    {
        var sep = pair.IndexOf('/');
        if (sep <= 0 || sep >= pair.Length - 1)
            throw new ArgumentException($"非法颜色对: {pair}", nameof(pair));
        return new XyuiColorToken(tokenId, pair[..sep], pair[(sep + 1)..]);
    }

    public string Hex(bool dark) => dark ? DarkHex : LightHex;

    public Color ToColor(bool dark) => Color.Parse(Hex(dark));
}
