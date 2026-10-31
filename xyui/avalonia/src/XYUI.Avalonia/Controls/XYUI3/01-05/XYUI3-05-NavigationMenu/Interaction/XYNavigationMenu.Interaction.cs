namespace XYUI.Avalonia.Controls;

public sealed partial class XYNavigationMenu
{
    public void CommitDestination(string id) => NavigationState.Select(id);
}
