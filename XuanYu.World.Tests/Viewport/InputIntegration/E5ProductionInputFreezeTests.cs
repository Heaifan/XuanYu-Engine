using XUnit = Xunit;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class E5ProductionInputFreezeTests
{
    [XUnit.Fact]
    public void UiWin_window_deactivation_uses_production_input_sink()
    {
        var path = Path.Combine(TestContext.Root, "XuanYu.Editor.UI", "Win", "UiWin.axaml.cs");
        var source = File.ReadAllText(path);

        XUnit.Assert.DoesNotContain("CancelInteractionFromWindowDeactivated", source);
        XUnit.Assert.Contains("ViewportInput.Sink", source);
        XUnit.Assert.Contains("WindowDeactivated", source);
    }

    static class TestContext
    {
        public static string Root => FindRoot(AppContext.BaseDirectory);

        static string FindRoot(string start)
        {
            var directory = new DirectoryInfo(start);
            while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "XuanYu.Engine.slnx")))
                directory = directory.Parent;
            return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
        }
    }
}
