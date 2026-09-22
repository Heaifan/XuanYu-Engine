using Avalonia.Controls;
using Avalonia.Layout;

namespace XYUI.Avalonia.Controls;

public sealed class XYContextActionPane : Border
{
    readonly StackPanel _panel = new();
    public IReadOnlyList<XYContextAction> Actions { get; private set; } = [];
    public XYContextAction? SelectedAction { get; private set; }
    public event EventHandler<XYContextAction>? ActionExecuted;
    public XYContextActionPane() { Classes.Add("xyui-context-action-pane"); Child = _panel; }
    public void SetActions(IEnumerable<XYContextAction> actions) { Actions = actions.ToArray(); SelectedAction = Actions.FirstOrDefault(); Refresh(); }
    public void SelectAction(string id) { SelectedAction = Actions.FirstOrDefault(x => x.Id == id) ?? SelectedAction; Refresh(); }
    public void Move(int delta)
    {
        if (Actions.Count == 0) return; var index = Math.Max(0, Array.IndexOf(Actions.ToArray(), SelectedAction) + delta); index = Math.Min(index, Actions.Count - 1); SelectedAction = Actions[index]; Refresh();
    }
    public void ExecuteSelected()
    {
        if (SelectedAction is not { IsEnabled: true } action) return; action.Execute?.Invoke(); ActionExecuted?.Invoke(this, action);
    }
    void Refresh()
    {
        _panel.Children.Clear();
        foreach (var action in Actions)
        {
            var button = new XYButton { Content = action.Label, Tag = action.Id, Variant = XyuiButtonVariant.Secondary, Classes = { "xyui-context-action" }, Height = 32, HorizontalContentAlignment = HorizontalAlignment.Left, IsEnabled = action.IsEnabled };
            if (action == SelectedAction) button.Classes.Add("xyui-context-action-selected"); button.Click += (_, _) => { SelectedAction = action; Refresh(); ExecuteSelected(); }; _panel.Children.Add(button);
        }
    }
}
