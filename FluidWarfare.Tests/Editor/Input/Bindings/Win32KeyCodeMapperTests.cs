using FluidWarfare.Editor.Input.Bindings;

namespace FluidWarfare.Tests.Editor.Input.Bindings;

/// <summary>
/// Win32KeyCodeMapper 映射表测试（纯字典测试，无平台依赖）。
/// </summary>
public sealed class Win32KeyCodeMapperTests
{
    [Fact]
    public void Map_Home_ReturnsHome()
    {
        var result = Win32KeyCodeMapper.Map(0x24);
        Assert.Equal("Home", result);
    }

    [Fact]
    public void Map_Escape_ReturnsEscape()
    {
        var result = Win32KeyCodeMapper.Map(0x1B);
        Assert.Equal("Escape", result);
    }

    [Fact]
    public void Map_Numpad1_ReturnsNumpad1()
    {
        var result = Win32KeyCodeMapper.Map(0x61);
        Assert.Equal("Numpad1", result);
    }

    [Fact]
    public void Map_LetterA_ReturnsA()
    {
        var result = Win32KeyCodeMapper.Map(0x41);
        Assert.Equal("A", result);
    }

    [Fact]
    public void Map_Digit0_Returns0()
    {
        var result = Win32KeyCodeMapper.Map(0x30);
        Assert.Equal("0", result);
    }

    [Fact]
    public void Map_Unknown_ReturnsNull()
    {
        var result = Win32KeyCodeMapper.Map(0xFF);
        Assert.Null(result);
    }
}
