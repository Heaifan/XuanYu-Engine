namespace FluidWarfare.Render.Camera;

/// <summary>
/// 完整的相机姿态 — 渲染、Picking、Ground Hover 的唯一真源。
/// 由 SceneOrbitCameraState 计算生成，直接传递给渲染层和拾取层。
/// 包含位置、目标、上方向、视场角和裁剪面。
/// </summary>
public sealed record SceneCameraPose
{
    /// <summary>相机世界位置 X。</summary>
    public required float PositionX { get; init; }

    /// <summary>相机世界位置 Y。</summary>
    public required float PositionY { get; init; }

    /// <summary>相机世界位置 Z。</summary>
    public required float PositionZ { get; init; }

    /// <summary>观察目标 X。</summary>
    public required float TargetX { get; init; }

    /// <summary>观察目标 Y。</summary>
    public required float TargetY { get; init; }

    /// <summary>观察目标 Z。</summary>
    public required float TargetZ { get; init; }

    /// <summary>上方向 X（通常 0）。</summary>
    public required float UpX { get; init; }

    /// <summary>上方向 Y（通常 1）。</summary>
    public required float UpY { get; init; }

    /// <summary>上方向 Z（通常 0）。</summary>
    public required float UpZ { get; init; }

    /// <summary>垂直视场角（度）。</summary>
    public required float FieldOfViewDegrees { get; init; }

    /// <summary>近裁剪面。</summary>
    public required float NearPlane { get; init; }

    /// <summary>远裁剪面。</summary>
    public required float FarPlane { get; init; }

    /// <summary>
    /// 修订号。每次相机状态变更递增，用于缓存失效判断。
    /// </summary>
    public int Revision { get; init; }

    /// <summary>
    /// 从轨道相机状态计算完整相机姿态（Z-Up）。
    /// Position = Pivot + OrbitDirection(Yaw, Pitch) × Distance
    /// Target = Pivot
    /// Up = (0, 0, 1)
    /// </summary>
    public static SceneCameraPose FromOrbitState(SceneOrbitCameraState orbit, int revision)
    {
        var (px, py, pz) = orbit.ComputePosition();
        return new SceneCameraPose
        {
            PositionX = px,
            PositionY = py,
            PositionZ = pz,
            TargetX = orbit.PivotX,
            TargetY = orbit.PivotY,
            TargetZ = orbit.PivotZ,
            UpX = 0,
            UpY = 0,
            UpZ = 1,
            FieldOfViewDegrees = orbit.FieldOfViewDegrees,
            NearPlane = orbit.NearPlane,
            FarPlane = orbit.FarPlane,
            Revision = revision
        };
    }

    /// <summary>
    /// 返回诊断摘要。
    /// </summary>
    public string ToSummary()
    {
        return $"Position ({PositionX:F1},{PositionY:F1},{PositionZ:F1}), " +
               $"Target ({TargetX:F1},{TargetY:F1},{TargetZ:F1}), " +
               $"FOV {FieldOfViewDegrees:F0}°, " +
               $"Near {NearPlane}, Far {FarPlane}, " +
               $"Rev {Revision}";
    }
}
