using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.World;

public sealed class UiHierarchyConnectorTests
{
    static EditorTreeNode N(string key, int level) => new(key, key, "t", "p", level, "entity");

    static IReadOnlyList<EditorTreeNode> BuildHierarchy()
    {
        var nodes = new List<EditorTreeNode>
        {
            N("root", 0),
            N("camera", 1),
            N("ground", 1),
            N("region0", 1),
            N("e01", 2), N("e02", 2), N("e03", 2),
            N("region1", 1),
            N("e05", 2), N("e10", 2)
        };
        return TreeGuideBuilder.Visible(nodes, new HashSet<string>());
    }

    [Fact]
    public void Root_has_no_connector()
    {
        var nodes = BuildHierarchy();
        Assert.Empty(nodes.Single(n => n.Key == "root").GuideSegments);
    }

    [Fact]
    public void First_level_siblings_use_tee_except_last_elbow()
    {
        var nodes = BuildHierarchy();
        Assert.Equal(TreeGuideSegmentKind.Tee, nodes.Single(n => n.Key == "camera").GuideSegments.Single().Kind);
        Assert.Equal(TreeGuideSegmentKind.Tee, nodes.Single(n => n.Key == "ground").GuideSegments.Single().Kind);
        Assert.Equal(TreeGuideSegmentKind.Tee, nodes.Single(n => n.Key == "region0").GuideSegments.Single().Kind);
        Assert.Equal(TreeGuideSegmentKind.Elbow, nodes.Single(n => n.Key == "region1").GuideSegments.Single().Kind);
    }

    [Fact]
    public void Region_children_keep_root_continuation_except_last_region()
    {
        var nodes = BuildHierarchy();
        var e01 = nodes.Single(n => n.Key == "e01");
        Assert.Equal(TreeGuideSegmentKind.Full, e01.GuideSegments[0].Kind);
        Assert.Equal(TreeGuideSegmentKind.Tee, e01.GuideSegments[1].Kind);

        var e03 = nodes.Single(n => n.Key == "e03");
        Assert.Equal(TreeGuideSegmentKind.Full, e03.GuideSegments[0].Kind);
        Assert.Equal(TreeGuideSegmentKind.Elbow, e03.GuideSegments[1].Kind);

        var e05 = nodes.Single(n => n.Key == "e05");
        Assert.Equal(TreeGuideSegmentKind.Blank, e05.GuideSegments[0].Kind);
        Assert.Equal(TreeGuideSegmentKind.Tee, e05.GuideSegments[1].Kind);

        var e10 = nodes.Single(n => n.Key == "e10");
        Assert.Equal(TreeGuideSegmentKind.Blank, e10.GuideSegments[0].Kind);
        Assert.Equal(TreeGuideSegmentKind.Elbow, e10.GuideSegments[1].Kind);
    }

    [Fact]
    public void Collapsing_region_recomputes_child_guides()
    {
        var visible = TreeGuideBuilder.Visible(
            new List<EditorTreeNode> { N("root", 0), N("region0", 1), N("e01", 2), N("region1", 1), N("e10", 2) },
            new HashSet<string>(StringComparer.Ordinal) { "region0" });
        Assert.DoesNotContain(visible, n => n.Key == "e01");
        Assert.Equal(TreeGuideSegmentKind.Elbow, visible.Single(n => n.Key == "region1").GuideSegments.Single().Kind);
    }
}
