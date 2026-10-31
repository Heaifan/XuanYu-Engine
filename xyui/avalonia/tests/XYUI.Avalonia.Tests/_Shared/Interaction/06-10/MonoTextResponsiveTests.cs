using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Gallery.Views;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class MonoTextResponsiveTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public MonoTextResponsiveTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Shared_column_contract_keeps_complete_auto_labels() => _fx.Run(() =>
    {
        var mono = Preview();
        Assert.Equal(HorizontalAlignment.Left, mono.HorizontalAlignment);
        Assert.Equal(GridLength.Auto, mono.ColumnDefinitions[0].Width);
        Assert.Equal(new GridLength(XYMonoText.LabelValueGap), mono.ColumnDefinitions[1].Width);
        Assert.Equal(GridLength.Auto, mono.ColumnDefinitions[2].Width);
        Assert.Equal(new GridLength(XYMonoText.ValueUnitGap), mono.ColumnDefinitions[3].Width);
        Assert.Equal(GridLength.Auto, mono.ColumnDefinitions[4].Width);
    });

    [Fact]
    public void Narrow_and_wide_hosts_preserve_content_width_and_shared_axes() => _fx.Run(() =>
    {
        var narrow = Preview(); Arrange(narrow, 360);
        var wide = Preview(); Arrange(wide, 760);
        Assert.Equal(narrow.Bounds.Width, wide.Bounds.Width);
        Assert.Single(Edges(narrow, "value", right: true).Distinct());
        Assert.Single(Edges(wide, "value", right: true).Distinct());
        Assert.Single(Edges(narrow, "unit", right: false).Distinct());
        Assert.Single(Edges(wide, "unit", right: false).Distinct());
        Assert.Equal(Edges(narrow, "value", true), Edges(wide, "value", true));
        Assert.Equal(Edges(narrow, "unit", false), Edges(wide, "unit", false));
    });

    [Fact]
    public void Gallery_host_keeps_the_real_mono_preview_content_sized() => _fx.Run(() =>
    {
        var document = XYUI1DocumentationCatalog.Build().Single(x => x.Id == "XYUI-1-08");
        var view = new XYUI1ComponentDocumentView { DataContext = document };
        var host = view.FindControl<ContentControl>("PreviewHost")!;
        Assert.Equal(HorizontalAlignment.Left, host.HorizontalContentAlignment);
        Assert.IsType<XYMonoText>(host.Content);
    });

    static XYMonoText Preview() => Assert.IsType<XYMonoText>(XYUI1GalleryCatalog.CreatePreview("XYUI-1-08"));
    static void Arrange(XYMonoText mono, double width) { mono.Measure(new Size(width, 300)); mono.Arrange(new Rect(0, 0, width, mono.DesiredSize.Height)); }
    static double[] Edges(XYMonoText mono, string role, bool right) => mono.Children.OfType<TextBlock>()
        .Where(x => x.Classes.Contains($"xyui-mono-data-{role}"))
        .Select(x => right ? x.Bounds.Right : x.Bounds.Left).ToArray();
}
