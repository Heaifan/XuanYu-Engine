using XuanYu.Core.Spatial;
namespace XuanYu.World.Spatial;

public sealed partial class DynamicAabbTree
{
    void RemoveLeaf(int leaf)
    {
        if (leaf == _root)
        {
            _root = -1;
            _nodes[leaf].Parent = -1;
            return;
        }

        var parent = _nodes[leaf].Parent;
        var grand = _nodes[parent].Parent;
        var sibling = _nodes[parent].Left == leaf ? _nodes[parent].Right : _nodes[parent].Left;
        _nodes[sibling].Parent = grand;
        if (grand < 0) _root = sibling;
        else ReplaceChild(grand, parent, sibling);
        _nodes[leaf].Parent = -1;
        Refit(grand);
    }

    void ReplaceChild(int parent, int oldChild, int newChild)
    {
        if (_nodes[parent].Left == oldChild) _nodes[parent].Left = newChild;
        else _nodes[parent].Right = newChild;
    }
}
