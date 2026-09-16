using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace XYUI.Avalonia.Vector;

public enum XyuiVectorIcon { Info, Error, Warning, Search, Locate, Browse, Copy, Code, Tag, StatusDot, Check, Section, Empty, ChevronDown, Clear, Filter, Eye, Calendar, Clock, ChevronLeft, ChevronRight, ScrubLeftRight, MoreHorizontal, InspectorRecent, InspectorBasic, InspectorGeometry, InspectorStatus, InspectorRelation, InspectorMore, Add, DragGrip, File, NewFile, Open, Save, Undo, Redo, Play, Stop, Select, BoxSelect, Move, Rotate, Scale, Focus, ViewAll, Pan, Orbit, Snap }

public static class XyuiVectorIcons
{
    public const double LogicalIconSize = 24d;
    public static bool IsPlatformReady => global::Avalonia.Application.Current is not null;
    public static IReadOnlyDictionary<XyuiVectorIcon, string> PathData { get; } =
        new Dictionary<XyuiVectorIcon, string>
        {
            [XyuiVectorIcon.Info] = "M12 3 C7.029 3 3 7.029 3 12 C3 16.971 7.029 21 12 21 C16.971 21 21 16.971 21 12 C21 7.029 16.971 3 12 3 Z M12 10 V17 M12 7 V8",
            [XyuiVectorIcon.Error] = "M12 3 C7.029 3 3 7.029 3 12 C3 16.971 7.029 21 12 21 C16.971 21 21 16.971 21 12 C21 7.029 16.971 3 12 3 Z M9 9 L15 15 M15 9 L9 15",
            [XyuiVectorIcon.Warning] = "M12 3 L22 20 H2 Z M12 9 V14 M12 17 V17.5",
            [XyuiVectorIcon.Search] = "M10.5 4.5 C7.186 4.5 4.5 7.186 4.5 10.5 C4.5 13.814 7.186 16.5 10.5 16.5 C13.814 16.5 16.5 13.814 16.5 10.5 C16.5 7.186 13.814 4.5 10.5 4.5 Z M15 15 L21 21",
            [XyuiVectorIcon.Locate] = "M12 3 V7 M12 17 V21 M3 12 H7 M17 12 H21 M12 7 C9.239 7 7 9.239 7 12 C7 14.761 9.239 17 12 17 C14.761 17 17 14.761 17 12 C17 9.239 14.761 7 12 7 Z",
            [XyuiVectorIcon.Browse] = "M3 6 H9 L11 8 H21 V20 H3 Z M3 6 V4 H10 L12 6",
            [XyuiVectorIcon.Copy] = "M8 8 H20 V20 H8 Z M4 4 H16 V16 H4 Z",
            [XyuiVectorIcon.Code] = "M9 6 L3 12 L9 18 M15 6 L21 12 L15 18 M13 4 L11 20",
            [XyuiVectorIcon.Tag] = "M0 11 L11 0 H24 V22 H11 Z",
            [XyuiVectorIcon.StatusDot] = "M12 3 C16.971 3 21 7.029 21 12 C21 16.971 16.971 21 12 21 C7.029 21 3 16.971 3 12 C3 7.029 7.029 3 12 3 Z",
            [XyuiVectorIcon.Check] = "M4 12 L9 17 L20 6",
            [XyuiVectorIcon.Section] = "M3 2 H7 V22 H3 Z",
            [XyuiVectorIcon.Empty] = "M3 12 H21",
            [XyuiVectorIcon.ChevronDown] = "M6 9 L12 15 L18 9",
            [XyuiVectorIcon.Clear] = "M5 5 L19 19 M19 5 L5 19",
            [XyuiVectorIcon.Filter] = "M3 5 H21 L14 13 V19 L10 21 V13 Z",
            [XyuiVectorIcon.Eye] = "M2.5 12 C5.2 7.5 8.6 5.5 12 5.5 C15.4 5.5 18.8 7.5 21.5 12 C18.8 16.5 15.4 18.5 12 18.5 C8.6 18.5 5.2 16.5 2.5 12 Z M9.2 12 C9.2 10.45 10.45 9.2 12 9.2 C13.55 9.2 14.8 10.45 14.8 12 C14.8 13.55 13.55 14.8 12 14.8 C10.45 14.8 9.2 13.55 9.2 12 Z",
            [XyuiVectorIcon.Calendar] = "M5 4 H19 V21 H5 Z M8 2 V6 M16 2 V6 M5 9 H19 M8 12 H10 M12 12 H14 M16 12 H18 M8 16 H10 M12 16 H14 M16 16 H18",
            [XyuiVectorIcon.Clock] = "M12 3 C7.029 3 3 7.029 3 12 C3 16.971 7.029 21 12 21 C16.971 21 21 16.971 21 12 C21 7.029 16.971 3 12 3 Z M12 7 V12 L16 15",
            [XyuiVectorIcon.ChevronLeft] = "M15 6 L9 12 L15 18",
            [XyuiVectorIcon.ChevronRight] = "M9 6 L15 12 L9 18",
            [XyuiVectorIcon.ScrubLeftRight] = "M8 7 L3 12 L8 17 M16 7 L21 12 L16 17 M4 12 H20",
            [XyuiVectorIcon.MoreHorizontal] = "M5 10.5 A1.5 1.5 0 1 0 5 13.5 A1.5 1.5 0 1 0 5 10.5 M12 10.5 A1.5 1.5 0 1 0 12 13.5 A1.5 1.5 0 1 0 12 10.5 M19 10.5 A1.5 1.5 0 1 0 19 13.5 A1.5 1.5 0 1 0 19 10.5",
            [XyuiVectorIcon.InspectorRecent] = "M5.2 6.1 A6 6 0 1 1 4.1 11 M5.2 3.9 V6.4 H2.7 M10 6.7 V10 L12.4 11.5",
            [XyuiVectorIcon.InspectorBasic] = "M3.25 4 H16.75 A1.75 1.75 0 0 1 18.5 5.75 V14.25 A1.75 1.75 0 0 1 16.75 16 H3.25 A1.75 1.75 0 0 1 1.5 14.25 V5.75 A1.75 1.75 0 0 1 3.25 4 Z M7.85 8 A1.25 1.25 0 1 1 5.35 8 A1.25 1.25 0 1 1 7.85 8 M9.5 7.1 H14 M5.2 12 H14",
            [XyuiVectorIcon.InspectorGeometry] = "M5 14.5 L7.6 5.5 L15 8 L13.2 14.5 H5 Z M7.6 4.2 A1.3 1.3 0 1 1 7.6 6.8 A1.3 1.3 0 1 1 7.6 4.2 M15 6.7 A1.3 1.3 0 1 1 15 9.3 A1.3 1.3 0 1 1 15 6.7 M13.2 13.2 A1.3 1.3 0 1 1 13.2 15.8 A1.3 1.3 0 1 1 13.2 13.2 M5 13.2 A1.3 1.3 0 1 1 5 15.8 A1.3 1.3 0 1 1 5 13.2",
            [XyuiVectorIcon.InspectorStatus] = "M8 6 A2 2 0 1 1 4 6 A2 2 0 1 1 8 6 M10 6 H15 M16 13.5 A2 2 0 1 1 12 13.5 A2 2 0 1 1 16 13.5 M5 13.5 H10",
            [XyuiVectorIcon.InspectorRelation] = "M7 9.2 L13 5.8 M7 10.8 L13 14.2 M7 10 A2 2 0 1 1 3 10 A2 2 0 1 1 7 10 M17 5 A2 2 0 1 1 13 5 A2 2 0 1 1 17 5 M17 15 A2 2 0 1 1 13 15 A2 2 0 1 1 17 15",
            [XyuiVectorIcon.InspectorMore] = "M6.25 10 A1.25 1.25 0 1 1 3.75 10 A1.25 1.25 0 1 1 6.25 10 M11.25 10 A1.25 1.25 0 1 1 8.75 10 A1.25 1.25 0 1 1 11.25 10 M16.25 10 A1.25 1.25 0 1 1 13.75 10 A1.25 1.25 0 1 1 16.25 10",
            [XyuiVectorIcon.Add] = "M12 5 V19 M5 12 H19",
            [XyuiVectorIcon.DragGrip] = "M8 7 A1 1 0 1 0 8 9 A1 1 0 1 0 8 7 M16 7 A1 1 0 1 0 16 9 A1 1 0 1 0 16 7 M8 15 A1 1 0 1 0 8 17 A1 1 0 1 0 8 15 M16 15 A1 1 0 1 0 16 17 A1 1 0 1 0 16 15",
            [XyuiVectorIcon.File] = "M3 3H13L17 7V21H3ZM13 3V7H17M6 11H14M6 15H14",
            [XyuiVectorIcon.NewFile] = "M3 3H15L21 9V21H3ZM15 3V9H21M12 13V18M9.5 15.5H14.5",
            [XyuiVectorIcon.Open] = "M2 6H6L8 4H21V19H2ZM2 6V19H18L21 4",
            [XyuiVectorIcon.Save] = "M3 3H21V21H3ZM6 3V9H18V3M7 14H17V19H7Z",
            [XyuiVectorIcon.Undo] = "M6 5L3 8L6 11M3 8H12C16 8 18 10 18 14V17",
            [XyuiVectorIcon.Redo] = "M18 5L21 8L18 11M21 8H12C8 8 6 10 6 14V17",
            [XyuiVectorIcon.Play] = "M5 3L19 12L5 21Z",
            [XyuiVectorIcon.Stop] = "M5 5H19V19H5Z",
            [XyuiVectorIcon.Select] = "M5 3L17 14L11.7 15.1L14.7 20.2L12.4 21.5L9.4 16.4L5.8 20.2Z",
            [XyuiVectorIcon.BoxSelect] = "M4 5H17V16H4ZM14.5 14L20 19.2L17.4 19.8L18.9 22L17.2 23L15.8 20.7L14.1 22.4Z",
            [XyuiVectorIcon.Move] = "M12 3V21M12 3L9 6M12 3L15 6M12 21L9 18M12 21L15 18M3 12H21M3 12L6 9M3 12L6 15M21 12L18 9M21 12L18 15M12 10.5A1.5 1.5 0 1 0 12 13.5A1.5 1.5 0 1 0 12 10.5",
            [XyuiVectorIcon.Rotate] = "M12 10.5A1.5 1.5 0 1 0 12 13.5A1.5 1.5 0 1 0 12 10.5M6.2 9.5A6.6 6.6 0 0 1 15.8 5.3M15.8 5.3L15.2 2.8M15.8 5.3L13.2 5.9M17.8 14.5A6.6 6.6 0 0 1 8.2 18.7M8.2 18.7L8.8 21.2M8.2 18.7L10.8 18.1",
            [XyuiVectorIcon.Scale] = "M8 4H4V8M4 4L9 9M16 4H20V8M20 4L15 9M8 20H4V16M4 20L9 15M16 20H20V16M20 20L15 15M9 9H15V15H9Z",
            [XyuiVectorIcon.Focus] = "M4 9V4H9M15 4H20V9M20 15V20H15M9 20H4V15M12 9.8A2.2 2.2 0 1 0 12 14.2A2.2 2.2 0 0 0 12 9.8M12 11.2A0.8 0.8 0 1 0 12 12.8A0.8 0.8 0 0 0 12 11.2",
            [XyuiVectorIcon.ViewAll] = "M3 7V3H7M17 3H21V7M21 17V21H17M7 21H3V17M7 7H17V17H7Z",
            [XyuiVectorIcon.Pan] = "M8 11V5.8C8 5 8.6 4.4 9.4 4.4C10.2 4.4 10.8 5 10.8 5.8V11M10.8 10.5V4.8C10.8 4 11.4 3.4 12.2 3.4C13 3.4 13.6 4 13.6 4.8V11M13.6 10.5V6.2C13.6 5.4 14.2 4.8 15 4.8C15.8 4.8 16.4 5.4 16.4 6.2V12.5M8 11L6.8 9.8C6.2 9.2 5.2 9.2 4.7 9.9C4.2 10.5 4.3 11.3 4.8 11.9L9.3 17.8C10.1 18.8 11.3 19.4 12.6 19.4H15.1C17.5 19.4 19.4 17.5 19.4 15.1V11.4",
            [XyuiVectorIcon.Orbit] = "M4.7 15.4C3.5 13.2 6.1 10.1 10.5 8.2C14.9 6.4 19.5 6.5 20.3 8.6C21.1 10.8 18.6 13.9 14.2 15.7C9.8 17.6 5.8 17.4 4.7 15.4M17.8 6.8L20.7 7.4L19.4 10M12 10.2A1.8 1.8 0 1 0 12 13.8A1.8 1.8 0 1 0 12 10.2",
            [XyuiVectorIcon.Snap] = "M7 4V11C7 13.8 9.2 16 12 16C14.8 16 17 13.8 17 11V4M7 4H10.5V11C10.5 11.8 11.2 12.5 12 12.5C12.8 12.5 13.5 11.8 13.5 11V4H17M4 4H7M17 4H20M4 8H7M17 8H20",
        };

    public static StreamGeometry Create(XyuiVectorIcon icon) => StreamGeometry.Parse(PathData[icon]);

    public static XyuiVectorIconMetrics GetMetrics(XyuiVectorIcon icon) =>
        new(LogicalIconSize, Create(icon).Bounds, new global::Avalonia.Vector(0, 0));

    public static ResourceDictionary CreateResources()
    {
        var resources = new ResourceDictionary();
        foreach (var icon in PathData.Keys) resources[$"XY.Icon.{icon}"] = Create(icon);
        return resources;
    }
}
