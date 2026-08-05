using XuanYu.Editor.MapEditing;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.MapEditing;

// MAP-A-R2-D2：默认会话与根状态合同。
public sealed class MapEditSessionCreationTests
{
    [Fact]
    public void Default_session_produces_valid_complete_map()
    {
        var map = new MapEditSession().CurrentMap;
        Assert.True(MapDefinitionValidator.Validate(map).Succeeded);
        Assert.Equal(10000.0, map.SizeMeters.Width);
        Assert.Equal(10000.0, map.SizeMeters.Depth);
        Assert.Equal(MapSurfaceKinds.Flat, map.Surface.Kind);
        Assert.Equal(3, map.Layers.Length);
        Assert.True(map.Regions.IsEmpty);
    }

    [Fact]
    public void Default_session_state_contract()
    {
        var session = new MapEditSession();
        Assert.Equal(MapSelectionKind.Map, session.Selection.Kind);
        Assert.Null(session.CurrentFilePath);
        Assert.Null(session.SavedStateId);
        Assert.True(session.IsDirty);
        Assert.False(session.CanUndo);
        Assert.False(session.CanRedo);
        Assert.Equal(0, session.ChangeSequence);
    }

    [Fact]
    public void Initial_map_is_respected()
    {
        var custom = MapDefaultDefinition.CreateDefault() with { DisplayName = "自定义地图" };
        var session = new MapEditSession(custom);
        Assert.Equal("自定义地图", session.CurrentMap.DisplayName);
        Assert.True(session.IsDirty);
    }
}
