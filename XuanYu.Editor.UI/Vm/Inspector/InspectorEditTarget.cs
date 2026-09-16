namespace XuanYu.Editor.UI;

public readonly record struct InspectorObjectIdentity(
    InspectorObjectKind ObjectKind,
    string ObjectId);

public readonly record struct InspectorEditTarget(
    InspectorObjectKind ObjectKind,
    string ObjectId,
    string PropertyKey)
{
    public InspectorObjectIdentity Identity => new(ObjectKind, ObjectId);
}
