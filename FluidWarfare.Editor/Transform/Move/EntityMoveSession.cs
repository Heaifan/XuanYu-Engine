using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Move.Projection;

namespace FluidWarfare.Editor.Transform.Move;

/// <summary>
/// 视口实体移动会话状态机。
/// 使用 GroundMoveProjection 进行平面求交→相对差值映射，
/// 使用 VerticalMoveProjection 进行 Z 轴垂直位移。
/// </summary>
public sealed class EntityMoveSession
{
    private Vector3d _initialPosition;
    private Vector3d _currentPosition;
    private EntityMoveAxis _axis;
    private string _entityId = string.Empty;
    private bool _positionChanged;
    private bool _initialWasDirty;
    private GroundMoveAnchor _anchor;
    private MoveMappingMode _mappingMode;

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

    /// <summary>移动开始时的 Dirty 状态（用于取消恢复）。</summary>
    public bool InitialWasDirty => _initialWasDirty;

    /// <summary>当前移动锚点。</summary>
    public GroundMoveAnchor Anchor => _anchor;

    /// <summary>当前移动映射模式。</summary>
    public MoveMappingMode MappingMode => _mappingMode;

    // ─── 事件 ──────────────────────────────────────────

    /// <summary>移动确认或取消时触发。</summary>
    public event Action<EntityMoveResult>? Completed;

    // ─── 方法 ──────────────────────────────────────────

    /// <summary>
    /// 开始新移动会话，同时记录 GroundMoveAnchor。
    /// anchor 由 EditorShell 在 PointerDown 时通过射线求交构建。
    /// </summary>
    public void Begin(string entityId, Vector3d initialPosition, EntityMoveAxis axis,
        GroundMoveAnchor anchor, bool initialSceneDirty = false)
    {
        _entityId = entityId;
        _initialPosition = initialPosition;
        _currentPosition = initialPosition;
        _axis = axis;
        _positionChanged = false;
        _initialWasDirty = initialSceneDirty;
        _anchor = anchor;
        _mappingMode = MoveMappingMode.PlaneIntersection;
    }

    /// <summary>
    /// 使用 GroundMoveProjection.PlaneIntersection 模式更新位置。
    /// 公式：TargetPosition = InitialEntityPosition + (CurrentPlaneHit - InitialPlaneHit)
    /// </summary>
    public void UpdateFromPlaneHit(Vector3d currentPlaneHit)
    {
        if (!IsMoving) return;
        var targetPos = GroundMoveProjection.Map(_anchor, currentPlaneHit, _axis);
        ApplyPosition(targetPos);
    }

    /// <summary>
    /// 使用 GroundMoveProjection.ScreenDeltaFallback 模式更新位置。
    /// </summary>
    public void UpdateFromScreenDelta(
        Vector3d cameraRight, Vector3d cameraForward,
        double deltaPixelX, double deltaPixelY,
        double worldPerPixel)
    {
        if (!IsMoving) return;
        _mappingMode = MoveMappingMode.ScreenDeltaFallback;
        var targetPos = GroundMoveProjection.MapScreenDelta(
            _anchor, cameraRight, cameraForward,
            deltaPixelX, deltaPixelY, worldPerPixel, _axis);
        ApplyPosition(targetPos);
    }

    /// <summary>
    /// 使用 VerticalMoveProjection 更新 Z 轴位置。
    /// </summary>
    public void UpdateVertical(double deltaPixelY, double worldPerPixel)
    {
        if (!IsMoving) return;
        var newPos = VerticalMoveProjection.ApplyToPosition(
            _currentPosition, _initialPosition,
            deltaPixelY, worldPerPixel);
        ApplyPosition(newPos);
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
    /// 取消移动并恢复到初始位置和初始 Dirty 状态。
    /// </summary>
    public void Cancel()
    {
        if (!IsMoving) return;
        var result = new EntityMoveResult(
            IsConfirmed: false,
            IsCancelled: true,
            HasPositionChanged: false,
            FinalPosition: _initialPosition,
            InitialWasDirty: _initialWasDirty);
        Completed?.Invoke(result);
        Reset();
    }

    /// <summary>
    /// 强行中止（如 CaptureLost、切换工具），与 Cancel 相同。
    /// </summary>
    public void Abort() => Cancel();

    private void ApplyPosition(Vector3d position)
    {
        if (position != _currentPosition)
        {
            _currentPosition = position;
            _positionChanged = position != _initialPosition;
        }
    }

    private void Reset()
    {
        _entityId = string.Empty;
        _initialPosition = Vector3d.Zero;
        _currentPosition = Vector3d.Zero;
        _axis = EntityMoveAxis.GroundPlane;
        _positionChanged = false;
        _anchor = default;
        _mappingMode = MoveMappingMode.PlaneIntersection;
    }
}
