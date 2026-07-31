namespace XuanYu.World;

public sealed partial class EntityRegistry
{
    public void Replace(IReadOnlyList<WorldEntitySnapshot> entities)
    {
        _entities.Clear();
        _nextId = 1;
        foreach (var entity in entities.OrderBy(item => item.EntityKey.Value))
        {
            _entities.Add(entity.EntityKey, entity);
            _nextId = Math.Max(_nextId, entity.EntityKey.Value + 1);
        }
    }
}
