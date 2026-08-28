using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYComboBox
{
    internal static FuncControlTemplate<XYComboBox> CreateTemplate() => new((control, scope) =>
    {
        var text = new XYTextField { Name = "PART_TextField", VerticalAlignment = VerticalAlignment.Stretch, Background = Brushes.Transparent, BorderBrush = Brushes.Transparent, BorderThickness = new Thickness(0), CornerRadius = new CornerRadius(0) };
        text.Classes.Add("xyui-combo-embedded");
        var icon = new XYIcon { Icon = XyuiVectorIcon.ChevronDown, Size = XyuiIconSize.Small };
        var chevron = new Button { Name = "PART_Chevron", Width = 32, MinWidth = 32, MaxWidth = 32, HorizontalAlignment = HorizontalAlignment.Left, Content = icon, Focusable = false, Padding = new Thickness(0), BorderThickness = new Thickness(0) };
        var chevronCell = new Border { Name = "PART_ChevronCell", Width = 32, HorizontalAlignment = HorizontalAlignment.Stretch, VerticalAlignment = VerticalAlignment.Stretch, Child = chevron };
        var content = new Grid { ColumnDefinitions = new ColumnDefinitions { new ColumnDefinition(1, GridUnitType.Star), new ColumnDefinition(32, GridUnitType.Pixel) }, Children = { text, chevronCell } }; Grid.SetColumn(chevronCell, 1);
        var border = new Border { Name = "PART_Chrome", Child = content }; border[!Border.BackgroundProperty] = control[!TemplatedControl.BackgroundProperty]; border[!Border.BorderBrushProperty] = control[!TemplatedControl.BorderBrushProperty]; border[!Border.BorderThicknessProperty] = control[!TemplatedControl.BorderThicknessProperty]; border[!Border.CornerRadiusProperty] = control[!TemplatedControl.CornerRadiusProperty];
        var edge = new Border { Name = "PART_FocusEdge", Height = 3, VerticalAlignment = VerticalAlignment.Bottom, IsHitTestVisible = false };
        var list = new ListBox { Name = "PART_List", MinHeight = 28, MaxHeight = 240 }; var popupSurface = new Border { Name = "PART_PopupSurface", Child = list }; var popup = new Popup { Name = "PART_Popup", Height = 0, IsVisible = false, Placement = PlacementMode.Bottom, IsLightDismissEnabled = false, Child = popupSurface };
        scope?.Register("PART_TextField", text); scope?.Register("PART_Chevron", chevron); scope?.Register("PART_ChevronCell", chevronCell); scope?.Register("PART_Chrome", border); scope?.Register("PART_FocusEdge", edge); scope?.Register("PART_List", list); scope?.Register("PART_PopupSurface", popupSurface); scope?.Register("PART_Popup", popup);
        return new Grid { Children = { border, edge, popup } };
    });

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (TextFieldPart is not null) { TextFieldPart.PropertyChanged -= OnTextFieldChanged; TextFieldPart.KeyDown -= OnTextKeyDown; }
        if (ListPart is not null) ListPart.SelectionChanged -= OnListSelectionChanged;
        base.OnApplyTemplate(e); TextFieldPart = e.NameScope.Find<XYTextField>("PART_TextField"); ChevronPart = e.NameScope.Find<Button>("PART_Chevron"); PopupPart = e.NameScope.Find<Popup>("PART_Popup"); ListPart = e.NameScope.Find<ListBox>("PART_List");
        if (TextFieldPart is null || ChevronPart is null || PopupPart is null || ListPart is null) return;
        TextFieldPart.PropertyChanged += OnTextFieldChanged; TextFieldPart.KeyDown += OnTextKeyDown; ChevronPart.Click += OnChevronClick; ListPart.SelectionChanged += OnListSelectionChanged; TextFieldPart.Placeholder = Placeholder; SyncText(); RefreshItems(false); if (IsDropDownOpen) OpenPopup();
    }

    void OnTextFieldChanged(object? sender, AvaloniaPropertyChangedEventArgs e) { if (e.Property == TextBox.TextProperty && !SyncingText) { Text = TextFieldPart!.Text; RefreshItems(true); } }
    void OnTextKeyDown(object? sender, KeyEventArgs e) => OnComboKeyDown(e);
    void OnChevronClick(object? sender, RoutedEventArgs e) => ToggleDropDown();
    void OnListSelectionChanged(object? sender, SelectionChangedEventArgs e) { if (!IsKeyboardSelecting && ListPart?.SelectedItem is object item) SelectItem(item); }
    void SyncText() { if (TextFieldPart is not null && TextFieldPart.Text != Text) { SyncingText = true; TextFieldPart.Text = Text; SyncingText = false; } }
    internal void ToggleDropDown() { if (IsDropDownOpen && ShowingAllItems) IsDropDownOpen = false; else { ShowingAllItems = true; RefreshItems(false); IsDropDownOpen = true; } }
    void OpenPopup() { if (PopupPart is null) return; PopupPart.Height = double.NaN; PopupPart.IsVisible = true; PopupPart.PlacementTarget = this; PopupPart.Width = Bounds.Width; PopupPart.IsOpen = true; }
    void SelectItem(object item) { SelectedItem = item; Text = item.ToString(); SyncText(); IsError = false; IsDropDownOpen = false; }
}
