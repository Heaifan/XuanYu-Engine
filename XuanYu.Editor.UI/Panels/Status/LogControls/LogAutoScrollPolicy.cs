namespace XuanYu.Editor.UI;

// MAP-A-R2-D3-F2：日志自动跟随纯策略——底部附近跟随、远离不强制拉回、滚到底恢复。
public static class LogAutoScrollPolicy
{
    // "底部附近"= 距最大滚动值不超过约一行高度（20 DIP）。
    public const double FollowThresholdDips = 20.0;

    public static bool ShouldFollow(double offset, double maxOffset) =>
        maxOffset <= 0 || offset >= maxOffset - FollowThresholdDips;
}
