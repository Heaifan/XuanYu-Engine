using XuanYu.Engine.Core.Math;
using FluidWarfare.Editor.Windows.Shell;
using FluidWarfare.Editor.Windows.Panels.Viewport;

namespace FluidWarfare.Editor.Windows.Viewport.Selection.Presentation;

/// <summary>世界实体选择展示结果。Shell Apply 到 Inspector / StatusBar / Viewport。</summary>
public sealed record WorldEntitySelectionResult(
    EditorSelection InspectorSelection,
    string? InspectorEntityId,
    Vector3d? EntityPosition,
    string? EntitySourcePath,
    string? StatusBarSelection,
    bool GroundPlaceEnabled,
    string LogMessage,
    ViewportEntitySummary? ViewportSummary,
    string? VisualKindText);

/// <summary>项目文件选择展示结果。</summary>
public sealed record ProjectContentSelectionResult(
    EditorSelection InspectorSelection,
    string? StatusBarSelection,
    string LogMessage);
