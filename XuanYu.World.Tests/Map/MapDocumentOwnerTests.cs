using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：当前地图状态所有者（New/Load/Modify/Save/Unload 基础状态）。
public sealed class MapDocumentOwnerTests
{
    static MapDocument Valid() => MapDocument.CreateNew("TestBattlefield");

    [Fact]
    public void Initial_state_has_no_map()
    {
        var owner = new MapDocumentOwner();
        Assert.Null(owner.CurrentMap);
        Assert.Null(owner.CurrentPath);
        Assert.False(owner.IsDirty);
    }

    [Fact]
    public void New_map_is_dirty_without_path()
    {
        var owner = new MapDocumentOwner();
        owner.New(Valid());
        Assert.NotNull(owner.CurrentMap);
        Assert.Null(owner.CurrentPath);
        Assert.True(owner.IsDirty);
    }

    [Fact]
    public void Save_marks_clean()
    {
        var owner = new MapDocumentOwner();
        owner.New(Valid());
        owner.Save("/maps/TestBattlefield/map.xymap");
        Assert.False(owner.IsDirty);
        Assert.Equal("/maps/TestBattlefield/map.xymap", owner.CurrentPath);
    }

    [Fact]
    public void Modify_after_save_marks_dirty_again()
    {
        var owner = new MapDocumentOwner();
        owner.New(Valid());
        owner.Save("path");
        var changed = Valid() with { Name = "Renamed" };
        Assert.True(owner.Modify(changed));
        Assert.True(owner.IsDirty);
        Assert.Equal("Renamed", owner.CurrentMap!.Name);
    }

    [Fact]
    public void Modify_without_map_fails()
    {
        var owner = new MapDocumentOwner();
        Assert.False(owner.Modify(Valid()));
        Assert.False(owner.IsDirty);
    }

    [Fact]
    public void Load_is_clean_with_path()
    {
        var owner = new MapDocumentOwner();
        owner.Load("/maps/x/map.xymap", Valid());
        Assert.False(owner.IsDirty);
        Assert.Equal("/maps/x/map.xymap", owner.CurrentPath);
        Assert.NotNull(owner.CurrentMap);
    }

    [Fact]
    public void Unload_clears_everything()
    {
        var owner = new MapDocumentOwner();
        owner.Load("path", Valid());
        owner.Unload();
        Assert.Null(owner.CurrentMap);
        Assert.Null(owner.CurrentPath);
        Assert.False(owner.IsDirty);
    }
}
