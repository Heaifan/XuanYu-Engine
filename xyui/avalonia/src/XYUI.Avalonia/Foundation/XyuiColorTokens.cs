namespace XYUI.Avalonia.Foundation;

// XYUI-0 Foundation 颜色 token 权威表（83 个唯一 id，值转录自 token-canonical-map.json）
public static partial class XyuiColorTokens
{
    private static IReadOnlyList<XyuiColorToken>? _all;

    public static IReadOnlyList<XyuiColorToken> All => _all ??= BuildAll();

    // 延迟构建：partial 静态字段跨文件初始化顺序不可控，首次访问时各家族数组已就绪
    private static XyuiColorToken[] BuildAll() =>
        Core.Concat(Text).Concat(Surface).Concat(Border).Concat(Accent)
            .Concat(State).Concat(Semantic).Concat(Editor)
            .ToArray();

    // 运行时 Brush 资源键：XY.Brush.<canonical_token_id 去 XY. 前缀>
    public static string BrushKey(string tokenId) =>
        "XY.Brush." + (tokenId.StartsWith("XY.") ? tokenId[3..] : tokenId);

    public static bool TryFind(string tokenId, out XyuiColorToken token)
    {
        foreach (var t in All)
        {
            if (t.TokenId == tokenId)
            {
                token = t;
                return true;
            }
        }
        token = default;
        return false;
    }
}
