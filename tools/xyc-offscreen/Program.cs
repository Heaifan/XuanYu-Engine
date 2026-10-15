if (args.Length > 0 && args[0] == "--benchmark")
    return TerrainBenchmarkRunner.Run(BenchmarkOptions.Parse(args[1..]));
if (args.Length > 0 && args[0] == "--aggregate")
    return BenchmarkReportBuilder.Run(args[1..]);
return OffscreenSingleRender.Run(args);
