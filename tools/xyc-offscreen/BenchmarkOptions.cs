internal sealed record BenchmarkOptions(string Source, string OutputDirectory, int Replicate,
    int[] Samples, string[] Poses, int WarmupFrames, int MeasuredFrames)
{
    public static BenchmarkOptions Parse(string[] args)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < args.Length; i++)
        {
            if (!args[i].StartsWith("--", StringComparison.Ordinal) || i + 1 >= args.Length)
                throw new ArgumentException("Benchmark 参数必须采用 --name value 形式。");
            values[args[i][2..]] = args[++i];
        }
        string Need(string key) => values.TryGetValue(key, out var value)
            ? value : throw new ArgumentException($"缺少参数 --{key}");
        var samples = values.TryGetValue("samples", out var rawSamples)
            ? rawSamples.Split(',').Select(int.Parse).ToArray() : [513, 1025, 2049];
        if (samples.Any(x => x is not (513 or 1025 or 2049)) || samples.Distinct().Count() != samples.Length)
            throw new ArgumentException("--samples 只接受去重后的 513、1025、2049。");
        var poses = values.TryGetValue("pose", out var rawPoses)
            ? rawPoses.Split(',', StringSplitOptions.RemoveEmptyEntries) : ["all"];
        var warmup = values.TryGetValue("warmup", out var rawWarmup) ? int.Parse(rawWarmup) : 2;
        var frames = values.TryGetValue("frames", out var rawFrames) ? int.Parse(rawFrames) : 30;
        if (warmup < 0 || frames < 1) throw new ArgumentOutOfRangeException(nameof(args), "warmup/frames 范围非法");
        return new(Path.GetFullPath(Need("source")), Path.GetFullPath(Need("out")),
            values.TryGetValue("replicate", out var replica) ? int.Parse(replica) : 1,
            samples, poses, warmup, frames);
    }
}
