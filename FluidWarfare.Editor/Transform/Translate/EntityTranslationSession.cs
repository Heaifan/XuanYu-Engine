using FluidWarfare.Core.Math;

namespace FluidWarfare.Editor.Transform.Translate;

public sealed class EntityTranslationSession
{
    private TranslationDragAnchor? _anchor;

    public bool IsActive { get; private set; }

    public string EntityId { get; private set; } = string.Empty;

    public Vector3d InitialPosition { get; private set; }

    public Vector3d CurrentPosition { get; private set; }

    public bool InitialWasDirty { get; private set; }

    public TranslationConstraint Constraint => _anchor?.Constraint ?? TranslationConstraint.GroundPlane;

    public TranslationCameraSnapshot? Camera { get; private set; }

    public event Action<TranslationResult>? Completed;

    public bool Begin(
        string entityId,
        Vector3d initialPosition,
        TranslationConstraint constraint,
        TranslationRay startRay,
        double pointerX,
        double pointerY,
        TranslationCameraSnapshot camera,
        bool initialSceneDirty)
    {
        if (!TranslationAnchorFactory.TryCreate(initialPosition, constraint, startRay, pointerX, pointerY, camera, out var anchor))
            return false;

        EntityId = entityId;
        InitialPosition = initialPosition;
        CurrentPosition = initialPosition;
        InitialWasDirty = initialSceneDirty;
        Camera = camera;
        _anchor = anchor;
        IsActive = true;
        return true;
    }

    public bool Preview(TranslationRay currentRay, double pointerX, double pointerY, out Vector3d position)
    {
        position = CurrentPosition;
        if (!IsActive || _anchor is null)
            return false;

        if (!TranslationSolver.TrySolve(_anchor, currentRay, pointerX, pointerY, out position))
            return false;

        CurrentPosition = position;
        return true;
    }

    public bool SwitchConstraint(TranslationConstraint constraint, TranslationRay currentRay, double pointerX, double pointerY)
    {
        if (!IsActive || Camera is null)
            return false;

        if (!TranslationAnchorFactory.TryCreate(CurrentPosition, constraint, currentRay, pointerX, pointerY, Camera, out var anchor))
            return false;

        _anchor = anchor;
        return true;
    }

    public void Confirm() => Complete(isConfirmed: true, isCancelled: false, CurrentPosition);

    public void Cancel() => Complete(isConfirmed: false, isCancelled: true, InitialPosition);

    public void Abort() => Complete(isConfirmed: false, isCancelled: true, InitialPosition);

    private void Complete(bool isConfirmed, bool isCancelled, Vector3d finalPosition)
    {
        if (!IsActive)
            return;

        var result = new TranslationResult(EntityId, InitialPosition, finalPosition,
            isConfirmed, isCancelled, InitialWasDirty);
        Reset();
        Completed?.Invoke(result);
    }

    private void Reset()
    {
        IsActive = false;
        EntityId = string.Empty;
        InitialPosition = default;
        CurrentPosition = default;
        InitialWasDirty = false;
        Camera = null;
        _anchor = null;
    }
}
