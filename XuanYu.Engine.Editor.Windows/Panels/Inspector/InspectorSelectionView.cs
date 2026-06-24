using Avalonia.Controls;
using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.Windows.Shell;

namespace FluidWarfare.Editor.Windows.Panels.Inspector;

/// <summary>Inspector 选择区显示管理。空选择/项目文件/世界实体的展示切换。</summary>
public sealed class InspectorSelectionView
{
    readonly TextBlock? _empty, _kind, _name, _entityId, _source;
    readonly StackPanel? _details;

    public InspectorSelectionView(TextBlock? empty, StackPanel? details,
        TextBlock? kind, TextBlock? name, TextBlock? entityId, TextBlock? source)
    { _empty = empty; _details = details; _kind = kind; _name = name; _entityId = entityId; _source = source; }

    public void ShowEmpty()
    { SetEmptyVis(true); SetDetailsVis(false); }

    public void ShowProjectFile(EditorSelection sel)
    { ShowCore(sel); SetDetailsVis(true); }

    public void ShowWorldEntity(EditorSelection sel, string? entityId, Vector3d? position, string? sourcePath)
    {
        ShowCore(sel); SetDetailsVis(true);
        if (_entityId is not null) _entityId.Text = $"EntityId：{entityId ?? "无"}";
        if (_source is not null) { _source.IsVisible = sourcePath is not null; _source.Text = sourcePath is not null ? $"来源：{sourcePath}" : ""; }
    }

    void ShowCore(EditorSelection sel)
    { SetEmptyVis(false); SetDetailsVis(true); if (_kind is not null) _kind.Text = $"类型：{sel.Kind}"; if (_name is not null) _name.Text = $"名称：{sel.DisplayName}"; }

    void SetEmptyVis(bool v) { if (_empty is not null) { _empty.IsVisible = v; if (v) _empty.Text = "未选择对象"; } }
    void SetDetailsVis(bool v) { if (_details is not null) _details.IsVisible = v; }
}
