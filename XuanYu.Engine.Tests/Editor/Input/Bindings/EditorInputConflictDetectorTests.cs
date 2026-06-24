using FluidWarfare.Editor.Input.Actions;
using FluidWarfare.Editor.Input.Bindings;

namespace FluidWarfare.Tests.Editor.Input.Bindings;

public sealed class EditorInputConflictDetectorTests
{
    [Fact]
    public void SameContextSameGesture_IsHardConflict()
    {
        var orbitBinding = new EditorInputBinding
        {
            ActionId = "viewport.orbit",
            PrimaryGesture = new(EditorInputDevice.Mouse, "Middle",
                kind: EditorInputGestureKind.MouseDrag)
        };

        var conflict = EditorInputConflictDetector.DetectConflict(
            [orbitBinding],
            "viewport.pan",
            new(EditorInputDevice.Mouse, "Middle", kind: EditorInputGestureKind.MouseDrag),
            out var conflictActionId, out var conflictSlot);

        Assert.True(conflict);
        Assert.Equal("viewport.orbit", conflictActionId);
    }

    [Fact]
    public void DifferentContext_SameGesture_IsAllowed()
    {
        var globalBinding = new EditorInputBinding
        {
            ActionId = "editor.open_preferences",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Comma", EditorInputModifiers.Control)
        };

        var conflict = EditorInputConflictDetector.DetectConflict(
            [globalBinding],
            "viewport.frame_all",
            new(EditorInputDevice.Keyboard, "Comma", EditorInputModifiers.Control),
            out _, out _);

        Assert.False(conflict);
    }

    [Fact]
    public void ReservedLeftClick_IsRejected()
    {
        var gesture = new EditorInputGesture(EditorInputDevice.Mouse, "Left");
        Assert.True(EditorInputConflictDetector.IsReservedGesture(gesture));
    }

    [Fact]
    public void ReservedRightClick_IsRejected()
    {
        var gesture = new EditorInputGesture(EditorInputDevice.Mouse, "Right");
        Assert.True(EditorInputConflictDetector.IsReservedGesture(gesture));
    }

    [Fact]
    public void ModifiedLeftClick_IsAllowed()
    {
        var gesture = new EditorInputGesture(EditorInputDevice.Mouse, "Left", EditorInputModifiers.Alt);
        Assert.False(EditorInputConflictDetector.IsReservedGesture(gesture));
    }

    [Fact]
    public void MiddleDrag_IsNotReserved()
    {
        var gesture = new EditorInputGesture(EditorInputDevice.Mouse, "Middle",
            kind: EditorInputGestureKind.MouseDrag);
        Assert.False(EditorInputConflictDetector.IsReservedGesture(gesture));
    }

    [Fact]
    public void SameContextDiffGesture_NoConflict()
    {
        var orbitBinding = new EditorInputBinding
        {
            ActionId = "viewport.orbit",
            PrimaryGesture = new(EditorInputDevice.Mouse, "Middle",
                kind: EditorInputGestureKind.MouseDrag)
        };

        var conflict = EditorInputConflictDetector.DetectConflict(
            [orbitBinding],
            "viewport.pan",
            new(EditorInputDevice.Mouse, "Right",
                EditorInputModifiers.Shift,
                EditorInputGestureKind.MouseDrag),
            out _, out _);

        Assert.False(conflict);
    }

    [Fact]
    public void GlobalAndViewport_SameGesture_NoCrossContextConflict()
    {
        var globalBinding = new EditorInputBinding
        {
            ActionId = "editor.open_preferences",
            PrimaryGesture = new(EditorInputDevice.Keyboard, "Comma", EditorInputModifiers.Control)
        };

        var conflict = EditorInputConflictDetector.DetectConflict(
            [globalBinding],
            "viewport.frame_all",
            new(EditorInputDevice.Keyboard, "Comma", EditorInputModifiers.Control),
            out _, out _);

        Assert.False(conflict);
    }

    [Fact]
    public void SameContextSameGesture_TwoDifferentActions_Conflicts()
    {
        var orbitBinding = new EditorInputBinding
        {
            ActionId = "viewport.orbit",
            PrimaryGesture = new(EditorInputDevice.Mouse, "Middle",
                kind: EditorInputGestureKind.MouseDrag)
        };

        var conflict = EditorInputConflictDetector.DetectConflict(
            [orbitBinding],
            "viewport.pan",
            new(EditorInputDevice.Mouse, "Middle", kind: EditorInputGestureKind.MouseDrag),
            out var conflictActionId, out _);

        Assert.True(conflict);
        Assert.Equal("viewport.orbit", conflictActionId);
    }
}
