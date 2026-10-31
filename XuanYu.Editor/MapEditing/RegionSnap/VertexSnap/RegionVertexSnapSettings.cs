namespace XuanYu.Editor.MapEditing;

public readonly record struct RegionVertexSnapSettings(
    double EnterRadiusPx,
    double ReleaseRadiusPx)
{
    public static RegionVertexSnapSettings Default { get; } = new(8, 12);

    public RegionVertexSnapSettings Validate()
    {
        if (!double.IsFinite(EnterRadiusPx) || !double.IsFinite(ReleaseRadiusPx) ||
            EnterRadiusPx <= 0 || ReleaseRadiusPx < EnterRadiusPx)
            throw new ArgumentOutOfRangeException(nameof(EnterRadiusPx), "吸附半径必须为正且释放半径不得小于进入半径。");
        return this;
    }
}
