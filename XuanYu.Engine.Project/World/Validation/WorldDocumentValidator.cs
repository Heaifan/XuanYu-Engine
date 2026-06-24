using XuanYu.Engine.Project.World.Documents;

namespace XuanYu.Engine.Project.World.Validation;

/// <summary>World 文档校验器。验证 SchemaVersion、WorldId、EntityId、Transform Position 等。</summary>
public static class WorldDocumentValidator
{
    public static WorldValidationReport Validate(WorldDocument document)
    {
        var errors = new List<WorldValidationError>();

        if (document.SchemaVersion != 1)
            errors.Add(new WorldValidationError("SchemaVersion 必须为 1。"));

        if (string.IsNullOrWhiteSpace(document.WorldId))
            errors.Add(new WorldValidationError("WorldId 不能为空。"));

        if (string.IsNullOrWhiteSpace(document.DisplayName))
            errors.Add(new WorldValidationError("DisplayName 不能为空。"));

        if (document.Entities is null)
        {
            errors.Add(new WorldValidationError("Entities 不能为 null。"));
            return new WorldValidationReport(errors);
        }

        var entityIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var entity in document.Entities)
        {
            if (string.IsNullOrWhiteSpace(entity.EntityId))
                errors.Add(new WorldValidationError("EntityId 不能为空。"));
            else if (!entityIds.Add(entity.EntityId))
                errors.Add(new WorldValidationError($"EntityId \"{entity.EntityId}\" 重复。"));

            if (entity.Components is null) continue;

            foreach (var component in entity.Components)
            {
                if (component is TransformComponentDocument t)
                    ValidateTransformComponent(t, entity.EntityId, errors);
            }
        }

        return new WorldValidationReport(errors);
    }

    static void ValidateTransformComponent(
        TransformComponentDocument t, string entityId, List<WorldValidationError> errors)
    {
        if (t.Position is null)
        {
            errors.Add(new WorldValidationError($"实体 \"{entityId}\" 的 Transform 缺少 Position。"));
            return;
        }

        var (x, y, z) = (t.Position.X, t.Position.Y, t.Position.Z);
        if (float.IsNaN(x) || float.IsInfinity(x) ||
            float.IsNaN(y) || float.IsInfinity(y) ||
            float.IsNaN(z) || float.IsInfinity(z))
            errors.Add(new WorldValidationError($"实体 \"{entityId}\" 的 Position 包含无效数值（NaN 或 Infinity）。"));
    }
}
