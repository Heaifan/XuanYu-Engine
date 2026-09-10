using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2AlignmentContractTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2AlignmentContractTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Text_and_icon_text_buttons_center_the_whole_content()
    {
        _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var textWindow = XyuiBatchTestHost.Show(new XYButton { Content = "应用修改" });
            var iconTextWindow = XyuiBatchTestHost.Show(new XYButton
            {
                Icon = XyuiVectorIcon.Save, Content = "保存"
            });
            AssertCentered(textWindow.Content as XYButton);
            AssertCentered(iconTextWindow.Content as XYButton);
            var textButton = (Control)textWindow.Content!;
            var iconTextButton = (Control)iconTextWindow.Content!;
            AssertCenteredContent(textButton, textButton.GetVisualDescendants().OfType<TextBlock>().Single());
            AssertCenteredContent(iconTextButton, iconTextButton.GetVisualDescendants().OfType<StackPanel>().Single());
            textWindow.Close();
            iconTextWindow.Close();
        });
    }

    [Fact]
    public void Icon_button_and_toggle_center_their_icon_hosts()
    {
        _fx.Run(() =>
        {
            XyuiBatchTestHost.Prepare();
            var iconButton = new XYIconButton { Icon = XyuiVectorIcon.Eye };
            var toggle = new XYToggleButton { Content = new Border { Width = 16, Height = 16 } };
            var iconWindow = XyuiBatchTestHost.Show(iconButton);
            var toggleWindow = XyuiBatchTestHost.Show(toggle);
            AssertCentered(iconButton);
            AssertCenteredContent(iconButton, iconButton.GetVisualDescendants().OfType<XYIcon>().Single());
            AssertCentered(toggle);
            AssertCenteredContent(toggle, toggle.GetVisualDescendants().OfType<Border>().Single(x => x.Width == 16));
            iconWindow.Close();
            toggleWindow.Close();
        });
    }

    static void AssertCentered(Control? control)
    {
        Assert.NotNull(control);
        var presenter = control!.GetVisualDescendants().OfType<ContentPresenter>().Single();
        var center = presenter.TranslatePoint(new Point(presenter.Bounds.Width / 2, presenter.Bounds.Height / 2), control)
                     ?? new Point(presenter.Bounds.Width / 2, presenter.Bounds.Height / 2);
        Assert.Equal(control.Bounds.Width / 2, center.X, 0.5);
        Assert.Equal(control.Bounds.Height / 2, center.Y, 0.5);
    }

    static void AssertCenteredContent(Control host, Control content)
    {
        var center = content.TranslatePoint(new Point(content.Bounds.Width / 2, content.Bounds.Height / 2), host);
        Assert.NotNull(center);
        Assert.True(Math.Abs(host.Bounds.Width / 2 - center!.Value.X) <= 0.5,
            $"host={host.Bounds}, content={content.Bounds}, parent={content.Parent?.GetType().Name}, alignment={(host as Button)?.HorizontalContentAlignment}");
        Assert.True(Math.Abs(host.Bounds.Height / 2 - center.Value.Y) <= 0.5,
            $"host={host.Bounds}, content={content.Bounds}, parent={content.Parent?.GetType().Name}");
    }

}
