using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class UiLayerDeleteDialogContractTests
{
    static readonly string Repo = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");

    static string Read(string file) => File.ReadAllText(Path.Combine(Repo, "XuanYu.Editor.UI", "Win", file));

    [Fact]
    public void Layer_delete_uses_an_owned_window_instead_of_the_overlay_host()
    {
        var host = Read("UiWin.DialogHost.Danger.cs");
        var window = Read("LayerDeleteConfirmationWindow.axaml.cs");
        Assert.Contains("ConfirmLayerDeleteAsync", host);
        Assert.Contains("ShowAsync(this, layer.Name", host);
        Assert.Contains("ShowDialog<bool>(owner)", window);
        Assert.DoesNotContain("DialogOverlay", host);
    }

    [Fact]
    public void Owned_delete_window_is_visible_and_safe_by_default()
    {
        var view = Read("LayerDeleteConfirmationWindow.axaml");
        var code = Read("LayerDeleteConfirmationWindow.axaml.cs");
        Assert.Contains("WindowStartupLocation=\"CenterOwner\"", view);
        Assert.Contains("x:Name=\"CancelButton\"", view);
        Assert.Contains("Content=\"删除\"", view);
        Assert.Contains("Key.Escape", code);
        Assert.Contains("Complete(false)", code);
        Assert.Contains("if (_completed) return;", code);
    }
}
