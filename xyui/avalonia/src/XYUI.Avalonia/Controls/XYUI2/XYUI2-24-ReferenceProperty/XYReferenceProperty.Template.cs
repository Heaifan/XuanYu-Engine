using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Automation;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public partial class XYReferenceProperty
{
    internal static FuncControlTemplate<XYReferenceProperty> CreateTemplate() => new((control, scope) =>
    {
        var label = new TextBlock { Name = "PART_Label", Text = control.Label, VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
        var type = new Border { Name = "PART_TypeCell", Width = 25, Child = new XYIcon { Icon = XyuiVectorIcon.Tag, Size = XyuiIconSize.Small } };
        var name = new TextBlock { Name = "PART_Name", VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
        var identity = new TextBlock { Name = "PART_Identity", VerticalAlignment = VerticalAlignment.Center, TextTrimming = TextTrimming.CharacterEllipsis };
        var text = new StackPanel { Spacing = 1, Children = { name, identity } };
        var locate = Action("PART_Locate", XyuiVectorIcon.Locate, "定位", control.OnLocate); var browse = Action("PART_Browse", XyuiVectorIcon.Browse, "浏览", control.OnBrowse); var clear = Action("PART_Clear", XyuiVectorIcon.Clear, "清除", control.OnClear);
        var actions = new StackPanel { Name = "PART_Actions", Orientation = Orientation.Horizontal, Spacing = 2, Children = { locate, browse, clear } };
        var field = new Border { Name = "PART_ReferenceField", Padding = new Thickness(4, 0), Child = new Grid { ColumnDefinitions = new ColumnDefinitions("25,*,Auto"), Children = { type, text, actions } } }; Grid.SetColumn(text, 1); Grid.SetColumn(actions, 2);
        var content = (Grid)field.Child; var pickerContent = new ContentControl { Name = "PART_PickerContent" }; pickerContent[!ContentControl.ContentProperty] = control[!ReferencePickerContentProperty];
        var popupSurface = new Border { Padding = new Thickness(8), Child = pickerContent }; var popup = new Popup { Name = "PART_Popup", Height = 0, IsVisible = false, Placement = PlacementMode.Bottom, IsLightDismissEnabled = true, Child = popupSurface };
        var row = new Grid { Name = "PART_Row", Children = { label, field, popup } }; XYPropertyLayoutMetrics.ConfigureRow(row, label, field, 0); control.LabelPart = label; control.ReferenceFieldPart = field; control.ContentPart = content; control.ActionsPart = actions; control.RowPart = row; control.NamePart = name; control.IdentityPart = identity; control.LocatePart = locate; control.BrowsePart = browse; control.ClearPart = clear; control.PopupPart = popup;
        DragDrop.SetAllowDrop(field, true); DragDrop.AddDragOverHandler(field, control.OnDragOver); DragDrop.AddDropHandler(field, control.OnDrop); popup.Closed += control.OnPopupClosed;
        scope?.Register("PART_Label", label); scope?.Register("PART_ReferenceField", field); scope?.Register("PART_Name", name); scope?.Register("PART_Identity", identity); scope?.Register("PART_Actions", actions); scope?.Register("PART_Locate", locate); scope?.Register("PART_Browse", browse); scope?.Register("PART_Clear", clear); scope?.Register("PART_Popup", popup); return row;
    });
    static XYIconButton Action(string partName, XyuiVectorIcon icon, string name, EventHandler<RoutedEventArgs> handler) { var button = new XYIconButton { Name = partName, Content = new XYIcon { Icon = icon, Size = XyuiIconSize.Small } }; button.SetValue(AutomationProperties.NameProperty, name); button.Click += handler; return button; }
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) { base.OnApplyTemplate(e); LabelPart = e.NameScope.Find<TextBlock>("PART_Label") ?? LabelPart; ReferenceFieldPart = e.NameScope.Find<Border>("PART_ReferenceField") ?? ReferenceFieldPart; ContentPart = ReferenceFieldPart?.Child as Grid ?? ContentPart; NamePart = e.NameScope.Find<TextBlock>("PART_Name") ?? NamePart; IdentityPart = e.NameScope.Find<TextBlock>("PART_Identity") ?? IdentityPart; ActionsPart = e.NameScope.Find<StackPanel>("PART_Actions") ?? ActionsPart; LocatePart = e.NameScope.Find<XYIconButton>("PART_Locate") ?? LocatePart; BrowsePart = e.NameScope.Find<XYIconButton>("PART_Browse") ?? BrowsePart; ClearPart = e.NameScope.Find<XYIconButton>("PART_Clear") ?? ClearPart; PopupPart = e.NameScope.Find<Popup>("PART_Popup") ?? PopupPart; RowPart = e.NameScope.Find<Grid>("PART_Row") ?? RowPart; AttachPicker(); SyncParts(); UpdateLayoutMode(); }
}
