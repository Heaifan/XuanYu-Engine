using XuanYu.Core.Math;
using XuanYu.Core.Space;

namespace XuanYu.Editor.Camera;

// F3-F4：正交视图生成。六方向标准视图（±X/±Y/±Z）切换为正交投影时，
// 正交尺度取当前透视相机在观察中心处的可见竖直范围，保证切换视觉连续。
public static class OrthographicViewFactory
{
    public static double ScaleForDistance(double distance, double verticalFovDegrees)
    {
        if (!double.IsFinite(distance) || distance <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(distance));
        }

        if (!double.IsFinite(verticalFovDegrees) || verticalFovDegrees <= 0.0 || verticalFovDegrees >= 180.0)
        {
            throw new ArgumentOutOfRangeException(nameof(verticalFovDegrees));
        }

        return 2.0 * distance * System.Math.Tan(verticalFovDegrees * System.Math.PI / 360.0);
    }
}
