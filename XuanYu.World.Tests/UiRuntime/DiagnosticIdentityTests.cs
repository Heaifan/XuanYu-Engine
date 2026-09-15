using Avalonia.Controls;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticIdentityTests : IDisposable
{
    public DiagnosticIdentityTests() => DiagnosticRegistry.Clear();
    public void Dispose() => DiagnosticRegistry.Clear();

    [Theory]
    [InlineData("XYE.INSPECTOR.ROAD.STATE")]
    [InlineData("XYE.AREA.RIGHT")]
    [InlineData("XYE.CONTEXT_TOOLBAR")]
    public void Validate_accepts_uppercase_semantic_ids(string value)
    {
        var error = Record.Exception(() => DiagnosticId.Validate(value));

        Assert.Null(error);
    }

    [Theory]
    [InlineData("Grid1")]
    [InlineData("XYE.RIGHT3")]
    [InlineData("XYE.inspector.road")]
    [InlineData("")]
    [InlineData("Window/Grid[0]/Border")]
    public void Validate_rejects_non_semantic_ids(string value)
    {
        Assert.Throws<ArgumentException>(() => DiagnosticId.Validate(value));
    }

    [Fact]
    public void Attached_ids_can_exist_independently()
    {
        var target = new Border();

        XYDiagnostic.SetDebugId(target, "XYE.VIEWPORT");
        Assert.Equal("XYE.VIEWPORT", XYDiagnostic.GetDebugId(target));
        Assert.Null(XYDiagnostic.GetAreaId(target));

        XYDiagnostic.SetAreaId(target, "XYE.AREA.CENTER");
        Assert.Equal("XYE.AREA.CENTER", XYDiagnostic.GetAreaId(target));
    }

    [Fact]
    public void Register_rejects_duplicate_debug_id()
    {
        var first = new Border();
        var second = new Border();
        XYDiagnostic.SetDebugId(first, "XYE.VIEWPORT");
        XYDiagnostic.SetDebugId(second, "XYE.VIEWPORT");
        DiagnosticRegistry.Register(first);

        var error = Assert.Throws<InvalidOperationException>(
            () => DiagnosticRegistry.Register(second));

        Assert.Contains("XYE.VIEWPORT", error.Message);
    }

    [Fact]
    public void Register_ignores_missing_id_and_clear_empties_read_only_targets()
    {
        DiagnosticRegistry.Register(new Border());
        var target = new Border();
        XYDiagnostic.SetDebugId(target, "XYE.VIEWPORT");
        DiagnosticRegistry.Register(target);

        DiagnosticRegistry.Clear();

        Assert.Empty(DiagnosticRegistry.Targets);
        var dictionary = Assert.IsAssignableFrom<IDictionary<string, Control>>(
            DiagnosticRegistry.Targets);
        Assert.Throws<NotSupportedException>(
            () => dictionary.Add("XYE.MENU", new Border()));
    }
}
