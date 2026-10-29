using XuanYu.Core.Scene;

namespace XuanYu.World;

internal static class WorldCandidateEntityValidator
{
    internal static void Validate(WorldEntitySnapshot entity)
    {
        if (!entity.EntityKey.IsValid)
            throw WorldInvariantValidator.Invalid("候选实体的 EntityId 必须有效。");
        if (string.IsNullOrWhiteSpace(entity.Name)
            || string.IsNullOrWhiteSpace(entity.Type))
            throw WorldInvariantValidator.Invalid("候选实体的名称和类型不能为空。");
        if (!Enum.IsDefined(entity.Activity))
            throw WorldInvariantValidator.Invalid("候选实体的 Activity 值无效。");

        ValidateTransform(entity.Transform);
        ValidateSpatialBounds(entity);
    }

    static void ValidateTransform(CommittedTransform transform)
    {
        try
        {
            _ = new CommittedTransform(transform.Position, transform.Rotation, transform.Scale);
        }
        catch (ArgumentException)
        {
            throw WorldInvariantValidator.Invalid("候选实体的 Transform 无效。");
        }
    }

    static void ValidateSpatialBounds(WorldEntitySnapshot entity)
    {
        try
        {
            _ = entity.Bounds.WorldBounds;
        }
        catch (ArgumentException)
        {
            throw WorldInvariantValidator.Invalid("候选实体的空间 Bounds 无效。");
        }
    }
}
