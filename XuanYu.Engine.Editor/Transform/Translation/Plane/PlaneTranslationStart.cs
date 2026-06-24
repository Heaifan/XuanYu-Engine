using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.Transform.Translation.Plane;

/// <summary>
/// 从射线-平面交点创建 PlaneTranslationAnchor 的辅助方法。
/// </summary>
public static class PlaneTranslationStart
{
    public static PlaneTranslationAnchor CreateAnchor(
        Vector3d initialPosition,
        Vector3d planeOrigin,
        Vector3d planeNormal,
        Vector3d startHit)
    {
        return new PlaneTranslationAnchor(
            initialPosition, planeOrigin, planeNormal,
            startHit, PlaneTranslationMode.DragPlane);
    }
}
