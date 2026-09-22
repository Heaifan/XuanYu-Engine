using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Tests;

public sealed partial class GalleryLayoutArchitectureTests
{
    [Fact] public void SharedDocumentShell_HasSingleVerticalScrollAuthority()
    {
        var shell = ReadView("GalleryDocumentShell.cs");
        Assert.Contains("class GalleryDocumentShell : ScrollViewer", shell);
        var names = new[] { "XYUI1ComponentDocumentView.axaml",
            "XYUI3ComponentDocumentView.axaml" };
        foreach (var name in names)
        {
            var view = ReadView(name);
            Assert.Single(Regex.Matches(view, "<views:GalleryDocumentShell"));
            Assert.DoesNotContain("<ScrollViewer", view);
        }
    }

    [Fact] public void SharedDocumentShell_LongDocument_AllowsVerticalScrolling() => _fx.Run(() =>
    {
        var shell = new GalleryDocumentShell { Content = new Border { Height = 600 } };
        var window = XyuiBatchTestHost.Show(shell); Dispatcher.UIThread.RunJobs();
        Assert.True(shell.Extent.Height > shell.Viewport.Height);
        shell.ScrollTo(100);
        Assert.True(shell.Offset.Y > 0); window.Close();
    });

    [Fact] public void SharedDocumentShell_DoesNotDisableVerticalScrollbarContract()
    {
        var source = ReadView("GalleryDocumentShell.cs");
        Assert.Contains("VerticalScrollBarVisibility = ScrollBarVisibility.Visible", source);
        Assert.DoesNotContain("VerticalScrollBarVisibility = ScrollBarVisibility.Disabled", source);
    }

    [Fact] public void XYUI1_ModuleGroup_HasOverviewEntry() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var vm = new XYUI1DocumentationViewModel();
        Assert.Equal("XYUI-1", vm.Items[0].Id); Assert.Null(vm.Items[0].Document);
    });

    [Fact] public void XYUI2_ModuleGroup_HasOverviewEntry() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var vm = new XYUI1DocumentationViewModel();
        Assert.Equal("XYUI-2", vm.XYUI2Items[0].Id); Assert.Null(vm.XYUI2Items[0].Document);
    });

    [Fact] public void XYUI3_ModuleGroup_HasOverviewEntry() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var vm = new XYUI1DocumentationViewModel();
        Assert.Equal("XYUI-3", vm.XYUI3Items[0].Id); Assert.Null(vm.XYUI3Items[0].Document);
    });

    [Fact] public void ModuleOverview_IsFirstEntry() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var vm = new XYUI1DocumentationViewModel();
        Assert.All(new[] { vm.Items, vm.XYUI2Items, vm.XYUI3Items },
            items => Assert.Null(items[0].Document));
    });

    [Fact] public void ComponentCount_RemainsUnchanged() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var vm = new XYUI1DocumentationViewModel();
        Assert.Equal(24, vm.Items.Skip(1).Count());
        Assert.Equal(24, vm.XYUI2Items.Skip(1).Count());
        Assert.Equal(25, vm.XYUI3Items.Skip(1).Count());
        Assert.Equal("25/25", vm.XYUI3CountText);
    });
}
