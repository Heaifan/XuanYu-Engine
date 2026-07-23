namespace XuanYu.Editor.UI;

public static class TreeGuideBuilder
{
    public static IReadOnlyList<EditorTreeNode> Visible(IReadOnlyList<EditorTreeNode> nodes, ISet<string> collapsed)
    {
        var visible = new List<EditorTreeNode>();
        for (var i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            if (HasCollapsedAncestor(nodes, i, collapsed)) continue;
            visible.Add(node);
        }
        Apply(visible, collapsed);
        return visible;
    }

    public static void Apply(IReadOnlyList<EditorTreeNode> nodes)
    {
        Apply(nodes, new HashSet<string>());
    }

    static void Apply(IReadOnlyList<EditorTreeNode> nodes, ISet<string> collapsed)
    {
        var last = new bool[nodes.Count];
        for (var i = 0; i < nodes.Count; i++)
        {
            last[i] = IsLastChild(nodes, i);
            var hasChildren = HasChildren(nodes, i);
            nodes[i].SetTreeState(hasChildren, hasChildren && !collapsed.Contains(nodes[i].Key));
        }

        var ancestorLast = new Dictionary<int, bool>();
        for (var i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            foreach (var key in ancestorLast.Keys.Where(level => level >= node.Level).ToArray())
                ancestorLast.Remove(key);
            node.SetGuide(Build(node.Level, last[i], ancestorLast));
            ancestorLast[node.Level] = last[i];
        }
    }

    static IReadOnlyList<TreeGuideSegment> Build(int level, bool isLast, Dictionary<int, bool> ancestors)
    {
        if (level <= 0) return [];
        var result = new List<TreeGuideSegment>(level);
        for (var depth = 0; depth < level - 1; depth++)
        {
            var blank = ancestors.TryGetValue(depth + 1, out var last) && last;
            result.Add(new(depth, blank ? TreeGuideSegmentKind.Blank : TreeGuideSegmentKind.Full));
        }
        result.Add(new(level - 1, isLast ? TreeGuideSegmentKind.Elbow : TreeGuideSegmentKind.Tee));
        return result;
    }

    static bool IsLastChild(IReadOnlyList<EditorTreeNode> nodes, int index)
    {
        var level = nodes[index].Level;
        for (var i = index + 1; i < nodes.Count; i++)
        {
            if (nodes[i].Level < level) return true;
            if (nodes[i].Level == level) return false;
        }
        return true;
    }

    static bool HasChildren(IReadOnlyList<EditorTreeNode> nodes, int index) =>
        index + 1 < nodes.Count && nodes[index + 1].Level > nodes[index].Level;

    static bool HasCollapsedAncestor(IReadOnlyList<EditorTreeNode> nodes, int index, ISet<string> collapsed)
    {
        var level = nodes[index].Level;
        for (var i = index - 1; i >= 0; i--)
        {
            if (nodes[i].Level >= level) continue;
            if (collapsed.Contains(nodes[i].Key)) return true;
            level = nodes[i].Level;
        }
        return false;
    }
}
