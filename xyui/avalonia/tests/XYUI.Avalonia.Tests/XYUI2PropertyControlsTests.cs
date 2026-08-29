using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using XYUI.Avalonia.Controls;

namespace XYUI.Avalonia.Tests;

[Collection("XyuiHeadless")]
public sealed class XYUI2PropertyControlsTests : IClassFixture<XyuiHeadlessFixture>
{
    readonly XyuiHeadlessFixture _fx;
    public XYUI2PropertyControlsTests(XyuiHeadlessFixture fx) => _fx = fx;

    [Fact]
    public void Number_property_reuses_number_field_and_syncs_value() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = new XYNumberProperty { Label = "速度", Value = 2, Step = .5, Suffix = "米/秒" }; var w = XyuiBatchTestHost.Show(p);
        var field = Assert.IsType<XYNumberField>(p.ValueFieldPart); Assert.Same(field, p.GetVisualDescendants().OfType<XYNumberField>().Single()); Assert.True(field.IsScrubEnabled);
        Assert.Equal(3, p.RowPart!.ColumnDefinitions.Count); Assert.Equal("2.00", field.Text);
        field.Value = 3.5; Dispatcher.UIThread.RunJobs(); Assert.Equal(3.5, p.Value); p.Value = 4; Assert.Equal(4, field.Value);
        p.IsReadOnly = true; Assert.True(field.IsReadOnly); p.IsEnabled = false; Assert.False(field.IsEnabled); w.Close();
    });

    [Fact]
    public void Vector_property_reuses_number_fields_and_preserves_other_axes() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = new XYVectorProperty { Width = 620, Dimension = XYVectorDimension.Vector3, X = 1, Y = 0, Z = 0 }; var w = XyuiBatchTestHost.Show(p);
        Assert.Equal(4, p.AxisFields.Count); Assert.Equal(3, p.AxisHosts.Count(x => x.IsVisible)); var y = p.AxisFields[1]; y.Value = 8; Dispatcher.UIThread.RunJobs();
        Assert.Equal("0.00", p.AxisFields[2].Text); Assert.Equal(p.AxisHosts[0].Bounds.Y, p.AxisHosts[2].Bounds.Y); Assert.True(p.AxisHosts[0].Bounds.Width > 0);
        Assert.Equal(1, p.X); Assert.Equal(8, p.Y); Assert.Equal(0, p.Z); Assert.True(y.IsScrubEnabled); p.Dimension = XYVectorDimension.Vector2; Assert.False(p.AxisHosts[2].IsVisible); w.Close();
    });

    [Fact]
    public void Vector_property_stacks_label_before_axes_when_width_is_not_wide() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = new XYVectorProperty { Width = 420, Dimension = XYVectorDimension.Vector3 }; var w = XyuiBatchTestHost.Show(p);
        Assert.Single(p.RowPart!.ColumnDefinitions); Assert.True(p.RowPart.RowDefinitions.Count == 2); Assert.Equal(1, Grid.GetRow(p.AxisPanelPart!));
        Assert.All(p.AxisHosts.Take(3), host => Assert.True(host.Bounds.Width > 0)); w.Close();
    });

    [Fact]
    public void Enum_property_reuses_select_and_syncs_selection() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = new XYEnumProperty { Label = "模式", ItemsSource = new[] { "实体", "线框", "点" }, SelectedIndex = 0 }; var w = XyuiBatchTestHost.Show(p);
        var select = Assert.IsType<XYSelect>(p.SelectPart); Assert.Same(select, p.GetVisualDescendants().OfType<XYSelect>().Single()); select.ListPart!.SelectedIndex = 2; Dispatcher.UIThread.RunJobs();
        Assert.Equal(3, p.RowPart!.ColumnDefinitions.Count);
        Assert.Equal(2, p.SelectedIndex); Assert.Equal("点", p.SelectedItem); p.IsReadOnly = true; Assert.False(select.IsEnabled); w.Close();
    });

    [Fact]
    public void Reference_property_uses_icon_actions_and_picker_lifecycle() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var list = new ListBox { ItemsSource = new[] { new XYReferenceValue("实体二", "Entity", "E2") } }; var p = new XYReferenceProperty { Reference = new("实体一", "Entity", "E1"), ReferenceState = XYReferenceState.Resolved, ExpectedType = "Entity", ReferencePickerContent = list }; var w = XyuiBatchTestHost.Show(p);
        Assert.Equal(3, p.GetVisualDescendants().OfType<XYIconButton>().Count()); var located = 0; p.LocateRequested += (_, _) => located++; p.LocatePart!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.Equal(1, located);
        p.BrowsePart!.RaiseEvent(new RoutedEventArgs(Button.ClickEvent)); Assert.True(p.IsPickerOpen); list.SelectedIndex = 0; Dispatcher.UIThread.RunJobs(); Assert.Equal("E2", p.ReferenceId); Assert.False(p.IsPickerOpen);
        Assert.False(p.TryAssignReference(new("道路", "Dataset", "D1"))); Assert.Equal("E2", p.ReferenceId); p.ClearReference(); Assert.Equal(XYReferenceState.Empty, p.ReferenceState); w.Close();
    });

    [Fact]
    public void Reference_property_closes_empty_state_and_reflows_narrow_layout() => _fx.Run(() =>
    {
        XyuiBatchTestHost.Prepare(); var p = new XYReferenceProperty { Width = 280, ReferenceState = XYReferenceState.Resolved }; var w = XyuiBatchTestHost.Show(p);
        Assert.Equal(XYReferenceState.Empty, p.ReferenceState); Assert.Equal("未设置引用", p.IdentityPart!.Text); Assert.False(p.IdentityPart.IsVisible);
        Assert.Equal(1, Grid.GetRow(p.ActionsPart!)); Assert.All(new[] { p.LocatePart!, p.BrowsePart!, p.ClearPart! }, button => Assert.True(button.Width >= 34)); w.Close();
    });
}
