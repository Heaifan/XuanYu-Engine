namespace XuanYu.Engine.Render.ViewportNavigation;

/// <summary>
/// ViewportNavigationLayout 的命中测试和动作映射。
/// </summary>
public static class ViewportNavigationHitTest
{
    /// <summary>根据像素坐标检测命中的导航元素。</summary>
    public static ViewportNavigationElement HitTest(
        float pixelX, float pixelY, ViewportNavigationLayout layout)
    {
        // 轴端（反向遍历：正面优先）
        for (var i = layout.AxisProjections.Count - 1; i >= 0; i--)
        {
            var p = layout.AxisProjections[i];
            var dx = pixelX - p.ScreenX;
            var dy = pixelY - p.ScreenY;
            if (dx * dx + dy * dy <= p.Radius * p.Radius)
                return p.Element;
        }

        // Orbit 区域
        var gdx = pixelX - layout.GizmoOrbitCircle.CenterX;
        var gdy = pixelY - layout.GizmoOrbitCircle.CenterY;
        if (gdx * gdx + gdy * gdy <= layout.GizmoOrbitCircle.Radius * layout.GizmoOrbitCircle.Radius)
            return ViewportNavigationElement.GizmoCenter;

        // 按钮
        if (InRect(pixelX, pixelY, layout.PanButtonRect)) return ViewportNavigationElement.PanButton;
        if (InRect(pixelX, pixelY, layout.FrameButtonRect)) return ViewportNavigationElement.FrameButton;
        if (InRect(pixelX, pixelY, layout.ProjectionButtonRect)) return ViewportNavigationElement.ProjectionButton;

        return ViewportNavigationElement.None;
    }

    /// <summary>将 HitTest 结果转换为导航动作。</summary>
    public static ViewportNavigationAction ElementToAction(ViewportNavigationElement element) => element switch
    {
        ViewportNavigationElement.PositiveX => ViewportNavigationAction.SnapPositiveX,
        ViewportNavigationElement.NegativeX => ViewportNavigationAction.SnapNegativeX,
        ViewportNavigationElement.PositiveY => ViewportNavigationAction.SnapPositiveY,
        ViewportNavigationElement.NegativeY => ViewportNavigationAction.SnapNegativeY,
        ViewportNavigationElement.PositiveZ => ViewportNavigationAction.SnapPositiveZ,
        ViewportNavigationElement.NegativeZ => ViewportNavigationAction.SnapNegativeZ,
        ViewportNavigationElement.GizmoCenter => ViewportNavigationAction.Orbit,
        ViewportNavigationElement.PanButton => ViewportNavigationAction.Pan,
        ViewportNavigationElement.FrameButton => ViewportNavigationAction.Frame,
        ViewportNavigationElement.ProjectionButton => ViewportNavigationAction.ToggleProjection,
        _ => ViewportNavigationAction.None,
    };

    private static bool InRect(float px, float py, Rect r) =>
        px >= r.X && px <= r.X + r.W && py >= r.Y && py <= r.Y + r.H;
}
