using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;
using XYUI.Avalonia.Gallery;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2PasswordFieldTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2PasswordFieldTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void PasswordField_masks_text_and_keeps_placeholder_and_gallery_samples() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-16"); Assert.Equal(2, preview.GetVisualDescendants().OfType<XYPasswordField>().Count()); var field = new XYPasswordField { Width = 360, Password = "部署密钥" }; var window = XyuiBatchTestHost.Show(field);
        Assert.False(field.IsRevealed); Assert.Equal("●●●●", field.PasswordPresenterPart!.Text); Assert.Equal(10, field.TextPaddingPart!.Padding.Left); Assert.Equal(8, field.TextPaddingPart.Padding.Right); Assert.Equal(32, field.RevealPart!.Bounds.Width); Assert.Equal(32, field.RevealPart.Bounds.Height); window.Close();
    });

    [Fact]
    public void PasswordField_first_focus_selects_all_and_reveal_preserves_selection() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYPasswordField { Width = 280, Password = "部署密钥" }; var window = XyuiBatchTestHost.Show(field); field.Focus(); Assert.Equal(0, field.SelectionStart); Assert.Equal(field.Password.Length, field.SelectionEnd); field.SetRevealed(true); Assert.True(field.IsRevealed); Assert.Equal(field.Password, field.PasswordPresenterPart!.Text); Assert.Equal(0, field.SelectionStart); Assert.Equal(field.Password.Length, field.SelectionEnd); field.ForceHidePassword(); Assert.False(field.IsRevealed); Assert.Equal("●●●●", field.PasswordPresenterPart.Text); window.Close();
    });

    [Fact]
    public void PasswordField_keyboard_reveal_hides_on_key_up_and_disabled_forces_hide() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYPasswordField { Width = 280, Password = "密码" }; var window = XyuiBatchTestHost.Show(field); var eye = field.RevealPart!; eye.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Space }); Assert.True(field.IsRevealed); eye.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyUpEvent, Key = Key.Space }); Assert.False(field.IsRevealed); field.SetRevealed(true); field.IsEnabled = false; Dispatcher.UIThread.RunJobs(); Assert.False(field.IsRevealed); Assert.False(eye.IsEnabled); window.Close();
    });

    [Fact]
    public void PasswordField_real_pointer_press_reveals_and_release_hides() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYPasswordField { Width = 280, Password = "密码" }; var window = XyuiBatchTestHost.Show(field); var eye = field.RevealPart!; var point = eye.TranslatePoint(new Point(16, 16), window)!.Value;
        window.MouseMove(point); window.MouseDown(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); Assert.True(field.IsRevealed); Assert.Equal(field.Password, field.PasswordPresenterPart!.Text); window.MouseUp(point, MouseButton.Left); Dispatcher.UIThread.RunJobs(); Assert.False(field.IsRevealed); Assert.Equal("●●", field.PasswordPresenterPart.Text); window.Close();
    });

    [Fact]
    public void PasswordField_capture_loss_and_detach_force_mask() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var field = new XYPasswordField { Width = 280, Password = "密码" }; var window = XyuiBatchTestHost.Show(field); field.SetRevealed(true); field.ForceHidePassword(); Assert.False(field.IsRevealed); field.SetRevealed(true); window.Content = null; Assert.False(field.IsRevealed); window.Close();
    });
}
