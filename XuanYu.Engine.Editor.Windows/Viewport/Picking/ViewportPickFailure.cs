namespace XuanYu.Engine.Editor.Windows.Viewport.Picking;

/// <summary>Picking 失败原因。</summary>
public enum ViewportPickFailure
{
    None,
    SnapshotUnavailable,
    RayBuildFailed,
    MatrixInvalid,
    SnapshotExtentMismatch,
    PixelOutOfBounds,
}

/// <summary>Picking 失败结果。</summary>
public sealed record ViewportPickFailureResult(
    ViewportPickFailure Failure,
    string? Message);
