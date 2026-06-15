using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Move;

namespace FluidWarfare.Tests.Editor.Transform.Move;

public sealed class EntityMoveSessionTests
{
    private static readonly Vector3d TestPos = new(10, 20, 30);
    private static readonly Vector3d MovedPos = new(15, 25, 30);

    [Fact]
    public void Begin_SetsInitialPosition()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane);
        Assert.True(s.IsMoving);
        Assert.Equal(TestPos, s.InitialPosition);
        Assert.Equal(TestPos, s.CurrentPosition);
    }

    [Fact]
    public void Begin_StoresInitialDirty()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, initialSceneDirty: true);
        Assert.True(s.InitialWasDirty);
    }

    [Fact]
    public void UpdatePosition_GroundPlane_PreservesZ()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane);
        s.UpdatePosition(new Vector3d(100, 200, 999));
        Assert.Equal(TestPos.Z, s.CurrentPosition.Z);
        Assert.Equal(100, s.CurrentPosition.X);
        Assert.Equal(200, s.CurrentPosition.Y);
    }

    [Fact]
    public void UpdatePosition_XConstraint_ChangesOnlyX()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.X);
        s.UpdatePosition(new Vector3d(100, 200, 999));
        Assert.Equal(100, s.CurrentPosition.X);
        Assert.Equal(TestPos.Y, s.CurrentPosition.Y);
        Assert.Equal(TestPos.Z, s.CurrentPosition.Z);
    }

    [Fact]
    public void UpdatePosition_YConstraint_ChangesOnlyY()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.Y);
        s.UpdatePosition(new Vector3d(100, 200, 999));
        Assert.Equal(TestPos.X, s.CurrentPosition.X);
        Assert.Equal(200, s.CurrentPosition.Y);
        Assert.Equal(TestPos.Z, s.CurrentPosition.Z);
    }

    [Fact]
    public void UpdatePosition_ZConstraint_ChangesOnlyZ()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.Z);
        s.UpdatePosition(new Vector3d(100, 200, 999));
        Assert.Equal(TestPos.X, s.CurrentPosition.X);
        Assert.Equal(TestPos.Y, s.CurrentPosition.Y);
        Assert.Equal(999, s.CurrentPosition.Z);
    }

    [Fact]
    public void SetAxisConstraint_ChangesConstraint()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane);
        s.SetAxisConstraint(EntityMoveAxis.X);
        s.UpdatePosition(new Vector3d(100, 200, 999));
        Assert.Equal(100, s.CurrentPosition.X);
        Assert.Equal(TestPos.Y, s.CurrentPosition.Y);
    }

    [Fact]
    public void PointerDownWithoutMove_DoesNotChangePosition()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane);
        // No UpdatePosition call — position unchanged
        Assert.Equal(TestPos, s.CurrentPosition);
        Assert.False(s.HasPositionChanged);
    }

    [Fact]
    public void Confirm_FiresCompletedWithFinalPosition()
    {
        var s = new EntityMoveSession();
        EntityMoveResult? captured = null;
        s.Completed += r => captured = r;

        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane);
        s.UpdatePosition(MovedPos);
        s.Confirm();

        Assert.NotNull(captured);
        Assert.True(captured.Value.IsConfirmed);
        Assert.Equal(MovedPos, captured.Value.FinalPosition);
    }

    [Fact]
    public void Cancel_FiresCompletedWithInitialPosition()
    {
        var s = new EntityMoveSession();
        EntityMoveResult? captured = null;
        s.Completed += r => captured = r;

        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane);
        s.UpdatePosition(MovedPos);
        s.Cancel();

        Assert.NotNull(captured);
        Assert.True(captured.Value.IsCancelled);
        Assert.Equal(TestPos, captured.Value.FinalPosition);
    }

    [Fact]
    public void Cancel_PreservesInitialDirty()
    {
        var s = new EntityMoveSession();
        EntityMoveResult? captured = null;
        s.Completed += r => captured = r;

        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, initialSceneDirty: true);
        s.Cancel();

        Assert.True(captured?.InitialWasDirty);
    }

    [Fact]
    public void Abort_SameAsCancel()
    {
        var s = new EntityMoveSession();
        EntityMoveResult? captured = null;
        s.Completed += r => captured = r;

        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane);
        s.Abort();

        Assert.NotNull(captured);
        Assert.True(captured.Value.IsCancelled);
        Assert.Equal(TestPos, captured.Value.FinalPosition);
    }

    [Fact]
    public void GroundMove_PreservesGrabOffset()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane);

        // Simulate: initial hit at (12, 22, 30), move to (15, 25)
        // Expected: initial(10,20,30) + ((15,25,30) - (12,22,30)) = (13,23,30)
        var initialHit = new Vector3d(12, 22, 30);
        var currentHit = new Vector3d(15, 25, 30);
        var grabOffset = initialHit - TestPos;
        var targetPos = currentHit - grabOffset;
        s.UpdatePosition(targetPos);

        Assert.Equal(13, s.CurrentPosition.X, 3);
        Assert.Equal(23, s.CurrentPosition.Y, 3);
        Assert.Equal(30, s.CurrentPosition.Z);
    }

    [Fact]
    public void HasPositionChanged_DetectsChange()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane);
        Assert.False(s.HasPositionChanged);

        s.UpdatePosition(new Vector3d(11, 20, 30));
        Assert.True(s.HasPositionChanged);
    }

    [Fact]
    public void SamePosition_IsNoOp()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane);
        s.UpdatePosition(TestPos);
        Assert.False(s.HasPositionChanged);
    }
}
