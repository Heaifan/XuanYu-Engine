using FluidWarfare.Core.Identity;

namespace FluidWarfare.Engine.World;

/// <summary>
/// 表示 World 中一个实体的最小可显示信息。
/// 保存 EntityId、显示名与可选项目内容来源。
/// 不保存组件，不操作 World，不读取文件。
/// </summary>
public sealed record WorldEntityInfo(
    EntityId EntityId,
    string DisplayName,
    ProjectContentEntitySource? Source);
