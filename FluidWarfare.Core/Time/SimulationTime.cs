using System.Globalization;

namespace FluidWarfare.Core.Time;

public readonly record struct SimulationTime
{
    private SimulationTime(double seconds)
    {
        Seconds = seconds;
    }

    public double Seconds { get; }

    public double Milliseconds => Seconds * 1000.0;

    public bool IsZero => Seconds == 0.0;

    public static SimulationTime Zero { get; } = new(0.0);

    public static SimulationTime FromSeconds(double seconds)
    {
        if (!double.IsFinite(seconds) || seconds < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "Simulation time must be finite and zero or greater.");
        }

        return new SimulationTime(seconds);
    }

    public static SimulationTime FromMilliseconds(double milliseconds)
    {
        if (!double.IsFinite(milliseconds) || milliseconds < 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(milliseconds), milliseconds, "Simulation time must be finite and zero or greater.");
        }

        return new SimulationTime(milliseconds / 1000.0);
    }

    public SimulationTime Advance(TimeStep step)
    {
        if (!step.IsPositive)
        {
            throw new ArgumentOutOfRangeException(nameof(step), step, "Time step must be positive.");
        }

        return new SimulationTime(Seconds + step.Seconds);
    }

    public override string ToString()
    {
        return $"SimulationTime({Seconds.ToString("0.###", CultureInfo.InvariantCulture)}s)";
    }
}
