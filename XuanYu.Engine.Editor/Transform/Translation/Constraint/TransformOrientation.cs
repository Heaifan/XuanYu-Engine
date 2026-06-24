using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Editor.Transform.Translation.Constraint;

public enum TransformOrientation
{
    Global,
    Local,
}

public static class TransformOrientationExtensions
{
    /// <summary>
    /// 将约束方向从 Global 转换到当前 Orientation。
    /// Global 返回原值，Local 用实体旋转旋转该方向。
    /// </summary>
    public static Vector3d Resolve(this TransformOrientation orientation, Vector3d globalDir, Vector3d entityRotation)
    {
        return orientation == TransformOrientation.Local
            ? TranslationAxisExtensions.RotateByEuler(globalDir, entityRotation)
            : globalDir;
    }
}
