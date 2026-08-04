using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.UI;

public sealed record CameraSessionSnapshot(
    long SessionId,
    long PointerId,
    CameraSessionMode Mode,
    double StartX,
    double StartY,
    CameraState StartCamera,
    Vector3d StartCenter,
    int Width,
    int Height);
