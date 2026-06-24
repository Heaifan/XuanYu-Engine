using XuanYu.Engine.Core.Math;
using XuanYu.Engine.Editor.Transform.Translation.Axis;
using XuanYu.Engine.Render.Selection;
using XuanYu.Engine.Render.Vulkan.Camera;

namespace XuanYu.Engine.Editor.Windows.Viewport.Transform.Drag;

/// <summary>
/// 从相机快照构建 DragPlane 模式 AxisTranslationAnchor。
/// 使用射线-轴约束平面方案，替代旧屏幕投影法。
/// </summary>
public static class AxisDragAnchorBuilder
{
    public static (AxisTranslationAnchor Anchor, bool Success) Build(
        Vector3d axis, double x, double y, Vector3d pivot,
        PresentedCameraSnapshot camera, Vector3d currentPosition)
    {
        var pose = camera.CameraPose;
        if (pose is null) return (default, false);

        // 构造起始鼠标射线
        var status = VulkanSceneRayBuilder.TryBuild((float)x, (float)y, camera,
            (uint)camera.ViewportWidth, (uint)camera.ViewportHeight, out var ray);
        if (status != SceneRayBuildStatus.Success || ray is null)
            return (default, false);

        // 从相机姿态计算方向向量
        var fwd = new Vector3d(pose.TargetX - pose.PositionX, pose.TargetY - pose.PositionY, pose.TargetZ - pose.PositionZ);
        if (fwd.IsZero) return (default, false);
        fwd = fwd.Normalize();
        var up = new Vector3d(pose.UpX, pose.UpY, pose.UpZ);
        // Cross product: Right = Forward × Up
        var right = new Vector3d(
            fwd.Y * up.Z - fwd.Z * up.Y,
            fwd.Z * up.X - fwd.X * up.Z,
            fwd.X * up.Y - fwd.Y * up.X);
        if (right.IsZero) right = new Vector3d(1, 0, 0);
        right = right.Normalize();

        // Gram-Schmidt：构造包含轴、面向摄像机的平面法线
        var planeNormal = BuildPlaneNormal(axis, fwd, right, up);
        if (planeNormal is null) return (default, false);

        // 射线-平面求交得到起始交点
        var denom = ray.Direction.Dot(planeNormal.Value);
        if (Math.Abs(denom) < 1e-10) return (default, false);
        var t = (pivot - ray.Origin).Dot(planeNormal.Value) / denom;
        if (t <= 0) return (default, false);
        var startHit = ray.Origin + ray.Direction * t;

        var anchor = new AxisTranslationAnchor(
            currentPosition, axis, pivot,
            PixelsPerWorldUnit: 0, ScreenDirection: default,
            StartPointerX: 0, StartPointerY: 0,
            Mode: AxisTranslationMode.DragPlane)
        {
            StartIntersection = startHit,
            DragPlaneNormal = planeNormal.Value,
            CameraForward = fwd,
            CameraRight = right,
            CameraUp = up,
        };
        return (anchor, true);
    }

    /// <summary>Gram-Schmidt：构造包含 axis 且尽量面向 cameraForward 的法线。</summary>
    static Vector3d? BuildPlaneNormal(Vector3d axis, Vector3d fwd, Vector3d right, Vector3d up)
    {
        var an = axis.Normalize();
        var vn = fwd.Normalize();

        // 主方案：从视线剔除沿轴分量
        var n = vn - vn.Dot(an) * an;
        if (n.Length > 1e-6) return n.Normalize();

        // Fallback 1：用 cameraRight
        n = right - right.Dot(an) * an;
        if (n.Length > 1e-6) return n.Normalize();

        // Fallback 2：用 cameraUp
        n = up - up.Dot(an) * an;
        if (n.Length > 1e-6) return n.Normalize();

        return null; // 所有 fallback 均失败（极罕见）
    }
}
