using XuanYu.Editor.UI;

namespace XuanYu.Core.Tests.Render.NavigationGizmo;

public sealed class NavigationGizmoDirectionContractTests
{
    [Theory]
    [InlineData("+X", "+X 视图")]
    [InlineData("-X", "-X 视图")]
    [InlineData("+Y", "+Y 视图")]
    [InlineData("-Y", "-Y 视图")]
    [InlineData("+Z", "顶视图")]
    [InlineData("-Z", "底视图")]
    public void Every_endpoint_maps_to_one_standard_view(string endpoint, string view)
    {
        Assert.Equal(view, StandardViewResolver.EndpointToViewName(endpoint));
        Assert.True(StandardViewResolver.TryResolve(view, out _, out _));
    }

    [Theory]
    [InlineData("+X 视图", 0)]
    [InlineData("-X 视图", 1)]
    [InlineData("+Y 视图", 2)]
    [InlineData("-Y 视图", 3)]
    [InlineData("顶视图", 4)]
    [InlineData("底视图", 5)]
    public void Active_view_index_is_stable(string view, int index) =>
        Assert.Equal(index, NavigationGizmoLayout.ActiveIndexFor(view));
}
