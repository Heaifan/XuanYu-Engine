using System.Text.Json;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Tests;

// Canonical 对照：运行时 token 表必须与 token-canonical-map.json 逐条一致（唯一真值）
public class CanonicalAlignmentTests
{
    static readonly string MapPath = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "..", "..",
        "tokens", "architecture", "token-canonical-map.json");

    [Fact]
    public void Every_Token_Matches_Canonical_Map()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(MapPath));
        var entries = doc.RootElement.GetProperty("entries");
        foreach (var t in XyuiColorTokens.All)
        {
            var match = entries.EnumerateArray().FirstOrDefault(e =>
                e.GetProperty("canonical_token_id").GetString() == t.TokenId &&
                e.TryGetProperty("value", out var v) && v.ValueKind == JsonValueKind.String);
            Assert.False(match.ValueKind == JsonValueKind.Undefined, $"map 中找不到 {t.TokenId}");
            Assert.Equal($"{t.LightHex}/{t.DarkHex}", match.GetProperty("value").GetString());
        }
    }

    [Fact]
    public void Canonical_Map_Excludes_Unknown_Tokens()
    {
        using var doc = JsonDocument.Parse(File.ReadAllText(MapPath));
        var canonical = doc.RootElement.GetProperty("entries").EnumerateArray()
            .Where(e => e.TryGetProperty("value", out var v)
                        && v.ValueKind == JsonValueKind.String
                        && v.GetString()!.StartsWith('#'))
            .Select(e => e.GetProperty("canonical_token_id").GetString()!)
            .ToHashSet();
        foreach (var t in XyuiColorTokens.All)
        {
            Assert.Contains(t.TokenId, canonical);
        }
    }
}
