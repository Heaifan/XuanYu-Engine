using XuanYu.Engine.Core.Math;

namespace FluidWarfare.Editor.ViewportGround;

/// <summary>
/// 平台无关的地面指针状态。
/// HoverHit — 鼠标当前指向位置（状态栏反馈）。
/// CommittedHit — 用户最后一次点击确认位置（Scene3D 落点标记）。
/// </summary>
public sealed class EditorGroundPointerState
{
    private Vector3d? _hoverHit;
    private Vector3d? _committedHit;
    private string? _hoverSource;
    private int _revision;

    /// <summary>鼠标当前指向的地面位置，null 表示未在视口内。</summary>
    public Vector3d? HoverHit => _hoverHit;

    /// <summary>用户最后一次点击确认的地面位置，null 表示未设置。</summary>
    public Vector3d? CommittedHit => _committedHit;

    /// <summary>Hover 来源描述（"鼠标"），供诊断用。</summary>
    public string? HoverSource => _hoverSource;

    /// <summary>CommittedHit 变更次数，递增用于 Scene3D 帧按需重绘判断。</summary>
    public int Revision => _revision;

    /// <summary>
    /// 更新鼠标 Hover 位置。坐标相同时为 NoOp。
    /// </summary>
    public EditorGroundPointerChange SetHover(Vector3d? worldPosition, string? source)
    {
        var prev = _hoverHit;
        if (prev == worldPosition && _hoverSource == source)
            return EditorGroundPointerChange.NoChange;

        _hoverHit = worldPosition;
        _hoverSource = source;
        return new EditorGroundPointerChange(true, false, prev, worldPosition);
    }

    /// <summary>
    /// 提交地面点击确认位置。坐标相同时为 NoOp。
    /// 返回 true 表示需要 Scene3D 帧重绘。
    /// </summary>
    public EditorGroundPointerChange Commit(Vector3d? worldPosition)
    {
        var prev = _committedHit;
        if (prev == worldPosition)
            return EditorGroundPointerChange.NoChange;

        _committedHit = worldPosition;
        _revision++;
        return new EditorGroundPointerChange(true, true, prev, worldPosition);
    }

    /// <summary>
    /// 清除 CommittedHit（点击天空时调用）。
    /// </summary>
    public EditorGroundPointerChange ClearCommit()
    {
        var prev = _committedHit;
        if (prev is null)
            return EditorGroundPointerChange.NoChange;

        _committedHit = null;
        _revision++;
        return new EditorGroundPointerChange(true, true, prev, null);
    }
}
