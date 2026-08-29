using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2ComboBoxTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2ComboBoxTests(XyuiHeadlessFixture fx) => _fx = fx;

    static readonly string[] Items = ["Alpha", "Beta", "North Region", "Northern Coast"];

    [Fact]
    public void ComboBox_has_real_editable_text_host_and_separate_chevron() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var combo = new XYComboBox { Width = 220, ItemsSource = Items, Placeholder = "选择地区" }; var window = XyuiBatchTestHost.Show(combo);
        Assert.True(combo.IsEditable); Assert.IsType<XYTextField>(combo.TextFieldPart); Assert.IsType<Button>(combo.ChevronPart); Assert.Equal(32, combo.ChevronPart!.Bounds.Width); Assert.Equal(32, combo.Bounds.Height); window.Close();
    });

    [Fact]
    public void ComboBox_filters_case_insensitively_without_replacing_source() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var combo = new XYComboBox { ItemsSource = Items }; var window = XyuiBatchTestHost.Show(combo); combo.TextFieldPart!.Text = "north";
        Assert.Equal(2, combo.FilteredItems.Count); Assert.Same(Items, combo.ItemsSource); Assert.Equal("north", combo.Text); window.Close();
    });

    [Fact]
    public void Chevron_opens_all_candidates_after_filter() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var combo = new XYComboBox { ItemsSource = Items }; var window = XyuiBatchTestHost.Show(combo); combo.TextFieldPart!.Text = "north";
        combo.ToggleDropDown(); Assert.True(combo.IsDropDownOpen); Assert.Equal(4, combo.ListPart!.ItemCount); window.Close();
    });

    [Fact]
    public void Selecting_candidate_syncs_text_item_and_closes_popup() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var combo = new XYComboBox { ItemsSource = Items }; var window = XyuiBatchTestHost.Show(combo); combo.IsDropDownOpen = true; combo.ListPart!.SelectedItem = "Northern Coast";
        Assert.Equal("Northern Coast", combo.SelectedItem); Assert.Equal("Northern Coast", combo.Text); Assert.False(combo.IsDropDownOpen); window.Close();
    });

    [Fact]
    public void ComboBox_keyboard_opens_navigates_selects_and_escapes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var combo = new XYComboBox { ItemsSource = Items }; var window = XyuiBatchTestHost.Show(combo); combo.TextFieldPart!.Focus();
        Raise(combo, Key.Down); Raise(combo, Key.Down); Raise(combo, Key.Enter); Assert.Equal("Beta", combo.Text); Assert.Equal("Beta", combo.SelectedItem); combo.IsDropDownOpen = true; Raise(combo, Key.Escape); Assert.False(combo.IsDropDownOpen); window.Close();
    });

    [Fact]
    public void ComboBox_placeholder_and_custom_value_contracts_are_distinct() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var fixedCombo = new XYComboBox { ItemsSource = Items, Placeholder = "选择地区" }; var window = XyuiBatchTestHost.Show(fixedCombo);
        Assert.Equal("选择地区", fixedCombo.TextFieldPart!.Placeholder); fixedCombo.TextFieldPart.Text = "Unknown"; Raise(fixedCombo, Key.Enter); Assert.True(fixedCombo.IsError); window.Close();
        var customCombo = new XYComboBox { ItemsSource = Items, IsCustomValueAllowed = true }; var second = XyuiBatchTestHost.Show(customCombo); customCombo.TextFieldPart!.Text = "Unknown"; Raise(customCombo, Key.Enter); Assert.False(customCombo.IsError); Assert.Null(customCombo.SelectedItem); Assert.Equal("Unknown", customCombo.Text); second.Close();
    });

    [Fact]
    public void ComboBox_closes_popup_when_gallery_host_detaches() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var combo = new XYComboBox { ItemsSource = Items }; var window = XyuiBatchTestHost.Show(combo); combo.IsDropDownOpen = true;
        Assert.True(combo.PopupPart!.IsOpen); window.Content = null;
        Assert.False(combo.IsDropDownOpen); Assert.False(combo.PopupPart.IsOpen); Assert.False(combo.PopupPart.IsVisible); window.Close();
    });

    static void Raise(XYComboBox combo, Key key) => combo.TextFieldPart!.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = key });
}
