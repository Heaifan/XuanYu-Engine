namespace FluidWarfare.Editor.Transform.Translation.Constraint;

public static class TranslationConstraintText
{
    public static string AxisName(TranslationAxis axis) => axis switch
    {
        TranslationAxis.X => "X",
        TranslationAxis.Y => "Y",
        TranslationAxis.Z => "Z",
        _ => "?",
    };

    public static string PlaneName(TranslationPlane plane) => plane switch
    {
        TranslationPlane.XY => "XY",
        TranslationPlane.XZ => "XZ",
        TranslationPlane.YZ => "YZ",
        TranslationPlane.View => "View",
        _ => "?",
    };

    public static string OrientationName(TransformOrientation o) => o switch
    {
        TransformOrientation.Global => "Global",
        TransformOrientation.Local => "Local",
        _ => "?",
    };
}
