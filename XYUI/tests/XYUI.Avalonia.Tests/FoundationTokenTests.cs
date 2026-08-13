using System;
using Avalonia.Media;
using Xunit;
using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Tests;

public sealed class FoundationTokenTests
{
    [Fact]
    public void CoreColorsAreCanonicalAndOpaque()
    {
        Assert.Equal(Color.Parse("#2563EB"), XYTokens.Accent);
        Assert.Equal(255, XYTokens.Surface.A);
        Assert.Equal(255, XYTokens.TextPrimary.A);
    }

    [Fact]
    public void DensityAndThemeVariantsExposeStableContract()
    {
        Assert.Equal(new[] { "Compact", "Default", "Comfortable" },
            Enum.GetNames<XYDensity>());
        Assert.Equal(new[] { "Light", "Dark" }, Enum.GetNames<XYThemeVariant>());
    }
}
