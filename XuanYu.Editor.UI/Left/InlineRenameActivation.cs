namespace XuanYu.Editor.UI;

public static class InlineRenameActivation
{
    public static void Schedule(
        Func<bool> isVisible,
        Action<Action> postAfterLayout,
        Action focus,
        Action selectAll)
    {
        if (!isVisible()) return;
        postAfterLayout(() =>
        {
            if (!isVisible()) return;
            focus();
            selectAll();
        });
    }
}
