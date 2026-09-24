namespace XuanYu.Editor.Input;

public enum ViewportInputDispatchKind { Ignored, Observed, Handled, Captured, Released, Cancelled }

public readonly record struct ViewportInputDispatchResult(ViewportInputDispatchKind Kind)
{
    public bool ClaimsGesture => Kind is ViewportInputDispatchKind.Handled or ViewportInputDispatchKind.Captured;
    public static ViewportInputDispatchResult Ignored => new(ViewportInputDispatchKind.Ignored);
    public static ViewportInputDispatchResult Observed => new(ViewportInputDispatchKind.Observed);
    public static ViewportInputDispatchResult Handled => new(ViewportInputDispatchKind.Handled);
    public static ViewportInputDispatchResult Captured => new(ViewportInputDispatchKind.Captured);
}
