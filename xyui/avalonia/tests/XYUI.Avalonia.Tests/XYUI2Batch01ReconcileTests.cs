using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Gallery.Views;
using XYUI.Avalonia.Typography;
namespace XYUI.Avalonia.Tests;

// XYUI-2 Batch 01 · R1 对齐合同：Gallery Foundation 导航模板、Batch 01 验收状态文本、
// Action Edge 铺满 Chrome 内宽（非 Padding 内缩短线）、按钮家族排版消费 Foundation token。
[Collection("XyuiHeadless")]
public sealed class XYUI2Batch01ReconcileTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2Batch01ReconcileTests(XyuiHeadlessFixture fx) => _fx = fx;
    static readonly FoundationNavigationItem Sample = new("palette", "色彩", "Palette");
    [Fact]
    public void Foundation_navigation_template_matches_foundation_item() => _fx.Run(() =>
    {
        var view = new XYUI1DocumentationView();
        Assert.True(view.TryFindResource("FoundationNavItemTemplate", out var foundation));
        Assert.True(Assert.IsAssignableFrom<IDataTemplate>(foundation).Match(Sample),
            "Foundation 导航必须有匹配 FoundationNavigationItem 的模板，否则 ListBox 回退 ToString() 泄漏类型名");
        Assert.True(view.TryFindResource("NavItemTemplate", out var component));
        Assert.False(Assert.IsAssignableFrom<IDataTemplate>(component).Match(Sample),
            "组件导航模板不得匹配 Foundation 项，两类导航必须各自持有 DataTemplate");
    });
    [Fact]
    public void Foundation_navigation_renders_names_without_type_name_leak() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var view = new XYUI1DocumentationView();
        var window = new Window { Width = 1120, Height = 760, Content = view };
        window.Show();
        view.ApplyStyling();
        Dispatcher.UIThread.RunJobs();
        var texts = view.GetVisualDescendants().OfType<TextBlock>()
            .Select(x => x.Text ?? string.Empty).ToArray();
        Assert.DoesNotContain(texts, x => x.Contains("FoundationNavigationItem"));
        Assert.Contains("色彩", texts);
        Assert.Contains("Palette", texts);
        window.Close();
    });
    [Fact]
    public void Batch01_pages_are_not_marked_user_visual_accepted() => _fx.Run(() =>
    {
        var documents = XYUI2DocumentationCatalog.Build();
        Assert.Equal(5, documents.Count);
        Assert.All(documents, document =>
        {
            Assert.StartsWith(XYUI2DocumentationCatalog.PendingAcceptance, document.StatusText);
            Assert.DoesNotContain("ACCEPTED", document.StatusText);
        });
    });

    [Fact]
    public void SplitButton_documentation_uses_canonical_title_and_control_name() => _fx.Run(() =>
    {
        var document = XYUI2DocumentationCatalog.Build().Single(x => x.Id == "XYUI-2-04");
        Assert.Equal("XY.SplitButton · XYSplitButton", document.CanonicalDisplay);
        Assert.Equal("XYSplitButton", document.AvaloniaType.Split('.').Last());
        Assert.All(document.Usages, usage => Assert.Contains("XYSplitButton", usage));
    });

    [Fact]
    public void Action_edge_spans_full_chrome_inner_width() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        var button = new XYButton { Content = "新建" };
        var window = XyuiBatchTestHost.Show(button);
        var edge = XyuiBatchTestHost.Edge(button);
        var inner = button.Bounds.Width - button.BorderThickness.Left - button.BorderThickness.Right;
        Assert.True(inner > 0, $"Chrome 内宽必须为正，实测 {inner}");
        Assert.True(edge.Bounds.Width >= inner - 0.5,
            $"Action Edge 必须铺满 Chrome 内宽（内宽 {inner}，实测 {edge.Bounds.Width}）；" +
            "被 Padding 内缩会退化成悬空短线而非 Chrome 底边");
        window.Close();
    });

    [Fact]
    public void Button_family_consumes_foundation_typography() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare();
        TemplatedControl[] controls =
            [new XYButton { Content = "新建" }, new XYToggleButton { Content = "网格吸附" }, new XYSplitButton { Content = "新建" }];
        foreach (var control in controls)
        {
            var window = XyuiBatchTestHost.Show(control);
            Assert.Equal(XyuiTypographyTokens.FontUi, control.FontFamily.Name);
            Assert.Equal(XyuiTypographyTokens.FontSizeBody, control.FontSize);
            Assert.Equal(FontWeight.Medium, control.FontWeight);
            window.Close();
        }
    });
}
