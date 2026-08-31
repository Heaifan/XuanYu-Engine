using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;
using XYUI.Avalonia.Theme;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class R5F4FidelityTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public R5F4FidelityTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Text_and_label_use_primary_and_medium_contracts() => _fx.Run(() =>
    {
        var app = global::Avalonia.Application.Current!; app.Resources.MergedDictionaries.Add(XyuiTheme.CreateLight()); app.Styles.Add(XyuiComponentStyles.Create());
        var text = new XYText { Text = "正文" }; var label = new XYLabel { Text = "字段" }; var host = new StackPanel(); host.Children.Add(text); host.Children.Add(label); var window = new Window { Content = host }; window.Show(); text.ApplyStyling(); label.ApplyStyling();
        Assert.Equal("XYUI-1-01", text.CanonicalId); Assert.Equal("XYUI-1-02", label.CanonicalId);
        Assert.Equal("Source Han Sans SC", text.FontFamily.ToString()); Assert.Equal("Source Han Sans SC", label.FontFamily.ToString());
        Assert.Equal(global::Avalonia.Media.FontWeight.Normal, text.FontWeight); Assert.Equal(global::Avalonia.Media.FontWeight.Medium, label.FontWeight);
        Assert.Contains("xyui-text", text.Classes); Assert.Contains("xyui-label", label.Classes);
    });

    [Fact]
    public void Rich_mono_run_uses_foundation_font() => _fx.Run(() =>
    {
        var rich = new XYRichText { Text = "普通", MonoText = "region-id" };
        var mono = rich.Inlines!.OfType<Run>().Last();
        Assert.Equal("Source Code Pro", mono.FontFamily.Name);
    });

    [Fact]
    public void Selectable_technical_is_a_formal_variant() => _fx.Run(() =>
    {
        var text = new XYSelectableText { Text = "region-id", Variant = XyuiSelectableTextVariant.Technical };
        Assert.Equal(XyuiSelectableTextVariant.Technical, text.Variant);
        Assert.Contains("xyui-selectable-text-technical", ((Panel)text.Child!).Children[0].Classes);
    });

    [Fact]
    public void Shortcut_hint_creates_separate_keycaps() => _fx.Run(() =>
    {
        var hint = new XYShortcutHint { Shortcut = "Ctrl + Shift + S" };
        var panel = Assert.IsType<StackPanel>(hint.Child);
        Assert.Equal(5, panel.Children.Count);
        Assert.Equal("Ctrl", ((Border)panel.Children[0]).Child is TextBlock t ? t.Text : "");
    });

    [Fact]
    public void Tooltip_exposes_canonical_behavior_parameters() => _fx.Run(() =>
    {
        var tooltip = new XYTooltip();
        Assert.Equal(280, tooltip.MaxWidth); Assert.Equal(400, tooltip.ShowDelay);
        Assert.True(tooltip.ViewportAvoidance); Assert.True(tooltip.AutoFlip);
        Assert.False(tooltip.PointerCapture); Assert.False(tooltip.InteractiveContent);
    });

    [Fact]
    public void Gallery_exposes_identity_and_gap_state() => _fx.Run(() =>
    {
        var docs = XYUI1DocumentationCatalog.Build();
        Assert.Equal(24, docs.Count);
        Assert.Equal("XY.Badge", docs.Single(x => x.Id == "XYUI-1-09").CanonicalIdentity);
        Assert.Equal("XYUI1-GAP-002", docs.Single(x => x.Id == "XYUI-1-24").KnownGap);
        Assert.Equal("USER VISUAL ACCEPTED · GAP RETAINED", docs.Single(x => x.Id == "XYUI-1-24").StatusText);
    });

    [Fact]
    public void Mapping_ref_counts_and_identity_source_are_consistent()
    {
        var root = FindRoot();
        using var map = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "xyui", "specs", "XYUI1", "XYUI-1.mapping.json")));
        using var identity = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "xyui", "specs", "XYUI1", "XYUI-1.identity.json")));
        var components = map.RootElement.GetProperty("components").EnumerateArray().ToArray();
        Assert.All(components, component => Assert.Equal(component.GetProperty("ref_count").GetInt32(), component.GetProperty("refs").GetArrayLength()));
        Assert.Equal(24, identity.RootElement.GetProperty("components").GetArrayLength());
    }

    static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "xyui", "specs", "XYUI1", "XYUI-1.mapping.json"))) return directory.FullName;
            directory = directory.Parent;
        }
        throw new DirectoryNotFoundException("XYUI repository root not found");
    }
}
