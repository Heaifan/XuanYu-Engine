using XuanYu.Core.Math;

namespace XuanYu.Editor.Camera;

public static partial class EditorCameraFraming
{
    const double DraftFocusMinimumRadius = 75.0;

    public static CameraFrameResult FrameDraftWithCenter(
        IEnumerable<Vector3d> positions, double aspect, long revision)
    {
        return Frame(positions.ToArray(), aspect, revision, DraftFocusMinimumRadius, Direction);
    }
}
