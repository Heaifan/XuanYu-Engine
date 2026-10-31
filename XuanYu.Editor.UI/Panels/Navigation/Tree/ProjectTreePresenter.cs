using System.ComponentModel;
using System.Runtime.CompilerServices;
using XuanYu.World;

namespace XuanYu.Editor.UI;

public sealed class ProjectTreePresenter : INotifyPropertyChanged
{
    readonly Dictionary<string, EditorTreeNode> _nodes = new();
    readonly HashSet<string> _collapsed = new(StringComparer.Ordinal);
    readonly HierarchyTreeProjection _hierarchy = new();
    readonly EditorTreeNode _scene = new("scene:current", "未命名场景", "场景", "未命名场景", 0, "project");
    IReadOnlyList<EditorTreeNode> _allProjectItems = [];
    IReadOnlyList<EditorTreeNode> _projectItems = [];
    EditorTreeNode? _selectedItem, _selectedHierarchyItem;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action<EditorTreeNode?>? ProjectSelectionRequested;
    public event Action<EditorTreeNode?>? HierarchySelectionRequested;
    public IReadOnlyList<EditorTreeNode> ProjectItems => _projectItems;
    public IReadOnlyList<EditorTreeNode> HierarchyItems => _hierarchy.Items;
    public IReadOnlySet<string> CollapsedHierarchyKeys => _hierarchy.CollapsedKeys;
    public EditorTreeNode? SelectedItem { get => _selectedItem; set => ProjectSelectionRequested?.Invoke(value); }
    public EditorTreeNode? SelectedHierarchyItem { get => _selectedHierarchyItem; set => HierarchySelectionRequested?.Invoke(value); }
    public IReadOnlySet<string> CollapsedKeys => _collapsed;

    public IReadOnlyList<EditorTreeNode> Build(
        string title, string path, IReadOnlyList<WorldEntitySnapshot> entities)
    {
        var live = new HashSet<string>(StringComparer.Ordinal) { _scene.Key };
        _scene.Update(title, "场景", path, 0, "project");
        _scene.SetTreeState(entities.Count > 0, !_collapsed.Contains(_scene.Key));
        var nodes = new List<EditorTreeNode> { _scene };
        foreach (var entity in entities.OrderBy(item => item.EntityKey.Value))
            AddEntityNode(entity, live, nodes);
        foreach (var key in _nodes.Keys.Where(key => !live.Contains(key)).ToArray()) _nodes.Remove(key);
        _allProjectItems = nodes;
        _projectItems = TreeGuideBuilder.Visible(_allProjectItems, _collapsed);
        OnPropertyChanged(nameof(ProjectItems));
        return _projectItems;
    }

    public IReadOnlyList<EditorTreeNode> BuildHierarchy(IReadOnlyList<WorldEntitySnapshot> entities)
    {
        var items = _hierarchy.Build(entities);
        OnPropertyChanged(nameof(HierarchyItems));
        return items;
    }

    public bool ToggleHierarchy(EditorTreeNode node)
    {
        if (!_hierarchy.Toggle(node)) return false;
        OnPropertyChanged(nameof(HierarchyItems));
        return true;
    }

    public bool Toggle(EditorTreeNode node)
    {
        if (!node.CanToggle) return false;
        if (!_collapsed.Add(node.Key)) _collapsed.Remove(node.Key);
        _projectItems = TreeGuideBuilder.Visible(_allProjectItems, _collapsed);
        OnPropertyChanged(nameof(ProjectItems));
        return true;
    }

    public bool SetSelectedItem(EditorTreeNode? node)
    {
        if (SelectedItem == node) return false;
        _selectedItem = node;
        OnPropertyChanged(nameof(SelectedItem));
        return true;
    }

    public bool SetSelectedHierarchyItem(EditorTreeNode? node)
    {
        if (SelectedHierarchyItem == node) return false;
        _selectedHierarchyItem = node;
        OnPropertyChanged(nameof(SelectedHierarchyItem));
        return true;
    }

    public bool IsSceneNode(string key) => key == _scene.Key;

    void AddEntityNode(WorldEntitySnapshot entity, HashSet<string> live, List<EditorTreeNode> nodes)
    {
        var key = entity.EntityKey.ToString();
        live.Add(key);
        var path = $"主世界/{EditorDisplayText.Region(entity.RegionKey)}/{EditorDisplayText.Entity(entity.EntityKey)}";
        if (!_nodes.TryGetValue(key, out var node))
            _nodes.Add(key, node = new(key, entity.Name, EditorDisplayText.EntityType(entity.Type), path, 1, "entity"));
        else node.Update(entity.Name, EditorDisplayText.EntityType(entity.Type), path, 1, "entity");
        nodes.Add(node);
    }

    void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
