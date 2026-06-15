using FluidWarfare.Core.Math;
using FluidWarfare.Editor.Transform.Move;
using FluidWarfare.Editor.Transform.Move.Projection;

namespace FluidWarfare.Tests.Editor.Transform.Move;

public sealed class EntityMoveSessionTests
{
    private static readonly Vector3d TestPos = new(10, 20, 30);
    private static readonly GroundMoveAnchor TestAnchor = new(TestPos, new Vector3d(10, 20, 30), 400, 300);

    [Fact]
    public void Begin_SetsInitialPosition()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor);
        Assert.True(s.IsMoving);
        Assert.Equal(TestPos, s.InitialPosition);
        Assert.Equal(TestPos, s.CurrentPosition);
    }

    [Fact]
    public void Begin_StoresInitialDirty()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor, initialSceneDirty: true);
        Assert.True(s.InitialWasDirty);
    }

    [Fact]
    public void UpdateFromPlaneHit_GroundPlane_PreservesZ()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor);
        s.UpdateFromPlaneHit(new Vector3d(100, 200, 999));
        // Relative delta: (100-10, 200-20, 999-30) → only X,Y applied → (10+90, 20+180, 30)
        Assert.Equal(100, s.CurrentPosition.X, 3);
        Assert.Equal(200, s.CurrentPosition.Y, 3);
        Assert.Equal(30, s.CurrentPosition.Z);
    }

    [Fact]
    public void UpdateFromPlaneHit_XConstraint_ChangesOnlyX()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.X, TestAnchor);
        s.UpdateFromPlaneHit(new Vector3d(100, 200, 999));
        Assert.Equal(100, s.CurrentPosition.X, 3);
        Assert.Equal(20, s.CurrentPosition.Y);
        Assert.Equal(30, s.CurrentPosition.Z);
    }

    [Fact]
    public void UpdateFromPlaneHit_YConstraint_ChangesOnlyY()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.Y, TestAnchor);
        s.UpdateFromPlaneHit(new Vector3d(100, 200, 999));
        Assert.Equal(10, s.CurrentPosition.X);
        Assert.Equal(200, s.CurrentPosition.Y, 3);
        Assert.Equal(30, s.CurrentPosition.Z);
    }

    [Fact]
    public void UpdateVertical_ChangesZ()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.Z, TestAnchor);
        s.UpdateVertical(10, 0.1); // -10 * 0.1 = -1
        Assert.Equal(10, s.CurrentPosition.X);
        Assert.Equal(20, s.CurrentPosition.Y);
        Assert.Equal(29, s.CurrentPosition.Z, 3);
    }

    [Fact]
    public void UpdateVertical_Accumulates()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.Z, TestAnchor);
        s.UpdateVertical(10, 0.1); // -1
        s.UpdateVertical(20, 0.1); // -2 more
        Assert.Equal(27, s.CurrentPosition.Z, 3);
    }

    [Fact]
    public void SetAxisConstraint_ChangesConstraint()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor);
        s.SetAxisConstraint(EntityMoveAxis.X);
        s.UpdateFromPlaneHit(new Vector3d(100, 200, 999));
        Assert.Equal(100, s.CurrentPosition.X, 3);
        Assert.Equal(20, s.CurrentPosition.Y);
    }

    [Fact]
    public void PointerDownWithoutMove_DoesNotChangePosition()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor);
        Assert.Equal(TestPos, s.CurrentPosition);
        Assert.False(s.HasPositionChanged);
    }

    [Fact]
    public void Confirm_FiresCompletedWithFinalPosition()
    {
        var s = new EntityMoveSession();
        EntityMoveResult? captured = null;
        s.Completed += r => captured = r;

        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor);
        s.UpdateFromPlaneHit(new Vector3d(15, 25, 35));
        s.Confirm();

        Assert.NotNull(captured);
        Assert.True(captured.Value.IsConfirmed);
    }

    [Fact]
    public void Cancel_FiresCompletedWithInitialPosition()
    {
        var s = new EntityMoveSession();
        EntityMoveResult? captured = null;
        s.Completed += r => captured = r;

        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor);
        s.UpdateFromPlaneHit(new Vector3d(100, 200, 999));
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
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor, initialSceneDirty: true);
        s.Cancel();
        Assert.True(captured?.InitialWasDirty);
    }

    [Fact]
    public void Abort_SameAsCancel()
    {
        var s = new EntityMoveSession();
        EntityMoveResult? captured = null;
        s.Completed += r => captured = r;
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor);
        s.Abort();
        Assert.NotNull(captured);
        Assert.True(captured.Value.IsCancelled);
        Assert.Equal(TestPos, captured.Value.FinalPosition);
    }

    [Fact]
    public void GroundMove_UsesPlaneHitDifference()
    {
        var anchor = new GroundMoveAnchor(
            new Vector3d(10, 20, 0),
            new Vector3d(12, 22, 0),
            400, 300);
        var s = new EntityMoveSession();
        s.Begin("1", new Vector3d(10, 20, 0), EntityMoveAxis.GroundPlane, anchor);
        s.UpdateFromPlaneHit(new Vector3d(15, 25, 0));
        // delta = (15-12, 25-22) = (3, 3) → target = (10+3, 20+3, 0) = (13, 23, 0)
        Assert.Equal(13, s.CurrentPosition.X, 3);
        Assert.Equal(23, s.CurrentPosition.Y, 3);
        Assert.Equal(0, s.CurrentPosition.Z);
    }

    [Fact]
    public void HasPositionChanged_DetectsChange()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor);
        Assert.False(s.HasPositionChanged);
        s.UpdateFromPlaneHit(new Vector3d(11, 20, 30));
        Assert.True(s.HasPositionChanged);
    }

    [Fact]
    public void SameHit_IsNoOp()
    {
        var s = new EntityMoveSession();
        s.Begin("1", TestPos, EntityMoveAxis.GroundPlane, TestAnchor);
        s.UpdateFromPlaneHit(TestAnchor.InitialPlaneHit);
        Assert.False(s.HasPositionChanged);
    }
}
