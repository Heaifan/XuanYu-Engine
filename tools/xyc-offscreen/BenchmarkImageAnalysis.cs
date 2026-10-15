using XuanYu.Core.Diagnostics;
using XuanYu.World.Terrain.Source;
using XuanYu.Render.Abstractions;

internal sealed record ImageComparison(double MeanAbsoluteRgb, double ChangedPixelPercent,
    double TerrainMaskChangedPercent, int TerrainPixels, double Coverage);

internal static class BenchmarkImageAnalysis
{
    const byte SkyR = 89, SkyG = 140, SkyB = 204;

    public static int CountTerrainPixels(byte[] rgba)
    {
        var count = 0;
        for (var i = 0; i < rgba.Length; i += 4)
            if (rgba[i] != SkyR || rgba[i + 1] != SkyG || rgba[i + 2] != SkyB) count++;
        return count;
    }

    public static ImageComparison Compare(byte[] low, byte[] high, int width, int height, string diffPath)
    {
        var diff = new byte[low.Length]; var changed = 0; var maskChanged = 0;
        long absolute = 0; var pixels = width * height;
        for (var p = 0; p < pixels; p++)
        {
            var i = p * 4; var delta = 0;
            for (var c = 0; c < 3; c++) { var d = Math.Abs(low[i + c] - high[i + c]); absolute += d; delta = Math.Max(delta, d); }
            if (delta > 8) changed++;
            if (IsSky(low, i) != IsSky(high, i)) maskChanged++;
            diff[i] = diff[i + 1] = diff[i + 2] = (byte)Math.Min(255, delta * 3); diff[i + 3] = 255;
        }
        File.WriteAllBytes(diffPath, VulkanPngEncoder.EncodeRgba(width, height, diff));
        return new(absolute / (double)(pixels * 3), changed * 100d / pixels,
            maskChanged * 100d / pixels, CountTerrainPixels(high), CountTerrainPixels(high) * 100d / pixels);
    }

    static bool IsSky(byte[] data, int i) => data[i] == SkyR && data[i + 1] == SkyG && data[i + 2] == SkyB;

    public static (double WorldP95, double VisualP95) HeightErrorP95(
        TerrainElevationTile source, TerrainHeightfield field, double exaggeration)
    {
        var errors = new List<double>(); const int probes = 64;
        for (var y = 0; y < probes; y++)
        for (var x = 0; x < probes; x++)
        {
            var sx = (int)Math.Round((x + 0.5) * (source.Width - 1) / probes);
            var sy = (int)Math.Round((y + 0.5) * (source.Height - 1) / probes);
            if (source.Raster.NoDataMask[sy * source.Width + sx]) continue;
            var fx = sx * (field.Width - 1d) / (source.Width - 1);
            var fy = (source.Height - 1 - sy) * (field.Height - 1d) / (source.Height - 1);
            if (!TryBilinear(field, fx, fy, out var reconstructed)) continue;
            errors.Add(Math.Abs(source.ElevationAt(sy, sx) - reconstructed));
        }
        if (errors.Count == 0) return (double.NaN, double.NaN);
        var ordered = errors.Order().ToArray(); var index = (int)Math.Ceiling(0.95 * ordered.Length) - 1;
        return (ordered[index], ordered[index] * exaggeration);
    }

    static bool TryBilinear(TerrainHeightfield field, double x, double y, out double value)
    {
        var x0 = Math.Clamp((int)x, 0, field.Width - 1); var y0 = Math.Clamp((int)y, 0, field.Height - 1);
        var x1 = Math.Min(x0 + 1, field.Width - 1); var y1 = Math.Min(y0 + 1, field.Height - 1);
        var indices = new[] { y0 * field.Width + x0, y0 * field.Width + x1,
            y1 * field.Width + x0, y1 * field.Width + x1 };
        if (indices.Any(i => field.NoDataMask[i])) { value = 0; return false; }
        var tx = x - x0; var ty = y - y0;
        var top = field.ElevationAt(y0, x0) * (1 - tx) + field.ElevationAt(y0, x1) * tx;
        var bottom = field.ElevationAt(y1, x0) * (1 - tx) + field.ElevationAt(y1, x1) * tx;
        value = top * (1 - ty) + bottom * ty; return true;
    }
}
