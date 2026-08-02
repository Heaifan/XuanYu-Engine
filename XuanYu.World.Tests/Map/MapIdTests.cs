using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：MapId 与地图合同校验（纯内存）。
public sealed class MapIdTests
{
    [Fact]
    public void New_map_id_is_32_hex_digits()
    {
        var id = MapId.New();
        Assert.Equal(32, id.Value.Length);
        Assert.True(id.Value.All(Uri.IsHexDigit));
        Assert.True(id.IsValid);
    }

    [Fact]
    public void Two_new_map_ids_differ()
    {
        Assert.NotEqual(MapId.New(), MapId.New());
    }

    [Fact]
    public void TryParse_accepts_32_hex()
    {
        Assert.True(MapId.TryParse("21e4a2d34d4a4a1eb2539eac76d412a8", out var id));
        Assert.True(id.IsValid);
    }

    [Fact]
    public void TryParse_rejects_wrong_length()
    {
        Assert.False(MapId.TryParse("21e4a2d3", out _));
        Assert.False(MapId.TryParse("21e4a2d34d4a4a1eb2539eac76d412a8ff", out _));
    }

    [Fact]
    public void TryParse_rejects_non_hex_chars()
    {
        Assert.False(MapId.TryParse("zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz", out _));
    }

    [Fact]
    public void TryParse_rejects_blank()
    {
        Assert.False(MapId.TryParse("", out _));
        Assert.False(MapId.TryParse(null, out _));
    }

    [Fact]
    public void CreateNew_map_keeps_its_id()
    {
        var doc = MapDocument.CreateNew("TestBattlefield");
        Assert.True(doc.MapId.IsValid);
        Assert.Equal(32, doc.MapId.Value.Length);
    }
}
