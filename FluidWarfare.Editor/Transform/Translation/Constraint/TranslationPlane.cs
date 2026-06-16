using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translation.Constraint;

public enum TranslationPlane
{
    XY,
    XZ,
    YZ,
    View,
}

public static class TranslationPlaneExtensions
{
    /// <summary>返回平面法线（约束方向）。XY → Z, XZ → Y, YZ → X, View → cameraForward。</summary>
    public static Vector3d GetNormal(this TranslationPlane plane, Vector3d cameraForward)
    {
        return plane switch
        {
            TranslationPlane.XY => new Vector3d(0, 0, 1),
            TranslationPlane.XZ => new Vector3d(0, 1, 0),
            TranslationPlane.YZ => new Vector3d(1, 0, 0),
            TranslationPlane.View => cameraForward,
            _ => Vector3d.Zero,
        };
    }
}
