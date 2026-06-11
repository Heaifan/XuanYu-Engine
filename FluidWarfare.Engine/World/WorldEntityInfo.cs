using FluidWarfare.Core.Identity;

namespace FluidWarfare.Engine.World;

/// <summary>
/// 表示 World 中一个实体的最小可显示信息。
/// 只保存 EntityId 和显示名，不保存组件，不操作 World。
/// </summary>
public sealed record WorldEntityInfo(
    EntityId EntityId,
    string DisplayName);
