using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

public static partial class EditorCameraFraming
{
    public static CameraFrameResult FrameMapOrthographicWithCenter(
        IEnumerable<Vector3d> positions, double aspect, double distance, long revision)
    {
        return FrameOrthographicWithCenter(
            positions, MapPitchDirection, DefaultEditorCamera.Up,
            aspect, distance, revision);
    }
}
