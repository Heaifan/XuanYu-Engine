namespace XuanYu.Editor.UI;

public enum UiMotionPreference
{
    Reduce,
    Default
}

public static class UiMotionContract
{
    public const int FastMs = 80;
    public const int StandardMs = 120;
    public const int SlowMs = 180;

    public static bool AllowsNonEssentialTransitions(UiMotionPreference preference) =>
        preference == UiMotionPreference.Default;

    public static int EffectiveHoverMs(UiMotionPreference preference) =>
        AllowsNonEssentialTransitions(preference) ? FastMs : 0;

    public static int EffectiveDialogMs(UiMotionPreference preference) =>
        AllowsNonEssentialTransitions(preference) ? StandardMs : 0;
}
