using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Terrain.Import;

public static class TerrainSourceImport
{
    public static TerrainTileSet ReadMany(IEnumerable<string> paths,
        IProgress<TerrainImportProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var sources = paths?.ToArray() ?? throw new ArgumentNullException(nameof(paths));
        if (sources.Length == 0) throw new TerrainSourceReadException("未选择地形源。");
        var tiles = new List<TerrainElevationTile>(sources.Length);
        progress?.Report(new(TerrainImportStage.Preparing, 0, sources.Length, 0, "准备导入"));
        for (var index = 0; index < sources.Length; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var mapped = new MappingProgress(progress, index, sources.Length);
            tiles.Add(ReadOne(sources[index], mapped, cancellationToken));
            mapped.Report(new(TerrainImportStage.Activating, 1, 1, 100, "当前文件读取完成"));
        }
        cancellationToken.ThrowIfCancellationRequested();
        var result = TerrainTileSet.Create(tiles);
        progress?.Report(new(TerrainImportStage.BuildingTerrain, 1, 1, 95, "正在建立 Tile 集合"));
        progress?.Report(new(TerrainImportStage.Activating, 1, 1, 100, "导入完成"));
        return result;
    }

    static TerrainElevationTile ReadOne(string path, IProgress<TerrainImportProgress> progress,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(path);
        if (extension.Equals(".zip", StringComparison.OrdinalIgnoreCase))
            return HgtTerrainElevationTileReader.ReadZip(path, progress);
        if (!extension.Equals(".hgt", StringComparison.OrdinalIgnoreCase))
            throw new TerrainSourceReadException($"不支持的地形扩展名：{extension}。");
        using var stream = File.OpenRead(path);
        cancellationToken.ThrowIfCancellationRequested();
        return new HgtTerrainElevationTileReader().Read(stream, path, progress);
    }

    sealed class MappingProgress(IProgress<TerrainImportProgress>? target, int index, int count)
        : IProgress<TerrainImportProgress>
    {
        double _last;
        public void Report(TerrainImportProgress value)
        {
            var percentage = Math.Clamp(10 + (index + value.Percentage / 100) * 80 / count, 0, 90);
            _last = Math.Max(_last, percentage);
            target?.Report(value with { Percentage = _last });
        }
    }
}
