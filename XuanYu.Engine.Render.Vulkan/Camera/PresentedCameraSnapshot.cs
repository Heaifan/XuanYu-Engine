using XuanYu.Engine.Render.Camera;
using XuanYu.Engine.Render.Camera.Navigation;

namespace XuanYu.Engine.Render.Vulkan.Camera;

/// <summary>
/// 已成功 Present 的相机快照。
/// 包含完整的相机姿态、ViewProjection / InverseViewProjection 矩阵和视口尺寸。
/// Picking、Ground Hover 应使用此快照，而非等待渲染的最新状态，
/// 确保射线与当前屏幕显示完全一致。
/// </summary>
public sealed record PresentedCameraSnapshot
{
    /// <summary>已呈现帧使用的完整相机姿态。</summary>
    public required SceneCameraPose CameraPose { get; init; }

    /// <summary>列优先 ViewProjection 矩阵 (float[16])。</summary>
    public required float[] ViewProjection { get; init; }

    /// <summary>列优先 InverseViewProjection 矩阵 (double[16])。
    /// 由 Present 时预计算保存，避免 Picking 时重复高斯消元。</summary>
    public required double[] InverseViewProjection { get; init; }

    /// <summary>视口宽度（像素）。</summary>
    public required int ViewportWidth { get; init; }

    /// <summary>视口高度（像素）。</summary>
    public required int ViewportHeight { get; init; }

    /// <summary>帧索引。</summary>
    public required int FrameIndex { get; init; }

    /// <summary>相机修订号。</summary>
    public required int CameraRevision { get; init; }

    /// <summary>投影模式（透视/正交）。</summary>
    public SceneProjectionMode ProjectionMode { get; init; } = SceneProjectionMode.Perspective;

    /// <summary>
    /// 场景为空时的静态空快照（Picking 返回无命中）。
    /// </summary>
    public static PresentedCameraSnapshot Empty { get; } = new()
    {
        CameraPose = null!,
        ViewProjection = null!,
        InverseViewProjection = null!,
        ViewportWidth = 0,
        ViewportHeight = 0,
        FrameIndex = -1,
        CameraRevision = -1
    };

    /// <summary>快照是否有效（非空且有有效矩阵和逆矩阵）。</summary>
    public bool IsValid => ViewProjection is { Length: 16 } &&
                           InverseViewProjection is { Length: 16 } &&
                           ViewportWidth > 0 &&
                           ViewportHeight > 0;
}
