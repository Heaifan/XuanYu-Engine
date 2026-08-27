using System.Globalization;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

// 家族文字对齐合同（2026-08-27 VISUAL REJECTED 实测产物）：四类文字着墨中心同偏 −0.37 DIP
// （字体行盒不对称 + Avalonia 整 DIP 栅格锁定，亚像素补偿无实现通道）；等线即合同，0.45 上界拦截整像素错位复发；
// 文字左对齐内距统一 12；Chevron 居中恒为 0。
[Collection("XyuiHeadless")]
public sealed class XYUI2InkAlignmentAuditTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2InkAlignmentAuditTests(XyuiHeadlessFixture fx) => _fx = fx;

    internal static readonly IReadOnlyDictionary<string, Func<TemplatedControl>> Samples =
        new Dictionary<string, Func<TemplatedControl>>
        {
            ["Button"] = () => new XYButton { Content = "导出", Width = 120 },
            ["Toggle"] = () => new XYToggleButton { Content = "导出", Width = 120 }, ["SplitMain"] = () => new XYSplitButton { Content = "导出", Width = 150 },
            ["DropdownZone"] = () => new XYDropDownButton { Content = "导出", Width = 150 },
        };

    [Fact]
    public void Family_text_shares_one_vertical_ink_line() => _fx.Run(() =>
    {
        var shifts = Samples.ToDictionary(kv => kv.Key, kv => Parse(TextLine(Audit(kv.Value())), "shiftNeededDown"));
        var reference = shifts["Button"];
        Assert.True(Math.Abs(reference) < 0.45, $"基准偏差 {reference:F3} 已达整像素级");
        foreach (var kv in shifts)
            Assert.True(Math.Abs(kv.Value - reference) <= 0.01, $"{kv.Key} 与 Button 垂直失线：{kv.Value:F3} vs {reference:F3}");
    });

    [Theory]
    [InlineData("Button", 12d)]
    [InlineData("Toggle", 12d)]
    [InlineData("SplitMain", 12d)]
    [InlineData("DropdownZone", 12d)]
    public void Family_text_is_left_aligned_with_uniform_inset(string key, double inset) => _fx.Run(() =>
        Assert.Equal(inset, Parse(TextLine(Audit(Samples[key]())), "leftInset"), 2));

    [Theory]
    [InlineData("SplitMain")]
    [InlineData("DropdownZone")]
    public void Chevron_centers_exactly_in_both_slots(string key) => _fx.Run(() =>
        Assert.InRange(Parse(Audit(Samples[key]()).Split('\n').First(l => l.Contains("kind=ICON")), "shiftNeededDown"), -0.001, 0.001));

    static string Audit(TemplatedControl control)
    {
        XyuiBatchTestHost.Prepare();
        var window = XyuiBatchTestHost.Show(new StackPanel { Margin = new Thickness(40, 30), Children = { control } });
        var report = Inspect(control, window);
        window.Close();
        return report;
    }
    internal static string Inspect(TemplatedControl control, Window window)
    {
        var f = CultureInfo.InvariantCulture;
        var b = new StringBuilder();
        var chrome = control.GetVisualDescendants().OfType<Border>().First();
        var at0 = chrome.TranslatePoint(new Point(0, 0), window)!.Value;
        var innerH = chrome.Bounds.Height - chrome.BorderThickness.Top - chrome.BorderThickness.Bottom;
        var centerY = at0.Y + chrome.BorderThickness.Top + innerH / 2;
        var innerLeft = at0.X + chrome.BorderThickness.Left;
        foreach (var node in control.GetVisualDescendants())
        {
            double top, bottom, left;
            if (node is TextBlock text)
            {   // BuildGeometry 以行盒左上角为原点（基线内嵌于布局空间），直接映射窗口坐标。
                var font = new Typeface(text.FontFamily, text.FontStyle, text.FontWeight);
                var formatted = new FormattedText(text.Text ?? "", CultureInfo.CurrentUICulture,
                    FlowDirection.LeftToRight, font, text.FontSize, Brushes.Black);
                var geometry = formatted.BuildGeometry(new Point(0, 0)); Assert.NotNull(geometry); var ink = geometry!.Bounds;
                var at = node.TranslatePoint(new Point(0, 0), window)!.Value;
                top = at.Y + ink.Top; bottom = at.Y + ink.Bottom; left = at.X + ink.Left;
            }
            else if (node is XYIcon icon)
            {
                var at = icon.TranslatePoint(new Point(0, 0), window)!.Value;
                top = at.Y; bottom = at.Y + icon.Bounds.Height; left = at.X;
            }
            else continue;
            b.AppendLine(CultureInfo.InvariantCulture,
                $"kind={(node is TextBlock ? "TEXT" : "ICON")}|innerH={innerH.ToString("F1", f)}|inkTop={top.ToString("F2", f)} inkBottom={bottom.ToString("F2", f)}|" +
                $"shiftNeededDown={((top + bottom) / 2 - centerY).ToString("F2", f)}|leftInset={(left - innerLeft).ToString("F2", f)}");
        }
        return b.ToString().TrimEnd();
    }

    static string TextLine(string report) => report.Split('\n').First(l => l.Contains("kind=TEXT"));
    static double Parse(string line, string marker) =>
        double.Parse(line.Split('|').First(p => p.StartsWith(marker))[(marker.Length + 1)..],
            CultureInfo.InvariantCulture);
}
