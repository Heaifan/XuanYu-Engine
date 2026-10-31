using Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI3MenuGalleryCloseoutTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI3MenuGalleryCloseoutTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Menu_and_MenuBar_documents_expose_complete_closeout_metadata() => _fx.Run(() =>
    {
        var docs = XYUI3DocumentationCatalog.Build();
        var menu = Assert.Single(docs, x => x.Id == "XYUI-3-3.02");
        var bar = Assert.Single(docs, x => x.Id == "XYUI-3-3.01");

        Assert.Contains("Command=\"{Binding RunCommand}\"", menu.QuickStartXaml);
        Assert.Contains("CommandParameter=\"New\"", menu.QuickStartXaml);
        Assert.Contains("CheckKind=\"Radio\"", menu.QuickStartXaml);
        Assert.Contains("Classes=\"compact\"", bar.QuickStartXaml);

        Assert.Contains(menu.States, s => s.Name == "Default");
        Assert.Contains(menu.States, s => s.Name == "Disabled");
        Assert.Contains(menu.States, s => s.Name == "Checked");
        Assert.Contains(menu.States, s => s.Name == "Radio Selected");
        Assert.Contains(menu.States, s => s.Name == "Popup Open");

        Assert.NotNull(menu.LiveExamplesFactory?.Invoke());
        Assert.NotNull(bar.LiveExamplesFactory?.Invoke());
    });

    [Fact]
    public void Menu_sample_viewmodel_executes_commands_and_maintains_states()
    {
        var vm = new XYUIMenuSampleViewModel();
        Assert.False(vm.DisabledCommand.CanExecute(null));

        vm.RunCommand.Execute("新建项目");
        Assert.Contains("新建项目", vm.LastAction);

        Assert.True(vm.IsMapEditor);
        Assert.False(vm.IsRegionEditor);

        vm.IsRegionEditor = true;
        Assert.False(vm.IsMapEditor);
        Assert.True(vm.IsRegionEditor);
        Assert.Contains("区域编辑", vm.StatusSummary);

        vm.IsGridVisible = false;
        Assert.Contains("关", vm.StatusSummary);
    }
}
