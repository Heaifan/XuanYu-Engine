using System.Text;

namespace XuanYu.Render.Abstractions.Diagnostics;

internal sealed class RendererEventFileSink : IAsyncDisposable
{
    readonly string _directory;
    readonly string _stem;
    readonly long _maxBytes;
    readonly int _retained;
    StreamWriter? _writer;
    long _bytes;

    public RendererEventFileSink(string directory, string stem, long maxBytes, int retained)
    {
        _directory = directory;
        _stem = stem;
        _maxBytes = maxBytes;
        _retained = retained;
        Directory.CreateDirectory(directory);
    }

    public string InitialPath => SegmentPath(0);

    public async ValueTask WriteAsync(RendererEvent item)
    {
        var json = System.Text.Json.JsonSerializer.Serialize(item, RendererEventJson.Options);
        var length = Encoding.UTF8.GetByteCount(json) + 1;
        if (length > _maxBytes) throw new InvalidOperationException("Renderer event exceeds segment size limit.");
        if (_writer is not null && _bytes + length > _maxBytes) await RotateAsync().ConfigureAwait(false);
        _writer ??= Open();
        await _writer.WriteLineAsync(json).ConfigureAwait(false);
        _bytes += length;
        if (item.Level >= RendererEventLevel.Error) await _writer.FlushAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_writer is null) return;
        await _writer.FlushAsync().ConfigureAwait(false);
        await _writer.DisposeAsync().ConfigureAwait(false);
        _writer = null;
    }

    StreamWriter Open()
    {
        var stream = new FileStream(InitialPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 8192, true);
        _bytes = stream.Length;
        return new StreamWriter(stream, new UTF8Encoding(false)) { NewLine = "\n" };
    }

    async ValueTask RotateAsync()
    {
        await DisposeAsync().ConfigureAwait(false);
        for (var index = _retained - 1; index > 0; index--)
        {
            var source = SegmentPath(index - 1);
            var target = SegmentPath(index);
            if (File.Exists(target)) File.Delete(target);
            if (File.Exists(source)) File.Move(source, target);
        }
        if (File.Exists(InitialPath)) File.Delete(InitialPath);
        _bytes = 0;
    }

    string SegmentPath(int index) => Path.Combine(_directory, $"{_stem}-{index:D3}.jsonl");
}
