using Avalonia.Controls;
using Avalonia.VisualTree;
using XuanYu.Editor.UI;
using XYUI.Avalonia.Controls;

namespace XuanYu.World.Tests.UiRuntime;

[Collection("UiRuntime")]
public sealed class DiagnosticR1IdentityCompletionTests
{
    readonly UiHeadlessFixture _fixture;
    public DiagnosticR1IdentityCompletionTests(UiHeadlessFixture fixture) => _fixture = fixture;

    [Fact]
    public void Card_displays_complete_xyui_identity_and_parent_area()
    {
        _fixture.Run(() =>
        {
            var area = new Border(); XYDiagnostic.SetDebugId(area, "XYE.MENU");
            var target = new XYButton { Name = "SaveSceneButton", Content = "保存" };
            XYDiagnostic.SetDebugId(target, "XYE.TOP.FILE.SAVE"); area.Child = target;
            var snapshot = DiagnosticElementSnapshot.Capture(DiagnosticProbeResolver.Resolve(target));
            var card = new DiagnosticFloatingCard(snapshot, _ => Task.CompletedTask);
            var identity = card.FindControl<TextBlock>("Identity")?.Text ?? string.Empty;
            var details = card.FindControl<TextBlock>("Details")?.Text ?? string.Empty;
            Assert.Contains("组件类型：XYButton", identity);
            Assert.Contains("组件索引：XYUI2", identity);
            Assert.Contains("组件编号：XYUI-2-01", identity);
            Assert.Contains("调试编号：XYE.TOP.FILE.SAVE", details);
            Assert.Contains("所属区域：XYE.MENU", details);
        });
    }

    [Fact]
    public void Top_file_buttons_have_stable_names_and_debug_ids()
    {
        var file = ReadModule("FileModule.axaml");
        Assert.Contains("x:Name=\"SaveSceneButton\"", file);
        Assert.Contains("local:XYDiagnostic.DebugId=\"XYE.TOP.FILE.SAVE\"", file);
        Assert.Contains("x:Name=\"OpenSceneButton\"", file);
        Assert.Contains("local:XYDiagnostic.DebugId=\"XYE.TOP.FILE.OPEN\"", file);
        var view = ReadModule("ViewModule.axaml");
        Assert.Contains("x:Name=\"ViewAllButton\"", view);
        Assert.Contains("local:XYDiagnostic.DebugId=\"XYE.TOP.VIEW.ALL\"", view);
        var edit = ReadModule("EditToolsModule.axaml");
        Assert.Contains("x:Name=\"SelectToolButton\"", edit);
        Assert.Contains("local:XYDiagnostic.DebugId=\"XYE.TOP.EDIT.SELECT\"", edit);
    }

    static string ReadModule(string name) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Top", name));
}
