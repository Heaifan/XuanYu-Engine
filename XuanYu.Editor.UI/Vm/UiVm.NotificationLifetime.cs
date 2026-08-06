namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    public bool ShouldAutoDismissNotification(DateTime now, TimeSpan lifetime) =>
        HasNotification && lifetime > TimeSpan.Zero && now - CreatedAt >= lifetime;
}
