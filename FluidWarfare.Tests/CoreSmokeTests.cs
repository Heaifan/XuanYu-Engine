namespace FluidWarfare.Tests;

public sealed class CoreSmokeTests
{
    [Fact]
    public void CoreAssemblyIsAvailable()
    {
        var assemblyName = System.Reflection.Assembly
            .Load("FluidWarfare.Core")
            .GetName()
            .Name;

        Assert.Equal("FluidWarfare.Core", assemblyName);
    }
}
