using XuanYu.Engine.Core.Identity;
using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.WorldHierarchy;
using XuanYu.Engine.World;

namespace FluidWarfare.Tests.Editor.WorldHierarchy;

public sealed class WorldHierarchyTreeBuilderTests
{
    [Fact]
    public void Build_WithNoEntities_ReturnsEmptyTree()
    {
        var world = new WorldState();
        var tree = WorldHierarchyTreeBuilder.Build(world);

        Assert.NotNull(tree);
        Assert.Equal(1, tree.NodeCount);
        Assert.Equal(0, tree.EntityNodeCount);
        // Empty 常量硬编码为 "World"，非 TreeBuilder 生成的 "世界"
        Assert.Equal("World", tree.Root.DisplayName);
        Assert.Empty(tree.Root.Children);
        Assert.Empty(tree.EntityNodes);
    }

    [Fact]
    public void Build_WithSingleEntity_CreatesGroupAndEntityNodes()
    {
        var world = new WorldState();
        var id1 = world.CreateEntity("测试单位", new Vector3d(0, 0, 0));

        var groupLookup = new Dictionary<EntityId, string>
        {
            [id1] = "单位"
        };

        var tree = WorldHierarchyTreeBuilder.Build(world, groupLookup);

        Assert.Equal(3, tree.NodeCount); // root + group(单位) + entity
        Assert.Equal(1, tree.EntityNodeCount);

        // Root
        Assert.Single(tree.Root.Children);
        var groupNode = tree.Root.Children[0];
        Assert.Equal("group:units", groupNode.NodeId);
        Assert.Equal("单位 (1)", groupNode.DisplayName);
        Assert.Equal(WorldHierarchyNodeKind.EntityGroup, groupNode.Kind);
        Assert.Equal(2, groupNode.DescendantCount); // group + entity

        // Entity
        Assert.Single(groupNode.Children);
        var entityNode = groupNode.Children[0];
        Assert.Equal($"entity:{id1.Value}", entityNode.NodeId);
        Assert.Equal("测试单位", entityNode.DisplayName);
        Assert.Equal(WorldHierarchyNodeKind.Entity, entityNode.Kind);
        Assert.True(entityNode.IsSelectable);
        Assert.Equal(id1, entityNode.EntityId);
    }

    [Fact]
    public void Build_MultipleEntities_OrdersByGroupThenDisplayName()
    {
        var world = new WorldState();
        var id1 = world.CreateEntity("甲单位", new Vector3d(0, 0, 0));
        var id2 = world.CreateEntity("乙单位", new Vector3d(1, 0, 0));
        var id3 = world.CreateEntity("A工事", new Vector3d(2, 0, 0));

        var groupLookup = new Dictionary<EntityId, string>
        {
            [id1] = "单位",
            [id2] = "单位",
            [id3] = "工事"
        };

        var tree = WorldHierarchyTreeBuilder.Build(world, groupLookup);

        // Group order: 工事(2nd in GroupOrder[2]) > 单位(2nd in GroupOrder[1])
        // Actually GroupOrder is ["地形", "单位", "工事", "触发器", "其他"]
        // IndexOf("工事") = 2, IndexOf("单位") = 1
        // So 单位 comes first

        Assert.Equal(2, tree.Root.Children.Count);
        Assert.Equal("group:units", tree.Root.Children[0].NodeId);
        Assert.Equal("group:fortifications", tree.Root.Children[1].NodeId);

        // Entities sorted by DisplayName (Ordinal: "乙" U+4E59 < "甲" U+7532)
        Assert.Equal(2, tree.Root.Children[0].Children.Count);
        Assert.Equal("乙单位", tree.Root.Children[0].Children[0].DisplayName);
        Assert.Equal("甲单位", tree.Root.Children[0].Children[1].DisplayName);
    }

    [Fact]
    public void Build_UnknownGroup_FallsBackToOther()
    {
        var world = new WorldState();
        var id1 = world.CreateEntity("未知物", new Vector3d(0, 0, 0));

        // 不提供 groupLookup 时，实体应归入"其他"分组
        var tree = WorldHierarchyTreeBuilder.Build(world);

        Assert.Single(tree.Root.Children);
        Assert.Equal("group:other", tree.Root.Children[0].NodeId);
        Assert.Equal("其他 (1)", tree.Root.Children[0].DisplayName);
    }

    [Fact]
    public void Build_NoGroupLookup_AllEntitiesInOther()
    {
        var world = new WorldState();
        world.CreateEntity("无分组单位", new Vector3d(0, 0, 0));
        world.CreateEntity("另一个", new Vector3d(1, 0, 0));

        var tree = WorldHierarchyTreeBuilder.Build(world);

        Assert.Single(tree.Root.Children);
        Assert.Equal("group:other", tree.Root.Children[0].NodeId);
        Assert.Equal(2, tree.Root.Children[0].Children.Count);
    }

    [Fact]
    public void Build_AncestorMap_ReferencesCorrectly()
    {
        var world = new WorldState();
        var id1 = world.CreateEntity("单位A", new Vector3d(0, 0, 0));

        var groupLookup = new Dictionary<EntityId, string> { [id1] = "单位" };
        var tree = WorldHierarchyTreeBuilder.Build(world, groupLookup);

        var entityIdStr = id1.Value.ToString();
        Assert.True(tree.EntityAncestorNodeIds.ContainsKey(entityIdStr));

        var ancestors = tree.EntityAncestorNodeIds[entityIdStr];
        Assert.Equal(2, ancestors.Count);
        Assert.Equal("world:root", ancestors[0]);
        Assert.Equal("group:units", ancestors[1]);
    }

    [Fact]
    public void Build_FindEntity_ReturnsCorrectNode()
    {
        var world = new WorldState();
        var id1 = world.CreateEntity("可查找", new Vector3d(0, 0, 0));

        var groupLookup = new Dictionary<EntityId, string> { [id1] = "单位" };
        var tree = WorldHierarchyTreeBuilder.Build(world, groupLookup);

        var found = tree.FindEntity(id1.Value.ToString());
        Assert.NotNull(found);
        Assert.Equal("可查找", found.DisplayName);
    }

    [Fact]
    public void Build_EntityDescendantCount_AccumulatesCorrectly()
    {
        var world = new WorldState();
        world.CreateEntity("e1", new Vector3d(0, 0, 0));
        world.CreateEntity("e2", new Vector3d(1, 0, 0));
        world.CreateEntity("e3", new Vector3d(2, 0, 0));

        var groupLookup = new Dictionary<EntityId, string>();
        var allEntities = world.ListEntities();
        foreach (var e in allEntities)
            groupLookup[e.EntityId] = "单位";

        var tree = WorldHierarchyTreeBuilder.Build(world, groupLookup);

        // Root: 1 + group(1) + 3 entities = 5
        Assert.Equal(5, tree.NodeCount);

        // Group: 1 + 3 entities = 4
        Assert.Equal(4, tree.Root.Children[0].DescendantCount);

        // Each entity: 1
        foreach (var child in tree.Root.Children[0].Children)
            Assert.Equal(1, child.DescendantCount);
    }
}
