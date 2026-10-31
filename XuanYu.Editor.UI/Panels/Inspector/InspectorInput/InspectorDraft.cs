using System.Globalization;
using XuanYu.Core.Math;
using XuanYu.Core.Scene;

namespace XuanYu.Editor.UI;

internal sealed class InspectorDraft
{
    string _entityNameText = "", _entityNameStart = "", _layerNameText = "";
    InspectorEditTarget? _entityNameTarget;
    bool _cancelledEntityNameCommit;

    public string EntityNameText => _entityNameText;
    public string LayerNameText => _layerNameText;
    public InspectorEditTarget? EntityNameTarget => _entityNameTarget;

    public bool SetEntityNameText(string value) => Set(ref _entityNameText, value);
    public bool SetLayerNameText(string value) => Set(ref _layerNameText, value);

    public void BeginEntityNameEdit(InspectorEditTarget target, string text)
    {
        _entityNameTarget = target;
        _entityNameStart = text;
        _cancelledEntityNameCommit = false;
    }

    public bool CancelEntityNameEdit()
    {
        if (_entityNameTarget is null) return false;
        _entityNameTarget = null;
        _entityNameText = _entityNameStart;
        _cancelledEntityNameCommit = true;
        return true;
    }

    public bool ConsumeCancelledEntityNameCommit()
    {
        var cancelled = _cancelledEntityNameCommit;
        _cancelledEntityNameCommit = false;
        return cancelled;
    }

    public InspectorEditTarget? TakeEntityNameTarget()
    {
        var target = _entityNameTarget;
        _entityNameTarget = null;
        return target;
    }

    public static bool TryParseNumber(string text, out double value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(text)) return false;
        return (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value) ||
            double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value)) && double.IsFinite(value);
    }

    public static bool TryBuildTransform(CommittedTransform current, string group, string axis,
        double value, out CommittedTransform next, out string error)
    {
        next = current;
        error = "";
        var normalizedAxis = axis.Trim().ToUpperInvariant();
        if (normalizedAxis is not ("X" or "Y" or "Z")) { error = "检查器提交失败：轴必须是 X、Y 或 Z。"; return false; }
        try { next = BuildTransform(current, group.Trim(), normalizedAxis, value); }
        catch (ArgumentOutOfRangeException) { error = "检查器提交失败：缩放必须为正数且不能接近 0。"; return false; }
        if (next == current && group.Trim() is not ("位置" or "旋转" or "缩放"))
        { error = "检查器提交失败：字段必须是位置、旋转或缩放。"; return false; }
        return true;
    }

    public static string FormatNumber(double value) => value.ToString("0.######", CultureInfo.InvariantCulture);
    static CommittedTransform BuildTransform(CommittedTransform current, string group, string axis, double value) => group switch
    {
        "位置" => current.WithPosition(ReplaceAxis(current.Position, axis, value)),
        "旋转" => current.WithRotation(ReplaceAxis(current.Rotation, axis, value)),
        "缩放" => current.WithScale(ReplaceAxis(current.Scale, axis, value)),
        _ => current
    };

    static Vector3d ReplaceAxis(Vector3d value, string axis, double next) => axis switch
    {
        "X" => new Vector3d(next, value.Y, value.Z), "Y" => new Vector3d(value.X, next, value.Z),
        _ => new Vector3d(value.X, value.Y, next)
    };

    static bool Set(ref string target, string value)
    {
        if (target == value) return false;
        target = value;
        return true;
    }
}
