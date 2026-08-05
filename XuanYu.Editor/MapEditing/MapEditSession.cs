using XuanYu.Core.History;
using XuanYu.Core.Results;
using XuanYu.World.Map;

namespace XuanYu.Editor.MapEditing;

// MAP-A-R2-D2：地图编辑会话（唯一状态权威）。
// CurrentMap 是唯一地图内容；历史直接复用 Core.EditorHistoryOwner（通用历史，方案 A）。
// CurrentStateId=历史游标（Undo 可回退到旧节点）；ChangeSequence 单调递增（事件/去重，不可回退）。
// IsDirty = 无文件路径 或 无保存点 或 当前状态 != 保存状态（D2 合同）。
public sealed partial class MapEditSession
{
    readonly EditorHistoryOwner _history = new();
    readonly Func<bool> _isWriteThread;
    MapDefinition _currentMap;
    string? _currentPath;
    long? _savedStateId;
    long _changeSequence;
    MapSelection _selection = MapSelection.Map;

    public MapEditSession(MapDefinition? initialMap = null, Func<bool>? isWriteThread = null)
    {
        _currentMap = initialMap ?? MapDefaultDefinition.CreateDefault();
        _isWriteThread = isWriteThread ?? (() => true);
        _activeRegionLayerId = FirstRegionLayerId(_currentMap);
    }

    public MapDefinition CurrentMap => _currentMap;

    static MapLayerId FirstRegionLayerId(MapDefinition map) =>
        MapLayerStack.RegionLayers(map.Layers).FirstOrDefault()?.LayerId ?? MapLayerId.New();

    // D4：活动图层（会话临时状态）——显式字段避免构造期触发事件。
    MapLayerId _activeRegionLayerId;
    public MapLayerId ActiveRegionLayerId => _activeRegionLayerId;

    public long CurrentStateId => _history.CurrentRevision;

    public long? SavedStateId => _savedStateId;

    public long ChangeSequence => _changeSequence;

    public string? CurrentFilePath => _currentPath;

    public MapSelection Selection => _selection;

    public bool CanUndo => _history.Count > 0;

    public bool CanRedo => _history.RedoCount > 0;

    public bool IsDirty =>
        CurrentFilePath is null || SavedStateId is null || CurrentStateId != SavedStateId;

    public event Action<MapContentChangedEventArgs>? ContentChanged;

    public event Action<MapSelectionChangedEventArgs>? SelectionChanged;

    public event Action<MapDirtyChangedEventArgs>? DirtyChanged;

    public event Action<MapHistoryAvailabilityChangedEventArgs>? HistoryAvailabilityChanged;

    static EngineResult Ok() => EngineResult.Success();

    static EngineResult Fail(string code, string message) =>
        EngineResult.Fail(EngineError.Create(code, message));

    bool GuardWriteThread() => _isWriteThread();
}
