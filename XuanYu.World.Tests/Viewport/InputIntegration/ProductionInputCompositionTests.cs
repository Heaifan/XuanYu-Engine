using XuanYu.Editor.Input;
using XuanYu.Editor.Input.Lifecycle;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class ProductionInputCompositionTests
{
    [Fact]
    public void ProductionComposition_creates_single_router_and_lifecycle()
    {
        var composition = Create();

        Assert.Same(composition.Router.Lifecycle, composition.Lifecycle);
        Assert.Equal(8, composition.Consumers.Count);
        Assert.Equal(8, composition.Consumers.Select(x => x.Owner).Distinct().Count());
        Assert.Contains(GestureOwner.Navigation, composition.Consumers.Select(x => x.Owner));
        Assert.DoesNotContain(GestureOwner.None, composition.Consumers.Select(x => x.Owner));
        Assert.DoesNotContain(GestureOwner.SnapInteractionHelper, composition.Consumers.Select(x => x.Owner));
    }

    [Fact]
    public void Production_sink_dispatches_to_single_consumer()
    {
        var consumers = Enumerable.Range(0, 7).Select(i => new Probe((GestureOwner)(i + 1))).ToArray();
        var composition = new ViewportInputComposition(consumers, new CaptureProbe());

        composition.Sink.Handle(Event());

        Assert.Equal(GestureOwner.Camera, composition.Router.State.Owner);
        Assert.Equal(1, consumers.Sum(x => x.BeginCount));
    }

    [Fact]
    public void Production_capture_coordinator_is_the_composition_instance()
    {
        var composition = Create();

        Assert.Same(composition.CaptureCoordinator, composition.Router.CaptureCoordinator);
    }

    [Fact]
    public void UiVm_owns_one_production_input_composition()
    {
        var vm = new XuanYu.Editor.UI.UiVm(null, () => true, seedInitialScene: false);

        Assert.Equal(8, vm.ViewportInput.Consumers.Count);
        Assert.Same(vm.ViewportInput.Lifecycle, vm.ViewportInput.Router.Lifecycle);
        Assert.Same(vm.ViewportInput.CaptureCoordinator, vm.ViewportInput.Router.CaptureCoordinator);
    }

    static ViewportInputComposition Create() => new(
        Enum.GetValues<GestureOwner>().Where(x => x is not GestureOwner.None and not GestureOwner.SnapInteractionHelper)
            .Select(x => new Probe(x)), new CaptureProbe());

    static EditorPointerEvent Event() => new(EditorPointerEventKind.Pressed, new(1, 2),
        EditorPointerButtons.Left, EditorPointerModifiers.None, 0, 1, new("test"), 1);

    sealed class Probe(GestureOwner owner) : IViewportInputConsumer
    {
        public GestureOwner Owner => owner;
        public int BeginPriority => owner == GestureOwner.Camera ? 100 : 0;
        public int BeginCount { get; private set; }
        public bool CanBegin(EditorPointerEvent p, ViewportGestureState s) => p.Kind == EditorPointerEventKind.Pressed;
        public ViewportInputDispatchResult Handle(EditorPointerEvent p, ViewportGestureState s) =>
            new(ViewportInputDispatchKind.Captured);
        public void Begin(ViewportGestureContext c) => BeginCount++;
        public void Update(ViewportGestureContext c) { }
        public void Commit(ViewportGestureContext c) { }
        public void Cancel(ViewportCancellationContext c) { }
    }

    sealed class CaptureProbe : IViewportPointerCaptureCoordinator
    {
        public void Capture(long pointerId, GestureOwner owner) { }
        public void Release(long pointerId, GestureOwner owner) { }
    }
}
