using XuanYu.Core.Diagnostics;

namespace XuanYu.Core.Tests.Observability.Runtime;

public sealed class CoreSelfTestTests
{
    [Fact]
    public void EmptyReportCannotPass()
    {
        var report = new CoreSelfTestReport([]);

        Assert.False(report.IsPassed);
    }

    [Fact]
    public void FailedItemMakesReportFail()
    {
        var report = new CoreSelfTestReport([
            new CoreSelfTestItem("OK", true, ""),
            new CoreSelfTestItem("FAILED", false, "diagnostic")
        ]);

        Assert.False(report.IsPassed);
    }

    [Fact]
    public void NonEmptySuccessfulReportPasses()
    {
        var report = new CoreSelfTestReport([
            new CoreSelfTestItem("OK", true, "")
        ]);

        Assert.True(report.IsPassed);
    }

    [Fact]
    public void CoreSelfTestListsSixChecks()
    {
        var report = CoreSelfTest.Run();

        Assert.Equal(6, report.Items.Count);
        Assert.Contains(report.Items, item => item.Name == "EngineError.Create");
        Assert.Contains(report.Items, item => item.Name == "EngineResult.Fail");
        Assert.Contains(report.Items, item => item.Name == "Vector3d math");
        Assert.Contains(report.Items, item => item.Name == "YawRotation normalize");
        Assert.Contains(report.Items, item => item.Name == "SimulationTime advance");
        Assert.Contains(report.Items, item => item.Name == "EngineLogEntry format");
    }
}
