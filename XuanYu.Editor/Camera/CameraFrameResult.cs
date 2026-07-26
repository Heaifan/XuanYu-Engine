using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

public readonly record struct CameraFrameResult(
    CameraState Camera,
    Vector3d ObservationCenter);
