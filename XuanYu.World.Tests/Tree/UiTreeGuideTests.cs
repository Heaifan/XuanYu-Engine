using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed class UiTreeGuideTests
{
    [Fact]
    public void Project_tree_uses_continuous_guides_for_middle_and_last_children()
    {
        var nodes = TreeGuideBuilder.Visible(UiText.ProjectTreeItems, new HashSet<string>());

        var worlds = nodes.Single(node => node.Key == "project:worlds");
        var main = nodes.Single(node => node.Key == "world:main");
        var test = nodes.Single(node => node.Key == "world:test");
        var resources = nodes.Single(node => node.Key == "project:assets");
        var build = nodes.Single(node => node.Key == "asset:build");

        Assert.Equal(TreeGuideSegmentKind.Tee, worlds.GuideSegments.Single().Kind);
        Assert.Equal(TreeGuideSegmentKind.Full, main.GuideSegments[0].Kind);
        Assert.Equal(TreeGuideSegmentKind.Tee, main.GuideSegments[1].Kind);
        Assert.Equal(TreeGuideSegmentKind.Full, test.GuideSegments[0].Kind);
        Assert.Equal(TreeGuideSegmentKind.Elbow, test.GuideSegments[1].Kind);
        Assert.Equal(TreeGuideSegmentKind.Elbow, resources.GuideSegments.Single().Kind);
        Assert.Equal("构建配置", build.Title);
    }

    [Fact]
    public void Collapsed_project_branch_recomputes_visible_guides()
    {
        var nodes = TreeGuideBuilder.Visible(UiText.ProjectTreeItems,
            new HashSet<string>(StringComparer.Ordinal) { "project:worlds" });

        Assert.DoesNotContain(nodes, node => node.Key == "world:main");
        Assert.DoesNotContain(nodes, node => node.Key == "world:test");
        Assert.Equal(TreeGuideSegmentKind.Tee,
            nodes.Single(node => node.Key == "project:worlds").GuideSegments.Single().Kind);
        Assert.Equal(TreeGuideSegmentKind.Elbow,
            nodes.Single(node => node.Key == "project:assets").GuideSegments.Single().Kind);
    }
}
