using System.Globalization;

namespace XuanYu.Core.Time;

public readonly record struct TimeStep
{
    private TimeStep(double seconds)
    {
        Seconds = seconds;
    }

    public double Seconds { get; }

    public double Milliseconds => Seconds * 1000.0;

    public bool IsPositive => Seconds > 0.0;

    public static TimeStep FromSeconds(double seconds)
    {
        if (!double.IsFinite(seconds) || seconds <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "时间步长必须是有限数，并且必须大于 0。");
        }

        return new TimeStep(seconds);
    }

    public static TimeStep FromMilliseconds(double milliseconds)
    {
        if (!double.IsFinite(milliseconds) || milliseconds <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(milliseconds), milliseconds, "时间步长必须是有限数，并且必须大于 0。");
        }

        var seconds = milliseconds / 1000.0;
        if (seconds == 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(milliseconds), milliseconds, "毫秒转换后的时间步长不能为零。");
        }

        return new TimeStep(seconds);
    }

    public override string ToString()
    {
        return $"TimeStep({Seconds.ToString("0.###", CultureInfo.InvariantCulture)}s)";
    }
}
