using XYUI.Avalonia.Foundation;

namespace XYUI.Avalonia.Tests;

// G1 骨架引用链验证：Tests → Library
public class SkeletonTests
{
    [Fact]
    public void TokenTable_Has_83_Unique_Tokens()
    {
        var ids = XyuiColorTokens.All.Select(t => t.TokenId).Distinct().Count();
        Assert.Equal(83, ids);
    }

    [Fact]
    public void BrushKey_Is_Prefixed()
    {
        Assert.Equal("XY.Brush.Text.Primary", XyuiColorTokens.BrushKey("XY.Text.Primary"));
        Assert.Equal("XY.Brush.Surface.App", XyuiColorTokens.BrushKey("XY.Surface.App"));
    }
}
