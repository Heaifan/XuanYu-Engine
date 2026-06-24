using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Editor.Transform.Translation.Plane;

/// <summary>
/// 平面平移拖动锚点。一次拖动期间不可变。
/// 使用射线-平面求交：target = initialPosition + (currentHit - startHit)
/// </summary>
public readonly record struct PlaneTranslationAnchor(
    Vector3d InitialPosition,
    Vector3d PlaneOrigin,
    Vector3d PlaneNormal,
    Vector3d StartHit,
    PlaneTranslationMode Mode)
{
    public bool IsValid =>
        double.IsFinite(PlaneNormal.X) && PlaneNormal.LengthSquared > 0;
}
