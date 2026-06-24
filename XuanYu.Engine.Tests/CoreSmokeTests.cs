namespace XuanYu.Engine.Tests;

public sealed class CoreSmokeTests
{
    [Fact]
    public void CoreAssemblyIsAvailable()
    {
        var assemblyName = System.Reflection.Assembly
            .Load("XuanYu.Engine.Core")
            .GetName()
            .Name;

        Assert.Equal("XuanYu.Engine.Core", assemblyName);
    }
}
