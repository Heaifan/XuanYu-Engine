using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Move;

/// <summary>
/// 视口实体移动会话状态机。
/// 维护移动初始位置、当前草稿位置和轴向约束。
/// 地面交点和屏幕→世界转换由调用方（EditorShell）负责，
/// UpdatePosition 接收世界坐标，内部应用轴向约束。
/// </summary>
public sealed class EntityMoveSession
{
    private Vector3d _initialPosition;
    private Vector3d _currentPosition;
    private EntityMoveAxis _axis;
    private string _entityId = string.Empty;
    private bool _positionChanged;

    // ─── 公共属性 ──────────────────────────────────────

    /// <summary>当前移动的实体 ID。</summary>
    public string EntityId => _entityId;

    /// <summary>移动开始时的位置（用于取消恢复）。</summary>
    public Vector3d InitialPosition => _initialPosition;

    /// <summary>当前草稿位置。</summary>
    public Vector3d CurrentPosition => _currentPosition;

    /// <summary>当前轴向约束。</summary>
    public EntityMoveAxis Axis => _axis;

    /// <summary>是否有活动移动会话。</summary>
    public bool IsMoving => _entityId.Length > 0;

    /// <summary>当前位置是否与初始位置不同。</summary>
    public bool HasPositionChanged => _positionChanged;

    // ─── 事件 ──────────────────────────────────────────

    /// <summary>移动确认（提交最终位置）。</summary>
    public event Action<EntityMoveResult>? Completed;

    // ─── 方法 ──────────────────────────────────────────

    /// <summary>
    /// 开始新移动会话。
    /// </summary>
    public void Begin(string entityId, Vector3d initialPosition, EntityMoveAxis axis)
    {
        _entityId = entityId;
        _initialPosition = initialPosition;
        _currentPosition = initialPosition;
        _axis = axis;
        _positionChanged = false;
    }

    /// <summary>
    /// 应用新的世界坐标并受轴向约束。
    /// </summary>
    public void UpdatePosition(Vector3d worldPosition)
    {
        if (!IsMoving) return;

        var constrained = _axis switch
        {
            EntityMoveAxis.GroundPlane => new Vector3d(worldPosition.X, worldPosition.Y, _initialPosition.Z),
            EntityMoveAxis.X => new Vector3d(worldPosition.X, _initialPosition.Y, _initialPosition.Z),
            EntityMoveAxis.Y => new Vector3d(_initialPosition.X, worldPosition.Y, _initialPosition.Z),
            EntityMoveAxis.Z => new Vector3d(_initialPosition.X, _initialPosition.Y, worldPosition.Z),
            _ => worldPosition,
        };

        if (constrained != _currentPosition)
        {
            _currentPosition = constrained;
            _positionChanged = constrained != _initialPosition;
        }
    }

    /// <summary>
    /// 运行时切换轴向约束。
    /// </summary>
    public void SetAxisConstraint(EntityMoveAxis axis)
    {
        _axis = axis;
    }

    /// <summary>
    /// 确认移动并提交最终位置。
    /// </summary>
    public void Confirm()
    {
        if (!IsMoving) return;
        var result = new EntityMoveResult(
            IsConfirmed: true,
            IsCancelled: false,
            HasPositionChanged: _positionChanged,
            FinalPosition: _currentPosition);
        Completed?.Invoke(result);
        Reset();
    }

    /// <summary>
    /// 取消移动并恢复到初始位置。
    /// </summary>
    public void Cancel()
    {
        if (!IsMoving) return;
        var result = new EntityMoveResult(
            IsConfirmed: false,
            IsCancelled: true,
            HasPositionChanged: false,
            FinalPosition: _initialPosition);
        Completed?.Invoke(result);
        Reset();
    }

    /// <summary>
    /// 强行中止（如 CaptureLost），不触发事件。
    /// </summary>
    public void Abort()
    {
        Reset();
    }

    private void Reset()
    {
        _entityId = string.Empty;
        _initialPosition = Vector3d.Zero;
        _currentPosition = Vector3d.Zero;
        _axis = EntityMoveAxis.GroundPlane;
        _positionChanged = false;
    }
}
