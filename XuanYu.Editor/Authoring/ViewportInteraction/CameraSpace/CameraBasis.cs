using XuanYu.Core.Math;

namespace XuanYu.Editor.Camera;

// F3-F2：唯一相机正交基生成器（Editor 相机规则；不进入 Core，不持有 UiVm/Vulkan）。
// 输入 Position/ObservationCenter/PreferredUp；输出 Forward/Right/Up 或明确失败原因。
// 手性与 CameraState 合同一致：Right = Forward×ReferenceUp，Up = Right×Forward（+Z Up 右手系）。
public static class CameraBasis
{
    const double ParallelLimit = 0.98;
    const double MinLength = 1e-9;

    public static bool TryCreate(Vector3d position, Vector3d observationCenter, Vector3d preferredUp,
        out Vector3d forward, out Vector3d right, out Vector3d up, out string failureReason)
    {
        forward = default; right = default; up = default; failureReason = "";
        var raw = observationCenter - position;
        if (!IsFinite(raw) || raw.Length <= MinLength)
        {
            failureReason = "观察方向为零或非有限（位置与观察中心重合）";
            return false;
        }

        forward = raw.Normalize();
        var reference = ResolveReferenceUp(forward, preferredUp);
        right = forward.Cross(reference).Normalize();
        up = right.Cross(forward).Normalize();
        if (!IsFinite(right) || !IsFinite(up))
        {
            failureReason = "正交化结果非有限";
            return false;
        }

        return true;
    }

    static Vector3d ResolveReferenceUp(Vector3d forward, Vector3d preferredUp)
    {
        if (IsFinite(preferredUp) && preferredUp.Length > MinLength)
        {
            var normalized = preferredUp.Normalize();
            if (System.Math.Abs(normalized.Dot(forward)) < ParallelLimit) return normalized;
        }

        // 回退：从世界轴选择与 Forward 最不平行的一条（可靠处理顶视/底视/极端 Orbit）。
        var best = Vector3d.UnitZ;
        var bestAbsDot = 1.0;
        foreach (var axis in new[] { Vector3d.UnitZ, Vector3d.UnitY, Vector3d.UnitX })
        {
            var dot = System.Math.Abs(axis.Dot(forward));
            if (dot < bestAbsDot)
            {
                bestAbsDot = dot;
                best = axis;
            }
        }

        return best;
    }

    static bool IsFinite(Vector3d vector) =>
        double.IsFinite(vector.X) && double.IsFinite(vector.Y) && double.IsFinite(vector.Z);
}
