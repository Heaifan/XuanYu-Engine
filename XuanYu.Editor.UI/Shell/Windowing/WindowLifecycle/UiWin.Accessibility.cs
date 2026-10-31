using System;

namespace XuanYu.Editor.UI;

public partial class UiWin
{
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        UiAutomationNamer.Apply(this);
    }
}
