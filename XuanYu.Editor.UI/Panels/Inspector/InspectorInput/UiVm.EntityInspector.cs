using System.Globalization;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    string _inspectorEntityNameText = "";
    InspectorEditTarget? _entityNameEditTarget;

    public bool IsEntityInspector => SelectedDataset is null &&
        TrySelectedEntityKey(out var key) && _sceneState.TryGetEntity(key, out _);

    public string InspectorEntitySubtitle => TrySelectedEntity(out var entity)
        ? $"{EditorDisplayText.EntityType(entity.Type)} · Entity" : SelectionSubtitle;

    public string InspectorEntityNameText
    {
        get => _inspectorEntityNameText;
        set => Set(ref _inspectorEntityNameText, value);
    }

    public double InspectorPositionX => InspectorVector("位置").X;
    public double InspectorPositionY => InspectorVector("位置").Y;
    public double InspectorPositionZ => InspectorVector("位置").Z;
    public double InspectorRotationX => InspectorVector("旋转").X;
    public double InspectorRotationY => InspectorVector("旋转").Y;
    public double InspectorRotationZ => InspectorVector("旋转").Z;
    public double InspectorScaleX => InspectorVector("缩放").X;
    public double InspectorScaleY => InspectorVector("缩放").Y;
    public double InspectorScaleZ => InspectorVector("缩放").Z;

    public void BeginInspectorEntityNameEdit() =>
        _entityNameEditTarget = CreateInspectorEditTarget("Entity.Basic.Name");

    public bool CommitInspectorEntityName()
    {
        var target = _entityNameEditTarget ?? CreateInspectorEditTarget("Entity.Basic.Name");
        _entityNameEditTarget = null;
        return CommitInspectorProperty(target, InspectorEntityNameText);
    }

    public void CommitInspectorVector(string group, double x, double y, double z)
    {
        foreach (var (axis, value) in new[] { ("X", x), ("Y", y), ("Z", z) })
        {
            if (InspectorAxis(group, axis) == value) continue;
            TryCommitInspectorTransformValue(group, axis,
                value.ToString("R", CultureInfo.InvariantCulture));
        }
        RaiseInspectorEntityBindings();
    }

    void RefreshInspectorEntityProjection()
    {
        _inspectorEntityNameText = TrySelectedEntity(out var entity) ? entity.Name : "";
        RaiseInspectorEntityBindings();
    }

    void RaiseInspectorEntityBindings()
    {
        OnPropertyChanged(nameof(IsEntityInspector));
        OnPropertyChanged(nameof(InspectorEntityNameText));
        OnPropertyChanged(nameof(InspectorPositionX)); OnPropertyChanged(nameof(InspectorPositionY));
        OnPropertyChanged(nameof(InspectorPositionZ)); OnPropertyChanged(nameof(InspectorRotationX));
        OnPropertyChanged(nameof(InspectorRotationY)); OnPropertyChanged(nameof(InspectorRotationZ));
        OnPropertyChanged(nameof(InspectorScaleX)); OnPropertyChanged(nameof(InspectorScaleY));
        OnPropertyChanged(nameof(InspectorScaleZ));
    }

    bool TrySelectedEntity(out XuanYu.World.WorldEntitySnapshot entity)
    {
        entity = default!;
        return TrySelectedEntityKey(out var key) && _sceneState.TryGetEntity(key, out entity);
    }

    (double X, double Y, double Z) InspectorVector(string group)
    {
        if (!TrySelectedEntity(out var entity)) return (0, 0, 0);
        var value = group switch
        {
            "位置" => entity.Transform.Position,
            "旋转" => entity.Transform.Rotation,
            "缩放" => entity.Transform.Scale,
            _ => XuanYu.Core.Math.Vector3d.Zero
        };
        return (value.X, value.Y, value.Z);
    }

    double InspectorAxis(string group, string axis) => group switch
    {
        "位置" => axis switch { "X" => InspectorPositionX, "Y" => InspectorPositionY, _ => InspectorPositionZ },
        "旋转" => axis switch { "X" => InspectorRotationX, "Y" => InspectorRotationY, _ => InspectorRotationZ },
        "缩放" => axis switch { "X" => InspectorScaleX, "Y" => InspectorScaleY, _ => InspectorScaleZ },
        _ => 0
    };
}
