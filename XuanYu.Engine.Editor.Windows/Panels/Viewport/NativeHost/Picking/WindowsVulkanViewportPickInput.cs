namespace XuanYu.Engine.Editor.Windows.Panels.Viewport.NativeHost;

/// <summary>
/// Win32 视口左键点击检测。
/// 记录按下位置，松开时判断移动距离是否小于阈值，是则视为点击。
/// NativeHost 只抛出 ViewportPickRequested 事件，不参与 Picking 计算。
/// </summary>
public sealed class WindowsVulkanViewportPickInput
{
    private const int MaxDragThreshold = 4;

    private bool _tracking;
    private int _downX;
    private int _downY;

    /// <summary>
    /// 点击事件：参数为 (pixelX, pixelY)。
    /// </summary>
    public event Action<int, int>? PickRequested;

    /// <summary>
    /// 记录按下位置。WM_LBUTTONDOWN 时调用。
    /// </summary>
    public void OnDown(int x, int y)
    {
        _tracking = true;
        _downX = x;
        _downY = y;
    }

    /// <summary>
    /// 松开时判定是否点击。WM_LBUTTONUP 时调用。
    /// </summary>
    public void OnUp(int x, int y)
    {
        if (!_tracking) return;
        _tracking = false;

        if (Math.Abs(x - _downX) <= MaxDragThreshold &&
            Math.Abs(y - _downY) <= MaxDragThreshold)
        {
            PickRequested?.Invoke(x, y);
        }
    }

    /// <summary>
    /// 失去焦点时取消跟踪。WM_KILLFOCUS 时调用。
    /// </summary>
    public void OnKillFocus()
    {
        _tracking = false;
    }
}
