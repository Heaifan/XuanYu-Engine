using Avalonia.Controls;
using FluidWarfare.Core.Identity;
using FluidWarfare.Engine.World;

namespace FluidWarfare.Editor.Windows.Panels.WorldEntities;

public sealed partial class WorldEntityListPanel : UserControl
{
    private StackPanel? _entityListPanel;
    private TextBlock? _emptyText;

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
                    Content = displayText
                };

                var captured = entity;
                button.Click += (_, _) => OnEntityClicked(captured);
                _entityListPanel.Children.Add(button);
            }
        }
    }

    private void OnEntityClicked(WorldEntityInfo entity)
    {
        EntitySelected?.Invoke(entity);
    }
}
