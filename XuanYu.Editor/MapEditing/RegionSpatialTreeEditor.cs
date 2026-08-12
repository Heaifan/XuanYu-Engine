namespace XuanYu.Editor.MapEditing;

internal static class RegionSpatialTreeEditor
{
    public static void Insert(ref RegionSpatialNode? root, RegionSpatialNode leaf)
    {
        if (root is null)
        {
            root = leaf;
            return;
        }
        var sibling = FindBestSibling(root, leaf.Bounds);
        var oldParent = sibling.Parent;
        var parent = new RegionSpatialNode(sibling.Bounds.Union(leaf.Bounds))
        {
            Parent = oldParent,
            Left = sibling,
            Right = leaf
        };
        sibling.Parent = parent;
        leaf.Parent = parent;
        ReplaceChild(ref root, oldParent, sibling, parent);
        RefitToRoot(ref root, parent);
    }

    public static void Remove(ref RegionSpatialNode? root, RegionSpatialNode leaf)
    {
        if (ReferenceEquals(root, leaf))
        {
            root = null;
            return;
        }
        var parent = leaf.Parent ?? throw new InvalidOperationException("空间索引叶节点缺少父节点。");
        var sibling = ReferenceEquals(parent.Left, leaf) ? parent.Right! : parent.Left!;
        var grand = parent.Parent;
        ReplaceChild(ref root, grand, parent, sibling);
        sibling.Parent = grand;
        leaf.Parent = null;
        parent.Left = null;
        parent.Right = null;
        RefitToRoot(ref root, grand);
    }

    static RegionSpatialNode FindBestSibling(RegionSpatialNode root, RegionSpatialBounds bounds)
    {
        var node = root;
        while (!node.IsLeaf)
        {
            var left = node.Left!;
            var right = node.Right!;
            var leftCost = Growth(left.Bounds, bounds);
            var rightCost = Growth(right.Bounds, bounds);
            node = leftCost < rightCost || (leftCost == rightCost && left.Height <= right.Height)
                ? left : right;
        }
        return node;
    }

    static double Growth(RegionSpatialBounds current, RegionSpatialBounds added) =>
        current.Union(added).Perimeter - current.Perimeter;

    static void RefitToRoot(ref RegionSpatialNode? root, RegionSpatialNode? node)
    {
        while (node is not null)
        {
            node.Refit();
            var top = RegionSpatialTreeBalancer.Balance(ref root, node);
            node = top.Parent;
        }
    }

    internal static void ReplaceChild(
        ref RegionSpatialNode? root,
        RegionSpatialNode? parent,
        RegionSpatialNode oldChild,
        RegionSpatialNode newChild)
    {
        if (parent is null) root = newChild;
        else if (ReferenceEquals(parent.Left, oldChild)) parent.Left = newChild;
        else if (ReferenceEquals(parent.Right, oldChild)) parent.Right = newChild;
        else throw new InvalidOperationException("空间索引父子关系不一致。");
    }
}
