using Avalonia.Controls;
using Avalonia.Interactivity;

using XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual;
namespace XuanYu.Engine.Editor.Windows.Panels.HierarchyVisual;

public sealed partial class HierarchyNodeRow : UserControl
{
    public static readonly RoutedEvent<HierarchyExpansionRequestedEventArgs>
        ExpansionRequestedEvent =
            RoutedEvent.Register<
                HierarchyNodeRow,
                HierarchyExpansionRequestedEventArgs>(
                nameof(ExpansionRequested),
                RoutingStrategies.Bubble);

    public event EventHandler<HierarchyExpansionRequestedEventArgs>
        ExpansionRequested
    {
        add => AddHandler(ExpansionRequestedEvent, value);
        remove => RemoveHandler(ExpansionRequestedEvent, value);
    }

    public HierarchyNodeRow()
    {
        InitializeComponent();
    }

    private void OnToggleClicked(
        object? sender,
        RoutedEventArgs eventArgs)
    {
        if (DataContext is not IHierarchyNodeView node ||
            !node.HasChildren)
        {
            return;
        }

        RaiseEvent(
            new HierarchyExpansionRequestedEventArgs(
                ExpansionRequestedEvent,
                node));

        eventArgs.Handled = true;
    }
}

public sealed class HierarchyExpansionRequestedEventArgs : RoutedEventArgs
{
    public HierarchyExpansionRequestedEventArgs(
        RoutedEvent routedEvent,
        IHierarchyNodeView node)
        : base(routedEvent)
    {
        Node = node;
    }

    public IHierarchyNodeView Node { get; }
}
