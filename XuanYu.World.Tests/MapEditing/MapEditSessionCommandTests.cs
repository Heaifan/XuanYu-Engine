using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

// MAP-A-R2-D2：地图基础编辑命令（改名/尺寸/基础高度/No-op/非法输入）。
public sealed class MapEditSessionCommandTests
{
    [Fact]
    public void Rename_succeeds_and_keeps_ids()
    {
        var session = new MapEditSession();
        var mapId = session.CurrentMap.MapId;
        var layerId = session.CurrentMap.Layers[0].LayerId;
        var result = session.RenameMap("测试战场");
        Assert.True(result.IsSuccess);
        Assert.Equal("测试战场", session.CurrentMap.DisplayName);
        Assert.Equal(mapId, session.CurrentMap.MapId);
        Assert.Equal(layerId, session.CurrentMap.Layers[0].LayerId);
        Assert.True(session.IsDirty);
        Assert.True(session.CanUndo);
        Assert.False(session.CanRedo);
    }

    [Fact]
    public void Rename_trims_and_rejects_blank()
    {
        var session = new MapEditSession();
        Assert.True(session.RenameMap("  测试战场  ").IsSuccess);
        Assert.Equal("测试战场", session.CurrentMap.DisplayName);
        Assert.False(session.RenameMap("   ").IsSuccess);
        Assert.Equal("测试战场", session.CurrentMap.DisplayName);
    }

    [Fact]
    public void Rename_same_name_is_noop()
    {
        var session = new MapEditSession();
        session.RenameMap("测试战场");
        var stateId = session.CurrentStateId;
        var sequence = session.ChangeSequence;
        var result = session.RenameMap("测试战场");
        Assert.True(result.IsSuccess);
        Assert.Equal(stateId, session.CurrentStateId);
        Assert.Equal(sequence, session.ChangeSequence);
        Assert.True(session.CanUndo);
    }

    [Fact]
    public void Resize_succeeds_within_bounds()
    {
        var session = new MapEditSession();
        Assert.True(session.ResizeMap(20000.0, 8000.0).IsSuccess);
        Assert.Equal(20000.0, session.CurrentMap.SizeMeters.Width);
        Assert.Equal(8000.0, session.CurrentMap.SizeMeters.Depth);
    }

    [Fact]
    public void Resize_out_of_range_rejected()
    {
        var session = new MapEditSession();
        var result = session.ResizeMap(50.0, 10000.0);
        Assert.True(result.IsFailure);
        Assert.Equal("InvalidMapSize", result.Error!.Value.Code);
        Assert.Equal(10000.0, session.CurrentMap.SizeMeters.Width);
        Assert.False(session.CanUndo);
    }

    [Fact]
    public void Set_base_height_changes_only_surface()
    {
        var session = new MapEditSession();
        var kind = session.CurrentMap.Surface.Kind;
        Assert.True(session.SetBaseHeight(25.0).IsSuccess);
        Assert.Equal(25.0, session.CurrentMap.Surface.BaseHeightMeters);
        Assert.Equal(kind, session.CurrentMap.Surface.Kind);
    }

    [Fact]
    public void Set_base_height_same_value_is_noop()
    {
        var session = new MapEditSession();
        session.SetBaseHeight(25.0);
        var sequence = session.ChangeSequence;
        Assert.True(session.SetBaseHeight(25.0).IsSuccess);
        Assert.Equal(sequence, session.ChangeSequence);
    }
}
