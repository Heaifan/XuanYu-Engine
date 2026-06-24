using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.EntityTransform;
using XuanYu.Engine.World;

namespace XuanYu.Engine.Editor.Windows.Inspector.TransformEdit;

/// <summary>应用 Inspector Transform 编辑请求到 WorldState。校验 Scale > 0 和有限数字。</summary>
public static class SelectedEntityTransformApply
{
    public static TransformEditResult Apply(TransformEditRequest request, WorldState world,
        EditorWorldDirtyState dirty, Action<string>? logInfo, Action<string>? logWarn)
    {
        // 校验 EntityId
        if (string.IsNullOrWhiteSpace(request.EntityId))
            return Failure("未选中实体，无法应用 Transform。");

        if (!int.TryParse(request.EntityId, out var idValue) || idValue <= 0)
            return Failure($"无效的实体编号 \"{request.EntityId}\"。");
        var entityId = EntityId.FromInt(idValue);

        if (world.FindPosition(entityId) is null)
            return Failure($"实体 \"{request.EntityId}\" 不存在，无法应用 Transform。");

        // 校验有限数字
        if (HasInvalid(request.Position))
            return Failure("Position 包含无效数值（NaN 或 Infinity），拒绝写入。");
        if (HasInvalid(request.RotationDegrees))
            return Failure("RotationDegrees 包含无效数值（NaN 或 Infinity），拒绝写入。");
        if (HasInvalid(request.Scale))
            return Failure("Scale 包含无效数值（NaN 或 Infinity），拒绝写入。");

        // 校验 Scale > 0
        if (request.Scale.X <= 0 || request.Scale.Y <= 0 || request.Scale.Z <= 0)
            return Failure($"Scale 必须大于 0（当前 {request.Scale.X}, {request.Scale.Y}, {request.Scale.Z}）。");

        // 写入 WorldState
        var posChanged = world.SetPosition(entityId, request.Position);
        var rotChanged = world.SetRotation(entityId, request.RotationDegrees);
        var scaleChanged = world.SetScale(entityId, request.Scale);

        if (posChanged || rotChanged || scaleChanged)
        {
            dirty.MarkDirty(request.EntityId);
            logInfo?.Invoke($"已更新实体 {request.EntityId} 的 Transform。");
        }

        return new TransformEditResult(true, $"实体 {request.EntityId} Transform 已更新。");
    }

    static bool HasInvalid(Vector3d v) =>
        double.IsNaN(v.X) || double.IsInfinity(v.X) ||
        double.IsNaN(v.Y) || double.IsInfinity(v.Y) ||
        double.IsNaN(v.Z) || double.IsInfinity(v.Z);

    static TransformEditResult Failure(string msg) => new(false, msg);
}
