using System.Globalization;

namespace FluidWarfare.Core.Time;

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
            throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "Time step must be finite and greater than zero.");
        }

        return new TimeStep(seconds);
    }

    public static TimeStep FromMilliseconds(double milliseconds)
    {
        if (!double.IsFinite(milliseconds) || milliseconds <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(milliseconds), milliseconds, "Time step must be finite and greater than zero.");
        }

        return new TimeStep(milliseconds / 1000.0);
    }

    public override string ToString()
    {
        return $"TimeStep({Seconds.ToString("0.###", CultureInfo.InvariantCulture)}s)";
    }
}
