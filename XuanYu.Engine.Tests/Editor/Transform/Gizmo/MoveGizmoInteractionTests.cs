using XuanYu.Engine.Editor.Windows.Viewport.Transform.Gizmo;

namespace XuanYu.Engine.Tests.Editor.Transform.Gizmo;

public sealed class MoveGizmoInteractionTests
{
    [Fact]
    public void InitialState_IsNone()
    {
        var gizmo = new MoveGizmoInteraction();
        Assert.Equal(MoveGizmoElement.None, gizmo.HoveredElement);
        Assert.Equal(MoveGizmoElement.None, gizmo.ActiveElement);
        Assert.False(gizmo.IsDragging);
    }

    [Fact]
    public void SetHover_UpdatesHoveredElement()
    {
        var gizmo = new MoveGizmoInteraction();
        gizmo.SetHover(MoveGizmoElement.AxisX);
        Assert.Equal(MoveGizmoElement.AxisX, gizmo.HoveredElement);
    }

    [Fact]
    public void ClearHover_ResetsHoveredElement()
    {
        var gizmo = new MoveGizmoInteraction();
        gizmo.SetHover(MoveGizmoElement.AxisX);
        gizmo.ClearHover();
        Assert.Equal(MoveGizmoElement.None, gizmo.HoveredElement);
    }

    [Fact]
    public void TryBeginDrag_WithValidElement_SetsActiveAndIsDragging()
    {
        var gizmo = new MoveGizmoInteraction();
        var result = gizmo.TryBeginDrag(MoveGizmoElement.AxisX);

        Assert.True(result);
        Assert.Equal(MoveGizmoElement.AxisX, gizmo.ActiveElement);
        Assert.True(gizmo.IsDragging);
    }

    [Fact]
    public void TryBeginDrag_WithNoneElement_ReturnsFalse()
    {
        var gizmo = new MoveGizmoInteraction();
        var result = gizmo.TryBeginDrag(MoveGizmoElement.None);

        Assert.False(result);
        Assert.Equal(MoveGizmoElement.None, gizmo.ActiveElement);
        Assert.False(gizmo.IsDragging);
    }

    [Fact]
    public void EndDrag_ClearsActiveElementAndHoveredElement()
    {
        var gizmo = new MoveGizmoInteraction();
        gizmo.SetHover(MoveGizmoElement.AxisY);
        gizmo.TryBeginDrag(MoveGizmoElement.AxisY);

        Assert.True(gizmo.IsDragging);
        gizmo.EndDrag();

        Assert.Equal(MoveGizmoElement.None, gizmo.ActiveElement);
        Assert.Equal(MoveGizmoElement.None, gizmo.HoveredElement);
        Assert.False(gizmo.IsDragging);
    }

    [Fact]
    public void EndDrag_WithoutBeginDrag_DoesNotThrow()
    {
        var gizmo = new MoveGizmoInteraction();
        gizmo.EndDrag(); // 幂等调用
        Assert.Equal(MoveGizmoElement.None, gizmo.ActiveElement);
        Assert.Equal(MoveGizmoElement.None, gizmo.HoveredElement);
    }

    [Fact]
    public void SetHover_DuringDrag_DoesNotAffectActiveElement()
    {
        var gizmo = new MoveGizmoInteraction();
        gizmo.TryBeginDrag(MoveGizmoElement.AxisX);
        gizmo.SetHover(MoveGizmoElement.AxisY);

        Assert.Equal(MoveGizmoElement.AxisX, gizmo.ActiveElement);
        Assert.Equal(MoveGizmoElement.AxisY, gizmo.HoveredElement);
    }
}
