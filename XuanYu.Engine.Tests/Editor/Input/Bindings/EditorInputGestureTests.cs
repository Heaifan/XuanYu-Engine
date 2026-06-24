using XuanYu.Engine.Editor.Input.Bindings;

namespace FluidWarfare.Tests.Editor.Input.Bindings;

public sealed class EditorInputGestureTests
{
    [Fact]
    public void KeyGesture_HasCorrectSignature()
    {
        var g = new EditorInputGesture(EditorInputDevice.Keyboard, "Home");
        Assert.Equal("|KeyPress|Keyboard|Home", g.Signature);
    }

    [Fact]
    public void ModifierGesture_HasCorrectSignature()
    {
        var g = new EditorInputGesture(EditorInputDevice.Mouse, "Middle",
            EditorInputModifiers.Shift, EditorInputGestureKind.MouseDrag);
        Assert.Equal("Shift|MouseDrag|Mouse|Middle", g.Signature);
    }

    [Fact]
    public void WheelGesture_HasCorrectSignature()
    {
        var g = new EditorInputGesture(EditorInputDevice.Wheel, "Y", kind: EditorInputGestureKind.MouseWheel);
        Assert.Equal("|MouseWheel|Wheel|Y", g.Signature);
    }

    [Fact]
    public void DisplayString_ShowsModifiers()
    {
        var g = new EditorInputGesture(EditorInputDevice.Mouse, "Middle",
            EditorInputModifiers.Control, EditorInputGestureKind.MouseDrag);
        Assert.Contains("Ctrl+", g.ToDisplayString());
        Assert.Contains("Middle", g.ToDisplayString());
        Assert.Contains("拖动", g.ToDisplayString());
    }

    [Fact]
    public void DisplayString_ShowsRoll()
    {
        var g = new EditorInputGesture(EditorInputDevice.Wheel, "Y", kind: EditorInputGestureKind.MouseWheel);
        Assert.Contains("滚轮", g.ToDisplayString());
    }

    [Fact]
    public void DifferentGestures_HaveDifferentSignatures()
    {
        var g1 = new EditorInputGesture(EditorInputDevice.Keyboard, "A");
        var g2 = new EditorInputGesture(EditorInputDevice.Keyboard, "B");
        Assert.NotEqual(g1.Signature, g2.Signature);
    }
}
