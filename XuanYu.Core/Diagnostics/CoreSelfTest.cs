using XuanYu.Core.Logging;
using XuanYu.Core.Math;
using XuanYu.Core.Results;
using XuanYu.Core.Time;

namespace XuanYu.Core.Diagnostics;

public static class CoreSelfTest
{
    public static CoreSelfTestReport Run()
    {
        var items = new List<CoreSelfTestItem>
        {
            Check("EngineError.Create", () =>
            {
                var error = EngineError.Create("Core.Test", "ok");
                return error.IsValid && error.Code == "Core.Test";
            }),
            Check("EngineResult.Fail", () =>
            {
                var error = EngineError.Create("Core.Fail", "failed");
                var result = EngineResult.Fail(error);
                return result.IsFailure && result.Error?.Code == "Core.Fail";
            }),
            Check("Vector3d math", () =>
            {
                var a = new Vector3d(1, 2, 3);
                var b = new Vector3d(4, 5, 6);
                var sum = a + b;
                return sum == new Vector3d(5, 7, 9) && a.DistanceTo(b) > 0.0;
            }),
            Check("YawRotation normalize", () =>
            {
                var yaw = YawRotation.FromDegrees(450.0);
                return global::System.Math.Abs(yaw.Degrees - 90.0) < 0.000001 &&
                       global::System.Math.Abs(yaw.ForwardOnXZPlane.X - 1.0) < 0.000001;
            }),
            Check("SimulationTime advance", () =>
            {
                var time = SimulationTime.FromSeconds(1.0).Advance(TimeStep.FromMilliseconds(500.0));
                return global::System.Math.Abs(time.Seconds - 1.5) < 0.000001;
            }),
            Check("EngineLogEntry format", () =>
            {
                var entry = EngineLogEntry.Create(1.0, EngineLogLevel.Info, "Core", "Ready");
                return entry.ToDisplayString().Contains("Ready", StringComparison.Ordinal);
            })
        };

        return new CoreSelfTestReport(items);
    }

    static CoreSelfTestItem Check(string name, Func<bool> test)
    {
        try
        {
            return new CoreSelfTestItem(name, test(), "");
        }
        catch (Exception ex)
        {
            return new CoreSelfTestItem(name, false, ex.Message);
        }
    }
}

public sealed record CoreSelfTestItem(string Name, bool Passed, string Detail);

public sealed record CoreSelfTestReport(IReadOnlyList<CoreSelfTestItem> Items)
{
    public bool IsPassed => Items.All(item => item.Passed);
}
