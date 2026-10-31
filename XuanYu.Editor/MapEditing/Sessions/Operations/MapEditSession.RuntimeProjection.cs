using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

public sealed partial class MapEditSession
{
    public EngineResult ApplyRuntimeLayerProjection(MapDefinition candidate)
    {
        if (!GuardWriteThread()) return Fail("NotOnWriteThread", "更新运行时图层必须在编辑写线程执行。");
        if (MapDefinitionValidator.Validate(candidate) is { Succeeded: false } validation)
            return Fail("InvalidMap", validation.Message);
        RebuildRegionSpatialIndex(candidate);
        _geometrySpatialIndex.Rebuild(candidate);
        _currentMap = candidate;
        _changeSequence++;
        NormalizeSelection();
        NormalizeActiveLayer();
        RaiseContentChanged(MapEditReason.RuntimeProjection);
        return Ok();
    }
}
