namespace XuanYu.World.Tests.Render;

public sealed class TerrainGpuCacheRetirementSimulationTests
{
    [Fact]
    public void Budget_trim_keeps_buffers_alive_while_recorded_command_buffers_reference_them()
    {
        var cache = new SimulatedCache(10);
        var recorder = new SimulatedRecorder();
        cache.BeginRecording();
        var first = new SimulatedBuffer(8);
        cache.Add("first", first);
        recorder.Bind(first);
        var second = new SimulatedBuffer(8);
        cache.Add("second", second);
        recorder.Bind(second);

        Assert.False(first.Disposed);
        Assert.Contains(first, recorder.ReferencedBuffers);
        cache.CompleteRecording(recorder.ReferencedBuffers);
        cache.BeginRecording();
        cache.Add("next", new SimulatedBuffer(8));
        cache.CompleteRecording([cache.Current("next")]);
        Assert.True(first.Disposed);
        Assert.True(second.Disposed);
    }

    sealed class SimulatedBuffer(ulong bytes)
    {
        public ulong Bytes { get; } = bytes;
        public bool Disposed { get; private set; }
        public void Dispose() => Disposed = true;
    }

    sealed class SimulatedRecorder
    {
        public List<SimulatedBuffer> ReferencedBuffers { get; } = [];
        public void Bind(SimulatedBuffer buffer) => ReferencedBuffers.Add(buffer);
    }

    sealed class SimulatedCache(ulong budget)
    {
        readonly Dictionary<string, SimulatedBuffer> _resources = [];
        readonly LinkedList<string> _lru = [];
        readonly HashSet<SimulatedBuffer> _recorded = [];
        readonly HashSet<SimulatedBuffer> _retired = [];
        HashSet<SimulatedBuffer>? _recording;

        public void BeginRecording() => _recording = [];
        public SimulatedBuffer Current(string key) => _resources[key];
        public void CompleteRecording(IEnumerable<SimulatedBuffer> current)
        { _recorded.Clear(); _recorded.UnionWith(current); _recording = null; Drain(); }

        public void Add(string key, SimulatedBuffer buffer)
        {
            _resources[key] = buffer;
            _lru.AddLast(key);
            _recording?.Add(buffer);
            var resident = _resources.Values.Aggregate(0UL,
                (total, resource) => total + resource.Bytes);
            while (resident > budget && _resources.Count > 1)
            {
                var oldest = _lru.First!.Value;
                _lru.RemoveFirst();
                _retired.Add(_resources[oldest]);
                _resources.Remove(oldest);
                Drain();
                resident = _resources.Values.Aggregate(0UL,
                    (total, resource) => total + resource.Bytes);
            }
        }

        void Drain()
        {
            foreach (var buffer in _retired.Where(x => !_recorded.Contains(x) &&
                         (_recording is null || !_recording.Contains(x))).ToArray())
            { buffer.Dispose(); _retired.Remove(buffer); }
        }
    }
}
