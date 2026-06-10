using Avalonia.Controls;
using FluidWarfare.Editor.Windows.Shell;

namespace FluidWarfare.Editor.Windows.Panels.Inspector;

public sealed partial class InspectorPanel : UserControl
{
    private TextBlock? _emptySelectionText;
    private TextBlock? _selectionDescriptionText;
    private StackPanel? _selectionDetails;
    private TextBlock? _selectionKindText;
    private TextBlock? _selectionNameText;

    public InspectorPanel()
    {
        InitializeComponent();
        _emptySelectionText = this.FindControl<TextBlock>("EmptySelectionText");
        _selectionDetails = this.FindControl<StackPanel>("SelectionDetails");
        _selectionKindText = this.FindControl<TextBlock>("SelectionKindText");
        _selectionNameText = this.FindControl<TextBlock>("SelectionNameText");
        _selectionDescriptionText = this.FindControl<TextBlock>("SelectionDescriptionText");
    }

    public void ShowSelection(EditorSelection selection)
    {
        if (_emptySelectionText is not null)
        {
            _emptySelectionText.IsVisible = false;
        }

        if (_selectionDetails is not null)
        {
            _selectionDetails.IsVisible = true;
        }

        if (_selectionKindText is not null)
        {
            _selectionKindText.Text = $"类型：{selection.Kind}";
        }

        if (_selectionNameText is not null)
        {
            _selectionNameText.Text = $"名称：{selection.DisplayName}";
        }

        if (_selectionDescriptionText is not null)
        {
            _selectionDescriptionText.Text = $"说明：{selection.Description}";
        }
    }
}
