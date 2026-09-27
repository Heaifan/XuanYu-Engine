using Avalonia;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Gallery.Views;
using XYUI.Avalonia.Controls;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed partial class XYUI4GalleryTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI4GalleryTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Gallery_registers_xyui4_components_and_routes_selection() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var vm = new XYUI1DocumentationViewModel();
        Assert.Equal("13/13", vm.XYUI4CountText);
        Assert.Equal(new[] { "XYUI-4-4.01", "XYUI-4-4.02", "XYUI-4-4.03", "XYUI-4-4.04", "XYUI-4-4.05", "XYUI-4-4.06", "XYUI-4-4.07", "XYUI-4-4.08", "XYUI-4-4.09", "XYUI-4-4.10" },
            vm.XYUI4Items.Take(10).Select(x => x.Id));
        Assert.Equal("XYHoverState", vm.XYUI4Items[0].CanonicalName);
        Assert.Equal("SelectedState", vm.XYUI4Items[1].CanonicalName);
        Assert.Equal("ActiveState", vm.XYUI4Items[2].CanonicalName);
        Assert.Equal("SelectionContextFocus", vm.XYUI4Items[3].CanonicalName);
        Assert.Equal("MultiSelection", vm.XYUI4Items[4].CanonicalName);
        Assert.Equal("SelectionGroup", vm.XYUI4Items[5].CanonicalName);
        Assert.Equal("XYMarqueeSelection", vm.XYUI4Items[6].CanonicalName);
        Assert.Equal("XYLassoSelection", vm.XYUI4Items[7].CanonicalName);
        Assert.Equal("XYSelectionOutline", vm.XYUI4Items[8].CanonicalName);
        Assert.Equal("XYBoundingBox", vm.XYUI4Items[9].CanonicalName);
        Assert.Equal(XYUI4DocumentationCatalog.LatestComponentId, vm.SelectedXYUI4Item?.Id);
        vm.Select("XYUI-4-4.15");
        Assert.Equal("XYUI-4-4.15", vm.SelectedXYUI4Item?.Id);
        Assert.True(vm.IsXYUI4Expanded);
        Assert.IsType<XYUI1ComponentDocumentView>(vm.SelectedDocument);
    });

    [Fact]
    public void Multi_selection_and_group_gallery_expose_primary_secondary_contracts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var multi = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.05");
        var multiWindow = new Window { Content = multi };
        multiWindow.Show();
        Assert.True(multi.GetVisualDescendants().OfType<ListBox>().Single().SelectedItems!.Count >= 2);
        var group = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.06");
        Assert.Contains(group.GetVisualDescendants().OfType<TextBlock>(), x => x.Text?.Contains("选择组") == true);
        Assert.NotNull(XYUI4GalleryCatalog.CreateLiveExamples("XYUI-4-4.05"));
        Assert.NotNull(XYUI4GalleryCatalog.CreateLiveExamples("XYUI-4-4.06"));
        multiWindow.Close();
    });

    [Fact]
    public void Selection_state_gallery_uses_existing_interaction_style_contracts() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var vm = new XYUI1DocumentationViewModel();
        foreach (var id in new[] { "XYUI-4-4.02", "XYUI-4-4.03", "XYUI-4-4.04" })
        {
            vm.Select(id);
            var document = vm.XYUI4Items.Single(x => x.Id == id).Document;
            Assert.NotNull(document);
            Assert.NotNull(document!.PreviewFactory());
            Assert.NotNull(document.LiveExamplesFactory?.Invoke());
        }
    });

    [Fact]
    public void Selection_outline_gallery_exposes_result_boundary() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var preview = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.09");
        var outline = preview.GetVisualDescendants().OfType<XYSelectionOutline>().Single();
        Assert.True(outline.Points.Count >= 5);
        Assert.NotNull(XYUI4GalleryCatalog.CreateLiveExamples("XYUI-4-4.09"));
    });

    [Fact]
    public void Bounding_box_gallery_exposes_transform_handles() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var preview = XYUI4GalleryCatalog.CreatePreview("XYUI-4-4.10");
        var box = preview.GetVisualDescendants().OfType<XYBoundingBox>().Single();
        Assert.Equal(7, box.HandleSize);
        Assert.True(box.ShowRotationHandle);
        Assert.True(box.ShowPivot);
        Assert.True(box.IsHitTestVisible);
        Assert.Equal(0, box.Angle);
        Assert.NotNull(XYUI4GalleryCatalog.CreateLiveExamples("XYUI-4-4.10"));
    });

}
