using XuanYu.Engine.Core.Math;

namespace XuanYu.Engine.Render.Vulkan.Scene3D.GroundCursor;

/// <summary>
/// Ground Cursor 运行时状态（可见性 + 世界坐标）。
/// 相同坐标设置应为 NoOp。
/// </summary>
public sealed class VulkanGroundCursorState
{
    private bool _isVisible;
    private Vector3d? _worldPosition;
    private int _revision;

    /// <summary>地面标记是否可见。</summary>
    public bool IsVisible => _isVisible;

    /// <summary>当前世界坐标。</summary>
    public Vector3d? WorldPosition => _worldPosition;

    /// <summary>状态修订号，每次真实变化递增。</summary>
    public int Revision => _revision;

    /// <summary>
    /// 设置地面标记。相同可见状态 + 相同坐标时返回 false（NoOp）。
    /// </summary>
    public bool Set(Vector3d? worldPosition)
    {
        if (!_isVisible && worldPosition is null)
            return false; // 已经是隐藏状态

        if (_isVisible && worldPosition is not null && _worldPosition == worldPosition)
            return false; // 相同坐标

        _isVisible = worldPosition is not null;
        _worldPosition = worldPosition;
        _revision++;
        return true;
    }

    /// <summary>
    /// 隐藏地面标记。已隐藏时返回 false。
    /// </summary>
    public bool Hide()
    {
        if (!_isVisible)
            return false;
        _isVisible = false;
        _worldPosition = null;
        _revision++;
        return true;
    }
}
