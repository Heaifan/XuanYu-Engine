using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class UiLayerDeleteDialogContractTests
{
    static readonly string Repo = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..");

    static string Read(string file) => File.ReadAllText(Path.Combine(Repo, "XuanYu.Editor.UI", "Win", file));

    [Fact]
    public void Both_delete_semantics_use_the_owned_window_instead_of_the_overlay_host()
    {
        var host = Read("UiWin.DialogHost.Danger.cs");
        var window = Read("LayerDeleteConfirmationWindow.axaml.cs");
        Assert.Contains("ConfirmLayerDeleteAsync", host);
        Assert.Contains("name is \"删除图层\" or \"解除注册数据集\"", host);
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
        Assert.Contains("x:Name=\"DialogTitle\"", view);
        Assert.Contains("x:Name=\"ActionButton\"", view);
        Assert.Contains("Content=\"删除\"", view);
        Assert.Contains("Key.Escape", code);
        Assert.Contains("Complete(false)", code);
        Assert.Contains("if (_completed) return;", code);
    }

    [Fact]
    public void Dataset_unregister_keeps_its_captured_layer_target()
    {
        var route = File.ReadAllText(Path.Combine(Repo, "XuanYu.Editor.UI", "Vm", "Map",
            "UiVm.MapCommandRouting.Danger.cs"));
        var danger = File.ReadAllText(Path.Combine(Repo, "XuanYu.Editor.UI", "Vm", "Map", "UiVm.MapDanger.cs"));
        Assert.Contains("RequestDangerousConfirmation(\"解除注册数据集\", selected.LayerId)", route);
        Assert.Contains("name == \"解除注册数据集\" && layerId is { } datasetLayerId", danger);
        Assert.DoesNotContain("解除注册数据集\" && SelectedLayer", danger);
    }
}
