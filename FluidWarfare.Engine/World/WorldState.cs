using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Engine.Components;

namespace FluidWarfare.Engine.World;

/// <summary>最小世界状态，支持创建、查询和枚举实体。不读项目文件，不写日志。</summary>
public sealed class WorldState
{
    int _nextEntityValue = 1;
    readonly Dictionary<EntityId, DisplayNameComponent> _displayNames = [];
    readonly Dictionary<EntityId, PositionComponent> _positions = [];
    readonly Dictionary<EntityId, ProjectContentEntitySource> _sources = [];

    public EntityId CreateEntity(string displayName, Vector3d position) => CreateEntity(displayName, position, null);

    public EntityId CreateEntity(string displayName, Vector3d position, ProjectContentEntitySource? source)
    {
        if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("实体显示名称不能为空。", nameof(displayName));
        var entityId = EntityId.FromInt(_nextEntityValue); _nextEntityValue++;
        _displayNames[entityId] = new DisplayNameComponent(displayName);
        _positions[entityId] = new PositionComponent(position);
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

    public bool SetPosition(EntityId entityId, Vector3d newPosition)
    {
        if (!_positions.TryGetValue(entityId, out var c)) return false;
        if (c.Value == newPosition) return false;
        _positions[entityId] = new PositionComponent(newPosition); return true;
    }

    public IReadOnlyList<WorldEntityInfo> ListEntities() =>
        _displayNames.Select(kvp => { _sources.TryGetValue(kvp.Key, out var s); return new WorldEntityInfo(kvp.Key, kvp.Value.Value, s); }).ToList();
}
