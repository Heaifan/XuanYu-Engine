using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.Terrain;

public sealed class TerrainFix1NotificationContractTests
{
    [Fact]
    public void Notification_host_is_bottom_right_and_does_not_capture_outside_cards()
    {
        var root = TerrainFix1Source.Read("XuanYu.Editor.UI/Root/UiRoot.axaml");
        var host = TerrainFix1Source.Read("XuanYu.Editor.UI/Notification/BottomRightNotificationHost.axaml");

        Assert.Contains("BottomRightNotificationHost", root);
        Assert.Contains("HorizontalAlignment=\"Right\"", host);
        Assert.Contains("VerticalAlignment=\"Bottom\"", host);
        Assert.Contains("IsHitTestVisible=\"False\"", host);
        Assert.Contains("IsHitTestVisible=\"True\"", host);
        Assert.DoesNotContain("Grid.Row=\"0\"", host);
    }

    [Fact]
    public void Progress_card_is_only_visible_while_importing_and_is_not_in_top()
    {
        var root = TerrainFix1Source.Read("XuanYu.Editor.UI/Root/UiRoot.axaml");
        var host = TerrainFix1Source.Read("XuanYu.Editor.UI/Notification/BottomRightNotificationHost.axaml");

        Assert.Contains("IsVisible=\"{Binding IsTerrainImporting}\"", host);
        Assert.Contains("TerrainImportPercentage", host);
        Assert.Contains("TerrainImportMessage", host);
        Assert.DoesNotContain("IsTerrainImporting", TerrainFix1Source.Read("XuanYu.Editor.UI/Top/Top.axaml"));
        Assert.DoesNotContain("VerticalAlignment=\"Top\"", root);
    }

    [Fact]
    public void Import_terminal_states_publish_one_notification_state()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);

        vm.NotifySuccess("地形导入完成");
        Assert.True(vm.HasNotification);
        Assert.Equal("地形导入完成", vm.NotificationText);
        var sequence = vm.NotificationSequence;

        vm.NotifySuccess("地形导入完成");
        Assert.Equal(sequence + 1, vm.NotificationSequence);
        Assert.Equal(2, vm.NotificationCount);
        Assert.Equal(1, Count(TerrainFix1Source.Read(
            "XuanYu.Editor.UI/Notification/BottomRightNotificationHost.axaml"),
            "<local:NotificationBar"));
    }

    static int Count(string source, string token) =>
        source.Split(token).Length - 1;
}

static class TerrainFix1Source
{
    public static string Read(string relative) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "../../../../", relative));
}
