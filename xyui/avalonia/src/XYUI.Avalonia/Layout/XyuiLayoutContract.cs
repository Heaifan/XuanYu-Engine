namespace XYUI.Avalonia.Layout;

public enum XyuiLayoutRecipe
{
    Toolbar, Form, Inspector, Menu, Popup, Dialog, PropertyGrid,
}

public sealed record XyuiLayoutContract(
    XyuiLayoutRecipe Recipe,
    bool ComponentOwnsPadding,
    bool ParentOwnsSiblingGap,
    bool MarginIsSiblingLayoutTool);

public static class XyuiLayoutContracts
{
    public static XyuiLayoutContract For(XyuiLayoutRecipe recipe) =>
        new(recipe, ComponentOwnsPadding: true, ParentOwnsSiblingGap: true,
            MarginIsSiblingLayoutTool: false);
}
