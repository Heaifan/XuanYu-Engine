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
        XyuiBatchTestHost.Prepare(); var preview = XYUI2GalleryCatalog.CreatePreview("XYUI-2-16"); Assert.True(preview.GetVisualDescendants().OfType<XYPasswordField>().Count() >= 3); var field = new XYPasswordField { Width = 360, Password = "部署密钥" }; var window = XyuiBatchTestHost.Show(field);
        Assert.False(field.IsRevealed); Assert.Equal("●●●●", field.PasswordPresenterPart!.Text); Assert.Equal(34, field.RevealPart!.Bounds.Width); window.Close();
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
}
