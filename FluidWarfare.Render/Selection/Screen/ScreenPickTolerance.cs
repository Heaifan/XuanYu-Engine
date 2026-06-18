namespace FluidWarfare.Render.Selection.Screen;

/// <summary>
/// 屏幕空间 Picking 容差参数。
/// 编辑器点击远处小实体时，精确 Ray-AABB 容易 Miss，
/// 此容差允许鼠标在实体投影像素矩形附近仍可选中。
/// </summary>
public static class ScreenPickTolerance
{
    /// <summary>默认屏幕空间容差（像素）。</summary>
    public const int DefaultPixels = 5;
}
