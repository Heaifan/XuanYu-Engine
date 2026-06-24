using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Components;

namespace XuanYu.Engine.World;

/// <summary>最小世界状态，支持创建、查询和枚举实体。不读项目文件，不写日志。</summary>
public sealed class WorldState
{
    int _nextEntityValue = 1;
    readonly Dictionary<EntityId, DisplayNameComponent> _displayNames = [];
    readonly Dictionary<EntityId, PositionComponent> _positions = [];
    readonly Dictionary<EntityId, RotationComponent> _rotations = [];
    readonly Dictionary<EntityId, ScaleComponent> _scales = [];
    readonly Dictionary<EntityId, ProjectContentEntitySource> _sources = [];

    public EntityId CreateEntity(string displayName, Vector3d position) =>
        CreateEntity(displayName, position, null);

    public EntityId CreateEntity(string displayName, Vector3d position, ProjectContentEntitySource? source) =>
        CreateEntity(displayName, position, Vector3d.Zero, new Vector3d(1, 1, 1), source);

    public EntityId CreateEntity(string displayName, Vector3d position, Vector3d rotationDegrees, Vector3d scale,
        ProjectContentEntitySource? source = null)
    {
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("实体显示名称不能为空。", nameof(displayName));
        var entityId = EntityId.FromInt(_nextEntityValue); _nextEntityValue++;
        _displayNames[entityId] = new DisplayNameComponent(displayName);
        _positions[entityId] = new PositionComponent(position);
        _rotations[entityId] = new RotationComponent(rotationDegrees);
        _scales[entityId] = new ScaleComponent(scale);
        if (source is not null) _sources[entityId] = source;
        return entityId;
    }

    public bool ContainsEntity(EntityId entityId) => _displayNames.ContainsKey(entityId);

    public WorldEntityInfo? FindEntity(EntityId entityId)
    {
        if (_displayNames.TryGetValue(entityId, out var n))
        { _sources.TryGetValue(entityId, out var s); return new WorldEntityInfo(entityId, n.Value, s); }
        return null;
    }

    public PositionComponent? FindPosition(EntityId entityId) =>
        _positions.TryGetValue(entityId, out var p) ? p : null;

    public RotationComponent? FindRotation(EntityId entityId) =>
        _rotations.TryGetValue(entityId, out var r) ? r : null;

    public ScaleComponent? FindScale(EntityId entityId) =>
        _scales.TryGetValue(entityId, out var s) ? s : null;

    public bool SetPosition(EntityId entityId, Vector3d newPosition)
    {
        if (!_positions.TryGetValue(entityId, out var c)) return false;
        if (c.Value == newPosition) return false;
        _positions[entityId] = new PositionComponent(newPosition); return true;
    }

    public bool SetRotation(EntityId entityId, Vector3d newRotation)
    {
        if (!_rotations.TryGetValue(entityId, out var c)) return false;
        if (c.Value == newRotation) return false;
        _rotations[entityId] = new RotationComponent(newRotation); return true;
    }

    public bool SetScale(EntityId entityId, Vector3d newScale)
    {
        if (!_scales.TryGetValue(entityId, out var c)) return false;
        if (c.Value == newScale) return false;
        _scales[entityId] = new ScaleComponent(newScale); return true;
    }

    public IReadOnlyList<WorldEntityInfo> ListEntities() =>
        _displayNames.Select(kvp => { _sources.TryGetValue(kvp.Key, out var s); return new WorldEntityInfo(kvp.Key, kvp.Value.Value, s); }).ToList();
}
