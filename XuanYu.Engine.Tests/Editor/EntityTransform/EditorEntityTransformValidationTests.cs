using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.EntityTransform;

namespace FluidWarfare.Tests.Editor.EntityTransform;

public sealed class EditorEntityTransformValidationTests
{
    [Fact]
    public void ValidCoordinates_ReturnParsedPosition()
    {
        var ok = EditorEntityTransformValidation.TryParse(
            "1.5", "2.5", "3.5", out var pos, out var err);
        Assert.True(ok);
        Assert.Equal(new Vector3d(1.5, 2.5, 3.5), pos);
        Assert.Empty(err);
    }

    [Fact]
    public void NegativeCoordinates_AreAccepted()
    {
        var ok = EditorEntityTransformValidation.TryParse(
            "-4", "0", "1", out var pos, out _);
        Assert.True(ok);
        Assert.Equal(new Vector3d(-4, 0, 1), pos);
    }

    [Fact]
    public void DecimalCoordinates_AreAccepted()
    {
        var ok = EditorEntityTransformValidation.TryParse(
            "3.14159", "-0.001", "100.5", out var pos, out _);
        Assert.True(ok);
        Assert.Equal(new Vector3d(3.14159, -0.001, 100.5), pos);
    }

    [Fact]
    public void EmptyX_ReturnsChineseError()
    {
        var ok = EditorEntityTransformValidation.TryParse(
            "", "0", "0", out _, out var err);
        Assert.False(ok);
        Assert.Contains("不能为空", err);
    }

    [Fact]
    public void EmptyY_ReturnsChineseError()
    {
        var ok = EditorEntityTransformValidation.TryParse(
            "0", "", "0", out _, out var err);
        Assert.False(ok);
        Assert.Contains("不能为空", err);
    }

    [Fact]
    public void EmptyZ_ReturnsChineseError()
    {
        var ok = EditorEntityTransformValidation.TryParse(
            "0", "0", "", out _, out var err);
        Assert.False(ok);
        Assert.Contains("不能为空", err);
    }

    [Fact]
    public void NaN_ReturnsChineseError()
    {
        var ok = EditorEntityTransformValidation.TryParse(
            "NaN", "0", "0", out _, out var err);
        Assert.False(ok);
        Assert.Contains("NaN", err);
    }

    [Fact]
    public void Infinity_ReturnsChineseError()
    {
        var ok = EditorEntityTransformValidation.TryParse(
            "Infinity", "0", "0", out _, out var err);
        Assert.False(ok);
        Assert.Contains("无穷", err);
    }

    [Fact]
    public void InvalidText_ReturnsChineseError()
    {
        var ok = EditorEntityTransformValidation.TryParse(
            "abc", "0", "0", out _, out var err);
        Assert.False(ok);
        Assert.Contains("有效数字", err);
    }
}
