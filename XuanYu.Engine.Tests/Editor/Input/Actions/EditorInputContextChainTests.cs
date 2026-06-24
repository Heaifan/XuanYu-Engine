using FluidWarfare.Editor.Input.Actions;

namespace FluidWarfare.Tests.Editor.Input.Actions;

public sealed class EditorInputContextChainTests
{
    [Fact]
    public void GlobalAction_AlwaysAllowed()
    {
        Assert.True(EditorInputContextChain.IsContextAllowed(
            EditorInputActionContext.Global, EditorInputActionContext.Global));
        Assert.True(EditorInputContextChain.IsContextAllowed(
            EditorInputActionContext.Global, EditorInputActionContext.BindingCapture));
        Assert.True(EditorInputContextChain.IsContextAllowed(
            EditorInputActionContext.Global, EditorInputActionContext.TextInput));
        Assert.True(EditorInputContextChain.IsContextAllowed(
            EditorInputActionContext.Global, EditorInputActionContext.Viewport3D));
    }

    [Fact]
    public void Viewport3D_AllowedUnderGlobal()
    {
        // Global (index 5) >= Viewport3D (index 4) → true
        Assert.True(EditorInputContextChain.IsContextAllowed(
            EditorInputActionContext.Viewport3D, EditorInputActionContext.Global));
    }

    [Fact]
    public void Viewport3D_BlockedUnderTextInput()
    {
        // TextInput (index 1) >= Viewport3D (index 4) → false
        Assert.False(EditorInputContextChain.IsContextAllowed(
            EditorInputActionContext.Viewport3D, EditorInputActionContext.TextInput));
    }

    [Fact]
    public void Viewport3D_BlockedUnderActiveEditorTool()
    {
        Assert.False(EditorInputContextChain.IsContextAllowed(
            EditorInputActionContext.Viewport3D, EditorInputActionContext.ActiveEditorTool));
    }

    [Fact]
    public void TextInput_BlockedUnderBindingCapture()
    {
        Assert.False(EditorInputContextChain.IsContextAllowed(
            EditorInputActionContext.TextInput, EditorInputActionContext.BindingCapture));
    }

    [Fact]
    public void InspectorTransform_BlockedUnderTextInput()
    {
        Assert.False(EditorInputContextChain.IsContextAllowed(
            EditorInputActionContext.InspectorTransform, EditorInputActionContext.TextInput));
    }

    [Fact]
    public void InspectorTransform_AllowedUnderGlobal()
    {
        Assert.True(EditorInputContextChain.IsContextAllowed(
            EditorInputActionContext.InspectorTransform, EditorInputActionContext.Global));
    }

    [Fact]
    public void ContextChain_OrderIsCorrect()
    {
        Assert.Equal(0, EditorInputContextChain.IndexOf(EditorInputActionContext.BindingCapture));
        Assert.Equal(1, EditorInputContextChain.IndexOf(EditorInputActionContext.TextInput));
        Assert.Equal(2, EditorInputContextChain.IndexOf(EditorInputActionContext.InspectorTransform));
        Assert.Equal(3, EditorInputContextChain.IndexOf(EditorInputActionContext.ActiveEditorTool));
        Assert.Equal(4, EditorInputContextChain.IndexOf(EditorInputActionContext.Viewport3D));
        Assert.Equal(5, EditorInputContextChain.IndexOf(EditorInputActionContext.Global));
    }

    [Fact]
    public void UnknownContext_FallsBackToLast()
    {
        var unknown = (EditorInputActionContext)999;
        Assert.Equal(5, EditorInputContextChain.IndexOf(unknown));
    }
}
