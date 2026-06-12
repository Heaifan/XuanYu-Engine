using Avalonia.Controls;
using Avalonia.Media;
using FluidWarfare.Core.Identity;
using FluidWarfare.Engine.World;

namespace FluidWarfare.Editor.Windows.Panels.WorldEntities;

public sealed partial class WorldEntityListPanel : UserControl
{
    private StackPanel? _entityListPanel;
    private TextBlock? _emptyText;
    private readonly Dictionary<EntityId, Button> _entityButtons = [];

    public event Action<WorldEntityInfo>? EntitySelected;

    public WorldEntityListPanel()
    {
        InitializeComponent();
        _emptyText = this.FindControl<TextBlock>("WorldEntityEmptyText");
        _entityListPanel = this.FindControl<StackPanel>("WorldEntityList");
    }

    /// <summary>
    /// 显示 World 实体列表。
    /// </summary>
    public void ShowEntities(IReadOnlyList<WorldEntityInfo> entities)
    {
        _entityListPanel ??= this.FindControl<StackPanel>("WorldEntityList");
        _emptyText ??= this.FindControl<TextBlock>("WorldEntityEmptyText");
        _entityButtons.Clear();

        if (entities is null || entities.Count == 0)
        {
            if (_emptyText is not null) _emptyText.IsVisible = true;
            if (_entityListPanel is not null) _entityListPanel.IsVisible = false;
            return;
        }

        if (_emptyText is not null) _emptyText.IsVisible = false;
        if (_entityListPanel is not null)
        {
            _entityListPanel.IsVisible = true;
            _entityListPanel.Children.Clear();

            foreach (var entity in entities)
            {
                var displayText = entity.Source is not null
                    ? $"{entity.DisplayName}\n{entity.Source.RelativePath}"
                    : entity.DisplayName;

                var button = new Button
                {
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = Avalonia.Layout.HorizontalAlignment.Left,
                    Content = displayText,
                    Tag = entity.EntityId
                };

                var captured = entity;
                button.Click += (_, _) => OnEntityClicked(captured);
                _entityListPanel.Children.Add(button);
                _entityButtons[entity.EntityId] = button;
            }
        }
    }

    /// <summary>
    /// 选中指定实体，高亮显示。
    /// </summary>
    public void SelectEntity(EntityId entityId)
    {
        // 清除所有高亮
        foreach (var btn in _entityButtons.Values)
            btn.Background = null;

        // 高亮目标
        if (_entityButtons.TryGetValue(entityId, out var target))
            target.Background = new SolidColorBrush(Color.FromRgb(60, 120, 200));
    }

    /// <summary>
    /// 清除所有高亮。
    /// </summary>
    public void ClearSelection()
    {
        foreach (var btn in _entityButtons.Values)
            btn.Background = null;
    }

    private void OnEntityClicked(WorldEntityInfo entity)
    {
        EntitySelected?.Invoke(entity);
    }
}
