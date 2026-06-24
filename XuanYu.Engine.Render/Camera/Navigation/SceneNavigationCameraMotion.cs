using FluidWarfare.Core.Math;

namespace FluidWarfare.Render.Camera.Navigation;

/// <summary>
/// 标准视图方向的 Yaw/Pitch 计算及投影模式切换。
/// 所有标准轴向从同一公式生成，不硬编码角度。
/// </summary>
public static class SceneNavigationCameraMotion
{
    /// <summary>
    /// 根据目标方向向量计算轨道相机的 Yaw 和 Pitch。
    /// 方向向量指向从 Pivot 到相机的方向。
    ///
    /// Pitch = asin(direction.Z)
    /// Yaw = atan2(direction.X, -direction.Y)
    ///
    /// Yaw 范围 0..360，Pitch 范围 -90..+90。
    /// </summary>
    /// <param name="direction">归一化的世界方向向量（从 Pivot 指向相机位置）。</param>
    /// <returns>(Yaw, Pitch) 角度。</returns>
    public static (float Yaw, float Pitch) DirectionToYawPitch(Vector3d direction)
    {
        var length = Math.Sqrt(
            direction.X * direction.X +
            direction.Y * direction.Y +
            direction.Z * direction.Z);
        if (length < 1e-10)
            return (0f, 0f);

        var dirX = (float)(direction.X / length);
        var dirY = (float)(direction.Y / length);
        var dirZ = (float)(direction.Z / length);

        var pitch = (float)Math.Asin(Math.Clamp(dirZ, -1.0f, 1.0f));
        var yaw = (float)Math.Atan2(dirX, -dirY);

        // 转换为角度
        var yawDeg = yaw * 180f / MathF.PI;
        var pitchDeg = pitch * 180f / MathF.PI;

        // Yaw 归一化到 0..360
        if (yawDeg < 0) yawDeg += 360f;
        if (yawDeg >= 360f) yawDeg -= 360f;

        return (yawDeg, pitchDeg);
    }

    /// <summary>
    /// 将相机对齐到指定世界方向（Z-Up）。
    /// 设置相机的 Yaw 和 Pitch，使相机从指定方向观察 Pivot。
    /// </summary>
    /// <param name="state">当前轨道相机状态。</param>
    /// <param name="direction">归一化目标方向。</param>
    /// <returns>对齐后的新状态。自动切换为 Orthographic。</returns>
    public static SceneOrbitCameraState SnapToDirection(
        SceneOrbitCameraState state,
        Vector3d direction)
    {
        var (yaw, pitch) = DirectionToYawPitch(direction);

        return state with
        {
            Yaw = yaw,
            Pitch = Math.Clamp(pitch, -89f, 89f),
            ProjectionMode = SceneProjectionMode.Orthographic
        };
    }

    /// <summary>
    /// 获取标准视图的世界方向向量。
    /// </summary>
    public static Vector3d GetViewDirection(SceneNavigationView view)
    {
        return view switch
        {
            SceneNavigationView.PositiveX => new Vector3d(1, 0, 0),
            SceneNavigationView.NegativeX => new Vector3d(-1, 0, 0),
            SceneNavigationView.PositiveY => new Vector3d(0, 1, 0),
            SceneNavigationView.NegativeY => new Vector3d(0, -1, 0),
            SceneNavigationView.PositiveZ => new Vector3d(0, 0, 1),
            SceneNavigationView.NegativeZ => new Vector3d(0, 0, -1),
            _ => new Vector3d(0.707, 0.707, 0.707) // Free → default
        };
    }

    /// <summary>
    /// 将相机对齐到标准视图。
    /// </summary>
    /// <param name="state">当前轨道相机状态。</param>
    /// <param name="view">标准视图方向。</param>
    /// <returns>对齐后的新状态。轴端点击自动进入正交。</returns>
    public static SceneOrbitCameraState SnapToView(
        SceneOrbitCameraState state,
        SceneNavigationView view)
    {
        if (view == SceneNavigationView.Free)
            return state;

        var dir = GetViewDirection(view);
        return SnapToDirection(state, dir);
    }

    /// <summary>
    /// 切换投影模式。透视 ↔ 正交，保留其他状态。
    /// </summary>
    /// <param name="state">当前轨道相机状态。</param>
    /// <returns>新投影模式的状态。</returns>
    public static SceneOrbitCameraState ToggleProjection(SceneOrbitCameraState state)
    {
        var halfFovRadians = state.FieldOfViewDegrees * MathF.PI / 360f;
        var tangent = MathF.Max(0.0001f, MathF.Tan(halfFovRadians));

        if (state.ProjectionMode == SceneProjectionMode.Perspective)
        {
            // 以 Pivot 深度处的可见高度作为正交高度，避免切换时画面突变。
            var visibleHeight = Math.Clamp(2f * state.Distance * tangent, 2f, 200f);
            return state with
            {
                ProjectionMode = SceneProjectionMode.Orthographic,
                OrthographicHeight = visibleHeight
            };
        }

        var fittedDistance = Math.Clamp(
            state.OrthographicHeight / (2f * tangent), 2f, 200f);
        return state with
        {
            ProjectionMode = SceneProjectionMode.Perspective,
            Distance = fittedDistance
        };
    }

    /// <summary>
    /// 从当前视图方向推测最近的 SceneNavigationView。
    /// </summary>
    public static SceneNavigationView DetectView(SceneOrbitCameraState state)
    {
        var yawRad = state.Yaw * MathF.PI / 180f;
        var pitchRad = state.Pitch * MathF.PI / 180f;
        var cp = MathF.Cos(pitchRad);

        var dirX = MathF.Sin(yawRad) * cp;
        var dirY = -MathF.Cos(yawRad) * cp;
        var dirZ = MathF.Sin(pitchRad);

        // 找到在标准方向中夹角最小的
        var views = new (SceneNavigationView view, float dx, float dy, float dz)[]
        {
            (SceneNavigationView.PositiveX, 1, 0, 0),
            (SceneNavigationView.NegativeX, -1, 0, 0),
            (SceneNavigationView.PositiveY, 0, 1, 0),
            (SceneNavigationView.NegativeY, 0, -1, 0),
            (SceneNavigationView.PositiveZ, 0, 0, 1),
            (SceneNavigationView.NegativeZ, 0, 0, -1),
        };

        var bestView = SceneNavigationView.Free;
        var bestDot = -1f;

        foreach (var (view, dx, dy, dz) in views)
        {
            var dot = dirX * dx + dirY * dy + dirZ * dz;
            if (dot > bestDot)
            {
                bestDot = dot;
                bestView = view;
            }
        }

        return bestView;
    }
}
