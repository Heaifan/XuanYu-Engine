namespace XuanYu.Editor.MapEditing;

internal static class RegionSpatialTreeBalancer
{
    public static RegionSpatialNode Balance(ref RegionSpatialNode? root, RegionSpatialNode node)
    {
        if (node.IsLeaf || node.Height < 2) return node;
        var left = node.Left!;
        var right = node.Right!;
        if (right.Height - left.Height > 1) return RotateRightUp(ref root, node, right);
        if (left.Height - right.Height > 1) return RotateLeftUp(ref root, node, left);
        return node;
    }

    static RegionSpatialNode RotateRightUp(
        ref RegionSpatialNode? root,
        RegionSpatialNode node,
        RegionSpatialNode right)
    {
        var oldParent = node.Parent;
        var near = right.Left!;
        var far = right.Right!;
        RegionSpatialTreeEditor.ReplaceChild(ref root, oldParent, node, right);
        right.Parent = oldParent;
        right.Left = node;
        node.Parent = right;
        if (near.Height > far.Height)
        {
            right.Right = near;
            node.Right = far;
            far.Parent = node;
        }
        else
        {
            right.Right = far;
            node.Right = near;
            near.Parent = node;
        }
        node.Refit();
        right.Refit();
        return right;
    }

    static RegionSpatialNode RotateLeftUp(
        ref RegionSpatialNode? root,
        RegionSpatialNode node,
        RegionSpatialNode left)
    {
        var oldParent = node.Parent;
        var far = left.Left!;
        var near = left.Right!;
        RegionSpatialTreeEditor.ReplaceChild(ref root, oldParent, node, left);
        left.Parent = oldParent;
        left.Right = node;
        node.Parent = left;
        if (far.Height > near.Height)
        {
            left.Left = far;
            node.Left = near;
            near.Parent = node;
        }
        else
        {
            left.Left = near;
            node.Left = far;
            far.Parent = node;
        }
        node.Refit();
        left.Refit();
        return left;
    }
}
