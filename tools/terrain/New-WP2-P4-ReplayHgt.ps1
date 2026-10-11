[CmdletBinding()]
param(
    [string]$OutputDirectory = '',
    [int]$Size = 3601
)

$ErrorActionPreference = 'Stop'
$repo = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$safeRoot = Join-Path $repo 'artifacts\diagnostics\wp2-p4-r3'
if ([string]::IsNullOrWhiteSpace($OutputDirectory)) { $OutputDirectory = $safeRoot }
$target = [IO.Path]::GetFullPath($OutputDirectory)
$safeRootFull = [IO.Path]::GetFullPath($safeRoot).TrimEnd([IO.Path]::DirectorySeparatorChar)
$isSafeTarget = $target.Equals($safeRootFull, [StringComparison]::OrdinalIgnoreCase) -or
    $target.StartsWith($safeRootFull + [IO.Path]::DirectorySeparatorChar,
        [StringComparison]::OrdinalIgnoreCase)
if (-not $isSafeTarget) {
    throw "输出目录必须位于忽略的诊断目录：$safeRoot"
}
if ($Size -lt 3 -or $Size % 2 -eq 0) { throw 'HGT 边长必须是大于 2 的奇数。' }
New-Item -ItemType Directory -Force -Path $target | Out-Null
$hgtPath = Join-Path $target 'n23e121.hgt'
$manifestPath = Join-Path $target 'n23e121.manifest.json'
if (Test-Path -LiteralPath $hgtPath) { throw "拒绝覆盖已存在文件：$hgtPath" }

$generator = @'
using System;
using System.Buffers.Binary;
public static class Wp2ReplayHgtGenerator
{
    public static byte[] Create(int size)
    {
        var bytes = new byte[checked(size * size * 2)];
        for (var row = 0; row < size; row++)
        for (var x = 0; x < size; x++)
        {
            var y = size - 1 - row;
            var h = -180.0 + (x - 1800) * 0.08 + y * 0.04;
            if (x >= 1050 && x <= 1080) h += 1100.0;
            h += Radial(x, y, 2250, 1350, 46, 2300.0);
            h += Radial(x, y, 2860, 2700, 96, -1700.0);
            h += Radial(x, y, 520, 3150, 150, -420.0);
            var value = x >= 3000 && x < 3012 && y >= 1650 && y < 1662
                ? short.MinValue : (short)Math.Clamp(Math.Round(h), -32000, 32000);
            BinaryPrimitives.WriteInt16BigEndian(bytes.AsSpan((row * size + x) * 2, 2), value);
        }
        return bytes;
    }

    static double Radial(int x, int y, int cx, int cy, int radius, double peak)
    {
        var dx = x - cx; var dy = y - cy;
        var distance = Math.Sqrt(dx * dx + dy * dy);
        return distance >= radius ? 0 : peak * (1 - distance / radius);
    }
}
'@
Add-Type -TypeDefinition $generator -Language CSharp
$bytes = [Wp2ReplayHgtGenerator]::Create($Size)
[IO.File]::WriteAllBytes($hgtPath, $bytes)
$sha = (Get-FileHash -LiteralPath $hgtPath -Algorithm SHA256).Hash
$manifest = [ordered]@{
    source = 'deterministic synthetic fixture; not NASADEM'
    tileId = 'n23e121'
    width = $Size
    height = $Size
    encoding = 'signed Int16 Big-Endian'
    rowOrder = 'north-to-south'
    bytes = $bytes.Length
    sha256 = $sha
    randomSeed = 'none; analytic height function'
    features = @('north-south baseline slope', '1100m ridge step x=1050..1080',
        '2300m isolated peak', '-1700m deep valley', '-420m negative basin',
        '12x12 NoData patch encoded as Int16.MinValue')
}
$manifest | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $manifestPath -Encoding utf8
Write-Output "HGT=$hgtPath"
Write-Output "BYTES=$($bytes.Length)"
Write-Output "SHA256=$sha"
Write-Output "MANIFEST=$manifestPath"
