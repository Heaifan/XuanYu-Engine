namespace XuanYu.Render.Vulkan.Render.Frame;

internal sealed class VulkanFrameResourceRetirement(int frameCount)
{
    readonly HashSet<IDisposable>[] _leases = Enumerable.Range(0, frameCount)
        .Select(_ => new HashSet<IDisposable>(ReferenceEqualityComparer.Instance)).ToArray();
    readonly List<(IDisposable Resource, Action? OnDisposed)> _retired = [];

    internal void Track(int frameIndex, IDisposable resource)
        => _leases[frameIndex].Add(resource);

    internal void Retire(IDisposable resource, Action? onDisposed = null)
    {
        if (_leases.Any(x => x.Contains(resource))) _retired.Add((resource, onDisposed));
        else Dispose(resource, onDisposed);
    }

    internal void Complete(int frameIndex)
    {
        _leases[frameIndex].Clear();
        for (var i = _retired.Count - 1; i >= 0; i--)
        {
            var (resource, onDisposed) = _retired[i];
            if (_leases.Any(x => x.Contains(resource))) continue;
            _retired.RemoveAt(i);
            Dispose(resource, onDisposed);
        }
    }

    internal string Describe() =>
        $"frameLeases=[{string.Join(',', _leases.Select(x => x.Count))}];pendingRetirement={_retired.Count}";

    static void Dispose(IDisposable resource, Action? onDisposed)
    {
        resource.Dispose();
        onDisposed?.Invoke();
    }
}
