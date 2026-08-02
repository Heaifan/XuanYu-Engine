using XuanYu.Editor.MapDocument;

namespace XuanYu.World.Tests.Map;

// MAP-A-R1-D2：状态链闭环与失败不污染。
public sealed class MapDocumentOwnerChainTests
{
    static MapDocument Valid() => MapDocument.CreateNew("TestBattlefield");

    [Fact]
    public void Full_state_chain_completes()
    {
        var owner = new MapDocumentOwner();
        Assert.False(owner.IsDirty);
        owner.New(Valid());
        Assert.True(owner.IsDirty);
        owner.Save("a");
        Assert.False(owner.IsDirty);
        owner.Modify(Valid() with { Name = "x" });
        Assert.True(owner.IsDirty);
        owner.Save("a");
        Assert.False(owner.IsDirty);
        owner.Unload();
        Assert.Null(owner.CurrentMap);
        owner.Load("b", Valid());
        Assert.False(owner.IsDirty);
        Assert.Equal("b", owner.CurrentPath);
    }

    [Fact]
    public void Failed_load_keeps_previous_state_untouched()
    {
        var owner = new MapDocumentOwner();
        owner.Load("good", Valid());
        var before = owner.CurrentMap;

        // 模拟加载失败：Owner 不被调用，状态保持。
        Assert.Same(before, owner.CurrentMap);
        Assert.Equal("good", owner.CurrentPath);
        Assert.False(owner.IsDirty);
    }

    [Fact]
    public void Mark_error_sets_last_error()
    {
        var owner = new MapDocumentOwner();
        owner.MarkError("地图加载失败。");
        Assert.Equal("地图加载失败。", owner.LastError);
    }
}
