using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Rendering.Composition;
using Avalonia.VisualTree;

namespace XuanYu.Viewport.CompositionSpike;

public sealed class CompositionSurfaceHost : Control
{
    CompositionSurfaceVisual? _visual;
    Compositor? _compositor;
    CompositionDrawingSurface? _surface;
    VulkanCompositionResources? _resources;
    ICompositionGpuInterop? _interop;
    public event EventHandler<string>? StatusChanged;
    void Report(string text) { Console.WriteLine("[A1.5] " + text); StatusChanged?.Invoke(this, text); }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Initialize();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _ = DisposeResources();
        ElementComposition.SetElementChildVisual(this, null);
        _visual = null;
        _compositor = null;
        base.OnDetachedFromLogicalTree(e);
    }

    async Task DisposeResources()
    { if (_resources is not null) await _resources.DisposeAsync(); _resources = null; _surface?.Dispose(); }

    async void Initialize()
    {
        var owner = ElementComposition.GetElementVisual(this)!;
        _compositor = owner.Compositor;
        _surface = _compositor.CreateDrawingSurface();
        _visual = _compositor.CreateSurfaceVisual();
        _visual.Surface = _surface;
        ElementComposition.SetElementChildVisual(this, _visual);
        _interop = await _compositor.TryGetCompositionGpuInterop();
        if (_interop is null) { Report("FAIL：当前 Avalonia backend 没有 GPU interop"); return; }
        try
        {
            var size = PixelSizeForBounds();
            _resources = VulkanCompositionResources.Create(_interop, size);
            await _resources.PresentAsync(_surface);
            Report("PASS链路候选：" + _resources.DeviceInfo);
        }
        catch (Exception error) { Report("FAIL：Vulkan GPU Composition：" + error); }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == BoundsProperty && _visual is not null)
        {
            _visual.Size = new Vector(Bounds.Width, Bounds.Height);
            Report("SPIKE：Composition Resize=" + Bounds.Size);
            _ = RecreateForResize();
        }
    }

    PixelSize PixelSizeForBounds() => PixelSize.FromSize(Bounds.Size, (VisualRoot as TopLevel)?.RenderScaling ?? 1);

    async Task RecreateForResize()
    {
        if (_interop is null || _surface is null || Bounds.Width <= 0 || Bounds.Height <= 0) return;
        var old = _resources;
        try
        {
            _resources = VulkanCompositionResources.Create(_interop, PixelSizeForBounds());
            await _resources.PresentAsync(_surface);
            if (old is not null) await old.DisposeAsync();
            Report("GPU Composition Resize 完成=" + PixelSizeForBounds());
        }
        catch (Exception error) { Report("FAIL：Resize Composition：" + error.Message); }
    }
}
