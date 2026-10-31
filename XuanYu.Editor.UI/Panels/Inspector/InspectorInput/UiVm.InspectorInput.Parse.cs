using System.Globalization;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    static bool TryParseInspectorNumber(string text, out double value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;
        return (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ||
            double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)) &&
            double.IsFinite(value);
    }

    static bool TryBuildInspectorTransform(
        CommittedTransform current,
        string group,
        string axis,
        double value,
        out CommittedTransform next,
        out string error)
    {
        next = current;
        error = "";
        var normalizedAxis = axis.Trim().ToUpperInvariant();
        if (normalizedAxis is not ("X" or "Y" or "Z"))
        {
            error = "检查器提交失败：轴必须是 X、Y 或 Z。";
            return false;
        }

        try
        {
            next = BuildInspectorTransform(current, group.Trim(), normalizedAxis, value);
        }
        catch (ArgumentOutOfRangeException)
        {
            error = "检查器提交失败：缩放必须为正数且不能接近 0。";
            return false;
        }

        if (next == current && group.Trim() is not ("位置" or "旋转" or "缩放"))
        {
            error = "检查器提交失败：字段必须是位置、旋转或缩放。";
            return false;
        }

        return true;
    }

    static CommittedTransform BuildInspectorTransform(
        CommittedTransform current,
        string group,
        string axis,
        double value) =>
        group switch
        {
            "位置" => current.WithPosition(ReplaceAxis(current.Position, axis, value)),
            "旋转" => current.WithRotation(ReplaceAxis(current.Rotation, axis, value)),
            "缩放" => current.WithScale(ReplaceAxis(current.Scale, axis, value)),
            _ => current
        };

    static Vector3d ReplaceAxis(Vector3d value, string axis, double next) =>
        axis switch
        {
            "X" => new Vector3d(next, value.Y, value.Z),
            "Y" => new Vector3d(value.X, next, value.Z),
            _ => new Vector3d(value.X, value.Y, next)
        };

    static string FormatNumber(double value) =>
        value.ToString("0.######", CultureInfo.InvariantCulture);
}
