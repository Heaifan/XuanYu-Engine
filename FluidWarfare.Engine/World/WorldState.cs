using FluidWarfare.Core.Identity;
using FluidWarfare.Core.Math;
using FluidWarfare.Engine.Components;

namespace FluidWarfare.Engine.World;

/// <summary>
/// 最小世界状态，支持创建、查询和枚举带显示名、位置与可选来源的实体。
/// 不读取项目文件，不写日志，不依赖 Editor。
/// </summary>
public sealed class WorldState
{
    private int _nextEntityValue = 1;
    private readonly Dictionary<EntityId, DisplayNameComponent> _displayNames = [];
    private readonly Dictionary<EntityId, PositionComponent> _positions = [];
    private readonly Dictionary<EntityId, ProjectContentEntitySource> _sources = [];

    /// <summary>
    /// 创建一个新实体（无来源）。
    /// </summary>
    public EntityId CreateEntity(string displayName, Vector3d position)
    {
        return CreateEntity(displayName, position, null);
    }

    /// <summary>
    /// 创建一个新实体（可携带项目内容来源）。
    /// </summary>
    /// <param name="displayName">实体显示名称，不能为空。</param>
    /// <param name="position">实体位置，必须为有效 Vector3d。</param>
    /// <param name="source">可选的项目内容来源信息。</param>
    /// <returns>新创建的实体编号。</returns>
    /// <exception cref="ArgumentException">displayName 为空时抛出。</exception>
    public EntityId CreateEntity(string displayName, Vector3d position, ProjectContentEntitySource? source)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("实体显示名称不能为空。", nameof(displayName));
        }

        var entityId = EntityId.FromInt(_nextEntityValue);
        _nextEntityValue++;

        _displayNames[entityId] = new DisplayNameComponent(displayName);
        _positions[entityId] = new PositionComponent(position);

        if (source is not null)
        {
            _sources[entityId] = source;
        }

        return entityId;
    }

    /// <summary>
    /// 判断实体是否存在。
    /// </summary>
    public bool ContainsEntity(EntityId entityId)
    {
        return _displayNames.ContainsKey(entityId);
    }

    /// <summary>
    /// 读取实体显示信息。
    /// </summary>
    public WorldEntityInfo? FindEntity(EntityId entityId)
    {
        if (_displayNames.TryGetValue(entityId, out var nameComponent))
        {
            _sources.TryGetValue(entityId, out var source);
            return new WorldEntityInfo(entityId, nameComponent.Value, source);
        }

        return null;
    }

    /// <summary>
    /// 读取实体位置。
    /// </summary>
    public PositionComponent? FindPosition(EntityId entityId)
    {
        if (_positions.TryGetValue(entityId, out var position))
        {
            return position;
        }

        return null;
    }

    /// <summary>
    /// 修改实体的位置。相同位置为 NoOp。
    /// </summary>
    /// <param name="entityId">目标实体 ID。</param>
    /// <param name="newPosition">新位置。</param>
    /// <returns>位置是否实际变化。</returns>
    public bool SetPosition(EntityId entityId, Vector3d newPosition)
    {
        if (!_positions.TryGetValue(entityId, out var current))
            return false; // Entity not found

        if (current.Value == newPosition)
            return false; // NoOp — same position

        _positions[entityId] = new PositionComponent(newPosition);
        return true;
    }

    /// <summary>
    /// 枚举所有实体。
    /// </summary>
    public IReadOnlyList<WorldEntityInfo> ListEntities()
    {
        return _displayNames
            .Select(kvp =>
            {
                _sources.TryGetValue(kvp.Key, out var source);
                return new WorldEntityInfo(kvp.Key, kvp.Value.Value, source);
            })
            .ToList();
    }
}
