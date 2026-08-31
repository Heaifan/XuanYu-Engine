using Avalonia.Controls;
using Avalonia.Layout;
using XYUI.Avalonia.Vector;

namespace XYUI.Avalonia.Controls;

public enum XYStepState { Completed, Current, Pending, Warning, Error }
public sealed class XYStepNode : Border
{
    public string Label { get; }
    public XYStepState State { get; }
    public XYStepNode(string label, XYStepState state) { Label = label; State = state; Classes.Add("xyui-step-node"); Build(); }
    void Build()
    {
        Classes.Set("xyui-step-completed", State == XYStepState.Completed); Classes.Set("xyui-step-current", State == XYStepState.Current); Classes.Set("xyui-step-pending", State == XYStepState.Pending); Classes.Set("xyui-step-warning", State == XYStepState.Warning); Classes.Set("xyui-step-error", State == XYStepState.Error);
        var icon = State == XYStepState.Completed ? XyuiVectorIcon.StatusDot : State == XYStepState.Warning ? XyuiVectorIcon.Warning : State == XYStepState.Error ? XyuiVectorIcon.Error : XyuiVectorIcon.StatusDot;
        var dot = new Border { Width = State == XYStepState.Current ? 26 : 24, Height = State == XYStepState.Current ? 26 : 24, Child = new XYIcon { Icon = icon, Size = XyuiIconSize.Tiny, HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center } };
        Child = new StackPanel { Spacing = 8, HorizontalAlignment = HorizontalAlignment.Center, Children = { dot, new TextBlock { Text = Label, VerticalAlignment = VerticalAlignment.Center } } };
    }
}
