namespace XuanYu.Editor.MapEditing;

public readonly record struct RegionEdgeSnapSettings(
    double EnterRadiusPx,
    double ReleaseRadiusPx)
{
    public static RegionEdgeSnapSettings Default { get; } = new(8, 12);

    public RegionEdgeSnapSettings Validate()
    {
        if (!double.IsFinite(EnterRadiusPx) || !double.IsFinite(ReleaseRadiusPx) ||
            EnterRadiusPx <= 0 || ReleaseRadiusPx < EnterRadiusPx)
            throw new ArgumentOutOfRangeException(nameof(EnterRadiusPx), "吸附半径必须为正且释放半径不得小于进入半径。");
        return this;
    }
}
